#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Syncfusion.Windows.Shared;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Controls;
using System.Diagnostics;
using System.Windows.Input;
using System.Windows.Media;
using System.Runtime.InteropServices;

namespace Syncfusion.Windows.Tools.Controls
{
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    [SkinType(SkinVisualStyle = Skin.Office2007Blue,
Type = typeof(FloatWindowBorder), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/DockingManager/Themes/Office2007BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Black,
    Type = typeof(DockingManager), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/DockingManager/Themes/Office2007BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Silver,
    Type = typeof(FloatWindowBorder), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/DockingManager/Themes/Office2007SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Blue,
Type = typeof(FloatWindowBorder), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/DockingManager/Themes/Office2010BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Black,
    Type = typeof(FloatWindowBorder), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/DockingManager/Themes/Office2010BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Silver,
    Type = typeof(FloatWindowBorder), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/DockingManager/Themes/Office2010SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2003,
    Type = typeof(FloatWindowBorder), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/DockingManager/Themes/Office2003Style.xaml")]
    [SkinType(SkinVisualStyle = Skin.Blend,
    Type = typeof(FloatWindowBorder), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/DockingManager/Themes/BlendStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.SyncOrange,
    Type = typeof(FloatWindowBorder), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/DockingManager/Themes/SyncOrangeStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.ShinyRed,
    Type = typeof(FloatWindowBorder), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/DockingManager/Themes/ShinyRedStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.ShinyBlue,
    Type = typeof(FloatWindowBorder), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/DockingManager/Themes/ShinyBlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Default,
    Type = typeof(FloatWindowBorder), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/DockingManager/Themes/vista.aero.xaml")]
    [SkinType(SkinVisualStyle = Skin.VS2010,
    Type = typeof(FloatWindowBorder), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/DockingManager/Themes/VS2010Style.xaml")]
    [SkinType(SkinVisualStyle = Skin.Metro,
   Type = typeof(DockingManager), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/DockingManager/Themes/MetroStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Transparent,
    Type = typeof(FloatWindowBorder), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/DockingManager/Themes/TransparentStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2013,
    Type = typeof(FloatWindowBorder), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/DockingManager/Themes/Office2013Style.xaml")] 
    public class NativeFloatWindow : Window
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr DefWindowProc(
            IntPtr hWnd,
            int msg,
            IntPtr wParam,
            IntPtr lParam);  

        #region Constants

        private const uint WM_CONTEXTMENU = 0x00A5;

        private const uint WM_SYSTEMMENU = 0xa4;
        private const uint WP_SYSTEMMENU = 0x02;
        private const uint WM_NCLBUTTONDBLCLK = 0x00A3;
        private const uint WM_NCLBUTTONDOWN = 0x00A1;
        private const uint WM_NCRBUTTONDOWN = 0x00A4;
        private const uint WM_NCLBUTTONUP = 0x00A2;
        private const uint WM_NCMOUSEMOVE = 0x0084;
        private const uint WM_NCMOUSELEAVE = 0x00A0;
        private const uint WM_SIZING = 0x0214;
        private const uint VK_ESCAPE = 27;

        private CustomContextMenu m_systemenu = null;
        private Rect m_windowPosition;
        private bool m_windowresizing = false;

        internal bool m_mouseLeftButtonDown = false;
        /// <summary>
        /// Specifies hide menu item.
        /// </summary>
        private const string HIDE_MENUITEM_NAME = "PART_HideMenuItem";

        /// <summary>
        /// Specifies dockable menu item.
        /// </summary>
        private const string DOCKABLE_MENUITEM_NAME = "PART_DockableMenuItem";

        /// <summary>
        /// Specifies floating menu item.
        /// </summary>
        private const string FLOATING_MENUITEM_NAME = "PART_FloatingMenuItem";

        /// <summary>
        /// Indicates auto hide item.
        /// </summary>
        private const string AUTO_HIDEITEM_NAME = "PART_AutoHideMenuItem";

        /// <summary>
        /// Indicates tabbed item.
        /// </summary>
        private const string DOCUMENT_NAME = "PART_TabbedMenuItem";

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This constant holds the name of one of the items. 
        /// </summary>
        private const string MAXIMIZE_NAME = "PART_MaximizeMenuItem";

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This constant holds the name of one of the items. 
        /// </summary>
        private const string MINIMIZE_NAME = "PART_MinimizeMenuItem";

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This constant holds the name of one of the items. 
        /// </summary>
        private const string RESTORE_NAME = "PART_RetoreMenuItem";


        private  Visibility bdefault = Visibility.Visible;

        /// <summary>
        /// Indicates TouchDevice Id.
        /// </summary>
        internal int m_nativeTouchDeviceId = -1;

        internal SystemGesture m_nativeSystemGesture;

        #endregion

        public static RoutedUICommand CloseCommand = new RoutedUICommand("Close", "Close", typeof(NativeFloatWindow));

        public static RoutedUICommand MaximizeCommand = new RoutedUICommand("Maximize", "Maximize", typeof(NativeFloatWindow));

        public static RoutedUICommand RestoreCommand = new RoutedUICommand("Restore", "Restore", typeof(NativeFloatWindow));

        # region Public Properties

        private DockedElementTabbedHost m_MouseUnderHost;

        internal Rect PlacementRectangle
        {
            get;
            set;
        }

        public ContextMenu SystemMenu
        {
            get
            {
                return m_systemenu as ContextMenu;

            }
            set
            {
                m_systemenu = value as CustomContextMenu;
            }
        }
       
        public bool HitTestDisabled
        {
            get;
            set;
        }


        internal Rect InternalPlacementRect
        {
            get { return (Rect)GetValue(InternalPlacementRectProperty); }
            set { SetValue(InternalPlacementRectProperty, value); }
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

        public bool IsOpen
        {
            get
            {
                return (bool)GetValue(IsOpenProperty);
            }

            set
            {
                if (value)
                {
                    this.Show();
                }
                else
                {
                    this.Visibility = Visibility.Collapsed;
                }
                SetValue(IsOpenProperty, value);
            }
        }

        protected override void OnIsKeyboardFocusedChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnIsKeyboardFocusedChanged(e);
        }

        /// <summary>
        /// Gets or sets a value indicating whether IsDragging of the FloatWindow.
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

        internal UIElement NativeHeader { get; set; }

        /// <summary>
        /// Gets or sets PrimaryElement of the FloatWindow.This is a dependency property.
        /// </summary>
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

        internal bool MaximizeButtonEnabled
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

        #endregion

        #region dependency propery definition



        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Header.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(string), typeof(NativeFloatWindow), new UIPropertyMetadata(string.Empty));

        

        public static readonly DependencyProperty IsOpenProperty =
           DependencyProperty.Register("IsOpen", typeof(bool), typeof(NativeFloatWindow), new FrameworkPropertyMetadata(false));
        /// <summary>
        /// Identifies PrimaryElement dependency property of the FloatWindow.
        /// </summary
        public static readonly DependencyProperty PrimaryElementProperty = DependencyProperty.Register("PrimaryElement", typeof(FrameworkElement), typeof(NativeFloatWindow));

        internal static readonly DependencyProperty MaximizeButtonEnabledProperty = DependencyProperty.Register("MaximizeButtonEnabled", typeof(bool), typeof(NativeFloatWindow), new PropertyMetadata(false));
        /// <summary>
        /// Identifies IsDragging dependency property of the FloatWindow.
        /// </summary>
        public static readonly DependencyProperty IsDraggingProperty =
            DependencyProperty.Register("IsDragging", typeof(bool), typeof(NativeFloatWindow), new FrameworkPropertyMetadata(false, OnIsDraggingChanged));

        /// <summary>
        /// Identifies IsMultiHostsContainer dependency property of the FloatWindow.
        /// </summary>
        internal static readonly DependencyProperty IsMultiHostsContainerProperty =
            DependencyProperty.Register("IsMultiHostsContainer", typeof(bool), typeof(NativeFloatWindow), new UIPropertyMetadata(false));

        /// <summary>
        /// Identifies DockingManager dependency property of the FloatWindow.
        /// </summary>
        public static readonly DependencyProperty DockingManagerProperty =
            DependencyProperty.Register("DockingManager", typeof(DockingManager), typeof(NativeFloatWindow));

        // Using a DependencyProperty as the backing store for InternalPlacementRect.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty InternalPlacementRectProperty =
            DependencyProperty.Register("PlacementRectangle", typeof(Rect), typeof(NativeFloatWindow), new FrameworkPropertyMetadata(Rect.Empty, new PropertyChangedCallback(OnPlacementRectangleChanged)));


        #endregion

        #region ContextMenu

        private void ShowContextMenu()
        {
            if (SystemMenu != null)
            {
                SystemMenu.IsOpen = true;
            }
        }

        protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            base.OnGotKeyboardFocus(e);
            
            DockedElementTabbedHost host = e.OriginalSource as DockedElementTabbedHost;

            if (null != host)
            {
                if (this.PrimaryElement != null && this.PrimaryElement is FrameworkElement)
                {
                    foreach (FrameworkElement element in host.TabChildren)
                    {
                        if (element.Equals(this.PrimaryElement))
                        {
                            InternalDataContext = host;
                        }
                    }
                }
                else
                {
                    InternalDataContext = host;
                }
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            SystemMenu = GetTemplateChild("Part_CustomContextMenu") as CustomContextMenu;
            NativeHeader = GetTemplateChild("BorderHeader") as ContentControl;
            if (SystemMenu != null)
            {
                SystemMenu.Opened += new RoutedEventHandler(SystemMenu_Opened);
                SystemMenu.Closed += new RoutedEventHandler(SystemMenu_Closed);
            }
        }

        void SystemMenu_Opened(object sender, RoutedEventArgs e)
        {
            SkinStorage.SetVisualStyle((sender as CustomContextMenu), SkinStorage.GetVisualStyle(this));
            UIElement hostedElement = FindHostedElement();
            bool noDock = (hostedElement != null) ? DockingManager.GetNoDock(hostedElement) : false;

            if (hostedElement != null
                && (DockingManager.CollapseDefaultContextMenuItems || DockingManager.GetCollapseDefaultContextMenuItemsInFloat(hostedElement as DependencyObject)))
            {
                bdefault = Visibility.Collapsed;
                ValidateContextMenu();
            }
            if (hostedElement != null && !DockingManager.GetCanDock(hostedElement) && DockingManager.GetCanFloat(hostedElement) && DockingManager.GetState(hostedElement) == DockState.Float)
            {
                noDock = true;
            }

            if (SystemMenu == null)
            {
                SystemMenu = SystemMenu = GetTemplateChild("Part_CustomContextMenu") as CustomContextMenu;
            }

            if (SystemMenu != null)
            {
                foreach (MenuItem item in SystemMenu.Items)
                {
                    if (item != null)
                    {
                        switch (item.Name)
                        {
                            case FLOATING_MENUITEM_NAME:
                                item.IsChecked = noDock;
                                item.Visibility = bdefault;
                                if (bdefault == Visibility.Visible && PrimaryElement != null)
                                {
                                    if (DockingManager.GetShowFloatingMenuItem(PrimaryElement))
                                    {
                                        item.Visibility = Visibility.Visible;
                                    }
                                    else
                                    {
                                        item.Visibility = Visibility.Collapsed;
                                    }

                                }
                                break;
                            case DOCKABLE_MENUITEM_NAME:
                                item.IsChecked = !noDock;
                                item.Visibility = bdefault;
                                if (bdefault == Visibility.Visible && PrimaryElement != null)
                                {
                                    if (DockingManager.GetShowDockableMenuItem(PrimaryElement))
                                    {
                                        item.Visibility = Visibility.Visible;
                                    }
                                    else
                                    {
                                        item.Visibility = Visibility.Collapsed;
                                    }

                                }
                                break;
                            case AUTO_HIDEITEM_NAME:
                                item.IsEnabled = false;
                                item.Visibility = bdefault;
                                if (bdefault == Visibility.Visible && PrimaryElement != null)
                                {
                                    if (DockingManager.GetShowAutoHiddenMenuItem(PrimaryElement))
                                    {
                                        item.Visibility = Visibility.Visible;
                                    }
                                    else
                                    {
                                        item.Visibility = Visibility.Collapsed;
                                    }

                                }
                                break;

                            case HIDE_MENUITEM_NAME:
                                item.Visibility = bdefault;
                                if (bdefault == Visibility.Visible && PrimaryElement != null)
                                {
                                    if (DockingManager.GetShowHiddenMenuItem(PrimaryElement))
                                    {
                                        item.Visibility = Visibility.Visible;
                                    }
                                    else
                                    {
                                        item.Visibility = Visibility.Collapsed;
                                    }

                                }
                                break;

                            case MAXIMIZE_NAME:
                                item.Visibility = Visibility.Collapsed;
                                break;

                            case MINIMIZE_NAME:
                                item.Visibility = Visibility.Collapsed;
                                break;

                            case RESTORE_NAME:
                                item.Visibility = Visibility.Collapsed;
                                break;

                            case DOCUMENT_NAME:
                                item.Visibility = bdefault;
                                if (bdefault == Visibility.Visible && PrimaryElement != null)
                                {
                                    if (DockingManager.GetShowTabbedMenuItem(PrimaryElement))
                                    {
                                        item.Visibility = Visibility.Visible;
                                    }
                                    else
                                    {
                                        item.Visibility = Visibility.Collapsed;
                                    }

                                }
                                break;
                        }

                        item.Click -= new RoutedEventHandler(OnMenuItemClick);
                        item.Click += new RoutedEventHandler(OnMenuItemClick);
                    }
                }
            }
        }

        private void ValidateContextMenu()
        {
            if (DockingManager != null)
            {
                SystemMenu.Items.Clear();
                if (DockingManager.m_custommenuitems != null)
                {
                    for (int i = 0; i < DockingManager.m_custommenuitems.Count; i++)
                    {
                        SystemMenu.Items.Add(DockingManager.m_custommenuitems[i] as MenuItem);
                    }
                }
            }
        }

        private void OnMenuItemClick(object sender, RoutedEventArgs e)
        {
            FireContextMenuItemClick();
            MenuItem item = (MenuItem)sender;

            switch (item.Name)
            {
                case HIDE_MENUITEM_NAME:
                    ExecuteChangeState(null, null);
                    break;
                case DOCKABLE_MENUITEM_NAME:
                    SetDockable();
                    break;
                case FLOATING_MENUITEM_NAME:
                    SetFloating();
                    break;
                case DOCUMENT_NAME:
                    SetDocument();
                    break;
            }
        }

        private void FireContextMenuItemClick()
        {
            RoutedEventArgs args = new RoutedEventArgs(DockingManager.ContextMenuItemClickEvent);
            RaiseEvent(args);
        }

        private void SetDockable()
        {
            UIElement hostedElement = InternalDataContext as DockedElementTabbedHost;

            if (hostedElement != null && PrimaryElement != null)
            {
                DockingManager.SetNoDock(PrimaryElement, false);
            }
        }
        private void ExecuteChangeState(object sender, ExecutedRoutedEventArgs e)
        {
            FireWindowVisibilityChanged();
            this.Visibility = Visibility.Collapsed;
            DockedElementTabbedHost host = InternalDataContext as DockedElementTabbedHost;
            FrameworkElement element = host.InternalDataContext as FrameworkElement;
            if (element != null)
            {
                DockingManager.ExecuteClose(element);
                DockingManager.SetFocus(element);
            }
        }

        private void SetDocument()
        {
            DockingManager owner = DockingManager;

            if (null != owner)
            {
                DockedElementTabbedHost host = InternalDataContext as DockedElementTabbedHost;
                FrameworkElement element = host.InternalDataContext as FrameworkElement;
                owner.ExecuteDocument(element);
            }
        }

        private void SetFloating()
        {
            FrameworkElement hostedElement = FindHostedElement();

            if (hostedElement != null)
            {
                DockState dockState = DockingManager.GetState(hostedElement);

                if (!DockingManager.GetNoDock(hostedElement))
                {
                    DockingManager owner = DockingManager.ResolveManager(hostedElement);

                    if (owner != null)
                    {
                        DockingManager.SetNoDock(hostedElement, true);
                        if (dockState != DockState.Float)
                        {
                            owner.ExtractElementToWindow(hostedElement, ActionMode.Active, false);
                            DockingManager.SetNewFocusedElement(hostedElement);
                            owner.LockLayoutUpdate = true;
                        }
                    }
                }
            }
        }
        void SystemMenu_Closed(object sender, RoutedEventArgs e)
        {
            UIElement hostedElement = FindHostedElement();
            if (hostedElement != null
                && (DockingManager.CollapseDefaultContextMenuItems || DockingManager.GetCollapseDefaultContextMenuItemsInFloat(hostedElement as DependencyObject)))
            {
                SystemMenu.Items.Clear();
            }
        }
        #endregion

        #region Initialization
        public NativeFloatWindow()
        {
           this.Loaded += new RoutedEventHandler(NativeFloatWindow_Loaded);
           this.IsMouseCapturedChanged += new DependencyPropertyChangedEventHandler(NativeFloatWindow_IsMouseCapturedChanged);
        }

        /// <summary>
        /// Find Escape key is pressed
        /// </summary>
        private void EscKeypressed()
        {            
            if (DockingManager != null && DockingManager.m_managerDragPreview != null && DockingManager.m_draggedElement != null && DockingManager.m_hostUnderMouse!=null)
            {
                DockPreviewRecord? record = DockingManager.m_managerDragPreview.FindDockingPlace(DockingManager.m_draggedElement, DockingManager.m_hostUnderMouse, Mouse.GetPosition(DockingManager.m_hostUnderMouse));
                if (record != null)
                {
                    if (!DockingManager.m_hostUnderMouse.TabChildren.Contains(DockingManager.m_draggedElement) ||
                        (DraggingType.NormalDragging != DockingManager.DraggingType && 1 == DockingManager.m_hostUnderMouse.TabChildren.Count &&
                        DockingManager.m_draggedElement != DockingManager.m_hostUnderMouse.TabChildren[0]))
                    {
                        DockingManager.CompleteDocking(Mouse.GetPosition(DockingManager.m_hostUnderMouse), true, false);
                    }
                    else if (DockingManager.IsDragItem(DockingManager.m_hostUnderMouse.TabChildren))
                    {
                        DockingManager.CompleteDocking(Mouse.GetPosition(DockingManager.m_hostUnderMouse), true, true);
                    }
                    else
                    {
                        DockingManager.CompleteDocking();
                    }
                }                
                else
                {
                    DockingManager.m_managerDragPreview.HideDockPreview();
                    DockingManager.m_managerDragPreview.HideDockPreviewMainButton(true);
                }
            }
           
        }

        void NativeFloatWindow_IsMouseCapturedChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (DockingManager!=null && DockingManager.m_managerDragPreview != null && DockingManager.m_managerDragPreview.dockpreview != null)
            {
                DockingManager.m_managerDragPreview.HideDockPreview();
            }
        }
        void NativeFloatWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Initialization();
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            this.BringIntoView();
        }
        private void Initialization()
        {
            if (DockingManager != null)
            {
                SkinStorage.SetVisualStyle(this, SkinStorage.GetVisualStyle(DockingManager));
                this.Owner = DockingManager.parentWindow;
            }
#if !SyncfusionFramework3_5
            {
              this.UseLayoutRounding = this.DockingManager.UseLayoutRounding;
              TextOptions.SetTextFormattingMode(this, TextOptions.GetTextFormattingMode(this.DockingManager));
            }
#endif
           
            if (this.Owner != null)
                SkinStorage.SetMetroBrush(this as DependencyObject, SkinStorage.GetMetroBrush(this.Owner as Window));
            this.ShowInTaskbar = false;
            FrameworkElement element = FindHostedElement();
            if (element != null)
            {
                WindowState = DockingManager.GetFloatWindowState(element);
                if (DockingManager.GetHeader(element as DependencyObject) != null)
                    this.Title = DockingManager.GetHeader(element as DependencyObject).ToString();
                this.Header = Title;
                this.SizeChanged += new SizeChangedEventHandler(NativeFloatWindow_SizeChanged);
                this.Unloaded += new RoutedEventHandler(NativeFloatWindow_Unloaded);
                this.Closing += new System.ComponentModel.CancelEventHandler(NativeFloatWindow_Closing);
                this.LocationChanged += new EventHandler(NativeFloatWindow_LocationChanged);
                this.MouseLeftButtonUp += new MouseButtonEventHandler(NativeFloatWindow_MouseLeftButtonUp);
#if !SyncfusionFramework3_5
                //this.TouchUp += NativeFloatWindow_TouchUp;
                //this.TouchMove += NativeFloatWindow_TouchMove;
#endif
                this.StateChanged += new EventHandler(NativeFloatWindow_StateChanged);
                this.MouseMove += new MouseEventHandler(NativeFloatWindow_MouseMove);
                m_windowPosition = new Rect(Left, Top, Width, Height);               
            }

            if (PrimaryElement != null)
            {
                MaximizeButtonEnabled = DockingManager.GetCanFloatMaximize(PrimaryElement);
                CalculateResizing();
            }
                   
        }

        void NativeFloatWindow_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.StylusDevice == null || e.StylusDevice != null)
            {
                DockingManager.m_mouseeventargs = e;
            }
        }
       
        void NativeFloatWindow_LocationChanged(object sender, EventArgs e)
        {
            Rect rect = new Rect((sender as Window).Left, (sender as Window).Top, Width, Height);
            if (PrimaryElement != null && (sender as NativeFloatWindow) != null && (sender as NativeFloatWindow).IsOpen)
                DockingManager.SetFloatingWindowRect(PrimaryElement, rect);
            if (!m_windowresizing && m_mouseLeftButtonDown)
            {
                IsDragging = true;
                bool bShowProviders = false;
                CaptureMouse();
               
                m_windowPosition = rect;
                Point p = Mouse.GetPosition(DockingManager);
                if (DockingManager.IsPointInsideElement(DockingManager))
                {
                    foreach (var host in DockingManager.m_usedHosts)
                    {
                        if (host !=null && host.InternalDataContext != null && host.State == DockState.Float)
                        {
                            Point n = Mouse.GetPosition(host);

                            if (DockingManager.IsPointInsideElement(host) && DockingManager.IsntSelf(host) && !DockingManager.GetNoDock(host.InternalDataContext as DependencyObject))
                            {
                                m_MouseUnderHost = host;
                                bShowProviders = true;
                                VisualTreeHelper.HitTest(host as Visual, null, new HitTestResultCallback(DockingManager.NativeWindowCallback), new PointHitTestParameters(n));
                            }
                        }
                    }

                    if (m_MouseUnderHost == null ||(m_MouseUnderHost!=null && !DockingManager.IsPointInsideElement(m_MouseUnderHost)))
                    {
                        bShowProviders = true;
                        VisualTreeHelper.HitTest(DockingManager as Visual, null, new HitTestResultCallback(DockingManager.NativeWindowCallback), new PointHitTestParameters(p));
                    }
                }

                else
                {
                    foreach (var host in DockingManager.m_usedHosts)
                    {
                        if (host != null && host.InternalDataContext != null)
                        {
                            Point n = Mouse.GetPosition(host);

                            if (DockingManager.IsPointInsideElement(host) && !DockingManager.GetNoDock(host.InternalDataContext as DependencyObject))
                            {
                                m_MouseUnderHost = host;
                                bShowProviders = true;
                                VisualTreeHelper.HitTest(host as Visual, null, new HitTestResultCallback(DockingManager.NativeWindowCallback), new PointHitTestParameters(n));
                            }

                        }
                    }
                }
                if (!DockingManager.IsPointInsideElement(DockingManager) && !bShowProviders)
                {
                    if (m_MouseUnderHost == null&&DockingManager.m_managerDragPreview!=null)
                    {
                        DockingManager.m_managerDragPreview.HideDockPreview();
                        DockingManager.m_managerDragPreview.HideDockPreviewMainButton(true);
                    }
                    if (m_MouseUnderHost != null && DockingManager.m_managerDragPreview != null && (m_MouseUnderHost!=null&&!DockingManager.IsPointInsideElement(m_MouseUnderHost)))
                    {
                        DockingManager.m_managerDragPreview.HideDockPreview();
                        DockingManager.m_managerDragPreview.HideDockPreviewMainButton(true);
                        DockingManager.m_hostUnderMouse = null;
                    }
                    DockingManager.m_hostUnderMouse = null;
                }
                DockingManager.StartDraggingNativeWindow((sender as NativeFloatWindow));
                FireWindowMovingEvent();
            }
        }

        void NativeFloatWindow_StateChanged(object sender, EventArgs e)
        {
            if (!DockingManager.GetCanFloatMaximize(PrimaryElement))
            {
                WindowState = WindowState.Normal;
            }
            DockingManager.SetFloatWindowState(PrimaryElement, WindowState);
            if (WindowState == WindowState.Normal)
            {
                DockingManager.SetFloatingWindowRect(PrimaryElement,DockingManager.GetPreviousFloatingWindowRect(PrimaryElement));
            }
        }

        internal bool IsOpenCallBack()
        {
            bool result = IsOpen;
            if (result && null != DockingManager && DockingManager.IsVisible)
               result = true;
            else
               result = false;

           return  result;
        }

        void NativeFloatWindow_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (e.StylusDevice == null || e.StylusDevice != null)
            {
                if (DockingManager.IsDragging)
                {
                    ReleaseMouseCapture();
                    DockingManager.m_nativeWindowDragging = false;
                    DockingManager.IsDragging = false;
                    IsDragging = false;
                }
            }
        }

        protected override void OnStylusSystemGesture(StylusSystemGestureEventArgs e)
        {
            m_nativeSystemGesture = e.SystemGesture;
            base.OnStylusSystemGesture(e);
        }

#if !SyncfusionFramework3_5
        //protected override void OnTouchEnter(TouchEventArgs e)
        //{
        //    m_nativeTouchDeviceId = (m_nativeTouchDeviceId == -1) ? e.TouchDevice.Id : m_nativeTouchDeviceId;
        //    base.OnTouchEnter(e);
        //}

        //protected override void OnTouchLeave(TouchEventArgs e)
        //{
        //    if (DockingManager != null && DockingManager.IsTouchEnabled && m_nativeTouchDeviceId == e.TouchDevice.Id)
        //    {
        //        m_nativeTouchDeviceId = -1;
        //        m_nativeSystemGesture = SystemGesture.None;
        //        base.OnTouchLeave(e);
        //    }
        //}

        //void NativeFloatWindow_TouchUp(object sender, TouchEventArgs e)
        //{
        //    if (DockingManager != null && DockingManager.IsTouchEnabled && m_nativeTouchDeviceId == e.TouchDevice.Id)
        //    {
        //        #region NativeFloatWindow_TouchLeftFingerUp
        //        if (m_nativeSystemGesture == SystemGesture.Tap)
        //        {
        //            if (DockingManager.IsDragging)
        //            {
        //                ReleaseMouseCapture();
        //                DockingManager.m_nativeWindowDragging = false;
        //                DockingManager.IsDragging = false;
        //                IsDragging = false;
        //            }
        //        }
        //        #endregion
        //    }
        //}

        //void NativeFloatWindow_TouchMove(object sender, TouchEventArgs e)
        //{
        //    if (DockingManager != null && DockingManager.IsTouchEnabled && m_nativeTouchDeviceId == e.TouchDevice.Id)
        //        DockingManager.m_mouseeventargs = e;
        //}
#endif

        
        protected override void OnContentChanged(object oldContent, object newContent)
        {
            FrameworkElement content = newContent as FrameworkElement;

            if (newContent is DockedElementTabbedHost)
            {
                DockedElementTabbedHost host = newContent as DockedElementTabbedHost;
               
                DockedElementsContainer container = new DockedElementsContainer(DockingManager);
                if (host.Parent != null && (host.Parent as DockedElementsContainer) != null)
                {
                    (host.Parent as DockedElementsContainer).RemoveChild(host);
                }
                container.Children.Add(host);
                content = container;
            }

            this.Content = content;
        }

        /// <summary>
        /// Invoke <see cref="System.Windows.Input.Keyboard.PreviewKeyDown"/> on the DockingManager first,
        /// since the native float window is logically a child of the DockingManager.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.KeyEventArgs"/> that contains the event data.</param>
        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            this.DockingManager.RaiseEvent(e);
            if (!e.Handled)
            {
                base.OnPreviewKeyDown(e);
            }
        }

        /// <summary>
        /// Invoke <see cref="System.Windows.Input.Keyboard.KeyDown"/> on the DockingManager,
        /// since the native float window is logically a child of the DockingManager.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.KeyEventArgs"/> that contains the event data.</param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (!e.Handled)
            {
                this.DockingManager.RaiseEvent(e);
            }
        }

