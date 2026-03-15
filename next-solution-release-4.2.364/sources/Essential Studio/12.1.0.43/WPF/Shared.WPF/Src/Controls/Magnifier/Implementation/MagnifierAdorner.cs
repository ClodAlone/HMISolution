// <copyright file="MagnifierAdorner.cs" company="Syncfusion">
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
using System.Windows.Documents;
using System.Windows;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Controls;

namespace Syncfusion.Windows.Shared
{
    /// <summary>
    /// An adorner that is being created when <see cref="Magnifier"/> is added to an application. This class is internally used by <see cref="Magnifier"/>.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class MagnifierAdorner : Adorner
    {
        #region Members

        /// <summary>
        /// Define the magnifier
        /// </summary>
        private Magnifier mMagnifier;

        /// <summary>
        /// Define the mouse pointer
        /// </summary>
        private Point mMousePoint;

        #endregion

        #region Constructors
        //// <summary>
        ///// Initializes static members of the <see cref="MagnifierAdorner"/> class.
        ///// </summary>
        // static MagnifierAdorner()
        // {        
        //// }

        /// <summary>
        /// Initializes a new instance of the <see cref="MagnifierAdorner"/> class.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="magnifier">The magnifier.</param>
        public MagnifierAdorner(UIElement element, Magnifier magnifier)
            : base(element)
        {
            mMagnifier = magnifier;
            AddVisualChild(magnifier);
            magnifier.TargetElementChanging += new CoerceValueCallback(Magnifier_TargetElementChanging);

            InputManager.Current.PostProcessInput += new ProcessInputEventHandler(Current_PostProcessInput);
            UpdateMagnifierViewbox();
        }

        #endregion

        #region Private/Internal methods

        /// <summary>
        /// Performs TargetElement changing logic of the Magnifier.
        /// </summary>
        /// <param name="d">The d object.</param>
        /// <param name="baseValue">The base value.</param>
        /// <returns>Return the Magnifier for the control</returns>
        private object Magnifier_TargetElementChanging(DependencyObject d, object baseValue)
        {
            UIElement target = baseValue as UIElement;
            if (target != mMagnifier.TargetElement)
            {
                mMagnifier.TargetElementChanging -= new CoerceValueCallback(Magnifier_TargetElementChanging);
                RemoveVisualChild(mMagnifier);
            }

            return baseValue;
        }

        /// <summary>
        /// Performs mouse move logic.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ProcessInputEventArgs"/> instance containing the event data.</param>
        private void Current_PostProcessInput(object sender, ProcessInputEventArgs e)
        {
            Point pt = Mouse.GetPosition(this);

            if (mMousePoint != pt)
            {
                mMousePoint = pt;
                UpdateMagnifierViewbox();
                InvalidateArrange();
            }
        }

        /// <summary>
        /// Updates value of Magnifier.Viewbox internal property.
        /// </summary>
        internal void UpdateMagnifierViewbox()
        {
            Point ptViewbox = CalculateMagnifierViewboxLocation();

            mMagnifier.Viewbox = new Rect(ptViewbox, mMagnifier.Viewbox.Size);
        }

        /// <summary>
        /// Calculates position of Magnifier's view box taking cursor's location into account.
        /// </summary>
        /// <returns>Return the point</returns>
        private Point CalculateMagnifierViewboxLocation()
        {
            double offsetX = 0, offsetY = 0;

            if (mMagnifier.ActualTargetElement != null)
            {
                Point ptAdorner = Mouse.GetPosition(this);
                Point ptTarget = Mouse.GetPosition(AdornedElement);

                offsetX = ptTarget.X - ptAdorner.X;
                offsetY = ptTarget.Y - ptAdorner.Y;

                FrameworkElement feTarget = mMagnifier.ActualTargetElement as FrameworkElement;
                if (feTarget != null)
                {
                    Panel pnParent = feTarget.Parent as Panel;
                    if (pnParent != null)
                    {
                        //// When target element is placed in some panel, take it into account and correct current location.
                        Point ptParent = Mouse.GetPosition(pnParent);
                        offsetX += ptParent.X - ptTarget.X;
                        offsetY += ptParent.Y - ptTarget.Y;
                    }
                    else
                    {
                        //// When ActualTargetElement is FrameworkElement and has non-zero margin, take it into account.
                        offsetX += feTarget.Margin.Left;
                        offsetY += feTarget.Margin.Top;
                    }
                }
            }

            double left = mMousePoint.X - mMagnifier.Viewbox.Width / 2 + offsetX;
            double top = mMousePoint.Y - mMagnifier.Viewbox.Height / 2 + offsetY;
            return new Point(left, top);
        }

        /// <summary>
        /// Disconnects magnifier from the adorner.
        /// </summary>
        internal void DisconnectMagnifier()
        {
            if (mMagnifier != null)
            {
                RemoveVisualChild(mMagnifier);
            }
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Override for Magnifier usage purposes.
        /// </summary>
        /// <param name="index">Not used in this method.</param>
        /// <returns>
        /// The Magnifier the Adorner is associated with.
        /// </returns>
        protected override Visual GetVisualChild(int index)
        {
            return mMagnifier;
        }

        /// <summary>
        /// Gets the number of visual child elements within this element.
        /// </summary>
        /// <value>Visual Children count</value>
        /// <returns>The number of visual child elements for this element.</returns>
        protected override int VisualChildrenCount
        {
            get
            {
                return 1;
            }
        }

        /// <summary>
        /// Override for Magnifier usage purposes.
        /// </summary>
        /// <param name="constraint">A size to constrain the adorner to.</param>
        /// <returns>
        /// A <see cref="T:System.Windows.Size"/> object representing the amount of layout space needed by the adorner.
        /// </returns>
        protected override Size MeasureOverride(Size constraint)
        {
            mMagnifier.Measure(constraint);
            return base.MeasureOverride(constraint);
        }

        /// <summary>
        /// Override for Magnifier usage purposes.
        /// </summary>
        /// <param name="finalSize">The final area within the parent that this element should use to arrange itself and its children.</param>
        /// <returns>The actual size used.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            Rect rect = new Rect(
                mMousePoint.X - mMagnifier.CurrentSize.Width / 2,
              mMousePoint.Y - mMagnifier.CurrentSize.Height / 2,
              mMagnifier.CurrentSize.Width,
              mMagnifier.CurrentSize.Height);

            mMagnifier.Arrange(rect);

            return base.ArrangeOverride(finalSize);
        }

        #endregion
    }
}
