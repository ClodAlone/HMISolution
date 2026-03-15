#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Windows;
using System.Linq;
using System.Windows.Data;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using System.Windows.Media.Animation;
using System.ComponentModel;
using System.Threading;
#if SILVERLIGHT
using System.Windows.Browser;
#endif

namespace Syncfusion.Windows.Shared
{
    /// <summary>
    /// Represents the Menu Item Adv Class.
    /// </summary>
    [TemplateVisualState(Name = "Normal", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "Selected", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "MenuItemFocused", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "MenuItemSelected", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "SubMenuItemFocused", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "Disabled", GroupName = "CommonStates")]

    [StyleTypedProperty(Property = "TopScrollButtonStyle", StyleTargetType = typeof(Button))]
    [StyleTypedProperty(Property = "BottomScrollButtonStyle", StyleTargetType = typeof(Button))]
    [StyleTypedProperty(Property = "CheckBoxStyle", StyleTargetType = typeof(CheckBox))]
    [StyleTypedProperty(Property = "RadioButtonStyle", StyleTargetType = typeof(RadioButton))] 
#if SILVERLIGHT
    public class MenuItemAdv : HeaderedItemsControl
#else
    public class MenuItemAdv : HeaderedItemsControl, ICommandSource
#endif
    {
        # region Variables

        internal CheckBox CheckBoxPanel;

        internal RadioButton RadioButtonPanel;

        private Popup SubMenuItemPopUp;

        private Border PopUpBorder;

        private Grid IconGrid, PopUpGrid;

        internal Grid menuItemAdvGrid;

        private StackPanel menuAdvStackPanel, panel;

        private ScrollViewer PART_ScrollViewer;

        internal Button PART_BottomScroll;

        internal Button PART_TopScroll;

        private TextBlock GestureTextBlock;

        private bool canGestureTextBlockVisible = false;

        //private double greatestWidthOfItem = 0;

        private DispatcherTimer ScrollerTick;

        private double scrollableValue = 2;

        private IEnumerator<DependencyObject> items;

        private bool ScrollabilityEnabled = false;

        internal bool IsBoundaryDetected = false;

        internal bool isSubMenuItemsPopupOpenedThroughRightKey = false;

        internal bool isSubMenuItemsPopupOpenedThroughEnterKey = false;

        internal bool isSubMenuItemsPopupOpenedThroughLeftKey = false;

        internal bool isSubMenuItemsPopupOpenedThroughDownKey = false;

        internal bool canFocused = true;

        private Border MenuItemBorder;

#if SILVERLIGHT
        private bool IsMouseDownOnMenuItemAdv = false;
#endif

        internal bool isActiveScope;

        internal bool IsRightLeftKeyPressed = false;

        private ContentControl IconContent;

        

#if SILVERLIGHT
        private bool _altWasDown = (Key.Alt) != 0;

        private Timer _keyboardPoller = null;
#endif
        private double PanelHeight;
        # endregion

        #region Constructors
        /// <summary>
        /// 
        /// </summary>
        public MenuItemAdv()
        {
            DefaultStyleKey = typeof(MenuItemAdv);
            this.ContainersToItems = new Dictionary<DependencyObject, object>();
            this.Loaded -= new RoutedEventHandler(MenuItemAdv_Loaded);
            this.Unloaded -= new RoutedEventHandler(MenuItemAdv_Unloaded);
            this.IsEnabledChanged -= new DependencyPropertyChangedEventHandler(MenuItemAdv_IsEnabledChanged);
            this.Loaded += new RoutedEventHandler(MenuItemAdv_Loaded);
            this.Unloaded += new RoutedEventHandler(MenuItemAdv_Unloaded);          
            this.IsEnabledChanged += new DependencyPropertyChangedEventHandler(MenuItemAdv_IsEnabledChanged);
        }

        void MenuItemAdv_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            MenuItemAdv item = sender as MenuItemAdv;
            if (item != null && item.IsEnabled == true)
            {
                item.IsSubMenuOpen = false;
                CallVisualState(item, "Normal");
            }
            else if (item != null && item.IsEnabled == false)
            {
                VisualStateManager.GoToState(item, "Disabled", false);
            }
        }
    
        void MenuItemAdv_Unloaded(object sender, RoutedEventArgs e)
        {
            isActiveScope = false;
            if (panel != null)
                panel.LayoutUpdated -= new EventHandler(panel_LayoutUpdated);
            if (this.CheckBoxPanel != null)
            {
                this.CheckBoxPanel.MouseLeave -= new MouseEventHandler(CheckBoxPanel_MouseLeave);
                this.CheckBoxPanel.Click -= new RoutedEventHandler(CheckBoxPanel_Click);
            }
            if (this.RadioButtonPanel != null)
            {
                this.RadioButtonPanel.MouseLeave -= new MouseEventHandler(RadioButtonPanel_MouseLeave);
                this.RadioButtonPanel.Click -= new RoutedEventHandler(RadioButtonPanel_Click);
            }
            if (this.PART_TopScroll != null)
            {
                this.PART_TopScroll.MouseEnter -= new MouseEventHandler(PART_TopScroll_MouseEnter);
                this.PART_TopScroll.MouseLeave -= new MouseEventHandler(PART_TopScroll_MouseLeave);
            }
            if (this.PART_BottomScroll != null)
            {
                this.PART_BottomScroll.MouseEnter -= new MouseEventHandler(PART_BottomScroll_MouseEnter);
                this.PART_BottomScroll.MouseLeave -= new MouseEventHandler(PART_BottomScroll_MouseLeave);
            }
            if (this.SubMenuItemPopUp != null)
                this.SubMenuItemPopUp.LayoutUpdated -= new EventHandler(SubMenuItemPopUp_LayoutUpdated);
            if (this.PopUpClosing != null)
            this.PopUpClosing -= new RoutedEventHandler(MenuItemAdv_PopUpClosing);
#if WPF
            if (this.MenuItemBorder != null)
            {
                this.MenuItemBorder.MouseEnter -= new MouseEventHandler(menuItemAdvGrid_MouseEnter);
            }
#endif
            if(ScrollerTick != null)
            ScrollerTick.Stop();
#if SILVERLIGHT
            if (_keyboardPoller != null)
            {
                _keyboardPoller.Dispose();
            }
#endif
        }    

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLostFocus(RoutedEventArgs e)
        {
            if (!this.IsSubMenuOpen)
                CallVisualState(this, "Normal");
            base.OnLostFocus(e);
        }

        #endregion


        internal ContentControl cpresenter = null;
        # region Event Handlers
        /// <summary>
        /// Occurs when [PopUpclosed].
        /// </summary>
        public event RoutedEventHandler PopUpClosed;

        /// <summary>
        /// Occurs when [PopUpclosing].
        /// </summary>
        public event RoutedEventHandler PopUpClosing;

        /// <summary>
        /// Occurs when [PopUpopened].
        /// </summary>
        public event RoutedEventHandler PopUpOpened;

        /// <summary>
        /// Occurs when [PopUpopening].
        /// </summary>
        public event RoutedEventHandler PopUpOpening;

        /// <summary>
        /// Occurs when [click].
        /// </summary>
#if !WPF
        public event RoutedEventHandler Click;
#else

        //ClickEvent event called when the MenuItemAdv is Clicked
        public static readonly RoutedEvent ClickEvent = EventManager.RegisterRoutedEvent(
           "Click",
           RoutingStrategy.Bubble,
           typeof(RoutedEventHandler),
           typeof(MenuItemAdv));

             /// Occurs when Click
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

#endif

        /// <summary>
        /// Occurs when [checked].
        /// </summary>
        public event RoutedEventHandler Checked;

        /// <summary>
        /// Occurs when [un checked].
        /// </summary>
        public event RoutedEventHandler UnChecked;

        #endregion

        # region Properties

        /// <summary>
        /// Gets or sets the containers to items.
        /// </summary>
        /// <value>The containers to items.</value>
        internal IDictionary<DependencyObject, object> ContainersToItems { get; set; }

        /// <summary>
        /// Gets or sets the parent menu adv.
        /// </summary>
        /// <value>The parent menu adv.</value>
        internal MenuAdv ParentMenuAdv
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the parent menu item adv.
        /// </summary>
        /// <value>The parent menu item adv.</value>
        internal MenuItemAdv ParentMenuItemAdv
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the parent object of this <see cref="T:System.Windows.FrameworkElement"/> in the object tree.
        /// </summary>
        /// <value></value>
        /// <returns>The parent object of this object in the object tree.</returns>
        internal new MenuAdv Parent
        {
            get;
            set;
        }

        internal MenuAdv ParentMenu
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the role.
        /// </summary>
        /// <value>The role.</value>       

        public Role Role
        {
            get { return (Role)GetValue(RoleProperty); }
            private set { SetValue(RoleProperty, value); }
        }                   
        
        /// <summary>
        /// Gets or sets a value indicating whether this instance is sub menu open.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is sub menu open; otherwise, <c>false</c>.
        /// </value>
        [Category("Common properties")]
        [Description("Represents the SubMenu popup can be open")]
        public bool IsSubMenuOpen
        {
            get { return (bool)GetValue(IsSubMenuOpenProperty); }
            set { SetValue(IsSubMenuOpenProperty, value); }
        }

        /// <summary>
        /// Gets or sets the icon.
        /// </summary>
        /// <value>The icon.</value>
        [Category("Appearance")]
        [Description("Represents the object which is to be placed as Icon.")]
        public object Icon
        {
            get { return GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is checkable.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is checkable; otherwise, <c>false</c>.
        /// </value>
        [Category("Appearance")]
        [Description("Represents the MenuItem can be checkable or not")]
        public bool IsCheckable
        {
            get { return (bool)GetValue(IsCheckableProperty); }
            set { SetValue(IsCheckableProperty, value); }
        }
        /// <summary>
        /// 
        /// </summary>
        public bool IsPressed
        {
            get { return (bool)GetValue(IsPressedProperty); }
            set { SetValue(IsPressedProperty, value); }
        }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is checked.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is checked; otherwise, <c>false</c>.
        /// </value>
        [Category("Appearance")]
        [Description("Represents the MenuItem is Checked or not")]
        public bool IsChecked
        {
            get { return (bool)GetValue(IsCheckedProperty); }
            set { SetValue(IsCheckedProperty, value); }
        }

        /// <summary>
        /// Gets or sets the type of the check icon.
        /// </summary>
        /// <value>The type of the check icon.</value>
        [Category("Appearance")]
        [Description("Represents the CheckIconType of the MenuItem, Which may be a Check Box or Radio Button")]
        public CheckIconType CheckIconType
        {
            get { return (CheckIconType)GetValue(CheckIconTypeProperty); }
            set { SetValue(CheckIconTypeProperty, value); }
        }

        /// <summary>
        /// Gets or sets the name of the group.
        /// </summary>
        /// <value>The name of the group.</value>
        [Category("Common Properties")]
        [Description("Represents the Group Name of Menu Items which are to be used as Radio Buttons")]
        public string GroupName
        {
            get { return (string)GetValue(GroupNameProperty); }
            set { SetValue(GroupNameProperty, value); }
        }

        /// <summary>
        /// Gets or sets the Input Gesture Text.
        /// </summary>
        /// <value>The Input Gesture Text.</value>
        [Category("Common Properties")]
        [Description("Represents the Command Paramenter which has to be passed with the Command to execute it.")]
        public string InputGestureText
        {
            get { return (string)GetValue(InputGestureTextProperty); }
            set { SetValue(InputGestureTextProperty, value); }
        }

        /// <summary>
        /// Gets or sets the top scroll button style.
        /// </summary>
        /// <value>The top scroll button style.</value>
        [Category("Appearance")]
        [Description("Represents the style of the Scroll Button present in the Top of the SubMenu popup")]
        public Style TopScrollButtonStyle
        {
            get { return (Style)GetValue(TopScrollButtonStyleProperty); }
            set { SetValue(TopScrollButtonStyleProperty, value); }
        }

        /// <summary>
        /// Gets or sets the bottom scroll button style.
        /// </summary>
        /// <value>The bottom scroll button style.</value>
        [Category("Appearance")]
        [Description("Represents the style of the Scroll Button present in the Bottom of the SubMenu popup")]
        public Style BottomScrollButtonStyle
        {
            get { return (Style)GetValue(BottomScrollButtonStyleProperty); }
            set { SetValue(BottomScrollButtonStyleProperty, value); }
        }

        /// <summary>
        /// Gets or sets the check box style.
        /// </summary>
        /// <value>The check box style.</value>
        [Category("Appearance")]
        [Description("Represents the style of the Check Box present in the MenuItem Icon")]
        public Style CheckBoxStyle
        {
            get { return (Style)GetValue(CheckBoxStyleProperty); }
            set { SetValue(CheckBoxStyleProperty, value); }
        }

        /// <summary>
        /// Gets or sets the radio button style.
        /// </summary>
        /// <value>The radio button style.</value>
        [Category("Appearance")]
        [Description("Represents the style of the Radio Button present in the MenuItem Icon")]
        public Style RadioButtonStyle
        {
            get { return (Style)GetValue(RadioButtonStyleProperty); }
            set { SetValue(RadioButtonStyleProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is stays open on click.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is stays open on click; otherwise, <c>false</c>.
        /// </value>
        [Category("Common Properties")]
        [Description("Represents the SubMenu popup can be closed while selecting a MenuItem")]
        public bool StaysOpenOnClick
        {
            get { return (bool)GetValue(IsStaysOpenOnClickProperty); }
            set { SetValue(IsStaysOpenOnClickProperty, value); }
        }

        /// <summary>
        /// Gets or sets the width of the panel.
        /// </summary>
        /// <value>The width of the panel.</value>
        internal double PanelWidth
        {
            get { return (double)GetValue(PanelWidthProperty); }
            set { SetValue(PanelWidthProperty, value); }
        }

        /// <summary>
        /// Gets or sets the extend button visibility.
        /// </summary>
        /// <value>The extend button visibility.</value>
        public Visibility ExtendButtonVisibility
        {
            get { return (Visibility)GetValue(ExtendButtonVisibilityProperty); }
            set { SetValue(ExtendButtonVisibilityProperty, value); }
        }

        /// <summary>
        /// Gets or sets the check box visibility.
        /// </summary>
        /// <value>The check box visibility.</value>
        public Visibility CheckBoxVisibility
        {
            get { return (Visibility)GetValue(CheckBoxVisibilityProperty); }
            set { SetValue(CheckBoxVisibilityProperty, value); }
        }

        /// <summary>
        /// Gets or sets the radio button visibility.
        /// </summary>
        /// <value>The radio button visibility.</value>
        public Visibility RadioButtonVisibility
        {
            get { return (Visibility)GetValue(RadioButtonVisibilityProperty); }
            set { SetValue(RadioButtonVisibilityProperty, value); }
        }

        /// <summary>
        /// Gets or sets the height of the scroller.
        /// </summary>
        /// <value>The height of the scroller.</value>
        public double ScrollerHeight
        {
            get { return (double)GetValue(ScrollerHeightProperty); }
            set { SetValue(ScrollerHeightProperty, value); }
        }
        # endregion

        # region Dependency Properties

        /// <summary>
        /// Using a DependencyProperty as the backing store for Role.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty RoleProperty =
            DependencyProperty.Register("Role", typeof(Role), typeof(MenuItemAdv), new PropertyMetadata(null));

        /// <summary>
        /// Using a DependencyProperty as the backing store for InputGestureText. It Represents InputGestureText of the MenuItem to be displayed right to the header.
        /// </summary>
        public static readonly DependencyProperty InputGestureTextProperty =
            DependencyProperty.Register("InputGestureText", typeof(string), typeof(MenuItemAdv), new PropertyMetadata(string.Empty));
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty IsPressedProperty =
      DependencyProperty.Register("IsPressed", typeof(bool), typeof(MenuItemAdv), new PropertyMetadata(false));
        /// <summary>
        /// Represents SubMenu popup is Open or closed
        /// </summary>
        public static readonly DependencyProperty IsSubMenuOpenProperty =
            DependencyProperty.Register("IsSubMenuOpen", typeof(bool), typeof(MenuItemAdv), new PropertyMetadata(false, OnIsSubMenuOpenChanged));

        /// <summary>
        /// Represents the ExtendButton is Visible or not
        /// </summary>
        public static readonly DependencyProperty ExtendButtonVisibilityProperty =
            DependencyProperty.Register("ExtendButtonVisibility", typeof(Visibility), typeof(MenuItemAdv), new PropertyMetadata(Visibility.Collapsed));

        /// <summary>
        /// Using a DependencyProperty as the backing store for Icon of type Object
        /// </summary>
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(object), typeof(MenuItemAdv), new PropertyMetadata(null));

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsCheckable. It Represents the MenuItem can be checkable or not.
        /// </summary>
        public static readonly DependencyProperty IsCheckableProperty =
            DependencyProperty.Register("IsCheckable", typeof(bool), typeof(MenuItemAdv), new PropertyMetadata(false, new PropertyChangedCallback(OnIsCheckablePropertyChanged)));

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsChecked. It Represents the MenuItem is Checked or not.
        /// </summary>
        public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.Register("IsChecked", typeof(bool), typeof(MenuItemAdv), new PropertyMetadata(false, OnIsCheckedPropertyChanged));

        /// <summary>
        /// Using a DependencyProperty as the backing store for CheckIconType. It may be CheckBox or Radio Button.
        /// </summary>
        internal static readonly DependencyProperty CheckIconTypeProperty =
            DependencyProperty.Register("CheckIconType", typeof(CheckIconType), typeof(MenuItemAdv), new PropertyMetadata(CheckIconType.CheckBox, new PropertyChangedCallback(OnCheckIconTypeChanged)));

        /// <summary>
        /// Using a DependencyProperty as the backing store for CheckBox Visibility. It Represents the checkBox placed in MenuItem Icon can be visible or not.
        /// </summary>
        public static readonly DependencyProperty CheckBoxVisibilityProperty =
            DependencyProperty.Register("CheckBoxVisibility", typeof(Visibility), typeof(MenuItemAdv), new PropertyMetadata(Visibility.Collapsed));

        /// <summary>
        /// Using a DependencyProperty as the backing store for Radio Button Visibility. It Represents the Radio Button placed in MenuItem Icon can be visible or not.
        /// </summary>
        public static readonly DependencyProperty RadioButtonVisibilityProperty =
            DependencyProperty.Register("RadioButtonVisibility", typeof(Visibility), typeof(MenuItemAdv), new PropertyMetadata(Visibility.Collapsed));

        /// <summary>
        /// Using a DependencyProperty as the backing store for GroupName. It Represents GroupName of the MenuItem to use the Radion Button fuctionality.
        /// </summary>
        public static readonly DependencyProperty GroupNameProperty =
            DependencyProperty.Register("GroupName", typeof(string), typeof(MenuItemAdv), new PropertyMetadata(string.Empty));

        /// <summary>
        /// Using a DependencyProperty as the backing store for Scroller Height. It Represents the checkBox placed in MEnuItem Icon can be visible or not.
        /// </summary>
        public static readonly DependencyProperty ScrollerHeightProperty =
            DependencyProperty.Register("ScrollerHeight", typeof(double), typeof(MenuItemAdv), new PropertyMetadata(0d));

        /// <summary>
        /// Represents the MenuItem popup to stay open On click
        /// </summary>
        public static readonly DependencyProperty IsStaysOpenOnClickProperty = DependencyProperty.Register(
            "StaysOpenOnClick", typeof(bool), typeof(MenuItemAdv), new PropertyMetadata(false));

        /// <summary>
        /// Represents the style of Top Scroll Button present in the subMenu Popup.
        /// </summary>
        public static readonly DependencyProperty TopScrollButtonStyleProperty = DependencyProperty.Register(
            "TopScrollButtonStyle", typeof(Style), typeof(MenuItemAdv), new PropertyMetadata(null));

        /// <summary>
        /// Represents the style of Bottom Scroll Button present in the subMenu Popup.
        /// </summary>
        public static readonly DependencyProperty BottomScrollButtonStyleProperty = DependencyProperty.Register(
            "BottomScrollButtonStyle", typeof(Style), typeof(MenuItemAdv), new PropertyMetadata(null));

        /// <summary>
        /// Represents the style of Check Box present in the MenuItem Icon.
        /// </summary>
        public static readonly DependencyProperty CheckBoxStyleProperty = DependencyProperty.Register(
            "CheckBoxStyle", typeof(Style), typeof(MenuItemAdv), new PropertyMetadata(null));

        /// <summary>
        /// Represents the style of Radio Button present in the MenuItem Icon.
        /// </summary>
        public static readonly DependencyProperty RadioButtonStyleProperty = DependencyProperty.Register(
            "RadioButtonStyle", typeof(Style), typeof(MenuItemAdv), new PropertyMetadata(null));

        internal static readonly DependencyProperty PanelWidthProperty =
            DependencyProperty.Register("PanelWidth", typeof(double), typeof(MenuItemAdv), new PropertyMetadata(OnPanelWidthChanged));

        # endregion

        # region Dependency Property Events

        private static void OnIsCheckedPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if (((MenuItemAdv)obj).IsCheckable == true)
            {
                ((MenuItemAdv)obj).OnIsCheckedPropertyChanged(args);
            }
        }

        internal static void OnPanelWidthChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            MenuItemAdv m = (MenuItemAdv)obj;
            if (m != null)
                m.OnPanelWidthChanged(args);
        }

        # endregion

        # region Overrides

        /// <summary>
        /// Builds the visual tree for the <see cref="T:System.Windows.Controls.HeaderedItemsControl"/> when a new template is applied.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.ContainersToItems.Clear();
            GetTemplateChildrens();
            RegisterEvents();

            if (this.ParentMenuAdv != null)
            {
                if (this.ParentMenuAdv.isInitialOrientation)
                {
                    menuAdvStackPanel = GetStackPanel();
                    if (menuAdvStackPanel != null)
                    {
                        Binding bind = new Binding();
                        bind.Source = this.ParentMenuAdv.Orientation;
                        bind.Mode = BindingMode.OneWay;
                        menuAdvStackPanel.SetBinding(StackPanel.OrientationProperty, bind);
                    }

                    this.ParentMenuAdv.isInitialOrientation = false;
                }
            }

            if (panel != null)
            {
                panel.LayoutUpdated -= new EventHandler(panel_LayoutUpdated);
            }

            panel = this.GetStackPanel();

            if (panel != null)
            {
                panel.LayoutUpdated += new EventHandler(panel_LayoutUpdated);
            }
#if WPF
            if (this.Command != null && (this.Command is System.Windows.Input.RoutedUICommand))
            {
                if (this.Header == null)
                    this.Header = (this.Command as System.Windows.Input.RoutedUICommand).Text;

                if ((this.InputGestureText == string.Empty) && ((this.Command as System.Windows.Input.RoutedUICommand).InputGestures.Count > 0))
                {
                    this.InputGestureText = ((this.Command as System.Windows.Input.RoutedUICommand).InputGestures[0] as System.Windows.Input.KeyGesture).DisplayString;
                }
            }
#else
            if (this.ParentMenuAdv != null)
            {
                if (!System.ComponentModel.DesignerProperties.IsInDesignTool && !Application.Current.IsRunningOutOfBrowser && this.ParentMenuAdv.FocusOnAlt)
                {
                    _keyboardPoller = new Timer(new TimerCallback(OnKeyboardPollerTick),
                        null, 0, 100 /* ms */);

                    HtmlPage.Plugin.Focus();
                }
            }

#endif
            
        }

