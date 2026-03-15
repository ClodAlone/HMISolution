// <copyright file="NativeMethods.cs" company="Syncfusion">
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
using System.Security;
using System.Diagnostics;
using System.ComponentModel;
using System.Windows;
using System.Diagnostics.CodeAnalysis;

namespace Syncfusion.Windows.Shared
{
    /// <summary>
    /// Represents window layout information.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct WINDOWPOS
    {
        /// <summary>
        /// Handle of window.
        /// </summary>
        public IntPtr hwnd;

        /// <summary>
        /// Either the handle of the window to position this window behind, or a flag stating where in the Z-order to put the window:
        /// * -2 = Put the window below all topmost windows and above all non-topmost windows.
        /// * -1 = Make the window topmost (above all other windows) permanently.
        /// * 0 = Put the window at the top of the Z-order.
        /// * 1 = Put the window at the bottom of the Z-order. 
        /// </summary>
        public IntPtr hwndInsertAfter;

        /// <summary>
        /// The x coordinate of where to put the upper-left corner of the window.
        /// </summary>
        public int x;

        /// <summary>
        /// The y coordinate of where to put the upper-left corner of the window.
        /// </summary>
        public int y;

        /// <summary>
        /// The x coordinate of where to put the lower-right corner of the window.
        /// </summary>
        public int cx;

        /// <summary>
        /// The y coordinate of where to put the lower-right corner of the window.
        /// </summary>
        public int cy;

        /// <summary>
        /// Zero or more of the following flags stating how to move the window:
        /// * 32 = Fully redraw the window in its new position.
        /// * 128 = Hide the window from the screen.
        /// * 16 = Do not make the window active after moving it unless it was already the active window.
        /// * 256 = Do not redraw anything drawn on the window after it is moved.
        /// * 2 = Do not move the window.
        /// * 1 = Do not resize the window. 
        /// * 8 = Do not remove the image of the window in its former position, effectively leaving a ghost image on the screen.
        /// * 4 = Do not change the window's position in the Z-order.
        /// * 64 = Show the window if it is hidden. 
        /// </summary>
        public int flags;
    }

    /// <summary>
    /// GetWindowLongPtr values, GWL_*
    /// </summary>
    public enum GWL
    {
        /// <summary>
        /// Value for WNDPROC
        /// </summary>
        WNDPROC = (-4),

        /// <summary>
        /// Value for HINSTANCE
        /// </summary>
        HINSTANCE = (-6),

        /// <summary>
        /// Value for HWNDPARENT
        /// </summary>
        HWNDPARENT = (-8),

        /// <summary>
        /// Value for STYLE
        /// </summary>
        STYLE = (-16),

        /// <summary>
        /// Value for EXSTYLE
        /// </summary>
        EXSTYLE = (-20),

        /// <summary>
        /// Value for USERDATA
        /// </summary>
        USERDATA = (-21),

        /// <summary>
        /// Value for ID
        /// </summary>
        ID = (-12)
    }

