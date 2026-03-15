#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
//  Author: Jeff Boenig
//
#endregion

using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace Syncfusion.Runtime.InteropServices.WinAPI
{
    /// <internalonly/>
    [Syncfusion.Documentation.DocumentationExclude()]
    internal delegate bool CallBack(int hwnd, ref RECT lParam);

    /// <internalonly/>
    [Syncfusion.Documentation.DocumentationExclude()]
    internal delegate bool BrowserFolderCallback(IntPtr hwnd, [MarshalAs(UnmanagedType.U4)] int uMsg, IntPtr lParam, IntPtr lpData);

    [
    Guid(@"00000002-0000-0000-c000-000000000046"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)
    ]
    internal interface IMalloc
    {
        IntPtr Alloc(int cb);
        void Free(IntPtr pv);
        IntPtr Realloc(IntPtr pv, int cb);
        int GetSize(IntPtr pv);
        int DidAlloc(IntPtr pv);
        void HeapMinimize();
    }

    [
        ComVisibleAttribute(false),
        SuppressUnmanagedCodeSecurityAttribute(),
        Syncfusion.Documentation.DocumentationExclude()
    ]
    internal class Macros
    {
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        internal static int HIWORD(int n)
        {
            return ((n >> 16) & 0xffff/*=~0x0000*/);
        }

        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        internal static int LOWORD(int n)
        {
            return (n & 0xffff/*=~0x0000*/);
        }

        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        internal static int LOWORD(IntPtr n)
        {
            return LOWORD((int)n);
        }

        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        internal static int HIWORD(IntPtr n)
        {
            return HIWORD((int)n);
        }

        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        internal static int MAKELONG(int low, int high)
        {
            return ((high << 16) | (low & 0xffff));
        }

        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        internal static int MAKELPARAM(int low, int high)
        {
            return ((high << 16) | (low & 0xffff));
        }

        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        internal static int RGBToCOLORREF(int rgbValue)
        {
            int n0;
            n0 = (rgbValue & 255/*0xff*/) << 16/*0x10*/;
            rgbValue = (rgbValue & 16776960/*0xffff00*/);
            rgbValue = (rgbValue
                | (rgbValue >> 16/*0x10*/ & 255/*0xff*/));
            rgbValue = (rgbValue & 65535/*0xffff*/);
            rgbValue = (rgbValue | n0);
            return rgbValue;
        }

        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        internal static int COLORREFToRGB(int colorRef)
        {
            int r = colorRef & 255/*0xff*/;
            int g = (colorRef >> 8) & 255/*0xff*/;
            int b = (colorRef >> 16) & 255/*0xff*/;

            int rgb = (r << 16) + (g << 8) + b;

            return rgb;
        }

        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        internal static int GetRValue(int rgb)
        {
            return (rgb & 0xff0000) >> 16;
        }

        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        internal static int GetGValue(int rgb)
        {
            return (rgb & 0x00ff00) >> 8;
        }

        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        internal static int GetBValue(int rgb)
        {
            return (rgb & 0x0000ff);
        }
    }

    /// <internalonly/>	
    [StructLayout(LayoutKind.Sequential),
    Syncfusion.Documentation.DocumentationExclude()]
    internal struct POINT
    {
        internal int X;
        internal int Y;

        internal POINT(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
    }

    /// <internalonly/>
    [StructLayout(LayoutKind.Sequential),
    Syncfusion.Documentation.DocumentationExclude()]
    internal struct SIZE
    {
        internal int CX;
        internal int CY;

        internal SIZE(int cx, int cy)
        {
            this.CX = cx;
            this.CY = cy;
        }
    }

    /// <internalonly/>
    [StructLayout(LayoutKind.Sequential),
    Syncfusion.Documentation.DocumentationExclude()]
    internal struct RECT
    {
        internal int left;
        internal int top;
        internal int right;
        internal int bottom;

        internal RECT(Rectangle rect)
        {
            this.bottom = rect.Bottom;
            this.left = rect.Left;
            this.right = rect.Right;
            this.top = rect.Top;
        }

        internal RECT(int left, int top, int right, int bottom)
        {
            this.bottom = bottom;
            this.left = left;
            this.right = right;
            this.top = top;
        }

        internal static RECT FromXYWH(int x, int y, int width, int height)
        {
            return new RECT(x, y, x + width, y + height);
        }

        internal int Width { get { return this.right - this.left; } }
        internal int Height { get { return this.bottom - this.top; } }

        public override /*Object*/ string ToString()
        {
            return String.Concat(
                "Left = ",
                this.left,
                " Top ",
                this.top,
                " Right = ",
                this.right,
                " Bottom = ",
                this.bottom);
        }
    }

    /// <internalonly/>
    [StructLayout(LayoutKind.Sequential),
    Syncfusion.Documentation.DocumentationExclude()]
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

        internal COMRECT(int left, int top, int right, int bottom)
        {
            this.left = left;
            this.top = top;
            this.right = right;
            this.bottom = bottom;
            return;
        }

        // Methods
        public override /*Object*/ string ToString()
        {
            return String.Concat(
                "Left = ",
                this.left,
                " Top ",
                this.top,
                " Right = ",
                this.right,
                " Bottom = ",
                this.bottom);
        }

        internal static COMRECT FromXYWH(int x, int y, int width, int height)
        {
            return new COMRECT(x, y, (x + width), (y + height));
        }
    }

    [
    StructLayout(LayoutKind.Sequential),
    Syncfusion.Documentation.DocumentationExclude()
    ]
    internal struct SCROLLINFO
    {
        internal int cbSize;
        internal int fMask;
        internal int nMin;
        internal int nMax;
        internal int nPage;
        internal int nPos;
        internal int nTrackPos;
    }

    [
        ComVisibleAttribute(true),
        StructLayout(LayoutKind.Sequential),
        Syncfusion.Documentation.DocumentationExclude()
    ]
    internal class TOOLINFO_T
    {
        // Fields
        internal int cbSize = 0; // = Marshal.SizeOf(typeof(TOOLINFO_T));
        internal int uFlags = 0;
        internal IntPtr hWnd = IntPtr.Zero;
        internal int uId = 0;
        internal RECT rect = new RECT();
        internal IntPtr hinst = IntPtr.Zero;
        [MarshalAs(UnmanagedType.LPTStr)]
        internal string lpszText = "";
    }

    [
        StructLayout(LayoutKind.Sequential),
        Syncfusion.Documentation.DocumentationExclude()
    ]
    internal struct MSG
    {
        // Fields
        internal IntPtr hwnd;
        internal int message;
        internal IntPtr wParam;
        internal IntPtr lParam;
        internal int time;
        internal int pt_x;
        internal int pt_y;
    }

    [
        StructLayout(LayoutKind.Sequential),
        Syncfusion.Documentation.DocumentationExclude()
    ]
    internal struct LOGBRUSH
    {
        internal int lbStyle;
        internal int lbColor;
        internal IntPtr lbHatch;
    }

    [
        StructLayout(LayoutKind.Sequential),
        Syncfusion.Documentation.DocumentationExclude()
    ]
    internal struct LOGFONT
    {
        internal int lfHeight;
        internal int lfWidth;
        internal int lfEscapement;
        internal int lfOrientation;
        internal int lfWeight;
        internal byte lfItalic;
        internal byte lfUnderline;
        internal byte lfStrikeOut;
        internal byte lfCharSet;
        internal byte lfOutPrecision;
        internal byte lfClipPrecision;
        internal byte lfQuality;
        internal byte lfPitchAndFamily;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        internal string lfFaceName;
    }

    [
        StructLayout(LayoutKind.Sequential),
        Syncfusion.Documentation.DocumentationExclude()
    ]
    internal struct NONCLIENTMETRICS
    {
        internal int cbSize;
        internal int iBorderWidth;
        internal int iScrollWidth;
        internal int iScrollHeight;
        internal int iCaptionWidth;
        internal int iCaptionHeight;
        internal LOGFONT lfCaptionFont;
        internal int iSmCaptionWidth;
        internal int iSmCaptionHeight;
        internal LOGFONT lfSmCaptionFont;
        internal int iMenuWidth;
        internal int iMenuHeight;
        internal LOGFONT lfMenuFont;
        internal LOGFONT lfStatusFont;
        internal LOGFONT lfMessageFont;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal class TEXTMETRIC
    {

        // Fields
        internal int tmHeight = 0;
        internal int tmAscent = 0;
        internal int tmDescent = 0;
        internal int tmInternalLeading = 0;
        internal int tmExternalLeading = 0;
        internal int tmAveCharWidth = 0;
        internal int tmMaxCharWidth = 0;
        internal int tmWeight = 0;
        internal int tmOverhang = 0;
        internal int tmDigitizedAspectX = 0;
        internal int tmDigitizedAspectY = 0;
        internal char tmFirstChar = '\0';
        internal char tmLastChar = '\0';
        internal char tmDefaultChar = '\0';
        internal char tmBreakChar = '\0';
        internal byte tmItalic = 0;
        internal byte tmUnderlined = 0;
        internal byte tmStruckOut = 0;
        internal byte tmPitchAndFamily = 0;
        internal byte tmCharSet = 0;

        // Methods
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct MARGINS
    {
        internal int cxLeftWidth;      // width of left border that retains its size
        internal int cxRightWidth;     // width of right border that retains its size
        internal int cyTopHeight;      // height of top border that retains its size
        internal int cyBottomHeight;   // height of bottom border that retains its size

        internal MARGINS(int cxLeftWidth, int cxRightWidth, int cyTopHeight, int cyBottomHeight)
        {
            this.cxLeftWidth = cxLeftWidth;
            this.cxRightWidth = cxRightWidth;
            this.cyTopHeight = cyTopHeight;
            this.cyBottomHeight = cyBottomHeight;
        }
        public override string ToString()
        {
            return "LeftWidth: " + cxLeftWidth + " RightWidth: " + cxRightWidth +
                " TopHeight: " + cyTopHeight + " BottomHeight: " + cyBottomHeight;
        }
    }
    internal class INTLIST
    {
        int iValueCount = 0;      // number of values in iValues
        [MarshalAs(UnmanagedType.LPArray)]
        int[] iValues;

        internal INTLIST()
        {
            this.iValues = new int[10];
        }
        internal int ValueCount
        {
            get { return this.iValueCount; }
        }
    }

#if false
	[StructLayout(LayoutKind.Sequential)] 
	internal struct THEME_ERROR_CONTEXT
	{
		internal ulong dwSize;

		//---- error context information ----
		internal IntPtr hr;                     // error code from last error
		internal string szMsgParam1;    // value of first param for msg
		internal string szMsgParam2;    // value of second param for msg
		internal string szFileName;     // associated source filename
		internal string szSourceLine;   // source line
		internal int iLineNum;                   // source line number

		internal THEME_ERROR_CONTEXT(IntPtr hr, string szMsgParam1, string szMsgParam2, string szFileName, string szSourceLine,
			int iLineNum)
		{
			this.dwSize = 0;
			this.hr = hr;
			this.szMsgParam1 = szMsgParam1;
			this.szMsgParam2 = szMsgParam2;
			this.szFileName = szFileName;
			this.szSourceLine = szSourceLine;
			this.iLineNum = iLineNum;
		}

	}
#endif

    [
        StructLayout(LayoutKind.Sequential),
        ComVisible(false),
        Syncfusion.Documentation.DocumentationExclude()
    ]
    internal class BROWSEINFO
    {
        internal IntPtr hwndOwner = IntPtr.Zero;
        internal IntPtr pidlRoot = IntPtr.Zero;
        internal IntPtr pszDisplayName = IntPtr.Zero;
        [MarshalAs(UnmanagedType.LPTStr)]
        internal string lpszTitle = String.Empty;
        internal int ulFlags = 0;
        [MarshalAs(UnmanagedType.FunctionPtr)]
        internal BrowserFolderCallback lpfn = null;
        internal IntPtr lParam = IntPtr.Zero;
        internal int iImage = -1;
    }

#if false
	[
		ComVisibleAttribute(false),
		SuppressUnmanagedCodeSecurityAttribute(),
		Syncfusion.Documentation.DocumentationExclude()
	]
	internal class Kernel
	{
		[DllImport("kernel32.dll", CharSet=CharSet.Auto, CallingConvention=CallingConvention.Winapi)] 
		internal static extern IntPtr GetModuleHandle(string modName); 

		[DllImport("kernel32.dll", CharSet=CharSet.Auto, ExactSpelling=true, CallingConvention=CallingConvention.Winapi)] 
		internal static extern int GetCurrentThreadId();
	}

	[
		ComVisibleAttribute(false),
		SuppressUnmanagedCodeSecurityAttribute(),
		Syncfusion.Documentation.DocumentationExclude()
	]
	internal class CommonControls
	{
		[
			ComVisibleAttribute(true),
			StructLayout(LayoutKind.Sequential, Pack = 1) ,
			Syncfusion.Documentation.DocumentationExclude()
			]
			internal class INITCOMMONCONTROLSEX 
		{
			internal int dwSize = Marshal.SizeOf(typeof(INITCOMMONCONTROLSEX));
			internal int dwICC = 0;
		}

		[DllImport("comctl32")]
		extern internal static void InitCommonControls()  ;

		[DllImport("comctl32")]
		extern internal static bool InitCommonControlsEx(INITCOMMONCONTROLSEX icc)  ;

		[DllImport("comctl32")]
		extern internal static bool InitializeFlatSB(IntPtr hWnd);

		[DllImport("comctl32")]
		extern internal static bool UninitializeFlatSB(IntPtr hWnd);

		[DllImport("comctl32")]
		extern internal static int FlatSB_SetScrollInfo(IntPtr hWnd, int fnBar, ref SCROLLINFO si, bool redraw)  ;

		[DllImport("comctl32")]
		extern internal static bool FlatSB_GetScrollInfo(IntPtr hWnd, int fnBar, ref SCROLLINFO si);

		[DllImport("comctl32")]
		extern internal static bool FlatSB_SetScrollProp(IntPtr hWnd, int index, int newValue, bool fRedraw);

		[DllImport("comctl32")]
		extern internal static bool FlatSB_GetScrollProp(IntPtr hWnd, int index, ref int value);

		[DllImport("comctl32")]
		extern internal static bool FlatSB_EnableScrollBar(IntPtr hWnd, int wSBflags, int wArrows);
	}

	[
		ComVisibleAttribute(false),
		SuppressUnmanagedCodeSecurityAttribute(),
		Syncfusion.Documentation.DocumentationExclude()
	]
	internal class Shell
	{
		[DllImport("shell32.dll", CallingConvention=CallingConvention.Winapi)] 
		internal static extern int SHGetSpecialFolderLocation(IntPtr hwnd, int csidl, ref IntPtr ppidl); 
		        
		[DllImport("shell32.dll", CharSet=CharSet.Auto, CallingConvention=CallingConvention.Winapi)] 
		internal static extern bool SHGetPathFromIDList(IntPtr pidl, IntPtr pszPath); 
		        
		[DllImport("shell32.dll", CharSet=CharSet.Auto, CallingConvention=CallingConvention.Winapi)] 
		internal static extern IntPtr SHBrowseForFolder([In] BROWSEINFO lpbi); 
		        
		[DllImport("shell32.dll", CallingConvention=CallingConvention.Winapi)] 
		internal static extern int SHGetMalloc([Out,MarshalAs(UnmanagedType.LPArray)] IMalloc[] ppMalloc); 

		[DllImport("shell32.dll")]
		internal static extern int  SHAppBarMessage(int  dwMessage, ref Window.APPBARDATA pData);
	}
#endif

    [
        ComVisibleAttribute(false),
        SuppressUnmanagedCodeSecurityAttribute(),
        Syncfusion.Documentation.DocumentationExclude()
    ]
    internal class Window
    {
        internal const int SW_ERASE = 4; // 0x0004 
        internal const int SW_INVALIDATE = 2; // 0x0002 
        internal const int SW_SCROLLCHILDREN = 1; // 0x0001 
        internal const int SW_SMOOTHSCROLL = 0x0010; // Use smooth scrolling 

        internal const Int32 HTCLIENT = 1; // 0x0001 
        internal const int HTBOTTOMLEFT = 16;
        internal const int HTBOTTOMRIGHT = 17;
        internal const int HTBOTTOM = 15;
        internal const int HTRIGHT = 11;
        internal const int HTTOP = 12;
        internal const int HTTOPLEFT = 13;
        internal const int HTTOPRIGHT = 14;
        internal const int HTLEFT = 10;
        internal const int HTBORDER = 18;

        internal const Int32 SM_CXVSCROLL = 2;
        internal const Int32 SM_CYHSCROLL = 3;

        internal const int HWND_TOP = 0;
        internal const int HWND_BOTTOM = 1; // 0x0001 
        internal const int HWND_TOPMOST = -1; // 0xffff 
        internal const int HWND_NOTOPMOST = -2; // 0xfffe 

        internal const int WS_OVERLAPPED = 0 /*0x0000*/;
        internal const int WS_POPUP = -2147483648 /*0x80000000*/;
        internal const int WS_CHILD = 1073741824 /*0x40000000*/;
        internal const int WS_MINIMIZE = 536870912 /*0x20000000*/;
        internal const int WS_VISIBLE = 268435456 /*0x10000000*/;
        internal const int WS_DISABLED = 134217728 /*0x8000000*/;
        internal const int WS_CLIPSIBLINGS = 67108864 /*0x4000000*/;
        internal const int WS_CLIPCHILDREN = 33554432 /*0x2000000*/;
        internal const int WS_MAXIMIZE = 16777216 /*0x1000000*/;
        internal const int WS_CAPTION = 12582912 /*0xC00000*/;
        internal const int WS_BORDER = 8388608 /*0x800000*/;
        internal const int WS_DLGFRAME = 4194304 /*0x400000*/;
        internal const int WS_VSCROLL = 2097152 /*0x200000*/;
        internal const int WS_HSCROLL = 1048576 /*0x100000*/;
        internal const int WS_SYSMENU = 524288 /*0x80000*/;
        internal const int WS_THICKFRAME = 262144 /*0x40000*/;
        internal const int WS_TABSTOP = 65536 /*0x10000*/;
        internal const int WS_MINIMIZEBOX = 131072 /*0x20000*/;
        internal const int WS_MAXIMIZEBOX = 65536 /*0x10000*/;
        internal const int WS_EX_DLGMODALFRAME = 1 /*0x0001*/;
        internal const int WS_EX_MDICHILD = 64 /*0x0040*/;
        internal const int WS_EX_TOOLWINDOW = 128 /*0x0080*/;
        internal const int WS_EX_CLIENTEDGE = 512 /*0x0200*/;
        internal const int WS_EX_CONTEXTHELP = 1024 /*0x0400*/;
        internal const int WS_EX_RIGHT = 4096 /*0x1000*/;
        internal const int WS_EX_LEFT = 0 /*0x0000*/;
        internal const int WS_EX_RTLREADING = 8192 /*0x2000*/;
        internal const int WS_EX_LEFTSCROLLBAR = 16384 /*0x4000*/;
        internal const int WS_EX_CONTROLPARENT = 65536 /*0x10000*/;
        internal const int WS_EX_STATICEDGE = 131072 /*0x20000*/;
        internal const int WS_EX_APPWINDOW = 262144 /*0x40000*/;
        internal const int WS_EX_LAYERED = 524288 /*0x80000*/;
        internal const int WS_EX_TOPMOST = 8 /*0x0008*/;

        internal const int WM_REFLECT = 0x2000;

        internal const int WM_ACTIVATE = 6; // 0x0006 
        internal const int WM_ACTIVATEAPP = 28; // 0x001c 
        internal const int WM_AFXFIRST = 864; // 0x0360 
        internal const int WM_AFXLAST = 895; // 0x037f 
        internal const int WM_APP = 32768; // 0x8000 
        internal const int WM_ASKCBFORMATNAME = 780; // 0x030c 
        internal const int WM_CANCELJOURNAL = 75; // 0x004b 
        internal const int WM_CANCELMODE = 31; // 0x001f 
        internal const int WM_CAPTURECHANGED = 533; // 0x0215 
        internal const int WM_CHANGECBCHAIN = 781; // 0x030d 
        internal const int WM_CHANGEUISTATE = 295; // 0x0127 
        internal const int WM_CHAR = 258; // 0x0102 
        internal const int WM_CHARTOITEM = 47; // 0x002f 
        internal const int WM_CHILDACTIVATE = 34; // 0x0022 
        internal const int WM_CHOOSEFONT_GETLOGFONT = 1025; // 0x0401 
        internal const int WM_CLEAR = 771; // 0x0303 
        internal const int WM_CLOSE = 16; // 0x0010 
        internal const int WM_COMMAND = 273; // 0x0111 
        internal const int WM_COMMNOTIFY = 68; // 0x0044 
        internal const int WM_COMPACTING = 65; // 0x0041 
        internal const int WM_COMPAREITEM = 57; // 0x0039 
        internal const int WM_CONTEXTMENU = 123; // 0x007b 
        internal const int WM_COPY = 769; // 0x0301 
        internal const int WM_COPYDATA = 74; // 0x004a 
        internal const int WM_CREATE = 1; // 0x0001 
        internal const int WM_CTLCOLORBTN = 309; // 0x0135 
        internal const int WM_CTLCOLORDLG = 310; // 0x0136 
        internal const int WM_CTLCOLOREDIT = 307; // 0x0133 
        internal const int WM_CTLCOLORLISTBOX = 308; // 0x0134 
        internal const int WM_CTLCOLORMSGBOX = 306; // 0x0132 
        internal const int WM_CTLCOLORSCROLLBAR = 311; // 0x0137 
        internal const int WM_CTLCOLORSTATIC = 312; // 0x0138 
        internal const int WM_CUT = 768; // 0x0300 
        internal const int WM_DDE_ACK = 996; // 0x03e4 
        internal const int WM_DDE_ADVISE = 994; // 0x03e2 
        internal const int WM_DDE_DATA = 997; // 0x03e5 
        internal const int WM_DDE_EXECUTE = 1000; // 0x03e8 
        internal const int WM_DDE_FIRST = 992; // 0x03e0 
        internal const int WM_DDE_INITIATE = 992; // 0x03e0 
        internal const int WM_DDE_LAST = 1000; // 0x03e8 
        internal const int WM_DDE_POKE = 999; // 0x03e7 
        internal const int WM_DDE_REQUEST = 998; // 0x03e6 
        internal const int WM_DDE_TERMINATE = 993; // 0x03e1 
        internal const int WM_DDE_UNADVISE = 995; // 0x03e3 
        internal const int WM_DEADCHAR = 259; // 0x0103 
        internal const int WM_DELETEITEM = 45; // 0x002d 
        internal const int WM_DESTROY = 2; // 0x0002 
        internal const int WM_DESTROYCLIPBOARD = 775; // 0x0307 
        internal const int WM_DEVICECHANGE = 537; // 0x0219 
        internal const int WM_DEVMODECHANGE = 27; // 0x001b 
        internal const int WM_DISPLAYCHANGE = 126; // 0x007e 
        internal const int WM_DRAWCLIPBOARD = 776; // 0x0308 
        internal const int WM_DRAWITEM = 43; // 0x002b 
        internal const int WM_DROPFILES = 563; // 0x0233 
        internal const int WM_ENABLE = 10; // 0x000a 
        internal const int WM_ENDSESSION = 22; // 0x0016 
        internal const int WM_ENTERIDLE = 289; // 0x0121 
        internal const int WM_ENTERMENULOOP = 529; // 0x0211 
        internal const int WM_ENTERSIZEMOVE = 561; // 0x0231 
        internal const int WM_ERASEBKGND = 20; // 0x0014 
        internal const int WM_EXITMENULOOP = 530; // 0x0212 
        internal const int WM_EXITSIZEMOVE = 562; // 0x0232 
        internal const int WM_FONTCHANGE = 29; // 0x001d 
        internal const int WM_GETDLGCODE = 135; // 0x0087 
        internal const int WM_GETFONT = 49; // 0x0031 
        internal const int WM_GETHOTKEY = 51; // 0x0033 
        internal const int WM_GETICON = 127; // 0x007f 
        internal const int WM_GETMINMAXINFO = 36; // 0x0024 
        internal const int WM_GETOBJECT = 61; // 0x003d 
        internal const int WM_GETTEXT = 13; // 0x000d 
        internal const int WM_GETTEXTLENGTH = 14; // 0x000e 
        internal const int WM_HANDHELDFIRST = 856; // 0x0358 
        internal const int WM_HANDHELDLAST = 863; // 0x035f 
        internal const int WM_HELP = 83; // 0x0053 
        internal const int WM_HOTKEY = 786; // 0x0312 
        internal const int WM_HSCROLL = 276; // 0x0114 
        internal const int WM_HSCROLLCLIPBOARD = 782; // 0x030e 
        internal const int WM_ICONERASEBKGND = 39; // 0x0027 
        internal const int WM_IME_CHAR = 646; // 0x0286 
        internal const int WM_IME_COMPOSITION = 271; // 0x010f 
        internal const int WM_IME_COMPOSITIONFULL = 644; // 0x0284 
        internal const int WM_IME_CONTROL = 643; // 0x0283 
        internal const int WM_IME_ENDCOMPOSITION = 270; // 0x010e 
        internal const int WM_IME_KEYDOWN = 656; // 0x0290 
        internal const int WM_IME_KEYLAST = 271; // 0x010f 
        internal const int WM_IME_KEYUP = 657; // 0x0291 
        internal const int WM_IME_NOTIFY = 642; // 0x0282 
        internal const int WM_IME_SELECT = 645; // 0x0285 
        internal const int WM_IME_SETCONTEXT = 641; // 0x0281 
        internal const int WM_IME_STARTCOMPOSITION = 269; // 0x010d 
        internal const int WM_INITDIALOG = 272; // 0x0110 
        internal const int WM_INITMENU = 278; // 0x0116 
        internal const int WM_INITMENUPOPUP = 279; // 0x0117 
        internal const int WM_INPUTLANGCHANGE = 81; // 0x0051 
        internal const int WM_INPUTLANGCHANGEREQUEST = 80; // 0x0050 
        internal const int WM_KEYDOWN = 256; // 0x0100 
        internal const int WM_KEYFIRST = 256; // 0x0100 
        internal const int WM_KEYLAST = 264; // 0x0108 
        internal const int WM_KEYUP = 257; // 0x0101 
        internal const int WM_KILLFOCUS = 8; // 0x0008 
        internal const int WM_LBUTTONDBLCLK = 515; // 0x0203 
        internal const int WM_LBUTTONDOWN = 513; // 0x0201 
        internal const int WM_LBUTTONUP = 514; // 0x0202 
        internal const int WM_MBUTTONDBLCLK = 521; // 0x0209 
        internal const int WM_MBUTTONDOWN = 519; // 0x0207 
        internal const int WM_MBUTTONUP = 520; // 0x0208 
        internal const int WM_MDIACTIVATE = 546; // 0x0222 
        internal const int WM_MDICASCADE = 551; // 0x0227 
        internal const int WM_MDICREATE = 544; // 0x0220 
        internal const int WM_MDIDESTROY = 545; // 0x0221 
        internal const int WM_MDIGETACTIVE = 553; // 0x0229 
        internal const int WM_MDIICONARRANGE = 552; // 0x0228 
        internal const int WM_MDIMAXIMIZE = 549; // 0x0225 
        internal const int WM_MDINEXT = 548; // 0x0224 
        internal const int WM_MDIREFRESHMENU = 564; // 0x0234 
        internal const int WM_MDIRESTORE = 547; // 0x0223 
        internal const int WM_MDISETMENU = 560; // 0x0230 
        internal const int WM_MDITILE = 550; // 0x0226 
        internal const int WM_MEASUREITEM = 44; // 0x002c 
        internal const int WM_MENUCHAR = 288; // 0x0120 
        internal const int WM_MENUSELECT = 287; // 0x011f 
        internal const int WM_MOUSEACTIVATE = 33; // 0x0021 
        //internal Read-only static int WM_MOUSEENTER;
        internal const int WM_MOUSEFIRST = 512; // 0x0200 
        internal const int WM_MOUSEHOVER = 673; // 0x02a1 
        internal const int WM_MOUSELAST = 522; // 0x020a 
        internal const int WM_MOUSELEAVE = 675; // 0x02a3 
        internal const int WM_MOUSEMOVE = 512; // 0x0200 
        internal const int WM_MOUSEWHEEL = 522; // 0x020a 
        internal const int WM_MOVE = 3; // 0x0003 
        internal const int WM_MOVING = 534; // 0x0216 
        internal const int WM_NCACTIVATE = 134; // 0x0086 
        internal const int WM_NCCALCSIZE = 131; // 0x0083 
        internal const int WM_NCCREATE = 129; // 0x0081 
        internal const int WM_NCDESTROY = 130; // 0x0082 
        internal const int WM_NCHITTEST = 132; // 0x0084 
        internal const int WM_NCLBUTTONDBLCLK = 163; // 0x00a3 
        internal const int WM_NCLBUTTONDOWN = 161; // 0x00a1 
        internal const int WM_NCLBUTTONUP = 162; // 0x00a2 
        internal const int WM_NCMBUTTONDBLCLK = 169; // 0x00a9 
        internal const int WM_NCMBUTTONDOWN = 167; // 0x00a7 
        internal const int WM_NCMBUTTONUP = 168; // 0x00a8 
        internal const int WM_NCMOUSEHOVER = 672; // 0x02a0 
        internal const int WM_NCMOUSELEAVE = 674; // 0x02a2 
        internal const int WM_NCMOUSEMOVE = 160; // 0x00a0 
        internal const int WM_NCPAINT = 133; // 0x0085 
        internal const int WM_NCRBUTTONDBLCLK = 166; // 0x00a6 
        internal const int WM_NCRBUTTONDOWN = 164; // 0x00a4 
        internal const int WM_NCRBUTTONUP = 165; // 0x00a5 
        internal const int WM_NEXTDLGCTL = 40; // 0x0028 
        internal const int WM_NEXTMENU = 531; // 0x0213 
        internal const int WM_NOTIFY = 78; // 0x004e 
        internal const int WM_NOTIFYFORMAT = 85; // 0x0055 
        internal const int WM_NULL = 0;
        internal const int WM_PAINT = 15; // 0x000f 
        internal const int WM_PAINTCLIPBOARD = 777; // 0x0309 
        internal const int WM_PAINTICON = 38; // 0x0026 
        internal const int WM_PALETTECHANGED = 785; // 0x0311 
        internal const int WM_PALETTEISCHANGING = 784; // 0x0310 
        internal const int WM_PARENTNOTIFY = 528; // 0x0210 
        internal const int WM_PASTE = 770; // 0x0302 
        internal const int WM_PENWINFIRST = 896; // 0x0380 
        internal const int WM_PENWINLAST = 911; // 0x038f 
        internal const int WM_POWER = 72; // 0x0048 
        internal const int WM_POWERBROADCAST = 536; // 0x0218 
        internal const int WM_PRINT = 791; // 0x0317 
        internal const int WM_PRINTCLIENT = 792; // 0x0318 
        internal const int WM_PSD_ENVSTAMPRECT = 1029; // 0x0405 
        internal const int WM_PSD_FULLPAGERECT = 1025; // 0x0401 
        internal const int WM_PSD_GREEKTEXTRECT = 1028; // 0x0404 
        internal const int WM_PSD_MARGINRECT = 1027; // 0x0403 
        internal const int WM_PSD_MINMARGINRECT = 1026; // 0x0402 
        internal const int WM_PSD_PAGESETUPDLG = 1024; // 0x0400 
        internal const int WM_PSD_YAFULLPAGERECT = 1030; // 0x0406 
        internal const int WM_QUERYDRAGICON = 55; // 0x0037 
        internal const int WM_QUERYENDSESSION = 17; // 0x0011 
        internal const int WM_QUERYNEWPALETTE = 783; // 0x030f 
        internal const int WM_QUERYOPEN = 19; // 0x0013 
        internal const int WM_QUERYUISTATE = 297; // 0x0129 
        internal const int WM_QUEUESYNC = 35; // 0x0023 
        internal const int WM_QUIT = 18; // 0x0012 
        internal const int WM_RBUTTONDBLCLK = 518; // 0x0206 
        internal const int WM_RBUTTONDOWN = 516; // 0x0204 
        internal const int WM_RBUTTONUP = 517; // 0x0205 
        internal const int WM_RENDERALLFORMATS = 774; // 0x0306 
        internal const int WM_RENDERFORMAT = 773; // 0x0305 
        internal const int WM_SETCURSOR = 32; // 0x0020 
        internal const int WM_SETFOCUS = 7; // 0x0007 
        internal const int WM_SETFONT = 48; // 0x0030 
        internal const int WM_SETHOTKEY = 50; // 0x0032 
        internal const int WM_SETICON = 128; // 0x0080 
        internal const int WM_SETREDRAW = 11; // 0x000b 
        internal const int WM_SETTEXT = 12; // 0x000c 
        internal const int WM_SETTINGCHANGE = 26; // 0x001a 
        internal const int WM_SHOWWINDOW = 24; // 0x0018 
        internal const int WM_SIZE = 5; // 0x0005 
        internal const int WM_SIZECLIPBOARD = 779; // 0x030b 
        internal const int WM_SIZING = 532; // 0x0214 
        internal const int WM_SPOOLERSTATUS = 42; // 0x002a 
        internal const int WM_STYLECHANGED = 125; // 0x007d 
        internal const int WM_STYLECHANGING = 124; // 0x007c 
        internal const int WM_SYSCHAR = 262; // 0x0106 
        internal const int WM_SYSCOLORCHANGE = 21; // 0x0015 
        internal const int WM_SYSCOMMAND = 274; // 0x0112 
        internal const int WM_SYSDEADCHAR = 263; // 0x0107 
        internal const int WM_SYSKEYDOWN = 260; // 0x0104 
        internal const int WM_SYSKEYUP = 261; // 0x0105 
        internal const int WM_TCARD = 82; // 0x0052 
        internal const int WM_TIMECHANGE = 30; // 0x001e 
        internal const int WM_TIMER = 275; // 0x0113 
        internal const int WM_UNDO = 772; // 0x0304 
        internal const int WM_UPDATEUISTATE = 296; // 0x0128 
        internal const int WM_USER = 1024; // 0x0400 
        internal const int WM_USERCHANGED = 84; // 0x0054 
        internal const int WM_VKEYTOITEM = 46; // 0x002e 
        internal const int WM_VSCROLL = 277; // 0x0115 
        internal const int WM_VSCROLLCLIPBOARD = 778; // 0x030a 
        internal const int WM_WINDOWPOSCHANGED = 71; // 0x0047 
        internal const int WM_WINDOWPOSCHANGING = 70; // 0x0046 
        internal const int WM_WININICHANGE = 26; // 0x001a 

        internal const int SBM_ENABLE_ARROWS = 228; // 0x00e4 
        internal const int SBM_GETPOS = 225; // 0x00e1 
        internal const int SBM_GETRANGE = 227; // 0x00e3 
        internal const int SBM_GETSCROLLINFO = 234; // 0x00ea 
        internal const int SBM_SETPOS = 224; // 0x00e0 
        internal const int SBM_SETRANGE = 226; // 0x00e2 
        internal const int SBM_SETRANGEREDRAW = 230; // 0x00e6 
        internal const int SBM_SETSCROLLINFO = 233; // 0x00e9 

        internal const int BFFM_ENABLEOK = WM_USER + 101;
        internal const int BFFM_SETSELECTIONW = WM_USER + 103;
        internal const int BFFM_SETSTATUSTEXTW = WM_USER + 104;
        internal const int BFFM_SETOKTEXT = WM_USER + 105;
        internal const int BFFM_SETEXPANDED = WM_USER + 106;

        internal const int BFFM_INITIALIZED = 1;
        internal const int BFFM_SELCHANGED = 2;
        internal const int BFFM_VALIDATEFAILEDW = 4;   // lParam:wzPath ret:1(cont),0(EndDialog)
        internal const int BFFM_IUNKNOWN = 5;   // provides IUnknown to client. lParam: IUnknown*

        internal const int GW_HWNDFIRST = 0;
        internal const int GW_HWNDLAST = 1;
        internal const int GW_HWNDNEXT = 2;
        internal const int GW_HWNDPREV = 3;
        internal const int GW_OWNER = 4;
        internal const int GW_MAX = 5;
        internal const int GW_CHILD = 5;

        internal const int QS_KEY = 0x0001;
        internal const int QS_MOUSEMOVE = 0x0002;
        internal const int QS_MOUSEBUTTON = 0x0004;
        internal const int QS_POSTMESSAGE = 0x0008;
        internal const int QS_TIMER = 0x0010;
        internal const int QS_PAINT = 0x0020;
        internal const int QS_SENDMESSAGE = 0x0040;
        internal const int QS_HOTKEY = 0x0080;
        internal const int QS_ALLPOSTMESSAGE = 0x0100;
        internal const int QS_RAWINPUT = 0x0400;
        internal const int QS_MOUSE = (QS_MOUSEMOVE | QS_MOUSEBUTTON);
        internal const int QS_INPUT = (QS_MOUSE | QS_KEY | QS_RAWINPUT);
        internal const int QS_ALLEVENTS = (QS_INPUT | QS_POSTMESSAGE | QS_TIMER | QS_PAINT | QS_HOTKEY);
        internal const int QS_ALLINPUT = (QS_INPUT | QS_POSTMESSAGE | QS_TIMER | QS_PAINT | QS_HOTKEY | QS_SENDMESSAGE);
        internal const int WAIT_OBJECT_0 = 0;
        internal const int WAIT_TIMEOUT = 258;
#if false
		internal const uint INFINITE = 0xFFFFFFFF;
#endif

        [DllImport("USER32.dll")]
        extern internal static bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, int uFlags);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool ScrollWindowEx(IntPtr hWnd, int nXAmount, int nYAmount, ref RECT rectScrollRegion, ref RECT rectClip, IntPtr hrgnUpdate, ref RECT prcUpdate, int flags);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool ScrollWindowEx(IntPtr hWnd, int nXAmount, int nYAmount, COMRECT rectScrollRegion, ref RECT rectClip, IntPtr hrgnUpdate, ref RECT prcUpdate, int flags);

        [DllImport("user32", CharSet = CharSet.Auto, ExactSpelling = true)]
        extern internal static bool InvertRect(IntPtr hDC, ref RECT lpRect);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        extern internal static IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        extern internal static IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        extern internal static IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, int lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        extern internal static IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, IntPtr lParam);

        [DllImport("user32", CharSet = CharSet.Auto)]
        extern internal static int SendMessage(IntPtr hWnd, int Msg, int wParam, TOOLINFO_T lParam);

        [DllImport("user32")]
        internal static extern bool ScrollWindow(IntPtr hWnd, int nXAmount, int nYAmount, ref RECT rectScrollRegion, ref RECT rectClip);

        [DllImport("user32", CharSet = CharSet.Auto, ExactSpelling = true)]
        extern internal static int SetScrollInfo(IntPtr hWnd, int fnBar, ref SCROLLINFO si, bool redraw);

        [DllImport("user32", CharSet = CharSet.Auto, ExactSpelling = true)]
        extern internal static bool GetScrollInfo(IntPtr hWnd, int fnBar, ref SCROLLINFO si);

        [DllImport("user32")]
        extern internal static IntPtr GetSysColorBrush(int nIndex);

        [DllImport("user32")]
        extern internal static int GetSysColor(int nIndex);

        [DllImport("user32", CharSet = CharSet.Auto, ExactSpelling = true)]
        extern internal static int FillRect(IntPtr hdc, ref RECT rect, int hBrush);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        internal static extern IntPtr ChildWindowFromPoint(IntPtr hwndParent, int x, int y);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        internal static extern IntPtr WindowFromPoint(int x, int y);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool SubtractRect([In] ref RECT rcdest, ref RECT rc1, ref RECT rc2);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool LockWindowUpdate(IntPtr hWndLock);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool RedrawWindow(IntPtr hWnd, IntPtr lprcUpdate, IntPtr hrgnUpdate, uint flags);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool AnimateWindow(IntPtr hwnd, uint dwTime, uint dwFlags);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        internal static extern IntPtr GetWindow(IntPtr hwnd, uint uCmd);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        internal static extern IntPtr GetActiveWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        internal static extern IntPtr SetActiveWindow(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        internal static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool IsWindow(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        internal static extern IntPtr SetFocus(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        internal static extern IntPtr GetFocus();

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        internal static extern IntPtr GetParent(IntPtr hwnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        internal static extern IntPtr SetParent(IntPtr hwndchild, IntPtr hwnparent);

        // Unsafe calls
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        internal static extern IntPtr GetDCEx(IntPtr hWnd, IntPtr hrgnClip, int flags);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        internal static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool DrawFrameControl(IntPtr hDC, ref RECT rect, int type, int state);

        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        internal delegate IntPtr HookProc(int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        internal static extern IntPtr SetWindowsHookEx(int idHook, HookProc pfnHook, IntPtr hinst, int dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool UnhookWindowsHookEx(IntPtr hhook);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        internal static extern IntPtr CallNextHookEx(IntPtr hhook, int code, IntPtr wparam, IntPtr lparam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        internal static extern IntPtr GetAncestor(IntPtr hwnd, int gaFlags);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        extern internal static int ClientToScreen(IntPtr hWnd, ref POINT pt);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        extern internal static int MapWindowPoints(IntPtr hWndFrom, IntPtr hWndTo,
            [MarshalAs(UnmanagedType.LPArray)] POINT[] lpPoints, uint cPoints);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        extern internal static int ScreenToClient(IntPtr hWnd, ref POINT pt);

        [DllImport("user32", CharSet = CharSet.Auto)]
        extern internal static int SendDlgItemMessage(IntPtr hDlg, int nIDDlgItem, int Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32", CharSet = CharSet.Auto, ExactSpelling = true)]
        extern internal static IntPtr GetDlgItem(IntPtr hWnd, int nIDDlgItem);

        [DllImport("user32", CharSet = CharSet.Auto, ExactSpelling = true)]
        extern internal static bool EnableWindow(IntPtr hWnd, bool enable);

        [DllImport("user32", CharSet = CharSet.Auto, ExactSpelling = true)]
        extern internal static int GetDlgItemInt(IntPtr hWnd, int nIDDlgItem, bool[] err, bool signed);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        extern internal static bool PostMessage(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam);

        [DllImport("user32.dll")]
        internal static extern int GetSystemMetrics(int nIndex);

        [
            StructLayout(LayoutKind.Sequential),
            Syncfusion.Documentation.DocumentationExclude()
        ]
        internal struct APPBARDATA
        {
            internal int cbSize;
            internal int hwnd;
            internal int uCallbackMessage;
            internal int uEdge;
            internal RECT rc;
            internal int lParam;
        }

        internal const int ABM_GETTASKBARPOS = 5;
        internal const int SPI_GETWORKAREA = 48;

        internal const int ABE_BOTTOM = 3;
        internal const int ABE_LEFT = 0;
        internal const int ABE_RIGHT = 2;
        internal const int ABE_TOP = 1;

        [DllImport("user32.dll")]
        internal static extern int SystemParametersInfo(int uAction, int uParam, ref RECT lpvParam, int fuWinIni);

        [DllImport("user32.dll")]
        internal static extern int SystemParametersInfo(int uAction, int uParam, ref NONCLIENTMETRICS lpvParam, int fuWinIni);

        [DllImport("user32.dll")]
        internal static extern int SystemParametersInfo(int uAction, int uParam, ref bool lpvParam, int fuWinIni);

        [DllImport("user32.dll")]
        internal static extern int FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        internal static extern int GetWindowRect(int hwnd, ref RECT lpRect);

        [DllImport("user32.dll")]
        internal static extern int EnumChildWindows(int hWndParent, CallBack lpEnumFunc, ref RECT lParam);

        internal delegate bool EnumChildWindowsCallBack(IntPtr hwnd, IntPtr lParam);

        [DllImport("user32.dll")]
        internal static extern int EnumChildWindows(IntPtr hWndParent, EnumChildWindowsCallBack lpEnumFunc, IntPtr lParam);

        [DllImport("user32.dll")]
        internal static extern int GetClassName(IntPtr hwnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32.dll")]
        internal static extern IntPtr GetTopWindow(IntPtr hWnd);

        [
            StructLayout(LayoutKind.Sequential),
            Syncfusion.Documentation.DocumentationExclude()
        ]
        internal struct WINDOWPLACEMENT
        {
            internal uint length;
            internal uint flags;
            internal uint showCmd;
            internal POINT ptMinPosition;
            internal POINT ptMaxPosition;
            internal RECT rcNormalPosition;

            internal WINDOWPLACEMENT(uint len, uint flags, uint showcmd, POINT ptmin, POINT ptmax, RECT rcnormal)
            {
                this.length = len;
                this.flags = flags;
                this.showCmd = showcmd;
                this.ptMinPosition = ptmin;
                this.ptMaxPosition = ptmax;
                this.rcNormalPosition = rcnormal;
            }
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool SetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool SetMenu(IntPtr hWnd, IntPtr hMenu);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        internal static extern int GetMenuItemCount(IntPtr hMenu);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        internal static extern IntPtr GetMenu(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool IsMenu(IntPtr hMenu);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        internal static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        internal static extern int GetMenuString(IntPtr hMenu, uint uIDItem, [MarshalAs(UnmanagedType.LPTStr)]string lpString, int maxCount, uint uFlag);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool DrawMenuBar(IntPtr hWnd);

#if false
		internal const uint WM_CBAR_SELANDDISP = 0x8101;	// WM_APP + 101
#endif

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool IsZoomed(IntPtr hWnd);
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        internal delegate bool DrawStateProc(IntPtr hdc, IntPtr lData, IntPtr wData, int cx, int cy);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool DrawState(IntPtr hdc, IntPtr hbr, DrawStateProc lpOutputFunc, IntPtr lData, IntPtr wData,
            int x, int y, int cx, int cy, uint fuFlags);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        internal static extern IntPtr CopyImage(IntPtr hImage, uint uType, int cxDesired, int cyDesired, uint fuflags);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PeekMessage([In] ref MSG msg, IntPtr hwnd, int msgMin, int msgMax, int remove);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool TranslateMessage([In] ref MSG msg);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        internal static extern int DispatchMessage([In] ref MSG msg);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool IsChild(IntPtr hWndParent, IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        internal static extern int DrawText(IntPtr hDC, string lpszString, int nCount, ref RECT lpRect, int nFormat);

        [DllImport("user32.dll")]
        internal static extern int MsgWaitForMultipleObjects(int nCount,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] IntPtr[] pHandles,
            bool bWaitAll, uint dwMilliseconds, int dwWakeMask);
    }

    [
        ComVisibleAttribute(false),
        SuppressUnmanagedCodeSecurityAttribute(),
        Syncfusion.Documentation.DocumentationExclude()
    ]
    internal class GDI
    {
        internal const int R2_NOTXORPEN = 10;   // DPxn

        internal const int PS_SOLID = 0;
        internal const int PS_DASH = 1;
        internal const int PS_DOT = 2;
        internal const int PS_DASHDOT = 3;
        internal const int PS_DASHDOTDOT = 4;

        // GetDCEx() flags
        internal const int DCX_WINDOW = 0x00000001;
        internal const int DCX_CACHE = 0x00000002;
        internal const int DCX_NORESETATTRS = 0x00000004;
        internal const int DCX_CLIPCHILDREN = 0x00000008;
        internal const int DCX_CLIPSIBLINGS = 0x00000010;
        internal const int DCX_PARENTCLIP = 0x00000020;
        internal const int DCX_EXCLUDERGN = 0x00000040;
        internal const int DCX_INTERSECTRGN = 0x00000080;
        internal const int DCX_EXCLUDEUPDATE = 0x00000100;
        internal const int DCX_INTERSECTUPDATE = 0x00000200;
        internal const int DCX_LOCKWINDOWUPDATE = 0x00000400;
        internal const int DCX_VALIDATE = 0x00200000;

        internal static IntPtr CreateDC(string lpszDriver)
        {
            return GDI.CreateDC(lpszDriver, null, null, IntPtr.Zero);
        } // end of method CreateDC

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        extern internal static IntPtr CreateDC(string lpszDriver, string lpszDeviceName, string lpszOutput, IntPtr devMode);

        [DllImport("gdi32", EntryPoint = "DeleteDC", CharSet = CharSet.Auto, ExactSpelling = true)]
        extern internal static bool DeleteDC(IntPtr hDC);

        [DllImport("gdi32")]
        extern internal static int MoveToEx(IntPtr hDC, int X, int Y, ref POINT pt);

        [DllImport("gdi32")]
        extern internal static int LineTo(IntPtr hDC, int X, int Y);

        [DllImport("gdi32.dll")]
        internal static extern IntPtr GetStockObject(int fnObject);

        [DllImport("gdi32")]
        extern internal static int SetTextColor(IntPtr hDC, int crColor);

        [DllImport("gdi32")]
        extern internal static int SetBkColor(IntPtr hDC, int clr);

        [DllImport("gdi32")]
        extern internal static int SetBkMode(IntPtr hdc, int iBkMode);

        [DllImport("gdi32")]
        extern internal static IntPtr CreateSolidBrush(int crColor);

        [DllImport("gdi32")]
        extern internal static IntPtr CreatePen(int fnPenStyle, int nWidth, int crColor);

        [DllImport("gdi32")]
        extern internal static bool DeleteObject(IntPtr hObject);

        [DllImport("gdi32", CharSet = CharSet.Auto, ExactSpelling = true)]
        extern internal static IntPtr CreateBitmap(int nWidth, int nHeight, int nPlanes, int nBitsPerPixel,
            [MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPArray)] short[] lpvBits);

        [DllImport("gdi32", CharSet = CharSet.Auto, ExactSpelling = true)]
        extern internal static IntPtr CreateBrushIndirect(ref LOGBRUSH lb);

        [DllImport("gdi32", CharSet = CharSet.Auto, ExactSpelling = true)]
        extern internal static IntPtr SelectObject(IntPtr hdc, IntPtr hObject);

        [DllImport("gdi32", CharSet = CharSet.Auto)]
        extern internal static bool ExtTextOut(IntPtr hdc, int x, int y, int nOptions, ref RECT lpRect, string s, int nStrLength, int[] lpDx);

        [DllImport("gdi32", CharSet = CharSet.Auto, ExactSpelling = true)]
        extern internal static bool SetViewportOrgEx(IntPtr hdc, int x, int y, out POINT point);

        [DllImport("gdi32", CharSet = CharSet.Auto, ExactSpelling = true)]
        extern internal static bool PatBlt(IntPtr hdc, int x, int y, int nWidth, int nHeight, int dwRop);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        internal extern static int GetDeviceCaps(IntPtr hDC, int nIndex);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        internal static extern int SetROP2(IntPtr hDC, int drawMode);

        [DllImport("gdi32.dll")]
        internal static extern bool RoundRect(IntPtr hdc, int nLeftRect, int nTopRect, int nRightRect, int nBottomRect,
            int nWidth, int nHeight);

        [DllImport("gdi32.dll")]
        internal static extern bool Arc(IntPtr hdc, int nLeftRect, int nTopRect, int nRightRect, int nBottomRect,
            int nXStartArc, int nYStartArc, int nXEndArc, int nYEndArc);

        [DllImport("gdi32.dll")]
        internal static extern bool Polygon(IntPtr hdc, POINT[] lpPoints, int nCount);

        [DllImport("gdi32.dll")]
        internal static extern bool Ellipse(IntPtr hdc, int nLeftRect, int nTopRect, int nRightRect, int nBottomRect);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        internal static extern int Rectangle(IntPtr hDC, int nLeft, int nTop, int nRight, int nBottom);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        internal static extern int Polyline(IntPtr hDC, POINT[] lpPoints, int nPointCount);

        [DllImport("gdi32.dll")]
        internal static extern bool BitBlt(IntPtr hdcDest, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, System.Int32 dwRop);

        [DllImport("Gdi32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool GetTextExtentPoint32(IntPtr hdc, string lpString, int cbString, ref SIZE lpSize);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        internal static extern int PlayEnhMetaFile(IntPtr hDC, IntPtr hEnhMetafile, ref RECT rect);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        internal static extern IntPtr GetWindowDC(IntPtr hWnd);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        internal static extern IntPtr CreateCompatibleDC(IntPtr hDC);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        internal static extern IntPtr CreateCompatibleBitmap(IntPtr hDC, int width, int height);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, ExactSpelling = false, CallingConvention = CallingConvention.Winapi)]
        internal static extern IntPtr CreateEnhMetaFile(IntPtr hdcRef, string lpFilename, ref RECT lprect, string lpDescription);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        internal static extern IntPtr CloseEnhMetaFile(IntPtr hdc);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        internal static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);
    }

#if false
	[
		ComVisibleAttribute(false),
		SuppressUnmanagedCodeSecurityAttribute(),
		Syncfusion.Documentation.DocumentationExclude()
	]
	internal class Themes
	{
		[DllImportAttribute("uxtheme.dll", EntryPoint="OpenThemeData", SetLastError=true)]
		internal static extern IntPtr OpenThemeData(IntPtr hwnd, [MarshalAs(UnmanagedType.LPWStr)] string pszClassList);

		[DllImport("uxtheme.dll", CharSet=CharSet.Auto, CallingConvention=CallingConvention.Winapi)] 
		internal static extern IntPtr CloseThemeData(IntPtr hTheme);

		[DllImport("uxtheme.dll", CharSet=CharSet.Auto, CallingConvention=CallingConvention.Winapi)] 
		internal static extern bool IsThemeActive();

		[DllImport("uxtheme.dll", CharSet=CharSet.Auto, CallingConvention=CallingConvention.Winapi)] 
		internal static extern int DrawThemeBackground(IntPtr hTheme, IntPtr hdc, 
			Int32 iPartId, Int32 iStateId, ref RECT rect, ref RECT clipRect);

		[DllImport("uxtheme.dll", CharSet=CharSet.Auto, CallingConvention=CallingConvention.Winapi)] 
		internal static extern int DrawThemeText(IntPtr hTheme, IntPtr hdc, Int32 iPartId, 
			Int32 iStateId, string pszText, Int32 iCharCount, uint dwTextFlags, 
			uint dwTextFlags2, [MarshalAs(UnmanagedType.Struct)]ref RECT rect);

		[DllImportAttribute("uxtheme.dll", EntryPoint="GetThemeBackgroundContentRect")]
		internal static extern void GetThemeBackgroundContentRect(int hTheme, IntPtr hdc, int iPartId, int iStateId, ref RECT pBoundingRect, ref RECT pContentRect);

		[DllImportAttribute("uxtheme.dll", EntryPoint="GetThemeBackgroundExtent")]
		internal static extern void GetThemeBackgroundExtent(int hTheme, IntPtr hdc, int iPartId, int iStateId, ref RECT pContentRect, ref RECT pExtentRect);

		// Assumes you will always pass NULL for the 5th param.
		[DllImport("uxtheme.dll")]
		internal static extern uint GetThemePartSize(IntPtr hTheme, IntPtr hdc, int iPartId,
			int iStateId, IntPtr prc, int sizeType,	out SIZE psz);

		[DllImport("uxtheme.dll", CharSet=CharSet.Auto, CallingConvention=CallingConvention.Winapi)] 
		internal static extern uint GetThemeTextExtent(IntPtr hTheme, IntPtr hdc, 
			Int32 iPartId, Int32 iStateId, String pszText, Int32 iCharCount, 
			uint dwTextFlags,  [MarshalAs(UnmanagedType.Struct)]ref RECT pBoundingRect, 
			out RECT pExtentRect);

		[DllImport("uxtheme.dll", CharSet=CharSet.Auto, CallingConvention=CallingConvention.Winapi)] 
		internal static extern ulong GetThemeTextMetrics(IntPtr hTheme,  IntPtr hdc, 
			Int32 iPartId, Int32 iStateId, [In, Out]ref TEXTMETRIC ptm);

		[DllImport("uxtheme.dll", CharSet=CharSet.Auto, CallingConvention=CallingConvention.Winapi)] 
		internal static extern ulong GetThemeBackgroundRegion(IntPtr hTheme,  
			Int32 iPartId, Int32 iStateId, RECT pRect, [In, Out]ref IntPtr pRegion);

		[DllImport("uxtheme.dll", CharSet=CharSet.Auto, CallingConvention=CallingConvention.Winapi)] 
		internal static extern ulong HitTestThemeBackground(IntPtr hTheme,  IntPtr hdc, Int32 iPartId, 
			Int32 iStateId, ulong dwOptions, RECT pRect,  IntPtr hrgn, 
			POINT ptTest, [In, Out]ref uint wHitTestCode);

		[DllImport("uxtheme.dll", CharSet=CharSet.Auto, CallingConvention=CallingConvention.Winapi)] 
		internal static extern ulong DrawThemeLine(IntPtr hTheme, IntPtr hdc, Int32 iStateId, 
			RECT pRect, ulong dwDtlFlags);

		[DllImport("uxtheme.dll", CharSet=CharSet.Auto, CallingConvention=CallingConvention.Winapi)] 
		internal static extern ulong DrawThemeEdge(IntPtr hTheme, IntPtr hdc, Int32 iPartId, Int32 iStateId, 
			RECT pDestRect, uint uEdge, uint uFlags,  [In, Out]ref RECT contentRect);

		[DllImport("uxtheme.dll", CharSet=CharSet.Auto, CallingConvention=CallingConvention.Winapi)] 
		internal static extern ulong DrawThemeBorder(IntPtr hTheme, IntPtr hdc, Int32 iStateId, 
			RECT pRect);

		[DllImport("uxtheme.dll", CharSet=CharSet.Auto, CallingConvention=CallingConvention.Winapi)] 
		internal static extern ulong DrawThemeIcon(IntPtr hTheme, IntPtr hdc, Int32 iPartId, 
			Int32 iStateId, RECT pRect, IntPtr himl, Int32 iImageIndex);

		[DllImport("uxtheme.dll", CharSet=CharSet.Auto, CallingConvention=CallingConvention.Winapi)] 
		internal static extern bool IsThemePartDefined(IntPtr hTheme, Int32 iPartId, 
			Int32 iStateId);

		[DllImport("uxtheme.dll", CharSet=CharSet.Auto, CallingConvention=CallingConvention.Winapi)] 
		internal static extern bool IsThemeBackgroundPartiallyTransparent(IntPtr hTheme, 
			Int32 iPartId, Int32 iStateId);

		[DllImport("uxtheme.dll", CharSet=CharSet.Auto, CallingConvention=CallingConvention.Winapi)] 
		internal static extern int GetThemeColor(IntPtr hTheme, Int32 iPartId, 
			Int32 iStateId, Int32 iPropId, [In, Out]ref ulong color);

		[DllImport("uxtheme.dll", CharSet=CharSet.Auto, CallingConvention=CallingConvention.Winapi)] 
		internal static extern ulong GetThemeMetric(IntPtr hTheme, Int32 iPartId, 
			Int32 iStateId, Int32 iPropId, [In, Out]ref Int32 iVal);

		[DllImport("uxtheme.dll", CharSet=CharSet.Auto, CallingConvention=CallingConvention.Winapi)] 
		internal static extern ulong GetThemeString(IntPtr hTheme, Int32 iPartId, 
			Int32 iStateId, Int32 iPropId, [In, Out]ref string pszBuff, Int32 cchMaxBuffChars);

		[DllImport("uxtheme.dll", CharSet=CharSet.Auto, CallingConvention=CallingConvention.Winapi)] 
		internal static extern ulong GetThemeBool(IntPtr hTheme, Int32 iPartId, 
			Int32 iStateId, Int32 iPropId, [In, Out]ref bool fVal);

		[DllImport("uxtheme.dll", CharSet=CharSet.Auto, CallingConvention=CallingConvention.Winapi)] 
		internal static extern ulong GetThemeInt(IntPtr hTheme, Int32 iPartId, 
			Int32 iStateId, Int32 iPropId, [In, Out]ref Int32 iVal);

		[DllImport("uxtheme.dll", CharSet=CharSet.Auto, CallingConvention=CallingConvention.Winapi)] 
		internal static extern ulong GetThemeEnumValue(IntPtr hTheme, Int32 iPartId, 
			Int32 iStateId, Int32 iPropId, [In, Out]ref Int32 iVal);

		[DllImport("uxtheme.dll", CharSet=CharSet.Auto, CallingConvention=CallingConvention.Winapi)] 
		internal static extern ulong GetThemePosition(IntPtr hTheme, Int32 iPartId, 
			Int32 iStateId, Int32 iPropId, [In, Out]ref POINT point);

		[DllImport("uxtheme.dll", CharSet=CharSet.Auto, CallingConvention=CallingConvention.Winapi)] 
		internal static extern ulong GetThemeFont(IntPtr hTheme, Int32 iPartId, 
			Int32 iStateId, Int32 iPropId, [Out]out LOGFONT font);

		[DllImport("uxtheme.dll", CharSet=CharSet.Auto, CallingConvention=CallingConvention.Winapi)] 
		internal static extern ulong GetThemeRect(IntPtr hTheme, Int32 iPartId, 
			Int32 iStateId, Int32 iPropId, [In, Out]ref RECT pRect);

		[DllImport("uxtheme.dll", CharSet=CharSet.Auto, CallingConvention=CallingConvention.Winapi)] 
		internal static extern ulong GetThemeMargins(IntPtr hTheme, Int32 iPartId, 
			Int32 iStateId, Int32 iPropId, [Out] out MARGINS margins);

		[DllImport("uxtheme.dll", CharSet=CharSet.Auto, CallingConvention=CallingConvention.Winapi)] 
		internal static extern ulong GetThemeIntList(IntPtr hTheme, Int32 iPartId, 
			Int32 iStateId, Int32 iPropId, [In, Out]ref INTLIST intList);

		[DllImport("uxtheme.dll", CharSet=CharSet.Auto, CallingConvention=CallingConvention.Winapi)] 
		internal static extern ulong GetThemePropertyOrigin(IntPtr hTheme, Int32 iPartId, 
			Int32 iStateId, Int32 iPropId, [In, Out]ref int origin);

		[DllImport("uxtheme.dll", CharSet=CharSet.Auto, CallingConvention=CallingConvention.Winapi)] 
		internal static extern ulong SetWindowTheme(IntPtr hwnd, string pszSubAppName, 
			string pszSubIdList);

		[DllImport("uxtheme.dll", CharSet=CharSet.Auto, CallingConvention=CallingConvention.Winapi)] 
		internal static extern ulong GetThemeFilename(IntPtr hTheme, Int32 iPartId, 
			Int32 iStateId, Int32 iPropId, [In, Out]ref string pszThemeFileName, Int32 cchMaxBuffChars);

		[DllImport("uxtheme.dll", CharSet=CharSet.Auto, CallingConvention=CallingConvention.Winapi)] 
		internal static extern ulong GetThemeSysColor(IntPtr hTheme, Int32 iColorId);

		[DllImport("uxtheme.dll", CharSet=CharSet.Auto, CallingConvention=CallingConvention.Winapi)] 
		internal static extern IntPtr GetThemeSysColorBrush(IntPtr hTheme, Int32 iColorId);

		[DllImport("uxtheme.dll", CharSet=CharSet.Auto, CallingConvention=CallingConvention.Winapi)] 
		internal static extern Int32 GetThemeSysSize(IntPtr hTheme, Int32 iSizeId);

		[DllImport("uxtheme.dll", CharSet=CharSet.Auto, CallingConvention=CallingConvention.Winapi)] 
		internal static extern bool GetThemeSysBool(IntPtr hTheme, Int32 iBoolId);

		[DllImport("uxtheme.dll", CharSet=CharSet.Auto, CallingConvention=CallingConvention.Winapi)] 
		internal static extern ulong GetThemeSysFont(IntPtr hTheme, Int32 iFontId, [In, Out]ref LOGFONT lf);

		[DllImport("uxtheme.dll", CharSet=CharSet.Auto, CallingConvention=CallingConvention.Winapi)] 
		internal static extern ulong GetThemeSysString(IntPtr hTheme, Int32 iStringId, 
			[In, Out]ref string pszStringBuff, Int32 cchMaxStringChars);

		[DllImport("uxtheme.dll", CharSet=CharSet.Auto, CallingConvention=CallingConvention.Winapi)] 
		internal static extern ulong GetThemeSysInt(IntPtr hTheme, Int32 iIntId, [In, Out]ref Int32 iValue);

		[DllImport("uxtheme.dll", CharSet=CharSet.Auto, CallingConvention=CallingConvention.Winapi)] 
		internal static extern bool IsAppThemed();

		[DllImport("uxtheme.dll", CharSet=CharSet.Auto, CallingConvention=CallingConvention.Winapi)] 
		internal static extern IntPtr GetWindowTheme(IntPtr hwnd);

		[DllImport("uxtheme.dll", CharSet=CharSet.Auto, CallingConvention=CallingConvention.Winapi)] 
		internal static extern ulong EnableThemeDialogTexture(IntPtr hwnd, bool fEnable);

		[DllImport("uxtheme.dll", CharSet=CharSet.Auto, CallingConvention=CallingConvention.Winapi)] 
		internal static extern bool IsThemeDialogTextureEnabled(IntPtr hwnd);

		[DllImport("uxtheme.dll", CharSet=CharSet.Auto, CallingConvention=CallingConvention.Winapi)] 
		internal static extern ulong GetThemeAppProperties();

		[DllImport("uxtheme.dll", CharSet=CharSet.Auto, CallingConvention=CallingConvention.Winapi)] 
		internal static extern void SetThemeAppProperties(ulong dwFlags);

		[DllImport("uxtheme.dll", CharSet=CharSet.Auto, CallingConvention=CallingConvention.Winapi)] 
		internal static extern ulong GetCurrentThemeName(
			[In, Out]ref string pszThemeFileName, Int32 cchMaxNameChars, 
			[In, Out]ref  string pszColorBuff, Int32 cchMaxColorChars,
			[In, Out]ref  string pszSizeBuff, Int32 cchMaxSizeChars);

		[DllImport("uxtheme.dll", CharSet=CharSet.Auto, CallingConvention=CallingConvention.Winapi)] 
		internal static extern ulong GetThemeDocumentationProperty(string pszThemeName,
			string pszPropertyName, [In, Out]ref string pszValueBuff, Int32 cchMaxValChars);

		[DllImport("uxtheme.dll", CharSet=CharSet.Auto, CallingConvention=CallingConvention.Winapi)] 
		internal static extern ulong GetThemeLastErrorContext([In, Out]ref THEME_ERROR_CONTEXT context);

		[DllImport("uxtheme.dll", CharSet=CharSet.Auto, CallingConvention=CallingConvention.Winapi)] 
		internal static extern ulong FormatThemeMessage(ulong dwLanguageId, 
			THEME_ERROR_CONTEXT context, [In, Out]ref string pszMessageBuff, 
			Int32 cchMaxMessageChars);

		[DllImport("uxtheme.dll", CharSet=CharSet.Auto, CallingConvention=CallingConvention.Winapi)] 
		internal static extern ulong GetThemeImageFromParent(IntPtr hwnd, IntPtr hdc, RECT rc);

		[DllImport("uxtheme.dll", CharSet=CharSet.Auto, CallingConvention=CallingConvention.Winapi)] 
		internal static extern IntPtr DrawThemeParentBackground(IntPtr hwnd, IntPtr hdc, ref RECT prc);
	}
#endif
}
