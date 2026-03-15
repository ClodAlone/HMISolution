#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Text;

namespace Syncfusion.Windows.Forms.Edit.Utils
{
	#region WINDOWPOS
	[StructLayout( LayoutKind.Sequential, CharSet = CharSet.Auto )]
	internal struct WINDOWPOS
	{
		public IntPtr hwnd;
		public IntPtr hwndInsertAfter;
		public int x;
		public int y;
		public int cx;
		public int cy;
		public SetWindowPosFlags flags;
	}
	#endregion

	#region DeviceContextValues
	/// <summary>Values to pass to the GetDCEx method.</summary>
	[Flags()]
	internal enum DeviceContextValues : uint
	{
		/// <summary>DCX_WINDOW: Returns a DC that corresponds to the window rectangle rather 
		/// than the client rectangle.</summary>
		Window = 0x00000001,
		/// <summary>DCX_CACHE: Returns a DC from the cache, rather than the OWNDC or CLASSDC 
		/// window. Essentially overrides CS_OWNDC and CS_CLASSDC.</summary>
		Cache = 0x00000002,
		/// <summary>DCX_NORESETATTRS: Does not reset the attributes of this DC to the 
		/// default attributes when this DC is released.</summary>
		NoResetAttrs = 0x00000004,
		/// <summary>DCX_CLIPCHILDREN: Excludes the visible regions of all child windows 
		/// below the window identified by hWnd.</summary>
		ClipChildren = 0x00000008,
		/// <summary>DCX_CLIPSIBLINGS: Excludes the visible regions of all sibling windows 
		/// above the window identified by hWnd.</summary>
		ClipSiblings = 0x00000010,
		/// <summary>DCX_PARENTCLIP: Uses the visible region of the parent window. The 
		/// parent's WS_CLIPCHILDREN and CS_PARENTDC style bits are ignored. The origin is 
		/// set to the upper-left corner of the window identified by hWnd.</summary>
		ParentClip = 0x00000020,
		/// <summary>DCX_EXCLUDERGN: The clipping region identified by hrgnClip is excluded 
		/// from the visible region of the returned DC.</summary>
		ExcludeRgn = 0x00000040,
		/// <summary>DCX_INTERSECTRGN: The clipping region identified by hrgnClip is 
		/// intersected with the visible region of the returned DC.</summary>
		IntersectRgn = 0x00000080,
		/// <summary>DCX_EXCLUDEUPDATE: Unknown...Undocumented</summary>
		ExcludeUpdate = 0x00000100,
		/// <summary>DCX_INTERSECTUPDATE: Unknown...Undocumented</summary>
		IntersectUpdate = 0x00000200,
		/// <summary>DCX_LOCKWINDOWUPDATE: Allows drawing even if there is a LockWindowUpdate 
		/// call in effect that would otherwise exclude this window. Used for drawing during 
		/// tracking.</summary>
		LockWindowUpdate = 0x00000400,
		/// <summary>DCX_VALIDATE When specified with DCX_INTERSECTUPDATE, causes the DC to 
		/// be completely validated. Using this function with both DCX_INTERSECTUPDATE and 
		/// DCX_VALIDATE is identical to using the BeginPaint function.</summary>
		Validate = 0x00200000,
	}

	#endregion

	#region POINT
	[StructLayout( LayoutKind.Sequential )]
	internal struct POINT
	{
		internal int x;
		internal int y;


		public POINT( int X, int Y )
		{
			x = X;
			y = Y;
		}
		/// <summary>
		/// Point creation from lParam `s data
		/// </summary>
		/// <param name="lParam"></param>
		public POINT( int lParam )
		{
			x = ( lParam & 0xffff );
			y = ( lParam >> 16 );
		}

		public static implicit operator System.Drawing.Point( POINT p )
		{
			return new System.Drawing.Point( p.x, p.y );
		}

		public static implicit operator POINT( System.Drawing.Point p )
		{
			return new POINT( p.X, p.Y );
		}

	}
	#endregion

	#region RECT
	[StructLayout( LayoutKind.Sequential )]
	internal struct RECT
	{
		public int left;
		public int top;
		public int right;
		public int bottom;

		public static implicit operator Rectangle( RECT rect )
		{
			return Rectangle.FromLTRB( rect.left, rect.top, rect.right, rect.bottom );
		}

		public static implicit operator Size( RECT rect )
		{
			return new Size( rect.right - rect.left, rect.bottom - rect.top );
		}

		public static explicit operator RECT( Rectangle rect )
		{
			RECT rc = new RECT();

			rc.left = rect.Left;
			rc.right = rect.Right;
			rc.top = rect.Top;
			rc.bottom = rect.Bottom;

			return rc;
		}
	}

	#endregion

	#region COMRECT
	[StructLayout( LayoutKind.Sequential )]
	internal class COMRECT
	{
		// Fields
		internal int left;
		internal int top;
		internal int right;
		internal int bottom;

		// Constructors
		internal COMRECT()
		{
		}

		internal COMRECT( int left, int top, int right, int bottom )
		{
			this.left = left;
			this.top = top;
			this.right = right;
			this.bottom = bottom;
			return;
		}

		// Methods
		public override string ToString()
		{
			return String.Concat(
				"Left = ",
				this.left,
				" Top ",
				this.top,
				" Right = ",
				this.right,
				" Bottom = ",
				this.bottom );
		}

		internal static COMRECT FromXYWH( int x, int y, int width, int height )
		{
			return new COMRECT( x, y, ( x + width ), ( y + height ) );
		}

	}
	#endregion

	#region SIZE
	[StructLayout( LayoutKind.Sequential )]
	internal struct SIZE
	{
		internal int cx;
		internal int cy;
	}
	#endregion

	#region SCROLLINFO
	[StructLayout( LayoutKind.Sequential )]
	internal struct SCROLLINFO
	{
		internal int cbSize;
		internal int fMask;
		internal int nMin;
		internal int nMax;
		internal int nPage;
		internal int nPos;
		internal int nTrackPos;

		internal SCROLLINFO( int mask, int min, int max, int page, int pos )
		{
			this.cbSize = Marshal.SizeOf( typeof( SCROLLINFO ) );
			this.fMask = mask;
			this.nMin = min;
			this.nMax = max;
			this.nPage = page;
			this.nPos = pos;
			nTrackPos = 0;
		}
	}
	#endregion

	#region Window Extended Styles
	[Flags]
	internal enum WindowExStyles
	{
		WS_EX_DLGMODALFRAME = 0x00000001,
		WS_EX_NOPARENTNOTIFY = 0x00000004,
		WS_EX_TOPMOST = 0x00000008,
		WS_EX_ACCEPTFILES = 0x00000010,
		WS_EX_TRANSPARENT = 0x00000020,
		WS_EX_MDICHILD = 0x00000040,
		WS_EX_TOOLWINDOW = 0x00000080,
		WS_EX_WINDOWEDGE = 0x00000100,
		WS_EX_CLIENTEDGE = 0x00000200,
		WS_EX_CONTEXTHELP = 0x00000400,
		WS_EX_RIGHT = 0x00001000,
		WS_EX_LEFT = 0x00000000,
		WS_EX_RTLREADING = 0x00002000,
		WS_EX_LTRREADING = 0x00000000,
		WS_EX_LEFTSCROLLBAR = 0x00004000,
		WS_EX_RIGHTSCROLLBAR = 0x00000000,
		WS_EX_CONTROLPARENT = 0x00010000,
		WS_EX_STATICEDGE = 0x00020000,
		WS_EX_APPWINDOW = 0x00040000,
		WS_EX_OVERLAPPEDWINDOW = 0x00000300,
		WS_EX_PALETTEWINDOW = 0x00000188,
		WS_EX_LAYERED = 0x00080000
	}
	#endregion

