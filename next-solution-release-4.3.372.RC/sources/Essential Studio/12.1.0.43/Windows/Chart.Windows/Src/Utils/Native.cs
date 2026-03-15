#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;

namespace Syncfusion.Windows.Forms.Chart
{
        internal class Native
        {
            #region Class constants
            /// <summary>
            /// Windows message.
            /// </summary>
            internal const uint PRF_CHECKVISIBLE = 0x00000001;
            /// <summary>
            /// Windows message.
            /// </summary>
            internal const uint PRF_CHILDREN = 0x00000010;
            /// <summary>
            /// Windows message.
            /// </summary>
            internal const uint PRF_CLIENT = 0x00000004;
            /// <summary>
            /// Windows message.
            /// </summary>
            internal const uint PRF_ERASEBKGND = 0x00000008;
            /// <summary>
            /// Windows message.
            /// </summary>
            internal const uint PRF_NONCLIENT = 0x00000002;
            /// <summary>
            /// Windows message.
            /// </summary>
            internal const uint PRF_OWNED = 0x00000020;
            /// <summary>
            /// Windows message.
            /// </summary>
            internal const uint PW_CLIENTONLY = 0x00000001;
            /// <summary>
            /// Windows message.
            /// </summary>
            internal const uint WM_PRINT = 0x0317;
            /// <summary>
            /// Windows message.
            /// </summary>
            internal const uint WM_PRINTCLIENT = 0x0318;
            /// <summary>
            /// HRESULT S_OK constant.
            /// </summary>
            internal const int S_OK = 0;
            /// <summary>
            /// HRESULT S_OK constant.
            /// </summary>
            internal const int S_FALSE = 1;

            public const int S_NOTIMPL = unchecked((int)0x80004001);
            #endregion

            #region Class native methods
            [DllImport("gdi32.dll")]
            public static extern IntPtr CreateCompatibleDC(IntPtr hdc);
            [DllImport("gdi32.dll")]
            public static extern bool DeleteDC(IntPtr hdc);
            [DllImport("gdi32.dll")]
            public static extern bool DeleteObject(IntPtr hObject);
            [DllImport("user32.dll")]
            public static extern bool PrintWindow(IntPtr hwnd, IntPtr hdcBlt, uint nFlags);
            [DllImport("gdi32.dll")]
            public static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);
            [DllImport("user32.dll")]
            public static extern int SendMessage(IntPtr hWnd, uint Msg, uint wParam, uint lParam);
            [DllImport("ole32.dll", ExactSpelling = true, SetLastError = true)]
            public static extern bool OleDraw
              ([MarshalAs(UnmanagedType.IUnknown)] object pUnkn,
              int dwAspect, IntPtr hDC, ref Rectangle R);
            #endregion
        }
   
}
