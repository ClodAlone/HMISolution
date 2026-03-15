// <copyright file="MainHost.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using Syncfusion.Windows.Shared;
using System.Windows.Interop;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents MainHost of the <see cref="DockingManager"/>.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class MainHost : ContentControl
    {
        #region Constants
        /// <summary>
        /// Specify top panel.
        /// </summary>
        private const string TOPPANEL_NAME = "PART_TopPanel";

        /// <summary>
        /// Specify left panel.
        /// </summary>
        private const string LEFTPANEL_NAME = "PART_LeftPanel";

        /// <summary>
        /// Specify right panel.
        /// </summary>
        private const string RIGHTPANEL_NAME = "PART_RightPanel";

        /// <summary>
        /// Specify bottom panel.
        /// </summary>
        private const string BOTTOMPANEL_NAME = "PART_BottomPanel";

        /// <summary>
        /// Specify panel number.
        /// </summary>
        private const int PANEL_NUMBER = 4;
        #endregion

        #region Private member
        /// <summary>
        /// Specify docking manager.
        /// </summary>
        private DockingManager m_Owner = null;

        /// <summary>
        /// Specify panel list.
        /// </summary>
        private List<SidePanel> m_PanelsList = new List<SidePanel>(PANEL_NUMBER);

        /// <summary>
        /// Specify top side panel.
        /// </summary>
        private SidePanel m_topSidePanel;
        /// <summary>
        /// Specify left side panel.
        /// </summary>
        private SidePanel m_leftSidePanel;

        /// <summary>
        /// Specify bottom side panel.
        /// </summary>
        private SidePanel m_bottomSidePanel;

        /// <summary>
        /// Specify right side panel.
        /// </summary>
        private SidePanel m_rightSidePanel;

        /// <summary>
        /// Specify content presenter.
        /// </summary>
        private ContentPresenter m_mainContent;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the content of the main.
        /// </summary>
        /// <value>The content of the main.</value>
        internal ContentPresenter MainContent
        {
            get
            {
                return m_mainContent;
            }
        }

        /// <summary>
        /// Gets the owner.
        /// </summary>
        /// <value>The owner.</value>
        internal DockingManager Owner
        {
            get
            {
                return m_Owner;
            }
        }
        #endregion

        #region Initialization

        /// <summary>
        /// Initializes static members of the <see cref="MainHost"/> class.
        /// </summary>
        static MainHost()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MainHost), new FrameworkPropertyMetadata(typeof(MainHost)));
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="MainHost"/> class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        public MainHost(DockingManager owner)
        {
            if (null != owner)
            {
                m_Owner = owner;
                BindingUtils.SetRelativeBinding(this, FrameworkElement.StyleProperty, typeof(DockingManager), DockingManager.MainHostStyleProperty, BindingMode.OneWay);
            }
            else
            {
                throw new ArgumentException("Parameter 'owner' is incorrect!");
            }
            //this.Unloaded += new RoutedEventHandler(MainHost_Unloaded);
            //this.Loaded += new RoutedEventHandler(MainHost_Loaded);
           

        }

       

        /// <summary>
        /// Setnulls this instance.
        /// </summary>
        internal void setnull()
        {
            m_bottomSidePanel.setnull();
            m_leftSidePanel.setnull();
            m_rightSidePanel.setnull();
            m_topSidePanel.setnull();
            Content = null;
            Template = null;
            m_bottomSidePanel = null;
            m_leftSidePanel = null;
            m_Owner = null;
            m_rightSidePanel = null;
            m_topSidePanel = null;
            m_PanelsList.Clear();
            m_PanelsList = null;
        }

        /// <summary>
        /// Gets the side panel list.
        /// </summary>
        /// <returns>return side panel list.</returns>
        internal List<SidePanel> GetSidePanelList()
        {
            List<SidePanel> panels = new List<SidePanel>();

            if (m_topSidePanel!=null && m_topSidePanel.Visibility == Visibility.Visible)
            {
                panels.Add(m_topSidePanel);
            }

            if (m_leftSidePanel!=null && m_leftSidePanel.Visibility == Visibility.Visible)
            {
                panels.Add(m_leftSidePanel);
            }

            if (m_bottomSidePanel!=null && m_bottomSidePanel.Visibility == Visibility.Visible)
            {
                panels.Add(m_bottomSidePanel);
            }

            if (m_rightSidePanel!=null && m_rightSidePanel.Visibility == Visibility.Visible)
            {
                panels.Add(m_rightSidePanel);
            }

            return panels;
        }

        /// <summary>
        /// Sets the panels list.
        /// </summary>
        private void SetPanelsList()
        {
            m_PanelsList.Clear();

            m_PanelsList.Add(m_topSidePanel);
            m_PanelsList.Add(m_leftSidePanel);
            m_PanelsList.Add(m_bottomSidePanel);
            m_PanelsList.Add(m_rightSidePanel);
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Builds the current template's visual tree.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            InitializeMembersOfChildren();
            SetPanelsList();
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Sets the element in side panel.
        /// </summary>
        /// <param name="element">The element.</param>
        internal void SetElemetInSidePanel(FrameworkElement element)
        {
            DockSide dock = DockingManager.GetTargetSide(m_Owner, element);

            switch (dock)
            {
                case DockSide.Top:
                    AddToSidPanel(m_topSidePanel, element);
                    break;
                case DockSide.Left:
                    AddToSidPanel(m_leftSidePanel, element);
                    break;
                case DockSide.Bottom:
                    AddToSidPanel(m_bottomSidePanel, element);
                    break;
                case DockSide.Right:
                    AddToSidPanel(m_rightSidePanel, element);
                    break;
                default:
                    throw new NotImplementedException("Enum was not supported.");
            }
        }

        /// <summary>
        /// Sets the element in side panel.
        /// </summary>
        /// <param name="element">The framework element.</param>
        /// <param name="dock">The dock value.</param>
        internal void SetElemetInSidePanel(FrameworkElement element, Dock dock)
        {
            switch (dock)
            {
                case Dock.Top:
                    AddToSidPanel(m_topSidePanel, element);
                    break;
                case Dock.Left:
                    AddToSidPanel(m_leftSidePanel, element);
                    break;
                case Dock.Bottom:
                    AddToSidPanel(m_bottomSidePanel, element);
                    break;
                case Dock.Right:
                    AddToSidPanel(m_rightSidePanel, element);
                    break;
            }
        }

        /// <summary>
        /// Gets the side panel.
        /// </summary>
        /// <param name="dock">The dock value</param>
        /// <returns>return side panel.</returns>
        internal SidePanel GetSidePanel(Dock dock)
        {
            SidePanel sidePanel = null;

            switch (dock)
            {
                case Dock.Top:
                    sidePanel = m_topSidePanel;
                    break;
                case Dock.Left:
                    sidePanel = m_leftSidePanel;
                    break;
                case Dock.Bottom:
                    sidePanel = m_bottomSidePanel;
                    break;
                case Dock.Right:
                    sidePanel = m_rightSidePanel;
                    break;
            }

            return sidePanel;
        }

        /// <summary>
        /// Removes from side panel.
        /// </summary>
        /// <param name="element">The element.</param>
        internal void RemoveFromSidePanel(FrameworkElement element)
        {
            if (IsLoaded)
            {
                RemoveFromeSidePanel(m_topSidePanel, element);
                RemoveFromeSidePanel(m_leftSidePanel, element);
                RemoveFromeSidePanel(m_bottomSidePanel, element);
                RemoveFromeSidePanel(m_rightSidePanel, element);
            }
        }

        /// <summary>
        /// Refreshes the sipe panel.
        /// </summary>
        internal void RefreshSipePanel()
        {
            foreach (SidePanel panel in m_PanelsList)
            {
                ListCollectionView view = (ListCollectionView)CollectionViewSource.GetDefaultView(panel.TabChildren);
                view.CustomSort = new SideTabChildComparer();
                view.Refresh();
            }
        }

        /// <summary>
        /// Selects as tab.
        /// </summary>
        /// <param name="element">The element.</param>
        internal void SelectAsTab(FrameworkElement element)
        {
            SidePanel sidePanel = null;

            foreach (SidePanel panel in m_PanelsList)
            {
                if (panel.TabChildren.Contains(element))
                {
                    sidePanel = panel;
                    break;
                }
            }

            if (null != sidePanel)
            {
                sidePanel.SelectTab(element);
            }
            /*
            else
            {
                throw new ArgumentOutOfRangeException();
            }
            */
        }

        /// <summary>
        /// Auto's the hide tab.
        /// </summary>
        /// <param name="element">The element.</param>
        internal void AutoHideTab(FrameworkElement element)
        {
            SidePanel sidePanel = null;

            foreach (SidePanel panel in m_PanelsList)
            {
                if (panel.TabChildren.Contains(element))
                {
                    sidePanel = panel;
                    break;
                }
            }

            if (null != sidePanel)
            {
                sidePanel.AutoHideTab(element);
            }
        }

        /// <summary>
        /// Clears the side panels.
        /// </summary>
        internal void ClearSidePanels()
        {
            if (m_PanelsList != null)
            {
                foreach (SidePanel panel in m_PanelsList)
                {
                    if (panel.TabChildren != null)
                    {
                        panel.TabChildren.Clear();
                    }
                }
            }
        }

        /// <summary>
        /// Gets the new panel.
        /// </summary>
        /// <param name="oldSidePanel">The old side panel.</param>
        /// <param name="panelName">Name of the panel.</param>
        /// <returns>return side panel value.</returns>
        private SidePanel GetNewPanel(SidePanel oldSidePanel, string panelName)
        {
            SidePanel result = GetTemplateChild(panelName) as SidePanel;

            if (null == result)
            {
                throw new NotImplementedException("Incorrect template!");
            }

            if (null != oldSidePanel)
            {
                oldSidePanel.IsKeyboardFocusWithinChanged -= new DependencyPropertyChangedEventHandler(OnSidePanelIsKeyboardFocusWithinChanged);
                oldSidePanel.MouseLeftButtonUp -= new System.Windows.Input.MouseButtonEventHandler(OnSidePanelMouseLeftButtonUp);
#if !SyncfusionFramework3_5
                //result.TouchUp -= result_TouchUp;
#endif

                if (oldSidePanel.HasItems)
                {
                    while (0 != oldSidePanel.TabChildren.Count)
                    {
                        FrameworkElement item = oldSidePanel.TabChildren[0];
                        oldSidePanel.TabChildren.Remove(item);
                        result.TabChildren.Add(item);
                    }
                }
            }

            result.IsKeyboardFocusWithinChanged += new DependencyPropertyChangedEventHandler(OnSidePanelIsKeyboardFocusWithinChanged);
            result.MouseLeftButtonUp += new MouseButtonEventHandler(OnSidePanelMouseLeftButtonUp);
#if !SyncfusionFramework3_5
            //result.TouchUp += result_TouchUp;
#endif
            result.Unloaded += new RoutedEventHandler(result_Unloaded);
            return result;
        }

        void result_Unloaded(object sender, RoutedEventArgs e)
        {
           SidePanel result = sender as SidePanel;
           result.IsKeyboardFocusWithinChanged -= new DependencyPropertyChangedEventHandler(OnSidePanelIsKeyboardFocusWithinChanged);
           result.MouseLeftButtonUp -= new MouseButtonEventHandler(OnSidePanelMouseLeftButtonUp);
#if !SyncfusionFramework3_5
           //result.TouchUp -= result_TouchUp;
#endif
           result.Unloaded -= new RoutedEventHandler(result_Unloaded);
        }

        /// <summary>
        /// Called when the <see cref="P:System.Windows.Controls.ContentControl.Content"/> property changes.
        /// </summary>
        /// <param name="oldContent">The old value of the <see cref="P:System.Windows.Controls.ContentControl.Content"/> property.</param>
        /// <param name="newContent">The new value of the <see cref="P:System.Windows.Controls.ContentControl.Content"/> property.</param>
        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);
            if (newContent is DockedElementsContainer)
            {
                DockedElementsContainer container = newContent as DockedElementsContainer;
                container.MouseDown += new MouseButtonEventHandler(container_MouseDown);
#if !SyncfusionFramework3_5
                //container.TouchDown += container_TouchDown;
#endif
            }
        }

