#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Security;
using System.Security.Permissions;

namespace Syncfusion.GridHelperClasses
{
    /// <summary>
    /// Used internally for declaring the constants of the nativemethods.
    /// </summary>
    internal static class NativeMethods
    {
        internal const int WM_NCHITTEST = 0x0084,
                           WM_NCACTIVATE = 0x0086,
                           WS_EX_TRANSPARENT = 0x00000020,
                           WS_EX_TOOLWINDOW = 0x00000080,
                           WS_EX_LAYERED = 0x00080000,
                           WS_EX_NOACTIVATE = 0x08000000,
                           HTTRANSPARENT = -1,
                           HTLEFT = 10,
                           HTRIGHT = 11,
                           HTTOP = 12,
                           HTTOPLEFT = 13,
                           HTTOPRIGHT = 14,
                           HTBOTTOM = 15,
                           HTBOTTOMLEFT = 16,
                           HTBOTTOMRIGHT = 17,
                           WM_PRINT = 0x0317,
                           WM_USER = 0x0400,
                           WM_REFLECT = WM_USER + 0x1C00,
                           WM_COMMAND = 0x0111,
                           CBN_DROPDOWN = 7,
                           WM_GETMINMAXINFO = 0x0024;

        private static HandleRef HWND_TOPMOST = new HandleRef(null, new IntPtr(-1));

        [Flags]
        internal enum AnimationFlags : int
        {
            Roll = 0x0000, // Uses a roll animation.
            HorizontalPositive = 0x00001, // Animates the window from left to right. This flag can be used with roll or slide animation.
            HorizontalNegative = 0x00002, // Animates the window from right to left. This flag can be used with roll or slide animation.
            VerticalPositive = 0x00004, // Animates the window from top to bottom. This flag can be used with roll or slide animation.
            VerticalNegative = 0x00008, // Animates the window from bottom to top. This flag can be used with roll or slide animation.
            Center = 0x00010, // Makes the window appear to collapse inward if Hide is used or expand outward if the Hide is not used.
            Hide = 0x10000, // Hides the window. By default, the window is shown.
            Activate = 0x20000, // Activates the window.
            Slide = 0x40000, // Uses a slide animation. By default, roll animation is used.
            Blend = 0x80000, // Uses a fade effect. This flag can be used only with a top-level window.
            Mask = 0xfffff,
        }

        /// <summary>
        /// Method to animate the ToolTip window.
        /// </summary>
        /// <param name="windowHandle">windows handle.</param>
        /// <param name="time">time to animate the tooltip.</param>
        /// <param name="flags">animation flags.</param>
        /// <returns>The security handle.</returns>
        [SuppressUnmanagedCodeSecurity]
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int AnimateWindow(HandleRef windowHandle, int time, AnimationFlags flags);

        internal static void AnimateWindow(Control control, int time, AnimationFlags flags)
        {
            try
            {
                SecurityPermission sp = new SecurityPermission(SecurityPermissionFlag.UnmanagedCode);
                sp.Demand();
                AnimateWindow(new HandleRef(control, control.Handle), time, flags);
            }
            catch (SecurityException) { }
        }

        /// <summary>
        /// To set the windows position of the ToolTip, used internally.
        /// </summary>
        /// <param name="hWnd">handle to a resource</param>
        /// <param name="hWndInsertAfter">handle to the resource</param>
        /// <param name="x">x postion</param>
        /// <param name="y">y postion</param>
        /// <param name="cx">width</param>
        /// <param name="cy">height</param>
        /// <param name="flags">falgs</param>
        /// <returns>a boolean value</returns>
        [SuppressUnmanagedCodeSecurity]
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        private static extern bool SetWindowPos(HandleRef hWnd, HandleRef hWndInsertAfter, int x, int y, int cx, int cy, int flags);

        /// <summary>
        /// To set the passed control as topmost in the application.
        /// </summary>
        /// <param name="control">passed control</param>
        internal static void SetTopMost(Control control)
        {
            try
            {
                SecurityPermission sp = new SecurityPermission(SecurityPermissionFlag.UnmanagedCode);
                sp.Demand();
                SetWindowPos(new HandleRef(control, control.Handle), HWND_TOPMOST, 0, 0, 0, 0, 0x13);
            }
            catch (SecurityException) { }
        }

        /// <summary>
        /// used internally.
        /// </summary>
        /// <param name="n"></param>
        /// <returns>integer value</returns>
        internal static int HIWORD(int n)
        {
            return (short)((n >> 16) & 0xffff);
        }

        /// <summary>
        /// used internally.
        /// </summary>
        /// <param name="n"></param>
        /// <returns>integer value</returns>
        internal static int HIWORD(IntPtr n)
        {
            return HIWORD(unchecked((int)(long)n));
        }

        /// <summary>
        /// used internally.
        /// </summary>
        /// <param name="n"></param>
        /// <returns>integer value</returns>
        internal static int LOWORD(int n)
        {
            return (short)(n & 0xffff);
        }

        /// <summary>
        /// used internally.
        /// </summary>
        /// <param name="n"></param>
        /// <returns>integer value</returns>
        internal static int LOWORD(IntPtr n)
        {
            return LOWORD(unchecked((int)(long)n));
        }

        /// <summary>
        /// used internally.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct MINMAXINFO
        {
            public Point reserved;
            public Size maxSize;
            public Point maxPosition;
            public Size minTrackSize;
            public Size maxTrackSize;
        }

        private static bool? _isRunningOnMono;
        /// <summary>
        /// used internally, Gets the application running mode.
        /// </summary>
        public static bool IsRunningOnMono
        {
            get
            {
                if (!_isRunningOnMono.HasValue)
                    _isRunningOnMono = Type.GetType("Mono.Runtime") != null;
                return _isRunningOnMono.Value;
            }
        }
    }
}