// <copyright file="GenaricAnimation.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Animation;

namespace Syncfusion.Windows.Shared
{
    /// <summary>
    /// Animation step value provider
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public interface IDoubleAnimationStepValueProvider
    {
        /// <summary>
        /// Gets the animation step value.
        /// </summary>
        /// <param name="totalseconds">The total seconds</param>
        /// <param name="from">From value for animation.</param>
        /// <param name="current">The current.</param>
        /// <param name="duration">The duration.</param>
        /// <returns>Return animation step value</returns>
        double GetAnimationStepValue(double totalseconds, double from, double current, double duration);
    }

    /// <summary>
    /// Double Animation 
    /// </summary>
    public class GenericDoubleAnimation : DoubleAnimationBase
    {
        #region Initialization

        /// <summary>
        /// From value for animation
        /// </summary>
        public static readonly DependencyProperty FromProperty =
    DependencyProperty.Register("From", typeof(double), typeof(GenericDoubleAnimation), new PropertyMetadata(0.0));

        /// <summary>
        /// To Value for animation
        /// </summary>
        public static readonly DependencyProperty ToProperty =
            DependencyProperty.Register("To", typeof(double), typeof(GenericDoubleAnimation), new PropertyMetadata(0.0));

        /// <summary>
        /// StepValue provider for animation
        /// </summary>
        public static readonly DependencyProperty StepValueProviderProperty =
    DependencyProperty.Register("StepValueProvider", typeof(IDoubleAnimationStepValueProvider), typeof(GenericDoubleAnimation), new PropertyMetadata(null));

        #endregion

        #region DP getters & setters

        /// <summary>
        /// Gets or sets from value for the animation.
        /// </summary>
        /// <value>From value.</value>
        public double From
        {
            get
            { 
                return (double)GetValue(FromProperty);
            }

            set
            { 
                SetValue(FromProperty, value); 
            }
        }

        /// <summary>
        /// Gets or sets Ending value for the animation.
        /// </summary>
        /// <value>Ending value for the animation.</value>
        public double To
        {
            get
            { 
                return (double)GetValue(ToProperty);
            }

            set
            { 
                SetValue(ToProperty, value); 
            }
        }

        /// <summary>
        /// Gets or sets the Step value for the animation 
        /// </summary>
        /// <value>The step value provider.</value>
        public IDoubleAnimationStepValueProvider StepValueProvider
        {
            get 
            { 
                return (IDoubleAnimationStepValueProvider)GetValue(StepValueProviderProperty);
            }

            set 
            { 
                SetValue(StepValueProviderProperty, value);
            }
        }

        #endregion

        #region Implementation

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericDoubleAnimation"/> class.
        /// </summary>
        public GenericDoubleAnimation()
        {
        }

        /// <summary>
        /// Gets the current value core.
        /// </summary>
        /// <param name="startValue">The start value.</param>
        /// <param name="targetValue">The target value.</param>
        /// <param name="clock">The clock.</param>
        /// <returns>Return the current step value </returns>
        protected override double GetCurrentValueCore(double startValue, double targetValue, AnimationClock clock)
        {
            try
            {
                if (this.StepValueProvider != null)
                {
                    return this.StepValueProvider.GetAnimationStepValue(clock.CurrentTime.Value.TotalSeconds, From, To - From, Duration.TimeSpan.TotalSeconds);
                }
            }
            catch
            {
            }

            return From;
        }

        /// <summary>
        /// When implemented in a derived class, creates a new instance of the <see cref="T:System.Windows.Freezable"/> derived class.
        /// </summary>
        /// <returns>The new instance.</returns>
        protected override Freezable CreateInstanceCore()
        {
            return new GenericDoubleAnimation();
        }

        #endregion
    }
}
