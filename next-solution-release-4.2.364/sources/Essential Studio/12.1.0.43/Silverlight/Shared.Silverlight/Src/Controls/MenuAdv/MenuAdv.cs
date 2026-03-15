#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Windows.Shared
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Net;
    using System.Windows;
    using System.Windows.Controls;
    using Syncfusion.Windows.Controls.Primitives;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Threading;
    using System.Windows.Shapes;
    using Syncfusion.Windows.Shared;
    using System.Windows.Controls.Primitives;
    using System.ComponentModel;
    using System.Windows.Resources;
    using System.IO;
    using System.Windows.Markup;
#if SILVERLIGHT
    using Syncfusion.Windows.Controls.Theming;
#endif
#if WPF
using Syncfusion.Licensing;
#endif

    /// <summary>
    /// MenuAdv class
    /// </summary>

#if SILVERLIGHT
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Blend,
        Type = typeof(MenuAdv), XamlResource = "/Syncfusion.Theming.Blend;component/MenuAdv.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Blue,
        Type = typeof(MenuAdv), XamlResource = "/Syncfusion.Theming.Office2007Blue;component/MenuAdv.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Black,
        Type = typeof(MenuAdv), XamlResource = "/Syncfusion.Theming.Office2007Black;component/MenuAdv.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Silver,
        Type = typeof(MenuAdv), XamlResource = "/Syncfusion.Theming.Office2007Silver;component/MenuAdv.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Default,
        Type = typeof(MenuAdv), XamlResource = "/Syncfusion.Theming.Default;component/MenuAdv.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Blue,
        Type = typeof(MenuAdv), XamlResource = "/Syncfusion.Theming.Office2010Blue;component/MenuAdv.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Black,
        Type = typeof(MenuAdv), XamlResource = "/Syncfusion.Theming.Office2010Black;component/MenuAdv.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Silver,
        Type = typeof(MenuAdv), XamlResource = "/Syncfusion.Theming.Office2010Silver;component/MenuAdv.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Windows7,
      Type = typeof(MenuAdv), XamlResource = "/Syncfusion.Theming.Windows7;component/MenuAdv.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.VS2010,
      Type = typeof(MenuAdv), XamlResource = "/Syncfusion.Theming.VS2010;component/MenuAdv.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Metro,
  Type = typeof(MenuAdv), XamlResource = "/Syncfusion.Theming.Metro;component/MenuAdv.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Transparent ,
