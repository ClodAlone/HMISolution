#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace Syncfusion.UI.Xaml.Controls.Extensions
{
    /// <summary>
    /// Represents a class for the Framework elemnt extensions
    /// </summary>
    public class FrameworkElementExtensions
    {
        #region Cursor
        /// <summary>
        /// Cursor Attached Dependency Property
        /// </summary>
        public static readonly DependencyProperty CursorProperty =
            DependencyProperty.RegisterAttached(
                "Cursor",
                typeof(CoreCursor),
                typeof(FrameworkElementExtensions),
                new PropertyMetadata(null, OnCursorChanged));

        /// <summary>
        /// Gets the Cursor property. This dependency property 
        /// indicates the cursor to use when a mouse cursor is moved over the control.
        /// </summary>
        public static CoreCursor GetCursor(DependencyObject d)
        {
            return (CoreCursor)d.GetValue(CursorProperty);
        }

        /// <summary>
        /// Sets the Cursor property. This dependency property 
        /// indicates the cursor to use when a mouse cursor is moved over the control.
        /// </summary>
        public static void SetCursor(DependencyObject d, CoreCursor value)
        {
            d.SetValue(CursorProperty, value);
        }

        /// <summary>
        /// Handles changes to the Cursor property.
        /// </summary>
        /// <param name="d">
        /// The <see cref="DependencyObject"/> on which
        /// the property has changed value.
        /// </param>
        /// <param name="e">
        /// Event data that is issued by any event that
        /// tracks changes to the effective value of this property.
        /// </param>
        private static void OnCursorChanged(
            DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CoreCursor oldCursor = (CoreCursor)e.OldValue;
            CoreCursor newCursor = (CoreCursor)d.GetValue(CursorProperty);

            if (oldCursor == null)
            {
                var handler = new CursorDisplayHandler();
                handler.Attach((FrameworkElement)d);
                SetCursorDisplayHandler(d, handler);
            }
            else
            {
                var handler = GetCursorDisplayHandler(d);

                if (newCursor == null)
                {
                    handler.Detach();
                    SetCursorDisplayHandler(d, null);
                }
                else
                {
                   handler.UpdateCursor();
                }
            }
        }
        #endregion

        #region CursorDisplayHandler
        /// <summary>
        /// CursorDisplayHandler Attached Dependency Property
        /// </summary>
        public static readonly DependencyProperty CursorDisplayHandlerProperty =
            DependencyProperty.RegisterAttached(
                "CursorDisplayHandler",
                typeof(CursorDisplayHandler),
                typeof(FrameworkElementExtensions),
                new PropertyMetadata(null));

        /// <summary>
        /// Gets the CursorDisplayHandler property. This dependency property 
        /// indicates the handler for displaying the Cursor when a mouse is moved over the control.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static CursorDisplayHandler GetCursorDisplayHandler(DependencyObject d)
        {
            return (CursorDisplayHandler)d.GetValue(CursorDisplayHandlerProperty);
        }

        /// <summary>
        /// Sets the CursorDisplayHandler property. This dependency property 
        /// indicates the handler for displaying the Cursor when a mouse is moved over the control.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void SetCursorDisplayHandler(DependencyObject d, CursorDisplayHandler value)
        {
            d.SetValue(CursorDisplayHandlerProperty, value);
        }
        #endregion


    }

    /// <summary>
    /// Represents a class for the cursor display handler
    /// </summary>
    public class CursorDisplayHandler
    {
        private FrameworkElement _control;
        private bool _isHovering;

        #region DefaultCursor
        private static CoreCursor _defaultCursor;
        /// <summary>
        /// Gets the Default cursor for the <see cref="T:Windows.UI.Core.CoreCursor"/>
        /// </summary>
        public static CoreCursor DefaultCursor
        {
            get
            {
                return _defaultCursor ?? (_defaultCursor = Window.Current.CoreWindow.PointerCursor);
            }
        }
        #endregion

        /// <summary>
        /// Gets the focus
        /// </summary>
        /// <param name="c"></param>
        public void Attach(FrameworkElement c)
        {
            _control = c;
            _control.PointerEntered += OnPointerEntered;
            _control.PointerExited += OnPointerExited;
            _control.Unloaded += OnControlUnloaded;
        }

        private void OnControlUnloaded(object sender, RoutedEventArgs e)
        {
            Detach();
        }

        /// <summary>
        /// Removes the focus
        /// </summary>
        public void Detach()
        {
            _control.PointerEntered -= OnPointerEntered;
            _control.PointerExited -= OnPointerExited;
            _control.Unloaded -= OnControlUnloaded;

            if (_isHovering)
            {
                Window.Current.CoreWindow.PointerCursor = DefaultCursor;
            }
        }

        private void OnPointerEntered(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Mouse)
            {
                _isHovering = true;
                UpdateCursor();
            }
        }

        private void OnPointerExited(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Mouse && !e.Pointer.IsInContact)
            {
                _isHovering = false;
                Window.Current.CoreWindow.PointerCursor = DefaultCursor;
            }
        }

        internal void UpdateCursor()
        {
            if (_defaultCursor == null)
            {
                _defaultCursor = Window.Current.CoreWindow.PointerCursor;
            }

            var cursor = FrameworkElementExtensions.GetCursor(_control);

            if (_isHovering)
            {
                if (cursor != null)
                {
                    Window.Current.CoreWindow.PointerCursor = cursor;
                }
                else
                {
                    Window.Current.CoreWindow.PointerCursor = DefaultCursor;
                }
            }
        }
    }
}
