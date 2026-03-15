// <copyright file="NonStickingPopup.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls.Primitives;
using System.Windows.Interop;
using System.Windows;
using System.ComponentModel;
using System.Windows.Media;
using System.Runtime.InteropServices;
using System.Diagnostics;


namespace Syncfusion.Windows.Shared
{
    /// <summary>
    /// Extends standard <see cref="Popup"/>. Represents popup element that can overflow beyond monitor area. 
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class NonStickingPopup : Popup
    {

        /// <summary>
        /// Represents is shifted
        /// </summary>
        private bool m_Shift = true;
        /// <summary>
        /// Represents shift count
        /// </summary>
        private int m_Shiftcount = 0;
        /// <summary>
        /// Initializes a new instance of the <see cref="NonStickingPopup"/> class.
        /// </summary>
        public NonStickingPopup()
        {

        }

        static NonStickingPopup()
        {
            PlacementRectangleProperty.OverrideMetadata(typeof(NonStickingPopup), new FrameworkPropertyMetadata(new Rect(0.0, 0.0, 0.0, 0.0), new PropertyChangedCallback(OnPlacementRectangleChanged), null));
        }

        /// <summary>
        /// Removehandles this instance.
        /// </summary>
        public void Removehandle()
        {

        }

        #region Implementation

        /// <summary>
        /// Invoked when <see>
        ///     <cref>NonStickingPopup.OnPlacementRectangleChanged</cref>
        /// </see>
        ///     property is changed.
        /// </summary>
        /// <param name="d">The sender.</param>
        /// <param name="args">The event arguments</param>
        private static void OnPlacementRectangleChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            NonStickingPopup popup = d as NonStickingPopup;
            popup.OnPlacementRectangleChanged(args);
        }

        private void OnPlacementRectangleChanged(DependencyPropertyChangedEventArgs args)
        {
            MaxWidth = 0.87 * SystemParameters.PrimaryScreenWidth;
            MaxHeight = 0.75 * SystemParameters.PrimaryScreenHeight;
            if (Child != null)
            {
                HwndSource source = (HwndSource)PresentationSource.FromVisual(this.Child);

                if (source != null)
                {
                    IntPtr hwnd = source.Handle;

                    NativeMethods.SetWindowPos(
                        hwnd,
                        NativeConstants.HWND_TOP,
                        0,
                        0,
                        0,
                        0,
                        NativeConstants.SWP_NOACTIVATE | NativeConstants.SWP_FRAMECHANGED | NativeConstants.SWP_NOSIZE | NativeConstants.SWP_NOMOVE);

                }
            }
        }

        /// <summary>
        /// Positioning the hook.
        /// </summary>
        /// <param name="hwnd">The Handle WND.</param>
        /// <param name="msg">The Message.</param>
        /// <param name="wParam">The wparam value.</param>
        /// <param name="lParam">The lparam value.</param>
        /// <param name="handled">if set to <c>true</c> [handled].</param>
        /// <returns>Returns window message IntPtr</returns>
        private IntPtr PositioningHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case NativeConstants.WM_WINDOWPOSCHANGING:
                    WINDOWPOS posInfo = (WINDOWPOS)Marshal.PtrToStructure(lParam, typeof(WINDOWPOS));
                    HwndSource source = HwndSource.FromHwnd(posInfo.hwnd);
                    if (m_Shift && ChangeDefault())
                    {
                        MaxWidth = 0.87 * SystemParameters.PrimaryScreenWidth;
                        MaxHeight = 0.75 * SystemParameters.PrimaryScreenHeight;
                        if (m_Shiftcount > 2)
                        {
                            m_Shift = false;
                        }
                        m_Shiftcount++;
                    }

                    if (source != null)
                    {
                        Matrix transformFromDevice = source.CompositionTarget.TransformFromDevice;
                        Matrix transformToDevice = source.CompositionTarget.TransformToDevice;

                        Point pointStart = new Point(PlacementRectangle.X, PlacementRectangle.Y);
                        Point pointEnd = new Point(PlacementRectangle.X + PlacementRectangle.Width, PlacementRectangle.Y + PlacementRectangle.Height);

                        pointStart = transformToDevice.Transform(pointStart);
                        pointEnd = transformToDevice.Transform(pointEnd);

                        posInfo.x = (int)pointStart.X;
                        posInfo.y = (int)pointStart.Y;
                        posInfo.cx = (int)(pointEnd.X - pointStart.X);
                        posInfo.cy = (int)(pointEnd.Y - pointStart.Y);

                        posInfo.flags &= ~(NativeConstants.SWP_NOMOVE | NativeConstants.SWP_NOSIZE);
                        posInfo.flags |= NativeConstants.SWP_FRAMECHANGED;

                        Marshal.StructureToPtr(posInfo, lParam, true);
                        //// Below line commented out to fix crash in Windows 8 with 150 DPI settings
                        //handled = true;
                    }
                    else
                    {
                        Marshal.DestroyStructure(lParam, typeof(WINDOWPOS));
                    }

                    return IntPtr.Zero;
            }

            return IntPtr.Zero;
        }
        

        /// <summary>
        /// Responds to the condition in which the value of the <see cref="P:System.Windows.Controls.Primitives.Popup.IsOpen"/> property changes from false to true.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        protected override void OnOpened(EventArgs e)
        {
            if (Child != null && PermissionHelper.HasUnmanagedCodePermission)
            {
                OnOpenedSecure();
            }

            base.OnOpened(e);
        }

        /// <summary>
        /// Changes the default.
        /// </summary>
        /// <returns></returns>
        private bool ChangeDefault()
        {
            if (Height != 100 && Width != 150)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Called when [opened secure].
        /// </summary>
        private void OnOpenedSecure()
        {
            HwndSource source = (HwndSource)PresentationSource.FromVisual(this.Child);
            HwndSourceHook hook = new HwndSourceHook(PositioningHook);

            source.RemoveHook(hook);
            source.AddHook(hook);
        }

        #endregion
    }
}
