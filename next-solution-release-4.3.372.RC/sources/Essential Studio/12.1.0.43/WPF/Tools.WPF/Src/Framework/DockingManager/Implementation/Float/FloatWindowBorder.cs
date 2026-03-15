// <copyright file="FloatWindowBorder.cs" company="Syncfusion">
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
using System.Windows.Input;
using System.Windows.Media;
using Syncfusion.Windows.Shared;
using System.Windows.Data;
using System.Diagnostics;
using System.Windows.Controls.Primitives;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents border of the float window.
    /// </summary>
    /// <remarks>
    /// FloatWindowBorder class is useful in XAML, where you can override its template.
    /// FloatWindowBorder templates must differ from different sides of the float window.
    /// For example when you override FloatWindowBorder templates you have to use <see cref="BorderMode"/> property to define 
    /// the side of the border and then define different templates for each border mode. Border mode can be Header, LeftTop,
    /// RightTop, Left, Right, LeftBottom, Bottom or RightBottom. Every border mode has its own template.
    /// </remarks>
    /// <example>
    /// <para/>This example shows how to initialize the style of the FloatWindowBorder class in XAML.
    /// <code language="XAML">
    /// <![CDATA[
    /// <Style x:Key="{x:Type Syncfusion:FloatWindowBorder}" TargetType="{x:Type Syncfusion:FloatWindowBorder}">
    /// <Style.Triggers>
    /// <Trigger Property="BorderMode" Value="Header">
    /// <Setter Property="Template" Value="{StaticResource FloatWindowBorderHeaderTemplate}" />
    /// </Trigger>
    /// <Trigger Property="BorderMode" Value="LeftTop" >
    /// <Setter Property="Template" Value="{StaticResource FloatWindowBorderLeftTopTemplate}" />
    /// </Trigger>
    /// <Trigger Property="BorderMode" Value="RightTop" >
    /// <Setter Property="Template" Value="{StaticResource FloatWindowBorderRightTopTemplate}" />
    /// </Trigger>
    /// </Style.Triggers>
    /// </Style>
    /// ]]>
    /// </code>
    /// <para/>This example shows how to initialize the template of the FloatWindowBorder header in XAML.
    /// <code language="XAML">
    /// <![CDATA[
    /// <ControlTemplate x:Key="FloatWindowBorderHeaderTemplate" TargetType="{x:Type Syncfusion:FloatWindowBorder}">
    /// <Border Name="borderTop"  BorderBrush="#FFDEDEDE" >
    /// <Border Name="borderWrap" BorderBrush="#FF595959" >
    /// <Border.ContextMenu>
    /// <Syncfusion:CustomContextMenu Name="PART_ContextMenu" Focusable="false" />
    /// </Border.ContextMenu>
    /// <DockPanel Name="MiddleHdrImg" LastChildFill="True">
    /// <Button Name="button" Style="{StaticResource FloatWindowCloseButtonStyle}" 
    /// Margin="2,1,0,0" Padding="0" DockPanel.Dock="Right">
    /// <Button.ToolTip>
    /// <ToolTip Name="tooltip" >
    /// <TextBlock Name="tooltipText" Text="Close" />
    /// </ToolTip>
    /// </Button.ToolTip>
    /// </Button>
    /// <ContentPresenter Name="contentWraper" VerticalAlignment="Center" IsHitTestVisible="false"
    /// Content="{Binding Path=(FrameworkElement.DataContext).(FrameworkElement.DataContext)
    /// .(Syncfusion:DockingManager.Header), RelativeSource={RelativeSource FindAncestor
    /// , AncestorType={x:Type Syncfusion:IWindow}}}"
    /// ContentTemplateSelector="{StaticResource FloatTrimmingTemplate}"
    /// ContentTemplate="{Binding Path=(FrameworkElement.DataContext).(FrameworkElement.DataContext)
    /// .(Syncfusion:DockingManager.HeaderTemplate), RelativeSource={RelativeSource FindAncestor
    /// , AncestorType={x:Type Syncfusion:IWindow}}}"
    /// />
    /// </DockPanel>
    /// </Border>
    /// </Border>
    /// </ControlTemplate>
    /// ]]>
    /// </code>
    /// </example>
    /// <exclude/>
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
    [SkinType(SkinVisualStyle = Skin.Transparent,
    Type = typeof(FloatWindowBorder), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/DockingManager/Themes/TransparentStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2013,
    Type = typeof(FloatWindowBorder), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/DockingManager/Themes/Office2013Style.xaml")]  
    public class FloatWindowBorder : Control
    {
        #region Constants

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

        #endregion

        #region Commands
        /// <summary>
        /// Identifies ChangeState command of the FloatWindow.
        /// </summary>
        public static RoutedUICommand ChangeStateCommand = new RoutedUICommand("ChangeState", "ChangeState", typeof(FloatWindow));
        #endregion

        #region Private members
        /// <summary>
        /// Specifies isPressed.
        /// </summary>
        private bool m_isPressed;

        /// <summary>
        /// Specifies isResizing.
        /// </summary>
        private bool m_isResizing;

        /// <summary>
        /// Specifies isPreview.
        /// </summary>
        private bool m_isPreview;

        /// <summary>
        /// Specifies pointMouseStartpos.
        /// </summary>
        private Point m_pointMouseStartPos;

        /// <summary>
        /// Specifies contextMenu.
        /// </summary>
        internal ContextMenu m_contextMenu;

        /// <summary>
        /// Contains true if element is dragging and false when element stops dragging.
        /// </summary>
        private bool m_firedDragStart = false;

        /// <summary>
        /// Contains counter for dragging in mouseMove method.
        /// </summary>
        private int m_dragCounter = 0;

        /// <summary>
        /// It has the CloseButton instance of window
        /// </summary>
        internal ToggleButton CloseButton = null;

        /// <summary>
        /// It sets the value when mouse left button event occurs
        /// </summary>
        private bool m_leftbuttondown = false;

        private bool m_TouchDown;

        /// <summary>
        /// Indicates TouchDevice Id.
        /// </summary>
        internal int m_TouchDeviceId = -1;

        #endregion

        #region Initialization
        /// <summary>
        /// Initializes static members of the <see cref="FloatWindowBorder"/> class.
        /// </summary>
        static FloatWindowBorder()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FloatWindowBorder),new FrameworkPropertyMetadata(typeof(FloatWindowBorder)));
            
        }

        public FloatWindowBorder()
        {
            this.Loaded += new RoutedEventHandler(FloatWindowBorder_Loaded);
            this.Unloaded += new RoutedEventHandler(FloatWindowBorder_Unloaded);
        }

        private void FloatWindowBorder_Loaded(object sender, RoutedEventArgs e)
        {
            if (ParentWindow != null && ParentWindow.PrimaryElement != null)
            {
                if (CloseButton != null)
                {
                    DockingManager owner = DockingManager.ResolveManager(ParentWindow.PrimaryElement as UIElement);
                    if (owner != null)
                    {
                        bool close = DockingManager.GetCanClose(ParentWindow.PrimaryElement as DependencyObject);
                        if (!close)
                        {
                            switch (owner.DisabledCloseButtonsBehavior)
                            {
                                case DisabledButtonsBehavior.Collapse:
                                    CloseButton.Visibility = Visibility.Collapsed;
                                    break;
                                case DisabledButtonsBehavior.Hide:
                                    CloseButton.Visibility = Visibility.Hidden;
                                    break;
                                case DisabledButtonsBehavior.Disable:
                                    CloseButton.Visibility = Visibility.Visible;
                                    break;
                            }
                        }
                        else
                        {
                            CloseButton.Visibility = Visibility.Visible;
                        }
                    }
                }
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets BorderMode of the FloatWindowBorder. This is a dependency property.
        /// </summary>
        public FloatWindowBorderMode BorderMode
        {
            get
            {
                return (FloatWindowBorderMode)GetValue(BorderModeProperty);
            }

            set
            {
                SetValue(BorderModeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets ParentWindow of the FloatWindowBorder This is a dependency property.
        /// </summary>
        public IWindow ParentWindow
        {
            get
            {
                return (IWindow)GetValue(ParentWindowProperty);
            }

            set
            {
                SetValue(ParentWindowProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets MinWindowWidth of the <see cref="FloatWindowBorder"/>. This is a dependency property.
        /// </summary>
        public double MinWindowWidth
        {
            get
            {
                return (double)GetValue(MinWindowWidthProperty);
            }

            set
            {
                SetValue(MinWindowWidthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets MinWindowHeight of the FloatWindowBorder. This is a dependency property.
        /// </summary>
        public double MinWindowHeight
        {
            get
            {
                return (double)GetValue(MinWindowHeightProperty);
            }

            set
            {
                SetValue(MinWindowHeightProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether ProcessDoubleClick of the FloatWindowBorder. This is a dependency property.
        /// </summary>
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
        #endregion

        #region Public methods
        /// <summary>
        /// Raises WindowVisibilityChanged event.
        /// </summary>
        internal void FireWindowVisibilityChanged()
        {
            RoutedEventArgs args = new RoutedEventArgs(DockingManager.WindowVisibilityChangedEvent);
            RaiseEvent(args);
        }

        /// <summary>
        /// Finds the orientation.
        /// </summary>
        /// <returns></returns>
        internal Orientation FindOrientation()
        {
            switch (BorderMode)
            {
                case FloatWindowBorderMode.Header:
                case FloatWindowBorderMode.Bottom:
                    return Orientation.Vertical;                    
                case FloatWindowBorderMode.LeftTop:
                case FloatWindowBorderMode.RightBottom:
                case FloatWindowBorderMode.LeftBottom:
                case FloatWindowBorderMode.RightTop:
                case FloatWindowBorderMode.Left:
                case FloatWindowBorderMode.Right:
                    return Orientation.Horizontal;                   
            }
            return Orientation.Horizontal;
        }

        /// <summary>
        /// Raises DockMenuClick event.
        /// </summary>
        internal void FireContextMenuItemClick()
        {
            RoutedEventArgs args = new RoutedEventArgs(DockingManager.ContextMenuItemClickEvent);
            RaiseEvent(args);
        }


        /// <summary>
        /// Called when an internal process or application calls ApplyTemplate, which is used to build the current template's visual tree.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (BorderMode == FloatWindowBorderMode.Header)
            {
                m_contextMenu = GetTemplateChild("PART_ContextMenu") as CustomContextMenu;
                CloseButton = GetTemplateChild("button") as ToggleButton;
                ////if (ParentWindow == null)
                ////{
                ////    ParentWindow = VisualUtils.FindAncestor(this, typeof(IWindow)) as IWindow;
                ////}
                //ParentWindow.Header = this;
                AutoTemplatedContentControl autoTemplatedContent = TemplatedParent as AutoTemplatedContentControl;
                if (autoTemplatedContent != null)
                {
                    DockingManager owner;
                    IWindow floatWnd = (IWindow)autoTemplatedContent.Parent;
 		    		ParentWindow = floatWnd;

                    if (ParentWindow != null)
                    {
                        ParentWindow.Header = this;
                        owner = floatWnd.DockingManager;
                        if (owner != null && floatWnd.PrimaryElement != null && m_contextMenu != null)
                        {
                            if (!(DockingManager.GetDockAbility(floatWnd.PrimaryElement) == DockAbility.All || DockingManager.GetDockAbility(floatWnd.PrimaryElement) == DockAbility.Tabbed))
                            {
                                if (m_contextMenu != null)
                                {
                                    (m_contextMenu as CustomContextMenu).IsEnabledTabbedMenuItem = false;
                                }
                            }
                            if (!DockingManager.GetIsContextMenuVisible(floatWnd.PrimaryElement))
                            {
                                m_contextMenu.Visibility = Visibility.Collapsed;
                            }
                            else
                            {
                                m_contextMenu.Visibility = Visibility.Visible;
                            }
                        }
                        if (ParentWindow!=null && ParentWindow.PrimaryElement != null)
                        {
                            if (m_contextMenu != null)
                            {
                                foreach (CustomMenuItem item in m_contextMenu.Items)
                                {
                                    if (item.Name == "PART_DockableMenuItem")
                                    {
                                        BindingUtils.SetBinding(item, ParentWindow.PrimaryElement, CustomMenuItem.IsEnabledProperty, DockingManager.CanDockProperty, BindingMode.OneWay);
                                    }
                                }
                            }
                            ToEnableorDisableCloseButton(ParentWindow.PrimaryElement);
                        }
                    }
                }
                if (null != m_contextMenu)
                {
                    m_contextMenu.Opened += new RoutedEventHandler(OnContextMenuOpened);
                }
            }
        }

        /// <summary>
        /// Called to remeasure a control.
        /// </summary>
        /// <param name="constraint">The maximum size that the method can return.</param>
        /// <returns>
        /// The size of the control, up to the maximum specified by <paramref name="constraint"/>.
        /// </returns>
        protected override Size MeasureOverride(Size constraint)
        {
            if (ParentWindow != null && ParentWindow.PrimaryElement != null && m_contextMenu != null)
            {
                if (!DockingManager.GetIsContextMenuVisible(ParentWindow.PrimaryElement))
                {
                    m_contextMenu.Visibility = Visibility.Collapsed;
                }
                else
                {
                    m_contextMenu.Visibility = Visibility.Visible;
                }
            }
            return base.MeasureOverride(constraint);
        }

        /// <summary>
        /// Called when state of the float window change.
        /// </summary>
        /// <param name="sender">Object that raise this event.</param>
        /// <param name="e">Provides data for the Executed and PreviewExecuted�routed events.</param>
        public void ExecuteChangeState(object sender, ExecutedRoutedEventArgs e)
        {
            FireWindowVisibilityChanged();
            AutoTemplatedContentControl autoTemplatedContent = TemplatedParent as AutoTemplatedContentControl;

            if (null != autoTemplatedContent)
            {
                IWindow floatWnd = (IWindow)autoTemplatedContent.Parent;
                DockingManager owner = floatWnd.DockingManager;

                if (ProcessDoubleClick)
                {
                    WindowClosingEventArgs close = new WindowClosingEventArgs();
                    if (close != null)
                    {
                        if (ParentWindow != null && ParentWindow.InternalDataContext != null
                               && (ParentWindow.InternalDataContext as DockedElementTabbedHost) != null)
                        {                            
                            close.TargetItem = (ParentWindow.InternalDataContext as DockedElementTabbedHost).InternalDataContext as FrameworkElement;
                        }

                        else if (floatWnd.PrimaryElement != null)
                        {
                            close.TargetItem = floatWnd.PrimaryElement;
                        }
                        owner.FireWindowClosingEvent(owner, close);
                        if (!close.Cancel)
                        {
                            if (sender == null)
                            {
                                DockedElementTabbedHost host = ParentWindow.InternalDataContext as DockedElementTabbedHost;
                                if (host != null)
                                {
                                    FrameworkElement element = host.InternalDataContext as FrameworkElement;
                                    if (element != null)
                                    {
                                        owner.ExecuteClose(element);
                                        owner.SetFocus(element);
                                    }
                                }
                            }
                            else
                            {
                                owner.ExecuteClose(ParentWindow);

                            }
                        }
                    }
                }
                else
                {
                    if (ParentWindow!=null && ParentWindow.PrimaryElement != null)
                    {
                        DockingManager.SetNoDock(ParentWindow.PrimaryElement, false);
                    }
                    owner.ExecuteDoubleClick(ParentWindow);
                }
            }
        }
        #endregion

        #region Implementation

        internal void ToEnableorDisableCloseButton(FrameworkElement element)
        {
            if (CloseButton != null && element != null)
            {
                BindingUtils.SetBinding(CloseButton, element, Button.IsEnabledProperty, DockingManager.CanCloseProperty, BindingMode.OneWay);
            }
        }
        /// <summary>
        /// Invoked when an unhandled MouseLeftButtonDown�routed event is raised on this element.
        /// </summary>
        /// <param name="e">The MouseButtonEventArgs that contains the event data. The event data reports that the mouse button was released.</param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            if (ParentWindow != null && ParentWindow.DockingManager != null && e.StylusDevice == null || e.StylusDevice != null)
            {
                m_leftbuttondown = true;
                UIElement element = (UIElement)ParentWindow.InternalDataContext;
                ParentWindow.DockingManager.OnMouseDownOnHeader(this, e);
                ParentWindow.DockingManager.m_pointMouseStartPosOffset = Mouse.GetPosition(this);

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
                    }
                    else
                    {
                        ParentWindow.DockingManager.ActiveWindow = args.NewValue;
                    }
                    element.Focus();
                    #endregion
                }
                else
                {
                    FloatWindow.SetPrimaryElementAsDataContext(ParentWindow);
                }

                m_pointMouseStartPos = Mouse.GetPosition(this);
                if (ParentWindow.DockingManager.FlowDirection == FlowDirection.RightToLeft && BorderMode != FloatWindowBorderMode.Header)
                    m_pointMouseStartPos.X = -m_pointMouseStartPos.X;
                bool isHeader = BorderMode == FloatWindowBorderMode.Header;
                bool isMouseInHeaderResizeArea = isHeader && (m_pointMouseStartPos.Y > 3);
                m_isPressed = isMouseInHeaderResizeArea;
                if (ParentWindow != null && ParentWindow.PrimaryElement != null)
                {
                    if ((ParentWindow.PrimaryElement as DependencyObject) != null)
                    {
                        if (DockingManager.GetPrevChild(ParentWindow.PrimaryElement) == null)
                        {
                            m_isResizing = !isMouseInHeaderResizeArea;
                        }
                    }
                }
                else
                {
                    m_isResizing = !isMouseInHeaderResizeArea;
                }

                CaptureMouse();
                m_leftbuttondown = false;
            }
        }

        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            if (ParentWindow != null)
            {
                UIElement element = (UIElement)ParentWindow.InternalDataContext;
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
                    }
                    else
                    {
                        ParentWindow.DockingManager.ActiveWindow = args.NewValue;
                    }
                    element.Focus();
                    #endregion
                }
            }
            base.OnPreviewMouseDown(e);
        }

        /// <summary>
        /// Invoked when an unhandled LostMouseCapture�attached event reaches an element in its route that is derived from this class.
        /// </summary>
        /// <param name="e">The MouseEventArgs that contains the event data.</param>
        protected override void OnLostMouseCapture(MouseEventArgs e)
        {
            if (m_isPressed && m_dragCounter > 2 && ParentWindow.IsDragging)
            {
                CompleteDragging();
            }
            else if (m_isResizing)
            {
                CompleteResizing();
            }

            base.OnLostMouseCapture(e);
        }

        /// <summary>
        /// Invoked when an unhandled MouseLeftButtonUp�routed event is raised on this element.
        /// </summary>
        /// <param name="e">The MouseButtonEventArgs that contains the event data. The event data reports that the mouse button was released.</param>
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            if (ParentWindow != null && ParentWindow.DockingManager != null && e.StylusDevice == null || e.StylusDevice != null)
            {
                ParentWindow.DockingManager.OnMouseUpOnHeader(this, e);
                ParentWindow.DockingManager.m_pointMouseStartPosOffset = new Point(double.NegativeInfinity, double.NegativeInfinity);

                if (m_isPressed)
                {
                    CompleteDragging();
                }
                else if (m_isResizing)
                {
                    CompleteResizing();
                }
                else if (m_isPreview)
                {
                    //ParentWindow.DockingManager.DockToFloatWindow(ParentWindow);
                    m_isPreview = false;
                }

                ReleaseMouseCapture();
            }
        }

        /// <summary>
        /// Invoked when an unhandled attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">contains the event data.</param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (ParentWindow != null && ParentWindow.DockingManager != null && e.StylusDevice == null)
            {
                Point currentPos = Mouse.GetPosition(this);
                if (ParentWindow.DockingManager.FlowDirection == FlowDirection.RightToLeft && BorderMode != FloatWindowBorderMode.Header)
                    currentPos.X = -currentPos.X;
                if (ParentWindow != null)
                {
                    if (ParentWindow.PrimaryElement != null && (ParentWindow.PrimaryElement as DependencyObject) != null)
                    {
                        if (DockingManager.GetPrevChild(ParentWindow.PrimaryElement) == null && DockingManager.CheckResize(ParentWindow.PrimaryElement, DockState.Float, FindOrientation()))
                        {
                            UpdateCursor(currentPos);
                        }
                    }
                }
                else
                {
                    UpdateCursor(currentPos);
                }
                if (m_isPressed || m_isResizing && ParentWindow != null && ParentWindow.PrimaryElement != null && (ParentWindow.PrimaryElement as DependencyObject) != null && DockingManager.CheckResize(ParentWindow.PrimaryElement, DockState.Float, FindOrientation()))
                {
                    if (ParentWindow != null && ParentWindow.PrimaryElement != null)
                    {
                        DockInfoInternal info = DockingManager.GetDockInfo(ParentWindow.PrimaryElement as DependencyObject);
                        bool bDraggingActivated = m_dragCounter > 0;

                        if (info != null)
                        {
                            if (bDraggingActivated)
                            {
                                if (!m_firedDragStart && !info.DockingManager.m_firedDragStart)
                                {
                                    info.DockingManager.m_firedDragStart = m_firedDragStart = true;
                                    ParentWindow.Header = this;
                                    if (ParentWindow != null && ParentWindow.PrimaryElement != null)
                                    {
                                        info.DockingManager.FireWindowDragStart(ParentWindow.PrimaryElement);
                                    }
                                    info.DockingManager.StartDraggingWindow(ParentWindow);
                                }
                            }
                        }
                        ////info.DockingManager.HostUnderMouse = ParentWindow.PrimaryElement as DockedElementTabbedHost;


                        if (m_isPressed && bDraggingActivated || m_isResizing)
                        {
                            Rect rectWindow = GetPlacementRectangle(ParentWindow);
                            Rect rectOriginal = rectWindow;
                            DockingManager docking = ParentWindow.DockingManager;
                            if (docking != null )
                            {
                                if (docking.FlowDirection == FlowDirection.LeftToRight)
                                {
                                    switch (BorderMode)
                                    {
                                        case FloatWindowBorderMode.Header:
                                            SetRectWindowForHeader(ref rectWindow, currentPos);
                                            break;
                                        case FloatWindowBorderMode.LeftTop:
                                            SetRectWindowWidthHeightXY(ref rectWindow, currentPos);
                                            break;
                                        case FloatWindowBorderMode.RightTop:
                                            SetRectWindowWidthHeightY(ref rectWindow, currentPos);
                                            break;
                                        case FloatWindowBorderMode.Left:
                                            SetRectWindowWidthX(ref rectWindow, currentPos);
                                            break;
                                        case FloatWindowBorderMode.Right:
                                            SetRectWindowWidth(ref rectWindow, currentPos);
                                            break;
                                        case FloatWindowBorderMode.LeftBottom:
                                            SetRectWindowWidthHeightX(ref rectWindow, currentPos);
                                            break;
                                        case FloatWindowBorderMode.RightBottom:
                                            SetRectWindowWidthHeight(ref rectWindow, currentPos);
                                            break;
                                        case FloatWindowBorderMode.Bottom:
                                            SetRectWindowHeight(ref rectWindow, currentPos);
                                            break;
                                        default:
                                            throw new InvalidOperationException("Unsupported BorderMode.");
                                    }
                                }
                                else 
                                {
                                    switch (BorderMode)
                                    {
                                        case FloatWindowBorderMode.Header:
                                            SetRectWindowForHeader(ref rectWindow, currentPos);
                                            break;
                                        case FloatWindowBorderMode.RightTop:
                                            SetRectWindowWidthHeightXY(ref rectWindow, currentPos);
                                            break;
                                        case FloatWindowBorderMode.LeftTop:
                                            SetRectWindowWidthHeightY(ref rectWindow, currentPos);
                                            break;
                                        case FloatWindowBorderMode.Right:
                                            SetRectWindowWidthX(ref rectWindow, currentPos);
                                            break;
                                        case FloatWindowBorderMode.Left:
                                            SetRectWindowWidth(ref rectWindow, currentPos);
                                            break;
                                        case FloatWindowBorderMode.RightBottom:
                                            SetRectWindowWidthHeightX(ref rectWindow, currentPos);
                                            break;
                                        case FloatWindowBorderMode.LeftBottom:
                                            SetRectWindowWidthHeight(ref rectWindow, currentPos);
                                            break;
                                        case FloatWindowBorderMode.Bottom:
                                            SetRectWindowHeight(ref rectWindow, currentPos);
                                            break;
                                        default:
                                            throw new InvalidOperationException("Unsupported BorderMode.");
                                    }
                                }
                            }
                           
                            WindowResizingEventArgs args = new WindowResizingEventArgs();
                            if (m_isResizing && !m_leftbuttondown)
                            {
                                args.DesiredHeight = rectWindow.Height;
                                args.DesiredWidth = rectWindow.Width;
                                args.State = DockState.Float;
                                docking.FireWindowResizingEvent(ParentWindow as FloatWindow, args);
                            }
                            if(rectOriginal!=rectWindow)
                            SetParentWindowPlacementRectangle(rectWindow, rectOriginal);
                        }

                        if (m_isPressed && !m_firedDragStart)
                        {
                            m_dragCounter++;
                            if (info.DockingManager.m_dragCounter < m_dragCounter)
                                info.DockingManager.m_dragCounter = m_dragCounter;
                        }
                    }
                }
                else
                {
                    if (ParentWindow == null)
                    {
                        ParentWindow = VisualUtils.FindAncestor(this, typeof(IWindow)) as IWindow;
                    }

                    if (ParentWindow != null && ParentWindow.PrimaryElement != null)
                    {
                        DockingManager owner = ParentWindow.DockingManager;
                        m_isPreview = !ParentWindow.IsMultiHostsContainer && owner.IsDragging
                            && BorderMode == FloatWindowBorderMode.Header &&
                            !DockingManager.GetNoDock(ParentWindow.PrimaryElement);

                        if (m_isPreview)
                        {
                            m_isPreview = owner.ShowDockPreviewInternal(ParentWindow);
                        }
                    }
                }
                if (m_firedDragStart)
                {
                    if (ParentWindow != null && ParentWindow.PrimaryElement != null && (ParentWindow.PrimaryElement as DependencyObject) != null)
                    {
                        DockInfoInternal info = DockingManager.GetDockInfo(ParentWindow.PrimaryElement as DependencyObject);

                        if (info != null)
                        {
                            WindowMovingEventArgs args = new WindowMovingEventArgs();
                            args.State = DockState.Float;
                            if (ParentWindow.DockingManager != null)
                            {
                                if (ParentWindow.PrimaryElement.IsVisible)
                                {
                                    args.X = ParentWindow.PrimaryElement.PointToScreen(new Point(0, 0)).X;
                                    args.Y = ParentWindow.PrimaryElement.PointToScreen(new Point(0, 0)).Y;
                                }
                            }
                            info.DockingManager.FireWindowMoving(ParentWindow.PrimaryElement, args);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Invoked when an unhandled MouseDoubleClick�routed event is raised on this element.
        /// </summary>
        /// <param name="e">The MouseButtonEventArgs that contains the event data. The event data reports that the mouse button was released.</param>
        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            if (ParentWindow != null && ParentWindow.DockingManager != null && e.StylusDevice==null)
            {
                FrameworkElement originalSource = (FrameworkElement)e.OriginalSource;

                if (originalSource.TemplatedParent == null || typeof(Button) != originalSource.TemplatedParent.GetType())
                {
                    HitTestResult hitTest = VisualTreeHelper.HitTest(this, e.GetPosition(this));

                    if (null != hitTest)
                    {
                        FrameworkElement visualHit = (FrameworkElement)hitTest.VisualHit;

                        if (MouseButtonState.Pressed == e.LeftButton && FloatWindowBorderMode.Header == BorderMode
                            && ProcessDoubleClick && !(visualHit.TemplatedParent is Button) && ParentWindow != null && ParentWindow.PrimaryElement != null)
                        {
                            DockingManager owner = ParentWindow.DockingManager;

                            List<FrameworkElement> tabs = owner.GetContainerTabs(ParentWindow.FloatChild as DockedElementsContainer);
                            bool bNoDock = DockingManager.GetNoDock(ParentWindow.PrimaryElement);
                            bool canDock = true;

                            foreach (FrameworkElement tab in tabs)
                            {
                                DockStateChangingEventArgs args = new DockStateChangingEventArgs();
                                args.PresentState = DockState.Float;
                                DockSide Side = DockingManager.GetSideInDockedMode(tab);
                                args.TargetElement = tab;
                                args.TargetSide = Side;
                                if (!DockingManager.GetCanDock(tab))
                                {
                                    canDock = false;
                                    break;
                                }
                                if (!bNoDock && canDock)
                                {
                                    owner.FireDockStateChanging(tab, args);
                                    if (args.Cancel)
                                    {
                                        canDock = false;
                                        break;
                                    }
                                    else
                                    {
                                        owner.m_IsStateChangingChecked = true;
                                    }
                                }
                            }

                            if (!bNoDock && canDock)
                            {
                                owner.ExecuteDoubleClick(ParentWindow);
                                DockingManager.SetNewFocusedElement(tabs[0]);
                                foreach (FrameworkElement tab in tabs)
                                {
                                    string targetName = DockingManager.GetTargetNameInDockedMode(tab);

                                    if (!string.IsNullOrEmpty(targetName))
                                    {
                                        FrameworkElement target = owner.FindChild(targetName);
                                        if (target != null)
                                        {
                                            DockState state = DockingManager.GetState(target);

                                            if (DockState.Dock != state)
                                            {
                                                DockingManager.RestoreElement(tab, DockState.Dock, owner);
                                                DockSide side = DockingManager.GetSideInDockedMode(target);
                                                if (state == DockState.AutoHidden)
                                                {
                                                    DockingManager.updatedockflag = false;
                                                    DockingManager.SetSideInDockedMode(target, side);
                                                }
                                            }
                                        }

                                    }
                                }
                            }
                            else if (ParentWindow != null && ParentWindow.PrimaryElement != null && DockingManager.GetIsRollupFloatWindow(ParentWindow.PrimaryElement))
                            {
                                owner.FlipFloatWindow(ParentWindow as FloatWindow);
                            }

                        }

                        e.Handled = true;
                    }
                }
            }
            base.OnMouseDoubleClick(e);
        }

        private SystemGesture m_floatSystemGesture;
        protected override void OnStylusSystemGesture(StylusSystemGestureEventArgs e)
        {
            m_floatSystemGesture = e.SystemGesture;
            base.OnStylusSystemGesture(e);
        }

#if !SyncfusionFramework3_5
        private void OnTouchDoubleClick(TouchEventArgs e)
        {
            FrameworkElement originalSource = (FrameworkElement)e.OriginalSource;

            if (originalSource.TemplatedParent == null || typeof(Button) != originalSource.TemplatedParent.GetType())
            {
                HitTestResult hitTest = VisualTreeHelper.HitTest(this, e.GetTouchPoint(this).Position);

                if (null != hitTest)
                {
                    FrameworkElement visualHit = (FrameworkElement)hitTest.VisualHit;

                    if (m_TouchDown && FloatWindowBorderMode.Header == BorderMode
                        && ProcessDoubleClick && !(visualHit.TemplatedParent is Button) && ParentWindow != null && ParentWindow.PrimaryElement != null)
                    {
                        DockingManager owner = ParentWindow.DockingManager;

                        List<FrameworkElement> tabs = owner.GetContainerTabs(ParentWindow.FloatChild as DockedElementsContainer);
                        bool bNoDock = DockingManager.GetNoDock(ParentWindow.PrimaryElement);
                        bool canDock = true;

                        foreach (FrameworkElement tab in tabs)
                        {
                            DockStateChangingEventArgs args = new DockStateChangingEventArgs();
                            args.PresentState = DockState.Float;
                            DockSide Side = DockingManager.GetSideInDockedMode(tab);
                            args.TargetElement = tab;
                            args.TargetSide = Side;
                            if (!DockingManager.GetCanDock(tab))
                            {
                                canDock = false;
                                break;
                            }
                            if (!bNoDock && canDock)
                            {
                                owner.FireDockStateChanging(tab, args);
                                if (args.Cancel)
                                {
                                    canDock = false;
                                    break;
                                }
                                else
                                {
                                    owner.m_IsStateChangingChecked = true;
                                }
                            }
                        }

                        if (!bNoDock && canDock)
                        {
                            owner.ExecuteDoubleClick(ParentWindow);
                            DockingManager.SetNewFocusedElement(tabs[0]);
                            foreach (FrameworkElement tab in tabs)
                            {
                                string targetName = DockingManager.GetTargetNameInDockedMode(tab);

                                if (!string.IsNullOrEmpty(targetName))
                                {
                                    FrameworkElement target = owner.FindChild(targetName);
                                    if (target != null)
                                    {
                                        DockState state = DockingManager.GetState(target);

                                        if (DockState.Dock != state)
                                        {
                                            DockingManager.RestoreElement(tab, DockState.Dock, owner);
                                            DockSide side = DockingManager.GetSideInDockedMode(target);
                                            if (state == DockState.AutoHidden)
                                            {
                                                DockingManager.updatedockflag = false;
                                                DockingManager.SetSideInDockedMode(target, side);
                                            }
                                        }
                                    }

                                }
                            }
                        }
                        else if (ParentWindow != null && ParentWindow.PrimaryElement != null && DockingManager.GetIsRollupFloatWindow(ParentWindow.PrimaryElement))
                        {
                            owner.FlipFloatWindow(ParentWindow as FloatWindow);
                        }

                    }

                    e.Handled = true;
                }
            }
        }

        private void OnTouchLeftFingerDown(TouchEventArgs e)
        {
            if (ParentWindow != null && ParentWindow.DockingManager != null)
            {
                m_leftbuttondown = true;
                UIElement element = (UIElement)ParentWindow.InternalDataContext;
                ParentWindow.DockingManager.OnMouseDownOnHeader(this, e);
                ParentWindow.DockingManager.m_pointMouseStartPosOffset = Mouse.GetPosition(this);

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
                    }
                    else
                    {
                        ParentWindow.DockingManager.ActiveWindow = args.NewValue;
                    }
                    element.Focus();
                    #endregion
                }
                else
                {
                    FloatWindow.SetPrimaryElementAsDataContext(ParentWindow);
                }

                m_pointMouseStartPos = Mouse.GetPosition(this);
                if (ParentWindow.DockingManager.FlowDirection == FlowDirection.RightToLeft && BorderMode != FloatWindowBorderMode.Header)
                    m_pointMouseStartPos.X = -m_pointMouseStartPos.X;
                bool isHeader = BorderMode == FloatWindowBorderMode.Header;
                bool isMouseInHeaderResizeArea = isHeader && (m_pointMouseStartPos.Y > 3);
                m_isPressed = isMouseInHeaderResizeArea;
                if (ParentWindow != null && ParentWindow.PrimaryElement != null)
                {
                    if ((ParentWindow.PrimaryElement as DependencyObject) != null)
                    {
                        if (DockingManager.GetPrevChild(ParentWindow.PrimaryElement) == null)
                        {
                            m_isResizing = !isMouseInHeaderResizeArea;
                        }
                    }
                }
                else
                {
                    m_isResizing = !isMouseInHeaderResizeArea;
                }

                CaptureTouch(e.TouchDevice);
                m_leftbuttondown = false;
            }
        }

        private void OnTouchLeftFingerUp(TouchEventArgs e)
        {
            if (ParentWindow != null && ParentWindow.DockingManager != null)
            {
                ParentWindow.DockingManager.OnMouseUpOnHeader(this, e);
                ParentWindow.DockingManager.m_pointMouseStartPosOffset = new Point(double.NegativeInfinity, double.NegativeInfinity);

                if (m_isPressed)
                {
                    CompleteDragging();
                }
                else if (m_isResizing)
                {
                    CompleteResizing();
                }
                else if (m_isPreview)
                {
                    //ParentWindow.DockingManager.DockToFloatWindow(ParentWindow);
                    m_isPreview = false;
                }

                ReleaseTouchCapture(e.TouchDevice);
            }
        }

        protected override void OnTouchEnter(TouchEventArgs e)
        {
            m_TouchDeviceId = (m_TouchDeviceId == -1) ? e.TouchDevice.Id : m_TouchDeviceId;
            if (ParentWindow != null && ParentWindow.DockingManager != null && ParentWindow.DockingManager.IsTouchEnabled && m_TouchDeviceId == e.TouchDevice.Id)
            {
                ParentWindow.DockingManager.OnMouseEnterOnHeader(this, e);
                base.OnTouchEnter(e);
            }
        }

        private bool m_isCloseButtonClicked;
        protected override void OnTouchDown(TouchEventArgs e)
        {
            if (CloseButton != null)
            {
                Point point = e.GetTouchPoint(this).Position;
                if (point.X + CloseButton.ActualWidth > this.ActualWidth)
                {
                    m_isCloseButtonClicked = true;
                }
            }
            if (!m_isCloseButtonClicked)
            {
                if (ParentWindow != null && ParentWindow.DockingManager != null && ParentWindow.DockingManager.IsTouchEnabled && m_TouchDeviceId == e.TouchDevice.Id)
                {
                    m_TouchDown = true;
                    isDragStart = false;
                    base.OnTouchDown(e);
                    OnTouchLeftFingerDown(e);
                }
            }
        }

        protected override void OnTouchMove(TouchEventArgs e)
        {
            if (!m_isCloseButtonClicked)
            {
                base.OnTouchMove(e);
                if (ParentWindow != null && ParentWindow.DockingManager != null && ParentWindow.DockingManager.IsTouchEnabled && m_TouchDeviceId == e.TouchDevice.Id)
                {
                    if (m_floatSystemGesture == SystemGesture.Tap)
                        OnTouchLeftFingerDown(e);

                    Point currentPos = Mouse.GetPosition(this);
                    if (ParentWindow != null)
                    {
                        if (ParentWindow.PrimaryElement != null && (ParentWindow.PrimaryElement as DependencyObject) != null)
                        {
                            if (DockingManager.GetPrevChild(ParentWindow.PrimaryElement) == null && DockingManager.CheckResize(ParentWindow.PrimaryElement, DockState.Float, FindOrientation()))
                            {
                                UpdateCursor(currentPos);
                            }
                        }
                    }
                    else
                    {
                        UpdateCursor(currentPos);
                    }
                    if (m_isPressed || m_isResizing && ParentWindow != null && ParentWindow.PrimaryElement != null && (ParentWindow.PrimaryElement as DependencyObject) != null && DockingManager.CheckResize(ParentWindow.PrimaryElement, DockState.Float, FindOrientation()))
                    {
                        if (ParentWindow != null && ParentWindow.PrimaryElement != null)
                        {
                            DockInfoInternal info = DockingManager.GetDockInfo(ParentWindow.PrimaryElement as DependencyObject);
                            bool bDraggingActivated = m_dragCounter > 0;

                            if (info != null)
                            {
                                if (bDraggingActivated)
                                {
                                    if (!m_firedDragStart && !info.DockingManager.m_firedDragStart)
                                    {
                                        info.DockingManager.m_firedDragStart = m_firedDragStart = true;
                                        ParentWindow.Header = this;
                                        if (ParentWindow != null && ParentWindow.PrimaryElement != null)
                                        {
                                            info.DockingManager.FireWindowDragStart(ParentWindow.PrimaryElement);
                                        }
                                        info.DockingManager.StartDraggingWindow(ParentWindow);
                                    }
                                }
                            }
                            ////info.DockingManager.HostUnderMouse = ParentWindow.PrimaryElement as DockedElementTabbedHost;


                            if (m_isPressed && bDraggingActivated || m_isResizing)
                            {
                                Rect rectWindow = GetPlacementRectangle(ParentWindow);
                                Rect rectOriginal = rectWindow;

                                switch (BorderMode)
                                {
                                    case FloatWindowBorderMode.Header:
                                        SetRectWindowForHeader(ref rectWindow, currentPos);
                                        break;
                                    case FloatWindowBorderMode.LeftTop:
                                        SetRectWindowWidthHeightXY(ref rectWindow, currentPos);
                                        break;
                                    case FloatWindowBorderMode.RightTop:
                                        SetRectWindowWidthHeightY(ref rectWindow, currentPos);
                                        break;
                                    case FloatWindowBorderMode.Left:
                                        SetRectWindowWidthX(ref rectWindow, currentPos);
                                        break;
                                    case FloatWindowBorderMode.Right:
                                        SetRectWindowWidth(ref rectWindow, currentPos);
                                        break;
                                    case FloatWindowBorderMode.LeftBottom:
                                        SetRectWindowWidthHeightX(ref rectWindow, currentPos);
                                        break;
                                    case FloatWindowBorderMode.RightBottom:
                                        SetRectWindowWidthHeight(ref rectWindow, currentPos);
                                        break;
                                    case FloatWindowBorderMode.Bottom:
                                        SetRectWindowHeight(ref rectWindow, currentPos);
                                        break;
                                    default:
                                        throw new InvalidOperationException("Unsupported BorderMode.");
                                }
                                DockingManager docking = ParentWindow.DockingManager;
                                WindowResizingEventArgs args = new WindowResizingEventArgs();
                                if (m_isResizing && !m_leftbuttondown)
                                {
                                    args.DesiredHeight = rectWindow.Height;
                                    args.DesiredWidth = rectWindow.Width;
                                    args.State = DockState.Float;
                                    docking.FireWindowResizingEvent(ParentWindow as FloatWindow, args);
                                }
                                SetParentWindowPlacementRectangle(rectWindow, rectOriginal);
                            }

                            if (m_isPressed && !m_firedDragStart)
                            {
                                m_dragCounter++;
                                if (info.DockingManager.m_dragCounter < m_dragCounter)
                                    info.DockingManager.m_dragCounter = m_dragCounter;
                            }
                        }
                    }
                    else
                    {
                        if (ParentWindow == null)
                        {
                            ParentWindow = VisualUtils.FindAncestor(this, typeof(IWindow)) as IWindow;
                        }

                        if (ParentWindow != null && ParentWindow.PrimaryElement != null)
                        {
                            DockingManager owner = ParentWindow.DockingManager;
                            m_isPreview = !ParentWindow.IsMultiHostsContainer && owner.IsDragging
                                && BorderMode == FloatWindowBorderMode.Header &&
                                !DockingManager.GetNoDock(ParentWindow.PrimaryElement);

                            if (m_isPreview)
                            {
                                m_isPreview = owner.ShowDockPreviewInternal(ParentWindow);
                            }
                        }
                    }
                    if (m_firedDragStart)
                    {
                        if (ParentWindow != null && ParentWindow.PrimaryElement != null && (ParentWindow.PrimaryElement as DependencyObject) != null)
                        {
                            DockInfoInternal info = DockingManager.GetDockInfo(ParentWindow.PrimaryElement as DependencyObject);

                            if (info != null)
                            {
                                WindowMovingEventArgs args = new WindowMovingEventArgs();
                                args.State = DockState.Float;
                                if (ParentWindow.DockingManager != null)
                                {
                                    if (ParentWindow.PrimaryElement.IsVisible)
                                    {
                                        args.X = ParentWindow.PrimaryElement.PointToScreen(new Point(0, 0)).X;
                                        args.Y = ParentWindow.PrimaryElement.PointToScreen(new Point(0, 0)).Y;
                                    }
                                }
                                info.DockingManager.FireWindowMoving(ParentWindow.PrimaryElement, args);
                            }
                        }
                    }
                }
            }
        }

        protected override void OnTouchUp(TouchEventArgs e)
        {
            if (!m_isCloseButtonClicked)
            {
                if (ParentWindow != null && ParentWindow.DockingManager != null && ParentWindow.DockingManager.IsTouchEnabled && m_TouchDeviceId == e.TouchDevice.Id)
                {
                    if (m_floatSystemGesture == SystemGesture.Tap)
                        OnTouchLeftFingerUp(e);
                    else if (m_floatSystemGesture == SystemGesture.TwoFingerTap)
                        OnTouchDoubleClick(e);
                    m_TouchDown = false;
                    m_TouchDeviceId = -1;
                    m_floatSystemGesture = SystemGesture.None;
                }
                base.OnTouchUp(e);
            }
            m_TouchDown = false;
            m_TouchDeviceId = -1;
            m_floatSystemGesture = SystemGesture.None;
            m_isCloseButtonClicked = false;
        }

        protected override void OnTouchLeave(TouchEventArgs e)
        {
            if (ParentWindow != null && ParentWindow.DockingManager != null && ParentWindow.DockingManager.IsTouchEnabled && m_TouchDeviceId == e.TouchDevice.Id)
            {
                m_TouchDeviceId = -1;
                m_floatSystemGesture = SystemGesture.None;
                ParentWindow.DockingManager.OnMouseLeaveOnHeader(this, e);
                base.OnTouchLeave(e);
            }
        }
#endif

        /// <summary>
        /// Raises the Initialized event.
        /// </summary>
        /// <param name="e">The EventArgs that contains the event data.</param>
        protected override void OnInitialized(EventArgs e)
        {
            CommandBinding bindingChangeState = new CommandBinding(ChangeStateCommand, ExecuteChangeState);
            CommandBindings.Add(bindingChangeState);

            

            base.OnInitialized(e);
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            if (ParentWindow != null && ParentWindow.DockingManager != null && e.StylusDevice == null || e.StylusDevice != null)
            {
                ParentWindow.DockingManager.OnMouseEnterOnHeader(this, e);
            }
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            if (ParentWindow != null && ParentWindow.DockingManager != null && e.StylusDevice == null || e.StylusDevice != null)
            {
                ParentWindow.DockingManager.OnMouseLeaveOnHeader(this, e);
            }
            base.OnMouseLeave(e);
        }

        /// <summary>
        /// Handles the Unloaded event of the FloatWindowBorder control.FloatItem
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"> instance containing the event data.</param>
        void FloatWindowBorder_Unloaded(object sender, RoutedEventArgs e)
        {
           
            if (m_contextMenu != null)
            {
                m_contextMenu = null;
            }
        }

        /// <summary>
        /// Gets if width including coordinates is bigger than MinWindowWidth.
        /// </summary>
        /// <param name="rectWindow">floating window</param>
        /// <param name="currentPos">current position of the floating window</param>
        /// <returns>return bool value.</returns>
        private bool RectWidthBiggerMinWidth(Rect rectWindow, Point currentPos)
        {
            if (rectWindow.Width < DockingManager.MinWidhtInFloat)
            {
                rectWindow.Width = DockingManager.MinWidhtInFloat;
            }
            if (DockingManager.MaxWidhtInFloat > 0)
            {
                if (rectWindow.Width > DockingManager.MaxWidhtInFloat)
                {
                    rectWindow.Width = DockingManager.MaxWidhtInFloat;
                }
            }
            return rectWindow.Width + m_pointMouseStartPos.X - currentPos.X >= MinWindowWidth;
        }

        /// <summary>
        /// Gets if height including coordinates is bigger than MinWindowHeight.
        /// </summary>
        /// <param name="rectWindow">floating window</param>
        /// <param name="currentPos">current position of the floating window</param>
        /// <returns>return bool value.</returns>
        private bool RectHeightBiggerMinHeight(Rect rectWindow, Point currentPos)
        {
            if (rectWindow.Height < DockingManager.MinHeightInFloat)
            {
                rectWindow.Height = DockingManager.MinHeightInFloat;
            }
            return rectWindow.Height + m_pointMouseStartPos.Y - currentPos.Y >= MinWindowHeight;
        }

        /// <summary>
        /// Gets if height including coordinates is bigger than MinWindowHeight.
        /// </summary>
        /// <param name="rectWindow">floating window</param>
        /// <param name="currentPos">current position of the floating window</param>
        /// <returns>return bool value.</returns>
        private bool RectHeightAndCurrentMoreMin(Rect rectWindow, Point currentPos)
        {
            if (rectWindow.Height < DockingManager.MinHeightInFloat)
            {
                rectWindow.Height = DockingManager.MinHeightInFloat;
            }
            if (DockingManager.MaxHeightInFloat > 0)
            {
                if (rectWindow.Height > DockingManager.MaxHeightInFloat)
                {
                    rectWindow.Height = DockingManager.MaxHeightInFloat;
                }
            }
            return rectWindow.Height + currentPos.Y - m_pointMouseStartPos.Y >= MinWindowHeight;
        }

        /// <summary>
        /// Gets if width including coordinates is bigger than MinWindowWidth.
        /// </summary>
        /// <param name="rectWindow">floating window</param>
        /// <param name="currentPos">current position of the floating window</param>
        /// <returns>return bool value.</returns>
        private bool RectWidthAndCurrentMoreMin(Rect rectWindow, Point currentPos)
        {
            if (rectWindow.Width < DockingManager.MinWidhtInFloat)
            {
                rectWindow.Width = DockingManager.MinWidhtInFloat;
            }
            if (DockingManager.MaxWidhtInFloat > 0)
            {
                if (rectWindow.Width > DockingManager.MaxWidhtInFloat)
                {
                    rectWindow.Width = DockingManager.MaxWidhtInFloat;
                }
            }
            return rectWindow.Width + currentPos.X - m_pointMouseStartPos.X >= MinWindowWidth; 
            
        }
        
        /// <summary>
        /// Sets X and Y position of the floating window.
        /// </summary>
        /// <param name="rectWindow">floating window</param>
        /// <param name="currentPos">current position of the floating window</param>
        private void SetRectWindowXYPosition(ref Rect rectWindow, Point currentPos)
        {
            if (!(rectWindow.Width == DockingManager.MinWidhtInFloat))
            {
                rectWindow.X += currentPos.X - m_pointMouseStartPos.X;
            }
            if(!(rectWindow.Height==DockingManager.MinHeightInFloat))
                rectWindow.Y += currentPos.Y - m_pointMouseStartPos.Y;
        }

        /// <summary>
        /// Sets position of the floating window in Header FloatWindowBorderMode.
        /// </summary>
        /// <param name="rectWindow">floating window</param>
        /// <param name="currentPos">current position of the floating window</param>
        private void SetRectWindowForHeader(ref Rect rectWindow, Point currentPos)
        {
            if (m_isPressed)
            {
                SetRectWindowXYPosition(ref rectWindow, currentPos);

                DockingManager manager = ParentWindow.DockingManager;

                if (manager.DraggingType == DraggingType.NormalDragging)
                {
                    if (!ParentWindow.IsDragging)
                    {
                        ParentWindow.IsDragging = true;
                        ParentWindow.HitTestDisabled = true;
                    }
                }
                else if (!manager.CurrentDragPopup.IsOpen)
                {
                    List<FrameworkElement> dragList = new List<FrameworkElement>();

                    IEnumerable<Visual> hosts
                        = VisualUtils.EnumChildrenOfType(ParentWindow.Child, typeof(DockedElementTabbedHost));

                    foreach (DockedElementTabbedHost host in hosts)
                    {
                        dragList.AddRange(host.TabChildren);
                    }
                    if (ParentWindow!=null && ParentWindow.PrimaryElement != null)
                    {
                        manager.CurrentDragPopup.StartDragging(ParentWindow.PrimaryElement, dragList);
                    }
                }
            }
            else if (m_isResizing && RectHeightBiggerMinHeight(rectWindow, currentPos))
            {
                //int iOffset = (int)(currentPos.Y - m_pointMouseStartPos.Y);
                if(!(rectWindow.Height==DockingManager.MinHeightInFloat))
                    rectWindow.Y += currentPos.Y - m_pointMouseStartPos.Y;
                rectWindow.Height += m_pointMouseStartPos.Y - currentPos.Y;
                if (rectWindow.Height < DockingManager.MinHeightInFloat)
                    rectWindow.Height = DockingManager.MinHeightInFloat;
                if (DockingManager.MaxHeightInFloat > 0)
                {
                    if (rectWindow.Height > DockingManager.MaxHeightInFloat)
                    {
                        rectWindow.Height = DockingManager.MaxHeightInFloat;
                    }
                }
            }
        }

        /// <summary>
        /// Sets X Y Width Height position of the floating window in LeftTop FloatWindowBorderMode.
        /// </summary>
        /// <param name="rectWindow">floating window</param>
        /// <param name="currentPos">current position of the floating window</param>
        private void SetRectWindowWidthHeightXY(ref Rect rectWindow, Point currentPos)
        {
            if (m_isResizing)
            {
                if (RectWidthBiggerMinWidth(rectWindow, currentPos) &&
                    RectHeightBiggerMinHeight(rectWindow, currentPos))
                {
                    SetRectWindowXYPosition(ref rectWindow, currentPos);
                    rectWindow.Width += m_pointMouseStartPos.X - currentPos.X;
                    rectWindow.Height += m_pointMouseStartPos.Y - currentPos.Y;

                    if (rectWindow.Width < DockingManager.MinWidhtInFloat)
                    {
                        rectWindow.Width = DockingManager.MinWidhtInFloat;
                    }
                    if (rectWindow.Height < DockingManager.MinHeightInFloat)
                    {
                        rectWindow.Height = DockingManager.MinHeightInFloat;
                    }
                    if (DockingManager.MaxWidhtInFloat > 0)
                    {
                        if (rectWindow.Width > DockingManager.MaxWidhtInFloat)
                        {
                            rectWindow.Width = DockingManager.MaxWidhtInFloat;
                        }
                    }
                    if (DockingManager.MaxHeightInFloat > 0)
                    {
                        if (rectWindow.Height > DockingManager.MaxHeightInFloat)
                        {
                            rectWindow.Height = DockingManager.MaxHeightInFloat;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Completes the dragging.
        /// </summary>
        internal void CompleteDragging()
        {
            m_isPressed = false;
            m_dragCounter = 0;
            m_firedDragStart = false;

            ////DockingManager manager = (DockingManager)ParentWindow.PlacementTarget;

            ////if( m_firedDragStart )
            ////{
            ////    manager.FireWindowDragEnd();
            ////    m_firedDragStart = false;
            ////}

            ////if( manager.DraggingType == DraggingType.NormalDragging )
            ////{
            ////    ParentWindow.IsDragging = false;
            ////    ParentWindow.HitTestDisabled = false;
            ////}
            ////else
            ////    manager.CurrentDragPopup.CompleteDragging( true );
        }

        /// <summary>
        /// Completes the resizing.
        /// </summary>
        private void CompleteResizing()
        {
            m_isResizing = false;
            List<DockedElementTabbedHost> hosts = ParentWindow.DockingManager.GetContainerHosts(ParentWindow.FloatChild as DockedElementsContainer);
            foreach (DockedElementTabbedHost host in hosts)
            {
                int count = host.TabChildren.Count;
                FrameworkElement selectedtab = null;
                Rect placementrect = Rect.Empty;
                for (int i = 0; i < count; i++)
                {
                    string targetName = DockingManager.GetTargetNameInFloatingMode(host.TabChildren[i]);

                    if (!string.IsNullOrEmpty(targetName))
                    {
                        if (DockingManager.GetFloatingWindowRect(host.TabChildren[i]) != Rect.Empty)
                        {
                            placementrect = DockingManager.GetFloatingWindowRect(host.TabChildren[i]);
                        }
                        DockingManager.SetFloatingWindowRect(host.TabChildren[i], Rect.Empty);
                    }
                    if (DockingManager.GetIsSelectedTab(host.TabChildren[i]))
                    {
                        selectedtab = host.TabChildren[i];
                    }
                }
                if (selectedtab != null)
                {
                    if (placementrect != Rect.Empty)
                    {
                        DockingManager.SetFloatingWindowRect(selectedtab, placementrect);
                        if (DockingManager.GetFloatWindow(selectedtab).PlacementRectangle == Rect.Empty)
                        {
                            DockingManager.GetFloatWindow(selectedtab).PlacementRectangle = placementrect;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Sets Y Width Height position of the floating window in RightTop FloatWindowBorderMode.
        /// </summary>
        /// <param name="rectWindow">floating window</param>
        /// <param name="currentPos">current position of the floating window</param>
        private void SetRectWindowWidthHeightY(ref Rect rectWindow, Point currentPos)
        {
            if (m_isResizing)
            {
                if (RectWidthAndCurrentMoreMin(rectWindow, currentPos) &&
                    RectHeightBiggerMinHeight(rectWindow, currentPos))
                {
                    rectWindow.Width += currentPos.X - m_pointMouseStartPos.X;
                    if(!(rectWindow.Height==DockingManager.MinHeightInFloat))
                        rectWindow.Y += currentPos.Y - m_pointMouseStartPos.Y;
                    rectWindow.Height += m_pointMouseStartPos.Y - currentPos.Y;
                    if (rectWindow.Width < DockingManager.MinWidhtInFloat)
                    {
                        rectWindow.Width = DockingManager.MinWidhtInFloat;
                    }
                    if (rectWindow.Height < DockingManager.MinHeightInFloat)
                    {
                        rectWindow.Height = DockingManager.MinHeightInFloat;
                    }
                    if (DockingManager.MaxWidhtInFloat > 0)
                    {
                        if (rectWindow.Width > DockingManager.MaxWidhtInFloat)
                        {
                            rectWindow.Width = DockingManager.MaxWidhtInFloat;
                        }
                    }
                    if (DockingManager.MaxHeightInFloat > 0)
                    {
                        if (rectWindow.Height > DockingManager.MaxHeightInFloat)
                        {
                            rectWindow.Height = DockingManager.MaxHeightInFloat;
                        }
                    }
                }
               
            }
        }

        /// <summary>
        /// Sets X Width position of the floating window in Left FloatWindowBorderMode.
        /// </summary>
        /// <param name="rectWindow">floating window</param>
        /// <param name="currentPos">current position of the floating window</param>
        private void SetRectWindowWidthX(ref Rect rectWindow, Point currentPos)
        {
            if (m_isResizing && RectWidthBiggerMinWidth(rectWindow, currentPos))
            {
                rectWindow.Width += m_pointMouseStartPos.X - currentPos.X;
                if (rectWindow.Width < DockingManager.MinWidhtInFloat)
                {
                    rectWindow.Width = DockingManager.MinWidhtInFloat;
                }
                if (DockingManager.MaxWidhtInFloat > 0)
                {
                    if (rectWindow.Width > DockingManager.MaxWidhtInFloat)
                    {
                        rectWindow.Width = DockingManager.MaxWidhtInFloat;
                    }
                }

                if(!(rectWindow.Width==DockingManager.MinWidhtInFloat))
                    rectWindow.X += currentPos.X - m_pointMouseStartPos.X;
            }
        }

        /// <summary>
        /// Sets X Width Height position of the floating window in LeftBottom FloatWindowBorderMode.
        /// </summary>
        /// <param name="rectWindow">floating window</param>
        /// <param name="currentPos">current position of the floating window</param>
        private void SetRectWindowWidthHeightX(ref Rect rectWindow, Point currentPos)
        {
            if (m_isResizing)
            {
                if (RectWidthBiggerMinWidth(rectWindow, currentPos) &&
                    RectHeightAndCurrentMoreMin(rectWindow, currentPos))
                {
                    if(!(rectWindow.Width==DockingManager.MinWidhtInFloat))
                        rectWindow.X += currentPos.X - m_pointMouseStartPos.X;
                    rectWindow.Width += m_pointMouseStartPos.X - currentPos.X;
                    rectWindow.Height += currentPos.Y - m_pointMouseStartPos.Y;
                    if (rectWindow.Width < DockingManager.MinWidhtInFloat)
                    {
                        rectWindow.Width = DockingManager.MinWidhtInFloat;
                    }
                    if (rectWindow.Height < DockingManager.MinHeightInFloat)
                    {
                        rectWindow.Height = DockingManager.MinHeightInFloat;
                    }
                    if (DockingManager.MaxWidhtInFloat > 0)
                    {
                        if (rectWindow.Width > DockingManager.MaxWidhtInFloat)
                        {
                            rectWindow.Width = DockingManager.MaxWidhtInFloat;
                        }
                    }
                    if (DockingManager.MaxHeightInFloat > 0)
                    {
                        if (rectWindow.Height > DockingManager.MaxHeightInFloat)
                        {
                            rectWindow.Height = DockingManager.MaxHeightInFloat;
                        }
                    }
                }
               
            }
           
        }

        /// <summary>
        /// Sets Width Height position of the floating window in RightBottom FloatWindowBorderMode.
        /// </summary>
        /// <param name="rectWindow">floating window</param>
        /// <param name="currentPos">current position of the floating window</param>
        private void SetRectWindowWidthHeight(ref Rect rectWindow, Point currentPos)
        {
            if (m_isResizing)
            {
                if (RectWidthAndCurrentMoreMin(rectWindow, currentPos) &&
                    RectHeightAndCurrentMoreMin(rectWindow, currentPos))
                {
                    rectWindow.Width += currentPos.X - m_pointMouseStartPos.X;
                    if (rectWindow.Width < DockingManager.MinWidhtInFloat)
                    {
                        rectWindow.Width = DockingManager.MinWidhtInFloat;
                    }
                    rectWindow.Height += currentPos.Y - m_pointMouseStartPos.Y;
                    if (rectWindow.Height < DockingManager.MinHeightInFloat)
                    {
                        rectWindow.Height = DockingManager.MinHeightInFloat;
                    }
                    if (DockingManager.MaxWidhtInFloat > 0)
                    {
                        if (rectWindow.Width > DockingManager.MaxWidhtInFloat)
                        {
                            rectWindow.Width = DockingManager.MaxWidhtInFloat;
                        }
                    }
                    if (DockingManager.MaxHeightInFloat > 0)
                    {
                        if (rectWindow.Height > DockingManager.MaxHeightInFloat)
                        {
                            rectWindow.Height = DockingManager.MaxHeightInFloat;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Sets Width position of the floating window in Right FloatWindowBorderMode.
        /// </summary>
        /// <param name="rectWindow">floating window</param>
        /// <param name="currentPos">current position of the floating window</param>
        private void SetRectWindowWidth(ref Rect rectWindow, Point currentPos)
        {
            if (m_isResizing && RectWidthAndCurrentMoreMin(rectWindow, currentPos))
            {
                rectWindow.Width += currentPos.X - m_pointMouseStartPos.X;
                if (rectWindow.Width < DockingManager.MinWidhtInFloat)
                {
                    rectWindow.Width = DockingManager.MinWidhtInFloat;
                }
                if (DockingManager.MaxWidhtInFloat > 0)
                {
                    if (rectWindow.Width > DockingManager.MaxWidhtInFloat)
                    {
                        rectWindow.Width = DockingManager.MaxWidhtInFloat;
                    }
                }
            
            }
        }

        /// <summary>
        /// Sets Height position of the floating window in Bottom FloatWindowBorderMode.
        /// </summary>
        /// <param name="rectWindow">floating window</param>
        /// <param name="currentPos">current position of the floating window</param>
        private void SetRectWindowHeight(ref Rect rectWindow, Point currentPos)
        {
            if (m_isResizing && RectHeightAndCurrentMoreMin(rectWindow, currentPos))
            {
                rectWindow.Height += currentPos.Y - m_pointMouseStartPos.Y;
                if (rectWindow.Height < DockingManager.MinHeightInFloat)
                {
                    rectWindow.Height = DockingManager.MinHeightInFloat;
                }
                if (DockingManager.MaxHeightInFloat > 0)
                {
                    if (rectWindow.Height > DockingManager.MaxHeightInFloat)
                    {
                        rectWindow.Height = DockingManager.MaxHeightInFloat;
                    }
                }
            }
        }

        /// <summary>
        /// Sets PlacementRectangle for ParentWindow.
        /// </summary>
        /// <param name="rectWindow">floating window</param>
        /// <param name="rectOriginal">original Rect</param>
        private void SetParentWindowPlacementRectangle(Rect rectWindow, Rect rectOriginal)
        {
            if (rectOriginal != rectWindow)
            {
                DockingManager manager = ParentWindow.DockingManager;

                if (manager.DraggingType == DraggingType.NormalDragging || m_isResizing)
                {
                    System.Drawing.Rectangle rect = new System.Drawing.Rectangle((int)rectWindow.X, (int)rectWindow.Y, (int)rectWindow.Width, (int)rectWindow.Height);
                    System.Windows.Forms.Screen screen = System.Windows.Forms.Screen.FromRectangle(rect);
                    System.Drawing.Rectangle workingRectangle = screen.WorkingArea;
                    if (!m_isResizing)
                    {
                        if (workingRectangle.Contains((int)rectWindow.TopLeft.X + 30, (int)rectWindow.TopLeft.Y - 30))
                            SetPlacementRectangle(ParentWindow, ScreenUtils.FixByScreenBounds(rectWindow));
                    }
                    else if (m_isResizing)
                    {
                        if (rectOriginal.TopLeft.X < rectWindow.TopLeft.X)
                        {
                            if (workingRectangle.Contains((int)rectWindow.TopLeft.X + 20, (int)rectWindow.TopLeft.Y))
                                SetPlacementRectangle(ParentWindow, ScreenUtils.FixByScreenBounds(rectWindow));
                        }
                        else if (rectOriginal.TopRight.X > rectWindow.TopRight.X)
                        {
                            if (workingRectangle.Contains((int)rectWindow.TopRight.X - 20, (int)rectWindow.TopRight.Y))
                                SetPlacementRectangle(ParentWindow, ScreenUtils.FixByScreenBounds(rectWindow));
                        }
                        else if (rectOriginal.TopLeft.Y > rectWindow.TopLeft.Y)
                        {
                            if (workingRectangle.Contains((int)rectWindow.TopLeft.X, (int)rectWindow.TopLeft.Y - 3))
                                SetPlacementRectangle(ParentWindow, ScreenUtils.FixByScreenBounds(rectWindow));
                        }
                        else
                        {
                            SetPlacementRectangle(ParentWindow, ScreenUtils.FixByScreenBounds(rectWindow));
                        }
                    }                    
                }
                else
                {
                    manager.CurrentDragPopup.PlacementRectangle = ScreenUtils.FixByScreenBounds(rectWindow);
                }
            }
        }

        /// <summary>
        /// Called when [context menu opened].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">Instance containing the event data.</param>
        private void OnContextMenuOpened(object sender, RoutedEventArgs e)
        {
            UIElement hostedElement = FindHostedElement();
            bool noDock = (hostedElement != null) ? DockingManager.GetNoDock(hostedElement) : false;


            Visibility bdefault = Visibility.Visible;
            if (ParentWindow != null && hostedElement!=null 
                && (ParentWindow.DockingManager.CollapseDefaultContextMenuItems || DockingManager.GetCollapseDefaultContextMenuItemsInFloat(hostedElement as DependencyObject)))
            {
                bdefault = Visibility.Collapsed;
            }
            if (hostedElement != null && !DockingManager.GetCanDock(hostedElement) && DockingManager.GetCanFloat(hostedElement) && DockingManager.GetState(hostedElement) == DockState.Float)
            {
                noDock = true;
            }

            if (m_contextMenu == null)
            {
                m_contextMenu = GetTemplateChild("PART_ContextMenu") as CustomContextMenu;
            }

            if (m_contextMenu != null)
            {
                foreach (MenuItem item in m_contextMenu.Items)
                {
                    if (item != null)
                    {
                        switch (item.Name)
                        {
                            case FLOATING_MENUITEM_NAME:
                                item.IsChecked = noDock;
                                item.Visibility = bdefault;
                                if (bdefault == Visibility.Visible && ParentWindow != null && ParentWindow.PrimaryElement != null)
                                {
                                    if (DockingManager.GetShowFloatingMenuItem(ParentWindow.PrimaryElement))
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
                                if (bdefault == Visibility.Visible && ParentWindow != null && ParentWindow.PrimaryElement != null)
                                {
                                    if (DockingManager.GetShowDockableMenuItem(ParentWindow.PrimaryElement))
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
                                if (bdefault == Visibility.Visible && ParentWindow != null && ParentWindow.PrimaryElement != null)
                                {
                                    if (DockingManager.GetShowAutoHiddenMenuItem(ParentWindow.PrimaryElement))
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
                                if (bdefault == Visibility.Visible && ParentWindow != null && ParentWindow.PrimaryElement != null)
                                {
                                    if (DockingManager.GetShowHiddenMenuItem(ParentWindow.PrimaryElement))
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
                                if (bdefault == Visibility.Visible && ParentWindow != null && ParentWindow.PrimaryElement != null)
                                {
                                    if (DockingManager.GetShowTabbedMenuItem(ParentWindow.PrimaryElement))
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

        /// <summary>
        /// Called when [menu item click].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">Instance containing the event data.</param>
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

        /// <summary>
        /// Sets the floating.
        /// </summary>
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

                        owner.ExtractElementToWindow(hostedElement, ActionMode.Active, false);
                        DockingManager.SetNewFocusedElement(hostedElement);
                        owner.LockLayoutUpdate = true;
                    }
                }
            }
        }

        /// <summary>
        /// Sets the document.
        /// </summary>
        private void SetDocument()
        {
            DockingManager owner = ParentWindow.DockingManager;

            if (null != owner)
            {
                DockedElementTabbedHost host = ParentWindow.InternalDataContext as DockedElementTabbedHost;
                FrameworkElement element = host.InternalDataContext as FrameworkElement;
                owner.ExecuteDocument(element);
            }
        }

        /// <summary>
        /// Sets the dockable.
        /// </summary>
        private void SetDockable()
        {
            UIElement hostedElement = FindHostedElement();

            if (hostedElement != null && ParentWindow!=null && ParentWindow.PrimaryElement!=null)
            {
                DockingManager.SetNoDock(ParentWindow.PrimaryElement, false);
            }
        }

        /// <summary>
        /// Finds the hosted element.
        /// </summary>
        /// <returns>return framework element.</returns>
        private FrameworkElement FindHostedElement()
        {
            FrameworkElement returnElement = null;
            FrameworkElement element = ParentWindow.InternalDataContext as FrameworkElement;

            if (null != element)
            {
                returnElement = DockingManager.GetInternalDataContext(element);
                DataContext = element;
            }
            else
            {
                AutoTemplatedContentControl autoTemplatedContent = TemplatedParent as AutoTemplatedContentControl;

                if (null != autoTemplatedContent)
                {
                    IEnumerable<Visual> visuals = VisualUtils.EnumChildrenOfType(autoTemplatedContent, typeof(DockedElementTabbedHost));

                    foreach (DockedElementTabbedHost host in visuals)
                    {
                        returnElement = host.HostedElement;
                        DataContext = returnElement;
                        break;
                    }
                }
            }

            return returnElement;
        }

        /// <summary>
        /// Updates the cursor.
        /// </summary>
        /// <param name="pt">The point value.</param>
        private void UpdateCursor(Point pt)
        {
            switch (BorderMode)
            {
                case FloatWindowBorderMode.Header:
                    Cursor = (pt.Y <= 3 || m_isResizing) ? Cursors.SizeNS : Cursors.Arrow;
                    break;
                case FloatWindowBorderMode.LeftTop:
                case FloatWindowBorderMode.RightBottom:
                    Cursor = Cursors.SizeNWSE;
                    break;
                case FloatWindowBorderMode.LeftBottom:
                case FloatWindowBorderMode.RightTop:
                    Cursor = Cursors.SizeNESW;
                    break;
                case FloatWindowBorderMode.Left:
                case FloatWindowBorderMode.Right:
                    Cursor = Cursors.SizeWE;
                    break;
                case FloatWindowBorderMode.Bottom:
                    Cursor = Cursors.SizeNS;
                    break;
            }
        }

        /// <summary>
        /// Gets the placement rectangle.
        /// </summary>
        /// <param name="window">The window.</param>
        /// <returns>return rect value.</returns>
        private Rect GetPlacementRectangle(IWindow window)
        {
            Rect rect = new Rect();

            if (ParentWindow is FloatWindow)
            {
                rect = new Rect(ParentWindow.PlacementRectangle.Location, new Size(ParentWindow.Width, ParentWindow.Height));
            }
            else if (ParentWindow is AdornerFloatWindow)
            {
                rect = AdornerWindowsLayoutPanel.GetPlacementRactangle(ParentWindow as AdornerFloatWindow);
            }

            return rect;
        }

        bool isDragStart = false;

        /// <summary>
        /// Sets the placement rectangle.
        /// </summary>
        /// <param name="window">The window value.</param>
        /// <param name="rect">The rect value.</param>
        /// <returns>return rect value.</returns>
        private Rect SetPlacementRectangle(IWindow window, Rect rect)
        {
            if (ParentWindow is FloatWindow && (!isDragStart || !m_TouchDown))
            {
                ParentWindow.PlacementRectangle = ScreenUtils.FixByScreenBounds(rect);
                isDragStart = true;
            }
            else if (ParentWindow is AdornerFloatWindow && !m_isPressed)
            {
                AdornerWindowsLayoutPanel.SetPlacementRactangle(ParentWindow as AdornerFloatWindow, rect);
            }

            return rect;
        }

        /// <summary>
        /// Borders the mode to size direction.
        /// </summary>
        /// <param name="mode">The float window border mode.</param>
        /// <returns>return direction.</returns>
        private WindowInterop.SizingDirection BorderModeToSizeDirection(FloatWindowBorderMode mode)
        {
            WindowInterop.SizingDirection direction = WindowInterop.SizingDirection.None;

            switch (mode)
            {
                case FloatWindowBorderMode.Left:
                    direction = WindowInterop.SizingDirection.West;
                    break;

                case FloatWindowBorderMode.Right:
                    direction = WindowInterop.SizingDirection.East;
                    break;

                case FloatWindowBorderMode.Header:
                    direction = WindowInterop.SizingDirection.North;
                    break;

                case FloatWindowBorderMode.Bottom:
                    direction = WindowInterop.SizingDirection.South;
                    break;

                case FloatWindowBorderMode.LeftTop:
                    direction = WindowInterop.SizingDirection.NorthWest;
                    break;

                case FloatWindowBorderMode.RightTop:
                    direction = WindowInterop.SizingDirection.NorthEast;
                    break;

                case FloatWindowBorderMode.LeftBottom:
                    direction = WindowInterop.SizingDirection.SouthWest;
                    break;

                case FloatWindowBorderMode.RightBottom:
                    direction = WindowInterop.SizingDirection.SouthEast;
                    break;
            }

            return direction;
        }
        #endregion

        #region Dependency properties

        /// <summary>
        /// Identifies BorderMode dependency property of the FloatWindowBorder.
        /// </summary>
        public static readonly DependencyProperty BorderModeProperty =
            DependencyProperty.Register("BorderMode", typeof(FloatWindowBorderMode), typeof(FloatWindowBorder), new UIPropertyMetadata(FloatWindowBorderMode.Header));

        /// <summary>
        /// Identifies ParentWindow dependency property of the FloatWindowBorder.
        /// </summary>
        public static readonly DependencyProperty ParentWindowProperty =
            DependencyProperty.Register("ParentWindow", typeof(IWindow), typeof(FloatWindowBorder), new UIPropertyMetadata(null));

        /// <summary>
        /// Identifies MinWindowWidth dependency property of the FloatWindowBorder.
        /// </summary>
        public static readonly DependencyProperty MinWindowWidthProperty =
            DependencyProperty.Register("MinWindowWidth", typeof(double), typeof(FloatWindowBorder), new UIPropertyMetadata(0.0));

        /// <summary>
        /// Identifies MinWindowHeight dependency property of the FloatWindowBorder.
        /// </summary>
        public static readonly DependencyProperty MinWindowHeightProperty =
            DependencyProperty.Register("MinWindowHeight", typeof(double), typeof(FloatWindowBorder), new UIPropertyMetadata(0.0));

        /// <summary>
        /// Identifies ProcessDoubleClick dependency property of the FloatWindowBorder.
        /// </summary>
        public static readonly DependencyProperty ProcessDoubleClickProperty =
            DependencyProperty.Register("ProcessDoubleClick", typeof(bool), typeof(FloatWindowBorder), new UIPropertyMetadata(true));
        #endregion
    }
}
