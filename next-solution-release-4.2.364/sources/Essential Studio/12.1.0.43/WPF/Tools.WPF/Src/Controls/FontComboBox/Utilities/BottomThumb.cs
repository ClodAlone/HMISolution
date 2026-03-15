// <copyright file="BottomThumb.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws.
// </copyright>

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents bottom thumb of the popup of <see cref="Syncfusion.Windows.Tools.Controls.FontListComboBox"/> control.
    /// Used for changing height of the popup.
    /// </summary>
#if SyncfusionFramework4_0

    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class BottomThumb : Control
    {
        #region Initialization

        /// <summary>
        /// Initializes static members of the <see cref="BottomThumb"/> class.
        /// Overrides some dependency properties.
        /// </summary>
        static BottomThumb()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BottomThumb), new FrameworkPropertyMetadata(typeof(BottomThumb)));
        }

        #endregion Initialization

        #region Properties

        /// <summary>
        /// Gets or sets the height of the popup. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// </value>
        /// <seealso cref="double"/>
        public double Offset
        {
            get
            {
                return (double)GetValue(OffsetProperty);
            }

            set
            {
                SetValue(OffsetProperty, value);
            }
        }

        #endregion Properties

        #region Events

        /// <summary>
        /// Event that is raised when Offset property is changed.
        /// </summary>
        public event PropertyChangedCallback OffsetChanged;

        #endregion Events

        #region Dependency properties

        /// <summary>
        /// Identifies <see cref="Offset"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty OffsetProperty =
            DependencyProperty.Register("Offset", typeof(double), typeof(BottomThumb), new FrameworkPropertyMetadata(Double.NaN, new PropertyChangedCallback(OnOffsetChanged)));

        #endregion Dependency properties

        #region Implementation

        /// <summary>
        /// Invoked when an <see cref="E:System.Windows.Input.Mouse.MouseDown"/>�attached event reaches an element.
        /// </summary>
        /// <param name="e">Mouse Button Event Data</param>
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            Mouse.Capture(this);

            base.OnMouseDown(e);
        }

        /// <summary>
        /// Invoked when an <see cref="W:System.Windows.Input.Mouse.MouseUp"/>�routed event reaches an element.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> that contains the event data.</param>
        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            Mouse.Capture(null);

            base.OnMouseUp(e);
        }

        /// <summary>
        /// Invoked when an <see cref="E:System.Windows.Input.Mouse.MouseMove"/>�attached event reaches an element.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> that contains the event data.</param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            this.Cursor = Cursors.SizeNS;

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Point point = e.GetPosition((IInputElement)this.Parent);

                double temp = point.Y;

                if (temp > 100)
                {
                    Offset = temp;
                }
            }

            base.OnMouseMove(e);
        }

        /// <summary>
        /// Calls OnOffsetChanged method of the instance, notifies of the
        /// dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value and new value.</param>
        private static void OnOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BottomThumb instance = (BottomThumb)d;
            instance.OnOffsetChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="OffsetChanged"/> event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value and new value.</param>
        protected virtual void OnOffsetChanged(DependencyPropertyChangedEventArgs e)
        {
            if (OffsetChanged != null)
            {
                OffsetChanged(this, e);
            }
        }

        #endregion Implementation
    }
}