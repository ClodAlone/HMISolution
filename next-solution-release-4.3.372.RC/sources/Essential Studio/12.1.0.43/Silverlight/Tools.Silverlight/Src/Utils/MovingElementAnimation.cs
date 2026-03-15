#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media.Animation;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Media;

namespace Syncfusion.Silverlight.Tools.Controls
{
    /// <summary>
    /// Specifies interface that you should implement to provide custom values for animation steps.
    /// </summary>
    public interface IDoubleAnimationStepValueProvider
    {
        /// <summary>
        /// Get Animation step value 
        /// </summary>
        /// <param name="totalseconds">passing totalseconds value</param>
        /// <param name="from">passing from value</param>
        /// <param name="current">passing current value</param>
        /// <param name="duration">passing duration value</param>
        /// <returns>The intermediate animation position.</returns>
        double GetAnimationStepValue(double totalseconds, double from, double current, double duration);
    }

    /// <summary>
    /// Lets you animate elements using custom animation equations.
    /// </summary>
    /// <remarks>
    /// The <see cref="MoveToPoint"/> method lets you to move an element from the current to a new position, using an 
    /// algorithm defined by your custom <see cref="IDoubleAnimationStepValueProvider"/> implementation. Note that this 
    /// type assumes that the bound <see cref="UIElement"/> is parented by a Canvas.
    /// </remarks>
    public class MovingElementAnimation
    {
        DispatcherTimer timer = new DispatcherTimer();

        /// <summary>
        /// Gets returns the UIElement that this instance is bound to.
        /// </summary>
        public UIElement UIElement { get; private set; }

        double m_beginXOffset = 0;
        double m_beginYOffset = 0;
        double m_duration = 0;
        double m_timeStep = 0;
        double m_endXOffset = 0;
        double m_endYOffset = 0;

        /// <summary>
        /// Gets returns the custom <see cref="IDoubleAnimationStepValueProvider"/> instance.
        /// </summary>
        public IDoubleAnimationStepValueProvider StepValueProvider { get; private set; }

        /// <summary>
        /// Fired after an animation is completed.
        /// </summary>
        public event EventHandler AfterAnimation;

        /// <summary>
        /// Creates a new instance with the specified params.
        /// </summary>
        /// <param name="element">The UIElement to bind to.</param>
        /// <param name="duration">The duration for the animation.</param>
        /// <param name="stepValueProvider">The custom <see cref="IDoubleAnimationStepValueProvider"/> implementation
        /// that will be used to arrive at the step values.</param>
        public MovingElementAnimation(UIElement element, double duration, IDoubleAnimationStepValueProvider stepValueProvider)
        {
            if (element == null)
            {
                throw new ArgumentException("The element specified cannot be null in MovingElementAnimation constructor.");
            }

            if (stepValueProvider == null)
            {
                throw new ArgumentException("The IDoubleAnimationStepValueProvider specified cannot be null in MovingElementAnimation constructor.");
            }

            UIElement = element;
            m_duration = duration;
            this.StepValueProvider = stepValueProvider;

            timer.Tick += new EventHandler(this.Part_Tick);
        }

        /// <summary>
        /// Call this method to move the bound UIElement to this position.
        /// </summary>
        /// <param name="pt">A Point instance in parent Canvas co-ords.</param>
        public void MoveToPoint(Point pt)
        {
            this.MoveToPoint(pt.X, pt.Y);
        }

        /// <summary>
        /// Call this method to move the bound UIElement to this position.
        /// </summary>
        /// <param name="x">The X value in parent Canvas co-ords.</param>
        /// <param name="y">The Y value in parent Canvas co-ords.</param>
        public void MoveToPoint(double x, double y)
        {
            timer.Stop();
            m_timeStep = 0;

            timer.Interval = new TimeSpan(0, 0, 0, 0, (int)m_duration/20);
            timer.Start();
            m_beginXOffset = m_endXOffset;
            m_beginYOffset = m_endYOffset;

            double orgLeft = (double)UIElement.GetValue(Canvas.LeftProperty);
            double orgTop = (double)UIElement.GetValue(Canvas.TopProperty);
            m_endXOffset = x - orgLeft;
            m_endYOffset = y - orgTop; 
        }

        private double CurrentLeft
        {
            get
            {
                double curLeft = (double)UIElement.GetValue(Canvas.LeftProperty);
                if (UIElement.RenderTransform != null && UIElement.RenderTransform is TranslateTransform)
                {
                    TranslateTransform tt = UIElement.RenderTransform as TranslateTransform;
                    curLeft += tt.X;
                }

                return curLeft;
            }
        }

        private double CurrentTop
        {
            get
            {
                double curTop = (double)UIElement.GetValue(Canvas.TopProperty);
                if (UIElement.RenderTransform != null && UIElement.RenderTransform is TranslateTransform)
                {
                    TranslateTransform tt = UIElement.RenderTransform as TranslateTransform;
                    curTop += tt.Y;
                }

                return curTop;
            }
        }

        private void Part_Tick(object sender, EventArgs e)
        {
            ////timer.IsEnabled = false;
            if (m_timeStep <= m_duration)
            {
                this.NextPart();
                ////timer.IsEnabled = true;
            }
            else
            {
                timer.Stop();
                if (this.AfterAnimation != null)
                {
                    this.AfterAnimation(this, EventArgs.Empty);
                }
            }
        }

         private void NextPart()
        {
            TranslateTransform t = null;

            if (UIElement.RenderTransform != null && UIElement.RenderTransform is TranslateTransform)
            {
                t = UIElement.RenderTransform as TranslateTransform;
            }

            if (t == null)
            {
                UIElement.RenderTransform = t = new TranslateTransform();
            }

            t.ClearValue(TranslateTransform.XProperty);
            t.ClearValue(TranslateTransform.YProperty);

            t.X = this.StepValueProvider.GetAnimationStepValue(m_timeStep, m_beginXOffset, m_endXOffset - m_beginXOffset, m_duration);
            t.Y = this.StepValueProvider.GetAnimationStepValue(m_timeStep, m_beginYOffset, m_endYOffset - m_beginYOffset, m_duration);

            m_timeStep+=20;
        }
    }
}