// <copyright file="VistaWindowInterop.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace Syncfusion.Windows.Shared
{
    /// <summary>
    /// This class is responsible for operations with Win32 Windows.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class VistaWindowInterop
    {
        #region Win32 Types and API calls
        /// <summary>
        /// Changes the size, position, and Z order of a window.
        /// </summary>
        /// <param name="hwnd">Handle of window.</param>
        /// <param name="hwndInsertAfter">
        /// Either the handle of the window to position this window behind, or a flag stating where in the Z-order to put the window:
        /// * -2 = Put the window below all topmost windows and above all non-topmost windows.
        /// * -1 = Make the window topmost (above all other windows) permanently.
        /// * 0 = Put the window at the top of the Z-order.
        /// * 1 = Put the window at the bottom of the Z-order. </param>
        /// <param name="x">The x coordinate of where to put the upper-left corner of the window.</param>
        /// <param name="y">The y coordinate of where to put the upper-left corner of the window.</param>
        /// <param name="cx">The x coordinate of where to put the lower-right corner of the window.</param>
        /// <param name="cy">The y coordinate of where to put the lower-right corner of the window.</param>
        /// <param name="wFlags">Zero or more of the following flags stating how to move the window:
        /// * 32 = Fully redraw the window in its new position.
        /// * 128 = Hide the window from the screen.
        /// * 16 = Do not make the window active after moving it unless it was already the active window.
        /// * 256 = Do not redraw anything drawn on the window after it is moved.
        /// * 2 = Do not move the window.
        /// * 1 = Do not resize the window. 
        /// * 8 = Do not remove the image of the window in its former position, effectively leaving a ghost image on the screen.
        /// * 4 = Do not change the window's position in the Z-order.
        /// * 64 = Show the window if it is hidden. 
        /// </param>
        /// <returns>
        /// Returns 1 if successful, or 0 if an error occurred.
        /// </returns>
        [DllImport("user32", EntryPoint = "SetWindowPos")]
        public static extern int SetWindowPos(IntPtr hwnd, int hwndInsertAfter, int x, int y, int cx, int cy, int wFlags);

        /// <summary>
        /// Retrieves the dimensions of the bounding rectangle of the specified window. 
        /// The dimensions are given in screen coordinates that are relative to the upper-left corner of the screen. 
        /// </summary>
        /// <param name="hWnd">Handle of window.</param>
        /// <param name="r">Variable that receives the coordinates of the upper-left and lower-right corners of the window.</param>
        /// <returns>Returns true if successful, or false if an error occurred.</returns>
        [DllImport("user32", EntryPoint = "GetWindowRect")]
        public static extern bool GetWindowRect(IntPtr hWnd, ref VistaWindowInterop.RECT r);

        /// <summary>
        /// Extends the window frame behind the client area.
        /// </summary>
        /// <param name="hWnd">Handle to the window of interest.</param>
        /// <param name="pMarInset">The pointer to a MARGINS Structure
        /// structure that describes the margins
        /// to use when extending the frame into
        /// the client area.</param>
        [DllImport("dwmapi.dll", CharSet = CharSet.Auto)]
        private static extern void DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS pMarInset);

        /// <summary>
        /// Obtains a value that indicates whether Desktop Window Manager
        /// (DWM) composition is enabled. Applications can listen for
        /// composition state changes by handling the
        /// WM_DWMCOMPOSITIONCHANGED notification.
        /// </summary>
        /// <param name="pfEnabled">The pointer that receives the value
        /// indicating whether DWM composition
        /// is enabled. TRUE if DWM composition
        /// is enabled; otherwise, FALSE.</param>
        [DllImport("dwmapi.dll", CharSet = CharSet.Auto)]
        private static extern void DwmIsCompositionEnabled(ref bool pfEnabled);

        /// <summary>
        /// The MonitorFromWindow function retrieves a handle to the
        /// display monitor that has the largest area of intersection
        /// with the bounding rectangle of a specified window.
        /// </summary>
        /// <param name="hMonitor">Handle to the display monitor of
        /// interest.</param>
        /// <param name="lpmi">Pointer to a MONITORINFO or
        /// MONITORINFOEX structure that receives
        /// information about the specified
        /// display monitor.<para></para>You must
        ///  set the cbSize member of the
        /// structure to sizeof(MONITORINFO) or
        /// sizeof(MONITORINFOEX) before calling
        /// the GetMonitorInfo function. Doing so
        /// lets the function determine the type
        /// of structure you are passing to it.<para></para>The
        /// MONITORINFOEX structure is a superset
        /// of the MONITORINFO structure. It has
        /// one additional member\: a string that
        /// contains a name for the display
        /// monitor. Most applications have no
        /// use for a display monitor name, and
        /// so can save some bytes by using a
        /// MONITORINFO structure. </param>
        /// <returns>
        /// If the function succeeds, the return value is nonzero. If the
        /// function fails, the return value is zero. Windows NT/2000/XP:
        /// To get extended error information, call GetLastError.
        /// </returns>
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern bool GetMonitorInfo(IntPtr hMonitor, MONITORINFO lpmi);

        /// <summary>
        /// The GetSystemMenu function allows the application to access
        /// the window menu (also known as the system menu or the control
        /// menu) for copying and modifying.
        /// </summary>
        /// <param name="hWnd">Handle to the window that will own a
        /// copy of the window menu.</param>
        /// <param name="bRevert">Specifies the action to be taken. If
        /// this parameter is FALSE, GetSystemMenu
        /// returns a handle to the copy of the
        /// window menu currently in use. The copy
        /// is initially identical to the window
        /// menu, but it can be modified. If this
        /// parameter is TRUE, GetSystemMenu
        /// resets the window menu back to the
        /// default state. The previous window
        /// menu, if any, is destroyed.</param>
        /// <returns>
        /// If the bRevert parameter is FALSE, the return value is a
        /// handle to a copy of the window menu. If the bRevert parameter
        /// is TRUE, the return value is NULL.
        /// </returns>
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        /// <summary>
        /// The MonitorFromWindow function retrieves a handle to the
        /// display monitor that has the largest area of intersection
        /// with the bounding rectangle of a specified window.
        /// </summary>
        /// <param name="hwnd">Handle to the window of interest.</param>
        /// <param name="dwFlags">Determines the function's return value
        /// if the window does not intersect any
        /// display monitor. This parameter can be
        /// one of the following values. </param>
        /// <returns>
        /// If the window intersects one or more display monitor
        /// rectangles, the return value is an HMONITOR handle to the
        /// display monitor that has the largest area of intersection
        /// with the window. If the window does not intersect a display
        /// monitor, the return value depends on the value of dwFlags.
        /// </returns>
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr MonitorFromWindow(IntPtr hwnd, int dwFlags);

        /// <summary>
        /// The SendMessage function sends the specified message to a
        /// window or windows. It calls the window procedure for the
        /// specified window and does not return until the window
        /// procedure has processed the message.
        /// </summary>
        /// <param name="hWnd">Handle to the window whose window
        /// procedure will receive the message. If
        /// this parameter is HWND_BROADCAST, the
        /// message is sent to all top\-level
        /// windows in the system, including
        /// disabled or invisible unowned windows,
        /// overlapped windows, and pop\-up
        /// windows; but the message is not sent to
        /// child windows.</param>
        /// <param name="msg">Specifies the message to be sent.</param>
        /// <param name="wParam">Specifies additional message\-specific
        /// information.</param>
        /// <param name="lParam">Specifies additional message\-specific
        /// information's</param>
        /// <returns>
        /// The return value specifies the result of the message
        /// processing; it depends on the message sent.
        /// </returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        /// <summary>
        /// The TrackPopupMenu function displays a shortcut menu at the
        /// specified location and tracks the selection of items on the
        /// menu. The shortcut menu can appear anywhere on the screen.
        /// </summary>
        /// <param name="hMenu">Handle to the shortcut menu to be
        /// displayed. The handle can be
        /// obtained by calling CreatePopupMenu
        /// to create a new shortcut menu, or by
        /// calling GetSubMenu to retrieve a
        /// handle to a submenu associated with
        /// an existing menu item.</param>
        /// <param name="uFlags">Use zero of more of these flags to
        /// specify function options.More
        /// information can be found on the
        /// following link\: http\://msdn2.microsoft.com/en\-us/library/ms648002.aspx</param>
        /// <param name="x">Specifies the horizontal location of
        /// the shortcut menu, in screen
        /// coordinates.</param>
        /// <param name="y">Specifies the vertical location of
        /// the shortcut menu, in screen
        /// coordinates.</param>
        /// <param name="nReserved">Reserved; must be zero.</param>
        /// <param name="hWnd">Handle to the window that owns the
        /// shortcut menu. This window receives
        /// all messages from the menu. The
        /// window does not receive a WM_COMMAND
        /// message from the menu until the
        /// function returns. If you specify
        /// TPM_NONOTIFY in the uFlags
        /// parameter, the function does not
        /// send messages to the window
        /// identified by hWnd. However, you
        /// must still pass a window handle in
        /// hWnd. It can be any window handle
        /// from your application.</param>
        /// <param name="prcRect">Ignored value.</param>
        /// <returns>
        /// If you specify TPM_RETURNCMD in the uFlags parameter, the
        /// return value is the menu-item identifier of the item that
        /// the user selected. If the user cancels the menu without
        /// making a selection, or if an error occurs, then the return
        /// value is zero.
        /// <para/>
        /// If you do not specify TPM_RETURNCMD in the uFlags parameter,
        /// the return value is nonzero if the function succeeds and zero
        /// if it fails. To get extended error information, call
        /// GetLastError.
        /// </returns>
        [DllImport("User32.dll", CharSet = CharSet.Auto)][CLSCompliant (false)]
        public static extern int TrackPopupMenu(IntPtr hMenu, UInt32 uFlags, int x, int y, int nReserved, IntPtr hWnd, IntPtr prcRect);

        /// <summary>
        /// Represents an x- and y-coordinate pair in two-dimensional
        /// space.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct POINT
        {
            /// <summary>
            /// The x-coordinate of this Point.
            /// </summary>
            public int x;

            /// <summary>
            /// The y-coordinate of this Point.
            /// </summary>
            public int y;

            /// <summary>
            /// Initializes a new instance of the <see cref="POINT"/> struct.
            /// </summary>
            /// <param name="x1">The x1 value.</param>
            /// <param name="y1">The y1 value.</param>
            public POINT(int x1, int y1)
            {
                this.x = x1;
                this.y = y1;
            }
        }

        /// <summary>
        /// The RECT structure defines the coordinates of the upper-left
        /// and lower-right corners of a rectangle.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            /// <summary>
            /// Specifies the x-coordinate of the upper-left corner of the
            /// rectangle.
            /// </summary>
            public int left;

            /// <summary>
            /// Specifies the y-coordinate of the upper-left corner of the
            /// rectangle.
            /// </summary>
            public int top;

            /// <summary>
            /// Specifies the x-coordinate of the lower-right corner of the
            /// rectangle.
            /// </summary>
            public int right;

            /// <summary>
            /// Specifies the y-coordinate of the lower-right corner of the
            /// rectangle.
            /// </summary>
            public int bottom;

            /// <summary>
            /// Initializes a new instance of the <see cref="RECT"/> struct.
            /// </summary>
            /// <param name="l">The l value.</param>
            /// <param name="t">The t value.</param>
            /// <param name="r">The r value.</param>
            /// <param name="b">The b value.</param>
            public RECT(int l, int t, int r, int b)
            {
                this.left = l;
                this.top = t;
                this.right = r;
                this.bottom = b;
            }
        }

        /// <summary>
        /// Returned by the GetThemeMargins function to define the
        /// margins of windows that have visual styles applied.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct MARGINS
        {
            /// <summary>
            /// Width of the left border that retains its size.
            /// </summary>
            public int cxLeftWidth;

            /// <summary>
            /// Width of the right border that retains its size.
            /// </summary>
            public int cxRightWidth;

            /// <summary>
            /// Height of the top border that retains its size.
            /// </summary>
            public int cyTopHeight;

            /// <summary>
            /// Height of the bottom border that retains its size.
            /// </summary>
            public int cyBottomHeight;

            /// <summary>
            /// Initializes a new instance of the <see cref="MARGINS"/> struct.
            /// </summary>
            /// <param name="thickness">The thickness.</param>
            public MARGINS(Thickness thickness)
            {
                this.cxLeftWidth = (int)thickness.Left;
                this.cxRightWidth = (int)thickness.Right;
                this.cyTopHeight = (int)thickness.Top;
                this.cyBottomHeight = (int)thickness.Bottom;
            }
        }

        /// <summary>
        /// The MONITORINFO structure contains information about a
        /// display monitor.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public class MONITORINFO
        {
            /// <summary>
            /// The size of the structure, in bytes. Set the cbSize member to
            /// sizeof(MONITORINFO) before calling the GetMonitorInfo
            /// function. Doing so lets the function determine the type of
            /// structure you are passing to it.
            /// </summary>
            public int cbSize = Marshal.SizeOf(typeof(VistaWindowInterop.MONITORINFO));

            /// <summary>
            /// A RECT structure that specifies the display monitor
            /// rectangle, expressed in virtual-screen coordinates.
            /// </summary>
            /// Note
            /// If the monitor is not the primary display monitor, some of
            /// the rectangle's coordinates may be negative values.
            public RECT rcMonitor;

            /// <summary>
            /// A RECT structure that specifies the work area rectangle of
            /// the display monitor, expressed in virtual-screen coordinates.
            /// </summary>
            /// Note
            /// If the monitor is not the primary display monitor, some of
            /// the rectangle's coordinates may be negative values.
            public RECT rcWork;

            /// <summary>
            /// A set of flags that represent attributes of the display
            /// monitor.
            /// </summary>
            public int dwFlags;
        }

        /// <summary>
        /// The MINMAXINFO structure contains information about a
        /// window's maximized size and position and its minimum and
        /// maximum tracking size.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct MINMAXINFO
        {
            /// <summary>
            /// Reserved; do not use.
            /// </summary>
            public POINT ptReserved;

            /// <summary>
            /// Specifies the maximized width (POINT.x) and the maximized
            /// height (POINT.y) of the window. For top-level windows, this
            /// value is based on the width of the primary monitor.
            /// </summary>
            public POINT ptMaxSize;

            /// <summary>
            /// Specifies the position of the left side of the maximized
            /// window (POINT.x) and the position of the top of the maximized
            /// window (POINT.y). For top-level windows, this value is based
            /// on the position of the primary monitor.
            /// </summary>
            public POINT ptMaxPosition;

            /// <summary>
            /// Specifies the minimum tracking width (POINT.x) and the
            /// minimum tracking height (POINT.y) of the window. This value
            /// can be obtained programmatically from the system metrics
            /// SM_CXMINTRACK and SM_CYMINTRACK.
            /// </summary>
            public POINT ptMinTrackSize;

            /// <summary>
            /// Specifies the maximum tracking width (POINT.x) and the
            /// maximum tracking height (POINT.y) of the window. This value
            /// is based on the size of the virtual screen and can be
            /// obtained programmatically from the system metrics
            /// SM_CXMAXTRACK and SM_CYMAXTRACK.
            /// </summary>
            public POINT ptMaxTrackSize;
        }
        #endregion

        #region Constants
        /// <summary>
        /// Set system command
        /// </summary>
        internal const int WM_SYSCOMMAND = 0x112;

        /// <summary>
        /// Set return command 
        /// </summary>
        internal const int TPM_RETURNCMD = 0x100;

        /// <summary>
        /// Returns a handle to the display monitor that is nearest to
        /// the window.
        /// </summary>
        internal const int MONITOR_DEFAULTTONEAREST = 2;
        #endregion

        #region Implementation
        /// <summary>
        /// Defines whether this is Vista OS.
        /// </summary>
        /// <returns>
        /// True if this is Vista OS; otherwise, false.
        /// </returns>
        internal static bool IsVista()
        {
            return Environment.OSVersion.Version.Major >= 6;
        }

        /// <summary>
        /// Gets x coordinate.
        /// </summary>
        /// <param name="point">xy coordinates.</param>
        /// <returns>
        /// X coordinate.
        /// </returns>
        internal static short GetX(int point)
        {
            return (short)(point & 0xffff);
        }

        /// <summary>
        /// Gets y coordinate
        /// </summary>
        /// <param name="a_0">The a_0 value.</param>
        /// <returns>Y coordinate.</returns>
        internal static short GetY(int a_0)
        {
            return (short)((a_0 >> 0x10) & 0xffff);
        }

        /// <summary>
        /// Shows system content menu.
        /// </summary>
        /// <param name="handle">Handle to the window that will own a copy
        /// of the window menu.</param>
        /// <param name="point">Point in which the menu should be
        /// shown.</param>
        internal static void ShowSystemMenu(IntPtr handle, Point point)
        {
            IntPtr menu = GetSystemMenu(handle, false);
            int retValue = TrackPopupMenu(menu, TPM_RETURNCMD, (int)point.X, (int)point.Y, 0, handle, IntPtr.Zero);

            if (retValue != 0)
            {
                SendMessage(handle, WM_SYSCOMMAND, new IntPtr(retValue), IntPtr.Zero);
            }
        }

        /// <summary>
        /// Extends Window into client area.
        /// </summary>
        /// <param name="window">Reference to window object.</param>
        /// <param name="thickness">Thickness to extend.</param>
        internal static void ExtendWindowIntoClientArea(Window window, Thickness thickness)
        {
            IntPtr hwnd = new WindowInteropHelper(window).Handle;

            if (hwnd != IntPtr.Zero)
            {
                window.Background = Brushes.Transparent;
                HwndSource.FromHwnd(hwnd).CompositionTarget.BackgroundColor = Colors.Transparent;
                MARGINS c = new MARGINS(thickness);
                DwmExtendFrameIntoClientArea(hwnd, ref c);
            }
        }

        /// <summary>
        /// Handles minimize/maximize messages of the window.
        /// </summary>
        /// <param name="window">Reference to RibbonWindow object. </param>
        /// <param name="hwnd">Handle of the window.</param>
        /// <param name="lParam">MINMAXINFO structure.</param>
        internal static void HandleMinMax(VistaWindow window, IntPtr hwnd, IntPtr lParam)
        {
            MINMAXINFO structure = (MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(MINMAXINFO));

            IntPtr ptr = MonitorFromWindow(hwnd, MONITOR_DEFAULTTONEAREST);
            if (ptr != IntPtr.Zero)
            {
                //// int num2 = 3 + (A_0.IsGlassActive ? ((int) SystemParameters.BorderWidth) : 0);
                int borderWidth = 3;
                MONITORINFO monInfo = new MONITORINFO();
                GetMonitorInfo(ptr, monInfo);
                RECT work = monInfo.rcWork;
                RECT monitor = monInfo.rcMonitor;

                structure.ptMaxPosition.x = Math.Abs((int)(work.left - monitor.left)) - borderWidth;
                structure.ptMaxPosition.y = Math.Abs((int)(work.top - monitor.top)) - borderWidth;
                structure.ptMaxSize.x = Math.Abs((int)(work.right - work.left)) + (2 * borderWidth);
                structure.ptMaxSize.y = Math.Abs((int)(work.bottom - work.top)) + (2 * borderWidth);

                //// Minimal size for a window
                structure.ptMinTrackSize.x = 160;
                structure.ptMinTrackSize.y = 38;
            }

            Marshal.StructureToPtr(structure, lParam, true);
        }

        /// <summary>
        /// Indicates whether this object can provide its extender
        /// properties to the specified object.
        /// </summary>
        /// <returns>
        /// True if this object can provide extender properties to the
        /// specified object; otherwise, false.
        /// </returns>
        internal static bool CanExtend()
        {
            if (!IsVista() || BrowserInteropHelper.IsBrowserHosted)
            {
                return false;
            }

            bool value = false;
            DwmIsCompositionEnabled(ref value);
            return value;
        }
        #endregion
    }
}