        private void GetTemplateChildrens()
        {
            if (IconContent != null)
            {
                IconContent.Content = null;
                IconContent = null;
            }

            if (this.CheckBoxPanel != null)
            {
                this.CheckBoxPanel.MouseLeave -= new MouseEventHandler(CheckBoxPanel_MouseLeave);
            }

            if (this.RadioButtonPanel != null)
            {
                this.RadioButtonPanel.MouseLeave -= new MouseEventHandler(RadioButtonPanel_MouseLeave);
            }

            if (this.PART_TopScroll != null)
            {
                this.PART_TopScroll.MouseEnter -= new MouseEventHandler(PART_TopScroll_MouseEnter);
                this.PART_TopScroll.MouseLeave -= new MouseEventHandler(PART_TopScroll_MouseLeave);
            }

            if (this.PART_BottomScroll != null)
            {
                this.PART_BottomScroll.MouseEnter -= new MouseEventHandler(PART_BottomScroll_MouseEnter);
                this.PART_BottomScroll.MouseLeave -= new MouseEventHandler(PART_BottomScroll_MouseLeave);
            }

            if (this.SubMenuItemPopUp != null)
            {
                this.SubMenuItemPopUp.LayoutUpdated -= new EventHandler(SubMenuItemPopUp_LayoutUpdated);
                this.PopUpClosing -= new RoutedEventHandler(MenuItemAdv_PopUpClosing);
            }

            IconContent = this.GetTemplateChild("IconContent") as ContentControl;
            if (IconContent != null)
            {
                this.IconContent.Content = this.Icon;
            }

            this.CheckBoxPanel = GetTemplateChild("CheckBoxPanel") as CheckBox;
            this.RadioButtonPanel = GetTemplateChild("RadioButtonPanel") as RadioButton;
            this.menuItemAdvGrid = GetTemplateChild("menuItemAdvGrid") as Grid;

            this.SubMenuItemPopUp = GetTemplateChild("SubMenuPopup") as Popup;
            this.IconGrid = GetTemplateChild("IconGrid") as Grid;

            this.PART_TopScroll = this.GetTemplateChild("PART_TopScroll") as Button;
            this.PART_BottomScroll = this.GetTemplateChild("PART_BottomScroll") as Button;
            this.PART_ScrollViewer = this.GetTemplateChild("PART_ScrollViewer") as ScrollViewer;

            this.PopUpGrid = this.GetTemplateChild("PopUpGrid") as Grid;
            this.PopUpBorder = this.GetTemplateChild("PopUpBorder") as Border;
            this.GestureTextBlock = this.GetTemplateChild("GestureTextBlock") as TextBlock;
            this.MenuItemBorder = this.GetTemplateChild("MenuItemBorder") as Border;

#if SILVERLIGHT
            cpresenter = this.GetTemplateChild("MenuItemContent") as ContentControl;
            if (ItemsSource!=null && cpresenter!=null && cpresenter.Content == null && Header!=null)
            {
                Binding binding = new Binding();
                binding.Source = this;
                binding.Path = new PropertyPath("Header");
                binding.Mode = BindingMode.TwoWay;
                cpresenter.SetBinding(ContentControl.ContentProperty, binding);
            }
#endif
#if WPF
            EventManager.RegisterClassHandler(typeof(MenuItemAdv), AccessKeyManager.AccessKeyPressedEvent, new AccessKeyPressedEventHandler(OnAccessKeyPressed));
#endif
        }

#if WPF
        private static void OnAccessKeyPressed(object sender, AccessKeyPressedEventArgs e)
        {
            MenuItemAdv menuItem = sender as MenuItemAdv;
            bool isScope = false;
            if (menuItem.Parent != null && menuItem.Parent is MenuAdv && menuItem.Parent.IsAltKeyPressed && menuItem.Parent.IsEnabled)
            {
                if (e.Target == null)
                {
                    if (Mouse.Captured == null || Mouse.Captured is MenuAdv)
                    {
                        e.Target = menuItem;
                        if (e.OriginalSource == menuItem && menuItem.IsSubMenuOpen)
                        {
                            isScope = true;
                        }
                    }
                    else
                    {
                        e.Handled = true;
                    }
                }
                else if (e.Scope == null)
                {

                    if (e.Target != menuItem && e.Target is MenuItemAdv)
                    {
                        isScope = true;
                    }
                    else
                    {

                        DependencyObject source = e.Source as DependencyObject;

                        while (source != null)
                        {
                            if (source == menuItem)
                            {
                                break;
                            }

                            UIElement uiElement = source as UIElement;

                            if ((uiElement != null) && (ItemsControlFromItemContainer(uiElement) == menuItem))
                            {
                                isScope = true;
                                break;
                            }

                            source = VisualTreeHelper.GetParent(source);
                        }

                    }
                }
                e.Handled = true;
                if (isScope)
                {
                    e.Scope = menuItem;
                    e.Handled = true;
                }
            }
        }

        protected override void OnAccessKey(AccessKeyEventArgs e)
        {
            base.OnAccessKey(e);
            if (!e.IsMultiple)
            {
                if (Role.Equals(Role.TopLevelHeader) && Parent.IsTopLevelItem)
                {
                    this.Parent.CloseAllPopUps();
                }

                EnterKeyAction();

                if (this.Role.Equals(Role.TopLevelHeader))
                {
                    Parent.IsTopLevelItem = true;
                }
                else
                {
                    Parent.IsTopLevelItem = false;
                }
            }
            else
            {
                if(ParentMenuAdv !=null)
                    CallVisualState(this, "MenuItemSelected");
                else if(ParentMenuItemAdv!=null)
                    CallVisualState(this, "SubMenuItemFocused");
            }
           
        }
#endif
        private void RegisterEvents()
        {
            if (this.CheckBoxPanel != null)
            {
                this.CheckBoxPanel.MouseLeave += new MouseEventHandler(CheckBoxPanel_MouseLeave);
                this.CheckBoxPanel.Click += new RoutedEventHandler(CheckBoxPanel_Click);
            }

            if (this.RadioButtonPanel != null)
            {
                this.RadioButtonPanel.MouseLeave += new MouseEventHandler(RadioButtonPanel_MouseLeave);
                this.RadioButtonPanel.Click += new RoutedEventHandler(RadioButtonPanel_Click);
            }

            this.ScrollerHeight = 0d;

            if (this.PART_TopScroll != null)
            {
                ScrollerTick = new DispatcherTimer();
                ScrollerTick.Tick += new EventHandler(ScrollUp_Tick);
                this.PART_TopScroll.MouseEnter += new MouseEventHandler(PART_TopScroll_MouseEnter);
                this.PART_TopScroll.MouseLeave += new MouseEventHandler(PART_TopScroll_MouseLeave);
            }

            if (this.PART_BottomScroll != null)
            {
                this.PART_BottomScroll.MouseEnter += new MouseEventHandler(PART_BottomScroll_MouseEnter);
                this.PART_BottomScroll.MouseLeave += new MouseEventHandler(PART_BottomScroll_MouseLeave);
            }

            if (this.SubMenuItemPopUp != null)
            {
                this.SubMenuItemPopUp.LayoutUpdated += new EventHandler(SubMenuItemPopUp_LayoutUpdated);
                this.PopUpClosing += new RoutedEventHandler(MenuItemAdv_PopUpClosing);
            }
        }

        void RadioButtonPanel_Click(object sender, RoutedEventArgs e)
        {
            if (this.StaysOpenOnClick == false && this.Items.Count <= 0)
            {
                this.Parent.CloseAllPopUps();
            }
        }

        void CheckBoxPanel_Click(object sender, RoutedEventArgs e)
        {
            if (this.StaysOpenOnClick == false && this.Items.Count <= 0)
            {
                this.IsChecked = false;
                this.Parent.CloseAllPopUps();
            }
            else if (this.StaysOpenOnClick == true)
            {
                this.CheckBoxPanel.IsChecked = false;
            }
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

        /// <summary>
        /// Prepares the specified element to display the specified item.
        /// </summary>
        /// <param name="element">The container element used to display the specified item.</param>
        /// <param name="item">The content to display.</param>
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            MenuItemAdv subMenuItem = element as MenuItemAdv;
            if (subMenuItem != null)
            {
                subMenuItem.ParentMenuAdv = null;
                subMenuItem.ParentMenuItemAdv = this;
                subMenuItem.Parent = this.Parent;
                subMenuItem.ParentMenu = this.ParentMenu;
            }
            this.ContainersToItems[element] = item;
            CheckGroupName(subMenuItem);
            base.PrepareContainerForItemOverride(subMenuItem, item);
#if SILVERLIGHT
            if (ItemsSource != null && subMenuItem != null)
            {
                if (subMenuItem.ItemTemplate == null)
                {
                    subMenuItem.ItemTemplate = this.ItemTemplate;
                }
                DataTemplate template = this.ItemTemplate;
                bool setContent = true;
                if (subMenuItem != item)
                {
                    if (template != null)
                    {
                        if (subMenuItem.HeaderTemplate == null)
                        {
                            subMenuItem.HeaderTemplate = template;
                        }
                        else
                        {
                            subMenuItem.HeaderTemplate = this.HeaderTemplate;
                        }
                    }
                }
                else if (!string.IsNullOrEmpty(this.DisplayMemberPath))
                {
                    Binding binding = new Binding();
                    binding.Source = subMenuItem.DataContext;
                    binding.Path = new PropertyPath(this.DisplayMemberPath, new object[0]);
                    TextBlock text = new TextBlock();
                    text.SetBinding(TextBlock.TextProperty, binding);
                    subMenuItem.Header = text;
                    setContent = false;
                }
                if (setContent)
                {
                    if (subMenuItem.ItemTemplate != null && subMenuItem.HeaderTemplate == null)
                    {
                        DataTemplate ownTemplate = subMenuItem.ItemTemplate;
                        subMenuItem.HeaderTemplate = ownTemplate;
                    }
                    subMenuItem.Header = item;
                }
            }
            
#endif
        }

