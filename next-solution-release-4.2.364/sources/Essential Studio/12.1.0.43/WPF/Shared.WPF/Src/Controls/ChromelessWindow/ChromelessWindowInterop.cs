// <copyright file="ChromelessWindowInterop.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws.
// </copyright>

using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace Syncfusion.Windows.Shared
{
    /// <summary>
    /// This class is responsible for Interop with Win32 Windows.
    /// </summary>
#if SyncfusionFramework4_0

    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class ChromelessWindowInterop
    {
        #region Win32 Types and API calls

        /// <summary>
        /// Sets the window pos.
        /// </summary>
        /// <param name="hwnd">The HWND value.</param>
        /// <param name="hwndInsertAfter">The HWND insert after.</param>
        /// <param name="x">The x value.</param>
        /// <param name="y">The y value.</param>
        /// <param name="cx">The cx value.</param>
        /// <param name="cy">The cy value.</param>
        /// <param name="wFlags">The w flags.</param>
        /// <returns>Return the value</returns>
        [DllImport("user32", EntryPoint = "SetWindowPos")]
        internal static extern int SetWindowPos(IntPtr hwnd, int hwndInsertAfter, int x, int y, int cx, int cy, int wFlags);

        /// <summary>
        /// Gets the window rect.
        /// </summary>
        /// <param name="hWnd">The h WND.</param>
        /// <param name="r">The r value.</param>
        /// <returns>Return the value</returns>
        [DllImport("user32", EntryPoint = "GetWindowRect")]
        internal static extern bool GetWindowRect(IntPtr hWnd, ref ChromelessWindowInterop.RECT r);

        /// <summary>
        /// Extends the window frame behind the client area.
        /// </summary>
        /// <param name="hWnd">Handle to the window of interest.</param>
        /// <param name="pMarInset">The pointer to a MARGINS Structure
        /// structure that describes the margins
        /// to use when extending the frame into
        /// the client area.</param>
        /// <property name="flag" value="Finished"/>
        [DllImport("dwmapi.dll", CharSet = CharSet.Auto)]
        internal static extern void DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS pMarInset);

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
        /// <property name="flag" value="Finished"/>
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
        /// set the cbSize member of the
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
        /// MONITORINFO structure.</param>
        /// <returns>
        /// If the function succeeds, the return value is nonzero. If the
        /// function fails, the return value is zero. Windows NT/2000/XP:
        /// To get extended error information, call GetLastError.
        /// </returns>
        /// <property name="flag" value="Finished"/>
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        internal static extern bool GetMonitorInfo(IntPtr hMonitor, MONITORINFO lpmi);

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
        /// <property name="flag" value="Finished"/>
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
        /// one of the following values.</param>
        /// <returns>
        /// If the window intersects one or more display monitor
        /// rectangles, the return value is an HMONITOR handle to the
        /// display monitor that has the largest area of intersection
        /// with the window. If the window does not intersect a display
        /// monitor, the return value depends on the value of dwFlags.
        /// </returns>
        /// <property name="flag" value="Finished"/>
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        internal static extern IntPtr MonitorFromWindow(IntPtr hwnd, int dwFlags);

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
        /// information value.</param>
        /// <returns>
        /// The return value specifies the result of the message
        /// processing; it depends on the message sent.
        /// </returns>
        /// <property name="flag" value="Finished"/>
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);

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
        /// If you do not specify TPM_RETURNCMD in the uFlags parameter,
        /// the return value is nonzero if the function succeeds and zero
        /// if it fails. To get extended error information, call
        /// GetLastError.
        /// </returns>
        /// <property name="flag" value="Finished"/>
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        [CLSCompliant(false)]
        public static extern int TrackPopupMenu(IntPtr hMenu, UInt32 uFlags, int x, int y, int nReserved, IntPtr hWnd, IntPtr prcRect);

        /// <summary>
        /// Creates the round rect RGN.
        /// </summary>
        /// <param name="x1">The x1 value.</param>
        /// <param name="y1">The y1 value.</param>
        /// <param name="x2">The x2 value.</param>
        /// <param name="y2">The y2 value.</param>
        /// <param name="cx">The cx value.</param>
        /// <param name="cy">The cy value.</param>
        /// <returns>Return the point.</returns>
        [DllImport("gdi32.dll")]
        internal static extern IntPtr CreateRoundRectRgn(int x1, int y1, int x2, int y2, int cx, int cy);

        /// <summary>
        /// Sets the window RGN.
        /// </summary>
        /// <param name="hWnd">The h WND.</param>
        /// <param name="hRgn">The h RGN.</param>
        /// <param name="bRedraw">if set to <c>true</c> [b redraw].</param>
        /// <returns>Return the point.</returns>
        [DllImport("user32.dll")]
        internal static extern int SetWindowRgn(IntPtr hWnd, IntPtr hRgn, bool bRedraw);

        /// <summary>
        /// Gets the window long.
        /// </summary>
        /// <param name="hWnd">The h WND.</param>
        /// <param name="nIndex">Index of the n.</param>
        /// <returns>Return the point.</returns>
        [DllImport("user32")]
        internal static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        /// <summary>
        /// Sets the window long.
        /// </summary>
        /// <param name="hWnd">The h WND.</param>
        /// <param name="nIndex">Index of the n.</param>
        /// <param name="dwNewLong">The dw new long.</param>
        /// <returns>Return the point.</returns>
        [DllImport("user32")]
        internal static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        /// <summary>
        /// Sets the window pos.
        /// </summary>
        /// <param name="hWnd">The h WND.</param>
        /// <param name="hWndInsertAfter">The h WND insert after.</param>
        /// <param name="x">The X value.</param>
        /// <param name="y">The Y value.</param>
        /// <param name="cx">The cx value.</param>
        /// <param name="cy">The cy value.</param>
        /// <param name="uFlags">The u flags.</param>
        /// <returns>Return the point.</returns>
        [DllImport("user32")]
        internal static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, int uFlags);

        /// <summary>
        /// Gets the cursor pos.
        /// </summary>
        /// <param name="lpPoint">The lp point.</param>
        /// <returns>Return the point.</returns>
        [DllImport("user32.dll")]
        internal static extern bool GetCursorPos(out POINT lpPoint);

        /// <summary>
        /// Adjusts the window rect ex.
        /// </summary>
        /// <param name="lpRECT">The lp RECT.</param>
        /// <param name="dwStyle">The dw style.</param>
        /// <param name="bMenu">if set to <c>true</c> [b menu].</param>
        /// <param name="dwExStyle">The dw ex style.</param>
        /// <returns>Return the point.</returns>
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        internal static extern bool AdjustWindowRectEx(ref RECT lpRECT, int dwStyle, bool bMenu, int dwExStyle);

        /// <summary>
        /// DWMs the def window proc.
        /// </summary>
        /// <param name="hwnd">The HWND value.</param>
        /// <param name="msg">The MSG value.</param>
        /// <param name="wParam">The w param.</param>
        /// <param name="lParam">The l param.</param>
        /// <param name="plResult">The pl result.</param>
        /// <returns>Return the point.</returns>
        [DllImport("dwmapi.dll", CharSet = CharSet.Auto)]
        internal static extern int DwmDefWindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, out IntPtr plResult);

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Represents an x- and y-coordinate pair in two-dimensional
        /// space.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct POINT
        {
            /// <property name="flag" value="Finished" />
            /// <summary>
            /// The x-coordinate of this Point.
            /// </summary>
            internal int x;

            /// <property name="flag" value="Finished" />
            /// <summary>
            /// The y-coordinate of this Point.
            /// </summary>
            internal int y;

            /// <summary>
            /// Initializes a new instance of the <see cref="POINT"/> struct.
            /// </summary>
            /// <param name="x1">The x1 value.</param>
            /// <param name="y1">The y1 value.</param>
            internal POINT(int x1, int y1)
            {
                this.x = x1;
                this.y = y1;
            }

            /// <summary>
            /// Gets the location.
            /// </summary>
            /// <value>The location.</value>
            internal Point Location
            {
                get { return new Point(x, y); }
            }
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// The RECT structure defines the coordinates of the upper-left
        /// and lower-right corners of a rectangle.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct RECT
        {
            /// <property name="flag" value="Finished" />
            /// <summary>
            /// Specifies the x-coordinate of the upper-left corner of the
            /// rectangle.
            /// </summary>
            internal int left;

            /// <property name="flag" value="Finished" />
            /// <summary>
            /// Specifies the y-coordinate of the upper-left corner of the
            /// rectangle.
            /// </summary>
            internal int top;

            /// <property name="flag" value="Finished" />
            /// <summary>
            /// Specifies the x-coordinate of the lower-right corner of the
            /// rectangle.
            /// </summary>
            internal int right;

            /// <property name="flag" value="Finished" />
            /// <summary>
            /// Specifies the y-coordinate of the lower-right corner of the
            /// rectangle.
            /// </summary>
            internal int bottom;

            /// <summary>
            /// Initializes a new instance of the <see cref="RECT"/> struct.
            /// </summary>
            /// <param name="l">The l value.</param>
            /// <param name="t">The t value.</param>
            /// <param name="r">The r value.</param>
            /// <param name="b">The b value.</param>
            internal RECT(int l, int t, int r, int b)
            {
                this.left = l;
                this.top = t;
                this.right = r;
                this.bottom = b;
            }

            /// <summary>
            /// Gets the height.
            /// </summary>
            /// <value>The height.</value>
            internal int Height
            {
                get
                {
                    return (int)(bottom - top);
                }
            }

            /// <summary>
            /// Gets the width.
            /// </summary>
            /// <value>The width.</value>
            internal int Width
            {
                get
                {
                    return (int)(right - left);
                }
            }

            /// <summary>
            /// Toes the rectangle.
            /// </summary>
            /// <returns>Return the rectangle</returns>
            internal Rect ToRectangle()
            {
                return new Rect((double)left, (double)top, (double)(right - left), (double)(bottom - top));
            }

            /// <summary>
            /// Gets the location.
            /// </summary>
            /// <value>The location.</value>
            internal System.Drawing.Point Location
            {
                get
                {
                    return new System.Drawing.Point(left, top);
                }
            }

            /// <summary>
            /// Forms the rectangle.
            /// </summary>
            /// <param name="r">The r value.</param>
            /// <returns>Return the rectangle</returns>
            internal static RECT FromRectangle(Rect r)
            {
                return new RECT(
                    (int)((int)Math.Ceiling(r.Left)),
                    (int)((int)Math.Ceiling(r.Top)),
                    (int)((int)Math.Ceiling(r.Right)),
                    (int)((int)Math.Ceiling(r.Bottom)));
            }

            /// <summary>
            /// Gets the extended rect.
            /// </summary>
            /// <param name="rect">The rect value.</param>
            /// <param name="thickness">The thickness.</param>
            /// <param name="padding"></param>
            /// <param name="Margin"></param>
            /// <returns>Return the rectangle</returns>
            internal static Rect GetExtendedRect(Rect rect, Thickness thickness, Thickness padding, Thickness Margin)
            {
                return new Rect((double)(rect.Left + thickness.Left + padding.Left + Margin.Left), (double)(rect.Top + thickness.Top + padding.Top + Margin.Top), Math.Max(0.0, (double)((rect.Width - (thickness.Left + padding.Left)) - (thickness.Right + padding.Right)) - (Margin.Left + Margin.Right)), Math.Max(0.0, (double)((rect.Height - (thickness.Top + padding.Top)) - (thickness.Bottom + padding.Bottom))) - (Margin.Top + Margin.Bottom));
            }
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Returned by the GetThemeMargins function to define the
        /// margins of windows that have visual styles applied.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct MARGINS
        {
            /// <property name="flag" value="Finished" />
            /// <summary>
            /// Width of the left border that retains its size.
            /// </summary>
            internal int cxLeftWidth;

            /// <property name="flag" value="Finished" />
            /// <summary>
            /// Width of the right border that retains its size.
            /// </summary>
            internal int cxRightWidth;

            /// <property name="flag" value="Finished" />
            /// <summary>
            /// Height of the top border that retains its size.
            /// </summary>
            internal int cyTopHeight;

            /// <property name="flag" value="Finished" />
            /// <summary>
            /// Height of the bottom border that retains its size.
            /// </summary>
            internal int cyBottomHeight;

            /// <summary>
            /// Initializes a new instance of the <see cref="MARGINS"/> struct.
            /// </summary>
            /// <param name="thickness">The thickness.</param>
            internal MARGINS(Thickness thickness)
            {
                this.cxLeftWidth = (int)thickness.Left;
                this.cxRightWidth = (int)thickness.Right;
                this.cyTopHeight = (int)thickness.Top;
                this.cyBottomHeight = (int)thickness.Bottom;
            }
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// The MONITORINFO structure contains information about a
        /// display monitor.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal class MONITORINFO
        {
            /// <property name="flag" value="Finished" />
            /// <summary>
            /// The size of the structure, in bytes. Set the cbSize member to
            /// sizeof(MONITORINFO) before calling the GetMonitorInfo
            /// function. Doing so lets the function determine the type of
            /// structure you are passing to it.
            /// </summary>
            internal int cbSize = Marshal.SizeOf(typeof(ChromelessWindowInterop.MONITORINFO));

            /// <property name="flag" value="Finished" />
            /// <summary>
            /// A RECT structure that specifies the display monitor
            /// rectangle, expressed in virtual-screen coordinates.
            /// </summary>
            /// Note
            /// If the monitor is not the primary display monitor, some of
            /// the rectangle's coordinates may be negative values.
            internal RECT rcMonitor;

            /// <property name="flag" value="Finished" />
            /// <summary>
            /// A RECT structure that specifies the work area rectangle of
            /// the display monitor, expressed in virtual-screen coordinates.
            /// </summary>
            /// Note
            /// If the monitor is not the primary display monitor, some of
            /// the rectangle's coordinates may be negative values.
            internal RECT rcWork;

            /// <property name="flag" value="Finished" />
            /// <summary>
            /// A set of flags that represent attributes of the display
            /// monitor.
            /// </summary>
            internal int dwFlags;
        }

        /// <summary>
        /// The MINMAXINFO structure contains information about a
        /// window's maximized size and position and its minimum and
        /// maximum tracking size.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct MINMAXINFO
        {
            /// <property name="flag" value="Finished" />
            /// <summary>
            /// Reserved; do not use.
            /// </summary>
            internal POINT ptReserved;

            /// <property name="flag" value="Finished" />
            /// <summary>
            /// Specifies the maximized width (POINT.x) and the maximized
            /// height (POINT.y) of the window. For top-level windows, this
            /// value is based on the width of the primary monitor.
            /// </summary>
            internal POINT ptMaxSize;

            /// <property name="flag" value="Finished" />
            /// <summary>
            /// Specifies the position of the left side of the maximized
            /// window (POINT.x) and the position of the top of the maximized
            /// window (POINT.y). For top-level windows, this value is based
            /// on the position of the primary monitor.
            /// </summary>
            internal POINT ptMaxPosition;

            /// <property name="flag" value="Finished" />
            /// <summary>
            /// Specifies the minimum tracking width (POINT.x) and the
            /// minimum tracking height (POINT.y) of the window. This value
            /// can be obtained programmatically from the system metrics
            /// SM_CXMINTRACK and SM_CYMINTRACK.
            /// </summary>
            internal POINT ptMinTrackSize;

            /// <property name="flag" value="Finished" />
            /// <summary>
            /// Specifies the maximum tracking width (POINT.x) and the
            /// maximum tracking height (POINT.y) of the window. This value
            /// is based on the size of the virtual screen and can be
            /// obtained programmatically from the system metrics
            /// SM_CXMAXTRACK and SM_CYMAXTRACK.
            /// </summary>
            internal POINT ptMaxTrackSize;
        }

        /// <summary>
        /// Structure for Window position
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct WINDOWPOS
        {
            /// <summary>
            /// Define window position
            /// </summary>
            internal IntPtr hwnd;

            /// <summary>
            /// Define window position insert after
            /// </summary>
            internal IntPtr hwndInsertAfter;

            /// <summary>
            /// Define window x position
            /// </summary>
            internal int x;

            /// <summary>
            /// Define window y position
            /// </summary>
            internal int y;

            /// <summary>
            /// Define window cx position
            /// </summary>
            internal int cx;

            /// <summary>
            /// Define window cy position
            /// </summary>
            internal int cy;

            /// <summary>
            /// Define the flags
            /// </summary>
            internal int flags;
        }

        /// <summary>
        /// Define Rectangle size param
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct NCCALCSIZE_PARAMS
        {
            /// <summary>
            /// Define rectangle left size
            /// </summary>
            internal RECT rgrc0;

            /// <summary>
            /// Define rectangle right size
            /// </summary>
            internal RECT rgrc1;

            /// <summary>
            /// Define rectangle top
            /// </summary>
            internal RECT rgrc2;

            /// <summary>
            /// Define rectangle bottom
            /// </summary>
            internal IntPtr lppos;
        }

        #endregion Win32 Types and API calls

        #region Constants

        /// <summary>
        /// Define system command
        /// </summary>
        internal const int WM_SYSCOMMAND = 0x112;

        /// <summary>
        /// Define button
        /// </summary>
        internal const int WM_LBUTTONUP = 0x0202;

        /// <summary>
        /// Define return command
        /// </summary>
        internal const int TPM_RETURNCMD = 0x100;

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Returns a handle to the display monitor that is nearest to
        /// the window.
        /// </summary>
        internal const int MONITOR_DEFAULTTONEAREST = 2;

        #endregion Constants

        #region Implementation

        /// <summary>
        /// Defines whether this is Vista OS.
        /// </summary>
        /// <returns>
        /// True if this is Vista OS; otherwise, false.
        /// </returns>
        /// <property name="flag" value="Finished"/>
        internal static bool IsVista()
        {
            return Environment.OSVersion.Version.Major >= 6;
        }

        /// <summary>
        /// Gets x coordinate.
        /// </summary>
        /// <param name="point">xy coordinates.</param>
        /// <returns>X coordinate.</returns>
        /// <property name="flag" value="Finished"/>
        internal static short GetX(int point)
        {
            return (short)(point & 0xffff);
        }

        /// <summary>
        /// Gets y coordinate
        /// </summary>
        /// <param name="point">The point value.</param>
        /// <returns>Y coordinate.</returns>
        /// <property name="flag" value="Finished"/>
        internal static short GetY(int point)
        {
            return (short)((point >> 0x10) & 0xffff);
        }

        /// <summary>
        /// Gets the X.
        /// </summary>
        /// <param name="point">The point value.</param>
        /// <returns>Return the x position of window</returns>
        internal static int GetX(IntPtr point)
        {
            return GetX((int)point);
        }

        /// <summary>
        /// Gets the Y.
        /// </summary>
        /// <param name="point">The point value.</param>
        /// <returns>Return the y position of window</returns>
        internal static int GetY(IntPtr point)
        {
            return GetX((int)point);
        }

        /// <summary>
        /// Shows system content menu.
        /// </summary>
        /// <param name="handle">Handle to the window that will own a copy
        /// of the window menu.</param>
        /// <param name="point">Point in which the menu should be
        /// shown.</param>
        /// <property name="flag" value="Finished"/>
        internal static void ShowSystemMenu(IntPtr handle, Point point)
        {
            IntPtr menu = GetSystemMenu(handle, false);

            int retValue = TrackPopupMenu(menu, TPM_RETURNCMD, (int)point.X, (int)point.Y, 0, handle, IntPtr.Zero);

            if (retValue != 0)
            {
                SendMessage(handle, WM_SYSCOMMAND, retValue, 0);
            }
        }

        /// <summary>
        /// Handles minimize/maximize messages of the window.
        /// </summary>
        /// <param name="window">Reference to RibbonWindow object.</param>
        /// <param name="hwnd">Handle of the window.</param>
        /// <param name="lParam">MINMAXINFO structure.</param>
        /// <property name="flag" value="Finished"/>
        internal static void HandleMinMax(ChromelessWindow window, IntPtr hwnd, IntPtr lParam)
        {
            MINMAXINFO structure = (MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(MINMAXINFO));

            IntPtr ptr = MonitorFromWindow(hwnd, MONITOR_DEFAULTTONEAREST);
            if (ptr != IntPtr.Zero)
            {
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
        /// <property name="flag" value="Finished"/>
        internal static bool CanEnableDwm()
        {
            if (!IsVista() || BrowserInteropHelper.IsBrowserHosted)
            {
                return false;
            }

            bool value = false;
            DwmIsCompositionEnabled(ref value);
            return value;
        }

        /// <summary>
        /// Gets the transformed point.
        /// </summary>
        /// <param name="visual">The visual.</param>
        /// <returns>Return the transformed position</returns>
        internal static Point GetTransformedPoint(Visual visual)
        {
            PresentationSource source = PresentationSource.FromVisual(visual);
            Point result = new Point((double)120.0, (double)120.0);

            if (source != null)
            {
                MatrixTransform transform = new MatrixTransform(source.CompositionTarget.TransformToDevice);
                Point zeroPoint = new Point((double)0.0, (double)0.0);
                zeroPoint = transform.Transform(zeroPoint);
                Point point = new Point((double)96.0, (double)96.0);
                point = transform.Transform(point);

                result.X = (double)(point.X - zeroPoint.X);
                result.Y = (double)(point.Y - zeroPoint.Y);
            }

            return result;
        }

        /// <summary>
        /// Extends the window.
        /// </summary>
        /// <param name="hWnd">The h WND value.</param>
        /// <param name="size">The size value.</param>
        internal static void ExtendWindow(IntPtr hWnd, int size)
        {
            MARGINS pMargins = new MARGINS();

            pMargins.cxLeftWidth = 0;
            pMargins.cxRightWidth = 0;
            pMargins.cyTopHeight = size;
            pMargins.cyBottomHeight = 0;

            DwmExtendFrameIntoClientArea(hWnd, ref pMargins);
        }

        #endregion Implementation

        #region Enum

        /// <summary>
        /// Sizing Direction
        /// </summary>
        public enum SizingDirection
        {
            /// <summary>
            /// The none value
            /// </summary>
            None,

            /// <summary>
            /// The west value
            /// </summary>
            West,

            /// <summary>
            /// The east value
            /// </summary>
            East,

            /// <summary>
            /// The north value
            /// </summary>
            North,

            /// <summary>
            /// The northwest value
            /// </summary>
            NorthWest,

            /// <summary>
            /// The northeast value
            /// </summary>
            NorthEast,

            /// <summary>
            /// The south value
            /// </summary>
            South,

            /// <summary>
            /// The southwest value
            /// </summary>
            SouthWest,

            /// <summary>
            /// The southeast value
            /// </summary>
            SouthEast
        }

        #endregion Enum
    }

    #region Enum

    /// <summary>
    /// Define Window Messages
    /// </summary>
    internal enum WindowsMessages
    {
        /// <summary>
        /// Define composition changed
        /// </summary>
        WM_DWMCOMPOSITIONCHANGED = 0x31e,

        /// <summary>
        /// Define enter move size
        /// </summary>
        WM_ENTERSIZEMOVE = 0x231,

        /// <summary>
        /// Define exit move size
        /// </summary>
        WM_EXITSIZEMOVE = 0x232,

        /// <summary>
        /// Define  Max/Min info
        /// </summary>
        WM_GETMINMAXINFO = 0x24,

        /// <summary>
        /// Define cancel size
        /// </summary>
        WM_NCCALCSIZE = 0x83,

        /// <summary>
        /// Define test
        /// </summary>
        WM_NCHITTEST = 0x84,

        /// <summary>
        /// Define size
        /// </summary>
        WM_SIZE = 5,

        /// <summary>
        /// Define sizing
        /// </summary>
        WM_SIZING = 0x214,

        /// <summary>
        /// Define system command
        /// </summary>
        WM_SYSCOMMAND = 0x112,

        /// <summary>
        /// Define valid rectangle
        /// </summary>
        WVR_VALIDRECTS = 0x400,

        /// <summary>
        /// Define position changing
        /// </summary>
        WM_WINDOWPOSCHANGING = 0x0046,

        /// <summary>
        /// Define position changed
        /// </summary>
        WM_WINDOWPOSCHANGED = 0x0047
    }

    /// <summary>
    /// Define Window Position Params
    /// </summary>
    [Flags]
    internal enum SetWindowPosParams
    {
        /// <summary>
        /// Define frame changed
        /// </summary>
        SWP_FRAMECHANGED = 0x20,

        /// <summary>
        /// Define no move
        /// </summary>
        SWP_NOMOVE = 2,

        /// <summary>
        /// Define no size
        /// </summary>
        SWP_NOSIZE = 1,

        /// <summary>
        /// Define no order
        /// </summary>
        SWP_NOZORDER = 4
    }

    /// <summary>
    /// Define Style
    /// </summary>
    internal enum WindowLongValues
    {
        /// <summary>
        /// Define ex-style
        /// </summary>
        GWL_EXSTYLE = -20,

        /// <summary>
        /// Define style
        /// </summary>
        GWL_STYLE = -16
    }

    #endregion Enum
}