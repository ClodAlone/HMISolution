// <copyright file="TitleButtonAdorner.cs" company="Syncfusion">
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
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents the Title button Adorner
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    internal class TitleButtonAdorner : TemplatedAdornerBase
    {
        SystemGesture m_systemGesture;

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="TitleButtonAdorner"/> class.
        /// </summary>
        /// <param name="button">The button value.</param>
        public TitleButtonAdorner(WindowTitleBarButton button)
            : base(button)
        {
            Binding binding = new Binding("Content");
            binding.Source = button;
            SetBinding(ContentProperty, binding);

            binding = new Binding("Background");
            binding.Source = button;
            SetBinding(ColorProperty, binding);

            binding = new Binding("ToolTip");
            binding.Source = button;
            SetBinding(ToolTipProperty, binding);
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        /// <value>The content.</value>
        public object Content
        {
            get
            {
                return (object)GetValue(ContentProperty);
            }

            set
            {
                SetValue(ContentProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value of the Color dependency property. 
        /// </summary>
        public Brush Color
        {
            get
            {
                return (Brush)GetValue(ColorProperty);
            }

            set
            {
                SetValue(ColorProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is pressed.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is pressed; otherwise, <c>false</c>.
        /// </value>
        public bool IsPressed
        {
            get
            {
                return (bool)GetValue(IsPressedProperty);
            }

            set
            {
                SetValue(IsPressedProperty, value);
            }
        }
        #endregion

        #region Dependency properties
        /// <summary>
        /// Using a DependencyProperty as the backing store for Content.
        /// This enables animation, styling, binding data etc...
        /// </summary>
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof(object), typeof(TitleButtonAdorner), new UIPropertyMetadata(null));

        /// <summary>
        /// Using a DependencyProperty as the backing store for Color.
        /// This enables animation, styling, binding data etc...
        /// </summary>
        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Brush), typeof(TitleButtonAdorner), new UIPropertyMetadata(Brushes.Transparent));

        /// <summary>
        /// Using a DependencyProperty as the backing store for
        /// IsPressed. This enables animation, styling, binding data etc...
        /// </summary>
        public static readonly DependencyProperty IsPressedProperty =
            DependencyProperty.Register("IsPressed", typeof(bool), typeof(TitleButtonAdorner), new UIPropertyMetadata(false));
        #endregion

        #region Overrides
        /// <summary>
        /// Raises the MouseMove event.
        /// </summary>
        /// <param name="e">A MouseEventArgs that contains the event
        /// data.</param>
        protected override void OnMouseMove(System.Windows.Input.MouseEventArgs e)
        {
            if (e.StylusDevice == null)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    IsPressed = true;
                }
                else
                {
                    IsPressed = false;
                }

                base.OnMouseMove(e);
            }
        }

        /// <summary>
        /// Raises the MouseLeave event.
        /// </summary>
        /// <param name="e">An EventArgs that contains the event data.</param>
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            if (e.StylusDevice == null)
            {
                IsPressed = false;

                base.OnMouseLeave(e);
            }
        }

        /// <summary>
        /// Invoked when an unhandled MouseLeftButtonDown routed event is
        /// raised on this element. Implement this method to add class
        /// handling for this event.
        /// </summary>
        /// <param name="e">The MouseButtonEventArgs that contains the
        /// event data. The event data reports that the
        /// left mouse button was pressed.</param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (e.StylusDevice == null)
            {
                IsPressed = true;
                base.OnMouseLeftButtonDown(e);
            }
        }

        /// <summary>
        /// Invoked when an unhandled MouseLeftButtonUp routed event
        /// reaches an element in its route that is derived from this
        /// class. Implement this method to add class handling for this
        /// event.
        /// </summary>
        /// <param name="e">The MouseButtonEventArgs that contains the
        /// event data. The event data reports that the
        /// left mouse button was released.</param>
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (e.StylusDevice == null)
            {
                IsPressed = false;
                base.OnMouseLeftButtonUp(e);
            }
        }


        protected override void OnStylusSystemGesture(StylusSystemGestureEventArgs e)
        {
            m_systemGesture = e.SystemGesture;
            base.OnStylusSystemGesture(e);
        }

         #if !SyncfusionFramework3_5
        protected override void OnTouchUp(TouchEventArgs e)
        {
            if (this.AreAnyTouchesOver && m_systemGesture == SystemGesture.Tap)
            {
                IsPressed = false;
                base.OnTouchUp(e);
            }
        }

        protected override void OnTouchDown(TouchEventArgs e)
        {
            if (this.AreAnyTouchesOver && m_systemGesture == SystemGesture.Tap)
            {
                IsPressed = true;
                base.OnTouchDown(e);
            }
        }

        protected override void OnTouchLeave(TouchEventArgs e)
        {
            if (this.AreAnyTouchesOver)
            {
                IsPressed = false;
                base.OnTouchLeave(e);
            }
        }
#endif

        #endregion
    }
}
