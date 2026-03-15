// <copyright file="DockHeaderPresenter.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Control that is used to represent the header of the docked element.
    /// </summary>
    /// <remarks>
    /// You can use DockHeaderPresenter class to make header of the dock window.
    /// When you create some dock window, there will be header inside.
    /// DockHeaderPresenter can be useful when you override some template, for example template of the <see cref="SidePanel"/>.
    /// </remarks>
    /// <example>
    /// <para/>This example shows how to use DockHeaderPresenter class in XAML.
    /// <code language="XAML">
    /// <![CDATA[
    /// <Syncfusion:DockHeaderPresenter x:Name="PART_Header" DockPanel.Dock="Top" IsRichHeader="False" 
    /// Style="{Binding Path=(Syncfusion:DockingManager.DockHeaderStyle)
    /// , RelativeSource={RelativeSource AncestorType={x:Type Syncfusion:DockingManager}}}"
    /// IsTemplateParenKeyboardFocusWithin="{TemplateBinding IsShowedFocusedItem}"/>
    /// ]]>
    /// </code>
    /// </example>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public sealed class DockHeaderPresenter : Control
    {
        #region Constants
        /// <summary>
        /// Indicates distance start drag.
        /// </summary>
        private const double DistanceStartDrag = 3;

        /// <summary>
        /// Indicates hide menu item name.
        /// </summary>
        private const string HideMenuItemName = "PART_HideMenuItem";

        /// <summary>
        /// Indicates floating menu item name.
        /// </summary>
        private const string FloatingMenuItemName = "PART_FloatingMenuItem";

        /// <summary>
        /// Indicates dockable menu item name.
        /// </summary>
        private const string DockableMenuItemName = "PART_DockableMenuItem";

        /// <summary>
        /// Indicates tabbed menu item name.
        /// </summary>
        private const string TabbedMenuItemName = "PART_TabbedMenuItem";

        /// <summary>
        /// Indicates auto hide menu item.
        /// </summary>
        private const string AutoHideMenuItemName = "PART_AutoHideMenuItem";

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This constant holds the name of one of the items. 
        /// </summary>
        private const string MaximizeMenuItemName = "PART_MaximizeMenuItem";

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This constant holds the name of one of the items. 
        /// </summary>
        private const string MinimizeMenuItemName = "PART_MinimizeMenuItem";

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This constant holds the name of one of the items. 
        /// </summary>
        private const string RestoreMenuItemName = "PART_RetoreMenuItem";


        /// <summary>
        /// Indicates context menu name.
        /// </summary>
        private const string ContextMenuName = "PART_ContextMenu";
        #endregion

        #region Private member
        /// <summary>
        /// Indicates drag start point.
        /// </summary>
        private Point m_dragStart;

        /// <summary>
        /// Indicates docked element tabbed host.
        /// </summary>
        private DockedElementTabbedHost m_host = null;

        /// <summary>
        /// Indicates side panel.
        /// </summary>
        private SidePanel m_panel = null;

        /// <summary>
        /// Indicates pressed or not.
        /// </summary>
        private bool m_pressed;

        /// <summary>
        /// Indicates context menu.
        /// </summary>
        private ContextMenu m_contextMenu;

        internal ToggleButton m_awlButton;

        internal ToggleButton maxBtn;

        internal ToggleButton minBtn;

        internal ToggleButton restoreBtn;

        //private bool m_TouchDown;

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes static members of the <see cref="DockHeaderPresenter"/> class.
        /// </summary>
        static DockHeaderPresenter()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DockHeaderPresenter), new FrameworkPropertyMetadata(typeof(DockHeaderPresenter)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DockHeaderPresenter"/> class.
        /// </summary>
        public DockHeaderPresenter()
        {
            IsVisibleChanged += new DependencyPropertyChangedEventHandler(DockHeaderPresenter_IsVisibleChanged);
            Loaded += DockHeaderPresenter_Loaded;
            Unloaded += new RoutedEventHandler(DockHeaderPresenter_Unloaded);
        }

        void DockHeaderPresenter_Loaded(object sender, RoutedEventArgs e)
        {
            if (InternalDataContext != null)
                DockingManager = DockingManager.ResolveManager(InternalDataContext);
            if (DockingManager == null)
                DockingManager = VisualUtils.FindAncestor(this as Visual, typeof(DockingManager)) as DockingManager;
        }

        void DockHeaderPresenter_Unloaded(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
            if(m_contextMenu!=null)
            m_contextMenu.Items.Clear();
        }

        /// <summary>
        /// Handles the IsVisibleChanged event of the DockHeaderPresenter control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        void DockHeaderPresenter_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            //throw new NotImplementedException();
        }
        #endregion

        #region Commands

        /// <summary>
        /// Represents the Close command, which requests that element be moved to hidden state.
        /// </summary>
        public static RoutedUICommand CloseCommand = new RoutedUICommand("Close", "Close", typeof(DockHeaderPresenter));

        /// <summary>
        /// Represents the OpenContextMenu command, which requests that context menu be opened on the header.
        /// </summary>
        public static RoutedUICommand OpenContextMenuCommand = new RoutedUICommand("OpenContextMenu", "OpenContextMenu", typeof(DockHeaderPresenter));

        /// <summary>
        /// Represents the ChangeAwlState command, which requests that element be moved to auto hidden or dock state.
        /// </summary>
        public static RoutedUICommand ChangeAwlStateCommand = new RoutedUICommand("ChangeAwlState", "ChangeAwlState", typeof(DockHeaderPresenter));

        /// <summary>
        /// Represents the MaximizeState command, which requests that element be moved to maximize window state.
        /// </summary>
        public static RoutedUICommand MaximizeStateCommand = new RoutedUICommand("MaximizeState", "MaximizeState", typeof(DockHeaderPresenter));

        /// <summary>
        /// Represents the MinimizeState command, which requests that element be moved to Minimize window state.
        /// </summary>
        public static RoutedUICommand MinimizeStateCommand = new RoutedUICommand("MinimizeState", "MinimizeState", typeof(DockHeaderPresenter));

        /// <summary>
        /// Represents the RestoreState command, which requests that element be moved to Restore window state.
        /// </summary>
        public static RoutedUICommand RestoreStateCommand = new RoutedUICommand("RestoreState", "RestoreState", typeof(DockHeaderPresenter));

        #endregion

        #region Events

        /// <summary>
        /// Event that is raised when IsContextMenuOpen property is changed.
        /// </summary>
        internal event PropertyChangedCallback IsContextMenuOpenChanged;
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether state can be changed after double click.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="bool"/>
        /// Provides ProcessDoubleClick value for the <see cref="DockHeaderPresenter"/>.
        /// </value>
        /// <remarks>
        /// ProcessDoubleClick property gets or sets the value indicating whether double click on the header of the dock window can be processed.
        /// Default value of this property is true.
        /// </remarks>
        /// <example>
        /// <para/>When you override <see cref="DockHeaderPresenter"/> template, you can use this property to set when double click on header can be processed.
        /// This example shows how to use ProcessDoubleClick property in XAML. 
        /// <code language="XAML">
        /// <![CDATA[
        ///     <Trigger Property="utilsOuter:SkinStorage.VisualStyle" Value="Blend">
        ///        <Setter Property="ProcessDoubleClick" Value="False" />
        ///    </Trigger>
        /// ]]>
        /// </code>
        /// </example>
        /// <seealso cref="bool"/>
        public bool ProcessDoubleClick
        {
            get
            {
                return (bool)GetValue(ProcessDoubleClickProperty);
            }

            set
            {
                SetValue(ProcessDoubleClickProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether header belongs to element in docked or auto hidden states.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="bool"/>
        /// Provides IsRichHeader value for the <see cref="DockHeaderPresenter"/>.
        /// </value>
        /// <remarks>
        /// Default value of this property is true.
        /// </remarks>
        /// <example>
        /// <para/>When you initialize <see cref="DockHeaderPresenter"/> template, you can set this property.
        /// This example shows how to use IsRichHeader property in XAML. 
        /// <code language="XAML">
        /// <![CDATA[
        ///         <Syncfusion:DockHeaderPresenter x:Name="PART_Header" DockPanel.Dock="Top" IsRichHeader="False" 
        ///                  Style="{Binding Path=(Syncfusion:DockingManager.DockHeaderStyle)
        ///                  , RelativeSource={RelativeSource AncestorType={x:Type Syncfusion:DockingManager}}}"
        ///                  IsTemplateParenKeyboardFocusWithin="{TemplateBinding IsShowedFocusedItem}"/>
        ///          ]]>
        /// </code>
        /// </example>
        /// <seealso cref="bool"/>
        public bool IsRichHeader
        {
            get
            {
                return (bool)GetValue(IsRichHeaderProperty);
            }

            set
            {
                SetValue(IsRichHeaderProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether template parent has keyboard focus.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="bool"/>
        /// Provides IsTemplateParenKeyboardFocusWithin value for the <see cref="DockHeaderPresenter"/>.
        /// </value>
        /// <remarks>
        /// Default value of this property is false.
        /// </remarks>
        /// <example>
        /// To set IsTemplateParenKeyboardFocusWithin property please see <see cref="IsRichHeader"/> property example.
        /// </example>
        /// <seealso cref="bool"/>
        public bool IsTemplateParenKeyboardFocusWithin
        {
            get
            {
                return (bool)GetValue(IsTemplateParenKeyboardFocusWithinProperty);
            }

            set
            {
                SetValue(IsTemplateParenKeyboardFocusWithinProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether context menu is opened.
        /// This is a dependency property.
        /// </summary>
        internal bool IsContextMenuOpen
        {
            get
            {
                return (bool)GetValue(IsContextMenuOpenProperty);
            }

            set
            {
                SetValue(IsContextMenuOpenProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the internal data context.
        /// </summary>
        /// <value>The internal data context.</value>
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

        public DockedElementTabbedHost ElementHost
        {
            get
            {
                return (DockedElementTabbedHost)GetValue(ElementHostProperty);
            }
            internal set
            {
                SetValue(ElementHostProperty, value);
            }
        }

        internal DockingManager DockingManager
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

        public bool UseNativeFloatWindow
        {
            get
            {
                return (bool)GetValue(UseNativeFloatWindowProperty);
            }
            internal set
            {
                SetValue(UseNativeFloatWindowProperty, value);
            }
        }
        #endregion

     
        #region Public methods

        /// <summary>
        /// Updates the header bindings.
        /// </summary>
        /// <param name="element">The element.</param>
        internal void UpdateHeaderBindings(FrameworkElement element)
        {
            if (DockingManager.GetState(element) != DockState.Float
                    && DockingManager.GetState(element) != DockState.AutoHidden)
            {
                if (m_awlButton != null) 
                    BindingUtils.SetBinding(m_awlButton, element, ToggleButton.VisibilityProperty, DockingManager.CanAutoHideProperty, BindingMode.OneWay, new BoolToVisibilityConverter());
                if (maxBtn != null)
                {
                    BindingUtils.SetBinding(maxBtn, element, ToggleButton.VisibilityProperty, DockingManager.MaximizeButtonVisibilityProperty, BindingMode.OneWay);
                    BindingUtils.SetBinding(maxBtn, element, ToggleButton.IsEnabledProperty, DockingManager.CanMaximizeProperty, BindingMode.OneWay);
                }
                if (minBtn != null)
                    BindingUtils.SetBinding(minBtn, element, ToggleButton.VisibilityProperty, DockingManager.MinimizeButtonVisibilityProperty, BindingMode.OneWay);
                if (restoreBtn != null)
                    BindingUtils.SetBinding(restoreBtn, element, ToggleButton.VisibilityProperty, DockingManager.RestoreButtonVisibilityProperty, BindingMode.OneWay);
            }
        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal 
        /// processes call <see cref="System.Windows.FrameworkElement.ApplyTemplate"/>
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            m_contextMenu = GetTemplateChild(ContextMenuName) as ContextMenu;
            DockingManager owner = null;
            if (InternalDataContext != null)
            {
                if (!(DockingManager.GetDockAbility(InternalDataContext) == DockAbility.All || DockingManager.GetDockAbility(InternalDataContext) == DockAbility.Tabbed))
                {
                    if (m_contextMenu != null)
                    {
                        (m_contextMenu as CustomContextMenu).IsEnabledTabbedMenuItem = false;
                    }
                }
                owner = DockingManager.ResolveManager(InternalDataContext);
                DockingManager.SetDockHeaderPresenter(InternalDataContext, this);
            }
            if (null != m_contextMenu)
            {
                if (owner != null)
                {
                    if (!DockingManager.GetIsContextMenuVisible(InternalDataContext))
                    {
                        m_contextMenu.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        m_contextMenu.Visibility = Visibility.Visible;
                    }
                }

                m_contextMenu.PlacementTarget = this;
                m_contextMenu.Closed -= new RoutedEventHandler(OnContextMenuClosed);
                m_contextMenu.Closed += new RoutedEventHandler(OnContextMenuClosed);
                PrepareContextMenu();
            }

            m_awlButton = GetTemplateChild("PART_AwlButton") as ToggleButton;
            ToggleButton closeButton = GetTemplateChild("PART_CloseButton") as ToggleButton;
            maxBtn = GetTemplateChild("PART_MaximizeButton") as ToggleButton;
            minBtn = GetTemplateChild("PART_MinimizeButton") as ToggleButton;
            restoreBtn = GetTemplateChild("PART_RestoreButton") as ToggleButton;

            if (InternalDataContext != null)
            {
                UpdateHeaderBindings(InternalDataContext);

                if (m_contextMenu != null && DockingManager.GetState(InternalDataContext) != DockState.Float)
                {
                    foreach (CustomMenuItem item in m_contextMenu.Items)
                    {
                        if (item.Name == "PART_AutoHideMenuItem")
                        {
                            BindingUtils.SetBinding(item, InternalDataContext, CustomMenuItem.IsEnabledProperty, DockingManager.CanAutoHideProperty, BindingMode.OneWay);
                        }
                    }
                }

                ToggleButton menubutton = GetTemplateChild("PART_ContextMenuButton") as ToggleButton;
                if (menubutton != null)
                {
                    if (!DockingManager.GetIsContextMenuButtonVisible(InternalDataContext))
                    {
                        menubutton.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        menubutton.Visibility = Visibility.Visible;
                    }
                }
            }
            m_awlButton.PreviewMouseDoubleClick += new MouseButtonEventHandler(HandleMouseDoubleClick);
            
            closeButton.PreviewMouseDoubleClick += new MouseButtonEventHandler(HandleMouseDoubleClick);
#if !SyncfusionFramework3_5
            //m_awlButton.PreviewTouchUp += closeButton_PreviewTouchUp;
            //closeButton.PreviewTouchUp += closeButton_PreviewTouchUp;
#endif
        }

        internal ToggleButton GetMenuButton()
        {
           return GetTemplateChild("PART_ContextMenuButton") as ToggleButton;
        }

        /// <summary>
        /// Handles the mouse double click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void HandleMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.StylusDevice == null || e.StylusDevice != null)
                e.Handled = true;
        }
        #endregion

        #region Implemantetation

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.UIElement.MouseLeftButtonDown"/>�routed event is raised on this element. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs"/> that contains the event data. The event data reports that the left mouse button was pressed</param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (e.StylusDevice == null || e.StylusDevice != null)
            {
                if (IsRichHeader)
                {
                    m_dragStart = e.GetPosition(this);
                    m_pressed = true;

                    DockedElementTabbedHost tabbedHost = TemplatedParent as DockedElementTabbedHost;

                    if (null != tabbedHost)
                    {
                        if (InternalDataContext != null)
                        {
                            UIElement element = InternalDataContext as UIElement;

                            if (null != element)
                            {
                                element.Focus();
                            }
                        }
                        else
                        {
                            tabbedHost.Focus();
                        }
                    }
                }
            }
            base.OnMouseLeftButtonDown(e);
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.UIElement.MouseLeftButtonUp"/>�routed event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs"/> that contains the event data. The event data reports that the left mouse button was released.</param>
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (e.StylusDevice == null || e.StylusDevice != null)
            {
                m_pressed = false;
            }
            base.OnMouseLeftButtonUp(e);
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.UIElement.MouseRightButtonDown"/>�routed event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs"/> that contains the event data. The event data reports that the right mouse button was pressed.</param>
        protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseRightButtonDown(e);
            if (e.StylusDevice == null || e.StylusDevice != null)
            {
                FrameworkElement element = DockingManager.GetInternalDataContext(this);
                DockingManager owner = DockingManager.ResolveManager(element);
                ActiveWindowChangingEventArgs args = new ActiveWindowChangingEventArgs();
                args.OldValue = owner.ActiveWindow;
                args.NewValue = element;
                if (args.OldValue != args.NewValue)
                {
                    owner.FireActiveWindowChanging(element, args);
                    if (!args.Cancel)
                    {
                        DockingManager.SetActiveWindow(element);
                        FireBeforeContextMenuOpen(element);

                        if (IsRichHeader)
                        {
                            m_host.Focus();
                        }
                        else
                        {
                            m_panel.Focus();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.MouseMove"/>�attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseEventArgs"/> that contains the event data.</param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (e.StylusDevice == null || e.StylusDevice != null)
            {
                m_pressed &= e.LeftButton == MouseButtonState.Pressed;

                if (IsRichHeader && m_pressed)
                {
                    Point point = e.GetPosition(this);
                    double deltaX = Math.Abs(m_dragStart.X - point.X);
                    double deltaY = Math.Abs(m_dragStart.Y - point.Y);

                    if (deltaX >= DistanceStartDrag || deltaY >= DistanceStartDrag)
                    {
                        if (m_host != null && m_host.DockingManager.IgnoreNamesOnDeserialize)
                        {
                            m_host.DockingManager.ResolveConflict();
                        }

                        ActiveWindowChangingEventArgs args = new ActiveWindowChangingEventArgs();
                        args.OldValue = m_host.DockingManager.ActiveWindow;
                        args.NewValue = m_host.InternalDataContext;
                        if (args.OldValue != args.NewValue)
                        {
                            m_host.DockingManager.FireActiveWindowChanging(m_host.InternalDataContext, args);
                            if (!args.Cancel)
                            {
                                DockingManager.StartDragging(m_host);
                                m_host.DockingManager.m_dragstarted = false;
                            }
                        }
                        else if (args.OldValue == args.NewValue)
                        {
                            DockingManager.StartDragging(m_host);
                            m_host.DockingManager.m_dragstarted = false;
                        }
                    }
                }
            }
            base.OnMouseMove(e);
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.MouseLeave"/>�attached event is raised on this element. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseEventArgs"/> that contains the event data.</param>
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            if (e.StylusDevice == null || e.StylusDevice != null)
            {
                m_pressed = false;
            }
            base.OnMouseLeave(e);
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.MouseDown"/>�attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs"/> that contains the event data. This event data reports details about the mouse button that was pressed and the handled state.</param>
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            if (e.StylusDevice == null || e.StylusDevice != null)
            {
                FrameworkElement element = DockingManager.GetInternalDataContext(this);

                if (element != null)
                {
                    DockingManager owner = DockingManager.ResolveManager(element);
                    if (owner != null && owner.ActiveWindow != element)
                    {
                        ActiveWindowChangingEventArgs args = new ActiveWindowChangingEventArgs();
                        args.OldValue = owner.ActiveWindow;
                        args.NewValue = element;
                        owner.FireActiveWindowChanging(element, args);
                        if (!args.Cancel)
                        {
                            DockingManager.SetNewFocusedElement(element);
                            FrameworkElement templateParent = TemplatedParent as FrameworkElement;
                            if (null != templateParent)
                            {
                                templateParent.Focus();
                            }
                        }
                        else
                        {
                            owner.m_ActiveWindowChangingFlag = false;
                        }
                    }
                    else
                    {
                        DockingManager.SetNewFocusedElement(element);
                        FrameworkElement templateParent = TemplatedParent as FrameworkElement;

                        if (null != templateParent)
                        {
                            templateParent.Focus();
                        }
                    }
                }

                if (InternalDataContext != null && m_contextMenu != null)
                {
                    if (!DockingManager.GetIsContextMenuVisible(InternalDataContext))
                    {
                        m_contextMenu.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        m_contextMenu.Visibility = Visibility.Visible;
                    }
                }
            }
            base.OnMouseDown(e);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Controls.Control.MouseDoubleClick"/> routed event.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            if (e.StylusDevice == null || e.StylusDevice != null)
            {
                if (IsRichHeader && ProcessDoubleClick)
                {
                    HitTestResult hitTest = VisualTreeHelper.HitTest(this, e.GetPosition(this));

                    if (null != hitTest)
                    {
                        FrameworkElement visualHit = hitTest.VisualHit as FrameworkElement;
                        if (visualHit != null)
                        {

                            if (!(visualHit.TemplatedParent is Button) && e.RightButton == MouseButtonState.Released)
                            {
                                DockingManager owner = DockingManager.ResolveManager(DockingManager.GetInternalDataContext(this));

                                FrameworkElement element = DockingManager.GetInternalDataContext(this);
                                bool bCanChangeState = true;

                                DockState newState = (m_host.State == DockState.Dock ||
                                    DockingManager.NeedExtractToWindow(element, ActionMode.Group))
                                    ? DockState.Float : DockState.Dock;

                                foreach (FrameworkElement item in m_host.TabChildren)
                                {
                                    if (!DockingManager.CanChangeState(element, newState))
                                    {
                                        bCanChangeState = false;
                                        break;
                                    }
                                }

                                if (bCanChangeState)
                                {
                                    if (owner != null && owner.ActiveWindow == element)
                                    {
                                        DockSide Side = DockingManager.GetSideInDockedMode(element);
                                        DockStateChangingEventArgs args = new DockStateChangingEventArgs();
                                        args.TargetState = newState;
                                        args.TargetSide = Side;
                                        args.TargetElement = element;
                                        owner.FireDockStateChanging(element, args);
                                        if (!args.Cancel)
                                        {
                                            owner.m_IsStateChangingChecked = true;
                                            owner.ExecuteDoubleClick(element, ActionMode.Group);
                                            owner.LockLayoutUpdate = true;
                                        }
                                    }
                                }

                                e.Handled = true;
                            }
                        }
                    }
                }
            }
            base.OnMouseDoubleClick(e);
        }

#if !SyncfusionFramework3_5
        //void closeButton_PreviewTouchUp(object sender, TouchEventArgs e)
        //{
        //    if (DockingManager != null && DockingManager.IsTouchEnabled && DockingManager.m_TouchDeviceId == e.TouchDevice.Id)
        //    {
        //        if (DockingManager.m_dockingManagerSystemGesture == SystemGesture.TwoFingerTap)
        //        {
        //            e.Handled = true;
        //        }
                
        //    }
        //}
        //private void OnTouchDoubleClick(TouchEventArgs e)
        //{
        //    if (IsRichHeader && ProcessDoubleClick)
        //    {
        //        HitTestResult hitTest = VisualTreeHelper.HitTest(this, e.GetTouchPoint(this).Position);

        //        if (null != hitTest)
        //        {
        //            FrameworkElement visualHit = hitTest.VisualHit as FrameworkElement;
        //            if (visualHit != null)
        //            {

        //                if (!(visualHit.TemplatedParent is Button) && e.GetTouchPoint(this).Action == TouchAction.Up)
        //                {
        //                    DockingManager owner = DockingManager.ResolveManager(DockingManager.GetInternalDataContext(this));

        //                    FrameworkElement element = DockingManager.GetInternalDataContext(this);
        //                    bool bCanChangeState = true;

        //                    DockState newState = (m_host.State == DockState.Dock ||
        //                        DockingManager.NeedExtractToWindow(element, ActionMode.Group))
        //                        ? DockState.Float : DockState.Dock;

        //                    foreach (FrameworkElement item in m_host.TabChildren)
        //                    {
        //                        if (!DockingManager.CanChangeState(element, newState))
        //                        {
        //                            bCanChangeState = false;
        //                            break;
        //                        }
        //                    }

        //                    if (bCanChangeState)
        //                    {
        //                        if (owner != null && owner.ActiveWindow == element)
        //                        {
        //                            DockSide Side = DockingManager.GetSideInDockedMode(element);
        //                            DockStateChangingEventArgs args = new DockStateChangingEventArgs();
        //                            args.TargetState = newState;
        //                            args.TargetSide = Side;
        //                            args.TargetElement = element;
        //                            owner.FireDockStateChanging(element, args);
        //                            if (!args.Cancel)
        //                            {
        //                                owner.m_IsStateChangingChecked = true;
        //                                owner.ExecuteDoubleClick(element, ActionMode.Group);
        //                                owner.LockLayoutUpdate = true;
        //                            }
        //                        }
        //                    }

        //                    e.Handled = true;
        //                }
        //            }
        //        }
        //    }
        //}

        //private void OnTouchLeftFingerDown(TouchEventArgs e)
        //{
        //    if (IsRichHeader)
        //    {
        //        m_dragStart = e.GetTouchPoint(this).Position;
        //        m_pressed = true;

        //        DockedElementTabbedHost tabbedHost = TemplatedParent as DockedElementTabbedHost;

        //        if (null != tabbedHost)
        //        {
        //            if (InternalDataContext != null)
        //            {
        //                UIElement element = InternalDataContext as UIElement;

        //                if (null != element)
        //                {
        //                    element.Focus();
        //                }
        //            }
        //            else
        //            {
        //                tabbedHost.Focus();
        //            }
        //        }
        //    }
        //}

        //private void OnTouchLeftFingerUp(TouchEventArgs e)
        //{
        //    m_pressed = false;
        //}

        //private void OnTouchRightFingerDown(TouchEventArgs e)
        //{
        //    FrameworkElement element = DockingManager.GetInternalDataContext(this);
        //    DockingManager owner = DockingManager.ResolveManager(element);
        //    ActiveWindowChangingEventArgs args = new ActiveWindowChangingEventArgs();
        //    args.OldValue = owner.ActiveWindow;
        //    args.NewValue = element;
        //    if (args.OldValue != args.NewValue)
        //    {
        //        owner.FireActiveWindowChanging(element, args);
        //        if (!args.Cancel)
        //        {
        //            DockingManager.SetActiveWindow(element);
        //            FireBeforeContextMenuOpen(element);

        //            if (IsRichHeader)
        //            {
        //                m_host.Focus();
        //            }
        //            else
        //            {
        //                m_panel.Focus();
        //            }
        //        }
        //    }
        //}

        //protected override void OnTouchEnter(TouchEventArgs e)
        //{
        //    base.OnTouchEnter(e);
        //}

        //protected override void OnTouchDown(TouchEventArgs e)
        //{
        //    if (DockingManager != null && DockingManager.IsTouchEnabled && DockingManager.m_TouchDeviceId == e.TouchDevice.Id)
        //    {
        //        m_TouchDown = true;
        //        FrameworkElement element = DockingManager.GetInternalDataContext(this);

        //        if (element != null)
        //        {
        //            DockingManager owner = DockingManager.ResolveManager(element);
        //            if (owner != null && owner.ActiveWindow != element)
        //            {
        //                ActiveWindowChangingEventArgs args = new ActiveWindowChangingEventArgs();
        //                args.OldValue = owner.ActiveWindow;
        //                args.NewValue = element;
        //                owner.FireActiveWindowChanging(element, args);
        //                if (!args.Cancel)
        //                {
        //                    DockingManager.SetNewFocusedElement(element);
        //                    FrameworkElement templateParent = TemplatedParent as FrameworkElement;
        //                    if (null != templateParent)
        //                    {
        //                        templateParent.Focus();
        //                    }
        //                }
        //                else
        //                {
        //                    owner.m_ActiveWindowChangingFlag = false;
        //                }
        //            }
        //            else
        //            {
        //                DockingManager.SetNewFocusedElement(element);
        //                FrameworkElement templateParent = TemplatedParent as FrameworkElement;

        //                if (null != templateParent)
        //                {
        //                    templateParent.Focus();
        //                }
        //            }
        //        }

        //        if (InternalDataContext != null && m_contextMenu != null)
        //        {
        //            if (!DockingManager.GetIsContextMenuVisible(InternalDataContext))
        //            {
        //                m_contextMenu.Visibility = Visibility.Collapsed;
        //            }
        //            else
        //            {
        //                m_contextMenu.Visibility = Visibility.Visible;
        //            }
        //        }
        //        base.OnTouchDown(e);
        //        OnTouchLeftFingerDown(e);
        //    }
        //}

        //protected override void OnTouchMove(TouchEventArgs e)
        //{
        //    if (DockingManager != null && DockingManager.IsTouchEnabled && DockingManager.m_TouchDeviceId == e.TouchDevice.Id)
        //    {
        //        if (DockingManager.m_dockingManagerSystemGesture == SystemGesture.Drag)
        //        {
        //            //m_pressed = DockingManager.m_dockingManagerSystemGesture == SystemGesture.Drag;

        //            if (IsRichHeader)
        //            {
        //                Point point = e.GetTouchPoint(this).Position;
        //                double deltaX = Math.Abs(m_dragStart.X - point.X);
        //                double deltaY = Math.Abs(m_dragStart.Y - point.Y);

        //                if (deltaX >= DistanceStartDrag || deltaY >= DistanceStartDrag)
        //                {
        //                    if (m_host != null && m_host.DockingManager.IgnoreNamesOnDeserialize)
        //                    {
        //                        m_host.DockingManager.ResolveConflict();
        //                    }

        //                    ActiveWindowChangingEventArgs args = new ActiveWindowChangingEventArgs();
        //                    args.OldValue = m_host.DockingManager.ActiveWindow;
        //                    args.NewValue = m_host.InternalDataContext;
        //                    if (args.OldValue != args.NewValue)
        //                    {
        //                        m_host.DockingManager.FireActiveWindowChanging(m_host.InternalDataContext, args);
        //                        if (!args.Cancel)
        //                        {
        //                            DockingManager.StartDragging(m_host);
        //                            m_host.DockingManager.m_dragstarted = false;
        //                        }
        //                    }
        //                    else if (args.OldValue == args.NewValue)
        //                    {
        //                        DockingManager.StartDragging(m_host);
        //                        m_host.DockingManager.m_dragstarted = false;
        //                    }
        //                }
        //            }
        //            if (DockingManager.m_dockingManagerSystemGesture == SystemGesture.HoldEnter)
        //                OnTouchRightFingerDown(e);
        //            else if (DockingManager.m_dockingManagerSystemGesture == SystemGesture.Tap)
        //                OnTouchLeftFingerDown(e);

        //            base.OnTouchMove(e);
        //        }
        //    }
        //}

        //protected override void OnTouchLeave(TouchEventArgs e)
        //{
        //    if (DockingManager != null && DockingManager.IsTouchEnabled && DockingManager.m_TouchDeviceId == e.TouchDevice.Id)
        //    {
        //        m_pressed = false;
        //        base.OnTouchLeave(e);
        //    }
        //}

        //protected override void OnTouchUp(TouchEventArgs e)
        //{
        //    if (DockingManager != null && DockingManager.IsTouchEnabled && DockingManager.m_TouchDeviceId == e.TouchDevice.Id)
        //    {
        //        if (DockingManager.m_dockingManagerSystemGesture == SystemGesture.Tap)
        //            OnTouchLeftFingerUp(e);
        //        else if (DockingManager.m_dockingManagerSystemGesture == SystemGesture.TwoFingerTap)
        //            OnTouchDoubleClick(e);
        //        m_TouchDown = false;
        //        base.OnTouchUp(e);
        //    }
        //}
#endif

        /// <summary>
        /// Raises the <see cref="E:System.Windows.FrameworkElement.Initialized"/> event. 
        /// This method is invoked whenever <see cref="P:System.Windows.FrameworkElement.IsInitialized"/> is set to true internally.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.RoutedEventArgs"/> that contains the event data.</param>
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            ValidateTemplateParent();

            
            CommandBinding bindingClose = new CommandBinding(CloseCommand, ExecuteClose);
            CommandBindings.Add(bindingClose);

            CommandBinding bindingOpenContextMenu = new CommandBinding(OpenContextMenuCommand, ExecuteOpenContextMenu);
            CommandBindings.Add(bindingOpenContextMenu);

            CommandBinding bindingChangeAwlState = new CommandBinding(ChangeAwlStateCommand, ExecuteChangeAwlState);
            CommandBindings.Add(bindingChangeAwlState);

            CommandBinding bindingMaximizeState = new CommandBinding(MaximizeStateCommand, ExecuteMaximizeState);
            CommandBindings.Add(bindingMaximizeState);

            CommandBinding bindingMinimizeState = new CommandBinding(MinimizeStateCommand, ExecuteMinimizeState);
            CommandBindings.Add(bindingMinimizeState);

            CommandBinding bindingRestoreState = new CommandBinding(RestoreStateCommand, ExecuteRestoreState);
            CommandBindings.Add(bindingRestoreState);
        }

        /// <summary>
        /// Sets context menu items properties and subscribes events.
        /// </summary>
        private void PrepareContextMenu()
        {
            foreach (MenuItem item in m_contextMenu.Items)
            {
                string itemName = item.Name;

                if (IsRichHeader)
                {
                    if (DockableMenuItemName == itemName)
                    {
                        item.IsChecked = true;
                    }
                }
                else
                {
                    if (AutoHideMenuItemName == itemName)
                    {
                        item.IsChecked = true;
                    }
                    else if (FloatingMenuItemName == itemName
                        || DockableMenuItemName == itemName
                        || TabbedMenuItemName == itemName)
                    {
                        item.IsEnabled = false;
                    }
                }

                item.Click -= new RoutedEventHandler(OnMenuItemClick);
                item.Click += new RoutedEventHandler(OnMenuItemClick);
            }
        }

        /// <summary>
        /// Fires the context menu item click.
        /// </summary>
        /// <param name="source">The source.</param>
        private void FireContextMenuItemClick(FrameworkElement source)
        {
            RoutedEventArgs args = new RoutedEventArgs(DockingManager.ContextMenuItemClickEvent, source);
            RaiseEvent(args);
        }

        /// <summary>
        /// Fires the before context menu open.
        /// </summary>
        /// <param name="source">The source.</param>
        private void FireBeforeContextMenuOpen(FrameworkElement source)
        {
            RoutedEventArgs args = new RoutedEventArgs(DockingManager.BeforeContextMenuOpenEvent, source);
            RaiseEvent(args);
        }

        /// <summary>
        /// Fires the window visibility changed.
        /// </summary>
        /// <param name="source">The source.</param>
        private void FireWindowVisibilityChanged(FrameworkElement source)
        {
            RoutedEventArgs args = new RoutedEventArgs(DockingManager.WindowVisibilityChangedEvent, source);
            RaiseEvent(args);
        }

        /// <summary>
        /// Fires the dock menu click changed.
        /// </summary>
        /// <param name="source">The source.</param>
        private void FireDockMenuClickChanged(FrameworkElement source)
        {
            RoutedEventArgs arg = new RoutedEventArgs(DockingManager.DockMenuClickEvent, source);
            RaiseEvent(arg);
        }

        /// <summary>
        /// Raises IsContextMenuOpenChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private void OnIsContextMenuOpenChanged(DependencyPropertyChangedEventArgs e)
        {
            if (IsContextMenuOpenChanged != null)
            {
                IsContextMenuOpenChanged(this, e);
            }
        }

        /// <summary>
        /// Called when [menu item click].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void OnMenuItemClick(object sender, RoutedEventArgs e)
        {
            MenuItem item = (MenuItem)sender;
            FrameworkElement element = DockingManager.GetInternalDataContext(this);
            DockingManager owner = DockingManager.ResolveManager(element);
            FireContextMenuItemClick(element);

            if (HideMenuItemName == item.Name)
            {
                owner.ExecuteClose(element);
                FireWindowVisibilityChanged(element);
            }
            else if (FloatingMenuItemName == item.Name)
            {
                if (DockingManager.CanChangeState(element, DockState.Float))
                {
                    DockStateChangingEventArgs args = new DockStateChangingEventArgs();
                    args.SourceElement = element;
                    args.TargetState = DockState.Float;
                    args.PresentState = DockState.Dock;
                    owner.FireDockStateChanging(element, args);
                    if (!owner.CanNestedFloat)
                    {
                        owner.m_IsStateChangingChecked = true;
                        var hasFloatingParent = owner.HasFloatingParent(element);

                        if (hasFloatingParent)
                        {
                            args.Cancel = true;
                        }
                    }
                    if (!args.Cancel)
                    {
                        owner.ExtractElementToWindow(element, ActionMode.Active, false);
                        DockingManager.SetNoDock(element, true);
                        ActiveWindowChangingEventArgs act = new ActiveWindowChangingEventArgs();
                        act.NewValue = element;
                        act.OldValue = owner.ActiveWindow;
                        if (act.OldValue != act.NewValue)
                        {
                            owner.FireActiveWindowChanging(element, act);
                            if (!act.Cancel)
                            {
                                DockingManager.SetNewFocusedElement(element);
                            }
                        }
                    }
                }
            }
            else if (AutoHideMenuItemName == item.Name)
            {
                ChangeAwlState();
            }
            else if (TabbedMenuItemName == item.Name)
            {
                owner.ExecuteDocument(element);
            }
            else if (MaximizeMenuItemName == item.Name)
            {
                owner.ExecuteMaximize(element);
                ToggleButton restorebutton = GetTemplateChild("PART_RestoreButton") as ToggleButton;
                ToggleButton maximizebutton = GetTemplateChild("PART_MaximizeButton") as ToggleButton;
                if (maximizebutton != null && restorebutton != null)
                {
                    maximizebutton.Visibility = Visibility.Collapsed;
                    restorebutton.Visibility = Visibility.Visible;
                }
            }
            else if (RestoreMenuItemName == item.Name)
            {
                owner.ExecuteRestore(element);
                ToggleButton restorebutton = GetTemplateChild("PART_RestoreButton") as ToggleButton;
                ToggleButton maximizebutton = GetTemplateChild("PART_MaximizeButton") as ToggleButton;
                if (maximizebutton != null && restorebutton != null)
                {
                    maximizebutton.Visibility = Visibility.Visible;
                    restorebutton.Visibility = Visibility.Collapsed;
                }
            }
            else if (MinimizeMenuItemName == item.Name)
            {
                owner.ExecuteMinimize(element);
            }
        }

        /// <summary>
        /// Checks the collection of siblings of the HostedElement for CanAutoHide value.
        /// </summary>
        /// <returns>true if any sibling of the HostedElement CanAutoHide</returns>
        private bool HasCanAutoHide()
        {
            FrameworkElement currentElement = m_host.HostedElement;
            DockingManager owner = DockingManager.ResolveManager(currentElement);
            DockState state = DockingManager.GetState(currentElement);
            List<FrameworkElement> siblingsList = owner.FindSiblings(currentElement, state, true);

            if (!DockingManager.GetCanAutoHide(currentElement))
            {
                return false;
            }
            else
            {
                for (int i = 0, cnt = siblingsList.Count; i < cnt; ++i)
                {
                    FrameworkElement sibling = siblingsList[i];
                    if (!DockingManager.GetCanAutoHide(sibling))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Is used when hiding of the element called.
        /// </summary>
        /// <param name="newState">new state of the element</param>
        private void ExecuteHide(DockState newState)
        {
            FrameworkElement currentElement = DockingManager.GetInternalDataContext(this);
            DockingManager owner = DockingManager.ResolveManager(currentElement);

            if (null != owner)
            {
                if (newState == DockState.AutoHidden)
                {
                    owner.ExecuteAutoHide((FrameworkElement)currentElement);
                }
                else
                {
                    owner.ExecuteClose((FrameworkElement)currentElement);
                }
            }
        }

        /// <summary>
        /// Validates the template parent.
        /// </summary>
        private void ValidateTemplateParent()
        {
            if (IsRichHeader)
            {
                m_host = TemplatedParent as DockedElementTabbedHost;
                ElementHost = m_host;
                UseNativeFloatWindow = ElementHost.DockingManager.UseNativeFloatWindow;
            }
            else
            {
                m_panel = TemplatedParent as SidePanel;

                if (null == m_panel)
                {
                    throw new NotSupportedException("DockHeaderPresenter is intended to be used only inside the template of the DockedElementTabbedHost or SidePanel.");
                }
            }
        }

        /// <summary>
        /// Called when [context menu closed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void OnContextMenuClosed(object sender, RoutedEventArgs e)
        {
            IsContextMenuOpen = false;
        }

        /// <summary>
        /// This method changes the state of the context element.
        /// </summary>
        private void ChangeAwlState()
        {
            if (IsRichHeader)
            {
                ExecuteHide(DockState.AutoHidden);
            }
            else
            {
                FrameworkElement element = DockingManager.GetInternalDataContext(this);
                DockingManager owner = DockingManager.ResolveManager(element);

                bool bCanExecute = owner.DockFill && owner.FilterChildren(DockState.Document).Count > 0
                    && owner.DockFillDocumentMode == DockFillDocumentMode.Fill ? false : true;
                if (bCanExecute)
                {
                    owner.ExecuteUnAutoHide(element);
                }
            }
        }

        /// <summary>
        /// Executes the close.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private void ExecuteClose(object sender, ExecutedRoutedEventArgs e)
        {
            FrameworkElement element = DockingManager.GetInternalDataContext(this);
            DockingManager owner = DockingManager.ResolveManager(element);

            if (IsRichHeader)
            {
                if (ProcessDoubleClick)
                {
                    owner.ExecuteClose(element);
                    FireWindowVisibilityChanged(element);
                    owner.SetFocus(element);
                }
                else
                {
                    owner.ExecuteDoubleClick(element, ActionMode.Active);
                    owner.LockLayoutUpdate = true;
                }
            }
            else
            {
                owner.ExecuteClose(element);
            }
        }

        /// <summary>
        /// Executes the open context menu.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private void ExecuteOpenContextMenu(object sender, ExecutedRoutedEventArgs e)
        {
            FrameworkElement element = DockingManager.GetInternalDataContext(this);
            FireDockMenuClickChanged(element);

            if (IsRichHeader)
            {
                m_host.Focus();
            }
            
            if (null != m_contextMenu)
            {
                if (null == m_contextMenu.DataContext)
                {
                   //m_contextMenu.DataContext = DockingManager.GetInternalDataContext(this);
                }

                IsContextMenuOpen = true;
                DockingManager.SetActiveWindow(element);
                FireBeforeContextMenuOpen(element);
                m_contextMenu.IsOpen = true;
            }
        }

        /// <summary>
        /// Executes the state of the change awl.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private void ExecuteChangeAwlState(object sender, ExecutedRoutedEventArgs e)
        {
            ChangeAwlState();
        }

        /// <summary>
        /// Executes the state of the maximize.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private void ExecuteMaximizeState(object sender, ExecutedRoutedEventArgs e)
        {
            FrameworkElement element = DockingManager.GetInternalDataContext(this);
            DockingManager owner = DockingManager.ResolveManager(element);
            if(owner !=null && element !=null)
            {
                owner.ExecuteMaximize(element);
                DockingManager.SetRestoreButtonVisibility(element, Visibility.Visible);
                DockingManager.SetMaximizeButtonVisibility(element, Visibility.Collapsed);
            }           
        }

        /// <summary>
        /// Executes the state of the minimize.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private void ExecuteMinimizeState(object sender, ExecutedRoutedEventArgs e)
        {
            FrameworkElement element = DockingManager.GetInternalDataContext(this);
            DockingManager owner = DockingManager.ResolveManager(element);
            if(owner !=null && element!=null)
            {
                owner.ExecuteMinimize(element);
            }
        }

        /// <summary>
        /// Changes the state of the restore.
        /// </summary>
        /// <param name="child">The child.</param>
        internal void ChangeRestoreState(FrameworkElement child)
        {
            DockingManager owner = DockingManager.ResolveManager(child);
            if(owner !=null && child !=null)
            {
                owner.ExecuteRestore(child);
                DockingManager.SetMaximizeButtonVisibility(child, Visibility.Visible);
                DockingManager.SetRestoreButtonVisibility(child, Visibility.Collapsed);
            }
        }

        /// <summary>
        /// Executes the state of the restore.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private void ExecuteRestoreState(object sender, ExecutedRoutedEventArgs e)
        {
            FrameworkElement element = DockingManager.GetInternalDataContext(this);
            DockingManager owner = DockingManager.ResolveManager(element);
            if(owner !=null && element !=null)
            {
                owner.ExecuteRestore(element);
                DockingManager.SetMaximizeButtonVisibility(element, Visibility.Visible);
                DockingManager.SetRestoreButtonVisibility(element, Visibility.Collapsed);
            }
        }

        /// <summary>
        /// Calls OnIsContextMenuOpenChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnIsContextMenuOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DockHeaderPresenter instance = (DockHeaderPresenter)d;
            instance.OnIsContextMenuOpenChanged(e);
        }

        #endregion

        #region Dependency properties

        /// <summary>
        /// Identifies the <see cref="ProcessDoubleClick"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ProcessDoubleClickProperty =
            DependencyProperty.Register("ProcessDoubleClick", typeof(bool), typeof(DockHeaderPresenter), new UIPropertyMetadata(true));

        /// <summary>
        /// Identifies the <see cref="IsRichHeader"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsRichHeaderProperty =
            DependencyProperty.Register("IsRichHeader", typeof(bool), typeof(DockHeaderPresenter), new UIPropertyMetadata(true));

        /// <summary>
        /// Identifies the <see cref="IsTemplateParenKeyboardFocusWithin"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsTemplateParenKeyboardFocusWithinProperty =
            DependencyProperty.Register("IsTemplateParenKeyboardFocusWithin", typeof(bool), typeof(DockHeaderPresenter), new UIPropertyMetadata(false));

        /// <summary>
        /// Identifies the <see cref="IsContextMenuOpen"/> dependency property.
        /// </summary>
        internal static readonly DependencyProperty IsContextMenuOpenProperty =
            DependencyProperty.Register("IsContextMenuOpen", typeof(bool), typeof(DockHeaderPresenter), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsContextMenuOpenChanged)));

        public static readonly DependencyProperty ElementHostProperty =
            DependencyProperty.Register("ElementHost", typeof(DockedElementTabbedHost), typeof(DockHeaderPresenter), new FrameworkPropertyMetadata(null));
        
        public static readonly DependencyProperty UseNativeFloatWindowProperty =
            DependencyProperty.Register("UseNativeFloatWindow", typeof(bool), typeof(DockHeaderPresenter), new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Identifies the Docking Manager Property
        /// </summary>
        public static readonly DependencyProperty DockingManagerProperty =
            DependencyProperty.Register("DockingManager", typeof(DockingManager), typeof(DockHeaderPresenter));
        #endregion
    }
}