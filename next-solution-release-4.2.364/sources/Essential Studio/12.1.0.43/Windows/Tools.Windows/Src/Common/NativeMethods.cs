#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
using System.Runtime.InteropServices.ComTypes;
#endif
using System.Security;
using System.Text;
using System.Windows.Forms;
using Syncfusion.Documentation;
using System.Security.Permissions;

namespace Syncfusion.Runtime.InteropServices
{
    [DocumentationExclude()]
    internal delegate bool CallBack(int hwnd, ref NativeMethods.RECT lParam);

    [ComVisible(false), SuppressUnmanagedCodeSecurity(), DocumentationExclude()]
    internal class NativeMethods
    {
        #region ListView messages
        public const int LVN_FIRST = -100;
        public const int LVM_FIRST = 0x1000;
        public const int LVN_GETDISPINFO = LVN_FIRST - 77;
        public const int LVS_OWNERDATA = 0x1000;
        public const int LVM_SETITEMCOUNT = LVM_FIRST + 47;
        public const int LVM_SETITEMSTATE = LVM_FIRST + 43;
        public const int LVM_GETSELECTEDCOUNT = LVM_FIRST + 50;
        public const int LVM_GETITEMSTATE = LVM_FIRST + 44;
        public const int LVM_FINDITEM = LVM_FIRST + 83;
        public const int LVM_GETITEM = LVM_FIRST + 5;
        public const int LVM_GETNEXTITEM = LVM_FIRST + 12;
        public const int LVM_GETHOTITEM = LVM_FIRST + 61;
        public const int LVM_ENSUREVISIBLE = LVM_FIRST + 19;
        public const int LVM_GETHEADER = LVM_FIRST + 31;
        public const int LVM_GETITEMSPACING = LVM_FIRST + 51;
        public const int LVM_GETITEMPOSITION = LVM_FIRST + 16;
        public const int LVM_HITTEST = LVM_FIRST + 18;

        public const uint LVIS_FOCUSED = 0x0001;
        public const uint LVIS_SELECTED = 0x0002;
        public const uint LVIS_CUT = 0x0004;
        public const uint LVIS_DROPHILITED = 0x0008;
        public const uint LVIS_GLOW = 0x0010;
        public const uint LVIS_ACTIVATING = 0x0020;

        public const uint LVIS_OVERLAYMASK = 0x0F00;
        public const uint LVIS_STATEIMAGEMASK = 0xF000;

        public const int LVNI_ALL = 0x0000;
        public const int LVNI_FOCUSED = 0x0001;
        public const int LVNI_SELECTED = 0x0002;
        public const int LVNI_CUT = 0x0004;
        public const int LVNI_DROPHILITED = 0x0008;

        public const uint LVIF_TEXT = 0x0001;
        public const uint LVIF_IMAGE = 0x0002;
        public const uint LVIF_PARAM = 0x0004;
        public const uint LVIF_STATE = 0x0008;
        public const uint LVIF_INDENT = 0x0010;
        public const uint LVIF_NORECOMPUTE = 0x0800;
        public const uint LVIF_GROUPID = 0x0100;
        public const uint LVIF_COLUMNS = 0x0200;

        public const uint LVFI_PARAM = 0x0001;
        public const uint LVFI_STRING = 0x0002;
        public const uint LVFI_PARTIAL = 0x0008;
        public const uint LVFI_WRAP = 0x0020;
        public const uint LVFI_NEARESTXY = 0x0040;
        #endregion

        #region ListView structs

        [StructLayout(LayoutKind.Sequential)]
        internal struct LVDISPINFO
        {
            public NMHDR hdr;
            public LVITEM item;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct LVITEM
        {
            public uint mask;
            public int iItem;
            public int iSubItem;
            public uint state;
            public uint stateMask;
            public IntPtr pszText;
            public int cchTextMax;
            public int iImage;
            public IntPtr lParam;
            public int iIndent;
            public int iGroupId;
            public uint cColumns;
            public IntPtr puColumns;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct NMHDR
        {
            public IntPtr hwndFrom;
            public IntPtr idFrom;
            public IntPtr code;
        }

        #endregion

        internal const int CB_GETCOMBOBOXINFO = 356; // 0x0164

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern bool GetComboBoxInfo(IntPtr hWnd, ref COMBOBOXINFO cbi);

        [StructLayout(LayoutKind.Sequential)]
        public struct COMBOBOXINFO
        {
            public uint cbSize;
            public RECT rcItem;
            public RECT rcButton;
            public uint stateButton;
            public IntPtr hwndCombo;
            public IntPtr hwndItem;
            public IntPtr hwndList;
        }

        [DocumentationExclude()]
        public class WinEventConstants
        {
            // Copied from WinUser.h, incomplete.
            public static ulong EVENT_SYSTEM_MENUSTART = 0x0004;
            public static ulong EVENT_SYSTEM_MENUEND = 0x0005;
            public static ulong EVENT_SYSTEM_MENUPOPUPSTART = 0x0006;
            public static ulong EVENT_SYSTEM_MENUPOPUPEND = 0x0007;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
        internal struct LVFINDINFO
        {
            public uint flags;
            public string psz;
            public IntPtr lParam;
            public int ptX;
            public int ptY;
            public int vkDirection;
        }

        [StructLayout(LayoutKind.Sequential),
           DocumentationExclude()]
        internal struct NCCALCSIZE_PARAMS
        {
            public RECT rgrc0, rgrc1, rgrc2;
            public IntPtr lppos;
        }

        [StructLayout(LayoutKind.Sequential),
           DocumentationExclude()]
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

        [StructLayout(LayoutKind.Sequential),
           DocumentationExclude()]
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

        [StructLayout(LayoutKind.Sequential),
           DocumentationExclude()]
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

            internal int Width
            {
                get
                {
                    return this.right - this.left;
                }
                set
                {
                    this.right = left + value;
                }
            }
            internal int Height
            {
                get
                {
                    return this.bottom - this.top;
                }
                set
                {
                    this.bottom = this.top + value;
                }
            }

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

        [StructLayout(LayoutKind.Sequential)]
        internal struct BITMAP
        {
            internal uint bmType;
            internal uint bmWidth;
            internal uint bmHeight;
            internal uint bmWidthBytes;
            internal ushort bmPlanes;
            internal ushort bmBitsPixel;
            internal IntPtr bmBits;
        }

        [StructLayout(LayoutKind.Sequential),
           DocumentationExclude()]
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
                return new NativeMethods.COMRECT(x, y, (x + width), (y + height));
            }
        }

        [
            ComVisible(true),
            StructLayout(LayoutKind.Sequential),
            DocumentationExclude()
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
            internal string lpszText = string.Empty;
        }

        [StructLayout(LayoutKind.Sequential),
           DocumentationExclude()]
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

        [StructLayout(LayoutKind.Sequential),
           DocumentationExclude()]
        internal struct LOGBRUSH
        {
            internal int lbStyle;
            internal int lbColor;
            internal IntPtr lbHatch;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode),
           DocumentationExclude()]
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

        [StructLayout(LayoutKind.Sequential)]
        internal class TRACKMOUSEEVENT
        {
            internal int cbSize = 0;
            internal int dwFlags = 0;
            internal IntPtr hwndTrack = IntPtr.Zero;
            internal int dwHoverTime = 0;
        }

        [
        StructLayout(LayoutKind.Sequential),
        DocumentationExclude()
        ]
        internal struct NONCLIENTMETRICS
        {
            internal int cbSize;
            internal int iBorderWidth;
            internal int iScrollWidth;
            internal int iScrollHeight;
            internal int iCaptionWidth;
            internal int iCaptionHeight;
            internal NativeMethods.LOGFONT lfCaptionFont;
            internal int iSmCaptionWidth;
            internal int iSmCaptionHeight;
            internal NativeMethods.LOGFONT lfSmCaptionFont;
            internal int iMenuWidth;
            internal int iMenuHeight;
            internal NativeMethods.LOGFONT lfMenuFont;
            internal NativeMethods.LOGFONT lfStatusFont;
            internal NativeMethods.LOGFONT lfMessageFont;
        }

        #region SetWindowPosFlags
        public enum SetWindowPosFlags : int
        {
            /// <summary>
            /// Represents NOSIZE
            /// </summary>
            SWP_NOSIZE = 0x0001,

            /// <summary>
            /// Represents NOMOVE
            /// </summary>
            SWP_NOMOVE = 0x0002,

            /// <summary>
            /// Represents NOZORDER
            /// </summary>
            SWP_NOZORDER = 0x0004,

            /// <summary>
            /// Represents NOREDRAW
            /// </summary>
            SWP_NOREDRAW = 0x0008,

            /// <summary>
            /// Represents NOACTIVATE
            /// </summary>
            SWP_NOACTIVATE = 0x0010,

            /// <summary>
            /// Represents FRAMECHANGED
            /// </summary>
            SWP_FRAMECHANGED = 0x0020,

            /// <summary>
            /// Represents SHOWWINDOW
            /// </summary>
            SWP_SHOWWINDOW = 0x0040,

            /// <summary>
            /// Represents HIDEWINDOW
            /// </summary>
            SWP_HIDEWINDOW = 0x0080,
            
            /// <summary>
            /// Represents NOCOPYBITS
            /// </summary>
            SWP_NOCOPYBITS = 0x0100,

            /// <summary>
            /// Represents NOOWNERZORDER
            /// </summary>
            SWP_NOOWNERZORDER = 0x0200,

            /// <summary>
            /// Represents NOSENDCHANGING
            /// </summary>
            SWP_NOSENDCHANGING = 0x0400,

            /// <summary>
            /// Represents DRAWFRAME 
            /// </summary>
            SWP_DRAWFRAME = 0x0020,

            /// <summary>
            /// Represents NOREPOSITION
            /// </summary>
            SWP_NOREPOSITION = 0x0200,

            /// <summary>
            /// Represents DEFERERASE
            /// </summary>
            SWP_DEFERERASE = 0x2000,

            /// <summary>
            /// Represents ASYNCWINDOWPOS
            /// </summary>
            SWP_ASYNCWINDOWPOS = 0x4000
        }
        #endregion

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
            internal int cxLeftWidth; // width of left border that retains its size
            internal int cxRightWidth; // width of right border that retains its size
            internal int cyTopHeight; // height of top border that retains its size
            internal int cyBottomHeight; // height of bottom border that retains its size

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
            private int iValueCount = 0; // number of values in iValues
            [MarshalAs(UnmanagedType.LPArray)]
            private int[] iValues;

            internal INTLIST()
            {
                this.iValues = new int[10];
            }