	#region Dialog Codes
	internal enum DialogCodes
	{
		DLGC_WANTARROWS = 0x0001,
		DLGC_WANTTAB = 0x0002,
		DLGC_WANTALLKEYS = 0x0004,
		DLGC_WANTMESSAGE = 0x0004,
		DLGC_HASSETSEL = 0x0008,
		DLGC_DEFPUSHBUTTON = 0x0010,
		DLGC_UNDEFPUSHBUTTON = 0x0020,
		DLGC_RADIOBUTTON = 0x0040,
		DLGC_WANTCHARS = 0x0080,
		DLGC_STATIC = 0x0100,
		DLGC_BUTTON = 0x2000
	}
	#endregion

	#region Windows Messages
	internal enum Msg
	{
		WM_NULL = 0x0000,
		WM_CREATE = 0x0001,
		WM_DESTROY = 0x0002,
		WM_MOVE = 0x0003,
		WM_SIZE = 0x0005,
		WM_ACTIVATE = 0x0006,
		WM_SETFOCUS = 0x0007,
		WM_KILLFOCUS = 0x0008,
		WM_ENABLE = 0x000A,
		WM_SETREDRAW = 0x000B,
		WM_SETTEXT = 0x000C,
		WM_GETTEXT = 0x000D,
		WM_GETTEXTLENGTH = 0x000E,
		WM_PAINT = 0x000F,
		WM_CLOSE = 0x0010,
		WM_QUERYENDSESSION = 0x0011,
		WM_QUIT = 0x0012,
		WM_QUERYOPEN = 0x0013,
		WM_ERASEBKGND = 0x0014,
		WM_SYSCOLORCHANGE = 0x0015,
		WM_ENDSESSION = 0x0016,
		WM_SHOWWINDOW = 0x0018,
		WM_CTLCOLOR = 0x0019,
		WM_WININICHANGE = 0x001A,
		WM_SETTINGCHANGE = 0x001A,
		WM_DEVMODECHANGE = 0x001B,
		WM_ACTIVATEAPP = 0x001C,
		WM_FONTCHANGE = 0x001D,
		WM_TIMECHANGE = 0x001E,
		WM_CANCELMODE = 0x001F,
		WM_SETCURSOR = 0x0020,
		WM_MOUSEACTIVATE = 0x0021,
		WM_CHILDACTIVATE = 0x0022,
		WM_QUEUESYNC = 0x0023,
		WM_GETMINMAXINFO = 0x0024,
		WM_PAINTICON = 0x0026,
		WM_ICONERASEBKGND = 0x0027,
		WM_NEXTDLGCTL = 0x0028,
		WM_SPOOLERSTATUS = 0x002A,
		WM_DRAWITEM = 0x002B,
		WM_MEASUREITEM = 0x002C,
		WM_DELETEITEM = 0x002D,
		WM_VKEYTOITEM = 0x002E,
		WM_CHARTOITEM = 0x002F,
		WM_SETFONT = 0x0030,
		WM_GETFONT = 0x0031,
		WM_SETHOTKEY = 0x0032,
		WM_GETHOTKEY = 0x0033,
		WM_QUERYDRAGICON = 0x0037,
		WM_COMPAREITEM = 0x0039,
		WM_GETOBJECT = 0x003D,
		WM_COMPACTING = 0x0041,
		WM_COMMNOTIFY = 0x0044,
		WM_WINDOWPOSCHANGING = 0x0046,
		WM_WINDOWPOSCHANGED = 0x0047,
		WM_POWER = 0x0048,
		WM_COPYDATA = 0x004A,
		WM_CANCELJOURNAL = 0x004B,
		WM_NOTIFY = 0x004E,
		WM_INPUTLANGCHANGEREQUEST = 0x0050,
		WM_INPUTLANGCHANGE = 0x0051,
		WM_TCARD = 0x0052,
		WM_HELP = 0x0053,
		WM_USERCHANGED = 0x0054,
		WM_NOTIFYFORMAT = 0x0055,
		WM_CONTEXTMENU = 0x007B,
		WM_STYLECHANGING = 0x007C,
		WM_STYLECHANGED = 0x007D,
		WM_DISPLAYCHANGE = 0x007E,
		WM_GETICON = 0x007F,
		WM_SETICON = 0x0080,
		WM_NCCREATE = 0x0081,
		WM_NCDESTROY = 0x0082,
		WM_NCCALCSIZE = 0x0083,
		WM_NCHITTEST = 0x0084,
		WM_NCPAINT = 0x0085,
		WM_NCACTIVATE = 0x0086,
		WM_GETDLGCODE = 0x0087,
		WM_SYNCPAINT = 0x0088,
		WM_NCMOUSEMOVE = 0x00A0,
		WM_NCLBUTTONDOWN = 0x00A1,
		WM_NCLBUTTONUP = 0x00A2,
		WM_NCLBUTTONDBLCLK = 0x00A3,
		WM_NCRBUTTONDOWN = 0x00A4,
		WM_NCRBUTTONUP = 0x00A5,
		WM_NCRBUTTONDBLCLK = 0x00A6,
		WM_NCMBUTTONDOWN = 0x00A7,
		WM_NCMBUTTONUP = 0x00A8,
		WM_NCMBUTTONDBLCLK = 0x00A9,
        EM_SETSEL = 0x00B1,
        EM_LINEINDEX = 0x00BB,
        EM_REPLACESEL = 0x00C2,
		WM_KEYDOWN = 0x0100,
		WM_KEYUP = 0x0101,
		WM_CHAR = 0x0102,
		WM_DEADCHAR = 0x0103,
		WM_SYSKEYDOWN = 0x0104,
		WM_SYSKEYUP = 0x0105,
		WM_SYSCHAR = 0x0106,
		WM_SYSDEADCHAR = 0x0107,
		WM_KEYLAST = 0x0108,
		WM_IME_STARTCOMPOSITION = 0x010D,
		WM_IME_ENDCOMPOSITION = 0x010E,
		WM_IME_COMPOSITION = 0x010F,
		WM_IME_KEYLAST = 0x010F,
		WM_INITDIALOG = 0x0110,
		WM_COMMAND = 0x0111,
		WM_SYSCOMMAND = 0x0112,
		WM_TIMER = 0x0113,
		WM_HSCROLL = 0x0114,
		WM_VSCROLL = 0x0115,
		WM_INITMENU = 0x0116,
		WM_INITMENUPOPUP = 0x0117,
		WM_MENUSELECT = 0x011F,
		WM_MENUCHAR = 0x0120,
		WM_ENTERIDLE = 0x0121,
		WM_MENURBUTTONUP = 0x0122,
		WM_MENUDRAG = 0x0123,
		WM_MENUGETOBJECT = 0x0124,
		WM_UNINITMENUPOPUP = 0x0125,
		WM_MENUCOMMAND = 0x0126,
		WM_CTLCOLORMSGBOX = 0x0132,
		WM_CTLCOLOREDIT = 0x0133,
		WM_CTLCOLORLISTBOX = 0x0134,
		WM_CTLCOLORBTN = 0x0135,
		WM_CTLCOLORDLG = 0x0136,
		WM_CTLCOLORSCROLLBAR = 0x0137,
		WM_CTLCOLORSTATIC = 0x0138,
		WM_MOUSEMOVE = 0x0200,
		WM_LBUTTONDOWN = 0x0201,
		WM_LBUTTONUP = 0x0202,
		WM_LBUTTONDBLCLK = 0x0203,
		WM_RBUTTONDOWN = 0x0204,
		WM_RBUTTONUP = 0x0205,
		WM_RBUTTONDBLCLK = 0x0206,
		WM_MBUTTONDOWN = 0x0207,
		WM_MBUTTONUP = 0x0208,
		WM_MBUTTONDBLCLK = 0x0209,
		WM_MOUSEWHEEL = 0x020A,
		WM_PARENTNOTIFY = 0x0210,
		WM_ENTERMENULOOP = 0x0211,
		WM_EXITMENULOOP = 0x0212,
		WM_NEXTMENU = 0x0213,
		WM_SIZING = 0x0214,
		WM_CAPTURECHANGED = 0x0215,
		WM_MOVING = 0x0216,
		WM_DEVICECHANGE = 0x0219,
		WM_MDICREATE = 0x0220,
		WM_MDIDESTROY = 0x0221,
		WM_MDIACTIVATE = 0x0222,
		WM_MDIRESTORE = 0x0223,
		WM_MDINEXT = 0x0224,
		WM_MDIMAXIMIZE = 0x0225,
		WM_MDITILE = 0x0226,
		WM_MDICASCADE = 0x0227,
		WM_MDIICONARRANGE = 0x0228,
		WM_MDIGETACTIVE = 0x0229,
		WM_MDISETMENU = 0x0230,
		WM_ENTERSIZEMOVE = 0x0231,
		WM_EXITSIZEMOVE = 0x0232,
		WM_DROPFILES = 0x0233,
		WM_MDIREFRESHMENU = 0x0234,
		WM_IME_SETCONTEXT = 0x0281,
		WM_IME_NOTIFY = 0x0282,
		WM_IME_CONTROL = 0x0283,
		WM_IME_COMPOSITIONFULL = 0x0284,
		WM_IME_SELECT = 0x0285,
		WM_IME_CHAR = 0x0286,
		WM_IME_REQUEST = 0x0288,
		WM_IME_KEYDOWN = 0x0290,
		WM_IME_KEYUP = 0x0291,
		WM_MOUSEHOVER = 0x02A1,
		WM_MOUSELEAVE = 0x02A3,
		WM_CUT = 0x0300,
		WM_COPY = 0x0301,
		WM_PASTE = 0x0302,
		WM_CLEAR = 0x0303,
		WM_UNDO = 0x0304,
		WM_RENDERFORMAT = 0x0305,
		WM_RENDERALLFORMATS = 0x0306,
		WM_DESTROYCLIPBOARD = 0x0307,
		WM_DRAWCLIPBOARD = 0x0308,
		WM_PAINTCLIPBOARD = 0x0309,
		WM_VSCROLLCLIPBOARD = 0x030A,
		WM_SIZECLIPBOARD = 0x030B,
		WM_ASKCBFORMATNAME = 0x030C,
		WM_CHANGECBCHAIN = 0x030D,
		WM_HSCROLLCLIPBOARD = 0x030E,
		WM_QUERYNEWPALETTE = 0x030F,
		WM_PALETTEISCHANGING = 0x0310,
		WM_PALETTECHANGED = 0x0311,
		WM_HOTKEY = 0x0312,
		WM_PRINT = 0x0317,
		WM_PRINTCLIENT = 0x0318,
		WM_HANDHELDFIRST = 0x0358,
		WM_HANDHELDLAST = 0x035F,
		WM_AFXFIRST = 0x0360,
		WM_AFXLAST = 0x037F,
		WM_PENWINFIRST = 0x0380,
		WM_PENWINLAST = 0x038F,
		WM_APP = 0x8000,
		WM_USER = 0x0400,
		WM_REFLECT = WM_USER + 0x1c00,
		WM_TASKBAR_CREATED = 0xC086,
		NIN_BALLOONSHOW = 0x402,
		NIN_BALLOONHIDE = 0x403,
		NIN_BALLOONTIMEOUT = 0x404,
		NIN_BALLOONUSERCLICK = 0x405
	}
	#endregion