        /// <summary>
        /// Called before the <see cref="E:System.Windows.UIElement.MouseEnter"/> event occurs.
        /// </summary>
        /// <param name="e">The data for the event.</param>
#if SILVERLIGHT
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
#else
        void menuItemAdvGrid_MouseEnter(object sender, MouseEventArgs e)
        {
#endif
            if (this.Parent != null)
                this.Parent.IsItemMouseOver = true;

            if (this.ParentMenuAdv != null)
            {
                foreach (MenuItemAdv item in this.ParentMenuAdv.ContainersToItems.Keys)
                {
                    if (item != this && item.IsEnabled == true)
                    {
                        item.IsSubMenuOpen = false;
                        CallVisualState(item, "Normal");
                    }
                }

                if ((this.ParentMenuAdv.ExpandMode == ExpandModes.ExpandOnMouseOver) || (this.ParentMenuAdv.firstClick))
                {
                    if (this.Items.Count > 0)
                    {
                        //Debug.WriteLine("Call at Mouse Enter");
                        IsSubMenuOpen = true;
                    }
                }

                if (this.IsSubMenuOpen == true && this.Parent.Orientation == Orientation.Horizontal)
                {
                    CallVisualState(this, "MenuItemSelected");
                }
                else
                {
                    CallVisualState(this, "MenuItemFocused");
                }
#if WPF
                this.Focus();
#endif
            }
            else if (this.ParentMenuItemAdv != null)
            {
                foreach (var item1 in this.ParentMenuItemAdv.ContainersToItems.Keys)
                {
                    if (item1 is MenuItemAdv)
                    {
                        if (item1 != this && (item1 as MenuItemAdv).IsEnabled == true)
                        {
                            (item1 as MenuItemAdv).IsSubMenuOpen = false;
                            CallVisualState((item1 as MenuItemAdv), "Normal");
                        }
                    }
                    CallVisualState(this, "SubMenuItemFocused");
#if WPF
                    this.Focus();
#endif
                }

                if (this.Items.Count > 0)
                {
                    IsSubMenuOpen = true;
                }
            }

            if (this.ContainersToItems.Keys.Count > 0)
            {
                foreach (var item in this.ContainersToItems.Keys)
                {
                    if ((item is MenuItemAdv) && ((item as MenuItemAdv).IsEnabled == true))
                    {
                        (item as MenuItemAdv).IsSubMenuOpen = false;
                        CallVisualState(item as MenuItemAdv, "Normal");
                    }
                }
            }
        }

        /// <summary>
        /// Called before the <see cref="E:System.Windows.UIElement.MouseLeave"/> event occurs.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            if (this.ParentMenuItemAdv != null)
            {
                this.Parent.IsItemMouseOver = false;
                if (ParentMenuAdv != null)
                    this.ParentMenuAdv.isMouseOver = false;
            }

            if ((this.ParentMenuAdv != null) && (!this.ParentMenuAdv.firstClick) && (this.ParentMenuAdv.ExpandMode == ExpandModes.ExpandOnClick) && this.IsEnabled == true)
            {
                CallVisualState(this, "Normal");
                this.Parent.IsAltKeyPressed = false;
            }
            if (!this.IsSubMenuOpen)
            {
                CallVisualState(this, "Normal");
            }
        }

        /// <summary>
        /// Called before the <see cref="E:System.Windows.UIElement.MouseLeftButtonDown"/> event occurs.
        /// </summary>
        /// <param name="e">The data for the event.</param>
#if WPF
        protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
        {
           base.OnPreviewMouseLeftButtonUp(e);
        
           if (this.ParentMenuAdv != null && this.ParentMenuItemAdv !=null)
           {
               if (this.ParentMenuAdv.ExpandMode == ExpandModes.ExpandOnClick)
               {
                   foreach (MenuItemAdv item in this.ParentMenuAdv.ContainersToItems.Keys)
                   {
                       if (item.IsEnabled == true)
                       {
                           item.IsSubMenuOpen = false;
                           CallVisualState(item, "Normal");
                       }
                   }
               }
           }

           if (this.ParentMenuItemAdv != null)
           {
               if (this.Parent != null)
               {
                   if ((this.Items.Count <= 0) && (this.StaysOpenOnClick == false))
                   {
                       this.Parent.CloseAllPopUps();
                       this.IsSubMenuOpen = false;
                       this.Parent.IsItemSelected = false;
                       CallVisualState(this.ParentMenuItemAdv, "Normal");
                   }
               }
           }
           else
           {
               if (this.ParentMenuAdv != null)
               {
                   this.ParentMenuAdv.IsItemSelected = true;

                   foreach (MenuItemAdv item in this.ParentMenuAdv.ContainersToItems.Keys)
                   {
                       if (item != this && item.IsEnabled == true)
                       {
                           item.IsSubMenuOpen = false;
                           CallVisualState(item, "Normal");
                       }
                       if (item.IsEnabled == false)
                       {
                           VisualStateManager.GoToState(item, "Disabled", false);
                       }
                   }

               }
               //IsSubMenuOpen = Items.Count > 0;
               if (this.IsSubMenuOpen == true && this.Parent.Orientation == Orientation.Horizontal)
               {
                   CallVisualState(this, "MenuItemSelected");
               }
               else
               {
                   CallVisualState(this, "MenuItemFocused");
               }
               this.Focus();
           }
           if (this.Role == Role.SubmenuHeader || this.Role == Role.SubmenuItem)
           {
               this.IsPressed = false;
           }
           if (this.Command != null)
           {
               RoutedCommand command = Command as RoutedCommand;

               if (command != null)
               {
                   if(CommandTarget!=null)
                   command.Execute(CommandParameter, CommandTarget);
                   else
                       command.Execute(CommandParameter, this);
               }
               else
               {
                   ((ICommand)Command).Execute(CommandParameter);
               }
           }
             if (ClickEvent != null)
             {
               RaiseEvent(new RoutedEventArgs(ClickEvent));
             }

           if (this.IsCheckable == true && this.CheckIconType!=CheckIconType.None)
           {
               if (this.CheckIconType == CheckIconType.CheckBox )
               {
                   this.IsChecked = !this.IsChecked;
               }
               else
               {
                   if (this.GroupName.Equals(string.Empty))
                       this.IsChecked = true;
                   else
                   {
                       this.IsChecked = true;
                       this.UnCheck(this);
                   }
               }
           }
        }
#endif
#if WPF

        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonDown(e);
#endif
#if SILVERLIGHT
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            IsMouseDownOnMenuItemAdv = true;
            base.OnMouseLeftButtonDown(e);
            if ((null != Command))
            {
                if (Command.CanExecute(CommandParameter))
                {
                    Command.Execute(CommandParameter);
                    this.IsEnabled = true;
                }
                else
                {
                    this.IsEnabled = false;
                }
            }
             if (null != Click)
            {
                Click(this, new RoutedEventArgs());
            }

            if (this.IsCheckable == true)
            {
                if (this.CheckIconType == CheckIconType.CheckBox || this.CheckIconType == CheckIconType.RadioButton)
                {
                    this.IsChecked = !this.IsChecked;
                    if (this.CheckIconType == CheckIconType.RadioButton && this.ParentMenuItemAdv!=null)
                    {
                        UnCheck(this);
                    }
                }
                else
                {
                    if (this.GroupName.Equals(string.Empty))
                        this.IsChecked = true;
                    else
                    {
                        this.IsChecked = true;
                        this.UnCheck(this);
                    }
                }
            }

            if (this.ParentMenuAdv != null)
            {
                if (this.ParentMenuAdv.ExpandMode == ExpandModes.ExpandOnClick)
                {
                    foreach (MenuItemAdv item in this.ParentMenuAdv.ContainersToItems.Keys)
                    {
                        if (item.IsEnabled == true)
                        {
                            item.IsSubMenuOpen = false;
                            CallVisualState(item, "Normal");
                        }
                    }

                    if ((this.Items.Count > 0) && (!this.ParentMenuAdv.firstClick))
                    {
                        IsSubMenuOpen = true;
                        IsMouseDownOnMenuItemAdv = false;
                        this.ParentMenuAdv.firstClick = true;
                    }

                    if ((this.Items.Count > 0) && this.ParentMenuAdv.firstClick)
                    {
                        this.ParentMenuAdv.isSecondClick = !this.ParentMenuAdv.isSecondClick;
                    }

                    if (this.ParentMenuAdv.isSecondClick && this.Items.Count > 0)
                    {
                        IsSubMenuOpen = true;
                    }

                   
                   
                }
            }

            if (this.ParentMenuItemAdv != null)
            {
                if (this.Parent != null)
                {
                    if ((this.Items.Count <= 0) && (this.StaysOpenOnClick == false))
                    {
                        this.Parent.CloseAllPopUps();
                        this.Parent.IsItemSelected = false;
                        CallVisualState(this.ParentMenuItemAdv, "Normal");
                    }
                }
            }
            else
            {
                if (this.ParentMenuAdv != null)
                {
                    this.ParentMenuAdv.IsItemSelected = true;

                    foreach (MenuItemAdv item in this.ParentMenuAdv.ContainersToItems.Keys)
                    {
                        if (item != this && item.IsEnabled == true)
                        {
                            item.IsSubMenuOpen = false;
                            CallVisualState(item, "Normal");
                        }
                        if (item.IsEnabled == false)
                        {
                            VisualStateManager.GoToState(item, "Disabled", false);
                        }
                    }

                }
                //IsSubMenuOpen = Items.Count > 0;
                if (this.IsSubMenuOpen == true && this.Parent.Orientation == Orientation.Horizontal)
                {
                    CallVisualState(this, "MenuItemSelected");
                }
                else
                {
                    CallVisualState(this, "MenuItemFocused");
                }
#if WPF
                this.Focus();
#endif
            }

#else
            if (this.Role == Role.SubmenuHeader || this.Role == Role.SubmenuItem)
            {
                this.IsPressed = true;
            }

            if (this.ParentMenuAdv != null)
            {
                if (this.ParentMenuAdv.ExpandMode == ExpandModes.ExpandOnClick)
                {                

                    if ((this.Items.Count > 0) && (!this.ParentMenuAdv.firstClick))
                    {
                        IsSubMenuOpen = true;
                        this.ParentMenuAdv.firstClick = true;
                    }
                    else 
                    {
                        MenuItemAdv menuItem=null;
                        if(e.Source is MenuItemAdv)
                        {
                            menuItem = e.Source as MenuItemAdv;
                            if (menuItem != null && menuItem.ParentMenuItemAdv == null)
                            {
                                this.ParentMenuAdv.firstClick = false;
                                menuItem.IsSubMenuOpen = false;
                                CallVisualState(menuItem, "Normal");
                            }
                        }
                        else
                        {
                                menuItem = VisualUtils.FindAncestor((Visual)e.Source, typeof(MenuItemAdv)) as MenuItemAdv;
                                if (menuItem != null && menuItem.ParentMenuItemAdv == null)
                                {
                                    this.ParentMenuAdv.firstClick = false;
                                    menuItem.IsSubMenuOpen = false;
                                    CallVisualState(menuItem, "Normal");
                                }
                         }
                    }
                   
                }
            }
#endif
        }

        /// <summary>
        /// Called before the <see cref="E:System.Windows.UIElement.MouseLeftButtonUp"/> event occurs.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
        }
