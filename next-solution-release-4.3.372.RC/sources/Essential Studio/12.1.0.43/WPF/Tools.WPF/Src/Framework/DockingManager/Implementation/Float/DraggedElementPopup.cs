// <copyright file="DraggedElementPopup.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Interop;
using Syncfusion.Windows.Shared;
using System.Windows.Markup;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents docking manager's floating window and helper frame
    /// internal window.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class DraggedElementPopup : NonStickingPopup, IWindow
    {
        #region Constants
        /// <summary>
        /// Specifies hit test value.
        /// </summary>
        private const int WM_NCHITTEST = 0x0084;

        /// <summary>
        /// Specifies HTTRANSPARENT.
        /// </summary>
        private const int HTTRANSPARENT = -1;

        /// <summary>
        /// Specifies prefix attachment name.
        /// </summary>
        private const string PrefixAttachName = "AttachName";
        #endregion

        #region Private member
        /// <summary>
        /// Specifies framework element.
        /// </summary>
        private FrameworkElement m_element = null;

        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets DragType of the <see cref="DraggedElementPopup"/>. This is a dependency property.
        /// </summary>
        /// <value>The type of the drag.</value>
        public DraggingType DragType
        {
            get
            {
                return (DraggingType)GetValue(DragTypeProperty);
            }

            set
            {
                SetValue(DragTypeProperty, value);
            }
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="DraggedElementPopup"/> class.
        /// </summary>
        public DraggedElementPopup()
        {
            AllowsTransparency = true;
            Placement = PlacementMode.Absolute;
            AutoTemplatedContentControl child = new AutoTemplatedContentControl(GetType());
            Child = child;
            this.Unloaded += new RoutedEventHandler(DraggedElementPopup_Unloaded);
        }

        void DraggedElementPopup_Unloaded(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
            this.Unloaded -= new RoutedEventHandler(DraggedElementPopup_Unloaded);
            Removehandle();
        }
        #endregion

        #region Implementation
        /// <summary>
        /// This method initialize starts the dragging.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="dragingList">The dragging list.</param>
        internal void StartDragging(FrameworkElement element, List<FrameworkElement> dragingList)
        {
            m_element = element;
            PlacementRectangle = DockingManager.GetFloatingWindowRect(element);
            IsOpen = true;
        }

        /// <summary>
        /// This method initialize completes the dragging.
        /// </summary>
        /// <param name="setFloatingRect">if set to <c>true</c> [set floating rect].</param>
        internal void CompleteDragging(bool setFloatingRect)
        {
            if (m_element != null)
            {
                if (setFloatingRect)
                {
                    DockingManager.SetFloatingWindowRect(m_element, PlacementRectangle);
                }

                m_element = null;
            }

            IsOpen = false;
        }

        /// <summary>
        /// Raises the Opened event.
        /// </summary>
        /// <param name="e">An EventArgs that contains the event data.</param>
        protected override void OnOpened(EventArgs e)
        {
            if(PlacementRectangle.Width>0)
                Width = PlacementRectangle.Width;
            if(PlacementRectangle.Height>0)
                Height = PlacementRectangle.Height;

            if (PermissionHelper.HasUnmanagedCodePermission)
            {
                OnOpenedSecure();
            }

            base.OnOpened(e);
        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="DraggedElementPopup"/> is reclaimed by garbage collection.
        /// </summary>
        ~DraggedElementPopup()
        {
        }
        /// <summary>
        /// Called when [opened secure].
        /// </summary>
        private void OnOpenedSecure()
        {
            IntPtr hwnd = ((HwndSource)PresentationSource.FromVisual(Child)).Handle;

            NativeMethods.SetWindowPos(hwnd, -1, 0, 0, 0, 0, NativeConstants.SWP_NOSIZE | NativeConstants.SWP_NOMOVE | NativeConstants.SWP_NOACTIVATE);

            HwndSource.FromHwnd(hwnd).AddHook(new HwndSourceHook(HookMethod));
        }

        /// <summary>
        /// Hooks the method.
        /// </summary>
        /// <param name="hwnd">The handler WND.</param>
        /// <param name="msg">The window MSG.</param>
        /// <param name="wParam">The window param.</param>
        /// <param name="lParam">The lntptr param.</param>
        /// <param name="handled">if set to <c>true</c> [handled].</param>
        /// <returns>return Intptr value</returns>
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
        #endregion

        #region Dependendency properties
        /// <summary>
        /// Identifies DragType dependency property of the <see cref="DraggedElementPopup"/>.
        /// </summary>
        public static readonly DependencyProperty DragTypeProperty =
          DependencyProperty.Register("DragType", typeof(DraggingType), typeof(DraggedElementPopup), new UIPropertyMetadata(DraggingType.NormalDragging));
        #endregion

        #region IWindow Members
        /// <summary>
        /// Gets or sets the primary element.
        /// </summary>
        /// <value>The primary element.</value>
        public FrameworkElement PrimaryElement
        {
            get
            {
                return null;
            }

            set
            {
            }
        }

        /// <summary>
        /// Gets or sets the float child.
        /// </summary>
        /// <value>The float child.</value>
        public FrameworkElement FloatChild
        {
            get
            {
                return null;
            }

            set
            {
            }
        }

        /// <summary>
        /// Gets or sets the docking manager.
        /// </summary>
        /// <value>The docking manager.</value>
        public DockingManager DockingManager
        {
            get
            {
                return null;
            }

            set
            {
            }
        }

        /// <summary>
        /// Gets or sets the header.
        /// </summary>
        /// <value>The header.</value>
        public UIElement Header
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether [hit test disabled].
        /// </summary>
        /// <value><c>true</c> if [hit test disabled]; otherwise, <c>false</c>.</value>
        public bool HitTestDisabled
        {
            get
            {
                return false;
            }

            set
            {
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is dragging.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is dragging; otherwise, <c>false</c>.
        /// </value>
        public bool IsDragging
        {
            get
            {
                return false;
            }

            set
            {
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is multi hosts container.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is multi hosts container; otherwise, <c>false</c>.
        /// </value>
        public bool IsMultiHostsContainer
        {
            get
            {
                return false;
            }

            set
            {
            }
        }

        /// <summary>
        /// Gets or sets the data context.
        /// </summary>
        /// <value>The data context.</value>
        public FrameworkElement InternalDataContext
        {
            get
            {
                return (FrameworkElement)DockingManager.GetInternalDataContext(this);
            }

            set
            {
                DockingManager.SetInternalDataContext(this, value);
            }
        }

        /// <summary>
        /// Completes the dragging.
        /// </summary>
        public void CompleteDragging()
        {
        }

        /// <summary>
        /// Updates the is multi host property.
        /// </summary>
        public void UpdateIsMultiHostProperty()
        {
        }

        /// <summary>
        /// Gets the visible hosts count.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <returns>return zero.</returns>
        public int GetVisibleHostsCount(DockedElementsContainer container)
        {
            return 0;
        }

        /// <summary>
        /// Sets the new primary element.
        /// </summary>
        /// <param name="element">The element.</param>
        public void SetNewPrimaryElement(FrameworkElement element)
        {
        }

        /// <summary>
        /// Updates the data context.
        /// </summary>
        public void UpdateDataContext()
        {
        }

        /// <summary>
        /// Sets the window on top.
        /// </summary>
        public void SetWindowOnTop()
        {
        }

        #endregion
    }
}