	#region Scroller Enums
	/// <summary>
	/// Scroller constants.
	/// </summary>
	internal enum ScrollerConst
	{
		/// <summary>
		/// SB_HORZ
		/// </summary>
		Horizontal = 0,
		/// <summary>
		/// SB_VERT
		/// </summary>
		Vertical,
		/// <summary>
		/// SB_CTL
		/// </summary>
		Control,
		/// <summary>
		/// SB_BOTH
		/// </summary>
		Both
	}
	internal enum ScrollCommands
	{
		/// <summary>
		/// SB_LINEUP
		/// </summary>
		LineUp = 0,
		/// <summary>
		/// SB_LINEDOWN
		/// </summary>
		LineDonw,
		/// <summary>
		/// SB_PAGEUP
		/// </summary>
		PageUp,
		/// <summary>
		/// SB_PAGEDOWN
		/// </summary>
		PageDown,
		/// <summary>
		/// SB_THUMBPOSITION
		/// </summary>
		ThumbPosition,
		/// <summary>
		/// SB_THUMBTRACK
		/// </summary>
		ThumbTrack,
		/// <summary>
		/// SB_TOP
		/// </summary>
		Top,
		/// <summary>
		/// SB_BOTTOM
		/// </summary>
		Bottom,
		/// <summary>
		/// SB_ENDSCROLL
		/// </summary>
		EndScroll,

		/// <summary>
		/// SB_LINELEFT
		/// </summary>
		LineLeft = LineUp,
		/// <summary>
		/// SB_LINERIGHT
		/// </summary>
		LineRight = LineDonw,
		/// <summary>
		/// SB_PAGELEFT
		/// </summary>
		PageLeft = PageUp,
		/// <summary>
		/// SB_PAGERIGHT
		/// </summary>
		PageRight = PageDown,
		/// <summary>
		/// SB_LEFT
		/// </summary>
		Left = Top,
		/// <summary>
		/// SB_RIGHT
		/// </summary>
		Right = Bottom,
	}
	#endregion

	#region ToolTipsDelays
	/// <summary>
	/// Type of the dalay of the tooltip.
	/// </summary>
	internal enum ToolTipsDelays
	{
		Automatic = 0,
		Reshow = 1,
		AutoPop = 2,
		Initial = 3
	}
	#endregion

	#region Window Styles
	[Flags]
	internal enum WindowStyles : uint
	{
		WS_OVERLAPPED = 0x00000000,
		WS_POPUP = 0x80000000,
		WS_CHILD = 0x40000000,
		WS_MINIMIZE = 0x20000000,
		WS_VISIBLE = 0x10000000,
		WS_DISABLED = 0x08000000,
		WS_CLIPSIBLINGS = 0x04000000,
		WS_CLIPCHILDREN = 0x02000000,
		WS_MAXIMIZE = 0x01000000,
		WS_CAPTION = 0x00C00000,
		WS_BORDER = 0x00800000,
		WS_DLGFRAME = 0x00400000,
		WS_VSCROLL = 0x00200000,
		WS_HSCROLL = 0x00100000,
		WS_SYSMENU = 0x00080000,
		WS_THICKFRAME = 0x00040000,
		WS_GROUP = 0x00020000,
		WS_TABSTOP = 0x00010000,
		WS_MINIMIZEBOX = 0x00020000,
		WS_MAXIMIZEBOX = 0x00010000,
		WS_TILED = 0x00000000,
		WS_ICONIC = 0x20000000,
		WS_SIZEBOX = 0x00040000,
		WS_POPUPWINDOW = 0x80880000,
		WS_OVERLAPPEDWINDOW = 0x00CF0000,
		WS_TILEDWINDOW = 0x00CF0000,
		WS_CHILDWINDOW = 0x40000000
	}
	#endregion

	#region SetWindowPos Z Order
	internal enum SetWindowPosZOrder
	{
		HWND_TOP = 0,
		HWND_BOTTOM = 1,
		HWND_TOPMOST = -1,
		HWND_NOTOPMOST = -2
	}
	#endregion