#if WPF
        protected override void OnPreviewKeyUp(KeyEventArgs e)
        {           
            ParentMenu.accesscount = 0;
            if (this.ParentMenuAdv != null)
            {
                int index = this.ParentMenuAdv.ItemContainerGenerator.IndexFromContainer(this);
                if (index == 0 || index == ParentMenuAdv.Items.Count - 1)
                {
                    CallVisualState(this, "MenuItemSelected");
                }
            }
            base.OnPreviewKeyUp(e);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {           
            ParentMenu.accesscount = 0;
            base.OnKeyUp(e);
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {           
            ParentMenu.accesscount = 0;
            base.OnPreviewKeyDown(e);
        }
#endif
        /// <summary>
        /// Called before the <see cref="E:System.Windows.UIElement.KeyDown"/> event occurs.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            switch ((Key)e.Key)
            {
                case Key.Up:
                    e.Handled = true;
                    if ((this.ParentMenuAdv != null) && (this.ParentMenuAdv.Orientation == Orientation.Vertical))
                    {
                        this.LeftKeyAction();
                    }
                    else
                    {
                        this.UpKeyAction();
                    }
                    this.Parent.IsAltKeyPressed = true;
                    break;

                case Key.Down:
                    e.Handled = true;
                    if ((this.ParentMenuAdv != null) && (this.ParentMenuAdv.Orientation == Orientation.Vertical))
                    {
                        this.RightKeyAction();
                    }
                    else
                    {
                        this.DownKeyAction();
                    }
                    this.Parent.IsAltKeyPressed = true;
                    break;

                case Key.Left:
                    e.Handled = true;
                    if ((this.ParentMenuAdv != null) && (this.ParentMenuAdv.Orientation == Orientation.Vertical))
                    {
                        this.UpKeyAction();
                    }
                    else
                    {
                        IsRightLeftKeyPressed = true;
                        this.LeftKeyAction();
                    }
                    this.Parent.IsAltKeyPressed = true;
                    break;

                case Key.Right:
                    e.Handled = true;
                    if ((this.ParentMenuAdv != null) && (this.ParentMenuAdv.Orientation == Orientation.Vertical))
                    {
                        this.DownKeyAction();
                    }
                    else
                    {
                        IsRightLeftKeyPressed = true;
                        this.RightKeyAction();
                    }
                    this.Parent.IsAltKeyPressed = true;
                    break;

                case Key.Tab:
                    e.Handled = true;
#region TabAction
                    bool isLast = false;
                    if (this.ParentMenuItemAdv != null)
                    {
                        if (Keyboard.Modifiers == ModifierKeys.Shift)
                        {
                            if ((this.ParentMenuAdv != null) && (this.ParentMenuAdv.Orientation == Orientation.Vertical))
                            {
                                this.LeftKeyAction();
                            }
                            else
                            {
                                this.UpKeyAction();
                            }
                        }
                        else
                        {
                            if ((this.ParentMenuAdv != null) && (this.ParentMenuAdv.Orientation == Orientation.Vertical))
                            {
                                this.RightKeyAction();
                            }
                            else
                            {
                                this.DownKeyAction();
                            }
                        }
                        this.Parent.IsAltKeyPressed = true;
                    }
                    else if (this.ParentMenuAdv != null)
                    {
#if WPF
                        if (KeyboardNavigation.GetTabNavigation(this.ParentMenuAdv) == KeyboardNavigationMode.Continue || KeyboardNavigation.GetTabNavigation(this.ParentMenuAdv) == KeyboardNavigationMode.Contained)
                        {
#else
                        if(this.ParentMenuAdv.TabNavigation == KeyboardNavigationMode.Cycle)
                        {
#endif
                            int currentIndex = this.ParentMenu.ItemContainerGenerator.IndexFromContainer(this);
                            if ((Keyboard.Modifiers != ModifierKeys.Shift && currentIndex == this.ParentMenuAdv.Items.Count - 1) ||(Keyboard.Modifiers == ModifierKeys.Shift) && currentIndex == 0)
                            {
                                isLast = true;
                                this.IsSubMenuOpen = false;
                                CallVisualState(this, "Normal");
                                e.Handled = false;
                                this.ParentMenuAdv.firstClick = false;
                            }
                        }

                        if (!isLast)
                        {
                            if (!(Keyboard.Modifiers == ModifierKeys.Shift) && this.ParentMenuAdv != null)
                            {
                                this.RightKeyAction();
                            }
                            else
                            {
                                this.LeftKeyAction();
                            }

                            this.Parent.IsAltKeyPressed = true;
                        }
                    }
                    break;
#endregion
                case Key.Enter:
                    e.Handled = true;
                    this.EnterKeyAction();
                    break;

                case Key.Escape:
                    e.Handled = true;
                    this.EscapeKeyAction();
                    break;
            }

#if WPF
            switch ((Key)e.SystemKey)
            {
                case Key.LeftAlt:
                    this.AltKeyAction();
                    break;

                case Key.RightAlt:
                    this.AltKeyAction();
                    break;
            }
#endif
            base.OnKeyDown(e);
        }

        # endregion

        # region Events

        void MenuItemAdv_PopUpClosing(object sender, RoutedEventArgs e)
        {
            CallVisualState((this), "Normal");
            foreach (var item in this.ContainersToItems.Keys)
            {
                if ((item is MenuItemAdv) && (item as MenuItemAdv).IsSubMenuOpen == true)
                {
                    (item as MenuItemAdv).IsSubMenuOpen = false;
                    if ((item as MenuItemAdv).PART_BottomScroll != null && (item as MenuItemAdv).PART_BottomScroll.Visibility == System.Windows.Visibility.Visible)
                        (item as MenuItemAdv).PART_BottomScroll.Visibility = System.Windows.Visibility.Collapsed;

                    if ((item as MenuItemAdv).PART_TopScroll != null && (item as MenuItemAdv).PART_TopScroll.Visibility == System.Windows.Visibility.Visible)
                        (item as MenuItemAdv).PART_TopScroll.Visibility = System.Windows.Visibility.Collapsed;
                }
            }

        }

        void RadioButtonPanel_MouseLeave(object sender, MouseEventArgs e)
        {
            VisualStateManager.GoToState(this.RadioButtonPanel, "MouseOver", false);
        }

        void CheckBoxPanel_MouseLeave(object sender, MouseEventArgs e)
        {
            VisualStateManager.GoToState(this.CheckBoxPanel, "MouseOver", false);
        }

        void MenuItemAdv_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.ParentMenuItemAdv == null)
                this.isActiveScope = true;
            ChangeExtendButtonVisibility();
            this.ScrollerHeight = 0d;

            if (this.ParentMenuItemAdv == null)
            {
                if (this.ParentMenuAdv != null && this.Items.Count > 0)
                {
                    this.Role = Shared.Role.TopLevelHeader;
                }
                else if (this.ParentMenuAdv != null && this.Items.Count <= 0)
                {
                    this.Role = Shared.Role.TopLevelItem;
                }

                if (this.ParentMenuAdv != null && this.IsCheckable == false && this.Icon == null && this.IconGrid != null)
                {
                    this.IconGrid.Visibility = System.Windows.Visibility.Collapsed;
                }
            }
            else
            {
                if (this.ParentMenuItemAdv != null && this.Items.Count > 0)
                {
                    this.Role = Shared.Role.SubmenuHeader;
                }
                else if (this.ParentMenuItemAdv != null && this.Items.Count <= 0)
                {
                    this.Role = Shared.Role.SubmenuItem;
                }

                foreach (var menuitem in this.ParentMenuItemAdv.ContainersToItems.Keys)
                {
                    if ((menuitem is MenuItemAdv) && ((menuitem as MenuItemAdv).InputGestureText != string.Empty))
                    {
                        this.canGestureTextBlockVisible = true;
                    }

                    //if ((menuitem is MenuItemAdv) && (this.greatestWidthOfItem < ((menuitem as MenuItemAdv).ActualWidth)))
                    //{
                    //    this.greatestWidthOfItem = (menuitem as MenuItemAdv).ActualWidth;
                    //}
                }

                if (this.GestureTextBlock != null && this.canGestureTextBlockVisible)
                    this.GestureTextBlock.Visibility = System.Windows.Visibility.Visible;

                //if (this.MenuItemContent != null && this.greatestWidthOfItem > 0)
                //    this.MenuItemContent.Width = this.greatestWidthOfItem -40;
            }

            if (this.IsEnabled == false)
                VisualStateManager.GoToState(this, "Disabled", false);
#if WPF
            if (this.MenuItemBorder != null)
            {
                this.MenuItemBorder.MouseEnter += new MouseEventHandler(menuItemAdvGrid_MouseEnter);
            }

            bool canFirstItemFocused = true;
            if (this.ParentMenuAdv != null)
            {
                foreach (var item in this.ParentMenuAdv.ContainersToItems.Keys)
                {
                    if ((item is MenuItemAdv) && (item as MenuItemAdv).IsSubMenuOpen == true)
                        canFirstItemFocused = false;
                    break;
                }

                foreach (var item in this.ParentMenuAdv.ContainersToItems.Keys)
                {
                    if ((item is MenuItemAdv) && (canFirstItemFocused == true) && (item as MenuItemAdv).IsEnabled == true && this.Parent.FocusOnAlt)
                    {
                        (item as MenuItemAdv).Focus();
                        VisualStateManager.GoToState((item as MenuItemAdv), "Normal", false);
                        if((item as MenuItemAdv).CheckBoxPanel!=null)
                        VisualStateManager.GoToState((item as MenuItemAdv).CheckBoxPanel, "Normal", false);
                        if((item as MenuItemAdv).RadioButtonPanel!=null)
                        VisualStateManager.GoToState((item as MenuItemAdv).RadioButtonPanel, "Normal", false);
                        (item as MenuItemAdv).canFocused = false;
                    }
                    break;
                }
            }

#endif
        }

#if SILVERLIGHT
        private void OnKeyboardPollerTick(object cookie)
        {
            this.Dispatcher.BeginInvoke(PollModifiers);
        }

        private void PollModifiers()
        {
            if ((Keyboard.Modifiers & ModifierKeys.Alt) != 0 && !_altWasDown)
            {
                // Alt is down, so focus the document object
                
                //MessageBox.Show(this.Header.ToString());
                if (this.ParentMenuAdv != null && this.ParentMenuAdv.FocusOnAlt)
                {
                    _altWasDown = true;
                    HtmlPage.Document.Body.Invoke("focus", new object[] { });
                    AltKeyAction();
                }
            }
            else if ((Keyboard.Modifiers & ModifierKeys.Alt) == 0 && _altWasDown)
            {
                // Alt is not down, so focus the plugin
                _altWasDown = false;
                HtmlPage.Plugin.Focus();
            }
        }
