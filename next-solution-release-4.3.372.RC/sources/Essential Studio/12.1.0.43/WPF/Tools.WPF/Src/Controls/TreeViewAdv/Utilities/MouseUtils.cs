// <copyright file="MouseUtils.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws.
// </copyright>

#region file using

using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

#endregion file using

namespace Syncfusion.Windows.Tools
{
    /// <summary>
    /// Represents MouseUtils class
    /// </summary>
    internal static class MouseUtils
    {
        /// <summary>
        /// Represents Win32Point struct
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        private struct Win32Point
        {
            /// <summary>
            /// Presents X
            /// </summary>
            public Int32 X;

            /// <summary>
            /// Presents Y
            /// </summary>
            public Int32 Y;
        }

        #region Implementation

        /// <summary>
        /// Gets the cursor pos.
        /// </summary>
        /// <param name="pt">The pt win32 point .</param>
        /// <returns>bool type value </returns>
        [DllImport("user32.dll")]
        private static extern bool GetCursorPos(ref Win32Point pt);

        /// <summary>
        /// Screens to client.
        /// </summary>
        /// <param name="hwnd">The HWND intptr.</param>
        /// <param name="pt">The pt win32 point.</param>
        /// <returns>bool type value </returns>
        [DllImport("user32.dll")]
        private static extern bool ScreenToClient(IntPtr hwnd, ref Win32Point pt);

        /// <summary>
        /// Returns the DPI fraction value.
        /// </summary>
        /// <returns>double type value</returns>
        public static double GetDPIFraction()
        {
            System.Drawing.Graphics graphics = System.Drawing.Graphics.FromHwnd(IntPtr.Zero);
            if (graphics != null)
                return (graphics.DpiY / 96);
            return 1;
        }

        /// <summary>
        /// Gets the mouse position.
        /// </summary>
        /// <param name="relativeTo">The relative to.</param>
        /// <returns> Point type</returns>
        public static Point GetMousePosition(Visual relativeTo)
        {
            Point resultPoint;
            Win32Point mouse = new Win32Point();
            GetCursorPos(ref mouse);
            resultPoint = new Point(mouse.X, mouse.Y);

            if (relativeTo != null)
            {
                HwndSource presentationSource = (HwndSource)PresentationSource.FromVisual(relativeTo);

                if (presentationSource != null)
                {
                    ScreenToClient(presentationSource.Handle, ref mouse);
                    GeneralTransform transform = relativeTo.TransformToAncestor(presentationSource.RootVisual);
                    Point offset = transform.Transform(new Point(0, 0));
                    resultPoint = new Point(mouse.X - (offset.X * GetDPIFraction()), mouse.Y - (offset.Y * GetDPIFraction()));
                }
            }

            return resultPoint;
        }

        /// <summary>
        /// Gets the item mouse over.
        /// </summary>
        /// <returns> TreeViewItem Adv</returns>
        public static Syncfusion.Windows.Tools.Controls.TreeViewItemAdv GetItemMouseOver()
        {
            Syncfusion.Windows.Tools.Controls.TreeViewItemAdv view = null;
            FrameworkElement element = Mouse.DirectlyOver as FrameworkElement;

            if (element != null)
            {
                view = ItemsControl.ItemsControlFromItemContainer(element) as Syncfusion.Windows.Tools.Controls.TreeViewItemAdv;

                if (view == null)
                {
                    while (element != null)
                    {
                        element = VisualTreeHelper.GetParent(element) as FrameworkElement;

                        if (element is Syncfusion.Windows.Tools.Controls.TreeViewItemAdv)
                        {
                            view = (Syncfusion.Windows.Tools.Controls.TreeViewItemAdv)element;
                            break;
                        }
                    }
                }
            }

            return view;
        }

        #endregion Implementation
    }
}