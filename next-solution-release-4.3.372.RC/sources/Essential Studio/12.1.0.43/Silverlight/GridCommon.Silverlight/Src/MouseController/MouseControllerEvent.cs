#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

#if !WinRT
using System.Windows.Media;
using Syncfusion.Windows.Controls.Cells;
using System.Windows.Browser;
namespace Syncfusion.Windows.Controls.Scroll
#else
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml.Input;

namespace Syncfusion.WinRT.Controls.Scroll
#endif
{
    /// <summary>
    /// Provides event data for mouse events dispatched from the <see cref="MouseControllerDispatcher"/> 
    /// to <see cref="IMouseController"/> controllers.
    /// </summary>
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public class MouseControllerEventArgs : EventArgs
    {
#if !WinRT
        MouseEventArgs sourceEventArgs;
        MouseButtons button;
#else
        PointerRoutedEventArgs sourceEventArgs;
#endif
        int clicks;
        double delta;
        Point location;
        private ICaptureContext cancelCaptureInfo;
        bool isTracking;
        private bool isMouseOverChildElement;
        bool handled;
#if (SILVERLIGHT || WinRT)
        private bool isPressed;
#endif

        // Methods
        /// <summary>
        /// Initializes a new instance of the <see cref="MouseControllerEventArgs"/> class.
        /// </summary>
        /// <param name="sourceEventArgs">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the underlying mouse event data.</param>
        /// <param name="isPreviewEvent">if set to <c>true</c> this is a preview event.</param>
        /// <param name="button">The button.</param>
        /// <param name="clicks">The clicks.</param>
        /// <param name="location">The mouse location.</param>
        /// <param name="delta">The mouse-wheel delta.</param>
#if !WinRT
        public MouseControllerEventArgs(MouseEventArgs sourceEventArgs, MouseButtons button, int clicks, Point location, double delta, bool isPressed)
        {
            Debug.Assert(sourceEventArgs != null);
            this.sourceEventArgs = sourceEventArgs;
            this.clicks = clicks;
            this.location = location;
            this.delta = delta;
            this.isPressed = isPressed;
            this.button = button;
        }
#else
        public MouseControllerEventArgs(PointerRoutedEventArgs sourceEventArgs, int clicks, Point location, double delta, bool isPressed)
        {
            Debug.Assert(sourceEventArgs != null);
            this.sourceEventArgs = sourceEventArgs;
            this.clicks = clicks;
            this.location = location;
            this.delta = delta;
            this.isPressed = isPressed;
        }
#endif

#if (SILVERLIGHT || WinRT)
        /// <summary>
        /// Gets the pressed state of the mouse button
        /// </summary>
        public bool IsPressed
        {
            get
            {
                return this.isPressed;
            }
        }
#endif

        /// <summary>
        /// Gets the <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the underlying mouse event data.
        /// </summary>
        /// <value>The source event args.</value>
        /// 

#if !WinRT
        public MouseEventArgs SourceEventArgs
        {
            get
            {               
                return sourceEventArgs;
            }
        }
#else
        public PointerRoutedEventArgs SourceEventArgs
        {
            get
            {
                return sourceEventArgs;
            }
        }
#endif

        /// <summary>
        /// Gets the cancel capture context.
        /// </summary>
        /// <value>The cancel capture context.</value>
        public ICaptureContext CancelCaptureInfo
        {
            get
            {
                return this.cancelCaptureInfo;
            }
            internal set
            {
                this.cancelCaptureInfo = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the is mouse over child UIElement.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is mouse over a child element; otherwise, <c>false</c>.
        /// </value>
        public bool IsMouseOverChildElement
        {
            get
            {
                return this.isMouseOverChildElement;
            }
            internal set
            {
                this.isMouseOverChildElement = value;
            }
        }

#if (!SILVERLIGHT && !WinRT)
        /// <summary>
        /// Gets the cell renderer that the child UIElement belongs 
        /// to which is returned by MouseDevice.DirectlyOver property.
        /// </summary>
        /// <value>The directly over renderer.</value>
        public ICellRenderer DirectlyOverRenderer
        {
            get
            {
                DependencyObject dpo = SourceEventArgs.MouseDevice.DirectlyOver as DependencyObject;
                if (dpo != null)
                    return VirtualizingCellsControl.GetCellRenderer(dpo);
                return null;
            }
        }
#endif

        /// <summary>
        /// Gets a value indicating whether the mouse tracking feature is enabled.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is tracking; otherwise, <c>false</c>.
        /// </value>
        public bool IsTracking
        {
            get { return isTracking; }
            internal set { isTracking = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this event is handled.
        /// </summary>
        /// <value><c>true</c> if event is handled; otherwise, <c>false</c>.</value>
        public bool Handled
        {
            get
            {
#if !WinRT
                if (SourceEventArgs is MouseButtonEventArgs)
                    return ((MouseButtonEventArgs) SourceEventArgs).Handled;
                return handled; 
#else
                return handled;
#endif
            }
            set
            {
                handled = value;
#if !WinRT
                if (SourceEventArgs is MouseButtonEventArgs)
                    ((MouseButtonEventArgs) SourceEventArgs).Handled = value;
#endif
            }
        }
#if !WinRT
        // Properties
        /// <summary>
        /// Gets the mouse button that is pressed.
        /// </summary>
        /// <value>The mouse button or null if not pressed.</value>
        public MouseButtons Button
        {
            get
            {
                return this.button;
            }
        }
#endif

        /// <summary>
        /// Gets the click count (1 - single click, 2 - double click)
        /// </summary>
        /// <value>The click count.</value>
        public int ClickCount
        {
            get
            {
                return this.clicks;
            }
        }

        /// <summary>
        /// Gets the mouse wheel delta.
        /// </summary>
        /// <value>The delta.</value>
        public double Delta
        {
            get
            {
                return this.delta;
            }
        }

        /// <summary>
        /// Gets or sets the mouse location.
        /// </summary>
        /// <value>The location.</value>
        public Point Location
        {
            get
            {
                return location;
            }
            internal set
            {
                location = value;
            }
        }
    }

    /// <summary>
    /// Represents the method that will handle <see cref="MouseControllerDispatcher"/> related events.
    /// </summary>
    public delegate void MouseControllerEventHandler(object sender, MouseControllerEventArgs e);
}
