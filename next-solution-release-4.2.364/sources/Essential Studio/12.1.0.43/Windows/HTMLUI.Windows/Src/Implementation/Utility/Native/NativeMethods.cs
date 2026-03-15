#region Copyright Syncfusion Inc. 2001 - 2014
////  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
////  Use of this code is subject to the terms of our license.
////  A copy of the current license can be obtained at any time by e-mailing
////  licensing@syncfusion.com. Re-distribution in any form is strictly
////  prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System;
using System.Runtime.InteropServices;

namespace Syncfusion.Windows.Forms.HTMLUI.Implementation.Utility
{
    /// <summary>
    /// Class representing the native functions of OS.
    /// </summary>
    internal class NativeMethods
    {
        #region Enums
        /// <summary>
        /// Flags for controlling the printing.
        /// </summary>
        [Flags]
        public enum PrintFlags : int
        {
            /// <summary>
            /// Check Visible Flag
            /// </summary>
            PRF_CHECKVISIBLE = 0x01,

            /// <summary>
            /// Non Client Flag
            /// </summary>
            PRF_NONCLIENT = 0x02,

            /// <summary>
            /// Client Flag
            /// </summary>
            PRF_CLIENT = 0x04,

            /// <summary>
            /// Erase Background
            /// </summary>
            PRF_ERASEBKGND = 0x08,

            /// <summary>
            /// Children Flag
            /// </summary>
            PRF_CHILDREN = 0x10,

            /// <summary>
            /// Owned Flag
            /// </summary>
            PRF_OWNED = 0x20,

            /// <summary>
            /// All flags
            /// </summary>
            PRF_ALL = PRF_CHECKVISIBLE | PRF_NONCLIENT | PRF_CLIENT |
              PRF_ERASEBKGND | PRF_CHILDREN | PRF_OWNED
        }
        #endregion

        #region Class constants
        /// <summary>
        /// Declaration of WM_PRINT message.
        /// </summary>
        public const int WM_PRINT = 791; // 0x0317

        /// <summary>
        /// Declaration of WM_PRINTCLIENT message.
        /// </summary>
        public const int WM_PRINTCLIENT = 792; // 0x0318

        /// <summary>
        /// The distance from the left edge of the physical page to the left edge 
        /// of the printable area, in device units.
        /// </summary>
        public const int PHYSICALOFFSETX = 112;

        /// <summary>
        /// The distance from the top edge of the physical page to the top edge 
        /// of the printable area, in device units.
        /// </summary>
        public const int PHYSICALOFFSETY = 113;

        /// <summary>
        /// Declaration of WM_SETREDRAW message.
        /// </summary>
        public const int WM_SETREDRAW = 11; // 0x000B

        #endregion

        #region Function declarations
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        internal static extern IntPtr GetWindowDC(IntPtr hWnd);
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        internal static extern IntPtr GetDC(IntPtr hWnd);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, out bool bValue);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, int lParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, IntPtr lParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        internal static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("gdi32", CharSet = CharSet.Auto, ExactSpelling = true)]
        internal static extern IntPtr SelectObject(IntPtr hdc, IntPtr hObject);
        [DllImport("gdi32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        internal static extern IntPtr CreateCompatibleDC(IntPtr hDC);
        [DllImport("gdi32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        internal static extern IntPtr CreateCompatibleBitmap(IntPtr hDC, int width, int height);
        [DllImport("gdi32")]
        internal static extern bool DeleteObject(IntPtr hObject);

        /// <summary>
        /// Retrieves device-specific information for the specified device.
        /// </summary>
        /// <param name="hdc">Handle to DC.</param>
        /// <param name="capindex">Index of capability.</param>
        /// <returns>When nIndex is BITSPIXEL and the device has 15bpp or 16bpp, the return value is 16.</returns>
        [DllImport("gdi32.dll")]
        internal static extern int GetDeviceCaps(IntPtr hdc, int capindex);

        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Initializes a new instance of the NativeMethods class
        /// </summary>
        public NativeMethods()
        {
        }
        #endregion
    }
}
