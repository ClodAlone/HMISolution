// <copyright file="Docking.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using Syncfusion.Windows.Shared;
using System.Windows.Controls.Primitives;
using System.Collections.ObjectModel;
using System.Collections;
using System.Diagnostics;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents DockingManager.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(true)]
#endif
    
    public partial class DockingManager : FrameworkElement
    {
        #region Constants
        /// <summary>
        /// Indicates minimum float window height constant value.
        /// </summary>
        private const double MinFloatWindowHeight = 25;

        /// <summary>
        /// Exception message constant.
        /// </summary>
        private const string C_STRCanStateExceptionMessage = "Cannot set property {0} to false because element is in {1} state now";

        /// <summary>
        /// represent make docking flag
        /// </summary>
        internal static bool makedockflag = true;

        /// <summary>
        /// represent update docking flag
        /// </summary>
        internal static bool updatedockflag = true;

        private Dictionary<string, string> nameMappings = new Dictionary<string, string>();

        internal static double MinWidhtInFloat;

        internal static double MinHeightInFloat;

        internal static double MaxWidhtInFloat;

        internal static double MaxHeightInFloat;

        internal static double MinWidhtInDocked;

        internal static double MinHeightInDocked;

        internal static double MaxWidhtInDocked;

        internal static double MaxHeightInDocked;

        private bool hasChild = false;

        #endregion

        /// <summary>
        /// Stores the Custom menu item collection.
        /// </summary>
        internal CustomMenuItemCollection m_custommenuitems = null;

        /// <summary>
        /// Stores the maximized elements
        /// </summary>
        internal ObservableFrameworkElements m_maximizedelements = new ObservableFrameworkElements();

        /// <summary>
        /// Stores the restorable elements
        /// </summary>
        internal ObservableFrameworkElements m_restoreelements = new ObservableFrameworkElements();

        /// <summary>
        /// Stores the restorable host elements
        /// </summary>
        internal ObservableFrameworkElements m_restorehostelements = new ObservableFrameworkElements();

        /// <summary>
        /// Stores the collection of DocumentTabControl
        /// </summary>
        internal List<DocumentTabControl> m_documentTabContrlsList = new List<DocumentTabControl>();

        internal bool m_cansetfixedsize = true;

        #region Events
        /// <summary>
        /// Occurs when [active window changed].
        /// </summary>
        public event PropertyChangedCallback ActiveWindowChanged;

        /// <summary>
        /// Occurs when [active window changed].
        /// </summary>
        public event PropertyChangedCallback IsActiveWindowChanged;

        /// <summary>
        /// Occurs when [active selected tab changed].
        /// </summary>
        public event PropertyChangedCallback IsSelectedTabChanged;

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Event that occurs when the name of the dock-target in docked
        /// mode is changed.
        /// </summary>
        public event DockTargetNameChangedEventHandler TargetNameInDockedModeChanged;

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Event that occurs when the name of the dock-target in
        /// floating mode changed.
        /// </summary>        
        public event DockTargetNameChangedEventHandler TargetNameInFloatingModeChanged;

        /// <summary>
        /// Event that occurs when the element state is changed.
        /// </summary>
        public event DockStateHandler DockStateChanged;

        /// <summary>
        /// Event that occurs when the element window state is changed.
        /// </summary>
        public event DockWindowStateHandler DockWindowStateChanged;

        /// <summary>
        /// Occurs when [dock state changing].
        /// </summary>
        public event DockStateChangingHandler DockStateChanging;

        /// <summary>
        /// Occurs when [ WindowResizing ]
        /// </summary>
        public event WindowResizingEventHandler WindowResizing;

        /// <summary>
        /// Occurs when [ WindowMoving ]
        /// </summary>
        public event WindowMovingEventHandler WindowMoving;

        /// <summary>
        /// Occurs when [ FloatWindowClosing ]
        /// </summary>
        public event WindowClosingEventHandler WindowClosing;

        /// <summary>
        /// Occurs when [transferred from manager].
        /// </summary>
        public event TransferManagerEventHandler TransferredFromManager;

        /// <summary>
        /// Occurs when [transferred to manager].
        /// </summary>
        public event TransferManagerEventHandler TransferredToManager;

        /// <summary>
        /// Occurs when [active window changing].
        /// </summary>
        public event ActiveWindowChangingHandler ActiveWindowChanging;

        /// <summary>
        /// Event that occurs when the element changing dock state to Hidden.
        /// </summary>
        public event ElementHiddenEventHandler ElementHidden;

        /// <summary>
        /// Event that occurs when the element changing dock state from Hidden.
        /// </summary>
        public event ElementShownEventHandler ElementShown;

        /// <summary>
        /// Event that occurs when the DockPreview is opened.
        /// </summary>
        public event DockProviderShownEventHandler DockProviderShown;

        /// <summary>
        /// Event that occurs when the Tab is Closed.
        /// </summary>
        public event TabClosedEventHandler TabClosed;

        /// <summary>
        /// Event that occurs when the TabGroup is Created.
        /// </summary>
        public event TabGroupEventHandler TabGroupCreated;

        /// <summary>
        /// Event that occurs when the Tab is moved to other TabGroup.
        /// </summary>
        public event TabGroupEventHandler MoveToOtherTabGroup;

        /// <summary>
        /// represent active window set internally or not
        /// </summary>
        internal static bool m_setactivewindow = false;
        /// <summary>
        /// represent state changed flag
        /// </summary>
        public bool m_onStatechange = false;

        internal bool m_ActiveWindowChangingFlag = true;

        /// <summary>
        /// Identifies DockingManager.AutoHideAnimationStart event.
        /// </summary>
        public static readonly RoutedEvent AutoHideAnimationStartEvent = EventManager.RegisterRoutedEvent(
            "AutoHideAnimationStart",
            RoutingStrategy.Bubble,
            typeof(EventHandler),
            typeof(DockingManager));

        /// <summary>
        /// Bubbling routed event fired when AutoHideAnimationStart
        /// is changed, i.e. when AutoHideAnimationStart is completed.
        /// </summary>
        public event RoutedEventHandler AutoHideAnimationStart
        {
            add
            {
                AddHandler(AutoHideAnimationStartEvent, value);
            }

            remove
            {
                RemoveHandler(AutoHideAnimationStartEvent, value);
            }
        }

        /// <summary>
        /// Identifies DockingManager.AutoHideAnimationStop event.
        /// </summary>
        public static readonly RoutedEvent AutoHideAnimationStopEvent = EventManager.RegisterRoutedEvent(
            "AutoHideAnimationStop",
            RoutingStrategy.Bubble,
            typeof(EventHandler),
            typeof(DockingManager));

        /// <summary>
        /// Bubbling routed event fired when AutoHideAnimationStop
        /// is changed, i.e. when AutoHideAnimationStop is completed.
        /// </summary>
        public event RoutedEventHandler AutoHideAnimationStop
        {
            add
            {
                AddHandler(AutoHideAnimationStopEvent, value);
            }

            remove
            {
                RemoveHandler(AutoHideAnimationStopEvent, value);
            }
        }

        /// <summary>
        /// Identifies DockingManager.WindowActivated event.
        /// </summary>
        public static readonly RoutedEvent WindowActivatedEvent = EventManager.RegisterRoutedEvent(
            "WindowActivated",
            RoutingStrategy.Bubble,
            typeof(EventHandler),
            typeof(DockingManager));

        /// <summary>
        /// Bubbling routed event fired when WindowActivated
        /// is changed, i.e. when WindowActivated is completed.
        /// </summary>
        public event RoutedEventHandler WindowActivated
        {
            add
            {
                AddHandler(WindowActivatedEvent, value);
            }

            remove
            {
                RemoveHandler(WindowActivatedEvent, value);
            }
        }

        /// <summary>
        /// Identifies DockingManager.WindowDeactivated event.
        /// </summary>
        public static readonly RoutedEvent WindowDeactivatedEvent = EventManager.RegisterRoutedEvent(
          "WindowDeactivated",
          RoutingStrategy.Bubble,
          typeof(EventHandler),
          typeof(DockingManager));

        /// <summary>
        /// Bubbling routed event fired when WindowDeactivated
        /// is changed, i.e. when WindowDeactivated is completed.
        /// </summary>
        public event RoutedEventHandler WindowDeactivated
        {
            add
            {
                AddHandler(WindowDeactivatedEvent, value);
            }

            remove
            {
                RemoveHandler(WindowDeactivatedEvent, value);
            }
        }

        /// <summary>
        /// Identifies DockingManager.ContextMenuItemClick  event.
        /// </summary>
        public static readonly RoutedEvent ContextMenuItemClickEvent = EventManager.RegisterRoutedEvent(
          "ContextMenuItemClick",
          RoutingStrategy.Bubble,
          typeof(EventHandler),
          typeof(DockingManager));

        /// <summary>
        /// Bubbling routed event fired when ContextMenuItemClick 
        /// is changed, i.e. when ContextMenuItemClick  is completed.
        /// </summary>
        public event RoutedEventHandler ContextMenuItemClick
        {
            add
            {
                AddHandler(ContextMenuItemClickEvent, value);
            }

            remove
            {
                RemoveHandler(ContextMenuItemClickEvent, value);
            }
        }

        /// <summary>
        /// Identifies DockingManager.DockVisibilityChanged event.
        /// </summary>
        public static readonly RoutedEvent WindowVisibilityChangedEvent = EventManager.RegisterRoutedEvent(
          "WindowVisibilityChanged",
          RoutingStrategy.Bubble,
          typeof(EventHandler),
          typeof(DockingManager));

        /// <summary>
        /// Bubbling routed event fired when DockVisibilityChanged 
        /// is changed, i.e. when DockVisibilityChanged is completed.
        /// </summary>
        public event RoutedEventHandler WindowVisibilityChanged
        {
            add
            {
                AddHandler(WindowVisibilityChangedEvent, value);
            }

            remove
            {
                RemoveHandler(WindowVisibilityChangedEvent, value);
            }
        }

        /// <summary>
        /// Identifies DockingManager.DockDragStart event.
        /// </summary>
        public static readonly RoutedEvent WindowDragStartEvent = EventManager.RegisterRoutedEvent(
          "WindowDragStart",
          RoutingStrategy.Bubble,
          typeof(EventHandler),
          typeof(DockingManager));

        /// <summary>
        /// Bubbling routed event fired when DockDragStart 
        /// is changed, i.e. when DockDragStart is completed.
        /// </summary>
        public event RoutedEventHandler WindowDragStart
        {
            add
            {
                AddHandler(WindowDragStartEvent, value);
            }

            remove
            {
                RemoveHandler(WindowDragStartEvent, value);
            }
        }

        /// <summary>
        /// Identifies DockingManager.DockDragEnd event.
        /// </summary>
        public static readonly RoutedEvent WindowDragEndEvent = EventManager.RegisterRoutedEvent(
          "WindowDragEnd",
          RoutingStrategy.Bubble,
          typeof(EventHandler),
          typeof(DockingManager));

        /// <summary>
        /// Bubbling routed event fired when DockDragEnd 
        /// is changed, i.e. when DockDragEnd is completed.
        /// </summary>
        public event RoutedEventHandler WindowDragEnd
        {
            add
            {
                AddHandler(WindowDragEndEvent, value);
            }

            remove
            {
                RemoveHandler(WindowDragEndEvent, value);
            }
        }

        /// <summary>
        /// Identifies DockingManager.BeforeContextMenuOpen  event.
        /// </summary>
        public static readonly RoutedEvent BeforeContextMenuOpenEvent = EventManager.RegisterRoutedEvent(
          "BeforeContextMenuOpen",
          RoutingStrategy.Bubble,
          typeof(EventHandler),
          typeof(DockingManager));

        /// <summary>
        /// Bubbling routed event fired when BeforeContextMenuOpen  
        /// is changed, i.e. when BeforeContextMenuOpen  is completed.
        /// </summary>
        public event RoutedEventHandler BeforeContextMenuOpen
        {
            add
            {
                AddHandler(BeforeContextMenuOpenEvent, value);
            }

            remove
            {
                RemoveHandler(BeforeContextMenuOpenEvent, value);
            }
        }

        /// <summary>
        /// Identifies DockingManager.DockMenuClick event.
        /// </summary>
        public static readonly RoutedEvent DockMenuClickEvent = EventManager.RegisterRoutedEvent(
          "DockMenuClick",
          RoutingStrategy.Bubble,
          typeof(EventHandler),
          typeof(DockingManager));

        /// <summary>
        /// Bubbling routed event fired when DockMenuClick 
        /// is changed, i.e. when DockMenuClick  is completed.
        /// </summary>
        public event RoutedEventHandler DockMenuClick
        {
            add
            {
                AddHandler(DockMenuClickEvent, value);
            }

            remove
            {
                RemoveHandler(DockMenuClickEvent, value);
            }
        }

        /// <summary>
        /// Occurs when [is froze changed].
        /// </summary>
        public event PropertyChangedCallback IsFrozeChanged;

        /// <summary>
        /// Bubble routed event, raised when the state of the element has changed. 
        /// </summary>
        public static readonly RoutedEvent DockStateChangedEvent = EventManager.RegisterRoutedEvent("DockStateChanged", RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(DockingManager));


        /// <summary>
        /// Bubble routed event, raised when the window state of the element has changed. 
        /// </summary>
        public static readonly RoutedEvent DockWindowStateChangedEvent = EventManager.RegisterRoutedEvent("DockWindowStateChanged", RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(DockingManager));


        #endregion

        #region Public methods

        /// <summary>
        /// Adds to target managers list.
        /// </summary>
        /// <param name="dockingManager">The docking manager.</param>
        public void AddToTargetManagersList(DockingManager dockingManager)
        {
            if (!this.targetmanagers.Contains(dockingManager))
            {
                this.targetmanagers.Add(dockingManager);
            }
        }

        /// <summary>
        /// Removes from target managers list.
        /// </summary>
        /// <param name="dockingManager">The docking manager.</param>
        public void RemoveFromTargetManagersList(DockingManager dockingManager)
        {
            if (this.targetmanagers.Contains(dockingManager))
            {
                this.targetmanagers.Remove(dockingManager);
            }
        }

        /// <summary>
        /// Sets the float window focus.
        /// </summary>
        /// <param name="window">The window.</param>
        public static void SetFloatWindowFocus(IWindow window, FrameworkElement element)
        {
            if (((window is FloatWindow) && (window as FloatWindow).IsVisible) || ((window is AdornerFloatWindow) && (window as AdornerFloatWindow).IsVisible))
            {
                DependencyObject obj = window as DependencyObject;
                BindingUtils.SetBinding(obj, element, Popup.PlacementRectangleProperty, DockingManager.FloatingWindowRectProperty, BindingMode.TwoWay);
                window.IsMultiHostsContainer = false;
                window.IsOpen = true;
                window.UpdateIsMultiHostProperty();
            }
        }
        public static void SetFloatWindowFocus(NativeFloatWindow window, FrameworkElement element)
        {
            if (window.IsVisible)
            {
                DockingManager owner = ResolveManager(element);
                DependencyObject obj = window as DependencyObject;
                BindingUtils.SetBinding(obj, element, NativeFloatWindow.InternalPlacementRectProperty, DockingManager.FloatingWindowRectProperty, BindingMode.TwoWay);
                window.IsMultiHostsContainer = false;
                if (owner != null && (!owner.m_NativeWindowsUnRegistered.Contains(window)))
                window.IsOpen = true;
                window.UpdateIsMultiHostProperty();
            }
        }
        /// <summary>
        /// Gives focus to a specified element.
        /// </summary>
        /// <param name="element">Element which will receive the focus.</param>
        public static void SetNewFocusedElement(FrameworkElement element)
        {
            DockingManager owner = DockingManager.ResolveManager(element);

            if (null != owner)
            {
                foreach (FrameworkElement child in owner.Children)
                {
                    DockingManager.SetHasFocus(child, false);
                }

                DockingManager.SetHasFocus(element, true);
                if (element == owner.ActiveWindow)
                {
                    DocumentContainer container = VisualUtils.FindDescendant(owner, typeof(DocumentContainer)) as DocumentContainer;
                    if (container != null)
                    {
                        if (container.AddTabDocumentAtLast == true)
                        {
                            TabControlExt tab = DockingManager.GetTabControl(element as DependencyObject);
                            if (tab != null && tab.TabLayoutPanel != null && tab.TabLayoutPanel.DesiredSize.Width > 0)
                                {
                                    double offset = tab.TabLayoutPanel.PrepareScrollInfo(element as DependencyObject, 0, tab.SelectedIndex, -1, tab.TabLayoutPanel.DesiredSize.Width);
                                    tab.TabLayoutPanel.StartScrolling(0, offset);
                                }
                            
                        }
                    }
                }
                if(owner.ActiveWindow!=element)
                owner.ActiveWindow = element;

                //Comments: SD10642 has been refixed and committed

                foreach (var window in owner.m_WindowsRegistered)
                {
                    if (!window.IsOpen && window.PrimaryElement != null && window.PrimaryElement.Equals(element))
                    {
                        SetFloatWindowFocus(window, element);
                    }
                }
                foreach (NativeFloatWindow window in owner.m_NativeWindowsRegistered)
                {
                    if (!window.IsOpen && window.PrimaryElement != null && window.PrimaryElement.Equals(element))
                    {
                        SetFloatWindowFocus(window, element);
                    }
                }
                element.Focus();
                
            }
        }

        /// <summary>
        /// Gets the child to focus.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>Return the child.</returns>
        private FrameworkElement GetChildToFocus(FrameworkElement element)
        {
            DockingManager owner = DockingManager.ResolveManager(element);
            FrameworkElement targetChild = null;
            int index = owner.Children.IndexOf(element);
                foreach (FrameworkElement oldactive in lastacitive)
                 {
                    if (DockingManager.GetState(oldactive) != DockState.Hidden)
                    {
                        targetChild = oldactive;
                       if (DockingManager.GetState(targetChild) == DockState.Document)
                        {
                            ActivateWindow(targetChild.Name);
                        }
                        break;
                    }
                 }
            
            if (targetChild == null)
            {
                if (index > 0 && index < owner.Children.Count - 1)
                {
                    if (DockingManager.GetState(owner.Children[index + 1]) != DockState.Hidden)
                    {
                        targetChild = owner.Children[index + 1];
                    }
                    else if (DockingManager.GetState(owner.Children[index - 1]) != DockState.Hidden)
                    {
                        targetChild = owner.Children[index - 1];
                    }
                    else
                    {
                        foreach (FrameworkElement child in owner.Children)
                        {
                            DockState state = DockingManager.GetState(child);
                            if (state != DockState.Hidden)
                            {
                                targetChild = child;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    foreach (FrameworkElement child in owner.Children)
                    {
                        DockState state = DockingManager.GetState(child);
                        if (state != DockState.Hidden)
                        {
                            targetChild = child;
                            break;
                        }
                    }
                }
            }

            if (targetChild != null)
            {
                if (DockingManager.GetSideInDockedMode(targetChild) == DockSide.Tabbed)
                {
                    if (DockingManager.GetState(targetChild) == DockState.Dock || DockingManager.GetState(targetChild) == DockState.Float)
                    {
                        DockedElementTabbedHost host = DockingManager.GetTabbedHost(targetChild, DockingManager.GetState(targetChild));
                        if (host != null && host.TabChildren != null)
                        {
                            foreach (FrameworkElement tabelement in host.TabChildren)
                            {
                                if (DockingManager.GetIsSelectedTab(tabelement))
                                {
                                    targetChild = tabelement;
                                }
                            }
                        }
                    }
                }
                if (DockingManager.GetState(targetChild) == DockState.Document)
                {
                    TabControlExt tabcontrol = DockingManager.GetTabControl(targetChild as DependencyObject);
                    if (tabcontrol != null)
                    {
                        targetChild = (tabcontrol as DocumentTabControl).SelectedItem as FrameworkElement;
                    }
                }
            }

            return targetChild;
        }

        /// <summary>
        /// Sets the focus.
        /// </summary>
        /// <param name="element">The element.</param>
        internal void SetFocus(FrameworkElement element)
        {
            if (ActiveWindow == element)
            {
                DockingManager owner = DockingManager.ResolveManager(element);
                FrameworkElement elementtosetFocus = GetChildToFocus(element);
                if (null != owner)
                {
                    foreach (FrameworkElement child in owner.Children)
                    {
                        DockingManager.SetHasFocus(child, false);
                    }

                    if (elementtosetFocus != null)
                    {
                        DockingManager.SetHasFocus(elementtosetFocus, true);
                        DockState state = DockingManager.GetState(elementtosetFocus);
                        DockedElementTabbedHost host = DockingManager.GetTabbedHost(elementtosetFocus, state);
                        if (host != null)
                        {
                            host.SelectTab(elementtosetFocus);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Return the next non hidden element
        /// </summary>
        /// <returns></returns>
        private FrameworkElement ElementToFocus()
        {
            foreach (FrameworkElement element in Children)
            {
                if (DockingManager.GetState(element) != DockState.Hidden)
                {
                    return element;
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the dockside of the element for specified state.
        /// </summary>
        /// <param name="obj">Dock Element.</param>
        /// <param name="state">Dock State.</param>
        /// <returns>Dockside of the element in specified state.</returns>
        /// <remarks>
        /// This method gets side for element in dock or float state. If state difference throws NotImplementedException.
        /// </remarks>
        /// <example>
        /// <para/>This example shows how to swap element side in C#.
        /// <code language="C#">
        /// <![CDATA[
        /// using System.Windows;
        /// using Syncfusion.Windows.Tools.Controls;
        /// namespace Sample1
        /// {
        ///     public partial class Window1 : Window
        ///     {
        ///         public Window1()
        ///         {
        ///             InitializeComponent();
        ///         }
        ///         private void SwapSide( FrameworkElement element )
        ///         {
        ///             DockState state = DockingManager.GetState( element );
        ///             DockSide side = DockingManager.GetSide( element, state );
        ///             side = DockingManager.getOppositeSide( side );
        ///             DockingManager.SetSide( element, state );
        ///         }
        ///     }
        /// }
        /// ]]>
        /// </code>
        /// </example>
        public static DockSide GetSide(DependencyObject obj, DockState state)
        {
            switch (state)
            {
                case DockState.Dock:
                    return GetSideInDockedMode(obj);

                case DockState.Float:
                    return GetSideInFloatMode(obj);

                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Sets the dockside of the element for specified state.
        /// </summary>
        /// <param name="obj">The dock element.</param>
        /// <param name="side">Dock host dock side.</param>
        /// <param name="state">The dock state.</param>
        /// <remarks>
        /// This method sets side for element in dock or float state. If state difference throws NotImplementedException.
        /// </remarks>
        /// <example>
        /// <para/>This example shows how to swap element side in C#.
        /// <code language="C#">
        /// <![CDATA[
        /// using System.Windows;
        /// using Syncfusion.Windows.Tools.Controls;
        /// namespace Sample1
        /// {
        ///     public partial class Window1 : Window
        ///     {
        ///         public Window1()
        ///         {
        ///             InitializeComponent();
        ///         }
        ///         private void SwapSide(FrameworkElement element)
        ///         {
        ///             DockState state = DockingManager.GetState(element);
        ///             DockSide side = DockingManager.GetSide(element, state);
        ///             side = DockingManager.GetOppositeSide(side);
        ///             DockingManager.SetSide(element, state);
        ///         }
        ///     }
        /// }
        /// ]]>
        /// </code>
        /// </example>
        public static void SetSide(DependencyObject obj, DockSide side, DockState state)
        {
            switch (state)
            {
                case DockState.Dock:
                    
                    SetSideInDockedMode(obj, side);
                    break;

                case DockState.Float:
                    SetSideInFloatMode(obj, side);
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gets the target name for the element in specified state.
        /// </summary>
        /// <param name="obj">Dock Element.</param>
        /// <param name="state">Dock host State.</param>
        /// <returns>String containing the name of element's target. It can be empty if target is DockingManager instance.</returns>
        /// <remarks>
        /// This method gets target name for element in dock or float state. If state difference throws NotImplementedException.
        /// </remarks>
        /// <example>
        /// <para/>This example shows how to determine whether list of elements has common target.
        /// <code language="C#">
        /// <![CDATA[
        /// using System.Windows;
        /// using Syncfusion.Windows.Tools.Controls;
        /// namespace Sample1
        /// {
        ///     public partial class Window1 : Window
        ///     {
        ///         public Window1()
        ///         {
        ///             InitializeComponent();
        ///         }
        ///         private void IsCommonTarget( <List> items, DockState state )
        ///         {
        ///             bool result = false;
        ///             if( items.Count > 2 )
        ///             {
        ///                 string targetName = DockingManager.GetTargetName( items[ 0 ], state );
        ///                 for( int i = 1, cnt = items.Count; i < cnt; i++ )
        ///                 {
        ///                     string name = DockingManager.GetTargetName( items[ i ], state );
        ///                     if( string.Equals( targetName, name )
        ///                     {
        ///                         result = true;
        ///                         break;
        ///                     }
        ///                 }
        ///             }
        ///             return result;
        ///         }
        ///     }
        /// }
        /// ]]>
        /// </code>
        /// </example>
        public static string GetTargetName(DependencyObject obj, DockState state)
        {
            switch (state)
            {
                case DockState.Dock:
                    return GetTargetNameInDockedMode(obj);

                case DockState.Float:
                    return GetTargetNameInFloatingMode(obj);

                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Sets the target name for the element in specified state.
        /// </summary>
        /// <param name="obj">Dock Element.</param>
        /// <param name="targetName">Target name.</param>
        /// <param name="state">DOck host State.</param>
        /// <remarks>
        /// This method sets target name for element in dock or float state. If state difference throws NotImplementedException.
        /// </remarks>
        /// <example>
        /// <para/>This example shows how to set target for a group of elements in C#.
        /// <code language="C#">
        /// <![CDATA[
        /// using System.Windows;
        /// using Syncfusion.Windows.Tools.Controls;
        /// namespace Sample1
        /// {
        ///     public partial class Window1 : Window
        ///     {
        ///         public Window1()
        ///         {
        ///             InitializeComponent();
        ///         }
        ///         private void SetCommonTarget( <List> items, string targetName )
        ///         {
        ///             FrameworkElement target = dockingmanager.FindChild( targetName );
        ///             DockState state = DockingManager.GetState( target );
        ///             foreach( FrameworkElement item in items )
        ///             {
        ///                 DockingManager.SetTargetName( item, targetName, state );
        ///             }
        ///         }
        ///     }
        /// }
        /// ]]>
        /// </code>
        /// </example>
        public static void SetTargetName(DependencyObject obj, string targetName, DockState state)
        {
            switch (state)
            {
                case DockState.Dock:
                    SetTargetNameInDockedMode(obj, targetName);
                    break;

                case DockState.Float:
                    SetTargetNameInFloatingMode(obj, targetName);
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gets the index.
        /// </summary>
        /// <param name="obj">The DependencyObject.</param>
        /// <param name="state">The DockState.</param>
        /// <returns>return index.</returns>
        public static int GetIndex(DependencyObject obj, DockState state)
        {
            switch (state)
            {
                case DockState.Dock:
                    return GetIndexInDockMode(obj);

                case DockState.Float:
                    return GetIndexInFloatMode(obj);

                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Sets the index externally.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="state">The state.</param>
        /// <param name="value">The value.</param>
        public static void SetIndexExternally(DependencyObject obj, DockState state, int value)
        {
            switch (state)
            {
                case DockState.Dock:
                    DockingManager.SetIndexInDockModeExternally(obj, value);
                    break;

                case DockState.Float:
                    DockingManager.SetIndexInFloatModeExternally(obj, value);
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Sets the index.
        /// </summary>
        /// <param name="obj">The DependencyObject.</param>
        /// <param name="state">The DockState.</param>
        /// <param name="value">The value.</param>
        internal static void SetIndex(DependencyObject obj, DockState state, int value)
        {
            switch (state)
            {
                case DockState.Dock:
                    SetIndexInDockMode(obj, value);
                    break;

                case DockState.Float:
                    SetIndexInFloatMode(obj, value);
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gets the width of the desired.
        /// </summary>
        /// <param name="obj">The DependencyObject.</param>
        /// <param name="state">The DockState.</param>
        /// <returns>return width.</returns>
        internal static double GetDesiredWidth(DependencyObject obj, DockState state)
        {
            switch (state)
            {
                case DockState.Dock:
                    return GetDesiredWidthInDockedMode(obj);
                case DockState.Float:
                    return GetDesiredWidthInFloatingMode(obj);
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Sets the width of the desired.
        /// </summary>
        /// <param name="obj">The DependencyObject.</param>
        /// <param name="state">The DockState.</param>
        /// <param name="value">The value.</param>
        internal static void SetDesiredWidth(DependencyObject obj, DockState state, double value)
        {
            switch (state)
            {
                case DockState.Dock:
                    SetDesiredWidthInDockedMode(obj, value);
                    break;
                case DockState.Float:
                    SetDesiredWidthInFloatingMode(obj, value);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gets the height of the desired.
        /// </summary>
        /// <param name="obj">The DependencyObject.</param>
        /// <param name="state">The DockState.</param>
        /// <returns>return height.</returns>
        internal static double GetDesiredHeight(DependencyObject obj, DockState state)
        {
            switch (state)
            {
                case DockState.Dock:
                    return GetDesiredHeightInDockedMode(obj);
                case DockState.Float:
                    return GetDesiredHeightInFloatingMode(obj);
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Sets the height of the desired.
        /// </summary>
        /// <param name="obj">The DependencyObject.</param>
        /// <param name="state">The DockState.</param>
        /// <param name="value">The value.</param>
        internal static void SetDesiredHeight(DependencyObject obj, DockState state, double value)
        {
            switch (state)
            {
                case DockState.Dock:
                    SetDesiredHeightInDockedMode(obj, value);
                    break;

                case DockState.Float:
                    SetDesiredHeightInFloatingMode(obj, value);
                    break;

                default:
                    throw new NotImplementedException();
            }
        }
        #endregion

        #region DP getters / setters

        /// <summary>
        /// Gets the value of the DockingManager.AnimationDelay�attached property from a given DependencyObject.
        /// </summary>
        /// <param name="obj">The element from which to read the property value.</param>
        /// <returns>The value of the DockingManager.AnimationDelay�attached property.</returns>
        public static Duration GetAnimationDelay(DependencyObject obj)
        {
            return (Duration)obj.GetValue(AnimationDelayProperty);
        }

        /// <summary>
        /// Sets the value of the DockingManager.AnimationDelay�attached property to a given DependencyObject.
        /// </summary>
        /// <param name="obj">The element on which to set the DockingManager.AnimationDelay�attached property.</param>
        /// <param name="value">The property value to set.</param>
        public static void SetAnimationDelay(DependencyObject obj, Duration value)
        {
            obj.SetValue(AnimationDelayProperty, value);
        }

        /// <summary>
        /// Gets the value of the DockingManager.DesiredWidthInDockedMode�attached property from a given DependencyObject.
        /// </summary>
        /// <param name="obj">The element from which to read the property value.</param>
        /// <returns>The value of the DockingManager.DesiredWidthInDockedMode�attached property.</returns>
        public static double GetDesiredWidthInDockedMode(DependencyObject obj)
        {
            return (double)obj.GetValue(DesiredWidthInDockedModeProperty);
        }

        /// <summary>
        /// Sets the value of the DockingManager.DesiredWidthInDockedMode�attached property to a given DependencyObject.
        /// </summary>
        /// <param name="obj">The element on which to set the DockingManager.DesiredWidthInDockedMode�attached property.</param>
        /// <param name="value">The property value to set.</param>
        public static void SetDesiredWidthInDockedMode(DependencyObject obj, double value)
        {
            obj.SetValue(DesiredWidthInDockedModeProperty, value);            
        }

        /// <summary>
        /// Gets the size of the float window.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public static Size GetFloatWindowSize(DependencyObject obj)
        {
            return (Size)obj.GetValue(FloatWindowSizeProperty);
        }

        /// <summary>
        /// Sets the size of the float window.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="value">The value.</param>
        public static void SetFloatWindowSize(DependencyObject obj, Size value)
        {
            obj.SetValue(FloatWindowSizeProperty, value);
        }

        /// <summary>
        /// Gets the width of the float window.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public static double GetFloatWindowWidth(DependencyObject obj)
        {
            return (double)obj.GetValue(FloatWindowWidthProperty);
        }

        /// <summary>
        /// Sets the width of the float window.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="value">The value.</param>
        public static void SetFloatWindowWidth(DependencyObject obj, double value)
        {
            obj.SetValue(FloatWindowWidthProperty, value);
        }

        /// <summary>
        /// Gets the height of the float window.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public static double GetFloatWindowHeight(DependencyObject obj)
        {
            return (double)obj.GetValue(FloatWindowHeightProperty);
        }


        //public static double GetDesiredMinHeightInFloatingMode(DependencyObject obj)
        //{
        //    return (double)obj.GetValue(DesiredHeightInFloatingModeProperty);
        //}


        //public static double SetDesiredMinHeightInFloatingMode(DependencyObject obj,double value)
        //{
        //   obj.SetValue(DesiredHeightInFloatingModeProperty,value);
        //}

        /// <summary>
        /// Sets the height of the float window.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="value">The value.</param>
        public static void SetFloatWindowHeight(DependencyObject obj, double value)
        {
            obj.SetValue(FloatWindowHeightProperty, value);
        }

        /// <summary>
        /// Gets the value of the DockingManager.DesiredHeightInDockedMode�attached property from a given DependencyObject.
        /// </summary>
        /// <param name="obj">The element from which to read the property value.</param>
        /// <returns>The value of the DockingManager.DesiredHeightInDockedMode�attached property.</returns>
        public static double GetDesiredHeightInDockedMode(DependencyObject obj)
        {
            return (double)obj.GetValue(DesiredHeightInDockedModeProperty);
        }

        /// <summary>
        /// Sets the value of the DockingManager.DesiredHeightInDockedMode�attached property to a given DependencyObject.
        /// </summary>
        /// <param name="obj">The element on which to set the DockingManager.DesiredHeightInDockedMode�attached property.</param>
        /// <param name="value">The property value to set.</param>
        public static void SetDesiredHeightInDockedMode(DependencyObject obj, double value)
        {
            obj.SetValue(DesiredHeightInDockedModeProperty, value);
        }

        /// <summary>
        /// Gets the height of the fixed.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        internal static double GetFixedHeight(DependencyObject obj)
        {
            return (double)obj.GetValue(FixedHeightProperty);
        }

        /// <summary>
        /// Sets the height of the fixed.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="value">The value.</param>
        internal static void SetFixedHeight(DependencyObject obj, double value)
        {
            obj.SetValue(FixedHeightProperty, value);
        }

        /// <summary>
        /// Gets the width of the fixed.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        internal static double GetFixedWidth(DependencyObject obj)
        {
            return (double)obj.GetValue(FixedWidthProperty);
        }

        /// <summary>
        /// Sets the width of the fixed.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="value">The value.</param>
        internal static void SetFixedWidth(DependencyObject obj, double value)
        {
            obj.SetValue(FixedWidthProperty, value);
        }

        /// <summary>
        /// Gets the desired client height in docked mode.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public static double GetDesiredClientHeightInDockedMode(DependencyObject obj)
        {
            return (double)obj.GetValue(DesiredClientHeightInDockedModeProperty);
        }

        /// <summary>
        /// Sets the desired client height in docked mode.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="value">The value.</param>
        public static void SetDesiredClientHeightInDockedMode(DependencyObject obj, double value)
        {
            obj.SetValue(DesiredClientHeightInDockedModeProperty, value);
        }

        internal static void SetHostRect(DependencyObject obj, Rect value)
        {
            obj.SetValue(HostRectProperty, value);
        }
        internal static Rect GetHostRect(DependencyObject obj)
        {
            return (Rect)obj.GetValue(HostRectProperty);
        }

        /// <summary>
        /// Sets the size of the docked elements container.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="value">The value.</param>
        public static void SetDockedElementsContainerDesiredSize(DependencyObject obj, Size value)
        {
            obj.SetValue(DockedElementsContainerDesiredSizeProperty, value);
        }


        internal static void SetElementFlag(DependencyObject obj, bool value)
        {
            obj.SetValue(ElementFlagProperty, value);
        }

        internal static bool GetElementFlag(DependencyObject obj)
        {
            return (bool)obj.GetValue(ElementFlagProperty);
        }
        /// <summary>
        /// Gets the size of the docked elements container desired.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public static Size GetDockedElementsContainerDesiredSize(DependencyObject obj)
        {
            return (Size)obj.GetValue(DockedElementsContainerDesiredSizeProperty);
        }

        /// <summary>
        /// Gets the desired client height in float mode.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public static double GetDesiredClientHeightInFloatMode(DependencyObject obj)
        {
            return (double)obj.GetValue(DesiredClientHeightInFloatModeProperty);
        }

        /// <summary>
        /// Sets the desired client height in float mode.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="value">The value.</param>
        public static void SetDesiredClientHeightInFloatMode(DependencyObject obj, double value)
        {
            obj.SetValue(DesiredClientHeightInFloatModeProperty, value);
        }

        /// <summary>
        /// Gets the value of the DockingManager.DesiredWidthInFloatingMode�attached property from a given DependencyObject.
        /// </summary>
        /// <param name="obj">The element from which to read the property value.</param>
        /// <returns>The value of the DockingManager.DesiredWidthInFloatingMode�attached property.</returns>
        public static double GetDesiredWidthInFloatingMode(DependencyObject obj)
        {
            return (double)obj.GetValue(DesiredWidthInFloatingModeProperty);
        }

        /// <summary>
        /// Sets the value of the DockingManager.DesiredWidthInFloatingMode�attached property to a given DependencyObject.
        /// </summary>
        /// <param name="obj">The element on which to set the DockingManager.DesiredWidthInFloatingMode�attached property.</param>
        /// <param name="value">The property value to set.</param>
        public static void SetDesiredWidthInFloatingMode(DependencyObject obj, double value)
        {
            obj.SetValue(DesiredWidthInFloatingModeProperty, value);
        }


        /// <summary>
        /// Gets the value of the DockingManager.DesiredMinWidthInFloatingMode�attached property from a given DependencyObject.
        /// </summary>
        /// <param name="obj">The element from which to read the property value.</param>
        /// <returns>The value of the DockingManager.DesiredMinWidthInFloatingMode�attached property.</returns>
        public static double GetDesiredMinWidthInFloatingMode(DependencyObject obj)
        {
            return (double)obj.GetValue(DesiredMinWidthInFloatingModeProperty);
        }

        /// <summary>
        /// Sets the value of the DockingManager.DesiredMinWidthInFloatingMode�attached property to a given DependencyObject.
        /// </summary>
        /// <param name="obj">The element on which to set the DockingManager.DesiredMinWidthInFloatingMode�attached property.</param>
        /// <param name="value">The property value to set.</param>
        public static void SetDesiredMinWidthInFloatingMode(DependencyObject obj, double value)
        {
            obj.SetValue(DesiredMinWidthInFloatingModeProperty, value);
        }

        /// <summary>
        /// Gets the value of the DockingManager.DesiredMaxWidthInFloatingMode�attached property from a given DependencyObject.
        /// </summary>
        /// <param name="obj">The element from which to read the property value.</param>
        /// <returns>The value of the DockingManager.DesiredMaxWidthInFloatingMode�attached property.</returns>
        public static double GetDesiredMaxWidthInFloatingMode(DependencyObject obj)
        {
            return (double)obj.GetValue(DesiredMaxWidthInFloatingModeProperty);
        }

        /// <summary>
        /// Sets the value of the DockingManager.DesiredMaxWidthInFloatingMode�attached property to a given DependencyObject.
        /// </summary>
        /// <param name="obj">The element on which to set the DockingManager.DesiredMaxWidthInFloatingMode�attached property.</param>
        /// <param name="value">The property value to set.</param>
        public static void SetDesiredMaxWidthInFloatingMode(DependencyObject obj, double value)
        {
            obj.SetValue(DesiredMaxWidthInFloatingModeProperty, value);
        }

        /// <summary>
        /// Gets the value of the DockingManager.DesiredMinWidthInDockedMode�attached property from a given DependencyObject.
        /// </summary>
        /// <param name="obj">The element from which to read the property value.</param>
        /// <returns>The value of the DockingManager.DesiredMinWidthInDockedMode�attached property.</returns>
        public static double GetDesiredMinWidthInDockedMode(DependencyObject obj)
        {
            return (double)obj.GetValue(DesiredMinWidthInDockedModeProperty);
        }

        /// <summary>
        /// Sets the value of the DockingManager.DesiredMinWidthInDockedMode�attached property to a given DependencyObject.
        /// </summary>
        /// <param name="obj">The element on which to set the DockingManager.DesiredMinWidthInDockedMode�attached property.</param>
        /// <param name="value">The property value to set.</param>
        public static void SetDesiredMinWidthInDockedMode(DependencyObject obj, double value)
        {
            obj.SetValue(DesiredMinWidthInDockedModeProperty, value);
        }

        /// <summary>
        /// Gets the value of the DockingManager.DesiredMaxWidthInDockedMode�attached property from a given DependencyObject.
        /// </summary>
        /// <param name="obj">The element from which to read the property value.</param>
        /// <returns>The value of the DockingManager.DesiredMaxWidthInDockedMode�attached property.</returns>
        public static double GetDesiredMaxWidthInDockedMode(DependencyObject obj)
        {
            return (double)obj.GetValue(DesiredMaxWidthInDockedModeProperty);
        }

        /// <summary>
        /// Sets the value of the DockingManager.DesiredMaxWidthInDockedMode�attached property to a given DependencyObject.
        /// </summary>
        /// <param name="obj">The element on which to set the DockingManager.DesiredMaxWidthInDockedMode�attached property.</param>
        /// <param name="value">The property value to set.</param>
        public static void SetDesiredMaxWidthInDockedMode(DependencyObject obj, double value)
        {
            obj.SetValue(DesiredMaxWidthInDockedModeProperty, value);
        }

        /// <summary>
        /// Sets DockingManager.SizetoContentInFloat attached property
        /// </summary>
        /// <param name="obj">element</param>
        /// <param name="value">value</param>
        public static void SetSizetoContentInFloat(DependencyObject obj, bool value)
        {
            obj.SetValue(SizetoContentInFloatProperty, value);
        }

        /// <summary>
        /// Gets DockingManager.SizetoContentInFloat attached property
        /// </summary>
        /// <param name="obj">element</param>
        public static bool GetSizetoContentInFloat(DependencyObject obj)
        {
            return (bool)obj.GetValue(SizetoContentInFloatProperty);
        }

        /// <summary>
        /// Sets the sizeto content in dock.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        public static void SetSizetoContentInDock(DependencyObject obj, bool value)
        {
            obj.SetValue(SizetoContentInDockProperty, value);
        }

        /// <summary>
        /// Gets the sizeto content in dock.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public static bool GetSizetoContentInDock(DependencyObject obj)
        {
            return (bool)obj.GetValue(SizetoContentInDockProperty);
        }

        /// <summary>
        /// Gets the value of the DockingManager.DesiredHeightInFloatingMode�attached property from a given DependencyObject.
        /// </summary>
        /// <param name="obj">The element from which to read the property value.</param>
        /// <returns>The value of the DockingManager.DesiredHeightInFloatingMode�attached property.</returns>
        public static double GetDesiredHeightInFloatingMode(DependencyObject obj)
        {
            return (double)obj.GetValue(DesiredHeightInFloatingModeProperty);
        }



        /// <summary>
        /// Sets the value of the DockingManager.DesiredHeightInFloatingMode�attached property to a given DependencyObject.
        /// </summary>
        /// <param name="obj">The element on which to set the DockingManager.DesiredHeightInFloatingMode�attached property.</param>
        /// <param name="value">The property value to set.</param>
        public static void SetDesiredHeightInFloatingMode(DependencyObject obj, double value)
        {
            obj.SetValue(DesiredHeightInFloatingModeProperty, value);
        }

        /// <summary>
        /// Gets the value of the DockingManager.DesiredMinHeightInFloatingMode�attached property from a given DependencyObject.
        /// </summary>
        /// <param name="obj">The element from which to read the property value.</param>
        /// <returns>The value of the DockingManager.DesiredMinHeightInFloatingMode�attached property.</returns>
        public static double GetDesiredMinHeightInFloatingMode(DependencyObject obj)
        {
            return (double)obj.GetValue(DesiredMinHeightInFloatingModeProperty);
        }

        /// <summary>
        /// Sets the value of the DockingManager.DesiredMinHeightInFloatingMode�attached property to a given DependencyObject.
        /// </summary>
        /// <param name="obj">The element on which to set the DockingManager.DesiredMinHeightInFloatingMode�attached property.</param>
        /// <param name="value">The property value to set.</param>
        public static void SetDesiredMinHeightInFloatingMode(DependencyObject obj, double value)
        {
            obj.SetValue(DesiredMinHeightInFloatingModeProperty, value);
        }

        /// <summary>
        /// Gets the value of the DockingManager.DesiredMaxHeightInFloatingMode�attached property from a given DependencyObject.
        /// </summary>
        /// <param name="obj">The element from which to read the property value.</param>
        /// <returns>The value of the DockingManager.DesiredMaxHeightInFloatingMode�attached property.</returns>
        public static double GetDesiredMaxHeightInFloatingMode(DependencyObject obj)
        {
            return (double)obj.GetValue(DesiredMaxHeightInFloatingModeProperty);
        }

        /// <summary>
        /// Sets the value of the DockingManager.DesiredMaxHeightInFloatingMode�attached property to a given DependencyObject.
        /// </summary>
        /// <param name="obj">The element on which to set the DockingManager.DesiredMaxHeightInFloatingMode�attached property.</param>
        /// <param name="value">The property value to set.</param>
        public static void SetDesiredMaxHeightInFloatingMode(DependencyObject obj, double value)
        {
            obj.SetValue(DesiredMaxHeightInFloatingModeProperty, value);
        }

        /// <summary>
        /// Gets the value of the DockingManager.DesiredMinHeightInDockedMode�attached property from a given DependencyObject.
        /// </summary>
        /// <param name="obj">The element from which to read the property value.</param>
        /// <returns>The value of the DockingManager.DesiredMinHeightInDockedMode�attached property.</returns>
        public static double GetDesiredMinHeightInDockedMode(DependencyObject obj)
        {
            return (double)obj.GetValue(DesiredMinHeightInDockedModeProperty);
        }

        /// <summary>
        /// Sets the value of the DockingManager.DesiredMinHeightInDockedMode�attached property to a given DependencyObject.
        /// </summary>
        /// <param name="obj">The element on which to set the DockingManager.DesiredMinHeightInDockedMode�attached property.</param>
        /// <param name="value">The property value to set.</param>
        public static void SetDesiredMinHeightInDockedMode(DependencyObject obj, double value)
        {
            obj.SetValue(DesiredMinHeightInDockedModeProperty, value);
        }

        /// <summary>
        /// Gets the value of the DockingManager.DesiredMaxHeightInDockedMode�attached property from a given DependencyObject.
        /// </summary>
        /// <param name="obj">The element from which to read the property value.</param>
        /// <returns>The value of the DockingManager.DesiredMaxHeightInDockedMode�attached property.</returns>
        public static double GetDesiredMaxHeightInDockedMode(DependencyObject obj)
        {
            return (double)obj.GetValue(DesiredMaxHeightInDockedModeProperty);
        }

        /// <summary>
        /// Sets the value of the DockingManager.DesiredMaxHeightInDockedMode�attached property to a given DependencyObject.
        /// </summary>
        /// <param name="obj">The element on which to set the DockingManager.DesiredMaxHeightInDockedMode�attached property.</param>
        /// <param name="value">The property value to set.</param>
        public static void SetDesiredMaxHeightInDockedMode(DependencyObject obj, double value)
        {
            obj.SetValue(DesiredMaxHeightInDockedModeProperty, value);
        }

        /// <summary>
        /// Gets the value of the DockingManager.TargetNameInFloatingMode�attached property from a given DependencyObject.
        /// </summary>
        /// <param name="obj">The element from which to read the property value.</param>
        /// <returns>The value of the DockingManager.TargetNameInFloatingMode�attached property.</returns>
        public static string GetTargetNameInFloatingMode(DependencyObject obj)
        {
            return (string)obj.GetValue(TargetNameInFloatingModeProperty);
        }

        /// <summary>
        /// Sets the value of the DockingManager.TargetNameInFloatingMode�attached property to a given DependencyObject.
        /// </summary>
        /// <param name="obj">The element on which to set the DockingManager.TargetNameInFloatingMode�attached property.</param>
        /// <param name="value">The property value to set.</param>
        public static void SetTargetNameInFloatingMode(DependencyObject obj, string value)
        {
            if (DockingManager.GetState(obj) != DockState.Float)
                obj.SetValue(TargetNameInFloatingModeProperty, string.Empty);
            else
                obj.SetValue(TargetNameInFloatingModeProperty, value);
        }

        /// <summary>
        /// Gets the value of the DockingManager.TargetNameInDockedMode�attached property from a given DependencyObject.
        /// </summary>
        /// <param name="obj">The element from which to read the property value.</param>
        /// <returns>The value of the DockingManager.TargetNameInDockedMode�attached property.</returns>
        public static string GetTargetNameInDockedMode(DependencyObject obj)
        {
            return (string)obj.GetValue(TargetNameInDockedModeProperty);
        }

        /// <summary>
        /// Gets the value of the DockingManager.AnimateOnNewItemAdded�attached property from a given DependencyObject.
        /// </summary>
        /// <param name="obj">The element from which to read the property value.</param>
        /// <returns>The value of the DockingManager.AnimateOnNewItemAdded�attached property.</returns>
       
        public static bool GetAnimateOnNewItemAdded(DependencyObject obj)
        {
            return (bool)obj.GetValue(AnimateOnNewItemAddedProperty);
        }

        /// <summary>
        /// Sets the value of the DockingManager.AnimateOnNewItemAdded�attached property to a given DependencyObject.
        /// </summary>
        /// <param name="obj">The element on which to set the DockingManager.AnimateOnNewItemAdded�attached property.</param>
        /// <param name="value">The property value to set.</param>        

        public static void SetAnimateOnNewItemAdded(DependencyObject obj, bool value)
        {
            obj.SetValue(AnimateOnNewItemAddedProperty, value);
        }

        public static Size GetPreviousContainerDesiredSize(DependencyObject obj)
        {
            return (Size)obj.GetValue(PreviousContainerDesiredSizeProperty);
        }

        public static void SetPreviousContainerDesiredSize(DependencyObject obj, Size value)
        {
            obj.SetValue(PreviousContainerDesiredSizeProperty, value);
        }

        public static double GetPreviousDesiredWidthInDockedMode(DependencyObject obj)
        {
            return (double)obj.GetValue(PreviousDesiredWidthInDockedModeProperty);
        }

        public static void SetPreviousDesiredWidthInDockedMode(DependencyObject obj, double value)
        {
            obj.SetValue(PreviousDesiredWidthInDockedModeProperty, value);
        }


        public static double GetPreviousDesiredHeightInDockedMode(DependencyObject obj)
        {
            return (double)obj.GetValue(PreviousDesiredHeightInDockedModeProperty);
        }

        public static void SetPreviousDesiredHeightInDockedMode(DependencyObject obj, double value)
        {
            obj.SetValue(PreviousDesiredHeightInDockedModeProperty, value);
        }


        /// <summary>
        /// Checks the child.
        /// </summary>
        /// <param name="nametocheck">The nametocheck.</param>
        /// <returns></returns>
        private bool CheckChild(string nametocheck)
        {
            FrameworkElement element = null;
            for (int i = 0; i < Children.Count; i++)
            {
                element = Children[i];
                if (nametocheck == element.Name)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Checks the container children.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="childcontainer">The childcontainer.</param>
        /// <returns></returns>
        private bool CheckContainerChildren(DockedElementsContainer container,DockedElementsContainer childcontainer,DockedElementsContainer maincontainer)
        {
            bool exist = false;
            foreach (FrameworkElement element in container.Children)
            {
                if (element is DockedElementsContainer && !(element as DockedElementsContainer).Equals(childcontainer) && !(element as DockedElementsContainer).Equals(maincontainer))
                    exist = CheckContainerChildren(element as DockedElementsContainer, childcontainer, maincontainer);
                if (element.Equals(childcontainer))
                {
                    exist = true;
                    return exist;
                }
            }
            return exist;
        }

        /// <summary>
        /// Gets the container level.
        /// </summary>
        /// <param name="targetcontainer">The targetcontainer.</param>
        /// <param name="childcontainer">The childcontainer.</param>
        /// <returns></returns>
        private int GetContainerLevel(DockedElementsContainer targetcontainer, DockedElementsContainer childcontainer)
        {
            int containerlevel = 0;
            DockedElementsContainer container = targetcontainer;
            while (container != null)
            {
                if (childcontainer != null && childcontainer.Equals(container))
                    return containerlevel;

                container = VisualUtils.FindAncestor((Visual)container, typeof(DockedElementsContainer)) as DockedElementsContainer;
                if (container != null)
                {
                    containerlevel++;
                    bool containerexist = CheckContainerChildren(container, childcontainer, targetcontainer);
                    if (containerexist)
                    {
                        childcontainer.rootParentContainer = container;
                        return containerlevel;
                    }
                }
            }
            container = targetcontainer;
            containerlevel = 0;
            while (container != null)
            {
                if (childcontainer != null && childcontainer.Equals(container))
                    return containerlevel;

                container = VisualUtils.FindDescendant((Visual)container, typeof(DockedElementsContainer)) as DockedElementsContainer;

                if (container != null)
                {
                    containerlevel--;
                    bool containerexist = CheckContainerChildren(container, childcontainer, targetcontainer);
                    if (containerexist)
                    {
                        childcontainer.rootParentContainer = container;
                        return containerlevel;
                    }
                }
            }
            return containerlevel;
        }

        /// <summary>
        /// Traverses the container.
        /// </summary>
        /// <param name="finaldockElements">The finaldock elements.</param>
        /// <param name="innerDockElements">The inner dock elements.</param>
        /// <param name="dockhostcollection">The dockhostcollection.</param>
        /// <param name="rootcontainer">The rootcontainer.</param>
        /// <param name="targetcontainer">The targetcontainer.</param>
        /// <returns></returns>
        private List<FrameworkElement> TraverseContainer(List<FrameworkElement> finaldockElements, List<FrameworkElement> innerDockElements, List<DockedElementTabbedHost> dockhostcollection, DockedElementsContainer rootcontainer, DockedElementsContainer targetcontainer)
        {
            List<DockedElementsContainer> containers = new List<DockedElementsContainer>();
            List<FrameworkElement> topcontainerelements = new List<FrameworkElement>();
            List<FrameworkElement> bottomcontainerelements = new List<FrameworkElement>();
            foreach (FrameworkElement element in rootcontainer.Children)
            {
                if (element is DockedElementTabbedHost && dockhostcollection.Contains(element as DockedElementTabbedHost))
                {
                    int index = dockhostcollection.IndexOf(element as DockedElementTabbedHost);
                    if (index != -1 && innerDockElements.Count > index && !finaldockElements.Contains(innerDockElements[index]))
                    {
                        DockSide side = DockingManager.GetSideInDockedMode(innerDockElements[index]);
                        if (side == DockSide.Bottom || side == DockSide.Right)
                            bottomcontainerelements.Add(innerDockElements[index]);
                        else
                            topcontainerelements.Add(innerDockElements[index]);
                        finaldockElements.Add(innerDockElements[index]);
                    }
                }
                if (element is DockedElementsContainer)
                    containers.Add(element as DockedElementsContainer);
            }
            if (topcontainerelements.Count > 0 || bottomcontainerelements.Count > 0)
            {
                foreach (FrameworkElement element in topcontainerelements)
                {
                    if (finaldockElements.Contains(element))
                        finaldockElements.Remove(element);
                }
                foreach (FrameworkElement element in bottomcontainerelements)
                {
                    if (finaldockElements.Contains(element))
                        finaldockElements.Remove(element);
                }
                for (int i = 0; i <= topcontainerelements.Count - 1; i++)               
                    finaldockElements.Add(topcontainerelements[i]);       

                for (int i = bottomcontainerelements.Count - 1; i >= 0; i--)
                    finaldockElements.Add(bottomcontainerelements[i]);
            }
            topcontainerelements.Clear();
            bottomcontainerelements.Clear();
            foreach (FrameworkElement element in containers)
            {
                finaldockElements = TraverseContainer(finaldockElements, innerDockElements, dockhostcollection, element as DockedElementsContainer, targetcontainer);
            }
            return finaldockElements;
        }

        /// <summary>
        /// Checks the order.
        /// </summary>
        /// <param name="childElement">The child element.</param>
        /// <param name="targetElement">The target element.</param>
        private List<string> CheckOrder(FrameworkElement childElement, FrameworkElement targetElement, List<string> innerDockElements)
        {
            List<string> finaldockelements = new List<string>();
            DockedElementTabbedHost childdockhost = DockingManager.GetDockHost(childElement);
            DockedElementTabbedHost targetdockhost = DockingManager.GetDockHost(targetElement);
            if (childdockhost != null && targetdockhost != null)
            {
                DockedElementsContainer childcontainer = VisualUtils.FindAncestor((Visual)childdockhost, typeof(DockedElementsContainer)) as DockedElementsContainer;
                DockedElementsContainer targetcontainer = VisualUtils.FindAncestor((Visual)targetdockhost, typeof(DockedElementsContainer)) as DockedElementsContainer;
                int topchildelementlevel = 0;
                DockedElementsContainer rootcontainer = null;
                List<DockedElementTabbedHost> dockhostcollection = new List<DockedElementTabbedHost>();
                List<int> containerlevels = new List<int>();
                Hashtable dockelementcollection = new Hashtable();

                if (innerDockElements.Contains(targetElement.Name))
                    innerDockElements.Remove(targetElement.Name);

                if (childcontainer != null && targetcontainer != null && innerDockElements.Count > 1)
                {
                    foreach (string childname in innerDockElements)
                    {
                        FrameworkElement element = FindChild(childname) as FrameworkElement;
                        if (element != null)
                        {
                            DockedElementTabbedHost dockhost = DockingManager.GetDockHost(element);
                            DockedElementsContainer childcontainer1 = dockhost != null ? VisualUtils.FindAncestor((Visual)dockhost, typeof(DockedElementsContainer)) as DockedElementsContainer : null;
                            int containerlevel = GetContainerLevel(targetcontainer, childcontainer1);

                            List<FrameworkElement> childDockElements = dockelementcollection[containerlevel] as List<FrameworkElement>;
                            if (dockelementcollection[containerlevel] == null)
                                childDockElements = new List<FrameworkElement>();

                            if (!containerlevels.Contains(containerlevel))
                                containerlevels.Add(containerlevel);
                            childDockElements.Add(element);

                            if (dockelementcollection.ContainsKey(containerlevel))
                                dockelementcollection[containerlevel] = childDockElements;
                            else
                                dockelementcollection.Add(containerlevel, childDockElements);

                            if (containerlevel > topchildelementlevel)
                            {
                                topchildelementlevel = containerlevel;
                                rootcontainer = childcontainer1.rootParentContainer != null ? childcontainer1.rootParentContainer : childcontainer1;
                                childcontainer1.rootParentContainer = null;
                            }
                        }
                    }

                    containerlevels.Sort();

                    for (int i = containerlevels.Count - 1; i >= 0; i--)
                    {
                        List<FrameworkElement> elements = dockelementcollection[containerlevels[i]] as List<FrameworkElement>;

                        if (elements.Count > 1)
                        {
                            List<FrameworkElement> finalelements = new List<FrameworkElement>();

                            foreach (FrameworkElement child in elements)
                            {
                                DockedElementTabbedHost dockhost = DockingManager.GetDockHost(child);
                                if (dockhost != null)
                                    dockhostcollection.Add(dockhost);
                            }

                            if (rootcontainer != null)
                                finalelements = TraverseContainer(finalelements, elements, dockhostcollection, rootcontainer, targetcontainer);
                            else
                                finalelements = TraverseContainer(finalelements, elements, dockhostcollection, targetcontainer, null);

                            foreach (FrameworkElement child in finalelements)
                            {
                                finaldockelements.Add(child.Name);
                            }
                        }
                        else
                            finaldockelements.Add(elements[0].Name);

                        dockhostcollection.Clear();
                    }
                }
                else
                    finaldockelements = innerDockElements;
            }
            return finaldockelements;
        }

        /// <summary>
        /// Checks the index.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="value">The value.</param>
        private void CheckInnerDockIndex(DependencyObject childElement, string targetName,bool swapping, int indexposition)
        {
            ///This method has been added for MT2257, 91433, Mt2246, MT2269, Mt2270 92396 - Component Swapping issue 

            DockingManager owner = DockingManager.ResolveManager(childElement as UIElement);
            if (owner != null && targetName != string.Empty && !owner.m_loadingState)
            {
                FrameworkElement targetelement = owner.FindChild(targetName);

                if (!swapping && InnerDockElements[(childElement as FrameworkElement).Name] != null)
                    (InnerDockElements[(childElement as FrameworkElement).Name] as List<string>).Clear();

                List<string> innerDockElements = InnerDockElements[targetName] as List<string>;
                if (InnerDockElements[targetName] == null)
                    innerDockElements = new List<string>();

                DockState targetState = DockingManager.GetState(targetelement as DependencyObject);
                if (IsVisibleState(targetState))
                {
                    DockSide targetSide = DockingManager.GetSide(targetelement as DependencyObject, targetState);
                    if (targetState == DockState.Dock && targetSide != DockSide.Tabbed && targetelement != childElement)
                    {
                        if (innerDockElements != null)
                        {
                            if (innerDockElements.Contains((childElement as FrameworkElement).Name))
                                innerDockElements.Remove((childElement as FrameworkElement).Name);
                            if (indexposition != -1 && indexposition <= innerDockElements.Count - 1)
                            {
                                innerDockElements.Insert(indexposition, (childElement as FrameworkElement).Name);
                                swapping = false;
                            }
                            else
                                innerDockElements.Add((childElement as FrameworkElement).Name);
                        }

                        if (swapping)
                            innerDockElements = CheckOrder(childElement as FrameworkElement, targetelement, innerDockElements);

                        List<int> indexes = new List<int>();
                        foreach (string childname in innerDockElements)
                        {
                            FrameworkElement child = FindChild(childname) as FrameworkElement;
                            if (child != null)
                                indexes.Add(DockingManager.GetIndexInDockMode(child));
                        }

                        indexes.Sort();

                        for (int i = indexes.Count - 1; i >= 0; i--)
                        {
                            DependencyObject obj = FindChild(innerDockElements[i]) as DependencyObject;
                            if (obj != null)
                            {
                                int childindex = DockingManager.GetIndexInDockMode(obj);
                                if (childindex != indexes[i])
                                    DockingManager.SetIndexInDockMode(obj, indexes[i]);
                            }
                        }
                    }

                    if (InnerDockElements.ContainsKey(targetName))
                        InnerDockElements[targetName] = innerDockElements;
                    else
                        InnerDockElements.Add(targetName, innerDockElements);
                }
            }
        }

        /// <summary>
        /// Sets the value of the DockingManager.TargetNameInDockedMode�attached property to a given DependencyObject.
        /// </summary>
        /// <param name="obj">The element on which to set the DockingManager.TargetNameInDockedMode�attached property.</param>
        /// <param name="value">The property value to set.</param>
        public static void SetTargetNameInDockedMode(DependencyObject obj, string value)
        {
            if (value != "")
                updatedockflag = false;
            obj.SetValue(TargetNameInDockedModeProperty, value);
        }


        /// <summary>
        /// Sets the target name in auto hide mode.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="value">The value.</param>
        internal static void SetTargetNameInAutoHideMode(DependencyObject obj, string value)
        {
            obj.SetValue(TargetNameInAutoHideModeProperty, value);
        }

        /// <summary>
        /// Gets the target name in auto hide mode.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        internal static string GetTargetNameInAutoHideMode(DependencyObject obj)
        {
            return (string)obj.GetValue(TargetNameInAutoHideModeProperty);
        }

        /// <summary>
        /// Gets the value of the DockingManager.NoHeader�attached property from a given DependencyObject.
        /// </summary>
        /// <param name="obj">The element from which to read the property value.</param>
        /// <returns>The value of the DockingManager.NoHeader�attached property.</returns>
        public static bool GetNoHeader(DependencyObject obj)
        {
            return (bool)obj.GetValue(NoHeaderProperty);
        }

        /// <summary>
        /// Sets the value of the DockingManager.NoHeader�attached property to a given DependencyObject.
        /// </summary>
        /// <param name="obj">The element on which to set the DockingManager.NoHeader�attached property.</param>
        /// <param name="value">The property value to set.</param>
        public static void SetNoHeader(DependencyObject obj, bool value)
        {
            obj.SetValue(NoHeaderProperty, value);
        }

        /// <summary>
        /// Gets the value of the DockingManager.NoHeader�attached property from a given DependencyObject.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        internal static bool GetPreviousNoHeader(DependencyObject obj)
        {
            return (bool)obj.GetValue(PreviousNoHeaderProperty);
        }

        /// <summary>
        /// Sets the value of the DockingManager.NoHeader�attached property to a given DependencyObject.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        internal static void SetPreviousNoHeader(DependencyObject obj, bool value)
        {
            obj.SetValue(PreviousNoHeaderProperty, value);
        }
        
        /// <summary>
        /// Gets the value of the DockingManager.SideInFloatMode�attached property from a given DependencyObject.
        /// </summary>
        /// <param name="obj">The element from which to read the property value.</param>
        /// <returns>The value of the DockingManager.SideInFloatMode�attached property.</returns>
        public static DockSide GetSideInFloatMode(DependencyObject obj)
        {
            return (DockSide)obj.GetValue(SideInFloatModeProperty);
        }

        /// <summary>
        /// Sets the value of the DockingManager.SideInFloatMode�attached property to a given DependencyObject.
        /// </summary>
        /// <param name="obj">The element on which to set the DockingManager.SideInFloatMode�attached property.</param>
        /// <param name="value">The property value to set.</param>
        public static void SetSideInFloatMode(DependencyObject obj, DockSide value)
        {
            obj.SetValue(SideInFloatModeProperty, value);
        }

        /// <summary>
        /// Gets the value of the DockingManager.SideInDockedMode�attached property from a given DependencyObject.
        /// </summary>
        /// <param name="obj">The element from which to read the property value.</param>
        /// <returns>The value of the DockingManager.SideInDockedMode�attached property.</returns>
        public static DockSide GetSideInDockedMode(DependencyObject obj)
        {
            return (DockSide)obj.GetValue(SideInDockedModeProperty);
        }

        /// <summary>
        /// Sets the value of the DockingManager.SideInDockedMode�attached property to a given DependencyObject.
        /// </summary>
        /// <param name="obj">The element on which to set the DockingManager.SideInDockedMode�attached property.</param>
        /// <param name="value">The property value to set.</param>
        /// <remarks>
        /// See <see cref="DockSide"/> for all possible DockSide cases.
        /// </remarks>
        public static void SetSideInDockedMode(DependencyObject obj, DockSide value)
        {
            //makedockflag = false;
            obj.SetValue(SideInDockedModeProperty, value);
        }
        
        /// <summary>
        /// Gets the value of the DockingManager.Header�attached property from a given DependencyObject.
        /// </summary>
        /// <param name="obj">The element from which to read the property value.</param>
        /// <returns>The value of the DockingManager.Header�attached property.</returns>
        public static object GetHeader(DependencyObject obj)
        {
            return obj.GetValue(HeaderProperty);
        }

        /// <summary>
        /// Sets the value of the DockingManager.Header�attached property to a given DependencyObject.
        /// </summary>
        /// <param name="obj">The element on which to set the DockingManager.Header�attached property.</param>
        /// <param name="value">The property value to set.</param>
        public static void SetHeader(DependencyObject obj, object value)
        {
            obj.SetValue(HeaderProperty, value);
        }

        /// <summary>
        /// Gets the value of the DockingManager.DockHeaderPresenter�attached property from a given DependencyObject.
        /// </summary>
        /// <param name="obj">The element from which to read the property value.</param>
        /// <returns>The value of the DockingManager.DockHeaderPresenter�attached property.</returns>
        public static DockHeaderPresenter GetDockHeaderPresenter(DependencyObject obj)
        {
            return obj.GetValue(DockHeaderPresenterProperty) as DockHeaderPresenter;
        }

        /// <summary>
        /// Sets the value of the DockingManager.DockHeaderPresenter�attached property to a given DependencyObject.
        /// </summary>
        /// <param name="obj">The element on which to set the DockingManager.DockHeaderPresenter�attached property.</param>
        /// <param name="value">The property value to set.</param>
        public static void SetDockHeaderPresenter(DependencyObject obj, object value)
        {
            obj.SetValue(DockHeaderPresenterProperty, value);
        }
        /// <summary>
        /// Gets the icon.
        /// </summary>
        /// <param name="obj">The dependency obj.</param>
        /// <returns>return Icon value.</returns>
        public static Brush GetIcon(DependencyObject obj)
        {
            return (Brush)obj.GetValue(IconProperty);
        }

        /// <summary>
        /// Sets the icon.
        /// </summary>
        /// <param name="obj">The dependency object.</param>
        /// <param name="value">The value.</param>
        public static void SetIcon(DependencyObject obj, Brush value)
        {
            obj.SetValue(IconProperty, value);
        }

        /// <summary>
        /// Gets the value of the DockingManager.CanClose�attached property from a given DependencyObject.
        /// </summary>
        /// <param name="obj">The element from which to read the property value.</param>
        /// <returns>The value of the DockingManager.CanClose�attached property.</returns>
        public static bool GetCanClose(DependencyObject obj)
        {
            return (bool)obj.GetValue(CanCloseProperty);
        }

        /// <summary>
        /// Gets the allows transparency for float window.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public static bool GetAllowsTransparencyForFloatWindow(DependencyObject obj)
        {
            return (bool)obj.GetValue(AllowsTransparencyForFloatWindowProperty);
        }

        /// <summary>
        /// Gets the maximize button visibility.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        internal static Visibility GetMaximizeButtonVisibility(DependencyObject obj)
        {
            return (Visibility)obj.GetValue(MaximizeButtonVisibilityProperty);
        }

        /// <summary>
        /// Gets the restore button visibility.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        internal static Visibility GetRestoreButtonVisibility(DependencyObject obj)
        {
            return (Visibility)obj.GetValue(RestoreButtonVisibilityProperty);
        }

        /// <summary>
        /// Sets the restore button visibility.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="value">The value.</param>
        internal static void SetRestoreButtonVisibility(DependencyObject obj, Visibility value)
        {
            obj.SetValue(RestoreButtonVisibilityProperty, value);
        }

        /// <summary>
        /// Sets the maximize button visibility.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="value">The value.</param>
        internal static void SetMaximizeButtonVisibility(DependencyObject obj, Visibility value)
        {
            obj.SetValue(MaximizeButtonVisibilityProperty, value);
        }

        /// <summary>
        /// Gets the Minimize button visibility.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        internal static Visibility GetMinimizeButtonVisibility(DependencyObject obj)
        {
            return (Visibility)obj.GetValue(MinimizeButtonVisibilityProperty);
        }

        /// <summary>
        /// Sets the Minimize button visibility.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="value">The value.</param>
        internal static void SetMinimizeButtonVisibility(DependencyObject obj, Visibility value)
        {
            obj.SetValue(MinimizeButtonVisibilityProperty, value);
        }

        /// <summary>
        /// Sets the value of the DockingManager.CanClose�attached property to a given DependencyObject.
        /// </summary>
        /// <param name="obj">The element on which to set the DockingManager.CanClose�attached property.</param>
        /// <param name="value">The property value to set.</param>
        public static void SetCanClose(DependencyObject obj, bool value)
        {
            obj.SetValue(CanCloseProperty, value);
        }

        /// <summary>
        /// Gets the height of the is fixed element.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public static bool GetIsFixedHeight(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsFixedHeightProperty);
        }

        /// <summary>
        /// Gets the width of the is fixed element.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public static bool GetIsFixedWidth(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsFixedWidthProperty);
        }

        /// <summary>
        /// Gets the size of the is fixed element.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public static bool GetIsFixedSize(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsFixedSizeProperty);
        }

        /// <summary>
        /// Sets the size of the is fixed element.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        public static void SetIsFixedSize(DependencyObject obj, bool value)
        {
            obj.SetValue(IsFixedSizeProperty, value);
        }

        /// <summary>
        /// Sets the height of the is fixed element.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        public static void SetIsFixedHeight(DependencyObject obj, bool value)
        {
            obj.SetValue(IsFixedHeightProperty, value);
        }

        /// <summary>
        /// Sets the width of the is fixed element .
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        public static void SetIsFixedWidth(DependencyObject obj, bool value)
        {
            obj.SetValue(IsFixedWidthProperty, value);
        }

        /// <summary>
        /// Sets the allows transparency for float window.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        public static void SetAllowsTransparencyForFloatWindow(DependencyObject obj,bool value)
        {
            obj.SetValue(AllowsTransparencyForFloatWindowProperty,value);
        }

        /// <summary>
        /// Gets the value of the DockingManager.CanDrag�attached property from a given DependencyObject.
        /// </summary>
        /// <param name="obj">The element from which to read the property value.</param>
        /// <returns>The value of the DockingManager.CanDrag�attached property.</returns>
        public static bool GetCanDrag(DependencyObject obj)
        {
            return (bool)obj.GetValue(CanDragProperty);
        }

        /// <summary>
        /// Sets the value of the DockingManager.CanDrag�attached property to a given DependencyObject.
        /// </summary>
        /// <param name="obj">The element on which to set the DockingManager.CanDrag�attached property.</param>
        /// <param name="value">The property value to set.</param>
        public static void SetCanDrag(DependencyObject obj, bool value)
        {
            obj.SetValue(CanDragProperty, value);
        }

        /// <summary>
        /// Gets the can maximize.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public static bool GetCanMaximize(DependencyObject obj)
        {
            return (bool)obj.GetValue(CanMaximizeProperty);
        }

        /// <summary>
        /// Sets the can maximize.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        public static void SetCanMaximize(DependencyObject obj, bool value)
        {
            obj.SetValue(CanMaximizeProperty, value);
        }

        /// <summary>
        /// Gets the can minimize.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public static bool GetCanMinimize(DependencyObject obj)
        {
            return (bool)obj.GetValue(CanMinimizeProperty);
        }

        /// <summary>
        /// Sets the can minimize.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        public static void SetCanMinimize(DependencyObject obj, bool value)
        {
            obj.SetValue(CanMinimizeProperty, value);
        }

        /// <summary>
        /// Gets the value of the DockingManager.CanDock�attached property from a given DependencyObject.
        /// </summary>
        /// <param name="obj">The element from which to read the property value.</param>
        /// <returns>The value of the DockingManager.CanDock�attached property.</returns>
        public static bool GetCanDock(DependencyObject obj)
        {
            return (bool)obj.GetValue(CanDockProperty);
        }

        /// <summary>
        /// Sets the value of the DockingManager.CanDock�attached property to a given DependencyObject.
        /// </summary>
        /// <param name="obj">The element on which to set the DockingManager.CanDock�attached property.</param>
        /// <param name="value">The property value to set.</param>
        public static void SetCanDock(DependencyObject obj, bool value)
        {
            obj.SetValue(CanDockProperty, value);
        }

        /// <summary>
        /// Gets the value of the DockingManager.CanFloat�attached property from a given DependencyObject.
        /// </summary>
        /// <param name="obj">The element from which to read the property value.</param>
        /// <returns>The value of the DockingManager.CanFloat�attached property.</returns>
        public static bool GetCanFloat(DependencyObject obj)
        {
            return (bool)obj.GetValue(CanFloatProperty);
        }

        /// <summary>
        /// Sets the value of the DockingManager.CanFloat�attached property to a given DependencyObject.
        /// </summary>
        /// <param name="obj">The element on which to set the DockingManager.CanFloat�attached property.</param>
        /// <param name="value">The property value to set.</param>
        public static void SetCanFloat(DependencyObject obj, bool value)
        {
            obj.SetValue(CanFloatProperty, value);
        }

        /// <summary>
        /// Gets the value of the DockingManager.CanDocument�attached property from a given DependencyObject.
        /// </summary>
        /// <param name="obj">The element from which to read the property value.</param>
        /// <returns>The value of the DockingManager.CanDocument�attached property.</returns>
        public static bool GetCanDocument(DependencyObject obj)
        {
            return (bool)obj.GetValue(CanDocumentProperty);
        }

        /// <summary>
        /// Sets the value of the DockingManager.CanDocument�attached property to a given DependencyObject.
        /// </summary>
        /// <param name="obj">The element on which to set the DockingManager.CanDocument�attached property.</param>
        /// <param name="value">The property value to set.</param>
        public static void SetCanDocument(DependencyObject obj, bool value)
        {
            obj.SetValue(CanDocumentProperty, value);
        }

        /// <summary>
        /// Gets the value of the DockingManager.State�attached property from a given DependencyObject.
        /// </summary>
        /// <param name="obj">The element from which to read the property value.</param>
        /// <returns>The value of the DockingManager.State�attached property.</returns>
        public static DockState GetState(DependencyObject obj)
        {
            return (DockState)obj.GetValue(StateProperty);
        }

        /// <summary>
        /// Gets the state of the dock window.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public static WindowState GetDockWindowState(DependencyObject obj)
        {
            return (WindowState)obj.GetValue(DockWindowStateProperty);
        }

        /// <summary>
        /// Sets the value of the DockingManager.State�attached property to a given DependencyObject.
        /// </summary>
        /// <param name="obj">The element on which to set the DockingManager.State�attached property.</param>
        /// <param name="value">The property value to set.</param>
        /// <remarks>
        /// See <see cref="DockState"/> for all possible DockState cases.
        /// </remarks>
        public static void SetState(DependencyObject obj, DockState value)
        {
            obj.SetValue(StateProperty, value);
        }

        /// <summary>
        /// Sets the state of the dock window.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="value">The value.</param>
        public static void SetDockWindowState(DependencyObject obj, WindowState value)
        {
            obj.SetValue(DockWindowStateProperty, value);
        }

        /// <summary>
        /// Sets the state of the can resize in float.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        public static void SetCanResizeInFloatState(DependencyObject obj, bool value)
        {
            obj.SetValue(CanResizeInFloatStateProperty, value);
        }

        /// <summary>
        /// Sets the state of the can resize height in float.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        public static void SetCanResizeHeightInFloatState(DependencyObject obj, bool value)
        {
            obj.SetValue(CanResizeHeightInFloatStateProperty, value);
        }

        /// <summary>
        /// Sets the state of the can resize width in float.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        public static void SetCanResizeWidthInFloatState(DependencyObject obj, bool value)
        {
            obj.SetValue(CanResizeWidthInFloatStateProperty, value);
        }

        /// <summary>
        /// Sets the state of the can resize in docked.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        public static void SetCanResizeInDockedState(DependencyObject obj, bool value)
        {
            obj.SetValue(CanResizeInDockedStateProperty, value);
        }

        /// <summary>
        /// Sets the state of the can resize height in docked.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        public static void SetCanResizeHeightInDockedState(DependencyObject obj, bool value)
        {
            obj.SetValue(CanResizeHeightInDockedStateProperty, value);
        }

        /// <summary>
        /// Sets the state of the can resize width in docked.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        public static void SetCanResizeWidthInDockedState(DependencyObject obj, bool value)
        {
            obj.SetValue(CanResizeWidthInDockedStateProperty, value);
        }

        /// <summary>
        /// Gets the state of the can resize in float.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public static bool GetCanResizeInFloatState(DependencyObject obj)
        {
            return (bool)obj.GetValue(CanResizeInFloatStateProperty);
        }

        /// <summary>
        /// Gets the state of the can resize in docked.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public static bool GetCanResizeInDockedState(DependencyObject obj)
        {
            return (bool)obj.GetValue(CanResizeInDockedStateProperty);
        }

        /// <summary>
        /// Gets the state of the can resize height in docked.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public static bool GetCanResizeHeightInDockedState(DependencyObject obj)
        {
            return (bool)obj.GetValue(CanResizeHeightInDockedStateProperty);
        }

        /// <summary>
        /// Gets the state of the can resize width in docked.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public static bool GetCanResizeWidthInDockedState(DependencyObject obj)
        {
            return (bool)obj.GetValue(CanResizeWidthInDockedStateProperty);
        }

        /// <summary>
        /// Gets the state of the can resize height in float.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public static bool GetCanResizeHeightInFloatState(DependencyObject obj)
        {
            return (bool)obj.GetValue(CanResizeHeightInFloatStateProperty);
        }

        /// <summary>
        /// Gets the state of the can resize width in float.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public static bool GetCanResizeWidthInFloatState(DependencyObject obj)
        {
            return (bool)obj.GetValue(CanResizeWidthInFloatStateProperty);
        }

        /// <summary>
        /// Sets the show hidden menu item.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        public static void SetShowHiddenMenuItem(DependencyObject obj, bool value)
        {
            obj.SetValue(ShowHiddenMenuItemProperty, value);
        }

        /// <summary>
        /// Gets the show hidden menu item.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public static bool GetShowHiddenMenuItem(DependencyObject obj)
        {
            return (bool)obj.GetValue(ShowHiddenMenuItemProperty);
        }

        /// <summary>
        /// Sets the show auto hidden menu item.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        public static void SetShowAutoHiddenMenuItem(DependencyObject obj, bool value)
        {
            obj.SetValue(ShowAutoHiddenMenuItemProperty, value);
        }

        /// <summary>
        /// Gets the show auto hidden menu item.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public static bool GetShowAutoHiddenMenuItem(DependencyObject obj)
        {
           return (bool) obj.GetValue(ShowAutoHiddenMenuItemProperty);
        }

        /// <summary>
        /// Gets the show dockable menu item.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public static bool GetShowDockableMenuItem(DependencyObject obj)
        {
            return (bool)obj.GetValue(ShowDockableMenuItemProperty);
        }

        /// <summary>
        /// Sets the show dockable menu item.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        public static void SetShowDockableMenuItem(DependencyObject obj, bool value)
        {
            obj.SetValue(ShowDockableMenuItemProperty, value);
        }

        /// <summary>
        /// Gets the show floating menu item.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public static bool GetShowFloatingMenuItem(DependencyObject obj)
        {
            return (bool)obj.GetValue(ShowFloatingMenuItemProperty);
        }

        /// <summary>
        /// Sets the show floating menu item.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        public static void SetShowFloatingMenuItem(DependencyObject obj, bool value)
        {
            obj.SetValue(ShowFloatingMenuItemProperty, value);
        }

        /// <summary>
        /// Gets the show tabbed menu item.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public static bool GetShowTabbedMenuItem(DependencyObject obj)
        {
            return (bool)obj.GetValue(ShowTabbedMenuItemProperty);
        }

        /// <summary>
        /// Sets the show tabbed menu item.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        public static void SetShowTabbedMenuItem(DependencyObject obj, bool value)
        {
            obj.SetValue(ShowTabbedMenuItemProperty, value);
        }

        /// <summary>
        /// Gets the show document menu item.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public static bool GetShowDocumentMenuItem(DependencyObject obj)
        {
            return (bool)obj.GetValue(ShowDocumentMenuItemProperty);
        }

        /// <summary>
        /// Sets the show document menu item.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        public static void SetShowDocumentMenuItem(DependencyObject obj, bool value)
        {
            obj.SetValue(ShowDocumentMenuItemProperty, value);
        }


        /// <summary>
        /// Gets the show close menu item.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public static bool GetShowCloseMenuItem(DependencyObject obj)
        {
            return (bool)obj.GetValue(ShowCloseMenuItemProperty);
        }

        /// <summary>
        /// Sets the show close menu item.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        public static void SetShowCloseMenuItem(DependencyObject obj, bool value)
        {
            obj.SetValue(ShowCloseMenuItemProperty, value);
        }

        /// <summary>
        /// Gets the show close all menu item.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public static bool GetShowCloseAllMenuItem(DependencyObject obj)
        {
            return (bool)obj.GetValue(ShowCloseAllMenuItemProperty);
        }

        /// <summary>
        /// Sets the show close all menu item.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        public static void SetShowCloseAllMenuItem(DependencyObject obj, bool value)
        {
            obj.SetValue(ShowCloseAllMenuItemProperty, value);
        }

        /// <summary>
        /// Gets the show close all but this menu item.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public static bool GetShowCloseAllButThisMenuItem(DependencyObject obj)
        {
            return (bool)obj.GetValue(ShowCloseAllButThisMenuItemProperty);
        }

        /// <summary>
        /// Sets the show close all but this menu item.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        public static void SetShowCloseAllButThisMenuItem(DependencyObject obj, bool value)
        {
            obj.SetValue(ShowCloseAllButThisMenuItemProperty, value);
        }

        /// <summary>
        /// Gets the show horizontal tab group menu item.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public static bool GetShowHorizontalTabGroupMenuItem(DependencyObject obj)
        {
            return (bool)obj.GetValue(ShowHorizontalTabGroupMenuItemProperty);
        }

        /// <summary>
        /// Sets the show horizontal tab group menu item.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        public static void SetShowHorizontalTabGroupMenuItem(DependencyObject obj, bool value)
        {
            obj.SetValue(ShowHorizontalTabGroupMenuItemProperty, value);
        }

        /// <summary>
        /// Gets the show vertical tab group menu item.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public static bool GetShowVerticalTabGroupMenuItem(DependencyObject obj)
        {
            return (bool)obj.GetValue(ShowVerticalTabGroupMenuItemProperty);
        }

        /// <summary>
        /// Sets the show vertical tab group menu item.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        public static void SetShowVerticalTabGroupMenuItem(DependencyObject obj, bool value)
        {
            obj.SetValue(ShowVerticalTabGroupMenuItemProperty, value);
        }

        /// <summary>
        /// Gets the show moveto next tab group menu item.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public static bool GetShowMovetoNextTabGroupMenuItem(DependencyObject obj)
        {
            return (bool)obj.GetValue(ShowMovetoNextTabGroupMenuItemProperty);
        }

        /// <summary>
        /// Sets the show moveto next tab group menu item.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        public static void SetShowMovetoNextTabGroupMenuItem(DependencyObject obj, bool value)
        {
            obj.SetValue(ShowMovetoNextTabGroupMenuItemProperty, value);
        }

        /// <summary>
        /// Gets the show moveto previous tab group menu item.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public static bool GetShowMovetoPreviousTabGroupMenuItem(DependencyObject obj)
        {
            return (bool)obj.GetValue(ShowMovetoPreviousTabGroupMenuItemProperty);
        }

        /// <summary>
        /// Sets the show moveto previous tab group menu item.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        public static void SetShowMovetoPreviousTabGroupMenuItem(DependencyObject obj, bool value)
        {
            obj.SetValue(ShowMovetoPreviousTabGroupMenuItemProperty, value);
        }

        /// <summary>
        /// Gets the show restore menu item.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public static bool GetShowRestoreMenuItem(DependencyObject obj)
        {
            return (bool)obj.GetValue(ShowRestoreMenuItemProperty);
        }

        /// <summary>
        /// Sets the show restore menu item.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        public static void SetShowRestoreMenuItem(DependencyObject obj, bool value)
        {
            obj.SetValue(ShowRestoreMenuItemProperty, value);
        }

        /// <summary>
        /// Gets the show move menu item.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public static bool GetShowMoveMenuItem(DependencyObject obj)
        {
            return (bool)obj.GetValue(ShowMoveMenuItemProperty);
        }

        /// <summary>
        /// Sets the show move menu item.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        public static void SetShowMoveMenuItem(DependencyObject obj, bool value)
        {
            obj.SetValue(ShowMoveMenuItemProperty, value);
        }

        /// <summary>
        /// Gets the show resize menu item.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public static bool GetShowResizeMenuItem(DependencyObject obj)
        {
            return (bool)obj.GetValue(ShowResizeMenuItemProperty);
        }

        /// <summary>
        /// Sets the show resize menu item.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        public static void SetShowResizeMenuItem(DependencyObject obj, bool value)
        {
            obj.SetValue(ShowResizeMenuItemProperty, value);
        }


        /// <summary>
        /// Gets the show minimize menu item.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public static bool GetShowMinimizeMenuItem(DependencyObject obj)
        {
            return (bool)obj.GetValue(ShowMinimizeMenuItemProperty);
        }

        /// <summary>
        /// Sets the show minimize menu item.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        public static void SetShowMinimizeMenuItem(DependencyObject obj, bool value)
        {
            obj.SetValue(ShowMinimizeMenuItemProperty, value);
        }


        /// <summary>
        /// Gets the show maximized menu item.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public static bool GetShowMaximizedMenuItem(DependencyObject obj)
        {
            return (bool)obj.GetValue(ShowMaximizedMenuItemProperty);
        }

        /// <summary>
        /// Sets the show maximized menu item.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        public static void SetShowMaximizedMenuItem(DependencyObject obj, bool value)
        {
            obj.SetValue(ShowMaximizedMenuItemProperty, value);
        }

        /// <summary>
        /// Gets the value of the DockingManager.NoDock�attached property from a given DependencyObject.
        /// </summary>
        /// <param name="obj">The element from which to read the property value.</param>
        /// <returns>The value of the DockingManager.NoDock�attached property.</returns>
        /// <remarks>
        /// See <see cref="DockState"/> for all possible DockState cases.
        /// </remarks>
        public static bool GetNoDock(DependencyObject obj)
        {
            return (bool)obj.GetValue(NoDockProperty);
        }

        /// <summary>
        /// Sets the value of the DockingManager.NoDock�attached property to a given DependencyObject.
        /// </summary>
        /// <param name="obj">The element on which to set the DockingManager.NoDock�attached property.</param>
        /// <param name="value">The property value to set.</param>
        public static void SetNoDock(DependencyObject obj, bool value)
        {
            obj.SetValue(NoDockProperty, value);
        }

        /// <summary>
        /// Gets the value of the DockingManager.FloatingWindowRect�attached property from a given DependencyObject.
        /// </summary>
        /// <param name="obj">The element from which to read the property value.</param>
        /// <returns>The value of the DockingManager.FloatingWindowRect�attached property.</returns>
        public static Rect GetFloatingWindowRect(DependencyObject obj)
        {
            return (Rect)obj.GetValue(FloatingWindowRectProperty);
        }
        /// <summary>
        /// Gets the value of the DockingManager.IsContextMenuVisible attached property from a given DependencyObject.
        /// </summary>
        /// <param name="obj">dependency object</param>
        /// <returns>value of dependency object</returns>
        public static bool GetIsContextMenuVisible(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsContextMenuVisibleProperty);
        }

        /// <summary>
        /// Gets the value of the DockingManager.IsContextMenuButtonVisible attached property from a given DependencyObject.
        /// </summary>
        /// <param name="obj">dependency object</param>
        /// <returns>value of dependency object</returns>
        public static bool GetIsContextMenuButtonVisible(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsContextMenuButtonVisibleProperty);
        }

        /// <summary>
        /// Gets the value of the DockingManager.IsRollupFloatWindowProperty attached property from a given DependencyObject.
        /// </summary>
        /// <param name="obj">dependency object</param>
        /// <returns>value of dependency object</returns>
        public static bool GetIsRollupFloatWindow(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsRollupFloatWindowProperty);
        }

        /// <summary>
        /// Sets the value of the DockingManager.IsRollupFloatWindow�attached property to a given DependencyObject.
        /// </summary>
        /// <param name="obj">The element on which to set the DockingManager.IsRollupFloatWindow�attached property.</param>
        /// <param name="value">The property value to set.</param>
        public static void SetIsRollupFloatWindow(DependencyObject obj, bool value)
        {
            obj.SetValue(IsRollupFloatWindowProperty, value);
        }


        /// <summary>
        /// Sets the value of the DockingManager.FloatingWindowRect�attached property to a given DependencyObject.
        /// </summary>
        /// <param name="obj">The element on which to set the DockingManager.FloatingWindowRect�attached property.</param>
        /// <param name="value">The property value to set.</param>
        public static void SetFloatingWindowRect(DependencyObject obj, Rect value)
        {
            obj.SetValue(FloatingWindowRectProperty, value);
        }


        /// <summary>
        /// Sets the value of the DockingManager.IsContextMenuVisible�attached property to a given DependencyObject.
        /// </summary>
        /// <param name="obj">The element on which to set the DockingManager.IsContextMenuVisible�attached property.</param>
        /// <param name="value">The property value to set.</param>
        public static void SetIsContextMenuVisible(DependencyObject obj, bool value)
        {
            obj.SetValue(IsContextMenuVisibleProperty, value);
        }

        /// <summary>
        /// Sets the value of the DockingManager.IsContextMenuButtonVisible�attached property to a given DependencyObject.
        /// </summary>
        /// <param name="obj">The element on which to set the DockingManager.IsContextMenuButtonVisible�attached property.</param>
        /// <param name="value">The property value to set.</param>
        public static void SetIsContextMenuButtonVisible(DependencyObject obj, bool value)
        {
            obj.SetValue(IsContextMenuButtonVisibleProperty, value);
        }
        /// <summary>
        /// Gets the value of the DockingManager.DockToFill�attached property from a given DependencyObject.
        /// </summary>
        /// <param name="obj">The element from which to read the property value.</param>
        /// <returns>The value of the DockingManager.DockToFill�attached property.</returns>
        public static bool GetDockToFill(DependencyObject obj)
        {
            return (bool)obj.GetValue(DockToFillProperty);
        }

        /// <summary>
        /// Sets the value of the DockingManager.DockToFill�attached property to a given DependencyObject.
        /// </summary>
        /// <param name="obj">The element on which to set the DockingManager.DockToFill�attached property.</param>
        /// <param name="value">The property value to set.</param>
        public static void SetDockToFill(DependencyObject obj, bool value)
        {
            obj.SetValue(DockToFillProperty, value);
        }

        /// <summary>
        /// Gets the value of the DockingManager.CustomMenuItems�attached property from a given DependencyObject.
        /// </summary>
        /// <param name="obj">The element from which to read the property value.</param>
        /// <returns>The value of the DockingManager.CustomMenuItems�attached property.</returns>
        public static CustomMenuItemCollection GetCustomMenuItems(DependencyObject obj)
        {
            return (CustomMenuItemCollection)obj.GetValue(CustomMenuItemsProperty);
        }

        /// <summary>
        /// Sets the value of the DockingManager.CustomMenuItems�attached property to a given DependencyObject.
        /// </summary>
        /// <param name="obj">The element on which to set the DockingManager.CustomMenuItems�attached property.</param>
        /// <param name="value">The property value to set.</param>
        public static void SetCustomMenuItems(DependencyObject obj, CustomMenuItemCollection value)
        {
            obj.SetValue(CustomMenuItemsProperty, value);
        }


        /// <summary>
        /// Gets the float window context menu items.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public static CustomMenuItemCollection GetFloatWindowContextMenuItems(DependencyObject obj)
        {
            return (CustomMenuItemCollection)obj.GetValue(FloatWindowContextMenuItemsProperty);
        }

        /// <summary>
        /// Sets the float window context menu items.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="value">The value.</param>
        public static void SetFloatWindowContextMenuItems(DependencyObject obj, CustomMenuItemCollection value)
        {
            obj.SetValue(FloatWindowContextMenuItemsProperty, value);
        }

        /// <summary>
        /// Gets the dock window context menu items.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public static CustomMenuItemCollection GetDockWindowContextMenuItems(DependencyObject obj)
        {
            return (CustomMenuItemCollection)obj.GetValue(DockWindowContextMenuItemsProperty);
        }

        /// <summary>
        /// Sets the dock window context menu items.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="value">The value.</param>
        public static void SetDockWindowContextMenuItems(DependencyObject obj, CustomMenuItemCollection value)
        {
            obj.SetValue(DockWindowContextMenuItemsProperty, value);
        }

        /// <summary>
        /// Gets the document tab item context menu items.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public static DocumentTabItemMenuItemCollection GetDocumentTabItemContextMenuItems(DependencyObject obj)
        {
            return (DocumentTabItemMenuItemCollection)obj.GetValue(DocumentTabItemContextMenuItemsProperty);
        }

        /// <summary>
        /// Sets the document tab item context menu items.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="value">The value.</param>
        public static void SetDocumentTabItemContextMenuItems(DependencyObject obj, DocumentTabItemMenuItemCollection value)
        {
            obj.SetValue(DocumentTabItemContextMenuItemsProperty, value);
        }

        /// <summary>
        /// Gets the value of the DockingManager.HeaderTemplate�attached property from a given DependencyObject.
        /// </summary>
        /// <param name="obj">The element from which to read the property value.</param>
        /// <returns>The value of the DockingManager.HeaderTemplate�attached property.</returns>
        public static DataTemplate GetHeaderTemplate(DependencyObject obj)
        {
            return (DataTemplate)obj.GetValue(HeaderTemplateProperty);
        }

        /// <summary>
        /// Sets the value of the DockingManager.HeaderTemplate�attached property to a given DependencyObject.
        /// </summary>
        /// <param name="obj">The element on which to set the DockingManager.HeaderTemplate�attached property.</param>
        /// <param name="value">The property value to set.</param>
        public static void SetHeaderTemplate(DependencyObject obj, DataTemplate value)
        {
            obj.SetValue(HeaderTemplateProperty, value);
        }

        /// <summary>
        /// Gets the collapse default context menu items in float.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public static bool GetCollapseDefaultContextMenuItemsInFloat(DependencyObject obj)
        {
            return (bool)obj.GetValue(CollapseDefaultContextMenuItemsInFloatProperty);
        }

        /// <summary>
        /// Sets the collapse default context menu items in float.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        public static void SetCollapseDefaultContextMenuItemsInFloat(DependencyObject obj, bool value)
        {
            obj.SetValue(CollapseDefaultContextMenuItemsInFloatProperty, value);
        }

        /// <summary>
        /// Gets the collapse default context menu items in dock.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public static bool GetCollapseDefaultContextMenuItemsInDock(DependencyObject obj)
        {
            return (bool)obj.GetValue(CollapseDefaultContextMenuItemsInDockProperty);
        }

        /// <summary>
        /// Sets the collapse default context menu items in dock.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        public static void SetCollapseDefaultContextMenuItemsInDock(DependencyObject obj, bool value)
        {
            obj.SetValue(CollapseDefaultContextMenuItemsInDockProperty, value);
        }

        /// <summary>
        /// Gets the collapse default context menu items in document tab.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public static bool GetCollapseDefaultContextMenuItemsInDocumentTab(DependencyObject obj)
        {
            return (bool)obj.GetValue(CollapseDefaultContextMenuItemsInDocumentTabProperty);
        }

        /// <summary>
        /// Sets the collapse default context menu items in document tab.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        public static void SetCollapseDefaultContextMenuItemsInDocumentTab(DependencyObject obj, bool value)
        {
            obj.SetValue(CollapseDefaultContextMenuItemsInDocumentTabProperty, value);
        }

        /// <summary>
        /// Gets the width of the child minimized.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        internal static double GetChildMinimizedWidth(DependencyObject obj)
        {
            return (double)obj.GetValue(ChildMinimizedWidthProperty);
        }


        /// <summary>
        /// Sets the width of the child minimized.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="value">The value.</param>
        internal static void SetChildMinimizedWidth(DependencyObject obj, double value)
        {
            obj.SetValue(ChildMinimizedWidthProperty, value);
        }

        /// <summary>
        /// Gets the height of the child minimized.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        internal static double GetChildMinimizedHeight(DependencyObject obj)
        {
            return (double)obj.GetValue(ChildMinimizedHeightProperty);
        }


        /// <summary>
        /// Sets the height of the child minimized.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="value">The value.</param>
        internal static void SetChildMinimizedHeight(DependencyObject obj, double value)
        {
            obj.SetValue(ChildMinimizedHeightProperty, value);
        }


        /// <summary>
        /// Gets the is active window.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public static bool GetIsActiveWindow(DependencyObject obj)
        {
            return (bool)obj.GetValue(DockingManager.IsActiveWindowProperty);
        }

        /// <summary>
        /// Sets the is active window.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        internal static void SetIsActiveWindow(DependencyObject obj,bool value)
        {
            obj.SetValue(DockingManager.IsActiveWindowProperty, value);
        }

        /// <summary>
        /// Gets the value of the DockingManager.CanAutoHide�attached property from a given DependencyObject.
        /// </summary>
        /// <param name="obj">The element from which to read the property value.</param>
        /// <returns>The value of the DockingManager.CanAutoHide�attached property.</returns>
        public static bool GetCanAutoHide(DependencyObject obj)
        {
            return (bool)obj.GetValue(CanAutoHideProperty);
        }

        /// <summary>
        /// Get the value of the DockingManager.closefloat�attached property from a given DependencyObject.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        internal static bool Getclosefloat(DependencyObject obj)
        {
            return (bool)obj.GetValue(closefloatproperty);
        }

        /// <summary>
        /// Sets the value of the DockingManager.AnimationDelay�attached property to a given DependencyObject.
        /// </summary>
        /// <param name="obj">The element on which to set the DockingManager.AnimationDelay�attached property.</param>
        /// <param name="value">The property value to set.</param>
        public static void SetCanAutoHide(DependencyObject obj, bool value)
        {
            obj.SetValue(CanAutoHideProperty, value);
        }

        /// <summary>
        /// Set the value of the DockingManager.closefloat�attached property from a given DependencyObject.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        internal static void Setclosefloatproperty(DependencyObject obj, bool value)
        {
            obj.SetValue(closefloatproperty, value);
        }

        /// <summary>
        /// Sets the is swapped.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        internal static void SetIsSwapped(DependencyObject obj, bool value)
        {
            obj.SetValue(IsSwappedProperty, value);
        }

        /// <summary>
        /// Gets the is swapped.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        internal static bool GetIsSwapped(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsSwappedProperty);
        }

        /// <summary>
        /// Gets the dock fill mode.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public static DockFillModes GetDockFillMode(DependencyObject obj)
        {
            return (DockFillModes)obj.GetValue(DockFillModeProperty);
        }

        /// <summary>
        /// Sets the dock fill mode.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="value">The value.</param>
        public static void SetDockFillMode(DependencyObject obj, DockFillModes value)
        {
            obj.SetValue(DockFillModeProperty, value);
        }

        /// <summary>
        /// Gets the value of the DockingManager.IsFroze�attached property from a given DependencyObject.
        /// </summary>
        /// <param name="obj">The element from which to read the property value.</param>
        /// <returns>The value of the DockingManager.IsFroze�attached property.</returns>
        public static bool GetIsFroze(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsFrozeProperty);
        }

        /// <summary>
        /// Sets the value of the DockingManager.IsFroze�attached property to a given DependencyObject.
        /// </summary>
        /// <param name="obj">The element on which to set the DockingManager.IsFroze�attached property.</param>
        /// <param name="value">The property value to set.</param>
        public static void SetIsFroze(DependencyObject obj, bool value)
        {
            obj.SetValue(IsFrozeProperty, value);
        }

        /// <summary>
        /// Gets the value of the DockingManager.PreviousState�attached property from a given DependencyObject.
        /// </summary>
        /// <param name="obj">The element from which to read the property value.</param>
        /// <returns>The value of the DockingManager.PreviousState�attached property.</returns>
        internal static DockState GetPreviousState(DependencyObject obj)
        {
            return (DockState)obj.GetValue(PreviousStateProperty);
        }

        /// <summary>
        /// Sets the value of the DockingManager.PreviousState�attached property to a given DependencyObject.
        /// </summary>
        /// <param name="obj">The element on which to set the DockingManager.PreviousState�attached property.</param>
        /// <param name="value">The property value to set.</param>
        protected static void SetPreviousState(DependencyObject obj, DockState value)
        {
            obj.SetValue(PreviousStateProperty, value);
        }


        /// <summary>
        /// Gets the previous side in dock mode.
        /// </summary>
        /// <param name="obj">The dependency object.</param>
        /// <returns>return index.</returns>
        public static DockSide GetPreviousSideInDockMode(DependencyObject obj)
        {
            return (DockSide)obj.GetValue(DockingManager.PreviousSideInDockModeProperty);
        }

        /// <summary>
        /// Sets the previous in dock mode.
        /// </summary>
        /// <param name="obj">The dependency object.</param>
        /// <param name="value">The value.</param>
        internal static void SetPreviousSideInDockMode(DependencyObject obj, DockSide value)
        {
            obj.SetValue(DockingManager.PreviousSideInDockModeProperty, value);
        }

        /// <summary>
        /// Sets the IsDragged.
        /// </summary>
        /// <param name="obj">The dependency object.</param>
        /// <param name="value">The value.</param>
        internal static void SetIsDragged(DependencyObject obj, bool value)
        {
            obj.SetValue(DockingManager.IsDraggedProperty, value);
        }

        /// <summary>
        /// Gets the IsDragged
        /// </summary>
        /// <param name="obj">The dependency object.</param>
        /// <returns>return bool.</returns>
        internal static bool GetIsDragged(DependencyObject obj)
        {
            return (bool)obj.GetValue(DockingManager.IsDraggedProperty);
        }



        /// <summary>
        /// Gets the index in dock mode.
        /// </summary>
        /// <param name="obj">The dependency object.</param>
        /// <returns>return index.</returns>
        public static int GetIndexInDockMode(DependencyObject obj)
        {
            return (int)obj.GetValue(DockingManager.IndexInDockModeProperty);
        }

        /// <summary>
        /// Sets the index in dock mode externally.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="value">The value.</param>
        public static void SetIndexInDockModeExternally(DependencyObject obj, int value)
        {
            DockingManager dockingManager = DockingManager.ResolveManager(obj as UIElement);
            if (dockingManager != null)
            {
                dockingManager.Children.m_itemchangedexternally = true;
                DockingManager.SetIndexInDockMode(obj, value);
                dockingManager.Children.m_itemchangedexternally = false;
            }
        }

        /// <summary>
        /// Sets the index in dock mode.
        /// </summary>
        /// <param name="obj">The dependency object.</param>
        /// <param name="value">The value.</param>
        public static void SetIndexInDockMode(DependencyObject obj, int value)
        {
            obj.SetValue(DockingManager.IndexInDockModeProperty, value);
        }

        /// <summary>
        /// Sets the tab parent.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="value">The value.</param>
        public static void SetTabParent(DependencyObject obj, String value)
        {
            obj.SetValue(DockingManager.TabParentProperty, value);
        }

        /// <summary>
        /// Gets the tab parent.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public static String GetTabParent(DependencyObject obj)
        {
            return (String)obj.GetValue(DockingManager.TabParentProperty);
        }
        
        /// <summary>
        /// Sets the Previous index in dock mode.
        /// </summary>
        /// <param name="obj">The dependency object.</param>
        /// <param name="value">The value.</param>        
        internal static void SetPreviousIndexInDockMode(DependencyObject obj, int value)
        {
            obj.SetValue(DockingManager.PreviousIndexInDockModeProperty, value);
        }
        /// <summary>
        /// Gets the Previous index in dock mode.
        /// </summary>
        /// <param name="obj">The dependency object.</param>
        /// <returns>return index.</returns>       
        public static int GetPreviousIndexInDockMode(DependencyObject obj)
        {
            return (int)obj.GetValue(DockingManager.PreviousIndexInDockModeProperty);
        }


        /// <summary>
        /// Sets the Previous Target Elements in dock mode.
        /// </summary>
        /// <param name="obj">The dependency object.</param>
        /// <param name="value">The value.</param>
        internal static void SetPreviousTargetInDockMode(DependencyObject obj, String value)
        {
            obj.SetValue(DockingManager.PreviousTargetInDockModeProperty, value);
        }


        /// <summary>
        /// Gets the Previous Target Elements in dock mode.
        /// </summary>
        /// <param name="obj">The dependency object.</param>
        internal static String GetPreviousTargetInDockMode(DependencyObject obj)
        {
            return (String)obj.GetValue(DockingManager.PreviousTargetInDockModeProperty);
        }


        /// <summary>
        /// Sets the Previous Target Elements in dock mode.
        /// </summary>
        /// <param name="obj">The dependency object.</param>
        /// <param name="value">The value.</param>
        internal static void SetTargetInDockModeTab(DependencyObject obj, String value)
        {
            obj.SetValue(DockingManager.TargetInDockModeTabProperty, value);
        }


        /// <summary>
        /// Gets the Previous Target Elements in dock mode.
        /// </summary>
        /// <param name="obj">The dependency object.</param>
        internal static String GetTargetInDockModeTab(DependencyObject obj)
        {
            return (String)obj.GetValue(DockingManager.TargetInDockModeTabProperty);
        }

        /// <summary>
        /// Sets the Previous Parent Side Elements in dock mode.
        /// </summary>
        /// <param name="obj">The dependency object.</param>
        /// <param name="value">The value.</param>
        internal static void SetPreviousParentSideInDockMode(DependencyObject obj, DockSide value)
        {
            obj.SetValue(DockingManager.PreviousParentSideInDockModeProperty, value);
        }


        /// <summary>
        /// Gets the Previous Parent Side Elements in dock mode.
        /// </summary>
        /// <param name="obj">The dependency object.</param>
        internal static DockSide GetPreviousParentSideInDockMode(DependencyObject obj)
        {
            return (DockSide)obj.GetValue(DockingManager.PreviousParentSideInDockModeProperty);
        }



        /// <summary>
        /// Sets the Is Target Changed in dock mode.
        /// </summary>
        /// <param name="obj">The dependency object.</param>
        /// <param name="value">The value.</param>
        internal static void SetIsTargetChanged(DependencyObject obj, bool value)
        {
            obj.SetValue(DockingManager.IsTargetChangedProperty, value);
        }


        /// <summary>
        /// Gets the Is Target Changed in dock mode.
        /// </summary>
        /// <param name="obj">The dependency object.</param>
        internal static bool GetIsTargetChanged(DependencyObject obj)
        {
            return (bool)obj.GetValue(DockingManager.IsTargetChangedProperty);
        }



        /// <summary>
        /// Sets the Previous Child Elements in dock mode.
        /// </summary>
        /// <param name="obj">The dependency object.</param>
        /// <param name="value">The value.</param>
        internal static void SetPreviousChildElements(DependencyObject obj, List<String> value)
        {
            obj.SetValue(DockingManager.PreviousChildElementsProperty, value);
        }

       
        /// <summary>
        /// Gets the Previous Child Elements in dock mode.
        /// </summary>
        /// <param name="obj">The dependency object.</param>
        /// <returns>return index.</returns>
        public static List<String> GetPreviousChildElements(DependencyObject obj)
        {
            return (List<String>)obj.GetValue(DockingManager.PreviousChildElementsProperty);
            
        }


        /// <summary>
        /// Gets the Previous Child Elements in dock mode.
        /// </summary>
        /// <param name="obj">The dependency object.</param>
        /// <returns>return index.</returns>
        public static DockSide GetSideRelativetoContainer(DependencyObject obj)
        {
            return (DockSide)obj.GetValue(DockingManager.SideRelativetoContainerProperty);
        }


        /// <summary>
        /// Sets the Side Relative to Container.
        /// </summary>
        /// <param name="obj">The dependency object.</param>
        /// <param name="value">The value.</param>
        public static void SetSideRelativetoContainer(DependencyObject obj, DockSide value)
        {
            obj.SetValue(DockingManager.SideRelativetoContainerProperty, value);
        }
        /// <summary>
        /// Gets the index in float mode.
        /// </summary>
        /// <param name="obj">The dependency object.</param>
        /// <returns>return index.</returns>
        public static int GetIndexInFloatMode(DependencyObject obj)
        {
            return (int)obj.GetValue(DockingManager.IndexInFloatModeProperty);
        }

        /// <summary>
        /// Sets the index in float mode externally.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="value">The value.</param>
        public static void SetIndexInFloatModeExternally(DependencyObject obj, int value)
        {
            DockingManager dockingManager = DockingManager.ResolveManager(obj as UIElement);
            if (dockingManager != null)
            {
                dockingManager.Children.m_itemchangedexternally = true;
                DockingManager.SetIndexInFloatMode(obj, value);
                dockingManager.Children.m_itemchangedexternally = false;
            }
        }

        /// <summary>
        /// Sets the index in float mode.
        /// </summary>
        /// <param name="obj">The dependency object.</param>
        /// <param name="value">The value.</param>
        internal static void SetIndexInFloatMode(DependencyObject obj, int value)
        {
            obj.SetValue(DockingManager.IndexInFloatModeProperty, value);
        }

        /// <summary>
        /// Gets the document tab header style.
        /// </summary>
        /// <param name="obj">The obj DependencyObject.</param>
        /// <returns>The value DependencyObject.</returns>
        public static Style GetDocumentTabItemStyle(DependencyObject obj)
        {
            return (Style)obj.GetValue(DocumentTabItemStyleProperty);
        }

        /// <summary>
        /// Sets the document tab header style.
        /// </summary>
        /// <param name="obj">The obj DependencyObject.</param>
        /// <param name="value">The value DependencyObject.</param>
        public static void SetDocumentTabItemStyle(DependencyObject obj, Style value)
        {
            obj.SetValue(DocumentTabItemStyleProperty, value);
        }

        /// <summary>
        /// Gets the document tab control style.
        /// </summary>
        /// <param name="obj">The obj value.</param>
        /// <returns>Return the Object.</returns>
        public static Style GetDocumentTabControlStyle(DependencyObject obj)
        {
            return obj.GetValue(DocumentTabControlStyleProperty) as Style;
        }

        /// <summary>
        /// Sets the document tab header style.
        /// </summary>
        /// <param name="obj">The obj DependencyObject.</param>
        /// <param name="value">The value DependencyObject.</param>
        public static void SetDocumentTabControlStyle(DependencyObject obj, Style value)
        {
            obj.SetValue(DocumentTabControlStyleProperty, value);
        }

        /// <summary>
        /// Gets the document mdi control style.
        /// </summary>
        /// <param name="obj">The obj value.</param>
        /// <returns>Return the Object.</returns>
        public static object GetDocumentMDIHeaderStyle(DependencyObject obj)
        {
            return obj.GetValue(DocumentMDIHeaderStyleProperty);
        }

        /// <summary>
        /// Sets the document mdi header style.
        /// </summary>
        /// <param name="obj">The obj DependencyObject.</param>
        /// <param name="value">The value DependencyObject.</param>
        public static void SetDocumentMDIHeaderStyle(DependencyObject obj, object value)
        {
            obj.SetValue(DocumentMDIHeaderStyleProperty, value);
        }

        /// <summary>
        /// Gets the value of can drag for tab item
        /// </summary>
        /// <param name="obj">The obj value.</param>
        /// <returns>Return the Object.</returns>
        public static bool GetCanDragTab(DependencyObject obj)
        {
            return (bool)obj.GetValue(CanDragTabProperty);
        }

        /// <summary>
        /// Sets the CanDrag for tab item
        /// </summary>
        /// <param name="obj">The obj DependencyObject.</param>
        /// <param name="value">The value DependencyObject.</param>
        public static void SetCanDragTab(DependencyObject obj, bool value)
        {
            obj.SetValue(CanDragTabProperty, value);
        }


        internal static bool GetIsLogicalChild(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsLogicalChildProperty);
        }

        internal static void SetIsLogicalChild(DependencyObject obj, bool value)
        {
            obj.SetValue(IsLogicalChildProperty, value);
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets DockTabAlignment. This is a dependency property.
        /// </summary>
        /// <remarks>
        /// DockTabAlignment property is used as tab strip placement for all
        /// DockedElementTabbedHosts inside DockingManager instance. Default value is Dock.Bottom.
        /// </remarks>
        public Dock DockTabAlignment
        {
            get
            {
                return (Dock)GetValue(DockTabAlignmentProperty);
            }

            set
            {
                SetValue(DockTabAlignmentProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [collapse default context menu items].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [collapse default context menu items]; otherwise, <c>false</c>.
        /// </value>
        public bool CollapseDefaultContextMenuItems
        {
            get
            {
                return (bool)GetValue(CollapseDefaultContextMenuItemsProperty);
            }
            set
            {
                SetValue(CollapseDefaultContextMenuItemsProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [collapse default tab list context menu items].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [collapse default tab list context menu items]; otherwise, <c>false</c>.
        /// </value>
        public bool CollapseDefaultTabListContextMenuItems
        {
            get
            {
                return (bool)GetValue(CollapseDefaultTabListContextMenuItemsProperty);
            }
            set
            {
                SetValue(CollapseDefaultTabListContextMenuItemsProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the tab list context menu items.
        /// </summary>
        /// <value>The tab list context menu items.</value>
        public DocumentTabItemMenuItemCollection TabListContextMenuItems
        {
            get
            {
                return (DocumentTabItemMenuItemCollection)GetValue(TabListContextMenuItemsProperty);
            }
            set
            {
                SetValue(TabListContextMenuItemsProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the CloseTabs mode. This is a dependency property.
        /// </summary>
        /// <remarks>
        /// CloseTabs property is used to specify the behavior for tabs closing. Two possible cases are available
        /// - CloseAll and CloseActive. If the first case is set - all tabs inside tabbed host will be closed, otherwise
        /// only the active one will move to hidden state. Default value is CloseTabsMode.CloseActive.
        /// </remarks>
        public CloseTabsMode CloseTabs
        {
            get
            {
                return (CloseTabsMode)GetValue(CloseTabsProperty);
            }

            set
            {
                SetValue(CloseTabsProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether hot tracking is enabled. This is a dependency property.
        /// </summary>
        /// <remarks>
        /// IsEnableHotTracking property allows highlighting hosts headers when mouse is over them.
        /// Works only in default skin. Default value is true.
        /// </remarks>
        public bool IsEnableHotTracking
        {
            get
            {
                return (bool)GetValue(IsEnableHotTrackingProperty);
            }

            set
            {
                SetValue(IsEnableHotTrackingProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [maximize button enabled].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [maximize button enabled]; otherwise, <c>false</c>.
        /// </value>
        public bool MaximizeButtonEnabled
        {
            get
            {
                return (bool)GetValue(MaximizeButtonEnabledProperty);
            }

            set
            {
                SetValue(MaximizeButtonEnabledProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [minimize button enabled].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [minimize button enabled]; otherwise, <c>false</c>.
        /// </value>
        public bool MinimizeButtonEnabled
        {
            get
            {
                return (bool)GetValue(MinimizeButtonEnabledProperty);
            }

            set
            {
                SetValue(MinimizeButtonEnabledProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the minimized item visibility.
        /// </summary>
        /// <value>The minimized item visibility.</value>
        public MaximizeMode MaximizeMode
        {
            get
            {
                return (MaximizeMode)GetValue(MaximizeModeProperty);
            }

            set
            {
                SetValue(MaximizeModeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the AutoHideTabsMode. This is a dependency property.
        /// </summary>
        /// <remarks>
        /// AutoHideTabsMode property is used to specify the behavior for auto hiding tabs. Two possible cases are available
        /// - AutoHideGroup and AutoHideActive. If the first case is set - all tabs inside tabbed host will be auto hidden otherwise
        /// only the active one will move to auto hidden state. Default value is AutoHideTabsMode.AutoHideGroup.
        /// </remarks>        
        public AutoHideTabsMode AutoHideTabsMode
        {
            get
            {
                return (AutoHideTabsMode)GetValue(AutoHideTabsModeProperty);
            }

            set
            {
                SetValue(AutoHideTabsModeProperty, value);
            }
        }

        /// <summary>
        /// Identifies DockingManager.MyActiveWindowChangedEvent event.
        /// </summary>
        public static readonly RoutedEvent TunnelActiveWindowChangedEvent = EventManager.RegisterRoutedEvent(
            "TunnelActiveWindowChanged",
            RoutingStrategy.Tunnel,
            typeof(EventHandler),
            typeof(DockingManager));

        /// <summary>
        /// Tunnel Routed event which fires on ActiveWindow changed
        /// </summary>
        public event RoutedEventHandler TunnelActiveWindowChanged
        {
            add
            {
                AddHandler(TunnelActiveWindowChangedEvent, value);
            }

            remove
            {
                RemoveHandler(TunnelActiveWindowChangedEvent, value);
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether to show auto hide buttons. This is a dependency property.
        /// </summary>
        /// <remarks>
        /// AutoHideVisibility property is used to allow / deny changing elements state to auto hidden through GUI 
        /// by enabling / disabling awl button in host header and Auto hide context menu item. Default value is true.
        /// </remarks>
        public bool AutoHideVisibility
        {
            get
            {
                return (bool)GetValue(AutoHideVisibilityProperty);
            }

            set
            {
                SetValue(AutoHideVisibilityProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the auto hide animation mode. This is a dependency property.
        /// </summary>
        /// <remarks>
        /// Docking manager supports 3 kinds of auto hide animations - Slide, Scale and Fade. Animation is used
        /// to show how element appears or disappears while mouse cursor enter or leave it.
        /// </remarks>
        public AutoHideAnimationMode AutoHideAnimationMode
        {
            get
            {
                return (AutoHideAnimationMode)GetValue(AutoHideAnimationModeProperty);
            }

            set
            {
                SetValue(AutoHideAnimationModeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [use document container].
        /// </summary>
        /// <value>
        /// <c>true</c> if [use document container]; otherwise, <c>false</c>.
        /// </value>
        public bool UseDocumentContainer
        {
            get
            {
                return (bool)GetValue(UseDocumentContainerProperty);
            }

            set
            {
                SetValue(UseDocumentContainerProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [use interop compatibility mode]. This is a dependency property.
        /// </summary>
        /// <value>
        /// <c>true</c> if [use interop compatibility mode]; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        /// If DockingManager contains a Windows Forms Control need to set this property to true. Default value is false.
        /// </remarks>
        public bool UseInteropCompatibilityMode
        {
            get
            {
                return (bool)GetValue(UseInteropCompatibilityModeProperty);
            }

            set
            {
                SetValue(UseInteropCompatibilityModeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is tab preview enabled.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is tab preview enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsTabPreviewEnabled
        {
            get
            {
                return (bool)GetValue(IsTabPreviewEnabledProperty);
            }
            set
            {
                SetValue(IsTabPreviewEnabledProperty, value);
            }
        }
        /// <summary>
        /// Gets or Sets IsAnimationEnabledOnMouseOverProperty Hide property
        /// </summary>
        public bool IsAnimationEnabledOnMouseOver
        {
            get
            {
                return (bool)GetValue(IsAnimationEnabledOnMouseOverProperty);
            }

            set
            {
                SetValue(IsAnimationEnabledOnMouseOverProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [enable scrollable side panel].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [enable scrollable side panel]; otherwise, <c>false</c>.
        /// </value>
        public bool EnableScrollableSidePanel
        {
            get
            {
                return (bool)GetValue(EnableScrollableSidePanelProperty);
            }

            set
            {
                SetValue(EnableScrollableSidePanelProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the active window.
        /// </summary>
        /// <value>The active window.</value>
        public FrameworkElement ActiveWindow
        {
            get
            {
                return (FrameworkElement)GetValue(ActiveWindowProperty);
            }

            set
            {
                SetValue(ActiveWindowProperty, value);
                if (ActiveWindow != null)
                   ActiveWindow.Focus();
            }
        }

    
        /// <summary>
        /// Gets or sets a value indicating whether [dock fill].
        /// </summary>
        /// <value><c>true</c> if [dock fill]; otherwise, <c>false</c>.</value>
        public bool DockFill
        {
            get
            {
                return (bool)GetValue(DockFillProperty);
            }

            set
            {
                SetValue(DockFillProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the dock fill document mode.
        /// </summary>
        /// <value>The dock fill document mode.</value>
        public DockFillDocumentMode DockFillDocumentMode
        {
            get
            {
                return (DockFillDocumentMode)GetValue(DockFillDocumentModeProperty);
            }

            set
            {
                SetValue(DockFillDocumentModeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the size change on maximize.
        /// </summary>
        /// <value>The size change on maximize.</value>
        public SizeChangeOnMaximizeMode SizeChangeOnMaximize
        {
            get
            {
                return (SizeChangeOnMaximizeMode)GetValue(SizeChangeOnMaximizeProperty);
            }

            set
            {
                SetValue(SizeChangeOnMaximizeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the maximize button mode.
        /// </summary>
        /// <value>The maximize button mode.</value>
        public VisibilityMode MaximizeButtonMode
        {
            get
            {
                return (VisibilityMode)GetValue(MaximizeButtonModeProperty);
            }

            set
            {
                SetValue(MaximizeButtonModeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the container splitter resize.
        /// </summary>
        /// <value>The container splitter resize.</value>
        public SplitterResizeMode ContainerSplitterResize
        {
            get
            {
                return (SplitterResizeMode)GetValue(ContainerSplitterResizeProperty);
            }

            set
            {
                SetValue(ContainerSplitterResizeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the client control.
        /// </summary>
        /// <value>The client control.</value>
        public UIElement ClientControl
        {
            get
            {
                return (UIElement)GetValue(ClientControlProperty);
            }

            set
            {
                SetValue(ClientControlProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [use adorner drag provider].
        /// </summary>
        /// <value>
        /// <c>true</c> if [use adorner drag provider]; otherwise, <c>false</c>.
        /// </value>
        public bool UseAdornerDragProvider
        {
            get
            {
                return (bool)GetValue(UseAdornerDragProviderProperty);
            }

            set
            {
                SetValue(UseAdornerDragProviderProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [use adorner float window].
        /// </summary>
        /// <value>
        /// <c>true</c> if [use adorner float window]; otherwise, <c>false</c>.
        /// </value>
        public bool UseAdornerFloatWindow
        {
            get
            {
                return (bool)GetValue(UseAdornerFloatWindowProperty);
            }

            set
            {
                SetValue(UseAdornerFloatWindowProperty, value);
            }
        }
        /// <summary>
        /// Gets,Sets Value of IsContextMenuButtonVisibleProperty attached property
        /// </summary>
        public bool IsContextMenuButtonVisible
        {
            get
            {
                return (bool)GetValue(IsContextMenuButtonVisibleProperty);
            }
            set
            {
                SetValue(IsContextMenuButtonVisibleProperty, value);
            }
        }

        /// <summary>
        /// Gets,Sets Value of IsRollupFloatWindow attached property
        /// </summary>
        public bool IsRollupFloatWindow
        {
            get
            {
                return (bool)GetValue(IsRollupFloatWindowProperty);
            }
            set
            {
                SetValue(IsRollupFloatWindowProperty, value);
            }
        }

        /// <summary>
        /// Gets,Sets Value of IsContextMenuVisibleProperty attached property
        /// </summary>

        public bool IsContextMenuVisible
        {
            get
            {
                return (bool)GetValue(IsContextMenuVisibleProperty);
            }
            set
            {
                SetValue(IsContextMenuVisibleProperty, value);
            }
        }
        /// <summary>
        /// Gets or Sets HideTDIHeaderOnsingleChild Dependency property
        /// </summary>
        public bool HideTDIHeaderOnSingleChild
        {
            get
            {
                return (bool)GetValue(HideTDIHeaderOnSingleChildProperty);
            }
            set
            {
                SetValue(HideTDIHeaderOnSingleChildProperty, value);
            }
        }
        /// <summary>
        /// Gets or sets IsTDIDragDropEnabled property
        /// </summary>
        public bool IsTDIDragDropEnabled
        {
            get
            {
                return (bool)GetValue(IsTDIDragDropEnabledProperty);
            }
            set
            {
                SetValue(IsTDIDragDropEnabledProperty, value);
            }
        }

        /// <summary>
        /// Gets or Sets the value of TabGroupEnabled dependency property
        /// </summary>
        public bool TabGroupEnabled
        {
            get
            {
                return (bool)GetValue(TabGroupEnabledProperty);
            }
            set
            {
                SetValue(TabGroupEnabledProperty, value);
            }
        } 


        public bool UseNativeFloatWindow
        {
            get
            {
                return (bool)GetValue(UseNativeFloatWindowProperty);
            }
            set
            {
                SetValue(UseNativeFloatWindowProperty, value);
            }
        }

        public static bool GetActivateOnClose(DependencyObject obj)
        {
            return (bool)obj.GetValue(ActivateOnCloseProperty);
        }

        public static void SetActivateOnClose(DependencyObject obj, bool value)
        {
            obj.SetValue(ActivateOnCloseProperty, value);
        }

        // Using a DependencyProperty as the backing store for ActivateOnClose.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ActivateOnCloseProperty =
            DependencyProperty.RegisterAttached("ActivateOnClose", typeof(bool), typeof(DockingManager), new UIPropertyMetadata(false));


        /// <summary>
        /// Gets the docking manager.
        /// </summary>
        /// <param name="obj">The dependency object.</param>
        /// <returns>return docking manager.</returns>
        public static DockingManager GetDockingManager(DependencyObject obj)
        {
            return (DockingManager)obj.GetValue(DockingManager.DockingManagerProperty);
        }

        /// <summary>
        /// Sets the docking manager.
        /// </summary>
        /// <param name="obj">The dependency object.</param>
        /// <param name="value">The value.</param>
        public static void SetDockingManager(DependencyObject obj, DockingManager value)
        {
            obj.SetValue(DockingManager.DockingManagerProperty, value);
        }

        /// <summary>
        /// Gets the internal data context.
        /// </summary>
        /// <param name="obj">The obj value.</param>
        /// <returns>data context</returns>
        public static FrameworkElement GetInternalDataContext(DependencyObject obj)
        {
            return (FrameworkElement)obj.GetValue(DockingManager.InternalDataContextProperty);
        }

        /// <summary>
        /// Sets the internal data context.
        /// </summary>
        /// <param name="obj">The obj value.</param>
        /// <param name="value">The value.</param>
        public static void SetInternalDataContext(DependencyObject obj, DependencyObject value)
        {
            obj.SetValue(DockingManager.InternalDataContextProperty, value);
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Shows AutoHideAnimationcomplete
        /// </summary>
        internal bool m_autohideanimationcomplete = true;
        /// <summary>
        /// Stores the lastanimation item 
        /// </summary>
        internal FrameworkElement m_lastanimationitem = null;
       
        /// <summary>
        /// Fires the auto hide animation start.
        /// </summary>
        /// <param name="source">The source.</param>
        internal void FireAutoHideAnimationStart(FrameworkElement source)
        {
            RoutedEventArgs args = new RoutedEventArgs(AutoHideAnimationStartEvent, source);
            if (!m_autohideanimationcomplete && m_lastanimationitem != null)
            {
                if (Children.Contains(m_lastanimationitem))
                {
                    RoutedEventArgs completeanimationargs = new RoutedEventArgs(AutoHideAnimationStopEvent, m_lastanimationitem);
                    RaiseEvent(completeanimationargs);
                    m_lastanimationitem = null;
                    m_autohideanimationcomplete = true;
                }
            }
            RaiseEvent(args);
            m_lastanimationitem = source;
            m_autohideanimationcomplete = false;
        }

        /// <summary>
        /// Fires the auto hide animation stop.
        /// </summary>
        /// <param name="source">The source.</param>
        internal void FireAutoHideAnimationStop(FrameworkElement source)
        {
            RoutedEventArgs args = new RoutedEventArgs(AutoHideAnimationStopEvent, source);
            if (!m_autohideanimationcomplete)
            {
                RaiseEvent(args);
                m_lastanimationitem = null;
                m_autohideanimationcomplete = true;
            }
        }

        /// <summary>
        /// Fires the window drag start.
        /// </summary>
        /// <param name="source">The source.</param>
        internal void FireWindowDragStart(FrameworkElement source)
        {
            RoutedEventArgs args = new RoutedEventArgs(WindowDragStartEvent, source);
            RaiseEvent(args);
        }

        /// <summary>
        /// Fires the transferred from manager.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="args">The <see cref="Syncfusion.Windows.Tools.Controls.TransferManagerEventArgs"/> instance containing the event data.</param>
        internal void FireTransferredFromManager(FrameworkElement source, TransferManagerEventArgs args)
        {
            if (TransferredFromManager != null)
            {
                TransferredFromManager(source, args);
            }
        }


        internal void FireDockProviderShownEvent(DockedElementTabbedHost host)
        {
            DockProviderShownEventArgs args = new DockProviderShownEventArgs();
            args.SourceElement = m_draggedElement;
            args.TargetElement = host.HostedElement;
            if (args.TargetElement != null)
            args.TargetState = DockingManager.GetState(args.TargetElement);

            if (DockProviderShown != null)
            {
                DockProviderShown(this, args);
            }
        }

        /// <summary>
        /// Fires the transferred to manager.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="args">The <see cref="Syncfusion.Windows.Tools.Controls.TransferManagerEventArgs"/> instance containing the event data.</param>
        internal void FireTransferredToManager(FrameworkElement source, TransferManagerEventArgs args)
        {
            if (TransferredToManager != null)
            {
                TransferredToManager(source, args);
            }
        }

        /// <summary>
        /// Fires the window moving.
        /// </summary>
        /// <param name="source">The source.</param>
        internal void FireWindowMoving(FrameworkElement source,WindowMovingEventArgs args)
        {
            if (WindowMoving != null)
            {
                WindowMoving(source, args);
            }
        }

        /// <summary>
        /// Fires after while resizing window i.e window may be float or dock
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        internal void FireWindowResizingEvent(FrameworkElement source,WindowResizingEventArgs args)
        {
            if (WindowResizing != null)
            {
                WindowResizing(source, args);
            }
        }

        /// <summary>
        /// Fires the window closing event.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="args">The <see cref="Syncfusion.Windows.Tools.Controls.WindowClosingEventArgs"/> instance containing the event data.</param>
        internal void FireWindowClosingEvent(FrameworkElement source, WindowClosingEventArgs args)
        {
            if (WindowClosing != null)
            {
                WindowClosing(source, args);
                if(args.TargetItem!=null)
                DockingManager.Setclosefloatproperty(args.TargetItem, true);
            }
        }
        /// <summary>
        /// Fires the before dock to float.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="Syncfusion.Windows.Tools.Controls.DockStateChangingEventArgs"/> instance containing the event data.</param>
        internal void FireDockStateChanging(FrameworkElement sender,DockStateChangingEventArgs args)
        {
            if (DockStateChanging != null)
            {
                DockStateChanging(sender, args);
            }
        }

        /// <summary>
        /// Fires the active window changing.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="Syncfusion.Windows.Tools.Controls.DockStateChangingEventArgs"/> instance containing the event data.</param>
        internal void FireActiveWindowChanging(FrameworkElement sender, ActiveWindowChangingEventArgs args)
        {
            m_setactivewindow = true;
            if (ActiveWindowChanging != null)
            {
                ActiveWindowChanging(sender, args);
            }
        }
        
        /// <summary>
        /// Fires the window drag end.
        /// </summary>
        /// <param name="source">The source.</param>
        internal void FireWindowDragEnd(FrameworkElement source)
        {
            RoutedEventArgs args = new RoutedEventArgs(WindowDragEndEvent, source);
            RaiseEvent(args);
        }

        /// <summary>
        /// Called when [element hidden].
        /// </summary>
        /// <param name="sender">The sender.</param>
        internal void OnElementHidden(UIElement sender)
        {
            ElementHiddenEventHandler handler = ElementHidden;

            if (handler != null)
            {
                handler(sender);
            }
        }

        /// <summary>
        /// Called when [element shown].
        /// </summary>
        /// <param name="sender">The sender.</param>
        internal void OnElementShown(UIElement sender)
        {
            ElementShownEventHandler handler = ElementShown;

            if (handler != null)
            {
                handler(sender);
            }
        }

        /// <summary>
        /// Finds the child.
        /// </summary>
        /// <param name="nameForFind">The name for find.</param>
        /// <returns>return FrameworkElement</returns>
        internal FrameworkElement FindChild(string nameForFind)
        {
            foreach (FrameworkElement element in Children)
            {
                if (nameForFind == element.Name)
                {
                    return element;
                }
            }
            return null;
            //throw new ArgumentException("Incorrect name for search element");
        }

        /// <summary>
        /// Sets the target name safe.
        /// </summary>
        /// <param name="obj">The dependency object.</param>
        /// <param name="targetName">Name of the target.</param>
        /// <param name="state">The dock state.</param>
        internal static void SetTargetNameSafe(DependencyObject obj, string targetName, DockState state)
        {
            switch (state)
            {
                case DockState.Float:
                    SetTargetNameInFloatingMode(obj, targetName);
                    break;

                default:
                    SetTargetNameInDockedMode(obj, targetName);
                    break;
            }
        }

        /// <summary>
        /// Gets the target name safe.
        /// </summary>
        /// <param name="obj">The dependency object.</param>
        /// <param name="state">The dock state.</param>
        /// <returns>return DockState.</returns>
        internal static string GetTargetNameSafe(DependencyObject obj, DockState state)
        {
            switch (state)
            {
                case DockState.Float:
                    return GetTargetNameInFloatingMode(obj);

                default:
                    return GetTargetNameInDockedMode(obj);
            }
        }

        /// <summary>
        /// Sets the side safe.
        /// </summary>
        /// <param name="obj">The dependency object.</param>
        /// <param name="side">The dock side.</param>
        /// <param name="state">The dock state.</param>
        internal static void SetSideSafe(DependencyObject obj, DockSide side, DockState state)
        {
            switch (state)
            {
                case DockState.Float:
                    SetSideInFloatMode(obj, side);
                    break;

                default:
                    SetSideInDockedMode(obj, side);
                    break;
            }
        }

        /// <summary>
        /// Gets the side safe.
        /// </summary>
        /// <param name="obj">The dependency object.</param>
        /// <param name="state">The dock state.</param>
        /// <returns>return dock state.</returns>
        internal static DockSide GetSideSafe(DependencyObject obj, DockState state)
        {
            switch (state)
            {
                case DockState.Float:
                    return GetSideInFloatMode(obj);
                default:
                    return GetSideInDockedMode(obj);
            }
        }

        /// <summary>
        /// Gets the is added element.
        /// </summary>
        /// <param name="obj">The dependency object.</param>
        /// <returns>return bool value</returns>
        internal static bool GetIsAddedElement(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsAddedElementProperty);
        }

        /// <summary>
        /// Sets the is added element.
        /// </summary>
        /// <param name="obj">The dependency object.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        internal static void SetIsAddedElement(DependencyObject obj, bool value)
        {
            obj.SetValue(IsAddedElementProperty, value);
        }

        /// <summary>
        /// Gets the dock info.
        /// </summary>
        /// <param name="obj">The dependency object.</param>
        /// <returns>return DockInfoInternal value.</returns>
        internal static DockInfoInternal GetDockInfo(DependencyObject obj)
        {
            return (DockInfoInternal)obj.GetValue(DockInfoProperty);
        }

        /// <summary>
        /// Sets the dock info.
        /// </summary>
        /// <param name="obj">The dependency object.</param>
        /// <param name="value">The value.</param>
        internal static void SetDockInfo(DependencyObject obj, DockInfoInternal value)
        {
            obj.SetValue(DockInfoPropertyKey, value);
        }

        /// <summary>
        /// Gets the is selected tab.
        /// </summary>
        /// <param name="obj">The dependency object.</param>
        /// <returns>return bool value.</returns>
        public static bool GetIsSelectedTab(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsSelectedTabProperty);
        }

        /// <summary>
        /// Sets the is selected tab.
        /// </summary>
        /// <param name="obj">The dependency object.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        public static void SetIsSelectedTab(DependencyObject obj, bool value)
        {
            obj.SetValue(IsSelectedTabProperty, value);
        }

        /// <summary>
        /// Gets the is PrevChild tab.
        /// </summary>
        /// <param name="obj">The dependency object.</param>
        /// <returns>return bool value.</returns>
        internal static PrevChildInfo GetPrevChild(DependencyObject obj)
        {
            return (PrevChildInfo)obj.GetValue(PrevChildProperty);
        }

        /// <summary>
        /// Sets the is PrevChild tab.
        /// </summary>
        /// <param name="obj">The dependency object.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        internal static void SetPrevChild(DependencyObject obj, PrevChildInfo value)
        {
            obj.SetValue(PrevChildProperty, value);
        }

        /// <summary>
        /// Gets the has focus.
        /// </summary>
        /// <param name="obj">The dependency object.</param>
        /// <returns>return bool value.</returns>
        internal static bool GetHasFocus(DependencyObject obj)
        {
            return (bool)obj.GetValue(HasFocusProperty);
        }

        /// <summary>
        /// Sets the has focus.
        /// </summary>
        /// <param name="obj">The dependency object.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        internal static void SetHasFocus(DependencyObject obj, bool value)
        {
            obj.SetValue(HasFocusProperty, value);
        }

        /// <summary>
        /// Gets the side panel dock.
        /// </summary>
        /// <param name="obj">The dependency object.</param>
        /// <returns>return Dock.</returns>
        public static Dock GetSidePanelDock(DependencyObject obj)
        {
            return (Dock)obj.GetValue(DockingManager.SidePanelDockProperty);
        }

        /// <summary>
        /// Sets the side panel dock.
        /// </summary>
        /// <param name="obj">The dependency object.</param>
        /// <param name="value">The value.</param>
        public static void SetSidePanelDock(DependencyObject obj, Dock value)
        {
            obj.SetValue(DockingManager.SidePanelDockProperty, value);
        }

        /// <summary>
        /// Gets the dock ability.
        /// </summary>
        /// <param name="obj">The dependency object.</param>
        /// <returns>return DockAbility.</returns>
        public static DockAbility GetDockAbility(DependencyObject obj)
        {
            return (DockAbility)obj.GetValue(DockingManager.DockAbilityProperty);
        }

        /// <summary>
        /// Sets the dock ability.
        /// </summary>
        /// <param name="obj">The dependency object.</param>
        /// <param name="value">The value.</param>
        public static void SetDockAbility(DependencyObject obj, DockAbility value)
        {
            obj.SetValue(DockingManager.DockAbilityProperty, value);
        }

        /// <summary>
        /// Gets the host.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="state">The dock state.</param>
        /// <returns>return DockedElementTabbedHost value.</returns>
        public static DockedElementTabbedHost GetHost(FrameworkElement element, DockState state)
        {
            DockedElementTabbedHost host = null;

            DockInfoInternal info = DockingManager.GetDockInfo(element);

            if (info != null)
            {
                switch (state)
                {
                    case DockState.Dock:
                        host = info.HostDock;
                        break;

                    case DockState.Float:
                        host = info.HostFloat;
                        break;
                }
            }

            return host;
        }

        /// <summary>
        /// Gets the size of the host.
        /// </summary>
        /// <param name="host">The docked element tabbed host.</param>
        /// <returns>return host size.</returns>
        public static Size GetHostSize(DockedElementTabbedHost host)
        {
            return ((IDesiredSize)host).DesiredSize;
        }

        /// <summary>
        /// Sets the size of the host.
        /// </summary>
        /// <param name="host">The DockedElementTabbedHost.</param>
        /// <param name="size">The size of Dock element.</param>
        public static void SetHostSize(DockedElementTabbedHost host, Size size)
        {
            ((IDesiredSize)host).DesiredSize = size;
        }

        /// <summary>
        /// Gets the width of the host.
        /// </summary>
        /// <param name="host">The DockedElementTabbedHost.</param>
        /// <returns>return host width.</returns>
        public static double GetHostWidth(DockedElementTabbedHost host)
        {
            return ((IDesiredSize)host).DesiredSize.Width;
        }

        /// <summary>
        /// Gets the height of the host.
        /// </summary>
        /// <param name="host">The DockedElementTabbedHost.</param>
        /// <returns>return host height.</returns>
        public static double GetHostHeight(DockedElementTabbedHost host)
        {
            return ((IDesiredSize)host).DesiredSize.Height;
        }

        /// <summary>
        /// Sets the width of the host.
        /// </summary>
        /// <param name="host">The docked element tabbed host.</param>
        /// <param name="width">The width.</param>
        public static void SetHostWidth(DockedElementTabbedHost host, double width)
        {
            Size size = ((IDesiredSize)host).DesiredSize;
            size.Width = width;
            ((IDesiredSize)host).DesiredSize = size;
            DockingManager.InvalidateParentMeasure(host);
        }

        /// <summary>
        /// Sets the height of the host.
        /// </summary>
        /// <param name="host">The docked element tabbed host.</param>
        /// <param name="height">The height.</param>
        public static void SetHostHeight(DockedElementTabbedHost host, double height)
        {
            Size size = ((IDesiredSize)host).DesiredSize;
            size.Height = height;
            ((IDesiredSize)host).DesiredSize = size;
        }

        /// <summary>
        /// Copies the size of the window.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="sibling">The sibling.</param>
        /// <param name="state">The dock state.</param>
        internal static void CopyWindowSize(FrameworkElement element, FrameworkElement sibling, DockState state)
        {
            switch (state)
            {
                case DockState.Dock:
                    CopyDockWindowSize(element, sibling);
                    break;

                case DockState.Float:
                    CopyFloatWindowSize(element, sibling);
                    break;
            }
        }

        /// <summary>
        /// Copies the size of the window.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="siblings">The siblings.</param>
        /// <param name="state">The dock state.</param>
        internal static void CopyWindowSize(FrameworkElement element, List<FrameworkElement> siblings, DockState state)
        {
            foreach (FrameworkElement sibling in siblings)
            {
                CopyWindowSize(element, sibling, state);
                
            }
        }

        /// <summary>
        /// Raises the DockStateChanged event.
        /// </summary>
        /// <param name="element">Element that has changed the dock state.</param>
        /// <param name="e">The event data that describes the property that changed, as well as old and new values.</param>
        protected virtual void OnStatePropertyChanged(FrameworkElement element, DependencyPropertyChangedEventArgs e)
        {
            if (null != DockStateChanged)
            {
                DockStateEventArgs arg = new DockStateEventArgs((DockState)e.OldValue, (DockState)e.NewValue);
                DockStateChanged(element, arg);
            }            
        }

        /// <summary>
        /// Called when [is froze changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="arg">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnIsFrozeChanged(FrameworkElement sender, DependencyPropertyChangedEventArgs arg)
        {
            if (null != IsFrozeChanged)
            {
                IsFrozeChanged(sender, arg);
            }
        }

        /// <summary>
        /// Called when [is selected tab changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnIsSelectedTabChanged(FrameworkElement sender, DependencyPropertyChangedEventArgs args)
        {
            if (IsSelectedTabChanged != null)
            {
                IsSelectedTabChanged(sender,args);
            }
        }
        
        /// <summary>
        /// Called when [is active window changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnIsActiveWindowChanged(FrameworkElement sender,DependencyPropertyChangedEventArgs args)
        {
            if (IsActiveWindowChanged != null)
            {
                IsActiveWindowChanged(sender, args);
            }
        }


        /// <summary>
        /// Called when [dock window state property changed].
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnDockWindowStatePropertyChanged(FrameworkElement element, DependencyPropertyChangedEventArgs e)
        {
            if (null != DockWindowStateChanged)
            {
                DockWindowStateEventArgs arg = new DockWindowStateEventArgs((WindowState)e.OldValue, (WindowState)e.NewValue);
                DockWindowStateChanged(element, arg);
            }
        }

       //public FrameworkElement oldwindow;
        /// <summary>
        /// Raises the <see cref="E:ActiveWindowChanged"/> event.
        /// </summary>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
       protected virtual void OnActiveWindowChanged(DependencyPropertyChangedEventArgs args)
        {
            if (null != ActiveWindowChanged)
            {
                ActiveWindowChanged(this, args);
                RoutedEventArgs args1 = new RoutedEventArgs(DockingManager.WindowActivatedEvent, args.NewValue);
                RaiseEvent(args1);
                RoutedEventArgs args2 = new RoutedEventArgs(DockingManager.WindowDeactivatedEvent, args.OldValue);
                RaiseEvent(args2);
                //oldwindow = args.OldValue as FrameworkElement;                  
           }
            if ((args.OldValue as FrameworkElement) != null)
            {
                FrameworkElement element = args.OldValue as FrameworkElement;
                if (lastacitive.Count <= Children.Count)
                {
                    if (lastacitive.Count < 2)
                        lastacitive.Insert(0, element);
                    else if (lastacitive.Count >= 2)
                    {
                        int i;
                        bool same = false;
                        for (i = 0; i < lastacitive.Count; i++)
                        {
                            if (lastacitive[i] == element)
                            {
                                same = true;
                                break;
                            }
                        }
                        if (same)
                        {
                            lastacitive.RemoveAt(i);
                        }
                        lastacitive.Insert(0, element);
                    }
                }
                else if (lastacitive.Count > Children.Count)
                {
                    lastacitive.RemoveAt(Children.Count);
                    lastacitive.Insert(0, element);
                }
            }
            RoutedEventArgs arg = new RoutedEventArgs(DockingManager.TunnelActiveWindowChangedEvent, this);
            RaiseEvent(arg);
        }

        /// <summary>
        /// Participates in rendering operations that are directed by the layout system. 
        /// The rendering instructions for this element are not used directly when this method is invoked, 
        /// and are instead preserved for later asynchronous use by layout and drawing.
        /// </summary>
        /// <param name="drawingContext">The drawing instructions for a specific element. 
        /// This context is provided to the layout system.</param>
        protected override void OnRender(DrawingContext drawingContext)
        {
            if (Background != null)
            {
                Rect rectangleClient = new Rect(new Point(0, 0), RenderSize);
                drawingContext.DrawRectangle(Background, null, rectangleClient);
            }

            base.OnRender(drawingContext);
        }

        /// <summary>
        /// Invoked just before the <see cref="E:System.Windows.UIElement.IsKeyboardFocusWithinChanged"/> event is raised by this element. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        protected override void OnIsKeyboardFocusWithinChanged(DependencyPropertyChangedEventArgs e)
        {
            foreach (var window in this.m_WindowsRegistered)
            {
                if (!window.IsOpen && window.PrimaryElement != null)
                {
                    window.IsOpen = true;
                }
            }
            base.OnIsKeyboardFocusWithinChanged(e);
        }

        /// <summary>
        /// Finds element from given name.
        /// </summary>
        /// <param name="nameForSaerch">name for search</param>
        /// <returns>fined element</returns>
        private FrameworkElement FindChildSafe(string nameForSaerch)
        {
            FrameworkElement result = null;

            foreach (FrameworkElement element in Children)
            {
                if (nameForSaerch == element.Name)
                {
                    result = element;
                    break;
                }
            }

            return result;
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Gets an attached dock info of the specified element. It is
        /// created if it does not exist.
        /// </summary>
        /// <param name="element">Element to get dock info for.</param>
        /// <returns>
        /// Dock info associated with the element.
        /// </returns>
        private DockInfoInternal GetDockInfoInternal(UIElement element)
        {
            DockInfoInternal info = DockingManager.GetDockInfo(element);

            if (info == null)
            {
                info = new DockInfoInternal(this);
                DockingManager.SetDockInfo(element, info);
            }

            return info;
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Raises TargetNameInDockedModeChanged event with the
        /// specified arguments.
        /// </summary>
        /// <param name="sender">Specifies an object, the property
        /// value is associated with.</param>
        /// <param name="oldValue">Specifies an old value of the
        /// TargetNameInDockedMode property.</param>
        /// <param name="newValue">Specifies an new value of the
        /// TargetNameInDockedMode property.</param>
        private void RaiseTargetNameInDockedModeChangedEvent(UIElement sender, string oldValue, string newValue)
        {
            if (TargetNameInDockedModeChanged != null)
            {
                TargetNameInDockedModeChanged(sender, new DockTargetNameChangedEventArgs(oldValue, newValue));
            }
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Raises TargetNameInFloatingModeChanged event with the
        /// specified arguments.
        /// </summary>
        /// <param name="sender">Specifies an object, the property
        /// value is associated with.</param>
        /// <param name="oldValue">Specifies an old value of the
        /// TargetNameInFloatingMode property.</param>
        /// <param name="newValue">Specifies an new value of the
        /// TargetNameInFloatingMode property.</param>
        private void RaiseTargetNameInFloatingModeChangedEvent(UIElement sender, string oldValue, string newValue)
        {
            if (TargetNameInFloatingModeChanged != null)
            {
                TargetNameInFloatingModeChanged(sender, new DockTargetNameChangedEventArgs(oldValue, newValue));
            }
        }

        /// <summary>
        /// Imports the element.
        /// </summary>
        /// <param name="element">The element.</param>
        private void MouseMoveOnTargetManager(FrameworkElement element,DockedElementTabbedHost host,InputEventArgs e)
        {
            DockingManager m_previousmanager = DockingManager.GetDockingManager(element as DependencyObject);
            if(m_previousmanager!=null)
            {
                bool m_execute = false;
                ArrayList m_targetmanagers = m_previousmanager.TargetManagers;
                for (int i = 0; i < m_targetmanagers.Count; i++)
                {
                    if (m_targetmanagers[i].Equals(this))
                    {
                        m_execute = true;
                    }
                }

                if (m_execute)
                {
                    if (!this.Children.Contains(element))
                    {
                        this.IsDragging = true;
                        m_draggedElement = m_previousmanager.m_draggedElement;
                    }

                    if (host != null)
                    {
                        TransferManagerMouseMove(host, e);
                    }
                    else
                    {
                        TransferManagerMouseMove(this, e);
                    }
                }
            }
        }

        /// <summary>
        /// Transfers the manager mouse move.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        private void TransferManagerMouseMove(object sender, InputEventArgs e)
        {
            DockedElementTabbedHost host = e.Source as DockedElementTabbedHost;

            if (UseAdornerFloatWindow && host != null)
            {
                return;
            }

            host = host == null ? sender as DockedElementTabbedHost : host;

            if (host !=null && IsDragging && host.Visibility == Visibility.Visible && IsntSelf(host))
            {
                FrameworkElement hostedElement = host.HostedElement;
                //bool bDock = CanDock(hostedElement) && !GetNoDock(m_draggedElement);

                //Fixed for Cannot DockWindow Automation issue Id - 462
                bool bDock = DockingManager.GetCanDock(hostedElement) && !DockingManager.GetNoDock(m_draggedElement);

                if (bDock && !CanDockToTarget(m_draggedElement, host))
                {
                    bDock = false;
                }

                if (bDock)
                {
                    DirectTabPanel tabPanel = host.InternalTabPanel;
                    if (IsInTabPanel(tabPanel, e))
                    {
                        bool allowdocking = false;
                        DockSide elementside = DockingManager.GetSide(m_draggedElement, DockingManager.GetState(m_draggedElement));
                        DockAbility elementability = DockingManager.GetDockAbility(m_draggedElement);
                        if (elementside == DockSide.Top)
                        {
                            allowdocking = (elementability & DockAbility.Top) == DockAbility.Top;
                        }
                        else if (elementside == DockSide.Left)
                        {
                            allowdocking = (elementability & DockAbility.Left) == DockAbility.Left;
                        }
                        else if (elementside == DockSide.Right)
                        {
                            allowdocking = (elementability & DockAbility.Right) == DockAbility.Right;
                        }
                        else if (elementside == DockSide.Bottom)
                        {
                            allowdocking = (elementability & DockAbility.Bottom) == DockAbility.Bottom;
                        }
                        else if (elementside == DockSide.Tabbed)
                        {
                            allowdocking = (elementability & DockAbility.Tabbed) == DockAbility.Tabbed;
                        }
                        if (allowdocking || elementability == DockAbility.All)
                        {
                            if (!UseInteropCompatibilityMode)
                            {
                                if (DraggingType.NormalDragging == DraggingType)
                                {
                                    DockInfoInternal info = DockingManager.GetDockInfo(m_draggedElement);
                                    m_holdList = GetContainerTabs(info.FloatingWindow.FloatChild as DockedElementsContainer);
                                    info.FloatingWindow.IsOpen = false;

                                }
                                else
                                {
                                    CurrentDragPopup.IsOpen = false;
                                }

                                PreparePreviewElements(m_holdList);
                                DockState state = host.State;
                                int iTabOrder = GetEdgeTabOrder(host.TabChildren, state, false);

                                foreach (FrameworkElement tab in m_previewElements)
                                {
                                    DockingManager.SetState(tab, state);
                                    DockedElementTabbedHost.SetTabOrder(tab, state, ++iTabOrder);
                                    host.TabChildren.Add(tab);
                                }

                                m_bLoadPreview = true;
                                m_isTabPressed = true;
                                LockLayoutUpdate = true;
                                CompletePreview();
                                if (e is MouseEventArgs)
                                    tabPanel.CaptureMouse();
#if !SyncfusionFramework3_5
                                else if (e is TouchEventArgs)
                                    tabPanel.CaptureTouch((e as TouchEventArgs).TouchDevice);
#endif
                                m_prevHostUnderMouse = host;
                            }
                        }
                    }
                    else
                    {
                        DockingManager docking = DockingManager.ResolveManager(hostedElement);
                        
                        FireDockProviderShownEvent(host);

                        if (NeedPreview(host))
                        {
                            ProcessingCreateMainPreview(host);
                            m_prevHostUnderMouse = host;
                        }

                        m_hostUnderMouse = host;

                        ProcessingSetOnTopFloatWindow(m_draggedElement);
                        ProcessingCreateDockPreview(e);
                    }
                }
            }
            else if (host!=null && IsDragging && DraggingType.NormalDragging != DraggingType && !IsntSelf(host))
            {
                if (m_prevHostUnderMouse != null)
                {
                    FireDockProviderShownEvent(host);

                    m_hostUnderMouse = m_prevHostUnderMouse;
                    ProcessingCreateDockPreview(e);
                }
            }

            this.Focus();
            Keyboard.Focus(this);
            if (UseAdornerFloatWindow)
            {
                SearchHostUnderMouse(e);
            }

            bool b_ShouldHide = false;
            if (m_hostUnderMouse != null)
            {
                Point point = new Point(0, 0);
                if (e is MouseEventArgs)
                    point = (e as MouseEventArgs).GetPosition(m_hostUnderMouse);
#if !SyncfusionFramework3_5
                else if (e is TouchEventArgs)
                    point = ((e as TouchEventArgs).GetTouchPoint(m_hostUnderMouse) as TouchPoint).Position;
#endif
                DockPreviewRecord? record = m_managerDragPreview.FindDockingPlace(m_draggedElement, m_hostUnderMouse, point);
                b_ShouldHide = record == null;
            }

            if (b_ShouldHide)
            {
                m_managerDragPreview.HideDockPreview();
            }

        }

        /// <summary>
        /// Initializes the floating window coordinates of the element
        /// using current visible host.
        /// </summary>
        /// <param name="element">Element, the floating window
        /// coordinates should be set for.</param>
        /// <param name="state">The dock state.</param>
        /// <property name="flag" value="Finished"/>
        private static void InitFloatingWindowRect(FrameworkElement element, DockState state)
        {
            DockedElementTabbedHost host = DockingManager.GetHost(element, state);

            if (host != null && host.Visibility == Visibility.Visible)
            {
                PresentationSource presentationSource = PresentationSource.FromVisual(host);

                if (presentationSource != null)
                {
                    Rect rect = new Rect
                    {
                        Location = host.PointToScreen(new Point(0, 0))
                    };
                    Point point = host.PointToScreen(new Point(host.RenderSize.Width, host.RenderSize.Height));

                    double rectWidth = point.X - rect.Location.X;
                    double savedWidth = DockingManager.m_floatWindowRect.Width;
                    rect.Width = (rectWidth < savedWidth) ? savedWidth : rectWidth;

                    double rectHeight = point.Y - rect.Location.Y;
                    double savedHeight = DockingManager.m_floatWindowRect.Height;
                    rect.Height = (rectHeight < savedHeight) ? savedHeight : rectHeight;

                    if (BrowserInteropHelper.IsBrowserHosted)
                    {
                        Visual root = VisualUtils.FindRootVisual(host) as Window;

                        if (null == root)
                        {
                            DockingManager owner = DockingManager.ResolveManager(element);
                            root = VisualUtils.FindRootVisual(owner);
                        }

                        Point rootOffset = root.PointToScreen(new Point());
                        rect.X -= rootOffset.X;
                        rect.Y -= rootOffset.Y;
                    }

                    DockingManager.SetFloatingWindowRect(element, rect);
                }
            }
        }

        /// <summary>
        /// Corrects the floating window rect location.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="state">The dock state.</param>
        private static void CorrectFloatingWindowRectLocation(FrameworkElement element, DockState state)
        {
            DockedElementTabbedHost host = DockingManager.GetHost(element, state);

            if (host != null && host.Visibility == Visibility.Visible)
            {
                PresentationSource presentationSource = PresentationSource.FromVisual(host);

                if (presentationSource != null)
                {
                    Rect rect = DockingManager.GetFloatingWindowRect(element);
                    Point pt = Mouse.GetPosition(host);
                    pt = host.PointToScreen(pt);
                    double xOffset = 0, yOffset = 0;

                    if (BrowserInteropHelper.IsBrowserHosted)
                    {
                        Visual root = VisualUtils.FindRootVisual(host);

                        if (root is Window)
                        {
                            Point rootOffset = root.PointToScreen(new Point());
                            xOffset = rootOffset.X;
                            yOffset = rootOffset.Y;
                        }
                    }

                    if (pt.X < rect.Left || pt.X > rect.Right)
                    {
                        rect.X = pt.X - rect.Width / 2 - xOffset;
                    }

                    if (pt.Y < rect.Y || pt.Y > rect.Y)
                    {
                        rect.Y = pt.Y - 10 - yOffset;
                    }

                    DockingManager.SetFloatingWindowRect(element, rect);
                }
            }
        }

        /// <summary>
        /// Gets the host.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="state">The dock state.</param>
        /// <returns>return host.</returns>
        private static DockedElementTabbedHost GetHost(DependencyObject element, DockState state)
        {
            DockInfoInternal info = GetDockInfo(element);
            DockedElementTabbedHost host = null;

            if (info != null)
            {
                DockSide side = GetSide(element, state);

                if (side == DockSide.Tabbed)
                {
                    string target = GetTargetName(element, state);
                    FrameworkElement elementParent = info.DockingManager.FindChild(target);
                    if (elementParent != null)
                    {
                        DockInfoInternal infoParent = DockingManager.GetDockInfo(elementParent);
                        host = infoParent.GetHost(state) ?? info.GetHost(state);
                    }
                }
                else
                {
                    host = info.GetHost(state);
                }
            }

            return host;
        }

        /// <summary>
        /// Gets the new target name for tab group.
        /// </summary>
        /// <param name="notTabSiblings">The not tab siblings.</param>
        /// <param name="state">The state.</param>
        /// <returns>return name.</returns>
        private static string GetNewTargetNameForTabGroup(IList<FrameworkElement> notTabSiblings, DockState state)
        {
            string result = string.Empty;

            if (notTabSiblings.Count > 1)
            {
                updatedockflag = false;
                FrameworkElement element = notTabSiblings[0];
                DockingManager.SetTargetName(element, string.Empty, state);
                result = element.Name;
                updatedockflag = true;
            }

            return result;
        }

        /// <summary>
        /// Copies the size of the dock window.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="sibling">The sibling.</param>
        private static void CopyDockWindowSize(DependencyObject element, DependencyObject sibling)
        {
            double width = DockingManager.GetDesiredWidthInDockedMode(element);
            double height = DockingManager.GetDesiredHeightInDockedMode(element);
            DockingManager.SetDesiredWidthInDockedMode(sibling, width);
            DockingManager.SetDesiredHeightInDockedMode(sibling, height);
        }

        /// <summary>
        /// Copies the size of the float window.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="sibling">The sibling.</param>
        private static void CopyFloatWindowSize(DependencyObject element, DependencyObject sibling)
        {
            double width = DockingManager.GetDesiredWidthInFloatingMode(element);
            double height = DockingManager.GetDesiredHeightInFloatingMode(element);
            DockingManager.SetDesiredWidthInFloatingMode(sibling, width);
            DockingManager.SetDesiredHeightInFloatingMode(sibling, height);
        }

        /// <summary>
        /// Detaches from children async.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="state">The dock state.</param>
        private static void DetachFromChildrenAsync(FrameworkElement element, DockState state)
        {
            DetachFromChildren(element, state);
        }

        /// <summary>
        /// Detaches from children.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="state">The dock state.</param>
        private static void DetachFromChildren(FrameworkElement element, DockState state)
        {
            DockingManager docking = ResolveManager(element);

            if (docking != null)
            {
                docking.RemoveElementFromDockingTree(element, state, true);
            }
        }

        /// <summary>
        /// Called when [coerce ActiveWindow].
        /// </summary>
        /// <param name="element"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static object OnCoerceActiveWindow(DependencyObject element, object value)
        {
            if (!m_setactivewindow)
            {
                var owner = element as DockingManager;
                var active = value as FrameworkElement;
                if (owner != null && active != null)
                {
                    ActiveWindowChangingEventArgs args = new ActiveWindowChangingEventArgs();
                    args.OldValue = owner.ActiveWindow as FrameworkElement;
                    args.NewValue = active;
                    if (args.OldValue != args.NewValue)
                    {
                        owner.FireActiveWindowChanging(active, args);
                        if (!args.Cancel)
                        {
                            value = active;                            
                        }
                        else
                        {
                            value = args.OldValue;
                        }
                    }
                }
            }
            m_setactivewindow = false;
            return value;
        }
        /// <summary>
        /// Called when [coerce size].
        /// </summary>
        /// <param name="d">The DependencyObject.</param>
        /// <param name="value">The value.</param>
        /// <returns>return object.</returns>
        private static object OnCoerceSize(DependencyObject d, object value)
        {
            if ((double)value < 1)
            {
                return 1d;
            }
            else
            {
                return value;
            }
        }

        /// <summary>
        /// Called when [coerce MinWidth size].
        /// </summary>
        /// <param name="d">The DependencyObject.</param>
        /// <param name="value">The value.</param>
        /// <returns>return object.</returns>
        private static object OnCoerceMinWidthSize(DependencyObject d, object value)
        {
            double WidhtInFloat = DockingManager.GetDesiredWidthInFloatingMode(d);

            if (WidhtInFloat > (double)value)
            {
                d.SetValue(DesiredWidthInFloatingModeProperty, value);
            }
            MinWidhtInFloat = (double)value;
            return value;
        }

        /// <summary>
        /// Called when [coerce MinHeight size].
        /// </summary>
        /// <param name="d">The DependencyObject.</param>
        /// <param name="value">The value.</param>
        /// <returns>return object.</returns>
        private static object OnCoerceMinHeightSize(DependencyObject d, object value)
        {
            double HeightInFloat = DockingManager.GetDesiredHeightInFloatingMode(d);

            if (HeightInFloat > (double)value)
            {
                d.SetValue(DesiredHeightInFloatingModeProperty, value);
            }
            MinHeightInFloat = (double)value;
            return value;
        }

        /// <summary>
        /// Called when [coerce MaxWidth size].
        /// </summary>
        /// <param name="d">The DependencyObject.</param>
        /// <param name="value">The value.</param>
        /// <returns>return object.</returns>
        private static object OnCoerceMaxWidthSize(DependencyObject d, object value)
        {
            double maxWidthInFloat = DockingManager.GetDesiredWidthInFloatingMode(d);

            if (maxWidthInFloat > (double)value)
            {
                d.SetValue(DesiredWidthInFloatingModeProperty, value);
            }

            MaxWidhtInFloat = (double)value;
            return value;
        }

        /// <summary>
        /// Called when [coerce MaxHeight size].
        /// </summary>
        /// <param name="d">The DependencyObject.</param>
        /// <param name="value">The value.</param>
        /// <returns>return object.</returns>
        private static object OnCoerceMaxHeightSize(DependencyObject d, object value)
        {
            double maxHeightInFloat = DockingManager.GetDesiredHeightInFloatingMode(d);

            if (maxHeightInFloat > (double)value)
            {
                d.SetValue(DesiredHeightInFloatingModeProperty, value);
            }

            MaxHeightInFloat = (double)value;

            return value;
        }

        /// <summary>
        /// Called when [coerce MinWidthDocked size].
        /// </summary>
        /// <param name="d">The DependencyObject.</param>
        /// <param name="value">The value.</param>
        /// <returns>return object.</returns>
        private static object OnCoerceMinWidthDockedSize(DependencyObject d, object value)
        {
            double minWidthinDocked = DockingManager.GetDesiredWidthInDockedMode(d);

            if (minWidthinDocked > (double)value)
            {
                d.SetValue(DesiredWidthInDockedModeProperty, value);
            }

            MinWidhtInDocked = (double)value;
            return value;
        }

        /// <summary>
        /// Called when [coerce MinHeightDocked size].
        /// </summary>
        /// <param name="d">The DependencyObject.</param>
        /// <param name="value">The value.</param>
        /// <returns>return object.</returns>
        private static object OnCoerceMinHeightDockedSize(DependencyObject d, object value)
        {
            double minHeightinDocked = DockingManager.GetDesiredHeightInDockedMode(d);

            if (minHeightinDocked > (double)value)
            {
                d.SetValue(DesiredHeightInDockedModeProperty, value);
            }

            MinHeightInDocked = (double)value;
            return value;
        }

        /// <summary>
        /// Called when [coerce MaxWidthDocked size].
        /// </summary>
        /// <param name="d">The DependencyObject.</param>
        /// <param name="value">The value.</param>
        /// <returns>return object.</returns>
        private static object OnCoerceMaxWidthDockedSize(DependencyObject d, object value)
        {
            double maxWidthinDocked = DockingManager.GetDesiredWidthInDockedMode(d);

            if (maxWidthinDocked > (double)value)
            {
                d.SetValue(DesiredWidthInDockedModeProperty, value);
            }

            MaxWidhtInDocked = (double)value;
            return value;
        }

        /// <summary>
        /// Called when [coerce MaxHeightDocked size].
        /// </summary>
        /// <param name="d">The DependencyObject.</param>
        /// <param name="value">The value.</param>
        /// <returns>return object.</returns>
        private static object OnCoerceMaxHeightDockedSize(DependencyObject d, object value)
        {
            double maxHeightinDocked = DockingManager.GetDesiredHeightInDockedMode(d);

            if (maxHeightinDocked > (double)value)
            {
                d.SetValue(DesiredHeightInDockedModeProperty, value);
            }

            MaxHeightInDocked = (double)value;
            return value;
        }

        /// <summary>
        /// Called when [coerce side in docked mode].
        /// </summary>
        /// <param name="d">The DependencyObject.</param>
        /// <param name="value">The value.</param>
        /// <returns>return DockSide.</returns>
        private static object OnCoerceSideInDockedMode(DependencyObject d, object value)
        {
            DockSide dockSideValue = (DockSide)value;

            if (dockSideValue == DockSide.Tabbed
                && string.IsNullOrEmpty(DockingManager.GetTargetNameInDockedMode(d)))
            {
                return DockSide.Left;
            }

            return value;
        }

        /// <summary>
        /// Called when [coerce side in float mode].
        /// </summary>
        /// <param name="d">The DependencyObject.</param>
        /// <param name="value">The value.</param>
        /// <returns>return DockSide value.</returns>
        private static object OnCoerceSideInFloatMode(DependencyObject d, object value)
        {
            bool noDock = DockingManager.GetNoDock(d);
            DockSide dockSideValue = (DockSide)value;
            string targetNameInFloatingMode = GetTargetNameInFloatingMode(d);

            if ((noDock && dockSideValue == DockSide.Tabbed)
                || string.IsNullOrEmpty(targetNameInFloatingMode))
            {
                return DockSide.Left;
            }

            return value;
        }

        /// <summary>
        /// Called when [coerce target name in floating mode].
        /// </summary>
        /// <param name="d">The DependencyObject.</param>
        /// <param name="value">The value.</param>
        /// <returns>return object.</returns>
        private static object OnCoerceTargetNameInFloatingMode(DependencyObject d, object value)
        {
            object returnValue = value;
            string strValue = value.ToString();
            FrameworkElement element = (FrameworkElement)d;

            if (element.Name == strValue)
            {
                returnValue = string.Empty;
            }
            else
            {
                bool isNoSetValue = true;

                if (!string.IsNullOrEmpty(strValue))
                {
                    DockingManager manager = ResolveManager(element);

                    if (manager != null)
                    {
                        UIElement target = manager.FindChild(strValue);
                        if (target != null)
                        {
                            bool noDockTarget = DockingManager.GetNoDock(target);

                            if (noDockTarget)
                            {
                                returnValue = string.Empty;
                                isNoSetValue = false;
                            }
                        }
                    }
                }

                if (isNoSetValue)
                {
                    bool noDock = DockingManager.GetNoDock(d);

                    if (noDock)
                    {
                        returnValue = string.Empty;
                    }
                }
            }

            return returnValue;
        }

        /// <summary>
        /// Coerces the state.
        /// </summary>
        /// <param name="d">The DependencyObject.</param>
        /// <param name="value">The value.</param>
        /// <returns>return DockState.</returns>
        private static object CoerceState(DependencyObject d, object value)
        {
            DockState returnValue = (DockState)value;

            DockingManager manager=DockingManager.ResolveManager((UIElement)d);

            bool noDock = DockingManager.GetNoDock(d);
            bool canDrag = DockingManager.GetCanDrag(d);
            bool canDock = DockingManager.GetCanDock(d);
            bool canFloat = DockingManager.GetCanFloat(d);
            bool canClose = DockingManager.GetCanClose(d);
            bool canAutohide = DockingManager.GetCanAutoHide(d);

            DockState newState = returnValue;

            if (noDock && !canDrag && DockState.Dock == newState && canFloat)
            {
                returnValue = DockState.Float;
            }

            if (returnValue == DockState.Document && IsInitializedManager(d))
            {
                DockingManager docking = DockingManager.GetDockInfo(d).DockingManager;
                //returnValue = docking.UseDocumentContainer ? returnValue : DockState.Hidden;
            }

            if ((DockState)value==DockState.Float && manager !=null && !manager.CanNestedFloat && manager.HasFloatingParent(d as FrameworkElement))
            {
                returnValue = DockingManager.GetState(d);
            }

            if (!canDock && newState == DockState.Dock ||
                !canFloat && newState == DockState.Float ||
                !canClose && newState == DockState.Hidden ||
                !canAutohide && newState == DockState.AutoHidden)
            {
                if (IsInitializedManager(d) && manager !=null && manager.CanNestedFloat)
                {
                    throw new InvalidOperationException(string.Format("{0} state is not allowed", newState));
                }
            }

            return returnValue;
        }

        /// <summary>
        /// Coerces the state of the dock window.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        private static object CoerceDockWindowState(DependencyObject d, object value)
        {
            WindowState returnValue = (WindowState)value;

            return returnValue;
        }

        /// <summary>
        /// Coerces the can dock.
        /// </summary>
        /// <param name="d">The DependencyObject.</param>
        /// <param name="value">The value.</param>
        /// <returns>return bool value.</returns>
        private static object CoerceCanDock(DependencyObject d, object value)
        {
            bool returnValue = (bool)value;
            DockState state = GetState(d);
            DockingManager manager=DockingManager.ResolveManager(d as UIElement);

            if (state == DockState.Dock && !returnValue)
            {
                if (IsInitializedManager(d) && manager !=null && manager.CanNestedFloat)
                {
                    throw new InvalidOperationException(string.Format(C_STRCanStateExceptionMessage, "CanDock", "Dock"));
                }
            }

            return returnValue;
        }

        /// <summary>
        /// Coerces the can maximize.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        private static object CoerceCanMaximize(DependencyObject d, object value)
        {
            bool returnValue = (bool)value;
            WindowState state = GetDockWindowState(d);

            if (state == WindowState.Maximized && !returnValue)
            {
                if (IsInitializedManager(d))
                {
                    throw new InvalidOperationException(string.Format(C_STRCanStateExceptionMessage, "CanMaximize", "Maximize"));
                }
            }

            return returnValue;
        }

        /// <summary>
        /// Coerces the can minimize.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        private static object CoerceCanMinimize(DependencyObject d, object value)
        {
            bool returnValue = (bool)value;
            WindowState state = GetDockWindowState(d);

            if (state == WindowState.Minimized && !returnValue)
            {
                if (IsInitializedManager(d))
                {
                    throw new InvalidOperationException(string.Format(C_STRCanStateExceptionMessage, "CanMinimize", "Minimize"));
                }
            }

            return returnValue;
        }

        /// <summary>
        /// Coerces the can float.
        /// </summary>
        /// <param name="d">The DependencyObject.</param>
        /// <param name="value">The value.</param>
        /// <returns>return bool value.</returns>
        private static object CoerceCanFloat(DependencyObject d, object value)
        {
            bool returnValue = (bool)value;
            DockState state = GetState(d);
            DockingManager docking = DockingManager.ResolveManager(d as UIElement);

            if (state == DockState.Float && !returnValue)
            {
                if (IsInitializedManager(d) && docking !=null && docking.CanNestedFloat)
                {
                    throw new InvalidOperationException(string.Format(C_STRCanStateExceptionMessage, "CanFloat", "Float"));
                }
            }

            return returnValue;
        }

        /// <summary>
        /// Coerces the can document.
        /// </summary>
        /// <param name="d">The DependencyObject.</param>
        /// <param name="value">The value.</param>
        /// <returns>return bool value.</returns>
        private static object CoerceCanDocument(DependencyObject d, object value)
        {
            bool returnValue = (bool)value;
            DockState state = GetState(d);
            DockingManager docking = DockingManager.ResolveManager(d as UIElement);

            if (state == DockState.Document && !returnValue)
            {
                if (IsInitializedManager(d) && docking !=null && docking.CanNestedFloat)
                {
                    throw new InvalidOperationException(string.Format(C_STRCanStateExceptionMessage, "CanDocument", "Document"));
                }
            }

            if (IsInitializedManager(d))
            {
                DockingManager manager = DockingManager.GetDockInfo(d).DockingManager;
                returnValue &= manager.UseDocumentContainer;
            }

            return returnValue;
        }

        /// <summary>
        /// Coerces the can close.
        /// </summary>
        /// <param name="d">The DependencyObject.</param>
        /// <param name="value">The value.</param>
        /// <returns>return bool value.</returns>
        private static object CoerceCanClose(DependencyObject d, object value)
        {
            bool returnValue = (bool)value;
            DockState state = GetState(d);
            DockingManager docking = DockingManager.ResolveManager(d as UIElement);

            if (state == DockState.Hidden && !returnValue)
            {
                if (IsInitializedManager(d) && docking !=null && docking.CanNestedFloat)
                {
                    throw new InvalidOperationException(string.Format(C_STRCanStateExceptionMessage, "CanClose", "Hidden"));
                }
            }

            return returnValue;
        }

        /// <summary>
        /// Coerces the can auto hide.
        /// </summary>
        /// <param name="d">The dependency object.</param>
        /// <param name="value">The value.</param>
        /// <returns>return bool value.</returns>
        private static object CoerceCanAutoHide(DependencyObject d, object value)
        {
            bool returnValue = (bool)value;
            DockState state = GetState(d);
            if (state == DockState.AutoHidden && !returnValue)
            {
                if (IsInitializedManager(d))
                {
                    throw new InvalidOperationException(string.Format(C_STRCanStateExceptionMessage, "CanAutoHide", "Autohidden"));
                }
            }

            return returnValue;
        }

        /// <summary>
        /// Coerces the no dock.
        /// </summary>
        /// <param name="d">The DependencyObject.</param>
        /// <param name="value">The value.</param>
        /// <returns>return bool value.</returns>
        private static object CoerceNoDock(DependencyObject d, object value)
        {
            bool returnValue = (bool)value;
            DockState state = GetState(d);
            DockingManager docking = DockingManager.ResolveManager(d as UIElement);

            if (state == DockState.Dock && returnValue)
            {
                if (IsInitializedManager(d) && docking !=null && docking.CanNestedFloat)
                {
                    throw new InvalidOperationException(string.Format(C_STRCanStateExceptionMessage, "NoDock", "Dock"));
                }
            }

            return returnValue;
        }

        /// <summary>
        /// Coerces the no dock.
        /// </summary>
        /// <param name="d">The DependencyObject.</param>
        /// <param name="value">The value.</param>
        /// <returns>return bool value.</returns>
        private static object CoerceNoHeader(DependencyObject d, object value)
        {
            bool returnValue = (bool)value;
            return returnValue;
        }

        /// <summary>
        /// Coerces the floating window rect.
        /// </summary>
        /// <param name="d">The DependencyObject.</param>
        /// <param name="baseValue">The base value.</param>
        /// <returns>return Rect value of Float Window.</returns>
        private static object CoerceFloatingWindowRect(DependencyObject d, object baseValue)
        {
            Rect baseRect = (Rect)baseValue;
            DockingManager owner = DockingManager.ResolveManager(d as UIElement);
            if (owner == null || (owner != null && !owner.UseNativeFloatWindow))
            {
                if (baseRect.Width == 0 || baseRect.Height == 0)
                {
                    baseRect = Rect.Empty;
                }

                if (!baseRect.IsEmpty && MinFloatWindowHeight > baseRect.Height)
                {
                    baseRect.Height = MinFloatWindowHeight;
                }

                double screenwidth = SystemParameters.PrimaryScreenWidth * 0.87;
                double screenheight = SystemParameters.PrimaryScreenHeight * 0.75;

                if (owner != null && !owner.IsDragging && !owner.UseNativeFloatWindow)
                {
                    foreach (System.Windows.Forms.Screen screen in System.Windows.Forms.Screen.AllScreens)
                    {
                        bool canexecute = baseRect.X > 0 ? baseRect.X + screenwidth > SystemParameters.PrimaryScreenWidth : baseRect.X + screenwidth < SystemParameters.PrimaryScreenWidth;
                        if (!screen.Primary && canexecute)
                        {
                            if (screen.Bounds.Width < System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width)
                                screenwidth = screen.WorkingArea.Width * 0.87;
                            if (screen.Bounds.Height < System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height)
                                screenheight = screen.WorkingArea.Height * 0.75;
                        }
                    }
                }

                if (baseRect.Width > screenwidth)
                {
                    baseRect.Width = screenwidth;
                }
                if (baseRect.Height > screenheight)
                {
                    baseRect.Height = screenheight;
                }

                if (owner != null && owner.EnableBoundaryDeduction && !owner.UseNativeFloatWindow)
                {
                    baseRect = owner.CheckRectBoundaryDetection(baseRect);
                }
                //// TODO: Implement DPI-dependent calculations
                ////baseRect = ScreenUtils.FixByScreenBounds( baseRect );
            }
            //// TODO: Implement DPI-dependent calculations
            ////baseRect = ScreenUtils.FixByScreenBounds( baseRect );

            return baseRect;
        }

        /// <summary>
        /// Called when [desired size in docked mode changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnDesiredSizeInDockedModeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)sender;
            DockedElementTabbedHost host = DockingManager.ResolveHostDock(element);
            DockingManager manager = DockingManager.ResolveManager(element as UIElement);
            DockSide side = DockingManager.GetSideInDockedMode(element as DependencyObject);

            if (host != null && side != DockSide.Tabbed)
            {
                IChildrenResize resize = host as IChildrenResize;
                if (!double.IsNaN((double)e.NewValue) && host.m_desiredSize != Size.Empty)
                {
                    if (e.Property.Name == "DesiredHeightInDockedMode")
                    {
                        resize.SetHeight((double)e.NewValue);
                    }
                    else
                    {
                        resize.SetWidth((double)e.NewValue);
                    }
                }
            }

            if (manager==null)
            {
                if (host != null)
                {
                    if (host.ActualHeight > 0 && DockingManager.CheckFixedsize(element, Orientation.Vertical))
                        DockingManager.SetFixedHeight(element as DependencyObject, host.ActualHeight);

                    if (host.ActualWidth > 0 && DockingManager.CheckFixedsize(element, Orientation.Horizontal))
                        DockingManager.SetFixedWidth(element as DependencyObject, host.ActualWidth);
                }
                else
                {
                    if (DockingManager.CheckFixedsize(element, Orientation.Vertical))
                        DockingManager.SetFixedHeight(element as DependencyObject, DockingManager.GetDesiredHeightInDockedMode(element as DependencyObject));

                    if (DockingManager.CheckFixedsize(element, Orientation.Horizontal))
                        DockingManager.SetFixedWidth(element as DependencyObject, DockingManager.GetDesiredWidthInDockedMode(element as DependencyObject));
                }
            }

            DockingManager.InvalidateParentMeasure(host);
            DockingManager.CorrectTabsSize(element);
        }

        /// <summary>
        /// Called when [desired size in floating mode changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnDesiredSizeInFloatingModeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)sender;
            if (double.IsNaN((double)e.NewValue) || double.IsInfinity((double)e.NewValue))
            {
                if (e.Property.Name == "DesiredHeightInFloatingMode")
                {
                    DockingManager.SetDesiredHeightInFloatingMode(element, (double)e.OldValue);
                }
                else
                {
                    DockingManager.SetDesiredWidthInFloatingMode(element, (double)e.OldValue);
                }
            }
            DockedElementTabbedHost host = DockingManager.ResolveHostFloat(element);
            DockingManager.InvalidateParentMeasure(host);
            DockingManager.CorrectTabsSize(element);
        }

        /// <summary>
        /// Called when [desired MinWidth in floating mode changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnDesiredMinWidthInFloatingModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }

        /// <summary>
        /// Called when [desired MinHeight in floating mode changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnDesiredMinHeightInFloatingModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }

        /// <summary>
        /// Called when [desired MaxWidth in floating mode changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnDesiredMaxWidthInFloatingModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }

        /// <summary>
        /// Called when [desired MaxHeight in floating mode changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnDesiredMaxHeightInFloatingModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }

        /// <summary>
        /// Called when [desired MaxWidth in Docked mode changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnDesiredMaxWidthInDockedModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }

        /// <summary>
        /// Called when [desired MaxHeight in Docked mode changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnDesiredMaxHeightInDockedModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }

        /// <summary>
        /// Called when [desired MinWidth in Docked mode changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnDesiredMinWidthInDockedModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }

        /// <summary>
        /// Called when [desired MinHeight in Docked mode changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnDesiredMinHeightInDockedModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }

        /// <summary>
        /// Called when [float window size changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnFloatWindowSizeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            Rect rect = DockingManager.GetFloatingWindowRect(sender);
            if (!rect.IsEmpty)
            {
                if (e.Property.Name == "FloatWindowWidth")
                {
                    rect.Width = (double)e.NewValue;
                }
                else if (e.Property.Name == "FloatWindowHeight")
                {
                    rect.Height = (double)e.NewValue;
                }
                else
                {
                    rect.Size = (Size)e.NewValue;
                }
            }
            DockingManager.SetFloatingWindowRect(sender, rect);
        }
        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Calls OnTargetNameInFloatingModeChanged method of the
        /// instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnTargetNameInFloatingModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)d;
            if (!updatedockflag)
            {
                makedockflag = false;
            }
            element.CoerceValue(DockingManager.SideInFloatModeProperty);
            DockingManager manager = ResolveManager(element);
            string newName = e.NewValue.ToString();

            if (manager != null)
            {
                string oldName = e.OldValue.ToString();
                manager.RaiseTargetNameInFloatingModeChangedEvent(element, oldName, newName);
            }

            DockState state = GetState(element);

            if (state == DockState.Float && string.IsNullOrEmpty(newName) &&
                !manager.LockPropertyChangedAction)
            {
                if (DockingManager.GetTabControl(element) != null)
                {
                    if ((DockingManager.GetTabControl(element)).Items.Count <= 1)
                    {
                        InitFloatingWindowRect(element, DockState.Float);
                    }
                }
                else
                {
                    InitFloatingWindowRect(element, DockState.Float);
                }
            }
            
            ////UpdateLayout( element );
        }

        /// <summary>
        /// Called when [previous target in dock mode changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnPreviousTargetInDockModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)d;
            if (!string.IsNullOrEmpty(e.OldValue as string))
                DockingManager.SetIsSwapped(element, true);
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Calls OnTargetNameInDockedModeChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnTargetNameInDockedModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UIElement element = (UIElement)d;
            if (!updatedockflag)
            {
                makedockflag = false;
            }
            element.CoerceValue(DockingManager.SideInDockedModeProperty);
            DockingManager manager = ResolveManager(element);
            ///This below code has been added for MT2257, 91433, Mt2246, MT2269, Mt2270, 92396 - Component Swapping issue  
            if (!e.OldValue.Equals(e.NewValue) && manager != null)
            {
                string targetName = e.OldValue as string;
                FrameworkElement targetelement = manager.FindChild(targetName);
                if (targetelement != null)
                {
                    List<string> innerDockElements = manager.InnerDockElements[targetName] as List<string>;
                    if (innerDockElements != null && innerDockElements.Contains((element as FrameworkElement).Name))
                    {
                        innerDockElements.Remove((element as FrameworkElement).Name);
                        manager.InnerDockElements[targetName] = innerDockElements;
                    }
                }
                else if ((e.NewValue as string).Equals((element as FrameworkElement).Name))
                    DockingManager.SetTargetNameInDockedMode(d, string.Empty);
            }
            FrameworkElement elementFrame = d as FrameworkElement;
            if (manager != null)
            {
                manager.RaiseTargetNameInDockedModeChangedEvent(element, e.OldValue.ToString(), e.NewValue.ToString());
            }

            ////UpdateLayout( element );
        }

        private static void OnIsContextMenuButtonVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement element = d as FrameworkElement;
            if (element != null)
            {
                DockHeaderPresenter presenter = DockingManager.GetDockHeaderPresenter(element);
                if (presenter != null)
                {
                    ToggleButton menubutton = presenter.GetMenuButton();
                    if (menubutton != null)
                    {
                        if (!(bool)e.NewValue)
                        {
                            menubutton.Visibility = Visibility.Collapsed;

                        }
                        else
                        {
                            menubutton.Visibility = Visibility.Visible;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Called when [dock window state changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnDockWindowStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)d;
            DockingManager owner = DockingManager.ResolveManager(element);
            if (e.NewValue != null && CanChangeWindowState(element, (WindowState)e.NewValue) && owner!=null && !owner.m_loadingState)
            {
                switch ((WindowState)e.NewValue)
                {
                    case WindowState.Maximized:
                        if (!owner.m_executingmaximizeflag)
                        {
                            owner.ExecuteMaximize(element);
                        }
                        if (!owner.m_maximizedelements.Contains(element))
                        {
                            owner.m_maximizedelements.Add(element);
                        }
                        break;
                    case WindowState.Minimized:
                        if (!owner.m_exceutingminimizeflag)
                        {
                            owner.ExecuteMinimize(element);
                        }
                        break;
                    case WindowState.Normal:
                        if (e.OldValue.Equals(WindowState.Maximized) && !owner.m_executingrestoreflag)
                        {
                            if(owner !=null && element !=null)
                                owner.ExecuteRestore(element);
                        }
                        if (owner.m_maximizedelements.Contains(element))
                        {
                            owner.m_maximizedelements.Remove(element);
                        }
                        break;
                }
                if (element != null)
                {
                    RoutedEventArgs args = new RoutedEventArgs(DockWindowStateChangedEvent, element);
                    element.RaiseEvent(args);
                    if (owner != null)
                    {
                        owner.OnDockWindowStatePropertyChanged(element, e);
                    }
                }
            }
        }

        internal FrameworkElement m_lockedelement=null;       

        /// <summary>
        /// Called when [state property changed].
        /// </summary>
        /// <param name="d">The DependencyObject.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnStatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)d;
            DockingManager owner = DockingManager.ResolveManager(element);
            DockInfoInternal info = DockingManager.GetDockInfo(element);
            //if (info != null && !IsVisibleState((DockState)e.NewValue))
            //{
            //    DockedElementTabbedHost host = DockingManager.GetTabbedHost(element, (DockState)e.NewValue);
            //    if (host == null && owner != null)
            //        owner.AddHostToDocking(element, DockState.Dock);
            //}
            bool m_canexecutechange = true;
            if (owner!=null && !owner.m_IsStateChangingChecked)
            {
                DockStateChangingEventArgs args = new DockStateChangingEventArgs();
                owner.hasChild = false;
                args.SourceElement = element;
                args.PresentState = (DockState)e.OldValue;
                args.TargetState = (DockState)e.NewValue;
                args.TargetSide = DockingManager.GetSideInDockedMode(element);
                owner.FireDockStateChanging(element, args);
                if (args.Cancel)
                {
                    m_canexecutechange = false;
                }
            }
            else if (owner != null)
            {
                owner.m_IsStateChangingChecked = false;
            }
            if (m_canexecutechange)
            {
                try
                {
                    d.CoerceValue(CanDockProperty);
                    d.CoerceValue(CanFloatProperty);
                    d.CoerceValue(CanCloseProperty);
                    d.CoerceValue(CanAutoHideProperty);

                    DockState newState = (DockState)e.NewValue;
                    DockState oldState = (DockState)e.OldValue;

                    // SD 12536 - This loop will iterate the childrens to find the maximized component. It it finds the maximized component just will restore the 
                    // element before to change the state.

                    if (owner != null)
                    {
                        foreach (FrameworkElement child in owner.Children)
                        {
                            if (owner.MaximizeMode == MaximizeMode.FullScreen && DockingManager.GetDockWindowState(child) == WindowState.Maximized)
                            {
                                owner.m_lockedelement = element;

                                owner.RestoreFullScreenMode(child);
                                DockingManager.SetDockWindowState(child as DependencyObject, WindowState.Normal);

                                if (owner.MaximizeMode == MaximizeMode.FullScreen)
                                {
                                    

                                    if (!owner.CheckMaximizeButtonMode(child) && (DockingManager.GetDockWindowState(child) == WindowState.Normal))
                                    {
                                        DockingManager.SetMaximizeButtonVisibility(child, Visibility.Visible);
                                    }
                                    if (owner.MinimizeButtonEnabled && DockingManager.GetCanMinimize(child))
                                    {
                                        DockingManager.SetMinimizeButtonVisibility(child, Visibility.Visible);
                                    }
                                }
                            }
                            if (owner.MaximizeButtonEnabled && DockingManager.GetCanMaximize(child))
                            {
                                if (DockingManager.GetDockWindowState(child) == WindowState.Normal)
                                    DockingManager.SetMaximizeButtonVisibility(child, Visibility.Visible);
                                else if (DockingManager.GetDockWindowState(child) == WindowState.Maximized)
                                    DockingManager.SetRestoreButtonVisibility(child, Visibility.Visible);
                            }

                        }
                    }
                    if (oldState == DockState.Document && newState == DockState.Float)
                    {
                        TabControlExt tab = DockingManager.GetTabControl(element as DependencyObject);
                        if (tab != null)
                        {
                            DocumentTabControl tabcontrol = tab as DocumentTabControl;
                            if (tabcontrol != null)
                            {
                                if (tabcontrol.TabPositionCache.Contains(element as UIElement))
                                {
                                    DockingManager.SetDocumentTabOrderIndex(element, tabcontrol.TabPositionCache.IndexOf(element));
                                }
                            }
                            tab.IsTabGroupFocus = false;

                            tab.IsTabGroupFocus = false;
                            if (tab.SelectedItem != null)
                            {
                                (tab.SelectedItem as TabItemExt).IsTabGroupFocus = false;
                            }
                        }
                    }
                    DockingManager.SetPreviousState(element, oldState);
                    if (owner!=null &&owner.UseNativeFloatWindow)
                    {
                        DockingManager.ValidateFloatingWindowRect(element, newState);
                    }
                    else
                    {
                        DockingManager.ValidateFloatingWindowRect(element, newState);
                    }
                    if (oldState == DockState.Document && newState == DockState.Dock && DockingManager.GetDockHost(element) != null)
                    {
                        Size size = ((IDesiredSize)owner.GetDocumentContainerHost()).DesiredSize;
                        DockingManager.GetDockHost(element).m_desiredSize = size;
                    }

                    if (null != owner)
                    {
                        if (!owner.LockPropertyChangedAction)
                        {
                            owner.m_onStatechange = true;
                            if (((owner.MaximizeMode == MaximizeMode.FullScreen) && (owner.m_lockedelement == null || (owner.m_lockedelement != null && owner.m_lockedelement != element))) || (owner.MaximizeMode == MaximizeMode.Default))
                            {
                                DockingManager.ChangeState(element, newState);
                                owner.m_lockedelement = null;
                            }
                            owner.m_onStatechange = false;
                        }



                        if (newState == DockState.Hidden)
                        {
                            owner.OnElementHidden(element);
                        }
                        else if (oldState == DockState.Hidden)
                        {
                            owner.OnElementShown(element);
                        }

                        if (owner.DockFill && (newState == DockState.Document || oldState == DockState.Document || newState == DockState.Dock || oldState == DockState.Dock))
                        {
                            owner.ActivateDockFill();
                            owner.CheckDockFillProperty();
                        }

                        if (oldState == DockState.Float && newState == DockState.Hidden)
                        {
                            if (owner.UseNativeFloatWindow)
                            {
                                NativeFloatWindow window = GetNativeWindow(element);
                                if (window != null && !owner.NativeFloatWindowHasChild(window))
                                {
                                    window.IsOpen = false;
                                    owner.ValidateNativeFloatWindow(window);
                                    if (!owner.IsExecuteClose)
                                    {
                                        window.Close();
                                    }
                                    else
                                        owner.IsExecuteClose = false;
                                }
                            }
                            else
                            {
                                IWindow window = GetFloatWindow(element);                                
                                if (window != null && !owner.FloatWindowHasChild(window))
                                {
                                    window.IsOpen = false;                                    
                                }
                            }
                        }

                        if (oldState == DockState.Float)
                        {
                            if (owner!=null && owner.UseNativeFloatWindow)
                            {
                                if ((DockingManager.GetNativeWindow(element)) != null)
                                {
                                    if (DockingManager.ResolveManager(element as UIElement) != null)
                                    {
                                        if (DockingManager.GetNativeWindow(element).IsOpen && !owner.NativeFloatWindowHasChild(DockingManager.GetNativeWindow(element)))
                                        {
                                            DockingManager.GetNativeWindow(element).IsOpen = false;  
                                            DockingManager.ResolveManager(element as UIElement).ValidateNativeFloatWindow(DockingManager.GetNativeWindow(element));
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if ((DockingManager.GetFloatWindow(element)) != null)
                                {
                                    if (DockingManager.ResolveManager(element as UIElement) != null)
                                    {
                                        if (DockingManager.GetFloatWindow(element).IsOpen && !owner.FloatWindowHasChild(DockingManager.GetFloatWindow(element)))
                                        {
                                            DockingManager.GetFloatWindow(element).IsOpen = false;
                                        }
                                            DockingManager.ResolveManager(element as UIElement).RemoveWindow(DockingManager.GetFloatWindow(element));                                        
                                    }
                                }
                            }
                        }
                    }
                    if (owner != null && oldState==DockState.Float && (newState == DockState.Dock || newState == DockState.Document))
                    {
                        if (owner.m_hostUnderMouse == null)
                        {
                            owner.RearrangeHostElements(owner.RootContainer, element);
                        }
                    }
                    if (newState == DockState.Float)
                    {
                        DockingManager.SetElementFlag(element, false);
                    }
                    if (newState != DockState.Float && element != null && element is DockingManager)
                    {
                        List<IWindow> elements = new List<IWindow>();
                        foreach (IWindow window in (element as DockingManager).m_WindowsRegistered)
                        {
                            elements.Add(window);
                        }

                        foreach (IWindow window in elements)
                        {
                            try
                            {
                                FrameworkElement element1 = window.PrimaryElement;
                                (element as DockingManager).Children.Remove(window.PrimaryElement);
                                (element as DockingManager).Children.Add(element1);
                                (element as DockingManager).ExtractElementToWindow(element1, ActionMode.Active, false);
                            }
                            catch { }
                        }
                    }
                }
                finally
                {
                    RoutedEventArgs args = new RoutedEventArgs(DockStateChangedEvent, element);
                    if (element != null && DockingManager.GetState(element) == DockState.Float)
                        DockingManager.Setclosefloatproperty(element, false);
                    element.RaiseEvent(args);
                    if (owner != null)
                    {
                        owner.OnStatePropertyChanged(element, e);
                    }
                }
            }
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Calls OnSideInDockedModeChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnSideInDockedModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!updatedockflag)
            {
                makedockflag = false;
            }
            else
            {
                makedockflag = true;
            }
            DockingManager.OnSideModeChanged(d, (DockSide)e.NewValue, (DockSide)e.OldValue, DockState.Dock);
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Calls OnSideInFloatModeChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnSideInFloatModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!updatedockflag)
            {
                makedockflag = false;
            }
            DockingManager.OnSideModeChanged(d, (DockSide)e.NewValue, (DockSide)e.OldValue, DockState.Float);
        }
        /// <summary>
        /// Called when [no dock changed].
        /// </summary>
        /// <param name="d">The DependencyObject.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnNoDockChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)d;
            element.CoerceValue(DockingManager.StateProperty);

            DockingManager owner = DockingManager.ResolveManager(element);

            if (owner != null)
            {
                bool bNoDock = (bool)e.NewValue;
                DockInfoInternal info = DockingManager.GetDockInfo(element);

                if (info.FloatingWindow != null)
                {
                    info.FloatingWindow.AllowsTransparency = !bNoDock;
                }
            }
        }

        private static void OnSideTabItemBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SetIsSideTabItemBackgroundEnabled(d, true);
        }

        private static void OnSideTabItemForegroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SetIsSideTabItemForegroundEnabled(d, true);
        }

        /// <summary>
        /// Called when [can dock changed].
        /// </summary>
        /// <param name="d">The DependencyObject.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnCanDockChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            d.CoerceValue(DockingManager.StateProperty);
        }

        /// <summary>
        /// Called when [can maximize changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnCanMaximizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            d.CoerceValue(DockingManager.DockWindowStateProperty);
            var element = DockingManager.GetDockHost(d as FrameworkElement);
            DockingManager _dockingmanager = DockingManager.ResolveManager(d as UIElement);
            if (_dockingmanager != null && _dockingmanager.MaximizeButtonEnabled==true)
            {
                if (_dockingmanager.MaximizeMode == MaximizeMode.Default)
                {

                    if (element != null && element.Parent is DockedElementsContainer)
                    {
                        if ((bool)e.NewValue == true)
                        {
                            DockedElementsContainer.EnableMaxMinButtonVisibility(element.Parent as DockedElementsContainer);
                        }
                        else if ((bool)e.NewValue == false)
                        {
                            if (_dockingmanager.CheckMaximizeButtonMode(d))
                            {
                                DockingManager.SetMaximizeButtonVisibility(d, Visibility.Collapsed);
                            }
                        }
                    }
                }
                else if (_dockingmanager.MaximizeMode == MaximizeMode.FullScreen)
                {
                    if ((bool)e.NewValue == false)
                    {
                        if (_dockingmanager.CheckMaximizeButtonMode(d))
                        {
                            DockingManager.SetMaximizeButtonVisibility(d, Visibility.Collapsed);
                        }
                    }
                    else if ((bool)e.NewValue == true)
                    {
                        DockingManager.SetMaximizeButtonVisibility(d, Visibility.Visible);
                    }
                }
            }
        }

        internal bool CheckMaximizeButtonMode(DependencyObject d)
        {
            DockingManager _dockingmanager = DockingManager.ResolveManager(d as UIElement);
            return ((_dockingmanager!=null)
                     &&_dockingmanager.MaximizeButtonEnabled
                     && (!DockingManager.GetCanMaximize(d)) 
                     && _dockingmanager.MaximizeButtonMode == VisibilityMode.Disable) ? false: true; 
        }

        /// <summary>
        /// Called when [can minimize changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnCanMinimizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            d.CoerceValue(DockingManager.DockWindowStateProperty);
            
            DockingManager _dockingmanager = DockingManager.ResolveManager(d as UIElement);
            if (_dockingmanager != null && _dockingmanager.MinimizeButtonEnabled == true)
            {
                if ((bool)e.NewValue == false)
                {
                    DockingManager.SetMinimizeButtonVisibility(d, Visibility.Collapsed);                    
                }
                else if ((bool)e.NewValue == true)
                {
                    DockingManager.SetMinimizeButtonVisibility(d, Visibility.Visible);
                }
            }
        }

        /// <summary>
        /// Called when [minimized item visibility changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnMaximizeModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DockingManager manager = d as DockingManager;
            if (manager != null)
            {
                if ((MaximizeMode)e.NewValue == MaximizeMode.FullScreen)
                {
                    for (int i = 0; i < manager.Children.Count; i++)
                    {
                        DockingManager.SetChildMinimizedHeight(manager.Children[i], 0d);
                        DockingManager.SetChildMinimizedWidth(manager.Children[i], 0d);
                    }
                }
                else 
                {
                    for (int i = 0; i < manager.Children.Count; i++)
                    {
                        DockingManager.SetChildMinimizedHeight(manager.Children[i], 20d);
                        DockingManager.SetChildMinimizedWidth(manager.Children[i], 30d);
                        if (DockingManager.GetRestoreButtonVisibility(manager.Children[i]) == Visibility.Visible)
                        {
                            manager.m_executingrestoreflag = true;
                            manager.RestoreFullScreenMode(manager.Children[i]);
                            DockingManager.SetMaximizeButtonVisibility(manager.Children[i], Visibility.Visible);
                            DockingManager.SetRestoreButtonVisibility(manager.Children[i], Visibility.Collapsed);
                            manager.m_executingrestoreflag = false;
                        }

                    }
                }
            }
        }

        /// <summary>
        /// Called when [can float changed].
        /// </summary>
        /// <param name="d">The DependencyObject.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnCanFloatChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            d.CoerceValue(DockingManager.StateProperty);
        }

        /// <summary>
        /// Called when [can document changed].
        /// </summary>
        /// <param name="d">The DependencyObject.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnCanDocumentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            d.CoerceValue(DockingManager.StateProperty);
        }

        /// <summary>
        /// Called when [can close changed].
        /// </summary>
        /// <param name="d">The DependencyObject.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnCanCloseChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            d.CoerceValue(DockingManager.StateProperty);

            #region CloseButtonVisibilityChecking

            TabControlExt instance = DockingManager.GetTabControl(d);
            if (instance != null && instance.TabPanel != null && instance.TabPanel.Template != null) 
            {
                ToggleButton closebutton = instance.TabPanel.Template.FindName("PART_CloseButton", instance.TabPanel) as ToggleButton;
                instance.CheckCloseButtonVisibility(closebutton);
            }

            #endregion
        }

        /// <summary>
        /// Called when [no header changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnNoHeaderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DockState state = DockingManager.GetState(d);
            DockingManager owner = DockingManager.ResolveManager(d as UIElement);
            DockInfoInternal dockinfo = DockingManager.GetDockInfo(d);
            bool IsMultiHostContainer = false;
            if (owner !=null && !owner.UseNativeFloatWindow)
            {
                IsMultiHostContainer = dockinfo != null && dockinfo.FloatingWindow != null ? dockinfo.FloatingWindow.IsMultiHostsContainer : false;
            }
            else
            {
                IsMultiHostContainer = dockinfo != null && dockinfo.NativeWindow != null ? dockinfo.NativeWindow.IsMultiHostsContainer : false;
            }
            bool canexecute = DockingManager.GetTargetNameInFloatingMode(d) == string.Empty ? !IsMultiHostContainer : false;

            if (!IsVisibleState(state) && DockingManager.GetPreviousNoHeader(d) != (bool)e.NewValue && owner != null && !owner.IsDragging && !owner.m_loadingState)
                DockingManager.SetPreviousNoHeader(d, (bool)e.NewValue);
            else if (state == DockState.Float && !(bool)e.NewValue && canexecute)
            {
                DockingManager.SetPreviousNoHeader(d, (bool)e.NewValue);
                DockingManager.SetNoHeader(d, true);
            }
        }

        /// <summary>
        /// Called when [header changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnHeaderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Style style = DockingManager.GetDocumentTabItemStyle(d);
            if (style == null )
            {
                if(DockingManager.GetHeader(d)!=null)
                {
                    if (CheckHasSpecialChar(DockingManager.GetHeader(d).ToString()))
                    {
                        ResourceDictionary dictionary = new ResourceDictionary
                        {
                            Source = new Uri(
                                "pack://application:,,,/Syncfusion.Tools.WPF;component/Framework/DockingManager/Themes/vista.aero.xaml",
                                UriKind.RelativeOrAbsolute)
                        };

                        if (m_dockingheadertemplate != null)
                        {
                            DockingManager.SetHeaderTemplate(d, m_dockingheadertemplate);
                        }
                        else if (DockingManager.ResolveManager(d as UIElement) != null)
                        {
                            DataTemplate headertemp = dictionary["DocumentTabItemHeaderTemplate"] as DataTemplate;
                            DockingManager.SetHeaderTemplate(d, headertemp);
                        }
                        else
                        {
                            if (DocumentContainer.GetHeaderTemplate(d) == null && m_dockingheadertemplate == null && DockingManager.GetHeader(d).GetType() == typeof(string))
                            {
                                DataTemplate headertemp = dictionary["DocumentTabItemHeaderTemplate"] as DataTemplate;
                                DocumentContainer.SetHeaderTemplate(d, headertemp);
                            }
                        }
                    }
                }
                
            }
               
        }

        private bool FloatWindowHasChild(IWindow window)
        {
            List<FrameworkElement> floatchild = new List<FrameworkElement>();

            for (int i = 0; i < this.FilterChildren(DockState.Float).Count; i++)
                floatchild.Add(this.FilterChildren(DockState.Float)[i]);
            for (int j = 0; j < floatchild.Count; j++)
            {
                if (DockingManager.GetTabbedHost(floatchild[j], DockingManager.GetState(floatchild[j])).Equals(window.InternalDataContext))
                {
                    this.hasChild = true;
                }
            }
            return this.hasChild;
        }

        private bool NativeFloatWindowHasChild(NativeFloatWindow window)
        {
            List<FrameworkElement> floatchild = new List<FrameworkElement>();

            for (int i = 0; i < this.FilterChildren(DockState.Float).Count; i++)
                floatchild.Add(this.FilterChildren(DockState.Float)[i]);
            for (int j = 0; j < floatchild.Count; j++)
            {
                if (DockingManager.GetTabbedHost(floatchild[j], DockingManager.GetState(floatchild[j])) != null && DockingManager.GetTabbedHost(floatchild[j], DockingManager.GetState(floatchild[j])).Equals(window.InternalDataContext))
                {
                    this.hasChild = true;
                }
            }
            return this.hasChild;
        }

        /// <summary>
        /// Checks the has special char.
        /// </summary>
        /// <param name="header">The header.</param>
        /// <returns></returns>
        private static bool CheckHasSpecialChar(string header)
        {
            if(header!=null)
            {
                byte[] asciibyte = Encoding.ASCII.GetBytes(header); 
                foreach (byte byt in asciibyte)
                {
                    if ((byt>32 && byt <48) || (byt>57 && byt<65)|| (byt>90 && byt <97) || (byt>122 && byt <127))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        /// <property name="flag" value="Finished" />s
        /// <summary>
        /// Calls OnFloatingWindowRectChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnFloatingWindowRectChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        /// <summary>
        /// Checks the rect boundary detection.
        /// </summary>
        /// <param name="screenrect">The screenrect.</param>
        /// <param name="actualrect">The actualrect.</param>
        private Rect CheckRectBoundaryDetection(Rect actualrect)
        {
            if (!this.UseNativeFloatWindow)
            {
                foreach (System.Windows.Forms.Screen screen in System.Windows.Forms.Screen.AllScreens)
                {
                    if (screen.Bounds.X + screen.Bounds.Width > actualrect.X)
                    {
                        if (actualrect.Width > 0.87 * screen.Bounds.Width)
                        {
                            actualrect.Width = 0.87 * screen.Bounds.Width;
                        }
                        if (actualrect.Height > 0.75 * screen.Bounds.Height)
                        {
                            actualrect.Height = 0.75 * screen.Bounds.Height;
                        }
                        break;
                    }
                }
            }
            return actualrect;
        }

        /// <summary>
        /// Occurs when AutoHideVisibility changed, updates docking layout.
        /// </summary>
        /// <param name="d">Instance of docking manager class.</param>
        /// <param name="e">event args.</param>
        private static void OnAutoHideVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DockingManager owner = (DockingManager)d;

            for (int i = 0, cnt = owner.Children.Count; i < cnt; ++i)
            {
                owner.Children[i].CoerceValue(DockingManager.StateProperty);
            }
        }

        /// <summary>
        /// Called when [can auto hide changed].
        /// </summary>
        /// <param name="d">The dependency object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnCanAutoHideChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            d.CoerceValue(DockingManager.StateProperty);
        }

        /// <summary>
        /// Called when [can resize changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnCanResizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DockState elementstate = DockingManager.GetState(d);
            DockingManager manager = DockingManager.ResolveManager(d as UIElement);
            if (!(bool)e.NewValue)
            {
                if (manager != null)
                {
                    if (elementstate == DockState.Dock)
                    {
                        DockedElementTabbedHost host = DockingManager.ResolveHostDock(d as UIElement);
                        if (host != null)
                        {
                            if (host.ActualHeight > 0)
                            {
                                DockingManager.SetFixedHeight(d, host.ActualHeight);
                            }
                            if (host.ActualWidth > 0)
                            {
                                DockingManager.SetFixedWidth(d, host.ActualWidth);
                            }
                        }
                    }
                    if (elementstate == DockState.Float && manager.UseNativeFloatWindow)
                    {
                        NativeFloatWindow window = GetNativeWindow(d as FrameworkElement);
                        if (e.Property.Name == "CanResizeInFloatState")
                        {
                            window.ResizeMode = ResizeMode.NoResize;
                        }
                        if (e.Property.Name == "CanResizeHeightInFloatState")
                        {
                            window.MinHeight = window.MaxHeight = window.Height;
                        }
                        if (e.Property.Name == "CanResizeWidthInFloatState")
                        {
                            window.MinWidth = window.MaxWidth = window.Width;
                        }
                        if (!(DockingManager.GetCanResizeWidthInFloatState(d)))
                        {
                            if (!(DockingManager.GetCanResizeHeightInFloatState(d)))
                            {
                                window.ResizeMode = ResizeMode.NoResize;
                                window.MinHeight = 0.0;
                                window.MaxHeight = double.PositiveInfinity;
                                 window.MinWidth = 0.0;
                            window.MaxWidth = double.PositiveInfinity;
                            }
                        }
                    }
                   
                }
            }
            else
            {
                if (manager != null)
                {
                    if (elementstate == DockState.Float && manager.UseNativeFloatWindow)
                    {
                        NativeFloatWindow window = GetNativeWindow(d as FrameworkElement);
                        if (e.Property.Name == "CanResizeInFloatState")
                        {
                            window.ResizeMode = ResizeMode.CanResize;
                        }
                        if (e.Property.Name == "CanResizeHeightInFloatState")
                        {
                            window.MinHeight = 0.0;
                            window.MaxHeight = double.PositiveInfinity;
                        }
                        if (e.Property.Name == "CanResizeWidthInFloatState")
                        {
                            window.MinWidth = 0.0;
                            window.MaxWidth = double.PositiveInfinity;
                        }
                    }
                }
            }

        }
        
        /// <summary>
        /// Called when [side mode changed].
        /// </summary>
        /// <param name="d">The DependencyObject.</param>
        /// <param name="newValue">The new value.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="state">The dock state.</param>
        private static void OnSideModeChanged(DependencyObject d, DockSide newValue, DockSide oldValue, DockState state)
        {
            FrameworkElement element = (FrameworkElement)d;
            DockingManager owner = DockingManager.ResolveManager(element);

            if (owner != null && !owner.LockPropertyChangedAction)
            {
				if (newValue == DockSide.Tabbed && IsVisibleState(state) && !owner.m_bLockedSideChanged)
				{
					DetachFromChildrenAsync(element, state);
					foreach (FrameworkElement child in owner.Children)
					{
						string targetName = DockingManager.GetTargetName(child, state);

                        if (targetName == element.Name && DockingManager.GetSide(child, state) == newValue && DockingManager.GetState(child) == state
                            && DockingManager.GetTargetName(element, state) != child.Name && DockingManager.GetTargetName(element, state) != string.Empty)
                        {
                            DockingManager.SetTargetName(child, DockingManager.GetTargetName(element, state), state);
                        }
					}
				}

                if (!owner.m_bLockedSideChanged && state == DockState.Dock && IsVisibleState(DockingManager.GetState(element)))
                {
                    List<FrameworkElement> siblings = new List<FrameworkElement>();
                    string parentName = element.Name;

                    foreach (FrameworkElement child in owner.Children)
                    {
                        string targetName = DockingManager.GetTargetNameInDockedMode(child);

                        if (targetName == parentName && DockState.AutoHidden == DockingManager.GetState(child))
                        {
                            siblings.Add(child);
                        }
                    }

                    bool bIsSetNewName = false;
                    string targetNameInDockedMode = string.Empty;

                    foreach (FrameworkElement sibling in siblings)
                    {
                        DockingManager.SetTargetNameInDockedMode(sibling, targetNameInDockedMode);

                        if (!bIsSetNewName)
                        {
                            targetNameInDockedMode = sibling.Name;
                            DockingManager.SetSideInDockedMode(sibling, oldValue);
                            bIsSetNewName = true;
                        }
                    }
                }

               
            }
			//if (state == DockState.Dock)
			//{
               // DockingManager.SetSideInDockedMode(element, newValue);
			//}
            if (owner!=null && owner.UseDocumentContainer && makedockflag)
            {
                owner.LockLayoutUpdate = true;
            }

            if (owner != null && !owner.m_onStatechange && makedockflag)
            {
                ////owner.m_loadflag = true;
                owner.ResetDocking();
                owner.LockLayoutUpdate = false;
                owner.UpdateLayout();
                owner.LockLayoutUpdate = true;

                //owner.Refresh(element);
            }
            if (owner!=null && owner.UseDocumentContainer && makedockflag && owner.LockLayoutUpdate)
            {
                owner.LockLayoutUpdate = false;
            }
            makedockflag = true;
        }

        /// <summary>
        /// Invalidates the parent measure.
        /// </summary>
        /// <param name="element">The element.</param>
        private static void InvalidateParentMeasure(FrameworkElement element)
        {
            if (element != null)
            {
                VisualUtils.InvalidateParentMeasure(element);
            }
        }

        /// <summary>
        /// Corrects the size of the tabs.
        /// </summary>
        /// <param name="element">The element.</param>
        private static void CorrectTabsSize(FrameworkElement element)
        {
            DockingManager owner = DockingManager.ResolveManager(element);

            if (owner != null)
            {
                DockState state = DockingManager.GetState(element);

                if (IsVisibleState(state))
                {
                    List<FrameworkElement> tabs = owner.FindSiblingsSafe(element, state);

                    foreach (FrameworkElement tab in tabs)
                    {
                        if (GetSideSafe(tab, state) == DockSide.Tabbed)
                        {
                            CopyWindowSize(element, tab, state);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Called when [is froze changed].
        /// </summary>
        /// <param name="d">The DependencyObject.</param>
        /// <param name="arg">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnIsFrozeChanged(DependencyObject d, DependencyPropertyChangedEventArgs arg)
        {
            FrameworkElement element = (FrameworkElement)d;
            DockingManager owner = DockingManager.ResolveManager(element);

            if (null != owner)
            {
                owner.OnIsFrozeChanged(element, arg);
            }
        }

        /// <summary>
        /// Validates the floating window rect.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="newState">The new state.</param>
        private static void ValidateFloatingWindowRect(FrameworkElement element, DockState newState)
        {
            if (newState == DockState.Float)
            {
                Rect rect = DockingManager.GetFloatingWindowRect(element);

                if (rect == Rect.Empty)
                {
                    InitFloatingWindowRect(element, DockState.Dock);
                }
            }
        }

        /// <summary>
        /// Called when [dock fill property changed].
        /// </summary>
        /// <param name="d">The DependencyObject.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnDockFillPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DockingManager docking = (DockingManager)d;

            if ((bool)e.NewValue)
            {
                docking.ActivateDockFill();
            }
            else
            {
                docking.DeactivateDockFill();
            }
        }

        /// <summary>
        /// Called when [dock fill document mode property changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnDockFillDocumentModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DockingManager docking = (DockingManager)d;


            if (docking.Children.Count > 0)
            {
                if (docking.DockFill)
                {
                    docking.ActivateDockFill();
                    docking.CheckDockFillProperty();
                }
                else
                {
                    docking.DeactivateDockFill();
                }
            }
        }

        /// <summary>
        /// Called when [client control changed].
        /// </summary>
        /// <param name="d">The DependencyObject.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnClientControlChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DockingManager docking = (DockingManager)d;
            UIElement newContent = e.NewValue as UIElement;
            if (docking != null && newContent != null)
            {
                BindingUtils.SetBinding(newContent, docking, FrameworkElement.DataContextProperty, FrameworkElement.DataContextProperty, BindingMode.OneWay);
            }
        }
        /// <summary>
        /// Called when [HideTdiHeaderOnSingleChild changed]
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnHideTDIHeaderOnSingleChildChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DockingManager docking = (DockingManager)d;
            if (docking.UseDocumentContainer && docking.DocContainer!=null && docking.ContainerMode==DocumentContainerMode.TDI)
            {
                DocumentContainer container = docking.DocContainer as DocumentContainer;
                if (container != null)
                {
                    container.HideTDIHeaderOnSingleChild = docking.HideTDIHeaderOnSingleChild;
                }
            }
        }

        /// <summary>
        /// Called when [use document container changed].
        /// </summary>
        /// <param name="d">The DependencyObject.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnUseDocumentContainerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DockingManager docking = (DockingManager)d;
            bool bUseDC = (bool)e.NewValue;
            if (bUseDC && docking.m_container == null)
            {
                docking.m_container = docking.InitializationDocumentContainer();
                docking.m_container.FlipParent = docking;
                docking.AddDocumentContainerHandler();
            }
            if (docking.IsInitialized)
            {
                docking.SetControlCenter();
                docking.LockPropertyChangedAction = true;
                docking.LockLayoutUpdate = true;
            }

            docking.Children.ForEach(element => element.CoerceValue(DockingManager.StateProperty));
            docking.Children.ForEach(element => element.CoerceValue(DockingManager.CanDocumentProperty));

            if (docking.IsInitialized)
            {
                docking.Children.ForEach(element =>
                {
                    if (bUseDC && DockState.Document == DockingManager.GetState(element))
                    {
                        docking.InsertToDocumentContainer(element);
                    }
                    else
                    {
                        docking.RemoveFromDocumentContainer(element);
                    }
                });
            }

            docking.LockPropertyChangedAction = false;
        }

        /// <summary>
        /// Called when [custom menu items changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnCustomMenuItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            DockingManager owner = d as DockingManager;
            if (owner != null)
            {
                if ((owner.Parent as DockingManager) == null)
                {
                    owner.m_custommenuitems = (CustomMenuItemCollection)args.NewValue;
                }
                else
                {
                    if (args.NewValue!=null && !args.NewValue.Equals((owner.Parent as DockingManager).m_custommenuitems))
                    {
                        owner.m_custommenuitems = (CustomMenuItemCollection)args.NewValue;
                    }
                }
            }
            else
            {
                owner = DockingManager.ResolveManager(d as UIElement);
                if (owner != null)
                {
                    if ((owner.Parent as DockingManager) == null)
                    {
                        owner.m_custommenuitems = (CustomMenuItemCollection)args.NewValue;
                    }
                    else
                    {
                        if (args.NewValue!=null && !args.NewValue.Equals((owner.Parent as DockingManager).m_custommenuitems))
                        {
                            owner.m_custommenuitems = (CustomMenuItemCollection)args.NewValue;
                        }
                    }
                }
            }
        }

        internal bool m_activeflag = true;

        internal bool m_nativeWindowDragging = false;

        /// <summary>
        /// Called when [document tab item style changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnDocumentTabItemStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            DockingManager dockingmanager = d as DockingManager;
            ObservableFrameworkElements DocumentTabElements = new ObservableFrameworkElements();
            if (dockingmanager != null)
            {
                DocumentTabElements = dockingmanager.GetDocumentTabElements(dockingmanager);

                if ((args.NewValue as Style) != null)
                {
                    if (DocumentTabElements.Count > 0)
                    {
                        foreach (FrameworkElement element in DocumentTabElements)
                        {
                            TabControlExt tabcontrol = DockingManager.GetTabControl(element as DependencyObject);
                            if (tabcontrol != null)
                            {
                                foreach (object obj in tabcontrol.Items)
                                {
                                    TabItemExt tabitem = obj as TabItemExt;
                                    if (tabitem != null)
                                    {
                                        tabitem.Style = args.NewValue as Style;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                dockingmanager = DockingManager.ResolveManager(d as UIElement);
                if (dockingmanager != null)
                {
                    DocumentTabElements = dockingmanager.GetDocumentTabElements(dockingmanager);

                    if ((args.NewValue as Style) != null)
                    {
                        if (DocumentTabElements.Count > 0)
                        {
                            foreach (FrameworkElement element in DocumentTabElements)
                            {
                                TabControlExt tabcontrol = DockingManager.GetTabControl(element as DependencyObject);
                                if (tabcontrol != null)
                                {
                                    foreach (object obj in tabcontrol.Items)
                                    {
                                        TabItemExt tabitem = obj as TabItemExt;
                                        if (tabitem != null)
                                        {
                                            if ((tabitem.Content as ContentPresenter).Content == d)
                                            {
                                                tabitem.Style = args.NewValue as Style;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                
            }
        }
        private ObservableFrameworkElements GetDocumentTabElements(DockingManager dockingmanager)
        {
            ObservableFrameworkElements DocumenttabElements = new ObservableFrameworkElements();
            if (dockingmanager.Children.Count > 0)
                    {
                        foreach (FrameworkElement element in dockingmanager.Children)
                        {
                            if (DockingManager.GetState(element) == DockState.Document)
                            {
                                DocumenttabElements.Add(element);
                            }
                        }
                    }
            return DocumenttabElements;
        }

        /// <summary>
        /// Called when [document tab control style changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnDocumentTabControlStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            DockingManager dockingmanager = d as DockingManager;
            if (dockingmanager != null)
            {
                ObservableFrameworkElements DocumentTabElements = new ObservableFrameworkElements();
                if (dockingmanager != null)
                {
                    if (dockingmanager.Children.Count > 0)
                    {
                        foreach (FrameworkElement element in dockingmanager.Children)
                        {
                            if (DockingManager.GetState(element) == DockState.Document)
                            {
                                DocumentTabElements.Add(element);
                            }
                        }
                    }
                }
                if ((args.NewValue as Style) != null)
                {
                    if (DocumentTabElements.Count > 0)
                    {
                        foreach (FrameworkElement element in DocumentTabElements)
                        {
                            TabControlExt tabcontrol = DockingManager.GetTabControl(element as DependencyObject);
                            if (tabcontrol != null)
                            {
                                tabcontrol.Style = args.NewValue as Style;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Called when [is selected tab changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnIsSelectedTabChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            FrameworkElement element = (FrameworkElement)d;
            DockingManager owner = DockingManager.ResolveManager(element);
            if (owner != null)
            {
                owner.OnIsSelectedTabChanged(element, args);
            }
        }

        /// <summary>
        /// Called when [is active window changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnIsActiveWindowChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            FrameworkElement element = (FrameworkElement)d;
            DockingManager owner = DockingManager.ResolveManager(element);
            if (owner != null)
            {
                owner.OnIsActiveWindowChanged(element,args);
            }
        }

        /// <summary>
        /// Called when [active window changed].
        /// </summary>
        /// <param name="d">The DependencyObject.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        internal List<FrameworkElement> lastacitive = new List<FrameworkElement>();
        private static void OnActiveWindowChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            DockingManager owner = (DockingManager)d;
            if (owner != null && (args.NewValue as DependencyObject) != null)
            {
                var state = DockingManager.GetState(args.NewValue as DependencyObject);
                var host = DockingManager.GetTabbedHost(owner.ActiveWindow, state);
                if (host != null)
                {
                    if (host.InternalTabControl != null && args.NewValue != host.InternalTabControl.SelectedItem)
                    {
                        host.SelectTab(owner.ActiveWindow);
                    }
                }
            }
            owner.OnActiveWindowChanged(args);

            if ((args.NewValue as DependencyObject) != null)
            {
                DockingManager.SetIsActiveWindow(args.NewValue as DependencyObject, true);
                var frameworkElement = args.NewValue as FrameworkElement;
                if (frameworkElement != null) frameworkElement.Focus();
            }

            if ((args.OldValue as DependencyObject) != null)
            {
                DockingManager.SetIsActiveWindow(args.OldValue as DependencyObject, false);
            }

            if ((args.OldValue as DependencyObject) != null && (args.NewValue as DependencyObject)!=null)
            {
                TabControlExt tabcontrol = DockingManager.GetTabControl(args.OldValue as DependencyObject);
                TabControlExt tabcontrol1 = null;
                if ((args.NewValue as DependencyObject) != null)
                {
                    tabcontrol1 = DockingManager.GetTabControl(args.NewValue as DependencyObject);
                }
                if (tabcontrol != null&&tabcontrol1==null)
                {
                    if (DockingManager.GetState(args.NewValue as DependencyObject) == DockState.Document)
                    {
                        tabcontrol.IsFocus = true;
                        if ((tabcontrol.SelectedItem as TabItemExt) != null)
                        {
                            (tabcontrol.SelectedItem as TabItemExt).IsFocus = true;
                        }
                    }
                    else
                    {
                        tabcontrol.IsFocus = false;
                        if ((tabcontrol.SelectedItem as TabItemExt) != null)
                        {
                            (tabcontrol.SelectedItem as TabItemExt).IsFocus = false;
                        }
                    }
                }
                else if (tabcontrol1 != null && tabcontrol == null)
                {
                    tabcontrol1.IsFocus = true;
                    if ((tabcontrol1.SelectedItem as TabItemExt) != null)
                    {
                        (tabcontrol1.SelectedItem as TabItemExt).IsFocus = true;
                    }
                }
                else if(tabcontrol1!=null)
                {
                    tabcontrol1.IsFocus = true;
                    tabcontrol1.IsTabGroupFocus = true;
                    if ((tabcontrol1.SelectedItem as TabItemExt) != null)
                    {
                        (tabcontrol1.SelectedItem as TabItemExt).IsFocus = true;
                    }
                }
            }
            if (owner.DocContainer != null && owner.m_activeflag)
            {
                owner.DocContainer.ValidateActiveDocument((UIElement)args.NewValue);
            }
        }

        private static void OnIsLogicalChildChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            DockingManager docmgr = DockingManager.ResolveManager(obj as UIElement) as DockingManager;
            if (docmgr != null && docmgr.Children.Contains(obj as FrameworkElement))
            {
                docmgr.AddLogicalChild(obj);
            }
        } 
        #endregion

        #region Depenedency Properties

        // Using a DependencyProperty as the backing store for CanNestedFloat.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UseNativeFloatWindowProperty =
             DependencyProperty.Register("UseNativeFloatWindow", typeof(bool), typeof(DockingManager), new UIPropertyMetadata(false, OnUseNativeFloatWindowChanged));

        private static void OnUseNativeFloatWindowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DockingManager docking = (DockingManager)d;
            if ((bool)e.NewValue)
            {
                docking.UseAdornerFloatWindow = false;
            }
        }
        public static int GetDocumentTabOrderIndex(DependencyObject obj)
        {
            return (int)obj.GetValue(DocumentTabOrderIndexProperty);
        }

        public static void SetDocumentTabOrderIndex(DependencyObject obj, int value)
        {
            obj.SetValue(DocumentTabOrderIndexProperty, value);
        }

        // Using a DependencyProperty as the backing store for TabOrderIndex.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty DocumentTabOrderIndexProperty =
            DependencyProperty.RegisterAttached("DocumentTabOrderIndex", typeof(int), typeof(DockingManager), new PropertyMetadata(-1));


        /// <summary>
        /// Identifies DockingManager.IsContextMenuButtonVisible attached property
        /// </summary>

        public static readonly DependencyProperty IsContextMenuVisibleProperty =
         DependencyProperty.RegisterAttached("IsContextMenuVisible", typeof(bool), typeof(DockingManager), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.Inherits));//, new PropertyChangedCallback(OnDesiredSizeInDockedModeChanged), new CoerceValueCallback(OnCoerceSize)));

        /// <summary>
        /// Identifies DockingManager.AllowsTransparencyForFloatWindow attached property
        /// </summary>
        public static readonly DependencyProperty AllowsTransparencyForFloatWindowProperty=
            DependencyProperty.RegisterAttached("AllowsTransparencyForFloatWindow",typeof(bool),typeof(DockingManager),new FrameworkPropertyMetadata(true,FrameworkPropertyMetadataOptions.Inherits));


        //public static readonly DependencyProperty AllowMDIResizeProperty = DependencyProperty.RegisterAttached("AllowMDIResize", typeof(bool), typeof(DocumentContainer), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.Inherits, new PropertyChangedCallback(OnAllowMDIResizeChanged)));
        /// <summary>
        /// Identifies DockingManager.IsContextMenuButtonVisible attached property
        /// </summary>

        public static readonly DependencyProperty IsContextMenuButtonVisibleProperty =
           DependencyProperty.RegisterAttached("IsContextMenuButtonVisible", typeof(bool), typeof(DockingManager), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.Inherits, OnIsContextMenuButtonVisibleChanged));//, new PropertyChangedCallback(OnDesiredSizeInDockedModeChanged), new CoerceValueCallback(OnCoerceSize)));

        /// <summary>
        /// Identifies DockingManager.IsRollupFloatWindow attached property
        /// </summary>
        public static readonly DependencyProperty IsRollupFloatWindowProperty =
          DependencyProperty.RegisterAttached("IsRollupFloatWindow", typeof(bool), typeof(DockingManager), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits));//, new PropertyChangedCallback(OnDesiredSizeInDockedModeChanged), new CoerceValueCallback(OnCoerceSize)));
 
        /// <summary>
        /// Identifies DockingManager.DesiredWidthInDockedMode attached property.
        /// </summary>
        /// <remarks>
        /// This property can be attached to a docking manager child and is used to calculate parent host width in docked state.
        /// Default value is 90.
        /// </remarks>
        public static readonly DependencyProperty DesiredWidthInDockedModeProperty =
            DependencyProperty.RegisterAttached("DesiredWidthInDockedMode", typeof(double), typeof(DockingManager), new FrameworkPropertyMetadata(90d, new PropertyChangedCallback(OnDesiredSizeInDockedModeChanged), new CoerceValueCallback(OnCoerceSize)));
        /// <summary>
        /// Identifies DockingManager.SizetoContentInFloatProperty
        /// </summary>
        public static readonly DependencyProperty SizetoContentInFloatProperty =
            DependencyProperty.RegisterAttached("SizetoContentInFloat", typeof(bool), typeof(DockingManager), new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Identifies DockingManager.SizetoContentInDockProperty
        /// </summary>
        public static readonly DependencyProperty SizetoContentInDockProperty =
            DependencyProperty.RegisterAttached("SizetoContentInDock", typeof(bool), typeof(DockingManager), new FrameworkPropertyMetadata(false));


        /// <summary>
        /// Identifies DockingManager.DesiredHeightInDockedMode attached property.
        /// </summary>
        /// <remarks>
        /// This property can be attached to a docking manager child and is used to calculate parent host height in docked state.
        /// Default value is 90.
        /// </remarks>
        public static readonly DependencyProperty DesiredHeightInDockedModeProperty =
            DependencyProperty.RegisterAttached("DesiredHeightInDockedMode", typeof(double), typeof(DockingManager), new FrameworkPropertyMetadata(90d, new PropertyChangedCallback(OnDesiredSizeInDockedModeChanged), new CoerceValueCallback(OnCoerceSize)));

        /// <summary>
        /// Identifies DockingManager.FixedHeight attached property.
        /// </summary>
        /// <remarks>
        /// This property can be attached to a docking manager child and is used to calculate parent host fixed height.
        /// Default value is 90.
        /// </remarks>
        internal static readonly DependencyProperty FixedHeightProperty =
            DependencyProperty.RegisterAttached("FixedHeight", typeof(double), typeof(DockingManager), new FrameworkPropertyMetadata(0d));

        /// <summary>
        /// Identifies DockingManager.FixedWidth attached property.
        /// </summary>
        /// <remarks>
        /// This property can be attached to a docking manager child and is used to calculate parent host fixed width.
        /// Default value is 90.
        /// </remarks>
        internal static readonly DependencyProperty FixedWidthProperty =
            DependencyProperty.RegisterAttached("FixedWidth", typeof(double), typeof(DockingManager), new FrameworkPropertyMetadata(0d));

        internal static readonly DependencyProperty HostRectProperty=
            DependencyProperty.RegisterAttached("HostRect",typeof(Rect),typeof(DockingManager),new FrameworkPropertyMetadata(new Rect(new Point(0,0),new Size(0,0))));

        /// <summary>
        /// Identifies DockingManager.DesiredClientHeightInDockedMode attached property.
        /// </summary>
        /// <remarks>
        /// This property can be attached to a docking manager child and is used to calculate parent host element's height in docked state.
        /// Default value is 90.
        /// </remarks>
        public static readonly DependencyProperty DesiredClientHeightInDockedModeProperty =
            DependencyProperty.RegisterAttached("DesiredClientHeightInDockedMode", typeof(double), typeof(DockingManager), new FrameworkPropertyMetadata(90d, new PropertyChangedCallback(OnDesiredSizeInDockedModeChanged), new CoerceValueCallback(OnCoerceSize)));

        /// <summary>
        /// Identifies DockingManager.DesiredClientHeightInFloatMode attached property.
        /// </summary>
        /// <remarks>
        /// This property can be attached to a docking manager child and is used to calculate parent host element's height in float state.
        /// Default value is 90.
        /// </remarks>
        public static readonly DependencyProperty DesiredClientHeightInFloatModeProperty =
            DependencyProperty.RegisterAttached("DesiredClientHeightInFloatMode", typeof(double), typeof(DockingManager), new FrameworkPropertyMetadata(90d, new PropertyChangedCallback(OnDesiredSizeInFloatingModeChanged), new CoerceValueCallback(OnCoerceSize)));

        /// <summary>
        /// Identifies DockingManager.DockedElementsContainerDesiredSize attached property.
        /// </summary>
        /// <remarks>
        /// This property can be attached to a docking manager child and is used to store elements parent container size.
        /// Default value is new Size(0,0).
        /// </remarks>
        internal static readonly DependencyProperty DockedElementsContainerDesiredSizeProperty =
            DependencyProperty.RegisterAttached("DockedElementsContainerDesiredSize", typeof(Size), typeof(DockingManager), new FrameworkPropertyMetadata(new Size(0, 0)));

        internal static readonly DependencyProperty ElementFlagProperty =
            DependencyProperty.RegisterAttached("ElementFlag", typeof(bool), typeof(DockingManager), new FrameworkPropertyMetadata(false));

        ///// <summary>
        ///// Identifies DockingManager.SizeToContentInFloat attached property.
        ///// </summary>
        ///// <remarks>
        ///// This property can be attached to a docking manager child and is used to calculate parent host height in docked state.
        ///// Default value is 90.
        ///// </remarks>
        ////public static readonly DependencyProperty SizeToContentInFloatModeProperty =
        ////    DependencyProperty.RegisterAttached("SizeToContentInFloatMode", typeof(bool), typeof(DockingManager), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnSizeToContentInFloatModeChanged), new CoerceValueCallback(OnCoerceSize)));

        /// <summary>
        /// Identifies DockingManager.DesiredWidthInFloatingMode attached property.
        /// </summary>
        /// <remarks>
        /// This property can be attached to a docking manager child and is used to calculate parent host width in float state.
        /// Default value is 90.
        /// </remarks>
        public static readonly DependencyProperty DesiredWidthInFloatingModeProperty =
            DependencyProperty.RegisterAttached("DesiredWidthInFloatingMode", typeof(double), typeof(DockingManager), new FrameworkPropertyMetadata(90d, new PropertyChangedCallback(OnDesiredSizeInFloatingModeChanged), new CoerceValueCallback(OnCoerceSize)));

        /// <summary>
        /// Identified DockingManager.DesiredMinWidthInFloatingMode dependency property
        /// </summary>
        public static readonly DependencyProperty DesiredMinWidthInFloatingModeProperty =
            DependencyProperty.RegisterAttached("DesiredMinWidthInFloatingMode", typeof(double), typeof(DockingManager), new FrameworkPropertyMetadata(0d, new PropertyChangedCallback(OnDesiredMinWidthInFloatingModeChanged), new CoerceValueCallback(OnCoerceMinWidthSize)));

        /// <summary>
        /// Identified DockingManager.DesiredMaxWidthInFloatingMode dependency property
        /// </summary>
        public static readonly DependencyProperty DesiredMaxWidthInFloatingModeProperty =
            DependencyProperty.RegisterAttached("DesiredMaxWidthInFloatingMode", typeof(double), typeof(DockingManager), new FrameworkPropertyMetadata(0d, new PropertyChangedCallback(OnDesiredMaxWidthInFloatingModeChanged), new CoerceValueCallback(OnCoerceMaxWidthSize)));

        /// <summary>
        /// Identified DockingManager.DesiredMinWidthInDockedMode dependency property
        /// </summary>
        public static readonly DependencyProperty DesiredMinWidthInDockedModeProperty =
            DependencyProperty.RegisterAttached("DesiredMinWidthInDockedMode", typeof(double), typeof(DockingManager), new FrameworkPropertyMetadata(0d, new PropertyChangedCallback(OnDesiredMinWidthInDockedModeChanged), new CoerceValueCallback(OnCoerceMinWidthDockedSize)));

        /// <summary>
        /// Identified DockingManager.DesiredMaxWidthInDockedMode dependency property
        /// </summary>
        public static readonly DependencyProperty DesiredMaxWidthInDockedModeProperty =
            DependencyProperty.RegisterAttached("DesiredMaxWidthInDockedMode", typeof(double), typeof(DockingManager), new FrameworkPropertyMetadata(0d, new PropertyChangedCallback(OnDesiredMaxWidthInDockedModeChanged), new CoerceValueCallback(OnCoerceMaxWidthDockedSize)));



        /// <summary>
        /// Identifies DockingManager.FloatWindowSizeProperty attached propehrty.
        /// </summary>
        public static readonly DependencyProperty FloatWindowSizeProperty =
          DependencyProperty.RegisterAttached("FloatWindowSize", typeof(Size), typeof(DockingManager), new FrameworkPropertyMetadata(new Size(0, 0), new PropertyChangedCallback(OnFloatWindowSizeChanged)));

        /// <summary>
        /// Identifies DockingManager.FloatWindowWidthProperty attached property.
        /// </summary>
        public static readonly DependencyProperty FloatWindowWidthProperty =
          DependencyProperty.RegisterAttached("FloatWindowWidth", typeof(double), typeof(DockingManager), new FrameworkPropertyMetadata(double.NaN, new PropertyChangedCallback(OnFloatWindowSizeChanged)));

        /// <summary>
        /// Identifies DockingManager.FloatWindowHeightProperty attached property.
        /// </summary>
        public static readonly DependencyProperty FloatWindowHeightProperty =
          DependencyProperty.RegisterAttached("FloatWindowHeight", typeof(double), typeof(DockingManager), new FrameworkPropertyMetadata(double.NaN, new PropertyChangedCallback(OnFloatWindowSizeChanged)));

        /// <summary>
        /// Identifies DockingManager.DesiredHeightInFloatingMode attached property.
        /// </summary>
        /// <remarks>
        /// This property can be attached to a docking manager child and is used to calculate parent host height in float state.
        /// Default value is 90.
        /// </remarks>
        public static readonly DependencyProperty DesiredHeightInFloatingModeProperty =
            DependencyProperty.RegisterAttached("DesiredHeightInFloatingMode", typeof(double), typeof(DockingManager), new FrameworkPropertyMetadata(90d, new PropertyChangedCallback(OnDesiredSizeInFloatingModeChanged), new CoerceValueCallback(OnCoerceSize)));

        /// <summary>
        /// Identified DockingManager.DesiredMinHeightInFloatingMode dependency property
        /// </summary>
        public static readonly DependencyProperty DesiredMinHeightInFloatingModeProperty =
            DependencyProperty.RegisterAttached("DesiredMinHeightInFloatingMode", typeof(double), typeof(DockingManager), new FrameworkPropertyMetadata(0d, new PropertyChangedCallback(OnDesiredMinHeightInFloatingModeChanged), new CoerceValueCallback(OnCoerceMinHeightSize)));

        /// <summary>
        /// Identified DockingManager.DesiredMaxHeightInFloatingMode dependency property
        /// </summary>
        public static readonly DependencyProperty DesiredMaxHeightInFloatingModeProperty =
            DependencyProperty.RegisterAttached("DesiredMaxHeightInFloatingMode", typeof(double), typeof(DockingManager), new FrameworkPropertyMetadata(0d, new PropertyChangedCallback(OnDesiredMaxHeightInFloatingModeChanged), new CoerceValueCallback(OnCoerceMaxHeightSize)));

        /// <summary>
        /// Identified DockingManager.DesiredMinHeightInDockedMode dependency property
        /// </summary>
        public static readonly DependencyProperty DesiredMinHeightInDockedModeProperty =
            DependencyProperty.RegisterAttached("DesiredMinHeightInDockedMode", typeof(double), typeof(DockingManager), new FrameworkPropertyMetadata(0d, new PropertyChangedCallback(OnDesiredMinHeightInDockedModeChanged), new CoerceValueCallback(OnCoerceMinHeightDockedSize)));

        /// <summary>
        /// Identified DockingManager.DesiredMaxHeightInDockedMode dependency property
        /// </summary>
        public static readonly DependencyProperty DesiredMaxHeightInDockedModeProperty =
            DependencyProperty.RegisterAttached("DesiredMaxHeightInDockedMode", typeof(double), typeof(DockingManager), new FrameworkPropertyMetadata(0d, new PropertyChangedCallback(OnDesiredMaxHeightInDockedModeChanged), new CoerceValueCallback(OnCoerceMaxHeightDockedSize)));

        /// <summary>
        /// Identifies DockingManager.State dependency property.
        /// </summary>
        /// <remarks>
        /// This property can be attached to a docking manager child and is used to get / set the element DockState.
        /// For all possible cases see <see cref="DockState"/> enum. The default value is DockState.Dock.
        /// </remarks>
        public static readonly DependencyProperty StateProperty =
            DependencyProperty.RegisterAttached("State", typeof(DockState), typeof(DockingManager), new FrameworkPropertyMetadata(DockState.Dock, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnStatePropertyChanged, new CoerceValueCallback(CoerceState)));

        /// <summary>
        /// Identifies DockingManager.DockWindowState dependency property.
        /// </summary>
        public static readonly DependencyProperty DockWindowStateProperty =
            DependencyProperty.RegisterAttached("DockWindowState", typeof(WindowState), typeof(DockingManager), new FrameworkPropertyMetadata(WindowState.Normal, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnDockWindowStateChanged, new CoerceValueCallback(CoerceDockWindowState)));

        /// <summary>
        /// Identified DockingManager.CanResizeInFloatState dependency property
        /// </summary>
        public static readonly DependencyProperty CanResizeInFloatStateProperty =
          DependencyProperty.RegisterAttached("CanResizeInFloatState", typeof(bool), typeof(DockingManager), new FrameworkPropertyMetadata(true,new PropertyChangedCallback(OnCanResizeChanged)));

        /// <summary>
        /// Identified DockingManager.CanResizeHeightInFloatState dependency property
        /// </summary>
        public static readonly DependencyProperty CanResizeHeightInFloatStateProperty =
          DependencyProperty.RegisterAttached("CanResizeHeightInFloatState", typeof(bool), typeof(DockingManager), new FrameworkPropertyMetadata(true, new PropertyChangedCallback(OnCanResizeChanged)));

        /// <summary>
        /// Identified DockingManager.CanResizeWidthInFloatState dependency property
        /// </summary>
        public static readonly DependencyProperty CanResizeWidthInFloatStateProperty =
          DependencyProperty.RegisterAttached("CanResizeWidthInFloatState", typeof(bool), typeof(DockingManager), new FrameworkPropertyMetadata(true, new PropertyChangedCallback(OnCanResizeChanged)));

        /// <summary>
        /// Identified DockingManager.CanResizeHeightInDockedState dependency property
        /// </summary>
        public static readonly DependencyProperty CanResizeHeightInDockedStateProperty =
          DependencyProperty.RegisterAttached("CanResizeHeightInDockedState", typeof(bool), typeof(DockingManager), new FrameworkPropertyMetadata(true, new PropertyChangedCallback(OnCanResizeChanged)));

        /// <summary>
        /// Identified DockingManager.CanResizeWidthInDockedState dependency property
        /// </summary>
        public static readonly DependencyProperty CanResizeWidthInDockedStateProperty =
          DependencyProperty.RegisterAttached("CanResizeWidthInDockedState", typeof(bool), typeof(DockingManager), new FrameworkPropertyMetadata(true, new PropertyChangedCallback(OnCanResizeChanged)));

        /// <summary>
        /// Identified DockingManager.CanResizeInDockedState dependency property
        /// </summary>
        public static readonly DependencyProperty CanResizeInDockedStateProperty =
          DependencyProperty.RegisterAttached("CanResizeInDockedState", typeof(bool), typeof(DockingManager), new FrameworkPropertyMetadata(true, new PropertyChangedCallback(OnCanResizeChanged)));

        /// <summary>
        /// Identified DockingManager.ShowHiddenMenuItem dependency property
        /// </summary>
        public static readonly DependencyProperty ShowHiddenMenuItemProperty =
          DependencyProperty.RegisterAttached("ShowHiddenMenuItem", typeof(bool), typeof(DockingManager), new FrameworkPropertyMetadata(true));

        /// <summary>
        /// Identified DockingManager.ShowFloatingMenuItem dependency property
        /// </summary>
        public static readonly DependencyProperty ShowFloatingMenuItemProperty =
          DependencyProperty.RegisterAttached("ShowFloatingMenuItem", typeof(bool), typeof(DockingManager), new FrameworkPropertyMetadata(true));

        /// <summary>
        /// Identified DockingManager.ShowDockableMenuItem dependency property
        /// </summary>
        public static readonly DependencyProperty ShowDockableMenuItemProperty =
          DependencyProperty.RegisterAttached("ShowDockableMenuItem", typeof(bool), typeof(DockingManager), new FrameworkPropertyMetadata(true));

        /// <summary>
        /// Identified DockingManager.CanResizeInFloatState dependency property
        /// </summary>
        public static readonly DependencyProperty ShowTabbedMenuItemProperty =
          DependencyProperty.RegisterAttached("ShowTabbedMenuItem", typeof(bool), typeof(DockingManager), new FrameworkPropertyMetadata(true));

        /// <summary>
        /// Identified DockingManager.ShowAutoHiddenMenuItem dependency property
        /// </summary>
        public static readonly DependencyProperty ShowAutoHiddenMenuItemProperty =
          DependencyProperty.RegisterAttached("ShowAutoHiddenMenuItem", typeof(bool), typeof(DockingManager), new FrameworkPropertyMetadata(true));

        /// <summary>
        /// Identified DockingManager.ShowDocumentMenuItem dependency property
        /// </summary>
        public static readonly DependencyProperty ShowDocumentMenuItemProperty =
         DependencyProperty.RegisterAttached("ShowDocumentMenuItem", typeof(bool), typeof(DockingManager), new FrameworkPropertyMetadata(true));

        /// <summary>
        /// Identified DockingManager.ShowCloseMenuItem dependency property
        /// </summary>
        public static readonly DependencyProperty ShowCloseMenuItemProperty =
         DependencyProperty.RegisterAttached("ShowCloseMenuItem", typeof(bool), typeof(DockingManager), new FrameworkPropertyMetadata(true));

        /// <summary>
        /// Identified DockingManager.ShowCloseAllButThisMenuItem dependency property
        /// </summary>
        public static readonly DependencyProperty ShowCloseAllButThisMenuItemProperty =
         DependencyProperty.RegisterAttached("ShowCloseAllButThisMenuItem", typeof(bool), typeof(DockingManager), new FrameworkPropertyMetadata(true));

        /// <summary>
        /// Identified DockingManager.ShowCloseAllMenuItem dependency property
        /// </summary>
        public static readonly DependencyProperty ShowCloseAllMenuItemProperty =
           DependencyProperty.RegisterAttached("ShowCloseAllMenuItem", typeof(bool), typeof(DockingManager), new FrameworkPropertyMetadata(true));

        /// <summary>
        /// Identified DockingManager.ShowHorizontalTabGroupMenuItem dependency property
        /// </summary>
        public static readonly DependencyProperty ShowHorizontalTabGroupMenuItemProperty =
           DependencyProperty.RegisterAttached("ShowHorizontalTabGroupMenuItem", typeof(bool), typeof(DockingManager), new FrameworkPropertyMetadata(true));

        /// <summary>
        /// Identified DockingManager.ShowVerticalTabGroupMenuItem dependency property
        /// </summary>
        public static readonly DependencyProperty ShowVerticalTabGroupMenuItemProperty =
           DependencyProperty.RegisterAttached("ShowVerticalTabGroupMenuItem", typeof(bool), typeof(DockingManager), new FrameworkPropertyMetadata(true));

        /// <summary>
        /// Identified DockingManager.ShowMovetoNextTabGroupMenuItem dependency property
        /// </summary>
        public static readonly DependencyProperty ShowMovetoNextTabGroupMenuItemProperty =
         DependencyProperty.RegisterAttached("ShowMovetoNextTabGroupMenuItem", typeof(bool), typeof(DockingManager), new FrameworkPropertyMetadata(true));

        /// <summary>
        /// Identified DockingManager.ShowMovetoPreviousTabGroupMenuItem dependency property
        /// </summary>
        public static readonly DependencyProperty ShowMovetoPreviousTabGroupMenuItemProperty =
         DependencyProperty.RegisterAttached("ShowMovetoPreviousTabGroupMenuItem", typeof(bool), typeof(DockingManager), new FrameworkPropertyMetadata(true));

        /// <summary>
        /// Identified DockingManager.ShowRestoreMenuItem dependency property
        /// </summary>
        public static readonly DependencyProperty ShowRestoreMenuItemProperty =
        DependencyProperty.RegisterAttached("ShowRestoreMenuItem", typeof(bool), typeof(DockingManager), new FrameworkPropertyMetadata(true));

        /// <summary>
        /// Identified DockingManager.ShowMoveMenuItem dependency property
        /// </summary>
        public static readonly DependencyProperty ShowMoveMenuItemProperty =
        DependencyProperty.RegisterAttached("ShowMoveMenuItem", typeof(bool), typeof(DockingManager), new FrameworkPropertyMetadata(true));

        /// <summary>
        /// Identified DockingManager.ShowResizeMenuItem dependency property
        /// </summary>
        public static readonly DependencyProperty ShowResizeMenuItemProperty =
        DependencyProperty.RegisterAttached("ShowResizeMenuItem", typeof(bool), typeof(DockingManager), new FrameworkPropertyMetadata(true));

        /// <summary>
        /// Identified DockingManager.ShowMinimizeMenuItem dependency property
        /// </summary>
        public static readonly DependencyProperty ShowMinimizeMenuItemProperty =
        DependencyProperty.RegisterAttached("ShowMinimizeMenuItem", typeof(bool), typeof(DockingManager), new FrameworkPropertyMetadata(true));

        /// <summary>
        /// Identified DockingManager.ShowMaximizedMenuItem dependency property
        /// </summary>
        public static readonly DependencyProperty ShowMaximizedMenuItemProperty =
        DependencyProperty.RegisterAttached("ShowMaximizedMenuItem", typeof(bool), typeof(DockingManager), new FrameworkPropertyMetadata(true));

        /// <summary>
        /// Identifies DockingManager.Header attached property.
        /// </summary>
        /// <remarks>
        /// This property can be attached to a docking manager child and is used to present element in the 
        /// parent host header.
        /// The default value is empty string.
        /// </remarks>
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.RegisterAttached("Header", typeof(object), typeof(DockingManager), new FrameworkPropertyMetadata(string.Empty, new PropertyChangedCallback(OnHeaderChanged)));

        /// <summary>
        /// Identifies DockingManager.IsTDIDragDropEnabled attached property.
        /// </summary>
        public static readonly DependencyProperty IsTDIDragDropEnabledProperty = DependencyProperty.Register("IsTDIDragDropEnabled", typeof(bool), typeof(DockingManager), new FrameworkPropertyMetadata(true));

        /// <summary>
        /// Identifies DockingManager.DockHeaderPresenter attached property.
       /// </summary>
        internal static readonly DependencyProperty DockHeaderPresenterProperty =
            DependencyProperty.RegisterAttached("DockHeaderPresenter", typeof(DockHeaderPresenter), typeof(DockingManager), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Identifies DockingManager.ChildMinimizedWidth attached property.
       /// </summary>
        internal static readonly DependencyProperty ChildMinimizedWidthProperty =
            DependencyProperty.RegisterAttached("ChildMinimizedWidth", typeof(double), typeof(DockingManager), new FrameworkPropertyMetadata(30d));


        /// <summary>
        /// Identifies DockingManager.ChildMinimizedHeight attached property.
        /// </summary>
        internal static readonly DependencyProperty ChildMinimizedHeightProperty =
            DependencyProperty.RegisterAttached("ChildMinimizedHeight", typeof(double), typeof(DockingManager), new FrameworkPropertyMetadata(20d));

        /// <summary>
        /// Identifies DockingManager.IsFixedSize attached property.
        /// </summary>
        /// <remarks>
        /// This property is used to allow or deny resize of element 
        /// If it is set to true it means element cannot be resized.
        /// The default value is false.
        /// </remarks>
        public static readonly DependencyProperty IsFixedSizeProperty =
            DependencyProperty.RegisterAttached("IsFixedSize", typeof(bool), typeof(DockingManager), new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Identifies DockingManager.IsFixedHeight attached property.
        /// </summary>
        /// <remarks>
        /// This property is used to allow or deny resize of element's height
        /// If it is set to true it means element's height cannot be resized.
        /// The default value is false.
        /// </remarks>
        public static readonly DependencyProperty IsFixedHeightProperty =
            DependencyProperty.RegisterAttached("IsFixedHeight", typeof(bool), typeof(DockingManager), new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Identifies DockingManager.IsFixedWidth attached property.
        /// </summary>
        /// <remarks>
        /// This property is used to allow or deny resize of element's width
        /// If it is set to true it means element's width cannot be resized.
        /// The default value is false.
        /// </remarks>
        public static readonly DependencyProperty IsFixedWidthProperty =
            DependencyProperty.RegisterAttached("IsFixedWidth", typeof(bool), typeof(DockingManager), new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Identifies DockingManager.CanClose attached property.
        /// </summary>
        /// <remarks>
        /// This property is used to allow or deny change element state to DockState.Hidden.
        /// If it is set to false it means element cannot be closed.
        /// The default value is true.
        /// </remarks>
        public static readonly DependencyProperty CanCloseProperty =
            DependencyProperty.RegisterAttached("CanClose", typeof(bool), typeof(DockingManager), new FrameworkPropertyMetadata(true, new PropertyChangedCallback(OnCanCloseChanged), new CoerceValueCallback(CoerceCanClose)));

        /// <summary>
        /// Identifies DockingManager.CanDrag attached property.
        /// </summary>
        /// <remarks>
        /// This property is used to allow or deny dragging the element.
        /// If it is set to false it means element cannot be dragged.
        /// The default value is true.
        /// </remarks>
        public static readonly DependencyProperty CanDragProperty =
            DependencyProperty.RegisterAttached("CanDrag", typeof(bool), typeof(DockingManager), new UIPropertyMetadata(true));

        /// <summary>
        /// Identifies DockingManager.CanDock attached property.
        /// </summary>
        /// <remarks>
        /// This property is used to allow or deny change element state to DockState.Dock.
        /// If it is set to false it means element cannot be docked to any host in docked state.
        /// It can only be docked in float state.
        /// The default value is true.
        /// </remarks>
        public static readonly DependencyProperty CanDockProperty =
            DependencyProperty.RegisterAttached("CanDock", typeof(bool), typeof(DockingManager), new FrameworkPropertyMetadata(true, new PropertyChangedCallback(OnCanDockChanged), new CoerceValueCallback(CoerceCanDock)));

        /// <summary>
        /// Identifies DockingManager.CanMaximize attached property.
        /// </summary>
        /// <remarks>
        /// This property is used to allow or deny change element window state to WindowState.Maximized.
        /// If it is set to false it means element cannot be maximized in dock state.
        /// The default value is true.
        /// </remarks>
        public static readonly DependencyProperty CanMaximizeProperty =
            DependencyProperty.RegisterAttached("CanMaximize", typeof(bool), typeof(DockingManager), new FrameworkPropertyMetadata(true, new PropertyChangedCallback(OnCanMaximizeChanged), new CoerceValueCallback(CoerceCanMaximize)));

        /// <summary>
        /// Identifies DockingManager.MaximizeButtonVisibility attached property.
        /// </summary>
        internal static readonly DependencyProperty MaximizeButtonVisibilityProperty =
    DependencyProperty.RegisterAttached("MaximizeButtonVisibility", typeof(Visibility), typeof(DockingManager), new FrameworkPropertyMetadata(Visibility.Collapsed));

        /// <summary>
        /// Identifies DockingManager.MinimizeButtonVisibility attached property.
        /// </summary>
        internal static readonly DependencyProperty MinimizeButtonVisibilityProperty =
    DependencyProperty.RegisterAttached("MinimizeButtonVisibility", typeof(Visibility), typeof(DockingManager), new FrameworkPropertyMetadata(Visibility.Collapsed));

        /// <summary>
        /// Identifies DockingManager.RestoreButtonVisibilityProperty attached property.
        /// </summary>
        internal static readonly DependencyProperty RestoreButtonVisibilityProperty =
    DependencyProperty.RegisterAttached("RestoreButtonVisibility", typeof(Visibility), typeof(DockingManager), new FrameworkPropertyMetadata(Visibility.Collapsed));

        /// <summary>
        /// Identifies DockingManager.CanMinimize attached property.
        /// </summary>
        /// <remarks>
        /// This property is used to allow or deny change element window state to WindowState.Minimized.
        /// If it is set to false it means element cannot be minimized in dock state.
        /// The default value is true.
        /// </remarks>
        public static readonly DependencyProperty CanMinimizeProperty =
            DependencyProperty.RegisterAttached("CanMinimize", typeof(bool), typeof(DockingManager), new FrameworkPropertyMetadata(true, new PropertyChangedCallback(OnCanMinimizeChanged), new CoerceValueCallback(CoerceCanMinimize)));

        /// <summary>
        /// Identifies DockingManager.CanFloat attached property.
        /// </summary>
        /// <remarks>
        /// This property is used to allow or deny change element state to DockState.Float.
        /// If it is set to false it means element cannot be in float window.
        /// The default value is true.
        /// </remarks>
        public static readonly DependencyProperty CanFloatProperty =
            DependencyProperty.RegisterAttached("CanFloat", typeof(bool), typeof(DockingManager), new FrameworkPropertyMetadata(true, new PropertyChangedCallback(OnCanFloatChanged), new CoerceValueCallback(CoerceCanFloat)));

        /// <summary>
        /// Identifies DockingManager.CanFloat attached property.
        /// </summary>
        /// <remarks>
        /// This property is used to allow or deny change element state to DockState.Document.
        /// If it is set to false it means element cannot be in document window.
        /// The default value is true.
        /// </remarks>
        public static readonly DependencyProperty CanDocumentProperty =
            DependencyProperty.RegisterAttached("CanDocument", typeof(bool), typeof(DockingManager), new FrameworkPropertyMetadata(true, new PropertyChangedCallback(OnCanDocumentChanged), new CoerceValueCallback(CoerceCanDocument)));

        /// <summary>
        /// Identifies DockingManager.NoHeader attached property.
        /// </summary>
        /// <remarks>
        /// This property can be attached to a docking manager child and is used to determine whether
        /// to show header in parent host.
        /// The default value is false.
        /// </remarks>
        public static readonly DependencyProperty NoHeaderProperty =
            DependencyProperty.RegisterAttached("NoHeader", typeof(bool), typeof(DockingManager), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnNoHeaderChanged), new CoerceValueCallback(CoerceNoHeader)));
        
        /// <summary>
        /// Identifies DockingManager.PreviousNoHeader attached property.
        /// </summary>
        /// <remarks>
        /// This property can be attached to a docking manager child.
        /// Actually when the window it moving to FloatingState we set the NoHeader=true. Then we set the NoHeader=false when it again moves to
        /// DockState. So we cant assure the NoHeader property value before the window moves to FloatingState. Thats why we have to maintain 
        /// the NoHeader property value for the child before to set the FloatingState.
        /// The default value is false.
        /// </remarks>
        internal static readonly DependencyProperty PreviousNoHeaderProperty =
            DependencyProperty.RegisterAttached("PreviousNoHeader", typeof(bool), typeof(DockingManager), new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Identifies DockingManager.SideInDockedMode attached property.
        /// </summary>
        /// <remarks>
        /// This property can be attached to a docking manager child and is used to get / set the element DockSide in docked mode.
        /// DockSide means how element is located regards its target and is used for layout.
        /// For all possible cases see <see cref="DockSide"/> enum. The default value is DockSide.Left.
        /// </remarks>
        public static readonly DependencyProperty SideInDockedModeProperty =
            DependencyProperty.RegisterAttached("SideInDockedMode", typeof(DockSide), typeof(DockingManager), new FrameworkPropertyMetadata(DockSide.Left, new PropertyChangedCallback(OnSideInDockedModeChanged), new CoerceValueCallback(OnCoerceSideInDockedMode)));

        /// <summary>
        /// Identifies DockingManager.SideInFloatMode attached property.
        /// </summary>
        /// <remarks>
        /// This property can be attached to a docking manager child and is used to get / set the element DockSide in float mode.
        /// DockSide means how element is located regards its target and is used for layout.
        /// For all possible cases see <see cref="DockSide"/> enum. The default value is DockSide.Left.
        /// </remarks>
        public static readonly DependencyProperty SideInFloatModeProperty =
            DependencyProperty.RegisterAttached("SideInFloatMode", typeof(DockSide), typeof(DockingManager), new FrameworkPropertyMetadata(DockSide.Left, new PropertyChangedCallback(OnSideInFloatModeChanged), new CoerceValueCallback(OnCoerceSideInFloatMode)));

        /// <summary>
        /// Identifies DockingManager.TargetNameInFloatingMode attached property.
        /// </summary>
        /// <remarks>
        /// This property can be attached to a docking manager child and is used to get / set the name of the element's target in float mode.
        /// If name is an empty string it means child is connected directly to a docking manager and it is displayed as a float window, 
        /// otherwise it's connected to its target which is also one of docking manager child with the specified name.
        /// The default value is empty string.
        /// </remarks>
        public static readonly DependencyProperty TargetNameInFloatingModeProperty =
            DependencyProperty.RegisterAttached("TargetNameInFloatingMode", typeof(string), typeof(DockingManager), new FrameworkPropertyMetadata(string.Empty, new PropertyChangedCallback(OnTargetNameInFloatingModeChanged), new CoerceValueCallback(OnCoerceTargetNameInFloatingMode)));

        /// <summary>
        /// Identifies DockingManager.TargetNameInDockedMode attached property.
        /// </summary>
        /// <remarks>
        /// This property can be attached to a docking manager child and is used to get / set the name of the element's target in docked mode.
        /// If name is an empty string it means child is connected directly to a docking manager main host, otherwise it's connected to its target 
        /// which is also one of docking manager child with the specified name.
        /// The default value is empty string.
        /// </remarks>
        public static readonly DependencyProperty TargetNameInDockedModeProperty =
            DependencyProperty.RegisterAttached("TargetNameInDockedMode", typeof(string), typeof(DockingManager), new FrameworkPropertyMetadata(string.Empty, new PropertyChangedCallback(OnTargetNameInDockedModeChanged)));

        internal static readonly DependencyProperty TargetNameInAutoHideModeProperty =
           DependencyProperty.RegisterAttached("TargetNameInAutoHideMode", typeof(string), typeof(DockingManager), new FrameworkPropertyMetadata(string.Empty));

        /// <summary>
        /// Identifies DockingManager.NoDock attached property.
        /// </summary>
        /// <remarks>
        /// This property can be attached to a docking manager child and is used to allow or deny docking the element.
        /// If it is set to true element cannot be dock to any host and can exist only in float state as a separate window.
        /// </remarks>
        public static readonly DependencyProperty NoDockProperty =
            DependencyProperty.RegisterAttached("NoDock", typeof(bool), typeof(DockingManager), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnNoDockChanged), new CoerceValueCallback(CoerceNoDock)));

        /// <summary>
        /// Identifies DockingManager.FloatingWindowRect attached property.
        /// </summary>
        /// <remarks>
        /// This property can be attached to a docking manager child and is used to determine this size and location
        /// of the floating window where child is hosted.
        /// The default value is empty rect.
        /// </remarks>
        public static readonly DependencyProperty FloatingWindowRectProperty =
            DependencyProperty.RegisterAttached("FloatingWindowRect", typeof(Rect), typeof(DockingManager), new FrameworkPropertyMetadata(Rect.Empty, new PropertyChangedCallback(OnFloatingWindowRectChanged), new CoerceValueCallback(CoerceFloatingWindowRect)));


        internal static readonly DependencyProperty PreviousFloatingWindowRectProperty =
            DependencyProperty.RegisterAttached("PreviousFloatingWindowRect", typeof (Rect), typeof (DockingManager),new FrameworkPropertyMetadata(Rect.Empty));

        /// <summary>
        /// Identifies DockingManager.DockToFill attached property.
        /// </summary>
        /// <remarks>
        /// This property can be attached to a docking manager child and is used to specify how to operate with child size in its container.
        /// If it's set to true element will use all available space instead if its desired size. 
        /// The default value is false.
        /// </remarks>
        public static readonly DependencyProperty DockToFillProperty =
            DependencyProperty.RegisterAttached("DockToFill", typeof(bool), typeof(DockingManager), new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Identifies DockingManager.CustomMenuItems attached property.
        /// </summary>
        /// <remarks>
        /// This property can be attached to a docking manager child and gives an ability to add 
        /// some extra menu items to the context menu which can be called for the element. 
        /// This can easily extend GUI functionality by using that custom menu items to provide actions form the sample.
        /// </remarks>
        public static readonly DependencyProperty CustomMenuItemsProperty =
            DependencyProperty.RegisterAttached("CustomMenuItems", typeof(CustomMenuItemCollection), typeof(DockingManager), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits,new PropertyChangedCallback(OnCustomMenuItemsChanged)));

        /// <summary>
        /// Identifies DockingManager.DocumentTabItemContextMenuItems attached property
        /// </summary>
        public static readonly DependencyProperty DocumentTabItemContextMenuItemsProperty =
          DependencyProperty.RegisterAttached("DocumentTabItemContextMenuItems", typeof(DocumentTabItemMenuItemCollection), typeof(DockingManager), new FrameworkPropertyMetadata(new DocumentTabItemMenuItemCollection(), FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// Identifies DockingManager.FloatWindowContextMenuItems attached property
        /// </summary>
        public static readonly DependencyProperty FloatWindowContextMenuItemsProperty =
          DependencyProperty.RegisterAttached("FloatWindowContextMenuItems", typeof(CustomMenuItemCollection), typeof(DockingManager), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// Identifies DockingManager.DockWindowContextMenuItems attached property
        /// </summary>
        public static readonly DependencyProperty DockWindowContextMenuItemsProperty =
          DependencyProperty.RegisterAttached("DockWindowContextMenuItems", typeof(CustomMenuItemCollection), typeof(DockingManager), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// Identifies DockingManager.CollapseDefaultContextMenuItemsInFloat attached property
        /// </summary>
        public static readonly DependencyProperty CollapseDefaultContextMenuItemsInFloatProperty =
          DependencyProperty.RegisterAttached("CollapseDefaultContextMenuItemsInFloat", typeof(bool), typeof(DockingManager), new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Identifies DockingManager.CollapseDefaultContextMenuItemsInDock attached property
        /// </summary>
        public static readonly DependencyProperty CollapseDefaultContextMenuItemsInDockProperty =
          DependencyProperty.RegisterAttached("CollapseDefaultContextMenuItemsInDock", typeof(bool), typeof(DockingManager), new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Identifies DockingManager.CollapseDefaultContextMenuItemsInDocumentTab attached property
        /// </summary>
        public static readonly DependencyProperty CollapseDefaultContextMenuItemsInDocumentTabProperty =
          DependencyProperty.RegisterAttached("CollapseDefaultContextMenuItemsInDocumentTab", typeof(bool), typeof(DockingManager), new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Identifies DockingManager.CollapseDefaultContextMenuItems attached property
        /// </summary>
        public static readonly DependencyProperty CollapseDefaultContextMenuItemsProperty =
          DependencyProperty.Register("CollapseDefaultContextMenuItems", typeof(bool), typeof(DockingManager), new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Represents the CollapseDefaultTabListContextMenuItemsProperty 
        /// </summary>
        public static readonly DependencyProperty CollapseDefaultTabListContextMenuItemsProperty =
             DependencyProperty.Register("CollapseDefaultTabListContextMenuItems", typeof(bool), typeof(DockingManager), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnCollapseDefaultTabListContextMenuItemsChanged)));

        /// <summary>
        /// Represents the TabListContextMenuProperty 
        /// </summary>
        public static readonly DependencyProperty TabListContextMenuItemsProperty =
             DependencyProperty.Register("TabListContextMenuItems", typeof(DocumentTabItemMenuItemCollection), typeof(DockingManager), new FrameworkPropertyMetadata(new DocumentTabItemMenuItemCollection()));

        /// <summary>
        /// Identifies DockingManager.DockTabAlignment dependency property.
        /// </summary>
        public static readonly DependencyProperty DockTabAlignmentProperty =
            DependencyProperty.Register("DockTabAlignment", typeof(Dock), typeof(DockingManager), new UIPropertyMetadata(Dock.Bottom));

        /// <summary>
        /// Identifies DockingManager.IsEnableHotTracking dependency property.
        /// </summary>
        public static readonly DependencyProperty IsEnableHotTrackingProperty =
            DependencyProperty.Register("IsEnableHotTracking", typeof(bool), typeof(DockingManager), new UIPropertyMetadata(false));

        /// <summary>
        /// Identifies DockingManager.MaximizeButtonEnabled dependency property.
        /// </summary>
        public static readonly DependencyProperty MaximizeButtonEnabledProperty =
            DependencyProperty.Register("MaximizeButtonEnabled", typeof(bool), typeof(DockingManager), new UIPropertyMetadata(false));

        /// <summary>
        /// Identifies DockingManager.MinimizeButtonEnabled dependency property.
        /// </summary>
        public static readonly DependencyProperty MinimizeButtonEnabledProperty =
            DependencyProperty.Register("MinimizeButtonEnabled", typeof(bool), typeof(DockingManager), new UIPropertyMetadata(false));

        /// <summary>
        /// Identifies DockingManager.MaximizeMode dependency property.
        /// </summary>
        public static readonly DependencyProperty MaximizeModeProperty =
            DependencyProperty.Register("MaximizeMode", typeof(MaximizeMode), typeof(DockingManager), new UIPropertyMetadata(MaximizeMode.Default, new PropertyChangedCallback(OnMaximizeModeChanged)));

        /// <summary>
        /// Identifies DockingManager.IsSelectedTab attached property.
        /// </summary>
        /// <remarks>
        /// This property can be attached to a docking manager child and is used to specify 
        /// whether element will be selected if it is in tabbed side.
        /// The default value is false.
        /// </remarks>
        public static readonly DependencyProperty IsSelectedTabProperty =
            DependencyProperty.RegisterAttached("IsSelectedTab", typeof(bool), typeof(DockingManager), new UIPropertyMetadata(false, new PropertyChangedCallback(OnIsSelectedTabChanged)));

        /// <summary>
        /// Identifies DockingManager.PrevChild attached property
        /// </summary>
        internal static readonly DependencyProperty PrevChildProperty =
           DependencyProperty.RegisterAttached("PrevChild", typeof(PrevChildInfo), typeof(DockingManager), new UIPropertyMetadata(null));

        /// <summary>
        /// Identifies DockingManager.IsActiveWindow attached property.
        /// </summary>
        public static readonly DependencyProperty IsActiveWindowProperty =
            DependencyProperty.RegisterAttached("IsActiveWindow", typeof(bool), typeof(DockingManager),new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsActiveWindowChanged)));

        /// <summary>
        /// Identifies DockingManager.HasFocus attached property.
        /// </summary>
        /// <remarks>
        /// This property can be attached to a docking manager child and is used to specify whether element has focus.
        /// Only one child can have focus at the same time.
        /// the default value is false.
        /// </remarks>
        protected static readonly DependencyProperty HasFocusProperty =
            DependencyProperty.RegisterAttached("HasFocus", typeof(bool), typeof(DockingManager), new UIPropertyMetadata(false));

        /// <summary>
        /// Identifies DockingManager.CloseTabs dependency property.
        /// </summary>        
        public static readonly DependencyProperty CloseTabsProperty =
            DependencyProperty.Register("CloseTabs", typeof(CloseTabsMode), typeof(DockingManager), new FrameworkPropertyMetadata(CloseTabsMode.CloseActive, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Identifies DockingManager.AutoHideTabsMode dependency property.
        /// </summary>
        public static readonly DependencyProperty AutoHideTabsModeProperty =
           DependencyProperty.Register("AutoHideTabsMode", typeof(AutoHideTabsMode), typeof(DockingManager), new UIPropertyMetadata(AutoHideTabsMode.AutoHideGroup));

        /// <summary>
        /// Identifies DockingManager.AutoHideVisibility dependency property.
        /// </summary>
        public static readonly DependencyProperty AutoHideVisibilityProperty =
            DependencyProperty.Register("AutoHideVisibility", typeof(bool), typeof(DockingManager), new FrameworkPropertyMetadata(true, new PropertyChangedCallback(OnAutoHideVisibilityChanged)));

        /// <summary>
        /// Identifies DockingManager.AutoHideAnimationMode dependency property.
        /// </summary>
        public static readonly DependencyProperty AutoHideAnimationModeProperty =
            DependencyProperty.Register("AutoHideAnimationMode", typeof(AutoHideAnimationMode), typeof(DockingManager), new UIPropertyMetadata(AutoHideAnimationMode.Slide));

        /// <summary>
        /// Identifies DockingManager.AnimationDelay attached property.
        /// </summary>
        /// <remarks>
        ///  This property can be attached to a docking manager child and is used to determine
        ///  the amount of time for the auto hide animation.
        ///  The default value is 200 milliseconds
        /// </remarks>
        public static readonly DependencyProperty AnimationDelayProperty =
            DependencyProperty.RegisterAttached("AnimationDelay", typeof(Duration), typeof(DockingManager), new FrameworkPropertyMetadata(new Duration(TimeSpan.FromMilliseconds(200)), FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// Identifies DockingManager.CanAutoHide attached property.
        /// </summary>
        /// <remarks>
        /// This property can be attached to a docking manager child and is used to allow or deny change element state to DockState.Autohidden.
        /// If it is set to false it means element cannot be auto hidden
        /// The default value is true.
        /// </remarks>
        public static readonly DependencyProperty CanAutoHideProperty =
            DependencyProperty.RegisterAttached("CanAutoHide", typeof(bool), typeof(DockingManager), new FrameworkPropertyMetadata(true, new PropertyChangedCallback(OnCanAutoHideChanged), new CoerceValueCallback(CoerceCanAutoHide)));


        internal static readonly DependencyProperty IsSwappedProperty =
    		DependencyProperty.RegisterAttached("IsSwapped", typeof(bool), typeof(DockingManager), new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Identifies EnableScrollableSidePanel dependency property.
        /// </summary>
        public static readonly DependencyProperty EnableScrollableSidePanelProperty =
        DependencyProperty.Register("EnableScrollableSidePanel", typeof(bool), typeof(DockingManager), new FrameworkPropertyMetadata(false));
       
        internal static readonly DependencyProperty closefloatproperty =
           DependencyProperty.RegisterAttached("Closefloat", typeof(bool), typeof(DockingManager), new FrameworkPropertyMetadata(false));



        public static bool GetCanFloatMaximize(DependencyObject obj)
        {
            return (bool)obj.GetValue(CanFloatMaximizeProperty);
        }

        public static void SetCanFloatMaximize(DependencyObject obj, bool value)
        {
            obj.SetValue(CanFloatMaximizeProperty, value);
        }

        /// <summary>
        /// Property supports only NativeFloatWindow maximization.
        /// </summary>
        
        public static readonly DependencyProperty CanFloatMaximizeProperty =
            DependencyProperty.RegisterAttached("CanFloatMaximize", typeof(bool), typeof(DockingManager), new UIPropertyMetadata(false));        


        /// <summary>
        /// Identifies DockingManager.IsAddedElement attached property.
        /// </summary>
        internal static readonly DependencyProperty IsAddedElementProperty =
            DependencyProperty.RegisterAttached("IsAddedElement", typeof(bool), typeof(DockingManager), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// Identifies DockingManager.IsFroze attached property.
        /// </summary>
        public static readonly DependencyProperty IsFrozeProperty =
            DependencyProperty.RegisterAttached("IsFroze", typeof(bool), typeof(DockingManager), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsFrozeChanged)));

       

        /// <summary>
        /// Identifies DockingManager.PreviousState attached property.
        /// </summary>
        /// <remarks>
        /// This property can be attached to a docking manager child and is used to store element state before closing.
        /// Use static method RestoreElement from class <see cref="DockingManager"/> to set the previous state.
        /// The default value is DockState.Dock.
        /// </remarks>
        protected static readonly DependencyProperty PreviousStateProperty =
            DependencyProperty.RegisterAttached("PreviousState", typeof(DockState), typeof(DockingManager), new UIPropertyMetadata(DockState.Dock));

        /// <summary>
        /// Property to identify the DockInfoPropertyKey
        /// </summary>
        protected static readonly DependencyPropertyKey DockInfoPropertyKey =
            DependencyProperty.RegisterAttachedReadOnly("DockInfo", typeof(DockInfoInternal), typeof(DockingManager), new UIPropertyMetadata(null));

        /// <summary>
        /// Property to identify the DockInfoProperty
        /// </summary>
        protected static readonly DependencyProperty DockInfoProperty = DockInfoPropertyKey.DependencyProperty;


        /// <summary>
        /// Property to identify the SideRelativetoContainerProperty
        /// </summary>

        internal static readonly DependencyProperty SideRelativetoContainerProperty =
           DependencyProperty.RegisterAttached("SideRelativetoContainer", typeof(DockSide), typeof(DockingManager));

        /// <summary>
        /// Property to identify the PreviousTargetInDockMode
        /// </summary>

        internal static readonly DependencyProperty TargetInDockModeTabProperty =
           DependencyProperty.RegisterAttached("TargetInDockModeTab", typeof(String), typeof(DockingManager));

        internal static readonly DependencyProperty TabControlProperty =
            DependencyProperty.RegisterAttached("TabControl", typeof(TabControlExt), typeof(DockingManager));

        /// <summary>
        /// Property to identify the PreviousTargetInDockMode
        /// </summary>

        internal static readonly DependencyProperty PreviousTargetInDockModeProperty =
           DependencyProperty.RegisterAttached("PreviousTargetInDockMode", typeof(String), typeof(DockingManager), new FrameworkPropertyMetadata(string.Empty, new PropertyChangedCallback(OnPreviousTargetInDockModeChanged)));

        /// <summary>
        /// Property to identify the IsDragged
        /// </summary>
        internal static readonly DependencyProperty IsDraggedProperty =
         DependencyProperty.RegisterAttached("IsDragged", typeof(bool), typeof(DockingManager));
        

        /// <summary>
        /// Property to identify the PreviousParentSideInDockMode
        /// </summary>
        internal static readonly DependencyProperty PreviousParentSideInDockModeProperty =
         DependencyProperty.RegisterAttached("PreviousParentSideInDockMode", typeof(String), typeof(DockingManager));
        
        /// <summary>
        /// Property to identify the IsTargetChangedProperty
        /// </summary>
        internal static readonly DependencyProperty IsTargetChangedProperty =
         DependencyProperty.RegisterAttached("IsTargetChanged", typeof(bool), typeof(DockingManager));        


        /// <summary>
        /// Property to identify the PreviousSideInDockModeProperty
        /// </summary>
        
        internal static readonly DependencyProperty PreviousSideInDockModeProperty =
           DependencyProperty.RegisterAttached("PreviousSideInDockMode", typeof(DockSide), typeof(DockingManager));        
        /// <summary>
        /// Property to identify the PreviousIndexInDockModeProperty
        /// </summary>
        internal static readonly DependencyProperty PreviousIndexInDockModeProperty =
           DependencyProperty.RegisterAttached("PreviousIndexInDockMode", typeof(int), typeof(DockingManager));
        /// <summary>
        /// Property to identify the TabParentName
        /// </summary>
        internal static readonly DependencyProperty TabParentProperty =
           DependencyProperty.RegisterAttached("TabParent", typeof(String), typeof(DockingManager));
        
        /// <summary>
        /// Property to identify the PreviousChildElementsProperty
        /// </summary>
        internal static readonly DependencyProperty PreviousChildElementsProperty =
           DependencyProperty.RegisterAttached("PreviousChildElements", typeof(List<String>), typeof(DockingManager));
        /// <summary>
        /// Property to identify the IndexInDockMode
        /// </summary>
        internal static readonly DependencyProperty IndexInDockModeProperty =
            DependencyProperty.RegisterAttached("IndexInDockMode", typeof(int), typeof(DockingManager));

        /// <summary>
        /// Property to identify the IndexInFloatMode
        /// </summary>
        internal static readonly DependencyProperty IndexInFloatModeProperty =
            DependencyProperty.RegisterAttached("IndexInFloatMode", typeof(int), typeof(DockingManager));

        /// <summary>
        /// Property to identify the UseDocumentContainerProperty
        /// </summary>
        public static readonly DependencyProperty UseDocumentContainerProperty =
            DependencyProperty.Register("UseDocumentContainer", typeof(bool), typeof(DockingManager), new PropertyMetadata(false, new PropertyChangedCallback(OnUseDocumentContainerChanged)));

        // Using a DependencyProperty as the backing store for IsLazyLoaded.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsLazyLoadedProperty =
            DependencyProperty.Register("IsLazyLoaded", typeof(bool), typeof(DockingManager), new PropertyMetadata(false));

        
        /// <summary>
        /// Identifies DockingManager.UseInteropCompatibilityMode dependency property.
        /// </summary>
        public static readonly DependencyProperty UseInteropCompatibilityModeProperty =
            DependencyProperty.Register("UseInteropCompatibilityMode", typeof(bool), typeof(DockingManager));

        /// <summary>
        /// Indicate that whether tabpreview controls.
        /// </summary>
        public static readonly DependencyProperty IsTabPreviewEnabledProperty = DependencyProperty.Register("IsTabPreviewEnabled", typeof(bool), typeof(DockingManager), new FrameworkPropertyMetadata(true));

        /// <summary>
        /// Identifies DockingManager.HideTDIHeaderOnSongleChild Property
        /// </summary>
        public static readonly DependencyProperty HideTDIHeaderOnSingleChildProperty = DependencyProperty.Register("HideTDIHeaderOnSingleChild", typeof(bool), typeof(DockingManager), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnHideTDIHeaderOnSingleChildChanged)));
        

        /// <summary>
        /// Identifies DockingManager.ActiveWindowProperty dependency property.
        /// </summary>
        public static readonly DependencyProperty ActiveWindowProperty =
            DependencyProperty.Register("ActiveWindow", typeof(FrameworkElement), typeof(DockingManager), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnActiveWindowChanged), new CoerceValueCallback(OnCoerceActiveWindow)));

        /// <summary>
        /// Identifies DockingManager.SidePanelDockProperty dependency property.
        /// </summary>
        internal static readonly DependencyProperty SidePanelDockProperty =
            DependencyProperty.RegisterAttached("SidePanelDock", typeof(Dock), typeof(DockingManager));
        /// <summary>
        /// Identifies DockingManager.IsAnimationEnabledOnMouseOverProperty dependency property.
        /// </summary>
        public static readonly DependencyProperty IsAnimationEnabledOnMouseOverProperty =
            DependencyProperty.Register("IsAnimationEnabledOnMouseOver", typeof(bool), typeof(DockingManager), new PropertyMetadata(true));

        /// <summary>
        /// Identifies DockingManager.DockFillProperty dependency property.
        /// </summary>
        public static readonly DependencyProperty DockFillProperty =
            DependencyProperty.Register("DockFill", typeof(bool), typeof(DockingManager), new PropertyMetadata(false, OnDockFillPropertyChanged));

        /// <summary>
        /// Identifies DockingManager.DockFillDocumentMode dependency property.
        /// </summary>
        public static readonly DependencyProperty DockFillDocumentModeProperty =
            DependencyProperty.Register("DockFillDocumentMode", typeof(DockFillDocumentMode), typeof(DockingManager), new PropertyMetadata(DockFillDocumentMode.Fill, OnDockFillDocumentModePropertyChanged));

        /// <summary>
        /// Identifies DockingManager.SizeChangeOnMaximize dependency property.
        /// </summary>
        public static readonly DependencyProperty SizeChangeOnMaximizeProperty =
            DependencyProperty.Register("SizeChangeOnMaximize", typeof(SizeChangeOnMaximizeMode), typeof(DockingManager), new PropertyMetadata(SizeChangeOnMaximizeMode.Normal));

        /// <summary>
        /// Identifies DockingManager.MaximizeButtonMode dependency property.
        /// </summary>
        public static readonly DependencyProperty MaximizeButtonModeProperty =
            DependencyProperty.Register("MaximizeButtonMode", typeof(VisibilityMode), typeof(DockingManager), new PropertyMetadata(VisibilityMode.Collapse));

        /// <summary>
        /// Identifies DockingManager.ContainerSplitterResize dependency property.
        /// </summary>
        public static readonly DependencyProperty ContainerSplitterResizeProperty =
            DependencyProperty.Register("ContainerSplitterResize", typeof(SplitterResizeMode), typeof(DockingManager), new PropertyMetadata(SplitterResizeMode.AllChildren));

        /// <summary>
        /// Identifies DockingManager.DockFillModeProperty dependency property.
        /// </summary>
        public static readonly DependencyProperty DockFillModeProperty =
            DependencyProperty.RegisterAttached("DockFillMode", typeof(DockFillModes), typeof(DockingManager), new PropertyMetadata(DockFillModes.Default));

        /// <summary>
        /// Identifies DockingManager.DockAbilityProperty dependency property.
        /// </summary>
        public static readonly DependencyProperty DockAbilityProperty =
            DependencyProperty.RegisterAttached("DockAbility", typeof(DockAbility), typeof(DockingManager), new PropertyMetadata(DockAbility.All));

        /// <summary>
        /// Identifies DockingManager.ClientControlProperty dependency property.
        /// </summary>
        public static readonly DependencyProperty ClientControlProperty =
            DependencyProperty.Register("ClientControl", typeof(UIElement), typeof(DockingManager), new PropertyMetadata(OnClientControlChanged));

        /// <summary>
        /// Identifies DockingManager.UseAdornerDragProviderProperty dependency property.
        /// </summary>
        public static readonly DependencyProperty UseAdornerDragProviderProperty =
            DependencyProperty.Register("UseAdornerDragProvider", typeof(bool), typeof(DockingManager));

        /// <summary>
        /// Identifies DockingManager.UseAdornerFloatWindowProperty dependency property.
        /// </summary>
        public static readonly DependencyProperty UseAdornerFloatWindowProperty =
             DependencyProperty.Register("UseAdornerFloatWindow", typeof(bool), typeof(DockingManager), new PropertyMetadata(false, OnUseAdornerFloatWindowChanged));


        private static void OnUseAdornerFloatWindowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DockingManager docking = (DockingManager)d;
            if ((bool)e.NewValue)
            {
                if (docking.UseNativeFloatWindow)
                    docking.UseAdornerFloatWindow = false;
            }
        }
        /// <summary>
        /// Identifies DockingManager.DockingManagerProperty dependency property.
        /// </summary>
        public static readonly DependencyProperty DockingManagerProperty =
            DependencyProperty.RegisterAttached("DockingManager", typeof(DockingManager), typeof(DockingManager), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// Identifies DockingManager.InternalDataContext dependency property.
        /// </summary>
        public static readonly DependencyProperty InternalDataContextProperty =
            DependencyProperty.RegisterAttached("InternalDataContext", typeof(FrameworkElement), typeof(DockingManager), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));
             

        /// <summary>
        /// Identifies DockingManager.AnimateOnNewItemAdded dependency property.
        /// </summary>
        public static readonly DependencyProperty AnimateOnNewItemAddedProperty =
            DependencyProperty.RegisterAttached("AnimateOnNewItemAdded", typeof(bool), typeof(DockingManager), new UIPropertyMetadata(true));

        public bool UseOuterDockAbility
        {
            get { return (bool)GetValue(UseOuterDockAbilityProperty); }
            set { SetValue(UseOuterDockAbilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for UseOuterDockAbility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UseOuterDockAbilityProperty =
            DependencyProperty.Register("UseOuterDockAbility", typeof(bool), typeof(DockingManager), new UIPropertyMetadata(false));

        public bool CanNestedFloat
        {
            get { return (bool)GetValue(CanNestedFloatProperty); }
            set { SetValue(CanNestedFloatProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CanNestedFloat.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CanNestedFloatProperty =
            DependencyProperty.Register("CanNestedFloat", typeof(bool), typeof(DockingManager), new UIPropertyMetadata(true));
        
        public static OuterDockAbility GetOuterDockAbility(DependencyObject obj)
        {
            return (OuterDockAbility)obj.GetValue(OuterDockAbilityProperty);
        }

        public static void SetOuterDockAbility(DependencyObject obj, OuterDockAbility value)
        {
            obj.SetValue(OuterDockAbilityProperty, value);
        }

        // Using a DependencyProperty as the backing store for OuterDockAbility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OuterDockAbilityProperty =
            DependencyProperty.RegisterAttached("OuterDockAbility", typeof(OuterDockAbility), typeof(DockingManager), new UIPropertyMetadata(OuterDockAbility.All));




        // Using a DependencyProperty as the backing store for PreviousContainerDesiredSize.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty PreviousContainerDesiredSizeProperty =
            DependencyProperty.RegisterAttached("PreviousContainerDesiredSize", typeof(Size), typeof(DockingManager), new FrameworkPropertyMetadata(new Size(0, 0)));




        // Using a DependencyProperty as the backing store for PreviousDesiredWidthInDockedMode.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty PreviousDesiredWidthInDockedModeProperty =
            DependencyProperty.RegisterAttached("PreviousDesiredWidthInDockedMode", typeof(double), typeof(DockingManager), new FrameworkPropertyMetadata(0d));



        // Using a DependencyProperty as the backing store for PreviousDesiredHeightInDockedMode.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty PreviousDesiredHeightInDockedModeProperty =
            DependencyProperty.RegisterAttached("PreviousDesiredHeightInDockedMode", typeof(double), typeof(DockingManager), new FrameworkPropertyMetadata(0d));


        /// <summary>
        /// Disabling Dynamic TabGroup Creation while dragging tabitem at right and bottom edge of container.
        /// </summary>
        public static readonly DependencyProperty TabGroupEnabledProperty = DependencyProperty.Register("TabGroupEnabled", typeof(bool), typeof(DockingManager), new FrameworkPropertyMetadata(true));


        /// <summary>
        /// Using Dependency Property for Disabling dragging for particular tab item.
        /// </summary>
        public static readonly DependencyProperty CanDragTabProperty =DependencyProperty.RegisterAttached("CanDragTab", typeof(bool), typeof(DockingManager),  new FrameworkPropertyMetadata(true));



        public Style NativeWindowStyle
        {
            get
            {
                return (Style)GetValue(NativeWindowStyleProperty);
            }
            set
            {
                SetValue(NativeWindowStyleProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for NativeWindowStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NativeWindowStyleProperty =
            DependencyProperty.Register("NativeWindowStyle", typeof(Style), typeof(DockingManager), new FrameworkPropertyMetadata(null,OnStyleChanged));

        private static void OnStyleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {

        }

        internal static readonly DependencyProperty ZorderInFloatModeProperty =
            DependencyProperty.RegisterAttached("ZorderInFloatMode", typeof(int), typeof(DockingManager));

        public static int GetZorderInFloatMode(DependencyObject obj)
        {
            return (int)obj.GetValue(DockingManager.ZorderInFloatModeProperty);
        }
        public static void SetZorderInFloatMode(DependencyObject obj, int value)
        {
            obj.SetValue(ZorderInFloatModeProperty, value);
        }

        public static Rect GetPreviousFloatingWindowRect(DependencyObject obj)
        {
            return (Rect) obj.GetValue(DockingManager.PreviousFloatingWindowRectProperty);
        }

        public static void SetPreviousFloatingWindowRect(DependencyObject obj, Rect value)
        {
            obj.SetValue(PreviousFloatingWindowRectProperty, value);
        }


        internal static readonly DependencyProperty FloatWindowStateProperty =
            DependencyProperty.RegisterAttached("FloatWindowState", typeof(WindowState), typeof(DockingManager), new FrameworkPropertyMetadata(WindowState.Normal));


        public static WindowState GetFloatWindowState(DependencyObject obj)
        {
            return (WindowState)obj.GetValue(DockingManager.FloatWindowStateProperty);
        }

        public static void SetFloatWindowState(DependencyObject obj, WindowState value)
        {
            obj.SetValue(FloatWindowStateProperty, value);
        }

        
        #endregion

        #region Commands
        /// <summary>
        /// Represents move to Floating state.
        /// </summary>
        internal static readonly RoutedCommand FloatingCommand
            = new RoutedCommand("Floating", typeof(DockingManager));

        /// <summary>
        /// Represents move to Dockable state.
        /// </summary>
        internal static readonly RoutedCommand DockableCommand
            = new RoutedCommand("Dockable", typeof(DockingManager));
        #endregion
    }
}