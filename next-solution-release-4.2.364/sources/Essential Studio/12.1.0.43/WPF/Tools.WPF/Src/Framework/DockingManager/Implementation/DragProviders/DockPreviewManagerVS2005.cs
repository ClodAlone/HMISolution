// <copyright file="DockPreviewManagerVS2005.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using Syncfusion.Windows.Shared;
using System;
using System.Diagnostics;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents docking manager's preview base for VS2005.
    /// </summary>
    /// <exclude/>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class DockPreviewManagerVS2005 : DockPreviewManagerVS2003
    {
        #region Constants

        /// <summary>
        /// Indicates Close button name.
        /// </summary>
        private const string CLOSE_BUTTON_NAME = "PART_CloseButton";

        /// <summary>
        /// Indicates context menu name.
        /// </summary>
        private const string CONTEXT_MENUBUTTON_NAME = "PART_ContextMenuButton";

        /// <summary>
        /// Indicated awl button name.
        /// </summary>
        private const string AWLBUTTON_NAME = "PART_AwlButton";

        /// <summary>
        /// Indicates prefix name.
        /// </summary>
        private const string PREFIX_NAME = "InternalName";

        /// <summary>
        /// Indicates content presenter name.
        /// </summary>
        private const string CONTENTPRESENTER_NAME = "PART_ContentPresenter";
        #endregion

        #region Private member
        /// <summary>
        /// Indicates center button.
        /// </summary>
        private DockPreviewMainButtonVS2005 m_centerButton;

        /// <summary>
        /// Indicates top button.
        /// </summary>
        private DockPreviewMainButtonVS2005 m_topButton;

        /// <summary>
        /// Indicates left button.
        /// </summary>
        private DockPreviewMainButtonVS2005 m_leftButton;

        /// <summary>
        /// Indicates right button.
        /// </summary>
        private DockPreviewMainButtonVS2005 m_rightButton;

        /// <summary>
        /// Indicates bottom button.
        /// </summary>
        private DockPreviewMainButtonVS2005 m_bottomButton;

        /// <summary>
        /// Indicates docking manager.
        /// </summary>
        private DockingManager m_manager;

        /// <summary>
        /// Indicates center drag provider adorner.
        /// </summary>
        private UIElementAdorner m_centerDragProviderAdorner;

        /// <summary>
        /// Indicates side drag provider adorner.
        /// </summary>
        private UIElementAdorner m_sideDragProviderAdorner;

        /// <summary>
        /// Indicates host adorner layer.
        /// </summary>
        private AdornerLayer m_hostAdornerLayer;

        /// <summary>
        /// Indicates docking adorner layer.
        /// </summary>
        private AdornerLayer m_dockingAdornerLayer;

        /// <summary>
        /// Indicates preview size.
        /// </summary>
        private double m_previewSize;

        /// <summary>
        /// Indicates dock preview main button's visibility.
        /// </summary>
        private bool m_isDockPreviewMainButtonVisible = false;
        #endregion

        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="DockPreviewManagerVS2005"/> class.
        /// </summary>
        /// <param name="manager">Specifies docking manager, the
        /// instance is bound to.</param>
        /// <property name="flag" value="Finished"/>
        public DockPreviewManagerVS2005(DockingManager manager)
            : base(manager)
        {
            m_topButton = InitializeButton(DockSide.Top, manager);
            m_leftButton = InitializeButton(DockSide.Left, manager);
            m_rightButton = InitializeButton(DockSide.Right, manager);
            m_bottomButton = InitializeButton(DockSide.Bottom, manager);
            m_manager = manager;
            m_centerButton = new DockPreviewMainButtonVS2005 { ParentDockingManager = manager };
            
        }

        /// <summary>
        /// Mins the mem.
        /// </summary>
        public void MinMem()
        {
            m_bottomButton = null;
            m_centerButton = null;
            m_centerDragProviderAdorner = null;
            m_dockingAdornerLayer = null;
            m_hostAdornerLayer = null;
            m_lastButton = null;
            m_leftButton = null;
            m_rightButton = null;

        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets value of the IsDockPreviewMainButtonVisible property.
        /// </summary>
        public override bool IsDockPreviewMainButtonVisible
        {
            get
            {
                return m_isDockPreviewMainButtonVisible;
            }
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Gets value of the ProviderAction dependency property.
        /// </summary>
        /// <param name="obj">DependencyObject that contains ProviderAction dependency property</param>
        /// <returns>action of Drag Provider</returns>
        public static DragProviderAction GetProviderAction(DependencyObject obj)
        {
            return (DragProviderAction)obj.GetValue(ProviderActionProperty);
        }

        /// <summary>
        /// Gets value to the ProviderAction dependency property.
        /// </summary>
        /// <param name="obj">DependencyObject that contains ProviderAction dependency property</param>
        /// <param name="value">action of Drag Provider</param>
        public static void SetProviderAction(DependencyObject obj, DragProviderAction value)
        {
            obj.SetValue(ProviderActionProperty, value);
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Checks the preview mode.
        /// </summary>
        private void CheckPreviewMode()
        {
            DockPreviewMainButtonVS2005[] buttons = new DockPreviewMainButtonVS2005[5] { m_leftButton, m_rightButton, m_topButton, m_bottomButton, m_centerButton };

            bool bUseAdornerDragProvider = DockingManager.UseAdornerDragProvider;

            foreach (DockPreviewMainButtonVS2005 button in buttons)
            {
                button.SetPreviewMode(bUseAdornerDragProvider);
            }
        }

        /// <summary>
        /// Removes the dock preview adorner.
        /// </summary>
        /// <param name="removeSideButtons">if set to <c>true</c> [remove side buttons].</param>
        private void RemoveDockPreviewAdorner(bool removeSideButtons)
        {
            if (m_hostAdornerLayer != null)
            {
                m_hostAdornerLayer.Remove(m_centerDragProviderAdorner);
                m_centerDragProviderAdorner.RemoveElement(m_centerButton);
                m_centerDragProviderAdorner = null;
                m_hostAdornerLayer = null;
            }

            if (removeSideButtons && m_dockingAdornerLayer != null)
            {
                m_dockingAdornerLayer.Remove(m_sideDragProviderAdorner);
                m_sideDragProviderAdorner.RemoveElement(m_leftButton);
                m_sideDragProviderAdorner.RemoveElement(m_topButton);
                m_sideDragProviderAdorner.RemoveElement(m_rightButton);
                m_sideDragProviderAdorner.RemoveElement(m_bottomButton);
                m_sideDragProviderAdorner = null;
                m_dockingAdornerLayer = null;
            }
        }

        /// <summary>
        /// Finds the place in docking window where an element can be docked to.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="host">DockedElementTabbedHost where an element can be docked to</param>
        /// <param name="point">the point where an element can be docked to</param>
        /// <returns>the place, an element can be docked to</returns>
        protected override DockPreviewRecord? FindDockingPlaceInternal(FrameworkElement element, DockedElementTabbedHost host, Point point)
        {
            DockPreviewRecord result = new DockPreviewRecord
            {
                State = host != null && host.DockingManager.Equals(DockingManager) ? host.State : DockState.Dock,
                Element = element,
                PreviewSize = m_previewSize
            };
            bool hasResult = CheckIfFeets(host, point, ref result);

            return hasResult ? (DockPreviewRecord?)result : null;
        }

        /// <summary>
        /// Creates dock preview where an element can be docked to.
        /// </summary>
        /// <param name="record">The record.</param>
        /// <param name="host">FrameworkElement where adorner can be created</param>
        /// <param name="size">The element Size.</param>
        /// <returns>adorner with templates support</returns>
        protected override TemplatedAdornerBase CreateDockPreviewAdorner(DockPreviewRecord record, FrameworkElement host, Size size)
        {
            HostAdornerVS2005 result = CheckHost(record, host);
            DockSide side = record.Side;
            if (m_manager.DragHostAdornerStyle != null)
            {
                try
                {
                    result.Style = m_manager.DragHostAdornerStyle;
                }
                //SU I78477
                //catch (Exception e)
                catch (Exception)
                    //EU I78477
                {
                }
            }
            if (m_manager.IsVS2010DraggingEnabled && m_manager.m_mousemoveonheaderpanel)
            {
                if (m_manager.m_draggedElement != null)
                {
                    TabControlExt tabcontrol = DockingManager.GetTabControl(m_manager.m_draggedElement as DependencyObject);
                    if (tabcontrol != null)
                    {
                        result.TabStripPlacement = tabcontrol.TabStripPlacement;
                    }
                    else
                    {
                        result.TabStripPlacement = Dock.Top;
                    }
                    if (m_manager.m_mouseonheaderpanelposition > 0)
                    {
                        result.TabAreaTopMargin = new Thickness((m_manager.m_mouseonheaderpanelposition), 0, 0, 0);
                    }
                    result.TabAreaWidth = m_manager.m_mouseonheaderpaneltabareawidth;
                }
            }
            else if (m_manager.IsVS2010DraggingEnabled && m_manager.m_mousemoveontdilayoutpanel)
            {
                if (m_manager.m_draggedElement != null)
                {
                    TabControlExt tabcontrol = DockingManager.GetTabControl(m_manager.m_draggedElement as DependencyObject);
                    if (tabcontrol != null)
                    {
                        result.TabStripPlacement = tabcontrol.TabStripPlacement;
                    }
                    else
                    {
                        result.TabStripPlacement = Dock.Top;
                    }
                }
            }
            else
            {
                result.TabStripPlacement = m_manager.DockTabAlignment;
            }
            result.Side = side;

            if (side != DockSide.Tabbed)
            {
                CalculatePreviewSize(ref record, host, size, result);
            }

            return result;
        }

        /// <summary>
        /// Check the host for container
        /// </summary>
        /// <param name="record">The record.</param>
        /// <param name="host">FrameworkElement where adorner can be created</param>
        /// <returns>HostAdornerVS2005</returns>
        protected HostAdornerVS2005 CheckHost(DockPreviewRecord record, FrameworkElement host)
        {
            if (host == DockingManager.GetDocumentContainerHost() && m_manager.IsVS2010DraggingEnabled)
            {
                if (m_manager.m_mousemoveonheaderpanel)
                {
                    Point point = Mouse.GetPosition(host);
                    HitTestResult testresult = VisualTreeHelper.HitTest(host, point);
                    if (testresult != null)
                    {
                        if (DockingManager != null && DockingManager.m_documentTabContrlsList.Count > 0)
                        {
                            foreach (DocumentTabControl tabControl in DockingManager.m_documentTabContrlsList)
                            {
                                if (tabControl != null && tabControl.Items.Count > 0)
                                {
                                    TabItemExt tabItemExt = tabControl.Items[0] as TabItemExt;
                                    if (tabItemExt != null)
                                    {
                                        HeaderPanel headerPanel = VisualUtils.FindAncestor(tabItemExt as Visual, typeof(HeaderPanel)) as HeaderPanel;
                                        if (headerPanel != null)
                                        {
                                            HitTestResult headerPanelHitTestResult = VisualTreeHelper.HitTest(headerPanel as Visual, Mouse.GetPosition(headerPanel));
                                            if (headerPanelHitTestResult != null)
                                            {
                                                TDILayoutPanel tdipanel = VisualUtils.FindAncestor(tabControl as Visual, typeof(TDILayoutPanel)) as TDILayoutPanel;

                                                if (tdipanel != null)
                                                    tdipanel.ActiveTabControl = tabControl;
                                                return new HostAdornerVS2005(tabControl);

                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return new HostAdornerVS2005(host);
        }

        /// <summary>
        /// Arranges the buttons.
        /// </summary>
        /// <param name="host">The FrameworkElement host.</param>
        /// <param name="bShowSideButtons">if set to <c>true</c> [b show side buttons].</param>
        private void ArrangeButtons(FrameworkElement host, bool bShowSideButtons)
        {
            double centerX, centerY, width, height, x, y;

            centerX = DockingManager.ActualWidth / 2;
            centerY = DockingManager.ActualHeight / 2;
            ContentPresenter mainContent = DockingManager.MainContent;

            width = m_topButton.ChildActualWidth;
            height = m_topButton.ChildActualHeight;
            SetDataToPreviewButton(m_topButton, (centerX - width / 2), 0, width, height, bShowSideButtons);

            width = m_leftButton.ChildActualWidth;
            height = m_leftButton.ChildActualHeight;
            SetDataToPreviewButton(m_leftButton, 0, (centerY - height / 2), width, height, bShowSideButtons);

            width = m_rightButton.ChildActualWidth;
            height = m_rightButton.ChildActualHeight;
            x = mainContent.ActualWidth - width;
            y = centerY - height / 2;
            SetDataToPreviewButton(m_rightButton, x, y, width, height, bShowSideButtons);

            width = m_bottomButton.ChildActualWidth;
            height = m_bottomButton.ChildActualHeight;
            x = centerX - width / 2;
            y = mainContent.ActualHeight - height;
            SetDataToPreviewButton(m_bottomButton, x, y, width, height, bShowSideButtons);

            width = m_centerButton.ChildActualWidth;
            height = m_centerButton.ChildActualHeight;
            x = host.RenderSize.Width / 2 - width / 2;
            y = host.RenderSize.Height / 2 - height / 2;
            SetDataToPreviewButton(m_centerButton, x, y, width, height, true);
        }

        /// <summary>
        /// Creates main button in internal dock preview.
        /// </summary>
        /// <param name="host">the host to create preview button in</param>
        /// <param name="bShowSideButtons">true if side buttons must be shown</param>
        protected override void CreateDockPreviewMainButtonInternal(FrameworkElement host, bool bShowSideButtons)
        {
            CheckPreviewMode();
            ArrangeButtons(host, bShowSideButtons);

            if (!DockingManager.UseAdornerDragProvider)
            {
                m_centerButton.IsOpen = false;
                m_centerButton.SetPlacement(host, PlacementMode.Center, Rect.Empty);
            }
            else
            {
                CreateDockPreviewMainButtonInternalBasedOnAdorners(host, bShowSideButtons);
            }

            if (m_isDockPreviewMainButtonVisible)
            {
                m_isLowPerform = true;
            }

            ShowButtons(bShowSideButtons);
            PrepareCenterButton(host);
            m_isDockPreviewMainButtonVisible = true;
        }

        /// <summary>
        /// Creates the dock preview main button internal based on adorners.
        /// </summary>
        /// <param name="host">The FrameworkElement host.</param>
        /// <param name="bShowSideButtons">if set to <c>true</c> [b show side buttons].</param>
        private void CreateDockPreviewMainButtonInternalBasedOnAdorners(FrameworkElement host, bool bShowSideButtons)
        {
            RemoveDockPreviewAdorner(false);
            m_hostAdornerLayer = AdornerLayer.GetAdornerLayer(host);
            m_dockingAdornerLayer = AdornerLayer.GetAdornerLayer(DockingManager.MainContent);

            m_leftButton.IsHitTestVisible = false;
            m_topButton.IsHitTestVisible = false;
            m_rightButton.IsHitTestVisible = false;
            m_bottomButton.IsHitTestVisible = false;

            if (m_sideDragProviderAdorner == null)
            {
                m_sideDragProviderAdorner = new UIElementAdorner(DockingManager.MainContent, new Canvas());
                m_dockingAdornerLayer.Add(m_sideDragProviderAdorner);

                m_sideDragProviderAdorner.AddElement(m_leftButton);
                m_sideDragProviderAdorner.AddElement(m_topButton);
                m_sideDragProviderAdorner.AddElement(m_rightButton);
                m_sideDragProviderAdorner.AddElement(m_bottomButton);
            }

            m_centerButton.IsHitTestVisible = false;
            m_centerDragProviderAdorner = new UIElementAdorner(host, new Canvas());
            m_hostAdornerLayer.Add(m_centerDragProviderAdorner);
            m_centerDragProviderAdorner.AddElement(m_centerButton);
        }

        /// <summary>
        /// Hides main button in internal dock preview.
        /// </summary>
        /// <param name="canSwitchPerform">true if perform can be switched</param>
        protected override void HideDockPreviewMainButtonInternal(bool canSwitchPerform)
        {
            RemoveDockPreviewAdorner(true);
            m_centerButton.IsOpen = false;
            ShowSideButtons(false);
            m_isDockPreviewMainButtonVisible = false;

            if (canSwitchPerform)
            {
                m_isLowPerform = false;
            }
        }

        /// <summary>
        /// Shows the buttons.
        /// </summary>
        /// <param name="bShowSideButtons">if set to <c>true</c> [b show side buttons].</param>
        private void ShowButtons(bool bShowSideButtons)
        {
            if (m_isLowPerform)
            {
                if (!DockingManager.UseAdornerDragProvider)
                {
                    m_centerButton.IsOpen = true;
                }

                ShowSideButtons(bShowSideButtons);
            }
            else
            {
                ThreadStart method = delegate
                {
                    bool bCenterIndicator = DockingManager.IsDragging && null != DockingManager.HostUnderMouse;
                    if (bCenterIndicator || !bShowSideButtons)
                    {
                        m_centerButton.IsOpen = bCenterIndicator;
                        ShowSideButtons(bShowSideButtons);
                    }
                };

                Dispatcher.BeginInvoke(DispatcherPriority.Input, new ThreadStart(method));
            }
        }

        /// <summary>
        /// Shows the side buttons.
        /// </summary>
        /// <param name="bShowSideButtons">if set to <c>true</c> [b show side buttons].</param>
        private void ShowSideButtons(bool bShowSideButtons)
        {
            if (!DockingManager.UseAdornerDragProvider)
            {
                m_topButton.IsOpen = bShowSideButtons;
                m_leftButton.IsOpen = bShowSideButtons;
                m_bottomButton.IsOpen = bShowSideButtons;
                m_rightButton.IsOpen = bShowSideButtons;
            }
        }

        /// <summary>
        /// Makes the side buttons inactive
        /// </summary>
        private void MakeSideButtonsUnactive()
        {
            m_topButton.IsSideButtonActive = false;
            m_leftButton.IsSideButtonActive = false;
            m_rightButton.IsSideButtonActive = false;
            m_bottomButton.IsSideButtonActive = false;
        }

        /// <summary>
        /// Sets the data to preview button.
        /// </summary>
        /// <param name="button">The dock preview main button VS2005.</param>
        /// <param name="x">The button's x value.</param>
        /// <param name="y">The button's y value.</param>
        /// <param name="width">The button's width value.</param>
        /// <param name="height">The button's height value.</param>
        /// <param name="bShowSideButtons">if set to <c>true</c> [b show side buttons].</param>
        private void SetDataToPreviewButton(DockPreviewMainButtonVS2005 button, double x, double y, double width, double height, bool bShowSideButtons)
        {
            if (bShowSideButtons)
            {
                if (!DockingManager.UseAdornerDragProvider)
                {
                    Rect rect = new Rect(x, y, width, height);
                    button.SetPlacement(DockingManager.MainContent, PlacementMode.Relative, rect);
                }
                else
                {
                    Canvas.SetLeft(button, x);
                    Canvas.SetTop(button, y);
                }
            }
        }

        /// <summary>
        /// Calculates the size of the preview.
        /// </summary>
        /// <param name="record">The dock preview record.</param>
        /// <param name="host">The UI element host.</param>
        /// <param name="size">The size value.</param>
        /// <param name="hostAdorner">The host adorner.</param>
        private void CalculatePreviewSize(ref DockPreviewRecord record, UIElement host, Size size, FrameworkElement hostAdorner)
        {
            bool bIsHorizontal = IsHorizontalDockSide(record.Side);
            double hostlength = bIsHorizontal ? host.RenderSize.Width : host.RenderSize.Height;
            double elementlength = bIsHorizontal ? size.Width : size.Height;

            if (elementlength > hostlength / 2)
            {
                elementlength = hostlength / 2;
            }

            if (bIsHorizontal)
            {
                hostAdorner.Width = elementlength;
                hostAdorner.Height = double.NaN;
            }
            else
            {
                hostAdorner.Height = elementlength;
                hostAdorner.Width = double.NaN;
            }

            record.PreviewSize = elementlength;
            m_previewSize = elementlength;
        }

        /// <summary>
        /// Prepares the center button.
        /// </summary>
        /// <param name="host">The framework element host.</param>
        private void PrepareCenterButton(FrameworkElement host)
        {
            DockedElementTabbedHost tHost = host as DockedElementTabbedHost;

            if (null != tHost && host.Visibility == Visibility.Visible)
            {
                FrameworkElement element = tHost.HostedElement;

                if (string.IsNullOrEmpty(element.Name) && !m_manager.UseDocumentContainer)
                {
                    m_centerButton.IsDisableCenter = true;
                }
                else
                {
                    m_centerButton.IsDisableCenter = false;
                }
                if (m_manager != null && m_manager.m_draggedElement != null)
                {
                    DockAbility ability = DockingManager.GetDockAbility(m_manager.m_draggedElement);
                    if ((host as DockedElementTabbedHost).HostedElement != null)
                    {
                        if (DockingManager.GetIsFixedHeight((host as DockedElementTabbedHost).HostedElement))
                            ability = DockAbility.Horizontal;
                        if (DockingManager.GetIsFixedWidth((host as DockedElementTabbedHost).HostedElement))
                            ability = DockAbility.Vertical;
                        if(DockingManager.GetIsFixedSize((host as DockedElementTabbedHost).HostedElement)
                            || (DockingManager.GetIsFixedHeight((host as DockedElementTabbedHost).HostedElement) && DockingManager.GetIsFixedWidth((host as DockedElementTabbedHost).HostedElement)))
                            ability = DockAbility.None;
                    }
                    m_centerButton.IsTopEnable = (ability & DockAbility.Top) == DockAbility.Top;
                    m_centerButton.IsLeftEnable = (ability & DockAbility.Left) == DockAbility.Left;
                    m_centerButton.IsRightEnable = (ability & DockAbility.Right) == DockAbility.Right;
                    m_centerButton.IsBottomEnable = (ability & DockAbility.Bottom) == DockAbility.Bottom;
                    m_centerButton.IsTabbedEnable = (ability & DockAbility.Tabbed) == DockAbility.Tabbed;
                }
            }
        }

        /// <summary>
        /// Indicates last button.
        /// </summary>
        private UIElement m_lastButton;

        /// <summary>
        /// Gets the drag provider action.
        /// </summary>
        /// <param name="elementHost">The element host.</param>
        /// <param name="point">The point.</param>
        /// <returns>return drag provider action.</returns>
        private DragProviderAction GetDragProviderAction(Visual elementHost, Point point)
        {
            DragProviderAction action = DragProviderAction.None;
            HitTestResult htResult = null;

            if (BrowserInteropHelper.IsBrowserHosted)
            {
                AdornerHitTestResult ahtResult = null;

                if (m_sideDragProviderAdorner != null && m_centerDragProviderAdorner != null &&
                    (m_sideDragProviderAdorner.IsLoaded || m_centerDragProviderAdorner.IsLoaded))
                {
                    point = Mouse.GetPosition(m_dockingAdornerLayer);

                    ahtResult = CheckHitTestForButton(m_leftButton, point, ahtResult, m_dockingAdornerLayer);
                    ahtResult = CheckHitTestForButton(m_topButton, point, ahtResult, m_dockingAdornerLayer);
                    ahtResult = CheckHitTestForButton(m_rightButton, point, ahtResult, m_dockingAdornerLayer);
                    ahtResult = CheckHitTestForButton(m_bottomButton, point, ahtResult, m_dockingAdornerLayer);

                    point = Mouse.GetPosition(m_hostAdornerLayer);
                    //ahtResult = ahtResult != null ? (ahtResult is Image) ? ahtResult : null : null;
                    ahtResult = CheckHitTestForButton(m_centerButton, point, ahtResult, m_hostAdornerLayer);

                    bool bAreEqualAdorners = false;

                    if (ahtResult != null)
                    {
                        bAreEqualAdorners = ahtResult.Adorner == m_sideDragProviderAdorner || ahtResult.Adorner == m_centerDragProviderAdorner;
                    }

                    if (bAreEqualAdorners)
                    {
                        m_lastButton = ahtResult.VisualHit as UIElement;
                        m_prevResult = ahtResult;
                    }
                    else if (ahtResult != null && m_lastButton != null)
                    {
                        Rect rect = new Rect(new Point(0, 0), m_lastButton.RenderSize);

                        if (rect.Contains(Mouse.GetPosition(m_lastButton)))
                        {
                            ahtResult = m_prevResult;
                        }
                    }

                    htResult = ahtResult;
                    ////htResult = bAreEqualAdorners ? ahtResult : m_prevResult;
                    ////m_prevResult = bAreEqualAdorners ? ahtResult : m_prevResult;
                    ////m_prevResult = ahtResult != null ? m_prevResult : null;
                }
            }
            else
            {
                Point globalPoint = new Point(0, 0);
                if ((elementHost as UIElement).IsVisible)
                {
                    globalPoint = VisualUtils.PointToScreen(elementHost as UIElement, point);
                }
                else
                {
                    (elementHost as UIElement).UpdateLayout();
                    globalPoint = PermissionHelper.GetSafePointToScreen(elementHost as UIElement, point);
                    globalPoint.X = globalPoint.X + Mouse.GetPosition(elementHost as UIElement).X;
                    globalPoint.Y = globalPoint.Y + Mouse.GetPosition(elementHost as UIElement).Y;
                }

                checkdockability();
                htResult = CheckHitTestForButton(m_centerButton, globalPoint, htResult);
                htResult = CheckHitTestForButton(m_leftButton, globalPoint, htResult);
                htResult = CheckHitTestForButton(m_topButton, globalPoint, htResult);
                htResult = CheckHitTestForButton(m_rightButton, globalPoint, htResult);
                htResult = CheckHitTestForButton(m_bottomButton, globalPoint, htResult);
            }

            if (htResult != null)
            {
                FrameworkElement visual = htResult.VisualHit as FrameworkElement;

                if (visual != null && visual.Visibility == Visibility.Visible)
                {
                    action = DockPreviewManagerVS2005.GetProviderAction(visual);
                }
            }

            return action;
        }




        private void checkdockability()
        {
            FrameworkElement Dragelemt = DockingManager.m_draggedElement as FrameworkElement;
            if(Dragelemt!=null)
            {
                DockAbility action=DockingManager.GetDockAbility(Dragelemt);
                OuterDockAbility outer = DockingManager.GetOuterDockAbility(Dragelemt);
                EnableButtons();
                if (DockingManager.UseOuterDockAbility)
                {
                    if (outer != OuterDockAbility.All)
                    {
                        SetOuterDockAbility(outer);
                    }
                }
                else
                {
                    if (action != DockAbility.All)
                    {
                        SetDockAbility(action.ToString());
                    }
                }
            }

        }

        private void SetOuterDockAbility(OuterDockAbility ability)
        {
            m_topButton.Visibility = ((ability & OuterDockAbility.Top) == OuterDockAbility.Top) ? Visibility.Visible : Visibility.Collapsed;
            m_leftButton.Visibility = ((ability & OuterDockAbility.Left) == OuterDockAbility.Left) ? Visibility.Visible : Visibility.Collapsed;
            m_rightButton.Visibility =((ability & OuterDockAbility.Right) == OuterDockAbility.Right) ? Visibility.Visible : Visibility.Collapsed;
            m_bottomButton.Visibility =((ability & OuterDockAbility.Bottom) == OuterDockAbility.Bottom) ? Visibility.Visible : Visibility.Collapsed;
        }

        private void EnableButtons()
        {
            m_topButton.Visibility = Visibility.Visible;
            m_leftButton.Visibility = Visibility.Visible;
            m_rightButton.Visibility = Visibility.Visible;
            m_bottomButton.Visibility = Visibility.Visible;
            m_centerButton.Visibility = Visibility.Visible;
        }

        private void SetDockAbility(string dockablity)
        {
            switch (dockablity.ToUpper())
            {
                case "LEFT":
                    m_topButton.Visibility = Visibility.Collapsed;
                    m_rightButton.Visibility = Visibility.Collapsed;
                    m_bottomButton.Visibility = Visibility.Collapsed;
                    break;

                case "RIGHT":
                    m_leftButton.Visibility = Visibility.Collapsed;
                    m_topButton.Visibility = Visibility.Collapsed;
                    m_bottomButton.Visibility = Visibility.Collapsed;
                    break;

                case "HORIZONTAL":
                    m_topButton.Visibility = Visibility.Collapsed;
                    m_bottomButton.Visibility = Visibility.Collapsed;
                    break;

                case "BOTTOM":
                    m_leftButton.Visibility = Visibility.Collapsed;
                    m_topButton.Visibility = Visibility.Collapsed;
                    m_rightButton.Visibility = Visibility.Collapsed;

                    break;

                case "TOP":
                    m_leftButton.Visibility = Visibility.Collapsed;
                    m_rightButton.Visibility = Visibility.Collapsed;
                    m_bottomButton.Visibility = Visibility.Collapsed;
                    break;

                case "VERTICAL":
                    m_leftButton.Visibility = Visibility.Collapsed;
                    m_rightButton.Visibility = Visibility.Collapsed;
                    break;

                case "NONE":
                    m_leftButton.Visibility = Visibility.Collapsed;
                    m_topButton.Visibility = Visibility.Collapsed;
                    m_rightButton.Visibility = Visibility.Collapsed;
                    m_bottomButton.Visibility = Visibility.Collapsed;
                    break;

                case "TABBED":
                    m_leftButton.Visibility = Visibility.Collapsed;
                    m_topButton.Visibility = Visibility.Collapsed;
                    m_rightButton.Visibility = Visibility.Collapsed;
                    m_bottomButton.Visibility = Visibility.Collapsed;
                    break;

            }
        }

        /// <summary>
        /// Checks if feet's
        /// </summary>
        /// <param name="elementHost">The element host.</param>
        /// <param name="point">The point.</param>
        /// <param name="result">The result.</param>
        /// <returns>return bool value.</returns>
        private bool CheckIfFeets(UIElement elementHost, Point point, ref DockPreviewRecord result)
        {
            DockedElementTabbedHost host = (DockedElementTabbedHost)elementHost;
            bool foundInCenter = false;
            bool foundInSide = false;
            FrameworkElement Dragelemt = DockingManager.m_draggedElement as FrameworkElement;
            if (elementHost != null && elementHost.Visibility != Visibility.Collapsed)
            {
                DragProviderAction action = GetDragProviderAction(elementHost, point);
                if (m_manager.m_mousemoveonheaderpanel && m_manager.IsVS2010DraggingEnabled)
                {
                    if (action == DragProviderAction.None)
                    {
                        action = DragProviderAction.Center;
                    }
                }
                MakeSideButtonsUnactive();
                result.Action = action;

                switch (action)
                {
                    case DragProviderAction.Left:
                        result.Side = DockSide.Left;
                        foundInCenter = true;
                        break;

                    case DragProviderAction.Top:
                        result.Side = DockSide.Top;
                        foundInCenter = true;
                        break;

                    case DragProviderAction.Right:
                        result.Side = DockSide.Right;
                        foundInCenter = true;
                        break;

                    case DragProviderAction.Bottom:
                        result.Side = DockSide.Bottom;
                        foundInCenter = true;
                        break;

                    case DragProviderAction.GlobalLeft:
                        result.Side = DockSide.Left;
                        if (!(DockingManager.GetDockAbility(Dragelemt) == DockAbility.None))
                        {
                            m_leftButton.IsSideButtonActive = true;
                            foundInSide = true;
                        }
                        break;

                    case DragProviderAction.GlobalTop:
                        result.Side = DockSide.Top;
                        if (!(DockingManager.GetDockAbility(Dragelemt) == DockAbility.None))
                        {
                            m_topButton.IsSideButtonActive = true;
                            foundInSide = true;
                        }
                        break;

                    case DragProviderAction.GlobalRight:
                        result.Side = DockSide.Right;
                        if (!(DockingManager.GetDockAbility(Dragelemt) == DockAbility.None))
                        {
                            m_rightButton.IsSideButtonActive = true;
                            foundInSide = true;
                        }
                        break;

                    case DragProviderAction.GlobalBottom:
                        result.Side = DockSide.Bottom;
                        if (!(DockingManager.GetDockAbility(Dragelemt) == DockAbility.None))
                        {
                            m_bottomButton.IsSideButtonActive = true;
                            foundInSide = true;
                        }
                        break;
                }

                if (!(foundInSide || foundInCenter) && Dragelemt!=null)
                {
                    bool isTabbed = IsBeforeTabbed(host, point);

                    DockAbility dockableValue = DockingManager.GetDockAbility(Dragelemt);

                    if ((action == DragProviderAction.Center || isTabbed) &&
                                          (!host.HostedElement.Equals(DockingManager.ClientAreaContainer) || DockingManager.UseDocumentContainer) && (dockableValue == DockAbility.All || (dockableValue & DockAbility.Tabbed)  == DockAbility.Tabbed))
                    {
                        result.Side = DockSide.Tabbed;
                        foundInCenter = true;
                    }
                }

                DefineCenterButtonActiveSide(foundInSide, foundInCenter, host, ref result);
            }

            return foundInSide || foundInCenter;
        }

        /// <summary>
        /// Defines center button preview side when dragging window is inside another docking window.
        /// </summary>
        /// <param name="foundInSide">if dragging window found inside</param>
        /// <param name="foundInCenter">if dragging window found in center</param>
        /// <param name="host">DockedElementTabbedHost to dock dragged window</param>
        /// <param name="result">DockPreviewRecord to define the side</param>
        private void DefineCenterButtonActiveSide(bool foundInSide, bool foundInCenter, DockedElementTabbedHost host, ref DockPreviewRecord result)
        {
            if (foundInCenter)
            {
                result.TargetElement = (host != null) && DockingManager==host.DockingManager ? host.HostedElement : null;
                m_centerButton.CenterButtonActiveSide = result.Side;
            }
            else if (foundInSide)
            {
                m_centerButton.CenterButtonActiveSide = DockSide.None;
                result.TargetElement = null;
                result.State = DockState.Dock;
            }
            else
            {
                m_centerButton.CenterButtonActiveSide = DockSide.None;
            }
        }

        /// <summary>
        /// Initializes the button.
        /// </summary>
        /// <param name="side">The DockSide.</param>
        /// <param name="manager">The DockingManager.</param>
        /// <returns>return DockPreviewMainButtonVS2005</returns>
        private static DockPreviewMainButtonVS2005 InitializeButton(DockSide side, DockingManager manager)
        {
            return new DockPreviewMainButtonVS2005
            {
                DockType = side,
                ParentDockingManager = manager,
                IsOpen = false
            };
        }

        /// <summary>
        /// Checks the hit test for button.
        /// </summary>
        /// <param name="button">The button.</param>
        /// <param name="globalPoint">The global point.</param>
        /// <param name="htResult">The ht result.</param>
        /// <returns>return hit test result.</returns>
        private static HitTestResult CheckHitTestForButton(DockPreviewMainButtonVS2005 button, Point globalPoint, HitTestResult htResult)
        {
            if (null == htResult && ((FrameworkElement)button).IsLoaded)
            {
                htResult = VisualTreeHelper.HitTest(button, DockingManager.GetSafePointFromScreen(button, globalPoint));
            }

            return htResult;
        }

        /// <summary>
        /// Checks the hit test for button.
        /// </summary>
        /// <param name="button">The button.</param>
        /// <param name="point">The point.</param>
        /// <param name="htResult">The ht result.</param>
        /// <param name="layer">The layer.</param>
        /// <returns>Return AdornerHitTestResult value.</returns>
        private static AdornerHitTestResult CheckHitTestForButton(DockPreviewMainButtonVS2005 button, Point point, AdornerHitTestResult htResult, AdornerLayer layer)
        {
            if (null == htResult)
            {
                htResult = layer.AdornerHitTest(point);
            }

            return htResult;
        }

        /// <summary>
        /// Determines whether [is before tabbed] [the specified host].
        /// </summary>
        /// <param name="host">The DockedElementTabbedHost.</param>
        /// <param name="point">The point.</param>
        /// <returns>
        /// <c>true</c> if [is before tabbed] [the specified host]; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsBeforeTabbed(DockedElementTabbedHost host, Point point)
        {
            bool resultValue = false;

            if (DockingManager.GetNoHeader((DependencyObject)host.InternalDataContext) && 0 > point.Y)
            {
                resultValue = true;
            }
            else
            {
                HitTestResult hitTestResult = VisualTreeHelper.HitTest(host, point);

                if (null != hitTestResult)
                {
                    FrameworkElement visualHit = hitTestResult.VisualHit as FrameworkElement;

                    if (null != visualHit)
                    {
                        FrameworkElement hitTemplatedParent = (FrameworkElement)visualHit.TemplatedParent;

                        if ((null != hitTemplatedParent)
                            && ((hitTemplatedParent is TabControl
                            && IsInternalTabItem(hitTemplatedParent))
                            || (hitTemplatedParent is TabItem && IsInternalTabItem(hitTemplatedParent))
                            || (hitTemplatedParent is DockHeaderPresenter)
                            || (hitTemplatedParent is ContentPresenter && CONTENTPRESENTER_NAME == hitTemplatedParent.Name)
                            || (hitTemplatedParent.TemplatedParent is TabItem && IsInternalTabItem((FrameworkElement)hitTemplatedParent.TemplatedParent))
                            || (hitTemplatedParent is Button && IsInternalButton(hitTemplatedParent))
                            || (hitTemplatedParent is ToggleButton && AWLBUTTON_NAME == hitTemplatedParent.Name)))
                        {
                            resultValue = true;
                        }
                    }
                }
            }

            return resultValue;
        }

        /// <summary>
        /// Determines whether [is horizontal dock side] [the specified side].
        /// </summary>
        /// <param name="side">The dock side.</param>
        /// <returns>
        /// <c>true</c> if [is horizontal dock side] [the specified side]; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsHorizontalDockSide(DockSide side)
        {
            return side == DockSide.Left || side == DockSide.Right;
        }

        /// <summary>
        /// Determines whether [is internal button] [the specified button].
        /// </summary>
        /// <param name="button">The button.</param>
        /// <returns>
        /// <c>true</c> if [is internal button] [the specified button]; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsInternalButton(IFrameworkInputElement button)
        {
            string buttonName = button.Name;
            return CLOSE_BUTTON_NAME == buttonName || CONTEXT_MENUBUTTON_NAME == buttonName;
        }

        /// <summary>
        /// Determines whether [is internal tab item] [the specified tab item].
        /// </summary>
        /// <param name="tabItem">The tab item.</param>
        /// <returns>
        /// <c>true</c> if [is internal tab item] [the specified tab item]; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsInternalTabItem(IFrameworkInputElement tabItem)
        {
            return tabItem.Name.StartsWith(PREFIX_NAME);
        }
        #endregion

        #region Dependency properties
        /// <summary>
        /// Identifies DockPreviewManagerVS2005.ProviderAction dependency property.
        /// </summary>
        public static readonly DependencyProperty ProviderActionProperty =
            DependencyProperty.RegisterAttached("ProviderAction", typeof(DragProviderAction), typeof(DockPreviewManagerVS2005), new UIPropertyMetadata(DragProviderAction.None));
        #endregion
    }
}
