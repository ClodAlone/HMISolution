// <copyright file="GroupBarItem.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

#region file using
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Documents;
using System.Windows.Shapes;
using System.Diagnostics;
using System.IO.Packaging;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Markup;
using System.Globalization;
using System.Windows.Media.Imaging;
using System.Collections.Specialized;
using Syncfusion.Windows.Shared;
using Syncfusion.Licensing;
using System.Collections;
#endregion

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents the GroupBarItem UI element.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif

    [SkinType(SkinVisualStyle = Skin.Office2007Blue,
  Type = typeof(GroupBarItem), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/GroupBar/Themes/Office2007BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Black,
    Type = typeof(GroupBarItem), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/GroupBar/Themes/Office2007BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Silver,
    Type = typeof(GroupBarItem), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/GroupBar/Themes/Office2007SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Blue,
 Type = typeof(GroupBar), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/GroupBar/Themes/Office2010BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Black,
    Type = typeof(GroupBar), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/GroupBar/Themes/Office2010BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Silver,
    Type = typeof(GroupBar), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/GroupBar/Themes/Office2010SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2003,
     Type = typeof(GroupBarItem), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/GroupBar/Themes/Office2003Style.xaml")]
    [SkinType(SkinVisualStyle = Skin.ShinyBlue,
     Type = typeof(GroupBarItem), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/GroupBar/Themes/ShinyBlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.ShinyRed,
     Type = typeof(GroupBarItem), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/GroupBar/Themes/ShinyRedStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.SyncOrange,
     Type = typeof(GroupBarItem), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/GroupBar/Themes/SyncOrangeStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Blend,
    Type = typeof(GroupBarItem), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/GroupBar/Themes/BlendStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Default,
    Type = typeof(GroupBarItem), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/GroupBar/Themes/DefaultStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.VS2010,
     Type = typeof(GroupBarItem), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/GroupBar/Themes/VS2010Style.xaml")]
    [SkinType(SkinVisualStyle = Skin.Metro,
     Type = typeof(GroupBarItem), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/GroupBar/Themes/MetroStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Transparent ,
   Type = typeof(GroupBarItem), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/GroupBar/Themes/TransparentStyle.xaml")]
   
    public class GroupBarItem : HeaderedContentControl,IDisposable
    {
        #region Constants
        /// <summary>
        /// Name of the header host from template.
        /// </summary>
        private const string C_nameHeaderHost = "HeaderHost";
        
        /// <summary>
        /// Message for the main host if it is not found.
        /// </summary>
        private const string C_errorHeaderHost = "HeaderHost not found";
        
        /// <summary>
        /// Name of the content host border from template.
        /// </summary>
        private const string C_nameContentHostBorder = "ContentHostBorder";
        
        /// <summary>
        /// Message for the main host if it is not found.
        /// </summary>
        private const string C_errorContentHostBorder = "ContentHostBorder not found";
        
        /// <summary>
        /// Message for the main host if it is not found.
        /// </summary>
        private const int C_Offset = 5;
        #endregion

        #region Private members
        /// <summary>
        /// Indicates whether applying the animation to the control is allowed.
        /// </summary>
        private bool m_enableAnimation = true;
        
        /// <summary>
        /// Indicates whether space key is pressed.
        /// </summary>
        private bool m_isSpaceKeyDown = false;
        
        /// <summary>
        /// Adorner is shown when the current item is dragged.
        /// </summary>
        private DragAdorner m_dragAdorner;
        
        /// <summary>
        /// Adorner is shown when the current item is dragged.
        /// </summary>
        private DragMarkerAdorner m_dragMarkerAdorner;
        
        /// <summary>
        /// Point on the item header that stores the mouse cursor coordinates
        /// when left mouse button is pressed.
        /// </summary>
        private Point m_mouseDownPoint;
        
        /// <summary>
        /// Indicates whether an item is currently dragged.
        /// </summary>
        private bool m_isDragging = false;
        
        /// <summary>
        /// Element in the control's visual tree that hosts the header.
        /// </summary>
        private FrameworkElement m_visualHeader;
        
        /// <summary>
        /// Element in the control's visual tree that hosts the content. Used
        /// only in non-stack mode template.
        /// </summary>
        private FrameworkElement m_visualContent;
        
        /// <summary>
        /// Animation is applied to the content when the control is selected. Used
        /// only in non-stack mode.
        /// </summary>
        private DoubleAnimation m_selectedAnimation;
        
        /// <summary>
        /// Animation is applied to the content when the control is unselected. Used
        /// only in non-stack mode.
        /// </summary>
        private DoubleAnimation m_unselectedAnimation;
        
        /// <summary>
        /// Indicates whether the custom animation is applied when mouse
        /// enters the header.
        /// </summary>
        private bool m_customMouseEnterApplied;
        
        /// <summary>
        /// Indicates whether the custom animation is applied when mouse
        /// leaves the header.
        /// </summary>
        private bool m_customMouseLeaveApplied;
        
        /// <summary>
        /// Available height for the content of the item.
        /// </summary>
        private double m_contentLength = 0;

        private double m_contentBreadth = 0;
        
        /// <summary>
        /// Indicates that keyboard is focused on one of the controls.
        /// </summary>
        private static bool keyboardFocused = false;
        
        /// <summary>
        /// Indicates that mouse hovers over one of the controls.
        /// </summary>
        private static bool mouseHovers = false;

        /////// <summary>
        /////// Default background.
        /////// </summary>
        ////private Brush m_defaultBackground = null;

        /////// <summary>
        /////// Default Foreground.
        /////// </summary>
        ////private Brush m_defaultForeground = null;
        
        /// <summary>
        /// Contains default visual style key name.
        /// </summary>
        private const string C_defaultVisStyleName = "Default";

        /// <summary>
        /// Identifies whether the <see cref="Syncfusion.Windows.Tools.Controls.GroupBarItem"/> is hidden or not.
        /// </summary>
        private bool m_isHidden = false;

        public event PropertyChangedCallback HeaderStyleChanged;
        #endregion

        #region Dependency properties
        /////// <summary>
        /////// Identifies the <see cref="DefaultBackground"/> dependency property.
        /////// </summary>
        ////internal static readonly DependencyProperty DefaultBackgroundProperty = DependencyProperty.Register("DefaultBackground", typeof(Brush), typeof(GroupBarItem), new FrameworkPropertyMetadata(Brushes.Transparent));
       
        /////// <summary>
        /////// Identifies the <see cref="DefaultForeground"/> dependency property.
        /////// </summary>
        ////internal static readonly DependencyProperty DefaultForegroundProperty = DependencyProperty.Register("DefaultForeground", typeof(Brush), typeof(GroupBarItem), new FrameworkPropertyMetadata(Brushes.Black));
        
        /// <summary>
        /// Identifies <see cref="CustomAnimations"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CustomAnimationsProperty = DependencyProperty.Register("CustomAnimations", typeof(CustomAnimationsCollection), typeof(GroupBarItem), new UIPropertyMetadata(null));
        
        /// <summary>
        /// Identifies <see cref="ShowInGroupBar"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ShowInGroupBarProperty = DependencyProperty.Register("ShowInGroupBar", typeof(bool), typeof(GroupBarItem), new UIPropertyMetadata(true, OnShowInGroupBarChanged));
        
        /// <summary>
        /// Identifies <see cref="HeaderText"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HeaderTextProperty = DependencyProperty.Register("HeaderText", typeof(string), typeof(GroupBarItem), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(OnHeaderTextChanged)));
        
        /// <summary>
        /// Identifies <see cref="HeaderImageSource"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HeaderImageSourceProperty = DependencyProperty.Register("HeaderImageSource", typeof(ImageSource), typeof(GroupBarItem), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(OnHeaderImageSourceChanged)));
        
        /// <summary>
        /// Identifies <see cref="IsSelected"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsSelectedProperty = Selector.IsSelectedProperty.AddOwner(typeof(GroupBarItem), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Journal | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsParentMeasure, new PropertyChangedCallback(GroupBarItem.OnIsSelectedChanged)));
        
        /// <summary>
        /// Identifies <see cref="IsPressed"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsPressedProperty = DependencyProperty.Register("IsPressed", typeof(bool), typeof(GroupBarItem), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsPressedChanged), new CoerceValueCallback(CoerceIsPressedChanged)));
        
        /// <summary>
        /// Identifies <see cref="IsExpanded"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsExpandedProperty = DependencyProperty.Register("IsExpanded", typeof(bool), typeof(GroupBarItem), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(GroupBarItem.OnIsExpandedChanged)));
        
        /// <summary>
        /// Identifies <see cref="IsInEditMode"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsInEditModeProperty = DependencyProperty.Register("IsInEditMode", typeof(bool), typeof(GroupBarItem), new FrameworkPropertyMetadata(false, OnIsInEditModeChanged));
        
        /// <summary>
        /// Identifies <see cref="IsLastItemProperty"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsLastItemProperty = DependencyProperty.Register("IsLastItem", typeof(bool), typeof(GroupBarItem), new UIPropertyMetadata(false));
        /// <summary>
        /// Identifies <see cref="GroupBarItemCornerRadius"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty GroupBarItemCornerRadiusProperty = DependencyProperty.Register("GroupBarItemCornerRadius", typeof(CornerRadius), typeof(GroupBarItem), new UIPropertyMetadata(new CornerRadius(0d),new PropertyChangedCallback(OnGroupBarItemCornerRadiusChanged)));
       
        /// <summary>
        /// Identifies <see cref="IsHighlighted"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsHighlightedProperty = DependencyProperty.Register("IsHighlighted", typeof(bool), typeof(GroupBarItem), new UIPropertyMetadata(false));
        
        /// <summary>
        /// Content's animated property. Within horizontal layout -
        /// width, within vertical layout - height.
        /// </summary>
        private DependencyProperty m_animatedContentProperty;
        
        /// <summary>
        /// Identifies <see cref="IsDragOverTop"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsDragOverTopProperty = DependencyProperty.Register("IsDragOverTop", typeof(bool), typeof(GroupBarItem), new UIPropertyMetadata(false));
       
        /// <summary>
        /// Identifies <see cref="IsDragOverDown"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsDragOverDownProperty = DependencyProperty.Register("IsDragOverDown", typeof(bool), typeof(GroupBarItem), new UIPropertyMetadata(false));
        
        /// <summary>
        /// Identifies <see cref="IsContextMenuOpened"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsContextMenuOpenedProperty = DependencyProperty.Register("IsContextMenuOpened", typeof(bool), typeof(GroupBarItem), new UIPropertyMetadata(false));
        
        /// <summary>
        /// Identifies <see cref="IsAnimating"/> dependency property.
        /// </summary>
        /// <remarks>
        /// Used for setting ScrollBarVisibility when animation is executing.
        /// </remarks>
        public static readonly DependencyProperty IsAnimatingProperty = DependencyProperty.Register("IsAnimating", typeof(bool), typeof(GroupBarItem), new FrameworkPropertyMetadata(false));
        
        /// <summary>
        /// Identifies <see cref="SelectedAnimation"/> dependency property.
        /// </summary>
        public static DependencyProperty SelectedAnimationProperty = DependencyProperty.Register("SelectedAnimation", typeof(DoubleAnimation), typeof(GroupBarItem), new UIPropertyMetadata(null, OnSelectedAnimationChanged));
        
        /// <summary>
        /// Identifies <see cref="UnselectedAnimation"/> dependency property.
        /// </summary>
        public static DependencyProperty UnselectedAnimationProperty = DependencyProperty.Register("UnselectedAnimation", typeof(DoubleAnimation), typeof(GroupBarItem), new UIPropertyMetadata(null, OnUnselectedAnimationChanged));



        public Style HeaderStyle
        {
            get { return (Style)GetValue(HeaderStyleProperty); }
            set { SetValue(HeaderStyleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HeaderStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderStyleProperty =
            DependencyProperty.Register("HeaderStyle", typeof(Style), typeof(GroupBarItem), new FrameworkPropertyMetadata(null,FrameworkPropertyMetadataOptions.Inherits,new PropertyChangedCallback(OnHeaderStyleChanged)));

        
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
        /// Gets the logical parent.
        /// </summary>
        /// <value>The logical parent.</value>
        public GroupBar LogicalParent
        {
            get
            {
                return ItemsControl.ItemsControlFromItemContainer(this) as GroupBar;
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
        /// Gets or sets a value indicating whether [show in group bar].
        /// </summary>
        /// <value><c>true</c> if [show in group bar]; otherwise, <c>false</c>.</value>
        public bool ShowInGroupBar
        {
            get
            {
                return (bool)GetValue(ShowInGroupBarProperty);
            }

            set
            {
                SetValue(ShowInGroupBarProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the header text.
        /// </summary>
        /// <value>The header text.</value>
        public string HeaderText
        {
            get
            {
                return (string)GetValue(HeaderTextProperty);
            }

            set
            {
                OnBeforeEdit();
                SetValue(HeaderTextProperty, value);
                OnAfterEdit();
            }
        }
        
        /// <summary>
        /// Gets or sets the source of the image shown in the header.
        /// </summary>
        /// <value>
        /// Type: <see cref="ImageSource"/>
        /// Default value is null.
        /// </value>
        /// <seealso cref="ImageSource"/>
        /// <example> 
        ///    // Create a new instance of the GroupBar
        ///    GroupBar groupBar = new GroupBar();
        ///    // Add GroupBar to grid.
        ///    grid1.Children.Add( groupBar );
        ///    // Create a new instance of the GroupBarItem
        ///    GroupBarItem barItem1 = new GroupBarItem();
        ///    // Add item to GroupBar.Items collection
        ///    groupBar.Items.Add( barItem1 );
        ///    // Set content of GroupBarItem as GroupView
        ///    barItem1.Content = new CalendarEdit();
        ///    // Create bitmap image
        ///    BitmapImage bmpImage = new BitmapImage(new Uri("pack://application:,,/Images/picture.png"));
        ///    // Set image for the header of the GroupBarItem
        ///    barItem1.HeaderImageSource = bmpImage;
        /// /*
        /// Result: 
        ///     Chosen image will be displayed on the header of the GroupBarItem.
        /// */ 
        /// </example>
        public ImageSource HeaderImageSource
        {
            get
            {
                return (ImageSource)GetValue(HeaderImageSourceProperty);
            }

            set
            {
                SetValue(HeaderImageSourceProperty, value);
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
                return (bool)GetValue(GroupBarItem.IsSelectedProperty);
            }

            set
            {
                SetValue(GroupBarItem.IsSelectedProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is last item.
        /// </summary>
        /// <value>
        /// true if this instance is last item; otherwise, false.
        /// </value>
        public bool IsLastItem
        {
            get
            {
                return (bool)GetValue(IsLastItemProperty);
            }

            set
            {
                SetValue(IsLastItemProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the Corner Radius of the GroupBarItem
        /// </summary>
        /// <value>
        /// Type: <see cref="CornerRadius"/>
        /// Default value is 0.
        /// </value>
        /// <example> 
        ///    // Create a new instance of the GroupBar
        ///    GroupBar groupBar = new GroupBar();
        ///    // Add GroupBar to grid.
        ///    grid1.Children.Add( groupBar );
        ///    // Create a new instance of the GroupBarItem
        ///    GroupBarItem barItem1 = new GroupBarItem();
        ///    // Add item to GroupBar.Items collection
        ///    groupBar.Items.Add( barItem1 );
        ///    // Set content of GroupBarItem as GroupView
        ///    barItem1.Content = new CalendarEdit();
        ///   // Set the CornerRadius of the GroupBarItem
        ///    barItem1.GroupBarItemCornerRadius=new CornerRadius(20d);
        /// /*
        /// Result: 
        ///    GroupBarItems,Cornerradius is set to 20.
        /// */ 
        /// </example>
        public CornerRadius GroupBarItemCornerRadius
        {
            get
            {
                return (CornerRadius)GetValue(GroupBarItemCornerRadiusProperty);
            }

            set
            {
                SetValue(GroupBarItemCornerRadiusProperty, value);
            }
        }
        /// <summary>
        /// Gets a value indicating whether this instance is pressed.
        /// </summary>
        /// <value>
        /// true if this instance is pressed; otherwise, false
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
        /// Gets or sets a value indicating whether this instance is expanded.
        /// </summary>
        /// <value>
        /// true if this instance is expanded; otherwise, false
        /// </value>
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
        /// Gets a value indicating whether this instance is highlighted.
        /// </summary>
        /// <value>
        /// true if this instance is highlighted; otherwise, false
        /// </value>
        public bool IsHighlighted
        {
            get
            {
                return (bool)GetValue(IsHighlightedProperty);
            }

            private set
            {
                SetValue(IsHighlightedProperty, value);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is dragging.
        /// </summary>
        /// <value>
        /// true if this instance is dragging; otherwise, false.
        /// </value>
        internal bool IsDragging
        {
            get
            {
                return m_isDragging;
            }

            private set
            {
                m_isDragging = value;
            }
        }

        /// <summary>
        /// Gets the content of the visual.
        /// </summary>
        /// <value>The content of the visual.</value>
        internal FrameworkElement VisualContent
        {
            get
            {
                return m_visualContent;
            }
        }

        /// <summary>
        /// Gets the visual header.
        /// </summary>
        /// <value>The visual header.</value>
        internal FrameworkElement VisualHeader
        {
            get
            {
                return m_visualHeader;
            }
        }

        /// <summary>
        /// Gets or sets the length of the content.
        /// </summary>
        /// <value>The length of the content.</value>
        internal double ContentLength
        {
            get
            {
                return m_contentLength;
            }

            set
            {
                if (value != m_contentLength)
                {
                    m_contentLength = value;
                }
            }
        }

        internal double ContentBreadth
        {
            get
            {
                return m_contentBreadth;
            }

            set
            {
                if (value != m_contentBreadth)
                {
                    m_contentBreadth = value;
                }
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [enable animation].
        /// </summary>
        /// <value>true if [enable animation]; otherwise, false.</value>
        private bool EnableAnimation
        {
            get
            {
                return m_enableAnimation;
            }

            set
            {
                m_enableAnimation = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is space key down.
        /// </summary>
        /// <value>
        /// true if this instance is space key down; otherwise, false
        /// </value>
        private bool IsSpaceKeyDown
        {
            get
            {
                return m_isSpaceKeyDown;
            }

            set
            {
                m_isSpaceKeyDown = value;
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
                    m_dragAdorner = new DragAdorner(m_visualHeader);
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
        /// Gets or sets a value indicating whether this instance is context menu opened.
        /// </summary>
        /// <value>
        /// true if this instance is context menu opened; otherwise, false.
        /// </value>
        public bool IsContextMenuOpened
        {
            get
            {
                return (bool)GetValue(IsContextMenuOpenedProperty);
            }

            set
            {
                SetValue(IsContextMenuOpenedProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is animating.
        /// </summary>
        /// <value>
        /// true if this instance is animating; otherwise, false.
        /// </value>
        public bool IsAnimating
        {
            get
            {
                return (bool)GetValue(IsAnimatingProperty);
            }

            set
            {
                SetValue(IsAnimatingProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the selected animation.
        /// </summary>
        /// <value>The selected animation.</value>
        public DoubleAnimation SelectedAnimation
        {
            get
            {
                return (DoubleAnimation)GetValue(SelectedAnimationProperty);
            }

            set
            {
                SetValue(SelectedAnimationProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the unselected animation.
        /// </summary>
        /// <value>The unselected animation.</value>
        public DoubleAnimation UnselectedAnimation
        {
            get
            {
                return (DoubleAnimation)GetValue(UnselectedAnimationProperty);
            }

            set
            {
                SetValue(UnselectedAnimationProperty, value);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="Syncfusion.Windows.Tools.Controls.GroupBarItem"/> is in hidden state or not.
        /// </summary>
        /// <value><c>true</c> if this instance is hidden; otherwise, <c>false</c>.</value>
        public bool IsHidden
        {
            get
            {
                return m_isHidden;
            }
            internal set
            {
                m_isHidden = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is deleted.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is deleted; otherwise, <c>false</c>.
        /// </value>
        internal bool IsDeleted
        {
            get;

            set;
        }
        #endregion

        #region Events
        /// <summary>
        /// Identifies <see cref="Click"/> event.
        /// </summary>
        public static readonly RoutedEvent ClickEvent = EventManager.RegisterRoutedEvent("Click", RoutingStrategy.Bubble, typeof(EventHandler), typeof(GroupBarItem));
        
        /// <summary>
        /// Bubbling routed event is fired when mouse click on the header is
        /// performed.
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
        /// Identifies <see cref="DoubleClick"/> event.
        /// </summary>
        public static readonly RoutedEvent DoubleClickEvent = EventManager.RegisterRoutedEvent("DoubleClick", RoutingStrategy.Bubble, typeof(EventHandler), typeof(GroupBarItem));
        
        /// <summary>
        /// Bubbling routed event is fired when mouse double click on the header is
        /// performed.
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
        /// Identifies <see cref="Press"/> event.
        /// </summary>
        public static readonly RoutedEvent PressEvent = EventManager.RegisterRoutedEvent("Press", RoutingStrategy.Bubble, typeof(EventHandler), typeof(GroupBarItem));
        
        /// <summary>
        /// Bubbling routed event is fired when the header is pressed.
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
        /// Identifies <see cref="Hover"/> event.
        /// </summary>
        public static readonly RoutedEvent HoverEvent = EventManager.RegisterRoutedEvent("Hover", RoutingStrategy.Bubble, typeof(EventHandler), typeof(GroupBarItem));
        
        /// <summary>
        /// Bubbling routed event is fired when mouse hovers over the header.
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
        /// Identifies <see cref="AfterEdit"/> event.
        /// </summary>
        public static readonly RoutedEvent AfterEditEvent = EventManager.RegisterRoutedEvent("AfterEdit", RoutingStrategy.Bubble, typeof(EventHandler), typeof(GroupBarItem));
        
        /// <summary>
        /// Bubbling routed event is fired after renaming the GroupBarItem.
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
        /// Identifies <see cref="BeforeEdit"/> event.
        /// </summary>
        public static readonly RoutedEvent BeforeEditEvent = EventManager.RegisterRoutedEvent("BeforeEdit", RoutingStrategy.Bubble, typeof(EventHandler), typeof(GroupBarItem));
        
        /// <summary>
        /// Bubbling routed event is fired before renaming the GroupBarItem.
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
        #endregion

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="GroupBarItem"/> class.
        /// </summary>
        public GroupBarItem()
            : base()
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                if (Application.Current != null && !BindingUtils.GetEnableBindingErrors(Application.Current.MainWindow))
                    System.Diagnostics.PresentationTraceSources.DataBindingSource.Switch.Level = System.Diagnostics.SourceLevels.Critical;
            }

            if (DesignerProperties.GetIsInDesignMode(this))
            {
                InitializeInput();
            }
            this.Unloaded += new RoutedEventHandler(GroupBarItem_Unloaded);
        }

        void GroupBarItem_Unloaded(object sender, RoutedEventArgs e)
        {
            //this.Dispose();
        }
        
        /// <summary>
        /// Initializes the input.
        /// </summary>
        private void InitializeInput()
        {
            if (EnvironmentTestTools.IsSecurityGranted)
            {
                EnvironmentTestTools.StartValidateLicense(typeof(GroupBarItem));
            }
            //// Process the user input in design time using InputManager.
            InputManager.Current.PostProcessInput += new ProcessInputEventHandler(InputManager_PostProcessInput);
        }

        /// <summary>
        /// Initializes static members of the <see cref="GroupBarItem"/> class.
        /// </summary>
        static GroupBarItem()
        {
           // EnvironmentTest.ValidateLicense(typeof(GroupBarItem));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GroupBarItem), new FrameworkPropertyMetadata(typeof(GroupBarItem)));
            VisibilityProperty.OverrideMetadata(typeof(GroupBarItem), new UIPropertyMetadata(Visibility.Visible, OnVisibilityChanged));
        }
        
        /// <summary>
        /// Initializes the control in the given template.
        /// </summary>
        /// <param name="inTemplate">Template to initialize control in.</param>
        private void Initialize(ControlTemplate inTemplate)
        {
            m_visualHeader = inTemplate.FindName(C_nameHeaderHost, this) as FrameworkElement;
            (m_visualHeader as Border).CornerRadius = this.GroupBarItemCornerRadius;

            if (m_visualHeader == null)
            {
                throw new ApplicationException(C_errorHeaderHost);
            }

            if (LogicalParent != null && LogicalParent.VisualMode != VisualMode.StackMode)
            {
                m_visualContent = inTemplate.FindName(C_nameContentHostBorder, this) as FrameworkElement;

                if (m_visualContent == null && LogicalParent.VisualMode != VisualMode.StackMode)
                {
                    throw new ApplicationException(C_errorContentHostBorder);
                }
            }

            m_dragAdorner = null;

            if (CustomAnimations != null)
            {
                OwnerTemlpateMapping nativeMapping = new OwnerTemlpateMapping();
                nativeMapping.Owner = this;
                nativeMapping.Template = Template;

                OwnerTemlpateMapping headerMapping = new OwnerTemlpateMapping();
                GroupBarItemHeader header = Header as GroupBarItemHeader;

                if (header != null)
                {
                    header.ApplyTemplate();
                    headerMapping.Owner = header;
                    headerMapping.Template = header.Template;
                }

                CustomAnimations.Initialize(nativeMapping, headerMapping);

                m_customMouseEnterApplied = CustomAnimations.FindAnimation(m_visualHeader.Name, MouseEnterEvent) != null;
                m_customMouseLeaveApplied = CustomAnimations.FindAnimation(m_visualHeader.Name, MouseLeaveEvent) != null;
            }

            m_visualHeader.MouseEnter += new MouseEventHandler(OnMouseEnterVisualHeader);
            m_visualHeader.MouseLeave += new MouseEventHandler(OnMouseLeaveVisualHeader);

            if (LogicalParent != null && LogicalParent.VisualMode != VisualMode.StackMode)
            {
                UpdateContentSize();
                UpdateContentAnimation();
            }
        }
        
        /// <summary>
        /// Initializes predefined animations according to the visual style.
        /// </summary>
        private void InitializePredefinedAnimation()
        {
            if (SelectedAnimation != null)
            {
                m_selectedAnimation = SelectedAnimation.Clone();
                m_selectedAnimation.Completed += new EventHandler(Animation_Completed);
            }

            if (UnselectedAnimation != null)
            {
                m_unselectedAnimation = UnselectedAnimation.Clone();
                m_unselectedAnimation.Completed += new EventHandler(Animation_Collapse_Completed);
            }

            UpdateContentAnimation();
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Invoked when the control is selected.
        /// </summary>
        protected virtual void OnSelected()
        {
            FireRoutedEvent(Selector.SelectedEvent);
        }
        
        /// <summary>
        /// Invoked when the control is unselected.
        /// </summary>
        protected virtual void OnUnselected()
        {
            FireRoutedEvent(Selector.UnselectedEvent);
        }
        
        /// <summary>
        /// Invoked when the mouse performs click on the header.
        /// </summary>
        protected virtual void OnClick()
        {
            FireRoutedEvent(ClickEvent);
        }
        
        /// <summary>
        /// Invoked when mouse performs double click on the header.
        /// </summary>
        protected virtual void OnDoubleClick()
        {
            FireRoutedEvent(DoubleClickEvent);
        }
        
        /// <summary>
        /// Invoked after renaming the GroupBarItem.
        /// </summary>
        internal virtual void OnAfterEdit()
        {
            FireRoutedEvent(AfterEditEvent);
        }
        
        /// <summary>
        /// Invoked before renaming GroupBarItem.
        /// </summary>
        internal virtual void OnBeforeEdit()
        {
            FireRoutedEvent(BeforeEditEvent);
        }
        
        /// <summary>
        /// Invoked when the header is pressed.
        /// </summary>
        protected virtual void OnPress()
        {
            FireRoutedEvent(PressEvent);
        }
        
        /// <summary>
        /// Invoked when mouse hovers over the header.
        /// </summary>
        protected virtual void OnHover()
        {
            FireRoutedEvent(HoverEvent);
        }
        
        /// <summary>
        /// Captures the focus. In non-stack mode tries to set the focus on
        /// the content, then in the case of failure, on the header. In stack mode
        /// gives the focus to the logical parent.
        /// </summary>
        internal void CaptureFocus()
        {
            object focusedElement = LogicalParent.VisualMode == VisualMode.StackMode ?
                LogicalParent.SelectedContent : Content;

            if (!SetFocusOn(focusedElement))
            {
                SetFocusOn(Header);
            }
        }
        
        /// <summary>
        /// Focuses the header.
        /// </summary>
        internal void FocusHeader()
        {
            GroupBarItemHeader header = Header as GroupBarItemHeader;

            if (header != null)
            {
                header.Focus();
            }
        }
        
        /// <summary>
        /// Gets a value indicating whether the header is focused.
        /// </summary>
        internal bool IsFocusedHeader
        {
            get
            {
                bool focused = false;

                GroupBarItemHeader header = Header as GroupBarItemHeader;

                if (header != null)
                {
                    focused = header.IsFocused;
                }

                return focused;
            }
        }
        
        /// <summary>
        /// Tries to set the focus on the given object.
        /// </summary>
        /// <param name="obj">Object to set focus on.</param>
        /// <returns>
        /// Value indicating whether the focus has been successfully set.
        /// </returns>
        internal bool SetFocusOn(object obj)
        {
            bool ret = false;
            UIElement element = obj as UIElement;

            if (element != null)
            {
                element.Focus();
                ret = true;
            }

            return ret;
        }
       
        /// <summary>
        /// Sets the given visibility to all adorners on the given UI element.
        /// </summary>
        /// <param name="onElement">Element to search adorners for.</param>
        /// <param name="newVisibility">New visibility which will be
        /// applied to all adorners.</param>
        internal void SetAdornersVisibility(UIElement onElement, Visibility newVisibility)
        {
            if (onElement != null)
            {
                AdornerLayer layer = GroupBar.GetAdornerLayer(this);

                if (layer != null)
                {
                    Adorner[] adorners = layer.GetAdorners(onElement);

                    if (adorners != null)
                    {
                        for (int i = adorners.Length - 1; i >= 0; --i)
                        {
                            adorners[i].Visibility = newVisibility;
                        }
                    }
                }
            }
        }
        
        /// <summary>
        /// Updates the size of the content according to the logical parent's
        /// <see cref="Syncfusion.Windows.Tools.Controls.GroupBar.ItemContentLengthProperty"/> property. Used only in non-stack mode.
        /// </summary>
        internal void UpdateContentSize()
        {
            UpdateContentSize(IsExpanded);
        }
        
        /// <summary>
        /// Updates the size of the content according to the logical parent's
        /// <see cref="Syncfusion.Windows.Tools.Controls.GroupBar.ItemContentLengthProperty"/> property and sets forced selection to
        /// the control. Used only in non-stack mode.
        /// </summary>
        /// <param name="forcedSelection">Value indicating whether
        /// the control should be selected or unselected.</param>
        internal void UpdateContentSize(bool forcedSelection)
        {
            GroupView view = this.Content as GroupView;
            EnableAnimation = false;
            DetachAnimation(m_visualContent, m_animatedContentProperty);

            if (m_visualContent != null)
            {
                GroupBar groupBar=null;
                if(this.LogicalParent != null)
                 groupBar = this.LogicalParent as GroupBar;                
                bool bMultipleExpansion;
                if (groupBar != null && groupBar.FitContent)
                {
                    bMultipleExpansion = (this.LogicalParent != null && (this.LogicalParent.VisualMode == VisualMode.MultipleExpansion || this.LogicalParent.VisualMode == VisualMode.Default));
                }
                else
                {
                    bMultipleExpansion = this.LogicalParent != null && this.LogicalParent.VisualMode == VisualMode.MultipleExpansion;
                }
                if (this.LogicalParent != null && this.LogicalParent.Orientation == Orientation.Horizontal)
                {
                    m_visualContent.SetBinding(FrameworkElement.HeightProperty, Binder.Bind<GroupBarItem>(this, "ContentLength"));

                        if (forcedSelection)
                        {
                            if (bMultipleExpansion)
                            {
                                m_visualContent.SetBinding(FrameworkElement.WidthProperty, Binder.Bind<GroupBarItem>(this, "ContentBreadth"));
                            }
                            else
                            {
                                m_visualContent.SetBinding(FrameworkElement.WidthProperty, Binder.Bind<GroupBar>(LogicalParent, "ItemContentLength"));
                            }
                        }
                        else
                        {
                            m_visualContent.Width = 0;
                        }

                        m_animatedContentProperty = WidthProperty;
                }
                else
                {
                    m_visualContent.SetBinding(FrameworkElement.WidthProperty, Binder.Bind<GroupBarItem>(this, "ContentBreadth"));

                    if (forcedSelection)
                    {
                        if (bMultipleExpansion)
                        {
                            m_visualContent.SetBinding(FrameworkElement.HeightProperty, Binder.Bind<GroupBarItem>(this, "ContentLength"));
                        }
                        else
                        {
                            m_visualContent.SetBinding(FrameworkElement.HeightProperty, Binder.Bind<GroupBar>(LogicalParent, "ItemContentLength"));
                        }
                    }
                    else
                    {
                        m_visualContent.Height = 0;
                    }

                    m_animatedContentProperty = HeightProperty;
                }
            }

            EnableAnimation = true;
        }

        /// <summary>
        /// Updates the content animation. properties of the animations applied to
        /// content on selection/unselection. Used only in non-stack mode.
        /// </summary>
        internal void UpdateContentAnimation()
        {
            if (m_selectedAnimation != null && m_unselectedAnimation != null)
            {
                if (this.LogicalParent != null && this.LogicalParent.VisualMode == VisualMode.MultipleExpansion)
                {
                    if (LogicalParent.Orientation == Orientation.Vertical)
                    {
                        m_selectedAnimation.To = m_unselectedAnimation.From = this.ContentLength;
                    }
                    else
                    {
                        GroupView view = this.Content as FrameworkElement as GroupView;
                        if (view != null)
                        {
                            m_selectedAnimation.To = m_unselectedAnimation.From = view.ActualWidth;
                        }
                        else
                        {
                            m_selectedAnimation.To = m_unselectedAnimation.From = LogicalParent.ItemContentLength;
                        }
                    }
                }
                else
                {
                    if (null != LogicalParent && LogicalParent.ItemContentLength > 0)
                    {
                        m_selectedAnimation.To = m_unselectedAnimation.From = LogicalParent.ItemContentLength;
                    }
                    else
                    {
                        m_selectedAnimation.To = m_unselectedAnimation.From = 0;
                    }
                }
            }
        }
        
        /// <summary>
        /// Updates the length of the content.
        /// </summary>
        internal void UpdateContentLength()
        {
            FrameworkElement contentElement = this.Content as FrameworkElement;

            GroupView view = contentElement as GroupView;

            if (contentElement != null)
            {
                if (LogicalParent.Orientation == Orientation.Horizontal)
                {
                    if (view != null && LogicalParent.VisualMode == VisualMode.MultipleExpansion)
                    {
                        view.UpdateLayout();
                        ContentLength = view.ActualWidth;
                    }
                    else
                    {
                        ContentLength = contentElement.DesiredSize.Width;
                    }
                }
                else
                {
                    if (view != null && view.MainHost != null)
                    {
                        ContentLength = view.MainHost.ActualHeight;
                    }
                    else
                    {
                        double actualHeight = GetActualHeight(contentElement);
                        ContentLength = actualHeight + C_Offset;
                         if(this.Content is FrameworkElement && actualHeight == 0)
                        {
                            (this.Content as FrameworkElement).Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                            if (contentElement is ScrollViewer)
                            {
                                ContentLength = ((this.Content as FrameworkElement).DesiredSize.Height);
                            }
                        }
                    }
                }
            }
            else
            {
                if (this.LogicalParent.ItemsSource != null && this.LogicalParent.VisualMode == VisualMode.MultipleExpansion)
                {
                    if (this.LogicalParent.Orientation == Orientation.Vertical)
                    {
                        this.ContentLength = this.LogicalParent.ActualHeight - (this.LogicalParent.ItemHeaderHeight * this.LogicalParent.Items.Count);
                    }
                }
                else
                {
                    this.ContentLength = 0;
                }
            }

            UpdateContentAnimation();
        }

        internal void UpdateContentBreadth()
        {
            FrameworkElement contentElement = this.Content as FrameworkElement;

            GroupView view = contentElement as GroupView;

            if (contentElement != null)
            {
                if (LogicalParent.Orientation == Orientation.Horizontal)
                {
                    if (view != null && LogicalParent.VisualMode == VisualMode.MultipleExpansion)
                    {
                        view.UpdateLayout();
                        ContentBreadth = view.ActualHeight;
                    }
                    else
                    {
                        ContentBreadth = contentElement.DesiredSize.Height;
                    }
                }
                else
                {
                    if (view != null && view.MainHost != null)
                    {
                        ContentBreadth = view.MainHost.ActualWidth;
                    }
                    else
                    {
                        double actualWidth = GetActualWidth(contentElement);
                        ContentBreadth = actualWidth + C_Offset;
                        if (this.Content is FrameworkElement && actualWidth == 0)
                        {
                            (this.Content as FrameworkElement).Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                            if (contentElement is ScrollViewer)
                            {
                                ContentBreadth = ((this.Content as FrameworkElement).DesiredSize.Width);
                            }
                        }
                    }
                }
            }
            else
            {
                if (this.LogicalParent.ItemsSource != null && this.LogicalParent.VisualMode == VisualMode.MultipleExpansion)
                {
                    if (this.LogicalParent.Orientation == Orientation.Vertical)
                    {
                        this.ContentLength = this.LogicalParent.ActualHeight - (this.LogicalParent.ItemHeaderHeight * this.LogicalParent.Items.Count);
                    }
                }
                else
                {
                    this.ContentLength = 0;
                }
            }

            UpdateContentAnimation();
        }

        /// <summary>
        /// Get Actual Height of the Visual tree children.
        /// </summary>
        /// <param name="visual"></param>
        /// <returns></returns>
        private double GetActualHeight(Visual visual)
        {
            FrameworkElement element = visual as FrameworkElement;
            
            if (null != element)
            {
                if (element.Height > 0)
                {
                    return element.Height;
                }
                else
                {
                    if (VisualTreeHelper.GetChildrenCount(element) > 1)
                    {
                        return GetChildrenActualHeight(element);
                    }
                    else
                    {
                        if (VisualUtils.FindDescendant(element, typeof(FrameworkElement)) != null)
                        {
                            return GetActualHeight(VisualUtils.FindDescendant(element, typeof(FrameworkElement)));
                        }
                        else
                        {
                            return element.ActualHeight;
                        }
                    }
                }
            }
            return 0;
        }

        private double GetActualWidth(Visual visual)
        {
            FrameworkElement element = visual as FrameworkElement;

            if (null != element)
            {
                if (element.Width > 0)
                {
                    return element.Width;
                }
                else
                {
                    if (VisualTreeHelper.GetChildrenCount(element) > 1)
                    {
                        return GetChildrenActualWidth(element);
                    }
                    else
                    {
                        if (VisualUtils.FindDescendant(element, typeof(FrameworkElement)) != null)
                        {
                            return GetActualWidth(VisualUtils.FindDescendant(element, typeof(FrameworkElement)));
                        }
                        else
                        {
                            return element.ActualWidth;
                        }
                    }
                }
            }
            return 0;
        }

        /// <summary>
        /// Get Children Actual height
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        private double GetChildrenActualHeight(FrameworkElement element)
        {
            double height = 0;
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
            {
                FrameworkElement item = VisualTreeHelper.GetChild(element, i) as FrameworkElement;
                height += item.ActualHeight;
            }
            return height;
        }

        private double GetChildrenActualWidth(FrameworkElement element)
        {
            double width = 0;
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
            {
                FrameworkElement item = VisualTreeHelper.GetChild(element, i) as FrameworkElement;
                width += item.ActualWidth;
            }
            return width;
        }
        /// <summary>
        /// Invoked when mouse enters the header.
        /// </summary>
        /// <param name="sender">Source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> that contains the event data.</param>
        private void OnMouseEnterVisualHeader(object sender, MouseEventArgs e)
        {
            mouseHovers = true;
            CorrectKeyboardFocus(this);

            if (!keyboardFocused)
            {
                StartOnEnterAnimation();
                IsHighlighted = true;
            }

            OnHover();
        }
        
        /// <summary>
        /// Invoked when mouse leaves the header.
        /// </summary>
        /// <param name="sender">Source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> that contains the event data.</param>
        private void OnMouseLeaveVisualHeader(object sender, MouseEventArgs e)
        {
            if (keyboardFocused)
            {
                Keyboard.Focus(LogicalParent);
            }
            else
            {
                StartOnLeaveAnimation();
                IsHighlighted = false;
            }

            mouseHovers = false;
        }
        
        /// <summary>
        /// Starts the animation when mouse enters the header.
        /// </summary>
        private void StartOnEnterAnimation()
        {
        }
        
        /// <summary>
        /// Starts the animation when mouse leaves the header.
        /// </summary>
        private void StartOnLeaveAnimation()
        {
        }
        
        /// <summary>
        /// Tries to add the given adorner to the adorner layer of the current
        /// instance.
        /// </summary>
        /// <param name="adorner">Adorner to add to the adorner layer.</param>
        /// <returns>
        /// Value indicating whether the adorner has been successfully added
        /// to the adorner layer.
        /// </returns>
        private bool TryAddAdorner(Adorner adorner)
        {
            bool ret = false;

            if (adorner != null)
            {
                AdornerLayer layer = GroupBar.GetAdornerLayer(this);

                if (layer != null)
                {
                    layer.Add(adorner);
                    ret = true;
                }
            }

            return ret;
        }
        
        /// <summary>
        /// Tries to remove the given adorner from the adorner layer of the current
        /// instance.
        /// </summary>
        /// <param name="adorner">Adorner to remove from the adorner layer.</param>
        /// <returns>
        /// Value indicating whether the adorner has been successfully
        /// removed from the adorner layer.
        /// </returns>
        private bool TryRemoveAdorner(Adorner adorner)
        {
            bool ret = false;

            if (adorner != null)
            {
                AdornerLayer adornerLayer = GroupBar.GetAdornerLayer(this);

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
        /// Tries to clear all adorners from adorner layer of current
        /// instance.
        /// </summary>
        /// <returns>
        /// Value indicating whether all adorners have been successfully
        /// removed from the adorner layer.
        /// </returns>
        internal bool TryClearAdorners()
        {
            bool ret = false;
            AdornerLayer layer = GroupBar.GetAdornerLayer(this);

            if (layer != null && m_visualHeader != null)
            {
                Adorner[] adorners = layer.GetAdorners(VisualHeader);

                if (adorners != null)
                {
                    for (int i = adorners.Length - 1; i >= 0; --i)
                    {
                        layer.Remove(adorners[i]);
                    }

                    ret = true;
                }
            }

            return ret;
        }
        
        /// <summary>
        /// Updates <see cref="IsPressed"/> property in accordance with the current mouse
        /// position.
        /// </summary>
        private void UpdateIsPressed()
        {
            Point currentPoint = Mouse.PrimaryDevice.GetPosition(this);

            if ((currentPoint.X >= 0 && currentPoint.X <= m_visualHeader.ActualWidth)
                && (currentPoint.Y >= 0 && currentPoint.Y <= m_visualHeader.ActualHeight))
            {
                IsPressed = true;
            }
            else
            {
                IsPressed = false;
            }
        }
        
        /// <summary>
        /// If dragging has not been started yet, method defines whether
        /// it is the time to start dragging operation. If dragging is
        /// already being processed, left and top offsets are calculated
        /// and dragging adorner is updated.
        /// </summary>
        private void UpdateDragging()
        {
            Point currentPosition = Mouse.GetPosition(LogicalParent);
            double leftOffset = currentPosition.X - m_mouseDownPoint.X;
            double topOffset = currentPosition.Y - m_mouseDownPoint.Y;

            if (LogicalParent.FlowDirection == FlowDirection.RightToLeft)
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
            }
        }
        
        /// <summary>
        /// Updates the adorner marker for the dragging item.
        /// </summary>
        private void UpdateAdornerMarker()
        {
            if (this.LogicalParent != null)
            {
                //foreach (GroupBarItem item in LogicalParent.Items)
                //{
                //    item.TryRemoveAdorner(item.AdornerMarker);
                //    item.IsDragOverTop = false;
                //    item.IsDragOverDown = false;
                //}

                for (int i = 0; i < LogicalParent.Items.Count; i++)
                {
                    GroupBarItem item = LogicalParent.ItemContainerGenerator.ContainerFromIndex(i) as GroupBarItem;

                    item.TryRemoveAdorner(item.AdornerMarker);
                    item.IsDragOverTop = false;
                    item.IsDragOverDown = false;
                }

                Point currentPosition = Mouse.GetPosition(LogicalParent);
                FrameworkElement dragingElement = LogicalParent.InputHitTest(currentPosition) as FrameworkElement;

                if (dragingElement != null && IsDragging)
                {
                    GroupBarItem dragingItem = dragingElement.TemplatedParent as GroupBarItem;

                    if (dragingItem != null && dragingItem != this)
                    {
                        Point itemPosition = Mouse.GetPosition(dragingItem);

                        if (itemPosition.Y > dragingItem.ActualHeight / 2)
                        {
                            dragingItem.IsDragOverDown = true;
                        }
                        else
                        {
                            dragingItem.IsDragOverTop = true;
                        }

                        dragingItem.TryAddAdorner(dragingItem.AdornerMarker);
                        dragingItem.AdornerMarker.TopOffset = dragingItem.IsDragOverDown ? dragingItem.ActualHeight : 0;
                    }
                }
            }
        }
        
        /// <summary>
        /// Defines whether given point, relative to the logical parent, lies
        /// on the header.
        /// </summary>
        /// <param name="pointOnParent">Point relative to the logical parent
        /// to be checked.</param>
        /// <returns>
        /// Value indicating whether given point lies on the header.
        /// </returns>
        private bool IsPointOnHeader(Point pointOnParent)
        {
            if (m_visualHeader == null)
            {
                return false;
            }
            if (LogicalParent != null)
            {
                Point headerPoint = LogicalParent.TranslatePoint(pointOnParent, m_visualHeader);
                return m_visualHeader.InputHitTest(headerPoint) != null;
            }
            return false;
        }
        
        /// <summary>
        /// Detaches animation, if any, from the given property of the
        /// given element. Detaching animation unfreezes property
        /// allowing thus changing its value.
        /// </summary>
        /// <param name="owner">Element for which the animation
        /// should be detached.</param>
        /// <param name="property">Property for which animation should
        /// be detached.</param>
        private void DetachAnimation(FrameworkElement owner, DependencyProperty property)
        {
            if (owner != null && property != null)
            {
                owner.BeginAnimation(property, null);
            }
        }
        
        /// <summary>
        /// Sets the visibility of the control in accordance with
        /// <see cref="Syncfusion.Windows.Tools.Controls.GroupBarItem.ShowInGroupBarProperty"/> property.
        /// </summary>
        private void SetVisibility()
        {
            Visibility = ShowInGroupBar ? Visibility.Visible : Visibility.Collapsed;
        }
        
        /// <summary>
        /// Invoked when visibility of the control is changed.
        /// </summary>
        /// <param name="d">Source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private static void OnVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GroupBarItem owner = d as GroupBarItem;

            if (owner != null && owner.VisualHeader != null && e != null)
            {
                GroupBar gBar = owner.LogicalParent as GroupBar;
                if (gBar != null)
                {
                    Visibility newValue = (Visibility)e.NewValue;
                    Visibility oldValue = (Visibility)e.OldValue;

                    UpdateVisibleItemsCount(newValue, oldValue, gBar);

                    owner.SetAdornersVisibility(owner.VisualHeader, newValue);

                    if (gBar.VisualMode == VisualMode.StackMode)
                    {
                        StackModeVisibilityChanged(e, owner, newValue, gBar);
                    }
                    else
                    {
                        owner.SetAdornersVisibility(owner.VisualHeader, newValue);

                        if (gBar.Items.Contains(owner))
                        {
                            gBar.SelectedTab = owner;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Updates count of the visible items and height of the StackItemsHost (StackItemsHost is the container of the items)
        /// </summary>
        /// <param name="newValue">The new visibility value.</param>
        /// <param name="oldValue">The old visibility value.</param>
        /// <param name="gBar">parent of the item.</param>
        private static void UpdateVisibleItemsCount(Visibility newValue, Visibility oldValue, GroupBar gBar)
        {
            double itemsHeight = 0;
            if (gBar != null)
            {
                if (gBar.StackItemsHost != null)
                {
                    itemsHeight = gBar.StackItemsHost.Height;

                    if (double.IsNaN(itemsHeight))
                    {
                        itemsHeight = gBar.StackItemsHost.ActualHeight;
                    }
                }

                if (newValue == Visibility.Collapsed && gBar.VisibleItemsCount > 0)
                {
                    gBar.VisibleItemsCount--;

                    if (gBar.StackItemsHost != null && itemsHeight > gBar.ItemHeaderHeight)
                    {
                        gBar.StackItemsHost.Height = itemsHeight - gBar.ItemHeaderHeight;
                    }
                }
                else if (gBar.VisibleItemsCount < gBar.Items.Count && oldValue != Visibility.Hidden)
                {
                    gBar.VisibleItemsCount++;

                    if (gBar.StackItemsHost != null)
                    {
                        gBar.StackItemsHost.Height = itemsHeight + gBar.ItemHeaderHeight;
                    }
                }
            }
        }
        
        /// <summary>
        /// Stacks the mode visibility changed.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        /// <param name="owner">The owner.</param>
        /// <param name="newValue">The new value.</param>
        /// <param name="gBar">The g bar.</param>
        private static void StackModeVisibilityChanged(DependencyPropertyChangedEventArgs e, GroupBarItem owner, Visibility newValue, GroupBar gBar)
        {
            if (gBar != null && !gBar.IsSplitting)
            {
                if (gBar.Toolbar != null)
                {
                    gBar.CalculateItemContentLength(gBar.Toolbar.ActualHeight);
                }

                FrameworkElement content = owner.Content as FrameworkElement;
                FrameworkElement header = gBar.HeaderContent as FrameworkElement;
                int index = gBar.Items.IndexOf(owner);
                if (newValue == Visibility.Collapsed)
                {
                    int itemsCnt = gBar.Items.Count;

                    gBar.HiddenIndices.Add(index);
                    if (itemsCnt > index && index < itemsCnt - 1)
                    {
                        if (owner == gBar.SelectedTab && owner.ShowInGroupBar)
                        {
                            gBar.SelectedTab = owner;
                        }

                        if (header != null && owner.Visibility == Visibility.Visible)
                        {
                            header.Visibility = Visibility.Visible;
                        }
                    }
                    else if (index == itemsCnt - 1)
                    {
                        if (index - 1 != -1)
                        {
                            if (owner == gBar.SelectedTab && owner.ShowInGroupBar)
                            {
                                ////gBar.SelectedTab = gBar.Items[ index - 1 ] as GroupBarItem;
                            }

                            if (header != null && owner.Visibility == Visibility.Visible)
                            {
                                header.Visibility = Visibility.Visible;
                            }
                        }
                        else
                        {
                            SetContentVisibility(newValue, content, header);
                        }
                  }

                    SetContentVisibility(newValue, content, header);
                    for (int i = 0; i < gBar.Items.Count; i++)
                    {
                        GroupBarItem gitem = gBar.ItemContainerGenerator.ContainerFromIndex(i) as GroupBarItem;
                        if (gitem.IsSelected == true && gitem.Visibility == Visibility.Visible)
                        {
                            FrameworkElement tempcontent = gitem.Content as FrameworkElement;
                            FrameworkElement tempheader = gBar.HeaderContent as FrameworkElement;
                            SetContentVisibility(Visibility.Visible, tempcontent, tempheader);
                        }
                        else if (gitem.IsSelected == true && !gitem.ShowInGroupBar)
                        {
                            FrameworkElement tempcontent = gitem.Content as FrameworkElement;
                            FrameworkElement tempheader = gBar.HeaderContent as FrameworkElement;
                            SetContentVisibility(Visibility.Visible, tempcontent, tempheader);
                        }
                    }
                }
                else
                {
                    gBar.HiddenIndices.Remove(index);
                    SetContentVisibility(newValue, content, header);

                    if (gBar.Items.Contains(owner) && owner == gBar.SelectedTab && owner.ShowInGroupBar)
                    {
                        ////gBar.SelectedTab = owner;
                    }
                }
            }
        }
        
        /// <summary>
        /// Sets new Visibility value to the content of GBI.
        /// </summary>
        /// <param name="newValue">new Visibility value</param>
        /// <param name="content">content of the item</param>
        /// <param name="header">header of the item</param>
        internal static void SetContentVisibility(Visibility newValue, FrameworkElement content, FrameworkElement header)
        {
            if (content != null)
            {
                content.Visibility = newValue;
            }

            if (header != null)
            {
                if (newValue == Visibility.Collapsed)
                {
                    newValue = Visibility.Hidden;
                }

                header.Visibility = newValue;
            }
        }
        
        /// <summary>
        /// Invoked when the value of <see cref="Syncfusion.Windows.Tools.Controls.GroupBarItem.ShowInGroupBarProperty"/> property is changed.
        /// Method manages control's visibility according to <see cref="Syncfusion.Windows.Tools.Controls.GroupBarItem.ShowInGroupBarProperty"/> property.
        /// </summary>
        /// <param name="d">Source of the event</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private static void OnShowInGroupBarChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GroupBarItem owner = d as GroupBarItem;

            if (owner != null && e != null)
            {
                GroupBar gBar = owner.Parent as GroupBar;
                owner.SetVisibility();

                if (gBar != null && gBar.Toolbar != null)
                {
                    gBar.Toolbar.RefreshButtonsMenu();
                    if ((bool)e.NewValue)
                    {
                        int count = owner.LogicalParent.Toolbar.Items.Count - 1;
                        for (int i = count; i >= 0; i--)
                        {
                            var item = owner.LogicalParent.Toolbar.Items[i] as NavigationToolbarItem;
                            if (item.GroupBarItem == owner)
                            {
                                owner.LogicalParent.Toolbar.Items.Remove(item);
                            }
                        }
                    }
                }
            }
        }
        
        /// <summary>
        /// Invoked when the value of <see cref="IsSelected"/> property is changed.
        /// Method sets the value of logical parent's selected item
        /// property and fires Selected or Unselected event.
        /// </summary>
        /// <param name="d">Source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GroupBarItem owner = d as GroupBarItem;

            if (owner != null && owner.LogicalParent != null && e != null)
            {
                bool isSelected = (bool)e.NewValue;
                GroupBar gBar = owner.LogicalParent as GroupBar;

                if (isSelected)
                {
                    GroupView gView = owner.Content as GroupView;

                    if (gView != null  && gBar.Template != null)
                    {
                        ContentPresenter cPresenter = gBar.Template.FindName("HeaderContent", gBar) as ContentPresenter;
                        if (cPresenter != null)
                        {
                            cPresenter.Visibility = gView.Visibility;
                        }
                    }

                    owner.OnSelected();

                    if (gBar != null)
                    {
                        if (gBar.SelectedTab != owner)
                        {
                            gBar.SelectedObject = owner;
                        }
                    }
                }
                else
                {
                    owner.OnUnselected();
                }
            }
        }
        
        /// <summary>
        /// Invoked when the value of <see cref="IsExpanded"/> property is changed.
        /// </summary>
        /// <param name="d">Source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private static void OnIsExpandedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GroupBarItem item = d as GroupBarItem;

            if (item != null && item.IsLoaded && e != null && item.LogicalParent != null)
            {
                bool isExpanded = (bool)e.NewValue;

                if (item.LogicalParent.SelectedTab != item)
                {
                    item.LogicalParent.SelectedTab = item;
                }

                if (isExpanded)
                {
                    ExpandContent(item);
                }
                else
                {
                    CollapseContent(item);
                }
            }
        }
        
        /// <summary>
        /// Invoked when the value of <see cref="IsPressed"/> property is changed.
        /// Method fires <see cref="Press"/> event.
        /// </summary>
        /// <param name="d">Source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private static void OnIsPressedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            
        }

        private static object CoerceIsPressedChanged(DependencyObject d, object value)
        {
            GroupBarItem owner = d as GroupBarItem;

            if (owner != null && owner.IsLoaded && value != null)
            {
                bool isPressed = (bool)value;

                if (isPressed)
                {
                    owner.OnPress();
                }
                
            }
            return value;
        }

        /// <summary>
        /// If keyboard focus is set on one of the controls, but not on the
        /// current control, method return focus to the logical parent.
        /// </summary>
        /// <param name="item">Item to check the focus on.</param>
        private static void CorrectKeyboardFocus(GroupBarItem item)
        {
            IInputElement focusedElement = Keyboard.FocusedElement;

            if (focusedElement != null
                && focusedElement.GetType() == typeof(GroupBarItem)
                && focusedElement != item)
            {
                Keyboard.Focus(item.LogicalParent);
                keyboardFocused = false;
            }
        }
        
        /// <summary>
        /// Refreshes menu's buttons of the toolbar.
        /// </summary>
        /// <param name="groupBarItem">Item to take the parent from.</param>
        private static void RefreshButtonsMenu(GroupBarItem groupBarItem)
        {
            if (groupBarItem != null && groupBarItem.Parent != null)
            {
                GroupBar groupBar = groupBarItem.Parent as GroupBar;

                if (groupBar != null && groupBar.Toolbar != null)
                {
                    groupBar.Toolbar.RefreshButtonsMenu();
                }
            }
        }
        
        /// <summary>
        /// Invoked when the value of <see cref="HeaderImageSource"/> property is changed.
        /// </summary>
        /// <param name="d">Source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private static void OnHeaderImageSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GroupBarItem groupBarItem = d as GroupBarItem;
            RefreshButtonsMenu(groupBarItem);
           
        }

        /// <summary>
        /// Called when [group bar item corner radius changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnGroupBarItemCornerRadiusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GroupBarItem groupBarItem = d as GroupBarItem;
            if (groupBarItem.m_visualHeader != null)
            {
                (groupBarItem.m_visualHeader as Border).CornerRadius = groupBarItem.GroupBarItemCornerRadius;
            }
            
        }
        /// <summary>
        /// Invoked when the value of <see cref="HeaderText"/> property is changed.
        /// </summary>
        /// <param name="d">Source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private static void OnHeaderTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GroupBarItem groupBarItem = d as GroupBarItem;
            BindHeaderText(groupBarItem);
            RefreshButtonsMenu(groupBarItem);
          
        }

        private static void BindHeaderText(GroupBarItem groupBarItem)
        {
            if (groupBarItem.HeaderTemplate == null)
            {
                GroupBarItemHeader header = groupBarItem.Header as GroupBarItemHeader;

                if (groupBarItem.Header != null && groupBarItem.Header is string)
                {
                    if (groupBarItem.HeaderText == null)
                    {
                        string newHeader = (string)groupBarItem.Header;

                        header = new GroupBarItemHeader(newHeader, groupBarItem.HeaderImageSource);
                        groupBarItem.Header = header;
                        groupBarItem.HeaderText = newHeader;
                    }
                    else
                    {
                        header = new GroupBarItemHeader(groupBarItem.HeaderText, groupBarItem.HeaderImageSource);
                        groupBarItem.Header = header;
                    }
                }
                else if (groupBarItem.Header == null || header != null)
                {
                    if (header == null)
                    {
                        header = new GroupBarItemHeader(groupBarItem.HeaderText, groupBarItem.HeaderImageSource);
                    }

                    if (groupBarItem.Header == null)
                    {
                        groupBarItem.Header = header;
                    }

                    groupBarItem.SetBinding(HeaderTextProperty, Binder.Bind(header, "Text"));
                    groupBarItem.SetBinding(HeaderImageSourceProperty, Binder.Bind(header, "ImageSource"));
                  
                }
            }
            else if (groupBarItem.HeaderText != null && groupBarItem.Header == null)
            {
                groupBarItem.Header = new GroupBarItemHeader(groupBarItem.HeaderText, groupBarItem.HeaderImageSource);
            }

        }
        
        /// <summary>
        /// Invoked when the value of <see cref="ContentLength"/> is changed.
        /// </summary>
        /// <param name="d">Source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private static void OnContentLengthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }
        
        /// <summary>
        /// Collapses the content of the GroupBarItem.
        /// </summary>
        /// <param name="item">GroupBarItem object.</param>
        internal static void CollapseContent(GroupBarItem item)
        {
            if (item != null && item.VisualContent != null && item.EnableAnimation)
            {
                item.IsAnimating = true;
                item.UpdateContentLength();
                item.UpdateContentBreadth();
                item.UpdateLayout();
                item.m_visualContent.BeginAnimation(item.m_animatedContentProperty, item.m_unselectedAnimation);
            }
        }
        
        /// <summary>
        /// Expands the content of the GroupBarItem.
        /// </summary>
        /// <param name="item">GroupBarItem object.</param>
        internal static void ExpandContent(GroupBarItem item)
        {
            if (item != null && item.VisualContent != null && item.EnableAnimation)
            {
                item.IsAnimating = true;
                item.VisualContent.Visibility = Visibility.Visible;
                item.InvalidateMeasure();
                item.UpdateContentLength();
                item.UpdateContentBreadth();
                item.UpdateLayout();
                item.VisualContent.BeginAnimation(item.m_animatedContentProperty, item.m_selectedAnimation);
                if (item.LogicalParent != null)
                    item.LogicalParent.UpdateItemsSize();
            }
        }

        /// <summary>
        /// Invoked when <see cref="IsInEditMode"/> is changed.
        /// </summary>
        /// <param name="d">Source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private static void OnIsInEditModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GroupBarItem item = d as GroupBarItem;

            if (item != null)
            {
                GroupBarItemHeader header = item.Header as GroupBarItemHeader;

                if (header != null)
                {
                    header.IsInEditMode = item.IsInEditMode;
                }

                if (item.LogicalParent != null)
                {
                    item.LogicalParent.EnabledKeyBoardNavigation = !item.IsInEditMode;
                }
            }
        }
        
        /// <summary>
        /// Invoked when <see cref="SelectedAnimation"/> is changed.
        /// </summary>
        /// <param name="d">Source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private static void OnSelectedAnimationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GroupBarItem item = d as GroupBarItem;

            if (item != null)
            {
                item.InitializePredefinedAnimation();
            }
        }
        /// <summary>
        /// Called when [group bar item header style changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnHeaderStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GroupBarItem instance = (GroupBarItem)d;
            instance.OnHeaderStyleChanged(e);
        }


        /// <summary>
        /// Raises the <see cref="E:GroupBarHeaderStyleChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnHeaderStyleChanged(DependencyPropertyChangedEventArgs e)
        {
            if (HeaderStyleChanged != null)
            {
                HeaderStyleChanged(this, e);
            }
        }
        /// <summary>
        /// Invoked when <see cref="UnselectedAnimation"/> is changed.
        /// </summary>
        /// <param name="d">Source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private static void OnUnselectedAnimationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GroupBarItem item = d as GroupBarItem;

            if (item != null)
            {
                item.InitializePredefinedAnimation();
            }
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
            m_mouseDownPoint = e.GetPosition(LogicalParent);

            if (IsPointOnHeader(m_mouseDownPoint))
            {
                if (e.ButtonState == MouseButtonState.Pressed)
                {
                    IsPressed = true;
                    if (LogicalParent.VisualMode != VisualMode.StackMode)
                    {
                        CaptureMouse();
                    }

                    if (LogicalParent.ItemsSource != null)
                    {
                        if (e.Source != null)
                        {
                            LogicalParent.SelectedObject = (e.Source as FrameworkElement).DataContext;
                            LogicalParent.SelectedTab = e.Source as GroupBarItem;
                        }
                        else if (e.Source is GroupBarItemHeader)
                        {
                            GroupBarItemHeader header = e.Source as GroupBarItemHeader;
                            LogicalParent.SelectedObject = header.Parent as GroupBarItem;
                        }
                    }
                    else
                    {
                        if (e.Source is GroupBarItem && LogicalParent.VisualMode == VisualMode.StackMode)
                        {
                            LogicalParent.SelectedObject=e.Source as GroupBarItem;
                        }
                        else if (e.Source is GroupBarItemHeader)
                        {
                            GroupBarItemHeader header = e.Source as GroupBarItemHeader;
                            LogicalParent.SelectedObject = header.Parent as GroupBarItem;
                        }
                    }
                    
                }
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
            Point mouseUpPoint = e.GetPosition(LogicalParent);

            if (IsMouseCaptured)
            {
                IsPressed = false;
                ReleaseMouseCapture();

                if (IsMouseOver)
                {
                    if (LogicalParent != null && !IsDragging)
                    {
                        SetExpanded();
                        //LogicalParent.UpdateItemsSize();
                    }

                    FocusHeader();
                    OnClick();
                }

                if (IsDragging)
                {
                    DragProvider.DragCompleted(this, mouseUpPoint);
                    UpdateAdornerMarker();
                    FocusHeader();
                }
            }
        }
        
        /// <summary>
        /// Invoked when an unhandled <see cref="System.Windows.UIElement.MouseRightButtonDown"/>routed
        ///  event reaches an element in its route that is derived from this class. 
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> that contains the event data.</param>
        protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseRightButtonDown(e);
            m_mouseDownPoint = e.GetPosition(LogicalParent);

            if (IsPointOnHeader(m_mouseDownPoint))
            {
                if (e.ButtonState == MouseButtonState.Pressed)
                {
                    IsPressed = true;
                    CaptureMouse();
                }
            }
        }
        
        /// <summary>
        /// Invoked when an unhandled <see cref="System.Windows.UIElement.MouseRightButtonUp"/>routed
        ///  event reaches an element in its route that is derived from this class. 
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> that contains the event data.</param>
        protected override void OnMouseRightButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseRightButtonUp(e);
            Point mouseUpPoint = e.GetPosition(LogicalParent);

            if (IsMouseCaptured)
            {
                IsPressed = false;
                ReleaseMouseCapture();

                if (IsMouseOver)
                {
                    CaptureFocus();
                }
            }

            if (IsDragging)
            {
                DragProvider.DragCompleted(this, mouseUpPoint);
            }
        }

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
            ////else if (e.Property == SkinStorage.VisualStylesListProperty)
            ////{
            ////    Shared.DictionaryList list1 = SkinStorage.GetVisualStylesList(this);
            ////    if (list1 != null)
            ////    {
            ////        Shared.DictionaryList list2 = list1["Default"] as Shared.DictionaryList;
            ////        if (list2 != null)
            ////        {
            ////            if (list2.ContainsKey("GroupBarItem_NormalStateBrush") &&
            ////                m_defaultBackground == null)
            ////            {
            ////                m_defaultBackground = list2["GroupBarItem_NormalStateBrush"] as Brush;
            ////                Background = m_defaultBackground;
            ////            }

            ////            if (list2.ContainsKey("TabText_Brush") &&
            ////                m_defaultForeground == null)
            ////            {
            ////                m_defaultForeground = list2["TabText_Brush"] as Brush;
            ////                Foreground = m_defaultForeground;
            ////            }
            ////        }
            ////    }
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
        /// Sets <see cref="IsExpanded"/> property.
        /// </summary>
        internal void SetExpanded()
        {
            //GroupBarItem item = null;
            if (LogicalParent != null)
            {
                if (LogicalParent.VisualMode == VisualMode.MultipleExpansion)
                {
                    IsExpanded = !IsExpanded;
                }
                else
                {
                    for (int i = 0; i < LogicalParent.Items.Count; i++)
                    {
                        GroupBarItem item = (GroupBarItem)LogicalParent.ItemContainerGenerator.ContainerFromIndex(i);

                        if (item != null && item != this)
                        {
                            item.IsExpanded = false;
                        }
                    }

                    this.IsExpanded = true;
                }
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Controls.Control.MouseDoubleClick"/> event. 
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> that contains the event data.</param>
        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            base.OnMouseDoubleClick(e);
            OnDoubleClick();
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.MouseMove"/> attached event reaches 
        /// an element in its route that is derived from this class. 
        /// Implement this method to add class handling for this event. 
        /// </summary>
        /// <param name="e">The MouseEventArgs that contains the event data.</param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (Mouse.PrimaryDevice.LeftButton == MouseButtonState.Pressed && IsMouseCaptured)
            {
                UpdateDragging();
                UpdateIsPressed();
            }
        }
        
        /// <summary>
        /// Invoked whenever an unhandled <see cref="System.Windows.UIElement.GotFocus"/> event reaches this element in its route. 
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> that contains the event data.</param>
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
            IsPressed = false;
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
                IsInEditMode = false;
            }

            if (LogicalParent != null)
            {
                if (LogicalParent.EnabledKeyBoardNavigation)
                {
                    if (e.Key == Key.Right)
                    {
                        LogicalParent.MoveIn();
                    }
                    else if (e.Key == Key.Left)
                    {
                        LogicalParent.MoveOut();
                    }
                    else if (e.Key == Key.Tab
                        && LogicalParent.SelectedTab != null && !LogicalParent.SelectedTab.IsExpanded)
                    {
                        LogicalParent.MoveIn();
                    }
                }
            }
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
            ////e.Handled = true;
            /* Condition 2 : keyboard focus may be on an element of the content, so we have to check the type
             * of FocusedElement and whether focused element is GroupBarItem by comparing types.
             * If types are equal, it means that header has keyboard focus.
             * Condition 2: item may be scrolled down while in stack mode.
             * Therefore we have to check its visibility*/
            if (!mouseHovers && Keyboard.FocusedElement != null
                && Keyboard.FocusedElement.GetType() == typeof(GroupBarItem)
                && Visibility == Visibility.Visible)
            {
                keyboardFocused = true;

                IsHighlighted = true;

                StartOnEnterAnimation();
            }
        }
        
        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Keyboard.LostKeyboardFocus"/> attached event reaches 
        /// an element in its route that is derived from this class. 
        /// Implement this method to add class handling for this event. 
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.KeyboardFocusChangedEventArgs"/> that contains the event data.</param>
        protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            base.OnLostKeyboardFocus(e);
            /* Checking whether item (i.e. header) had keyboard focus */
            if (keyboardFocused)
            {
                keyboardFocused = false;

                IsHighlighted = false;

                StartOnLeaveAnimation();
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.FrameworkElement.Initialized"/> event. This method is invoked whenever <see cref="P:System.Windows.FrameworkElement.IsInitialized"/> is set to true internally.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.RoutedEventArgs"/> that contains the event data.</param>
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            FrameworkElement contentElement = this.Content as FrameworkElement;

            if (null != contentElement)
            {
                if (!(contentElement is GroupView))
                {
                    //actualHeight = GetActualHeight(contentElement);
                }
            }

            Loaded += new RoutedEventHandler(OnLoaded);
        }
        
        /// <summary>
        /// Builds the current template's visual tree if necessary.
        /// </summary>
        public override void OnApplyTemplate()
        {
            TryClearAdorners();
            base.OnApplyTemplate();
            Initialize(Template);

            if (LogicalParent != null && ShowInGroupBar && !LogicalParent.IsItemFullyVisible(this))
            {
                SetAdornersVisibility(m_visualHeader, Visibility.Collapsed);
            }
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
            constraint = base.MeasureOverride(constraint);

            if (!IsAnimating && !IsExpanded && LogicalParent != null)
            {
                constraint.Height = LogicalParent.ItemHeaderHeight;
            }

            return constraint;
        }
        
        /// <summary>
        /// Occurs when collapsing or expanding animation is completely finished.
        /// </summary>
        /// <param name="sender">Source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> that contains the event data.</param>
        private void Animation_Completed(object sender, EventArgs e)
        {
            IsAnimating = false;
        }
        
        /// <summary>
        /// Occurs when collapsing or expanding animation is completely finished.
        /// </summary>
        /// <param name="sender">Source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> that contains the event data.</param>
        private void Animation_Collapse_Completed(object sender, EventArgs e)
        {
            IsAnimating = false;

            if (LogicalParent != null && !LogicalParent.DraggingItemInProgress && !IsExpanded)
            {
                VisualContent.Visibility = Visibility.Collapsed;
            }
        }
        
        /// <summary>
        /// Updates the visibility of the content.
        /// </summary>
        internal void UpdateContentVisibility()
        {
            if (VisualContent != null)
            {
                VisualContent.Visibility = IsExpanded ? Visibility.Visible : Visibility.Collapsed;
            }
        }
        
        /// <summary>
        /// Invoked when the control is loaded and ready for presentation.
        /// </summary>
        /// <param name="sender">Source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> that contains the event data.</param>
        protected virtual void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (CustomAnimations != null)
            {
                CustomAnimations.InitializeResources(LogicalParent.LogicalParent);
            }

            ApplyTemplate();

            this.GotFocus += new RoutedEventHandler(GroupBarItem_GotFocus);
               
        }

        void GroupBarItem_GotFocus(object sender, RoutedEventArgs e)
        {
            if (this.LogicalParent != null && this.LogicalParent.SelectedTab != this && this.LogicalParent.VisualMode == VisualMode.MultipleExpansion)
            {
                if (this.LogicalParent.SelectedItem != null)
                    this.LogicalParent.SelectedItem = null;
                this.LogicalParent.SelectedTab = this;
            }
        }

        /// <summary>
        /// Handles the PostProcessInput event of the InputManager control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ProcessInputEventArgs"/> instance containing the event data.</param>
        private void InputManager_PostProcessInput(object sender, ProcessInputEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed && IsPressed == false)
            {
                m_mouseDownPoint = Mouse.GetPosition(LogicalParent);

                if (IsPointOnHeader(m_mouseDownPoint))
                {
                    IsPressed = true;
                }
            }
            else if (Mouse.LeftButton == MouseButtonState.Released && IsPressed == true)
            {
                m_mouseDownPoint = Mouse.GetPosition(LogicalParent);

                if (IsPointOnHeader(m_mouseDownPoint))
                {
                    IsPressed = false;
                    SetExpanded();
                }
            }
        }
        #endregion

        #region Disposable members

        public void Dispose()
        {
            if (m_visualHeader != null)
            {
                m_visualHeader.MouseEnter -= new MouseEventHandler(OnMouseEnterVisualHeader);
                m_visualHeader.MouseLeave -= new MouseEventHandler(OnMouseLeaveVisualHeader);
            }
        }

        #endregion

        #region Internal declarations
        /// <summary>
        /// Manages all drag and drop operations of <see cref="Syncfusion.Windows.Tools.Controls.GroupBarItem"/>.
        /// </summary>
        private static class DragProvider
        {
            #region Class members
            /// <summary>
            /// Cursor that is over the <see cref="Syncfusion.Windows.Tools.Controls.GroupBarItem"/> before 
            /// starting the dragging.
            /// </summary>
            private static Cursor previousCursor;
            #endregion

            #region Class static internal methods
            /// <summary>
            /// Should be called when the <see cref="Syncfusion.Windows.Tools.Controls.GroupBarItem"/> is going to be dragged.
            /// </summary>
            /// <param name="caller">The <see cref="Syncfusion.Windows.Tools.Controls.GroupBarItem"/> to be dragged.</param>
            internal static void DragStarted(GroupBarItem caller)
            {
                caller.IsDragging = true;
                previousCursor = caller.Cursor;
                caller.Cursor = Cursors.Hand;
                caller.TryAddAdorner(caller.AdornerDrag);
                caller.VisualHeader.Opacity = 0d;
            }
            
            /// <summary>
            /// Should be called when the <see cref="Syncfusion.Windows.Tools.Controls.GroupBarItem"/> 
            /// has already been dragged and needs to update its adorner.
            /// </summary>
            /// <param name="caller">The <see cref="Syncfusion.Windows.Tools.Controls.GroupBarItem"/> that has been dragged.</param>
            /// <param name="leftOffset">New left offset of the adorner.</param>
            /// <param name="topOffset">New top offset of the adorner.</param>
            internal static void DragMoved(GroupBarItem caller, double leftOffset, double topOffset)
            {
                caller.AdornerDrag.LeftOffset = leftOffset;
                caller.AdornerDrag.TopOffset = topOffset;
            }
            
            /// <summary>
            /// Should be called when the <see cref="Syncfusion.Windows.Tools.Controls.GroupBarItem"/> has been dropped.
            /// </summary>
            /// <param name="caller">The <see cref="Syncfusion.Windows.Tools.Controls.GroupBarItem"/> that has been dropped.</param>
            /// <param name="mouseUpPoint">The point relative to the <see cref="Syncfusion.Windows.Tools.Controls.GroupBarItem"/>
            /// logical parent where the <see cref="Syncfusion.Windows.Tools.Controls.GroupBarItem"/> has been dropped.</param>
            internal static void DragCompleted(GroupBarItem caller, Point mouseUpPoint)
            {
                caller.AdornerDrag.ResetPosition();
                caller.TryRemoveAdorner(caller.AdornerDrag);

                GroupBarItem dropTarget = GetItemFromDropPoint(mouseUpPoint);

                if (dropTarget != null && caller != dropTarget
                    && caller.LogicalParent == dropTarget.LogicalParent)
                {
                    DoDragDrop(caller, dropTarget);
                }

                caller.Cursor = previousCursor;
                caller.IsDragging = false;
                caller.VisualHeader.Opacity = 1d;
            }
            #endregion

            #region Class static private methods
            /// <summary>
            /// Performs drag and drop operation.
            /// </summary>
            /// <param name="dragSource">The <see cref="Syncfusion.Windows.Tools.Controls.GroupBarItem"/> being dragged.</param>
            /// <param name="dropTarget">The <see cref="Syncfusion.Windows.Tools.Controls.GroupBarItem"/> on which dragged 
            /// <see cref="Syncfusion.Windows.Tools.Controls.GroupBarItem"/> is dropped.</param>
            private static void DoDragDrop(GroupBarItem dragSource, GroupBarItem dropTarget)
            {
                GroupBar parent = dragSource.LogicalParent;

                int sourceIndex = parent.Items.IndexOf(dragSource);
                int targetIndex = parent.Items.IndexOf(dropTarget);

                bool isSourceSelected = dragSource.IsSelected;

                if (parent.VisualMode != VisualMode.StackMode)
                {
                    if (!parent.IsItemFullyVisible(dropTarget))
                    {
                        dragSource.SetAdornersVisibility(dragSource.VisualHeader, Visibility.Collapsed);
                    }
                    else
                    {
                        dragSource.SetAdornersVisibility(dragSource.VisualHeader, Visibility.Visible);
                    }

                    if (!parent.IsItemFullyVisible(dragSource))
                    {
                        if (sourceIndex > 0)
                        {
                            GroupBarItem item = parent.Items[sourceIndex - 1] as GroupBarItem;
                            item.SetAdornersVisibility(item.VisualHeader, Visibility.Collapsed);
                        }
                    }
                    else
                    {
                        dropTarget.SetAdornersVisibility(dropTarget.VisualHeader, Visibility.Visible);
                    }
                }

                parent.UpdateHiddenItems(sourceIndex, targetIndex);

                if (parent.ItemsSource == null)
                {
                    parent.Items.Remove(dragSource);
                    targetIndex = parent.Items.IndexOf(dropTarget);

                    if (dropTarget.IsDragOverDown)
                    {
                        parent.Items.Insert(targetIndex + 1, dragSource);
                    }
                    else
                    {
                        parent.Items.Insert(targetIndex, dragSource);
                    }
                }
                else
                {
                   

                    targetIndex = parent.Items.IndexOf(dropTarget.DataContext);

                    if (dropTarget.IsDragOverDown)
                    {
                        ((IList)parent.ItemsSource).Insert(targetIndex + 1, dragSource.DataContext);
                    }
                    else
                    {
                        ((IList)parent.ItemsSource).Insert(targetIndex, dragSource.DataContext);
                    }
                    ((IList)parent.ItemsSource).Remove(dragSource.DataContext);
                    if (dropTarget.IsDragOverDown)
                    {
                        dragSource = (GroupBarItem)parent.ItemContainerGenerator.ContainerFromIndex(targetIndex + 1);
                    }
                    else
                    {
                        dragSource = (GroupBarItem)parent.ItemContainerGenerator.ContainerFromIndex(targetIndex);
                    }
                }

                if (parent.VisualMode == VisualMode.StackMode)
                {
                    parent.Toolbar.RefreshButtonsMenu(dragSource, dropTarget);

                    if (isSourceSelected && dragSource.LogicalParent != null)
                    {
                        if(dragSource != null)
                        dragSource.LogicalParent.SelectedObject = dragSource;
                    }
                }
                else
                {
                    if(dragSource != null)
                    dragSource.UpdateContentSize(isSourceSelected);
                }
            }
            
            /// <summary>
            /// Gets the <see cref="Syncfusion.Windows.Tools.Controls.GroupBarItem"/> from given point relative to
            /// the <see cref="Syncfusion.Windows.Tools.Controls.GroupBarItem"/> logical parent. The method returns
            /// the <see cref="Syncfusion.Windows.Tools.Controls.GroupBarItem"/> reference only if drop point lies on 
            /// the item's header.
            /// </summary>
            /// <param name="point">The point relative to the <see cref="Syncfusion.Windows.Tools.Controls.GroupBarItem"/> logical
            /// parent where the <see cref="Syncfusion.Windows.Tools.Controls.GroupBarItem"/> has been dropped.</param>
            /// <returns>
            /// The <see cref="Syncfusion.Windows.Tools.Controls.GroupBarItem"/> reference only when drop point lies on 
            /// the item's header.
            /// </returns>
            internal static GroupBarItem GetItemFromDropPoint(Point point)
            {
                UIElement element = Mouse.DirectlyOver as UIElement;

                while (element != null)
                {
                    element = VisualTreeHelper.GetParent(element) as UIElement;
                    GroupBarItem item = element as GroupBarItem;

                    if (item != null && item.IsPointOnHeader(point))
                    {
                        return item;
                    }
                }

                return null;
            }
            #endregion
        }

        /// <summary>
        /// Used to create the adorner from the header of the <see cref="Syncfusion.Windows.Tools.Controls.GroupBarItem"/>.
        /// </summary>
#if SyncfusionFramework4_0
        [System.ComponentModel.DesignTimeVisible(false)]
#endif
        private class HeaderAdorner : Adorner
        {
            #region class members
            /// <summary>
            /// Rectangle to be painted with the visual of the item's header.
            /// </summary>
            protected Rectangle m_child;
            
            /// <summary>
            /// Logical parent as <see cref="Syncfusion.Windows.Tools.Controls.GroupBarItem"/>.
            /// </summary>
            protected GroupBarItem m_logicalParent;
            #endregion

            #region Class public properties
            /// <summary>
            /// Gets the logical parent.
            /// </summary>
            /// <value>The logical parent.</value>
            public GroupBarItem LogicalParent
            {
                get
                {
                    return m_logicalParent;
                }
            }
            #endregion

            #region class Initialize/Finalize methods
            /// <summary>
            /// Initializes a new instance of the <see cref="HeaderAdorner"/> class.
            /// </summary>
            /// <param name="adornedElement">The adorned element.</param>
            public HeaderAdorner(FrameworkElement adornedElement)
                : base(adornedElement)
            {
                IsHitTestVisible = false;
                m_child = new Rectangle();
                m_child.SetBinding(Rectangle.WidthProperty, Binder.Bind(adornedElement, "ActualWidth"));
                m_child.SetBinding(Rectangle.HeightProperty, Binder.Bind(adornedElement, "ActualHeight"));
            }

            #endregion

            #region class overrides
            /// <summary>
            /// Called to remeasure the control. 
            /// </summary>
            /// <param name="constraint">Measurement constraints, 
            /// the control cannot return a size larger than the constraint.</param>
            /// <returns>The size of the control.</returns>
            protected override Size MeasureOverride(Size constraint)
            {
                m_child.Measure(constraint);
                return m_child.DesiredSize;
            }
            
            /// <summary>
            /// Called to arrange and size the content of the control.
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
            /// <returns>The requested child element. 
            /// This should not return a null reference (Nothing in Visual Basic).</returns>
            protected override Visual GetVisualChild(int index)
            {
                return m_child;
            }

            /// <summary>
            /// Gets the number of visual child elements within this element.
            /// </summary>
            /// <value></value>
            /// <returns>The number of visual child elements for this element.</returns>
            protected override int VisualChildrenCount
            {
                get
                {
                    return 1;
                }
            }
            #endregion
        }

        /// <summary>
        /// Used for creating the visual adorner for the <see cref="Syncfusion.Windows.Tools.Controls.GroupBarItem"/>
        /// when it is dragged.
        /// </summary>
#if SyncfusionFramework4_0
        [System.ComponentModel.DesignTimeVisible(false)]
#endif
        private class DragAdorner : HeaderAdorner
        {
            #region Class members
            /// <summary>
            /// Left offset of the adorner relatively to the point where
            /// the <see cref="Syncfusion.Windows.Tools.Controls.GroupBarItem"/> header was pressed.
            /// </summary>
            private double m_topOffset;
            
            /// <summary>
            /// Top offset of the adorner relatively to the point where
            /// <see cref="Syncfusion.Windows.Tools.Controls.GroupBarItem"/> header was pressed.
            /// </summary>
            private double m_leftOffset;
            #endregion

            #region Class public properties
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
            #endregion

            #region Class Initialize/Finalize methods
            /// <summary>
            /// Initializes a new instance of the <see cref="DragAdorner"/> class.
            /// </summary>
            /// <param name="adornedElement">The adorned element.</param>
            public DragAdorner(FrameworkElement adornedElement)
                : base(adornedElement)
            {
                m_logicalParent = adornedElement.TemplatedParent as GroupBarItem;

                RenderTargetBitmap renderer = new RenderTargetBitmap((int)m_child.Width, (int)m_child.Height, 0d, 0d, PixelFormats.Default);
                renderer.Render(adornedElement);
                m_child.Fill = new ImageBrush(renderer);
                m_child.Opacity = 0.3d;
            }

            #endregion

            #region class helper methods

            #region internal methods
            /// <summary>
            /// Resets the position of the adorner.
            /// </summary>
            internal void ResetPosition()
            {
                m_leftOffset = 0;
                m_topOffset = 0;
            }
            #endregion

            #region private methods
            /// <summary>
            /// Updates the position of the adorner.
            /// </summary>
            private void UpdatePosition()
            {
                AdornerLayer adornerLayer = GroupBar.GetAdornerLayer(LogicalParent.VisualHeader);

                if (adornerLayer != null)
                {
                    adornerLayer.Update(AdornedElement);
                }
            }
            #endregion

            #endregion

            #region Class overrides
            /// <summary>
            /// Returns transform for the adorner, based on the transform 
            /// that is currently applied to the adorned element. 
            /// </summary>
            /// <param name="transform">The transform that is currently applied 
            /// to the adorned element.</param>
            /// <returns>A transform to be applied to the adorner.</returns>
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
        /// drag marker of the <see cref="Syncfusion.Windows.Tools.Controls.GroupBarItem"/> when it is dragged.
        /// </summary>
#if SyncfusionFramework4_0
        [System.ComponentModel.DesignTimeVisible(false)]
#endif
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
            protected GroupBarItem m_logicalParent;
            
            /// <summary>
            /// Left offset of the adorner relatively to the point where
            /// the <see cref="Syncfusion.Windows.Tools.Controls.GroupBarItem"/> header was pressed.
            /// </summary>
            private double m_topOffset;
            #endregion

            #region Properties
            /// <summary>
            /// Gets the logical parent.
            /// </summary>
            /// <value>The logical parent.</value>
            public GroupBarItem LogicalParent
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
            #endregion

            #region Initialize/Finalize methods
            /// <summary>
            /// Initializes a new instance of the <see cref="DragMarkerAdorner"/> class.
            /// </summary>
            /// <param name="adornedElement">The adorned element.</param>
            public DragMarkerAdorner(FrameworkElement adornedElement)
                : base(adornedElement)
            {
                m_logicalParent = adornedElement as GroupBarItem;
                m_child = new Rectangle();

                if (LogicalParent != null)
                {
                    m_child.SetBinding(Rectangle.WidthProperty, Binder.Bind(LogicalParent, "ActualWidth"));

                    if (LogicalParent.LogicalParent != null)
                    {
                        m_child.SetBinding(Rectangle.FillProperty, Binder.Bind(LogicalParent.LogicalParent, "DragMarkerBrush"));
                    }
                    else
                    {
                        m_child.Fill = Brushes.Red;
                    }

                    m_child.Height = C_defaultMarkerHeight;
                }
            }

            #endregion

            #region Implementation
            /// <summary>
            /// Resets the position of the adorner.
            /// </summary>
            internal void ResetPosition()
            {
                m_topOffset = 0;
            }
           
            /// <summary>
            /// Updates the position of the adorner.
            /// </summary>
            private void UpdatePosition()
            {
                AdornerLayer adornerLayer = GroupBar.GetAdornerLayer(LogicalParent);

                if (adornerLayer != null)
                {
                    adornerLayer.Update(AdornedElement);
                }
            }
            
            /// <summary>
            /// Returns transform for the adorner, based on the transform 
            /// that is currently applied to the adorned element. 
            /// </summary>
            /// <param name="transform">The transform that is currently applied 
            /// to the adorned element.</param>
            /// <returns>The transform to be applied to the adorner.</returns>
            public override GeneralTransform GetDesiredTransform(GeneralTransform transform)
            {
                GeneralTransformGroup result = new GeneralTransformGroup();
                result.Children.Add(base.GetDesiredTransform(transform));

                if (LogicalParent.LogicalParent.Orientation == Orientation.Vertical)
                {
                    result.Children.Add(new TranslateTransform(0, TopOffset));
                }
                else
                {
                    result.Children.Add(new TranslateTransform(TopOffset, 0));
                }

                return result;
            }
            
            /// <summary>
            /// Called to remeasure the control. 
            /// </summary>
            /// <param name="constraint">Measurement constraints, 
            /// the control cannot return a size larger than the constraint.</param>
            /// <returns>The size of the control.</returns>
            protected override Size MeasureOverride(Size constraint)
            {
                m_child.Measure(constraint);

                return m_child.DesiredSize;
            }
            
            /// <summary>
            /// Called to arrange and size the content of the control.
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
            /// <returns>The requested child element. 
            /// This should not return a null reference (Nothing in Visual Basic).</returns>
            protected override Visual GetVisualChild(int index)
            {
                return m_child;
            }

            /// <summary>
            /// Gets the number of visual child elements within this element.
            /// </summary>
            /// <value></value>
            /// <returns>The number of visual child elements for this element.</returns>
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
