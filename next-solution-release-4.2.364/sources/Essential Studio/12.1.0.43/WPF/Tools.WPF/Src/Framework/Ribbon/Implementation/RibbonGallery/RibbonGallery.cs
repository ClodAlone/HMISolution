// <copyright file="RibbonGallery.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Syncfusion.Licensing;
using Syncfusion.Windows.Shared;
using System.IO;
using System.Windows.Markup;
using System.Xml;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents a RibbonGallery control.
    /// </summary>
    /// <list type="table">
    /// <listheader>
    /// <term>Help Page</term>
    /// <description>Syntax</description>
    /// </listheader>
    /// <example>
    /// <list type="table">
    /// <listheader>
    /// <description>C#</description>
    /// </listheader>
    /// <example><code>public class RibbonGallery : RibbonItemsControl</code></example>
    /// </list>
    /// <para/>
    /// <list type="table">
    /// <listheader>
    /// <description>XAML Object Element Usage</description>
    /// </listheader>
    /// <example><code><![CDATA[<ribbon:RibbonGallery Name="gallery" />]]></code></example>
    /// </list>
    /// </example>
    /// </list>
    /// <remarks>
    /// RibbonGallery class represents a gallery control that can display collection of UIElements in inline of popup control.
    /// </remarks>
    /// <example>
    /// <para/>This example shows how to create a RibbonGallery in XAML.
    /// <code>
    /// <![CDATA[
    /// <ribbon:RibbonGallery Margin="3" VisualMode="DropDown" SizeForm="Large" Label="The gallery" LargeIcon="/SampleImages/SchemeBlack.png" ExpandWidth="270" ItemWidth="80">
    /// <ribbon:RibbonGalleryItem>
    /// <Border ClipToBounds="True">
    /// <TextBlock>TrueItem</TextBlock>
    /// </Border>
    /// </ribbon:RibbonGalleryItem>
    /// <TextBlock>Test</TextBlock>
    /// <Image Source="SampleImages/Apex.png"/>
    /// <Image Source="SampleImages/Aspect.png"/>
    /// </ribbon:RibbonGallery]]>
    /// </code>
    /// <para/>This example shows how to create a RibbonGallery in C#.
    /// <code>
    /// RibbonGallery gallery = new RibbonGallery();
    /// TextBlock text = new TextBlock();
    /// text.Text = "Item";
    /// gallery.Items.Add(text);
    /// </code>
    /// </example>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class RibbonGallery : RibbonItemsControl, IDisposable
    {
        #region Internal declarations
        /// <summary>
        /// Resize direction enum.
        /// </summary>
        private enum Direction
        {
            /// <summary>
            /// Represents the None
            /// </summary>
            None,

            /// <summary>
            /// Represents the Up
            /// </summary>
            Up,

            /// <summary>
            /// Represents the Down
            /// </summary>
            Down
        }
        #endregion

        #region Private members
        /// <summary>
        /// Represents the Double Animation for Up
        /// </summary>
        private DoubleAnimation m_upAnimation;

        /// <summary>
        /// Represents the Double Animation for Down
        /// </summary>
        private DoubleAnimation m_downAnimation;

        /// <summary>
        /// Represents the scroll up button
        /// </summary>
        private Button m_scrollUpButton;

        /// <summary>
        /// Represents the scroll down button
        /// </summary>
        private Button m_scrollDownButton;

        /// <summary>
        /// Represents the scroll presenter
        /// </summary>
        private InRibbonItemsPresenter m_scrollPresenter;

        /// <summary>
        /// Represents the Direction
        /// </summary>
        private Direction m_currentDirection;

        /// <summary>
        /// Represents the popup items control
        /// </summary>
        private ItemsControl m_popupItemsControl;

        /// <summary>
        /// Represents the main items control
        /// </summary>
        private ItemsControl m_mainItemsControl;


        /// <summary>
        /// Represents the Ribbon gallery arranged items
        /// </summary>
        private ObservableCollection<RibbonGalleryItem> m_arrangedItems = null;


        internal bool bScrollbtnclicked = false;

        SystemGesture m_systemGesture;

        ScrollViewer PART_ScrollViewer = null;

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes static members of the <see cref="RibbonGallery"/> class.
        /// </summary>
        static RibbonGallery()
        {
            EnvironmentTest.ValidateLicense(typeof(RibbonGallery));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RibbonGallery), new FrameworkPropertyMetadata(typeof(RibbonGallery)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RibbonGallery"/> class.
        /// </summary>
        public RibbonGallery()
        {
            GalleryGroups = new ObservableCollection<RibbonGalleryGroup>();
            GalleryGroups.CollectionChanged += new NotifyCollectionChangedEventHandler(GalleryGroups_CollectionChanged);
            GalleryFilters = new ObservableCollection<RibbonGalleryFilter>();
            GalleryFilters.CollectionChanged += new NotifyCollectionChangedEventHandler(GalleryFilters_CollectionChanged);
            
            MenuItems = new ObservableCollection<object>();
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether [menu icon bar enabled].
        /// </summary>
        /// <value><c>true</c> if [menu icon bar enabled]; otherwise, <c>false</c>.</value>
        public bool MenuIconBarEnabled
        {
            get
            {
                return (bool)GetValue(MenuIconBarEnabledProperty);
            }

            set
            {
                SetValue(MenuIconBarEnabledProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has filters.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has filters; otherwise, <c>false</c>.
        /// </value>
        public bool HasFilters
        {
            get
            {
                return (bool)GetValue(HasFiltersProperty);
            }

            protected set
            {
                SetValue(HasFiltersPropertyKey, value);
            }
        }

        /// <summary>
        /// Gets the current filter.
        /// </summary>
        /// <value>The current filter.</value>
        public RibbonGalleryFilter CurrentFilter
        {
            get
            {
                return (RibbonGalleryFilter)GetValue(CurrentFilterProperty);
            }

            internal set
            {
                SetValue(CurrentFilterProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the gallery groups.
        /// </summary>
        /// <value>The gallery groups.</value>
        public ObservableCollection<RibbonGalleryGroup> GalleryGroups
        {
            get
            {
                return (ObservableCollection<RibbonGalleryGroup>)GetValue(GalleryGroupsProperty);
            }

            set
            {
                SetValue(GalleryGroupsProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the width of the expand.
        /// </summary>
        /// <value>The width of the expand.</value>
        public double ExpandWidth
        {
            get
            {
                return (double)GetValue(ExpandWidthProperty);
            }

            set
            {
                SetValue(ExpandWidthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the height of the expand.
        /// </summary>
        /// <value>The height of the expand.</value>
        public double ExpandHeight
        {
            get
            {
                return (double)GetValue(ExpandHeightProperty);
            }

            set
            {
                SetValue(ExpandHeightProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the width of the item.
        /// </summary>
        /// <value>The width of the item.</value>
        public int ItemWidth
        {
            get
            {
                return (int)GetValue(ItemWidthProperty);
            }

            set
            {
                SetValue(ItemWidthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the height of the item.
        /// </summary>
        /// <value>The height of the item.</value>
        public int ItemHeight
        {
            get
            {
                return (int)GetValue(ItemHeightProperty);
            }

            set
            {
                SetValue(ItemHeightProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the resize direction.
        /// </summary>
        /// <value>The resize direction.</value>
        public ResizeDirection ResizeDirection
        {
            get
            {
                return (ResizeDirection)GetValue(ResizeDirectionProperty);
            }

            set
            {
                SetValue(ResizeDirectionProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the popup position.
        /// </summary>
        /// <value>The popup position.</value>
        public PopupPosition PopupPosition
        {
            get
            {
                return (PopupPosition)GetValue(PopupPositionProperty);
            }

            protected set
            {
                SetValue(PopupPositionPropertyKey, value);
            }
        }

        /// <summary>
        /// Gets or sets the gallery filters.
        /// </summary>
        /// <value>The gallery filters.</value>
        public ObservableCollection<RibbonGalleryFilter> GalleryFilters
        {
            get
            {
                return (ObservableCollection<RibbonGalleryFilter>)GetValue(GalleryFiltersProperty);
            }

            set
            {
                SetValue(GalleryFiltersProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has groups.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has groups; otherwise, <c>false</c>.
        /// </value>
        public bool HasGroups
        {
            get
            {
                return (bool)GetValue(HasGroupsProperty);
            }

            protected set
            {
                SetValue(HasGroupsPropertyKey, value);
            }
        }

        /// <summary>
        /// Gets or sets the menu items.
        /// </summary>
        /// <value>The menu items.</value>
        public ObservableCollection<object> MenuItems
        {
            get
            {
                return (ObservableCollection<object>)GetValue(MenuItemsProperty);
            }

            set
            {
                SetValue(MenuItemsProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the selected item.
        /// </summary>
        /// <value>The selected item.</value>
        public object SelectedItem
        {
            get
            {
                return (object)GetValue(SelectedItemProperty);
            }

            set
            {
                SetValue(SelectedItemProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the visual mode.
        /// </summary>
        /// <value>The visual mode.</value>
        public RibbonGalleryVisualMode VisualMode
        {
            get
            {
                return (RibbonGalleryVisualMode)GetValue(VisualModeProperty);
            }

            set
            {
                SetValue(VisualModeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the corner radius for item mouse over border.
        /// </summary>
        /// <value>The visual mode.</value>
        public CornerRadius ItemMouseOverBorderCornerRadius
        {
            get
            {
                return (CornerRadius)GetValue(ItemMouseOverBorderCornerRadiusProperty);
            }

            set
            {
                SetValue(ItemMouseOverBorderCornerRadiusProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the thickness for item mouse over border.
        /// </summary>
        /// <value>The visual mode.</value>
        public double ItemMouseOverBorderThickness
        {
            get
            {
                return (double)GetValue(ItemMouseOverBorderThicknessProperty);
            }

            set
            {
                SetValue(ItemMouseOverBorderThicknessProperty, value);
            }
        }

        #endregion

        #region Events
        /// <summary>
        /// Event that is raised when ItemWidth property is changed.
        /// </summary>
        public event PropertyChangedCallback ItemWidthChanged;

        /// <summary>
        /// Event that is raised when ItemHeight property is changed.
        /// </summary>
        public event PropertyChangedCallback ItemHeightChanged;

        /// <summary>
        /// Event that is raised when ResizeDirection property is changed.
        /// </summary>
        public event PropertyChangedCallback ResizeDirectionChanged;

        /// <summary>
        /// Event that is raised when ExpandWidth property is changed.
        /// </summary>
        public event PropertyChangedCallback ExpandWidthChanged;

        /// <summary>
        /// Event that is raised when ExpandHeight property is changed.
        /// </summary>
        public event PropertyChangedCallback ExpandHeightChanged;

        /// <summary>
        /// Event that is raised when PopupPosition property is changed.
        /// </summary>
        public event PropertyChangedCallback PopupPositionChanged;

        /// <summary>
        /// Event that is raised when HasGroups property is changed.
        /// </summary>
        public event PropertyChangedCallback HasGroupsChanged;

        /// <summary>
        /// Event that is raised when HasFilters property is changed.
        /// </summary>
        public event PropertyChangedCallback HasFiltersChanged;

        /// <summary>
        /// Event that is raised when CurrentFilter property is changed.
        /// </summary>
        public event PropertyChangedCallback CurrentFilterChanged;

        /// <summary>
        /// Event that is raised when SelectedItem property is changed.
        /// </summary>
        public event PropertyChangedCallback SelectedItemChanged;

        /// <summary>
        /// Event that is raised when VisualMode property is changed.
        /// </summary>
        public event PropertyChangedCallback VisualModeChanged;

        #endregion

        #region Dependency properties
        /// <summary>
        /// Defines the gallery item mouse over border thickness.  This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemMouseOverBorderThicknessProperty =
            DependencyProperty.Register("ItemMouseOverBorderThickness", typeof(double), typeof(RibbonGallery), new FrameworkPropertyMetadata(4d));

        /// <summary>
        /// Defines the gallery item mouse over border corner radius.  This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemMouseOverBorderCornerRadiusProperty =
            DependencyProperty.Register("ItemMouseOverBorderCornerRadius", typeof(CornerRadius), typeof(RibbonGallery), new FrameworkPropertyMetadata(new CornerRadius(2)));

        /// <summary>
        /// Defines the gallery item width.  This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemWidthProperty =
            DependencyProperty.Register("ItemWidth", typeof(int), typeof(RibbonGallery), new FrameworkPropertyMetadata(60, new PropertyChangedCallback(OnItemWidthChanged)));

        /// <summary>
        /// Defines the gallery item height.  This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemHeightProperty =
            DependencyProperty.Register("ItemHeight", typeof(int), typeof(RibbonGallery), new FrameworkPropertyMetadata(60, new PropertyChangedCallback(OnItemHeightChanged)));

        /// <summary>
        /// Defines resize direction of the popup gallery.  This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty ResizeDirectionProperty =
            DependencyProperty.Register("ResizeDirection", typeof(ResizeDirection), typeof(RibbonGallery), new FrameworkPropertyMetadata(ResizeDirection.HorizontalAndVertical, new PropertyChangedCallback(OnResizeDirectionChanged)));

        /// <summary>
        /// Defines the expand width of the popup gallery.  This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty ExpandWidthProperty =
            DependencyProperty.Register("ExpandWidth", typeof(double), typeof(RibbonGallery), new FrameworkPropertyMetadata(250d, new PropertyChangedCallback(OnExpandWidthChanged)));

        /// <summary>
        /// Defines the expand height of the popup gallery.  This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty ExpandHeightProperty =
            DependencyProperty.Register("ExpandHeight", typeof(double), typeof(RibbonGallery), new FrameworkPropertyMetadata(250d, new PropertyChangedCallback(OnExpandHeightChanged)));

        /// <summary>
        /// Defines the the popup position.  This is a dependency property key.
        /// </summary>
        protected static readonly DependencyPropertyKey PopupPositionPropertyKey =
            DependencyProperty.RegisterReadOnly("PopupPosition", typeof(PopupPosition), typeof(RibbonGallery), new FrameworkPropertyMetadata(PopupPosition.Below, new PropertyChangedCallback(OnPopupPositionChanged)));

        /// <summary>
        /// Defines the the popup position.  This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty PopupPositionProperty = PopupPositionPropertyKey.DependencyProperty;

        /// <summary>
        /// Defines collection of ribbon gallery groups.  This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty GalleryGroupsProperty =
            DependencyProperty.Register("GalleryGroups", typeof(ObservableCollection<RibbonGalleryGroup>), typeof(RibbonGallery), new UIPropertyMetadata(null));

        /// <summary>
        /// Defines whether the gallery has groups.  This is a dependency property key.
        /// </summary>
        protected static readonly DependencyPropertyKey HasGroupsPropertyKey =
            DependencyProperty.RegisterReadOnly("HasGroups", typeof(bool), typeof(RibbonGallery), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnHasGroupsChanged)));

        /// <summary>
        /// Defines whether the gallery has groups.  This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty HasGroupsProperty = HasGroupsPropertyKey.DependencyProperty;

        /// <summary>
        /// Defines the collection of gallery menu items.  This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty MenuItemsProperty =
            DependencyProperty.Register("MenuItems", typeof(ObservableCollection<object>), typeof(RibbonGallery), new UIPropertyMetadata(null));

        /// <summary>
        /// Defines whether menu icon bar is enabled.  This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty MenuIconBarEnabledProperty =
            DependencyProperty.Register("MenuIconBarEnabled", typeof(bool), typeof(RibbonGallery), new UIPropertyMetadata(false));

        /// <summary>
        /// Defines gallery filter indexes.  This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty FilterIndexesProperty =
            DependencyProperty.Register("FilterIndexes", typeof(Int32Collection), typeof(RibbonGallery), new FrameworkPropertyMetadata(new Int32Collection(), new PropertyChangedCallback(OnFilterIndexesChanged)));

        /// <summary>
        /// Defines the collection of gallery filters.  This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty GalleryFiltersProperty =
            DependencyProperty.Register("GalleryFilters", typeof(ObservableCollection<RibbonGalleryFilter>), typeof(RibbonGallery), new UIPropertyMetadata(null));

        /// <summary>
        /// Defines current gallery filter.  This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty CurrentFilterProperty =
            DependencyProperty.Register("CurrentFilter", typeof(RibbonGalleryFilter), typeof(RibbonGallery), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnCurrentFilterChanged)));

        /// <summary>
        /// Defines whether gallery control has gallery filters.  This is a dependency property key.
        /// </summary>
        protected static readonly DependencyPropertyKey HasFiltersPropertyKey =
            DependencyProperty.RegisterReadOnly("HasFilters", typeof(bool), typeof(RibbonGallery), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnHasFiltersChanged)));

        /// <summary>
        /// Defines whether gallery control has gallery filters.  This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty HasFiltersProperty = HasFiltersPropertyKey.DependencyProperty;

        /// <summary>
        /// Defines the gallery visual mode.  This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty VisualModeProperty =
            DependencyProperty.Register("VisualMode", typeof(RibbonGalleryVisualMode), typeof(RibbonGallery), new FrameworkPropertyMetadata(RibbonGalleryVisualMode.InRibbon, FrameworkPropertyMetadataOptions.AffectsParentMeasure | FrameworkPropertyMetadataOptions.AffectsParentArrange | FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnVisualModeChanged)));

        /// <summary>
        /// Defines the selected gallery item.  This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(object), typeof(RibbonGallery), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnSelectedItemChanged)));

        /// <summary>
        /// Gets the value of the FilterIndexes property for a given element.
        /// </summary>
        /// <param name="obj">The element for which to retrieve the FilterIndexes value.</param>
        /// <returns>The Filter Index property</returns>
        public static Int32Collection GetFilterIndexes(DependencyObject obj)
        {
            return (Int32Collection)obj.GetValue(FilterIndexesProperty);
        }

        /// <summary>
        /// Sets the value of the FilterIndexes property for a given element.
        /// </summary>
        /// <param name="obj">The element on which to apply the property value.</param>
        /// <param name="value">FilterIndexes value</param>
        public static void SetFilterIndexes(DependencyObject obj, Int32Collection value)
        {
            obj.SetValue(FilterIndexesProperty, value);
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Calls OnItemWidthChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnItemWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RibbonGallery instance = (RibbonGallery)d;
            instance.OnItemWidthChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises ItemWidthChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnItemWidthChanged(DependencyPropertyChangedEventArgs e)
        {
            if (ItemWidthChanged != null)
            {
                ItemWidthChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnItemHeightChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnItemHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RibbonGallery instance = (RibbonGallery)d;
            instance.OnItemHeightChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises ItemHeightChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnItemHeightChanged(DependencyPropertyChangedEventArgs e)
        {
            if (ItemHeightChanged != null)
            {
                ItemHeightChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnResizeDirectionChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnResizeDirectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RibbonGallery instance = (RibbonGallery)d;
            instance.OnResizeDirectionChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises ResizeDirectionChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnResizeDirectionChanged(DependencyPropertyChangedEventArgs e)
        {
            if (ResizeDirectionChanged != null)
            {
                ResizeDirectionChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnExpandWidthChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnExpandWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RibbonGallery instance = (RibbonGallery)d;
            instance.OnExpandWidthChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises ExpandWidthChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnExpandWidthChanged(DependencyPropertyChangedEventArgs e)
        {
            if (ExpandWidthChanged != null)
            {
                ExpandWidthChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnExpandHeightChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnExpandHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RibbonGallery instance = (RibbonGallery)d;
            instance.OnExpandHeightChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises ExpandHeightChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnExpandHeightChanged(DependencyPropertyChangedEventArgs e)
        {
            if (ExpandHeightChanged != null)
            {
                ExpandHeightChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnPopupPositionChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnPopupPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

            RibbonGallery instance = (RibbonGallery)d;
            instance.OnPopupPositionChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises PopupPositionChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnPopupPositionChanged(DependencyPropertyChangedEventArgs e)
        {

            if (PopupPositionChanged != null)
            {
                PopupPositionChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnHasGroupsChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnHasGroupsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RibbonGallery instance = (RibbonGallery)d;
            instance.OnHasGroupsChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises HasGroupsChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnHasGroupsChanged(DependencyPropertyChangedEventArgs e)
        {
            if (HasGroupsChanged != null)
            {
                HasGroupsChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnFilterIndexesChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnFilterIndexesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // Some code can be used to process property change
        }

        /// <summary>
        /// Calls OnCurrentFilterChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnCurrentFilterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RibbonGallery instance = (RibbonGallery)d;
            instance.OnCurrentFilterChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises CurrentFilterChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnCurrentFilterChanged(DependencyPropertyChangedEventArgs e)
        {
            RibbonGalleryFilter filter = e.NewValue as RibbonGalleryFilter;


            if (filter != null)
            {
                Items.Filter = new Predicate<object>(FilterItems);
            }

            if (m_popupItemsControl != null)
            {
                m_popupItemsControl.ItemsSource = CloneRibbonGalleryItem();

                UnCheckGallaryItems();
                UncheckPopupGalleryItem();
                SelectedItem = null;
            }

            if (m_scrollPresenter != null)
            {
                m_scrollPresenter.BeginAnimation(InRibbonItemsPresenter.ChildVerticalOffsetProperty, null);
            }

            if (CurrentFilterChanged != null)
            {
                CurrentFilterChanged(this, e);
            }
        }

        /// <summary>
        /// Filters the items.
        /// </summary>
        /// <param name="item">The item value.</param>
        /// <returns>Return Results</returns>
        private bool FilterItems(object item)
        {
            RibbonGalleryFilter filter = CurrentFilter;
            int filterIndex = GalleryFilters.IndexOf(CurrentFilter);

            foreach (RibbonGalleryGroup group in GalleryGroups)
            {
                Int32Collection c = GetFilterIndexes(group);
                if (c.Contains(filterIndex))
                {
                    bool result = group.Items.Contains(item);

                    if (result)
                    {
                        return result;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Calls OnHasFiltersChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnHasFiltersChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RibbonGallery instance = (RibbonGallery)d;
            instance.OnHasFiltersChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises HasFiltersChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnHasFiltersChanged(DependencyPropertyChangedEventArgs e)
        {
            if (HasFiltersChanged != null)
            {
                HasFiltersChanged(this, e);
            }
        }

        /// <summary>
        /// Handles the CollectionChanged event of the GalleryFilters control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        private void GalleryFilters_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (GalleryFilters.Count > 0)
            {
                HasFilters = true;
                if (CurrentFilter == null)
                {
                    CurrentFilter = GalleryFilters[0];
                }
            }
            else
            {
                HasFilters = false;
                CurrentFilter = null;
            }

        }

        /// <summary>
        /// Handles the CollectionChanged event of the GalleryGroups control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        private void GalleryGroups_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ObservableCollection<RibbonGalleryGroup> groupCollection = sender as ObservableCollection<RibbonGalleryGroup>;

            int index = -1;

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:

                    index = groupCollection.Count - 1;

                    RibbonGalleryGroup group = null;

                    if (index == e.NewStartingIndex)
                    {
                        group = groupCollection[index];
                        AddGroupItems(group);
                    }
                    else
                    {
                        index = e.NewStartingIndex;
                        group = e.NewItems[0] as RibbonGalleryGroup;

                        if (group.Items.Count > 0)
                        {
                            int insertItemIndex = Items.IndexOf(GalleryGroups[index + 1].Items[0]);

                            foreach (object item in group.Items)
                            {
                                Items.Insert(insertItemIndex, item);
                                insertItemIndex++;
                            }
                        }
                    }

                    group.Items.CollectionChanged += new NotifyCollectionChangedEventHandler(Group_Items_CollectionChanged);

                    break;

                case NotifyCollectionChangedAction.Move:
                    break;

                case NotifyCollectionChangedAction.Remove:
                    /*RibbonGalleryGroup*/
                    group = e.OldItems[0] as RibbonGalleryGroup;
                    group.Items.CollectionChanged -= Group_Items_CollectionChanged;
                    RemoveGroupItems(group);
                    break;

                case NotifyCollectionChangedAction.Replace:
                    break;

                case NotifyCollectionChangedAction.Reset:
                    break;
            }

            if (GalleryGroups.Count == 0)
            {
                HasGroups = false;
            }
            else
            {
                HasGroups = true;
            }
        }

        /// <summary>
        /// Handles the CollectionChanged event of the Group_Items control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        private void Group_Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ObservableCollection<object> groupCollection = sender as ObservableCollection<object>;
            int count = groupCollection.Count;

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:

                    object obj = e.NewItems[0];

                    RibbonGalleryGroup group = null;
                    foreach (RibbonGalleryGroup galleryGroup in GalleryGroups)
                    {
                        if (galleryGroup.Items.Contains(obj))
                        {
                            group = galleryGroup;
                            break;
                        }
                    }

                    int newStartIndex = e.NewStartingIndex;

                    if (newStartIndex == count - 1)
                    {
                        int groupsCount = GalleryGroups.Count;
                        int groupIndex = GalleryGroups.IndexOf(group);

                        if (groupsCount - 1 == groupIndex)
                        {
                            Items.Add(obj);
                        }
                        else
                        {
                            int groupItemsCount = group.Items.Count;

                            if (groupItemsCount > 1)
                            {
                                object temp = group.Items[groupItemsCount - 2];
                                int insertIndex = Items.IndexOf(temp) + 1;
                                Items.Insert(insertIndex, obj);
                            }
                            else
                            {
                                RibbonGalleryGroup galleryGroup = null;

                                for (int i = groupIndex + 1; i < groupsCount; i++)
                                {
                                    if (GalleryGroups[i].Items.Count != 0)
                                    {
                                        galleryGroup = GalleryGroups[i];
                                        break;
                                    }
                                }

                                if (galleryGroup != null)
                                {
                                    int insertIndex = Items.IndexOf(galleryGroup.Items[0]);
                                    Items.Insert(insertIndex, obj);
                                }
                                else
                                {
                                    Items.Add(obj);
                                }
                            }
                        }
                    }
                    else
                    ////Insert
                    {
                        int insertIndex = Items.IndexOf(group.Items[newStartIndex + 1]);
                        Items.Insert(insertIndex, obj);
                    }

                    break;
                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Remove:
                    break;
                case NotifyCollectionChangedAction.Replace:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Removes the group items.
        /// </summary>
        /// <param name="group">The group.</param>
        private void RemoveGroupItems(RibbonGalleryGroup group)
        {
            foreach (object item in group.Items)
            {
                Items.Remove(item);

            }
        }

        /// <summary>
        /// Adds the group items.
        /// </summary>
        /// <param name="group">The group.</param>
        private void AddGroupItems(RibbonGalleryGroup group)
        {
            foreach (object item in group.Items)
            {
                Items.Add(item);

            }
        }

        /// <summary>
        /// Scrolls to selected.
        /// </summary>
        private void ScrollToSelected()
        {
            RibbonGalleryItem item = GetGalleryItem(SelectedItem);

            if (item != null)
            {

                double panelWidth = m_scrollPresenter.PanelActualWidth;
                double panelHeight = m_scrollPresenter.PanelActualHeight;

                double itemWidth = item.Width;
                double itemHeight = item.Height;

                if (double.IsNaN(itemHeight) || double.IsNaN(ItemWidth))
                    return;
                
                int horizontalCount = (int)(panelWidth / itemWidth);
                //int index = Items.IndexOf(item);
                int index =  m_popupItemsControl.Items.IndexOf(item);
                index = index / horizontalCount;

                m_scrollPresenter.BeginAnimation(InRibbonItemsPresenter.ChildVerticalOffsetProperty, null);

                m_scrollPresenter.ChildVerticalOffset = -index * itemHeight;

            }
          
        }

        /// <summary>
        /// Clones the ribbon gallery item.
        /// </summary>
        /// <returns></returns>
        private ObservableCollection<RibbonGalleryItem> CloneRibbonGalleryItem()
        {

            ObservableCollection<RibbonGalleryItem> clone = new ObservableCollection<RibbonGalleryItem>();

            foreach (object obj in Items)
            {
              UIElement item = GetGalleryItem(obj);
              clone.Add(CloneBasic(item) as RibbonGalleryItem);
            }
        
            return clone;
        }

        /// <summary>
        /// Clones the basic.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        private UIElement CloneBasic(UIElement element)
        {
            StringReader strReader = new StringReader(XamlWriter.Save(element));
            XmlReader xmlReader = XmlReader.Create(strReader);

            UIElement cloned = XamlReader.Load(xmlReader) as UIElement;


            RibbonGalleryItem clonedItem = cloned as RibbonGalleryItem;
            if (clonedItem != null && element is FrameworkElement)
            {
                clonedItem.ToolTip = (element as FrameworkElement).ToolTip;
                clonedItem.Height = element.RenderSize.Height;
                clonedItem.Width = element.RenderSize.Width;

                CloneManager.CloneEventHandler(element, cloned, RibbonGalleryItem.ClickEvent);
                BindingUtils.SetBinding(cloned,element, RibbonGalleryItem.IsCheckedProperty, RibbonGalleryItem.IsCheckedProperty, BindingMode.TwoWay);
                BindingUtils.SetBinding(cloned, element, RibbonGalleryItem.CommandProperty, RibbonGalleryItem.CommandProperty, BindingMode.TwoWay);
                BindingUtils.SetBinding(cloned, element, RibbonGalleryItem.CommandParameterProperty, RibbonGalleryItem.CommandParameterProperty, BindingMode.TwoWay);
                BindingUtils.SetBinding(cloned, element, RibbonGalleryItem.CommandTargetProperty, RibbonGalleryItem.CommandTargetProperty, BindingMode.TwoWay);

                if (clonedItem.IsChecked)
                {
                    clonedItem.IsChecked = false;
                    clonedItem.ApplyTemplate();
                    clonedItem.IsChecked = true;
                }
                

                //BindingUtils.SetBinding(element, cloned, RibbonGalleryItem.HeightProperty, RibbonGalleryItem.HeightProperty, BindingMode.TwoWay);
                //BindingUtils.SetBinding(element, cloned, RibbonGalleryItem.WidthProperty, RibbonGalleryItem.WidthProperty, BindingMode.TwoWay);
                //BindingUtils.SetBinding(element, cloned, RibbonGalleryItem., RibbonGalleryItem.WidthProperty, BindingMode.TwoWay);
            }

            //Syncfusion.Windows.Shared.DictionaryList styleList = SkinStorage.GetVisualStylesList(element);
            //if (styleList != null)
            //{
            //    RibbonSkinObjectExtension ext = new RibbonSkinObjectExtension();
            //    SkinStorage.SetVisualStylesList(cloned, styleList);
            //}


            return cloned as UIElement;
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Determines if the specified item is (or is eligible to be) its own container.
        /// </summary>
        /// <param name="item">The item to check.</param>
        /// <returns>
        /// true if the item is (or is eligible to be) its own container; otherwise, false.
        /// </returns>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is RibbonGalleryItem;
        }

        /// <summary>
        /// Creates or identifies the element used to display the specified item.
        /// </summary>
        /// <returns>Return RibbonGalleryItem</returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new RibbonGalleryItem();
        }

        /// <summary>
        /// Called when ApplyTemplate is called.
        /// </summary>    
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();


            m_scrollPresenter = GetTemplateChild("PART_ScrollPresenter") as /*InRibbonScrollDecorator; //*/InRibbonItemsPresenter;

            m_scrollUpButton = GetTemplateChild("PART_ScrollUpButton") as Button;

            m_scrollDownButton = GetTemplateChild("PART_ScrollDownButton") as Button;

            m_popupItemsControl = GetTemplateChild("PART_PopupItemsControl") as ItemsControl;

            m_mainItemsControl = GetTemplateChild("PART_MainItemsControl") as ItemsControl;            

            if (Popup != null)
            {
                Popup.CustomPopupPlacementCallback = new CustomPopupPlacementCallback(PlacePopup);
            }
            if (m_scrollUpButton != null)
            {
                m_scrollUpButton.Click -= new RoutedEventHandler(ScrollUpButton_Click);
            }

            if (m_scrollDownButton != null)
            {
                m_scrollDownButton.Click -= new RoutedEventHandler(ScrollDownButton_Click);
            }

            if (m_scrollUpButton != null)
            {
                m_scrollUpButton.Click += new RoutedEventHandler(ScrollUpButton_Click);
            }

            if (m_scrollDownButton != null)
            {
                m_scrollDownButton.Click += new RoutedEventHandler(ScrollDownButton_Click);
            }

            if (VisualMode == RibbonGalleryVisualMode.InRibbon)
            {
                InvalidateAnimations();
            }



            RibbonGalleryFilter filter = CurrentFilter;
            if (filter != null)
            {
                Items.Filter = new Predicate<object>(FilterItems);
            }
            this.Loaded -= new RoutedEventHandler(RibbonGallery_Loaded);
            this.Unloaded -= new RoutedEventHandler(RibbonGallery_Unloaded);
            this.Loaded += new RoutedEventHandler(RibbonGallery_Loaded);
            this.Unloaded += new RoutedEventHandler(RibbonGallery_Unloaded);
        }

        void RibbonGallery_Unloaded(object sender, RoutedEventArgs e)
        {
            Dispose();
        }

        /// <summary>
        /// Handles the Loaded event of the RibbonGallery control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void RibbonGallery_Loaded(object sender, RoutedEventArgs e)
        {
            if (m_scrollUpButton != null)
            {
                m_scrollUpButton.Click -= new RoutedEventHandler(ScrollUpButton_Click);
            }

            if (m_scrollDownButton != null)
            {
                m_scrollDownButton.Click -= new RoutedEventHandler(ScrollDownButton_Click);
            }
            if (m_scrollUpButton != null)
            {
                m_scrollUpButton.Click += new RoutedEventHandler(ScrollUpButton_Click);
            }

            if (m_scrollDownButton != null)
            {
                m_scrollDownButton.Click += new RoutedEventHandler(ScrollDownButton_Click);
            }
            if (m_scrollPresenter != null)
            {
                if (!Double.IsNaN(m_scrollPresenter.PanelActualHeight))
                {
                    if (m_scrollPresenter.PanelActualHeight > this.ActualHeight)
                    {
                        m_scrollDownButton.IsEnabled = true;
                    }
                }
            }
        }

        /// <summary>
        /// Handles the Closed event of the Popup control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected override void Popup_Closed(object sender, EventArgs e)
        {
            FireAfterDropDownPopup();
            InvalidateMeasure();
            InvalidateArrange();

            if (VisualMode == RibbonGalleryVisualMode.InRibbon)
            {
                ScrollToSelected();
            }

            if (m_scrollPresenter != null)
            {
                InvalidateAnimations();
            }
        }

        protected override void Popup_Opened(object sender, EventArgs e)
        {
#if !SyncfusionFramework3_5

            PART_ScrollViewer = GetTemplateChild("PART_ScrollViewer") as ScrollViewer;

            if (PART_ScrollViewer != null)
            {
                if (SkinStorage.GetEnableTouch(this))
                    PART_ScrollViewer.PanningMode = PanningMode.VerticalOnly;
                else
                    PART_ScrollViewer.PanningMode = PanningMode.None;
            }
            
#endif
            CheckSelectedItem();
        }

        private void CheckSelectedItem()
        {
            if (this.ItemsSource != null)
            {
                RibbonGalleryItem item = this.GetGalleryItem(SelectedItem);
                if (item != null && item.CheckOnClick)
                {
                    item.IsChecked = true;
                }
            }
        }
       

        /// <summary>
        /// Called when Items collection is changed.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);

            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                bool prevItemChecked = false;
                RibbonGalleryItem item = GetGalleryItem(e.NewItems[e.NewItems.Count - 1]);

                if (item == null)
                    return;
                if (item.IsChecked)
                    prevItemChecked = true;
                UnCheckGallaryItems();

                item.ApplyTemplate();
                if (e.NewItems.Count > 0 && prevItemChecked)
                {
                    item.IsChecked = true;
                    SelectedItem = item;
                }

                InvalidateAnimations();
 
            }
        }

        /// <summary>
        /// Provides class handling for the MouseRightButtonUp routed event that occurs
        /// when the Right mouse button is released while the mouse pointer is over this control.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseRightButtonUp(MouseButtonEventArgs e)
        {
            var ribbonTouch = VisualUtils.FindAncestor(this, typeof(Ribbon)) as Ribbon;
            if (e.StylusDevice == null || (ribbonTouch!=null && !ribbonTouch.EnableTouch))
            {
                base.OnMouseRightButtonUp(e);
                UpdateBorder();
            }
        }        

        protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            var ribbonTouch = VisualUtils.FindAncestor(this, typeof(Ribbon)) as Ribbon;
            if (e.StylusDevice == null || (ribbonTouch!=null && !ribbonTouch.EnableTouch))
            {
                Visual originalSource = e.OriginalSource as Visual;

                if (originalSource != null)
                {
                    RibbonGalleryItem item = null;

                    if (originalSource is RibbonGalleryItem)
                    {
                        item = originalSource as RibbonGalleryItem;
                    }
                    else
                    {
                        item = VisualUtils.FindAncestor(originalSource, typeof(RibbonGalleryItem)) as RibbonGalleryItem;
                    }

                    if (item != null)
                    {
                        object newSelectedItem = null;

                        newSelectedItem = item;
                        //if (Items.Contains(item))
                        //{
                        //    newSelectedItem = item;
                        //}
                        //else
                        //{
                        //    newSelectedItem = item.Content;
                        //}

                        object oldSelectedItem = SelectedItem;

                        if (oldSelectedItem != newSelectedItem)
                        {
                            //CheckGalleryItem(oldSelectedItem, false);

                            CheckGalleryItem(newSelectedItem, true);

                            RibbonGalleryItem gItem = GetGalleryItem(newSelectedItem);
                            if (this.ItemsSource != null)
                                SelectedItem = gItem.DataContext;
                            else
                                SelectedItem = gItem;
                        }
                    }
                }

                base.OnPreviewMouseLeftButtonUp(e);
            }
        }

         #if !SyncfusionFramework3_5
        protected override void OnPreviewTouchUp(TouchEventArgs e)
        {
            var ribbonTouch = VisualUtils.FindAncestor(this, typeof(Ribbon)) as Ribbon;
            if (ribbonTouch != null && ribbonTouch.EnableTouch && m_systemGesture == SystemGesture.Tap)
            {
                Visual originalSource = e.OriginalSource as Visual;

                if (originalSource != null)
                {
                    RibbonGalleryItem item = null;

                    if (originalSource is RibbonGalleryItem)
                    {
                        item = originalSource as RibbonGalleryItem;
                    }
                    else
                    {
                        item = VisualUtils.FindAncestor(originalSource, typeof(RibbonGalleryItem)) as RibbonGalleryItem;
                    }

                    if (item != null)
                    {
                        object newSelectedItem = null;

                        newSelectedItem = item;
                        //if (Items.Contains(item))
                        //{
                        //    newSelectedItem = item;
                        //}
                        //else
                        //{
                        //    newSelectedItem = item.Content;
                        //}

                        object oldSelectedItem = SelectedItem;

                        if (oldSelectedItem != newSelectedItem)
                        {
                            //CheckGalleryItem(oldSelectedItem, false);

                            CheckGalleryItem(newSelectedItem, true);

                            RibbonGalleryItem gItem = GetGalleryItem(newSelectedItem);
                            if (this.ItemsSource != null)
                                SelectedItem = gItem.DataContext;
                            else
                                SelectedItem = gItem;
                        }
                    }
                }

                base.OnPreviewTouchUp(e);
            }
        }
        #endif
        /// <summary>
        /// Provides class handling for the MouseLeftButtonUp routed event that occurs
        /// when the left mouse button is released while the mouse pointer is over this control.
        /// Invoked when an unhandled <see cref="E:System.Windows.UIElement.MouseLeftButtonUp"/>�routed event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs"/> that contains the event data. The event data reports that the left mouse button was released.</param>
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            var ribbonTouch = VisualUtils.FindAncestor(this, typeof(Ribbon)) as Ribbon;
            if (e.StylusDevice == null || (ribbonTouch!=null && !ribbonTouch.EnableTouch))
            {
                base.OnMouseLeftButtonUp(e);
            }
        }

        #if !SyncfusionFramework3_5
        protected override void OnTouchUp(TouchEventArgs e)
        {
            var ribbonTouch = VisualUtils.FindAncestor(this, typeof(Ribbon)) as Ribbon;
            if (ribbonTouch!=null && ribbonTouch.EnableTouch && m_systemGesture == SystemGesture.Tap)
            {
                base.OnTouchUp(e);
            }
            if (ribbonTouch != null && ribbonTouch.EnableTouch && m_systemGesture == SystemGesture.RightTap)
            {
                base.OnTouchUp(e);
                UpdateBorder();
            }
        }
        #endif

        protected override void OnStylusSystemGesture(StylusSystemGestureEventArgs e)
        {
            m_systemGesture = e.SystemGesture;
            base.OnStylusSystemGesture(e);
        }

        /// <summary>
        /// Gets the gallery item.
        /// </summary>
        /// <param name="obj">The obj param value.</param>
        /// <returns>Returns Item</returns>
        private RibbonGalleryItem GetGalleryItem(object obj)
        {
            RibbonGalleryItem item = null;

            if (obj is RibbonGalleryItem)
            {
                if ((obj as RibbonGalleryItem).Content != null && (obj as RibbonGalleryItem).Content.ToString() == "{DisconnectedItem}")
                {
                    int i = m_popupItemsControl == null ? -1 : m_popupItemsControl.Items.IndexOf(obj);

                    if(i != -1)
                        return ItemContainerGenerator.ContainerFromIndex(i) as RibbonGalleryItem;
                }
                item = obj as RibbonGalleryItem;
            }
            else
            {
                item = ItemContainerGenerator.ContainerFromItem(obj) as RibbonGalleryItem;
            }

            return item;
        }

        /// <summary>
        /// Checks the gallery item.
        /// </summary>
        /// <param name="obj">The obj param value.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        private void CheckGalleryItem(object obj, bool value)
        {
            //Removes the previous selection.
            UnCheckGallaryItems();

            RibbonGalleryItem item = GetGalleryItem(obj);
            if (item != null && item.CheckOnClick)
            {
                item.ApplyTemplate();
                item.IsChecked = value;
            }
        }

        /// <summary>
        /// Uns the check gallary items.
        /// </summary>
        private void UnCheckGallaryItems()
        {
            foreach (object items in Items)
            {
                RibbonGalleryItem gallaryItem = GetGalleryItem(items);
                if (gallaryItem.IsChecked)
                    gallaryItem.IsChecked = false;
            }
        }

        /// <summary>
        /// Unchecks the popup gallery item.
        /// </summary>
        private void UncheckPopupGalleryItem()
        {
            RibbonGalleryItem item = GetGalleryItem(SelectedItem);
            foreach (object items in m_popupItemsControl.Items)
            {
                
                RibbonGalleryItem gallaryItem = GetGalleryItem(items);
                if (item == gallaryItem)
                   gallaryItem.IsChecked = true;
                else if (gallaryItem.IsChecked)
                    gallaryItem.IsChecked = false;
            }
        }

        /// <summary>
        /// Runs down animation.
        /// </summary>
        /// <param name="speed">The speed.</param>
        private void RunDownAnimation(double speed)
        {
            double currentValue = m_scrollPresenter.ChildVerticalOffset;
            double itemHeight = ItemHeight;

            if (m_currentDirection != Direction.Down)
            {
                double temp = Math.Abs(currentValue) % itemHeight;
                if (temp > 0)
                {
                    itemHeight = temp;
                }
            }

            if (m_downAnimation != null)
            {
                m_downAnimation.Completed -= DownAnimation_Completed;
            }

            m_downAnimation = new DoubleAnimation();
            m_downAnimation.Completed += DownAnimation_Completed;
            m_downAnimation.SpeedRatio = speed;

            m_downAnimation.To = currentValue + itemHeight;
            m_downAnimation.Duration = TimeSpan.FromSeconds(0.5);

            m_scrollPresenter.BeginAnimation(InRibbonItemsPresenter.ChildVerticalOffsetProperty, m_downAnimation);

            m_currentDirection = Direction.Down;
        }

        /// <summary>
        /// Runs the up animation.
        /// </summary>
        /// <param name="speed">The speed.</param>
        private void RunUpAnimation(double speed)
        {
            double currentValue = m_scrollPresenter.ChildVerticalOffset;
            double itemHeight = ItemHeight;

            if (m_currentDirection != Direction.Up)
            {
                double temp = Math.Abs(currentValue) % itemHeight;
                if (temp > 0)
                {
                    itemHeight -= temp;
                }
            }

            if (m_upAnimation != null)
            {
                m_upAnimation.Completed -= UpAnimation_Completed;
            }

            m_upAnimation = new DoubleAnimation();
            m_upAnimation.Completed += UpAnimation_Completed;
            m_upAnimation.SpeedRatio = speed;
            m_upAnimation.To = currentValue - itemHeight;
            m_upAnimation.Duration = TimeSpan.FromSeconds(0.5);

            m_scrollPresenter.BeginAnimation(InRibbonItemsPresenter.ChildVerticalOffsetProperty, m_upAnimation);
            m_currentDirection = Direction.Up;
        }

        /// <summary>
        /// Handles the Completed event of the DownAnimation control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void DownAnimation_Completed(object sender, EventArgs e)
        {
            InvalidateAnimations();

            if (m_scrollUpButton.IsPressed && m_scrollUpButton.IsEnabled)
            {
                RunDownAnimation(4);
            }
            else
            {
                m_currentDirection = Direction.None;
            }
        }

        /// <summary>
        /// Handles the Completed event of the UpAnimation control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void UpAnimation_Completed(object sender, EventArgs e)
        {
            InvalidateAnimations();

            if (m_scrollDownButton.IsPressed && m_scrollDownButton.IsEnabled)
            {
                RunUpAnimation(4);
            }
            else
            {
                m_currentDirection = Direction.None;
            }
        }

        /// <summary>
        /// Handles the Click event of the ScrollUpButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void ScrollUpButton_Click(object sender, RoutedEventArgs e)
        {
            InvalidateAnimations();

            if (m_scrollUpButton.IsEnabled && m_currentDirection != Direction.Down)
            {
                RunDownAnimation(2);
            }
            Ribbon rb = VisualUtils.FindSomeParent(this, typeof(Ribbon)) as Ribbon;
            if (rb != null && rb.RibbonState == RibbonState.Adorner)
            {
                bScrollbtnclicked = true;
            }
        }

        /// <summary>
        /// Handles the Click event of the ScrollDownButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void ScrollDownButton_Click(object sender, RoutedEventArgs e)
        {
            InvalidateAnimations();

            if (m_scrollDownButton.IsEnabled && m_currentDirection != Direction.Up)
            {
                RunUpAnimation(2);
            }
            Ribbon rb = VisualUtils.FindSomeParent(this, typeof(Ribbon)) as Ribbon;
            if (rb != null && rb.RibbonState == RibbonState.Adorner)
            {
                bScrollbtnclicked = true;
            }
        }

        /// <summary>
        /// Invalidates the animations.
        /// </summary>
        private void InvalidateAnimations()
        {
            if (m_scrollPresenter == null)
                return;

            double panelActualHeight = /*( m_scrollPresenter.Child as FrameworkElement ).ActualHeight; //*/m_scrollPresenter.PanelActualHeight;

            double verticalOffset = m_scrollPresenter.ChildVerticalOffset; ////VerticalOffset;

            //Debug.WriteLine(verticalOffset);

            if (!Double.IsNaN(panelActualHeight))
            {
                if (Math.Abs(verticalOffset) < panelActualHeight - ItemHeight)
                {
                    m_scrollDownButton.IsEnabled = true;
                }
                else
                {                    
                    m_scrollDownButton.IsEnabled = false;
                }
            }
            else
            {
                m_scrollDownButton.IsEnabled = false;
            }

            if (verticalOffset < 0 && Math.Abs(verticalOffset) != panelActualHeight)
            {
                m_scrollUpButton.IsEnabled = true;
            }
            else
            {
                m_scrollUpButton.IsEnabled = false;
            }
        }

        /// <summary>
        /// Places the popup.
        /// </summary>
        /// <param name="popupSize">Size of the popup.</param>
        /// <param name="targetSize">Size of the target.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>custom pop up placement value</returns>
        private CustomPopupPlacement[] PlacePopup(System.Windows.Size popupSize, System.Windows.Size targetSize, System.Windows.Point offset)
        {
            CustomPopupPlacement[] ttplaces = new CustomPopupPlacement[] { new CustomPopupPlacement() };
            ttplaces[0].PrimaryAxis = PopupPrimaryAxis.None;

            double x = (FlowDirection == FlowDirection.RightToLeft) ? -popupSize.Width : 0d;

            if (PopupPosition == PopupPosition.Below)
            {
                if (VisualMode != RibbonGalleryVisualMode.InRibbon)
                {
                    ttplaces[0].Point = new Point(x, ActualHeight + 1d);
                }

                if (FlowDirection == FlowDirection.RightToLeft && VisualMode == RibbonGalleryVisualMode.InRibbon)
                {
                    ttplaces[0].Point = new Point(x - 1d, 0);
                }

                return ttplaces;
            }

            IntPtr hwndGallery = ((HwndSource)PresentationSource.FromVisual(this)).Handle;
            WindowInterop.RECT galleryRect = new WindowInterop.RECT();
            WindowInterop.GetWindowRect(hwndGallery, ref galleryRect);

            double actualHeight = (VisualMode == RibbonGalleryVisualMode.InRibbon) ? ActualHeight : 0d;
            double y = actualHeight - popupSize.Height;

            ttplaces[0].Point = new Point(x, y);

            return ttplaces;
        }

        /// <summary>
        /// Gets the arranged gallery items.
        /// </summary>
        /// <value>The arranged gallery items.</value>
        private ObservableCollection<RibbonGalleryItem> ArrangedGalleryItems
        {
            get
            {
                m_arrangedItems = new ObservableCollection<RibbonGalleryItem>();
                foreach (object obj in Items)
                {
                    RibbonGalleryItem item = GetGalleryItem(obj);
                    m_arrangedItems.Add(item);
                }
                return m_arrangedItems;
            }
        }

        #endregion

        #region OnChanged handlers

        /// <summary>
        /// Calls OnSelectedItemChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RibbonGallery instance = (RibbonGallery)d;
            instance.OnSelectedItemChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises SelectedItemChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnSelectedItemChanged(DependencyPropertyChangedEventArgs e)
        {
            if (SelectedItemChanged != null)
            {
                SelectedItemChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises IsDropDownOpenChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected override void OnIsDropDownOpenChanged(DependencyPropertyChangedEventArgs e)
        {
            if (IsDropDownOpen == true)
            {
                if (VisualMode == RibbonGalleryVisualMode.InRibbon)
                {
                    if (HasGroups)
                        m_popupItemsControl.ItemsSource = CloneRibbonGalleryItem();
                    else
                        m_popupItemsControl.ItemsSource = ArrangedGalleryItems;
                   
                }
                
                if (Parent != null && Parent is RibbonBar 
                    && (Parent as RibbonBar).PanelState == RibbonBarState.Collapsed && (bool)e.NewValue)
                {
                    (Parent as RibbonBar).ShowPopup();
                }
                
                IntPtr hwndGallery = ((HwndSource)PresentationSource.FromVisual(this)).Handle;
                WindowInterop.RECT galleryRect = new WindowInterop.RECT();
                WindowInterop.GetWindowRect(hwndGallery, ref galleryRect);

                double aboveHeight = galleryRect.Location.Y;
                double belowHeight = SystemParameters.PrimaryScreenHeight - aboveHeight;

                double maxHeight = Math.Max(aboveHeight, belowHeight);

                if (VisualMode == RibbonGalleryVisualMode.InRibbon)
                {
                    Popup.MinWidth = ActualWidth + 10d;
                    Popup.Width = Popup.MinWidth;
                }
                else
                {
                    Popup.MinWidth = ExpandWidth;
                    Popup.Width = ExpandWidth;
                }

                Popup.MaxHeight = maxHeight;
                Popup.Height = (ExpandHeight < maxHeight) ? ExpandHeight : 2d / 3d * maxHeight;

                QuickAccessToolBar qat = VisualUtils.FindSomeParent(this, typeof(QuickAccessToolBar)) as QuickAccessToolBar;
                if (qat != null)
                {
                    Popup.MaxWidth = SystemParameters.PrimaryScreenWidth - this.PointToScreen(new Point()).X;
                    Popup.Width = double.NaN;
                }

                if (aboveHeight > belowHeight)
                {
                    PopupPosition = PopupPosition.Above;
                }
                else
                {
                    PopupPosition = PopupPosition.Below;
                }

                foreach (RibbonGalleryGroup group in GalleryGroups)
                {
                    SkinStorage.SetVisualStyle(group, SkinStorage.GetVisualStyle(this));
                    group.Visibility = Visibility.Visible;
                }

                if (GetGalleryItem(SelectedItem) != null)
                {
                    GetGalleryItem(SelectedItem).BringIntoView();
                }
            
            }
            else
            {
                Items.Refresh();
                UpdateBorder();
                RibbonGalleryItem item = GetGalleryItem(SelectedItem);
                if (item != null && item.CheckOnClick)
                {
                    item.IsChecked = true;
                }

            }

            base.OnIsDropDownOpenChanged(e);
        }

        /// <summary>
        /// Updates the border.
        /// </summary>
        private void UpdateBorder()
        {
            foreach (object items in Items)
            {
                RibbonGalleryItem gallaryItem = GetGalleryItem(items);
                if (gallaryItem.IsChecked)
                {
                    gallaryItem.IsChecked = false;
                    gallaryItem.ApplyTemplate();
                    gallaryItem.IsChecked = true;
                }
               
            }
        }

        /// <summary>
        /// Calls OnVisualModeChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnVisualModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RibbonGallery instance = (RibbonGallery)d;
            instance.OnVisualModeChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises VisualModeChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnVisualModeChanged(DependencyPropertyChangedEventArgs e)
        {
            if (VisualModeChanged != null)
            {
                VisualModeChanged(this, e);
            }
        }

        #endregion


        public void Dispose()
        {
            if (m_scrollUpButton != null)
            {
                m_scrollUpButton.Click -= new RoutedEventHandler(ScrollUpButton_Click);
            }

            if (m_scrollDownButton != null)
            {
                m_scrollDownButton.Click -= new RoutedEventHandler(ScrollDownButton_Click);
            }
            this.Unloaded -= new RoutedEventHandler(RibbonGallery_Unloaded);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.Key == Key.Down || e.Key == Key.Up || e.Key == Key.Right || e.Key == Key.Left)
            {
                if (m_popupItemsControl != null && m_popupItemsControl.Items.Count > 0 && m_popupItemsControl.Items[0] is RibbonGalleryItem)
                    (m_popupItemsControl.Items[0] as RibbonGalleryItem).Focus();
                e.Handled = true;
            }

        }
    }
}
