// <copyright file="FloatWindow.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interop;
using Syncfusion.Licensing;
using Syncfusion.Windows.Shared;
using System.Windows.Threading;
using System.Threading;
using System.Diagnostics;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents docking manager's floating window and helper frame
    /// internal window.
    /// </summary>
    /// <example>
    /// 	<para/>This example shows how to use FloatWindow in C#.
    /// <code language="C#">
    /// FloatWindow fw = new FloatWindow( dockingManager, true );
    /// fw.Height = 100;
    /// fw.Width = 50;
    /// dockingManager.Children.Add( fw );
    /// </code>
    /// 	<para/>This example shows how to initialize the style of the FloatWindow class in XAML.
    /// <code language="XAML">
    /// 		<![CDATA[
    /// <Style x:Key="{x:Type Syncfusion:FloatWindow}" TargetType="{x:Type ContentControl}">
    /// <Setter Property="Template" Value="{StaticResource FloatWindowTemplate}" />
    /// </Style>
    /// ]]>
    /// 	</code>
    /// 	<para/>This example shows how to initialize the template of the FloatWindow class in XAML.
    /// <code language="XAML">
    /// 		<![CDATA[
    /// <ControlTemplate x:Key="FloatWindowTemplate" TargetType="{x:Type ContentControl}">
    /// <AdornerDecorator>
    /// <DockPanel Focusable="False" LastChildFill="True" >
    /// <Border Name="FloatWindowOutBorder"  Focusable="False" BorderBrush="Red" BorderThickness="5" Background="Gray">
    /// <Grid Focusable="False">
    /// <Grid.RowDefinitions>
    /// <RowDefinition Name="TopRow" Height="25" />
    /// <RowDefinition Height="*" />
    /// <RowDefinition Name="BottomRow" Height="4" />
    /// </Grid.RowDefinitions>
    /// <Grid.ColumnDefinitions>
    /// <ColumnDefinition Name="LeftCol" Width="4" />
    /// <ColumnDefinition Width="*" />
    /// <ColumnDefinition Name="RightCol" Width="4" />
    /// </Grid.ColumnDefinitions>
    /// <Syncfusion:FloatWindowBorder BorderMode="LeftTop" Name="BorderLeftTop" Grid.Column="0" Grid.Row="0" />
    /// <Syncfusion:FloatWindowBorder BorderMode="Header" Name="BorderHeader" Grid.Column="1" Grid.Row="0" />
    /// <Syncfusion:FloatWindowBorder BorderMode="RightTop" Name="BorderRightTop" Grid.Column="2" Grid.Row="0" />
    /// <Syncfusion:FloatWindowBorder BorderMode="Left" Name="BorderLeft" Grid.Column="0" Grid.Row="1" />
    /// <ContentPresenter Name="ContentPresenter"  Grid.Column="1" Grid.Row="1"
    /// ContentTemplate="{TemplateBinding ContentControl.ContentTemplate}"
    /// Content="{TemplateBinding ContentControl.Content}" />
    /// <Syncfusion:FloatWindowBorder BorderMode="Right" Name="BorderRight" Grid.Column="2" Grid.Row="1" />
    /// <Syncfusion:FloatWindowBorder BorderMode="LeftBottom" Name="BorderLeftBottom" Grid.Column="0" Grid.Row="2" />
    /// <Syncfusion:FloatWindowBorder BorderMode="Bottom" Name="BorderBottom" Grid.Column="1" Grid.Row="2" />
    /// <Syncfusion:FloatWindowBorder BorderMode="RightBottom" Name="BorderRightBottom" Grid.Column="2" Grid.Row="2" />
    /// </Grid>
    /// </Border>
    /// </DockPanel>
    /// </AdornerDecorator>
    /// </ControlTemplate>
    /// ]]>
    /// 	</code>
    /// </example>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class FloatWindow : NonStickingPopup, IWindow
    {
        #region Private members
        /// <summary>
        /// FIX:Specifies the Variable used to track the no of times float window focus
        /// </summary>
        private int Temp = 0;

        /// <summary>
        /// Specifies hwnd source hook.
        /// </summary>
        private readonly HwndSourceHook m_hookHitTestDisabling;

        /// <summary>
        /// Specifies UI Element.
        /// </summary>
        private UIElement m_header;

        /// <summary>
        /// Specifies hit test.
        /// </summary>
        private bool m_isHitTestDisabled;

        private Window parentWindow;

        #endregion

        #region Properties
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

        /// <summary>
        /// Gets or sets a value indicating whether Float container for window.
        /// </summary>
        /// <value>The float child.</value>
        /// <property name="flag" value="Finished"/>
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
                    ////FrameworkElement hostChild = host.TabChildren[ 0 ];

                    ////if( hostChild != null && DockingManager.GetSideInDockedMode( hostChild ) == DockSide.Tabbed )
                    ////{
                    ////    host.HostedElement = null;
                    ////    host.HostedElement = hostChild;
                    ////}

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
        /// Gets or sets a value indicating whether HitTestDisabled of the <see cref="FloatWindow"/>.
        /// </summary>
        /// <value><c>true</c> if [hit test disabled]; otherwise, <c>false</c>.</value>
        public bool HitTestDisabled
        {
            get
            {
                return m_isHitTestDisabled;
            }

            set
            {
                if (m_isHitTestDisabled != value)
                {
                    if (EnvironmentTest.IsSecurityGranted)
                    {
                        HwndSource source = (HwndSource)PresentationSource.FromVisual(Child);

                        if (source != null)
                        {
                            m_isHitTestDisabled = value;

                            if (m_isHitTestDisabled)
                            {
                                Mouse.OverrideCursor = Cursors.Arrow;
                                source.AddHook(m_hookHitTestDisabling);
                            }
                            else
                            {
                                Mouse.OverrideCursor = null;
                                source.RemoveHook(m_hookHitTestDisabling);
                            }
                        }
                    }
                    else
                    {
                        m_isHitTestDisabled = value;
                    }
                }
            }
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
        /// Gets or sets the header.
        /// </summary>
        /// <value>The header.</value>
        public UIElement Header
        {
            get
            {
                return m_header;
            }

            set
            {
                m_header = value as FloatWindowBorder;
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

        internal Rect InternalPlacementRect
        {
            get { return (Rect)GetValue(InternalPlacementRectProperty); }
            set { SetValue(InternalPlacementRectProperty, value); }
        }

        // Using a DependencyProperty as the backing store for InternalPlacementRect.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty InternalPlacementRectProperty =
            DependencyProperty.Register("InternalPlacementRect", typeof(Rect), typeof(FloatWindow), new FrameworkPropertyMetadata(new Rect(0.0,0.0,0.0,0.0),new PropertyChangedCallback(OnPlacementRectangleChanged)));

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes static members of the FloatWindow class.
        /// </summary>
        static FloatWindow()
        {
            IsOpenProperty.OverrideMetadata(typeof(FloatWindow), new FrameworkPropertyMetadata(false, null, new CoerceValueCallback(CoerceIsOpenCallback)));
        }



        /// <summary>
        /// structure for the rectrangle.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            /// <summary>
            /// Represent the left
            /// </summary>
            public int Left;

            /// <summary>
            /// Represent the top
            /// </summary>
            public int Top;

            /// <summary>
            /// Represent the right
            /// </summary>
            public int Right;

            /// <summary>
            /// Represent the bottom
            /// </summary>
            public int Bottom;
        }

        /// <summary>
        /// Gets the window rect.
        /// </summary>
        /// <param name="hWnd">The h WND.</param>
        /// <param name="lpRect">The lp rect.</param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        /// <summary>
        /// Sets the window pos.
        /// </summary>
        /// <param name="hWnd">The h WND.</param>
        /// <param name="hwndInsertAfter">The HWND insert after.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="cx">The cx.</param>
        /// <param name="cy">The cy.</param>
        /// <param name="wFlags">The w flags.</param>
        /// <returns></returns>
        [DllImport("user32", EntryPoint = "SetWindowPos")]
        private static extern int SetWindowPos(IntPtr hWnd, int hwndInsertAfter, int x, int y, int cx, int cy, int wFlags);

        /// <summary>
        /// Initializes a new instance of the FloatWindow class.
        /// </summary>
        /// <param name="docking">The docking.</param>
        /// <param name="isAllowsTransparency">if set to <c>true</c> [is allows transparency].</param>
        public FloatWindow(DockingManager docking, bool isAllowsTransparency)
        {
            ////PrimaryElement = docking;
            DockingManager = docking;
            AllowsTransparency = isAllowsTransparency;
            Placement = PlacementMode.Absolute;

            //// FIX: Such dummy setting of the property value is needed to enable it's repositioning in future. The reason of such behavior should be investigated later.
            PlacementRectangle = new Rect(1, 1, 1, 1);
            AutoTemplatedContentControl child = new AutoTemplatedContentControl(GetType());
            BindingUtils.SetBinding(child, docking, SkinStorage.VisualStyleProperty, SkinStorage.VisualStyleProperty);
            Child = child;

            Binding bind = new Binding();
            bind.Path = new PropertyPath("PlacementRectangle");
            bind.RelativeSource = new RelativeSource { Mode = RelativeSourceMode.Self };
            bind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            SetBinding(FloatWindow.InternalPlacementRectProperty, bind);

            m_hookHitTestDisabling = new HwndSourceHook(HookMethod);
            this.Loaded += new RoutedEventHandler(FloatWindow_Loaded);
            
        }

        void FloatWindow_Loaded(object sender, RoutedEventArgs e)
        {
            HookEvents();
        }

        private void HookEvents()
        {
            Opened += new EventHandler(FloatWindow_Opened);
            this.Unloaded += new RoutedEventHandler(FloatWindow_Unloaded);
        }

        void FloatWindow_Opened(object sender, EventArgs e)
        {
            parentWindow = getParentWindow();
            HookWindowEvents();
        }

        private void HookWindowEvents()
        {
            if (parentWindow != null)
            {
                parentWindow.Unloaded += new RoutedEventHandler(parentWindow_Unloaded);
                if (Is64bitOS)
                {
                    parentWindow.Deactivated += new EventHandler(parentWindow_Deactivated);
                }
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            UnHookWindowEvents();
            base.OnClosed(e);
        }

        private void UnHookWindowEvents()
        {
            if (parentWindow != null)
            {
                parentWindow.Unloaded -= new RoutedEventHandler(parentWindow_Unloaded);
                parentWindow.Deactivated -= new EventHandler(parentWindow_Deactivated);
            }
        }

        void parentWindow_Deactivated(object sender, EventArgs e)
        {
            if (this.Child != null)
            {
                HwndSource temphwnd = ((HwndSource)PresentationSource.FromVisual(this.Child));
                if (temphwnd != null)
                {
                    var hwnd = temphwnd.Handle;
                    RECT rect;

                    if (GetWindowRect(hwnd, out rect))
                    {
                        SetWindowPos(hwnd, -2, rect.Left, rect.Top, (int)this.Width, (int)this.Height, 0);
                    }
                }
            }
        }

        // SD 12534 - Code removed for Empty float window issue.
        void parentWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            this.Unloaded -= new RoutedEventHandler(FloatWindow_Unloaded);
            if (m_header != null)
            m_header = null;
            if (Header != null)
                Header = null;

            Dispatcher.BeginInvoke(DispatcherPriority.Background, (ThreadStart)delegate
            {
                GC.Collect();
            });

        }

        /// <summary>
        /// Handles the Unloaded event of the FloatWindow control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The instance containing the event data.</param>
        void FloatWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            DisposeFloatWindow();
            Opened -= new EventHandler(FloatWindow_Opened);
        }

        internal void DisposeFloatWindow()
        {
            Header = null;
            this.Unloaded -= new RoutedEventHandler(FloatWindow_Unloaded);
            UnHookWindowEvents();
        }

        /// <summary>
        /// Clears the docking manager.
        /// </summary>
        internal void ClearDockingManager()
        {
            PrimaryElement = null;
        }

        /// <summary>
        /// protected variable for iscalled flag
        /// </summary>
        protected internal bool IsCalled = false;

        /// <summary>
        /// Gets a value indicating whether [is64bit OS].
        /// </summary>
        /// <value><c>true</c> if [is64bit OS]; otherwise, <c>false</c>.</value>
        public bool Is64bitOS
        {
            get { return (Environment.GetEnvironmentVariable("ProgramFiles(x86)") != null); }
        }

        /// <summary>
        /// Gets the parent window.
        /// </summary>
        /// <returns></returns>
        protected internal Window getParentWindow()
        {
            DependencyObject d = DockingManager;
            while (d != null && !(d is Window))
                d = LogicalTreeHelper.GetParent(d);
            return d as Window;
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Gets the foreground window.
        /// </summary>
        /// <returns>return Intptr value.</returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr GetForegroundWindow();
        #endregion

        #region Implementation
        /// <summary>
        /// Puts the popup on the top of all other floating windows.
        /// </summary>
        /// <property name="flag" value="Finished"/>
        public void SetWindowOnTop()
        {
            if (PermissionHelper.HasUnmanagedCodePermission)
            {
                SetWindowOnTopSecure();
            }
        }

        /// <summary>
        /// Sets the window no activate on top.
        /// </summary>
        internal void SetWindowNoActivateOnTop()
        {
            if (PermissionHelper.HasUnmanagedCodePermission)
            {
                HwndSource src = (HwndSource)PresentationSource.FromVisual(Child);

                if (src != null)
                {
                    IntPtr hwnd = src.Handle;
                    //NativeMethods.SetWindowPos(hwnd, NativeConstants.HWND_TOP, 0, 0, 0, 0, NativeConstants.SWP_NOSIZE | NativeConstants.SWP_NOMOVE);
                    NativeMethods.SetWindowPos(hwnd, NativeConstants.HWND_NOTOPMOST, 0, 0, 0, 0, NativeConstants.SWP_NOSIZE | NativeConstants.SWP_NOMOVE);
                }
            }
        }

        /// <summary>
        /// Raises FireBeforeContextMenu event.
        /// </summary>
        internal void FireBeforeContextMenuOpen()
        {
            RoutedEventArgs args = new RoutedEventArgs(DockingManager.BeforeContextMenuOpenEvent);
            RaiseEvent(args);
        }

        /// <summary>
        /// Sets the primary element as data context.
        /// </summary>
        /// <param name="window">The window.</param>
        internal static void SetPrimaryElementAsDataContext(IWindow window)
        {
            if (window!=null && window.PrimaryElement != null && (window.PrimaryElement as DependencyObject) != null)
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

        /// <summary>
        /// Activates the parent.
        /// </summary>
        void ActivateParent()
        {
            FrameworkElement element = Parent as FrameworkElement;
            bool bActivated = false;
            if (element != null)
            {
                if (element.Parent != null)
                {
                    while (element.Parent != null)
                    {
                        element = element.Parent as FrameworkElement;
                        if (element != null && element is Window)
                        {
                            (element as Window).Activate();
                            bActivated = true;
                            break;
                        }
                    }
                }
                if (!bActivated )
                {
                    Window win = VisualUtils.FindAncestor(element, typeof(Window)) as Window;
                    if (win != null)
                    {
                        win.Activate();
                    }
                }
            }

        }


        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            if (DockingManager != null && e.StylusDevice == null || e.StylusDevice != null)
            {
                if (!BrowserInteropHelper.IsBrowserHosted)
                {
                    ActivateParent();
                }
                Keyboard.Focus(this);
                this.Focus();

                SetWindowOnTop();
                DockingManager.m_WindowOrder.Remove(this);
                DockingManager.m_WindowOrder.Add(this);
                DockingManager.UpdateZorderInFloatMode();
                FrameworkElement host = e.Source as DockedElementTabbedHost;

                if (null != host)
                {
                    InternalDataContext = host;
                    //host.Focus();
                    if (!host.IsKeyboardFocusWithin)
                    {
                        host.Focus();
                    }
                }
                else if (!IsKeyboardFocusWithin)
                {
                    DockedElementTabbedHost contextHost = (DockedElementTabbedHost)InternalDataContext;

                    if (null != contextHost)
                    {
                        contextHost.Focus();
                    }
                    else
                    {
                        FloatWindow.SetPrimaryElementAsDataContext(this);
                    }
                }
            }
            base.OnPreviewMouseDown(e);
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.StylusDevice == null || e.StylusDevice != null)
            {
                if (e.RightButton == MouseButtonState.Pressed)
                {
                    FireBeforeContextMenuOpen();
                }
            }
        }
        int m_TouchDeviceId = -1;
        internal SystemGesture m_floatWindowSystemGesture;

        protected override void OnStylusSystemGesture(StylusSystemGestureEventArgs e)
        {
            m_floatWindowSystemGesture = e.SystemGesture;
            base.OnStylusSystemGesture(e);
        }

# if !SyncfusionFramework3_5
        protected override void OnTouchMove(TouchEventArgs e)
        {
            if (DockingManager != null && DockingManager.IsTouchEnabled && m_TouchDeviceId == e.TouchDevice.Id)
            {
                if (m_floatWindowSystemGesture == SystemGesture.HoldEnter)
                    OnTouchRightFingerDown(e);
                base.OnTouchMove(e);
            }
        }

        protected override void OnTouchEnter(TouchEventArgs e)
        {
            m_TouchDeviceId = (m_TouchDeviceId == -1) ? e.TouchDevice.Id : m_TouchDeviceId;
            base.OnTouchEnter(e);
        }

        protected override void OnTouchLeave(TouchEventArgs e)
        {
            if (DockingManager != null && DockingManager.IsTouchEnabled && m_TouchDeviceId == e.TouchDevice.Id)
            {
                m_TouchDeviceId = -1;
                m_floatWindowSystemGesture = SystemGesture.None;
                base.OnTouchLeave(e);
            }
        }

        protected override void OnTouchUp(TouchEventArgs e)
        {
            if (DockingManager != null && DockingManager.IsTouchEnabled && m_TouchDeviceId == e.TouchDevice.Id)
            {
                m_TouchDeviceId = -1;
                m_floatWindowSystemGesture = SystemGesture.None;
                base.OnTouchUp(e);
            }
        }

        protected override void OnPreviewTouchDown(TouchEventArgs e)
        {
            if (DockingManager != null && DockingManager.IsTouchEnabled && m_TouchDeviceId == e.TouchDevice.Id)
            {
                if (!BrowserInteropHelper.IsBrowserHosted)
                {
                    ActivateParent();
                }
                Keyboard.Focus(this);
                this.Focus();

                SetWindowOnTop();
                DockingManager.m_WindowOrder.Remove(this);
                DockingManager.m_WindowOrder.Add(this);
                DockingManager.UpdateZorderInFloatMode();
                FrameworkElement host = e.Source as DockedElementTabbedHost;

                if (null != host)
                {
                    InternalDataContext = host;
                    //host.Focus();
                    if (!host.IsKeyboardFocusWithin)
                    {
                        host.Focus();
                    }
                }
                else if (!IsKeyboardFocusWithin)
                {
                    DockedElementTabbedHost contextHost = (DockedElementTabbedHost)InternalDataContext;

                    if (null != contextHost)
                    {
                        contextHost.Focus();
                    }
                    else
                    {
                        FloatWindow.SetPrimaryElementAsDataContext(this);
                    }
                }
            }
            base.OnPreviewTouchDown(e);
        }

        private void OnTouchRightFingerDown(TouchEventArgs e)
        {
            FireBeforeContextMenuOpen();
        }
#endif

        /// <summary>
        /// Raises the Opened event.
        /// </summary>
        /// <param name="e">An EventArgs that contains the event data.</param>
        protected override void OnOpened(EventArgs e)
        {
            if (PermissionHelper.HasUnmanagedCodePermission)
            {
                OnOpenedSecure();
            }

            base.OnOpened(e);
        }

        /// <summary>
        /// Raises the GotKeyboardFocus event.
        /// </summary>
        /// <param name="e">An KeyboardFocusChangedEventArgs that contains the event data.</param>
        protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            base.OnGotKeyboardFocus(e);
            //// FIX:Jumping of FloatWindow from screen edges , reason for such behaviour will be investigate later
            //// SetWindowOnTop();


            Rect r = PlacementRectangle;
            if (!r.IsEmpty)
            {
                r.X = r.X - 1;
                PlacementRectangle = r;
                r.X = r.X + 1;
                PlacementRectangle = r;
            }
            Temp++;
            DockedElementTabbedHost host = e.OriginalSource as DockedElementTabbedHost;

            if (null != host)
            {
                if (this.PrimaryElement!=null && this.PrimaryElement is FrameworkElement)
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

        /// <summary>
        /// Raises the GotFocus event.
        /// </summary>
        /// <param name="e">An RoutedEventArgs that contains the event data.</param>
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
            SetWindowOnTop();
        }

        /// <summary>
        /// Invoked when an unhandled GotMouseCapture�attached event reaches an element in its route that is derived from this class.
        /// </summary>
        /// <param name="e">The MouseEventArgs that contains the event data.</param>
        protected override void OnGotMouseCapture(MouseEventArgs e)
        {
            if (e.Source is DockedElementTabbedHost)
            {
                InternalDataContext = e.Source as FrameworkElement;
            }

            base.OnGotMouseCapture(e);
        }

        /// <summary>
        /// Raises the System.Windows.FrameworkElement.Initialized event. This method 
        /// is invoked whenever DockingManager.IsInitialized is set to true internally.
        /// </summary>
        /// <param name="e">The System.Windows.RoutedEventArgs that contains the event data.</param>
        protected override void OnInitialized(EventArgs e)
        {
            if (null != DockingManager)
            {
                BindingUtils.SetBinding(Child, DockingManager, FrameworkElement.StyleProperty, DockingManager.FloatWindowStyleProperty);
            }
            else
            {
                throw new OperationCanceledException("Some over patching was adds. Need will be added new logic for this case.");
            }

            base.OnInitialized(e);
        }

        /// <summary>
        /// Sets the window on top secure.
        /// </summary>
        private void SetWindowOnTopSecure()
        {
            if (Child != null)
            {
                HwndSource src = PresentationSource.FromVisual(Child) as HwndSource;
                ////Rect r = PlacementRectangle;
                ////int x = (int)r.TopLeft.X;
                ////int y = (int)r.TopLeft.Y;
                ////int cx = (int)r.BottomRight.X;
                ////int cy = (int)r.BottomRight.Y;

                if (src != null)
                {
                    IntPtr hwnd = src.Handle;
                    ////FIX:Jumping of FloatWindow from screen edges , reason for such behaviour will be investigate later
                    //if (Temp > 2)
                    //{
                    //    NativeMethods.SetWindowPos(hwnd, NativeConstants.HWND_TOP, 0, 0, 0, 0, NativeConstants.SWP_NOSIZE | NativeConstants.SWP_NOMOVE | NativeConstants.SWP_NOACTIVATE);
                    //}
                }
            }
        }

        /// <summary>
        /// Called when [opened secure].
        /// </summary>
        private void OnOpenedSecure()
        {
            if (Child != null)
            {
                HwndSource source = (HwndSource)PresentationSource.FromVisual(Child);

                if (source != null)
                {
                    IntPtr hwnd = source.Handle;
                    NativeMethods.SetWindowPos(hwnd, NativeConstants.HWND_NOTOPMOST, 0, 0, 0, 0, NativeConstants.SWP_NOSIZE | NativeConstants.SWP_NOMOVE | NativeConstants.SWP_NOACTIVATE);
                }
            }
        }

        /// <summary>
        /// Updates width and height of the window using the size from the PlacementRectangle.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private static void OnPlacementRectangleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FloatWindow window = d as FloatWindow;
            window.OnPlacementRectangleChanged(e);
        }

        private void OnPlacementRectangleChanged(DependencyPropertyChangedEventArgs e)
        {
            if (!PlacementRectangle.IsEmpty)
            {
                Width = PlacementRectangle.Width;
                Height = PlacementRectangle.Height;
            }
            else
            {
                Width = 150;
                Height = 100;
            }
        }

        /// <summary>
        /// Hooks the method.
        /// </summary>
        /// <param name="hwnd">The handler WND.</param>
        /// <param name="msg">The Window MSG.</param>
        /// <param name="wParam">The window param.</param>
        /// <param name="lParam">The lntptr param.</param>
        /// <param name="handled">if set to <c>true</c> [handled].</param>
        /// <returns>return Intptr value.</returns>
        private static IntPtr HookMethod(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case NativeConstants.WM_NCHITTEST:
                    handled = true;
                    return new IntPtr(NativeConstants.HTTRANSPARENT);
            }

            return new IntPtr(0);
        }

        /// <summary>
        /// Coerces the is open callback.
        /// </summary>
        /// <param name="d">The dependency object.</param>
        /// <param name="baseValue">The base value.</param>
        /// <returns>return object.</returns>
        private static object CoerceIsOpenCallback(DependencyObject d, object baseValue)
        {
            FloatWindow window = (FloatWindow)d;
            bool result = (bool)baseValue;
            UIElement placementTarget = window.PlacementTarget;

            if (result && null != placementTarget && placementTarget.IsVisible)
            {
                Window windowTarget = Window.GetWindow(placementTarget);

                if (windowTarget != null)
                {
                    result &= windowTarget.IsActive & windowTarget.IsLoaded;

                    if (PermissionHelper.HasUnmanagedCodePermission && !BrowserInteropHelper.IsBrowserHosted)
                    {
                        WindowInteropHelper helper = new WindowInteropHelper(windowTarget);
                        bool isActive = ConnectedToForegroundWindow(helper.Handle);

                        result &= isActive;
                    }
                }
                else if (placementTarget != null)
                {

                    result = placementTarget.IsVisible;
                }
                else
                {
                    result = false;
                }
            }
            else
            {
                result = false;
            }

            return result;
        }

        /// <summary>
        /// Connected to foreground window.
        /// </summary>
        /// <param name="window">The window.</param>
        /// <returns>return bool value.</returns>
        [SecurityCritical]
        private static bool ConnectedToForegroundWindow(IntPtr window)
        {
            IntPtr foregroundWindow = GetForegroundWindow();
            return window == foregroundWindow;
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
        #endregion

        #region Dependendency properties
        /// <summary>
        /// Identifies PrimaryElement dependency property of the FloatWindow.
        /// </summary>
        public static readonly DependencyProperty PrimaryElementProperty
            = DependencyProperty.Register("PrimaryElement", typeof(FrameworkElement), typeof(FloatWindow));

        /// <summary>
        /// Identifies IsDragging dependency property of the FloatWindow.
        /// </summary>
        public static readonly DependencyProperty IsDraggingProperty =
            DependencyProperty.Register("IsDragging", typeof(bool), typeof(FloatWindow), new UIPropertyMetadata(false));

        /// <summary>
        /// Identifies IsMultiHostsContainer dependency property of the FloatWindow.
        /// </summary>
        internal static readonly DependencyProperty IsMultiHostsContainerProperty =
            DependencyProperty.Register("IsMultiHostsContainer", typeof(bool), typeof(FloatWindow), new UIPropertyMetadata(false));

        /// <summary>
        /// Identifies DockingManager dependency property of the FloatWindow.
        /// </summary>
        public static readonly DependencyProperty DockingManagerProperty =
            DependencyProperty.Register("DockingManager", typeof(DockingManager), typeof(FloatWindow));
        #endregion

        #region Alternative methods
        /// <summary>
        /// Updates the is multi host property.
        /// </summary>
        public void UpdateIsMultiHostProperty()
        {
            DockedElementsContainer container = (Child as AutoTemplatedContentControl).Content as DockedElementsContainer;
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

        /// <summary>
        /// Gets the visible hosts count.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <returns>return count.</returns>
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
        #endregion
    }
}
