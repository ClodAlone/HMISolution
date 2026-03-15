#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls.Primitives;
using System.Collections;
using System.Windows.Data;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Windows.Media.Imaging;
using System.Windows.Browser;
using System.ComponentModel;
using System.Windows.Threading;
using Syncfusion.Windows.Controls.Theming;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// 
    /// </summary>
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Blend,
      Type = typeof(TreeViewItemAdv), XamlResource = "/Syncfusion.Theming.Blend;component/TreeViewAdv.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Blue,
        Type = typeof(TreeViewItemAdv), XamlResource = "/Syncfusion.Theming.Office2007Blue;component/TreeViewAdv.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Black,
        Type = typeof(TreeViewItemAdv), XamlResource = "/Syncfusion.Theming.Office2007Black;component/TreeViewAdv.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Silver,
        Type = typeof(TreeViewItemAdv), XamlResource = "/Syncfusion.Theming.Office2007Silver;component/TreeViewAdv.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Blue,
        Type = typeof(TreeViewItemAdv), XamlResource = "/Syncfusion.Theming.Office2010Blue;component/TreeViewAdv.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Black,
        Type = typeof(TreeViewItemAdv), XamlResource = "/Syncfusion.Theming.Office2010Black;component/TreeViewAdv.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Silver,
        Type = typeof(TreeViewItemAdv), XamlResource = "/Syncfusion.Theming.Office2010Silver;component/TreeViewAdv.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Default,
        Type = typeof(TreeViewItemAdv), XamlResource = "/Syncfusion.Theming.Default;component/TreeViewAdv.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2003,
        Type = typeof(TreeViewItemAdv), XamlResource = "/Syncfusion.Theming.Office2003;component/TreeViewAdv.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Windows7,
        Type = typeof(TreeViewItemAdv), XamlResource = "/Syncfusion.Theming.Windows7;component/TreeViewAdv.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.VS2010,
      Type = typeof(TreeViewItemAdv), XamlResource = "/Syncfusion.Theming.VS2010;component/TreeViewAdv.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Metro,
  Type = typeof(TreeViewItemAdv), XamlResource = "/Syncfusion.Theming.Metro;component/TreeViewAdv.xaml")]
    
    [TemplatePart(Name = TreeViewItemAdv.ElementRootName, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = TreeViewItemAdv.ItemsPresenterName, Type = typeof(ItemsPresenter))]
    [TemplatePart(Name = TreeViewItemAdv.ExpanderName, Type = typeof(ToggleButton))]
    [TemplateVisualState(Name = "MouseOver", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "SelectedMouseOver", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "UnFocusedSelection", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "Normal", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "Selected", GroupName = "SelectionStates")]
    [TemplateVisualState(Name = "Unselected", GroupName = "SelectionStates")]
    [TemplateVisualState(Name = "Collapsed", GroupName = "ItemStates")]
    [TemplateVisualState(Name = "Expanded", GroupName = "ItemStates")]
    [TemplateVisualState(Name = "HasItems", GroupName = "SourceStates")]
    [TemplateVisualState(Name = "NoItems", GroupName = "SourceStates")]

    public class TreeViewItemAdv : HeaderedItemsControl
    {
        #region constants
        Grid treeviewitemgrid = null;
        double horLineAlignment = 0.0;
        #endregion

        #region Dependency properties

        // Using a DependencyProperty as the backing store for VerticalLine3Height.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty VerticalLine3HeightProperty =
            DependencyProperty.Register("VerticalLine3Height", typeof(double), typeof(TreeViewItemAdv), new PropertyMetadata(0.0));

        // Using a DependencyProperty as the backing store for RootLineMargin.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty RootLineMarginProperty =
            DependencyProperty.Register("RootLineMargin", typeof(Thickness), typeof(TreeViewItemAdv), new PropertyMetadata(new Thickness(0)));

        // Using a DependencyProperty as the backing store for VerticalRootLine3Visibility.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty VerticalRootLine3VisibilityProperty =
            DependencyProperty.Register("VerticalRootLine3Visibility", typeof(Visibility), typeof(TreeViewItemAdv), new PropertyMetadata(Visibility.Visible));

        // Using a DependencyProperty as the backing store for VerticalRootLine1Visibility.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty VerticalRootLine1VisibilityProperty =
            DependencyProperty.Register("VerticalRootLine1Visibility", typeof(Visibility), typeof(TreeViewItemAdv), new PropertyMetadata(Visibility.Visible));

        // Using a DependencyProperty as the backing store for VerticalRootLine2Visibility.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty VerticalRootLine2VisibilityProperty =
            DependencyProperty.Register("VerticalRootLine2Visibility", typeof(Visibility), typeof(TreeViewItemAdv), new PropertyMetadata(Visibility.Visible));

        // Using a DependencyProperty as the backing store for HeaderMargin.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty HeaderMarginProperty =
            DependencyProperty.Register("HeaderMargin", typeof(Thickness), typeof(TreeViewItemAdv), new PropertyMetadata(new Thickness(0, 0, 0, 0)));

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty ExpanderVisibilityProperty =
            DependencyProperty.Register("ExpanderVisibility", typeof(Visibility), typeof(TreeViewItemAdv), new PropertyMetadata(Visibility.Visible, new PropertyChangedCallback(OnExpanderVisibilityChanged)));

        /// <summary>
        /// This Property determines whether the particular Node  has children
        /// </summary>
        public static readonly DependencyProperty HasItemsProperty =
              DependencyProperty.Register("HasItems", typeof(bool), typeof(TreeViewItemAdv), new PropertyMetadata(false, new PropertyChangedCallback(HasItemsChangedCallback)));

        /// <summary>
        /// This Property contains the Text to be displayed
        /// </summary>
        public new static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(object), typeof(TreeViewItemAdv), new PropertyMetadata(null, new PropertyChangedCallback(OnHeaderChanged)));

        /// <summary>
        /// This Property indicates the Template for the Header part of the control.
        /// </summary>
        public new static readonly DependencyProperty HeaderTemplateProperty =
            DependencyProperty.Register("HeaderTemplate", typeof(DataTemplate), typeof(TreeViewItemAdv), new PropertyMetadata(null));

        /// <summary>
        /// This Property determines whether the Item is expanded or not.
        /// </summary>
        public static readonly DependencyProperty IsExpandedProperty =
            DependencyProperty.Register("IsExpanded", typeof(bool), typeof(TreeViewItemAdv), new PropertyMetadata(new PropertyChangedCallback(OnIsExpandedChanged)));

        /// <summary>
        /// This Property determines whether the mouse is over the particular Item.
        /// </summary>
        public static readonly DependencyProperty IsMouseOverProperty =
            DependencyProperty.Register("IsMouseOver", typeof(bool), typeof(TreeViewItemAdv), new PropertyMetadata(new PropertyChangedCallback(OnIsMouseOverChanged)));

        /// <summary>
        /// This Property determines whether the Item  is selected or not.
        /// </summary>
        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(TreeViewItemAdv), new PropertyMetadata(new PropertyChangedCallback(IsSelectedChangedCallback)));

        /// <summary>
        /// This Property determines whether the item should be selected when the expansion changed
        /// </summary>
        public static readonly DependencyProperty SelectOnExpandChangeProperty =
            DependencyProperty.Register("SelectOnExpandChange", typeof(bool?), typeof(TreeViewItemAdv), new PropertyMetadata(new PropertyChangedCallback(OnSelectOnExpandChangeChanged)));

        /// <summary>
        /// This Property indicates the source for the Left image of the item.
        /// </summary>
        public static readonly DependencyProperty LeftImageSourceProperty =
            DependencyProperty.Register("LeftImageSource", typeof(ImageSource), typeof(TreeViewItemAdv), new PropertyMetadata(null, new PropertyChangedCallback(OnLeftImageSourceChanged)));

        /// <summary>
        /// This Property indicates the source for the Right image of the item.
        /// </summary>
        public static readonly DependencyProperty RightImageSourceProperty =
            DependencyProperty.Register("RightImageSource", typeof(ImageSource), typeof(TreeViewItemAdv), new PropertyMetadata(null, new PropertyChangedCallback(OnRightImageSourceChanged)));

        /// <summary>
        /// This Property indicates the Width for the Left image of the item.
        /// </summary>
        public static readonly DependencyProperty LeftImageWidthProperty =
            DependencyProperty.Register("LeftImageWidth", typeof(double), typeof(TreeViewItemAdv), new PropertyMetadata(15d, new PropertyChangedCallback(OnLeftImageWidthChanged)));

        /// <summary>
        /// This Property indicates the Width for the Right image of the item.
        /// </summary>
        public static readonly DependencyProperty RightImageWidthProperty =
            DependencyProperty.Register("RightImageWidth", typeof(double), typeof(TreeViewItemAdv), new PropertyMetadata(15d, new PropertyChangedCallback(OnRightImageWidthChanged)));

        /// <summary>
        /// This Property indicates the Height for the Left image of the item.
        /// </summary>
        public static readonly DependencyProperty LeftImageHeightProperty =
            DependencyProperty.Register("LeftImageHeight", typeof(double), typeof(TreeViewItemAdv), new PropertyMetadata(15d, new PropertyChangedCallback(OnLeftImageHeightChanged)));

        /// <summary>
        /// This Property indicates the Height for the Right image of the item.
        /// </summary>
        public static readonly DependencyProperty RightImageHeightProperty =
            DependencyProperty.Register("RightImageHeight", typeof(double), typeof(TreeViewItemAdv), new PropertyMetadata(15d, new PropertyChangedCallback(OnRightImageHeightChanged)));

        /// <summary>
        /// This Property indicates the Horizontal Alignment for the image of the item.
        /// </summary>
        public static readonly DependencyProperty ImageHorizontalAlignmentProperty =
            DependencyProperty.Register("ImageHorizontalAlignment", typeof(HorizontalAlignment), typeof(TreeViewItemAdv), new PropertyMetadata(HorizontalAlignment.Center, new PropertyChangedCallback(OnImageHorizontalAlignmentChanged)));

        /// <summary>
        /// This Property indicates the Vertical Alignment for the image of the item.
        /// </summary>
        public static readonly DependencyProperty ImageVerticalAlignmentProperty =
            DependencyProperty.Register("ImageVerticalAlignment", typeof(VerticalAlignment), typeof(TreeViewItemAdv), new PropertyMetadata(VerticalAlignment.Center, new PropertyChangedCallback(OnImageVerticalAlignmentChanged)));

        /// <summary>
        /// This Property indicates the Margin for the image of the item.
        /// </summary>
        public static readonly DependencyProperty ImageMarginProperty =
            DependencyProperty.Register("ImageMargin", typeof(Thickness), typeof(TreeViewItemAdv), new PropertyMetadata(new PropertyChangedCallback(OnImageMarginChanged)));

        /// <summary>
        /// This Property indicates the Horizontal Alignment for the Content 
        /// </summary>
        public static readonly DependencyProperty TextHorizontalAlignmentProperty =
            DependencyProperty.Register("TextHorizontalAlignment", typeof(HorizontalAlignment), typeof(TreeViewItemAdv), new PropertyMetadata(HorizontalAlignment.Center, new PropertyChangedCallback(OnTextHorizontalAlignmentChanged)));

        /// <summary>
        /// This Property indicates the Vertical Alignment for the Content 
        /// </summary>
        public static readonly DependencyProperty TextVerticalAlignmentProperty =
            DependencyProperty.Register("TextVerticalAlignment", typeof(VerticalAlignment), typeof(TreeViewItemAdv), new PropertyMetadata(VerticalAlignment.Center, new PropertyChangedCallback(OnTextVerticalAlignmentChanged)));

        /// <summary>
        /// This Property indicates the Margin for the Content 
        /// </summary>
        public static readonly DependencyProperty TextMarginProperty =
            DependencyProperty.Register("TextMargin", typeof(Thickness), typeof(TreeViewItemAdv), new PropertyMetadata(new PropertyChangedCallback(OnTextMarginChanged)));

        /// <summary>
        /// This property indicates the template for the toggle button.
        /// </summary>
        public static readonly DependencyProperty ExpanderTemplateProperty =
            DependencyProperty.Register("ExpanderTemplate", typeof(ControlTemplate), typeof(TreeViewItemAdv), new PropertyMetadata(null, new PropertyChangedCallback(OnExpanderTemplateChanged)));

        /// <summary>
        /// This Property indicates the source for the Collapse image of the item.
        /// </summary>
        public static readonly DependencyProperty CollapseImageSourceProperty =
            DependencyProperty.Register("CollapseImageSource", typeof(ImageSource), typeof(TreeViewItemAdv), new PropertyMetadata(null, new PropertyChangedCallback(OnCollapseImageSourceChanged)));

        /// <summary>
        /// This  Property indicates the source for the Expand image of the item.
        /// </summary>
        public static readonly DependencyProperty ExpandImageSourceProperty =
           DependencyProperty.Register("ExpandImageSource", typeof(ImageSource), typeof(TreeViewItemAdv), new PropertyMetadata(null, new PropertyChangedCallback(OnExpandImageSourceChanged)));

        /// <summary>
        /// This Property indicates the Width for the Collapse Image of the item.
        /// </summary>
        public static readonly DependencyProperty CollapseImageWidthProperty =
            DependencyProperty.Register("CollapseImageWidth", typeof(double), typeof(TreeViewItemAdv), new PropertyMetadata(0d, new PropertyChangedCallback(OnCollapseImageWidthChanged)));

        /// <summary>
        /// This Property indicates the Height for the Collapse Image of the item.
        /// </summary>
        public static readonly DependencyProperty CollapseImageHeightProperty =
            DependencyProperty.Register("CollapseImageHeight", typeof(double), typeof(TreeViewItemAdv), new PropertyMetadata(0d, new PropertyChangedCallback(OnCollapseImageHeightChanged)));

        /// <summary>
        /// This Property indicates the Width for the Expand Image of the item.
        /// </summary>
        public static readonly DependencyProperty ExpandImageWidthProperty =
            DependencyProperty.Register("ExpandImageWidth", typeof(double), typeof(TreeViewItemAdv), new PropertyMetadata(0d, new PropertyChangedCallback(OnExpandImageWidthChanged)));

        /// <summary>
        /// This Property indicates the Height for the Expand Image of the Item.
        /// </summary>
        public static readonly DependencyProperty ExpandImageHeightProperty =
            DependencyProperty.Register("ExpandImageHeight", typeof(double), typeof(TreeViewItemAdv), new PropertyMetadata(0d, new PropertyChangedCallback(OnExpandImageHeightChanged)));

        /// <summary>
        /// This Property indicates the LineStrokeArray for the Expand Image of the Item.
        /// </summary>
        public static readonly DependencyProperty LineStrokeArrayProperty =
            DependencyProperty.Register("LineStrokeArray", typeof(DoubleCollection), typeof(TreeViewItemAdv), new PropertyMetadata(null, new PropertyChangedCallback(OnLineStrokeArrayChanged)));

        /// <summary>
        /// Identifies the RootLineStroke Dependency Property.
        /// </summary>
        public static readonly DependencyProperty RootLineStrokeProperty =
            DependencyProperty.Register("RootLineStroke", typeof(Brush), typeof(TreeViewItemAdv), new PropertyMetadata(new SolidColorBrush(Colors.Black), new PropertyChangedCallback(OnRootLineStrokeChanged)));

        /// <summary>
        /// Identifies the RootLineVisibility Dependency Property.
        /// </summary>
        public static readonly DependencyProperty RootLineVisibilityProperty =
            DependencyProperty.Register("RootLineVisibility", typeof(Visibility), typeof(TreeViewItemAdv), new PropertyMetadata(Visibility.Visible, new PropertyChangedCallback(OnRootLineVisibilityChanged)));

        /// <summary>
        /// Identifies the HorizontalLineVisibility Dependency Property.
        /// </summary>
        public static readonly DependencyProperty HorizontalLineVisibilityProperty =
           DependencyProperty.Register("HorizontalLineVisibility", typeof(Visibility), typeof(TreeViewItemAdv), new PropertyMetadata(Visibility.Collapsed));

        /// <summary>
        /// Identifies the DragLineVisibility Dependency Property.
        /// </summary>
        public static readonly DependencyProperty DragLineVisibilityProperty =
           DependencyProperty.Register("DragLineVisibility", typeof(Visibility), typeof(TreeViewItemAdv), new PropertyMetadata(Visibility.Collapsed, new PropertyChangedCallback(OnDragLineVisibilityChanged)));

        /// <summary>
        /// Identifies the DragLineColor Dependency Property.
        /// </summary>
        public static readonly DependencyProperty DragLineColorProperty =
           DependencyProperty.Register("DragLineColor", typeof(Brush), typeof(TreeViewItemAdv), new PropertyMetadata(new SolidColorBrush(Colors.Black), new PropertyChangedCallback(OnDragLineColorChanged)));

        /// <summary>
        /// This Property determines the Text for the TreeViewAdv Control.
        /// </summary>
        public static readonly DependencyProperty RectangleHeightProperty = DependencyProperty.Register("RectangleHeight", typeof(double), typeof(TreeViewItemAdv), new PropertyMetadata(30d, new PropertyChangedCallback(OnRectangleHeightChanged)));
        /// <summary>
        /// Identifies the IsMultiSelect Dependency Property.
        /// </summary>
        public static readonly DependencyProperty IsMultiSelectProperty = DependencyProperty.Register("IsMultiSelect", typeof(bool), typeof(TreeViewItemAdv), new PropertyMetadata(false, new PropertyChangedCallback(OnIsMultiSelectChanged)));


 		/// <summary>
        /// Identifies the LoadingHeader Dependency Property.
        /// </summary>
        public object LoadingHeader
        {
            get { return (object)GetValue(LoadingHeaderProperty); }
            set { SetValue(LoadingHeaderProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for LoadingHeader.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty LoadingHeaderProperty =
            DependencyProperty.Register("LoadingHeader", typeof(object), typeof(TreeViewItemAdv), new PropertyMetadata(null));


		 /// <summary>
        /// Identifies the LoadingHeaderTemplate Dependency Property.
        /// </summary>
        public DataTemplate LoadingHeaderTemplate
        {
            get { return (DataTemplate)GetValue(LoadingHeaderTemplateProperty); }
            set { SetValue(LoadingHeaderTemplateProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for LoadingHeaderTemplate.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty LoadingHeaderTemplateProperty =
            DependencyProperty.Register("LoadingHeaderTemplate", typeof(DataTemplate), typeof(TreeViewItemAdv), new PropertyMetadata(null));


        
        

        #endregion

        #region DP Events

        /// <summary>
        /// 
        /// </summary>
        public event ExpandCollapseEventHandler Expanding;
        /// <summary>
        /// 
        /// </summary>
        public event ExpandCollapseEventHandler Collapsing;

        /// <summary>
        /// Event that is raised when the HasItems Property is Changed.
        /// </summary>
        public event PropertyChangedCallback HasItemsChanged;

        /// <summary>
        /// Event that is raised when the Header Property is Changed.
        /// </summary>
        public event PropertyChangedCallback HeaderChanged;

        /// <summary>
        /// Event that is raised when the IsExpanded Property is Changed.
        /// </summary>
        public event PropertyChangedCallback IsExpandedChanged;

        /// <summary>
        /// Event that is raised when the IsMouseOver Property is Changed.
        /// </summary>
        public event PropertyChangedCallback IsMouseOverChanged;

        /// <summary>
        /// Event that is raised when the IsSelected Property is Changed.
        /// </summary>
        public event PropertyChangedCallback IsSelectedChanged;

        /// <summary>
        /// Event that is raised when the SelectOnExpandChange Property is Changed.
        /// </summary>
        public event PropertyChangedCallback SelectOnExpandChangeChanged;

        /// <summary>
        /// Event that is raised when the LeftImageSource Property is Changed.
        /// </summary>
        public event PropertyChangedCallback LeftImageSourceChanged;

        /// <summary>
        /// Event that is raised when the RightImageSource Property is Changed.
        /// </summary>
        public event PropertyChangedCallback RightImageSourceChanged;

        /// <summary>
        /// Event that is raised when the  LeftImageWidth Property is Changed.
        /// </summary>
        public event PropertyChangedCallback LeftImageWidthChanged;

        /// <summary>
        /// Event that is raised when the RightImageWidth Property is Changed.
        /// </summary>
        public event PropertyChangedCallback RightImageWidthChanged;

        /// <summary>
        /// Event that is raised when the LeftImageHeight Property is Changed.
        /// </summary>
        public event PropertyChangedCallback LeftImageHeightChanged;

        /// <summary>
        /// Event that is raised when the RightImageHeight Property is Changed.
        /// </summary>
        public event PropertyChangedCallback RightImageHeightChanged;

        /// <summary>
        /// Event that is raised when the ImageHorizontalAlignment Property is Changed.
        /// </summary>
        public event PropertyChangedCallback ImageHorizontalAlignmentChanged;

        /// <summary>
        /// Event that is raised when the ImageVerticalAlignment Property is Changed.
        /// </summary>
        public event PropertyChangedCallback ImageVerticalAlignmentChanged;

        /// <summary>
        /// Event that is raised when the ImageMargin Property is Changed.
        /// </summary>
        public event PropertyChangedCallback ImageMarginChanged;

        /// <summary>
        /// Event that is raised when the TextHorizontalAlignment Property is Changed.
        /// </summary>
        public event PropertyChangedCallback TextHorizontalAlignmentChanged;

        /// <summary>
        /// Event that is raised when the TextVerticalAlignment Property is Changed.
        /// </summary>
        public event PropertyChangedCallback TextVerticalAlignmentChanged;

        /// <summary>
        /// Event that is raised when the TextMargin Property is Changed.
        /// </summary>
        public event PropertyChangedCallback TextMarginChanged;

        /// <summary>
        /// Event that is raised when the ExpanderTemplate Property is Changed.
        /// </summary>
        public event PropertyChangedCallback ExpanderTemplateChanged;

        /// <summary>
        /// Event that is raised when the CollapseImageSource Property is Changed.
        /// </summary>
        public event PropertyChangedCallback CollapseImageSourceChanged;

        /// <summary>
        /// Event that is raised when the ExpandImageSource Property is Changed.
        /// </summary>
        public event PropertyChangedCallback ExpandImageSourceChanged;

        /// <summary>
        /// Event that is raised when the CollapseImageWidth Property is Changed.
        /// </summary>
        public event PropertyChangedCallback CollapseImageWidthChanged;

        /// <summary>
        /// Event that is raised when the CollapseImageHeight Property is Changed.
        /// </summary>
        public event PropertyChangedCallback CollapseImageHeightChanged;

        /// <summary>
        /// Event that is raised when the ExpandImageWidth Property is Changed.
        /// </summary>
        public event PropertyChangedCallback ExpandImageWidthChanged;

        /// <summary>
        /// Event that is raised when the ExpandImageHeight Property is Changed.
        /// </summary>
        public event PropertyChangedCallback ExpandImageHeightChanged;


        internal event PropertyChangedCallback LineStrokeArrayChanged;
        internal event PropertyChangedCallback RootLineStrokeChanged;
        internal event PropertyChangedCallback RootLineVisibilityChanged;
        internal event PropertyChangedCallback DragLineVisibilityChanged;
        internal event PropertyChangedCallback DragLineColorChanged;
        internal event PropertyChangedCallback RectangleHeightChanged;
        internal event PropertyChangedCallback IsMultiSelectChanged;

        #endregion

        #region Properties

        internal bool IsFocused = true; 
        internal bool IsInEditMode = false;
        
        /// <summary>
        /// 
        /// </summary>
        public bool IsLoading
        {
            get;

            internal set;
        }

        /// <summary>
        /// Gets or sets the height of the vertical line3.
        /// </summary>
        /// <value>The height of the vertical line3.</value>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [BrowsableAttribute(false)]
        public double VerticalLine3Height
        {
            get { return (double)GetValue(VerticalLine3HeightProperty); }
            set { SetValue(VerticalLine3HeightProperty, value); }
        }

        /// <summary>
        /// Gets or sets the root line margin.
        /// </summary>
        /// <value>The root line margin.</value>
        public Thickness RootLineMargin
        {
            get { return (Thickness)GetValue(RootLineMarginProperty); }
            set { SetValue(RootLineMarginProperty, value); }
        }

        /// <summary>
        /// Gets or sets the header margin.
        /// </summary>
        /// <value>The header margin.</value>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [BrowsableAttribute(false)]
        public Thickness HeaderMargin
        {
            get { return (Thickness)GetValue(HeaderMarginProperty); }
            set { SetValue(HeaderMarginProperty, value); }
        }

        /// <summary>
        /// Gets or sets the vertical root line3 visibility.
        /// </summary>
        /// <value>The vertical root line3 visibility.</value>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [BrowsableAttribute(false)]
        public Visibility VerticalRootLine3Visibility
        {
            get { return (Visibility)GetValue(VerticalRootLine3VisibilityProperty); }
            set { SetValue(VerticalRootLine3VisibilityProperty, value); }
        }

        /// <summary>
        /// Gets or sets the vertical root line1 visibility.
        /// </summary>
        /// <value>The vertical root line1 visibility.</value>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [BrowsableAttribute(false)]
        public Visibility VerticalRootLine1Visibility
        {
            get { return (Visibility)GetValue(VerticalRootLine1VisibilityProperty); }
            set { SetValue(VerticalRootLine1VisibilityProperty, value); }
        }

        /// <summary>
        /// Gets or sets the vertical root line2 visibility.
        /// </summary>
        /// <value>The vertical root line2 visibility.</value>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [BrowsableAttribute(false)]
        public Visibility VerticalRootLine2Visibility
        {
            get { return (Visibility)GetValue(VerticalRootLine2VisibilityProperty); }
            set { SetValue(VerticalRootLine2VisibilityProperty, value); }
        }

        /// <summary>
        /// Gets or sets the toggle button visibility.
        /// </summary>
        /// <value>The toggle button visibility.</value>
        public Visibility ExpanderVisibility
        {
            get { return (Visibility)GetValue(ExpanderVisibilityProperty); }
            set { SetValue(ExpanderVisibilityProperty, value); }
        }


        /// <summary>
        /// Gets or sets a value indicating whether the HasItems Dependency Property
        /// </summary>
        public bool HasItems
        {
            get
            {
                return (bool)GetValue(HasItemsProperty);
            }

            set
            {
                SetValue(HasItemsProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Header dependency Property
        /// </summary>
		[EditorBrowsable(EditorBrowsableState.Never)]
        [BrowsableAttribute(false)]
        public new object Header
        {
            get
            {
                return GetValue(HeaderProperty);
            }

            set
            {
                SetValue(HeaderProperty, value);
                if (this.contentpresenter != null)
                {
                    double height = this.LeftImageHeight > this.RightImageHeight ? this.LeftImageHeight : this.RightImageHeight;
                    height = height > (IsExpanded ? this.ExpandImageHeight : this.CollapseImageHeight) ? height : (IsExpanded ? this.ExpandImageHeight : this.CollapseImageHeight);
                    //RectangleHeight = this.contentpresenter.ActualHeight > height ? this.contentpresenter.ActualHeight : height;

                    RectangleHeight = (this.contentpresenter.ActualHeight > height ? this.contentpresenter.ActualHeight + this.contentpresenter.Margin.Top + this.contentpresenter.Margin.Bottom : height);// +5;
                    //ContentHeight = (this.contentpresenter.ActualHeight > height ? this.contentpresenter.ActualHeight + this.contentpresenter.Margin.Top + this.contentpresenter.Margin.Bottom : height);

                    this.VerticalLine3Height = this.RectangleHeight / 2;
                    this.RefreshFullRowSelect();
                }
            }
        }

        /// <summary>
        /// Gets or sets the HeaderTemplate Dependency Property
        /// </summary>
        public new DataTemplate HeaderTemplate
        {
            get
            {
                return GetValue(HeaderTemplateProperty) as DataTemplate;
            }

            set
            {
                SetValue(HeaderTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the IsExpanded Dependency Property
        /// </summary>
        public bool IsExpanded
        {
            get
            {
                return (bool)GetValue(IsExpandedProperty);
            }

            set
            {
                SetValue(IsExpandedProperty, value);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the IsMouseOver Dependency Property
        /// </summary>
        public bool IsMouseOver
        {
            get
            {
                return (bool)GetValue(IsMouseOverProperty);
            }

            internal set
            {
                SetValue(IsMouseOverProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the IsSelected Dependency Property
        /// </summary>
        public bool IsSelected
        {
            get
            {
                return (bool)GetValue(IsSelectedProperty);
            }

            set
            {
                SetValue(IsSelectedProperty, value);
                if (value == false)
                {
                    //if (this.nodemouseover != null)
                    //{
                    //    nodemouseover.Visibility = Visibility.Collapsed;
                    //    nodemouseover.Opacity = 0;
                    //}
                    //if (this.nodeselection != null)
                    //{
                    //    nodeselection.Visibility = Visibility.Collapsed;
                    //    nodeselection.Opacity = 0;
                    //}
                    //IsMouseOver = false;
                }
                else
                {
                    //IsMouseOver = false;
                }
            }
        }

        /// <summary>
        /// Gets the Nodes Property
        /// </summary>
        public IList<TreeViewItemAdv> Nodes
        {
            get;
            private set;
        }

        internal IDictionary<DependencyObject, object> ContainersToItems { get; set; }

        /// <summary>
        /// Gets  the ParentNode Property
        /// </summary>
        public TreeViewItemAdv ParentNode
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets or sets the parent treeview.
        /// </summary>
        /// <value>The parent treeview.</value>
        public TreeViewAdv ParentTreeview
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets or sets the SelectOnExpandChange Dependency Property
        /// </summary>
        public bool? SelectOnExpandChange
        {
            get
            {
                return (bool?)GetValue(SelectOnExpandChangeProperty);
            }

            set
            {
                SetValue(SelectOnExpandChangeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets The LeftImageSource Dependency Property
        /// </summary>
        public ImageSource LeftImageSource
        {
            get
            {
                return (ImageSource)GetValue(LeftImageSourceProperty);
            }

            set
            {
                SetValue(LeftImageSourceProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the RightImageSoruce Dependency Property
        /// </summary>
        public ImageSource RightImageSource
        {
            get
            {
                return (ImageSource)GetValue(RightImageSourceProperty);
            }

            set
            {
                SetValue(RightImageSourceProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the LeftImageWidth Dependency Property
        /// </summary>
        public double LeftImageWidth
        {
            get
            {
                return (double)GetValue(LeftImageWidthProperty);
            }

            set
            {
                SetValue(LeftImageWidthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the RightImageWidth Dependency Property
        /// </summary>
        public double RightImageWidth
        {
            get
            {
                return (double)GetValue(RightImageWidthProperty);
            }

            set
            {
                SetValue(RightImageWidthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the LeftImageHeight Dependency Property
        /// </summary>
        public double LeftImageHeight
        {
            get
            {
                return (double)GetValue(LeftImageHeightProperty);
            }

            set
            {
                SetValue(LeftImageHeightProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the RightImageHeight Dependency Property
        /// </summary>
        public double RightImageHeight
        {
            get
            {
                return (double)GetValue(RightImageHeightProperty);
            }

            set
            {
                SetValue(RightImageHeightProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the ImageHorizontalAlignment Dependency Property
        /// </summary>
        public HorizontalAlignment ImageHorizontalAlignment
        {
            get
            {
                return (HorizontalAlignment)GetValue(ImageHorizontalAlignmentProperty);
            }

            set
            {
                SetValue(ImageHorizontalAlignmentProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the ImageVerticalAlignment Dependency Property
        /// </summary>
        public VerticalAlignment ImageVerticalAlignment
        {
            get
            {
                return (VerticalAlignment)GetValue(ImageVerticalAlignmentProperty);
            }

            set
            {
                SetValue(ImageVerticalAlignmentProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the ImageMargin Dependency Property
        /// </summary>
        public Thickness ImageMargin
        {
            get
            {
                return (Thickness)GetValue(ImageMarginProperty);
            }

            set
            {
                SetValue(ImageMarginProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the TextHorizontalAlignment Dependency Property
        /// </summary>
        public HorizontalAlignment TextHorizontalAlignment
        {
            get
            {
                return (HorizontalAlignment)GetValue(TextHorizontalAlignmentProperty);
            }

            set
            {
                SetValue(TextHorizontalAlignmentProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the TextVerticalAlignment Dependency Property
        /// </summary>
        public VerticalAlignment TextVerticalAlignment
        {
            get
            {
                return (VerticalAlignment)GetValue(TextVerticalAlignmentProperty);
            }

            set
            {
                SetValue(TextVerticalAlignmentProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the TextMargin Dependency Property
        /// </summary>
        public Thickness TextMargin
        {
            get
            {
                return (Thickness)GetValue(TextMarginProperty);
            }

            set
            {
                SetValue(TextMarginProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the ExpanderTemplate Dependency Property
        /// </summary>
        public ControlTemplate ExpanderTemplate
        {
            get
            {
                return (ControlTemplate)GetValue(ExpanderTemplateProperty);
            }

            set
            {
                SetValue(ExpanderTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the CollapseImageSource Dependency Property
        /// </summary>
        public ImageSource CollapseImageSource
        {
            get
            {
                return (ImageSource)GetValue(CollapseImageSourceProperty);
            }

            set
            {
                SetValue(CollapseImageSourceProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the ExpandImageSource Dependency Property
        /// </summary>
        public ImageSource ExpandImageSource
        {
            get
            {
                return (ImageSource)GetValue(ExpandImageSourceProperty);
            }

            set
            {
                SetValue(ExpandImageSourceProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the CollapseImageWidth Dependency Property
        /// </summary>
        public double CollapseImageWidth
        {
            get
            {
                return (double)GetValue(CollapseImageWidthProperty);
            }

            set
            {
                SetValue(CollapseImageWidthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the CollapseImageHeight Dependency Property
        /// </summary>
        public double CollapseImageHeight
        {
            get
            {
                return (double)GetValue(CollapseImageHeightProperty);
            }

            set
            {
                SetValue(CollapseImageHeightProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the ExpandImageWidth Dependency Property
        /// </summary>
        public double ExpandImageWidth
        {
            get
            {
                return (double)GetValue(ExpandImageWidthProperty);
            }

            set
            {
                SetValue(ExpandImageWidthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the ExpandImageHeight Dependency Property
        /// </summary>
        public double ExpandImageHeight
        {
            get
            {
                return (double)GetValue(ExpandImageHeightProperty);
            }

            set
            {
                SetValue(ExpandImageHeightProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the line stroke array.
        /// </summary>
        /// <value>The line stroke array.</value>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [BrowsableAttribute(false)]
        public DoubleCollection LineStrokeArray
        {
            get
            {
                return (DoubleCollection)GetValue(LineStrokeArrayProperty);
            }

            set
            {
                SetValue(LineStrokeArrayProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the root line stroke.
        /// </summary>
        /// <value>The root line stroke.</value>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [BrowsableAttribute(false)]
        public Brush RootLineStroke
        {
            get
            {
                return (Brush)GetValue(RootLineStrokeProperty);
            }

            set
            {
                SetValue(RootLineStrokeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the root line visibility.
        /// </summary>
        /// <value>The root line visibility.</value>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [BrowsableAttribute(false)]
        public Visibility RootLineVisibility
        {
            get
            {
                return (Visibility)GetValue(RootLineVisibilityProperty);
            }

            set
            {
                SetValue(RootLineVisibilityProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the drag line visibility.
        /// </summary>
        /// <value>The drag line visibility.</value>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [BrowsableAttribute(false)]
        public Visibility DragLineVisibility
        {
            get
            {
                return (Visibility)GetValue(DragLineVisibilityProperty);
            }

            set
            {
                SetValue(DragLineVisibilityProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the color of the drag line.
        /// </summary>
        /// <value>The color of the drag line.</value>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [BrowsableAttribute(false)]
        public Brush DragLineColor
        {
            get
            {
                return (Brush)GetValue(DragLineColorProperty);
            }

            set
            {
                SetValue(DragLineColorProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the hort line visibility.
        /// </summary>
        /// <value>The hort line visibility.</value>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [BrowsableAttribute(false)]
        public Visibility HorizontalLineVisibility
        {
            get
            {
                return (Visibility)GetValue(HorizontalLineVisibilityProperty);
            }

            set
            {
                SetValue(HorizontalLineVisibilityProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the height of the rectangle.
        /// </summary>
        /// <value>The height of the rectangle.</value>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [BrowsableAttribute(false)]
        public double RectangleHeight
        {
            get
            {
                return (double)GetValue(RectangleHeightProperty);
            }

            set
            {
                SetValue(RectangleHeightProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is multi select.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is multi select; otherwise, <c>false</c>.
        /// </value>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [BrowsableAttribute(false)]
        public bool IsMultiSelect
        {
            get
            {
                return (bool)GetValue(IsMultiSelectProperty);
            }

            set
            {
                SetValue(IsMultiSelectProperty, value);
            }
        }

        /// <summary>
        /// Gets the level.
        /// </summary>
        /// <value>The level.</value>
        public int Level
        {
            get
            {
                TreeViewItemPath path = this.GetPath(this);
                int k = 0;
                for (int i = 0; i < path.FullPath.Count(); i++)
                {
                    if (this == (TreeViewItemAdv)path.FullPath[i])
                    {
                        k = i;
                        break;
                    }
                }

                return k;
            }
        }


		/// <summary>
		/// 
		/// </summary>
        public bool IsLoadOnDemand
        {
            get { return (bool)GetValue(IsLoadOnDemandProperty); }
            set { SetValue(IsLoadOnDemandProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsLoadOnDemand.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsLoadOnDemandProperty =
            DependencyProperty.Register("IsLoadOnDemand", typeof(bool), typeof(TreeViewItemAdv), new PropertyMetadata(false, new PropertyChangedCallback(OnIsLoadOnDemandChanged)));


        


        #endregion

        #region DP Events Implementation


        private static void OnIsLoadOnDemandChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            TreeViewItemAdv treeItem = sender as TreeViewItemAdv;
            if (treeItem != null)
            {
                if ((bool)args.NewValue)
                {
                    treeItem.HasItems = true;
                }
                else
                {
                    treeItem.IsLoading = false;
                    if (treeItem.Items.Count > 0)
                    {
                        treeItem.HasItems = true;
                    }
                    else
                    {
                        treeItem.HasItems = false;
                    }
                    treeItem.RefreshToogleButton();
                }
                treeItem.UpdateVisualState(false);
            }
            
        }


        /// <summary>
        /// Calls HasItemsChangedCallback method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        private static void HasItemsChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TreeViewItemAdv source = (TreeViewItemAdv)d;
            if (!(bool)e.NewValue && source.expander != null)
            {
                source.expander.IsEnabled = false;
                //source.expander.IsChecked = false;
            }
            else if (source.expander != null)
            {
                source.expander.IsEnabled = true;
            }

            ((TreeViewItemAdv)d).UpdateVisualState(true);
            source.HasItemsChangedCallback(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="HasItemsChanged"/> event.
        /// </summary>
        protected virtual void HasItemsChangedCallback(DependencyPropertyChangedEventArgs e)
        {
            if (HasItemsChanged != null)
            {
                HasItemsChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnIsMouseOverChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        private static void OnIsMouseOverChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TreeViewItemAdv source = (TreeViewItemAdv)d;
            source.UpdateVisualState(true);
            source.OnIsMouseOverChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="IsMouseOverChanged"/> event.
        /// </summary>
        protected virtual void OnIsMouseOverChanged(DependencyPropertyChangedEventArgs e)
        {
            if (IsMouseOverChanged != null)
            {
                IsMouseOverChanged(this, e);
            }
        }

        /// <summary>
        /// Calls IsSelectedChangedCallback method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        private static void IsSelectedChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TreeViewItemAdv element = (TreeViewItemAdv)d;
            bool newValue = (bool)e.NewValue;
            element.Select(newValue);
            if (!newValue)
            {
                element.IsMouseOver = false;
            }
            element.UpdateVisualState(true);
            element.IsSelectedChangedCallback(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="IsSelectedChanged"/> event.
        /// </summary>
        protected virtual void IsSelectedChangedCallback(DependencyPropertyChangedEventArgs e)
        {
            if (IsSelectedChanged != null)
            {
                IsSelectedChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnIsExpandedChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        private static void OnIsExpandedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TreeViewItemAdv source = d as TreeViewItemAdv;
            source.OnIsExpandedChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="IsExpandedChanged"/> event.
        /// </summary>
        protected virtual void OnIsExpandedChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.IsExpanded == true)
            {
                ExpandCollapseEventArgs args = new ExpandCollapseEventArgs() { Cancel = false };
                if (Expanding != null)
                {
                    this.Expanding(this, args);
                }
                if (this.ParentTreeview != null)
                {
                    this.ParentTreeview.ExpandingTreeViewItem(this, args);
                }
                if (args.Cancel)
                    this.IsExpanded = !this.IsExpanded;
            }
            else
            {
                ExpandCollapseEventArgs args = new ExpandCollapseEventArgs() { Cancel = false };
                if (this.Collapsing != null)
                {
                    this.Collapsing(this, args);
                }
                if (this.ParentTreeview != null)
                {
                    this.ParentTreeview.CollapsingTreeViewItem(this, args);
                }
                if (args.Cancel)
                    this.IsExpanded = !this.IsExpanded;
            }
            if (this.ParentTreeview != null)
            {
                this.ParentTreeview.RefreshToogleButton();
            }

            this.RefreshToogleButton();
            this.UpdateVisualState(true);

            if (IsExpandedChanged != null)
            {
                IsExpandedChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnSelectOnExpandChangeChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        private static void OnSelectOnExpandChangeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TreeViewItemAdv source = (TreeViewItemAdv)d;
            source.OnSelectOnExpandChangeChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="SelectOnExpandChangeChanged"/> event.
        /// </summary>
        protected virtual void OnSelectOnExpandChangeChanged(DependencyPropertyChangedEventArgs e)
        {
            if (SelectOnExpandChangeChanged != null)
            {
                SelectOnExpandChangeChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnExpanderTemplateChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        private static void OnExpanderTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TreeViewItemAdv source = (TreeViewItemAdv)d;
            source.OnExpanderTemplateChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="ExpanderTemplateChanged"/> event.
        /// </summary>
        protected virtual void OnExpanderTemplateChanged(DependencyPropertyChangedEventArgs e)
        {
            if (ExpanderTemplateChanged != null)
            {
                ExpanderTemplateChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnLeftImageSourceChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        private static void OnLeftImageSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TreeViewItemAdv obj = (TreeViewItemAdv)d;
            obj.OnLeftImageSourceChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="LeftImageSourceChanged"/> event.
        /// </summary>
        protected virtual void OnLeftImageSourceChanged(DependencyPropertyChangedEventArgs e)
        {
            if (LeftImageSourceChanged != null)
            {
                LeftImageSourceChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnRightImageSourceChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        private static void OnRightImageSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TreeViewItemAdv obj = (TreeViewItemAdv)d;
            obj.OnRightImageSourceChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="RightImageSourceChanged"/> event.
        /// </summary>
        protected virtual void OnRightImageSourceChanged(DependencyPropertyChangedEventArgs e)
        {
            if (RightImageSourceChanged != null)
            {
                RightImageSourceChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnRightImageWidthChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        private static void OnRightImageWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TreeViewItemAdv obj = (TreeViewItemAdv)d;
            obj.OnRightImageWidthChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="RightImageWidthChanged"/> event.
        /// </summary>
        protected virtual void OnRightImageWidthChanged(DependencyPropertyChangedEventArgs e)
        {
            if (RightImageWidthChanged != null)
            {
                RightImageWidthChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnLeftImageWidthChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        private static void OnLeftImageWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TreeViewItemAdv obj = (TreeViewItemAdv)d;
            obj.OnLeftImageWidthChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="LeftImageWidthChanged"/> event.
        /// </summary>
        protected virtual void OnLeftImageWidthChanged(DependencyPropertyChangedEventArgs e)
        {
            if (LeftImageWidthChanged != null)
            {
                LeftImageWidthChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnRightImageHeightChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        private static void OnRightImageHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TreeViewItemAdv obj = (TreeViewItemAdv)d;
            obj.OnRightImageHeightChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="RightImageHeightChanged"/> event.
        /// </summary>
        protected virtual void OnRightImageHeightChanged(DependencyPropertyChangedEventArgs e)
        {
            if (horLineAlignment < (double)e.NewValue)
            {
                horLineAlignment = (double)e.NewValue;
            }

            checkForHorLineMovement(horLineAlignment);
            if (RightImageHeightChanged != null)
            {
                RightImageHeightChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnLeftImageHeightChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        private static void OnLeftImageHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TreeViewItemAdv obj = (TreeViewItemAdv)d;
            obj.OnLeftImageHeightChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="LeftImageHeightChanged"/> event.
        /// </summary>
        protected virtual void OnLeftImageHeightChanged(DependencyPropertyChangedEventArgs e)
        {
            if (horLineAlignment < (double)e.NewValue)
            {
                horLineAlignment = (double)e.NewValue;
            }

            checkForHorLineMovement(horLineAlignment);

            if (LeftImageHeightChanged != null)
            {
                LeftImageHeightChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnImageVerticalAlignmentChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        private static void OnImageVerticalAlignmentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TreeViewItemAdv obj = (TreeViewItemAdv)d;
            obj.OnImageVerticalAlignmentChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="ImageVerticalAlignmentChanged"/> event.
        /// </summary>
        protected virtual void OnImageVerticalAlignmentChanged(DependencyPropertyChangedEventArgs e)
        {
            if (ImageVerticalAlignmentChanged != null)
            {
                ImageVerticalAlignmentChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnImageHorizontalAlignmentChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        private static void OnImageHorizontalAlignmentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TreeViewItemAdv obj = (TreeViewItemAdv)d;
            obj.OnImageHorizontalAlignmentChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="ImageHorizontalAlignmentChanged"/> event.
        /// </summary>
        protected virtual void OnImageHorizontalAlignmentChanged(DependencyPropertyChangedEventArgs e)
        {
            if (ImageHorizontalAlignmentChanged != null)
            {
                ImageHorizontalAlignmentChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnImageMarginChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        private static void OnImageMarginChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TreeViewItemAdv obj = (TreeViewItemAdv)d;
            obj.OnImageMarginChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="ImageMarginChanged"/> event.
        /// </summary>
        protected virtual void OnImageMarginChanged(DependencyPropertyChangedEventArgs e)
        {
            if (ImageMarginChanged != null)
            {
                ImageMarginChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnTextVerticalAlignmentChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        private static void OnTextVerticalAlignmentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TreeViewItemAdv obj = (TreeViewItemAdv)d;
            obj.OnTextVerticalAlignmentChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="TextVerticalAlignmentChanged"/> event.
        /// </summary>
        protected virtual void OnTextVerticalAlignmentChanged(DependencyPropertyChangedEventArgs e)
        {
            if (TextVerticalAlignmentChanged != null)
            {
                TextVerticalAlignmentChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnTextHorizontalAlignmentChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        private static void OnTextHorizontalAlignmentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TreeViewItemAdv obj = (TreeViewItemAdv)d;
            obj.OnTextHorizontalAlignmentChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="TextHorizontalAlignmentChanged"/> event.
        /// </summary>
        protected virtual void OnTextHorizontalAlignmentChanged(DependencyPropertyChangedEventArgs e)
        {
            if (TextHorizontalAlignmentChanged != null)
            {
                TextHorizontalAlignmentChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnTextMarginChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        private static void OnTextMarginChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TreeViewItemAdv obj = (TreeViewItemAdv)d;
            obj.OnTextMarginChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="TextMarginChanged"/> event.
        /// </summary>
        protected virtual void OnTextMarginChanged(DependencyPropertyChangedEventArgs e)
        {
            if (TextMarginChanged != null)
            {
                TextMarginChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnCollapseImageSourceChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        private static void OnCollapseImageSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TreeViewItemAdv obj = (TreeViewItemAdv)d;
            obj.OnCollapseImageSourceChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="CollapseImageSourceChanged"/> event.
        /// </summary>
        protected virtual void OnCollapseImageSourceChanged(DependencyPropertyChangedEventArgs e)
        {
            if (CollapseImageSourceChanged != null)
            {
                CollapseImageSourceChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnExpandImageSourceChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        private static void OnExpandImageSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TreeViewItemAdv obj = (TreeViewItemAdv)d;
            obj.OnExpandImageSourceChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="ExpandImageSourceChanged"/> event.
        /// </summary>
        protected virtual void OnExpandImageSourceChanged(DependencyPropertyChangedEventArgs e)
        {
            if (ExpandImageSourceChanged != null)
            {
                ExpandImageSourceChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnCollapseImageWidthChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        private static void OnCollapseImageWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TreeViewItemAdv obj = (TreeViewItemAdv)d;
            obj.OnCollapseImageWidthChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="CollapseImageWidthChanged"/> event.
        /// </summary>
        protected virtual void OnCollapseImageWidthChanged(DependencyPropertyChangedEventArgs e)
        {
            if (CollapseImageWidthChanged != null)
            {
                CollapseImageWidthChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnCollapseImageHeightChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        private static void OnCollapseImageHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TreeViewItemAdv obj = (TreeViewItemAdv)d;
            obj.OnCollapseImageHeightChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="CollapseImageHeightChanged"/> event.
        /// </summary>
        protected virtual void OnCollapseImageHeightChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.CollapsedIconPart != null)
            {
                if (this.CollapsedIconPart.Visibility == Visibility.Visible)
                {
                    if (horLineAlignment < (double)e.NewValue)
                    {
                        horLineAlignment = (double)e.NewValue;
                    }

                    checkForHorLineMovement(horLineAlignment);
                }
            }

            if (CollapseImageHeightChanged != null)
            {
                CollapseImageHeightChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnExpandImageWidthChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        private static void OnExpandImageWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TreeViewItemAdv obj = (TreeViewItemAdv)d;
            obj.OnExpandImageWidthChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="ExpandImageWidthChanged"/> event.
        /// </summary>
        protected virtual void OnExpandImageWidthChanged(DependencyPropertyChangedEventArgs e)
        {
            if (ExpandImageWidthChanged != null)
            {
                ExpandImageWidthChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnExpandImageHeightChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        private static void OnExpandImageHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TreeViewItemAdv obj = (TreeViewItemAdv)d;
            obj.OnExpandImageHeightChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="ExpandImageHeightChanged"/> event.
        /// </summary>
        protected virtual void OnExpandImageHeightChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.ExpandIconPart != null)
            {
                if (this.ExpandIconPart.Visibility == Visibility.Visible)
                {
                    if (horLineAlignment < (double)e.NewValue)
                    {
                        horLineAlignment = (double)e.NewValue;
                    }

                    checkForHorLineMovement(horLineAlignment);
                }
            }

            if (ExpandImageHeightChanged != null)
            {
                ExpandImageHeightChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnLineStrokeArrayChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        private static void OnLineStrokeArrayChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TreeViewItemAdv obj = (TreeViewItemAdv)d;
            obj.OnLineStrokeArrayChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="LineStrokeArrayChanged"/> event.
        /// </summary>
        internal virtual void OnLineStrokeArrayChanged(DependencyPropertyChangedEventArgs e)
        {
            if (LineStrokeArrayChanged != null)
            {
                LineStrokeArrayChanged(this, e);
            }
        }

        /// <summary>
        /// Called when [root line stroke changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnRootLineStrokeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TreeViewItemAdv obj = (TreeViewItemAdv)d;
            obj.OnRootLineStrokeChanged(e);
        }

        /// <summary>
        /// Raises the <see cref="E:RootLineStrokeChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        internal virtual void OnRootLineStrokeChanged(DependencyPropertyChangedEventArgs e)
        {
            if (RootLineStrokeChanged != null)
            {
                RootLineStrokeChanged(this, e);
            }
        }

        /// <summary>
        /// Called when [root line visibility changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnRootLineVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TreeViewItemAdv obj = (TreeViewItemAdv)d;
            obj.OnRootLineVisibilityChanged(e);
        }

        /// <summary>
        /// Raises the <see cref="E:RootLineVisibilityChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        internal virtual void OnRootLineVisibilityChanged(DependencyPropertyChangedEventArgs e)
        {
            if (RootLineVisibilityChanged != null)
            {
                RootLineVisibilityChanged(this, e);
            }
        }

        /// <summary>
        /// Called when [drag line visibility changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnDragLineVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TreeViewItemAdv obj = (TreeViewItemAdv)d;
            obj.OnDragLineVisibilityChanged(e);
        }

        /// <summary>
        /// Raises the <see cref="E:DragLineVisibilityChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        internal virtual void OnDragLineVisibilityChanged(DependencyPropertyChangedEventArgs e)
        {
            if (DragLineVisibilityChanged != null)
            {
                DragLineVisibilityChanged(this, e);
            }
        }

        /// <summary>
        /// Called when [drag line color changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnDragLineColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TreeViewItemAdv obj = (TreeViewItemAdv)d;
            obj.OnDragLineColorChanged(e);
        }

        /// <summary>
        /// Raises the <see cref="E:DragLineColorChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        internal virtual void OnDragLineColorChanged(DependencyPropertyChangedEventArgs e)
        {
            if (DragLineColorChanged != null)
            {
                DragLineColorChanged(this, e);
            }
        }

        /// <summary>
        /// Called when [rectangle height changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnRectangleHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TreeViewItemAdv obj = (TreeViewItemAdv)d;
            obj.OnRectangleHeightChanged(e);
        }

        /// <summary>
        /// Raises the <see cref="E:RectangleHeightChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnRectangleHeightChanged(DependencyPropertyChangedEventArgs e)
        {
            if (RectangleHeightChanged != null)
            {
                RectangleHeightChanged(this, e);
            }
        }

        /// <summary>
        /// Called when [header changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnHeaderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TreeViewItemAdv obj = (TreeViewItemAdv)d;
            obj.OnHeaderChanged(e);
        }

        /// <summary>
        /// Raises the <see cref="E:HeaderChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnHeaderChanged(DependencyPropertyChangedEventArgs e)
        {
            if (HeaderChanged != null)
            {
                HeaderChanged(this, e);
            }
        }

        /// <summary>
        /// Called when [is multi select changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnIsMultiSelectChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TreeViewItemAdv element = (TreeViewItemAdv)d;
            bool newValue = (bool)e.NewValue;
            element.UpdateVisualState(true);
            element.OnIsMultiSelectChanged(e);
        }

        /// <summary>
        /// Raises the <see cref="E:IsMultiSelectChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnIsMultiSelectChanged(DependencyPropertyChangedEventArgs e)
        {
            if (IsMultiSelectChanged != null)
            {
                IsMultiSelectChanged(this, e);
            }
        }

        /// <summary>
        /// Checks for hor line movement.
        /// </summary>
        /// <param name="val">The val.</param>
        private void checkForHorLineMovement(double val)
        {
        }

        List<double> temp;
        /// <summary>
        /// Checks the large.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        private double CheckLarge(double value)
        {
            temp = new List<double>();
            temp.Add(this.LeftImageHeight);
            temp.Add(this.RightImageHeight);
            if (this.ExpandIconPart.Visibility == Visibility.Visible)
            {
                temp.Add(this.ExpandImageHeight);
            }

            if (this.CollapsedIconPart.Visibility == Visibility.Visible)
            {
                temp.Add(this.CollapseImageHeight);
            }

            double max = 0;
            for (int i = 0; i < temp.Count; i++)
            {
                if (max < temp[i])
                {
                    max = temp[i];
                }
            }

            return max;
        }
        #endregion

        #region control parts
        internal const string ElementRootName = "LayoutRoot";
        internal const string ExpanderName = "Expander";
        internal const string ItemsPresenterName = "ItemsHost";
        internal FrameworkElement elementRoot;
        internal ToggleButton expander;
        internal ItemsPresenter itemsHost;
        internal TextBox textbox = null;
        internal ContentPresenter contentpresenter = null;
        internal Image leftimage = null;
        internal Image rightimage = null;
        internal TreeViewItemAdv lastContainerCheck=null;
        internal Border HeaderBorder = null;
        internal Grid LayoutRoot = null;
        internal Line HorizontalLine = null;
        internal Image ExpandIconPart = null;
        internal Image CollapsedIconPart = null;
        internal Rectangle VerticalLine1 = null, VerticalLine2 = null;
        internal StackPanel contenthost = null;
        internal bool oddClickMade = false;
        private bool HasSelectedItem = false;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes the TreeViewItemAdv
        /// </summary>
        public TreeViewItemAdv()
        {
            DefaultStyleKey = typeof(TreeViewItemAdv);
            this.Nodes = new List<TreeViewItemAdv>();
            ContainersToItems = new Dictionary<DependencyObject, object>();
            this.HasItems = false;
            TabNavigation = KeyboardNavigationMode.Local;
            IsTabStop = true;
            MouseLeave += this.OnMouseLeave;
            this.LostFocus += new RoutedEventHandler(TreeViewItemAdv_LostFocus);
            this.MouseEnter += new MouseEventHandler(TreeViewItemAdv_MouseEnter);
            this.MouseLeave += new MouseEventHandler(TreeViewItemAdv_MouseLeave);
            //this.KeyDown += new KeyEventHandler(TreeViewItem_KeyDown);
            this.Loaded += new RoutedEventHandler(TreeViewItem_Loaded);
            this.Unloaded += new RoutedEventHandler(TreeViewItemAdv_Unloaded);
        }

        void TreeViewItemAdv_Unloaded(object sender, RoutedEventArgs e)
        {
            MouseLeave -= this.OnMouseLeave;
            this.LostFocus -= new RoutedEventHandler(TreeViewItemAdv_LostFocus);
            this.MouseEnter -= new MouseEventHandler(TreeViewItemAdv_MouseEnter);
            this.MouseLeave -= new MouseEventHandler(TreeViewItemAdv_MouseLeave);
            //this.Loaded -= new RoutedEventHandler(TreeViewItem_Loaded);
            if (this.contentpresenter != null)
            {
                this.contentpresenter.LayoutUpdated -= new EventHandler(contentpresenter_LayoutUpdated);
            }
            if (this.textbox != null)
            {
                this.textbox.KeyDown -= new KeyEventHandler(textbox_KeyDown);
                this.textbox.LostFocus -= new RoutedEventHandler(textbox_LostFocus);
                this.textbox.TextChanged -= new TextChangedEventHandler(textbox_TextChanged);
            }

            if (this.expander != null)
            {
                this.expander.Checked -= this.ToggleButton_CheckChange;
                this.expander.Unchecked -= this.ToggleButton_CheckChange;
            }

            
            if (this.contenthost != null && this.ParentTreeview != null && this.ParentTreeview.DragOnText)
            {
                this.contenthost.MouseEnter -= new MouseEventHandler(treeviewitemgrid_MouseEnter);
                this.contenthost.MouseLeave -= new MouseEventHandler(treeviewitemgrid_MouseLeave);
                this.contenthost.MouseMove -= new MouseEventHandler(treeviewitemgrid_MouseMove);
            }

            if (this.treeviewitemgrid != null)
            {
                this.treeviewitemgrid.MouseEnter -= new MouseEventHandler(treeviewitemgrid_MouseEnter);
                this.treeviewitemgrid.MouseLeave -= new MouseEventHandler(treeviewitemgrid_MouseLeave);
                this.treeviewitemgrid.MouseMove -= new MouseEventHandler(treeviewitemgrid_MouseMove);
            }
        }
        void TreeViewItemAdv_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!this.IsSelected)
                VisualStateManager.GoToState(this, "Normal", true);
        }

        void TreeViewItemAdv_MouseEnter(object sender, MouseEventArgs e)
        {
            this.nodemouseover.Height = this.HeaderBorder.ActualHeight;
            if (this.IsSelected && this.IsMouseOver)
                VisualStateManager.GoToState(this, "SelectedMouseOver", true);
            if (this.IsSelected && !this.IsMouseOver)
                VisualStateManager.GoToState(this, "Selected", true);
            if (this.ParentTreeview != null)
            {
                if (this.ParentTreeview.EnableMouseOverEffect)
                {
                    if (!this.IsSelected && this.IsMouseOver)
                        VisualStateManager.GoToState(this, "MouseOver", true);
                }
            }
        }

        void TreeViewItemAdv_LostFocus(object sender, RoutedEventArgs e)
        {
            if (this.IsSelected)
                VisualStateManager.GoToState(this, "UnFocusedSelection", true);
        }

        /// <summary>
        /// Handles the Loaded event of the TreeViewItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void TreeViewItem_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.ParentTreeview != null)
            {
                if (this.ParentTreeview.itemCount == this.Items.Count || (this.ParentTreeview.itemCount <= 1 && this.Items.Count == 0))
                {
                    this.ParentTreeview.RefreshRootLines(this.ParentTreeview);
                    this.RefreshToogleButton();
                    this.ParentTreeview.itemCount++;
                    if (this.IsSelected)
                    {
                        this.ParentTreeview.SelectionChgd(this, this.IsSelected);
                        if (this.IsSelected && this.nodeSelection != null)
                            this.nodeSelection.Visibility = Visibility.Visible;
                    }
                }
            }
            if (this.expander != null)
            {
                this.expander.Checked += this.ToggleButton_CheckChange;
                this.expander.Unchecked += this.ToggleButton_CheckChange;
            }
            this.UpdateVisualState(true);
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Finds the child node.
        /// </summary>
        /// <param name="dataItem">The data item.</param>
        /// <returns></returns>
        internal TreeViewItemAdv FindChildNode(object dataItem)
        {
            var item = (from n in Nodes where n.DataContext == dataItem select n).FirstOrDefault();

            if (item == null)
            {
                foreach (var childNode in this.Nodes)
                {
                    item = childNode.FindChildNode(dataItem);
                    if (item != null)
                    {
                        return item;
                    }
                }
            }

            return item;
        }

        private Border nodemouseover;
        private Border multiSelection;
        private Border nodeunfocusedselection;
        private Border normal;
        private Border nodeSelection;
        private Border nodeselctionmouseover;

        private Panel _itemsHost;
        internal Panel ItemsHost
        {
            get
            {
                // Lookup the ItemsHost if we haven't already cached it.
                if (_itemsHost == null && this != null && this.ItemContainerGenerator != null)
                {
                    // Get any live container
                    DependencyObject container = this.ItemContainerGenerator.ContainerFromIndex(0);
                    if (container != null)
                    {
                        // Get the parent of the container
                        _itemsHost = VisualTreeHelper.GetParent(container) as Panel;
                    }
                }

                return _itemsHost;
            }
        }

        private Grid PART_Header;
        /// <summary>
        /// Called when an internal process or application calls
        /// ApplyTemplate, which is used to build the current template's
        /// visual tree. 
        /// </summary>
        public override void OnApplyTemplate()
        {
            //_itemsHost = null;
            if (ParentTreeview != null)
            {
                if (this.Items.Count > 0)
                    this.ParentTreeview.itemCount = this.Items.Count;
                else if (this.ParentTreeview.itemCount > 0)
                    this.ParentTreeview.itemCount = this.Items.Count;
            }
            base.OnApplyTemplate();

            if (this.contentpresenter != null)
            {
                this.contentpresenter.LayoutUpdated += new EventHandler(contentpresenter_LayoutUpdated);
            }

            if (this.textbox != null)
            {
                this.textbox.KeyDown -= new KeyEventHandler(textbox_KeyDown);
                this.textbox.LostFocus -= new RoutedEventHandler(textbox_LostFocus);
                this.textbox.TextChanged -= new TextChangedEventHandler(textbox_TextChanged);
            }

            if (this.contenthost != null)
            {
                this.contenthost.MouseEnter -= new MouseEventHandler(treeviewitemgrid_MouseEnter);
                this.contenthost.MouseLeave -= new MouseEventHandler(treeviewitemgrid_MouseLeave);
                this.contenthost.MouseMove -= new MouseEventHandler(treeviewitemgrid_MouseMove);
            }

            if (this.treeviewitemgrid != null)
            {
                this.treeviewitemgrid.MouseEnter -= new MouseEventHandler(treeviewitemgrid_MouseEnter);
                this.treeviewitemgrid.MouseLeave -= new MouseEventHandler(treeviewitemgrid_MouseLeave);
                this.treeviewitemgrid.MouseMove -= new MouseEventHandler(treeviewitemgrid_MouseMove);
            }

            if (this.ParentNode != null && this.ParentNode.ItemsSource != null)
                this.Style = this.ParentNode.Style;

            this.elementRoot = GetTemplateChild(ElementRootName) as FrameworkElement;
            this.itemsHost = GetTemplateChild(ItemsPresenterName) as ItemsPresenter;
            this.expander = GetTemplateChild(ExpanderName) as ToggleButton;
            this.contentpresenter = GetTemplateChild("ContentPresenter") as ContentPresenter;

            this.textbox = GetTemplateChild("TextBox") as TextBox;

            this.leftimage = GetTemplateChild("LeftImage") as Image;
            this.rightimage = GetTemplateChild("RightImage") as Image;
            this.treeviewitemgrid = GetTemplateChild("LayoutRoot") as Grid;
            this.HeaderBorder = this.GetTemplateChild("HeaderBorder") as Border;
            this.LayoutRoot = this.GetTemplateChild("LayoutRoot") as Grid;

            nodemouseover = this.GetTemplateChild("nodemouseover") as Border;
            multiSelection = this.GetTemplateChild("multiSelection") as Border;
            nodeunfocusedselection = this.GetTemplateChild("nodeunfocusedselection") as Border;
            normal = this.GetTemplateChild("normal") as Border;
            nodeSelection = this.GetTemplateChild("nodeSelection") as Border;
            nodeselctionmouseover = this.GetTemplateChild("nodeselctionmouseover") as Border;

            this.HorizontalLine = this.GetTemplateChild("HorizontalRootLine") as Line;
            this.VerticalLine1 = this.GetTemplateChild("VerticalRootLine1") as Rectangle;
            this.VerticalLine2 = this.GetTemplateChild("VerticalRootLine2") as Rectangle;

            this.ExpandIconPart = this.GetTemplateChild("ExpandIconPart") as Image;
            this.CollapsedIconPart = this.GetTemplateChild("CollapsedIconPart") as Image;
            this.contenthost = this.GetTemplateChild("PART_ContentHost") as StackPanel;
            this.PART_Header = this.GetTemplateChild("PART_Header") as Grid;

            if (this.contentpresenter != null)
            {
                this.contentpresenter.LayoutUpdated += new EventHandler(contentpresenter_LayoutUpdated);
            }

            if (this.textbox != null)
            {
                this.textbox.KeyDown += new KeyEventHandler(textbox_KeyDown);
                this.textbox.LostFocus += new RoutedEventHandler(textbox_LostFocus);
                this.textbox.TextChanged += new TextChangedEventHandler(textbox_TextChanged);
            }

            if (this.expander != null)
            {
                this.expander.IsEnabled = this.HasItems;
                this.expander.IsChecked = this.IsExpanded;
                this.expander.Checked += this.ToggleButton_CheckChange;
                this.expander.Unchecked += this.ToggleButton_CheckChange;
            }

            if (this.contenthost != null && this.ParentTreeview != null && this.ParentTreeview.DragOnText)
            {
                this.contenthost.MouseEnter += new MouseEventHandler(treeviewitemgrid_MouseEnter);
                this.contenthost.MouseLeave += new MouseEventHandler(treeviewitemgrid_MouseLeave);
                this.contenthost.MouseMove += new MouseEventHandler(treeviewitemgrid_MouseMove);
            }

            if (this.treeviewitemgrid != null)
            {
                this.treeviewitemgrid.MouseEnter += new MouseEventHandler(treeviewitemgrid_MouseEnter);
                this.treeviewitemgrid.MouseLeave += new MouseEventHandler(treeviewitemgrid_MouseLeave);
                this.treeviewitemgrid.MouseMove += new MouseEventHandler(treeviewitemgrid_MouseMove);
            }

            for (int i = 0; i < this.Items.Count; i++)
            {
                if (this.ItemsSource != null)
                {
                    TreeViewItemAdv m_treeViewItem = this.ItemContainerGenerator.ContainerFromItem(this.Items[i]) as TreeViewItemAdv;
                    if (m_treeViewItem != null)
                        m_treeViewItem.Style = this.ParentTreeview.ItemContainerStyle;
                }
                else
                {
                    if (this.Items[i] is TreeViewItemAdv && this.Style != null)
                    {
                        if (this.ParentTreeview != null)
                            (this.Items[i] as TreeViewItemAdv).Style = this.ParentTreeview.ItemContainerStyle;
                    }
                }
            }
           UpdateVisualState(true);
        }

        void treeviewitemgrid_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.ParentTreeview != null)
            {
                if (this.ParentTreeview.EnableMouseOverEffect)
                {
                    if (this.MouseOverFlag)
                    {
                        if (!this.IsMultiSelect)
                        {
                            if (this.ParentTreeview == null)
                                return;

                            if (this.ParentTreeview.mouseoveritem != null)
                                this.ParentTreeview.mouseoveritem.IsMouseOver = false;

                            if (!this.IsSelected)
                            {
                                this.IsMouseOver = true;
                                this.UpdateVisualState(true);
                            }
                            else
                                this.IsMouseOver = false;
                        }

                        this.ParentTreeview.mouseoveritem = this;
                        if (this.ParentTreeview != null)
                        { ParentTreeview.MouseHoveredItem = this; }
                    }
                    if (this.ParentNode != null)
                        this.ParentNode.MouseOverFlag = false;
                }
            }
        }

        void treeviewitemgrid_MouseLeave(object sender, MouseEventArgs e)
        {
            this.IsMouseOver = false;

            if (this.ParentNode != null)
            {
                this.ParentNode.MouseOverFlag = true;
            }
        }

        internal bool MouseOverFlag = true;
        void treeviewitemgrid_MouseEnter(object sender, MouseEventArgs e)
        {
            if (this.ParentTreeview != null)
            {
                if (this.ParentTreeview.EnableMouseOverEffect)
                {
                    if (this.MouseOverFlag)
                    {
                        if (!this.IsMultiSelect)
                        {
                            if (this.ParentTreeview == null)
                                return;

                            if (this.ParentTreeview.mouseoveritem != null)
                                this.ParentTreeview.mouseoveritem.IsMouseOver = false;

                            if (!this.IsSelected)
                            {
                                this.IsMouseOver = true;
                                this.UpdateVisualState(true);
                            }
                            else
                                this.IsMouseOver = false;
                        }

                        this.ParentTreeview.mouseoveritem = this;
                        if (this.ParentTreeview != null)
                        { ParentTreeview.MouseHoveredItem = this; }
                    }
                    if (this.ParentNode != null)
                        this.ParentNode.MouseOverFlag = false;
                }
            }
        }

        /// <summary>
        /// Handles the MouseLeave event of the contenthost control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        void contenthost_MouseLeave(object sender, MouseEventArgs e)
        {
            this.IsMouseOver = false;
        }

        /// <summary>
        /// Handles the MouseEnter event of the contenthost control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        void contenthost_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!this.IsMultiSelect)
            {
                if (this.ParentTreeview == null)
                {
                    return;
                }

                if (this.ParentTreeview.mouseoveritem != null)
                {
                    this.ParentTreeview.mouseoveritem.IsMouseOver = false;
                    //this.ParentTreeview.mouseoveritem.UpdateVisualState(false);
                }
                if (!this.IsSelected)
                {
                    this.IsMouseOver = true;
                    this.UpdateVisualState(true);
                }
                else
                {
                    this.IsMouseOver = false;
                }
            }

            this.ParentTreeview.mouseoveritem = this;
            //if (this.nodemouseover != null)
            //{
            //    this.nodemouseover.Visibility = Visibility.Visible;
            //    this.nodemouseover.Opacity = 1.0;
            //}
            if (this.ParentTreeview != null)
            { ParentTreeview.MouseHoveredItem = this; }
        }

        /// <summary>
        /// Handles the LayoutUpdated event of the contentpresenter control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void contentpresenter_LayoutUpdated(object sender, EventArgs e)
        {
            double height = this.LeftImageHeight > this.RightImageHeight ? this.LeftImageHeight : this.RightImageHeight;
            height = height > (IsExpanded ? this.ExpandImageHeight : this.CollapseImageHeight) ? height : (IsExpanded ? this.ExpandImageHeight : this.CollapseImageHeight);
            height = height + 5;
            RectangleHeight = (this.contentpresenter.ActualHeight + this.contentpresenter.Margin.Top + 2 + this.contentpresenter.Margin.Bottom > height + 2 ? this.contentpresenter.ActualHeight + this.contentpresenter.Margin.Top + this.contentpresenter.Margin.Bottom : height);// +5;
            //this.RectangleHeight = this.contentpresenter.ActualHeight + 5;
            if (RectangleHeight != height)
            {
            }
            this.VerticalLine3Height = this.RectangleHeight / 2;
            this.RefreshFullRowSelect();
        }

        /// <summary>
        /// Method that handles when the mouse left button is down over the item
        /// </summary>
        /// <param name="e">Contains information about the cursor position</param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            this.Focus();
            TreeViewItemAdv treeviewitem = (TreeViewItemAdv)this;
            TreeViewAdv treeview = treeviewitem.ParentTreeview;
            if (treeview != null)
            {
                treeview.exactItemFromPoint = treeviewitem;
                treeview.treeviewgrid_MouseLeftButtonDown(treeview, e);
            }
        }

        /// <summary>
        /// Called before the <see cref="E:System.Windows.UIElement.MouseMove"/> event occurs.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
        }

        /// <summary>
        /// Handles the LostFocus event of the textbox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void textbox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (this.IsInEditMode == true)
            {
                TextBox texteditor = (TextBox)sender;
                this.Header = texteditor.Text;
                this.contentpresenter.Visibility = Visibility.Visible;
                this.textbox.Visibility = Visibility.Collapsed;

                this.oddClickMade = false;
                TreeViewAdv pTreeView1 = this.ParentTreeview;
                pTreeView1.FireNodeEditedEvent();
            }
        }

        /// <summary>
        /// Handles the KeyDown event of the textbox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.KeyEventArgs"/> instance containing the event data.</param>
        void textbox_KeyDown(object sender, KeyEventArgs e)
        {
            TextBox texteditor = (TextBox)sender;
            if (e.Key == Key.Escape)
            {
                this.contentpresenter.Visibility = Visibility.Visible;
                this.textbox.Visibility = Visibility.Collapsed;
                this.IsInEditMode = false;
                this.ParentTreeview.FireNodeEditCancelledEvent();
                e.Handled = true;
            }
            if (texteditor.Text != string.Empty)
            {
                switch (e.Key)
                {
                    case Key.Enter:
                        this.ParentTreeview.HandleEditMode(false);
                        e.Handled = true;
                        break;
                    default:
                        break;
                }
            }
        }

        void textbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox texteditor = (TextBox)sender;
            NodeEditorCancellableEventArgs nodeArgs = new NodeEditorCancellableEventArgs(this) { Cancel = false, ContinueEditing = true, Text = texteditor.Text };
            nodeArgs = this.ParentTreeview.FireNodeEditorValidateStringEvent(nodeArgs);
            if (nodeArgs.Cancel)
            {
                this.textbox.Text = this.Header.ToString();
            }
            else
            {
                this.textbox.Text = nodeArgs.Text;
            }
            if (!nodeArgs.ContinueEditing)
            {
                this.ParentTreeview.HandleEditMode(false);
            }
        }

        /// <summary>
        /// CHKs the last item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        private bool ChkLastItem(TreeViewItemAdv item)
        {
            TreeViewItemAdv tvItem = item.ParentNode;
            int cnt1 = tvItem.Items.Count;
            int index1 = tvItem.Nodes.IndexOf(item);
            if (index1 == (cnt1 - 1))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the path.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns></returns>
        public TreeViewItemPath GetPath(TreeViewItemAdv node)
        {
            if (this.ParentTreeview == null)
            {
                return null;
            }

            if (node == this.ParentTreeview.root)
            {
                return TreeViewItemPath.Empty;
            }
            else
            {
                Stack<object> stack = new Stack<object>();
                while (node != this.ParentTreeview.root)
                {
                    stack.Push(node);
                    if (node.ParentNode != null)
                    {
                        node = node.ParentNode;
                        if (node == this.ParentTreeview.root)
                        {
                            stack.Push(node);
                        }
                    }
                    else
                    {
                        break;
                    }
                }

                return new TreeViewItemPath(stack.ToArray());
            }
        }

        /// <summary>
        /// Gets the parent items control.
        /// </summary>
        /// <value>The parent items control.</value>
        public ItemsControl ParentItemsControl
        {
            get
            {
                return ItemsControl.ItemsControlFromItemContainer(this);
            }
        }

        /// <summary>
        /// Goes to state.
        /// </summary>
        /// <param name="useTransitions">if set to <c>true</c> [use transitions].</param>
        /// <param name="stateNames">The state names.</param>
        private void GoToState(bool useTransitions, params string[] stateNames)
        {
            if (stateNames != null)
            {
                foreach (string str in stateNames)
                {
                    try
                    {
                        if (VisualStateManager.GoToState(this, str, useTransitions))
                        {
                            return;
                        }
                    }
                    catch
                    {
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Called when [mouse leave].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            if (!this.IsMultiSelect)
            {
                this.IsMouseOver = false;
                this.UpdateVisualState(false);
            }
            this.Opacity = 1;
        }

        /// <summary>
        /// Method that handles when the control has lost focus.
        /// </summary>
        /// <param name="e">On Lost Focus this routed event argument is passed.</param>
        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);
            this.oddClickMade = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
            //if (this.IsSelected && this.IsMouseOver)
            //    VisualStateManager.GoToState(this, "SelectedMouseOver", true);
            //if (this.IsSelected && !this.IsMouseOver)
            //    VisualStateManager.GoToState(this, "Selected", true);
        }

        /// <summary>
        /// Selects the specified selected.
        /// </summary>
        /// <param name="selected">if set to <c>true</c> [selected].</param>
        internal void Select(bool selected)
        {
           
            TreeViewAdv parentTreeView = this.ParentTreeview;
            if (parentTreeView != null)
            {
                parentTreeView.SelectionChgd(this, selected);
                if (selected && this.nodeSelection != null)
                    this.nodeSelection.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Handles the CheckChange event of the ToggleButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void ToggleButton_CheckChange(object sender, EventArgs e)
        {
            this.IsExpanded = this.expander.IsChecked.GetValueOrDefault();

            bool? selectOnExpandChange = null;
            TreeViewItemAdv nodeParent = this;

            while (nodeParent != null && selectOnExpandChange == null)
            {
                selectOnExpandChange = nodeParent.SelectOnExpandChange;
                nodeParent = nodeParent.ParentNode;
            }

            if (selectOnExpandChange == null)
            {
                selectOnExpandChange = this.ParentTreeview.SelectOnExpandChange;
            }

            if (selectOnExpandChange.Value == true)
            {
                if (!this.IsExpanded)
                {
                    if (this.Items.Count > 0)
                    {
                        for (int i = 0; i < this.Items.Count; i++)
                        {
                            if (this.Items[i] is TreeViewItemAdv)
                            {
                                FindInternalSelectedItems(this.Items[i] as TreeViewItemAdv);
                                if (HasSelectedItem)
                                {
                                    this.IsSelected = true;
                                    HasSelectedItem = false;
                                }
                            }
                            else
                            {
                                TreeViewItemAdv treeitem = this.ItemContainerGenerator.ContainerFromItem(this.Items[i]) as TreeViewItemAdv;
                                FindInternalSelectedItems(treeitem);
                                if (HasSelectedItem)
                                {
                                    this.IsSelected = true;
                                    HasSelectedItem = false;
                                }
                            }
                        }
                    }
                }
            }

            if (IsLoadOnDemand)
            {
                
            }

            //if (selectOnExpandChange.Value)
            //{
            //this.IsSelected = true;
            //} 
            //double height = this.itemsHost.ActualHeight;
        }

       

        private void FindInternalSelectedItems(TreeViewItemAdv treeviewitem)
        {
            if (treeviewitem != null && treeviewitem.IsSelected)
            {
                treeviewitem.IsSelected = false;
                HasSelectedItem = true;
            }
            
            if (treeviewitem != null && treeviewitem.Items.Count > 0)
            {
                for (int i = 0; i < treeviewitem.Items.Count; i++)
                {
                    if (treeviewitem.Items[i] is TreeViewItemAdv)
                        FindInternalSelectedItems(treeviewitem.Items[i] as TreeViewItemAdv);
                    else
                    {
                        TreeViewItemAdv actualtreeitem = this.ItemContainerGenerator.ContainerFromItem(treeviewitem.Items[i]) as TreeViewItemAdv;
                        FindInternalSelectedItems(actualtreeitem);
                    }
                       
                }
            }

        }


        #endregion Methods

        #region VisualState
        /// <summary>
        /// Updates the state of the visual.
        /// </summary>
        /// <param name="useTransitions">if set to <c>true</c> [use transitions].</param>
        internal void UpdateVisualState(bool useTransitions)
        {
            TreeViewAdv treeView = this.Parent as TreeViewAdv; 
            if (this.IsSelected && this.nodeSelection != null)
            {
                this.nodeSelection.Height = this.HeaderBorder.ActualHeight;
                this.GoToState(useTransitions, new string[] { "Selected" });
                #region implementation
                if (this.ParentTreeview != null)
                {
                    if (this.ParentTreeview.Theme == "Metro")
                    {
                        if (!this.IsEnabled)
                            this.Foreground = new SolidColorBrush(Colors.Gray);
                        else
                            this.Foreground = new SolidColorBrush(Colors.White);
                    }
                    else if (this.ParentTreeview.Theme == "Office2010Black")
                    {
                        if (!this.IsEnabled)
                            this.Foreground = new SolidColorBrush(Colors.Gray);
                        else
                            this.Foreground = new SolidColorBrush(Colors.White);
                    }

                    else
                    {
                        if (!this.IsEnabled)
                            this.Foreground = new SolidColorBrush(Colors.Gray);
                        else
                            this.Foreground = new SolidColorBrush(Colors.Black);
                    }
                }
                #endregion
            }
            else
            {
                this.GoToState(useTransitions, new string[] { "Unselected" });
                #region implement
                if (this.ParentTreeview != null)
                {
                    if (this.ParentTreeview.Theme == "Metro")
                    {
                        if (!this.IsEnabled)
                            this.Foreground = new SolidColorBrush(Colors.Gray);
                        else
                            this.Foreground = this.ParentTreeview.Foreground;
                    }
                    else if (this.ParentTreeview.Theme == "Blend")
                    {
                        if (!this.IsEnabled)
                            this.Foreground = new SolidColorBrush(Colors.Gray);
                        else
                            this.Foreground = new SolidColorBrush(Colors.White);
                    }
                    else
                    {
                        if (!this.IsEnabled)
                            this.Foreground = new SolidColorBrush(Colors.Gray);
                        else
                            this.Foreground = this.ParentTreeview.Foreground;
                    }
                }
                #endregion

                if (this.IsMouseOver && !this.IsSelected)
                {
                    this.GoToState(useTransitions, new string[] { "MouseOver" });
                }

                else if (!this.IsSelected)
                {
                    this.GoToState(useTransitions, new string[] { "Normal" });
                }
            }

            if (this.IsMouseOver && !this.IsSelected)
            {
                this.GoToState(useTransitions, new string[] { "MouseOver" });
                #region Implement
                if (this.ParentTreeview.Theme == "Office2007Black")
                {
                    if (IsFocused)
                        this.Foreground = new SolidColorBrush(Colors.White);
                }
                #endregion
            }

            else if (!this.IsSelected)
            {
                this.GoToState(useTransitions, new string[] { "Normal" });
            }

            if (!this.IsMouseOver && !this.IsSelected)
            {
                this.GoToState(useTransitions, new string[] { "Normal" });
            }

            if (IsMultiSelect && this.IsSelected)
            {
                this.multiSelection.Height = this.HeaderBorder.ActualHeight;
                if (ParentTreeview != null)
                {
                    if (ParentTreeview.SelectedNodes.Contains(this))
                    {
                        this.GoToState(useTransitions, new string[] { "MultiSelect" });
                    }
                }
            }

            if (this.IsExpanded == true)
            {
                this.GoToState(useTransitions, new string[] { "Expanded" });
                if (this.expander != null)
                {
                    this.expander.IsChecked = true;
                }
            }
            else
            {
                this.GoToState(useTransitions, new string[] { "Collapsed" });
                if (this.expander != null)
                {
                    this.expander.IsChecked = false;
                }
            }

            if (this.HasItems)
            {
                this.GoToState(useTransitions, new string[] { "HasItems", "NoItems" });
            }

            else
            {
                this.GoToState(useTransitions, new string[] { "NoItems" });
            }

            if (IsLoading)
            {
                this.GoToState(useTransitions, new string[] { "Loading" });
            }
            else
            {
                if (this.IsMouseOver && !this.IsSelected)
                {
                    this.GoToState(useTransitions, new string[] { "MouseOver" });
                    #region Implement
                    //if (this.ParentTreeview.Theme == "Office2010Black")
                    //{
                    //    if (IsFocused)
                    //        this.Foreground = new SolidColorBrush(Colors.Gray);
                    //}
                    #endregion
                }

                else if (!this.IsSelected)
                {
                    this.GoToState(useTransitions, new string[] { "Normal" });
                }

                if (!this.IsMouseOver && !this.IsSelected)
                {
                    this.GoToState(useTransitions, new string[] { "Normal" });
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void UpdateUnfocusedState()
        {
            if (this.ParentTreeview.SelectedItem is TreeViewItemAdv)
            {
                foreach (TreeViewItemAdv treeviewitem in this.ParentTreeview.SelectedItems)
                {
                    treeviewitem.nodeunfocusedselection.Height = this.HeaderBorder.ActualHeight;
                    treeviewitem.nodeSelection.Visibility = Visibility.Collapsed;
                    treeviewitem.GoToState(true, new string[] { "UnFocusedSelection" });
                }
            }            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        public Color FromHex(string hex)
        {
            string v = hex.TrimStart('#');
            if (v.Length > 8)
                return Colors.Blue;
            if (v.Length == 6)
                v = "FF" + v;
            if (v.Length < 6)
                v = "FF" + v;
            while (v.Length < 8)
                v += "0";
            Color c = new Color();
            c.A = (byte)System.Convert.ToInt32(v.Substring(0, 2), 16);
            c.R = (byte)System.Convert.ToInt32(v.Substring(2, 2), 16);
            c.G = (byte)System.Convert.ToInt32(v.Substring(4, 2), 16);
            c.B = (byte)System.Convert.ToInt32(v.Substring(6, 2), 16);
            return c;
        }

        #endregion

        #region KeyBoard Navigation
        /// <summary>
        /// Gets a value indicating whether this instance is control key down.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is control key down; otherwise, <c>false</c>.
        /// </value>
        private static bool IsControlKeyDown
        {
            get
            {
                return (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is shift key down.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is shift key down; otherwise, <c>false</c>.
        /// </value>
        private static bool IsShiftKeyDown
        {
            get
            {
                return (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift;
            }
        }

        /// <summary>
        /// Logicals the left.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        private bool LogicalLeft(Key key)
        {
            return key == Key.Left;
        }

        /// <summary>
        /// Gets a value indicating whether this instance can expand.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can expand; otherwise, <c>false</c>.
        /// </value>
        private bool CanExpand
        {
            get
            {
                return this.HasItems;
            }
        }

        /// <summary>
        /// Gets the item.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public TreeViewItemAdv GetItem(int index)
        {
            TreeViewItemAdv item = null;

            if (index > -1 && Items.Count > 0)
            {
                item = Items[index] as TreeViewItemAdv;

                if (item == null)
                {
                    item = ItemContainerGenerator.ContainerFromIndex(index) as TreeViewItemAdv;
                }
            }

            return item;
        }

        /// <summary>
        /// Gets the previous node.
        /// </summary>
        /// <param name="flg2">if set to <c>true</c> [FLG2].</param>
        /// <returns></returns>
        internal TreeViewItemAdv GetPreviousNode(bool flg2)
        {
            TreeViewAdv parentTreeView = this.ParentTreeview;
            TreeViewItemAdv parentTreeViewItem = this.ParentNode;
            TreeViewItemAdv previousTreeViewItem = null;
            int currentIndex = -1;
            int previousIndex = -1;
            if (parentTreeViewItem != null)
            {
                currentIndex = parentTreeViewItem.GetIndex(this as object);//parentTreeViewItem.Items.IndexOf(this);
                if (ItemsSource != null)
                {
                    currentIndex = parentTreeViewItem.GetIndex(this as object);//parentTreeViewItem.Items.IndexOf(this.Header);
                    previousIndex = currentIndex - 1;
                    if (previousIndex > -1)
                    {
                        previousTreeViewItem = parentTreeViewItem.GetItem(previousIndex);
                        //previousTreeViewItem = ((parentTreeView.Items[previousIndex] as TreeViewItemAdv) != null) ? parentTreeView.Items[previousIndex] as TreeViewItemAdv : (TreeViewItemAdv)parentTreeView.ItemContainerGenerator.ContainerFromItem(parentTreeView.Items[previousIndex] as object);
                        if (flg2 && previousTreeViewItem.IsExpanded && previousTreeViewItem.Items.Count > 0)
                        {
                            previousTreeViewItem = previousTreeViewItem.GetItem(previousTreeViewItem.Items.Count - 1);
                        }

                        return previousTreeViewItem;
                    }
                    else
                    {
                        return parentTreeViewItem;
                    }
                }
                else
                {
                    previousIndex = currentIndex - 1;
                    if (previousIndex > -1)
                    {
                        //previousTreeViewItem = parentTreeViewItem.Items[previousIndex] as TreeViewItemAdv;
                        previousTreeViewItem = ((parentTreeViewItem.Items[previousIndex] as TreeViewItemAdv) != null) ? parentTreeViewItem.Items[previousIndex] as TreeViewItemAdv : (TreeViewItemAdv)parentTreeViewItem.ItemContainerGenerator.ContainerFromItem(parentTreeViewItem.Items[previousIndex] as object);
                        if (flg2 && previousTreeViewItem.IsExpanded && previousTreeViewItem.Items.Count > 0)
                        {
                            //previousTreeViewItem = previousTreeViewItem.Items[previousTreeViewItem.Items.Count - 1] as TreeViewItemAdv;
                            previousTreeViewItem = ((previousTreeViewItem.Items[previousTreeViewItem.Items.Count - 1] as TreeViewItemAdv) != null) ? previousTreeViewItem.Items[previousTreeViewItem.Items.Count - 1] as TreeViewItemAdv : (TreeViewItemAdv)previousTreeViewItem.ItemContainerGenerator.ContainerFromItem(previousTreeViewItem.Items[previousTreeViewItem.Items.Count - 1] as object);

                        }

                        return previousTreeViewItem;
                    }
                    else
                    {
                        return parentTreeViewItem;
                    }
                }
            }
            else
            {
                if (ItemsSource != null)
                {
                    currentIndex = parentTreeView.Items.IndexOf(this.Header);
                    previousIndex = currentIndex - 1;
                    if (previousIndex > -1)
                    {
                        previousTreeViewItem = parentTreeView.GetItem(previousIndex);
                        if (flg2 && previousTreeViewItem.IsExpanded && previousTreeViewItem.Items.Count > 0)
                        {
                            previousTreeViewItem = previousTreeViewItem.GetItem(previousTreeViewItem.Items.Count - 1);
                        }

                        return previousTreeViewItem;
                    }
                    else
                    {
                        return parentTreeViewItem;
                    }
                }
                else
                {
                    if (parentTreeView != null)
                    {
                        currentIndex = parentTreeView.GetIndex(this as object);//parentTreeView.Items.IndexOf(this);
                        previousIndex = currentIndex - 1;
                        if (previousIndex > -1)
                        {
                            //previousTreeViewItem = parentTreeView.Items[previousIndex] as TreeViewItemAdv;
                            previousTreeViewItem = ((parentTreeView.Items[previousIndex] as TreeViewItemAdv) != null) ? parentTreeView.Items[previousIndex] as TreeViewItemAdv : (TreeViewItemAdv)parentTreeView.ItemContainerGenerator.ContainerFromItem(parentTreeView.Items[previousIndex] as object);
                            if (flg2 && previousTreeViewItem.IsExpanded && previousTreeViewItem.Items.Count > 0)
                            {
                                // previousTreeViewItem = previousTreeViewItem.Items[previousTreeViewItem.Items.Count - 1] as TreeViewItemAdv;
                                previousTreeViewItem = ((previousTreeViewItem.Items[previousTreeViewItem.Items.Count - 1] as TreeViewItemAdv) != null) ? previousTreeViewItem.Items[previousTreeViewItem.Items.Count - 1] as TreeViewItemAdv : (TreeViewItemAdv)previousTreeViewItem.ItemContainerGenerator.ContainerFromItem(previousTreeViewItem.Items[previousTreeViewItem.Items.Count - 1] as object);

                            }

                            return previousTreeViewItem;
                        }
                        else
                        {
                            return null;
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the index.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public int GetIndex(object obj)
        {
            int index = -1;

            if (obj != null)
            {
                if (obj is TreeViewItemAdv)
                {
                    index = ItemContainerGenerator.IndexFromContainer(obj as TreeViewItemAdv);
                }
                else
                {
                    TreeViewItemAdv item = ItemContainerGenerator.ContainerFromItem(obj) as TreeViewItemAdv;

                    if (item != null)
                    {
                        index = ItemContainerGenerator.IndexFromContainer(item);
                    }
                }
            }

            return index;
        }

        /// <summary>
        /// Gets the next node.
        /// </summary>
        /// <param name="flag">if set to <c>true</c> [flag].</param>
        /// <returns></returns>
        internal TreeViewItemAdv GetNextNode(bool flag)
        {
            TreeViewAdv parentTreeView = this.ParentTreeview;
            TreeViewItemAdv parentTreeViewItem = this.ParentNode;
            //TreeViewItemAdv nextTreeViewItem = null;
            int currentIndex = -1;
            int nextIndex = -1;
            //parentTreeView ---> put this and check
            if (parentTreeViewItem != null)
            {
                if (flag && this.IsExpanded && this.Items.Count > 0)
                {
                    //nextTreeViewItem = this.Items[0] as TreeViewItemAdv;
                    //return nextTreeViewItem;
                    return ((this.Items[0] as TreeViewItemAdv) != null) ? this.Items[0] as TreeViewItemAdv : (TreeViewItemAdv)this.ItemContainerGenerator.ContainerFromItem(this.Items[0] as object);
                }
                else
                {
                    //parentTreeView ---> Put this and check
                    currentIndex = parentTreeViewItem.GetIndex(this as object);//parentTreeViewItem.Items.IndexOf(this);
                    nextIndex = currentIndex + 1;
                    if (nextIndex < parentTreeViewItem.Items.Count)
                    {
                        //nextTreeViewItem = parentTreeViewItem.Items[nextIndex] as TreeViewItemAdv;
                        //return nextTreeViewItem;
                        return ((parentTreeViewItem.Items[nextIndex] as TreeViewItemAdv) != null) ? parentTreeViewItem.Items[nextIndex] as TreeViewItemAdv : (TreeViewItemAdv)parentTreeViewItem.ItemContainerGenerator.ContainerFromItem(parentTreeViewItem.Items[nextIndex] as object);

                    }
                    else
                    {
                        return parentTreeViewItem.GetNextNode(false);
                    }
                }
            }
            else
            {
                if (flag && this.IsExpanded && this.Items.Count > 0)
                {
                    //nextTreeViewItem = this.Items[0] as TreeViewItemAdv;
                    //return nextTreeViewItem; 
                    return ((this.Items[0] as TreeViewItemAdv) != null) ? this.Items[0] as TreeViewItemAdv : (TreeViewItemAdv)this.ItemContainerGenerator.ContainerFromItem(this.Items[0] as object);
                }
                else
                {
                    if (parentTreeView != null)
                    {
                        currentIndex = parentTreeView.GetIndex(this as object);//parentTreeView.Items.IndexOf(this);
                        nextIndex = currentIndex + 1;
                        if (nextIndex < parentTreeView.Items.Count)
                        {
                            //nextTreeViewItem = parentTreeView.Items[nextIndex] as TreeViewItemAdv;
                            //return nextTreeViewItem;
                            return ((parentTreeView.Items[nextIndex] as TreeViewItemAdv) != null) ? parentTreeView.Items[nextIndex] as TreeViewItemAdv : (TreeViewItemAdv)parentTreeView.ItemContainerGenerator.ContainerFromItem(parentTreeView.Items[nextIndex] as object);

                        }
                        else
                        {
                            return null;
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the parent.
        /// </summary>
        /// <param name="e">The e.</param>
        /// <returns></returns>
        public static FrameworkElement GetParent(FrameworkElement e)
        {
            if (!(e is UIElement))
            {
                return e.Parent as FrameworkElement;
            }

            Panel parent = e.Parent as Panel;
            if (parent == null)
            {
                return null;
            }

            return parent.Parent as FrameworkElement;
        }

        /// <summary>
        /// Gets the type of the parent of.
        /// </summary>
        /// <param name="e">The e.</param>
        /// <param name="elementType">Type of the element.</param>
        /// <returns></returns>
        public static FrameworkElement GetParentOfType(FrameworkElement e, Type elementType)
        {
            e = GetParent(e);
            while (e != null)
            {
                if (elementType.IsAssignableFrom(e.GetType()))
                {
                    return e;
                }

                e = GetParent(e);
            }

            return null;
        }

        /// <summary>
        /// Handles up key.
        /// </summary>
        /// <returns></returns>
        internal bool HandleUpKey()
        {
            if (this.IsInEditMode)
            {
                return false;
            }
            this.ParentTreeview.ClearSelectedNodes();
            TreeViewItemAdv previousNode = this.GetPreviousNode(true);
            if (previousNode != null)
            {
                previousNode.Focus();
                previousNode.Select(true);
                previousNode.ParentTreeview.ScrollIntoView(previousNode);
                return true;
            }
            else
            {
                this.Focus();
                this.Select(true);
                this.ParentTreeview.ScrollIntoView(this);
                return true;
            }
        }

        /// <summary>
        /// Handles down key.
        /// </summary>
        /// <returns></returns>
        internal bool HandleDownKey()
        {
            if (this.IsInEditMode && this.textbox.Visibility == Visibility.Visible)
            {
                return false;
            }
            this.ParentTreeview.ClearSelectedNodes();
            TreeViewItemAdv nextNode = this.GetNextNode(true);
            if (nextNode != null)
            {
                nextNode.Focus();
                nextNode.Select(true);
                nextNode.ParentTreeview.ScrollIntoView(nextNode);
                return true;
            }
            else
            {
                this.Focus();
                this.Select(true);
                this.ParentTreeview.ScrollIntoView(this);
                return true;
            }
        }

        /// <summary>
        /// Handles the KeyDown event of the TreeViewItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.KeyEventArgs"/> instance containing the event data.</param>
        void TreeViewItem_KeyDown(object sender, KeyEventArgs e)
        {
            if (!e.Handled)
            {
                switch (e.Key)
                {
                    case Key.Left:
                    case Key.Right:
                        if (!this.LogicalLeft(e.Key))
                        {
                            if (!IsControlKeyDown && this.CanExpand)
                            {
                                if (!this.IsExpanded)
                                {
                                    this.IsExpanded = true;
                                    if (this.expander != null)
                                    {
                                        this.expander.IsChecked = true;
                                    }

                                    e.Handled = true;
                                    return;
                                }
                                if (this.IsSelected)
                                {
                                    this.ParentTreeview.ClearSelectedNodes();
                                    if (this.Items.Count > 0)
                                    {
                                        if (this.ItemsSource == null)
                                        {
                                            (this.Items[0] as TreeViewItemAdv).Focus();
                                            (this.Items[0] as TreeViewItemAdv).Select(true);
                                        }
                                        else
                                        {
                                            IList list = this.ItemsSource as IList;
                                            (this.ItemContainerGenerator.ContainerFromItem(list[0]) as TreeViewItemAdv).Focus();
                                            (this.ItemContainerGenerator.ContainerFromItem(list[0]) as TreeViewItemAdv).Select(true);
                                        }
                                    }
                                    else
                                    {
                                        this.Focus();
                                        this.Select(true);
                                    }
                                    e.Handled = true;
                                }
                            }
                            return;
                        }

                        if ((IsControlKeyDown || !this.CanExpand) || !this.IsExpanded)
                        {
                            return;
                        }

                        if (!this.IsSelected)
                        {
                            this.Focus();
                            this.Select(true);
                            break;
                        }

                        this.IsExpanded = false;
                        if (this.expander != null)
                        {
                            this.expander.IsChecked = false;
                        }

                        break;
                    case Key.Up:
                        if (!IsControlKeyDown && this.HandleUpKey())
                            e.Handled = true;
                        else
                            e.Handled = true;
                        return;

                    case Key.Down:
                        if (!IsControlKeyDown && this.HandleDownKey())
                            e.Handled = true;
                        else
                            e.Handled = true;
                        return;

                    case Key.Add:
                        if (this.CanExpand && !this.IsExpanded)
                        {
                            this.expander.IsChecked = true;
                            e.Handled = true;
                        }
                        return;
                    case Key.Enter:
                        if (this.Items.Count > 0)
                        {
                            this.expander.IsChecked = !this.expander.IsChecked;//true;
                        }
                        break;
                    case Key.F2:
                        if (this.IsSelected)
                        {
                            if (this.ParentTreeview.IsInEditMode)
                            {
                                if (!(this.Header is string))
                                {
                                    return;
                                }
                                this.ParentTreeview.currentItemAdv = this;
                                this.ParentTreeview.HandleEditMode(true);
                            }
                        }
                        break;
                    case Key.Home:
                        if (this.ParentTreeview != null)
                        {
                            this.ParentTreeview.ClearSelectedNodes();
                            (this.ParentTreeview.ItemContainerGenerator.ContainerFromItem(this.ParentTreeview.Items[0]) as TreeViewItemAdv).Focus();
                            (this.ParentTreeview.ItemContainerGenerator.ContainerFromItem(this.ParentTreeview.Items[0]) as TreeViewItemAdv).Select(true);
                            this.ParentTreeview.ScrollIntoView(this.ParentTreeview.ItemContainerGenerator.ContainerFromItem(this.ParentTreeview.Items[0]) as TreeViewItemAdv);
                            e.Handled = true;
                        }
                        break;
                    case Key.End:
                        if (this.ParentTreeview != null)
                        {
                            TreeViewItemAdv itemadv = this.ParentTreeview.GetLastItem();
                            if (itemadv != null)
                            {
                                this.ParentTreeview.ClearSelectedNodes();
                                this.ParentTreeview.ScrollIntoView(itemadv);
                                itemadv.Focus();
                                itemadv.Select(true);
                            }
                        }
                        break;
                    case Key.PageDown:
                        if (this.ParentTreeview != null)
                        {
                            this.ParentTreeview.m_Height = 0.0;
                            this.ParentTreeview.GetItemFromHeight(this.ParentTreeview, this.ParentTreeview.Items, this.ParentTreeview, this.ParentTreeview.elementScrollViewer.ViewportHeight - this.HeaderBorder.ActualHeight);
                            if (this.ParentTreeview.firstlastViewItem != null)
                            {
                                this.ParentTreeview.firstlastViewItem.Select(true);
                                this.ParentTreeview.Focus();
                                this.ParentTreeview.GetItemFromHeight(this.ParentTreeview, this.ParentTreeview.Items, this.ParentTreeview, (this.ParentTreeview.elementScrollViewer.ViewportHeight * 2) - (this.HeaderBorder.ActualHeight * 2));
                                this.ParentTreeview.ScrollIntoView(this.ParentTreeview.firstlastViewItem);
                            }
                            e.Handled = true;
                        }
                        break;
                    case Key.PageUp:
                        if (this.ParentTreeview != null)
                        {
                            TreeViewItemAdv itemadv = this.ParentTreeview.GetFirstVisibleItem();
                            TreeViewAdv.PageUp();
                            e.Handled = true;
                        }
                        break;

                    case Key.Subtract:
                        if (this.CanExpand && this.IsExpanded)
                        {
                            this.expander.IsChecked = false;
                            e.Handled = true;
                        }

                        return;
                    default:
                        return;
                }

                e.Handled = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Left:
                case Key.Right:
                    if (!this.LogicalLeft(e.Key))
                    {
                        if (!IsControlKeyDown && this.CanExpand)
                        {
                            if (!this.IsExpanded)
                            {
                                this.IsExpanded = true;
                                if (this.expander != null)
                                {
                                    this.expander.IsChecked = true;
                                }

                                e.Handled = true;
                                return;
                            }
                            if (this.IsSelected)
                            {
                                this.ParentTreeview.ClearSelectedNodes();
                                if (this.Items.Count > 0)
                                {
                                    if (this.ItemsSource == null)
                                    {
                                        (this.Items[0] as TreeViewItemAdv).Focus();
                                        (this.Items[0] as TreeViewItemAdv).Select(true);
                                    }
                                    else
                                    {
                                        IList list = this.ItemsSource as IList;
                                        (this.ItemContainerGenerator.ContainerFromItem(list[0]) as TreeViewItemAdv).Focus();
                                        (this.ItemContainerGenerator.ContainerFromItem(list[0]) as TreeViewItemAdv).Select(true);
                                    }
                                }
                                else
                                {
                                    this.Focus();
                                    this.Select(true);
                                }
                                e.Handled = true;
                            }
                        }
                        return;
                    }

                    if ((IsControlKeyDown || !this.CanExpand) || !this.IsExpanded)
                    {
                        return;
                    }

                    if (!this.IsSelected)
                    {
                        this.Focus();
                        this.Select(true);
                        break;
                    }

                    this.IsExpanded = false;
                    if (this.expander != null)
                    {
                        this.expander.IsChecked = false;
                    }
                    break;
                case Key.Up:
                    if (!IsControlKeyDown && this.HandleUpKey())
                        e.Handled = true;
                    else
                        e.Handled = true;
                    return;

                case Key.Down:
                    if (!IsControlKeyDown && this.HandleDownKey())
                        e.Handled = true;
                    else
                        e.Handled = true;
                    return;

                case Key.Add:
                    if (this.CanExpand && !this.IsExpanded)
                    {
                        this.expander.IsChecked = true;
                        e.Handled = true;
                    }
                    return;
                case Key.Enter:
                    if (this.Items.Count > 0)
                    {
                        this.expander.IsChecked = !this.expander.IsChecked;//true;
                    }
                    break;
                case Key.F2:
                    if (this.IsSelected)
                    {
                        if (this.ParentTreeview.IsInEditMode)
                        {
                            if (!(this.Header is string))
                            {
                                return;
                            }
                            this.ParentTreeview.currentItemAdv = this;
                            this.ParentTreeview.HandleEditMode(true);
                        }
                    }
                    break;
                case Key.Home:
                    if (this.ParentTreeview != null)
                    {
                        this.ParentTreeview.ClearSelectedNodes();
                        (this.ParentTreeview.ItemContainerGenerator.ContainerFromItem(this.ParentTreeview.Items[0]) as TreeViewItemAdv).Focus();
                        (this.ParentTreeview.ItemContainerGenerator.ContainerFromItem(this.ParentTreeview.Items[0]) as TreeViewItemAdv).Select(true);
                        this.ParentTreeview.ScrollIntoView(this.ParentTreeview.ItemContainerGenerator.ContainerFromItem(this.ParentTreeview.Items[0]) as TreeViewItemAdv);
                        e.Handled = true;
                    }
                    break;
                case Key.End:
                    if (this.ParentTreeview != null)
                    {
                        TreeViewItemAdv itemadv = this.ParentTreeview.GetLastItem();
                        if (itemadv != null)
                        {
                            this.ParentTreeview.ClearSelectedNodes();
                            this.ParentTreeview.ScrollIntoView(itemadv);
                            itemadv.Focus();
                            itemadv.Select(true);
                        }
                    }
                    break;
                case Key.PageDown:
                    if (this.ParentTreeview != null)
                    {
                        this.ParentTreeview.m_Height = 0.0;
                        this.ParentTreeview.GetItemFromHeight(this.ParentTreeview, this.ParentTreeview.Items, this.ParentTreeview, this.ParentTreeview.elementScrollViewer.ViewportHeight - this.HeaderBorder.ActualHeight);
                        if (this.ParentTreeview.firstlastViewItem != null)
                        {
                            this.ParentTreeview.firstlastViewItem.Select(true);
                            this.ParentTreeview.Focus();
                            this.ParentTreeview.GetItemFromHeight(this.ParentTreeview, this.ParentTreeview.Items, this.ParentTreeview, (this.ParentTreeview.elementScrollViewer.ViewportHeight * 2) - (this.HeaderBorder.ActualHeight * 2));
                            this.ParentTreeview.ScrollIntoView(this.ParentTreeview.firstlastViewItem);
                        }
                        e.Handled = true;
                    }
                    break;
                case Key.PageUp:
                    if (this.ParentTreeview != null)
                    {
                        TreeViewItemAdv itemadv = this.ParentTreeview.GetFirstVisibleItem();
                        TreeViewAdv.PageUp();
                        e.Handled = true;
                    }
                    break;

                case Key.Subtract:
                    if (this.CanExpand && this.IsExpanded)
                    {
                        this.expander.IsChecked = false;
                        e.Handled = true;
                    }
                    return;
                //default:
                    //if (this.ParentTreeview != null)
                    //{
                    //    //if (this.timer.IsEnabled)
                    //    //{
                    //    //    char c = (e.Key.ToString().ToCharArray()[0]);
                    //    //    this.ParentTreeview.AutoSearchString += c;
                    //    //    Debug.WriteLine("1" + this.ParentTreeview.AutoSearchString);
                    //    //    timer.Start();
                    //    //}

                    //    //else
                    //    //{
                    //    //    this.ParentTreeview.AutoSearchString = string.Empty;
                    //    //    char c = (e.Key.ToString().ToCharArray()[0]);
                    //    //    this.ParentTreeview.AutoSearchString = c.ToString();
                    //    //    timer.Start();
                    //    //}
                    //    //Console.WriteLine(this.ParentTreeview.AutoSearchString);
                    //    //e.Handled = true;

                    //    //if (this.ParentTreeview.AutoSearchEnabled && !this.timer.IsEnabled)
                    //    //{
                    //    //    timer.Tick += new EventHandler(timer_Tick);
                    //    //    timer.Start();
                    //    //    timer.Stop();
                    //    //}
                    //}
            }
            base.OnKeyDown(e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnTextInput(TextCompositionEventArgs e)
        {
            if (this.IsSelected && this.IsFocused)
            {
                if (this.ParentTreeview != null)
                {
                    if (this.ParentTreeview.AutoSearchTimer.IsEnabled)
                    {
                        string c = e.Text;
                        this.ParentTreeview.AutoSearchString += c;
                        this.ParentTreeview.AutoSearchTimer.Start();
                        this.AutoSearchStartNode = true;
                        this.AutoSearch(0);
                    }
                    else
                    {
                        this.ParentTreeview.AutoSearchString = string.Empty;
                        string c = e.Text;
                        this.ParentTreeview.AutoSearchString = c.ToString();
                        this.ParentTreeview.AutoSearchTimer.Start();
                        this.AutoSearchStartNode = true;
                        this.AutoSearch(0);
                    }
                }
            }
            base.OnTextInput(e);
        }

        #endregion

        internal bool AutoSearchStartNode = false;
        internal bool AutoSearchStopFlag = false;
        internal bool AutoSearch(int StartFrom)
        {
            if (AutoSearchStopFlag == true)
            {
                this.AutoSearchStopFlag = false;
                return false;
            }

            if (this.IsExpanded && StartFrom < this.Items.Count)
            {
                for (int i = StartFrom; i < this.Items.Count; i++)
                {
                    TreeViewItemAdv treeviewitem = (TreeViewItemAdv)this.ItemContainerGenerator.ContainerFromIndex(i);

                    #region AutoSearchHeader
                    if (this.ItemsSource == null)
                    {
                        if (treeviewitem.Header.ToString().StartsWith(this.ParentTreeview.AutoSearchString))
                        {
                            treeviewitem.IsSelected = true;
                            treeviewitem.Focus();
                            treeviewitem.ParentTreeview.ScrollIntoView(treeviewitem);
                            AutoSearchStartNode = false;
                            AutoSearchStopFlag = false;
                            return true;
                        }
                        else if (treeviewitem.IsExpanded)
                        {
                            if (!AutoSearchStartNode)
                                return treeviewitem.AutoSearch(0);
                            else
                            {
                                if (treeviewitem.AutoSearch(0))
                                {
                                    AutoSearchStartNode = false;
                                    AutoSearchStopFlag = false;
                                    return true;
                                }
                            }
                        }
                    }
                    #endregion

                    #region AutoSearchPath
                    else
                    {
                        if (!this.ParentTreeview.DisplayMemberPath.Equals(""))
                        {
                            Type t = this.Items[i].GetType();
                            var prop = t.GetProperty(this.ParentTreeview.DisplayMemberPath);
                            object pathstring = prop.GetValue(this.Items[i], null);

                            if (pathstring.ToString().StartsWith(this.ParentTreeview.AutoSearchString))
                            {
                                treeviewitem.IsSelected = true;
                                treeviewitem.Focus();
                                treeviewitem.ParentTreeview.ScrollIntoView(treeviewitem);
                                AutoSearchStartNode = false;
                                AutoSearchStopFlag = false;
                                return true;
                            }
                            else if (treeviewitem.IsExpanded)
                            {
                                if (!AutoSearchStartNode)
                                    return treeviewitem.AutoSearch(0);
                                else
                                {
                                    if (treeviewitem.AutoSearch(0))
                                    {
                                        AutoSearchStartNode = false;
                                        AutoSearchStopFlag = false;
                                        return true;
                                    }
                                }
                            }
                        }
                        else
                            return false;
                    }
                    #endregion
                }
            }
         
            if (this.ParentNode != null)
            {
                if (!AutoSearchStartNode)
                    return this.ParentNode.AutoSearch(this.ParentNode.ItemContainerGenerator.IndexFromContainer(this) + 1);
                else
                {
                    if (this.ParentNode.AutoSearch(this.ParentNode.ItemContainerGenerator.IndexFromContainer(this) + 1))
                    {
                        AutoSearchStartNode = false;
                        AutoSearchStopFlag = false;
                        return true;
                    }
                }
            }
            else
            {
                if (this.ParentTreeview != null)
                {
                    int index = this.ParentTreeview.ItemContainerGenerator.IndexFromContainer(this);
                    if (!AutoSearchStartNode)
                        return this.ParentTreeview.AutoSearch(index + 1);
                    else
                    {
                        if (this.ParentTreeview.AutoSearch(index + 1))
                        {
                            AutoSearchStartNode = false;
                            AutoSearchStopFlag = false;
                            return true;
                        }
                    }
                }
            }
        
            this.AutoSearchStopFlag = true;
            if (this.ParentTreeview != null)
            {
                AutoSearchStartNode = false;
                AutoSearchStopFlag = false;
                return this.ParentTreeview.AutoSearch(0);
            }

            AutoSearchStartNode = false;
            AutoSearchStopFlag = false;
            return false;
        }

        #region Overrides

        /// <summary>
        /// Method for clearing the container for the item
        /// </summary>
        /// <param name="element">element is a dependency object is used to pass in clearcontainerforitemoverride function.</param>
        /// <param name="item">item is a object is used to pass in clearcontainerforitemoverride function.</param>
        protected override void ClearContainerForItemOverride(DependencyObject element, object item)
        {
            this.Nodes.Remove(item as TreeViewItemAdv);
            base.ClearContainerForItemOverride(element, item);

        }

        /// <summary>
        /// Method for getting the container for the item
        /// </summary>
        /// <returns>Type : TreeViewItemAdv</returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            //if (this.lastContainerCheck != null)
            //{
            //    return this.lastContainerCheck;
            //}

            TreeViewItemAdv itm = new TreeViewItemAdv();
            return itm;
        }

        /// <summary>
        /// Method that determines the item is its own container
        /// </summary>
        /// <param name="item">item is a object is used to check its a TreeViewItemAdv</param>
        /// <returns>Type : bool</returns>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            //this.lastContainerCheck = null;
            if (item is TreeViewItemAdv)
            {
                return true;
            }

            DataTemplate template = ItemTemplate;

            //if (template != null)
            //{
            //    DependencyObject container = template.LoadContent();
            //    if (container is TreeViewItemAdv)
            //    {
            //        this.lastContainerCheck = (TreeViewItemAdv)container;
            //    }
            //}

            return false;
        }

        /// <summary>
        /// Called when the value of the <see cref="P:System.Windows.Controls.ItemsControl.Items"/> property changes.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> that contains the event data</param>
        protected override void OnItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);
            if (e.NewItems == null || e.NewItems.Count == 0)
            {
                if (this.ItemsSource != null)
                {
                    IEnumerator iterator = ItemsSource.GetEnumerator();
                    this.HasItems = iterator.MoveNext();
                }
                else
                {
                    this.HasItems = false;
                }
            }
            else
            {
                this.HasItems = e.NewItems.Count > 0;
            }
            if (this.ParentTreeview != null)
            {
                if (this.Items.Count > 0)
                {
                    this.HasItems = true;
                    if (this.ItemsSource == null)
                    {
                        if (this.Items.Count > 0)
                        {
                            object temp1 = this.Items[0];
                            if (temp1 is TreeViewItemAdv)
                            {
                                this.ParentTreeview.RefreshRootLines(this.ParentTreeview);
                                this.ParentTreeview.RefreshToogleButton();
                                if (this.expander != null)
                                {
                                    this.expander.IsChecked = true;
                                }
                                this.Select(true);
                                this.ParentTreeview.searchfocus = false;
                            }
                        }
                    }
                    else
                    {
                        RefreshRootLinesWithBinding(this);
                        this.ParentTreeview.RefreshRootLines(this.ParentTreeview);
                        this.ParentTreeview.RefreshToogleButton();
                    }
                }
            }
        }

        /// <summary>
        /// Method prepares the container for the item
        /// </summary>
        /// <param name="element">element is used to pass in PrepareContainerForItemOverride function.</param>
        /// <param name="item">is is used to pass in PrepareContainerForItemOverride function.</param>
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
            ContainersToItems[element] = item;
            TreeViewItemAdv tvitem = element as TreeViewItemAdv;
            if (this.ParentTreeview != null)
                tvitem.ExpanderTemplate = this.ParentTreeview.ExpanderTemplate;

            tvitem.ParentTreeview = this.ParentTreeview;
            tvitem.ExpanderVisibility = this.ExpanderVisibility;
            if (tvitem.IsSelected)
            {
                tvitem.Select(true);
                tvitem.UpdateVisualState(true);
            }
            this.Nodes.Add(tvitem);

            TreeViewAdv ptview = this.ParentTreeview;
            TreeViewItemAdv pitem = this.ParentNode;
          
            tvitem.HeaderMargin = new Thickness(this.HeaderMargin.Left + 22 + this.ExpandImageWidth, 0, 0, 0);//new Thickness((pitem.HeaderMargin.Left > 0 ? pitem.HeaderMargin.Left : 22) + 22 + this.ExpandImageWidth, 0, 0, 0);
            tvitem.ParentNode = this;

            if (this.ItemsSource == null)
            {
                if (this.Items.Count > 0)
                {
                    object temp1 = this.Items[0];
                    if (temp1 is TreeViewItemAdv && this.ParentTreeview != null && this.ParentTreeview.RootLineVisibility == System.Windows.Visibility.Visible)
                    {
                        this.ParentTreeview.RefreshRootLines(this.ParentTreeview);
                        this.ParentTreeview.RefreshToogleButton();
                    }
                }
            }
            else
            {
                RefreshRootLinesWithBinding(this);
            }

            if (tvitem.ItemTemplate == null)
            {
                tvitem.ItemTemplate = this.ItemTemplate;
            }

            if (this.lastContainerCheck != null)
            {
                return;
            }


            DataTemplate template = ItemTemplate;
            bool setContent = true;
            if (tvitem != item)
            {
                if (null != template && !(template is HierarchicalDataTemplate))
                {
                    tvitem.HeaderTemplate = template;
                }
                else if (!string.IsNullOrEmpty(DisplayMemberPath))
                {

                    Binding binding = new Binding(DisplayMemberPath);
                    binding.Converter = new MemberValueConverter();
                    tvitem.SetBinding(ContentPresenter.ContentProperty, binding);
                    setContent = false;
                }

                if (setContent)
                {
                    if (tvitem.ItemTemplate != null && tvitem.HeaderTemplate == null)
                    {
                        DataTemplate ownTemplate = tvitem.ItemTemplate;
                        tvitem.HeaderTemplate = ownTemplate;
                    }
                    tvitem.Header = item;
                }
            }
        }

        #endregion

        #region RootLines

        /// <summary>
        /// Refreshes the root lines.
        /// </summary>
        /// <param name="tncoll">The tncoll.</param>
        internal void RefreshRootLines(List<object> tncoll)
        {
            if (tncoll == null)
            {
                return;
            }
            if (this.ItemsSource != null)
            {
                //if (!(tncoll is TreeViewItemAdv))
                //{
                    List<object> collection = new List<object>();
                    foreach (object ob in tncoll)
                    {
                        TreeViewItemAdv m_treeViewItem = (TreeViewItemAdv)this.ItemContainerGenerator.ContainerFromItem(ob);
                        if (m_treeViewItem != null)
                        {
                            collection.Add(m_treeViewItem as object);
                        }
                    }

                    tncoll = collection;

                    if (tncoll.Count == 0)
                    {
                        return;
                    }
                //}
            }

            for (int i = 0; i < tncoll.Count; i++)
            {
                if (tncoll[i] is TreeViewItemAdv)
                {
                    if (((TreeViewItemAdv)tncoll[i]).ParentTreeview != null)
                    {
                        ((TreeViewItemAdv)tncoll[i]).HorizontalLineVisibility = ((TreeViewItemAdv)tncoll[i]).ParentTreeview.RootLineVisibility;
                        ((TreeViewItemAdv)tncoll[i]).VerticalRootLine1Visibility = ((TreeViewItemAdv)tncoll[i]).ParentTreeview.RootLineVisibility;
                        ((TreeViewItemAdv)tncoll[i]).VerticalRootLine2Visibility = ((TreeViewItemAdv)tncoll[i]).ParentTreeview.RootLineVisibility;
                        ((TreeViewItemAdv)tncoll[i]).VerticalRootLine3Visibility = ((TreeViewItemAdv)tncoll[i]).ParentTreeview.RootLineVisibility;
                        ((TreeViewItemAdv)tncoll[i]).RootLineStroke = ((TreeViewItemAdv)tncoll[i]).ParentTreeview.RootLineStroke;

                        ((TreeViewItemAdv)tncoll[i]).DragLineVisibility = Visibility.Collapsed;
                        ((TreeViewItemAdv)tncoll[i]).IsMouseOver = false;

                        if (i == 0 && tncoll.Count == 1)
                        {
                            ((TreeViewItemAdv)tncoll[i]).VerticalRootLine2Visibility = Visibility.Collapsed;
                        }

                        if (i == (tncoll.Count - 1))
                        {
                            ((TreeViewItemAdv)tncoll[i]).VerticalRootLine3Visibility = Visibility.Collapsed;
                            ((TreeViewItemAdv)tncoll[i]).VerticalRootLine2Visibility = Visibility.Collapsed;
                            //if (((TreeViewItemAdv)tncoll[i]).HasItems)
                            //{
                            //    ((TreeViewItemAdv)tncoll[i]).VerticalRootLine1Visibility = Visibility.Collapsed;
                            //}
                        }
                    }
                }
                RefreshRootLines(((TreeViewItemAdv)tncoll[i]).Items.ToList<object>());
                if (this.ParentTreeview != null)
                    this.ParentTreeview.RefreshToogleButton();
            }
            
            return;
        }

        /// <summary>
        /// Refreshes the root lines with binding.
        /// </summary>
        /// <param name="item">The item.</param>
        internal void RefreshRootLinesWithBinding(TreeViewItemAdv item)
        {
            ItemCollection tncoll = item.Items;

            for (int i = 0; i < item.ContainersToItems.Count; i++)
            {
                if ((TreeViewItemAdv)ItemContainerGenerator.ContainerFromIndex(i) != null)
                {
                    if (((TreeViewItemAdv)ItemContainerGenerator.ContainerFromIndex(i)).ParentTreeview != null)
                    {
                        ((TreeViewItemAdv)ItemContainerGenerator.ContainerFromIndex(i)).HorizontalLineVisibility = ((TreeViewItemAdv)ItemContainerGenerator.ContainerFromIndex(i)).ParentTreeview.RootLineVisibility;
                        ((TreeViewItemAdv)ItemContainerGenerator.ContainerFromIndex(i)).VerticalRootLine1Visibility = ((TreeViewItemAdv)ItemContainerGenerator.ContainerFromIndex(i)).ParentTreeview.RootLineVisibility;
                        ((TreeViewItemAdv)ItemContainerGenerator.ContainerFromIndex(i)).VerticalRootLine2Visibility = ((TreeViewItemAdv)ItemContainerGenerator.ContainerFromIndex(i)).ParentTreeview.RootLineVisibility;
                        ((TreeViewItemAdv)ItemContainerGenerator.ContainerFromIndex(i)).VerticalRootLine3Visibility = ((TreeViewItemAdv)ItemContainerGenerator.ContainerFromIndex(i)).ParentTreeview.RootLineVisibility;
                        ((TreeViewItemAdv)ItemContainerGenerator.ContainerFromIndex(i)).RootLineStroke = ((TreeViewItemAdv)ItemContainerGenerator.ContainerFromIndex(i)).ParentTreeview.RootLineStroke;
                    }
                    ((TreeViewItemAdv)ItemContainerGenerator.ContainerFromIndex(i)).DragLineVisibility = Visibility.Collapsed;
                    ((TreeViewItemAdv)ItemContainerGenerator.ContainerFromIndex(i)).IsMouseOver = false;

                    if (i == (tncoll.Count - 1))
                    {
                        ((TreeViewItemAdv)ItemContainerGenerator.ContainerFromIndex(i)).VerticalRootLine2Visibility = Visibility.Collapsed;
                        ((TreeViewItemAdv)ItemContainerGenerator.ContainerFromIndex(i)).VerticalRootLine3Visibility = Visibility.Collapsed;
                        //if (((TreeViewItemAdv)ItemContainerGenerator.ContainerFromIndex(i)).HasItems)
                        //{
                        //    ((TreeViewItemAdv)ItemContainerGenerator.ContainerFromIndex(i)).VerticalRootLine1Visibility = Visibility.Collapsed;
                        //}
                    }
                }
            }
            return;

        }

        #endregion

        internal TreeViewItemAdv GetLastSubItem()
        {
            TreeViewItemAdv itemFound = null;

            for (int i = Items.Count - 1; i >= 0; i--)
            {
                TreeViewItemAdv item = Items[i] as TreeViewItemAdv;

                if (item == null)
                {
                    item = ItemContainerGenerator.ContainerFromIndex(i) as TreeViewItemAdv;
                }

                if (item != null && item.Visibility == Visibility.Visible)
                {
                    if (item.Items.Count > 0)
                    {
                        if (item.IsExpanded == true)
                            itemFound = item.GetLastSubItem();
                        else
                            itemFound = item;
                    }
                    else
                        itemFound = item;
                    break;
                }
            }
            return itemFound;
        }

        internal void RefreshFullRowSelect()
        {
            this.RootLineMargin = new Thickness(0, this.RectangleHeight / 2, 0, this.RectangleHeight / 2);
            if (this.ParentTreeview != null)
            {
                Thickness thickness = new Thickness(this.HeaderMargin.Left + 22, 0, 0, 0);
                thickness = this.ParentTreeview.FullRowSelect ? new Thickness(0) : thickness;
                if (nodemouseover != null)
                {
                    if (this.LayoutRoot.ActualWidth > 0)
                    {
                        if (this.LayoutRoot.ActualWidth > this.ParentTreeview.ActualWidth)
                            nodemouseover.Width = this.ParentTreeview.FullRowSelect ? this.ParentTreeview.ActualWidth - 1 : this.HeaderBorder.ActualWidth;
                        else
                            nodemouseover.Width = this.ParentTreeview.FullRowSelect ? this.LayoutRoot.ActualWidth - 1 : this.HeaderBorder.ActualWidth;
                    }
                    nodemouseover.Margin = thickness;
                }
                if (this.multiSelection != null)
                {
                    multiSelection.Margin = thickness;
                    if (this.LayoutRoot.ActualWidth > 0)
                    {
                        if (this.LayoutRoot.ActualWidth > this.ParentTreeview.ActualWidth)
                            multiSelection.Width = this.ParentTreeview.FullRowSelect ? this.ParentTreeview.ActualWidth - 1 : this.HeaderBorder.ActualWidth;
                        else
                            multiSelection.Width = this.ParentTreeview.FullRowSelect ? this.LayoutRoot.ActualWidth - 1 : this.HeaderBorder.ActualWidth;
                    }
                }
                if (this.nodeunfocusedselection != null)
                {
                    nodeunfocusedselection.Margin = thickness;
                    if (this.LayoutRoot.ActualWidth > 0)
                    {
                        if (this.LayoutRoot.ActualWidth > this.ParentTreeview.ActualWidth)
                            nodeunfocusedselection.Width = this.ParentTreeview.FullRowSelect ? this.ParentTreeview.ActualWidth - 1 : this.HeaderBorder.ActualWidth;
                        else
                            nodeunfocusedselection.Width = this.ParentTreeview.FullRowSelect ? this.LayoutRoot.ActualWidth - 1 : this.HeaderBorder.ActualWidth;
                    }

                }
                if (this.normal != null)
                {
                    normal.Margin = thickness;

                    if (this.LayoutRoot.ActualWidth > 0)
                    {
                        if (this.LayoutRoot.ActualWidth > this.ParentTreeview.ActualWidth)
                            normal.Width = this.ParentTreeview.FullRowSelect ? this.ParentTreeview.ActualWidth - 1 : this.HeaderBorder.ActualWidth;
                        else
                            normal.Width = this.ParentTreeview.FullRowSelect ? this.LayoutRoot.ActualWidth - 1 : this.HeaderBorder.ActualWidth;
                    }
                }
                if (this.nodeSelection != null)
                {
                    nodeSelection.Margin = thickness;

                    if (this.LayoutRoot.ActualWidth > 0)
                    {
                        if (this.LayoutRoot.ActualWidth > this.ParentTreeview.ActualWidth)
                            nodeSelection.Width = this.ParentTreeview.FullRowSelect ? this.ParentTreeview.ActualWidth - 1 : this.HeaderBorder.ActualWidth;
                        else
                            nodeSelection.Width = this.ParentTreeview.FullRowSelect ? this.LayoutRoot.ActualWidth - 1 : this.HeaderBorder.ActualWidth;
                    }
                }
                if (this.nodeselctionmouseover != null)
                {
                    nodeselctionmouseover.Margin = thickness;
                    if (this.LayoutRoot.ActualWidth > 0)
                    {
                        if (this.LayoutRoot.ActualWidth > this.ParentTreeview.ActualWidth)
                            nodeselctionmouseover.Width = this.ParentTreeview.FullRowSelect ? this.ParentTreeview.ActualWidth - 1 : this.HeaderBorder.ActualWidth;
                        else
                            nodeselctionmouseover.Width = this.ParentTreeview.FullRowSelect ? this.LayoutRoot.ActualWidth - 1 : this.HeaderBorder.ActualWidth;
                    }
                }

                if (nodemouseover != null)
                {
                    nodemouseover.Margin = thickness;
                    if (this.LayoutRoot.ActualWidth > 0)
                    {
                        if (this.LayoutRoot.ActualWidth > this.ParentTreeview.ActualWidth)
                            nodemouseover.Width = this.ParentTreeview.FullRowSelect ? this.ParentTreeview.ActualWidth - 1 : this.HeaderBorder.ActualWidth;
                        else
                            nodemouseover.Width = this.ParentTreeview.FullRowSelect ? this.LayoutRoot.ActualWidth - 1 : this.HeaderBorder.ActualWidth;
                    }
                }
            }
        }

        private static void OnExpanderVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TreeViewItemAdv obj = (TreeViewItemAdv)d;
            obj.RefreshToogleButton();
        }

        internal void RefreshToogleButton()
        {
            for (int i = 0; i < this.Items.Count; i++)
            {
                if ((this.ItemContainerGenerator.ContainerFromItem(this.Items[i])) != null)
                    (this.ItemContainerGenerator.ContainerFromItem(this.Items[i]) as TreeViewItemAdv).ExpanderVisibility = this.ExpanderVisibility;//Visibility.Collapsed
                else if (this.Items[i] is TreeViewItemAdv)
                    ((TreeViewItemAdv)this.Items[i]).ExpanderVisibility = this.ExpanderVisibility;
            }
        }

    }
}

