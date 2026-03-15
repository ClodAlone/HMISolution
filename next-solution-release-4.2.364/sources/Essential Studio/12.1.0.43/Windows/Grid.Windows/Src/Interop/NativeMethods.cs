//-------------------------------------------------------------------------------------------------
// <copyright file="NativeMethods.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Security;
using System.Windows.Forms;

namespace Syncfusion.Windows.Forms.Grid
{
    [ComVisibleAttribute(false),
    SuppressUnmanagedCodeSecurityAttribute()]
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class UnsafeNativeMethods
    {
        // Methods
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        extern public static int ClientToScreen(IntPtr hWnd, NativeMethods.POINT pt);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        extern public static int ScreenToClient(IntPtr hWnd, NativeMethods.POINT pt);
    }

    [ComVisibleAttribute(false),
    SuppressUnmanagedCodeSecurityAttribute]
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class NativeMethods
    {
        public static IntPtr NullIntPtr = (IntPtr)0;
        public static IntPtr InvalidIntPtr = (IntPtr)(-1);

        private NativeMethods()
        {
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct RECT
        {
            public RECT(Rectangle rect)
            {
                this.bottom = rect.Bottom;
                this.left = rect.Left;
                this.right = rect.Right;
                this.top = rect.Top;
            }

            public RECT(int left, int top, int right, int bottom)
            {
                this.bottom = bottom;
                this.left = left;
                this.right = right;
                this.top = top;
            }

            public static RECT FromXYWH(int x, int y, int width, int height)
            {
                return new RECT(x, y, x + width, y + height);
            }

            internal int Width
            {
                get
                {
                    return this.right - this.left;
                }
            }

            internal int Height
            {
                get
                {
                    return this.bottom - this.top;
                }
            }

            public int left;
            public int top;
            public int right;
            public int bottom;

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

        [ComVisibleAttribute(true),
            StructLayout(LayoutKind.Sequential)]
        [Syncfusion.Documentation.DocumentationExclude()]
        internal class TOOLINFO_T
        {
            // Fields
            public int cbSize = 0; // = Marshal.SizeOf(typeof(TOOLINFO_T));
            public int uFlags = 0;
            public IntPtr hWnd = IntPtr.Zero;
            public int uId = 0;
            public RECT rect = new RECT();
            public IntPtr hinst = IntPtr.Zero;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpszText = null;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal class IMAGELISTDRAWPARAMS
        {
            public int cbSize = Marshal.SizeOf(typeof(IMAGELISTDRAWPARAMS));
            public IntPtr himl = IntPtr.Zero;
            public int i = 0;
            public IntPtr hdcDst = IntPtr.Zero;
            public int x = 0;
            public int y = 0;
            public int cx = 0;
            public int cy = 0;
            public int xBitmap = 0;
            public int yBitmap = 0;
            public int rgbBk = 0;
            public int rgbFg = 0;
            public int fStyle = 0;
            public int dwRop = 0;
        }

        [DllImport("comctl32.dll", CallingConvention = CallingConvention.Winapi)]
        public static extern IntPtr ImageList_Create(int cx, int cy, int flags, int cInitial, int cGrow);

        [DllImport("comctl32.dll", CallingConvention = CallingConvention.Winapi)]
        public static extern bool ImageList_Destroy(IntPtr himl);

        [DllImport("comctl32.dll", CallingConvention = CallingConvention.Winapi)]
        public static extern int ImageList_GetImageCount(IntPtr himl);

        [DllImport("comctl32.dll", CallingConvention = CallingConvention.Winapi)]
        public static extern int ImageList_Add(IntPtr himl, IntPtr hbmImage, IntPtr hbmMask);

        [DllImport("comctl32.dll", CallingConvention = CallingConvention.Winapi)]
        public static extern int ImageList_ReplaceIcon(IntPtr himl, int index, IntPtr hicon);

        [DllImport("comctl32.dll", CallingConvention = CallingConvention.Winapi)]
        public static extern int ImageList_SetBkColor(IntPtr himl, int clrBk);

        [DllImport("comctl32.dll", CallingConvention = CallingConvention.Winapi)]
        public static extern bool ImageList_Draw(IntPtr himl, int i, IntPtr hdcDst, int x, int y, int fStyle);

        [DllImport("comctl32.dll", CallingConvention = CallingConvention.Winapi)]
        public static extern bool ImageList_Replace(IntPtr himl, int i, IntPtr hbmImage, IntPtr hbmMask);

        [DllImport("comctl32.dll", CallingConvention = CallingConvention.Winapi)]
        public static extern bool ImageList_DrawEx(IntPtr himl, int i, IntPtr hdcDst, int x, int y, int dx, int dy, uint rgbBk, uint rgbFg, uint fStyle);

        [DllImport("comctl32.dll", CallingConvention = CallingConvention.Winapi)]
        public static extern bool ImageList_DrawIndirect(NativeMethods.IMAGELISTDRAWPARAMS pimldp);

        [DllImport("comctl32.dll", CallingConvention = CallingConvention.Winapi)]
        public static extern bool ImageList_Remove(IntPtr himl, int i);

        ////        [DllImport("comctl32.dll", CallingConvention=CallingConvention.Winapi)] 
        ////        public static extern bool ImageList_GetImageInfo(IntPtr himl, int i, NativeMethods.IMAGEINFO pImageInfo); 
        ////        
        ////        [DllImport("comctl32.dll", CallingConvention=CallingConvention.Winapi)] 
        ////        public static extern IntPtr ImageList_Read(UnsafeNativeMethods.IStream pstm); 
        ////        
        ////        [DllImport("comctl32.dll", CallingConvention=CallingConvention.Winapi)] 
        ////        public static extern bool ImageList_Write(IntPtr himl, UnsafeNativeMethods.IStream pstm); 

        [ComVisibleAttribute(true),
            StructLayout(LayoutKind.Sequential, Pack = 1)]
        [Syncfusion.Documentation.DocumentationExclude()]
        internal class INITCOMMONCONTROLSEX
        {
            public int dwSize = Marshal.SizeOf(typeof(INITCOMMONCONTROLSEX));
            public int dwICC = 0;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct POINT
        {
            public int X;
            public int Y;

            public POINT(int x, int y)
            {
                this.X = x;
                this.Y = y;
            }
        }

        public const Int32 HTCLIENT = 1; // 0x0001 

        public const Int32 SM_CXVSCROLL = 2;
        public const Int32 SM_CYHSCROLL = 3;

        [DllImport("user32", CharSet = CharSet.Auto, ExactSpelling = true)]
        extern public static bool UpdateWindow(IntPtr hWnd);

        [DllImport("user32", CharSet = CharSet.Auto, ExactSpelling = true)]
        extern public static bool EnableWindow(IntPtr hWnd, bool enable);

        [DllImport("user32", CharSet = CharSet.Auto, ExactSpelling = true)]
        extern public static int GetDlgItemInt(IntPtr hWnd, int nIDDlgItem, bool[] err, bool signed);

        [DllImport("user32", CharSet = CharSet.Auto, ExactSpelling = true)]
        extern public static bool SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int y, int cx, int cy, int flags);

        [DllImport("user32", CharSet = CharSet.Auto)]
        extern public static int SendDlgItemMessage(IntPtr hDlg, int nIDDlgItem, int Msg, int wParam, int lParam);

        [DllImport("user32", CharSet = CharSet.Auto)]
        extern public static int SendDlgItemMessage(IntPtr hDlg, int nIDDlgItem, int Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        extern public static IntPtr PostMessage(IntPtr hwnd, int msg, int wparam, int lparam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        public static extern bool PeekMessage(ref MSG msg, IntPtr hwnd, int msgMin, int msgMax, int remove);

        [DllImport("user32", CharSet = CharSet.Auto, ExactSpelling = true)]
        extern public static IntPtr GetDlgItem(IntPtr hWnd, int nIDDlgItem);

        [DllImport("user32", CharSet = CharSet.Auto, ExactSpelling = true)]
        extern public static IntPtr GetForegroundWindow();

        [DllImport("user32", CharSet = CharSet.Auto, ExactSpelling = true)]
        extern public static IntPtr GetParent(IntPtr hWnd);

        [DllImport("user32", CharSet = CharSet.Auto, ExactSpelling = true)]
        extern public static int MsgWaitForMultipleObjects(int nCount, int pHandles, bool fWaitAll, int dwMilliseconds, int dwWakeMask);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        extern public static IntPtr GetModuleHandle(string modName);

        [DllImport("user32", CharSet = CharSet.Auto, ExactSpelling = true)]
        extern public static IntPtr CallNextHookEx(IntPtr hhook, int code, IntPtr wparam, IntPtr lparam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        extern public static bool UnhookWindowsHookEx(IntPtr hhook);

        [DllImport("user32")]
        public static extern int GetSystemMetrics(Int32 nIndex);

        [DllImport("user32")]
        public static extern bool ScrollWindow(IntPtr hWnd, int nXAmount, int nYAmount, ref RECT rectScrollRegion, ref RECT rectClip);

        [DllImport("comctl32")]
        extern public static void InitCommonControls();

        [DllImport("comctl32")]
        extern public static bool InitCommonControlsEx(INITCOMMONCONTROLSEX icc);

        [DllImport("comctl32")]
        extern public static bool InitializeFlatSB(IntPtr hWnd);

        [DllImport("comctl32")]
        extern public static bool UninitializeFlatSB(IntPtr hWnd);

        [DllImport("comctl32")]
        extern public static int FlatSB_SetScrollInfo(IntPtr hWnd, int fnBar, ref SCROLLINFO si, bool redraw);

        [DllImport("comctl32")]
        extern public static bool FlatSB_GetScrollInfo(IntPtr hWnd, int fnBar, ref SCROLLINFO si);

        [DllImport("comctl32")]
        extern public static bool FlatSB_SetScrollProp(IntPtr hWnd, int index, int newValue, bool fRedraw);

        [DllImport("comctl32")]
        extern public static bool FlatSB_GetScrollProp(IntPtr hWnd, int index, ref int value);

        [DllImport("comctl32")]
        extern public static bool FlatSB_EnableScrollBar(IntPtr hWnd, int wSBflags, int wArrows);

        [DllImport("user32", CharSet = CharSet.Auto)]
        extern public static IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, TOOLINFO_T lParam);

        [DllImport("user32", CharSet = CharSet.Auto)]
        extern public static IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [DllImport("user32", CharSet = CharSet.Auto)]
        extern public static IntPtr SendMessage(IntPtr hWnd, int msg, ref short wparam, ref short lparam);

        [DllImport("user32", CharSet = CharSet.Auto)]
        extern public static IntPtr SendMessage(IntPtr hWnd, int msg, int wparam, string lparam);

        [DllImport("user32", CharSet = CharSet.Auto)]
        extern public static IntPtr SendMessage(IntPtr hWnd, int msg, bool wparam, int lparam);

        /*

        [DllImport("user32", CharSet=CharSet.Auto)]
        extern public static int SetWindowLong(IntPtr hWnd, int nIndex, WndProc wndproc)  ;

        [DllImport("user32", CharSet=CharSet.Auto)]
        extern public static int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong)  ;
      
        public delegate int HookProc(int nCode, int wParam, int lParam);

        [DllImport("user32", CharSet=CharSet.Auto)]
        extern public static int SetWindowsHookEx(int hookid, HookProc pfnhook, int hinst, int threadid)  ;

        [DllImport("gdi32", CharSet=CharSet.Auto, ExactSpelling=true)]
            extern public static int CreateBitmap(int nWidth, int nHeight, int nPlanes, int nBitsPerPixel, 
            [MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPArray)] short[] lpvBits);

        [DllImport("gdi32", CharSet=CharSet.Auto, ExactSpelling=true)]
            extern public static int CreateBrushIndirect(ref LOGBRUSH lb)  ;

        [DllImport("gdi32", CharSet=CharSet.Auto, ExactSpelling=true)]
            extern public static int SetTextColor(int hDC, int crColor)  ;

        [DllImport("gdi32", CharSet=CharSet.Auto, ExactSpelling=true)]
            extern public static int SetBkColor(int hDC, int clr)  ;

        [DllImport("gdi32", CharSet=CharSet.Auto, ExactSpelling=true)]
            extern public static bool DeleteObject(int hObject)  ;

        [DllImport("gdi32", CharSet=CharSet.Auto, ExactSpelling=true)]
            extern public static int SelectObject(int hDC, int hObject)  ;

        [DllImport("user32", CharSet=CharSet.Auto, ExactSpelling=true)]
            extern public static int FillRect(int hDC, ref RECT rect, int hBrush)  ;

        [DllImport("gdi32", CharSet=CharSet.Auto)]
            extern public static bool ExtTextOut(int hDC, int x, int y, int nOptions, ref RECT lpRect, string s, int nStrLength, int[] lpDx)  ;

        [DllImport("gdi32", CharSet=CharSet.Auto, ExactSpelling=true)]
            extern public static bool SetViewportOrgEx(int hDC, int x, int y, out POINT point)  ;

        [DllImport("gdi32", CharSet=CharSet.Auto, ExactSpelling=true)]
            extern public static bool PatBlt(int hDC, int x, int y, int nWidth, int nHeight, int dwRop)  ;


        //[DllImport("user32", CharSet=CharSet.Auto)]
        //extern public static int SendMessage(IntPtr hWnd, int Msg, int wParam, ListViewCompareCallback pfnCompare)  ;

        [DllImport("user32", CharSet=CharSet.Auto)]
        public static extern int SendMessage(IntPtr hWnd, int Msg, [MarshalAs(int wParam)] object wParam , int lParam);
        
        [DllImport("user32", CharSet=CharSet.Auto)]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, [MarshalAs(int lParam)] object lParam);        
        
        
        //public static int SendMessage(IntPtr hWnd, int Msg, bool wParam, int lParam)  { } 
        //public static int SendMessage(IntPtr hWnd, int Msg, object wParam, ref RECT lParam)  { } 

        [DllImport("user32", CharSet=CharSet.Auto)]
        extern public static int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam)  ;

        [DllImport("user32", CharSet=CharSet.Auto)]
        extern public static int SendMessage(IntPtr hWnd, int Msg, ref short wParam, ref short lParam)  ;

        // public static int SendMessage(IntPtr hWnd, int Msg, object wParam, object lParam)  { } 
        [DllImport("user32", CharSet=CharSet.Auto)]
        extern public static int SendMessage(IntPtr hWnd, int Msg, int wParam, ref RECT lParam)  ;

        [DllImport("user32", CharSet=CharSet.Ansi, ExactSpelling=true)]
        extern public static int SendMessageA(IntPtr hWnd, int Msg, int wParam, int lParam)  ;

        [DllImport("user32", CharSet=CharSet.Ansi, ExactSpelling=true)]
        extern public static int SendMessageA(IntPtr hWnd, int Msg, int wparam, string lparam)  ;

        [DllImport("user32", CharSet=CharSet.Auto)]
        extern public static int SendMessageTimeout(IntPtr hWnd, int Msg, int wParam, int lParam, int flags, int timeout, int[] pdwResult)  ;

        [DllImport("user32", CharSet=CharSet.Unicode, ExactSpelling=true)]
        extern public static int SendMessageW(IntPtr hWnd, int Msg, int wParam, int lParam)  ;
*/
        public const int HWND_TOP = 0;
        public const int HWND_BOTTOM = 1; // 0x0001 
        public const int HWND_TOPMOST = -1; // 0xffff 
        public const int HWND_NOTOPMOST = -2; // 0xfffe 

        public const int WS_HSCROLL = 0x100000;
        public const int WS_VSCROLL = 0x200000;
        public const int WM_REFLECT = 0x2000;

        public const int EM_GETSEL = 176 /*0x00B0*/;
        public const int EM_SETSEL = 177 /*0x00B1*/;
        public const int EM_GETRECT = 178 /*0x00B2*/;
        public const int EM_SETRECT = 179 /*0x00B3*/;
        public const int EM_SETRECTNP = 180 /*0x00B4*/;
        public const int EM_SCROLL = 181 /*0x00B5*/;
        public const int EM_LINESCROLL = 182 /*0x00B6*/;
        public const int EM_SCROLLCARET = 183 /*0x00B7*/;
        public const int EM_GETMODIFY = 184 /*0x00B8*/;
        public const int EM_SETMODIFY = 185 /*0x00B9*/;
        public const int EM_GETLINECOUNT = 186 /*0x00BA*/;
        public const int EM_LINEINDEX = 187 /*0x00BB*/;
        public const int EM_SETHANDLE = 188 /*0x00BC*/;
        public const int EM_GETHANDLE = 189 /*0x00BD*/;
        public const int EM_GETTHUMB = 190 /*0x00BE*/;
        public const int EM_LINELENGTH = 193 /*0x00C1*/;
        public const int EM_REPLACESEL = 194 /*0x00C2*/;
        public const int EM_GETLINE = 196 /*0x00C4*/;
        public const int EM_LIMITTEXT = 197 /*0x00C5*/;
        public const int EM_CANUNDO = 198 /*0x00C6*/;
        public const int EM_UNDO = 199 /*0x00C7*/;
        public const int EM_FMTLINES = 200 /*0x00C8*/;
        public const int EM_LINEFROMCHAR = 201 /*0x00C9*/;
        public const int EM_SETTABSTOPS = 203 /*0x00CB*/;
        public const int EM_SETPASSWORDCHAR = 204 /*0x00CC*/;
        public const int EM_EMPTYUNDOBUFFER = 205 /*0x00CD*/;
        public const int EM_GETFIRSTVISIBLELINE = 206 /*0x00CE*/;
        public const int EM_SETREADONLY = 207 /*0x00CF*/;
        public const int EM_SETWORDBREAKPROC = 208 /*0x00D0*/;
        public const int EM_GETWORDBREAKPROC = 209 /*0x00D1*/;
        public const int EM_GETPASSWORDCHAR = 210 /*0x00D2*/;
        public const int EM_SETMARGINS = 211 /*0x00D3*/;
        public const int EM_GETMARGINS = 212 /*0x00D4*/;
        public const int EM_SETLIMITTEXT = 197 /*0x00C5*/;
        public const int EM_GETLIMITTEXT = 213 /*0x00D5*/;
        public const int EM_POSFROMCHAR = 214 /*0x00D6*/;
        public const int EM_CHARFROMPOS = 215 /*0x00D7*/;
        public const int EC_LEFTMARGIN = 1 /*0x0001*/;
        public const int EC_RIGHTMARGIN = 2 /*0x0002*/;
        public const int EC_USEFONTINFO = 65535 /*0xFFFF*/;

        public const int WM_ACTIVATE = 6; // 0x0006 
        public const int WM_ACTIVATEAPP = 28; // 0x001c 
        public const int WM_AFXFIRST = 864; // 0x0360 
        public const int WM_AFXLAST = 895; // 0x037f 
        public const int WM_APP = 32768; // 0x8000 
        public const int WM_ASKCBFORMATNAME = 780; // 0x030c 
        public const int WM_CANCELJOURNAL = 75; // 0x004b 
        public const int WM_CANCELMODE = 31; // 0x001f 
        public const int WM_CAPTURECHANGED = 533; // 0x0215 
        public const int WM_CHANGECBCHAIN = 781; // 0x030d 
        public const int WM_CHANGEUISTATE = 295; // 0x0127 
        public const int WM_CHAR = 258; // 0x0102 
        public const int WM_CHARTOITEM = 47; // 0x002f 
        public const int WM_CHILDACTIVATE = 34; // 0x0022 
        public const int WM_CHOOSEFONT_GETLOGFONT = 1025; // 0x0401 
        public const int WM_CLEAR = 771; // 0x0303 
        public const int WM_CLOSE = 16; // 0x0010 
        public const int WM_COMMAND = 273; // 0x0111 
        public const int WM_COMMNOTIFY = 68; // 0x0044 
        public const int WM_COMPACTING = 65; // 0x0041 
        public const int WM_COMPAREITEM = 57; // 0x0039 
        public const int WM_CONTEXTMENU = 123; // 0x007b 
        public const int WM_COPY = 769; // 0x0301 
        public const int WM_COPYDATA = 74; // 0x004a 
        public const int WM_CREATE = 1; // 0x0001 
        public const int WM_CTLCOLORBTN = 309; // 0x0135 
        public const int WM_CTLCOLORDLG = 310; // 0x0136 
        public const int WM_CTLCOLOREDIT = 307; // 0x0133 
        public const int WM_CTLCOLORLISTBOX = 308; // 0x0134 
        public const int WM_CTLCOLORMSGBOX = 306; // 0x0132 
        public const int WM_CTLCOLORSCROLLBAR = 311; // 0x0137 
        public const int WM_CTLCOLORSTATIC = 312; // 0x0138 
        public const int WM_CUT = 768; // 0x0300 
        public const int WM_DDE_ACK = 996; // 0x03e4 
        public const int WM_DDE_ADVISE = 994; // 0x03e2 
        public const int WM_DDE_DATA = 997; // 0x03e5 
        public const int WM_DDE_EXECUTE = 1000; // 0x03e8 
        public const int WM_DDE_FIRST = 992; // 0x03e0 
        public const int WM_DDE_INITIATE = 992; // 0x03e0 
        public const int WM_DDE_LAST = 1000; // 0x03e8 
        public const int WM_DDE_POKE = 999; // 0x03e7 
        public const int WM_DDE_REQUEST = 998; // 0x03e6 
        public const int WM_DDE_TERMINATE = 993; // 0x03e1 
        public const int WM_DDE_UNADVISE = 995; // 0x03e3 
        public const int WM_DEADCHAR = 259; // 0x0103 
        public const int WM_DELETEITEM = 45; // 0x002d 
        public const int WM_DESTROY = 2; // 0x0002 
        public const int WM_DESTROYCLIPBOARD = 775; // 0x0307 
        public const int WM_DEVICECHANGE = 537; // 0x0219 
        public const int WM_DEVMODECHANGE = 27; // 0x001b 
        public const int WM_DISPLAYCHANGE = 126; // 0x007e 
        public const int WM_DRAWCLIPBOARD = 776; // 0x0308 
        public const int WM_DRAWITEM = 43; // 0x002b 
        public const int WM_DROPFILES = 563; // 0x0233 
        public const int WM_ENABLE = 10; // 0x000a 
        public const int WM_ENDSESSION = 22; // 0x0016 
        public const int WM_ENTERIDLE = 289; // 0x0121 
        public const int WM_ENTERMENULOOP = 529; // 0x0211 
        public const int WM_ENTERSIZEMOVE = 561; // 0x0231 
        public const int WM_ERASEBKGND = 20; // 0x0014 
        public const int WM_EXITMENULOOP = 530; // 0x0212 
        public const int WM_EXITSIZEMOVE = 562; // 0x0232 
        public const int WM_FONTCHANGE = 29; // 0x001d 
        public const int WM_GETDLGCODE = 135; // 0x0087 
        public const int WM_GETFONT = 49; // 0x0031 
        public const int WM_GETHOTKEY = 51; // 0x0033 
        public const int WM_GETICON = 127; // 0x007f 
        public const int WM_GETMINMAXINFO = 36; // 0x0024 
        public const int WM_GETOBJECT = 61; // 0x003d 
        public const int WM_GETTEXT = 13; // 0x000d 
        public const int WM_GETTEXTLENGTH = 14; // 0x000e 
        public const int WM_HANDHELDFIRST = 856; // 0x0358 
        public const int WM_HANDHELDLAST = 863; // 0x035f 
        public const int WM_HELP = 83; // 0x0053 
        public const int WM_HOTKEY = 786; // 0x0312 
        public const int WM_HSCROLL = 276; // 0x0114 
        public const int WM_HSCROLLCLIPBOARD = 782; // 0x030e 
        public const int WM_ICONERASEBKGND = 39; // 0x0027 
        public const int WM_IME_CHAR = 646; // 0x0286 
        public const int WM_IME_COMPOSITION = 271; // 0x010f 
        public const int WM_IME_COMPOSITIONFULL = 644; // 0x0284 
        public const int WM_IME_CONTROL = 643; // 0x0283 
        public const int WM_IME_ENDCOMPOSITION = 270; // 0x010e 
        public const int WM_IME_KEYDOWN = 656; // 0x0290 
        public const int WM_IME_KEYLAST = 271; // 0x010f 
        public const int WM_IME_KEYUP = 657; // 0x0291 
        public const int WM_IME_NOTIFY = 642; // 0x0282 
        public const int WM_IME_SELECT = 645; // 0x0285 
        public const int WM_IME_SETCONTEXT = 641; // 0x0281 
        public const int WM_IME_STARTCOMPOSITION = 269; // 0x010d 
        public const int WM_INITDIALOG = 272; // 0x0110 
        public const int WM_INITMENU = 278; // 0x0116 
        public const int WM_INITMENUPOPUP = 279; // 0x0117 
        public const int WM_INPUTLANGCHANGE = 81; // 0x0051 
        public const int WM_INPUTLANGCHANGEREQUEST = 80; // 0x0050 
        public const int WM_KEYDOWN = 256; // 0x0100 
        public const int WM_KEYFIRST = 256; // 0x0100 
        public const int WM_KEYLAST = 264; // 0x0108 
        public const int WM_KEYUP = 257; // 0x0101 
        public const int WM_KILLFOCUS = 8; // 0x0008 
        public const int WM_LBUTTONDBLCLK = 515; // 0x0203 
        public const int WM_LBUTTONDOWN = 513; // 0x0201 
        public const int WM_LBUTTONUP = 514; // 0x0202 
        public const int WM_MBUTTONDBLCLK = 521; // 0x0209 
        public const int WM_MBUTTONDOWN = 519; // 0x0207 
        public const int WM_MBUTTONUP = 520; // 0x0208 
        public const int WM_MDIACTIVATE = 546; // 0x0222 
        public const int WM_MDICASCADE = 551; // 0x0227 
        public const int WM_MDICREATE = 544; // 0x0220 
        public const int WM_MDIDESTROY = 545; // 0x0221 
        public const int WM_MDIGETACTIVE = 553; // 0x0229 
        public const int WM_MDIICONARRANGE = 552; // 0x0228 
        public const int WM_MDIMAXIMIZE = 549; // 0x0225 
        public const int WM_MDINEXT = 548; // 0x0224 
        public const int WM_MDIREFRESHMENU = 564; // 0x0234 
        public const int WM_MDIRESTORE = 547; // 0x0223 
        public const int WM_MDISETMENU = 560; // 0x0230 
        public const int WM_MDITILE = 550; // 0x0226 
        public const int WM_MEASUREITEM = 44; // 0x002c 
        public const int WM_MENUCHAR = 288; // 0x0120 
        public const int WM_MENUSELECT = 287; // 0x011f 
        public const int WM_MOUSEACTIVATE = 33; // 0x0021 
        ////public readonly static int WM_MOUSEENTER = 0;
        public const int WM_MOUSEFIRST = 512; // 0x0200 
        public const int WM_MOUSEHOVER = 673; // 0x02a1 
        public const int WM_MOUSELAST = 522; // 0x020a 
        public const int WM_MOUSELEAVE = 675; // 0x02a3 
        public const int WM_MOUSEMOVE = 512; // 0x0200 
        public const int WM_MOUSEWHEEL = 522; // 0x020a 
        public const int WM_MOVE = 3; // 0x0003 
        public const int WM_MOVING = 534; // 0x0216 
        public const int WM_NCACTIVATE = 134; // 0x0086 
        public const int WM_NCCALCSIZE = 131; // 0x0083 
        public const int WM_NCCREATE = 129; // 0x0081 
        public const int WM_NCDESTROY = 130; // 0x0082 
        public const int WM_NCHITTEST = 132; // 0x0084 
        public const int WM_NCLBUTTONDBLCLK = 163; // 0x00a3 
        public const int WM_NCLBUTTONDOWN = 161; // 0x00a1 
        public const int WM_NCLBUTTONUP = 162; // 0x00a2 
        public const int WM_NCMBUTTONDBLCLK = 169; // 0x00a9 
        public const int WM_NCMBUTTONDOWN = 167; // 0x00a7 
        public const int WM_NCMBUTTONUP = 168; // 0x00a8 
        public const int WM_NCMOUSEHOVER = 672; // 0x02a0 
        public const int WM_NCMOUSELEAVE = 674; // 0x02a2 
        public const int WM_NCMOUSEMOVE = 160; // 0x00a0 
        public const int WM_NCPAINT = 133; // 0x0085 
        public const int WM_NCRBUTTONDBLCLK = 166; // 0x00a6 
        public const int WM_NCRBUTTONDOWN = 164; // 0x00a4 
        public const int WM_NCRBUTTONUP = 165; // 0x00a5 
        public const int WM_NEXTDLGCTL = 40; // 0x0028 
        public const int WM_NEXTMENU = 531; // 0x0213 
        public const int WM_NOTIFY = 78; // 0x004e 
        public const int WM_NOTIFYFORMAT = 85; // 0x0055 
        public const int WM_NULL = 0;
        public const int WM_PAINT = 15; // 0x000f 
        public const int WM_PAINTCLIPBOARD = 777; // 0x0309 
        public const int WM_PAINTICON = 38; // 0x0026 
        public const int WM_PALETTECHANGED = 785; // 0x0311 
        public const int WM_PALETTEISCHANGING = 784; // 0x0310 
        public const int WM_PARENTNOTIFY = 528; // 0x0210 
        public const int WM_PASTE = 770; // 0x0302 
        public const int WM_PENWINFIRST = 896; // 0x0380 
        public const int WM_PENWINLAST = 911; // 0x038f 
        public const int WM_POWER = 72; // 0x0048 
        public const int WM_POWERBROADCAST = 536; // 0x0218 
        public const int WM_PRINT = 791; // 0x0317 
        public const int WM_PRINTCLIENT = 792; // 0x0318 
        public const int WM_PSD_ENVSTAMPRECT = 1029; // 0x0405 
        public const int WM_PSD_FULLPAGERECT = 1025; // 0x0401 
        public const int WM_PSD_GREEKTEXTRECT = 1028; // 0x0404 
        public const int WM_PSD_MARGINRECT = 1027; // 0x0403 
        public const int WM_PSD_MINMARGINRECT = 1026; // 0x0402 
        public const int WM_PSD_PAGESETUPDLG = 1024; // 0x0400 
        public const int WM_PSD_YAFULLPAGERECT = 1030; // 0x0406 
        public const int WM_QUERYDRAGICON = 55; // 0x0037 
        public const int WM_QUERYENDSESSION = 17; // 0x0011 
        public const int WM_QUERYNEWPALETTE = 783; // 0x030f 
        public const int WM_QUERYOPEN = 19; // 0x0013 
        public const int WM_QUERYUISTATE = 297; // 0x0129 
        public const int WM_QUEUESYNC = 35; // 0x0023 
        public const int WM_QUIT = 18; // 0x0012 
        public const int WM_RBUTTONDBLCLK = 518; // 0x0206 
        public const int WM_RBUTTONDOWN = 516; // 0x0204 
        public const int WM_RBUTTONUP = 517; // 0x0205 
        public const int WM_RENDERALLFORMATS = 774; // 0x0306 
        public const int WM_RENDERFORMAT = 773; // 0x0305 
        public const int WM_SETCURSOR = 32; // 0x0020 
        public const int WM_SETFOCUS = 7; // 0x0007 
        public const int WM_SETFONT = 48; // 0x0030 
        public const int WM_SETHOTKEY = 50; // 0x0032 
        public const int WM_SETICON = 128; // 0x0080 
        public const int WM_SETREDRAW = 11; // 0x000b 
        public const int WM_SETTEXT = 12; // 0x000c 
        public const int WM_SETTINGCHANGE = 26; // 0x001a 
        public const int WM_SHOWWINDOW = 24; // 0x0018 
        public const int WM_SIZE = 5; // 0x0005 
        public const int WM_SIZECLIPBOARD = 779; // 0x030b 
        public const int WM_SIZING = 532; // 0x0214 
        public const int WM_SPOOLERSTATUS = 42; // 0x002a 
        public const int WM_STYLECHANGED = 125; // 0x007d 
        public const int WM_STYLECHANGING = 124; // 0x007c 
        public const int WM_SYSCHAR = 262; // 0x0106 
        public const int WM_SYSCOLORCHANGE = 21; // 0x0015 
        public const int WM_SYSCOMMAND = 274; // 0x0112 
        public const int WM_SYSDEADCHAR = 263; // 0x0107 
        public const int WM_SYSKEYDOWN = 260; // 0x0104 
        public const int WM_SYSKEYUP = 261; // 0x0105 
        public const int WM_TCARD = 82; // 0x0052 
        public const int WM_TIMECHANGE = 30; // 0x001e 
        public const int WM_TIMER = 275; // 0x0113 
        public const int WM_UNDO = 772; // 0x0304 
        public const int WM_UPDATEUISTATE = 296; // 0x0128 
        public const int WM_USER = 1024; // 0x0400 
        public const int WM_USERCHANGED = 84; // 0x0054 
        public const int WM_VKEYTOITEM = 46; // 0x002e 
        public const int WM_VSCROLL = 277; // 0x0115 
        public const int WM_VSCROLLCLIPBOARD = 778; // 0x030a 
        public const int WM_WINDOWPOSCHANGED = 71; // 0x0047 
        public const int WM_WINDOWPOSCHANGING = 70; // 0x0046 
        public const int WM_WININICHANGE = 26; // 0x001a 

        public const int SBM_ENABLE_ARROWS = 228; // 0x00e4 
        public const int SBM_GETPOS = 225; // 0x00e1 
        public const int SBM_GETRANGE = 227; // 0x00e3 
        public const int SBM_GETSCROLLINFO = 234; // 0x00ea 
        public const int SBM_SETPOS = 224; // 0x00e0 
        public const int SBM_SETRANGE = 226; // 0x00e2 
        public const int SBM_SETRANGEREDRAW = 230; // 0x00e6 
        public const int SBM_SETSCROLLINFO = 233; // 0x00e9 

        [DllImport("USER32.dll")]
        extern public static bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, int uFlags);

        [DllImport("user32", CharSet = CharSet.Auto, ExactSpelling = true)]
        extern public static bool ScrollWindowEx(IntPtr hWnd, int nXAmount, int nYAmount, ref RECT rectScrollRegion, ref RECT rectClip, int hrgnUpdate, ref RECT prcUpdate, int flags);

        [DllImport("user32", CharSet = CharSet.Auto, ExactSpelling = true)]
        extern public static bool InvertRect(IntPtr hDC, ref RECT lpRect);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        extern public static IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        extern public static IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, int lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        extern public static IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, IntPtr lParam);

        // FontState
        [DllImport("gdi32", CharSet = CharSet.Auto, ExactSpelling = true)]
        extern public static int GetDeviceCaps(IntPtr hDC, int nIndex);

        [DllImport("user32", EntryPoint = "GetDC", CharSet = CharSet.Auto, ExactSpelling = true)]
        extern public static IntPtr GetDC(IntPtr hWnd);

        [DllImport("user32", EntryPoint = "ReleaseDC", CharSet = CharSet.Auto, ExactSpelling = true)]
        extern public static IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDC);

        // MouseProcHooker    
        [StructLayout(LayoutKind.Sequential)]
        [Syncfusion.Documentation.DocumentationExclude()]
        internal class CPWSTRUCT
        {
            public IntPtr lParam = IntPtr.Zero;
            public int wParam = 0;
            public int message = 0;
            public IntPtr hWnd = IntPtr.Zero;
        }

        [StructLayout(LayoutKind.Sequential)]
        [Syncfusion.Documentation.DocumentationExclude()]
        internal class CPWRETSTRUCT
        {
            public int lResult = 0;
            public int lParam = 0;
            public int wParam = 0;
            public int message = 0;
            public IntPtr hWnd = IntPtr.Zero;
        }

        [StructLayout(LayoutKind.Sequential)]
        [Syncfusion.Documentation.DocumentationExclude()]
        internal class MSG
        {
            public IntPtr hwnd = IntPtr.Zero;
            public int message = 0;
            public int wParam = 0;
            public int lParam = 0;
            public int time = 0;
            public int pt_x = 0;
            public int pt_y = 0;
        }

        [StructLayout(LayoutKind.Sequential)]
        [Syncfusion.Documentation.DocumentationExclude()]
        internal class MOUSEHOOKSTRUCT
        {
            public int pt_x = 0;
            public int pt_y = 0;
            public IntPtr hwnd = IntPtr.Zero;
            public int wHitTestCode = 0;
            public int dwExtraInfo = 0;
        }

        [DllImport("user32", CharSet = CharSet.Auto, ExactSpelling = true)]
        extern public static IntPtr CallNextHookEx(IntPtr hhook, int code, int wparam, int lparam);

        [DllImport("user32", CharSet = CharSet.Auto)]
        extern public static IntPtr SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [Syncfusion.Documentation.DocumentationExclude()]
        public delegate IntPtr HookProc(int nCode, int wParam, int lParam);

        [DllImport("user32", CharSet = CharSet.Auto)]
        extern public static IntPtr SetWindowsHookEx(int hookid, HookProc pfnhook, IntPtr hinst, int threadid);

        [DllImport("user32", CharSet = CharSet.Auto, ExactSpelling = true)]
        extern public static int GetWindowThreadProcessId(IntPtr hWnd, ref int lpdwProcessId);

        // GridReflectScrollBar
        [StructLayout(LayoutKind.Sequential)]
        internal struct SCROLLINFO
        {
            public int cbSize;
            public int fMask;
            public int nMin;
            public int nMax;
            public int nPage;
            public int nPos;
            public int nTrackPos;
        }

        [DllImport("user32", CharSet = CharSet.Auto, ExactSpelling = true)]
        extern public static int SetScrollInfo(IntPtr hWnd, int fnBar, ref SCROLLINFO si, bool redraw);

        [DllImport("user32", CharSet = CharSet.Auto, ExactSpelling = true)]
        extern public static bool GetScrollInfo(IntPtr hWnd, int fnBar, ref SCROLLINFO si);

        [DllImport("gdi32")]
        extern public static int SetTextColor(IntPtr hDC, int crColor);

        [DllImport("gdi32")]
        extern public static int SetBkColor(IntPtr hDC, int clr);

        [DllImport("gdi32")]
        extern public static IntPtr CreateSolidBrush(int crColor);

        [DllImport("gdi32")]
        extern public static bool DeleteObject(IntPtr hObject);

        [DllImport("user32")]
        extern public static IntPtr GetSysColorBrush(int nIndex);

        [StructLayout(LayoutKind.Sequential)]
        internal struct LOGBRUSH
        {
            public int lbStyle;
            public int lbColor;
            public int lbHatch;
        }

        [DllImport("gdi32", CharSet = CharSet.Auto, ExactSpelling = true)]
        extern public static IntPtr CreateBitmap(int nWidth, int nHeight, int nPlanes, int nBitsPerPixel, [MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPArray)] short[] lpvBits);

        [DllImport("gdi32", CharSet = CharSet.Auto, ExactSpelling = true)]
        extern public static IntPtr CreateBrushIndirect(ref LOGBRUSH lb);

        [DllImport("gdi32", CharSet = CharSet.Auto, ExactSpelling = true)]
        extern public static IntPtr SelectObject(IntPtr hdc, IntPtr hObject);

        [DllImport("user32", CharSet = CharSet.Auto, ExactSpelling = true)]
        extern public static int FillRect(IntPtr hdc, ref RECT rect, int hBrush);

        [DllImport("gdi32", CharSet = CharSet.Auto)]
        extern public static bool ExtTextOut(IntPtr hdc, int x, int y, int nOptions, ref RECT lpRect, string s, int nStrLength, int[] lpDx);

        [DllImport("gdi32", CharSet = CharSet.Auto, ExactSpelling = true)]
        extern public static bool SetViewportOrgEx(IntPtr hdc, int x, int y, out POINT point);

        [DllImport("gdi32", CharSet = CharSet.Auto, ExactSpelling = true)]
        extern public static bool PatBlt(IntPtr hdc, int x, int y, int nWidth, int nHeight, int dwRop);

        // DragWindow
        [DllImport("USER32.dll")]
        extern public static bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        // RichTextBox3
        [Syncfusion.Documentation.DocumentationExclude()]
        [StructLayout(LayoutKind.Sequential)]
        internal class FORMATRANGE
        {
            public IntPtr hdc = IntPtr.Zero;
            public IntPtr hdcTarget = IntPtr.Zero;
            public RECT rc = new RECT();
            public RECT rcPage = new RECT();
            public CHARRANGE chrg = new CHARRANGE();
        }

        [Syncfusion.Documentation.DocumentationExclude()]
        [StructLayout(LayoutKind.Sequential)]
        internal struct CHARRANGE
        {
            public int cpMin;
            public int cpMax;
        }

        [Syncfusion.Documentation.DocumentationExclude()]
        [StructLayout(LayoutKind.Sequential)]
        internal struct SIZE
        {
            public int cx;
            public int cy;
        }

        // Methods
        [DllImport("gdi32", CharSet = CharSet.Auto, ExactSpelling = true)]
        extern public static bool GetViewportExtEx(IntPtr hDC, out SIZE s);

        [DllImport("gdi32", CharSet = CharSet.Auto, ExactSpelling = true)]
        extern public static bool GetWindowExtEx(IntPtr hDC, out SIZE s);

        [DllImport("gdi32", CharSet = CharSet.Auto, ExactSpelling = true)]
        extern public static int SetMapMode(IntPtr hDC, int nMapMode);

        [DllImport("gdi32", CharSet = CharSet.Auto, ExactSpelling = true)]
        extern public static bool SetViewportExtEx(IntPtr hDC, int x, int y, out SIZE size);

        [DllImport("gdi32", CharSet = CharSet.Auto, ExactSpelling = true)]
        extern public static bool SetWindowExtEx(IntPtr hDC, int x, int y, out SIZE size);

        [DllImport("user32", CharSet = CharSet.Auto)]
        extern public static IntPtr SendMessage(IntPtr hWnd, int Msg, bool wParam, FORMATRANGE fr);

        [DllImport("user32", CharSet = CharSet.Auto)]
        extern public static IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, ref POINT pt);

        [DllImport("user32", CharSet = CharSet.Auto, ExactSpelling = true)]
        extern public static IntPtr GetDesktopWindow();

        [DllImport("user32", CharSet = CharSet.Auto, ExactSpelling = true)]
        extern public static bool GetUpdateRect(IntPtr hWnd, ref RECT rc, bool fErase);

        [DllImport("gdi32", EntryPoint = "CreateCompatibleDC", CharSet = CharSet.Auto, ExactSpelling = true)]
        extern public static IntPtr CreateCompatibleDC(IntPtr hDC);

        [DllImport("gdi32", EntryPoint = "CreateCompatibleBitmap", CharSet = CharSet.Auto, ExactSpelling = true)]
        extern public static IntPtr CreateCompatibleBitmap(IntPtr hDC, int nWidth, int nHeight);

        [DllImport("gdi32", CharSet = CharSet.Auto, ExactSpelling = true)]
        extern public static bool BitBlt(IntPtr hDC, int x, int y, int nWidth, int nHeight, IntPtr hSrcDC, int xSrc, int ySrc, int dwRop);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        private static extern IntPtr CreateDC(string lpszDriver, string lpszDeviceName, string lpszOutput, IntPtr devMode);

        public static IntPtr CreateDC(string lpszDriver)
        {
            return CreateDC(lpszDriver, null, null, IntPtr.Zero);
        } // end of method CreateDC

        [DllImport("gdi32", EntryPoint = "DeleteDC", CharSet = CharSet.Auto, ExactSpelling = true)]
        extern public static bool DeleteDC(IntPtr hDC);

        [DllImport("user32", CharSet = CharSet.Auto)]
        extern public static int GetWindowLong(IntPtr hWnd, int nIndex);

        // GridCheckBoxCell
        [DllImport("user32", CharSet = CharSet.Auto, ExactSpelling = true)]
        extern public static bool DrawFrameControl(int hDC, ref RECT rect, int type, int state);

        [DllImport("user32", CharSet = CharSet.Auto, ExactSpelling = true)]
        extern public static bool ShowWindow(IntPtr hWnd, int nCmdShow);

        public const int SW_ERASE = 4; // 0x0004 
        public const int SW_INVALIDATE = 2; // 0x0002 
        public const int SW_SCROLLCHILDREN = 1; // 0x0001 

        [Syncfusion.Documentation.DocumentationExclude()]
        public static int HIWORD(int n)
        {
            return (n >> 16) & 0xffff/*=~0x0000*/;
        }

        [Syncfusion.Documentation.DocumentationExclude()]
        public static int LOWORD(int n)
        {
            return n & 0xffff/*=~0x0000*/;
        }

        [Syncfusion.Documentation.DocumentationExclude()]
        public static int LOWORD(IntPtr n)
        {
            return LOWORD((int)n);
        }

        [Syncfusion.Documentation.DocumentationExclude()]
        public static int HIWORD(IntPtr n)
        {
            return HIWORD((int)n);
        }

        [Syncfusion.Documentation.DocumentationExclude()]
        internal static IntPtr MAKELPARAM(int low, int high)
        {
            return (System.IntPtr)(high << 16/*0x10*/
                | (low & 65535/*0xffff*/));
        } // end of method MAKELPARAM

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        extern internal static bool PostMessage(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam);

        internal const int MK_LBUTTON = 0x0001;
        internal const int MK_RBUTTON = 0x0002;
        internal const int MK_SHIFT = 0x0004;
        internal const int MK_CONTROL = 0x0008;
        internal const int MK_MBUTTON = 0x0010;

        /// <summary>
        /// Sends a WM_LBUTTONDOWN and WM_LBUTTONUP message to the control at the specified client coordinates.
        /// </summary>
        /// <param name="c">The target control</param>
        /// <param name="point">The client coordinates where to simulate the click</param>
        public static void FakeLeftMouseClick(Control c, Point point)
        {
            IntPtr lParam = NativeMethods.MAKELPARAM(point.X, point.Y);
            IntPtr wParam = (IntPtr)NativeMethods.MK_LBUTTON; ////(int) e.Button;
            NativeMethods.PostMessage(c.Handle, NativeMethods.WM_LBUTTONDOWN, wParam, lParam);
            NativeMethods.PostMessage(c.Handle, NativeMethods.WM_LBUTTONUP, wParam, lParam);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        public static extern IntPtr GetFocus();

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        public static extern IntPtr SetFocus(IntPtr hWnd);

        [Syncfusion.Documentation.DocumentationExclude()]
        internal static int RGBToCOLORREF(int rgbValue)
        {
            int n0;
            n0 = (rgbValue & 255/*0xff*/) << 16/*0x10*/;
            rgbValue = rgbValue & 16776960/*0xffff00*/;
            rgbValue = rgbValue
                | (rgbValue >> 16/*0x10*/ & 255/*0xff*/);
            rgbValue = rgbValue & 65535/*0xffff*/;
            rgbValue = rgbValue | n0;
            return rgbValue;
        }

        [Syncfusion.Documentation.DocumentationExclude()]
        internal static int COLORREFToRGB(int colorRef)
        {
            int r = colorRef & 255/*0xff*/;
            int g = (colorRef >> 8) & 255/*0xff*/;
            int b = (colorRef >> 16) & 255/*0xff*/;

            int rgb = (r << 16) + (g << 8) + b;

            return rgb;
        }

        [Syncfusion.Documentation.DocumentationExclude()]
        internal static int GetRValue(int rgb)
        {
            return (rgb & 0xff0000) >> 16;
        }

        [Syncfusion.Documentation.DocumentationExclude()]
        internal static int GetGValue(int rgb)
        {
            return (rgb & 0x00ff00) >> 8;
        }

        [Syncfusion.Documentation.DocumentationExclude()]
        internal static int GetBValue(int rgb)
        {
            return rgb & 0x0000ff;
        }

        public const int ETO_OPAQUE = 0x0002;
        public const int ETO_CLIPPED = 0x0004;
        public const int TRANSPARENT = 1;
        public const int OPAQUE = 2;
        public const int PATCOPY = 15728673 /*0xF00021*/;
        public const int PATINVERT = 5898313 /*0x5A0049*/;

        public const int TA_NOUPDATECP = 0;
        public const int TA_UPDATECP = 1;
        public const int TA_LEFT = 0;
        public const int TA_RIGHT = 2;
        public const int TA_CENTER = 6;
        public const int TA_TOP = 0;
        public const int TA_BOTTOM = 8;
        public const int TA_BASELINE = 24;

        public const int DT_LEFT = 0 /*0x0000*/;
        public const int DT_CENTER = 1;
        public const int DT_RIGHT = 2 /*0x0002*/;
        public const int DT_VCENTER = 4 /*0x0004*/;
        public const int DT_BOTTOM = 8 /*0x0004*/;
        public const int DT_WORDBREAK = 16 /*0x0004*/;
        public const int DT_SINGLELINE = 32 /*0x0020*/;
        public const int DT_NOCLIP = 256 /*0x0100*/;
        public const int DT_CALCRECT = 1024 /*0x0400*/;
        public const int DT_NOPREFIX = 2048 /*0x0800*/;
        public const int DT_EDITCONTROL = 8192 /*0x2000*/;
        public const int DT_EXPANDTABS = 64 /*0x0040*/;
        public const int DT_END_ELLIPSIS = 32768 /*0x8000*/;
        public const int DT_RTLREADING = 131072 /*0x20000*/;
        public const int DT_WORD_ELLIPSIS = 0x00040000;
        public const int DT_PATH_ELLIPSIS = 0x00004000;

        [StructLayout(LayoutKind.Sequential)]
        internal struct TEXTMETRICA
        {
            internal int tmHeight;
            internal int tmAscent;
            internal int tmDescent;
            internal int tmInternalLeading;
            internal int tmExternalLeading;
            internal int tmAveCharWidth;
            internal int tmMaxCharWidth;
            internal int tmWeight;
            internal int tmOverhang;
            internal int tmDigitizedAspectX;
            internal int tmDigitizedAspectY;
            internal byte tmFirstChar;
            internal byte tmLastChar;
            internal byte tmDefaultChar;
            internal byte tmBreakChar;
            internal byte tmItalic;
            internal byte tmUnderlined;
            internal byte tmStruckOut;
            internal byte tmPitchAndFamily;
            internal byte tmCharSet;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct TEXTMETRICW
        {
            internal int tmHeight;
            internal int tmAscent;
            internal int tmDescent;
            internal int tmInternalLeading;
            internal int tmExternalLeading;
            internal int tmAveCharWidth;
            internal int tmMaxCharWidth;
            internal int tmWeight;
            internal int tmOverhang;
            internal int tmDigitizedAspectX;
            internal int tmDigitizedAspectY;
            internal ushort tmFirstChar;
            internal ushort tmLastChar;
            internal ushort tmDefaultChar;
            internal ushort tmBreakChar;
            internal byte tmItalic;
            internal byte tmUnderlined;
            internal byte tmStruckOut;
            internal byte tmPitchAndFamily;
            internal byte tmCharSet;
        }

        [StructLayout(LayoutKind.Sequential),
            Syncfusion.Documentation.DocumentationExclude()]
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

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        public static extern int GetTextExtentPoint32(IntPtr hDC, string str, int len, ref NativeMethods.SIZE size);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        public static extern bool GetClipBox(IntPtr hdc, ref NativeMethods.RECT lpRect);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        public static extern int IntersectClipRect(IntPtr hDC, int x1, int y1, int x2, int y2);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        public static extern int SelectClipRgn(IntPtr hDC, IntPtr hRgn);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        public static extern bool GetTextMetricsA(IntPtr hdc, ref NativeMethods.TEXTMETRICA tm);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        public static extern bool GetTextMetricsW(IntPtr hdc, ref NativeMethods.TEXTMETRICW tm);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        public static extern int FillRect(IntPtr hdc, ref NativeMethods.RECT rect, IntPtr hbrush);

        ////        [DllImport("gdi32.dll", CharSet=CharSet.Auto, ExactSpelling=true, CallingConvention=CallingConvention.Winapi)] 
        ////        public static extern int SetBkColor(IntPtr hDC, int clr); 

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        public static extern int SetTextAlign(IntPtr hDC, int align);

        ////        [DllImport("gdi32.dll", CharSet=CharSet.Auto, ExactSpelling=true, CallingConvention=CallingConvention.Winapi)] 
        ////        public static extern int SetTextColor(IntPtr hDC, int clr); 
        ////
        ////        [DllImport("gdi32", CharSet=CharSet.Auto, ExactSpelling=true)]
        ////        extern internal static IntPtr CreateBrushIndirect(ref LOGBRUSH lb)  ;
        ////
        ////        [DllImport("gdi32", CharSet=CharSet.Auto, ExactSpelling=true)]
        ////        extern internal static IntPtr SelectObject(IntPtr hdc, IntPtr hObject)  ;
        ////
        ////        [DllImport("gdi32", CharSet=CharSet.Auto)]
        ////        extern internal static bool ExtTextOut(IntPtr hdc, int x, int y, int nOptions, ref RECT lpRect, string s, int nStrLength, int[] lpDx)  ;
        ////
        ////        [DllImport("gdi32", CharSet=CharSet.Auto, ExactSpelling=true)]
        ////        extern internal static bool PatBlt(IntPtr hdc, int x, int y, int nWidth, int nHeight, int dwRop)  ;
        ////
        ////        [DllImport("gdi32")]
        ////        extern internal static bool DeleteObject(IntPtr hObject)  ;

        [DllImport("gdi32", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr CreateFontIndirectA(ref LOGFONT lplf);

        [DllImport("gdi32", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr CreateFontIndirectW(ref LOGFONT lplf);

        public static IntPtr CreateFontIndirect(ref LOGFONT lplf)
        {
            if (Marshal.SystemDefaultCharSize == 1)
            {
                return CreateFontIndirectA(ref lplf);
            }
            else
            {
                return CreateFontIndirectW(ref lplf);
            }
        }

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        public static extern int SetBkMode(IntPtr hDC, int nBkMode);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        public static extern int DrawText(IntPtr hDC, string lpszString, int nCount, ref NativeMethods.RECT lpRect, int nFormat);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        public static extern int GetRegionData(IntPtr hRgn, int size, byte[] data);

        public class RegionCracker
        {
            public static NativeMethods.RECT[] CrackRegionData(IntPtr hRgn)
            {
                NativeMethods.RECT[] nativeRects = new NativeMethods.RECT[0];
                byte[] rgnData;
                int rgnSize;
                int cbHeader;
                int count;
                try
                {
                    rgnSize = NativeMethods.GetRegionData(hRgn, 0, null);
                    rgnData = new byte[(uint)rgnSize];
                    if (rgnSize != NativeMethods.GetRegionData(hRgn, rgnSize, rgnData))
                    {
                        throw new InvalidOperationException("FailedToGetRegionInfo");
                    }

                    cbHeader = RegionCracker.ToInt(rgnData, 0);
                    count = RegionCracker.ToInt(rgnData, 8);
                    nativeRects = RegionCracker.GetRects(rgnData, cbHeader, count);
                }
                catch (Exception)
                {
                }

                return nativeRects;
            } // end of method CrackRegionData

            private static NativeMethods.RECT[] GetRects(byte[] buffer, int cbHeader, int nCount)
            {
                NativeMethods.RECT[] nativeRects = new NativeMethods.RECT[nCount];
                int left;
                int top;
                int right;
                int bottom;
                for (int n = 0; n < nCount; n++)
                {
                    left = RegionCracker.ToInt(buffer, cbHeader);
                    top = RegionCracker.ToInt(buffer, (cbHeader + 4));
                    right = RegionCracker.ToInt(buffer, (cbHeader + 8));
                    bottom = RegionCracker.ToInt(buffer, (cbHeader + 12/*0xc*/));
                    nativeRects[n] = new NativeMethods.RECT(left, top, right, bottom);
                    cbHeader = cbHeader + 16/*0x10*/;
                }

                return nativeRects;
            } // end of method GetRects

            private static int ToInt(byte[] buffer, int offset)
            {
                int n0;
                n0 = (int)buffer[offset];
                n0 = n0 | (int)buffer[(offset + 1)] << 8;
                n0 = n0 | (int)buffer[(offset + 2)] << 16/*0x10*/;
                n0 = n0 | (int)buffer[(offset + 3)] << 24/*0x18*/;
                return n0;
            } // end of method ToInt
        }

        public const int CFS_DEFAULT = 0x0000;
        public const int CFS_RECT = 0x0001;
        public const int CFS_POINT = 0x0002;
        public const int CFS_FORCE_POSITION = 0x0020;
        public const int CFS_CANDIDATEPOS = 0x0040;
        public const int CFS_EXCLUDE = 0x0080;

        [StructLayout(LayoutKind.Sequential)]
        internal struct tagCOMPOSITIONFORM
        {
            public long dwStyle;
            public NativeMethods.POINT ptCurrentPos;
            public NativeMethods.RECT rcArea;
        }

        [DllImport("imm32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        public static extern bool ImmSetCompositionWindow(IntPtr hIMC, ref tagCOMPOSITIONFORM lpCompForm);

        [DllImport("imm32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        public static extern bool ImmSetConversionStatus(IntPtr hIMC, int conversion, int sentence);

        [DllImport("imm32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        public static extern bool ImmGetConversionStatus(IntPtr hIMC, ref int conversion, ref int sentence);

        [DllImport("imm32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        public static extern IntPtr ImmGetContext(IntPtr hWnd);

        [DllImport("imm32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        public static extern bool ImmReleaseContext(IntPtr hWnd, IntPtr hIMC);

        [DllImport("imm32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        public static extern IntPtr ImmAssociateContext(IntPtr hWnd, IntPtr hIMC);

        [DllImport("imm32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        public static extern bool ImmDestroyContext(IntPtr hIMC);

        [DllImport("imm32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        public static extern IntPtr ImmCreateContext();

        [DllImport("imm32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        public static extern bool ImmSetOpenStatus(IntPtr hIMC, bool open);

        [DllImport("imm32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        public static extern bool ImmGetOpenStatus(IntPtr hIMC);

        [DllImport("imm32.dll")]
        public static extern IntPtr ImmGetCompositionWindow(IntPtr himc, ref COMPOSITIONFORM lpCompositionForm);

        [DllImport("imm32.dll")]
        public static extern IntPtr ImmSetCompositionWindow(IntPtr himc, COMPOSITIONFORM lpCompositionForm);

        [StructLayout(LayoutKind.Sequential)]
        [Syncfusion.Documentation.DocumentationExclude()]
        internal struct COMPOSITIONFORM
        {
            public int dwStyle;
            public POINT ptCurrentPos;
            public RECT rcArea;
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        public static extern bool DrawFrameControl(IntPtr hDC, ref NativeMethods.RECT rect, int type, int state);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        public static extern IntPtr BeginPaint(IntPtr hWnd, ref PAINTSTRUCT lpPaint);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        public static extern bool EndPaint(IntPtr hWnd, ref PAINTSTRUCT lpPaint);

        [StructLayout(LayoutKind.Sequential)]
        public struct PAINTSTRUCT
        {
            // Fields
            public IntPtr hdc;
            public bool fErase;
            public int rcPaint_left;
            public int rcPaint_top;
            public int rcPaint_right;
            public int rcPaint_bottom;
            public bool fRestore;
            public bool fIncUpdate;
            public int reserved1;
            public int reserved2;
            public int reserved3;
            public int reserved4;
            public int reserved5;
            public int reserved6;
            public int reserved7;
            public int reserved8;
        }

        public const int RDW_INVALIDATE = 0x0001;
        public const int RDW_INTERNALPAINT = 0x0002;
        public const int RDW_ERASE = 0x0004;
        public const int RDW_VALIDATE = 0x0008;
        public const int RDW_NOINTERNALPAINT = 0x0010;
        public const int RDW_NOERASE = 0x0020;
        public const int RDW_NOCHILDREN = 0x0040;
        public const int RDW_ALLCHILDREN = 0x0080;
        public const int RDW_UPDATENOW = 0x0100;
        public const int RDW_ERASENOW = 0x0200;
        public const int RDW_FRAME = 0x0400;
        public const int RDW_NOFRAME = 0x0800;

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        public static extern bool RedrawWindow(IntPtr hWnd, ref RECT lprcUpdate, IntPtr hrgnUpdate, uint flags);
    }
}
