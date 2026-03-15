// <copyright file="Docking.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using Syncfusion.Licensing;
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents Docking Class.
    /// </summary>

    public partial class DockingManager
    {
        #region Constants

        /// <summary>
        /// Represents the Mouse enter flag
        /// </summary>
        //SU I78477
        //bool MouseEnter = false;
        new bool MouseEnter = false;
        //EU I78477
        /// <summary>
        /// It represents TAB_PANEL_NAME.
        /// </summary>        
        private const string TAB_PANEL_NAME = "PART_TabPanel";

        /// <summary>
        /// It represents TAG_INTERNAL_TAB_ITEM.
        /// </summary>
        private const string TAG_INTERNAL_TAB_ITEM = "IsInternalTabItem";

        /// <summary>
        /// Contains minimum height of dragged element in docked mode 
        /// (may be set after dragging if height of dragged element is too low).
        /// </summary>
        private const double MIN_HEIGHT_IN_DOCKEDMODE = 60;
        #endregion

        #region Private member

        internal bool m_setinternal = false;

        internal bool m_fromNative = false;

        private bool m_HeaderPanelCallback = false;

        internal MouseEventArgs m_HeaderPanelMouseEventArgs = null;

        /// <summary>
        /// stores mouse move event args
        /// </summary>
        internal InputEventArgs m_mouseeventargs = null;

        /// <summary>
        /// stores mouse up event args
        /// </summary>
        internal InputEventArgs m_mousebuttonargs = null;

        /// <summary>
        /// It represents m_FrozeElements.
        /// </summary>
        private readonly List<FrameworkElement> m_FrozeElements = new List<FrameworkElement>();

        /// <summary>
        /// It represents m_frozeHosts.
        /// </summary>
        private List<DockedElementTabbedHost> m_frozeHosts = new List<DockedElementTabbedHost>();

        /// <summary>
        /// This list contains elements for dragging.
        /// <remarks>First element is main, others are his tab siblings.</remarks>
        /// </summary>
        internal FrameworkElement m_draggedElement;

        /// <summary>
        /// represents whether mouse is moved on tdilayout panel
        /// </summary>
        internal bool m_mousemoveontdilayoutpanel = false;

        /// <summary>
        /// represents whether mouse is moved on header panel
        /// </summary>
        internal bool m_mousemoveonheaderpanel = false;

        /// <summary>
        /// represents whether mouse is moved on Tabpaneladv
        /// </summary>
        internal bool m_mousemoveonTabpaneladv = false;

        /// <summary>
        /// represents the index of the item under mouse moved on header panel 
        /// </summary>
        internal int m_mouseonheaderpanelindex = -1;

        /// <summary>
        /// represents the width of the tabbed item under mouse moved on header panel 
        /// </summary>
        internal double m_mouseonheaderpaneltabareawidth = 0;

        /// <summary>
        /// represents the position under mouse moved on header panel 
        /// </summary>
        internal double m_mouseonheaderpanelposition = 0.0;

        /// <summary>
        /// Contains host element under dragging element. 
        /// </summary>
        internal DockedElementTabbedHost m_hostUnderMouse;

        /// <summary>
        /// Contains previous element under dragging element. 
        /// </summary>
        private DockedElementTabbedHost m_prevHostUnderMouse;

        /// <summary>
        /// determines whether placement rectangle for Floating window is set while dragging.
        /// </summary>
        internal bool m_placementrectset = false;

        /// <summary>
        /// Contains a list of dragged tabs host.
        /// </summary>
        internal List<FrameworkElement> m_holdList = null;

        internal static List<FrameworkElement> m_holdTabItemExtList = null;

        /// <summary>
        /// It represents swap directions.
        /// </summary>
        private int m_swapDirection = -1;

        /// <summary>
        /// Indicated whether the Tab is pressed or not.
        /// </summary>
        private bool m_isTabPressed;

        /// <summary>
        /// This member indicate dragging type.
        /// </summary>
        private bool m_isPointKnown = false;

        /// <summary>
        /// Indicated the mouse start position.
        /// </summary>
        private Point m_pointMouseStartPos = new Point(double.NegativeInfinity, double.NegativeInfinity);

        /// <summary>
        /// Indicates the mouse start position offset respective to FloatWindow
        /// </summary>
        internal Point m_pointMouseStartPosOffset = new Point(double.NegativeInfinity, double.PositiveInfinity);

        /// <summary>
        /// Used for saving floating window rect coordinates when ends dragging tab element.
        /// </summary>
        private Rect m_floatWindowRectCoord;

        /// <summary>
        /// Saves rectangle of the floating window for GenerateFloatingWindows method 
        /// when floating window is created from tab.
        /// </summary>
        private static Rect m_floatWindowRect = Rect.Empty;

        /// <summary>
        /// Saves rectangle of the dragged floating window while dragging through the captions of tabbed group.
        /// </summary>
        private Rect m_rectBeforeAddingTab = Rect.Empty;

        /// <summary>
        /// Contains true if element is dragging and false when element stops dragging.
        /// </summary>
        internal bool m_firedDragStart = false;

        /// <summary>
        /// Contains counter for dragging in mouseMove method.
        /// </summary>
        internal int m_dragCounter = 0;

        /// <summary>
        /// Indicates load preview.
        /// </summary>
        private bool m_bLoadPreview;

        /// <summary>
        /// Indicates TouchDevice Id.
        /// </summary>
        internal int m_TouchDeviceId = -1;

#if !SyncfusionFramework3_5
        internal TouchDevice m_TouchDevice;
#endif

        internal SystemGesture m_dockingManagerSystemGesture;

        /// <summary>
        /// Indicated locked side changed.
        /// </summary>
        //SU I78477
        //private bool m_bLockedSideChanged;
        private bool m_bLockedSideChanged=false;
        //EU I78477


        internal static bool isSidePanelClicked = false;

        private List<HitTestResult> testresult;

        #endregion

        #region Public method
        /// <summary>
        /// Gets value of the ListenTabItemEvents property
        /// </summary>
        /// <param name="obj">DependencyObject that contains this property</param>
        /// <returns>bool value of the property</returns>
        public static bool GetListenTabItemEvents(DependencyObject obj)
        {
            return (bool)obj.GetValue(ListenTabItemEventsProperty);
        }

        /// <summary>
        /// Sets value of the ListenTabItemEvents property
        /// </summary>
        /// <param name="obj">DependencyObject that contains this property</param>
        /// <param name="value">value to set to the property</param>
        public static void SetListenTabItemEvents(DependencyObject obj, bool value)
        {
            obj.SetValue(ListenTabItemEventsProperty, value);
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether this instance is V S2010 dragging enabled.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is V S2010 dragging enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsVS2010DraggingEnabled
        {
            get
            {
                return (bool)GetValue(IsVS2010DraggingEnabledProperty);
            }

            set
            {
                SetValue(IsVS2010DraggingEnabledProperty, value);
            }
        }



        internal bool IsTouchEnabled
        {
            get { return (bool)GetValue(IsTouchEnabledProperty); }
            set { SetValue(IsTouchEnabledProperty, value); }
        }

        /// <summary>
        /// Gets or sets CenterDragProvider of the <see cref="DockingManager"/>. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="ControlTemplate"/>
        /// Provides CenterDragProvider value for the <see cref="DockingManager"/>.
        /// </value>
        /// <remarks>
        /// Center drag provider is a center button appearing when you are dragging some window.
        /// You can override default template of the CenterDragProvider. 
        /// </remarks>
        /// <example>
        /// <para/>This example shows how to set new CenterDragProvider template value in C#.
        /// <code language="C#">
        /// dockingManager.CenterDragProvider = (ControlTemplate)FindResource( "CenterButtonCustomTemplate" );
        /// </code>
        /// <para/>This example shows how to override default template of the CenterDragProvider in XAML.
        /// <code language="XAML">
        /// <![CDATA[
        /// <ControlTemplate x:Key="CenterButtonCustomTemplate" TargetType="{x:Type ContentControl}">
        /// <Grid>
        /// <Grid.RowDefinitions>
        /// <RowDefinition Height="30" />
        /// <RowDefinition Height="30" />
        /// <RowDefinition Height="30" />
        /// </Grid.RowDefinitions>
        /// <Grid.ColumnDefinitions>
        /// <ColumnDefinition Width="30" />
        /// <ColumnDefinition Width="30" />
        /// <ColumnDefinition Width="30" />
        /// </Grid.ColumnDefinitions>
        /// <Image Name="TopImg" Grid.Column="1" Grid.Row="0" Width="27" Height="27" Syncfusion:DockPreviewManagerVS2005.ProviderAction="Top" Source="Images\CustomDragProvider.png" />
        /// <Image Name="LeftImg" Grid.Column="0" Grid.Row="1" Width="27" Height="27" Syncfusion:DockPreviewManagerVS2005.ProviderAction="Left" Source="Images\CustomDragProvider.png" />
        /// <Image Name="CenterImg" Grid.Column="1" Grid.Row="1" Width="27" Height="27" Syncfusion:DockPreviewManagerVS2005.ProviderAction="Center" Source="Images\CustomDragProvider.png" />
        /// <Image Name="RightImg" Grid.Column="2" Grid.Row="1" Width="27" Height="27" Syncfusion:DockPreviewManagerVS2005.ProviderAction="Right" Source="Images\CustomDragProvider.png" />
        /// <Image Name="BottomImg" Grid.Column="1" Grid.Row="2" Width="27" Height="27" Syncfusion:DockPreviewManagerVS2005.ProviderAction="Bottom" Source="Images\CustomDragProvider.png" />
        /// </Grid>
        /// </ControlTemplate>
        /// ]]>
        /// </code>
        /// </example>
        /// <seealso cref="ControlTemplate"/>
        public ControlTemplate CenterDragProvider
        {
            get
            {
                return (ControlTemplate)GetValue(CenterDragProviderProperty);
            }

            set
            {
                SetValue(CenterDragProviderProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets LeftDragProvider of the <see cref="DockingManager"/>. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="ControlTemplate"/>
        /// Provides LeftDragProvider value for the <see cref="DockingManager"/>.
        /// </value>
        /// <remarks>
        /// Left drag provider is a left button appearing when you are dragging some window.
        /// Using left drag button you can dock dragging window to the left side of the parent.
        /// You can override default template of the LeftDragProvider.
        /// </remarks>
        /// <example>
        /// <para/>This example shows how to set new LeftDragProvider template value in C#.
        /// <code language="C#">
        /// dockingManager.LeftDragProvider = ( ControlTemplate )FindResource( "LeftButtonCustomTemplate" );
        /// </code>
        /// <para/>This example shows how to override default template of the LeftDragProvider in XAML.
        /// <code language="XAML">
        /// <![CDATA[
        /// <ControlTemplate x:Key="LeftButtonCustomTemplate" TargetType="{x:Type ContentControl}">
        /// <Image Name="Img" Width="27" Height="27" Syncfusion:DockPreviewManagerVS2005.ProviderAction="GlobalLeft" Source="Images\CustomDragProvider.png" />
        /// <ControlTemplate.Triggers>
        /// <DataTrigger Binding="{Binding Path=IsSideButtonActive, RelativeSource={RelativeSource FindAncestor, AncestorType={x:Type Syncfusion:DockPreviewMainButtonVS2005}}}" Value="true">
        /// <Setter TargetName="Img" Property="Source" Value="Images\CustomDragProviderOver.png"/>
        /// </DataTrigger>
        /// </ControlTemplate.Triggers>
        /// </ControlTemplate>
        /// ]]>
        /// </code>
        /// </example>
        /// <seealso cref="ControlTemplate"/>
        public ControlTemplate LeftDragProvider
        {
            get
            {
                return (ControlTemplate)GetValue(LeftDragProviderProperty);
            }

            set
            {
                SetValue(LeftDragProviderProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets TopDragProvider of the <see cref="DockingManager"/>. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="ControlTemplate"/>
        /// Provides TopDragProvider value for the <see cref="DockingManager"/>.
        /// </value>
        /// <remarks>
        /// Top drag provider is a top button appearing when you are dragging some window.
        /// Using top drag button you can dock dragging window to the top side of the parent.
        /// You can override default template of the TopDragProvider.
        /// </remarks>
        /// <example>
        /// <para/>This example shows how to set new TopDragProvider template value in C#.
        /// <code language="C#">
        /// dockingManager.TopDragProvider = (ControlTemplate)FindResource( "TopButtonCustomTemplate" );
        /// </code>
        /// <para/>This example shows how to override default template of the TopDragProvider in XAML.
        /// <code language="XAML">
        /// <![CDATA[
        /// <ControlTemplate x:Key="TopButtonCustomTemplate" TargetType="{x:Type ContentControl}">
        /// <Image Name="Img" Width="27" Height="27" Syncfusion:DockPreviewManagerVS2005.ProviderAction="GlobalTop" Source="Images\CustomDragProvider.png" />
        /// <ControlTemplate.Triggers>
        /// <DataTrigger Binding="{Binding Path=IsSideButtonActive, RelativeSource={RelativeSource FindAncestor, AncestorType={x:Type Syncfusion:DockPreviewMainButtonVS2005}}}" Value="true">
        /// <Setter TargetName="Img" Property="Source" Value="Images\CustomDragProviderOver.png"/>
        /// </DataTrigger>
        /// </ControlTemplate.Triggers>
        /// </ControlTemplate>
        /// ]]>
        /// </code>
        /// </example>
        /// <seealso cref="ControlTemplate"/>
        public ControlTemplate TopDragProvider
        {
            get
            {
                return (ControlTemplate)GetValue(TopDragProviderProperty);
            }

            set
            {
                SetValue(TopDragProviderProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets RightDragProvider of the <see cref="DockingManager"/>. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="ControlTemplate"/>
        /// Provides RightDragProvider value for the <see cref="DockingManager"/>.
        /// </value>
        /// <remarks>
        /// Right drag provider is a right button appearing when you are dragging some window.
        /// Using right drag button you can dock dragging window to the right side of the parent.
        /// You can override default template of the RightDragProvider.
        /// </remarks>
        /// <example>
        /// <para/>This example shows how to set new RightDragProvider template value in C#.
        /// <code language="C#">
        /// dockingManager.RightDragProvider = (ControlTemplate)FindResource( "RightButtonCustomTemplate" );
        /// </code>
        /// <para/>This example shows how to override default template of the RightDragProvider in XAML.
        /// <code language="XAML">
        /// <![CDATA[
        /// <ControlTemplate x:Key="RightButtonCustomTemplate" TargetType="{x:Type ContentControl}">
        /// <Image Name="Img" Width="27" Height="27" Syncfusion:DockPreviewManagerVS2005.ProviderAction="GlobalRight" Source="Images\CustomDragProvider.png" />
        /// <ControlTemplate.Triggers>
        /// <DataTrigger Binding="{Binding Path=IsSideButtonActive, RelativeSource={RelativeSource FindAncestor, AncestorType={x:Type Syncfusion:DockPreviewMainButtonVS2005}}}" Value="true">
        /// <Setter TargetName="Img" Property="Source" Value="Images\CustomDragProviderOver.png"/>
        /// </DataTrigger>
        /// </ControlTemplate.Triggers>
        /// </ControlTemplate>
        /// ]]>
        /// </code>
        /// </example>
        /// <seealso cref="ControlTemplate"/>
        public ControlTemplate RightDragProvider
        {
            get
            {
                return (ControlTemplate)GetValue(RightDragProviderProperty);
            }

            set
            {
                SetValue(RightDragProviderProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets BottomDragProvider of the <see cref="DockingManager"/>. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="ControlTemplate"/>
        /// Provides BottomDragProvider value for the <see cref="DockingManager"/>.
        /// </value>
        /// <remarks>
        /// Bottom drag provider is a bottom button appearing when you are dragging some window.
        /// Using bottom drag button you can dock dragging window to the bottom side of the parent.
        /// You can override default template of the BottomDragProvider.
        /// </remarks>
        /// <example>
        /// <para/>This example shows how to set new BottomDragProvider template value in C#.
        /// <code language="C#">
        /// dockingManager.BottomDragProvider = (ControlTemplate)FindResource( "BottomButtonCustomTemplate" );
        /// </code>
        /// <para/>This example shows how to override default template of the BottomDragProvider in XAML.
        /// <code language="XAML">
        /// <![CDATA[
        /// <ControlTemplate x:Key="BottomButtonCustomTemplate" TargetType="{x:Type ContentControl}">
        /// <Image Name="Img" Width="27" Height="27" Syncfusion:DockPreviewManagerVS2005.ProviderAction="GlobalBottom" Source="Images\CustomDragProvider.png" />
        /// <ControlTemplate.Triggers>
        /// <DataTrigger Binding="{Binding Path=IsSideButtonActive, RelativeSource={RelativeSource FindAncestor, AncestorType={x:Type Syncfusion:DockPreviewMainButtonVS2005}}}" Value="true">
        /// <Setter TargetName="Img" Property="Source" Value="Images\CustomDragProviderOver.png"/>
        /// </DataTrigger>
        /// </ControlTemplate.Triggers>
        /// </ControlTemplate>
        /// ]]>
        /// </code>
        /// </example>
        /// <seealso cref="ControlTemplate"/>
        public ControlTemplate BottomDragProvider
        {
            get
            {
                return (ControlTemplate)GetValue(BottomDragProviderProperty);
            }

            set
            {
                SetValue(BottomDragProviderProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets DraggingType of the <see cref="DockingManager"/>. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="DraggingType"/>
        /// Provides DraggingType value for the <see cref="DockingManager"/>.
        /// </value>
        /// <remarks>
        /// DraggingType can be normal, border or shadow. Default value of the DraggingType property is NormalDragging.
        /// </remarks>
        /// <example>
        /// <para/>This example shows how to set DraggingType in C#.
        /// <code language="C#">
        /// dockingManager.DraggingType = DraggingType.BorderDragging;
        /// </code>
        /// </example>
        /// <seealso cref="DraggingType"/>
        public DraggingType DraggingType
        {
            get
            {
                return (DraggingType)GetValue(DraggingTypeProperty);
            }

            set
            {
                SetValue(DraggingTypeProperty, value);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is dragging.
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

            internal set
            {
                if (UseInteropCompatibilityMode)
                {
                    if (value)
                    {
                        SetFakes();
                    }
                    else
                    {
                        RemoveFakes();
                    }
                }

                ProcessHitTestOnFloatOnlyWindows(value);

                SetValue(IsDraggingProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [restrict window minimum size].
        /// </summary>
        /// <value>
        /// <c>true</c> if [restrict window minimum size]; otherwise, <c>false</c>.
        /// </value>
        public bool RestrictWindowMinimumSize
        {
            get
            {
                return (bool)GetValue(RestrictWindowMinimumSizeProperty);
            }

            set
            {
                SetValue(RestrictWindowMinimumSizeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets host element under the dragging element. 
        /// </summary>
        /// <value>
        /// Type: <see cref="DockedElementTabbedHost"/>
        /// Provides HostUnderMouse value for the <see cref="DockingManager"/>.
        /// </value>
        /// <seealso cref="DockedElementTabbedHost"/>
        public DockedElementTabbedHost HostUnderMouse
        {
            get
            {
                return m_hostUnderMouse;
            }

            set
            {
                m_hostUnderMouse = value;
            }
        }
        #endregion

        #region Implemantation

        /// <summary>
        /// Gets the target element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="elementState">State of the element.</param>
        /// <returns>return target</returns>
        internal static FrameworkElement GetTargetElement(FrameworkElement element, DockState elementState)
        {
            FrameworkElement target = null;

            DockingManager owner = DockingManager.ResolveManager(element);
            string targetName = DockingManager.GetTargetName(element, elementState);

            if (!string.IsNullOrEmpty(targetName))
            {
                target = owner.FindChild(targetName);
            }

            return target;
        }

        /// <summary>
        /// Called when [direct tab panel mouse up].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        internal void OnDirectTabPanelMouseUp(object sender, MouseEventArgs e)
        {
            if (e.StylusDevice == null || e.StylusDevice != null)
            {
                DirectTabPanel panel = (DirectTabPanel)sender;
                if (this.Dispatcher.CheckAccess())
                {
                    if (IsDockTabPanel(panel))
                    {
                        if (m_bLoadPreview)
                        {
                            if (m_prevHostUnderMouse != null)
                            {
                                m_prevHostUnderMouse.TabChildren.RemoveRange(m_previewElements);
                                m_prevHostUnderMouse.m_previewedElements.Clear();
                            }
                            m_previewElements.Clear();
                            if (m_draggedElement != null && m_prevHostUnderMouse != null && (DockingManager.GetDockAbility(m_draggedElement) == DockAbility.All || DockingManager.GetDockAbility(m_draggedElement) == DockAbility.Tabbed))
                            {
                                InsertIntoContainer(m_draggedElement, m_prevHostUnderMouse.HostedElement, DockingManager.GetState(m_draggedElement), m_prevHostUnderMouse.State, DockSide.Tabbed, 0);
                            }
                        }

                        m_bLoadPreview = false;
                        m_isTabPressed = false;
                        panel.ReleaseMouseCapture();
                    }
                }
            }
        }
#if !SyncfusionFramework3_5
        /// <summary>
        /// Called when [direct tab panel mouse up].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        //internal void OnDirectTabPanelTouchUp(object sender, TouchEventArgs e)
        //{
        //    DirectTabPanel panel = (DirectTabPanel)sender;
        //    if (this.Dispatcher.CheckAccess())
        //    {
        //        if (IsDockTabPanel(panel))
        //        {
        //            if (m_bLoadPreview)
        //            {
        //                if (m_prevHostUnderMouse != null)
        //                {
        //                    m_prevHostUnderMouse.TabChildren.RemoveRange(m_previewElements);
        //                    m_prevHostUnderMouse.m_previewedElements.Clear();
        //                }
        //                m_previewElements.Clear();
        //                if (m_draggedElement != null && m_prevHostUnderMouse != null && (DockingManager.GetDockAbility(m_draggedElement) == DockAbility.All || DockingManager.GetDockAbility(m_draggedElement) == DockAbility.Tabbed))
        //                {
        //                    InsertIntoContainer(m_draggedElement, m_prevHostUnderMouse.HostedElement, DockingManager.GetState(m_draggedElement), m_prevHostUnderMouse.State, DockSide.Tabbed, 0);
        //                }
        //            }

        //            m_bLoadPreview = false;
        //            m_isTabPressed = false;
        //            panel.ReleaseTouchCapture(e.TouchDevice);
        //        }
        //    }
        //}
#endif
        /// <summary>
        /// Releases the point mouse start pos.
        /// </summary>
        internal void ReleasePointMouseStartPos()
        {
            m_pointMouseStartPos = new Point(double.NegativeInfinity, double.NegativeInfinity);
        }

        internal void StartDraggingNativeWindow(NativeFloatWindow window)
        {
            if (!IsDragging)
            {
                if (window != null && window.PrimaryElement != null && (window.PrimaryElement as DependencyObject) != null)
                {
                    m_draggedElement = window.PrimaryElement;
                    m_DraggingSource = DraggingSource.Window;
                    IsDragging = true;

                window.IsDragging = true;
                window.HitTestDisabled = true;

                if (DraggingType == DraggingType.NormalDragging)
                {
                    StartNormalDragging(window);
                    }  
                }
                MouseMove += OnDockingManagerMouseMove;
            }
        }
        /// <summary>
        /// Starts the dragging window.
        /// </summary>
        /// <param name="window">The window.</param>
        internal void StartDraggingWindow(IWindow window)
        {
            if (Mouse.Capture(this, CaptureMode.SubTree))
            {
                if (!IsDragging)
                {
                    if (window!=null && window.PrimaryElement != null && (window.PrimaryElement as DependencyObject) != null)
                    {
                        m_draggedElement = window.PrimaryElement;
                        m_DraggingSource = DraggingSource.Window;
                        IsDragging = true;

                        window.IsDragging = true;
                        window.HitTestDisabled = true;

                        if (UseAdornerFloatWindow)
                        {
                            m_pointMouseStartPos = Mouse.GetPosition(window as AdornerFloatWindow);
                            m_isPointKnown = true;
                        }

                        if (window.AllowsTransparency && IsntFrozenChild())
                        {
                            window.Opacity = DockingManager.GetNoDock(window.PrimaryElement) ? 1.0 : 0.7;
                        }

                        if (DraggingType == DraggingType.NormalDragging)
                        {
                            StartNormalDragging(window);
                        }
                        else
                        {
                            StartLightDragging(window);
                        }

                        MouseMove += OnDockingManagerMouseMove;
                    }
                }
            }
            ////else
            ////Debug.WriteLine( "Can not capture mouse to start dragging element." );
        }

        /// <summary>
        /// Shows the dock preview internal.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        internal void ShowDockPreviewInternal(InputEventArgs e)
        {
            Point point = new Point();
            if(e is MouseEventArgs)
                point = (e as MouseEventArgs).GetPosition(m_hostUnderMouse);
#if !SyncfusionFramework3_5
            else if (e is TouchEventArgs)
                point = (e as TouchEventArgs).GetTouchPoint(m_hostUnderMouse).Position;
#endif
            DockPreviewRecord? record = m_managerDragPreview.FindDockingPlace(m_draggedElement, m_hostUnderMouse, point);

            if (record != null)
            {
                if (record.Value.State == DockState.Dock
                || (record.Value.State == DockState.Float))
                {
                    FireDockProviderShownEvent(m_hostUnderMouse);
                    m_managerDragPreview.ShowDockPreview(record.Value);
                }
            }
        }

        /// <summary>
        /// Shows the dock preview internal.
        /// </summary>
        /// <param name="window">The window.</param>
        /// <returns>Return bool value.</returns>
        internal bool ShowDockPreviewInternal(IWindow window)
        {
            DockInfoInternal info = DockingManager.GetDockInfo(m_draggedElement);
            bool bShowPreview = false;

            if (window != info.FloatingWindow && !DockingManager.GetNoDock(m_draggedElement))
            {
                if (m_prevHostUnderMouse != null)
                {
                    m_managerDragPreview.HideDockPreviewMainButton(false);
                    m_managerDragPreview.HideDockPreview();
                    m_prevHostUnderMouse = null;
                }
                else
                {
                    m_hostUnderMouse = window.InternalDataContext as DockedElementTabbedHost;
                    DockPreviewRecord record = GetTabbedPrevievRecord();
                    FireDockProviderShownEvent(m_hostUnderMouse);
                    DockAbility ability = DockingManager.GetDockAbility(m_hostUnderMouse.InternalDataContext);
                    if (AllowDock(ability, record.Side) || ability == DockAbility.All)
                    {
                        m_managerDragPreview.ShowDockPreview(record);
                        bShowPreview = true;
                    }
                }
            }

            return bShowPreview;
        }

        /// <summary>
        /// Raises the <see cref="E:MouseUpInernal"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.InputEventArgs"/> instance containing the event data.</param>
        internal void OnMouseUpInernal(InputEventArgs e)
        {
            if (IsDragging)
            {
                DoDocking(e);
            }
        }

        /// <summary>
        /// Swaps the element and target internal.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="target">The target.</param>
        /// <param name="state">The state.</param>
        /// <param name="owner">The owner.</param>
        /// <param name="bSwapTabs">if set to <c>true</c> [b swap tabs].</param>
        /// <param name="bSwapSiblings">if set to <c>true</c> [b swap siblings].</param>
        internal static void SwapElementAndTargetInternal(FrameworkElement element, FrameworkElement target, DockState state, DockingManager owner, bool bSwapTabs, bool bSwapSiblings)
        {
            List<FrameworkElement> siblings = owner.FindSiblingsSafe(target, state);
            DockSide targetSide = DockingManager.GetSideSafe(target, state);
            DockSide elementSide = DockingManager.GetSideSafe(element, state);
            string targetParentName = DockingManager.GetTargetNameSafe(target, state);
            string elementName = element.Name;
            DockingManager.SetPreviousTargetInDockMode(target, targetParentName);
            updatedockflag = false;

            foreach (FrameworkElement sibling in siblings)
            {
                DockSide siblingSide = DockingManager.GetSideSafe(sibling, state);
                if ((bSwapTabs && siblingSide == DockSide.Tabbed) || (bSwapSiblings && siblingSide != DockSide.Tabbed))
                {
                    bool canexecute = true;

                    DockedElementsContainer elementcontainer = VisualUtils.FindAncestor((Visual)DockingManager.GetTabbedHost(element, state), typeof(DockedElementsContainer)) as DockedElementsContainer;
                    DockedElementsContainer targetcontainer = VisualUtils.FindAncestor((Visual)DockingManager.GetTabbedHost(sibling, state), typeof(DockedElementsContainer)) as DockedElementsContainer;
                    if (elementcontainer != null && targetcontainer != null && !elementcontainer.Equals(targetcontainer))
                    {
                        Orientation elementorientation = IsHorizontalDockSide(DockingManager.GetSideInDockedMode(element)) ? Orientation.Horizontal : Orientation.Vertical;
                        string parentname = DockingManager.GetTargetNameInDockedMode(sibling);
                        FrameworkElement parent = owner.FindChild(parentname);
                        DockedElementsContainer parentcontainer = VisualUtils.FindAncestor((Visual)DockingManager.GetTabbedHost(parent, state), typeof(DockedElementsContainer)) as DockedElementsContainer;
                        if (parent != null)
                        {
                            Orientation parentorientation = parentcontainer == null ? IsHorizontalDockSide(DockingManager.GetSideInDockedMode(parent)) ? Orientation.Horizontal : Orientation.Vertical : parentcontainer.Orientation;

                            if (!parentorientation.Equals(elementorientation) && owner.GetContainerLevel(targetcontainer, elementcontainer) >= 0)
                            {
                                DockState parentstate = parent.Equals(target) ? DockState.Float : DockingManager.GetState(parent);
                                if (parent != null && parentstate == DockState.Float && parentstate.Equals(DockingManager.GetState(sibling)))
                                    canexecute = false;
                            }
                        }
                    } 

                    if (canexecute && !sibling.Equals(target))
                    {
                        if (sibling.Name.Equals(elementName))
                            DockingManager.SetTargetNameSafe(sibling, "", state);
                        else
                            DockingManager.SetTargetNameSafe(sibling, elementName, state);

                        if (!sibling.Name.Equals(element.Name))
                            owner.CheckInnerDockIndex(sibling, elementName, true, -1);

                        if (state == DockState.Dock)
                        {
                            DockingManager.SetPreviousTargetInDockMode(sibling, target.Name);
                        }
                    }
                }
            }

            int index = -1;

            if (!String.IsNullOrEmpty(targetParentName))
            {
                List<string> elementsname = owner.InnerDockElements[targetParentName] as List<string>;
				if (elementsname != null && elementsname.Count > 0) 
                    index = elementsname.IndexOf(target.Name);
            }

            DockingManager.SetPreviousSideInDockMode(target, DockingManager.GetSideInDockedMode(target));
            DockingManager.SetPreviousTargetInDockMode(target, DockingManager.GetTargetNameInDockedMode(target));
            DockingManager.SetTargetNameSafe(target, elementName, state);
            DockingManager.SetSideSafe(target, GetOpositeSide(elementSide), state);
            DockingManager.SetTargetNameSafe(element, targetParentName, state);
            DockingManager.SetPreviousSideInDockMode(element, DockingManager.GetSideInDockedMode(element));
            DockingManager.SetSideSafe(element, targetSide, state);

            if (!target.Name.Equals(elementName))
                owner.CheckInnerDockIndex(target, elementName, true, -1);

            if (!targetParentName.Equals(element.Name))
                owner.CheckInnerDockIndex(element, targetParentName, true, index);
        }

        /// <summary>
        /// Sets the target for unhidden element.
        /// </summary>
        /// <param name="element">The element.</param>
        internal static void SetTargetForUnhidedElement(FrameworkElement element)
        {
            DockingManager owner = DockingManager.ResolveManager(element);

            if (null != owner)
            {
                string targetName = DockingManager.GetTargetNameInDockedMode(element);

                if (!string.IsNullOrEmpty(targetName))
                {
                    FrameworkElement targetElement = owner.FindChild(targetName);
                    if (targetElement != null)
                    {
                        if (DockState.Dock != DockingManager.GetState(targetElement))
                        {
                            if (DockSide.Tabbed == DockingManager.GetSideInDockedMode(element))
                            {
                                FrameworkElement parent = DockingManager.GetTargetElement(targetElement, DockState.Dock);

                                if (parent != null)
                                {
                                    SwapElementAndTargetInternal(element, targetElement, DockState.Dock, owner, true, false);

                                    if (targetElement != null && DockState.Dock != DockingManager.GetState(parent))
                                    {
                                        SwapElementAndTargetInternal(element, parent, DockState.Dock, owner, false, true);
                                    }
                                }
                                else
                                {
                                    SwapElementAndTargetInternal(element, targetElement, DockState.Dock, owner, true, false);
                                }
                            }
                            else
                            {
                                DockingManager.RestoreElement(element, DockState.Dock, owner);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Starts the dragging.
        /// </summary>
        /// <param name="host">The docked element tabbed host.</param>
        internal static void StartDragging(DockedElementTabbedHost host)
        {
            DockInfoInternal info = host.HostedElement != null ? DockingManager.GetDockInfo(host.HostedElement)
                : null;
            if (info != null)
            {
                if (info.DockingManager.UseNativeFloatWindow)
                {
                    info.DockingManager.m_nativeWindowDragging = true;
                }
                if (info.DockingManager.DockStateChanging != null)
                {
                    DockStateChangingEventArgs args = new DockStateChangingEventArgs();
                    args.SourceElement = host.HostedElement;
                    args.PresentState = DockState.Dock;
                    args.TargetState = DockState.Float;
                    info.DockingManager.DockStateChanging(host.HostedElement, args);
                    if (!args.Cancel)
                    {
                        info.DockingManager.m_IsStateChangingChecked = true;
                        info.DockingManager.StartDragging(host, host.State);
                    }
                }
                else
                {
                    info.DockingManager.StartDragging(host, host.State);
                }
            }
        }

        /// <summary>
        /// Processes the tab orders.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="tabs">The list of tabs.</param>
        /// <param name="targetName">Name of the target.</param>
        /// <param name="state">The state.</param>
        /// <param name="shiftTabs">if set to <c>true</c> [shift tabs].</param>
        internal static void ProcessTabOrders(DockingManager owner, List<FrameworkElement> tabs, string targetName, DockState state, bool shiftTabs)
        {
            FrameworkElement target = owner.FindChild(targetName);
            if (target != null)
            {
                int iCount = tabs.Count;
                int offset = 0;
                tabs.Sort(new TabChildrenComparer());

                List<FrameworkElement> targetTabs = owner.FindSiblingsSafe(target, state);
                targetTabs.Add(target);

                if (tabs.Count == 1)
                {
                    DockingManager.SetIsSelectedTab(tabs[0], true);
                }

                foreach (FrameworkElement tab in targetTabs)
                {
                    DockingManager.SetIsSelectedTab(tab, false);
                    tabs.Remove(tab);
                }

                if (shiftTabs)
                {
                    foreach (FrameworkElement tab in targetTabs)
                    {
                        int order = DockedElementTabbedHost.GetTabOrder(tab, state);
                        DockedElementTabbedHost.SetTabOrder(tab, state, order + iCount);
                    }
                }
                else
                {
                    offset = GetLastTabOrder(targetTabs) + 1;
                }

                foreach (FrameworkElement tab in tabs)
                {
                    DockedElementTabbedHost.SetTabOrder(tab, state, offset++);
                }
            }
        }

        /// <summary>
        /// Corrects the tabs order after state switch.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="state">The state.</param>
        internal static void CorrectTabsOrderAfterStateSwitch(FrameworkElement element, DockState state)
        {
            DockState newState = (state == DockState.Dock) ? DockState.Float : DockState.Dock;
            string targetName = DockingManager.GetTargetName(element, newState);

            if (!string.IsNullOrEmpty(targetName))
            {
                DockingManager owner = DockingManager.ResolveManager(element);
                List<FrameworkElement> tabs = owner.FindSiblings(element, state, true);
                tabs.Add(element);
                List<FrameworkElement> floatTabs = new List<FrameworkElement>();

                foreach (FrameworkElement tab in tabs)
                {
                    if (DockState.Float == DockingManager.GetState(tab))
                    {
                        floatTabs.Add(tab);
                    }
                }

                DockingManager.ProcessTabOrders(owner, floatTabs, targetName, newState, false);
            }
            else
            {
                DockingManager.SynchronizeTabOrders(element, newState);
            }
        }
       
        internal bool extract = false;
        /// <summary>
        /// Invoked when an unhandled MouseUp�routed event reaches an element in its route that is derived from this class.
        /// </summary>
        /// <param name="e">The MouseButtonEventArgs that contains the event data. The event data reports that the mouse button was released.</param>
        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            if (e.StylusDevice == null || e.StylusDevice != null)
            {
                if (DraggingType.NormalDragging != DraggingType && m_draggedElement != null && extract)
                {
                    ExtractElementToWindow(m_draggedElement, extractmode, true);
                    extract = false;
                }
                bool m_execute = true;
                bool bIsPointInsideTargetManager = IsPointInsideElement(this as UIElement);
                for (int i = 0; i < this.TargetManagers.Count; i++)
                {
                    if ((this.TargetManagers[i] as DockingManager).IsLoaded)
                    {
                        if (!bIsPointInsideTargetManager)
                        {
                            bIsPointInsideTargetManager = IsPointInsideElement(this.TargetManagers[i] as UIElement);
                            if (!bIsPointInsideTargetManager)
                            {
                                DockingManager target = this.TargetManagers[i] as DockingManager;
                                if (target != null && target.m_managerDragPreview != null)
                                {
                                    target.m_managerDragPreview.IsLowPerform = false;
                                    target.m_managerDragPreview.HideDockPreviewMainButton(true);
                                    target.m_managerDragPreview.HideDockPreview();
                                    target.m_hostUnderMouse = null;
                                    target.IsDragging = false;
                                    target.m_draggedElement = null;
                                }
                            }
                            else
                            {
                                if (this.Children.Contains(m_draggedElement))
                                {
                                    this.Children.Remove(m_draggedElement);
                                    DockingManager.SetTargetNameInDockedMode(m_draggedElement, "");
                                    DockingManager.SetTargetNameInFloatingMode(m_draggedElement, "");
                                    TransferManagerEventArgs args = new TransferManagerEventArgs();
                                    args.PreviousManager = this;
                                    args.TargetManager = (this.TargetManagers[i] as DockingManager);
                                    args.TargetElement = m_draggedElement;
                                    FireTransferredFromManager(this, args);
                                }

                                (this.TargetManagers[i] as DockingManager).Children.Add(m_draggedElement);
                                TransferManagerEventArgs transferredargs = new TransferManagerEventArgs();
                                transferredargs.PreviousManager = this;
                                transferredargs.TargetElement = m_draggedElement;
                                transferredargs.TargetManager = (this.TargetManagers[i] as DockingManager);
                                FireTransferredToManager(this, transferredargs);

                                MouseMove -= OnDockingManagerMouseMove;
                                if (m_managerDragPreview != null)
                                {
                                    m_managerDragPreview.HideDockPreviewMainButton(false);
                                    m_managerDragPreview.HideDockPreview();
                                }
                                IsDragging = false;
                                m_draggedElement = null;
                                m_hostUnderMouse = null;
                                m_prevHostUnderMouse = null;
                                ReleaseMouseCapture();

                                m_execute = false;
                                m_mousebuttonargs = e;
                                Point p = Mouse.GetPosition(this.TargetManagers[i] as IInputElement);
                                VisualTreeHelper.HitTest(this.TargetManagers[i] as Visual, null,
                                                         new HitTestResultCallback(TransferManagerCallback),
                                                         new PointHitTestParameters(p));
                            }
                        }
                        else
                        {
                            DockingManager target = this.TargetManagers[i] as DockingManager;
                            if (target != null && target.m_managerDragPreview != null)
                            {
                                target.m_managerDragPreview.IsLowPerform = false;
                                target.m_managerDragPreview.HideDockPreviewMainButton(true);
                                target.m_managerDragPreview.HideDockPreview();
                                target.m_hostUnderMouse = null;
                                target.IsDragging = false;
                                target.m_draggedElement = null;
                            }
                        }
                    }
                }
                if (m_execute)
                {
                    if (UseAdornerFloatWindow)
                    {
                        if (IsDragging)
                        {
                            AdornerFloatWindow windowUnderMouse = GetWindowUnderMouse(e);
                            bool hostSearch = true;

                            if (windowUnderMouse != null)
                            {
                                if (IsPointInsideElement(windowUnderMouse.Header) &&
                                    !windowUnderMouse.IsMultiHostsContainer && IsDragging &&
                                    !DockingManager.GetNoDock(windowUnderMouse.PrimaryElement))
                                {
                                    DockToFloatWindow(DockingManager.GetDockInfo(m_draggedElement).FloatingWindow);
                                    hostSearch = false;
                                }
                            }

                            if (hostSearch)
                            {
                                m_hostUnderMouse = GetHostUnderMouse(e);
                            }
                        }
                    }

                    if (null != m_hostUnderMouse)
                    {
                        if (!m_hostUnderMouse.TabChildren.Contains(m_draggedElement) ||
                            (DraggingType.NormalDragging != DraggingType && 1 == m_hostUnderMouse.TabChildren.Count &&
                            m_draggedElement != m_hostUnderMouse.TabChildren[0]))
                        {
                            CompleteDocking(e.GetPosition(m_hostUnderMouse), true, false);
                        }
                        else if (IsDragItem(m_hostUnderMouse.TabChildren))
                        {
                            if(m_hostUnderMouse.TabChildren.Contains(m_draggedElement) && m_hostUnderMouse.TabChildren.Count==1&& m_hostUnderMouse.HostedElement==m_draggedElement)
                            CompleteDocking();
                            else
                            CompleteDocking(e.GetPosition(m_hostUnderMouse), true, false);
                        }
                        else
                        {
                            CompleteDocking();
                        }
                    }
                    else
                    {
                        FrameworkElement primaryElement = null;
                        if (m_draggedElement != null)
                        {
                            primaryElement = m_draggedElement;
                            //primaryElement.Focus();
                        }
                        CompleteDocking();
                        if (primaryElement != null)
                        {
                            primaryElement.Focus();
                        }
                        else
                        {
                            if (e.Source is DockedElementTabbedHost)
                            {
                                DockedElementTabbedHost host = e.Source as DockedElementTabbedHost;
                                if (host.TabChildren.Count > 0)
                                {
                                    foreach (FrameworkElement child in host.TabChildren)
                                    {
                                        if (DockingManager.GetIsSelectedTab(child))
                                        {
                                            child.Focus();
                                        }
                                    }
                                }
                            }
                            else if (e.OriginalSource is FloatWindowBorder)
                            {
                                FloatWindowBorder header = e.OriginalSource as FloatWindowBorder;
                                if (header.ParentWindow != null)
                                {
                                    FloatWindow window = header.ParentWindow as FloatWindow;
                                    if (window != null && window.PrimaryElement != null)
                                    {
                                        window.PrimaryElement.Focus();
                                    }
                                }
                            }
                        }

                    }
                    DockedElementTabbedHost tabbed = GetHostUnderMouse(e);
                    if (tabbed != null && tabbed.TabChildren.Count == 1)
                    {

                        // SwapTargetInternalAndElement(tabbed.TabChildren[0]);
                    }
                }
                base.OnMouseUp(e);
            }
        }

        internal HitTestResultBehavior NativeWindowCallback(HitTestResult result)
        {

            DockedElementTabbedHost host = VisualUtils.FindAncestor(result.VisualHit as Visual, typeof(DockedElementTabbedHost)) as DockedElementTabbedHost;
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                DockingManager owner = DockingManager.GetDockingManager(result.VisualHit);
                if (owner == null)
                {
                    NativeFloatWindow window = VisualUtils.FindAncestor(result.VisualHit as Visual, typeof(NativeFloatWindow)) as NativeFloatWindow;
                    if (window != null)
                        owner = window.DockingManager;
                }
                if (owner != null && host != null && owner.m_mouseeventargs != null) 
                {
                    DocumentTabControl tabcontrol = VisualUtils.FindDescendant(host as Visual, typeof(DocumentTabControl)) as DocumentTabControl;
                    if (tabcontrol != null && tabcontrol.Items.Count>0)
                    {
                        TabItemExt tabItemExt = tabcontrol.Items[0] as TabItemExt;
                        if (tabItemExt != null)
                        {
                            HeaderPanel headerpanel = VisualUtils.FindAncestor(tabItemExt as Visual, typeof(HeaderPanel)) as HeaderPanel;
                            if (headerpanel != null)
                            {
                                Point p = Mouse.GetPosition(headerpanel);
                                HitTestResult result1 = VisualTreeHelper.HitTest(headerpanel as Visual, p);
                                if (result1 != null && this.m_HeaderPanelMouseEventArgs != null)
                                {
                                    m_HeaderPanelCallback = true;
                                    this.OnMouseMoveOnHeaderPanel(headerpanel, this.m_HeaderPanelMouseEventArgs);
                                    m_HeaderPanelCallback = false;
                                }
                                else if (this.m_HeaderPanelMouseEventArgs != null)
                                    this.OnMouseLeaveOnHeaderPanel(headerpanel, this.m_HeaderPanelMouseEventArgs);
                            }
                        }
                    }
                    owner.OnDockingManagerMouseMove(owner, m_mouseeventargs);
                    owner.OnMouseMoveOnHost(host, owner.m_mouseeventargs);
                    //m_hostUnderMouse = host;
                 }
            }
            if (Mouse.LeftButton == MouseButtonState.Released)
            {
                DockingManager owner = DockingManager.GetDockingManager(result.VisualHit);
                if (owner == null)
                {
                    NativeFloatWindow window = VisualUtils.FindAncestor(result.VisualHit as Visual, typeof(NativeFloatWindow)) as NativeFloatWindow;
                    if (window != null) 
                        owner = window.DockingManager;
                }
                if (owner != null && host != null) 
                {
                    owner.OnMouseUpOnHost(host);
                    ReleaseMouseCapture();
                }
            }
            return HitTestResultBehavior.Continue;
        }

        /// <summary>
        /// Transfers the manager callback.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns></returns>
        internal HitTestResultBehavior TransferManagerCallback(HitTestResult result)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                for (int i = 0; i < this.TargetManagers.Count; i++)
                {
                    DockingManager owner = DockingManager.GetDockingManager(result.VisualHit);
                    if (owner != null)
                    {
                        if (owner == this.TargetManagers[i])
                        {
                            if(ActiveWindow!=null)
                            {
                                DockedElementTabbedHost host = VisualUtils.FindAncestor(result.VisualHit as Visual, typeof(DockedElementTabbedHost)) as DockedElementTabbedHost;
                                if (host != null)
                                {
                                    if (host.InternalDataContext != null)
                                    owner.MouseMoveOnTargetManager(ActiveWindow, host, this.m_mouseeventargs);
                                }
                                else
                                {
                                    {
                                        owner.MouseMoveOnTargetManager(ActiveWindow,owner.m_hostUnderMouse,this.m_mouseeventargs);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else if (Mouse.LeftButton == MouseButtonState.Released)
            {
                for (int i = 0; i < this.TargetManagers.Count; i++)
                {
                    DockingManager owner = DockingManager.GetDockingManager(result.VisualHit);
                    if (owner != null)
                    {
                        if (owner == this.TargetManagers[i])
                        {
                            if (ActiveWindow != null)
                            {
                                DockedElementTabbedHost host = VisualUtils.FindAncestor(result.VisualHit as Visual, typeof(DockedElementTabbedHost)) as DockedElementTabbedHost;
                                
                                if (host != null)
                                {
                                    owner.OnMouseUpOnHost(host, this.m_mousebuttonargs);
                                }
                                else
                                {
                                    if (owner.m_hostUnderMouse != null && owner.m_hostUnderMouse.Visibility== Visibility.Visible)
                                    {
                                        owner.OnMouseUpOnHost(owner.m_hostUnderMouse, this.m_mousebuttonargs);
                                    }
                                    else
                                    {
                                      if(owner.m_prevHostUnderMouse!=null && owner.m_prevHostUnderMouse.Visibility==Visibility.Visible)
                                          owner.OnMouseUpOnHost(owner.m_prevHostUnderMouse, this.m_mousebuttonargs);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return HitTestResultBehavior.Continue;
        }

#if !SyncfusionFramework3_5

        //private void OnTouchLeftFingerDown(TouchEventArgs e)
        //{
        //    if (!BrowserInteropHelper.IsBrowserHosted)
        //    {
        //        ActivateParent();
        //    }

        //    if (e.OriginalSource is FrameworkElement)
        //    {
        //        FrameworkElement originalSource = (FrameworkElement)e.OriginalSource;
        //        DependencyObject templateParent = originalSource.TemplatedParent;

        //        if (null != originalSource)
        //        {
        //            //MT 2261 - Auto Hide panel issues.
        //            SidePanel sidepanel = VisualUtils.FindAncestor(e.OriginalSource as Visual, typeof(SidePanel)) as SidePanel;

        //            if (sidepanel == null && !UsePopupAutoHidePreview)
        //            {
        //                //originalSource.Focusable = true;
        //                // originalSource.Focus();
        //                HideSidePanelItems();
        //            }
        //        }

        //        if (!(templateParent is Splitter))
        //        {
        //            FloatWindowBorder floatBorder = templateParent as FloatWindowBorder;

        //            if (null == floatBorder || FloatWindowBorderMode.Header == floatBorder.BorderMode)
        //            {
        //                m_isPointKnown = false;
        //            }
        //        }
        //    }
        //}

        //protected override void OnTouchMove(TouchEventArgs e)
        //{
        //    if (this.IsTouchEnabled && m_TouchDeviceId == e.TouchDevice.Id)
        //    {
        //        if (IsDragging)
        //        {
        //            //m_HeaderPanelMouseEventArgs = e;
        //            bool bIsPointInsideTargetManager = IsPointInsideElement(this as UIElement);
        //            for (int i = 0; i < this.TargetManagers.Count; i++)
        //            {
        //                if ((this.TargetManagers[i] as DockingManager).IsLoaded)
        //                {
        //                    if (!bIsPointInsideTargetManager)
        //                    {
        //                        OnTouchLeave(e);
        //                        bIsPointInsideTargetManager = IsPointInsideElement(this.TargetManagers[i] as UIElement);
        //                        if (!bIsPointInsideTargetManager)
        //                        {
        //                            DockingManager target = this.TargetManagers[i] as DockingManager;
        //                            if (target != null && target.m_managerDragPreview != null)
        //                            {
        //                                target.m_managerDragPreview.IsLowPerform = false;
        //                                target.m_managerDragPreview.HideDockPreviewMainButton(true);
        //                                target.m_managerDragPreview.HideDockPreview();
        //                                target.m_hostUnderMouse = null;
        //                                target.IsDragging = false;
        //                                target.m_draggedElement = null;
        //                            }
        //                        }
        //                        else
        //                        {
        //                            m_mouseeventargs = e;
        //                            Point p = Mouse.GetPosition(this.TargetManagers[i] as IInputElement);
        //                            bIsPointInsideTargetManager = IsPointInsideElement(this.TargetManagers[i] as UIElement);

        //                            VisualTreeHelper.HitTest(this.TargetManagers[i] as Visual, null,
        //                                                     new HitTestResultCallback(TransferManagerCallback),
        //                                                     new PointHitTestParameters(p));
        //                            if (bIsPointInsideTargetManager)
        //                            {
        //                                DockingManager target = this.TargetManagers[i] as DockingManager;

        //                                if (target != null && target.m_managerDragPreview != null)
        //                                {
        //                                    target.dragelementflag = true;
        //                                    target.m_managerDragPreview.IsLowPerform = false;
        //                                    if (target.Parent is ContentControl)
        //                                    {
        //                                        Point poit = target.PointFromScreen(new Point(0, 0));
        //                                        if (main != poit)
        //                                        {
        //                                            main = poit;
        //                                            target.m_managerDragPreview.HideDockPreviewMainButton(true);
        //                                        }
        //                                    }
        //                                    if (m_draggedElement != null)
        //                                        target.m_hostUnderMouse = DockingManager.GetHost(m_draggedElement,
        //                                                                                         DockingManager.GetState(
        //                                                                                             m_draggedElement));
        //                                    target.IsDragging = true;
        //                                    target.m_draggedElement = m_draggedElement;
        //                                    target.m_firedDragStart = true;
        //                                }
        //                            }
        //                        }
        //                    }
        //                    else
        //                    {
        //                        DockingManager target = this.TargetManagers[i] as DockingManager;
        //                        if (target != null && target.m_managerDragPreview != null)
        //                        {
        //                            target.m_managerDragPreview.IsLowPerform = false;
        //                            target.m_managerDragPreview.HideDockPreviewMainButton(true);
        //                            target.m_managerDragPreview.HideDockPreview();
        //                            target.m_hostUnderMouse = null;
        //                            target.IsDragging = false;
        //                            target.m_draggedElement = null;
        //                        }
        //                    }
        //                }
        //            }
        //            m_dragCounter++;

        //            if (m_dragCounter > 1 && !m_firedDragStart)
        //            {
        //                FireWindowDragStart(ActiveWindow);
        //                m_firedDragStart = true;
        //            }

        //            if (e.Source is DockingManager && e.OriginalSource is DockingManager
        //                && m_managerDragPreview.IsDockPreviewMainButtonVisible)
        //            {
        //                bool bIsPointInsideDocking = IsPointInsideElement(this);
        //                if (!bIsPointInsideDocking)
        //                {
        //                    if (m_usedHosts.Contains(m_hostUnderMouse))
        //                        return;
        //                    else
        //                    {
        //                        m_managerDragPreview.HideDockPreviewMainButton(true);
        //                        m_managerDragPreview.HideDockPreview();
        //                        m_hostUnderMouse = null;
        //                    }
        //                }
        //            }

        //            if (m_firedDragStart)
        //            {
        //                WindowMovingEventArgs args = new WindowMovingEventArgs();
        //                args.State = DockState.Float;
        //                if (ActiveWindow != null && ActiveWindow.IsVisible)
        //                {
        //                    args.X = ActiveWindow.PointToScreen(new Point(0, 0)).X;
        //                    args.Y = ActiveWindow.PointToScreen(new Point(0, 0)).Y;
        //                    FireWindowMoving(ActiveWindow, args);


        //                    if (UseInteropCompatibilityMode && (UsePopupAutoHidePreview || !UseAdornerDockPreview))
        //                    {
        //                        FloatWindow window = DockingManager.GetFloatWindow(ActiveWindow) as FloatWindow;
        //                        DockedElementTabbedHost host = m_hostUnderMouse != null ? m_hostUnderMouse : null;
        //                        List<DockedElementTabbedHost> floathost = new List<DockedElementTabbedHost>();
        //                        List<DockedElementTabbedHost> dockinghost = new List<DockedElementTabbedHost>();
        //                        bool b_mouseonhostexecuted = false;
        //                        if (host != null)
        //                        {
        //                            foreach (DockedElementTabbedHost item in m_usedHosts)
        //                            {
        //                                if (item != null)
        //                                {
        //                                    if (item.State == DockState.Float)
        //                                        floathost.Add(item);
        //                                    else if (item.State == DockState.Dock)
        //                                        dockinghost.Add(item);
        //                                }
        //                            }
        //                            TraverseHitTestHost(floathost, e, DockState.Float, ref b_mouseonhostexecuted);

        //                            if (!b_mouseonhostexecuted)
        //                                TraverseHitTestHost(dockinghost, e, DockState.Dock, ref b_mouseonhostexecuted);
        //                            else
        //                            {
        //                                testresult = null;
        //                                floathost = null;
        //                                dockinghost = null;
        //                                base.OnTouchMove(e);
        //                                return;
        //                            }
        //                        }

        //                        if (host != null && !b_mouseonhostexecuted)
        //                        {
        //                            this.OnDockingManagerMouseMove(host, e);
        //                            this.OnMouseMoveOnHost(host, e);
        //                        }
        //                        testresult = null;
        //                        floathost = null;
        //                        dockinghost = null;
        //                    }
        //                }
        //            }
        //        }
        //        base.OnTouchMove(e);
        //    }
        //}

        protected override void OnTouchDown(TouchEventArgs e)
        {
            if (this.IsTouchEnabled)
            {
                base.OnTouchDown(e);
                m_TouchDevice = e.TouchDevice;
                //OnTouchLeftFingerDown(e);
                //newOnHeaderPoint = false;
                //onHeaderPoint.X = 0;
                //onHeaderPoint.Y = 0;
                //isTouchDown = true;
            }
        }

        //protected override void OnTouchEnter(TouchEventArgs e)
        //{
        //    m_TouchDeviceId = (m_TouchDeviceId == -1) ? e.TouchDevice.Id : m_TouchDeviceId;
        //    if (this.IsTouchEnabled)
        //    {
        //        HitTestResult hitTest = VisualTreeHelper.HitTest(this, (e.GetTouchPoint(this) as TouchPoint).Position);

        //        if (null != hitTest)
        //        {
        //            if (m_managerDragPreview != null)
        //            {
        //                m_managerDragPreview.IsLowPerform = true;
        //            }
        //        }
        //        MouseEnter = true;
        //        base.OnTouchEnter(e);
        //    }
        //}

        //protected override void OnTouchLeave(TouchEventArgs e)
        //{
        //    if (this.IsTouchEnabled && m_TouchDeviceId == e.TouchDevice.Id)
        //    {
        //        m_TouchDeviceId = -1;
        //        m_dockingManagerSystemGesture = SystemGesture.None;
        //        if (!(BrowserInteropHelper.IsBrowserHosted && IsDragging) && m_managerDragPreview != null)
        //        {
        //            m_managerDragPreview.IsLowPerform = false;
        //            m_managerDragPreview.HideDockPreview();
        //            this.m_managerDragPreview.HideDockPreviewMainButton(true);
        //        }
        //        base.OnTouchLeave(e);
        //    }
        //}

        protected override void OnTouchUp(TouchEventArgs e)
        {
            TouchMove -= OnDockingManagerMouseMove;
            isTouchDown = false;
            if (this.IsTouchEnabled)
            {

                //   OnTouchLeaveOnHeaderPanel(this, e);
                if (DraggingType.NormalDragging != DraggingType && m_draggedElement != null && extract)
                {
                    ExtractElementToWindow(m_draggedElement, extractmode, true);
                    extract = false;
                }
                bool m_execute = true;
                bool bIsPointInsideTargetManager = IsPointInsideElement(this as UIElement);
                for (int i = 0; i < this.TargetManagers.Count; i++)
                {
                    if ((this.TargetManagers[i] as DockingManager).IsLoaded)
                    {
                        if (!bIsPointInsideTargetManager)
                        {
                            bIsPointInsideTargetManager = IsPointInsideElement(this.TargetManagers[i] as UIElement);
                            if (!bIsPointInsideTargetManager)
                            {
                                DockingManager target = this.TargetManagers[i] as DockingManager;
                                if (target != null && target.m_managerDragPreview != null)
                                {
                                    target.m_managerDragPreview.IsLowPerform = false;
                                    target.m_managerDragPreview.HideDockPreviewMainButton(true);
                                    target.m_managerDragPreview.HideDockPreview();
                                    target.m_hostUnderMouse = null;
                                    target.IsDragging = false;
                                    target.m_draggedElement = null;
                                }
                            }
                            else
                            {
                                if (this.Children.Contains(m_draggedElement))
                                {
                                    this.Children.Remove(m_draggedElement);
                                    DockingManager.SetTargetNameInDockedMode(m_draggedElement, "");
                                    DockingManager.SetTargetNameInFloatingMode(m_draggedElement, "");
                                    TransferManagerEventArgs args = new TransferManagerEventArgs();
                                    args.PreviousManager = this;
                                    args.TargetManager = (this.TargetManagers[i] as DockingManager);
                                    args.TargetElement = m_draggedElement;
                                    FireTransferredFromManager(this, args);
                                }

                                (this.TargetManagers[i] as DockingManager).Children.Add(m_draggedElement);
                                TransferManagerEventArgs transferredargs = new TransferManagerEventArgs();
                                transferredargs.PreviousManager = this;
                                transferredargs.TargetElement = m_draggedElement;
                                transferredargs.TargetManager = (this.TargetManagers[i] as DockingManager);
                                FireTransferredToManager(this, transferredargs);

                                TouchMove -= OnDockingManagerMouseMove;
                                if (m_managerDragPreview != null)
                                {
                                    m_managerDragPreview.HideDockPreviewMainButton(false);
                                    m_managerDragPreview.HideDockPreview();
                                }
                                IsDragging = false;
                                m_draggedElement = null;
                                m_hostUnderMouse = null;
                                m_prevHostUnderMouse = null;
                                ReleaseTouchCapture(e.TouchDevice);

                                m_execute = false;
                                m_mousebuttonargs = e;
                                Point p = Mouse.GetPosition(this.TargetManagers[i] as IInputElement);
                                VisualTreeHelper.HitTest(this.TargetManagers[i] as Visual, null,
                                                         new HitTestResultCallback(TransferManagerCallback),
                                                         new PointHitTestParameters(p));
                            }
                        }
                        else
                        {
                            DockingManager target = this.TargetManagers[i] as DockingManager;
                            if (target != null && target.m_managerDragPreview != null)
                            {
                                target.m_managerDragPreview.IsLowPerform = false;
                                target.m_managerDragPreview.HideDockPreviewMainButton(true);
                                target.m_managerDragPreview.HideDockPreview();
                                target.m_hostUnderMouse = null;
                                target.IsDragging = false;
                                target.m_draggedElement = null;
                            }
                        }
                    }
                }
                if (m_execute)
                {
                    if (UseAdornerFloatWindow)
                    {
                        if (IsDragging)
                        {
                            AdornerFloatWindow windowUnderMouse = GetWindowUnderMouse(e);
                            bool hostSearch = true;

                            if (windowUnderMouse != null)
                            {
                                if (IsPointInsideElement(windowUnderMouse.Header) &&
                                    !windowUnderMouse.IsMultiHostsContainer && IsDragging &&
                                    !DockingManager.GetNoDock(windowUnderMouse.PrimaryElement))
                                {
                                    DockToFloatWindow(DockingManager.GetDockInfo(m_draggedElement).FloatingWindow);
                                    hostSearch = false;
                                }
                            }

                            if (hostSearch)
                            {
                                m_hostUnderMouse = GetHostUnderMouse(e);
                            }
                        }
                    }

                    if (null != m_hostUnderMouse)
                    {
                        if (!m_hostUnderMouse.TabChildren.Contains(m_draggedElement) ||
                            (DraggingType.NormalDragging != DraggingType && 1 == m_hostUnderMouse.TabChildren.Count &&
                            m_draggedElement != m_hostUnderMouse.TabChildren[0]))
                        {
                            CompleteDocking((e.GetTouchPoint(m_hostUnderMouse) as TouchPoint).Position, true, false);
                        }
                        else if (IsDragItem(m_hostUnderMouse.TabChildren))
                        {
                            CompleteDocking((e.GetTouchPoint(m_hostUnderMouse) as TouchPoint).Position, true, true);
                        }
                        else
                        {
                            CompleteDocking();
                        }
                    }
                    else
                    {
                        FrameworkElement primaryElement = null;
                        if (m_draggedElement != null)
                        {
                            primaryElement = m_draggedElement;
                            //primaryElement.Focus();
                        }
                        CompleteDocking();
                        if (primaryElement != null)
                        {
                            primaryElement.Focus();
                        }
                        else
                        {
                            if (e.Source is DockedElementTabbedHost)
                            {
                                DockedElementTabbedHost host = e.Source as DockedElementTabbedHost;
                                if (host.TabChildren.Count > 0)
                                {
                                    foreach (FrameworkElement child in host.TabChildren)
                                    {
                                        if (DockingManager.GetIsSelectedTab(child))
                                        {
                                            child.Focus();
                                        }
                                    }
                                }
                            }
                            else if (e.OriginalSource is FloatWindowBorder)
                            {
                                FloatWindowBorder header = e.OriginalSource as FloatWindowBorder;
                                if (header.ParentWindow != null)
                                {
                                    FloatWindow window = header.ParentWindow as FloatWindow;
                                    if (window != null && window.PrimaryElement != null)
                                    {
                                        window.PrimaryElement.Focus();
                                    }
                                }
                            }
                        }

                    }
                    DockedElementTabbedHost tabbed = GetHostUnderMouse(e);
                    if (tabbed != null && tabbed.TabChildren.Count == 1)
                    {

                        // SwapTargetInternalAndElement(tabbed.TabChildren[0]);
                    }
                }
                base.OnTouchUp(e);
            }
        }

        /// <summary>
        /// Called when [Touch leave on header panel].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        internal void OnTouchLeaveOnHeaderPanel(object sender, TouchEventArgs e)
        {
            if (this.Dispatcher.CheckAccess())
            {
                if (m_dockingManagerSystemGesture == SystemGesture.Drag)
                {
                    //DocumentTabControl tabcontrol = VisualUtils.FindDescendant(this as Visual, typeof(DocumentTabControl)) as DocumentTabControl;
                    //if (tabcontrol != null && tabcontrol.Items.Count > 0)
                    //{
                    //    foreach (var item in tabcontrol.Items)
                    //    {
                    //        TabItemExt tabitem = item as TabItemExt;
                    //        if (tabitem != null && tabitem.Content != null && (tabitem.Content as ContentPresenter) != null && (tabitem.Content as ContentPresenter).Content != null)
                    //        {
                    //            if ((tabitem.Content as ContentPresenter).Content == m_draggedElement)
                    //            {
                    //                if (!tabitem.IsSelected)
                    //                {
                    //                    m_draggedElement = null;
                    //                }
                    //            }
                    //        }
                    //    }
                    //}
                    if (this.IsVS2010DraggingEnabled && this.m_draggedElement != null
                        && !DockingManager.GetIsDragged(m_draggedElement) && !m_dragstarted
                        && DockingManager.GetState(this.m_draggedElement) == DockState.Document
                        && DockingManager.GetCanFloat(this.m_draggedElement) && DockingManager.GetCanDrag(m_draggedElement))
                    {
                        DockedElementTabbedHost host = DockingManager.GetTabbedHost(this.m_draggedElement, DockState.Float);
                        if (host != null)
                        {
                            DockingManager owner = DockingManager.ResolveManager(this.m_draggedElement);
                            if (owner != null)
                            {
                                owner.RemoveFromDocumentContainer(this.m_draggedElement);
                                host.TabChildren.Add(this.m_draggedElement);
                                DockingManager.StartDragging(host);
                                m_dragstarted = false;
                            }
                        }
                    }
                }
                if (this.IsVS2010DraggingEnabled)
                {
                    int index = m_mouseonheaderpanelindex;
                    m_mousemoveonheaderpanel = false;
                    m_mousemoveonTabpaneladv = false;
                    m_mouseonheaderpanelindex = -1;
                    if (!UseAdornerDockPreview)
                    {
                        Point mousepoint = Mouse.GetPosition(this);
                        HitTestResult testresult = VisualTreeHelper.HitTest(this, mousepoint);
                        HeaderPanel panel = (testresult != null) ? VisualUtils.FindAncestor(testresult.VisualHit as Visual, typeof(HeaderPanel)) as HeaderPanel : null;
                        if (panel != null && index > 0)
                        {
                            m_mouseonheaderpanelindex = index;
                            m_mousemoveonheaderpanel = true;
                        }
                    }
                    else
                    {
                        DocumentTabControl tabcontrol = VisualUtils.FindDescendant(this as Visual, typeof(DocumentTabControl)) as DocumentTabControl;
                        if (tabcontrol != null && tabcontrol.Items.Count > 0)
                        {
                            TabItemExt tabItemExt = tabcontrol.Items[0] as TabItemExt;
                            if (tabItemExt != null)
                            {
                                HeaderPanel headerpanel = VisualUtils.FindAncestor(tabItemExt as Visual, typeof(HeaderPanel)) as HeaderPanel;
                                if (headerpanel != null)
                                {
                                    Point p = Mouse.GetPosition(headerpanel);
                                    HitTestResult result1 = VisualTreeHelper.HitTest(headerpanel as Visual, p);
                                    if (result1 != null)
                                    {
                                        m_mousemoveonheaderpanel = true;
                                    }
                                    else
                                    {
                                    }
                                }
                            }
                        }
                        if (m_managerDragPreview != null)
                        {
                            m_managerDragPreview.IsLowPerform = false;
                            m_managerDragPreview.HideDockPreview();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Called when [mouse move on header panel].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        internal void OnTouchMoveOnHeaderPanel(object sender, TouchEventArgs e)
        {
            if (this.Dispatcher.CheckAccess())
            {
                if (m_hostUnderMouse != null && IsVS2010DraggingEnabled)
                {
                    if (DockingManager.GetState(m_draggedElement) == DockState.Float)
                    {
                        if (UseNativeFloatWindow)
                        {
                            NativeFloatWindow nativewindow = DockingManager.GetNativeWindow(m_draggedElement);
                            if (nativewindow != null)
                                m_mouseonheaderpaneltabareawidth = nativewindow.NativeHeader.DesiredSize.Width;

                            if (m_HeaderPanelCallback)
                            {
                                TabPanelAdv tabpanel = VisualUtils.FindDescendant((sender as HeaderPanel) as Visual, typeof(TabPanelAdv)) as TabPanelAdv;
                                if (tabpanel != null)
                                {
                                    Point tabpanelpoint = Mouse.GetPosition(tabpanel);
                                    HitTestResult testresult = VisualTreeHelper.HitTest(tabpanel as Visual, tabpanelpoint);
                                    if (testresult != null)
                                    {
                                        if (!m_mousemoveonTabpaneladv)
                                        {
                                            double increment = 0.0;
                                            m_mouseonheaderpanelposition = e.GetTouchPoint(tabpanel as IInputElement).Position.X;
                                            if (tabpanel.tab != null
                                                && tabpanel.tab.newtabParent != null)
                                            {
                                                m_mouseonheaderpanelindex = tabpanel.tab.newtabParent.Items.Count;
                                            }
                                            else
                                            {
                                                m_mouseonheaderpanelindex = 0;
                                            }
                                            m_mousemoveonTabpaneladv = true;
                                            foreach (TabItemExt tabitem in tabpanel.tab.newtabParent.Items)
                                            {
                                                increment += tabitem.ActualWidth;
                                            }
                                            if (m_mouseonheaderpanelposition > increment)
                                            {
                                                m_mouseonheaderpanelposition = increment;
                                            }
                                        }
                                    }
                                }
                                TabLayoutPanel tablayoutpanel = VisualUtils.FindDescendant((sender as HeaderPanel) as Visual, typeof(TabLayoutPanel)) as TabLayoutPanel;
                                if (tablayoutpanel != null)
                                {
                                    Point tablayoutpanelpoint = Mouse.GetPosition(tablayoutpanel);
                                    HitTestResult testresult1 = VisualTreeHelper.HitTest(tablayoutpanel as Visual, tablayoutpanelpoint);
                                    if (testresult1 != null)
                                    {
                                        if (tablayoutpanel.Children.Count > 0 && ((tablayoutpanel.Children[0] as TabItemExt).Parent as DocumentTabControl) != null)
                                        {
                                            DocumentTabControl tabcontrol = ((tablayoutpanel.Children[0] as TabItemExt).Parent as DocumentTabControl);
                                            if (m_mouseonheaderpanelindex != tabcontrol.m_mousemoveonitemindex || m_mousemoveonTabpaneladv)
                                            {
                                                foreach (var item in tabcontrol.Items)
                                                {
                                                    TabItemExt tabitem = item as TabItemExt;
                                                    Point tabitempoint = Mouse.GetPosition(tabitem);
                                                    HitTestResult tabitemresult = VisualTreeHelper.HitTest(tabitem as Visual, tabitempoint);
                                                    if (tabitemresult != null)
                                                    {
                                                        m_mouseonheaderpanelindex = tabcontrol.Items.IndexOf(item);
                                                        break;
                                                    }
                                                }
                                                m_mouseonheaderpanelposition = (e.GetTouchPoint(tablayoutpanel as IInputElement).Position.X - e.GetTouchPoint((tablayoutpanel.Children[m_mouseonheaderpanelindex] as TabItemExt) as IInputElement).Position.X) + 6;
                                                tabcontrol.m_mousemoveonitem = m_draggedElement;
                                            }
                                            m_mousemoveonTabpaneladv = false;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            IWindow window = DockingManager.GetFloatWindow(m_draggedElement);
                            if (window != null)
                                m_mouseonheaderpaneltabareawidth = window.Header.DesiredSize.Width;
                            if ((e.Source as TabPanelAdv) != null && ((e.Source as TabPanelAdv) as IInputElement) != null)
                            {
                                if (!m_mousemoveonTabpaneladv)
                                {
                                    double increment = 0.0;
                                    m_mouseonheaderpanelposition = e.GetTouchPoint((e.Source as TabPanelAdv) as IInputElement).Position.X;
                                    if ((e.Source as TabPanelAdv).tab != null
                                        && (e.Source as TabPanelAdv).tab.newtabParent != null)
                                    {
                                        m_mouseonheaderpanelindex = (e.Source as TabPanelAdv).tab.newtabParent.Items.Count;
                                    }
                                    else
                                    {
                                        m_mouseonheaderpanelindex = 0;
                                    }
                                    m_mousemoveonTabpaneladv = true;
                                    foreach (TabItemExt tabitem in (e.Source as TabPanelAdv).tab.newtabParent.Items)
                                    {
                                        increment += tabitem.ActualWidth;
                                    }
                                    if (m_mouseonheaderpanelposition > increment)
                                    {
                                        m_mouseonheaderpanelposition = increment;
                                    }
                                }
                            }
                            if ((e.Source as TabLayoutPanel) != null && ((e.Source as TabLayoutPanel) as IInputElement) != null)
                            {
                                if ((e.Source as TabLayoutPanel).Children.Count > 0
                                    && (((e.Source as TabLayoutPanel).Children[0] as TabItemExt).Parent as DocumentTabControl) != null)
                                {
                                    DocumentTabControl tabcontrol = (((e.Source as TabLayoutPanel).Children[0] as TabItemExt).Parent as DocumentTabControl);
                                    if (m_mouseonheaderpanelindex != tabcontrol.m_mousemoveonitemindex || m_mousemoveonTabpaneladv)
                                    {
                                        m_mouseonheaderpanelposition = (e.GetTouchPoint((e.Source as TabLayoutPanel) as IInputElement).Position.X - e.GetTouchPoint(((e.Source as TabLayoutPanel).Children[tabcontrol.m_mousemoveonitemindex] as TabItemExt) as IInputElement).Position.X) + 6;
                                        m_mouseonheaderpanelindex = tabcontrol.m_mousemoveonitemindex;
                                        tabcontrol.m_mousemoveonitem = m_draggedElement;
                                    }
                                    m_mousemoveonTabpaneladv = false;
                                }
                            }
                        }
                    }
                    m_mousemoveonheaderpanel = true;
                    if (!UseAdornerDockPreview && m_mouseonheaderpanelindex != -1 && m_managerDragPreview.m_headerpanelindex != -1 && m_mouseonheaderpanelindex != m_managerDragPreview.m_headerpanelindex)
                    {
                        if (m_managerDragPreview != null)
                        {
                            m_managerDragPreview.HideDockPreview();
                        }
                    }
                    FireDockProviderShownEvent(m_hostUnderMouse);
                    ProcessingCreateDockPreview(e);
                }
            }
        }

        internal void OnTouchMoveOnTDILayoutPanel(object sender, TouchEventArgs e)
        {
            HeaderPanel headerpanel = VisualUtils.FindDescendant(this as Visual, typeof(HeaderPanel)) as HeaderPanel;
            Point mousepoint = Mouse.GetPosition(headerpanel);
            HitTestResult testresult = (headerpanel != null) ? VisualTreeHelper.HitTest(headerpanel, mousepoint) : null;
            if (this.Dispatcher.CheckAccess())
            {
                if (IsVS2010DraggingEnabled && !IsDragging)
                {


                    //if ((e.Source as DocumentTabControl) != null)
                    {
                        if (this.m_draggedElement != null && !m_dragstarted
                            && DockingManager.GetState(this.m_draggedElement) == DockState.Document
                            && DockingManager.GetCanFloat(this.m_draggedElement) && DockingManager.GetCanDrag(m_draggedElement))
                        {
                            DockedElementTabbedHost host = DockingManager.GetTabbedHost(this.m_draggedElement, DockState.Float);
                            if (host != null)
                            {
                                DockingManager owner = DockingManager.ResolveManager(this.m_draggedElement);
//                                if (owner != null && testresult == null)
                                if (owner != null)
                                {
                                    owner.RemoveFromDocumentContainer(this.m_draggedElement);
                                    host.TabChildren.Add(this.m_draggedElement);
                                    DockingManager.StartDragging(host);
                                    m_dragstarted = false;
                                }
                                //else if (owner != null && testresult != null)
                                //{
                                //    UpdateTabItemPosition(headerpanel, e);
                                //}
                            }
                        }
                        m_mousemoveonheaderpanel = false;
                        m_mousemoveonTabpaneladv = false;
                        m_mouseonheaderpanelindex = -1;
                        if (m_managerDragPreview != null)
                        {
                            m_managerDragPreview.IsLowPerform = false;
                            m_managerDragPreview.HideDockPreview();
                        }
                    }

                    m_mousemoveontdilayoutpanel = true;
                }
            }
        }

        internal void OnTouchLeaveOnTDILayoutPanel(object sender, TouchEventArgs e)
        {
            if (this.Dispatcher.CheckAccess())
            {
                if (IsVS2010DraggingEnabled)
                {
                    m_mousemoveontdilayoutpanel = false;
                }
            }
        }

        //void item_TouchUp(object sender, TouchEventArgs e)
        //{
        //    if (this.IsTouchEnabled && m_TouchDeviceId == e.TouchDevice.Id)
        //    {
        //        #region item_TouchLeftFingerUp
        //        if (this.m_dockingManagerSystemGesture == SystemGesture.Tap)
        //            m_DraggingTabs.Clear();
        //        #endregion
        //    }
        //}

        //void item_TouchEnter(object sender, TouchEventArgs e)
        //{
        //    if (this.IsTouchEnabled && m_TouchDeviceId == e.TouchDevice.Id)
        //        m_DraggingTabs.Clear();
        //}

        //static void tabItem_TouchUp(object sender, TouchEventArgs e)
        //{
        //    DockingManager docking = VisualUtils.FindAncestor((sender as TabItem) as Visual, typeof(DockingManager)) as DockingManager;
        //    if (docking.IsTouchEnabled && docking.m_TouchDeviceId == e.TouchDevice.Id)
        //    {
        //        #region tabItem_DoubleTouch
        //        if (docking.m_dockingManagerSystemGesture == SystemGesture.TwoFingerTap)
        //        {
        //            tabItem_DoubleTouch(sender, e);
        //        }
        //        #endregion
        //    }
        //}

        //static void tabItem_PreviewTouchDown(object sender, TouchEventArgs e)
        //{
        //    DockingManager docking = VisualUtils.FindAncestor((sender as TabItem) as Visual, typeof(DockingManager)) as DockingManager;
        //    if (docking.IsTouchEnabled && docking.m_TouchDeviceId == e.TouchDevice.Id)
        //    {
        //        #region tabItem_PreviewTouchLeftFingerDown
        //        tabItem_PreviewTouchLeftFingerDown(sender, e);
        //        #endregion
        //    }
        //}

        //static void tabItem_PreviewTouchMove(object sender, TouchEventArgs e)
        //{
        //    DockingManager docking = VisualUtils.FindAncestor((sender as TabItem) as Visual, typeof(DockingManager)) as DockingManager;
        //    if (docking.IsTouchEnabled && docking.m_TouchDeviceId == e.TouchDevice.Id)
        //    {
        //        #region tabItem_PreviewTouchRightFingerDown
        //        if (docking.m_dockingManagerSystemGesture == SystemGesture.HoldEnter)
        //        {
        //            tabItem_PreviewTouchRightFingerDown(sender, e);
        //        }
        //        #endregion
        //    }
        //}

        //private static void tabItem_PreviewTouchLeftFingerDown(object sender, TouchEventArgs e)
        //{
        //    TabItem tabItem = (TabItem)sender;
        //    FrameworkElement element = (FrameworkElement)tabItem.Content;

        //    if (element != null && DockingManager.GetCanDrag(element))
        //    {
        //        DockInfoInternal info = DockingManager.GetDockInfo(element);
        //        if (info != null)
        //        {
        //            info.DockingManager.m_isTabPressed = true;
        //            info.DockingManager.m_holdList = new List<FrameworkElement>(1) { element };
        //            info.DockingManager.m_floatWindowRectCoord = GetFloatingWindowRect(element);
        //            info.DockingManager.m_rectBeforeAddingTab = Rect.Empty;
        //        }
        //    }
        //}

        //private static void tabItem_PreviewTouchRightFingerDown(object sender, TouchEventArgs e)
        //{
        //    TabItem tabItem = (TabItem)sender;
        //    FrameworkElement element = (FrameworkElement)tabItem.Content;
        //    DockingManager.SelectTab(element);
        //}

        //private static void tabItem_DoubleTouch(object sender, TouchEventArgs e)
        //{
        //    TabItem tabItem = (TabItem)sender;

        //    if (DockingManager.IsDockTabItem(tabItem))
        //    {
        //        FrameworkElement element = (FrameworkElement)tabItem.Content;
        //        m_floatWindowRect = DockingManager.GetFloatingWindowRect(element);

        //        if (null != element)
        //        {
        //            DockState oldState = DockingManager.GetState(element);
        //            DockState newState = (DockState.Dock == oldState) ? DockState.Float : DockState.Dock;
        //            DockingManager owner = DockingManager.ResolveManager(element);

        //            bool bCanChangeState = DockingManager.CanChangeState(element, newState);

        //            if (bCanChangeState)
        //            {
        //                DockedElementTabbedHost.RemoveTab(element, oldState);
        //                owner.ExecuteDoubleClick(element, ActionMode.Active);
        //                DockedElementTabbedHost.AddTab(element, false);
        //                DockedElementTabbedHost.SelectTab(element, newState);
        //                DockingManager.SetNewFocusedElement(element);
        //                owner.LockLayoutUpdate = true;
        //            }

        //            e.Handled = true;
        //        }
        //    }
        //}

        //internal static void tabItemext_PreviewTouchDown(object sender, TouchEventArgs e)
        //{
        //    DockingManager docking = VisualUtils.FindAncestor((sender as TabItemExt) as Visual, typeof(DockingManager)) as DockingManager;
        //    if (docking.IsTouchEnabled && docking.m_TouchDeviceId == e.TouchDevice.Id)
        //    {
        //        #region tabItemext_PreviewTouchLeftFingerDown

        //        tabItemext_PreviewTouchLeftFingerDown(sender, e);

        //        #endregion
        //    }
        //}

        //internal static void tabItemext_PreviewTouchUp(object sender, TouchEventArgs e)
        //{
        //    DockingManager docking = VisualUtils.FindAncestor((sender as TabItemExt) as Visual, typeof(DockingManager)) as DockingManager;
        //    if (docking!=null && docking.IsTouchEnabled && docking.m_TouchDeviceId == e.TouchDevice.Id)
        //    {
        //        #region tabItemext_PreviewTouchLeftFingerUp
        //        if (docking.m_dockingManagerSystemGesture == SystemGesture.Tap)
        //        {
        //            tabItemext_PreviewTouchLeftFingerUp(sender, e);
        //        }
        //        #endregion
        //    }
        //}

        //private static void tabItemext_PreviewTouchLeftFingerDown(object sender, TouchEventArgs e)
        //{
        //    TabItemExt tabItem = (TabItemExt)sender;
        //    FrameworkElement element = (FrameworkElement)tabItem.Content;

        //    if (element != null && DockingManager.GetCanDrag(element))
        //    {
        //        DockingManager.m_holdTabItemExtList = new List<FrameworkElement>(1) { element };
        //    }
        //}

        //private static void tabItemext_PreviewTouchLeftFingerUp(object sender, TouchEventArgs e)
        //{
        //    TabItemExt tabItem = (TabItemExt)sender;
        //    FrameworkElement element = (FrameworkElement)tabItem.Content;

        //    if (element != null && DockingManager.GetCanDrag(element))
        //    {
        //        DockingManager.m_holdTabItemExtList = new List<FrameworkElement>(1) { element };
        //    }
        //}

#endif

        protected override void OnStylusSystemGesture(StylusSystemGestureEventArgs e)
        {
            m_dockingManagerSystemGesture = e.SystemGesture;
            base.OnStylusSystemGesture(e);
        }
        /// <summary>
        /// Invoked when an unhandled MouseMove�attached event reaches an element in its route that is derived from this class.
        /// </summary>
        /// <param name="e">The MouseEventArgs that contains the event data.</param>
        Point main;
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (e.StylusDevice == null || e.StylusDevice != null)
            {
                if (IsDragging)
                {
                    m_HeaderPanelMouseEventArgs = e;
                    bool bIsPointInsideTargetManager = IsPointInsideElement(this as UIElement);
                    for (int i = 0; i < this.TargetManagers.Count; i++)
                    {
                        if ((this.TargetManagers[i] as DockingManager).IsLoaded)
                        {
                            if (!bIsPointInsideTargetManager)
                            {
                                OnMouseLeave(e);
                                bIsPointInsideTargetManager = IsPointInsideElement(this.TargetManagers[i] as UIElement);
                                if (!bIsPointInsideTargetManager)
                                {
                                    DockingManager target = this.TargetManagers[i] as DockingManager;
                                    if (target != null && target.m_managerDragPreview != null)
                                    {
                                        target.m_managerDragPreview.IsLowPerform = false;
                                        target.m_managerDragPreview.HideDockPreviewMainButton(true);
                                        target.m_managerDragPreview.HideDockPreview();
                                        target.m_hostUnderMouse = null;
                                        target.IsDragging = false;
                                        target.m_draggedElement = null;
                                    }
                                }
                                else
                                {
                                    m_mouseeventargs = e;
                                    Point p = Mouse.GetPosition(this.TargetManagers[i] as IInputElement);
                                    bIsPointInsideTargetManager = IsPointInsideElement(this.TargetManagers[i] as UIElement);

                                    VisualTreeHelper.HitTest(this.TargetManagers[i] as Visual, null,
                                                             new HitTestResultCallback(TransferManagerCallback),
                                                             new PointHitTestParameters(p));
                                    if (bIsPointInsideTargetManager)
                                    {
                                        DockingManager target = this.TargetManagers[i] as DockingManager;

                                        if (target != null && target.m_managerDragPreview != null)
                                        {
                                            target.dragelementflag = true;
                                            target.m_managerDragPreview.IsLowPerform = false;
                                            if (target.Parent is ContentControl)
                                            {
                                                Point poit = target.PointFromScreen(new Point(0, 0));
                                                if (main != poit)
                                                {
                                                    main = poit;
                                                    target.m_managerDragPreview.HideDockPreviewMainButton(true);
                                                }
                                            }
                                            if (m_draggedElement != null)
                                                target.m_hostUnderMouse = DockingManager.GetHost(m_draggedElement,
                                                                                                 DockingManager.GetState(
                                                                                                     m_draggedElement));
                                            target.IsDragging = true;
                                            target.m_draggedElement = m_draggedElement;
                                            target.m_firedDragStart = true;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                DockingManager target = this.TargetManagers[i] as DockingManager;
                                if (target != null && target.m_managerDragPreview != null)
                                {
                                    target.m_managerDragPreview.IsLowPerform = false;
                                    target.m_managerDragPreview.HideDockPreviewMainButton(true);
                                    target.m_managerDragPreview.HideDockPreview();
                                    target.m_hostUnderMouse = null;
                                    target.IsDragging = false;
                                    target.m_draggedElement = null;
                                }
                            }
                        }
                    }
                    m_dragCounter++;

                    if (m_dragCounter > 1 && !m_firedDragStart)
                    {
                        FireWindowDragStart(ActiveWindow);
                        m_firedDragStart = true;
                    }

                    if (e.Source is DockingManager && e.OriginalSource is DockingManager
                        && m_managerDragPreview.IsDockPreviewMainButtonVisible)
                    {
                        bool bIsPointInsideDocking = IsPointInsideElement(this);
                        if (!bIsPointInsideDocking)
                        {
                            if (m_usedHosts.Contains(m_hostUnderMouse))
                                return;
                            else
                            {
                                m_managerDragPreview.HideDockPreviewMainButton(true);
                                m_managerDragPreview.HideDockPreview();
                                m_hostUnderMouse = null;
                            }
                        }
                    }

                    if (m_firedDragStart)
                    {
                        WindowMovingEventArgs args = new WindowMovingEventArgs();
                        args.State = DockState.Float;
                        if (ActiveWindow != null && ActiveWindow.IsVisible)
                        {
                            args.X = ActiveWindow.PointToScreen(new Point(0, 0)).X;
                            args.Y = ActiveWindow.PointToScreen(new Point(0, 0)).Y;
                            FireWindowMoving(ActiveWindow, args);


                            if (UseInteropCompatibilityMode && (UsePopupAutoHidePreview || !UseAdornerDockPreview))
                            {
                                FloatWindow window = DockingManager.GetFloatWindow(ActiveWindow) as FloatWindow;
                                DockedElementTabbedHost host = m_hostUnderMouse != null ? m_hostUnderMouse : null;
                                List<DockedElementTabbedHost> floathost = new List<DockedElementTabbedHost>();
                                List<DockedElementTabbedHost> dockinghost = new List<DockedElementTabbedHost>();
                                bool b_mouseonhostexecuted = false;
                                if (host != null)
                                {
                                    foreach (DockedElementTabbedHost item in m_usedHosts)
                                    {
                                        if (item != null)
                                        {
                                            if (item.State == DockState.Float)
                                                floathost.Add(item);
                                            else if (item.State == DockState.Dock)
                                                dockinghost.Add(item);
                                        }
                                    }
                                    TraverseHitTestHost(floathost, e, DockState.Float, ref b_mouseonhostexecuted);

                                    if (!b_mouseonhostexecuted)
                                        TraverseHitTestHost(dockinghost, e, DockState.Dock, ref b_mouseonhostexecuted);
                                    else
                                    {
                                        testresult = null;
                                        floathost = null;
                                        dockinghost = null;
                                        base.OnMouseMove(e);
                                        return;
                                    }
                                }

                                if (host != null && !b_mouseonhostexecuted)
                                {
                                    this.OnDockingManagerMouseMove(host, e);
                                    this.OnMouseMoveOnHost(host, e);
                                }
                                testresult = null;
                                floathost = null;
                                dockinghost = null;
                            }
                        }
                    }
                }

                base.OnMouseMove(e);
            }
        }

        /// <summary>
        /// Invoked when an unhandled LostMouseCapture�attached event reaches an element in its route that is derived from this class.
        /// </summary>
        /// <param name="e">The MouseEventArgs that contains the event data.</param>
        protected override void OnLostMouseCapture(MouseEventArgs e)
        {
            if (null != m_draggedElement && null != m_hostUnderMouse)
            {
                DirectTabPanel panel = VisualUtils.FindDescendant(m_hostUnderMouse as Visual, typeof(DirectTabPanel)) as DirectTabPanel;
                bool canexecute = panel != null ? !panel.IsMouseCaptured : true;
                if (canexecute)
                {
                    if (IsVS2010DraggingEnabled)
                    {
                        CompleteDocking(e.GetPosition(m_hostUnderMouse), true, false);
                    }
                    else
                    {
                        CompleteDocking(true);
                    }
                }
            }

            base.OnLostMouseCapture(e);
        }

        /// <summary>
        /// Invoked when an unhandled MouseLeftButtonDown�routed event is raised on this element.
        /// </summary>
        /// <param name="e">The MouseButtonEventArgs that contains the event data. The event data reports that the mouse button was released.</param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (e.StylusDevice == null || e.StylusDevice != null)
            {
                base.OnMouseLeftButtonDown(e);

                if (!BrowserInteropHelper.IsBrowserHosted)
                {
                    ActivateParent();
                }

                if (e.OriginalSource is FrameworkElement)
                {
                    FrameworkElement originalSource = (FrameworkElement)e.OriginalSource;
                    DependencyObject templateParent = originalSource.TemplatedParent;

                    if (null != originalSource)
                    {
                        //MT 2261 - Auto Hide panel issues.
                        SidePanel sidepanel = VisualUtils.FindAncestor(e.OriginalSource as Visual, typeof(SidePanel)) as SidePanel;

                        if (sidepanel == null && !UsePopupAutoHidePreview)
                        {
                            //originalSource.Focusable = true;
                            // originalSource.Focus();
                            HideSidePanelItems();
                        }
                    }

                    if (!(templateParent is Splitter))
                    {
                        FloatWindowBorder floatBorder = templateParent as FloatWindowBorder;

                        if (null == floatBorder || FloatWindowBorderMode.Header == floatBorder.BorderMode)
                        {
                            Point point = e.GetPosition(this);
                            m_pointMouseStartPos = PermissionHelper.GetSafePointToScreen(this, point);
                            m_isPointKnown = false;
                        }
                    }
                }
            }
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
        }
        /// <summary>
        /// Invoked when an unhandled MouseEnter�attached event is raised on this element.
        /// </summary>
        /// <param name="e">The MouseEventArgs that contains the event data.</param>
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            if (e.StylusDevice == null || e.StylusDevice != null)
            {
                HitTestResult hitTest = VisualTreeHelper.HitTest(this, e.GetPosition(this));

                if (null != hitTest)
                {
                    if (m_managerDragPreview != null)
                    {
                        m_managerDragPreview.IsLowPerform = true;
                    }
                }
                MouseEnter = true;
                base.OnMouseEnter(e);
            }
        }

        /// <summary>
        /// Invoked when an unhandled MouseLeave�attached event is raised on this element.
        /// </summary>
        /// <param name="e">The MouseEventArgs that contains the event data.</param>
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            if (e.StylusDevice == null || e.StylusDevice != null)
            {
                if (!(BrowserInteropHelper.IsBrowserHosted && IsDragging) && m_managerDragPreview != null)
                {
                    m_managerDragPreview.IsLowPerform = false;
                    m_managerDragPreview.HideDockPreview();
                    this.m_managerDragPreview.HideDockPreviewMainButton(true);
                }
                MouseEnter = false;
                base.OnMouseLeave(e);
            }
        }

        internal void HideSidePanelItems()
        {
            foreach (SidePanel panel in m_primaryChild.GetSidePanelList())
            {
                panel.HideContent();
            }
        }

        /// <summary>
        /// Determines whether [contains not allowed state element] [the specified element].
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="state">The state.</param>
        /// <returns>
        /// <c>true</c> if [contains not allowed state element] [the specified element]; otherwise, <c>false</c>.
        /// </returns>
        internal static bool ContainsNotAllowedStateElement(FrameworkElement element, DockState state)
        {
            if (element == null)
            {
                return false;
            }

            DockingManager owner = DockingManager.ResolveManager(element);
            DockState currentState = DockingManager.GetState(element);
            if(currentState!=state)
            return ContainsNotAllowedStateElement(element, owner, currentState, state);
            return false;
        }

        /// <summary>
        /// Determines whether [contains not allowed state element] [the specified element].
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="owner">The owner.</param>
        /// <param name="currentState">State of the current.</param>
        /// <param name="desiredState">State of the desired.</param>
        /// <returns>
        /// <c>true</c> if [contains not allowed state element] [the specified element]; otherwise, <c>false</c>.
        /// </returns>
        private static bool ContainsNotAllowedStateElement(FrameworkElement element, DockingManager owner, DockState currentState, DockState desiredState)
        {
            bool bContains = false;
            if (DockingManager.CanChangeState(element, desiredState))
            {
                List<FrameworkElement> siblings = owner.FindSiblings(element, currentState, false);

                foreach (FrameworkElement sibling in siblings)
                {
                    if (!DockingManager.CanChangeState(sibling, desiredState))
                    {
                        bContains = true;
                        break;
                    }
                }

                if (!bContains)
                {
                    foreach (FrameworkElement sibling in siblings)
                    {
                        bContains = ContainsNotAllowedStateElement(sibling, owner, currentState, desiredState);

                        if (bContains)
                        {
                            break;
                        }
                    }
                }
            }
            else
            {
                bContains = true;
            }

            return bContains;
        }

        /// <summary>
        /// Gets the index of the target.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="state">The state.</param>
        /// <returns>return index.</returns>
        private int GetTargetIndex(FrameworkElement target, DockState state)
        {
            bool bIsNullTarget = null == target;
            int iIndex = (!bIsNullTarget) ? Children.Count - 1 : 0;
            if (!bIsNullTarget && !string.IsNullOrEmpty(target.Name))
            {
                iIndex = DockingManager.GetIndex(target, state);
            }

            return iIndex;
        }

        /// <summary>
        /// Sets the index of the child.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="state">The state.</param>
        /// <param name="index">The index.</param>
        private void SetChildIndex(DependencyObject element, DockState state, int index)
        {
            int oldIndex = DockingManager.GetIndex(element, state);

            if (oldIndex != index)
            {
                int shift = (oldIndex > index) ? 1 : -1;

                foreach (FrameworkElement child in Children)
                {
                    int childIndex = DockingManager.GetIndex(child, state);
                    if ((oldIndex > index) && (childIndex >= index) && (childIndex < oldIndex) || (oldIndex < index) && (childIndex <= index) && (childIndex > oldIndex))
                    {
                        DockingManager.SetIndex(child, state, childIndex + shift);
                    }
                }

                DockingManager.SetIndex(element, state, index);
            }
        }

        /// <summary>
        /// Corrects the tab orders.
        /// </summary>
        /// <param name="dockRecord">The dock record.</param>
        private void CorrectTabOrders(DockPreviewRecord dockRecord)
        {
            if (dockRecord.Side == DockSide.Tabbed)
            {
                DockState state = DockingManager.GetState(m_draggedElement);
                List<FrameworkElement> tabs = FindSiblingsSafe(m_draggedElement, state);
                tabs.Add(m_draggedElement);

                List<FrameworkElement> dockedTabs = new List<FrameworkElement>();

                foreach (FrameworkElement tab in tabs)
                {
                    if (DockState.Float == DockingManager.GetState(tab))
                    {
                        dockedTabs.Add(tab);
                    }
                }

                ProcessTabOrders(this, dockedTabs, dockRecord.TargetElement.Name, dockRecord.State, !m_bLoadPreview);
            }
        }

        /// <summary>
        /// Called when [docking manager mouse move].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        internal void OnDockingManagerMouseMove(object sender, InputEventArgs e)
        {

            this.Focus();
            Keyboard.Focus(this);
            if (UseAdornerFloatWindow)
            {
                SearchHostUnderMouse(e);
            }
            if (UseNativeFloatWindow)
            {
                if (m_draggedElement != null)
                {
                    DockInfoInternal info = DockingManager.GetDockInfo(m_draggedElement);
                    if (info != null && info.NativeWindow != null && !info.NativeWindow.m_mouseLeftButtonDown)
                    {
                        DockedElementTabbedHost hostUnderMouse = GetHostUnderMouse(e);
                        if (hostUnderMouse != null)
                        {
                            DocumentTabControl tabcontrol = VisualUtils.FindDescendant(this as Visual, typeof(DocumentTabControl)) as DocumentTabControl;
                            if (tabcontrol != null && tabcontrol.Items.Count > 0)
                            {
                                TabItemExt tabItemExt = tabcontrol.Items[0] as TabItemExt;
                                if (tabItemExt != null)
                                {
                                    HeaderPanel headerpanel = VisualUtils.FindAncestor(tabItemExt as Visual, typeof(HeaderPanel)) as HeaderPanel;
                                    if (headerpanel != null)
                                    {
                                        Point p = Mouse.GetPosition(headerpanel);
                                        HitTestResult result1 = VisualTreeHelper.HitTest(headerpanel as Visual, p);
                                        if (result1 != null && this.m_HeaderPanelMouseEventArgs != null)
                                        {
                                            m_HeaderPanelCallback = true;
                                            this.OnMouseMoveOnHeaderPanel(headerpanel, this.m_HeaderPanelMouseEventArgs);
                                            m_HeaderPanelCallback = false;
                                        }
                                        else if (this.m_HeaderPanelMouseEventArgs != null)
                                            this.OnMouseLeaveOnHeaderPanel(headerpanel, this.m_HeaderPanelMouseEventArgs);
                                    }
                                }
                            }
                            OnMouseMoveOnHost(hostUnderMouse, e);
                        }
                    }
                }
            }

            bool b_ShouldHide = false;
            if (m_hostUnderMouse != null)
            {
                Point point = new Point(0, 0);
                if (e is MouseEventArgs)
                    point = (e as MouseEventArgs).GetPosition(m_hostUnderMouse);
#if !SyncfusionFramework3_5
                else if (e is TouchEventArgs)
                    point = (e as TouchEventArgs).GetTouchPoint(m_hostUnderMouse).Position;
#endif

                DockPreviewRecord? record = m_managerDragPreview.FindDockingPlace(m_draggedElement, m_hostUnderMouse, point);
                ProcessingCreateDockPreview(e);
                b_ShouldHide = (record != null) ? ((m_managerDragPreview.m_previousdockside != DockSide.None) ? m_managerDragPreview.m_previousdockside != record.Value.Side : false) : true;
            }
            bool _flag = false;
            if (m_draggedElement != null)
            {
                DockInfoInternal info = DockingManager.GetDockInfo(m_draggedElement);
                if (info != null && info.NativeWindow != null)
                {
                    _flag = info.NativeWindow.m_mouseLeftButtonDown;
                }
            }
            if (!_flag)
            {
                PrepareHostPosition(e);
            }
            if (!UseAdornerDockPreview)
            {
                Point mousepoint = Mouse.GetPosition(this);
                HitTestResult testresult = VisualTreeHelper.HitTest(this, mousepoint);
                HeaderPanel panel = (testresult != null) ? VisualUtils.FindAncestor(testresult.VisualHit as Visual, typeof(HeaderPanel)) as HeaderPanel : null;

                if (panel != null)
                {
                    m_mousemoveonheaderpanel = true;
                }
                else
                {
                    m_mousemoveonheaderpanel = false;
                    if (b_ShouldHide)
                    {
                        m_managerDragPreview.HideDockPreview();
                    }
                }
            }
            else
            {
                DocumentTabControl tabcontrol = VisualUtils.FindDescendant(this as Visual, typeof(DocumentTabControl)) as DocumentTabControl;
                if (tabcontrol != null && tabcontrol.Items.Count > 0)
                {
                    TabItemExt tabItemExt = tabcontrol.Items[0] as TabItemExt;
                    if (tabItemExt != null)
                    {
                        HeaderPanel headerpanel = VisualUtils.FindAncestor(tabItemExt as Visual, typeof(HeaderPanel)) as HeaderPanel;
                        if (headerpanel != null)
                        {
                            Point p = Mouse.GetPosition(headerpanel);
                            HitTestResult result1 = VisualTreeHelper.HitTest(headerpanel as Visual, p);
                            if (result1 != null)
                            {
                                m_mousemoveonheaderpanel = true;
                            }
                            else
                            {
                                if (b_ShouldHide)
                                {
                                    m_managerDragPreview.HideDockPreview();
                                }
                            }
                        }
                        else
                        {
                            if (b_ShouldHide)
                            {
                                m_managerDragPreview.HideDockPreview();
                            }
                        }
                    }
                }
            }

        }

        protected override void OnDragOver(DragEventArgs e)
        {
            base.OnDragOver(e);
        }

        /// <summary>
        /// Gets the window under mouse.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        /// <returns>Return AdornerFloatWindow value</returns>
        private AdornerFloatWindow GetWindowUnderMouse(InputEventArgs e)
        {
            AdornerFloatWindow windowUnderMouse = null;
            IWindow draggedWindow = DockingManager.GetDockInfo(m_draggedElement).FloatingWindow;

            foreach (AdornerFloatWindow window in m_AdornerWindows)
            {
                if (window != null && window.IsOpen && draggedWindow != window)
                {
                    if (IsPointInsideElement(window))
                    {
                        windowUnderMouse = window;
                    }
                }
            }

            return windowUnderMouse;
        }

        /// <summary>
        /// Gets the host under mouse.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        /// <returns>Return DockedElementTabbedHost value.</returns>
        private DockedElementTabbedHost GetHostUnderMouse(InputEventArgs e)
        {
            DockedElementTabbedHost hostUnderMouse = null;
            List<DockedElementTabbedHost> hosts = new List<DockedElementTabbedHost>(m_usedHosts);
            hosts.Add(GetDocumentContainerHost());

            foreach (DockedElementTabbedHost host in hosts)
            {
                if (host != null && host.Visibility == Visibility.Visible)
                {
                    if (IsPointInsideElement(host))
                    {
                        hostUnderMouse = host;

                        if (host.State == DockState.Float)
                        {
                            break;
                        }
                    }
                }
            }

            return hostUnderMouse;
        }


        internal DockedElementTabbedHost GetHostMouse()
        {
            DockedElementTabbedHost hostUnderMouse = null;
            List<DockedElementTabbedHost> hosts = new List<DockedElementTabbedHost>(m_usedHosts);
            hosts.Add(GetDocumentContainerHost());

            foreach (DockedElementTabbedHost host in hosts)
            {
                if (host != null && host.Visibility == Visibility.Visible)
                {
                    if (IsPointInsideElement(host))
                    {
                        hostUnderMouse = host;

                        if (host.State == DockState.Float)
                        {
                            break;
                        }
                    }
                }
            }

            return hostUnderMouse;
        }
        /// <summary>
        /// Searches the host under mouse.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        private void SearchHostUnderMouse(InputEventArgs e)
        {
            AdornerFloatWindow windowUnderMouse = GetWindowUnderMouse(e);
            bool hostSearch = true;

            if (windowUnderMouse != null)
            {
                if (IsPointInsideElement(windowUnderMouse.Header) &&
                    !windowUnderMouse.IsMultiHostsContainer && IsDragging &&
                    !DockingManager.GetNoDock(windowUnderMouse.PrimaryElement))
                {
                    ShowDockPreviewInternal(windowUnderMouse);
                    hostSearch = false;
                }
            }

            if (hostSearch)
            {
                DockedElementTabbedHost hostUnderMouse = GetHostUnderMouse(e);

                if (hostUnderMouse != null)
                {
                    OnMouseMoveOnHost(hostUnderMouse, e);
                }
            }
        }

        /// <summary>
        /// Determines whether [is point inside element] [the specified element].
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>
        /// <c>true</c> if [is point inside element] [the specified element]; otherwise, <c>false</c>.
        /// </returns>
        internal bool IsPointInsideElement(UIElement element)
        {
            Point pt = Mouse.GetPosition(element);
            return pt.X >= 0 && pt.Y >= 0 && pt.X < element.RenderSize.Width && pt.Y < element.RenderSize.Height;
        }

        internal void PrepareNativeWindowPosition(MouseEventArgs e)
        {
            if (e.StylusDevice == null || e.StylusDevice != null)
            {
                if (null != m_draggedElement)
                {
                    DockInfoInternal info = DockingManager.GetDockInfo(m_draggedElement);
                    NativeFloatWindow window = info.NativeWindow;
                    if (DraggingType == DraggingType.NormalDragging)
                    {
                        if (window != null && window.IsOpen)
                        {
                            SetNativeStartPos(window);
                            Rect rect = GetNativeLocationRect(window, e);
                            if (!m_placementrectset)
                            {
                                if ((Mouse.DirectlyOver as UIElement) != null)
                                {
                                    rect.X = VisualUtils.PointToScreen(Mouse.DirectlyOver as UIElement, new Point(0, 0)).X;
                                }
                                m_placementrectset = true;
                            }
                            window.IsDragging = true;
                            window.HitTestDisabled = true;
                            DockingManager.SetFloatingWindowRect(m_draggedElement, rect);
                        }
                    }
                }
            }
        }

        bool isTouchDown = false;
        bool newOnHeaderPoint = false;
        Point onHeaderPoint = new Point();
        /// <summary>
        /// Prepares the host position.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        internal void PrepareHostPosition(InputEventArgs e)
        {
            if (null != m_draggedElement)
            {
                DockInfoInternal info = DockingManager.GetDockInfo(m_draggedElement);
                if (info.DockingManager.UseNativeFloatWindow)
                {
                    NativeFloatWindow window = info.NativeWindow;
                    if ((DraggingType == DraggingType.NormalDragging || !m_currentDragPopup.IsOpen) && (Mouse.LeftButton==MouseButtonState.Pressed))
                    {
                        if (window != null && window.IsOpen)
                        {
                            SetNativeStartPos(window);
                            Rect rect = GetNativeLocationRect(window, e);
                            if (!m_placementrectset)
                            {
                                if ((Mouse.DirectlyOver as UIElement) != null && (Mouse.DirectlyOver as UIElement) != this && (Mouse.DirectlyOver as UIElement).IsVisible)
                                {
                                    rect.X = VisualUtils.PointToScreen(Mouse.DirectlyOver as UIElement, new Point(0, 0)).X;
                                }
                                m_placementrectset = true;
                            }
                            window.IsDragging = true;
                            window.HitTestDisabled = true;
                            if (m_draggedElement != null)
                            DockingManager.SetFloatingWindowRect(m_draggedElement, rect);
                        }
                        else
                        {
                            Debug.WriteLineIf(window != null, "Drag started but window is not opened.");
                        }
                    }
                    else if (m_currentDragPopup.IsOpen)
                    {
                        SetStartPos(CurrentDragPopup);
                        CurrentDragPopup.PlacementRectangle = GetLocationRect(CurrentDragPopup, e);
                    }   
                }
                else
                {
                    IWindow window = info.FloatingWindow;


                    if (DraggingType == DraggingType.NormalDragging)
                    {
                        if (window != null && window.IsOpen)
                        {
                            SetStartPos(window);
                            Rect rect = GetLocationRect(window, e);

                            if (!newOnHeaderPoint && IsTouchEnabled && isTouchDown)
                            {
                                onHeaderPoint = Mouse.GetPosition(info.FloatingWindow.Child);
                                newOnHeaderPoint = true;
                            }
                            rect.X = rect.X - onHeaderPoint.X;
                            rect.Y = rect.Y - onHeaderPoint.Y;
                            if (!m_placementrectset)
                            {                               
                                m_placementrectset = true;
                            }
                            window.IsDragging = true;
                            window.HitTestDisabled = true;

                            if (window.AllowsTransparency && IsntFrozenChild())
                            {
                                if (window != null && window.PrimaryElement != null)
                                {
                                    window.Opacity = DockingManager.GetNoDock(window.PrimaryElement) ? 1.0 : 0.7;
                                }
                            }

                            DockingManager.SetFloatingWindowRect(m_draggedElement, rect);
                            AdornerWindowsLayoutPanel.SetPlacementRactangle(window as DependencyObject, rect);
                        }
                        else
                        {
                            Debug.WriteLineIf(window != null, "Drag started but window is not opened.");
                        }
                    }
                    else
                    {
                        SetStartPos(CurrentDragPopup);
                        CurrentDragPopup.PlacementRectangle = GetLocationRect(CurrentDragPopup, e);
                    }
                }
            }
        }
        internal void SetNativeStartPos(NativeFloatWindow elementForReadout)
        {
            if (!m_isPointKnown)
            {
                if (double.IsNegativeInfinity(m_pointMouseStartPos.X)
                    && double.IsNegativeInfinity(m_pointMouseStartPos.Y))
                {
                    if (DockingManager.ResolveManager(elementForReadout.PrimaryElement) != null
                        && DockingManager.ResolveManager(elementForReadout.PrimaryElement).IsVS2010DraggingEnabled
                        && DockingManager.GetPreviousState(elementForReadout.PrimaryElement) == DockState.Document)
                    {
                        double xpoint = Mouse.GetPosition(DockingManager.GetFloatHost(elementForReadout.PrimaryElement)).X;
                        double ypoint = Mouse.GetPosition(DockingManager.GetFloatHost(elementForReadout.PrimaryElement)).Y;
                        if (m_hostUnderMouse != null)
                        {
                            xpoint = Mouse.GetPosition(Mouse.DirectlyOver as UIElement).X;
                            ypoint = Mouse.GetPosition(Mouse.DirectlyOver as UIElement).Y;
                        }
                        m_pointMouseStartPos = new Point(xpoint, 5);
                    }
                    else
                    {
                        m_pointMouseStartPos = new Point(5, -5);
                    }
                }
                else
                {
                    if (UseAdornerFloatWindow)
                    {
                          m_pointMouseStartPos = new Point(elementForReadout.PrimaryElement.RenderSize.Width / 2, 10);
                    }
                    else
                    {
                        m_pointMouseStartPos = PermissionHelper.HasUnmanagedCodePermission
                            ? GetSafePointFromScreen(elementForReadout.PrimaryElement, m_pointMouseStartPos)
                            : EMPTY_POINT;
                    }
                }

                ////CorrectPosistionRelativelyToBrowser();
                m_isPointKnown = true;
            }
        }
        /// <summary>
        /// Sets the start pos.
        /// </summary>
        /// <param name="elementForReadout">The element for readout.</param>
        private void SetStartPos(IWindow elementForReadout)
        {
            if (!m_isPointKnown)
            {
                if (double.IsNegativeInfinity(m_pointMouseStartPos.X)
                    && double.IsNegativeInfinity(m_pointMouseStartPos.Y))
                {
                    if (DockingManager.ResolveManager(elementForReadout.PrimaryElement) != null
                        && DockingManager.ResolveManager(elementForReadout.PrimaryElement).IsVS2010DraggingEnabled
                        && DockingManager.GetPreviousState(elementForReadout.PrimaryElement) == DockState.Document)
                    {
                        double xpoint = Mouse.GetPosition(DockingManager.GetFloatHost(elementForReadout.PrimaryElement)).X;
                        double ypoint = Mouse.GetPosition(DockingManager.GetFloatHost(elementForReadout.PrimaryElement)).Y;
                        if (m_hostUnderMouse != null)
                        {
                            xpoint = Mouse.GetPosition(Mouse.DirectlyOver as UIElement).X;
                            ypoint = Mouse.GetPosition(Mouse.DirectlyOver as UIElement).Y;
                        }
                        m_pointMouseStartPos = new Point(xpoint, 5);
                    }
                    else
                    {
                        m_pointMouseStartPos = new Point(5, 5);
                    }
                }
                else
                {
                    if (UseAdornerFloatWindow)
                    {
                        ////m_PointMouseStartPos = Mouse.GetPosition( elementForReadout as ContentControl );
                        ////m_PointMouseStartPos.Y = 10;
                        m_pointMouseStartPos = new Point(elementForReadout.Child.RenderSize.Width / 2, 10);
                    }
                    else
                    {
                        m_pointMouseStartPos = PermissionHelper.HasUnmanagedCodePermission
                            ? GetSafePointFromScreen(elementForReadout.Child, m_pointMouseStartPos)
                            : EMPTY_POINT;
                    }
                }

                ////CorrectPosistionRelativelyToBrowser();
                m_isPointKnown = true;
            }
        }

        /// <summary>
        /// Corrects the position relatively to browser.
        /// </summary>
        private void CorrectPosistionRelativelyToBrowser()
        {
            if (BrowserInteropHelper.IsBrowserHosted)
            {
                Visual root = VisualUtils.FindRootVisual(this);
                Point rootOffset = root.PointToScreen(new Point());
                m_pointMouseStartPos.X -= rootOffset.X;
                m_pointMouseStartPos.Y += rootOffset.Y;
            }
        }

        /// <summary>
        /// Checks the is windows XP.
        /// </summary>
        /// <returns></returns>
        internal bool CheckIsWindowsXP()
        {
            System.OperatingSystem osInfo = System.Environment.OSVersion;

            switch (osInfo.Platform)
            {
                case System.PlatformID.Win32NT:
                    switch (osInfo.Version.Major)
                    {
                        case 5:
                            if (osInfo.Version.Minor == 0)
                                return false;
                            else
                                return true;                            
                    }
                    break;
            }
            return false;
        }

        internal Rect GetNativeLocationRect(NativeFloatWindow window, InputEventArgs e)
        {
            Rect rect = !UseAdornerFloatWindow
                ? new Rect(window.InternalPlacementRect.Location, new Size(window.Width, window.Height))
                : AdornerWindowsLayoutPanel.GetPlacementRactangle(window as DependencyObject);

            if ((double.IsInfinity(m_pointMouseStartPosOffset.X) && double.IsInfinity(m_pointMouseStartPosOffset.Y)) || !CheckIsWindowsXP())
            {
                Point currentPos = new Point(0,0);
                if (e is MouseEventArgs)
                    currentPos = (e as MouseEventArgs).GetPosition(window.PrimaryElement);
#if !SyncfusionFramework3_5
                else if (e is TouchEventArgs)
                    currentPos = ((e as TouchEventArgs).GetTouchPoint(window.PrimaryElement) as TouchPoint).Position;
#endif

                double xOffset = currentPos.X - m_pointMouseStartPos.X;
                rect.X += (FlowDirection == FlowDirection.RightToLeft) ? -xOffset : xOffset;
                rect.Y += currentPos.Y - m_pointMouseStartPos.Y;
            }
            else
            {
                rect.X = PermissionHelper.GetSafePointToScreen(this, Mouse.GetPosition(this)).X - m_pointMouseStartPosOffset.X;
                rect.Y = PermissionHelper.GetSafePointToScreen(this, Mouse.GetPosition(this)).Y - m_pointMouseStartPosOffset.Y;
                m_placementrectset = true;
            }
            return rect;
        }
        /// <summary>
        /// Gets the location rect.
        /// </summary>
        /// <param name="popup">The popup.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        /// <returns> Return Rect value.</returns>
        private Rect GetLocationRect(IWindow popup, InputEventArgs e)
        {
            Rect rect = !UseAdornerFloatWindow
                ? new Rect(popup.PlacementRectangle.Location, new Size(popup.Width, popup.Height))
                : AdornerWindowsLayoutPanel.GetPlacementRactangle(popup as DependencyObject);

            if ((double.IsInfinity(m_pointMouseStartPosOffset.X) && double.IsInfinity(m_pointMouseStartPosOffset.Y)) || !CheckIsWindowsXP())
            {
                Point currentPos = new Point(0, 0);
                if (e is MouseEventArgs)
                    currentPos = (e as MouseEventArgs).GetPosition(popup.Child);
#if !SyncfusionFramework3_5
                else if (e is TouchEventArgs)
                    currentPos = ((e as TouchEventArgs).GetTouchPoint(popup.Child) as TouchPoint).Position;
#endif
                double xOffset = currentPos.X - m_pointMouseStartPos.X;
                rect.X += (FlowDirection == FlowDirection.RightToLeft) ? -xOffset : xOffset;
                rect.Y += currentPos.Y - m_pointMouseStartPos.Y;
            }
            else
            {
                rect.X = PermissionHelper.GetSafePointToScreen(this, Mouse.GetPosition(this)).X - m_pointMouseStartPosOffset.X;
                rect.Y = PermissionHelper.GetSafePointToScreen(this, Mouse.GetPosition(this)).Y - m_pointMouseStartPosOffset.Y;
                m_placementrectset = true;
            }
            return rect;
        }
        /// <summary>
        /// called when mouse down on host
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal void OnMouseDownOnHost(object sender, InputEventArgs e)
        {
            if (this.Dispatcher.CheckAccess())
            {
                if (!BrowserInteropHelper.IsBrowserHosted)
                {
                    DockedElementTabbedHost tab = (DockedElementTabbedHost)sender;
                    if (tab.DockingManager.Equals(this))
                    {
                        if (!UseNativeFloatWindow)
                        {
                            ActivateParent();
                        }
                    }
                }
            }
           
        }

        /// <summary>
        /// Called when [mouse down on header].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        internal void OnMouseDownOnHeader(object sender, InputEventArgs e)
        {
            if (this.Dispatcher.CheckAccess())
            {
                if (!BrowserInteropHelper.IsBrowserHosted)
                {
                    FloatWindowBorder border = (FloatWindowBorder)sender;

                    if (border.ParentWindow.DockingManager.Equals(this))
                    {
                        ActivateParent();
                    }
                }
            }
        }

        internal void OnMouseUpOnHost(object sender)
        {
            if (this.Dispatcher.CheckAccess())
            {
                if (IsDragging)
                {
                    if (DraggingType.NormalDragging != DraggingType && m_prevHostUnderMouse != null)
                    {
                        m_hostUnderMouse = m_prevHostUnderMouse;
                    }
                    m_placementrectset = false;
                    DoDocking();
                }
            }
        }
       
        /// <summary>
        /// Called when Key down
        /// </summary>
        /// <param name="e"></param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (UseNativeFloatWindow && m_managerDragPreview != null && m_draggedElement != null && m_hostUnderMouse != null)
            {
                DockPreviewRecord? record = m_managerDragPreview.FindDockingPlace(m_draggedElement, m_hostUnderMouse, Mouse.GetPosition(m_hostUnderMouse));
                if (record != null)
                {
                    if (!m_hostUnderMouse.TabChildren.Contains(m_draggedElement) ||
                        (DraggingType.NormalDragging != DraggingType && 1 == m_hostUnderMouse.TabChildren.Count &&
                        m_draggedElement != m_hostUnderMouse.TabChildren[0]))
                    {
                        CompleteDocking(Mouse.GetPosition(m_hostUnderMouse), true, false);
                    }
                    else if (IsDragItem(m_hostUnderMouse.TabChildren))
                    {
                        CompleteDocking(Mouse.GetPosition(m_hostUnderMouse), true, true);
                    }
                    else
                    {
                        CompleteDocking();
                    }
                }
                else
                {
                    m_managerDragPreview.HideDockPreview();
                    m_managerDragPreview.HideDockPreviewMainButton(true);
                }
            }

            base.OnKeyDown(e);
           
        }

        /// <summary>
        /// Called when [mouse up on host].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        internal void OnMouseUpOnHost(object sender, InputEventArgs e)
        {
            //if (!BrowserInteropHelper.IsBrowserHosted)
            //{
            //    ActivateParent();
            //}
            if (this.Dispatcher.CheckAccess())
            {
                if (IsDragging)
                {
                    m_hostUnderMouse = (DockedElementTabbedHost)sender;

                    if (DraggingType.NormalDragging != DraggingType && m_prevHostUnderMouse != null)
                    {
                        m_hostUnderMouse = m_prevHostUnderMouse;
                    }
                    if (DraggingType.NormalDragging != DraggingType && m_draggedElement != null && extract)
                    {
                        ExtractElementToWindow(m_draggedElement, extractmode, true);
                        extract = false;
                    }
                    m_placementrectset = false;
                    DoDocking(e);
                }
            }
        }

        /// <summary>
        /// Draws drag providers for host under mouse when dragging on header of the dock window is done.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        internal void OnMouseEnterOnHost(object sender, InputEventArgs e)
        {
            DockedElementTabbedHost host = e.Source as DockedElementTabbedHost;

            if (this.Dispatcher.CheckAccess())
            {
                if (null != host)
                {
                    ////bool bDock = CanDock( host.HostedElement );

                    if (IsDragging && CanDock(host.HostedElement))
                    {
                        DockInfoInternal info = DockingManager.GetDockInfo(host.HostedElement);

                        if (info != null && info.FloatingWindow != null && !DockingManager.GetNoDock(m_draggedElement))
                        {
                            info.FloatingWindow.SetWindowOnTop();
                        }

                        m_hostUnderMouse = null;
                    }
                }
            }
           
        }

        /// <summary>
        /// Gets the list of elements.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="state">The state.</param>
        /// <returns>Return FrameWork list value.</returns>
        internal List<FrameworkElement> GetListOfElements(FrameworkElement element, DockState state)
        {
            List<FrameworkElement> list = new List<FrameworkElement> { element };
            GetListOfElements(element, state, list);
            return list;
        }

        /// <summary>
        /// Gets the list of elements.
        /// </summary>
        /// <param name="element">Represents the FrameworkElement.</param>
        /// <param name="state">Represents the DockState.</param>
        /// <param name="list">The FrameworkElement list.</param>
        private void GetListOfElements(FrameworkElement element, DockState state, ICollection<FrameworkElement> list)
        {
            List<FrameworkElement> siblings = FindSiblings(element, state, false);
            foreach (FrameworkElement item in siblings)
            {
                list.Add(item);
                GetListOfElements(item, state, list);
            }
        }

        internal Point mousestartpoint = new Point();       
        /// <summary>
        /// Called when [mouse move on TDI layout panel].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        internal void OnMouseMoveOnTDILayoutPanel(object sender, MouseEventArgs e)
        {
            if (e.StylusDevice == null || e.StylusDevice != null)
            {
                HeaderPanel headerpanel = VisualUtils.FindDescendant(this as Visual, typeof(HeaderPanel)) as HeaderPanel;
                Point mousepoint = Mouse.GetPosition(headerpanel);
                HitTestResult testresult = (headerpanel != null) ? VisualTreeHelper.HitTest(headerpanel, mousepoint) : null;
                if (this.Dispatcher.CheckAccess())
                {
                    if (IsVS2010DraggingEnabled && !IsDragging)
                    {
                        if (e.LeftButton == MouseButtonState.Pressed)
                        {
                            //if ((e.Source as DocumentTabControl) != null)
                            {
                                if (this.m_draggedElement != null && !m_dragstarted
                                    && DockingManager.GetState(this.m_draggedElement) == DockState.Document
                                    && DockingManager.GetCanFloat(this.m_draggedElement) && DockingManager.GetCanDrag(m_draggedElement))
                                {
                                    DockedElementTabbedHost host = DockingManager.GetTabbedHost(this.m_draggedElement, DockState.Float);
                                    if (host != null)
                                    {
                                        DockingManager owner = DockingManager.ResolveManager(this.m_draggedElement);
                                        bool vs2010drag = false;
                                        TabLayoutPanel tablayoutpanel = null;
                                        TabPanelAdv tabpanel = headerpanel.Children[0] as TabPanelAdv;
                                        tablayoutpanel = (tabpanel != null && tabpanel.Content is TabLayoutPanel) ? (tabpanel.Content as TabLayoutPanel) : (tabpanel != null && tabpanel.Content is ScrollViewer && (tabpanel.Content as ScrollViewer).Content is TabLayoutPanel) ? ((tabpanel.Content as ScrollViewer).Content as TabLayoutPanel) : null;                                        
                                        if (tablayoutpanel != null)
                                        {
                                            double _layoutpanelheight = new Rect(tablayoutpanel.RenderSize).Height;
                                            Point _currentmouseposition = e.GetPosition(tablayoutpanel);
                                            double _diffy = Math.Abs(mousestartpoint.Y - _currentmouseposition.Y);
                                            double _allowdragdiff = Math.Abs((_layoutpanelheight - 3) - mousestartpoint.Y);                                           
                                            if (_currentmouseposition.Y + 4 > _layoutpanelheight)
                                            {
                                                vs2010drag = true;
                                            }
                                            else if (_diffy > _allowdragdiff)
                                            {
                                                vs2010drag = true;
                                            }
                                        }
                                        if (owner != null && (testresult == null || (vs2010drag && m_draggedElement is HwndHost)))
                                        {
                                            owner.RemoveFromDocumentContainer(this.m_draggedElement);
                                            host.TabChildren.Add(this.m_draggedElement);
                                            DockingManager.StartDragging(host);
                                            m_dragstarted = false;
                                        }
                                        else if (owner != null && testresult != null)
                                        {
                                            UpdateTabItemPosition(headerpanel, e);
                                        }
                                    }
                                }
                                m_mousemoveonheaderpanel = false;
                                m_mousemoveonTabpaneladv = false;
                                m_mouseonheaderpanelindex = -1;
                                if (m_managerDragPreview != null)
                                {
                                    m_managerDragPreview.IsLowPerform = false;
                                    m_managerDragPreview.HideDockPreview();
                                }
                            }
                        }
                        m_mousemoveontdilayoutpanel = true;
                    }
                }
            }
        }

        /// <summary>
        /// Called when [mouse leave on TDI layout panel].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        internal void OnMouseLeaveOnTDILayoutPanel(object sender, MouseEventArgs e)
        {
            if (e.StylusDevice == null || e.StylusDevice != null)
            {
                if (this.Dispatcher.CheckAccess())
                {
                    if (IsVS2010DraggingEnabled)
                    {
                        m_mousemoveontdilayoutpanel = false;
                    }
                }
            }
        }

        /// <summary>
        /// Called when [mouse move on header panel].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        internal void OnMouseMoveOnHeaderPanel(object sender, MouseEventArgs e)
        {
            if (e.StylusDevice == null || e.StylusDevice != null)
            {
                if (this.Dispatcher.CheckAccess())
                {
                    if (m_hostUnderMouse != null && IsVS2010DraggingEnabled)
                    {
                        if (DockingManager.GetState(m_draggedElement) == DockState.Float)
                        {
                            if (UseNativeFloatWindow)
                            {
                                NativeFloatWindow nativewindow = DockingManager.GetNativeWindow(m_draggedElement);
                                if (nativewindow != null)
                                    m_mouseonheaderpaneltabareawidth = nativewindow.NativeHeader.DesiredSize.Width;

                                if (m_HeaderPanelCallback)
                                {
                                    TabPanelAdv tabpanel = VisualUtils.FindDescendant((sender as HeaderPanel) as Visual, typeof(TabPanelAdv)) as TabPanelAdv;
                                    if (tabpanel != null)
                                    {
                                        Point tabpanelpoint = Mouse.GetPosition(tabpanel);
                                        HitTestResult testresult = VisualTreeHelper.HitTest(tabpanel as Visual, tabpanelpoint);
                                        if (testresult != null)
                                        {
                                            if (!m_mousemoveonTabpaneladv)
                                            {
                                                double increment = 0.0;
                                                m_mouseonheaderpanelposition = e.GetPosition(tabpanel as IInputElement).X;
                                                if (tabpanel.tab != null
                                                    && tabpanel.tab.newtabParent != null)
                                                {
                                                    m_mouseonheaderpanelindex = tabpanel.tab.newtabParent.Items.Count;
                                                }
                                                else
                                                {
                                                    m_mouseonheaderpanelindex = 0;
                                                }
                                                m_mousemoveonTabpaneladv = true;
                                                foreach (TabItemExt tabitem in tabpanel.tab.newtabParent.Items)
                                                {
                                                    increment += tabitem.ActualWidth;
                                                }
                                                if (m_mouseonheaderpanelposition > increment)
                                                {
                                                    m_mouseonheaderpanelposition = increment;
                                                }
                                            }
                                        }
                                    }
                                    TabLayoutPanel tablayoutpanel = VisualUtils.FindDescendant((sender as HeaderPanel) as Visual, typeof(TabLayoutPanel)) as TabLayoutPanel;
                                    if (tablayoutpanel != null)
                                    {
                                        Point tablayoutpanelpoint = Mouse.GetPosition(tablayoutpanel);
                                        HitTestResult testresult1 = VisualTreeHelper.HitTest(tablayoutpanel as Visual, tablayoutpanelpoint);
                                        if (testresult1 != null)
                                        {
                                            if (tablayoutpanel.Children.Count > 0 && ((tablayoutpanel.Children[0] as TabItemExt).Parent as DocumentTabControl) != null)
                                            {
                                                DocumentTabControl tabcontrol = ((tablayoutpanel.Children[0] as TabItemExt).Parent as DocumentTabControl);
                                                if (m_mouseonheaderpanelindex != tabcontrol.m_mousemoveonitemindex || m_mousemoveonTabpaneladv)
                                                {
                                                    foreach (var item in tabcontrol.Items)
                                                    {
                                                        TabItemExt tabitem = item as TabItemExt;
                                                        Point tabitempoint = Mouse.GetPosition(tabitem);
                                                        HitTestResult tabitemresult = VisualTreeHelper.HitTest(tabitem as Visual, tabitempoint);
                                                        if (tabitemresult != null)
                                                        {
                                                            m_mouseonheaderpanelindex = tabcontrol.Items.IndexOf(item);
                                                            break;
                                                        }
                                                    }
                                                    m_mouseonheaderpanelposition = (e.GetPosition(tablayoutpanel as IInputElement).X - e.GetPosition((tablayoutpanel.Children[m_mouseonheaderpanelindex] as TabItemExt) as IInputElement).X) + 6;
                                                    tabcontrol.m_mousemoveonitem = m_draggedElement;
                                                }
                                                m_mousemoveonTabpaneladv = false;
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                IWindow window = DockingManager.GetFloatWindow(m_draggedElement);
                                if (window != null)
                                    m_mouseonheaderpaneltabareawidth = window.Header.DesiredSize.Width;
                                if ((e.Source as TabPanelAdv) != null && ((e.Source as TabPanelAdv) as IInputElement) != null)
                                {
                                    if (!m_mousemoveonTabpaneladv)
                                    {
                                        double increment = 0.0;
                                        m_mouseonheaderpanelposition = e.GetPosition((e.Source as TabPanelAdv) as IInputElement).X;
                                        if ((e.Source as TabPanelAdv).tab != null
                                            && (e.Source as TabPanelAdv).tab.newtabParent != null)
                                        {
                                            m_mouseonheaderpanelindex = (e.Source as TabPanelAdv).tab.newtabParent.Items.Count;
                                        }
                                        else
                                        {
                                            m_mouseonheaderpanelindex = 0;
                                        }
                                        m_mousemoveonTabpaneladv = true;
                                        foreach (TabItemExt tabitem in (e.Source as TabPanelAdv).tab.newtabParent.Items)
                                        {
                                            increment += tabitem.ActualWidth;
                                        }
                                        if (m_mouseonheaderpanelposition > increment)
                                        {
                                            m_mouseonheaderpanelposition = increment;
                                        }
                                    }
                                }
                                if ((e.Source as TabLayoutPanel) != null && ((e.Source as TabLayoutPanel) as IInputElement) != null)
                                {
                                    if ((e.Source as TabLayoutPanel).Children.Count > 0
                                        && (((e.Source as TabLayoutPanel).Children[0] as TabItemExt).Parent as DocumentTabControl) != null)
                                    {
                                        DocumentTabControl tabcontrol = (((e.Source as TabLayoutPanel).Children[0] as TabItemExt).Parent as DocumentTabControl);
                                        if (m_mouseonheaderpanelindex != tabcontrol.m_mousemoveonitemindex || m_mousemoveonTabpaneladv)
                                        {
                                            m_mouseonheaderpanelposition = (e.GetPosition((e.Source as TabLayoutPanel) as IInputElement).X - e.GetPosition(((e.Source as TabLayoutPanel).Children[tabcontrol.m_mousemoveonitemindex] as TabItemExt) as IInputElement).X) + 6;
                                            m_mouseonheaderpanelindex = tabcontrol.m_mousemoveonitemindex;
                                            tabcontrol.m_mousemoveonitem = m_draggedElement;
                                        }
                                        m_mousemoveonTabpaneladv = false;
                                    }
                                }
                            }
                        }
                        m_mousemoveonheaderpanel = true;
                        if (!UseAdornerDockPreview && m_mouseonheaderpanelindex != -1 && m_managerDragPreview.m_headerpanelindex != -1 && m_mouseonheaderpanelindex != m_managerDragPreview.m_headerpanelindex)
                        {
                            if (m_managerDragPreview != null)
                            {
                                m_managerDragPreview.HideDockPreview();
                            }
                        }
                        FireDockProviderShownEvent(m_hostUnderMouse);
                        ProcessingCreateDockPreview(e);
                    }
                }
            }
        }

        /// <summary>
        /// Called when [mouse leave on header panel].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        internal void OnMouseLeaveOnHeaderPanel(object sender, MouseEventArgs e)
        {
            if (e.StylusDevice == null || e.StylusDevice != null)
            {
                if (this.Dispatcher.CheckAccess())
                {
                    if (e.LeftButton == MouseButtonState.Pressed)
                    {
                        if (this.IsVS2010DraggingEnabled && this.m_draggedElement != null
                            && !DockingManager.GetIsDragged(m_draggedElement) && !m_dragstarted
                            && DockingManager.GetState(this.m_draggedElement) == DockState.Document
                            && DockingManager.GetCanFloat(this.m_draggedElement) && DockingManager.GetCanDrag(m_draggedElement))
                        {
                            DockedElementTabbedHost host = DockingManager.GetTabbedHost(this.m_draggedElement, DockState.Float);
                            if (host != null)
                            {
                                DockingManager owner = DockingManager.ResolveManager(this.m_draggedElement);
                                if (owner != null && (!(m_draggedElement is HwndHost)))
                                {
                                    owner.RemoveFromDocumentContainer(this.m_draggedElement);
                                    host.TabChildren.Add(this.m_draggedElement);
                                    DockingManager.StartDragging(host);
                                    m_dragstarted = false;
                                }
                            }
                        }
                    }
                    if (this.IsVS2010DraggingEnabled)
                    {
                        int index = m_mouseonheaderpanelindex;
                        m_mousemoveonheaderpanel = false;
                        m_mousemoveonTabpaneladv = false;
                        m_mouseonheaderpanelindex = -1;
                        if (!UseAdornerDockPreview)
                        {
                            Point mousepoint = Mouse.GetPosition(this);
                            HitTestResult testresult = VisualTreeHelper.HitTest(this, mousepoint);
                            HeaderPanel panel = (testresult != null) ? VisualUtils.FindAncestor(testresult.VisualHit as Visual, typeof(HeaderPanel)) as HeaderPanel : null;
                            if (panel != null && index > 0)
                            {
                                m_mouseonheaderpanelindex = index;
                                m_mousemoveonheaderpanel = true;
                            }
                        }
                        else
                        {
                            DocumentTabControl tabcontrol = VisualUtils.FindDescendant(this as Visual, typeof(DocumentTabControl)) as DocumentTabControl;
                            if (tabcontrol != null && tabcontrol.Items.Count > 0)
                            {
                                TabItemExt tabItemExt = tabcontrol.Items[0] as TabItemExt;
                                if (tabItemExt != null)
                                {
                                    HeaderPanel headerpanel = VisualUtils.FindAncestor(tabItemExt as Visual, typeof(HeaderPanel)) as HeaderPanel;
                                    if (headerpanel != null)
                                    {
                                        Point p = Mouse.GetPosition(headerpanel);
                                        HitTestResult result1 = VisualTreeHelper.HitTest(headerpanel as Visual, p);
                                        if (result1 != null)
                                        {
                                            m_mousemoveonheaderpanel = true;
                                        }
                                        else
                                        {
                                        }
                                    }
                                }
                            }
                            if (m_managerDragPreview != null)
                            {
                                m_managerDragPreview.IsLowPerform = false;
                                m_managerDragPreview.HideDockPreview();
                            }
                        }
                    }
                }
            }
        }
        protected override void OnDragEnter(DragEventArgs e)
        {
            base.OnDragEnter(e);
            HideSidePanelItems(); 
        }

        /// <summary>
        /// Draws drag providers for host under mouse when dragging is done.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        internal void OnMouseMoveOnHost(object sender, InputEventArgs e)
        {
            DockedElementTabbedHost host = e.Source as DockedElementTabbedHost;
            
            if (this.Dispatcher.CheckAccess())
            {
                if (UseAdornerFloatWindow && host != null)
                {
                    return;
                }

                host = host == null ? sender as DockedElementTabbedHost : host;
                if (IsDragging && host.Visibility == Visibility.Visible && IsntSelf(host) && Mouse.LeftButton==MouseButtonState.Pressed)
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
                            DockSide elementside = DockingManager.GetSide(m_draggedElement, DockingManager.GetState(m_draggedElement));
                            DockAbility elementability = DockingManager.GetDockAbility(m_draggedElement);

                            if (AllowDock(elementability, elementside) || elementability == DockAbility.All)
                            {
                                if (!UseInteropCompatibilityMode)
                                {
                                    if (DraggingType.NormalDragging == DraggingType)
                                    {
                                        DockInfoInternal info = DockingManager.GetDockInfo(m_draggedElement);
                                        if (UseNativeFloatWindow)
                                        {
                                            m_holdList = GetContainerTabs(info.NativeWindow.Content as DockedElementsContainer);
                                            info.NativeWindow.IsOpen = false;
                                            ValidateNativeFloatWindow(info.NativeWindow);
                                        }
                                        else
                                        {
                                            m_holdList = GetContainerTabs(info.FloatingWindow.FloatChild as DockedElementsContainer);
                                            info.FloatingWindow.IsOpen = false;
                                            ValidateFloatWindow(info.FloatingWindow);
                                        }
                                    }
                                    else
                                    {
                                        CurrentDragPopup.IsOpen = false;
                                    }
                                    DockState state = host.State;
                                    int iTabOrder = GetEdgeTabOrder(host.TabChildren, state, false);
                                    if (UseNativeFloatWindow)
                                    {
                                        if (PreparePreviewElements(m_holdList, host))
                                        {
                                            foreach (FrameworkElement tab in m_previewElements)
                                            {
                                                DockingManager.SetState(tab, state);
                                                DockedElementTabbedHost.SetTabOrder(tab, state, ++iTabOrder);
                                                host.TabChildren.Add(tab);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        PreparePreviewElements(m_holdList);
                                        foreach (FrameworkElement tab in m_previewElements)
                                        {
                                            DockingManager.SetState(tab, state);
                                            DockedElementTabbedHost.SetTabOrder(tab, state, ++iTabOrder);
                                            host.TabChildren.Add(tab);
                                         }
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

                            if (docking == m_draggedElement.Parent)
                            {
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
                }
                else if (IsDragging && DraggingType.NormalDragging != DraggingType && !IsntSelf(host))
                {
                    if (m_prevHostUnderMouse != null)
                    {
                        FireDockProviderShownEvent(host);
                        m_hostUnderMouse = m_prevHostUnderMouse;
                        ProcessingCreateDockPreview(e);
                    }
                }
                FrameworkElement child = VisualUtils.FindDescendant((Visual)host, typeof(HwndHost)) as FrameworkElement;
                if (child != null && !UsePopupAutoHidePreview)
                {
                    HideSidePanelItems();
                }
            }
        }

        /// <summary>
        /// Draws drag providers for host under mouse when dragging on header of the float window is done.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        internal void OnMouseEnterOnHeader(object sender, InputEventArgs e)
        {
            FloatWindowBorder origSource = sender as FloatWindowBorder;
            if (this.Dispatcher.CheckAccess())
            {
                if (origSource != null && IsDragging)
                {
                    ProcessingSetOnTopFloatWindow(m_draggedElement);
                    //SetHostUnderMouse(origSource);

                    if (m_hostUnderMouse != null)
                    {
                        FireDockProviderShownEvent(m_hostUnderMouse);

                        ProcessingCreateDockPreview(e);
                        m_hostUnderMouse = null;
                    }
                }
                MouseEnter = true;
            }
        }

        /// <summary>
        /// Sets the host under mouse.
        /// </summary>
        /// <param name="source">The source.</param>
        private void SetHostUnderMouse(FrameworkElement source)
        {
            AutoTemplatedContentControl content = (AutoTemplatedContentControl)source.TemplatedParent;
            m_hostUnderMouse = content.Content as DockedElementTabbedHost;
        }

        /// <summary>
        /// Deletes drag providers for host under mouse when dragging on header of the float window is done.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        internal void OnMouseLeaveOnHeader(object sender, InputEventArgs e)
        {
            FloatWindowBorder origSource = sender as FloatWindowBorder;
            if (this.Dispatcher.CheckAccess())
            {
                if (origSource != null && IsDragging)
                {
                    m_managerDragPreview.HideDockPreview();
                }
                MouseEnter = false;
            }
        }

        /// <summary>
        /// Does docking when dragging on header of the float window is done.
        /// </summary>
        /// <param name="sender">object to take FloatWindowBorder from</param>
        /// <param name="e">MouseEvent Args</param>
        internal void OnMouseUpOnHeader(object sender, InputEventArgs e)
        {
            //if (!BrowserInteropHelper.IsBrowserHosted)
            //{
            //    ActivateParent();
            //}
            FloatWindowBorder origSource = sender as FloatWindowBorder;
            if (this.Dispatcher.CheckAccess())
            {
                if (origSource != null && IsDragging)
                {
                    //SetHostUnderMouse(origSource);
                    DoDocking(e);
                }
            }
        }

        private void DoDocking()
        {
            if (CanDockToTarget(m_draggedElement, m_hostUnderMouse) && AllowDock())
            {
                CompleteDocking(Mouse.GetPosition(m_hostUnderMouse), true, false);
                
            }
            if (m_draggedElement != null)
            {
                SwapTargetInternalAndElement(m_draggedElement);
            }
        }

        /// <summary>
        /// Docks window to the target.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        private void DoDocking(InputEventArgs e)
        {
            if (CanDockToTarget(m_draggedElement, m_hostUnderMouse) && AllowDock())
            {             
                if (m_hostUnderMouse != null && (!m_hostUnderMouse.TabChildren.Contains(m_draggedElement)))
                {
                    if (e is MouseEventArgs)
                        CompleteDocking((e as MouseEventArgs).GetPosition(m_hostUnderMouse), true, false);
#if !SyncfusionFramework3_5
                    else if (e is TouchEventArgs)
                        CompleteDocking(((e as TouchEventArgs).GetTouchPoint(m_hostUnderMouse) as TouchPoint).Position, true, false);
#endif
                }
            }
            if (m_draggedElement != null)
            {
                SwapTargetInternalAndElement(m_draggedElement);
            }
        }

        /// <summary>
        /// Swaps the target internal and element.
        /// </summary>
        /// <param name="element">The element.</param>
        private void SwapTargetInternalAndElement(FrameworkElement element)
        {
            List<FrameworkElement> listElement = FindPreviousSiblingsSafe(element);
            if (listElement.Count > 0)
            {
                FrameworkElement prevParent = FindChildSafe(DockingManager.GetTargetNameInDockedMode(element));
                if (prevParent != null)
                {
                    updatedockflag = false;
                    foreach (FrameworkElement child in listElement)
                    {
                        if (DockingManager.GetState(child) == DockState.Dock)
                        {
                            DockingManager.SetTargetNameInDockedMode(child, element.Name);
                            if (!child.Name.Equals(element.Name))
                                CheckInnerDockIndex(child, element.Name, true, -1);
                        }
                    }
                    DockingManager.SetSideInDockedMode(prevParent, DockingManager.GetPreviousSideInDockMode(prevParent));
                    DockingManager.SetTargetNameInDockedMode(element, DockingManager.GetPreviousTargetInDockMode(element));
                    if (!DockingManager.GetPreviousTargetInDockMode(element).Equals(element.Name))
                        CheckInnerDockIndex(element, DockingManager.GetPreviousTargetInDockMode(element), true, -1);
                    DockingManager.SetSideInDockedMode(element, DockingManager.GetPreviousSideInDockMode(element));
                }
            }
        }


        /// <summary>
        /// Completes the docking.
        /// </summary>
        internal void CompleteDocking()
        {
            CompleteDocking(true);
            m_isTabPressed = false;
        }

        /// <summary>
        /// Completes the docking.
        /// </summary>
        /// <param name="isSetPlacement">if set to <c>true</c> [is set placement].</param>
        internal void CompleteDocking(bool isSetPlacement)
        {
            if (IsDragging)
            {
                if (m_firedDragStart)
                {
                    FireWindowDragEnd(ActiveWindow);
                    m_firedDragStart = false;
                }

                m_dragCounter = 0;

                if (isSetPlacement)
                {
                    ReleasePointMouseStartPos();
                    m_isPointKnown = false;
                }

                MouseMove -= OnDockingManagerMouseMove;
                m_managerDragPreview.HideDockPreviewMainButton(false);
                m_managerDragPreview.HideDockPreview();
                IsDragging = false;

                if (null != m_draggedElement)
                {
                    if (DraggingType == DraggingType.NormalDragging)
                    {
                        DockInfoInternal info = DockingManager.GetDockInfo(m_draggedElement);
                        TaskBarDirection dire = GetTaskBarDirestion();

                        if (info.FloatingWindow != null)
                        {
                            if (info.FloatingWindow.PlacementRectangle.Top < 0)
                            {
                                Rect m_placementrect = info.FloatingWindow.PlacementRectangle;
                                info.FloatingWindow.PlacementRectangle = new Rect(m_placementrect.Left, m_placementrect.Top+10, m_placementrect.Width, m_placementrect.Height);

                            }
                            if (TaskBarDirection.Top == dire && info.FloatingWindow.PlacementRectangle.Top < (System.Windows.Forms.Screen.PrimaryScreen.Bounds.Y + (System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height - System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height)))
                            {
                                Rect m_placementrect = info.FloatingWindow.PlacementRectangle;
                                info.FloatingWindow.PlacementRectangle = new Rect(m_placementrect.Left, (System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height - System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height), m_placementrect.Width, m_placementrect.Height);
                            }

                            if (info.FloatingWindow.PlacementRectangle.Top > (System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height - 20))
                            {
                                Rect m_placementrect = info.FloatingWindow.PlacementRectangle;
                                double top = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height - 20;
                                info.FloatingWindow.PlacementRectangle = new Rect(m_placementrect.Left, top, m_placementrect.Width, m_placementrect.Height);
                            }

                            if (TaskBarDirection.Bottom == dire && info.FloatingWindow.PlacementRectangle.Top > (System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height - (System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height - System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height) - 20))
                            {
                                Rect m_placementrect = info.FloatingWindow.PlacementRectangle;
                                double top = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height - (System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height - System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height);
                                info.FloatingWindow.PlacementRectangle = new Rect(m_placementrect.Left, top - 20, m_placementrect.Width, m_placementrect.Height);
                            }

                            
                            if (info.FloatingWindow.PlacementRectangle.Left > (System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width - 20))
                            {
                                Rect m_placementrect = info.FloatingWindow.PlacementRectangle;
                                if (System.Windows.Forms.Screen.AllScreens.Length == 1)
                                {
                                    double right = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width - 20;
                                    info.FloatingWindow.PlacementRectangle = new Rect(right, m_placementrect.Top, m_placementrect.Width, m_placementrect.Height);
                                }
                                else
                                {
                                    double right = System.Windows.Forms.Screen.AllScreens[0].Bounds.Width - 160;
                                    for (int i = 1; i < System.Windows.Forms.Screen.AllScreens.Length; i++)
                                    {
                                        right = right + System.Windows.Forms.Screen.AllScreens[i].Bounds.Width;
                                    }
                                    
                                    if (info.FloatingWindow.PlacementRectangle.Left > right)
                                    {
                                        info.FloatingWindow.PlacementRectangle = new Rect(right, m_placementrect.Top, m_placementrect.Width, m_placementrect.Height);
                                    }
                                    
                                }
                                
                            }
                            if (System.Windows.Forms.Screen.AllScreens.Length == 1 && TaskBarDirection.Right == dire && info.FloatingWindow.PlacementRectangle.Left > (System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width - (System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width - System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width) - 20))
                            {
                                Rect m_placementrect = info.FloatingWindow.PlacementRectangle;
                                double right = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width - (System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width - System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width) - 20;
                                info.FloatingWindow.PlacementRectangle = new Rect(right, m_placementrect.Top, m_placementrect.Width, m_placementrect.Height);
                            }

                            if (info.FloatingWindow.PlacementRectangle.Right < (System.Windows.Forms.Screen.PrimaryScreen.Bounds.X))
                            {
                                Rect m_placementrect = info.FloatingWindow.PlacementRectangle;
                                
                                if (System.Windows.Forms.Screen.AllScreens.Length == 1)
                                {
                                    double left = System.Windows.Forms.Screen.PrimaryScreen.Bounds.X;
                                    info.FloatingWindow.PlacementRectangle = new Rect(left, m_placementrect.Top, m_placementrect.Width, m_placementrect.Height);
                                }
                            }

                            if (dire == TaskBarDirection.Left && info.FloatingWindow.PlacementRectangle.Right < (System.Windows.Forms.Screen.PrimaryScreen.Bounds.X + (System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width - System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width) + 30))
                            {
                                Rect m_placementrect = info.FloatingWindow.PlacementRectangle;
                                double right = System.Windows.Forms.Screen.PrimaryScreen.Bounds.X + (System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width - System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width) + 30;
                                info.FloatingWindow.PlacementRectangle = new Rect(right, m_placementrect.Top, m_placementrect.Width, m_placementrect.Height);
                            }
                            info.FloatingWindow.CompleteDragging();
                        }
                    }
                    else
                    {
                        if (isSetPlacement)
                        {
                            switch (m_DraggingSource)
                            {
                                case DraggingSource.Tab:
                                    ExtractElementToWindow(m_draggedElement, ActionMode.Active, false);
                                    break;

                                case DraggingSource.Host:
                                    ExtractElementToWindow(m_draggedElement, ActionMode.Group, false);
                                    break;

                                case DraggingSource.Window:
                                    DockInfoInternal info = DockingManager.GetDockInfo(m_draggedElement);

                                    if (!info.DockingManager.UseNativeFloatWindow)
                                    {
                                        if (info.FloatingWindow.PlacementRectangle.Top < 0)
                                        {
                                            Rect m_placementrect = info.FloatingWindow.PlacementRectangle;
                                            info.FloatingWindow.PlacementRectangle = new Rect(m_placementrect.Left, 0, m_placementrect.Width, m_placementrect.Height);
                                        }
                                        else if (info.FloatingWindow.PlacementRectangle.Top > System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height - 20)
                                        {
                                            Rect m_placementrect = info.FloatingWindow.PlacementRectangle;
                                            double top = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height - 20;
                                            info.FloatingWindow.PlacementRectangle = new Rect(m_placementrect.Left, top, m_placementrect.Width, m_placementrect.Height);
                                        }
                                        info.FloatingWindow.CompleteDragging();
                                    }
                                    else
                                    {
                                        if (info.NativeWindow.InternalPlacementRect.Top < 0)
                                        {
                                            Rect m_placementrect = info.NativeWindow.InternalPlacementRect;
                                            info.NativeWindow.InternalPlacementRect = new Rect(m_placementrect.Left, 0, m_placementrect.Width, m_placementrect.Height);
                                        }
                                        else if (info.NativeWindow.InternalPlacementRect.Top > System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height - 20)
                                        {
                                            Rect m_placementrect = info.NativeWindow.InternalPlacementRect;
                                            double top = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height - 20;
                                            info.NativeWindow.InternalPlacementRect = new Rect(m_placementrect.Left, top, m_placementrect.Width, m_placementrect.Height);
                                        }
                                  }
                                    
                                    break;
                            }

                            m_DraggingSource = DraggingSource.Window;
                            LockLayoutUpdate = true;
                        }

                        if (!m_bLoadPreview)
                        {
                            CurrentDragPopup.CompleteDragging(isSetPlacement);
                        }
                    }
                    DockedElementTabbedHost floatHost = DockingManager.ResolveHostFloat(m_draggedElement);
                    Keyboard.Focus(floatHost);
                    m_draggedElement = null;
                    m_hostUnderMouse = null;
                    m_prevHostUnderMouse = null;
                    ReleaseMouseCapture();
                }
            }
        }

        internal static TaskBarDirection GetTaskBarDirestion()
        {
            System.Drawing.Rectangle screenRect = System.Windows.Forms.Screen.PrimaryScreen.Bounds;
            TaskBarDirection direction = TaskBarDirection.Bottom;

            RECT rect;
            Point point = new Point(0.0, 0.0);
            IntPtr taskbar = NativeMethods.FindWindow("Shell_TrayWnd", String.Empty);
            if (NativeMethods.GetWindowRect(taskbar, out rect))
            {
                foreach (System.Windows.Forms.Screen screen in System.Windows.Forms.Screen.AllScreens)
                {
                    if (screen.Bounds.Contains(rect.left, rect.top))
                    {
                        screenRect = screen.Bounds;
                        break;
                    }
                }
                if (rect.right < screenRect.Right)
                {
                    direction = TaskBarDirection.Left;
                }
                else if (rect.left > screenRect.Left)
                {
                    direction = TaskBarDirection.Right;
                }
                else if (rect.top > screenRect.Top)
                {
                    direction = TaskBarDirection.Bottom;
                }
                else if (rect.bottom < screenRect.Bottom)
                {
                    direction = TaskBarDirection.Top;
                }
            }

            return direction;
        }

        /// <summary>
        /// Completes the docking.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="isPreview">if set to <c>true</c> [is preview].</param>
        /// <param name="allowDockToSelf">if set to <c>true</c> [allow dock to self].</param>
        internal void CompleteDocking(Point point, bool isPreview, bool allowDockToSelf)
        {
            if (IsDragging)
            {
                DockPreviewRecord? record = isPreview ? m_managerDragPreview.FindDockingPlace(m_draggedElement, m_hostUnderMouse, point) : GetTabbedPrevievRecord();

                bool isSetPlacement = true;

                if (null != record)
                {
                    isSetPlacement = false;
                    DockPreviewRecord recordPreview = record.Value;

                    if (recordPreview.Side == DockSide.Tabbed && !Children.Contains(recordPreview.TargetElement))
                    {
                        List<FrameworkElement> list = GetListOfElements(m_draggedElement, DockState.Float);
                        bool bContainsNoDocumentElement = false;

                        foreach (FrameworkElement element in list)
                        {
                            bContainsNoDocumentElement |= !DockingManager.GetCanDocument(element);
                            if (bContainsNoDocumentElement)
                            {
                                break;
                            }
                        }

                        if (!bContainsNoDocumentElement)
                        {
                            DockState state = DockingManager.GetState(recordPreview.Element);

                            if (DockingManager.GetDockAbility(recordPreview.Element) == DockAbility.All || DockingManager.GetDockAbility(recordPreview.Element) == DockAbility.Tabbed)
                            {
                                InsertIntoContainer(recordPreview.Element, null, state, DockState.Document, DockSide.Tabbed, recordPreview.PreviewSize);
                            }
                        }
                    }
                    else if (CanDockToTarget(m_draggedElement, m_hostUnderMouse) &&
                        !DockingManager.GetNoDock(m_draggedElement))
                    {
                        if (DockStateChanging != null)
                        {
                            DockStateChangingEventArgs args = new DockStateChangingEventArgs();
                            args.SourceElement = m_draggedElement;
                            args.TargetElement = recordPreview.TargetElement;
                            args.PresentState = DockState.Float;
                            args.TargetState = recordPreview.State;
                            args.TargetSide = recordPreview.Side;
                            DockStateChanging(m_draggedElement, args);
                            if (!args.Cancel)
                            {
                                m_IsStateChangingChecked = true;
                                DockNormal(recordPreview);
                            }
                        }
                        else
                        {
                            DockNormal(recordPreview);
                        }
                    }
                }

                CompleteDocking(isSetPlacement);
            }
        }

        /// <summary>
        /// Checks the dock ability.
        /// </summary>
        /// <param name="elementability">The elementability.</param>
        /// <param name="elementside">The elementside.</param>
        /// <returns></returns>
        internal bool AllowDock(DockAbility elementability, DockSide elementside)
        {
            if (elementside == DockSide.Top)
            {
                return (elementability & DockAbility.Top) == DockAbility.Top;
            }
            else if (elementside == DockSide.Left)
            {
                return (elementability & DockAbility.Left) == DockAbility.Left;
            }
            else if (elementside == DockSide.Right)
            {
                return (elementability & DockAbility.Right) == DockAbility.Right;
            }
            else if (elementside == DockSide.Bottom)
            {
                return (elementability & DockAbility.Bottom) == DockAbility.Bottom;
            }
            else if (elementside == DockSide.Tabbed)
            {
                return (elementability & DockAbility.Tabbed) == DockAbility.Tabbed;
            }
            return false;
        }

        /// <summary>
        /// Docks the normal.
        /// </summary>
        /// <param name="recordPreview">The record preview.</param>
        private void DockNormal(DockPreviewRecord recordPreview)
        {
            if (DraggingType.NormalDragging == DraggingType || null == m_holdList)
            {
                MakeDocking(recordPreview);
            }
            else
            {
                FrameworkElement element = recordPreview.Element;
                FrameworkElement target = recordPreview.TargetElement;
                DockState state = DockingManager.GetState(element);

                if (element == target)
                {
                    FrameworkElement sibling = GetSibling(ref element, state, true);

                    if (sibling != null)
                    {
                        SwapElementAndTargetInternal(sibling, element, state, this, true, true);
                        DockedElementTabbedHost host = DockingManager.GetTabbedHost(element, state);
                        target = sibling;
                        host.HostedElement = sibling;
                    }

                    if (state == DockState.Float)
                    {
                        DockInfoInternal info = DockingManager.GetDockInfo(element);
                        if (info.FloatingWindow!=null && info.FloatingWindow.PrimaryElement != null)
                        {
                            if (info.FloatingWindow.PrimaryElement == element)
                            {
                                info.FloatingWindow.SetNewPrimaryElement(sibling);
                            }
                        }
                    }
                }
                else
                {
                    FrameworkElement sibling = GetSibling(ref element, state, true);
                    
                    if (sibling != null)
                    {
                        SwapElementAndTargetInternal(sibling, element, state, this, true, true);
                    }
                }

                DockSide elementside = recordPreview.Side;
                DockAbility elementability = DockingManager.GetDockAbility(element);

                if (AllowDock(elementability, elementside) || elementability == DockAbility.All)
                {
                    InsertIntoContainer(element, target, state, recordPreview.State, recordPreview.Side, recordPreview.PreviewSize);
                }
                if (recordPreview.State == DockState.Dock && target!=null && element!=null)
                {
                    ArrangeCorrectIndex(target, element);
                    ArrangeIndex(target);
                }

            }
        }

        private void ArrangeCorrectIndex(FrameworkElement target,FrameworkElement element)
        {
            List<FrameworkElement> siblings = FindSiblingsEx(target, DockState.Dock);
            int Iindex=DockingManager.GetIndexInDockMode(target) - 1;
            DockingManager.SetIndexInDockMode(element, Iindex);
            if (siblings.Count > 0)
            {
                List<FrameworkElement> elements = new List<FrameworkElement>();
                foreach (FrameworkElement child in siblings)
                {
                    if (child != element)
                    {
                        elements.Add(child);
                    }
                }
                elements.Sort(new Comparison<FrameworkElement>((x, y) => DockingManager.GetIndexInDockMode(x).CompareTo(DockingManager.GetIndexInDockMode(y))));
                
                for (int i = elements.Count - 1; i > -1; i--)
                {
                    DockingManager.SetIndexInDockMode(elements[i], --Iindex);
                }
            }
        }

        private void ArrangeIndex(FrameworkElement target)
        {
            List<FrameworkElement> siblings = FindSiblingsEx(target, DockState.Dock);
            int Iindex = DockingManager.GetIndexInDockMode(target);
            if (siblings.Count > 0)
            {
                siblings.Sort(new Comparison<FrameworkElement>((x, y) => DockingManager.GetIndexInDockMode(x).CompareTo(DockingManager.GetIndexInDockMode(y))));
                for (int i = siblings.Count - 1; i > -1; i--)
                {
                    DockingManager.SetIndexInDockMode(siblings[i], --Iindex);
                }
            }
        }

        /// <summary>
        /// Starts dragging internal element
        /// </summary>
        /// <param name="element">FrameworkElement to drag.</param>
        /// <param name="state">True if tabs must be detached.</param>
        /// <returns>return FrameworkElement</returns>
        private List<FrameworkElement> FindTabSibling(IFrameworkInputElement element, DockState state)
        {
            List<FrameworkElement> resultList = new List<FrameworkElement>();
            string name = element.Name;
            foreach (FrameworkElement child in Children)
            {
                string childTargetName = DockingManager.GetTargetName(child, state);

                if (name == childTargetName)
                {
                    DockState childState = DockingManager.GetState(child);

                    if (state == childState)
                    {
                        DockSide childSide = DockingManager.GetSide(child, state);

                        if (DockSide.Tabbed == childSide)
                        {
                            resultList.Add(child);
                        }
                    }
                }
            }

            return resultList;
        }

        private IList<TabItemExt> m_DraggingTabs = new List<TabItemExt>();
        /// <summary>
        /// This method updates position of tab-item.
        /// </summary>
        /// <param name="panel">Panel is host for items.</param>
        /// <param name="arg">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        private void UpdateTabItemPosition(Panel panel, InputEventArgs arg)
        {
            if (panel != null)
            {
                if (panel is DirectTabPanel)
                {
                    TabItem tabItem = null;
                    int overIndex = 0;
                    TabItem dragTabItem = null;
                    int dragIndex = 0;

                    for (int i = 0, cnt = panel.Children.Count; i < cnt; i++)
                    {
                        TabItem item = (TabItem)panel.Children[i];
                        Point point = new Point();
                        if (arg is MouseEventArgs)
                            point = (arg as MouseEventArgs).GetPosition(item);
#if !SyncfusionFramework3_5
                        else if (arg is TouchEventArgs)
                            point = (arg as TouchEventArgs).GetTouchPoint(item).Position;
#endif
                        Rect rect = new Rect(item.RenderSize);

                        if (rect.Contains(point))
                        {
                            tabItem = item;
                            overIndex = i;
                            if (null != dragTabItem)
                            {
                                break;
                            }
                        }

                        if (item.Content == m_holdList[0])
                        {
                            dragTabItem = item;
                            dragIndex = i;
                            if (null != tabItem)
                            {
                                break;
                            }
                        }
                    }

                    UpdateTabItemPosition(panel, arg, tabItem, dragTabItem, overIndex, dragIndex);
                }

                else if (panel is HeaderPanel)
                {
                    TabItemExt tabItem = null;
                    int overIndex = 0;
                    TabItemExt dragTabItem = null;
                    int dragIndex = 0;

                    TabPanelAdv tabpanel = panel.Children[0] as TabPanelAdv;
                    if (tabpanel != null)
                    {
                        TabLayoutPanel tablayoutpanel = tabpanel.Content as TabLayoutPanel;
                        if (tablayoutpanel != null)
                        {
                            for (int i = 0, cnt = tablayoutpanel.Children.Count; i < cnt; i++)
                            {
                                TabItemExt item = (TabItemExt)tablayoutpanel.Children[i];
                                Point point = new Point();
                                if (arg is MouseEventArgs)
                                    point = (arg as MouseEventArgs).GetPosition(item);
#if !SyncfusionFramework3_5
                                else if (arg is TouchEventArgs)
                                    point = (arg as TouchEventArgs).GetTouchPoint(item).Position;
#endif
                                Rect rect = new Rect(item.RenderSize);

                                if (rect.Contains(point))
                                {
                                    tabItem = item;
                                    overIndex = i;
                                    if (null != dragTabItem)
                                    {
                                        break;
                                    }
                                }

                                if (DockingManager.m_holdTabItemExtList!=null && item.Content == DockingManager.m_holdTabItemExtList[0])
                                {
                                    item.MouseLeftButtonUp -= DockingManager_MouseLeftButtonUp;
#if !SyncfusionFramework3_5
                                    //item.TouchUp -= item_TouchUp;
                                    //item.TouchUp += item_TouchUp;
                                    //item.TouchEnter -= item_TouchEnter;
                                    //item.TouchEnter += item_TouchEnter;
#endif
                                    item.MouseLeftButtonUp += DockingManager_MouseLeftButtonUp;
                                    item.MouseEnter -= item_MouseEnter;
                                    item.MouseEnter += item_MouseEnter;
                                    dragTabItem = item;
                                    dragIndex = i;
                                    if (null != tabItem)
                                    {
                                        break;
                                    }
                                }
                            }
                            if (tabItem != null && dragTabItem != null && tabItem != dragTabItem)
                            {
                                Point tabpoint = new Point();
                                if (arg is MouseEventArgs)
                                    tabpoint = (arg as MouseEventArgs).GetPosition(tabItem);
#if !SyncfusionFramework3_5
                                else if (arg is TouchEventArgs)
                                    tabpoint = (arg as TouchEventArgs).GetTouchPoint(tabItem).Position;
#endif
                                Rect tabrect = new Rect(tabItem.RenderSize);

                                Point dragpoint = new Point();
                                if (arg is MouseEventArgs)
                                    dragpoint = (arg as MouseEventArgs).GetPosition(dragTabItem);
#if !SyncfusionFramework3_5
                                else if (arg is TouchEventArgs)
                                    dragpoint = (arg as TouchEventArgs).GetTouchPoint(dragTabItem).Position;
#endif
                                Rect dragrect = new Rect(dragTabItem.RenderSize);

                                if (dragrect.Width > tabrect.Width)
                                {
                                    UpdateTabItemPosition(panel, arg, tabItem, dragTabItem, overIndex, dragIndex);
                                }
                                else
                                {
                                    if (dragpoint.X > 0)
                                    {
                                        if (m_DraggingTabs.Count > 0 && m_DraggingTabs.Contains(dragTabItem) && m_DraggingTabs.Contains(tabItem))
                                        {
                                            if (dragpoint.X < dragrect.Width)
                                            {
                                                UpdateTabItemPosition(panel, arg, tabItem, dragTabItem, overIndex, dragIndex);
                                            }
                                            else if ((tabrect.Width - tabpoint.X) + 2 < dragrect.Width)
                                            {
                                                UpdateTabItemPosition(panel, arg, tabItem, dragTabItem, overIndex, dragIndex);
                                            }
                                        }
                                        else
                                        {
                                            UpdateTabItemPosition(panel, arg, tabItem, dragTabItem, overIndex, dragIndex);
                                        }

                                    }
                                    else if (dragpoint.X < 0)
                                    {
                                        if (m_DraggingTabs.Count > 0 && m_DraggingTabs.Contains(dragTabItem) && m_DraggingTabs.Contains(tabItem))
                                        {
                                            if (tabpoint.X > tabrect.Width)
                                            {
                                                UpdateTabItemPosition(panel, arg, tabItem, dragTabItem, overIndex, dragIndex);
                                            }
                                            else if (tabpoint.X + 2 < dragrect.Width)
                                            {
                                                UpdateTabItemPosition(panel, arg, tabItem, dragTabItem, overIndex, dragIndex);
                                            }
                                        }
                                        else
                                        {
                                            UpdateTabItemPosition(panel, arg, tabItem, dragTabItem, overIndex, dragIndex);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    
                }
            }
        }

        private void item_MouseEnter(object sender, MouseEventArgs e)
        {
            if (e.StylusDevice == null || e.StylusDevice != null)
            {
                m_DraggingTabs.Clear();
            }
        }

        private void DockingManager_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (e.StylusDevice == null || e.StylusDevice != null)
            {
                m_DraggingTabs.Clear();
            }
        }

        private void UpdateTabItemPositionInDragMode(Panel panel, DragEventArgs arg)
        {
            if (panel != null)
            {
                if (panel is DirectTabPanel)
                {
                    TabItem oldSelectedItem = null;
                    int oldIndex = 0;
                    TabItem newItem = null;
                    int overIndex = 0;

                    for (int i = 0, cnt = panel.Children.Count; i < cnt; i++)
                    {
                        TabItem item = (TabItem)panel.Children[i];
                        Point point = arg.GetPosition(item);
                        Rect rect = new Rect(item.RenderSize);

                        if (rect.Contains(point))
                        {
                            newItem = item;
                            overIndex = i;
                            if (null != oldSelectedItem)
                            {
                                break;
                            }
                        }
                        if (item.IsSelected)
                        {
                            oldSelectedItem = item;
                            oldIndex = i;
                            if (null != newItem)
                            {
                                break;
                            }
                        }
                    }

                    if (oldSelectedItem != null && newItem != null)
                    {
                        oldSelectedItem.IsSelected = false;
                        newItem.IsSelected = true;
                    }
                }
            }
        }

        /// <summary>
        /// This method updates position of tab-item.
        /// </summary>
        /// <param name="panel">Panel is host for items.</param>
        /// <param name="e">MouseEventArgs for calculates position.</param>
        /// <param name="tabItem">Tab item value.</param>
        /// <param name="dragTabItem">Drag tab item.</param>
        /// <param name="overIndex">Over index value.</param>
        /// <param name="dragIndex">Drag index value.</param>
        private void UpdateTabItemPosition(Panel panel, InputEventArgs e, UIElement tabItem, UIElement dragTabItem, int overIndex, int dragIndex)
        {
            if (panel is DirectTabPanel)
            {
                if (tabItem != null && dragTabItem != null && overIndex != dragIndex)
                {
                    bool canSwap = false;

                    if (m_swapDirection == -1)
                    {
                        canSwap = true;
                        m_swapDirection = (dragIndex > overIndex) ? 0 : 1;
                    }
                    else
                    {
                        if (dragIndex > overIndex)
                        {
                            double itemWidth = dragTabItem.RenderSize.Width;
                            canSwap = CalculateCanSwap(panel, 0, overIndex, itemWidth, true, e);
                        }
                        else
                        {
                            if (dragIndex + m_holdList.Count < panel.Children.Count)
                            {
                                double itemWidth = tabItem.RenderSize.Width;
                                canSwap = CalculateCanSwap(panel, 1, dragIndex, itemWidth, false, e);
                            }
                        }
                    }

                    if (canSwap)
                    {
                        DockState state = DockingManager.GetState(m_holdList[0]);

                        if (state == DockState.Dock)
                        {
                            MountTabOrderProperties(panel, DockedElementTabbedHost.TabOrderInDockModeProperty, dragIndex, overIndex);
                            if (m_holdList[0] != null)
                            {
                                DockedElementTabbedHost.SetTabOrderInDockMode(m_holdList[0] as DependencyObject, overIndex);
                            }
                        }
                        else if (state == DockState.Float)
                        {
                            MountTabOrderProperties(panel, DockedElementTabbedHost.TabOrderInFloatModeProperty, dragIndex, overIndex);
                            if (m_holdList[0] != null)
                            {
                                DockedElementTabbedHost.SetTabOrderInFloatMode(m_holdList[0] as DependencyObject, overIndex);
                            }
                        }
                    }
                }
            }

            else if (panel is HeaderPanel)
            {
                TabPanelAdv tabpaneladv = panel.Children[0] as TabPanelAdv;
                TabLayoutPanel tablayoutpanel = (tabpaneladv != null) ? tabpaneladv.Content as TabLayoutPanel : null;
                
                if (tablayoutpanel.CheckToUpdatePosition(tablayoutpanel.m_finalsize, tabItem as TabItemExt))
                {
                    if (tabItem != null && dragTabItem != null && overIndex != dragIndex)
                    {
                        tablayoutpanel.m_NeedScrolling = false;
                        int selectedindex = overIndex;
                        TabControlExt tabcontrol = VisualUtils.FindAncestor(tabItem as Visual, typeof(TabControlExt)) as TabControlExt;
                        if (tabcontrol != null)
                        {
                            tablayoutpanel.m_IsRemoving = true;
                            tablayoutpanel.m_CheckingSize = tablayoutpanel.m_scrollInfo.Offset;
                            tabcontrol.Items.Remove(tabItem);
                            tabcontrol.Items.Remove(dragTabItem);
                            tablayoutpanel.m_IsRemoving = false;
                            if (dragIndex < overIndex)
                            {
                                tabcontrol.Items.Insert(dragIndex, tabItem);
                                tabcontrol.Items.Insert(overIndex, dragTabItem);
                            }
                            else
                            {
                                tabcontrol.Items.Insert(overIndex, dragTabItem);
                                tabcontrol.Items.Insert(dragIndex, tabItem);
                            }
                            if (!(tabcontrol.Items[selectedindex] as TabItemExt).IsSelected)
                                (tabcontrol.Items[selectedindex] as TabItemExt).IsSelected = true;
                            m_DraggingTabs.Clear();
                            m_DraggingTabs.Add(dragTabItem as TabItemExt);
                            m_DraggingTabs.Add(tabItem as TabItemExt);
                        }
                        tablayoutpanel.m_NeedScrolling = true;
                    }
                }
            }
        }

        /// <summary>
        /// Called when [direct tab panel lost mouse capture].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        internal void OnDirectTabPanelLostCapture(object sender, InputEventArgs e)
        {
            if (this.Dispatcher.CheckAccess())
            {
                if (!m_isTabPressed)
                {
                    StopTabItemDragging((DirectTabPanel)sender);
                }
            }
        }

        /// <summary>
        /// Stops the tab item dragging.
        /// </summary>
        /// <param name="panel">The panel.</param>
        private void StopTabItemDragging(FrameworkElement panel)
        {
            m_holdList = null;
            m_isTabPressed = false;
            m_swapDirection = -1;

            if (null != panel)
            {
                panel.ReleaseMouseCapture();
            }
        }

        /// <summary>
        /// Called when [direct tab panel drag over].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.DragEventArgs"/> instance containing the event data.</param>
        internal void OnDirectTabPanelDragOver(FrameworkElement sender, DragEventArgs e)
        {
            DirectTabPanel panel = (DirectTabPanel)sender;

            if (IsDockTabPanel(panel))
            {
                UpdateTabItemPositionInDragMode(panel, e);
            }
        }

        /// <summary>
        /// Called when [direct tab panel mouse move].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        internal ActionMode extractmode = ActionMode.Group;
        internal void OnDirectTabPanelMouseMove(FrameworkElement sender,MouseEventArgs e)
        {
            try
            {
                if (e.StylusDevice == null || e.StylusDevice != null)
                {
                    DirectTabPanel panel = (DirectTabPanel)sender;

                    if (this.Dispatcher.CheckAccess())
                    {
                        if (IsDockTabPanel(panel) && m_isTabPressed && null != m_holdList && e.LeftButton == MouseButtonState.Pressed)
                        {
                            if (!panel.IsMouseCaptured)
                                panel.CaptureMouse();
                            Point point = e.GetPosition(panel);
                            Rect rect = new Rect(panel.RenderSize);
                            DockState state = DockingManager.GetState(m_holdList[0]);

                            if (!rect.Contains(point))
                            {
                                if (GetCanFloat(m_holdList[0]))
                                {
                                    if (DraggingType.NormalDragging == DraggingType)
                                    {
                                        if (UseNativeFloatWindow)
                                        {
                                            m_nativeWindowDragging = true;
                                        }

                                        if (!m_bLoadPreview)
                                        {
                                            m_isTabPressed = false;
                                            m_isPointKnown = false;
                                            m_pointMouseStartPos = VisualUtils.PointToScreen(panel, point);
                                            m_draggedElement = m_holdList[0];
                                            ExtractElementToWindow(m_holdList[0], ActionMode.Active, true);

                                        }
                                        else
                                        {
                                            if (m_prevHostUnderMouse != null)
                                            {
                                                m_prevHostUnderMouse.TabChildren.RemoveRange(m_previewElements);
                                                m_prevHostUnderMouse.m_previewedElements.Clear();
                                            }
                                            if (m_draggedElement != null)
                                            {
                                                DockInfoInternal info = DockingManager.GetDockInfo(m_draggedElement);
                                                if (UseNativeFloatWindow)
                                                {
                                                    if (info != null && info.NativeWindow != null && !m_NativeWindowsUnRegistered.Contains(info.NativeWindow))
                                                    {
                                                        info.NativeWindow.IsOpen = true;
                                                        info.NativeWindow.m_mouseLeftButtonDown = false;
                                                    }
                                                }
                                                else
                                                {
                                                    info.FloatingWindow.IsOpen = true;
                                                    info.FloatingWindow.HitTestDisabled = false;
                                                    info.FloatingWindow.HitTestDisabled = true;
                                                }
                                            }
                                            m_previewElements.Clear();
                                        }

                                        panel.ReleaseMouseCapture();
                                        Mouse.Capture(this, CaptureMode.SubTree);
                                        LockLayoutUpdate = true;
                                        m_bLoadPreview = false;
                                        IsDragging = true;
                                    }
                                    else
                                    {
                                        IsDragging = true;

                                        if (m_bLoadPreview)
                                        {
                                            m_prevHostUnderMouse.TabChildren.RemoveRange(m_previewElements);
                                            m_currentDragPopup.IsOpen = true;
                                            m_previewElements.Clear();
                                        }
                                        else
                                        {
                                            m_isTabPressed = false;
                                            m_isPointKnown = false;
                                            m_pointMouseStartPos = VisualUtils.PointToScreen(panel, point);
                                            m_draggedElement = m_holdList[0];
                                            extract = true;
                                            extractmode = ActionMode.Active;
                                            if (DockingManager.GetState(m_draggedElement) == DockState.Dock ||
                                                !string.IsNullOrEmpty(DockingManager.GetTargetNameInFloatingMode(m_draggedElement)))
                                            {
                                                HandleWindowPlacement(m_draggedElement, true);
                                            }
                                            //  if (!UseNativeFloatWindow) 
                                            CurrentDragPopup.StartDragging(m_draggedElement, m_holdList);
                                        }

                                        panel.ReleaseMouseCapture();
                                        Mouse.Capture(this, CaptureMode.SubTree);
                                        LockLayoutUpdate = true;
                                        m_bLoadPreview = false;
                                        m_isTabPressed = false;
                                    }

                                    MouseMove += OnDockingManagerMouseMove;
                                }
                            }
                            else
                            {
                                UpdateTabItemPosition(panel, e);

                            }
                        }
                    }
                }
            }
            catch
            { }
        }

#if !SyncfusionFramework3_5
        //internal void OnDirectTabPanelTouchMove(FrameworkElement sender, TouchEventArgs e)
        //{
        //    DirectTabPanel panel = (DirectTabPanel)sender;

        //    if (this.Dispatcher.CheckAccess())
        //    {
        //        if (this.m_dockingManagerSystemGesture
        //            == SystemGesture.Drag)
        //        {
        //        }
        //        if (IsDockTabPanel(panel) && m_isTabPressed && null != m_holdList && this.m_dockingManagerSystemGesture == SystemGesture.Drag)
        //        {
        //            if (!panel.IsMouseCaptured)
        //                panel.CaptureMouse();
        //            Point point = e.GetTouchPoint(panel).Position;
        //            Rect rect = new Rect(panel.RenderSize);
        //            DockState state = DockingManager.GetState(m_holdList[0]);

        //            if (!rect.Contains(point))
        //            {
        //                if (GetCanFloat(m_holdList[0]))
        //                {
        //                    if (DraggingType.NormalDragging == DraggingType)
        //                    {
        //                        if (UseNativeFloatWindow)
        //                        {
        //                            m_nativeWindowDragging = true;
        //                        }

        //                        if (!m_bLoadPreview)
        //                        {
        //                            m_isTabPressed = false;
        //                            m_isPointKnown = false;
        //                            m_pointMouseStartPos = VisualUtils.PointToScreen(panel, point);
        //                            m_draggedElement = m_holdList[0];
        //                            ExtractElementToWindow(m_holdList[0], ActionMode.Active, true);

        //                        }
        //                        else
        //                        {
        //                            if (m_prevHostUnderMouse != null)
        //                            {
        //                                m_prevHostUnderMouse.TabChildren.RemoveRange(m_previewElements);
        //                                m_prevHostUnderMouse.m_previewedElements.Clear();
        //                            }
        //                            if (m_draggedElement != null)
        //                            {
        //                                DockInfoInternal info = DockingManager.GetDockInfo(m_draggedElement);
        //                                if (UseNativeFloatWindow)
        //                                {
        //                                    if (info != null && info.NativeWindow != null && !m_NativeWindowsUnRegistered.Contains(info.NativeWindow))
        //                                    {
        //                                        info.NativeWindow.IsOpen = true;
        //                                        info.NativeWindow.m_mouseLeftButtonDown = false;
        //                                    }
        //                                }
        //                                else
        //                                {
        //                                    info.FloatingWindow.IsOpen = true;
        //                                    info.FloatingWindow.HitTestDisabled = false;
        //                                    info.FloatingWindow.HitTestDisabled = true;
        //                                }
        //                            }
        //                            m_previewElements.Clear();
        //                        }

        //                        panel.ReleaseTouchCapture(e.TouchDevice);
        //                        e.TouchDevice.Capture(this, CaptureMode.SubTree);
        //                        LockLayoutUpdate = true;
        //                        m_bLoadPreview = false;
        //                        IsDragging = true;
        //                    }
        //                    else
        //                    {
        //                        IsDragging = true;

        //                        if (m_bLoadPreview)
        //                        {
        //                            m_prevHostUnderMouse.TabChildren.RemoveRange(m_previewElements);
        //                            m_currentDragPopup.IsOpen = true;
        //                            m_previewElements.Clear();
        //                        }
        //                        else
        //                        {
        //                            m_isTabPressed = false;
        //                            m_isPointKnown = false;
        //                            m_pointMouseStartPos = VisualUtils.PointToScreen(panel, point);
        //                            m_draggedElement = m_holdList[0];
        //                            extract = true;
        //                            extractmode = ActionMode.Active;
        //                            if (DockingManager.GetState(m_draggedElement) == DockState.Dock ||
        //                                !string.IsNullOrEmpty(DockingManager.GetTargetNameInFloatingMode(m_draggedElement)))
        //                            {
        //                                HandleWindowPlacement(m_draggedElement, true);
        //                            }
        //                            //  if (!UseNativeFloatWindow) 
        //                            CurrentDragPopup.StartDragging(m_draggedElement, m_holdList);
        //                        }

        //                        panel.ReleaseTouchCapture(e.TouchDevice);
        //                        e.TouchDevice.Capture(this, CaptureMode.SubTree);
        //                        LockLayoutUpdate = true;
        //                        m_bLoadPreview = false;
        //                        m_isTabPressed = false;
        //                    }

        //                    MouseMove += OnDockingManagerMouseMove;
        //                }
        //            }
        //            else
        //            {
        //                UpdateTabItemPosition(panel, e);

        //            }
        //        }
        //    }
        //}
#endif

        /// <summary>
        /// This method prepares and sets tabed property for panel children.
        /// </summary>
        /// <param name="panel">Instance panel</param>
        /// <param name="depProperty">Stated property.</param>
        /// <param name="dragIndex">Drag index for panel.</param>
        /// <param name="overIndex">Over index for panel.</param>
        private void MountTabOrderProperties(Panel panel, DependencyProperty depProperty, int dragIndex, int overIndex)
        {
            int listItemsCount = m_holdList.Count;
            int start = overIndex;
            int count = dragIndex;
            int increment = listItemsCount;

            if (dragIndex < overIndex)
            {
                start = dragIndex;
                count = overIndex + listItemsCount;
                increment = -listItemsCount;
                overIndex = dragIndex;
            }
            m_setinternal = true;
            SetTabOrderProperties(panel, depProperty, start, count, increment);
            SetHoldListTabOrderProperties(depProperty, listItemsCount, overIndex);
            m_setinternal = false;
        }

        /// <summary>
        /// This method sets stated property for HoldList children.
        /// </summary>
        /// <param name="depProperty">Stated property.</param>
        /// <param name="listItemsCount">HoldList children count</param>
        /// <param name="dragIndex">Drag index for panel.</param>
        private void SetHoldListTabOrderProperties(DependencyProperty depProperty, int listItemsCount, int dragIndex)
        {
            for (int i = 0; i < listItemsCount; ++i)
            {
                m_holdList[i].SetValue(depProperty, dragIndex + i);
            }
        }

        /// <summary>
        /// This method calculate swap.
        /// </summary>
        /// <param name="panel">Instance panel.</param>
        /// <param name="comparer">Comparer value.</param>
        /// <param name="count">Count child for calculate width to swap tab.</param>
        /// <param name="itemWidth">Item width value.</param>
        /// <param name="isBelow">Direct for compare</param>
        /// <param name="e">MouseEventArgs for calculate position.</param>
        /// <returns>Set TRUE than we can swap, otherwise FALSE.</returns>
        private bool CalculateCanSwap(Panel panel, int comparer, int count, double itemWidth, bool isBelow, InputEventArgs e)
        {
            bool result;

            if (m_swapDirection != comparer)
            {
                double width = 0.0;

                for (int i = 0; i < count; ++i)
                {
                    width += panel.Children[i].RenderSize.Width;
                }

                width += itemWidth;
                double pointX = 0.0;
                if (e is MouseEventArgs)
                    pointX = (e as MouseEventArgs).GetPosition(panel).X;
#if !SyncfusionFramework3_5
                else if (e is TouchEventArgs)
                    pointX = (e as TouchEventArgs).GetTouchPoint(panel).Position.X;
#endif
                result = isBelow ? (pointX < width) : (pointX > width);
            }
            else
            {
                result = true;
            }

            return result;
        }

        /// <summary>
        /// Makes group or single element docking.
        /// </summary>
        /// <param name="recordPreview">The record preview.</param>
        private void MakeDocking(DockPreviewRecord recordPreview)
        {
            
            DockAbility elementability = DockingManager.GetDockAbility(m_draggedElement);
            if (AllowDock(elementability, recordPreview.Side))
            {
                InsertIntoContainer(m_draggedElement, recordPreview.TargetElement, DockState.Float, recordPreview.State, recordPreview.Side, recordPreview.PreviewSize);

                if (recordPreview.TargetElement != null)
                    DockingManager.SetDockedElementsContainerDesiredSize(recordPreview.TargetElement, new Size(0, 0));
            }
        }

        /// <summary>
        /// Gets the tabbed preview record.
        /// </summary>
        /// <returns>return result</returns>
        private DockPreviewRecord GetTabbedPrevievRecord()
        {
            FrameworkElement hostedElement = m_hostUnderMouse.HostedElement;

            DockPreviewRecord result = new DockPreviewRecord
            {
                Side = DockSide.Tabbed,
                Element = m_draggedElement,
                TargetElement = hostedElement,
                State = DockingManager.GetState(hostedElement)
            };

            return result;
        }

        /// <summary>
        /// Allows the dock.
        /// </summary>
        /// <returns>return bool value.</returns>
        private bool AllowDock()
        {
            return DraggingType == DraggingType.NormalDragging || IsntSelf(m_hostUnderMouse);
        }

        /// <summary>
        /// Processing's the selection in tabs.
        /// </summary>
        /// <param name="host">The DockedElementTabbedHost.</param>
        private void ProcessingSelectionInTabs(DockedElementTabbedHost host)
        {
            foreach (FrameworkElement tabbed in host.InternalTabControl.Items)
            {
                DockingManager.SetIsSelectedTab(tabbed, false);
            }

            DockingManager.SetIsSelectedTab(m_holdList[0], true);
        }

        /// <summary>
        /// Processing's the create main preview.
        /// </summary>
        /// <param name="host">The FrameworkElement.</param>
        internal void ProcessingCreateMainPreview(FrameworkElement host)
        {
            if (m_managerDragPreview != null)
            {
                m_managerDragPreview.IsLowPerform = true;
                m_managerDragPreview.CreateDockPreviewMainButton(host, true);
            }
        }
       
        /// <summary>
        /// Processing's the create dock preview.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        internal void ProcessingCreateDockPreview(InputEventArgs e)
        {
            Point point = new Point(0, 0);
            if (e is MouseEventArgs)
                point = (e as MouseEventArgs).GetPosition(m_hostUnderMouse);
#if !SyncfusionFramework3_5
            else if (e is TouchEventArgs)
                point = ((e as TouchEventArgs).GetTouchPoint(m_hostUnderMouse) as TouchPoint).Position;
#endif
            DockPreviewRecord? record = m_managerDragPreview.FindDockingPlace(m_draggedElement, m_hostUnderMouse, point);

            if (record != null)
            {
                if (record.Value.State == DockState.Dock
                || (record.Value.State == DockState.Float))
                {
                    m_managerDragPreview.ShowDockPreview(record.Value);
                }
            }
        }

        /// <summary>
        /// Isnts the self.
        /// </summary>
        /// <param name="host">The docked element tabbed host.</param>
        /// <returns>return bool value.</returns>
        internal bool IsntSelf(DockedElementTabbedHost host)
        {
            return null != host && null != m_draggedElement && IsDragItem(host.TabChildren)
                && (DraggingType.NormalDragging != DraggingType || m_draggedElement != host.HostedElement);
        }

        /// <summary>
        /// Needs the preview.
        /// </summary>
        /// <param name="host">The docked element tabbed host.</param>
        /// <returns>return bool value.</returns>
        private bool NeedPreview(DockedElementTabbedHost host)
        {
            ObservableFrameworkElements tabChildren = host.TabChildren;

            return m_hostUnderMouse != host
                && (0 != tabChildren.Count ? IsDragItem(tabChildren) : host.HostedElement != m_draggedElement);
        }

        /// <summary>
        /// Determines whether [is drag item] [the specified tab children].
        /// </summary>
        /// <param name="tabChildren">The tab children.</param>
        /// <returns>
        /// <c>true</c> if [is drag item] [the specified tab children]; otherwise, <c>false</c>.
        /// </returns>
        internal bool IsDragItem(ICollection<FrameworkElement> tabChildren)
        {
            return DraggingType == DraggingType.NormalDragging || (1 < tabChildren.Count && m_DraggingSource == DraggingSource.Tab && !m_bLoadPreview) || !tabChildren.Contains(m_draggedElement);
        }

        /// <summary>
        /// Gets the host.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>return DockState.</returns>
        private DockedElementTabbedHost GetHost(FrameworkElement element)
        {
            DockState state = DockingManager.GetState(element);
            DockSide side = DockingManager.GetSide(element, state);

            if (side == DockSide.Tabbed && IsVisibleState(state))
            {
                string targetName = DockingManager.GetTargetName(element, state);
                element = FindChild(targetName);
            }
            if (element != null)
            {
                DockInfoInternal info = DockingManager.GetDockInfo(element);
                return DockState.Dock == state ? info.HostDock : info.HostFloat;
            }
            return null;
        }

        private void StartNormalDragging(NativeFloatWindow window)
        {
            m_holdList = GetContainerTabs(window.Content as DockedElementsContainer);
            if (window != null && window.PrimaryElement != null)
            {
                m_holdList.Remove(window.PrimaryElement);
                m_holdList.Insert(0, window.PrimaryElement);
            }
        }
        /// <summary>
        /// Starts the normal dragging.
        /// </summary>
        /// <param name="window">The window.</param>
        private void StartNormalDragging(IWindow window)
        {
            m_holdList = GetContainerTabs(window.FloatChild as DockedElementsContainer);
            if (window!=null && window.PrimaryElement != null)
            {
                m_holdList.Remove(window.PrimaryElement);
                m_holdList.Insert(0, window.PrimaryElement);
            }
        }

        /// <summary>
        /// Starts the light dragging.
        /// </summary>
        /// <param name="window">The window.</param>
        private void StartLightDragging(IWindow window)
        {
            m_holdList = GetContainerTabs(window.FloatChild as DockedElementsContainer);
            window.HitTestDisabled = true;
            if (window!=null && window.PrimaryElement != null)
            {
                CurrentDragPopup.StartDragging(window.PrimaryElement, m_holdList);
            }
        }
        /// <summary>
        /// Determines whether [is visible state] [the specified state].
        /// </summary>
        /// <param name="state">The state.</param>
        /// <returns>
        /// <c>true</c> if [is visible state] [the specified state]; otherwise, <c>false</c>.
        /// </returns>
        internal static bool IsVisibleState(DockState state)
        {
            return state == DockState.Dock || state == DockState.Float;
        }

        /// <summary>
        /// Determines whether [is visible dock window state] [the specified dockwindowstate].
        /// </summary>
        /// <param name="dockwindowstate">The dockwindowstate.</param>
        /// <returns>
        /// 	<c>true</c> if [is visible dock window state] [the specified dockwindowstate]; otherwise, <c>false</c>.
        /// </returns>
        internal static bool IsVisibleDockWindowState(WindowState dockwindowstate)
        {
            return dockwindowstate == WindowState.Maximized || dockwindowstate == WindowState.Minimized;
        }

        /// <summary>
        /// Gets the first visible sibling.
        /// </summary>
        /// <param name="siblings">The siblings.</param>
        /// <param name="state">The state.</param>
        /// <returns>return FrameworkElement</returns>
        private static FrameworkElement GetFirstVisibleSibling(List<FrameworkElement> siblings, DockState state)
        {
            FrameworkElement element = null;
            siblings.Sort(GetIndexComparer(state));
            siblings.Reverse();
            foreach (FrameworkElement sibling in siblings)
            {
                if (GetState(sibling) == state)
                {
                    element = sibling;
                    break;
                }
            }

            return element;
        }

        /// <summary>
        /// Processes the new state of the additional operations for.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="state">The state.</param>
        /// <param name="owner">The owner.</param>
        /// <param name="bOverrideTarget">if set to <c>true</c> [b override target].</param>
        /// <param name="overrideTargetName">Name of the override target.</param>
        /// <param name="side">The DockState.</param>
        private static void ProcessAdditionalOperationsForNewState(FrameworkElement element, DockState state, DockingManager owner, bool bOverrideTarget, string overrideTargetName, DockSide side)
        {
            FrameworkElement targetInNewState = DockingManager.GetTargetElement(element, state);

            if (targetInNewState != null)
            {
                DockState targetNewState = DockingManager.GetState(targetInNewState);

                if (state == DockState.Float && DockingManager.GetNoDock(targetInNewState))
                {
                    DockingManager.SetTargetNameInFloatingMode(element, string.Empty);
                }
                else if (ContainsTargetAsSibling(element, targetInNewState, state, owner) ||
                    targetNewState != state)
                {
                    updatedockflag = false;
                    if (!bOverrideTarget)
                    {
                        RestoreElement(element, state, owner);
                        ////SwapElementAndTarget( element, targetInNewState, state, owner );
                    }
                    else
                    {
                        DockingManager.SetTargetName(element, overrideTargetName, state);
                        DockingManager.SetSide(element, side, state);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the correct target for tabbed elements.
        /// </summary>
        /// <param name="element">The framework element.</param>
        /// <param name="state">The dock state.</param>
        /// <param name="side">The doc side.</param>
        /// <param name="owner">The docking manager owner.</param>
        /// <param name="bUpdate">if set to <c>true</c> [b update].</param>
        /// <returns>return correctTargetName value</returns>
        private static string GetCorrectTargetForTabbedElements(FrameworkElement element, DockState state, DockSide side, DockingManager owner, ref bool bUpdate)
        {
            string correctTargetName = element.Name;

            if (side == DockSide.Tabbed && IsVisibleState(state))
            {
                correctTargetName = DockingManager.GetTargetName(element, state);
                FrameworkElement target = owner.FindChild(correctTargetName);
                if (target != null)
                {
                    if (DockSide.Tabbed == DockingManager.GetSide(target, state))
                    {
                        correctTargetName = DockingManager.GetTargetName(target, state);
                        bUpdate = true;
                    }
                    else if (state != DockingManager.GetState(target))
                    {
                        correctTargetName = element.Name;
                    }
                }
            }

            return correctTargetName;
        }

        /// <summary>
        /// Restores the element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="state">The state.</param>
        /// <param name="owner">The owner.</param>
        internal static void RestoreElement(FrameworkElement element, DockState state, DockingManager owner)
        {
            string targetName = DockingManager.GetTargetNameSafe(element, state);

            if (!string.IsNullOrEmpty(targetName))
            {
                RestoreElementInternal(element, state, owner,false);
            }
        }

        /// <summary>
        /// Overloaded method to carry the IsMaximized variable to indicate the Maximized state.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="state"></param>
        /// <param name="owner"></param>
        /// <param name="IsMaximized"></param>
        internal static void RestoreElement(FrameworkElement element, DockState state, DockingManager owner, bool IsMaximized)
        {
            string targetName = DockingManager.GetTargetNameSafe(element, state);

            if (!string.IsNullOrEmpty(targetName))
            {
                RestoreElementInternal(element, state, owner, IsMaximized);
            }
        }

        /// <summary>
        /// Restores the element internal.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="state">The state.</param>
        /// <param name="owner">The owner.</param>
        private static void RestoreElementInternal(FrameworkElement element, DockState state, DockingManager owner, bool IsMaximized)
        {
            string targetName = DockingManager.GetTargetNameSafe(element, state);
            updatedockflag = false;

            if (targetName != String.Empty && targetName == element.Name)
            {
                DockingManager.SetTargetName(element, String.Empty, state);
            }

            if (!string.IsNullOrEmpty(targetName))
            {
                FrameworkElement target = owner.FindChild(targetName);
                if (target != null)
                {
                    DockState targetState = DockingManager.GetState(target);
                    DockSide elementSide = DockingManager.GetSideSafe(element, state);

                    if (targetState != state && elementSide == DockSide.Tabbed && !IsMaximized)
                    {
                        SwapElementAndTargetInternal(element, target, state, owner, true, true);
                        targetName = DockingManager.GetTargetNameSafe(element, state);

                        if (!string.IsNullOrEmpty(targetName))
                        {
                            target = owner.FindChild(targetName);
                            if (target != null)
                            {
                                targetState = DockingManager.GetState(target);
                            }
                        }
                        else
                        {
                            targetState = DockState.Dock;
                        }
                    }

                    while (targetState != state && !IsMaximized && !DockingManager.GetIsSwapped(target))
                    {
                        ////SwapSizes( element, target, state );
                        SwapElementAndTargetInternal(element, target, state, owner, false, false);
                        SwapIndexes(element, target, state, owner);
                        targetName = DockingManager.GetTargetNameSafe(element, state);

                        if (!string.IsNullOrEmpty(targetName))
                        {
                            target = owner.FindChild(targetName);
                            if (target != null)
                            {
                                targetState = DockingManager.GetState(target);
                            }
                        }
                        else
                        {
                            DockingManager.SetTargetNameSafe(element, targetName, state);
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Swaps the sizes.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="target">The target.</param>
        /// <param name="state">The state.</param>
        private static void SwapSizes(FrameworkElement element, FrameworkElement target, DockState state)
        {
            DockSide side = DockingManager.GetSide(element, state);

            if (IsHorizontalDockSide(side))
            {
                double elementWidth = DockingManager.GetDesiredWidth(element, state);
                double targetWidth = DockingManager.GetDesiredWidth(target, state);

                DockingManager.SetDesiredWidth(element, state, targetWidth);
                DockingManager.SetDesiredWidth(target, state, targetWidth - elementWidth);
            }
            else
            {
                double targetWidth = DockingManager.GetDesiredWidth(target, state);
                DockingManager.SetDesiredWidth(element, state, targetWidth);
            }
        }

        /// <summary>
        /// Gets the correct index for sibling.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="target">The target.</param>
        /// <param name="state">The state.</param>
        /// <param name="owner">The owner.</param>
        /// <param name="targetSiblings">The target siblings.</param>
        /// <returns>Return min index value.</returns>
        private static int GetCorrectIndexForSibling(FrameworkElement element, FrameworkElement target, DockState state, DockingManager owner, List<FrameworkElement> targetSiblings)
        {
            List<FrameworkElement> siblings = owner.FindSiblingsSafe(element, state);

            if (targetSiblings != null)
            {
                foreach (FrameworkElement sibling in targetSiblings)
                {
                    siblings.Remove(sibling);
                }
            }

            int iMinIndex = DockingManager.GetIndex(target, state);

            foreach (FrameworkElement sibling in siblings)
            {
                if (DockSide.Tabbed != DockingManager.GetSideSafe(sibling, state))
                {
                    int iIndex = DockingManager.GetIndex(sibling, state);
                    iMinIndex = (iIndex < iMinIndex) ? iIndex : iMinIndex;
                }
            }

            return iMinIndex;
        }

        /// <summary>
        /// Swaps the indexes.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="target">The target.</param>
        /// <param name="state">The state.</param>
        /// <param name="owner">The owner.</param>
        private static void SwapIndexes(FrameworkElement element, FrameworkElement target, DockState state, DockingManager owner)
        {
            SwapIndexes(element, target, state, owner, null);
        }

        /// <summary>
        /// Swaps the indexes.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="target">The target.</param>
        /// <param name="state">The state.</param>
        /// <param name="owner">The owner.</param>
        /// <param name="targetSiblings">The target siblings.</param>
        private static void SwapIndexes(FrameworkElement element, FrameworkElement target, DockState state, DockingManager owner, List<FrameworkElement> targetSiblings)
        {
            int iElementIndex = DockingManager.GetIndex(element, state);
            int iTargetIndex = DockingManager.GetIndex(target, state);

            DockingManager.SetIndex(element, state, iTargetIndex);
            DockingManager.SetIndex(target, state, iElementIndex);

            int iIndex = GetCorrectIndexForSibling(element, target, state, owner, targetSiblings);
            owner.SetChildIndex(target, state, iIndex);
        }

        /// <summary>
        /// Swaps the element and target.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="target">The target.</param>
        /// <param name="state">The state.</param>
        /// <param name="owner">The owner.</param>
        private static void SwapElementAndTarget(FrameworkElement element, FrameworkElement target, DockState state, DockingManager owner)
        {
            updatedockflag = false;
            DockState targetState = DockingManager.GetState(target);
            string targetParentName = GetFirstStateCompatibleTargetName(target, state, ResolveManager(target));
            DockSide targetSide = DockingManager.GetSide(target, state);

            if (state != targetState || ContainsTargetAsSibling(element, target, state, owner))
            {
                SwapElementAndTargetInternal(element, target, state, owner, false, true);
            }
            else if (targetSide == DockSide.Tabbed)
            {
                DockingManager.SetTargetName(element, targetParentName, state);
            }
        }

        /// <summary>
        /// Gets the first name of the state compatible target.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="state">The state.</param>
        /// <param name="owner">The owner.</param>
        /// <returns>Return target state value.</returns>
        private static string GetFirstStateCompatibleTargetName(DependencyObject element, DockState state, DockingManager owner)
        {
            string result = string.Empty;

            string targetName = DockingManager.GetTargetName(element, state);

            if (!string.IsNullOrEmpty(targetName))
            {
                FrameworkElement target = owner.FindChild(targetName);
                if (target != null)
                {
                    DockState targetState = DockingManager.GetState(target);
                    result = targetState == state ? targetName : GetFirstStateCompatibleTargetName(target, state, owner);
                }
            }

            return result;
        }

        /// <summary>
        /// Determines whether [contains target as sibling] [the specified element].
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="target">The target.</param>
        /// <param name="state">The state.</param>
        /// <param name="owner">The owner.</param>
        /// <returns>
        /// <c>true</c> if [contains target as sibling] [the specified element]; otherwise, <c>false</c>.
        /// </returns>
        private static bool ContainsTargetAsSibling(FrameworkElement element, FrameworkElement target, DockState state, DockingManager owner)
        {
            List<FrameworkElement> siblings = owner.FindSiblings(element, state, false);
            return siblings.Contains(target);
        }

        /// <summary>
        /// Determines whether [contains target as sibling] [the specified element].
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="targetName">Name of the target.</param>
        /// <param name="state">The state.</param>
        /// <param name="owner">The owner.</param>
        /// <returns>
        /// <c>true</c> if [contains target as sibling] [the specified element]; otherwise, <c>false</c>.
        /// </returns>
        private static bool ContainsTargetAsSibling(FrameworkElement element, string targetName, DockState state, DockingManager owner)
        {
            FrameworkElement target = owner.FindChild(targetName);
            if (target != null)
            {
                List<FrameworkElement> siblings = owner.FindSiblings(element, state, false);
                return siblings.Contains(target);
            }
            return false;
        }

        /// <summary>
        /// Gets the side for element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="state">The state.</param>
        /// <param name="siblings">The siblings.</param>
        /// <param name="overridedTargetName">Name of the override target.</param>
        /// <returns>Return DockSide result.</returns>
        private static DockSide GetSideForElement(DependencyObject element, DockState state, IEnumerable<FrameworkElement> siblings, ref string overridedTargetName)
        {
            DockSide resultSide = DockingManager.GetSide(element, state);

            foreach (FrameworkElement sibling in siblings)
            {
                if (DockingManager.GetSide(sibling, state) != DockSide.Tabbed)
                {
                    resultSide = DockingManager.GetSide(sibling, state);
                    overridedTargetName = DockingManager.GetTargetName(sibling, state);
                    break;
                }
            }

            return resultSide;
        }

        /// <summary>
        /// Sets the size desired.
        /// </summary>
        /// <param name="record">The record.</param>
        private static void SetSizeDesired(DockPreviewRecord record)
        {
            if (record.Side != DockSide.Tabbed)
            {
                DockedElementTabbedHost targetHost = record.TargetElement == null ? null
                    : DockingManager.ResolveHost(record.TargetElement, record.State);

                bool bIsHorizontal = IsHorizontalDockSide(record.Side);

                if (targetHost != null)
                {
                    double length = bIsHorizontal
                        ? targetHost.ActualWidth - record.PreviewSize
                        : targetHost.ActualHeight - record.PreviewSize;

                    ////double targetLength = ( bIsHorizontal )
                    ////    ? targetHost.ActualHeight : targetHost.ActualWidth;

                    double targetLength = bIsHorizontal
                        ? GetDesiredHeight(targetHost.HostedElement, record.State)
                        : GetDesiredWidth(targetHost.HostedElement, record.State);

                    SetDesiredSize(record.TargetElement, record.State, length, bIsHorizontal);
                    SetDesiredSize(record.Element, record.State, targetLength, !bIsHorizontal);
                }

                SetDesiredSize(record.Element, record.State, record.PreviewSize, bIsHorizontal);
            }
        }

        /// <summary>
        /// Gets the size of the desired.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="state">The state.</param>
        /// <param name="bIsHorizontal">if set to <c>true</c> [b is horizontal].</param>
        /// <returns>Return dock state result.</returns>
        private static double GetDesiredSize(DependencyObject element, DockState state, bool bIsHorizontal)
        {
            double result = 0;

            if (bIsHorizontal)
            {
                result = DockingManager.GetDesiredWidth(element, state);
            }
            else
            {
                result = DockingManager.GetDesiredHeight(element, state);
            }

            return result;
        }

        /// <summary>
        /// Sets the size of the desired.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="state">The state.</param>
        /// <param name="value">The value.</param>
        /// <param name="bIsHorizontal">if set to <c>true</c> [b is horizontal].</param>
        private static void SetDesiredSize(DependencyObject element, DockState state, double value, bool bIsHorizontal)
        {
            if (bIsHorizontal)
            {
                DockingManager.SetDesiredWidth(element, state, value);
            }
            else
            {
                if (DockState.Dock == state)
                {
                    DockingManager.SetDesiredHeightInDockedMode(element, value);
                }
                else
                {
                    DockingManager.SetDesiredHeightInFloatingMode(element, value);
                }
            }
        }

        /// <summary>
        /// Determines whether [is horizontal dock side] [the specified side].
        /// </summary>
        /// <param name="side">The DockSide value.</param>
        /// <returns>
        /// <c>true</c> if [is horizontal dock side] [the specified side]; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsHorizontalDockSide(DockSide side)
        {
            return side == DockSide.Left || side == DockSide.Right;
        }

        /// <summary>
        /// Compares is same orientation.
        /// </summary>
        /// <param name="sideX">The side X.</param>
        /// <param name="sideY">The side Y.</param>
        /// <returns>
        /// <c>true</c> if [is same orientation] [the specified side X]; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsSameOrientation(DockSide sideX, DockSide sideY)
        {
            bool bIsHorizontalSideX = IsHorizontalDockSide(sideX);
            bool bIsHorizontalSideY = IsHorizontalDockSide(sideY);
            bool result = bIsHorizontalSideX == bIsHorizontalSideY;

            if (result && (sideX != sideY && (sideX == DockSide.Tabbed || sideY == DockSide.Tabbed)))
            {
                result = false;
            }

            return result;
        }

        /// <summary>
        /// Gets the last tab order.
        /// </summary>
        /// <param name="tabs">The tabs list.</param>
        /// <returns>return the result.</returns>
        private static int GetLastTabOrder(IEnumerable<FrameworkElement> tabs)
        {
            int result = 0;

            foreach (FrameworkElement tab in tabs)
            {
                DockState tabState = DockingManager.GetState(tab);

                tabState = (tabState == DockState.AutoHidden) ? DockState.Dock
                    : (tabState == DockState.Hidden) ? DockingManager.GetPreviousState(tab) : tabState;

                int order = DockedElementTabbedHost.GetTabOrder(tab, tabState);
                result = (order > result) ? order : result;
            }

            return result;
        }

        /// <summary>
        /// Corrects the indexes.
        /// </summary>
        /// <param name="list">The FrameworkElement list.</param>
        /// <param name="state">The state.</param>
        /// <param name="iTargetIndex">Index of the i target.</param>
        /// <param name="bAllowReplaceIndex">if set to <c>true</c> [b allow replace index].</param>
        private void CorrectIndexes(IEnumerable<FrameworkElement> list, DockState state, int iTargetIndex, bool bAllowReplaceIndex)
        {
            int iIndex = Children.Count - 1;
            bool bTargetShifted = false;

            if (bAllowReplaceIndex && iTargetIndex == iIndex)
            {
                //iTargetIndex--;
                bTargetShifted = true;
            }

            foreach (FrameworkElement element in list)
            {
                int iElementIndex = DockingManager.GetIndex(element, state);

                if (bAllowReplaceIndex && !bTargetShifted && (iElementIndex < iTargetIndex))
                {
                    iElementIndex = iTargetIndex - 1;
                }
                else
                {
                    iElementIndex = iTargetIndex;
                }

                SetChildIndex(element, state, iIndex);
                SetChildIndex(element, state, iElementIndex);
            }
        }

        /// <summary>
        /// Called when [direct tab panel mouse down].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        internal void OnDirectTabPanelMouseDown(object sender, MouseEventArgs e)
        {
            if (e.StylusDevice == null || e.StylusDevice != null)
            {
                DirectTabPanel panel = (DirectTabPanel)sender;
                if (this.Dispatcher.CheckAccess())
                {
                    if (IsDockTabPanel(panel))
                    {
                        panel.CaptureMouse();
                    }
                    if (!BrowserInteropHelper.IsBrowserHosted && MouseEnter)
                    {
                        ActivateParent();
                    }
                }
            }
        }

#if !SyncfusionFramework3_5
        //internal void OnDirectTabPanelTouchDown(object sender, TouchEventArgs e)
        //{
        //    DirectTabPanel panel = (DirectTabPanel)sender;
        //    if (this.Dispatcher.CheckAccess())
        //    {
        //        if (IsDockTabPanel(panel))
        //        {
        //            panel.CaptureTouch(e.TouchDevice);
        //        }
        //        if (!BrowserInteropHelper.IsBrowserHosted && MouseEnter)
        //        {
        //            ActivateParent();
        //        }
        //    }
        //}

        //internal void OnDirectTabPanelLostTouchCapture(object sender, TouchEventArgs e)
        //{
        //    if (this.Dispatcher.CheckAccess())
        //    {
        //        if (!m_isTabPressed)
        //        {
        //            StopTabItemDragging((DirectTabPanel)sender);
        //        }
        //    }
        //}
#endif
        /// <summary>
        /// for parent window activation
        /// </summary>
        private void ActivateParent()
        {
            FrameworkElement element = Parent as FrameworkElement;

            if (element != null)
            {
                while (element.Parent != null)
                {
                    element = element.Parent as FrameworkElement;
                    if (element != null && element is Window)
                    {
                        if (!(element as Window).IsActive)
                        {
                            (element as Window).Activate();
                            break;
                        }
                    }
                }
            }
            //this.CaptureMouse();
           

        }


        /// <summary>
        /// Determines whether [is dock tab item] [the specified tab item].
        /// </summary>
        /// <param name="tabItem">The tab item.</param>
        /// <returns>
        /// <c>true</c> if [is dock tab item] [the specified tab item]; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsDockTabItem(FrameworkElement tabItem)
        {
            return null != tabItem.Tag && TAG_INTERNAL_TAB_ITEM == tabItem.Tag.ToString();
        }

        /// <summary>
        /// Called when [tab item mouse double click].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private static void OnTabItemMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.StylusDevice == null || e.StylusDevice != null)
            {
                TabItem tabItem = (TabItem)sender;

                if (DockingManager.IsDockTabItem(tabItem) && MouseButtonState.Pressed == e.LeftButton)
                {
                    FrameworkElement element = (FrameworkElement)tabItem.Content;
                    m_floatWindowRect = DockingManager.GetFloatingWindowRect(element);

                    if (null != element)
                    {
                        DockState oldState = DockingManager.GetState(element);
                        DockState newState = (DockState.Dock == oldState) ? DockState.Float : DockState.Dock;
                        DockingManager owner = DockingManager.ResolveManager(element);

                        bool bCanChangeState = DockingManager.CanChangeState(element, newState);

                        if (bCanChangeState)
                        {
                            DockedElementTabbedHost.RemoveTab(element, oldState);
                            owner.ExecuteDoubleClick(element, ActionMode.Active);
                            DockedElementTabbedHost.AddTab(element, false);
                            DockedElementTabbedHost.SelectTab(element, newState);
                            DockingManager.SetNewFocusedElement(element);
                            owner.LockLayoutUpdate = true;
                        }

                        e.Handled = true;
                    }
                }
            }
        }

        /// <summary>
        /// Called when [listen tab item events changed].
        /// </summary>
        /// <param name="d">The dependencyObject value.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnListenTabItemEventsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabItem tabItem = d as TabItem;

            if (tabItem!=null && VisualUtils.FindAncestor(tabItem, typeof(DockedElementTabbedHost)) != null)
            {
                tabItem.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(OnTabItemPreviewMouseLeftButtonDown);
                tabItem.PreviewMouseRightButtonDown += new MouseButtonEventHandler(OnTabItemPreviewMouseRightButtonDown);
                tabItem.MouseDoubleClick += new MouseButtonEventHandler(OnTabItemMouseDoubleClick);
#if !SyncfusionFramework3_5
                //tabItem.TouchUp += tabItem_TouchUp;
                //tabItem.PreviewTouchDown += tabItem_PreviewTouchDown;
                //tabItem.PreviewTouchMove += tabItem_PreviewTouchMove;
#endif
            }
        }

        /// <summary>
        /// Called when [tab item preview mouse left button down].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private static void OnTabItemPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.StylusDevice == null || e.StylusDevice != null)
            {
                try
                {
                    TabItem tabItem = (TabItem)sender;
                    FrameworkElement element = (FrameworkElement)tabItem.Content;

                    if (element != null && DockingManager.GetCanDrag(element))
                    {
                        DockInfoInternal info = DockingManager.GetDockInfo(element);
                        if (info != null)
                        {
                            info.DockingManager.m_isTabPressed = true;
                            info.DockingManager.m_holdList = new List<FrameworkElement>(1) { element };
                            info.DockingManager.m_floatWindowRectCoord = GetFloatingWindowRect(element);
                            info.DockingManager.m_rectBeforeAddingTab = Rect.Empty;
                        }
                    }
                }
                catch { }
            }
        }

        /// <summary>
        /// Called when [TabItemExt preview mouse left button down].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        internal static void OnTabItemExtPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.StylusDevice == null || e.StylusDevice != null)
            {
                TabItemExt tabItem = (TabItemExt)sender;
                FrameworkElement element = (FrameworkElement)tabItem.Content;

                if (element != null && DockingManager.GetCanDrag(element))
                {
                    DockingManager.m_holdTabItemExtList = new List<FrameworkElement>(1) { element };
                }
            }
        }
        /// <summary>
        /// Called when [TabItemExt preview mouse left button Up].
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal static void OnTabItemExtPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (e.StylusDevice == null || e.StylusDevice != null)
            {
                DockingManager.m_holdTabItemExtList = null;
            }
        }

        /// <summary>
        /// Called when [tab item preview mouse right button down].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private static void OnTabItemPreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.StylusDevice == null || e.StylusDevice != null)
            {
                TabItem tabItem = (TabItem)sender;
                FrameworkElement element = (FrameworkElement)tabItem.Content;
                DockingManager.SelectTab(element);
            }
        }

        /// <summary>
        /// Processing's the set on top float window.
        /// </summary>
        /// <param name="hostedElement">The hosted element.</param>
        private static void ProcessingSetOnTopFloatWindow(DependencyObject hostedElement)
        {
            DockInfoInternal info = DockingManager.GetDockInfo(hostedElement);
            DockingManager owner = DockingManager.GetDockingManager(hostedElement);
            if (owner != null && owner.UseNativeFloatWindow)
            {
                if (info.NativeWindow != null)
                {
                    info.NativeWindow.BringIntoView();
                }
            }
            else
            {
                if (info.FloatingWindow != null)
                {
                    info.FloatingWindow.SetWindowOnTop();
                }
            }

        }

        /// <summary>
        /// Determines whether [is in tab panel] [the specified tab panel].
        /// </summary>
        /// <param name="tabPanel">The tab panel.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        /// <returns>
        /// <c>true</c> if [is in tab panel] [the specified tab panel]; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsInTabPanel(UIElement tabPanel, InputEventArgs e)
        {
            bool result = false;

            if (null != tabPanel)
            {
                Point point = new Point(0, 0);
                if (e is MouseEventArgs)
                    point = (e as MouseEventArgs).GetPosition(tabPanel);
#if !SyncfusionFramework3_5
                else if (e is TouchEventArgs)
                    point = ((e as TouchEventArgs).GetTouchPoint(tabPanel) as TouchPoint).Position;
#endif
                Rect rect = new Rect(tabPanel.RenderSize);

                result = rect.Contains(point) && point.X != 0 && point.Y != 0;
            }

            return result;
        }

        /// <summary>
        /// Determines whether [is dock tab panel] [the specified panel].
        /// </summary>
        /// <param name="panel">The panel.</param>
        /// <returns>
        /// <c>true</c> if [is dock tab panel] [the specified panel]; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsDockTabPanel(IFrameworkInputElement panel)
        {
            return null != panel && TAB_PANEL_NAME == panel.Name;
        }

        /// <summary>
        /// Determines whether this instance can dock the specified hosted element.
        /// </summary>
        /// <param name="hostedElement">The hosted element.</param>
        /// <returns>
        /// <c>true</c> if this instance can dock the specified hosted element; otherwise, <c>false</c>.
        /// </returns>
        private static bool CanDock(DependencyObject hostedElement)
        {
            return null == hostedElement || !DockingManager.GetNoDock(hostedElement);
        }

        /// <summary>
        /// Determines whether this instance [can dock to target] the specified element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="target">The target.</param>
        /// <returns>
        /// <c>true</c> if this instance [can dock to target] the specified element; otherwise, <c>false</c>.
        /// </returns>
        private static bool CanDockToTarget(FrameworkElement element, DockedElementTabbedHost target)
        {
            bool bCanDockToTarget = true;
            bool canDock = !ContainsNotAllowedStateElement(element, DockState.Dock);

            if (!canDock && target != null && target.HostedElement != null && DockingManager.GetState(target.HostedElement) == DockState.Dock)
            {
                bCanDockToTarget = false;
            }

            return bCanDockToTarget;
        }

        /// <summary>
        /// This method sets stated property for panel children.
        /// </summary>
        /// <param name="panel">Instance panel</param>
        /// <param name="depProperty">Stated property.</param>
        /// <param name="start">Index first child.</param>
        /// <param name="count">Count child.</param>
        /// <param name="increment">Increment for correction sets value.</param>
        private static void SetTabOrderProperties(Panel panel, DependencyProperty depProperty, int start, int count, int increment)
        {
            count = (panel.Children.Count > count) ? count : panel.Children.Count;

            for (int i = start; i < count; ++i)
            {
                TabItem item = (TabItem)panel.Children[i];
                FrameworkElement element = (FrameworkElement)item.Content;
                element.SetValue(depProperty, i + increment);
            }
        }

        private void TraverseHitTestHost(List<DockedElementTabbedHost> hostcollection, InputEventArgs e, DockState state, ref bool b_executed)
        {
            foreach (DockedElementTabbedHost item in hostcollection)
            {
                DockedElementTabbedHost host2;
                if (testresult != null)
                    testresult.Clear();
                if (state == DockState.Float)
                    VisualTreeHelper.HitTest(item, null, new HitTestResultCallback(ObtainedHitResultfromFloat), new PointHitTestParameters(Mouse.GetPosition(item)));
                else
                    VisualTreeHelper.HitTest(item, null, new HitTestResultCallback(ObtainedHitResult), new PointHitTestParameters(Mouse.GetPosition(item)));

                if (testresult != null && testresult.Count > 0)
                {
                    foreach (HitTestResult result in testresult)
                    {
                        host2 = VisualUtils.FindAncestor(result.VisualHit as Visual, typeof(DockedElementTabbedHost)) as DockedElementTabbedHost;

                        if (host2 != null && host2.Visibility == Visibility.Visible)
                        {
                            this.OnDockingManagerMouseMove(host2, e);
                            this.OnMouseMoveOnHost(host2, e);
                            b_executed = true;
                        }
                    }
                }

            }
        }

        private HitTestResultBehavior ObtainedHitResult(HitTestResult result)
        {
            if (testresult == null)
                testresult = new List<HitTestResult>();

            testresult.Add(result);
            return HitTestResultBehavior.Stop;
        }

        private HitTestResultBehavior ObtainedHitResultfromFloat(HitTestResult result)
        {
            if (testresult == null)
                testresult = new List<HitTestResult>();

            testresult.Add(result);
            return HitTestResultBehavior.Continue;
        }
        #endregion

        #region Dependency properties
        /// <summary>
        /// Identifies DockingManager.ListenTabItemEvents dependency property.
        /// </summary>
        public static readonly DependencyProperty ListenTabItemEventsProperty =
          DependencyProperty.RegisterAttached("ListenTabItemEvents", typeof(bool), typeof(DockingManager), new UIPropertyMetadata(false, new PropertyChangedCallback(OnListenTabItemEventsChanged)));

        /// <summary>
        /// Identifies DockingManager.IsVS2010DraggingEnabled dependency property.
        /// </summary>
        public static readonly DependencyProperty IsVS2010DraggingEnabledProperty =
            DependencyProperty.Register("IsVS2010DraggingEnabled", typeof(bool), typeof(DockingManager), new UIPropertyMetadata(null));

        /// <summary>
        /// Identifies DockingManager.IsTouchEnabled dependency property.
        /// </summary>
        internal static readonly DependencyProperty IsTouchEnabledProperty =
            DependencyProperty.Register("IsTouchEnabled", typeof(bool), typeof(DockingManager), new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Identifies DockingManager.CenterDragProvider dependency property.
        /// </summary>
        public static readonly DependencyProperty CenterDragProviderProperty =
            DependencyProperty.Register("CenterDragProvider", typeof(ControlTemplate), typeof(DockingManager), new UIPropertyMetadata(null));

        /// <summary>
        /// Identifies DockingManager.LeftDragProvider dependency property.
        /// </summary>
        public static readonly DependencyProperty LeftDragProviderProperty =
            DependencyProperty.Register("LeftDragProvider", typeof(ControlTemplate), typeof(DockingManager), new UIPropertyMetadata(null));

        /// <summary>
        /// Identifies DockingManager.TopDragProvider dependency property.
        /// </summary>
        public static readonly DependencyProperty TopDragProviderProperty =
            DependencyProperty.Register("TopDragProvider", typeof(ControlTemplate), typeof(DockingManager), new UIPropertyMetadata(null));

        /// <summary>
        /// Identifies DockingManager.RightDragProvider dependency property.
        /// </summary>
        public static readonly DependencyProperty RightDragProviderProperty =
            DependencyProperty.Register("RightDragProvider", typeof(ControlTemplate), typeof(DockingManager), new UIPropertyMetadata(null));

        /// <summary>
        /// Identifies DockingManager.BottomDragProvider dependency property.
        /// </summary>
        public static readonly DependencyProperty BottomDragProviderProperty =
            DependencyProperty.Register("BottomDragProvider", typeof(ControlTemplate), typeof(DockingManager), new UIPropertyMetadata(null));

        /// <summary>
        /// Identifies DockingManager.DraggingType dependency property.
        /// </summary>
        public static readonly DependencyProperty DraggingTypeProperty =
            DependencyProperty.Register("DraggingType", typeof(DraggingType), typeof(DockingManager), new UIPropertyMetadata(DraggingType.NormalDragging));

        /// <summary>
        /// Identifies DockingManager.IsDragging dependency property.
        /// </summary>
        public static readonly DependencyProperty IsDraggingProperty =
            DependencyProperty.Register("IsDragging", typeof(bool), typeof(DockingManager), new UIPropertyMetadata(false));

        /// <summary>
        /// Identifies DockingManager.RestrictWindowMinimumSize dependency property.
        /// </summary>
        public static readonly DependencyProperty RestrictWindowMinimumSizeProperty =
            DependencyProperty.Register("RestrictWindowMinimumSize", typeof(bool), typeof(DockingManager));
        #endregion
    }
}