#if !SyncfusionFramework3_5
        //void container_TouchDown(object sender, TouchEventArgs e)
        //{
        //    if (Owner != null && Owner.IsTouchEnabled && Owner.m_TouchDeviceId == e.TouchDevice.Id)
        //    {
        //        DockedElementTabbedHost ele = e.Source as DockedElementTabbedHost;
        //        if (ele != null)
        //        {
        //            m_rightSidePanel.SwitchAnimation(false);
        //            m_topSidePanel.SwitchAnimation(false);
        //            m_leftSidePanel.SwitchAnimation(false);
        //            m_bottomSidePanel.SwitchAnimation(false);
        //        }
        //    }
        //}

        //void result_TouchUp(object sender, TouchEventArgs e)
        //{
        //    if (Owner != null && Owner.IsTouchEnabled && Owner.m_TouchDeviceId == e.TouchDevice.Id)
        //    {
        //        #region result_TouchLeftFingerUp
        //        if (Owner.m_dockingManagerSystemGesture == SystemGesture.Tap)
        //        {
        //            OnSidePanelTouchLeftFingerUp(sender, e);
        //        }
        //        #endregion
        //    }
        //}

        //private void OnSidePanelTouchLeftFingerUp(Object sender, TouchEventArgs e)
        //{
        //    SetActiveWindow((SidePanel)sender);
        //}
