// <copyright file="PopupResizeThumb.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents popup resize Thumb control.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class PopupResizeThumb : Control
    {
        #region Private members
        /// <summary>
        /// Represents the popup parent
        /// </summary>
        private FrameworkElement m_popupParent;

        /// <summary>
        /// Represents the Correcting offset
        /// </summary>
        private double m_correctingOffset = 0d;

        /// <summary>
        /// Represents the Placement Coefficient
        /// </summary>
        private int m_placementCoefficient = 1;

        SystemGesture m_systemGesture;

         #if !SyncfusionFramework3_5
        TouchDevice capturedDevice=null;
        #endif

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes static members of the <see cref="PopupResizeThumb"/> class.
        /// </summary>
        static PopupResizeThumb()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PopupResizeThumb), new FrameworkPropertyMetadata(typeof(PopupResizeThumb)));
        }
        #endregion

        #region Overrides

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.UIElement.PreviewMouseLeftButtonDown"/>�routed event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs"/> that contains the event data. The event data reports that the left mouse button was pressed.</param>
        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            var ribbonTouch = VisualUtils.FindAncestor(this, typeof(Ribbon)) as Ribbon;
            if (e.StylusDevice == null || (ribbonTouch!=null && !ribbonTouch.EnableTouch))
            {
                CorrectVerticalOffset();
                base.OnPreviewMouseLeftButtonDown(e);
            }
        }

         #if !SyncfusionFramework3_5
        protected override void OnPreviewTouchDown(TouchEventArgs e)
        {
            var ribbonTouch = VisualUtils.FindAncestor(this, typeof(Ribbon)) as Ribbon;
            if (ribbonTouch != null && ribbonTouch.EnableTouch && m_systemGesture == SystemGesture.Tap)
            {
                CorrectVerticalOffset();
                base.OnPreviewTouchDown(e);
            }
        }
        #endif

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.UIElement.MouseLeftButtonDown"/>�routed event is raised on this element. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs"/> that contains the event data. The event data reports that the left mouse button was pressed.</param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            var ribbonTouch = VisualUtils.FindAncestor(this, typeof(Ribbon)) as Ribbon;
            if (e.StylusDevice == null || (ribbonTouch!=null && !ribbonTouch.EnableTouch)) 
            {
                Mouse.Capture(this);
                base.OnMouseLeftButtonDown(e);
            }
        }

         #if !SyncfusionFramework3_5
        protected override void OnTouchDown(TouchEventArgs e)
        {
            var ribbonTouch = VisualUtils.FindAncestor(this, typeof(Ribbon)) as Ribbon;
            if (ribbonTouch != null && ribbonTouch.EnableTouch && m_systemGesture == SystemGesture.Tap)
            {
                capturedDevice = e.TouchDevice;
                if (capturedDevice != null)
                    capturedDevice.Capture(this);
                base.OnTouchDown(e);
            }
        }
        #endif

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.UIElement.MouseLeftButtonUp"/>�routed event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs"/> that contains the event data. The event data reports that the left mouse button was released.</param>
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            var ribbonTouch = VisualUtils.FindAncestor(this, typeof(Ribbon)) as Ribbon;
            if (e.StylusDevice == null || (ribbonTouch!=null && !ribbonTouch.EnableTouch))
            {
                Mouse.Capture(null);
                base.OnMouseLeftButtonUp(e);
            }
        }

         #if !SyncfusionFramework3_5
        protected override void OnTouchUp(TouchEventArgs e)
        {
            var ribbonTouch = VisualUtils.FindAncestor(this, typeof(Ribbon)) as Ribbon;
            if (ribbonTouch != null && ribbonTouch.EnableTouch && m_systemGesture == SystemGesture.Tap)
            {
                if (capturedDevice != null)
                    capturedDevice.Capture(null);
                base.OnTouchUp(e);
            }
        }
    #endif

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.MouseMove"/>�attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseEventArgs"/> that contains the event data.</param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            var ribbonTouch = VisualUtils.FindAncestor(this, typeof(Ribbon)) as Ribbon;
            if (e.StylusDevice == null || (ribbonTouch!=null && !ribbonTouch.EnableTouch))
            {
                if (e.LeftButton == MouseButtonState.Pressed && m_popupParent != null)
                {
                    Point point = e.GetPosition(m_popupParent);
                    double tempX = point.X + 10d;
                    double tempY = point.Y + 10d - m_correctingOffset;

                    if (Math.Abs(tempX) > m_popupParent.ActualWidth + 10d)
                    {
                        HorizontalOffset = Math.Abs(tempX);
                    }

                    if (tempY * m_placementCoefficient > m_popupParent.ActualHeight + 10d)
                    {
                        VerticalOffset = Math.Abs(tempY);
                    }
                }

                base.OnMouseMove(e);
            }
        }

         #if !SyncfusionFramework3_5
        protected override void OnTouchMove(TouchEventArgs e)
        {
            var ribbonTouch = VisualUtils.FindAncestor(this, typeof(Ribbon)) as Ribbon;
            if (ribbonTouch != null && ribbonTouch.EnableTouch)
            {
                Point point = e.GetTouchPoint(m_popupParent).Position;
                double tempX = point.X + 10d;
                double tempY = point.Y + 10d - m_correctingOffset;

                if (Math.Abs(tempX) > m_popupParent.ActualWidth + 10d)
                {
                    HorizontalOffset = Math.Abs(tempX);
                }

                if (tempY * m_placementCoefficient > m_popupParent.ActualHeight + 10d)
                {
                    VerticalOffset = Math.Abs(tempY);
                }              
            }
            base.OnTouchMove(e);
        }

        #endif
        /// <summary>
        /// Invoked when the parent of this element in the visual tree is changed. Overrides <see cref="M:System.Windows.UIElement.OnVisualParentChanged(System.Windows.DependencyObject)"/>.
        /// </summary>
        /// <param name="oldParent">The old parent element. May be null to indicate that the element did not have a visual parent previously.</param>
        protected override void OnVisualParentChanged(DependencyObject oldParent)
        {
            base.OnVisualParentChanged(oldParent);
            FrameworkElement fe = VisualUtils.FindRootVisual(this as Visual) as FrameworkElement;
            Popup popup = fe.Parent as Popup;
            if (popup != null)
            {
                m_popupParent = popup.TemplatedParent as FrameworkElement;
            }
        }

        protected override void OnStylusSystemGesture(StylusSystemGestureEventArgs e)
        {
            m_systemGesture = e.SystemGesture;
            base.OnStylusSystemGesture(e);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the value of the horizontal offset.
        /// </summary>
        public double HorizontalOffset
        {
            get
            {
                return (double)GetValue(HorizontalOffsetProperty);
            }

            set
            {
                SetValue(HorizontalOffsetProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value of the vertical offset.
        /// </summary>
        public double VerticalOffset
        {
            get
            {
                return (double)GetValue(VerticalOffsetProperty);
            }

            set
            {
                SetValue(VerticalOffsetProperty, value);
            }
        }
        #endregion

        #region Events
        /// <summary>
        /// Event that is raised when HorizontalOffset property is changed.
        /// </summary>
        public event PropertyChangedCallback HorizontalOffsetChanged;

        /// <summary>
        /// Event that is raised when VerticalOffset property is changed.
        /// </summary>
        public event PropertyChangedCallback VerticalOffsetChanged;
        #endregion

        #region Dependency properties
        /// <summary>
        /// Defines horizontal offset of the control.
        /// </summary>
        public static readonly DependencyProperty HorizontalOffsetProperty =
            DependencyProperty.Register("HorizontalOffset", typeof(double), typeof(PopupResizeThumb), new FrameworkPropertyMetadata(0d, new PropertyChangedCallback(OnHorizontalOffsetChanged)));

        /// <summary>
        /// Defines vertical offset of the control.
        /// </summary>
        public static readonly DependencyProperty VerticalOffsetProperty =
            DependencyProperty.Register("VerticalOffset", typeof(double), typeof(PopupResizeThumb), new FrameworkPropertyMetadata(0d, new PropertyChangedCallback(OnVerticalOffsetChanged)));
        #endregion

        #region Implementation
        /// <summary>
        /// Calls OnHorizontalOffsetChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnHorizontalOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PopupResizeThumb instance = (PopupResizeThumb)d;
            instance.OnHorizontalOffsetChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises HorizontalOffsetChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnHorizontalOffsetChanged(DependencyPropertyChangedEventArgs e)
        {
            if (HorizontalOffsetChanged != null)
            {
                HorizontalOffsetChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnVerticalOffsetChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnVerticalOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PopupResizeThumb instance = (PopupResizeThumb)d;
            instance.OnVerticalOffsetChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises VerticalOffsetChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnVerticalOffsetChanged(DependencyPropertyChangedEventArgs e)
        {
            if (VerticalOffsetChanged != null)
            {
                VerticalOffsetChanged(this, e);
            }
        }

        /// <summary>
        /// Corrects vertical offset.
        /// </summary>
        private void CorrectVerticalOffset()
        {
            RibbonGallery rg = m_popupParent as RibbonGallery;
            double actualHeight = 0d;
            if (rg != null && rg.PopupPosition == PopupPosition.Above)
            {
                actualHeight = (rg.VisualMode == RibbonGalleryVisualMode.InRibbon) ? rg.ActualHeight : 0d;
                m_correctingOffset = actualHeight + 20d;
                m_placementCoefficient = -1;
            }
            else
            {
                actualHeight = (rg.VisualMode != RibbonGalleryVisualMode.InRibbon) ? rg.ActualHeight : 0d;
                m_correctingOffset = actualHeight;
                m_placementCoefficient = 1;
            }
        }
        #endregion
    }
}