#endif
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        protected void OnPanelWidthChanged(DependencyPropertyChangedEventArgs args)
        {
            if (args.OldValue != args.NewValue)
            {
                this.HandlePopupOpen();
                this.SelectPopUpAnimation();
            }
        }

        void panel_LayoutUpdated(object sender, EventArgs e)
        {
            if (this.ParentMenuItemAdv != null)
            {
                if (this.ParentMenuItemAdv.PanelWidth != this.panel.ActualWidth)
                {
                    if (this.ParentMenuItemAdv.IsSubMenuOpen == true && this.panel.ActualWidth != 0)
                    {
                        this.ParentMenuItemAdv.PanelWidth = this.panel.ActualWidth;
                        this.ParentMenuItemAdv.PanelHeight = this.panel.ActualHeight;
                    }
                }
            }
            else if (this.Parent != null && this.Parent.PanelWidth != this.ActualWidth)
            {
                if (this.IsSubMenuOpen == true && this.ActualWidth != 0)
                {
                    if (this.SubMenuItemPopUp.IsOpen == false)
                        this.SubMenuItemPopUp.IsOpen = true;
                    this.Parent.GetMenuItem(this);
                }
            }
        }

        void SubMenuItemPopUp_LayoutUpdated(object sender, EventArgs e)
        {
            if (this.Parent != null && this.Parent.IsScrollEnabled)
            {
                if (this.PART_ScrollViewer != null && this.PART_ScrollViewer.ScrollableHeight > 0 && this.PART_ScrollViewer.ScrollableHeight != this.PART_ScrollViewer.ExtentHeight)
                {
                    this.ScrollerHeight = 10d;
                }
                else
                {
                    this.ScrollerHeight = 0d;
                }

#if WPF
                if(this.ParentMenuItemAdv != null)
                this.ParentMenuItemAdv.ScrollabilityEnabled = true;
#endif

                if (this.ParentMenuItemAdv != null && this.ParentMenuItemAdv.ScrollabilityEnabled)
                {
                    if ((this.ParentMenuItemAdv.PART_ScrollViewer.ScrollableHeight - this.ParentMenuItemAdv.PART_ScrollViewer.VerticalOffset) > 0)
                    {
                        this.ParentMenuItemAdv.PART_BottomScroll.Visibility = System.Windows.Visibility.Visible;
                    }
                    else
                    {
                        this.ParentMenuItemAdv.PART_BottomScroll.Visibility = System.Windows.Visibility.Collapsed;
                        VisualStateManager.GoToState(this.ParentMenuItemAdv.PART_BottomScroll, "Normal", false);
#if SILVERLIGHT
                        this.ParentMenuItemAdv.ScrollerTick.Stop();
#endif
                    }

                    if (this.ParentMenuItemAdv.PART_ScrollViewer.VerticalOffset > 0)
                    {
                        this.ParentMenuItemAdv.PART_TopScroll.Visibility = System.Windows.Visibility.Visible;
                    }
                    else
                    {
                        this.ParentMenuItemAdv.PART_TopScroll.Visibility = System.Windows.Visibility.Collapsed;
                        VisualStateManager.GoToState(this.ParentMenuItemAdv.PART_TopScroll, "Normal", false);
#if SILVERLIGHT
                        this.ParentMenuItemAdv.ScrollerTick.Stop();
#endif
                    }

#if WPF
                    if ((this.ParentMenuItemAdv.PART_ScrollViewer.ScrollableHeight - this.ParentMenuItemAdv.PART_ScrollViewer.VerticalOffset) > 0 && this.PopUpGrid.Height > 0)
                    {
                        this.PopUpGrid.Height = -25;
                    }
                    else if (this.ParentMenuItemAdv.PART_TopScroll.Visibility == System.Windows.Visibility.Visible && this.PopUpGrid.Height > 0)
                    {
                        this.PopUpGrid.Height = -25;
                    }

                    if (this.Parent.PopUpAnimationType == AnimationTypes.Slide && this.ParentMenuItemAdv.ParentMenuAdv != null &&
                    this.ParentMenuItemAdv.ParentMenuAdv.Orientation == Orientation.Horizontal && (!((this.ParentMenuItemAdv.PART_ScrollViewer.VerticalOffset > 0)
                    || (this.ParentMenuItemAdv.PART_ScrollViewer.ScrollableHeight - this.ParentMenuItemAdv.PART_ScrollViewer.VerticalOffset) > 0)))
                    this.ParentMenuItemAdv.SubMenuItemPopUp.VerticalOffset = -20;
#endif
                }
            }
            else if (this.ParentMenuItemAdv != null)
            {
                if (this.ParentMenuItemAdv.PART_TopScroll.Visibility == System.Windows.Visibility.Visible)
                    this.ParentMenuItemAdv.PART_TopScroll.Visibility = System.Windows.Visibility.Collapsed;

                if (this.ParentMenuItemAdv.PART_BottomScroll.Visibility == System.Windows.Visibility.Visible)
                    this.ParentMenuItemAdv.PART_BottomScroll.Visibility = System.Windows.Visibility.Collapsed;
            }

            if (this.isSubMenuItemsPopupOpenedThroughRightKey == true)
                RightKeyAction();

            if (this.isSubMenuItemsPopupOpenedThroughEnterKey == true)
                EnterKeyAction();

            if (this.isSubMenuItemsPopupOpenedThroughLeftKey == true)
                LeftKeyAction();

            if (this.isSubMenuItemsPopupOpenedThroughDownKey == true)
                DownKeyAction();
        }

        private void ScrollUp_Tick(object sender, EventArgs e)
        {
            this.PART_ScrollViewer.ScrollToVerticalOffset(this.PART_ScrollViewer.VerticalOffset + scrollableValue);
        }

        private void PART_TopScroll_MouseLeave(object sender, MouseEventArgs e)
        {
            ScrollerTick.Stop();
            if (this.IsSubMenuOpen == false)
                VisualStateManager.GoToState(PART_TopScroll, "Normal", false);
        }

        private void PART_BottomScroll_MouseLeave(object sender, MouseEventArgs e)
        {
            ScrollerTick.Stop();
            this.Parent.IsItemMouseOver = false;
            if (this.IsSubMenuOpen == false)
            {
                VisualStateManager.GoToState(PART_BottomScroll, "Normal", false);
            }
        }

        private void PART_BottomScroll_MouseEnter(object sender, MouseEventArgs e)
        {
            ScrollerTick.Start();
#if WPF
            scrollableValue = 0.2;
            this.Parent.IsItemMouseOver = true;
#endif
#if SILVERLIGHT
            scrollableValue = 1.5;
#endif
            if (this.ContainersToItems.Keys.Count > 0)
            {
                foreach (var item in this.ContainersToItems.Keys)
                {
                    if (item is MenuItemAdv && (item as MenuItemAdv).IsEnabled == true)
                    {
                        (item as MenuItemAdv).IsSubMenuOpen = false;
                        VisualStateManager.GoToState((item as MenuItemAdv), "Normal", false);
                        VisualStateManager.GoToState((item as MenuItemAdv).CheckBoxPanel, "Normal", false);
                        VisualStateManager.GoToState((item as MenuItemAdv).RadioButtonPanel, "Normal", false);
                    }
                }
            }
        }

        private void PART_TopScroll_MouseEnter(object sender, MouseEventArgs e)
        {
            ScrollerTick.Start();
#if WPF
            scrollableValue = -0.2;
            this.Parent.IsItemMouseOver = true;
#endif
#if SILVERLIGHT
            scrollableValue = -1.5;
#endif
            if (this.ContainersToItems.Keys.Count > 0)
            {
                foreach (var item in this.ContainersToItems.Keys)
                {
                    if (item is MenuItemAdv && (item as MenuItemAdv).IsEnabled == true)
                    {
                        (item as MenuItemAdv).IsSubMenuOpen = false;
                        VisualStateManager.GoToState((item as MenuItemAdv), "Normal", false);
                        VisualStateManager.GoToState((item as MenuItemAdv).CheckBoxPanel, "Normal", false);
                        VisualStateManager.GoToState((item as MenuItemAdv).RadioButtonPanel, "Normal", false);
                    }
                }
            }
        }

        private static void OnIsSubMenuOpenChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            MenuItemAdv menu = obj as MenuItemAdv;

            if (menu.Parent != null && menu.Parent is MenuAdv)
            {
                menu.Parent.IsAllPopupClosed = false;
            }
            if (menu.SubMenuItemPopUp != null)
            {
                if (menu.IsSubMenuOpen == true)
                {
                    menu.SubMenuItemPopUp.IsOpen = true;
                    menu.OnOpening(new RoutedEventArgs());
                    menu.HandlePopupOpen();
                    menu.SelectPopUpAnimation();
                    menu.OnOpened(new RoutedEventArgs());
                    if(menu.ParentMenuAdv !=null)
                        menu.ParentMenuAdv.IsMenuItemOpened = true;
                    menu.Parent.IsAltKeyPressed = true;
                }
                else
                {
                    menu.OnClosing(new RoutedEventArgs());
                    menu.SubMenuItemPopUp.IsOpen = false;
                    menu.IsBoundaryDetected = false;
                    menu.OnClosed(new RoutedEventArgs());
                    if (menu.ParentMenuAdv != null)
                    {
                        menu.ParentMenuAdv.IsMenuItemOpened = false;
                    }
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnOpening(RoutedEventArgs e)
        {
            RoutedEventHandler handler = PopUpOpening;
            if (null != handler)
            {
                handler.Invoke(this, e);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnOpened(RoutedEventArgs e)
        {
            RoutedEventHandler handler = PopUpOpened;
            foreach (var item in this.Items)
            {
                MenuItemAdv mitem = item as MenuItemAdv;
                if (mitem != null)
                {
                    mitem.isActiveScope = true;
                }
            }

            if (this.ParentMenuItemAdv != null)
            {
                foreach (var item in this.ParentMenuItemAdv.Items)
                {
                    MenuItemAdv mitem = item as MenuItemAdv;
                    if (mitem != null)
                    {
                        mitem.isActiveScope = false;
                    }
                }
            }
            else
            {
                foreach (var item in this.ParentMenu.Items)
                {
                    MenuItemAdv mitem = item as MenuItemAdv;
                    if (mitem != null)
                    {
                        mitem.isActiveScope = false;
                    }
                }
            }
            
            if (ParentMenu != null)
                ParentMenu.currentOpenMenu = this;
            if (null != handler)
            {
                handler.Invoke(this, e);
            }
#if SILVERLIGHT
            if (Parent != null && Parent is MenuAdv && Parent.OutsidePopupCanvas != null)
            {
                Parent.OutsidePopupCanvas.Visibility = Visibility.Visible;
                //Parent.OutsidePopupCanvas.Height = (Application.Current.Host.Content.ActualHeight / (Application.Current.Host.Content.ZoomFactor * 100)) * 100;
                //Parent.OutsidePopupCanvas.Width = (Application.Current.Host.Content.ActualWidth / (Application.Current.Host.Content.ZoomFactor * 100)) * 100;
            }
             var popupAncestor = FindHighestAncestor(SubMenuItemPopUp);
                if (popupAncestor == null)
                {
                    return;
                }
             popupAncestor.AddHandler(Popup.MouseLeftButtonDownEvent, (MouseButtonEventHandler)OnMouseLeftButtonDown, true);
#endif
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnClosing(RoutedEventArgs e)
        {
            RoutedEventHandler handler = PopUpClosing;
            if (null != handler)
            {
                handler.Invoke(this, e);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnClosed(RoutedEventArgs e)
        {
            RoutedEventHandler handler = PopUpClosed;
            if (ParentMenu != null)
            {
                ParentMenu.currentOpenMenu = null;
                if (this.ParentMenuItemAdv != null && this.ParentMenuItemAdv.IsSubMenuOpen)
                {
                    ParentMenu.currentOpenMenu = this.ParentMenuItemAdv;
                    CallVisualState(this, "Normal");
                }

            }
            if (null != handler)
            {
                handler.Invoke(this, e);
            }
            foreach (var item in this.Items)
            {
                MenuItemAdv mitem = item as MenuItemAdv;
                if (mitem != null)
                {
                    mitem.isActiveScope = false;
                }
            }

            if (this.ParentMenuItemAdv != null)
            {
                foreach (var item in this.ParentMenuItemAdv.Items)
                {
                    MenuItemAdv mitem = item as MenuItemAdv;
                    if (mitem != null)
                    {
                        mitem.isActiveScope = true;
                    }
                }
            }
            else
            {
                foreach (var item in this.ParentMenu.Items)
                {
                    MenuItemAdv mitem = item as MenuItemAdv;
                    if (mitem != null)
                    {
                        mitem.isActiveScope = true;
                    }
                }
            }
#if SILVERLIGHT
           
             var popupAncestor = FindHighestAncestor(SubMenuItemPopUp);
                if (popupAncestor == null)
                {
                    return;
                }
             popupAncestor.RemoveHandler(Popup.MouseLeftButtonDownEvent, (MouseButtonEventHandler)OnMouseLeftButtonDown);
#endif
        }
#if SILVERLIGHT
        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this.ParentMenuAdv != null)
            {
                if (this.ParentMenuAdv.ExpandMode == ExpandModes.ExpandOnClick)
                {
                    var storyboard = new Storyboard() { Duration = TimeSpan.Zero };
                    var objectAnimation = new ObjectAnimationUsingKeyFrames() { Duration = TimeSpan.Zero };
                    objectAnimation.KeyFrames.Add(new DiscreteObjectKeyFrame() { KeyTime = KeyTime.FromTimeSpan(TimeSpan.Zero), Value = false });
                    Storyboard.SetTarget(objectAnimation, SubMenuItemPopUp);
                    Storyboard.SetTargetProperty(objectAnimation, new PropertyPath("IsOpen"));
                    storyboard.Children.Add(objectAnimation);
                    storyboard.Begin();
                }
                if (!IsMouseDownOnMenuItemAdv)
                {
                    this.ParentMenuAdv.IsItemSelected = false;
                    this.ParentMenuAdv.CloseAllPopUps();
                }
            }
            IsMouseDownOnMenuItemAdv = false;
        }

        private static FrameworkElement FindHighestAncestor(Popup popup)
        {
            var ancestor = (FrameworkElement)popup;

            while (true)
            {
                var parent = VisualTreeHelper.GetParent(ancestor) as FrameworkElement;

                if (parent == null)
                {
                    return ancestor;
                }

                ancestor = parent;
            }
        }
#endif
        private static void OnIsCheckablePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if (((MenuItemAdv)obj).IsCheckable == false)
            {
                ((MenuItemAdv)obj).RadioButtonVisibility = Visibility.Collapsed;
                ((MenuItemAdv)obj).CheckBoxVisibility = Visibility.Collapsed;
            }
            else
            {
                if (((MenuItemAdv)obj).IsChecked)
                {
                    ((MenuItemAdv)obj).CheckBoxVisibility = ((MenuItemAdv)obj).CheckIconType != CheckIconType.RadioButton ? Visibility.Visible : Visibility.Collapsed;
                    ((MenuItemAdv)obj).RadioButtonVisibility = ((MenuItemAdv)obj).CheckIconType == CheckIconType.RadioButton ? Visibility.Visible : Visibility.Collapsed;
                }
            }
        }

        private static void OnCheckIconTypeChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if (((MenuItemAdv)obj).IsCheckable == true)
            {
                if (((MenuItemAdv)obj).IsChecked)
                {
                    ((MenuItemAdv)obj).CheckBoxVisibility = ((MenuItemAdv)obj).CheckIconType != CheckIconType.RadioButton ? Visibility.Visible : Visibility.Collapsed;
                    ((MenuItemAdv)obj).RadioButtonVisibility = ((MenuItemAdv)obj).CheckIconType == CheckIconType.RadioButton ? Visibility.Visible : Visibility.Collapsed;
                }
                else
                {
                    ((MenuItemAdv)obj).CheckBoxVisibility = Visibility.Collapsed;
                    ((MenuItemAdv)obj).RadioButtonVisibility = Visibility.Collapsed;
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        protected virtual void OnIsCheckedPropertyChanged(DependencyPropertyChangedEventArgs args)
        {
            if (this.IsChecked)
            {
                this.CheckBoxVisibility = this.CheckIconType != CheckIconType.RadioButton ? Visibility.Visible : Visibility.Collapsed;
                this.RadioButtonVisibility = this.CheckIconType == CheckIconType.RadioButton ? Visibility.Visible : Visibility.Collapsed;
                if (null != Checked)
                {
                    Checked(this, new RoutedEventArgs());
                }
            }
            else
            {
                this.CheckBoxVisibility = Visibility.Collapsed;
                this.RadioButtonVisibility = Visibility.Collapsed;
                if (null != UnChecked)
                {
                    UnChecked(this, new RoutedEventArgs());
                }
            }
        }

        # endregion

        # region Helper Methods

        /// <summary>
        /// Calls the state of the visual.
        /// </summary>
        /// <param name="Item">The item.</param>
        /// <param name="state">The state.</param>
        internal void CallVisualState(MenuItemAdv Item, string state)
        {
            VisualStateManager.GoToState(Item, state, false);
            if (state == "Normal" && Item.CheckBoxPanel != null && Item.RadioButtonPanel != null)
            {
                VisualStateManager.GoToState(Item.CheckBoxPanel, "Normal", false);
                VisualStateManager.GoToState(Item.RadioButtonPanel, "Normal", false);
            }
            else if (Item.CheckBoxPanel != null && Item.RadioButtonPanel != null)
            {
                VisualStateManager.GoToState(Item.CheckBoxPanel, "MouseOver", false);
                VisualStateManager.GoToState(Item.RadioButtonPanel, "MouseOver", false);
            }
        }

        /// <summary>
        /// Ups the key action.
        /// </summary>
        internal void UpKeyAction()
        {
            if (this.ParentMenuAdv != null)
            {
                if (this.Items.Count > 0)
                {
                    int currentIndex = this.Items.Count - 1;
                    items = this.ContainersToItems.Keys.GetEnumerator();
                    items.Reset();
                    for (int j = 0; j <= currentIndex; j++)
                    {
                        items.MoveNext();
                    }

                    while ((currentIndex >= 0) && (items.Current is MenuItemSeparator))
                    {
                        currentIndex--;
                        items.Reset();
                        for (int j = 0; j <= currentIndex; j++)
                        {
                            items.MoveNext();
                        }
                    }

                    if ((currentIndex < this.Items.Count) && (items.Current is MenuItemAdv))
                    {
                        CallVisualState((MenuItemAdv)items.Current, "SubMenuItemFocused");
                        ((MenuItemAdv)items.Current).Focus();
                    }
                }
            }
            else if (this.ParentMenuItemAdv != null)
            {
                int currentIndex = 0;
                items = this.ParentMenuItemAdv.ContainersToItems.Keys.GetEnumerator();
                items.Reset();
                items.MoveNext();
                while ((items.Current is MenuItemSeparator) || (((MenuItemAdv)items.Current) != this))
                {
                    currentIndex++;
                    items.MoveNext();
                }

                CallVisualState((MenuItemAdv)items.Current, "Normal");
                ((MenuItemAdv)items.Current).IsSubMenuOpen = false;
                if (currentIndex >= 0)
                {
                    currentIndex--;
                    items.Reset();
                    for (int j = 0; j <= currentIndex; j++)
                    {
                        items.MoveNext();
                    }
                }

                while ((currentIndex >= 0) && ((items.Current is MenuItemSeparator) || (!((MenuItemAdv)items.Current).IsEnabled)))
                {
                    currentIndex--;
                    items.Reset();
                    for (int j = 0; j <= currentIndex; j++)
                    {
                        items.MoveNext();
                    }
                }

                if (currentIndex >= 0)
                {
                    CallVisualState((MenuItemAdv)items.Current, "SubMenuItemFocused");
                    ((MenuItemAdv)items.Current).Focus();
                }
                else
                {
                    currentIndex = this.ParentMenuItemAdv.Items.Count - 1;
                    items.Reset();
                    for (int j = 0; j <= currentIndex; j++)
                    {
                        items.MoveNext();
                    }

                    while ((currentIndex >= 0) && ((items.Current is MenuItemSeparator) || (!((MenuItemAdv)items.Current).IsEnabled)))
                    {
                        currentIndex--;
                        items.Reset();
                        for (int j = 0; j <= currentIndex; j++)
                        {
                            items.MoveNext();
                        }
                    }
                    CallVisualState((MenuItemAdv)items.Current, "SubMenuItemFocused");
                    ((MenuItemAdv)items.Current).Focus();
                }
            }
        }

        /// <summary>
        /// Downs the key action.
        /// </summary>
        internal void DownKeyAction()
        {
            if (this.ParentMenuAdv != null)
            {
                if (this.Items.Count > 0)
                {
                    this.IsSubMenuOpen = true;
                    this.ParentMenuAdv.firstClick = true;
                    this.isSubMenuItemsPopupOpenedThroughDownKey = true;
                    foreach (var menuItem in this.ContainersToItems.Keys)
                    {
                        if (menuItem is MenuItemAdv)
                        {
                            CallVisualState((this), "MenuItemSelected");
                            (menuItem as MenuItemAdv).Focus();
                            CallVisualState((menuItem as MenuItemAdv), "SubMenuItemFocused");
                            this.isSubMenuItemsPopupOpenedThroughDownKey = false;
                            break;
                        }
                    }
                }
            }
            else if (this.ParentMenuItemAdv != null)
            {
                int currentIndex = 0;
                items = this.ParentMenuItemAdv.ContainersToItems.Keys.GetEnumerator();
                items.Reset();
                items.MoveNext();
                while ((items.Current is MenuItemSeparator) || (((MenuItemAdv)items.Current) != this))
                {
                    items.MoveNext();
                    currentIndex++;
                }
                CallVisualState((MenuItemAdv)items.Current, "Normal");
                ((MenuItemAdv)items.Current).IsSubMenuOpen = false;
                if (currentIndex < this.ParentMenuItemAdv.Items.Count)
                {
                    items.MoveNext();
                    currentIndex++;
                }

                while (items.Current != null && (items.Current is MenuItemSeparator || (!((MenuItemAdv)items.Current).IsEnabled)))
                {
                    if ((currentIndex + 1) < this.ParentMenuItemAdv.ContainersToItems.Keys.Count)
                    {
                        currentIndex++;
                        items.MoveNext();
                    }
                    else
                    {
                        currentIndex = 0;
                        items.Reset();
                        items.MoveNext();
                        while ((currentIndex < this.ParentMenuItemAdv.Items.Count) && (!((MenuItemAdv)items.Current).IsEnabled))
                        {
                            items.MoveNext();
                            currentIndex++;
                        }
                        CallVisualState((MenuItemAdv)items.Current, "SubMenuItemFocused");
                        ((MenuItemAdv)items.Current).Focus();
                    }
                }

                if (currentIndex < this.ParentMenuItemAdv.Items.Count)
                {
                    if (items.Current is MenuItemAdv)
                    {
                        CallVisualState((MenuItemAdv)items.Current, "SubMenuItemFocused");
                        ((MenuItemAdv)items.Current).Focus();
                    }
                }
                else
                {
                    currentIndex = 0;
                    items.Reset();
                    items.MoveNext();
                    while ((currentIndex < this.ParentMenuItemAdv.Items.Count) && (!((MenuItemAdv)items.Current).IsEnabled))
                    {
                        items.MoveNext();
                        currentIndex++;
                    }
                    CallVisualState((MenuItemAdv)items.Current, "SubMenuItemFocused");
                    ((MenuItemAdv)items.Current).Focus();
                }
            }
        }

        /// <summary>
        /// Rights the key action.
        /// </summary>
        internal void RightKeyAction()
        {
            if (this.ParentMenuAdv != null)
            {
                items = this.ParentMenuAdv.ContainersToItems.Keys.GetEnumerator();
                items.Reset();
                items.MoveNext();
                int currentIndex = 0;
                while ((items.Current is MenuItemSeparator) || ((items.Current) != this))
                {
                    items.MoveNext();
                    currentIndex++;
                }
                CallVisualState(this, "Normal");

                if (((MenuItemAdv)items.Current).IsEnabled == true)
                {
                    CallVisualState((MenuItemAdv)items.Current, "Normal");
                    ((MenuItemAdv)items.Current).IsSubMenuOpen = false;
                }

                if (currentIndex < this.ParentMenuAdv.Items.Count)
                {
                    items.MoveNext();
                    currentIndex++;
                }

                while ((currentIndex < this.ParentMenuAdv.ContainersToItems.Keys.Count) && ((items.Current is MenuItemSeparator) || (!((MenuItemAdv)items.Current).IsEnabled)))
                {
                    currentIndex++;
                    items.MoveNext();
                }

                if (currentIndex <= this.ParentMenuAdv.ContainersToItems.Keys.Count)
                {
                    if (currentIndex == this.ParentMenuAdv.ContainersToItems.Keys.Count)
                    {
                        currentIndex = 0;
                        items.Reset();
                        items.MoveNext();
                    }
                    //if (((MenuItemAdv)items.Current).IsSubMenuOpen == true)
                    //{
                    //if(ItemContainerGenerator.IndexFromContainer(items.Current)!= Items.Count-1)
                    //    CallVisualState((MenuItemAdv)items.Current, "MenuItemSelected");
                    ////}
                    //else
                    //{
                    //    CallVisualState((MenuItemAdv)items.Current, "MenuItemFocused");
                    //}
                        CallVisualState((MenuItemAdv)items.Current, "MenuItemSelected");
                        ((MenuItemAdv)items.Current).Focus();
                        
                    if ((this.ParentMenuAdv.ExpandMode == ExpandModes.ExpandOnMouseOver) || (this.ParentMenuAdv.firstClick))
                    {
                        ((MenuItemAdv)items.Current).IsSubMenuOpen = true;
                        foreach (var item in ((MenuItemAdv)items.Current).Items)
                        {
                            if(item is MenuItemAdv)
                                UpdateCanExecute(item as MenuItemAdv);
                        }
                        
                        if (ItemContainerGenerator.IndexFromContainer(items.Current) != Items.Count - 1)
                            CallVisualState((MenuItemAdv)items.Current, "MenuItemSelected");
                        if (IsRightLeftKeyPressed)
                        {
                            IsRightLeftKeyPressed = false;
                            MenuItemAdv NextItem = (items.Current as MenuItemAdv).ItemContainerGenerator.ContainerFromIndex(0) as MenuItemAdv;
                            if (NextItem != null)
                            {
                                NextItem.Focus();
                                CallVisualState(NextItem, "SubMenuItemFocused");
                            }
                        }
#if !WPF
                        foreach (var menuItem in ((MenuItemAdv)items.Current).ContainersToItems.Keys)
                        {
                            if (menuItem is MenuItemAdv && (menuItem as MenuItemAdv).IsEnabled == true)
                            {
                                (menuItem as MenuItemAdv).Focus();
                                CallVisualState((menuItem as MenuItemAdv), "SubMenuItemFocused");
                                ((MenuItemAdv)items.Current).isSubMenuItemsPopupOpenedThroughRightKey = false;
                                break;
                            }
                        }
#endif
                    }
                }
                else
                {
                    currentIndex = 0;
                    items.Reset();
                    items.MoveNext();
                    while ((currentIndex < this.ParentMenuAdv.ContainersToItems.Keys.Count) && ((items.Current is MenuItemSeparator) || (!((MenuItemAdv)items.Current).IsEnabled)))
                    {
                        items.MoveNext();
                        currentIndex++;
                    }

                    CallVisualState((MenuItemAdv)items.Current, "MenuItemSelected");
                    ((MenuItemAdv)items.Current).Focus();
                    if ((this.ParentMenuAdv.ExpandMode == ExpandModes.ExpandOnMouseOver) || (this.ParentMenuAdv.firstClick))
                    {
                        ((MenuItemAdv)items.Current).IsSubMenuOpen = true;
                        foreach (var item in ((MenuItemAdv)items.Current).Items)
                        {
                            if (item is MenuItemAdv)
                                UpdateCanExecute(item as MenuItemAdv);
                        }
                        ((MenuItemAdv)items.Current).isSubMenuItemsPopupOpenedThroughRightKey = true;
                        CallVisualState((MenuItemAdv)items.Current, "MenuItemSelected");
                        if (IsRightLeftKeyPressed)
                        {
                            IsRightLeftKeyPressed = false;
                            ((items.Current as MenuItemAdv).ItemContainerGenerator.ContainerFromIndex(0) as MenuItemAdv).Focus();
                            //CallVisualState((menuItem as MenuItemAdv), "SubMenuItemFocused");
                            CallVisualState((items.Current as MenuItemAdv).ItemContainerGenerator.ContainerFromIndex(0) as MenuItemAdv, "SubMenuItemFocused");
                        }
#if !WPF
                        foreach (var menuItem in ((MenuItemAdv)items.Current).ContainersToItems.Keys)
                        {
                            if (menuItem is MenuItemAdv && (menuItem as MenuItemAdv).IsEnabled == true)
                            {
                                (menuItem as MenuItemAdv).Focus();
                                CallVisualState((menuItem as MenuItemAdv), "SubMenuItemFocused");
                                ((MenuItemAdv)items.Current).isSubMenuItemsPopupOpenedThroughRightKey = false;
                                break;
                            }
                        }
#endif
                    }
                }
            }
            else if (this.ParentMenuItemAdv != null)
            {
                if (this.Items.Count > 0)
                {
                    this.Focus();
                    this.IsSubMenuOpen = true;
                    foreach (var item in this.Items)
                    {
                        if (item is MenuItemAdv)
                            UpdateCanExecute(item as MenuItemAdv);
                    }
                    this.isSubMenuItemsPopupOpenedThroughRightKey = true;

                    foreach (var menuItem in this.ContainersToItems.Keys)
                    {
                        if (menuItem is MenuItemAdv && (menuItem as MenuItemAdv).IsEnabled == true)
                        {
                            (menuItem as MenuItemAdv).Focus();
                            CallVisualState((menuItem as MenuItemAdv), "SubMenuItemFocused");
                            this.isSubMenuItemsPopupOpenedThroughRightKey = false;
                            break;
                        }
                    }
                }
                else if (this.ParentMenuItemAdv.ParentMenuAdv != null)
                {
                    CallVisualState(this, "Normal");
                    this.ParentMenuItemAdv.IsRightLeftKeyPressed = true;
                    this.ParentMenuItemAdv.RightKeyAction();
                }
                else if (this.ParentMenuItemAdv != null && this.ParentMenuItemAdv.ParentMenuAdv == null)
                {
                    MenuItemAdv item1 = this;

                    while (item1.ParentMenuItemAdv != null & item1.ParentMenuItemAdv.ParentMenuAdv == null)
                    {
                        MenuItemAdv parentItem = item1.ParentMenuItemAdv;
                        if (item1.IsEnabled == true)
                        {
                            CallVisualState(item1, "Normal");
                            item1.ParentMenuItemAdv.IsSubMenuOpen = false;
                            item1 = parentItem;
                        }
                    }
                    CallVisualState(item1, "Normal");
                    item1.ParentMenuItemAdv.IsRightLeftKeyPressed = true;
                    item1.ParentMenuItemAdv.RightKeyAction();
                }
            }
        }

        /// <summary>
        /// Lefts the key action.
        /// </summary>
        internal void LeftKeyAction()
        {
            if (this.ParentMenuAdv != null)
            {
                items = this.ParentMenuAdv.ContainersToItems.Keys.GetEnumerator();
                items.Reset();
                items.MoveNext();
                int currentIndex = 0;
                while ((items.Current is MenuItemSeparator) || (((MenuItemAdv)items.Current) != this))
                {
                    items.MoveNext();
                    currentIndex++;
                }

                if (((MenuItemAdv)items.Current).IsEnabled == true)
                {
                    CallVisualState((MenuItemAdv)items.Current, "MenuItemSelected");
                    ((MenuItemAdv)items.Current).IsSubMenuOpen = false;
                }

                if (currentIndex >= 0)
                {
                    currentIndex--;
                    items.Reset();
                    for (int i = 0; i <= currentIndex; i++)
                    {
                        items.MoveNext();
                    }
                }

                while ((currentIndex >= 0) && ((items.Current is MenuItemSeparator) || (!((MenuItemAdv)items.Current).IsEnabled)))
                {
                    currentIndex--;
                    items.Reset();
                    for (int i = 0; i <= currentIndex; i++)
                    {
                        items.MoveNext();
                    }
                }

                if (currentIndex >= 0)
                {
                    if (((MenuItemAdv)items.Current).IsEnabled)
                    {
                        ((MenuItemAdv)items.Current).Focus();
                        if ((this.ParentMenuAdv.ExpandMode == ExpandModes.ExpandOnMouseOver) || (this.ParentMenuAdv.firstClick))
                        {
                            ((MenuItemAdv)items.Current).IsSubMenuOpen = true;
                            if (IsRightLeftKeyPressed)
                            {
                                IsRightLeftKeyPressed = false;
                                CallVisualState((MenuItemAdv)items.Current, "MenuItemSelected");
                                MenuItemAdv NextItem = (items.Current as MenuItemAdv).ItemContainerGenerator.ContainerFromIndex(0) as MenuItemAdv;
                                if(NextItem!=null)
                                {
                                NextItem.Focus();
                                CallVisualState(NextItem, "SubMenuItemFocused");
                                }
                            }
                            //((MenuItemAdv)items.Current).isSubMenuItemsPopupOpenedThroughKB = true;
                        }
                            CallVisualState((MenuItemAdv)items.Current, "MenuItemSelected");
                    }
                }
                else
                {
                    currentIndex = this.ParentMenuAdv.ContainersToItems.Keys.Count - 1;
                    items.Reset();
                    for (int i = 0; i <= currentIndex; i++)
                    {
                        items.MoveNext();
                    }

                    while ((currentIndex >= 0) && ((items.Current is MenuItemSeparator) || (!((MenuItemAdv)items.Current).IsEnabled)))
                    {
                        currentIndex--;
                        items.Reset();
                        for (int i = 0; i <= currentIndex; i++)
                        {
                            items.MoveNext();
                        }
                    }

                    if ((this.ParentMenuAdv.ExpandMode == ExpandModes.ExpandOnMouseOver) || (this.ParentMenuAdv.firstClick))
                    {
                        ((MenuItemAdv)items.Current).IsSubMenuOpen = true;
                        //((MenuItemAdv)items.Current).isSubMenuItemsPopupOpenedThroughKB = true;
                    }
                    CallVisualState((MenuItemAdv)items.Current, "MenuItemSelected");
                    ((MenuItemAdv)items.Current).Focus();
                }
            }
            else if (this.ParentMenuItemAdv != null)
            {
                if (this.ParentMenuItemAdv.ParentMenuAdv == null)
                {
                    if (this.ParentMenuItemAdv.IsSubMenuOpen)
                    {
                        CallVisualState(this, "Normal");
                        this.ParentMenuItemAdv.IsSubMenuOpen = false;
                        CallVisualState(this.ParentMenuItemAdv, "SubMenuItemFocused");
                        this.ParentMenuItemAdv.Focus();
                    }
                    else
                    {
                        this.ParentMenuItemAdv.LeftKeyAction();
                    }
                }
                else
                {
                    CallVisualState(this, "Normal");
                    this.ParentMenuItemAdv.IsRightLeftKeyPressed = true;
                    this.ParentMenuItemAdv.LeftKeyAction();
                }
            }
        }

        /// <summary>
        /// Enters the key action.
        /// </summary>
        internal void EnterKeyAction()
        {
            if (!this.StaysOpenOnClick && this.Items.Count <= 0)
            {
                this.Parent.CloseAllPopUps();
                if (this.Parent.ExpandMode == ExpandModes.ExpandOnClick)
                {
                    this.Parent.firstClick = false;
                }
            }
            else
            {
                if (this.Items.Count > 0)
                {
                    CallVisualState(this, "SubMenuItemFocused");
                    this.IsSubMenuOpen = true;
                    foreach (var item in this.Items)
                    {
                        if (item is MenuItemAdv)
                            UpdateCanExecute(item as MenuItemAdv);
                    }
                }

                if (this.ParentMenuAdv != null)
                {
                    this.ParentMenuAdv.firstClick = true;
                    if (this.IsSubMenuOpen == true && this.Parent.Orientation == Orientation.Horizontal)
                    {
                        CallVisualState(this, "MenuItemSelected");
                    }
                    else
                    {
                        CallVisualState(this, "MenuItemFocused");
                    }
                }

                this.isSubMenuItemsPopupOpenedThroughEnterKey = true;

                foreach (var menuItem in this.ContainersToItems.Keys)
                {
                    if (menuItem is MenuItemAdv && (menuItem as MenuItemAdv).IsEnabled == true)
                    {
                        (menuItem as MenuItemAdv).Focus();
                        CallVisualState((menuItem as MenuItemAdv), "SubMenuItemFocused");
                        this.isSubMenuItemsPopupOpenedThroughEnterKey = false;
                        break;
                    }
                }
            }

            if (this.Items.Count == 0)
            {
#if WPF
                
                 if (ClickEvent != null)
                {
                    RaiseEvent(new RoutedEventArgs(ClickEvent));
                }
                

#else
                
                if (this.Click != null)
                {
                    this.Click(this, new RoutedEventArgs());
                }
               
#endif

                if (this.IsCheckable == true && this.CheckIconType != CheckIconType.None)
                {
                    if (this.CheckIconType == CheckIconType.CheckBox)
                    {
                        this.IsChecked = !this.IsChecked;
                    }
                    else
                    {
                        if (this.GroupName.Equals(string.Empty))
                            this.IsChecked = true;
                        else
                        {
                            this.IsChecked = true;
                            this.UnCheck(this);
                        }
                    }
                }

#if SILVERLIGHT
                if ((null != Command))
                {
                    if (Command.CanExecute(CommandParameter))
                    {
                        Command.Execute(CommandParameter);
                        this.IsEnabled = true;
                    }
                    else
                    {
                        this.IsEnabled = false;
                    }
                }
#else
            if (this.Command != null)
            {
                RoutedCommand command = Command as RoutedCommand;

                if (command != null)
                {
                    command.Execute(CommandParameter, CommandTarget);
                }
                else
                {
                    ((ICommand)Command).Execute(CommandParameter);
                }
            }
#endif
            }
        }

        /// <summary>
        /// Tabs the key action.
        /// </summary>
        internal void TabKeyAction()
        {
            if (this.ParentMenuAdv != null)
            {
                this.RightKeyAction();
            }
            else if (this.ParentMenuItemAdv != null)
            {
                this.DownKeyAction();
            }
        }

        /// <summary>
        /// Alts the key action.
        /// </summary>
        internal void AltKeyAction()
        {
            if (this.Parent.IsAltKeyPressed == false && this.Parent.FocusOnAlt)
            {
                foreach (var menuItem in this.Parent.ContainersToItems.Keys)
                {
                    if (menuItem is MenuItemAdv)
                    {
                      //  CallVisualState((menuItem as MenuItemAdv), "MenuItemFocused");

                        this.Parent.IsAltKeyPressed = true;
                        (menuItem as MenuItemAdv).Focus();
                        (menuItem as MenuItemAdv).canFocused = true;
                        break;
                    }
                }
            }
            else
            {
                this.Parent.IsAltKeyPressed = false;
                this.Parent.CloseAllPopUps();
            }
        }

        /// <summary>
        /// Escapes the key action.
        /// </summary>
        internal void EscapeKeyAction()
        {
            if (this.ParentMenuItemAdv != null)
            {
                CallVisualState(this, "Normal");
                this.ParentMenuItemAdv.IsSubMenuOpen = false;
                this.ParentMenuItemAdv.Focus();
                if ((this.ParentMenuItemAdv as MenuItemAdv).ParentMenuAdv!=null)
                    this.ParentMenuItemAdv.Parent.firstClick = false;
                CallVisualState(this.ParentMenuItemAdv, "SubMenuItemFocused");
            }
            else if (this.ParentMenuAdv != null && (!this.IsSubMenuOpen))
            {
                CallVisualState(this, "MenuItemFocused");
                this.Parent.firstClick = false;
                this.Parent.IsAltKeyPressed = false;
                //foreach (var menuItem in this.ContainersToItems.Keys)
                //{
                //    if (menuItem is MenuItemAdv)
                //    {
                //        (menuItem as MenuItemAdv).Focus();
                //        break;
                //    }
                //}
            }
            else if (this.ParentMenuAdv != null && (this.IsSubMenuOpen))
            {
                this.IsSubMenuOpen = false;
            }
        }

        /// <summary>
        /// Handles the popup open.
        /// </summary>
        internal void HandlePopupOpen()
        {
#if SILVERLIGHT
            if (this.SubMenuItemPopUp != null)
            {
                GeneralTransform gt = this.SubMenuItemPopUp.TransformToVisual(Application.Current.RootVisual as UIElement);
                Point offset = gt.Transform(new Point(0, 0));

                this.SubMenuItemPopUp.VerticalOffset = 0;
                this.SubMenuItemPopUp.HorizontalOffset = 0;

                double FromTop = offset.Y + this.SubMenuItemPopUp.VerticalOffset;
                double FromLeft = offset.X + this.SubMenuItemPopUp.HorizontalOffset;

                this.PopUpBorder.Projection = new System.Windows.Media.PlaneProjection();

                this.Parent.PanelHeight = this.PopUpGrid.ActualHeight;

                int itemCount = 0;

                foreach (var item in this.Items)
                {
                    if (item is MenuItemAdv)
                    {
                        itemCount++;
                    }
                }

                double browserWidth = (Application.Current.Host.Content.ActualWidth / (Application.Current.Host.Content.ZoomFactor * 100)) * 100;
                double browserHeight = (Application.Current.Host.Content.ActualHeight / (Application.Current.Host.Content.ZoomFactor * 100)) * 100;

                if (this.ParentMenuAdv != null && this.Parent.Orientation == Orientation.Horizontal)
                {
                    if ((FromTop + this.ActualHeight + (itemCount * this.ActualHeight)) > browserHeight)
                    {
                        this.PopUpGrid.Height = (itemCount * this.ActualHeight)+10;
                        if (this.Parent.IsScrollEnabled)
                        {
                            if (offset.Y >= (browserHeight - offset.Y))
                            {
                                if ((itemCount * this.ActualHeight) > offset.Y)
                                {
                                    this.PopUpGrid.Height = offset.Y - 25;
                                    this.SubMenuItemPopUp.VerticalOffset = -(offset.Y + 3);
                                    this.ScrollabilityEnabled = true;
                                }
                                else
                                {
                                    this.SubMenuItemPopUp.VerticalOffset = -(itemCount * this.ActualHeight) - ((this.Items.Count - itemCount) * 2) - 15;
                                }
                                this.IsBoundaryDetected = true;
                            }
                            else
                            {
                                this.PopUpGrid.Height = browserHeight - offset.Y - this.Parent.ActualHeight - 25;
                                this.SubMenuItemPopUp.VerticalOffset = this.ActualHeight;
                                this.ScrollabilityEnabled = true;
                            }
                        }
                        else
                        {
                            if (offset.Y >= (browserHeight - offset.Y))
                            {
                                this.SubMenuItemPopUp.VerticalOffset = -(itemCount * this.ActualHeight) - ((this.Items.Count - itemCount) * 2) - 15;
                                this.IsBoundaryDetected = true;
                            }
                            else
                            {
                                if (menuItemAdvGrid.ActualHeight < this.ActualHeight)
                                {
                                    SubMenuItemPopUp.VerticalOffset = menuItemAdvGrid.ActualHeight;
                                }
                                else
                                {
                                    SubMenuItemPopUp.VerticalOffset = this.ActualHeight;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (menuItemAdvGrid.ActualHeight < this.ActualHeight)
                        {
                            SubMenuItemPopUp.VerticalOffset = menuItemAdvGrid.ActualHeight;
                        }
                        else
                        {
                            SubMenuItemPopUp.VerticalOffset = this.ActualHeight;
                        }
                    }

                    if (FromLeft + this.PanelWidth > browserWidth)
                    {
                        this.SubMenuItemPopUp.HorizontalOffset = browserWidth - (FromLeft + this.PanelWidth);
                    }
                }
                else
                {
                    if ((FromTop + (itemCount * this.ActualHeight)) > browserHeight)
                    {
                        this.PopUpGrid.Height = (itemCount * this.ActualHeight);
                        this.SubMenuItemPopUp.VerticalOffset = this.SubMenuItemPopUp.VerticalOffset - ((FromTop + (itemCount * this.ActualHeight)) - browserHeight) - 10;
                    }

                    if (FromLeft + panel.ActualWidth + this.PanelWidth > browserWidth)
                    {
                        this.SubMenuItemPopUp.HorizontalOffset = -this.PanelWidth - 5;
                        this.IsBoundaryDetected = true;
                    }
                    else if (this.ParentMenuAdv != null && this.Parent.Orientation == Orientation.Vertical)
                    {
                        this.SubMenuItemPopUp.HorizontalOffset = this.Parent.ActualWidth - 2.5;
                    }
                    else
                    {
                        this.SubMenuItemPopUp.HorizontalOffset = this.ParentMenuItemAdv.PanelWidth;
                    }

                    if (this.Parent.IsScrollEnabled)
                    {
                        if (this.PopUpGrid.Height > browserHeight)
                        {
                            this.PopUpGrid.Height = browserHeight - 25;
                            this.SubMenuItemPopUp.VerticalOffset = -offset.Y;
                            this.Parent.PanelHeight = this.PopUpGrid.Height;
                            this.ScrollabilityEnabled = true;
                        }
                    }
                }
            }
#endif

#if WPF
            if(this.SubMenuItemPopUp != null)
            {
                TransformGroup Tg = new TransformGroup();
                ScaleTransform St = new ScaleTransform();
                TranslateTransform Tt = new TranslateTransform();
                Tg.Children.Add(St);
                Tg.Children.Add(Tt);
                this.PopUpBorder.RenderTransform = Tg;
                double ScreenWidth = System.Windows.SystemParameters.PrimaryScreenWidth;
                double ScreenHeight = System.Windows.SystemParameters.PrimaryScreenHeight;
                Point locationFromScreen = new Point();

                try
                {
                    locationFromScreen = this.PointToScreen(new Point(0, 0));
                }
                catch
                { }

                this.SubMenuItemPopUp.VerticalOffset = 0;
                this.SubMenuItemPopUp.HorizontalOffset = 0;
                if (this.ParentMenuAdv != null && this.Parent.Orientation == Orientation.Horizontal)
                {
                    this.SubMenuItemPopUp.PlacementTarget = this;
                    if (this.PopUpBorder.ActualHeight > 0 && (locationFromScreen.Y + this.ActualHeight + this.PopUpBorder.ActualHeight) > ScreenHeight)
                    {
                        if (locationFromScreen.Y > (ScreenHeight - this.ActualWidth - locationFromScreen.Y))
                        {
                            this.SubMenuItemPopUp.Placement = PlacementMode.Top;
                            IsBoundaryDetected = true;
                        }
                        else
                        {
                            this.SubMenuItemPopUp.Placement = PlacementMode.Bottom;
                            IsBoundaryDetected = false;
                        }
                    }
                    else
                    {
                        this.SubMenuItemPopUp.Placement = PlacementMode.Bottom;
                        IsBoundaryDetected = false;
                    }
                }
                else
                {
                    this.SubMenuItemPopUp.PlacementTarget = this;
                    if (this.PopUpBorder.ActualWidth > 0 && (locationFromScreen.X + this.ActualWidth + this.PopUpBorder.ActualWidth) > ScreenWidth)
                    {
                        this.SubMenuItemPopUp.Placement = PlacementMode.Left;
                        this.IsBoundaryDetected = true;
                    }
                    else
                    {
                        this.SubMenuItemPopUp.Placement = PlacementMode.Right;
                        this.IsBoundaryDetected = false;
                    }
                }
            }
#endif
        }

        /// <summary>
        /// Selects the pop up animation.
        /// </summary>
        internal void SelectPopUpAnimation()
        {
            if (this.Parent != null)
            {
                if (this.Parent.PopUpAnimationType == AnimationTypes.Fade)
                {
                    Storyboard FadeAnimation = new Storyboard();
                    DoubleAnimationUsingKeyFrames Animation = new DoubleAnimationUsingKeyFrames();
                    FadeAnimation.Children.Add(Animation);
                    Storyboard.SetTarget(Animation, this.PopUpBorder);
                    Storyboard.SetTargetProperty(Animation, new System.Windows.PropertyPath("(UIElement.Opacity)"));
                    SplineDoubleKeyFrame DoubleKeyFrame3 = new SplineDoubleKeyFrame();
                    DoubleKeyFrame3.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0));
                    DoubleKeyFrame3.Value = 0.5;
                    Animation.KeyFrames.Add(DoubleKeyFrame3);
                    SplineDoubleKeyFrame DoubleKeyFrame4 = new SplineDoubleKeyFrame();
                    DoubleKeyFrame4.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.4));
                    DoubleKeyFrame4.Value = 1;
                    Animation.KeyFrames.Add(DoubleKeyFrame4);
                    FadeAnimation.Begin();
                }
                else if (this.Parent.PopUpAnimationType == AnimationTypes.Scroll)
                {
                    if (this.ParentMenuAdv != null && this.Parent.Orientation == Orientation.Horizontal)
                    {
#if SILVERLIGHT
                        Storyboard ScrollVertical = new Storyboard();
                        PointAnimation pointAnimation = new PointAnimation();
                        ScrollVertical.Children.Add(pointAnimation);
                        Storyboard.SetTarget(pointAnimation, this.PopUpBorder);
                        Storyboard.SetTargetProperty(pointAnimation, new System.Windows.PropertyPath("(UIElement.RenderTransformOrigin)"));
                        if (this.IsBoundaryDetected)
                        {
                            if (this.PopUpBorder.RenderTransformOrigin.X != 0.5 || this.PopUpBorder.RenderTransformOrigin.Y != 1)
                                this.PopUpBorder.RenderTransformOrigin = new Point(0.5, 1);
                            pointAnimation.To = new Point(0.5, 1);
                        }
                        else
                        {
                            if (this.PopUpBorder.RenderTransformOrigin.X != 0.5 || this.PopUpBorder.RenderTransformOrigin.Y != 0)
                                this.PopUpBorder.RenderTransformOrigin = new Point(0.5, 0);
                            pointAnimation.To = new Point(0.5, 0);
                        }
                        DoubleAnimationUsingKeyFrames HeightAnimation = new DoubleAnimationUsingKeyFrames();
                        ScrollVertical.Children.Add(HeightAnimation);
                        Storyboard.SetTarget(HeightAnimation, this.PopUpBorder);
                        Storyboard.SetTargetProperty(HeightAnimation, new System.Windows.PropertyPath("(UIElement.RenderTransform).(CompositeTransform.ScaleY)"));
                        EasingDoubleKeyFrame DoubleKeyFrame1 = new EasingDoubleKeyFrame();
                        DoubleKeyFrame1.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0));
                        DoubleKeyFrame1.Value = 0;
                        HeightAnimation.KeyFrames.Add(DoubleKeyFrame1);
                        EasingDoubleKeyFrame DoubleKeyFrame2 = new EasingDoubleKeyFrame();
                        DoubleKeyFrame2.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.3));
                        DoubleKeyFrame2.Value = 1;
                        HeightAnimation.KeyFrames.Add(DoubleKeyFrame2);
                        ScrollVertical.Begin();
#endif

#if WPF
                        Storyboard ScrollVertical = new Storyboard();
                        DoubleAnimationUsingKeyFrames HeightAnimation = new DoubleAnimationUsingKeyFrames();
                        ScrollVertical.Children.Add(HeightAnimation);
                        Storyboard.SetTarget(HeightAnimation, this.PopUpBorder);
                        Storyboard.SetTargetProperty(HeightAnimation, new System.Windows.PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)"));
                        SplineDoubleKeyFrame DoubleKeyFrame1 = new SplineDoubleKeyFrame();
                        DoubleKeyFrame1.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0));
                        DoubleKeyFrame1.Value = 0;
                        HeightAnimation.KeyFrames.Add(DoubleKeyFrame1);
                        SplineDoubleKeyFrame DoubleKeyFrame2 = new SplineDoubleKeyFrame();
                        DoubleKeyFrame2.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.3));
                        DoubleKeyFrame2.Value = 1;
                        HeightAnimation.KeyFrames.Add(DoubleKeyFrame2);

                        PointAnimationUsingKeyFrames pointAnimation = new PointAnimationUsingKeyFrames();
                        ScrollVertical.Children.Add(pointAnimation);
                        Storyboard.SetTarget(pointAnimation, this.PopUpBorder);
                        Storyboard.SetTargetProperty(pointAnimation, new System.Windows.PropertyPath("(UIElement.RenderTransformOrigin)"));
                        SplinePointKeyFrame pointKey = new SplinePointKeyFrame();
                        if (!IsBoundaryDetected)
                        {
                            if (this.PopUpBorder.RenderTransformOrigin.X != 0.5 && this.PopUpBorder.RenderTransformOrigin.Y != 0)
                                this.PopUpBorder.RenderTransformOrigin = new Point(0.5, 0);
                            pointKey.Value = new Point(0.5, 0);
                        }
                        else
                        {
                            if (this.PopUpBorder.RenderTransformOrigin.X != 0.5 && this.PopUpBorder.RenderTransformOrigin.Y != 1)
                                this.PopUpBorder.RenderTransformOrigin = new Point(0.5, 1);
                            pointKey.Value = new Point(0.5, 1);
                        }
                        pointAnimation.KeyFrames.Add(pointKey);
                        ScrollVertical.Begin();
#endif
                    }
                    else
                    {
#if SILVERLIGHT
                        Storyboard ScrollHorizontal = new Storyboard();
                        PointAnimation pointAnimation = new PointAnimation();
                        ScrollHorizontal.Children.Add(pointAnimation);
                        Storyboard.SetTarget(pointAnimation, this.PopUpBorder);
                        Storyboard.SetTargetProperty(pointAnimation, new System.Windows.PropertyPath("(UIElement.RenderTransformOrigin)"));
                        if (this.IsBoundaryDetected)
                        {
                            if (this.PopUpBorder.RenderTransformOrigin.X != 1 || this.PopUpBorder.RenderTransformOrigin.Y != 0.5)
                                this.PopUpBorder.RenderTransformOrigin = new Point(1, 0.5);
                            pointAnimation.To = new Point(1, 0.5);
                        }
                        else
                        {
                            if (this.PopUpBorder.RenderTransformOrigin.X != 0 || this.PopUpBorder.RenderTransformOrigin.Y != 0.5)
                                this.PopUpBorder.RenderTransformOrigin = new Point(0, 0.5);
                            pointAnimation.To = new Point(0, 0.5);
                        }
                        DoubleAnimationUsingKeyFrames WidthAnimation = new DoubleAnimationUsingKeyFrames();
                        ScrollHorizontal.Children.Add(WidthAnimation);
                        Storyboard.SetTarget(WidthAnimation, this.PopUpBorder);
                        Storyboard.SetTargetProperty(WidthAnimation, new System.Windows.PropertyPath("(UIElement.RenderTransform).(CompositeTransform.ScaleX)"));
                        EasingDoubleKeyFrame DoubleKeyFrame1 = new EasingDoubleKeyFrame();
                        DoubleKeyFrame1.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0));
                        DoubleKeyFrame1.Value = 0;
                        WidthAnimation.KeyFrames.Add(DoubleKeyFrame1);
                        EasingDoubleKeyFrame DoubleKeyFrame2 = new EasingDoubleKeyFrame();
                        DoubleKeyFrame2.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.3));
                        DoubleKeyFrame2.Value = 1;
                        WidthAnimation.KeyFrames.Add(DoubleKeyFrame2);
                        ScrollHorizontal.Begin();
                        this.PopUpBorder.RenderTransformOrigin = new Point(0.5, 0.5);