	#region SetWindowPosFlags
	internal enum SetWindowPosFlags : uint
	{
		SWP_NOSIZE = 0x0001,
		SWP_NOMOVE = 0x0002,
		SWP_NOZORDER = 0x0004,
		SWP_NOREDRAW = 0x0008,
		SWP_NOACTIVATE = 0x0010,
		SWP_FRAMECHANGED = 0x0020,
		SWP_SHOWWINDOW = 0x0040,
		SWP_HIDEWINDOW = 0x0080,
		SWP_NOCOPYBITS = 0x0100,
		SWP_NOOWNERZORDER = 0x0200,
		SWP_NOSENDCHANGING = 0x0400,
		SWP_DRAWFRAME = 0x0020,
		SWP_NOREPOSITION = 0x0200,
		SWP_DEFERERASE = 0x2000,
		SWP_ASYNCWINDOWPOS = 0x4000
	}
	#endregion

	#region ToolTipStyles
	/// <summary>
	/// Styles of the tooltip.
	/// </summary>
	internal enum ToolTipStyles
	{
		AlwaysVisible = 0x01,
		NoPrefix = 0x02,
		NoAnimate = 0x10,
		NoFade = 0x20,
		Baloon = 0x40
	}
	#endregion

	#region ToolTip Flags
	internal enum ToolTipFlags
	{
		TTF_IDISHWND = 0x0001,
		TTF_CENTERTIP = 0x0002,
		TTF_RTLREADING = 0x0004,
		TTF_SUBCLASS = 0x0010,
		TTF_TRACK = 0x0020,
		TTF_ABSOLUTE = 0x0080,
		TTF_TRANSPARENT = 0x0100,
		TTF_DI_SETITEM = 0x8000
	}
	#endregion

	#region ToolTipMsg
	internal enum ToolTipMsg
	{
		TTM_ACTIVATE = ( Msg.WM_USER + 1 ),
		TTM_SETDELAYTIME = ( Msg.WM_USER + 3 ),
		TTM_ADDTOOLA = ( Msg.WM_USER + 4 ),
		TTM_ADDTOOLW = ( Msg.WM_USER + 50 ),
		TTM_DELTOOLA = ( Msg.WM_USER + 5 ),
		TTM_DELTOOLW = ( Msg.WM_USER + 51 ),
		TTM_NEWTOOLRECTA = ( Msg.WM_USER + 6 ),
		TTM_NEWTOOLRECTW = ( Msg.WM_USER + 52 ),
		TTM_RELAYEVENT = ( Msg.WM_USER + 7 ),
		TTM_GETTOOLINFOA = ( Msg.WM_USER + 8 ),
		TTM_GETTOOLINFOW = ( Msg.WM_USER + 53 ),
		TTM_SETTOOLINFOA = ( Msg.WM_USER + 9 ),
		TTM_SETTOOLINFOW = ( Msg.WM_USER + 54 ),
		TTM_HITTESTA = ( Msg.WM_USER + 10 ),
		TTM_HITTESTW = ( Msg.WM_USER + 55 ),
		TTM_GETTEXTA = ( Msg.WM_USER + 11 ),
		TTM_GETTEXTW = ( Msg.WM_USER + 56 ),
		TTM_UPDATETIPTEXTA = ( Msg.WM_USER + 12 ),
		TTM_UPDATETIPTEXTW = ( Msg.WM_USER + 57 ),
		TTM_GETTOOLCOUNT = ( Msg.WM_USER + 13 ),
		TTM_ENUMTOOLSA = ( Msg.WM_USER + 14 ),
		TTM_ENUMTOOLSW = ( Msg.WM_USER + 58 ),
		TTM_GETCURRENTTOOLA = ( Msg.WM_USER + 15 ),
		TTM_GETCURRENTTOOLW = ( Msg.WM_USER + 59 ),
		TTM_WINDOWFROMPOINT = ( Msg.WM_USER + 16 ),
		TTM_TRACKACTIVATE = ( Msg.WM_USER + 17 ),  // wParam = TRUE/FALSE start end  lparam = LPTOOLINFO
		TTM_TRACKPOSITION = ( Msg.WM_USER + 18 ),  // lParam = dwPos
		TTM_SETTIPBKCOLOR = ( Msg.WM_USER + 19 ),
		TTM_SETTIPTEXTCOLOR = ( Msg.WM_USER + 20 ),
		TTM_GETDELAYTIME = ( Msg.WM_USER + 21 ),
		TTM_GETTIPBKCOLOR = ( Msg.WM_USER + 22 ),
		TTM_GETTIPTEXTCOLOR = ( Msg.WM_USER + 23 ),
		TTM_SETMAXTIPWIDTH = ( Msg.WM_USER + 24 ),
		TTM_GETMAXTIPWIDTH = ( Msg.WM_USER + 25 ),
		TTM_SETMARGIN = ( Msg.WM_USER + 26 ),  // lParam = lprc
		TTM_GETMARGIN = ( Msg.WM_USER + 27 ),  // lParam = lprc
		TTM_POP = ( Msg.WM_USER + 28 ),
		TTM_UPDATE = ( Msg.WM_USER + 29 ),
		TTM_GETBUBBLESIZE = ( Msg.WM_USER + 30 ),
		TTM_ADJUSTRECT = ( Msg.WM_USER + 31 ),
		TTM_SETTITLEA = ( Msg.WM_USER + 32 ),  // wParam = TTI_*, lParam = char* szTitle
		TTM_SETTITLEW = ( Msg.WM_USER + 33 )  // wParam = TTI_*, lParam = wchar* szTitle
	}
	#endregion

	#region ToolTipNotifyMsg
	internal enum ToolTipNotifyMsg : int
	{
		TTN_FIRST = ( 0 - 520 ),       // tooltips
		TTN_GETDISPINFOA = ( TTN_FIRST - 0 ),
		TTN_GETDISPINFOW = ( TTN_FIRST - 10 ),
		TTN_SHOW = ( TTN_FIRST - 1 ),
		TTN_POP = ( TTN_FIRST - 2 )
	}
	#endregion

	#region Draw Text format flags
	/// <summary>
	/// Flags, used to specify formatting of the string.
	/// </summary>
	[Flags]
	internal enum DrawTextFormatFlags
	{
		DT_TOP = 0x00000000,
		DT_LEFT = 0x00000000,
		DT_CENTER = 0x00000001,
		DT_RIGHT = 0x00000002,
		DT_VCENTER = 0x00000004,
		DT_BOTTOM = 0x00000008,
		DT_WORDBREAK = 0x00000010,
		DT_SINGLELINE = 0x00000020,
		DT_EXPANDTABS = 0x00000040,
		DT_TABSTOP = 0x00000080,
		DT_NOCLIP = 0x00000100,
		DT_EXTERNALLEADING = 0x00000200,
		DT_CALCRECT = 0x00000400,
		DT_NOPREFIX = 0x00000800,
		DT_INTERNAL = 0x00001000,
		DT_EDITCONTROL = 0x00002000,
		DT_PATH_ELLIPSIS = 0x00004000,
		DT_END_ELLIPSIS = 0x00008000,
		DT_MODIFYSTRING = 0x00010000,
		DT_RTLREADING = 0x00020000,
		DT_WORD_ELLIPSIS = 0x00040000
	}

	#endregion

	#region Common Controls Initialization flags
	internal enum CommonControlInitFlags
	{
		ICC_LISTVIEW_CLASSES = 0x00000001,
		ICC_TREEVIEW_CLASSES = 0x00000002,
		ICC_BAR_CLASSES = 0x00000004,
		ICC_TAB_CLASSES = 0x00000008,
		ICC_UPDOWN_CLASS = 0x00000010,
		ICC_PROGRESS_CLASS = 0x00000020,
		ICC_HOTKEY_CLASS = 0x00000040,
		ICC_ANIMATE_CLASS = 0x00000080,
		ICC_WIN95_CLASSES = 0x000000FF,
		ICC_DATE_CLASSES = 0x00000100,
		ICC_USEREX_CLASSES = 0x00000200,
		ICC_COOL_CLASSES = 0x00000400,
		ICC_INTERNET_CLASSES = 0x00000800,
		ICC_PAGESCROLLER_CLASS = 0x00001000,
		ICC_NATIVEFNTCTL_CLASS = 0x00002000
	}
	#endregion

