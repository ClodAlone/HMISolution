// <copyright file="TreeViewItemDragPopup.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws.
// </copyright>

#region file using

using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Interop;

#endregion file using

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents a control for contains and displaying drag item.
    /// </summary>
#if SyncfusionFramework4_0

    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    internal class TreeViewItemAdvDragPopup : Popup
    {
        #region Constants

        /// <summary>
        /// Presents WM_NCHITTEST
        /// </summary>
        private const int WM_NCHITTEST = 0x0084;

        /// <summary>
        /// Presents HTTRANSPARENT
        /// </summary>
        private const int HTTRANSPARENT = -1;

        /// <summary>
        /// Presents SWP_NOSIZE
        /// </summary>
        private const int SWP_NOSIZE = 1;

        /// <summary>
        /// Presents SWP_NOMOVE
        /// </summary>
        private const int SWP_NOMOVE = 2;

        /// <summary>
        /// Presents SWP_NOACTIVATE
        /// </summary>
        private const int SWP_NOACTIVATE = 16;

        #endregion Constants

        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="TreeViewItemAdvDragPopup"/> class.Container to be set as window's
        /// content.
        /// </summary>
        public TreeViewItemAdvDragPopup()
        {
            AllowsTransparency = true;
            StaysOpen = false;
            if (BrowserInteropHelper.IsBrowserHosted)
            {
                Placement = PlacementMode.Relative;
            }
            else
            {
                Placement = PlacementMode.Absolute;
            }
        }

        #endregion Initialization

        #region Implemantation

        /// <summary>
        /// Responds to the condition in which the value of the IsOpen property changes
        /// from false to true.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        protected override void OnOpened(EventArgs e)
        {
            base.OnOpened(e);

            IntPtr hwnd = ((HwndSource)PresentationSource.FromVisual(Child)).Handle;
            SetWindowPos(hwnd, -1, 0, 0, 0, 0, SWP_NOSIZE | SWP_NOMOVE | SWP_NOACTIVATE);
            HwndSource.FromHwnd(hwnd).AddHook(new HwndSourceHook(HookMethod));
        }

        /// <summary>
        /// Represents the method that handles Win32 window messages.
        /// </summary>
        /// <param name="hwnd">The window handle.</param>
        /// <param name="msg">The message ID.</param>
        /// <param name="wParam">The message's wParam value.</param>
        /// <param name="lParam">The message's lparam</param>
        /// <param name="handled">The message's lParam value.</param>
        /// <returns>INtPtr value </returns>
        private IntPtr HookMethod(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case WM_NCHITTEST:
                    {
                        handled = true;
                        return new IntPtr(HTTRANSPARENT);
                    }
            }

            return new IntPtr(0);
        }

        #endregion Implemantation

        #region External Functions

        /// <summary>
        /// The SetWindowPos function changes the size,
        /// position, and Z order of a child, pop-up,
        /// or top-level window. Child, pop-up, and top-level windows are
        /// ordered according to their appearance on the screen.
        /// The topmost window receives the highest rank and is the first window in the Z order.
        /// </summary>
        /// <param name="hwnd">Handle to the window.</param>
        /// <param name="hwndInsertAfter">Handle to the window to precede the positioned window
        /// in the Z order. This parameter must be a window handle or one of the following values.</param>
        /// <param name="x">Specifies the new position of the left side of the window,
        /// in client coordinates.</param>
        /// <param name="y">Specifies the new position of the top of the window, in client coordinates.</param>
        /// <param name="cx">Specifies the new width of the window, in pixels.</param>
        /// <param name="cy">Specifies the new height of the window, in pixels.</param>
        /// <param name="wFlags">Specifies the window sizing and positioning flags. This parameter can be a combination of the following values.</param>
        /// <returns>If the function succeeds, the return value is nonzero.
        /// If the function fails, the return value is zero.
        /// </returns>
        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("user32", EntryPoint = "SetWindowPos")]
        private static extern int SetWindowPos(IntPtr hwnd, int hwndInsertAfter, int x, int y, int cx, int cy, int wFlags);

        #endregion External Functions
    }
}