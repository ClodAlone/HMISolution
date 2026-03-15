// <copyright file="DockPreviewMainButtonVS2005.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Interop;
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents docking manager's floating window and helper frame
    /// internal window.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class DockPreviewMainButtonVS2005 : ContentControl
    {
        #region Constants
        
        /// <summary>
        /// Indicates WM_NCHITTEST.
        /// </summary>
        private const int WM_NCHITTEST = 0x0084;
        
        /// <summary>
        /// Indicates HTTRANSPARENT.
        /// </summary>
        private const int HTTRANSPARENT = -1;

        #endregion

        #region Properties
        
        /// <summary>
        /// Gets or sets the internal popup.
        /// </summary>
        /// <value>The internal popup.</value>
        internal Popup InternalPopup
        {
            get;
            set;
        }
        
        /// <summary>
        /// Gets or sets a value indicating whether this instance is open.
        /// </summary>
        /// <value><c>true</c> if this instance is open; otherwise, <c>false</c>.</value>
        public bool IsOpen
        {
            get
            {
                return (InternalPopup != null) ? InternalPopup.IsOpen : false;
            }

            set
            {
                if (InternalPopup != null)
                {
                    InternalPopup.IsOpen = value;
                }
            }
        }
        
        /// <summary>
        /// Gets or sets the child.
        /// </summary>
        /// <value>The child.</value>
        public FrameworkElement Child
        {
            get
            {
                return Content as FrameworkElement;
            }

            set
            {
                Content = value;
            }
        }

        /// <summary>
        /// Gets or sets the center button active side.
        /// </summary>
        /// <value>The center button active side.</value>
        public DockSide CenterButtonActiveSide
        {
            get
            {
                return (DockSide)GetValue(CenterButtonActiveSideProperty);
            }

            set
            {
                SetValue(CenterButtonActiveSideProperty, value);
            }
        }
        
        /// <summary>
        /// Gets or sets the type of the dock.
        /// </summary>
        /// <value>The type of the dock.</value>
        public DockSide DockType
        {
            get
            {
                return (DockSide)GetValue(DockTypeProperty);
            }

            set
            {
                SetValue(DockTypeProperty, value);
            }
        }
        
        /// <summary>
        /// Gets or sets a value indicating whether this instance is side button active.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is side button active; otherwise, <c>false</c>.
        /// </value>
        public bool IsSideButtonActive
        {
            get
            {
                return (bool)GetValue(IsSideButtonActiveProperty);
            }

            set
            {
                SetValue(IsSideButtonActiveProperty, value);
            }
        }
       
        /// <summary>
        /// Gets or sets ParentDockingManager of the <see cref="DockingManager"/>. This is a dependency property.
        /// </summary>
        public DockingManager ParentDockingManager
        {
            get
            {
                return (DockingManager)GetValue(ParentDockingManagerProperty);
            }

            set
            {
                SetValue(ParentDockingManagerProperty, value);
            }
        }
        
        /// <summary>
        /// Gets ChildActualWidth of the <see cref="DockPreviewMainButtonVS2005"/>
        /// </summary>
        public double ChildActualWidth
        {
            get
            {
                CheckInitSize();
                return Child.DesiredSize.Width;
            }
        }

        /// <summary>
        /// Gets the actual height of the child.
        /// </summary>
        /// <value>The actual height of the child.</value>
        public double ChildActualHeight
        {
            get
            {
                CheckInitSize();
                return Child.DesiredSize.Height;
            }
        }
        
        /// <summary>
        /// Gets or sets a value indicating whether this instance is disable center.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is disable center; otherwise, <c>false</c>.
        /// </value>
        public bool IsDisableCenter
        {
            get
            {
                return (bool)GetValue(IsDisableCenterProperty);
            }

            set
            {
                SetValue(IsDisableCenterProperty, value);
            }
        }
        
        /// <summary>
        /// Gets or sets a value indicating whether this instance is top enable.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is top enable; otherwise, <c>false</c>.
        /// </value>
        public bool IsTopEnable
        {
            get
            {
                return (bool)GetValue(IsTopEnableProperty);
            }

            set
            {
                SetValue(IsTopEnableProperty, value);
            }
        }
        
        /// <summary>
        /// Gets or sets a value indicating whether this instance is bottom enable.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is bottom enable; otherwise, <c>false</c>.
        /// </value>
        public bool IsBottomEnable
        {
            get
            {
                return (bool)GetValue(IsBottomEnableProperty);
            }

            set
            {
                SetValue(IsBottomEnableProperty, value);
            }
        }
        
        /// <summary>
        /// Gets or sets a value indicating whether this instance is left enable.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is left enable; otherwise, <c>false</c>.
        /// </value>
        public bool IsLeftEnable
        {
            get
            {
                return (bool)GetValue(IsLeftEnableProperty);
            }

            set
            {
                SetValue(IsLeftEnableProperty, value);
            }
        }
        
        /// <summary>
        /// Gets or sets a value indicating whether this instance is right enable.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is right enable; otherwise, <c>false</c>.
        /// </value>
        public bool IsRightEnable
        {
            get
            {
                return (bool)GetValue(IsRightEnableProperty);
            }

            set
            {
                SetValue(IsRightEnableProperty, value);
            }
        }
        
        /// <summary>
        /// Gets or sets a value indicating whether this instance is tabbed enable.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is tabbed enable; otherwise, <c>false</c>.
        /// </value>
        public bool IsTabbedEnable
        {
            get
            {
                return (bool)GetValue(IsTabbedEnableProperty);
            }

            set
            {
                SetValue(IsTabbedEnableProperty, value);
            }
        }
        #endregion

        #region Initialization
        
        /// <summary>
        /// Initializes a new instance of the <see cref="DockPreviewMainButtonVS2005"/> class.
        /// </summary>
        public DockPreviewMainButtonVS2005()
        {
            Child = new AutoTemplatedContentControl(GetType());
        }

        /// <summary>
        /// Called when [internal popup opened].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnInternalPopupOpened(object sender, EventArgs e)
        {
            if (PermissionHelper.HasUnmanagedCodePermission)
            {
                OnOpenedSecure();
            }
            //this.Unloaded += new RoutedEventHandler(DockPreviewMainButtonVS2005_Unloaded);
        }

        /// <summary>
        /// Handles the Unloaded event of the DockPreviewMainButtonVS2005 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void DockPreviewMainButtonVS2005_Unloaded(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
        }

       
        #endregion

        #region Implemantation
        
        /// <summary>
        /// Sets the preview mode.
        /// </summary>
        /// <param name="isAdornerMode">if set to <c>true</c> [is adorner mode].</param>
        internal void SetPreviewMode(bool isAdornerMode)
        {
            if (isAdornerMode)
            {
                if (InternalPopup != null)
                {
                    InternalPopup.IsOpen = true;
                    InternalPopup.Child = new UIElement();
                    InternalPopup.IsOpen = false;
                }
            }
            else
            {
                if (InternalPopup == null)
                {
                    InternalPopup = new Popup
                    {
                        AllowsTransparency = true,
                        PopupAnimation = PopupAnimation.Fade,
                        Child = this
                    };

                    InternalPopup.Opened += new EventHandler(OnInternalPopupOpened);
                }
            }
        }
        
        /// <summary>
        /// Sets the placement.
        /// </summary>
        /// <param name="target">The UI Element's target.</param>
        /// <param name="mode">The placement mode.</param>
        /// <param name="rect">The rect value.</param>
        internal void SetPlacement(UIElement target, PlacementMode mode, Rect rect)
        {
            InternalPopup.Placement = mode;
            InternalPopup.PlacementTarget = target;
            InternalPopup.PlacementRectangle = rect;
        }

        /// <summary>
        /// Gets the template child internal.
        /// </summary>
        /// <returns>return Child.</returns>
        internal DependencyObject GetTemplateChildInternal()
        {
            return GetTemplateChild("TopImg");
        }
        
        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.DragDrop.PreviewDragOver"/>�attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.DragEventArgs"/> that contains the event data.</param>
        protected override void OnPreviewDragOver(DragEventArgs e)
        {
            base.OnPreviewDragOver(e);
        }
        
        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.PreviewMouseMove"/>�attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseEventArgs"/> that contains the event data.</param>
        protected override void OnPreviewMouseMove(System.Windows.Input.MouseEventArgs e)
        {
            base.OnPreviewMouseMove(e);
        }
       
        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.MouseEnter"/>�attached event is raised on this element. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseEventArgs"/> that contains the event data.</param>
        protected override void OnMouseEnter(System.Windows.Input.MouseEventArgs e)
        {
            if (BrowserInteropHelper.IsBrowserHosted && !PermissionHelper.HasUnmanagedCodePermission)
            {
                if (ParentDockingManager != null && e.StylusDevice == null || e.StylusDevice != null)
                    ParentDockingManager.ShowDockPreviewInternal(e);
            }

            base.OnMouseEnter(e);
        }
       
        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.MouseUp"/>�routed event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs"/> that contains the event data. The event data reports that the mouse button was released.</param>
        protected override void OnMouseUp(System.Windows.Input.MouseButtonEventArgs e)
        {
            if (BrowserInteropHelper.IsBrowserHosted && !PermissionHelper.HasUnmanagedCodePermission)
            {
                if (ParentDockingManager != null && e.StylusDevice == null || e.StylusDevice != null)
                    ParentDockingManager.OnMouseUpInernal(e);
            }

            base.OnMouseUp(e);
        }
       
        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.MouseMove"/>�attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseEventArgs"/> that contains the event data.</param>
        protected override void OnMouseMove(System.Windows.Input.MouseEventArgs e)
        {
            base.OnMouseMove(e);
        }