	#region TOOLINFO
	[StructLayout( LayoutKind.Sequential )]
	internal struct TOOLINFO
	{
		internal UInt32 cbSize;
		internal UInt32 uFlags;
		internal IntPtr hwnd;
		internal IntPtr uId;       // UINT_PTR
		internal RECT rect;
		internal IntPtr hinst;
		[MarshalAs( UnmanagedType.LPTStr )]
		internal string lpszText;
		internal IntPtr lParam;
	}
	#endregion

	#region NMHDR
	[StructLayout( LayoutKind.Sequential )]
	internal struct NMHDR
	{
		internal IntPtr hwndFrom;
		internal int idFrom;
		internal int code;
	}
	#endregion

	#region CWPSTRUCT
	[StructLayout( LayoutKind.Sequential )]
	internal struct CWPSTRUCT
	{
		public IntPtr lparam;
		public IntPtr wparam;
		public int message;
		public IntPtr hwnd;
	}
	#endregion

	[StructLayout( LayoutKind.Sequential )]
	internal struct KBDLLHOOKSTRUCT
	{
		internal uint vkCode;
		internal uint scanCode;
		internal uint flags;
		internal uint time;
		internal IntPtr dwExtraInfo;
	};

	[StructLayout( LayoutKind.Sequential )]
	internal struct tagCOMPOSITIONFORM
	{
		public int dwStyle;
		public POINT ptCurrentPos;
		public RECT rcArea;
	}

	#region Windows Hook Codes
	internal enum WindowsHookCodes
	{
		WH_MSGFILTER = ( -1 ),
		WH_JOURNALRECORD = 0,
		WH_JOURNALPLAYBACK = 1,
		WH_KEYBOARD = 2,
		WH_GETMESSAGE = 3,
		WH_CALLWNDPROC = 4,
		WH_CBT = 5,
		WH_SYSMSGFILTER = 6,
		WH_MOUSE = 7,
		WH_HARDWARE = 8,
		WH_DEBUG = 9,
		WH_SHELL = 10,
		WH_FOREGROUNDIDLE = 11,
		WH_CALLWNDPROCRET = 12,
		WH_KEYBOARD_LL = 13,
		WH_MOUSE_LL = 14
	}
	#endregion

	/// <summary>
	/// Summary description for Native.
	/// </summary>
	internal sealed class CaretAPI
	{
		#region Class Initialize/Finalize methods
		/// <summary>
		/// Hide default constructor
		/// </summary>
		private CaretAPI()
		{
			throw new NotImplementedException();
		}

		#endregion

		#region Class utility methods
		/// <summary>
		/// The CreateCaret function creates a new shape for the system caret and assigns
		/// ownership of the caret to the specified window. The caret shape can be a line,
		/// a block, or a bitmap.
		/// </summary>
		/// <param name="handle">[in] Handle to the window that owns the caret.</param>
		/// <param name="hBitmap">[in] Handle to the bitmap that defines the caret shape.
		/// If this parameter is NULL, the caret is solid. If this parameter is
		/// (HBITMAP) 1, the caret is gray. If this parameter is a bitmap handle,
		/// the caret is the specified bitmap. The bitmap handle must have been created
		/// by the CreateBitmap, CreateDIBitmap, or LoadBitmap function. If hBitmap is
		/// a bitmap handle, CreateCaret ignores the nWidth and nHeight parameters;
		/// the bitmap defines its own width and height.</param>
		/// <param name="nWidth">in] Specifies the width of the caret in logical units.
		/// If this parameter is zero, the width is set to the system-defined window
		/// border width. If hBitmap is a bitmap handle, CreateCaret ignores this parameter
		/// </param>
		/// <param name="nHeight">[in] Specifies the height, in logical units, of the
		/// caret. If this parameter is zero, the height is set to the system-defined
		/// window border height. If hBitmap is a bitmap handle, CreateCaret ignores
		/// this parameter</param>
		/// <returns>If the function succeeds, the return value is nonzero.
		/// If the function fails, the return value is zero. To get extended
		/// error information, call GetLastError.</returns>
		[DllImport( "user32" )]
		internal static extern int CreateCaret( IntPtr handle, IntPtr hBitmap, int nWidth, int nHeight );

		/// <summary>
		/// The DestroyCaret function destroys the caret's current shape,
		/// frees the caret from the window, and removes the caret from the screen.
		/// </summary>
		/// <returns>If the function succeeds, the return value is nonzero.
		/// If the function fails, the return value is zero. To get extended
		/// error information, call GetLastError. </returns>
		[DllImport( "user32" )]
		internal static extern int DestroyCaret();

		/// <summary>
		/// Returns the time required to invert the caret's pixels. The user can set this value.
		/// </summary>
		/// <returns>If the function succeeds, the return value is the blink time, in milliseconds.
		/// If the function fails, the return value is zero. To get extended
		/// error information, call GetLastError. </returns>
		[DllImport( "user32" )]
		internal static extern int GetCaretBlinkTime();

		/// <summary>
		/// The GetCaretPos function copies the caret's position to the specified POINT structure
		/// </summary>
		/// <param name="point">[out] Pointer to the POINT structure that is
		/// to receive the client coordinates of the caret.
		/// </param>
		/// <returns>If the function succeeds, the return value is nonzero.
		/// If the function fails, the return value is zero. To get extended
		/// error information, call GetLastError. </returns>
		[DllImport( "user32" )]
		internal static extern int GetCaretPos( ref POINT point );

		/// <summary>
		/// The HideCaret function removes the caret from the screen.
		/// Hiding a caret does not destroy its current shape or
		/// invalidate the insertion point.
		/// </summary>
		/// <param name="handle">[in] Handle to the window that owns
		/// the caret. If this parameter is NULL, HideCaret searches
		/// the current task for the window that owns the caret. </param>
		/// <returns>If the function succeeds, the return value is nonzero.
		/// If the function fails, the return value is zero. To get extended
		/// error information, call GetLastError. </returns>
		[DllImport( "user32" )]
		internal static extern int HideCaret( IntPtr handle );

		/// <summary>
		/// Sets the caret blink time to the specified number of milliseconds.
		/// The blink time is the elapsed time, in milliseconds, required to
		/// invert the caret's pixels.
		/// </summary>
		/// <param name="wMSeconds">[in] Specifies the new blink time, in milliseconds.</param>
		/// <returns>If the function succeeds, the return value is nonzero.
		/// If the function fails, the return value is zero. To get extended
		/// error information, call GetLastError. </returns>
		[DllImport( "user32" )]
		internal static extern int SetCaretBlinkTime( int wMSeconds );

		/// <summary>
		/// moves the caret to the specified coordinates. If the window that owns
		/// the caret was created with the CS_OWNDC class style, then the specified
		/// coordinates are subject to the mapping mode of the device context
		/// associated with that window
		/// </summary>
		/// <param name="x">[in] Specifies the new x-coordinate of the caret.</param>
		/// <param name="y">[in] Specifies the new y-coordinate of the caret.</param>
		/// <returns>If the function succeeds, the return value is nonzero.
		/// If the function fails, the return value is zero. To get extended
		/// error information, call GetLastError. </returns>
		[DllImport( "user32" )]
		internal static extern int SetCaretPos( int x, int y );

