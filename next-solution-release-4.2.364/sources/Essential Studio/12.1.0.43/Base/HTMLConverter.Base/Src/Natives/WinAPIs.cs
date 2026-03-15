#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Text;
using System.Collections;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text.RegularExpressions;


namespace Syncfusion.HtmlConverter.Natives
{
    #region WINAPIS
    /// <summary>
    /// Windows API constants and functions
    /// </summary>
    internal sealed class WinApis
    {
        
        public const uint MAX_PATH = 512;
        public const uint STGM_READ = 0x00000000;
        public const uint SHDVID_SSLSTATUS = 33;
        public const int GWL_WNDPROC = -4;
        public const uint KEYEVENTF_EXTENDEDKEY = 0x01;
        public const uint KEYEVENTF_KEYUP = 0x02;
        public const short
            VAR_TRUE = -1,
            VAR_FALSE = 0;

        #region Methods - GetWindowName - GetWindowClass

        public static int HiWord(int number)
        {
            if ((number & 0x80000000) == 0x80000000)
                return (number >> 16);
            else
                return (number >> 16) & 0xffff;
        }

        public static int LoWord(int number)
        {
            return number & 0xffff;
        }

        public static int GET_X_LPARAM(int lp)
        {
            return ((int)(short)(lp & 0xffff));
        }

        public static int GET_Y_LPARAM(int lp)
        {
            if ((lp & 0x80000000) == 0x80000000)
                return ((int)(short)(lp >> 16));
            else
                return ((int)(short)(lp >> 16) & 0xffff);
        }

        public static int MakeLong(int LoWord, int HiWord)
        {
            return (HiWord << 16) | (LoWord & 0xffff);
        }

        public static IntPtr MakeLParam(int LoWord, int HiWord)
        {
            return (IntPtr)((HiWord << 16) | (LoWord & 0xffff));
        }

        public static string GetWindowName(IntPtr Hwnd)
        {
            if (Hwnd == IntPtr.Zero)
                return string.Empty;
            // This function gets the name of a window from its handle
            StringBuilder Title = new StringBuilder((int)WinApis.MAX_PATH);
            WinApis.GetWindowText(Hwnd, Title, (int)WinApis.MAX_PATH);

            return Title.ToString().Trim();
        }

        public static string GetWindowClass(IntPtr Hwnd)
        {
            if (Hwnd == IntPtr.Zero)
                return string.Empty;
            // This function gets the name of a window class from a window handle
            StringBuilder Title = new StringBuilder((int)WinApis.MAX_PATH);
            WinApis.RealGetWindowClass(Hwnd, Title, (int)WinApis.MAX_PATH);

            return Title.ToString().Trim();
        }

        public static FILETIME DateTimeToFiletime(DateTime time)
        {
            FILETIME ft;
            long hFT1 = time.ToFileTimeUtc();
            ft.dwLowDateTime = (uint)(hFT1 & 0xFFFFFFFF);
            ft.dwHighDateTime = (uint)(hFT1 >> 32);
            return ft;
        }

        public static DateTime FiletimeToDateTime(FILETIME fileTime)
        {
            if ((fileTime.dwHighDateTime == Int32.MaxValue) ||
                (fileTime.dwHighDateTime == 0 && fileTime.dwLowDateTime == 0))
            {
                return DateTime.MaxValue;
            }
    
            SYSTEMTIME syst = new SYSTEMTIME();
            SYSTEMTIME systLocal = new SYSTEMTIME();
            if (0 == FileTimeToSystemTime(ref fileTime, ref syst))
            {
                throw new ApplicationException("Error calling FileTimeToSystemTime: " + Marshal.GetLastWin32Error().ToString());
            }
            if (0 == SystemTimeToTzSpecificLocalTime(IntPtr.Zero, ref syst, out systLocal))
            {
                throw new ApplicationException("Error calling SystemTimeToTzSpecificLocalTime: " + Marshal.GetLastWin32Error().ToString());
            }
            return new DateTime(systLocal.Year, systLocal.Month, systLocal.Day, systLocal.Hour, systLocal.Minute, systLocal.Second);
        }
        
        public static string ToStringFromFileTime(FILETIME ft)
        {
            DateTime dt = FiletimeToDateTime(ft);
            if (dt == DateTime.MinValue)
            {
                return string.Empty;
            }

            return dt.ToString();
        }

       
       
        #endregion
        
        [DllImport("user32.dll")]
        public static extern void keybd_event(byte bVk, byte bScan, uint dwFlags,
           UIntPtr dwExtraInfo);

