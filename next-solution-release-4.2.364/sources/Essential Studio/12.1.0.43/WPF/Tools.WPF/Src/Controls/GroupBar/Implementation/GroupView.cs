// <copyright file="GroupView.cs" company="Syncfusion">
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
using System.ComponentModel;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Input;
using System.Windows.Controls.Primitives;
using System.Collections.Specialized;
using Syncfusion.Licensing;
using Syncfusion.Windows.Shared;
#endregion

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents the FxGroupView UI element. 
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif

    [SkinType(SkinVisualStyle = Skin.Office2007Blue,
Type = typeof(GroupView), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/GroupBar/Themes/Office2007BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Black,
    Type = typeof(GroupView), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/GroupBar/Themes/Office2007BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Silver,
    Type = typeof(GroupView), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/GroupBar/Themes/Office2007SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Blue,
Type = typeof(GroupView), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/GroupBar/Themes/Office2010BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Black,
    Type = typeof(GroupView), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/GroupBar/Themes/Office2010BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Silver,
    Type = typeof(GroupView), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/GroupBar/Themes/Office2010SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2003,
     Type = typeof(GroupView), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/GroupBar/Themes/Office2003Style.xaml")]
    [SkinType(SkinVisualStyle = Skin.ShinyBlue,
     Type = typeof(GroupView), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/GroupBar/Themes/ShinyBlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.ShinyRed,
     Type = typeof(GroupView), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/GroupBar/Themes/ShinyRedStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.SyncOrange,
     Type = typeof(GroupView), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/GroupBar/Themes/SyncOrangeStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Blend,
    Type = typeof(GroupView), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/GroupBar/Themes/BlendStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Default,
    Type = typeof(GroupView), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/GroupBar/Themes/DefaultStyle.xaml")]  
    [SkinType(SkinVisualStyle=Skin.VS2010,
      Type=typeof(GroupView),XamlResource= "/Syncfusion.Tools.Wpf;component/Controls/GroupBar/Themes/VS2010Style.xaml")]
    [SkinType(SkinVisualStyle = Skin.Metro,
     Type = typeof(GroupView), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/GroupBar/Themes/MetroStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Transparent ,
    Type = typeof(GroupView), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/GroupBar/Themes/TransparentStyle.xaml")]
   
    public class GroupView : ItemsControl
    {
        #region Constants
        /// <summary>
        /// Default orientation.
        /// </summary>
        private const Orientation DEF_DEFAULT_ORIENTATION = Orientation.Vertical;

        /// <summary>
        /// Gab for items when the <see cref="Syncfusion.Windows.Tools.Controls.GroupBar"/> has horizontal orientation.
        /// </summary>
        private const double C_dHorizontalOrientationGap = 10;

        /// <summary>
        /// Null index for the collection.
        /// </summary>
        private const int C_nullIndex = -1;

        /// <summary>
        /// Default text for new item.
        /// </summary>
        private const string C_newItemText = "NewItem";

        /// <summary>
        /// Name of the main host from the template.
        /// </summary>
        private const string C_nameMainHost = "MainHost";

        /// <summary>
        /// Message for the main host if it is not found.
        /// </summary>
        private const string C_errorMainHost = "MainHost not found";

        /// <summary>
        /// Contains default visual style key name.
        /// </summary>
        private const string C_defaultVisStyleName = "Default";
        #endregion

        #region Private members
        /// <summary>
        /// The main host in the control's visual tree.
        /// </summary>
        private FrameworkElement m_mainHost = null;

       
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
        ////internal static readonly DependencyProperty DefaultBackgroundProperty = DependencyProperty.Register("DefaultBackground", typeof(Brush), typeof(GroupView), new FrameworkPropertyMetadata(Brushes.Transparent));

        /////// <summary>
        /////// Identifies the <see cref="DefaultForeground"/> dependency property.
        /////// </summary>
        ////internal static readonly DependencyProperty DefaultForegroundProperty = DependencyProperty.Register("DefaultForeground", typeof(Brush), typeof(GroupView), new FrameworkPropertyMetadata(Brushes.Transparent));

        /// <summary>
        /// Identifies <see cref="CustomAnimations"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CustomAnimationsProperty = DependencyProperty.Register("CustomAnimations", typeof(CustomAnimationsCollection), typeof(GroupView), new UIPropertyMetadata(null));

        /// <summary>
        /// Identifies <see cref="Orientation"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof(Orientation), typeof(GroupView), new FrameworkPropertyMetadata(DEF_DEFAULT_ORIENTATION, FrameworkPropertyMetadataOptions.AffectsRender, OnOrientationChanged));

        /// <summary>
        /// Identifies TextHorizontalAlignment dependency property.
        /// </summary>
        public static readonly DependencyProperty TextAlignmentProperty = DependencyProperty.Register("TextAlignment", typeof(HorizontalAlignment), typeof(GroupView), new FrameworkPropertyMetadata(HorizontalAlignment.Left, OnTextAlignmentChanged, OnTextAlignmentCoerce));

        /// <summary>
        /// Identifies <see cref="ShowToolTip"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ShowToolTipProperty = DependencyProperty.RegisterAttached("ShowToolTip", typeof(bool), typeof(GroupView), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// Identifies <see cref="IsShowText"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsShowTextProperty = DependencyProperty.Register("IsShowText", typeof(bool), typeof(GroupView), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsRender, OnIsShowTextChanged, OnIsShowTextCoerce));

        /// <summary>
        /// Identifies <see cref="GroupViewItemCursorType"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty GroupViewItemCursorTypeProperty = DependencyProperty.Register("GroupViewItemCursorType", typeof(ItemCursorType), typeof(GroupView), new UIPropertyMetadata(ItemCursorType.Default));

        /// <summary>
        /// Identifies <see cref="InternalContentLength"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty InternalContentLengthProperty = DependencyProperty.Register("InternalContentLength", typeof(double), typeof(GroupView), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsRender, OnInternalContentLengthChanged, OnInternalContentLengthCoerce));

        /// <summary>
        /// Identifies <see cref="IsListViewMode"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsListViewModeProperty =
            DependencyProperty.Register("IsListViewMode", typeof(bool), typeof(GroupView), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsRender, OnIsListViewModeChanged, OnIsListViewModeCoerce));

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
        /// Gets or sets the orientation.
        /// </summary>
        /// <value>The orientation.</value>
        public Orientation Orientation
        {
            get
            {
                return (Orientation)GetValue(OrientationProperty);
            }

            set
            {
                SetValue(OrientationProperty, value);
            }
        }

        /// <summary>
        /// Gets the logical parent.
        /// </summary>
        /// <value>The logical parent.</value>
        public GroupBarItem LogicalParent
        {
            get
            {
                return base.Parent as GroupBarItem;
            }
        }

        /// <summary>
        /// Gets or sets the text alignment.
        /// </summary>
        /// <value>The text alignment.</value>
        public HorizontalAlignment TextAlignment
        {
            get
            {
                return (HorizontalAlignment)GetValue(TextAlignmentProperty);
            }

            set
            {
                SetValue(TextAlignmentProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show tool tip].
        /// </summary>
        /// <value><c>true</c> if [show tool tip]; otherwise, <c>false</c>.</value>
        public bool ShowToolTip
        {
            get
            {
                return (bool)GetValue(ShowToolTipProperty);
            }

            set
            {
                SetValue(ShowToolTipProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is show text.
        /// </summary>
        /// <value>
        /// true if this instance is show text; otherwise, false.
        /// </value>
        public bool IsShowText
        {
            get
            {
                return (bool)GetValue(IsShowTextProperty);
            }

            set
            {
                SetValue(IsShowTextProperty, value);
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
        /// Gets or sets the type of the group view item cursor.
        /// </summary>
        /// <value>The type of the group view item cursor.</value>
        public ItemCursorType GroupViewItemCursorType
        {
            get
            {
                return (ItemCursorType)GetValue(GroupViewItemCursorTypeProperty);
            }

            set
            {
                SetValue(GroupViewItemCursorTypeProperty, value);
            }
        }

        /// <summary>
        /// Gets the group view item cursor.
        /// </summary>
        /// <value>The group view item cursor.</value>
        public Cursor GroupViewItemCursor
        {
            get
            {
                ItemCursorType cursorType = (ItemCursorType)GetValue(GroupViewItemCursorTypeProperty);
                Cursor cursor = Cursors.Arrow;

                if (cursorType == ItemCursorType.Hand)
                {
                    cursor = Cursors.Hand;
                }

                return cursor;
            }
        }

        /// <summary>
        /// Gets or sets the length of the internal content.
        /// </summary>
        /// <value>The length of the internal content.</value>
        public double InternalContentLength
        {
            get
            {
                return (double)GetValue(InternalContentLengthProperty);
            }

            set
            {
                SetValue(InternalContentLengthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is list view mode.
        /// </summary>
        /// <value>
        /// true if this instance is list view mode; otherwise, false.
        /// </value>
        public bool IsListViewMode
        {
            get
            {
                return (bool)GetValue(IsListViewModeProperty);
            }

            set
            {
                SetValue(IsListViewModeProperty, value);
            }
        }
        #endregion

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="GroupView"/> class.
        /// </summary>
        public GroupView()
            : base()
        {
            if (EnvironmentTestTools.IsSecurityGranted)
            {
                EnvironmentTestTools.StartValidateLicense(typeof(GroupView));
            }
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                if (Application.Current != null && !BindingUtils.GetEnableBindingErrors(Application.Current.MainWindow))
                    System.Diagnostics.PresentationTraceSources.DataBindingSource.Switch.Level = System.Diagnostics.SourceLevels.Critical;
            }

        }

        /// <summary>
        /// Initializes static members of the <see cref="GroupView"/> class.
        /// </summary>
        static GroupView()
        {
           // EnvironmentTest.ValidateLicense(typeof(GroupView));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GroupView), new FrameworkPropertyMetadata(typeof(GroupView)));

            HorizontalContentAlignmentProperty.OverrideMetadata(typeof(GroupView), new FrameworkPropertyMetadata(OnHorizontalContentAlignmentChanged, OnHorizontalContentAlignmentCoerce));

            VisibilityProperty.OverrideMetadata(typeof(GroupView), new UIPropertyMetadata(Visibility.Visible, OnVisibilityChanged));
        }
        #endregion

        #region Events
        /// <summary>
        /// Occurs when a new <see cref="GroupViewItem"/> is added into the <see cref="GroupView"/>.
        /// </summary>
        public event NotifyCollectionChangedEventHandler GroupViewItemAdded;

        /// <summary>
        /// Occurs when an existing <see cref="GroupViewItem"/> is removed from the <see cref="GroupView"/>.
        /// </summary>
        public event NotifyCollectionChangedEventHandler GroupViewItemRemoved;
        #endregion

        #region Implementation
        /// <summary>
        /// Called when some dependencyProperty is changed.
        /// </summary>
        /// <param name="e">Provides data for various property changed events. Typically these events report effective value changes in the value of a read-only dependency property.</param>
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            ////if (e.Property == SkinStorage.VisualStyleProperty && (string)e.NewValue == C_defaultVisStyleName)
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
        /// Invoked when visibility of the control is changed.
        /// </summary>
        /// <param name="d">Source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private static void OnVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {           
            GroupView owner = d as GroupView;

            if (owner != null && e != null)
            {
                Visibility visibility = (Visibility)e.NewValue;
                owner.Visibility = visibility;

                GroupBarItem gBarItem = owner.Parent as GroupBarItem;
                GroupBar gBar = gBarItem.Parent as GroupBar;
                

                if (gBar != null)
                {
                    int index = gBar.Items.IndexOf(gBarItem);
                    int itemsCnt = gBar.Items.Count;
                    int visibilityCount = gBar.VisibleItemsCount;
                   
                    if (itemsCnt > index && index < itemsCnt - 1 && visibilityCount>0)
                    {
                        GroupBarItem groupBarItem1 = owner.CheckCorrectItem(gBarItem, gBar);
                        if (groupBarItem1 != null && groupBarItem1.ShowInGroupBar)
                        gBar.SelectedTab = owner.CheckCorrectItem(gBarItem, gBar);
                    }
                    else if (index == itemsCnt - 1 && visibilityCount > 0)
                    {
                        GroupBarItem groupBarItem1 = owner.CheckCorrectItem(gBarItem, gBar);
                        if (groupBarItem1 != null && groupBarItem1.ShowInGroupBar)
                            gBar.SelectedTab = owner.CheckCorrectItem(gBarItem, gBar);
                    }
                }
            }
        }

        /// <summary>
        /// Checks the correct item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="owner">The owner.</param>
        /// <returns></returns>
        private GroupBarItem CheckCorrectItem(GroupBarItem item,GroupBar owner)
        {
            if (item!=null && item.Visibility == Visibility.Collapsed)
            {
                int index = owner.Items.IndexOf(item);
                int itemsCnt = owner.Items.Count;
                int visibilityCount = owner.VisibleItemsCount;
                for (int i = index; i < itemsCnt; i++)
                {
                    GroupBarItem item1 = owner.Items[i] as GroupBarItem;
                    if (item1.Visibility == Visibility.Visible)
                    {
                        return item1;
                    }
                }
                for (int j = index - 1; j >= 0; j--)
                {
                    GroupBarItem item2 = owner.Items[j] as GroupBarItem;
                    if (item2.Visibility == Visibility.Visible)
                    {
                        return item2;
                    }
                }                
            }
            return null;
        }

        /// <summary>
        /// Initializes the control in the given template.
        /// </summary>
        /// <param name="inTemplate">Template to initialize control in.</param>
        private void Initialize(FrameworkTemplate inTemplate)
        {
            if (CustomAnimations != null)
            {
                CustomAnimations.Initialize(this, Template);
            }
        }

        /// <summary>
        /// Invoked whenever an unhandled <see cref="System.Windows.UIElement.GotFocus"/> event reaches this element in its route. 
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> that contains the event data.</param>
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);

            if (HasItems)
            {
                GroupViewItem viewItem = Items[0] as GroupViewItem;

                if (viewItem != null)
                {
                    viewItem.Focus();
                }
            }
        }

        /// <summary>
        /// Indicates whether the specified item is (or is eligible to be) its own container.
        /// </summary>
        /// <param name="item">The item to check.</param>
        /// <returns>Returns true if the item is (or is eligible to be) its own container; 
        /// otherwise, false.</returns>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is GroupViewItem;
        }

        /// <summary>
        /// Creates or identifies the element that is used to display the given item. 
        /// </summary>
        /// <returns>The element that is used to display the given item.</returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            GroupViewItem item = new GroupViewItem();

            if (LogicalParent != null && LogicalParent.LogicalParent != null)
            {
                //item.SetBinding(GroupViewItem.ImageSourceProperty, Binder.Bind(LogicalParent.LogicalParent.DefaultImage, "Source"));
            }

            return item;
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
            ////        if (list2.ContainsKey("GroupView_Background") &&
            ////            m_defaultBackground == null)
            ////        {
            ////            m_defaultBackground = list2["GroupView_Background"] as Brush;
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
        /// Invoked when the control is loaded and ready for presentation.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> that contains the event data.</param>
        protected virtual void OnLoaded(object sender, RoutedEventArgs e)
        {
            ApplyTemplate();

            UpdaterDefaultImage();

            HorizontalAlignment alignment = this.HorizontalContentAlignment;
            this.HorizontalContentAlignment = HorizontalAlignment.Stretch;
            UpdateInternalContentLength();
            this.HorizontalContentAlignment = alignment;
        }

        /// <summary>
        /// Builds the current template's visual tree if necessary.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            Initialize(Template);
            
            m_mainHost = this.Template.FindName(C_nameMainHost, this) as FrameworkElement;
            bool isDesignerMode = (bool)DesignerProperties.IsInDesignModeProperty.GetMetadata(typeof(DependencyObject)).DefaultValue;

            if (m_mainHost == null && !isDesignerMode && LogicalParent != null
                && LogicalParent.LogicalParent != null && LogicalParent.LogicalParent.VisualMode != VisualMode.StackMode)
            {
                throw new ApplicationException(C_errorMainHost);
            }

            if (CustomAnimations != null)
            {
                CustomAnimations.InitializeResources(LogicalParent.LogicalParent.LogicalParent);
            }
        }

        /// <summary>
        /// Gets a value indicating whether tooltip is shown for the items.
        /// </summary>
        /// <param name="obj">GroupView object.</param>
        /// <returns>True if toolTip is shown for the items; otherwise, false.</returns>
        public static bool GetShowToolTip(DependencyObject obj)
        {
            return (bool)obj.GetValue(ShowToolTipProperty);
        }

        /// <summary>
        /// Sets the show tool tip.
        /// </summary>
        /// <param name="obj">The obj DependencyObject.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        public static void SetShowToolTip(DependencyObject obj, bool value)
        {
            obj.SetValue(ShowToolTipProperty, value);
        }

        /// <summary>
        /// Adds the item.
        /// </summary>
        /// <param name="caption">The caption.</param>
        /// <param name="image">The image.</param>
        /// <param name="index">The index.</param>
        /// <param name="tagObject">The tag object.</param>
        /// <param name="tooltip">The tooltip.</param>
        /// <returns>Group ViewItem</returns>
        public GroupViewItem AddItem(string caption, Image image, int index, object tagObject, string tooltip)
        {
            IList list = (ItemsSource == null) ?
                Items as IList : ItemsSource as IList;

            GroupViewItem item = new GroupViewItem();
            item.Text = caption;
            item.Tag = tagObject;
            item.ToolTip = tooltip;

            if (image != null)
            {
                item.ImageSource = image.Source;
            }

            if (list != null)
            {
                if (index > C_nullIndex)
                {
                    list.Insert(index, item);
                }
                else
                {
                    index = list.Add(item);
                }

                item.UpdateLayout();
            }

            return item;
        }

        /// <summary>
        /// Adds the item to the collection of items.
        /// </summary>
        /// <returns>New <see cref="Syncfusion.Windows.Tools.Controls.GroupViewItem"/> object.</returns>
        public GroupViewItem AddItem()
        {
            return AddItem(C_newItemText, null, C_nullIndex, null, String.Empty);
        }

        /// <summary>
        /// Adds the item to the collection of items.
        /// </summary>
        /// <param name="index">Index in the collection where the specified item is located.</param>
        /// <returns>New <see cref="Syncfusion.Windows.Tools.Controls.GroupViewItem"/> object</returns>
        public GroupViewItem AddItem(int index)
        {
            return AddItem(C_newItemText, null, index, null, String.Empty);
        }

        /// <summary>
        /// Adds the item to the collection of items.
        /// </summary>
        /// <param name="caption">Displayed text of the item.</param>
        /// <returns>New <see cref="Syncfusion.Windows.Tools.Controls.GroupViewItem"/> object.</returns>
        public GroupViewItem AddItem(string caption)
        {
            return AddItem(caption, null, C_nullIndex, null, String.Empty);
        }

        /// <summary>
        ///  Adds the item to the collection of items.
        /// </summary>
        /// <param name="caption">Displayed text of the item.</param>
        /// <param name="image">The image of the item..</param>
        /// <returns>New <see cref="Syncfusion.Windows.Tools.Controls.GroupViewItem"/> object.</returns>
        public GroupViewItem AddItem(string caption, Image image)
        {
            return AddItem(caption, image, C_nullIndex, null, String.Empty);
        }

        /// <summary>
        /// Adds the item to the collection of items.
        /// </summary>
        /// <param name="caption">Displayed text of the item.</param>
        /// <param name="image">The image of the item.</param>
        /// <param name="tagObject">Arbitrary object value.</param>
        /// <returns>New <see cref="Syncfusion.Windows.Tools.Controls.GroupViewItem"/> object.</returns>
        public GroupViewItem AddItem(string caption, Image image, object tagObject)
        {
            return AddItem(caption, image, C_nullIndex, tagObject, String.Empty);
        }

        /// <summary>
        /// Rename the item from the collection of items.
        /// </summary>
        /// <param name="index">Index in the collection where the specified item is located.</param>
        /// <param name="caption">The caption.</param>
        /// <returns>Renamed <see cref="Syncfusion.Windows.Tools.Controls.GroupViewItem"/> object.</returns>
        public GroupViewItem RenameItem(int index, string caption)
        {
            IList list = (ItemsSource == null) ?
                Items as IList : ItemsSource as IList;
            GroupViewItem item = null;

            if (list != null && index > -1 && index < Items.Count)
            {
                item = list[index] as GroupViewItem;

                if (item != null)
                {
                    item.Text = caption;
                }
            }

            return item;
        }

        /// <summary>
        /// Invoked when a new <see cref="GroupViewItem"/> is added into the <see cref="GroupView"/>.
        /// </summary>
        /// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> that contains the event data.</param>
        protected virtual void OnGroupViewItemAdded(NotifyCollectionChangedEventArgs e)
        {
            if (this.GroupViewItemAdded != null)
            {
                this.GroupViewItemAdded(this, e);
            }
        }

        /// <summary>
        /// Invoked when an existing <see cref="GroupViewItem"/> is removed from the <see cref="GroupView"/>.
        /// </summary>
        /// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> that contains the event data.</param>
        protected virtual void OnGroupViewItemRemoved(NotifyCollectionChangedEventArgs e)
        {
            if (this.GroupViewItemRemoved != null)
            {
                this.GroupViewItemRemoved(this, e);
            }
        }

        /// <summary>
        /// Invoked when Items property of the control is changed.
        /// </summary>
        /// <param name="e">The instance containing the event data.</param>
        protected override void OnItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (LogicalParent != null && LogicalParent.LogicalParent != null)
            {
                LogicalParent.LogicalParent.UpdateItemsSize();
                LogicalParent.LogicalParent.UpdateLayout();
            }
            else
            {
              GroupBar gbar=  VisualUtils.FindAncestor(this, typeof(GroupBar)) as GroupBar;
              if (gbar != null)
              {
                  gbar.UpdateItemsSize();
                  gbar.UpdateLayout();
              }
            }

            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                IList list = new ArrayList();
                for (int i = 0; i < e.NewItems.Count; i++)
                {
                    GroupViewItem item = ItemContainerGenerator.ContainerFromIndex(i) as GroupViewItem;
                    if (item != null)
                    {
                        list.Add(item);
                    }
                }

                if (list.Count > 0)
                {
                    NotifyCollectionChangedEventArgs args =
                        new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, list, e.NewStartingIndex);
                    this.OnGroupViewItemAdded(args);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                if (e.OldItems.Count > 0)
                {
                    this.OnGroupViewItemRemoved(e);
                }
            }

            base.OnItemsChanged(e);
        }

        /// <summary>
        /// Updates the default image.
        /// </summary>
        private void UpdaterDefaultImage()
        {
            if (this.ItemsSource != null && this.Items.Count > 0)
            {
                for (int i = 0; i < Items.Count; i++)
                {
                    GroupViewItem item = ItemContainerGenerator.ContainerFromItem(Items[i]) as GroupViewItem;

                    if (item != null && item.ParentGroupBar != null && item.ImageSource == null)
                    {
                        item.SetBinding(GroupViewItem.ImageSourceProperty, Binder.Bind(item.ParentGroupBar.DefaultImage, "Source"));
                    }
                }
            }
        }

        /// <summary>
        /// Updates the internal content width for all items.
        /// </summary>
        internal void UpdateInternalContentLength()
        {
            if (IsLoaded)
            {
                InternalContentLength = GetMaxInternalContentLength();
            }
        }

        /// <summary>
        /// Gets maximum internal content length for the item from Items collection. 
        /// </summary>
        /// <returns>Maximum internal content length for the item from Items collection.</returns>
        private double GetMaxInternalContentLength()
        {
            double width = 0;

            if (IsLoaded && IsInitialized && Items.Count > 0)
            {
                double itemWidth = 0;

                for (int i = 0, count = Items.Count; i < count; i++)
                {
                    GroupViewItem groupViewItem = (ItemsSource == null) ?
                        Items[i] as GroupViewItem :
                        ItemContainerGenerator.ContainerFromItem(Items[i]) as GroupViewItem;

                    if (groupViewItem != null)
                    {
                        itemWidth = groupViewItem.GetInternalContentLength();

                        if (itemWidth > width)
                        {
                            width = itemWidth;
                        }
                    }
                }
            }

            return width;
        }

        /// <summary>
        /// Invoked when HorizontalContentAlignment property is changed.
        /// </summary>
        /// <param name="d">GroupView object.</param>
        /// <param name="e">The instance containing the event data.</param>
        private static void OnHorizontalContentAlignmentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GroupView view = d as GroupView;

            if (view != null)
            {
                view.UpdateInternalContentLength();
                view.UpdateLayout();
            }
        }

        /// <summary>
        /// Coerces the value of HorizontalContentAlignment property.
        /// </summary>
        /// <param name="d">Object to which the property belongs.</param>
        /// <param name="value">Value that should be checked.</param>
        /// <returns>
        /// Checked value.
        /// </returns>
        private static object OnHorizontalContentAlignmentCoerce(DependencyObject d, object value)
        {
            GroupView view = d as GroupView;

            if (view != null)
            {
                if (view.LogicalParent != null && view.LogicalParent.LogicalParent != null)
                {
                    if (view.LogicalParent.LogicalParent.Orientation == Orientation.Horizontal
                        && view.LogicalParent.LogicalParent.VisualMode == VisualMode.MultipleExpansion)
                    {
                        value = HorizontalAlignment.Stretch;
                    }
                }
            }

            return value;
        }

        /// <summary>
        /// Called to remeasure a control.
        /// </summary>
        /// <param name="constraint">The maximum size that the method can return.</param>
        /// <returns>
        /// The size of the control, up to the maximum specified by constraint.
        /// </returns>
        protected override Size MeasureOverride(Size constraint)
        {
            GroupBar groupBar = null;

            if (LogicalParent != null && LogicalParent.LogicalParent != null)
            {
                groupBar = LogicalParent.LogicalParent;
            }

            if (groupBar != null)
            {
                if (groupBar.Orientation == Orientation.Horizontal
                    && groupBar.VisualMode == VisualMode.MultipleExpansion)
                {
                    constraint.Width = GetHorizontalExpandLength();
                }

                if (double.IsInfinity(constraint.Width))
                {
                    GroupBarItem barItem = groupBar.SelectedTab;
                    GroupView groupView = null;

                    if (barItem != null)
                    {
                        groupView = barItem.Content as GroupView;
                    }
                    else
                    {
                        groupView = groupBar.SelectedContent as GroupView;
                    }

                    constraint = new Size(groupView.ActualWidth, double.PositiveInfinity);
                }

                UpdateScrollViewer(groupBar);
            }

            constraint = base.MeasureOverride(constraint);

            return constraint;
        }

        /// <summary>
        /// Enables or disables the horizontal ScrollViewer.
        /// </summary>
        /// <param name="groupBar">The group bar.</param>
        internal void UpdateScrollViewer(GroupBar groupBar)
        {
            ScrollViewer scroll = GetTemplateChild(C_nameMainHost) as ScrollViewer;

            if (scroll != null)
            {
                if (groupBar.ContentPopup != null)
                {
                    double offsetWidth = 0d;

                    if (scroll.ComputedVerticalScrollBarVisibility == Visibility.Visible)
                    {
                        offsetWidth = groupBar.ContentPopup.Width - 38;
                    }
                    else
                    {
                        offsetWidth = groupBar.ContentPopup.Width - 22;
                    }

                    if (groupBar.ContentPopup.IsOpen &&
                        !double.IsNaN(groupBar.ContentPopup.Width) &&
                        offsetWidth <= groupBar.MinWidthOfList &&
                        groupBar.MinWidthOfList != 0)
                    {
                        scroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
                    }
                    else
                    {
                        scroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
                    }
                }
            }
        }

        /// <summary>
        /// Called to arrange and size the content of the control.
        /// </summary>
        /// <param name="arrangeBounds">The computed size that is used to arrange the content.</param>
        /// <returns>The size of the control.</returns>
        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            Size bounds = base.ArrangeOverride(arrangeBounds);
            this.UpdateInternalContentLength();
            return bounds;
        }

        /// <summary>
        /// Coerces the value of <see cref="InternalContentLength"/> property.
        /// </summary>
        /// <param name="d">Object to which the property belongs.</param>
        /// <param name="value">Value that should be checked.</param>
        /// <returns>
        /// Checked value.
        /// </returns>
        private static object OnInternalContentLengthCoerce(DependencyObject d, object value)
        {
            GroupView view = d as GroupView;

            if (view != null && view.HorizontalContentAlignment == HorizontalAlignment.Stretch)
            {
                if (view.LogicalParent != null && view.LogicalParent.LogicalParent != null)
                {
                    if (!(view.LogicalParent.LogicalParent.Orientation == Orientation.Horizontal
                        && view.LogicalParent.LogicalParent.VisualMode == VisualMode.MultipleExpansion)
                        && view.ActualWidth != 0)
                    {
                        value = view.GetContentLength();
                    }
                }
                else
                {
                    value = view.GetContentLength();
                }
            }

            return value;
        }

        /// <summary>
        /// Invoked when the value of <see cref="InternalContentLength"/> property is changed.
        /// </summary>
        /// <param name="d">GroupView object.</param>
        /// <param name="e">The instance containing the event data.</param>
        private static void OnInternalContentLengthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        /// <summary>
        /// Invoked when the <see cref="Orientation"/> property is changed.
        /// </summary>
        /// <param name="d">GroupView object.</param>
        /// <param name="e">The instance containing the event data.</param>
        private static void OnOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GroupView view = d as GroupView;

            if (view != null)
            {
                if (view.LogicalParent != null && view.LogicalParent.LogicalParent != null)
                {
                    GroupBar bar = view.LogicalParent.LogicalParent;
                    if (bar != null)
                    {
                        bar.UpdateItemsSize();
                        bar.UpdateLayout();
                    }
                }
                else
                {
                    view.UpdateInternalContentLength();
                    view.UpdatePropertyCoerce();
                    view.InvalidateMeasureItems();
                }
            }
        }

        /// <summary>
        /// Invalidates the measurement state (layout) for all item 
        /// from the Items collection. 
        /// </summary>
        internal void InvalidateMeasureItems()
        {
            GroupViewItem item = null;

            foreach (object obj in Items)
            {
                item = GetItemContainer(obj);

                if (item != null)
                {
                    item.InvalidateMeasure();
                }
            }
        }

        /// <summary>
        /// Updates the property coerce.
        /// </summary>
        internal void UpdatePropertyCoerce()
        {
            CoerceValue(GroupView.HorizontalContentAlignmentProperty);
            CoerceValue(GroupView.IsListViewModeProperty);
            CoerceValue(GroupView.IsShowTextProperty);
            CoerceValue(GroupView.InternalContentLengthProperty);
        }

        /// <summary>
        /// Invoked when <see cref="IsListViewMode"/> property is changed.
        /// </summary>
        /// <param name="d">The <see cref="GroupView"/> object.</param>
        /// <param name="e">The instance containing the event data.</param>
        private static void OnIsListViewModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GroupView view = d as GroupView;

            if (view != null)
            {
                if (view.LogicalParent != null)
                {
                    view.LogicalParent.UpdateContentSize(view.LogicalParent.IsExpanded);
                    view.LogicalParent.UpdateContentAnimation();
                }
            }
        }

        /// <summary>
        /// Coerces the value of <see cref="IsListViewMode"/> property.
        /// </summary>
        /// <param name="d">Object to which the property belongs.</param>
        /// <param name="value">Value that should be checked.</param>
        /// <returns>
        /// Checked value.
        /// </returns>
        private static object OnIsListViewModeCoerce(DependencyObject d, object value)
        {
            GroupView view = d as GroupView;

            if (view != null)
            {
                if (null != view.LogicalParent && null != view.LogicalParent.LogicalParent)
                {
                    if (view.LogicalParent.LogicalParent.VisualMode == VisualMode.MultipleExpansion
                        && view.LogicalParent.LogicalParent.Orientation == Orientation.Horizontal)
                    {
                        value = true;
                    }
                    else if (view.Orientation == Orientation.Horizontal)
                    {
                        value = false;
                    }
                }
                else if (view.Orientation == Orientation.Horizontal)
                {
                    value = false;
                }
            }

            return value;
        }

        /// <summary>
        /// Coerces the value of <see cref="IsShowText"/> property.
        /// </summary>
        /// <param name="d">Object to which the property belongs.</param>
        /// <param name="value">Value that should be checked.</param>
        /// <returns>
        /// Checked value.
        /// </returns>
        private static object OnIsShowTextCoerce(DependencyObject d, object value)
        {
            GroupView view = d as GroupView;

            if (view != null && view.Orientation == Orientation.Horizontal)
            {
                value = true;
            }

            return value;
        }

        /// <summary>
        /// Called when <see cref="IsShowText"/> property is changed.
        /// </summary>
        /// <param name="d">GroupBarItem object.</param>
        /// <param name="e">The instance containing the event data.</param>
        private static void OnIsShowTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        /// <summary>
        /// Gets the maximum content length.
        /// </summary>
        /// <returns>
        /// The maximum content length.
        /// </returns>
        private double GetContentLength()
        {
            double length = 0;
            double itemLength = 0;
            GroupViewItem item = null;

            foreach (object obj in Items)
            {
                item = GetItemContainer(obj);

                if (item != null && item.MainHost != null && item.ContentHost != null)
                {
                    itemLength = item.MainHost.ActualWidth;
                    itemLength -= item.ContentHost.Margin.Left + item.ContentHost.Margin.Right;

                    if (length < itemLength)
                    {
                        length = itemLength;
                    }
                }
            }

            if (MainHost is ScrollViewer)
            {
                ScrollViewer scroll = (ScrollViewer)MainHost;
                length -= scroll.ScrollableWidth;
            }

            return length;
        }

        /// <summary>
        /// Gets the item container.
        /// </summary>
        /// <param name="item">The item GroupViewItem.</param>
        /// <returns>GroupView Item</returns>
        internal GroupViewItem GetItemContainer(object item)
        {
            GroupViewItem container = null;

            if (item != null)
            {
                container = item as GroupViewItem;

                if (container == null)
                {
                    container = ItemContainerGenerator.ContainerFromItem(item) as GroupViewItem;
                }
            }

            return container;
        }

        /// <summary>
        /// Gets the length for the expanded state when the <see cref="Syncfusion.Windows.Tools.Controls.GroupBar"/> 
        /// object has horizontal orientation.
        /// </summary>
        /// <returns>
        /// The length for the expanded state.
        /// </returns>
        private double GetHorizontalExpandLength()
        {
            double length = InternalContentLength + C_dHorizontalOrientationGap;
            return length;
        }

        /// <summary>
        /// Called when TextAlignmet property is changed.
        /// </summary>
        /// <param name="d">GroupView object.</param>
        /// <param name="e">The instance containing the event data.</param>
        private static void OnTextAlignmentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        /// <summary>
        /// Coerces the value of TextAlignmet property.
        /// </summary>
        /// <param name="d">Object to which the property belongs.</param>
        /// <param name="value">Value that should be checked.</param>
        /// <returns>
        /// Checked value.
        /// </returns>
        private static object OnTextAlignmentCoerce(DependencyObject d, object value)
        {
            GroupView view = d as GroupView;

            if (view != null)
            {
                HorizontalAlignment alignment = (HorizontalAlignment)value;

                if (alignment == HorizontalAlignment.Stretch)
                {
                    value = HorizontalAlignment.Left;
                }
            }

            return value;
        }

        /// <summary>
        /// Raises the <see cref="System.Windows.FrameworkElement.SizeChanged"/> event, 
        /// using the specified information as part of the eventual event data. 
        /// </summary>
        /// <param name="sizeInfo">Details of the old and new size involved in the change.</param>
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            UpdatePropertyCoerce();
            base.OnRenderSizeChanged(sizeInfo);
        }

        /// <summary>
        /// Gets the index of the selected item.
        /// </summary>
        /// <returns> index value</returns>
        internal int GetSelectedItemIndex()
        {
            int index = -1;
            GroupViewItem item = null;

            foreach (object obj in Items)
            {
                item = GetItemContainer(obj);

                if (item != null && item.IsSelected)
                {
                    index = Items.IndexOf(obj);
                }
            }

            return index;
        }
        #endregion
    }
}
