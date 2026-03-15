#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Globalization;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Imaging;
using System.Collections.ObjectModel;
using System.Collections;
using System.Windows.Threading;
using System.Windows.Browser;
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Main class for window Control.
    /// </summary>
    [TemplatePart(Name = "PART_Window", Type = typeof(Grid))]
    [TemplatePart(Name = "PART_CaptionBar", Type = typeof(Border))]
    [TemplatePart(Name = "PART_CaptionText", Type = typeof(TextBlock))]
    ////[TemplatePart(Name = "PART_CloseButton", Type = typeof(Button))]
    [TemplatePart(Name = "PART_ScrollContent", Type = typeof(ScrollViewer))]
    [TemplatePart(Name = "PART_ContentPresenter", Type = typeof(Grid))]
    [TemplatePart(Name = "PART_Dock", Type = typeof(ToggleButton))] ////Docking Pin Button
    [TemplatePart(Name = "PART_OptionsButton", Type = typeof(ToggleButton))] ////PART_OptionsButton
    [TemplatePart(Name = "PART_optionsPopUp", Type = typeof(Popup))]////PART_optionsPopUp
    [TemplatePart(Name = "PART_LeftButtonPopUp", Type = typeof(Popup))]////PART_LeftButtonPopUp
    [TemplatePart(Name = "PART_RightButtonPopUp", Type = typeof(Popup))]////PART_RightButtonPopUp
    [TemplatePart(Name = "PART_BottomButtonPopUp", Type = typeof(Popup))]////PART_BottomButtonPopUp
    [TemplatePart(Name = "PART_TopButtonPopUp", Type = typeof(Popup))]////PART_TopButtonPopUp
    [TemplatePart(Name = "PART_CenterPopUp", Type = typeof(Popup))]////PART_CenterPopUp
    [TemplatePart(Name = "PART_InnerGrid", Type = typeof(Grid))]////PART_InnerGrid
    [TemplatePart(Name = "PART_LeftShadowPopUp", Type = typeof(Popup))]////PART_LeftShadowPopUp
    [TemplatePart(Name = "PART_RightShadowPopUp", Type = typeof(Popup))]////PART_RightShadowPopUp
    [TemplatePart(Name = "PART_BottomShadowPopUp", Type = typeof(Popup))]////PART_BottomShadowPopUp
    [TemplatePart(Name = "PART_TopShadowPopUp", Type = typeof(Popup))]////PART_TopShadowPopUp
    [TemplatePart(Name = "PART_CenterShadowPopUp", Type = typeof(Popup))]////PART_CenterShadowPopUp
    [TemplatePart(Name = "PART_CloseButton", Type = typeof(ToggleButton))]//
    [TemplatePart(Name = "PART_Caption", Type = typeof(ContentControl))]//PART_Caption
    [TemplatePart(Name = "Part_popUpCollection", Type = typeof(Grid))]////Part_popUpCollection

    public partial class Window : ContentControl, IDisposable
    {
        /// <summary>
        /// Represents the contextMenu.
        /// </summary>
        protected internal ContextMenuAdv contextMenu = null;

        /// <summary>
        /// Represents the floatContextMenuItemAdv.
        /// </summary>
        protected internal ContextMenuItemAdv floatContextMenuItemAdv = null;

        /// <summary>
        /// Represents the dockContextMenuItemAdv.
        /// </summary>
        protected internal ContextMenuItemAdv dockContextMenuItemAdv = null;

        /// <summary>
        /// Represents the autoHideContextMenuItemAdv.
        /// </summary>
        protected internal ContextMenuItemAdv autoHideContextMenuItemAdv = null;

        /// <summary>
        /// Represents the hideContextMenuItemAdv.
        /// </summary>
        protected internal ContextMenuItemAdv hideContextMenuItemAdv = null;

        /// <summary>
        /// Represents the containerInvoke.
        /// </summary>
        protected internal bool containerInvoke = false;

        /// <summary>
        /// Represents the _headerEventFired.
        /// </summary>
        protected internal bool _headerEventFired = false;

        /// <summary>
        /// Represents the contentBorder.
        /// </summary>
        protected internal Border contentBorder = null;

        /// <summary>
        /// 
        /// </summary>
        protected internal ToggleButton maximizeButton = null;

        /// <summary>
        /// Gets or sets the selected tab item.
        /// </summary>
        /// <value>The selected tab item.</value>
        protected internal string SelectedTabItem
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the name of the move window target.
        /// </summary>
        /// <value>The name of the move window target.</value>
        protected internal string MoveWindowTargetName
        {
            get;
            set;
        }

        internal bool setWhileRestoring = false;

        // protected internal int Order = 0;

        /// <summary>
        /// Gets or sets the move dock position.
        /// </summary>
        /// <value>The move dock position.</value>
        protected internal Dock MoveDockPosition
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the dock tab order.
        /// </summary>
        /// <value>The dock tab order.</value>
        protected internal int DockTabOrder
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the float tab order.
        /// </summary>
        /// <value>The float tab order.</value>
        protected internal int FloatTabOrder
        {
            get;
            set;
        }

        /// <summary>
        /// Represents the temp grid.
        /// </summary>
        protected internal Grid tempGrid = null;


        /// <summary>
        /// Gets or sets the width of the pane.
        /// </summary>
        /// <value>The width of the pane.</value>
        public double PaneWidth
        {
            get
            {
                return m_PaneWidth;
            }
            set
            {
                m_PaneWidth = value;
            }
        }

        /// <summary>
        /// The width of the pane.
        /// </summary>
        public double m_PaneWidth = 80;


        /// <summary>
        /// Gets or sets the height of the pane.
        /// </summary>
        /// <value>The height of the pane.</value>
        public double PaneHeight
        {
            get
            {
                return m_PaneHeight;
            }
            set
            {
                m_PaneHeight = value;
            }
        }

        /// <summary>
        /// Represents the Pane Height.
        /// </summary>
        public double m_PaneHeight = 80;

        /// <summary>
        /// Represents the Caption.
        /// </summary>
        protected internal ContentControl caption = null;

        /// <summary>
        /// Stores the CaptionBar From Generic.xaml.
        /// Caption bar is in the form of Border.
        /// </summary>
        protected internal FrameworkElement captionBar = null;

        /// <summary>
        /// Stores the Grid and Getting from the Generic.Xaml file.
        /// </summary>
        protected internal Grid window = null;

        /// <summary>
        /// Stores the ContentGrid and Getting from the Generic.xaml file.
        /// </summary>
        protected internal Grid ContentGrid = null;

        /// <summary>
        /// Stores the Context Menu.
        /// </summary>
        protected internal Popup optionsPopUp = null;

        /// <summary>
        /// Stores the Content and getting from the Generic.xaml file.
        /// </summary>
        protected internal Grid innerGrid = null;

        /// <summary>
        /// Stores the PopUp for Leftside and getting from the Generic.xaml file.
        /// </summary>
        protected internal Popup leftSidePopUp = null;

        /// <summary>
        /// Stores the popup for rightSide and Gettting from the Generic.xaml file.
        /// </summary>
        protected internal Popup rightSidePopUp = null;

        /// <summary>
        /// Stores the popup for topSide and Getting from the Generic.xaml file.
        /// </summary>
        protected internal Popup topSidePopUp = null;

        /// <summary>
        /// Stores the popup for bottomSideGrid and Getting from the generic.xaml file
        /// </summary>
        protected internal Popup bottomSidePopUp = null;

        /// <summary>
        /// Stores the popup for CenterPopUP and ots getting from the Genric.xaml file.
        /// </summary>
        protected internal Popup centerPopUp = null;

        /// <summary>
        /// Stores the PopupCollection grid from Generic.xaml file.
        /// </summary>
        protected internal Grid popupCollectionGrid = null;

        /// <summary>
        /// Stores the popup for LeftSide from Generic.xaml file.
        /// </summary>
        protected internal Popup leftshadowPopUp = null;

        /// <summary>
        /// Stores the Popup for RightSide from the Generic.xaml file.
        /// </summary>
        protected internal Popup rightshadowPopUp = null;

        /// <summary>
        /// Stores the Popup for Top Side from the Generic.xaml file.
        /// </summary>
        protected internal Popup topshadowPopUp = null;

        /// <summary>
        /// Stores the PopUp for Bottom side from the Generic.xaml file.
        /// </summary>
        protected internal Popup bottomshadowPopUp = null;

        /// <summary>
        /// Stores the PopUp for CenterShdow from the Generic.xaml file.
        /// </summary>
        protected internal Popup centershadowPopUp = null;

        /// <summary>
        /// Stores the CloseButton template from the Generic.xaml file.
        /// </summary>
        protected internal ToggleButton closeButton = null;

        /// <summary>
        /// Stores the DockButton ie AWLbutton template from the Generic.xaml file.
        /// </summary>
        protected internal ToggleButton dockToggle = null;

        /// <summary>
        /// Stores the tabbedPanel from the Generic.xaml file.
        /// </summary>
        private StackPanel tabbedPanel = null;

        /// <summary>
        /// Stores the CaptionText from the Generic.xaml file.
        /// </summary>
        protected internal TextBlock captionText;

        /// <summary>
        /// Stores the Scrollviewer.
        /// </summary>
        protected internal ScrollViewer scrollcontent = null;

        /// <summary>
        /// Stores the Options Button for context menu.its getting from the Generic.xaml file.
        /// </summary>
        protected internal ToggleButton optionsButton = null;

        /// <summary>
        /// Stores the Grid from the Generic.xaml file
        /// </summary>
        protected internal Grid contentpresenter = null;

        /// <summary>
        /// Stores the double value for Offset
        /// </summary>
        protected internal double innerContentPresenterOffset = -1;

        /// <summary>
        /// Stores the BooleanValue for PopupLoaded
        /// </summary>
        protected internal bool popupLoaded = false;

        /// <summary>
        /// Stores the Rectangle.
        /// </summary>
        private Rectangle outerRectangle = null;

        /// <summary>
        /// Stores the CloseButtonpath from Generic.xaml file
        /// </summary>
        private Path outerpath = null;

        /// <summary>
        /// Stores the InnerPath from Genric.xaml file
        /// </summary>
        private Path innerpath = null;

        /// <summary>
        /// Stores the dispatchtimer for Double click and Animation
        /// </summary>
        private DispatcherTimer timer, _timer = null;

        /// <summary>
        /// Stores the CustomTabControl for ContentHolder
        /// </summary>
        protected internal CustomTabControl contentHolder = null;

        /// <summary>
        /// Identifies whether internally raised or not.
        /// </summary>
        private bool _internallyRaised = false;

        private bool pinnedCancelled = false;

        private bool unPinnedCancelled = false;

        private bool menuItemAutoHide = false;

        private bool intenallyChangedMaximizedState = false;

        internal Point dragPoint;

        /// <summary>
        /// Gets or sets a value indicating whether [internallly raised dock state changed].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [internallly raised dock state changed]; otherwise, <c>false</c>.
        /// </value>
        protected internal bool InternalllyRaisedDockStateChanged
        {
            get
            {
                return _internallyRaised;
            }
            set
            {
                _internallyRaised = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected internal bool InternallyRaisedMaximizedStateChanged
        {
            get
            {
               return intenallyChangedMaximizedState;
            }
            set
            {
                intenallyChangedMaximizedState = value;
            }
        }

        bool internallyChecked = false;

        internal bool InternallyChecked
        {
            get
            {
                return internallyChecked;
            }
            set
            {
                internallyChecked = value;
            }
        }

        /// <summary>
        /// Gets or sets the current state main.
        /// </summary>
        /// <value>The current state main.</value>
        protected internal StateMaintanance CurrentStateMain
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the previous state main.
        /// </summary>
        /// <value>The previous state main.</value>
        protected internal StateMaintanance PreviousStateMain
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets  the DockState.
        /// </summary>
        /// <value>The state of the previous.</value>
        protected internal DockState PreviousState
        {
            get;
            set;
        }


        DockableState m_dock;
        /// <summary>
        /// Gets or sets the state of the dockable.
        /// </summary>
        /// <value>The state of the dockable.</value>
        protected internal DockableState DockableState
        {
            get
            {
                return m_dock;
            }

            set
            {
                m_dock = value;
            }
        }

        /// <summary>
        /// Identifies whether removed or not.
        /// </summary>
        protected internal bool isRemoved = false;

        /// <summary>
        /// Gets or sets the Collection of CustomTab Items.
        /// </summary>
        /// <value>The target name collection.</value>
        protected internal List<string> TargetNameCollection
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the float window target name collection.
        /// </summary>
        /// <value>The float window target name collection.</value>
        protected internal List<CustomTabItem> FloatWindowTargetNameCollection
        {
            get;
            set;
        }

        /// <summary>
        /// Represents the float window target name.
        /// </summary>
        protected internal string floatWindowTargetName;

        /// <summary>
        /// Gets or sets the Destination Window
        /// </summary>
        /// <value>The stored move to window.</value>
        protected internal Window StoredMoveToWindow
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the DockPosition of the Destination Window
        /// </summary>
        /// <value>The movet to dock position.</value>
        protected internal Dock MovetToDockPosition
        {
            get;
            set;
        }


        /// <summary>
        /// Gets or sets the state of the previous dock.
        /// </summary>
        /// <value>The state of the previous dock.</value>
        protected internal DockState PreviousDockState
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets DockSide.
        /// </summary>
        /// <value>The previous dock side.</value>
        protected internal Dock PreviousDockSide
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the desired width in docked mode.
        /// </summary>
        /// <value>The desired width in docked mode.</value>
        protected internal double DesiredWidthInDockedMode
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the desired height in docked mode.
        /// </summary>
        /// <value>The desired height in docked mode.</value>
        protected internal double DesiredHeightInDockedMode
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the desired width in float mode.
        /// </summary>
        /// <value>The desired width in float mode.</value>
        protected internal double DesiredWidthInFloatMode
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the desired height in float mode.
        /// </summary>
        /// <value>The desired height in float mode.</value>
        protected internal double DesiredHeightInFloatMode
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the zindex order.
        /// </summary>
        /// <value>The zindex order.</value>
        protected internal int ZindexOrder
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the container left.
        /// </summary>
        /// <value>The container left.</value>
        protected internal double ContainerLeft
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the container top.
        /// </summary>
        /// <value>The container top.</value>
        protected internal double ContainerTop
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the height of the container.
        /// </summary>
        /// <value>The height of the container.</value>
        protected internal double ContainerHeight
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the width of the container.
        /// </summary>
        /// <value>The width of the container.</value>
        protected internal double ContainerWidth
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the Dockingmanager.
        /// </summary>
        /// <value>The docking manager.</value>
        protected internal DockingManager DockingManager
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the LeftPosition of the Float Window.
        /// </summary>
        private double m_leftposition;

        /// <summary>
        /// Gets or sets the left position.
        /// </summary>
        /// <value>The left position.</value>
        protected internal double LeftPosition
        {
            get
            {
                if (this.DockingManager != null)
                {
                    if (this.DockingManager.ActualWidth <= m_leftposition && this.DockingManager.ActualWidth != 0.0 && m_leftposition > 0)
                    {
                        m_leftposition = this.DockingManager.ActualWidth - 70;

                    }
                }
                return m_leftposition;
            }
            set
            {
                m_leftposition = value;
            }

        }

        /// <summary>
        /// Gets or sets the TopPosition of the Float window .
        /// </summary>
        private double m_topposition;

        /// <summary>
        /// Gets or sets the top position.
        /// </summary>
        /// <value>The top position.</value>
        protected internal double TopPosition
        {
            get
            {
                if (this.DockingManager != null)
                {
                    if (this.DockingManager.ActualHeight <= m_topposition && this.DockingManager.ActualHeight != 0.0 && m_topposition > 0)
                    {
                        m_topposition = this.DockingManager.ActualHeight - 70;

                    }
                }
                return m_topposition;
            }
            set
            {
                m_topposition = value;
            }
        }

        /// <summary>
        /// Gets or sets the TargetName of the Tabbed Window .
        /// </summary>
        /// <value>The target name in docked mode.</value>
        protected internal string TargetNameInDockedMode
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the TargetName of the Tabbed Window .
        /// </summary>
        /// <value>The target name in float mode.</value>
        protected internal string TargetNameInFloatMode
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Parent Window.
        /// </summary>
        /// <value>The parent window.</value>
        protected internal Window ParentWindow
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is parent contaniner.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is parent contaniner; otherwise, <c>false</c>.
        /// </value>
        protected internal bool IsParentContaniner
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Left difference between the Grid .
        /// </summary>
        /// <value>The left diff.</value>
        protected internal double LeftDiff
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets  the Topdifference between the Grid.
        /// </summary>
        /// <value>The top diff.</value>
        protected internal double TopDiff
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Collection of the  Window.
        /// </summary>
        /// <value>The window collection.</value>
        protected internal List<Window> WindowCollection
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Collection of the  Window.
        /// </summary>
        /// <value>The duplicate window collection.</value>
        protected internal List<Window> DuplicateWindowCollection
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Height for the Float Window.
        /// </summary>
        /// <value>The height of the float.</value>
        protected internal double FloatHeight
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the width of the float window.
        /// </summary>
        /// <value>The width of the float.</value>
        protected internal double FloatWidth
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Animation Width for the Float window.
        /// </summary>
        /// <value>The width of the animation.</value>
        protected internal double AnimationWidth
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Animation height of the Float Window.
        /// </summary>
        /// <value>The height of the animation.</value>
        protected internal double AnimationHeight
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Parent window of the Tabbed Window.
        /// </summary>
        /// <value>The parent tab window.</value>
        protected internal Window ParentTabWindow
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the height of the Docked Window .
        /// </summary>
        /// <value>The height of the dock.</value>
        protected internal double DockHeight
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Width of the Docked Window .
        /// </summary>
        /// <value>The width of the dock.</value>
        protected internal double DockWidth
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether [already existing].
        /// </summary>
        /// <value><c>true</c> if [already existing]; otherwise, <c>false</c>.</value>
        protected internal bool AlreadyExisting
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets a value of contextListBoxStyle .
        /// </summary>
        /// <value>The context list box style.</value>
        public Style contextListBoxStyle
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value of contextListBoxStyle .
        /// </summary>
        /// <value>The context list item style.</value>
        public Style contextListItemStyle
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether .
        /// Stores the boolean value for Window can able to dock.
        /// </summary>
        /// <value>
        /// 	<see langword="true"/> If ; otherwise, <see langword="false"/>.
        /// </value>
        public bool CanDock
        {
            get
            {
                return (bool)GetValue(CanDockProperty);
            }

            set
            {
                SetValue(CanDockProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether .
        /// Stores the boolean value for Window can able to float
        /// </summary>
        /// <value>
        /// <see langword="true"/> if ; otherwise, <see langword="false"/>.
        /// </value>
        public bool CanFloat
        {
            get
            {
                return (bool)GetValue(CanFloatProperty);
            }

            set
            {
                SetValue(CanFloatProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether .
        /// Stores the Boolean value for Window can able to Close
        /// </summary>
        /// <value>
        /// <see langword="true"/> if ; otherwise, <see langword="false"/>.
        /// </value>
        public bool CanClose
        {
            get
            {
                return (bool)GetValue(CanCloseProperty);
            }

            set
            {
                SetValue(CanCloseProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether .
        /// </summary>
        /// <value>
        /// <see langword="true"/> if ; otherwise, <see langword="false"/>.
        /// </value>
        public bool CanAutoHide
        {
            get
            {
                return (bool)GetValue(CanAutoHideProperty);
            }

            set
            {
                SetValue(CanAutoHideProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether .
        /// Stores the Boolean value for Window can able to Drag
        /// </summary>
        /// <value>
        /// <see langword="true"/> if ; otherwise, <see langword="false"/>.
        /// </value>
        public bool CanDrag
        {
            get
            {
                return (bool)GetValue(CanDragProperty);
            }

            set
            {
                SetValue(CanDragProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the Maximized state of a window
        /// </summary>
        public MaximizedState MaximizedState
        {
            get
            {
                return (MaximizedState)GetValue(MaximizedStateProperty);
            }
            set
            {
                SetValue(MaximizedStateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether .
        /// Stores the boolean value for Window is Removed from the Parent
        /// </summary>
        /// <value>
        /// <see langword="true"/> if ; otherwise, <see langword="false"/>.
        /// </value>
        protected internal bool IsremovedFromParent
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Dock State .
        /// </summary>
        public DockState DockState
        {
            get
            {
                return (DockState)GetValue(DockStateWindowProperty);
            }

            set
            {
                SetValue(DockStateWindowProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="CanDrag"/> dependency property key.
        /// </summary>
        public static readonly DependencyProperty MaximizedStateProperty =
       DependencyProperty.Register("MaximizedState", typeof(MaximizedState), typeof(Window), new PropertyMetadata(MaximizedState.Restored, OnMaximizedStatePropertyChanged));

        /// <summary>
        /// Identifies the <see cref="CanDrag"/> dependency property key.
        /// </summary>
        public static readonly DependencyProperty CanDragProperty =
       DependencyProperty.Register("CanDrag", typeof(bool), typeof(Window), new PropertyMetadata(false, OnCanDragPropertyChanged));

        /// <summary>
        /// Identifies the <see cref="CanAutoHide"/> dependency property key.
        /// </summary>
        public static readonly DependencyProperty CanAutoHideProperty =
        DependencyProperty.Register("CanAutoHide", typeof(bool), typeof(Window), new PropertyMetadata(false, OnCanAutoHidePropertyChanged));

        /// <summary>
        /// Identifies the <see cref="CanClose"/> dependency property key.
        /// </summary>
        public static readonly DependencyProperty CanCloseProperty =
         DependencyProperty.Register("CanClose", typeof(bool), typeof(Window), new PropertyMetadata(false, OnCanClosePropertyChanged));

        /// <summary>
        /// Identifies the <see cref="CanDock"/> dependency property key.
        /// </summary>
        public static readonly DependencyProperty CanDockProperty =
           DependencyProperty.Register("CanDock", typeof(bool), typeof(Window), new PropertyMetadata(false, OnCanDockPropertyChanged));

        /// <summary>
        /// Identifies the <see cref="CanFloat"/> dependency property key.
        /// </summary>
        public static readonly DependencyProperty CanFloatProperty =
         DependencyProperty.Register("CanFloat", typeof(bool), typeof(Window), new PropertyMetadata(false, OnCanFloatPropertyChanged));

        /// <summary>
        /// Identifies the <see cref="DockState"/> dependency property key.
        /// </summary>
        private static readonly DependencyProperty DockStateWindowProperty =
         DependencyProperty.Register("DockState", typeof(DockState), typeof(Window), new PropertyMetadata(DockState.Dock, OnDockStateChanged));

        /// <summary>
        /// Invoked whenever the <see cref="CanFloat"/> property is changed.
        /// </summary>
        /// <param name="d">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private static void OnCanFloatPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Window instance = (Window)d;
            instance.OnCanFloatPropertyChanged(e);
        }

        /// <summary>
        /// Invoked whenever the <see cref="CanFloat"/> property is changed.
        /// </summary>
        /// <param name="e">The source of the event.</param>        
        protected virtual void OnCanFloatPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            this.CanFloat = (bool)e.NewValue;
        }

        private static void OnMaximizedStatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Window instance = (Window)d;
            instance.OnMaximizedStatePropertyChanged(e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnMaximizedStatePropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            InternallyRaisedMaximizedStateChanged = true;
            DockingManager.SetMaximizedState(this.WindowChildElement, (MaximizedState)e.NewValue);
            InternallyRaisedMaximizedStateChanged = false;
        }

        /// <summary>
        /// Invoked whenever the <see cref="CanDock"/> property is changed.
        /// </summary>
        /// <param name="d">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private static void OnCanDockPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Window instance = (Window)d;
            instance.OnCanDockPropertyChanged(e);
        }

        /// <summary>
        /// Invoked whenever the <see cref="CanClose"/> property is changed.
        /// </summary>
        /// <param name="d">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private static void OnCanClosePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Window instance = (Window)d;
            instance.OnCanClosePropertyChanged(e);
        }

        /// <summary>
        /// Invoked whenever the <see cref="CanAutoHide"/> property is changed.
        /// </summary>
        /// <param name="d">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private static void OnCanAutoHidePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Window instance = (Window)d;
            instance.OnCanAutoHidePropertyChanged(e);
        }

        /// <summary>
        /// Invoked whenever the <see cref="CanDrag"/> property is changed.
        /// </summary>
        /// <param name="d">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private static void OnCanDragPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Window instance = (Window)d;
            instance.OnCanDragPropertyChanged(e);
        }

        /// <summary>
        /// Invoked whenever the <see cref="CanDrag"/> property is changed.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        protected virtual void OnCanDragPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            this.DraggingEnabled = (bool)e.NewValue;
            this.CanDrag = (bool)e.NewValue;
        }

        /// <summary>
        /// Invoked whenever the <see cref="CanAutoHide"/> property is changed.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        protected virtual void OnCanAutoHidePropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            this.CanAutoHide = (bool)e.NewValue;
        }

        /// <summary>
        /// Invoked whenever the <see cref="CanDock"/> property is changed.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        protected virtual void OnCanClosePropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            this.CanClose = (bool)e.NewValue;
        }

        /// <summary>
        /// Invoked whenever the <see cref="CanDock"/> property is changed.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        protected virtual void OnCanDockPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            this.CanDock = (bool)e.NewValue;
        }

        /// <summary>
        /// Invoked whenever the <see cref="DockState"/> property is changed.
        /// </summary>
        /// <param name="d">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private static void OnDockStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Window instance = (Window)d;
            instance.OnDockStateChanged(e);
        }

        /// <summary>
        /// Invoked whenever the <see cref="DockState"/> property is changed.
        /// </summary>       
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        protected virtual void OnDockStateChanged(DependencyPropertyChangedEventArgs e)
        {
            //DockingGrid dg = null;
            //    dg = this.DockingManager.GetParentDockManager();
            //    if (dg != null)
            //    {
            //        if (dg.rootWindow != null)
            //        {
            //            dg.rootWindow.ApplyDockStyle();
            //        }
            //    }
            this.InternalllyRaisedDockStateChanged = false;
            this.PreviousDockState = (DockState)e.OldValue;
            if ((DockState)e.OldValue == DockState.Hidden)
            {
                DockState dockState = PreviousState;
                if (this.PreviousState == DockState.Hidden)
                {
                    this.PreviousState = DockState.Dock;
                }
                else
                {
                    this.PreviousDockState = dockState;
                }
            }
            else
            {
                this.PreviousState = (DockState)e.OldValue;
            }
            if (DockState.Float == (DockState)e.NewValue)
            {
                if (WindowChildElement != null)
                {
                    DockingManager.SetNoHeader(WindowChildElement, false);
                }
                if (this.DockingManager != null)
                {
                    ApplyBorderForFloatWindow();
                }

                if (this.DockingManager.ActualWidth > 0 && dockToggle != null)
                {
                    if (!((bool)this.dockToggle.IsChecked))
                    {
                        SetFloatWindow();

                        //if ((this.DockManager.Children[0] as DockingGrid).gridDocking.Children.Count == 1)
                        //{
                        //    AutoHideParentWindow(this);
                        //}
                    }
                    else
                    {
                        switch (this.DockPosition)
                        {
                            case Dock.Bottom:

                                break;
                            case Dock.Top:

                                break;
                            case Dock.Left:

                                break;
                            case Dock.Right:

                                break;
                        }
                        if (this.TabbedElement != null)
                        {
                            if (this.TabbedElement.Children.Count > 0)
                            {
                                //this.DockingManager.RecentlyMouseHoveredSidePanel = (SidePanel)this.TabbedElement.Children[0];
                            }
                        }

                        this.dockToggle.IsChecked = false;
                        SetFloatWindow();
                    }
                }
                else
                {
                    Rect rect;
                    rect = this.DockingManager.Rectangle(40, 40, 300, 300);
                    this.DockingManager.SetSizeForEachChild(this, rect);
                }
                if (this.WindowChildElement != null)
                {
                    this.InternalllyRaisedDockStateChanged = true;
                    if (DockingManager.GetDockState(this.WindowChildElement) == DockState.Float)
                    {
                        this.InternalllyRaisedDockStateChanged = false;
                    }
                    DockingManager.SetDockState(this.WindowChildElement, DockState.Float);
                }
                this.ApplyBorderForFloatWindow();
            }
            else if (DockState.AutoHidden == (DockState)e.NewValue && this.CustomTabControl != null)
            {
                DockingManager.SetNoHeader(WindowChildElement, false);
                if (this.CustomTabControl.Items.Count > 0)
                {
                    this.DockingManager.ShowDockbutton(this);
                    this.DockingManager.ApplyParticularWindowStyle(this);
                    if (this.DockingManager.ActualWidth > 0 && dockToggle != null && this.Width > 0 && this.Height > 0)
                    {
                        Window _pinnedWindow = this as Window;
                        if (_pinnedWindow != null)
                        {
                            Animation _pinnedAnimationWindow = new Animation(_pinnedWindow);
                            switch (_pinnedWindow.DockPosition)
                            {
                                case Dock.Bottom:
                                    _pinnedAnimationWindow.AnimateSize(_pinnedWindow.Width, 0);
                                    _pinnedAnimationWindow.AnimatePosition(Canvas.GetLeft(_pinnedWindow), _pinnedWindow.DockingManager.ActualHeight + 1);
                                    break;

                                case Dock.Left:
                                    _pinnedAnimationWindow.AnimateSize(0, _pinnedWindow.Height);
                                    break;

                                case Dock.Right:
                                    _pinnedAnimationWindow.AnimateSize(0, _pinnedWindow.Height);
                                    _pinnedAnimationWindow.AnimatePosition(_pinnedWindow.DockingManager.ActualWidth + 1, Canvas.GetTop(_pinnedWindow));
                                    break;

                                case Dock.Top:
                                    _pinnedAnimationWindow.AnimateSize(_pinnedWindow.Width, 0);
                                    _pinnedAnimationWindow.AnimatePosition(Canvas.GetLeft(_pinnedWindow), 0);
                                    break;
                            }

                            _pinnedWindow.IsremovedFromParent = false;
                        }

                        menuItemAutoHide = true;
                        dockToggle.IsChecked = true;
                        menuItemAutoHide = false;
                    }
                    else
                    {
                        if (this.WindowDockPin == DockPin.UnPinned)
                        {
                            menuItemAutoHide = true;
                            this.Visibility = Visibility.Collapsed;
                            UIElement temp = (UIElement)VisualTreeHelper.GetParent(this);
                            this.DockingManager.GenerateSideGrid(this);
                            //this.WindowDockPin = DockPin.Pinned;
                            this.Visibility = Visibility.Collapsed;
                            if (dockToggle != null)
                            {
                                switch (this.DockPosition)
                                {
                                    case Dock.Bottom:
                                        this.CanAutoHide = true;
                                        dockToggle.IsChecked = true;
                                        break;
                                    case Dock.Left:
                                        this.CanAutoHide = true;
                                        dockToggle.IsChecked = true;
                                        break;
                                    case Dock.Right:
                                        this.CanAutoHide = true;
                                        dockToggle.IsChecked = true;
                                        break;
                                    case Dock.Top:
                                        this.CanAutoHide = true;
                                        dockToggle.IsChecked = true;
                                        break;
                                }
                            }
                            menuItemAutoHide = false;
                        }
                    }
                    if (this.WindowChildElement != null)
                    {
                        this.InternalllyRaisedDockStateChanged = true;
                        if (DockingManager.GetDockState(this.WindowChildElement) == DockState.AutoHidden)
                        {
                            this.InternalllyRaisedDockStateChanged = false;
                        }
                        DockingManager.SetDockState(this.WindowChildElement, DockState.AutoHidden);
                    }
                    foreach (CustomTabItem cstabItem in this.CustomTabControl.Items)
                    {
                        if (cstabItem.OwnWindow != null && cstabItem.OwnWindow != this && cstabItem.OwnWindow.WindowChildElement != null)
                        {
                            cstabItem.OwnWindow.InternalllyRaisedDockStateChanged = true;
                            if (DockingManager.GetDockState(cstabItem.OwnWindow.WindowChildElement) == DockState.AutoHidden)
                            {
                                cstabItem.OwnWindow.InternalllyRaisedDockStateChanged = false;
                            }
                            DockingManager.SetDockState(cstabItem.OwnWindow.WindowChildElement, DockState.AutoHidden);
                        }
                    }
                }
            }
            else if (DockState.Dock == (DockState)e.NewValue)
            {
                if (this.dockToggle != null)
                {
                    ApplyDockStyle();
                    bool isRemoved = this.IsremovedFromParent;
                    dockToggle.IsChecked = false;
                    this.IsremovedFromParent = isRemoved;
                    this.DockableState = DockableState.Dockable;
                }
                if (this.WindowChildElement != null)
                {
                    InternalllyRaisedDockStateChanged = true;
                    if (DockingManager.GetDockState(this.WindowChildElement) == DockState.Dock)
                    {
                        this.InternalllyRaisedDockStateChanged = false;
                    }
                    DockingManager.SetDockState(this.WindowChildElement, DockState.Dock);
                }
                if (this.CustomTabControl != null)
                {
                    foreach (CustomTabItem cstabItem in this.CustomTabControl.Items)
                    {
                        if (cstabItem.OwnWindow != null && cstabItem.OwnWindow != this && cstabItem.OwnWindow.WindowChildElement != null)
                        {
                            cstabItem.OwnWindow.InternalllyRaisedDockStateChanged = true;
                            if (DockingManager.GetDockState(cstabItem.OwnWindow.WindowChildElement) == DockState.Dock)
                            {
                                cstabItem.OwnWindow.InternalllyRaisedDockStateChanged = false;
                            }
                            DockingManager.SetDockState(cstabItem.OwnWindow.WindowChildElement, DockState.Dock);
                        }
                    }
                }
            }
            else if (DockState.Hidden == (DockState)e.NewValue)
            {
                if (this.DockingManager != null)
                {
                    if (this.DockingManager.FloatWindowHeaderBackground != null)
                    {
                        this.HeaderBackgroud = this.DockingManager.FloatWindowHeaderBackground;
                        this.WindowBorderBrush = this.DockingManager.FloatWindowBorderBrush;
                        this.WindowBorderThickness = this.DockingManager.FloatWindowBorderThickness;
                        this.WindowCornerRadius = this.DockingManager.FloatWindowCornerRadius;
                    }
                    if (contentBorder != null)
                    {
                        contentBorder.BorderBrush = _windowBorderBrush;
                        contentBorder.BorderThickness = new Thickness(2);
                        contentBorder.Margin = new Thickness(5, -2, 5, 5);
                    }                   
                }

                if (this.CustomTabControl != null && this.CustomTabControl.Items.Count == 1)
                {
                    //this.CustomTabControl.WindowContentBackground = this.DockingManager.FloatWindowContentBackground;
                    //this.CustomTabControl.WindowContentMargin = this.DockingManager.FloatWindowContentMargin;
                    //this.CustomTabControl.WindowContentBorderBrush = this.DockingManager.FloatWindowContentBorderBrush;
                    //this.CustomTabControl.WindowContentBorderThickness = this.DockingManager.FloatWindowContentBorderThickness;
                    //this.CustomTabControl.WindowBackground = this.DockingManager.FloatWindowBackground;
                }
                if (this.WindowChildElement != null)
                {
                    this.InternalllyRaisedDockStateChanged = true;
                    if (DockingManager.GetDockState(this.WindowChildElement) == DockState.Hidden)
                    {
                        this.InternalllyRaisedDockStateChanged = false;
                    }
                    DockingManager.SetDockState(this.WindowChildElement, DockState.Hidden);
                }
                if (this.CustomTabControl != null)
                {
                    foreach (CustomTabItem cstabItem in this.CustomTabControl.Items)
                    {
                        if (cstabItem.OwnWindow != null && cstabItem.OwnWindow != this && cstabItem.OwnWindow.WindowChildElement != null)
                        {
                            cstabItem.OwnWindow.InternalllyRaisedDockStateChanged = true;
                            if (DockingManager.GetDockState(cstabItem.OwnWindow.WindowChildElement) == DockState.Hidden)
                            {
                                cstabItem.OwnWindow.InternalllyRaisedDockStateChanged = false;
                            }
                            DockingManager.SetDockState(cstabItem.OwnWindow.WindowChildElement, DockState.Hidden);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Applies the dock style.
        /// </summary>
        protected internal void ApplyDockStyle()
        {
            if (this.captionBar != null)
            {
                if (this.DockingManager.ActiveWindow == this)
                {
                    this.HeaderBackgroud = this.DockingManager.ActiveWindowColor;
                    this.ActiveForeground = this.DockingManager.ActiveForeground;
                    this.WindowBorderBrush = this.DockingManager.WindowBorderBrush;
                    this.WindowBorderThickness = this.DockingManager.WindowBorderThickness;
                    this.WindowCornerRadius = this.DockingManager.WindowCornerRadius;
                    if (this.maximizeButton != null)
                        VisualStateManager.GoToState(this.maximizeButton, "Active", false);
                    if (this.closeButton != null)
                        VisualStateManager.GoToState(this.closeButton, "Active", false);
                    if (this.dockToggle != null)
                        VisualStateManager.GoToState(this.dockToggle, "Active", false);
                    if (this.optionsButton != null)
                        VisualStateManager.GoToState(this.optionsButton, "Active", false);
                }
                else if (this.captionBar.Visibility == Visibility.Visible)
                {
                    this.HeaderBackgroud = this.DockingManager.HeaderBackground;
                    this.CaptionForeGround = this.DockingManager.CaptionForeGround;
                    this.WindowBorderBrush = this.DockingManager.WindowBorderBrush;
                    this.WindowBorderThickness = this.DockingManager.WindowBorderThickness;
                    this.WindowCornerRadius = this.DockingManager.WindowCornerRadius;
                    if (this.maximizeButton != null)
                        VisualStateManager.GoToState(this.maximizeButton, "InActive", false);
                    if (this.closeButton != null)
                        VisualStateManager.GoToState(this.closeButton, "InActive", false);
                    if (this.dockToggle != null)
                        VisualStateManager.GoToState(this.dockToggle, "InActive", false);
                    if (this.optionsButton != null)
                        VisualStateManager.GoToState(this.optionsButton, "InActive", false);
                }
                else
                {
                    //(this.ContentGrid.Parent as Border).BorderBrush = new SolidColorBrush(Colors.Transparent);
                    //(this.ContentGrid.Parent as Border).BorderThickness = new Thickness(0);
                    ((this.ContentGrid.Parent as Border).Parent as Border).BorderBrush = new SolidColorBrush(Colors.Transparent);
                    ((this.ContentGrid.Parent as Border).Parent as Border).BorderThickness = new Thickness(0);
                }
                if (contentBorder != null)
                {
                    contentBorder.Margin = new Thickness(0, 0, 0, 0);
                }

                if (this.CustomTabControl != null && this.captionBar.Visibility == Visibility.Visible)
                {
                    this.CustomTabControl.WindowContentBackground = this.DockingManager.WindowContentBackground;
                    this.CustomTabControl.WindowContentMargin = this.DockingManager.WindowContentMargin;
                    this.CustomTabControl.WindowContentBorderBrush = this.DockingManager.WindowContentBorderBrush;

                    if (this.CustomTabControl.Items.Count <= 1)
                    {
                        if (this.CustomTabControl.TabPanelBorder != null)
                        {
                            this.CustomTabControl.primitiveTabPanel.Visibility = Visibility.Collapsed;
                            this.CustomTabControl.TabPanelBorder.Visibility = Visibility.Collapsed;
                            this.CustomTabControl.WindowContentBorderThickness = new Thickness(0, 1, 0, 0);
                            this.CustomTabControl.WindowContentBorderBrush = WindowBorderBrush;
                            this.CustomTabControl.WindowContentMargin = new Thickness(0, -1, 0, 0);
                        }
                        if (this.CustomTabControl.RelatedWindow != null && this.CustomTabControl.RelatedWindow != this)
                        {
                            if (this.CustomTabControl.RelatedWindow.CustomTabControl != null)
                            {
                                if (this.CustomTabControl.RelatedWindow.CustomTabControl.Items.Count <= 1)
                                {
                                    if (this.CustomTabControl.RelatedWindow.CustomTabControl.TabPanelBorder != null)
                                    {
                                        this.CustomTabControl.RelatedWindow.CustomTabControl.WindowContentBorderThickness = new Thickness();
                                        this.CustomTabControl.RelatedWindow.CustomTabControl.TabPanelBorder.Visibility = Visibility.Collapsed;
                                    }
                                }
                                else
                                {
                                    this.CustomTabControl.RelatedWindow.CustomTabControl.WindowContentBorderThickness = new Thickness(0, 0, 0, 1);
                                    if (this.CustomTabControl.RelatedWindow.CustomTabControl.TabPanelBorder != null)
                                    {
                                        this.CustomTabControl.RelatedWindow.CustomTabControl.TabPanelBorder.Visibility = Visibility.Visible;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (this.CustomTabControl.TabPanelBorder != null)
                        {
                            this.CustomTabControl.WindowContentBorderThickness = new Thickness(0, 1, 0, 0);
                            this.CustomTabControl.WindowContentBorderBrush = WindowBorderBrush;
                            this.CustomTabControl.primitiveTabPanel.Visibility = Visibility.Visible;
                            this.CustomTabControl.TabPanelBorder.Visibility = Visibility.Visible;
                            this.CustomTabControl.WindowContentBorderThickness = this.DockingManager.WindowContentBorderThickness;
                        }
                    }

                    this.CustomTabControl.WindowBackground = this.DockingManager.WindowBackground;
                }
                else if (this.CustomTabControl == null && this.captionBar.Visibility == Visibility.Visible)
                {
                    if (this.windowBorder != null)
                    {
                        this.windowBorder.BorderThickness = new Thickness(0);
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether .
        /// stores the boolean value for Window is Loaded or not
        /// </summary>
        /// <value>
        /// <see langword="true"/> if ; otherwise, <see langword="false"/>.
        /// </value>
        protected internal bool WindowLoaded
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Dock position.
        /// </summary>
        Dock _dockState = Dock.Left;

        /// <summary>
        /// Gets or sets the dock position.
        /// </summary>
        /// <value>The dock position.</value>
        protected internal Dock DockPosition
        {
            get
            {
                if (this.DockingManager != null && this._Caption != string.Empty && this.DockState == DockState.AutoHidden)
                {
                    Window w = null;
                    if (this.DockState != DockState.Float)
                    {
                        w = this.DockingManager.GetExactParentWindowForNonTabDockedWindow(this);
                        if (w != null)
                        {
                            Dock dockPosition = DockingManager.GetSideInDockedMode(w.WindowChildElement);
                            if (DockingManager.GetDockState(w.WindowChildElement) != DockState.AutoHidden && w != this)
                            {
                                return dockPosition;
                            }
                            else
                            {
                                return _dockState;
                            }
                        }
                        else
                        {
                            return _dockState;
                        }
                    }
                    else
                    {

                        w = this.DockingManager.GetExactParentWindowForNonTabFloatWindow(this);
                        if (w != null)
                        {
                            Dock dockPosition = DockingManager.GetSideInFloatMode(w.WindowChildElement);
                            return dockPosition;
                        }
                        else
                        {
                            return _dockState;
                        }
                    }

                }
                else
                {
                    return _dockState;
                }
            }
            set
            {
                if ((Dock)value != Dock.Tabbed)
                {
                    _dockState = (Dock)value;
                }
                else
                {

                }
            }
        }

        /// <summary>
        /// Gets or sets the TabName Collection.
        /// </summary>
        /// <value>The tab name collection.</value>
        protected internal List<string> TabNameCollection
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Frame work element of the window's Content .
        /// </summary>
        /// <value>The window child element.</value>
        public UIElement WindowChildElement
        {
            get;
            protected internal set;
        }

        /// <summary>
        /// Gets or sets DockPin.
        /// </summary>
        /// <value>The window dock pin.</value>
        protected internal DockPin WindowDockPin
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Tabbed Element.
        /// </summary>
        /// <value>The tabbed element.</value>
        protected internal StackPanel TabbedElement
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Window Dock Side .
        /// </summary>
        /// <value>The window dock side.</value>
        protected internal Dock WindowDockSide
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Window TargetName.
        /// </summary>
        /// <value>The window target name in docked mode.</value>
        protected internal string WindowTargetNameInDockedMode
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Window Container .
        /// </summary>
        /// <value>The window container.</value>
        protected internal WindowContainer WindowContainer
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a value indicating whether this instance .
        /// </summary>
        /// <value>
        /// <see langword="true"/> if this instance ; otherwise, <see langword="false"/>.
        /// </value>
        public bool IsHidden
        {
            get
            {
                if (this.DockingManager != null)
                {
                    if (((Canvas)DockingManager).Children.Contains(this))
                    {
                        if (this.Visibility == Visibility.Visible)
                        {
                            if (this.dockToggle != null)
                            {
                                if (this.dockToggle.Visibility == Visibility.Visible && (bool)this.dockToggle.IsChecked == false)
                                {
                                    return (WindowChildElement != null) ? ((DockingManager.GetSideInDockedMode(WindowChildElement) == Dock.Tabbed) ? true : DockState != DockState.Dock) : DockState != DockState.Dock;
                                }
                                else
                                {
                                    //return DockState != DockState.Dock;
                                    if (this.DockManager.Parent is WindowContainer && this.DockState == DockState.Float)
                                    {
                                        return (WindowChildElement != null) ? ((DockingManager.GetSideInFloatMode(WindowChildElement) == Dock.Tabbed) ? true : false) : false;
                                    }
                                    else
                                    {
                                        return (WindowChildElement != null) ? ((DockingManager.GetSideInDockedMode(WindowChildElement) == Dock.Tabbed) ? true : DockState != DockState.Dock) : DockState != DockState.Dock;
                                    }
                                }
                            }
                            else
                            {
                                if (this.DockManager.Parent is WindowContainer && this.DockState == DockState.Float)
                                {
                                    return (WindowChildElement != null) ? ((DockingManager.GetSideInFloatMode(WindowChildElement) == Dock.Tabbed) ? true : false) : false;
                                }
                                else
                                {
                                    return (WindowChildElement != null) ? ((DockingManager.GetSideInDockedMode(WindowChildElement) == Dock.Tabbed) ? true : DockState != DockState.Dock) : DockState != DockState.Dock;
                                }
                            }
                        }
                        else
                        {
                            //return DockState != DockState.Dock;
                            if (this.DockManager.Parent is WindowContainer && this.DockState == DockState.Float)
                            {
                                Dock dsd = DockingManager.GetSideInFloatMode(WindowChildElement);
                                return (WindowChildElement != null) ? ((DockingManager.GetSideInFloatMode(WindowChildElement) == Dock.Tabbed) ? true : false) : false;
                            }
                            else
                            {
                                return (WindowChildElement != null) ? ((DockingManager.GetSideInDockedMode(WindowChildElement) == Dock.Tabbed) ? true : DockState != DockState.Dock) : DockState != DockState.Dock;
                            }
                        }
                    }
                    else
                    {
                        if (this.DockManager.Parent is WindowContainer && this.DockState == DockState.Float)
                        {

                            return (WindowChildElement != null) ? ((DockingManager.GetSideInFloatMode(WindowChildElement) == Dock.Tabbed) ? true : false) : false;
                        }
                        else
                        {
                            return (WindowChildElement != null) ? ((DockingManager.GetSideInDockedMode(WindowChildElement) == Dock.Tabbed) ? true : DockState != DockState.Dock) : DockState != DockState.Dock;
                        }
                    }
                }
                else
                {
                    return (WindowChildElement != null) ? ((DockingManager.GetSideInDockedMode(WindowChildElement) == Dock.Tabbed) ? true : DockState != DockState.Dock) : DockState != DockState.Dock;
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.Silverlight.DockingManager.Window">Window</see> class.
        /// </summary>
        public Window()
        {
            this.DefaultStyleKey = typeof(Window);
            WindowDockSide = Dock.Left;
            WindowTargetNameInDockedMode = String.Empty;
            WindowDockPin = DockPin.UnPinned;
            TabNameCollection = new List<string>();
            timer = new DispatcherTimer();
            timer.Tick += new EventHandler(timer_Tick);
            timer.Interval = TimeSpan.FromMilliseconds(200);
            _timer = new DispatcherTimer();
            _timer.Interval = new TimeSpan(0, 0, 0, 0, 200);
            _timer.Tick += new EventHandler(_timer_Tick);
            WindowCollection = new List<Window>();
            DuplicateWindowCollection = new List<Window>();
            contextListBoxStyle = new Style();
            contextListItemStyle = new Style();
            TargetNameCollection = new List<string>();
            FloatWindowTargetNameCollection = new List<CustomTabItem>();
            DesiredHeightInFloatMode = 200;
            DesiredWidthInFloatMode = 200;
            SizeChanged += new SizeChangedEventHandler(Window_SizeChanged);
        }

        void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            InvalidateWindowBounds();
        }

        /// <summary>
        /// Invalidates the bounds
        /// </summary>
        internal void InvalidateWindowBounds()
        {
            if (Parent != null && ActualWidth > 0 && ActualHeight > 0)
            {
                try
                {
                    if (WindowContainer == null)
                    {
                        GeneralTransform transform = TransformToVisual(Application.Current.RootVisual);
                        Point point = transform.Transform(new Point(0, 0));
                        Size size = new Size(ActualWidth, ActualHeight);
                        BoundingRectangle = new Rect(point, size);
                        if (DockingManager != null && !DockingManager.windowList.Contains(this))
                        {
                            DockingManager.windowList.Add(this);
                        }
                    }
                    else
                    {
                        foreach (Window window in WindowCollection)
                        {
                            if (window.Parent != null && window.ActualWidth > 0 && window.ActualHeight > 0)
                            {
                                GeneralTransform transform = window.TransformToVisual(Application.Current.RootVisual);
                                Point point = transform.Transform(new Point(0, 0));
                                Size size = new Size(window.ActualWidth, window.ActualHeight);
                                window.BoundingRectangle = new Rect(point, size);
                                if (DockingManager != null && !DockingManager.windowList.Contains(window))
                                {
                                    DockingManager.windowList.Add(window);
                                }
                            }
                        }
                    }
                }
                catch
                {

                }
            }
        }

        /// <summary>
        /// Handles the Tick event of the _timer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void _timer_Tick(object sender, EventArgs e)
        {
            _timer.Stop();
        }

        /// <summary>
        /// Handles the Tick event of the timer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void timer_Tick(object sender, EventArgs e)
        {
            if (timer.IsEnabled)
            {
                DoubleAnimation opacityAnimation = new DoubleAnimation();
                Duration duration = new Duration(TimeSpan.FromSeconds(Convert.ToInt32(1000)));
                Storyboard m_monthStoryboard = new Storyboard();
                m_monthStoryboard.Duration = duration;
                opacityAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(Convert.ToInt32(1000)));
                opacityAnimation.To = 1;
                opacityAnimation.From = 0.0001;
                this.Opacity = 1;
                Storyboard.SetTargetProperty(opacityAnimation, new PropertyPath("(FrameworkElement.Opacity)"));
                Storyboard.SetTarget(opacityAnimation, this);
                m_monthStoryboard.Children.Add(opacityAnimation);
                if (!this.Resources.Contains("samplee"))
                {
                    this.Resources.Add("samplee", m_monthStoryboard);
                }

                opacityAnimation.Completed += new EventHandler(m_MonthStoryboard_Completed);
                m_monthStoryboard.Begin();
                timer.Stop();
            }
        }

        /// <summary>
        /// Handles the Completed event of the m_MonthStoryboard control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void m_MonthStoryboard_Completed(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Updates the size of the float.
        /// </summary>
        protected internal void UpdateFloatSize()
        {
            LeftPosition = ((Canvas.GetLeft(this) > 0) ? Canvas.GetLeft(this) : LeftPosition) + this.DockingManager.PresentLeftSideGrid();
            TopPosition = ((Canvas.GetTop(this) > 0) ? Canvas.GetTop(this) : TopPosition) + this.DockingManager.PresentTopSideGrid();
            if (this.ActualHeight != 0.0 && this.ActualWidth != 0.0)
            {
                //// (this.ActualHeight > 200 && this.ActualHeight != 0) ? this.ActualHeight : 200;
                ////(this.ActualWidth > 200 && this.ActualWidth != 0) ? this.ActualWidth : 200; 
                FloatHeight = this.ActualHeight;
                FloatWidth = this.ActualWidth;
            }
        }

        /// <summary>
        /// Invoked Whenever the window state is changed to Float
        /// </summary>
        protected internal void UpDateFloatWindowSize()
        {
            try
            {
                LeftPosition = ((Canvas.GetLeft(this) > 0) ? Canvas.GetLeft(this) : LeftPosition) + this.DockingManager.PresentLeftSideGrid();
                TopPosition = ((Canvas.GetTop(this) > 0) ? Canvas.GetTop(this) : TopPosition) + this.DockingManager.PresentTopSideGrid();
                if (this.ActualHeight != 0.0 && this.ActualWidth != 0.0)
                {
                    //// (this.ActualHeight > 200 && this.ActualHeight != 0) ? this.ActualHeight : 200;
                    ////(this.ActualWidth > 200 && this.ActualWidth != 0) ? this.ActualWidth : 200; 
                    FloatHeight = this.ActualHeight;
                    FloatWidth = this.ActualWidth;
                }
                else if (this.Width > 0.0 && this.Height > 0.0)
                {
                    FloatHeight = this.Height;
                    FloatWidth = this.Width;
                }

                ////this.Width = FloatWidth;
                ////this.Height = FloatHeight;
                this.Arrange(new Rect(LeftPosition, TopPosition, FloatWidth, FloatHeight));
                //Canvas.SetLeft(this, LeftPosition);
                //Canvas.SetTop(this, TopPosition);
            }
            catch
            {
            }
        }

        /// <summary>
        /// Invoked Whenever the Window State is changed to Float.
        /// </summary>
        /// <param name="w">Window object</param>
        protected internal void UpDateFloatWindowSize(Window w)
        {
            LeftPosition = Canvas.GetLeft(w);
            TopPosition = Canvas.GetTop(w);
            FloatHeight = w.ActualHeight;
            FloatWidth = w.ActualWidth;
            // w.Arrange(new Rect(LeftPosition, TopPosition, FloatWidth, FloatHeight));
        }

        /// <summary>
        /// Invoked whenever the Window State is changed to dock
        /// </summary>
        protected internal void UpdateDockWidowSize()
        {
            DockHeight = this.Height;
            DockWidth = this.Width;
        }

        /// <summary>
        /// Gets or sets the custom tab control.
        /// </summary>
        /// <value>The custom tab control.</value>
        protected internal CustomTabControl CustomTabControl
        {
            get;
            set;
        }

        /// <summary>
        /// Handles the LayoutUpdated event of the Window control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void Window_LayoutUpdated(object sender, EventArgs e)
        {
            if (innerContentPresenterOffset == -1)
            {
                innerContentPresenterOffset = this.ActualWidth - (scrollcontent.ActualWidth - scrollcontent.Margin.Left
                    - scrollcontent.Margin.Right - scrollcontent.Padding.Left - scrollcontent.Padding.Right
                    - scrollcontent.BorderThickness.Left - scrollcontent.BorderThickness.Right);
                innerContentPresenterOffset = Math.Max(innerContentPresenterOffset, 0);
                SetContentPresenterSizeAndMinSize();
            }
        }

        /// <summary>
        /// Represents the Window border.
        /// </summary>
        protected internal Border windowBorder = null;

        /// <summary>
        /// Represents the Inner Grid.
        /// </summary>
        protected internal Grid partInnerGrid = null;

        /// <summary>
        /// Represents the Stack panel.
        /// </summary>
        protected internal StackPanel stackpanel = null;

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="Window"/> is reclaimed by garbage collection.
        /// </summary>
        ~Window()
        {

        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes (such as a rebuilding layout pass) call <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            if (!WindowLoaded)
            {
                caption = GetTemplateChild("PART_Caption") as ContentControl;
                stackpanel = this.GetTemplateChild("PART_StackPanelCaption") as StackPanel;
                contextMenu = GetTemplateChild("PART_ContextMenu") as ContextMenuAdv;
                floatContextMenuItemAdv = GetTemplateChild("PART_FloatContextMenuItemAdv") as ContextMenuItemAdv;
                dockContextMenuItemAdv = GetTemplateChild("PART_DockContextMenuItemAdv") as ContextMenuItemAdv;
                autoHideContextMenuItemAdv = GetTemplateChild("PART_AutoHiddenContextMenuItemAdv") as ContextMenuItemAdv;
                hideContextMenuItemAdv = GetTemplateChild("PART_HiddenContextMenuItemAdv") as ContextMenuItemAdv;
                window = GetTemplateChild("PART_Window") as Grid;
                windowBorder = GetTemplateChild("PART_WindowBorder") as Border;
                partInnerGrid = GetTemplateChild("PART_InnerGrid") as Grid;
                scrollcontent = GetTemplateChild("PART_ScrollContent") as ScrollViewer;
                tabbedPanel = GetTemplateChild("PART_TabbedPanel") as StackPanel;
                contentpresenter = GetTemplateChild("PART_ContentPresenter") as Grid;
                captionBar = GetTemplateChild("PART_CaptionBar") as FrameworkElement;
                captionText = GetTemplateChild("PART_CaptionText") as TextBlock;
                optionsButton = GetTemplateChild("PART_OptionsButton") as ToggleButton;
                leftSidePopUp = GetTemplateChild("PART_LeftButtonPopUp") as Popup;
                rightSidePopUp = GetTemplateChild("PART_RightButtonPopUp") as Popup;
                bottomSidePopUp = GetTemplateChild("PART_BottomButtonPopUp") as Popup;
                centerPopUp = GetTemplateChild("PART_CenterPopUp") as Popup;
                topSidePopUp = GetTemplateChild("PART_TopButtonPopUp") as Popup;
                optionsPopUp = GetTemplateChild("PART_optionsPopUp") as Popup;
                innerGrid = GetTemplateChild("PART_InnerGrid") as Grid;
                outerRectangle = GetTemplateChild("Rectangle1") as Rectangle;
                outerpath = GetTemplateChild("PART_OuterPath") as Path;
                innerpath = GetTemplateChild("PART_innerPath") as Path;
                leftshadowPopUp = GetTemplateChild("PART_LeftShadowPopUp") as Popup;
                rightshadowPopUp = GetTemplateChild("PART_RightShadowPopUp") as Popup;
                bottomshadowPopUp = GetTemplateChild("PART_BottomShadowPopUp") as Popup;
                centershadowPopUp = GetTemplateChild("PART_CenterShadowPopUp") as Popup;
                topshadowPopUp = GetTemplateChild("PART_TopShadowPopUp") as Popup;
                ContentGrid = GetTemplateChild("PART_Grid") as Grid;
                contentHolder = GetTemplateChild("ContentPresent") as CustomTabControl;
                popupCollectionGrid = GetTemplateChild("Part_popUpCollection") as Grid;
                maximizeButton = GetTemplateChild("PART_MaximizeButton") as ToggleButton;
                if (captionText != null)
                {
                    captionText.Text = header;
                }
                if (contextMenu != null)
                {
                    contextMenu.Opened += new RoutedEventHandler(contextMenu_ContextMenuOpened);
                
                }

                if (floatContextMenuItemAdv != null)
                {
                    floatContextMenuItemAdv.Checked += new RoutedEventHandler(floatContextMenuItemAdv_Checked);
                }
                if (dockContextMenuItemAdv != null)
                {
                    dockContextMenuItemAdv.Checked += new RoutedEventHandler(dockContextMenuItemAdv_Checked);
                }
                if (autoHideContextMenuItemAdv != null)
                {
                    autoHideContextMenuItemAdv.Checked += new RoutedEventHandler(autoHideContextMenuItemAdv_Checked);
                }
                if (hideContextMenuItemAdv != null)
                {
                    hideContextMenuItemAdv.Checked += new RoutedEventHandler(hideContextMenuItemAdv_Checked);
                }

                closeButton = GetTemplateChild("PART_CloseButton") as ToggleButton;
                tempGrid = GetTemplateChild("Bordersh") as Grid;
                contentBorder = GetTemplateChild("PART_ContentBorder") as Border;
                if (tempGrid != null)
                {
                    tempGrid.MouseLeftButtonDown += new MouseButtonEventHandler(tempGrid_MouseLeftButtonDown);
                }
                if (closeButton != null)
                {
                    if (this.DockingManager != null)
                    {
                        if (this.DockingManager.CloseButtonTemplate != null)
                        {
                            closeButton.Style = this.DockingManager.CloseButtonTemplate;
                        }
                    }

                    closeButton.Click += new RoutedEventHandler(closeButton_Click);
                    closeButton.MouseEnter += new MouseEventHandler(closeButton_MouseEnter);
                    closeButton.MouseLeave += new MouseEventHandler(closeButton_MouseLeave);
                }

                if (contentpresenter != null)
                {
                    contentpresenter.MouseLeftButtonDown += new MouseButtonEventHandler(contentpresenter_MouseLeftButtonDown);
                }

                dockToggle = this.GetTemplateChild("PART_Dock") as ToggleButton;
                if (dockToggle != null)
                {
                    if (this.DockingManager != null)
                    {
                        if (this.DockingManager.AwlButtonTemplate != null)
                        {
                            dockToggle.Style = this.DockingManager.AwlButtonTemplate;
                        }
                    }
                    if (this.DockState == DockState.AutoHidden)
                    {
                        this.dockToggle.IsChecked = true;
                        this.WindowDockPin = DockPin.Pinned;
                    }
                    dockToggle.Checked += new RoutedEventHandler(dockToggle_pinned);
                    dockToggle.Unchecked += new RoutedEventHandler(dockToggle_Unpinned);
                }

                if (maximizeButton != null)
                {
                    if (this.DockingManager != null)
                    {
                        if (this.DockingManager.MaximizeButtonTemplate != null)
                        {
                            maximizeButton.Style = this.DockingManager.MaximizeButtonTemplate;
                        }
                    }
                    maximizeButton.Checked += new RoutedEventHandler(maximizeButton_Checked);
                    maximizeButton.Unchecked += new RoutedEventHandler(maximizeButton_Unchecked);
                }

                if (optionsPopUp != null)
                {
                    optionsPopUp.MouseEnter += new MouseEventHandler(optionsPopUp_MouseEnter);
                    optionsPopUp.MouseLeave += new MouseEventHandler(optionsPopUp_MouseLeave);
                }

                if (optionsButton != null)
                {
                    if (this.DockingManager != null)
                    {
                        if (this.DockingManager.MenuButtonTemplate != null)
                        {
                            optionsButton.Style = this.DockingManager.MenuButtonTemplate;
                        }
                    }

                    optionsButton.Click += new RoutedEventHandler(optionsButton_Click);
                    optionsButton.MouseMove += new MouseEventHandler(optionsButton_MouseMove);
                }

                DefineDragEvents();
                DefineResizeEvents();
                TabbedElement = tabbedPanel;
                WindowLoaded = true;
                base.OnApplyTemplate();
                if (_Caption == string.Empty && WindowContainer != null && !containerInvoke)
                {
                    this.dockToggle.Visibility = Visibility.Collapsed;
                    this.maximizeButton.Visibility = Visibility.Collapsed;
                    this.optionsButton.Visibility = Visibility.Collapsed;
                    this.WindowContainer.DockingManager.AddWindowIntoContainer(WindowContainer.ChildrenPosition, WindowContainer);
                    if (WindowContainer.OnApply)
                    {
                        this.Visibility = Visibility.Collapsed;
                    }
                }
                else if (_Caption == string.Empty && WindowContainer != null && containerInvoke)
                {
                    this.dockToggle.Visibility = Visibility.Collapsed;
                    this.maximizeButton.Visibility = Visibility.Collapsed;
                    this.optionsButton.Visibility = Visibility.Collapsed;
                    if (this.contentpresenter != null)
                    {
                        this.contentpresenter.Children.Add(this.WindowContainer);
                    }
                    if (WindowContainer.OnApply)
                    {
                        this.Visibility = Visibility.Collapsed;
                    }
                }

                if (this.DockState == DockState.AutoHidden && this.maximizeButton != null)
                {
                    this.maximizeButton.Visibility = Visibility.Collapsed;
                }

                if (this.DockingManager != null)
                {
                    if (captionText != null)
                    {
                        captionText.Margin = this.DockingManager.CaptionMargin;
                        captionText.FontFamily = this.DockingManager.CaptionFontFamily;
                        captionText.FontSize = this.DockingManager.CaptionFontSize;
                        captionText.Foreground = this.DockingManager.CaptionForeGround;
                        captionText.Text = Caption;
                    }
                    //((Border)captionBar).BorderBrush = this.DockingManager.HeaderBorderBrush;
                    partInnerGrid.Background = this.DockingManager.HeaderBackground;
                    windowBorder.CornerRadius = this.DockingManager.WindowCornerRadius;
                    windowBorder.BorderThickness = this.DockingManager.WindowBorderThickness;
                    windowBorder.BorderBrush = this.DockingManager.WindowBorderBrush;
                    windowBorder.Background = this.DockingManager.WindowBackground;
                }
                WindowLoaded = true;

            }
            if (this.DockingManager != null)
            {
                ((Border)captionBar).Background = this.DockingManager.HeaderBackground;
                this.DockingManager.ApplyWindowStyle(this);
                if (this.CustomTabControl != null)
                {
                    contentpresenter.Children.Add(this.CustomTabControl);
                }
            }
            if (this.DockManager != null && this.DockingManager != null)
            {
                if (!this.DockingManager.DockFill)
                {
                    if ((this.DockManager.Children[0] as DockingGrid).rootWindow == this)
                    {
                        if (this.DockingManager != null)
                        {
                            this.DockingManager.UpdateDockingGridLayOut(this.DockManager);
                        }
                    }
                }
            }

            if (WindowChildElement != null && captionBar != null)
            {
                object obj = (object)ToolTipService.GetToolTip(WindowChildElement);
                if (obj != null)
                {
                    ToolTipService.SetToolTip(captionBar, obj);
                }
            }

            if (this._Caption == string.Empty)
            {
                if (this.DockManager != null)
                {
                    if (this.DockManager.gridDocking.rootWindow == this && this.DockManager.Parent is DockingManager)
                    {
                        if (this.DockManager.DockingParent != null)
                        {
                            this.DockManager.DockingParent.UpdateDockingGridLayOut(this.DockManager);
                        }
                    }
                }
            }
            if (this.DockState == DockState.AutoHidden)
            {
                if (dockToggle != null)
                {
                    dockToggle.IsChecked = true;
                }
            }
            if (this.DockManager != null && this._Caption != string.Empty)
            {
                if (this.DockManager.Parent is WindowContainer)
                {
                    this.DockingManager.RemoveDock(this);
                }
                else if (this.DockManager.Parent is DockingManager)
                {
                    this.DockingManager.ShowDockbutton(this);
                    if (((Canvas)DockingManager).Children.Contains(this) && this.DockState != DockState.AutoHidden)
                    {
                        this.dockToggle.Visibility = Visibility.Collapsed;
                        this.ApplyBorderForFloatWindow();
                    }
                    else if (((Canvas)DockingManager).Children.Contains(this) && this.DockState == DockState.AutoHidden && WindowChildElement != null)
                    {
                        if (this.DockingManager.ShowAwlButton && this.DockState != DockState.Float &&  DockingManager.GetAwlButtonVisible(WindowChildElement))
                        {
                            this.dockToggle.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            this.dockToggle.Visibility = Visibility.Collapsed;
                        }
                        this.ApplyDockStyle();
                    }
                }
                if (dockToggle != null)
                {
                    if (this.DockingManager.ShowAwlButton && this.DockState != DockState.Float && WindowChildElement != null && DockingManager.GetAwlButtonVisible(WindowChildElement))
                    {
                        dockToggle.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        dockToggle.Visibility = Visibility.Collapsed;
                    }
                }
            }


            if (closeButton != null && this.WindowChildElement != null)
            {
                if (this.DockingManager.ShowCloseButton && DockingManager.GetCloseButtonVisible(this.WindowChildElement))
                {
                    closeButton.Visibility = Visibility.Visible;
                }
                else
                {
                    closeButton.Visibility = Visibility.Collapsed;
                }
            }

            if (optionsButton != null && WindowChildElement != null)
            {
                if (this.DockingManager.ShowMenuButton && DockingManager.GetMenuButtonVisible(WindowChildElement))
                {
                    optionsButton.Visibility = Visibility.Visible;
                }
                else
                {
                    optionsButton.Visibility = Visibility.Collapsed;
                }
            }
            if (partInnerGrid != null)
            {
                if (this.DockingManager.ShowWindowTitleBar || DockState == DockState.Float)
                {
                    partInnerGrid.Visibility = Visibility.Visible;
                }
                else
                {
                    partInnerGrid.Visibility = Visibility.Collapsed;
                }
            }
            if (captionBar != null && this.DockingManager!= null)
            {
                ((Border)captionBar).BorderBrush = this.DockingManager.WindowBorderBrush;
                if (this.DockingManager.ActiveWindow == this)
                {
                    HeaderBackgroud = (this.DockState == DockState.Float) ? this.DockingManager.FloatWindowActiveHeaderBackground : this.DockingManager.ActiveWindowColor;
                    ActiveForeground = this.DockingManager.ActiveForeground;
                    if (this.maximizeButton != null)
                        VisualStateManager.GoToState(this.maximizeButton, "Active", false);
                    if (this.closeButton != null)
                        VisualStateManager.GoToState(this.closeButton, "Active", false);
                    if (this.dockToggle != null)
                        VisualStateManager.GoToState(this.dockToggle, "Active", false);
                    if (this.optionsButton != null)
                        VisualStateManager.GoToState(this.optionsButton, "Active", false);
                }
            }
            if (windowBorder != null)
            {
                SolidColorBrush brush = new SolidColorBrush(Colors.Transparent);
                bool canExecute = DockingManager.DocumentBackGround != null ? (DockingManager.DocumentBackGround as SolidColorBrush).Color != brush.Color : true;
                if (canExecute)
                {
                    windowBorder.BorderThickness = _windowBorderThickness;
                    windowBorder.CornerRadius = _windowCornerRadius;
                    windowBorder.Background = _windowBackGround;
                    windowBorder.BorderBrush = _windowBorderBrush;
                }
                if (contentBorder != null)
                {
                    contentBorder.BorderBrush = _windowBorderBrush;
                    contentBorder.BorderThickness = new Thickness(0, 0, 0, 0);
                }
            }
            if (this.DockManager != null)
            {
                if (this.DockManager.Parent is WindowContainer)
                {
                    if (((Canvas)this.DockingManager).Children.Contains(this))
                    {
                        ApplyBorderForFloatWindow();
                    }
                    else
                    {
                        this.ApplyDockStyle();
                    }
                }
                else if (((Canvas)this.DockingManager).Children.Contains(this))
                {
                    ApplyBorderForFloatWindow();
                }
            }
            else if (((Canvas)this.DockingManager).Children.Contains(this))
            {
                ApplyBorderForFloatWindow();
            }
            if (this.DockingManager != null && WindowChildElement != null)
            {
                if (DockingManager.GetDockState(this.WindowChildElement) == DockState.Float || DockingManager.GetDockState(this.WindowChildElement) == DockState.AutoHidden)
                {
                    DockingManager.SetNoHeader(WindowChildElement, false);
                }
                NoHeaderVisibility(DockingManager.GetNoHeader(WindowChildElement), DockingManager.GetHeaderHeight(this.WindowChildElement));
            }

            if (this.DockingManager != null)
            {
                if (this.DockingManager.HeaderTemplate != null)
                {
                    this.HeaderTemplate = this.DockingManager.HeaderTemplate;
                }
            }

            if (this.maximizeButton != null)
            {
                DockingManagerResourceWrapper dockingManagerResource = new DockingManagerResourceWrapper();
                dockingManagerResource.DockingManager = DockingManager;
                if (this.MaximizedState == MaximizedState.Maximized)
                    ToolTipService.SetToolTip(this.maximizeButton, dockingManagerResource.RestoreToolTip);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void maximizeButton_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!this.InternallyChecked)
            {
                this.DockingManager.RestoreWindow(this);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void maximizeButton_Checked(object sender, RoutedEventArgs e)
        {
            if (!this.InternallyChecked)
            {
                this.DockingManager.MaximizeWindow(this);
            }
        }

        void hideContextMenuItemAdv_Checked(object sender, RoutedEventArgs e)
        {
            UIElement uiElement;
            if (this.CustomTabControl.Items.Count > 1)
            {
                uiElement = ((CustomTabItem)this.CustomTabControl.SelectedItem).OwnWindow.WindowChildElement;
            }
            else
            {
                uiElement = this.WindowChildElement;
            }

            DockStateChangingEventArgs args = new DockStateChangingEventArgs(uiElement, this.DockState, DockState.Hidden, DockSide.Left);
            DockState dockState = this.DockState;
            this.DockingManager.FireDockStateChanging(args);
            if (!args.Cancel)
            {
                CommonMethodForHideClose();
                this.DockingManager.FireDockStateChanged(uiElement, dockState, DockState.Hidden);
            }
        }

        void autoHideContextMenuItemAdv_Checked(object sender, RoutedEventArgs e)
        {
            if (this.DockState != DockState.AutoHidden)
            {
                UIElement uiElement;
                if (this.CustomTabControl.Items.Count > 1)
                {
                    uiElement = ((CustomTabItem)this.CustomTabControl.SelectedItem).OwnWindow.WindowChildElement;
                }
                else
                {
                    uiElement = this.WindowChildElement;
                }
                DockStateChangingEventArgs args = new DockStateChangingEventArgs(uiElement, this.DockState, DockState.AutoHidden, DockSide.Left);
                DockState dockState = this.DockState;
                this.DockingManager.FireDockStateChanging(args);
                if (!args.Cancel)
                {
                    AutoHideStateMenu();
                    this.DockingManager.FireDockStateChanged(uiElement, dockState, DockState.AutoHidden);
                }
            }
        }

        void dockContextMenuItemAdv_Checked(object sender, RoutedEventArgs e)
        {
            DockingStateMenu();
        }

        void floatContextMenuItemAdv_Checked(object sender, RoutedEventArgs e)
        {
            UIElement uiElement;
            if (this.CustomTabControl.Items.Count > 1)
            {
                uiElement = ((CustomTabItem)this.CustomTabControl.SelectedItem).OwnWindow.WindowChildElement;
            }
            else
            {
                uiElement = this.WindowChildElement;
            }
            DockStateChangingEventArgs args = new DockStateChangingEventArgs(uiElement, this.DockState, DockState.Float,DockingManager.GetSide(uiElement,this.DockState));
            DockState dockState = this.DockState;
            if (this.DockState != DockState.Float)
                this.DockingManager.FireDockStateChanging(args);
            if (!args.Cancel)
            {
                FloatingStateMenu();
                if (this.CustomTabControl != null)
                {
                    if (this.CustomTabControl.Items.Count > 1 && this.CustomTabControl.SelectedItem != null)
                    {
                        CustomTabItem cs = this.CustomTabControl.SelectedItem as CustomTabItem;
                        if (cs.OwnWindow.CanFloat)
                        {
                            Canvas.SetZIndex(cs.OwnWindow, ++currentZIndex);
                        }
                    }
                    else
                    {
                        Canvas.SetZIndex(this, ++currentZIndex);
                    }
                }
                this.DockingManager.FireDockStateChanged(uiElement, dockState, DockState.Float);
            }
        }

        /// <summary>
        /// Customs the contenxt menu item load.
        /// </summary>
        /// <param name="customContextMenuItemCollection">The custom context menu item collection.</param>
        /// <param name="isParent">if set to <c>true</c> [is parent].</param>
        protected internal void CustomContenxtMenuItemLoad(CustomContextMenuItemCollection customContextMenuItemCollection, bool isParent)
        {
            if (this.contextMenu != null && customContextMenuItemCollection != null)
            {
                if (isParent)
                {
                    foreach (ContextMenuItemAdv menuItem in customContextMenuItemCollection)
                    {
                        if (menuItem.Parent is ContextMenuAdv)
                        {
                            if ((menuItem.Parent as ContextMenuAdv).Items.Contains(menuItem))
                            {
                                (menuItem.Parent as ContextMenuAdv).Items.Remove(menuItem);
                            }
                        }
                        this.contextMenu.Items.Add(menuItem);
                    }
                }
                else
                {
                    foreach (ContextMenuItemAdv menuItem in customContextMenuItemCollection)
                    {
                        this.contextMenu.Items.Add(menuItem);
                    }
                }
            }
        }


        /// <summary>
        /// Noes the header visibility.
        /// </summary>
        /// <param name="visibility">if set to <c>true</c> [visibility].</param>
        /// <param name="headerHeight">Height of the header.</param>
        protected internal void NoHeaderVisibility(bool visibility, double headerHeight)
        {
            if (visibility == false)
            {
                if (this.ContentGrid != null)
                {
                    if (this.DockingManager != null)
                    {
                        if (this.DockingManager.ShowWindowTitleBar || (DockState == DockState.Float && Parent != null && Parent.GetType() == typeof(DockingManager) || (DockState == DockState.Float && !DockManager._windowContainer)))
                        {
                            ContentGrid.RowDefinitions.Clear();
                            for (int i = 0; i < 2; i++)
                            {
                                if (i == 0)
                                {
                                    RowDefinition row = new RowDefinition();
                                    row.Height = new GridLength(headerHeight);
                                    this.ContentGrid.RowDefinitions.Add(row);
                                }
                                else
                                {
                                    RowDefinition row = new RowDefinition();
                                    row.Height = new GridLength(1, GridUnitType.Star);
                                    this.ContentGrid.RowDefinitions.Add(row);
                                }
                            }
                            if (this.captionBar != null)
                            {
                                this.captionBar.Visibility = Visibility.Visible;
                            }
                            if (this.partInnerGrid != null)
                            {
                                this.partInnerGrid.Visibility = Visibility.Visible;
                            }
                        }
                        else
                        {
                            NoHeaderVisibility(true, headerHeight);
                        }
                    }
                }
            }
            else
            {
                if (this.ContentGrid != null)
                {
                    if (this.DockingManager != null)
                    {
                        bool flag = true;

                        if ((DockState == DockState.Float && Parent != null && Parent.GetType() == typeof(DockingManager) || (DockState == DockState.Float && !DockManager._windowContainer)))
                        {
                            flag = false;
                        }

                        if (flag)
                        {
                            if (ContentGrid.RowDefinitions.Count >= 0)
                            {
                                ContentGrid.RowDefinitions.Clear();
                                if (this.captionBar != null)
                                {
                                    this.captionBar.Visibility = Visibility.Collapsed;
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Handles the MouseLeftButtonDown event of the tempGrid control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        void tempGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        /// <summary>
        /// Handles the MouseLeftButtonDown event of the contentpresenter control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        void contentpresenter_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this.IsremovedFromParent || this.DockState == DockState.Float)
            {
                Canvas.SetZIndex(this, ++currentZIndex);
            }
        }

        /// <summary>
        /// Handles the MouseLeave event of the closeButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        void closeButton_MouseLeave(object sender, MouseEventArgs e)
        {
            ////outerpath.Fill = (LinearGradientBrush)this.window.Resources["closebtn_mouseOut1"];
            ////outerRectangle.Fill = (LinearGradientBrush)this.window.Resources["closebtn_mouseOut_rect"];
            ////innerpath.Fill = (LinearGradientBrush)this.window.Resources["closebtn_mouseOut2"];
        }

        /// <summary>
        /// Handles the MouseEnter event of the closeButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        void closeButton_MouseEnter(object sender, MouseEventArgs e)
        {
            ////outerpath.Fill = (LinearGradientBrush)this.window.Resources["closebtn_mouseIn1"];
            ////outerRectangle.Fill = (LinearGradientBrush)this.window.Resources["closebtn_mouseIn_rect"];
            ////innerpath.Fill = (LinearGradientBrush)this.window.Resources["closebtn_mouseIn2"];
        }

        /// <summary>
        /// Handles the MouseMove event of the rightSidePopUp control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        void rightSidePopUp_MouseMove(object sender, MouseEventArgs e)
        {
        }

        /// <summary>
        /// Handles the MouseLeave event of the optionsPopUp control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        void optionsPopUp_MouseLeave(object sender, MouseEventArgs e)
        {
        }

        /// <summary>
        /// Handles the MouseEnter event of the optionsPopUp control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        void optionsPopUp_MouseEnter(object sender, MouseEventArgs e)
        {
        }

        /// <summary>
        /// Represents the position of Options button.
        /// </summary>
        private Point optionsButtonPosition = new Point();

        /// <summary>
        /// Handles the MouseMove event of the optionsButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        void optionsButton_MouseMove(object sender, MouseEventArgs e)
        {
            optionsButtonPosition = e.GetPosition(this);
        }

        /// <summary>
        /// Handles the ContextMenuOpened event of the contextMenu control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void contextMenu_ContextMenuOpened(object sender, EventArgs e)
        {
            if (this.DockingManager.ContextMenuStyle != null && this.DockingManager.ContextMenuItemStyle != null)
            {
                this.contextMenu.Style = this.DockingManager.ContextMenuStyle;
                this.floatContextMenuItemAdv.Style = this.DockingManager.ContextMenuItemStyle;
                this.dockContextMenuItemAdv.Style = this.DockingManager.ContextMenuItemStyle;
                this.autoHideContextMenuItemAdv.Style = this.DockingManager.ContextMenuItemStyle;
                this.hideContextMenuItemAdv.Style = this.DockingManager.ContextMenuItemStyle;
            }
          
            if (this.DockingManager != null)
            {
                if (this.DockingManager.ShowMenuButton && WindowChildElement != null && DockingManager.GetMenuButtonVisible(WindowChildElement))
                {
                    List<object> collection = new List<object>();
                    if (this.contextMenu != null)
                    {
                        foreach (var menuItem in this.contextMenu.Items)
                        {
                            if (floatContextMenuItemAdv != menuItem && dockContextMenuItemAdv != menuItem &&
                                autoHideContextMenuItemAdv != menuItem && hideContextMenuItemAdv != menuItem)
                            {
                                collection.Add(menuItem);
                            }
                        }
                    }
                    foreach (var menuItem in collection)
                    {
                        if (this.contextMenu.Items.Contains(menuItem))
                        {
                            this.contextMenu.Items.Remove(menuItem);
                        }
                    }

                    DockContextMenuEventArgs args = new DockContextMenuEventArgs(this.contextMenu, (this.CustomTabControl!= null && this.CustomTabControl.Items.Count > 1)?((CustomTabItem) this.CustomTabControl.SelectedItem).OwnWindow.WindowChildElement:this.WindowChildElement);
                    this.DockingManager.FireDockContextMenuOpening(args);
                    if (args.Cancel)
                    {
                        if (this.contextMenu != null)
                        {
                            this.contextMenu.IsOpen = false;
                        }
                    }
                    else
                    {
                        if (this.CustomTabControl != null)
                        {
                            if (this.CustomTabControl.SelectedItem != null)
                            {
                                if (
                                    (this.CustomTabControl.SelectedItem as CustomTabItem).OwnWindow.WindowChildElement !=
                                    null)
                                {
                                    if (
                                        DockingManager.GetCustomContextMenuItems(
                                            (this.CustomTabControl.SelectedItem as CustomTabItem).OwnWindow.
                                                WindowChildElement) != null)
                                    {
                                        CustomContenxtMenuItemLoad(
                                            DockingManager.GetCustomContextMenuItems(
                                                (this.CustomTabControl.SelectedItem as CustomTabItem).OwnWindow.
                                                    WindowChildElement), false);
                                    }
                                    else if (DockingManager != null)
                                    {
                                        if (DockingManager.GetCustomContextMenuItems(this.DockingManager) != null)
                                        {
                                            CustomContenxtMenuItemLoad(
                                                DockingManager.GetCustomContextMenuItems(this.DockingManager), true);
                                        }
                                    }
                                }
                            }
                        }

                        if (this.contextMenu != null)
                        {
                            foreach (var menuItem in this.contextMenu.Items)
                            {
                                if (menuItem is ContextMenuItemAdv)
                                {
                                    if (floatContextMenuItemAdv != menuItem && dockContextMenuItemAdv != menuItem &&
                                        autoHideContextMenuItemAdv != menuItem && hideContextMenuItemAdv != menuItem)
                                    {
                                        if (this.DockingManager.ContextMenuItemStyle != null)
                                            ((ContextMenuItemAdv)menuItem).Style = this.DockingManager.ContextMenuItemStyle;
                                        else if(this.ContextMenuItemStyle != null)
                                            ((ContextMenuItemAdv)menuItem).Style = this.ContextMenuItemStyle;
                                    }
                                }
                            }
                        }

                        if (floatContextMenuItemAdv != null)
                        {
                            this.isDragging = false;
                            floatContextMenuItemAdv.IsEnabled = true;
                            autoHideContextMenuItemAdv.IsEnabled = true;
                            dockContextMenuItemAdv.IsEnabled = true;
                            hideContextMenuItemAdv.IsEnabled = true;
                            if (this._Caption != string.Empty)
                            {
                                // this.DockingManager.ActiveWindow = this;
                            }
                            if (DockableState == DockableState.Floating)
                            {
                                floatContextMenuItemAdv.IsChecked = true;
                                autoHideContextMenuItemAdv.IsChecked = false;
                                dockContextMenuItemAdv.IsChecked = false;
                                hideContextMenuItemAdv.IsChecked = false;
                            }
                            else if (DockableState == DockableState.Dockable)
                            {
                                floatContextMenuItemAdv.IsChecked = false;
                                autoHideContextMenuItemAdv.IsChecked = false;
                                dockContextMenuItemAdv.IsChecked = true;
                                hideContextMenuItemAdv.IsChecked = false;
                            }

                            if (this._Caption == string.Empty)
                            {
                                autoHideContextMenuItemAdv.IsEnabled = false;
                            }
                            else
                            {
                                if (this.DockState == DockState.Hidden || this.DockState == DockState.Float)
                                {
                                    autoHideContextMenuItemAdv.IsEnabled = false;
                                }
                                else if (this.dockToggle.Visibility == Visibility.Collapsed)
                                {
                                    autoHideContextMenuItemAdv.IsEnabled = false;
                                }

                            }
                            if (DockState == DockState.AutoHidden)
                            {
                                autoHideContextMenuItemAdv.IsEnabled = true;
                                floatContextMenuItemAdv.IsChecked = false;
                                floatContextMenuItemAdv.IsEnabled = false;
                                autoHideContextMenuItemAdv.IsChecked = true;
                                dockContextMenuItemAdv.IsChecked = false;
                                dockContextMenuItemAdv.IsEnabled = false;
                                hideContextMenuItemAdv.IsChecked = false;
                            }
                        }
                    }
                }
                else
                {
                    if (this.contextMenu != null)
                    {
                        this.contextMenu.IsOpen = false;
                    }
                }
            }
        }

        /// <summary>
        /// Invoked Whenever Changing the State through Context Menu
        /// </summary>
        /// <param name="sender">sender object.</param>
        /// <param name="e">Routed Event Args.</param>
        void optionsButton_Click(object sender, RoutedEventArgs e)
        {
            GeneralTransform transform = ((ToggleButton)sender).TransformToVisual(Application.Current.RootVisual as FrameworkElement);
            Point pt = transform.Transform(new Point(0, 0));

            if(this.WindowChildElement != null)
                contextMenu.OpenPopup(new Point(pt.X, pt.Y + DockingManager.GetHeaderHeight(this.WindowChildElement) + 1));
        }

        /// <summary>
        /// Handles the MouseLeave event of the l control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        void l_MouseLeave(object sender, MouseEventArgs e)
        {
            if ((bool)this.dockToggle.IsChecked)
            {
                this.DockingManager.stackPanelLeave = true;
            }
        }

        /// <summary>
        /// Handles the MouseEnter event of the l control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        void l_MouseEnter(object sender, MouseEventArgs e)
        {
            if ((bool)this.dockToggle.IsChecked)
            {
                this.DockingManager.stackPanelLeave = false;
            }
        }

        /// <summary>
        /// Handles the MouseLeftButtonDown event of the l control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        void l_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Popup p = (Popup)((ListBox)sender).Parent;
            ((ListBox)sender).Visibility = Visibility.Collapsed;
            p.IsOpen = false;
        }

        /// <summary>
        /// Actives the window.
        /// </summary>
        /// <param name="_currentwindow">The _currentwindow.</param>
        /// <returns></returns>
        Window ActiveWindow(Window _currentwindow)
        {
            Window _tabbedWindow = null;
            if (_currentwindow.CustomTabControl != null)
            {
                if (_currentwindow.CustomTabControl.Items.Count > 0)
                {
                    CustomTabItem tabItem = (CustomTabItem)_currentwindow.CustomTabControl.SelectedItem;
                    for (int i = 1; i <= _currentwindow.DockingManager.WindowCollection.Count; i++)
                    {
                        Window w;
                        if (_currentwindow.DockingManager.WindowCollection[i].GetType() == typeof(Window))
                        {
                            w = (Window)_currentwindow.DockingManager.WindowCollection[i];
                            if (tabItem.OwnWindow == w && tabItem.OwnWindow != _currentwindow)
                            {
                                CustomTabControl.Items.Remove(tabItem);
                                w.CustomTabControl.Items.Add(tabItem);
                                w._Caption = tabItem.Header.ToString().Trim();
                                w.Visibility = Visibility.Visible;
                                _tabbedWindow = w;
                                break;
                            }
                        }
                    }
                }
            }

            return _tabbedWindow;
        }

        /// <summary>
        /// Generates the window from tab.
        /// </summary>
        /// <param name="rect">The rect.</param>
        void GenerateWindowFromTab(Rect rect)
        {
            if (this.CustomTabControl != null)
            {
                if (this.CustomTabControl.Items.Count > 0)
                {
                    for (int m = CustomTabControl.Items.Count - 1; m >= 0; m--)
                    {
                        CustomTabItem tabItem = (CustomTabItem)this.CustomTabControl.Items[m];
                        for (int i = 1, index = 1; i <= this.DockingManager.WindowCollection.Count; i++)
                        {
                            Window w;
                            if (this.DockingManager.WindowCollection[i].GetType() == typeof(Window))
                            {
                                w = (Window)this.DockingManager.WindowCollection[i];
                                if (tabItem.OwnWindow == w && tabItem.OwnWindow != this)
                                {
                                    CustomTabControl.Items.Remove(tabItem);
                                    w.CustomTabControl.Items.Add(tabItem);
                                    w._Caption = tabItem.Header.ToString().Trim();
                                    Canvas.SetZIndex(w, Canvas.GetZIndex(this) + index);
                                    w.Visibility = Visibility.Visible;
                                    ((Canvas)DockingManager).Children.Add(w);
                                    w.ApplyBorderForFloatWindow();
                                    this.DockingManager.SetSizeForEachChild(w, rect);
                                    this.DockingManager.RemoveDock(w);
                                    w.DockState = DockState.Float;
                                    break;
                                }
                            }
                        }
                    }
                }

                this.CustomTabControl.primitiveTabPanel.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Removes from paretn window container.
        /// </summary>
        protected internal void RemoveFromParetnWindowContainer()
        {
            if ((this.DockManager.Children[0] as DockingGrid).gridDocking.Children.Count == 1)
            {
                (this.DockManager.Parent as WindowContainer)._window.Visibility = Visibility.Collapsed;
                if ((this.DockManager.Parent as WindowContainer)._window.Parent is Grid)
                {
                    (this.DockManager.Parent as WindowContainer)._window.ChangeState(DockState.Hidden);
                }

                if ((this.DockManager.Parent as WindowContainer)._window.Parent == null)
                {
                    //Window windowcontain = (_window.DockManager.Parent as WindowContainer)._window;
                    //if ((windowcontain.DockManager.Children[0] as DockingGrid).gridDocking.Children.Count == 0)
                    //{
                    ((Canvas)DockingManager).Children.Add((this.DockManager.Parent as WindowContainer)._window);
                    // }
                }
                else
                {

                    //_window.GetFloatParentWindowContainer((_window.DockManager.Parent as WindowContainer)._window);
                    this.AutoHideParentWindow(this);
                    ((this.DockManager.Parent as WindowContainer)._window.Parent as Grid).Children.Remove((this.DockManager.Parent as WindowContainer)._window);
                    ((Canvas)DockingManager).Children.Add((this.DockManager.Parent as WindowContainer)._window);
                }
                this.DockState = DockState.Float;
                this.AutoHideParentWindow(this);
            }
            else
            {
                this.Visibility = Visibility.Collapsed;
                this.ChangeState(DockState.AutoHidden);
                if (!((Canvas)DockingManager).Children.Contains(this))
                {
                    if (this.Parent == null)
                    {
                        ((Canvas)DockingManager).Children.Add(this);
                        this.ApplyBorderForFloatWindow();
                    }
                }

                this.DockingManager.GenerateSideGrid(this);
                this.WindowDockPin = DockPin.Pinned;
                this.Visibility = Visibility.Collapsed;

            }
        }

        /// <summary>
        /// Gets the children.
        /// </summary>
        /// <param name="windowcont">The windowcont.</param>
        protected void GetChildren(Window windowcont)
        {
            int count = 0, index = 0;
            for (int i = 0; i < windowcont.WindowCollection.Count; i++)
            {
                if (windowcont.WindowCollection[i].DockState == DockState.Dock)
                {
                    index = i;
                }
                if (windowcont.WindowCollection[i].DockState != DockState.Dock)
                {
                    count++;
                }
                else
                {
                    if (windowcont.DockManager != windowcont.WindowCollection[i].DockManager && windowcont.WindowCollection[i].DockState != DockState.Dock)
                    {
                        count++;
                    }
                }
            }
            if (count == windowcont.WindowCollection.Count - 1)
            {
                if (windowcont.WindowCollection[index].DockManager.Parent != null)
                {
                    if (((Canvas)DockingManager).Children.Contains((windowcont.WindowCollection[index].DockManager.Parent as WindowContainer)._window))
                    {
                        RemoveWindowContainer(windowcont.WindowCollection[index]);
                    }
                    else
                    {
                        if ((windowcont.WindowCollection[index].DockManager.Parent as WindowContainer)._window._Caption == string.Empty)
                        {
                            if ((windowcont.WindowCollection[index].DockManager.Parent as WindowContainer)._window.WindowContainer != null)
                            {
                                if ((windowcont.WindowCollection[index].DockManager.Parent as WindowContainer)._window.WindowContainer.DockManager != null)
                                {
                                    if (((windowcont.WindowCollection[index].DockManager.Parent as WindowContainer)._window.WindowContainer.DockManager.Children[0] as DockingGrid).gridDocking.Children.Count == 1)
                                    {
                                        Window _w = ((windowcont.WindowCollection[index].DockManager.Parent as WindowContainer)._window.WindowContainer.DockManager.Children[0] as DockingGrid).gridDocking.Children[0] as Window;
                                        //RemoveWindowContainer(_w);
                                        if (_w._Caption != string.Empty)
                                        {
                                            RemoveWindowContainer(_w);
                                        }
                                        else
                                        {
                                            RemoveWindowContainer((windowcont.WindowCollection[index].DockManager.Parent as WindowContainer)._window);
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            RemoveWindowContainer((windowcont.WindowCollection[index].DockManager.Parent as WindowContainer)._window);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Checks the window is present.
        /// </summary>
        /// <param name="leastWindow">The least window.</param>
        protected internal void CheckWindowIsPresent(Window leastWindow)
        {
            Window internalWindow = null;
            int count = 0, index = 0;
            for (int i = 0; i < leastWindow.WindowCollection.Count; i++)
            {
                if (leastWindow.WindowCollection[i].DockState == DockState.Dock)
                {
                    index = i;
                }

                if (leastWindow.WindowCollection[i].DockState != DockState.Dock)
                {
                    count++;
                }
                else
                {
                    if (leastWindow.DockManager != leastWindow.WindowCollection[i].DockManager && leastWindow.WindowCollection[i].DockState != DockState.Dock)
                    {
                        count++;
                    }
                }
            }
            if (leastWindow.WindowCollection.Count != 0)
            {
                internalWindow = leastWindow.WindowCollection[index];
                if (internalWindow._Caption != string.Empty && internalWindow.WindowCollection.Count - 1 == count)
                {
                    if (!((Canvas)internalWindow.DockingManager).Children.Contains(internalWindow))
                    {
                        internalWindow.Width = (internalWindow.Parent as Grid).ActualWidth;
                        internalWindow.Height = (internalWindow.Parent as Grid).ActualHeight;
                        Canvas.SetLeft(internalWindow, Canvas.GetLeft((internalWindow.DockManager.Parent as WindowContainer)._window));
                        Canvas.SetTop(internalWindow, Canvas.GetTop((internalWindow.DockManager.Parent as WindowContainer)._window));
                        Canvas.SetZIndex(internalWindow, Canvas.GetZIndex((internalWindow.DockManager.Parent as WindowContainer)._window));
                    }

                    //leastWindow.ChangeState(DockState.Hidden);
                    (internalWindow.DockManager.Children[0] as DockingGrid).Remove(leastWindow);
                    internalWindow.DockState = DockState.Float;
                    internalWindow.DockingManager.RemoveDock(internalWindow);
                    ((Canvas)internalWindow.DockingManager).Children.Add(internalWindow);
                    internalWindow.ApplyBorderForFloatWindow();
                    ((Canvas)internalWindow.DockingManager).Children.Remove((internalWindow.DockManager.Parent as WindowContainer)._window);
                }
                else
                {
                    Window parent = GetParent(internalWindow);
                    if (parent.WindowContainer != null)
                    {
                        if (parent.WindowContainer.DockManager != null)
                        {
                            CheckWindowIsPresent(internalWindow);
                        }
                    }
                }
            }
            else
            {
                if (leastWindow._Caption != string.Empty)
                {
                    Window parent = GetParent(leastWindow);
                    if (leastWindow.DockManager.Parent is WindowContainer)
                    {
                        Window _parent = (leastWindow.DockManager.Parent as WindowContainer)._window;
                        index = 0;
                        count = 0;
                        for (int i = 0; i < _parent.WindowCollection.Count; i++)
                        {
                            if (_parent.WindowCollection[i].DockState == DockState.Dock)
                            {
                                index = i;
                            }

                            if (_parent.WindowCollection[i].DockState != DockState.Dock)
                            {
                                count++;
                            }
                            else
                            {
                                if (_parent.DockManager != _parent.WindowCollection[i].DockManager && _parent.WindowCollection[i].DockState != DockState.Dock)
                                {
                                    count++;
                                }
                            }
                        }
                        if (_parent.WindowCollection.Count - 1 == count)
                        {
                            counter = 0;
                            CheckMoreWindowContainer(parent);
                            if (!((Canvas)leastWindow.DockingManager).Children.Contains(leastWindow) && counter == 2)
                            {
                                leastWindow.Width = parent.ActualWidth;
                                leastWindow.Height = parent.ActualHeight;
                                _leftValue = 0.0;
                                _topValue = 0.0;
                                Grid temp = GetParent((UIElement)leastWindow.Parent);
                                _attachedParentWindow = null;
                                Window _w = GetParent(leastWindow);
                                Canvas.SetLeft(leastWindow, _leftValue);
                                Canvas.SetTop(leastWindow, _topValue);
                                Canvas.SetZIndex(leastWindow, ++currentZIndex);
                            }

                            //leastWindow.ChangeState(DockState.Hidden);
                            (leastWindow.DockManager.Children[0] as DockingGrid).Remove(leastWindow);
                            leastWindow.DockState = DockState.Float;
                            leastWindow.DockingManager.RemoveDock(leastWindow);
                            ((Canvas)leastWindow.DockingManager).Children.Add(leastWindow);
                            leastWindow.ApplyBorderForFloatWindow();
                            ((Canvas)leastWindow.DockingManager).Children.Remove((leastWindow.DockManager.Parent as WindowContainer)._window);
                            ((Canvas)leastWindow.DockingManager).Children.Remove(parent);
                        }
                    }
                    else
                    {
                        // CheckWindowIsPresent(leastWindow);
                    }
                }
            }
        }

        /// <summary>
        /// Represents the counter.
        /// </summary>
        static int counter = 0;

        /// <summary>
        /// Checks the more window container.
        /// </summary>
        /// <param name="_parent">The _parent.</param>
        /// <returns>The bool.</returns>
        protected internal bool CheckMoreWindowContainer(Window _parent)
        {
            int count = 0;
            for (int i = 0; i < _parent.WindowCollection.Count; i++)
            {
                if (_parent.WindowCollection[i]._Caption == string.Empty)
                {
                    CheckMoreWindowContainer(_parent.WindowCollection[i]);
                }
                if (_parent.WindowCollection[i].DockState != DockState.Dock)
                {
                    count++;
                }
                else
                {
                    if (_parent.DockManager != _parent.WindowCollection[i].DockManager && _parent.WindowCollection[i].DockState != DockState.Dock)
                    {
                        count++;
                    }
                }
            }

            if (_parent.WindowCollection.Count - 1 == count)
            {
                ++counter;
            }
            return false;
        }


        /// <summary>
        /// Removes the window container.
        /// </summary>
        /// <param name="_window">The _window.</param>
        protected internal void RemoveWindowContainer(Window _window)
        {
            DockManager dm = _window.DockingManager.GetDockingGrid(((UIElement)_window));
            if ((_window.DockManager.Children[0] as DockingGrid).gridDocking.Children.Count == 1)
            {
                if (!((_window.DockManager.Parent as WindowContainer)._window.Parent is Grid))
                {
                    Window leastWindow = (Window)((_window.DockManager.Children[0] as DockingGrid).gridDocking.Children[0] as Window);
                    if (leastWindow._Caption != string.Empty)
                    {
                        if (!((Canvas)_window.DockingManager).Children.Contains(leastWindow))
                        {
                            leastWindow.Width = (leastWindow.Parent as Grid).ActualWidth;
                            leastWindow.Height = (leastWindow.Parent as Grid).ActualHeight;
                            Canvas.SetLeft(leastWindow, Canvas.GetLeft((_window.DockManager.Parent as WindowContainer)._window));
                            Canvas.SetTop(leastWindow, Canvas.GetTop((_window.DockManager.Parent as WindowContainer)._window));
                            Canvas.SetZIndex(leastWindow, Canvas.GetZIndex((_window.DockManager.Parent as WindowContainer)._window));
                        }

                        //leastWindow.ChangeState(DockState.Hidden);
                        (dm.Children[0] as DockingGrid).Remove(leastWindow);
                        leastWindow.DockState = DockState.Float;
                        _window.DockingManager.RemoveDock(leastWindow);
                        ((Canvas)_window.DockingManager).Children.Add(leastWindow);
                        leastWindow.ApplyBorderForFloatWindow();
                        ((Canvas)_window.DockingManager).Children.Remove((_window.DockManager.Parent as WindowContainer)._window);
                        if ((_window.DockManager.Parent as WindowContainer)._window.Parent is Grid)
                        {

                            (_window.DockManager.Parent as WindowContainer)._window.ChangeState(DockState.Hidden);
                        }
                    }
                    else if ((_window.DockManager.Parent as WindowContainer)._window.Parent is DockingManager)
                    {
                        CheckWindowIsPresent(leastWindow);
                    }
                }
                else if ((_window.DockManager.Parent as WindowContainer)._window.Parent is Grid)
                {
                    Window el = (Window)((Grid)(_window.DockManager.Parent as WindowContainer)._window.Parent).Children[0];
                    Window leastWindow = (Window)((_window.DockManager.Children[0] as DockingGrid).gridDocking.Children[0] as Window);
                    if ((el.DockManager.Children[0] as DockingGrid).gridDocking.Children.Count == 1)
                    {
                        if (!((el.DockManager.Parent as WindowContainer)._window.Parent is Grid))
                        {

                        }
                    }
                }
            }
        }

        /// <summary>
        /// Sets the pin dock window.
        /// </summary>
        protected internal void SetPinDockWindow()
        {
            Rect rect;
            if (this._Caption != string.Empty)
            {
                DockManager dm = this.DockingManager.GetDockingGrid(((UIElement)this));
                if (this.Parent != null && dm != null && dm.Parent != null)
                {
                    if (dm.Parent is DockingManager)
                    {
                        this.Visibility = Visibility.Collapsed;
                        this.ChangeState(DockState.AutoHidden);
                        if (!((Canvas)DockingManager).Children.Contains(this))
                        {
                            if (this.Parent == null)
                            {
                                ((Canvas)DockingManager).Children.Add(this);
                                this.ApplyBorderForFloatWindow();
                            }
                        }

                        this.DockingManager.GenerateSideGrid(this);
                        this.WindowDockPin = DockPin.Pinned;
                        this.Visibility = Visibility.Collapsed;
                    }
                    else if (dm.Parent.GetType() == typeof(WindowContainer))
                    {
                        int numberofChildren = NumberofChildren(this);
                        if (numberofChildren != 1)
                        {
                            this.Visibility = Visibility.Collapsed;
                            this.ChangeState(DockState.AutoHidden);
                            if (!((Canvas)DockingManager).Children.Contains(this))
                            {
                                if (this.Parent == null)
                                {
                                    // this.DockState = DockState.AutoHidden;
                                    ((Canvas)DockingManager).Children.Add(this);
                                    this.ApplyBorderForFloatWindow();
                                }
                            }

                            this.DockingManager.GenerateSideGrid(this);
                            this.WindowDockPin = DockPin.Pinned;
                            this.Visibility = Visibility.Collapsed;
                            if (NumberofChildren(this) <= 1)
                            {
                                Window _window = (dm.Parent as WindowContainer)._window;//GetParent(this);
                                if (!CheckParenthasChildren(dm))
                                {
                                    leastWindow = null;
                                    if (!CheckChildrenPresent(_window))
                                    {
                                        if (leastWindow != null)
                                        {
                                            //leastWindow.Visibility = Visibility.Collapsed;
                                            //leastWindow.ChangeState(DockState.AutoHidden);
                                            //if (!leastWindow.DockingManager.Children.Contains(leastWindow))
                                            //{
                                            //    if (leastWindow.Parent == null)
                                            //    {
                                            //      //  leastWindow.DockState = DockState.AutoHidden;
                                            //        leastWindow.DockingManager.Children.Add(leastWindow);
                                            //    }
                                            //}

                                            //leastWindow.DockingManager.GenerateSideGrid(this);
                                            //leastWindow.WindowDockPin = DockPin.Pinned;
                                            //leastWindow.Visibility = Visibility.Collapsed;

                                            //RemoveDockFloatWindowContainer_forAutoHide(leastWindow);
                                        }
                                    }
                                }
                            }
                            // AutoHideParentWindow(this);
                        }
                        else
                        {

                            this.Visibility = Visibility.Collapsed;
                            this.ChangeState(DockState.AutoHidden);
                            if (!((Canvas)DockingManager).Children.Contains(this))
                            {
                                if (this.Parent == null)
                                {
                                    // this.DockState = DockState.AutoHidden;
                                    ((Canvas)DockingManager).Children.Add(this);
                                    this.ApplyBorderForFloatWindow();
                                }
                            }

                            this.DockingManager.GenerateSideGrid(this);
                            this.WindowDockPin = DockPin.Pinned;
                            this.Visibility = Visibility.Collapsed;

                            RemoveDockFloatWindowContainer(this);
                        }
                    }
                    else
                    {
                        Window _activeWindow = null;
                        if (this.CustomTabControl.Items.Count > 1)
                        {
                            _activeWindow = ActiveWindow(this);
                        }
                        else
                        {
                            _activeWindow = this;
                        }

                        _leftValue = 0.0;
                        _topValue = 0.0;
                        Grid temp = GetParent((UIElement)this.Parent);
                        _attachedParentWindow = null;
                        Window _w = GetParent(this);
                        Window _windowContainer = (this.DockManager.Parent as WindowContainer)._window;
                        int index = _windowContainer.WindowCollection.IndexOf(this);
                        if (temp != null && _windowContainer.WindowCollection.Count > 1)
                        {
                            rect = this.DockingManager.Rectangle(_leftValue, _topValue, ((FrameworkElement)this.Parent).ActualWidth, ((FrameworkElement)this.Parent).ActualHeight + 35);
                            if (!((Canvas)DockingManager).Children.Contains(this))
                            {
                                (this.Parent as Grid).Children.Remove(this);
                                ((Canvas)DockingManager).Children.Add(this);
                                this.ApplyBorderForFloatWindow();
                            }

                            Canvas.SetZIndex(this, ++currentZIndex);
                            this.dockToggle.Visibility = Visibility.Collapsed;
                            if (this.DockingManager.ShowMenuButton && WindowChildElement != null &&  DockingManager.GetMenuButtonVisible(WindowChildElement))
                            {
                                this.optionsButton.Visibility = Visibility.Visible;
                            }
                            else
                            {
                                this.optionsButton.Visibility = Visibility.Collapsed;
                            }
                            this.ChangeState(DockState.Float);
                            this.DockingManager.SetSizeForEachChild(this, rect);
                            UpDateFloatWindowSize(this);
                            _topValue = 0.0;
                            _leftValue = 0.0;
                            _parentDockingGrid = null;
                            if (index == _windowContainer.WindowCollection.Count - 1)
                            {
                                this.DockingManager.ActiveWindow = _windowContainer.WindowCollection[index - 1];
                            }
                            else
                            {
                                this.DockingManager.ActiveWindow = _windowContainer.WindowCollection[index + 1];
                            }

                            _windowContainer.WindowCollection.Remove(this);

                        }
                    }
                    this.DockingManager.FireDockStateChanged(this.WindowChildElement, DockState.Dock, DockState.AutoHidden);
                }              
            }
            else if (this._Caption == string.Empty)
            {
                if (this.WindowCollection.Count > 0)
                {
                    Window _removedWindow = null;
                    foreach (Window w in this.WindowCollection)
                    {
                        if (this.DockingManager.ActiveWindow != null)
                        {
                            if (this.DockingManager.ActiveWindow == w)
                            {
                                Window _activeWindow = null;
                                if (w.CustomTabControl != null)
                                {
                                    if (w.CustomTabControl.Items.Count > 1)
                                    {
                                        _activeWindow = ActiveWindow(w);
                                    }
                                    else
                                    {
                                        _activeWindow = w;
                                    }
                                }
                                else
                                {
                                    _activeWindow = w;
                                }

                                Grid temp = GetParent((UIElement)w.Parent);
                                int index = this.WindowCollection.IndexOf(w);
                                if (temp != null && this.WindowCollection.Count > 1)
                                {
                                    rect = this.DockingManager.Rectangle(Canvas.GetLeft(this) + _leftValue, Canvas.GetTop(this) + _topValue, ((FrameworkElement)w.Parent).ActualWidth, ((FrameworkElement)w.Parent).ActualHeight + 35);
                                    if (!((Canvas)w.DockingManager).Children.Contains(w))
                                    {
                                        (w.Parent as Grid).Children.Remove(w);
                                        ((Canvas)w.DockingManager).Children.Add(w);
                                        w.ApplyBorderForFloatWindow();
                                        _removedWindow = w;
                                    }

                                    Canvas.SetZIndex(w, ++currentZIndex);
                                    w.dockToggle.Visibility = Visibility.Collapsed;
                                    if (w.DockingManager.ShowMenuButton && w.WindowChildElement != null && DockingManager.GetMenuButtonVisible(w.WindowChildElement))
                                    {
                                        w.optionsButton.Visibility = Visibility.Visible;
                                    }
                                    else
                                    {
                                        w.optionsButton.Visibility = Visibility.Collapsed;
                                    }
                                    //w.DockState = DockState.Float;
                                    w.ChangeState(DockState.Float);
                                    (w.DockManager.Children[0] as DockingGrid).Remove(w);
                                    w.DockingManager.SetSizeForEachChild(w, rect);
                                    UpDateFloatWindowSize(w);
                                    _topValue = 0.0;
                                    _leftValue = 0.0;
                                    _parentDockingGrid = null;
                                    if (index == this.WindowCollection.Count - 1)
                                    {
                                        w.DockingManager.ActiveWindow = this.WindowCollection[index - 1];
                                    }
                                    else
                                    {
                                        w.DockingManager.ActiveWindow = this.WindowCollection[index + 1];
                                    }

                                }
                            }
                        }
                    }

                    this.WindowCollection.Remove(_removedWindow);
                    this.DockingManager.FireDockStateChanged(this.WindowChildElement, DockState.Dock, DockState.AutoHidden);
                }            
            }           
        }

        /// <summary>
        /// Applies the border for float window.
        /// </summary>
        protected internal void ApplyBorderForFloatWindow()
        {
            if (this.DockManager != null)
            {
                if (this.DockState == DockState.Float)
                {
                    bool allowMe = false;
                    if (!(this.DockManager.Parent is WindowContainer))
                    {
                        allowMe = true;
                    }
                    else
                    {
                        if (((Canvas)this.DockingManager).Children.Contains(this))
                        {
                            allowMe = true;
                        }
                    }
                    if (this.DockingManager.FloatWindowHeaderBackground != null && allowMe)
                    {
                        if (this.DockingManager.ActiveWindow != this)
                        {
                            this.HeaderBackgroud = this.DockingManager.FloatWindowHeaderBackground;
                            this.CaptionForeGround = this.DockingManager.CaptionForeGround;
                            if (this.maximizeButton != null)
                                VisualStateManager.GoToState(this.maximizeButton, "InActive", false);
                            if (this.closeButton != null)
                                VisualStateManager.GoToState(this.closeButton, "InActive", false);
                            if (this.dockToggle != null)
                                VisualStateManager.GoToState(this.dockToggle, "InActive", false);
                            if (this.optionsButton != null)
                                VisualStateManager.GoToState(this.optionsButton, "InActive", false);
                        }
                        else
                        {
                            this.HeaderBackgroud = this.DockingManager.FloatWindowActiveHeaderBackground;
                            this.ActiveForeground = this.DockingManager.ActiveForeground;
                            if (this.maximizeButton != null)
                                VisualStateManager.GoToState(this.maximizeButton, "Active", false);
                            if (this.closeButton != null)
                                VisualStateManager.GoToState(this.closeButton, "Active", false);
                            if (this.dockToggle != null)
                                VisualStateManager.GoToState(this.dockToggle, "Active", false);
                            if (this.optionsButton != null)
                                VisualStateManager.GoToState(this.optionsButton, "Active", false);
                        }
                        this.WindowBorderBrush = this.DockingManager.FloatWindowBorderBrush;
                        this.WindowBorderThickness = this.DockingManager.FloatWindowBorderThickness;
                        this.WindowCornerRadius = this.DockingManager.FloatWindowCornerRadius;
                        if (contentBorder != null)
                        {
                            contentBorder.BorderBrush = this.DockingManager.FloatWindowContentBorderBrush; ;
                            contentBorder.BorderThickness = this.DockingManager.FloatWindowContentBorderThickness;
                            //contentBorder.Margin = new Thickness(5, 0, 5, 5);
                            contentBorder.Margin = this.DockingManager.FloatWindowContentMargin;
                        }
                        if (this.CustomTabControl != null)
                        {

                            this.CustomTabControl.WindowContentMargin = new Thickness();
                            this.CustomTabControl.WindowContentBorderBrush = this.DockingManager.FloatWindowContentBorderBrush;
                            //this.CustomTabControl.WindowContentBorderThickness = new Thickness(0,0,0,1);
                            this.CustomTabControl.WindowBackground = this.DockingManager.FloatWindowBackground;
                            this.WindowBackGround = this.DockingManager.FloatWindowBackground;
                            this.CustomTabControl.WindowContentBackground = this.DockingManager.FloatWindowContentBackground;
                            if (this.CustomTabControl.Items.Count <= 1)
                            {
                                if (this.CustomTabControl.TabPanelBorder != null)
                                {
                                    this.CustomTabControl.WindowContentBorderThickness = new Thickness();
                                    this.CustomTabControl.TabPanelBorder.Visibility = Visibility.Collapsed;
                                }
                                if (this.CustomTabControl.RelatedWindow != null)
                                {
                                    if (this.CustomTabControl.RelatedWindow.CustomTabControl != null)
                                    {
                                        if (this.CustomTabControl.RelatedWindow.CustomTabControl.Items.Count <= 1)
                                        {
                                            if (this.CustomTabControl.RelatedWindow.CustomTabControl.TabPanelBorder != null)
                                            {
                                                this.CustomTabControl.RelatedWindow.CustomTabControl.WindowContentBorderThickness = new Thickness();
                                                this.CustomTabControl.RelatedWindow.CustomTabControl.TabPanelBorder.Visibility = Visibility.Collapsed;
                                            }
                                        }
                                        else
                                        {
                                            this.CustomTabControl.RelatedWindow.CustomTabControl.WindowContentBorderThickness = new Thickness(0, 0, 0, 1);
                                            this.CustomTabControl.RelatedWindow.CustomTabControl.TabPanelBorder.Visibility = Visibility.Visible;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (this.CustomTabControl.TabPanelBorder != null)
                                {
                                    this.CustomTabControl.TabPanelBorder.Visibility = Visibility.Visible;
                                    this.CustomTabControl.TabPanelBorder.BorderBrush = this.DockingManager.WindowContentBorderBrush;
                                    this.CustomTabControl.WindowContentBorderThickness = new Thickness(0, 0, 0, 1);
                                }
                            }
                            this.WindowBackGround = this.DockingManager.FloatWindowBackground;
                        }
                        //else if (this.contentpresenter != null && this.CustomTabControl != null && this.CustomTabControl.Items.Count > 1)
                        //{
                        //    this.CustomTabControl.WindowContentBackground = this.DockingManager.FloatWindowContentBackground;
                        //    this.contentpresenter.Margin = this.DockingManager.FloatWindowContentMargin;
                        //    //this.contentpresenter.BorderBrush = this.DockingManager.FloatWindowContentBorderBrush;
                        //    //this.CustomTabControl.WindowContentBorderThickness = this.DockingManager.FloatWindowContentBorderThickness;
                        //    this.CustomTabControl.WindowBackground = this.DockingManager.FloatWindowBackground;
                        //    this.contentpresenter.Background = this.DockingManager.FloatWindowBackground;
                        //}
                    }
                    else
                    {
                        this.DockingManager.ApplyDefaultBackground(this);
                    }
                }
            }
            else
            {
                this.DockingManager.ApplyDefaultBackground(this);
            }
        }
        /// <summary>
        /// Sets the float window.
        /// </summary>
        void SetFloatWindow()
        {
            Rect rect;
            if (this._Caption != string.Empty && !((Canvas)DockingManager).Children.Contains(this))
            {
                DockManager dm = this.DockingManager.GetDockingGrid(((UIElement)this));
                if (dm == null)
                {
                    dm = this.DockManager;
                }
                if (this.Parent != null && dm != null && dm.Parent != null)
                {
                    if (dm.Parent is DockingManager)
                    {
                        if (!((Canvas)DockingManager).Children.Contains(this))
                        {
                            //this.DockState = DockState.Float;
                            if (this.FloatHeight <= 0)
                            {
                                this.Height = (this.Parent as Grid).ActualHeight;
                            }
                            else
                            {
                                this.Height = this.FloatHeight;
                            }
                            if (this.FloatHeight <= 0)
                            {
                                this.Width = (this.Parent as Grid).ActualWidth;
                            }
                            else
                            {
                                this.Width = this.FloatWidth;
                            }
                            //this.Width = (this.Parent as Grid).ActualWidth;
                            //this.Height = (this.Parent as Grid).ActualHeight;
                            if (dm.Parent is DockingManager && LeftPosition == 0 && TopPosition == 0)
                            {
                                _leftValue = 0.0;
                                _topValue = 0.0;
                                Grid temp = GetParent((UIElement)this.Parent);
                                LeftPosition = _leftValue;
                                TopPosition = _topValue;
                                if (this.FloatHeight <= 0 || this.FloatWidth <= 0)
                                {
                                    UpDateFloatWindowSize(this);
                                }
                            }
                            else if (LeftPosition == 0 && TopPosition == 0)
                            {
                                _leftValue = 0.0;
                                _topValue = 0.0;
                                Grid temp = GetParent((UIElement)this.Parent);
                                _attachedParentWindow = null;
                                Window _w = GetParent(this);
                                LeftPosition = _leftValue;
                                TopPosition = _topValue;
                                UpDateFloatWindowSize(this);
                            }

                            this.DockState = DockState.Float;
                            (dm.Children[0] as DockingGrid).ArrangeLayout();
                            this.dockToggle.Visibility = Visibility.Collapsed;
                            //if(this.maximizeButton != null)
                            //    this.maximizeButton.Visibility = Visibility.Collapsed;
                            Canvas.SetLeft(this, LeftPosition);
                            Canvas.SetTop(this, TopPosition);
                            Canvas.SetZIndex(this, ++currentZIndex);
                            if (!((Canvas)DockingManager).Children.Contains(this))
                            {
                                ((Canvas)DockingManager).Children.Add(this);
                                this.ApplyBorderForFloatWindow();
                            }
                        }
                    }
                    else if (dm.Parent.GetType() == typeof(WindowContainer))
                    {
                        DockManager windowcontainer = dm;
                        int numberofChildren = NumberofChildren((dm.Parent as WindowContainer)._window, dm);
                        if (numberofChildren != 0)
                        {
                            if (!((Canvas)DockingManager).Children.Contains(this))
                            {
                                //if (LeftPosition == 0 && TopPosition == 0 && FloatHeight == 0 && FloatWidth == 0)
                                //{
                                _leftValue = 0.0;
                                _topValue = 0.0;
                                Grid temp = GetParent((UIElement)this.Parent);
                                _attachedParentWindow = null;
                                Window _w = GetParent(this);
                                this.Width = (this.Parent as Grid).ActualWidth;
                                this.Height = (this.Parent as Grid).ActualHeight;
                                LeftPosition = _leftValue;
                                TopPosition = _topValue;
                                FloatHeight = this.Height;
                                FloatWidth = this.FloatWidth;
                                //}

                                Canvas.SetLeft(this, LeftPosition);
                                Canvas.SetTop(this, TopPosition);
                                // rect = this.DockingManager.Rectangle(_leftValue, _topValue, ((FrameworkElement)this.Parent).ActualWidth, ((FrameworkElement)this.Parent).ActualHeight);
                                if (!((Canvas)DockingManager).Children.Contains(this))
                                {
                                    // UpdateContainerWindowCollection(this);                                    
                                    this.DockManager = this.DockingManager.GetParentDockManager()._dockManager;
                                    this.OldValueDockManager = dm;
                                    (dm.Children[0] as DockingGrid).ArrangeLayout();
                                    ((Canvas)DockingManager).Children.Add(this);
                                    this.ApplyBorderForFloatWindow();
                                    this.DockState = DockState.Float;
                                }

                                Canvas.SetZIndex(this, ++currentZIndex);
                                if (this.dockToggle != null)
                                {
                                    this.dockToggle.Visibility = Visibility.Collapsed;
                                }
                                if (this.DockingManager.ShowMenuButton)
                                {
                                    this.optionsButton.Visibility = Visibility.Visible;
                                }
                                else
                                {
                                    this.optionsButton.Visibility = Visibility.Collapsed;
                                }

                                UpDateFloatWindowSize(this);
                                _topValue = 0.0;
                                _leftValue = 0.0;

                            }
                            if (NumberofChildren((dm.Parent as WindowContainer)._window, dm) <= 1)
                            {
                                Window _window = (dm.Parent as WindowContainer)._window;
                                if (!CheckParenthasChildren(dm))
                                {
                                    leastWindow = null;
                                    if (!CheckChildrenPresent(_window))
                                    {
                                        if (leastWindow != null && leastWindow.dockToggle.Visibility == Visibility.Collapsed)
                                        {
                                            if (!((Canvas)leastWindow.DockingManager).Children.Contains(leastWindow))
                                            {
                                                leastWindow.Width = (leastWindow.Parent as Grid).ActualWidth;
                                                leastWindow.Height = (leastWindow.Parent as Grid).ActualHeight;
                                                _leftValue = 0.0;
                                                _topValue = 0.0;
                                                Grid temp = GetParent((UIElement)(leastWindow.DockManager.Parent as WindowContainer)._window.Parent);
                                                _attachedParentWindow = null;
                                                Window _w = GetParent(leastWindow);
                                                rect = leastWindow.DockingManager.Rectangle(_leftValue, _topValue, ((FrameworkElement)leastWindow.Parent).ActualWidth, ((FrameworkElement)leastWindow.Parent).ActualHeight);
                                                Canvas.SetZIndex(leastWindow, ++currentZIndex);
                                                Canvas.SetLeft(leastWindow, _leftValue);
                                                Canvas.SetTop(leastWindow, _topValue);
                                                leastWindow.dockToggle.Visibility = Visibility.Collapsed;
                                                if (leastWindow.DockingManager.ShowMenuButton && leastWindow.WindowChildElement != null && DockingManager.GetMenuButtonVisible(leastWindow.WindowChildElement))
                                                {
                                                    leastWindow.optionsButton.Visibility = Visibility.Visible;
                                                }
                                                else
                                                {
                                                    leastWindow.optionsButton.Visibility = Visibility.Collapsed;
                                                }
                                                LeftPosition = _leftValue;
                                                TopPosition = _topValue;
                                            }
                                            //UpdateContainerWindowCollection(leastWindow);
                                            leastWindow.DockManager = this.DockingManager.GetParentDockManager()._dockManager;
                                            leastWindow.OldValueDockManager = dm;
                                            (dm.Children[0] as DockingGrid).ArrangeLayout();
                                            //(leastWindow.DockManager.Children[0] as DockingGrid).Remove(leastWindow);                                            
                                            leastWindow.DockingManager.RemoveDock(leastWindow);
                                            if (!((Canvas)DockingManager).Children.Contains(leastWindow))
                                            {
                                                ((Canvas)leastWindow.DockingManager).Children.Add(leastWindow);
                                                leastWindow.ApplyBorderForFloatWindow();
                                            }
                                            (dm.Parent as WindowContainer)._window.Visibility = Visibility.Collapsed;
                                            RemoveDockFloatWindowContainer(leastWindow);
                                            leastWindow.DockState = DockState.Float;

                                            this.DockingManager.SetboolValueWithTargetName(leastWindow, string.Empty, DockState.Float);
                                            DockingManager.SetTargetNameInFloatingMode(leastWindow.WindowChildElement, string.Empty);
                                            leastWindow.MoveWindowTargetName = string.Empty;
                                            
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (!((Canvas)DockingManager).Children.Contains(this))
                            {
                                this.Width = (this.Parent as Grid).ActualWidth;
                                this.Height = (this.Parent as Grid).ActualHeight;
                                _leftValue = 0.0;
                                _topValue = 0.0;
                                Grid temp = GetParent((UIElement)(dm.Parent as WindowContainer)._window.Parent);
                                _attachedParentWindow = null;
                                Window _w = GetParent(this);
                                rect = this.DockingManager.Rectangle(_leftValue, _topValue, ((FrameworkElement)this.Parent).ActualWidth, ((FrameworkElement)this.Parent).ActualHeight);
                                Canvas.SetZIndex(this, ++currentZIndex);
                                Canvas.SetLeft(this, _leftValue);
                                Canvas.SetTop(this, _topValue);
                                this.dockToggle.Visibility = Visibility.Collapsed;
                                if (this.DockingManager.ShowMenuButton && WindowChildElement != null && DockingManager.GetMenuButtonVisible(WindowChildElement))
                                {
                                    this.optionsButton.Visibility = Visibility.Visible;

                                }
                                else
                                {
                                    this.optionsButton.Visibility = Visibility.Collapsed;
                                }
                                LeftPosition = _leftValue;
                                TopPosition = _topValue;
                            }
                            //UpdateContainerWindowCollection(this);
                            this.DockManager = this.DockingManager.GetParentDockManager()._dockManager;
                            this.OldValueDockManager = dm;
                            (dm.Children[0] as DockingGrid).ArrangeLayout();
                            //(this.DockManager.Children[0] as DockingGrid).Remove(this);
                            //this.DockState = DockState.Float;
                            this.DockingManager.RemoveDock(this);
                            ((Canvas)DockingManager).Children.Add(this);
                            this.ApplyBorderForFloatWindow();
                            RemoveDockFloatWindowContainer(this);
                            Window _window = (dm.Parent as WindowContainer)._window;
                            if (!CheckParenthasChildren(dm))
                            {
                                leastWindow = null;
                                if (!CheckChildrenPresent(_window))
                                {
                                    if (leastWindow != null && leastWindow.dockToggle.Visibility == Visibility.Collapsed)
                                    {
                                        if (!((Canvas)leastWindow.DockingManager).Children.Contains(leastWindow))
                                        {
                                            leastWindow.Width = (leastWindow.Parent as Grid).ActualWidth;
                                            leastWindow.Height = (leastWindow.Parent as Grid).ActualHeight;
                                            _leftValue = 0.0;
                                            _topValue = 0.0;
                                            Grid temp = GetParent((UIElement)(leastWindow.DockManager.Parent as WindowContainer)._window.Parent);
                                            _attachedParentWindow = null;
                                            Window _w = GetParent(leastWindow);
                                            rect = leastWindow.DockingManager.Rectangle(_leftValue, _topValue, ((FrameworkElement)leastWindow.Parent).ActualWidth, ((FrameworkElement)leastWindow.Parent).ActualHeight);
                                            Canvas.SetZIndex(leastWindow, ++currentZIndex);
                                            Canvas.SetLeft(leastWindow, _leftValue);
                                            Canvas.SetTop(leastWindow, _topValue);
                                            leastWindow.dockToggle.Visibility = Visibility.Collapsed;
                                            if (leastWindow.DockingManager.ShowMenuButton && leastWindow.WindowChildElement != null && DockingManager.GetMenuButtonVisible(leastWindow.WindowChildElement))
                                            {
                                                leastWindow.optionsButton.Visibility = Visibility.Visible;

                                            }
                                            else
                                            {
                                                leastWindow.optionsButton.Visibility = Visibility.Collapsed;
                                            }
                                            LeftPosition = _leftValue;
                                            TopPosition = _topValue;
                                        }
                                        //UpdateContainerWindowCollection(leastWindow);
                                        leastWindow.DockManager = this.DockingManager.GetParentDockManager()._dockManager;
                                        leastWindow.OldValueDockManager = dm;
                                        (dm.Children[0] as DockingGrid).ArrangeLayout();
                                        //(leastWindow.DockManager.Children[0] as DockingGrid).Remove(leastWindow);
                                        //leastWindow.DockState = DockState.Hidden;
                                        leastWindow.DockingManager.RemoveDock(leastWindow);
                                        if (!((Canvas)DockingManager).Children.Contains(leastWindow))
                                        {
                                            ((Canvas)leastWindow.DockingManager).Children.Add(leastWindow);
                                            leastWindow.ApplyBorderForFloatWindow();
                                        }
                                        (dm.Parent as WindowContainer)._window.Visibility = Visibility.Collapsed;
                                        RemoveDockFloatWindowContainer(leastWindow);
                                        leastWindow.DockState = DockState.Float;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        Window _activeWindow = null;
                        if (this.CustomTabControl.Items.Count > 1)
                        {
                            _activeWindow = ActiveWindow(this);
                        }
                        else
                        {
                            _activeWindow = this;
                        }

                        _leftValue = 0.0;
                        _topValue = 0.0;
                        Grid temp = GetParent((UIElement)this.Parent);
                        _attachedParentWindow = null;
                        Window _w = GetParent(this);
                        Window _windowContainer = (this.DockManager.Parent as WindowContainer)._window;
                        int index = _windowContainer.WindowCollection.IndexOf(this);
                        if (temp != null && _windowContainer.WindowCollection.Count > 1)
                        {
                            rect = this.DockingManager.Rectangle(_leftValue, _topValue, ((FrameworkElement)this.Parent).ActualWidth, ((FrameworkElement)this.Parent).ActualHeight + 35);
                            if (!((Canvas)DockingManager).Children.Contains(this))
                            {
                                (this.Parent as Grid).Children.Remove(this);
                                ((Canvas)DockingManager).Children.Add(this);
                                this.ApplyBorderForFloatWindow();
                            }

                            Canvas.SetZIndex(this, ++currentZIndex);
                            this.dockToggle.Visibility = Visibility.Collapsed;
                            if (this.DockingManager.ShowMenuButton && WindowChildElement != null && DockingManager.GetMenuButtonVisible(WindowChildElement))
                            {
                                this.optionsButton.Visibility = Visibility.Visible;

                            }
                            else
                            {
                                this.optionsButton.Visibility = Visibility.Collapsed;
                            }
                            this.ChangeState(DockState.Float);
                            this.DockingManager.SetSizeForEachChild(this, rect);
                            UpDateFloatWindowSize(this);
                            _topValue = 0.0;
                            _leftValue = 0.0;
                            _parentDockingGrid = null;
                            if (index == _windowContainer.WindowCollection.Count - 1)
                            {
                                this.DockingManager.ActiveWindow = _windowContainer.WindowCollection[index - 1];
                            }
                            else
                            {
                                this.DockingManager.ActiveWindow = _windowContainer.WindowCollection[index + 1];
                            }

                            _windowContainer.WindowCollection.Remove(this);

                        }
                    }
                }
                else
                {
                    if (!((Canvas)DockingManager).Children.Contains(this))
                    {
                        _leftValue = this.DockingManager.ActualWidth / 2.0;
                        _topValue = this.DockingManager.ActualHeight / 2.0;
                        this.Width = (this.ActualWidth) > 0 ? this.ActualWidth : 250;
                        this.Height = (this.ActualHeight) > 0 ? this.ActualHeight : 250;
                        // ChangeState(DockState.Float);
                        if (this.dockToggle != null)
                        {
                            this.dockToggle.Visibility = Visibility.Collapsed;
                        }
                        Canvas.SetLeft(this, _leftValue);
                        Canvas.SetTop(this, _topValue);
                        Canvas.SetZIndex(this, ++currentZIndex);
                        ((Canvas)DockingManager).Children.Add(this);
                        this.ApplyBorderForFloatWindow();
                        // this.DockManager
                    }
                }
            }
            else if (this._Caption == string.Empty)
            {
                if (this.WindowCollection.Count > 0)
                {
                    foreach (Window w in this.WindowCollection)
                    {
                        if (this.DockingManager.ActiveWindow != null)
                        {
                            if (this.DockingManager.ActiveWindow == w)
                            {
                                if (w.CustomTabControl != null)
                                {
                                    if (w.CustomTabControl.Items.Count == 1)
                                    {
                                        DockManager dockManager = w.DockManager;
                                        bool isWindowContainerPresent = false;
                                        if (w.DockManager.Parent is WindowContainer)
                                        {
                                            isWindowContainerPresent = true;
                                        }
                                        else if (w.OldValueDockManager != null)
                                        {
                                            if (w.OldValueDockManager.Parent is WindowContainer)
                                            {
                                                w.DockingManager.UpdateDockModeTarget(w, true);
                                            }
                                        }


                                        //this.DockingManager.UpdateDockModeTarget(this, isWindowContainerPresent);
                                        w.SetHiddenWindowHeaderMove();
                                        if (isWindowContainerPresent)
                                        {
                                            w.DockingManager.UpdateMoveToTargetName(w, isWindowContainerPresent);
                                        }

                                        if (isWindowContainerPresent)
                                        {
                                            if (dockManager != null)
                                            {
                                                if (dockManager.Parent is WindowContainer)
                                                {
                                                    if ((dockManager.Parent as WindowContainer)._window.WindowCollection.Contains(w))
                                                    {
                                                        (dockManager.Parent as WindowContainer)._window.WindowCollection.Remove(w);
                                                    }
                                                    if (w.CustomTabControl != null)
                                                    {
                                                        foreach (CustomTabItem cstabitem in w.CustomTabControl.Items)
                                                        {
                                                            if (cstabitem.OwnWindow != w)
                                                            {
                                                                if ((dockManager.Parent as WindowContainer)._window.WindowCollection.Contains(cstabitem.OwnWindow))
                                                                {
                                                                    (dockManager.Parent as WindowContainer)._window.WindowCollection.Remove(cstabitem.OwnWindow);
                                                                }
                                                                cstabitem.OwnWindow.OldValueDockManager = null;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (w.OldValueDockManager != null)
                                            {
                                                if (w.OldValueDockManager.Parent is WindowContainer)
                                                {
                                                    if ((w.OldValueDockManager.Parent as WindowContainer)._window.WindowCollection.Contains(w))
                                                    {
                                                        (w.OldValueDockManager.Parent as WindowContainer)._window.WindowCollection.Remove(w);
                                                    }
                                                    if (w.CustomTabControl != null)
                                                    {
                                                        foreach (CustomTabItem cstabitem in w.CustomTabControl.Items)
                                                        {
                                                            if (cstabitem.OwnWindow != w)
                                                            {
                                                                if ((w.OldValueDockManager.Parent as WindowContainer)._window.WindowCollection.Contains(cstabitem.OwnWindow))
                                                                {
                                                                    (w.OldValueDockManager.Parent as WindowContainer)._window.WindowCollection.Remove(cstabitem.OwnWindow);
                                                                }
                                                                cstabitem.OwnWindow.OldValueDockManager = null;
                                                            }
                                                        }
                                                        w.OldValueDockManager = null;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        //_activeWindow = w;
                                    }
                                    //}
                                    //else
                                    //{
                                    //    _activeWindow = w;

                                    // this.DockingManager.ActiveWindow.SetFloatWindow();
                                }

                                //Grid temp = GetParent((UIElement)w.Parent);
                                //int index = this.WindowCollection.IndexOf(w);
                                //if (temp != null && this.WindowCollection.Count > 1)
                                //{
                                //    rect = this.DockingManager.Rectangle(Canvas.GetLeft(this) + _leftValue, Canvas.GetTop(this) + _topValue, ((FrameworkElement)w.Parent).ActualWidth, ((FrameworkElement)w.Parent).ActualHeight + 35);
                                //    if (!((Canvas)w.DockingManager).Children.Contains(w))
                                //    {
                                //        //(w.Parent as Grid).Children.Remove(w);
                                //        //w.DockingManager.Children.Add(w);
                                //        _removedWindow = w;
                                //    }

                                //    Canvas.SetZIndex(w, ++currentZIndex);
                                //    w.dockToggle.Visibility = Visibility.Collapsed;
                                //    w.optionsButton.Visibility = Visibility.Visible;
                                //    w.DockState = DockState.Float;
                                //    (w.DockManager.Children[0] as DockingGrid).ArrangeLayout();
                                //    //w.ChangeState(DockState.Float);
                                //    //(w.DockManager.Children[0] as DockingGrid).Remove(w);
                                //    w.DockingManager.SetSizeForEachChild(w, rect);
                                //    UpDateFloatWindowSize(w);
                                //    _topValue = 0.0;
                                //    _leftValue = 0.0;
                                //    _parentDockingGrid = null;
                                //    if (index == this.WindowCollection.Count - 1)
                                //    {
                                //        w.DockingManager.ActiveWindow = this.WindowCollection[index - 1];
                                //    }
                                //    else
                                //    {
                                //        w.DockingManager.ActiveWindow = this.WindowCollection[index + 1];
                                //    }

                                // }
                                break;
                            }
                        }
                    }

                    // this.WindowCollection.Remove(_removedWindow);

                }
            }

        }

        /// <summary>
        /// Updates the container window collection.
        /// </summary>
        /// <param name="w">The w.</param>
        protected internal void UpdateContainerWindowCollection(Window w)
        {
            if (w.DockManager.Parent is WindowContainer)
            {
                Window _windowContainer = (w.DockManager.Parent as WindowContainer)._window;
                if (_windowContainer.WindowCollection.Contains(w))
                {
                    _windowContainer.WindowCollection.Remove(w);
                }
            }
        }
        /// <summary>
        /// Sets the hidden window header move.
        /// </summary>
        void SetHiddenWindowHeaderMove()
        {
            Rect rect;
            int counter = (this.DockManager.Children[0] as DockingGrid).gridDocking.Children.Count;
            if (this._Caption != string.Empty)
            {
                if (this.CurrentStateMain == StateMaintanance.WindowContainer)
                {
                    this.CurrentStateMain = StateMaintanance.Float;
                }
                else
                {
                    StateMaintanance st;
                    if (this.CustomTabControl != null)
                    {
                        if (this.CustomTabControl.Items.Count > 1)
                        {
                            for (int i = 0; i < this.CustomTabControl.Items.Count; i++)
                            {
                                CustomTabItem cstabitem = this.CustomTabControl.Items[i] as CustomTabItem;
                                if (cstabitem.OwnWindow != null)
                                {
                                    if (cstabitem.OwnWindow != this)
                                    {
                                        cstabitem.OwnWindow.DockState = DockState.Float;
                                        st = cstabitem.OwnWindow.CurrentStateMain;
                                        cstabitem.OwnWindow.CurrentStateMain = StateMaintanance.TabWithFloat;
                                        cstabitem.OwnWindow.PreviousStateMain = st;
                                        if (((Canvas)DockingManager).Children.Contains(cstabitem.OwnWindow))
                                        {
                                            ((Canvas)DockingManager).Children.Remove(cstabitem.OwnWindow);
                                        }
                                    }
                                    else
                                    {
                                        // st = this.CurrentStateMain;
                                        this.CurrentStateMain = StateMaintanance.TabWithFloat;
                                        this.PreviousStateMain = StateMaintanance.TabWithDock;
                                    }
                                }
                            }
                        }
                        else
                        {
                            st = this.CurrentStateMain;
                            this.CurrentStateMain = this.PreviousStateMain;
                            this.PreviousStateMain = st;
                        }
                    }
                    else
                    {
                        st = this.CurrentStateMain;
                        this.CurrentStateMain = this.PreviousStateMain;
                        this.PreviousStateMain = st;
                    }
                }

                DockManager dm = this.DockingManager.GetDockingGrid(((UIElement)this));
                if (this.Parent != null && dm != null && dm.Parent != null)
                {
                    if (dm.Parent is DockingManager)
                    {
                        if (!((Canvas)DockingManager).Children.Contains(this))
                        {
                            //this.DockState = DockState.Float;
                            if (dm.Parent is DockingManager)
                            {
                                _leftValue = 0.0;
                                _topValue = 0.0;
                                Grid temp = GetParent((UIElement)this.Parent);
                            }
                            else
                            {
                                _leftValue = 0.0;
                                _topValue = 0.0;
                                Grid temp = GetParent((UIElement)this.Parent);
                                _attachedParentWindow = null;
                                Window _w = GetParent(this);
                            }
                            if (FloatHeight == 0 && FloatWidth == 0)
                            {
                                this.Width = (this.Parent as Grid).ActualWidth;
                                this.Height = (this.Parent as Grid).ActualHeight;
                                FloatHeight = this.Height;
                                FloatWidth = this.Width;
                            }
                            else
                            {
                                this.Height = FloatHeight;
                                this.Width = FloatWidth;
                            }
                            // UpdateContainerWindowCollection(this);
                            if (this.DockState != DockState.Float)
                            {
                                this.DockState = DockState.Float;
                            }
                            else
                            {
                                (this.DockManager.Children[0] as DockingGrid).ArrangeLayout();
                            }
                            this.Height = FloatHeight;
                            this.Width = FloatWidth;

                            //this.OldValueDockManager = null;

                            this.dockToggle.Visibility = Visibility.Collapsed;
                            Canvas.SetLeft(this, _leftValue);
                            Canvas.SetTop(this, _topValue);
                            Canvas.SetZIndex(this, ++currentZIndex);
                            if (this.Parent == null)
                            {
                                ((Canvas)DockingManager).Children.Add(this);
                                this.ApplyBorderForFloatWindow();
                            }
                            else
                            {
                                if (this.Parent is Grid)
                                {
                                    (this.Parent as Grid).Children.Remove(this);
                                    ((Canvas)DockingManager).Children.Add(this);
                                    this.ApplyBorderForFloatWindow();
                                }
                            }
                            this.CaptureMouse();
                            //if (this.DockingManager.DockFill)
                            //{
                            //    if (this.DockingManager.DockFill && counter == 1)
                            //    {
                            //        //(this.DockManager.Children[0] as DockingGrid).gridDocking.Children.Clear();
                            //    }
                            //}
                            // this.DockManager
                        }
                    }
                    else if (dm.Parent.GetType() == typeof(WindowContainer))
                    {
                        int numberofChildren = NumberofChildren((dm.Parent as WindowContainer)._window, dm);
                        if (numberofChildren != 1)
                        {
                            if (!((Canvas)DockingManager).Children.Contains(this))
                            {
                                _leftValue = 0.0;
                                _topValue = 0.0;
                                double left = 0.0;
                                double top = 0.0;
                                // Grid temp = GetParent((UIElement)this.Parent);
                                _attachedParentWindow = null;
                                Window _w = GetParent(this);
                                this.Width = (this.Parent as Grid).ActualWidth;
                                this.Height = (this.Parent as Grid).ActualHeight;
                                left = _leftValue;
                                top = _topValue;
                                Canvas.SetLeft(this, _leftValue);
                                Canvas.SetTop(this, _topValue);
                                //rect = this.DockingManager.Rectangle(_leftValue, _topValue, ((FrameworkElement)this.Parent).ActualWidth, ((FrameworkElement)this.Parent).ActualHeight);
                                //ChangeState(DockState.Hidden);
                                if (!((Canvas)DockingManager).Children.Contains(this))
                                {
                                    // UpdateContainerWindowCollection(this);
                                    this.DockState = DockState.Float;
                                    if (((Canvas)DockingManager).Children.Contains(this))
                                    {
                                        ((Canvas)DockingManager).Children.Remove(this);
                                    }
                                    this.DockManager = this.DockingManager.GetParentDockManager()._dockManager;
                                    (dm.Children[0] as DockingGrid).ArrangeLayout();
                                    if (!((Canvas)DockingManager).Children.Contains(this))
                                    {
                                        _leftValue = left;
                                        _topValue = top;
                                        Canvas.SetLeft(this, left);
                                        Canvas.SetTop(this, top);

                                        ((Canvas)DockingManager).Children.Add(this);
                                        this.ApplyBorderForFloatWindow();
                                    }
                                }
                                this.OldValueDockManager = null;
                                if (this.CustomTabControl != null)
                                {
                                    foreach (CustomTabItem cstabItem in this.CustomTabControl.Items)
                                    {
                                        if (cstabItem.OwnWindow != this)
                                        {
                                            cstabItem.OwnWindow.DockManager = this.DockManager;
                                            cstabItem.OwnWindow.OldValueDockManager = null;
                                        }
                                    }
                                }
                                if ((dm.Parent as WindowContainer)._window.WindowCollection.Contains(this))
                                {
                                    //                                    (dm.Parent as WindowContainer)._window.WindowCollection.Remove(this);
                                }
                                Canvas.SetZIndex(this, ++currentZIndex);
                                this.dockToggle.Visibility = Visibility.Collapsed;
                                if (this.DockingManager.ShowMenuButton && WindowChildElement != null && DockingManager.GetMenuButtonVisible(WindowChildElement))
                                {
                                    this.optionsButton.Visibility = Visibility.Visible;

                                }
                                else
                                {
                                    this.optionsButton.Visibility = Visibility.Collapsed;
                                }

                                UpDateFloatWindowSize(this);
                            }
                            if (NumberofChildren((dm.Parent as WindowContainer)._window, dm) <= 1)
                            {
                                Window _window = (dm.Parent as WindowContainer)._window;
                                if (!CheckParenthasChildren(dm))
                                {
                                    leastWindow = null;
                                    if (!CheckChildrenPresent(_window))
                                    {
                                        if (leastWindow != null && leastWindow.dockToggle.Visibility == Visibility.Collapsed)
                                        {
                                            if (!((Canvas)leastWindow.DockingManager).Children.Contains(leastWindow))
                                            {
                                                leastWindow.Width = (leastWindow.Parent as Grid).ActualWidth;
                                                leastWindow.Height = (leastWindow.Parent as Grid).ActualHeight;
                                                _leftValue = 0.0;
                                                _topValue = 0.0;
                                                Grid temp = GetParent((UIElement)(leastWindow.DockManager.Parent as WindowContainer)._window.Parent);
                                                _attachedParentWindow = null;
                                                Window _w = GetParent(leastWindow);
                                                rect = leastWindow.DockingManager.Rectangle(_leftValue, _topValue, ((FrameworkElement)leastWindow.Parent).ActualWidth, ((FrameworkElement)leastWindow.Parent).ActualHeight);
                                                Canvas.SetZIndex(leastWindow, ++currentZIndex);
                                                Canvas.SetLeft(leastWindow, _leftValue);
                                                Canvas.SetTop(leastWindow, _topValue);
                                                leastWindow.dockToggle.Visibility = Visibility.Collapsed;
                                                if (leastWindow.DockingManager.ShowMenuButton && leastWindow.WindowChildElement != null && DockingManager.GetMenuButtonVisible(leastWindow.WindowChildElement))
                                                {
                                                    leastWindow.optionsButton.Visibility = Visibility.Visible;

                                                }
                                                else
                                                {
                                                    leastWindow.optionsButton.Visibility = Visibility.Collapsed;
                                                }
                                                LeftPosition = _leftValue;
                                                TopPosition = _topValue;
                                            }

                                            //(leastWindow.DockManager.Children[0] as DockingGrid).Remove(leastWindow);
                                            // UpdateContainerWindowCollection(leastWindow);
                                            leastWindow.DockState = DockState.Float;
                                            //leastWindow.DockingManager.RemoveDock(leastWindow);
                                            DockManager windowcontainer = this.DockManager;
                                            DockManager oldvalue = this.OldValueDockManager;
                                            leastWindow.DockManager = this.DockingManager.GetParentDockManager()._dockManager;
                                            (dm.Children[0] as DockingGrid).ArrangeLayout();
                                            leastWindow.OldValueDockManager = null;
                                            if (!((Canvas)DockingManager).Children.Contains(leastWindow))
                                            {
                                                ((Canvas)DockingManager).Children.Add(leastWindow);
                                                leastWindow.ApplyBorderForFloatWindow();
                                            }
                                            //leastWindow.DockingManager.Children.Add(leastWindow);

                                            if (_window.WindowCollection.Contains(leastWindow))
                                            {
                                                _window.WindowCollection.Remove(leastWindow);
                                            }
                                            if (leastWindow.CustomTabControl != null)
                                            {
                                                foreach (CustomTabItem cstabitem in leastWindow.CustomTabControl.Items)
                                                {
                                                    if (cstabitem.OwnWindow != leastWindow)
                                                    {
                                                        if (_window.WindowCollection.Contains(cstabitem.OwnWindow))
                                                        {
                                                            _window.WindowCollection.Remove(cstabitem.OwnWindow);
                                                        }
                                                        cstabitem.OwnWindow.OldValueDockManager = null;
                                                        cstabitem.OwnWindow.DockManager = leastWindow.DockManager;
                                                    }
                                                }
                                            }

                                            _window.Visibility = Visibility.Collapsed;
                                            RemoveDockFloatWindowContainer(leastWindow);
                                        }
                                    }
                                }
                            }
                            // AutoHideParentWindow(this);
                        }
                        else
                        {

                            if (!((Canvas)DockingManager).Children.Contains(this))
                            {
                                this.Width = (this.Parent as Grid).ActualWidth;
                                this.Height = (this.Parent as Grid).ActualHeight;
                                _leftValue = 0.0;
                                _topValue = 0.0;
                                Grid temp = GetParent((UIElement)(dm.Parent as WindowContainer)._window.Parent);
                                _attachedParentWindow = null;
                                Window _w = GetParent(this);
                                rect = this.DockingManager.Rectangle(_leftValue, _topValue, ((FrameworkElement)this.Parent).ActualWidth, ((FrameworkElement)this.Parent).ActualHeight);
                                Canvas.SetZIndex(this, ++currentZIndex);
                                Canvas.SetLeft(this, _leftValue);
                                Canvas.SetTop(this, _topValue);
                                this.dockToggle.Visibility = Visibility.Collapsed;
                                if (this.DockingManager.ShowMenuButton && WindowChildElement != null && DockingManager.GetMenuButtonVisible(WindowChildElement))
                                {
                                    this.optionsButton.Visibility = Visibility.Visible;

                                }
                                else
                                {
                                    this.optionsButton.Visibility = Visibility.Collapsed;
                                }
                                LeftPosition = _leftValue;
                                TopPosition = _topValue;
                            }

                            //UpdateContainerWindowCollection(this);
                            this.DockState = DockState.Float;
                            this.DockManager = this.DockingManager.GetParentDockManager()._dockManager;
                            (dm.Children[0] as DockingGrid).ArrangeLayout();

                            this.DockingManager.RemoveDock(this);
                            if (!((Canvas)DockingManager).Children.Contains(this))
                            {
                                ((Canvas)DockingManager).Children.Add(this);
                                this.ApplyBorderForFloatWindow();
                            }

                            RemoveDockFloatWindowContainer(this);
                            this.OldValueDockManager = null;
                            Window _window = (dm.Parent as WindowContainer)._window;
                            if (!CheckParenthasChildren(dm))
                            {
                                leastWindow = null;
                                if (!CheckChildrenPresent(_window))
                                {
                                    if (leastWindow != null && leastWindow.dockToggle.Visibility == Visibility.Collapsed)
                                    {
                                        if (!((Canvas)leastWindow.DockingManager).Children.Contains(leastWindow))
                                        {
                                            leastWindow.Width = (leastWindow.Parent as Grid).ActualWidth;
                                            leastWindow.Height = (leastWindow.Parent as Grid).ActualHeight;
                                            _leftValue = 0.0;
                                            _topValue = 0.0;
                                            Grid temp = GetParent((UIElement)(leastWindow.DockManager.Parent as WindowContainer)._window.Parent);
                                            _attachedParentWindow = null;
                                            Window _w = GetParent(leastWindow);
                                            rect = leastWindow.DockingManager.Rectangle(_leftValue, _topValue, ((FrameworkElement)leastWindow.Parent).ActualWidth, ((FrameworkElement)leastWindow.Parent).ActualHeight);
                                            Canvas.SetZIndex(leastWindow, ++currentZIndex);
                                            Canvas.SetLeft(leastWindow, _leftValue);
                                            Canvas.SetTop(leastWindow, _topValue);
                                            leastWindow.dockToggle.Visibility = Visibility.Collapsed;
                                            if (leastWindow.DockingManager.ShowMenuButton && leastWindow.WindowChildElement != null && DockingManager.GetMenuButtonVisible(leastWindow.WindowChildElement))
                                            {
                                                leastWindow.optionsButton.Visibility = Visibility.Visible;

                                            }
                                            else
                                            {
                                                leastWindow.optionsButton.Visibility = Visibility.Collapsed;
                                            }
                                            LeftPosition = _leftValue;
                                            TopPosition = _topValue;
                                        }

                                        //UpdateContainerWindowCollection(leastWindow);
                                        leastWindow.DockState = DockState.Float;
                                        //leastWindow.DockingManager.RemoveDock(leastWindow);
                                        DockManager windowcontainer = this.DockManager;
                                        DockManager oldvalue = this.OldValueDockManager;
                                        leastWindow.DockManager = this.DockingManager.GetParentDockManager()._dockManager;
                                        (dm.Children[0] as DockingGrid).ArrangeLayout();
                                        leastWindow.OldValueDockManager = null;
                                        leastWindow.DockingManager.RemoveDock(leastWindow);
                                        if (!((Canvas)DockingManager).Children.Contains(leastWindow))
                                        {
                                            ((Canvas)DockingManager).Children.Add(leastWindow);
                                            leastWindow.ApplyBorderForFloatWindow();
                                        }
                                        leastWindow.OldValueDockManager = null;
                                        if (_window.WindowCollection.Contains(leastWindow))
                                        {
                                            _window.WindowCollection.Remove(leastWindow);
                                        }
                                        if (leastWindow.CustomTabControl != null)
                                        {
                                            foreach (CustomTabItem cstabitem in leastWindow.CustomTabControl.Items)
                                            {
                                                if (cstabitem.OwnWindow != leastWindow)
                                                {
                                                    if (_window.WindowCollection.Contains(cstabitem.OwnWindow))
                                                    {
                                                        _window.WindowCollection.Remove(cstabitem.OwnWindow);
                                                    }
                                                    cstabitem.OwnWindow.OldValueDockManager = null;
                                                }
                                            }
                                        }

                                        _window.Visibility = Visibility.Collapsed;
                                        RemoveDockFloatWindowContainer(leastWindow);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        Window _activeWindow = null;
                        if (this.CustomTabControl.Items.Count > 1)
                        {
                            _activeWindow = ActiveWindow(this);
                        }
                        else
                        {
                            _activeWindow = this;
                        }

                        _leftValue = 0.0;
                        _topValue = 0.0;
                        Grid temp = GetParent((UIElement)this.Parent);
                        _attachedParentWindow = null;
                        Window _w = GetParent(this);
                        Window _windowContainer = (this.DockManager.Parent as WindowContainer)._window;
                        int index = _windowContainer.WindowCollection.IndexOf(this);
                        if (temp != null && _windowContainer.WindowCollection.Count > 1)
                        {
                            rect = this.DockingManager.Rectangle(_leftValue, _topValue, ((FrameworkElement)this.Parent).ActualWidth, ((FrameworkElement)this.Parent).ActualHeight + 35);
                            //UpdateContainerWindowCollection(this);
                            this.DockState = DockState.Float;
                            this.DockManager = this.DockingManager.GetParentDockManager()._dockManager;
                            (dm.Children[0] as DockingGrid).ArrangeLayout();
                            if (!((Canvas)DockingManager).Children.Contains(this))
                            {
                                (this.Parent as Grid).Children.Remove(this);
                                ((Canvas)DockingManager).Children.Add(this);
                                this.ApplyBorderForFloatWindow();
                            }

                            this.OldValueDockManager = null;
                            Canvas.SetZIndex(this, ++currentZIndex);
                            this.dockToggle.Visibility = Visibility.Collapsed;
                            if (this.DockingManager.ShowMenuButton && WindowChildElement != null && DockingManager.GetMenuButtonVisible(WindowChildElement))
                            {
                                this.optionsButton.Visibility = Visibility.Visible;

                            }
                            else
                            {
                                this.optionsButton.Visibility = Visibility.Collapsed;
                            }
                            //this.ChangeState(DockState.Float);
                            this.DockingManager.SetSizeForEachChild(this, rect);
                            UpDateFloatWindowSize(this);
                            _topValue = 0.0;
                            _leftValue = 0.0;
                            _parentDockingGrid = null;
                            if (index == _windowContainer.WindowCollection.Count - 1)
                            {
                                this.DockingManager.ActiveWindow = _windowContainer.WindowCollection[index - 1];
                            }
                            else
                            {
                                this.DockingManager.ActiveWindow = _windowContainer.WindowCollection[index + 1];
                            }

                            _windowContainer.WindowCollection.Remove(this);

                        }
                    }
                }
            }
            else if (this._Caption == string.Empty)
            {
                if (this.WindowCollection.Count > 0)
                {
                    Window _removedWindow = null;
                    foreach (Window w in this.WindowCollection)
                    {
                        if (this.DockingManager.ActiveWindow != null)
                        {
                            if (this.DockingManager.ActiveWindow == w)
                            {
                                Window _activeWindow = null;
                                if (w.CustomTabControl != null)
                                {
                                    if (w.CustomTabControl.Items.Count > 1)
                                    {
                                        _activeWindow = ActiveWindow(w);
                                    }
                                    else
                                    {
                                        _activeWindow = w;
                                    }
                                }
                                else
                                {
                                    _activeWindow = w;
                                }

                                Grid temp = GetParent((UIElement)w.Parent);
                                int index = this.WindowCollection.IndexOf(w);
                                if (temp != null && this.WindowCollection.Count > 1)
                                {
                                    rect = this.DockingManager.Rectangle(Canvas.GetLeft(this) + _leftValue, Canvas.GetTop(this) + _topValue, ((FrameworkElement)w.Parent).ActualWidth, ((FrameworkElement)w.Parent).ActualHeight + 35);
                                    if (!((Canvas)w.DockingManager).Children.Contains(w))
                                    {
                                        //(w.Parent as Grid).Children.Remove(w);
                                        //w.DockingManager.Children.Add(w);
                                        _removedWindow = w;
                                    }

                                    Canvas.SetZIndex(w, ++currentZIndex);
                                    w.dockToggle.Visibility = Visibility.Collapsed;
                                    if (w.DockingManager.ShowMenuButton && WindowChildElement != null && DockingManager.GetMenuButtonVisible(WindowChildElement))
                                    {
                                        w.optionsButton.Visibility = Visibility.Visible;

                                    }
                                    else
                                    {
                                        w.optionsButton.Visibility = Visibility.Collapsed;
                                    }
                                    //w.DockState = DockState.Float;
                                    //UpdateContainerWindowCollection(w);
                                    w.DockState = DockState.Float;
                                    (w.DockManager.Children[0] as DockingGrid).ArrangeLayout();
                                    w.OldValueDockManager = null;
                                    //w.ChangeState(DockState.Float);
                                    //(w.DockManager.Children[0] as DockingGrid).Remove(w);
                                    w.DockingManager.SetSizeForEachChild(w, rect);
                                    UpDateFloatWindowSize(w);
                                    _topValue = 0.0;
                                    _leftValue = 0.0;
                                    _parentDockingGrid = null;
                                    if (index == this.WindowCollection.Count - 1)
                                    {
                                        w.DockingManager.ActiveWindow = this.WindowCollection[index - 1];
                                    }
                                    else
                                    {
                                        w.DockingManager.ActiveWindow = this.WindowCollection[index + 1];
                                    }

                                }
                            }
                        }
                    }

                    this.WindowCollection.Remove(_removedWindow);

                }
            }
        }

        /// <summary>
        /// Sets the hidden state window.
        /// </summary>
        void SetHiddenStateWindow()
        {
            Rect rect;
            if (this._Caption != string.Empty)
            {
                DockManager dm = this.DockingManager.GetDockingGrid(((UIElement)this));
                if (this.Parent != null && dm != null && dm.Parent != null)
                {
                    if (dm.Parent is DockingManager)
                    {
                        if (!((Canvas)DockingManager).Children.Contains(this))
                        {

                            //ChangeState(DockState.Hidden);
                            //UpdateContainerWindowCollection(this);
                            this.DockState = DockState.Hidden;
                            (this.DockManager.Children[0] as DockingGrid).ArrangeLayout();
                            StateMaintanance st = this.CurrentStateMain;
                            if (st != StateMaintanance.Float && st != StateMaintanance.WindowContainer)
                            {
                                this.CurrentStateMain = this.PreviousStateMain;
                                this.PreviousStateMain = st;
                            }
                            if (((Canvas)DockingManager).Children.Contains(this))
                            {
                                ((Canvas)DockingManager).Children.Remove(this);
                            }

                            // this.DockManager
                        }
                    }
                    else if (dm.Parent.GetType() == typeof(WindowContainer))
                    {
                        int numberofChildren = NumberofChildren((dm.Parent as WindowContainer)._window, dm);
                        if (numberofChildren != 1)
                        {
                            if (!((Canvas)DockingManager).Children.Contains(this))
                            {
                                if (!((Canvas)DockingManager).Children.Contains(this))
                                {
                                    //this.DockState = DockState.Hidden;
                                    //(this.DockManager.Children[0] as DockingGrid).Remove(this);
                                    //UpdateContainerWindowCollection(this);
                                    this.DockState = DockState.Hidden;
                                    (this.DockManager.Children[0] as DockingGrid).ArrangeLayout();
                                    StateMaintanance st = this.CurrentStateMain;
                                    if (st != StateMaintanance.Float && st != StateMaintanance.WindowContainer)
                                    {
                                        this.CurrentStateMain = this.PreviousStateMain;
                                        this.PreviousStateMain = st;
                                    }
                                    if (((Canvas)DockingManager).Children.Contains(this))
                                    {
                                        ((Canvas)DockingManager).Children.Remove(this);
                                    }
                                }
                            }
                            if (NumberofChildren((dm.Parent as WindowContainer)._window, dm) <= 1)
                            {
                                Window _window = (dm.Parent as WindowContainer)._window;
                                if (!CheckParenthasChildren(dm))
                                {
                                    leastWindow = null;
                                    if (!CheckChildrenPresent(_window))
                                    {
                                        if (leastWindow != null && leastWindow.dockToggle.Visibility == Visibility.Collapsed)
                                        {
                                            if (!((Canvas)leastWindow.DockingManager).Children.Contains(leastWindow))
                                            {
                                                leastWindow.Width = (leastWindow.Parent as Grid).ActualWidth;
                                                leastWindow.Height = (leastWindow.Parent as Grid).ActualHeight;
                                                _leftValue = 0.0;
                                                _topValue = 0.0;
                                                Grid temp = GetParent((UIElement)(leastWindow.DockManager.Parent as WindowContainer)._window.Parent);
                                                _attachedParentWindow = null;
                                                Window _w = GetParent(leastWindow);
                                                rect = leastWindow.DockingManager.Rectangle(_leftValue, _topValue, ((FrameworkElement)leastWindow.Parent).ActualWidth, ((FrameworkElement)leastWindow.Parent).ActualHeight);
                                                Canvas.SetZIndex(leastWindow, ++currentZIndex);
                                                Canvas.SetLeft(leastWindow, _leftValue);
                                                Canvas.SetTop(leastWindow, _topValue);
                                                leastWindow.dockToggle.Visibility = Visibility.Collapsed;
                                                if (leastWindow.DockingManager.ShowMenuButton && leastWindow.WindowChildElement != null && DockingManager.GetMenuButtonVisible(leastWindow.WindowChildElement))
                                                {
                                                    leastWindow.optionsButton.Visibility = Visibility.Visible;

                                                }
                                                else
                                                {
                                                    leastWindow.optionsButton.Visibility = Visibility.Collapsed;
                                                }
                                                LeftPosition = _leftValue;
                                                TopPosition = _topValue;
                                            }

                                            //(leastWindow.DockManager.Children[0] as DockingGrid).Remove(leastWindow);
                                            //leastWindow.DockState = DockState.Hidden;
                                            //UpdateContainerWindowCollection(leastWindow);
                                            DockingGrid dockingGrid = this.DockingManager.GetParentDockManager();
                                            DockManager parentDockManager = null;
                                            if (dockingGrid != null)
                                            {
                                                parentDockManager = dockingGrid._dockManager;
                                            }
                                            if (leastWindow.DockManager.Parent is WindowContainer)
                                            {
                                                DockManager swap = leastWindow.DockManager;
                                                leastWindow.DockManager = parentDockManager;
                                                leastWindow.OldValueDockManager = swap;
                                                (swap.Children[0] as DockingGrid).ArrangeLayout();
                                                (swap.Parent as WindowContainer)._window.Visibility = Visibility.Collapsed;
                                            }
                                            ((Canvas)leastWindow.DockingManager).Children.Add(leastWindow);
                                            leastWindow.ApplyBorderForFloatWindow();
                                            leastWindow.DockingManager.RemoveDock(leastWindow);
                                            RemoveDockFloatWindowContainer(leastWindow);
                                        }
                                    }
                                }
                            }
                            // AutoHideParentWindow(this);
                        }
                        else
                        {
                            //(this.DockManager.Children[0] as DockingGrid).Remove(this);
                            //this.DockState = DockState.Hidden;
                            // UpdateContainerWindowCollection(this);
                            this.DockState = DockState.Hidden;
                            (this.DockManager.Children[0] as DockingGrid).ArrangeLayout();
                            StateMaintanance st = this.CurrentStateMain;
                            if (st != StateMaintanance.Float && st != StateMaintanance.WindowContainer)
                            {
                                this.CurrentStateMain = this.PreviousStateMain;
                                this.PreviousStateMain = st;
                            }
                            if (((Canvas)DockingManager).Children.Contains(this))
                            {
                                ((Canvas)DockingManager).Children.Remove(this);
                            }

                            RemoveDockFloatWindowContainer(this);

                            Window _window = (dm.Parent as WindowContainer)._window;
                            if (!CheckParenthasChildren(dm))
                            {
                                leastWindow = null;
                                if (!CheckChildrenPresent(_window))
                                {
                                    if (leastWindow != null && leastWindow.dockToggle.Visibility == Visibility.Collapsed)
                                    {
                                        if (!((Canvas)leastWindow.DockingManager).Children.Contains(leastWindow))
                                        {
                                            leastWindow.Width = (leastWindow.Parent as Grid).ActualWidth;
                                            leastWindow.Height = (leastWindow.Parent as Grid).ActualHeight;
                                            _leftValue = 0.0;
                                            _topValue = 0.0;
                                            Grid temp = GetParent((UIElement)(leastWindow.DockManager.Parent as WindowContainer)._window.Parent);
                                            _attachedParentWindow = null;
                                            Window _w = GetParent(leastWindow);
                                            rect = leastWindow.DockingManager.Rectangle(_leftValue, _topValue, ((FrameworkElement)leastWindow.Parent).ActualWidth, ((FrameworkElement)leastWindow.Parent).ActualHeight);
                                            Canvas.SetZIndex(leastWindow, ++currentZIndex);
                                            Canvas.SetLeft(leastWindow, _leftValue);
                                            Canvas.SetTop(leastWindow, _topValue);
                                            leastWindow.dockToggle.Visibility = Visibility.Collapsed;
                                            if (leastWindow.DockingManager.ShowMenuButton)
                                            {
                                                leastWindow.optionsButton.Visibility = Visibility.Visible;

                                            }
                                            else
                                            {
                                                leastWindow.optionsButton.Visibility = Visibility.Collapsed;
                                            }
                                            LeftPosition = _leftValue;
                                            TopPosition = _topValue;
                                        }

                                        // UpdateContainerWindowCollection(leastWindow);


                                        //leastWindow.DockState = DockState.Hidden;
                                        //(leastWindow.DockManager.Children[0] as DockingGrid).ArrangeLayout();

                                        //(leastWindow.DockManager.Children[0] as DockingGrid).Remove(leastWindow);
                                        //leastWindow.DockState = DockState.Hidden;

                                        DockingGrid dockingGrid = this.DockingManager.GetParentDockManager();
                                        DockManager parentDockManager = null;
                                        if (dockingGrid != null)
                                        {
                                            parentDockManager = dockingGrid._dockManager;
                                        }
                                        if (leastWindow.DockManager.Parent is WindowContainer)
                                        {
                                            DockManager swap = leastWindow.DockManager;
                                            leastWindow.DockManager = parentDockManager;
                                            leastWindow.OldValueDockManager = swap;
                                            (swap.Children[0] as DockingGrid).ArrangeLayout();
                                            (swap.Parent as WindowContainer)._window.Visibility = Visibility.Collapsed;
                                        }
                                        leastWindow.DockingManager.RemoveDock(leastWindow);
                                        ((Canvas)leastWindow.DockingManager).Children.Add(leastWindow);
                                        leastWindow.ApplyBorderForFloatWindow();
                                        RemoveDockFloatWindowContainer(leastWindow);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        Window _activeWindow = null;
                        if (this.CustomTabControl.Items.Count > 1)
                        {
                            _activeWindow = ActiveWindow(this);
                        }
                        else
                        {
                            _activeWindow = this;
                        }

                        _leftValue = 0.0;
                        _topValue = 0.0;
                        Grid temp = GetParent((UIElement)this.Parent);
                        _attachedParentWindow = null;
                        Window _w = GetParent(this);
                        Window _windowContainer = (this.DockManager.Parent as WindowContainer)._window;
                        int index = _windowContainer.WindowCollection.IndexOf(this);
                        if (temp != null && _windowContainer.WindowCollection.Count > 1)
                        {
                            rect = this.DockingManager.Rectangle(_leftValue, _topValue, ((FrameworkElement)this.Parent).ActualWidth, ((FrameworkElement)this.Parent).ActualHeight + 35);
                            //UpdateContainerWindowCollection(this);
                            this.DockState = DockState.Hidden;
                            (((Canvas)DockingManager).Children[0] as DockingGrid).ArrangeLayout();
                            if (!((Canvas)DockingManager).Children.Contains(this))
                            {
                                //(this.Parent as Grid).Children.Remove(this);
                                ((Canvas)DockingManager).Children.Add(this);
                                this.ApplyBorderForFloatWindow();
                            }

                            Canvas.SetZIndex(this, ++currentZIndex);
                            this.dockToggle.Visibility = Visibility.Collapsed;
                            if (this.DockingManager.ShowMenuButton && WindowChildElement != null && DockingManager.GetMenuButtonVisible(WindowChildElement))
                            {
                                this.optionsButton.Visibility = Visibility.Visible;

                            }
                            else
                            {
                                this.optionsButton.Visibility = Visibility.Collapsed;
                            }
                            //this.ChangeState(DockState.Float);
                            this.DockingManager.SetSizeForEachChild(this, rect);
                            UpDateFloatWindowSize(this);
                            _topValue = 0.0;
                            _leftValue = 0.0;
                            _parentDockingGrid = null;
                            if (index == _windowContainer.WindowCollection.Count - 1)
                            {
                                this.DockingManager.ActiveWindow = _windowContainer.WindowCollection[index - 1];
                            }
                            else
                            {
                                this.DockingManager.ActiveWindow = _windowContainer.WindowCollection[index + 1];
                            }

                            _windowContainer.WindowCollection.Remove(this);

                        }
                    }
                }
            }
            else if (this._Caption == string.Empty)
            {
                if (this.WindowCollection.Count > 0)
                {
                    Window _removedWindow = null;
                    foreach (Window w in this.WindowCollection)
                    {
                        if (this.DockingManager.ActiveWindow != null)
                        {
                            if (this.DockingManager.ActiveWindow == w)
                            {
                                Window _activeWindow = null;
                                if (w.CustomTabControl != null)
                                {
                                    if (w.CustomTabControl.Items.Count > 1)
                                    {
                                        _activeWindow = ActiveWindow(w);
                                    }
                                    else
                                    {
                                        _activeWindow = w;
                                    }
                                }
                                else
                                {
                                    _activeWindow = w;
                                }

                                Grid temp = GetParent((UIElement)w.Parent);
                                int index = this.WindowCollection.IndexOf(w);
                                if (temp != null && this.WindowCollection.Count > 1)
                                {
                                    rect = this.DockingManager.Rectangle(Canvas.GetLeft(this) + _leftValue, Canvas.GetTop(this) + _topValue, ((FrameworkElement)w.Parent).ActualWidth, ((FrameworkElement)w.Parent).ActualHeight + 35);
                                    if (!((Canvas)w.DockingManager).Children.Contains(w))
                                    {
                                        //(w.Parent as Grid).Children.Remove(w);
                                        //w.DockingManager.Children.Add(w);
                                        _removedWindow = w;
                                    }

                                    Canvas.SetZIndex(w, ++currentZIndex);
                                    w.dockToggle.Visibility = Visibility.Collapsed;
                                    if (w.DockingManager.ShowMenuButton && w.WindowChildElement != null && DockingManager.GetMenuButtonVisible(w.WindowChildElement))
                                    {
                                        w.optionsButton.Visibility = Visibility.Visible;

                                    }
                                    else
                                    {
                                        w.optionsButton.Visibility = Visibility.Collapsed;
                                    }
                                    //w.DockState = DockState.Float;
                                    //UpdateContainerWindowCollection(this);
                                    w.DockState = DockState.Hidden;

                                    DockingGrid dockingGrid = this.DockingManager.GetParentDockManager();
                                    DockManager parentDockManager = null;
                                    if (dockingGrid != null)
                                    {
                                        parentDockManager = dockingGrid._dockManager;
                                    }
                                    if (w.DockManager.Parent is WindowContainer)
                                    {
                                        DockManager swap = w.DockManager;
                                        w.DockManager = parentDockManager;
                                        w.OldValueDockManager = swap;
                                        (swap.Children[0] as DockingGrid).ArrangeLayout();
                                    }

                                    //(w.DockManager.Children[0] as DockingGrid).ArrangeLayout();

                                    //(w.DockManager.Children[0] as DockingGrid).Remove(w);
                                    w.DockingManager.SetSizeForEachChild(w, rect);
                                    UpDateFloatWindowSize(w);
                                    _topValue = 0.0;
                                    _leftValue = 0.0;
                                    _parentDockingGrid = null;
                                    if (index == this.WindowCollection.Count - 1)
                                    {
                                        w.DockingManager.ActiveWindow = this.WindowCollection[index - 1];
                                    }
                                    else
                                    {
                                        w.DockingManager.ActiveWindow = this.WindowCollection[index + 1];
                                    }

                                }
                            }
                        }
                    }

                    this.WindowCollection.Remove(_removedWindow);

                }
            }
        }


        /// <summary>
        /// Represents the Parent docking grid.
        /// </summary>
        private Grid _parentDockingGrid = null;

        /// <summary>
        /// Gets or sets the Left Value .
        /// </summary>
        /// <value>The _left value.</value>
        internal double _leftValue
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Top Value .
        /// </summary>
        /// <value>The _top value.</value>
        internal double _topValue
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the parent.
        /// </summary>
        /// <param name="elem">The elem.</param>
        /// <returns>The Parent Docking Grid.</returns>
        protected internal Grid GetParent(UIElement elem)
        {
            if (elem != null)
            {
                if (elem.GetType() == typeof(Grid))
                {
                    int ir = Grid.GetRow((FrameworkElement)elem);
                    int ic = Grid.GetColumn((FrameworkElement)elem);
                    UIElement ele = (UIElement)VisualTreeHelper.GetParent((UIElement)elem);
                    if (ele.GetType() != typeof(DockingGrid))
                    {
                        if (ir > 0)
                        {
                            _topValue = _topValue + ((Grid)ele).RowDefinitions[ir - 1].ActualHeight + 25;
                        }

                        if (ic > 0)
                        {
                            _leftValue = _leftValue + ((Grid)ele).ColumnDefinitions[ic - 1].ActualWidth + 6;
                        }
                    }

                    if (ele.GetType() == typeof(DockingGrid))
                    {
                        _parentDockingGrid = (ele as DockingGrid).gridDocking;
                        return _parentDockingGrid;
                    }
                    else
                    {
                        _parentDockingGrid = GetParent(ele);
                        return _parentDockingGrid;
                    }
                }
                else
                {
                    _parentDockingGrid = null;
                    return _parentDockingGrid;
                }
            }
            else
            {
                return new Grid();
            }
        }


        /// <summary>
        /// Tabs the item is active window.
        /// </summary>
        /// <param name="_w">The _w.</param>
        protected internal void TabItemIsActiveWindow(Window _w)
        {
            Window window = null;
            if (_w.CustomTabControl != null && (CustomTabItem)_w.CustomTabControl.SelectedItem != null)
            {
                CustomTabItem custabItem = (CustomTabItem)_w.CustomTabControl.SelectedItem;
                if (((CustomTabItem)_w.CustomTabControl.SelectedItem).OwnWindow == _w)
                {
                    if (_w.CustomTabControl.Items.Count > 1)
                    {
                        CustomTabItem cusTab = null;
                        if (_w.CustomTabControl.SelectedIndex != _w.CustomTabControl.Items.Count - 1)
                        {
                            cusTab = (CustomTabItem)_w.CustomTabControl.Items[_w.CustomTabControl.SelectedIndex + 1];
                        }
                        else
                        {
                            cusTab = (CustomTabItem)_w.CustomTabControl.Items[0];
                        }

                        for (int i = 1; i <= _w.DockingManager.WindowCollection.Count; i++)
                        {
                            if (_w.DockingManager.WindowCollection[i].GetType() == typeof(Window))
                            {
                                if (cusTab.OwnWindow == _w.DockingManager.WindowCollection[i])
                                {
                                    window = _w.DockingManager.WindowCollection[i];
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        custabItem = (CustomTabItem)_w.CustomTabControl.SelectedItem;
                        for (int i = 1; i <= _w.DockingManager.WindowCollection.Count; i++)
                        {
                            if (_w.DockingManager.WindowCollection[i].GetType() == typeof(Window))
                            {
                                if (custabItem.OwnWindow == _w.DockingManager.WindowCollection[i])
                                {
                                    window = _w.DockingManager.WindowCollection[i];
                                    break;
                                }
                            }
                        }

                        window = null;
                    }
                }
                else
                {
                    window = null;
                    for (int i = 1; i <= _w.DockingManager.WindowCollection.Count; i++)
                    {
                        if (_w.DockingManager.WindowCollection[i].GetType() == typeof(Window))
                        {
                            if (custabItem.OwnWindow == _w.DockingManager.WindowCollection[i])
                            {
                                window = _w.DockingManager.WindowCollection[i];
                                break;
                            }
                        }
                    }
                    _w.CustomTabControl.Items.Remove(custabItem);
                    _w.Caption = ((CustomTabItem)_w.CustomTabControl.SelectedItem).Header.ToString().Trim();
                    if (_w.CustomTabControl.Items.Count == 1)
                    {
                        if (_w.CustomTabControl.primitiveTabPanel != null)
                        {
                            _w.CustomTabControl.primitiveTabPanel.Visibility = Visibility.Collapsed;
                        }
                    }
                    if (window != null)
                    {
                        if (!window.CustomTabControl.Items.Contains(custabItem))
                        {
                            window.CustomTabControl.Items.Add(custabItem);
                            if (this.DockManager.Parent is DockingManager)
                            {
                                window.DockingManager.ShowDockbutton(window);
                            }
                            else
                            {
                                window.DockingManager.RemoveDock(window);
                            }
                        }
                        _leftValue = 0.0;
                        _topValue = 0.0;
                        window.DockState = DockState.Float;
                        _w.DockableState = DockableState.Dockable;
                        window.DockableState = DockableState.Floating;
                        Grid temp = GetParent((UIElement)this.Parent);
                        if (this.dockToggle.Visibility == Visibility.Collapsed)
                        {
                            _attachedParentWindow = null;
                            Window _win = GetParent(this);
                        }
                        if (this.Parent is Grid)
                        {
                            window.Width = (this.Parent as Grid).ActualWidth;
                            window.Height = (this.Parent as Grid).ActualHeight;
                            Canvas.SetLeft(window, _leftValue);
                            Canvas.SetTop(window, _topValue);
                            Canvas.SetZIndex(window, ++currentZIndex);
                        }
                        else
                        {
                            window.Width = this.Width;
                            window.Height = this.Height;
                            Canvas.SetLeft(window, Canvas.GetLeft(this));
                            Canvas.SetTop(window, Canvas.GetTop(this));
                            Canvas.SetZIndex(window, ++currentZIndex);
                        }
                        if (window.dockToggle != null)
                        {
                            window.dockToggle.Visibility = Visibility.Collapsed;
                        }

                        if (!((Canvas)DockingManager).Children.Contains(window))
                        {
                            ((Canvas)DockingManager).Children.Add(window);
                            window.ApplyBorderForFloatWindow();
                        }
                        _leftValue = 0.0;
                        _topValue = 0.0;
                        window.Visibility = Visibility.Visible;

                        if (_w.dockToggle.Visibility == Visibility.Visible)
                        {
                            _w.DockingManager.ShowDockbutton(_w);
                        }
                        else
                        {
                            _w.DockingManager.RemoveDock(_w);
                        }
                        window.DockState = DockState.Float;
                        window = null;
                    }
                }
            }

            if (window != null)
            {
                bool hasMoreTabItem = false;
                if (_w.CustomTabControl != null)
                {
                    if (_w.CustomTabControl.Items.Count > 1)
                    {
                        hasMoreTabItem = true;
                    }
                }
                window.Visibility = Visibility.Visible;
                CustomTabItem custabItem = (CustomTabItem)_w.CustomTabControl.SelectedItem;
                for (int m = _w.CustomTabControl.Items.Count - 1; m >= 0; m--)
                {
                    CustomTabItem cusTab = (CustomTabItem)_w.CustomTabControl.Items[m];
                    if (custabItem != cusTab)
                    {
                        _w.CustomTabControl.Items.Remove(cusTab);
                        window.CustomTabControl.Items.Insert(0, cusTab);
                        if (cusTab.OwnWindow == window)
                        {
                            window.CustomTabControl.SelectedItem = cusTab;
                        }
                    }
                }

                if (_w.DockManager.Parent.GetType() == typeof(WindowContainer))
                {
                    Canvas.SetZIndex((_w.DockManager.Parent as WindowContainer)._window, 1);
                }

                window.CustomTabControl.TabPanelBackground = _w.DockingManager.TabPanelBackground;
                window.PaneHeight = _w.ActualHeight;
                window.PaneWidth = _w.ActualWidth;
                if (hasMoreTabItem)
                {
                    if (this.DockManager.Parent is DockingManager)
                    {
                        _w.DockingManager.ShowDockbutton(window);
                    }
                    else
                    {
                        _w.DockingManager.RemoveDock(window);
                    }
                    _leftValue = 0.0;
                    _topValue = 0.0;
                    Grid temp = GetParent((UIElement)this.Parent);
                    if (this.dockToggle.Visibility == Visibility.Collapsed)
                    {
                        _attachedParentWindow = null;
                        Window _win = GetParent(this);
                    }
                    if (this.Parent is Grid)
                    {
                        this.Width = (this.Parent as Grid).ActualWidth;
                        this.Height = (this.Parent as Grid).ActualHeight;
                    }
                    this.dockToggle.Visibility = Visibility.Collapsed;
                    if (_w.DockState == DockState.Dock)
                    {
                        _w.DockingManager.UpdateTargetName(_w, window);
                    }
                    else
                    {
                        _w.DockingManager.UpdateTargetNameForFloatWindow(_w, window);
                    }
                    (_w.DockManager.Children[0] as DockingGrid).ReplaceChild(window, _w, _w.DockPosition);
                    _w.DockableState = DockableState.Floating;
                    window.DockableState = DockableState.Dockable;


                    if (!((Canvas)DockingManager).Children.Contains(this))
                    {
                        Canvas.SetLeft(this, _leftValue);
                        Canvas.SetTop(this, _topValue);
                        Canvas.SetZIndex(this, ++currentZIndex);
                        ((Canvas)DockingManager).Children.Add(this);
                        this.ApplyBorderForFloatWindow();
                    }
                    _leftValue = 0.0;
                    _topValue = 0.0;
                    window.Visibility = Visibility.Visible;
                    _w.Visibility = Visibility.Visible;
                    _w.DockingManager.RemoveDock(_w);
                    _w.DockState = DockState.Float;
                    if (window.DockManager.Parent is DockingManager)
                    {
                        _w.DockingManager.ShowDockbutton(window);
                    }
                    else
                    {
                        _w.DockingManager.RemoveDock(window);
                    }

                }
                else
                {
                    _w.DockState = DockState.Float;
                }
            }
            else
            {
                if (_w._Caption == string.Empty && _w.DockingManager.ActiveWindow != null)
                {
                    _w.DockingManager.ActiveWindow.ChangeState(DockState.Hidden);
                }
            }
        }

        /// <summary>
        /// Changes the dockable to floating.
        /// </summary>
        protected internal void ChangeDockableToFloating()
        {
            if (this._Caption != string.Empty)
            {
                if (this.CustomTabControl != null)
                {
                    if (this.CustomTabControl.Items.Count > 1)
                    {
                        CustomTabItem cstabitem = this.CustomTabControl.SelectedItem as CustomTabItem;
                        CustomTabControl cstab = cstabitem.Parent as CustomTabControl;
                        if (this.DockState == DockState.Float)
                        {
                            if (cstab.RelatedWindow.CustomTabControl.Items.Contains(cstabitem))
                            {

                                cstab.RelatedWindow.CustomTabControl.Items.Remove(cstabitem);
                                if (cstabitem.OwnWindow.CustomTabControl != null && cstab.RelatedWindow != cstabitem.OwnWindow)
                                {
                                    if (!cstabitem.OwnWindow.CustomTabControl.Items.Contains(cstabitem))
                                    {
                                        cstabitem.OwnWindow.CustomTabControl.Items.Add(cstabitem);
                                    }
                                }
                            }

                            if (cstab.RelatedWindow == cstabitem.OwnWindow)
                            {
                                this.DockingManager._parentTabbedWindow = cstab.RelatedWindow;
                                this.DockingManager.TabbedWindow = cstabitem.OwnWindow;
                                this.DockingManager.UpdateCustomTabItem();
                            }
                            else
                            {
                                this.DockingManager._parentTabbedWindow = cstab.RelatedWindow;
                                this.DockingManager.TabbedWindow = cstabitem.OwnWindow;
                                this.DockingManager.RemovedTabItem = cstabitem;
                                this.DockingManager.UpdateCustomTabItem();
                                if (cstab.RelatedWindow.CustomTabControl.SelectedItem != null)
                                {
                                    cstab.RelatedWindow.Caption = ((CustomTabItem)cstab.RelatedWindow.CustomTabControl.SelectedItem).Header.ToString();
                                }
                                else if (cstab.RelatedWindow.CustomTabControl.Items.Count == 1)
                                {
                                    cstab.RelatedWindow.Caption = ((CustomTabItem)cstab.RelatedWindow.CustomTabControl.Items[0]).Header.ToString();
                                }
                                if (cstabitem.OwnWindow.CustomTabControl.SelectedItem != null)
                                {
                                    cstabitem.OwnWindow.Caption = ((CustomTabItem)cstabitem.OwnWindow.CustomTabControl.SelectedItem).Header.ToString();
                                }
                                else if (cstabitem.OwnWindow.CustomTabControl.Items.Count == 1)
                                {
                                    cstabitem.OwnWindow.Caption = ((CustomTabItem)cstabitem.OwnWindow.CustomTabControl.Items[0]).Header.ToString();
                                }
                            }

                            _leftValue = 0.0;
                            _topValue = 0.0;
                            Grid temp = GetParent((UIElement)this.Parent);
                            _attachedParentWindow = null;
                            Window _w = GetParent(this);
                            if (this.Parent is Grid)
                            {
                                cstabitem.OwnWindow.Width = (this.Parent as Grid).ActualWidth;
                                cstabitem.OwnWindow.Height = (this.Parent as Grid).ActualHeight;
                            }
                            else
                            {
                                cstabitem.OwnWindow.DockingManager.SetFloatWidthAndHeightToWindow(cstabitem.OwnWindow);
                            }
                            cstabitem.OwnWindow.LeftPosition = _leftValue;
                            cstabitem.OwnWindow.TopPosition = _topValue;
                            cstabitem.OwnWindow.FloatHeight = cstabitem.OwnWindow.Height;
                            cstabitem.OwnWindow.FloatWidth = cstabitem.OwnWindow.Width;
                            Canvas.SetLeft(cstabitem.OwnWindow, cstabitem.OwnWindow.LeftPosition);
                            Canvas.SetTop(cstabitem.OwnWindow, cstabitem.OwnWindow.TopPosition);
                            if (!((Canvas)DockingManager).Children.Contains(cstabitem.OwnWindow))
                            {
                                ((Canvas)DockingManager).Children.Add(cstabitem.OwnWindow);
                                cstabitem.OwnWindow.ApplyBorderForFloatWindow();
                            }
                        }
                        else
                        {
                            this.DockingManager._parentTabbedWindow = cstab.RelatedWindow;
                            this.DockingManager.TabbedWindow = cstabitem.OwnWindow;
                            this.DockingManager.TabDoubleClick(cstabitem);
                        }
                        cstabitem.OwnWindow.Visibility = Visibility.Visible;
                        cstabitem.OwnWindow.OldValueDockManager = null;
                        cstabitem.OwnWindow.DockableState = DockableState.Floating;
                    }
                    else
                    {
                        if (this.DockState != DockState.Float)
                        {
                            DockManager dm = this.DockManager;
                            bool isWindowContainerPresent = false;
                            if (this.DockManager.Parent is WindowContainer)
                            {
                                isWindowContainerPresent = true;
                            }
                            else if (this.OldValueDockManager != null)
                            {
                                if (this.OldValueDockManager.Parent is WindowContainer)
                                {
                                    this.DockingManager.UpdateDockModeTarget(this, true);
                                }
                            }
                            this.DockState = DockState.Float;
                            if (isWindowContainerPresent)
                            {
                                this.DockingManager.UpdateMoveToTargetName(this, isWindowContainerPresent);
                            }

                            if (dm.Parent is WindowContainer)
                            {
                                Window windowContainer = (dm.Parent as WindowContainer)._window;
                                if (windowContainer.WindowCollection.Contains(this))
                                {
                                    windowContainer.WindowCollection.Remove(this);
                                }
                            }
                            this.OldValueDockManager = null;
                        }

                        else
                        {
                            DockManager dm = this.DockManager;
                            bool isWindowContainerPresent = false;
                            if (this.DockManager.Parent is WindowContainer)
                            {
                                isWindowContainerPresent = true;
                            }
                            else if (this.OldValueDockManager != null)
                            {
                                if (this.OldValueDockManager.Parent is WindowContainer)
                                {
                                    this.DockingManager.UpdateDockModeTarget(this, true);
                                }
                            }
                            SetFloatWindow();
                            if (isWindowContainerPresent)
                            {
                                this.DockingManager.UpdateMoveToTargetName(this, isWindowContainerPresent);

                                this.DockingManager.SetboolValueWithTargetName(this, string.Empty, DockState.Float);
                                DockingManager.SetTargetNameInFloatingMode(this.WindowChildElement, string.Empty);
                                this.MoveWindowTargetName = string.Empty;
                            }
                            if (dm.Parent is WindowContainer)
                            {
                                Window windowContainer = (dm.Parent as WindowContainer)._window;
                                if (windowContainer.WindowCollection.Contains(this))
                                {
                                    windowContainer.WindowCollection.Remove(this);
                                }
                            }
                            this.OldValueDockManager = null;
                        }
                        this.DockableState = DockableState.Floating;
                    }
                }
                else
                {
                    if (this.DockState != DockState.Float)
                    {
                        DockManager dm = this.DockManager;
                        bool isWindowContainerPresent = false;
                        if (this.DockManager.Parent is WindowContainer)
                        {
                            isWindowContainerPresent = true;
                        }
                        else if (this.OldValueDockManager != null)
                        {
                            if (this.OldValueDockManager.Parent is WindowContainer)
                            {
                                this.DockingManager.UpdateDockModeTarget(this, true);
                            }
                        }
                        this.DockState = DockState.Float;
                        if (isWindowContainerPresent)
                        {
                            this.DockingManager.UpdateMoveToTargetName(this, isWindowContainerPresent);
                        }
                        if (dm.Parent is WindowContainer)
                        {
                            Window windowContainer = (dm.Parent as WindowContainer)._window;
                            if (windowContainer.WindowCollection.Contains(this))
                            {
                                windowContainer.WindowCollection.Remove(this);
                            }
                        }
                        this.OldValueDockManager = null;
                    }

                    else
                    {
                        bool isWindowContainerPresent = false;
                        DockManager dm = this.DockManager;
                        if (this.DockManager.Parent is WindowContainer)
                        {
                            isWindowContainerPresent = true;
                        }
                        else if (this.OldValueDockManager != null)
                        {
                            if (this.OldValueDockManager.Parent is WindowContainer)
                            {
                                this.DockingManager.UpdateDockModeTarget(this, true);
                            }
                        }
                        SetFloatWindow();
                        if (isWindowContainerPresent)
                        {
                            this.DockingManager.UpdateMoveToTargetName(this, isWindowContainerPresent);
                        }

                        if (dm.Parent is WindowContainer)
                        {
                            Window windowContainer = (dm.Parent as WindowContainer)._window;
                            if (windowContainer.WindowCollection.Contains(this))
                            {
                                windowContainer.WindowCollection.Remove(this);
                            }
                        }
                        this.OldValueDockManager = null;
                    }
                    this.DockableState = DockableState.Floating;
                }
                if (this.DockingManager != null && this.DockingManager.TabbedWindow!= null && this.DockingManager.TabbedWindow.DockState == DockState.Float)
                {
                    if (this.DockingManager.TabbedWindow.WindowChildElement != null)
                    {
                        DockingManager.SetTargetNameInFloatingMode(this.DockingManager.TabbedWindow.WindowChildElement, string.Empty);
                        DockingManager.SetSideInFloatMode(this.DockingManager.TabbedWindow.WindowChildElement, Dock.Left);
                    }
                }
            }
        }

        /// <summary>
        /// Floatings the state menu.
        /// </summary>
        protected internal void FloatingStateMenu()
        {
            if (this.CustomTabControl != null)
            {
                if (this.CustomTabControl.SelectedItem != null)
                {
                    this.DockingManager.RemovedTabItem = this.CustomTabControl.SelectedItem as CustomTabItem;
                }
                else if (this.CustomTabControl.Items.Count > 0)
                {
                    this.DockingManager.RemovedTabItem = this.CustomTabControl.Items[0] as CustomTabItem;
                }
            }
            if (this._Caption != string.Empty)
            {
                ChangeDockableToFloating();
            }
            else
            {
                if (this._Caption == string.Empty)
                {
                    if (this.WindowCollection.Count > 0)
                    {
                        foreach (Window w in this.WindowCollection)
                        {
                            if (this.DockingManager.ActiveWindow != null)
                            {
                                if (this.DockingManager.ActiveWindow == w)
                                {
                                    w.ChangeDockableToFloating();
                                    break;
                                }
                            }
                        }
                    }

                    //SetFloatWindow();
                }

            }
            if (this.DockingManager.RemovedTabItem != null)
            {
                this.DockingManager.HideTabPanel(this.DockingManager.RemovedTabItem.OwnWindow);
            }
        }

        /// <summary>
        /// Dockings the state menu.
        /// </summary>
        protected internal void DockingStateMenu()
        {
            if (DockingManager.GetCanDock(WindowChildElement as UIElement))
            {
                this.CanDock = true;
            }
            else
            {
                this.CanDock = false;
            }
            this.DockableState = DockableState.Dockable;
        }

        /// <summary>
        /// Autoes the hide state menu.
        /// </summary>
        protected internal void AutoHideStateMenu()
        {
            if (this.DockState == DockState.AutoHidden)
            {
                this.dockToggle.IsChecked = false;
            }
            else
            {
                if (this.dockToggle.Visibility == Visibility.Visible)
                {
                    this.DockableState = DockableState.AutoHide;
                    this.ChangeState(DockState.AutoHidden);
                    this.DockingManager.UpdateSidePanelLayout();
                }
            }

        }

        /// <summary>
        /// Handles the MouseLeftButtonDown event of the MenuItemAdv control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        void MenuItemAdv_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MenuItemAdv menuIemAdv = (MenuItemAdv)sender;
            if (menuIemAdv.Header.ToString().ToLower() == "floating")
            {
                Window temp = null;
                DockManager dm = null;
                if (this.CustomTabControl != null)
                {
                    if (this.CustomTabControl.Items.Count > 1 && this.CustomTabControl.SelectedItem != null)
                    {
                        CustomTabItem cs = this.CustomTabControl.SelectedItem as CustomTabItem;
                        if (cs.OwnWindow.CanFloat)
                        {
                            temp = cs.OwnWindow;
                            if (cs.OwnWindow.DockManager != null)
                            {
                                if (cs.OwnWindow.DockManager.Parent is WindowContainer)
                                {
                                    dm = cs.OwnWindow.DockManager;
                                }
                                else if (cs.OwnWindow.OldValueDockManager != null)
                                {
                                    if (cs.OwnWindow.OldValueDockManager.Parent is WindowContainer)
                                    {
                                        dm = cs.OwnWindow.OldValueDockManager;
                                    }
                                }
                            }
                            DockStateChangingEventArgs args = new DockStateChangingEventArgs(cs.OwnWindow.WindowChildElement, cs.OwnWindow.DockState, DockState.Float, DockSide.Left);
                            DockState dockState = cs.OwnWindow.DockState;
                            if(cs.OwnWindow.DockState!=DockState.Float)
                                this.DockingManager.FireDockStateChanging(args);
                            if (!args.Cancel)
                            {
                                FloatingStateMenu();
                                this.DockingManager.FireDockStateChanged(this.WindowChildElement, dockState, DockState.Float);
                            }
                        }
                    }
                    else
                    {
                        if (this.CanFloat)
                        {
                            temp = this;
                            if (this.DockManager != null)
                            {
                                if (this.DockManager.Parent is WindowContainer)
                                {
                                    dm = this.DockManager;
                                }
                                else if (this.OldValueDockManager != null)
                                {
                                    if (this.OldValueDockManager.Parent is WindowContainer)
                                    {
                                        dm = this.OldValueDockManager;
                                    }
                                }
                            }
                            DockStateChangingEventArgs args = new DockStateChangingEventArgs(this.WindowChildElement, this.DockState, DockState.Float, DockSide.Left);
                            DockState dockState = this.DockState;
                            if (this.DockState != DockState.Float)
                                this.DockingManager.FireDockStateChanging(args);
                            if (!args.Cancel)
                            {
                                FloatingStateMenu();
                                this.DockingManager.FireDockStateChanged(this.WindowChildElement, dockState, DockState.Float);
                            }
                        }
                    }
                }
                else
                {
                    temp = this;
                    if (this.DockManager != null)
                    {
                        if (this.DockManager.Parent is WindowContainer)
                        {
                            dm = this.DockManager;
                        }
                        else if (this.OldValueDockManager != null)
                        {
                            if (this.OldValueDockManager.Parent is WindowContainer)
                            {
                                dm = this.OldValueDockManager;
                            }
                        }
                    }
                    FloatingStateMenu();
                }
                if (dm != null)
                {
                    if (dm.Parent is WindowContainer)
                    {
                        IEnumerable<Window> windowquery = (dm.Parent as WindowContainer)._window.WindowCollection.Where(tempwindow => ((Window)tempwindow).Visibility == Visibility.Visible && ((Window)tempwindow).CustomTabControl.Items.Count > 0 && DockingManager.GetSideInFloatMode(((Window)tempwindow).WindowChildElement) != Dock.Tabbed);
                        if (windowquery.Count() == 1)
                        {
                            foreach (Window w in windowquery)
                            {
                                this.DockingManager.SetboolValueWithTargetName(w, string.Empty, DockState.Float);
                                DockingManager.SetTargetNameInFloatingMode(w.WindowChildElement, string.Empty);
                            }
                        }

                        this.DockingManager.UpdateMoveToTargetName(temp, true);
                        Window _windowCollection = (dm.Parent as WindowContainer)._window;
                        temp.DockManager = this.DockingManager.GetParentDockManager()._dockManager;
                        temp.OldValueDockManager = null;
                        if (_windowCollection.WindowCollection.Contains(temp))
                        {
                            _windowCollection.WindowCollection.Remove(temp);
                        }
                    }
                    this.DockingManager.SetboolValueWithTargetName(temp, string.Empty, DockState.Float);
                    DockingManager.SetTargetNameInFloatingMode(temp.WindowChildElement, string.Empty);
                }
                if (this.CustomTabControl != null)
                {
                    if (this.CustomTabControl.Items.Count > 1 && this.CustomTabControl.SelectedItem != null)
                    {
                        if (temp != null)
                        {
                            Canvas.SetZIndex(temp, ++currentZIndex);
                        }
                    }
                    else
                    {
                        Canvas.SetZIndex(this, ++currentZIndex);
                    }
                }
            }
            else if (menuIemAdv.Header.ToString().ToLower() == "dockable")
            {
                if (this.CustomTabControl != null)
                {
                    if (this.CustomTabControl.Items.Count > 1 && this.CustomTabControl.SelectedItem != null)
                    {
                        CustomTabItem cs = this.CustomTabControl.SelectedItem as CustomTabItem;
                        if (cs.OwnWindow.CanDock)
                        {
                            DockingStateMenu();
                        }
                    }
                    else
                    {
                        if (this.CanDock)
                        {
                            DockingStateMenu();
                        }
                    }
                }
                else
                {
                    DockingStateMenu();
                }
                //this.DockState = DockState.Dock;
            }
            else if (menuIemAdv.Header.ToString().ToLower() == "tabbed")
            {
                this.DockableState = DockableState.Tabbed;
            }
            else if (menuIemAdv.Header.ToString().ToLower() == "auto hide")
            {
                if (this.CanAutoHide)
                {
                    DockStateChangingEventArgs args = new DockStateChangingEventArgs(this.WindowChildElement, this.DockState, DockState.AutoHidden, DockSide.Left);
                    DockState dockState = this.DockState;
                    this.DockingManager.FireDockStateChanging(args);
                    if (!args.Cancel)
                    {
                        AutoHideStateMenu();
                        this.DockingManager.FireDockStateChanged(this.WindowChildElement, dockState, DockState.AutoHidden);
                    }
                }
            }
            else if (menuIemAdv.Header.ToString().ToLower() == "hide")
            {
                DockStateChangingEventArgs args = new DockStateChangingEventArgs(this.WindowChildElement, this.DockState, DockState.Hidden,DockSide.Left);
                DockState dockState = this.DockState;
                this.DockingManager.FireDockStateChanging(args);
                if (!args.Cancel)
                {
                    if (this.CustomTabControl != null)
                    {
                        if (this.CustomTabControl.Items.Count > 1 && this.CustomTabControl.SelectedItem != null)
                        {
                            CustomTabItem cs = this.CustomTabControl.SelectedItem as CustomTabItem;
                            if (cs.OwnWindow.CanClose)
                            {
                                CommonMethodForHideClose();
                            }
                        }
                        else
                        {
                            if (this.CanClose)
                            {
                                CommonMethodForHideClose();
                            }
                        }
                    }
                    else
                    {
                        CommonMethodForHideClose();
                    }
                    this.DockingManager.FireDockStateChanged(this.WindowChildElement, dockState, DockState.Hidden);
                }

                //this.DockingManager.UpdateSidePanelLayout();
            }
        }

        /// <summary>
        /// Handles the SelectionChanged event of the l control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> instance containing the event data.</param>
        void l_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox lbox = (ListBox)sender;
            ListBoxItem lb = (ListBoxItem)lbox.SelectedItem;
            VisualStateManager.GoToState(lb, "ShowTick", true);
            if (lb.Content.ToString() == "Floating" && lb.Opacity != 0.5)
            {
                if (this.CustomTabControl != null)
                {
                    if (this.CustomTabControl.SelectedItem != null)
                    {
                        this.DockingManager.RemovedTabItem = this.CustomTabControl.SelectedItem as CustomTabItem;
                    }
                    else if (this.CustomTabControl.Items.Count > 0)
                    {
                        this.DockingManager.RemovedTabItem = this.CustomTabControl.Items[0] as CustomTabItem;
                    }
                }
                if (this._Caption != string.Empty && lb.Opacity != 0.5)
                {
                    ChangeDockableToFloating();
                }
                else
                {
                    if (this._Caption == string.Empty)
                    {
                        if (this.WindowCollection.Count > 0)
                        {
                            foreach (Window w in this.WindowCollection)
                            {
                                if (this.DockingManager.ActiveWindow != null)
                                {
                                    if (this.DockingManager.ActiveWindow == w)
                                    {
                                        w.ChangeDockableToFloating();
                                        break;
                                    }
                                }
                            }
                        }

                        //SetFloatWindow();
                    }

                }
            }
            else if (lb.Content.ToString() == "Dockable" && lb.Opacity != 0.5)
            {
                this.CanDock = true;
                this.DockableState = DockableState.Dockable;
                //this.DockState = DockState.Dock;
            }
            else if (lb.Content.ToString() == "Tabbed Document" && lb.Opacity != 0.5)
            {
                this.DockableState = DockableState.Tabbed;
            }
            else if (lb.Content.ToString() == "AutoHide" && lb.Opacity != 0.5)
            {
                if (this.dockToggle.Visibility == Visibility.Visible)
                {
                    this.DockableState = DockableState.AutoHide;
                    this.ChangeState(DockState.AutoHidden);
                    this.DockingManager.UpdateSidePanelLayout();
                }
            }
            else if (lb.Content.ToString() == "Hide" && lb.Opacity != 0.5)
            {
                CommonMethodForHideClose();
            }

            Popup p = (Popup)((ListBox)sender).Parent;
            ((ListBox)sender).Visibility = Visibility.Collapsed;
            p.IsOpen = false;
        }


        /// <summary>
        /// Removes the active window.
        /// </summary>
        /// <param name="_w">The _w.</param>
        protected internal void RemoveActiveWindow(Window _w)
        {
            Window window = null;
            if (_w.CustomTabControl != null && (CustomTabItem)_w.CustomTabControl.SelectedItem != null)
            {
                CustomTabItem custabItem = (CustomTabItem)_w.CustomTabControl.SelectedItem;
                if (((CustomTabItem)_w.CustomTabControl.SelectedItem).OwnWindow == _w)
                {
                    if (_w.CustomTabControl.Items.Count > 1)
                    {
                        CustomTabItem cusTab = null;
                        if (_w.CustomTabControl.SelectedIndex != _w.CustomTabControl.Items.Count - 1)
                        {
                            cusTab = (CustomTabItem)_w.CustomTabControl.Items[_w.CustomTabControl.SelectedIndex + 1];
                        }
                        else
                        {
                            cusTab = (CustomTabItem)_w.CustomTabControl.Items[0];
                        }

                        for (int i = 1; i <= _w.DockingManager.WindowCollection.Count; i++)
                        {
                            if (_w.DockingManager.WindowCollection[i].GetType() == typeof(Window))
                            {
                                if (cusTab.OwnWindow == _w.DockingManager.WindowCollection[i])
                                {
                                    window = _w.DockingManager.WindowCollection[i];
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (custabItem != null)
                        {
                            if (_w.DockManager.Parent.GetType() == typeof(WindowContainer))
                            {
                                SetHiddenStateWindow();
                            }
                            else
                            {
                                _w.ChangeState(DockState.Hidden);
                                StateMaintanance st = custabItem.OwnWindow.CurrentStateMain;
                                if (st != StateMaintanance.Float && st != StateMaintanance.WindowContainer)
                                {
                                    custabItem.OwnWindow.CurrentStateMain = custabItem.OwnWindow.PreviousStateMain;
                                    custabItem.OwnWindow.PreviousStateMain = st;
                                }
                                custabItem.OwnWindow.DockState = DockState.Hidden;
                                if (DockingManager.GetDockState(custabItem.OwnWindow.WindowChildElement) != DockState.Hidden)
                                {
                                    custabItem.OwnWindow.InternalllyRaisedDockStateChanged = true;
                                }
                                DockingManager.SetDockState(custabItem.OwnWindow.WindowChildElement, DockState.Hidden);
                            }
                        }

                        window = null;
                    }
                }
                else
                {
                    window = null;
                    _w.CustomTabControl.Items.Remove(custabItem);
                    if (custabItem.OwnWindow.WindowChildElement != null)
                    {
                        if (custabItem.OwnWindow.CustomTabControl != null)
                        {
                            custabItem.OwnWindow.CustomTabControl.Items.Add(custabItem);
                        }
                        StateMaintanance st = custabItem.OwnWindow.CurrentStateMain;
                        if (st != StateMaintanance.Float && st != StateMaintanance.WindowContainer)
                        {
                            custabItem.OwnWindow.CurrentStateMain = custabItem.OwnWindow.PreviousStateMain;
                            custabItem.OwnWindow.PreviousStateMain = st;
                        }
                        custabItem.OwnWindow.DockState = DockState.Hidden;
                        if (DockingManager.GetDockState(custabItem.OwnWindow.WindowChildElement) != DockState.Hidden)
                        {
                            custabItem.OwnWindow.InternalllyRaisedDockStateChanged = true;
                        }
                        DockingManager.SetDockState(custabItem.OwnWindow.WindowChildElement, DockState.Hidden);

                    }
                    _w.Caption = ((CustomTabItem)_w.CustomTabControl.SelectedItem).Header.ToString().Trim();
                    if (_w.CustomTabControl.Items.Count == 1)
                    {
                        if (_w.CustomTabControl.primitiveTabPanel != null)
                        {
                            _w.CustomTabControl.primitiveTabPanel.Visibility = Visibility.Collapsed;
                        }
                    }
                }
            }

            if (window != null)
            {
                window.Visibility = Visibility.Visible;
                CustomTabItem custabItem = (CustomTabItem)_w.CustomTabControl.SelectedItem;
                for (int m = _w.CustomTabControl.Items.Count - 1; m >= 0; m--)
                {
                    CustomTabItem cusTab = (CustomTabItem)_w.CustomTabControl.Items[m];
                    if (custabItem != cusTab)
                    {
                        _w.CustomTabControl.Items.Remove(cusTab);
                        window.CustomTabControl.Items.Insert(0, cusTab);
                        if (cusTab.OwnWindow == window)
                        {
                            window.CustomTabControl.SelectedItem = cusTab;
                        }
                    }
                }
                _w.DockingManager.UpdateMoveToTargetName(window, _w);
                if (_w.DockManager.Parent.GetType() == typeof(WindowContainer))
                {
                    Canvas.SetZIndex((_w.DockManager.Parent as WindowContainer)._window, 1);
                    _w.DockingManager.UpdateTargetNameForFloatWindow(_w, window);
                }
                else
                {
                    _w.DockingManager.UpdateTargetName(_w, window);
                }

                window.CustomTabControl.TabPanelBackground = _w.DockingManager.TabPanelBackground;
                window.PaneHeight = _w.ActualHeight;
                window.PaneWidth = _w.ActualWidth;
                _w.DockState = DockState.Hidden;

                (_w.DockManager.Children[0] as DockingGrid).ReplaceChild(window, _w, _w.DockPosition);
                ////(_w.DockManager.Children[0] as DockingGrid).ChangeTabOrder(_w, window);

                if (((Canvas)_w.DockingManager).Children.Contains(_w))
                {
                    ((Canvas)_w.DockingManager).Children.Remove(_w);
                }
                if (custabItem.OwnWindow.WindowChildElement != null)
                {
                    StateMaintanance st = custabItem.OwnWindow.CurrentStateMain;
                    if (st != StateMaintanance.Float && st != StateMaintanance.WindowContainer)
                    {
                        custabItem.OwnWindow.CurrentStateMain = custabItem.OwnWindow.PreviousStateMain;
                        custabItem.OwnWindow.PreviousStateMain = st;
                    }
                    custabItem.OwnWindow.DockState = DockState.Hidden;
                    if (DockingManager.GetDockState(custabItem.OwnWindow.WindowChildElement) != DockState.Hidden)
                    {
                        custabItem.OwnWindow.InternalllyRaisedDockStateChanged = true;
                    }
                    DockingManager.SetDockState(custabItem.OwnWindow.WindowChildElement, DockState.Hidden);
                }
            }
            else
            {
                if (_w._Caption == string.Empty && _w.DockingManager.ActiveWindow != null)
                {
                    _w.DockingManager.ActiveWindow.ChangeState(DockState.Hidden);
                }
            }
        }

        /// <summary>
        /// Commons the method for hide close.
        /// </summary>
        protected internal void CommonMethodForHideClose()
        {
            int counter = 0;
            bool allowMe = false;
            if (this.DockState == DockState.Float)
            {
                allowMe = true;
            }
            if (this.DockManager != null)
            {
                counter = (this.DockManager.Children[0] as DockingGrid).gridDocking.Children.Count;
            }
            if (this.dockToggle != null)
            {
                if (DockingManager.GetDockState(this.WindowChildElement) != DockState.Float)//(this.dockToggle.Visibility == Visibility.Visible)
                {
                    if (!(bool)this.dockToggle.IsChecked)
                    {
                        RemoveActiveWindow(this);
                    }
                    else
                    {
                        Window childWwindow = null;
                        if (this.CustomTabControl != null && (CustomTabItem)this.CustomTabControl.SelectedItem != null)
                        {
                            CustomTabItem custabItem = (CustomTabItem)this.CustomTabControl.SelectedItem;
                            if (((CustomTabItem)this.CustomTabControl.SelectedItem).OwnWindow == this)
                            {
                                if (this.CustomTabControl.Items.Count > 1)
                                {
                                    CustomTabItem cusTab = null;
                                    if (this.CustomTabControl.SelectedIndex != this.CustomTabControl.Items.Count - 1)
                                    {
                                        cusTab = (CustomTabItem)this.CustomTabControl.Items[this.CustomTabControl.SelectedIndex + 1];
                                    }
                                    else
                                    {
                                        cusTab = (CustomTabItem)this.CustomTabControl.Items[0];
                                    }

                                    for (int i = 1; i <= this.DockingManager.WindowCollection.Count; i++)
                                    {
                                        if (this.DockingManager.WindowCollection[i].GetType() == typeof(Window))
                                        {
                                            if (cusTab.OwnWindow == this.DockingManager.WindowCollection[i])
                                            {
                                                childWwindow = this.DockingManager.WindowCollection[i];
                                                break;
                                            }
                                        }
                                    }
                                    RemoveActiveWindow(this);
                                }
                            }
                        }
                        if (childWwindow != null)
                        {
                            switch (this.DockPosition)
                            {
                                case Dock.Left:
                                    this.DockingManager.btnPaneLeft.Children.Clear();
                                    break;
                                case Dock.Right:
                                    this.DockingManager.btnPaneRight.Children.Clear();
                                    break;
                                case Dock.Top:
                                    this.DockingManager.btnPaneTop.Children.Clear();
                                    break;
                                case Dock.Bottom:
                                    this.DockingManager.btnPaneBottom.Children.Clear();
                                    break;
                            }
                            if (childWwindow.DockingManager.tabNameCollection.Contains(this._Caption.ToString().Trim()))
                            {
                                childWwindow.DockingManager.tabNameCollection.Remove(this._Caption.ToString().Trim());
                            }
                            for (int i = 0; i < childWwindow.CustomTabControl.Items.Count; i++)
                            {
                                if (childWwindow.DockingManager.tabNameCollection.Contains((childWwindow.CustomTabControl.Items[i] as CustomTabItem).OwnWindow._Caption.ToString().Trim()))
                                {
                                    childWwindow.DockingManager.tabNameCollection.Remove((childWwindow.CustomTabControl.Items[i] as CustomTabItem).OwnWindow._Caption.ToString().Trim());
                                }
                            }
                            if (((Canvas)DockingManager).Children.Contains(this))
                            {
                                ((Canvas)DockingManager).Children.Remove(this);
                            }
                            this.DockState = DockState.Hidden;
                            if (DockingManager.GetDockState(this.WindowChildElement) != DockState.Hidden)
                            {
                                this.InternalllyRaisedDockStateChanged = true;
                            }
                            DockingManager.SetDockState(this.WindowChildElement, DockState.Hidden);
                            childWwindow.DockState = DockState.AutoHidden;
                            if (childWwindow.DockManager.gridDocking != null)
                            {
                                childWwindow.DockManager.gridDocking.ArrangeLayout();
                            }
                        }
                        else
                        {
                            if ((bool)this.dockToggle.IsChecked)
                            {
                                SidePanel m_sideGrid = null;
                                Grid grid = null;
                                switch (this.DockPosition)
                                {
                                    case Dock.Left:
                                        m_sideGrid = this.DockingManager.btnPaneLeft;
                                        grid = this.DockingManager.m_leftSideGrid;
                                        break;
                                    case Dock.Right:
                                        m_sideGrid = this.DockingManager.btnPaneRight;
                                        grid = this.DockingManager.m_rightSideGrid;
                                        break;
                                    case Dock.Top:
                                        m_sideGrid = this.DockingManager.btnPaneTop;
                                        grid = this.DockingManager.m_topSideGrid;
                                        break;
                                    case Dock.Bottom:
                                        m_sideGrid = this.DockingManager.btnPaneBottom;
                                        grid = this.DockingManager.m_bottomSideGrid;
                                        break;
                                }

                                if (this.DockingManager != null)
                                {
                                    if (this.DockingManager.RecentlyMouseHoveredSidePanel != null)
                                    {
                                        this.DockingManager.UpdateTabSideGridOrder(ref grid, this.DockPosition, m_sideGrid, this.DockingManager.RecentlyMouseHoveredSidePanel, this);
                                        this.DockingManager.UpdateSideGridChildren(grid, m_sideGrid);
                                    }
                                }

                                if (this.CustomTabControl.Items.Count > 1)
                                {
                                    for (int i = 0; i < this.CustomTabControl.Items.Count; i++)
                                    {
                                        if (((CustomTabItem)this.CustomTabControl.Items[i]).OwnWindow == this.DockingManager.RecentlyMouseHoveredSidePanel.OwnWindow)
                                        {
                                            CustomTabItem cstabItem = this.CustomTabControl.Items[i] as CustomTabItem;
                                            this.CustomTabControl.Items.RemoveAt(i);
                                            if (this.DockingManager.RecentlyMouseHoveredSidePanel != null)
                                            {
                                                if (this.DockingManager.RecentlyMouseHoveredSidePanel.OwnWindow.CustomTabControl != null)
                                                {
                                                    if (!this.DockingManager.RecentlyMouseHoveredSidePanel.OwnWindow.CustomTabControl.Items.Contains(cstabItem))
                                                    {
                                                        this.DockingManager.RecentlyMouseHoveredSidePanel.OwnWindow.CustomTabControl.Items.Add(cstabItem);
                                                    }
                                                }
                                            }
                                            if (this.DockingManager.tabNameCollection.Contains(this.DockingManager.RecentlyMouseHoveredSidePanel.OwnWindow._Caption.ToString().Trim()))
                                            {
                                                this.DockingManager.tabNameCollection.Remove(this.DockingManager.RecentlyMouseHoveredSidePanel.OwnWindow._Caption.ToString().Trim());
                                            }
                                            ////this.Caption = ((CustomTabItem)this.CustomTabControl.SelectedItem).Header.ToString().Trim();      
                                            if (((Canvas)DockingManager).Children.Contains(this.DockingManager.RecentlyMouseHoveredSidePanel.OwnWindow))
                                            {
                                                ((Canvas)DockingManager).Children.Remove(this.DockingManager.RecentlyMouseHoveredSidePanel.OwnWindow);
                                            }
                                            ((Canvas)DockingManager).Children.Remove(this);
                                            if (this.WindowChildElement != null)
                                            {
                                                StateMaintanance st = this.CurrentStateMain;
                                                if (st != StateMaintanance.Float && st != StateMaintanance.WindowContainer)
                                                {
                                                    this.CurrentStateMain = this.PreviousStateMain;
                                                    this.PreviousStateMain = st;
                                                }
                                                this.DockingManager.RecentlyMouseHoveredSidePanel.OwnWindow.DockState = DockState.Hidden;
                                                if (DockingManager.GetDockState(this.DockingManager.RecentlyMouseHoveredSidePanel.OwnWindow.WindowChildElement) != DockState.Hidden)
                                                {
                                                    this.DockingManager.RecentlyMouseHoveredSidePanel.OwnWindow.InternalllyRaisedDockStateChanged = true;
                                                }
                                                DockingManager.SetDockState(this.DockingManager.RecentlyMouseHoveredSidePanel.OwnWindow.WindowChildElement, DockState.Hidden);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    ((Canvas)DockingManager).Children.Remove(this);
                                    if (this.WindowChildElement != null)
                                    {
                                        StateMaintanance st = this.CurrentStateMain;
                                        if (st != StateMaintanance.Float && st != StateMaintanance.WindowContainer)
                                        {
                                            this.CurrentStateMain = this.PreviousStateMain;
                                            this.PreviousStateMain = st;
                                        }
                                        this.DockState = DockState.Hidden;
                                        if (DockingManager.GetDockState(this.WindowChildElement) != DockState.Hidden)
                                        {
                                            this.InternalllyRaisedDockStateChanged = true;
                                        }
                                        DockingManager.SetDockState(this.WindowChildElement, DockState.Hidden);
                                    }
                                    this.DockingManager.UpdateSidePanelLayout();
                                }
                                ////this.DockingManager.UpdateSidePanelLayout();
                            }
                        }
                    }
                }
                else
                {
                    if (((Canvas)DockingManager).Children.Contains(this))
                    {
                        if (this._Caption == string.Empty)
                        {
                            if (this._Caption == string.Empty && this.DockingManager.ActiveWindow != null)
                            {
                                //this.DockingManager.ActiveWindow.ChangeState(DockState.Hidden);
                                RemoveActiveWindow(this.DockingManager.ActiveWindow);
                            }
                        }
                        else
                        {
                            ((Canvas)DockingManager).Children.Remove(this);
                            if (this.WindowChildElement != null)
                            {
                                StateMaintanance st = this.CurrentStateMain;
                                if (st != StateMaintanance.Float && st != StateMaintanance.WindowContainer)
                                {
                                    this.CurrentStateMain = this.PreviousStateMain;
                                    this.PreviousStateMain = st;
                                }
                                this.DockState = DockState.Hidden;
                                if (DockingManager.GetDockState(this.WindowChildElement) != DockState.Hidden)
                                {
                                    this.InternalllyRaisedDockStateChanged = true;
                                }
                                DockingManager.SetDockState(this.WindowChildElement, DockState.Hidden);
                            }
                        }
                    }
                    else
                    {
                        RemoveActiveWindow(this);
                    }
                }
            }
            else
            {
                RemoveActiveWindow(this);
            }
            //if (this.DockingManager.DockFill)
            //{                
            //    if (this.DockingManager.DockFill && counter == 1 && !itemsPresent)
            //    {
            //        //(this.DockManager.Children[0] as DockingGrid).gridDocking.Children.Clear();
            //    }
            //}
            if (this.CustomTabControl != null && allowMe)
            {
                foreach (CustomTabItem cstabitem in this.CustomTabControl.Items)
                {
                    if (cstabitem.OwnWindow != this)
                    {
                        cstabitem.OwnWindow.DockState = DockState.Hidden;
                        if (DockingManager.GetDockState(cstabitem.OwnWindow.WindowChildElement) != DockState.Hidden)
                        {
                            cstabitem.OwnWindow.InternalllyRaisedDockStateChanged = true;
                        }
                        DockingManager.SetDockState(cstabitem.OwnWindow.WindowChildElement, DockState.Hidden);
                    }
                }
            }

        }

        /// <summary>
        /// Handles the MouseLeave event of the tab control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        void tab_MouseLeave(object sender, MouseEventArgs e)
        {
            Popup p = (Popup)((ListBox)sender).Parent;
            ((ListBox)sender).Visibility = Visibility.Collapsed;
            p.IsOpen = false;
        }

        /// <summary>
        /// Handles the MouseLeave event of the p control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        void p_MouseLeave(object sender, MouseEventArgs e)
        {
            Popup p = (Popup)sender;
            p.IsOpen = false;
        }

        #region HorizontalScrollBarVisibility

        /// <summary>
        /// Gets or sets the HorizontalScrollBarVisibility possible Value of the ScrollBarVisibility object.
        /// </summary>
        /// <value>The horizontal scroll bar visibility.</value>
        public ScrollBarVisibility HorizontalScrollBarVisibility
        {
            get
            {
                return (ScrollBarVisibility)GetValue(HorizontalScrollBarVisibilityProperty);
            }

            set
            {
                SetValue(HorizontalScrollBarVisibilityProperty, value);
            }
        }

        /// <summary> 
        /// Identifies the HorizontalScrollBarVisibility dependency property.
        /// </summary> 
        public static readonly DependencyProperty HorizontalScrollBarVisibilityProperty =
                        DependencyProperty.Register(
                                "HorizontalScrollBarVisibility",
                                typeof(ScrollBarVisibility),
                                typeof(Window),
                                new PropertyMetadata(OnHorizontalScrollBarVisibilityPropertyChanged));

        /// <summary>
        /// HorizontalScrollBarVisibilityProperty property changed handler. 
        /// </summary>
        /// <param name="d">Window that changed its HorizontalScrollBarVisibility.</param>
        /// <param name="e">Dependency Property Changed EventArgs.</param> 
        private static void OnHorizontalScrollBarVisibilityPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Window _window = d as Window;
            if (_window != null)
            {
                if (_window.scrollcontent != null)
                {
                    _window.scrollcontent.HorizontalScrollBarVisibility = (ScrollBarVisibility)e.NewValue;
                }
            }
        }
        #endregion HorizontalScrollBarVisibility

        #region VerticalScrollBarVisibility

        /// <summary>
        /// Gets or sets the VerticalScrollBarVisibility possible Value of the ScrollBarVisibility object.
        /// </summary>
        /// <value>The vertical scroll bar visibility.</value>
        public ScrollBarVisibility VerticalScrollBarVisibility
        {
            get
            {
                return (ScrollBarVisibility)GetValue(VerticalScrollBarVisibilityProperty);
            }

            set
            {
                SetValue(VerticalScrollBarVisibilityProperty, value);
            }
        }

        /// <summary> 
        /// Identifies the VerticalScrollBarVisibility dependency property.
        /// </summary> 
        public static readonly DependencyProperty VerticalScrollBarVisibilityProperty =
                        DependencyProperty.Register(
                                "VerticalScrollBarVisibility",
                                typeof(ScrollBarVisibility),
                                typeof(Window),
                                new PropertyMetadata(OnVerticalScrollBarVisibilityPropertyChanged));

        /// <summary>
        /// VerticalScrollBarVisibilityProperty property changed handler. 
        /// </summary>
        /// <param name="d">Window that changed its VerticalScrollBarVisibility.</param>
        /// <param name="e">Dependency Property Changed EventArgs.</param> 
        private static void OnVerticalScrollBarVisibilityPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Window _window = d as Window;
            if (_window != null)
            {
                if (_window.scrollcontent != null)
                {
                    _window.scrollcontent.VerticalScrollBarVisibility = (ScrollBarVisibility)e.NewValue;
                }
            }
        }
        #endregion VerticalScrollBarVisibility

        /// <summary>
        /// Occurs when [closed].
        /// </summary>
        public event EventHandler Closed;

        /// <summary>
        /// Closes this instance.
        /// </summary>
        public void Close()
        {
            bool allowMe = false;
            if (this.CustomTabControl.Items.Count > 1 && this.CustomTabControl.SelectedItem != null)
            {
                CustomTabItem cs = this.CustomTabControl.SelectedItem as CustomTabItem;
                if (cs.OwnWindow.CanClose)
                {
                    allowMe = true;
                }
            }
            else
            {
                if (this.CanClose)
                {
                    allowMe = true;
                }
            }
            if (allowMe)
            {
                if (Closed != null)
                {
                    Closed(this, EventArgs.Empty);
                }
                else
                {
                    CommonMethodForHideClose();
                }
            }
        }

        /// <summary>
        /// Windows the un pinned.
        /// </summary>
       protected internal void WindowUnPinned()
        {
            if (this.DockingManager != null)
            {
                if (this._Caption != string.Empty && this.CustomTabControl != null && this.CustomTabControl.SelectedItem != null)
                {
                    this.DockingManager.ActiveWindow = (this.CustomTabControl.SelectedItem as CustomTabItem).OwnWindow;
                }
            }
            bool evetnCanceled = false;
            if (this.DockingManager != null)
            {
                //this.DockingManager.InvokePreviewEventforUnPinned(this.WindowChildElement, ref evetnCanceled);
            }
            if (!evetnCanceled)
            {
                this.Height = double.NaN;
                this.Width = double.NaN;
                if (this._Caption != string.Empty)
                {
                    this.DockingManager.RemoveSideGrid(this);
                }

                this.ChangeState(DockState.Dock);
                if (this.CustomTabControl != null)
                {
                    if (this.CustomTabControl.primitiveTabPanel != null)
                    {
                        if (this.CustomTabControl.Items.Count <= 1)
                        {
                            this.CustomTabControl.primitiveTabPanel.Visibility = Visibility.Collapsed;
                        }
                        else
                        {
                            this.CustomTabControl.primitiveTabPanel.Visibility = Visibility.Visible;
                        }
                    }
                }

                this.DockingManager.stackPanelLeave = false;
                if (this.DockManager.Parent.GetType() == typeof(WindowContainer))
                {
                    //(this.DockManager.Parent as WindowContainer)._window.ChangeState(DockState.Dock);
                    GetDockParentWindowContainer(this);
                }

                this.DockingManager.UpdateSidePanelLayout();
                Canvas.SetZIndex(this, 1);
            }
            else
            {
                //this.dockToggle.IsChecked = true;                
            }
            if (this.CustomTabControl != null)
            {
                foreach (CustomTabItem csTabItem in this.CustomTabControl.Items)
                {
                    if (csTabItem.OwnWindow != this)
                    {
                        csTabItem.OwnWindow.DockState = DockState.Dock;
                    }
                }
            }
        }

       /// <summary>
       /// Handles the Unpinned event of the dockToggle control.
       /// </summary>
       /// <param name="sender">The source of the event.</param>
       /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void dockToggle_Unpinned(object sender, RoutedEventArgs e)
        {
            if (!pinnedCancelled)
            {
                if (this.DockingManager != null)
                {
                    if (this._Caption != string.Empty && this.CustomTabControl != null && this.CustomTabControl.SelectedItem != null)
                    {
                        this.DockingManager.ActiveWindow = (this.CustomTabControl.SelectedItem as CustomTabItem).OwnWindow;
                    }
                }
                #region Sample

                ////this.PaneHeight = this.Height;
                ////this.PaneWidth = this.Width;
                ////switch (this.DockPosition)
                ////{
                ////    case Dock.Left:
                ////        if (FloatWidth == 0)
                ////        {
                ////            FloatWidth = 200;
                ////            this.PaneWidth = 200;
                ////        }
                ////        else
                ////        {
                ////            this.PaneWidth = FloatWidth;
                ////        }
                ////        break;
                ////    case Dock.Right:
                ////        if (FloatWidth == 0)
                ////        {
                ////            FloatWidth = 200;
                ////            this.PaneWidth = 200;
                ////        }
                ////        else
                ////        {
                ////            this.PaneWidth = FloatWidth;
                ////        }
                ////        break;
                ////    case Dock.Bottom:
                ////        if (FloatHeight == 0)
                ////        {
                ////            FloatHeight = 200;
                ////            this.PaneHeight = 200;
                ////        }
                ////        else
                ////        {
                ////            this.PaneHeight = FloatHeight;
                ////        }
                ////        break;
                ////    case Dock.Top:
                ////        if (FloatHeight == 0)
                ////        {
                ////            FloatHeight = 200;
                ////            this.PaneHeight = 200;
                ////        }
                ////        else
                ////        {
                ////            this.PaneHeight = FloatHeight;
                ////        }
                ////        break;

                ////}
                #endregion

                DockStateChangingEventArgs arg = new DockStateChangingEventArgs(this.WindowChildElement, DockState.AutoHidden, DockState.Dock, DockingManager.GetSide(WindowChildElement,DockState.Dock));
                this.DockingManager.FireDockStateChanging(arg);
                if (!arg.Cancel)
                {
                    WindowUnPinned();
                    this.DockingManager.FireDockStateChanged(this.WindowChildElement, DockState.AutoHidden, DockState.Dock);
                }
                else
                {
                    unPinnedCancelled = true;
                    dockToggle.IsChecked = true;
                    unPinnedCancelled = false;
                }
            }
        }


        /// <summary>
        /// Gets the dock parent window container.
        /// </summary>
        /// <param name="_windows">The _windows.</param>
        protected internal void GetDockParentWindowContainer(Window _windows)
        {
            if (_windows.DockManager != null)
            {
                if (_windows.DockManager.Parent.GetType() == typeof(WindowContainer))
                {
                    (_windows.DockManager.Parent as WindowContainer)._window.ChangeState(DockState.Dock);
                    _windows.Visibility = Visibility.Visible;
                    GetDockParentWindowContainer((_windows.DockManager.Parent as WindowContainer)._window);
                }
            }
        }

        /// <summary>
        /// Gets the float parent window container.
        /// </summary>
        /// <param name="_windows">The _windows.</param>
        protected internal void GetFloatParentWindowContainer(Window _windows)
        {
            if (_windows.DockManager.Parent.GetType() == typeof(WindowContainer))
            {
                (_windows.DockManager.Parent as WindowContainer)._window.Visibility = Visibility.Collapsed;
                (_windows.DockManager.Parent as WindowContainer)._window.ChangeState(DockState.Hidden);
                GetFloatParentWindowContainer((_windows.DockManager.Parent as WindowContainer)._window);
            }
        }

        /// <summary>
        /// Numberofs the children.
        /// </summary>
        /// <param name="_windows">The _windows.</param>
        /// <param name="dm">The dm.</param>
        /// <returns></returns>
        protected internal int NumberofChildren(Window _windows, DockManager dm)
        {
            int count = 0;
            if (dm != null)
            {
                //if (dm.Parent.GetType() == typeof(WindowContainer))
                //{
                //    Window windows = (dm.Parent as WindowContainer)._window;
                IEnumerable<Window> windowquery = _windows.WindowCollection.Where(tempwindow => ((Window)tempwindow).DockManager == dm && ((Window)tempwindow).Visibility == Visibility.Visible && !((Canvas)this.DockingManager).Children.Contains(((Window)tempwindow)) && ((Window)tempwindow).Parent != null && ((Window)tempwindow).CustomTabControl.Items.Count > 0);
                //count = _windows.WindowCollection.Count;
                count = windowquery.Count();

                //for (int i = 0; i < windowquery.Count(); i++)
                //    {
                //        if (windowquery.ElementAt(i).DockState == DockState.Float)
                //        {
                //            count--;
                //        }
                //        else
                //        {
                //            if (dm != windowquery.ElementAt(i).DockManager)
                //            {
                //                count--;
                //            }
                //        }
                //    }
                //}
            }

            return count;
        }

        /// <summary>
        /// Numberofs the float with tar getname.
        /// </summary>
        /// <param name="_windows">The _windows.</param>
        /// <param name="dm">The dm.</param>
        /// <returns></returns>
        protected internal int NumberofFloatWithTarGetname(Window _windows, DockManager dm)
        {
            int count = 0;
            IEnumerable<Window> windowquery = _windows.WindowCollection.Where(tempwindow => ((Window)tempwindow).Visibility == Visibility.Visible && ((Window)tempwindow).CustomTabControl.Items.Count >= 1);
            if (dm != null)
            {
                for (int i = 0; i < windowquery.Count(); i++)
                {
                    if (windowquery.ElementAt(i) != this && windowquery.ElementAt(i).DockState == DockState.Float && windowquery.ElementAt(i).DockManager == dm && ((Canvas)windowquery.ElementAt(i).DockingManager).Children.Contains(windowquery.ElementAt(i)) && windowquery.ElementAt(i).Visibility == Visibility.Visible)
                    {
                        count++;
                    }
                }
            }

            if (count == 0)
            {
                List<Window> windowCollection = new List<Window>();
                for (int i = 0; i < windowquery.Count(); i++)
                {
                    if (windowquery.ElementAt(i).FloatWindowTargetNameCollection.Count > 0 && !windowCollection.Contains(windowquery.ElementAt(i)))
                    {
                        for (int m = 0; m < windowquery.ElementAt(i).FloatWindowTargetNameCollection.Count; m++)
                        {
                            //if (((Canvas)DockingManager).Children.Contains(windowquery.ElementAt(i).FloatWindowTargetNameCollection[m].OwnWindow) && _windows.WindowCollection[i].FloatWindowTargetNameCollection[m].OwnWindow.Visibility == Visibility.Visible)
                            //{
                            if (windowquery.ElementAt(i) != this && windowquery.ElementAt(i).FloatWindowTargetNameCollection[m].OwnWindow.DockState == DockState.Float && windowquery.ElementAt(i).FloatWindowTargetNameCollection[m].OwnWindow.DockManager == dm && ((Canvas)windowquery.ElementAt(i).FloatWindowTargetNameCollection[m].OwnWindow.DockingManager).Children.Contains(windowquery.ElementAt(i).FloatWindowTargetNameCollection[m].OwnWindow) && windowquery.ElementAt(i).FloatWindowTargetNameCollection[m].OwnWindow.Visibility == Visibility.Visible)
                            {
                                count++;
                            }
                            //}
                            if (windowquery.Contains(windowquery.ElementAt(i).FloatWindowTargetNameCollection[m].OwnWindow))
                            {
                                windowCollection.Add(windowquery.ElementAt(i).FloatWindowTargetNameCollection[m].OwnWindow);
                                //windowquery.remove(windowquery.ElementAt(i).FloatWindowTargetNameCollection[m].OwnWindow)
                            }
                        }
                    }
                }
            }
            return count;
        }


        /// <summary>
        /// Gets the float window with target name.
        /// </summary>
        /// <param name="_windows">The _windows.</param>
        /// <param name="dm">The dm.</param>
        /// <returns></returns>
        protected internal Window GetFloatWindowWithTarGetname(Window _windows, DockManager dm)
        {
            Window temp = null;
            if (dm != null)
            {
                for (int i = 0; i < _windows.WindowCollection.Count; i++)
                {
                    if (_windows.WindowCollection[i].DockState == DockState.Float && _windows.WindowCollection[i].DockManager == dm && ((Canvas)_windows.WindowCollection[i].DockingManager).Children.Contains(_windows.WindowCollection[i]) && _windows.WindowCollection[i].Visibility == Visibility.Visible)
                    {
                        temp = _windows.WindowCollection[i];
                        break;
                    }
                }
            }

            if (temp == null)
            {
                for (int i = 0; i < _windows.WindowCollection.Count; i++)
                {
                    if (_windows.WindowCollection[i].FloatWindowTargetNameCollection.Count > 0)
                    {
                        for (int m = 0; m < _windows.WindowCollection[i].FloatWindowTargetNameCollection.Count; m++)
                        {
                            if (((Canvas)DockingManager).Children.Contains(_windows.WindowCollection[i].FloatWindowTargetNameCollection[m].OwnWindow) && _windows.WindowCollection[i].FloatWindowTargetNameCollection[m].OwnWindow.Visibility == Visibility.Visible)
                            {
                                temp = _windows.WindowCollection[i].FloatWindowTargetNameCollection[m].OwnWindow;
                                break;
                            }
                        }
                        if (temp != null)
                        {
                            break;
                        }
                    }
                }
            }
            return temp;
        }

        /// <summary>
        /// Numberofs the float children.
        /// </summary>
        /// <param name="_windows">The _windows.</param>
        /// <param name="dm">The dm.</param>
        /// <returns></returns>
        protected internal int NumberofFloatChildren(Window _windows, DockManager dm)
        {
            int count = 0;
            if (dm != null)
            {
                IEnumerable<Window> windowquery = _windows.WindowCollection.Where(tempwindow => ((Window)tempwindow).Visibility == Visibility.Visible);

                for (int i = 0; i < windowquery.Count(); i++)
                {
                    if (windowquery.ElementAt(i) != this && windowquery.ElementAt(i).DockState == DockState.Float && ((Canvas)windowquery.ElementAt(i).DockingManager).Children.Contains(windowquery.ElementAt(i)) && windowquery.ElementAt(i).Visibility == Visibility.Visible)
                    {
                        count++;
                    }
                }
            }

            return count;
        }

        /// <summary>
        /// Numberofs the children.
        /// </summary>
        /// <param name="_windows">The _windows.</param>
        /// <returns></returns>
        protected internal int NumberofChildren(Window _windows)
        {
            int count = 0;
            if (_windows.DockManager != null)
            {
                if (_windows.DockManager.Parent.GetType() == typeof(WindowContainer))
                {
                    Window windows = (_windows.DockManager.Parent as WindowContainer)._window;
                    IEnumerable<Window> windowquery = windows.WindowCollection.Where(tempwindow => ((Window)tempwindow).Visibility == Visibility.Visible && ((Window)tempwindow).CustomTabControl.Items.Count > 0 && ((Window)tempwindow).DockManager.Parent is WindowContainer);
                    //count = _windows.WindowCollection.Count;
                    count = windowquery.Count();
                    for (int i = 0; i < windowquery.Count(); i++)
                    {
                        if (windowquery.ElementAt(i).DockState != DockState.Float)
                        {
                            count--;
                        }
                        else
                        {
                            if (_windows.DockManager != windowquery.ElementAt(i).DockManager)
                            {
                                count--;
                            }
                        }
                    }
                }
            }

            return count;
        }

        /// <summary>
        /// Gets the least window.
        /// </summary>
        /// <param name="_windows">The _windows.</param>
        /// <returns></returns>
        protected internal int GetLeastWindow(Window _windows)
        {
            int count = 0;
            if (_windows.DockManager != null)
            {
                if (_windows.DockManager.Parent.GetType() == typeof(WindowContainer))
                {
                    Window windows = (_windows.DockManager.Parent as WindowContainer)._window;
                    IEnumerable<Window> windowquery = windows.WindowCollection.Where(tempwindow => ((Window)tempwindow).Visibility == Visibility.Visible);
                    count = windowquery.Count();
                    for (int i = 0; i < windowquery.Count(); i++)
                    {
                        if (windowquery.ElementAt(i).DockState != DockState.Dock)
                        {
                            count--;
                        }
                        else
                        {
                            if (_windows.DockManager != windowquery.ElementAt(i).DockManager)
                            {
                                count--;
                            }
                        }
                    }
                }
            }

            return count;
        }


        /// <summary>
        /// Gets the least children.
        /// </summary>
        /// <param name="_windows">The _windows.</param>
        /// <returns></returns>
        protected internal Window GetLeastChildren(Window _windows)
        {
            Window leastWindow = null;
            int count = 0, indexer = 0;
            if (_windows.DockManager != null)
            {
                if (_windows.DockManager.Parent.GetType() == typeof(WindowContainer))
                {
                    Window windows = (_windows.DockManager.Parent as WindowContainer)._window;
                    //count = windows.WindowCollection.Count;
                    IEnumerable<Window> windowquery = windows.WindowCollection.Where(tempwindow => ((Window)tempwindow).Visibility == Visibility.Visible);
                    count = windowquery.Count();
                    for (int i = 0; i < windowquery.Count(); i++)
                    {
                        if (windowquery.ElementAt(i).DockState == DockState.Dock && _windows.DockManager == windowquery.ElementAt(i).DockManager)
                        {
                            indexer = i;
                        }
                        if (windowquery.ElementAt(i).DockState != DockState.Dock)
                        {
                            count--;
                        }
                        else
                        {
                            if (_windows.DockManager != windowquery.ElementAt(i).DockManager)
                            {
                                count--;
                            }
                        }
                    }
                    leastWindow = windowquery.ElementAt(indexer);
                }
            }

            return leastWindow;
        }

        /// <summary>
        /// Represents the least window.
        /// </summary>
        protected internal Window leastWindow = null;

        /// <summary>
        /// Checks the children present.
        /// </summary>
        /// <param name="_windows">The _windows.</param>
        /// <returns></returns>
        protected internal bool CheckChildrenPresent(Window _windows)
        {
            bool childrenpresent = false;

            #region old value

            IEnumerable<Window> windowquery = _windows.WindowCollection.Where(tempwindow => ((Window)tempwindow).DockManager == _windows.WindowContainer.DockManager && !((Canvas)this.DockingManager).Children.Contains(((Window)tempwindow)) && ((Window)tempwindow).Visibility == Visibility.Visible && ((Window)tempwindow).Parent != null && ((Window)tempwindow).CustomTabControl.Items.Count > 0 && ((Window)tempwindow).DockManager.Parent is WindowContainer);

            if (windowquery.Count() > 1)
            {
                childrenpresent = true;
            }
            else
            {
                if (windowquery.Count() > 0)
                {
                    leastWindow = windowquery.ElementAt(0);
                }
                childrenpresent = false;
            }
            //int count = 0, indexer = 0;
            //IEnumerable<Window> windowquery = _windows.WindowCollection.Where(tempwindow => ((Window)tempwindow).Visibility == Visibility.Visible && ((Window)tempwindow).Parent != null && ((Window)tempwindow).CustomTabControl.Items.Count > 0 && ((Window)tempwindow).DockManager.Parent is WindowContainer);
            ////count = windowquery.Count();
            //for (int i = 0; i < windowquery.Count(); i++)
            //{
            //    if (windowquery.ElementAt(i).DockState == DockState.Float && _windows.WindowContainer.DockManager == windowquery.ElementAt(i).DockManager)
            //    {
            //        indexer = i;
            //    }
            //    if (windowquery.ElementAt(i).DockState != DockState.Float)
            //    {
            //        count++;
            //    }
            //    else
            //    {
            //        if (_windows.WindowContainer.DockManager != windowquery.ElementAt(i).DockManager)
            //        {
            //            count++;
            //        }
            //    }
            //}
            //if (count == windowquery.Count() - 1)
            //{
            //    childrenpresent = false;
            //    leastWindow = windowquery.ElementAt(indexer);
            //    //childrenpresent = CheckChildrenPresent(_windows.WindowCollection[indexer]);
            //}
            //else if(count != _windows.WindowCollection.Count)
            //{
            //    childrenpresent = true;
            //}
            //}
            #endregion
            return childrenpresent;
        }

        /// <summary>
        /// Checks the float window is present.
        /// </summary>
        /// <param name="_windows">The _windows.</param>
        /// <param name="dm">The dm.</param>
        /// <returns></returns>
        protected internal Window CheckFloatWindowIsPresent(Window _windows, DockManager dm)
        {
            Window _w = null;
            int count = 0, indexer = 0;
            IEnumerable<Window> windowquery = _windows.WindowCollection.Where(tempwindow => ((Window)tempwindow).Visibility == Visibility.Visible);
            for (int i = 0; i < windowquery.Count(); i++)
            {
                if (windowquery.ElementAt(i) != this && windowquery.ElementAt(i).DockState == DockState.Float && ((Canvas)_windows.DockingManager).Children.Contains(windowquery.ElementAt(i)) && windowquery.ElementAt(i).Visibility == Visibility.Visible)
                {
                    indexer = i;
                    break;
                }
                if (windowquery.ElementAt(i).DockState != DockState.Dock)
                {
                    count++;
                }
                else
                {
                    //if (dm != windowquery.ElementAt(i).DockManager)
                    //{
                    count++;
                    // }
                }
            }
            if (indexer >= 0)
            {
                _w = windowquery.ElementAt(indexer);
            }
            return _w;
        }

        /// <summary>
        /// Checks the children present.
        /// </summary>
        /// <param name="_windows">The _windows.</param>
        /// <param name="dm">The dm.</param>
        /// <returns></returns>
        protected internal bool CheckChildrenPresent(Window _windows, DockManager dm)
        {
            bool childrenpresent = false;
            if (_windows._Caption != string.Empty)
            {
                if (_windows.DockManager != null)
                {
                    leastWindow = _windows;
                }
            }
            else
            {
                int count = 0, indexer = 0;
                IEnumerable<Window> windowquery = _windows.WindowCollection.Where(tempwindow => ((Window)tempwindow).Visibility == Visibility.Visible);
                for (int i = 0; i < windowquery.Count(); i++)
                {
                    if (windowquery.ElementAt(i).DockState == DockState.Dock && dm == windowquery.ElementAt(i).DockManager)
                    {
                        indexer = i;
                    }
                    if (windowquery.ElementAt(i).DockState != DockState.Dock)
                    {
                        count++;
                    }
                    else
                    {
                        if (dm != windowquery.ElementAt(i).DockManager)
                        {
                            count++;
                        }
                    }
                }
                if (count == windowquery.Count() - 1)
                {
                    childrenpresent = false;
                    leastWindow = windowquery.ElementAt(indexer);
                    //childrenpresent = CheckChildrenPresent(_windows.WindowCollection[indexer]);
                }
                else if (count != windowquery.Count())
                {
                    childrenpresent = true;
                }
            }
            return childrenpresent;
        }

        /// <summary>
        /// Checks the parenthas children.
        /// </summary>
        /// <param name="dockmanager">The dockmanager.</param>
        /// <returns></returns>
        protected internal bool CheckParenthasChildren(DockManager dockmanager)
        {
            bool childrenpresent = false;

            if (dockmanager != null)
            {
                if (dockmanager.Parent.GetType() == typeof(WindowContainer))
                {
                    Window windows = (dockmanager.Parent as WindowContainer)._window;                    
                    IEnumerable<Window> windowquery = windows.WindowCollection.Where(tempwindow => ((Window)tempwindow).DockManager == dockmanager && !((Canvas)this.DockingManager).Children.Contains(((Window)tempwindow)) && ((Window)tempwindow).Visibility == Visibility.Visible && ((Window)tempwindow).Parent != null && ((Window)tempwindow).CustomTabControl.Items.Count > 0 && ((Window)tempwindow).DockManager.Parent is WindowContainer);
                    //for (int i = 0; i < windowquery.Count(); i++)
                    //{
                    //    if (windowquery.ElementAt(i).DockState != DockState.Float)
                    //    {
                    //        count++;
                    //    }
                    //    else
                    //    {
                    //        if (dockmanager != windowquery.ElementAt(i).DockManager)
                    //        {
                    //            count++;
                    //        }
                    //    }
                    //}
                    //if (count == windowquery.Count() - 1)
                    //{
                    //    childrenpresent = false;
                    //   // childrenpresent = CheckParenthasChildren((dockmanager.Parent as WindowContainer)._window);
                    //}
                    //else
                    //{
                    //    childrenpresent = true;
                    //}
                    if (windowquery.Count() > 1)
                    {
                        childrenpresent = true;
                    }
                    else
                    {
                        childrenpresent = false;
                    }
                }
            }
            return childrenpresent;
        }

        /// <summary>
        /// Removes the dock float window container.
        /// </summary>
        /// <param name="_windows">The _windows.</param>
        protected internal void RemoveDockFloatWindowContainer(Window _windows)
        {
            if (_windows.DockManager != null)
            {
                if (_windows.DockManager.Parent.GetType() == typeof(WindowContainer))
                {
                    Window windows = (_windows.DockManager.Parent as WindowContainer)._window;

                    IEnumerable<Window> windowquery = windows.WindowCollection.Where(tempwindow => ((Window)tempwindow).Visibility == Visibility.Visible); int count = 0;
                    for (int i = 0; i < windowquery.Count(); i++)
                    {
                        if (windowquery.ElementAt(i).DockState != DockState.Dock)
                        {
                            count++;
                        }
                        else
                        {
                            if (_windows.DockManager != windowquery.ElementAt(i).DockManager)
                            {
                                count++;
                            }
                        }
                    }
                    if (count == windowquery.Count())
                    {
                        (_windows.DockManager.Parent as WindowContainer)._window.Visibility = Visibility.Collapsed;
                        //(_windows.DockManager.Parent as WindowContainer)._window.ChangeState(DockState.Hidden);
                        RemoveDockFloatWindowContainer((_windows.DockManager.Parent as WindowContainer)._window);
                    }
                }
            }
        }

        /// <summary>
        /// Autoes the hide parent window.
        /// </summary>
        /// <param name="_windows">The _windows.</param>
        protected internal void AutoHideParentWindow(Window _windows)
        {
            if (_windows.DockManager != null)
            {
                if (_windows.DockManager.Parent.GetType() == typeof(WindowContainer))
                {
                    Window windows = (_windows.DockManager.Parent as WindowContainer)._window;
                    int count = 0;
                    for (int i = 0; i < windows.WindowCollection.Count; i++)
                    {
                        if (windows.WindowCollection[i].DockState != DockState.Dock)
                        {
                            count++;
                        }
                        else
                        {
                            if (_windows.DockManager != windows.WindowCollection[i].DockManager)
                            {
                                count++;
                            }
                        }
                    }
                    if (count == windows.WindowCollection.Count)
                    {
                        (_windows.DockManager.Parent as WindowContainer)._window.Visibility = Visibility.Collapsed;
                        (_windows.DockManager.Parent as WindowContainer)._window.ChangeState(DockState.Hidden);
                        AutoHideParentWindow((_windows.DockManager.Parent as WindowContainer)._window);
                    }
                    else if (count == windows.WindowCollection.Count - 1)
                    {
                        Window _window = GetParent(this);
                        if (_window.Parent is DockingManager)
                        {
                            if (_window.WindowContainer != null)
                            {
                                if (_window.WindowContainer.DockManager != null)
                                {
                                    if ((_window.WindowContainer.DockManager.Children[0] as DockingGrid).gridDocking.Children.Count == 1)
                                    {
                                        Window _w = (Window)(_window.WindowContainer.DockManager.Children[0] as DockingGrid).gridDocking.Children[0];
                                        if (_w.WindowCollection.Count == 0)
                                        {
                                            GetChildren(_window);
                                        }
                                        else
                                        {
                                            GetChildren(_w);
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (windows.DockManager != null)
                            {
                                GetChildren(windows);
                            }
                            else if (((Canvas)DockingManager).Children.Contains(windows))
                            {
                                GetChildren(windows);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Setwidthes the height for animation.
        /// </summary>
        /// <param name="_pinnedWindow">The _pinned window.</param>
        /// <param name="_pinnedAnimationWindow">The _pinned animation window.</param>
        protected internal void SetwidthHeightForAnimation(Window _pinnedWindow, Animation _pinnedAnimationWindow)
        {
            switch (_pinnedWindow.DockPosition)
            {
                case Dock.Bottom:
                    _pinnedAnimationWindow.AnimateSize(_pinnedWindow.ActualWidth, 0);
                    _pinnedAnimationWindow.AnimatePosition(Canvas.GetLeft(_pinnedWindow), _pinnedWindow.DockingManager.ActualHeight + 1);
                    break;

                case Dock.Left:
                    _pinnedAnimationWindow.AnimateSize(0, _pinnedWindow.ActualHeight);
                    break;

                case Dock.Right:
                    _pinnedAnimationWindow.AnimateSize(0, _pinnedWindow.ActualHeight);
                    _pinnedAnimationWindow.AnimatePosition(_pinnedWindow.DockingManager.ActualWidth + 1, Canvas.GetTop(_pinnedWindow));
                    break;

                case Dock.Top:
                    _pinnedAnimationWindow.AnimateSize(_pinnedWindow.ActualWidth, 0);
                    _pinnedAnimationWindow.AnimatePosition(Canvas.GetLeft(_pinnedWindow), 0);
                    break;
            }
        }

        /// <summary>
        /// Handles the pinned event of the dockToggle control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void dockToggle_pinned(object sender, RoutedEventArgs e)
        {
            if (!unPinnedCancelled && !menuItemAutoHide)
            {
                Animation _pinnedAnimationWindow = null;

                if (this.DockingManager != null)
                {
                    if (this._Caption != string.Empty && this.CustomTabControl != null && this.CustomTabControl.SelectedItem != null)
                    {
                        this.DockingManager.ActiveWindow = (this.CustomTabControl.SelectedItem as CustomTabItem).OwnWindow;
                    }
                }
                DockStateChangingEventArgs arg = new DockStateChangingEventArgs(this.WindowChildElement, DockState.Dock, DockState.AutoHidden, DockingManager.GetSide(WindowChildElement,DockState.Dock));
                this.DockingManager.FireDockStateChanging(arg);
                if (!arg.Cancel)
                {

                    bool evetnCanceled = false;
                    if (this.DockingManager != null)
                    {
                        this.DockingManager.InvokePreviewEventforPinned(this.WindowChildElement, ref evetnCanceled);
                    }
                    if (!evetnCanceled)
                    {
                        this.AnimationHeight = 0;
                        this.AnimationWidth = 0;
                        if (this.CanAutoHide && this.DockState == DockState.Dock && this.dockToggle.Visibility == Visibility.Visible)
                        {
                            Window _pinnedWindow =
                                this as Window;
                            if (Canvas.GetZIndex(_pinnedWindow) <= 2)
                            {
                                Canvas.SetZIndex(_pinnedWindow, 1);
                            }

                            if (_pinnedWindow != null)
                            {
                                _pinnedAnimationWindow = new Animation(_pinnedWindow);
                                if (_pinnedWindow.Parent != null)
                                {
                                    if (_pinnedWindow.Parent.GetType() != typeof(WindowContainer))
                                    {
                                        SetwidthHeightForAnimation(_pinnedWindow, _pinnedAnimationWindow);
                                    }
                                }
                                else
                                {
                                    SetwidthHeightForAnimation(_pinnedWindow, _pinnedAnimationWindow);
                                }
                            }
                        }
                        else
                        {
                            if (this.DockState != DockState.AutoHidden)
                            {
                                dockToggle.IsChecked = false;
                            }
                        }
                    }
                    else
                    {
                        if (this.DockState != DockState.AutoHidden)
                        {
                            dockToggle.IsChecked = false;
                        }
                    }
                    if (this.DockingManager != null)
                    {
                        this.DockingManager.InvokeEventForPinned(this.WindowChildElement);
                        if (this.dockToggle.Visibility == Visibility.Collapsed)
                        {
                            _pinnedAnimationWindow.StopAllAnimation();
                            this.FloatWidth = this.Width;
                            this.FloatHeight = this.Height;
                            this.UpDateFloatWindowSize();
                        }
                        //StopAllAnimation
                    }
                }
                else
                {
                    pinnedCancelled = true;
                    dockToggle.IsChecked = false;
                    pinnedCancelled = false;
                }
            }
        }

        /// <summary>
        /// Handles the Click event of the closeButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void closeButton_Click(object sender, RoutedEventArgs e)
        {
            UIElement uiElement = null;
            bool cancelled = false;
            if (this.CustomTabControl != null)
            {
                if (this.CustomTabControl.Items.Count > 1)
                {
                    uiElement = ((CustomTabItem)this.CustomTabControl.SelectedItem).OwnWindow.WindowChildElement;
                }
                else
                {
                    uiElement = this.WindowChildElement;
                }

                DockStateChangingEventArgs args = new DockStateChangingEventArgs(uiElement, this.DockState, DockState.Hidden, DockingManager.GetSide(uiElement,DockState));
                
                this.DockingManager.FireDockStateChanging(args);
                cancelled = args.Cancel;
            }
            DockState dockState = this.DockState;
            if (!cancelled)
            {
                bool evetnCanceled = false;
                if (this.DockingManager != null)
                {
                    this.DockingManager.InvokePreviewEvent(this.WindowChildElement, ref evetnCanceled);
                }
                if (!evetnCanceled)
                {
                    if (this.DockingManager != null)
                    {
                        if (this._Caption != string.Empty)
                        {
                            if (this._Caption != string.Empty && this.CustomTabControl != null && this.CustomTabControl.SelectedItem != null)
                            {
                                this.DockingManager.ActiveWindow = (this.CustomTabControl.SelectedItem as CustomTabItem).OwnWindow;
                            }
                        }
                        else
                        {
                            if (this.DockingManager.ActiveWindow.DockState == DockState.AutoHidden)
                            {
                                this.DockingManager.ActiveWindow.AutoHide = true;
                                this.DockingManager.timer.Start();
                            }
                        }
                    }
                    if (this._Caption == string.Empty)
                    {
                        if (((Canvas)DockingManager).Children.Contains(this))
                        {
                            ((Canvas)DockingManager).Children.Remove(this);
                            for (int i = 0; i < this.WindowCollection.Count; i++)
                            {
                                DockStateChangingEventArgs args = new DockStateChangingEventArgs(this.WindowCollection[i].WindowChildElement, this.WindowCollection[i].DockState, DockState.Hidden, DockSide.Left);

                                this.DockingManager.FireDockStateChanging(args);
                                if (!args.Cancel)
                                {
                                    this.WindowCollection[i].DockState = DockState.Hidden;
                                    if (this.WindowCollection[i].DockingManager != null)
                                        this.DockingManager.FireDockStateChanged(this.WindowCollection[i].WindowChildElement, dockState, DockState.Hidden);
                                    if (this.WindowCollection[i].DockingManager != null)
                                        this.DockingManager.InvokeEvent(this.WindowCollection[i].WindowChildElement);
                                }
                            }
                        }
                    }
                    else
                    {
                        Close();
                    }
                }
                if (this.CustomTabControl != null)
                {
                    if (this.DockingManager != null)
                        this.DockingManager.FireDockStateChanged(uiElement, dockState, DockState.Hidden);
                    if (this.DockingManager != null)
                        this.DockingManager.InvokeEvent(uiElement);
                }
            }
        }

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether [dragging enabled].
        /// </summary>
        /// <value><c>true</c> if [dragging enabled]; otherwise, <c>false</c>.</value>
        protected internal bool DraggingEnabled
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Caption Text
        /// this property accesses and set and internal member of the template
        /// used top draw the title of the window, it may happen that you want to get or set
        /// this property before the template is actually loaded, so you have to
        /// bufferize the value and apply it later in the OnApplyTemplate function
        /// </summary>
        public string Caption
        {
            get
            {
                return (captionText != null) ? captionText.Text : header;
            }

            set
            {
                if (captionText != null)
                {
                    captionText.Text = value;
                }
                else
                {
                    header = value;
                }
                Header = value;
            }
        }

        /// <summary>
        /// Represents the caption.
        /// </summary>
        internal string _Caption = string.Empty;

        /// <summary>
        /// Represents the header.
        /// </summary>
        internal string header = string.Empty;
        #endregion

        #region Dragging Functions
        /// <summary>
        /// Stores the Boolean value.
        /// </summary>
        protected internal bool isDragging = false;

        /// <summary>
        /// Stores the Boolean value.
        /// </summary>
        protected internal bool isMoved = false;

        /// <summary>
        /// Represents the initial window location.
        /// </summary>
        internal Point initialWindowLocation = new Point(1, 1);

        /// <summary>
        /// Represents the Initial drag point.
        /// </summary>
        private Point initialDragPoint;

        /// <summary>
        /// Represents the current Z index.
        /// </summary>
        protected internal static int currentZIndex = 1;

        /// <summary>
        /// Defines the drag events.
        /// </summary>
        private void DefineDragEvents()
        {
            if (captionBar != null)
            {
                captionBar.MouseLeftButtonDown +=
                     new MouseButtonEventHandler(captionBar_MouseLeftButtonDown);
                captionBar.MouseMove +=
                    new MouseEventHandler(captionBar_MouseMove);
                captionBar.MouseLeftButtonUp +=
                     new MouseButtonEventHandler(captionBar_MouseLeftButtonUp);
            }
        }

        /// <summary>
        /// Deattaches the caption bar event.
        /// </summary>
        protected internal void DeattachCaptionBarEvent()
        {
            SizeChanged -= new SizeChangedEventHandler(Window_SizeChanged);

            if (captionBar != null)
            {
                captionBar.MouseLeftButtonDown -=
                        new MouseButtonEventHandler(captionBar_MouseLeftButtonDown);
                captionBar.MouseMove -=
                    new MouseEventHandler(captionBar_MouseMove);
                captionBar.MouseLeftButtonUp -=
                     new MouseButtonEventHandler(captionBar_MouseLeftButtonUp);
            }
            DuplicateWindowCollection.Clear();
            WindowCollection.Clear();
            StoredMoveToWindow = null;
            FloatWindowTargetNameCollection.Clear();
            TargetNameCollection.Clear();
            timer.Tick -= new EventHandler(timer_Tick);
            _timer.Tick -= new EventHandler(_timer_Tick);
            if (contextMenu != null)
            {
                contextMenu.Opened -= new RoutedEventHandler(contextMenu_ContextMenuOpened);
            }

            if (floatContextMenuItemAdv != null)
            {
                floatContextMenuItemAdv.Checked -= new RoutedEventHandler(floatContextMenuItemAdv_Checked);
            }
            if (dockContextMenuItemAdv != null)
            {
                dockContextMenuItemAdv.Checked -= new RoutedEventHandler(dockContextMenuItemAdv_Checked);
            }
            if (autoHideContextMenuItemAdv != null)
            {
                autoHideContextMenuItemAdv.Checked -= new RoutedEventHandler(autoHideContextMenuItemAdv_Checked);
            }
            if (hideContextMenuItemAdv != null)
            {
                hideContextMenuItemAdv.Checked -= new RoutedEventHandler(hideContextMenuItemAdv_Checked);
            }

            if (tempGrid != null)
            {
                tempGrid.MouseLeftButtonDown -= new MouseButtonEventHandler(tempGrid_MouseLeftButtonDown);
            }
            if (closeButton != null)
            {
                closeButton.Click -= new RoutedEventHandler(closeButton_Click);
                closeButton.MouseEnter -= new MouseEventHandler(closeButton_MouseEnter);
                closeButton.MouseLeave -= new MouseEventHandler(closeButton_MouseLeave);
            }

            if (contentpresenter != null)
            {
                contentpresenter.MouseLeftButtonDown -= new MouseButtonEventHandler(contentpresenter_MouseLeftButtonDown);
            }
            if (dockToggle != null)
            {
                dockToggle.Checked -= new RoutedEventHandler(dockToggle_pinned);
                dockToggle.Unchecked -= new RoutedEventHandler(dockToggle_Unpinned);
            }

            if (optionsPopUp != null)
            {
                optionsPopUp.MouseEnter -= new MouseEventHandler(optionsPopUp_MouseEnter);
                optionsPopUp.MouseLeave -= new MouseEventHandler(optionsPopUp_MouseLeave);
            }

            if (optionsButton != null)
            {
                optionsButton.Click -= new RoutedEventHandler(optionsButton_Click);
                optionsButton.MouseMove -= new MouseEventHandler(optionsButton_MouseMove);
            }
            if (window != null)
            {
                window.MouseEnter -= new MouseEventHandler(window_MouseEnter);
                window.MouseLeave -= new MouseEventHandler(window_MouseLeave);
                window.MouseLeftButtonDown -= new MouseButtonEventHandler(window_MouseLeftButtonDown);
                window.MouseMove -= new MouseEventHandler(window_MouseMove);
                window.MouseLeftButtonUp -= new MouseButtonEventHandler(window_MouseLeftButtonUp);
            }
            if (stackpanel != null)
            {
                stackpanel.Children.Clear();
                partInnerGrid.Children.Clear();
            }

            if (DockingManager != null && DockingManager.windowList.Contains(this))
            {
                DockingManager.windowList.Remove(this);
            }

            this.DockingManager = null;
            contentpresenter.Children.Clear();
            floatContextMenuItemAdv = null;
            dockContextMenuItemAdv = null;
            autoHideContextMenuItemAdv = null;
            hideContextMenuItemAdv = null;
           // ContextMenuAdv.SetContext(captionBar, null);
            if (optionsButton != null)
            {
                optionsButton.Template = null;
                optionsButton = null;
            }

        }

        /// <summary>
        /// Represents last drag position.
        /// </summary>
        private Point lastDragPosition;

        /// <summary>
        /// The drag moved event.
        /// </summary>
        public event DragEventHanlder DragMoved;

        /// <summary>
        /// Represents the OnStateChanged Event Handler.
        /// </summary>
        public EventHandler OnStateChanged;

        /// <summary>
        /// Occurs when [on dock changed].
        /// </summary>
        public event EventHandler OnDockChanged;

        /// <summary>
        /// Fires the on on dock changed.
        /// </summary>
        private void FireOnOnDockChanged()
        {
            if (OnDockChanged != null)
            {
                OnDockChanged(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Change dock border
        /// </summary>
        /// <param name="dock">New dock border</param>
        public void ChangeDock(Dock dock)
        {
            DockPosition = dock;
            FireOnOnDockChanged();
            ChangeState(DockState.Dock);
        }

        /// <summary>
        /// Moves to.
        /// </summary>
        /// <param name="destinationPane">The destination pane.</param>
        /// <param name="relativeDock">The relative dock.</param>
        internal void MoveTo(Window destinationPane, Dock relativeDock)
        {
            Window dockableDestPane = destinationPane as Window;
            if (dockableDestPane != null)
            {
                //  ChangeDock(dockableDestPane.DockPosition);
            }
            else
            {
                //  ChangeDock(relativeDock);
            }
            this.DockManager.DetachPaneEvents(this);
            DockManager.MoveTo(this, destinationPane, relativeDock);
            this.DockManager.AttachPaneEvents(this);

            //ChangeState(DockState.Dock);
        }

        /// <summary>
        /// Gets or sets the DockManager .
        /// </summary>
        private DockManager __dock = null;

        /// <summary>
        /// Gets or sets the dock manager.
        /// </summary>
        /// <value>The dock manager.</value>
        protected internal DockManager DockManager
        {
            get
            {
                return __dock;
            }
            set
            {
                __dock = value;
                if (this._Caption == "tt1")
                {
                }
            }
        }

        /// <summary>
        /// Gets or sets the DockManager .
        /// </summary>
        /// <value>The old value dock manager.</value>
        protected internal DockManager OldValueDockManager
        {
            get;
            set;
        }


        /// <summary>
        /// Gets or sets the container dock manager.
        /// </summary>
        /// <value>The container dock manager.</value>
        protected internal DockManager ContainerDockManager
        {
            get;
            set;
        }

        /// <summary>
        /// Fires the on on state changed.
        /// </summary>
        private void FireOnOnStateChanged()
        {
            if (OnStateChanged != null)
            {
                OnStateChanged(this, EventArgs.Empty);
            }
            else
            {
                if (this.DockManager != null)
                {
                    if (this.DockManager.gridDocking != null)
                    {
                        this.DockManager.gridDocking.ArrangeLayout();
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the relative pane.
        /// </summary>
        /// <value>The relative pane.</value>
        protected internal Window RelativePane
        {
            get;
            set;
        }

        /// <summary>
        /// Changes the state.
        /// </summary>
        /// <param name="newState">The new state.</param>
        internal void ChangeState(DockState newState)
        {
            this.DockState = newState;
            FireOnOnStateChanged();
        }


        /// <summary>
        /// During static instantiation, only the Netscape flag is checked
        /// </summary>
        static Window()
        {

        }

        /// <summary>   
        /// Flag indicating Navigator/Firefox/Safari or Internet Explorer   
        /// </summary>   
        private static bool _isNavigator;

        /// <summary>
        /// Gets the window object's client width
        /// </summary>
        /// <value>The width of the client.</value>
        public double ClientWidth
        {
            get
            {
                return Application.Current.IsRunningOutOfBrowser?DockingManager.ActualWidth:_isNavigator ? (double)HtmlPage.Window.GetProperty("innerWidth")
                    : (double)HtmlPage.Document.Body.GetProperty("clientWidth");
            }

        }

        internal Rect BoundingRectangle
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the window object's client height
        /// </summary>
        /// <value>The height of the client.</value>
        public double ClientHeight
        {
            get
            {
                return Application.Current.IsRunningOutOfBrowser ? DockingManager.ActualHeight:_isNavigator ? (double)HtmlPage.Window.GetProperty("innerHeight")
                    : (double)HtmlPage.Document.Body.GetProperty("clientHeight");
            }
        }

        ///// <summary>
        ///// Fires when the left mouse button goes down on the caption bar
        ///// </summary>
        ///// <param name="sender"> sender is a Border.</param>
        ///// <param name="e"> event args of MouseButton.</param>

        /// <summary>
        /// Handles the MouseLeftButtonDown event of the captionBar control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        void captionBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!Application.Current.IsRunningOutOfBrowser)
            {
                _isNavigator = HtmlPage.BrowserInformation.Name.Contains("Netscape");
            }
            this.DockingManager.popupLoadedInsideWindow = false;
            isMoved = false;
            UIElement parent = (UIElement)VisualTreeHelper.GetParent((UIElement)this);
            if (((Canvas)DockingManager).Children.Contains(this) || (this.IsremovedFromParent || this.DockState == DockState.Float))
            {
                if (!(currentZIndex > 3))
                {
                    currentZIndex = 3;
                }

                Canvas.SetZIndex(this, ++currentZIndex);
            }


            ((FrameworkElement)sender).CaptureMouse();
            if (((Canvas)DockingManager).Children.Contains(this))
            {
                this.initialDragPoint = e.GetPosition(this.DockingManager);
            }
            else
            {
                this.initialDragPoint = e.GetPosition(this.DockingManager);
            }

            this.initialWindowLocation.Y = this.initialDragPoint.Y;
            if (!((Canvas)DockingManager).Children.Contains(this))
            {
                if (this.FloatWidth != 0.0)
                {
                    if (this.DockManager.Parent is WindowContainer)
                    {
                        Point caption = e.GetPosition(this);
                        this.initialWindowLocation.X = this.initialDragPoint.X - caption.X;
                        this.initialWindowLocation.Y = this.initialDragPoint.Y - caption.Y;
                    }
                    else
                    {
                        Point caption = e.GetPosition(this);
                        double width = FloatWidth;
                        if (this.Parent is Grid)
                        {
                            if (((this.Parent as Grid).ActualWidth <= (width + 10)) && ((this.Parent as Grid).ActualWidth >= (width - 10)))
                            {
                                this.initialWindowLocation.X = this.initialDragPoint.X - caption.X;
                            }
                            else
                            {
                                this.initialWindowLocation.X = this.initialDragPoint.X - FloatWidth / 2.0;
                            }
                        }
                        else
                        {
                            this.initialWindowLocation.X = this.initialDragPoint.X - FloatWidth / 2.0;
                        }

                        this.initialWindowLocation.Y = this.initialDragPoint.Y - caption.Y;
                    }
                }
                else
                {
                    Point caption = e.GetPosition(this);
                    this.initialWindowLocation.X = this.initialDragPoint.X - caption.X;
                    this.initialWindowLocation.Y = this.initialDragPoint.Y - caption.Y;
                }
            }
            else
            {
                this.initialWindowLocation.X = Canvas.GetLeft(this);
                this.initialWindowLocation.Y = Canvas.GetTop(this);
            }
            if(this.CanDrag)
                this.isDragging = true;
            if (_timer.IsEnabled)
            {
                _timer.Stop();
                bool allowMe = false;
                bool Cancelled = false;
                DockState state = this.DockState; 
                if (this.DockState == DockState.Dock)
                {
                    if (this.CanFloat)
                    {
                        allowMe = true;
                        DockStateChangingEventArgs args = new DockStateChangingEventArgs(WindowChildElement,
                                                                                        this.DockState,
                                                                                        DockState.Float, DockingManager.GetSide(WindowChildElement,this.DockState));
                        if (this.DockingManager != null)
                            this.DockingManager.FireDockStateChanging(args);
                        allowMe = !args.Cancel;
                        Cancelled = args.Cancel;
                    }
                }
                else if (this.DockState == DockState.Float)
                {
                    if (this.CanDock)
                    {
                        allowMe = true;
                        DockStateChangingEventArgs args = new DockStateChangingEventArgs(WindowChildElement,
                                                                                        this.DockState,
                                                                                        DockState.Dock, DockingManager.GetSide(WindowChildElement,DockState.Dock));
                        if (this.DockingManager != null)
                            this.DockingManager.FireDockStateChanging(args);
                        allowMe = ! args.Cancel;
                        Cancelled = args.Cancel;
                    }
                }
                if (this._Caption != string.Empty && allowMe)
                {
                    if (this.MaximizedState == MaximizedState.Maximized)
                    {
                        if (this.maximizeButton != null)
                        {
                            this.setWhileRestoring = true;
                            this.maximizeButton.IsChecked = false;
                            this.setWhileRestoring = false;
                        }
                        this.MaximizedState = MaximizedState.Restored;
                    }

                    CustomTabItem cstabItem = this.CustomTabControl.SelectedItem as CustomTabItem;
                    _headerEventFired = true;
                    if (this.CustomTabControl != null)
                    {
                        DockingGrid dockingGrid = this.DockingManager.GetParentDockManager();
                        DockManager dm = null;
                        if (dockingGrid != null)
                        {
                            dm = dockingGrid._dockManager;
                        }
                        if (this.CustomTabControl.Items.Count > 1 && this.DockState == DockState.Dock)
                        {
                            foreach (CustomTabItem cstabitem in this.CustomTabControl.Items)
                            {
                                if (cstabitem.OwnWindow != this)
                                {
                                    if (cstabitem.OwnWindow.OldValueDockManager != null)
                                    {
                                        if (cstabitem.OwnWindow.OldValueDockManager.Parent is WindowContainer)
                                        {
                                            Window windowContainer = (cstabitem.OwnWindow.OldValueDockManager.Parent as WindowContainer)._window;
                                            //this.DockingManager.UpdateMoveToTargetName(cstabitem.OwnWindow);
                                            this.DockingManager.UpdateMoveToTargetName(cstabitem.OwnWindow, true);
                                        }
                                    }
                                    this.DockingManager.SetboolValueWithSideInMode(cstabitem.OwnWindow, Dock.Tabbed, DockState.Float);
                                    DockingManager.SetSideInFloatMode(cstabitem.OwnWindow.WindowChildElement, Dock.Tabbed);
                                    this.DockingManager.SetboolValueWithTargetName(cstabitem.OwnWindow, this._Caption, DockState.Float);
                                    DockingManager.SetTargetNameInFloatingMode(cstabitem.OwnWindow.WindowChildElement, this._Caption);
                                    cstabitem.OwnWindow.DockState = DockState.Float;
                                    cstabitem.OwnWindow.Visibility = Visibility.Collapsed;

                                }
                                cstabitem.OwnWindow.OldValueDockManager = null;
                            }
                            //this.InternalllyRaisedDockStateChanged = true;
                            this.DockingManager.SetboolValueWithSideInMode(this, Dock.Left, DockState.Float);
                            DockingManager.SetSideInFloatMode(this.WindowChildElement, Dock.Left);
                            this.DockingManager.SetboolValueWithTargetName(this, string.Empty, DockState.Float);
                            DockingManager.SetTargetNameInFloatingMode(this.WindowChildElement, string.Empty);
                            if (dm != null)
                            {
                                this.DockManager = dm;
                                this.OldValueDockManager = null;
                            }
                        }
                    }

                    if (this.CustomTabControl.Items.Count > 1 && this.DockState == DockState.Float)
                    {
                        List<CustomTabItem> cstabitemCollection = new List<CustomTabItem>();
                        foreach (CustomTabItem cstabitem in this.CustomTabControl.Items)
                        {
                            if (cstabitem.OwnWindow != null)
                            {
                                if (!cstabitemCollection.Contains(cstabitem))
                                {
                                    cstabitemCollection.Add(cstabitem);
                                }
                            }
                        }
                        foreach (CustomTabItem cstabitem in cstabitemCollection)
                        {
                            if (cstabitem.OwnWindow != this)
                            {
                                if (this.CustomTabControl.Items.Contains(cstabitem))
                                {
                                    this.CustomTabControl.Items.Remove(cstabitem);
                                }
                                if (!cstabitem.OwnWindow.CustomTabControl.Items.Contains(cstabitem))
                                {
                                    cstabitem.OwnWindow.CustomTabControl.Items.Add(cstabitem);
                                    if (cstabitem.OwnWindow.CustomTabControl.SelectedItem != null)
                                    {
                                        cstabitem.OwnWindow.Caption = cstabitem.Header.ToString();
                                        cstabitem.OwnWindow.DockingManager.HideTabPanel(cstabitem.OwnWindow);
                                    }
                                    else if (cstabitem.OwnWindow.CustomTabControl.Items.Count > 0)
                                    {
                                        cstabitem.OwnWindow.Caption = ((CustomTabItem)cstabitem.OwnWindow.CustomTabControl.Items[0]).Header.ToString();
                                        cstabitem.OwnWindow.DockingManager.HideTabPanel(cstabitem.OwnWindow);
                                    }
                                }
                            }
                            else
                            {
                                cstabitem.OwnWindow.Caption = cstabitem.Header.ToString();
                            }
                        }

                        foreach (CustomTabItem customTabItem in cstabitemCollection)
                        {
                            customTabItem.OwnWindow.StateTrans();

                            if (customTabItem.OwnWindow.DockingManager != null)
                                customTabItem.OwnWindow.DockingManager.FireDockStateChanged(customTabItem.OwnWindow.WindowChildElement, state,
                                                                                           customTabItem.OwnWindow.DockState);
                        }

                        cstabitemCollection.Clear();
                    }
                    else
                    {
                        StateTrans();

                        if (this.DockingManager != null)
                            this.DockingManager.FireDockStateChanged(this.WindowChildElement, state,
                                                                                       this.DockState);
                    }
                    _headerEventFired = false;

                    if (cstabItem != null)
                    {
                        cstabItem.IsSelected = true;
                        if (cstabItem.Parent is CustomTabControl)
                        {
                            if ((cstabItem.Parent as CustomTabControl).RelatedWindow != null)
                            {
                                (cstabItem.Parent as CustomTabControl).RelatedWindow.Caption = cstabItem.Header.ToString();
                            }
                        }
                    }
                }

                else if(!Cancelled)
                {
                    for (int i = 0; i < this.WindowCollection.Count; i++)
                    {
                        if (this.WindowCollection[i].MaximizedState == MaximizedState.Maximized)
                        {

                            if (this.WindowCollection[i].maximizeButton != null)
                            {
                                this.WindowCollection[i].InternallyChecked = true;
                                this.WindowCollection[i].maximizeButton.IsChecked = false;
                                this.WindowCollection[i].InternallyChecked = false;
                            }
                            this.WindowCollection[i].MaximizedState = MaximizedState.Restored;
                        }
                    }


                    for (int i = 0; i < this.WindowCollection.Count; i++)
                    {
                        if (this.WindowCollection[i].CustomTabControl != null)
                        {
                            if (this.WindowCollection[i].CustomTabControl != null)
                            {
                                if (this.WindowCollection[i].CustomTabControl.Items.Count > 1 && this.WindowCollection[i].DockState == DockState.Float)
                                {
                                    List<CustomTabItem> cstabitemCollection = new List<CustomTabItem>();
                                    foreach (CustomTabItem cstabitem in WindowCollection[i].CustomTabControl.Items)
                                    {
                                        if (cstabitem.OwnWindow != null)
                                        {
                                            if (!cstabitemCollection.Contains(cstabitem))
                                            {
                                                cstabitemCollection.Add(cstabitem);
                                            }
                                        }
                                    }
                                    foreach (CustomTabItem cstabitem in cstabitemCollection)
                                    {
                                        if (cstabitem.OwnWindow != WindowCollection[i])
                                        {
                                            if (WindowCollection[i].CustomTabControl.Items.Contains(cstabitem))
                                            {
                                                WindowCollection[i].CustomTabControl.Items.Remove(cstabitem);
                                            }
                                            if (!cstabitem.OwnWindow.CustomTabControl.Items.Contains(cstabitem))
                                            {
                                                cstabitem.OwnWindow.CustomTabControl.Items.Add(cstabitem);
                                                if (cstabitem.OwnWindow.CustomTabControl.SelectedItem != null)
                                                {
                                                    cstabitem.OwnWindow.Caption = cstabitem.Header.ToString();
                                                    cstabitem.OwnWindow.DockingManager.HideTabPanel(cstabitem.OwnWindow);
                                                }
                                                else if (cstabitem.OwnWindow.CustomTabControl.Items.Count > 0)
                                                {
                                                    cstabitem.OwnWindow.Caption = ((CustomTabItem)cstabitem.OwnWindow.CustomTabControl.Items[0]).Header.ToString();
                                                    cstabitem.OwnWindow.DockingManager.HideTabPanel(cstabitem.OwnWindow);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            cstabitem.OwnWindow.Caption = cstabitem.Header.ToString();
                                        }
                                    }
                                    cstabitemCollection.Clear();
                                }
                            }
                        }
                    }
                    for (int i = 0; i < this.WindowCollection.Count; i++)
                    {
                        if (this.WindowCollection[i].CustomTabControl != null)
                        {
                            if (this.WindowCollection[i].CustomTabControl != null && this.WindowCollection[i].DockState == DockState.Float)
                            {
                                if (this.WindowCollection[i].CustomTabControl.Items.Count == 1)
                                {
                                    this.WindowCollection[i].StateTrans();
                                    if (this.DockingManager != null)
                                        this.DockingManager.FireDockStateChanged(this.WindowCollection[i], state,
                                                                                                   WindowCollection[i].DockState);
                                }
                                else if (this.WindowCollection[i].CustomTabControl.Items.Count > 1)
                                {
                                    if (WindowCollection[i].CustomTabControl.SelectedItem != null)
                                    {
                                        WindowCollection[i].Caption = ((CustomTabItem)WindowCollection[i].CustomTabControl.SelectedItem).Header.ToString();
                                        WindowCollection[i].DockingManager.HideTabPanel(WindowCollection[i]);
                                    }
                                    else if (WindowCollection[i].CustomTabControl.Items.Count > 0)
                                    {
                                        WindowCollection[i].Caption = ((CustomTabItem)WindowCollection[i].CustomTabControl.Items[0]).Header.ToString();
                                        WindowCollection[i].DockingManager.HideTabPanel(WindowCollection[i]);
                                    }
                                    WindowCollection[i].StateTrans();
                                    if (this.DockingManager != null)
                                        this.DockingManager.FireDockStateChanged(this.WindowCollection[i], state,
                                                                                                   WindowCollection[i].DockState);
                                }
                            }
                        }
                    }
                    this.isDragging = false;
                    
                }
            }
            else
            {
                _timer.Start();
                // this.isDragging = false;
            }
        }

        /// <summary>
        /// Represents the Collection of grid.
        /// </summary>
        protected internal List<Grid> GridCollection;

        /// <summary>
        /// Adds the temp data.
        /// </summary>
        protected internal void AddTempData()
        {
            foreach (Grid g in GridCollection)
            {
                Window w = g.Children[0] as Window;
                g.Children.RemoveAt(0);
                TextBlock tb = new TextBlock();
                tb.Text = w.Caption;
                g.Children.Add(tb);
            }
        }

        /// <summary>
        /// Clears the specified grid.
        /// </summary>
        /// <param name="grid">The grid.</param>
        protected internal void Clear(Grid grid)
        {
            foreach (UIElement child in grid.Children)
            {
                if (child is Grid)
                {
                    Clear(child as Grid);
                }
                else
                {
                    if (child is Window)
                    {
                        //(child as Window).Caption = "dfdfdfdfc";


                        //(child as Window).Width = 100;
                        //(child as Window).Height = 100;
                        //TextBlock tb = new TextBlock();
                        //tb.Text = "fgfg";
                        //Grid temper = (child as Window).Parent as Grid;
                        //temper.Children.Remove(child);
                        //temper.Children.Add(tb);
                    }
                }
                if (grid.Children.Count == 1)
                {
                    if (grid.Children[0] is Window)
                    {
                        GridCollection.Add(grid);
                        //grid.Children.RemoveAt(0);
                        //TextBlock tb = new TextBlock();
                        //tb.Text = "fgfg";
                        //grid.Children.Add(tb);

                    }
                }
            }
        }

        /// <summary>
        /// Gets the window.
        /// </summary>
        /// <returns></returns>
        protected internal Window GetWindow()
        {
            Window w = null;
            string targetName = DockingManager.GetTargetNameInDockedMode(this.WindowChildElement).ToString();
            if (targetName != string.Empty)
            {
                for (int i = 1; i <= this.DockingManager.WindowCollection.Count; i++)
                {
                    if (this.DockingManager.WindowCollection[i].GetType() == typeof(Window))
                    {
                        if (DockingManager.GetTargetNameInDockedMode(this.WindowChildElement).ToString() == this.DockingManager.WindowCollection[i]._Caption)
                        {
                            w = this.DockingManager.WindowCollection[i];
                            break;
                        }
                    }
                }
            }
            else
            {
                w = this;
            }
            return w;
        }

        /// <summary>
        /// Checks the tar get name collection.
        /// </summary>
        /// <returns></returns>
        protected internal bool CheckTarGetNameCollection()
        {
            bool itempresented = false;
            for (int i = 1; i <= this.DockingManager.WindowCollection.Count; i++)
            {
                foreach (string s in this.TargetNameCollection)
                {
                    // List<UIElement> sidebuttonCollection = this.DockingManager.WindowCollection.Where(element => element.GetType() == typeof(Window) && ((Window)element).Visibility == Visibility.Visible).ToList();
                    if (this.DockingManager.WindowCollection[i]._Caption == s.Trim() && this.DockingManager.WindowCollection[i] != this && this.DockingManager.WindowCollection[i].Visibility == Visibility.Visible && this.DockingManager.WindowCollection[i].DockState == DockState.Dock)
                    {
                        if (this.CustomTabControl != null)
                        {
                            itempresented = true;
                            for (int m = this.CustomTabControl.Items.Count - 1; m >= 0; m--)
                            {
                                CustomTabItem custItem = (CustomTabItem)this.CustomTabControl.Items[m];
                                this.CustomTabControl.Items.Remove(custItem);
                                this.DockingManager.WindowCollection[i].CustomTabControl.Items.Add(custItem);
                            }

                            if (this.DockingManager.WindowCollection[i].CustomTabControl.primitiveTabPanel != null)
                            {
                                this.DockingManager.WindowCollection[i].CustomTabControl.TabPanelBorder.Visibility = Visibility.Visible;
                                this.DockingManager.WindowCollection[i].CustomTabControl.primitiveTabPanel.Visibility = Visibility.Visible;
                                this.DockingManager.WindowCollection[i].CustomTabControl.TabPanelBackground = this.DockingManager.TabPanelBackground;
                            }
                            ((Canvas)DockingManager).Children.Remove(this);
                            this.DockingManager.WindowCollection[i].Caption = ((CustomTabItem)this.DockingManager.WindowCollection[i].CustomTabControl.SelectedItem).Header.ToString();
                            break;
                        }
                    }
                }

                if (itempresented)
                {
                    break;
                }
            }
            return itempresented;
        }

        /// <summary>
        /// Adds the custom tab item when double clicked.
        /// </summary>
        /// <param name="temp">The temp.</param>
        /// <param name="custabItem">The custab item.</param>
        protected internal void AddCustomTabItemWhenDoubleClicked(Window temp, CustomTabItem custabItem)
        {
            CustomTabItem custab = custabItem;
            if (temp.CustomTabControl != null && custab != null)
            {
                if (!temp.CustomTabControl.Items.Contains(custab))
                {
                    this.CustomTabControl.Items.Remove(custab);
                    temp.CustomTabControl.Items.Add(custab);
                    if (temp.CustomTabControl.Items.Count > 1)
                    {
                        temp.DockingManager.VisibleTabpanel(temp, Visibility.Visible);
                    }
                    else
                    {
                        temp.DockingManager.HideTabPanel(temp);
                    }
                    if (this.CustomTabControl.Items.Count > 1)
                    {
                        temp.DockingManager.VisibleTabpanel(this, Visibility.Visible);
                    }
                    else
                    {
                        temp.DockingManager.HideTabPanel(this);
                    }
                }
            }
        }

        /// <summary>
        /// Arranges the custom tab item.
        /// </summary>
        /// <param name="temp">The temp.</param>
        protected internal void ArrangeCustomTabItem(Window temp)
        {
            if (this.CustomTabControl != null)
            {
                if (this.CustomTabControl.Items.Count == 1)
                {
                    CustomTabItem custab = this.CustomTabControl.Items[0] as CustomTabItem;
                    if (temp.CustomTabControl != null)
                    {
                        if (!temp.CustomTabControl.Items.Contains(custab))
                        {
                            this.CustomTabControl.Items.Remove(custab);
                            temp.CustomTabControl.Items.Add(custab);
                            //this.DockState = temp.DockState;
                        }
                    }
                }
                else if (this.CustomTabControl.Items.Count > 1)
                {
                    List<CustomTabItem> tempCustabitem = new List<CustomTabItem>();
                    for (int m = this.CustomTabControl.Items.Count - 1; m >= 0; m--)
                    {
                        CustomTabItem cstab = this.CustomTabControl.Items[m] as CustomTabItem;
                        if (!tempCustabitem.Contains(cstab) && cstab.OwnWindow == this)
                        {
                            this.CustomTabControl.Items.Remove(cstab);
                            tempCustabitem.Add(cstab);
                        }
                    }
                    for (int m = tempCustabitem.Count - 1; m >= 0; m--)
                    {
                        CustomTabItem cstab = tempCustabitem[m] as CustomTabItem;
                        if (temp.CustomTabControl != null && cstab.OwnWindow == this)
                        {
                            if (!temp.CustomTabControl.Items.Contains(cstab))
                            {
                                tempCustabitem.Remove(cstab);
                                temp.CustomTabControl.Items.Add(cstab);
                            }
                        }
                    }
                    if (temp != null)
                    {
                        //this.DockState = temp.DockState;
                    }
                }
            }
            this.DockingManager.HideTabPanel(temp);
            this.DockingManager.HideTabPanel(this);
        }

        /// <summary>
        /// Checks the dock window presented.
        /// </summary>
        /// <returns></returns>
        protected internal Window CheckDockWindowPresented()
        {
            Window temp = null;

            if (this.CurrentStateMain == StateMaintanance.Float || this.CurrentStateMain == StateMaintanance.WindowContainer || this.CurrentStateMain == StateMaintanance.TabWithContainer)
            {
                for (int i = 1; i <= this.DockingManager.WindowCollection.Count; i++)
                {
                    foreach (string s in this.TargetNameCollection)
                    {
                        // List<UIElement> sidebuttonCollection = this.DockingManager.WindowCollection.Where(element => element.GetType() == typeof(Window) && ((Window)element).Visibility == Visibility.Visible).ToList();
                        if (this.DockingManager.WindowCollection[i]._Caption == s.Trim() && this.DockingManager.WindowCollection[i] != this && this.DockingManager.WindowCollection[i].Visibility == Visibility.Visible)//&& this.DockingManager.WindowCollection[i].DockState == DockState.Dock)
                        {
                            if (this.DockingManager.WindowCollection[i].CurrentStateMain == this.PreviousStateMain)
                            {
                                if (this.DockingManager.WindowCollection[i].DockState == DockState.Dock)
                                {
                                    if (this.DockingManager.WindowCollection[i].DockManager.Parent is DockingManager)
                                    {
                                        temp = this.DockingManager.WindowCollection[i];
                                        ArrangeCustomTabItem(temp);
                                        if (((Canvas)DockingManager).Children.Contains(this))
                                        {
                                            ((Canvas)DockingManager).Children.Remove(this);
                                        }

                                        break;
                                    }
                                }
                            }
                        }
                    }
                    if (temp != null)
                    {
                        break;
                    }
                }
            }
            else if (this.CurrentStateMain == StateMaintanance.TabWithFloat)
            {
                for (int i = 1; i <= this.DockingManager.WindowCollection.Count; i++)
                {
                    foreach (string s in this.TargetNameCollection)
                    {
                        // List<UIElement> sidebuttonCollection = this.DockingManager.WindowCollection.Where(element => element.GetType() == typeof(Window) && ((Window)element).Visibility == Visibility.Visible).ToList();
                        if (this.DockingManager.WindowCollection[i]._Caption == s.Trim() && this.DockingManager.WindowCollection[i] != this && this.DockingManager.WindowCollection[i].Visibility == Visibility.Visible)//&& this.DockingManager.WindowCollection[i].DockState == DockState.Dock)
                        {
                            if (this.DockingManager.WindowCollection[i].CurrentStateMain == this.PreviousStateMain)
                            {
                                if (this.DockingManager.WindowCollection[i].DockState == DockState.Dock)
                                {
                                    if (this.DockingManager.WindowCollection[i].DockManager.Parent is DockingManager)
                                    {
                                        temp = this.DockingManager.WindowCollection[i];
                                        ArrangeCustomTabItem(temp);
                                        if (((Canvas)DockingManager).Children.Contains(this))
                                        {
                                            ((Canvas)DockingManager).Children.Remove(this);
                                        }
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    if (temp != null)
                    {
                        break;
                    }
                }
            }
            else if (this.DockState == DockState.Dock)
            {
                if (this.CustomTabControl != null)
                {
                    if (this.CustomTabControl.SelectedItem != null)
                    {
                        CustomTabItem custabitem = this.CustomTabControl.SelectedItem as CustomTabItem;

                        if (this.CustomTabControl.RelatedWindow.DockManager != null)
                        {
                            if (this.CustomTabControl.RelatedWindow.DockManager.Parent is WindowContainer)
                            {
                                for (int i = 1; i <= this.DockingManager.WindowCollection.Count; i++)
                                {
                                    foreach (string s in this.TargetNameCollection)
                                    {
                                        // List<UIElement> sidebuttonCollection = this.DockingManager.WindowCollection.Where(element => element.GetType() == typeof(Window) && ((Window)element).Visibility == Visibility.Visible).ToList();
                                        if (this.DockingManager.WindowCollection[i]._Caption == s.Trim() && this.DockingManager.WindowCollection[i] != this && this.DockingManager.WindowCollection[i].Visibility == Visibility.Visible)//&& this.DockingManager.WindowCollection[i].DockState == DockState.Dock)
                                        {
                                            if (this.DockingManager.WindowCollection[i].DockState == custabitem.OwnWindow.PreviousState)
                                            {
                                                if (this.DockingManager.WindowCollection[i].DockManager != null)
                                                {
                                                    if (this.DockingManager.WindowCollection[i].DockManager.Parent is DockingManager)
                                                    {
                                                        temp = this.DockingManager.WindowCollection[i];
                                                        if (temp.DockingManager._parentTabbedWindow != temp.DockingManager.TabbedWindow)
                                                        {
                                                            AddCustomTabItemWhenDoubleClicked(temp, custabitem);
                                                        }
                                                        break;
                                                    }
                                                }

                                            }
                                        }
                                    }
                                    if (temp != null)
                                    {
                                        break;
                                    }
                                }
                            }
                            else if (this.CustomTabControl.RelatedWindow.DockManager.Parent is DockingManager)
                            {
                                for (int i = 1; i <= this.DockingManager.WindowCollection.Count; i++)
                                {
                                    foreach (string s in this.TargetNameCollection)
                                    {
                                        // List<UIElement> sidebuttonCollection = this.DockingManager.WindowCollection.Where(element => element.GetType() == typeof(Window) && ((Window)element).Visibility == Visibility.Visible).ToList();
                                        if (this.DockingManager.WindowCollection[i]._Caption == s.Trim() && this.DockingManager.WindowCollection[i] != this && this.DockingManager.WindowCollection[i].Visibility == Visibility.Visible)//&& this.DockingManager.WindowCollection[i].DockState == DockState.Dock)
                                        {
                                            if (this.DockingManager.WindowCollection[i].DockState == custabitem.OwnWindow.PreviousState)
                                            {
                                                if (this.DockingManager.WindowCollection[i].DockManager != null)
                                                {
                                                    if (this.DockingManager.WindowCollection[i].DockManager.Parent is WindowContainer && this.DockingManager.WindowCollection[i].TargetNameCollection == this.TargetNameCollection)
                                                    {
                                                        temp = this.DockingManager.WindowCollection[i];
                                                        if (temp.DockingManager._parentTabbedWindow != temp.DockingManager.TabbedWindow)
                                                        {
                                                            AddCustomTabItemWhenDoubleClicked(temp, custabitem);
                                                        }
                                                        break;
                                                    }
                                                }

                                            }
                                        }
                                    }
                                    if (temp != null)
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return temp;
        }

        /// <summary>
        /// Checks the dock with hidden window presented.
        /// </summary>
        /// <returns></returns>
        protected internal Window CheckDockWithHiddenWindowPresented()
        {
            Window temp = null;
            for (int i = 1; i <= this.DockingManager.WindowCollection.Count; i++)
            {
                foreach (string s in this.TargetNameCollection)
                {
                    // List<UIElement> sidebuttonCollection = this.DockingManager.WindowCollection.Where(element => element.GetType() == typeof(Window) && ((Window)element).Visibility == Visibility.Visible).ToList();
                    if (this.DockingManager.WindowCollection[i]._Caption == s.Trim() && this.DockingManager.WindowCollection[i] != this && this.DockingManager.WindowCollection[i].Visibility == Visibility.Visible && this.DockingManager.WindowCollection[i].DockState == DockState.Hidden && ((Canvas)DockingManager).Children.Contains(this.DockingManager.WindowCollection[i]))
                    {
                        temp = this.DockingManager.WindowCollection[i];
                        break;
                    }
                }

                if (temp != null)
                {
                    break;
                }
            }
            return temp;
        }

        /// <summary>
        /// Sets the float.
        /// </summary>
        protected internal void SetFloat()
        {
            this.PreviousDockSide = Dock.None;
            if (this.Parent != null)
            {
                if (this.Parent.GetType() == typeof(Grid) && this.FloatHeight == 0.0)
                {
                    this.Width = (this.Parent as Grid).ActualWidth;
                    this.Height = (this.Parent as Grid).ActualHeight;
                    _leftValue = 0.0;
                    _topValue = 0.0;
                    if (this.DockManager != null)
                    {
                        if (this.DockManager.Parent is DockingManager)
                        {
                            Grid temp = GetParent((UIElement)this.Parent);
                        }
                        else
                        {
                            Grid temp = GetParent((UIElement)this.Parent);
                            _attachedParentWindow = null;
                            Window _w = GetParent(this);
                        }
                    }

                    Canvas.SetLeft(this, _leftValue);
                    Canvas.SetTop(this, _topValue);
                    UpDateFloatWindowSize();
                    FloatHeight = this.Height;
                    FloatWidth = this.Width;
                    Canvas.SetZIndex(this, Window.currentZIndex);
                }
            }

            ChangeState(DockState.Hidden);
            if (!((Canvas)DockingManager).Children.Contains(this))
            {
                try
                {
                    if (this.Parent != null)
                    {
                        if (this.Parent.GetType() == typeof(Grid))
                        {
                            (this.Parent as Grid).Children.Remove(this);
                            ((Canvas)DockingManager).Children.Add(this);
                            this.ApplyBorderForFloatWindow();
                        }
                    }
                    else
                    {
                        ((Canvas)DockingManager).Children.Add(this);
                        this.ApplyBorderForFloatWindow();
                        UpDateFloatWindowSize();
                    }
                }
                catch
                {
                }

                if (FloatHeight <= 0 || FloatWidth <= 0)
                {
                    Canvas.SetLeft(this, this.initialDragPoint.X);
                    Canvas.SetTop(this, this.initialDragPoint.Y);
                }
                else
                {
                    Canvas.SetLeft(this, LeftPosition);
                    Canvas.SetTop(this, TopPosition);
                    this.Height = FloatHeight;
                    this.Width = FloatWidth;
                }

                this.DockingManager.RemoveDock(this);
            }
        }

        /// <summary>
        /// Fires when the left mouse button goes up on the caption bar
        /// </summary>
        /// <param name="sender">Sender is a border.</param>
        /// <param name="e">EventArgs of Mouse Button.</param>
        void captionBar_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            UIElement parent = (UIElement)VisualTreeHelper.GetParent((UIElement)this);
            isMoved = false;
            bool docked = false;
            if (parent != null)
            {
                if (DraggingEnabled && parent.GetType() != typeof(WindowContainer))
                {
                    ((FrameworkElement)sender).ReleaseMouseCapture();
                    isDragging = false;
                    if (this.DockingManager.PopUpCollection.Count > 1)
                    {
                        for (int i = 0; i < this.DockingManager.PopUpCollection.Count; i++)
                        {

                            this.DockingManager.PopUpCollection[i].IsOpen = false;
                            this.DockingManager.popUpLoaded = false;
                            this.DockingManager.IsDockHintsShowing = false;
                            if (((Canvas)DockingManager).Children.Contains(this.DockingManager.PopUpCollection[i]))
                            {
                                ((Canvas)DockingManager).Children.Remove(this.DockingManager.PopUpCollection[i]);
                            }
                        }
                    }

                    this.DockingManager.HidePopUpInsideWindow();
                    this.DockingManager.popUpLoaded = false;
                    this.DockingManager.IsDockHintsShowing = false;
                    if (popupLoaded && hostElementOnPopUp)
                    {
                        this.DockingManager.RemovePopUp();
                        hostElementOnPopUp = false;
                        popupLoaded = false;
                        this.DockingManager.RecentlyMouseHoveredSidePanel = null;
                        DockAllowEventArgs args = new DockAllowEventArgs(this.WindowChildElement, this.DockingManager as UIElement);
                        this.DockingManager.FireDockAllow(args);
                        docked = true;
                        if (!args.Cancel)
                        {
                            DockStateChangingEventArgs arg = new DockStateChangingEventArgs(this.WindowChildElement, DockState.Float, DockState.Dock,DockSide.Left);
                            this.DockingManager.FireDockStateChanging(arg);
                            if (!arg.Cancel)
                            {
                                this.DockingManager.SetDockByPopUp(this, HostElementOnPopUpPosition);
                                this.DockingManager.FireDockStateChanged(this.WindowChildElement, DockState.Float, DockState.Dock);
                            }
                        }  
                    }
                    else if (this.DockState == DockState.Float && !this.CanFloat && !(this.DockingManager.mouseHoveredWindow.leftshadowPopUp.IsOpen || this.DockingManager.mouseHoveredWindow.rightshadowPopUp.IsOpen || this.DockingManager.mouseHoveredWindow.bottomshadowPopUp.IsOpen || this.DockingManager.mouseHoveredWindow.topshadowPopUp.IsOpen || this.DockingManager.mouseHoveredWindow.centershadowPopUp.IsOpen))
                    {
                        StateTrans();
                    }

                    this.DockingManager.popupLoadedInsideWindow = false;
                    this.DockingManager.HidePopUpInsideWindow();
                    if (!docked && this.DockingManager.mouseHoveredWindow != null && (this.DockingManager.mouseHoveredWindow.leftshadowPopUp.IsOpen || this.DockingManager.mouseHoveredWindow.rightshadowPopUp.IsOpen || this.DockingManager.mouseHoveredWindow.bottomshadowPopUp.IsOpen || this.DockingManager.mouseHoveredWindow.topshadowPopUp.IsOpen || this.DockingManager.mouseHoveredWindow.centershadowPopUp.IsOpen))
                    {
                        bool canDockStateChange = true;
                        DockAllowEventArgs args = new DockAllowEventArgs(this.WindowChildElement, this.DockingManager.mouseHoveredWindow.WindowChildElement);
                        this.DockingManager.FireDockAllow(args);
                        canDockStateChange = !args.Cancel;
                        if (canDockStateChange)
                        {
                            DockSide side = DockSide.Left;
                            if(DockingManager.mouseHoveredWindow.leftshadowPopUp.IsOpen)
                                side=DockSide.Left;
                            if (DockingManager.mouseHoveredWindow.rightshadowPopUp.IsOpen)
                                side = DockSide.Right;
                            if (DockingManager.mouseHoveredWindow.bottomshadowPopUp.IsOpen)
                                side = DockSide.Bottom;
                            if (DockingManager.mouseHoveredWindow.topshadowPopUp.IsOpen)
                                side = DockSide.Top;
                            if (DockingManager.mouseHoveredWindow.centershadowPopUp.IsOpen)
                                side = DockSide.Tabbed;

                            DockStateChangingEventArgs arg = new DockStateChangingEventArgs(this.WindowChildElement, DockState.Float, DockState.Dock,side);
                            this.DockingManager.FireDockStateChanging(arg);
                            canDockStateChange = !arg.Cancel;
                        }
                        if (!canDockStateChange)
                        {
                            this.DockingManager.mouseHoveredWindow.leftshadowPopUp.IsOpen = false;
                            this.DockingManager.mouseHoveredWindow.rightshadowPopUp.IsOpen = false;
                            this.DockingManager.mouseHoveredWindow.bottomshadowPopUp.IsOpen = false;
                            this.DockingManager.mouseHoveredWindow.topshadowPopUp.IsOpen = false;
                            this.DockingManager.mouseHoveredWindow.centershadowPopUp.IsOpen = false;
                        }
                    }
                    else if (this.DockingManager.mouseHoveredWindow != null)
                    {
                        this.DockingManager.mouseHoveredWindow.leftshadowPopUp.IsOpen = false;
                        this.DockingManager.mouseHoveredWindow.rightshadowPopUp.IsOpen = false;
                        this.DockingManager.mouseHoveredWindow.bottomshadowPopUp.IsOpen = false;
                        this.DockingManager.mouseHoveredWindow.topshadowPopUp.IsOpen = false;
                        this.DockingManager.mouseHoveredWindow.centershadowPopUp.IsOpen = false;
                    }
                    docked = false;
                    this.DockingManager.HideShadowPopUpInsideWindow("Up", e.GetPosition(Application.Current.RootVisual));
                    this.DockingManager.resizedWindow = this;
                    if (((Canvas)DockingManager).Children.Contains(this) && this.DockState == DockState.Dock && this.WindowDockPin == DockPin.UnPinned)
                    {
                        if (this.isMoved)
                        {
                            this.isMoved = false;
                        }
                    }
                }
                else if (DraggingEnabled && this.DockState == DockState.Dock && parent.GetType() == typeof(WindowContainer))
                {
                    this.isDragging = false;
                }
            }

            if (WindowChildElement != null)
            {
                NoHeaderVisibility(DockingManager.GetNoHeader(WindowChildElement), DockingManager.GetHeaderHeight(WindowChildElement));
            }
        }

        /// <summary>
        /// Shows the allfloat window.
        /// </summary>
        protected internal void ShowAllfloatWindow()
        {
            foreach (UIElement element in ((Canvas)DockingManager).Children)
            {
                if (element.Visibility == Visibility.Visible)
                {
                    if (element.GetType() == typeof(Window) && ((Window)element).WindowDockPin == DockPin.UnPinned && ((Window)element).IsremovedFromParent)
                    {
                        Canvas.SetZIndex(((Window)element), Window.currentZIndex + 2);
                        ((Window)element).UpDateFloatWindowSize();
                    }
                }
            }
        }

        /// <summary>
        /// Represents the host element.
        /// </summary>
        protected internal bool hostElementOnPopUp = false;

        /// <summary>
        /// Gets or sets the host element on pop up position.
        /// </summary>
        /// <value>The host element on pop up position.</value>
        protected internal Dock HostElementOnPopUpPosition
        {
            get;
            set;
        }

        /// <summary>
        /// Represents the attached parent window.
        /// </summary>
        protected internal Window _attachedParentWindow = null;

        /// <summary>
        /// Gets the parent.
        /// </summary>
        /// <param name="_w">The _w.</param>
        /// <returns>The Parent Window.</returns>
        protected internal Window GetParent(Window _w)
        {
            if (_w.DockManager != null)
            {
                if (_w.DockManager.Parent.GetType() == typeof(WindowContainer))
                {
                    GetParent((_w.DockManager.Parent as WindowContainer)._window);
                    //_w = (_w.DockManager.Parent as WindowContainer)._window;
                    if (((Canvas)DockingManager).Children.Contains(_w))
                    {
                        if (_attachedParentWindow == null)
                        {
                            _attachedParentWindow = _w;
                            _leftValue = _leftValue + Canvas.GetLeft(_w);
                            _topValue = _topValue + Canvas.GetTop(_w);
                        }
                    }
                    else
                    {
                        if (_attachedParentWindow == null)
                        {
                            _attachedParentWindow = _w;
                        }
                        GetParent((UIElement)_w.Parent);
                    }
                }
                else
                {
                    if (((Canvas)DockingManager).Children.Contains(_w))
                    {
                        if (_attachedParentWindow == null)
                        {
                            _attachedParentWindow = _w;
                            _leftValue = _leftValue + Canvas.GetLeft(_w);
                            _topValue = _topValue + Canvas.GetTop(_w);
                        }
                    }
                    else
                    {
                        if (_attachedParentWindow == null)
                        {
                            _attachedParentWindow = _w;
                        }
                        GetParent((UIElement)_w.Parent);
                    }
                }
            }
            else
            {
                if (((Canvas)DockingManager).Children.Contains(_w))
                {
                    if (_attachedParentWindow == null)
                    {
                        _attachedParentWindow = _w;
                        _leftValue = _leftValue + Canvas.GetLeft(_w);
                        _topValue = _topValue + Canvas.GetTop(_w);
                    }
                }
                else
                {
                    if (_attachedParentWindow == null)
                    {
                        _attachedParentWindow = _w;
                    }
                    GetParent((UIElement)_w.Parent);
                }
            }
            return _attachedParentWindow;
        }

        /// <summary>
        /// Shows the outer drag provider.
        /// </summary>
        /// <param name="position">The position.</param>
        protected internal void ShowOuterDragProvider(Point position)
        {
            if (this.DockableState != DockableState.Floating)
            {
                this.DockingManager.CreatePopUp();
                double leftimageWidth = ((Image)((Popup)this.DockingManager.PopUpCollection[0]).Child).ActualWidth;
                double leftimageHeight = ((Image)((Popup)this.DockingManager.PopUpCollection[0]).Child).ActualHeight;
                double rightimageWidth = ((Image)((Popup)this.DockingManager.PopUpCollection[1]).Child).ActualWidth;
                double rightimageHeight = ((Image)((Popup)this.DockingManager.PopUpCollection[1]).Child).ActualHeight;
                double topimageWidth = ((Image)((Popup)this.DockingManager.PopUpCollection[2]).Child).ActualWidth;
                double topimageHeight = ((Image)((Popup)this.DockingManager.PopUpCollection[2]).Child).ActualHeight;
                double bottomimageWidth = ((Image)((Popup)this.DockingManager.PopUpCollection[3]).Child).ActualWidth;
                double bottomimageHeight = ((Image)((Popup)this.DockingManager.PopUpCollection[3]).Child).ActualHeight;
                if (leftimageWidth > 0 || rightimageWidth > 0 || topimageWidth > 0 || bottomimageWidth > 0)
                {
                    Uri uri = null;
                    ImageSource imgSource = null;
                    if(!DockingManager.IsDockHintsShowing)
                    {
                        for (int i = 0; i < this.DockingManager.PopUpCollection.Count; i++)
                        {
                            DockingManager.IsDockHintsShowing = true;
                            switch (i)
                            {
                                case 0:
                                    uri = new Uri(this.DockingManager.LeftImagePath, UriKind.Relative);
                                    imgSource = new BitmapImage(uri);
                                    ((Image)((Popup)this.DockingManager.PopUpCollection[0]).Child).Source = imgSource;
                                    break;

                                case 1:
                                    uri = new Uri(this.DockingManager.RightImagePath, UriKind.Relative);
                                    imgSource = new BitmapImage(uri);
                                    ((Image)((Popup)this.DockingManager.PopUpCollection[1]).Child).Source = imgSource;
                                    break;

                                case 2:
                                    uri = new Uri(this.DockingManager.TopImagePath, UriKind.Relative);
                                    imgSource = new BitmapImage(uri);
                                    ((Image)((Popup)this.DockingManager.PopUpCollection[2]).Child).Source = imgSource;
                                    break;

                                case 3:
                                    uri = new Uri(this.DockingManager.BottomImagePath, UriKind.Relative);
                                    imgSource = new BitmapImage(uri);
                                    ((Image)((Popup)this.DockingManager.PopUpCollection[3]).Child).Source = imgSource;
                                    break;
                            }
                        }
                    }

                    if (position.X > (this.DockingManager.ActualWidth / 2.0) && position.X < ((this.DockingManager.ActualWidth / 2.0) + topimageWidth) && (position.Y < topimageHeight && position.Y <= this.DockingManager.ActualHeight) && position.Y >= 0.0)
                    {
                        uri = new Uri(this.DockingManager.TopOverImagePath, UriKind.Relative);
                        imgSource = new BitmapImage(uri);
                        ((Image)((Popup)this.DockingManager.PopUpCollection[2]).Child).Source = imgSource;
                        DockingManager.IsDockHintsShowing = false;
                        if (!popupLoaded)
                        {
                            this.DockingManager.DisplayDockingPopup(2, this);
                            hostElementOnPopUp = true;
                            HostElementOnPopUpPosition = Dock.Top;
                            popupLoaded = true;
                        }
                    }
                    else if (position.X > (this.DockingManager.ActualWidth / 2.0) && position.X < ((this.DockingManager.ActualWidth / 2.0) + bottomimageWidth) && (position.Y > (this.DockingManager.ActualHeight - bottomimageHeight) && position.Y <= this.DockingManager.ActualHeight) && position.Y >= 0.0)
                    {
                        uri = new Uri(this.DockingManager.BottomOverImagePath, UriKind.Relative);
                        imgSource = new BitmapImage(uri);
                        ((Image)((Popup)this.DockingManager.PopUpCollection[3]).Child).Source = imgSource;
                        DockingManager.IsDockHintsShowing = false;
                        if (!popupLoaded)
                        {
                            this.DockingManager.DisplayDockingPopup(3, this);
                            hostElementOnPopUp = true;
                            HostElementOnPopUpPosition = Dock.Bottom;
                            popupLoaded = true;
                        }
                    }
                    else if (position.Y > (this.DockingManager.ActualHeight / 2.0) && position.Y < ((this.DockingManager.ActualHeight / 2.0) + leftimageHeight) && (position.X < leftimageWidth && position.X <= this.DockingManager.ActualWidth) && position.X >= 0.0)
                    {
                        uri = new Uri(this.DockingManager.LeftOverImagePath, UriKind.Relative);
                        imgSource = new BitmapImage(uri);
                        ((Image)((Popup)this.DockingManager.PopUpCollection[0]).Child).Source = imgSource;
                        DockingManager.IsDockHintsShowing = false;
                        if (!popupLoaded)
                        {
                            this.DockingManager.DisplayDockingPopup(0, this);
                            hostElementOnPopUp = true;
                            HostElementOnPopUpPosition = Dock.Left;
                            popupLoaded = true;
                        }
                    }
                    else if (position.Y > (this.DockingManager.ActualHeight / 2.0) && position.Y < ((this.DockingManager.ActualHeight / 2.0) + rightimageHeight) && (position.X > (this.DockingManager.ActualWidth - rightimageWidth) && position.X <= this.DockingManager.ActualWidth) && position.X >= 0.0)
                    {
                        uri = new Uri(this.DockingManager.RightOverImagePath, UriKind.Relative);
                        imgSource = new BitmapImage(uri);
                        ((Image)((Popup)this.DockingManager.PopUpCollection[1]).Child).Source = imgSource;
                        DockingManager.IsDockHintsShowing = false;
                        if (!popupLoaded)
                        {
                            this.DockingManager.DisplayDockingPopup(1, this);
                            hostElementOnPopUp = true;
                            HostElementOnPopUpPosition = Dock.Right;
                            popupLoaded = true;
                        }
                    }
                    else if (popupLoaded)
                    {
                        this.DockingManager.RemovePopUp();
                        hostElementOnPopUp = false;
                        popupLoaded = false;
                    }

                }
            }
        }

        /// <summary>
        /// Windows the move from dock layout.
        /// </summary>
        /// <param name="isParentDockingManager">if set to <c>true</c> [is parent docking manager].</param>
        /// <param name="parentIsGridElement">if set to <c>true</c> [parent is grid element].</param>
        /// <param name="isWindowContainerPresent">if set to <c>true</c> [is window container present].</param>
        protected internal void WindowMoveFromDockLayout( ref bool isParentDockingManager, ref bool parentIsGridElement, ref bool isWindowContainerPresent)
        {
            DockManager dm = this.DockingManager.GetDockingGrid(((UIElement)this));
            if (dm.Parent is DockingManager)
            {
                isParentDockingManager = true;
            }
            if (this.Parent is Grid)
            {
                parentIsGridElement = true;
            }
            DockManager dockManager = this.DockManager;

            if (this.DockManager.Parent is WindowContainer)
            {
                isWindowContainerPresent = true;
            }
            else if (this.OldValueDockManager != null)
            {
                if (this.OldValueDockManager.Parent is WindowContainer)
                {
                    this.DockingManager.UpdateDockModeTarget(this, true);
                }
            }

            if (!isWindowContainerPresent)
            {
                if (this._Caption != string.Empty)
                {
                    if (this.CustomTabControl != null)
                    {
                        for (int i = 0; i < this.CustomTabControl.Items.Count; i++)
                        {
                            CustomTabItem cstabItem = this.CustomTabControl.Items[i] as CustomTabItem;
                            if (cstabItem.OwnWindow == this)
                            {
                                this.DockingManager.SetboolValueWithTargetName(this, string.Empty, DockState.Float);
                                DockingManager.SetTargetNameInFloatingMode(this.WindowChildElement, string.Empty);
                            }
                            else
                            {
                                this.DockingManager.SetboolValueWithSideInMode(cstabItem.OwnWindow, Dock.Tabbed, DockState.Float);
                                DockingManager.SetSideInFloatMode(cstabItem.OwnWindow.WindowChildElement, Dock.Tabbed);
                                this.DockingManager.SetboolValueWithTargetName(cstabItem.OwnWindow, this._Caption, DockState.Float);
                                DockingManager.SetTargetNameInFloatingMode(cstabItem.OwnWindow.WindowChildElement, this._Caption);
                            }
                        }
                    }
                }
            }
            else
            {
                IEnumerable<Window> windowquery = (dockManager.Parent as WindowContainer)._window.WindowCollection.Where(tempwindow => ((Window)tempwindow).Visibility == Visibility.Visible && ((Window)tempwindow).CustomTabControl.Items.Count > 0 && DockingManager.GetSideInFloatMode(((Window)tempwindow).WindowChildElement) != Dock.Tabbed);
                if (windowquery.Count() == 2)
                {
                    foreach (Window w in windowquery)
                    {
                        this.DockingManager.SetboolValueWithTargetName(w, string.Empty, DockState.Float);
                        DockingManager.SetTargetNameInFloatingMode(w.WindowChildElement, string.Empty);
                    }
                }
            }


            //this.DockingManager.UpdateDockModeTarget(this, isWindowContainerPresent);
            SetHiddenWindowHeaderMove();
            if (isWindowContainerPresent)
            {
                this.DockingManager.UpdateMoveToTargetName(this, isWindowContainerPresent);
            }

            if (isWindowContainerPresent)
            {
                Window windowContain = (dockManager.Parent as WindowContainer)._window;
                int numberofFloatWindow = windowContain.NumberofFloatChildren(windowContain, dockManager);
                int numberofChildren = this.NumberofChildren(windowContain, dockManager);
                if (numberofFloatWindow == 1 && numberofChildren == 0)
                {
                    Window w = this.DockingManager.GetWindow(DockingManager.GetTargetNameInFloatingMode(this.WindowChildElement));
                    if (w != null)
                    {
                        w = this.DockingManager.GetExactParentWindowForFloatTabWindow(w);
                        this.DockingManager.SetboolValueWithTargetName(this, string.Empty, DockState.Float);
                        DockingManager.SetTargetNameInFloatingMode(this.WindowChildElement, string.Empty);
                    }
                }
                this.DockingManager.SetboolValueWithTargetName(this, string.Empty, DockState.Float);
                DockingManager.SetTargetNameInFloatingMode(this.WindowChildElement, string.Empty);
                if (dockManager != null)
                {
                    if (dockManager.Parent is WindowContainer)
                    {
                        if ((dockManager.Parent as WindowContainer)._window.WindowCollection.Contains(this))
                        {
                            (dockManager.Parent as WindowContainer)._window.WindowCollection.Remove(this);
                        }
                        if (this.CustomTabControl != null)
                        {
                            foreach (CustomTabItem cstabitem in this.CustomTabControl.Items)
                            {
                                if (cstabitem.OwnWindow != this)
                                {
                                    if ((dockManager.Parent as WindowContainer)._window.WindowCollection.Contains(cstabitem.OwnWindow))
                                    {
                                        (dockManager.Parent as WindowContainer)._window.WindowCollection.Remove(cstabitem.OwnWindow);
                                    }
                                    cstabitem.OwnWindow.OldValueDockManager = null;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                if (this.OldValueDockManager != null)
                {
                    if (this.OldValueDockManager.Parent is WindowContainer)
                    {
                        if ((this.OldValueDockManager.Parent as WindowContainer)._window.WindowCollection.Contains(this))
                        {
                            (this.OldValueDockManager.Parent as WindowContainer)._window.WindowCollection.Remove(this);
                        }
                        if (this.CustomTabControl != null)
                        {
                            foreach (CustomTabItem cstabitem in this.CustomTabControl.Items)
                            {
                                if (cstabitem.OwnWindow != this)
                                {
                                    if ((this.OldValueDockManager.Parent as WindowContainer)._window.WindowCollection.Contains(cstabitem.OwnWindow))
                                    {
                                        (this.OldValueDockManager.Parent as WindowContainer)._window.WindowCollection.Remove(cstabitem.OwnWindow);
                                    }
                                    cstabitem.OwnWindow.OldValueDockManager = null;
                                }
                            }
                            this.OldValueDockManager = null;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Handles the MouseMove event of the captionBar control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        void captionBar_MouseMove(object sender, MouseEventArgs e)
        {
            UIElement parent = (UIElement)VisualTreeHelper.GetParent((UIElement)this);
            if (parent != null)
            {
                if (isDragging)
                {
                    DragAllowEventArgs args = new DragAllowEventArgs(this.WindowChildElement);
                    this.DockingManager.FireDragAllow(args);
                    isDragging = !args.Cancel;
                    if (isDragging && this.DockState != DockState.Float)
                    {
                        DockStateChangingEventArgs arg = new DockStateChangingEventArgs(this.WindowChildElement, this.DockState, DockState.Float, DockingManager.GetSide(WindowChildElement, DockState.Dock));
                        this.DockingManager.FireDockStateChanging(arg);
                        isDragging = !arg.Cancel;
                    }
                }
                if (this.isDragging && parent.GetType() != typeof(WindowContainer) && this.CanDrag && !(bool)this.dockToggle.IsChecked && !isResizing)
                {
                    Point position = e.GetPosition(this.DockingManager);
                    this.dragPoint = e.GetPosition(this.DockingManager);
                    bool allowDrag = true;
                    if (this.MaximizedState == MaximizedState.Maximized && this.DockState == DockState.Float && !(this.DockManager.Parent is WindowContainer))
                    {
                        this.setWhileRestoring = true;
                        this.maximizeButton.IsChecked = false;
                        this.setWhileRestoring = false;
                    }
                    else if (this.MaximizedState == MaximizedState.Maximized && this.DockState == DockState.Float && this.DockManager.Parent is WindowContainer)
                    {
                        this.setWhileRestoring = true;
                        this.maximizeButton.IsChecked = false;
                        this.setWhileRestoring = false;

                        this.FloatHeight = this.PreviousFloatHeight;
                        this.FloatWidth = this.PreviousFloatWidth;

                        this.Height = this.PreviousFloatHeight;
                        this.Width = this.PreviousFloatWidth;

                        double xValue = this.dragPoint.X - this.FloatWidth / 2;

                        this.initialWindowLocation.X = xValue;

                        Canvas.SetLeft(this, xValue);
                    }
                    else if (this.MaximizedState == MaximizedState.Maximized && this.DockState == DockState.Dock)
                    {                        
                        this.setWhileRestoring = true;
                        this.maximizeButton.IsChecked = false;
                        this.setWhileRestoring = false;

                        this.FloatHeight = this.PreviousHeight;
                        this.FloatWidth = this.PreviousWidth;

                        double xValue = this.dragPoint.X - this.FloatWidth / 2;

                        this.initialWindowLocation.X = xValue;

                        Canvas.SetLeft(this, xValue);
                    }
                    
                    if (!((Canvas)DockingManager).Children.Contains(this))
                    {
                        allowDrag = true;
                        bool isParentDockingManager = false;
                        bool parentIsGridElement = false;
                        bool isWindowContainerPresent = false;
                        DockState dockState = this.DockState;
                        WindowMoveFromDockLayout(ref isParentDockingManager, ref parentIsGridElement, ref isWindowContainerPresent);
                        DockManager dockManager = this.DockManager;

                        this.LeftPosition = initialWindowLocation.X + position.X - initialDragPoint.X;
                        this.TopPosition = initialWindowLocation.Y + position.Y - initialDragPoint.Y;
                        this.DockingManager.FireDockStateChanged(this.WindowChildElement, dockState, DockState.Float);

                        Canvas.SetZIndex(this, ++currentZIndex);
                    }

                    Point p = e.GetPosition(this.DockingManager);
                    if (!((p.X >= 5 && p.X <= this.DockingManager.ActualWidth - 30) && (p.Y > 5 && p.Y <= this.DockingManager.ActualHeight - 30)))
                    {
                        Point parent_Position = e.GetPosition(this.DockingManager);
                        if (p.X <= 5)
                        {
                            position.X = 4;
                        }
                        if (p.X > this.DockingManager.ActualWidth - 30)
                        {
                            position.X = this.DockingManager.ActualWidth - 30;
                        }
                        if (p.Y <= 5)
                        {
                            position.Y = parent_Position.Y - p.Y;
                        }
                        if (p.Y > this.DockingManager.ActualHeight - 5)
                        {
                            position.Y = this.DockingManager.ActualHeight - 5;
                        }
                    }
                    else
                    {
                        allowDrag = true;
                    }
                    if (allowDrag)
                    {
                        UpDateFloatWindowSize();
                        double x = initialWindowLocation.X + position.X - initialDragPoint.X;
                        Canvas.SetLeft(this, x);
                        double y = initialWindowLocation.Y + position.Y - initialDragPoint.Y;
                        Canvas.SetTop(this, y);
                        this.DockingManager.ClearAllShadowPopUp();
                        InvalidateWindowBounds();
                        if (this.CanDock)
                        {
                            if(this.DockingManager.OuterDockAbility!=OuterDockAbility.None)
                                ShowOuterDragProvider(position);

                            if (this.DragMoved != null && this.CanDock && this.DockableState != DockableState.Floating)
                            {
                                this.DragMoved(this, new DragEventArgs(position.X - this.lastDragPosition.X, position.Y - this.lastDragPosition.Y, e, string.Empty));
                            }
                        }
                    }
                }
            }
        }
        #endregion
        #region Resize Functions

        /// <summary>
        /// Represents the Hot spot width.
        /// </summary>
        private const int HotSpotWidth = 3;

        /// <summary>
        /// states if a resize operation is in progress
        /// </summary>
        protected internal bool isResizing = false;

        /// <summary>
        /// Represents the resize anchor.
        /// </summary>
        private ResizeAnchor resizeAnchor = ResizeAnchor.None;

        /// <summary>
        /// Represents the resize point.
        /// </summary>
        private Point initialResizePoint;

        /// <summary>
        /// Represents the initial window size.
        /// </summary>
        private Size initialWindowSize;

        /// <summary>
        /// Represents the Minimum window width.
        /// </summary>
        private const int MinWindowWidth = 60;

        /// <summary>
        /// defines where the user mouse is positioned inside the control
        /// </summary>
        private enum ResizeAnchor
        {
            /// <summary>
            /// Control is Resized to the left side of it's container.
            /// </summary>
            Left,

            /// <summary>
            /// Control is Resized to the TopLeft side of it's container.
            /// </summary>
            TopLeft,

            /// <summary>
            /// Control is Resized to the Top side of it's container.
            /// </summary>
            Top,

            /// <summary>
            /// Control is Resized to the TopRight side of it's container.
            /// </summary>
            TopRight,

            /// <summary>
            /// Control is Resized to the Right side of it's container.
            /// </summary>
            Right,

            /// <summary>
            /// Control is Resized to the BottomRight side of it's container.
            /// </summary>
            BottomRight,

            /// <summary>
            /// Control is Resized to the Bottom side of it's container.
            /// </summary>
            Bottom,

            /// <summary>
            /// Control is Resized to the BottomLeft side of it's container.
            /// </summary>
            BottomLeft,

            /// <summary>
            /// Control is Resized to the None side of it's container.
            /// </summary>
            None
        }

        /// <summary>
        /// Gets or sets a value indicating whether [resize enabled].
        /// </summary>
        /// <value><c>true</c> if [resize enabled]; otherwise, <c>false</c>.</value>
        public bool ResizeEnabled
        {
            get
            {
                if (this.CustomTabControl != null && this.CustomTabControl.SelectedItem != null && (this.CustomTabControl.SelectedItem as CustomTabItem).OwnWindow != null && (this.CustomTabControl.SelectedItem as CustomTabItem).OwnWindow.WindowChildElement != null)
                {
                    return DockingManager.GetCanResize((this.CustomTabControl.SelectedItem as CustomTabItem).OwnWindow.WindowChildElement);
                }
                return _resizeEnabled;
            }
            protected internal set
            {
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [resize enabled].
        /// </summary>
        private bool _resizeEnabled = true;

        /// <summary>
        /// Gets a value indicating whether this instance can resize.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can resize; otherwise, <c>false</c>.
        /// </value>
        protected internal bool CanResize
        {
            get
            {
                return ResizeEnabled && resizeAnchor != ResizeAnchor.None;
            }
        }

        /// <summary>
        /// Defines the resize events.
        /// </summary>
        private void DefineResizeEvents()
        {
            if (window != null)
            {
                window.MouseEnter += new MouseEventHandler(window_MouseEnter);
                window.MouseLeave += new MouseEventHandler(window_MouseLeave);
                window.MouseLeftButtonDown += new MouseButtonEventHandler(window_MouseLeftButtonDown);
                window.MouseMove += new MouseEventHandler(window_MouseMove);
                window.MouseLeftButtonUp += new MouseButtonEventHandler(window_MouseLeftButtonUp);
            }
        }

        /// <summary>
        /// Handles the MouseLeave event of the window control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        void window_MouseLeave(object sender, MouseEventArgs e)
        {
            //this.Visibility = Visibility.Visible;
            if (this.DockingManager != null)
            {
                if (this.DockingManager.stackPanelLeave && !isResizing && optionsPopUp != null)
                {
                    if (!optionsPopUp.IsOpen)
                    {
                        if (this.DockingManager.ActiveWindow != this)
                        {
                            AutoHide = true;
                            this.CaptionForeGround = this.DockingManager.CaptionForeGround;
                            this.DockingManager.timer.Start();
                            if (this.maximizeButton != null)
                                VisualStateManager.GoToState(this.maximizeButton, "InActive", false);
                            if (this.closeButton != null)
                                VisualStateManager.GoToState(this.closeButton, "InActive", false);
                            if (this.dockToggle != null)
                                VisualStateManager.GoToState(this.dockToggle, "InActive", false);
                            if (this.optionsButton != null)
                                VisualStateManager.GoToState(this.optionsButton, "InActive", false);
                        }
                        else
                        {
                            this.ActiveForeground = this.DockingManager.ActiveForeground;
                            if (this.maximizeButton != null)
                                VisualStateManager.GoToState(this.maximizeButton, "Active", false);
                            if (this.closeButton != null)
                                VisualStateManager.GoToState(this.closeButton, "Active", false);
                            if (this.dockToggle != null)
                                VisualStateManager.GoToState(this.dockToggle, "Active", false);
                            if (this.optionsButton != null)
                                VisualStateManager.GoToState(this.optionsButton, "Active", false);
                            if (this.DockState == DockState.Float)
                            {
                                this.HeaderBackgroud = this.DockingManager.FloatWindowActiveHeaderBackground;
                            }
                            else
                            {
                                this.HeaderBackgroud = this.DockingManager.ActiveWindowColor;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the auto hide window.
        /// </summary>
        /// <value>The auto hide window.</value>
        protected internal Window AutoHideWindow
        {
            get;
            set;
        }

        /// <summary>
        /// Represents the Auto Hide.
        /// </summary>
        protected internal bool AutoHide = false;

        /// <summary>
        /// Handles the MouseEnter event of the window control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        void window_MouseEnter(object sender, MouseEventArgs e)
        {
            if (this.DockingManager != null)
            {
                if (this.DockingManager.stackPanelLeave)
                {
                    AutoHide = false;
                    this.DockingManager.timer.Start();
                }
            }
        }

        /// <summary>
        /// Represents the temp point.
        /// </summary>
        Point temppoint = new Point();

        /// <summary>
        /// Called before the <see cref="E:System.Windows.UIElement.MouseLeftButtonDown"/> event occurs.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
        }

        /// <summary>
        /// Handles the MouseLeftButtonDown event of the window control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        void window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            if (this.DockingManager != null)
            {
                if (this._Caption != string.Empty && this.CustomTabControl != null && this.CustomTabControl.SelectedItem != null)
                {
                    this.DockingManager.ActiveWindow = (this.CustomTabControl.SelectedItem as CustomTabItem).OwnWindow;
                }
                else
                {
                    if (this.DockingManager.ActiveWindow != null)
                    {
                        if (this.DockingManager.ActiveWindow.DockState == DockState.AutoHidden)
                        {
                            this.DockingManager.ActiveWindow.AutoHide = true;
                            this.DockingManager.timer.Start();
                        }
                    }
                    this.DockingManager.RemoveAutoHideAnimation(this);
                }

                isResizing = false;

                if (this.DockState == DockState.Hidden || this.DockState == DockState.Float)
                {
                    Canvas.SetZIndex(this, ++currentZIndex);
                    //if(tempGrid.Visibility == Visibility.Visible)
                    //tempGrid.Visibility = Visibility.Collapsed;
                }
                else if (((Canvas)DockingManager).Children.Contains(this) && this.DockState != DockState.AutoHidden)
                {
                    Canvas.SetZIndex(this, ++currentZIndex);
                    //if (tempGrid.Visibility == Visibility.Visible)
                    //    tempGrid.Visibility = Visibility.Collapsed;
                }
                if (CanResize)
                {
                    try
                    {
                        ((FrameworkElement)sender).CaptureMouse();
                        Canvas.SetZIndex((UIElement)sender, 0);
                        this.initialResizePoint = e.GetPosition(this.Parent as UIElement);
                        //if ((this.Parent.GetType() == typeof(WindowContainer)) && (this.Parent as WindowContainer)._window.captionBar.Visibility == Visibility.Collapsed && !(this.Parent as WindowContainer)._window.IsremovedFromParent)
                        //{
                        //    temppoint = e.GetPosition(this.Parent as UIElement);
                        //    if ((temppoint.X <= 3 && temppoint.X > 0) || (temppoint.Y <= 3 && temppoint.Y > 0) || (temppoint.X >= (this.Parent as WindowContainer)._window.Width - 3 && temppoint.X <= (this.Parent as WindowContainer)._window.Width) || (temppoint.Y >= (this.Parent as WindowContainer)._window.Height - 3 && temppoint.Y <= (this.Parent as WindowContainer)._window.Height))
                        //    {
                        //        this.initialResizePoint = e.GetPosition(null);
                        //    }
                        //}

                        System.Diagnostics.Debug.WriteLine("Down X: = " + initialResizePoint.X + "Down Y: = " + initialResizePoint.Y);
                        initialWindowSize.Width = !double.IsNaN(this.Width) ? this.Width : this.ActualWidth;
                        initialWindowSize.Height = !double.IsNaN(this.Height) ? this.Height : this.ActualHeight;
                        Point pos = e.GetPosition(window);
                        this.initialWindowLocation.X = Canvas.GetLeft(this);
                        this.initialWindowLocation.Y = Canvas.GetTop(this);
                        if (((Canvas)DockingManager).Children.Contains(this))
                        {
                            this.isResizing = true;
                        }
                    }
                    catch
                    {
                    }
                }
            }

            UpdateZindex();
            //if (this._Caption == string.Empty)
            //{
            //    tempGrid.Visibility = Visibility.Collapsed;
            //    if (this.DockingManager.ActiveWindow != null)
            //    {
            //        this.DockingManager.ActiveWindow.tempGrid.Visibility = Visibility.Collapsed;
            //    }
            //}
            //else if (this.DockManager != null)
            //{
            //    if (this.DockManager.Parent is WindowContainer)
            //    {
            //        tempGrid.Visibility = Visibility.Collapsed;
            //    }
            //}
        }

        /// <summary>
        /// Updates the zindex.
        /// </summary>
        protected internal void UpdateZindex()
        {
            //if (this._Caption != string.Empty)
            //{
            //    for (int i = 1; i <= this.DockingManager.WindowCollection.Count; i++)
            //    {
            //        if (this.DockingManager.WindowCollection[i] != this)
            //        {
            //            if (this.DockingManager.WindowCollection[i].tempGrid != null)
            //            {
            //                this.DockingManager.WindowCollection[i].tempGrid.Visibility = Visibility.Visible;
            //            }
            //        }
            //        else if (this.DockingManager.WindowCollection[i].tempGrid != null)
            //        {
            //            this.DockingManager.WindowCollection[i].tempGrid.Visibility = Visibility.Collapsed;
            //        }
            //    }
            //    foreach (UIElement el in ((Canvas)DockingManager).Children)
            //    {
            //        if (el.GetType() == typeof(Window))
            //        {
            //            if (((Window)el)._Caption == string.Empty)
            //            {
            //                if (((Window)el).tempGrid != null)
            //                {
            //                   // ((Window)el).tempGrid.Visibility = Visibility.Visible;
            //                }
            //            }
            //        }
            //    }
            //foreach (UIElement el in ((Canvas)DockingManager).Children)
            //{
            //    if (el.GetType() == typeof(Window))
            //    {
            //        if (((Window)el) != this)
            //        {
            //            if (((Window)el).tempGrid != null)
            //            {
            //                ((Window)el).tempGrid.Visibility = Visibility.Visible;
            //            }
            //            if (((Window)el)._Caption == string.Empty)
            //            {
            //                for (int i = 0; i < ((Window)el).WindowCollection.Count; i++)
            //                {
            //                    if (((Window)el).WindowCollection[i].tempGrid != null)
            //                    {
            //                        if (!(((Window)el).WindowCollection[i]._Caption == string.Empty))
            //                        {
            //                            ((Window)el).WindowCollection[i].tempGrid.Visibility = Visibility.Visible;
            //                        }
            //                    }
            //                }
            //                if (((Window)el).tempGrid != null)
            //                {
            //                    ((Window)el).tempGrid.Visibility = Visibility.Collapsed;
            //                }
            //            }
            //        }
            //    }

            //}
            //if (this._Caption == string.Empty && this.DockManager != null)
            //{
            //    UpdateContainerZindex(this);
            //}
            //else if (this._Caption == string.Empty)
            //{
            //    for (int i = 0; i < this.WindowCollection.Count; i++)
            //    {
            //        if (this.WindowCollection[i].tempGrid != null)
            //        {
            //            if (!(this.WindowCollection[i]._Caption == string.Empty))
            //            {
            //                this.WindowCollection[i].tempGrid.Visibility = Visibility.Visible;
            //            }
            //        }
            //    }
            //    if (this.tempGrid != null)
            //    {
            //        this.tempGrid.Visibility = Visibility.Collapsed;
            //    }
            //}
            //    if (((Canvas)DockingManager).Children.Contains(this) && this.DockState != DockState.AutoHidden)
            //    {
            //        Canvas.SetZIndex(this, ++Window.currentZIndex);
            //    }
            //    else
            //    {
            //        if (this.DockManager.Parent is WindowContainer)
            //        {
            //            Canvas.SetZIndex((this.DockManager.Parent as WindowContainer)._window, ++Window.currentZIndex);
            //            if (this.tempGrid != null)
            //            {
            //              //  this.tempGrid.Visibility = Visibility.Collapsed;
            //            }
            //            if ((this.DockManager.Parent as WindowContainer)._window.tempGrid != null)
            //            {
            //               // (this.DockManager.Parent as WindowContainer)._window.tempGrid.Visibility = Visibility.Collapsed;
            //            }
            //        }
            //    }
            //}
        }

        /// <summary>
        /// Updates the container zindex.
        /// </summary>
        /// <param name="w">The w.</param>
        protected internal void UpdateContainerZindex(Window w)
        {
            //try
            //{
            //    bool zindexApplied = false;
            //    if (w.DockManager != null && !((Canvas)DockingManager).Children.Contains(w))
            //    {
            //        if ((w.DockManager.Parent is WindowContainer))
            //        {
            //            for (int i = 0; i < ((w.DockManager.Parent as WindowContainer)._window).WindowCollection.Count; i++)
            //            {
            //                if (((w.DockManager.Parent as WindowContainer)._window).WindowCollection[i].tempGrid != null)
            //                {
            //                    ((w.DockManager.Parent as WindowContainer)._window).WindowCollection[i].tempGrid.Visibility = Visibility.Visible;
            //                    if (((w.DockManager.Parent as WindowContainer)._window).WindowCollection[i]._Caption == string.Empty)
            //                    {
            //                        ((w.DockManager.Parent as WindowContainer)._window).WindowCollection[i].tempGrid.Visibility = Visibility.Collapsed;
            //                    }
            //                }
            //            }
            //                UpdateContainerZindex((w.DockManager.Parent as WindowContainer)._window);
            //        }
            //    }
            //    else
            //    {
            //        if (!zindexApplied)
            //        {
            //            if (w.tempGrid != null)
            //            {
            //                if (w._Caption == string.Empty)
            //                {
            //                    w.tempGrid.Visibility = Visibility.Collapsed;
            //                }
            //                //this.tempGrid.Visibility = Visibility.Visible;
            //            }
            //        }
            //    }
            //}
            //catch
            //{
            //}
        }

        /// <summary>
        /// Handles the MouseLeftButtonUp event of the window control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        void window_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (ResizeEnabled && isResizing)
            {
                if (!(bool)this.dockToggle.IsChecked)
                {
                    UpDateFloatWindowSize();
                }
                else if ((bool)this.dockToggle.IsChecked)
                {
                    this.AnimationHeight = this.Height;
                    this.AnimationWidth = this.Width;
                }

                ((FrameworkElement)sender).ReleaseMouseCapture();
                isResizing = false;
            }

            isResizing = false;
            if (this.DockingManager != null)
            {
                this.DockingManager.popupLoadedInsideWindow = false;
            }

            temppoint = new Point();
        }

        /// <summary>
        /// Handles the MouseMove event of the window control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        void window_MouseMove(object sender, MouseEventArgs e)
        {
            bool allowResize = true;
            if (DockingManager != null)
            {
                if (((Canvas)DockingManager).Children.Contains(this))
                {
                    ResizeEnabled = true;
                    allowResize = true;
                }
                else
                {
                    ResizeEnabled = false;
                    allowResize = false;
                    window.Cursor = Cursors.Arrow;
                }
            }
            else
            {
                ResizeEnabled = false;
                allowResize = false;
            }

            if (ResizeEnabled && allowResize)
            {
                if (!isResizing)
                {
                    Point pos = new Point();
                    try
                    {
                        pos = e.GetPosition(window);
                    }
                    catch
                    {
                    }

                    if ((pos.Y <= HotSpotWidth) && (pos.X <= HotSpotWidth))
                    {
                        if (!this.isDragging)
                        {
                            window.Cursor = Cursors.SizeNWSE;
                            resizeAnchor = ResizeAnchor.TopLeft;
                        }
                    }
                    else if ((pos.Y <= HotSpotWidth) && (pos.X >= (window.ActualWidth - HotSpotWidth)))
                    {
                        if (!this.isDragging)
                        {
                            window.Cursor = Cursors.SizeNESW;
                            resizeAnchor = ResizeAnchor.TopRight;
                        }
                    }
                    else if (pos.Y <= HotSpotWidth)
                    {
                        if ((bool)this.dockToggle.IsChecked && this.DockPosition != Dock.Top)
                        {
                            if (!this.isDragging)
                            {
                                if (!((this.DockPosition == Dock.Left || this.DockPosition == Dock.Right) && this.DockState == DockState.AutoHidden))
                                {
                                    window.Cursor = Cursors.SizeNS;
                                    resizeAnchor = ResizeAnchor.Top;
                                }
                            }
                        }
                        else if (!(bool)this.dockToggle.IsChecked)
                        {
                            if (!this.isDragging)
                            {
                                if (!((this.DockPosition == Dock.Left || this.DockPosition == Dock.Right) && this.DockState == DockState.AutoHidden))
                                {
                                    window.Cursor = Cursors.SizeNS;
                                    resizeAnchor = ResizeAnchor.Top;
                                }
                            }
                        }
                    }
                    else if ((pos.Y >= (window.ActualHeight - HotSpotWidth)) && (pos.X <= HotSpotWidth))
                    {
                        if (!this.isDragging)
                        {
                            window.Cursor = Cursors.SizeNESW;
                            resizeAnchor = ResizeAnchor.BottomLeft;
                        }
                    }
                    else if ((pos.Y >= (window.ActualHeight - HotSpotWidth)) && (pos.X >= (window.ActualWidth - HotSpotWidth)))
                    {
                        if (!this.isDragging)
                        {
                            window.Cursor = Cursors.SizeNWSE;
                            resizeAnchor = ResizeAnchor.BottomRight;
                        }
                    }
                    else if (pos.Y >= (window.ActualHeight - HotSpotWidth))
                    {
                        if ((bool)this.dockToggle.IsChecked && this.DockPosition != Dock.Bottom)
                        {
                            if (!this.isDragging)
                            {
                                if (!((this.DockPosition == Dock.Right || this.DockPosition == Dock.Left) && this.DockState == DockState.AutoHidden))
                                {
                                    window.Cursor = Cursors.SizeNS;
                                    resizeAnchor = ResizeAnchor.Bottom;
                                }
                            }
                        }
                        else if (!(bool)this.dockToggle.IsChecked)
                        {
                            if (!this.isDragging)
                            {
                                if (!((this.DockPosition == Dock.Right || this.DockPosition == Dock.Left) && this.DockState == DockState.AutoHidden))
                                {
                                    window.Cursor = Cursors.SizeNS;
                                    resizeAnchor = ResizeAnchor.Bottom;
                                }
                            }
                        }
                    }
                    else if (pos.X <= HotSpotWidth)
                    {
                        if (((bool)this.dockToggle.IsChecked) && this.DockPosition != Dock.Left)
                        {
                            if (!this.isDragging)
                            {
                               if (!((this.DockPosition == Dock.Top || this.DockPosition == Dock.Bottom) && this.DockState == DockState.AutoHidden))
                                {
                                    window.Cursor = Cursors.SizeWE;
                                    resizeAnchor = ResizeAnchor.Left;
                                }
                            }
                        }
                        else if (!(bool)this.dockToggle.IsChecked)
                        {
                            if (!this.isDragging)
                            {
                                if (!((this.DockPosition == Dock.Top || this.DockPosition == Dock.Bottom) && this.DockState == DockState.AutoHidden))
                                {
                                    window.Cursor = Cursors.SizeWE;
                                    resizeAnchor = ResizeAnchor.Left;
                                }
                            }
                        }
                    }
                    else if (pos.X >= (window.ActualWidth - HotSpotWidth))
                    {
                        if ((bool)this.dockToggle.IsChecked && this.DockPosition != Dock.Right)
                        {
                            if (!this.isDragging)
                            {
                               if (!((this.DockPosition == Dock.Bottom || this.DockPosition == Dock.Top) && this.DockState == DockState.AutoHidden))
                                {
                                    window.Cursor = Cursors.SizeWE;
                                    resizeAnchor = ResizeAnchor.Right;
                                }
                            }
                        }
                        else if (!(bool)this.dockToggle.IsChecked)
                        {
                            if (!this.isDragging)
                            {
                                if (!((this.DockPosition == Dock.Bottom || this.DockPosition == Dock.Top) && this.DockState == DockState.AutoHidden))
                                {
                                    window.Cursor = Cursors.SizeWE;
                                    resizeAnchor = ResizeAnchor.Right;
                                }
                            }
                        }
                    }
                    else
                    {
                        window.Cursor = null;
                        resizeAnchor = ResizeAnchor.None;
                    }
                }
                else
                {
                    Point position = e.GetPosition(this.Parent as UIElement);
                    if ((this.Parent.GetType() == typeof(WindowContainer)) && (this.Parent as WindowContainer)._window.captionBar.Visibility == Visibility.Collapsed && !(this.Parent as WindowContainer)._window.IsremovedFromParent)
                    {
                        if ((temppoint.X <= 3 && temppoint.X > 0 && initialResizePoint.X != 0 && initialResizePoint.X >= 3) || (temppoint.Y <= 3 && temppoint.Y > 0 && initialResizePoint.Y != 0 && initialResizePoint.Y >= 3) || (temppoint.X >= (this.Parent as WindowContainer)._window.Width - 3 && temppoint.X <= (this.Parent as WindowContainer)._window.Width) || (temppoint.Y >= (this.Parent as WindowContainer)._window.Height - 3 && temppoint.Y <= (this.Parent as WindowContainer)._window.Height))
                        {
                            position = e.GetPosition(Application.Current.RootVisual);
                        }
                    }

                    System.Diagnostics.Debug.WriteLine("Move X: = " + position.X + "Move Y: = " + position.Y);
                    double deltaX;
                    double deltaY;
                    Point p = e.GetPosition(Application.Current.RootVisual);
                    if (this.DockingManager != null)
                    {
                        if (!((p.X >= 5 && p.X <= ClientWidth - 5) && (p.Y > 5 && p.Y <= ClientHeight - 5)))
                        {
                            Point parent_Position = e.GetPosition(this.DockingManager);
                            if (p.X <= 5)
                            {
                                position.X = 1;
                            }
                            if (p.X > ClientWidth - 5)
                            {
                                position.X = ClientWidth - 5;
                            }
                            if (p.Y <= 5)
                            {
                                position.Y = parent_Position.Y - p.Y;
                            }
                            if (p.Y > ClientHeight - 5)
                            {
                                position.Y = this.DockingManager.ActualHeight - 10;
                            }
                        }
                    }
                    if (position.X <= this.DockingManager.ActualWidth && position.Y <= this.DockingManager.ActualHeight)
                    {
                        deltaX = position.X - initialResizePoint.X;
                        deltaY = position.Y - initialResizePoint.Y;
                    }
                    else
                    {
                        deltaX = this.DockingManager.ActualWidth - initialResizePoint.X;
                        deltaY = this.DockingManager.ActualHeight - initialResizePoint.Y;
                    }

                    switch (resizeAnchor)
                    {
                        case ResizeAnchor.Left:
                            if (this.DockState == DockState.Hidden || this.DockState == DockState.Float || this.DockState == DockState.AutoHidden)
                            {
                                ResizeLeft(deltaX);
                            }
                            else if (((Canvas)DockingManager).Children.Contains(this))
                            {
                                ResizeLeft(deltaX);
                            }

                            break;

                        case ResizeAnchor.Top:
                            if (this.DockState == DockState.Hidden || this.DockState == DockState.Float || this.DockState == DockState.AutoHidden)
                            {
                                ResizeTop(deltaY);
                            }
                            else if (((Canvas)DockingManager).Children.Contains(this))
                            {
                                ResizeTop(deltaY);
                            }

                            break;

                        case ResizeAnchor.Right:
                            if (this.DockState == DockState.Hidden || this.DockState == DockState.Float || this.DockState == DockState.AutoHidden)
                            {
                                ResizeRight(deltaX);
                            }
                            else if (((Canvas)DockingManager).Children.Contains(this))
                            {
                                ResizeRight(deltaX);
                            }

                            break;

                        case ResizeAnchor.Bottom:
                            if (this.DockState == DockState.Hidden || this.DockState == DockState.Float || this.DockState == DockState.AutoHidden)
                            {
                                ResizeBottom(deltaY);
                            }
                            else if (((Canvas)DockingManager).Children.Contains(this))
                            {
                                ResizeBottom(deltaY);
                            }

                            break;

                        case ResizeAnchor.TopLeft:
                            if (this.IsremovedFromParent || this.DockState == DockState.Float)
                            {
                                ResizeLeft(deltaX);
                                ResizeTop(deltaY);
                            }
                            else if (((Canvas)DockingManager).Children.Contains(this))
                            {
                                ResizeLeft(deltaX);
                                ResizeTop(deltaY);
                            }

                            break;

                        case ResizeAnchor.TopRight:
                            if (this.IsremovedFromParent || this.DockState == DockState.Float)
                            {
                                ResizeRight(deltaX);
                                ResizeTop(deltaY);
                            }
                            else if (((Canvas)DockingManager).Children.Contains(this))
                            {
                                ResizeRight(deltaX);
                                ResizeTop(deltaY);
                            }
                            break;

                        case ResizeAnchor.BottomLeft:
                            if (this.IsremovedFromParent || this.DockState == DockState.Float)
                            {
                                ResizeLeft(deltaX);
                                ResizeBottom(deltaY);
                            }
                            else if (((Canvas)DockingManager).Children.Contains(this))
                            {
                                ResizeLeft(deltaX);
                                ResizeBottom(deltaY);
                            }
                            break;

                        case ResizeAnchor.BottomRight:
                            if (this.IsremovedFromParent || this.DockState == DockState.Float)
                            {
                                ResizeRight(deltaX);
                                ResizeBottom(deltaY);
                            }
                            else if (((Canvas)DockingManager).Children.Contains(this))
                            {
                                ResizeRight(deltaX);
                                ResizeBottom(deltaY);
                            }
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Raises the moved event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        protected internal void RaiseMovedEvent(MouseEventArgs e)
        {
            if (this.DragMoved != null && this.CanDock)
            {
                this.DragMoved(this, new DragEventArgs(0, 0, e, string.Empty));
            }
        }

        /// <summary>
        /// Resizes the bottom.
        /// </summary>
        /// <param name="deltaY">The delta Y.</param>
        private void ResizeBottom(double deltaY)
        {
            if (((Canvas)DockingManager).Children.Contains(this))
            {
                if (this._Caption == string.Empty)
                {
                    this.Height = Math.Max(initialWindowSize.Height + deltaY, captionBar.ActualHeight);
                    this.WindowContainer.Width = Double.NaN;
                    this.WindowContainer.Height = Double.NaN;
                    (this.WindowContainer.Children[0] as DockManager).Width = Double.NaN;
                    (this.WindowContainer.Children[0] as DockManager).Height = Double.NaN;
                    //((this.WindowContainer.Children[0] as DockManager).Children[0] as DockingGrid).ClearRowColumnMAXWidthHeight(((this.WindowContainer.Children[0] as DockManager).Children[0] as DockingGrid).gridDocking);
                }
                else
                {
                    this.Height = Math.Max(initialWindowSize.Height + deltaY, captionBar.ActualHeight);
                }
            }
        }

        /// <summary>
        /// Resizes the right.
        /// </summary>
        /// <param name="deltaX">The delta X.</param>
        private void ResizeRight(double deltaX)
        {
            if (((Canvas)DockingManager).Children.Contains(this))
            {
                if (this._Caption == string.Empty)
                {
                    this.Width = Math.Max(initialWindowSize.Width + deltaX, MinWindowWidth);
                }
                else
                {
                    this.Width = Math.Max(initialWindowSize.Width + deltaX, MinWindowWidth);
                }
            }
        }

        /// <summary>
        /// Resizes the top.
        /// </summary>
        /// <param name="deltaY">The delta Y.</param>
        private void ResizeTop(double deltaY)
        {
            if (((Canvas)DockingManager).Children.Contains(this))
            {
                double maxY = initialWindowLocation.Y + this.initialWindowSize.Height - captionBar.ActualHeight;
                Canvas.SetTop(this, Math.Min(initialResizePoint.Y + deltaY, maxY));
                if (this._Caption == string.Empty)
                {
                    this.Height = Math.Max(initialWindowSize.Height - deltaY, captionBar.ActualHeight);
                }
                else
                {
                    this.Height = Math.Max(initialWindowSize.Height - deltaY, captionBar.ActualHeight);
                }
            }
        }

        /// <summary>
        /// Resizes the left.
        /// </summary>
        /// <param name="deltaX">The delta X.</param>
        private void ResizeLeft(double deltaX)
        {
            if (((Canvas)DockingManager).Children.Contains(this))
            {
                double maxX = initialWindowLocation.X + this.initialWindowSize.Width - MinWindowWidth;
                Canvas.SetLeft(this, Math.Min(initialResizePoint.X + deltaX, maxX));
                if (this._Caption == string.Empty)
                {
                    this.Width = Math.Max(initialWindowSize.Width - deltaX, MinWindowWidth);
                }
                else
                {
                    this.Width = Math.Max(initialWindowSize.Width - deltaX, MinWindowWidth);
                }
            }
        }

        /// <summary>
        /// we set the width of the content presenter to avoid unwanted resize of the window
        /// due to exapnsion of the surfaces of the containers cause they are resized proportionally with the content
        /// if no dimensions are setted for them
        /// </summary>
        private void SetContentPresenterSizeAndMaxSize()
        {
            try
            {
                contentpresenter.Width = this.ActualWidth - innerContentPresenterOffset;
                contentpresenter.MinWidth = (contentpresenter as FrameworkElement).MinWidth;
                contentpresenter.MinHeight = (contentpresenter as FrameworkElement).MinHeight;
            }
            catch
            {
            }
        }

        /// <summary>
        /// Sets the size of the content presenter size and min.
        /// </summary>
        private void SetContentPresenterSizeAndMinSize()
        {
            try
            {
                contentpresenter.Width = this.ActualWidth;
                contentpresenter.MinWidth = (contentpresenter as FrameworkElement).MinWidth;
                contentpresenter.MinHeight = (contentpresenter as FrameworkElement).MinHeight;
            }
            catch
            {
            }
        }

        #endregion
    }

    /// <summary>
    /// Represents the State of the windows.
    /// </summary>
    public enum StateMaintanance
    {
        /// <summary>
        /// Represent the Dock State
        /// </summary>
        Dock,
        /// <summary>
        /// Represent the Float State
        /// </summary>
        Float,
        /// <summary>
        /// Represent the WindowContainer State
        /// </summary>
        WindowContainer,
        /// <summary>
        /// Represent the WindowContainerToDock State
        /// </summary>
        WindowContainerToDock,
        /// <summary>
        /// Represent the DockToWindowContainer State
        /// </summary>
        DockToWindowContainer,
        /// <summary>
        /// Represent the TabWithContainer State
        /// </summary>
        TabWithContainer,
        /// <summary>
        /// Represent the TabWithFloat State
        /// </summary>
        TabWithFloat,
        /// <summary>
        /// Represent the TabWithDock State
        /// </summary>
        TabWithDock
    }

    /// <summary>
    /// 
    /// </summary>
    public class WindowComparer:IComparer<int>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int Compare(int x, int y)
        {
            if (y > x)
                return 1;
            if (y < x)
                return -1;
            return 0;
        }
    }
}