#endif

#if WPF
                        Storyboard ScrollVertical = new Storyboard();
                        DoubleAnimationUsingKeyFrames HeightAnimation = new DoubleAnimationUsingKeyFrames();
                        ScrollVertical.Children.Add(HeightAnimation);
                        Storyboard.SetTarget(HeightAnimation, this.PopUpBorder);
                        Storyboard.SetTargetProperty(HeightAnimation, new System.Windows.PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleX)"));
                        SplineDoubleKeyFrame DoubleKeyFrame1 = new SplineDoubleKeyFrame();
                        DoubleKeyFrame1.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0));
                        DoubleKeyFrame1.Value = 0;
                        HeightAnimation.KeyFrames.Add(DoubleKeyFrame1);
                        SplineDoubleKeyFrame DoubleKeyFrame2 = new SplineDoubleKeyFrame();
                        DoubleKeyFrame2.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.3));
                        DoubleKeyFrame2.Value = 1;
                        HeightAnimation.KeyFrames.Add(DoubleKeyFrame2);

                        PointAnimationUsingKeyFrames pointAnimation = new PointAnimationUsingKeyFrames();
                        ScrollVertical.Children.Add(pointAnimation);
                        Storyboard.SetTarget(pointAnimation, this.PopUpBorder);
                        Storyboard.SetTargetProperty(pointAnimation, new System.Windows.PropertyPath("(UIElement.RenderTransformOrigin)"));
                        SplinePointKeyFrame pointKey = new SplinePointKeyFrame();
                        if (!IsBoundaryDetected)
                        {
                            if (this.PopUpBorder.RenderTransformOrigin.X != 0 || this.PopUpBorder.RenderTransformOrigin.Y != 0.5)
                                this.PopUpBorder.RenderTransformOrigin = new Point(0, 0.5);
                            pointKey.Value = new Point(0, 0.5);
                        }
                        else
                        {
                            if (this.PopUpBorder.RenderTransformOrigin.X != 1 || this.PopUpBorder.RenderTransformOrigin.Y != 0.5)
                                this.PopUpBorder.RenderTransformOrigin = new Point(1, 0.5);
                            pointKey.Value = new Point(1, 0.5);
                        }

                        pointAnimation.KeyFrames.Add(pointKey);
                        ScrollVertical.Begin();
