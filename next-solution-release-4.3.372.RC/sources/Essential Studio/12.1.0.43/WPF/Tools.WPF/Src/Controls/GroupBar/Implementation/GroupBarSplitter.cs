// <copyright file="GroupBarSplitter.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

#region file using
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
using System.Diagnostics;
using Syncfusion.Licensing;
#endregion

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents splitter control for the <see cref="Syncfusion.Windows.Tools.Controls.GroupBar"/>.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class GroupBarSplitter : UserControl
    {
        #region Private members
        /// <summary>
        /// Direction of the dragging.
        /// </summary>
        private DragDirection m_dragDirection = DragDirection.None;

        /// <summary>
        /// Point on the splitter that stores mouse cursor coordinates
        /// when left mouse button is pressed.
        /// </summary>
        private Point m_mouseDownPoint;
        #endregion

        #region Dependency properties
        /// <summary>
        /// Identifies <see cref="DragIncrementProperty"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DragIncrementProperty = DependencyProperty.Register("DragIncrement", typeof(double), typeof(GroupBarSplitter), new UIPropertyMetadata(0d));

        /// <summary>
        /// Identifies <see cref="IsPressedProperty"/> dependency property.
        /// </summary>
        public static readonly DependencyPropertyKey IsPressedPropertyKey = DependencyProperty.RegisterReadOnly("IsPressed", typeof(bool), typeof(GroupBarSplitter), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.None, new PropertyChangedCallback(OnIsPressedChanged)));

        /// <summary>
        /// Identifies <see cref="IsPressedProperty"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsPressedProperty = IsPressedPropertyKey.DependencyProperty;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the logical parent  element of this element.
        /// </summary>
        /// <value></value>
        /// <returns>This element's logical parent.</returns>
        new public GroupBar Parent
        {
            get
            {
                return TemplatedParent as GroupBar;
            }
        }

        /// <summary>
        /// Gets or sets the drag increment.
        /// </summary>
        /// <value>The drag increment.</value>
        public double DragIncrement
        {
            get
            {
                return (double)GetValue(DragIncrementProperty);
            }

            set
            {
                SetValue(DragIncrementProperty, value);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is pressed.
        /// </summary>
        /// <value>
        /// true if this instance is pressed; otherwise, false
        /// </value>
        public bool IsPressed
        {
            get
            {
                return (bool)GetValue(IsPressedProperty);
            }
        }

        /// <summary>
        /// Gets the drag direction.
        /// </summary>
        /// <value>The drag direction.</value>
        public DragDirection DragDirection
        {
            get
            {
                return m_dragDirection;
            }
        }
        #endregion

        #region Events
        /// <summary>
        /// Identifies <see cref="DragIncremented"/> event.
        /// </summary>
        public static readonly RoutedEvent DragIncrementedEvent = EventManager.RegisterRoutedEvent("DragIncremented", RoutingStrategy.Bubble, typeof(EventHandler), typeof(GroupBarSplitter));

        /// <summary>
        /// Bubbling routed event fired when <see cref="DragIncremented"/> changed.
        /// </summary>
        public event RoutedEventHandler DragIncremented
        {
            add
            {
                AddHandler(DragIncrementedEvent, value);
            }

            remove
            {
                RemoveHandler(DragIncrementedEvent, value);
            }
        }
        #endregion

        #region Inititalize
        /// <summary>
        /// Initializes a new instance of the <see cref="GroupBarSplitter"/> class.
        /// </summary>
        public GroupBarSplitter()
            : base()
        {
        }

        /// <summary>
        /// Initializes static members of the <see cref="GroupBarSplitter"/> class.
        /// </summary>
        static GroupBarSplitter()
        {
            EnvironmentTest.ValidateLicense(typeof(GroupBarSplitter));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GroupBarSplitter), new FrameworkPropertyMetadata(typeof(GroupBarSplitter)));
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Raises <see cref="IsPressed"/> event.
        /// </summary>
        /// <param name="d">The object to which the property belongs.</param>
        /// <param name="e">The instance containing the event data.</param>
        private static void OnIsPressedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        /// <summary>
        /// Called when <see cref="DragIncremented"/> changed.
        /// </summary>
        protected virtual void OnDragIncremented()
        {
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.MouseLeftButtonDown"/> routed event is raised on this element. 
        /// Implement this method to add class handling for this event. 
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> that contains the event data. 
        /// The event data reports that the left mouse button was pressed.</param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            CaptureMouse();
            m_mouseDownPoint = e.GetPosition(this);
            SetValue(IsPressedPropertyKey, true);
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.MouseDown"/> attached event reaches 
        /// an element in its route that is derived from this class. 
        /// Implement this method to add class handling for this event. 
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> that contains the event data. 
        /// This event data reports details about the mouse button that was pressed 
        /// and the handled state.</param>
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            Parent.Focus();
        }

        /// <summary>
        /// Processes left mouse button release.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs" /> that
        /// contains the event data. The event data reports that the left mouse button was
        /// released.</param>
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            ReleaseMouseCapture();
            base.OnMouseLeftButtonUp(e);
            SetValue(IsPressedPropertyKey, false);
        }

        /// <summary>
        /// Processes mouse move.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> that
        /// contains the event data.</param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Point point = e.GetPosition(this);
                double deltaY = point.Y - m_mouseDownPoint.Y;

                m_dragDirection = deltaY > 0 ? DragDirection.Down : DragDirection.Up;

                double absDeltaY = Math.Abs(deltaY);

                if (absDeltaY % DragIncrement > 0 && absDeltaY / DragIncrement >= 1)
                {
                    FireBeforeDragEvents();
                    FireDragIncremented();
                    FireAfterDragEvents();
                }
            }
        }

        /// <summary>
        /// Fire group bar events after dragging.
        /// </summary>
        private void FireAfterDragEvents()
        {
            if (DragDirection == DragDirection.Up)
            {
                Parent.FireAfterSplitUp();
            }
            else if (DragDirection == DragDirection.Down)
            {
                Parent.FireAfterSplitDown();
            }
        }

        /// <summary>
        /// Fire group bar events before dragging.
        /// </summary>
        private void FireBeforeDragEvents()
        {
            if (DragDirection == DragDirection.Up)
            {
                Parent.FireBeforeSplitUp();
            }
            else if (DragDirection == DragDirection.Down)
            {
                Parent.FireBeforeSplitDown();
            }
        }

        /// <summary>
        /// Raises <see cref="DragIncremented"/> event.
        /// </summary>
        private void FireDragIncremented()
        {
            OnDragIncremented();
            RoutedEventArgs args = new RoutedEventArgs(DragIncrementedEvent);
            RaiseEvent(args);
        }
        #endregion
    }
}
