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
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;

namespace Syncfusion.Windows.Forms.Chart.Utils
{
    /// <summary>
    /// Utility class for different static methods.
    /// </summary>
    internal sealed class WinUtilities
    {
        #region Class Static methods
        /// <summary>
        /// Makes snapshot from control and saves it to image.
        /// </summary>
        /// <param name="control">Control to be printed.</param>
        /// <returns>Image with snapshot of control.</returns>
        public static Bitmap PrintControl(Control control)
        {
            if (control == null)
            {
                throw new ArgumentNullException("control");
            }

            IntPtr hWnd = control.Handle;
            IntPtr hWndDC = IntPtr.Zero;
            Rectangle windowRectangle = Rectangle.Empty;
            IntPtr hCompatibleBMP = IntPtr.Zero;
            IntPtr hCompatibleDC = IntPtr.Zero;
            int windowHeight = 0;
            int windowWidth = 0;

            try
            {
                // Calculate the width and height of the window.
                windowWidth = control.Width;
                windowHeight = control.Height;

                // Get a DC for the window and create a compatible memory DC.
                hWndDC = NativeMethods.GetWindowDC(hWnd);
                hCompatibleDC = NativeMethods.CreateCompatibleDC(hWndDC);

                // Create a compatible bitmap and select it into the compatible DC.
                hCompatibleBMP = NativeMethods.CreateCompatibleBitmap(hWndDC, windowWidth, windowHeight);
                NativeMethods.SelectObject(hCompatibleDC, hCompatibleBMP);

                // Send WM_PRINT message to window specifying the memory DC as the device context
                // and setting all the PrintFlags flags.
                int flags = (int)NativeMethods.PrintFlags.PRF_ALL;
                NativeMethods.SendMessage(hWnd, NativeMethods.WM_PRINT, hCompatibleDC, flags);

                return Image.FromHbitmap(hCompatibleBMP);
            }
            finally
            {
                NativeMethods.ReleaseDC(hWnd, hWndDC);
                NativeMethods.DeleteObject(hCompatibleBMP);
                NativeMethods.DeleteObject(hCompatibleDC);
            }
        }
        #endregion
    }
}