#endif
                    }
                }
                else if (this.Parent.PopUpAnimationType == AnimationTypes.Slide)
                {
                    if (this.ParentMenuAdv != null & this.Parent.Orientation == Orientation.Horizontal)
                    {
#if SILVERLIGHT
                        Storyboard SlideVertical = new Storyboard();
                        ObjectAnimationUsingKeyFrames VisibilityAnimation = new ObjectAnimationUsingKeyFrames();
                        SlideVertical.Children.Add(VisibilityAnimation);
                        Storyboard.SetTarget(VisibilityAnimation, this.PopUpBorder);
                        Storyboard.SetTargetProperty(VisibilityAnimation, new System.Windows.PropertyPath("(UIElement.Visibility)"));
                        DiscreteObjectKeyFrame DoubleKeyFrame1 = new DiscreteObjectKeyFrame();
                        DoubleKeyFrame1.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0));
                        DoubleKeyFrame1.Value = Visibility.Collapsed;
                        VisibilityAnimation.KeyFrames.Add(DoubleKeyFrame1);
                        DiscreteObjectKeyFrame DoubleKeyFrame2 = new DiscreteObjectKeyFrame();
                        DoubleKeyFrame2.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.175));
                        DoubleKeyFrame2.Value = Visibility.Visible;
                        VisibilityAnimation.KeyFrames.Add(DoubleKeyFrame2);

                        DoubleAnimationUsingKeyFrames VerticalSlideAnimation = new DoubleAnimationUsingKeyFrames();
                        SlideVertical.Children.Add(VerticalSlideAnimation);
                        Storyboard.SetTarget(VerticalSlideAnimation, this.PopUpBorder);
                        Storyboard.SetTargetProperty(VerticalSlideAnimation, new System.Windows.PropertyPath("(UIElement.Projection).(PlaneProjection.LocalOffsetY)"));
                        EasingDoubleKeyFrame DoubleKeyFrame3 = new EasingDoubleKeyFrame();
                        DoubleKeyFrame3.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0));
                        if (this.IsBoundaryDetected)
                        {
                            DoubleKeyFrame3.Value = this.Parent.ActualHeight;
                        }
                        else
                        {
                            DoubleKeyFrame3.Value = -this.Parent.ActualHeight;
                        }
                        VerticalSlideAnimation.KeyFrames.Add(DoubleKeyFrame3);
                        EasingDoubleKeyFrame DoubleKeyFrame4 = new EasingDoubleKeyFrame();
                        DoubleKeyFrame4.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.25));
                        DoubleKeyFrame4.Value = 0;
                        VerticalSlideAnimation.KeyFrames.Add(DoubleKeyFrame4);
                        SlideVertical.Begin();