            internal int ValueCount
            {
                get
                {
                    return this.iValueCount;
                }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct THEME_ERROR_CONTEXT
        {
            internal ulong dwSize;

            //---- error context information ----
            internal IntPtr hr; // error code from last error
            internal string szMsgParam1; // value of first param for msg
            internal string szMsgParam2; // value of second param for msg
            internal string szFileName; // associated source filename
            internal string szSourceLine; // source line
            internal int iLineNum; // source line number
            internal THEME_ERROR_CONTEXT(IntPtr hr, string szMsgParam1, string szMsgParam2, string szFileName, string szSourceLine, int iLineNum)
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

        internal static readonly int TTM_SETTOOLINFO = 1082; // 1033/*0x409*/;
        internal static readonly int TTM_ADDTOOL = 1074; // 1028/*0x404*/;
        internal static readonly int TTN_GETDISPINFO = -520 /*0xfffffdf8*/;

        [StructLayout(LayoutKind.Sequential)]
        internal class TOOLINFO
        {
            // Fields
            internal int cbSize;
            internal int uFlags = 0;
            internal IntPtr hwnd = IntPtr.Zero;
            internal IntPtr uId = IntPtr.Zero;
            internal RECT rect = new RECT(Rectangle.Empty);
            internal IntPtr hinst = IntPtr.Zero;
            internal string lpszText = String.Empty;
            internal IntPtr lParam = IntPtr.Zero;

            // Constructors
            internal TOOLINFO()
            {
                this.cbSize = Marshal.SizeOf(typeof(NativeMethods.TOOLINFO));
            }
        }

        [StructLayout(LayoutKind.Sequential),
           DocumentationExclude()]
        internal struct CWPSTRUCT
        {
            internal IntPtr lParam;
            internal IntPtr wParam;
            internal int message;
            internal IntPtr hwnd;
        }

        [DocumentationExclude()]
        internal static int HIWORD(int n)
        {
            return (n >> 16) & 0xffff /*=~0x0000*/;
        }

        [DocumentationExclude()]
        internal static int LOWORD(int n)
        {
            return n & 0xffff /*=~0x0000*/;
        }

        [DocumentationExclude()]
        internal static int LOWORD(IntPtr n)
        {
            return LOWORD((int)n);
        }

        [DocumentationExclude()]
        internal static int HIWORD(IntPtr n)
        {
            return HIWORD((int)n);
        }

        [DocumentationExclude()]
        internal static int MAKELONG(int low, int high)
        {
            return (high << 16) | (low & 0xffff);
        }

        [DocumentationExclude()]
        internal static int MAKELPARAM(int low, int high)
        {
            return (high << 16) | (low & 0xffff);
        }

        [DocumentationExclude()]
        internal static int RGBToCOLORREF(int rgbValue)
        {
            int n0;
            n0 = (rgbValue & 255 /*0xff*/ ) << 16 /*0x10*/;
            rgbValue = rgbValue & 16776960 /*0xffff00*/;
            rgbValue = rgbValue | (rgbValue >> 16 /*0x10*/ & 255 /*0xff*/ );
            rgbValue = rgbValue & 65535 /*0xffff*/;
            rgbValue = rgbValue | n0;
            return rgbValue;
        }

        [DocumentationExclude()]
        internal static int COLORREFToRGB(int colorRef)
        {
            int r = colorRef & 255 /*0xff*/;
            int g = (colorRef >> 8) & 255 /*0xff*/;
            int b = (colorRef >> 16) & 255 /*0xff*/;

            int rgb = (r << 16) + (g << 8) + b;

            return rgb;
        }

        [DocumentationExclude()]
        internal static int GetRValue(int rgb)
        {
            return (rgb & 0xff0000) >> 16;
        }

        [DocumentationExclude()]
        internal static int GetGValue(int rgb)
        {
            return (rgb & 0x00ff00) >> 8;
        }

        [DocumentationExclude()]
        internal static int GetBValue(int rgb)
        {
            return rgb & 0x0000ff;
        }

        internal const int MF_BYCOMMAND = 0x00000000;
        internal const int MF_BYPOSITION = 0x00000400;

        internal const int MF_ENABLED = 0x00000000;
        internal const int MF_GRAYED = 0x00000001;
        internal const int MF_DISABLED = 0x00000002;

        internal const int HTTRANSPARENT = -1;
        internal const int RDW_INVALIDATE = 0x0001;
        internal const int RDW_INTERNALPAINT = 0x0002;
        internal const int RDW_ERASE = 0x0004;
        internal const int RDW_VALIDATE = 0x0008;

        internal const int RDW_NOINTERNALPAINT = 0x0010;
        internal const int RDW_NOERASE = 0x0020;
        internal const int RDW_NOCHILDREN = 0x0040;
        internal const int RDW_ALLCHILDREN = 0x0080;

        internal const int RDW_UPDATENOW = 0x0100;
        internal const int RDW_ERASENOW = 0x0200;
        internal const int RDW_FRAME = 0x0400;
        internal const int RDW_NOFRAME = 0x0800;

        internal const int EM_SETREADONLY = 0x00CF;

        internal const int SW_ERASE = 4; // 0x0004 
        internal const int SW_INVALIDATE = 2; // 0x0002 
        internal const int SW_SCROLLCHILDREN = 1; // 0x0001 
        internal const int SW_SMOOTHSCROLL = 0x0010; // Use smooth scrolling 
        internal const int SW_SHOWMINIMIZED = 2;
        internal const int SW_SHOWMAXIMIZED = 3;
        internal const int SW_HIDE = 0;
        internal const int SW_SHOWNORMAL = 1;
        internal const int SW_PARENTCLOSING = 1;
        internal const int SW_OTHERZOOM = 2;
        internal const int SW_PARENTOPENING = 3;
        internal const int SW_OTHERUNZOOM = 4;

        internal const int HTCLIENT = 1; // 0x0001 
        internal const int HTBOTTOMLEFT = 16;
        internal const int HTBOTTOMRIGHT = 17;
        internal const int HTBOTTOM = 15;
        internal const int HTRIGHT = 11;
        internal const int HTTOP = 12;
        internal const int HTTOPLEFT = 13;
        internal const int HTTOPRIGHT = 14;
        internal const int HTLEFT = 10;
        internal const int HTBORDER = 18;
        internal const int HTCAPTION = 2;
        internal const int HTCLOSE = 20;
        internal const int HTNOWHERE = 0;

        internal const int SM_CXVSCROLL = 2;
        internal const int SM_CYHSCROLL = 3;

        internal const int HWND_TOP = 0;
        internal const int HWND_BOTTOM = 1; // 0x0001 
        internal const int HWND_TOPMOST = -1; // 0xffff 
        internal const int HWND_NOTOPMOST = -2; // 0xfffe 

        internal const int MK_LBUTTON = 0x0001;
        internal const int MK_RBUTTON = 0x0002;
        internal const int MK_SHIFT = 0x0004;
        internal const int MK_CONTROL = 0x0008;
        internal const int MK_MBUTTON = 0x0010;

        internal const int TME_HOVER = 0x0001;
        internal const int TME_LEAVE = 0x0002;
        internal const int TME_NONCLIENT = 0x0010;
        internal const int HOVER_DEFAULT = -1;

        internal const int WA_INACTIVE = 0;
        internal const int WA_ACTIVE = 1;
        internal const int WA_CLICKACTIVE = 2;

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
        internal const int WS_EX_NOACTIVATE = 134217728 /*0x08000000*/;
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
        internal const int WS_EX_NOINHERITLAYOUT = 0x100000;
        internal const int WS_EX_LAYOUTRTL = 0x400000;
        internal const int WS_EX_TRANSPARENT = 0x00000020;
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
        internal const int WM_THEMECHANGED = 0x031A;

        internal const int VK_ESCAPE = 0x1B;
        internal const int VK_CONTROL = 0x11;

        internal const int WM_WIN_FORMS_MOUSE_ENTER = 0xC2F6;

        internal const int SBM_ENABLE_ARROWS = 228; // 0x00e4 
        internal const int SBM_GETPOS = 225; // 0x00e1 
        internal const int SBM_GETRANGE = 227; // 0x00e3 
        internal const int SBM_GETSCROLLINFO = 234; // 0x00ea 
        internal const int SBM_SETPOS = 224; // 0x00e0 
        internal const int SBM_SETRANGE = 226; // 0x00e2 
        internal const int SBM_SETRANGEREDRAW = 230; // 0x00e6 
        internal const int SBM_SETSCROLLINFO = 233; // 0x00e9 
        internal const int WMSZ_TOPLEFT = 4;
        internal const int WMSZ_TOP = 3;
        internal const int WMSZ_LEFT = 1;
        internal const int WMSZ_BOTTOMLEFT = 7;
        internal const int WMSZ_TOPRIGHT = 5;

        internal const int SC_CLOSE = 61536; // 0xF060
        internal const int SC_MOVE = 0xf012;
        internal const int SC_MINIMIZE = 61472; // 0xF020

        internal const int TMT_TEXTCOLOR = 3803;
        internal const int TMT_CAPTIONFONT = 801;

        internal const int BFFM_ENABLEOK = WM_USER + 101;
        internal const int BFFM_SETSELECTIONW = WM_USER + 103;
        internal const int BFFM_SETSTATUSTEXTW = WM_USER + 104;
        internal const int BFFM_SETOKTEXT = WM_USER + 105;
        internal const int BFFM_SETEXPANDED = WM_USER + 106;

        internal const int BFFM_INITIALIZED = 1;
        internal const int BFFM_SELCHANGED = 2;
        internal const int BFFM_VALIDATEFAILEDW = 4; // lParam:wzPath ret:1(cont),0(EndDialog)
        internal const int BFFM_IUNKNOWN = 5; // provides IUnknown to client. lParam: IUnknown*

        internal const int GW_HWNDFIRST = 0;
        internal const int GW_HWNDLAST = 1;
        internal const int GW_HWNDNEXT = 2;
        internal const int GW_HWNDPREV = 3;
        internal const int GW_OWNER = 4;
        internal const int GW_MAX = 5;
        internal const int GW_CHILD = 5;

        internal const int GWL_WNDPROC = -4 /*0xFFFFFFFC*/;
        internal const int GWL_HWNDPARENT = -8 /*0xFFFFFFF8*/;
        internal const int GWL_STYLE = -16 /*0xFFFFFFF0*/;
        internal const int GWL_EXSTYLE = -20 /*0xFFFFFFEC*/;
        internal const int GWL_ID = -12 /*0xFFFFFFF4*/;

        // mouse activate constants
        internal const int MA_ACTIVATE = 1;
        internal const int MA_ACTIVATEANDEAT = 2;
        internal const int MA_NOACTIVATE = 3;
        internal const int MA_NOACTIVATEANDEAT = 4;

        internal const int SWP_NOSIZE = 0x0001;
        internal const int SWP_NOMOVE = 0x0002;
        internal const int SWP_NOZORDER = 0x0004;
        internal const int SWP_NOACTIVATE = 0x0010;
        internal const int SWP_FRAMECHANGED = 0x0020;
        internal const int SWP_NOOWNERZORDER = 0x0200;
        internal const int SWP_HIDEWINDOW = 0x0080;

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        internal static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        internal static extern IntPtr SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong);
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        internal static extern IntPtr SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        internal const int DCX_WINDOW = 1 /*0x0001*/;
        internal const int DCX_CACHE = 2 /*0x0002*/;
        internal const int DCX_INTERSECTRGN = 0x0080;
        internal const int DCX_LOCKWINDOWUPDATE = 1024 /*0x0400*/;
        internal const int DCX_PARENTCLIP = 0x0020;
        internal const int DCX_CLIPSIBLINGS = 16 /*0x00000010*/;
        internal const int DCX_CLIPCHILDREN = 8 /*0x00000008L*/;
        internal const int DCX_EXCLUDERGN = 64 /*0x00000040*/;

        // ComboBox Control Messages
        internal const int CB_GETCURSEL = 0x0147;
        internal const int CB_GETLBTEXT = 0x0148;
        internal const int CB_GETLBTEXTLEN = 0x0149;
        internal const int CB_GETDROPPEDSTATE = 0x0157;

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        internal static extern int CombineRgn(IntPtr hRgn, IntPtr hRgn1, IntPtr hRgn2, int nCombineMode);
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        internal static extern int SetWindowRgn(IntPtr hWnd, IntPtr hRgn, bool bRedraw);
        [DllImport("gdi32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        internal static extern int SetRectRgn(IntPtr hRgn, int nLeftRect, int nTopRect, int nRightRect, int nBottomRect);

        internal const int RGN_AND = 1;
        internal const int RGN_OR = 2;
        internal const int RGN_XOR = 3;
        internal const int RGN_DIFF = 4;
        internal const int RGN_COPY = 5;

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        internal static extern IntPtr CreateRectRgn(int x1, int y1, int x2, int y2);

        [DllImport("gdi32")]
        internal static extern int GetRgnBox(IntPtr hrgn, ref RECT lprc);

        [DllImport("gdi32.dll", CallingConvention = CallingConvention.Winapi)]
        internal static extern bool LPtoDP(IntPtr hDC, NativeMethods.POINT lpPoint, int nCount);

        [DllImport("gdi32.dll", CallingConvention = CallingConvention.Winapi)]
        internal static extern bool LPtoDP(IntPtr hDC, int lpPoints, int nCount);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool LPtoDP(IntPtr hDC, NativeMethods.SIZE lpSize, int nCount);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool LPtoDP(IntPtr hDC, ref NativeMethods.RECT lpRect, int nCount);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool DPtoLP(IntPtr hDC, ref NativeMethods.RECT lpRect, int nCount);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool SetWindowOrgEx(IntPtr hDC, int x, int y, NativeMethods.POINT point);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool GetWindowExtEx(IntPtr hDC, out NativeMethods.SIZE s);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool GetViewportExtEx(IntPtr hDC, out NativeMethods.SIZE s);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        internal static extern int SetMapMode(IntPtr hDC, int nMapMode);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        internal static extern int GetMapMode(IntPtr hDC);

        [DllImport("gdi32", CharSet = CharSet.Auto, ExactSpelling = true)]
        internal static extern bool SetViewportExtEx(IntPtr hDC, int x, int y, out SIZE size);

        [DllImport("gdi32", CharSet = CharSet.Auto, ExactSpelling = true)]
        internal static extern bool SetWindowExtEx(IntPtr hDC, int x, int y, out SIZE size);

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
        internal const int QS_MOUSE = QS_MOUSEMOVE | QS_MOUSEBUTTON;
        internal const int QS_INPUT = QS_MOUSE | QS_KEY | QS_RAWINPUT;
        internal const int QS_ALLEVENTS = QS_INPUT | QS_POSTMESSAGE | QS_TIMER | QS_PAINT | QS_HOTKEY;
        internal const int QS_ALLINPUT = QS_INPUT | QS_POSTMESSAGE | QS_TIMER | QS_PAINT | QS_HOTKEY | QS_SENDMESSAGE;
        internal const int WAIT_OBJECT_0 = 0;
        internal const int WAIT_TIMEOUT = 258;
        internal const uint INFINITE = 0xFFFFFFFF;

        [DllImport("user32.dll")]
        internal static extern bool SetLayeredWindowAttributes(
            IntPtr hwnd, uint crKey, byte bAlpha, int dwFlags);

        [DllImport("user32.dll")]
        internal static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, int uFlags);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool ScrollWindowEx(IntPtr hWnd, int nXAmount, int nYAmount, ref NativeMethods.RECT rectScrollRegion, ref NativeMethods.RECT rectClip, IntPtr hrgnUpdate, ref NativeMethods.RECT prcUpdate, int flags);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool ScrollWindowEx(IntPtr hWnd, int nXAmount, int nYAmount, NativeMethods.COMRECT rectScrollRegion, ref NativeMethods.RECT rectClip, IntPtr hrgnUpdate, ref NativeMethods.RECT prcUpdate, int flags);

        [DllImport("user32", CharSet = CharSet.Auto, ExactSpelling = true)]
        internal static extern bool InvertRect(IntPtr hDC, ref RECT lpRect);

        [DllImport("user32.dll")]
        internal static extern uint MapVirtualKey(uint key, uint mapType);

        [DllImport("user32.dll")]
        internal static extern bool GetKeyboardState(byte[] keyState);

        [DllImport("user32.dll")]
        internal static extern short GetKeyState(int nVirtKey /* virtual-key code*/ );

        [DllImport("user32.dll")]
        internal static extern void NotifyWinEvent(ulong eventConstant, IntPtr hwnd, long idObject, long idChild);

        [DllImport("user32.dll")]
        internal static extern int ToUnicode(
            uint wVirtKey, // virtual-key code
            uint wScanCode, // scan code
            byte[] lpKeyState, // key-state array
            byte[] pwszBuff, // translated key buffer
            int cchBuff, // size of translated key buffer
            uint wFlags // function options
            );

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, ref RECT lpRect);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SendMessage(IntPtr hWnd, int msg, int wParam, ref TOOLINFO_D lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, out bool bValue);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, int lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, IntPtr lParam);

        [DllImport("user32", CharSet = CharSet.Auto)]
        internal static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, TOOLINFO_T lParam);