		/// <summary>
		/// Makes the caret visible on the screen at the caret's current position.
		/// When the caret becomes visible, it begins flashing automatically
		/// </summary>
		/// <param name="handle">[in] Handle to the window that owns the caret.
		/// If this parameter is NULL, ShowCaret searches the current task for
		/// the window that owns the caret.</param>
		/// <returns>If the function succeeds, the return value is nonzero.
		/// If the function fails, the return value is zero. To get extended
		/// error information, call GetLastError. </returns>
		[DllImport( "user32" )]
		internal static extern int ShowCaret( IntPtr handle );

		/// <summary>
		/// Retrieves the calling thread's last-error code value.
		/// The last-error code is maintained on a per-thread basis.
		/// Multiple threads do not overwrite each other's last-error code.
		/// </summary>
		/// <returns>The return value is the calling thread's last-error code value.
		/// Functions set this value by calling the SetLastError function.
		/// The Return Value section of each reference page notes the
		/// conditions under which the function sets the last-error code.
		/// </returns>
		[DllImport( "kernel32" )]
		internal static extern int GetLastError();
		#endregion
	}

	internal class GdiCaret : IDisposable
	{
		#region Class members
		/// <summary>
		/// Parent control
		/// </summary>
		private Control m_parent;
		/// <summary>
		///
		/// </summary>
		private bool m_visible;
		/// <summary>
		/// Blink time
		/// </summary>
		private int m_blinkTime = -1;
		/// <summary>
		/// Position of the caret in parent window
		/// </summary>
		private Point m_position;
		/// <summary>
		/// is class disposed before or not
		/// </summary>
		private bool m_bDisposed;
		#endregion

		#region Class Properties
		/// <summary>
		/// Parent of the caret
		/// </summary>
		public Control Parent
		{
			get
			{
				return m_parent;
			}
		}

		/// <summary>
		/// GET, SET the elapsed time, in milliseconds, required to invert the caret.
		/// </summary>
		public int BlinkTime
		{
			get
			{
				return m_blinkTime;
			}
			set
			{
				if( value != m_blinkTime )
				{
					int error = CaretAPI.SetCaretBlinkTime( value );
					if( error == 0 ) Marshal.ThrowExceptionForHR( CaretAPI.GetLastError() );

					m_blinkTime = value;
				}
			}
		}

		/// <summary>
		/// Position of caret in client coordinates
		/// </summary>
		public Point Position
		{
			get
			{
				return m_position;
			}
			set
			{
				if( value != m_position )
				{
					int error = CaretAPI.SetCaretPos( value.X, value.Y );
					if( error == 0 ) Marshal.ThrowExceptionForHR( CaretAPI.GetLastError() );

					m_position = value;
				}
			}
		}

		/// <summary>
		/// GET, SET visibility of the control`s carret
		/// </summary>
		public bool Visible
		{
			get
			{
				return m_visible;
			}
			set
			{
				if( m_visible != value )
				{
					m_visible = value;
					int error = 0;

					if( value )
					{
						error = CaretAPI.ShowCaret( m_parent.Handle );
					}
					else
					{
						error = CaretAPI.HideCaret( m_parent.Handle );
					}

					if( error == 0 ) Marshal.ThrowExceptionForHR( CaretAPI.GetLastError() );
				}
			}
		}

		#endregion

		#region Class Initialize/Finalize methods
		private GdiCaret()
		{
		}

		public GdiCaret( Control control )
			: this( control, Size.Empty )
		{
		}

		public GdiCaret( Control control, Size size )
		{
			if( control == null )
				throw new ArgumentNullException( "control" );

			m_parent = control;

			int error = CaretAPI.CreateCaret( control.Handle, IntPtr.Zero, size.Width, size.Height );
			if( error == 0 ) Marshal.ThrowExceptionForHR( CaretAPI.GetLastError() );

			Initialize();
		}

		public GdiCaret( Control control, Bitmap bmp )
		{
			if( control == null )
				throw new ArgumentNullException( "control" );

			if( bmp == null )
				throw new ArgumentNullException( "bmp" );

			m_parent = control;

			int error = CaretAPI.CreateCaret( control.Handle, bmp.GetHbitmap(), 0, 0 );
			if( error == 0 ) Marshal.ThrowExceptionForHR( CaretAPI.GetLastError() );

			Initialize();
		}

		protected void Initialize()
		{
			POINT point = new POINT( 0, 0 );
			m_blinkTime = CaretAPI.GetCaretBlinkTime();
			int error = CaretAPI.GetCaretPos( ref point );
			if( error == 0 ) Marshal.ThrowExceptionForHR( CaretAPI.GetLastError() );
		}

		~GdiCaret()
		{
			Dispose();
		}

		public void Dispose()
		{
			if( !m_bDisposed )
			{
				int error = CaretAPI.DestroyCaret();
				if( error == 0 ) Marshal.ThrowExceptionForHR( CaretAPI.GetLastError() );

				m_bDisposed = true;
				GC.SuppressFinalize( this );
			}
		}
		#endregion
	}

	internal class GDIAppi
	{
		/// <summary>
		/// The DrawText function draws formatted text in the specified rectangle.
		/// It formats the text according to the specified method
		/// (expanding tabs, justifying characters, breaking lines, and so forth).
		/// </summary>
		/// <param name="hdc">Handle to the device context.</param>
		/// <param name="lpString">Pointer to the string that specifies the text to be drawn. If the nCount parameter is 1, the string must be null-terminated.</param>
		/// <param name="nCount"> Specifies the length of the string.</param>
		/// <param name="lpRect">Pointer to a RECT structure that contains the rectangle
		/// (in logical coordinates) in which the text is to be formatted.</param>
		/// <param name="uFormat">Specifies the method of formatting the text. </param>
		/// <returns>0, if fails.</returns>
		[DllImport( "user32.dll", CharSet = CharSet.Auto )]
		internal extern static int DrawText( IntPtr hdc, string lpString, int nCount, ref RECT lpRect, DrawTextFormatFlags uFormat );

		/// <summary>
		/// Computes the width and height of the specified string of text.
		/// </summary>
		/// <param name="hdc">Handle to the device context.</param>
		/// <param name="lpString">Pointer to a buffer that specifies the text string.</param>
		/// <param name="cbString">Specifies the length of the lpString buffer.</param>
		/// <param name="lpSize">Pointer to a SIZE structure that receives the dimensions of the string, in logical units. </param>
		/// <returns>0, if fails.</returns>
		[DllImport( "Gdi32.dll", CharSet = CharSet.Auto )]
		internal extern static int GetTextExtentPoint32( IntPtr hdc, string lpString, int cbString, ref SIZE lpSize );

		[DllImport( "Gdi32.dll" )]
		internal extern static int SetBkMode( IntPtr hdc, int bkMode );

		[DllImport( "Gdi32.dll" )]
		internal extern static int SetTextColor( IntPtr hdc, int crColor );

		[DllImport( "Gdi32.dll" )]
		internal extern static int SelectClipRgn( IntPtr hdc, IntPtr hrgn );

		[DllImport( "Gdi32.dll" )]
		internal extern static int GetClipRgn( IntPtr hdc, IntPtr hrgn );

		/// <summary>
		/// Selects an object into the specified device context (DC). The new object replaces the previous object of the same type.
		/// </summary>
		/// <param name="hdc">Handle to the DC.</param>
		/// <param name="newObject">Handle to the object to be selected.</param>
		/// <returns>If the selected object is not a region and the function succeeds,
		/// the return value is a handle to the object being replaced.</returns>
		[DllImport( "Gdi32.dll", CharSet = CharSet.Auto )]
		internal extern static IntPtr SelectObject( IntPtr hdc, IntPtr newObject );

