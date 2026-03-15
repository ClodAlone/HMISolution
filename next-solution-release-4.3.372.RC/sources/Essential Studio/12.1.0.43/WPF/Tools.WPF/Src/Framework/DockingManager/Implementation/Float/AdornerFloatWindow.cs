// <copyright file="AdornerFloatWindow.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents the Adorner float window
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class AdornerFloatWindow : ContentControl, IWindow
    {
        #region Class fields

        /// <summary>
        /// Indicates the hit test internal.
        /// </summary>
        private bool m_hitTestInternal = true;

        /// <summary>
        /// Indicates adorner window layout panel.
        /// </summary>
        private readonly AdornerWindowsLayoutPanel m_panel;

        #endregion

        #region IWindow Members

        /// <summary>
        /// Gets or sets the placement rectangle.
        /// </summary>
        /// <value>The placement rectangle.</value>
        public Rect PlacementRectangle
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the primary element.
        /// </summary>
        /// <value>The primary element.</value>
        public FrameworkElement PrimaryElement
        {
            get
            {
                return (FrameworkElement)GetValue(PrimaryElementProperty);
            }

            set
            {
                SetValue(PrimaryElementProperty, value);
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
                AutoTemplatedContentControl child = (AutoTemplatedContentControl)Child;
                return child.Content as FrameworkElement;
            }

            set
            {
                AutoTemplatedContentControl child = (AutoTemplatedContentControl)Child;
                FrameworkElement content = value;

                if (value is DockedElementTabbedHost)
                {
                    DockedElementTabbedHost host = value as DockedElementTabbedHost;

                    DockedElementsContainer container = new DockedElementsContainer(DockingManager);
                    if (host.Parent != null && (host.Parent as DockedElementsContainer) != null)
                    {
                        (host.Parent as DockedElementsContainer).RemoveChild(host);
                    }
                    container.Children.Add(host);
                    content = container;
                }

                child.Content = content;
            }
        }

        /// <summary>
        /// Gets or sets the child.
        /// </summary>
        /// <value>The child.</value>
        public UIElement Child
        {
            get
            {
                return Content as UIElement;
            }

            set
            {
                Content = value;
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
        /// Gets or sets the docking manager.
        /// </summary>
        /// <value>The docking manager.</value>
        public DockingManager DockingManager
        {
            get
            {
                return (DockingManager)GetValue(DockingManagerProperty);
            }

            set
            {
                SetValue(DockingManagerProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [hit test disabled].
        /// </summary>
        /// <value><c>true</c> if [hit test disabled]; otherwise, <c>false</c>.</value>
        public bool HitTestDisabled
        {
            get
            {
                return m_hitTestInternal;
            }

            set
            {
                m_hitTestInternal = value;
                ////IsHitTestVisible = value;
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
                return (bool)GetValue(IsDraggingProperty);
            }

            set
            {
                SetValue(IsDraggingProperty, value);
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
                return (bool)GetValue(IsMultiHostsContainerProperty);
            }

            set
            {
                SetValue(IsMultiHostsContainerProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is open.
        /// </summary>
        /// <value><c>true</c> if this instance is open; otherwise, <c>false</c>.</value>
        public bool IsOpen
        {
            get
            {
                return Visibility == Visibility.Visible;
            }

            set
            {
                Visibility = value ? Visibility.Visible : Visibility.Collapsed;
                UpdatePlacement();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [allows transparency].
        /// </summary>
        /// <value><c>true</c> if [allows transparency]; otherwise, <c>false</c>.</value>
        public bool AllowsTransparency
        {
            get;
            set;
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
            Opacity = 1;
            HitTestDisabled = false;
            IsDragging = false;

            if (Header != null && Header is FloatWindowBorder)
            {
                (Header as FloatWindowBorder).CompleteDragging();
            }
        }

        /// <summary>
        /// Updates the is multi host property.
        /// </summary>
        public void UpdateIsMultiHostProperty()
        {
            DockedElementsContainer container = (Child as AutoTemplatedContentControl).Content as DockedElementsContainer;
            int hostsCount = GetVisibleHostsCount(container);
            bool bIsMultiHost = hostsCount > 1;

            if (hostsCount > 0)
            {
                if (bIsMultiHost != IsMultiHostsContainer)
                {
                    IsMultiHostsContainer = bIsMultiHost;

                    if (!bIsMultiHost && PrimaryElement != null)
                    {
                        InternalDataContext = DockingManager.GetTabbedHost(PrimaryElement, DockState.Float);
                    }
                }

                List<FrameworkElement> tabs = DockingManager.GetContainerTabs(container);

                foreach (FrameworkElement tab in tabs)
                {
                    if (!IsMultiHostsContainer)
                    {
                        DockingManager.SetPreviousNoHeader(tab, DockingManager.GetNoHeader(tab));
                    }
                    DockingManager.SetNoHeader(tab, !IsMultiHostsContainer);
                }
            }
            else
            {
                IsOpen = false;
            }
        }

        /// <summary>
        /// Gets the visible hosts count.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <returns>return DockedElementContainer.</returns>
        public int GetVisibleHostsCount(DockedElementsContainer container)
        {
            List<DockedElementTabbedHost> hosts = DockingManager.GetContainerHosts(container);
            int iCount = 0;

            foreach (DockedElementTabbedHost host in hosts)
            {
                if (host.Visibility == Visibility.Visible && ++iCount > 1)
                {
                    break;
                }
            }

            return iCount;
        }

        /// <summary>
        /// Sets the new primary element.
        /// </summary>
        /// <param name="element">The element.</param>
        public void SetNewPrimaryElement(FrameworkElement element)
        {
            if (element != null)
            {
                Rect rect = DockingManager.GetFloatingWindowRect(PrimaryElement);
                DockingManager.SetFloatingWindowRect(element, rect);
                InternalDataContext = DockingManager.GetTabbedHost(element, DockState.Float);
                BindingUtils.SetBinding(this, element, Popup.PlacementRectangleProperty, DockingManager.FloatingWindowRectProperty, BindingMode.TwoWay);
            }

            PrimaryElement = element;
        }

        /// <summary>
        /// Updates the data context.
        /// </summary>
        public void UpdateDataContext()
        {
            DockedElementsContainer container = (Child as AutoTemplatedContentControl).Content as DockedElementsContainer;
            List<DockedElementTabbedHost> hosts = DockingManager.GetContainerHosts(container);
            bool bHasVisibleHost = false;

            foreach (DockedElementTabbedHost host in hosts)
            {
                if (host.Visibility == Visibility.Visible)
                {
                    InternalDataContext = host;
                    bHasVisibleHost = true;
                    break;
                }
            }

            if (!bHasVisibleHost)
            {
                IsOpen = false;
            }
        }

        /// <summary>
        /// Sets the window on top.
        /// </summary>
        public void SetWindowOnTop()
        {
            m_panel.SetOnTop(this);
        }
        #endregion

        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="AdornerFloatWindow"/> class.
        /// </summary>
        /// <param name="docking">The docking.</param>
        /// <param name="panel">The panel.</param>
        public AdornerFloatWindow(DockingManager docking, AdornerWindowsLayoutPanel panel)
        {
            DockingManager = docking;
            m_panel = panel;

            AutoTemplatedContentControl child = new AutoTemplatedContentControl(GetType());
            BindingUtils.SetBinding(child, docking, SkinStorage.VisualStyleProperty, SkinStorage.VisualStyleProperty);
            Child = child;
        }
        #endregion

        #region Implementation

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.UIElement.MouseLeftButtonDown"/> routed event is raised on this element. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs"/> that contains the event data. The event data reports that the left mouse button was pressed.</param>
        protected override void OnMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            if (e.StylusDevice == null || e.StylusDevice != null)
                SetWindowOnTop();
        }

#if !SyncfusionFramework3_5
        //protected override void OnTouchDown(TouchEventArgs e)
        //{
        //    if (DockingManager != null && DockingManager.IsTouchEnabled && DockingManager.m_TouchDeviceId == e.TouchDevice.Id)
        //        OnTouchLeftFingerDown(e);
        //    base.OnTouchDown(e);
        //}

        //private void OnTouchLeftFingerDown(TouchEventArgs e)
        //{
        //    if (DockingManager != null && DockingManager.IsTouchEnabled && DockingManager.m_TouchDeviceId == e.TouchDevice.Id)
        //        SetWindowOnTop();
        //}
#endif

        /// <summary>
        /// Updates the placement.
        /// </summary>
        private void UpdatePlacement()
        {
            if (IsOpen)
            {
                Arrange(AdornerWindowsLayoutPanel.GetPlacementRactangle(this));
            }
        }
        #endregion

        #region Dependendency properties
        /// <summary>
        /// Identifies PrimaryElement dependency property of the <see cref="FloatWindow"/>.
        /// </summary>
        public static readonly DependencyProperty PrimaryElementProperty
            = DependencyProperty.Register("PrimaryElement", typeof(FrameworkElement), typeof(AdornerFloatWindow));

        /// <summary>
        /// Identifies IsDragging dependency property of the <see cref="FloatWindow"/>.
        /// </summary>
        public static readonly DependencyProperty IsDraggingProperty =
            DependencyProperty.Register("IsDragging", typeof(bool), typeof(AdornerFloatWindow), new UIPropertyMetadata(false));

        /// <summary>
        /// Identifies IsMultiHostsContainer dependency property of the <see cref="FloatWindow"/>.
        /// </summary>
        internal static readonly DependencyProperty IsMultiHostsContainerProperty =
            DependencyProperty.Register("IsMultiHostsContainer", typeof(bool), typeof(AdornerFloatWindow), new UIPropertyMetadata(false));

        /// <summary>
        /// Represents the DockingManager property
        /// </summary>
        public static readonly DependencyProperty DockingManagerProperty =
            DependencyProperty.Register("DockingManager", typeof(DockingManager), typeof(AdornerFloatWindow));
        #endregion
    }
}
