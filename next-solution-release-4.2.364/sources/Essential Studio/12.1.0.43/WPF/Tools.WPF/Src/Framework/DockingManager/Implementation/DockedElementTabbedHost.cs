// <copyright file="DockedElementTabbedHost.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Syncfusion.Windows.Shared;
using System.Windows.Forms.Integration;
using System.Windows.Interop;
using System.Windows.Controls.Primitives;


namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents a control that contains multiple items.
    /// </summary>
    /// <remarks>
    /// You can use DockedElementTabbedHost class when you need to show tabs in the element.
    /// When there is no need to show tabs in the element you have to use <see cref="DockedElementTabbedHost"/> class.
    /// </remarks>
    /// <example>
    /// <para/>This example shows how to initialize the template of the DockedElementTabbedHost class in XAML.
    /// <code language="XAML">
    /// <![CDATA[
    /// <ControlTemplate x:Key="DockedElementTabbedHostTabbedTemplate" TargetType="{x:Type Syncfusion:DockedElementTabbedHost}">
    /// <Border x:Name="BorderWrapForTab" Width="Auto" SnapsToDevicePixels="True" Background="Transparent">
    /// <DockPanel x:Name="DockPanel" Width="Auto" LastChildFill="True">
    /// <Syncfusion:DockHeaderPresenter x:Name="header" DockPanel.Dock="Top" RenderTransformOrigin="0.5,0.5"
    /// Style="{Binding Path=(Syncfusion:DockingManager.DockHeaderStyle)
    /// , RelativeSource={RelativeSource AncestorType={x:Type Syncfusion:DockingManager}}}"
    /// IsTemplateParenKeyboardFocusWithin="{TemplateBinding IsKeyboardFocusWithin}"
    /// />
    /// <Grid>
    /// <TabControl Name="PART_TabControl"
    /// ItemsSource="{TemplateBinding TabChildren}" IsSynchronizedWithCurrentItem="True" Padding="0"
    /// Style="{Binding Path=(Syncfusion:DockingManager.TabControlStyle)
    /// , RelativeSource={RelativeSource AncestorType={x:Type Syncfusion:DockingManager}}}"
    /// TabStripPlacement="{Binding Path=(Syncfusion:DockingManager.DockTabAlignment)
    /// , RelativeSource={RelativeSource AncestorType={x:Type Syncfusion:DockingManager}}}"
    /// />
    /// <Border Name="PART_CoverletControl" Visibility="Collapsed" Background="Transparent" />
    /// </Grid>
    /// </DockPanel>
    /// </Border>
    /// </ControlTemplate>
    /// ]]>
    /// </code>
    /// </example>
    /// <seealso cref="DockedElementTabbedHost"/>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