#endif
        /// <summary>
        /// Handles the MouseDown event of the container control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        void container_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.StylusDevice == null || e.StylusDevice != null)
            {
                //throw new NotImplementedException();
                DockedElementTabbedHost ele = e.Source as DockedElementTabbedHost;
                if (ele != null)
                {

                    m_rightSidePanel.SwitchAnimation(false);
                    m_topSidePanel.SwitchAnimation(false);
                    m_leftSidePanel.SwitchAnimation(false);
                    m_bottomSidePanel.SwitchAnimation(false);
                }
            }
        }

        /// <summary>
        /// Called when [side panel mouse left button up].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnSidePanelMouseLeftButtonUp(object sender, EventArgs e)
        {
            SetActiveWindow((SidePanel)sender);
        }

        /// <summary>
        /// Called when [side panel is keyboard focus within changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private void OnSidePanelIsKeyboardFocusWithinChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            //SetActiveWindow((SidePanel)sender);
        }

        /// <summary>
        /// Sets the active window.
        /// </summary>
        /// <param name="panel">The panel.</param>
        private void SetActiveWindow(Selector panel)
        {
            if (Owner != null)
            {
                ActiveWindowChangingEventArgs args = new ActiveWindowChangingEventArgs();
                args.OldValue = Owner.ActiveWindow;
                args.NewValue = (FrameworkElement)panel.SelectedItem;
                if (args.OldValue != args.NewValue)
                {
                    Owner.FireActiveWindowChanging(args.NewValue, args);
                    if (!args.Cancel)
                    {
                        Owner.ActiveWindow = (FrameworkElement)panel.SelectedItem;
                    }
                }
            }
        }

        /// <summary>
        /// Ares the side panels initialized.
        /// </summary>
        /// <returns> return bool.</returns>
        private bool ArentSidePanelsInitialized()
        {
            return null == m_topSidePanel || null == m_leftSidePanel
                        || null == m_bottomSidePanel || null == m_rightSidePanel;
        }

        /// <summary>
        /// Initializes the members of children.
        /// </summary>
        private void InitializeMembersOfChildren()
        {
            m_mainContent = GetTemplateChild("PART_ExpandSite") as ContentPresenter;

            if (null == m_mainContent)
            {
                throw new NotSupportedException("Incorrect template");
            }

            m_topSidePanel = GetNewPanel(m_topSidePanel, TOPPANEL_NAME);
            m_leftSidePanel = GetNewPanel(m_leftSidePanel, LEFTPANEL_NAME);
            m_bottomSidePanel = GetNewPanel(m_bottomSidePanel, BOTTOMPANEL_NAME);
            m_rightSidePanel = GetNewPanel(m_rightSidePanel, RIGHTPANEL_NAME);

            if (ArentSidePanelsInitialized())
            {
                throw new NotImplementedException("Incorrect template.");
            }
        }

        /// <summary>
        /// Adds to sid panel.
        /// </summary>
        /// <param name="panel">The panel.</param>
        /// <param name="element">The element.</param>
        private static void AddToSidPanel(SidePanel panel, FrameworkElement element)
        {
            if (!panel.Items.Contains(element))
            {
                panel.AddElement(element, true);
            }
        }

        /// <summary>
        /// Removes the frame side panel.
        /// </summary>
        /// <param name="panel">The panel.</param>
        /// <param name="element">The element.</param>
        private static void RemoveFromeSidePanel(SidePanel panel, FrameworkElement element)
        {
            if (panel!=null&&element!=null&&panel.Items.Contains(element))
            {
                panel.RemoveElement(element);
            }
        }
        #endregion
    }
}