        /// <summary>
        /// Invoke <see cref="System.Windows.Input.Keyboard.PreviewKeyUp"/> on the DockingManager first,
        /// since the native float window is logically a child of the DockingManager.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.KeyEventArgs"/> that contains the event data.</param>
        protected override void OnPreviewKeyUp(KeyEventArgs e)
        {
            this.DockingManager.RaiseEvent(e);
            if (!e.Handled)
            {
                base.OnPreviewKeyUp(e);
            }
        }

        /// <summary>
        /// Invoke <see cref="System.Windows.Input.Keyboard.KeyUp"/> on the DockingManager,
        /// since the native float window is logically a child of the DockingManager.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.KeyEventArgs"/> that contains the event data.</param>
        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
            if (!e.Handled)
            {
                this.DockingManager.RaiseEvent(e);
            }
        }

        private bool ActiveWindowCalculations()
        {
            UIElement element = (UIElement)this.InternalDataContext;
            NativeFloatWindow ParentWindow = this;
            if (null != element && ParentWindow != null && ParentWindow.PrimaryElement != null)
            {
                #region setting active window calculations
                ActiveWindowChangingEventArgs args = new ActiveWindowChangingEventArgs();

                if ((element as DockedElementTabbedHost) != null && (element as DockedElementTabbedHost).InternalDataContext != null)
                    args.NewValue = (element as DockedElementTabbedHost).InternalDataContext as FrameworkElement;
                else
                    args.NewValue = ParentWindow.PrimaryElement as FrameworkElement;
                args.OldValue = ParentWindow.DockingManager.ActiveWindow;
                if (args.OldValue != args.NewValue)
                {
                    ParentWindow.DockingManager.FireActiveWindowChanging(args.NewValue, args);
                    if (!args.Cancel)
                    {
                        ParentWindow.DockingManager.ActiveWindow = args.NewValue;
                    }
                    else
                    {
                        return false;   
                    }
                }
                else
                {
                    ParentWindow.DockingManager.ActiveWindow = args.NewValue;
                }
                element.Focus();
                #endregion
            }
            return true;
        }


        void NativeFloatWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this.DockingManager != null && this.PrimaryElement != null)
            {
                WindowClosingEventArgs close = new WindowClosingEventArgs();
                if (close != null)
                {
                    if (this.InternalDataContext != null && (this.InternalDataContext as DockedElementTabbedHost) != null)
                    {
                        var target=(this.InternalDataContext as DockedElementTabbedHost);
                        if (target.InternalDataContext != null && (target.InternalDataContext as FrameworkElement) != null)
                            close.TargetItem = target.InternalDataContext;
                        else if (this.PrimaryElement != null)
                            close.TargetItem = this.PrimaryElement;
                    }
                    else if (this.PrimaryElement != null)
                    {
                        close.TargetItem = this.PrimaryElement;
                    }
                }
                if (close.TargetItem != null)
                {
                    DockState state = DockingManager.GetState(close.TargetItem);
                    DockedElementTabbedHost host = DockingManager.GetHost(close.TargetItem, state);
                    if (state == DockState.Float && host != null && host.InternalTabControl != null)
                    {
                        DockingManager.ExecuteClose(close.TargetItem);
                        e.Cancel = true;
                    }
                    else
                    {
                        this.DockingManager.FireWindowClosingEvent(DockingManager, close);
                        if (close.Cancel)
                        {
                            e.Cancel = close.Cancel;
                        }
                        else
                        {
                            this.IsOpen = false;
                            this.DockingManager.ExecuteClose(this);
                        }
                    }
                }               
            }
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            CommandBinding bindingClose = new CommandBinding(CloseCommand, ExecuteClose);
            CommandBindings.Add(bindingClose);

            CommandBinding bindingMaximize = new CommandBinding(MaximizeCommand, ExecuteMaximize);
            CommandBindings.Add(bindingMaximize);

            CommandBinding bindingRestore = new CommandBinding(RestoreCommand, ExecuteRestore);
            CommandBindings.Add(bindingRestore);
        }
        private void ExecuteMaximize(object sender, ExecutedRoutedEventArgs e)
        {
            this.WindowState = WindowState.Maximized;
        }
        private void ExecuteRestore(object sender, ExecutedRoutedEventArgs e)
        {
            this.WindowState = WindowState.Normal;
        }
        private void ExecuteClose(object sender, ExecutedRoutedEventArgs e)
        {
            if (!DockingManager.m_NativeWindowsUnRegistered.Contains(this))
                DockingManager.m_NativeWindowsUnRegistered.Add(this);
            this.Close();           
        }
        internal void RemoveHandled()
        {
            SystemMenu.Items.Clear();
            SystemMenu.Opened -= new RoutedEventHandler(SystemMenu_Opened);
            SystemMenu.Closed -= new RoutedEventHandler(SystemMenu_Closed);
            this.Loaded -= new RoutedEventHandler(NativeFloatWindow_Loaded);
            this.Unloaded -= new RoutedEventHandler(NativeFloatWindow_Unloaded);
            this.SizeChanged -= new SizeChangedEventHandler(NativeFloatWindow_SizeChanged);
        }
        void NativeFloatWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            RemoveHandled();
        }
       
        #endregion

        #region WindowRect and size
       
        private void FireWindowMovingEvent()
        {
            if (this.PrimaryElement != null)
            {
                DockInfoInternal info = DockingManager.GetDockInfo(this.PrimaryElement as DependencyObject);

                if (info != null)
                {
                    WindowMovingEventArgs args = new WindowMovingEventArgs();
                    args.State = DockState.Float;
                    if (this.DockingManager != null)
                    {
                        if (this.PrimaryElement.IsVisible)
                        {
                            args.X = this.PrimaryElement.PointToScreen(new Point(0, 0)).X;
                            args.Y = this.PrimaryElement.PointToScreen(new Point(0, 0)).Y;
                        }
                    }
                    info.DockingManager.FireWindowMoving(this.PrimaryElement, args);
                }
            }
        }


        private void ValidateResizing(FrameworkElement element)
        {
            
        }

        void NativeFloatWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (WindowState == WindowState.Maximized && !DockingManager.m_loadingState)
            {
                if (PrimaryElement != null)
                    DockingManager.SetPreviousFloatingWindowRect(PrimaryElement as DependencyObject, DockingManager.GetFloatingWindowRect(PrimaryElement as DependencyObject));
            }
            else
            {
                Rect rect = new Rect((sender as Window).Left, (sender as Window).Top, Width, Height);
                if (PrimaryElement != null)
                    DockingManager.SetFloatingWindowRect(PrimaryElement as DependencyObject, rect);
            }
            
            
            WindowResizingEventArgs args = new WindowResizingEventArgs();
            args.DesiredHeight = e.NewSize.Height;
            args.DesiredWidth = e.NewSize.Width;
            args.State = DockState.Float;
            DockingManager.FireWindowResizingEvent(this, args);
        }
        #endregion

        
        #region NativeMethods
        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
  
            if (msg == WM_SIZING)
            {
                m_windowresizing = true;
                handled = true;
            }
            if (msg == (uint)WM.NCHITTEST)
            {
                var htLocation = DefWindowProc(hwnd, msg, wParam, lParam).ToInt32();
                  
                if (htLocation == 10 || htLocation == 12 || htLocation == 11 || htLocation == 15 || htLocation == 13 || htLocation == 14 || htLocation == 16 || htLocation == 17)
                {
                    m_windowresizing = true;
                }
                else
                {
                    m_windowresizing = false;
                }
            }

            if ((msg == WM_SYSTEMMENU || msg == WM_CONTEXTMENU) && wParam.ToInt32() == WP_SYSTEMMENU) 
            {
                ShowContextMenu();
                handled = true;
            }
            
            if (msg == WM_NCLBUTTONDBLCLK)
            {

                bool bNoDock = DockingManager.GetNoDock(PrimaryElement);
                bool canDock = DockingManager.GetCanDock(PrimaryElement);
                bool canFloatMaximize=DockingManager.GetCanFloatMaximize(PrimaryElement);
                if (!bNoDock && canDock)
                {
                    DockingManager.ExecuteDoubleClickOnNative(this);
                    DockingManager.IsDragging = false;
                    handled = true;
                }
                else if (canFloatMaximize)
                {
                    this.WindowState = WindowState.Maximized;
                    handled = true;
                }
                else
                {
                    handled = true;
                }
            }
           
            if (msg == WM_NCLBUTTONDOWN)
            {
                    m_mouseLeftButtonDown = true;
                if (ActiveWindowCalculations())
                {
                    handled = false;
                }
                else
                {
                    handled = true;
                }
            }
            if (msg == WM_NCRBUTTONDOWN)
            {
                if (ActiveWindowCalculations())
                {
                    handled = false;
                }
                else
                {
                    handled = true;
                }
            }
            if (msg == 160)
            {
                m_mouseLeftButtonDown = false;
            }
            if (msg == WM_NCMOUSELEAVE)
            {
                ReleaseMouseCapture();
                IsDragging = false;   
            }
            if ((int)wParam == VK_ESCAPE)
            {
                EscKeypressed();
            }
                      
            return IntPtr.Zero;
        }
        #endregion

        private void CalculateResizing()
        {

            if (!DockingManager.GetCanResizeInFloatState(PrimaryElement))
            {
                ResizeMode = ResizeMode.NoResize;
            }
            if (!DockingManager.GetCanResizeHeightInFloatState(PrimaryElement) && DockingManager.GetCanResizeWidthInFloatState(PrimaryElement))
            {
                MinHeight = MaxHeight = Height;
            }
            if (!DockingManager.GetCanResizeWidthInFloatState(PrimaryElement) && DockingManager.GetCanResizeHeightInFloatState(PrimaryElement))
            {
                MinWidth = MaxWidth = Width;
            }
            if (!DockingManager.GetCanResizeHeightInFloatState(PrimaryElement) && !DockingManager.GetCanResizeWidthInFloatState(PrimaryElement))
            {
                ResizeMode = ResizeMode.NoResize;
            }
            if (DockingManager.GetCanResizeInFloatState(PrimaryElement))
            {
                ResizeMode = ResizeMode.CanResize;
            }
            if (DockingManager.GetCanResizeHeightInFloatState(PrimaryElement))
            {
                MinHeight = 0.0;
                MaxHeight = double.PositiveInfinity;
            }
            if (DockingManager.GetCanResizeWidthInFloatState(PrimaryElement))
            {
                MinWidth = 0.0;
                MaxWidth = double.PositiveInfinity;
            } 
        }

        #region Dependency property change

        private static void OnIsDraggingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NativeFloatWindow window = d as NativeFloatWindow;
            window.OnIsDraggingChangedProperty(e);
        }
        private void OnIsDraggingChangedProperty(DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                DockingManager.m_draggedElement = PrimaryElement;
            }
            else
            {
                Point p = Mouse.GetPosition(DockingManager);
                if (DockingManager.IsPointInsideElement(DockingManager))
                {
                    VisualTreeHelper.HitTest(this.DockingManager as Visual, null, new HitTestResultCallback(DockingManager.NativeWindowCallback), new PointHitTestParameters(p));
                }
                    
                else if (m_MouseUnderHost != null && DockingManager.IsPointInsideElement(m_MouseUnderHost)) 
                {
                       Point n = Mouse.GetPosition(m_MouseUnderHost);
                       VisualTreeHelper.HitTest(m_MouseUnderHost as Visual, null, new HitTestResultCallback(DockingManager.NativeWindowCallback), new PointHitTestParameters(n));
                }
                else 
                {
                    DockingManager.m_managerDragPreview.HideDockPreview();
                    DockingManager.m_managerDragPreview.HideDockPreviewMainButton(true);
                    DockingManager.m_hostUnderMouse = null;
                    DockingManager.IsDragging = false;
                    DockingManager.m_draggedElement = null;
                    this.Focus();
                    ReleaseMouseCapture();
                }
                DockingManager.IsDragging = false;
            }
        }
        private static void OnPlacementRectangleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NativeFloatWindow window = d as NativeFloatWindow;
            window.OnPlacementRectangleChanged(e);
        }

        private void OnPlacementRectangleChanged(DependencyPropertyChangedEventArgs e)
        {
            if ((Rect)(e.NewValue) != Rect.Empty)
            {
                Rect temp = (Rect)(e.NewValue);
                Width = temp.Width;
                Height = temp.Height;
                Left = temp.X;
                Top = temp.Y;
            }
            else
            {
                Width = 150;
                Height = 100;
                Left = 0;
                Top = 0;
            }
        }
        #endregion

        #region WindowDragging
               
        private void WindowDragging()
        {
            DockingManager.IsDragging = true;
        }
        #endregion
      
        #region Implementation

        internal void FireWindowVisibilityChanged()
        {
            RoutedEventArgs args = new RoutedEventArgs(DockingManager.WindowVisibilityChangedEvent);
            RaiseEvent(args);
        }

        private FrameworkElement FindHostedElement()
        {
            FrameworkElement returnElement = null;
            FrameworkElement element = InternalDataContext as FrameworkElement;

            if (null != element)
            {
                returnElement = DockingManager.GetInternalDataContext(element);
                DataContext = element;
            }
            return returnElement;
        }
        internal static void SetPrimaryElementAsDataContext(NativeFloatWindow window)
        {
            if (window != null && window.PrimaryElement != null && (window.PrimaryElement as DependencyObject) != null)
            {
                DockInfoInternal info = DockingManager.GetDockInfo(window.PrimaryElement);
                DockedElementTabbedHost host = info.HostFloat;

                if (host != null)
                {
                    host.Focus();
                }

                window.InternalDataContext = host;
            }
        }
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            IntPtr mainWindowPtr = new WindowInteropHelper(this).Handle;
            HwndSource mainWindowSrc = HwndSource.FromHwnd(mainWindowPtr);
            mainWindowSrc.AddHook(WndProc);
        }
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

        public void UpdateDataContext()
        {
            DockedElementsContainer container = Content as DockedElementsContainer;
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

        public void UpdateIsMultiHostProperty()
        {
            DockedElementsContainer container = this.Content as DockedElementsContainer;
            int hostsCount = 0;
            if (container != null)
            {
                hostsCount = GetVisibleHostsCount(container);
            }

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
                    DockingManager.SetNoHeader(tab, !IsMultiHostsContainer);
                }
            }
            else
            {
                IsOpen = false;
            }
        }
        public void SetNewPrimaryElement(FrameworkElement element)
        {
            if (element != null)
            {
                Rect rect = DockingManager.GetFloatingWindowRect(PrimaryElement);
                DockingManager.SetFloatingWindowRect(element, rect);
                InternalDataContext = DockingManager.GetTabbedHost(element, DockState.Float);

                BindingUtils.SetBinding(this, element, NativeFloatWindow.InternalPlacementRectProperty, DockingManager.FloatingWindowRectProperty, BindingMode.TwoWay);
            }

            PrimaryElement = element;
         if (PrimaryElement != null) 
            this.Title = PrimaryElement.Name;
        }
        #endregion
    }
}