public class DockedElementTabbedHost : ContentControl, IChildrenResize, IDisposable
    {
        #region Constants
        /// <summary>
        /// Specifies the tab control.
        /// </summary>
        private const string PART_TabControl = "PART_TabControl";

        /// <summary>
        /// Specifies the internal name.
        /// </summary>
        private const string PrefixName = "InternalName";

        /// <summary>
        /// Specifies the coverlet control
        /// </summary>
        private const string CoverletControl = "PART_CoverletControl";
        #endregion

        #region Private member

        internal List<FrameworkElement> m_previewedElements = new List<FrameworkElement>();

        internal bool m_isAbsoluteSizeUpdated = false;

        /// <summary>
        /// contains whether the active window is allowed to change
        /// </summary>
        internal bool m_cancelActiveWindowChange = false;

        /// <summary>
        /// contains sibling element
        /// </summary>
        internal FrameworkElement m_siblingElement = null;

        /// <summary>
        /// contains template child of the DockedElementTabbedHost
        /// </summary>
        internal TabControl m_tabControl = null;

        /// <summary>
        /// determines whether any item is inserted into DockedElementTabbedHost
        /// </summary>
        internal bool m_insertitem = false;

        /// <summary>
        /// contains first DirectTabPanel of the current direct panels
        /// </summary>
        private DirectTabPanel m_tabPanel = null;

        /// <summary>
        /// Specifies m_isTabOrderUpdateStarted.
        /// </summary>
        private bool m_isTabOrderUpdateStarted = false;

        /// <summary>
        /// Specifies the border.
        /// </summary>
        private Border m_fakeBorder = null;

        /// <summary>
        /// Specifies m_bLockedTabOrderChanged.
        /// </summary>
        private bool m_bLockedTabOrderChanged;

        /// <summary>
        /// Specifies the size.
        /// </summary>
        internal Size m_desiredSize = Size.Empty;

        internal FrameworkElement m_firstelement;

        internal bool m_doubleclick = false;

        internal bool isPriorityControl = false;

        /// <summary>
        /// Specify the min element width.
        /// </summary>
        private const double MinElementWidth = 50;

        /// <summary>
        /// Specify the min element height.
        /// </summary>
        private const double MinElementHeight = 20;

        #endregion

        #region Initialization
        /// <summary>
        /// Initializes a new instance of the <see cref="DockedElementTabbedHost"/> class.
        /// </summary>
        /// <param name="docking">The docking.</param>
        public DockedElementTabbedHost(DockingManager docking)
        {
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                if (Application.Current != null && !BindingUtils.GetEnableBindingErrors(Application.Current.MainWindow))
                    System.Diagnostics.PresentationTraceSources.DataBindingSource.Switch.Level = System.Diagnostics.SourceLevels.Critical;
            }
            DockingManager = docking;           
            TabChildren = new ObservableFrameworkElements();
            TabChildren.CollectionChanged += new NotifyCollectionChangedEventHandler(OnTabChildrenCollectionChanged);
            this.Unloaded += new RoutedEventHandler(DockedElementTabbedHost_Unloaded);
            DockingManager.Unloaded += new RoutedEventHandler(DockingManager_Unloaded);

            // SD 12516 - NullReferenceException while Floating the DocumentTabs .

            Loaded += (sender, e) =>
                {
                    m_tabControl = GetTemplateChild(PART_TabControl) as TabControl;
                };
        }

        void DockingManager_Unloaded(object sender, RoutedEventArgs e)
        {

            this.Dispose();
        }

        #region IDisposable members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            GotFocus -= new RoutedEventHandler(ChildGotFocus);
            LostFocus -= new RoutedEventHandler(ChildLostFocus);
            if (m_tabControl != null && m_tabControl.Items.Count == 0)
            {
                m_tabControl.SelectionChanged -= new SelectionChangedEventHandler(OnChildTabControlSelectionChanged);
                m_tabControl.PreviewMouseLeftButtonDown -= new MouseButtonEventHandler(m_tabControl_PreviewMouseLeftButtonDown);
#if !SyncfusionFramework3_5
                //m_tabControl.PreviewTouchDown -= m_tabControl_PreviewTouchDown;
#endif
                if (m_tabControl.SelectedContent == HostedElement)
                    m_tabControl.SelectedItem = null;
            }
            DockingManager.Unloaded -= new RoutedEventHandler(DockingManager_Unloaded);
            this.Unloaded -= new RoutedEventHandler(DockedElementTabbedHost_Unloaded);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        void IDisposable.Dispose()
        {
            this.Dispose();
        }

        #endregion

        /// <summary>
        /// Handles the Unloaded event of the DockedElementTabbedHost control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void DockedElementTabbedHost_Unloaded(object sender, RoutedEventArgs e)
        {
            m_fakeBorder = null;
            m_firstelement = null;
            m_tabControl = null;
            m_tabPanel = null;
        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="DockedElementTabbedHost"/> is reclaimed by garbage collection.
        /// </summary>
        ~DockedElementTabbedHost()
        {
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the tab parent.
        /// </summary>
        /// <value>The tab parent.</value>
        public FrameworkElement TabParent
        {
            get
            {
                return m_firstelement;
            }
            set
            {
                m_firstelement = value;
            }
        }

        /// <summary>
        /// Gets internal tab control of the <see cref="DockedElementTabbedHost"/> instance.
        /// </summary>
        public TabControl InternalTabControl
        {
            get
            {
                //return m_tabControl;
                return GetTemplateChild(PART_TabControl) as TabControl;
            }
        }

        /// <summary>
        /// Gets first DirectTabPanel of the current direct panels if <see cref="DockedElementTabbedHost"/> instance is loaded.
        /// </summary>
        public DirectTabPanel InternalTabPanel
        {
            get
            {
                if (m_tabControl != null && m_tabPanel == null && IsLoaded)
                {
                    IEnumerable<Visual> directPanels = VisualUtils.EnumChildrenOfType(m_tabControl, typeof(DirectTabPanel));

                    foreach (DirectTabPanel panel in directPanels)
                    {
                        m_tabPanel = panel;
                        break;
                    }
                }

                return m_tabPanel;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether a Boolean value that indicates whether to show tabs. This is a dependency property.
        /// </summary>
        /// <remarks>
        /// ShowTabs property is used for <see cref="DockedElementTabbedHost"/> to determine whether it should show tabs or not.
        /// For example if number of tabs is 1 there is no necessary to display it alone and this property should be set to false in that case.
        /// </remarks>
        public bool ShowTabs
        {
            get
            {
                return (bool)GetValue(ShowTabsProperty);
            }

            set
            {
                SetValue(ShowTabsProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether a Boolean value that indicates whether to allow group auto hide This is a dependency property.
        /// </summary>
        /// <remarks>
        /// CanAutoHideGroup property is used by <see cref="DockedElementTabbedHost"/> to know whether all tabs can be auto hidden
        /// if DockingManager.AutoHideTabsMode is set to AutoHideGroup. This allows prevent moving the entire group of tabs 
        /// to auto hidden state through GUI by hiding awl button in header and disabling AutoHide context menu item.
        /// </remarks>
        public bool CanAutoHideGroup
        {
            get
            {
                return (bool)GetValue(CanAutoHideGroupProperty);
            }

            set
            {
                SetValue(CanAutoHideGroupProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether a Boolean value that indicates whether to allow group close. This is a dependency property.
        /// </summary>
        /// <remarks>
        /// CanCloseGroup property is used by <see cref="DockedElementTabbedHost"/> to know whether all tabs can be hidden
        /// if DockingManager.CloseTabs mode is set to CloseAll.This allows prevent moving the entire group of tabs 
        /// to hidden state through GUI by hiding close button in header and disabling Hide context menu item.
        /// </remarks>
        public bool CanCloseGroup
        {
            get
            {
                return (bool)GetValue(CanCloseGroupProperty);
            }

            set
            {
                SetValue(CanCloseGroupProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the collection of the tabs that should be
        /// displayed in tabbed mode.
        /// </summary>
        public ObservableFrameworkElements TabChildren
        {
            get
            {
                return (ObservableFrameworkElements)GetValue(TabChildrenProperty);
            }

            protected set
            {
                SetValue(TabChildrenPropertyKey, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether a dock state for the <see cref="DockedElementTabbedHost"/> instance.
        /// This is a dependency property.
        /// </summary>
        /// <remarks>
        /// State property is used to know what desired size of hosted element ( dock or float)
        /// use to calculate final size of host.
        /// You can use State property when you override templates of the dock or float windows.
        /// </remarks>
        /// <example>
        /// <para/>This example shows how to use State property in XAML. If you override the template of the float window,
        /// you have to hide context menu button and auto-hide button of the floating window header. 
        /// <code language="XAML">
        /// <![CDATA[
        /// <DataTrigger Binding="{Binding Path=(FrameworkElement.DataContext).(Syncfusion:DockingManager.State), RelativeSource={RelativeSource TemplatedParent}}"
        /// Value="Float" >
        /// <Setter TargetName="PART_AwlButton" Property="Visibility" Value="Collapsed" />
        /// <Setter TargetName="PART_ContextMenu" Property="IsEnabledAutoHideMenuItem" Value="False" />
        /// </DataTrigger>
        /// ]]>
        /// </code>
        /// </example>
        /// <seealso cref="DockState"/>
        public DockState State
        {
            get
            {
                return (DockState)GetValue(StateProperty);
            }

            set
            {
                SetValue(StateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether an element that is hosted in <see cref="DockedElementTabbedHost"/>.
        /// This is a dependency property.
        /// </summary>
        /// <remarks>
        /// HostedElement property is useful when you want to get or set element of the <see cref="DockedElementTabbedHost"/>.
        /// You can use HostedElement property when you override style of the <see cref="DockedElementTabbedHost"/>.
        /// </remarks>
        /// <example>
        /// <para/>This example shows how to use HostedElement property in XAML.
        /// <code language="XAML">
        /// <![CDATA[
        ///   <Style x:Key="DockedElementTabbedHostStyle" TargetType="{x:Type Syncfusion:DockedElementTabbedHost}">
        ///     <Setter Property="DataContext" Value="{Binding Path=(Syncfusion:DockedElementTabbedHost.HostedElement), RelativeSource={RelativeSource Self}}"/>
        ///     <Setter Property="Template" Value="{StaticResource DockedElementTabbedHostTemplate}" />
        ///   </Style>
        /// ]]>
        /// </code>
        /// </example>
        public FrameworkElement HostedElement
        {
            get
            {
                return (FrameworkElement)GetValue(HostedElementProperty);
            }

            set
            {
                SetValue(HostedElementProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the size that this element has as desired in it's current dock state.
        /// </summary>
        /// <returns>Desired size of element in it's current state.</returns>
        Size IDesiredSize.DesiredSize
        {
            get
            {
                if (m_desiredSize == Size.Empty)
                {
                    InitDesiredSize();
                }

                return m_desiredSize;
            }

            set
            {
                m_desiredSize = value;

                if (HostedElement != null && !DockingManager.GetDockToFill(HostedElement))
                {
                    switch (State)
                    {
                        case DockState.Dock:
                            DockingManager.SetDesiredWidthInDockedMode(HostedElement, m_desiredSize.Width);
                            DockingManager.SetDesiredHeightInDockedMode(HostedElement, m_desiredSize.Height);
                            break;
                        case DockState.Float:
                            DockingManager.SetDesiredWidthInFloatingMode(HostedElement, m_desiredSize.Width);
                            DockingManager.SetDesiredHeightInFloatingMode(HostedElement, m_desiredSize.Height);
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether element is marked as frozen. This is a dependency property.
        /// </summary>
        public bool MarkAsFrozen
        {
            get
            {
                return (bool)GetValue(MarkAsFrozenProperty);
            }

            set
            {
                SetValue(MarkAsFrozenProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the value indicating whether one of the children is focused.
        /// </summary>
        protected internal bool IsChildFocused
        {
            get
            {
                return (bool)GetValue(IsChildFocusedProperty);
            }

            set
            {
                SetValue(IsChildFocusedPropertyKey, value);
            }
        }

        /// <summary>
        /// Gets the fake border.
        /// </summary>
        /// <value>The fake border.</value>
        internal Border FakeBorder
        {
            get
            {
                if (null == m_fakeBorder)
                {
                    m_fakeBorder = (Border)Template.FindName(CoverletControl, this);
                }

                return m_fakeBorder;
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
        #endregion

        #region Public methods
        /// <summary>
        /// Sets the desired width for a hosted element.
        /// </summary>
        /// <param name="width">Desired width.</param>
        void IChildrenResize.SetWidth(double width)
        {
            if (HostedElement != null)
            {
                if (!DockingManager.GetDockToFill(HostedElement))
                {
                    switch (State)
                    {
                        case DockState.Dock:
                            bool m_cancheck = this.Parent != null ? this.Parent is DockedElementsContainer ? (this.Parent as DockedElementsContainer).m_checkminmaxsize : true : true;
                            if (m_cancheck)
                            {
                                double minwidth = DockingManager.GetDesiredMinWidthInDockedMode(HostedElement);
                                double maxwidth = DockingManager.GetDesiredMaxWidthInDockedMode(HostedElement);
                                double minHeight = DockingManager.GetDesiredMinHeightInDockedMode(HostedElement);
                                double maxHeight = DockingManager.GetDesiredMaxHeightInDockedMode(HostedElement);
                                if (width < minwidth)
                                {
                                    if (minwidth > 0)
                                    {
                                        width = minwidth;
                                    }
                                }
                                if (width > maxwidth)
                                {
                                    if (maxwidth > 0)
                                    {
                                        width = maxwidth;
                                    }
                                }
                            }
                            width = width < 0 ? MinElementWidth : width;
                            m_desiredSize = new Size(width, m_desiredSize.Height);
                            if (DockingManager.GetSizetoContentInDock(HostedElement))
                            {
                                if (width > DockingManager.GetDesiredWidthInDockedMode(HostedElement))
                                {
                                    DockingManager.SetDesiredWidthInDockedMode(HostedElement, width);
                                }
                            }
                            else
                            {
                                if (!DockingManager.CheckResize(HostedElement, DockState.Dock, Orientation.Horizontal)
                                    && DockingManager.CheckFixedsize(HostedElement, Orientation.Horizontal)
                                    && (DockingManager.GetSideInDockedMode(HostedElement) == DockSide.Left
                                    || DockingManager.GetSideInDockedMode(HostedElement) == DockSide.Right))
                                {
                                    if (DockingManager.GetFixedWidth(HostedElement) != 90)
                                    {
                                        if (DockingManager.GetDesiredWidthInDockedMode(HostedElement) != width
                                            && DockingManager.GetFixedWidth(HostedElement)!=0
                                            && DockingManager.GetDesiredWidthInDockedMode(HostedElement) != DockingManager.GetFixedWidth(HostedElement))
                                        {
                                            DockingManager.SetDesiredWidthInDockedMode(HostedElement, DockingManager.GetFixedWidth(HostedElement));
                                            ((IChildrenResize)this).SetWidth(DockingManager.GetFixedWidth(HostedElement));
                                        }
                                        else
                                        {
                                            DockingManager.SetDesiredWidthInDockedMode(HostedElement, width);
                                        }
                                    }
                                    else
                                    {
                                        if (DockingManager.GetDesiredWidthInDockedMode(HostedElement) != width)
                                        {
                                            ((IChildrenResize)this).SetWidth(DockingManager.GetDesiredWidthInDockedMode(HostedElement));
                                        }
                                    }
                                }
                                else
                                {
                                    DockingManager.SetDesiredWidthInDockedMode(HostedElement, width);
                                }
                            }
                            break;
                        case DockState.Float:
                            m_cancheck = this.Parent != null ? this.Parent is DockedElementsContainer ? (this.Parent as DockedElementsContainer).m_checkminmaxsize : true : true;
                            if (m_cancheck)
                            {
                                double minwidthfloat = DockingManager.GetDesiredMinWidthInFloatingMode(HostedElement);
                                double maxwidthfloat = DockingManager.GetDesiredMaxWidthInFloatingMode(HostedElement);
                                double minHeightfloat = DockingManager.GetDesiredMinHeightInFloatingMode(HostedElement);
                                double maxHeightfloat = DockingManager.GetDesiredMaxHeightInFloatingMode(HostedElement);
                                if (width < minwidthfloat)
                                {
                                    if (minwidthfloat > 0)
                                    {
                                        width = minwidthfloat;
                                    }
                                }
                                if (width > maxwidthfloat)
                                {
                                    if (maxwidthfloat > 0)
                                    {
                                        width = maxwidthfloat;
                                    }
                                }
                            }
                            width = width < 0 ? MinElementWidth : width;
                            m_desiredSize = new Size(width, m_desiredSize.Height);
                            if (DockingManager.GetSizetoContentInFloat(HostedElement))
                            {
                                if (width > DockingManager.GetDesiredWidthInFloatingMode(HostedElement))
                                {
                                    DockingManager.SetDesiredWidthInFloatingMode(HostedElement, width);
                                }
                            }
                            else
                            {
                                DockingManager.SetDesiredWidthInFloatingMode(HostedElement, width);
                            }
                            break;
                    }
                }

                VisualUtils.InvalidateParentMeasure(this);
            }
        }

        /// <summary>
        /// Sets the desired height for a hosted element.
        /// </summary>
        /// <param name="height">Desired height</param>
        void IChildrenResize.SetHeight(double height)
        {
            if (HostedElement != null)
            {
                if (!DockingManager.GetDockToFill(HostedElement))
                {
                    switch (State)
                    {
                        case DockState.Dock:
                            bool m_cancheck = this.Parent != null ? this.Parent is DockedElementsContainer ? (this.Parent as DockedElementsContainer).m_checkminmaxsize : true : true;
                            if (m_cancheck)
                            {
                                double minwidth = DockingManager.GetDesiredMinWidthInDockedMode(HostedElement);
                                double maxwidth = DockingManager.GetDesiredMaxWidthInDockedMode(HostedElement);
                                double minHeight = DockingManager.GetDesiredMinHeightInDockedMode(HostedElement);
                                double maxHeight = DockingManager.GetDesiredMaxHeightInDockedMode(HostedElement);
                                if (height < minHeight)
                                {
                                    if (minHeight > 0)
                                    {
                                        height = minHeight;
                                    }
                                }
                                if (height > maxHeight)
                                {
                                    if (maxHeight > 0)
                                    {
                                        height = maxHeight;
                                    }
                                }
                            }
                            height = height < MinElementHeight ? MinElementHeight : height;
                            m_desiredSize = new Size(m_desiredSize.Width, height);
                            if (DockingManager.GetSizetoContentInDock(HostedElement))
                            {
                                if (height > DockingManager.GetDesiredHeightInDockedMode(HostedElement))
                                {
                                    DockingManager.SetDesiredHeightInDockedMode(HostedElement, height);
                                }
                            }
                            else
                            {
                                if (!DockingManager.CheckResize(HostedElement, DockState.Dock, Orientation.Vertical)
                                    && DockingManager.CheckFixedsize(HostedElement, Orientation.Vertical)
                                    && (DockingManager.GetSideInDockedMode(HostedElement) == DockSide.Top
                                    || DockingManager.GetSideInDockedMode(HostedElement) == DockSide.Bottom))
                                {
                                    if (DockingManager.GetFixedHeight(HostedElement) != 90)
                                    {
                                        if (DockingManager.GetDesiredHeightInDockedMode(HostedElement) != height
                                            && DockingManager.GetFixedHeight(HostedElement) != 0
                                            && DockingManager.GetDesiredHeightInDockedMode(HostedElement) != DockingManager.GetFixedHeight(HostedElement))
                                        {
                                            DockingManager.SetDesiredHeightInDockedMode(HostedElement, DockingManager.GetFixedHeight(HostedElement));
                                            ((IChildrenResize)this).SetHeight(DockingManager.GetFixedHeight(HostedElement));
                                        }
                                        else
                                        {
                                            DockingManager.SetDesiredHeightInDockedMode(HostedElement, height);
                                        }
                                    }
                                    else
                                    {
                                        if (DockingManager.GetDesiredHeightInDockedMode(HostedElement) != height)
                                        {
                                            ((IChildrenResize)this).SetHeight(DockingManager.GetDesiredHeightInDockedMode(HostedElement));
                                        }
                                    }
                                }
                                else
                                {
                                    DockingManager.SetDesiredHeightInDockedMode(HostedElement, height);
                                }
                            }
                            break;
                        case DockState.Float:
                            m_cancheck = this.Parent != null ? this.Parent is DockedElementsContainer ? (this.Parent as DockedElementsContainer).m_checkminmaxsize : true : true;
                            if (m_cancheck)
                            {
                                double minwidthfloat = DockingManager.GetDesiredMinWidthInFloatingMode(HostedElement);
                                double maxwidthfloat = DockingManager.GetDesiredMaxWidthInFloatingMode(HostedElement);
                                double minHeightfloat = DockingManager.GetDesiredMinHeightInFloatingMode(HostedElement);
                                double maxHeightfloat = DockingManager.GetDesiredMaxHeightInFloatingMode(HostedElement);
                                if (height < minHeightfloat)
                                {
                                    if (minHeightfloat > 0)
                                    {
                                        height = minHeightfloat;
                                    }
                                }
                                if (height > maxHeightfloat)
                                {
                                    if (maxHeightfloat > 0)
                                    {
                                        height = maxHeightfloat;
                                    }
                                }
                            }
                            height = height < 0 ? MinElementHeight : height;
                            m_desiredSize = new Size(m_desiredSize.Width, height);
                            if (DockingManager.GetSizetoContentInFloat(HostedElement))
                            {
                                if (height > DockingManager.GetDesiredHeightInFloatingMode(HostedElement))
                                {
                                    DockingManager.SetDesiredHeightInFloatingMode(HostedElement, DockingManager.GetDesiredHeightInFloatingMode(HostedElement));
                                }
                            }
                            else
                            {
                                DockingManager.SetDesiredHeightInFloatingMode(HostedElement, height);
                            }
                            break;
                    }
                }

                VisualUtils.InvalidateParentMeasure(this);
            }
        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code 
        /// or internal processes call <see cref="System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (this.InternalDataContext != null)
            {
                double externalbordersize = 16;
                WindowsFormsHost m_windowsformshost = null;

                if (DockingManager.UseInteropCompatibilityMode)
                {
                    m_windowsformshost = VisualUtils.FindDescendant((Visual)this.InternalDataContext, typeof(WindowsFormsHost)) as WindowsFormsHost;
                }           
              
                DockHeaderPresenter header = GetTemplateChild("header") as DockHeaderPresenter;
                if (header != null)
                {
                    double headerheight = 0;
                    header.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    if (header.DesiredSize.Height <= 0)
                    {
                        headerheight = 19.96;
                    }
                    else
                    {                        
                        headerheight = header.DesiredSize.Height;
                    }
                    if (DockingManager.GetDesiredClientHeightInDockedMode(this.InternalDataContext) != 90d)
                    {
                        DockingManager.SetDesiredHeightInDockedMode(this.InternalDataContext, headerheight + externalbordersize + DockingManager.GetDesiredClientHeightInDockedMode(this.InternalDataContext));
                        (this as IChildrenResize).SetHeight(headerheight + externalbordersize + DockingManager.GetDesiredClientHeightInDockedMode(this.InternalDataContext));
                    }
                    if (DockingManager.GetDesiredClientHeightInFloatMode(this.InternalDataContext) != 90d)
                    {
                        DockingManager.SetDesiredHeightInFloatingMode(this.InternalDataContext, headerheight + externalbordersize + DockingManager.GetDesiredClientHeightInFloatMode(this.InternalDataContext));
                        Rect rect1 = new Rect();
                        rect1.Height = headerheight + externalbordersize + DockingManager.GetDesiredClientHeightInFloatMode(this.InternalDataContext);
                        rect1.Width = DockingManager.GetDesiredWidthInFloatingMode(this.InternalDataContext as DependencyObject);
                        DockingManager.SetFloatingWindowRect(this.InternalDataContext as DependencyObject, rect1);
                    }

                    if (DockingManager.GetSizetoContentInDock(this.InternalDataContext))
                    {
                        (this.InternalDataContext).Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                        if ((this.InternalDataContext).DesiredSize.Height != 0 && (this.InternalDataContext).DesiredSize.Width != 0)
                        {
                            DockingManager.SetDesiredHeightInDockedMode((this.InternalDataContext) as DependencyObject, (this.InternalDataContext).DesiredSize.Height + headerheight + externalbordersize);
                            DockingManager.SetDesiredWidthInDockedMode((this.InternalDataContext) as DependencyObject, (this.InternalDataContext).DesiredSize.Width + externalbordersize);
                        }
                    }

                    if (DockingManager.GetSizetoContentInFloat(this.InternalDataContext))
                    {
                        (this.InternalDataContext).Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                        if ((this.InternalDataContext).DesiredSize.Height != 0 && (this.InternalDataContext).DesiredSize.Width != 0)
                        {
                            Rect rect = new Rect();
                            if (!double.IsInfinity(DockingManager.GetFloatingWindowRect(this.InternalDataContext as DependencyObject).X))
                            {
                                rect.X = (DockingManager.GetFloatingWindowRect(this.InternalDataContext as DependencyObject)).X;
                            }
                            if (!double.IsInfinity(DockingManager.GetFloatingWindowRect(this.InternalDataContext as DependencyObject).Y))
                            {
                                rect.Y = (DockingManager.GetFloatingWindowRect(this.InternalDataContext as DependencyObject)).Y;
                            }
                            if ((this.InternalDataContext).ActualHeight > (this.InternalDataContext).DesiredSize.Height)
                            {
                                DockingManager.SetDesiredHeightInFloatingMode((this.InternalDataContext) as DependencyObject, (this.InternalDataContext).ActualHeight + headerheight + externalbordersize);
                                rect.Height = (this.InternalDataContext).ActualHeight + headerheight + externalbordersize;
                            }
                            else if (m_windowsformshost != null && m_windowsformshost.ActualHeight > (this.InternalDataContext).ActualHeight && m_windowsformshost.ActualHeight != 0)
                            {
                                DockingManager.SetDesiredHeightInFloatingMode((this.InternalDataContext) as DependencyObject, m_windowsformshost.ActualHeight + headerheight + externalbordersize);
                                rect.Height = m_windowsformshost.ActualHeight + headerheight + externalbordersize;
                            }
                            else
                            {
                                DockingManager.SetDesiredHeightInFloatingMode((this.InternalDataContext) as DependencyObject, (this.InternalDataContext).DesiredSize.Height + headerheight + externalbordersize);
                                rect.Height = (this.InternalDataContext).DesiredSize.Height + headerheight + externalbordersize;
                            }
                            if ((this.InternalDataContext).ActualWidth > (this.InternalDataContext).DesiredSize.Width)
                            {
                                DockingManager.SetDesiredWidthInFloatingMode((this.InternalDataContext) as DependencyObject, (this.InternalDataContext).ActualWidth + externalbordersize);
                                rect.Width = (this.InternalDataContext).ActualWidth + externalbordersize;
                            }
                            else if (m_windowsformshost != null &&m_windowsformshost.ActualWidth > (this.InternalDataContext).ActualWidth && m_windowsformshost.ActualWidth != 0)
                            {
                                DockingManager.SetDesiredWidthInFloatingMode((this.InternalDataContext) as DependencyObject, m_windowsformshost.ActualWidth + externalbordersize);
                                rect.Width = m_windowsformshost.ActualWidth + externalbordersize;
                            }
                            else
                            {
                                DockingManager.SetDesiredWidthInFloatingMode((this.InternalDataContext) as DependencyObject, (this.InternalDataContext).DesiredSize.Width + externalbordersize);
                                rect.Width = (this.InternalDataContext).DesiredSize.Width + externalbordersize;
                                
                            }
                            DockingManager.SetFloatingWindowRect((this.InternalDataContext) as DependencyObject, rect);
                        }
                    }
                }
            }

            m_tabPanel = null;
            m_fakeBorder = GetTemplateChild(CoverletControl) as Border;
            m_tabControl = GetTemplateChild(PART_TabControl) as TabControl;

            if (null != m_tabControl)
            {
                m_tabControl.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(m_tabControl_PreviewMouseLeftButtonDown);
#if !SyncfusionFramework3_5
                //if(DockingManager.IsTouchEnabled)
                //    m_tabControl.PreviewTouchDown += m_tabControl_PreviewTouchDown;
#endif
                m_tabControl.SelectionChanged += new SelectionChangedEventHandler(OnChildTabControlSelectionChanged);
                DockedElementTabbedHost.SetSelectedItem(m_tabControl);
                DockedElementTabbedHost.UpdateTabOrder(m_tabControl, State);

                if (null != m_tabControl.SelectedItem)
                {
                    DockingManager.SetIsSelectedTab((DependencyObject)m_tabControl.SelectedItem, true);
                    if (this.State == DockState.Float)
                    {
                        FloatWindow FloatWind = DockingManager.GetFloatWindow(m_tabControl.SelectedItem as FrameworkElement) as FloatWindow;
                        if (FloatWind != null)
                        {
                            FloatWindowBorder FloatWindBord = FloatWind.Header as FloatWindowBorder;
                            if (FloatWindBord != null)
                            {
                                FloatWindBord.ToEnableorDisableCloseButton(m_tabControl.SelectedItem as FrameworkElement);
                                if (FloatWindBord.CloseButton != null && (m_tabControl.SelectedItem as DependencyObject) != null)
                                {
                                    DockingManager owner = DockingManager.ResolveManager(m_tabControl.SelectedItem as UIElement);
                                    if (owner != null)
                                    {
                                        bool close = DockingManager.GetCanClose(m_tabControl.SelectedItem as DependencyObject);
                                        if (!close)
                                        {
                                            switch (owner.DisabledCloseButtonsBehavior)
                                            {
                                                case DisabledButtonsBehavior.Collapse:
                                                    FloatWindBord.CloseButton.Visibility = Visibility.Collapsed;
                                                    break;
                                                case DisabledButtonsBehavior.Hide:
                                                    FloatWindBord.CloseButton.Visibility = Visibility.Hidden;
                                                    break;
                                                case DisabledButtonsBehavior.Disable:
                                                    FloatWindBord.CloseButton.Visibility = Visibility.Visible;
                                                    break;
                                            }
                                        }
                                        else
                                        {
                                            FloatWindBord.CloseButton.Visibility = Visibility.Visible;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Handles the PreviewMouseLeftButtonDown event of the m_tabControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        void m_tabControl_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.StylusDevice == null || e.StylusDevice != null)
                m_insertitem = false;
        }

        /// <summary>
        /// Gets the height of the previous host.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        internal static double GetPreviousHostHeight(DependencyObject obj)
        {
            return (double)obj.GetValue(PreviousHostHeightProperty);
        }

        /// <summary>
        /// Gets the width of the previous host.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        internal static double GetPreviousHostWidth(DependencyObject obj)
        {
            return (double)obj.GetValue(PreviousHostWidthProperty);
        }

        /// <summary>
        /// Sets the width of the previous host.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="value">The value.</param>
        internal static void SetPreviousHostWidth(DependencyObject obj,double value)
        {
            obj.SetValue(PreviousHostWidthProperty,value);
        }

        /// <summary>
        /// Sets the height of the previous host.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="value">The value.</param>
        internal static void SetPreviousHostHeight(DependencyObject obj,double value)
        {
            obj.SetValue(PreviousHostHeightProperty,value);
        }

        /// <summary>
        /// Gets tab order for a given DependencyObject in specified dock state.
        /// </summary>
        /// <param name="obj">The element from which to read the tab order.</param>
        /// <param name="state">Dock state.</param>
        /// <returns>The tab order in specified state.</returns>
        public static int GetTabOrder(DependencyObject obj, DockState state)
        {
            int result = -1;

            switch (state)
            {
                case DockState.Dock:
                    result = DockedElementTabbedHost.GetTabOrderInDockMode(obj);
                    break;
                case DockState.Float:
                    result = DockedElementTabbedHost.GetTabOrderInFloatMode(obj);
                    break;
            }

            return result;
        }

        /// <summary>
        /// Sets the tab order for a given DependencyObject in specified dock state.
        /// </summary>
        /// <param name="obj">The element on which to set the tab order.</param>
        /// <param name="state">Dock state.</param>
        /// <param name="value">The tab order to be set.</param>
        public static void SetTabOrder(DependencyObject obj, DockState state, int value)
        {
            switch (state)
            {
                case DockState.Dock:
                    DockedElementTabbedHost.SetTabOrderInDockMode(obj, value);
                    break;
                case DockState.Float:
                    DockedElementTabbedHost.SetTabOrderInFloatMode(obj, value);
                    break;
            }
        }

        /// <summary>
        /// Gets the value of the DockedElementTabbedHost.TabOrderInDockMode�attached property from a given DependencyObject.
        /// </summary>
        /// <param name="obj">The element from which to read the property value.</param>
        /// <returns>The value of the DockedElementTabbedHost.TabOrderInDockMode�attached property.</returns>
        public static int GetTabOrderInDockMode(DependencyObject obj)
        {
            return (int)obj.GetValue(TabOrderInDockModeProperty);
        }

        /// <summary>
        /// Sets the value of the DockedElementTabbedHost.TabOrderInDockMode�attached property to a given DependencyObject.
        /// </summary>
        /// <param name="obj">The element on which to set the DockedElementTabbedHost.TabOrderInDockMode�attached property.</param>
        /// <param name="value">The property value to set.</param>
        public static void SetTabOrderInDockMode(DependencyObject obj, int value)
        {
            obj.SetValue(TabOrderInDockModeProperty, value);
        }

        /// <summary>
        /// Gets the value of the DockedElementTabbedHost.TabOrderInFloatMode�attached property from a given DependencyObject.
        /// </summary>
        /// <param name="obj">The element from which to read the property value.</param>
        /// <returns>The value of the DockedElementTabbedHost.TabOrderInFloatMode�attached property.</returns>
        public static int GetTabOrderInFloatMode(DependencyObject obj)
        {
            return (int)obj.GetValue(TabOrderInFloatModeProperty);
        }

        /// <summary>
        /// Sets the value of the DockedElementTabbedHost.TabOrderInFloatMode�attached property to a given DependencyObject.
        /// </summary>
        /// <param name="obj">The element on which to set the DockedElementTabbedHost.TabOrderInFloatMode�attached property.</param>
        /// <param name="value">The property value to set.</param>
        public static void SetTabOrderInFloatMode(DependencyObject obj, int value)
        {
            obj.SetValue(TabOrderInFloatModeProperty, value);
        }

        /// <summary>
        /// Removes element from its parent to give it ability to be inserted into other container.
        /// </summary>
        /// <param name="element">Element to be removed.</param>
        public static void RemoveElementFromParent(FrameworkElement element)
        {
            FrameworkElement parent = (FrameworkElement)element.Parent;
            IRemoveChild removeChild = parent as IRemoveChild;

            if (removeChild != null)
            {
                removeChild.RemoveChild(element);
            }
            else
            {
                ContentControl contentControl = parent as ContentControl;

                if (contentControl != null)
                {
                    contentControl.Content = null;
                }
            }
        }

        #endregion

        #region Implementation
        /// <summary>
        /// Removes current instance of <see cref="DockedElementTabbedHost"/> from parent.
        /// </summary>
        protected internal void RemoveFromParent()
        {
            RemoveElementFromParent(this);
        }

        /// <summary>
        /// Raises WindowActivated event.
        /// </summary>
        /// <param name="source">The source.</param>
        internal void FireWindowActivated(FrameworkElement source)
        {
            RoutedEventArgs args = new RoutedEventArgs(DockingManager.WindowActivatedEvent, source);
            RaiseEvent(args);
        }

        /// <summary>
        /// Raises WindowDeactivated event.
        /// </summary>
        /// <param name="source">The source.</param>
        internal void FireWindowDeactivated(FrameworkElement source)
        {
            RoutedEventArgs args = new RoutedEventArgs(DockingManager.WindowDeactivatedEvent, source);
            RaiseEvent(args);
        }

        /// <summary>
        /// Resolves the host.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="state">The state.</param>
        /// <returns>return host element.</returns>
        internal static DockedElementTabbedHost ResolveHost(UIElement element, DockState state)
        {
            DockedElementTabbedHost host = null;

            DockInfoInternal info = DockingManager.GetDockInfo(element);

            if (info != null)
            {
                switch (state)
                {
                    case DockState.Dock:
                        {
                            host = info.HostDock;

                            if (host == null)
                            {
                                string targetName = DockingManager.GetTargetNameInDockedMode(element);
                                if (targetName != string.Empty)
                                {
                                    FrameworkElement mainElement = info.DockingManager.FindChild(targetName);
                                    if (mainElement != null)
                                    {
                                        DockInfoInternal mainInfo = DockingManager.GetDockInfo(mainElement);
                                        host = mainInfo.HostDock;
                                    }
                                }
                            }
                        }

                        break;

                    case DockState.Float:
                        {
                            host = info.HostFloat;

                            if (host == null)
                            {
                                FrameworkElement mainElement = null;
                                string targetName = DockingManager.GetTargetNameInFloatingMode(element);

                                if (targetName != string.Empty)
                                {
                                    mainElement = info.DockingManager.FindChild(targetName);
                                }

                                if (mainElement != null)
                                {
                                    DockInfoInternal mainInfo = DockingManager.GetDockInfo(mainElement);
                                    host = mainInfo.HostFloat;
                                }
                            }
                        }

                        break;

                    default:
                        throw new NotSupportedException("ResolveHost: Invalid DockState!");
                }
            }

            return host;
        }

        /// <summary>
        /// Adds the tab.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="bFirst">if set to <c>true</c> [b first].</param>
        internal static void AddTab(FrameworkElement element, bool bFirst)
        {
            DockState state = DockingManager.GetState(element);
            DockedElementTabbedHost host = DockingManager.GetTabbedHost(element, state);
            DockingManager owner = DockingManager.ResolveManager(element);
            if (owner != null && host != null)
            {
                List<FrameworkElement> items = owner.GetHostReferences(host);

                items.Remove(element);
                int elementOrder = DockingManager.GetEdgeTabOrder(items, state, bFirst);

                host.m_bLockedTabOrderChanged = true;
                DockedElementTabbedHost.SetTabOrder(element, state, ++elementOrder);
                host.m_bLockedTabOrderChanged = false;
            }
        }

        /// <summary>
        /// Removes the tab.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="state">The state.</param>
        internal static void RemoveTab(FrameworkElement element, DockState state)
        {
            DockedElementTabbedHost host = DockingManager.GetTabbedHost(element, state);
            DockingManager owner = DockingManager.ResolveManager(element);
            if (owner != null && host != null)
            {
                List<FrameworkElement> items = owner.GetHostReferences(host);
                int elementOrder = DockedElementTabbedHost.GetTabOrder(element, state);

                items.Remove(element);
                host.m_bLockedTabOrderChanged = true;
                owner.m_setinternal = true;
                foreach (FrameworkElement tab in items)
                {
                    int tabOrder = DockedElementTabbedHost.GetTabOrder(tab, state);

                    if (tabOrder > elementOrder)
                    {
                        DockedElementTabbedHost.SetTabOrder(tab, state, tabOrder - 1);
                    }
                }
                owner.m_setinternal = false;


                host.m_bLockedTabOrderChanged = false;
            }
        }

        /// <summary>
        /// Sets the tab order for current Tab control
        /// </summary>
        /// <param name="hostTabItems">The host tab items.</param>
        /// <param name="hostState">State of the host.</param>
        internal static void UpdateTabOrder(ItemCollection hostTabItems, DockState hostState)
        {
            for (int i = 0, cnt = hostTabItems.Count; i < cnt; ++i)
            {
                FrameworkElement item = (FrameworkElement)hostTabItems[i];
                DockedElementTabbedHost.SetTabOrder(item, hostState, i);
            }
        }

        /// <summary>
        /// Sets the selected item.
        /// </summary>
        /// <param name="hostTabControl">The host tab control.</param>
        /// <returns>return result.</returns>
        internal static bool SetSelectedItem(TabControl hostTabControl)
        {
            bool result = false;

            foreach (FrameworkElement item in hostTabControl.Items)
            {
                if (DockingManager.GetIsSelectedTab(item))
                {
                    hostTabControl.SelectedItem = item;
                    result = true;
                    break;
                }
            }

            if (!result && hostTabControl.Items.Count > 1)
            {
                FrameworkElement item = hostTabControl.Items[0] as FrameworkElement;
                hostTabControl.SelectedItem = item;
                DockingManager.SetIsSelectedTab(item, true);
                result = true;
            }

            return result;
        }

        /// <summary>
        /// Sorts the tabs.
        /// </summary>
        /// <param name="host">The docked element tabbed host.</param>
        internal static void SortTabs(DockedElementTabbedHost host)
        {
            
            if (host != null)
            {
                ListCollectionView view = (ListCollectionView)CollectionViewSource.GetDefaultView(host.TabChildren);
                view.CustomSort = new TabChildrenComparer();
                view.Refresh();
            }
        }

        /// <summary>
        /// Selects the tab.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="state">The state.</param>
        internal static void SelectTab(FrameworkElement element, DockState state)
        {
            DockSide side = DockingManager.GetSide(element, state);

            if (side == DockSide.Tabbed)
            {
                string targetName = DockingManager.GetTargetName(element, state);
                DockingManager owner = DockingManager.ResolveManager(element);
                FrameworkElement target = owner.FindChild(targetName);
                if (target != null)
                {
                    List<FrameworkElement> tabs = owner.FindSiblings(target, state, false);
                    tabs.Add(target);

                    foreach (FrameworkElement tab in tabs)
                    {
                        DockingManager.SetIsSelectedTab(tab, false);
                    }

                    DockingManager.SetIsSelectedTab(element, true);
                }
            }
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.PreviewMouseDown"/> attached�routed event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs"/> that contains the event data. The event data reports that one or more mouse buttons were pressed.</param>
        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            if (e.StylusDevice == null || e.StylusDevice != null)
            {
                FrameworkElement element = InternalDataContext;
                DockingManager owner = DockingManager.ResolveManager(element);

                if (element != null && CanSetActiveWindow(element) && owner != null && owner.m_ActiveWindowChangingFlag)
                {
                    if (owner.ActiveWindow != element)
                    {
                        ActiveWindowChangingEventArgs arg = new ActiveWindowChangingEventArgs();
                        arg.OldValue = owner.ActiveWindow;
                        arg.NewValue = element as FrameworkElement;
                        owner.FireActiveWindowChanging(arg.NewValue, arg);
                        if (!arg.Cancel)
                        {
                            m_cancelActiveWindowChange = false;
                            Focus();
                            SetActiveWindow(element);
                            FireWindowActivated(element);
                            FireWindowDeactivated(arg.OldValue);
                        }
                        else
                        {
                            m_cancelActiveWindowChange = true;
                            e.Handled = true;
                        }
                    }
                    base.OnPreviewMouseDown(e);
                }
                else if (owner != null)
                {
                    owner.m_ActiveWindowChangingFlag = true;
                }

                DockingManager.HideSidePanelItems();
            }
        }

        //public bool ActiveFlag = true;
        /// <summary>
        /// Responds to System.Windows.UIElement.MouseDown event. Used to give a focus to the hosted element.
        /// </summary>
        /// <param name="e">Provides data for System.Windows.Input.MouseEventArgs.</param>
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
             base.OnMouseDown(e);
        }

        /// <summary>
        /// Invoked just before the <see cref="System.Windows.UIElement.IsKeyboardFocusWithinChanged"/> event is raised by this element. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">A <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        protected override void OnIsKeyboardFocusWithinChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnIsKeyboardFocusWithinChanged(e);
        }

        /// <summary>
        /// Raises the System.Windows.UIElement.LostFocus�routed event by using the event data that is provided.
        /// <see cref="DockedElementTabbedHost"/> uses LostFocus event to fire WindowDeactivated event.
        /// </summary>
        /// <param name="e">A System.Windows.RoutedEventArgs that contains event data. This event data
        /// must contain the identifier for the System.Windows.UIElement.LostFocus event. </param>
        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            //if (e.StylusDevice == null || e.StylusDevice != null)
            //{
                DockingManager.OnMouseMoveOnHost(this, e);
            //}
            base.OnMouseMove(e);
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            if (e.StylusDevice == null || e.StylusDevice != null)
            {
                DockingManager.OnMouseEnterOnHost(this, e);
            }
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (e.StylusDevice == null || e.StylusDevice != null)
            {
                DockingManager.OnMouseDownOnHost(this, e);
            }
            base.OnMouseLeftButtonDown(e);
        }

        protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (e.Source is DockedElementTabbedHost && e.StylusDevice == null || e.StylusDevice != null)
            {
                DockingManager.OnMouseUpOnHost(this, e);
            }
            base.OnPreviewMouseLeftButtonUp(e);
        }

#if !SyncfusionFramework3_5
        //void m_tabControl_PreviewTouchDown(object sender, TouchEventArgs e)
        //{
        //    if (DockingManager != null && DockingManager.IsTouchEnabled && DockingManager.m_TouchDeviceId == e.TouchDevice.Id)
        //    {
        //        if (DockingManager.m_dockingManagerSystemGesture == SystemGesture.Tap)
        //            m_insertitem = false;
        //    }
        //}

        //private void OnTouchLeftFingerDown(TouchEventArgs e)
        //{
        //    if (DockingManager != null && DockingManager.IsTouchEnabled && DockingManager.m_TouchDeviceId == e.TouchDevice.Id)
        //    {
        //        DockingManager.OnMouseDownOnHost(this, e);
        //    }
        //}

        //private void OnPreviewTouchLeftFingerUp(TouchEventArgs e)
        //{
        //    if (e.Source is DockedElementTabbedHost && DockingManager != null && DockingManager.IsTouchEnabled && DockingManager.m_TouchDeviceId == e.TouchDevice.Id)
        //    {
        //        DockingManager.OnMouseUpOnHost(this, e);
        //    }
        //}

        //protected override void OnTouchEnter(TouchEventArgs e)
        //{
        //    if (DockingManager != null && DockingManager.IsTouchEnabled && DockingManager.m_TouchDeviceId == e.TouchDevice.Id)
        //    {
        //        DockingManager.OnMouseEnterOnHost(this, e);
        //    }
        //    base.OnTouchEnter(e);
        //}

        //protected override void OnTouchDown(TouchEventArgs e)
        //{
        //    if (DockingManager != null && DockingManager.IsTouchEnabled && DockingManager.m_TouchDeviceId == e.TouchDevice.Id)
        //    {
        //        base.OnTouchDown(e);
        //        OnTouchLeftFingerDown(e);
        //    }
        //}

        //protected override void OnTouchMove(TouchEventArgs e)
        //{
        //    if (DockingManager != null && DockingManager.IsTouchEnabled && DockingManager.m_TouchDeviceId == e.TouchDevice.Id)
        //    {
        //        DockingManager.OnMouseMoveOnHost(this, e);
        //    }
        //    base.OnTouchMove(e);
        //}

        //protected override void OnPreviewTouchDown(TouchEventArgs e)
        //{
        //    if (DockingManager != null && DockingManager.IsTouchEnabled && DockingManager.m_TouchDeviceId == e.TouchDevice.Id)
        //    {
        //        FrameworkElement element = InternalDataContext;
        //        DockingManager owner = DockingManager.ResolveManager(element);

        //        if (element != null && CanSetActiveWindow(element) && owner != null && owner.m_ActiveWindowChangingFlag)
        //        {
        //            if (owner.ActiveWindow != element)
        //            {
        //                ActiveWindowChangingEventArgs arg = new ActiveWindowChangingEventArgs();
        //                arg.OldValue = owner.ActiveWindow;
        //                arg.NewValue = element as FrameworkElement;
        //                owner.FireActiveWindowChanging(arg.NewValue, arg);
        //                if (!arg.Cancel)
        //                {
        //                    m_cancelActiveWindowChange = false;
        //                    Focus();
        //                    SetActiveWindow(element);
        //                    FireWindowActivated(element);
        //                    FireWindowDeactivated(arg.OldValue);
        //                }
        //                else
        //                {
        //                    m_cancelActiveWindowChange = true;
        //                    e.Handled = true;
        //                }
        //            }
        //            base.OnPreviewTouchDown(e);
        //        }
        //        else if (owner != null)
        //        {
        //            owner.m_ActiveWindowChangingFlag = true;
        //        }

        //        DockingManager.HideSidePanelItems();
        //    }
        //}

        //protected override void OnPreviewTouchUp(TouchEventArgs e)
        //{
        //    if (DockingManager != null && DockingManager.IsTouchEnabled && DockingManager.m_TouchDeviceId == e.TouchDevice.Id)
        //    {
        //        if (DockingManager.m_dockingManagerSystemGesture == SystemGesture.Tap)
        //            OnPreviewTouchLeftFingerUp(e);
        //    }
        //    base.OnPreviewTouchUp(e);
        //}

#endif
        /// <summary>
        /// This method keeps focus if content element FocusedProperty is set to false.
        /// </summary>
        /// <param name="e">The instance containing the event data.</param>
        /// <property name="flag" value="Finished"/>
        protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            base.OnLostKeyboardFocus(e);
            TabItem item = e.NewFocus as TabItem;

            if (null != item && item.Name.StartsWith(PrefixName))
            {
                if (!DockingManager.isSidePanelClicked)
                {
                    Focus();
                }
                else
                {
                    DockingManager.isSidePanelClicked = false;
                }
            }           
        }
        
        /// <summary>
        /// Invoked when an unhandled <see cref="System.Windows.Input.Keyboard.PreviewGotKeyboardFocus"/>�attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.KeyboardFocusChangedEventArgs"/> that contains the event data.</param>
        protected override void OnPreviewGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {          
            base.OnPreviewGotKeyboardFocus(e);
            FrameworkElement gotFocusedelement = e.OriginalSource as FrameworkElement;
            if(m_cancelActiveWindowChange)
                e.Handled = true;       
        }
              

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Keyboard.PreviewKeyDown"/>�attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.KeyboardFocusChangedEventArgs"/> that contains the event data.</param>
        protected override void OnPreviewLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            base.OnPreviewLostKeyboardFocus(e);
            FrameworkElement lostedFocusElement = e.OriginalSource as FrameworkElement;

            if ((lostedFocusElement as DependencyObject) != null)
            {
                if (DockingManager.GetDockInfo(lostedFocusElement as DependencyObject) != null && (DockingManager.GetFloatWindow(lostedFocusElement)) != null)
                {
                    DockingManager dockingmanager = DockingManager.ResolveManager(lostedFocusElement as UIElement);
                    if (dockingmanager!=null && DockingManager.GetState(lostedFocusElement as DependencyObject) != DockState.Float)
                    {
                        dockingmanager.RemoveWindow(DockingManager.GetFloatWindow(lostedFocusElement));
                    }
                }
            }
        }

        /// <summary>
        /// Initializes data-bindings for the properties.
        /// </summary>
        /// <param name="e">The instance containing the event data.</param>
        /// <property name="flag" value="Finished"/>
        protected override void OnInitialized(EventArgs e)
        {
            GotFocus += new RoutedEventHandler(ChildGotFocus);
            LostFocus += new RoutedEventHandler(ChildLostFocus);
            base.OnInitialized(e);
            InternalDataContext = (TabChildren.Count == 0) ? null : InternalDataContext;
            FrameworkElement element = (FrameworkElement)InternalDataContext;

            if (null != element)
            {
                DockingManager owner = DockingManager.ResolveManager(element);

                if (null != owner)
                {
                    BindingUtils.SetBinding(this, owner, DockedElementTabbedHost.StyleProperty, DockingManager.DockedElementTabbedHostStyleProperty);
                }
                else
                {
#if DevCode
					Debugger.Break();
#endif
                }
            }
        }

        /// <summary>
        /// Measures the size in layout required for child elements and
        /// determines a size for the <see cref="DockedElementTabbedHost"/> and derived class.
        /// </summary>
        /// <param name="constraint">The maximum size that the method can return.</param>
        /// <returns>
        /// The size that this element determines it needs during layout, based on its
        /// calculations of child element sizes.
        /// </returns>
        protected override Size MeasureOverride(Size constraint)
        {
            base.MeasureOverride(constraint);
            return new Size(0, 0);
        }

        /// <summary>
        /// Updates property value cache and raises StateChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value
        /// and new value.</param>
        /// <property name="flag" value="Finished"/>
        protected virtual void OnStateChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        /// <summary>
        /// Updates property value cache and raises HostedElementChanged
        /// event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        /// <property name="flag" value="Finished"/>
        protected virtual void OnHostedElementChanged(DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement newElement = (FrameworkElement)e.NewValue;
            if (newElement != null)
            {
                Binding binding = new Binding("FocusVisualStyle");
                binding.Source = DockingManager.ResolveManager(newElement);
                if (binding.Source != null)
                {
                    newElement.SetBinding(FocusVisualStyleProperty, binding);
                }
            }
        }

        /// <summary>
        /// Coerces hosted element to assure that it's visible.
        /// </summary>
        /// <param name="baseValue">Hosted element.</param>
        /// <returns>
        /// BaseValue if it's acceptable, otherwise null.
        /// </returns>
        protected virtual object CoerceHostedElement(object baseValue)
        {
            object result = (Visibility == Visibility.Collapsed && !ShowTabs) ? null : baseValue;
            return result;
        }



        internal static void SwapElement(TabControl tab, FrameworkElement element, FrameworkElement target, DockState state)
        {
            if (element != target && target!=null && element!=null)
            {
                List<FrameworkElement> siblings = new List<FrameworkElement>();
                DockSide targetSide = DockingManager.GetSideSafe(target, state);
                DockSide elementSide = DockingManager.GetSideSafe(element, state);
                string targetParentName = DockingManager.GetTargetNameSafe(target, state);
                string elementName = element.Name;
                // DockingManager.SetPreviousTargetInDockMode(target, targetParentName);
                for (int i = 0; i < tab.Items.Count; i++)
                {
                    FrameworkElement tabElement = tab.Items[i] as FrameworkElement;
                    siblings.Add(tabElement);
                }


                foreach (FrameworkElement sibling in siblings)
                {
                    DockSide siblingSide = DockingManager.GetSideSafe(sibling, state);
                    if ((siblingSide == DockSide.Tabbed || siblingSide != DockSide.Tabbed) && ((sibling.Name != element.Name) && (sibling.Name != target.Name)))
                    {

                        DockingManager.SetTargetNameSafe(sibling, elementName, state);
                        DockingManager.SetTargetNameInFloatingMode(sibling, elementName);
                        if (state == DockState.Dock)
                        {
                            DockingManager.SetPreviousTargetInDockMode(sibling, target.Name);
                            DockingManager.SetDesiredHeightInFloatingMode(sibling, DockingManager.GetDesiredHeightInFloatingMode(target));
                            DockingManager.SetDesiredWidthInFloatingMode(sibling, DockingManager.GetDesiredWidthInFloatingMode(target));
                            DockingManager.SetDesiredWidthInDockedMode(sibling, DockingManager.GetDesiredWidthInDockedMode(target));
                            DockingManager.SetDesiredHeightInDockedMode(sibling, DockingManager.GetDesiredHeightInDockedMode(target));
                            DockInfoInternal info = DockingManager.GetDockInfo(sibling);

                        }
                    }
                }
                DockingManager.SetDesiredWidthInDockedMode(element, DockingManager.GetDesiredWidthInDockedMode(target));
                DockingManager.SetDesiredHeightInDockedMode(element, DockingManager.GetDesiredHeightInDockedMode(target));
                if (DockingManager.IsVisibleState(DockingManager.GetState(element))&& DockingManager.IsVisibleState(DockingManager.GetState(target)))
                {
                    DockingManager.SetDesiredHeight(element, DockingManager.GetState(element), DockingManager.GetDesiredHeight(target, DockingManager.GetState(target)));
                    DockingManager.SetDesiredWidth(element, DockingManager.GetState(element), DockingManager.GetDesiredWidth(target, DockingManager.GetState(target)));
                }
                //DockingManager.SetPreviousSideInDockMode(target, DockingManager.GetSideInDockedMode(target));
                //DockingManager.SetPreviousTargetInDockMode(target, DockingManager.GetTargetNameInDockedMode(target));
                DockingManager.SetTargetNameSafe(target, elementName, state);
                DockingManager.SetTargetNameInFloatingMode(target, elementName);
                DockingManager.SetSideSafe(target, DockSide.Tabbed, state);
                //DockingManager.SetTargetNameSafe(element, string.Empty, state);
                DockingManager.SetTargetNameInFloatingMode(element, DockingManager.GetTargetNameSafe(element as DependencyObject,state));
                DockingManager.SetTabParent(element, elementName);
                //DockingManager.SetPreviousSideInDockMode(element, DockingManager.GetSideInDockedMode(element));
                //DockingManager.SetSideSafe(element, targetSide, state);
            }
        }
        private bool CheckDockingManager(FrameworkElement target, FrameworkElement element)
        {
            bool _flag = element != null && DockingManager.ResolveManager(element) != null
                       && DockingManager.ResolveManager(element).Children.Contains(element)
                       && target!=null && DockingManager.ResolveManager(target as UIElement)!=null && DockingManager.ResolveManager(target as UIElement).Equals(DockingManager.ResolveManager(element as UIElement));
            return _flag;
        }
        bool m_Activeflag = true;
        /// <summary>
        /// Called when [child tab control selection changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void OnChildTabControlSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.OriginalSource is TabControl  && e.OriginalSource==sender)
            {
                DockingManager.updatedockflag = false;
                TabControl tabControl = (TabControl)sender;
                if (e.AddedItems.Count > 0)
                {
                    FrameworkElement element = e.AddedItems[0] as FrameworkElement;
                    FrameworkElement target = m_firstelement;
                    if (e.RemovedItems.Count > 0)
                    {
                        target = e.RemovedItems[0] as FrameworkElement;
                    }
                    if (CheckDockingManager(target, element))
                    {
                        DockState targetstate = DockingManager.GetState(target as DependencyObject);
                        DockState elementstate = DockingManager.GetState(element as DependencyObject);
                        if ((targetstate == DockState.Dock || targetstate == DockState.Float)
                            && (elementstate == DockState.Dock || elementstate == DockState.Float))
                        {
                            if (DockingManager.GetTargetName(target as DependencyObject, targetstate) != element.Name)
                            {
                                if (elementstate.Equals(targetstate))
                                {
                                    if (elementstate == DockState.Dock)
                                    {
                                        DockingManager.SetPreviousSideInDockMode(element as DependencyObject, DockingManager.GetSideInDockedMode(element as DependencyObject));
                                        DockingManager.SetPreviousTargetInDockMode(element as DependencyObject, DockingManager.GetTargetNameInDockedMode(element as DependencyObject));
                                        DockingManager.SetDesiredWidthInDockedMode(element as DependencyObject, DockingManager.GetDesiredWidthInDockedMode(target as DependencyObject));
                                        DockingManager.SetDesiredHeightInDockedMode(element as DependencyObject, DockingManager.GetDesiredHeightInDockedMode(target as DependencyObject));
                                    }
                                    if (element == target)
                                    {
                                        DockingManager.SetTargetNameSafe(element as DependencyObject, "", elementstate);
                                    }
                                    else
                                    {
                                        string name = DockingManager.GetTargetName(target as DependencyObject, targetstate);
                                        if (name != string.Empty)
                                        {
                                            FrameworkElement felement = DockingManager.ResolveManager(element as UIElement).FindChild(name);
                                            if (felement != null)
                                            {
                                                DockingManager.SetTargetNameSafe(element as DependencyObject, name, elementstate);
                                            }
                                        }
                                        else
                                        {
                                            DockingManager.SetTargetNameSafe(element as DependencyObject, "", elementstate);
                                        }
                                    }
                                    DockingManager.SetSideSafe(element as DependencyObject, DockingManager.GetSideSafe(target as DependencyObject, targetstate), targetstate);
                                }
                                else
                                {
                                    if (DockingManager.GetIsSelectedTab(element as DependencyObject))
                                    {
                                        if (DockingManager.GetState(element as DependencyObject) == DockState.Dock)
                                        {
                                            DockingManager.SetPreviousTargetInDockMode(element as DependencyObject, DockingManager.GetTargetNameInDockedMode(element as DependencyObject));
                                        }
                                        DockingManager.SetTargetNameSafe(element as DependencyObject, "", elementstate);
                                    }
                                }
                            }
                            if (element != target)
                            {
                                if (DockingManager.GetState(target as DependencyObject) == DockState.Dock)
                                {
                                    DockingManager.SetPreviousSideInDockMode(target as DependencyObject, DockingManager.GetSideInDockedMode(target as DependencyObject));
                                    DockingManager.SetPreviousTargetInDockMode(target as DependencyObject, DockingManager.GetTargetNameInDockedMode(target as DependencyObject));
                                }
                                DockingManager.SetTargetNameSafe(target as DependencyObject, element.Name, elementstate);
                                DockingManager.SetSideSafe(target as DependencyObject, DockSide.Tabbed, targetstate);
                            }
                        }
                        if (!m_insertitem)
                        {
                            SwapElement(tabControl, element, target, DockingManager.GetState(target as DependencyObject));
                        }
                        if (m_insertitem)
                        {
                            m_insertitem = false;
                        }
                        m_firstelement = element;
                        TabParent = element;

                    }
                    if (element != null)
                    {
                        if (tabControl.Items.Contains(element))
                        {
                        DockingManager.SetPreviousTargetInDockMode(element as DependencyObject, DockingManager.GetTargetNameInDockedMode(element as DependencyObject));
                        foreach (var item in tabControl.Items)
                        {
                            if (item != element)
                            {
                                DockingManager.SetPreviousTargetInDockMode(item as DependencyObject, DockingManager.GetTargetNameInDockedMode(item as DependencyObject));
                                DockingManager.SetTargetNameInDockedMode(item as DependencyObject, element.Name);
                                DockingManager.SetSideInDockedMode(item as DependencyObject, DockSide.Tabbed);
                            }
                        }
                      }
                    }
                }

                if (m_Activeflag)
                {
                    if (null != tabControl.SelectedItem)
                    {
                        InternalDataContext = tabControl.SelectedItem as FrameworkElement;

                        if (this.State == DockState.Float && !DockingManager.UseNativeFloatWindow)
                        {
                            FloatWindow FltWindow = DockingManager.GetFloatWindow(tabControl.SelectedItem as FrameworkElement) as FloatWindow;
                            if (FltWindow != null)
                            {
                                FloatWindowBorder FltWidowBorder = FltWindow.Header as FloatWindowBorder;
                                if (FltWidowBorder != null)
                                {
                                    FltWidowBorder.ToEnableorDisableCloseButton(tabControl.SelectedItem as FrameworkElement);
                                    if (FltWidowBorder.CloseButton != null && (tabControl.SelectedItem as DependencyObject) != null)
                                    {
                                        DockingManager owner2 = DockingManager.ResolveManager(tabControl.SelectedItem as UIElement);
                                        if (owner2 != null)
                                        {
                                            bool close = DockingManager.GetCanClose(tabControl.SelectedItem as DependencyObject);
                                            if (!close)
                                            {
                                                switch (owner2.DisabledCloseButtonsBehavior)
                                                {
                                                    case DisabledButtonsBehavior.Collapse:
                                                        FltWidowBorder.CloseButton.Visibility = Visibility.Collapsed;
                                                        break;
                                                    case DisabledButtonsBehavior.Hide:
                                                        FltWidowBorder.CloseButton.Visibility = Visibility.Hidden;
                                                        break;
                                                    case DisabledButtonsBehavior.Disable:
                                                        FltWidowBorder.CloseButton.Visibility = Visibility.Visible;
                                                        break;
                                                }
                                            }
                                            else
                                            {
                                                FltWidowBorder.CloseButton.Visibility = Visibility.Visible;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else if (HostedElement != null)
                    {

                        InternalDataContext = HostedElement;

                        if (this.State == DockState.Float && !DockingManager.UseNativeFloatWindow)
                        {
                            FloatWindow FltWindow = DockingManager.GetFloatWindow(HostedElement as FrameworkElement) as FloatWindow;
                            if (FltWindow != null)
                            {
                                FloatWindowBorder FltWidowBorder = FltWindow.Header as FloatWindowBorder;
                                if (FltWidowBorder != null)
                                {
                                    FltWidowBorder.ToEnableorDisableCloseButton(HostedElement as FrameworkElement);
                                    if (FltWidowBorder.CloseButton != null && (HostedElement as DependencyObject) != null)
                                    {
                                        DockingManager owner1 = DockingManager.ResolveManager(HostedElement as UIElement);
                                        if (owner1 != null)
                                        {
                                            bool close = DockingManager.GetCanClose(HostedElement as DependencyObject);
                                            if (!close)
                                            {
                                                switch (owner1.DisabledCloseButtonsBehavior)
                                                {
                                                    case DisabledButtonsBehavior.Collapse:
                                                        FltWidowBorder.CloseButton.Visibility = Visibility.Collapsed;
                                                        break;
                                                    case DisabledButtonsBehavior.Hide:
                                                        FltWidowBorder.CloseButton.Visibility = Visibility.Hidden;
                                                        break;
                                                    case DisabledButtonsBehavior.Disable:
                                                        FltWidowBorder.CloseButton.Visibility = Visibility.Visible;
                                                        break;
                                                }
                                            }
                                            else
                                            {
                                                FltWidowBorder.CloseButton.Visibility = Visibility.Visible;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        DockingManager manager = DockingManager.ResolveManager(HostedElement);
                        ActiveWindowChangingEventArgs args = new ActiveWindowChangingEventArgs();
                        args.OldValue = manager.ActiveWindow;
                        args.NewValue = HostedElement;
                        if (args.OldValue != args.NewValue)
                        {
                            manager.FireActiveWindowChanging(HostedElement, args);
                            if (!args.Cancel)
                            {
                                DockingManager.SetNewFocusedElement(HostedElement);
                                Focus();
                            }
                            else
                            {
                                m_Activeflag = false;
                                tabControl.SelectedItem = e.RemovedItems[0];
                                m_Activeflag = true;
                            }
                        }
                        else
                        {
                            DockingManager.SetNewFocusedElement(HostedElement);
                            Focus();
                        }
                    }



                    m_tabControl = m_tabControl ?? tabControl;
                    DockingManager owner = DockingManager.ResolveManager(HostedElement);

                    if (null != m_tabControl.SelectedItem && null != owner && owner.IsNonInternalChange)
                    {
                        FrameworkElement selectedItem = (FrameworkElement)m_tabControl.SelectedItem;
                        ActiveWindowChangingEventArgs args = new ActiveWindowChangingEventArgs();
                        args.OldValue = owner.ActiveWindow;
                        args.NewValue = selectedItem;
                        if (args.OldValue != args.NewValue)
                        {
                            owner.FireActiveWindowChanging(selectedItem, args);
                            if (!args.Cancel)
                            {
                                HostedElement = selectedItem;
                                SelectTab(selectedItem);
                            }
                            else
                            {
                                m_Activeflag = false;
                                if (e.RemovedItems.Count > 0)
                                {
                                    tabControl.SelectedItem = e.RemovedItems[0];
                                    InternalDataContext = e.RemovedItems[0] as FrameworkElement;
                                }
                                m_Activeflag = true;
                            }
                        }
                        else
                        {
                            HostedElement = selectedItem;
                            SelectTab(selectedItem);
                        }
                    }
                }
                else
                {
                    m_Activeflag = true;
                }

                DockHeaderPresenter presenter = VisualUtils.FindDescendant((Visual)this, typeof(DockHeaderPresenter)) as DockHeaderPresenter;
                if (presenter != null && InternalDataContext != null)
                {
                    presenter.UpdateHeaderBindings(InternalDataContext);
                }
            }
            
        }

        /// <summary>
        /// Called when TabChildrenCollection was changed.
        /// In CloseAll mode disabled close buttons will not display if there is no enabled close buttons in tab group.
        /// In AutoHideGroup mode disabled auto hide buttons will not display if there is no auto hide buttons in tab group.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        private void OnTabChildrenCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                bool m_settaborder = true;
                int count = 0;
                for (int i = 0; i < TabChildren.Count; i++)
                {
                    if (DockedElementTabbedHost.GetTabOrder(TabChildren[i] as DependencyObject, DockingManager.GetState(TabChildren[i] as DependencyObject)) == 0)
                    {
                        count++;
                    }
                }
                if (count <= 1)
                {
                    m_settaborder = false;
                }
                if (m_settaborder && DockingManager.ResolveManager(e.NewItems[0] as UIElement) != null)
                {
                    DockedElementTabbedHost.SetTabOrder(e.NewItems[0] as DependencyObject, DockingManager.GetState(e.NewItems[0] as DependencyObject), TabChildren.Count - 1);
                }
            }

            if (m_doubleclick && e.Action == NotifyCollectionChangedAction.Add)
            {
                SelectTab(e.NewItems[0] as FrameworkElement);
            }

            if (TabChildren.Count > 0) 
            {
                if (TabParent != null)
                {
                    if (!TabChildren.Contains(TabParent))
                    {
                        TabParent = TabChildren[0];
                    }
                }
                else
                {
                    TabParent = TabChildren[0];
                }
            }

            if ((e.Action == NotifyCollectionChangedAction.Remove) && DockingManager.IsDragging)
            {
                foreach (FrameworkElement element in TabChildren)
                {

                    if (element != TabChildren[0])
                    {
                        DockingManager.updatedockflag = false;
                        DockingManager.SetTargetNameInDockedMode(element, TabChildren[0].Name);
                        DockingManager.SetTargetInDockModeTab(element, TabChildren[0].Name);
                    }
                }               
            }
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                TabControl tabControl = (TabControl)InternalTabControl;
                if (e.OldItems.Count > 0)
                {
                    FrameworkElement target = e.OldItems[0] as FrameworkElement;
                    if (target != null && tabControl != null && tabControl.Items.Count > 0)
                    {
                        if (DockingManager.GetState(target as DependencyObject) == DockState.Dock)
                        {
                            foreach (FrameworkElement tab in tabControl.Items)
                            {
                                if (DockingManager.GetTargetNameInDockedMode(target as DependencyObject) != string.Empty)
                                {
                                    if (DockingManager.GetTargetNameInDockedMode(target as DependencyObject) == tab.Name)
                                    {
                                        if (DockingManager.GetIsSelectedTab(target as DependencyObject))
                                        {
                                            DockingManager.SetTargetNameInDockedMode(target as DependencyObject, "");
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
           

            HandleTabChildrenCollectionChanged();

            if (NotifyCollectionChangedAction.Reset == e.Action)
            {
                CanAutoHideGroup = false;
                CanCloseGroup = false;
            }
            else if (NotifyCollectionChangedAction.Add == e.Action
                || NotifyCollectionChangedAction.Remove == e.Action)
            {
                if (!CanAutoHideGroup)
                {
                    foreach (FrameworkElement item in TabChildren)
                    {
                        if (DockingManager.GetCanAutoHide(item))
                        {
                            CanAutoHideGroup = true;
                            break;
                        }
                    }
                }

                if (!CanCloseGroup)
                {
                    foreach (FrameworkElement item in TabChildren)
                    {
                        if (DockingManager.GetCanClose(item))
                        {
                            CanCloseGroup = true;
                            break;
                        }
                    }
                }
            }
            else
            {
#if DevCode
				Debugger.Break();
#endif
            }
        }

        /// <summary>
        /// Processes LostFocus events of the children. Sets IsChildFocused to false.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void ChildLostFocus(object sender, RoutedEventArgs e)
        {
            IsChildFocused = false;
        }

        /// <summary>
        /// Processes GotFocus events of the children. Sets IsChildFocused to true.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void ChildGotFocus(object sender, RoutedEventArgs e)
        {
            IsChildFocused = true;
        }

        /// <summary>
        /// Coerces the hosted element.
        /// </summary>
        /// <param name="d">The dependency object.</param>
        /// <param name="baseValue">The base value.</param>
        /// <returns>return host object.</returns>
        private static object CoerceHostedElement(DependencyObject d, object baseValue)
        {
            DockedElementTabbedHost host = (DockedElementTabbedHost)d;

            return host.CoerceHostedElement(baseValue);
        }

        /// <summary>
        /// Calls OnStateChanged method of the <see cref="DockedElementTabbedHost"/> instance, notifies of the
        /// dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        /// <property name="flag" value="Finished"/>
        private static void OnStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DockedElementTabbedHost instance = (DockedElementTabbedHost)d;
            instance.OnStateChanged(e);
        }

        /// <summary>
        /// Calls OnHostedElementChanged method of the <see cref="DockedElementTabbedHost"/> instance, notifies
        /// of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        /// <property name="flag" value="Finished"/>
        private static void OnHostedElementChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DockedElementTabbedHost instance = (DockedElementTabbedHost)d;
            instance.OnHostedElementChanged(e);
        }

        /// <summary>
        /// Shows the tabs changed.
        /// </summary>
        /// <param name="d">The dependency object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void ShowTabsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            d.CoerceValue(HostedElementProperty);
        }

        /// <summary>
        /// Called when [tab order in dock mode changed].
        /// </summary>
        /// <param name="d">The dependency object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnTabOrderInDockModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DockedElementTabbedHost.ResetTabOrder(d,e, DockState.Dock);
        }

        /// <summary>
        /// Called when [tab order in float mode changed].
        /// </summary>
        /// <param name="d">The dependency object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnTabOrderInFloatModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DockedElementTabbedHost.ResetTabOrder(d, e, DockState.Float);
        }

        /// <summary>
        /// Updates the tab order.
        /// </summary>
        /// <param name="d">The dependency object.</param>
        /// <param name="state">The state.</param>
        private static void UpdateTabOrder(DependencyObject d, DockState state)
        {
            DockInfoInternal info = DockingManager.GetDockInfo(d);

            if (info != null)
            {
                UIElement uiHost = (UIElement)d;
                DockedElementTabbedHost host = DockedElementTabbedHost.ResolveHost(uiHost, state);

                if (host != null)
                {
                    if (!host.m_isTabOrderUpdateStarted && !host.m_bLockedTabOrderChanged)
                    {
                        ThreadStart method = delegate
                        {
                            ListCollectionView view = (ListCollectionView)CollectionViewSource.GetDefaultView(host.TabChildren);
                            view.CustomSort = new TabChildrenComparer();
                            view.Refresh();
                            host.m_isTabOrderUpdateStarted = false;
                        };

                        host.m_isTabOrderUpdateStarted = true;
                        host.Dispatcher.BeginInvoke(DispatcherPriority.Send, method);
                    }
                }
                ////else
                ////    throw new ArgumentException( "Incorect element for prepare. It must owned float window" );
            }
            FrameworkElement selectedtab = null;
            TabControl tabcontrol = DockingManager.GetTabControl(d as FrameworkElement);
            if (tabcontrol != null && tabcontrol.Items.Count>0)
            {
                foreach (FrameworkElement tab in tabcontrol.Items)
                {
                    if (DockingManager.GetIsSelectedTab(tab as DependencyObject))
                    {
                        selectedtab = tab;
                    }
                }
                foreach (FrameworkElement tab in tabcontrol.Items)
                {
                    if (selectedtab == null)
                    {
                        selectedtab = tabcontrol.Items[0] as FrameworkElement;
                        DockingManager.SetIsSelectedTab(tabcontrol.Items[0] as DependencyObject, true);
                    }
                    else
                    {
                        if (!DockingManager.GetIsSelectedTab(tab as DependencyObject))
                        {
                            DockingManager.SetTargetNameSafe(tab as DependencyObject, selectedtab.Name, DockingManager.GetState(tab as DependencyObject));
                            DockingManager.SetSideSafe(tab as DependencyObject, DockSide.Tabbed, DockingManager.GetState(tab as DependencyObject));
                        }
                    }
                }
            }
        }

        private static void ResetTabOrder(DependencyObject d, DependencyPropertyChangedEventArgs e, DockState state)
        {
            int newvalue = (e.NewValue != null) ? (int)e.NewValue : 0;
            int oldvalue = (e.OldValue != null) ? (int)e.OldValue : 0;
            DockInfoInternal info = DockingManager.GetDockInfo(d);

            if (info != null)
            {
                UIElement uiHost = (UIElement)d;
                DockedElementTabbedHost host = DockedElementTabbedHost.ResolveHost(uiHost, state);

                if (host != null && !host.DockingManager.m_setinternal)
                {
                    if (DockedElementTabbedHost.GetTabOrder(d, state) > host.TabChildren.Count)
                    {
                        DockedElementTabbedHost.SetTabOrder(d, state, host.TabChildren.Count);
                    }
                    foreach (FrameworkElement element in host.TabChildren)
                    {
                        DockState elementstate = DockingManager.GetState(element);
                        if (DockedElementTabbedHost.GetTabOrder(element, elementstate) == newvalue && element != (d as FrameworkElement))
                        {
                            int index = host.TabChildren.IndexOf(element);
                            int taborder = DockedElementTabbedHost.GetTabOrder(element, elementstate);
                            SetTabOrder(element, state, taborder + 1);
                        }
                    }
                    UpdateTabOrder(d, state);
                }
                else if (host != null)
                {
                     if (!host.m_isTabOrderUpdateStarted && !host.m_bLockedTabOrderChanged)
                     {
                            ThreadStart method = delegate
                            {
                                ListCollectionView view = (ListCollectionView)CollectionViewSource.GetDefaultView(host.TabChildren);
                                view.CustomSort = new TabChildrenComparer();
                                view.Refresh();
                                host.m_isTabOrderUpdateStarted = false;
                            };

                            host.m_isTabOrderUpdateStarted = true;
                            host.Dispatcher.BeginInvoke(DispatcherPriority.Send, method);
                     }
                }
                
             
            }
        }

        /// <summary>
        /// Sets the active window.
        /// </summary>
        /// <param name="element">The element.</param>
        private static void SetActiveWindow(FrameworkElement element)
        {
            DockingManager owner = DockingManager.ResolveManager(element);

            if (null != owner)
            {
                owner.ActiveWindow = element;
            }
        }

        /// <summary>
        /// Determines whether this instance [can set active window] the specified element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>
        /// <c>true</c> if this instance [can set active window] the specified element; otherwise, <c>false</c>.
        /// </returns>
        private static bool CanSetActiveWindow(FrameworkElement element)
        {
            DockingManager owner = DockingManager.ResolveManager(element);

            if (null != owner)
            {
                ContentControl content = element as ContentControl;

                if (null != content && null != content.Content && content.Content.Equals(owner.DocContainer))
                {
                    return false;
                }
            }

            return true;
        }
        #endregion

        #region Dependency properties
        /// <summary>
        /// Identifies <see cref="DockedElementTabbedHost"/> ShowTabs dependency property.
        /// </summary>
        public static readonly DependencyProperty ShowTabsProperty =
            DependencyProperty.Register("ShowTabs", typeof(bool), typeof(DockedElementTabbedHost), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender, ShowTabsChanged));

        /// <summary>
        /// Identifies <see cref="DockedElementTabbedHost"/> TabOrderInDockMode attached property.
        /// </summary>
        /// <remarks>
        /// This property can be attached to docking manager child and is used for ordering the tabs in 
        /// <see cref="DockedElementTabbedHost"/> instance in docked state.
        /// The default number is 0.
        /// </remarks>         
        public static readonly DependencyProperty TabOrderInDockModeProperty =
            DependencyProperty.RegisterAttached("TabOrderInDockMode", typeof(int), typeof(DockedElementTabbedHost), new FrameworkPropertyMetadata(0, OnTabOrderInDockModeChanged));

        /// <summary>
        /// Identifies <see cref="DockedElementTabbedHost"/> TabOrderInFloatMode attached property.
        /// </summary>
        /// <remarks>
        /// This property can be attached to docking manager child and is used for ordering the tabs in 
        /// <see cref="DockedElementTabbedHost"/> instance in floating state.
        /// The default number is 0.
        /// </remarks>
        public static readonly DependencyProperty TabOrderInFloatModeProperty =
            DependencyProperty.RegisterAttached("TabOrderInFloatMode", typeof(int), typeof(DockedElementTabbedHost), new FrameworkPropertyMetadata(0, OnTabOrderInFloatModeChanged));

        /// <summary>
        /// Identifies <see cref="DockedElementTabbedHost"/> PreviousHostWidth attached property.
        /// </summary>
        internal static readonly DependencyProperty PreviousHostWidthProperty =
    DependencyProperty.RegisterAttached("PreviousHostWidth", typeof(double), typeof(DockedElementTabbedHost), new FrameworkPropertyMetadata(90d));

        /// <summary>
        /// Identifies <see cref="DockedElementTabbedHost"/> PreviousHostHeight attached property.
        /// </summary>
        internal static readonly DependencyProperty PreviousHostHeightProperty =
    DependencyProperty.RegisterAttached("PreviousHostHeight", typeof(double), typeof(DockedElementTabbedHost), new FrameworkPropertyMetadata(90d));

        /// <summary>
        /// DependencyPropertyKey for tab children collection
        /// </summary>
        protected static readonly DependencyPropertyKey TabChildrenPropertyKey =
            DependencyProperty.RegisterReadOnly("TabChildren", typeof(ObservableFrameworkElements), typeof(DockedElementTabbedHost), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// This DependencyProperty contains tab children collection.
        /// </summary>
        public static readonly DependencyProperty TabChildrenProperty = TabChildrenPropertyKey.DependencyProperty;

        /// <summary>
        /// Identifies <see cref="DockedElementTabbedHost"/> CanAutoHideGroup dependency property.
        /// </summary>
        public static readonly DependencyProperty CanAutoHideGroupProperty
            = DependencyProperty.Register("CanAutoHideGroup", typeof(bool), typeof(DockedElementTabbedHost), new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Identifies <see cref="DockedElementTabbedHost"/> CanCloseGroup dependency property.
        /// </summary>
        public static readonly DependencyProperty CanCloseGroupProperty
            = DependencyProperty.Register("CanCloseGroup", typeof(bool), typeof(DockedElementTabbedHost), new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Identifies the  MarkAsFrozen Dependency property
        /// </summary>
        public static readonly DependencyProperty MarkAsFrozenProperty =
            DependencyProperty.Register("MarkAsFrozen", typeof(bool), typeof(DockedElementTabbedHost), new UIPropertyMetadata(false));

        /// <summary>
        /// Identifies <see cref="DockedElementTabbedHost"/> State dependency property.
        /// </summary>
        public static readonly DependencyProperty StateProperty =
            DependencyProperty.Register("State", typeof(DockState), typeof(DockedElementTabbedHost), new FrameworkPropertyMetadata(DockState.Dock, new PropertyChangedCallback(OnStateChanged)));

        /// <summary>        
        /// Identifies <see cref="DockedElementTabbedHost"/> HostedElement dependency property.
        /// </summary>
        public static readonly DependencyProperty HostedElementProperty =
            DependencyProperty.Register("HostedElement", typeof(FrameworkElement), typeof(DockedElementTabbedHost), new FrameworkPropertyMetadata(null, OnHostedElementChanged, CoerceHostedElement));

        /// <summary>
        /// Key for the dependency property that specifies whether one of
        /// the children is focused.
        /// </summary>
        protected static readonly DependencyPropertyKey IsChildFocusedPropertyKey =
            DependencyProperty.RegisterReadOnly("IsChildFocused", typeof(bool), typeof(DockedElementTabbedHost), new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Read-only dependency property that specifies whether one of
        /// the children is focused.
        /// </summary>
        public static readonly DependencyProperty IsChildFocusedProperty = IsChildFocusedPropertyKey.DependencyProperty;

        /// <summary>
        /// Identifies the Docking Manager Property
        /// </summary>
        public static readonly DependencyProperty DockingManagerProperty =
            DependencyProperty.Register("DockingManager", typeof(DockingManager), typeof(DockedElementTabbedHost));
        #endregion

        #region Alternative methods
        /// <summary>
        /// Handles the tab children collection changed.
        /// </summary>
        private void HandleTabChildrenCollectionChanged()
        {
            DockedElementsContainer parent = Parent as DockedElementsContainer;

            switch (TabChildren.Count)
            {
                case 0:
                    Visibility = Visibility.Collapsed;
                    InternalDataContext = null;
                    HostedElement = null;
                    ShowTabs = false;
                    break;

                case 1:
                    Visibility = Visibility.Visible;
                    InternalDataContext = TabChildren[0];
                    HostedElement = TabChildren[0];
                    ShowTabs = false;
                    break;

                default:
                    ShowTabs = true;
                    break;
            }

            if (parent != null)
            {
                parent.CheckVisibility();
                if (InternalDataContext != null && !DockingManager.IsVisibleDockWindowState(DockingManager.GetDockWindowState(InternalDataContext)))
                    DockedElementsContainer.EnableMaxMinButtonVisibility(parent);
            }
        }

        /// <summary>
        /// Selects the tab.
        /// </summary>
        /// <param name="element">The element.</param>
        internal void SelectTab(FrameworkElement element)
        {
            foreach (FrameworkElement tab in TabChildren)
            {
                DockingManager.SetIsSelectedTab(tab, false);
            }

            DockingManager.SetIsSelectedTab(element, true);

            if (InternalTabControl != null)
            {
                InternalTabControl.SelectedItem = element;
            }

            ////HostedElement = element;
            InternalDataContext = element;
            ActiveWindowChangingEventArgs args = new ActiveWindowChangingEventArgs();
            args.NewValue = element;
            args.OldValue = DockingManager.ActiveWindow;
            if (args.OldValue != args.NewValue)
            {
                DockingManager.FireActiveWindowChanging(element, args);
                if (!args.Cancel)
                {
                    DockingManager.SetNewFocusedElement(element);
                }
            }
            else
                DockingManager.SetNewFocusedElement(element);
            Focus();
        }

        /// <summary>
        /// Inits the size of the desired.
        /// </summary>

        internal void InitDesiredSize()
        {
            FrameworkElement context = InternalDataContext;

            if ((context != null) && !DockingManager.GetDockToFill(context))
            {
                switch (State)
                {
                    case DockState.Dock:
                        double minwidth = DockingManager.GetDesiredMinWidthInDockedMode(context);
                        double maxwidth = DockingManager.GetDesiredMaxWidthInDockedMode(context);
                        double minHeight = DockingManager.GetDesiredMinHeightInDockedMode(context);
                        double maxHeight = DockingManager.GetDesiredMaxHeightInDockedMode(context);
                        double width = DockingManager.GetDesiredWidthInDockedMode(context);
                        double height=0;
                        if(HostedElement!=null)
                        height = DockingManager.GetDesiredHeightInDockedMode(HostedElement);
                        if (width < minwidth)
                        {
                            if (minwidth > 0)
                            {
                                width = minwidth;
                            }
                        }
                        if (width > maxwidth)
                        {
                            if (maxwidth > 0)
                            {
                                width = maxwidth;
                            }
                        }
                        if (height < minHeight)
                        {
                            if (minHeight > 0)
                            {
                                height = minHeight;
                            }
                        }
                        if (height > maxHeight)
                        {
                            if (maxHeight > 0)
                            {
                                height = maxHeight;
                            }
                        }
                        m_desiredSize = new Size(width,height);
                        break;

                    case DockState.Float:
                        double minwidthfloat = DockingManager.GetDesiredMinWidthInFloatingMode(context);
                        double maxwidthfloat = DockingManager.GetDesiredMaxWidthInFloatingMode(context);
                        double minheightfloat = DockingManager.GetDesiredMinHeightInFloatingMode(context);
                        double maxheightfloat = DockingManager.GetDesiredMaxHeightInFloatingMode(context);
                        double widthfloat = DockingManager.GetDesiredWidthInFloatingMode(context);
                        double heightfloat = DockingManager.GetDesiredHeightInFloatingMode(context);
                        if (widthfloat < minwidthfloat)
                        {
                            if (minwidthfloat > 0)
                            {
                                widthfloat = minwidthfloat;
                            }
                        }
                        if (widthfloat > maxwidthfloat)
                        {
                            if (maxwidthfloat > 0)
                            {
                                widthfloat = maxwidthfloat;
                            }
                        }
                        if (heightfloat < minheightfloat)
                        {
                            if (minheightfloat > 0)
                            {
                                heightfloat = minheightfloat;
                            }
                        }
                        if (heightfloat > maxheightfloat)
                        {
                            if (maxheightfloat > 0)
                            {
                                heightfloat = maxheightfloat;
                            }
                        }
                        m_desiredSize = new Size(widthfloat, heightfloat);
                        break;
                }
            }
            else
            {
                m_desiredSize = new Size(0, 0);
            }
        }

        /// <summary>
        /// Sets the style binding.
        /// </summary>
        /// <param name="host">The docked element tabbed host.</param>
        /// <param name="owner">The owner.</param>
        internal static void SetStyleBinding(DockedElementTabbedHost host, DockingManager owner)
        {
            BindingUtils.SetBinding(host, owner, DockedElementTabbedHost.StyleProperty, DockingManager.DockedElementTabbedHostStyleProperty);
        }

        /// <summary>
        /// Resets this instance.
        /// </summary>
        internal void Reset()
        {
            TabChildren.Clear();
            m_desiredSize = Size.Empty;
        }
        #endregion
    }
}