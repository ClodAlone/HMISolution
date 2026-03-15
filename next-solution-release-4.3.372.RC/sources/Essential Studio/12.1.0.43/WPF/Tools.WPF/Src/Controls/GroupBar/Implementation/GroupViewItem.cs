// <copyright file="GroupViewItem.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

#region file using
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Controls.Primitives;
using System.ComponentModel;
using System.Windows.Media.Animation;
using System.Windows.Documents;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Windows.Media.Imaging;
using System.Timers;
using System.Windows.Threading;
using Syncfusion.Licensing;
using Syncfusion.Windows.Shared;
#endregion

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents the GroupViewItem UI element.
    /// Class instances are hosted in the <see cref="Syncfusion.Windows.Tools.Controls.GroupView"/>.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif

    [SkinType(SkinVisualStyle = Skin.Office2007Blue,
Type = typeof(GroupViewItem), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/GroupBar/Themes/Office2007BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Black,
    Type = typeof(GroupViewItem), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/GroupBar/Themes/Office2007BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Silver,
    Type = typeof(GroupViewItem), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/GroupBar/Themes/Office2007SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Blue,
Type = typeof(GroupViewItem), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/GroupBar/Themes/Office2010BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Black,
    Type = typeof(GroupViewItem), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/GroupBar/Themes/Office2010BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Silver,
    Type = typeof(GroupViewItem), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/GroupBar/Themes/Office2010SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2003,
     Type = typeof(GroupViewItem), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/GroupBar/Themes/Office2003Style.xaml")]
    [SkinType(SkinVisualStyle = Skin.ShinyBlue,
     Type = typeof(GroupViewItem), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/GroupBar/Themes/ShinyBlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.ShinyRed,
     Type = typeof(GroupViewItem), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/GroupBar/Themes/ShinyRedStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.SyncOrange,
     Type = typeof(GroupViewItem), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/GroupBar/Themes/SyncOrangeStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Blend,
    Type = typeof(GroupViewItem), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/GroupBar/Themes/BlendStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Default,
    Type = typeof(GroupViewItem), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/GroupBar/Themes/DefaultStyle.xaml")]  
    [SkinType(SkinVisualStyle = Skin.VS2010,
     Type = typeof(GroupViewItem), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/GroupBar/Themes/VS2010Style.xaml")]
    [SkinType(SkinVisualStyle = Skin.Metro,
     Type = typeof(GroupViewItem), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/GroupBar/Themes/MetroStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Transparent ,
     Type = typeof(GroupViewItem), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/GroupBar/Themes/TransparentStyle.xaml")]

    public class GroupViewItem : TextImageControl
    {
        #region Constants
        /// <summary>
        /// Name of the main host from template.
        /// </summary>
        private const string C_nameMainHost = "MainGrid";

        private Adorner lastDraggingAdorner = null;

        /// <summary>
        /// Message for the main host if it is not found.
        /// </summary>
        private const string C_errorMainHost = "MainGrid not found.";

        /// <summary>
        /// Name of the content host from template.
        /// </summary>
        private const string C_nameContentHost = "ContentHost";

        /// <summary>
        /// Message for the content host if it is not found.
        /// </summary>
        private const string C_errorContentHost = "ContentHost not found";

        /// <summary>
        /// Name of the text host from template.
        /// </summary>
        private const string C_nameTextHost = "TextHost";

        /// <summary>
        /// Message for the content host if it is not found.
        /// </summary>
        private const string C_errorTextHost = "TextHost not found";

        /// <summary>
        /// Name of the text editor from template.
        /// </summary>
        private const string C_nameTextEditor = "TextEditor";

        /// <summary>
        /// Message for scroll viewer if it is not found.
        /// </summary>
        private const string C_errorTextEditor = "TextEditor not found";

        /// <summary>
        /// Name of the image host from template.
        /// </summary>
        private const string C_nameImageHost = "ImageHost";

        /// <summary>
        /// Message for the content host if it is not found.
        /// </summary>
        private const string C_errorImageHost = "ImageHost not found";

        /// <summary>
        /// Delay for auto expand tab if dragging item over it.
        /// </summary>
        private const double C_expandDelay = 1000;

        /// <summary>
        /// Contains default visual style key name.
        /// </summary>
        private const string C_defaultVisStyleName = "Default";
        #endregion

        #region Private members
        /// <summary>
        /// Adorner is shown while dragging the control.
        /// </summary>
        private DragAdorner m_dragAdorner;

        /// <summary>
        /// Adorner is shown while dragging the current item.
        /// </summary>
        private DragMarkerAdorner m_dragMarkerAdorner;

        /// <summary>
        /// Point where control is pressed.
        /// </summary>
        private Point m_mouseDownPoint;

        /// <summary>
        /// Indicates whether space key is pressed.
        /// </summary>
        private bool m_isSpaceKeyDown = false;

        /// <summary>
        /// Visual tree element of the control that hosts the whole control.
        /// </summary>
        private FrameworkElement m_mainHost;

        /// <summary>
        /// Visual tree element of the control that hosts the displayed text.
        /// </summary>
        private FrameworkElement m_textHost;

        /// <summary>
        /// Visual tree element of the control that hosts the content.
        /// </summary>
        private FrameworkElement m_contentHost;

        /// <summary>
        /// Visual tree element of the control that hosts the displayed image.
        /// </summary>
        private FrameworkElement m_imageHost;

        /// <summary>
        /// Text editor for the text content of the GroupViewItem.
        /// </summary>
        private TextBox m_textEditor = null;

        /// <summary>
        /// Contains the last drag over <see cref="Syncfusion.Windows.Tools.Controls.GroupBarItem"/>.
        /// </summary>
        private GroupBarItem m_lastDragoverItem;

        /////// <summary>
        /////// Default background.
        /////// </summary>
        ////private Brush m_defaultBackground = null;

        /////// <summary>
        /////// Default BorderBrush.
        /////// </summary>
        ////private Brush m_defaultForeground = null;
        #endregion

        #region Dependency properties

        /////// <summary>
        /////// Identifies the <see cref="DefaultBackground"/> dependency property.
        /////// </summary>
        ////internal static readonly DependencyProperty DefaultBackgroundProperty = DependencyProperty.Register("DefaultBackground", typeof(Brush), typeof(GroupViewItem), new FrameworkPropertyMetadata(Brushes.Transparent));

        /////// <summary>
        /////// Identifies the <see cref="DefaultForeground"/> dependency property.
        /////// </summary>
        ////internal static readonly DependencyProperty DefaultForegroundProperty = DependencyProperty.Register("DefaultForeground", typeof(Brush), typeof(GroupViewItem), new FrameworkPropertyMetadata(Brushes.Transparent));

        /// <summary>
        /// Identifies <see cref="CustomAnimations"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CustomAnimationsProperty = DependencyProperty.Register("CustomAnimations", typeof(CustomAnimationsCollection), typeof(GroupViewItem), new UIPropertyMetadata(null));

        /// <summary>
        /// Identifies <see cref="IsSelected"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsSelectedProperty;

        /// <summary>
        /// Identifies <see cref="IsPressed"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsPressedProperty = DependencyProperty.Register("IsPressed", typeof(bool), typeof(GroupViewItem), new UIPropertyMetadata(false, OnIsPressedChanged));

        /// <summary>
        /// Identifies <see cref="MouseHoverColor"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MouseHoverColorProperty = DependencyProperty.Register("MouseHoverColor", typeof(Color), typeof(GroupViewItem), new FrameworkPropertyMetadata(Colors.White, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>
        /// Identifies <see cref="Syncfusion.Windows.Tools.Controls.GroupViewItem.SelectedItemColorProperty"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedItemColorProperty = DependencyProperty.Register("SelectedItemColor", typeof(Color), typeof(GroupViewItem), new UIPropertyMetadata(Colors.White));

        /// <summary>
        /// Identifies <see cref="SelectedItemMouseHoverColor"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedItemMouseHoverColorProperty = DependencyProperty.Register("SelectedItemMouseHoverColor", typeof(Color), typeof(GroupViewItem), new UIPropertyMetadata(Colors.White));

        /// <summary>
        /// Identifies <see cref="MouseDownColor"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MouseDownColorProperty = DependencyProperty.Register("MouseDownColor", typeof(Color), typeof(GroupViewItem), new UIPropertyMetadata(Colors.White));

        /// <summary>
        /// Identifies <see cref="IsInEditMode"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsInEditModeProperty = DependencyProperty.Register("IsInEditMode", typeof(bool), typeof(GroupViewItem), new UIPropertyMetadata(false, OnIsInEditModeChanged));

        /// <summary>
        /// Identifies <see cref="IsDragOverTop"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsDragOverTopProperty = DependencyProperty.Register("IsDragOverTop", typeof(bool), typeof(GroupViewItem), new UIPropertyMetadata(false));

        /// <summary>
        /// Identifies <see cref="IsDragOverDown"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsDragOverDownProperty = DependencyProperty.Register("IsDragOverDown", typeof(bool), typeof(GroupViewItem), new UIPropertyMetadata(false));

        /// <summary>
        /// Identifies <see cref="IsDragging"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsDraggingProperty = DependencyProperty.Register("IsDragging", typeof(bool), typeof(GroupViewItem), new UIPropertyMetadata(false));
        #endregion

        #region Properties
        /////// <summary>
        /////// Gets or sets the default background.
        /////// </summary>
        /////// <value>The default background.</value>
        ////internal Brush DefaultBackground
        ////{
        ////    get
        ////    {
        ////        return (Brush)GetValue(DefaultBackgroundProperty);
        ////    }

        ////    set
        ////    {
        ////        SetValue(DefaultBackgroundProperty, value);
        ////    }
        ////}

        /////// <summary>
        /////// Gets or sets the default foreground.
        /////// </summary>
        /////// <value>The default foreground.</value>
        ////internal Brush DefaultForeground
        ////{
        ////    get
        ////    {
        ////        return (Brush)GetValue(DefaultForegroundProperty);
        ////    }

        ////    set
        ////    {
        ////        SetValue(DefaultForegroundProperty, value);
        ////    }
        ////}

        /// <summary>
        /// Gets or sets text to display.
        /// </summary>
        /// <value>Type: <see cref="string"/></value>
        /// <seealso cref="string"/>
        public override string Text
        {
            get
            {
                return base.Text;
            }

            set
            {
                OnBeforeEdit();
                base.Text = value;
                OnAfterEdit();
            }
        }

        /// <summary>
        /// Gets the logical parent.
        /// </summary>
        /// <value>The logical parent.</value>
        public GroupView LogicalParent
        {
            get
            {
                return ItemsControl.ItemsControlFromItemContainer(this) as GroupView;
            }
        }

        /// <summary>
        /// Gets the parent group bar.
        /// </summary>
        /// <value>The parent group bar.</value>
        public GroupBar ParentGroupBar
        {
            get
            {
                if (this.ParentTab != null)
                {
                    return ParentTab.LogicalParent;
                }

                else
                {
                    GroupBar parentGroupBar = VisualUtils.FindAncestor(this as Visual, typeof(GroupBar)) as GroupBar;
                    if (parentGroupBar != null)
                        return parentGroupBar;
                    return null;
                }
            }
        }

        /// <summary>
        /// Gets the parent tab.
        /// </summary>
        /// <value>The parent tab.</value>
        public GroupBarItem ParentTab
        {
            get
            {
                GroupBarItem tab = null;

                if (LogicalParent != null
                    && LogicalParent.LogicalParent != null)
                {
                    tab = LogicalParent.LogicalParent;
                }

                return tab;
            }
        }

        /// <summary>
        /// Gets or sets the custom animations.
        /// </summary>
        /// <value>The custom animations.</value>
        [TypeConverter(typeof(CustomAnimationsConverter))]
        public CustomAnimationsCollection CustomAnimations
        {
            get
            {
                return (CustomAnimationsCollection)GetValue(CustomAnimationsProperty);
            }

            set
            {
                SetValue(CustomAnimationsProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is selected.
        /// </summary>
        /// <value>
        /// true if this instance is selected; otherwise, false.
        /// </value>
        public bool IsSelected
        {
            get
            {
                return (bool)GetValue(GroupViewItem.IsSelectedProperty);
            }

            set
            {
                SetValue(GroupViewItem.IsSelectedProperty, value);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is pressed.
        /// </summary>
        /// <value>
        /// true if this instance is pressed; otherwise, false.
        /// </value>
        public bool IsPressed
        {
            get
            {
                return (bool)GetValue(IsPressedProperty);
            }

            private set
            {
                SetValue(IsPressedProperty, value);
            }
        }

        /// <summary>
        /// Gets a value indicating whether [show tool tip].
        /// </summary>
        /// <value>true if [show tool tip]; otherwise, false.</value>
        public bool ShowToolTip
        {
            get
            {
                return (bool)GroupView.GetShowToolTip(this);
            }
        }

        /// <summary>
        /// Gets or sets the color of the mouse hover.
        /// </summary>
        /// <value>The color of the mouse hover.</value>
        public Color MouseHoverColor
        {
            get
            {
                return (Color)GetValue(MouseHoverColorProperty);
            }

            set
            {
                SetValue(MouseHoverColorProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the color of the selected item.
        /// </summary>
        /// <value>The color of the selected item.</value>
        public Color SelectedItemColor
        {
            get
            {
                return (Color)GetValue(SelectedItemColorProperty);
            }

            set
            {
                SetValue(SelectedItemColorProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the color of the selected item mouse hover.
        /// </summary>
        /// <value>The color of the selected item mouse hover.</value>
        public Color SelectedItemMouseHoverColor
        {
            get
            {
                return (Color)GetValue(SelectedItemMouseHoverColorProperty);
            }

            set
            {
                SetValue(SelectedItemMouseHoverColorProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the color of the mouse down.
        /// </summary>
        /// <value>The color of the mouse down.</value>
        public Color MouseDownColor
        {
            get
            {
                return (Color)GetValue(MouseDownColorProperty);
            }

            set
            {
                SetValue(MouseDownColorProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is in edit mode.
        /// </summary>
        /// <value>
        /// true if this instance is in edit mode; otherwise, false.
        /// </value>
        public bool IsInEditMode
        {
            get
            {
                return (bool)GetValue(IsInEditModeProperty);
            }

            set
            {
                SetValue(IsInEditModeProperty, value);
            }
        }

        /// <summary>
        /// Gets the main host.
        /// </summary>
        /// <value>The main host.</value>
        internal FrameworkElement MainHost
        {
            get
            {
                return m_mainHost;
            }
        }

        /// <summary>
        /// Gets the content host.
        /// </summary>
        /// <value>The content host.</value>
        internal FrameworkElement ContentHost
        {
            get
            {
                return m_contentHost;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is dragging.
        /// </summary>
        /// <value>
        /// true if this instance is dragging; otherwise, false
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
        /// Gets the adorner drag.
        /// </summary>
        /// <value>The adorner drag.</value>
        private DragAdorner AdornerDrag
        {
            get
            {
                if (m_dragAdorner == null)
                {
                    m_dragAdorner = new DragAdorner(MainHost);
                }

                return m_dragAdorner;
            }
        }

        /// <summary>
        /// Gets the adorner marker.
        /// </summary>
        /// <value>The adorner marker.</value>
        private DragMarkerAdorner AdornerMarker
        {
            get
            {
                if (m_dragMarkerAdorner == null)
                {
                    m_dragMarkerAdorner = new DragMarkerAdorner(this);
                }

                return m_dragMarkerAdorner;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is space key down.
        /// </summary>
        /// <value>
        /// true if this instance is space key down; otherwise, false.
        /// </value>
        private bool IsSpaceKeyDown
        {
            get
            {
                return m_isSpaceKeyDown;
            }
        }

        /// <summary>
        /// Gets the text editor.
        /// </summary>
        /// <value>The text editor.</value>
        private TextBox TextEditor
        {
            get
            {
                return m_textEditor;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is drag over top.
        /// </summary>
        /// <value>
        /// true if this instance is drag over top; otherwise, false.
        /// </value>
        public bool IsDragOverTop
        {
            get
            {
                return (bool)GetValue(IsDragOverTopProperty);
            }

            set
            {
                SetValue(IsDragOverTopProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is drag over down.
        /// </summary>
        /// <value>
        /// true if this instance is drag over down; otherwise, false.
        /// </value>
        public bool IsDragOverDown
        {
            get
            {
                return (bool)GetValue(IsDragOverDownProperty);
            }

            set
            {
                SetValue(IsDragOverDownProperty, value);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is possible drop.
        /// </summary>
        /// <value>
        /// true if this instance is possible drop; otherwise, false.
        /// </value>
        private bool IsPossibleDrop
        {
            get
            {
                bool possible = false;

                if (this.IsDragging)
                {
                    GroupBar parentGroupBar = ParentGroupBar;
                    if (parentGroupBar == null)
                        parentGroupBar = VisualUtils.FindAncestor(this as Visual, typeof(GroupBar)) as GroupBar;
                    if (parentGroupBar != null && !parentGroupBar.IsCollapsed)
                    {
                        Point currentPosition = Mouse.GetPosition(parentGroupBar);
                        FrameworkElement overElement = parentGroupBar.InputHitTest(currentPosition) as FrameworkElement;

                        if (overElement != null)
                        {
                            GroupViewItem overContainer = overElement.TemplatedParent as GroupViewItem;
                            GroupView overView = GetGroupViewFromChildren(overElement);

                            if (overContainer != null)
                            {
                                overView = overContainer.LogicalParent;
                                object overItem = overView.ItemContainerGenerator.ItemFromContainer(overContainer);
                                object draggingItem = LogicalParent.ItemContainerGenerator.ItemFromContainer(this);

                                if (overItem != null && draggingItem != null)
                                {
                                    Type overItemType = overItem.GetType();
                                    Type draggingItemType = draggingItem.GetType();

                                    if (overItemType == draggingItemType)
                                    {
                                        possible = true;
                                    }
                                    else
                                        possible = true;
                                }
                                else
                                    possible = true;
                            }
                            else if (overView != null
                                && ((overView.ItemsSource != null && LogicalParent.ItemsSource != null
                                && overView.ItemsSource.GetType() == LogicalParent.ItemsSource.GetType())
                                || (overView.ItemsSource == null && LogicalParent.ItemsSource == null)))
                            {
                                possible = true;
                            }
                            else
                                possible = true;
                        }
                        else
                        {
                            possible = true;
                        }
                    }
                    else if (this.LogicalParent != null)
                    {
                        Point currentPosition = Mouse.GetPosition(this.LogicalParent);
                        FrameworkElement overElement = this.LogicalParent.InputHitTest(currentPosition) as FrameworkElement;
                        if (overElement != null)
                        {
                            possible = true;
                        }
                        else
                        {
                            UIElement element = Mouse.DirectlyOver as UIElement;

                            while (element != null)
                            {
                                element = VisualTreeHelper.GetParent(element) as UIElement;
                                GroupViewItem item = element as GroupViewItem;

                                if (item != null)
                                {
                                    if (item.ParentGroupBar != null)
                                    {
                                        if (item.ParentGroupBar.AllowDrop == true)
                                            possible = true;
                                    }
                                    else
                                    {
                                        GroupBar parentGroupbar = VisualUtils.FindAncestor(this as Visual, typeof(GroupBar)) as GroupBar;
                                        if (parentGroupbar != null)
                                        {
                                            if (parentGroupbar.AllowDrop)
                                                possible = true;

                                        }
                                    }

                                    break;
                                }

                                GroupView gview = element as GroupView;

                                if (gview != null)
                                {
                                    GroupBarItem gBarItem = gview.Parent as GroupBarItem;
                                    GroupBar gBar = null;
                                    if (gBarItem != null)
                                        gBar = gBarItem.Parent as GroupBar;
                                    if (gBar != null)
                                        if (gBar.AllowDrop == true)
                                            possible = true;
                                    break;
                                }

                                GroupBar gB = element as GroupBar;
                                if (gB != null)
                                {
                                    if (gB.AllowDrop == true)
                                        possible = true;
                                    break;
                                }
                            }
                        }
                    }
                }
                else
                {
                    possible = true;
                }

                return possible;
            }
        }
        #endregion

        #region Events
        /// <summary>
        /// Identifies <see cref="Click"/> routed event.
        /// </summary>
        public static readonly RoutedEvent ClickEvent = EventManager.RegisterRoutedEvent("Click", RoutingStrategy.Bubble, typeof(EventHandler), typeof(GroupViewItem));

        /// <summary>
        /// Bubbling routed event is fired when mouse click is performed on 
        /// the control.
        /// </summary>
        public event RoutedEventHandler Click
        {
            add
            {
                AddHandler(ClickEvent, value);
            }

            remove
            {
                RemoveHandler(ClickEvent, value);
            }
        }

        /// <summary>
        /// Identifies <see cref="DoubleClick"/> routed event.
        /// </summary>
        public static readonly RoutedEvent DoubleClickEvent = EventManager.RegisterRoutedEvent("DoubleClick", RoutingStrategy.Bubble, typeof(EventHandler), typeof(GroupViewItem));

        /// <summary>
        /// Bubbling routed event is fired when mouse double click is performed on
        /// the control.
        /// </summary>
        public event RoutedEventHandler DoubleClick
        {
            add
            {
                AddHandler(DoubleClickEvent, value);
            }

            remove
            {
                RemoveHandler(DoubleClickEvent, value);
            }
        }

        /// <summary>
        /// Identifies <see cref="Hover"/> routed event.
        /// </summary>
        public static readonly RoutedEvent HoverEvent = EventManager.RegisterRoutedEvent("Hover", RoutingStrategy.Bubble, typeof(EventHandler), typeof(GroupViewItem));

        /// <summary>
        /// Bubbling routed event is fired when mouse hovers over the control.
        /// </summary>
        public event RoutedEventHandler Hover
        {
            add
            {
                AddHandler(HoverEvent, value);
            }

            remove
            {
                RemoveHandler(HoverEvent, value);
            }
        }

        /// <summary>
        /// Identifies <see cref="Press"/> routed event.
        /// </summary>
        public static readonly RoutedEvent PressEvent = EventManager.RegisterRoutedEvent("Press", RoutingStrategy.Bubble, typeof(EventHandler), typeof(GroupViewItem));

        /// <summary>
        /// Bubbling routed event is fired when the control is pressed.
        /// </summary>
        public event RoutedEventHandler Press
        {
            add
            {
                AddHandler(PressEvent, value);
            }

            remove
            {
                RemoveHandler(PressEvent, value);
            }
        }

        /// <summary>
        /// Identifies <see cref="Selected"/> routed event.
        /// </summary>
        public static readonly RoutedEvent SelectedEvent = EventManager.RegisterRoutedEvent("Selected", RoutingStrategy.Bubble, typeof(EventHandler), typeof(GroupViewItem));

        /// <summary>
        /// Bubbling routed event is fired when the control is selected.
        /// </summary>
        public event RoutedEventHandler Selected
        {
            add
            {
                AddHandler(SelectedEvent, value);
            }

            remove
            {
                RemoveHandler(SelectedEvent, value);
            }
        }

        /// <summary>
        /// Identifies <see cref="Unselected"/> routed event.
        /// </summary>
        public static readonly RoutedEvent UnselectedEvent = EventManager.RegisterRoutedEvent("Unselected", RoutingStrategy.Bubble, typeof(EventHandler), typeof(GroupViewItem));

        /// <summary>
        /// Occurs when [unselected].
        /// </summary>
        public event RoutedEventHandler Unselected
        {
            add
            {
                AddHandler(UnselectedEvent, value);
            }

            remove
            {
                RemoveHandler(UnselectedEvent, value);
            }
        }

        /// <summary>
        /// Identifies <see cref="AfterEdit"/> routed event.
        /// </summary>
        public static readonly RoutedEvent AfterEditEvent = EventManager.RegisterRoutedEvent("AfterEdit", RoutingStrategy.Bubble, typeof(EventHandler), typeof(GroupViewItem));

        /// <summary>
        /// Bubbling routed event is fired after renaming the <see cref="Syncfusion.Windows.Tools.Controls.GroupBarItem"/>.
        /// </summary>
        public event RoutedEventHandler AfterEdit
        {
            add
            {
                AddHandler(AfterEditEvent, value);
            }

            remove
            {
                RemoveHandler(AfterEditEvent, value);
            }
        }

        /// <summary>
        /// Identifies <see cref="BeforeEdit"/> routed event.
        /// </summary>
        public static readonly RoutedEvent BeforeEditEvent = EventManager.RegisterRoutedEvent("BeforeEdit", RoutingStrategy.Bubble, typeof(EventHandler), typeof(GroupViewItem));

        /// <summary>
        /// Bubbling routed event is fired before renaming the <see cref="Syncfusion.Windows.Tools.Controls.GroupBarItem"/>.
        /// </summary>
        public event RoutedEventHandler BeforeEdit
        {
            add
            {
                AddHandler(BeforeEditEvent, value);
            }

            remove
            {
                RemoveHandler(BeforeEditEvent, value);
            }
        }

        /// <summary>
        /// Identifies <see cref="DragStart"/> routed event.
        /// </summary>
        public static readonly RoutedEvent DragStartEvent = EventManager.RegisterRoutedEvent("DragStart", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(GroupViewItem));

        /// <summary>
        /// Bubbling routed event is fired when the groupViewItem is dragged.
        /// </summary>
        public event RoutedEventHandler DragStart
        {
            add
            {
                AddHandler(DragStartEvent, value);
            }

            remove
            {
                RemoveHandler(DragStartEvent, value);
            }
        }

        /// <summary>
        /// Identifies <see cref="DragEnd"/> routed event.
        /// </summary>
        public static readonly RoutedEvent DragEndEvent = EventManager.RegisterRoutedEvent("DragEnd", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(GroupViewItem));

        /// <summary>
        /// Bubbling routed event is fired when the groupViewItem is dragged and dropped inside 
        /// the same or another <see cref="Syncfusion.Windows.Tools.Controls.GroupView"/>.
        /// </summary>
        public event RoutedEventHandler DragEnd
        {
            add
            {
                AddHandler(DragEndEvent, value);
            }

            remove
            {
                RemoveHandler(DragEndEvent, value);
            }
        }
        #endregion

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="GroupViewItem"/> class.
        /// </summary>
        public GroupViewItem()
            : base()
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                if (Application.Current != null && !BindingUtils.GetEnableBindingErrors(Application.Current.MainWindow))
                    System.Diagnostics.PresentationTraceSources.DataBindingSource.Switch.Level = System.Diagnostics.SourceLevels.Critical;
            }
            EventManager.RegisterClassHandler(typeof(UIElement), Keyboard.PreviewKeyDownEvent, (KeyEventHandler)OnPreviewKeyDown);
        }

        /// <summary>
        /// Initializes static members of the <see cref="GroupViewItem"/> class.
        /// </summary>
        static GroupViewItem()
        {
            EnvironmentTest.ValidateLicense(typeof(GroupViewItem));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GroupViewItem), new FrameworkPropertyMetadata(typeof(GroupViewItem)));

            GroupViewItem.IsSelectedProperty = Selector.IsSelectedProperty.AddOwner(typeof(GroupViewItem), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Journal | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsParentMeasure, OnIsSelectedChanged));

            GroupView.ShowToolTipProperty.AddOwner(typeof(GroupViewItem));
        }

        /// <summary>
        /// Initializes the control in the given template.
        /// </summary>
        /// <param name="inTemplate">Template to initialize control in.</param>
        private void Initialize(FrameworkTemplate inTemplate)
        {
            m_mainHost = inTemplate.FindName(C_nameMainHost, this) as FrameworkElement;
            if (m_mainHost == null)
            {
                throw new ApplicationException(C_errorMainHost);
            }

            m_contentHost = inTemplate.FindName(C_nameContentHost, this) as FrameworkElement;

            if (m_contentHost == null)
            {
                throw new ApplicationException(C_errorContentHost);
            }

            m_textHost = this.Template.FindName(C_nameTextHost, this) as FrameworkElement;

            if (m_textHost == null)
            {
                throw new ApplicationException(C_errorTextHost);
            }

            m_textEditor = this.Template.FindName(C_nameTextEditor, this) as TextBox;

            if (m_textEditor == null)
            {
                throw new ApplicationException(C_errorTextEditor);
            }

            TextEditor.IsVisibleChanged += new DependencyPropertyChangedEventHandler(TextEditor_IsVisibleChanged);
            TextEditor.LostKeyboardFocus += new KeyboardFocusChangedEventHandler(TextEditor_LostKeyboardFocus);

            m_imageHost = this.Template.FindName(C_nameImageHost, this) as FrameworkElement;

            if (m_imageHost == null)
            {
                throw new ApplicationException(C_errorImageHost);
            }

            if (CustomAnimations != null)
            {
                CustomAnimations.Initialize(this, Template);
            }

            if (ImageSource == null && ParentGroupBar != null && ParentGroupBar.DefaultItemImage != null)
            {
                ImageSource = ParentGroupBar.DefaultItemImage;
            }
        }

        /// <summary>
        /// Builds the current template's visual tree if necessary.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            Initialize(Template);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.FrameworkElement.Initialized"/> event. This method is invoked whenever <see cref="P:System.Windows.FrameworkElement.IsInitialized"/> is set to true internally.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.RoutedEventArgs"/> that contains the event data.</param>
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            Loaded += new RoutedEventHandler(OnLoaded);

            ////Shared.DictionaryList list1 = SkinStorage.GetVisualStylesList(this);
            ////if (list1 != null)
            ////{
            ////    Shared.DictionaryList list2 = list1["Default"] as Shared.DictionaryList;
            ////    if (list2 != null)
            ////    {
            ////        if (list2.ContainsKey("GroupViewItem_BackgroundBrush") &&
            ////            m_defaultBackground == null)
            ////        {
            ////            m_defaultBackground = list2["GroupViewItem_BackgroundBrush"] as Brush;
            ////            Background = m_defaultBackground;
            ////        }

            ////        if (list2.ContainsKey("ItemText_Brush") &&
            ////            m_defaultForeground == null)
            ////        {
            ////            m_defaultForeground = list2["ItemText_Brush"] as Brush;
            ////            Foreground = m_defaultForeground;
            ////        }
            ////    }
            ////}
        }

        /// <summary>
        /// Called when the control is loaded and ready for presentation.
        /// </summary>
        /// <param name="sender">Source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> that contains the event data.</param>
        protected virtual void OnLoaded(object sender, RoutedEventArgs e)
        {
            GroupBarItem barItem = null;
            GroupBar parentGroupBar = null;
            if (this.Parent != null && (this.Parent as GroupView).LogicalParent != null)
            {
                barItem = (this.Parent as GroupView).LogicalParent;
                parentGroupBar = barItem.LogicalParent;
            }

            if (CustomAnimations != null)
            {
                CustomAnimations.InitializeResources(ParentGroupBar.LogicalParent);
            }

            if (this.IsSelected)
            {

                if (IsMouseCaptured)
                {
                    IsPressed = false;
                }
                this.OnSelected();
                if (parentGroupBar != null)
                {
                    parentGroupBar.SelectedObject = this;
                    parentGroupBar.SelectedItem = this;
                }
            }
            else
            {
                this.OnUnselected();
                //if (parentGroupBar != null)
                //    parentGroupBar.SelectedObject = barItem;
            }

            if (ToolTip == null)
            {
                ToolTip = Text;
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Called when some dependencyProperty is changed.
        /// </summary>
        /// <param name="e">Provides data for various property changed events. Typically these events report effective value changes in the value of a read-only dependency property.</param>
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            ////if (e.Property == SkinStorage.VisualStyleProperty &&
            ////    (string)e.NewValue == C_defaultVisStyleName)
            ////{
            ////    ChangePropertiesToDefaultValues();
            ////}
            ////else
            ////{
            ////    UserUpdateDefaultSkin(e);
            ////}
        }

        /////// <summary>
        /////// Changes default skin properties values to values that user had been set.
        /////// </summary>
        /////// <param name="e">Provides data for various property changed events. Typically these events report effective value changes in the value of a read-only dependency property.</param>
        ////private void UserUpdateDefaultSkin(DependencyPropertyChangedEventArgs e)
        ////{
        ////    if (e.Property == BackgroundProperty)
        ////    {
        ////        m_defaultBackground = Background;
        ////    }
        ////    else if (e.Property == ForegroundProperty)
        ////    {
        ////        m_defaultForeground = Foreground;
        ////    }
        ////}

        /////// <summary>
        /////// Sets default values to the properties.
        /////// </summary>
        ////private void ChangePropertiesToDefaultValues()
        ////{
        ////    Background = m_defaultBackground;
        ////    Foreground = m_defaultForeground;
        ////}

        /// <summary>
        /// Invoked when the value of <see cref="IsPressed"/> property is changed.
        /// </summary>
        /// <param name="d">GroupViewItem object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private static void OnIsPressedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GroupViewItem item = d as GroupViewItem;

            if (item != null && item.IsLoaded && e != null)
            {
                bool isPressed = (bool)e.NewValue;

                if (isPressed)
                {
                    item.OnPress();
                }
            }
        }

        /// <summary>
        /// Invoked when the value of <see cref="IsSelected"/> property is changed.
        /// </summary>
        /// <param name="d">GroupViewItem object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GroupViewItem item = d as GroupViewItem;

            //GroupBarItem barItem = (item.Parent as GroupView).LogicalParent;
            //GroupBar parentGroupBar = barItem.LogicalParent;

            GroupBarItem barItem = null;
            GroupBar parentGroupBar = null;
            if (item.Parent as GroupView != null)
                barItem = (item.Parent as GroupView).LogicalParent;
            if (barItem != null)
                parentGroupBar = barItem.LogicalParent;
           
            if (item != null && item.IsLoaded && e != null)
            {
                bool isSelected = (bool)e.NewValue;

                if (isSelected)
                {
                    if (item.IsMouseCaptured)
                    {
                        item.IsPressed = false;
                    }
                    item.OnSelected();
                    if (parentGroupBar != null)
                    {
                        parentGroupBar.SelectedObject = item;                        
                    }
                }
                else
                {
                    item.OnUnselected();
                }
            }
        }

        /// <summary>
        /// Invoked when <see cref="IsInEditMode"/> is changed.
        /// </summary>
        /// <param name="d">GroupViewItem object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private static void OnIsInEditModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GroupViewItem item = d as GroupViewItem;

            if (item != null)
            {
                if (item.IsInEditMode)
                {
                    item.OnBeforeEdit();
                }
                else
                {
                    item.OnAfterEdit();
                }

                if (item.ParentGroupBar != null)
                {
                    item.ParentGroupBar.EnabledKeyBoardNavigation = !item.IsInEditMode;
                }

                if (item.m_textEditor != null)
                {
                    string tooltipText = item.ToolTip.ToString();

                    if (tooltipText == String.Empty || (bool)e.NewValue == false)
                    {
                        item.ToolTip = item.m_textEditor.Text;
                    }
                }
            }
        }

        /// <summary>
        /// Invoked when mouse hovers over the header.
        /// </summary>
        protected virtual void OnHover()
        {
            FireRoutedEvent(HoverEvent);
        }

        /// <summary>
        /// Invoked when <see cref="IsSelected"/> property is changed.
        /// Items is selected.
        /// </summary>
        protected virtual void OnSelected()
        {
            FireRoutedEvent(SelectedEvent);
        }

        /// <summary>
        /// Invoked when <see cref="IsSelected"/> property is changed.
        /// Items isn't selected.
        /// </summary>
        protected virtual void OnUnselected()
        {
            FireRoutedEvent(UnselectedEvent);
        }

        /// <summary>
        /// Invoked when the item is pressed.
        /// </summary>
        protected virtual void OnPress()
        {
            FireRoutedEvent(PressEvent);
        }

        /// <summary>
        /// Invoked when mouse performs click on the header.
        /// </summary>
        protected virtual void OnClick()
        {
            FireRoutedEvent(ClickEvent);
        }

        /// <summary>
        /// Invoked after renaming the <see cref="Syncfusion.Windows.Tools.Controls.GroupBarItem"/>.
        /// </summary>
        protected virtual void OnAfterEdit()
        {
            FireRoutedEvent(AfterEditEvent);
        }

        /// <summary>
        /// Invoked before renaming the <see cref="Syncfusion.Windows.Tools.Controls.GroupBarItem"/>.
        /// </summary>
        protected virtual void OnBeforeEdit()
        {
            FireRoutedEvent(BeforeEditEvent);
        }

        /// <summary>
        /// Invoked when mouse performs double click on the header.
        /// </summary>
        protected virtual void OnDoubleClick()
        {
            FireRoutedEvent(DoubleClickEvent);
        }

        /// <summary>
        /// Invoked when item is started to drag.
        /// </summary>
        protected virtual void OnDragStart()
        {
            if (VisualParent != null && ParentGroupBar != null)
            {
                ParentGroupBar.DraggingItemInProgress = true;                
            }

            FireRoutedEvent(DragStartEvent);
        }

        /// <summary>
        /// Invoked when item is ended to drag.
        /// </summary>
        protected virtual void OnDragEnd()
        {
            if (VisualParent != null && ParentGroupBar != null)
            {
                ParentGroupBar.DraggingItemInProgress = false;
            }

            FireRoutedEvent(DragEndEvent);
        }

        /// <summary>
        /// Updates the value of <see cref="IsPressed"/> property in accordance with
        /// the current position of mouse.
        /// </summary>
        private void UpdateIsPressed()
        {
            Point currentPoint = Mouse.PrimaryDevice.GetPosition(this);

            if ((currentPoint.X >= 0 && currentPoint.X <= ActualWidth)
                && (currentPoint.Y >= 0 && currentPoint.Y <= ActualHeight))
            {
                IsPressed = true;
            }
            else
            {
                IsPressed = false;
            }
        }

        private AdornerLayer adornerLayer = null;
        /// <summary>
        /// Tries to add the given adorner to the adorner layer of the logical
        /// parent.
        /// </summary>
        /// <param name="adorner">Adorner to be added.</param>
        /// <returns>
        /// Value indicating whether adorner has been successfully added
        /// to the adorner layer.
        /// </returns>
        private bool TryAddAdorner(Adorner adorner)
        {
            bool ret = false;
            if (adorner != null)
            {
                if(adornerLayer==null)
                 adornerLayer = GroupBar.GetAdornerLayer(LogicalParent);

                if (adornerLayer != null)
                {
                    adornerLayer.Add(adorner);
                    lastDraggingAdorner = adorner;
                    ret = true;
                }
            }

            return ret;
        }

        /// <summary>
        /// Tries to remove the given adorner from the adorner layer of
        /// the logical parent.
        /// </summary>
        /// <param name="adorner">Adorner to be removed.</param>
        /// <returns>
        /// Value indicating whether adorner has been successfully
        /// removed from the adorner layer.
        /// </returns>
        private bool TryRemoveAdorner(Adorner adorner)
        {
            bool ret = false;

            if (adorner != null)
            {
               if(adornerLayer==null)
               adornerLayer = GroupBar.GetAdornerLayer(LogicalParent);

                if (adornerLayer != null)
                {
                    adornerLayer.Remove(adorner);

                    if (adorner.Equals(m_dragAdorner))
                    {
                        m_dragAdorner = null;
                    }

                    ret = true;
                }
            }

            return ret;
        }

        /// <summary>
        /// If dragging has not been started yet, method defines whether
        /// it is the time to start dragging operation. If dragging is
        /// already being processed, it updates adorner with new left and
        /// top offsets.
        /// </summary>
        private void UpdateDragging()
        {
            Point currentPosition = Mouse.GetPosition(this);
            double leftOffset = currentPosition.X - m_mouseDownPoint.X;
            double topOffset = currentPosition.Y - m_mouseDownPoint.Y;

            if (ParentGroupBar != null && ParentGroupBar.FlowDirection == FlowDirection.RightToLeft)
            {
                leftOffset = -leftOffset;
            }

            if (!IsDragging)
            {
                if (Math.Abs(leftOffset) > SystemParameters.MinimumHorizontalDragDistance
                    || Math.Abs(topOffset) > SystemParameters.MinimumVerticalDragDistance)
                {
                    DragProvider.DragStarted(this);
                }
            }
            else
            {
                DragProvider.DragMoved(this, leftOffset, topOffset);
                UpdateAdornerMarker();
                ExpandByTimer();
            }
        }

        /// <summary>
        /// Expands <see cref="Syncfusion.Windows.Tools.Controls.GroupBarItem"/> when dragging item is over it.
        /// </summary>
        private void ExpandByTimer()
        {
            if (ParentGroupBar != null && IsDragging && ParentGroupBar.Items[0] != null && ParentGroupBar.Items[0] is GroupBarItem)
            {
                foreach (GroupBarItem itemOver in ParentGroupBar.Items)
                {
                    Point pos = Mouse.GetPosition(itemOver);
                    Rect rect = new Rect(0, 0, itemOver.ActualWidth, itemOver.ActualHeight);
                    m_lastDragoverItem = null;
                    GroupView content = itemOver.Content as GroupView;
                    Rect contentRect = new Rect();

                    if (content != null)
                    {
                        contentRect = new Rect(0, 0, content.ActualWidth, content.ActualHeight);
                    }

                    if (rect.Contains(pos) && !itemOver.IsExpanded)
                    {
                        if (contentRect.Contains(pos) || m_lastDragoverItem == null)
                        {
                            m_lastDragoverItem = itemOver;
                        }

                        if (itemOver.LogicalParent.VisualMode != VisualMode.MultipleExpansion)
                        {
                            itemOver.SetExpanded();
                        }
                        else
                        {
                            DispatcherTimer timer = new DispatcherTimer();
                            timer.Tag = itemOver;
                            timer.Tick += new EventHandler(ExpandTimer_Tick);
                            timer.Interval = TimeSpan.FromMilliseconds(C_expandDelay);
                            timer.Start();
                        }

                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Updates adorner marker for the dragging item.
        /// </summary>
        private void UpdateAdornerMarker()
        {
            if (LogicalParent != null)
            {
                GroupViewItem item = null;
                GroupView view = null;
                Point currentPosition = new Point();
                FrameworkElement dragingElement = null;

                if (ParentGroupBar != null && !ParentGroupBar.IsCollapsed && ParentGroupBar.Items[0] != null && ParentGroupBar.Items[0] is GroupBarItem)
                {

                    foreach (GroupBarItem barItem in ParentGroupBar.Items)
                    {
                        view = barItem.Content as GroupView;
                        if (view == null)
                        {
                            view = VisualUtils.FindDescendant(barItem as Visual, typeof(GroupView)) as GroupView;
                        }

                        if (view != null)
                        {
                            foreach (object obj in view.Items)
                            {
                                item = view.GetItemContainer(obj);

                                if (item != null)
                                {
                                    item.TryRemoveAdorner(item.AdornerMarker);
                                    item.IsDragOverTop = false;
                                    item.IsDragOverDown = false;
                                }
                            }
                        }
                    }
                     currentPosition = Mouse.GetPosition(ParentGroupBar);
                    dragingElement = ParentGroupBar.InputHitTest(currentPosition) as FrameworkElement;
                }
                else
                {
                    foreach (object obj in this.LogicalParent.Items)
                    {
                        item = this.LogicalParent.GetItemContainer(obj);

                        if (item != null)
                        {
                            item.TryRemoveAdorner(item.AdornerMarker);
                            item.IsDragOverTop = false;
                            item.IsDragOverDown = false;
                        }
                    }

                    currentPosition = Mouse.GetPosition(this.LogicalParent);
                    dragingElement = this.LogicalParent.InputHitTest(currentPosition) as FrameworkElement;
                }

                if (dragingElement != null && IsDragging)
                {
                    GroupViewItem dragingItem = dragingElement.TemplatedParent as GroupViewItem;

                    if (dragingItem != null && dragingItem != this)
                    {
                        dragingItem.TryAddAdorner(dragingItem.AdornerMarker);
                        Point itemPosition = Mouse.GetPosition(dragingItem);

                        if (LogicalParent.IsListViewMode)
                        {
                            if (itemPosition.Y > dragingItem.ActualHeight / 2)
                            {
                                dragingItem.IsDragOverDown = true;
                            }
                            else
                            {
                                dragingItem.IsDragOverTop = true;
                            }

                            dragingItem.AdornerMarker.TopOffset = dragingItem.IsDragOverDown ? dragingItem.ActualHeight : 0;
                            dragingItem.AdornerMarker.LeftOffset = 0;
                        }
                        else
                        {
                            if (itemPosition.X > dragingItem.ActualWidth / 2)
                            {
                                dragingItem.IsDragOverDown = true;
                            }
                            else
                            {
                                dragingItem.IsDragOverTop = true;
                            }

                            double offsetWidth = dragingItem.ActualWidth;

                            if (ParentGroupBar != null && ParentGroupBar.VisualMode != VisualMode.MultipleExpansion
                                && ParentGroupBar.FlowDirection == FlowDirection.RightToLeft)
                            {
                                offsetWidth = -dragingItem.ActualWidth;
                            }

                            dragingItem.AdornerMarker.LeftOffset = dragingItem.IsDragOverDown ? offsetWidth : 0;
                            dragingItem.AdornerMarker.TopOffset = 0;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Invokes when timer counting ends.
        /// </summary>
        /// <param name="sender">Source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> that contains the event data.</param>
        private void ExpandTimer_Tick(object sender, EventArgs e)
        {
            DispatcherTimer timer = sender as DispatcherTimer;

            if (timer != null)
            {
                timer.Stop();
                GroupBarItem item = timer.Tag as GroupBarItem;

                if (item != null && !item.IsExpanded && (item == m_lastDragoverItem) && IsDragging)
                {
                    item.IsExpanded = true;
                }
            }
        }

        /// <summary>
        /// Handles to IsVisibleChanged event of the text editor.
        /// </summary>
        /// <param name="sender">Source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private void TextEditor_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (TextEditor != null && this.TextEditor.IsVisible)
            {
                TextEditor.Focus();
                TextEditor.SelectAll();
            }
        }

        /// <summary>
        /// Handles the LostKeyboardFocus event of the TextEditor control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.KeyboardFocusChangedEventArgs"/> instance containing the event data.</param>
        private void TextEditor_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            LogicalParent.UpdateInternalContentLength();
            IsInEditMode = false;
        }

        /// <summary>
        /// Raises the routed event.
        /// </summary>
        /// <param name="e">The instance containing the event data.</param>
        private void FireRoutedEvent(RoutedEvent e)
        {
            RoutedEventArgs args = new RoutedEventArgs(e);
            RaiseEvent(args);
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="System.Windows.UIElement.MouseLeftButtonDown"/> routed 
        /// event is raised on this element. 
        /// Implement this method to add class handling for this event. 
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> that contains the event data. 
        /// The event data reports that the left mouse button was pressed.</param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            e.Handled = true;

            if (e.ButtonState == MouseButtonState.Pressed)
            {
                IsPressed = true;
                m_mouseDownPoint = e.GetPosition(this);
                CaptureMouse();
            }
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="System.Windows.UIElement.MouseLeftButtonUp"/> routed event reaches 
        /// an element in its route that is derived from this class. 
        /// Implement this method to add class handling for this event. 
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> that contains the event data. 
        /// The event data reports that the left mouse button was released.</param>
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            //e.Handled = true;

            if (IsMouseOver && !IsSelected)
            {
                Focus();
                if (ParentGroupBar != null)
                {
                    ParentGroupBar.SelectedObject = this;
                }
                OnClick();
            }

            if (IsMouseCaptured)
            {
                IsPressed = false;
                ReleaseMouseCapture();

                if (IsDragging)
                {
                    DragProvider.DragCompleted(this);

                    this.RefreshContentHostWidth();

                    if (VisualParent != null)
                    {
                        UpdateAdornerMarker();
                    }
                }
            }
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="System.Windows.UIElement.MouseRightButtonUp"/>�routed
        /// event reaches an element in its route that is derived from this class. 
        /// Used for correct editing of the groupViewItem.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> that contains the event data.</param>
        protected override void OnMouseRightButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseRightButtonUp(e);

            if (ParentGroupBar != null)
            {
                ParentGroupBar.SelectedObject = this;
            }

            if (IsDragging)
            {
                DragProvider.DragCompleted(this);
                e.Handled = true;
                if (e.ButtonState == MouseButtonState.Pressed)
                {
                    IsPressed = true;
                    CaptureMouse();
                }
            }
        }

        /// <summary>
        /// Raises the <see cref="System.Windows.Controls.Control.MouseDoubleClick"/> event. 
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> that contains the event data.</param>
        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            base.OnMouseDoubleClick(e);
            OnDoubleClick();
        }

        private bool isEscapeKeyPressed = false;

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.MouseMove"/> attached event reaches 
        /// an element in its route that is derived from this class. 
        /// Implement this method to add class handling for this event. 
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseEventArgs"/> that contains the event data.</param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (Keyboard.IsKeyDown(Key.Escape))
                isEscapeKeyPressed = true;
            if (isEscapeKeyPressed && Keyboard.IsKeyUp(Key.Escape))
            {
                isEscapeKeyPressed = false;
            }
            else if (IsMouseCaptured && Mouse.PrimaryDevice.LeftButton == MouseButtonState.Pressed)
            {
                UpdateDragging();
                UpdateIsPressed();
                e.Handled = true;
            }
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.MouseEnter"/> attached event
        /// is raised on this element. 
        /// Implement this method to add class handling for this event. 
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> that contains the event data.</param>
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            OnHover();
        }

        /// <summary>
        /// Invoked when to the <see cref="E:System.Windows.Input.Keyboard.KeyDown"/> event is received.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.KeyEventArgs"/> that contains the event data.</param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.Key == Key.Enter)
            {
                if (IsInEditMode)
                {
                    IsInEditMode = false;
                }
                else
                {
                    OnClick();
                }
            }
            else if (e.Key == Key.Escape)
            {
                if (IsInEditMode)
                {
                    m_textEditor.Undo();
                    IsInEditMode = false;
                }
                else
                {
                    OnClick();
                }
            }
        }

        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                if(IsMouseOver)
                DragCancel();
                TryRemoveAdorner(lastDraggingAdorner);
                m_dragAdorner = null;
            }
        }
        private void DragCancel()
        {
            IsPressed = false;
            ReleaseMouseCapture();
            this.IsDragging = false;
            this.IsDragOverDown = false;
            this.IsDragOverTop = false;            
            Focus();            
        }
        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Keyboard.GotKeyboardFocus"/> attached event reaches 
        /// an element in its route that is derived from this class. 
        /// Implement this method to add class handling for this event. 
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.KeyboardFocusChangedEventArgs"/> that contains the event data.</param>
        protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            base.OnGotKeyboardFocus(e);
        }

        /// <summary>
        /// Invoked whenever an unhandled <see cref="System.Windows.UIElement.GotFocus"/> event reaches this element in its route. 
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> that contains the event data.</param>
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);

            if (ParentGroupBar != null)
            {
                ParentGroupBar.SelectedObject = this;
            }
            else
            {
                if (this.LogicalParent != null)
                {
                    for (int i = 0; i < this.LogicalParent.Items.Count; i++)
                    {
                        GroupViewItem item = null;
                        if (item == null)
                            item = this.LogicalParent.ItemContainerGenerator.ContainerFromIndex(i) as GroupViewItem;
                        if (item != null)
                        {
                            item.IsSelected = false;
                        }
                    }
                }

                this.IsSelected = true;
            }
        }

        /// <summary>
        /// Raises the <see cref="System.Windows.UIElement.LostFocus"/> routed event by using the event data that is provided.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> that contains the event data.</param>
        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);
        }

        /// <summary>
        /// Gets the length of the internal content.
        /// </summary>
        /// <returns> Content width </returns>
        internal double GetInternalContentLength()
        {
            double width = 0d;

            if (m_textHost != null)
            {
                width += m_textHost.ActualWidth;
            }

            if (m_imageHost != null)
            {
                width += m_imageHost.ActualWidth;
            }

            return width;
        }

        /// <summary>
        /// Overrides MeasureOverride method.
        /// </summary>
        /// <param name="constraint">Settled by a father measure.</param>
        /// <returns>
        /// GroupView measure.
        /// </returns>
        protected override Size MeasureOverride(Size constraint)
        {
            double corectWidth = constraint.Width;
            constraint = base.MeasureOverride(constraint);

            if (LogicalParent.IsListViewMode == true)
            {
                constraint.Width = corectWidth;
            }

            if (double.IsInfinity(constraint.Width))
            {
                Border itemsHost = ParentGroupBar.ItemsHostBorder;

                if (itemsHost != null)
                {
                    constraint.Width = itemsHost.ActualWidth;
                }
                else
                {
                    constraint.Width = ParentGroupBar.PopupContentHost.ActualWidth;
                }
            }

            SetContentMinWidth();

            return constraint;
        }

        /// <summary>
        /// Sets MinWidth of the content host inside the GroupViewItem. Horizontal scrolling starts from this width.
        /// </summary>
        private void SetContentMinWidth()
        {
            DockPanel contentHost = GetTemplateChild(C_nameContentHost) as DockPanel;

            if (contentHost != null && ParentGroupBar != null)
            {
                contentHost.MinWidth = ParentGroupBar.MinWidthOfList;
            }
        }

        /// <summary>
        /// Invoked when the parent of this element in the visual tree is changed. 
        /// </summary>
        /// <param name="oldParent">The object that the context menu was previously attached to.</param>
        protected override void OnVisualParentChanged(DependencyObject oldParent)
        {
            base.OnVisualParentChanged(oldParent);

            if (VisualParent != null)
            {
                UpdateAdornerMarker();
            }
        }

        /// <summary>
        /// Gets the group view from children.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns> Group view item</returns>
        private GroupView GetGroupViewFromChildren(FrameworkElement element)
        {
            GroupView view = ItemsControl.ItemsControlFromItemContainer(element) as GroupView;

            if (view == null)
            {
                while (element != null)
                {
                    element = VisualTreeHelper.GetParent(element) as FrameworkElement;

                    if (element is GroupView)
                    {
                        view = (GroupView)element;
                        break;
                    }
                }
            }

            return view;
        }

        /// <summary>
        /// Refreshes width of the ContentHost template child after the drag'n'drop operation.
        /// </summary>
        /// <remarks>
        /// It is needed because of changing of the logical parent.
        /// </remarks>
        private void RefreshContentHostWidth()
        {
            DockPanel contentHost = this.GetTemplateChild(C_nameContentHost) as DockPanel;

            if (contentHost != null)
            {
                contentHost.SetBinding(FrameworkElement.WidthProperty, Binder.Bind(this.LogicalParent, "InternalContentLength"));
            }
        }
        #endregion

        #region Internal declarations
        /// <summary>
        /// Manages all drag and drop operations of the <see cref="Syncfusion.Windows.Tools.Controls.GroupViewItem"/>.
        /// </summary>
        private static class DragProvider
        {
            #region Constants
            /// <summary>
            /// Used for enabling scrolling the <see cref="Syncfusion.Windows.Tools.Controls.GroupBar"/> 
            /// content in multiple expansion mode when <see cref="Syncfusion.Windows.Tools.Controls.GroupViewItem"/> is dragging.
            /// </summary>
            private const double C_scrollingRowHeight = 2;

            internal static GroupViewItem currentDragItem = null;
            #endregion

            #region Class members
            /// <summary>
            /// Cursor that had been on <see cref="Syncfusion.Windows.Tools.Controls.GroupViewItem"/> before dragging was
            /// started.
            /// </summary>
            private static Cursor previousCursor;
            #endregion

            #region Class static internal methods
            /// <summary>
            /// Should be called when <see cref="Syncfusion.Windows.Tools.Controls.GroupViewItem"/> is going to be dragged.
            /// </summary>
            /// <param name="caller">The caller.</param>
            internal static void DragStarted(GroupViewItem caller)
            {
                caller.IsDragging = true;
                previousCursor = caller.Cursor;
                caller.Cursor = Cursors.Hand;
                caller.TryAddAdorner(caller.AdornerDrag);
                currentDragItem = caller;
                GroupBar gb = caller.ParentGroupBar as GroupBar;
                if (gb != null)
                {
                    if (gb.DragItemVisibility == Visibility.Visible)
                        caller.Opacity = 1d;
                    else
                        caller.Opacity = 0d;
                }
                else
                {
                    caller.Opacity = 0d;
                }
                caller.OnDragStart();
            }

            /// <summary>
            /// Should be called when <see cref="Syncfusion.Windows.Tools.Controls.GroupViewItem"/> is already dragged and
            /// needs to update its adorner.
            /// </summary>
            /// <param name="caller">The <see cref="Syncfusion.Windows.Tools.Controls.GroupViewItem"/> that is dragged.</param>
            /// <param name="leftOffset">New left offset of adorner.</param>
            /// <param name="topOffset">New top offset of adorner.</param>
            internal static void DragMoved(GroupViewItem caller, double leftOffset, double topOffset)
            {
                if (caller != null)
                {
                    caller.AdornerDrag.LeftOffset = leftOffset;
                    caller.AdornerDrag.TopOffset = topOffset;
                    caller.Cursor = caller.IsPossibleDrop ? Cursors.Hand : Cursors.No;

                    if (caller.ParentGroupBar != null
                        && caller.ParentGroupBar.VisualMode == VisualMode.MultipleExpansion)
                    {
                        Point currentPosition = Mouse.GetPosition(caller.ParentGroupBar);
                        double groupBarTopOffset = currentPosition.Y;
                        double actualHeight = Math.Floor(caller.ParentGroupBar.ActualHeight);

                        if (groupBarTopOffset >= actualHeight - C_scrollingRowHeight)
                        {
                            caller.ParentGroupBar.ScrollViewerHost.LineDown();
                        }
                        else if (groupBarTopOffset <= C_scrollingRowHeight)
                        {
                            caller.ParentGroupBar.ScrollViewerHost.LineUp();
                        }
                    }
                }
            }

            /// <summary>
            /// Should be called when <see cref="Syncfusion.Windows.Tools.Controls.GroupViewItem"/> is dropped.
            /// </summary>
            /// <param name="caller">The <see cref="Syncfusion.Windows.Tools.Controls.GroupViewItem"/> that is dropped.</param>
            internal static void DragCompleted(GroupViewItem caller)
            {
                caller.AdornerDrag.ResetPosition();
                caller.TryRemoveAdorner(caller.AdornerDrag);
                UIElement dropTarget = GetItemFromDropPoint();

                if (caller.IsPossibleDrop && dropTarget != null)
                {
                    GroupView gview = dropTarget as GroupView;
                    GroupBar gbar;

                    if (gview != null && !gview.Equals(caller.LogicalParent))
                    {
                        DoDragDrop(caller, gview);
                        if (gview.LogicalParent != null && gview.LogicalParent.LogicalParent != null)
                            gview.LogicalParent.LogicalParent.UpdateItemsSize();
                        else
                        {
                            gbar = VisualUtils.FindAncestor(gview, typeof(GroupBar)) as GroupBar;
                            if (gbar != null)
                                gbar.UpdateItemsSize();
                        }
                    }
                    else
                    {
                        GroupViewItem item = dropTarget as GroupViewItem;

                        if (!caller.Equals(item) && item != null)
                        {
                            DoDragDrop(caller, item);
                        }
                    }

                    if (dropTarget is GroupViewItem)
                    {
                        (dropTarget as GroupViewItem).IsDragOverTop = false;
                        (dropTarget as GroupViewItem).IsDragOverDown = false;
                    }

                    DoDragDrop(caller, dropTarget);
                }

                caller.Cursor = previousCursor;
                caller.IsDragging = false;
                caller.Opacity = 1d;
                caller.IsDragOverTop = false;
                caller.IsDragOverDown = false;
                caller.OnDragEnd();
            }

            /// <summary>
            /// Does the drag drop.
            /// </summary>
            /// <param name="dragSource">The drag source.</param>
            /// <param name="newParent">The new parent.</param>
            private static void DoDragDrop(GroupViewItem dragSource, UIElement newParent)
            {
                GroupView oldParent = dragSource.LogicalParent;
                if (oldParent != null)
                {
                    IList oldList = (oldParent.ItemsSource == null) ?
                            oldParent.Items as IList : oldParent.ItemsSource as IList;

                    object dragItem = oldParent.ItemContainerGenerator.ItemFromContainer(dragSource);
                    GroupViewItem item = dragSource as GroupViewItem;
                    DataObject dragData = new DataObject(typeof(GroupViewItem), dragItem);
                    try
                    {
                        DragDrop.DoDragDrop(dragSource, dragData, DragDropEffects.Copy);
                    }
                    //SU I78477
                    //catch (Exception ex)
                    catch (Exception)
                        //EU I78477
                    {
                        MessageBox.Show("Unable to detach the item from its parent");
                    }
                }
            }
            #endregion

            #region Class static private methods
            /// <summary>
            /// Performs drag and drop with drop target 
            /// as another <see cref="Syncfusion.Windows.Tools.Controls.GroupView"/>.
            /// </summary>
            /// <param name="dragSource">The drag source.</param>
            /// <param name="newParent">The drop target.</param>
            private static void DoDragDrop(GroupViewItem dragSource, GroupView newParent)
            {
                GroupView oldParent = dragSource.LogicalParent;

                if (newParent != null && oldParent != null)
                {
                    foreach (GroupViewItem item in oldParent.Items)
                    {
                        item.IsSelected = false;
                    }
                    IList oldList = (oldParent.ItemsSource == null) ?
                        oldParent.Items as IList : oldParent.ItemsSource as IList;
                    IList newList = (newParent.ItemsSource == null) ?
                        newParent.Items as IList : newParent.ItemsSource as IList;

                    if (oldList != null && newList != null)
                    {
                        object dragItem = oldParent.ItemContainerGenerator.ItemFromContainer(dragSource);
                        oldList.Remove(dragItem);
                        newList.Add(dragItem);
                    }
                    foreach (GroupViewItem item in newParent.Items)
                    {
                        item.IsSelected = false;
                    }
                    dragSource.IsSelected = true;
                }
            }

            /// <summary>
            /// Performs drag and drop with drop target 
            /// as another <see cref="Syncfusion.Windows.Tools.Controls.GroupViewItem"/>.
            /// </summary>
            /// <param name="dragSource">The drag source.</param>
            /// <param name="dropTarget">The drop target.</param>
            private static void DoDragDrop(GroupViewItem dragSource, GroupViewItem dropTarget)
            {
                GroupView oldParent = dragSource.LogicalParent;
                GroupView newParent = dropTarget.LogicalParent;

                IList oldList = (oldParent.ItemsSource == null) ?
                    oldParent.Items as IList : oldParent.ItemsSource as IList;
                IList newList = (newParent.ItemsSource == null) ?
                    newParent.Items as IList : newParent.ItemsSource as IList;

                if (oldList != null && newList != null)
                {
                    object dragItem = oldParent.ItemContainerGenerator.ItemFromContainer(dragSource);
                    object dropItem = newParent.ItemContainerGenerator.ItemFromContainer(dropTarget);
                    oldList.Remove(dragItem);
                    int targetIndex = newParent.Items.IndexOf(dropItem);
                    if (targetIndex >= 0)
                    {
                        if (dropTarget.IsDragOverDown)
                        {
                            newList.Insert(targetIndex + 1, dragItem);
                        }
                        else
                        {
                            newList.Insert(targetIndex, dragItem);
                        }
                    }
                    DataObject dragData = new DataObject(typeof(GroupViewItem), dragItem);
                    try
                    {
                        DragDrop.DoDragDrop(dragSource, dragData, DragDropEffects.Copy);
                    }
                    //SU I78477
                    //catch (Exception ex)
                    catch (Exception)
                        //EU I78477
                    {
                        MessageBox.Show("Unable to detach the item from its parent");
                    }
                }
            }

            /// <summary>
            /// Finds element on which dragged <see cref="Syncfusion.Windows.Tools.Controls.GroupViewItem"/> is dropped.
            /// </summary>
            /// <returns>
            /// Element on which <see cref="Syncfusion.Windows.Tools.Controls.GroupViewItem"/> is dropped.
            /// </returns>
            private static UIElement GetItemFromDropPoint()
            {
                UIElement element = Mouse.DirectlyOver as UIElement;
                UIElement returnElement = element;

                while (element != null)
                {
                    element = VisualTreeHelper.GetParent(element) as UIElement;
                    GroupViewItem item = element as GroupViewItem;

                    if (item != null)
                    {
                        return item;
                    }

                    GroupView gview = element as GroupView;

                    if (gview != null)
                    {
                        return gview;
                    }

                    GroupBar gB = element as GroupBar;
                    if (gB != null)
                    {
                        return gB;
                    }
                }

                return returnElement;
            }           
            #endregion
        }

        /// <summary>
        /// Used for creating the adorner from the control.
        /// </summary>
        private class DragAdorner : Adorner
        {
            #region Members
            /// <summary>
            /// Rectangle to be painted with the visual of the control.
            /// </summary>
            private Rectangle m_child;

            /// <summary>
            /// Logical parent as <see cref="Syncfusion.Windows.Tools.Controls.GroupViewItem"/>.
            /// </summary>
            private GroupViewItem m_logicalParent;

            /// <summary>
            /// Left offset of the adorner relatively to point where
            /// <see cref="Syncfusion.Windows.Tools.Controls.GroupViewItem"/> was pressed.
            /// </summary>
            private double m_topOffset;

            /// <summary>
            /// Top offset of the adorner relatively to point where
            /// <see cref="Syncfusion.Windows.Tools.Controls.GroupViewItem"/> was pressed.
            /// </summary>
            private double m_leftOffset;
            #endregion

            #region Properties
            /// <summary>
            /// Gets or sets the left offset.
            /// </summary>
            /// <value>The left offset.</value>
            public double LeftOffset
            {
                get
                {
                    return m_leftOffset;
                }

                set
                {
                    m_leftOffset = value;
                    UpdatePosition();
                }
            }

            /// <summary>
            /// Gets or sets the top offset.
            /// </summary>
            /// <value>The top offset.</value>
            public double TopOffset
            {
                get
                {
                    return m_topOffset;
                }

                set
                {
                    m_topOffset = value;
                    UpdatePosition();
                }
            }

            /// <summary>
            /// Gets the logical parent.
            /// </summary>
            /// <value>The logical parent.</value>
            public GroupViewItem LogicalParent
            {
                get
                {
                    return m_logicalParent;
                }
            }
            #endregion

            #region Initialize/Finalize methods
            /// <summary>
            /// Initializes a new instance of the <see cref="DragAdorner"/> class.
            /// </summary>
            /// <param name="adornedElement">The adorned element.</param>
            public DragAdorner(FrameworkElement adornedElement)
                : base(adornedElement)
            {
                IsHitTestVisible = false;

                m_logicalParent = adornedElement.TemplatedParent as GroupViewItem;
                m_logicalParent.UpdateLayout();

                m_child = new Rectangle();

                m_child.Width = adornedElement.RenderSize.Width;
                m_child.Height = adornedElement.RenderSize.Height;

                RenderTargetBitmap renderer = new RenderTargetBitmap(
                    (int)m_child.Width,
                    (int)m_child.Height,
                    0d,
                    0d,
                    PixelFormats.Default);
                renderer.Render(adornedElement);
                m_child.Fill = new ImageBrush(renderer);
                m_child.Opacity = 0.7d;
            }

            #endregion

            #region Implementation
            /// <summary>
            /// Updates the position of the adorner with new left and top offsets.
            /// </summary>
            private void UpdatePosition()
            {
                AdornerLayer adornerLayer = GroupBar.GetAdornerLayer(LogicalParent.LogicalParent);

                if (adornerLayer != null)
                {
                    Adorner[] adorners = adornerLayer.GetAdorners(AdornedElement);

                    if (adorners != null && adorners.Length > 0)
                    {
                        adornerLayer.Update(AdornedElement);
                    }
                }
            }

            /// <summary>
            /// Resets the position of the adorner.
            /// </summary>
            internal void ResetPosition()
            {
                m_leftOffset = 0;
                m_topOffset = 0;
            }
            #endregion

            #region Overrides
            /// <summary>
            /// Invoked to remeasure the control. 
            /// </summary>
            /// <param name="constraint">Measurement constraints, 
            /// a control cannot return a size larger than the constraint.</param>
            /// <returns>The size of the control.</returns>
            protected override Size MeasureOverride(Size constraint)
            {
                m_child.Measure(constraint);
                return m_child.DesiredSize;
            }

            /// <summary>
            /// Invoked to arrange and size the content of the control.
            /// </summary>
            /// <param name="finalSize">The computed size that is used to arrange the content.</param>
            /// <returns>The size of the control.</returns>
            protected override Size ArrangeOverride(Size finalSize)
            {
                m_child.Arrange(new Rect(finalSize));
                return finalSize;
            }

            /// <summary>
            /// Overrides <see cref="System.Windows.Media.Visual.GetVisualChild"/>, and returns a child at the specified index 
            /// from a collection of child elements. 
            /// </summary>
            /// <param name="index">The zero-based index of the requested 
            /// child element in the collection.</param>
            /// <returns>The requested child element.</returns>
            protected override Visual GetVisualChild(int index)
            {
                return m_child;
            }

            /// <summary>
            /// Gets the number of visual child elements within this element. 
            /// </summary>
            protected override int VisualChildrenCount
            {
                get
                {
                    return 1;
                }
            }

            /// <summary>
            /// Returns a transform for the adorner, based on the transform 
            /// that is currently applied to the adorned element. 
            /// </summary>
            /// <param name="transform">The transform that is currently applied 
            /// to the adorned element.</param>
            /// <returns>A transform to apply to the adorner.</returns>
            public override GeneralTransform GetDesiredTransform(GeneralTransform transform)
            {
                GeneralTransformGroup result = new GeneralTransformGroup();
                result.Children.Add(base.GetDesiredTransform(transform));
                result.Children.Add(new TranslateTransform(LeftOffset, TopOffset));
                return result;
            }

            #endregion
        }

        /// <summary>
        /// Used for creating the visual adorner for the 
        /// drag marker of the <see cref="Syncfusion.Windows.Tools.Controls.GroupViewItem"/> when it is dragged.
        /// </summary>
        private class DragMarkerAdorner : Adorner
        {
            #region Constants
            /// <summary>
            /// Default height for the marker.
            /// </summary>
            private const double C_defaultMarkerHeight = 2;
            #endregion

            #region Members
            /// <summary>
            /// Rectangle to be painted with the visual of the item's header.
            /// </summary>
            protected Rectangle m_child;

            /// <summary>
            /// Logical parent as <see cref="Syncfusion.Windows.Tools.Controls.GroupBarItem"/>.
            /// </summary>
            protected GroupViewItem m_logicalParent;

            /// <summary>
            /// Left offset of the adorner relatively to point where
            /// <see cref="Syncfusion.Windows.Tools.Controls.GroupBarItem"/> header was pressed.
            /// </summary>
            private double m_topOffset;

            /// <summary>
            /// Top offset of the adorner relatively to point where
            /// <see cref="Syncfusion.Windows.Tools.Controls.GroupBarItem"/> header was pressed.
            /// </summary>
            private double m_leftOffset;
            #endregion

            #region Properties

            /// <summary>
            /// Gets the logical parent.
            /// </summary>
            /// <value>The logical parent.</value>
            public GroupViewItem LogicalParent
            {
                get
                {
                    return m_logicalParent;
                }
            }

            /// <summary>
            /// Gets or sets the top offset.
            /// </summary>
            /// <value>The top offset.</value>
            public double TopOffset
            {
                get
                {
                    return m_topOffset;
                }

                set
                {
                    m_topOffset = value;
                    UpdatePosition();
                }
            }

            /// <summary>
            /// Gets or sets the left offset.
            /// </summary>
            /// <value>The left offset.</value>
            public double LeftOffset
            {
                get
                {
                    return m_leftOffset;
                }

                set
                {
                    m_leftOffset = value;
                    UpdatePosition();
                }
            }
            #endregion

            #region Initialize/Finalize methods
            /// <summary>
            /// Initializes a new instance of the <see cref="DragMarkerAdorner"/> class.
            /// </summary>
            /// <param name="adornedElement">The adorned element.</param>
            public DragMarkerAdorner(FrameworkElement adornedElement)
                : base(adornedElement)
            {
                m_logicalParent = adornedElement as GroupViewItem;
                m_child = new Rectangle();
                IntializeMarker();
            }
            #endregion

            #region Implementation
            /// <summary>
            /// Initializes the marker.
            /// </summary>
            private void IntializeMarker()
            {
                if (LogicalParent != null && LogicalParent.LogicalParent != null)
                {
                    if (LogicalParent.LogicalParent.IsListViewMode)
                    {
                        m_child.SetBinding(Rectangle.WidthProperty, Binder.Bind(LogicalParent, "ActualWidth"));
                        m_child.Height = C_defaultMarkerHeight;
                    }
                    else
                    {
                        m_child.SetBinding(Rectangle.HeightProperty, Binder.Bind(LogicalParent, "ActualHeight"));
                        m_child.Width = C_defaultMarkerHeight;
                    }

                    if (LogicalParent.ParentGroupBar != null)
                    {
                        m_child.SetBinding(Rectangle.FillProperty, Binder.Bind(LogicalParent.ParentGroupBar, "DragMarkerBrush"));
                    }
                    else
                    {
                        m_child.Fill = Brushes.Red;
                    }
                }
            }

            /// <summary>
            /// Resets position of the adorner.
            /// </summary>
            internal void ResetPosition()
            {
                m_topOffset = 0;
            }

            /// <summary>
            /// Updates position of the adorner.
            /// </summary>
            private void UpdatePosition()
            {
                IntializeMarker();
                AdornerLayer adornerLayer = GroupBar.GetAdornerLayer(LogicalParent);

                if (adornerLayer != null)
                {
                    Adorner[] adorners = adornerLayer.GetAdorners(AdornedElement);

                    if (adorners != null && adorners.Length > 0)
                    {
                        adornerLayer.Update(AdornedElement);
                    }
                }
            }

            /// <summary>
            /// Returns a transform for the adorner, based on the transform 
            /// that is currently applied to the adorned element. 
            /// </summary>
            /// <param name="transform">A transform that is currently applied 
            /// to the adorned element.</param>
            /// <returns>A transform to apply to the adorner.</returns>
            public override GeneralTransform GetDesiredTransform(GeneralTransform transform)
            {
                GeneralTransformGroup result = new GeneralTransformGroup();
                result.Children.Add(base.GetDesiredTransform(transform));
                result.Children.Add(new TranslateTransform(LeftOffset, TopOffset));

                return result;
            }

            /// <summary>
            /// Invoked to remeasure the control. 
            /// </summary>
            /// <param name="constraint">Measurement constraints, 
            /// a control cannot return a size larger than the constraint.</param>
            /// <returns>The size of the control.</returns>
            protected override Size MeasureOverride(Size constraint)
            {
                m_child.Measure(constraint);

                return m_child.DesiredSize;
            }

            /// <summary>
            /// Invoked to arrange and size the content of the control.
            /// </summary>
            /// <param name="finalSize">The computed size that is used to arrange the content.</param>
            /// <returns>The size of the control.</returns>
            protected override Size ArrangeOverride(Size finalSize)
            {
                m_child.Arrange(new Rect(finalSize));

                return finalSize;
            }

            /// <summary>
            /// Overrides <see cref="System.Windows.Media.Visual.GetVisualChild"/>, and returns a child at the specified index 
            /// from a collection of child elements. 
            /// </summary>
            /// <param name="index">The zero-based index of the requested 
            /// child element in the collection.</param>
            /// <returns>The requested child element.</returns>
            protected override Visual GetVisualChild(int index)
            {
                return m_child;
            }

            /// <summary>
            /// Gets the number of visual child elements within this element. 
            /// </summary>
            protected override int VisualChildrenCount
            {
                get
                {
                    return 1;
                }
            }
            #endregion
        }
        #endregion
    }
}
