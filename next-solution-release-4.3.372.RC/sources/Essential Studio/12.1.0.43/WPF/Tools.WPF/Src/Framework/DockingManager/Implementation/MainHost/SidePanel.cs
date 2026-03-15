// <copyright file="SidePanel.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents SidePanel of the <see cref="DockingManager"/>.
    /// </summary>
    /// <remarks>
    /// SidePanel is useful to implement auto-hide behavior of docked windows.
    /// When you click auto-hide button in the docked window it hides to the side panel.
    /// Side panel can be placed on top, bottom, right or left side of docking container.
    /// </remarks>
    /// <example>
    /// <para/>This example shows how to use the SidePanel in XAML.
    /// <code language="XAML">
    /// <![CDATA[
    /// <Syncfusion:SidePanel Name="PART_TopPanel" panelSide="Top" Grid.Row="0" Grid.Column="1"
    /// TabStripPlacement="Top" ContentRenderTransformY="1">
    /// <Syncfusion:SidePanel.Triggers>
    /// <EventTrigger RoutedEvent="Syncfusion:SidePanel.ShowEvent" >
    /// <EventTrigger.Actions>
    /// <BeginStoryboard>
    /// <Storyboard>
    /// <DoubleAnimation x:Name="PART_TopShowAnimation"
    /// Storyboard.TargetProperty="ContentRenderTransformY"
    /// To="0" BeginTime="{StaticResource ShadowTimeSpan}"
    /// Duration="{Binding Path=(TabControl.SelectedItem).(Syncfusion:DockingManager.AnimationDelay)
    /// , RelativeSource={RelativeSource FindAncestor, AncestorType={x:Type Syncfusion:SidePanel}}}" />
    /// </Storyboard>
    /// </BeginStoryboard>
    /// </EventTrigger.Actions>
    /// </EventTrigger>
    /// <EventTrigger RoutedEvent="Syncfusion:SidePanel.HideEvent" >
    /// <EventTrigger.Actions>
    /// <BeginStoryboard>
    /// <Storyboard>
    /// <DoubleAnimation x:Name="PART_TopHideAnimation"
    /// Storyboard.TargetProperty="ContentRenderTransformY" To="1"
    /// Duration="{Binding Path=(TabControl.SelectedItem).(Syncfusion:DockingManager.AnimationDelay)
    /// , RelativeSource={RelativeSource FindAncestor, AncestorType={x:Type Syncfusion:SidePanel}}}" />
    /// </Storyboard>
    /// </BeginStoryboard>
    /// </EventTrigger.Actions>
    /// </EventTrigger>
    /// </Syncfusion:SidePanel.Triggers>
    /// </Syncfusion:SidePanel>
    /// ]]>
    /// </code>
    /// </example>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class SidePanel : TabControl
    {
        #region Constants
        /// <summary>
        /// Specify the border name.
        /// </summary>
        private const string BorderName = "PART_BorderName";

        /// <summary>
        /// Specify the header.
        /// </summary>
        private const string HeaderName = "PART_Header";

        /// <summary>
        /// Specify the side splitter.
        /// </summary>
        private const string SideSplitterName = "PART_SideSplitter";

        /// <summary>
        /// Specify the left hide animation.
        /// </summary>
        private const string LeftHideAnimationName = "PART_LeftHideAnimation";

        /// <summary>
        /// Specify the right hide animation.
        /// </summary>
        private const string RightHideAnimationName = "PART_RightHideAnimation";

        /// <summary>
        /// Specify the top hide animation.
        /// </summary>
        private const string TopHideAnimationName = "PART_TopHideAnimation";

        /// <summary>
        /// Specify the bottom hide animation.
        /// </summary>
        private const string BottomHideAnimationName = "PART_BottomHideAnimation";

        /// <summary>
        /// Specify the left show animation.
        /// </summary>
        private const string LeftShowAnimationName = "PART_LeftShowAnimation";

        /// <summary>
        /// Specify the right show animation.
        /// </summary>
        private const string RightShowAnimationName = "PART_RightShowAnimation";

        /// <summary>
        /// Specify the top show animation.
        /// </summary>
        private const string TopShowAnimationName = "PART_TopShowAnimation";

        /// <summary>
        /// Specify the bottom show animation.
        /// </summary>
        private const string BottomShowAnimationName = "PART_BottomShowAnimation";

        /// <summary>
        /// Specify the awl button.
        /// </summary>
        private const string AwlButtonName = "PART_AwlButton";

        /// <summary>
        /// Specify the left close button.
        /// </summary>
        private const string CloseButtonName = "PART_CloseButton";

        /// <summary>
        /// Specify the left shadow.
        /// </summary>
        private const string ShadowName = "PART_Shadow";

        /// <summary>
        /// Specify the delay time.
        /// </summary>
        private const double DelayTime = 100;

        /// <summary>
        /// Specify the minimum width.
        /// </summary>
        private const double minElementWidth = 55d;

        /// <summary>
        /// Specify the minimum height.
        /// </summary>
        private const double minElementHeight = 25d;

        private const string m_popup = "PART_PopupPanel";
        #endregion

        #region Private members
        private DockHeaderPresenter header;
        private Border border;
        internal ScrollButtonsBar m_leftscrollingbuttons;
        internal ScrollButtonsBar m_topscrollingbuttons;

        /// <summary>
        /// Specify the timer.
        /// </summary>
        private readonly DispatcherTimer m_Timer = new DispatcherTimer();

        /// <summary>
        /// Specify the context menu.
        /// </summary>
        private bool m_contextMenuOpen = false;

        /// <summary>
        ///  Flag to restrict animation on scrolling buttons
        /// </summary>
        private bool m_restrictanimation = false;

        /// <summary>
        /// Specify isShow.
        /// </summary>
        private bool m_isShow = false;

        /// <summary>
        /// Specify isNewContenthide animation.
        /// </summary>
        private bool m_isNewContentHideAnimation = false;

        /// <summary>
        /// Specify the splitter.
        /// </summary>
        private Splitter m_splitter = null;

        /// <summary>
        /// This member indicate whether can instance show content.
        /// </summary>
        private DockingManager m_owner = null;

        /// <summary>
        /// Specify isShowing.
        /// </summary>
        private bool m_isShowing = false;

        /// <summary>
        /// Specify inDragMode.
        /// </summary>
        private bool m_inDragMode = false;

        /// <summary>
        /// Specify the focused item.
        /// </summary>
        private object m_focusedItem = null;

        /// <summary>
        /// Specify the weak show animation.
        /// </summary>
        private bool m_weakShowAnimation = true;

        /// <summary>
        /// shows when hide content ( animation ).
        /// </summary>
        private bool m_weakHideAnimation = true;

        /// <summary>
        /// Specify the shadow.
        /// </summary>
        private Border m_shadow = null;

        /// <summary>
        /// Is used for defining hiding of the tab when sidePanel context menu is called.
        /// </summary>
        private bool m_isHideTabWhenMenuOpen = true;

        /// <summary>
        /// Specify the groups.
        /// </summary>
        private List<int> groups = new List<int>();

        /// <summary>
        /// Specify the context menu
        /// </summary>
        private ContextMenu m_contextMenu;

        /// <summary>
        /// last selected item
        /// </summary>
        private object lastselecteditem = null;
        private PopupSidePanel m_popuppanel;

        private Window parentwindow;

        private bool b_windowactivation = false;

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes static members of the SidePanel class.
        /// </summary>
        static SidePanel()
        {
            ShowEvent = EventManager.RegisterRoutedEvent("ShowEvent", RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(SidePanel));
            HideEvent = EventManager.RegisterRoutedEvent("HideEvent", RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(SidePanel));
        }

        /// <summary>
        /// Initializes a new instance of the SidePanel class.
        /// </summary>
        public SidePanel()
        {
            this.AllowDrop = true;
            TabChildren = new ObservableFrameworkElements();
            m_Timer.Tick += new EventHandler(OnTimerTick);
            m_Timer.Interval = TimeSpan.FromMilliseconds(DelayTime);
            BindingUtils.SetRelativeBinding(this, AutoHideAnimationModeProperty, typeof(DockingManager), DockingManager.AutoHideAnimationModeProperty, BindingMode.OneWay, 1);
            ItemsSource = TabChildren;
            this.Loaded += new RoutedEventHandler(SidePanel_Loaded);
            this.Unloaded += new RoutedEventHandler(SidePanel_Unloaded);
            TabChildren.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(TabChildren_CollectionChanged);
        }

        void TabChildren_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (InternalDataContext != null
                && InternalDataContext.Parent != null && InternalDataContext.Parent is DockingManager
                && (InternalDataContext.Parent as DockingManager).EnableScrollableSidePanel)
            {
                if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add && e.NewItems.Count>0)
                {
                    ScrollViewer scrollviewer = GetTemplateChild("PART_ScrollPanel") as ScrollViewer;
                    DirectTabPanel panel = GetTemplateChild("PART_PanelName") as DirectTabPanel;
                    if (scrollviewer != null && panel!=null)
                    {
                        scrollviewer.Measure(panel.RenderSize);
                        scrollviewer.InvalidateScrollInfo();                        
                    }
                }            
            }            
        }

        void SidePanel_Loaded(object sender, RoutedEventArgs e)
        {
            SelectionChanged += new SelectionChangedEventHandler(OnSidePanelSelectionChanged);
            DockingManager = this.Owner;
        }

        
        /// <summary>
        /// Setnulls this instance.
        /// </summary>
        internal void setnull()
        {
            m_owner = null;
            m_shadow = null;
            m_splitter = null;
           
        }

        /// <summary>
        /// Handles the Unloaded event of the SidePanel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void SidePanel_Unloaded(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
            //m_owner = null;
            SelectionChanged -= new SelectionChangedEventHandler(OnSidePanelSelectionChanged);
            if (TabChildren!=null)
                TabChildren.CollectionChanged -= new System.Collections.Specialized.NotifyCollectionChangedEventHandler(TabChildren_CollectionChanged);
            //if (m_Timer != null)
            //{
            //    m_Timer.Tick -= new EventHandler(OnTimerTick);
            //}
            //m_weakHideAnimation = false;
            //m_weakShowAnimation = false;
            if (m_leftscrollingbuttons != null)
            {
                m_leftscrollingbuttons.MouseMove -= new MouseEventHandler(m_leftscrollingbuttons_MouseMove);
                m_leftscrollingbuttons.MouseEnter -= new MouseEventHandler(m_leftscrollingbuttons_MouseEnter);
#if !SyncfusionFramework3_5
                //m_leftscrollingbuttons.TouchMove -= m_leftscrollingbuttons_TouchMove;
                //m_leftscrollingbuttons.TouchEnter -= m_leftscrollingbuttons_TouchEnter;
#endif
            }

            if (m_topscrollingbuttons != null)
            {
                m_topscrollingbuttons.MouseMove -= new MouseEventHandler(m_topscrollingbuttons_MouseMove);
                m_topscrollingbuttons.MouseEnter -= new MouseEventHandler(m_topscrollingbuttons_MouseEnter);
#if !SyncfusionFramework3_5
                //m_leftscrollingbuttons.TouchMove -= m_leftscrollingbuttons_TouchMove;
                //m_leftscrollingbuttons.TouchEnter -= m_leftscrollingbuttons_TouchEnter;
#endif
            }

            if (null != border && null != border.ContextMenu)
            {
               
                border.ContextMenu.MouseLeave -= new MouseEventHandler(ContextMenu_MouseLeave);
                border.ContextMenu.PreviewMouseLeftButtonDown -= new MouseButtonEventHandler(OnContextMenuPreviewMouseLeftButtonDown);
#if !SyncfusionFramework3_5
                //border.ContextMenu.PreviewTouchDown -= ContextMenu_PreviewTouchDown;
#endif
                ContextMenuOpening -= new ContextMenuEventHandler(OnSidePanelContextMenuOpening);
            }
            if (null != header)
            {
                header.IsContextMenuOpenChanged -= new PropertyChangedCallback(OnHeaderIsContextMenuOpenChanged);
            }
            if (null != m_splitter)
            {
                m_splitter.OffsetChanged -= new PropertyChangedCallback(OnSplitterOffsetChanged);
                m_splitter.IsPressedChanged -= new EventHandler(OnSplitterIsPressedChanged);
            }
            if (parentwindow != null)
            {
                parentwindow.Activated -= new EventHandler(parentwindow_Activated);
                parentwindow.Deactivated -= new EventHandler(parentwindow_Deactivated);
                parentwindow.StateChanged += new EventHandler(parentwindow_StateChanged);
                parentwindow.SizeChanged += new SizeChangedEventHandler(parentwindow_SizeChanged);
            }

            DockingManager = null;
        }
        #endregion

        #region Events

        /// <summary>
        /// Initializes command to execute previous command
        /// </summary>
        public static RoutedUICommand PreviousCommand = new RoutedUICommand("Previous", "Previous", typeof(SidePanel));

        /// <summary>
        /// Initializes command to execute Home page command
        /// </summary>
        public static RoutedUICommand HomePageCommand = new RoutedUICommand("HomePage", "HomePage", typeof(SidePanel));

        /// <summary>
        /// Initializes command to execute next command
        /// </summary>
        public static RoutedUICommand NextCommand = new RoutedUICommand("Next", "Next", typeof(SidePanel));

        /// <summary>
        /// Initializes command to execute next page command
        /// </summary>
        public static RoutedUICommand EndPageCommand = new RoutedUICommand("EndPage", "EndPage", typeof(SidePanel));

        /// <summary>
        /// Initializes event to show auto hidden window.
        /// </summary>
        public static readonly RoutedEvent ShowEvent;

        /// <summary>
        /// Initializes event to hide auto hidden window.
        /// </summary>
        public static readonly RoutedEvent HideEvent;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets ContentRenderTransformX of the SidePanel. This is a dependency property.
        /// </summary>
        /// <remarks>
        /// The ContentRenderTransformX property is usable only when you override the template of the <see cref="MainHost"/> class.
        /// To override the template of the <see cref="MainHost"/> you have to set <see cref="Syncfusion.Windows.Tools.Controls.DockingManager.MainHostStyleProperty"/> property of the 
        /// <see cref="DockingManager"/> and then, in this style set the overridden template reference.
        /// ContentRenderTransformX is useful only for left and right side panels.
        /// Also ContentRenderTransformX is used for Slide animation.
        /// </remarks>
        /// <example>
        /// <para/>This example shows how to use ContentRenderTransformX in XAML.
        /// <code language="XAML">
        /// <![CDATA[
        /// <Syncfusion:SidePanel Name="PART_LeftPanel" panelSide="Left" Grid.Row="1" Grid.Column="0" TabStripPlacement="Left" ContentRenderTransformX="1">
        /// <Syncfusion:SidePanel.Triggers>
        /// <EventTrigger RoutedEvent="Syncfusion:SidePanel.ShowEvent" >
        /// <EventTrigger.Actions>
        /// <BeginStoryboard>
        /// <Storyboard >
        /// <DoubleAnimation x:Name="PART_LeftShowAnimation" Storyboard.TargetProperty="ContentRenderTransformX" To="0"  BeginTime="0:0:0.2"
        /// Duration="{Binding Path=(TabControl.SelectedItem).(Syncfusion:DockingManager.AnimationDelay)
        /// , RelativeSource={RelativeSource FindAncestor, AncestorType={x:Type Syncfusion:SidePanel}}}" />
        /// </Storyboard>
        /// </BeginStoryboard>
        /// </EventTrigger.Actions>
        /// </EventTrigger>
        /// <EventTrigger RoutedEvent="Syncfusion:SidePanel.HideEvent" >
        /// <EventTrigger.Actions>
        /// <BeginStoryboard>
        /// <Storyboard>
        /// <DoubleAnimation x:Name="PART_LeftHideAnimation" Storyboard.TargetProperty="ContentRenderTransformX" To="1"
        /// Duration="{Binding Path=(TabControl.SelectedItem).(Syncfusion:DockingManager.AnimationDelay)
        /// , RelativeSource={RelativeSource FindAncestor, AncestorType={x:Type Syncfusion:SidePanel}}}" />
        /// </Storyboard>
        /// </BeginStoryboard>
        /// </EventTrigger.Actions>
        /// </EventTrigger>
        /// </Syncfusion:SidePanel.Triggers>
        /// </Syncfusion:SidePanel>
        /// ]]>
        /// </code>
        /// </example>
        /// <seealso cref="double"/>
        public double ContentRenderTransformX
        {
            get
            {
                return (double)GetValue(ContentRenderTransformXProperty);
            }

            set
            {
                SetValue(ContentRenderTransformXProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets ContentRenderTransformY of the SidePanel. This is a dependency property.
        /// </summary>
        /// <remarks>
        /// The ContentRenderTransformY property is usable only when you override the template of the <see cref="MainHost"/> class.
        /// To override the template of the <see cref="MainHost"/> you have to set <see cref="Syncfusion.Windows.Tools.Controls.DockingManager.MainHostStyleProperty"/> property of the 
        /// <see cref="DockingManager"/> and then, in this style set the overridden template reference.
        /// ContentRenderTransformY is useful only for top and bottom side panels.
        /// Also ContentRenderTransformY is used for Slide animation.
        /// </remarks>
        /// <example>
        /// <para/>This example shows how to use ContentRenderTransformY in XAML.
        /// <code language="XAML">
        /// <![CDATA[
        /// <Syncfusion:SidePanel Name="PART_TopPanel" panelSide="Top" Grid.Row="0" Grid.Column="1" TabStripPlacement="Top" ContentRenderTransformY="1">
        /// <Syncfusion:SidePanel.Triggers>
        /// <EventTrigger RoutedEvent="Syncfusion:SidePanel.ShowEvent" >
        /// <EventTrigger.Actions>
        /// <BeginStoryboard>
        /// <Storyboard>
        /// <DoubleAnimation x:Name="PART_TopShowAnimation" Storyboard.TargetProperty="ContentRenderTransformY" To="0" BeginTime="0:0:0.2"
        /// Duration="{Binding Path=(TabControl.SelectedItem).(Syncfusion:DockingManager.AnimationDelay)
        /// , RelativeSource={RelativeSource FindAncestor, AncestorType={x:Type Syncfusion:SidePanel}}}" />
        /// </Storyboard>
        /// </BeginStoryboard>
        /// </EventTrigger.Actions>
        /// </EventTrigger>
        /// <EventTrigger RoutedEvent="Syncfusion:SidePanel.HideEvent" >
        /// <EventTrigger.Actions>
        /// <BeginStoryboard>
        /// <Storyboard>
        /// <DoubleAnimation x:Name="PART_TopHideAnimation" Storyboard.TargetProperty="ContentRenderTransformY" To="1"
        /// Duration="{Binding Path=(TabControl.SelectedItem).(Syncfusion:DockingManager.AnimationDelay)
        /// , RelativeSource={RelativeSource FindAncestor, AncestorType={x:Type Syncfusion:SidePanel}}}" />
        /// </Storyboard>
        /// </BeginStoryboard>
        /// </EventTrigger.Actions>
        /// </EventTrigger>
        /// </Syncfusion:SidePanel.Triggers>
        /// </Syncfusion:SidePanel>
        /// ]]>
        /// </code>
        /// </example>
        /// <seealso cref="double"/>
        public double ContentRenderTransformY
        {
            get
            {
                return (double)GetValue(ContentRenderTransformYProperty);
            }

            set
            {
                SetValue(ContentRenderTransformYProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets ContentScaleX of the SidePanel. This is a dependency property.
        /// </summary>
        /// <remarks>
        /// The ContentScaleX property is usable only when you override the template of the <see cref="MainHost"/> class or template of the SidePanel class.
        /// To override the template of the <see cref="MainHost"/> you have to set <see cref="Syncfusion.Windows.Tools.Controls.DockingManager.MainHostStyleProperty"/> property of the 
        /// <see cref="DockingManager"/> and then, in this style set the overridden template reference.
        /// To override the template of the SidePanel you have to set <see cref="Syncfusion.Windows.Tools.Controls.DockingManager.SidePanelStyleProperty"/> property of the 
        /// <see cref="DockingManager"/> and then, in this style set the overridden template reference. 
        /// When overriding template of the SidePanel, you can use ContentScaleX property to initialize <see cref="System.Windows.Media.ScaleTransform.ScaleXProperty"/> property of the <see cref="System.Windows.Media.ScaleTransform"/>.
        /// How to initialize <see cref="System.Windows.Media.ScaleTransform.ScaleXProperty"/> property of the <see cref="System.Windows.Media.ScaleTransform"/> see the example below.
        /// ContentScaleX is useful only for left and right side panels.
        /// Also ContentScaleX is used for Scale animation.
        /// </remarks>
        /// <example>
        /// <para/>How to initialize <see cref="System.Windows.Media.ScaleTransform.ScaleXProperty"/> property of the <see cref="ScaleTransform"/> in XAML.
        /// <code language="XAML">
        /// <![CDATA[
        /// <ScaleTransform ScaleX="{Binding Path=ContentScaleX, RelativeSource={RelativeSource TemplatedParent}}"
        /// ScaleY="{Binding Path=ContentScaleY, RelativeSource={RelativeSource TemplatedParent}}"/>
        /// ]]>
        /// </code>
        /// </example>
        /// <example>
        /// <para/>Using ContentScaleX property in XAML.
        /// <code language="XAML">
        /// <![CDATA[
        /// <Syncfusion:SidePanel Name="PART_LeftPanel" panelSide="Left" Grid.Row="1" Grid.Column="0" TabStripPlacement="Left" ContentScaleX="0">
        /// <Syncfusion:SidePanel.Triggers>
        /// <EventTrigger RoutedEvent="Syncfusion:SidePanel.ShowEvent" >
        /// <EventTrigger.Actions>
        /// <BeginStoryboard>
        /// <Storyboard >
        /// <DoubleAnimation x:Name="PART_LeftShowAnimation" Storyboard.TargetProperty="ContentScaleX" To="1"
        /// Duration="{Binding Path=(TabControl.SelectedItem).(Syncfusion:DockingManager.AnimationDelay)
        /// , RelativeSource={RelativeSource FindAncestor, AncestorType={x:Type Syncfusion:SidePanel}}}" />
        /// </Storyboard>
        /// </BeginStoryboard>
        /// </EventTrigger.Actions>
        /// </EventTrigger>
        /// <EventTrigger RoutedEvent="Syncfusion:SidePanel.HideEvent" >
        /// <EventTrigger.Actions>
        /// <BeginStoryboard>
        /// <Storyboard>
        /// <DoubleAnimation x:Name="PART_LeftHideAnimation" Storyboard.TargetProperty="ContentScaleX" To="0"
        /// Duration="{Binding Path=(TabControl.SelectedItem).(Syncfusion:DockingManager.AnimationDelay)
        /// , RelativeSource={RelativeSource FindAncestor, AncestorType={x:Type Syncfusion:SidePanel}}}" />
        /// </Storyboard>
        /// </BeginStoryboard>
        /// </EventTrigger.Actions>
        /// </EventTrigger>
        /// </Syncfusion:SidePanel.Triggers>
        /// </Syncfusion:SidePanel>
        /// ]]>
        /// </code>
        /// </example>
        public double ContentScaleX
        {
            get
            {
                return (double)GetValue(ContentScaleXProperty);
            }

            set
            {
                SetValue(ContentScaleXProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets ContentScaleY of the SidePanel. This is a dependency property.
        /// </summary>
        /// <remarks>
        /// The ContentScaleY property is usable only when you override the template of the <see cref="MainHost"/> class or template of the SidePanel class.
        /// To override the template of the <see cref="MainHost"/> you have to set <see cref="Syncfusion.Windows.Tools.Controls.DockingManager.MainHostStyleProperty"/> property of the 
        /// <see cref="DockingManager"/> and then, in this style set the overridden template reference.
        /// To override the template of the SidePanel you have to set <see cref="Syncfusion.Windows.Tools.Controls.DockingManager.SidePanelStyleProperty"/> property of the 
        /// <see cref="DockingManager"/> and then, in this style set the overridden template reference. 
        /// When overriding template of the SidePanel, you can use ContentScaleY property to initialize <see cref="System.Windows.Media.ScaleTransform.ScaleYProperty"/> property of the <see cref="System.Windows.Media.ScaleTransform"/>.
        /// How to initialize <see cref="System.Windows.Media.ScaleTransform.ScaleYProperty"/> property of the <see cref="System.Windows.Media.ScaleTransform"/> see the example of the <see cref="ContentScaleX"/> property.
        /// ContentScaleY is useful only for top and bottom side panels.
        /// Also ContentScaleY is used for Scale animation.
        /// </remarks>
        /// <example>
        /// <para/>How to initialize ScaleXProperty property of the ScaleTransform in XAML please see the example of the ContentScaleX property.
        /// </example>
        /// <example>
        /// <para/>Using ContentScaleY property in XAML.
        /// <code language="XAML">
        /// <![CDATA[
        /// <Syncfusion:SidePanel Name="PART_TopPanel" panelSide="Top" Grid.Row="0" Grid.Column="1" TabStripPlacement="Top" ContentScaleY="0">
        /// <Syncfusion:SidePanel.Triggers>
        /// <EventTrigger RoutedEvent="Syncfusion:SidePanel.ShowEvent" >
        /// <EventTrigger.Actions>
        /// <BeginStoryboard>
        /// <Storyboard>
        /// <DoubleAnimation x:Name="PART_TopShowAnimation" Storyboard.TargetProperty="ContentScaleY" To="1"
        /// Duration="{Binding Path=(TabControl.SelectedItem).(Syncfusion:DockingManager.AnimationDelay)
        /// , RelativeSource={RelativeSource FindAncestor, AncestorType={x:Type Syncfusion:SidePanel}}}" />
        /// </Storyboard>
        /// </BeginStoryboard>
        /// </EventTrigger.Actions>
        /// </EventTrigger>
        /// <EventTrigger RoutedEvent="Syncfusion:SidePanel.HideEvent" >
        /// <EventTrigger.Actions>
        /// <BeginStoryboard>
        /// <Storyboard>
        /// <DoubleAnimation x:Name="PART_TopHideAnimation" Storyboard.TargetProperty="ContentScaleY" To="0"
        /// Duration="{Binding Path=(TabControl.SelectedItem).(Syncfusion:DockingManager.AnimationDelay)
        /// , RelativeSource={RelativeSource FindAncestor, AncestorType={x:Type Syncfusion:SidePanel}}}" />
        /// </Storyboard>
        /// </BeginStoryboard>
        /// </EventTrigger.Actions>
        /// </EventTrigger>
        /// </Syncfusion:SidePanel.Triggers>
        /// </Syncfusion:SidePanel>
        /// ]]>
        /// </code>
        /// </example>
        public double ContentScaleY
        {
            get
            {
                return (double)GetValue(ContentScaleYProperty);
            }

            set
            {
                SetValue(ContentScaleYProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets ContentOpacity of the SidePanel. This is a dependency property.
        /// </summary>
        /// <remarks>
        /// The ContentOpacity property is usable only when you override the template of the <see cref="MainHost"/> class or template of the SidePanel class.
        /// To override the template of the <see cref="MainHost"/> you have to set <see cref="Syncfusion.Windows.Tools.Controls.DockingManager.MainHostStyleProperty"/> property of the 
        /// <see cref="DockingManager"/> and then, in this style set the overridden template reference.
        /// To override the template of the SidePanel you have to set <see cref="Syncfusion.Windows.Tools.Controls.DockingManager.SidePanelStyleProperty"/> property of the 
        /// <see cref="DockingManager"/> and then, in this style set the overridden template reference. 
        /// Also you can set binding on ContentOpacity property to initialize <see cref="System.Windows.UIElement.OpacityProperty"/> of the <see cref="OpacityDockPanel"/> when overriding SidePanel template. 
        /// The ContentOpacity property is used for Fade animation.
        /// </remarks>
        /// <example>
        /// <para/>Using ContentOpacity property in XAML.
        /// <code language="XAML">
        /// <![CDATA[
        /// <Syncfusion:SidePanel Name="PART_TopPanel" panelSide="Top" Grid.Row="0" Grid.Column="1" TabStripPlacement="Top" ContentOpacity="0">
        /// <Syncfusion:SidePanel.Triggers>
        /// <EventTrigger RoutedEvent="Syncfusion:SidePanel.ShowEvent" >
        /// <EventTrigger.Actions>
        /// <BeginStoryboard>
        /// <Storyboard>
        /// <DoubleAnimation x:Name="PART_TopShowAnimation" Storyboard.TargetProperty="ContentOpacity" To="1"
        /// Duration="{Binding Path=(TabControl.SelectedItem).(Syncfusion:DockingManager.AnimationDelay)
        /// , RelativeSource={RelativeSource FindAncestor, AncestorType={x:Type Syncfusion:SidePanel}}}" />
        /// </Storyboard>
        /// </BeginStoryboard>
        /// </EventTrigger.Actions>
        /// </EventTrigger>
        /// <EventTrigger RoutedEvent="Syncfusion:SidePanel.HideEvent" >
        /// <EventTrigger.Actions>
        /// <BeginStoryboard>
        /// <Storyboard>
        /// <DoubleAnimation x:Name="PART_TopHideAnimation" Storyboard.TargetProperty="ContentOpacity" To="0"
        /// Duration="{Binding Path=(TabControl.SelectedItem).(Syncfusion:DockingManager.AnimationDelay)
        /// , RelativeSource={RelativeSource FindAncestor, AncestorType={x:Type Syncfusion:SidePanel}}}" />
        /// </Storyboard>
        /// </BeginStoryboard>
        /// </EventTrigger.Actions>
        /// </EventTrigger>
        /// </Syncfusion:SidePanel.Triggers>
        /// </Syncfusion:SidePanel>
        /// ]]>
        /// </code>
        /// </example>
        public double ContentOpacity
        {
            get
            {
                return (double)GetValue(ContentOpacityProperty);
            }

            set
            {
                SetValue(ContentOpacityProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets TabChildren of the SidePanel. This is a dependency property.
        /// </summary>
        /// <remarks>
        /// The TabChildren property contains all tabbed children of the SidePanel.
        /// You can use TabChildren property to get tabbed children for some ItemSourceProperty.
        /// For example, you can set ItemSourceProperty in the Context Menu when overriding SidePanel template.
        /// </remarks>
        /// <example>
        /// <para/>Using TabChildren property in XAML.
        /// <code language="XAML">
        /// <![CDATA[
        /// <ContextMenu ItemsSource="{TemplateBinding TabChildren}"/>
        /// ]]>
        /// </code>
        /// </example>
        public ObservableFrameworkElements TabChildren
        {
            get
            {
                return (ObservableFrameworkElements)GetValue(TabChildrenProperty);
            }

            set
            {
                SetValue(TabChildrenProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets panelSide of the SidePanel. This is a dependency property.
        /// </summary>
        /// <remarks>
        /// The panelSide property is usable when you want to set the side of the SidePanel. 
        /// One of the next values can be set to the panelSide property: Dock.Top, Dock.Bottom, Dock.Left or Dock.Right.
        /// </remarks>
        /// <example>
        /// <para/>To use PanelSide property in XAML please see examples of the next properties: 
        /// ContentOpacity,ContentScaleY,ContentScaleX,ContentRenderTransformY,ContentRenderTransformX         
        /// </example>
        public Dock PanelSide
        {
            get
            {
                return (Dock)GetValue(PanelSideProperty);
            }

            set
            {
                SetValue(PanelSideProperty, value);
            }
        }

        /// <summary>
        /// Gets a value indicating whether or internally sets IsContentHiden of the SidePanel. This is a dependency property.
        /// </summary>
        /// <remarks>
        /// This property returns true if all windows that are docked to SidePanel are auto hidden otherwise, false.
        /// Default value of this property is true.
        /// This property can be useful when you override the template of the SidePanel. You can use IsContentHiden property
        /// for DoubleAnimation in the SidePanel.
        /// </remarks>
        public bool IsContentHiden
        {
            get
            {
                return (bool)GetValue(IsContentHidenProperty);
            }

            internal set
            {
                SetValue(IsContentHidenProperty, value);
            }
        }

        /// <summary>
        /// Gets a value indicating whether or internally sets IsShowedFocusedItem of the SidePanel. This is a dependency property.
        /// </summary>
        /// <remarks>
        /// This property returns true if focused window of the SidePanel is visible; otherwise, false.
        /// Default value of this property is false.
        /// This property can be useful when you override the template of the SidePanel. You can use IsShowedFocusedItem property
        /// for the DockHeaderPresenter implementation in the SidePanel.
        /// </remarks>
        public bool IsShowedFocusedItem
        {
            get
            {
                return (bool)GetValue(IsShowedFocusedItemProperty);
            }

            internal set
            {
                SetValue(IsShowedFocusedItemProperty, value);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is open header context menu.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is open header context menu; otherwise, <c>false</c>.
        /// </value>
        private bool IsOpenHeaderContextMenu
        {
            get
            {
                return (bool)GetValue(IsOpenHeaderContextMenuProperty);
            }
        }

        /// <summary>
        /// Gets or sets the auto hide animation mode.
        /// </summary>
        /// <value>The auto hide animation mode.</value>
        private AutoHideAnimationMode AutoHideAnimationMode
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
        /// Gets the value of the SidePanel.TabChildOrder�attached property from a given DependencyObject.
        /// </summary>
        /// <param name="obj">The element from which to read the property value.</param>
        /// <returns>The value of the SidePanel.TabChildOrder�attached property.</returns>
        public static int GetTabChildOrder(DependencyObject obj)
        {
            return (int)obj.GetValue(TabChildOrderProperty);
        }

        /// <summary>
        /// Sets the value of the SidePanel.TabChildOrder�attached property of a given DependencyObject.
        /// </summary>
        /// <param name="obj">The element that contains this property.</param>
        /// <param name="value">New value to set.</param>
        public static void SetTabChildOrder(DependencyObject obj, int value)
        {
            obj.SetValue(TabChildOrderProperty, value);
        }

        /// <summary>
        /// Gets the value of the SidePanel.TabGroupName�attached property from a given DependencyObject.
        /// </summary>
        /// <param name="obj">The element from which to read the property value.</param>
        /// <returns>The value of the SidePanel.TabGroupName�attached property.</returns>
        public static string GetTabGroupName(DependencyObject obj)
        {
            return (string)obj.GetValue(TabGroupNameProperty);
        }

        /// <summary>
        /// Sets the value of the SidePanel.TabGroupName�attached property of a given DependencyObject.
        /// </summary>
        /// <param name="obj">The element that contains this property.</param>
        /// <param name="value">New value to set.</param>
        public static void SetTabGroupName(DependencyObject obj, string value)
        {
            obj.SetValue(TabGroupNameProperty, value);
        }

        /// <summary>
        /// Gets the value of the SidePanel.IsTabGroupOwner�attached property from a given DependencyObject.
        /// </summary>
        /// <param name="obj">The element from which to read the property value.</param>
        /// <returns>The value of the SidePanel.IsTabGroupOwner�attached property.</returns>
        public static bool GetIsTabGroupOwner(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsTabGroupOwnerProperty);
        }

        /// <summary>
        /// Sets the value of the SidePanel.IsTabGroupOwner�attached property of a given DependencyObject.
        /// </summary>
        /// <param name="obj">The element that contains this property.</param>
        /// <param name="value">New value to set.</param>
        public static void SetIsTabGroupOwner(DependencyObject obj, bool value)
        {
            obj.SetValue(IsTabGroupOwnerProperty, value);
        }

        /// <summary>
        /// Gets a value indicating whether this instance is splitter pressed.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is splitter pressed; otherwise, <c>false</c>.
        /// </value>
        private bool IsSplitterPressed
        {
            get
            {
                if (null != m_splitter)
                {
                    return m_splitter.IsPressed;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Sets the focused item.
        /// </summary>
        /// <value>The focused item.</value>
        private object FocusedItem
        {
            set
            {
                m_focusedItem = value;
                SetIsShowedFocusedItem();
            }
        }

        /// <summary>
        /// Gets the owner.
        /// </summary>
        /// <value>The owner.</value>
        private DockingManager Owner
        {
            get
            {
                if (null == m_owner)
                {
                    ApplyTemplate();
                }

                return m_owner;
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

        #endregion

        #region Public methods
        /// <summary>
        /// Builds the current template's visual tree.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            border = GetTemplateChild(BorderName) as Border;

            m_leftscrollingbuttons = GetTemplateChild("PART_ScrollButtons") as ScrollButtonsBar;
            m_topscrollingbuttons = GetTemplateChild("PART_ScrollButtons") as ScrollButtonsBar;

            if (m_leftscrollingbuttons != null)
            {
                m_leftscrollingbuttons.MouseMove += new MouseEventHandler(m_leftscrollingbuttons_MouseMove);
                m_leftscrollingbuttons.MouseEnter += new MouseEventHandler(m_leftscrollingbuttons_MouseEnter);
#if !SyncfusionFramework3_5
                //m_leftscrollingbuttons.TouchMove += m_leftscrollingbuttons_TouchMove;
                //m_leftscrollingbuttons.TouchEnter += m_leftscrollingbuttons_TouchEnter;
#endif
            }

            if (m_topscrollingbuttons != null)
            {
                m_topscrollingbuttons.MouseMove += new MouseEventHandler(m_topscrollingbuttons_MouseMove);
                m_topscrollingbuttons.MouseEnter += new MouseEventHandler(m_topscrollingbuttons_MouseEnter);
#if !SyncfusionFramework3_5
                //m_topscrollingbuttons.TouchEnter += m_topscrollingbuttons_TouchEnter;
                //m_topscrollingbuttons.TouchMove += m_topscrollingbuttons_TouchMove;
#endif
            }

            if (null != border && null != border.ContextMenu)
            {
                m_contextMenu = border.ContextMenu;
                border.ContextMenu.MouseLeave += new MouseEventHandler(ContextMenu_MouseLeave);
#if !SyncfusionFramework3_5
                //border.ContextMenu.TouchLeave += ContextMenu_TouchLeave;
                //border.ContextMenu.PreviewTouchDown += ContextMenu_PreviewTouchDown;
#endif
                border.ContextMenu.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(OnContextMenuPreviewMouseLeftButtonDown);
                ContextMenuOpening += new ContextMenuEventHandler(OnSidePanelContextMenuOpening);
            }

            header = GetTemplateChild(HeaderName) as DockHeaderPresenter;

            if (null != header)
            {
                BindingUtils.SetBinding(this, header, SidePanel.IsOpenHeaderContextMenuProperty, DockHeaderPresenter.IsContextMenuOpenProperty);
                header.IsContextMenuOpenChanged += new PropertyChangedCallback(OnHeaderIsContextMenuOpenChanged);
            }

            m_splitter = GetTemplateChild(SideSplitterName) as Splitter;

            if (null != m_splitter)
            {
                m_splitter.OffsetChanged += new PropertyChangedCallback(OnSplitterOffsetChanged);
                m_splitter.IsPressedChanged += new EventHandler(OnSplitterIsPressedChanged);
            }

            AddHandlerForAnimation();

            m_shadow = GetTemplateChild(ShadowName) as Border;
            m_popuppanel = GetTemplateChild(m_popup) as PopupSidePanel;

            //Window window = VisualUtils.FindAncestor(this, typeof(Window)) as Window;
            //if (window != null)
            //{
            //    window.Closed += new EventHandler(window_Closed);
            //}

            parentwindow = VisualUtils.FindAncestor(this, typeof(Window)) as Window;
            if (parentwindow != null)
            {
                parentwindow.Activated += new EventHandler(parentwindow_Activated);
                parentwindow.Deactivated += new EventHandler(parentwindow_Deactivated);
                parentwindow.StateChanged += new EventHandler(parentwindow_StateChanged);
                parentwindow.SizeChanged += new SizeChangedEventHandler(parentwindow_SizeChanged);
                parentwindow.LocationChanged += new EventHandler(parentwindow_LocationChanged);
            }
        }

        void parentwindow_LocationChanged(object sender, EventArgs e)
        {
            if (m_popuppanel != null && m_popuppanel.IsOpen)
            {
                m_popuppanel.IsOpen = false;
                m_isShow = false;
            }
            ValidateCommands();
        }

        void parentwindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (m_popuppanel != null && m_popuppanel.IsOpen)
            {
                m_popuppanel.IsOpen = false;
                m_isShow = false;
            }
            ValidateCommands();
        }

        void parentwindow_StateChanged(object sender, EventArgs e)
        {
            if (m_popuppanel != null && m_popuppanel.IsOpen)
            {
                m_popuppanel.IsOpen = false;
                m_isShow = false;
            }
            ValidateCommands();
        }
       

        void parentwindow_Deactivated(object sender, EventArgs e)
        {
            if (m_popuppanel != null && m_popuppanel.IsOpen)
            {
                b_windowactivation = true;
                m_popuppanel.IsOpen = false;
                m_isShow = false;
            }
            ValidateCommands();
        }

        void parentwindow_Activated(object sender, EventArgs e)
        {
            if (m_popuppanel != null && b_windowactivation)
            {
                m_popuppanel.IsOpen = true;
                b_windowactivation = false;
            }
            ValidateCommands();
        }

        /// <summary>
        /// Handles the MouseEnter event of the m_topscrollingbuttons control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        private void m_topscrollingbuttons_MouseEnter(object sender, MouseEventArgs e)
        {
            if (e.StylusDevice == null || e.StylusDevice != null)
            {
                base.OnMouseEnter(e);
                m_restrictanimation = true;
            }
        }

        /// <summary>
        /// Handles the MouseEnter event of the m_leftscrollingbuttons control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        private void m_leftscrollingbuttons_MouseEnter(object sender, MouseEventArgs e)
        {
            if (e.StylusDevice == null || e.StylusDevice != null)
            {
                base.OnMouseEnter(e);
                m_restrictanimation = true;
            }
        }

        /// <summary>
        /// Handles the MouseMove event of the m_topscrollingbuttons control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        private void m_topscrollingbuttons_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.StylusDevice == null || e.StylusDevice != null)
            {
                base.OnMouseMove(e);
                m_restrictanimation = true;
            }
        }

        /// <summary>
        /// Handles the MouseMove event of the m_leftscrollingbuttons control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        private void m_leftscrollingbuttons_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.StylusDevice == null || e.StylusDevice != null)
            {
                base.OnMouseMove(e);
                m_restrictanimation = true;
            }
        }

        /// <summary>
        /// Handles the Closed event of the window control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void window_Closed(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            Window window = VisualUtils.FindAncestor(this, typeof(Window)) as Window;
            if (window != null)
            {
                window.Closed -= new EventHandler(window_Closed);
            }
            LocalValueEnumerator locallySetProperties = this.GetLocalValueEnumerator();
            while (locallySetProperties.MoveNext())
            {
                DependencyProperty propertyToClear = locallySetProperties.Current.Property;
                if (!propertyToClear.ReadOnly) { this.ClearValue(propertyToClear); }
            }
        }

        /// <summary>
        /// Selects the tab that contains the element.
        /// </summary>
        /// <param name="element">Element to select the tab.</param>
        public void SelectTab(FrameworkElement element)
        {
            m_weakShowAnimation = false;
            m_isHideTabWhenMenuOpen = true;

            if (Items.Contains(element))
            {
                if (element != null && !object.Equals(InternalDataContext, element))
                {
                    ShowNewContent(element, true);
                }
                else if (!m_isShow)
                {
                    FireShow();
                }

                FocusedItem = element;
            }
            ActiveWindowChangingEventArgs args = new ActiveWindowChangingEventArgs();
            args.NewValue = element;
            DockingManager docking = DockingManager.ResolveManager(element);
            args.OldValue = docking.ActiveWindow;
            docking.FireActiveWindowChanging(args.NewValue, args);
            if (!args.Cancel)
            {
                DockingManager.SetNewFocusedElement(element);
            }
            Focus();
        }

        /// <summary>
        /// Autos' the hide tab.
        /// </summary>
        /// <param name="element">The element.</param>
        public void AutoHideTab(FrameworkElement element)
        {
            if (element == (InternalDataContext as FrameworkElement))
            {
                HideContent();
            }
        }

        private void ValidateCommands()
        {
            if (InternalDataContext != null
                && InternalDataContext.Parent != null && InternalDataContext.Parent is DockingManager
                && (InternalDataContext.Parent as DockingManager).EnableScrollableSidePanel)
            {
                CommandManager.InvalidateRequerySuggested();
            }
        }
        #endregion

        #region Implementation

        private void CanExecuteNext(object sender,CanExecuteRoutedEventArgs args)
        {
            ScrollViewer scrollviewer = GetTemplateChild("PART_ScrollPanel") as ScrollViewer;
            if (scrollviewer != null && InternalDataContext != null
                && InternalDataContext.Parent != null && InternalDataContext.Parent is DockingManager
                && (InternalDataContext.Parent as DockingManager).ScrollButtonMode == ScrollingButtonMode.Extended)
            {
                if (PanelSide == Dock.Top || PanelSide == Dock.Bottom)
                {
                    args.CanExecute = scrollviewer.HorizontalOffset != scrollviewer.ScrollableWidth;
                }
                else
                {
                    args.CanExecute = scrollviewer.VerticalOffset != scrollviewer.ScrollableHeight;
                }
            }
            else
                args.CanExecute = true;
        }

        /// <summary>
        /// Executes the next.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private void ExecuteNext(object sender, ExecutedRoutedEventArgs e)
        {
            DirectTabPanel panel = GetTemplateChild("PART_PanelName") as DirectTabPanel;
            Border borderpanel = GetTemplateChild("PART_BorderName") as Border;
            ScrollViewer scrollviewer = GetTemplateChild("PART_ScrollPanel") as ScrollViewer;
            if (panel != null && scrollviewer!=null)
            {
                double actualheight = scrollviewer.ViewportHeight + scrollviewer.VerticalOffset ;
                double actualwidth = scrollviewer.ViewportWidth + scrollviewer.HorizontalOffset ;
                double sumHeight=0,sumwidth=0;
                for (int i = 0; i < panel.Children.Count; i++)
                {
                    FrameworkElement element=panel.Children[i] as FrameworkElement;
                    if (Visibility.Collapsed != element.Visibility)
                    {
                        element.Measure(new Size(double.PositiveInfinity,double.PositiveInfinity));
                        Size desiredSizeWithoutMargin = DirectTabPanel.GetDesiredSizeWithoutMargin(element);

                        if (m_leftscrollingbuttons != null && (PanelSide==Dock.Left || PanelSide==Dock.Right))
                        {
                            if ((sumHeight + desiredSizeWithoutMargin.Height) > actualheight)
                            {
                                if ((sumHeight + desiredSizeWithoutMargin.Height) > scrollviewer.ViewportHeight)
                                {
                                    if (sumHeight + desiredSizeWithoutMargin.Height == scrollviewer.ExtentHeight && borderpanel != null)
                                    {
                                        Canvas.SetTop(borderpanel, 0);
                                        scrollviewer.ScrollToVerticalOffset(scrollviewer.VerticalOffset - desiredSizeWithoutMargin.Height);
                                    }

                                    if ((sumHeight + desiredSizeWithoutMargin.Height - actualheight) > 1)
                                    {
                                        scrollviewer.ScrollToVerticalOffset((sumHeight + desiredSizeWithoutMargin.Height - actualheight) + scrollviewer.VerticalOffset);
                                    }
                                    else
                                    {
                                        if (desiredSizeWithoutMargin.Height - scrollviewer.VerticalOffset > 0)
                                        {
                                            scrollviewer.ScrollToVerticalOffset(desiredSizeWithoutMargin.Height + scrollviewer.VerticalOffset);
                                        }
                                        else
                                        {
                                            if (borderpanel != null)
                                            {
                                                Canvas.SetTop(borderpanel, 0);
                                            }
                                            scrollviewer.ScrollToEnd();
                                        }
                                    }
                                }
                                else
                                {
                                    if (borderpanel != null)
                                    {
                                        Canvas.SetTop(borderpanel, 0);
                                    }
                                    scrollviewer.ScrollToEnd();
                                }
                                break;
                            }
                            else if (actualheight == scrollviewer.ExtentHeight)
                            {
                                if (borderpanel != null)
                                {
                                    Canvas.SetTop(borderpanel, 0);
                                }
                                scrollviewer.ScrollToEnd();
                                break;
                            }
                        }

                        if (m_topscrollingbuttons != null && (PanelSide==Dock.Bottom || PanelSide==Dock.Top))
                        {
                            if ((sumwidth + desiredSizeWithoutMargin.Width) > actualwidth)
                            {
                                if ((sumwidth + desiredSizeWithoutMargin.Width) > scrollviewer.ViewportWidth)
                                {
                                    if (sumwidth + desiredSizeWithoutMargin.Width == scrollviewer.ExtentWidth && borderpanel != null)
                                    {
                                        Canvas.SetLeft(borderpanel, 0);
                                        scrollviewer.ScrollToHorizontalOffset(scrollviewer.HorizontalOffset - desiredSizeWithoutMargin.Width);                                        
                                    }
                                    
                                    if ((sumwidth + desiredSizeWithoutMargin.Width - actualwidth) > 1)
                                    {
                                        scrollviewer.ScrollToHorizontalOffset((sumwidth + desiredSizeWithoutMargin.Width - actualwidth) + scrollviewer.HorizontalOffset);
                                    }
                                    else
                                    {
                                        if (desiredSizeWithoutMargin.Width - scrollviewer.HorizontalOffset > 0)
                                        {
                                            scrollviewer.ScrollToHorizontalOffset(desiredSizeWithoutMargin.Width + scrollviewer.HorizontalOffset);                                            
                                        }
                                        else
                                        {
                                            if (borderpanel != null)
                                            {
                                                Canvas.SetLeft(borderpanel, 0);
                                            }
                                            scrollviewer.ScrollToRightEnd();
                                        }
                                    }
                                }
                                else
                                {
                                    if (borderpanel != null)
                                    {
                                        Canvas.SetLeft(borderpanel, 0);
                                    }
                                    scrollviewer.ScrollToRightEnd();
                                }
                                break;
                            }
                            else if (actualwidth == scrollviewer.ExtentWidth)
                            {
                                if (borderpanel != null)
                                {
                                    Canvas.SetLeft(borderpanel, 0);
                                }
                                scrollviewer.ScrollToRightEnd();
                                break;
                            }
                        }

                        sumHeight += desiredSizeWithoutMargin.Height;
                        sumwidth += desiredSizeWithoutMargin.Width;
                    }
                }
            }
        }

        private void CanExecuteEndPage(object sender,CanExecuteRoutedEventArgs args)
        {
            ScrollViewer scrollviewer = GetTemplateChild("PART_ScrollPanel") as ScrollViewer;
            if (scrollviewer != null && InternalDataContext!=null 
                && InternalDataContext.Parent != null && InternalDataContext.Parent is DockingManager
                && (InternalDataContext.Parent as DockingManager).ScrollButtonMode == ScrollingButtonMode.Extended)
            {
                if (PanelSide == Dock.Top || PanelSide == Dock.Bottom)
                {
                    args.CanExecute = scrollviewer.HorizontalOffset != scrollviewer.ScrollableWidth;
                }
                else
                {
                    args.CanExecute = scrollviewer.VerticalOffset != scrollviewer.ScrollableHeight;
                }
            }
            else
                args.CanExecute = true;
        }

        /// <summary>
        /// Executes the next page.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private void ExecuteEndPage(object sender, ExecutedRoutedEventArgs e)
        {
            ScrollViewer scrollviewer = GetTemplateChild("PART_ScrollPanel") as ScrollViewer;
            Border borderpanel = GetTemplateChild("PART_BorderName") as Border;
            if (scrollviewer != null)
            {
                if (m_leftscrollingbuttons != null && (PanelSide==Dock.Left || PanelSide==Dock.Right))
                {
                    if (borderpanel != null)
                    {
                        Canvas.SetTop(borderpanel, 0);
                    }
                    scrollviewer.ScrollToEnd();
                }

                if (m_topscrollingbuttons != null && (PanelSide==Dock.Top || PanelSide==Dock.Bottom))
                {
                    if (borderpanel != null)
                    {
                        Canvas.SetLeft(borderpanel, 0);
                    }
                    scrollviewer.ScrollToRightEnd();
                }
            }
        }

        private void CanExecutePrevious(object sender,CanExecuteRoutedEventArgs args)
        {
            ScrollViewer scrollviewer = GetTemplateChild("PART_ScrollPanel") as ScrollViewer;
            if (scrollviewer != null && InternalDataContext != null
                && InternalDataContext.Parent != null && InternalDataContext.Parent is DockingManager
                && (InternalDataContext.Parent as DockingManager).ScrollButtonMode == ScrollingButtonMode.Extended)
            {
                if (PanelSide == Dock.Top || PanelSide == Dock.Bottom)
                {
                    args.CanExecute = scrollviewer.HorizontalOffset != 0.0;
                }
                else
                {
                    args.CanExecute = scrollviewer.VerticalOffset != 0.0;
                }
            }
            else
                args.CanExecute = true;
        }

        /// <summary>
        /// Executes the previous.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private void ExecutePrevious(object sender, ExecutedRoutedEventArgs e)
        {
            DirectTabPanel panel = GetTemplateChild("PART_PanelName") as DirectTabPanel;
            Border borderpanel = GetTemplateChild("PART_BorderName") as Border;
            ScrollViewer scrollviewer = GetTemplateChild("PART_ScrollPanel") as ScrollViewer;
            if (panel != null && scrollviewer != null)
            {
                double actualheight = scrollviewer.ViewportHeight + scrollviewer.VerticalOffset;
                double actualwidth = scrollviewer.ViewportWidth + scrollviewer.HorizontalOffset;
                double sumHeight = 0, sumwidth = 0;
                for (int i = 0; i < panel.Children.Count; i++)
                {
                    FrameworkElement element = panel.Children[i] as FrameworkElement;
                    if (Visibility.Collapsed != element.Visibility)
                    {
                        element.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                        Size desiredSizeWithoutMargin = DirectTabPanel.GetDesiredSizeWithoutMargin(element);

                        if (m_leftscrollingbuttons != null && (PanelSide==Dock.Left || PanelSide==Dock.Right))
                        {
                            if (sumHeight + desiredSizeWithoutMargin.Height >= actualheight)
                            {
                                if (scrollviewer.VerticalOffset - (sumHeight + desiredSizeWithoutMargin.Height - actualheight) < 0 &&
                                        Canvas.GetTop(borderpanel) != m_leftscrollingbuttons.ActualWidth)
                                {
                                    Canvas.SetTop(borderpanel, m_leftscrollingbuttons.ActualWidth);
                                }
                                if ((sumHeight + desiredSizeWithoutMargin.Height) > scrollviewer.ViewportHeight)
                                {
                                    if ((sumHeight + desiredSizeWithoutMargin.Height - actualheight) > 1)
                                    {
                                        scrollviewer.ScrollToVerticalOffset(scrollviewer.VerticalOffset - (sumHeight + desiredSizeWithoutMargin.Height - actualheight));
                                    }
                                    else
                                    {
                                        if (scrollviewer.VerticalOffset - desiredSizeWithoutMargin.Height > 0)
                                        {
                                            scrollviewer.ScrollToVerticalOffset(scrollviewer.VerticalOffset - desiredSizeWithoutMargin.Height);
                                        }
                                        else
                                        {
                                            if (borderpanel != null)
                                            {
                                                Canvas.SetTop(borderpanel, m_leftscrollingbuttons.ActualWidth);
                                            }
                                            scrollviewer.ScrollToTop();
                                        }
                                    }
                                }
                                else
                                {
                                    if (borderpanel != null)
                                    {
                                        Canvas.SetTop(borderpanel, m_leftscrollingbuttons.ActualWidth);
                                    }
                                    scrollviewer.ScrollToTop();
                                }
                                break;
                            }
                        }

                        if (m_topscrollingbuttons != null && (PanelSide==Dock.Top || PanelSide==Dock.Bottom))
                        {
                            if (sumwidth + desiredSizeWithoutMargin.Width >= actualwidth)
                            {
                                if ((sumwidth + desiredSizeWithoutMargin.Width) > scrollviewer.ViewportWidth)
                                {
                                    if (scrollviewer.HorizontalOffset - (sumwidth + desiredSizeWithoutMargin.Width - actualwidth) < 0 &&
                                        Canvas.GetLeft(borderpanel) != m_topscrollingbuttons.ActualWidth)
                                    {
                                        Canvas.SetLeft(borderpanel, m_topscrollingbuttons.ActualWidth);
                                    }
                                    if ((sumwidth + desiredSizeWithoutMargin.Width - actualwidth) > 1)
                                    {
                                        scrollviewer.ScrollToHorizontalOffset(scrollviewer.HorizontalOffset - (sumwidth + desiredSizeWithoutMargin.Width - actualwidth));
                                    }
                                    else
                                    {
                                        if (scrollviewer.HorizontalOffset - desiredSizeWithoutMargin.Width > 0)
                                        {
                                            scrollviewer.ScrollToHorizontalOffset(scrollviewer.HorizontalOffset - desiredSizeWithoutMargin.Width);
                                        }
                                        else
                                        {
                                            if (borderpanel != null)
                                            {
                                                Canvas.SetLeft(borderpanel, m_topscrollingbuttons.ActualWidth);
                                            }
                                            scrollviewer.ScrollToLeftEnd();
                                        }
                                    }
                                }
                                else
                                {
                                    if (borderpanel != null)
                                    {
                                        Canvas.SetLeft(borderpanel, m_topscrollingbuttons.ActualWidth);
                                    }
                                    scrollviewer.ScrollToLeftEnd();
                                }
                                break;
                            }
                        }

                        sumHeight += desiredSizeWithoutMargin.Height;
                        sumwidth += desiredSizeWithoutMargin.Width;
                    }
                }
            }
        }

        private void CanExecuteHomePage(object sender,CanExecuteRoutedEventArgs args)
        {
            ScrollViewer scrollviewer = GetTemplateChild("PART_ScrollPanel") as ScrollViewer;
            if (scrollviewer != null && InternalDataContext!=null
                && InternalDataContext.Parent != null && InternalDataContext.Parent is DockingManager
                && (InternalDataContext.Parent as DockingManager).ScrollButtonMode == ScrollingButtonMode.Extended)
            {
                if (PanelSide == Dock.Top || PanelSide == Dock.Bottom)
                {
                    args.CanExecute = scrollviewer.HorizontalOffset != 0.0;
                }
                else
                {
                    args.CanExecute = scrollviewer.VerticalOffset != 0.0;
                }
            }
            else
                args.CanExecute = true;
        }

        /// <summary>
        /// Executes the Home page.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private void ExecuteHomePage(object sender, ExecutedRoutedEventArgs e)
        {
            ScrollViewer scrollviewer = GetTemplateChild("PART_ScrollPanel") as ScrollViewer;
            Border borderpanel = GetTemplateChild("PART_BorderName") as Border;
            if (scrollviewer != null)
            {
                if (m_leftscrollingbuttons != null && (PanelSide==Dock.Left || PanelSide==Dock.Right))
                {
                    if (borderpanel != null)
                    {
                        Canvas.SetTop(borderpanel, m_leftscrollingbuttons.ActualWidth);
                    }
                    scrollviewer.ScrollToTop();
                }

                if (m_topscrollingbuttons != null && (PanelSide==Dock.Top || PanelSide==Dock.Bottom))
                {
                    if (borderpanel != null)
                    {
                        Canvas.SetLeft(borderpanel, m_topscrollingbuttons.ActualWidth);
                    }
                    scrollviewer.ScrollToLeftEnd();
                }
            }
        }

        /// <summary>
        /// Adds new element to the side panel.
        /// </summary>
        /// <param name="element">The element to add.</param>
        /// <param name="update">if set to <c>true</c> [update].</param>
        internal void AddElement(FrameworkElement element, bool update)
        {
            AddElementInternal(element, update);
            TabChildren.Add(element);

            if (update)
            {
                UpdateTabGroupOwner(element);
            }

            HideNewContent(element);
        }

        /// <summary>
        /// Removes the element.
        /// </summary>
        /// <param name="element">The element.</param>
        internal void RemoveElement(FrameworkElement element)
        {
            CollapseShadow();
            CollapseContent();
            TabChildren.Remove(element);
            RemoveElementInternal(element);
            UpdateTabGroupOwner(element);
            if (Owner.UsePopupAutoHidePreview && m_popuppanel != null && m_popuppanel.IsOpen)
                m_popuppanel.IsOpen = false;
        }

        /// <summary>
        /// Gets the group.
        /// </summary>
        /// <param name="groupID">The group ID.</param>
        /// <returns>return list framework element.</returns>
        internal List<FrameworkElement> GetGroup(int groupID)
        {
            List<FrameworkElement> group = new List<FrameworkElement>();

            foreach (FrameworkElement item in TabChildren)
            {
                if (DockingManager.GetTabbedHost(item, DockState.Dock).GetHashCode() == groupID)
                {
                    group.Add(item);
                }
            }

            return group;
        }

        /// <summary>
        /// Raises the System.Windows.FrameworkElement.Initialized event. This method 
        /// is invoked whenever DockingManager.IsInitialized is set to true internally.
        /// </summary>
        /// <param name="e">The System.Windows.RoutedEventArgs that contains the event data.</param>
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            BindingUtils.SetRelativeBinding(this, FrameworkElement.StyleProperty, typeof(DockingManager), DockingManager.SidePanelStyleProperty, BindingMode.OneWay, 1);

            CommandBinding bindingprevious = new CommandBinding(PreviousCommand, ExecutePrevious,CanExecutePrevious);
            CommandBindings.Add(bindingprevious);

            CommandBinding bindingHomePage = new CommandBinding(HomePageCommand,ExecuteHomePage,CanExecuteHomePage);
            CommandBindings.Add(bindingHomePage);

            CommandBinding bindingnext = new CommandBinding(NextCommand, ExecuteNext,CanExecuteNext);
            CommandBindings.Add(bindingnext);

            CommandBinding bindingEndPage = new CommandBinding(EndPageCommand, ExecuteEndPage,CanExecuteEndPage);
            CommandBindings.Add(bindingEndPage);
        }

        protected override void OnDragEnter(DragEventArgs e)
        {
            base.OnDragEnter(e);
            m_inDragMode = true;
            if (this.Owner is DockingManager)
            {
                this.AllowDrop = true;
                if (m_owner != null && m_owner.IsAnimationEnabledOnMouseOver)
                {
                    FrameworkElement originalSource = e.OriginalSource as FrameworkElement;

                    FrameworkElement element = null;
                    bool m_animationenabled = true;
                    if ((e.Source as SidePanel) != null && (e.Source as SidePanel).InternalDataContext != null && originalSource != null)
                    {
                        FrameworkElement actualelement = null;
                        element = (FrameworkElement)originalSource.TemplatedParent;
                        if (element != null && (element as TabItem) != null)
                        {
                            actualelement = element;
                        }
                        else if (element != null)
                        {
                            actualelement = (FrameworkElement)element.TemplatedParent;
                        }
                        if (actualelement != null && (actualelement as TabItem) != null && (actualelement as TabItem).Content is DependencyObject
                            && DockingManager.GetDockWindowState((actualelement as TabItem).Content as DependencyObject) == WindowState.Minimized && !m_owner.m_exceutingminimizeflag)
                        {
                            m_animationenabled = false;
                        }
                    }

                    if (null != originalSource && !m_restrictanimation && IsNotHeaderButton(originalSource, AwlButtonName) && IsNotHeaderButton(originalSource, CloseButtonName)
                        && m_animationenabled)
                    {
                        SwitchHiddenWindowDragging(e);

                        if (HasItems)
                        {
                            bool notAboveTabItem = true;
                            FrameworkElement templateParent = (FrameworkElement)originalSource.TemplatedParent;

                            if (null != templateParent)
                            {
                                TabItem item = templateParent as TabItem;
                                item = item ?? templateParent.TemplatedParent as TabItem;

                                if (null != item)
                                {
                                    if (item.Parent == null && originalSource.Name == "Border" && originalSource.Parent == null)
                                    {
                                        ShowNewContent(item.Content, m_isShow);
                                        notAboveTabItem = false;
                                        e.Handled = true;
                                    }
                                }
                                else if (templateParent is DockHeaderPresenter
                                        || templateParent.TemplatedParent is DockHeaderPresenter)
                                {
                                    notAboveTabItem = false;
                                }
                            }
                            if (m_owner.sidepanelmouseover)
                            {
                                ProcessAnimation(notAboveTabItem, originalSource);
                            }
                            else
                            {
                                m_owner.sidepanelmouseover = true;
                            }
                            ValidateProcess();
                        }
                    }
                    m_restrictanimation = false;
                }
            }
        }

        protected override void OnDragLeave(DragEventArgs e)
        {
            if (Owner != null)
            {
                bool wfh = Owner.UseInteropCompatibilityMode && ContainsHwndHost((FrameworkElement)InternalDataContext);

                if (wfh && !IsCursorInsideHost() || !wfh)
                {
                    base.OnDragLeave(e);

                    if (!m_contextMenuOpen && !IsOpenHeaderContextMenu && !IsShowedFocusedItem)
                    {
                        SwitchAnimation(false);
                    }
                    else
                    {
                        m_contextMenuOpen = false;
                    }
                }
            }
            m_inDragMode = false;
        }

        /// <summary>
        /// Invoked when an unhandled Mouse.MouseMove attached event reaches an element 
        /// in its route that is derived from this class. Implement this method to add 
        /// class handling for this event. 
        /// </summary>
        /// <param name="e">The MouseEventArgs that contains the event data.</param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (m_owner != null && m_owner.IsAnimationEnabledOnMouseOver)
            {
                FrameworkElement originalSource = e.OriginalSource as FrameworkElement;

                FrameworkElement element = null;
                bool m_animationenabled = true;
                if ((e.Source as SidePanel) != null && (e.Source as SidePanel).InternalDataContext != null && originalSource!=null)
                {
                    FrameworkElement actualelement = null;
                    element = (FrameworkElement)originalSource.TemplatedParent;
                    if (element != null && (element as TabItem) != null)
                    {
                        actualelement = element;
                    }
                    else if(element!=null)
                    {
                        actualelement = (FrameworkElement)element.TemplatedParent;
                    }
                    if (actualelement != null && (actualelement as TabItem) != null && (actualelement as TabItem).Content is DependencyObject
                        &&  DockingManager.GetDockWindowState((actualelement as TabItem).Content as DependencyObject) == WindowState.Minimized && !m_owner.m_exceutingminimizeflag)
                    {
                        m_animationenabled = false;
                    }
                }

                if (null != originalSource && !m_restrictanimation && IsNotHeaderButton(originalSource, AwlButtonName) && IsNotHeaderButton(originalSource, CloseButtonName)
                    && m_animationenabled)
                {
                    SwitchHiddenWindow(e);

                    if (HasItems)
                    {
                        bool notAboveTabItem = true;
                        FrameworkElement templateParent = (FrameworkElement)originalSource.TemplatedParent;

                        if (null != templateParent)
                        {
                            TabItem item = templateParent as TabItem;
                            item = item ?? templateParent.TemplatedParent as TabItem;

                            if (null != item)
                            {
                                if (item.Parent == null &&  originalSource.Parent==null)
                                {
                                    ShowNewContent(item.Content, m_isShow);
                                    notAboveTabItem = false;
                                    e.Handled = true;
                                }
                            }
                            else if (templateParent is DockHeaderPresenter
                                    || templateParent.TemplatedParent is DockHeaderPresenter)
                            {
                                notAboveTabItem = false;
                            }
                        }
                        if (m_owner.sidepanelmouseover)
                        {
                            ProcessAnimation(notAboveTabItem, originalSource);
                        }
                        else
                        {
                            m_owner.sidepanelmouseover = true;
                        }
                        ValidateProcess();
                    }
                }
                m_restrictanimation = false;
            }
        }

        /// <summary>
        /// Occurs when mouse enters the tab. Shows hidden window.
        /// </summary>
        /// <param name="e">EventArgs to find window element</param>
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            if (e.StylusDevice == null || e.StylusDevice != null)
            {
                FrameworkElement element = null;
                bool m_animationenabled = true;
                if ((e.OriginalSource as SidePanel) != null && (e.OriginalSource as SidePanel).InternalDataContext != null)
                {
                    element = (e.OriginalSource as SidePanel).InternalDataContext as FrameworkElement;
                    if (element != null && m_owner != null && DockingManager.GetDockWindowState(element as DependencyObject) == WindowState.Minimized
                        && !m_owner.m_exceutingminimizeflag)
                    {
                        m_animationenabled = false;
                    }
                }
                if (m_owner != null && !m_restrictanimation && m_owner.IsAnimationEnabledOnMouseOver && m_animationenabled)
                {
                    SwitchHiddenWindow(e);
                }
                m_restrictanimation = false;
            }
        }

        /// <summary>
        /// Hides docked window when it lost focus.
        /// </summary>
        /// <param name="e">EventArgs to find focused window element</param>
        protected override void OnLostFocus(RoutedEventArgs e)
        {
            if (!IsShowedFocusedItem)
            {
                base.OnLostFocus(e);
                SwitchAnimation(false);
            }

            if (!m_contextMenuOpen && !IsOpenHeaderContextMenu && !IsShowedFocusedItem)
            {
                m_Timer.Stop();
                FireHide();
            }
        }

        /// <summary>
        /// Hides docked window when user first clicks child of the window and then clicks content.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.KeyboardFocusChangedEventArgs"/> that contains event data.</param>
        protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            if (!IsOpenHeaderContextMenu && !IsKeyboardFocusWithin)
            {
                if (e.NewFocus is FrameworkElement)
                {
                    FrameworkElement newFocus = (FrameworkElement)e.NewFocus;

                    if (null != newFocus && newFocus != m_contextMenu && !object.Equals(newFocus, this))
                    {
                        Visual instance = VisualUtils.FindAncestor(newFocus, typeof(SidePanel));

                        if (!object.Equals(instance, this))
                        {
                            base.OnLostKeyboardFocus(e);
                            SwitchAnimation(false);
                            FocusedItem = null;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gives focus to showed window when user clicks the tab of this window.
        /// </summary>
        /// <param name="e">EventArgs to find window element that must be focused</param>
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            if (e.StylusDevice == null || e.StylusDevice != null)
            {

                bool m_animationenabled = true;
                FrameworkElement element = (FrameworkElement)e.Source;
                FrameworkElement parent = (FrameworkElement)element.Parent;

                if ((parent is SidePanel) && (element is SidePanel == false))
                {
                    SidePanel panelParent = (SidePanel)parent;
                    panelParent.Focus();
                }
                if ((e.Source as SidePanel) != null && (e.Source as SidePanel).InternalDataContext != null)
                {
                    element = (e.Source as SidePanel).InternalDataContext as FrameworkElement;
                    if (element != null && m_owner != null && DockingManager.GetDockWindowState(element as DependencyObject) == WindowState.Minimized
                        && !m_owner.m_exceutingminimizeflag)
                    {
                        m_animationenabled = false;
                    }
                    if (element != null)
                    {
                        CheckElementView(element);
                    }
                }

                if (m_animationenabled)
                {
                    SwitchHiddenWindow(e);
                }
            }
        }

        /// <summary>
        /// Checks the element view.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="panel">The panel.</param>
        private void CheckElementView(FrameworkElement actualelement)
        {
            DirectTabPanel panel = GetTemplateChild("PART_PanelName") as DirectTabPanel;
            Border borderpanel = GetTemplateChild("PART_BorderName") as Border;
            ScrollViewer scrollviewer = GetTemplateChild("PART_ScrollPanel") as ScrollViewer;
            StackPanel m_leftscrollingbuttons = GetTemplateChild("PART_ScrollButtons") as StackPanel;
            ScrollButtonsBar m_topscrollingbuttons = GetTemplateChild("PART_BottomPanel") as ScrollButtonsBar;
            if (panel != null && scrollviewer != null)
            {
                double actualheight = scrollviewer.ViewportHeight + scrollviewer.VerticalOffset;
                double actualwidth = scrollviewer.ViewportWidth + scrollviewer.HorizontalOffset;
                double sumHeight = 0, sumWidth = 0;
                for (int i = 0; i < panel.Children.Count; i++)
                {
                    FrameworkElement element = panel.Children[i] as FrameworkElement;
                    if (Visibility.Collapsed != element.Visibility)
                    {
                        element.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                        Size desiredSizeWithoutMargin = DirectTabPanel.GetDesiredSizeWithoutMargin(element);

                        sumHeight += desiredSizeWithoutMargin.Height;
                        sumWidth += desiredSizeWithoutMargin.Width;
                        if ((element as TabItem) != null && (element as TabItem).Content != null && (element as TabItem).Content.Equals(actualelement))
                        {
                            if (m_leftscrollingbuttons != null)
                            {
                                if (sumHeight < scrollviewer.ViewportHeight)
                                {
                                    if (Canvas.GetTop(borderpanel).Equals(m_leftscrollingbuttons.ActualHeight))
                                    {
                                        if (sumHeight > scrollviewer.ViewportHeight - m_leftscrollingbuttons.ActualHeight)
                                        {
                                            scrollviewer.ScrollToVerticalOffset(scrollviewer.VerticalOffset + m_leftscrollingbuttons.ActualHeight);
                                        }
                                    }
                                    else
                                    {
                                        if (borderpanel != null)
                                        {
                                            Canvas.SetTop(borderpanel, m_leftscrollingbuttons.ActualHeight);
                                        }
                                        if ((scrollviewer.VerticalOffset + scrollviewer.ViewportHeight - (sumHeight - desiredSizeWithoutMargin.Height)) > 0)
                                        {
                                            scrollviewer.ScrollToVerticalOffset(scrollviewer.VerticalOffset - (scrollviewer.VerticalOffset - (sumHeight - desiredSizeWithoutMargin.Height)));
                                        }
                                        else
                                        {
                                            scrollviewer.ScrollToHome();
                                        }
                                    }
                                }
                                else if (sumHeight - desiredSizeWithoutMargin.Height - m_leftscrollingbuttons.ActualHeight > scrollviewer.ExtentHeight - scrollviewer.ViewportHeight)
                                {
                                    if (borderpanel != null)
                                    {
                                        Canvas.SetTop(borderpanel, 0);
                                    }
                                    if ((scrollviewer.VerticalOffset + scrollviewer.ViewportHeight + (sumHeight - (scrollviewer.VerticalOffset + scrollviewer.ViewportHeight))) < scrollviewer.ExtentHeight)
                                    {
                                        scrollviewer.ScrollToVerticalOffset(scrollviewer.VerticalOffset + (sumHeight - (scrollviewer.VerticalOffset + scrollviewer.ViewportHeight)));
                                    }
                                    else
                                    {
                                        scrollviewer.ScrollToEnd();
                                    }
                                }
                                else
                                {
                                    if (sumHeight.Equals(actualheight))
                                    {
                                        if (Canvas.GetTop(borderpanel).Equals(m_leftscrollingbuttons.ActualHeight))
                                        {
                                            scrollviewer.ScrollToVerticalOffset(scrollviewer.VerticalOffset + m_leftscrollingbuttons.ActualHeight);
                                        }
                                    }
                                    else if (sumHeight < actualheight)
                                    {
                                        if (actualheight - sumHeight - desiredSizeWithoutMargin.Height > scrollviewer.ViewportHeight)
                                        {
                                            scrollviewer.ScrollToVerticalOffset(scrollviewer.VerticalOffset - actualheight - sumHeight - desiredSizeWithoutMargin.Height - scrollviewer.ViewportHeight);
                                        }
                                        if (actualheight - sumHeight < m_leftscrollingbuttons.ActualHeight)
                                        {
                                            if (Canvas.GetTop(borderpanel).Equals(m_leftscrollingbuttons.ActualHeight))
                                            {
                                                scrollviewer.ScrollToVerticalOffset(scrollviewer.VerticalOffset + m_leftscrollingbuttons.ActualHeight);
                                            }
                                        }
                                    }
                                    else if (sumHeight > actualheight)
                                    {
                                        if (Canvas.GetTop(borderpanel).Equals(m_leftscrollingbuttons.ActualHeight))
                                        {
                                            scrollviewer.ScrollToVerticalOffset(scrollviewer.VerticalOffset + m_leftscrollingbuttons.ActualHeight + sumHeight - actualheight);
                                        }
                                        else
                                        {
                                            scrollviewer.ScrollToVerticalOffset(scrollviewer.VerticalOffset + sumHeight - actualheight);
                                        }
                                    }                                    
                                }
                            }

                            if (m_topscrollingbuttons != null)
                            {
                                if (sumWidth < scrollviewer.ViewportWidth)
                                {
                                    if (Canvas.GetLeft(borderpanel).Equals(m_topscrollingbuttons.ActualWidth))
                                    {
                                        if (sumWidth > scrollviewer.ViewportWidth - m_topscrollingbuttons.ActualWidth)
                                        {
                                            scrollviewer.ScrollToHorizontalOffset(scrollviewer.HorizontalOffset + m_topscrollingbuttons.ActualWidth);
                                        }
                                    }
                                    else
                                    {
                                        if (borderpanel != null)
                                        {
                                            Canvas.SetLeft(borderpanel, m_topscrollingbuttons.ActualWidth);
                                        }
                                        if ((scrollviewer.HorizontalOffset + scrollviewer.ViewportWidth - (sumWidth - desiredSizeWithoutMargin.Width)) > 0)
                                        {
                                            scrollviewer.ScrollToHorizontalOffset(scrollviewer.HorizontalOffset - (scrollviewer.HorizontalOffset - (sumWidth-desiredSizeWithoutMargin.Width)));
                                        }
                                        else
                                        {
                                            scrollviewer.ScrollToLeftEnd();
                                        }
                                    }
                                }
                                else if (sumWidth - desiredSizeWithoutMargin.Width - m_topscrollingbuttons.ActualWidth > scrollviewer.ExtentWidth - scrollviewer.ViewportWidth)
                                {
                                    if (borderpanel != null)
                                    {
                                        Canvas.SetLeft(borderpanel, 0);
                                    }
                                    if ((scrollviewer.HorizontalOffset + scrollviewer.ViewportWidth + (sumWidth - (scrollviewer.HorizontalOffset + scrollviewer.ViewportWidth))) < scrollviewer.ExtentWidth)
                                    {
                                        scrollviewer.ScrollToHorizontalOffset(scrollviewer.HorizontalOffset + (sumWidth - (scrollviewer.HorizontalOffset + scrollviewer.ViewportWidth)));
                                    }
                                    else
                                    {
                                        scrollviewer.ScrollToRightEnd();
                                    }
                                }
                                else
                                {
                                    if (sumWidth.Equals(actualwidth))
                                    {
                                        if (Canvas.GetLeft(borderpanel).Equals(m_topscrollingbuttons.ActualWidth))
                                        {
                                            scrollviewer.ScrollToHorizontalOffset(scrollviewer.HorizontalOffset + m_topscrollingbuttons.ActualWidth);
                                        }
                                    }
                                    else if (sumWidth < actualwidth)
                                    {
                                        if (actualwidth - sumWidth - desiredSizeWithoutMargin.Width > scrollviewer.ViewportWidth)
                                        {
                                            scrollviewer.ScrollToHorizontalOffset(scrollviewer.HorizontalOffset - actualwidth - sumWidth - desiredSizeWithoutMargin.Width - scrollviewer.ViewportWidth);
                                        }
                                        if (actualwidth - sumWidth < m_topscrollingbuttons.ActualWidth)
                                        {
                                            if (Canvas.GetLeft(borderpanel).Equals(m_topscrollingbuttons.ActualWidth))
                                            {
                                                scrollviewer.ScrollToHorizontalOffset(scrollviewer.HorizontalOffset + m_topscrollingbuttons.ActualWidth);
                                            }
                                        }
                                    }
                                    else if (sumWidth > actualwidth)
                                    {
                                        if (Canvas.GetLeft(borderpanel).Equals(m_topscrollingbuttons.ActualWidth))
                                        {
                                            scrollviewer.ScrollToHorizontalOffset(scrollviewer.HorizontalOffset + m_topscrollingbuttons.ActualWidth + sumWidth - actualwidth);
                                        }
                                        else
                                        {
                                            scrollviewer.ScrollToHorizontalOffset(scrollviewer.HorizontalOffset + sumWidth - actualwidth);
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
        /// Occurs when mouse leaves side panel.
        /// </summary>
        /// <param name="e">EventArgs to find side panel that must be leaved.</param>
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            if (Owner != null && e.StylusDevice == null || e.StylusDevice != null)
            {
                bool wfh = Owner.UseInteropCompatibilityMode && ContainsHwndHost((FrameworkElement)InternalDataContext);

                if (wfh && !IsCursorInsideHost() || !wfh)
                {
                    base.OnMouseLeave(e);

                    if (!m_contextMenuOpen && !IsOpenHeaderContextMenu && !IsShowedFocusedItem)
                    {
                        SwitchAnimation(false);
                    }
                    else
                    {
                        m_contextMenuOpen = false;
                    }
                }
            }
        }

        /// <summary>
        /// Invoked when an unhandled SelectionChanged�attached event reaches an element of the side panel.
        /// </summary>
        /// <param name="e">The SelectionChangedEventArgs that contains the event data.</param>
        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            if (lastselecteditem != null && e.RemovedItems.Count>0 && lastselecteditem != (e.RemovedItems[0] as FrameworkElement))
                RaiseHideEvent();
            base.OnSelectionChanged(e);
            m_isShow = false;
            if (e.AddedItems.Count > 0 && (e.AddedItems[0] as FrameworkElement) != null)
            {
                InternalDataContext = e.AddedItems[0] as FrameworkElement;
            }
            SetIsShowedFocusedItem();
        }

        /// <summary>
        /// Invoked when an unhandled PreviewMouseLeftButtonDown�routed event reaches an element.
        /// </summary>
        /// <param name="e">The MouseButtonEventArgs that contains the event data. The event data reports that the mouse button was released.</param>
        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonDown(e);
            if (DockingManager != null && e.StylusDevice == null || e.StylusDevice != null)
            {
                DockingManager.isSidePanelClicked = true;
                SetFocusedItem(e);
            }
        }

#if !SyncfusionFramework3_5

        //void m_leftscrollingbuttons_TouchEnter(object sender, TouchEventArgs e)
        //{
        //    if (DockingManager != null && DockingManager.IsTouchEnabled && DockingManager.m_TouchDeviceId == e.TouchDevice.Id)
        //        m_restrictanimation = true;
        //}

        //void m_leftscrollingbuttons_TouchMove(object sender, TouchEventArgs e)
        //{
        //    if (DockingManager != null && DockingManager.IsTouchEnabled && DockingManager.m_TouchDeviceId == e.TouchDevice.Id)
        //        m_restrictanimation = true;
        //}

        //void m_topscrollingbuttons_TouchMove(object sender, TouchEventArgs e)
        //{
        //    if (DockingManager != null && DockingManager.IsTouchEnabled && DockingManager.m_TouchDeviceId == e.TouchDevice.Id)
        //        m_restrictanimation = true;
        //}

        //void m_topscrollingbuttons_TouchEnter(object sender, TouchEventArgs e)
        //{
        //    if (DockingManager != null && DockingManager.IsTouchEnabled && DockingManager.m_TouchDeviceId == e.TouchDevice.Id)
        //        m_restrictanimation = true;
        //}

        //protected override void OnTouchEnter(TouchEventArgs e)
        //{
        //    if (DockingManager != null && DockingManager.IsTouchEnabled && DockingManager.m_TouchDeviceId == e.TouchDevice.Id)
        //    {
        //        FrameworkElement element = null;
        //        bool m_animationenabled = true;
        //        if ((e.OriginalSource as SidePanel) != null && (e.OriginalSource as SidePanel).InternalDataContext != null)
        //        {
        //            element = (e.OriginalSource as SidePanel).InternalDataContext as FrameworkElement;
        //            if (element != null && m_owner != null && DockingManager.GetDockWindowState(element as DependencyObject) == WindowState.Minimized
        //                && !m_owner.m_exceutingminimizeflag)
        //            {
        //                m_animationenabled = false;
        //            }
        //        }
        //        if (m_owner != null && !m_restrictanimation && m_owner.IsAnimationEnabledOnMouseOver && m_animationenabled)
        //        {
        //            SwitchHiddenWindow(e);
        //        }
        //        m_restrictanimation = false;
        //    }
        //    base.OnTouchEnter(e);
        //}

        //protected override void OnTouchLeave(TouchEventArgs e)
        //{
        //    if (DockingManager != null && DockingManager.IsTouchEnabled && DockingManager.m_TouchDeviceId == e.TouchDevice.Id)
        //    {
        //        bool wfh = Owner.UseInteropCompatibilityMode && ContainsHwndHost((FrameworkElement)InternalDataContext);

        //        if (wfh && !IsCursorInsideHost() || !wfh)
        //        {
        //            base.OnTouchLeave(e);

        //            if (!m_contextMenuOpen && !IsOpenHeaderContextMenu && !IsShowedFocusedItem)
        //            {
        //                SwitchAnimation(false);
        //            }
        //            else
        //            {
        //                m_contextMenuOpen = false;
        //            }
        //        }
        //    }
        //}

        //protected override void OnTouchMove(TouchEventArgs e)
        //{
        //    if (DockingManager != null && DockingManager.IsTouchEnabled && DockingManager.m_TouchDeviceId == e.TouchDevice.Id)
        //    {
        //        FrameworkElement originalSource = e.OriginalSource as FrameworkElement;

        //        FrameworkElement element = null;
        //        bool m_animationenabled = true;
        //        if ((e.Source as SidePanel) != null && (e.Source as SidePanel).InternalDataContext != null && originalSource != null)
        //        {
        //            FrameworkElement actualelement = null;
        //            element = (FrameworkElement)originalSource.TemplatedParent;
        //            if (element != null && (element as TabItem) != null)
        //            {
        //                actualelement = element;
        //            }
        //            else if (element != null)
        //            {
        //                actualelement = (FrameworkElement)element.TemplatedParent;
        //            }
        //            if (actualelement != null && (actualelement as TabItem) != null && (actualelement as TabItem).Content is DependencyObject
        //                && DockingManager.GetDockWindowState((actualelement as TabItem).Content as DependencyObject) == WindowState.Minimized && !m_owner.m_exceutingminimizeflag)
        //            {
        //                m_animationenabled = false;
        //            }
        //        }

        //        if (null != originalSource && !m_restrictanimation && IsNotHeaderButton(originalSource, AwlButtonName) && IsNotHeaderButton(originalSource, CloseButtonName)
        //            && m_animationenabled)
        //        {
        //            SwitchHiddenWindow(e);

        //            if (HasItems)
        //            {
        //                bool notAboveTabItem = true;
        //                FrameworkElement templateParent = (FrameworkElement)originalSource.TemplatedParent;

        //                if (null != templateParent)
        //                {
        //                    TabItem item = templateParent as TabItem;
        //                    item = item ?? templateParent.TemplatedParent as TabItem;

        //                    if (null != item)
        //                    {
        //                        if (item.Parent == null && originalSource.Parent == null)
        //                        {
        //                            ShowNewContent(item.Content, m_isShow);
        //                            notAboveTabItem = false;
        //                            e.Handled = true;
        //                        }
        //                    }
        //                    else if (templateParent is DockHeaderPresenter
        //                            || templateParent.TemplatedParent is DockHeaderPresenter)
        //                    {
        //                        notAboveTabItem = false;
        //                    }
        //                }
        //                if (m_owner.sidepanelmouseover)
        //                {
        //                    ProcessAnimation(notAboveTabItem, originalSource);
        //                }
        //                else
        //                {
        //                    m_owner.sidepanelmouseover = true;
        //                }
        //                ValidateProcess();
        //            }
        //        }
        //        m_restrictanimation = false;

        //        if (DockingManager.m_dockingManagerSystemGesture == SystemGesture.HoldEnter)
        //            OnTouchRightFingerDown(e);

        //        base.OnTouchMove(e);
        //    }
        //}

        //protected override void OnTouchUp(TouchEventArgs e)
        //{
        //    if (DockingManager != null && DockingManager.IsTouchEnabled && DockingManager.m_TouchDeviceId == e.TouchDevice.Id)
        //    {
        //        #region TouchLeftFingerUp
        //        if (DockingManager.m_dockingManagerSystemGesture == SystemGesture.Tap)
        //            OnTouchLeftFingerUp(e);
        //        #endregion
        //        base.OnTouchUp(e);
        //    }
        //}

        //protected override void OnPreviewTouchDown(TouchEventArgs e)
        //{
        //    base.OnPreviewTouchDown(e);
        //    if (DockingManager != null && DockingManager.IsTouchEnabled && DockingManager.m_TouchDeviceId == e.TouchDevice.Id)
        //    {
        //        SidePanel panel = VisualUtils.FindAncestor(e.OriginalSource as Visual, typeof(SidePanel)) as SidePanel;
        //        if (panel != null)
        //        {
        //            SetFocusedItem(e);
        //        }
        //        m_isHideTabWhenMenuOpen = false;
        //        OnPreviewTouchLeftFingerDown(e);
        //    }
        //}

        //private void OnTouchLeftFingerUp(TouchEventArgs e)
        //{
        //    bool m_animationenabled = true;
        //    FrameworkElement element = (FrameworkElement)e.Source;
        //    FrameworkElement parent = (FrameworkElement)element.Parent;

        //    if ((parent is SidePanel) && (element is SidePanel == false))
        //    {
        //        SidePanel panelParent = (SidePanel)parent;
        //        panelParent.Focus();
        //    }
        //    if ((e.Source as SidePanel) != null && (e.Source as SidePanel).InternalDataContext != null)
        //    {
        //        element = (e.Source as SidePanel).InternalDataContext as FrameworkElement;
        //        if (element != null && m_owner != null && DockingManager.GetDockWindowState(element as DependencyObject) == WindowState.Minimized
        //            && !m_owner.m_exceutingminimizeflag)
        //        {
        //            m_animationenabled = false;
        //        }
        //        if (element != null)
        //        {
        //            CheckElementView(element);
        //        }
        //    }

        //    if (m_animationenabled)
        //    {
        //        SwitchHiddenWindow(e);
        //    }
        //}

        //private void OnTouchRightFingerDown(TouchEventArgs e)
        //{
        //    if (e.OriginalSource is Border)
        //    {
        //        SetFocusedItem(e);
        //    }

        //    m_isHideTabWhenMenuOpen = false;
        //}

        //private void OnPreviewTouchLeftFingerDown(TouchEventArgs e)
        //{
        //    DockingManager.isSidePanelClicked = true;
        //    SetFocusedItem(e);
        //}

        //void ContextMenu_PreviewTouchDown(object sender, TouchEventArgs e)
        //{
        //    if (DockingManager != null && DockingManager.IsTouchEnabled && DockingManager.m_TouchDeviceId == e.TouchDevice.Id)
        //    {
        //        if (DockingManager.m_dockingManagerSystemGesture == SystemGesture.Tap)
        //        {
        //            m_isHideTabWhenMenuOpen = true;
        //            FrameworkElement element = (FrameworkElement)e.OriginalSource;
        //            MenuItem item = element.TemplatedParent as MenuItem;
        //            if (null != item)
        //            {
        //                //m_weakShowAnimation = false;
        //                bool bIsEqualDataContext = object.Equals(InternalDataContext, item.DataContext);

        //                Focus();

        //                if (null != item && !bIsEqualDataContext)
        //                {
        //                    ShowNewContent(item.DataContext, true);
        //                }
        //                else if (!IsContentHiden && !bIsEqualDataContext ||
        //                         IsContentHiden && bIsEqualDataContext)
        //                {
        //                    FireShow();
        //                }

        //                if (item != null)
        //                {
        //                    FocusedItem = item.DataContext;
        //                }
        //            }

        //            e.Handled = true;
        //        }
        //    }
        //}

        //void ContextMenu_TouchLeave(object sender, TouchEventArgs e)
        //{
        //    if (DockingManager != null && DockingManager.IsTouchEnabled && DockingManager.m_TouchDeviceId == e.TouchDevice.Id)
        //    {
        //        if (m_focusedItem == null)
        //        {
        //            m_isHideTabWhenMenuOpen = true;
        //            FireHide();
        //        }
        //    }
        //}

#endif
        /// <summary>
        /// Is called to show auto hided window.
        /// </summary>
        private void FireShow()
        {            
            if ( Owner!=null && !m_isShowing && (m_weakHideAnimation || AssureHideAnimationCompleted()) && !Owner.m_StopFireShow) 
            {
                Owner.CollapseAutohiddenItems(this);
                RestoreShadow();
                m_isShowing = true;

                //if (!Owner.UseInteropCompatibilityMode)
                //{
                    RaiseShowEvent();
                //}

                IsContentHiden = false;

                if (Owner.UsePopupAutoHidePreview && m_popuppanel != null && !m_popuppanel.IsOpen)
                {
                    if(AutoHideAnimationMode!=AutoHideAnimationMode.Slide)
                        m_popuppanel.AllowsTransparency = true;

                    if (PanelSide == Dock.Left)
                    {
                        m_popuppanel.HorizontalOffset = this.ActualWidth - 2;
                        m_popuppanel.Placement = PlacementMode.Right;
                    }
                    else if (PanelSide == Dock.Right)
                    {
                        m_popuppanel.Placement = PlacementMode.Left;
                    }
                    else if (PanelSide == Dock.Top)
                    {
                        m_popuppanel.VerticalOffset = this.ActualHeight;
                        m_popuppanel.Placement = PlacementMode.Bottom;
                    }
                    else
                        m_popuppanel.Placement = PlacementMode.Top;

                    
                    m_popuppanel.PlacementRectangle = new Rect(this.DesiredSize);
                    m_popuppanel.PlacementTarget = this;                    
                    m_popuppanel.IsOpen = true;
                    ShowPopupAnimation().Begin();
                }

                if (Owner.UseInteropCompatibilityMode && !Owner.UsePopupAutoHidePreview)
                {
                    Owner.SetFakesForDockedElements();
                    DrawFakeForWinFormHost((FrameworkElement)InternalDataContext);
                }
            }
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This method raises HideEvent event.
        /// </summary>
        private void FireHide()
        {
            if (Visibility.Collapsed != Visibility && null == SelectedItem && 0 < Items.Count)
            {
                SelectedItem = Items[0];
            }

            if ((m_weakShowAnimation || AssureShowAnimationCompleted()) && !IsContentHiden && !IsSplitterPressed)
            {
                RestoreShadow();
                m_isShowing = false;
                
                if (Owner!=null && !Owner.UsePopupAutoHidePreview)
                {
                    SaveWinFormHostFake();
                }

                if (Owner!=null && Owner.UsePopupAutoHidePreview && m_popuppanel != null && m_popuppanel.IsOpen)
                {
                    if(AutoHideAnimationMode!=AutoHideAnimationMode.Slide)
                        m_popuppanel.AllowsTransparency = true;
                    HidePopupAnimation().Begin();
                }

                //if (!Owner.UseInteropCompatibilityMode)
                //{
                    RaiseHideEvent();
                //}
            }
        }

        /// <summary>
        /// Occurs when mouse leave context menu with no selected item on it.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        private void ContextMenu_MouseLeave(object sender, MouseEventArgs e)
        {
            if (e.StylusDevice == null || e.StylusDevice != null)
            {
                if (m_focusedItem == null)
                {
                    m_isHideTabWhenMenuOpen = true;
                    FireHide();
                }
            }
        }

        /// <summary>
        /// Determines whether [contains HWND host] [the specified element].
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>
        /// <c>true</c> if [contains HWND host] [the specified element]; otherwise, <c>false</c>.
        /// </returns>
        private bool ContainsHwndHost(FrameworkElement element)
        {
            return (element == null) ? false : (element is HwndHost)
                ? true : VisualUtils.HasChildOfType((Visual)element, typeof(HwndHost));
        }

        /// <summary>
        /// Determines whether [is cursor inside host].
        /// </summary>
        /// <returns>
        /// <c>true</c> if [is cursor inside host]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsCursorInsideHost()
        {
            bool result = true;
            FrameworkElement element = (FrameworkElement)GetTemplateChild("FlyPanel");
            Size size = element.RenderSize;
            Point pt = Mouse.PrimaryDevice.GetPosition(element);

            bool xOut = pt.X < 0 || pt.X >= size.Width;
            bool yOut = pt.Y < 0 || pt.Y >= size.Height;
            Dock placement = TabStripPlacement;

            if (placement == Dock.Left && (pt.X > size.Width || pt.X < -2 || yOut) ||
                placement == Dock.Right && (pt.X < 0 || pt.X > (size.Width + 2) || yOut) ||
                placement == Dock.Top && (pt.Y >= size.Height || pt.Y < -2 || xOut) ||
                placement == Dock.Bottom && (pt.Y <= 0 || pt.Y > (size.Height + 2) || xOut))
            {
                result = false;
            }

            return result;
        }

        /// <summary>
        /// Switches to show autoHidden window when user moves mouse from tab on side panel, then
        /// moves through side panel to auto hide button of the window. 
        /// It can reproduce when user works with bottom side panel (where width from tab to auto hide button is large)
        /// </summary>
        private void FastSwitchToShowAnimation()
        {
            if (!m_isShow && !IsSplitterPressed && !IsContentHiden)
            {
                m_isShow = true;
                FireShow();
            }
        }

        /// <summary>
        /// Switches the animation.
        /// </summary>
        /// <param name="isShow">if set to <c>true</c> [is show].</param>
        internal void SwitchAnimation(bool isShow)
        {
            if (m_isShow != isShow)
            {
                m_isShow = isShow;

                if (m_Timer.IsEnabled)
                {
                    m_Timer.Stop();
                }

                m_Timer.Start();
            }
            else if (m_Timer.IsEnabled)
            {
                m_Timer.Stop();
                m_Timer.Start();
            }
        }

        /// <summary>
        /// Called when [timer tick].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnTimerTick(object sender, EventArgs e)
        {
            if (m_isShow)
            {
                FireShow();
            }
            else if (m_isHideTabWhenMenuOpen)
            {
                FireHide();
            }

            m_Timer.Stop();
        }

        /// <summary>
        /// Called when [header is context menu open changed].
        /// </summary>
        /// <param name="d">The dependency object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private void OnHeaderIsContextMenuOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            bool newValue = (bool)e.NewValue;

            if (!newValue && !IsKeyboardFocusWithin)
            {
                SwitchAnimation(false);
            }
        }

        /// <summary>
        /// Processes the animation.
        /// </summary>
        /// <param name="notAboveTabItem">if set to <c>true</c> [not above tab item].</param>
        /// <param name="source">The source.</param>
        private void ProcessAnimation(bool notAboveTabItem, object source)
        {
            bool isDirectTabPanel = IsCursorOverDirectTabPanel(source);

            if (!IsShowedFocusedItem && notAboveTabItem && isDirectTabPanel)
            {
                SwitchAnimation(false);
            }
            else if (!isDirectTabPanel)
            {
                FastSwitchToShowAnimation();
            }
        }

        /// <summary>
        /// Called when [context menu preview mouse left button down].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void OnContextMenuPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.StylusDevice == null || e.StylusDevice != null)
            {
                m_isHideTabWhenMenuOpen = true;
                FrameworkElement element = (FrameworkElement)e.OriginalSource;
                MenuItem item = element.TemplatedParent as MenuItem;
                if (null != item)
                {
                    //m_weakShowAnimation = false;
                    bool bIsEqualDataContext = object.Equals(InternalDataContext, item.DataContext);

                    Focus();

                    if (null != item && !bIsEqualDataContext)
                    {
                        ShowNewContent(item.DataContext, true);
                    }
                    else if (!IsContentHiden && !bIsEqualDataContext ||
                             IsContentHiden && bIsEqualDataContext)
                    {
                        FireShow();
                    }

                    if (item != null)
                    {
                        FocusedItem = item.DataContext;
                    }
                }

                e.Handled = true;
            }
        }

        /// <summary>
        /// Invoked when an unhandled MouseRightButtonDown�routed event reaches an element.
        /// </summary>
        /// <param name="e">The MouseButtonEventArgs that contains the event data. The event data reports that the mouse button was released.</param>
        protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseRightButtonDown(e);
            if (DockingManager != null && e.StylusDevice == null || e.StylusDevice != null)
            {
                if (e.OriginalSource is Border)
                {
                    SetFocusedItem(e);
                }

                m_isHideTabWhenMenuOpen = false;
            }
        }

        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseDown(e);
            if (e.StylusDevice == null || e.StylusDevice != null)
            {
                if (e.ChangedButton == MouseButton.Right || e.ChangedButton == MouseButton.Left)
                {
                    SidePanel panel = VisualUtils.FindAncestor(e.OriginalSource as Visual, typeof(SidePanel)) as SidePanel;
                    if (panel != null)
                    {
                        SetFocusedItem(e);
                    }
                    m_isHideTabWhenMenuOpen = false;
                }
            }
        }

        /// <summary>
        /// Invoked when an unhandled ContextMenuClosing�routed event reaches an element.
        /// </summary>
        /// <param name="e">The ContextMenuEventArgs that contains the event data.</param>
        protected override void OnContextMenuClosing(ContextMenuEventArgs e)
        {
            base.OnContextMenuClosing(e);
            m_isHideTabWhenMenuOpen = true;
        }

        /// <summary>
        /// Sets the focused item.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void SetFocusedItem(RoutedEventArgs e)
        {
            if (!IsCursorOverDirectTabPanel(e.OriginalSource))
            {
                FocusedItem = InternalDataContext;
            }
        }

        /// <summary>
        /// Called when [side panel selection changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void OnSidePanelSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            InternalDataContext = SelectedItem as FrameworkElement;
        }

        /// <summary>
        /// Called when [side panel context menu opening].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.ContextMenuEventArgs"/> instance containing the event data.</param>
        private void OnSidePanelContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            m_contextMenuOpen = true;
            m_isHideTabWhenMenuOpen = false;
        }

        private void SwitchHiddenWindowDragging(DragEventArgs e)
        {
            HitTestResult hitTest = VisualTreeHelper.HitTest(this, e.GetPosition(this));

            if (hitTest != null)
            {
                FrameworkElement visualHit = hitTest.VisualHit as FrameworkElement;

                if (null != visualHit)
                {
                    DependencyObject templatedParent = visualHit.TemplatedParent;

                    if (null != templatedParent
                        && (templatedParent is ContentPresenter) || (templatedParent is TabItem))
                    {
                        SwitchAnimation(true);
                    }
                }
            }
        }


        /// <summary>
        /// Checks for mouseOver on side panel or menu item and shows hidden window when mouse over was on menu item.
        /// </summary>
        /// <param name="e">mouse event parameters.</param>
        private void SwitchHiddenWindow(InputEventArgs e)
        {
            Point point = new Point();
            if (e is MouseEventArgs)
                point = (e as MouseEventArgs).GetPosition(this);
#if !SyncfusionFramework3_5
            else if (e is TouchEventArgs)
                point = (e as TouchEventArgs).GetTouchPoint(this).Position;
#endif
            HitTestResult hitTest = VisualTreeHelper.HitTest(this, point);

            if (hitTest != null)
            {
                FrameworkElement visualHit = hitTest.VisualHit as FrameworkElement;

                if (null != visualHit)
                {
                    DependencyObject templatedParent = visualHit.TemplatedParent;

                    if (null != templatedParent
                        && (templatedParent is ContentPresenter) || (templatedParent is TabItem))
                    {
                        SwitchAnimation(true);
                    }
                }
            }
        }

        /// <summary>
        /// Collapses the content.
        /// </summary>
        private void CollapseContent()
        {
            m_isShowing = false;
            IsContentHiden = true;

            switch (AutoHideAnimationMode)
            {
                case AutoHideAnimationMode.Slide:
                    SetSidePropertyByAnimation(ContentRenderTransformYProperty, ContentRenderTransformXProperty, 1);
                    break;
                case AutoHideAnimationMode.Scale:
                    SetSidePropertyByAnimation(ContentScaleYProperty, ContentScaleXProperty, 0);
                    break;
                case AutoHideAnimationMode.Fade:
                    VisualUtils.SetDependencyPropretyUsedByAnimation(this, ContentOpacityProperty, 0);
                    break;
                default:
                    throw new NotSupportedException("This case not support");
            }
        }

        /// <summary>
        /// Assures the show animation is completed.
        /// </summary>
        /// <returns>Assures the animation completed</returns>
        private bool AssureShowAnimationCompleted()
        {
            bool completed = false;
            bool isVertical = Dock.Top == PanelSide || Dock.Bottom == PanelSide;

            switch (AutoHideAnimationMode)
            {
                case AutoHideAnimationMode.Slide:
                    completed = isVertical ? ContentRenderTransformY == 0 : ContentRenderTransformX == 0;
                    break;
                case AutoHideAnimationMode.Scale:
                    completed = isVertical ? ContentScaleY == 1 : ContentScaleX == 1;
                    break;
                case AutoHideAnimationMode.Fade:
                    completed = ContentOpacity == 1;
                    break;
            }

            return completed;
        }

        /// <summary>
        /// Assures the hide animation is completed.
        /// </summary>
        /// <returns>Assures the animation completed</returns>
        private bool AssureHideAnimationCompleted()
        {
            bool completed = false;
            bool isVertical = Dock.Top == PanelSide || Dock.Bottom == PanelSide;

            switch (AutoHideAnimationMode)
            {
                case AutoHideAnimationMode.Slide:
                    completed = isVertical ? ContentRenderTransformY != 0 : ContentRenderTransformX != 0;
                    break;
                case AutoHideAnimationMode.Scale:
                    completed = isVertical ? ContentScaleY == 0 : ContentScaleX == 0;
                    break;
                case AutoHideAnimationMode.Fade:
                    completed = ContentOpacity == 0;
                    break;
            }

            return completed;
        }

        /// <summary>
        /// Shows the new content.
        /// </summary>
        /// <param name="element">The element.</param>
        private void ShowNewContent(object element)
        {
            CollapseContent();
            SelectedItem = element;
            FireShow();
        }

        /// <summary>
        /// Shows the new content.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="isShow">if set to <c>true</c> [is show].</param>
        private void ShowNewContent(object element, bool isShow)
        {
            if (isShow && !object.Equals(element, SelectedItem))
            {
                ShowNewContent(element);
            }
        }

        /// <summary>
        /// Sets the side property by animation.
        /// </summary>
        /// <param name="dpVertical">The dp vertical.</param>
        /// <param name="dpHorizontal">The dp horizontal.</param>
        /// <param name="value">The value.</param>
        private void SetSidePropertyByAnimation(DependencyProperty dpVertical, DependencyProperty dpHorizontal, double value)
        {
            if (Dock.Top == PanelSide || Dock.Bottom == PanelSide)
            {
                VisualUtils.SetDependencyPropretyUsedByAnimation(this, dpVertical, value);
            }
            else
            {
                VisualUtils.SetDependencyPropretyUsedByAnimation(this, dpHorizontal, value);
            }
        }

        /// <summary>
        /// Hides the new content.
        /// </summary>
        /// <param name="element">The element.</param>
        private void HideNewContent(DependencyObject element)
        {
            if (DockingManager.GetIsAddedElement(element))
            {
                SelectedItem = element;
                ClearBackground();
                if(DockingManager.GetAnimateOnNewItemAdded(element) && DockingManager.GetPreviousState(element) != DockState.Hidden)
                    ExpandContent();
                IsContentHiden = false;
                FireHide();
                DockingManager.SetIsAddedElement(element, false);
                m_weakHideAnimation = false;
                m_isNewContentHideAnimation = true;
            }
        }

        /// <summary>
        /// Expands the content.
        /// </summary>
        private void ExpandContent()
        {
            switch (AutoHideAnimationMode)
            {
                case AutoHideAnimationMode.Slide:
                    SetSidePropertyByAnimation(ContentRenderTransformYProperty, ContentRenderTransformXProperty, 0);
                    break;
                case AutoHideAnimationMode.Scale:
                    SetSidePropertyByAnimation(ContentScaleYProperty, ContentScaleXProperty, 1);
                    break;
                case AutoHideAnimationMode.Fade:
                    VisualUtils.SetDependencyPropretyUsedByAnimation(this, ContentOpacityProperty, 1);
                    break;
                default:
                    throw new NotSupportedException("This case not support");
            }
        }

        /// <summary>
        /// Called when [splitter is pressed changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnSplitterIsPressedChanged(object sender, EventArgs e)
        {
            if (IsSplitterPressed)
            {
                FrameworkElement element = (FrameworkElement)InternalDataContext;
                if (element != null)
                {
                    if (element.IsFocused)
                    {
                        element.Focus();
                    }
                    else
                    {
                        Focus();
                    }

                    double desiredWidth = DockingManager.GetDesiredWidthInDockedMode(element);
                    double desiredHeight = DockingManager.GetDesiredHeightInDockedMode(element);
                    bool IsHorizontal = m_splitter.Orientation == Orientation.Horizontal;
                    double splitterOffset = Math.Max(0, IsHorizontal ? desiredHeight - minElementHeight : desiredWidth - minElementWidth);

                    switch (PanelSide)
                    {
                        case Dock.Bottom:
                        case Dock.Right:
                            m_splitter.MaxOffsetRight = splitterOffset;
                            break;
                        case Dock.Top:
                        case Dock.Left:
                            m_splitter.MaxOffsetLeft = splitterOffset;
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Called when [splitter offset changed].
        /// </summary>
        /// <param name="d">The dependency object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private void OnSplitterOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            switch (PanelSide)
            {
                case Dock.Bottom:
                    ProcessForVerticalMode(-(double)e.NewValue);
                    break;
                case Dock.Top:
                    ProcessForVerticalMode((double)e.NewValue);
                    break;
                case Dock.Left:
                    ProcessForHorizontalMode((double)e.NewValue);
                    break;
                case Dock.Right:
                    ProcessForHorizontalMode(-(double)e.NewValue);
                    break;
            }

            ApplyTemplate();
        }

        /// <summary>
        /// Processes for horizontal mode.
        /// </summary>
        /// <param name="newValue">The new value.</param>
        private void ProcessForHorizontalMode(double newValue)
        {
            DependencyObject element = (DependencyObject)InternalDataContext;
            if (element != null)
            {
                double desiredWidth = DockingManager.GetDesiredWidthInDockedMode(element);

                desiredWidth += newValue;

                if (0 > desiredWidth)
                {
                    desiredWidth = 0;
                }
                var dock = DockingManager.GetDockingManager(this);
                if (desiredWidth > (dock.ActualWidth - 2 * this.ActualWidth))
                {
                    desiredWidth = dock.ActualWidth - 2 * this.ActualWidth;
                }

                DockingManager.SetDesiredWidthInDockedMode(element, desiredWidth);
            }
        }

        /// <summary>
        /// Processes for vertical mode.
        /// </summary>
        /// <param name="newValue">The new value.</param>
        private void ProcessForVerticalMode(double newValue)
        {
            DependencyObject element = (DependencyObject)InternalDataContext;
            if (element != null)
            {
                double desiredHeight = DockingManager.GetDesiredHeightInDockedMode(element);
                desiredHeight += newValue;

                if (0 > desiredHeight)
                {
                    desiredHeight = 0;
                }
                var dock = DockingManager.GetDockingManager(this);
                if (desiredHeight > (dock.ActualHeight - 2 * this.ActualHeight))
                {
                    desiredHeight = dock.ActualHeight - 2 * this.ActualHeight;
                }

                DockingManager.SetDesiredHeightInDockedMode(element, desiredHeight);
            }
        }

        /// <summary>
        /// Adds the handler for animation.
        /// </summary>
        private void AddHandlerForAnimation()
        {
            MainHost mainHost = TemplatedParent as MainHost;

            if (null != mainHost)
            {
                m_owner = mainHost.Owner;

                //// if( AutoHideAnimationMode.Slide == Owner.AutoHideAnimationMode )
                DoubleAnimation hideAnimation = null;

                switch (PanelSide)
                {
                    case Dock.Left:
                        hideAnimation = FindName(LeftHideAnimationName) as DoubleAnimation;
                        break;
                    case Dock.Right:
                        hideAnimation = FindName(RightHideAnimationName) as DoubleAnimation;
                        break;
                    case Dock.Top:
                        hideAnimation = FindName(TopHideAnimationName) as DoubleAnimation;
                        break;
                    case Dock.Bottom:
                        hideAnimation = FindName(BottomHideAnimationName) as DoubleAnimation;
                        break;
                }

                if (null != hideAnimation)
                {
                    hideAnimation.Completed -= new EventHandler(OnHideAnimationCompleted);
                    hideAnimation.Completed += new EventHandler(OnHideAnimationCompleted);
                }
                else
                {
                    throw new NotImplementedException("Set incorrect template!");
                }

                DoubleAnimation showAnimation = null;

                switch (PanelSide)
                {
                    case Dock.Left:
                        showAnimation = FindName(LeftShowAnimationName) as DoubleAnimation;
                        break;
                    case Dock.Right:
                        showAnimation = FindName(RightShowAnimationName) as DoubleAnimation;
                        break;
                    case Dock.Top:
                        showAnimation = FindName(TopShowAnimationName) as DoubleAnimation;
                        break;
                    case Dock.Bottom:
                        showAnimation = FindName(BottomShowAnimationName) as DoubleAnimation;
                        break;
                }

                if (null != showAnimation)
                {
                    showAnimation.Completed -= new EventHandler(OnShowAnimationCompleted);
                    showAnimation.Completed += new EventHandler(OnShowAnimationCompleted);
                }
            }
        }

        /// <summary>
        /// Sets the is showed focused item.
        /// </summary>
        private void SetIsShowedFocusedItem()
        {
            if (object.Equals(InternalDataContext, m_focusedItem))
            {
                IsShowedFocusedItem = true;

                if (!IsKeyboardFocusWithin)
                {
                    Focus();
                }
            }
            else
            {
                IsShowedFocusedItem = false;
            }
        }

        /// <summary>
        /// Collapses the shadow.
        /// </summary>
        private void CollapseShadow()
        {
            if (null != m_shadow)
            {
                m_shadow.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Restores the shadow.
        /// </summary>
        private void RestoreShadow()
        {
            if (null != m_shadow && Visibility.Visible != m_shadow.Visibility)
            {
                m_shadow.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Validates the process.
        /// </summary>
        private void ValidateProcess()
        {
            if (null == SelectedItem)
            {
                ShowNewContent(Items[0], m_isShow);
            }
        }

        /// <summary>
        /// Is called when show animation was completed
        /// </summary>
        /// <param name="sender">Animation clock</param>
        /// <param name="e">The EventArgs</param>
        private void OnShowAnimationCompleted(object sender, EventArgs e)
        {
            if (AssureShowAnimationCompleted())
            {
                if (m_weakShowAnimation && Owner!=null)
                {
                    Owner.FireAutoHideAnimationStop(SelectedItem as FrameworkElement);

                    if (m_isShowing && Owner.UseInteropCompatibilityMode && ContainsHwndHost((FrameworkElement)SelectedItem))
                    {
                        FrameworkElement element = (FrameworkElement)InternalDataContext;
                        element.Visibility = Visibility.Visible;
                        Owner.ShowHwndHosts(element);
                        ClearBackground();
                    }
                }

                
                m_weakShowAnimation = true;
                if (Owner.UsePopupAutoHidePreview && m_popuppanel != null)
                {
                    m_popuppanel.AllowsTransparency = false;
                    m_popuppanel.Child.UpdateLayout();
                    m_popuppanel.UpdateLayout();
                }                
            }
        }

        /// <summary>
        /// Is called when hide animation was completed
        /// </summary>
        /// <param name="sender">Animation clock</param>
        /// <param name="e">The EventArgs</param>
        private void OnHideAnimationCompleted(object sender, EventArgs e)
        {
            if (Owner!=null && AutoHideAnimationMode.Slide == Owner.AutoHideAnimationMode)
            {
                if ((1 == ContentRenderTransformX && (Dock.Left == PanelSide || Dock.Right == PanelSide)) || (1 == ContentRenderTransformY && (Dock.Top == PanelSide || Dock.Bottom == PanelSide)))
                {
                    IsContentHiden = true;
                }
            }

            if (Owner !=null)
            {
                Owner.FireAutoHideAnimationStop(InternalDataContext as FrameworkElement);
            }

            if (m_isNewContentHideAnimation)
            {
                m_isShow = false;
                m_isNewContentHideAnimation = false;
            }

            if (Owner!=null && Owner.UseInteropCompatibilityMode && AssureHideAnimationCompleted())
            {
                ThreadStart fake = Owner.RemoveFakesForDockedElements;
                Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, fake);
            }
            m_isShowing = false;
            m_weakHideAnimation = true;

            if (Owner.UsePopupAutoHidePreview && m_popuppanel != null && m_popuppanel.IsOpen)
            {
                m_popuppanel.AllowsTransparency = false;                
                m_popuppanel.IsOpen = false;
            }
        }

        /// <summary>
        /// Draws the fake for win form host.
        /// </summary>
        /// <param name="element">The element.</param>
        private void DrawFakeForWinFormHost(FrameworkElement element)
        {
            if (ContainsHwndHost(element))
            {
                DockingManager owner = DockingManager.ResolveManager(element);

                if (owner != null)
                {
                    Border border = (Border)GetTemplateChild("Content");
                    Brush brush = element.GetValue(SidePanel.ElementFakeProperty) as Brush;
                    border.Background = brush;
                    element.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                ClearBackground();
            }

            ThreadStart method = delegate
            {
                RaiseShowEvent();
            };

            Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, method);
        }

        /// <summary>
        /// Clears the background.
        /// </summary>
        private void ClearBackground()
        {
            Border border = (Border)GetTemplateChild("Content");

            if (border != null)
            {
                border.Background = null;
            }
        }

        /// <summary>
        /// Checks the selected item.
        /// </summary>
        internal void CheckSelectedItem()
        {
            if (Owner !=null && Owner.UseInteropCompatibilityMode)
            {
                ThreadStart method = delegate
                {
                    FrameworkElement element = DataContext as FrameworkElement;

                    if (element != null && ContainsHwndHost(element))
                    {
                        element.Visibility = Visibility.Collapsed;
                    }
                };

                Dispatcher.BeginInvoke(DispatcherPriority.Loaded, method);
            }
        }

        /// <summary>
        /// Saves the win form host fake.
        /// </summary>
        private void SaveWinFormHostFake()
        {
            if (Owner!=null && Owner.UseInteropCompatibilityMode)
            {
                ThreadStart fake = SaveWinFormHostFakeInternal;
                Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, fake);
            }
        }

        /// <summary>
        /// Saves the win form host fake internal.
        /// </summary>
        private void SaveWinFormHostFakeInternal()
        {
            FrameworkElement element = InternalDataContext as FrameworkElement;

            if (ContainsHwndHost(element))
            {
                if (element.Visibility == Visibility.Visible && element.IsLoaded)
                {
                    DockingManager owner = DockingManager.ResolveManager(element);

                    if (owner != null)
                    {
                        owner.ShowHwndHosts(element);

                        ThreadStart fake = SaveNewWinFormHostFakeInternal;
                        Dispatcher.BeginInvoke(DispatcherPriority.Background, fake);
                    }
                }
            }
            else
            {
                ClearBackground();

                ThreadStart method = delegate
                {
                    RaiseHideEvent();
                };

                Dispatcher.BeginInvoke(DispatcherPriority.Background, method);
            }
        }

        /// <summary>
        /// Saves the new win form host fake internal.
        /// </summary>
        private void SaveNewWinFormHostFakeInternal()
        {
            FrameworkElement element = InternalDataContext as FrameworkElement;

            DockingManager owner = DockingManager.ResolveManager(element);

            if (owner != null && element.Visibility == Visibility.Visible)
            {
                Border border = GetTemplateChild("Content") as Border;

                bool bHasVisaulParent = true;

                if (VisualTreeHelper.GetParent(element) == null)
                {
                    AddVisualChild(element);
                    bHasVisaulParent = false;
                }

                DrawingBrush brush = DrawingUtils.PrepareFake(element);

                if (!bHasVisaulParent)
                {
                    RemoveVisualChild(element);
                }

                element.SetValue(SidePanel.ElementFakeProperty, brush);
                element.Visibility = Visibility.Collapsed;

                if (border != null)
                {
                    border.Background = brush;
                }

                ThreadStart method = delegate
                {
                    RaiseHideEvent();
                };

                Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, method);
            }
        }

        /// <summary>
        /// Determines whether [is not header button] [the specified o source].
        /// </summary>
        /// <param name="oSource">The o source.</param>
        /// <param name="buttonName">Name of the button.</param>
        /// <returns>
        /// <c>true</c> if [is not header button] [the specified o source]; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsNotHeaderButton(FrameworkElement oSource, string buttonName)
        {
            bool result = true;
            FrameworkElement tParetn = (FrameworkElement)oSource.TemplatedParent;

            if (null != tParetn && tParetn is ToggleButton && buttonName == tParetn.Name)
            {
                result = false;
            }

            return result;
        }

        /// <summary>
        /// Determines whether [is cursor over direct tab panel] [the specified obj].
        /// </summary>
        /// <param name="obj">The obj value.</param>
        /// <returns>
        /// <c>true</c> if [is cursor over direct tab panel] [the specified obj]; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsCursorOverDirectTabPanel(object obj)
        {
            bool result = false;

            if (obj is Border)
            {
                result = (obj as Border).Child is DirectTabPanel;
            }

            return result;
        }

        /// <summary>
        /// Hides the content.
        /// </summary>
        internal void HideContent()
        {
            if (!m_inDragMode)
            {
                FireHide();
                IsContentHiden = true;
                m_isShow = false;
            }
        }

        /// <summary>
        /// Raises the show event.
        /// </summary>
        private void RaiseShowEvent()
        {
            if (Owner != null)
            {
                Owner.FireAutoHideAnimationStart(InternalDataContext as FrameworkElement);
            }
            RoutedEventArgs args = new RoutedEventArgs(ShowEvent);
            RaiseEvent(args);
        }

        /// <summary>
        /// Raises the hide event.
        /// </summary>
        private void RaiseHideEvent()
        {
            if (Owner != null)
            {
                Owner.FireAutoHideAnimationStart(InternalDataContext as FrameworkElement);
            }
            RoutedEventArgs args = new RoutedEventArgs(HideEvent);
            RaiseEvent(args);
            lastselecteditem = (InternalDataContext as FrameworkElement);
        }

        /// <summary>
        /// Determines whether [has group reference] [the specified id].
        /// </summary>
        /// <param name="id">The id value.</param>
        /// <returns>
        /// <c>true</c> if [has group reference] [the specified id]; otherwise, <c>false</c>.
        /// </returns>
        private bool HasGroupReference(int id)
        {
            bool bHasGroupReference = false;

            try
            {
                foreach (FrameworkElement item in TabChildren)
                {
                    if (DockingManager.GetTabbedHost(item, DockState.Dock).GetHashCode() == id)
                    {
                        bHasGroupReference = true;
                        break;
                    }
                }
            }
            catch { }

            return bHasGroupReference;
        }

        /// <summary>
        /// Sets the index of the tab group.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="groupID">The group ID.</param>
        private void SetTabGroupIndex(FrameworkElement element, int groupID)
        {
            int iPos = groups.IndexOf(groupID);
            int iIndex = DockedElementTabbedHost.GetTabOrderInDockMode(element);

            iIndex = iPos * 1024 + iIndex;
            SidePanel.SetTabChildOrder(element, iIndex);
        }

        /// <summary>
        /// Updates the tab group owner.
        /// </summary>
        /// <param name="element">The element.</param>
        private void UpdateTabGroupOwner(FrameworkElement element)
        {
            DockedElementTabbedHost host=DockingManager.GetTabbedHost(element, DockState.Dock);
            if (host != null)
            {
                int groupID = host.GetHashCode();
                List<FrameworkElement> group = GetGroup(groupID);
                FrameworkElement groupOwner = null;
                int iTabGroupOwnerIndex = int.MaxValue;

                foreach (FrameworkElement item in group)
                {
                    int iTabChildOrder = SidePanel.GetTabChildOrder(item);

                    if (iTabChildOrder < iTabGroupOwnerIndex)
                    {
                        iTabGroupOwnerIndex = iTabChildOrder;
                        groupOwner = item;
                    }

                    SidePanel.SetIsTabGroupOwner(item, false);
                }

                if (groupOwner != null)
                {
                    SidePanel.SetIsTabGroupOwner(groupOwner, true);
                }
            }
        }

        /// <summary>
        /// Adds the element internal.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="update">if set to <c>true</c> [update].</param>
        private void AddElementInternal(FrameworkElement element, bool update)
        {
            if (DockingManager.GetTabbedHost(element, DockState.Dock) != null)
            {
                int iGroupID = DockingManager.GetTabbedHost(element, DockState.Dock).GetHashCode();

                if (!groups.Contains(iGroupID))
                {
                    groups.Add(iGroupID);
                }

                SidePanel.SetTabGroupName(element, iGroupID.ToString());

                if (update)
                {
                    SetTabGroupIndex(element, iGroupID);
                }
            }
        }

        /// <summary>
        /// Removes the element internal.
        /// </summary>
        /// <param name="element">The element.</param>
        private void RemoveElementInternal(FrameworkElement element)
        {
            DockedElementTabbedHost host = DockingManager.GetTabbedHost(element, DockState.Dock);
            if (host != null)
            {
                int iGroupID = DockingManager.GetTabbedHost(element, DockState.Dock).GetHashCode();
                int iGroupIndex = groups.IndexOf(iGroupID);
                m_isShow = false;

                SidePanel.SetTabChildOrder(element, 0);
                SidePanel.SetIsTabGroupOwner(element, false);

                if (!HasGroupReference(iGroupID))
                {
                    for (int i = iGroupIndex + 1; i < groups.Count; i++)
                    {
                        foreach (FrameworkElement item in GetGroup(groups[i]))
                        {
                            int index = SidePanel.GetTabChildOrder(item);
                            index = (i - 1) * 1024 + index % 1024;
                            SidePanel.SetTabChildOrder(item, index);
                        }
                    }

                    groups.Remove(iGroupID);
                }
            }
        }

        private Storyboard ShowPopupAnimation()
        {
            Storyboard sb = new Storyboard();
            bool isHorizontal=PanelSide == Dock.Left || PanelSide == Dock.Right;

            if (AutoHideAnimationMode == AutoHideAnimationMode.Fade)
            {
                DoubleAnimationUsingKeyFrames FadeAnimation = new DoubleAnimationUsingKeyFrames();
                sb.Children.Add(FadeAnimation);
                Storyboard.SetTarget(FadeAnimation, m_popuppanel.Child);
                Storyboard.SetTargetProperty(FadeAnimation, new PropertyPath("(UIElement.Opacity)"));
                SplineDoubleKeyFrame DoubleKeyFrame1 = new SplineDoubleKeyFrame();
                DoubleKeyFrame1.KeyTime = KeyTime.FromTimeSpan(new TimeSpan(0, 0, 0));
                DoubleKeyFrame1.Value = 0.0;
                FadeAnimation.KeyFrames.Add(DoubleKeyFrame1);
                SplineDoubleKeyFrame DoubleKeyFrame2 = new SplineDoubleKeyFrame();
                DoubleKeyFrame2.KeyTime = KeyTime.FromTimeSpan(DockingManager.GetAnimationDelay(this).TimeSpan);
                DoubleKeyFrame2.Value = 1;
                FadeAnimation.KeyFrames.Add(DoubleKeyFrame2);                
            }
            else if (AutoHideAnimationMode == AutoHideAnimationMode.Scale)
            {                
                ScaleTransform scale = new ScaleTransform();
                Point p=new Point();
                if (isHorizontal)
                {
                    scale.ScaleX = 0.5;
                    p = PanelSide == Dock.Right ? new Point(1, -1) : new Point(-1, 0);
                }
                else
                {
                    scale.ScaleY = 0.5;
                    p = PanelSide == Dock.Bottom ? new Point(0, 1) : new Point(1, -1);
                }

                TransformGroup transformgp = new TransformGroup();
                transformgp.Children.Add(scale);
                this.m_popuppanel.Child.RenderTransform = transformgp;
                this.m_popuppanel.Child.RenderTransformOrigin = p;

                string path = isHorizontal ? "(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleX)" : "(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)";

                DoubleAnimationUsingKeyFrames ScaleAnimation = new DoubleAnimationUsingKeyFrames();
                sb.Children.Add(ScaleAnimation);
                Storyboard.SetTarget(ScaleAnimation, this.m_popuppanel.Child);
                Storyboard.SetTargetProperty(ScaleAnimation, new System.Windows.PropertyPath(path));
                SplineDoubleKeyFrame DoubleKeyFrame3 = new SplineDoubleKeyFrame();
                DoubleKeyFrame3.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0));
                DoubleKeyFrame3.Value = 0.5;
                ScaleAnimation.KeyFrames.Add(DoubleKeyFrame3);
                SplineDoubleKeyFrame DoubleKeyFrame4 = new SplineDoubleKeyFrame();
                DoubleKeyFrame4.KeyTime = KeyTime.FromTimeSpan(DockingManager.GetAnimationDelay(this).TimeSpan);
                DoubleKeyFrame4.Value = 1;
                ScaleAnimation.KeyFrames.Add(DoubleKeyFrame4);
            }
            else
            {
                string path = isHorizontal ? "HorizontalOffset" : "VerticalOffset";
                bool isreverseflow = PanelSide == Dock.Right || PanelSide == Dock.Bottom;
                DoubleAnimation SlideAnimation = new DoubleAnimation();
                sb.Children.Add(SlideAnimation);
                Storyboard.SetTarget(SlideAnimation, this.m_popuppanel);
                Storyboard.SetTargetProperty(SlideAnimation, new System.Windows.PropertyPath(path));
                SlideAnimation.BeginTime = new TimeSpan(0, 0, 0);
                SlideAnimation.Duration = DockingManager.GetAnimationDelay(this).TimeSpan;
                SetShowAnimationDuration(isHorizontal, SlideAnimation,this);
            }
            return sb;
        }

        private Storyboard HidePopupAnimation()
        {
            Storyboard sb = new Storyboard();
            bool isHorizontal = PanelSide == Dock.Left || PanelSide == Dock.Right;

            if (AutoHideAnimationMode == AutoHideAnimationMode.Fade)
            {
                DoubleAnimationUsingKeyFrames FadeAnimation = new DoubleAnimationUsingKeyFrames();
                sb.Children.Add(FadeAnimation);
                Storyboard.SetTarget(FadeAnimation, m_popuppanel.Child);
                Storyboard.SetTargetProperty(FadeAnimation, new PropertyPath("(UIElement.Opacity)"));
                SplineDoubleKeyFrame DoubleKeyFrame1 = new SplineDoubleKeyFrame();
                DoubleKeyFrame1.KeyTime = KeyTime.FromTimeSpan(new TimeSpan(0, 0, 0));
                DoubleKeyFrame1.Value = 1;
                FadeAnimation.KeyFrames.Add(DoubleKeyFrame1);
                SplineDoubleKeyFrame DoubleKeyFrame2 = new SplineDoubleKeyFrame();
                DoubleKeyFrame2.KeyTime = KeyTime.FromTimeSpan(DockingManager.GetAnimationDelay(this).TimeSpan);
                DoubleKeyFrame2.Value = 0.0;
                FadeAnimation.KeyFrames.Add(DoubleKeyFrame2);
            }
            else if (AutoHideAnimationMode == AutoHideAnimationMode.Scale)
            {
                ScaleTransform scale = new ScaleTransform();
                Point p = new Point();
                if (isHorizontal)
                    p = PanelSide == Dock.Right ? new Point(1, 0) : new Point(0, 1);
                else                
                    p = PanelSide == Dock.Bottom ? new Point(1, 1) : p;                
                
                TransformGroup transformgp = new TransformGroup();
                transformgp.Children.Add(scale);
                this.m_popuppanel.Child.RenderTransform = transformgp;
                this.m_popuppanel.Child.RenderTransformOrigin = p;

                string path = isHorizontal ? "(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleX)" : "(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)";

                DoubleAnimationUsingKeyFrames ScaleAnimation = new DoubleAnimationUsingKeyFrames();
                sb.Children.Add(ScaleAnimation);
                Storyboard.SetTarget(ScaleAnimation, this.m_popuppanel.Child);
                Storyboard.SetTargetProperty(ScaleAnimation, new System.Windows.PropertyPath(path));
                SplineDoubleKeyFrame DoubleKeyFrame3 = new SplineDoubleKeyFrame();
                DoubleKeyFrame3.KeyTime = KeyTime.FromTimeSpan(new TimeSpan(0, 0, 0));
                DoubleKeyFrame3.Value = 1;
                ScaleAnimation.KeyFrames.Add(DoubleKeyFrame3);
                SplineDoubleKeyFrame DoubleKeyFrame4 = new SplineDoubleKeyFrame();
                DoubleKeyFrame4.KeyTime = KeyTime.FromTimeSpan(DockingManager.GetAnimationDelay(this).TimeSpan);
                DoubleKeyFrame4.Value = 0.5;
                ScaleAnimation.KeyFrames.Add(DoubleKeyFrame4);                
            }
            else
            {                
                string path = isHorizontal ? "HorizontalOffset" : "VerticalOffset";               
                DoubleAnimation SlideAnimation = new DoubleAnimation();
                sb.Children.Add(SlideAnimation);
                Storyboard.SetTarget(SlideAnimation, this.m_popuppanel);
                Storyboard.SetTargetProperty(SlideAnimation, new System.Windows.PropertyPath(path));
                SlideAnimation.BeginTime = new TimeSpan(0, 0, 0);
                SlideAnimation.Duration = DockingManager.GetAnimationDelay(this).TimeSpan;
                SetHideAnimationDuration(isHorizontal, SlideAnimation,this);
            }
            return sb;
        }

        private void SetShowAnimationDuration(bool ishorizontal, DoubleAnimation Animation,FrameworkElement Element)
        {
            if (ishorizontal && PanelSide == Dock.Right)
            {
                Animation.To = 0.0;
                Animation.From = Element.RenderSize.Width;
            }
            else if (!ishorizontal && PanelSide == Dock.Bottom)
            {
                Animation.To = 0.0;
                Animation.From = Element.RenderSize.Height;
            }
            else
            {
                Animation.From = 0.0;
                Animation.To = ishorizontal ? Element.RenderSize.Width : Element.RenderSize.Height;
            }
        }

        private void SetHideAnimationDuration(bool ishorizontal, DoubleAnimation Animation,FrameworkElement Element)
        {
            if (ishorizontal && PanelSide == Dock.Right)
            {
                Animation.From = 0.0;
                Animation.To = Element.RenderSize.Width;

            }
            else if (!ishorizontal && PanelSide == Dock.Bottom)
            {
                Animation.From = 0.0;
                Animation.To = Element.RenderSize.Height;
            }
            else
            {
                Animation.To = 0.0;
                Animation.From = ishorizontal ? Element.RenderSize.Width : Element.RenderSize.Height;
            }
        }

        #endregion

        #region Dependency property
        /// <summary>
        /// Identifies ContentRenderTransformX dependency property of the SidePanel.
        /// </summary>
        public static readonly DependencyProperty ContentRenderTransformXProperty =
            DependencyProperty.Register("ContentRenderTransformX", typeof(double), typeof(SidePanel), new UIPropertyMetadata(0d));

        /// <summary>
        /// Identifies ContentRenderTransformY dependency property of the SidePanel.
        /// </summary>
        public static readonly DependencyProperty ContentRenderTransformYProperty =
            DependencyProperty.Register("ContentRenderTransformY", typeof(double), typeof(SidePanel), new UIPropertyMetadata(0d));

        /// <summary>
        /// Identifies ContentScaleX dependency property of the SidePanel.
        /// </summary>
        public static readonly DependencyProperty ContentScaleXProperty =
            DependencyProperty.Register("ContentScaleX", typeof(double), typeof(SidePanel), new UIPropertyMetadata(1d));

        /// <summary>
        /// Identifies ContentScaleY dependency property of the SidePanel.
        /// </summary>
        public static readonly DependencyProperty ContentScaleYProperty =
            DependencyProperty.Register("ContentScaleY", typeof(double), typeof(SidePanel), new UIPropertyMetadata(1d));

        /// <summary>
        /// Identifies ContentOpacity dependency property of the SidePanel.
        /// </summary>
        public static readonly DependencyProperty ContentOpacityProperty =
            DependencyProperty.Register("ContentOpacity", typeof(double), typeof(SidePanel), new UIPropertyMetadata(1d));

        /// <summary>
        /// Identifies TabChildren dependency property of the SidePanel.
        /// </summary>
        public static readonly DependencyProperty TabChildrenProperty =
            DependencyProperty.Register("TabChildren", typeof(ObservableFrameworkElements), typeof(SidePanel), new UIPropertyMetadata(null));

        /// <summary>
        /// Identifies panelSide dependency property of the SidePanel.
        /// </summary>
        public static readonly DependencyProperty PanelSideProperty =
            DependencyProperty.Register("PanelSide", typeof(Dock), typeof(SidePanel), new UIPropertyMetadata(Dock.Left));

        /// <summary>
        /// Identifies IsContentHiden dependency property of the SidePanel.
        /// </summary>
        public static readonly DependencyProperty IsContentHidenProperty =
            DependencyProperty.Register("IsContentHiden", typeof(bool), typeof(SidePanel), new UIPropertyMetadata(true));

        /// <summary>
        /// Identifies IsShowedFocusedItem dependency property of the SidePanel.
        /// </summary>
        public static readonly DependencyProperty IsShowedFocusedItemProperty =
           DependencyProperty.Register("IsShowedFocusedItem", typeof(bool), typeof(SidePanel), new UIPropertyMetadata(false));

        /// <summary>
        /// Identifies TabChildOrder attached property of the SidePanel.
        /// </summary>
        /// <remarks>
        ///  This property can be attached to a docking manager child and is used to determine the ordering
        ///  of tabs in the SidePanel. 
        ///  The default number is 0.
        /// </remarks>
        public static readonly DependencyProperty TabChildOrderProperty =
           DependencyProperty.RegisterAttached("TabChildOrder", typeof(int), typeof(SidePanel), new UIPropertyMetadata(0));

        /// <summary>
        /// Identifies TabGroupName attached property of the SidePanel.
        /// </summary>
        /// <remarks>
        /// This property can be attached to a docking manager child and is used to determine 
        /// the group name for tabbed elements in the SidePanel. SidePanel 
        /// arranges its children by groups and makes margins between them for visual differentiating.
        /// New group is created when user auto hide one element or several elements that are tabbed.
        /// </remarks>
        public static readonly DependencyProperty TabGroupNameProperty =
           DependencyProperty.RegisterAttached("TabGroupName", typeof(string), typeof(SidePanel));

        /// <summary>
        /// Identifies IsTabGroupOwner attached property of the SidePanel.
        /// </summary>
        /// <remarks>
        /// This property can be attached to a docking manager child and specifies whether element is a group owner.
        /// If it is set to true it means that element will be located first in the side panel group and will have 
        /// some separation from the previous group  for visual differentiating between them.
        /// The default value is false.
        /// </remarks>
        public static readonly DependencyProperty IsTabGroupOwnerProperty =
           DependencyProperty.RegisterAttached("IsTabGroupOwner", typeof(bool), typeof(SidePanel));

        /// <summary>
        /// Identifies IsOpenHeaderContextMenu dependency property of the SidePanel.
        /// </summary>
        private static readonly DependencyProperty IsOpenHeaderContextMenuProperty =
            DependencyProperty.Register("IsOpenHeaderContextMenu", typeof(bool), typeof(SidePanel), new UIPropertyMetadata(false));

        /// <summary>
        /// Identifies AutoHideAnimationMode dependency property of the SidePanel.
        /// </summary>
        private static readonly DependencyProperty AutoHideAnimationModeProperty =
            DependencyProperty.Register("AutoHideAnimationMode", typeof(AutoHideAnimationMode), typeof(SidePanel), new UIPropertyMetadata(AutoHideAnimationMode.Slide));

        /// <summary>
        /// Identifies ElementFake dependency property of the SidePanel.
        /// </summary>
        private static readonly DependencyProperty ElementFakeProperty =
            DependencyProperty.RegisterAttached("ElementFake", typeof(Brush), typeof(SidePanel));

        /// <summary>
        /// Identifies the Docking Manager Property
        /// </summary>
        public static readonly DependencyProperty DockingManagerProperty =
            DependencyProperty.Register("DockingManager", typeof(DockingManager), typeof(SidePanel));

        #endregion
    }
}