        [DllImport("user32.dll")]
        public static extern bool DestroyIcon(IntPtr hIcon);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int CallWindowProc(
            IntPtr lpPrevWndFunc, IntPtr hWnd, int Msg, int wParam, int lParam);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr SetWindowLong(
            IntPtr hWnd, int nIndex, IntPtr newProc);

        [DllImport("ole32.dll", SetLastError = true,
        ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern int RevokeObjectParam(
            [In, MarshalAs(UnmanagedType.LPWStr)] string pszKey);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetWindowDC(IntPtr hWnd);

     
        [DllImport("gdi32.dll", SetLastError = true)]
        public static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int nWidth,
           int nHeight);

        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr CreateCompatibleDC(IntPtr hdc);

        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern bool DeleteDC(IntPtr hdc);

        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern bool DeleteObject(IntPtr hObject);

        [DllImport("gdi32.dll", SetLastError = true)]
        public static extern bool SetStretchBltMode(IntPtr hdc, StretchMode iStretchMode);

        [DllImport("gdi32.dll", SetLastError = true)]
        public static extern bool BitBlt(IntPtr hObject, int nXDest, int nYDest,
            int nWidth, int nHeight,
            IntPtr hObjSource, int nXSrc, int nYSrc,
            TernaryRasterOperations dwRop);

        [DllImport("gdi32.dll", SetLastError = true)]
        public static extern bool StretchBlt(IntPtr hdcDest, int nXDest, int nYDest,
            int nWidthDest, int nHeightDest,
            IntPtr hdcSrc, int nXSrc, int nYSrc, int nWidthSrc, int nHeightSrc,
            TernaryRasterOperations dwRop);

        
        [DllImport("ole32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern int CreateBindCtx(
            [MarshalAs(UnmanagedType.U4)] uint dwReserved,
            [Out, MarshalAs(UnmanagedType.Interface)] out IBindCtx ppbc);

        
        
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(HandleRef hWnd, uint Msg,
            IntPtr wParam, IntPtr lParam);

        
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(HandleRef hWnd, uint Msg,
            IntPtr wParam, ref StringBuilder lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern void SendMessage(HandleRef hWnd, uint msg,
            IntPtr wParam, ref tagRECT lParam);

        
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(HandleRef hWnd, uint msg,
            IntPtr wParam, ref tagPOINT lParam);

        [DllImport("ole32.dll", CharSet = CharSet.Auto)]
        public static extern int CreateStreamOnHGlobal(IntPtr hGlobal, bool fDeleteOnRelease,
          [MarshalAs(UnmanagedType.Interface)] out IStream ppstm);

        [DllImport("user32.dll")]
        public static extern short GetKeyState(int nVirtKey);

        [DllImport("ole32.dll", ExactSpelling = true, PreserveSig = false)]
        [return: MarshalAs(UnmanagedType.Interface)]
        public static extern object CoCreateInstance(
           [In, MarshalAs(UnmanagedType.LPStruct)] Guid rclsid,
           [MarshalAs(UnmanagedType.IUnknown)] object pUnkOuter,
           CLSCTX dwClsContext,
           [In, MarshalAs(UnmanagedType.LPStruct)] Guid riid);

        
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern uint MessageBox(
            IntPtr hWnd, String text, String caption, uint type);

        
        [DllImport("user32.dll")]
        public static extern bool GetClientRect(IntPtr hWnd, out tagRECT lpRect);

        [DllImport("user32.dll")]
        public static extern bool IsWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool BringWindowToTop(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        
        [DllImport("user32.dll")]
        public static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string className, IntPtr windowTitle);

        [DllImport("user32.dll")]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder title, int size);

        
        [DllImport("user32.dll")]
        public static extern uint RealGetWindowClass(IntPtr hWnd, StringBuilder pszType, uint cchType);

        [DllImport("user32.dll")]
        public static extern IntPtr SetFocus(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool CopyRect(
            [In, Out, MarshalAs(UnmanagedType.Struct)] ref tagRECT lprcDst,
            [In, MarshalAs(UnmanagedType.Struct)] ref tagRECT lprcSrc);

      
        [DllImport("ole32.dll")]
        public static extern void ReleaseStgMedium(
            [In, MarshalAs(UnmanagedType.Struct)]
            ref System.Runtime.InteropServices.ComTypes.STGMEDIUM pmedium);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GlobalLock(IntPtr hMem);

        [DllImport("kernel32.dll")]
        public static extern bool GlobalUnlock(IntPtr hMem);

        [DllImport("kernel32.dll")]
        public static extern UIntPtr GlobalSize(IntPtr hMem);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern long FileTimeToSystemTime(ref Syncfusion.HtmlConverter.FILETIME FileTime,
            ref SYSTEMTIME SystemTime);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern long SystemTimeToTzSpecificLocalTime(
            IntPtr lpTimeZoneInformation, ref SYSTEMTIME lpUniversalTime,
            out SYSTEMTIME lpLocalTime);

       
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool SystemParametersInfo(uint uiAction, uint uiParam,
            IntPtr pvParam, uint fWinIni);

        private const uint SPIF_UPDATEINIFILE = 0x0001;
        private const uint SPIF_SENDWININICHANGE = 0x0002;
       
        private const uint SPI_SETBEEP = 0x0002;

        //For older windows
        public static bool SetSystemBeep(bool bEnable)
        {
            if (bEnable)
                return SystemParametersInfo(SPI_SETBEEP, 1, IntPtr.Zero, (SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE));
            else
                return SystemParametersInfo(SPI_SETBEEP, 0, IntPtr.Zero, (SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE));
        }

      
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern int MapWindowPoints(IntPtr hWndFrom, IntPtr hWndTo,
            [In, Out] tagPOINT pt, int cPoints);


    }
    #endregion

    #region HResults
    internal sealed class Hresults
    {
        public const int NOERROR = 0;
        public const int S_OK = 0;
        public const int S_FALSE = 1;
        public const int E_PENDING = unchecked((int)0x8000000A);
        public const int E_HANDLE = unchecked((int)0x80070006);
        public const int E_NOTIMPL = unchecked((int)0x80004001);
        public const int E_NOINTERFACE = unchecked((int)0x80004002);

        public const int E_POINTER = unchecked((int)0x80004003);
        public const int E_ABORT = unchecked((int)0x80004004);
        public const int E_FAIL = unchecked((int)0x80004005);
        public const int E_OUTOFMEMORY = unchecked((int)0x8007000E);
        public const int E_ACCESSDENIED = unchecked((int)0x80070005);
        public const int E_UNEXPECTED = unchecked((int)0x8000FFFF);
        public const int E_FLAGS = unchecked((int)0x1000);
        public const int E_INVALIDARG = unchecked((int)0x80070057);

        //Wininet
        public const int ERROR_SUCCESS = 0;
        public const int ERROR_FILE_NOT_FOUND = 2;
        public const int ERROR_ACCESS_DENIED = 5;
        public const int ERROR_INSUFFICIENT_BUFFER = 122;
        public const int ERROR_NO_MORE_ITEMS = 259;

        //Ole Errors
        public const int OLE_E_FIRST = unchecked((int)0x80040000);
        public const int OLE_E_LAST = unchecked((int)0x800400FF);
        public const int OLE_S_FIRST = unchecked((int)0x00040000);
        public const int OLE_S_LAST = unchecked((int)0x000400FF);

        public const int OLECMDERR_E_FIRST = unchecked((int)(OLE_E_LAST + 1));
        public const int OLECMDERR_E_NOTSUPPORTED = unchecked((int)(OLECMDERR_E_FIRST));
        public const int OLECMDERR_E_DISABLED = unchecked((int)(OLECMDERR_E_FIRST + 1));
        public const int OLECMDERR_E_NOHELP = unchecked((int)(OLECMDERR_E_FIRST + 2));
        public const int OLECMDERR_E_CANCELED = unchecked((int)(OLECMDERR_E_FIRST + 3));
        public const int OLECMDERR_E_UNKNOWNGROUP = unchecked((int)(OLECMDERR_E_FIRST + 4));

        public const int OLEOBJ_E_NOVERBS = unchecked((int)0x80040180);
        public const int OLEOBJ_S_INVALIDVERB = unchecked((int)0x00040180);
        public const int OLEOBJ_S_CANNOT_DOVERB_NOW = unchecked((int)0x00040181);
        public const int OLEOBJ_S_INVALIDHWND = unchecked((int)0x00040182);

        public const int DV_E_LINDEX = unchecked((int)0x80040068);
        public const int OLE_E_OLEVERB = unchecked((int)0x80040000);
        public const int OLE_E_ADVF = unchecked((int)0x80040001);
        public const int OLE_E_ENUM_NOMORE = unchecked((int)0x80040002);
        public const int OLE_E_ADVISENOTSUPPORTED = unchecked((int)0x80040003);
        public const int OLE_E_NOCONNECTION = unchecked((int)0x80040004);
        public const int OLE_E_NOTRUNNING = unchecked((int)0x80040005);
        public const int OLE_E_NOCACHE = unchecked((int)0x80040006);
        public const int OLE_E_BLANK = unchecked((int)0x80040007);
        public const int OLE_E_CLASSDIFF = unchecked((int)0x80040008);
        public const int OLE_E_CANT_GETMONIKER = unchecked((int)0x80040009);
        public const int OLE_E_CANT_BINDTOSOURCE = unchecked((int)0x8004000A);
        public const int OLE_E_STATIC = unchecked((int)0x8004000B);
        public const int OLE_E_PROMPTSAVECANCELLED = unchecked((int)0x8004000C);
        public const int OLE_E_INVALIDRECT = unchecked((int)0x8004000D);
        public const int OLE_E_WRONGCOMPOBJ = unchecked((int)0x8004000E);
        public const int OLE_E_INVALIDHWND = unchecked((int)0x8004000F);
        public const int OLE_E_NOT_INPLACEACTIVE = unchecked((int)0x80040010);
        public const int OLE_E_CANTCONVERT = unchecked((int)0x80040011);
        public const int OLE_E_NOSTORAGE = unchecked((int)0x80040012);
        public const int RPC_E_RETRY = unchecked((int)0x80010109);
    }
    #endregion

    #region ClassIDS
    /// <summary>
    /// GUID representation of IIDs and CLSIDs
    /// </summary>
    public sealed class Iid_Clsids
    {

        public static Guid IID_IUnknown = new Guid("00000000-0000-0000-C000-000000000046");
        public static Guid IID_IViewObject = new Guid("0000010d-0000-0000-C000-000000000046");
        public static Guid IID_IAuthenticate = new Guid("79eac9d0-baf9-11ce-8c82-00aa004ba90b");
        public static Guid IID_IWindowForBindingUI = new Guid("79eac9d5-bafa-11ce-8c82-00aa004ba90b");
        public static Guid IID_IHttpSecurity = new Guid("79eac9d7-bafa-11ce-8c82-00aa004ba90b");
        public static Guid IID_INewWindowManager = new Guid("D2BC4C84-3F72-4a52-A604-7BCBF3982CBB");
        public static Guid IID_IOleClientSite = new Guid("00000118-0000-0000-C000-000000000046");
        public static Guid IID_IDispatch = new Guid("{00020400-0000-0000-C000-000000000046}");
        public static Guid IID_TopLevelBrowser = new Guid("4C96BE40-915C-11CF-99D3-00AA004AE837");
        public static Guid IID_WebBrowserApp = new Guid("0002DF05-0000-0000-C000-000000000046");
        public static Guid IID_IBinding = new Guid("79EAC9C0-BAF9-11CE-8C82-00AA004BA90B");
        public static Guid IID_IBindStatusCallBack = new Guid("79EAC9C1-BAF9-11CE-8C82-00AA004BA90B");
        public static Guid IID_IOleObject = new Guid("00000112-0000-0000-C000-000000000046");
        public static Guid IID_IOleWindow = new Guid("00000114-0000-0000-C000-000000000046");
        public static Guid IID_IServiceProvider = new Guid("6d5140c1-7436-11ce-8034-00aa006009fa");
        public static Guid IID_IWebBrowser = new Guid("EAB22AC1-30C1-11CF-A7EB-0000C05BAE0B");
        public static Guid IID_IWebBrowser2 = new Guid("D30C1661-CDAF-11d0-8A3E-00C04FC9E26E");
        public static Guid CLSID_WebBrowser = new Guid("8856F961-340A-11D0-A96B-00C04FD705A2");
        public static Guid CLSID_CGI_IWebBrowser = new Guid("ED016940-BD5B-11CF-BA4E-00C04FD70816");
        public static Guid CLSID_CGID_DocHostCommandHandler = new Guid("F38BC242-B950-11D1-8918-00C04FC2C836");
        public static Guid CLSID_ShellUIHelper = new Guid("64AB4BB7-111E-11D1-8F79-00C04FC2FBE1");
        public static Guid CLSID_SecurityManager = new Guid("7b8a2d94-0ac9-11d1-896c-00c04fb6bfc4");
        public static Guid IID_IShellUIHelper = new Guid("729FE2F8-1EA8-11d1-8F85-00C04FC2FBE1");
        public static Guid Guid_MSHTML = new Guid("DE4BA900-59CA-11CF-9592-444553540000");
        public static Guid CLSID_InternetSecurityManager = new Guid("7b8a2d94-0ac9-11d1-896c-00c04fB6bfc4");
        public static Guid IID_IInternetSecurityManager = new Guid("79EAC9EE-BAF9-11CE-8C82-00AA004BA90B");
        public static Guid IID_IInternetSecurityManagerEx = new Guid("F164EDF1-CC7C-4f0d-9A94-34222625C393");
        public static Guid CLSID_InternetZoneManager = new Guid("7B8A2D95-0AC9-11D1-896C-00C04FB6BFC4");
        public static Guid CGID_ShellDocView = new Guid("000214D1-0000-0000-C000-000000000046");
        public static Guid SID_SDownloadManager = new Guid("988934A4-064B-11D3-BB80-00104B35E7F9");
        public static Guid IID_IDownloadManager = new Guid("988934A4-064B-11D3-BB80-00104B35E7F9");
        public static Guid IID_IHttpNegotiate = new Guid("79eac9d2-baf9-11ce-8c82-00aa004ba90b");
        public static Guid IID_IStream = new Guid("0000000c-0000-0000-C000-000000000046");
        public static Guid DIID_HTMLDocumentEvents2 = new Guid("3050f613-98b5-11cf-bb82-00aa00bdce0b");
        public static Guid DIID_HTMLWindowEvents2 = new Guid("3050f625-98b5-11cf-bb82-00aa00bdce0b");
        public static Guid DIID_HTMLElementEvents2 = new Guid("3050f60f-98b5-11cf-bb82-00aa00bdce0b");
        public static Guid DIID_HTMLSelectElementEvents2 = new Guid("3050f622-98b5-11cf-bb82-00aa00bdce0b");
        public static Guid DIID_HTMLScriptEvents2 = new Guid("3050f621-98b5-11cf-bb82-00aa00bdce0b");
        public static Guid IID_IDataObject = new Guid("0000010e-0000-0000-C000-000000000046");
        public static Guid CLSID_InternetShortcut = new Guid("FBF23B40-E3F0-101B-8488-00AA003E56F8");
        public static Guid IID_IUniformResourceLocatorA = new Guid("FBF23B80-E3F0-101B-8488-00AA003E56F8");
        public static Guid IID_IUniformResourceLocatorW = new Guid("CABB0DA0-DA57-11CF-9974-0020AFD79762");
        public static Guid IID_IHTMLBodyElement = new Guid("3050F1D8-98B5-11CF-BB82-00AA00BDCE0B");
        public static Guid CLSID_CUrlHistory = new Guid("3C374A40-BAE4-11CF-BF7D-00AA006946EE");
        public static Guid CLSID_HTMLDocument = new Guid("25336920-03F9-11cf-8FD0-00AA00686F13");
        public static Guid IID_IPropertyNotifySink = new Guid("9BFBBC02-EFF1-101A-84ED-00AA00341D07");
        public static Guid IID_IProtectFocus = new Guid("D81F90A3-8156-44F7-AD28-5ABB87003274");
        public static Guid IID_IHTMLOMWindowServices = new Guid("3050f5fc-98b5-11cf-bb82-00aa00bdce0b");
        public static Guid CLSID_HostDialogHelper = new Guid("429af92c-a51f-11d2-861e-00c04fa35c89");
        public static Guid IID_IHostDialogHelper = new Guid("53DEC138-A51E-11d2-861E-00C04FA35C89");
        public static Guid IID_ITravelLogStg = new Guid("7EBFDD80-AD18-11d3-A4C5-00C04F72D6B8");
        public static Guid SID_STravelLogCursor = new Guid("7EBFDD80-AD18-11d3-A4C5-00C04F72D6B8");
        public static Guid IID_IHTMLEditServices = new Guid("3050f663-98b5-11cf-bb82-00aa00bdce0b");
        public static Guid SID_SHTMLEditServices = new Guid("3050f7f9-98b5-11cf-bb82-00aa00bdce0b");
        public static Guid IID_IHTMLEditHost = new Guid("3050f6a0-98b5-11cf-bb82-00aa00bdce0b");
        public static Guid SID_SHTMLEditHost = new Guid("3050f6a0-98b5-11cf-bb82-00aa00bdce0b");
        public static Guid CGID_Explorer = new Guid("000214d0-0000-0000-c000-000000000046");
        public static Guid IID_IThumbnailCapture = new Guid("4ea39266-7211-409f-b622-f63dbd16c533");
        public static Guid CLSID_HTML_Thumbnail_Extractor = new Guid("EAB841A0-9550-11CF-8C16-00805F1408F3");
        public static Guid IID_ITargetFrame2 = new Guid("86D52E11-94A8-11d0-82AF-00C04FD5AE38");
        public static Guid IID_IDispatchEX = new Guid("A6EF9860-C720-11d0-9337-00A0C90DCAA9");
    }
    #endregion
}