#if !SyncfusionFramework3_5
        //protected override void OnTouchEnter(System.Windows.Input.TouchEventArgs e)
        //{
        //    if (BrowserInteropHelper.IsBrowserHosted && !PermissionHelper.HasUnmanagedCodePermission)
        //    {
        //        if (ParentDockingManager != null && ParentDockingManager.IsTouchEnabled && ParentDockingManager.m_TouchDeviceId == e.TouchDevice.Id)
        //            ParentDockingManager.ShowDockPreviewInternal(e);
        //    }
        //    base.OnTouchEnter(e);
        //}

        //protected override void OnTouchUp(System.Windows.Input.TouchEventArgs e)
        //{
        //    if (BrowserInteropHelper.IsBrowserHosted && !PermissionHelper.HasUnmanagedCodePermission)
        //    {
        //        if (ParentDockingManager != null && ParentDockingManager.IsTouchEnabled && ParentDockingManager.m_TouchDeviceId == e.TouchDevice.Id)
        //            ParentDockingManager.OnMouseUpInernal(e);
        //    }
        //    base.OnTouchUp(e);
        //}

#endif
       
        /// <summary>
        /// Called when [opened secure].
        /// </summary>
        private void OnOpenedSecure()
        {
            try
            {
                IntPtr hwnd = ((HwndSource)PresentationSource.FromVisual(Child)).Handle;
                NativeMethods.SetWindowPos(hwnd, -1, 0, 0, 0, 0, NativeConstants.SWP_NOSIZE | NativeConstants.SWP_NOMOVE | NativeConstants.SWP_NOACTIVATE);
                HwndSource.FromHwnd(hwnd).AddHook(new HwndSourceHook(HookMethod));
            }
            catch { }
        }

        /// <summary>
        /// Hooks the method.
        /// </summary>
        /// <param name="hwnd">The handler WND.</param>
        /// <param name="msg">The Window MSG.</param>
        /// <param name="wParam">The window param.</param>
        /// <param name="lParam">The interop param.</param>
        /// <param name="handled">if set to <c>true</c> [handled].</param>
        /// <returns>return IntPtr.</returns>
        private static IntPtr HookMethod(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case WM_NCHITTEST:
                    handled = true;
                    return new IntPtr(HTTRANSPARENT);
            }

            return new IntPtr(0);
        }
        
        /// <summary>
        /// Checks the size of the init.
        /// </summary>
        private void CheckInitSize()
        {
            if (Child.DesiredSize.Width == 0 || Child.DesiredSize.Height == 0)
            {
                Child.Measure(new Size(double.MaxValue, double.MaxValue));
            }
        }
        #endregion

        #region Dependency properties
        /// <summary>
        /// Identifies ParentDockingManager dependency property.
        /// </summary>
        public static readonly DependencyProperty ParentDockingManagerProperty =
            DependencyProperty.Register("ParentDockingManager", typeof(DockingManager), typeof(DockPreviewMainButtonVS2005), new UIPropertyMetadata(null));
       
        /// <summary>
        /// Identifies IsSideButtonActive dependency property.
        /// </summary>
        public static readonly DependencyProperty IsSideButtonActiveProperty =
            DependencyProperty.Register("IsSideButtonActive", typeof(bool), typeof(DockPreviewMainButtonVS2005), new UIPropertyMetadata(false));
        
        /// <summary>
        /// Identifies DockType dependency property.
        /// </summary>
        public static readonly DependencyProperty DockTypeProperty =
            DependencyProperty.Register("DockType", typeof(DockSide), typeof(DockPreviewMainButtonVS2005), new UIPropertyMetadata(DockSide.None));
      
        /// <summary>
        /// Identifies CenterButtonActiveSide dependency property.
        /// </summary>
        public static readonly DependencyProperty CenterButtonActiveSideProperty =
            DependencyProperty.Register("CenterButtonActiveSide", typeof(DockSide), typeof(DockPreviewMainButtonVS2005), new UIPropertyMetadata(DockSide.None));
        
        /// <summary>
        /// Identifies IsDisableCenter dependency property.
        /// </summary>
        public static readonly DependencyProperty IsDisableCenterProperty =
            DependencyProperty.Register("IsDisableCenter", typeof(bool), typeof(DockPreviewMainButtonVS2005), new UIPropertyMetadata(true));
       
        /// <summary>
        /// Identifies the IsTopEnable Dependency Property
        /// </summary>
        public static readonly DependencyProperty IsTopEnableProperty =
            DependencyProperty.Register("IsTopEnable", typeof(bool), typeof(DockPreviewMainButtonVS2005), new UIPropertyMetadata(true));
        
        /// <summary>
        /// Identifies the IsBottomEnable Dependency Property
        /// </summary>
        public static readonly DependencyProperty IsBottomEnableProperty =
            DependencyProperty.Register("IsBottomEnable", typeof(bool), typeof(DockPreviewMainButtonVS2005), new UIPropertyMetadata(true));
       
        /// <summary>
        /// Identifies the IsLeftEnable Dependency Property
        /// </summary>        
        public static readonly DependencyProperty IsLeftEnableProperty =
            DependencyProperty.Register("IsLeftEnable", typeof(bool), typeof(DockPreviewMainButtonVS2005), new UIPropertyMetadata(true));
        
        /// <summary>
        /// Identifies the IsRightEnable Dependency Property
        /// </summary>
        public static readonly DependencyProperty IsRightEnableProperty =
            DependencyProperty.Register("IsRightEnable", typeof(bool), typeof(DockPreviewMainButtonVS2005), new UIPropertyMetadata(true));
        
        /// <summary>
        /// identifies the IsTabbedEnable Dependency Property
        /// </summary>
        public static readonly DependencyProperty IsTabbedEnableProperty =
            DependencyProperty.Register("IsTabbedEnable", typeof(bool), typeof(DockPreviewMainButtonVS2005), new UIPropertyMetadata(true));
        #endregion
    }
}
