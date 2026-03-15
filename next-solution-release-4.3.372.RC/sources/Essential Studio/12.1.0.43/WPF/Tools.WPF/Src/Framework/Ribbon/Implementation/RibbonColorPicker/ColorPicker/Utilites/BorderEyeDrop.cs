// <copyright file="BorderEyeDrop.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Syncfusion.Licensing;
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Tools.Controls
{
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    /// <summary>
    /// Represent the Border eye drop class.
    /// </summary>
    internal class xBorderEyeDrop : Border
    {
        #region Private members
        /// <summary>
        /// Specifies whether the color-picking is currently in action.
        /// </summary>
        private bool m_bPressed;

        /// <summary>
        /// Stores last X position of the cursor.
        /// </summary>
        private int m_iLastX;

        /// <summary>
        /// Stores last Y position of the cursor.
        /// </summary>
        private int m_iLastY;

        /// <summary>
        /// Stores the cursor.
        /// </summary>
        private Cursor m_cursor;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the value of the Color, that is currently under the mouse cursor. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Color"/>
        /// </value>
        public Color Color
        {
            get
            {
                return (Color)GetValue(ColorProperty);
            }

            set
            {
                SetValue(ColorProperty, value);
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Raises the <see cref="E:System.Windows.FrameworkElement.Initialized"/> event. This method is invoked whenever <see cref="P:System.Windows.FrameworkElement.IsInitialized"/> is set to true internally.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.RoutedEventArgs"/> that contains the event data.</param>
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            if (EnvironmentTest.IsSecurityGranted)
            {
                Assembly ass = Assembly.GetExecutingAssembly();
                Stream stream = ass.GetManifestResourceStream("Syncfusion.Tools.WPF.Controls.ColorPicker.Images.EyeDropCursor.cur");
                m_cursor = new Cursor(stream);
            }
            else
            {
                m_cursor = Cursors.Pen;
            }
        }

        /// <summary>
        /// Raises the PreviewMouseLeftButtonDown event.
        /// </summary>
        /// <param name="e">The MouseButtonEventArgs that contains the event data.</param>
        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (BrowserInteropHelper.IsBrowserHosted && !PermissionHelper.HasUnmanagedCodePermission)
            {
                e.Handled = true;
            }

            base.OnPreviewMouseLeftButtonDown(e);
        }
        
        /// <summary>
        /// Processes pressings on the mouse buttons on the control, puts control into the color-picking mode.
        /// </summary>
        /// <param name="e">Data for mouse button related event.</param>
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            m_bPressed = CaptureMouse();

            if (m_bPressed)
            {
                LostMouseCapture += BorderEyeDrop_LostMouseCapture;
                MouseMove += BorderEyeDrop_MouseMove;
                InputManager.Current.PostNotifyInput += Current_PostNotifyInput;
                RaiseEvent(new RoutedEventArgs(BeginColorPickingEvent));
                Cursor = m_cursor;
            }

            e.Handled = true;
            base.OnMouseDown(e);
        }

        /// <summary>
        /// Cancels color-picking.
        /// </summary>
        /// <param name="sender">An object, the change occurs on.</param>
        /// <param name="e">Provides data for raw input being processed by the InputManager.
        /// </param>
        private void Current_PostNotifyInput(object sender, NotifyInputEventArgs e)
        {
            KeyEventArgs input = e.StagingItem.Input as KeyEventArgs;

            if (((input != null) && (input.Key == Key.Escape)) && m_bPressed)
            {
                m_bPressed = false;
                this.ReleaseMouseCapture();
            }
        }

        /// <summary>
        /// Puts control out of the color-picking mode.
        /// </summary>
        /// <param name="e">Data for mouse button related event.</param>
        protected override void OnMouseUp(System.Windows.Input.MouseButtonEventArgs e)
        {
            if (m_bPressed)
            {
                RaiseEvent(new RoutedEventArgs(EndColorPickingEvent));
                ReleaseMouseCapture();

                ClearValue(CursorProperty);
            }

            e.Handled = true;
            base.OnMouseUp(e);
        }

        /// <summary>
        /// Processes movements of the mouse while in color-picking mode.
        /// </summary>
        /// <param name="sender">An object, the change occurs on.</param>
        /// <param name="e">Data for mouse-related event.</param>
        private void BorderEyeDrop_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            UpdatePixelColor();
        }

        /// <summary>
        /// Processes loss of the mouse capture.
        /// </summary>
        /// <param name="sender">An object, the change occurs on.</param>
        /// <param name="e">Data for mouse-related event.</param>
        private void BorderEyeDrop_LostMouseCapture(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (!m_bPressed)
            {
                RaiseEvent(new RoutedEventArgs(CancelColorPickingEvent));
            }

            m_bPressed = false;
            InputManager.Current.PostNotifyInput -= Current_PostNotifyInput;
            LostMouseCapture -= BorderEyeDrop_LostMouseCapture;
            MouseMove -= BorderEyeDrop_MouseMove;
        }

        /// <summary>
        /// Updates Color property by the color of the pixel under the mouse.
        /// </summary>
        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        private void UpdatePixelColor()
        {
            Win32Point pt = new Win32Point(0, 0);
            GetCursorPos(ref pt);

            int x = pt.X;
            int y = pt.Y;

            if ((x != m_iLastX) || (y != m_iLastY))
            {
                m_iLastX = x;
                m_iLastY = y;

                IntPtr hdc = CreateDC("Display", null, null, IntPtr.Zero);
                int num3 = GetPixel(hdc, x, y);
                DeleteDC(hdc);

                Color color = Color.FromArgb(255, (byte)(num3 & 255), (byte)((num3 & 65280) >> 8), (byte)((num3 & 16711680) >> 16));

                if (Color != color)
                {
                    Color = color;
                }
            }
        }
        #endregion

        #region Dependency properties
        /// <summary>
        /// Specifies color, that is currently under the mouse cursor. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Color"/>
        /// </value>
        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Color), typeof(xBorderEyeDrop), new FrameworkPropertyMetadata(Colors.Transparent));
        #endregion

        #region Routed events
        /// <summary>
        /// Routed event that is raised when the color-picking is started.
        /// </summary>
        public static readonly RoutedEvent BeginColorPickingEvent = EventManager.RegisterRoutedEvent("BeginColorPicking", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(xBorderEyeDrop));

        /// <summary>
        /// Routed event that is raised when the color-picking is successfully ended.
        /// </summary>
        public static readonly RoutedEvent EndColorPickingEvent = EventManager.RegisterRoutedEvent("EndColorPicking", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(xBorderEyeDrop));

        /// <summary>
        /// Routed event that is raised when the color-picking is canceled.
        /// </summary>
        public static readonly RoutedEvent CancelColorPickingEvent = EventManager.RegisterRoutedEvent("CancelColorPicking", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(xBorderEyeDrop));
        #endregion

        #region DLL Imports
        /// <summary>
        /// API that gets the position of cursor.
        /// </summary>
        /// <param name="pt">Value for the pt</param>
        /// <returns>return an integer</returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        internal static extern int GetCursorPos(ref Win32Point pt);

        /// <summary>
        /// API that gets the device context, used to get the context of the screen.
        /// </summary>
        /// <param name="strDriver">Value for strDriver</param>
        /// <param name="strDevice">Value for strDevice</param>
        /// <param name="strOutput">Value for strOutput</param>
        /// <param name="pData">Value for pData</param>
        /// <returns>returns an IntPtr</returns>
        [DllImport("gdi32.dll", EntryPoint = "CreateDCW", CharSet = CharSet.Unicode)]
        internal static extern IntPtr CreateDC(string strDriver, string strDevice, string strOutput, IntPtr pData);

        /// <summary>
        /// API used to get the color of the pixel on the specified context by the given coordinates.
        /// </summary>
        /// <param name="hdc">Value for the hdc</param>
        /// <param name="x">Value for the x</param>
        /// <param name="y">Value for the y</param>
        /// <returns>returns an integer value</returns>
        [DllImport("gdi32.dll")]
        internal static extern int GetPixel(IntPtr hdc, int x, int y);

        /// <summary>
        /// API, used to deallocate the acquired device context.
        /// </summary>
        /// <param name="hdc">Value of the hdc</param>
        /// <returns>returns an integer</returns>
        [DllImport("gdi32.dll")]
        internal static extern int DeleteDC(IntPtr hdc);
        #endregion

        #region Internal classes
        /// <summary>
        /// Structure used to store the information regarding the point.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct Win32Point
        {
            /// <summary>
            /// Stores an integer value.
            /// </summary>
            public int X;

            /// <summary>
            /// Stores an integer value.
            /// </summary>
            public int Y;

            /// <summary>
            /// Initializes a new instance of the <see cref="Win32Point"/> struct.
            /// </summary>
            /// <param name="x">The X coordinate.</param>
            /// <param name="y">The Y coordinate.</param>
            public Win32Point(int x, int y)
            {
                this.X = x;
                this.Y = y;
            }
        }
        #endregion
    }
}