        [DllImport("user32", CharSet = CharSet.Auto)]
        internal static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, TV_HITTESTINFO lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        internal static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, TOOLINFO lParam);

        [DllImport("user32")]
        internal static extern bool ScrollWindow(IntPtr hWnd, int nXAmount, int nYAmount, ref RECT rectScrollRegion, ref RECT rectClip);

        [DllImport("user32.dll")]
        internal static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("gdi32.dll")]
        internal static extern IntPtr GetStockObject(int fnObject);

        [DllImport("comctl32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool ImageList_DrawEx(IntPtr himl, int i, IntPtr hdcDst, int x, int y, int dx, int dy, long rgbBk, long rgbFg, uint fStyle);

        [
            ComVisible(true),
            StructLayout(LayoutKind.Sequential, Pack = 1),
            DocumentationExclude()
            ]
        internal class INITCOMMONCONTROLSEX
        {
            internal int dwSize = Marshal.SizeOf(typeof(INITCOMMONCONTROLSEX));
            internal int dwICC = 0;
        }

        [DllImport("comctl32")]
        internal static extern void InitCommonControls();

        [DllImport("comctl32")]
        internal static extern bool InitCommonControlsEx(INITCOMMONCONTROLSEX icc);

        [DllImport("comctl32")]
        internal static extern bool InitializeFlatSB(IntPtr hWnd);

        [DllImport("comctl32")]
        internal static extern bool UninitializeFlatSB(IntPtr hWnd);

        [DllImport("comctl32")]
        internal static extern int FlatSB_SetScrollInfo(IntPtr hWnd, int fnBar, ref SCROLLINFO si, bool redraw);

        [DllImport("comctl32")]
        internal static extern bool FlatSB_GetScrollInfo(IntPtr hWnd, int fnBar, ref SCROLLINFO si);

        [DllImport("comctl32")]
        internal static extern bool FlatSB_SetScrollProp(IntPtr hWnd, int index, int newValue, bool fRedraw);

        [DllImport("comctl32")]
        internal static extern bool FlatSB_GetScrollProp(IntPtr hWnd, int index, ref int value);

        [DllImport("comctl32")]
        internal static extern bool FlatSB_EnableScrollBar(IntPtr hWnd, int wSBflags, int wArrows);

        [
            StructLayout(LayoutKind.Sequential),
            DocumentationExclude()
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

        public const int SB_HORZ = 0;
        public const int SB_VERT = 1;
        public const int SB_CTL = 2;

        [DllImport("user32", CharSet = CharSet.Auto, ExactSpelling = true)]
        internal static extern int SetScrollInfo(IntPtr hWnd, int fnBar, ref SCROLLINFO si, bool redraw);

        [DllImport("user32", CharSet = CharSet.Auto, ExactSpelling = true)]
        internal static extern bool GetScrollInfo(IntPtr hWnd, int fnBar, ref SCROLLINFO si);

        [DllImport("gdi32")]
        internal static extern int SetTextColor(IntPtr hDC, int crColor);

        [DllImport("gdi32")]
        internal static extern int SetBkColor(IntPtr hDC, int clr);

        [DllImport("gdi32")]
        internal static extern int SetBkMode(IntPtr hdc, int iBkMode);

        [DllImport("gdi32")]
        internal static extern IntPtr CreateSolidBrush(int crColor);

        [DllImport("gdi32")]
        internal static extern IntPtr CreatePen(int fnPenStyle, int nWidth, int crColor);

        [DllImport("gdi32")]
        internal static extern bool DeleteObject(IntPtr hObject);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern bool DestroyIcon(IntPtr hIcon);

        [DllImport("user32")]
        internal static extern IntPtr GetSysColorBrush(int nIndex);

        [DllImport("user32")]
        internal static extern int GetSysColor(int nIndex);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        internal static extern IntPtr CreateFontIndirect([In] ref LOGFONT lplf);

        [DllImport("gdi32", CharSet = CharSet.Auto, ExactSpelling = true)]
        internal static extern IntPtr CreateBitmap(int nWidth, int nHeight, int nPlanes, int nBitsPerPixel, [MarshalAs(UnmanagedType.LPArray)] short[] lpvBits);

        [DllImport("gdi32", CharSet = CharSet.Auto, ExactSpelling = true)]
        internal static extern IntPtr CreateBrushIndirect(ref LOGBRUSH lb);

        [DllImport("gdi32", CharSet = CharSet.Auto, ExactSpelling = true)]
        internal static extern IntPtr SelectObject(IntPtr hdc, IntPtr hObject);

        [DllImport("user32", CharSet = CharSet.Auto, ExactSpelling = true)]
        internal static extern int FillRect(IntPtr hdc, ref RECT rect, IntPtr hBrush);

        [DllImport("gdi32", CharSet = CharSet.Auto)]
        internal static extern bool ExtTextOut(IntPtr hdc, int x, int y, int nOptions, ref RECT lpRect, string s, int nStrLength, int[] lpDx);

        [DllImport("gdi32", CharSet = CharSet.Auto, ExactSpelling = true)]
        internal static extern bool SetViewportOrgEx(IntPtr hdc, int x, int y, out POINT point);

        [DllImport("gdi32", CharSet = CharSet.Auto, ExactSpelling = true)]
        internal static extern bool PatBlt(IntPtr hdc, int x, int y, int nWidth, int nHeight, int dwRop);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        private static extern IntPtr CreateDC(string lpszDriver, string lpszDeviceName, string lpszOutput, IntPtr devMode);

        internal static IntPtr CreateDC(string lpszDriver)
        {
            return CreateDC(lpszDriver, null, null, IntPtr.Zero);
        } // end of method CreateDC

        [DllImport("gdi32", EntryPoint = "DeleteDC", CharSet = CharSet.Auto, ExactSpelling = true)]
        internal static extern bool DeleteDC(IntPtr hDC);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        internal static extern IntPtr ChildWindowFromPoint(IntPtr hwndParent, int x, int y);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        internal static extern IntPtr ChildWindowFromPoint(IntPtr hwndParent, POINT pt);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        internal static extern IntPtr WindowFromPoint(int x, int y);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        internal static extern IntPtr WindowFromPoint(POINT pt);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool SubtractRect([In] ref RECT rcdest, ref RECT rc1, ref RECT rc2);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool LockWindowUpdate(IntPtr hWndLock);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool RedrawWindow(IntPtr hWnd, IntPtr lprcUpdate, IntPtr hrgnUpdate, uint flags);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool RedrawWindow(IntPtr hWnd, ref RECT lprcUpdate, IntPtr hrgnUpdate, uint flags);

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

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool IsWindowVisible(IntPtr hWnd);

        internal const int TECHNOLOGY = 2; /* Device classification                    */
        internal const int DT_METAFILE = 5; /* Metafile, VDM                    */
        internal const int DT_DISPFILE = 6; /* Display-file                     */

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        internal static extern int GetDeviceCaps(IntPtr hDC, int nIndex);

        [StructLayout(LayoutKind.Sequential)]
        internal struct ICONINFO
        {
            public bool fIcon;
            public int xHotspot;
            public int yHotspot;
            public IntPtr hbmMask;
            public IntPtr hbmColor;
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool GetIconInfo(IntPtr hIcon, out ICONINFO piconinfo);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool IsWindow(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool GetUpdateRect(IntPtr hWnd, ref RECT lprc, bool bErase);

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

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool SetLayout(IntPtr hDC, int dwLayout);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        internal static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool DrawFrameControl(IntPtr hDC, ref RECT rect, int type, int state);

        [DocumentationExclude()]
        internal delegate IntPtr HookProc(int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        internal static extern IntPtr SetWindowsHookEx(int idHook, HookProc pfnHook, IntPtr hinst, int dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool UnhookWindowsHookEx(IntPtr hhook);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        internal static extern IntPtr CallNextHookEx(IntPtr hhook, int code, IntPtr wparam, IntPtr lparam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        internal static extern IntPtr GetModuleHandle(string modName);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        internal static extern int GetCurrentThreadId();

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        internal static extern IntPtr GetAncestor(IntPtr hwnd, int gaFlags);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool SetWindowText(IntPtr hwnd, string windowtext);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        internal static extern int GetWindowText(IntPtr hwnd, [MarshalAs(UnmanagedType.LPTStr)] string windowtext, int maxcount);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        internal static extern int GetWindowText(IntPtr hwnd, StringBuilder lptString, int maxcount);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        internal static extern int GetWindowTextLength(IntPtr hwnd);

        [DllImport("user32")]
        internal static extern bool HideCaret(IntPtr hWnd);

        [DllImport("user32")]
        internal static extern bool ShowCaret(IntPtr hWnd);

        [DocumentationExclude()]
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
            StructLayout(LayoutKind.Sequential),
                ComVisible(false),
                DocumentationExclude()
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

        [
        StructLayout(LayoutKind.Sequential),
        ComVisible(false)
        ]
        internal class TV_HITTESTINFO
        {
            // Fields
            internal int pt_x = 0;
            internal int pt_y = 0;
            internal int flags = 0;
            internal int hItem = 0;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal class LV_HITTESTINFO
        {
            internal NativeMethods.POINT pt = new POINT(0, 0);
            internal int flags = 0;
            internal int iItem = 0;
            internal int iSubItem = 0;
        }

        [DllImport("shell32.dll", CallingConvention = CallingConvention.Winapi)]
        internal static extern int SHGetSpecialFolderLocation(IntPtr hwnd, int csidl, ref IntPtr ppidl);

        [DllImport("shell32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool SHGetPathFromIDList(IntPtr pidl, IntPtr pszPath);

        [DllImport("shell32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        internal static extern IntPtr SHBrowseForFolder([In] BROWSEINFO lpbi);

        [DllImport("shell32.dll", CallingConvention = CallingConvention.Winapi)]
        internal static extern int SHGetMalloc([Out, MarshalAs(UnmanagedType.LPArray)] IMalloc[] ppMalloc);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        internal static extern int ClientToScreen(IntPtr hWnd, ref NativeMethods.POINT pt);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        internal static extern int MapWindowPoints(IntPtr hWndFrom, IntPtr hWndTo, [MarshalAs(UnmanagedType.LPArray)] POINT[] lpPoints, uint cPoints);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        internal static extern int ScreenToClient(IntPtr hWnd, ref NativeMethods.POINT pt);

        [DllImport("user32", CharSet = CharSet.Auto)]
        internal static extern int SendDlgItemMessage(IntPtr hDlg, int nIDDlgItem, int Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32", CharSet = CharSet.Auto, ExactSpelling = true)]
        internal static extern IntPtr GetDlgItem(IntPtr hWnd, int nIDDlgItem);

        [DllImport("user32", CharSet = CharSet.Auto, ExactSpelling = true)]
        internal static extern bool EnableWindow(IntPtr hWnd, bool enable);

        [DllImport("user32", CharSet = CharSet.Auto, ExactSpelling = true)]
        internal static extern int GetDlgItemInt(IntPtr hWnd, int nIDDlgItem, bool[] err, bool signed);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        internal static extern int GetObject(IntPtr hgdiobj, int cbBuffer, out BITMAP bm);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern bool PostMessage(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam);

        [DllImport("user32.dll")]
        internal static extern int GetSystemMetrics(int nIndex);

        [StructLayout(LayoutKind.Sequential),
           DocumentationExclude()]
        internal struct APPBARDATA
        {
            internal int cbSize;
            internal int hwnd;
            internal int uCallbackMessage;
            internal int uEdge;
            internal NativeMethods.RECT rc;
            internal int lParam;
        }

        internal const int ABM_GETTASKBARPOS = 5;
        internal const int SPI_GETWORKAREA = 48;
        internal const int SPI_GETDROPSHADOW = 0x1024;

        internal const int ABE_BOTTOM = 3;
        internal const int ABE_LEFT = 0;
        internal const int ABE_RIGHT = 2;
        internal const int ABE_TOP = 1;

        [DllImport("shell32.dll")]
        internal static extern int SHAppBarMessage(int dwMessage, ref NativeMethods.APPBARDATA pData);

        [DllImport("user32.dll")]
        internal static extern int SystemParametersInfo(int uAction, int uParam, ref NativeMethods.RECT lpvParam, int fuWinIni);

        [DllImport("user32.dll")]
        internal static extern int SystemParametersInfo(int uAction, int uParam, ref NativeMethods.NONCLIENTMETRICS lpvParam, int fuWinIni);

        [DllImport("user32.dll")]
        internal static extern int SystemParametersInfo(int uAction, int uParam, ref bool lpvParam, int fuWinIni);

        [DllImport("user32.dll")]
        internal static extern int FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        internal static extern int GetWindowRect(int hwnd, ref NativeMethods.RECT lpRect);

        [DllImport("user32.dll")]
        public static extern bool GetClientRect(IntPtr hWnd, ref NativeMethods.RECT rc);

        [DllImport("user32.dll")]
        internal static extern int InvalidateRect(IntPtr hwnd, ref NativeMethods.RECT lpRect, bool bErase);

        [DllImport("user32.dll")]
        internal static extern int EnumChildWindows(int hWndParent, CallBack lpEnumFunc, ref NativeMethods.RECT lParam);

        internal delegate bool EnumChildWindowsCallBack(IntPtr hwnd, IntPtr lParam);

        [DllImport("user32.dll")]
        internal static extern int EnumChildWindows(IntPtr hWndParent, EnumChildWindowsCallBack lpEnumFunc, IntPtr lParam);

        internal const int CS_NOCLOSE = 0x200;
        internal const int CS_DROPSHADOW = 0x00020000;

        internal const int GCL_STYLE = -26;

        [DllImport("user32.dll")]
        internal static extern int GetClassLong(IntPtr hWnd, int index);

        [DllImport("user32.dll")]
        internal static extern int GetClassName(IntPtr hwnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32.dll")]
        internal static extern IntPtr GetTopWindow(IntPtr hWnd);

        [DllImport("gdi32.dll")]
        internal static extern bool BitBlt(IntPtr hdcDest, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, int dwRop);

        [StructLayout(LayoutKind.Sequential),
           DocumentationExclude()]
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

        [StructLayout(LayoutKind.Sequential)]
        internal struct WINDOWPOS
        {
            public IntPtr hwnd;
            public IntPtr hwndInsertAfter;
            public int x;
            public int y;
            public int cx;
            public int cy;
            public int flags;
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
        internal static extern IntPtr GetSubMenu(IntPtr hMenu, int count);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        internal static extern IntPtr GetMenu(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool IsMenu(IntPtr hMenu);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        internal static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        internal static extern int GetMenuString(IntPtr hMenu, uint uIDItem, [MarshalAs(UnmanagedType.LPTStr)] string lpString, int maxCount, uint uFlag);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool DrawMenuBar(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool DestroyMenu(IntPtr hMenu);

        internal const uint WM_CBAR_SELANDDISP = 0x8101; // WM_APP + 101

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool IsZoomed(IntPtr hWnd);

        [DocumentationExclude()]
        internal delegate bool DrawStateProc(IntPtr hdc, IntPtr lData, IntPtr wData, int cx, int cy);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool DrawState(IntPtr hdc, IntPtr hbr, DrawStateProc lpOutputFunc, IntPtr lData, IntPtr wData, int x, int y, int cx, int cy, uint fuFlags);
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        internal static extern IntPtr CopyImage(IntPtr hImage, uint uType, int cxDesired, int cyDesired, uint fuflags);
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool PeekMessage([In] ref MSG msg, IntPtr hwnd, int msgMin, int msgMax, int remove);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool TrackMouseEvent(ref TRACKMOUSEEVENT lpEventTrack);

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

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        internal static extern int MulDiv(int nNumber, int nNumerator, int nDenominator);

        [DllImport("user32.dll")]
        internal static extern int MsgWaitForMultipleObjects(int nCount, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] IntPtr[] pHandles, bool bWaitAll, uint dwMilliseconds, int dwWakeMask);

        [DllImport("USER32.dll")]
        internal static extern IntPtr SetCapture(IntPtr hWnd);

        [DllImport("USER32.dll")]
        internal static extern IntPtr GetCapture();

        [DllImport("USER32.dll")]
        internal static extern IntPtr ReleaseCapture();

        [DllImport("user32.dll")]
        internal static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, IntPtr dwExtraInfo);

        [DllImport("Gdi32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool GetTextExtentPoint32(IntPtr hdc, string lpString, int cbString, ref SIZE lpSize);

        [DllImport("uxtheme.dll", EntryPoint = "OpenThemeData", SetLastError = true)]
        internal static extern IntPtr IntOpenThemeData(IntPtr hwnd, [MarshalAs(UnmanagedType.LPWStr)] string pszClassList);

        [DllImport("uxtheme.dll", EntryPoint = "CloseThemeData", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        internal static extern IntPtr IntCloseThemeData(IntPtr hTheme);

#if ___DEBUG
		private static Hashtable themeHandles = new Hashtable();
#endif

        internal static IntPtr OpenThemeData(IntPtr hwnd, string pszClassList)
        {
            IntPtr hTheme = IntOpenThemeData(hwnd, pszClassList);
#if ___DEBUG
			themeHandles.Add( hTheme, null );
#endif
            return hTheme;
        }

        internal static IntPtr CloseThemeData(IntPtr hTheme)
        {
#if ___DEBUG
			if( !themeHandles.Contains( hTheme ) )
			{
				throw new InvalidOperationException( "Handle has been closed already." );
			}
			themeHandles.Remove( hTheme );
#endif
            return IntCloseThemeData(hTheme);
        }

        [DllImport("uxtheme.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool IsThemeActive();

        [DllImport("uxtheme.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        internal static extern int DrawThemeBackground(IntPtr hTheme, IntPtr hdc, int iPartId, int iStateId, ref NativeMethods.RECT rect, ref NativeMethods.RECT clipRect);

        [DllImport("uxtheme.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        internal static extern int DrawThemeText(IntPtr hTheme, IntPtr hdc, int iPartId, int iStateId, string pszText, int iCharCount, uint dwTextFlags, uint dwTextFlags2, [MarshalAs(UnmanagedType.Struct)] ref NativeMethods.RECT rect);

        [DllImport("uxtheme.dll", EntryPoint = "GetThemeBackgroundContentRect")]
        internal static extern void GetThemeBackgroundContentRect(int hTheme, IntPtr hdc, int iPartId, int iStateId, ref NativeMethods.RECT pBoundingRect, ref NativeMethods.RECT pContentRect);

        [DllImport("uxtheme.dll", EntryPoint = "GetThemeBackgroundExtent")]
        internal static extern void GetThemeBackgroundExtent(int hTheme, IntPtr hdc, int iPartId, int iStateId, ref NativeMethods.RECT pContentRect, ref NativeMethods.RECT pExtentRect);

        // Assumes you will always pass null for the 5th param.
        [DllImport("uxtheme.dll")]
        internal static extern uint GetThemePartSize(IntPtr hTheme, IntPtr hdc, int iPartId, int iStateId, IntPtr prc, int sizeType, out SIZE psz);

        [DllImport("uxtheme.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        internal static extern uint GetThemeTextExtent(IntPtr hTheme, IntPtr hdc, int iPartId, int iStateId, string pszText, int iCharCount, uint dwTextFlags, [MarshalAs(UnmanagedType.Struct)] ref NativeMethods.RECT pBoundingRect, out NativeMethods.RECT pExtentRect);

        [DllImport("uxtheme.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        internal static extern ulong GetThemeTextMetrics(IntPtr hTheme, IntPtr hdc, int iPartId, int iStateId, [In, Out] ref NativeMethods.TEXTMETRIC ptm);

        [DllImport("uxtheme.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        internal static extern ulong GetThemeBackgroundRegion(IntPtr hTheme, int iPartId, int iStateId, NativeMethods.RECT pRect, [In, Out] ref IntPtr pRegion);

        [DllImport("uxtheme.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        internal static extern ulong HitTestThemeBackground(IntPtr hTheme, IntPtr hdc, int iPartId, int iStateId, ulong dwOptions, NativeMethods.RECT pRect, IntPtr hrgn, NativeMethods.POINT ptTest, [In, Out] ref uint wHitTestCode);

        // [DllImport("uxtheme.dll", CharSet=CharSet.Auto, CallingConvention=CallingConvention.Winapi)] 
        // internal static extern ulong DrawThemeLine(IntPtr hTheme, IntPtr hdc, Int32 iStateId, 
        // NativeMethods.RECT pRect, ulong dwDtlFlags);
        [DllImport("uxtheme.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        internal static extern ulong DrawThemeEdge(IntPtr hTheme, IntPtr hdc, int iPartId, int iStateId, NativeMethods.RECT pDestRect, uint uEdge, uint uFlags, [In, Out] ref NativeMethods.RECT contentRect);

        [DllImport("uxtheme.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        internal static extern ulong DrawThemeIcon(IntPtr hTheme, IntPtr hdc, int iPartId, int iStateId, NativeMethods.RECT pRect, IntPtr himl, int iImageIndex);

        [DllImport("uxtheme.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool IsThemePartDefined(IntPtr hTheme, int iPartId, int iStateId);

        [DllImport("uxtheme.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool IsThemeBackgroundPartiallyTransparent(IntPtr hTheme, int iPartId, int iStateId);

        [DllImport("uxtheme.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        internal static extern int GetThemeColor(IntPtr hTheme, int iPartId, int iStateId, int iPropId, [In, Out] ref ulong color);

        [DllImport("uxtheme.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        internal static extern ulong GetThemeMetric(IntPtr hTheme, int iPartId, int iStateId, int iPropId, [In, Out] ref int iVal);

        [DllImport("uxtheme.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        internal static extern ulong GetThemeString(IntPtr hTheme, int iPartId, int iStateId, int iPropId, [In, Out] ref string pszBuff, int cchMaxBuffChars);

        [DllImport("uxtheme.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        internal static extern ulong GetThemeBool(IntPtr hTheme, int iPartId, int iStateId, int iPropId, [In, Out] ref bool fVal);

        [DllImport("uxtheme.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        internal static extern ulong GetThemeInt(IntPtr hTheme, int iPartId, int iStateId, int iPropId, [In, Out] ref int iVal);

        [DllImport("uxtheme.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        internal static extern ulong GetThemeEnumValue(IntPtr hTheme, int iPartId, int iStateId, int iPropId, [In, Out] ref int iVal);

        [DllImport("uxtheme.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        internal static extern ulong GetThemePosition(IntPtr hTheme, int iPartId, int iStateId, int iPropId, [In, Out] ref NativeMethods.POINT point);

        [DllImport("uxtheme.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Winapi)]
        internal static extern ulong GetThemeFont(IntPtr hTheme, IntPtr hdc, int iPartId, int iStateId, int iPropId, [Out] out NativeMethods.LOGFONT font);

        [DllImport("uxtheme.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        internal static extern ulong GetThemeRect(IntPtr hTheme, int iPartId, int iStateId, int iPropId, [In, Out] ref NativeMethods.RECT pRect);

        [DllImport("uxtheme.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        internal static extern ulong GetThemeMargins(IntPtr hTheme, int iPartId, int iStateId, int iPropId, [Out] out NativeMethods.MARGINS margins);

        [DllImport("uxtheme.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        internal static extern ulong GetThemeIntList(IntPtr hTheme, int iPartId, int iStateId, int iPropId, [In, Out] ref NativeMethods.INTLIST intList);

        [DllImport("uxtheme.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        internal static extern ulong GetThemePropertyOrigin(IntPtr hTheme, int iPartId, int iStateId, int iPropId, [In, Out] ref int origin);

        [DllImport("uxtheme.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        internal static extern ulong SetWindowTheme(IntPtr hwnd, string pszSubAppName, string pszSubIdList);

        [DllImport("uxtheme.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        internal static extern ulong GetThemeFilename(IntPtr hTheme, int iPartId, int iStateId, int iPropId, [In, Out] ref string pszThemeFileName, int cchMaxBuffChars);

        [DllImport("uxtheme.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        internal static extern ulong GetThemeSysColor(IntPtr hTheme, int iColorId);

        [DllImport("uxtheme.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        internal static extern IntPtr GetThemeSysColorBrush(IntPtr hTheme, int iColorId);

        [DllImport("uxtheme.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        internal static extern int GetThemeSysSize(IntPtr hTheme, int iSizeId);

        [DllImport("uxtheme.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool GetThemeSysBool(IntPtr hTheme, int iBoolId);

        [DllImport("uxtheme.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Winapi)]
        internal static extern ulong GetThemeSysFont(IntPtr hTheme, int iFontId, [In, Out] ref NativeMethods.LOGFONT lf);

        [DllImport("uxtheme.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        internal static extern ulong GetThemeSysString(IntPtr hTheme, int iStringId, [In, Out] ref string pszStringBuff, int cchMaxStringChars);

        [DllImport("uxtheme.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        internal static extern ulong GetThemeSysInt(IntPtr hTheme, int iIntId, [In, Out] ref int iValue);

        [DllImport("uxtheme.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool IsAppThemed();

        [DllImport("uxtheme.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        internal static extern IntPtr GetWindowTheme(IntPtr hwnd);

        [DllImport("uxtheme.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        internal static extern ulong EnableThemeDialogTexture(IntPtr hwnd, bool fEnable);

        [DllImport("uxtheme.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool IsThemeDialogTextureEnabled(IntPtr hwnd);

        [DllImport("uxtheme.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        internal static extern ulong GetThemeAppProperties();

        [DllImport("uxtheme.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        internal static extern void SetThemeAppProperties(ulong dwFlags);

        [DllImport("uxtheme.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        internal static extern ulong GetCurrentThemeName(
            [MarshalAs(UnmanagedType.LPWStr)] string pszThemeFileName, int cchMaxNameChars, [MarshalAs(UnmanagedType.LPWStr)] string pszColorBuff, int cchMaxColorChars, [MarshalAs(UnmanagedType.LPWStr)] string pszSizeBuff, int cchMaxSizeChars);

        [DllImport("uxtheme.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        internal static extern ulong GetThemeDocumentationProperty(string pszThemeName, string pszPropertyName, [In, Out] ref string pszValueBuff, int cchMaxValChars);

         // [DllImport("uxtheme.dll", CharSet=CharSet.Auto, CallingConvention=CallingConvention.Winapi)] 
         // internal static extern ulong GetThemeLastErrorContext([In, Out]ref NativeMethods.THEME_ERROR_CONTEXT context); 
         // [DllImport("uxtheme.dll", CharSet=CharSet.Auto, CallingConvention=CallingConvention.Winapi)] 
         // internal static extern ulong FormatThemeMessage(ulong dwLanguageId, 
         // NativeMethods.THEME_ERROR_CONTEXT context, [In, Out]ref string pszMessageBuff, 
         // Int32 cchMaxMessageChars)
         // [DllImport("uxtheme.dll", CharSet=CharSet.Auto, CallingConvention=CallingConvention.Winapi)] 
         // internal static extern ulong GetThemeImageFromParent(IntPtr hwnd, IntPtr hdc, NativeMethods.RECT rc);
        [DllImport("uxtheme.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        internal static extern IntPtr DrawThemeParentBackground(IntPtr hwnd, IntPtr hdc, ref NativeMethods.RECT prc);

        [StructLayout(LayoutKind.Sequential)]
        internal sealed class tagDVTARGETDEVICE
        {
            // Fields
            [MarshalAs(UnmanagedType.U4)]
            internal int tdSize = 0;
            [MarshalAs(UnmanagedType.U2)]
            internal short tdDriverNameOffset = 0;
            [MarshalAs(UnmanagedType.U2)]
            internal short tdDeviceNameOffset = 0;
            [MarshalAs(UnmanagedType.U2)]
            internal short tdPortNameOffset = 0;
            [MarshalAs(UnmanagedType.U2)]
            internal short tdExtDevmodeOffset = 0;
        } // end of class tagDVTARGETDEVICE

        [
            Guid(@"0000010d-0000-0000-C000-000000000046"),
                InterfaceType(ComInterfaceType.InterfaceIsIUnknown)
            ]
        internal interface IViewObject
        {
            // Methods
            void Draw(int dwDrawAspect, int lindex, IntPtr pvAspect, NativeMethods.tagDVTARGETDEVICE ptd, IntPtr hdcTargetDev, IntPtr hdcDraw, NativeMethods.COMRECT lprcBounds, NativeMethods.COMRECT lprcWBounds, IntPtr pfnContinue, int dwContinue);

            [PreserveSig]
            int GetColorSet(int dwDrawAspect, int lindex, IntPtr pvAspect, NativeMethods.tagDVTARGETDEVICE ptd, IntPtr hicTargetDev, NativeMethods.tagLOGPALETTE ppColorSet);

            [PreserveSig]
            int Freeze(int dwDrawAspect, int lindex, IntPtr pvAspect, IntPtr pdwFreeze);

            [PreserveSig]
            int Unfreeze(int dwFreeze);

            void SetAdvise(int aspects, int advf, IAdviseSink pAdvSink);

            void GetAdvise(int[] paspects, int[] advf, IAdviseSink[] pAdvSink);
        } // end of class IViewObject

        [
            InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
                Guid(@"00000127-0000-0000-C000-000000000046")
            ]
        internal interface IViewObject2
        {
            // Methods
            void Draw(int dwDrawAspect, int lindex, IntPtr pvAspect, NativeMethods.tagDVTARGETDEVICE ptd, IntPtr hdcTargetDev, IntPtr hdcDraw, NativeMethods.COMRECT lprcBounds, NativeMethods.COMRECT lprcWBounds, IntPtr pfnContinue, int dwContinue);

            [PreserveSig]
            int GetColorSet(int dwDrawAspect, int lindex, IntPtr pvAspect, NativeMethods.tagDVTARGETDEVICE ptd, IntPtr hicTargetDev, NativeMethods.tagLOGPALETTE ppColorSet);

            [PreserveSig]
            int Freeze(int dwDrawAspect, int lindex, IntPtr pvAspect, IntPtr pdwFreeze);

            [PreserveSig]
            int Unfreeze(int dwFreeze);

            void SetAdvise(int aspects, int advf, IAdviseSink pAdvSink);

            void GetAdvise(int[] paspects, int[] advf, IAdviseSink[] pAdvSink);

            void GetExtent(int dwDrawAspect, int lindex, NativeMethods.tagDVTARGETDEVICE ptd, NativeMethods.tagSIZEL lpsizel);
        } // end of class IViewObject2

        [
            InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
                Guid(@"00000118-0000-0000-C000-000000000046")
            ]
        internal interface IOleClientSite
        {
            // Methods
            [PreserveSig]
            int SaveObject();

            [PreserveSig]
            int GetMoniker(int dwAssign, int dwWhichMoniker, out object moniker);

            [return: MarshalAs(UnmanagedType.Interface)]
            IOleContainer GetContainer();

            [PreserveSig]
            int ShowObject();

            [PreserveSig]
            int OnShowWindow(int fShow);

            [PreserveSig]
            int RequestNewObjectLayout();
        } // end of class IOleClientSite

        [
            Guid(@"0000011B-0000-0000-C000-000000000046"),
                InterfaceType(ComInterfaceType.InterfaceIsIUnknown)
            ]
        internal interface IOleContainer
        {
            // Methods
            [PreserveSig]
            int ParseDisplayName(object pbc, string pszDisplayName, int[] pchEaten, object[] ppmkOut);

            [PreserveSig]
            int EnumObjects(int grfFlags, out IEnumUnknown ppenum);

            [PreserveSig]
            int LockContainer(int fLock);
        } // end of class IOleContainer

        [
            Guid(@"00000100-0000-0000-C000-000000000046"),
                InterfaceType(ComInterfaceType.InterfaceIsIUnknown)
            ]
        internal interface IEnumUnknown
        {
            // Methods
            [PreserveSig]
            int Next(int celt, IntPtr rgelt, IntPtr pceltFetched);

            [PreserveSig]
            int Skip(int celt);

            void Reset();

            void Clone(out IEnumUnknown ppenum);
        } // end of class IEnumUnknown

        [
            SuppressUnmanagedCodeSecurity(),
                InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
                Guid(@"0000010E-0000-0000-C000-000000000046")
            ]
        internal interface IOleDataObject
        {
            // Methods
            [PreserveSig]
            int OleGetData(NativeMethods.FORMATETC pFormatetc, NativeMethods.STGMEDIUM pMedium);

            [PreserveSig]
            int OleGetDataHere(NativeMethods.FORMATETC pFormatetc, NativeMethods.STGMEDIUM pMedium);

            [PreserveSig]
            int OleQueryGetData(NativeMethods.FORMATETC pFormatetc);

            [PreserveSig]
            int OleGetCanonicalFormatEtc(NativeMethods.FORMATETC pformatectIn, NativeMethods.FORMATETC pformatetcOut);

            [PreserveSig]
            int OleSetData(NativeMethods.FORMATETC pFormatectIn, NativeMethods.STGMEDIUM pmedium, int fRelease);

            [return: MarshalAs(UnmanagedType.Interface)]
            IEnumFORMATETC OleEnumFormatEtc(int dwDirection);

            [PreserveSig]
            int OleDAdvise(NativeMethods.FORMATETC pFormatetc, int advf, object pAdvSink, int[] pdwConnection);

            [PreserveSig]
            int OleDUnadvise(int dwConnection);

            [PreserveSig]
            int OleEnumDAdvise(object[] ppenumAdvise);
        } // end of class IOleDataObject

        [
            InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
                Guid(@"00000103-0000-0000-C000-000000000046")
            ]
        internal interface IEnumFORMATETC
        {
            // Methods
            [PreserveSig]
            int Next(int celt, NativeMethods.FORMATETC rgelt, int[] pceltFetched);

            [PreserveSig]
            int Skip(int celt);

            [PreserveSig]
            int Reset();

            [PreserveSig]
            int Clone(IEnumFORMATETC[] ppenum);
        } // end of class IEnumFORMATETC

        [StructLayout(LayoutKind.Sequential)]
        internal class STGMEDIUM
        {
            // Fields
            internal int tymed = 0;
            internal IntPtr unionmember = IntPtr.Zero;
            internal IntPtr pUnkForRelease = IntPtr.Zero;
        } // end of class STGMEDIUM

        // class: IOleObject
        [
            Guid(@"00000112-0000-0000-C000-000000000046"),
                InterfaceType(ComInterfaceType.InterfaceIsIUnknown)
            ]
        internal interface IOleObject
        {
            // Methods
            [PreserveSig]
            int SetClientSite(IOleClientSite pClientSite);

            IOleClientSite GetClientSite();

            [PreserveSig]
            int SetHostNames(string szContainerApp, string szContainerObj);

            [PreserveSig]
            int Close(int dwSaveOption);

            [PreserveSig]
            int SetMoniker(int dwWhichMoniker, object pmk);

            [PreserveSig]
            int GetMoniker(int dwAssign, int dwWhichMoniker, out object moniker);

            [PreserveSig]
            int InitFromData(IOleDataObject pDataObject, int fCreation, int dwReserved);

            [PreserveSig]
            int GetClipboardData(int dwReserved, out IOleDataObject data);

            [PreserveSig]
            int DoVerb(int iVerb, IntPtr lpmsg, IOleClientSite pActiveSite, int lindex, IntPtr hwndParent, NativeMethods.COMRECT lprcPosRect);

            [PreserveSig]
            int EnumVerbs(out IEnumOLEVERB e);

            [PreserveSig]
            int OleUpdate();

            [PreserveSig]
            int IsUpToDate();

            [PreserveSig]
            int GetUserClassID(ref Guid pClsid);

            [PreserveSig]
            int GetUserType(int dwFormOfType, out string userType);

            [PreserveSig]
            int SetExtent(int dwDrawAspect, NativeMethods.tagSIZEL pSizel);

            [PreserveSig]
            int GetExtent(int dwDrawAspect, NativeMethods.tagSIZEL pSizel);

            [PreserveSig]
            int Advise(IAdviseSink pAdvSink, out int cookie);

            [PreserveSig]
            int Unadvise(int dwConnection);

            [PreserveSig]
            int EnumAdvise(out IEnumSTATDATA e);

            [PreserveSig]
            int GetMiscStatus(int dwAspect, out int misc);

            [PreserveSig]
            int SetColorScheme(NativeMethods.tagLOGPALETTE pLogpal);
        } // end of class IOleObject

        // class: IEnumSTATDATA
        [
            Guid(@"00000105-0000-0000-C000-000000000046"),
                InterfaceType(ComInterfaceType.InterfaceIsIUnknown)
            ]
        internal interface IEnumSTATDATA
        {
            // Methods
            void Next(int celt, NativeMethods.STATDATA rgelt, int[] pceltFetched);

            void Skip(int celt);

            void Reset();

            void Clone(IEnumSTATDATA[] ppenum);
        } // end of class IEnumSTATDATA

        [StructLayout(LayoutKind.Sequential)]
        internal sealed class STATDATA
        {
            // Fields
            [MarshalAs(UnmanagedType.U4)]
            internal int advf = 0;
            [MarshalAs(UnmanagedType.U4)]
            internal int dwConnection = 0;
        } // end of class STATDATA

        // class: IEnumOLEVERB
        [
            InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
                Guid(@"00000104-0000-0000-C000-000000000046")
            ]
        internal interface IEnumOLEVERB
        {
            // Methods
            [PreserveSig]
            int Next(int celt, NativeMethods.tagOLEVERB rgelt, int[] pceltFetched);

            [PreserveSig]
            int Skip(int celt);

            void Reset();

            void Clone(out IEnumOLEVERB ppenum);
        } // end of class IEnumOLEVERB

        [StructLayout(LayoutKind.Sequential)]
        internal sealed class tagOLEVERB
        {
            // Fields
            internal int lVerb = 0;
            [MarshalAs(UnmanagedType.LPWStr)]
            internal string lpszVerbName = null;
            [MarshalAs(UnmanagedType.U4)]
            internal int fuFlags = 0;
            [MarshalAs(UnmanagedType.U4)]
            internal int grfAttribs = 0;
        } // end of class tagOLEVERB

        [StructLayout(LayoutKind.Sequential)]
        internal sealed class tagLOGPALETTE
        {
            // Fields
            [MarshalAs(UnmanagedType.U2)]
            internal short palVersion = 0;
            [MarshalAs(UnmanagedType.U2)]
            internal short palNumEntries = 0;
        } // end of class tagLOGPALETTE

        // class: IAdviseSink
        [
            Guid(@"0000010F-0000-0000-C000-000000000046"),
                InterfaceType(ComInterfaceType.InterfaceIsIUnknown)
            ]
        internal interface IAdviseSink
        {
            // Methods
            [PreserveSig]
            void OnDataChange(NativeMethods.FORMATETC pFormatetc, NativeMethods.STGMEDIUM pStgmed);

            [PreserveSig]
            void OnViewChange(int dwAspect, int lindex);

            [PreserveSig]
            void OnRename(object pmk);

            [PreserveSig]
            void OnSave();

            void OnClose();
        } // end of class IAdviseSink

        [StructLayout(LayoutKind.Sequential)]
        internal sealed class FORMATETC
        {
            // Fields
            internal short cfFormat = 0;
            internal short dummy = 0;
            internal IntPtr ptd = IntPtr.Zero;
            internal int dwAspect = 0;
            internal int lindex = 0;
            internal int tymed = 0;
        } // end of class FORMATETC

        [StructLayout(LayoutKind.Sequential)]
        internal sealed class tagSIZEL
        {
            // Fields
            internal int cx = 0;
            internal int cy = 0;
        } // end of class tagSIZEL

        [StructLayout(LayoutKind.Sequential)]
        internal class PICTDESC
        {
            // Fields
            internal int cbSizeOfStruct;
            internal int picType;
            internal IntPtr union1;
            internal int union2;
            internal int union3;

            // Methods
            internal virtual IntPtr GetHPal()
            {
                if (this.picType != 1)
                {
                    goto IL_0021;
                }
                return (IntPtr)(this.union2 | this.union3 << 32 /*0x20*/ );
            IL_0021:
            
                return IntPtr.Zero;
            } // end of method GetHPal

            internal virtual IntPtr GetHandle()
            {
                return this.union1;
            } // end of method GetHandle

            internal static PICTDESC CreateBitmapPICTDESC(IntPtr hbitmap, IntPtr hpal)
            {
                NativeMethods.PICTDESC pICTDESC0;
                pICTDESC0 = new NativeMethods.PICTDESC();
                pICTDESC0.cbSizeOfStruct = 16 /*0x10*/;
                pICTDESC0.picType = 1;
                pICTDESC0.union1 = hbitmap;
                pICTDESC0.union2 = (int)(((long)hpal) & (long)((ulong)0xffffffff));
                pICTDESC0.union3 = (int)(((long)hpal) >> 32 /*0x20*/ );
                return pICTDESC0;
            } // end of method CreateBitmapPICTDESC

            internal static PICTDESC CreateIconPICTDESC(IntPtr hicon)
            {
                NativeMethods.PICTDESC pICTDESC0;
                pICTDESC0 = new NativeMethods.PICTDESC();
                pICTDESC0.cbSizeOfStruct = 12 /*0xc*/;
                pICTDESC0.picType = 3;
                pICTDESC0.union1 = hicon;
                return pICTDESC0;
            } // end of method CreateIconPICTDESC

            internal static PICTDESC CreateEnhMetafilePICTDESC(IntPtr hEMF)
            {
                NativeMethods.PICTDESC pICTDESC0;
                pICTDESC0 = new NativeMethods.PICTDESC();
                pICTDESC0.cbSizeOfStruct = 12 /*0xc*/;
                pICTDESC0.picType = 4;
                pICTDESC0.union1 = hEMF;
                return pICTDESC0;
            } // end of method CreateEnhMetafilePICTDESC

            internal static PICTDESC CreateWinMetafilePICTDESC(IntPtr hmetafile, int x, int y)
            {
                NativeMethods.PICTDESC pICTDESC0;
                pICTDESC0 = new NativeMethods.PICTDESC();
                pICTDESC0.cbSizeOfStruct = 20 /*0x14*/;
                pICTDESC0.picType = 2;
                pICTDESC0.union1 = hmetafile;
                pICTDESC0.union2 = x;
                pICTDESC0.union3 = y;
                return pICTDESC0;
            } // end of method CreateWinMetafilePICTDESC
        } // end of class PICTDESC

        [
            Guid(@"7BF80980-BF32-101A-8BBB-00AA00300CAB"),
                InterfaceType(ComInterfaceType.InterfaceIsIUnknown)
            ]
        internal interface IPicture
        {
            // Methods
            IntPtr GetHandle();

            IntPtr GetHPal();

            [return: MarshalAs(UnmanagedType.I2)]
            short GetPictureType();

            int GetWidth();

            int GetHeight();

            void Render(IntPtr hDC, int x, int y, int cx, int cy, int xSrc, int ySrc, int cxSrc, int cySrc, IntPtr rcBounds);

            void SetHPal(IntPtr phpal);

            IntPtr GetCurDC();

            void SelectPicture(IntPtr hdcIn, IntPtr[] phdcOut, IntPtr[] phbmpOut);

            [return: MarshalAs(UnmanagedType.Bool)]
            bool GetKeepOriginalFormat();

            void SetKeepOriginalFormat(bool pfkeep);

            void PictureChanged();

            [PreserveSig]
            int SaveAsFile(NativeMethods.IStream pstm, int fSaveMemCopy, out int pcbSize);

            int GetAttributes();
        } // end of class System.Windows.Forms.SafeNativeMethods+IPicture

        // class: IStream
        [
            InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
                Guid(@"0000000C-0000-0000-C000-000000000046")
            ]
        internal interface IStream
        {
            // Methods
            int Read(IntPtr buf, int len);

            int Write(IntPtr buf, int len);

            [return: MarshalAs(UnmanagedType.I8)]
            long Seek(long dlibMove, int dwOrigin);

            void SetSize(long libNewSize);

            [return: MarshalAs(UnmanagedType.I8)]
            long CopyTo(IStream pstm, long cb, long[] pcbRead);

            void Commit(int grfCommitFlags);

            void Revert();

            void LockRegion(long libOffset, long cb, int dwLockType);

            void UnlockRegion(long libOffset, long cb, int dwLockType);

            void Stat(NativeMethods.STATSTG pStatstg, int grfStatFlag);

            [return: MarshalAs(UnmanagedType.Interface)]
            IStream Clone();
        } // end of class IStream

        [StructLayout(LayoutKind.Sequential)]
        internal class STATSTG
        {
            // Fields
            [MarshalAs(UnmanagedType.LPWStr)]
            internal string pwcsName = null;
            internal int type = 0;
            [MarshalAs(UnmanagedType.I8)]
            internal long cbSize = 0;
            [MarshalAs(UnmanagedType.I8)]
            internal long mtime = 0;
            [MarshalAs(UnmanagedType.I8)]
            internal long ctime = 0;
            [MarshalAs(UnmanagedType.I8)]
            internal long atime = 0;
            [MarshalAs(UnmanagedType.I4)]
            internal int grfMode = 0;
            [MarshalAs(UnmanagedType.I4)]
            internal int grfLocksSupported = 0;
            internal int clsid_data1 = 0;
            [MarshalAs(UnmanagedType.I2)]
            internal short clsid_data2 = 0;
            [MarshalAs(UnmanagedType.I2)]
            internal short clsid_data3 = 0;
            [MarshalAs(UnmanagedType.U1)]
            internal byte clsid_b0 = 0;
            [MarshalAs(UnmanagedType.U1)]
            internal byte clsid_b1 = 0;
            [MarshalAs(UnmanagedType.U1)]
            internal byte clsid_b2 = 0;
            [MarshalAs(UnmanagedType.U1)]
            internal byte clsid_b3 = 0;
            [MarshalAs(UnmanagedType.U1)]
            internal byte clsid_b4 = 0;
            [MarshalAs(UnmanagedType.U1)]
            internal byte clsid_b5 = 0;
            [MarshalAs(UnmanagedType.U1)]
            internal byte clsid_b6 = 0;
            [MarshalAs(UnmanagedType.U1)]
            internal byte clsid_b7 = 0;
            [MarshalAs(UnmanagedType.I4)]
            internal int grfStateBits = 0;
            [MarshalAs(UnmanagedType.I4)]
            internal int reserved = 0;
        } // end of class STATSTG

        internal class Util
        {
            internal static int MAKELONG(int low, int high)
            {
                return high << 16 /*0x10*/ | (low & 65535 /*0xffff*/ );
            } // end of method MAKELONG

            internal static IntPtr MAKELPARAM(int low, int high)
            {
                return (IntPtr)(high << 16 /*0x10*/ | (low & 65535 /*0xffff*/ ));
            } // end of method MAKELPARAM

            internal static int HIWORD(int n)
            {
                return n >> 16 /*0x10*/ & 65535 /*0xffff*/ ;
            } // end of method HIWORD

            internal static int HIWORD(IntPtr n)
            {
                return Util.HIWORD((int) n );
            } // end of method HIWORD

            internal static int LOWORD(int n)
            {
                return n & 65535 /*0xffff*/;
            } // end of method LOWORD

            internal static int LOWORD(IntPtr n)
            {
                return Util.LOWORD((int)n);
            } // end of method LOWORD

            internal static int SignedHIWORD(IntPtr n)
            {
                return Util.SignedHIWORD((int) n);
            } // end of method SignedHIWORD

            internal static int SignedLOWORD(IntPtr n)
            {
                return Util.SignedLOWORD((int) n);
            } // end of method SignedLOWORD

            internal static int SignedHIWORD(int n)
            {
                int n0;
                n0 = (short)(n >> 16 /*0x10*/ & 65535 /*0xffff*/ );
                return n0;
            } // end of method SignedHIWORD

            internal static int SignedLOWORD(int n)
            {
                int n0;
                n0 = (short)(n & 65535 /*0xffff*/ );
                return n0;
            } // end of method SignedLOWORD

            /// <summary>
            /// Computes the string size that should be passed to a typical Win32 call.
            /// This will be the character count under NT and the ubyte count for Win95.
            /// </summary>
            /// <param name="s">
            /// The string to get the size of.
            /// </param>
            /// <returns>
            /// The count of characters or bytes, depending on what the pinvoke
            /// call wants.
            /// </returns>
            internal static int GetPInvokeStringLength(string s)
            {
                if (s != null)
                {
                    goto IL_0005;
                }
                return 0;
            IL_0005:
                ;
                if (Marshal.SystemDefaultCharSize != 2)
                {
                    goto IL_0014;
                }
                return s.Length;
            IL_0014:

                if (s.Length != 0)
                {
                    goto IL_001e;
                }
                return 0;
            IL_001e:
                ;
                if (s.IndexOf('\0') <= -1 /*0xffffffff*/ )
                {
                    goto IL_002f;
                }
                return Util.GetEmbededNullStringLengthAnsi(s);
            IL_002f:
                ;
                return Util.lstrlen(s);
            } // end of method GetPInvokeStringLength

            private static int GetEmbededNullStringLengthAnsi(string s)
            {
                int length0;
                string s1;
                string s2;
                length0 = s.IndexOf('\0');
                if (length0 <= -1 /*0xffffffff*/)
                {
                    goto IL_002f;
                }
                s1 = s.Substring(0, length0);
                s2 = s.Substring((length0 + 1));
                return Util.GetPInvokeStringLength(s1) + Util.GetEmbededNullStringLengthAnsi(s2) + 1;
            IL_002f:
                ;
                return Util.GetPInvokeStringLength(s);
            } // end of method GetEmbededNullStringLengthAnsi

            [DllImport("kernel32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
            private static extern int lstrlen(string s);

            [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
            internal static extern int RegisterWindowMessage(string msg);
        } // end of class Util

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        internal static extern int IntersectClipRect(IntPtr hDC, int x1, int y1, int x2, int y2);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        internal static extern int GetClipBox(IntPtr hDC, ref NativeMethods.RECT lpRect);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        internal static extern int GetClipRgn(IntPtr hDC, IntPtr hRgn);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        internal static extern int SelectClipRgn(IntPtr hDC, IntPtr hRgn);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        internal static extern IntPtr CreateCompatibleDC(IntPtr hDC);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        internal static extern IntPtr CreateCompatibleBitmap(IntPtr hDC, int width, int height);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        internal static extern IntPtr GetWindowDC(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        internal static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        internal static extern int GetVersion();

        internal const int DVASPECT_CONTENT = 1 /*0x0001*/;
        internal const int DVASPECT_TRANSPARENT = 32 /*0x0020*/;
        internal const int DVASPECT_OPAQUE = 16 /*0x0010*/;
        internal const int BITSPIXEL = 12 /*0x000C*/;
        internal const int LOGPIXELSX = 88 /*0x0058*/;
        internal const int LOGPIXELSY = 90 /*0x005A*/;
        internal const int PLANES = 14 /*0x000E*/;

        // RichTextBox3
        [DocumentationExclude(), StructLayout(LayoutKind.Sequential)]

        internal class FORMATRANGE
        {
            public IntPtr hdc = IntPtr.Zero;
            public IntPtr hdcTarget = IntPtr.Zero;
            public RECT rc = new RECT();
            public RECT rcPage = new RECT();
            public CHARRANGE chrg = new CHARRANGE();
        }

        public enum ToolTipStyles
        {
            /// <summary>
            /// Represents always tip
            /// </summary>
            TTS_ALWAYSTIP = 0x01,

            /// <summary>
            /// Represents NOPREFIX
            /// </summary>
            TTS_NOPREFIX = 0x02,

            /// <summary>
            /// Represents NOANIMATE
            /// </summary>
            TTS_NOANIMATE = 0x10,

            /// <summary>
            /// Represents NOFADE
            /// </summary>
            TTS_NOFADE = 0x20,

            /// <summary>
            /// Represents Balloon
            /// </summary>
            TTS_BALLOON = 0x40
        }

        #region TOOLINFO
        [StructLayout(LayoutKind.Sequential)]
        internal struct TOOLINFO_D
        {
            public int cbSize;
            public int uFlags;
            public IntPtr hwnd;
            public IntPtr uId;       // UINT_PTR
            public RECT rect;
            public IntPtr hinst;

            public IntPtr lpszText;
            public IntPtr lParam;
        }
        #endregion

        public enum TTF : int
        {
            /// <summary>
            /// Represents IDISHWND
            /// </summary>
            TTF_IDISHWND = 0x0001,
            
            /// <summary>
            /// Represents SUBCLASS          
            /// </summary>
            TTF_SUBCLASS = 0x0010,

            /// <summary>
            /// Represents TRACK
            /// </summary>
            TTF_TRACK = 0x0020,
        }

        [DocumentationExclude(), StructLayout(LayoutKind.Sequential)]

        internal struct CHARRANGE
        {
            public int cpMin;
            public int cpMax;
        }

        [DllImport("user32", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, bool wParam, FORMATRANGE fr);

        /// <summary>
        /// Interop call to get the LCID of the current culture.
        /// </summary>
        /// <returns >Returns UserDefaultLCID </returns>
        [DllImport("Kernel32.dll", CallingConvention = CallingConvention.Winapi)]
        internal static extern int GetUserDefaultLCID();

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
                    bottom = RegionCracker.ToInt(buffer, (cbHeader + 12 /*0xc*/ ));
                    nativeRects[n] = new NativeMethods.RECT(left, top, right, bottom);
                    cbHeader = cbHeader + 16 /*0x10*/;
                }
                return nativeRects;
            } // end of method GetRects

            private static int ToInt(byte[] buffer, int offset)
            {
                int n0;
                n0 = (int)buffer[offset];
                n0 = n0 | ((int)buffer[(offset + 1)]) << 8;
                n0 = n0 | (int)buffer[(offset + 2)] << 16 /*0x10*/;
                n0 = n0 | (int)buffer[(offset + 3)] << 24 /*0x18*/;
                return n0;
            } // end of method ToInt
        }

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

        /*
         * public IntPtr hdc = IntPtr.Zero
                    public bool fErase = false;
                    public int rcPaint_left = 0;
                    public int rcPaint_top = 0;
                    public int rcPaint_right = 0;
                    public int rcPaint_bottom = 0;
                    public bool fRestore = false;
                    public bool fIncUpdate = false;
                    public int reserved1 = 0;
                    public int reserved2 = 0;
                    public int reserved3 = 0;
                    public int reserved4 = 0;
                    public int reserved5 = 0;
                    public int reserved6 = 0;
                    public int reserved7 = 0;
                    public int reserved8 = 0;
        */

        /// <summary>
        /// Specifies the type of character information the user wants to retrieve.
        /// </summary>
        [DocumentationExclude(), Flags]
        internal enum StringInfoType : uint
        {
            /// <summary>
            /// Retrieves character type info.
            /// </summary>
            CT_TYPE1 = 1,

            /// <summary>
            /// Retrieves bi-directional layout info.
            /// </summary>
            CT_TYPE2 = 2,

            /// <summary>
            /// Retrieves text processing info.
            /// </summary>
            CT_TYPE3 = 4
        }

        /// <summary>
        /// These types support ANSI C and POSIX (LC_CTYPE) character-typing functions.
        /// A combination of these values is returned in the array pointed to by the lpCharType parameter
        /// when the dwInfoType parameter is set to CT_CTYPE1.
        /// </summary>
        [DocumentationExclude(), Flags]
        internal enum StringInfoCtype1 : ushort
        {
            /// <summary>
            /// Represents Uppercase
            /// </summary>
            C1_UPPER = 0x0001,

            /// <summary>
            /// represents Lowercase
            /// </summary>
            C1_LOWER = 0x0002,

            /// <summary>
            /// Represents Decimal digits
            /// </summary>
            C1_DIGIT = 0x0004,

            /// <summary>
            /// Represents Space characters
            /// </summary>
            C1_SPACE = 0x0008,

            /// <summary>
            /// Represents Punctuation
            /// </summary>
            C1_PUNCT = 0x0010,

            /// <summary>
            /// Represents Control characters
            /// </summary>
            C1_CNTRL = 0x0020,

            /// <summary>
            /// Represents  Blank characters
            /// </summary>
            C1_BLANK = 0x0040,

            /// <summary>
            /// Represents Hexadecimal digits
            /// </summary>
            C1_XDIGIT = 0x0080, 

            /// <summary>
            /// Represents Any linguistic character: alphabetic, syllabary, or ideographic.
            /// </summary>
            C1_ALPHA = 0x0100, 
        }

        /// <summary>
        /// These types support proper layout of Unicode text. The direction attributes are assigned
        /// so that the bidirectional layout algorithm standardized by Unicode produces accurate results.
        /// These types are mutually exclusive.
        /// </summary>
        [DocumentationExclude(), Flags]
        internal enum StringInfoCtype2 : ushort
        {
            /// <summary>
            /// Left to right 
            /// </summary>
            C2_LEFTTORIGHT = 0x0001,
            
            /// <summary>
            /// Right to left
            /// </summary>
            C2_RIGHTTOLEFT = 0x0002,

            /// <summary>
            /// European number, European digit
            /// </summary>
            C2_EUROPENUMBER = 0x0003,

            /// <summary>
            /// European numeric separator
            /// </summary>
            C2_EUROPESEPARATOR = 0x0004,

            /// <summary>
            /// European numeric terminator
            /// </summary>
            C2_EUROPETERMINATOR = 0x0005,

            /// <summary>
            /// Arabic number
            /// </summary>
            C2_ARABICNUMBER = 0x0006,

            /// <summary>
            /// Common numeric separator
            /// </summary>
            C2_COMMONSEPARATOR = 0x0007,

            /// <summary>
            /// Block separator
            /// </summary>
            C2_BLOCKSEPARATOR = 0x0008,

            /// <summary>
            /// Segment separator
            /// </summary>
            C2_SEGMENTSEPARATOR = 0x0009,

            /// <summary>
            /// White space
            /// </summary>
            C2_WHITESPACE = 0x000A,

            /// <summary>
            /// Other neutrals 
            /// </summary>
            C2_OTHERNEUTRAL = 0x000B,

            /// <summary>
            ///  No implicit directionality (for example, control codes)
            /// </summary>
            C2_NOTAPPLICABLE = 0x0000 
        }

        /// <summary>
        /// These types are intended to be placeholders for extensions to the POSIX types required for general text processing
        /// or for the standard C library functions. A combination of these values is returned when dwInfoType is set to CT_CTYPE3.
        /// </summary>
        [DocumentationExclude(), Flags]
        internal enum StringInfoCtype3 : ushort
        {
            /// <summary>
            /// Diacritic nonspacing mark 
            /// </summary>
            C3_DIACRITIC = 0x0002,

            /// <summary>
            /// Vowel nonspacing mark
            /// </summary>
            C3_VOWELMARK = 0x0004,

            /// <summary>
            /// Represents Symbol
            /// </summary>
            C3_SYMBOL = 0x0008,

            /// <summary>
            /// Katakana character
            /// </summary>
            C3_KATAKANA = 0x0010,

            /// <summary>
            /// Hiragana character
            /// </summary>
            C3_HIRAGANA = 0x0020,

            /// <summary>
            /// Half-width (narrow) character
            /// </summary>
            C3_HALFWIDTH = 0x0040,

            /// <summary>
            /// Full-width (wide) character
            /// </summary>
            C3_FULLWIDTH = 0x0080,

            /// <summary>
            /// Ideographic character
            /// </summary>
            C3_IDEOGRAPH = 0x0100,

            /// <summary>
            /// Arabic Kashida character
            /// </summary>
            C3_KASHIDA = 0x0200, 

            /// <summary>
            /// Punctuation which is counted as part of the word (Kashida, hyphen, feminine/masculine ordinal indicators, equal sign, and so forth)
            /// </summary>
            C3_LEXICAL = 0x0400,

            /// <summary>
            /// All linguistic characters (alphabetical, syllabary, and ideographic)
            /// </summary>
            C3_ALPHA = 0x8000, 

            /// <summary>
            /// Not applicable
            /// </summary>
            C3_NOTAPPLICABLE = 0x0000
        }

        /// <summary>
        /// Retrieves character-type information for the characters in the specified source string.
        /// </summary>
        /// <param name="Locale">Value that specifies the locale identifier.</param>
        /// <param name="dwInfoType">Value that specifies the type of character information the user wants to retrieve.</param>
        /// <param name="lpSrcStr">Pointer to the string for which character types are requested.</param>
        /// <param name="cchSrc">Size, in characters, of the string pointed to by the lpSrcStr parameter.</param>
        /// <param name="lpCharType">Pointer to an array of 16-bit values.</param>
        /// <returns>Boolean result, indicates success of WinAPI call.</returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Winapi)]
        public static extern bool GetStringTypeEx(
            uint Locale, StringInfoType dwInfoType, string lpSrcStr, int cchSrc, [Out] ushort[] lpCharType);

        /// <summary>
        /// Specifies which type of input the application processes.
        /// </summary>
        [Flags]
        internal enum DialogCodes : int
        {
            /// <summary>
            /// Direction keys
            /// </summary>
            DLGC_WANTARROWS = 0x0001,

            /// <summary>
            /// TAB key
            /// </summary>
            DLGC_WANTTAB = 0x0002,

            /// <summary>
            /// All keyboard input
            /// </summary>
            DLGC_WANTALLKEYS = 0x0004,

            /// <summary>
            /// All keyboard input (the application passes this message in the MSG structure to the control)
            /// </summary>
            DLGC_WANTMESSAGE = 0x0004,

            /// <summary>
            /// EM_SETSEL messages
            /// </summary>
            DLGC_HASSETSEL = 0x0008,

            /// <summary>
            /// Default push button
            /// </summary>
            DLGC_DEFPUSHBUTTON = 0x0010,

            /// <summary>
            ///  Non-default push button
            /// </summary>
            DLGC_UNDEFPUSHBUTTON = 0x0020,

            /// <summary>
            /// Radio button
            /// </summary>
            DLGC_RADIOBUTTON = 0x0040,

            /// <summary>
            /// WM_CHAR messages
            /// </summary>
            DLGC_WANTCHARS = 0x0080,

            /// <summary>
            /// Represents Static control
            /// </summary>
            DLGC_STATIC = 0x0100,

            /// <summary>
            /// Represents Button
            /// </summary>
            DLGC_BUTTON = 0x2000
        }

        #region Windows Hook Codes enum
        internal enum WindowsHookCodes
        {
            /// <summary>
            /// Represents MSGFILTER
            /// </summary>
            WH_MSGFILTER = (-1),

            /// <summary>
            /// Represents JOURNALRECORD
            /// </summary>
            WH_JOURNALRECORD = 0,

            /// <summary>
            /// Represents JOURNALPLAYBACK
            /// </summary>
            WH_JOURNALPLAYBACK = 1,

            /// <summary>
            /// Represents KEYBOARD
            /// </summary>
            WH_KEYBOARD = 2,

            /// <summary>
            /// Represents GETMESSAGE
            /// </summary>
            WH_GETMESSAGE = 3,

            /// <summary>
            /// Represents CALLWNDPROC
            /// </summary>
            WH_CALLWNDPROC = 4,

            /// <summary>
            /// Represents CBT
            /// </summary>
            WH_CBT = 5,

            /// <summary>
            /// Represents SYSMSGFILTER
            /// </summary>
            WH_SYSMSGFILTER = 6,

            /// <summary>
            /// Represents MOUSE
            /// </summary>
            WH_MOUSE = 7,

            /// <summary>
            /// Represents HARDWARE
            /// </summary>
            WH_HARDWARE = 8,

            /// <summary>
            /// Represents DEBUG
            /// </summary>
            WH_DEBUG = 9,

            /// <summary>
            /// Represents SHELL
            /// </summary>
            WH_SHELL = 10,

            /// <summary>
            /// Represents FOREGROUNDIDLE
            /// </summary>
            WH_FOREGROUNDIDLE = 11,

            /// <summary>
            /// Represents CALLWNDPROCRET
            /// </summary>
            WH_CALLWNDPROCRET = 12,

            /// <summary>
            /// Represents KEYBOARD_LL
            /// </summary>
            WH_KEYBOARD_LL = 13,

            /// <summary>
            /// Represents MOUSE_LL
            /// </summary>
            WH_MOUSE_LL = 14
        }
        #endregion

        /// <summary>
        /// Determines whether a key is up or down at the time when the function is called and whether the key was pressed after a previous call to GetAsyncKeyState. 
        /// </summary>
        /// <param name="key">Specifies one of 256 possible virtual-key codes.</param>
        /// <returns> Returns AsyncKeyState</returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        internal static extern ushort GetAsyncKeyState(Keys key);

        [DllImport("user32.dll")]
        internal static extern IntPtr BeginDeferWindowPos(int nNumWindows);

        [DllImport("user32.dll")]
        internal static extern IntPtr DeferWindowPos(IntPtr hWinPosInfo, IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);

        [DllImport("user32.dll")]
        internal static extern bool EndDeferWindowPos(IntPtr hWinPosInfo);

        [DllImport("user32.dll")]
        internal static extern IntPtr GetDCEx(IntPtr hWnd, IntPtr hrgnClip, uint flags);

        #region TernaryRasterOperations
        internal enum TernaryRasterOperations
        {
            /// <summary>
            /// dest = source
            /// </summary>
            SRCCOPY = 0x00CC0020,

            /// <summary>
            /// dest = source OR dest
            /// </summary>
            SRCPAINT = 0x00EE0086,

            /// <summary>
            /// dest = source AND dest
            /// </summary>
            SRCAND = 0x008800C6,
            
            /// <summary>
            /// dest = source XOR dest
            /// </summary>
            SRCINVERT = 0x00660046,

            /// <summary>
            /// dest = source AND (NOT dest )
            /// </summary>
            SRCERASE = 0x00440328,

            /// <summary>
            /// dest = (NOT source)
            /// </summary>
            NOTSRCCOPY = 0x00330008,

            /// <summary>
            /// dest = (NOT src) AND (NOT dest)
            /// </summary>
            NOTSRCERASE = 0x001100A6,

            /// <summary>
            /// dest = (source AND pattern)
            /// </summary>
            MERGECOPY = 0x00C000CA,

            /// <summary>
            /// dest = (NOT source) OR dest
            /// </summary>
            MERGEPAINT = 0x00BB0226,

            /// <summary>
            /// dest = pattern
            /// </summary>
            PATCOPY = 0x00F00021,

            /// <summary>
            /// dest = DPSnoo
            /// </summary>
            PATPAINT = 0x00FB0A09,

            /// <summary>
            /// dest = pattern XOR dest
            /// </summary>
            PATINVERT = 0x005A0049,

            /// <summary>
            /// dest = (NOT dest)
            /// </summary>
            DSTINVERT = 0x00550009,

            /// <summary>
            /// dest = BLACK
            /// </summary>
            BLACKNESS = 0x00000042,

            /// <summary>
            /// dest = WHITE
            /// </summary>
            WHITENESS = 0x00FF0062,
        };
        #endregion

        #region DCFlags
        internal enum DCFlags
        {
            /// <summary>
            /// Represents WINDOW
            /// </summary>
            DCX_WINDOW = 0x0001,

            /// <summary>
            /// Represents CACHE
            /// </summary>
            DCX_CACHE = 0x0002,

            /// <summary>
            /// Represents INTERSECTRGN
            /// </summary>
            DCX_INTERSECTRGN = 0x0080,

            /// <summary>
            /// Represents LOCKWINDOWUPDATE
            /// </summary>
            DCX_LOCKWINDOWUPDATE = 0x0400,

            /// <summary>
            /// Represents PARENTCLIP
            /// </summary>
            DCX_PARENTCLIP = 0x0020,

            /// <summary>
            /// Represents CLIPSIBLINGS
            /// </summary>
            DCX_CLIPSIBLINGS = 0x00000010,

            /// <summary>
            /// Represents CLIPCHILDREN
            /// </summary>
            DCX_CLIPCHILDREN = 0x000008,

            /// <summary>
            ///  Represents EXCLUDERGN
            /// </summary>
            DCX_EXCLUDERGN = 0x00000040
        }
        #endregion

        #region LogBrush Styles
        internal enum LogBrushStyle
        {
            /// <summary>
            /// Represents SOLID
            /// </summary>
            BS_SOLID = 0,

            /// <summary>
            /// Represents HOLLOW
            /// </summary>
            BS_HOLLOW = 1,

            /// <summary>
            /// Represents HATCHED
            /// </summary>
            BS_HATCHED = 2,

            /// <summary>
            /// Represents PATTERN
            /// </summary>
            BS_PATTERN = 3,

            /// <summary>
            /// Represents DIBPATTERN
            /// </summary>
            BS_DIBPATTERN = 5,

            /// <summary>
            /// Represents DIBPATTERNPT
            /// </summary>
            BS_DIBPATTERNPT = 6,

            /// <summary>
            /// Represents PATTERN8X8
            /// </summary>
            BS_PATTERN8X8 = 7,

            /// <summary>
            /// Represents DIBPATTERN8X8 
            /// </summary>
            BS_DIBPATTERN8X8 = 8
        }
        #endregion

#if ( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
		[ DllImport( "ole32.dll") ]
		internal static extern int GetRunningObjectTable( int reserved, out UCOMIRunningObjectTable prot );
		
		[ DllImport( "ole32.dll" ) ]
		internal static extern int CreateBindCtx( int reserved, out UCOMIBindCtx ppbc );
#else
        [DllImport("ole32.dll")]
        internal static extern int GetRunningObjectTable(int reserved, out IRunningObjectTable prot);

        [DllImport("ole32.dll")]
        internal static extern int CreateBindCtx(int reserved, out IBindCtx ppbc);
#endif

        #region  DWM API
        internal const int WM_DWMNCRENDERINGCHANGED = 0x031F;
        internal const int WM_DWMCOMPOSITIONCHANGED = 0x031E;
        internal const int WM_DWMCOLORIZATIONCOLORCHANGED = 0x0320;
        internal const int S_OK = 0;

        [DllImport("dwmapi.dll", PreserveSig = false)]
        internal static extern bool DwmIsCompositionEnabled();

        [DllImport("dwmapi.dll")]
       internal static extern IntPtr DwmIsCompositionEnabled(ref bool isEnabled);

        [DllImport("DwmApi.dll")]
        internal static extern int DwmEnableComposition(
            bool fEnabled);

        [DllImport("dwmapi.dll", PreserveSig = false)]
        internal static extern bool DwmDefWindowProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, out int plResult);

        [DllImport("dwmapi.dll", PreserveSig = false)]
        internal static extern void DwmExtendFrameIntoClientArea(IntPtr hWnd, MARGINS pMargins);

        [DllImport("dwmapi.dll", PreserveSig = false)]
        internal static extern void DwmGetColorizationColor(
            out int pcrColorization,
            [MarshalAs(UnmanagedType.Bool)]out bool pfOpaqueBlend);

        [DllImport("DwmApi.dll", EntryPoint = "#103")]
        internal static extern int DwmRestartComposition();

        [DllImport("dwmapi.dll")]
        internal static extern int DwmGetWindowAttribute(IntPtr hWnd, uint dwAttribute, out int pvAttribute, uint cbAttribute);

        [System.Security.SuppressUnmanagedCodeSecurity()]
        [DllImport("dwmapi.dll")]
        internal static extern int DwmSetWindowAttribute(IntPtr hWnd, uint dwAttribute, IntPtr pvAttribute, uint cbAttribute);

        /// <summary>
        /// Window attributes
        /// </summary>
        internal enum DWMWINDOWATTRIBUTE
        {
            /// <summary>
            /// [get] Is non-client rendering enabled/disabled
            /// </summary>
            DWMWA_NCRENDERING_ENABLED = 1, 

            /// <summary>
            /// [set] Non-client rendering policy
            /// </summary>
            DWMWA_NCRENDERING_POLICY,

            /// <summary>
            /// [set] Potentially enable/forcibly disable transitions
            /// </summary>
            DWMWA_TRANSITIONS_FORCEDISABLED, 

            /// <summary>
            /// [set] Allow contents rendered in the non-client area to be visible on the DWM-drawn frame.
            /// </summary>
            DWMWA_ALLOW_NCPAINT,

            /// <summary>
            /// [get] Bounds of the caption button area in window-relative space.
            /// </summary>
            DWMWA_CAPTION_BUTTON_BOUNDS,

            /// <summary>
            /// [set] Is non-client content RTL mirrored
            /// </summary>
            DWMWA_NONCLIENT_RTL_LAYOUT,

            /// <summary>
            /// [set] Force this window to display iconic thumbnails.
            /// </summary>
            DWMWA_FORCE_ICONIC_REPRESENTATION,

            /// <summary>
            /// [set] Designates how Flip3D will treat the window.
            /// </summary>
            DWMWA_FLIP3D_POLICY, 

            /// <summary>
            /// [get] Gets the extended frame bounds rectangle in screen space
            /// </summary>
            DWMWA_EXTENDED_FRAME_BOUNDS,

            /// <summary>
            /// Represents Last
            /// </summary>
            DWMWA_LAST
        }

        /// <summary>
        ///  Non-client rendering policy attribute values
        /// </summary>
        internal enum DWMNCRENDERINGPOLICY
        {
            /// <summary>
            /// Enable/disable non-client rendering based on window style
            /// </summary>
            DWMNCRP_USEWINDOWSTYLE,

            /// <summary>
            /// Disabled non-client rendering; window style is ignored
            /// </summary>
            DWMNCRP_DISABLED,

            /// <summary>
            ///  Enabled non-client rendering; window style is ignored
            /// </summary>
            DWMNCRP_ENABLED,

            /// <summary>
            /// Represents last
            /// </summary>
            DWMNCRP_LAST
        }

        /// <summary>
        /// Enables/disable dwm non-client rendering
        /// </summary>
        /// <param name="hWnd">Represents Handle </param>
        /// <param name="enable">True to enable, False to disable.</param>
        /// <returns>True if Set NCRendering is successful, false otherwise.</returns>
        internal static bool SetDwmNCRendering(IntPtr hWnd, bool enable)
        {
            DWMNCRENDERINGPOLICY ncrp = enable ? DWMNCRENDERINGPOLICY.DWMNCRP_ENABLED : DWMNCRENDERINGPOLICY.DWMNCRP_DISABLED;
            IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(int)));
            Marshal.WriteInt32(ptr, (int)ncrp);
            int hr = DwmSetWindowAttribute(hWnd, (uint)DWMWINDOWATTRIBUTE.DWMWA_NCRENDERING_POLICY, ptr, (uint)Marshal.SizeOf(typeof(int)));
            Marshal.FreeHGlobal(ptr);

            if (hr == S_OK)
                return true;

            return false;
        }

        internal static bool IsDwmNCRenderingEnabled(IntPtr hWnd)
        {
            if (!IsCompositionEnabled())
                return false;
            int enabled;
            DwmGetWindowAttribute(hWnd, (uint)DWMWINDOWATTRIBUTE.DWMWA_NCRENDERING_ENABLED, out enabled, (uint)Marshal.SizeOf(typeof(int)));
            if (enabled > 0)
                return true;

            return false;
        }

        /// <summary>
        /// Indicates if composition is enabled
        /// </summary>
        /// <returns>True if composition is enabled. False otherwise.</returns>
        public static bool IsCompositionEnabled()
        {
            if (Environment.OSVersion.Version.Major < 6)
                return false;

            bool compEnabled = false;
            if (DwmIsCompositionEnabled(ref compEnabled) == new IntPtr(S_OK))
                return compEnabled;

            return false;
        }
        #endregion

        # region [ MultiTouch ]

        // One of the fields in GESTUREINFO structure is type of Int64 (8 bytes).
        // The relevant gesture information is stored in lower 4 bytes. This
        // bit mask is used to get 4 lower bytes from this argument.
        internal const Int64 ULL_ARGUMENTS_BIT_MASK = 0x00000000FFFFFFFF;

        //-----------------------------------------------------------------------
        // Multitouch/Touch glue (from winuser.h file)
        // Since the managed layer between C# and WinAPI functions does not 
        // exist at the moment for multi-touch related functions this part of 
        // code is required to replicate definitions from winuser.h file.
        //-----------------------------------------------------------------------
        // Touch event window message constants [winuser.h]
        internal const int WM_GESTURENOTIFY = 0x011A;
        internal const int WM_GESTURE = 0x0119;

        internal const int GC_ALLGESTURES = 0x00000001;

        // Gesture IDs 
        internal const int GID_BEGIN = 1;
        internal const int GID_END = 2;
        internal const int GID_ZOOM = 3;
        internal const int GID_PAN = 4;
        internal const int GID_ROTATE = 5;
        internal const int GID_TWOFINGERTAP = 6;
        internal const int GID_PRESSANDTAP = 7;
        internal const int GID_PRESSANDHOLD = 8;
        // Gesture flags - GESTUREINFO.dwFlags
        internal const int GF_BEGIN = 0x00000001;
        internal const int GF_INERTIA = 0x00000002;
        internal const int GF_END = 0x00000004;

        // Touch event window message constants [winuser.h]
        internal const int WM_TOUCHMOVE = 0x0240;
        internal const int WM_TOUCHDOWN = 0x0241;
        internal const int WM_TOUCHUP = 0x0242;

        // Touch event flags ((TOUCHINPUT.dwFlags) [winuser.h]
        internal const int TOUCHEVENTF_MOVE = 0x0001;
        internal const int TOUCHEVENTF_DOWN = 0x0002;
        internal const int TOUCHEVENTF_UP = 0x0004;
        internal const int TOUCHEVENTF_INRANGE = 0x0008;
        internal const int TOUCHEVENTF_PRIMARY = 0x0010;
        internal const int TOUCHEVENTF_NOCOALESCE = 0x0020;
        internal const int TOUCHEVENTF_PEN = 0x0040;

        // Touch input mask values (TOUCHINPUT.dwMask) [winuser.h]
        internal const int TOUCHINPUTMASKF_TIMEFROMSYSTEM = 0x0001; // the dwTime field contains a system generated value
        internal const int TOUCHINPUTMASKF_EXTRAINFO = 0x0002; // the dwExtraInfo field is valid
        internal const int TOUCHINPUTMASKF_CONTACTAREA = 0x0004; // the cxContact and cyContact fields are valid


        //
        // Gesture configuration structure
        //   - Used in SetGestureConfig and GetGestureConfig
        //   - Note that any setting not included in either GESTURECONFIG.dwWant
        //     or GESTURECONFIG.dwBlock will use the parent window's preferences
        //     or system defaults.
        //
        // Touch API defined structures [winuser.h]
        [StructLayout(LayoutKind.Sequential)]
        public struct GESTURECONFIG
        {
            public int dwID;    // gesture ID
            public int dwWant;  // settings related to gesture ID that are to be
            // turned on
            public int dwBlock; // settings related to gesture ID that are to be
            // turned off
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct POINTS
        {
            public short x;
            public short y;
        }

        //
        // Gesture information structure
        //   - Pass the HGESTUREINFO received in the WM_GESTURE message lParam 
        //     into the GetGestureInfo function to retrieve this information.
        //   - If cbExtraArgs is non-zero, pass the HGESTUREINFO received in 
        //     the WM_GESTURE message lParam into the GetGestureExtraArgs 
        //     function to retrieve extended argument information.
        //
        [StructLayout(LayoutKind.Sequential)]
        public struct GESTUREINFO
        {
            public int cbSize;           // size, in bytes, of this structure
            // (including variable length Args 
            // field)
            public int dwFlags;          // see GF_* flags
            public int dwID;             // gesture ID, see GID_* defines
            public IntPtr hwndTarget;    // handle to window targeted by this 
            // gesture
            [MarshalAs(UnmanagedType.Struct)]
            internal POINTS ptsLocation; // current location of this gesture
            public int dwInstanceID;     // internally used
            public int dwSequenceID;     // internally used
            public Int64 ullArguments;   // arguments for gestures whose 
            // arguments fit in 8 BYTES
            public int cbExtraArgs;      // size, in bytes, of extra arguments, 
            // if any, that accompany this gesture
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct TOUCHINPUT
        {
            public int x;
            public int y;
            public System.IntPtr hSource;
            public int dwID;
            public int dwFlags;
            public int dwMask;
            public int dwTime;
            public System.IntPtr dwExtraInfo;
            public int cxContact;
            public int cyContact;
        }
        // Currently touch/multitouch access is done through unmanaged code
        // We must p/invoke into user32 [winuser.h]
        [DllImport("user32")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetGestureConfig(IntPtr hWnd, int dwReserved, int cIDs, ref GESTURECONFIG pGestureConfig, int cbSize);

        [DllImport("user32")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetGestureInfo(IntPtr hGestureInfo, ref GESTUREINFO pGestureInfo);

        [DllImport("user32")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RegisterTouchWindow(System.IntPtr hWnd, ulong ulFlags);

        [DllImport("user32")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetTouchInputInfo(System.IntPtr hTouchInput, int cInputs, [In, Out] TOUCHINPUT[] pInputs, int cbSize);

        [DllImport("user32")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern void CloseTouchInputHandle(System.IntPtr lParam);


        // size of GESTURECONFIG structure
        internal static int _gestureConfigSize;
        // size of GESTUREINFO structure
        internal static int _gestureInfoSize;
        internal static int touchInputSize;

        [SecurityPermission(SecurityAction.Demand)]
        public static void SetupStructSizes()
        {
            // Both GetGestureCommandInfo and GetTouchInputInfo need to be
            // passed the size of the structure they will be filling
            // we get the sizes upfront so they can be used later.
            _gestureConfigSize = Marshal.SizeOf(new GESTURECONFIG());
            _gestureInfoSize = Marshal.SizeOf(new GESTUREINFO());
            touchInputSize = Marshal.SizeOf(new TOUCHINPUT());
        }

        public static void SetGestureConfig(IntPtr Handle)
        {
            // This is the right place to define the list of gestures
            // that this application will support. By populating 
            // GESTURECONFIG structure and calling SetGestureConfig 
            // function. We can choose gestures that we want to 
            // handle in our application. In this app we decide to 
            // handle all gestures.
            GESTURECONFIG gc = new GESTURECONFIG();
            gc.dwID = 0;                // gesture ID
            gc.dwWant = GC_ALLGESTURES; // settings related to gesture
            // ID that are to be turned on
            gc.dwBlock = 0; // settings related to gesture ID that are
            // to be     

            // We must p/invoke into user32 [winuser.h]
            bool bResult = SetGestureConfig(
                Handle, // window for which configuration is specified
                0,      // reserved, must be 0
                1,      // count of GESTURECONFIG structures
                ref gc, // array of GESTURECONFIG structures, dwIDs 
                // will be processed in the order specified 
                // and repeated occurances will overwrite 
                // previous ones
                _gestureConfigSize // sizeof(GESTURECONFIG)
            );

            if (!bResult)
            {
                throw new Exception("Error in execution of SetGestureConfig");
            }
        }

        // Taken from GCI_ROTATE_ANGLE_FROM_ARGUMENT.
        // Converts from "binary radians" to traditional radians.
        static internal double ArgToRadians(Int64 arg)
        {
            return ((((double)(arg) / 65535.0) * 4.0 * 3.14159265) - 2.0 * 3.14159265);
        }
        static internal int LoWord(int number)
        {
            return number & 0xffff;
        }

        # endregion
    }
}