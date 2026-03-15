#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Collections.Specialized;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.IO.IsolatedStorage;
using System.IO;
using System.Diagnostics;
using System.Windows.Markup;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Serialization;
using System.Xml;
using System.Text;
using System.Windows.Data;
using Syncfusion.Silverlight.Shared;
using System.ComponentModel;
using System.Windows.Resources;


namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents TabControlAdv class.
    /// </summary>
    [TemplateVisualState(Name = "TopPlacement", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "LeftPlacement", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "RightPlacement", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "BottomPlacement", GroupName = "CommonStates")]

    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Blend,
    Type = typeof(TabControlAdv), XamlResource = "/Syncfusion.Theming.Blend;component/TabControlAdv.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Blue,
        Type = typeof(TabControlAdv), XamlResource = "/Syncfusion.Theming.Office2007Blue;component/TabControlAdv.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Black,
        Type = typeof(TabControlAdv), XamlResource = "/Syncfusion.Theming.Office2007Black;component/TabControlAdv.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Silver,
        Type = typeof(TabControlAdv), XamlResource = "/Syncfusion.Theming.Office2007Silver;component/TabControlAdv.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Blue,
        Type = typeof(TabControlAdv), XamlResource = "/Syncfusion.Theming.Office2010Blue;component/TabControlAdv.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Black,
        Type = typeof(TabControlAdv), XamlResource = "/Syncfusion.Theming.Office2010Black;component/TabControlAdv.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Silver,
        Type = typeof(TabControlAdv), XamlResource = "/Syncfusion.Theming.Office2010Silver;component/TabControlAdv.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Default,
        Type = typeof(TabControlAdv), XamlResource = "/Syncfusion.Theming.Default;component/TabControlAdv.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2003,
        Type = typeof(TabControlAdv), XamlResource = "/Syncfusion.Theming.Office2003;component/TabControlAdv.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Windows7,
       Type = typeof(TabControlAdv), XamlResource = "/Syncfusion.Theming.Windows7;component/TabControlAdv.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.VS2010,
    Type = typeof(TabControlAdv), XamlResource = "/Syncfusion.Theming.VS2010;component/TabControlAdv.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Metro,
   Type = typeof(TabControlAdv), XamlResource = "/Syncfusion.Theming.Metro;component/TabControlAdv.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Transparent ,
