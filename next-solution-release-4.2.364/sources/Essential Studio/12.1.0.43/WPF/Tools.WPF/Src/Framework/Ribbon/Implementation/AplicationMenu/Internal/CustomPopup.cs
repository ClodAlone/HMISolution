// <copyright file="CustomPopup.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using Syncfusion.Windows.Shared;
using System.ComponentModel;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// This class overrides simple popup logic in Application Menu.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    [Browsable(false)]
    public class CustomPopup : Popup
    {
        #region Initialize

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomPopup"/> class.
        /// </summary>
        public CustomPopup()
        {
            CustomPopupPlacementCallback = new CustomPopupPlacementCallback(PlacePopup);
        }
        #endregion

        #region Overrides
        /// <summary>
        /// Invoked during the transition of a communication object into
        /// the opening state.
        /// </summary>
        /// <param name="e">The instance containing the event data.</param>
        protected override void OnOpened(EventArgs e)
        {
            base.OnOpened(e);
            bool canHandle = !BrowserInteropHelper.IsBrowserHosted;

            if (canHandle)
            {
                HandlePlacing();
            }
        }

        /// <summary>
        /// Raises the Initialized event. This method is invoked
        /// whenever IsInitialized is set to true internally.
        /// </summary>
        /// <param name="e">The RoutedEventArgs that contains the event
        /// data.</param> 
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            if (TemplatedParent != null)
            {
                FrameworkElement templatedParent = (FrameworkElement)TemplatedParent;
                if (templatedParent.Parent is MenuButtonBase || templatedParent.Parent is RibbonMenuGroup)
                {
                    Placement = PlacementMode.Right;
                    PlacementTarget = Parent as UIElement;
                }
            }
        }
        #endregion

        #region Implementation

        /// <summary>
        /// Places the popup.
        /// </summary>
        /// <param name="popupSize">Size of the popup.</param>
        /// <param name="targetSize">Size of the target.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>Returns the ttplaces.</returns>
        private CustomPopupPlacement[] PlacePopup(System.Windows.Size popupSize, System.Windows.Size targetSize, System.Windows.Point offset)
        {
            CustomPopupPlacement[] ttplaces = new CustomPopupPlacement[] { new CustomPopupPlacement() };
            if (PlacementTarget != null && PlacementTarget is Border)
            {
                if (FlowDirection == FlowDirection.RightToLeft)
                {
                    ttplaces[0].Point = new System.Windows.Point(-(PlacementTarget as Border).ActualWidth, 0);
                }
                else
                {
                    ttplaces[0].Point = new System.Windows.Point(1, 1);
                }

                ttplaces[0].PrimaryAxis = PopupPrimaryAxis.Horizontal;
                Width = (PlacementTarget as Border).ActualWidth;
                Height = (PlacementTarget as Border).ActualHeight;
                return ttplaces;
            }
            else
            {
                PlacementTarget = Parent as UIElement;

                if (FlowDirection == FlowDirection.RightToLeft)
                {
                    ttplaces[0].Point = new System.Windows.Point(-(PlacementTarget as FrameworkElement).ActualWidth - popupSize.Width, 0);
                }
                else
                {
                    ttplaces[0].Point = new System.Windows.Point((PlacementTarget as FrameworkElement).ActualWidth, 0);
                }

                return ttplaces;
            }
        }

        /// <summary>
        /// Handles the placing.
        /// </summary>
        private void HandlePlacing()
        {
            if (TemplatedParent is ApplicationMenu)
            {
                IntPtr hwndChild = ((HwndSource)PresentationSource.FromVisual(this.Child)).Handle;
                IntPtr hwndAppMenu = ((HwndSource)PresentationSource.FromVisual(this.TemplatedParent as Visual)).Handle;
                WindowInterop.RECT childRect = new WindowInterop.RECT();
                WindowInterop.RECT menuRect = new WindowInterop.RECT();
                WindowInterop.GetWindowRect(hwndChild, ref childRect);
                WindowInterop.GetWindowRect(hwndAppMenu, ref menuRect);
                if (childRect.top < menuRect.top)
                {
                    (TemplatedParent as ApplicationMenu).IsBelowAppButton = false;
                }
                else
                {
                    (TemplatedParent as ApplicationMenu).IsBelowAppButton = true;
                }

                if (FlowDirection == FlowDirection.RightToLeft)
                {
                    HorizontalOffset = -3.6;
                }
            }
        }
        #endregion
    }
}