Type = typeof(MenuAdv), XamlResource = "/Syncfusion.Theming.Transparent;component/MenuAdv.xaml")]
#else
    [SkinType(SkinVisualStyle = Skin.Office2007Blue,
     Type = typeof(MenuAdv), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/MenuAdv/Themes/Office2007BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Silver,
    Type = typeof(MenuAdv), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/MenuAdv/Themes/Office2007SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Black,
    Type = typeof(MenuAdv), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/MenuAdv/Themes/Office2007BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Blue,
     Type = typeof(MenuAdv), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/MenuAdv/Themes/Office2010BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Silver,
    Type = typeof(MenuAdv), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/MenuAdv/Themes/Office2010SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Black,
    Type = typeof(MenuAdv), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/MenuAdv/Themes/Office2010BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2003,
     Type = typeof(MenuAdv), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/MenuAdv/Themes/Office2003Style.xaml")]
    [SkinType(SkinVisualStyle = Skin.VS2010,
     Type = typeof(MenuAdv), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/MenuAdv/Themes/VS2010Style.xaml")]
    [SkinType(SkinVisualStyle = Skin.Blend,
    Type = typeof(MenuAdv), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/MenuAdv/Themes/BlendStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Default,
     Type = typeof(MenuAdv), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/MenuAdv/Themes/MenuAdvResources.xaml")]
    [SkinType(SkinVisualStyle = Skin.ShinyRed,
     Type = typeof(MenuAdv), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/MenuAdv/Themes/ShinyRedStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.ShinyBlue,
     Type = typeof(MenuAdv), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/MenuAdv/Themes/ShinyBlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.SyncOrange,
     Type = typeof(MenuAdv), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/MenuAdv/Themes/SyncOrangeStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Metro,
    Type = typeof(MenuAdv), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/MenuAdv/Themes/MetroStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Transparent ,
   Type = typeof(MenuAdv), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/MenuAdv/Themes/TransparentStyle.xaml")]
#endif

    public class MenuAdv : ItemsControl 
    {
        # region Internal Variables

        internal Canvas OutsidePopupCanvas = null;

        internal bool IsItemSelected;

        internal bool IsItemMouseOver = false;

        internal bool isMouseOver = false;

        internal bool isInitialOrientation = true;

        internal bool firstClick = false;

        internal bool isSecondClick = false;

        internal double PanelHeight = 0;

        internal double PanelWidth = 0;

        private double menuItemWidth = 0;

        internal bool IsAltKeyPressed = false;

        internal bool IsMenuItemOpened;

        internal int accesscount = 0;

        internal bool IsTopLevelItem = false;

        internal MenuItemAdv currentOpenMenu;

        internal bool IsAllPopupClosed = false;

        #if WPF
        Window mainWindow = null;
#endif

#if SILVERLIGHT
        internal FrameworkElement root;
#endif
        # endregion

        #region Constructor
        /// <summary>
        /// 
        /// </summary>
        public MenuAdv()
        {
#if WPF
            if (EnvironmentTest.IsSecurityGranted)
            {
                EnvironmentTest.StartValidateLicense(typeof(MenuAdv));
            }
#else
            ItemContGenerator = new ItemContainerGeneratorAdv(this);
#endif
            DefaultStyleKey = typeof(MenuAdv);
            this.ContainersToItems = new Dictionary<DependencyObject, object>();
#if WPF
            EventManager.RegisterClassHandler(typeof(MenuAdv), AccessKeyManager.AccessKeyPressedEvent, new AccessKeyPressedEventHandler(OnAccessKeyPressed));
#endif
        }

#if WPF
        static MenuAdv()
        {
           // EnvironmentTest.ValidateLicense(typeof(MenuAdv));
        }

        private static void OnAccessKeyPressed(object sender, AccessKeyPressedEventArgs e)
        {
            if (!(Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt)))
            {
                e.Scope = sender;
                if (sender != null && sender is MenuAdv)
                    (sender as MenuAdv).IsAltKeyPressed = false;
                e.Handled = true;
            }
            else
            {
                if (sender != null && sender is MenuAdv)
                    (sender as MenuAdv).IsAltKeyPressed = true;
            }
        }
#endif
        #endregion

        # region Properties

        internal IDictionary<DependencyObject, object> ContainersToItems { get; set; }

        /// <summary>
        /// Gets or sets the orientation.
        /// </summary>
        /// <value>The orientation.</value>
        [Category("Appearance")]
        [Description("Represents the Orientation of the MenuAdv, Which may be Horizontal or Vertical.")]
        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        /// <summary>
        /// Gets or sets the expand mode.
        /// </summary>
        /// <value>The expand mode.</value>
        [Category("Common Properties")]
        [Description("Represents the Expand modes of MenuItems which present in the MenuAdv")]
        public ExpandModes ExpandMode
        {
            get { return (ExpandModes)GetValue(ExpandModesProperty); }
            set { SetValue(ExpandModesProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is menu item scrollability enabled.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is menu item scrollability enabled; otherwise, <c>false</c>.
        /// </value>
        [Category("Common Properties")]
        [Description("Represents menu items present in submenu popup can be scrollable or not")]
        public bool IsScrollEnabled
        {
            get { return (bool)GetValue(IsMenuItemScrollabilityEnabledProperty); }
            set { SetValue(IsMenuItemScrollabilityEnabledProperty, value); }
        }

        /// <summary>
        /// Gets or sets the type of the pop up animation.
        /// </summary>
        /// <value>The type of the pop up animation.</value>
        [Category("Common Properties")]
        [Description("Represents the ANimation type to open the subMenu popup, which may be None, Fade, Slide or Scroll.")]
        public AnimationTypes PopUpAnimationType
        {
            get { return (AnimationTypes)GetValue(PopUpAnimationTypeProperty); }
            set { SetValue(PopUpAnimationTypeProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        [Category("Common Properties")]
        [Description("Represent the Focus Enable when pressed on Alt Key")]
        public bool FocusOnAlt
        {
            get { return (bool)GetValue(FocusOnAltProperty); }
            set { SetValue(FocusOnAltProperty, value); }
        }        


        /// <summary>
        /// Gets or sets the item container style.
        /// </summary>
        /// <value>The item container style.</value>
        /// To remove Warnings
        
#if  SILVERLIGHT
         public Style ItemContainerStyle

        {
            get
            {
                return (Style)GetValue(ItemContainerStyleProperty);
            }

            set
            {
                SetValue(ItemContainerStyleProperty, value);
            }
        }
#endif
        # endregion

        # region Dependency Properties

        // Using a DependencyProperty as the backing store for ItemContainerStyle.  This adds style to the menu items...
        //To remove Warnings


#if SILVERLIGHT
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty ItemContainerStyleProperty = DependencyProperty.Register("ItemContainerStyle", typeof(Style), typeof(MenuAdv), new PropertyMetadata(null, OnItemContainerStyleChanged));
#endif

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for Scrollability.  This enables scrolling of the submenu items...
        /// </summary>
        public static readonly DependencyProperty IsMenuItemScrollabilityEnabledProperty = DependencyProperty.Register("IsScrollEnabled", typeof(bool), typeof(MenuAdv), new PropertyMetadata(true));

        /// <summary>
        /// Using a DependencyProperty as the backing store for Expand modes.  This enables the Expand on click or expand on mouse over supports...
        /// </summary>
        public static readonly DependencyProperty ExpandModesProperty = DependencyProperty.Register("ExpandMode", typeof(ExpandModes), typeof(MenuAdv), new PropertyMetadata(ExpandModes.ExpandOnClick));

        /// <summary>
        /// Using a DependencyProperty as the backing store for Orientation.  This enables the orientation of the MenuAdv...
        /// </summary>
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof(Orientation), typeof(MenuAdv), new PropertyMetadata(Orientation.Horizontal, new PropertyChangedCallback(OnOrientationChanged)));

        /// <summary>
        /// Using a DependencyProperty as the backing store for Popup Animation.  This enables the animation of popup like fade, slide or scroll.
        /// </summary>
        public static readonly DependencyProperty PopUpAnimationTypeProperty = DependencyProperty.Register("PopUpAnimationType", typeof(AnimationTypes), typeof(MenuAdv), new PropertyMetadata(AnimationTypes.None));

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsFocusonAltKey.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty FocusOnAltProperty =
            DependencyProperty.Register("FocusOnAlt", typeof(bool), typeof(MenuAdv), new PropertyMetadata(false));

        # endregion

        # region Dp Events

        private static void OnItemContainerStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
#if SilverLight
            MenuAdv source = d as MenuAdv;
            Style value = e.NewValue as Style;
            source.ItemContGenerator.UpdateItemContainerStyle(value);
#endif
        }

        private static void OnOrientationChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            MenuAdv menuadv = (MenuAdv)obj;
            if (!menuadv.isInitialOrientation)
            {
                if (obj != null && obj is UIElement)
                {
                    StackPanel sp = MenuAdv.GetStackpanel(obj as UIElement) as StackPanel;
                    if (sp != null && e.NewValue != null)
                        sp.Orientation = (Orientation)e.NewValue;                    
                }
            }

            menuadv.CloseAllPopUps();
            menuadv.ChangeExtendButtonVisibility();
        }

        private static StackPanel GetStackpanel(UIElement parent)
        {
            int count = VisualTreeHelper.GetChildrenCount(parent);
            if (count > 0)
            {
                for (int i = 0; i < count; )
                {
                    UIElement child = (UIElement)VisualTreeHelper.GetChild(parent, i);
                    i++;
                    if (child.GetType() != typeof(StackPanel))
                    {
                        StackPanel sp = GetStackpanel(child) as StackPanel;
                        if (sp != null) 
                            return sp;
                    }
                    else
                    {
                        return child as StackPanel;
                    }
                }
            }
            return null;
        }

        # endregion

        # region Overrides
#if WPF
        protected override void OnPreviewKeyUp(KeyEventArgs e)
        {
            accesscount = 0;
            base.OnPreviewKeyUp(e);
        }
#endif
        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes (such as a rebuilding layout pass) call <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>. In simplest terms, this means the method is called just before a UI element displays in an application. For more information, see Remarks.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            isInitialOrientation = true;

            this.Unloaded -= new RoutedEventHandler(MenuAdv_Unloaded);
            this.Unloaded += new RoutedEventHandler(MenuAdv_Unloaded);

            this.Loaded -= new RoutedEventHandler(MenuAdv_Loaded);
            this.Loaded += new RoutedEventHandler(MenuAdv_Loaded);
#if SILVERLIGHT
            
                root = Application.Current.RootVisual as FrameworkElement;
                FrameworkElement element = base.Parent as FrameworkElement;          
                OutsidePopupCanvas = GetTemplateChild("OutsidePopupCanvas") as Canvas;
                if (OutsidePopupCanvas != null)
                {
                    OutsidePopupCanvas.MouseLeftButtonDown += new MouseButtonEventHandler(OutsidePopupCanvas_MouseLeftButtonDown);
                }
                var ancestors = element.GetVisualAncestorsAndSelf();
                foreach (var child in ancestors)
                {
                    FrameworkElement ele = child as FrameworkElement;
                    if (ele.Equals(root) || ele is ChildWindow)
                    {
                        ele.MouseLeftButtonDown -= new MouseButtonEventHandler(this.RootVisual_MouseLeftButtonDown);
                    }
                }
                foreach (var child in ancestors)
                {                    
                    FrameworkElement ele = child as FrameworkElement;
                    if (ele.Equals(root) || ele is ChildWindow)
                    {
                        ele.MouseLeftButtonDown += new MouseButtonEventHandler(this.RootVisual_MouseLeftButtonDown);
                    }
                }
            

#endif
        }

        void MenuAdv_Loaded(object sender, RoutedEventArgs e)
        {
#if WPF
            mainWindow = Window.GetWindow(this);
            if (mainWindow != null)
            {
                mainWindow.PreviewMouseLeftButtonUp -= new MouseButtonEventHandler(MainWindow_PreviewMouseLeftButtonUp);
                mainWindow.Deactivated -= new EventHandler(MainWindow_Deactivated);
                mainWindow.LocationChanged -= new EventHandler(MainWindow_LocationChanged);
                mainWindow.Deactivated += new EventHandler(MainWindow_Deactivated);
                mainWindow.LocationChanged += new EventHandler(MainWindow_LocationChanged);
                mainWindow.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(MainWindow_PreviewMouseLeftButtonUp);
                mainWindow.KeyUp += new KeyEventHandler(MainWindow_KeyUp);
            }
#endif
            
        }

#if SILVERLIGHT
        void OutsidePopupCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Parent != null)
                this.CloseAllPopUps();
            if (OutsidePopupCanvas!=null && OutsidePopupCanvas.Visibility == Visibility.Visible)
            {
                OutsidePopupCanvas.Visibility = Visibility.Collapsed;
            }
        }
#endif
        
        void MenuAdv_Unloaded(object sender, RoutedEventArgs e)
        {
#if WPF
            if (mainWindow != null)
            {
                mainWindow.PreviewMouseLeftButtonUp -= new MouseButtonEventHandler(MainWindow_PreviewMouseLeftButtonUp);
                mainWindow.Deactivated -= new EventHandler(MainWindow_Deactivated);
                mainWindow.LocationChanged -= new EventHandler(MainWindow_LocationChanged);
                mainWindow.KeyUp -= new KeyEventHandler(MainWindow_KeyUp);
            }
#else
            if(this.root != null)
                root.MouseLeftButtonDown -= new MouseButtonEventHandler(this.RootVisual_MouseLeftButtonDown);

            if (OutsidePopupCanvas != null)
            {
                OutsidePopupCanvas.MouseLeftButtonDown -= new MouseButtonEventHandler(OutsidePopupCanvas_MouseLeftButtonDown);
            }
#endif
            this.Unloaded -= new RoutedEventHandler(MenuAdv_Unloaded);         
        }
#if WPF
        void MainWindow_KeyUp(object sender, KeyEventArgs e)
        {
            if (this.FocusOnAlt && ( e.SystemKey == Key.LeftAlt || e.SystemKey == Key.RightAlt))
            {
                if (this.Items[0] != null)
                {
                    MenuItemAdv firstMenuItem = null;
                    
                    if (Items[0] is MenuItemAdv)
                        firstMenuItem = Items[0] as MenuItemAdv;
                    else                    
                        firstMenuItem = this.ItemContainerGenerator.ContainerFromIndex(0) as MenuItemAdv;

                    if (firstMenuItem != null && !firstMenuItem.IsFocused && this.IsEnabled)
                    {
                        firstMenuItem.Focus();
                        firstMenuItem.CallVisualState(firstMenuItem, "MenuItemSelected");
                        IsAltKeyPressed = true;
                        e.Handled = true;
                    }
                    else if(firstMenuItem!=null && this.IsEnabled)
                    {
                        TraversalRequest tRequest= new TraversalRequest(FocusNavigationDirection.Down);
                        UIElement keyboardFocus = Keyboard.FocusedElement as UIElement;

                        if (keyboardFocus != null)
                        {
                            keyboardFocus.MoveFocus(tRequest);
                        }
                        IsAltKeyPressed = false;
                        firstMenuItem.CallVisualState(firstMenuItem, "Normal");
                        e.Handled = true;
                    }
                }
            }
        }

        void MainWindow_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

            for (int i = 0; i < this.ContainersToItems.Count; i++)
            {
                if (ItemContainerGenerator.ContainerFromIndex(i) is MenuItemAdv)
                {
                    if (((MenuItemAdv)ItemContainerGenerator.ContainerFromIndex(i)).ParentMenuAdv != null)
                    {
                        if (!(((MenuItemAdv)ItemContainerGenerator.ContainerFromIndex(i)).IsSubMenuOpen))
                        {
                            IsItemMouseOver = true;
                        }
                    }
                }
            }
            if (!IsItemMouseOver)
            {
                this.IsItemSelected = false;
                CloseAllPopUps();
            }
            //Close popup on clicking outside the MenuAdv
            if (IsMenuItemOpened && e.Source!=null &&!(e.Source is MenuItemAdv) && e.OriginalSource !=null && e.OriginalSource is Visual && !(VisualUtils.FindAncestor((Visual)e.OriginalSource, typeof(MenuItemAdv)) is MenuItemAdv))
            {
                CloseAllPopUps();
            }
        }
#endif

        void MainWindow_LocationChanged(object sender, EventArgs e)
        {
            this.CloseAllPopUps();
        }

        void MainWindow_Deactivated(object sender, EventArgs e)
        {
            this.CloseAllPopUps();
        }

        /// <summary>
        /// Determines if the specified item is (or is eligible to be) its own container.
        /// </summary>
        /// <param name="item">The item to check.</param>
        /// <returns>
        /// true if the item is (or is eligible to be) its own container; otherwise, false.
        /// </returns>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return ((item is MenuItemAdv) || (item is MenuItemSeparator));
        }

        /// <summary>
        /// Creates or identifies the element that is used to display the given item.
        /// </summary>
        /// <returns>
        /// The element that is used to display the given item.
        /// </returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new MenuItemAdv();
        }


        internal void UpdateSeparatorVisibility(MenuItemSeparator item)
        {
#if SILVERLIGHT
            string style = "Default";
            if (this.root != null)
                style = SkinManager.GetVisualStyle(this.root).ToString();
            ResourceDictionary rd = new ResourceDictionary();
            if (style == "Default")
            {
                rd.Source = new Uri("/Syncfusion.Shared.Silverlight;component/Controls/MenuAdv/Themes/MenuAdvResources.xaml", UriKind.RelativeOrAbsolute);
            }
            else
            {

                rd.Source = new Uri("/Syncfusion.Theming." + style + ";component/MenuAdv.xaml", UriKind.RelativeOrAbsolute);
            }
            if (this.Orientation == Orientation.Horizontal)
                (item as MenuItemSeparator).Style = rd[style + "HorizontalMenuItemSeparatorStyle"] as Style;
            else
                (item as MenuItemSeparator).Style = rd[style + "VerticalMenuItemSeparatorStyle"] as Style;
#endif
        }
#if SILVERLIGHT
        /// <summary>
        /// 
        /// </summary>
        public ItemContainerGeneratorAdv ItemContGenerator
        {
            get;
            private set;
        }
#endif

        /// <summary>
        /// Prepares the specified element to display the specified item.
        /// </summary>
        /// <param name="element">The element used to display the specified item.</param>
        /// <param name="item">The item to display.</param>
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            MenuItemAdv menuItemAdv = element as MenuItemAdv;
            if (menuItemAdv != null)
            {
                menuItemAdv.ParentMenuAdv = this;
                menuItemAdv.ParentMenuItemAdv = null;
                menuItemAdv.Parent = this;
                menuItemAdv.ParentMenu = this;
            }
           
            
            if (!(item is MenuItemSeparator))
            {
                this.ContainersToItems[element] = item;
            }

            if (item is MenuItemSeparator)
            {
                UpdateSeparatorVisibility(item as MenuItemSeparator);
            }
            base.PrepareContainerForItemOverride(menuItemAdv, item);
#if SILVERLIGHT
           
            if (ItemsSource != null && menuItemAdv != null)
            {
                ItemContGenerator.ApplyPropertiesTochild(element, item, ItemContainerStyle);
                if (menuItemAdv.ItemTemplate == null)
                {
                    menuItemAdv.ItemTemplate = this.ItemTemplate;
                }
                DataTemplate template = this.ItemTemplate;
                bool setContent = true;
                if (menuItemAdv != item)
                {
                    if (menuItemAdv.HeaderTemplate == null && template != null)
                    {
                        menuItemAdv.HeaderTemplate = template;
                    }
                    else if (!string.IsNullOrEmpty(this.DisplayMemberPath))
                    {
                        Binding binding = new Binding();
                        binding.Source = menuItemAdv.DataContext;
                        binding.Path = new PropertyPath(this.DisplayMemberPath, new object[0]);
                        TextBlock text = new TextBlock();
                        text.SetBinding(TextBlock.TextProperty, binding);
                        menuItemAdv.Header = text;
                        setContent = false;
                    }

                }

                if (setContent)
                {
                    if (menuItemAdv.ItemTemplate != null && menuItemAdv.HeaderTemplate == null)
                    {
                        DataTemplate ownTemplate = menuItemAdv.ItemTemplate;
                        menuItemAdv.HeaderTemplate = ownTemplate;
                    }
                    menuItemAdv.Header = item;
                }
            }
           
#endif
        }

        /// <summary>
        /// Called before the <see cref="E:System.Windows.UIElement.MouseEnter"/> event occurs.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            this.IsItemMouseOver = true;
        }

        /// <summary>
        /// Called before the <see cref="E:System.Windows.UIElement.MouseLeave"/> event occurs.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            this.IsItemMouseOver = false;
        }

        # endregion

        # region Events

        void RootVisual_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            for (int i = 0; i < this.ContainersToItems.Count; i++)
            {
                if (ItemContainerGenerator.ContainerFromIndex(i) is MenuItemAdv)
                {
                    if (((MenuItemAdv)ItemContainerGenerator.ContainerFromIndex(i)).ParentMenuAdv != null)
                    {
                        //MenuItemAdv menuItem = ((MenuItemAdv)ItemContainerGenerator.ContainerFromIndex(i));
                        //if (FocusManager.GetFocusedElement(this) == menuItem)
                        //{
                        //    IsItemMouseOver = true;
                        //}
                    }
                }
            }
            if (!IsItemMouseOver)
            {
                this.IsItemSelected = false;
                CloseAllPopUps();
            }
        }

        # endregion

        # region Methods

        internal StackPanel GetStackPanel()
        {
            Panel itemsHost = null;
            if (itemsHost == null)
            {
                DependencyObject treeviewitem = this.Items[0] as MenuItemAdv; //this.ItemContainerGenerator.ContainerFromIndex(0); //ChildrenToItems.First().Key;
                itemsHost = VisualTreeHelper.GetParent(treeviewitem) as Panel;
            }
            return (StackPanel)itemsHost;
        }

        internal void GetMenuItem(MenuItemAdv menuItem)
        {
            if (menuItem != null)
            {
                if (this.menuItemWidth != menuItem.ActualWidth)
                {
                    this.PanelWidth = menuItem.ActualWidth;
                    menuItem.HandlePopupOpen();
                    menuItem.SelectPopUpAnimation();
                    this.menuItemWidth = menuItem.ActualWidth;
                }
            }
        }

        internal void CloseAllPopupsInternal(MenuItemAdv item)
        {
            for (int i = 0; i < item.ContainersToItems.Count; i++)
            {
                MenuItemAdv menuItem = item.ItemContainerGenerator.ContainerFromIndex(i) as MenuItemAdv;
                if (menuItem != null && menuItem.ParentMenuItemAdv != null)
                {
                    menuItem.IsSubMenuOpen = false;
                    if (menuItem.IsEnabled == true)
                    {
                        VisualStateManager.GoToState(menuItem, "Normal", true);
                        menuItem.CallVisualState(menuItem, "Normal");
                    }
                    if (menuItem.CheckBoxPanel != null)
                        VisualStateManager.GoToState(menuItem.CheckBoxPanel, "Normal", false);
                    if (menuItem.RadioButtonPanel != null)
                        VisualStateManager.GoToState(menuItem.RadioButtonPanel, "Normal", false);
                    menuItem.IsBoundaryDetected = false;
                    if (menuItem.PART_BottomScroll != null && menuItem.PART_BottomScroll.Visibility == System.Windows.Visibility.Visible)
                        menuItem.PART_BottomScroll.Visibility = System.Windows.Visibility.Collapsed;

                    if (menuItem.PART_TopScroll != null && menuItem.PART_TopScroll.Visibility == System.Windows.Visibility.Visible)
                        menuItem.PART_TopScroll.Visibility = System.Windows.Visibility.Collapsed;
                    this.IsAltKeyPressed = false;

                    if (menuItem.Items.Count>0)
                    {
                        CloseAllPopupsInternal(menuItem);
                    }
                }
            }
        }

        internal void CloseAllPopUps()
        {
            
            if (!IsAllPopupClosed)
            {
                for (int i = 0; i < this.ContainersToItems.Count; i++)
                {
                    if (ItemContainerGenerator.ContainerFromIndex(i) is MenuItemAdv)
                    {
                        if (((MenuItemAdv)ItemContainerGenerator.ContainerFromIndex(i)).ParentMenuAdv != null)
                        {
                            CloseAllPopupsInternal(((MenuItemAdv)ItemContainerGenerator.ContainerFromIndex(i)));
                            ((MenuItemAdv)ItemContainerGenerator.ContainerFromIndex(i)).IsSubMenuOpen = false;
                        }
                    }
                }
                IsTopLevelItem = false;
                if (this.ExpandMode == ExpandModes.ExpandOnClick)
                {
                    this.firstClick = false;
                }
                this.IsAltKeyPressed = false;
                IsAllPopupClosed = true;
            }
        }

        internal void ChangeExtendButtonVisibility()
        {
            foreach (var menuitem in this.Items)
            {
                if (menuitem is MenuItemAdv)
                {
                    ((MenuItemAdv)menuitem).ChangeExtendButtonVisibility();
                }
                if (menuitem is MenuItemSeparator)
                {
                    UpdateSeparatorVisibility(menuitem as MenuItemSeparator);
                }
            }
        }       
        # endregion
    }

    /// <summary>
    /// Menu ExpandModes enumeration
    /// </summary>
    public enum ExpandModes
    {
        /// <summary>
        /// Expands menu items present in menu on mouse hover.
        /// </summary>
        ExpandOnMouseOver,

        /// <summary>
        /// Expands menu items present in menu on mouse click.
        /// </summary>
        ExpandOnClick
    }

    /// <summary>
    /// Popup AnimationType enumeration
    /// </summary>
    public enum AnimationTypes
    {
        /// <summary>
        /// 
        /// </summary>
        None,
        /// <summary>
        /// 
        /// </summary>
        Fade,
        /// <summary>
        /// 
        /// </summary>
        Slide,
        /// <summary>
        /// 
        /// </summary>
        Scroll,
        /// <summary>
        /// 
        /// </summary>
        Custom
    }

    /// <summary>
    /// MenuItem Role enumeration
    /// </summary>
    public enum Role
    {
        /// <summary>
        /// 
        /// </summary>
        SubmenuHeader,
        /// <summary>
        /// 
        /// </summary>
        SubmenuItem,
        /// <summary>
        /// 
        /// </summary>
        TopLevelHeader,
        /// <summary>
        /// 
        /// </summary>
        TopLevelItem
    }
}