// <copyright file="Gallery.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Diagnostics;
using System.Windows.Media;
using System.Windows.Interop;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Collections;
using Syncfusion.Licensing;
using Syncfusion.Windows.Shared;
using System.ComponentModel;
using System.Windows.Data;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents a Gallery control.
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
    /// <example><code>public partial class Gallery : Selector</code></example>
    /// </list>
    /// <para/>
    /// <list type="table">
    /// <listheader>
    /// <description>XAML Object Element Usage</description>
    /// </listheader>
    /// <example><code><![CDATA[<local:Gallery Name="gallery" />]]></code></example>
    /// </list>
    /// </example>
    /// </list>
    /// <remarks>
    /// UI framework element based on <see cref="Selector"/> class. Control is used for logical grouping of special items into groups.
    /// </remarks> 
    /// <example>
    /// <para/>This example shows how to create a Gallery in XAML.
    /// <code>
    /// <![CDATA[
    /// <Window x:Class="Gallery.Window1" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    /// xmlns:local="clr-namespace:Syncfusion.Windows.Tools.Controls;assembly=Syncfusion.Tools.WPF"
    /// Title="Gallery" Height="300" Width="300">
    /// <StackPanel HorizontalAlignment="Center">
    ///     <local:Gallery Name="gallery" />
    /// </StackPanel>
    /// </Window>
    /// ]]>
    /// </code>
    /// <para/>This example shows how to create a Gallery in C#.
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
    ///         }
    ///     }
    /// }
    /// </code>
    /// </example>

    [SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.Skin.Office2007Blue,
   Type = typeof(Gallery), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/Gallery/Themes/Office2007BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.Skin.Office2007Black,
    Type = typeof(Gallery), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/Gallery/Themes/Office2007BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.Skin.Office2007Silver,
    Type = typeof(Gallery), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/Gallery/Themes/Office2007SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.Skin.Office2010Blue,
    Type = typeof(Gallery), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/Gallery/Themes/Office2010BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.Skin.Office2010Black,
    Type = typeof(Gallery), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/Gallery/Themes/Office2010BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.Skin.Office2010Silver,
    Type = typeof(Gallery), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/Gallery/Themes/Office2010SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.Skin.Office2003,
     Type = typeof(Gallery), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/Gallery/Themes/Office2003Style.xaml")]
    [SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.Skin.ShinyBlue,
     Type = typeof(Gallery), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/Gallery/Themes/ShinyBlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.Skin.ShinyRed,
     Type = typeof(Gallery), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/Gallery/Themes/ShinyRedStyle.xaml")]
    [SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.Skin.SyncOrange,
     Type = typeof(Gallery), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/Gallery/Themes/SyncOrangeStyle.xaml")]
    [SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.Skin.Blend,
    Type = typeof(Gallery), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/Gallery/Themes/BlendStyle.xaml")]
    [SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.Skin.Default,
    Type = typeof(Gallery), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/Gallery/Themes/Generic.xaml")]
    [SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.Skin.VS2010,
    Type = typeof(Gallery), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/Gallery/Themes/VS2010Style.xaml")]
    [SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.Skin.Metro,
   Type = typeof(Gallery), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/Gallery/Themes/MetroStyle.xaml")]
    [SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.Skin.Transparent,
   Type = typeof(Gallery), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/Gallery/Themes/TransparentStyle.xaml")]

#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(true)]
#endif
    public partial class Gallery : Selector
    {
        #region Constants
        /// <summary>
        /// Contains path to dictionary.
        /// </summary>
        //private const string C_pathToDictionary = "pack://application:,,,/Syncfusion.Tools.WPF;component/Controls/Gallery/Themes/Generic.xaml";
        #endregion

        #region Fields
        /// <summary>
        /// Contains dragging items in MultipleDragging mode.
        /// </summary>
        internal static List<GalleryItem> M_dragArray;

        /// <summary>
        /// Contains dictionary resources of Gallery.
        /// </summary>
        //private static ResourceDictionary m_dictionary = new ResourceDictionary();
        #endregion

        #region Initialization
        /// <summary>
        /// Initializes static members of the <see cref="Gallery"/> class.
        /// </summary>
        static Gallery()
        {

            //  EnvironmentTest.ValidateLicense(typeof(Gallery));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Gallery), new FrameworkPropertyMetadata(typeof(Gallery)));
            AllowDropProperty.OverrideMetadata(typeof(Gallery), new FrameworkPropertyMetadata(true));
            FocusableProperty.OverrideMetadata(typeof(Gallery), new FrameworkPropertyMetadata(false));

            BorderThicknessProperty.OverrideMetadata(typeof(Gallery), new FrameworkPropertyMetadata(new Thickness(0), new PropertyChangedCallback(OnBorderThicknessChanged)));
            BorderBrushProperty.OverrideMetadata(typeof(Gallery), new FrameworkPropertyMetadata(c_defaultBrushValue, new PropertyChangedCallback(OnBorderBrushChanged)));
            BackgroundProperty.OverrideMetadata(typeof(Gallery), new FrameworkPropertyMetadata(c_defaultBrushValue, new PropertyChangedCallback(OnBackgroundChanged)));
            PaddingProperty.OverrideMetadata(typeof(Gallery), new FrameworkPropertyMetadata(new Thickness(0), new PropertyChangedCallback(OnPaddingChanged)));
            MarginProperty.OverrideMetadata(typeof(Gallery), new FrameworkPropertyMetadata(new Thickness(0), new PropertyChangedCallback(OnMarginChanged)));

            ItemDraggingEvent = EventManager.RegisterRoutedEvent("ItemDragging", RoutingStrategy.Bubble, typeof(ItemDragEventHandler), typeof(Gallery));
            ItemDraggedEvent = EventManager.RegisterRoutedEvent("ItemDragged", RoutingStrategy.Bubble, typeof(ItemDragEventHandler), typeof(Gallery));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Gallery"/> class.
        /// </summary>
        public Gallery()
        {
            if (EnvironmentTestTools.IsSecurityGranted)
            {
                EnvironmentTestTools.StartValidateLicense(typeof(Gallery));
            }
            //m_dictionary.Source = new Uri(C_pathToDictionary, UriKind.RelativeOrAbsolute);
        }
        #endregion

        #region	Properties

        /// <summary>
        /// Gets or sets a value that indicates the allowed animations. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="AllowedAnimations"/>
        /// <para/>        
        /// Default value is AllowedAnimations.All.
        /// </value>                
        /// <seealso cref="AllowedAnimations"/> enum.
        public AllowedAnimations AllowedAnimations
        {
            get
            {
                return (AllowedAnimations)GetValue(AllowedAnimationsProperty);
            }

            set
            {
                SetValue(AllowedAnimationsProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether more then one item can be selected in group. This is dependency property.
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
                return (bool)GetValue(AllowMultiSelectProperty);
            }

            set
            {
                SetValue(AllowMultiSelectProperty, value);
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
                return (bool)GetValue(AllowVaryingItemSizeProperty);
            }

            set
            {
                SetValue(AllowVaryingItemSizeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value that describes how item size should be changed, when gallery size is changing. 
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
                return (AllowedItemResizeModes)GetValue(AllowedItemResizeModeProperty);
            }

            set
            {
                SetValue(AllowedItemResizeModeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether drag and drop is allowed.
        /// </summary>
        /// <value>
        /// Type: <see cref="bool"/>
        /// <para/>
        /// Default value is true.
        /// </value>
        public bool CanDragDrop
        {
            get
            {
                return (bool)GetValue(CanDragDropProperty);
            }

            set
            {
                SetValue(CanDragDropProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value that represents skin name.
        /// </summary>
        public string Skin
        {
            get
            {
                return (string)GetValue(SkinProperty);
            }

            set
            {
                SetValue(SkinProperty, value);
            }
        }

        /// <summary>
        /// Gets selected group.
        /// </summary>
        /// <value>
        /// Type: <see cref="GalleryGroup"/>
        /// <para/>
        /// Gallery group that is currently selected or null.
        /// </value>
        /// <seealso cref="GalleryGroup"/> class.
        public GalleryGroup SelectedGroupItem
        {
            get
            {
                if (SelectedIndex == -1)
                {
                    return null;
                }

                GalleryGroup item = (GalleryGroup)Items[SelectedIndex];
                return item;
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
                return (double)GetValue(SpaceLimitBetweenItemsProperty);
            }

            set
            {
                SetValue(SpaceLimitBetweenItemsProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value that represents visual mode of gallery. This is dependency property.
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
                return (GalleryVisualMode)GetValue(VisualModeProperty);
            }

            set
            {
                SetValue(VisualModeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value that represents corner radius of gallery.
        /// </summary>
        /// <value>
        /// Type: <see cref="CornerRadius"/>
        /// <para/>
        /// Default value is 0.
        /// </value>
        /// <seealso cref="CornerRadius"/> class.
        public CornerRadius GalleryCornerRadius
        {
            get
            {
                return (CornerRadius)GetValue(GalleryCornerRadiusProperty);
            }

            set
            {
                SetValue(GalleryCornerRadiusProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether gallery groups headers are visible. This is dependency property.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is header visible; otherwise, <c>false</c>.
        /// Default value is true.
        /// </value>
        public bool IsHeaderVisible
        {
            get
            {
                return (bool)GetValue(IsHeaderVisibleProperty);
            }

            set
            {
                SetValue(IsHeaderVisibleProperty, value);
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
        /// Gets or sets the value that represents brush of the border around caption and description in gallery item. 
        /// This is dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// <para/>
        /// Default value is transparent brush.
        /// </value>
        public Brush CaptionBorderBrush
        {
            get
            {
                return (Brush)GetValue(CaptionBorderBrushProperty);
            }

            set
            {
                SetValue(CaptionBorderBrushProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value that represents thickness of the border around caption and description in gallery item. 
        /// This is dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Thickness"/>
        /// <para/>
        /// Default value is the thickness equal to zero.
        /// </value>
        public Thickness CaptionBorderThickness
        {
            get
            {
                return (Thickness)GetValue(CaptionBorderThicknessProperty);
            }

            set
            {
                SetValue(CaptionBorderThicknessProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value that represents brush of the caption's and description's background in gallery item. 
        /// This is dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// <para/>
        /// Default value is transparent brush.
        /// </value>
        public Brush CaptionBackground
        {
            get
            {
                return (Brush)GetValue(CaptionBackgroundProperty);
            }

            set
            {
                SetValue(CaptionBackgroundProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value that represents corner radius of the border around caption and description in gallery item. 
        /// This is dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="CornerRadius"/>
        /// <para/>
        /// Default value is the corner radius equal to zero.
        /// </value>
        public CornerRadius CaptionCornerRadius
        {
            get
            {
                return (CornerRadius)GetValue(CaptionCornerRadiusProperty);
            }

            set
            {
                SetValue(CaptionCornerRadiusProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the caption and description in gallery item is visible. 
        /// This is dependency property.
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
        /// Gets or sets the value group standard border margin in the group standard template.
        /// </summary>
        internal Thickness GroupStandardBorderMargin
        {
            get
            {
                return (Thickness)GetValue(GroupStandardBorderMarginProperty);
            }

            set
            {
                SetValue(GroupStandardBorderMarginProperty, value);
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Gets interval between groups.
        /// </summary>
        /// <param name="obj">Gallery object.</param>
        /// <returns>
        /// The size of the interval.
        /// </returns>
        public static double GetInterval(DependencyObject obj)
        {
            return (double)obj.GetValue(IntervalProperty);
        }

        /// <summary>
        /// Sets interval between groups.
        /// </summary>
        /// <param name="obj">Gallery object.</param>
        /// <param name="value">The size of the interval.</param>
        public static void SetInterval(DependencyObject obj, double value)
        {
            obj.SetValue(IntervalProperty, value);
        }

        /// <summary>
        /// Invoked when <see cref="UIElement.MouseDown"/> event is raised.
        /// </summary>
        /// <param name="e">                
        /// The instance that contains the event data.</param>
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            int selectedItemIndex = this.SelectedIndex;

            GalleryGroup group = GetSourceGroup(e);

            if (group != null)
            {
                if (selectedItemIndex != -1)
                {
                    SetIsSelected((GalleryGroup)Items[selectedItemIndex], false);
                }

                SetIsSelected(group, true);
                SelectedIndex = Items.IndexOf(group);
            }

            e.Handled = true;
        }

        /// <summary>
        /// Invoked when <see cref="UIElement.PreviewDrop"/> event is raised.
        /// </summary>
        /// <param name="e">         
        /// The instance that contains the event data.</param>
        protected override void OnPreviewDrop(DragEventArgs e)
        {
            if (e.Data.GetDataPresent("Item"))
            {
                GalleryItem item = (GalleryItem)e.Data.GetData("Item");

                int index = -1;

                GalleryGroup group = GetSourceGroup(e);

                GalleryGroup itemParent = GetItemParent(item);

                if (group != null)
                {
                    index = Items.IndexOf(group);
                }

                if (index != -1)
                {
                    OnItemDragged(item, itemParent, group);
                }
                else
                {
                    OnItemDragged(item, itemParent, null);
                }
            }

            base.OnPreviewDrop(e);
        }

        /// <summary>
        /// Indicates when item type is GalleryItem.
        /// </summary>
        /// <param name="item">Given object.</param>
        /// <returns>True if item type is GalleryItem, otherwise - false.</returns>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is GalleryGroup;
        }

        /// <summary>
        /// Gets GalleryItem container for non GalleryItem type item.
        /// </summary>
        /// <returns>
        /// New instance of GalleryItem.
        /// </returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            GalleryGroup group = new GalleryGroup();
            return group;
        }

        /// <summary>
        /// Invoked when <see cref="UIElement.Drop"/> event is raised.
        /// </summary>
        /// <param name="e">        
        /// The instance that contains the event data.</param>
        protected override void OnDrop(DragEventArgs e)
        {
            if (e.Data.GetDataPresent("Group"))
            {
                GalleryGroup group = (GalleryGroup)e.Data.GetData("Group");
                int index = -1;

                if (e.Source is GalleryGroup)
                {
                    index = Items.IndexOf(e.Source);
                }
                else
                {
                    index = FindIndexToInsert(e.GetPosition(this));
                }

                Gallery gall = (Gallery)Gallery.FindAncestor(typeof(Gallery), group);

                if (gall != null && gall.ItemsSource != null)
                {
                    Gallery.RemoveFromItemSource(gall.ItemsSource, group.Header);


                }
                else
                {
                    gall.Items.Remove(group);
                }

                if (ItemsSource != null)
                {
                    IList list = ItemsSource as IList;
                    if (list != null)
                    {
                        if (index > -1 && index < Items.Count)
                        {
                            if (group is GalleryGroup)
                            {
                                list.Insert(index, group.Header);
                            }
                            else
                            {
                                list.Insert(index, group);
                            }
                        }
                        else
                        {
                            if (group is GalleryGroup)
                            {
                                list.Add(group.Header);
                            }
                            else
                            {
                                list.Add(group);
                            }
                        }
                    }
                }
                else
                {
                    if (index > -1 && index < Items.Count)
                    {
                        Items.Insert(index, group);
                    }
                    else
                    {
                        Items.Add(group);
                    }
                }

                group.Opacity = 1;
                SelectedIndex = Items.IndexOf(group);
            }

            base.OnDrop(e);
        }

        /// <summary>
        /// Removes from item source.
        /// </summary>
        /// <param name="itemsSource">The items source.</param>
        /// <param name="index">The index.</param>
        private static void RemoveFromItemSource(IEnumerable itemsSource, int index)
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
        private static void RemoveFromItemSource(IEnumerable itemsSource, object obj)
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
        /// Invoked when <see cref="UIElement.DragOver"/> event is raised.
        /// </summary>
        /// <param name="e">        
        /// The instance that contains the event data.</param>
        protected override void OnDragOver(DragEventArgs e)
        {
            if (e.Data.GetDataPresent("Item"))
            {
                GalleryItem item = (GalleryItem)e.Data.GetData("Item");

                GalleryGroup group = GetSourceGroup(e);

                GalleryGroup itemParent = GetItemParent(item);

                int index = -1;

                if (group != null)
                {
                    index = Items.IndexOf(group);
                }

                if (index != -1)
                {
                    OnItemDragging(item, itemParent, group);
                }
                else
                {
                    OnItemDragging(item, itemParent, null);
                }
            }

            base.OnDragOver(e);
        }

        /// <summary>
        /// Updates the current selection when an item in the <see cref="T:System.Windows.Controls.Primitives.Selector"/> has changed
        /// </summary>
        /// <param name="e">        
        /// The instance that contains the event data.</param>
        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            //if (NotifyCollectionChangedAction.Add == e.Action)
            //{
            //    IList list = e.NewItems;
            //    GalleryGroup galleryGroup = (GalleryGroup)list[list.Count - 1];
            //    galleryGroup.Focus();
            //}

            base.OnItemsChanged(e);
        }

        /// <summary>
        /// Raises <see cref="ItemDraggingEvent"/> event.
        /// </summary>
        /// <param name="item">Dragging item.</param>
        /// <param name="sourceGroup">Current parent.</param>
        /// <param name="targetGroup">The group above which the element
        /// is dragging.</param>
        protected virtual void OnItemDragging(GalleryItem item, GalleryGroup sourceGroup, GalleryGroup targetGroup)
        {
            ItemDragEventArgs args = new ItemDragEventArgs();
            args.Item = item;
            args.SourceGroup = sourceGroup;
            args.TargetGroup = targetGroup;
            args.RoutedEvent = Gallery.ItemDraggingEvent;
            args.Source = this;
            RaiseEvent(args);
        }

        /// <summary>
        /// Raises <see cref="ItemDraggedEvent"/> event.
        /// </summary>
        /// <param name="item">Dragged item.</param>
        /// <param name="sourceGroup">Old item parent.</param>
        /// <param name="targetGroup">New item parent.</param>
        protected virtual void OnItemDragged(GalleryItem item, GalleryGroup sourceGroup, GalleryGroup targetGroup)
        {
            ItemDragEventArgs args = new ItemDragEventArgs();
            args.Item = item;
            args.SourceGroup = sourceGroup;
            args.TargetGroup = targetGroup;
            args.RoutedEvent = Gallery.ItemDraggedEvent;
            args.Source = this;
            RaiseEvent(args);
        }

        /// <summary>
        /// Calls OnAllowedAnimationsChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnAllowedAnimationsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Gallery instance = (Gallery)d;
            instance.OnAllowedAnimationsChanged(e);
        }

        /// <summary>
        /// Calls OnAllowMultiSelectChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnAllowMultiSelectChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Gallery instance = (Gallery)d;
            instance.OnAllowMultiSelectChanged(e);
        }

        /// <summary>
        /// Called when [allow varying item size changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnAllowVaryingItemSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Gallery instance = (Gallery)d;
            instance.OnAllowVaryingItemSizeChanged(e);
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
            Gallery instance = (Gallery)d;
            instance.OnAllowedItemResizeModeChanged(e);
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
            Gallery instance = (Gallery)d;
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
            Gallery instance = (Gallery)d;
            instance.OnVisualModeChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises
        /// AllowedAnimationsChanged event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private void OnAllowedAnimationsChanged(DependencyPropertyChangedEventArgs e)
        {
            if (AllowedAnimationsChanged != null)
            {
                AllowedAnimationsChanged(this, e);
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
        /// Raises the <see cref="E:AllowVaryingItemSizeChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
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
        /// Used while drag-&amp;-drop.
        /// </summary>
        /// <param name="point">Current mouse position.</param>
        /// <returns>
        /// Returns index for group which should be dropped.
        /// </returns>
        private int FindIndexToInsert(Point point)
        {
            int i = -1;

            if (Items.Count > 0)
            {
                double temp = 0d;

                if (ItemContainerGenerator.ContainerFromIndex(0) is GalleryGroup)
                {
                    temp = ((GalleryGroup)ItemContainerGenerator.ContainerFromIndex(0)).DesiredSize.Height;
                }
                else
                {
                    temp = 0;
                }
                double interval = GetInterval(this);

                for (i = 1; i < Items.Count; i++)
                {
                    if (point.Y > temp && point.Y < temp + interval)
                    {
                        return i;
                    }
                    if (ItemContainerGenerator.ContainerFromIndex(i) is GalleryGroup)
                    {
                        temp += interval + ((GalleryGroup)ItemContainerGenerator.ContainerFromIndex(i)).DesiredSize.Height;
                    }
                    else
                    {
                        temp += interval + 0;
                    }
                }
            }

            return i;
        }

        /// <summary>
        /// Used for drag-&amp;-drop.
        /// </summary>
        /// <param name="e">The source that contains the event data.</param>
        /// <returns>
        /// GalleryGroup source type.
        /// </returns>
        private GalleryGroup GetSourceGroup(RoutedEventArgs e)
        {
            GalleryGroup group = null;
            FrameworkElement originalSource = (FrameworkElement)e.OriginalSource;
            group = FindAncestor(typeof(GalleryGroup), originalSource) as GalleryGroup;
            return group;
        }

        /// <summary>
        /// Used for gallery group items drag-&amp;-drop between groups.
        /// </summary>
        /// <param name="item">GalleryItem class instance.</param>
        /// <returns>
        /// Item parent.
        /// </returns>
        private GalleryGroup GetItemParent(GalleryItem item)
        {
            GalleryGroup itemParent = (GalleryGroup)item.Parent;

            if (itemParent == null)
            {
                object obj = item.Content;
                if (obj is FrameworkElement)
                {
                    itemParent = (GalleryGroup)((FrameworkElement)obj).Parent;
                }
                else
                {
                    int count = Items.Count;
                    for (int i = 0; i < count; i++)
                    {
                        GalleryGroup tempGroup = (GalleryGroup)Items[i];
                        if (tempGroup.Items.Contains(obj))
                        {
                            itemParent = tempGroup;
                            break;
                        }
                    }
                }
            }

            return itemParent;
        }

        /// <summary>
        /// Raises <see cref="GalleryCornerRadiusChanged"/> event.
        /// </summary>
        /// <param name="e">        
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnGalleryCornerRadiusChanged(DependencyPropertyChangedEventArgs e)
        {
            if (GalleryCornerRadiusChanged != null)
            {
                GalleryCornerRadiusChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnGalleryCornerRadiusChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnGalleryCornerRadiusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Gallery instance = (Gallery)d;
            instance.OnGalleryCornerRadiusChanged(e);
        }

        /// <summary>
        /// Raises <see cref="BorderThicknessChanged"/> event.
        /// </summary>
        /// <param name="e">        
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnBorderThicknessChanged(DependencyPropertyChangedEventArgs e)
        {
            if (BorderThicknessChanged != null)
            {
                BorderThicknessChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnBorderThicknessChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnBorderThicknessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Gallery instance = (Gallery)d;
            instance.OnBorderThicknessChanged(e);
        }

        /// <summary>
        /// Raises <see cref="BorderBrushChanged"/> event.
        /// </summary>
        /// <param name="e">        
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnBorderBrushChanged(DependencyPropertyChangedEventArgs e)
        {
            if (BorderBrushChanged != null)
            {
                BorderBrushChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnBorderBrushChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnBorderBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Gallery instance = (Gallery)d;
            instance.OnBorderBrushChanged(e);
        }

        /// <summary>
        /// Raises <see cref="BackgroundChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnBackgroundChanged(DependencyPropertyChangedEventArgs e)
        {
            if (BackgroundChanged != null)
            {
                BackgroundChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnBackgroundChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Gallery instance = (Gallery)d;
            instance.OnBackgroundChanged(e);
        }

        /// <summary>
        /// Raises <see cref="PaddingChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnPaddingChanged(DependencyPropertyChangedEventArgs e)
        {
            if (PaddingChanged != null)
            {
                PaddingChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnPaddingChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnPaddingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Gallery instance = (Gallery)d;
            instance.OnPaddingChanged(e);
        }

        /// <summary>
        /// Raises <see cref="MarginChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnMarginChanged(DependencyPropertyChangedEventArgs e)
        {
            if (MarginChanged != null)
            {
                MarginChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="IsHeaderVisibleChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnIsHeaderVisibleChanged(DependencyPropertyChangedEventArgs e)
        {
            if (IsHeaderVisibleChanged != null)
            {
                IsHeaderVisibleChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnMarginChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnMarginChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Gallery instance = (Gallery)d;
            instance.OnMarginChanged(e);
        }

        /// <summary>
        /// Calls OnIsHeaderVisibleChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnIsHeaderVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Gallery instance = (Gallery)d;
            instance.OnIsHeaderVisibleChanged(e);
        }

        /// <summary>
        /// Calls OnCaptionAlignmentChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnCaptionAlignmentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Gallery instance = (Gallery)d;
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
            Gallery instance = (Gallery)d;
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
        /// Calls OnCaptionBorderBrushChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnCaptionBorderBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Gallery instance = (Gallery)d;
            instance.OnCaptionBorderBrushChanged(e);
        }

        /// <summary>
        /// Raises <see cref="CaptionBorderBrushChanged"/> event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnCaptionBorderBrushChanged(DependencyPropertyChangedEventArgs e)
        {
            if (CaptionBorderBrushChanged != null)
            {
                CaptionBorderBrushChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnCaptionBorderThicknessChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnCaptionBorderThicknessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Gallery instance = (Gallery)d;
            instance.OnCaptionBorderThicknessChanged(e);
        }

        /// <summary>
        /// Raises <see cref="CaptionBorderThicknessChanged"/> event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnCaptionBorderThicknessChanged(DependencyPropertyChangedEventArgs e)
        {
            if (CaptionBorderThicknessChanged != null)
            {
                CaptionBorderThicknessChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnCaptionBackgroundChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnCaptionBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Gallery instance = (Gallery)d;
            instance.OnCaptionBackgroundChanged(e);
        }

        /// <summary>
        /// Raises <see cref="CaptionBackgroundChanged"/> event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnCaptionBackgroundChanged(DependencyPropertyChangedEventArgs e)
        {
            if (CaptionBackgroundChanged != null)
            {
                CaptionBackgroundChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnCaptionCornerRadiusChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnCaptionCornerRadiusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Gallery instance = (Gallery)d;
            instance.OnCaptionCornerRadiusChanged(e);
        }

        /// <summary>
        /// Called when caption corner radius changed
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnCaptionCornerRadiusChanged(DependencyPropertyChangedEventArgs e)
        {
            if (CaptionCornerRadiusChanged != null)
            {
                CaptionCornerRadiusChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnIsAlwaysShownCaptionChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnIsAlwaysShownCaptionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Gallery instance = (Gallery)d;
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
        /// Coerces the Value.
        /// </summary>
        /// <param name="d">Sender object</param>
        /// <param name="baseValue">BaseValue object</param>
        /// <returns>Coerced value</returns>
        private static object CoerceCanDragDrop(DependencyObject d, object baseValue)
        {
            return (BrowserInteropHelper.IsBrowserHosted == true) ? false : (bool)baseValue;
        }

        /// <summary>
        /// Calls OnCanDragDropChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnCanDragDropChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Gallery instance = (Gallery)d;
            instance.OnCanDragDropChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="CanDragDropChanged"/> event.
        /// </summary>
        /// <param name="e">        
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnCanDragDropChanged(DependencyPropertyChangedEventArgs e)
        {
            if (CanDragDropChanged != null)
            {
                CanDragDropChanged(this, e);
            }
        }

        /// <summary>
        /// Finds the ancestor.
        /// </summary>
        /// <param name="ancestorType">Type of the ancestor.</param>
        /// <param name="visual">The visual.</param>
        /// <returns> Framework Element </returns>
        public static FrameworkElement FindAncestor(Type ancestorType, Visual visual)
        {
            while (visual != null && !ancestorType.IsInstanceOfType(visual))
            {
                visual = (Visual)VisualTreeHelper.GetParent(visual);
            }

            return visual as FrameworkElement;
        }
        #endregion

        #region	Events
        /// <summary>
        /// Event that is raised when <see cref="AllowedAnimations"/> property is
        /// changed.
        /// </summary>
        public event PropertyChangedCallback AllowedAnimationsChanged;

        /// <summary>       
        /// Event that is raised when <see cref="AllowMultiSelect"/> property is
        /// changed.
        /// </summary>
        public event PropertyChangedCallback AllowMultiSelectChanged;

        /// <summary>
        /// Occurs when [allow varying item size changed].
        /// </summary>
        public event PropertyChangedCallback AllowVaryingItemSizeChanged;

        /// <summary>
        /// Event that is raised when <see cref="AllowedItemResizeMode"/> property is
        /// changed.
        /// </summary>
        public event PropertyChangedCallback AllowedItemResizeModeChanged;

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
        /// Event that is raised when <see cref="GalleryCornerRadius"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback GalleryCornerRadiusChanged;

        /// <summary>
        /// Event that is raised when Border Thickness property is changed.
        /// </summary>
        public event PropertyChangedCallback BorderThicknessChanged;

        /// <summary>
        /// Event that is raised when BorderBrush property is changed.
        /// </summary>
        public event PropertyChangedCallback BorderBrushChanged;

        /// <summary>
        /// Event that is raised when Background property is changed.
        /// </summary>
        public event PropertyChangedCallback BackgroundChanged;

        /// <summary>
        /// Event that is raised when Padding property is changed.
        /// </summary>
        public event PropertyChangedCallback PaddingChanged;

        /// <summary>
        /// Event that is raised when Margin property is changed.
        /// </summary>
        public event PropertyChangedCallback MarginChanged;

        /// <summary>
        /// Event that is raised when <see cref="IsHeaderVisible"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback IsHeaderVisibleChanged;

        /// <summary>
        /// Event that is raised when <see cref="CanDragDrop"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback CanDragDropChanged;

        /// <summary>
        /// Event that is raised when <see cref="CaptionAlignment"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback CaptionAlignmentChanged;

        /// <summary>
        /// Event that is raised when <see cref="DescriptionAlignment"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback DescriptionAlignmentChanged;

        /// <summary>
        /// Event that is raised when <see cref="CaptionBorderBrush"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback CaptionBorderBrushChanged;

        /// <summary>
        /// Event that is raised when <see cref="CaptionBorderThickness"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback CaptionBorderThicknessChanged;

        /// <summary>
        /// Event that is raised when <see cref="CaptionBackground"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback CaptionBackgroundChanged;

        /// <summary>
        /// Event that is raised when <see cref="CaptionCornerRadius"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback CaptionCornerRadiusChanged;

        /// <summary>
        /// Event that is raised when <see cref="IsAlwaysShownCaption"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback IsAlwaysShownCaptionChanged;

        #endregion

        #region	Dependency properties
        /// <summary>
        /// Identifies <see cref="AllowMultiSelect"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AllowMultiSelectProperty =
            DependencyProperty.Register("AllowMultiSelect", typeof(bool), typeof(Gallery), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.Inherits, new PropertyChangedCallback(OnAllowMultiSelectChanged)));

        /// <summary>
        /// Identifies <see cref="AllowVaryingItemSize"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AllowVaryingItemSizeProperty =
            DependencyProperty.Register("AllowVaryingItemSize", typeof(bool), typeof(Gallery), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits, new PropertyChangedCallback(OnAllowVaryingItemSizeChanged)));

        /// <summary>
        /// Identifies <see cref="AllowedAnimations"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AllowedAnimationsProperty =
            DependencyProperty.Register("AllowedAnimations", typeof(AllowedAnimations), typeof(Gallery), new FrameworkPropertyMetadata(AllowedAnimations.All, FrameworkPropertyMetadataOptions.Inherits, new PropertyChangedCallback(OnAllowedAnimationsChanged)));

        /// <summary>
        /// Identifies <see cref="AllowedItemResizeMode"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AllowedItemResizeModeProperty =
            DependencyProperty.Register("AllowedItemResizeMode", typeof(AllowedItemResizeModes), typeof(Gallery), new FrameworkPropertyMetadata(AllowedItemResizeModes.None, FrameworkPropertyMetadataOptions.Inherits, new PropertyChangedCallback(OnAllowedItemResizeModeChanged)));

        /// <summary>
        /// Identifies <see cref="CanDragDrop"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CanDragDropProperty =
            DependencyProperty.Register("CanDragDrop", typeof(bool), typeof(Gallery), new FrameworkPropertyMetadata(true, new PropertyChangedCallback(OnCanDragDropChanged), new CoerceValueCallback(CoerceCanDragDrop)));

        /// <summary>
        /// Identifies <see cref="Skin"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SkinProperty =
            DependencyProperty.Register("Skin", typeof(string), typeof(Gallery), new UIPropertyMetadata("Default"));

        /// <summary>
        /// Identifies <see cref="SpaceLimitBetweenItems"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SpaceLimitBetweenItemsProperty =
            DependencyProperty.Register("SpaceLimitBetweenItems", typeof(double), typeof(Gallery), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.Inherits, new PropertyChangedCallback(OnSpaceLimitBetweenItemsChanged)));

        /// <summary>
        /// Identifies <see cref="VisualMode"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty VisualModeProperty =
            DependencyProperty.Register("VisualMode", typeof(GalleryVisualMode), typeof(Gallery), new FrameworkPropertyMetadata(GalleryVisualMode.Standard, FrameworkPropertyMetadataOptions.Inherits, new PropertyChangedCallback(OnVisualModeChanged)));

        /// <summary>
        /// Identifies <see cref="Interval"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IntervalProperty =
            DependencyProperty.RegisterAttached("Interval", typeof(double), typeof(Gallery), new UIPropertyMetadata(10d));

        /// <summary>
        /// Identifies <see cref="GalleryCornerRadius"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty GalleryCornerRadiusProperty =
            DependencyProperty.Register("GalleryCornerRadius", typeof(CornerRadius), typeof(Gallery), new FrameworkPropertyMetadata(new CornerRadius(0), new PropertyChangedCallback(OnGalleryCornerRadiusChanged)));

        /// <summary>
        /// Identifies <see cref="IsHeaderVisible"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsHeaderVisibleProperty =
            DependencyProperty.Register("IsHeaderVisible", typeof(bool), typeof(Gallery), new FrameworkPropertyMetadata(true, new PropertyChangedCallback(OnIsHeaderVisibleChanged)));

        /// <summary>
        /// Identifies <see cref="CaptionAlignment"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CaptionAlignmentProperty =
          DependencyProperty.Register("CaptionAlignment", typeof(CaptionAlignment), typeof(Gallery), new FrameworkPropertyMetadata(CaptionAlignment.Top, new PropertyChangedCallback(OnCaptionAlignmentChanged)));

        /// <summary>
        /// Identifies <see cref="DescriptionAlignment"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DescriptionAlignmentProperty =
          DependencyProperty.Register("DescriptionAlignment", typeof(CaptionAlignment), typeof(Gallery), new FrameworkPropertyMetadata(CaptionAlignment.Bottom, new PropertyChangedCallback(OnDescriptionAlignmentChanged)));

        /// <summary>
        /// Identifies <see cref="CaptionBorderBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CaptionBorderBrushProperty =
          DependencyProperty.Register("CaptionBorderBrush", typeof(Brush), typeof(Gallery), new FrameworkPropertyMetadata(new SolidColorBrush(), new PropertyChangedCallback(OnCaptionBorderBrushChanged)));

        /// <summary>
        /// Identifies <see cref="CaptionBorderThickness"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CaptionBorderThicknessProperty =
          DependencyProperty.Register("CaptionBorderThickness", typeof(Thickness), typeof(Gallery), new FrameworkPropertyMetadata(new Thickness(0), new PropertyChangedCallback(OnCaptionBorderThicknessChanged)));

        /// <summary>
        /// Identifies <see cref="CaptionBackground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CaptionBackgroundProperty =
          DependencyProperty.Register("CaptionBackground", typeof(Brush), typeof(Gallery), new FrameworkPropertyMetadata(new SolidColorBrush(), new PropertyChangedCallback(OnCaptionBackgroundChanged)));

        /// <summary>
        /// Identifies <see cref="CaptionCornerRadius"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CaptionCornerRadiusProperty =
          DependencyProperty.Register("CaptionCornerRadius", typeof(CornerRadius), typeof(Gallery), new FrameworkPropertyMetadata(new CornerRadius(0), new PropertyChangedCallback(OnCaptionCornerRadiusChanged)));

        /// <summary>
        /// Identifies the <see cref="IsAlwaysShownCaption"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsAlwaysShownCaptionProperty =
          DependencyProperty.Register("IsAlwaysShownCaption", typeof(bool), typeof(Gallery), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsAlwaysShownCaptionChanged)));

        /// <summary>
        /// Identifies the <see cref="GroupStandardBorderMargin"/> dependency property.
        /// </summary>
        internal static readonly DependencyProperty GroupStandardBorderMarginProperty =
            DependencyProperty.Register("GroupStandardBorderMargin", typeof(Thickness), typeof(Gallery), new FrameworkPropertyMetadata(new Thickness(1, 0, 1, 3)));

        #endregion

        #region	Routed events

        /// <summary>
        /// Occurs when group item is dragging.
        /// </summary>
        public static readonly RoutedEvent ItemDraggingEvent;

        /// <summary>
        /// Occurs when group item is dragged.
        /// </summary>
        public static readonly RoutedEvent ItemDraggedEvent;
        #endregion
    }
}
