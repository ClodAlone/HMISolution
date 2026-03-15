// <copyright file="GalleryGroup.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

#define DEBUG
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Diagnostics;
using Syncfusion.Windows.Shared;
using Syncfusion.Licensing;
using System.Windows.Data;
using System.ComponentModel;
using System.Windows.Media;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents control using for grouping.
    /// </summary>
    /// <remarks>
    /// UI framework element based on <see cref="HeaderedItemsControl"/> class. 
    /// Control is used as a host container for items.
    /// </remarks>    
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
    /// <example><code>public partial class GalleryGroup : ContentControl</code></example>
    /// </list>
    /// <para/>
    /// <list type="table">
    /// <listheader>
    /// <description>XAML Object Element Usage</description>
    /// </listheader>
    /// <example><code><![CDATA[<local:GalleryGroup Name="galleryGroup" />]]></code></example>
    /// </list>
    /// </example>
    /// </list>
    /// <example>
    /// <para/>This example shows how to create a GalleryGroup in XAML.
    /// <code>
    /// <![CDATA[
    /// <Window x:Class="Gallery.Window1" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    /// xmlns:local="clr-namespace:Syncfusion.Windows.Tools.Controls;assembly=Syncfusion.Tools.WPF"
    /// Title="Gallery" Height="300" Width="300">
    /// <StackPanel HorizontalAlignment="Center">
    ///     <local:Gallery Name="gallery">
    ///         <local:GalleryGroup Name="group">
    ///             <local:GalleryItem/>
    ///             <local:GalleryItem/>
    ///             <local:GalleryItem/>
    ///         </local:GalleryGroup>
    ///     </local:Gallery>
    /// </StackPanel>
    /// </Window>
    /// ]]>
    /// </code>
    /// <para/>This example shows how to create a GalleryGroup in C#.
    /// <code>
    /// using System.Windows;
    /// using System.Windows.Controls;
    /// namespace Sample1
    /// {
    ///     public partial class Window1 : Window
    ///     {
    ///        public Window1()
    ///        {
    ///             InitializeComponent();
    ///             Gallery gallery = new Gallery();
    ///             stackPanel.Children.Add( gallery );
    ///             GalleryGroup group = new GalleryGroup();
    ///             gallery.Items.Add( group );
    ///         }
    ///     }
    /// }
    /// </code>
    /// </example>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    [SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.Skin.Office2007Blue,
   Type = typeof(GalleryGroup), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/Gallery/Themes/Office2007BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.Skin.Office2007Black,
    Type = typeof(GalleryGroup), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/Gallery/Themes/Office2007BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.Skin.Office2007Silver,
    Type = typeof(GalleryGroup), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/Gallery/Themes/Office2007SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.Skin.Office2003,
     Type = typeof(GalleryGroup), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/Gallery/Themes/Office2003Style.xaml")]
    [SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.Skin.ShinyBlue,
     Type = typeof(GalleryGroup), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/Gallery/Themes/ShinyBlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.Skin.ShinyRed,
     Type = typeof(GalleryGroup), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/Gallery/Themes/ShinyRedStyle.xaml")]
    [SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.Skin.SyncOrange,
     Type = typeof(GalleryGroup), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/Gallery/Themes/SyncOrangeStyle.xaml")]
    [SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.Skin.Blend,
    Type = typeof(GalleryGroup), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/Gallery/Themes/BlendStyle.xaml")]
    [SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.Skin.Default,
    Type = typeof(GalleryGroup), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/Gallery/Themes/Generic.xaml")]
    public class GalleryGroup : HeaderedItemsControl
    {
        #region Constants
        /// <summary>
        /// Defines index of row in matrix.
        /// </summary>
        private const int ROW_MATRIX_INDEX = 0;

        /// <summary>
        /// Defines index of col in matrix.
        /// </summary>
        private const int COL_MATRIX_INDEX = 1;

        /// <summary>
        /// This is format string. 
        /// </summary>
        private const string FORMAT_STRING = "Item";
        #endregion

        #region Private fields
        /// <summary>
        /// Utilized to indicate dragging.
        /// </summary>
        private Point m_startPoint;

        /// <summary>
        /// Utilized for drag-&amp;-drop.
        /// </summary>
        private DataObject m_dataObject;

        /// <summary>
        /// Index of selection start item.
        /// </summary>
        private int m_selectionStartIndex = -1;

        /// <summary>
        /// Index of focused item.
        /// </summary>
        private int m_hasFocusIndex = -1;

        /// <summary>
        /// Flag that is utilized for prevention of double call of
        /// SetGalleryItemIsSelected and SelectedItems_CollectionChanged
        /// methods.
        /// </summary>
        private bool m_skipIt = false;

        /// <summary>
        /// Collection of arranged items.
        /// </summary>
        private ItemsCollection m_arrangedItems;

        /// <summary>
        /// Matrix for navigation.
        /// </summary>
        private GalleryItem[,] m_navigationMatrix;

        /// <summary>
        /// Current row index.
        /// </summary>
        private int m_currentRowIndex = -1;

        /// <summary>
        /// Current column index.
        /// </summary>
        private int m_currentColIndex = -1;

        /// <summary>
        /// Count of rows.
        /// </summary>
        private int m_rowCount = -1;

        /// <summary>
        /// Count of columns.
        /// </summary>
        private int m_colCount = -1;

        /// <summary>
        /// Root container.
        /// </summary>
        private IInputElement m_iinputElement = null;

        /// <summary>
        /// Contains true if MultipleDragging started.
        /// </summary>
        private bool m_isMultipleDragging = false;

        /// <summary>
        /// Contains selected GalleryItem.
        /// </summary>
        private GalleryItem m_selectedGalleryItem = null;

        /// <summary>
        /// Contains true if more than one item was selected.
        /// </summary>
        private bool m_bMultiselectionDone = false;

        /// <summary>
        /// Indicating when tab button was pressed. Using for correct keyboard navigation.
        /// </summary>
        private bool m_tabWasPressed = false;
        #endregion

        #region Initialization
        /// <summary>
        /// Initializes static members of the <see cref="GalleryGroup"/> class.
        /// </summary>
        static GalleryGroup()
        {
            //EnvironmentTest.ValidateLicense(typeof(GalleryGroup));

            DefaultStyleKeyProperty.OverrideMetadata(typeof(GalleryGroup), new System.Windows.FrameworkPropertyMetadata(typeof(GalleryGroup)));

            HorizontalAlignmentProperty.OverrideMetadata(typeof(GalleryGroup), new FrameworkPropertyMetadata(HorizontalAlignment.Left, null, CoerceHorizontalAlignmentValueCallback));
            VerticalAlignmentProperty.OverrideMetadata(typeof(GalleryGroup), new FrameworkPropertyMetadata(VerticalAlignment.Top, null, CoerceVerticalAlignmentValueCallback));
            FocusableProperty.OverrideMetadata(typeof(GalleryGroup), new FrameworkPropertyMetadata(true));
            AllowDropProperty.OverrideMetadata(typeof(GalleryGroup), new FrameworkPropertyMetadata(true));

            Gallery.AllowedAnimationsProperty.AddOwner(typeof(GalleryGroup), new FrameworkPropertyMetadata(AllowedAnimations.All, FrameworkPropertyMetadataOptions.Inherits));
            Gallery.AllowedItemResizeModeProperty.AddOwner(typeof(GalleryGroup), new FrameworkPropertyMetadata(AllowedItemResizeModes.None, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.Inherits, new PropertyChangedCallback(OnAllowedItemResizeModeChanged)));
            Gallery.AllowMultiSelectProperty.AddOwner(typeof(GalleryGroup), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.Inherits, OnAllowMultiSelectChanged));
            Gallery.ItemMarginProperty.AddOwner(typeof(GalleryGroup), new FrameworkPropertyMetadata(new Thickness(0), FrameworkPropertyMetadataOptions.Inherits, new PropertyChangedCallback(OnItemMarginChanged)));
            Gallery.ItemWidthProperty.AddOwner(typeof(GalleryGroup), new FrameworkPropertyMetadata(50d, FrameworkPropertyMetadataOptions.Inherits, new PropertyChangedCallback(OnItemWidthChanged)));
            Gallery.ItemHeightProperty.AddOwner(typeof(GalleryGroup), new FrameworkPropertyMetadata(50d, FrameworkPropertyMetadataOptions.Inherits, new PropertyChangedCallback(OnItemHeightChanged)));
            Gallery.ItemMinWidthProperty.AddOwner(typeof(GalleryGroup), new FrameworkPropertyMetadata(20d, FrameworkPropertyMetadataOptions.Inherits, new PropertyChangedCallback(OnItemMinWidthChanged)));
            Gallery.ItemMinHeightProperty.AddOwner(typeof(GalleryGroup), new FrameworkPropertyMetadata(20d, FrameworkPropertyMetadataOptions.Inherits, new PropertyChangedCallback(OnItemMinHeightChanged)));
            Gallery.ItemMaxWidthProperty.AddOwner(typeof(GalleryGroup), new FrameworkPropertyMetadata(100d, FrameworkPropertyMetadataOptions.Inherits, new PropertyChangedCallback(OnItemMaxHeightChanged)));
            Gallery.ItemMaxHeightProperty.AddOwner(typeof(GalleryGroup), new FrameworkPropertyMetadata(100d, FrameworkPropertyMetadataOptions.Inherits, new PropertyChangedCallback(OnItemMaxHeightChanged)));
            Gallery.AllowVaryingItemSizeProperty.AddOwner(typeof(GalleryGroup), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits, new PropertyChangedCallback(OnAllowVaryingItemSizeChanged)));
            Gallery.SpaceLimitBetweenItemsProperty.AddOwner(typeof(GalleryGroup), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.Inherits, new PropertyChangedCallback(OnSpaceLimitBetweenItemsChanged)));
            Gallery.VisualModeProperty.AddOwner(typeof(GalleryGroup), new FrameworkPropertyMetadata(GalleryVisualMode.Standard, FrameworkPropertyMetadataOptions.Inherits, new PropertyChangedCallback(OnVisualModeChanged)));

            RoutedEvent ev = GalleryItem.IsSelectedChangedEvent.AddOwner(typeof(GalleryGroup));
            EventManager.RegisterClassHandler(typeof(GalleryGroup), ev, new RoutedEventHandler(IsSelectedChangedEventHandler));

            ev = GalleryItem.VisibilityChangedEvent.AddOwner(typeof(GalleryGroup));
            EventManager.RegisterClassHandler(typeof(GalleryGroup), ev, new RoutedEventHandler(VisibilityChangedEventHandler));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GalleryGroup"/> class.
        /// </summary>
        public GalleryGroup()
        {
            if (EnvironmentTestTools.IsSecurityGranted)
            {
                EnvironmentTestTools.StartValidateLicense(typeof(GalleryGroup));
            }

            SelectedItems = new ObjectCollection();
            Gallery.M_dragArray = new List<GalleryItem>();

            GalleryItemMouseLeftButtonDown += new MouseButtonEventHandler(OnGalleryItemMouseLeftButtonDown);
            SelectedItems.CollectionChanged += new NotifyCollectionChangedEventHandler(SelectedItems_CollectionChanged);

            FocusVisualStyle = null;
        }
        #endregion

        #region	Events

        /// <summary>
        /// Identifies the <see cref="ItemGenerated"/> routed_event.
        /// </summary>
        public static readonly RoutedEvent ItemGeneratedEvent = EventManager.RegisterRoutedEvent(
            "ItemGenerated",
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(GalleryGroup));

        /// <summary>
        /// Occurs when [item generated].
        /// </summary>
        public event RoutedEventHandler ItemGenerated
        {
            add
            {
                AddHandler(GalleryGroup.ItemGeneratedEvent, value);
            }

            remove
            {
                RemoveHandler(GalleryGroup.ItemGeneratedEvent, value);
            }
        }

        /// <summary>
        /// Event that is raised when item was clicked.
        /// </summary>
        public event MouseButtonEventHandler GalleryItemMouseLeftButtonDown;

        /// <summary>
        /// Event that is raised when <see cref="AllowedItemResizeMode"/> property is
        /// changed.
        /// </summary>
        public event PropertyChangedCallback AllowedItemResizeModeChanged;

        /// <summary>
        /// Event that is raised when <see cref="AllowMultiSelect"/> property is
        /// changed.
        /// </summary>
        public event PropertyChangedCallback AllowMultiSelectChanged;

        /// <summary>
        /// Event that is raised when <see cref="ItemContentTemplate"/> property is
        /// changed.
        /// </summary>
        public event PropertyChangedCallback ItemContentTemplateChanged;

        /// <summary>
        /// Event that is raised when <see cref="ItemContentTemplateSelector"/>
        /// property is changed.
        /// </summary>
        public event PropertyChangedCallback ItemContentTemplateSelectorChanged;

        /// <summary>
        /// Event that is raised when <see cref="ItemMargin"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback ItemMarginChanged;

        /// <summary>
        /// Event that is raised when <see cref="ItemWidth"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback ItemWidthChanged;

        /// <summary>
        /// Event that is raised when <see cref="ItemHeight"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback ItemHeightChanged;

        /// <summary>
        /// Event that is raised when <see cref="ItemMinWidth"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback ItemMinWidthChanged;

        /// <summary>
        /// Event that is raised when <see cref="ItemMaxWidth"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback ItemMaxWidthChanged;

        /// <summary>
        /// Event that is raised when <see cref="ItemMinHeight"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback ItemMinHeightChanged;

        /// <summary>
        /// Event that is raised when <see cref="ItemMaxHeight"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback ItemMaxHeightChanged;

        /// <summary>
        /// Event that is raised when <see cref="AllowVaryingItemSize"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback AllowVaryingItemSizeChanged;

        /// <summary>
        /// Event that is raised when <see cref="SpaceLimitBetweenItems"/> property is
        /// changed.
        /// </summary>
        public event PropertyChangedCallback SpaceLimitBetweenItemsChanged;

        /// <summary>
        /// Event that is raised when <see cref="VisualMode"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback VisualModeChanged;

        /// <summary>
        /// Event that is raised when <see cref="IsDragOver"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback IsDragOverChanged;

        /// <summary>
        /// Event that is raised when <see cref="CaptionAlignment"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback CaptionAlignmentChanged;

        /// <summary>
        /// Event that is raised when <see cref="DescriptionAlignment"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback DescriptionAlignmentChanged;

        /// <summary>
        /// Event that is raised when <see cref="IsAlwaysShownCaption"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback IsAlwaysShownCaptionChanged;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the value that describes how item size should be changed, when group size is changing. 
        /// This is dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="AllowedItemResizeModes"/>
        /// <para/>
        /// Default value is AllowedItemResizeModes.None
        /// </value>
        /// <seealso cref="AllowedItemResizeModes"/> enum.
        public AllowedItemResizeModes AllowedItemResizeMode
        {
            get
            {
                return (AllowedItemResizeModes)GetValue(Gallery.AllowedItemResizeModeProperty);
            }

            set
            {
                SetValue(Gallery.AllowedItemResizeModeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value that describes allowed animations when group size is changed.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="AllowedAnimations"/>
        /// <para/>
        /// Default value is AllowedAnimations.All
        /// </value>                
        /// <seealso cref="AllowedAnimations"/> enum.
        public AllowedAnimations AllowedAnimations
        {
            get
            {
                return (AllowedAnimations)GetValue(Gallery.AllowedAnimationsProperty);
            }

            set
            {
                SetValue(Gallery.AllowedAnimationsProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [allow varying item size].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [allow varying item size]; otherwise, <c>false</c>.
        /// </value>
        public bool AllowVaryingItemSize
        {
            get
            {
                return (bool)GetValue(Gallery.AllowVaryingItemSizeProperty);
            }

            set
            {
                SetValue(Gallery.AllowVaryingItemSizeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether more than one item can be selected in group.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="bool"/>
        /// <para/>
        /// True - more then one item can be selected in group, false - just one item can be selected.
        /// </value>
        public bool AllowMultiSelect
        {
            get
            {
                return (bool)GetValue(Gallery.AllowMultiSelectProperty);
            }

            set
            {
                SetValue(Gallery.AllowMultiSelectProperty, value);
            }
        }

        /// <summary>
        /// Gets the index of the has focus.
        /// </summary>
        /// <value>The index of the has focus.</value>
        public int HasFocusIndex
        {
            get
            {
                return m_hasFocusIndex;
            }
        }

        /// <summary>
        /// Gets or sets the value that represents the template of item content data. This is dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="DataTemplate"/>
        /// <para/>
        /// Default value is null.
        /// </value>    
        public DataTemplate ItemContentTemplate
        {
            get
            {
                return (DataTemplate)GetValue(ItemContentTemplateProperty);
            }

            set
            {
                SetValue(ItemContentTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value that provides a way to choose a <see cref="DataTemplate"/> based on the data object 
        /// and the data-bound element.
        /// </summary>
        /// <value>
        /// Type: <see cref="DataTemplateSelector"/>
        /// <para/>
        /// Default value is null.
        /// </value>
        /// <seealso cref="DataTemplateSelector"/>
        public DataTemplateSelector ItemContentTemplateSelector
        {
            get
            {
                return (DataTemplateSelector)GetValue(ItemContentTemplateSelectorProperty);
            }

            set
            {
                SetValue(ItemContentTemplateSelectorProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value that represents margins of items in group. This is dependency properties.
        /// </summary>
        /// <value>
        /// Type: <see cref="Thickness"/>
        /// <para/>
        /// Default value is 0.
        /// </value>        
        public Thickness ItemMargin
        {
            get
            {
                return (Thickness)GetValue(Gallery.ItemMarginProperty);
            }

            set
            {
                SetValue(Gallery.ItemMarginProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value that represents width of items in group. This is dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Double"/>
        /// <para/>
        /// Default value is 50.
        /// </value>
        public double ItemWidth
        {
            get
            {
                return (double)GetValue(Gallery.ItemWidthProperty);
            }

            set
            {
                SetValue(Gallery.ItemWidthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value that represents height of items in group. This is dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Double"/>
        /// <para/>
        /// Default value is 50.
        /// </value>
        public double ItemHeight
        {
            get
            {
                return (double)GetValue(Gallery.ItemHeightProperty);
            }

            set
            {
                SetValue(Gallery.ItemHeightProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether a group or item is dragging over this.
        /// This is dependency property.
        /// </summary>
        public bool IsDragOver
        {
            get
            {
                return (bool)GetValue(IsDragOverProperty);
            }

            protected internal set
            {
                SetValue(IsDragOverPropertyKey, value);
            }
        }

        /// <summary>
        /// Gets or sets the minimal value of <see cref="ItemWidth"/> property. This is dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Double"/>
        /// <para/>
        /// Default value is 0.
        /// </value>
        public double ItemMinWidth
        {
            get
            {
                return (double)GetValue(Gallery.ItemMinWidthProperty);
            }

            set
            {
                SetValue(Gallery.ItemMinWidthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the maximal value of <see cref="ItemWidth"/> property. This is dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Double"/>
        /// <para/>
        /// Default value is 100.
        /// </value>
        public double ItemMaxWidth
        {
            get
            {
                return (double)GetValue(Gallery.ItemMaxWidthProperty);
            }

            set
            {
                SetValue(Gallery.ItemMaxWidthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the minimal value of <see cref="ItemHeight"/> property. This is dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Double"/>
        /// <para/>
        /// Default value is 0.
        /// </value>
        public double ItemMinHeight
        {
            get
            {
                return (double)GetValue(Gallery.ItemMinHeightProperty);
            }

            set
            {
                SetValue(Gallery.ItemMinHeightProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the maximal value of <see cref="ItemHeight"/> property. This is dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Double"/>
        /// <para/>
        /// Default value is 0.
        /// </value>
        public double ItemMaxHeight
        {
            get
            {
                return (double)GetValue(Gallery.ItemMaxHeightProperty);
            }

            set
            {
                SetValue(Gallery.ItemMaxHeightProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value that is responsible for collecting of selected
        /// items. This is dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="ObjectCollection"/>
        /// <para/>
        /// Default value is collection with no items.
        /// </value>
        public ObjectCollection SelectedItems
        {
            get
            {
                return (ObjectCollection)GetValue(SelectedItemsProperty);
            }

            set
            {
                SetValue(SelectedItemsProperty, value);
            }
        }

        /// <summary>
        /// Gets the value that represents index of the first item in selection.
        /// </summary>
        /// <value>
        /// Type: <see cref="Int32"/>
        /// <para/>
        /// Returns -1 if group hasn't selected items, otherwise index of the first item in selection.
        /// </value>
        public int SelectionStartIndex
        {
            get
            {
                if (SelectedItems.Count > 0)
                {
                    return m_selectionStartIndex;
                }
                else
                {
                    return -1;
                }
            }
        }

        /// <summary>
        /// Gets the value that represents start item in selection of items.
        /// </summary>
        /// <value>
        /// Type: <see cref="object"/>
        /// <para/>
        /// If selection of items exists returns first item, otherwise null.
        /// </value>
        public object SelectionStartItem
        {
            get
            {
                if (m_selectionStartIndex != -1 && SelectedItems.Count > 0)
                {
                    object obj = new object();

                    GalleryItem item = ArrangedItems[m_selectionStartIndex];

                    if (!Items.Contains(item))
                    {
                        obj = item.Content;
                    }
                    else
                    {
                        obj = item;
                    }

                    return obj;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Gets or sets the value that represents space limit between items in group when <see cref="AllowedItemResizeMode"/> set to 
        /// Space value. This is dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Double"/>
        /// <para/>
        /// Default value is 0.
        /// </value>
        public double SpaceLimitBetweenItems
        {
            get
            {
                return (double)GetValue(Gallery.SpaceLimitBetweenItemsProperty);
            }

            set
            {
                SetValue(Gallery.SpaceLimitBetweenItemsProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value that represents visual mode of group. This is dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="GalleryVisualMode"/>
        /// <para/>
        /// Default value is Standard.
        /// </value>
        /// <seealso cref="GalleryVisualMode"/> enum.
        public GalleryVisualMode VisualMode
        {
            get
            {
                return (GalleryVisualMode)GetValue(Gallery.VisualModeProperty);
            }

            set
            {
                SetValue(Gallery.VisualModeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value that represents alignment of caption in gallery item. This is dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="CaptionAlignment"/>
        /// <para/>
        /// Default value is Top.
        /// </value>
        /// <remarks>
        /// Alignment occurs only in Standard mode when <see cref="IsAlwaysShownCaption"/> is true. 
        /// It doesn't affect captions in Detailed mode.
        /// </remarks>
        /// <seealso cref="GalleryVisualMode"/>
        public CaptionAlignment CaptionAlignment
        {
            get
            {
                return (CaptionAlignment)GetValue(CaptionAlignmentProperty);
            }

            set
            {
                SetValue(CaptionAlignmentProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value that represents alignment of description in gallery item. This is dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="CaptionAlignment"/>
        /// <para/>
        /// Default value is Bottom.
        /// </value>
        /// <remarks>
        /// Alignment occurs only in Standard mode when <see cref="IsAlwaysShownCaption"/> is true. 
        /// It doesn't affect descriptions in Detailed mode.
        /// </remarks>
        /// <seealso cref="GalleryVisualMode"/>
        public CaptionAlignment DescriptionAlignment
        {
            get
            {
                return (CaptionAlignment)GetValue(DescriptionAlignmentProperty);
            }

            set
            {
                SetValue(DescriptionAlignmentProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the caption and description in gallery item
        /// are visible. This is dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="bool"/>
        /// <para/>
        /// Default value is false.
        /// </value>
        /// <remarks>
        /// When the value is set to false, the caption and the description in gallery item are displayed only in Detailed mode.
        /// Otherwise they are always visible.
        /// </remarks>
        /// <seealso cref="GalleryVisualMode"/>
        public bool IsAlwaysShownCaption
        {
            get
            {
                return (bool)GetValue(IsAlwaysShownCaptionProperty);
            }

            set
            {
                SetValue(IsAlwaysShownCaptionProperty, value);
            }
        }

        /// <summary>
        /// Gets the row count.
        /// </summary>
        /// <value>The row count.</value>
        internal int RowCount
        {
            get
            {
                if (null == m_navigationMatrix)
                {
                    UpdateNavigationMatrix();
                }

                return m_rowCount;
            }
        }

        /// <summary>
        /// Gets the col count.
        /// </summary>
        /// <value>The col count.</value>
        internal int ColCount
        {
            get
            {
                if (null == m_navigationMatrix)
                {
                    UpdateNavigationMatrix();
                }

                return m_colCount;
            }
        }

        /// <summary>
        /// Gets collection of the GalleryItem class instances, which
        /// are arranged.
        /// </summary>
        private ItemsCollection ArrangedItems
        {
            get
            {
                if (null == m_arrangedItems)
                {
                    m_arrangedItems = new ItemsCollection();
                    GalleryItem galleryItem = new GalleryItem();

                    int count = Items.Count;

                    for (int i = 0; i < count; i++)
                    {
                        if (Items[i] is GalleryItem)
                        {
                            galleryItem = (GalleryItem)Items[i];
                        }
                        else
                        {
                            galleryItem = (GalleryItem)ItemContainerGenerator.ContainerFromIndex(i);
                        }

                        if (null != galleryItem && Visibility.Collapsed != galleryItem.Visibility)
                        {
                            m_arrangedItems.Add(galleryItem);
                        }
                    }
                }

                return m_arrangedItems;
            }
        }
        #endregion

        #region Implementation

        /// <summary>
        /// Raises the <see cref="E:ItemGenerated"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        internal virtual void OnItemGenerated(RoutedEventArgs e)
        {
            base.RaiseEvent(e);
        }

        /// <summary>
        /// Invoked when the <see cref="P:System.Windows.Controls.ItemsControl.Items"/> property changes.
        /// </summary>
        /// <param name="e">Information about the change.</param>
        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            if (null != m_arrangedItems)
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        IList list = e.NewItems;
                        GalleryItem galleryItem = new GalleryItem();

                        for (int i = 0, cnt = list.Count; i < cnt; i++)
                        {
                            if (list[i] is GalleryItem)
                            {
                                galleryItem = (GalleryItem)list[i];
                            }
                            else
                            {
                                int index = Items.IndexOf(list[i]);
                                galleryItem = (GalleryItem)ItemContainerGenerator.ContainerFromIndex(index);
                            }

                            if (galleryItem.Visibility != Visibility.Collapsed)
                            {
                                m_arrangedItems.Add(galleryItem);
                            }
                        }

                        break;
                    case NotifyCollectionChangedAction.Remove:
                        IList oldList = e.OldItems;
                        GalleryItem gItem = new GalleryItem();

                        for (int i = 0, cnt = oldList.Count; i < cnt; i++)
                        {
                            if (oldList[i] is GalleryItem)
                            {
                                gItem = (GalleryItem)oldList[i];
                            }
                            else
                            {
                                int index = Items.IndexOf(oldList[i]);

                                if (-1 == index)
                                {
                                    break;
                                }

                                gItem = (GalleryItem)ItemContainerGenerator.ContainerFromIndex(index);
                            }

                            if (m_arrangedItems.Contains(gItem))
                            {
                                m_arrangedItems.Remove(gItem);
                            }
                        }

                        break;
                    case NotifyCollectionChangedAction.Reset:
                        m_arrangedItems.Clear();
                        break;
                    default:
#if DEBUG
                        throw new NotImplementedException("This action isn't supported.");
#endif
                }
            }

            base.OnItemsChanged(e);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.FrameworkElement.SizeChanged"/> event, using the specified information as part of the eventual event data.
        /// </summary>
        /// <param name="sizeInfo">Information about size.</param>
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            if (m_hasFocusIndex != -1 && null != m_arrangedItems && m_hasFocusIndex < m_arrangedItems.Count)
            {
                m_arrangedItems[m_hasFocusIndex].HasFocus = false;
                m_hasFocusIndex = -1;
            }

            m_navigationMatrix = null;
            m_arrangedItems = null;

            base.OnRenderSizeChanged(sizeInfo);
        }

        /// <summary>
        /// Fires <see cref="GalleryItemMouseLeftButtonDown"/> event.
        /// </summary>
        /// <param name="e">
        /// The instance containing the event data.</param>
        /// <param name="source">Event source.</param>
        protected virtual void FireGalleryItemMouseLeftButtonDown(MouseButtonEventArgs e, GalleryItem source)
        {
            if (GalleryItemMouseLeftButtonDown != null)
            {
                GalleryItemMouseLeftButtonDown(source, e);
            }
        }

        /// <summary>
        /// Invoked when item was clicked.
        /// </summary>
        /// <param name="sender">Instance of GalleryItem item.</param>
        protected virtual void OnGalleryItemClick(GalleryItem sender)
        {
            ItemsCollection arrangedCollection = ArrangedItems;

            if (arrangedCollection.Count > 0)
            {
                if (m_hasFocusIndex != -1 && m_hasFocusIndex < arrangedCollection.Count)
                {
                    arrangedCollection[m_hasFocusIndex].HasFocus = false;
                }

                m_bMultiselectionDone = false;

                if (AllowMultiSelect)
                {
                    if (Keyboard.Modifiers == ModifierKeys.Shift)
                    {
                        SelectedItems.Clear();
                        Gallery.M_dragArray.Clear();

                        if (m_selectionStartIndex == -1)
                        {
                            m_selectionStartIndex = 0;
                        }

                        int current = arrangedCollection.IndexOf(sender);
                        int step = (current < m_selectionStartIndex) ? -1 : 1;
                        int i;

                        for (i = m_selectionStartIndex; i != current + step; i += step)
                        {
                            SelectedItems.Add(arrangedCollection[i]);
                            if (!Gallery.M_dragArray.Contains((GalleryItem)arrangedCollection[i]))
                            {
                                Gallery.M_dragArray.Add((GalleryItem)arrangedCollection[i]);
                            }
                        }

                        m_hasFocusIndex = arrangedCollection.IndexOf(arrangedCollection[i - step]);
                        m_bMultiselectionDone = true;
                    }
                    else if (Keyboard.Modifiers == ModifierKeys.Control)
                    {
                        sender.IsSelected = !sender.IsSelected;
                        m_hasFocusIndex = m_selectionStartIndex = arrangedCollection.IndexOf(sender);
                        if (!Gallery.M_dragArray.Contains(sender))
                        {
                            Gallery.M_dragArray.Add(sender);
                        }

                        m_bMultiselectionDone = true;
                    }
                }

                m_selectedGalleryItem = sender;
            }
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.UIElement.MouseLeftButtonUp"/>�routed event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs"/> that contains the event data. The event data reports that the left mouse button was released.</param>
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);

            ItemsCollection arrangedCollection = ArrangedItems;

            if (!m_bMultiselectionDone)
            {
                SelectedItems.Clear();
                Gallery.M_dragArray.Clear();
                m_hasFocusIndex = m_selectionStartIndex = arrangedCollection.IndexOf(m_selectedGalleryItem);

                if (null != m_selectedGalleryItem)
                {
                    SelectedItems.Add(m_selectedGalleryItem);
                    if (!Gallery.M_dragArray.Contains(m_selectedGalleryItem))
                    {

                        Gallery.M_dragArray.Add(m_selectedGalleryItem);
                    }
                }
            }

            if (m_hasFocusIndex != -1)
            {
                if (arrangedCollection.Count > 0)
                    arrangedCollection[m_hasFocusIndex].HasFocus = true;
            }
        }

        /// <summary>
        /// Invoked when <see cref="E:System.Windows.DragDrop.DragEnter"/> event is raised.
        /// </summary>
        /// <param name="e">
        /// The instance containing the event data.</param>
        protected override void OnDragEnter(DragEventArgs e)
        {
            IsDragOver = true;

            if (m_hasFocusIndex != -1 && null != m_arrangedItems && m_hasFocusIndex < m_arrangedItems.Count)
            {
                m_arrangedItems[m_hasFocusIndex].HasFocus = false;
            }

            if (Gallery.M_dragArray.Count > 1)
            {
                m_isMultipleDragging = true;
            }

            base.OnDragEnter(e);
        }

        /// <summary>
        /// Invoked when <see cref="E:System.Windows.DragDrop.DragLeave"/> event is raised.
        /// </summary>
        /// <param name="e">The instance containing the event data.</param>
        protected override void OnDragLeave(DragEventArgs e)
        {
            IsDragOver = false;

            base.OnDragLeave(e);
        }

        /// <summary>
        /// Calls OnCaptionAlignmentChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnCaptionAlignmentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GalleryGroup instance = (GalleryGroup)d;
            instance.OnCaptionAlignmentChanged(e);
        }

        /// <summary>
        /// Raises <see cref="CaptionAlignmentChanged"/> event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnCaptionAlignmentChanged(DependencyPropertyChangedEventArgs e)
        {
            if (CaptionAlignmentChanged != null)
            {
                CaptionAlignmentChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnDescriptionAlignmentChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnDescriptionAlignmentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GalleryGroup instance = (GalleryGroup)d;
            instance.OnDescriptionAlignmentChanged(e);
        }

        /// <summary>
        /// Raises <see cref="DescriptionAlignmentChanged"/> event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnDescriptionAlignmentChanged(DependencyPropertyChangedEventArgs e)
        {
            if (CaptionAlignmentChanged != null)
            {
                DescriptionAlignmentChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnIsAlwaysShownCaptionChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnIsAlwaysShownCaptionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GalleryGroup instance = (GalleryGroup)d;
            instance.OnIsAlwaysShownCaptionChanged(e);
        }

        /// <summary>
        /// Raises <see cref="IsAlwaysShownCaptionChanged"/> event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnIsAlwaysShownCaptionChanged(DependencyPropertyChangedEventArgs e)
        {
            if (IsAlwaysShownCaptionChanged != null)
            {
                IsAlwaysShownCaptionChanged(this, e);
            }
        }

        /// <summary>
        /// Updates states of gallery items.
        /// </summary>
        /// <param name="itemParent">GalleryGroup element</param>
        private void UpdateItems(GalleryGroup itemParent)
        {
            itemParent.m_selectionStartIndex = -1;
            itemParent.SelectedItems.Clear();

            m_arrangedItems = null;
            UpdateNavigationMatrix();
            Focus();
            Gallery groupParent = (Gallery)Gallery.FindAncestor(typeof(Gallery), this);
            groupParent.SelectedIndex = groupParent.Items.IndexOf(this);
        }

        /// <summary>
        /// Invoked when Drop event is raised.
        /// </summary>
        /// <param name="e">
        /// The instance containing the event data.</param>
        protected override void OnDrop(DragEventArgs e)
        {
            if (m_isMultipleDragging)
            {
                ProcessMultipleItemsDrop(e);
            }
            else if (e.Data.GetDataPresent(FORMAT_STRING))
            {
                ProcessSingleItemDrop(e);
            }

            IsDragOver = false;
            base.OnDrop(e);
        }

        /// <summary>
        /// Is used to process drop for single gallery item.
        /// </summary>
        /// <param name="e">Drag Event data</param>
        private void ProcessSingleItemDrop(DragEventArgs e)
        {
            GalleryItem item = (GalleryItem)e.Data.GetData(FORMAT_STRING);
            object obj = new object();
            GalleryGroup itemParent = null;

            if (item.Parent != null)
            {
                itemParent = (GalleryGroup)item.Parent;
                obj = item;
            }
            else
            {
                itemParent = item.GetParent();
                obj = item.Content;
            }

            if (itemParent != null)
            {
                GalleryItem source = GetGalleryItem(e);

                if (source != null)
                {
                    ChangeItemsList(source, item, itemParent, obj);
                }
                else
                {
                    GalleryItem gItem = GetItemUnderMouse(e, item.RenderSize);

                    if (null != gItem)
                    {
                        ChangeItemsList(gItem, item, itemParent, obj);
                    }
                    else
                    {
                        if (itemParent.ItemsSource != null)
                        {
                            RemoveFromItemSource(itemParent.ItemsSource, obj);
                        }
                        else
                        {
                            itemParent.Items.Remove(obj);
                        }

                        if (ItemsSource != null)
                        {
                            IList list = ItemsSource as IList;
                            if (list != null)
                            {
                                list.Add(obj);
                            }
                        }
                        else
                        {
                            Items.Add(obj);
                        }
                    }
                }

                UpdateItems(itemParent);

                //if (itemParent.ItemsSource != null)
                //{
                //    itemParent.Items.Refresh();
                //}
                //if (ItemsSource != null)
                //{
                //    Items.Refresh();
                //}
            }
        }

        /// <summary>
        /// Is used to process multiple drop for gallery items.
        /// </summary>
        /// <param name="e">Drag Event Args data</param>
        private void ProcessMultipleItemsDrop(DragEventArgs e)
        {
            GalleryItem item = (GalleryItem)e.Data.GetData(FORMAT_STRING);
            if (item != null)
            {
                GalleryGroup itemParent = item.GetParent();
                List<GalleryItem> dragArray = Gallery.M_dragArray;

                if (itemParent != null)
                {
                    GalleryItem source = GetGalleryItem(e);

                    if (null == source)
                    {
                        source = GetItemUnderMouse(e, item.RenderSize);
                    }

                    if (source != null)
                    {
                        for (int i = 0, arrayCount = dragArray.Count; i < arrayCount; i++)
                        {
                            ChangeItemsList(source, dragArray[i], itemParent, dragArray[i]);
                        }
                    }
                    else
                    {
                        MultipleDraggingBetweenGroups(itemParent);
                    }

                    UpdateItems(this);
                    UpdateItems(itemParent);
                }
            }

            Gallery.M_dragArray.Clear();
            m_isMultipleDragging = false;

        }

        /// <summary>
        /// Gets the item under mouse.
        /// </summary>
        /// <param name="arg">The <see cref="System.Windows.DragEventArgs"/> instance containing the event data.</param>
        /// <param name="renderSize">Size render of the GalleryItem.</param>
        /// <returns>Returns the gallery item</returns>
        private GalleryItem GetItemUnderMouse(DragEventArgs arg, Size renderSize)
        {
            GalleryItem gItem = null;

            if (null == m_navigationMatrix)
            {
                UpdateNavigationMatrix();
            }

            if (null != m_navigationMatrix)
            {
                Point point = arg.GetPosition((IInputElement)arg.Source);
                int i = (int)Math.Floor(point.Y / renderSize.Height) - 1;
                int j = (int)Math.Floor(point.X / renderSize.Width);

                if (0 <= i)
                {
                    bool nonAddLast = true;

                    if (m_navigationMatrix.GetLength(COL_MATRIX_INDEX) <= j)
                    {
                        if (m_navigationMatrix.GetLength(ROW_MATRIX_INDEX) - 1 > i)
                        {
                            j = 0;
                            ++i;
                        }
                        else
                        {
                            nonAddLast = false;
                        }
                        if (nonAddLast)
                        {
                            gItem = m_navigationMatrix[i, j];
                        }
                    }
                }
            }

            return gItem;
        }

        /// <summary>
        /// Removes dragged items from old place and inserts to new place.
        /// </summary>
        /// <param name="source">GalleryItem in the visual tree from the DragEventArgs</param>
        /// <param name="item">dragged item</param>
        /// <param name="itemParent">parent of the dragged item</param>
        /// <param name="obj">dragged item or dragged item content</param>
        private void ChangeItemsList(GalleryItem source, GalleryItem item, GalleryGroup itemParent, object obj)
        {
            int index = ArrangedItems.IndexOf(source);
            source.IsDragOver = false;
            itemParent.m_hasFocusIndex = -1;
            int indexToRemove = -1;
            object newObject = item.Content;

            if (obj is FrameworkElement)
            {
                indexToRemove = itemParent.Items.IndexOf(obj);
            }
            else
            {
                indexToRemove = itemParent.ItemContainerGenerator.IndexFromContainer(item);
            }

            if (indexToRemove == -1)
            {
                indexToRemove = itemParent.ItemContainerGenerator.IndexFromContainer(obj as GalleryItem);
            }

            if (indexToRemove != -1)
            {
                if (itemParent.ItemsSource != null)
                {
                    RemoveFromItemSource(itemParent.ItemsSource, indexToRemove);
                }
                else
                {
                    itemParent.Items.RemoveAt(indexToRemove);
                }

                if (Items.Count < index || 0 > index)
                {
                    index = Items.Count;
                }
                if (ItemsSource != null)
                {
                    IList list = ItemsSource as IList;
                    if (list != null)
                    {
                        if (obj is GalleryItem)
                        {
                            list.Insert(index, newObject);
                        }
                        else
                        {
                            list.Insert(index, obj);
                        }
                    }
                }
                else
                {
                    Items.Insert(index, obj);
                }
            }
        }

        /// <summary>
        /// Removes from item source.
        /// </summary>
        /// <param name="itemsSource">The items source.</param>
        /// <param name="index">The index.</param>
        private void RemoveFromItemSource(IEnumerable itemsSource, int index)
        {
            ICollectionView list = CollectionViewSource.GetDefaultView(itemsSource);
            if (list is ListCollectionView)
            {
                ListCollectionView lcollview = list as ListCollectionView;
                lcollview.RemoveAt(index);
            }
            else if (list is BindingListCollectionView)
            {
                BindingListCollectionView blcollview = list as BindingListCollectionView;
                blcollview.RemoveAt(index);
            }
        }

        /// <summary>
        /// Removes from item source.
        /// </summary>
        /// <param name="itemsSource">The items source.</param>
        /// <param name="obj">The obj.</param>
        private void RemoveFromItemSource(IEnumerable itemsSource, object obj)
        {
            ICollectionView list = CollectionViewSource.GetDefaultView(itemsSource);
            if (list is ListCollectionView)
            {
                ListCollectionView lcollview = list as ListCollectionView;
                lcollview.Remove(obj);
            }
            else if (list is BindingListCollectionView)
            {
                BindingListCollectionView blcollview = list as BindingListCollectionView;
                blcollview.Remove(obj);
            }
        }

        /// <summary>
        /// Implements multiple dragging between groups.
        /// </summary>
        /// <param name="itemParent">parent of gallery item</param>
        private void MultipleDraggingBetweenGroups(GalleryGroup itemParent)
        {
            List<GalleryItem> dragArray = Gallery.M_dragArray;
            object obj = new object();

            for (int i = 0, arrayCount = dragArray.Count; i < arrayCount; i++)
            {
                GalleryItem item = dragArray[i];

                if (itemParent.ArrangedItems.Contains(item))
                {
                    if (null == item.Parent)
                    {
                        int index = itemParent.Items.IndexOf(item.Content);
                        obj = item.Content;
                        if (itemParent.ItemsSource != null)
                        {
                            RemoveFromItemSource(itemParent.ItemsSource, obj);
                        }
                        else
                        {
                            itemParent.Items.RemoveAt(index);
                        }


                    }
                    else
                    {
                        GalleryGroup groupParent = (GalleryGroup)item.Parent;

                        if (groupParent.ItemsSource != null)
                        {
                            RemoveFromItemSource(groupParent.ItemsSource, item.Content);
                        }
                        else
                        {
                            groupParent.Items.Remove(item);
                        }


                    }

                    if (ItemsSource != null)
                    {
                        IList list = ItemsSource as IList;
                        if (list != null)
                        {
                            list.Add(obj);
                        }
                    }
                    else
                    {
                        Items.Add(item);
                    }
                }
            }
        }

        /// <summary>
        /// Invoked when QueryContinueDrag event is raised.
        /// </summary>
        /// <param name="e">
        /// The instance that contains the event data.</param>
        protected override void OnQueryContinueDrag(QueryContinueDragEventArgs e)
        {
            if (e.KeyStates != DragDropKeyStates.LeftMouseButton || e.EscapePressed)
            {
                Opacity = 1;
                IsDragOver = false;
            }

            base.OnQueryContinueDrag(e);
        }

        /// <summary>
        /// Invoked when <see cref="E:System.Windows.UIElement.PreviewMouseLeftButtonDown"/> event is raised.
        /// </summary>
        /// <param name="e">
        /// The instance that contains the event data.</param>
        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            //Focus();

            GalleryItem source = GetGalleryItem(e);

            if (source != null)
            {
                FireGalleryItemMouseLeftButtonDown(e, source);
            }
            else if (EnvironmentTest.IsSecurityGranted)
            {
                SelectedItems.Clear();
                m_selectedGalleryItem = null;
                m_iinputElement = FindRootContainer((FrameworkElement)this);
                m_startPoint = e.GetPosition(m_iinputElement);

                Gallery gallery = (Gallery)Gallery.FindAncestor(typeof(Gallery), this);
                if (gallery.CanDragDrop)
                {
                    m_dataObject = new DataObject();
                    m_dataObject.SetData("Group", this);
                }
            }
        }

        /// <summary>
        /// Invoked when <see cref="E:System.Windows.UIElement.MouseMove"/> event is raised.
        /// </summary>
        /// <param name="e">
        /// An instance that contains event data.</param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Point currentPoint = e.GetPosition(m_iinputElement);

                if (Math.Abs(currentPoint.X - m_startPoint.X) > SystemParameters.MinimumHorizontalDragDistance || Math.Abs(currentPoint.Y - m_startPoint.Y) > SystemParameters.MinimumVerticalDragDistance)
                {
                    if (m_dataObject != null)
                    {
                        Opacity = 0.5;
                        DragDrop.DoDragDrop(this, m_dataObject, DragDropEffects.Move | DragDropEffects.Copy);
                    }
                }
            }

            e.Handled = true;
        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
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
            return base.MeasureOverride(constraint);
        }

        /// <summary>
        /// Called to arrange and size the content of a <see cref="T:System.Windows.Controls.Control"/> object.
        /// </summary>
        /// <param name="arrangeBounds">The computed size that is used to arrange the content.</param>
        /// <returns>The size of the control.</returns>
        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            return base.ArrangeOverride(arrangeBounds);
        }
        /// <summary>
        /// Invoked when <see cref="E:System.Windows.UIElement.KeyDown"/> event is raised.
        /// </summary>
        /// <param name="e">
        /// The instance that contains the event data.</param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (!e.Handled)
            {
                ItemsCollection arrangedCollection = ArrangedItems;
                int arrangedCollCount = arrangedCollection.Count;

                bool bKeyboardModifiers = Keyboard.Modifiers == ModifierKeys.Shift;

                if (m_navigationMatrix == null)
                {
                    UpdateNavigationMatrix();
                }

                if (arrangedCollCount != 0)
                {
                    // check if all arranged items are hidden
                    if (m_currentRowIndex == -1 || m_currentColIndex == -1)
                    {
                        e.Handled = true;
                        return;
                    }

                    switch (e.Key)
                    {
                        case Key.Tab:
                            if (m_hasFocusIndex + 1 < arrangedCollCount)
                            {
                                m_hasFocusIndex++;
                                m_currentColIndex = m_hasFocusIndex % ColCount;
                                m_currentRowIndex = m_hasFocusIndex / ColCount;
                            }
                            else
                            {
                                m_hasFocusIndex = -1;
                            }

                            m_tabWasPressed = true;
                            break;
                        case Key.Right:
                            m_hasFocusIndex = FocusRight();
                            e.Handled = true;
                            break;

                        case Key.Left:
                            m_hasFocusIndex = FocusLeft();
                            e.Handled = true;
                            break;

                        case Key.Down:
                            m_hasFocusIndex = FocusDown();
                            e.Handled = true;
                            break;

                        case Key.Up:
                            m_hasFocusIndex = FocusUp();
                            e.Handled = true;
                            break;

                        case Key.Space:
                            if (m_hasFocusIndex != -1)
                            {
                                OnGalleryItemClick(arrangedCollection[m_hasFocusIndex]);
                            }

                            e.Handled = true;
                            break;

                        case Key.Home:
                            if (m_hasFocusIndex != -1)
                            {
                                arrangedCollection[m_hasFocusIndex].HasFocus = false;
                            }

                            m_hasFocusIndex = 0;
                            for (int i = 0; i < arrangedCollCount; i++)
                            {
                                if (arrangedCollection[i].Visibility != Visibility.Hidden)
                                {
                                    m_hasFocusIndex = i;
                                    break;
                                }
                            }

                            arrangedCollection[m_hasFocusIndex].HasFocus = true;

                            OnGalleryItemClick(arrangedCollection[m_hasFocusIndex]);

                            m_navigationMatrix = null;
                            e.Handled = true;
                            break;

                        case Key.End:
                            if (m_hasFocusIndex != -1)
                            {
                                arrangedCollection[m_hasFocusIndex].HasFocus = false;
                            }

                            m_hasFocusIndex = arrangedCollCount - 1;
                            for (int i = arrangedCollCount - 1; i >= 0; i--)
                            {
                                if (arrangedCollection[i].Visibility != Visibility.Hidden)
                                {
                                    m_hasFocusIndex = i;
                                    break;
                                }
                            }

                            arrangedCollection[m_hasFocusIndex].HasFocus = true;

                            OnGalleryItemClick(arrangedCollection[m_hasFocusIndex]);

                            m_navigationMatrix = null;

                            e.Handled = true;
                            break;
                    }

                    if (m_hasFocusIndex != -1 && m_hasFocusIndex < arrangedCollection.Count)
                    {
                        arrangedCollection[m_hasFocusIndex].BringIntoView();
                    }

                    if (bKeyboardModifiers && m_hasFocusIndex != -1)
                    {
                        OnGalleryItemClick(arrangedCollection[m_hasFocusIndex]);
                    }
                }
            }
        }

        /// <summary>
        /// Invoked when <see cref="E:System.Windows.UIElement.LostFocus"/> event is raised.
        /// </summary>
        /// <param name="e">
        /// An instance that contains event data. This
        /// event data must contains the identifier for
        /// the event.</param>               
        protected override void OnLostFocus(RoutedEventArgs e)
        {
            if (!m_tabWasPressed)
            {
                if (m_hasFocusIndex != -1)
                {
                    if (m_arrangedItems != null && m_hasFocusIndex < m_arrangedItems.Count)
                    {
                        m_arrangedItems[m_hasFocusIndex].HasFocus = false;
                    }

                    m_hasFocusIndex = -1;
                    m_arrangedItems = null;
                }
            }

            m_tabWasPressed = false;
            base.OnLostFocus(e);
        }

        /// <summary>
        /// Invoked when <see cref="E:System.Windows.UIElement.GotFocus"/> event is raised.
        /// </summary>
        /// <param name="e">
        /// The instance containing the event data.</param>
        protected override void OnGotFocus(RoutedEventArgs e)
        {
          //  Gallery gallery = (Gallery)Parent;
            Gallery gallery = (Gallery)Gallery.FindAncestor(typeof(Gallery), this);
            if (gallery != null)
            {
                gallery.SelectedIndex = gallery.Items.IndexOf(this);
            }
            UpdateNavigationMatrix();
            base.OnGotFocus(e);
        }

        /// <summary>
        /// Invoked when <see cref="E:System.Windows.Input.Mouse.MouseUp"/> event is raised.
        /// </summary>
        /// <param name="e">
        /// The instance that contains the event data.
        /// The event data reports that the mouse button
        /// was released.</param>             
        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            UpdateNavigationMatrix();

            base.OnMouseUp(e);
        }

        /// <summary>
        /// Indicates when item type is GalleryItem.
        /// </summary>
        /// <param name="item">Given object.</param>
        /// <returns>True if item type is GalleryItem, otherwise - false.</returns>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is GalleryItem;
        }

        /// <summary>
        /// Gets GalleryItem container for non GalleryItem type item.
        /// </summary>
        /// <returns>
        /// New instance of GalleryItem.
        /// </returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new GalleryItem();
        }

        /// <summary>
        /// Calls OnAllowedItemResizeModeChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnAllowedItemResizeModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GalleryGroup instance = (GalleryGroup)d;
            instance.m_navigationMatrix = null;
            instance.OnAllowedItemResizeModeChanged(e);
        }

        /// <summary>
        /// Calls OnAllowMultipleSelectionChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnAllowMultiSelectChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GalleryGroup instance = (GalleryGroup)d;
            instance.OnAllowMultiSelectChanged(e);

            instance.SelectedItems.Clear();

            int temp = instance.m_hasFocusIndex;

            if (temp != -1)
            {
                instance.GetGalleryItem(instance.Items[temp]).IsSelected = true;
            }
        }

        /// <summary>
        /// Calls OnItemMarginChanged method of the instance, notifies of
        /// the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnItemMarginChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GalleryGroup instance = (GalleryGroup)d;
            instance.OnItemMarginChanged(e);
        }

        /// <summary>
        /// Calls OnItemWidthChanged method of the instance, notifies of
        /// the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnItemWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GalleryGroup instance = (GalleryGroup)d;
            instance.OnItemWidthChanged(e);
        }

        /// <summary>
        /// Calls OnItemHeightChanged method of the instance, notifies of
        /// the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnItemHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GalleryGroup instance = (GalleryGroup)d;
            instance.OnItemHeightChanged(e);
        }

        /// <summary>
        /// Calls OnItemMinWidthChanged method of the instance, notifies
        /// of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnItemMinWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GalleryGroup instance = (GalleryGroup)d;
            instance.OnItemMinWidthChanged(e);
        }

        /// <summary>
        /// Calls OnItemMaxWidthChanged method of the instance, notifies
        /// of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnItemMaxWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GalleryGroup instance = (GalleryGroup)d;
            instance.OnItemMaxWidthChanged(e);
        }

        /// <summary>
        /// Calls OnItemMinHeightChanged method of the instance, notifies
        /// of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnItemMinHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GalleryGroup instance = (GalleryGroup)d;
            instance.OnItemMinHeightChanged(e);
        }

        /// <summary>
        /// Calls OnItemMaxHeightChanged method of the instance, notifies
        /// of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnItemMaxHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GalleryGroup instance = (GalleryGroup)d;
            instance.OnItemMaxHeightChanged(e);
        }

        /// <summary>
        /// Called when [allow varying item size changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnAllowVaryingItemSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GalleryGroup instance = (GalleryGroup)d;
            instance.OnAllowVaryingItemSizeChanged(e);
        }
        /// <summary>
        /// Calls OnItemContentTemplateChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnItemContentTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GalleryGroup instance = (GalleryGroup)d;
            instance.OnItemContentTemplateChanged(e);
        }

        /// <summary>
        /// Calls OnItemContentTemplateSelectorChanged method of the
        /// instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnItemContentTemplateSelectorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GalleryGroup instance = (GalleryGroup)d;
            instance.OnItemContentTemplateSelectorChanged(e);
        }

        /// <summary>
        /// Calls OnSpaceLimitBetweenItemsChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnSpaceLimitBetweenItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GalleryGroup instance = (GalleryGroup)d;
            instance.OnSpaceLimitBetweenItemsChanged(e);
        }

        /// <summary>
        /// Calls OnVisualModeChanged method of the instance, notifies of
        /// the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnVisualModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GalleryGroup instance = (GalleryGroup)d;
            instance.CoerceValue(HorizontalAlignmentProperty);
            instance.OnVisualModeChanged(e);
        }

        /// <summary>
        /// Invoked when IsSelectedChangedEvent of item is raised.
        /// </summary>
        /// <param name="sender">GalleryGroup instance.</param>
        /// <param name="e">The instance containing the event data.</param>
        private static void IsSelectedChangedEventHandler(object sender, RoutedEventArgs e)
        {
            GalleryItem item = (GalleryItem)e.OriginalSource;
            GalleryGroup group = (GalleryGroup)sender;

            group.SetGalleryItemIsSelected(item);
        }

        /// <summary>
        /// Invoked when VisibilityChangedEvent of item is raised.
        /// </summary>
        /// <param name="sender">The item in which changes appeared.</param>
        /// <param name="e">The instance that contains the event
        /// data.</param>
        private static void VisibilityChangedEventHandler(object sender, RoutedEventArgs e)
        {
            GalleryItem item = (GalleryItem)e.OriginalSource;
            GalleryGroup group = (GalleryGroup)sender;

            if (group.m_arrangedItems != null)
            {
                if (item.Visibility != Visibility.Visible)
                {
                    if (item.HasFocus)
                    {
                        group.m_hasFocusIndex = -1;
                    }
                }

                if (item.Visibility == Visibility.Collapsed)
                {
                    if (group.m_selectionStartIndex == group.m_arrangedItems.IndexOf(item))
                    {
                        group.m_selectionStartIndex = -1;
                    }

                    group.m_arrangedItems.Remove(item);
                }

                if (item.Visibility == Visibility.Visible)
                {
                    group.m_arrangedItems = null;
                }
            }

            group.UpdateNavigationMatrix();
        }

        /// <summary>
        /// Coerces HorizontalAlignment property to stretch when
        /// VisualMode is detailed.
        /// </summary>
        /// <param name="d">Instance of GalleryGroup.</param>
        /// <param name="baseValue">Base value.</param>
        /// <returns>
        /// Horizontal alignment.
        /// </returns>
        private static object CoerceHorizontalAlignmentValueCallback(DependencyObject d, object baseValue)
        {
            GalleryGroup group = (GalleryGroup)d;

            return (group.VisualMode == GalleryVisualMode.Detailed) ? HorizontalAlignment.Stretch : baseValue;
        }

        /// <summary>
        /// Coerces VerticalAlignment property to stretch when
        /// VisualMode is detailed.
        /// </summary>
        /// <param name="d">Instance of GalleryGroup.</param>
        /// <param name="baseValue">Base value.</param>
        /// <returns>
        /// Vertical alignment.
        /// </returns>
        private static object CoerceVerticalAlignmentValueCallback(DependencyObject d, object baseValue)
        {
            GalleryGroup group = (GalleryGroup)d;

            return (group.VisualMode == GalleryVisualMode.Detailed) ? VerticalAlignment.Stretch : baseValue;
        }

        /// <summary>
        /// Updates property value cache and raises
        /// ItemContentTemplateChanged event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private void OnItemContentTemplateChanged(DependencyPropertyChangedEventArgs e)
        {
            if (ItemContentTemplateChanged != null)
            {
                ItemContentTemplateChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises
        /// ItemContentTemplateSelectorChanged event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private void OnItemContentTemplateSelectorChanged(DependencyPropertyChangedEventArgs e)
        {
            if (ItemContentTemplateSelectorChanged != null)
            {
                ItemContentTemplateSelectorChanged(this, e);
            }
        }

        /// <summary>
        /// Calls when SelectedItems collection is changed.
        /// </summary>
        /// <param name="sender">The collection in which changes
        /// appeared.</param>
        /// <param name="e">The instance that contains the event
        /// data.</param>
        private void SelectedItems_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (m_skipIt == false)
            {
                ObjectCollection objectCollection = (ObjectCollection)sender;

                m_skipIt = true;

                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        int count = objectCollection.Count;
                        GalleryItem item = (GalleryItem)objectCollection[count - 1];

                        if (item != null)
                        {
                            if (!Items.Contains(item))
                            {
                                objectCollection[count - 1] = item.Content;
                            }

                            item.IsSelected = true;
                        }

                        m_skipIt = false;
                        break;

                    case NotifyCollectionChangedAction.Remove:

                        for (int i = 0; i < e.OldItems.Count; i++)
                        {
                            GetGalleryItem(Items[Items.IndexOf(e.OldItems[i])]).IsSelected = false;
                        }

                        break;

                    case NotifyCollectionChangedAction.Reset:
                        count = Items.Count;
                        for (int i = 0; i < count; i++)
                        {
                            GalleryItem tempitem = GetGalleryItem(i);
                            if (tempitem != null)
                            {
                                tempitem.IsSelected = false;
                            }
                        }

                        m_skipIt = false;
                        break;
                }
            }
        }

        /// <summary>
        /// Sets IsSelected item property to true.
        /// </summary>
        /// <param name="item">GalleryItem item instance.</param>
        private void SetGalleryItemIsSelected(GalleryItem item)
        {
            if (m_skipIt == false)
            {
                m_skipIt = true;

                object obj = new object();

                if (!Items.Contains(item))
                {
                    obj = item.Content;
                }
                else
                {
                    obj = item;
                }

                if (item.IsSelected)
                {
                    if (!AllowMultiSelect && SelectedItems.Count > 0)
                    {
                        m_skipIt = false;
                        SelectedItems.Clear();
                    }

                    SelectedItems.Add(obj);
                }
                else
                {
                    SelectedItems.Remove(obj);
                }
            }

            m_skipIt = false;
        }

        /// <summary>
        /// Invoked when GalleryItemMouseLeftButtonDown reaches item.
        /// </summary>
        /// <param name="sender">Sender Object.</param>
        /// <param name="e">The instance that contains the event
        /// data.</param>
        private void OnGalleryItemMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!e.Handled)
            {
                OnGalleryItemClick((GalleryItem)sender);
            }
        }

        /// <summary>
        /// Updates property value cache and raises
        /// AllowedItemResizeModeChanged event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private void OnAllowedItemResizeModeChanged(DependencyPropertyChangedEventArgs e)
        {
            if (AllowedItemResizeModeChanged != null)
            {
                AllowedItemResizeModeChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises
        /// AllowMultiSelectChanged event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private void OnAllowMultiSelectChanged(DependencyPropertyChangedEventArgs e)
        {
            if (AllowMultiSelectChanged != null)
            {
                AllowMultiSelectChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises ItemMarginChanged
        /// event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private void OnItemMarginChanged(DependencyPropertyChangedEventArgs e)
        {
            if (ItemMarginChanged != null)
            {
                ItemMarginChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises ItemWidthChanged
        /// event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private void OnItemWidthChanged(DependencyPropertyChangedEventArgs e)
        {
            if (ItemWidthChanged != null)
            {
                ItemWidthChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises ItemHeightChanged
        /// event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private void OnItemHeightChanged(DependencyPropertyChangedEventArgs e)
        {
            if (ItemHeightChanged != null)
            {
                ItemHeightChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises ItemMinWidthChanged
        /// event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private void OnItemMinWidthChanged(DependencyPropertyChangedEventArgs e)
        {
            if (ItemMinWidthChanged != null)
            {
                ItemMinWidthChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises ItemMaxWidthChanged
        /// event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private void OnItemMaxWidthChanged(DependencyPropertyChangedEventArgs e)
        {
            if (ItemMaxWidthChanged != null)
            {
                ItemMaxWidthChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises ItemMinHeightChanged
        /// event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private void OnItemMinHeightChanged(DependencyPropertyChangedEventArgs e)
        {
            if (ItemMinHeightChanged != null)
            {
                ItemMinHeightChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises ItemMaxHeightChanged
        /// event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private void OnItemMaxHeightChanged(DependencyPropertyChangedEventArgs e)
        {
            if (ItemMaxHeightChanged != null)
            {
                ItemMaxHeightChanged(this, e);
            }
        }

        private void OnAllowVaryingItemSizeChanged(DependencyPropertyChangedEventArgs e)
        {
            //if (AllowVaryingItemSize)
            //{
            //    ItemWidth = ItemMaxWidth;
            //    ItemHeight = ItemMaxHeight;
            //}
            if (AllowVaryingItemSizeChanged != null)
            {
                AllowVaryingItemSizeChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises
        /// SpaceLimitBetweenItemsChanged event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private void OnSpaceLimitBetweenItemsChanged(DependencyPropertyChangedEventArgs e)
        {
            if (SpaceLimitBetweenItemsChanged != null)
            {
                SpaceLimitBetweenItemsChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises VisualModeChanged
        /// event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private void OnVisualModeChanged(DependencyPropertyChangedEventArgs e)
        {
            if (VisualModeChanged != null)
            {
                VisualModeChanged(this, e);
            }
        }

        /// <summary>
        /// Updates the navigation matrix.
        /// </summary>
        private void UpdateNavigationMatrix()
        {
            ItemsCollection arrangedCollection = ArrangedItems;
            int arrangedCollCount = arrangedCollection.Count;
            m_currentRowIndex = -1;
            m_currentColIndex = -1;

            if (arrangedCollCount != 0)
            {
                Panel panel = VisualUtils.FindAncestor(arrangedCollection[0], typeof(Panel)) as Panel;
                if (panel != null)
                {
                    double actualItemWidth = arrangedCollection[0].ActualWidth + arrangedCollection[0].Margin.Left + arrangedCollection[0].Margin.Right;
                    m_colCount = (int)(panel.ActualWidth / actualItemWidth);

                    if (m_colCount >= 0)
                    {
                        if (m_colCount == 0 && arrangedCollCount != 0)
                        {
                            m_colCount++;
                        }

                        m_rowCount = arrangedCollCount / m_colCount;

                        if (arrangedCollCount % m_colCount != 0)
                        {
                            m_rowCount++;
                        }

                        m_navigationMatrix = new GalleryItem[m_rowCount, m_colCount];
                        int temp = 0;

                        for (int i = 0; i < m_rowCount; i++)
                        {
                            for (int j = 0; j < m_colCount; j++)
                            {
                                if (j + temp >= arrangedCollCount)
                                {
                                    break;
                                }

                                if (arrangedCollection[j + temp].Visibility != Visibility.Hidden)
                                {
                                    m_navigationMatrix[i, j] = arrangedCollection[j + temp];
                                }

                                if (arrangedCollection[j + temp].HasFocus)
                                {
                                    m_currentRowIndex = i;
                                    m_currentColIndex = j;
                                }
                            }

                            temp += m_colCount;
                        }

                        temp = m_navigationMatrix.Length - arrangedCollCount;

                        if (temp != 0 && HorizontalAlignment != HorizontalAlignment.Left
                            && HorizontalAlignment != HorizontalAlignment.Stretch)
                        {
                            if (HorizontalAlignment == HorizontalAlignment.Center)
                            {
                                temp /= 2;
                            }

                            if (temp != 0)
                            {
                                if (m_currentRowIndex == m_rowCount - 1)
                                {
                                    m_currentColIndex += temp;
                                }

                                for (int j = (arrangedCollCount % m_colCount) - 1; j >= 0; j--)
                                {
                                    m_navigationMatrix[m_rowCount - 1, j + temp] = m_navigationMatrix[m_rowCount - 1, j];
                                    m_navigationMatrix[m_rowCount - 1, j] = null;
                                }
                            }
                        }

                        #region Search for first not null item if m_HasFocusIndex == -1;
                        if (m_currentRowIndex == -1 || m_currentColIndex == -1)
                        {
                            for (int i = 0; i < m_rowCount; i++)
                            {
                                for (int j = 0; j < m_colCount; j++)
                                {
                                    if (m_navigationMatrix[i, j] != null)
                                    {
                                        m_currentRowIndex = i;
                                        m_currentColIndex = j;
                                        break;
                                    }
                                }
                            }
                        }
                        #endregion
                    }
                }
            }
        }

        /// <summary>
        /// Focus right.
        /// </summary>
        /// <returns>
        /// Index of the focused item.
        /// </returns>
        private int FocusRight()
        {
            m_navigationMatrix[m_currentRowIndex, m_currentColIndex].HasFocus = false;

            if (m_currentColIndex == m_colCount - 1)
            {
                m_currentColIndex = 0;
            }
            else
            {
                m_currentColIndex++;
            }

            while (m_navigationMatrix[m_currentRowIndex, m_currentColIndex] == null)
            {
                m_currentColIndex++;
                if (m_currentColIndex >= m_colCount)
                {
                    m_currentColIndex = 0;
                }
            }

            m_navigationMatrix[m_currentRowIndex, m_currentColIndex].HasFocus = true;

            return ArrangedItems.IndexOf(m_navigationMatrix[m_currentRowIndex, m_currentColIndex]);
        }

        /// <summary>
        /// Focus left.
        /// </summary>
        /// <returns>
        /// Index of the focused item.
        /// </returns>
        private int FocusLeft()
        {
            m_navigationMatrix[m_currentRowIndex, m_currentColIndex].HasFocus = false;

            if (m_currentColIndex == 0)
            {
                m_currentColIndex = m_colCount - 1;
            }
            else
            {
                m_currentColIndex--;
            }

            while (m_navigationMatrix[m_currentRowIndex, m_currentColIndex] == null)
            {
                m_currentColIndex--;
                if (m_currentColIndex < 0)
                {
                    m_currentColIndex = m_colCount - 1;
                }
            }

            m_navigationMatrix[m_currentRowIndex, m_currentColIndex].HasFocus = true;

            return ArrangedItems.IndexOf(m_navigationMatrix[m_currentRowIndex, m_currentColIndex]);
        }

        /// <summary>
        /// Focus down.
        /// </summary>
        /// <returns>
        /// Index of the focused item.
        /// </returns>
        private int FocusDown()
        {
            m_navigationMatrix[m_currentRowIndex, m_currentColIndex].HasFocus = false;

            if (m_currentRowIndex == m_rowCount - 1)
            {
                m_currentRowIndex = 0;
            }
            else
            {
                m_currentRowIndex++;
            }

            while (m_navigationMatrix[m_currentRowIndex, m_currentColIndex] == null)
            {
                m_currentRowIndex++;
                if (m_currentRowIndex >= m_rowCount)
                {
                    m_currentRowIndex = 0;
                }
            }

            m_navigationMatrix[m_currentRowIndex, m_currentColIndex].HasFocus = true;

            return ArrangedItems.IndexOf(m_navigationMatrix[m_currentRowIndex, m_currentColIndex]);
        }

        /// <summary>
        /// Method used to Focus up.
        /// </summary>
        /// <returns>
        /// Index of the focused item.
        /// </returns>
        private int FocusUp()
        {
            m_navigationMatrix[m_currentRowIndex, m_currentColIndex].HasFocus = false;

            if (m_currentRowIndex == 0)
            {
                m_currentRowIndex = m_rowCount - 1;
            }
            else
            {
                m_currentRowIndex--;
            }

            while (m_navigationMatrix[m_currentRowIndex, m_currentColIndex] == null)
            {
                m_currentRowIndex--;
                if (m_currentRowIndex < 0)
                {
                    m_currentRowIndex = m_rowCount - 1;
                }
            }

            m_navigationMatrix[m_currentRowIndex, m_currentColIndex].HasFocus = true;

            return ArrangedItems.IndexOf(m_navigationMatrix[m_currentRowIndex, m_currentColIndex]);
        }

        /// <summary>
        /// Used for drag-and-drop.
        /// </summary>
        /// <param name="frameworkElement">Instance of the FrameworkElement
        /// class.</param>
        /// <returns>
        /// Top container where group is located.
        /// </returns>
        private IInputElement FindRootContainer(FrameworkElement frameworkElement)
        {
            FrameworkElement result = frameworkElement;

            while (result.Parent != null)
            {
                result = (FrameworkElement)result.Parent;
            }

            return (IInputElement)result;
        }

        /// <summary>
        /// Looks for GalleryItem container for object.
        /// </summary>
        /// <param name="obj">Object for which the container should be
        /// found.</param>
        /// <returns>
        /// Instance of GalleryItem class.
        /// </returns>
        private GalleryItem GetGalleryItem(object obj)
        {
            GalleryItem item = new GalleryItem();

            if (obj is GalleryItem)
            {
                item = (GalleryItem)obj;
            }
            else
            {
                item = (GalleryItem)ItemContainerGenerator.ContainerFromItem(obj);
            }

            return item;
        }

        /// <summary>
        /// Looks for GalleryItem container by index.
        /// </summary>
        /// <param name="index">Item index by which the container of GalleryItem
        /// type should be found.</param>
        /// <returns>
        /// Instance of GalleryItem class.
        /// </returns>
        private GalleryItem GetGalleryItem(int index)
        {
            GalleryItem item = new GalleryItem();

            if (Items[index] is GalleryItem)
            {
                item = (GalleryItem)Items[index];
            }
            else
            {
                item = (GalleryItem)ItemContainerGenerator.ContainerFromIndex(index);
            }

            return item;
        }

        /// <summary>
        /// Looks for GalleryItem in the visual tree.
        /// </summary>
        /// <param name="e">The instance containing the event data.</param>
        /// <returns>
        /// GalleryItem item.
        /// </returns>
        private GalleryItem GetGalleryItem(RoutedEventArgs e)
        {
            GalleryItem source = null;

            FrameworkElement originalSource = (FrameworkElement)e.OriginalSource;
            FrameworkElement templatedParent = originalSource.TemplatedParent as FrameworkElement;

            if (templatedParent == null)
            {
                bool flag = true;
                templatedParent = originalSource.Parent as FrameworkElement;
                    do
                    {
                        if (templatedParent != null)
                        {
                            if (templatedParent is GalleryItem)
                            {
                                flag = false;
                            }
                            if (templatedParent.Parent != null)
                            {
                                if (templatedParent.Parent is GalleryItem)
                                {
                                    flag = false;
                                }
                                else
                                {
                                    templatedParent = templatedParent.Parent as FrameworkElement;
                                }
                            }
                            else
                            {
                                flag = false;
                            }
                        }
                        else
                        {
                            flag = false;
                        }
                    } while (flag) ;
            }

            if (e.Source is GalleryItem)
            {
                source = (GalleryItem)e.Source;
            }
            else if ((e.Source is FrameworkElement) && ((FrameworkElement)e.Source).Parent is GalleryItem)
            {
                source = (GalleryItem)((FrameworkElement)e.Source).Parent;
            }
            else if (ItemContainerGenerator.ContainerFromItem(e.Source) is GalleryItem)
            {
                source = (GalleryItem)ItemContainerGenerator.ContainerFromItem(e.Source);
            }
            else if (originalSource.TemplatedParent is GalleryItem)
            {
                source = (GalleryItem)originalSource.TemplatedParent;
            }
            else if (templatedParent != null && templatedParent.TemplatedParent is GalleryItem)
            {
                source = (GalleryItem)templatedParent.TemplatedParent;
            }
            else if (templatedParent != null && templatedParent.Parent is GalleryItem)
            {
                source = (GalleryItem)templatedParent.Parent;
            }
            else if (templatedParent != null && templatedParent is GalleryItem)
            {
                source = (GalleryItem)templatedParent;
            }
            else
            {
               if(templatedParent!=null)
               {
                   source = VisualUtils.FindAncestor(templatedParent, typeof (GalleryItem)) as GalleryItem;
               }
            }

            return source;
        }

        /// <summary>
        /// Calls OnIsDragOverChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnIsDragOverChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GalleryGroup instance = (GalleryGroup)d;
            instance.OnIsDragOverChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="IsDragOverChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnIsDragOverChanged(DependencyPropertyChangedEventArgs e)
        {
            if (IsDragOverChanged != null)
            {
                IsDragOverChanged(this, e);
            }
        }
        #endregion

        #region	Dependency Properties
        /// <summary>
        /// Identifies <see cref="SelectedItems"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedItemsProperty =
            DependencyProperty.Register("SelectedItems", typeof(ObjectCollection), typeof(GalleryGroup), new UIPropertyMetadata(null));

        /// <summary>
        /// Identifies <see cref="ItemContentTemplate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemContentTemplateProperty =
            DependencyProperty.Register("ItemContentTemplate", typeof(DataTemplate), typeof(GalleryGroup), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnItemContentTemplateChanged)));

        /// <summary>
        /// Identifies <see cref="ItemContentTemplateSelector"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemContentTemplateSelectorProperty =
            DependencyProperty.Register("ItemContentTemplateSelector", typeof(DataTemplateSelector), typeof(GalleryGroup), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnItemContentTemplateSelectorChanged)));

        /// <summary>
        /// Identifies <see cref="IsDragOver"/> dependency property key.
        /// </summary>
        protected static readonly DependencyPropertyKey IsDragOverPropertyKey =
            DependencyProperty.RegisterReadOnly("IsDragOver", typeof(bool), typeof(GalleryGroup), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsDragOverChanged)));

        /// <summary>
        /// Identifies <see cref="IsDragOver"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsDragOverProperty = IsDragOverPropertyKey.DependencyProperty;

        /// <summary>
        /// Identifies <see cref="CaptionAlignment"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CaptionAlignmentProperty =
          DependencyProperty.Register("CaptionAlignment", typeof(CaptionAlignment), typeof(GalleryGroup), new FrameworkPropertyMetadata(CaptionAlignment.Top, new PropertyChangedCallback(OnCaptionAlignmentChanged)));

        /// <summary>
        /// Identifies <see cref="DescriptionAlignment"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DescriptionAlignmentProperty =
          DependencyProperty.Register("DescriptionAlignment", typeof(CaptionAlignment), typeof(GalleryGroup), new FrameworkPropertyMetadata(CaptionAlignment.Bottom, new PropertyChangedCallback(OnDescriptionAlignmentChanged)));

        /// <summary>
        /// Identifies the <see cref="IsAlwaysShownCaption"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsAlwaysShownCaptionProperty =
          DependencyProperty.Register("IsAlwaysShownCaption", typeof(bool), typeof(GalleryGroup), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsAlwaysShownCaptionChanged)));
        #endregion
    }
}