Type = typeof(TabControlAdv), XamlResource = "/Syncfusion.Theming.Transparent;component/TabControlAdv.xaml")]

    public class TabControlAdv : ItemsControl
    {
        #region Class constants
        private TabItemAdv lastContainerCheck;
        /// <summary>
        /// Presents file name for saving in the internal isolated storage.
        /// </summary>
        private readonly string mStoreFileName = AppDomain.CurrentDomain.FriendlyName + ".dat";
        #endregion

        #region Private members
        /// <summary>
        /// Tab panel used for layout of tab layout panel, scrolling buttons, menu buttons and close buttons.
        /// </summary>
        private TabPanelAdv tabPanel;

        /// <summary>
        /// Panel used for layout of tab items.
        /// </summary>
        private TabLayoutPanel tabLayoutPanel;

        /// <summary>
        /// Content panel.
        /// </summary>
        private Border contentPanel;

        /// <summary>
        /// Panel used for containing header panel and content panel.
        /// </summary>
        private TabControlPanel tabControlPanel;

        private Border contentpanelpresenter = null;
        #endregion

        #region Initialization
        /// <summary>
        /// Initializes new instance of the TabControlAdv class.
        /// </summary>            
        public TabControlAdv()
        {
            DefaultStyleKey = typeof(TabControlAdv);
            this.TabHeaders = new TabHeaderCollection();
            this.Loaded += new RoutedEventHandler(TabControlAdv_Loaded);
            //this.ItemContGenerator = new ItemContainerGeneratorAdv(this);
        }

        void TabControlAdv_Loaded(object sender, RoutedEventArgs e)
        {

        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a collection used to generate the content of the <see cref="T:System.Windows.Controls.ItemsControl"/>.
        /// </summary>
        /// <value></value>
        /// <returns>The  object that is used to generate the content of the <see cref="T:System.Windows.Controls.ItemsControl"/>. The default is null.</returns>
        public new System.Collections.IEnumerable ItemsSource
        {
            get { return base.ItemsSource; }
            set
            {
                base.ItemsSource = value;
                EventArgs e = new EventArgs();
                OnItemsSourceChanged(e);
            }
        }
        
        /// <summary>
        /// Gets or sets the menu item style.
        /// </summary>
        /// <value>The menu item style.</value>
        [Description("Used to set MenuItemStyle")]
        [Category("Tab Control Properties")]
        public Style MenuItemStyle
        {
            get { return (Style)GetValue(MenuItemStyleProperty); }
            set { SetValue(MenuItemStyleProperty, value); }
        }

        /// <summary>
        /// Gets or sets the tab panel background.
        /// </summary>
        /// <value>The tab panel background.</value>
        [Description("To set TabPanel Background")]
        [Category("Tab Control Properties")]
        public Brush TabPanelBackground
        {
            get { return (Brush)GetValue(TabPanelBackgroundProperty); }
            set { SetValue(TabPanelBackgroundProperty, value); }
        }


        /// <summary>
        /// Gets or sets the left item navigation key.
        /// </summary>
        /// <value>The left item navigation key.</value>
        [Description("Used to set Hot Key for Left Item Navigation")]
        [Category("Tab Control Properties")]
        public Key LeftItemNavigationKey
        {
            get
            {
                return (Key)GetValue(LeftItemNavigationKeyProperty);
            }
            set
            {
                SetValue(LeftItemNavigationKeyProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the right item navigation key.
        /// </summary>
        /// <value>The right item navigation key.</value>
        [Description("Used to set Hot Key for Right Item Navigation")]
        [Category("Tab Control Properties")]
        public Key RightItemNavigationKey
        {
            get
            {
                return (Key)GetValue(RightItemNavigationKeyProperty);
            }
            set
            {
                SetValue(RightItemNavigationKeyProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the first item navigation key.
        /// </summary>
        /// <value>The first item navigation key.</value>
        [Description("Used to set Hot Key for First Item Navigation")]
        [Category("Tab Control Properties")]
        public Key FirstItemNavigationKey
        {
            get
            {
                return (Key)GetValue(FirstItemNavigationKeyProperty);
            }
            set
            {
                SetValue(FirstItemNavigationKeyProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the last item navigation key.
        /// </summary>
        /// <value>The last item navigation key.</value>
        [Description("Used to set Hot Key for Last Item Navigation")]
        [Category("Tab Control Properties")]
        public Key LastItemNavigationKey
        {
            get
            {
                return (Key)GetValue(LastItemNavigationKeyProperty);
            }
            set
            {
                SetValue(LastItemNavigationKeyProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the item container style.
        /// </summary>
        /// <value>The item container style.</value>
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



        /// <summary>
        /// Gets or sets the tab layout panel.
        /// </summary>
        /// <value>The tab layout panel.</value>
        internal TabLayoutPanel TabLayoutPanel
        {
            get
            {
                return this.tabLayoutPanel;
            }

            set
            {
                this.tabLayoutPanel = value;
            }
        }

        /// <summary>
        /// Gets the tab panel.
        /// </summary>
        /// <value>The tab panel.</value>
        internal TabPanelAdv TabPanel
        {
            get
            {
                return this.tabPanel;
            }
            //set
            //{
            //    this.tabPanel = value;
            //}
        }

        /// <summary>
        /// Gets or sets the content template.
        /// </summary>
        /// <value>The content template.</value>
        [Description("To define the Template of Tab Item Content")]
        [Category("Tab Control Properties")]
        public DataTemplate ContentTemplate
        {
            get
            {
                return (DataTemplate)GetValue(ContentTemplateProperty);
            }

            set
            {
                SetValue(ContentTemplateProperty, value);
            }
        }


        /// <summary>
        /// Gets or sets the Tab Item Open Mode.
        /// </summary>
        /// <value>The Item Open Mode</value>
        [Description("Used to set Tab Item Open Mode")]
        [Category("Tab Control Properties")]
        public TabItemOpenMode TabItemOpenMode
        {
            get
            {
                return (TabItemOpenMode)GetValue(TabItemOpenModeProperty);
            }
            set
            {
                SetValue(TabItemOpenModeProperty, value);
            }
        }


        #endregion

        #region DP getters and setters
        /// <summary>
        /// Gets or sets the tab item style.
        /// </summary>
        [Description("Used to set Tab Item Style")]
        [Category("Tab Control Properties")]
        [Obsolete("Property will not help due to internal arhitecture changes")]
        public Style TabItemStyle
        {
            get
            {
                return (Style)GetValue(TabItemStyleProperty);
            }

            set
            {
                SetValue(TabItemStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the tab headers.
        /// </summary>
        /// <value>The tab headers.</value>
        internal TabHeaderCollection TabHeaders
        {
            get
            {
                return (TabHeaderCollection)GetValue(TabHeadersProperty);
            }

            set
            {
                SetValue(TabHeadersProperty, value);
            }
        }

        /// <summary>
        /// Gets the content of the TabControlAdv.
        /// </summary>
        public object SelectedContent
        {
            get
            {
                return GetValue(SelectedContentProperty);
            }

            internal set
            {
                SetValue(SelectedContentProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the selected index.
        /// </summary>
        /// <value>The index of the selected.</value>
        public int SelectedIndex
        {
            get
            {
                return (int)GetValue(SelectedIndexProperty);
            }

            set
            {
                SetValue(SelectedIndexProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the selected item.
        /// </summary>
        /// <value>The selected item.</value>
        [CLSCompliant(false)]
        public TabItemAdv SelectedItem
        {
            get
            {
                return (TabItemAdv)GetValue(SelectedItemProperty);
            }

            set
            {
                SetValue(SelectedItemProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets value indicating how tab headers align relative to the tab content.
        /// </summary>
        /// <value>The tab strip placement.</value>
        [Description("To customize the Tab Control Placement")]
        [Category("Tab Control Properties")]
        public TabStripPlacement TabStripPlacement
        {
            get
            {
                return (TabStripPlacement)GetValue(TabStripPlacementProperty);
            }

            set
            {
                SetValue(TabStripPlacementProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets tabs scrolling time in milliseconds.
        /// </summary>
        /// <value>The scrolling time.</value>
        [Description("Used to create animation while scrolling (in Milli seconds)")]
        [Category("Tab Control Properties")]
        public int ScrollingTime
        {
            get
            {
                return (int)GetValue(ScrollingTimeProperty);
            }

            set
            {
                SetValue(ScrollingTimeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether all tabs are closed.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is all tabs closed; otherwise, <c>false</c>.
        /// </value>
        public bool IsAllTabsClosed
        {
            get
            {
                return (bool)GetValue(IsAllTabsClosedProperty);
            }

            protected internal set
            {
                SetValue(IsAllTabsClosedProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether label editing is enabled.
        /// </summary>
        /// <value><c>true</c> if [enable label edit]; otherwise, <c>false</c>.</value>
        [Description("To enable Edit in Tab Item Header Label ")]
        [Category("Tab Control Properties")]
        public bool EnableLabelEdit
        {
            get
            {
                return (bool)GetValue(EnableLabelEditProperty);
            }

            set
            {
                SetValue(EnableLabelEditProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to show tab list context menu.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [show tab list context menu]; otherwise, <c>false</c>.
        /// </value>
        [Description("To Show or Hide TabList Context Menu")]
        [Category("Tab Control Properties")]
        public bool ShowTabListContextMenu
        {
            get
            {
                return (bool)GetValue(ShowTabListContextMenuProperty);
            }

            set
            {
                SetValue(ShowTabListContextMenuProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value of the CloseButtonType dependency property.
        /// </summary>
        /// <value>The type of the close button.</value>
        [Description("To define the Close Button Type")]
        [Category("Tab Control Properties")]
        public CloseButtonType CloseButtonType
        {
            get
            {
                return (CloseButtonType)GetValue(CloseButtonTypeProperty);
            }

            set
            {
                SetValue(CloseButtonTypeProperty, value);
            }
        }

        

        /// <summary>
        /// Gets or sets a value indicating whether hot tracking is enabled.
        /// </summary>
        /// <value><c>true</c> if [hot tracking enabled]; otherwise, <c>false</c>.</value>
        public bool HotTrackingEnabled
        {
            get
            {
                return (bool)GetValue(HotTrackingEnabledProperty);
            }

            set
            {
                SetValue(HotTrackingEnabledProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to rotate text when tab placement is left or right.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [rotate text when vertical]; otherwise, <c>false</c>.
        /// </value>
        [Description("To Enable or Disable Text Rotation While Verical Placements")]
        [Category("Tab Control Properties")]
        public bool RotateTextWhenVertical
        {
            get
            {
                return (bool)GetValue(RotateTextWhenVerticalProperty);
            }

            set
            {
                SetValue(RotateTextWhenVerticalProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value of the TabPanelStyle dependency property.
        /// </summary>
        /// <value>The tab panel style.</value>
        [Description("To set TabPanel Style")]
        [Category("Tab Control Properties")]
        public Style TabPanelStyle
        {
            get
            {
                return (Style)GetValue(TabPanelStyleProperty);
            }

            set
            {
                SetValue(TabPanelStyleProperty, value);
            }
        }



        /// <summary>
        /// Gets or sets the value of the TabScrollButtonVisibility dependency property.
        /// </summary>
        /// <value>The tab scroll button visibility.</value>
        [Description("To define Tab Scroll Button Visibility")]
        [Category("Tab Control Properties")]
        public TabScrollButtonVisibility TabScrollButtonVisibility
        {
            get
            {
                return (TabScrollButtonVisibility)GetValue(TabScrollButtonVisibilityProperty);
            }

            set
            {
                SetValue(TabScrollButtonVisibilityProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value of the TabScrollStyle dependency property.
        /// </summary>
        /// <value>The tab scroll style.</value>
        [Description("To set Tab Scroll Button Style")]
        [Category("Tab Control Properties")]
        public TabScrollStyle TabScrollStyle
        {
            get
            {
                return (TabScrollStyle)GetValue(TabScrollStyleProperty);
            }

            set
            {
                SetValue(TabScrollStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value of the TabItemLayout dependency property.
        /// </summary>
        /// <value>The tab item layout.</value>
        public TabItemLayoutType TabItemLayout
        {
            get
            {
                return (TabItemLayoutType)GetValue(TabItemLayoutProperty);
            }

            set
            {
                SetValue(TabItemLayoutProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value of the TabItemSize dependency property.
        /// </summary>
        /// <value>The tab item size mode.</value>
        [Description("Set Tab Item Size Mode")]
        [Category("Tab Control Properties")]
        public TabItemSizeMode TabItemSizeMode
        {
            get
            {
                return (TabItemSizeMode)GetValue(TabItemSizeModeProperty);
            }

            set
            {
                SetValue(TabItemSizeModeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to keep tab in first row.
        /// </summary>
        /// <value><c>true</c> if [keep tab in front]; otherwise, <c>false</c>.</value>
        public bool KeepTabInFront
        {
            get
            {
                return (bool)GetValue(KeepTabInFrontProperty);
            }

            set
            {
                SetValue(KeepTabInFrontProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value of the SelectedItemFontWeight dependency property.
        /// </summary>
        /// <value>The selected item font weight.</value>
        public FontWeight SelectedItemFontWeight
        {
            get
            {
                return (FontWeight)GetValue(SelectedItemFontWeightProperty);
            }

            set
            {
                SetValue(SelectedItemFontWeightProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether drag and drop is allowed.
        /// </summary>
        /// <value><c>true</c> if [allow drag drop]; otherwise, <c>false</c>.</value>
        [Description("To Enable or Disable Drag and Drop")]
        [Category("Tab Control Properties")]
        public bool AllowDragDrop
        {
            get
            {
                return (bool)GetValue(AllowDragDropProperty);
            }

            set
            {
                SetValue(AllowDragDropProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a corner radius of the tab control border.
        /// </summary>
        /// <value>The corner radius.</value>
        public  CornerRadius CornerRadius
        {
            get
            {
                return (CornerRadius)GetValue(CornerRadiusProperty);
            }

            set
            {
                SetValue(CornerRadiusProperty, value);
            }
        }
        #endregion

        #region Events
        /// <summary>
        /// Event that is raised when <see cref="TabItemStyle"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback TabItemStyleChanged;



        /// <summary>
        /// Event that is raised when <see cref="SelectedContent"/> property is changed.
        /// </summary>
        internal event PropertyChangedCallback SelectedContentChanged;

        /// <summary>
        /// Event that is raised when <see cref="SelectedIndex"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback SelectedIndexChanged;

        /// <summary>
        /// Event that is raised when <see cref="SelectedItem"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback SelectedItemChanged;

        /// <summary>
        /// Event that is raised when <see cref="TabStripPlacement"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback TabStripPlacementChanged;

        /// <summary>
        /// Event that is raised when <see cref="ScrollingTime"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback ScrollingTimeChanged;

        /// <summary>
        /// Event that is raised when <see cref="IsAllTabsClosed"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback IsAllTabsClosedChanged;

        /// <summary>
        /// Event that is raised when <see cref="EnableLabelEdit"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback EnableLabelEditChanged;

        /// <summary>
        /// Event that is raised when <see cref="ShowTabListContextMenu"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback ShowTabListContextMenuChanged;

        /// <summary>
        /// Event that is raised when <see cref="CloseButtonType"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback CloseButtonTypeChanged;

        /// <summary>
        /// Event that is raised when <see cref="HotTrackingEnabled"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback HotTrackingEnabledChanged;

        /// <summary>
        /// Event that is raised when <see cref="RotateTextWhenVertical"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback RotateTextWhenVerticalChanged;

        /// <summary>
        /// Event that is raised when <see cref="TabPanelStyle"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback TabPanelStyleChanged;

        /// <summary>
        /// Event that is raised when <see cref="TabScrollButtonVisibility"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback TabScrollButtonVisibilityChanged;

        /// <summary>
        /// Event that is raised when <see cref="TabScrollStyle"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback TabScrollStyleChanged;

        /// <summary>
        /// Event that is raised when <see cref="TabItemLayout"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback TabItemLayoutChanged;

        /// <summary>
        /// Event that is raised when <see cref="TabItemSizeMode"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback TabItemSizeModeChanged;

        /// <summary>
        /// Event that is raised when <see cref="KeepTabInFront"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback KeepTabInFrontChanged;

        /// <summary>
        /// Event that is raised when <see cref="SelectedItemFontWeight"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback SelectedItemFontWeightChanged;

        /// <summary>
        /// Occurs when tab is closed.
        /// </summary>
        public event OnCloseTabsEventHandler TabClosing;

        /// <summary>
        /// Occurs before the label editing.
        /// </summary>
        public event BeforeLabelEditHandler BeforeLabelEdit;

        /// <summary>
        /// Occurs when after the label editing.
        /// </summary>
        public event AfterLabelEditHandler AfterLabelEdit;

        /// <summary>
        /// Occurs when drop down context menu is opened.
        /// </summary>
        public event EventHandler DropDownContextMenuOpen;

        /// <summary>
        /// Occurs when drop down context menu is closed.
        /// </summary>
        public event EventHandler DropDownContextMenuClose;

        /// <summary>
        /// Occurs when drag is started.
        /// </summary>
        public event EventHandler DragStart;

        /// <summary>
        /// Occurs when drag is ended.
        /// </summary>
        public event EventHandler DragEnd;

        /// <summary>
        /// Event that is raised when <see cref="AllowDragDrop"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback AllowDragDropChanged;

        /// <summary>
        /// 
        /// </summary>
        public event PropertyChangedCallback TabItemOpenModeChanged;
        #endregion

        #region Dependency properties

        /// <summary>
        /// Identifies the <see cref="ContentTemplate"/> dependency property.
        /// </summary>
        internal static readonly DependencyProperty ContentTemplateProperty =
           DependencyProperty.Register("ContentTemplate", typeof(DataTemplate), typeof(TabControlAdv), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="MenuItemStyle"/> dependency property.
        /// </summary>
        internal static readonly DependencyProperty MenuItemStyleProperty = DependencyProperty.Register(
             "MenuItemStyle",
             typeof(Style),
             typeof(TabControlAdv),
             new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="TabPanelBackground"/> dependency property.
        /// </summary>
        internal static readonly DependencyProperty TabPanelBackgroundProperty = DependencyProperty.Register(
           "TabPanelBackground",
           typeof(Brush),
           typeof(TabControlAdv),
           new PropertyMetadata(null));

        //private static void MenuItemStyleChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    ((TabPopupMenu)d).UpdateItems();
        //}

        /// <summary>
        /// Identifies the <see cref="ItemContainerStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemContainerStyleProperty = DependencyProperty.Register("ItemContainerStyle", typeof(Style), typeof(TabControlAdv), new PropertyMetadata(null, OnItemContainerStyleChanged));
        /// <summary>
        /// Identifies the <see cref="TabItemStyle"/> dependency property.
        /// </summary>
        internal static readonly DependencyProperty TabItemStyleProperty =
            DependencyProperty.Register("TabItemStyle", typeof(Style), typeof(TabControlAdv), new PropertyMetadata(new PropertyChangedCallback(OnTabItemStyleChanged)));

        /// <summary>
        /// Identifies the <see cref="TabHeaders"/> dependency property.
        /// </summary>
        internal static readonly DependencyProperty TabHeadersProperty =
            DependencyProperty.Register("TabHeaders", typeof(TabHeaderCollection), typeof(TabControlAdv), new PropertyMetadata(null));


        /// <summary>
        /// Identifies the <see cref="SelectedContent"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedContentProperty =
            DependencyProperty.Register("SelectedContent", typeof(object), typeof(TabControlAdv), new PropertyMetadata(new PropertyChangedCallback(OnSelectedContentChanged)));

        /// <summary>
        /// Identifies the <see cref="SelectedIndex"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedIndexProperty =
            DependencyProperty.Register("SelectedIndex", typeof(int), typeof(TabControlAdv), new PropertyMetadata(new PropertyChangedCallback(OnSelectedIndexChanged)));

        /// <summary>
        /// Identifies the <see cref="SelectedItem"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(TabItemAdv), typeof(TabControlAdv), new PropertyMetadata(new PropertyChangedCallback(OnSelectedItemChanged)));

        /// <summary>
        /// Identifies the <see cref="TabStripPlacement"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TabStripPlacementProperty =
            DependencyProperty.Register("TabStripPlacement", typeof(TabStripPlacement), typeof(TabControlAdv), new PropertyMetadata(new PropertyChangedCallback(OnTabStripPlacementChanged)));

        /// <summary>
        /// Identifies the <see cref="ScrollingTime"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ScrollingTimeProperty =
            DependencyProperty.Register("ScrollingTime", typeof(int), typeof(TabControlAdv), new PropertyMetadata(100, new PropertyChangedCallback(OnScrollingTimeChanged)));

        /// <summary>
        /// Identifies the <see cref="IsAllTabsClosed"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsAllTabsClosedProperty =
            DependencyProperty.Register("IsAllTabsClosed", typeof(bool), typeof(TabControlAdv), new PropertyMetadata(new PropertyChangedCallback(OnIsAllTabsClosedChanged)));

        /// <summary>
        /// Identifies the <see cref="EnableLabelEdit"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty EnableLabelEditProperty =
            DependencyProperty.Register("EnableLabelEdit", typeof(bool), typeof(TabControlAdv), new PropertyMetadata(new PropertyChangedCallback(OnEnableLabelEditChanged)));

        /// <summary>
        /// Identifies the <see cref="ShowTabListContextMenu"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ShowTabListContextMenuProperty =
            DependencyProperty.Register("ShowTabListContextMenu", typeof(bool), typeof(TabControlAdv), new PropertyMetadata(true,new PropertyChangedCallback(OnShowTabListContextMenuChanged)));

        /// <summary>
        /// Identifies the <see cref="CloseButtonType"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CloseButtonTypeProperty =
            DependencyProperty.Register("CloseButtonType", typeof(CloseButtonType), typeof(TabControlAdv), new PropertyMetadata(new PropertyChangedCallback(OnCloseButtonTypeChanged)));

        /// <summary>
        /// Identifies the <see cref="LeftItemNavigationKey"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LeftItemNavigationKeyProperty =
            DependencyProperty.Register("LeftItemNavigationKey", typeof(Key), typeof(TabControlAdv), new PropertyMetadata(Key.Left));

        /// <summary>
        /// Identifies the <see cref="RightItemNavigationKey"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RightItemNavigationKeyProperty =
            DependencyProperty.Register("RightItemNavigationKey", typeof(Key), typeof(TabControlAdv), new PropertyMetadata(Key.Right));

        /// <summary>
        /// Identifies the <see cref="FirstItemNavigationKey"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FirstItemNavigationKeyProperty =
            DependencyProperty.Register("FirstItemNavigationKey", typeof(Key), typeof(TabControlAdv), new PropertyMetadata(Key.Home));

        /// <summary>
        /// Identifies the <see cref="LastItemNavigationKey"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LastItemNavigationKeyProperty =
            DependencyProperty.Register("LastItemNavigationKey", typeof(Key), typeof(TabControlAdv), new PropertyMetadata(Key.End));


        /// <summary>
        /// Identifies the <see cref="HotTrackingEnabled"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HotTrackingEnabledProperty =
            DependencyProperty.Register("HotTrackingEnabled", typeof(bool), typeof(TabControlAdv), new PropertyMetadata(new PropertyChangedCallback(OnHotTrackingEnabledChanged)));

        /// <summary>
        /// Identifies the <see cref="RotateTextWhenVertical"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RotateTextWhenVerticalProperty =
            DependencyProperty.Register("RotateTextWhenVertical", typeof(bool), typeof(TabControlAdv), new PropertyMetadata(new PropertyChangedCallback(OnRotateTextWhenVerticalChanged)));

        /// <summary>
        /// Identifies the <see cref="TabPanelStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TabPanelStyleProperty =
            DependencyProperty.Register("TabPanelStyle", typeof(Style), typeof(TabControlAdv), new PropertyMetadata(new PropertyChangedCallback(OnTabPanelStyleChanged)));

        /// <summary>
        /// Identifies the <see cref="TabScrollButtonVisibility"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TabScrollButtonVisibilityProperty =
            DependencyProperty.Register("TabScrollButtonVisibility", typeof(TabScrollButtonVisibility), typeof(TabControlAdv), new PropertyMetadata(new PropertyChangedCallback(OnTabScrollButtonVisibilityChanged)));

        /// <summary>
        /// Identifies the <see cref="TabScrollStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TabScrollStyleProperty =
            DependencyProperty.Register("TabScrollStyle", typeof(TabScrollStyle), typeof(TabControlAdv), new PropertyMetadata(new PropertyChangedCallback(OnTabScrollStyleChanged)));

        /// <summary>
        /// Identifies the <see cref="TabItemLayout"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TabItemLayoutProperty =
            DependencyProperty.Register("TabItemLayout", typeof(TabItemLayoutType), typeof(TabControlAdv), new PropertyMetadata(new PropertyChangedCallback(OnTabItemLayoutChanged)));

        /// <summary>
        /// Identifies the <see cref="TabItemSizeMode"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TabItemSizeModeProperty =
            DependencyProperty.Register("TabItemSizeMode", typeof(TabItemSizeMode), typeof(TabControlAdv), new PropertyMetadata(new PropertyChangedCallback(OnTabItemSizeModeChanged)));

        /// <summary>
        /// Identifies the <see cref="KeepTabInFront"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty KeepTabInFrontProperty =
            DependencyProperty.Register("KeepTabInFront", typeof(bool), typeof(TabControlAdv), new PropertyMetadata(true, new PropertyChangedCallback(OnKeepTabInFrontChanged)));

        /// <summary>
        /// Identifies the <see cref="SelectedItemFontWeight"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedItemFontWeightProperty =
            DependencyProperty.Register("SelectedItemFontWeight", typeof(FontWeight), typeof(TabControlAdv), new PropertyMetadata(new PropertyChangedCallback(OnSelectedItemFontWeightChanged)));

        /// <summary>
        /// Identifies the <see cref="AllowDragDrop"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AllowDragDropProperty =
            DependencyProperty.Register("AllowDragDrop", typeof(bool), typeof(TabControlAdv), new PropertyMetadata(true, new PropertyChangedCallback(OnAllowDragDropChanged)));

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty TabItemOpenModeProperty =
            DependencyProperty.Register("TabItemOpenMode", typeof(TabItemOpenMode), typeof(TabControlAdv), new PropertyMetadata(null));


        /// <summary>
        /// Identifies the <see cref="CornerRadius"/> dependency property.
        /// </summary>
        internal static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(TabControlAdv), new PropertyMetadata(new CornerRadius(3)));



        /// <summary>
        /// 
        /// </summary>
        public TabVisualStyle TabVisualStyle
        {
            get { return (TabVisualStyle)GetValue(TabVisualStyleProperty); }
            set { SetValue(TabVisualStyleProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for TabVisualStyle.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TabVisualStyleProperty =
            DependencyProperty.Register("TabVisualStyle", typeof(TabVisualStyle), typeof(TabControlAdv), new PropertyMetadata(TabVisualStyle.None,new PropertyChangedCallback(OnTabVisualStyleChanged)));

        

        #endregion

        #region Overrides
        /// <summary>
        /// Called when [item container style changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnItemContainerStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabControlAdv source = d as TabControlAdv;
            Style value = e.NewValue as Style;
            // source.ItemContGenerator.UpdateItemContainerStyle(value);
        }


        /// <summary>
        /// Invoked whenever application code or internal processes (such as a rebuilding layout pass)
        /// call System.Windows.Controls.Control.ApplyTemplate() method.
        /// </summary>

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            bool isSelected = false;


            if (tabLayoutPanel != null)
            {
                tabLayoutPanel.Children.Clear();
            }
            ItemsPresenter itemspresenter = (ItemsPresenter)GetTemplateChild("ItemsPresenter");
            this.tabControlPanel = this.GetTemplateChild("TabControlPanel") as TabControlPanel;
            this.tabPanel = this.GetTemplateChild("TabPanel") as TabPanelAdv;
            this.contentPanel = this.GetTemplateChild("ContentPanel") as Border;
            this.TabLayoutPanel = this.GetTemplateChild("TabLayoutPanel") as TabLayoutPanel;
            this.contentpanelpresenter = this.GetTemplateChild("ContentPanel5") as Border;
            if (tabLayoutPanel != null)
            {
                this.tabLayoutPanel.TabControlParent = this;
                this.tabLayoutPanel.TabPanelParent = this.tabPanel;
                if (this.TabHeaders != null)
                {
                    this.TabHeaders.Parent = tabLayoutPanel;
                    this.TabHeaders.UpdatePanelChildren();
                }
            }

            if (this.TabPanel != null)
                this.TabPanel.TabControlParent = this;


            if (this.tabControlPanel != null)
            {
                this.tabControlPanel.TabStripPlacement = this.TabStripPlacement;
                this.tabControlPanel.TabLayoutPanel = tabLayoutPanel;
            }


            if (this.Items.Count > 0)
            {
                if (this.tabLayoutPanel != null)
                    TabLayoutPanel.Children.Clear();
                foreach (object item in this.Items)
                {
                    TabItemAdv tabItem = item as TabItemAdv;
                    if (this.tabLayoutPanel != null && tabItem != null && !this.tabLayoutPanel.Children.Contains(tabItem))
                    {
                        tabItem.TabControlParent = this;
                        this.tabLayoutPanel.Children.Add(tabItem);
                        tabItem.OnApplyTemplate();
                        if (tabItem.IsSelected)
                        {
                            isSelected = true;
                            this.SelectedIndex = this.GetIndexOfTabItemAdv(tabItem);
                        }
                    }
                }

                if (!isSelected)
                {
                    TabItemAdv firstItem = this.Items[0] as TabItemAdv;
                    if (firstItem != null)
                    {
                        firstItem.IsSelected = true;
                        this.SelectedIndex = 0;
                    }
                }
            }

            if (this.SelectedIndex >= 0 && this.SelectedIndex < this.Items.Count)
            {
                if (this.Items[SelectedIndex] is TabItemAdv)
                    this.SelectedItem = this.Items[this.SelectedIndex] as TabItemAdv;
            }

            if (this.tabLayoutPanel != null)
            {
                if ((this.TabStripPlacement == TabStripPlacement.Left) || (this.TabStripPlacement == TabStripPlacement.Right))
                {
                    if (this.ActualHeight < this.TabLayoutPanel.ActualWidth)
                    {
                        if (this.TabScrollStyle == TabScrollStyle.Extended)
                        {
                            this.tabLayoutPanel.MeasureElements(new Size(this.ActualHeight - 114, this.tabLayoutPanel.ActualHeight));
                            this.tabLayoutPanel.ArrangeElements(new Size(this.ActualHeight - 114, this.tabLayoutPanel.ActualHeight));
                        }
                        else
                        {
                            this.tabLayoutPanel.MeasureElements(new Size(this.ActualHeight - 58, this.tabLayoutPanel.ActualHeight));
                            this.tabLayoutPanel.ArrangeElements(new Size(this.ActualHeight - 58, this.tabLayoutPanel.ActualHeight));
                        }
                    }
                }
                else
                {
                    if (this.ActualWidth < this.TabLayoutPanel.ActualWidth)
                    {
                        if (this.TabScrollStyle == TabScrollStyle.Extended)
                        {
                            this.tabLayoutPanel.MeasureElements(new Size(this.ActualWidth - 114, this.tabLayoutPanel.ActualHeight));
                            this.tabLayoutPanel.ArrangeElements(new Size(this.ActualWidth - 114, this.tabLayoutPanel.ActualHeight));
                        }
                        else
                        {
                            this.tabLayoutPanel.MeasureElements(new Size(this.ActualWidth - 58, this.tabLayoutPanel.ActualHeight));
                            this.tabLayoutPanel.ArrangeElements(new Size(this.ActualWidth - 58, this.tabLayoutPanel.ActualHeight));
                        }
                    }
                }

                this.tabLayoutPanel.SelectItemInternal();
            }

            this.UpdateSelectedContent();
            this.UpdateCloseButtonsVisibility();
            this.UpdateCornerRadius();
            this.UpdateVisualState();
            this.VerifyZIndex();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [CLSCompliant(false)]
        public int GetIndexOfTabItemAdv(TabItemAdv item)
        {
            if (this.Items.Contains(item))
                for (int i = 0; i < this.Items.Count; i++)
                {
                    TabItemAdv tabItem = this.Items[i] as TabItemAdv;
                    if (tabItem == item)
                        return i;
                }
            return -1;
        }

        /// <summary>
        /// Called before the System.Windows.UIElement.KeyDown event occurs.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.OriginalSource is TabControlAdv)
            {
                if (!e.Handled)
                {
                    TabItemAdv item = null;
                    int direction = 0;
                    int startIndex = -1;
                    bool isHandled = false;

                    if (e.Key == this.LastItemNavigationKey)
                    {
                        direction = -1;
                        startIndex = this.Items.Count;
                        isHandled = true;
                    }
                    else if (e.Key == this.FirstItemNavigationKey)
                    {
                        direction = 1;
                        startIndex = -1;
                        isHandled = true;
                    }

                    else if (e.Key == this.LeftItemNavigationKey)
                    {
                        direction = -1;
                        startIndex = this.SelectedIndex;
                        isHandled = true;
                    }
                    else if (e.Key == this.RightItemNavigationKey)
                    {
                        direction = 1;
                        startIndex = this.SelectedIndex;
                        isHandled = true;
                    }
                    else if (e.Key == Key.F2)
                    {
                        if (this.EnableLabelEdit && this.TabLayoutPanel != null && this.SelectedItem != null)
                        {
                            this.TabLayoutPanel.LabelEditStartInternal(this.SelectedItem);
                        }
                    }

                    if (isHandled)
                    {
                        item = this.FindNextTabItem(startIndex, direction);
                        if ((item != null) && (item != this.SelectedItem))
                        {
                            e.Handled = true;
                            this.SelectedItem = item;
                            this.SelectedIndex = this.GetIndexOfTabItemAdv(item);
                            item.Focus();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Invoked when the System.Windows.Controls.ItemsControl.Items property changes.
        /// </summary>
        /// <param name="e">Information about the change.</param>
        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);


            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                if (this.tabPanel != null)
                {
                    this.tabPanel.Visibility = Visibility.Visible;
                    UpdateCloseButtonsVisibility();
                    this.tabPanel.MenuButton.Visibility = Visibility.Visible;
                    IsAllTabsClosed = false;
                }

                for (int i = 0; i < e.NewItems.Count; i++)
                {
                    TabItemAdv tabItem = e.NewItems[i] as TabItemAdv;
                    if (tabItem != null)
                    {
                        // Tab tab = new Tab();
                        //tab.TabItemParent = tabItem;
                        //tab.TabControlParent = this;
                        tabItem.IsSelected = tabItem.IsSelected;
                        //tabItem.Image = tabItem.Image;
                        // tabItem.Tab = tab;
                        this.TabHeaders.Insert(e.NewStartingIndex, tabItem);
                        tabItem.TabControlParent = this;
                    }
                    if (this.SelectedItem != null)
                    {
                        this.SelectedItem = this.Items[e.NewItems.IndexOf(tabItem)] as TabItemAdv;
                    }
                    if (this.contentpanelpresenter != null)
                    {
                        ContentPresenter contentpresenter = this.contentpanelpresenter.Child as ContentPresenter;
                        if (contentpresenter != null)
                        {
                            contentpresenter.Visibility = Visibility.Visible;
                        }
                    }
                    else if(this.contentPanel!=null && this.contentPanel.Child!=null)
                    {
                        ContentPresenter contentpresenter = this.contentPanel.Child as ContentPresenter;
                        if (contentpresenter != null)
                        {
                            contentpresenter.Visibility = Visibility.Visible;
                        }
                    }
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                int index = 0;
                for (int i = 0; i < e.OldItems.Count; i++)
                {
                    TabItemAdv tabItem = e.OldItems[i] as TabItemAdv;
                    index = e.OldItems.IndexOf(tabItem);
                    if (tabItem != null)
                    {
                        this.TabHeaders.Remove(tabItem);
                    }
                    if (this.Items.Count > 0)
                    {
                        this.SelectedItem = this.Items[index] as TabItemAdv;
                    }
                    if (this.Items.Count == 0)
                    {
                        if (this.contentPanel != null && this.contentPanel.Child != null)
                        {
                            ContentPresenter contentpresenter = this.contentPanel.Child as ContentPresenter;
                            if (contentpresenter != null)
                            {
                                contentpresenter.Visibility = Visibility.Collapsed;
                            }
                        }
                    }
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Replace)
            {
                for (int i = 0; i < e.OldItems.Count; i++)
                {
                    TabItemAdv tabItem = e.OldItems[i] as TabItemAdv;
                    if (tabItem != null)
                    {
                        this.TabHeaders.Remove(tabItem);
                    }
                }

                if (e.NewItems != null)
                {
                    for (int i = 0; i < e.NewItems.Count; i++)
                    {
                        TabItemAdv tabItem = e.NewItems[i] as TabItemAdv;
                        if (tabItem != null)
                        {
                            // Tab tab = new Tab();
                            //tab.TabItemParent = tabItem;
                            //tab.TabControlParent = this;
                            tabItem.IsSelected = tabItem.IsSelected;
                            // tabItem.Image = tabItem.Image;
                            // tabItem.Tab = tab;
                            this.TabHeaders.Insert(e.NewStartingIndex, tabItem);
                            tabItem.TabControlParent = this;
                        }
                    }
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                this.TabHeaders.Clear();
                if (this.contentPanel != null)
                if (this.contentPanel.Child != null)
                {
                    ContentPresenter contentpresenter = this.contentPanel.Child as ContentPresenter;
                    if (contentpresenter != null)
                    {
                        contentpresenter.Visibility = Visibility.Collapsed;
                    }
                }

                if (this.ItemsSource != null && this.Items.Count > 0)
                    this.OnItemsSourceChanged(new EventArgs());
            }

            if (this.SelectedItem != null)
            {
                this.SelectedIndex = this.ItemContainerGenerator.IndexFromContainer(this.SelectedItem);
            }

            this.VerifyZIndex();           
        }

        /// <summary>
        /// Prepares the specified element to display the specified item.
        /// </summary>
        /// <param name="element">The element used to display the specified item.</param>
        /// <param name="item">The item to display.</param>
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            //   ItemContGenerator.ApplyPropertiesTochild(element, item, ItemContainerStyle);
            base.PrepareContainerForItemOverride(element, item);
            if (element is TabItemAdv)
            {
                TabItemAdv tabItem = element as TabItemAdv;
                //Commented the below line since TabItemStyle is Obsolute
               //tabItem.Style = this.TabItemStyle;
                if (this.ItemTemplate != null)
                {
                    // tabItem.HeaderTemplate = this.ItemTemplate;
                    Binding binding = new Binding();
                    binding.Source = this;
                    binding.Path = new PropertyPath("ItemTemplate");
                    tabItem.SetBinding(Syncfusion.Windows.Controls.HeaderedContentControl.HeaderTemplateProperty, binding);
                }

                if (this.ItemsSource != null)
                {
                    if (tabItem != null)
                    {
                        //  Tab tab = new Tab();
                        // tab.TabItemParent = tabItem;
                        // tab.TabControlParent = this;
                        tabItem.IsSelected = tabItem.IsSelected;
                        //  tab.Image = tabItem.Image;
                        //  tabItem.Tab = tab;
                        this.TabHeaders.Insert(this.TabHeaders.Count, tabItem);
                        tabItem.TabControlParent = this;
                    }
                    tabItem.Header = item;

                }

                if (tabItem.TabContentPresenter != null)
                {
                    if (tabItem.IsSelected)
                    {
                        tabItem.TabContentPresenter.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        tabItem.TabContentPresenter.Visibility = Visibility.Collapsed;
                    }
                }
            }
        }

        #endregion

        #region Implementation
        /// <summary>
        /// Update visual state of the tabControl.
        /// </summary>
        internal void UpdateVisualState()
        {
            switch (this.TabStripPlacement)
            {
                case TabStripPlacement.Top:
                    VisualStateManager.GoToState(this, "TopPlacement", true);
                    break;

                case TabStripPlacement.Left:
                    VisualStateManager.GoToState(this, "LeftPlacement", true);
                    break;

                case TabStripPlacement.Right:
                    VisualStateManager.GoToState(this, "RightPlacement", true);
                    break;

                case TabStripPlacement.Bottom:
                    VisualStateManager.GoToState(this, "BottomPlacement", true);
                    break;
            }
        }

        /// <summary>
        /// Saves the state persisted for the current <see cref="TabControlAdv"/> location.
        /// </summary>
        public void SaveTabState()
        {
            IsolatedStorageFile isoStorage = IsolatedStorageFile.GetUserStoreForApplication();
            this.SaveTabState(isoStorage, mStoreFileName);
        }

        /// <summary>
        /// Loads the state persisted.
        /// </summary>
        public void LoadTabState()
        {
            IsolatedStorageFile isoStorage = IsolatedStorageFile.GetUserStoreForApplication();
            LoadTabStateFromIsoStorage(isoStorage, mStoreFileName);
        }

        /// <summary>
        /// Saves the state persisted for the current <see cref="T:Syncfusion.Windows.Tools.Controls.TabControlExt"/> location.
        /// </summary>
        /// <param name="isoStorage">Reference in isolated storage for saving the current <see cref="T:Syncfusion.Windows.Tools.Controls.TabControlExt"/> location.</param>
        /// <param name="storeFileName">File name for the isolated storage.</param>
        private void SaveTabState(IsolatedStorageFile isoStorage, string storeFileName)
        {
            if (null != isoStorage && !String.IsNullOrEmpty(storeFileName))
            {
                using (Stream stream = new IsolatedStorageFileStream(storeFileName, FileMode.Create, isoStorage))
                {
                    TabControlParams settings = new TabControlParams(this);
                    XmlSerializer xs = new XmlSerializer(typeof(TabControlParams));
                    xs.Serialize(stream, settings);
                }
            }
        }

        /// <summary>
        /// Loads the state persisted.
        /// </summary>
        /// <param name="isoStorage">Reference in isolated storage for
        /// load current TabControl location.</param>
        /// <param name="storeFileName">Present file name for isolated
        /// storage.</param>
        private void LoadTabStateFromIsoStorage(IsolatedStorageFile isoStorage, string storeFileName)
        {
            if (null != isoStorage && !String.IsNullOrEmpty(storeFileName)
                && 0 < isoStorage.GetFileNames(storeFileName).Length)
            {
                TabControlParams settings = null;
                string str = string.Empty;
                using (Stream stream = new IsolatedStorageFileStream(storeFileName, FileMode.OpenOrCreate, isoStorage))
                {
                    XmlSerializer xs = new XmlSerializer(typeof(TabControlParams));
                    settings = xs.Deserialize(stream) as TabControlParams;
                }

                this.ApplyState(settings);
            }
        }

        /// <summary>
        /// Applies the tab control state.
        /// </summary>
        /// <param name="settings">The settings.</param>
        private void ApplyState(TabControlParams settings)
        {
            List<ItemInfo> tabItems = settings.Items;
            if (TabStripPlacement != settings.TabStripPlacement)
            {
                TabStripPlacement = settings.TabStripPlacement;

            }

            List<TabItemAdv> changedItems = new List<TabItemAdv>();

            foreach (ItemInfo info in tabItems)
            {
                for (int i = 0; i < this.Items.Count; i++)
                {
                    TabItemAdv item = this.Items[i] as TabItemAdv;
                    if (item != null && info.Name == item.Name)
                    {
                        item.Header = item.Header != info.Header ? info.Header : item.Header;
                        item.Visibility = item.Visibility != info.Visibility ? info.Visibility : item.Visibility;
                        //if (item.Tab != null)
                        //{
                        //    item.Tab.Visibility = item.Visibility;
                        //}

                        changedItems.Add(item);

                        break;
                    }
                }
            }

            if (changedItems.Count > 0)
            {
                foreach (TabItemAdv item in changedItems)
                {
                    foreach (ItemInfo info in tabItems)
                    {
                        if (info.Name == item.Name)
                        {
                            this.SelectedContent = null;
                            this.Items.Remove(item);
                            int index = info.Index;
                            if (index > this.Items.Count)
                            {
                                index = this.Items.Count;
                            }

                            if (index < 0)
                            {
                                index = 0;
                            }

                            this.Items.Insert(index, item);
                            this.UpdateSelectedContent();
                            this.SelectedIndex = this.ItemContainerGenerator.IndexFromContainer(this.SelectedItem);
                            break;
                        }
                    }
                }
            }
        }



        /// <summary>
        /// Verifies Z-order of the tab items.
        /// </summary>
        protected internal void VerifyZIndex()
        {
            int count = this.TabHeaders.Count - 1;
            int firstTabIndex;

            for (firstTabIndex = 0; firstTabIndex <= count; firstTabIndex++)
            {
                TabItemAdv element = this.TabHeaders[firstTabIndex] as TabItemAdv;
                if (element.Visibility == Visibility.Visible)
                {
                    break;
                }
            }

            if (TabStripPlacement == TabStripPlacement.Top || TabStripPlacement == TabStripPlacement.Right)
            {
                for (int i = 0; i <= count; i++)
                {
                    TabItemAdv element = this.TabHeaders[i] as TabItemAdv;
                    if (element != null)
                    {
                        if (i == SelectedIndex)
                        {
                            Canvas.SetZIndex(element, 10000);
                        }
                        else
                        {
                            Canvas.SetZIndex(element, (count - i));
                        }

                        if (i == firstTabIndex)
                        {
                            Canvas.SetZIndex(element, 9999);
                        }
                    }
                }
            }
            else
            {
                int counter = 0;
                for (int i = count; i >= 0; i--)
                {
                    TabItemAdv element = this.TabHeaders[i] as TabItemAdv;
                    if (TabVisualStyle == TabVisualStyle.None)
                    {
                        if (element != null)
                        {
                            if (i == SelectedIndex)
                            {
                                Canvas.SetZIndex(element, 10000);
                            }
                            else
                            {
                                Canvas.SetZIndex(element, i);
                            }

                            if (i == count && (element.Visibility == Visibility.Visible))
                            {
                                Canvas.SetZIndex(element, 9999);
                            }
                        }
                    }

                    else
                    {
                        if (element != null)
                        {
                            if (i == SelectedIndex)
                            {
                                Canvas.SetZIndex(element, 10000+count);
                            }
                            else
                            {
                                Canvas.SetZIndex(element, 10000+counter);
                            }

                            if (i == count && (element.Visibility == Visibility.Visible)&&i!=SelectedIndex)
                            {
                                Canvas.SetZIndex(element, 9999);
                            }
                        }
                        counter++;
                    }
                    
                }
            }
        }

        /// <summary>
        /// Closes the selected tab item.
        /// </summary>
        internal void CloseSelectedTabItem()
        {
            if (this.SelectedItem != null)
            {
                //this.SelectedItem.Tab.Visibility = Visibility.Collapsed;
                this.SelectedItem.Visibility = Visibility.Collapsed;
                int itemIndex = this.SelectedIndex;
                if (itemIndex < this.Items.Count + 1)
                {
                    this.IsAllTabsClosed = false;

                    for (int i = itemIndex + 1; i < this.Items.Count; i++)
                    {
                        TabItemAdv item = this.Items[i] as TabItemAdv;
                        if (item != null && item != null && item.Visibility == Visibility.Visible)
                        {
                            this.SelectedItem = this.Items[i] as TabItemAdv;
                            this.SelectedIndex = i;
                            break;
                        }
                    }

                    if (itemIndex == this.SelectedIndex)
                    {
                        for (int i = itemIndex - 1; i >= 0; i--)
                        {
                            TabItemAdv item = this.Items[i] as TabItemAdv;
                            if (item != null && item != null && item.Visibility == Visibility.Visible)
                            {
                                this.SelectedItem = this.Items[i] as TabItemAdv;
                                this.SelectedIndex = i;
                                break;
                            }
                        }
                    }

                    if (itemIndex == this.SelectedIndex)
                    {

                        this.SelectedIndex = -1;
                        this.IsAllTabsClosed = true;
                    }
                }

                this.VerifyZIndex();
            }
            if (this.IsAllTabsClosed)
            {
                //this.tabControlPanel.Visibility = Visibility.Collapsed;
                //this.tabPanel.Visibility = Visibility.Collapsed;
                this.tabPanel.CloseButton.Visibility = Visibility.Collapsed;
                this.tabPanel.MenuButton.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Updates close buttons visibility.
        /// </summary>
        internal void UpdateCloseButtonsVisibility()
        {
            for (int i = 0; i < this.Items.Count; i++)
            {
                TabItemAdv item = this.Items[i] as TabItemAdv;
                if (item != null)
                {
                    item.UpdateCloseButtonVisibility();
                }
            }

            if (this.tabPanel != null && this.tabPanel.CloseButton != null)
            {
                switch (this.CloseButtonType)
                {
                    case CloseButtonType.Common:
                        this.tabPanel.CloseButton.Visibility = Visibility.Visible;                        
                        break;
                    case CloseButtonType.Both:
                        this.tabPanel.CloseButton.Visibility = Visibility.Visible;                       
                        break;
                    case CloseButtonType.Individual:
                        this.tabPanel.CloseButton.Visibility = Visibility.Collapsed;
                        break;
                    case CloseButtonType.IndividualOnMouseOver:
                        this.tabPanel.CloseButton.Visibility = Visibility.Collapsed;
                        for (int i = 0; i < this.Items.Count; i++)
                        {
                            TabItemAdv item = this.Items[i] as TabItemAdv;
                            if (item != null && item.CloseButton != null)
                            {
                                item.CloseButton.HideButton();
                            }
                        }

                        break;
                    case CloseButtonType.Hide:
                        this.tabPanel.CloseButton.Visibility = Visibility.Collapsed;
                        break;
                }
            }
            if (this.tabPanel != null && this.tabPanel.MenuPanel != null)
            {
                if (this.ShowTabListContextMenu)
                {
                    this.tabPanel.MenuPanel.Visibility = Visibility.Visible;
                }
                else
                {
                    this.tabPanel.MenuPanel.Visibility = Visibility.Collapsed;
                }
            }

            this.InvalidateTabs();
        }

        /// <summary>
        /// Fires the tab closing.
        /// </summary>
        /// <param name="e">The tabitem.</param>
        protected internal virtual void FireTabClosing(CloseTabEventArgs e)
        {
            if (this.TabClosing != null)
            {
                this.TabClosing(this, e);
            }
        }

        /// <summary>
        /// Fires the before label edit.
        /// </summary>
        /// <param name="headerBeforeLabelEdit">The header before label edit.</param>
        protected internal virtual void FireBeforeLabelEdit(object headerBeforeLabelEdit)
        {
            if (this.BeforeLabelEdit != null)
            {
                this.BeforeLabelEdit(this, new BeforeLabelEditEventArgs(headerBeforeLabelEdit));
            }
        }

        /// <summary>
        /// Fires the after label edit.
        /// </summary>
        /// <param name="headerAfterLabelEdit">The header after label edit.</param>
        protected internal virtual void FireAfterLabelEdit(object headerAfterLabelEdit)
        {
            if (this.AfterLabelEdit != null)
            {
                this.AfterLabelEdit(this, new AfterLabelEditEventArgs(headerAfterLabelEdit));
            }
        }

        /// <summary>
        /// Fires the before drop down context menu open.
        /// </summary>
        protected internal virtual void FireDropDownContextMenuOpen(TabPopupMenuItemCollection menuItemCollection)
        {
            if (this.DropDownContextMenuOpen != null)
            {
                this.DropDownContextMenuOpen(this, new MenuCollectionEventArgs (menuItemCollection));
            }
        }

        /// <summary>
        /// Fires the before drop down context menu close.
        /// </summary>
        protected internal virtual void FireDropDownContextMenuClose(TabPopupMenuItemCollection menuItemCollection)
        {
            if (this.DropDownContextMenuClose != null)
            {
                this.DropDownContextMenuClose(this, new MenuCollectionEventArgs(menuItemCollection));
            }
        }

        /// <summary>
        /// Fires the drag start.
        /// </summary>
        protected internal virtual void FireDragStart()
        {
            if (this.DragStart != null)
            {
                this.DragStart(this, new EventArgs());
            }
        }

        /// <summary>
        /// Fires the drag end.
        /// </summary>
        protected internal virtual void FireDragEnd()
        {
            if (this.DragEnd != null)
            {
                this.DragEnd(this, new EventArgs());
            }
        }

        /// <summary>
        /// Updates tabs layout.
        /// </summary>
        internal void InvalidateTabs()
        {
            for (int i = 0; i < this.Items.Count; i++)
            {
                TabItemAdv item = this.Items[i] as TabItemAdv;
                if (item != null)
                {
                    item.InvalidateMeasure();
                    item.InvalidateArrange();
                }
            }

            if (this.tabControlPanel != null)
            {
                this.tabControlPanel.InvalidateMeasure();
                this.tabControlPanel.InvalidateArrange();
            }

            if (this.tabLayoutPanel != null)
            {
                this.tabLayoutPanel.InvalidateMeasure();
                this.tabLayoutPanel.InvalidateArrange();
            }
        }

        /// <summary>
        /// Internal method to update border thickness
        /// </summary>
        private void UpdateCornerRadius()
        {
            if (this.contentPanel != null)
            {
                switch (this.TabStripPlacement)
                {
                    case TabStripPlacement.Left:
                        this.CornerRadius = new CornerRadius(0, 3, 3, 0);
                        break;

                    case TabStripPlacement.Top:
                        this.CornerRadius = new CornerRadius(0, 0, 3, 3);
                        break;

                    case TabStripPlacement.Right:
                        this.CornerRadius = new CornerRadius(3, 0, 0, 3);
                        break;

                    case TabStripPlacement.Bottom:
                        this.CornerRadius = new CornerRadius(3, 3, 0, 0);
                        break;
                }
            }
        }

        /// <summary>
        /// Updates appearance and layout of child elements according to the TabStripPlacement property.
        /// </summary>
        internal void UpdateTabStripPlacementAppearance()
        {
            Thickness thickness = new Thickness(0);

            switch (this.TabStripPlacement)
            {
                case TabStripPlacement.Top:
                    if (this.tabPanel != null)
                    {
                        if (this.tabPanel.CloseButton != null)
                        {
                            this.tabPanel.CloseButton.RotateButton(0);
                        }

                        if (this.tabPanel.ScrollingPanel != null)
                        {
                            this.tabPanel.ScrollingPanel.RotateButtons(0);
                        }
                    }

                    if (this.tabLayoutPanel != null)
                    {
                        this.tabLayoutPanel.IsRightToLeft = false;
                    }

                    for (int i = 0; i < this.TabHeaders.Count; i++)
                    {
                        TabItemAdv tabHeader = this.TabHeaders[i] as TabItemAdv;
                        if (tabHeader != null && tabHeader.CloseButton != null)
                        {
                            tabHeader.CloseButton.RotateButton(0);
                        }
                    }

                    break;

                case TabStripPlacement.Left:
                    if (this.tabPanel != null)
                    {
                        if (this.tabPanel.CloseButton != null)
                        {
                            this.tabPanel.CloseButton.RotateButton(90);
                        }

                        if (this.tabPanel.ScrollingPanel != null)
                        {
                            this.tabPanel.ScrollingPanel.RotateButtons(90);
                        }
                    }

                    if (this.tabLayoutPanel != null)
                    {
                        this.tabLayoutPanel.IsRightToLeft = true;
                    }

                    for (int i = 0; i < this.TabHeaders.Count; i++)
                    {
                        TabItemAdv tabHeader = this.TabHeaders[i] as TabItemAdv;
                        if (tabHeader != null && tabHeader.CloseButton != null)
                        {
                            if (!this.RotateTextWhenVertical)
                            {
                                tabHeader.CloseButton.RotateButton(90);
                            }
                            else
                            {
                                tabHeader.CloseButton.RotateButton(0);
                            }
                        }
                    }

                    break;

                case TabStripPlacement.Right:
                    if (this.tabPanel != null)
                    {
                        if (this.tabPanel.CloseButton != null)
                        {
                            this.tabPanel.CloseButton.RotateButton(-90);
                        }

                        if (this.tabPanel.ScrollingPanel != null)
                        {
                            this.tabPanel.ScrollingPanel.RotateButtons(-90);
                        }
                    }

                    if (this.tabLayoutPanel != null)
                    {
                        this.tabLayoutPanel.IsRightToLeft = false;
                    }

                    for (int i = 0; i < this.TabHeaders.Count; i++)
                    {
                        TabItemAdv tabHeader = this.TabHeaders[i] as TabItemAdv;
                        if (tabHeader != null && tabHeader.CloseButton != null)
                        {
                            if (!this.RotateTextWhenVertical)
                            {
                                tabHeader.CloseButton.RotateButton(-90);
                            }
                            else
                            {
                                tabHeader.CloseButton.RotateButton(0);
                            }
                        }
                    }

                    break;

                case TabStripPlacement.Bottom:
                    if (this.tabPanel != null)
                    {
                        if (this.tabPanel.CloseButton != null)
                        {
                            this.tabPanel.CloseButton.RotateButton(180);
                        }

                        if (this.tabPanel.ScrollingPanel != null)
                        {
                            this.tabPanel.ScrollingPanel.RotateButtons(180);
                        }
                    }                   
                        if (this.tabLayoutPanel != null)
                        {
                            this.tabLayoutPanel.IsRightToLeft = true; 
                        }

                        for (int i = 0; i < this.TabHeaders.Count; i++)
                        {
                            TabItemAdv tabHeader = this.TabHeaders[i] as TabItemAdv;
                            //Thickness margin;
                            if (tabHeader != null && tabHeader.CloseButton != null)
                            {
                                tabHeader.CloseButton.RotateButton(0);
                            }

                           
                        }

                    break;
            }
        }


        private static void OnTabVisualStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {          
            TabControlAdv instance = d as TabControlAdv;
            instance.OnTabVisualStyleChanged(e);           
        }
        private void RemoveDictionaryIfExist(FrameworkElement element, System.Windows.ResourceDictionary dictionary)
        {

            if (element != null)
            {

                for (int i = 0; i < element.Resources.MergedDictionaries.Count; i++)
                {
                    var rdic = element.Resources.MergedDictionaries[i];
                    if (rdic.Source == dictionary.Source)
                    {
                        element.Resources.MergedDictionaries.RemoveAt(i);
                        i--;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        public void OnTabVisualStyleChanged(DependencyPropertyChangedEventArgs e)
        {           


            if (e.NewValue != null)
            {
                if ((TabVisualStyle)e.NewValue == TabVisualStyle.ExcelBlack)
                {
                    string uri = "/Syncfusion.Tools.Silverlight;component/Controls/TabControlAdv/Themes/ExcelBlackTabControlstyle.xaml";
                    System.Windows.ResourceDictionary rd = new System.Windows.ResourceDictionary();
                    rd.Source = new Uri(uri, UriKind.RelativeOrAbsolute);
                    RemoveDictionaryIfExist(this, rd);
                    this.Resources.MergedDictionaries.Add(rd);
                   
                }
                else if ((TabVisualStyle)e.NewValue == TabVisualStyle.ExcelBlue)
                {
                    string uri = "/Syncfusion.Tools.Silverlight;component/Controls/TabControlAdv/Themes/ExcelBlueTabControlstyle.xaml";
                    System.Windows.ResourceDictionary  rd = new System.Windows.ResourceDictionary();
                    rd.Source = new Uri(uri, UriKind.RelativeOrAbsolute);
                     RemoveDictionaryIfExist(this, rd);
                    this.Resources.MergedDictionaries.Add(rd);
                            
                }
                else if ((TabVisualStyle)e.NewValue == TabVisualStyle.ExcelSilver)
                {
                    string uri = "/Syncfusion.Tools.Silverlight;component/Controls/TabControlAdv/Themes/ExcelSilverTabControlStyle.xaml";
                    System.Windows.ResourceDictionary rd = new System.Windows.ResourceDictionary();
                    rd.Source = new Uri(uri, UriKind.RelativeOrAbsolute);
                    RemoveDictionaryIfExist(this, rd);
                    this.Resources.MergedDictionaries.Add(rd);          
                }
                else if ((TabVisualStyle)e.NewValue == TabVisualStyle.None )
                {
                    string[] exceslstyles = new string[] { "Blue", "Black", "Silver" };
                    System.Windows.ResourceDictionary rd = new System.Windows.ResourceDictionary();
                    
                    foreach (string stylename in exceslstyles)
                    {
                        rd.Source = new Uri("/Syncfusion.Tools.Silverlight;component/Controls/TabControlAdv/Themes/Excel" + stylename + "TabControlstyle.xaml", UriKind.RelativeOrAbsolute);
                        RemoveDictionaryIfExist(this, rd);
                    }
                   
                    
                }
            }
        }


        /// <summary>
        /// Raises the <see cref="E:ItemsSourceChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected virtual void OnItemsSourceChanged(EventArgs e)
        {
            this.TabHeaders.Clear();

            if (this.tabPanel != null)
            {
                this.tabPanel.Visibility = Visibility.Visible;
                IsAllTabsClosed = false;
            }

            for (int i = 0; i < this.Items.Count; i++)
            {
                TabItemAdv tabItem = Items[i] as TabItemAdv;
                if (tabItem != null)
                {
                    tabItem.IsSelected = tabItem.IsSelected;

                    this.TabHeaders.Add(tabItem);
                    tabItem.TabControlParent = this;
                }
                if (this.SelectedItem != null)
                {
                    this.SelectedItem = this.Items[Items.IndexOf(tabItem)] as TabItemAdv;
                }
                if (this.contentpanelpresenter != null)
                {
                    ContentPresenter contentpresenter = this.contentpanelpresenter.Child as ContentPresenter;
                    contentpresenter.Visibility = Visibility.Visible;
                }
            }
        }


        /// <summary>
        /// Undoes the effects of the <see cref="M:System.Windows.Controls.ItemsControl.PrepareContainerForItemOverride(System.Windows.DependencyObject,System.Object)"/> method.
        /// </summary>
        /// <param name="element">The container element.</param>
        /// <param name="item">The item.</param>
        protected override void ClearContainerForItemOverride(DependencyObject element, object item)
        {
            base.ClearContainerForItemOverride(element, item);
        }


        /// <summary>
        /// Method returns the container
        /// </summary>
        /// <returns>Type : TreeViewItemAdv</returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            base.GetContainerForItemOverride();
            if (this.lastContainerCheck != null)
            {
                return this.lastContainerCheck;
            }

            TabItemAdv itm = new TabItemAdv();
            return itm;
        }

        /// <summary>
        /// Method Determines whether the item and container is same or not
        /// </summary>
        /// <param name="item">Indicates the item</param>
        /// <returns>Type : bool</returns>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            base.IsItemItsOwnContainerOverride(item);
            this.lastContainerCheck = null;
            if (item is TabItemAdv)
            {
                return true;
            }

            DataTemplate template = this.ItemTemplate;
            if (template != null)
            {
                DependencyObject container = template.LoadContent();
                if (container is TabItemAdv)
                {
                    this.lastContainerCheck = (TabItemAdv)container;
                }
            }

            return false;
        }

        /// <summary>
        /// Gets next tab item from the specified index according to the direction.
        /// </summary>
        /// <param name="startIndex">Index from which searching is started.</param>
        /// <param name="direction">The direction to search the tab item.</param>
        /// <returns>Next tab item.</returns>
        internal TabItemAdv FindNextTabItem(int startIndex, int direction)
        {
            if (this.TabStripPlacement == TabStripPlacement.Left)
            {
                direction *= -1;
            }

            int index = startIndex;
            for (int i = 0; i < this.Items.Count; i++)
            {
                index += direction;
                if (index >= this.Items.Count)
                {
                    index = 0;
                }
                else if (index < 0)
                {
                    index = this.Items.Count - 1;
                }

                TabItemAdv itemAtIndex = this.GetItemAtIndex(index);
                if (((itemAtIndex != null) && itemAtIndex.IsEnabled) && (itemAtIndex.Visibility == Visibility.Visible))
                {
                    return itemAtIndex;
                }
            }

            return null;
        }

        /// <summary>
        /// Updates selected content appearance.
        /// </summary>
        internal void UpdateSelectedContent()
        {
            TabItemAdv selectedTabItem = this.SelectedItem;
            if (selectedTabItem != null && selectedTabItem.Visibility == Visibility.Visible)
            {
                this.SelectedContent = selectedTabItem.Content;
                if (this.SelectedItem.TabContentPresenter != null)
                {
                    this.SelectedItem.TabContentPresenter.MouseMove += new MouseEventHandler(TabControlAdvMouseMove);
                    this.SelectedItem.TabContentPresenter.MouseEnter += new MouseEventHandler(TabControlAdvMouseEnter);
                }
            }

            else
            {
                this.SelectedContent = null;
            }
        }

        /// <summary>
        /// Occurs when the mouse enters the bounding area of a System.Windows.UIElement.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void TabControlAdvMouseEnter(object sender, MouseEventArgs e)
        {
            for (int i = 0; i < this.TabHeaders.Count; i++)
            {
                TabItemAdv tabHeader = this.TabHeaders[i] as TabItemAdv;
                if (tabHeader != null && tabHeader.TabBorder != null)
                {
                    bool isLeftRotated = tabHeader.IsTextRotated && this.TabStripPlacement == TabStripPlacement.Left;
                    bool isRightRotated = tabHeader.IsTextRotated && this.TabStripPlacement == TabStripPlacement.Right;
                    tabHeader.TabBorder.SetVisualState(false, isRightRotated, isLeftRotated);
                }
            }
        }

        /// <summary>
        /// Occurs when the coordinate position of the mouse changes while
        /// over a System.Windows.UIElement.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void TabControlAdvMouseMove(object sender, MouseEventArgs e)
        {
            if (this.CloseButtonType == CloseButtonType.IndividualOnMouseOver)
            {
                for (int i = 0; i < this.TabHeaders.Count; i++)
                {
                    TabItemAdv tabHeader = this.TabHeaders[i] as TabItemAdv;
                    if (tabHeader != null && tabHeader.CloseButton != null)
                    {
                        tabHeader.CloseButton.HideButton();
                    }
                }
            }
        }

        /// <summary>
        /// Gets the tab item at specified index.
        /// </summary>
        /// <param name="index">Index to get tab item at.</param>
        /// <returns>The tab item.</returns>
        private TabItemAdv GetItemAtIndex(int index)
        {
            if ((index >= 0) && (index < this.Items.Count))
            {
                return this.Items[index] as TabItemAdv;
            }

            return null;
        }

        /// <summary>
        /// Calls OnTabItemStyleChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnTabItemStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabControlAdv instance = (TabControlAdv)d;
            instance.OnTabItemStyleChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises TabItemStyleChanged event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnTabItemStyleChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.TabItemStyleChanged != null)
            {
                this.TabItemStyleChanged(this, e);
            }
            //for (int i = 0; i < this.Items.Count; i++)
            //{
            //    TabItemAdv tabItem = this.Items[i] as TabItemAdv;
            //    if (tabItem != null)
            //    {
            //        tabItem.Style = this.TabItemStyle;
            //    }
            //}

            //if (this.TabItemStyleChanged != null)
            //{
            //    this.TabItemStyleChanged(this, e);
            //}
        }



        /// <summary>
        /// Calls OnSelectedIndexChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnSelectedIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabControlAdv instance = (TabControlAdv)d;
            instance.OnSelectedIndexChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises SelectedIndexChanged event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnSelectedIndexChanged(DependencyPropertyChangedEventArgs e)
        {
            int newValue = (int)e.NewValue;
            int oldValue = (int)e.OldValue;
            if (newValue < 0)
            {
                this.SelectedItem = null;
            }
            else if (newValue >= this.Items.Count && oldValue < this.Items.Count)
            {
                this.SelectedIndex = oldValue;
                this.SelectedItem = null;
            }
            else
            {
                TabItemAdv itemAtIndex = this.GetItemAtIndex(newValue);
                if (this.SelectedItem != itemAtIndex)
                {
                    this.SelectedItem = itemAtIndex;
                    this.SelectedItem.Focus();
                    if (this.tabLayoutPanel != null)
                    {
                        this.tabLayoutPanel.SelectItemInternal();
                    }
                }

                if (oldValue >= 0 && oldValue < this.Items.Count)
                {
                    TabItemAdv item = this.Items[oldValue] as TabItemAdv;
                    if (item != null)
                    {
                        item.IsSelected = false;
                    }
                }

                this.SelectedItem.IsSelected = true;
            }

            this.VerifyZIndex();

            if (this.SelectedIndexChanged != null)
            {
                this.SelectedIndexChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnSelectedItemChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabControlAdv instance = (TabControlAdv)d;
            instance.OnSelectedItemChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises SelectedItemChanged event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnSelectedItemChanged(DependencyPropertyChangedEventArgs e)
        {
            TabItemAdv oldValue = e.OldValue as TabItemAdv;
            TabItemAdv newValue = e.NewValue as TabItemAdv;
            int num = (newValue == null) ? -1 : this.GetIndexOfTabItemAdv(newValue);

            if ((newValue == null) && (num == -1))
            {
                this.SelectedItem = oldValue;
                this.SelectedIndex = this.GetIndexOfTabItemAdv(oldValue);
            }
            else
            {
                if (oldValue != null)
                {
                    oldValue.IsSelected = false;
                }

                this.UpdateSelectedContent();
                if (num >= 0)
                    this.SelectedItem = this.Items[num] as TabItemAdv;
                this.SelectedIndex = num;
                this.SelectedItem.Focus();
                if (this.tabLayoutPanel != null)
                {
                    this.tabLayoutPanel.SelectItemInternal();
                }
            }

            this.VerifyZIndex();

            if (this.SelectedItemChanged != null)
            {
                this.SelectedItemChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnTabStripPlacementChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnTabStripPlacementChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabControlAdv instance = (TabControlAdv)d;
            instance.OnTabStripPlacementChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises TabStripPlacementChanged event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnTabStripPlacementChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.tabControlPanel != null)
            {
                this.tabControlPanel.TabStripPlacement = this.TabStripPlacement;
            }

            this.VerifyZIndex();
            this.UpdateTabStripPlacementAppearance();
            this.UpdateCornerRadius();
            this.UpdateVisualState();

            for (int i = 0; i < this.TabHeaders.Count; i++)
            {
                TabItemAdv tabHeader = this.TabHeaders[i] as TabItemAdv;
                if (tabHeader != null && tabHeader.TabBorder != null)
                {
                    tabHeader.TabBorder.RefreshBorderPaths();
                }

                tabHeader.UpdateTabsMargin();
            }

            this.InvalidateTabs();

            if (this.tabLayoutPanel != null)
            {
                if ((this.TabStripPlacement == TabStripPlacement.Left) || (this.TabStripPlacement == TabStripPlacement.Right))
                {
                    if (this.ActualHeight < this.TabLayoutPanel.ActualWidth)
                    {
                        if (this.TabScrollStyle == TabScrollStyle.Extended)
                        {
                            this.tabLayoutPanel.MeasureElements(new Size(this.ActualHeight - 114, this.tabLayoutPanel.ActualHeight));
                            this.tabLayoutPanel.ArrangeElements(new Size(this.ActualHeight - 114, this.tabLayoutPanel.ActualHeight));
                        }
                        else
                        {
                            this.tabLayoutPanel.MeasureElements(new Size(this.ActualHeight - 58, this.tabLayoutPanel.ActualHeight));
                            this.tabLayoutPanel.ArrangeElements(new Size(this.ActualHeight - 58, this.tabLayoutPanel.ActualHeight));
                        }
                    }
                }
                else
                {
                    if (this.ActualWidth < this.TabLayoutPanel.ActualWidth)
                    {
                        if (this.TabScrollStyle == TabScrollStyle.Extended)
                        {
                            this.tabLayoutPanel.MeasureElements(new Size(this.ActualWidth - 114, this.tabLayoutPanel.ActualHeight));
                            this.tabLayoutPanel.ArrangeElements(new Size(this.ActualWidth - 114, this.tabLayoutPanel.ActualHeight));
                        }
                        else
                        {
                            this.tabLayoutPanel.MeasureElements(new Size(this.ActualWidth - 58, this.tabLayoutPanel.ActualHeight));
                            this.tabLayoutPanel.ArrangeElements(new Size(this.ActualWidth - 58, this.tabLayoutPanel.ActualHeight));
                        }
                    }
                }

                this.tabLayoutPanel.SelectItemInternal();
            }
            if (this.TabStripPlacementChanged != null)
            {
                this.TabStripPlacementChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnSelectedContentChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnSelectedContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabControlAdv instance = (TabControlAdv)d;
            instance.OnSelectedContentChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises SelectedContentChanged event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnSelectedContentChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.SelectedContentChanged != null)
            {
                this.SelectedContentChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnScrollingTimeChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnScrollingTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabControlAdv instance = (TabControlAdv)d;
            instance.OnScrollingTimeChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises ScrollingTimeChanged event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnScrollingTimeChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.ScrollingTimeChanged != null)
            {
                this.ScrollingTimeChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnIsAllTabsClosedChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnIsAllTabsClosedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabControlAdv instance = (TabControlAdv)d;
            instance.OnIsAllTabsClosedChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises IsAllTabsClosedChanged event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnIsAllTabsClosedChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.IsAllTabsClosedChanged != null)
            {
                this.IsAllTabsClosedChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnEnableLabelEditChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnEnableLabelEditChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabControlAdv instance = (TabControlAdv)d;
            instance.OnEnableLabelEditChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises EnableLabelEditChanged event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnEnableLabelEditChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.EnableLabelEditChanged != null)
            {
                this.EnableLabelEditChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnShowTabListContextMenuChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnShowTabListContextMenuChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabControlAdv instance = (TabControlAdv)d;
            instance.OnShowTabListContextMenuChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises ShowTabListContextMenuChanged event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnShowTabListContextMenuChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.tabPanel != null)
            {
                if (this.ShowTabListContextMenu)
                {
                    this.tabPanel.MenuPanel.Visibility = Visibility.Visible;
                }
                else
                {
                    this.tabPanel.MenuPanel.Visibility = Visibility.Collapsed;
                }
            }

            if (this.ShowTabListContextMenuChanged != null)
            {
                this.ShowTabListContextMenuChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnCloseButtonTypeChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnCloseButtonTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabControlAdv instance = (TabControlAdv)d;
            instance.OnCloseButtonTypeChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises CloseButtonTypeChanged event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnCloseButtonTypeChanged(DependencyPropertyChangedEventArgs e)
        {
            this.UpdateCloseButtonsVisibility();

            if (this.CloseButtonTypeChanged != null)
            {
                this.CloseButtonTypeChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnHotTrackingEnabledChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnHotTrackingEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabControlAdv instance = (TabControlAdv)d;
            instance.OnHotTrackingEnabledChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises HotTrackingEnabledChanged event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnHotTrackingEnabledChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.HotTrackingEnabledChanged != null)
            {
                this.HotTrackingEnabledChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnRotateTextWhenVerticalChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnRotateTextWhenVerticalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabControlAdv instance = (TabControlAdv)d;
            instance.OnRotateTextWhenVerticalChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises RotateTextWhenVerticalChanged event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnRotateTextWhenVerticalChanged(DependencyPropertyChangedEventArgs e)
        {
            for (int i = 0; i < this.TabHeaders.Count; i++)
            {
                TabItemAdv tabHeader = this.TabHeaders[i] as TabItemAdv;
                if (tabHeader != null)
                {
                    if (tabHeader.CloseButton != null)
                    {
                        if (!tabHeader.IsTextRotated)
                        {
                            tabHeader.CloseButton.RotateButton(-90);
                        }
                        else
                        {
                            tabHeader.CloseButton.RotateButton(0);
                        }
                    }

                    if (tabHeader.TabBorder != null)
                    {
                        tabHeader.TabBorder.RefreshBorderPaths();
                    }

                    tabHeader.UpdateTabsMargin();
                }
            }

            this.InvalidateTabs();

            if (this.RotateTextWhenVerticalChanged != null)
            {
                this.RotateTextWhenVerticalChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnTabPanelStyleChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnTabPanelStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabControlAdv instance = (TabControlAdv)d;
            instance.OnTabPanelStyleChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises TabPanelStyleChanged event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnTabPanelStyleChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.TabPanelStyleChanged != null)
            {
                this.TabPanelStyleChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnTabScrollButtonVisibilityChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnTabScrollButtonVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabControlAdv instance = (TabControlAdv)d;
            instance.OnTabScrollButtonVisibilityChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises TabScrollButtonVisibilityChanged event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnTabScrollButtonVisibilityChanged(DependencyPropertyChangedEventArgs e)
        {
            this.InvalidateTabs();
            if (this.TabScrollButtonVisibilityChanged != null)
            {
                this.TabScrollButtonVisibilityChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnTabScrollStyleChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnTabScrollStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabControlAdv instance = (TabControlAdv)d;
            instance.OnTabScrollStyleChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises TabScrollStyleChanged event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnTabScrollStyleChanged(DependencyPropertyChangedEventArgs e)
        {
            this.InvalidateTabs();
            if (this.TabScrollStyleChanged != null)
            {
                this.TabScrollStyleChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnTabItemLayoutChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnTabItemLayoutChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabControlAdv instance = (TabControlAdv)d;
            instance.OnTabItemLayoutChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises TabItemLayoutChanged event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnTabItemLayoutChanged(DependencyPropertyChangedEventArgs e)
        {
            this.InvalidateTabs();
            if (this.TabItemLayoutChanged != null)
            {
                this.TabItemLayoutChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnTabItemSizeChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnTabItemSizeModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabControlAdv instance = (TabControlAdv)d;
            instance.OnTabItemSizeModeChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises TabItemSizeChanged event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnTabItemSizeModeChanged(DependencyPropertyChangedEventArgs e)
        {
            this.InvalidateTabs();
            if (this.TabItemSizeModeChanged != null)
            {
                this.TabItemSizeModeChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnKeepTabInFrontChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnKeepTabInFrontChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabControlAdv instance = (TabControlAdv)d;
            instance.OnKeepTabInFrontChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises KeepTabInFrontChanged event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnKeepTabInFrontChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.KeepTabInFrontChanged != null)
            {
                this.KeepTabInFrontChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnSelectedItemFontWeightChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnSelectedItemFontWeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabControlAdv instance = (TabControlAdv)d;
            instance.OnSelectedItemFontWeightChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises SelectedItemFontWeightChanged event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnSelectedItemFontWeightChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.SelectedItemFontWeightChanged != null)
            {
                this.SelectedItemFontWeightChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnAllowDragDropChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnAllowDragDropChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabControlAdv instance = (TabControlAdv)d;
            instance.OnAllowDragDropChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises AllowDragDropChanged event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnAllowDragDropChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.AllowDragDropChanged != null)
            {
                this.AllowDragDropChanged(this, e);
            }
        }

        private static void OnTabItemOpenModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabControlAdv instance = (TabControlAdv)d;
            instance.OnTabItemOpenModeChanged(e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnTabItemOpenModeChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.TabItemOpenModeChanged != null)
                this.TabItemOpenModeChanged(this, e);
        }

        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    public enum TabVisualStyle
    {
        /// <summary>
        /// 
        /// </summary>
        None,
        /// <summary>
        /// 
        /// </summary>
        ExcelBlue,
        /// <summary>
        /// 
        /// </summary>
        ExcelSilver,
        /// <summary>
        /// 
        /// </summary>
        ExcelBlack,
    }
   
    /// <summary>
    /// Represent Event Args for Menu ItemsCollection. 
    /// </summary>
    public class MenuCollectionEventArgs : EventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        public TabPopupMenuItemCollection Result;

        /// <summary>
        /// Initializes a new instance of the <see cref="MenuCollectionEventArgs"/> class.
        /// </summary>
        public MenuCollectionEventArgs()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MenuCollectionEventArgs"/> class.
        /// </summary>
        /// <param name="item">The item.</param>
        public MenuCollectionEventArgs(TabPopupMenuItemCollection item)
        {
            this.Result = item;
        }
    }
}