		[DllImport( "Gdi32.dll", CharSet = CharSet.Auto )]
		internal extern static int DeleteObject( IntPtr Object );
		[DllImport( "gdi32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi )]
		internal static extern IntPtr CreateRectRgn( int x1, int y1, int x2, int y2 );

		[DllImport( "user32.dll" )]
		internal static extern IntPtr GetDCEx( IntPtr hWnd, IntPtr hrgnClip, DeviceContextValues flags );

		[DllImport( "user32.dll" )]
		internal static extern int ReleaseDC( IntPtr hWnd, IntPtr hDC );
	}

	internal class WinAPI
	{
		[DllImport("User32.dll", CharSet = CharSet.Auto)]
		public static extern IntPtr GetCursor();

		[DllImport( "Imm32.dll", SetLastError = true )]
		public static extern bool ImmSetCompositionWindow( IntPtr hIMC, ref tagCOMPOSITIONFORM lpCompForm );

		[DllImport( "Imm32.dll", SetLastError = true )]
		public static extern IntPtr ImmGetContext( IntPtr hWnd );
		[DllImport( "user32.dll" )]
		internal static extern bool GetWindowRect( IntPtr hWnd, out RECT lpRect );

		[DllImport("user32.dll")]
		internal static extern bool GetKeyboardLayoutName(StringBuilder pwszKLID);
		internal const int KL_NAMELENGTH = 9;

		/// <summary>
		/// Sends windows message to window.
		/// </summary>
		/// <param name="hWnd"></param>
		/// <param name="msg"></param>
		/// <param name="wParam"></param>
		/// <param name="lParam"></param>
		/// <returns></returns>
		[DllImport( "user32.dll", CharSet = CharSet.Auto )]
		internal extern static IntPtr SendMessage( IntPtr hWnd, int msg, int wParam, int lParam );

		[DllImport( "user32.dll", CharSet = CharSet.Auto )]
		extern internal static IntPtr SendMessage( IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam );

		[DllImport( "user32.dll", CharSet = CharSet.Auto )]
		extern internal static IntPtr SendMessage( IntPtr hWnd, int msg, IntPtr wParam, ref RECT lpRect );

		[DllImport( "user32.dll", CharSet = CharSet.Auto )]
		extern internal static IntPtr SendMessage( IntPtr hWnd, int msg, IntPtr wParam, int lParam );

		[DllImport( "user32.dll", CharSet = CharSet.Auto )]
		extern internal static IntPtr SendMessage( IntPtr hWnd, int msg, int wParam, IntPtr lParam );

		[DllImport( "user32.dll", CharSet = CharSet.Auto )]
		static internal extern bool SetWindowPos( IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int Width, int Height, uint flags );

		[DllImport( "comctl32.dll" )]
		internal static extern bool InitCommonControls();

		[DllImport( "user32.dll", CharSet = CharSet.Auto )]
		internal static extern int SendMessage( IntPtr hWnd, int msg, int wParam, ref TOOLINFO lParam );

		[DllImport( "user32.dll", CharSet = CharSet.Auto )]
		internal static extern IntPtr GetActiveWindow();

		[DllImport( "user32.dll", CharSet = CharSet.Auto )]
		internal static extern IntPtr GetForegroundWindow();
		/// <summary>
		/// Delegate for events related to the hooks.
		/// </summary>
		internal delegate IntPtr HookProc( int nCode, IntPtr wParam, IntPtr lParam );

		public const uint VK_LSHIFT = 0xA0;
		public const uint VK_RSHIFT = 0xA1;
		public const uint VK_LCONTROL = 0xA2;
		public const uint VK_RCONTROL = 0xA3;
		public const uint VK_LMENU = 0xA4;
		public const uint VK_RMENU = 0xA5;

		/// <summary>
		/// Copies the status of the 256 virtual keys to the specified buffer.
		/// </summary>
		/// <param name="lpKeyState">The 256-byte array that receives the status data for each virtual key.</param>
		/// <returns>If the function succeeds, the return value is nonzero.</returns>
		[DllImport("user32.dll")]
		public static extern int GetKeyboardState(byte[] lpKeyState);

		/// <summary>
		/// Gets ID of the current thread.
		/// </summary>
		/// <returns>ID of the current thread.</returns>
		[DllImport( "kernel32.dll", ExactSpelling = true, CharSet = CharSet.Auto )]
		internal static extern int GetCurrentThreadId();
		/// <summary>
		/// Installs an application-defined hook procedure into a hook 
		/// chain. You would install a hook procedure to monitor the 
		/// system for certain types of events. These events are 
		/// associated either with a specific thread or with all 
		/// threads in the same desktop as the calling thread. 
		/// </summary>
		/// <param name="hookid">Specifies the type of hook procedure to 
		/// be installed.</param>
		/// <param name="pfnhook">Delegate for the method that will 
		/// process mesages.</param>
		/// <param name="hinst">Handle to the DLL containing the hook 
		/// procedure pointed to by the lpfn parameter. The hMod 
		/// parameter must be set to NULL if the dwThreadId parameter 
		/// specifies a thread created by the current process and if 
		/// the hook procedure is within the code associated with the 
		/// current process.</param>
		/// <param name="threadid">Specifies the identifier of the 
		/// thread with which the hook procedure is to be associated. If this parameter is zero, the hook procedure is associated with all existing threads running in the same desktop as the calling thread.</param>
		/// <returns></returns>
		[DllImport( "user32.dll", CharSet = CharSet.Auto )]
		internal static extern IntPtr SetWindowsHookEx( int hookid, HookProc pfnhook, IntPtr hinst, int threadid );
		/// <summary>
		/// Removes a hook procedure installed in a hook chain by the 
		/// SetWindowsHookEx function.
		/// </summary>
		/// <param name="hhook">Handle to the hook to be removed. 
		/// This parameter is a hook handle obtained by a previous 
		/// call to SetWindowsHookEx.</param>
		/// <returns>If the function succeeds, the return value is nonzero. 
		/// If the function fails, the return value is zero. 
		/// To get extended error information, call GetLastError.
		/// </returns>
		[DllImport( "user32.dll", CharSet = CharSet.Auto, ExactSpelling = true )]
		internal static extern bool UnhookWindowsHookEx( IntPtr hhook );
		/// <summary>
		/// The CallNextHookEx function passes the hook information to the 
		/// next hook procedure in the current hook chain. A hook 
		/// procedure can call this function either before or after 
		/// processing the hook information. 
		/// </summary>
		/// <param name="hhook">Ignored.</param>
		/// <param name="code">Specifies the hook code passed to the 
		/// current hook procedure. The next hook procedure uses this 
		/// code to determine how to process the hook information.</param>
		/// <param name="wparam">Specifies the wParam value passed to the 
		/// current hook procedure. The meaning of this parameter depends 
		/// on the type of hook associated with the current hook chain.
		/// </param>
		/// <param name="lparam">Specifies the lParam value passed to 
		/// the current hook procedure. The meaning of this parameter 
		/// depends on the type of hook associated with the current 
		/// hook chain.</param>
		/// <returns>This value is returned by the next hook procedure 
		/// in the chain. The current hook procedure must also return 
		/// this value. The meaning of the return value depends on 
		/// the hook type. For more information, see the descriptions 
		/// of the individual hook procedures.</returns>
		[DllImport( "user32.dll", CharSet = CharSet.Auto, ExactSpelling = true )]
		internal static extern IntPtr CallNextHookEx( IntPtr hhook, int code, IntPtr wparam, IntPtr lparam );
		/// <summary>
		/// The GetAsyncKeyState function determines whether a key is up or down at the time the function is called, and whether the key was pressed after a previous call to GetAsyncKeyState. 
		/// </summary>
		/// <param name="key">Specifies one of 256 possible virtual-key codes.</param>
		/// <returns></returns>
		[DllImport( "user32.dll", CharSet = CharSet.Auto, ExactSpelling = true )]
		internal static extern UInt16 GetAsyncKeyState( Keys key );
		/// <summary>
		/// Changes the size, position, and Z order of a child, pop-up, or top-level window. Child, pop-up, and top-level windows are ordered according to their appearance on the screen. The topmost window receives the highest rank and is the first window in the Z order.
		/// </summary>
		/// <param name="hWnd">Handle to the window.</param>
		/// <param name="hWndInsertAfter">Handle to the window to precede the positioned window in the Z order. This parameter must be a window handle or one of the following values.</param>
		/// <param name="X">Specifies the new position of the left side of the window, in client coordinates.</param>
		/// <param name="Y">Specifies the new position of the top of the window, in client coordinates.</param>
		/// <param name="cx">Specifies the new width of the window, in pixels.</param>
		/// <param name="cy">Specifies the new height of the window, in pixels.</param>
		/// <param name="uFlags">Specifies the window sizing and positioning flags. </param>
		/// <returns>If the function succeeds, the return value is nonzero.</returns>
		[DllImport( "USER32.dll" )]
		internal static extern bool SetWindowPos( IntPtr hWnd, int hWndInsertAfter, int X, int Y, int cx, int cy, int uFlags );
		/// <summary>
		/// Sets the specified window's show state.
		/// </summary>
		/// <param name="hWnd">Handle to the window. </param>
		/// <param name="nCmdShow">Specifies how the window is to be shown. This parameter is ignored the first time an application calls ShowWindow, if the program that launched the application provides a STARTUPINFO structure. Otherwise, the first time ShowWindow is called, the value should be the value obtained by the WinMain function in its nCmdShow parameter.</param>
		/// <returns>If the window was previously visible, the return value is nonzero. If the window was previously hidden, the return value is zero.</returns>
		[DllImport( "user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi )]
		internal static extern bool ShowWindow( IntPtr hWnd, int nCmdShow );

		[DllImport( "user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi )]
		internal static extern bool SetParent( IntPtr hWnd, IntPtr hWndParent );

		/// <summary>
		/// Updates the specified rectangle or region in a window's client area.
		/// </summary>
		/// <param name="hWnd"></param>
		/// <param name="lprcUpdate"></param>
		/// <param name="hrgnUpdate"></param>
		/// <param name="flags"></param>
		/// <returns></returns>
		[DllImport( "user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi )]
		internal static extern bool RedrawWindow( IntPtr hWnd, IntPtr lprcUpdate, IntPtr hrgnUpdate, uint flags );

		[Flags]
		internal enum RedrawWindowFlags
		{
			RDW_INVALIDATE = 0x0001,
			RDW_INTERNALPAINT = 0x0002,
			RDW_ERASE = 0x0004,
			RDW_VALIDATE = 0x0008,
			RDW_NOINTERNALPAINT = 0x0010,
			RDW_NOERASE = 0x0020,
			RDW_NOCHILDREN = 0x0040,
			RDW_ALLCHILDREN = 0x0080,
			RDW_UPDATENOW = 0x0100,
			RDW_ERASENOW = 0x0200,
			RDW_FRAME = 0x0400,
			RDW_NOFRAME = 0x0800
		}

		[DllImport( "user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi )]
		internal static extern IntPtr SetWindowLong( IntPtr hWnd, int nIndex, IntPtr dwNewLong );

		internal const int GWL_WNDPROC = -4 /*0xFFFFFFFC*/;
		internal const int GWL_HWNDPARENT = -8 /*0xFFFFFFF8*/;
		internal const int GWL_STYLE = -16 /*0xFFFFFFF0*/;
		internal const int GWL_EXSTYLE = -20 /*0xFFFFFFEC*/;
		internal const int GWL_ID = -12 /*0xFFFFFFF4*/;

		[DllImport( "user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi )]
		internal static extern int GetWindowLong( IntPtr hWnd, int nIndex );

		/// <summary>
		/// Locks/unlocks window update.
		/// </summary>
		/// <param name="hWndLock">Handle of the window that should be locked.</param>
		/// <returns>True if lock/unlock succeeded.</returns>
		[DllImport( "user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi )]
		internal static extern bool LockWindowUpdate( IntPtr hWndLock );

		/// <summary>
		/// The AttachThreadInput function attaches or detaches the input processing mechanism of one thread to that of another thread.
		/// </summary>
		/// <param name="idAttach">Identifier of the thread to be attached to another thread.
		/// The thread to be attached cannot be a system thread.</param>
		/// <param name="idAttachTo">Identifier of the thread to which idAttach will be attached.
		/// This thread cannot be a system thread.
		/// A thread cannot attach to itself. Therefore, idAttachTo cannot equal idAttach.</param>
		/// <param name="fAttach">If this parameter is TRUE, the two threads are attached.
		/// If the parameter is FALSE, the threads are detached.</param>
		/// <returns>If the function succeeds, the return value is nonzero.	If the function fails, the return value is zero.
		/// There is no extended error information; do not call GetLastError.</returns>
		[DllImport( "user32.dll" )]
		internal static extern bool AttachThreadInput( uint idAttach, uint idAttachTo, bool fAttach );
	}

	internal class ScrollApi
	{
		/// <summary>
		/// Setups scroller to specified position.
		/// </summary>
		/// <param name="hWnd">Handle of the scrollable window (or control).</param>
		/// <param name="bar">Type of scrollbar to be scrolled.</param>
		/// <param name="nPosition">New position of the scroller.</param>
		/// <param name="bRedraw"></param>
		/// <returns></returns>
		[DllImport( "user32.dll" )]
		internal static extern int SetScrollPos( IntPtr hWnd, ScrollerConst bar, int nPosition, bool bRedraw );
		[DllImport( "user32.dll" )]
		internal static extern int GetScrollPos( IntPtr hWnd, ScrollerConst bar );

		[DllImport( "user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi )]
		internal static extern bool GetScrollInfo( IntPtr hWnd, int fnBar, ref SCROLLINFO si );

		[DllImport( "user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi )]
		internal static extern int SetScrollInfo( IntPtr hWnd, int fnBar, ref SCROLLINFO si, bool redraw );

		[DllImport( "user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi )]
		internal static extern bool ScrollWindowEx( IntPtr hWnd, int nXAmount, int nYAmount, ref RECT rectScrollRegion, ref RECT rectClip, IntPtr hrgnUpdate, ref RECT prcUpdate, int flags );

		[DllImport( "user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi )]
		internal static extern bool ScrollWindowEx( IntPtr hWnd, int nXAmount, int nYAmount, COMRECT rectScrollRegion, ref RECT rectClip, IntPtr hrgnUpdate, ref RECT prcUpdate, int flags );
	}

	internal enum tagCOMPOSITIONFORM_Position
	{
		CFS_DEFAULT = 0,
		CFS_RECT = 1,
		CFS_POINT = 2,
		CFS_SCREEN = 4,
		CFS_FORCE_POSITION = 32,
		CFS_CANDIDATEPOS = 64,
		CFS_EXCLUDE = 128,
	}

	internal enum WM_IME_SETCONTEXT_DisplayOptions
	{
		ISC_SHOWUICANDIDATEWINDOW = 1,
		ISC_SHOWUICOMPOSITIONWINDOW = unchecked( ( int )0x80000000 ),
		ISC_SHOWUIGUIDELINE = 0x40000000,
		ISC_SHOWUIALLCANDIDATEWINDOW = 15,
		ISC_SHOWUIALL = unchecked( ( int )0x80000000 ),
	}

	internal class Callback
	{
		public delegate int WindowProc( IntPtr hWnd, int nMsg, IntPtr wParam, IntPtr lParam );
	}
	
	[StructLayout( LayoutKind.Sequential )]
	internal struct MSG
	{
		public IntPtr hwnd;
		public int message;
		public IntPtr wParam;
		public IntPtr lParam;
		public int time;
		public int pt_x;
		public int pt_y;
	}
}