    /// <summary>
    /// Represents point.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public class POINT
    {
        #region Public members

        /// <summary>
        /// x coordinate.
        /// </summary>
        public int x;

        /// <summary>
        /// y  coordinate.
        /// </summary>
        public int y;

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="POINT"/> class.
        /// </summary>
        public POINT()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="POINT"/> class.
        /// </summary>
        /// <param name="x">The x value.</param>
        /// <param name="y">The y value.</param>
        public POINT(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
        #endregion

        #region Overrides

        /// <summary>
        ///  Returns a <see cref="String"/> that represents the current <see cref="POINT"/>.
        /// </summary>
        /// <returns>A <see cref="String"/> that represents the current <see cref="POINT"/>.</returns>
        public override string ToString()
        {
            return "{" + x.ToString() + "; " + y.ToString() + "}";
        }
        #endregion
    }

    /// <summary>
    /// The RECT structure defines the coordinates of the upper-left
    /// and lower-right corners of a rectangle.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        #region Public members

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
        #endregion

        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="RECT"/> struct.
        /// </summary>
        /// <param name="lefttop"><see cref="Point"/> that represents upper-left corner of the rectangle.</param>
        /// <param name="rightbottom"><see cref="Point"/> that represents lower-right corner of the rectangle.</param>
        public RECT(Point lefttop, Point rightbottom)
            : this((int)lefttop.X, (int)lefttop.Y, (int)rightbottom.X, (int)rightbottom.Y)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RECT"/> struct.
        /// </summary>
        /// <param name="left">The x-coordinate of the upper-left corner of the
        /// rectangle.</param>
        /// <param name="top">The y-coordinate of the upper-left corner of the
        /// rectangle.</param>
        /// <param name="right">The x-coordinate of the lower-right corner of the
        /// rectangle.</param>
        /// <param name="bottom">The y-coordinate of the lower-right corner of the
        /// rectangle.</param>
        public RECT(int left, int top, int right, int bottom)
        {
            this.left = left;
            this.top = top;
            this.right = right;
            this.bottom = bottom;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RECT"/> struct.
        /// </summary>
        /// <param name="r">The rect value.</param>
        public RECT(Rect r)
        {
            this.left = (int)r.Left;
            this.top = (int)r.Top;
            this.right = (int)r.Right;
            this.bottom = (int)r.Bottom;
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Creates new <see cref="RECT"/> from given parameters.
        /// </summary>
        /// <param name="x">The x coordinate of the upper-left corner.</param>
        /// <param name="y">The y coordinate of the upper-left corner.</param>
        /// <param name="width">Given width.</param>
        /// <param name="height">Given height.</param>
        /// <returns>A new <see cref="RECT"/>.</returns>
        public static RECT FromXYWH(int x, int y, int width, int height)
        {
            return new RECT(x, y, x + width, y + height);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets actual size of the rectangle.
        /// </summary>
        public Size Size
        {
            get
            {
                return new Size(this.right - this.left, this.bottom - this.top);
            }
        }
        #endregion
    }

    /// <summary>
    /// Corresponding Result with hit test resultant value.
    /// </summary>
    internal enum HitTestResults
    {
        /// <summary>
        /// Error message
        /// </summary>
        Error = -2,

        /// <summary>
        /// Transparent message.
        /// </summary>
        Transparent = -1,

        /// <summary>
        /// Nowhere message.
        /// </summary>
        Nowhere = 0,

        /// <summary>
        /// Client message.
        /// </summary>
        Client = 1,

        /// <summary>
        /// Caption message.
        /// </summary>
        Caption = 2,

        /// <summary>
        /// Sysmenu message
        /// </summary>
        Sysmenu = 3,

        /// <summary>
        /// GrowBox message
        /// </summary>
        GrowBox = 4,

        /// <summary>
        /// Menu message
        /// </summary>
        Menu = 5,

        /// <summary>
        /// HScroll message
        /// </summary>
        HScroll = 6,

        /// <summary>
        /// VScroll message
        /// </summary>
        VScroll = 7,

        /// <summary>
        /// ButtonMin message
        /// </summary>
        ButtonMin = 8,

        /// <summary>
        /// ButtonMax message
        /// </summary>
        ButtonMax = 9,

        /// <summary>
        /// BorderLeft message
        /// </summary>
        BorderLeft = 10,

        /// <summary>
        /// BorderRight message
        /// </summary>
        BorderRight = 11,

        /// <summary>
        /// BorderTop message
        /// </summary>
        BorderTop = 12,

        /// <summary>
        /// BorderBottom message
        /// </summary>
        BorderBottom = 15,

        /// <summary>
        /// CornerLeftTop message
        /// </summary>
        CornerLeftTop = 13,

        /// <summary>
        /// CornerTopRight message
        /// </summary>
        CornerTopRight = 14,

        /// <summary>
        /// CornerTopLeft message
        /// </summary>
        CornerTopLeft = 16,

        /// <summary>
        /// CornerBottonRight message
        /// </summary>
        CornerBottonRight = 17,

        /// <summary>
        /// Border message
        /// </summary>
        Border = 18,

        /// <summary>
        /// Object message
        /// </summary>
        Object = 19,

        /// <summary>
        /// Close message
        /// </summary>
        Close = 20,

        /// <summary>
        /// Help message
        /// </summary>
        Help = 21
    }

    /// <summary>
    /// EnableMenuItem uEnable values
    /// </summary>
    [Flags][CLSCompliant(false)]
    public enum SystemMenuItemBehavior : uint
    {
        /// <summary>
        /// Possible return value for EnableMenuItem
        /// </summary>
        DOES_NOT_EXIST = unchecked((uint)-1),

        /// <summary>
        /// Value for Enabled
        /// </summary>
        ENABLED = 0,

        /// <summary>
        /// Value for BYCOMMAND
        /// </summary>
        BYCOMMAND = 0,

        /// <summary>
        /// Value for GRAYED
        /// </summary>
        GRAYED = 1,

        /// <summary>
        /// Value for DISABLED
        /// </summary>
        DISABLED = 2,
    }

    /// <summary>
    /// WindowStyle values
    /// </summary>
    [Flags][CLSCompliant (false)]
    public enum WindowStyleValues : uint
    {
        /// <summary>
        /// Value for overlapped
        /// </summary>
        OVERLAPPED = 0x00000000,

        /// <summary>
        /// Value for POPUP
        /// </summary>
        POPUP = 0x80000000,

        /// <summary>
        /// Value for CHILD
        /// </summary>
        CHILD = 0x40000000,

        /// <summary>
        /// Value for MINIMIZE
        /// </summary>
        MINIMIZE = 0x20000000,

        /// <summary>
        /// Value for VISIBLE
        /// </summary>
        VISIBLE = 0x10000000,

        /// <summary>
        /// Value for Disabled
        /// </summary>
        DISABLED = 0x08000000,

        /// <summary>
        /// Value for CLIPSIBLINGS
        /// </summary>
        CLIPSIBLINGS = 0x04000000,

        /// <summary>
        /// Value for CLIPCHILDREN
        /// </summary>
        CLIPCHILDREN = 0x02000000,

        /// <summary>
        /// Value for maximize
        /// </summary>
        MAXIMIZE = 0x01000000,

        /// <summary>
        /// Value for border
        /// </summary>
        BORDER = 0x00800000,

        /// <summary>
        /// Value for DLGFRAME
        /// </summary>
        DLGFRAME = 0x00400000,

        /// <summary>
        /// Value for VSCROLL
        /// </summary>
        VSCROLL = 0x00200000,

        /// <summary>
        /// Value for hscroll
        /// </summary>
        HSCROLL = 0x00100000,

        /// <summary>
        /// Value for sysmenu
        /// </summary>
        SYSMENU = 0x00080000,

        /// <summary>
        /// Value for thickframe
        /// </summary>
        THICKFRAME = 0x00040000,

        /// <summary>
        /// Value for group
        /// </summary>
        GROUP = 0x00020000,

        /// <summary>
        /// Value for tabstop
        /// </summary>
        TABSTOP = 0x00010000,

        /// <summary>
        /// Value for minimizebox
        /// </summary>
        MINIMIZEBOX = 0x00020000,

        /// <summary>
        /// Value for maximizebox
        /// </summary>
        MAXIMIZEBOX = 0x00010000,

        /// <summary>
        /// Value for caption
        /// </summary>
        CAPTION = BORDER | DLGFRAME,

        /// <summary>
        /// Value for tiled
        /// </summary>
        TILED = OVERLAPPED,

        /// <summary>
        /// Value for MINIMIZE
        /// </summary>
        ICONIC = MINIMIZE,

        /// <summary>
        /// Value for SIZEBOX
        /// </summary>
        SIZEBOX = THICKFRAME,

        /// <summary>
        /// Value for titledwindow
        /// </summary>
        TILEDWINDOW = OVERLAPPEDWINDOW,

        /// <summary>
        /// Value for overlappedwindow
        /// </summary>
        OVERLAPPEDWINDOW = OVERLAPPED | CAPTION | SYSMENU | THICKFRAME | MINIMIZEBOX | MAXIMIZEBOX,

        /// <summary>
        /// Value for popupwindow
        /// </summary>
        POPUPWINDOW = POPUP | BORDER | SYSMENU,

        /// <summary>
        /// Value for childwindow
        /// </summary>
        CHILDWINDOW = CHILD,
    }

    /// <summary>
    /// System menu options
    /// </summary>
    public enum SystemCommands
    {
        /// <summary>
        /// Resize the window.
        /// </summary>
        SIZE = 0xF000,

        /// <summary>
        /// Move the window.
        /// </summary>
        MOVE = 0xF010,

        /// <summary>
        /// Minimize state.
        /// </summary>
        MINIMIZE = 0xF020,

        /// <summary>
        /// Maximize state.
        /// </summary>
        MAXIMIZE = 0xF030,

        /// <summary>
        /// Next window state.
        /// </summary>
        NEXTWINDOW = 0xF040,

        /// <summary>
        /// Preview window state.
        /// </summary>
        PREVWINDOW = 0xF050,

        /// <summary>
        /// close state.
        /// </summary>
        CLOSE = 0xF060,

        /// <summary>
        /// Vertical scroll state.
        /// </summary>
        VSCROLL = 0xF070,

        /// <summary>
        /// Horizontal scroll state.
        /// </summary>
        HSCROLL = 0xF080,

        /// <summary>
        /// Mouse menu state.
        /// </summary>
        MOUSEMENU = 0xF090,

        /// <summary>
        /// Keyboard menu state.
        /// </summary>
        KEYMENU = 0xF100,

        /// <summary>
        /// Arrange window state.
        /// </summary>
        ARRANGE = 0xF110,

        /// <summary>
        /// Restore state.
        /// </summary>
        RESTORE = 0xF120,

        /// <summary>
        /// Task List state.
        /// </summary>
        TASKLIST = 0xF130,

        /// <summary>
        /// Screen save state.
        /// </summary>
        SCREENSAVE = 0xF140,

        /// <summary>
        /// Hot key state.
        /// </summary>
        HOTKEY = 0xF150,

        /// <summary>
        /// Default state.
        /// </summary>
        DEFAULT = 0xF160,

        /// <summary>
        /// Monitor power state.
        /// </summary>
        MONITORPOWER = 0xF170,

        /// <summary>
        /// Context help state.
        /// </summary>
        CONTEXTHELP = 0xF180,

        /// <summary>
        /// Separator state.
        /// </summary>
        SEPARATOR = 0xF00F,

        /// <summary>
        /// SCF_ISSECURE state.
        /// </summary>
        F_ISSECURE = 0x00000001,

        /// <summary>
        /// Icon image state.
        /// </summary>
        ICON = MINIMIZE,

        /// <summary>
        /// Zooming state.
        /// </summary>
        ZOOM = MAXIMIZE,
    }

    /// <summary>
    /// ShowWindow options.
    /// </summary>
    public enum ShowWindowOptions
    {
        /// <summary>
        /// Value for Hide state.
        /// </summary>
        HIDE = 0,

        /// <summary>
        /// Value for show normal state.
        /// </summary>
        SHOWNORMAL = 1,

        /// <summary>
        /// Value for Normal state.
        /// </summary>
        NORMAL = 1,

        /// <summary>
        /// Value for Show minimized
        /// </summary>
        SHOWMINIMIZED = 2,

        /// <summary>
        /// Value for Maximized state.
        /// </summary>
        SHOWMAXIMIZED = 3,

        /// <summary>
        /// Value for maxim.
        /// </summary>
        MAXIMIZE = 3,

        /// <summary>
        /// Value for Activate.
        /// </summary>
        SHOWNOACTIVATE = 4,

        /// <summary>
        /// Value for show.
        /// </summary>
        SHOW = 5,

        /// <summary>
        /// Value for minimize.
        /// </summary>
        MINIMIZE = 6,

        /// <summary>
        /// Value for SHOWMINNOACTIVE.
        /// </summary>
        SHOWMINNOACTIVE = 7,

        /// <summary>
        /// Value for SHOWNA
        /// </summary>
        SHOWNA = 8,

        /// <summary>
        /// Value for RESTORE
        /// </summary>
        RESTORE = 9,

        /// <summary>
        /// Value for SHOWDEFAULT
        /// </summary>
        SHOWDEFAULT = 10,

        /// <summary>
        /// Value for FORCEMINIMIZE
        /// </summary>
        FORCEMINIMIZE = 11,
    }

    /// <summary>
    /// Window message values, WM_*
    /// </summary>
    public enum WM
    {
        /// <summary>
        /// Value for NULL
        /// </summary>
        NULL = 0x0000,

        /// <summary>
        /// Value for CREATE
        /// </summary>
        CREATE = 0x0001,

        /// <summary>
        /// Value for DESTROY
        /// </summary>
        DESTROY = 0x0002,

        /// <summary>
        /// Value for MOVE
        /// </summary>
        MOVE = 0x0003,

        /// <summary>
        /// Value for SIZE
        /// </summary>
        SIZE = 0x0005,

        /// <summary>
        /// Value for ACTIVATE
        /// </summary>
        ACTIVATE = 0x0006,

        /// <summary>
        /// Value for SETFOCUS
        /// </summary>
        SETFOCUS = 0x0007,

        /// <summary>
        /// Value for KILLFOCUS
        /// </summary>
        KILLFOCUS = 0x0008,

        /// <summary>
        /// Value for ENABLE
        /// </summary>
        ENABLE = 0x000A,

        /// <summary>
        /// Value for SETREDRAW
        /// </summary>
        SETREDRAW = 0x000B,

        /// <summary>
        /// Value for SETTEXT
        /// </summary>
        SETTEXT = 0x000C,

        /// <summary>
        /// Value for GETTEXT
        /// </summary>
        GETTEXT = 0x000D,

        /// <summary>
        /// Value for GETTEXTLENGTH
        /// </summary>
        GETTEXTLENGTH = 0x000E,

        /// <summary>
        /// Value for PAINT
        /// </summary>
        PAINT = 0x000F,

        /// <summary>
        /// Value for CLOSE
        /// </summary>
        CLOSE = 0x0010,

        /// <summary>
        /// Value for QUERYENDSESSION
        /// </summary>
        QUERYENDSESSION = 0x0011,

        /// <summary>
        /// Value for QUIT
        /// </summary>
        QUIT = 0x0012,

        /// <summary>
        /// Value for QUERYOPEN
        /// </summary>
        QUERYOPEN = 0x0013,

        /// <summary>
        /// Value for ERASEBKGND
        /// </summary>
        ERASEBKGND = 0x0014,

        /// <summary>
        /// Value for SYSCOLORCHANGE
        /// </summary>
        SYSCOLORCHANGE = 0x0015,

        /// <summary>
        /// Value for WINDOWPOSCHANGING
        /// </summary>
        WINDOWPOSCHANGING = 0x0046,

        /// <summary>
        /// Value for WINDOWPOSCHANGED
        /// </summary>
        WINDOWPOSCHANGED = 0x0047,

        /// <summary>
        /// Value for SETICON
        /// </summary>
        SETICON = 0x0080,

        /// <summary>
        /// Value for NCCREATE
        /// </summary>
        NCCREATE = 0x0081,

        /// <summary>
        /// Value for NCDESTROY
        /// </summary>
        NCDESTROY = 0x0082,

        /// <summary>
        /// Value for NCCALCSIZE
        /// </summary>
        NCCALCSIZE = 0x0083,

        /// <summary>
        /// Value for NCHITTEST
        /// </summary>
        NCHITTEST = 0x0084,

        /// <summary>
        /// Value for NCPAINT
        /// </summary>
        NCPAINT = 0x0085,

        /// <summary>
        /// Value for NCACTIVATE
        /// </summary>
        NCACTIVATE = 0x0086,

        /// <summary>
        /// Value for GETDLGCODE
        /// </summary>
        GETDLGCODE = 0x0087,

        /// <summary>
        /// Value for SYNCPAINT
        /// </summary>
        SYNCPAINT = 0x0088,

        /// <summary>
        /// Value for NCMOUSEMOVE
        /// </summary>
        NCMOUSEMOVE = 0x00A0,

        /// <summary>
        /// Value for NCLBUTTONDOWN
        /// </summary>
        NCLBUTTONDOWN = 0x00A1,

        /// <summary>
        /// Value for NCLBUTTONUP
        /// </summary>
        NCLBUTTONUP = 0x00A2,

        /// <summary>
        /// Value for NCLBUTTONDBLCLK
        /// </summary>
        NCLBUTTONDBLCLK = 0x00A3,

        /// <summary>
        /// Value for NCRBUTTONDOWN
        /// </summary>
        NCRBUTTONDOWN = 0x00A4,

        /// <summary>
        /// Value for NCRBUTTONUP
        /// </summary>
        NCRBUTTONUP = 0x00A5,

        /// <summary>
        /// Value for NCRBUTTONDBLCLK
        /// </summary>
        NCRBUTTONDBLCLK = 0x00A6,

        /// <summary>
        /// Value for NCMBUTTONDOWN
        /// </summary>
        NCMBUTTONDOWN = 0x00A7,

        /// <summary>
        /// Value for NCMBUTTONUP
        /// </summary>
        NCMBUTTONUP = 0x00A8,

        /// <summary>
        /// Value for NCMBUTTONDBLCLK
        /// </summary>
        NCMBUTTONDBLCLK = 0x00A9,

        /// <summary>
        /// Value for SYSKEYDOWN
        /// </summary>
        SYSKEYDOWN = 0x0104,

        /// <summary>
        /// Value for SYSKEYUP
        /// </summary>
        SYSKEYUP = 0x0105,

        /// <summary>
        /// Value for SYSCHAR
        /// </summary>
        SYSCHAR = 0x0106,

        /// <summary>
        /// Value for SYSDEADCHAR
        /// </summary>
        SYSDEADCHAR = 0x0107,

        /// <summary>
        /// Value for SYSCOMMAND
        /// </summary>
        SYSCOMMAND = 0x0112,

        /// <summary>
        /// Value for DWMCOMPOSITIONCHANGED 
        /// </summary>
        DWMCOMPOSITIONCHANGED = 0x031E,

        /// <summary>
        /// Value for USER
        /// </summary>
        USER = 0x0400,

        /// <summary>
        /// Value for APP
        /// </summary>
        APP = 0x8000,
    }

    /// <summary>
    /// Window Placement
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public class WINDOWPLACEMENT
    {
        /// <summary>
        /// The Length value
        /// </summary>
        public int length = Marshal.SizeOf(typeof(WINDOWPLACEMENT));

        /// <summary>
        /// The Flag value
        /// </summary>
        public int flags;

        /// <summary>
        /// The Show window option
        /// </summary>
        public ShowWindowOptions showCmd;

        /// <summary>
        /// The Minimum position
        /// </summary>
        public POINT ptMinPosition;

        /// <summary>
        /// The Maximum Position
        /// </summary>
        public POINT ptMaxPosition;

        /// <summary>
        /// The normal position
        /// </summary>
        public RECT rcNormalPosition;
    }

    /// <summary>
    /// Stores flags for <see cref="NativeMethods"/> class functions.
    /// </summary>
    public class NativeConstants
    {
        /// <summary>
        /// Retains the current size. 
        /// </summary>
        public const int SWP_NOSIZE = 1;

        /// <summary>
        /// Retains the current position. 
        /// </summary>
        public const int SWP_NOMOVE = 2;

        /// <summary>
        /// Retains the current Z order. 
        /// </summary>
        public const int SWP_NOZORDER = 4;

        /// <summary>
        /// Does not redraw changes.
        /// </summary>
        public const int SWP_NOREDRAW = 8;

        /// <summary>
        /// Does not activate the window. 
        /// If this flag is not set, the window is activated and moved to the top of either the topmost 
        /// or non-topmost group (depending on the setting of the hWndInsertAfter
        /// parameter of ChromelessWindowInterop.SetWindowPos function.
        /// </summary>
        public const int SWP_NOACTIVATE = 16;

        /// <summary>
        /// Draws a frame (defined in the window's class description) around the window.
        /// </summary>
        public const int SWP_DRAWFRAME = 32;

        /// <summary>
        /// Applies new frame styles set using the SetWindowLong function. 
        /// Sends a WM_NCCALCSIZE message to the window, even if the window's size is not being changed. 
        /// If this flag is not specified, WM_NCCALCSIZE is sent only when the window's size is being changed.
        /// </summary>
        public const int SWP_FRAMECHANGED = 32;

        /// <summary>
        /// Displays the window.
        /// </summary>
        public const int SWP_SHOWWINDOW = 64;

        /// <summary>
        /// Hide the window from the screen.
        /// </summary>
        public const int SWP_HIDEWINDOW = 128;

        /// <summary>
        /// Discards the entire contents of the client area. 
        /// If this flag is not specified, the valid contents of the client area are saved and copied back into 
        /// the client area after the window is sized or repositioned.
        /// </summary>
        public const int SWP_NOCOPYBITS = 256;

        /// <summary>
        /// Does not change the owner window's position in the Z order.
        /// </summary>
        public const int SWP_NOOWNERZORDER = 512;

        /// <summary>
        /// Same as the <see cref="SWP_NOOWNERZORDER"/> flag.
        /// </summary>
        public const int SWP_NOREPOSITION = 512;

        /// <summary>
        /// Prevents the window from receiving the <see cref="WM_WINDOWPOSCHANGING"/> message.
        /// </summary>
        public const int SWP_NOSENDCHANGING = 1024;

        /// <summary>
        /// Prevents generation of the WM_SYNCPAINT message. 
        /// </summary>
        public const int SWP_DEFERERASE = 8192;

        /// <summary>
        /// If the calling thread and the thread that owns the window are attached to different input queues, 
        /// the system posts the request to the thread that owns the window. 
        /// This prevents the calling thread from blocking its execution while other threads process the request
        /// </summary>
        public const int SWP_ASYNCWINDOWPOS = 16384;

        /// <summary>
        /// Places the window above all non-topmost windows (that is, behind all topmost windows). 
        /// This flag has no effect if the window is already a non-topmost window.
        /// </summary>
        public const int HWND_NOTOPMOST = -2;

        /// <summary>
        /// Places the window at the top of the Z order.
        /// </summary>
        public const int HWND_TOP = 0;

        /// <summary>
        /// Places the window above all non-topmost windows. 
        /// The window maintains its topmost position even when it is deactivated.
        /// </summary>
        public const int HWND_TOPMOST = -1;

        /// <summary>
        /// The WM_NCHITTEST message is sent to a window when the cursor moves, or when a mouse button is pressed or released. 
        /// If the mouse is not captured, the message is sent to the window beneath the cursor. Otherwise, the message is sent 
        /// to the window that has captured the mouse.
        /// </summary>
        public const int WM_NCHITTEST = 0x0084;

        /// <summary>
        /// The WM_WINDOWPOSCHANGING message is sent to a window whose size, position, or place in the Z order is about to change 
        /// as a result of a call to the SetWindowPos function or another window-management function.
        /// </summary>
        public const int WM_WINDOWPOSCHANGING = 70;

        /// <summary>
        /// The WM_MOVE message is sent after a window has been moved. 
        /// </summary>
        public const int WM_MOVE = 3;

        /// <summary>
        /// In a window currently covered by another window in the same thread 
        /// (the message will be sent to underlying windows in the same thread until one of them returns a code that is not HTTRANSPARENT).
        /// </summary>
        public const int HTTRANSPARENT = -1;

        /// <summary>
        /// A window receives this message when the user chooses a command from the Window menu 
        /// (formerly known as the system or control menu) or when the user chooses the maximize button,
        /// minimize button, restore button, or close button.
        /// </summary>
        public const int WM_SYSCOMMAND = 274;

        /// <summary>
        /// Retrieves the window menu as a result of a keystroke. For more information, see the Remarks section.
        /// </summary>
        public const int SC_KEYMENU = 0xF100;

        /// <summary>
        /// Adds an icon to the status area. The hWnd and uID members of the NOTIFYICONDATA structure
        /// pointed to by lpdata will be used to identify the icon in later calls to Shell_NotifyIcon.
        /// </summary>
        public const int NIM_ADD = 0x0;

        /// <summary>
        /// Modifies an icon in the status area. Use the hWnd and uID members of the NOTIFYICONDATA
        /// structure pointed to by lpdata to identify the icon to be modified.
        /// </summary>
        public const int NIM_MODIFY = 0x1;

        /// <summary>
        /// Deletes an icon from the status area. Use the hWnd and uID members of the NOTIFYICONDATA
        /// structure pointed to by lpdata to identify the icon to be deleted.
        /// </summary>
        public const int NIM_DELETE = 0x2;
    }

    /// <summary>
    /// Stores methods for working with windows.
    /// </summary>
    public class NativeMethods
    {
        #region Constants
        /// <summary>
        /// WA_INACTIVE value.
        /// </summary>
        internal const int WA_INACTIVE = 0;

        /// <summary>
        /// SM_CXSCREEN value.
        /// </summary>
        public const int SM_CXSCREEN = 0;

        /// <summary>
        /// SM_CYSCREEN value.
        /// </summary>
        public const int SM_CYSCREEN = 1;

        #endregion

        #region Public methods

        /// <summary>
        /// Changes the size, position, and Z order of a window.
        /// </summary>
        /// <param name="hwnd">Handle of window.</param>
        /// <param name="hwndInsertAfter">
        /// Either the handle of the window to position this window behind, or a flag stating where in the Z-order to put the window.</param>
        /// <param name="x">The x coordinate of where to put the upper-left corner of the window.</param>
        /// <param name="y">The y coordinate of where to put the upper-left corner of the window.</param>
        /// <param name="cx">The x coordinate of where to put the lower-right corner of the window.</param>
        /// <param name="cy">The y coordinate of where to put the lower-right corner of the window.</param>
        /// <param name="wFlags">Specifies the window sizing and positioning flags.
        /// </param>
        /// <returns>
        /// Returns 1 if successful, or 0 if an error occurred.
        /// </returns>
        [SecurityCritical, DllImport("user32", EntryPoint = "SetWindowPos")]
        public static extern int SetWindowPos(IntPtr hwnd, int hwndInsertAfter, int x, int y, int cx, int cy, int wFlags);

        /// <summary>
        /// Converts given <see cref="IntPtr"/> value to <see cref="int"/> value.
        /// </summary>
        /// <param name="intPtr">Given <see cref="IntPtr"/> value.</param>
        /// <returns>Result of conversion.</returns>
        public static int IntPtrToInt32(IntPtr intPtr)
        {
            return (int)intPtr.ToInt64();
        }

        /// <summary>
        /// Assigns zero to all bits of given value except four lower bits.
        /// </summary>
        /// <param name="n">Given integer value.</param>
        /// <returns>Converted value.</returns>
        public static int LoWord(int n)
        {
            return n & 0xffff;
        }

        /// <summary>
        /// Assigns zero to all bits of given value except four lower bits.
        /// </summary>
        /// <param name="n">Given <see cref="IntPtr"/> value.</param>
        /// <returns>Converted value.</returns>
        public static int LoWord(IntPtr n)
        {
            return LoWord((int)n);
        }

        /// <summary>
        /// The function converts the screen coordinates of a specified point on the screen to client-area coordinates. 
        /// </summary>
        /// <param name="hWnd">Handle to the window whose client area will be used for the conversion. </param>
        /// <param name="pt"><see cref="POINT"/> structure that specifies the screen coordinates to be converted. </param>
        public static void ScreenToClient(IntPtr hWnd, [In, Out] POINT pt)
        {
            if (IntScreenToClient(hWnd, pt) == 0)
            {
                throw new Win32Exception();
            }
        }

        /// <summary>
        /// Confines the cursor to a rectangular area on the screen.
        /// </summary>
        /// <param name="rcClip">Points to the RECT structure that contains the screen coordinates of the 
        /// upper-left and lower-right corners of the confining rectangle. 
        /// If this parameter is NULL, the cursor is free to move anywhere on the screen.</param>
        /// <returns>Returns true if successful, or false if an error occurred.</returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern bool ClipCursor(ref RECT rcClip);

        /// <summary>
        /// The ClipCursor function confines the cursor to a rectangular area on the screen.
        /// </summary>
        /// <param name="passNull">Pointer to the RECT structure that contains the screen coordinates of the upper-left and lower-right corners of the confining rectangle. 
        /// If this parameter is NULL, the cursor is free to move anywhere on the screen.</param>
        /// <returns>Returns true if successful, or false if an error occurred.</returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern bool ClipCursor(object passNull);

        /// <summary>
        /// The MoveWindow function changes the position and dimensions of the specified window. 
        /// For a top-level window, the position and dimensions are relative to the upper-left corner of the screen. 
        /// For a child window, they are relative to the upper-left corner of the parent window's client area. 
        /// </summary>
        /// <param name="hWnd">Handle to the window.</param>
        /// <param name="X">Specifies the new position of the left side of the window. </param>
        /// <param name="Y">Specifies the new position of the top of the window. </param>
        /// <param name="nWidth">Specifies the new width of the window. </param>
        /// <param name="nHeight">Specifies the new height of the window. </param>
        /// <param name="bRepaint">Specifies whether the window is to be repainted. 
        /// If this parameter is TRUE, the window receives a message. 
        /// If the parameter is FALSE, no repainting of any kind occurs. 
        /// This applies to the client area, the nonclient area (including the title bar and scroll bars), 
        /// and any part of the parent window uncovered as a result of moving a child window.</param>
        /// <returns>If the function succeeds, the return value is nonzero.
        /// If the function fails, the return value is zero. To get extended error information, call GetLastError/>.</returns>
        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("user32.dll")]
        public static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        /// <summary>
        /// Sets the specified window�s show state.
        /// </summary>
        /// <param name="hWnd">The handle of the window to change the show status of.</param>
        /// <param name="nCmdShow">0 = Hide the window.
        /// 1 = Show the window and activate it.
        /// 2 = Show the window minimized.
        /// 3 = Maximize the window.
        /// 4 = Show the window in its most recent size and position but do not activate it.
        /// 5 = Show the window.
        /// 6 = Minimize the window.
        /// 7 = Show the window minimized but do not activate it.
        /// 8 = Show the window in its current state but do not activate it
        /// 9 = Restore the window (not maximized nor minimized).</param>
        /// <returns>
        /// If the window was previously visible, the return value is true.
        /// If the window was previously hidden, the return value is false.
        /// </returns>
        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern bool ShowWindow(HandleRef hWnd, int nCmdShow);

        /// <summary>
        /// The SetWindowLong function changes an attribute of the specified window. 
        /// The function also sets the 32-bit (long) value at the specified offset into the extra window memory.
        /// </summary>
        /// <param name="hWnd">Handle to the window and, indirectly, the class to which the window belongs.</param>
        /// <param name="nIndex">Specifies the zero-based offset to the value to be set. Valid values are in the range zero through the number of bytes of extra window memory, minus the size of an integer.</param>
        /// <param name="dwNewLong">Specifies the replacement value.</param>
        /// <returns>If the function succeeds, the return value is the previous value of the specified 32-bit integer.
        /// If the function fails, the return value is zero.
        /// </returns>
        [SecuritySafeCritical, SecurityCritical]
        public static IntPtr SetWindowLong(HandleRef hWnd, int nIndex, IntPtr dwNewLong)
        {
            int num1 = 0;
            IntPtr ptr1 = IntPtr.Zero;
            if (IntPtr.Size == 4)
            {
                int num2 = NativeMethods.IntSetWindowLong(hWnd, nIndex, NativeMethods.IntPtrToInt32(dwNewLong));
                num1 = Marshal.GetLastWin32Error();
                ptr1 = new IntPtr(num2);
            }
            else
            {
                ptr1 = NativeMethods.IntSetWindowLongPtr(hWnd, nIndex, dwNewLong);
                num1 = Marshal.GetLastWin32Error();
            }

            if (num1 != 0)
            {
                Debug.WriteLine(num1, "Win32 Error in SetWindowLong");
            }

            return ptr1;
        }

        /// <summary>
        /// Sets the window long PTR.
        /// </summary>
        /// <param name="hwnd">The HWND value.</param>
        /// <param name="nIndex">Index of the n.</param>
        /// <param name="dwNewLong">The dw new long.</param>
        /// <returns>Return the Integer Pointer</returns>
        public static IntPtr SetWindowLongPtr(IntPtr hwnd, GWL nIndex, IntPtr dwNewLong)
        {
            if (8 == IntPtr.Size)
            {
                return SetWindowLongPtr64(hwnd, nIndex, dwNewLong);
            }

            return SetWindowLongPtr32(hwnd, nIndex, dwNewLong);
        }

        /// <summary>
        /// Sets the window long PTR32.
        /// </summary>
        /// <param name="hWnd">The h WND.</param>
        /// <param name="nIndex">Index of the n.</param>
        /// <param name="dwNewLong">The dw new long.</param>
        /// <returns>Return the INTPTR</returns>
        [SuppressMessage("Microsoft.Portability", "CA1901:PInvokeDeclarationsShouldBePortable", MessageId = "2", Justification = "Used during unit testing"),
         SuppressMessage("Microsoft.Interoperability", "CA1400:PInvokeEntryPointsShouldExist", Justification = "Used during unit testing"),
         SuppressMessage("Microsoft.Portability", "CA1901:PInvokeDeclarationsShouldBePortable", MessageId = "return", Justification = "Used during unit testing"),
         DllImport("user32.dll", EntryPoint = "SetWindowLong", SetLastError = true)]
        private static extern IntPtr SetWindowLongPtr32(IntPtr hWnd, GWL nIndex, IntPtr dwNewLong);

        /// <summary>
        /// Sets the window long PTR64.
        /// </summary>
        /// <param name="hWnd">The h WND.</param>
        /// <param name="nIndex">Index of the n.</param>
        /// <param name="dwNewLong">The dw new long.</param>
        /// <returns>Return the Window position</returns>
        [SuppressMessage("Microsoft.Portability", "CA1901:PInvokeDeclarationsShouldBePortable", MessageId = "return", Justification = "Used during unit testing"),
            SuppressMessage("Microsoft.Interoperability", "CA1400:PInvokeEntryPointsShouldExist", Justification = "Used during unit testing"),
            DllImport("user32.dll", EntryPoint = "GetWindowLongPtr", SetLastError = true)]
        private static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, GWL nIndex, IntPtr dwNewLong);

        /// <summary>
        /// DWMs the def window proc.
        /// </summary>
        /// <param name="hwnd">The HWND Value.</param>
        /// <param name="msg">The MSG Value.</param>
        /// <param name="wParam">The w param.</param>
        /// <param name="lParam">The l param.</param>
        /// <param name="plResult">The pl result.</param>
        /// <returns>Return bool value</returns>
        [DllImport("dwmapi.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DwmDefWindowProc(IntPtr hwnd, WM msg, IntPtr wParam, IntPtr lParam, out IntPtr plResult);

        /// <summary>
        /// _s the enable menu item.
        /// </summary>
        /// <param name="hMenu">The h menu.</param>
        /// <param name="uIDEnableItem">The u ID enable item.</param>
        /// <param name="uEnable">The u enable.</param>
        /// <returns>Return int value</returns>
        [DllImport("user32.dll", EntryPoint = "EnableMenuItem")]
        private static extern int St_EnableMenuItem(IntPtr hMenu, SystemCommands uIDEnableItem, SystemMenuItemBehavior uEnable);

        /// <summary>
        /// Enables the menu item.
        /// </summary>
        /// <param name="hMenu">The h menu.</param>
        /// <param name="uIDEnableItem">The u ID enable item.</param>
        /// <param name="uEnable">The u enable.</param>
        /// <returns>Return the menu item</returns>
       [CLSCompliant (false)]
        public static SystemMenuItemBehavior EnableMenuItem(IntPtr hMenu, SystemCommands uIDEnableItem, SystemMenuItemBehavior uEnable)
        {
            int iRet = St_EnableMenuItem(hMenu, uIDEnableItem, uEnable);
            return (SystemMenuItemBehavior)iRet;
        }

        /// <summary>
        /// Gets the window placement.
        /// </summary>
        /// <param name="hwnd">The HWND Value.</param>
        /// <param name="lpwndpl">The LPWNDPL.</param>
        /// <returns>Return the bool value</returns>
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetWindowPlacement(IntPtr hwnd, WINDOWPLACEMENT lpwndpl);

        /// <summary>
        /// Gets the window placement.
        /// </summary>
        /// <param name="hwnd">The HWND Value.</param>
        /// <returns>Return the window placement</returns>
        public static WINDOWPLACEMENT GetWindowPlacement(IntPtr hwnd)
        {
            WINDOWPLACEMENT wndpl = new WINDOWPLACEMENT();
            if (GetWindowPlacement(hwnd, wndpl))
            {
                return wndpl;
            }

            throw new Win32Exception(Marshal.GetLastWin32Error());
        }

        /// <summary>
        /// This function is implemented as a macro to maintain compatibility with existing code.
        /// Use the menu handle returned by this macro to disable the Close button.
        /// There is no other use for the return value.
        /// </summary>
        /// <param name="hWnd">Handle to the window that will own a copy of the window menu.</param>
        /// <param name="bRevert">Boolean value of TRUE if GetSystemMenu is to reset the window menu
        /// to the default state and destroy the previous window menu, if any.
        /// If this parameter is FALSE, GetSystemMenu returns the handle to the copy of the window menu currently
        /// in use. The copy is initially identical to the window menu, but it can be modified.</param>
        /// <returns>IntPtr message value.</returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        public static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        /// <summary>
        /// The TrackPopupMenu function displays a shortcut menu at the specified location and tracks the selection of items on the menu.
        /// The shortcut menu can appear anywhere on the screen.
        /// </summary>
        /// <param name="hMenu">The h menu.</param>
        /// <param name="uFlags">The u flags.</param>
        /// <param name="x">The x value.</param>
        /// <param name="y">The y value.</param>
        /// <param name="nReserved">The n reserved.</param>
        /// <param name="hWnd">The handle WND.</param>
        /// <param name="prcRect">The PRC rect.</param>
        /// <returns>int message value.</returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)] [CLSCompliant (false)]
        public static extern int TrackPopupMenu(IntPtr hMenu, uint uFlags, int x, int y, int nReserved, IntPtr hWnd, IntPtr prcRect);

        /// <summary>
        /// Sends the message.
        /// </summary>
        /// <param name="hWnd">The handle WND.</param>
        /// <param name="msg">The MSG value.</param>
        /// <param name="wParam">The w param.</param>
        /// <param name="lParam">The l param.</param>
        /// <returns> IntPtr value.</returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        /// <summary>
        /// Sends the message.
        /// </summary>
        /// <param name="hWnd">The handle WND.</param>
        /// <param name="msg">The MSG value.</param>
        /// <param name="wParam">The window param.</param>
        /// <param name="lParam">The last param.</param>
        /// <returns>IntPtr value.</returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, int lParam);

        /// <summary>
        /// Sends the message.
        /// </summary>
        /// <param name="hWnd">The handle window.</param>
        /// <param name="msg">The Message.</param>
        /// <param name="wParam">The actual message param.</param>
        /// <param name="lParam">name of the task with which the pset is associated.</param>
        /// <returns>IntPtr value.</returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, IntPtr lParam);

        /// <summary>
        /// Shell_s the notify icon.
        /// </summary>
        /// <param name="dwMessage">The dw message.</param>
        /// <param name="pnid">The pnid value.</param>
        /// <returns>Boolean value.</returns>
        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        public static extern bool Shell_NotifyIcon(int dwMessage, ref NativeMethods.NotifyIconData pnid);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dwMessage"></param>
        /// <param name="pData"></param>
        /// <returns></returns>
        [DllImport("SHELL32", CallingConvention = CallingConvention.StdCall)]
        public static extern int SHAppBarMessage(int dwMessage, ref APPBARDATA pData);
        /// <summary>
        /// 
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct APPBARDATA
        {
            /// <summary>
            /// 
            /// </summary>
            public int cbSize;
            /// <summary>
            /// 
            /// </summary>
            public IntPtr hWnd;
            /// <summary>
            /// 
            /// </summary>
            public int uCallbackMessage;
            /// <summary>
            /// 
            /// </summary>
            public int uEdge;
            /// <summary>
            /// 
            /// </summary>
            public RECT rc;
            /// <summary>
            /// 
            /// </summary>
            public bool lParam;
        }


        /// <summary>
        /// 
        /// </summary>
        public enum ABEdge
        {
            /// <summary>
            /// 
            /// </summary>
            ABE_LEFT = 0,
            /// <summary>
            /// 
            /// </summary>
            ABE_TOP = 1,
            /// <summary>
            /// 
            /// </summary>
            ABE_RIGHT = 2,
            /// <summary>
            /// 
            /// </summary>
            ABE_BOTTOM = 3
        }


        /// <summary>
        /// Notify icon data.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct NotifyIconData
        {
            /// <summary>
            /// cb Size (DWORD)
            /// </summary>
            public System.Int32 cbSize;

            /// <summary>
            /// hWnd (HWND)
            /// </summary>
            public System.IntPtr hWnd;

            /// <summary>
            /// UINT - Unique identifier
            /// </summary>
            public System.Int32 uID;

            /// <summary>
            /// UINT - Unique identifier
            /// </summary>
            public System.Int32 uFlags;

            /// <summary>
            /// UINT - Unique identifier
            /// </summary>
            public System.Int32 uCallbackMessage;

            /// <summary>
            /// HICON - Icon
            /// </summary>
            public System.IntPtr hIcon;

            /// <summary>
            /// char[128] - szTip
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public System.String szTip;

            /// <summary>
            /// DWORD - dwState
            /// </summary>
            public System.Int32 dwState;

            /// <summary>
            /// DWORD - dwStateMask
            /// </summary>
            public System.Int32 dwStateMask;

            /// <summary>
            /// char[256] - szInfo
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public System.String szInfo;

            /// <summary>
            /// UINT - uTimeoutOrVersion
            /// </summary>
            public System.Int32 uTimeoutOrVersion;

            /// <summary>
            /// char[64] - szInfoTitle
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            public System.String szInfoTitle;

            /// <summary>
            /// DWORD - dwInfoFlags
            /// </summary>
            public System.Int32 dwInfoFlags;
        }

        /// <summary>
        /// Finds the window.
        /// </summary>
        /// <param name="lpClassName">Name of the lp class.</param>
        /// <param name="lpWindowName">Name of the lp window.</param>
        /// <returns>IntPtr message value.</returns>
        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        /// <summary>
        /// Gets the window rect.
        /// </summary>
        /// <param name="hWnd">The h WND.</param>
        /// <param name="lpRect">The lp rect.</param>
        /// <returns>Boolean value.</returns>
        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        /// <summary>
        /// Gets the desktop window.
        /// </summary>
        /// <returns>IntPtr value.</returns>
        [DllImport("user32.dll")]
        public static extern IntPtr GetDesktopWindow();

        /// <summary>
        /// Finds the window ex.
        /// </summary>
        /// <param name="parentHandle">The parent handle.</param>
        /// <param name="childAfter">The child after.</param>
        /// <param name="className">Name of the class.</param>
        /// <param name="windowTitle">The window title.</param>
        /// <returns>IntPtr message.</returns>
        [DllImport("user32.dll")]
        public static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string className, IntPtr windowTitle);

        /// <summary>
        /// Gets the DC.
        /// </summary>
        /// <param name="hWnd">The h WND.</param>
        /// <returns>IntPtr type value.</returns>
        [DllImport("user32.dll", EntryPoint = "GetDC")]
        public static extern IntPtr GetDC(IntPtr hWnd);

        /// <summary>
        /// Releases the DC.
        /// </summary>
        /// <param name="hwnd">The Handle WND.</param>
        /// <param name="hdc">The HDC message.</param>
        /// <returns>IntPtr type which returns from DC</returns>
        [DllImport("user32.dll")]
        public static extern Int32 ReleaseDC(IntPtr hwnd, IntPtr hdc);

        /// <summary>
        /// Gets the pixel.
        /// </summary>
        /// <param name="hdc">The HDC value.</param>
        /// <param name="x">The x value.</param>
        /// <param name="y">The y value.</param>
        /// <returns>IntPtr type which returns from Pixel</returns>
        [DllImport("gdi32.dll")]
        public static extern int GetPixel(IntPtr hdc, int x, int y);

        /// <summary>
        /// Gets the cursor pos.
        /// </summary>
        /// <param name="lpPoint">The lp point.</param>
        /// <returns>Bool value message.</returns>
        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(ref Point lpPoint);

        /// <summary>
        /// Enum for TernaryRasterOperations .
        /// </summary>
        public enum TernaryRasterOperations
        {
            /// <summary>
            /// dest = source
            /// </summary>
            SRCCOPY = 0x00CC0020, /* dest = source*/

            /// <summary>
            /// dest = source OR dest
            /// </summary>
            SRCPAINT = 0x00EE0086, /* dest = source OR dest*/

            /// <summary>
            /// dest = source AND dest
            /// </summary>
            SRCAND = 0x008800C6, /* dest = source AND dest*/

            /// <summary>
            /// dest = source XOR dest
            /// </summary>
            SRCINVERT = 0x00660046, /* dest = source XOR dest*/

            /// <summary>
            /// dest = source AND (NOT dest )
            /// </summary>
            SRCERASE = 0x00440328, /* dest = source AND (NOT dest )*/

            /// <summary>
            /// dest = (NOT source)
            /// </summary>
            NOTSRCCOPY = 0x00330008, /* dest = (NOT source)*/

            /// <summary>
            /// dest = (NOT src) AND (NOT dest)
            /// </summary>
            NOTSRCERASE = 0x001100A6, /* dest = (NOT src) AND (NOT dest) */

            /// <summary>
            /// dest = (source AND pattern)
            /// </summary>
            MERGECOPY = 0x00C000CA, /* dest = (source AND pattern)*/

            /// <summary>
            /// dest = (NOT source) OR dest
            /// </summary>
            MERGEPAINT = 0x00BB0226, /* dest = (NOT source) OR dest*/

            /// <summary>
            /// dest = (NOT source) OR dest
            /// </summary>
            PATCOPY = 0x00F00021, /* dest = pattern*/

            /// <summary>
            /// dest = pattern
            /// </summary>
            PATPAINT = 0x00FB0A09, /* dest = DPSnoo*/

            /// <summary>
            /// dest = DPSnoo
            /// </summary>
            PATINVERT = 0x005A0049, /* dest = pattern XOR dest*/

            /// <summary>
            /// dest = pattern XOR dest
            /// </summary>
            DSTINVERT = 0x00550009, /* dest = (NOT dest)*/

            /// <summary>
            /// dest = (NOT dest)
            /// </summary>
            BLACKNESS = 0x00000042, /* dest = BLACK*/

            /// <summary>
            /// dest = WHITE
            /// </summary>
            WHITENESS = 0x00FF0062, /* dest = WHITE*/
        }

        /// <summary>
        /// Stretches the BLT.
        /// </summary>
        /// <param name="hdcDest">The HDC dest.</param>
        /// <param name="nXOriginDest">The n X origin dest.</param>
        /// <param name="nYOriginDest">The n Y origin dest.</param>
        /// <param name="nWidthDest">The n width dest.</param>
        /// <param name="nHeightDest">The n height dest.</param>
        /// <param name="hdcSrc">The HDC SRC.</param>
        /// <param name="nXOriginSrc">The n X origin SRC.</param>
        /// <param name="nYOriginSrc">The n Y origin SRC.</param>
        /// <param name="nWidthSrc">The n width SRC.</param>
        /// <param name="nHeightSrc">The n height SRC.</param>
        /// <param name="dwRop">The dw raster operation.</param>
        /// <returns>Boolean message value.</returns>
        [DllImport("gdi32.dll")]
        public static extern bool StretchBlt(
            IntPtr hdcDest,
            int nXOriginDest,
            int nYOriginDest,
            int nWidthDest,
            int nHeightDest,
            IntPtr hdcSrc,
            int nXOriginSrc,
            int nYOriginSrc,
            int nWidthSrc,
            int nHeightSrc,
            TernaryRasterOperations dwRop);

        /// <summary>
        /// Gets the system metrics.
        /// </summary>
        /// <param name="abc">The abc (int value).</param>
        /// <returns>int value as system metrics</returns>
        [DllImport("user32.dll", EntryPoint = "GetSystemMetrics")]
        public static extern int GetSystemMetrics(int abc);

        /// <summary>
        /// Gets the window DC.
        /// </summary>
        /// <param name="ptr">The PTR value.</param>
        /// <returns>IntPtr value</returns>
        [DllImport("user32.dll", EntryPoint = "GetWindowDC")]
        public static extern IntPtr GetWindowDC(Int32 ptr);

        /// <summary>
        /// Deletes the DC.
        /// </summary>
        /// <param name="hDc">The handle dc.</param>
        /// <returns>IntPtr value.</returns>
        [DllImport("gdi32.dll", EntryPoint = "DeleteDC")]
        public static extern IntPtr DeleteDC(IntPtr hDc);

        /// <summary>
        /// Deletes the object.
        /// </summary>
        /// <param name="hDc">The h delete object.</param>
        /// <returns>IntPtr value.</returns>
        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        public static extern IntPtr DeleteObject(IntPtr hDc);

        /// <summary>
        /// Bits the BLT.
        /// </summary>
        /// <param name="hdcDest">The HDC dest.</param>
        /// <param name="xDest">The x dest.</param>
        /// <param name="yDest">The y dest.</param>
        /// <param name="wDest">The w dest.</param>
        /// <param name="hDest">The h dest.</param>
        /// <param name="hdcSource">The HDC source.</param>
        /// <param name="xSrc">The x SRC.</param>
        /// <param name="ySrc">The y SRC.</param>
        /// <param name="RasterOp">The raster op.</param>
        /// <returns>bool value.</returns>
        [DllImport("gdi32.dll", EntryPoint = "BitBlt")]
        public static extern bool BitBlt(IntPtr hdcDest, int xDest, int yDest, int wDest, int hDest, IntPtr hdcSource, int xSrc, int ySrc, int RasterOp);

        /// <summary>
        /// Creates the compatible bitmap.
        /// </summary>
        /// <param name="hdc">The HDC value.</param>
        /// <param name="nWidth">Width of the n.</param>
        /// <param name="nHeight">Height of the n.</param>
        /// <returns> IntPtr value.</returns>
        [DllImport("gdi32.dll", EntryPoint = "CreateCompatibleBitmap")]
        public static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int nWidth, int nHeight);

        /// <summary>
        /// Creates the compatible DC.
        /// </summary>
        /// <param name="hdc">The HDC value.</param>
        /// <returns>IntPtr value.</returns>
        [DllImport("gdi32.dll", EntryPoint = "CreateCompatibleDC")]
        public static extern IntPtr CreateCompatibleDC(IntPtr hdc);

        /// <summary>
        /// Selects the object.
        /// </summary>
        /// <param name="hdc">The HDC value.</param>
        /// <param name="bmp">The BMP brush value.</param>
        /// <returns>IntPtr value.</returns>
        [DllImport("gdi32.dll", EntryPoint = "SelectObject")]
        public static extern IntPtr SelectObject(IntPtr hdc, IntPtr bmp);
        [SuppressMessage("Microsoft.Interoperability", "CA1400:PInvokeEntryPointsShouldExist", Justification = "Used during unit testing"),
         SuppressMessage("Microsoft.Portability", "CA1901:PInvokeDeclarationsShouldBePortable", MessageId = "return", Justification = "Used during unit testing"),
         DllImport("user32.dll", EntryPoint = "GetWindowLong", SetLastError = true)]
        private static extern IntPtr GetWindowLongPtr32(IntPtr hWnd, GWL nIndex);

        [SuppressMessage("Microsoft.Portability", "CA1901:PInvokeDeclarationsShouldBePortable", MessageId = "return", Justification = "Used during unit testing"),
            SuppressMessage("Microsoft.Interoperability", "CA1400:PInvokeEntryPointsShouldExist", Justification = "Used during unit testing"), DllImport("user32.dll", EntryPoint = "GetWindowLongPtr", SetLastError = true)]
        private static extern IntPtr GetWindowLongPtr64(IntPtr hWnd, GWL nIndex);

        /// <summary>
        /// Gets the window long PTR.
        /// </summary>
        /// <param name="hwnd">The HWND Value.</param>
        /// <param name="nIndex">Index of the n.</param>
        /// <returns>Return the Window position</returns>
        public static IntPtr GetWindowLongPtr(IntPtr hwnd, GWL nIndex)
        {
            IntPtr ret = IntPtr.Zero;
            if (8 == IntPtr.Size)
            {
                ret = GetWindowLongPtr64(hwnd, nIndex);
            }
            else
            {
                ret = GetWindowLongPtr32(hwnd, nIndex);
            }

            if (IntPtr.Zero == ret)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            return ret;
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Ints the set window long PTR.
        /// </summary>
        /// <param name="hWnd">The h WND.</param>
        /// <param name="nIndex">Index of the n.</param>
        /// <param name="dwNewLong">The dw new long.</param>
        /// <returns>IntPtr value</returns>
        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr IntSetWindowLongPtr(HandleRef hWnd, int nIndex, IntPtr dwNewLong);

        /// <summary>
        /// Ints the set window long.
        /// </summary>
        /// <param name="hWnd">The h WND.</param>
        /// <param name="nIndex">Index of the n.</param>
        /// <param name="dwNewLong">The dw new long.</param>
        /// <returns>int value (Message)</returns>
        [DllImport("user32.dll", EntryPoint = "SetWindowLong", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int IntSetWindowLong(HandleRef hWnd, int nIndex, int dwNewLong);

        /// <summary>
        /// Ints the screen to client.
        /// </summary>
        /// <param name="hWnd">The handle WND.</param>
        /// <param name="pt">The pointer value.</param>
        /// <returns>Int value as client </returns>
        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("user32.dll", EntryPoint = "ScreenToClient", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        private static extern int IntScreenToClient(IntPtr hWnd, [In, Out] POINT pt);
        #endregion
    }
}