#else
                        Storyboard SlideVertical = new Storyboard();
                        DoubleAnimation VerticalSlideAnimation = new DoubleAnimation();
                        SlideVertical.Children.Add(VerticalSlideAnimation);
                        Storyboard.SetTarget(VerticalSlideAnimation, this.SubMenuItemPopUp);
                        Storyboard.SetTargetProperty(VerticalSlideAnimation, new System.Windows.PropertyPath("VerticalOffset"));
                        VerticalSlideAnimation.BeginTime = new TimeSpan(0, 0, 0);
                        VerticalSlideAnimation.Duration = new TimeSpan(0, 0, 0, 0, 100);
                        if (!IsBoundaryDetected)
                        {
                            VerticalSlideAnimation.From = -15;
                        }
                        else
                        {
                            VerticalSlideAnimation.From = 15;
                        }

                        VerticalSlideAnimation.To = 0;
                        SlideVertical.Begin();
#endif
                    }
                    else
                    {
#if SILVERLIGHT
                        Storyboard SlideHorizontal = new Storyboard();
                        ObjectAnimationUsingKeyFrames VisibilityAnimation = new ObjectAnimationUsingKeyFrames();
                        SlideHorizontal.Children.Add(VisibilityAnimation);
                        Storyboard.SetTarget(VisibilityAnimation, PopUpBorder);
                        Storyboard.SetTargetProperty(VisibilityAnimation, new System.Windows.PropertyPath("(UIElement.Visibility)"));
                        DiscreteObjectKeyFrame DoubleKeyFrame1 = new DiscreteObjectKeyFrame();
                        DoubleKeyFrame1.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0));
                        DoubleKeyFrame1.Value = Visibility.Collapsed;
                        VisibilityAnimation.KeyFrames.Add(DoubleKeyFrame1);
                        DiscreteObjectKeyFrame DoubleKeyFrame2 = new DiscreteObjectKeyFrame();
                        DoubleKeyFrame2.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.18));
                        DoubleKeyFrame2.Value = Visibility.Visible;
                        VisibilityAnimation.KeyFrames.Add(DoubleKeyFrame2);

                        DoubleAnimationUsingKeyFrames HorizontalSlideAnimation = new DoubleAnimationUsingKeyFrames();
                        SlideHorizontal.Children.Add(HorizontalSlideAnimation);
                        Storyboard.SetTarget(HorizontalSlideAnimation, this.PopUpBorder);
                        Storyboard.SetTargetProperty(HorizontalSlideAnimation, new System.Windows.PropertyPath("(UIElement.Projection).(PlaneProjection.LocalOffsetX)"));
                        EasingDoubleKeyFrame DoubleKeyFrame3 = new EasingDoubleKeyFrame();
                        DoubleKeyFrame3.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0));
                        if (this.ParentMenuAdv != null)
                        {
                            if (this.IsBoundaryDetected)
                            {
                                DoubleKeyFrame3.Value = this.Parent.PanelWidth;
                            }
                            else
                            {
                                DoubleKeyFrame3.Value = -this.Parent.PanelWidth;
                            }
                        }
                        else
                        {
                            if (this.IsBoundaryDetected)
                            {
                                DoubleKeyFrame3.Value = this.ParentMenuItemAdv.PanelWidth;
                            }
                            else
                            {
                                DoubleKeyFrame3.Value = -this.ParentMenuItemAdv.PanelWidth;
                            }
                        }
                        HorizontalSlideAnimation.KeyFrames.Add(DoubleKeyFrame3);
                        EasingDoubleKeyFrame DoubleKeyFrame4 = new EasingDoubleKeyFrame();
                        DoubleKeyFrame4.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.25));
                        DoubleKeyFrame4.Value = 0;
                        HorizontalSlideAnimation.KeyFrames.Add(DoubleKeyFrame4);
                        SlideHorizontal.Begin();
#else
                        Storyboard SlideHorizontal = new Storyboard();
                        DoubleAnimation HorizontalSlideAnimation = new DoubleAnimation();
                        SlideHorizontal.Children.Add(HorizontalSlideAnimation);
                        Storyboard.SetTarget(HorizontalSlideAnimation, this.SubMenuItemPopUp);
                        Storyboard.SetTargetProperty(HorizontalSlideAnimation, new System.Windows.PropertyPath("HorizontalOffset"));
                        HorizontalSlideAnimation.BeginTime = new TimeSpan(0, 0, 0);
                        HorizontalSlideAnimation.Duration = new TimeSpan(0, 0, 0, 0, 100);
                        if (!IsBoundaryDetected)
                        {
                            HorizontalSlideAnimation.From = -20;
                        }
                        else
                        {
                            HorizontalSlideAnimation.From = 20;
                        }

                        HorizontalSlideAnimation.To = 0;
                        SlideHorizontal.Begin();
#endif
                    }
                }
            }
        }

        /// <summary>
        /// Changes the extend button visibility.
        /// </summary>
        internal void ChangeExtendButtonVisibility()
        {
            if (this.ParentMenuItemAdv != null && this.Items.Count > 0)
            {
                ExtendButtonVisibility = Visibility.Visible;
            }
            else
            {
                ExtendButtonVisibility = Visibility.Collapsed;
            }

            if (this.ParentMenuAdv != null)
            {
                if (this.ParentMenuAdv.Orientation == Orientation.Vertical && this.Items.Count > 0)
                {
                    ExtendButtonVisibility = Visibility.Visible;
                }
            }
        }

        /// <summary>
        /// Checks the name of the group.
        /// </summary>
        /// <param name="menuitem">The menuitem.</param>
        internal void CheckGroupName(MenuItemAdv menuitem)
        {
            for (int i = 0; i < this.Items.Count; i++)
            {
                if (this.Items[i] is MenuItemAdv && this.Items[i] == menuitem)
                {
                    MenuItemAdv _item = (MenuItemAdv)this.Items[i];
                    if (_item.IsCheckable == true && _item.CheckIconType == CheckIconType.RadioButton && !_item.GroupName.Equals(string.Empty))
                    {
                        for (int j = i - 1; j >= 0; j--)
                        {
                            if (this.Items[j] is MenuItemAdv)
                            {
                                MenuItemAdv _previouseitem = this.Items[j] as MenuItemAdv;
                                if (_previouseitem.IsCheckable == true && _previouseitem.CheckIconType == CheckIconType.RadioButton && !_previouseitem.GroupName.Equals(string.Empty) && _item.GroupName.Equals(_previouseitem.GroupName) && _previouseitem.IsChecked)
                                {
                                    _item.IsChecked = false;
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Uns the check.
        /// </summary>
        /// <param name="menuitem">The menuitem.</param>
        private void UnCheck(MenuItemAdv menuitem)
        {
            if (this.ParentMenuItemAdv != null)
            {
                for (int i = 0; i < this.ParentMenuItemAdv.Items.Count; i++)
                {
                    if (this.ParentMenuItemAdv.Items[i] is MenuItemAdv)
                    {
                        MenuItemAdv _item = (MenuItemAdv)this.ParentMenuItemAdv.Items[i];
                        if (_item is MenuItemAdv && _item != menuitem && !_item.GroupName.Equals(string.Empty))
                        {
                            if (_item.IsCheckable == true && _item.CheckIconType == CheckIconType.RadioButton && _item.GroupName.Equals(menuitem.GroupName))
                            {
                                _item.IsChecked = false;
                            }
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < this.ParentMenuItemAdv.Items.Count; i++)
                {
                    if (this.ParentMenuItemAdv.Items[i] is MenuItemAdv)
                    {
                        MenuItemAdv _item = (MenuItemAdv)this.ParentMenuItemAdv.Items[i];
                        if (_item is MenuItemAdv && _item != menuitem && !_item.GroupName.Equals(string.Empty))
                        {
                            if (_item.IsCheckable == true && _item.CheckIconType == CheckIconType.RadioButton)
                            {
                                _item.IsChecked = false;
                            }
                        }
                    }
                }
            }
        }

        internal StackPanel GetStackPanel()
        {
            DependencyObject element = this;
            while (!(element is StackPanel) && element != null)
            {
                element = VisualTreeHelper.GetParent(element);
            }

            if (element != null)
            {
                return element as StackPanel;
            }

            return null;
        }

        # endregion

        #region ICommandSource Members

        /// <summary>
        /// Gets or sets the command.
        /// </summary>
        /// <value>The command.</value>
        [Category("Common Properties")]
        [Description("Represents the Command of the MenuItem")]
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        /// <summary>
        /// Gets or sets the command parameter.
        /// </summary>
        /// <value>The command parameter.</value>
        [Category("Common Properties")]
        [Description("Represents the Command Paramenter which has to be passed with the Command to execute it.")]
        public object CommandParameter
        {
            get { return (object)GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

#if WPF

        /// <summary>
        /// The object that the command is being executed on.
        /// </summary>
        /// <value></value>
        /// <returns>The object that the command is being executed on.</returns>
        public IInputElement CommandTarget
        {
            get { return (IInputElement)GetValue(CommandTargetProperty); }
            set { SetValue(CommandTargetProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Command. It Represents the Command to be excecuted.
        /// </summary>
        public static readonly DependencyProperty CommandProperty = ButtonBase.CommandProperty.AddOwner(typeof(MenuItemAdv), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(MenuItemAdv.CommandChanged)));

        /// <summary>
        /// Using a DependencyProperty as the backing store for Command Parameter. It Represents the Command Parameter to be passed to execute the command.
        /// </summary>
        public static readonly DependencyProperty CommandParameterProperty = ButtonBase.CommandParameterProperty.AddOwner(typeof(MenuItemAdv), new FrameworkPropertyMetadata(null));

        public static readonly DependencyProperty CommandTargetProperty = ButtonBase.CommandTargetProperty.AddOwner(typeof(MenuItemAdv), new FrameworkPropertyMetadata(null));
#else
        /// <summary>
        /// Using a DependencyProperty as the backing store for Command. It Represents the Command to be excecuted.
        /// </summary>
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(MenuItemAdv), new PropertyMetadata((ICommand)null, new PropertyChangedCallback(CommandChanged)));

        /// <summary>
        /// Using a DependencyProperty as the backing store for Command Parameter. It Represents the Command Parameter to be passed to execute the command.
        /// </summary>
        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register("CommandParameter", typeof(object), typeof(MenuItemAdv), new PropertyMetadata(null));
#endif

        #endregion

        #region ICommand Source Implementation

        private EventHandler CanExecuteChangedHandler;

        //public event EventHandler MyEvent
        //{
        //    add { lock (this) { CanExecuteChangedHandler += value; } }
        //    remove { lock (this) { CanExecuteChangedHandler -= value; } }
        //}

        private static void CommandChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            MenuItemAdv menuItem = (MenuItemAdv)obj;
            menuItem.OnCommandChanged((ICommand)e.OldValue, (ICommand)e.NewValue);
        }

        private void OnCommandChanged(ICommand oldCommand, ICommand newCommand)
        {
            if (oldCommand != null)
            {
                this.UnhookCommand(oldCommand);
            }
            if (newCommand != null)
            {
                this.HookCommand(newCommand);
            }
        }

        private void UnhookCommand(ICommand command)
        {
            EventHandler handler = CanExecuteChangedHandler;
            if (handler != null)
            {
                command.CanExecuteChanged -= handler;
            }
            this.UpdateCanExecute();
        }

        private void HookCommand(ICommand command)
        {
            EventHandler handler = new EventHandler(this.OnCanExecuteChanged);
            CanExecuteChangedHandler = handler;
            command.CanExecuteChanged += handler;
            this.UpdateCanExecute();
        }

        private void OnCanExecuteChanged(object sender, EventArgs e)
        {
            this.UpdateCanExecute();
        }

        private void UpdateCanExecute()
        {
#if WPF
            if (this.Command != null)
            {
                MenuItemAdv item = ItemsControl.ItemsControlFromItemContainer(this) as MenuItemAdv;
                if ((item == null) || item.IsSubMenuOpen)
                {
                    if (!CanExecuteCommandSource(this))
                    {
                        VisualStateManager.GoToState(this, "Disabled", false);
                        this.IsEnabled = false;
                    }
                    else
                    {
                        
                        //VisualStateManager.GoToState(this, "Normal", false);
                        this.IsEnabled = true;
                    }
                }
                //else
                //{
                //    System.Diagnostics.Debug.WriteLine("First Norma");
                //    VisualStateManager.GoToState(this, "Normal", false);
                //}
            }
            //else
            //{
            //    VisualStateManager.GoToState(this, "Normal", false);
            //}
#endif
        }

        private void UpdateCanExecute(MenuItemAdv menu)
        {
#if WPF
            if (menu.Command != null)
            {
                MenuItemAdv item = ItemsControl.ItemsControlFromItemContainer(menu) as MenuItemAdv;
                if ((item == null) || item.IsSubMenuOpen)
                {
                    if (!CanExecuteCommandSource(menu))
                    {
                        VisualStateManager.GoToState(menu, "Disabled", false);
                        menu.IsEnabled = false;
                    }
                    else
                    {
                        VisualStateManager.GoToState(menu, "Normal", false);
                        menu.IsEnabled = true;
                    }
                }
                else
                {
                    VisualStateManager.GoToState(menu, "Normal", false);
                }
            }
            else
            {
                VisualStateManager.GoToState(menu, "Normal", false);
            }
#endif
        }

#if WPF
        private static bool CanExecuteCommandSource(MenuItemAdv commandSource)
        {
            ICommand command = commandSource.Command;
            if (command == null)
            {
                return false;
            }
            object commandParameter = commandSource.CommandParameter;
            IInputElement commandTarget = commandSource.CommandTarget;
            RoutedCommand command2 = command as RoutedCommand;
            if (command2 == null)
            {
                return command.CanExecute(commandParameter);
            }
            if (commandTarget == null)
            {
                commandTarget = commandSource as IInputElement;
            }
            return command2.CanExecute(commandParameter, commandTarget);
        }
#endif
        #endregion
    }

#if WPF
    /// <summary>
    /// MenuItem Check Icon Type
    /// </summary>
    public enum CheckIconType
    {
        /// <summary>
        /// Check Box can be placed in MenuItem Icon.
        /// </summary>
        CheckBox,

        /// <summary>
        /// Radio Button can be placed in MenuItem Icon.
        /// </summary>
        RadioButton,

        ///<summary>
        ///None type
        ///</summary>
        None
    }
#endif
}
