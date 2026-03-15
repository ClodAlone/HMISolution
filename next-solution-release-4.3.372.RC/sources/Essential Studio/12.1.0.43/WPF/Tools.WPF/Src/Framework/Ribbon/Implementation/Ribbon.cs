// <copyright file="Ribbon.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml;
using System.Linq;
using Syncfusion.Licensing;
using Syncfusion.Windows.Shared;
using System.Collections.ObjectModel;
using System.IO.IsolatedStorage;
using System.Xml.Serialization;
using System.Windows.Forms.Integration;
using System.Windows.Interop;
using Syncfusion.Windows.Tools.Controls.Resources;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents a Ribbon control.
    /// </summary>
    /// <list type="table">
    /// 	<listheader>
    /// 		<term>Help Page</term>
    /// 		<description>Syntax</description>
    /// 	</listheader>
    /// 	<example>
    /// 		<list type="table">
    /// 			<listheader>
    /// 				<description>C#</description>
    /// 			</listheader>
    /// 			<example><code>public class Ribbon : Selector</code></example>
    /// 		</list>
    /// 		<para/>
    /// 		<list type="table">
    /// 			<listheader>
    /// 				<description>XAML Object Element Usage</description>
    /// 			</listheader>
    /// 			<example><code><![CDATA[<ribbon:Ribbon Name="ribbon" />]]></code></example>
    /// 		</list>
    /// 	</example>
    /// </list>
    /// <remarks>
    /// Ribbon class represents main Ribbon control that hosts RibbonTabs elements.
    /// </remarks>
    /// <example>
    /// 	<para/>This example shows how to create a Ribbon in XAML.
    /// <code>
    /// 		<![CDATA[
    /// <ribbon:Ribbon x:Name="MyRibbon">
    /// <ribbon:RibbonTab Caption="Home"/>
    /// <ribbon:RibbonTab Caption="Insert"/>
    /// </ribbon:Ribbon x:Name="MyRibbon">
    /// ]]>
    /// 	</code>
    /// 	<para/>This example shows how to create a Ribbon in C#.
    /// <code>
    /// StackPanel stackPanel;
    /// RibbonTab tab1;
    /// RibbonTab tab2;
    /// Ribbon ribbon = new Ribbon();
    /// ribbon.Items.Add(tab1);
    /// ribbon.Items.Add(tab2);
    /// stackPanel.Children.Add( ribbon );
    /// </code>
    /// </example>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(true)]
#endif

    [SkinType(SkinVisualStyle = Skin.Office2007Blue,
      Type = typeof(Ribbon), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/Ribbon/Themes/Office2007BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Black,
    Type = typeof(Ribbon), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/Ribbon/Themes/Office2007BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Silver,
    Type = typeof(Ribbon), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/Ribbon/Themes/Office2007SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2003,
    Type = typeof(Ribbon), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/Ribbon/Themes/Office2003Style.xaml")]
    [SkinType(SkinVisualStyle = Skin.Blend,
    Type = typeof(Ribbon), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/Ribbon/Themes/BlendStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.ShinyRed,
    Type = typeof(Ribbon), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/Ribbon/Themes/ShinyRedStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.ShinyBlue,
    Type = typeof(Ribbon), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/Ribbon/Themes/ShinyBlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Default,
    Type = typeof(Ribbon), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/Ribbon/Themes/Generic.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Black,
    Type = typeof(Ribbon), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/Ribbon/Themes/Office2010BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Blue,
    Type = typeof(Ribbon), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/Ribbon/Themes/Office2010BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Silver,
    Type = typeof(Ribbon), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/Ribbon/Themes/Office2010SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.VS2010,
    Type = typeof(Ribbon), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/Ribbon/Themes/VS2010Style.xaml")]
    [SkinType(SkinVisualStyle = Skin.SyncOrange ,
   Type = typeof(Ribbon), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/Ribbon/Themes/SyncOrangeStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Metro,
  Type = typeof(Ribbon), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/Ribbon/Themes/MetroStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Transparent,
 Type = typeof(Ribbon), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/Ribbon/Themes/TransparentStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Windows8,
Type = typeof(Ribbon), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/Ribbon/Themes/Windows8Style.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2013,
Type = typeof(Ribbon), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/Ribbon/Themes/Office2013Style.xaml")]

    public class Ribbon : Selector, IDisposable
    {
        #region Private Members


        internal bool EnableTouch = false;
        SystemGesture systemGesture;

        internal ArrayList qatApplicationItemIndex;

        internal bool isCheckedToggleButton = false;
        /// <summary>
        /// Represent Is QAT once loaded when State Persistence is enabled.
        /// </summary>
        internal bool IsQATOnceLoaded;
                
        /// <summary>
        /// RibbontoggleButton in Office 2010 UI.
        /// </summary>
        internal RibbonToggleButton ribbonToggleButton;

        internal RibbonContextMenu ribbon_contextmenu;
        /// <summary>
        /// Key tips current level.
        /// </summary>
        private Dictionary<string,KeyTip> m_keyTipsCurrentLevel;

        /// <summary>
        /// First visible context tab group.
        /// </summary>
        private ContextTabGroup m_firstVisibleGroup = null;

        /// <summary>
        /// Last visible context tab group.
        /// </summary>
        private ContextTabGroup m_lastVisibleGroup = null;

        /// <summary>
        /// Ribbon window instance.
        /// </summary>
        private RibbonWindow m_ribbonWindow = null;

        /// <summary>
        /// Normal window instance.
        /// </summary>
        private Window RootWindow = null;

        /// <summary>
        /// Mouse position at the window
        /// </summary>
        internal Point mousePoint;

        internal bool m_cancelAdornerState = false;

        private bool tabonwindow1 = true;
        private bool tabonwindow2 = true;
       
         /// <summary>
        /// Stores a tree of KeuTips.
        /// </summary>
        private Dictionary<string,KeyTip> m_keyTips;

        /// <summary>
        /// Path to the current KeyTip, beginning from the top of the
        /// tree.
        /// </summary>
        private List<string> m_keyTipPath;

        /// <summary>
        /// Stores pressed letters of the complex KeyTip.
        /// </summary>
        private string m_keyTipComplexPath=string.Empty;

        /// <summary>
        /// Current complex key tips level.
        /// </summary>
        private Dictionary<string,KeyTip> m_keyTipComplexLevel;

        /// <summary>
        /// Current key tips level(branch).
        /// </summary>
        private Dictionary<string,KeyTip> m_keyTipCurrentLevel;

        /// <summary>
        /// Defines whether any of current level key tips starts from the
        /// specified letter.
        /// </summary>
        private bool m_complexLetterAdded;

        /// <summary>
        /// Specifies whether adorner is shown.
        /// </summary>
        internal static bool m_isAdornersShown = false;

        /// <summary>
        /// Custom color scheme blend color. 
        /// </summary>
        private Color m_blendColor;

        /// <summary>
        /// Adorner layer.
        /// </summary>
        internal AdornerLayer m_adornerLayer = null;

        /// <summary>
        /// DateTime structure that is used for detecting ALT key pressed state for Key Tips correct appearance.
        /// </summary>
        private DateTime m_startTime = DateTime.MinValue;

        /// <summary>
        /// Boolean value that is used for prevention of undesirable hiding of Key Tips.
        /// </summary>
        private static bool m_altWasPressed = false;

        /// <summary>
        /// Represents the ItemsPanelAdornerLayerChanged 
        /// </summary>
        internal bool m_bItemsPanelAdornerLayerChanged = false;

        /// <summary>
        /// Boolean value that is used for checking the ribbon tab's of Key Tips.
        /// </summary>
        private bool m_ribbontabflag = true;

        /// <summary>
        /// Represents Complex string value.
        /// </summary>
        private string m_complexKeyTipString = string.Empty;

        /// <summary>
        /// Represents dropdown button.
        /// </summary>
        private DropDownButton m_dropDownbutton = null;

        /// <summary>
        /// Represents the splitmenu button
        /// </summary>
        private SplitMenuButton m_splitMenuButton = null;

        /// <summary>
        /// Represents application menu provider.
        /// </summary>
        private IExpandCollapseProvider appMenuProvider = null;

        /// <summary>
        /// Represent whether the current Ribbon is loaded
        /// </summary>
        private bool isItemLoaded = false;

        /// <summary>
        /// Represent whether the key is already selected
        /// </summary>
        private static bool IsKeySelected = true;

        internal RibbonStateParams Params;

        internal TabPanel m_TabPanel = null;

        /// <summary>
        /// Represent the isloaded flag.
        /// </summary>
        internal bool isloaded = false;
        /// <summary>
        /// Represent popup which is used to display ribbon in Adorner state.
        /// </summary>
        Popup Adorner_Popup;

        /// <summary>
        /// Represent the collection of FrameworkElements in the current window.
        /// </summary>
        Dictionary<FrameworkElement, Visibility> collapsedElements = new Dictionary<FrameworkElement, Visibility>();

        /// <summary>
        /// Have the current Tab collection temporarily .
        /// </summary>
        List<RibbonTab> _tempTabCollection = new List<RibbonTab>();

        /// <summary>
        /// To have the current ItemsSource temporarily 
        /// </summary>
        IEnumerable<RibbonTab> _itemsSourceItems;

        /// <summary>
        /// Keep the Modal Tab collection.
        /// </summary>
        List<RibbonTab> _modalTabCollection = new List<RibbonTab>();

        /// <summary>
        /// Keep the seleted tab item when handling Modal Tab.
        /// </summary>
        object _tempSelectedItem;

        /// <summary>
        /// Represent whether any Modal Tabs displayed on current. 
        /// </summary>
        bool modalTabDisplayed;

        /// <summary>
        /// Represent whether the StatusBar is collapsed on opening the BackStage. 
        /// </summary>
        bool isStatusBarCollapsedByBackStage = false;
        /// <summary>
        /// Represent whether the RibbonBar is collapsed when using KeyTip. 
        /// </summary>
        internal bool isCollapsedKeyTip = false;

        internal bool? WFHvisible = null;

        internal bool? browserVisible = null;

        internal bool m_Qatdropdownclicked = false;

        private bool isXMLStateLoading = false;

        private double ribbonHeight;

        internal bool isQATDialogOpened = false;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the is auto size form enabled.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public static bool GetIsAutoSizeFormEnabled(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsAutoSizeFormEnabledProperty);
        }

        /// <summary>
        /// Sets the is auto size form enabled.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        public static void SetIsAutoSizeFormEnabled(DependencyObject obj, bool value)
        {
            obj.SetValue(IsAutoSizeFormEnabledProperty, value);
        }

        // Using a DependencyProperty as the backing store for IsAutoSizeFormEnabled1.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsAutoSizeFormEnabledProperty =
            DependencyProperty.RegisterAttached("IsAutoSizeFormEnabled", typeof(bool), typeof(Ribbon), new UIPropertyMetadata(false));

        /// <summary>
        /// Instance to store QAT Items from QAT Bar.
        /// </summary>
        internal readonly Dictionary<UIElement, UIElement> QATItems = new Dictionary<UIElement, UIElement>();

        /// <summary>
        /// Instance to store QAT Items for ApplicationMenu
        /// </summary>
        internal ObservableCollection<UIElement> tempQATItems = new ObservableCollection<UIElement>();

        /// <summary>
        /// Instance to store QAT Items from QAT Bar.
        /// </summary>
        internal ObservableCollection<UIElement> QATItemCollection = new ObservableCollection<UIElement>();

        internal bool hadFound = false;

        /// <summary>
        /// Private Variable to Store Default Store file name.
        /// </summary>
        private string default_StoreFile = "RibbonState.dat";

        /// <summary>
        /// Private Variable to store Reset file name.
        /// </summary>
        private string reset_StoreFile = "ResetState.dat";

        private bool showBackStage = false;

        /// <summary>
        /// Gets or sets the key tips current level.
        /// </summary>
        /// <value>
        /// The key tips current level.
        /// </value>
        internal Dictionary<string,KeyTip> KeyTipsCurrentLevel
        {
            get
            {
                return m_keyTipsCurrentLevel;
            }

            set
            {
                m_keyTipsCurrentLevel = value;
            }
        }

        /// <summary>
        /// Gets or sets the state of the ribbon.
        /// </summary>
        /// <value>
        /// Type: <see cref="RibbonState"/>
        /// The state of the ribbon.
        /// </value>
        public RibbonState RibbonState
        {
            get
            {
                return (RibbonState)GetValue(RibbonStateProperty);
            }

            set
            {
                SetValue(RibbonStateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the context tab groups.
        /// </summary>
        /// <value>
        /// Type: <see cref="ContextTabGroupCollection"/>
        /// The collection of ContextTabGroups.
        /// </value>
        public ContextTabGroupCollection ContextTabGroups
        {
            get
            {
                return (ContextTabGroupCollection)GetValue(ContextTabGroupsProperty);
            }

            set
            {
                SetValue(ContextTabGroupsProperty, value);
            }
        }
        
        /// <summary>
        /// Gets or sets the back stage.
        /// </summary>
        /// <value>The back stage.</value>
        public Backstage BackStage
        {
            get { return (Backstage)GetValue(BackStageProperty); }
            set { SetValue(BackStageProperty, value); }
        }
        
        /// <summary>
        /// Gets or sets the modal tab collection.
        /// </summary>
        /// <value>The modal tab collection.</value>
        public ModalTabCollection ModalTabCollection
        {
            get { return (ModalTabCollection)GetValue(ModalTabCollectionProperty); }
            set { SetValue(ModalTabCollectionProperty, value); }
        }

        /// <summary>
        /// Gets the first visible group.
        /// </summary>
        protected internal ContextTabGroup FirstVisibleGroup
        {
            get
            {
                return m_firstVisibleGroup;
            }
        }

        /// <summary>
        /// Gets the last visible group.
        /// </summary>
        /// <value>The last visible group.</value>
        protected internal ContextTabGroup LastVisibleGroup
        {
            get
            {
                return m_lastVisibleGroup;
            }
        }

        /// <summary>
        /// Gets or sets the ribbon bar collapse image.
        /// </summary>
        /// <value>The ribbon bar collapse image.</value>
        public ImageSource RibbonBarCollapseImage
        {
            get { return (ImageSource)GetValue(RibbonBarCollapseImageProperty); }
            set { SetValue(RibbonBarCollapseImageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RibbonBarCollapseImage.  This enables animation, styling, binding, etc...
        /// <summary>
        /// Gets or Sets the Default RibbonBar Collapse Image.
        /// </summary>
        public static readonly DependencyProperty RibbonBarCollapseImageProperty =
            DependencyProperty.Register("RibbonBarCollapseImage", typeof(ImageSource), typeof(Ribbon), new UIPropertyMetadata(null));



        internal bool IsContextTabChecked
        {
            get { return (bool)GetValue(IsContextTabCheckedProperty); }
            set { SetValue(IsContextTabCheckedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsContextTabChecked.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty IsContextTabCheckedProperty =
            DependencyProperty.Register("IsContextTabChecked", typeof(bool), typeof(Ribbon), new PropertyMetadata(false));


        [Browsable(false)]
        public Color SelectedContextTabGroupBackColor   
        {
            get { return (Color)GetValue(SelectedContextTabGroupBackColorProperty); }
            internal set { SetValue(SelectedContextTabGroupBackColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty SelectedContextTabGroupBackColorProperty =
            DependencyProperty.Register("SelectedContextTabGroupBackColor", typeof(Color), typeof(Ribbon), new PropertyMetadata(Colors.Transparent));


        /// <summary>
        /// Gets or sets the TabGroupLabel Alignment.
        /// </summary>
        /// <value>
        /// Type:<see cref="Control"/>
        /// The TabGroupLabel Alignment.
        /// </value>
        public HorizontalAlignment TabGroupLabelAlignment
        {
            get { return (HorizontalAlignment)GetValue(TabGroupLabelAlignmentProperty); }
            set { SetValue(TabGroupLabelAlignmentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TabGroupLabelAlignment.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TabGroupLabelAlignmentProperty =
            DependencyProperty.Register("TabGroupLabelAlignment", typeof(HorizontalAlignment), typeof(Ribbon), new PropertyMetadata(HorizontalAlignment.Stretch));

        


        /// <summary>
        /// Gets or sets the tab panel item.
        /// </summary>
        /// <value>
        /// Type: <see cref="Control"/>
        /// The tab panel item.
        /// </value>
        public Control TabPanelItem
        {
            get
            {
                return (Control)GetValue(TabPanelItemProperty);
            }

            set
            {
                SetValue(TabPanelItemProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has visible context tab group.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has visible context tab group; otherwise, <c>false</c>.
        /// </value>
        protected bool HasVisibleContextTabGroup
        {
            get
            {
                return (bool)GetValue(HasVisibleContextTabGroupProperty);
            }

            set
            {
                SetValue(HasVisibleContextTabGroupPropertyKey, value);
            }
        }

        /// <summary>
        /// Gets or sets the Ribbon ApplicationMenu.
        /// </summary>
        /// <value>
        /// Type: <see cref="ApplicationMenu"/>
        /// ApplicationMenu control object.  Default value is null.
        /// </value>
        public ApplicationMenu ApplicationMenu
        {
            get
            {
                return (ApplicationMenu)GetValue(ApplicationMenuProperty);
            }

            set
            {
                SetValue(ApplicationMenuProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the QuickAccessToolBar of the Ribbon.
        /// </summary>
        /// <value>
        /// Type: <see cref="QuickAccessToolBar"/>
        /// QuickAccessToolBar control object.  Default value is null.
        /// </value>
        public QuickAccessToolBar QuickAccessToolBar
        {
            get
            {
                return (QuickAccessToolBar)GetValue(QuickAccessToolBarProperty);
            }

            set
            {
                SetValue(QuickAccessToolBarProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is QAT below.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is QAT below; otherwise, <c>false</c>.
        /// </value>
        public bool IsQATBelow
        {
            get
            {
                return (bool)GetValue(IsQATBelowProperty);
            }

            set
            {
                SetValue(IsQATBelowProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the selected tab item.
        /// </summary>
        /// <value>The selected tab item.</value>
        [Browsable(false)]
        public RibbonTab SelectedTabItem
        {
            get
            {
                return (RibbonTab)GetValue(SelectedTabItemProperty);
            }

            internal set
            {
                SetValue(SelectedTabItemProperty, value);
            }
        }



        /// <summary>
        /// Gets or sets the minimize button visibility.
        /// </summary>
        /// <value>The minimize button visibility.</value>
        public Visibility MinimizeButtonVisibility
        {
            get
            {
                return (Visibility)GetValue(MinimizeButtonVisibilityProperty);
            }
            set
            {
                SetValue(MinimizeButtonVisibilityProperty, value);
            }

        }

        /// <summary>
        /// Gets or sets a value indicating whether [save original state].
        /// </summary>
        /// <value><c>true</c> if [save original state]; otherwise, <c>false</c>.</value>
        [Description("Indicates whether to save state persisted on loading.")]
        public bool AutoPersist
        {
            get { return (bool)GetValue(AutoPersistProperty); }
            set { SetValue(AutoPersistProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="AutoPersist"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AutoPersistProperty =
           DependencyProperty.Register("AutoPersist", typeof(bool), typeof(Ribbon), new UIPropertyMetadata(false));

        /// <summary>
        /// Used to store the collection of persist elements. 
        /// </summary>
        private ObservableCollection<RibbonElements> persistElements = new ObservableCollection<RibbonElements>();

        /// <summary>
        /// Gets or sets the persist elements.
        /// </summary>
        /// <value>The persist elements.</value>
        public ObservableCollection<RibbonElements> PersistElements
        {
            get
            {
                return this.persistElements;
            }
            set
            {
                this.persistElements = value;
            }
        }


        /// <summary>
        /// Instance for a List to store Initial QAT Items
        /// </summary>
        internal List<UIElement> QATInitialItems = new List<UIElement>();

        /// <summary>
        /// Instance for QAT Items index int the form of Strings.
        /// </summary>
        internal StringBuilder QATInitialItemsString = new StringBuilder();

        static ResourceWrapper wrapper = new ResourceWrapper();



        public bool ShowCustomizeRibbon
        {
            get { return (bool)GetValue(ShowCustomizeRibbonProperty); }
            set { SetValue(ShowCustomizeRibbonProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowCustomizeRibbon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowCustomizeRibbonProperty =
            DependencyProperty.Register("ShowCustomizeRibbon", typeof(bool), typeof(Ribbon), new UIPropertyMetadata(false));

        
        #endregion

        #region Dependency Properties

        /// <summary>
        /// Identifies the IsAutoSizeFormEnabled Property used in Resizing ribbon controls dynamically.
        /// </summary>
        //public static readonly DependencyProperty IsAutoSizeFormEnabledProperty =
        //    DependencyProperty.Register("IsAutoSizeFormEnabled", typeof(bool), typeof(Ribbon), new PropertyMetadata(false));



        public static object GetRibbonQATCommandTag(DependencyObject obj)
        {
            return (object)obj.GetValue(RibbonQATCommandTagProperty);
        }

        public static void SetRibbonQATCommandTag(DependencyObject obj, object value)
        {
            obj.SetValue(RibbonQATCommandTagProperty, value);
        }

        // Using a DependencyProperty as the backing store for RibbonQATCommandTag.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RibbonQATCommandTagProperty =
            DependencyProperty.RegisterAttached("RibbonQATCommandTag", typeof(object), typeof(Ribbon), new FrameworkPropertyMetadata(null));

        

        /// <summary>
        /// Defines whether QATItem is item of the ribbon. This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty IsQATItemProperty =
            DependencyProperty.RegisterAttached("IsQATItem", typeof(bool), typeof(Ribbon), new FrameworkPropertyMetadata(true,FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// Defines QAT is customize. This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty CustomizeQATProperty =
            DependencyProperty.RegisterAttached("CustomizeQAT", typeof(RoutedCommand), typeof(Ribbon), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Defines whether QAT is shown below ribbon. This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty ShowBelowRibbonProperty =
            DependencyProperty.RegisterAttached("ShowBelowRibbon", typeof(RoutedCommand), typeof(Ribbon), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Defines whether QAT is shown above ribbon. This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty ShowAboveRibbonProperty =
            DependencyProperty.RegisterAttached("ShowAboveRibbon", typeof(RoutedCommand), typeof(Ribbon), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Defines Add item to QAT. This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty AddItemToQATProperty =
            DependencyProperty.RegisterAttached("AddToQAT", typeof(RoutedCommand), typeof(Ribbon), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Defines Add remove to QAT. This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty RemoveItemFromQATProperty =
            DependencyProperty.RegisterAttached("RemoveFromQAT", typeof(RoutedCommand), typeof(Ribbon), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Identifies Quick access ToolBar of the ribbon. This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty QuickAccessToolBarProperty =
            DependencyProperty.Register("QuickAccessToolBar", typeof(QuickAccessToolBar), typeof(Ribbon), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Identifies Application Menu of the ribbon. This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty ApplicationMenuProperty =
            DependencyProperty.Register("ApplicationMenu", typeof(ApplicationMenu), typeof(Ribbon), new UIPropertyMetadata(null));

        /// <summary>
        /// Identifies Ribbon state. This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty RibbonStateProperty =
                DependencyProperty.Register("RibbonState", typeof(RibbonState), typeof(Ribbon), new FrameworkPropertyMetadata(RibbonState.Normal, new PropertyChangedCallback(OnRibbonStateChanged), new CoerceValueCallback(OnRibbonStateCoerce)));

        /// <summary>
        /// Identifies Ribbon KeyTip attached dependency property. This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty KeyTipProperty =
            DependencyProperty.RegisterAttached("KeyTip", typeof(string), typeof(Ribbon), new UIPropertyMetadata(string.Empty));

        /// <summary>
        /// Represent the splitmenukey tip, this is a dependency property
        /// </summary>
        public static readonly DependencyProperty SplitMenuKeyTipProperty =
            DependencyProperty.RegisterAttached("SplitMenuKeyTip", typeof(string), typeof(Ribbon), new UIPropertyMetadata(string.Empty));

        /// <summary>
        /// Identifies collection of ribbon ContextTabGroups. This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty ContextTabGroupsProperty =
            DependencyProperty.Register("ContextTabGroups", typeof(ContextTabGroupCollection), typeof(Ribbon), new UIPropertyMetadata(null, new PropertyChangedCallback(ContextTabGroupsChangedCallBack)));

        // Using a DependencyProperty as the backing store for ModalTabCollection.  This enables animation, styling, binding, etc...
        /// <summary>
        /// Identifies collection of ribbon Modal Tabs. This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty ModalTabCollectionProperty =
            DependencyProperty.Register("ModalTabCollection", typeof(ModalTabCollection), typeof(Ribbon), new FrameworkPropertyMetadata(new ModalTabCollection()));

        /// <summary>
        /// Identifies Ribbon TabPanel item. This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty TabPanelItemProperty =
                DependencyProperty.Register("TabPanelItem", typeof(Control), typeof(Ribbon), new UIPropertyMetadata(null, new PropertyChangedCallback(OnTabPanelItemChanged)));

        /// <summary>
        /// Identifies whether there is any visible ContextTabGroup. This is a dependency property.
        /// </summary>
        protected static readonly DependencyPropertyKey HasVisibleContextTabGroupPropertyKey =
            DependencyProperty.RegisterReadOnly("HasVisibleContextTabGroup", typeof(bool), typeof(Ribbon), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnHasVisibleContextTabGroupChanged)));

        /// <summary>
        /// Gets whether Ribbon has visible context tab groups. This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty HasVisibleContextTabGroupProperty = HasVisibleContextTabGroupPropertyKey.DependencyProperty;

        /// <summary>
        /// Defines whether QAT is below or above the ribbon. This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty IsQATBelowProperty =
            DependencyProperty.Register("IsQATBelow", typeof(bool), typeof(Ribbon), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange, new PropertyChangedCallback(OnIsQATBelowChanged)));

        /// <summary>
        /// Defines whether Color Scheme is applied to control. This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty ActiveColorSchemeProperty =
            DependencyProperty.RegisterAttached("ActiveColorScheme", typeof(Brush), typeof(Ribbon), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnActiveColorSchemeChanged)));

        /// <summary>
        /// Represents the selected tab item, this is a dependency property
        /// </summary>
        internal static readonly DependencyProperty SelectedTabItemProperty =
            DependencyProperty.Register("SelectedTabItem", typeof(RibbonTab), typeof(Ribbon), new FrameworkPropertyMetadata(null));


        /// <summary>
        /// Identifies the MinimizeButtonVisibility Property used set RibbonMinimizebuttonVisibility dynamically.
        /// </summary>
        public static readonly DependencyProperty MinimizeButtonVisibilityProperty =
            DependencyProperty.Register("MinimizeButtonVisibility", typeof(Visibility), typeof(Ribbon), new FrameworkPropertyMetadata(Visibility.Visible));

        // Using a DependencyProperty as the backing store for BackStage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BackStageProperty =
            DependencyProperty.Register("BackStage", typeof(Backstage), typeof(Ribbon));


        public BackStageButton BackStageButton
        {
            get { return (BackStageButton)GetValue(BackStageButtonProperty); }
            set { SetValue(BackStageButtonProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BackStageButton.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BackStageButtonProperty =
            DependencyProperty.Register("BackStageButton", typeof(BackStageButton), typeof(Ribbon));
         /// <summary>
        /// Gets or Sets the IsQATCustomizationDilogEnabled property.
        /// </summary>
        public bool EnableMoreCommands
        {
            get { return (bool)GetValue(EnableMoreCommandsProperty); }
            set { SetValue(EnableMoreCommandsProperty, value); }
        }

         /// <summary>
        /// Defines whether QAT Customization Dialog Window Enabled or not. This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty EnableMoreCommandsProperty =
            DependencyProperty.Register("EnableMoreCommands", typeof(bool), typeof(Ribbon), new PropertyMetadata(true));

        


        public static bool GetShowInMoreCommands(DependencyObject obj)
        {
            return (bool)obj.GetValue(ShowInMoreCommandsProperty);
        }

        public static void SetShowInMoreCommands(DependencyObject obj, bool value)
        {
            obj.SetValue(ShowInMoreCommandsProperty, value);
        }

        // Using a DependencyProperty as the backing store for ShowInMoreCommands.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowInMoreCommandsProperty =
            DependencyProperty.RegisterAttached("ShowInMoreCommands", typeof(bool), typeof(Ribbon), new UIPropertyMetadata(true));




        public Style ContextAdornerStyle
        {
            get { return (Style)GetValue(ContextAdornerStyleProperty); }
            set { SetValue(ContextAdornerStyleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ContextAdornerStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContextAdornerStyleProperty =
            DependencyProperty.Register("ContextAdornerStyle", typeof(Style), typeof(Ribbon), new UIPropertyMetadata(null));

        
        #endregion

        #region Initialization

        /// <summary>
        /// Initializes static members of the <see cref="Ribbon"/> class.
        /// </summary>
        static Ribbon()
        {
            //EnvironmentTest.ValidateLicense(typeof(Ribbon));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Ribbon), new FrameworkPropertyMetadata(typeof(Ribbon)));
            SelectedIndexProperty.OverrideMetadata(typeof(Ribbon), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnSelectedIndexChanged)));
            RibbonContextMenuOpeningEvent = EventManager.RegisterRoutedEvent("RibbonContextMenuOpening", RoutingStrategy.Bubble, typeof(EventHandler), typeof(Ribbon));
            RibbonContextMenuClosingEvent = EventManager.RegisterRoutedEvent("RibbonContextMenuClosing", RoutingStrategy.Bubble, typeof(EventHandler), typeof(Ribbon));
            EventManager.RegisterClassHandler(typeof(UIElement), UIElement.PreviewMouseDownEvent, new MouseButtonEventHandler(OnMouseDown));

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Ribbon"/> class.
        /// </summary>
        public Ribbon()
        {
            if (EnvironmentTestTools.IsSecurityGranted)
            {
                EnvironmentTestTools.StartValidateLicense(typeof(Ribbon));
            }           

            SelectedIndex = 0;
            m_keyTipsCurrentLevel = new Dictionary<string, KeyTip>();
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                if (Application.Current != null && Application.Current.MainWindow != null && !BindingUtils.GetEnableBindingErrors(Application.Current.MainWindow))
                    System.Diagnostics.PresentationTraceSources.DataBindingSource.Switch.Level = System.Diagnostics.SourceLevels.Critical;
            }
            ContextTabGroups = new ContextTabGroupCollection();
            ContextTabGroups.CollectionChanged -= new NotifyCollectionChangedEventHandler(ContextTabGroups_CollectionChanged);

            ContextTabGroups.CollectionChanged += new NotifyCollectionChangedEventHandler(ContextTabGroups_CollectionChanged);

            //EventManager.RegisterClassHandler(typeof(UIElement), UIElement.KeyUpEvent, new KeyEventHandler(OnKeyUp));
            //EventManager.RegisterClassHandler(typeof(UIElement), UIElement.KeyDownEvent, new KeyEventHandler(OnKeyDown));
            //EventManager.RegisterClassHandler(typeof(UIElement), UIElement.PreviewMouseDownEvent, new MouseButtonEventHandler(OnMouseDown));

            CommandBindings.Add(new CommandBinding(RibbonCommands.RemoveItemFromQAT, ProcessRemoveFromQATCommand, CanProcessRemoveFromQATCommand));
            CommandBindings.Add(new CommandBinding(RibbonCommands.AddItemToQAT, ProcessAddToQATCommand, CanProcessAddToQATCommand));
            CommandBindings.Add(new CommandBinding(RibbonCommands.QATMoreCommands, ProcessQATMoreCommandsCommand, CanProcessQATMoreCommandsCommand));
            CommandBindings.Add(new CommandBinding(RibbonCommands.MinimizeRibbon, ProcessMinimizeCommand, CanProcessMinimizeCommand));
            CommandBindings.Add(new CommandBinding(RibbonCommands.PlaceQATAbove, ProcessPlaceQATAboveCommand, CanProcessPlaceQATAboveCommand));
            CommandBindings.Add(new CommandBinding(RibbonCommands.PlaceQATBelow, ProcessPlaceQATBelowCommand, CanProcessPlaceQATBelowCommand));
            this.Loaded -= new RoutedEventHandler(Ribbon_Loaded);
            this.SizeChanged -= new SizeChangedEventHandler(Ribbon_SizeChanged);
            this.Loaded += new RoutedEventHandler(Ribbon_Loaded);
            this.SizeChanged += new SizeChangedEventHandler(Ribbon_SizeChanged);
            qatApplicationItemIndex = new ArrayList();
        }


        /// <summary>
        /// Clears the unloaded elements.
        /// </summary>
        void ClearUnloadedElements()
        {
            Dispose();
        }

        #endregion

        #region Events
        /// <summary>
        /// Event that is raised when RibbonState property is changed.
        /// </summary>
        public event PropertyChangedCallback RibbonStateChanged;

        /// <summary>
        /// Event that is raised when TabPanelItem property is changed.
        /// </summary>
        public event PropertyChangedCallback TabPanelItemChanged;

        /// <summary>
        /// Event that is raised when SelectedIndex property is changed.
        /// </summary>
        public event PropertyChangedCallback SelectedIndexChanged;

        /// <summary>
        /// Event that is raised when HasVisibleContextTabGroup property is changed. 
        /// </summary>
        public event PropertyChangedCallback HasVisibleContextTabGroupChanged;

        /// <summary>
        /// Event that is raised after IsQATBelow property is changed.
        /// </summary>
        public event PropertyChangedCallback IsQATBelowChanged;

        /// <summary>
        /// Cancelable event that is raised before BackStage is opening.
        /// </summary>
        public event CancelEventHandler BackStageOpening;

        /// <summary>
        /// Cancelable event that is raised before BackStage is closing.
        /// </summary>
        public event CancelEventHandler BackStageClosing;

        /// <summary>
        /// Event that is raised after BackStage is opened.
        /// </summary>
        public event EventHandler BackStageOpened;

        /// <summary>
        /// Event that is raised after BackStage is closed.
        /// </summary>
        public event EventHandler BackStageClosed;


        /// <summary>
        /// Cancelable event that is raised before QATCustomizeDialog is opening.
        /// </summary>
        public event CancelEventHandler QATCustomizeDialogOpening;

        /// <summary>
        /// Event that is raised after QATCustomizeDialog is closed.
        /// </summary>
        public event EventHandler QATCustomizeDialogClosed;

        /// <summary>
        /// Occurs before QAT DropDown popup is opened.
        /// </summary>
        public event CancelEventHandler BeforeQatDropDownPopup;

        /// <summary>
        /// Occurs after QAT DropDown popup is opened.
        /// </summary>
        public event EventHandler AfterQatDropDownPopup;


        /// <summary>
        /// Raises when ribbon context menu opening. Handle=true avoid context menu opening.
        /// </summary>
        public static RoutedEvent RibbonContextMenuOpeningEvent;

        /// <summary>
        /// Event that is raised when <see cref=" RibbonContextMenu"/> is Opening.
        /// </summary>
        public event ContextMenuEventHandler RibbonContextMenuOpening;

        internal virtual void FireRibbonContextMenuOpening(ContextMenuEventArgs e)
        {
            if (RibbonContextMenuOpening != null)
            {
                RibbonContextMenuOpening(this, e);
            }
        }

        /// <summary>
        /// Raises when ribbon context menu closing.
        /// </summary>
        public static RoutedEvent RibbonContextMenuClosingEvent;

        /// <summary>
        /// Event that is raised when <see cref="RibbonContextMenu"/> is closing.
        /// </summary>
        public event ContextMenuEventHandler RibbonContextMenuClosing;

        internal virtual void FireRibbonContextMenuClosing(ContextMenuEventArgs e)
        {
            if (RibbonContextMenuClosing != null)
            {
                RibbonContextMenuClosing(this, e);
            }
        }

        /// <summary>
        /// Occurs when item is added or removed from QAT.
        /// </summary>
        public event QATItemsCollectionChangedEventHandler QATItemsCollectionChanged;

        #endregion

        #region Static Methods
        /// <summary>
        /// Calls OnRibbonStateChanged method of the instance, notifies
        /// of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnRibbonStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Ribbon instance = (Ribbon)d;
            instance.OnRibbonStateChanged(e);
        }



        /// <summary>
        /// Coerces RibbonState property.
        /// </summary>
        /// <param name="dObj">Dependency object containing property.</param>
        /// <param name="obj">Object Value</param>
        /// <returns>
        /// Coerced value.
        /// </returns>
        private static object OnRibbonStateCoerce(DependencyObject dObj, object obj)
        {
            RibbonState state = (RibbonState)obj;
            if (state == RibbonState.Adorner && !(dObj as FrameworkElement).IsLoaded)
            {
                return RibbonState.Hide;
            }
            else
            {
                return obj;
            }
        }

        /// <summary>
        /// Calls OnTabPanelItemChanged method of the instance, notifies
        /// of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnTabPanelItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Ribbon instance = (Ribbon)d;
            instance.OnTabPanelItemChanged(e);
        }

        /// <summary>
        /// Contexts the tab groups changed call back.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void ContextTabGroupsChangedCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Ribbon instance = (Ribbon)d;
            instance.AddContextTabGroups();
        }

        #endregion

        #region Override methods
        /// <summary>
        /// Raises the Initialized event. This method is invoked
        /// whenever IsInitialized is set to true internally.
        /// </summary>
        /// <param name="e">The EventArgs that contains the event data.</param>
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Loaded, new ThreadStart(CoerceRibbonState));
            RibbonStateChanged += new PropertyChangedCallback(Ribbon_RibbonStateChanged);
           
            if (QuickAccessToolBar != null)
            {
                QuickAccessToolBar.Ribbon = this;
                QuickAccessToolBar.VisualInitializeCompleete -= new EventHandler(QuickAccessToolBar_VisualInitializeCompleete);
                QuickAccessToolBar.VisualInitializeCompleete += new EventHandler(QuickAccessToolBar_VisualInitializeCompleete);
            }
        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever
        /// application code or internal processes call ApplyTemplate.
        /// </summary>
        public override void OnApplyTemplate()
        {
            Border border;

            if (RootWindow != null)
            {
                RootWindow.Closing -= new CancelEventHandler(RootWindow_Closing);
                RootWindow.Loaded -= new RoutedEventHandler(RootWindow_Loaded);
                RootWindow.SizeChanged -= new SizeChangedEventHandler(RootWindow_SizeChanged);
            }

            m_ribbonWindow = VisualUtils.FindRootVisual(this) as RibbonWindow;

            if (m_ribbonWindow != null && m_ribbonWindow.BackStage==null)
            {
                if (this.DataContext != null)
                {
                    if (this.BackStage != null && this.BackStage.DataContext == null)
                    {
                        this.BackStage.DataContext = this.DataContext;
                    }
                    else if (m_ribbonWindow != null && this.BackStage!=null && m_ribbonWindow.DataContext == this.BackStage.DataContext)
                    {
                            this.BackStage.DataContext = this.DataContext;
                    }
                }
                m_ribbonWindow.BackStage = this.BackStage;
            }

            RootWindow = VisualUtils.FindAncestor(this, typeof(Window)) as Window;
            if (RootWindow != null)
            {
                RootWindow.Closing -= new CancelEventHandler(RootWindow_Closing);
                RootWindow.Loaded -= new RoutedEventHandler(RootWindow_Loaded);
                RootWindow.SizeChanged -= new SizeChangedEventHandler(RootWindow_SizeChanged);
            }
            if (RootWindow != null)
            {
                RootWindow.Closing += new CancelEventHandler(RootWindow_Closing);
                RootWindow.Loaded += new RoutedEventHandler(RootWindow_Loaded);
                RootWindow.SizeChanged += new SizeChangedEventHandler(RootWindow_SizeChanged);
            }
            if (m_ribbonWindow != null)
            {
               
                m_ribbonWindow.PreviewMouseDown -= new MouseButtonEventHandler(m_ribbonWindow_MouseDown);
                 #if !SyncfusionFramework3_5
                m_ribbonWindow.PreviewTouchDown -= new EventHandler<TouchEventArgs>(m_ribbonWindow_PreviewTouchDown);
#endif
                m_ribbonWindow.LostKeyboardFocus -= new KeyboardFocusChangedEventHandler(m_ribbonWindow_LostKeyboardFocus);
                m_ribbonWindow.TitleBar.PreviewMouseLeftButtonDown -= new MouseButtonEventHandler(TitleBar_PreviewMouseLeftButtonDown);
                m_ribbonWindow.KeyDown -= new KeyEventHandler(m_ribbonWindow_KeyDown);
                m_ribbonWindow.SizeChanged -= new SizeChangedEventHandler(m_ribbonWindow_SizeChanged);
                m_ribbonWindow.KeyUp -= new KeyEventHandler(m_ribbonWindow_KeyUp);
                m_ribbonWindow.LocationChanged -= new EventHandler(m_ribbonWindow_LocationChanged);
            }


            if (this.BackStageButton != null)
            {
                this.BackStageButton.parentRibbon = this;
                if (this.BackStageHeader != this.BackStageButton.Header)
                    BindingUtils.SetBinding(this.BackStageButton, this, BackStageButton.HeaderProperty, Ribbon.BackStageHeaderProperty, BindingMode.TwoWay);
            }

            if (this.BackStage != null)
                this.BackStage.parentRibbon = this;

            if (this.Adorner_Popup != null)
            {
                this.Adorner_Popup.Child = null;
                this.Adorner_Popup = null;
            }

            Adorner_Popup = this.GetTemplateChild("Part_AdornerPopup") as Popup;

            if (Adorner_Popup != null && this.RibbonState == RibbonState.Adorner)
            {
                
                ShowAdorned();
                Adorner_Popup.Opened += (sender, e) =>
                    {
                        if (this.SelectedItem != null)
                        {
                            RibbonTab tab = (RibbonTab)ItemContainerGenerator.ContainerFromItem(this.SelectedItem);

                            if (m_keyTipsCurrentLevel != null && m_keyTipPath != null && m_keyTipCurrentLevel.Count <= 0 && tab != null)
                            {
                                m_keyTipPath.Add(Ribbon.GetKeyTip(tab));
                                m_keyTipCurrentLevel=FindKeyTipLevel(m_keyTipPath);

                            }
                            if (m_keyTipCurrentLevel != null && tab != null)
                            {
                                KeyTip keyTip = null;
                                if (m_keyTipCurrentLevel.ContainsKey(Ribbon.GetKeyTip(tab)))
                                    keyTip = m_keyTipCurrentLevel[Ribbon.GetKeyTip(tab)];

                                if (keyTip != null && m_isAdornersShown && keyTip.IsActive)
                                {
                                    HideKeyTips(m_keyTipCurrentLevel.Values);

                                    if (UpdateKeyTips(ref keyTip))
                                    {
                                        m_keyTipPath.Add(keyTip.Text);
                                        ShowKeyTips(keyTip.SubLevel.Values);
                                        KeyTipsCurrentLevel = keyTip.SubLevel;
                                    }
                                }
                            }
                        }
                    };
                Adorner_Popup.Closed -= new EventHandler(Adorner_Popup_Closed);
                Adorner_Popup.Closed += new EventHandler(Adorner_Popup_Closed);
               this.Adorner_Popup.StaysOpen = true;
            }

            this.ribbonToggleButton = this.GetTemplateChild("ToggleButton") as RibbonToggleButton;

            if (ribbonToggleButton != null)
                ribbonToggleButton.parentRibbon = this;

            if (m_ribbonWindow == null)
            {
                border = GetTemplateChild("PART_TitleBorder") as Border;
                border.Visibility = Visibility.Visible;
            }

            else
            {
                m_ribbonWindow.IsMinimalSizeReachedChanged += new PropertyChangedCallback(RibbonWindow_IsMinimalSizeRichedChanged);

                BindingUtils.SetBinding(m_ribbonWindow.TitleBar, this, TitleBar.HasVisibleContextTabGroupProperty, Ribbon.HasVisibleContextTabGroupProperty);
                BindingUtils.SetBinding(this, m_ribbonWindow.TitleBar, Ribbon.VisibilityProperty, TitleBar.RibbonIsVisibleProperty, BindingMode.OneWayToSource, new BooleanToVisibilityConverter());

                if (this.QuickAccessToolBar != null && !this.IsQATBelow)
                {
                    BindingUtils.SetBinding(m_ribbonWindow.TitleBar, this.QuickAccessToolBar, TitleBar.QATColumnWidthProperty, QuickAccessToolBar.ActualWidthProperty);
                }

                if (this.ApplicationMenu != null)
                {
                    BindingUtils.SetBinding(m_ribbonWindow.TitleBar, this.ApplicationMenu, TitleBar.AppMenuColumnWidthProperty, QuickAccessToolBar.ActualWidthProperty);
                }
               
                m_ribbonWindow.PreviewMouseDown += new MouseButtonEventHandler(m_ribbonWindow_MouseDown);
                 #if !SyncfusionFramework3_5
                m_ribbonWindow.PreviewTouchDown += new EventHandler<TouchEventArgs>(m_ribbonWindow_PreviewTouchDown);
#endif
                m_ribbonWindow.LostKeyboardFocus += new KeyboardFocusChangedEventHandler(m_ribbonWindow_LostKeyboardFocus);               
                m_ribbonWindow.TitleBar.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(TitleBar_PreviewMouseLeftButtonDown);
                m_ribbonWindow.KeyDown += new KeyEventHandler(m_ribbonWindow_KeyDown);
                m_ribbonWindow.SizeChanged += new SizeChangedEventHandler(m_ribbonWindow_SizeChanged);
                m_ribbonWindow.KeyUp += new KeyEventHandler(m_ribbonWindow_KeyUp);
                m_ribbonWindow.LocationChanged += new EventHandler(m_ribbonWindow_LocationChanged);
           
            }

            base.OnApplyTemplate();
        }

        void m_ribbonWindow_LocationChanged(object sender, EventArgs e)
        {
            if (this.RibbonState == Tools.RibbonState.Adorner)
                this.RibbonState = Tools.RibbonState.Hide;
        }

        void m_ribbonWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (m_keyTipPath != null)
            {
                foreach (KeyValuePair<string, KeyTip> o in m_keyTips)
				{
					if(!m_keyTipPath.Contains(o.Key))
                    m_keyTipPath.Add(o.Key);
				}
                HideKeyTips();
                m_keyTipPath.Clear();
                m_keyTips.Clear();
                foreach (KeyValuePair<string, KeyTip> o in KeyTipsCurrentLevel)
                {
                    m_keyTipPath.Add(o.Key);
                    m_keyTips.Add(o.Key,o.Value);
                }
                HideKeyTips();
            }
        }

        /// <summary>
        /// Handles the Closed event of the Adorner_Popup control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void Adorner_Popup_Closed(object sender, EventArgs e)
        {
            if (this.RibbonState == RibbonState.Adorner && !m_Qatdropdownclicked)
            {
                RibbonTab selectedTab = this.ItemContainerGenerator.ContainerFromItem(this.SelectedItem) as RibbonTab;

                if (selectedTab != null)
                    selectedTab.m_tabButton.IsChecked = false;

                this.RibbonState = RibbonState.Hide;
            }
        }

        /// <summary>
        /// Handles the SizeChanged event of the RootWindow control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.SizeChangedEventArgs"/> instance containing the event data.</param>
        void RootWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.RibbonState == RibbonState.Adorner)
            {
                RibbonTab selectedTab = this.ItemContainerGenerator.ContainerFromItem(this.SelectedItem) as RibbonTab;

                if (selectedTab != null)
                    selectedTab.m_tabButton.IsChecked = false;

                this.RibbonState = RibbonState.Hide;
            }
        }

        /// <summary>
        /// Handles the MouseDown event of the TitleBar control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        void TitleBar_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this.RibbonState == RibbonState.Adorner)
            {
                RibbonTab selectedTab = this.ItemContainerGenerator.ContainerFromItem(this.SelectedItem) as RibbonTab;

                if (selectedTab != null)
                    selectedTab.m_tabButton.IsChecked = false;

                this.RibbonState = RibbonState.Hide;
            }
        }

        /// <summary>
        /// Handles the Closing event of the RootWindow control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.CancelEventArgs"/> instance containing the event data.</param>
        void RootWindow_Closing(object sender, CancelEventArgs e)
        {
            if (this.AutoPersist || (this.QuickAccessToolBar != null && this.QuickAccessToolBar.AutoPersist) || (this.m_ribbonWindow != null && this.m_ribbonWindow.AutoPersist))
                SaveDefaultState(default_StoreFile);
            if (!e.Cancel)
            {
                ClearUnloadedElements();
            }
        }

        /// <summary>
        /// Handles the Loaded event of the RootWindow control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void RootWindow_Loaded(object sender, RoutedEventArgs e)
        {
            CheckQATInitialItems();
            if (this.AutoPersist || (this.QuickAccessToolBar != null && this.QuickAccessToolBar.AutoPersist) || (this.m_ribbonWindow != null && this.m_ribbonWindow.AutoPersist))
            {
                SaveInitialState();
                LoadDefaultState(default_StoreFile);
                if (Params != null && Params.QATItemsIndex.Count > 0)
                {
                    UpdateQATItems();
                }
                isXMLStateLoading = false;
            }
        }
        /// <summary>
        /// It returns QAT items
        /// </summary>
        /// <returns></returns>
        public List<UIElement> GetQatItemsSources()
        {
            List<UIElement> list = new List<UIElement>(this.QATItems.Values);
            foreach (var item in this.QuickAccessToolBar.Items)
            {
                if (item is QuickAccessToolBarItem)
                    list.Add((item as QuickAccessToolBarItem).SourceElement);
                else if (!this.QATItems.ContainsKey((UIElement)item))
                    list.Add((UIElement)item);
            }
            return list;

        }

        /// <summary>
        /// Checks the QAT initial items.
        /// </summary>
        private void CheckQATInitialItems()
        {
            if (this.QuickAccessToolBar != null)
            {
                foreach (var item in this.QuickAccessToolBar.Items)                
                    if(item is UIElement)
                        this.QATInitialItems.Add((UIElement)item);                
                CheckQATMenuItemsAtInitial();
            }
        }

        private void UpdateQATItems()
        {
            ObservableCollection<int> qatItems = new ObservableCollection<int>();
            if (Params != null)
            {
                foreach (QATItemState obj in Params.QATItemsIndex)
                {
                    qatItems.Add(obj.Index);
                }
            }
            if (qatItems.Count > 0 && this.QuickAccessToolBar!=null)
            {
                for (int i = this.QuickAccessToolBar.Items.Count - 1; i >= 0; i--)
                {
                    if (!qatItems.Contains(i))
                        this.QuickAccessToolBar.Items.RemoveAt(i);
                }

            }
        }

        /// <summary>
        /// Checks the QAT menu items at initial.
        /// </summary>
        private void CheckQATMenuItemsAtInitial()
        {
            foreach (var item in this.QATInitialItems)
            {
                if (item is RibbonButton)
                {
                    RibbonButton button = (RibbonButton)item;

                    var match = from RibbonButton menuItem in this.QuickAccessToolBar.QATMenuItems
                                where RibbonCommandManager.GetSynchronizedItem(menuItem) == RibbonCommandManager.GetSynchronizedItem(button)
                                select menuItem;
                    if (match.Count() > 0)
                        button.IsMenuItem = true;
                }
            }
        }

        /// <summary>
        /// Ribbons the window_ is minimal size reached changed.
        /// </summary>
        /// <param name="d">The d value.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private void RibbonWindow_IsMinimalSizeRichedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue == true)
            {
                this.Visibility = Visibility.Collapsed;

                if (this.ApplicationMenu != null)
                {
                    this.ApplicationMenu.Visibility = Visibility.Collapsed;
                    m_ribbonWindow.TitleBar.AppMenuColumnWidth = 0d;
                }

                if (this.QuickAccessToolBar != null)
                {
                    this.QuickAccessToolBar.Visibility = Visibility.Collapsed;
                    m_ribbonWindow.TitleBar.QATColumnWidth = 0d;
                }
            }
            else
            {
                this.Visibility = Visibility.Visible;

                if (this.ApplicationMenu != null)
                {
                    this.ApplicationMenu.Visibility = Visibility.Visible;
                    BindingUtils.SetBinding(m_ribbonWindow.TitleBar, this.ApplicationMenu, TitleBar.AppMenuColumnWidthProperty, QuickAccessToolBar.ActualWidthProperty);
                }

                if (this.QuickAccessToolBar != null)
                {
                    this.QuickAccessToolBar.Visibility = Visibility.Visible;
                    if (!this.IsQATBelow)
                    {
                        BindingUtils.SetBinding(m_ribbonWindow.TitleBar, this.QuickAccessToolBar, TitleBar.QATColumnWidthProperty, QuickAccessToolBar.ActualWidthProperty);
                    }
                }
            }
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.Property == FrameworkElement.FlowDirectionProperty)
            {
                if (this.HasVisibleContextTabGroup)
                {
                    IEnumerable contextAdorners = VisualUtils.EnumChildrenOfType(this, typeof(ContextAdorner));
                    foreach (var cAdoners in contextAdorners)
                    {
                        (cAdoners as ContextAdorner).FlowDirection = (FlowDirection)(1 - (int)FlowDirection);
                    }
                }
            }

            if (e.Property == SkinStorage.EnableTouchProperty)
            {
                EnableTouch = (bool)e.NewValue;
            }
            base.OnPropertyChanged(e);
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.MouseWheel" />attached
        /// event reaches an element in its route that is derived from
        /// this class. Implement this method to add class handling for
        /// this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseWheelEventArgs" />that
        /// contains the event data.</param>
        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            TabScrolling(e.Delta);

            base.OnMouseWheel(e);
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Keyboard.LostKeyboardFocus" />attached
        /// event reaches an element in its route that is derived from
        /// this class. Implement this method to add class handling for
        /// this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.KeyboardFocusChangedEventArgs" />that
        /// contains event data.</param>
        protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {

            if (RibbonState == RibbonState.Adorner)
            {
                RibbonGallery Rg = null;
                if (e.OldFocus != null && (e.OldFocus as FrameworkElement).TemplatedParent is RibbonGallery)
                {
                    Rg = (e.OldFocus as FrameworkElement).TemplatedParent as RibbonGallery;
                }
                if (!VisualUtils.IsDescendant(this, Keyboard.FocusedElement as DependencyObject) && !(e.OldFocus is RepeatButton || (Rg != null && Rg.bScrollbtnclicked)))
                {
                    RibbonTab selectedTab = this.ItemContainerGenerator.ContainerFromItem(this.SelectedItem) as RibbonTab;

                    if (selectedTab != null)
                    {
                        selectedTab.m_tabButton.IsChecked = false;
                    }

                    if (!(e.OriginalSource is Button || e.OriginalSource is DropDownButton ||m_Qatdropdownclicked) || showBackStage)
                    {
                        this.RibbonState = RibbonState.Hide;
                    }

                }

                if (Keyboard.FocusedElement is RibbonButton /*&& !( Keyboard.FocusedElement is RibbonDropDownButton )*/)
                {
                    (Keyboard.FocusedElement as RibbonButton).Click += new RoutedEventHandler(Ribbon_Click);
                }
            }

            
            HideKeyTips();
           

            base.OnLostKeyboardFocus(e);
        }

        /// <summary>
        /// Updates property value cache and raises TabPanelItemChanged
        /// event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnTabPanelItemChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.TabPanelItem != null)
                WindowChrome.SetIsHitTestVisibleInChrome(TabPanelItem, true);
            if (TabPanelItemChanged != null)
            {
                TabPanelItemChanged(this, e);
            }
        }

        /// <summary>
        /// Called when the source of an item in a selector changes.
        /// </summary>
        /// <param name="oldValue">Old value of the source.</param>
        /// <param name="newValue">New value of the source.</param>
        protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            base.OnItemsSourceChanged(oldValue, newValue);
        }

        /// <summary>
        /// Determines if the specified item is (or is eligible to be) its own container. 
        /// </summary>
        /// <param name="item">The item to check.</param>
        /// <returns>Returns true if the item is (or is eligible to be) its own container; 
        /// otherwise, false. </returns>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is RibbonTab;
        }

        /// <summary>
        /// Creates or identifies the element that is used to display the given item.
        /// </summary>
        /// <returns>The element that is used to display the given item.</returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new RibbonTab();
        }

        /// <summary>
        /// Prepare Container for the particular item.
        /// </summary>
        /// <param name="element">The container element used to display the given item.</param>
        /// <param name="item">The item for which the container has to be prepared.</param>
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            RibbonTab con_Tab;

            if (item is RibbonTab)
            {
                base.PrepareContainerForItemOverride(element, item);
            }
            else
            {
                con_Tab = GetTab(element, item);
                base.PrepareContainerForItemOverride(con_Tab, con_Tab);
            }
        }

        /// <summary>
        /// Gets the tab.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        private RibbonTab GetTab(DependencyObject element, object item)
        {
            RibbonTab returntab = element as RibbonTab;
            if ((null != ItemTemplate || ItemsSource != null) && returntab.Caption.Trim() == String.Empty)
            {
                if (DisplayMemberPath == string.Empty)
                    returntab.m_tabButton.Caption = new string(' ', item.ToString().Length);
                else
                {
                    PropertyDescriptor pDescriptor = DisplayMemberPath!=null ? TypeDescriptor.GetProperties(item)[DisplayMemberPath] :null;
                    if (pDescriptor != null)
                    {
                        object caption = pDescriptor.GetValue(item);
                        if (caption != null)
                        {
                            returntab.m_tabButton.Caption = caption.ToString();
                        }
                    }
                    else
                        returntab.m_tabButton.Caption = item.ToString();
                }
                returntab.m_tabButton.Content = item;
            }
            if (returntab.ItemsSource == null)
            {
                ContentControl content = new ContentControl();
                content.Content = item;
                content.ContentTemplate = returntab.ContentTemplate;
                content.ContentTemplateSelector = returntab.ContentTemplateSelector;
                returntab.Items.Add(content);
            }
            RibbonTab Selectedtab = null;
            if (SelectedIndex != -1 && returntab.ItemsSource != null)
            {
                Selectedtab = ItemContainerGenerator.ContainerFromIndex(SelectedIndex) as RibbonTab;
                if ((Items.IndexOf(item) == SelectedIndex) && (this.RibbonState != RibbonState.Hide))
                {
                    returntab.m_tabButton.IsChecked = true;
                }
            }
            else if (this.ItemsSource != null && SelectedIndex == -1 && Items.IndexOf(item) == 0 && returntab != null)
            {
                returntab.m_tabButton.IsChecked = true;
            }
            return returntab;
        }

        /// <summary>
        /// Invoked when the <see cref="P:System.Windows.Controls.ItemsControl.ItemTemplate"/> property changes.
        /// </summary>
        /// <param name="oldItemTemplate">The old <see cref="P:System.Windows.Controls.ItemsControl.ItemTemplate"/> property value.</param>
        /// <param name="newItemTemplate">The new <see cref="P:System.Windows.Controls.ItemsControl.ItemTemplate"/> property value.</param>
        protected override void OnItemTemplateChanged(DataTemplate oldItemTemplate, DataTemplate newItemTemplate)
        {
            base.OnItemTemplateChanged(oldItemTemplate, newItemTemplate);
        }
        /// <summary>
        /// Invoked when the Items property changes.
        /// </summary>
        /// <param name="e">Information about the change.</param>
        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    RibbonTab ribbonTab = e.NewItems[0] as RibbonTab;
                    if (null != ribbonTab)
                    {
                        if (Items.Count == 1)
                            ribbonTab.IsChecked = true;
                        ribbonTab.AddChildren();
                        //ribbonTab.UpdateLayout();
                        if (ribbonTab.ContextTabGroup == null)
                            ArrangeContextTabGroups();
                    }
                    break;

                case NotifyCollectionChangedAction.Move:
                    break;

                case NotifyCollectionChangedAction.Remove:
                    RibbonTab tab = e.OldItems[0] as RibbonTab;
                    if (null != tab)
                    {
                        ContextTabGroup ctg = tab.ContextTabGroup;
                        tab.UpdateLayout();
                        tab.ContextAdorner = null;

                        if (this.Items.Count <= 0)
                        {
                            if (SelectedTabItem != null && this.SelectedTabItem.ItemsSource == null)
                                this.SelectedTabItem = null;
                        }
                    }
                    else if (ItemsSource != null)
                    {
                        if (Items.Count == 0)
                        {
                            SelectedIndex = -1;
                            SelectedTabItem = null;
                        }
                        else if (Items.Count > 0)
                        {
                            SelectedIndex = Items.Count - 1;
                            RibbonTab item = (RibbonTab)this.ItemContainerGenerator.ContainerFromIndex(SelectedIndex);
                            SelectedTabItem = item;
                        }
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    break;

                case NotifyCollectionChangedAction.Reset:
                    {
                        if (ItemsSource != null)
                        {
                            if (m_TabPanel != null)
                            {
                                if (m_TabPanel.m_visibleChildren != null)
                                {
                                    m_TabPanel.m_visibleChildren.Clear();
                                    m_TabPanel.m_visibleChildren = null;
                                }
                            }
                            if (e.NewItems == null && Items.Count > 0)
                            {
                              //  RefreshItems();   
                            }
                            if (Items.Count == 0)
                            {
                                SelectedIndex = -1;
                                SelectedTabItem = null;
                            }
                            else if (Items.Count >0)
                            {
                                SelectedIndex = 0;
                            }
                        }
                        else
                        {
                            if (Items.Count == 0)
                            {
                                SelectedIndex = -1;
                                SelectedTabItem = null;
                            }
                        }
                    }
                    break;

                default:
                    break;

            }
        }


        /// <summary>
        /// Refreshes the items.
        /// </summary>
        private void RefreshItems()
        {
            IList olditems = ((IList)ItemsSource);
            ArrayList newitems = new ArrayList(olditems);
            foreach (var item in newitems)
            {
                ((IList)ItemsSource).Remove(item);
            }
            foreach (var item in newitems)
            {
                ((IList)ItemsSource).Add(item);
            }
        }


        /// <summary>
        /// Shows the key tips.
        /// </summary>
        internal void ShowKeyTips()
        {
            if (this.ApplicationMenu != null && this.ApplicationMenu.IsPopupOpen)
            {
               // ApplicationMenu.IsPopupOpen = false;
                return;
            }

            InitializeKeyTips();
            ShowKeyTips(m_keyTips.Values);
            m_isAdornersShown = true;
        }

        /// <summary>
        /// Hides the key tips.
        /// </summary>
        internal void HideKeyTips()
        {
            if (m_keyTipPath != null)
            {
                HideKeyTips(FindKeyTipLevel(m_keyTipPath).Values);
                if (this.isCheckedToggleButton)
                    KeyTipsCurrentLevel.Clear();                
                m_isAdornersShown = false;
            }


            if (m_dropDownbutton != null && m_dropDownbutton.IsDropDownOpen)
            {
                //m_dropDownbutton.IsDropDownOpen = false;
            }
            if (m_splitMenuButton != null && m_splitMenuButton.IsMenuOpen)
            {
                //m_splitMenuButton.IsMenuOpen = false;
            }
        }

        /// <summary>
        /// Raises the KeyUp event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A KeyEventArgs that contains the event data.</param>
        private void m_ribbonWindow_KeyUp(object sender, KeyEventArgs e)
        {
            bool isRibbonBarCollasped = false;
            if (e.Key == Key.Tab)
            {
                isShowKeyTip = false;
                isShowKeyTipWithCollection = false;
                isRibbonBarCollasped = true;
                HideKeyTips();
            }
            else
            {
            if (BackStageButton != null && BackStageButton.IsOpen)
               {
                   isShowKeyTip = false;
                   isShowKeyTipWithCollection = false;
                   return;
               }
            if (e.Key == Key.System)
            {
                RibbonItemsControl rItemsControl = VisualUtils.FindDescendant(this as Visual, typeof(RibbonItemsControl)) as RibbonItemsControl;
                if (rItemsControl != null)
                {
                    if (rItemsControl.IsDropDownOpen)
                    {
                        rItemsControl.IsDropDownOpen = false;
                        isRibbonBarCollasped = true;
                    }

                    if (ApplicationMenu != null && ApplicationMenu.closekeytip)
                    {
                        isShowKeyTip = false;
                        ApplicationMenu.closekeytip = false;
                    }
                }
            }
            if (!isRibbonBarCollasped && e.Key != Key.Escape)
            {
                //Case 22768: dont call ShowKeyTips() if detecting a keyup different from alt, without alt pressed and with keytips not already showed
                isShowKeyTip = isShowKeyTip && (m_isAdornersShown || e.SystemKey == Key.LeftAlt || e.SystemKey == Key.RightAlt || e.SystemKey == Key.F10 || Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt));
                if (isShowKeyTip)
                {
                    ShowKeyTips();
                }
                else if (isShowKeyTipWithCollection && keyCollection != null)
                {
                    ShowKeyTips(keyCollection.Values);
                }
                isShowKeyTip = false;
                isShowKeyTipWithCollection = false;
            }
            if ((e.Key == Key.System) && ((e.SystemKey == Key.LeftAlt) || (e.SystemKey == Key.RightAlt)))
            {
                e.Handled = true;
            }
            if (e.Key == Key.Escape)
            {
                HideKeyTips();
            }
            base.OnKeyUp(e);
          }
        }

        bool isShowKeyTip = false;
        bool isShowKeyTipWithCollection = false;
        bool isSystemKey = false;
        KeyTip selectedTabKeyTip = null;
        Dictionary<string, KeyTip> keyCollection = null;

        /// <summary>
        /// Raises the KeyDown event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A KeyEventArgs that contains the event
        /// data.</param>
        private void m_ribbonWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (m_ribbonWindow != null && m_ribbonWindow.IsActive)
            {
                if (m_isAdornersShown)
                {
                    e.Handled = true;
                }
                if (BackStageButton != null && BackStageButton.IsOpen)
                {
                    isShowKeyTip = false;
                    isShowKeyTipWithCollection = false;
                    return;
                }
                if (BackStageButton != null && e.Key == Key.Right && !BackStageButton.IsOpen && ((e.Source is RibbonTab) || (e.Source is Ribbon)))
                {
                    HideKeyTips();
                    var temp = this.SelectedIndex;
                    if (temp < this.Items.Count)
                    {
                        for (int i = temp + 1; i != this.SelectedIndex; i++)
                        {                            
                            if (i >= 0 && i <= this.Items.Count - 1)
                            {
                                RibbonTab tab = temp <= this.Items.Count - 1 ? this.ItemContainerGenerator.ContainerFromIndex(i) as RibbonTab : null;
                                if (tab != null && tab.IsEnabled && tab.Visibility == Visibility.Visible)
                                {
                                    temp = i;
                                    break;
                                }                                
                            }
                            if (i >= this.Items.Count - 1)
                            {
                                i = -1;
                            }
                        }

                        this.SelectedIndex = temp;
                    }
                }
                else if (BackStageButton == null && e.Key == Key.Right && ((e.Source is RibbonTab) || (e.Source is Ribbon)))
                {
                    HideKeyTips();
                    var temp = this.SelectedIndex;
                    if (temp < this.Items.Count)
                    {
                        for (int i = temp + 1; i != this.SelectedIndex; i++)
                        {
                            if (i >= 0 && i <= this.Items.Count - 1)
                            {
                                RibbonTab tab = temp <= this.Items.Count - 1 ? this.ItemContainerGenerator.ContainerFromIndex(i) as RibbonTab : null;
                                if (tab != null && tab.IsEnabled && tab.Visibility == Visibility.Visible)
                                {
                                    temp = i;
                                    break;
                                }
                            }
                            if (i >= this.Items.Count - 1)
                            {
                                i = -1;
                            }
                        }

                        this.SelectedIndex = temp;
                    }
                }

                if (BackStageButton != null && e.Key == Key.Left && !BackStageButton.IsOpen && ((e.Source is RibbonTab) || (e.Source is Ribbon)))
                {
                    HideKeyTips();
                    var temp = this.SelectedIndex;
                    if (temp >= 0)
                    {
                        for (int i = temp - 1; i != this.SelectedIndex; i--)
                        {                            
                            if (i >= 0 && i <= this.Items.Count - 1)
                            {
                                RibbonTab tab = temp >= 0 ? this.ItemContainerGenerator.ContainerFromIndex(i) as RibbonTab : null;

                                if (tab != null && tab.IsEnabled && tab.Visibility == Visibility.Visible)
                                {
                                    temp = i;
                                    break;
                                }                                
                            }
                            if (i <= 0)
                            {
                                i = this.Items.Count;
                            }
                        }

                        this.SelectedIndex = temp;
                    }
                }
                else if (BackStageButton == null && e.Key == Key.Left && ((e.Source is RibbonTab) || (e.Source is Ribbon)))
                {
                    HideKeyTips();
                    var temp = this.SelectedIndex;
                    if (temp >= 0)
                    {
                        for (int i = temp - 1; i != this.SelectedIndex; i--)
                        {
                            if (i >= 0 && i <= this.Items.Count - 1)
                            {
                                RibbonTab tab = temp >= 0 ? this.ItemContainerGenerator.ContainerFromIndex(i) as RibbonTab : null;

                                if (tab != null && tab.IsEnabled && tab.Visibility == Visibility.Visible)
                                {
                                    temp = i;
                                    break;
                                }
                            }
                            if (i <= 0)
                            {
                                i = this.Items.Count;
                            }
                        }

                        this.SelectedIndex = temp;
                    }
                }

                if (e.SystemKey >= Key.F1 && e.SystemKey!=Key.F10 && e.SystemKey <= Key.F12) 
                {
                    return;
                }

                if (e.Key >= Key.F1 && e.Key <= Key.F12)
                {
                    return;
                }
                if (m_ribbonWindow != null && !m_ribbonWindow.IsMinimalSizeReached && RibbonState != RibbonState.Adorner)
                {
                    if (e.IsRepeat && (e.SystemKey == Key.LeftAlt || e.SystemKey == Key.RightAlt || e.SystemKey == Key.F10))
                    {
                        TimeSpan delta = m_startTime - DateTime.Now;
                        if (~delta.Milliseconds >= 500)
                        {

                            //ShowKeyTips();
                            isShowKeyTip = true;
                            m_altWasPressed = true;

                            e.Handled = true;
                            return;
                        }
                    }
                    else
                    {
                        m_startTime = DateTime.Now;
                    }
                }

                if (m_ribbontabflag)
                {
                    KeyTip keyTip;
                    InitializeKeyTips();
                    m_keyTipPath = new List<string>();
                    char keyName = e.Key.ToString().ToUpper()[e.Key.ToString().Length - 1];
                    m_keyTipCurrentLevel = FindKeyTipLevel(m_keyTipPath);

                    if (!(e.SystemKey == Key.LeftAlt || e.SystemKey == Key.RightAlt))
                    {
                        m_complexKeyTipString += e.Key.ToString().ToUpper().Contains("D") && e.Key.ToString().Length == 2 ? e.Key.ToString().Substring(1, 1) : e.Key.ToString().ToUpper();
                    }

                    foreach (string key in m_keyTipCurrentLevel.Keys)
                    {
                        if (e.SystemKey == Key.LeftAlt || e.SystemKey == Key.RightAlt)
                        {
                            break;
                        }

                        keyTip = (KeyTip)m_keyTipCurrentLevel[key];
                        if (ModifierKeys.Alt == Keyboard.Modifiers)
                        {
                            if (keyTip.Host is RibbonTab && !keyTip.IsComplex && key[0] == keyName && keyTip.IsActive && m_ribbontabflag)
                            {
                                selectedTabKeyTip = keyTip;
                                InvokeKeyTip(keyTip);
                                m_complexKeyTipString = string.Empty;
                            }
                            else if (keyTip.Host is RibbonTab && keyTip.IsComplex && key.Equals(m_complexKeyTipString))
                            {
                                selectedTabKeyTip = keyTip;
                                InvokeKeyTip(keyTip);
                                m_complexKeyTipString = string.Empty;
                            }
                        }
                    }
                }
            }
            if (m_ribbonWindow != null && m_ribbonWindow.IsActive)
            {
                if (e.Key == Key.F1 && Keyboard.Modifiers == ModifierKeys.Control)
                {
                    MinimizeRibbon();
                    e.Handled = true;
                }

                if (m_ribbonWindow != null && !m_ribbonWindow.IsMinimalSizeReached)
                {
                    KeyTip keyTip;
                    if (ModifierKeys.Alt == Keyboard.Modifiers)
                    {
                        if (e.SystemKey == Key.Space)
                        {
                            isShowKeyTip = false;
                            IntPtr handle = new WindowInteropHelper(m_ribbonWindow).Handle;
                            WindowInterop.RECT rect = new WindowInterop.RECT();
                            WindowInterop.GetWindowRect(handle, ref rect);
                            Point point = new Point(rect.left + 8, rect.top + 24);

                IntPtr hMenu = WindowInterop.GetSystemMenu(handle, false);
                if (m_ribbonWindow.WindowState == System.Windows.WindowState.Maximized)
                {
                    WindowInterop.EnableMenuItem(hMenu, (uint)WindowInterop.GetMenuItemID(hMenu, 0), 0x000);                    
                    WindowInterop.EnableMenuItem(hMenu, (uint)WindowInterop.GetMenuItemID(hMenu, 1), 0x001);
                    WindowInterop.EnableMenuItem(hMenu, (uint)WindowInterop.GetMenuItemID(hMenu, 2), 0x001);
                    WindowInterop.EnableMenuItem(hMenu, (uint)WindowInterop.GetMenuItemID(hMenu, 3), 0x000);
                    WindowInterop.EnableMenuItem(hMenu, (uint)WindowInterop.GetMenuItemID(hMenu, 4), 0x001);
                }
                else if (m_ribbonWindow.WindowState == System.Windows.WindowState.Minimized)
                {
                    WindowInterop.EnableMenuItem(hMenu, (uint)WindowInterop.GetMenuItemID(hMenu, 0), 0x000);
                    WindowInterop.EnableMenuItem(hMenu, (uint)WindowInterop.GetMenuItemID(hMenu, 1), 0x001);                    
                    WindowInterop.EnableMenuItem(hMenu, (uint)WindowInterop.GetMenuItemID(hMenu, 2), 0x001);
                    WindowInterop.EnableMenuItem(hMenu, (uint)WindowInterop.GetMenuItemID(hMenu, 3), 0x001);
                    WindowInterop.EnableMenuItem(hMenu, (uint)WindowInterop.GetMenuItemID(hMenu, 4), 0x000);
                }
                else
                {
                    WindowInterop.EnableMenuItem(hMenu, (uint)WindowInterop.GetMenuItemID(hMenu, 0), 0x001);
                    WindowInterop.EnableMenuItem(hMenu, (uint)WindowInterop.GetMenuItemID(hMenu, 1), 0x000);
                    WindowInterop.EnableMenuItem(hMenu, (uint)WindowInterop.GetMenuItemID(hMenu, 2), 0x000);
                    WindowInterop.EnableMenuItem(hMenu, (uint)WindowInterop.GetMenuItemID(hMenu, 3), 0x000);                    
                    WindowInterop.EnableMenuItem(hMenu, (uint)WindowInterop.GetMenuItemID(hMenu, 4), 0x000);
                }
                if (m_ribbonWindow.ResizeMode == ResizeMode.NoResize)
                {
                    WindowInterop.EnableMenuItem(hMenu, (uint)WindowInterop.GetMenuItemID(hMenu, 0), 0x001);
                    WindowInterop.EnableMenuItem(hMenu, (uint)WindowInterop.GetMenuItemID(hMenu, 2), 0x001);
                    WindowInterop.EnableMenuItem(hMenu, (uint)WindowInterop.GetMenuItemID(hMenu, 3), 0x001);
                    WindowInterop.EnableMenuItem(hMenu, (uint)WindowInterop.GetMenuItemID(hMenu, 4), 0x001);
                }
                if (m_ribbonWindow.ResizeMode == ResizeMode.CanMinimize)
                {
                    WindowInterop.EnableMenuItem(hMenu, (uint)WindowInterop.GetMenuItemID(hMenu, 0), 0x001);
                    WindowInterop.EnableMenuItem(hMenu, (uint)WindowInterop.GetMenuItemID(hMenu, 2), 0x001);                    
                    WindowInterop.EnableMenuItem(hMenu, (uint)WindowInterop.GetMenuItemID(hMenu, 4), 0x001);
                }

                WindowInterop.ShowSystemMenu(handle, point);
                            e.Handled = true;
                            return;
                        }
                    }
                    if (e.SystemKey == Key.LeftAlt || e.SystemKey == Key.RightAlt || e.SystemKey == Key.F10)
                    {
                        if (this.RibbonState == RibbonState.Adorner)
                        {
                            RibbonTab selectedTab = this.ItemContainerGenerator.ContainerFromItem(this.SelectedItem) as RibbonTab;

                            if (selectedTab != null)
                                selectedTab.m_tabButton.IsChecked = false;
                            HideKeyTips();
                            this.RibbonState = RibbonState.Hide;
                            return;
                        }

                        if (m_dropDownbutton != null && m_dropDownbutton.IsDropDownOpen)
                        {
                            m_dropDownbutton.IsDropDownOpen = false;
                            m_altWasPressed = false;
                            m_isAdornersShown = false;
                            e.Handled = true;
                            return;
                        }

                        if (m_splitMenuButton != null && m_splitMenuButton.IsMenuOpen)
                        {
                            m_splitMenuButton.IsMenuOpen = false;
                            m_altWasPressed = false;
                            m_isAdornersShown = false;
                            e.Handled = true;
                            return;
                        }

                        if (!m_altWasPressed && !m_isAdornersShown)
                        {
                           // ShowKeyTips();
                            isShowKeyTip = true;
                            m_altWasPressed = true;
                        }

                        if (!m_altWasPressed && m_isAdornersShown)
                        {

                            HideKeyTips();
                            HideRibbonTab();
                        }

                        m_altWasPressed = false;
                        m_keyTipPath.Clear();
                        m_keyTipComplexPath = string.Empty;
                        //e.Handled = true;
                        //return;
                    }

                    if (e.Key == Key.Escape && m_isAdornersShown)
                    {
                        LevelUpKeyTips();
                        HideRibbonTab();
                        HideAppMenu();
                        e.Handled = true;
                        return;
                    }

                    if (dropdown != null && e.Key == Key.Escape && !m_isAdornersShown && dropdown.IsDropDownOpen)
                    {                        
                        dropdown.IsDropDownOpen = false;
                        e.Handled = true;
                        return;
                    }

                    if ((m_isAdornersShown || e.Key == Key.System)&& m_keyTipComplexPath != null)
                    {
                        char keyName;

                        if (e.Key != Key.System)
                        {
                            keyName = e.Key.ToString()[e.Key.ToString().Length - 1];
                        }
                        else
                        {
                            keyName = e.SystemKey.ToString()[e.SystemKey.ToString().Length - 1];
                            isSystemKey = true;
                        }

                        m_keyTipCurrentLevel = FindKeyTipLevel(m_keyTipPath);
                        //IsKeySelected = true;

                        foreach (string key in m_keyTipCurrentLevel.Keys)
                        {

                            keyTip = (KeyTip)m_keyTipCurrentLevel[key];
                            if (!keyTip.IsComplex && key.Length > 0 && key[0] == keyName && keyTip.IsActive)
                            {
                                if (keyTip.Text.Length == m_keyTipComplexPath.Length + 1)
                                {
                                    if (keyTip.Host is RibbonTab)
                                        selectedTabKeyTip = keyTip;
                                    if (e.Key != Key.Left && e.Key != Key.Right)
                                        InvokeKeyTip(keyTip);
                                    if ((keyTip.Host is SplitMenuButton && KeyTip.GetSplitMenuKeyTip(keyTip.Host) == keyTip.Text) || keyTip.Host is DropDownButton || keyTip.Host is ApplicationMenu || (keyTip.Host is RibbonBar && (keyTip.Host as FrameworkElement).Parent is QuickAccessToolBar) || keyTip.Host is RibbonGallery)
                                    {
                                        e.Handled = true;
                                        return;
                                    }

                                    if (!(keyTip.Host is RibbonTab))
                                    {
                                        HideKeyTips(m_keyTipCurrentLevel.Values);
                                        KeyTipsCurrentLevel.Clear();
                                        m_keyTipPath.Clear();
                                        m_keyTipComplexPath = string.Empty;
                                        e.Handled = true;
                                        return;
                                    }
                                }
                                else
                                {
                                    continue;
                                }
                                IsKeySelected = false;

                                HideKeyTips(m_keyTipCurrentLevel.Values);
                                if (UpdateKeyTips(ref keyTip))
                                {
                                    m_keyTipPath.Add(keyTip.Text);
                                    isShowKeyTipWithCollection = true;
                                    keyCollection = keyTip.SubLevel;
                                   // ShowKeyTips(keyTip.SubLevel.Values);

                                    KeyTipsCurrentLevel = keyTip.SubLevel;
                                }
                                else
                                {
                                    InvokeKeyTip(keyTip);
                                    KeyTipsCurrentLevel.Clear();
                                    m_keyTipPath.Clear();
                                    m_keyTipComplexPath = string.Empty;
                                    e.Handled = true;
                                    return;
                                }
                            }

                            if (keyTip.IsComplex)
                            {
                                int index, complexLength;
                                if (m_keyTipComplexPath == string.Empty)
                                {
                                    index = 0;
                                    complexLength = 1;
                                }
                                else
                                {
                                    if (!m_complexLetterAdded)
                                    {
                                        complexLength = m_keyTipComplexPath.Length + 1;
                                        index = m_keyTipComplexPath.Length;
                                    }
                                    else
                                    {
                                        complexLength = m_keyTipComplexPath.Length;
                                        index = m_keyTipComplexPath.Length - 1;
                                    }
                                }

                                if (key.Length == complexLength)
                                {
                                    if (key == m_keyTipComplexPath + keyName)
                                    {
                                        if (keyTip.Host is RibbonTab)
                                        {
                                            selectedTabKeyTip = keyTip;
                                            InvokeKeyTip(keyTip);
                                            HideKeyTips(m_keyTipCurrentLevel.Values);
                                            if (UpdateKeyTips(ref keyTip))
                                            {
                                                m_keyTipPath.Add(keyTip.Text);
                                                isShowKeyTipWithCollection = true;
                                                keyCollection = keyTip.SubLevel;
                                                //ShowKeyTips(keyTip.SubLevel.Values);
                                                KeyTipsCurrentLevel = keyTip.SubLevel;
                                            }
                                        }
                                        else
                                        {
                                            if ((keyTip.Host is SplitMenuButton && KeyTip.GetSplitMenuKeyTip(keyTip.Host) == keyTip.Text) || keyTip.Host is DropDownButton || keyTip.Host is ApplicationMenu ||(keyTip.Host is RibbonBar) || (keyTip.Host is RibbonBar && (keyTip.Host as FrameworkElement).Parent is QuickAccessToolBar))
                                            {
                                                HideKeyTips(m_keyTipCurrentLevel.Values);
                                                if (keyTip.Host is RibbonBar)
                                                {
                                                    RibbonBar rBar = keyTip.Host as RibbonBar;
                                                    if (rBar.KeyTipOnCollapsed == keyTip.Text && rBar.PanelState != RibbonBarState.Collapsed && rBar.Parent is RibbonTab)
                                                    {
                                                        if (selectedTabKeyTip != null &&  selectedTabKeyTip.Host == rBar.Parent)
                                                        {
                                                            isShowKeyTipWithCollection = true;
                                                            keyCollection = selectedTabKeyTip.SubLevel;
                                                        }
                                                    }
                                                    else
                                                        InvokeKeyTip(keyTip);
                                                }
                                                else
                                                    InvokeKeyTip(keyTip);
                                            }
                                            else if (keyTip.Host is RibbonGallery)
                                            {
                                                HideKeyTips(m_keyTipCurrentLevel.Values);
                                                InvokeKeyTip(keyTip);
                                            }
                                            else
                                            {
                                                HideKeyTips(m_keyTipCurrentLevel.Values);
                                                InvokeKeyTip(keyTip);
                                                m_keyTipPath.Clear();
                                                KeyTipsCurrentLevel.Clear();
                                            }
                                        }
                                        m_keyTipComplexLevel.Clear();
                                        m_keyTipComplexPath = string.Empty;
                                        m_complexLetterAdded = false;
                                        e.Handled = true;
                                        return;
                                    }
                                }
                                else
                                {
                                    if (index < key.Length && key.Substring(0, m_keyTipComplexPath.Length) == m_keyTipComplexPath && key[index] == keyName && e.Key != Key.System)
                                    {
                                        m_keyTipComplexLevel.Add(keyTip.Text, keyTip);

                                        if (!m_complexLetterAdded)
                                        {
                                            string nextLetter = new string(keyTip.Text[m_keyTipComplexPath.Length], 1);
                                            m_keyTipComplexPath = m_keyTipComplexPath.Insert(m_keyTipComplexPath.Length, nextLetter);
                                            m_complexLetterAdded = true;
                                        }
                                    }
                                }
                            }
                        }

                        if (m_complexLetterAdded)
                        {
                            ProcessComplexKeyTips();
                            IsKeySelected = false;
                        }

                        if (m_keyTipComplexPath == string.Empty && IsKeySelected)
                        {
                            m_keyTipPath.Clear();

                            LevelUpKeyTips();
                            IsKeySelected = false;
                           // e.Handled = true;
                            return;
                        }
                    }
                    else
                    {
                        if (m_keyTipCurrentLevel != null)
                        {
                            HideKeyTips(m_keyTipCurrentLevel.Values);
                            KeyTipsCurrentLevel.Clear();
                        }
                       
                        return;
                    }


                    //e.Handled = true;
                }
            }
            isSystemKey = false;
            base.OnKeyDown(e);
        }

        /// <summary>
        /// Handles the MouseDown event of the m_ribbonWindow control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        void m_ribbonWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var ribbonTouch = VisualUtils.FindAncestor(this, typeof(Ribbon)) as Ribbon;
            if (e.StylusDevice == null || (ribbonTouch!=null && !ribbonTouch.EnableTouch))
            {
                if (e.Source is Visual && VisualUtils.FindLogicalAncestor((e.Source as FrameworkElement), typeof(Ribbon)) is Ribbon)
                {
                    mousePoint = PermissionHelper.GetSafePointToScreen(e.Source as Visual, new Point(0, 0));
                }
                if ((!(e.Source is Ribbon) && !(VisualUtils.FindLogicalAncestor((e.Source as FrameworkElement), typeof(Ribbon)) is Ribbon)) && this.RibbonState == RibbonState.Adorner)
                {
                    RibbonTab selectedTab = this.ItemContainerGenerator.ContainerFromItem(this.SelectedItem) as RibbonTab;

                    if (selectedTab != null)
                        selectedTab.m_tabButton.IsChecked = false;
                    if(!(e.Source is UserControl))
                        this.RibbonState = RibbonState.Hide;
                }
                else if ((VisualUtils.FindLogicalAncestor((e.Source as FrameworkElement), typeof(Ribbon)) is Ribbon) && PresentationSource.FromVisual(this) != null)
                {
                    mousePoint = PermissionHelper.GetSafePointToScreen(e.Source as Visual, new Point(0, 0));
                    Point ribbonCurrentPoint = this.PointToScreen(new Point(0.0, 0.0));
                    double primaryScreenWidth = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
                    double finalWidth = primaryScreenWidth - ribbonCurrentPoint.X;
                    /*
                    if (System.Windows.Forms.Screen.AllScreens.Count() > 1)
                    {
                        if (ribbonCurrentPoint.X < 0)
                        {
                            finalWidth = primaryScreenWidth + ribbonCurrentPoint.X;
                            if (mousePoint.X < 0)
                            {
                                if ((-ribbonCurrentPoint.X) < this.ActualWidth)
                                {
                                    if (e.Source is RibbonTab && tabonwindow1)
                                    {
                                        this.RibbonState = RibbonState.Hide;
                                    }
                                }
                            }
                            else
                            {
                                if (e.Source is RibbonTab && tabonwindow2)
                                {
                                    this.RibbonState = RibbonState.Hide;
                                }
                            }
                        }
                        else
                        {
                            if (mousePoint.X > (primaryScreenWidth - 60) && finalWidth > 0)
                            {
                                if (e.Source is RibbonTab)
                                    this.RibbonState = RibbonState.Hide;
                            }
                            else if (finalWidth > 0 && finalWidth < this.ActualWidth / 2)
                            {
                                if (e.Source is RibbonTab)
                                    this.RibbonState = RibbonState.Hide;
                            }
                        }
                    }
                    */
                }

                if (m_isAdornersShown && m_keyTipPath != null && !(VisualUtils.FindLogicalAncestor((e.Source as FrameworkElement), typeof(Ribbon)) is Ribbon))
                {
                    HideKeyTips(FindKeyTipLevel(m_keyTipPath).Values);
                    HideRibbonTab();
                    KeyTipsCurrentLevel.Clear();
                    m_isAdornersShown = false;
                    m_altWasPressed = false;
                }

                base.OnPreviewMouseDown(e);
            }
        }

        #if !SyncfusionFramework3_5
        void m_ribbonWindow_PreviewTouchDown(object sender, TouchEventArgs e)
        {
            if (this.AreAnyTouchesOver)
            {
                if (e.Source is Visual && VisualUtils.FindLogicalAncestor((e.Source as FrameworkElement), typeof(Ribbon)) is Ribbon)
                {
                    mousePoint = PermissionHelper.GetSafePointToScreen(e.Source as Visual, new Point(0, 0));
                }
                if ((!(e.Source is Ribbon) && !(VisualUtils.FindLogicalAncestor((e.Source as FrameworkElement), typeof(Ribbon)) is Ribbon)) && this.RibbonState == RibbonState.Adorner)
                {
                    RibbonTab selectedTab = this.ItemContainerGenerator.ContainerFromItem(this.SelectedItem) as RibbonTab;

                    if (selectedTab != null)
                        selectedTab.m_tabButton.IsChecked = false;

                    this.RibbonState = RibbonState.Hide;
                }
                else if ((VisualUtils.FindLogicalAncestor((e.Source as FrameworkElement), typeof(Ribbon)) is Ribbon) && PresentationSource.FromVisual(this) != null)
                {
                    mousePoint = PermissionHelper.GetSafePointToScreen(e.Source as Visual, new Point(0, 0));
                    Point ribbonCurrentPoint = this.PointToScreen(new Point(0.0, 0.0));
                    double primaryScreenWidth = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
                    double finalWidth = primaryScreenWidth - ribbonCurrentPoint.X;
                    /*
                    if (System.Windows.Forms.Screen.AllScreens.Count() > 1)
                    {
                        if (ribbonCurrentPoint.X < 0)
                        {
                            finalWidth = primaryScreenWidth + ribbonCurrentPoint.X;
                            if (mousePoint.X < 0)
                            {
                                if ((-ribbonCurrentPoint.X) < this.ActualWidth)
                                {
                                    if (e.Source is RibbonTab && tabonwindow1)
                                    {
                                        this.RibbonState = RibbonState.Hide;
                                    }
                                }
                            }
                            else
                            {
                                if (e.Source is RibbonTab && tabonwindow2)
                                {
                                    this.RibbonState = RibbonState.Hide;
                                }
                            }
                        }
                        else
                        {
                            if (mousePoint.X > (primaryScreenWidth - 60) && finalWidth > 0)
                            {
                                if (e.Source is RibbonTab)
                                    this.RibbonState = RibbonState.Hide;
                            }
                            else if (finalWidth > 0 && finalWidth < this.ActualWidth / 2)
                            {
                                if (e.Source is RibbonTab)
                                    this.RibbonState = RibbonState.Hide;
                            }
                        }
                    }
                    */
                }

                if (m_isAdornersShown && m_keyTipPath != null && !(VisualUtils.FindLogicalAncestor((e.Source as FrameworkElement), typeof(Ribbon)) is Ribbon))
                {
                    HideKeyTips(FindKeyTipLevel(m_keyTipPath).Values);
                    HideRibbonTab();
                    KeyTipsCurrentLevel.Clear();
                    m_isAdornersShown = false;
                    m_altWasPressed = false;
                }
            }
        }

#endif

        void m_ribbonWindow_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (BackStage != null && !BackStage.isLostFocus)
                HideKeyTips();
        }
        /// <summary>
        /// Called when [mouse down].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private static void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            var ribbon = e.Source as Ribbon;
            if ((ribbon != null && e.StylusDevice == null )|| (ribbon!=null && !ribbon.EnableTouch))
                ribbon.OnMouseDown(e);
        }

        private new void OnMouseDown(MouseButtonEventArgs e)
        {
            if (m_isAdornersShown && m_keyTipPath != null)
            {
                this.Dispatcher.BeginInvoke(
                    new Action(
                        () =>
                        {
                            HideKeyTips(FindKeyTipLevel(m_keyTipPath).Values);
                            HideRibbonTab();
                            KeyTipsCurrentLevel.Clear();
                            m_isAdornersShown = false;
                            m_altWasPressed = false;
                        }
            ));
            }
        }

        #if !SyncfusionFramework3_5
        protected override void OnPreviewTouchDown(TouchEventArgs e)
        {
            if (this.EnableTouch)
                this.OnTouchDown(e);
        }

#endif

        #if !SyncfusionFramework3_5
        private new void OnTouchDown(TouchEventArgs e)
        {
            if (m_isAdornersShown && m_keyTipPath != null)
            {
                this.Dispatcher.BeginInvoke(
                    new Action(
                        () =>
                        {
                            HideKeyTips(FindKeyTipLevel(m_keyTipPath).Values);
                            HideRibbonTab();
                            KeyTipsCurrentLevel.Clear();
                            m_isAdornersShown = false;
                            m_altWasPressed = false;
                        }
            ));
            }
        }
#endif

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.UIElement.MouseRightButtonUp"/>�routed event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs"/> that contains the event data. The event data reports that the right mouse button was released.</param>
        protected override void OnMouseRightButtonUp(MouseButtonEventArgs e)
        {
            if (e.StylusDevice == null || (!this.EnableTouch))
            {
                base.OnMouseRightButtonUp(e);

                if (!VisualUtils.IsDescendant(this.ApplicationMenu, e.OriginalSource as DependencyObject))
                {
                    RibbonContextMenu.CreateContextMenu(this);
                }
            }
        }

        #if !SyncfusionFramework3_5
        protected override void OnTouchUp(TouchEventArgs e)
        {
            if (this.EnableTouch && systemGesture == SystemGesture.RightTap)
            {
                base.OnTouchUp(e);

                if (!VisualUtils.IsDescendant(this.ApplicationMenu, e.OriginalSource as DependencyObject))
                {
                    RibbonContextMenu.CreateContextMenu(this);
                }
            }
        }
#endif

        protected override void OnStylusSystemGesture(StylusSystemGestureEventArgs e)
        {
            systemGesture = e.SystemGesture;
            base.OnStylusSystemGesture(e);
        }

        /// <summary>
        /// Raises the <see cref="E:SelectedIndexChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnSelectedIndexChanged(DependencyPropertyChangedEventArgs e)
        {
            if (SelectedIndexChanged != null)
            {
                SelectedIndexChanged(this, e);
            }

            if (SelectedIndex != -1)
            {
                Items.MoveCurrentToPosition(SelectedIndex);
                RibbonTab item = (RibbonTab)this.ItemContainerGenerator.ContainerFromIndex(SelectedIndex);
                if (item != null)
                {
                    SelectedTabItem = item;

                    if (isItemLoaded)
                    {
                        item.IsChecked = true;
                        if (this.RibbonState == RibbonState.Hide && item.IsCancelRibbonState == false)
                        {
                            // this.RibbonState = RibbonState.Adorner;
                        }
                    }
                    else
                    {
                        if (item.IsChecked)
                            item.IsChecked = true;
                        else
                            item.IsChecked = false;
                    }
                }
                else
                {
                    item = Items[SelectedIndex] as RibbonTab;

                    if (item != null)
                    {
                        SelectedTabItem = item;

                        if (isItemLoaded)
                        {
                            item.IsChecked = true;
                            if (this.RibbonState == RibbonState.Hide && item.IsCancelRibbonState == false)
                            {
                                this.RibbonState = RibbonState.Adorner;
                            }
                        }
                        else
                        {
                            if (item.IsChecked)
                                item.IsChecked = true;
                            else
                                item.IsChecked = false;
                        }
                    }
                }
            }

        }

        #endregion

        #region Key tips handling
        /// <summary>
        /// Processing complex key tips.
        /// </summary>
        private void ProcessComplexKeyTips()
        {
            if (m_keyTipComplexLevel.Count == 1 && m_keyTipComplexLevel.ContainsKey(m_keyTipComplexPath))
            {
                InvokeKeyTip((KeyTip)m_keyTipComplexLevel[m_keyTipComplexPath]);
                m_keyTipComplexPath = string.Empty;
            }
            else
            {
                HideKeyTips(m_keyTipCurrentLevel.Values);
                ShowKeyTips(m_keyTipComplexLevel.Values);
            }

            m_complexLetterAdded = false;
            m_keyTipCurrentLevel = new Dictionary<string, KeyTip>();

            foreach (string key in m_keyTipComplexLevel.Keys)
            {
                m_keyTipCurrentLevel.Add(key, m_keyTipComplexLevel[key]);
            }

            m_keyTipComplexLevel.Clear();
            KeyTipsCurrentLevel = m_keyTipCurrentLevel;
        }

        /// <summary>
        /// Levels the up key tips.
        /// </summary>
        private void LevelUpKeyTips()
        {
            m_keyTipComplexPath = string.Empty;
            m_complexLetterAdded = false;
            m_keyTipCurrentLevel = FindKeyTipLevel(m_keyTipPath);
            HideKeyTips(m_keyTipCurrentLevel.Values);

            if (m_keyTipPath.Count > 0)
            {
                m_keyTipPath.RemoveAt(m_keyTipPath.Count - 1);
                m_keyTipCurrentLevel = FindKeyTipLevel(m_keyTipPath);
                ShowKeyTips(m_keyTipCurrentLevel.Values);
                KeyTipsCurrentLevel = m_keyTipCurrentLevel;
            }
            else
            {
                m_isAdornersShown = false;
                m_keyTipPath.Clear();
                KeyTipsCurrentLevel.Clear();
            }
        }

        /// <summary>
        /// Initializes the key tips.
        /// </summary>
        private void InitializeKeyTips()
        {
            m_keyTips = new Dictionary<string, KeyTip>();
            m_keyTipComplexLevel = new Dictionary<string, KeyTip>();
            m_keyTipCurrentLevel = new Dictionary<string, KeyTip>();
            m_keyTipPath = new List<string>();
            m_complexLetterAdded = false;

            if (ApplicationMenu != null && KeyTip.HasKeyTip(ApplicationMenu))
            {
                string keyTipText = KeyTip.GetKeyTip(ApplicationMenu);
                m_keyTips.Add(keyTipText, new KeyTip(ApplicationMenu, keyTipText));
            }

            if (BackStageButton != null && KeyTip.HasKeyTip(BackStage))
            {
                string keyTipText = KeyTip.GetKeyTip(BackStage);
                m_keyTips.Add(keyTipText, new KeyTip(BackStageButton, keyTipText));
            }

            if (QuickAccessToolBar != null && QuickAccessToolBar.m_popup_OverflowButton != null && QuickAccessToolBar.m_popup_OverflowButton.Visibility == Visibility.Visible)
            {
                m_keyTips.Add("00", new KeyTip(QuickAccessToolBar.m_popup_OverflowButton, "00"));
            }

            if (QuickAccessToolBar != null)
            {
                FindKeyTips(ref m_keyTips, QuickAccessToolBar);
            }

            if (this.TabPanelItem != null)
            {
                FindKeyTips(ref m_keyTips, this.TabPanelItem);
            }


            FindKeyTips(ref m_keyTips, this);
        }

        /// <summary>
        /// Finds the key tip level.
        /// </summary>
        /// <param name="path">The path value.</param>
        /// <returns>result value</returns>
        private Dictionary<string,KeyTip> FindKeyTipLevel(List<string> path)
        {
            Dictionary<string,KeyTip> result = null;

            if (path != null && path.Count > 0)
            {
                result = m_keyTips;
                for (int i = 0; i < path.Count; i++)
                {
                    if (result.ContainsKey(path[i]))
                    {
                        if ((result[path[i]] as KeyTip).SubLevel != null)
                        {
                            result = (result[path[i]] as KeyTip).SubLevel;
                        }
                    }
                }
            }
            else
            {
                result = m_keyTips;
            }

            return result;
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Shows key tips.
        /// </summary>
        /// <param name="colection">Key tip's collection.</param>
        private void ShowKeyTips(ICollection colection)
        {
            RibbonAdorner adorner;
            AdornerLayer adornerLayer;
            m_ribbontabflag = false;
            foreach (KeyTip keyTip in colection)
            {
                if (keyTip.Host != null)
                {
                    if (keyTip.Host is RibbonTab)
                    {
                        m_ribbontabflag = true;
                    }

                    UIElement host = null;
                    if (keyTip.Host is RibbonBar && !((keyTip.Host as FrameworkElement).Parent is QuickAccessToolBar))
                    {
                        RibbonBar bar = (RibbonBar)keyTip.Host;
                        host = bar.LauncherButton;
                    }
                    else
                    {
                        host = keyTip.Host;
                    }

                    if (keyTip.IsCollapsedElement)
                    {
                        RibbonBar ribbonbr = (RibbonBar)keyTip.Host;
                        if (keyTip.Text == ribbonbr.KeyTipOnCollapsed)
                        {
                            host = keyTip.Host;
                            adornerLayer = AdornerLayer.GetAdornerLayer(host);
                            if (adornerLayer == null)
                            {
                                adornerLayer = m_adornerLayer;
                            }

                            if (adornerLayer != null && !IsExistKeyTipAdorners(adornerLayer, host))
                            {                                
                                m_adornerLayer = adornerLayer;
                                adorner = new RibbonAdorner(host);
                                adorner.Enabled = host.IsEnabled;
                                adorner.Name = "PART_KeyTipAdorner";
                                adorner.Text = keyTip.Text;
                                LayoutKeyTipAdorner(keyTip, host, adorner);
                                try
                                {
                                    adornerLayer.Add(adorner);
                                }
                                catch { host.InvalidateVisual(); }
                                continue;
                            }
                        }
                    }

                    if (host != null && host.IsVisible)
                    {
                        adornerLayer = AdornerLayer.GetAdornerLayer(host);
                        if (adornerLayer == null)
                        {
                            adornerLayer = m_adornerLayer;
                        }

                        if (adornerLayer != null)
                        {
                            m_adornerLayer = adornerLayer;

                            if (host is ApplicationMenu)
                            {
                                (host as ApplicationMenu).IsKeyTipShown = true;
                            }

                            if (keyTip.Host is SplitMenuButton)
                            {
                                SplitMenuButton splitMnu = keyTip.Host as SplitMenuButton;
                                splitMnu.MouseMove += new MouseEventHandler(splitMenu_MouseMove);
                            }

                            if (!IsExistKeyTipAdorners(adornerLayer, host))
                            {
                                adorner = new RibbonAdorner(host);
                                adorner.Enabled = host.IsEnabled;
                                adorner.Name = "PART_KeyTipAdorner";
                                adorner.Text = keyTip.Text;
                                LayoutKeyTipAdorner(keyTip, host, adorner);
                                try
                                {
                                    if (keyTip.Host is RibbonBar)
                                    {
                                        RibbonBar rBar = keyTip.Host as RibbonBar;
                                        if (rBar.KeyTipOnCollapsed == keyTip.Text)
                                        {
                                            if (rBar.PanelState == RibbonBarState.Collapsed)
                                            {
                                                adornerLayer.Add(adorner);
                                            }
                                        }
                                        else
                                            adornerLayer.Add(adorner);
                                    }
                                    else
                                        adornerLayer.Add(adorner);
                                }
                                catch { host.InvalidateVisual(); }
                            }
                        }
                    }

                    if (keyTip.Host is DropDownButton && (keyTip.Host as FrameworkElement).Parent is QuickAccessToolBar)
                    {
                        DropDownButton dropDown = (DropDownButton)keyTip.Host;
                        dropDown.IsDropDownOpen = false;
                    }
                    else if (keyTip.Host is RibbonBar && (keyTip.Host as FrameworkElement).Parent is QuickAccessToolBar)
                    {
                        RibbonBar bar = keyTip.Host as RibbonBar;
                        if(bar.DropDownButton != null)
                            bar.DropDownButton.IsDropDownOpen = false;
                    }
                }
            }
            m_isAdornersShown = true;
        }

        private bool IsExistKeyTipAdorners(AdornerLayer adornerlayer, UIElement host)
        {
            if (adornerlayer != null && host != null)
            {
                Adorner[] adorners = adornerlayer.GetAdorners(host);

                if (adorners != null)
                {
                    foreach (Adorner adorner in adorners)
                    {
                        if (adorner.Name == "PART_KeyTipAdorner")
                        {
                            return true;
                        }
                    }                    
                    return false;
                }
                else
                    return false;
            }
            else
                return false;
        }

        /// <summary>
        /// Handles the MouseMove event of the splitMnu control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        private void splitMenu_MouseMove(object sender, MouseEventArgs e)
        {
            if (m_isAdornersShown && (sender as SplitMenuButton).IsMenuOpen == false)
            {
                HideKeyTips();
                (sender as SplitMenuButton).MouseMove -= new MouseEventHandler(splitMenu_MouseMove);
            }
        }

        /// <summary>
        /// Layouts the key tip adorner.
        /// </summary>
        /// <param name="tip">The tip value.</param>
        /// <param name="host">The host value.</param>
        /// <param name="adorner">The adorner.</param>
        private void LayoutKeyTipAdorner(KeyTip tip, UIElement host, TemplatedAdornerBase adorner)
        {
            Panel panel = VisualUtils.FindAncestor(host, typeof(LargeButtonPanel)) as Panel;
            if (panel == null)
            {
                panel = VisualUtils.FindAncestor(host, typeof(GroupPanel)) as Panel;
            }
            
            if (host is RibbonBar)
            {
                panel = (Panel)VisualUtils.FindDescendant(host, typeof(MultilinePanel));
            }

            if (panel != null && panel.IsLoaded)
            {
                adorner.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                if (host is RibbonButton)
                {
                    if ((host as RibbonButton).SizeForm == SizeForm.ExtraSmall)
                    {
                        adorner.Margin = new Thickness(9, 0, 0, 0);
                    }
                }

                if (host is SplitButton || host is DropDownButton)
                {
                    if (host is DropDownButton && (host as DropDownButton).SizeForm == SizeForm.ExtraSmall)
                    {
                        adorner.Margin = new Thickness(5, 0, 0, 0);
                    }
                }

                Size hostSize = host.RenderSize;
                Size adornerSize = adorner.DesiredSizeInternal;
                Point pPoint=new Point(0,0);
                Point chPoint=new Point(0,0);
                if(PresentationSource.FromVisual(panel) != null)
                 pPoint = panel.PointToScreen(new Point(0, 0));
                if(PresentationSource.FromVisual(host) != null)
                 chPoint = host.PointToScreen(new Point(0, 0));
                Point adPoint = new Point(chPoint.X + (hostSize.Width - adornerSize.Width) / 2, chPoint.Y + (hostSize.Height - adornerSize.Height) / 2);

                double maxUp = adPoint.Y - (pPoint.Y - adornerSize.Height / 2);
                double maxDown = (pPoint.Y + panel.RenderSize.Height - adornerSize.Height / 2) - adPoint.Y;

                Thickness margin = host is FrameworkElement ? (host as FrameworkElement).Margin : new Thickness();
                double diffYUp = chPoint.Y - pPoint.Y;
                double diffYDown = (pPoint.Y + panel.DesiredSize.Height) - (chPoint.Y + hostSize.Height + margin.Bottom);
                double delta = adornerSize.Height + (hostSize.Height - adornerSize.Height) / 2;

                if (diffYUp <= adornerSize.Height)
                {
                    if (isCollapsedKeyTip)
                    {
                        adorner.OffsetY = Math.Min(delta, maxUp);
                    }
                    else
                        adorner.OffsetY = -Math.Min(delta, maxUp);
                }

                if (diffYDown <= adornerSize.Height)
                {
                    adorner.OffsetY = Math.Min(delta, maxDown);
                }
            }

            if (host is RibbonTab || (host is RibbonButton && (host as FrameworkElement).Name == "PART_DialogLauncherButton"))
            {
                adorner.OffsetY = 13;
                if (host is RibbonButton)
                {
                    RibbonBar bar = (host as FrameworkElement).TemplatedParent as RibbonBar;
                    if (bar == null)
                        bar = VisualUtils.FindSomeParent(host as FrameworkElement, typeof(RibbonBar)) as RibbonBar;
                    if (bar != null && bar.PanelState == RibbonBarState.Collapsed)
                    {
                        adorner.OffsetY = -13;
                    }
                }
            }
            else if ((host as FrameworkElement).Parent is DropDownButton)
            {
                adorner.OffsetY = 6;
                adorner.OffsetX = -(((host as FrameworkElement).RenderSize.Width / 2) - 20);
            }
            else if (host is RibbonMenuItem)
            {
                adorner.OffsetY = 6;
            }
            else if (host is BackStageButton)
            {
                adorner.OffsetY = 13;
            }
            else if (host.Equals(this.TabPanelItem))
            {
                adorner.OffsetY = ((host as FrameworkElement).RenderSize.Height/1.5);
            }
            else if (host is SplitMenuButton || host is SimpleMenuButton)
            {
                adorner.OffsetY = 12;
                adorner.OffsetX = -(((host as FrameworkElement).RenderSize.Width / 2) - 30);
                if (KeyTip.HasSplitMenuKeyTip(host) && KeyTip.GetSplitMenuKeyTip(host) == tip.Text)
                    adorner.OffsetX = (((host as FrameworkElement).RenderSize.Width / 2) - 3);
            }
            else if ((host as FrameworkElement).Parent is QuickAccessToolBar)
            {
                adorner.OffsetY = 10;
            }
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Hides key tips.
        /// </summary>
        /// <param name="collection">Key tip's collection.</param>
        private void HideKeyTips(ICollection collection)
        {
            AdornerLayer adornerLayer;
            Adorner[] adorners;

            foreach (KeyTip keyTip in collection)
            {
                if (keyTip.Host != null)
                {
                    UIElement host = null;
                    if (keyTip.Host is RibbonBar && !((keyTip.Host as FrameworkElement).Parent is QuickAccessToolBar))
                    {
                        RibbonBar bar = (RibbonBar)keyTip.Host;
                        host = bar.LauncherButton;
                    }
                    else
                    {
                        host = keyTip.Host;
                    }

                    if (keyTip.IsCollapsedElement)
                    {
                        RibbonBar ribbonbr = (RibbonBar)keyTip.Host;
                        if (keyTip.Text == ribbonbr.KeyTipOnCollapsed)
                        {
                            host = keyTip.Host;
                        }
                    }

                    if (host != null)
                    {
                        if (host is ApplicationMenu)
                        {
                            (host as ApplicationMenu).IsKeyTipShown = false;
                        }

                        adornerLayer = AdornerLayer.GetAdornerLayer(host);
                        if (adornerLayer != null)
                        {
                            adorners = adornerLayer.GetAdorners(host);

                            if (adorners != null)
                            {
                                foreach (Adorner adorner in adorners)
                                {
                                    if (adorner.Name == "PART_KeyTipAdorner")
                                    {
                                        adornerLayer.Remove(adorner);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            m_isAdornersShown = false;
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Invokes KeyTip host element.
        /// </summary>
        /// <param name="keyTip">KeyTip which will be invoked.</param>
        private void InvokeKeyTip(KeyTip keyTip)
        {
            if (keyTip != null)
            {
                RibbonTab tab = keyTip.Host as RibbonTab;
                if (tab != null)
                {
                    if (RibbonState == RibbonState.Hide)
                    {
                        RibbonState = RibbonState.Adorner;
                    }
                    if (m_isAdornersShown || isSystemKey)
                        tab.IsChecked = true;
                    m_ribbontabflag = true;
                    isCollapsedKeyTip = false;
                }
                else
                {
                    GeneralInvoke(keyTip, keyTip.Host);
                    if ((keyTip.Host as FrameworkElement).Parent is QuickAccessToolBar)
                    {
                        QATItemsInvoke(keyTip);
                    }
                    RibbonBar rbar=null;
                    if (keyTip.Host is RibbonBar)
                         rbar = keyTip.Host as RibbonBar;                       
                    if (!(keyTip.Host is RibbonComboBox) && !(keyTip.Host is DropDownButton) && rbar != null && rbar.PanelState != RibbonBarState.Collapsed)
                    {
                        HideRibbonTab();
                    }
                }
            }
        }

        /// <summary>
        /// Hides the ribbon tab.
        /// </summary>
        private void HideRibbonTab()
        {
            if (RibbonState == RibbonState.Adorner)
            {
                HideAdorned();
                RibbonTab selectedTab = this.ItemContainerGenerator.ContainerFromItem(this.SelectedItem) as RibbonTab;

                if (selectedTab != null)
                {
                    selectedTab.m_tabButton.IsChecked = false;
                }
                this.RibbonState = RibbonState.Hide;
            }
        }

        /// <summary>
        /// Hides the application menu.
        /// </summary>
        private void HideAppMenu()
        {
            if (appMenuProvider != null && appMenuProvider.ExpandCollapseState == ExpandCollapseState.Expanded)
            {
                appMenuProvider.Collapse();
                ShowKeyTips();
            }
        }

        /// <summary>
        /// QATs the items invoke.
        /// </summary>
        /// <param name="keyTip">The key tip.</param>
        private void QATItemsInvoke(KeyTip keyTip)
        {
            Hashtable subLevelkeyTips = new Hashtable();
            if (keyTip.Host is RibbonBar)
            {
                RibbonBar bar = keyTip.Host as RibbonBar;
                m_dropDownbutton = bar.DropDownButton;
                m_dropDownbutton.IsDropDownOpen = true;
                m_dropDownbutton.IsDropDownOpenChanged += new PropertyChangedCallback(DropDownbutton_IsDropDownOpenChanged);

                HideKeyTips(m_keyTipCurrentLevel.Values);
                if (UpdateKeyTips(ref keyTip))
                {
                    m_keyTipPath.Add(keyTip.Text);
                    ShowKeyTips(keyTip.SubLevel.Values);
                    KeyTipsCurrentLevel = keyTip.SubLevel;
                }
            }
        }

        RibbonGallery currentGallery = null;

       internal DropDownButton dropdown;
        /// <summary>
        /// General invoke based on UIAutomation.
        /// </summary>
        /// <param name="keyTip">The key tip.</param>
        /// <param name="target">Target to be invoked.</param>
        /// <property name="flag" value="Finished"/>
        private void GeneralInvoke(KeyTip keyTip, UIElement target)
        {
            if (target is RibbonBar)
            {
                RibbonBar bar = (RibbonBar)target;
                if (keyTip.IsCollapsedElement && (bar.DropDownButton!=null && !bar.DropDownButton.IsDropDownOpen) && keyTip.Text != Ribbon.GetKeyTip(bar))
                {
                    dropdown = (DropDownButton)VisualUtils.FindDescendant(keyTip.Host, typeof(DropDownButton));
                    if (dropdown != null)
                    {
                        dropdown.IsDropDownOpen = true;
                        isCollapsedKeyTip = true;
                        dropdown.IsDropDownOpenChanged += new PropertyChangedCallback(DropDownbutton_IsDropDownOpenChanged);
                        HideKeyTips(m_keyTipCurrentLevel.Values);
                        if (UpdateKeyTips(ref keyTip))
                        {
                            m_keyTipPath.Add(keyTip.Text);
                            ShowKeyTips(keyTip.SubLevel.Values);
                            KeyTipsCurrentLevel = keyTip.SubLevel;
                        }
                    }
                }
                else
                {
                    target = bar.LauncherButton;
                    RoutedEventArgs args = new RoutedEventArgs();
                    args.RoutedEvent = RibbonBar.LauncherClickEvent;
                    bar.RaiseEvent(args);
                    isCollapsedKeyTip = false;
                    if (target is RibbonButton)
                    {
                        AutomationPeer peer = UIElementAutomationPeer.CreatePeerForElement(target);
                        IInvokeProvider provider = peer is IInvokeProvider ?peer.GetPattern(PatternInterface.Invoke) as IInvokeProvider:null;
                        if (provider != null)
                            provider.Invoke();
                    }
                }
            }
            else if (target is SplitMenuButton && KeyTip.GetSplitMenuKeyTip(target) == keyTip.Text)
            {

                m_splitMenuButton = target as SplitMenuButton;
                m_splitMenuButton.IsMenuOpen = true;
                m_splitMenuButton.IsMenuOpenChanged += new PropertyChangedCallback(SplitMenuButton_IsMenuOpenChanged);
                HideKeyTips(m_keyTipCurrentLevel.Values);
                if (UpdateKeyTips(ref keyTip))
                {
                    m_keyTipPath.Add(keyTip.Text);
                    ShowKeyTips(keyTip.SubLevel.Values);
                    KeyTipsCurrentLevel = keyTip.SubLevel;
                }
            }
            else if (target is DropDownButton)
            {
                if (target is FrameworkElement)
                {
                    RibbonBar bar = VisualUtils.FindSomeParent(target as FrameworkElement, typeof(RibbonBar)) as RibbonBar;
                    if (bar != null && bar.PanelState == RibbonBarState.Collapsed)
                        bar.ShowPopup();
                }
                m_dropDownbutton = target as DropDownButton;
                m_dropDownbutton.IsDropDownOpen = true;
                m_dropDownbutton.IsDropDownOpenChanged += new PropertyChangedCallback(DropDownbutton_IsDropDownOpenChanged);
                HideKeyTips(m_keyTipCurrentLevel.Values);
                if (UpdateKeyTips(ref keyTip))
                {
                    m_keyTipPath.Add(keyTip.Text);
                    ShowKeyTips(keyTip.SubLevel.Values);
                    KeyTipsCurrentLevel = keyTip.SubLevel;
                }
                else
                {
                    m_keyTipPath.Clear();
                    KeyTipsCurrentLevel.Clear();
                }
            }
            else if (target is BackStageButton)
            {
                HideKeyTips(m_keyTipCurrentLevel.Values);
                BackStage.IsKeytipOpen = true;
                ShowBackStage();
            }
            else if (target is RibbonGallery)
            {
                (target as RibbonGallery).IsDropDownOpen = true;
                (target as RibbonGallery).IsDropDownOpenChanged += new PropertyChangedCallback(Ribbon_IsDropDownOpenChanged);
                HideKeyTips(m_keyTipCurrentLevel.Values);
                if (UpdateKeyTips(ref keyTip))
                {
                    m_keyTipPath.Add(keyTip.Text);
                    ShowKeyTips(keyTip.SubLevel.Values);
                    KeyTipsCurrentLevel = keyTip.SubLevel;
                }
                else
                {
                    m_keyTipPath.Clear();
                    KeyTipsCurrentLevel.Clear();
                }

                currentGallery = target as RibbonGallery;
            }
            else
            {
                AutomationPeer peer = UIElementAutomationPeer.CreatePeerForElement(target);
                if (peer != null && peer.IsControlElement())
                {
                    if (peer is IInvokeProvider)
                    {
                        if (peer.GetClassName() == "RibbonButton")
                        {
                            RibbonButton ribbonButton = target as RibbonButton;
                            if (ribbonButton != null)
                            {
                                if (ribbonButton.IsToggle && ribbonButton.Command == null)
                                {
                                    ribbonButton.IsSelected = ribbonButton.IsSelected ? false : true;
                                }
                                if(ribbonButton.Command!=null)
                                if ((ribbonButton.Command is RoutedCommand && ((System.Windows.Input.RoutedCommand)(ribbonButton.Command)).Name.Contains("Toggle"))|| ribbonButton.Command is DelegateCommand<object>)
                                {
                                    ribbonButton.Command.Execute(ribbonButton.CommandParameter);
                                }
                            }
                        }
                        IInvokeProvider provider = peer.GetPattern(PatternInterface.Invoke) as IInvokeProvider;
                        if (provider != null)
                        {
                            if (currentGallery != null && currentGallery.IsDropDownOpen)
                                currentGallery.IsDropDownOpen = false;
                            provider.Invoke();
                        }
                        if (dropdown != null && dropdown.IsDropDownOpen)
                            dropdown.IsDropDownOpen = false;

                    }
                    else if (peer is IExpandCollapseProvider)
                    {
                        if (target is FrameworkElement)
                        {
                            RibbonBar bar = VisualUtils.FindSomeParent(target as FrameworkElement, typeof(RibbonBar)) as RibbonBar;
                            if (bar != null && bar.PanelState == RibbonBarState.Collapsed)
                                bar.ShowPopup();
                        }
                        target.Focus();
                        appMenuProvider = peer.GetPattern(PatternInterface.ExpandCollapse) as IExpandCollapseProvider;
                        if (appMenuProvider != null && appMenuProvider.ExpandCollapseState == ExpandCollapseState.Collapsed)
                        {
                            appMenuProvider.Expand();
                            ApplicationMenu menu = target as ApplicationMenu;
                            if (menu != null && menu.Items.Count > 0)
                            {
                                UIElement element = ApplicationMenu.Items[0] as UIElement;
                                if (element != null)
                                {
                                    //element.Focus();
                                    HideKeyTips(m_keyTipCurrentLevel.Values);
                                    if (UpdateKeyTips(ref keyTip))
                                    {
                                        m_keyTipPath.Add(keyTip.Text);
                                        ShowKeyTips(keyTip.SubLevel.Values);
                                        KeyTipsCurrentLevel = keyTip.SubLevel;
                                    }
                                }
                            }

                            return;
                        }
                    }

                    if (appMenuProvider != null && appMenuProvider.ExpandCollapseState == ExpandCollapseState.Expanded)
                    {
                        appMenuProvider.Collapse();
                    }

                    if (peer is IToggleProvider)
                    {
                        IToggleProvider provider = peer.GetPattern(PatternInterface.Toggle) as IToggleProvider;
                        if (provider != null)
                        {
                            provider.Toggle();
                        }
                        if (peer.GetClassName() == "CheckBox")
                        {
                            RibbonCheckBox ribbonCheckBox = target as RibbonCheckBox;

                            RoutedEventArgs newEventArgs = new RoutedEventArgs(ButtonBase.ClickEvent);
                            ribbonCheckBox.RaiseEvent(newEventArgs);
                            if (ribbonCheckBox.Command != null)
                            {
                                ribbonCheckBox.Command.Execute(ribbonCheckBox.CommandParameter);
                            }
                        }
                    }

                    if (peer.GetClassName() == "TextBox")
                    {
                        TextBox txtBox = target as TextBox;
                        if (txtBox != null)
                        {
                            txtBox.Focus();
                            if (txtBox.Text.Length > 0)
                            {
                                txtBox.SelectAll();
                            }
                        }
                    }

                    if (peer is ISelectionItemProvider)
                    {
                        ISelectionItemProvider provider = peer.GetPattern(PatternInterface.SelectionItem) as ISelectionItemProvider;
                        if (null != provider)
                        {
                            provider.Select();
                        }
                        if (peer.GetClassName() == "RadioButton")
                        {
                            RibbonRadioButton ribbonRadioButton = target as RibbonRadioButton;

                            RoutedEventArgs newEventArgs = new RoutedEventArgs(ButtonBase.ClickEvent);
                            ribbonRadioButton.RaiseEvent(newEventArgs);
                            if (ribbonRadioButton.Command != null)
                            {
                                ribbonRadioButton.Command.Execute(ribbonRadioButton.CommandParameter);
                            }
                        }
                    }
                    HideKeyTips();
                }
            }
        }

        private void Ribbon_IsDropDownOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.OldValue && m_isAdornersShown)
            {
                HideKeyTips(KeyTipsCurrentLevel.Values);
                LevelUpKeyTips();
            }
            else if ((bool)e.OldValue)
            {
                HideKeyTips(KeyTipsCurrentLevel.Values);
            }

            RibbonGallery gallery = d as RibbonGallery;
            gallery.IsDropDownOpenChanged -= new PropertyChangedCallback(Ribbon_IsDropDownOpenChanged);
        }

        /// <summary>
        /// Splits the menu button_ is menu open changed.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private void SplitMenuButton_IsMenuOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.OldValue && m_isAdornersShown)
            {                
                HideKeyTips(KeyTipsCurrentLevel.Values);
                LevelUpKeyTips();
            }
            else if ((bool)e.OldValue)
            {
                HideKeyTips(KeyTipsCurrentLevel.Values);                
            }

            SplitMenuButton button = d as SplitMenuButton;
            button.IsMenuOpenChanged -= new PropertyChangedCallback(SplitMenuButton_IsMenuOpenChanged);
        }
        /// <summary>
        /// Drops the downbutton_ is drop down open changed.
        /// </summary>
        /// <param name="d">The DependencyObject.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs<see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private void DropDownbutton_IsDropDownOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.OldValue && m_isAdornersShown)
            {                
                HideKeyTips(KeyTipsCurrentLevel.Values);
                LevelUpKeyTips();
            }
            else if ((bool)e.OldValue)
            {
                HideKeyTips(KeyTipsCurrentLevel.Values);                
            }

            DropDownButton button = d as DropDownButton;
            button.IsDropDownOpenChanged -= new PropertyChangedCallback(DropDownbutton_IsDropDownOpenChanged);
        }

        /// <summary>
        /// Finds the key tips.
        /// </summary>
        /// <param name="keyTips">The key tips.</param>
        /// <param name="root">The root value.</param>
        private void FindKeyTips(ref Dictionary<string,KeyTip> keyTips, UIElement root)
        {
            if (root == null)
            {
                throw new ArgumentNullException("root element");
            }

            KeyTip keyTip;
            UIElement element;
            bool digDeeper = true, isbuttonpanel = false;

            if (root is ItemsControl && ((ItemsControl)root).ItemsSource != null)
            {
                foreach (object child in ((ItemsControl)root).Items)
                {
                    if (!(root is RibbonTab))
                    {
                        element = ((ItemsControl)root).ItemContainerGenerator.ContainerFromItem(child) as UIElement;
                    }
                    else
                    {
                        element = child as UIElement;
                    }
                    if (element != null && KeyTip.HasKeyTip(element))
                    {
                        digDeeper = false;
                        if (element is RibbonBar && !((element as FrameworkElement).Parent is QuickAccessToolBar))
                        {
                            digDeeper = true;
                        }

                        RibbonBar ribbonBar = element as RibbonBar;
                        if (ribbonBar != null)
                        {
                           
                                keyTip = new KeyTip(element, ribbonBar.KeyTipOnCollapsed);

                                if (keyTips.ContainsKey(keyTip.Text))
                                {
                                    keyTips.Remove(keyTip.Text);
                                }

                                keyTips.Add(keyTip.Text, keyTip);
                           
                        }

                        keyTip = new KeyTip(element, KeyTip.GetKeyTip(element));

                        if (keyTips.ContainsKey(keyTip.Text))
                            keyTips.Remove(keyTip.Text);

                        keyTips.Add(keyTip.Text, keyTip);

                        if (element is SplitMenuButton)
                        {
                            keyTip = new KeyTip(element, KeyTip.GetSplitMenuKeyTip(element));
                            if (keyTip.Text != "")
                            {
                                if (keyTips.ContainsKey(keyTip.Text))
                                    keyTips.Remove(keyTip.Text);

                                keyTips.Add(keyTip.Text, keyTip);
                            }
                        }
                    }
                    else if (element != null && element is ButtonPanel)
                    {
                        digDeeper = true;
                        isbuttonpanel = true;
                    }
                }
            }
            else
            {
                foreach (object child in LogicalTreeHelper.GetChildren(root))
                {
                    element = child as UIElement;
                    if (element != null)
                    {
                        if(KeyTip.HasKeyTip(element))
                        digDeeper = false;
                        if (element is RibbonBar && !((element as FrameworkElement).Parent is QuickAccessToolBar))
                        {
                            digDeeper = true;
                        }

                        RibbonBar ribbonBar = element as RibbonBar;
                        if (ribbonBar != null && ribbonBar.Visibility == Visibility.Visible)
                        {
                            //if (ribbonBar.PanelState == RibbonBarState.Collapsed)
                            //{
                            if (ribbonBar.KeyTipOnCollapsed != string.Empty)
                            {
                                keyTip = new KeyTip(element, ribbonBar.KeyTipOnCollapsed);

                                if (keyTips.ContainsKey(keyTip.Text))
                                {
                                    keyTips.Remove(keyTip.Text);
                                }

                                keyTips.Add(keyTip.Text, keyTip);
                            }
                            //}
                        }

                        if (element.Visibility == Visibility.Visible && KeyTip.HasKeyTip(element))
                        {
                            keyTip = new KeyTip(element, KeyTip.GetKeyTip(element));
                            if (keyTips.ContainsKey(keyTip.Text))
                                keyTips.Remove(keyTip.Text);

                            keyTips.Add(keyTip.Text, keyTip);

                            if (element is SplitMenuButton)
                            {
                                keyTip = new KeyTip(element, KeyTip.GetSplitMenuKeyTip(element));
                                if (keyTip.Text != "")
                                {
                                    if (keyTips.ContainsKey(keyTip.Text))
                                        keyTips.Remove(keyTip.Text);

                                    keyTips.Add(keyTip.Text, keyTip);
                                }
                            }
                        }
                    }
                    else if (element != null && element is ButtonPanel)
                    {
                        digDeeper = true;
                        isbuttonpanel = true;
                    }
                }

                if (root is ApplicationMenu && (root as ApplicationMenu).ApplicationItems != null)
                {
                    foreach (object item in (root as ApplicationMenu).ApplicationItems)
                    {
                        UIElement appElement = item as UIElement;
                        if (appElement != null && appElement.Visibility == Visibility.Visible && KeyTip.HasKeyTip(appElement))
                        {
                            keyTip = new KeyTip(appElement, KeyTip.GetKeyTip(appElement));
                            if (keyTips.ContainsKey(keyTip.Text))
                                keyTips.Remove(keyTip.Text);

                            keyTips.Add(keyTip.Text, keyTip);
                        }
                    }
                }

                if (root is RibbonGallery && (root as RibbonGallery).MenuItems != null)
                {
                    foreach (object item in (root as RibbonGallery).MenuItems)
                    {
                        UIElement appElement = item as UIElement;
                        if (appElement != null && appElement.Visibility == Visibility.Visible && KeyTip.HasKeyTip(appElement))
                        {
                            keyTip = new KeyTip(appElement, KeyTip.GetKeyTip(appElement));
                            if (keyTips.ContainsKey(keyTip.Text))
                                keyTips.Remove(keyTip.Text);

                            keyTips.Add(keyTip.Text, keyTip);
                        }
                    }
                }

                if(root is RibbonBar)
                {
                    RibbonBar bar=root as RibbonBar;
                    if (bar != null && bar.Visibility == Visibility.Visible && bar.PanelState == RibbonBarState.Collapsed && bar.DropDownButton != null && bar.DropDownButton.IsDropDownOpen)
                    {
                        keyTip = new KeyTip(bar, KeyTip.GetKeyTip(bar));
                        if (keyTips.ContainsKey(keyTip.Text))
                            keyTips.Remove(keyTip.Text);

                        keyTips.Add(keyTip.Text, keyTip);

                    }
                }

                if (root.Equals(this.TabPanelItem))
                {
                    keyTip = new KeyTip(root, KeyTip.GetKeyTip(root));
                    if (keyTips.ContainsKey(keyTip.Text))
                        keyTips.Remove(keyTip.Text);
                    keyTips.Add(keyTip.Text, keyTip);                   
                }
            }

            if (digDeeper)
            {
                if (root is ItemsControl && ((ItemsControl)root).ItemsSource != null)
                {
                    foreach (object child in ((ItemsControl)root).Items)
                    {
                        element = child as UIElement;
                        if (element != null && element.Visibility==Visibility.Visible)
                        {
                            FindKeyTips(ref keyTips, element);
                        }
                    }
                }
                else
                {
                    foreach (object child in LogicalTreeHelper.GetChildren(root))
                    {
                        element = child as UIElement;
                        if (element != null && element.Visibility == Visibility.Visible)
                        {
                            FindKeyTips(ref keyTips, element);
                        }
                    }
                }
            }

            if (isbuttonpanel)
            {
                foreach (object child in LogicalTreeHelper.GetChildren(root))
                {
                    element = child as UIElement;
                    if (element != null && element.Visibility == Visibility.Visible)
                    {
                        FindKeyTips(ref keyTips, element);
                    }
                }
            }
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Tries to find a key tips sublevel below the root KeyTip.
        /// </summary>
        /// <param name="rootKeyTip">Root KeyTip.</param>
        /// <returns>
        /// Returns true if attempt was successful, otherwise, false.
        /// </returns>
        private bool UpdateKeyTips(ref KeyTip rootKeyTip)
        {
            Dictionary<string,KeyTip> keyTipBranch = new Dictionary<string,KeyTip>();
            UIElement root = rootKeyTip.Host;
            FindKeyTips(ref keyTipBranch, root);
            if (keyTipBranch.Count > 0)
            {
                rootKeyTip.SubLevel = keyTipBranch;
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Renders the key tips.
        /// </summary>
        internal void RenderKeyTips()
        {
            if (m_keyTipsCurrentLevel != null)
            {
                Adorner[] adorners;
                AdornerLayer adornerLayer;
                foreach (KeyTip keyTip in m_keyTipsCurrentLevel.Values)
                {
                    UIElement host = null;
                    if (keyTip.Host is RibbonBar && !((keyTip.Host as FrameworkElement).Parent is QuickAccessToolBar))
                    {
                        RibbonBar bar = (RibbonBar)keyTip.Host;
                        host = bar.LauncherButton;
                    }
                    else
                    {
                        host = keyTip.Host;
                    }

                    if (host != null)
                    {
                        adornerLayer = AdornerLayer.GetAdornerLayer(host);
                        if (adornerLayer != null)
                        {
                            adorners = adornerLayer.GetAdorners(host);

                            if (adorners != null)
                            {
                                foreach (Adorner adorner in adorners)
                                {
                                    if (adorner.Name == "PART_KeyTipAdorner")
                                    {
                                        adornerLayer.Remove(adorner);
                                    }
                                }
                            }
                        }
                    }
                }

                RibbonAdorner ribbonAdorner;

                foreach (KeyTip keyTip in m_keyTipsCurrentLevel.Values)
                {
                    UIElement host = null;
                    if (keyTip.Host is RibbonBar && !((keyTip.Host as FrameworkElement).Parent is QuickAccessToolBar))
                    {
                        RibbonBar bar = (RibbonBar)keyTip.Host;
                        host = bar.LauncherButton;
                    }
                    else
                    {
                        host = keyTip.Host;
                    }

                    if (host != null)
                    {
                        adornerLayer = AdornerLayer.GetAdornerLayer(host);
                        if (adornerLayer != null)
                        {
                            ribbonAdorner = new RibbonAdorner(host);
                            ribbonAdorner.Enabled = host.IsEnabled;
                            ribbonAdorner.Name = "PART_KeyTipAdorner";
                            ribbonAdorner.Text = keyTip.Text;

                            LayoutKeyTipAdorner(keyTip, host, ribbonAdorner);
                            try
                            {
                                adornerLayer.Add(ribbonAdorner);
                            }
                            catch { host.InvalidateVisual(); }
                        }
                    }
                }
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Minimizes the ribbon.
        /// </summary>
        internal void MinimizeRibbon()
        {
            if (RibbonState == RibbonState.Normal)
            {
                RibbonState = RibbonState.Hide;
                if (SelectedItem != null)
                {
                    RibbonTab selectedTab = this.ItemContainerGenerator.ContainerFromItem(this.SelectedItem) as RibbonTab;

                    if (selectedTab != null)
                    {
                        selectedTab.m_tabButton.IsChecked = false;
                    }
                    else
                    {
                        selectedTab = this.Items[this.SelectedIndex] as RibbonTab;
                        if (selectedTab != null)
                        {
                            selectedTab.m_tabButton.IsChecked = false;
                        }
                    }

                }
            }
            else if (RibbonState == RibbonState.Hide)
            {
                RibbonState = RibbonState.Normal;
                if (SelectedItem != null)
                {
                    RibbonTab selectedTab = this.ItemContainerGenerator.ContainerFromItem(this.SelectedItem) as RibbonTab;

                    if (selectedTab != null)
                    {
                        selectedTab.m_tabButton.IsChecked = true;
                    }
                    else
                    {
                        selectedTab = this.Items[this.SelectedIndex] as RibbonTab;
                        if (selectedTab != null)
                        {
                            selectedTab.m_tabButton.IsChecked = true;
                        }
                    }
                }
            }
            else
            {
                HideAdorned();
                RibbonState = RibbonState.Normal;
                if (SelectedItem != null)
                {
                    RibbonTab selectedTab = this.ItemContainerGenerator.ContainerFromItem(this.SelectedItem) as RibbonTab;

                    if (selectedTab != null)
                    {
                        selectedTab.m_tabButton.IsChecked = true;
                    }
                    else
                    {
                        selectedTab = this.Items[this.SelectedIndex] as RibbonTab;
                        if (selectedTab != null)
                        {
                            selectedTab.m_tabButton.IsChecked = true;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Shows the modal tab.
        /// </summary>
        /// <param name="ribbonTabName">Name of the ribbon tab.</param>
        /// <returns></returns>
        public bool ShowModalTab(string ribbonTabName)
        {
            if (!modalTabDisplayed)
            {
                this._tempTabCollection.Clear();
                foreach (RibbonTab item in this.Items)
                    this._tempTabCollection.Add(item);
                this._tempSelectedItem = this.SelectedItem;
                if(SelectedItem is RibbonTab)
                    (SelectedItem as RibbonTab).IsChecked = true;

                foreach (RibbonTab modalTab in this.ModalTabCollection)
                    if (modalTab.Name.Equals(ribbonTabName))
                    {
                        ShowModalTab(modalTab);
                        return true;
                    }
                return false;
            }
            return false;
        }

        /// <summary>
        /// Shows the modal tab.
        /// </summary>
        /// <param name="modalTab">The modal tab.</param>
        private void ShowModalTab(RibbonTab modalTab)
        {
            if (this.ItemsSource != null)
            {
                _itemsSourceItems = null;
                _itemsSourceItems = (IEnumerable<RibbonTab>)this.ItemsSource;
                ItemsSource = null;
                this.Items.Clear();

                _modalTabCollection.Clear();
                _modalTabCollection.Add(modalTab);

                modalTab.AddChildren();
                modalTab.UpdateLayout();

                this.ItemsSource = _modalTabCollection;

                (modalTab as RibbonTab).IsChecked = true;
                this.SelectedItem = modalTab;
            }

            else
            {
                this.Items.Clear();
                this.ContextTabGroups.Clear();
                this.Items.Add(modalTab);
                (modalTab as RibbonTab).IsChecked = true;
                this.SelectedItem = modalTab;
            }

            modalTabDisplayed = true;
        }

        /// <summary>
        /// Closes the modal tabs.
        /// </summary>
        /// <returns></returns>
        public bool CloseModalTabs()
        {
            if (IsSingleTab() && modalTabDisplayed)
            {
                if (this.ItemsSource != null)
                {
                    this.ItemsSource = null;
                    this.ItemsSource = _itemsSourceItems;

                    RibbonTab selectedTab = this.ItemContainerGenerator.ContainerFromIndex(0) as RibbonTab;
                    selectedTab.IsChecked = false;

                    this.SelectedItem = this._tempSelectedItem;

                    (this.SelectedItem as RibbonTab).IsChecked = true;
                }

                else
                {
                    this.Items.Clear();

                    foreach (RibbonTab tab in this._tempTabCollection)
                    {
                        if (!this.Items.Contains(tab))
                        {
                            if (tab.ContextTabGroup == null)
                                this.Items.Add(tab);
                            else
                            {
                                if(!this.ContextTabGroups.Contains(tab.ContextTabGroup))
                                    this.ContextTabGroups.Add(tab.ContextTabGroup);
                            }

                        }
                    }
                    RibbonTab selectedTab = this.ItemContainerGenerator.ContainerFromIndex(0) as RibbonTab;
                    if (selectedTab != null)
                        selectedTab.IsChecked = false;

                    this.SelectedItem = this._tempSelectedItem;
                    ArrangeContextTabGroups();
                }

                modalTabDisplayed = false;

                return true;
            }
            else
                return false;
        }
        
        internal bool IsSingleTab()
        {
            if (this.Items.Count > 1)
            {
                List<object> tabs = (from object tab in this.Items
                                 where !(tab as RibbonTab).HasContextTabGroup
                                 select tab).ToList<object>();

                if (tabs.Count == 1)
                    return true;

                return false;
            }
            else
                return this.Items.Count == 1;
        }

        /// <summary>
        /// Shows the adorned ribbon.
        /// </summary>
        internal void ShowAdorned()
        {
            if (this.Adorner_Popup != null)
            {
                Point ribbonPoint = this.PointToScreen(new Point(0.0, 0.0));
                
                System.Windows.Forms.Screen currentscreen = System.Windows.Forms.Screen.AllScreens[0];
                System.Drawing.Rectangle ribbonrect = new System.Drawing.Rectangle((int)ribbonPoint.X, (int)ribbonPoint.Y, (int)this.DesiredSize.Width, (int)this.DesiredSize.Height);
                System.Drawing.Point ribbonscreenpoint = new System.Drawing.Point((int)ribbonPoint.X, (int)ribbonPoint.Y);
                System.Drawing.Point mousepoint = System.Windows.Forms.Cursor.Position;

                currentscreen = System.Windows.Forms.Screen.FromPoint(mousepoint);
                double final=0;
                double height = 0.0;
                if (m_ribbonWindow != null)
                {
                    if ((2 * m_TabPanel.ActualHeight) > m_ribbonWindow.TitleBar.ActualHeight)
                        height = m_ribbonWindow.TitleBar.ActualHeight + m_TabPanel.ActualHeight;
                    else
                        height = m_ribbonWindow.TitleBar.ActualHeight;                   
                }


                if (ribbonscreenpoint.X >= currentscreen.WorkingArea.Left && ribbonscreenpoint.X + this.ActualWidth <= currentscreen.WorkingArea.Right)
                {
                    final = this.ActualWidth;
                }                
                else
                {

                    if (System.Windows.Forms.Screen.AllScreens.Count() > 1)
                    {
                        if (currentscreen.WorkingArea.Right > ribbonscreenpoint.X)
                            final = currentscreen.WorkingArea.Right - ribbonscreenpoint.X;
                        else
                            final = this.ActualWidth;

                        if (final > this.ActualWidth)
                            final = this.ActualWidth - currentscreen.WorkingArea.Left + ribbonscreenpoint.X;
                        else
                            Adorner_Popup.PlacementRectangle = new Rect(0, 0, final, height + 2);
                    }
                    else
                    {
                        if (ribbonscreenpoint.X < 0)
                            final = this.ActualWidth + ribbonscreenpoint.X;
                        else
                            final = currentscreen.WorkingArea.Right - ribbonscreenpoint.X;
#if SyncfusionFramework4_5

                        if (m_ribbonWindow != null && m_ribbonWindow.WindowState == WindowState.Maximized)
                            height = height + 4;
#endif

                        if (!SkinStorage.GetEnableTouch(this))
                            Adorner_Popup.PlacementRectangle = new Rect(0, 0, (final < 0) ? this.ActualWidth : final, height);
                    }
                    
                       
                }

                this.Adorner_Popup.Width = final > 0 ? final : this.ActualWidth;
                this.Adorner_Popup.IsOpen = true;
                Canvas.SetZIndex(Adorner_Popup, 0);
                
            }
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Hides the adorned ribbon.
        /// </summary>
        internal void HideAdorned()
        {
            if (this.RibbonState == RibbonState.Adorner)
                return;

            if (this.Adorner_Popup != null && this.Adorner_Popup.IsOpen == true)
            {
                this.Adorner_Popup.IsOpen = false;
                this.Adorner_Popup.Child = null;
            }
        }

        /// <summary>
        /// Adds the context tab groups.
        /// </summary>
        private void AddVisibleContextTabGroups()
        {
            foreach (ContextTabGroup group in this.ContextTabGroups)
            {
                //foreach (RibbonTab tab in group.RibbonTabs)
                //    tab.ContextAdorner = null;
                
                if (group.IsGroupVisible)
                    AddContextTabs(group);
            }
        }

        /// <summary>
        /// Adds the context tabs.
        /// </summary>
        /// <param name="contextTabGroup">Context tab group.</param>
        private void AddContextTabs(ContextTabGroup contextTabGroup)
        {
            RibbonTabCollection ribbonTabCollection = contextTabGroup.RibbonTabs;

            foreach (RibbonTab tab in ribbonTabCollection)
                Items.Add(tab);
        }

        /// <summary>
        /// Removes context tab.
        /// </summary>
        /// <param name="contextTabGroup">The Context Tab Group.</param>
        private void RemoveContextTabs(ContextTabGroup contextTabGroup)
        {
            RibbonTabCollection ribbonTabCollection = contextTabGroup.RibbonTabs;

            foreach (RibbonTab tab in ribbonTabCollection)
            {
                tab.ContextAdorner = null;
                Items.Remove(tab);
            }
        }

        /// <summary>
        /// Gets the first visible group.
        /// </summary>
        /// <param name="contextTabGroupCollection">The context tab group collection.</param>
        /// <returns>return result</returns>
        private ContextTabGroup GetFisrtVisibleGroup(ContextTabGroupCollection contextTabGroupCollection)
        {
            ContextTabGroup result = null;

            foreach (ContextTabGroup contextTabGroup in contextTabGroupCollection)
            {
                if (contextTabGroup.IsGroupVisible && contextTabGroup.RibbonTabs.Count > 0)
                {
                    result = contextTabGroup;
                    break;
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the last visible group.
        /// </summary>
        /// <param name="contextTabGroupCollection">The context tab group collection.</param>
        /// <returns> result value</returns>
        private ContextTabGroup GetLastVisibleGroup(ContextTabGroupCollection contextTabGroupCollection)
        {
            ContextTabGroup result = null;

            int count = contextTabGroupCollection.Count;

            for (int i = count - 1; i >= 0; i--)
            {
                if (contextTabGroupCollection[i].IsGroupVisible)
                {
                    result = contextTabGroupCollection[i];
                    break;
                }
            }

            return result;
        }

        /// <summary>
        /// Adds the item to QAT.
        /// </summary>
        /// <param name="target">Target value.</param>
        internal void AddItemToQAT(UIElement target)
        {
            QuickAccessToolBar.NeedItemWrap = false;
            QuickAccessToolBarItem item = new QuickAccessToolBarItem(target);
            bool CanAdd = QuickAccessToolBar.InternalCommandManager.CanAdd(item);
            QuickAccessToolBar.InternalCommandManager.Add(item);
            try
            {
                QATItems.Add(item.ClonedElement, target);

                if (CanAdd)
                    QuickAccessToolBar.Items.Add(item.ClonedElement);
            }
            catch
            {
            }

            QuickAccessToolBar.UpdateLayout();
            QuickAccessToolBar.InvalidateMeasure();

            QuickAccessToolBar.NeedItemWrap = true;

            CloseAllPopups(target);
        }

        /// <summary>
        /// Closes all popup
        /// </summary>
        /// <param name="target">The target.</param>
        private void CloseAllPopups(UIElement target)
        {
            FrameworkElement fe = VisualUtils.FindRootVisual(target) as FrameworkElement;
            while (fe != null && fe.GetType() == VisualUtils.RootPopupType)
            {
                Popup popup = fe.Parent as Popup;
                popup.IsOpen = false;

                fe = VisualUtils.FindRootVisual(popup.TemplatedParent as Visual) as FrameworkElement;
            }
        }

        /// <summary>
        /// Updates QAT Items due to QAT customize dialog last changes.
        /// </summary>
        private void UpdateItems()
        {
            InternalCommandManager commandManager = QuickAccessToolBar.InternalCommandManager;
            foreach (QuickAccessToolBarItem item in commandManager.Items)
            {

                if (commandManager.ResetFlag && item.SourceElement != null)
                {
                    try
                    {
                        QuickAccessToolBar.Items.Add(item.SourceElement);
                        QuickAccessToolBar.UpdateLayout();
                    }
                    catch
                    {
                    }
                }
                else if (item.ClonedElement != null)
                {
                    try
                    {
                        QuickAccessToolBar.Items.Add(item.ClonedElement);
                        QuickAccessToolBar.UpdateLayout();
                    }
                    catch
                    {
                    }
                }
                else
                {
                    Debug.WriteLine("ClonedElement not initialized");
                }
            }
            commandManager.ResetFlag = false;
        }

        /// <summary>
        /// Checks whether collections is equal or not.
        /// </summary>
        /// <param name="coll1">First collection</param>
        /// <param name="coll2">Second collection</param>
        /// <returns>
        /// True if collections is equal; otherwise, false.
        /// </returns>
        private bool IsCollectionsEquals(ArrayList coll1, ArrayList coll2)
        {
            if (coll1.Count != coll2.Count)
            {
                return false;
            }
            else
            {
                for (int i = 0; i < coll1.Count; i++)
                {
                    if (coll1[i] != coll2[i])
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Handles click on ribbon adorner.
        /// </summary>
        /// <param name="sender">Ribbon button.</param>
        /// <param name="e">The instance of RoutedEventArgs object.</param>
        public void Ribbon_Click(object sender, RoutedEventArgs e)
        {

        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// QuickAccessToolBar VisualInitializeCompleete method
        /// </summary><param name="sender">The source of the event.</param>
        /// <param name="e">The instance containing the event data.</param>
        private void QuickAccessToolBar_VisualInitializeCompleete(object sender, EventArgs e)
        {
            SynchronizeRibbonState();
            if (QuickAccessToolBar.PopupButton != null)
            {
                QuickAccessToolBar.PopupButton.BeforeDropDownPopup -= new CancelEventHandler(PopupButton_BeforeDropDownPopup);
                QuickAccessToolBar.PopupButton.AfterDropDownPopup -= new EventHandler(PopupButton_AfterDropDownPopup);
            }

            if (QuickAccessToolBar.PopupButtonOverflow != null)
            {
                QuickAccessToolBar.PopupButtonOverflow.BeforeDropDownPopup -= new CancelEventHandler(PopupButton_BeforeDropDownPopup);
                QuickAccessToolBar.PopupButtonOverflow.AfterDropDownPopup -= new EventHandler(PopupButton_AfterDropDownPopup);
            }
            if (QuickAccessToolBar.PopupButton != null)
            {
                QuickAccessToolBar.PopupButton.BeforeDropDownPopup += new CancelEventHandler(PopupButton_BeforeDropDownPopup);
                QuickAccessToolBar.PopupButton.AfterDropDownPopup += new EventHandler(PopupButton_AfterDropDownPopup);
            }

            if (QuickAccessToolBar.PopupButtonOverflow != null)
            {
                QuickAccessToolBar.PopupButtonOverflow.BeforeDropDownPopup += new CancelEventHandler(PopupButton_BeforeDropDownPopup);
                QuickAccessToolBar.PopupButtonOverflow.AfterDropDownPopup += new EventHandler(PopupButton_AfterDropDownPopup);
            }
        }

        /// <summary>
        /// Coerces the state of the ribbon.
        /// </summary>
        private void CoerceRibbonState()
        {
            this.CoerceValue(Ribbon.RibbonStateProperty);
            bool isDesignerMode = (bool)DesignerProperties.IsInDesignModeProperty.GetMetadata(typeof(DependencyObject)).DefaultValue;
            if (this.BackStageButton != null && !isDesignerMode && !this.BackStageButton.IsOpen)
                this.Focus();
        }

        /// <summary>
        /// Synchronizes the state of the ribbon.
        /// </summary>
        private void SynchronizeRibbonState()
        {
            RibbonState newState = RibbonState;
            RibbonButton minimizeRibbonButton = null;

            if (QuickAccessToolBar != null)
            {
                minimizeRibbonButton = (RibbonButton)QuickAccessToolBar.GetChild("PART_MinimizeButton");
            }

            if (minimizeRibbonButton != null)
            {
                if (newState == RibbonState.Hide)
                {
                    minimizeRibbonButton.IsSelected = true;
                }
                else if (newState == RibbonState.Normal)
                {
                    minimizeRibbonButton.IsSelected = false;
                }
            }

            if (newState == RibbonState.Hide)
            {
                tabonwindow1 = false;
                tabonwindow2 = true;
            }
            else if (newState == RibbonState.Normal)
            {
                tabonwindow2 = false;
                tabonwindow1 = true;
            }
        }

        /// <summary>
        /// Tabs the scrolling.
        /// </summary>
        /// <param name="delta">The delta.</param>
        internal void TabScrolling(int delta)
        {

            if (RibbonState == RibbonState.Normal)
            {
                if (delta < 0)
                {
                    for (int i = SelectedIndex + 1; i < Items.Count; i++)
                    {
                        if ((ItemContainerGenerator.ContainerFromIndex(i) as RibbonTab).IsVisible)
                        {
                            RibbonTab selectedTab = this.ItemContainerGenerator.ContainerFromItem(this.SelectedItem) as RibbonTab;

                            if (selectedTab != null)
                            {
                                selectedTab.IsChecked = false;
                            }

                            RibbonTab item = this.ItemContainerGenerator.ContainerFromIndex(i) as RibbonTab;
                            if (item != null && item.IsEnabled)
                            {
                                item.IsChecked = true;
                                this.SelectedIndex = i;
                                break;
                            }                            
                            
                        }
                    }
                }
                else if (delta > 0)
                {
                    for (int i = SelectedIndex - 1; i >= 0; i--)
                    {
                        RibbonTab rtab = Items[i] as RibbonTab;
                        if (rtab == null)
                            rtab = this.ItemContainerGenerator.ContainerFromIndex(i) as RibbonTab;
                        if (rtab.IsVisible)
                        {
                            RibbonTab selectedTab = this.ItemContainerGenerator.ContainerFromItem(this.SelectedItem) as RibbonTab;

                            if (selectedTab != null)
                            {
                                selectedTab.IsChecked = false;
                            }

                            RibbonTab item = this.ItemContainerGenerator.ContainerFromIndex(i) as RibbonTab;
                            if (item != null && item.IsEnabled)
                            {
                                item.IsChecked = true;
                                this.SelectedIndex = i;
                                break;
                            }                           
                            
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Adds the context tab groups.
        /// </summary>
        private void AddContextTabGroups()
        {
            if (this.ContextTabGroups != null)
            {
                ContextTabGroups.CollectionChanged += new NotifyCollectionChangedEventHandler(ContextTabGroups_CollectionChanged);
                foreach (ContextTabGroup contextTabGroup in this.ContextTabGroups)
                {
                    contextTabGroup.RibbonTabs.CollectionChanged += ContextTabGroup_RibbonTabs_CollectionChanged;
                    contextTabGroup.IsGroupVisibleChanged += new PropertyChangedCallback(ContextTabGroup_IsGroupVisibleChanged);
                }
            }
        }

        /// <summary>
        /// Arranges the context tab groups.
        /// </summary>
        private void ArrangeContextTabGroups()
        {
            ClearContextTabs();

            if (this.ContextTabGroups.Count > 0)
            {
                foreach (var item in this.ContextTabGroups)
                {
                    ContextTabGroup ctGroup = item as ContextTabGroup;
                    if (ctGroup != null)
                    {
                        if (ctGroup.DataContext == null)
                        {
                            if (this.DataContext != null)
                            {
                                ctGroup.DataContext = this.DataContext;
                            }
                        }
                    }
                }

            }

            foreach (ContextTabGroup group in this.ContextTabGroups)
            {
                if (group.IsGroupVisible)
                {
                    CheckContextTabGroupVisibility(group);
                    AddContextTabs(group);
                }
            }
        }

        /// <summary>
        /// Clears the context tabs.
        /// </summary>
        private void ClearContextTabs()
        {
            List<RibbonTab> temTabsCollection = new List<RibbonTab>();

            foreach (var tabin in this.Items)
                if (tabin is RibbonTab)
                    temTabsCollection.Add(tabin as RibbonTab);

            if (this.ContextTabGroups.Count > 0)
                foreach (var item in temTabsCollection)
                    if (item is RibbonTab)
                    {
                        RibbonTab tab = item as RibbonTab;

                        if (tab.ContextTabGroup != null)
                            Items.Remove(tab);
                    }

            //foreach (ContextTabGroup group in this.ContextTabGroups)
            //{
            //    foreach (var tab in group.RibbonTabs)
            //        if (tab is RibbonTab)
            //            (tab as RibbonTab).ContextAdorner = null;
            //}
        }

        /// <summary>
        /// Handles ContextTabGroupCollection changed event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The instance containing the event data.</param>
        private void ContextTabGroups_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ContextTabGroupCollection contextTabGroupCollection = sender as ContextTabGroupCollection;

            int index = -1;

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:

                    index = contextTabGroupCollection.Count - 1;

                    ContextTabGroup contextTabGroup = null;

                    if (index == e.NewStartingIndex)
                    {
                        contextTabGroup = contextTabGroupCollection[index];

                        CheckContextTabGroupVisibility(contextTabGroup);
                        if (contextTabGroup.IsGroupVisible == true)
                            AddContextTabs(contextTabGroup);
                    }
                    else
                    {
                        index = e.NewStartingIndex;
                        contextTabGroup = e.NewItems[0] as ContextTabGroup;

                        if (contextTabGroup.RibbonTabs.Count > 0)
                        {
                            int insertTabIndex = Items.IndexOf(ContextTabGroups[index + 1].RibbonTabs[0]);

                            foreach (RibbonTab tab in contextTabGroup.RibbonTabs)
                            {
                                Items.Insert(insertTabIndex, tab);
                                insertTabIndex++;
                            }

                            if (contextTabGroup.IsGroupVisible)
                            {
                                if (index < ContextTabGroups.IndexOf(m_firstVisibleGroup))
                                {
                                    m_firstVisibleGroup = contextTabGroup;
                                }

                                if (index >= ContextTabGroups.IndexOf(m_lastVisibleGroup))
                                {
                                    m_lastVisibleGroup = contextTabGroup;
                                }
                            }
                        }
                    }

                    contextTabGroup.RibbonTabs.CollectionChanged += ContextTabGroup_RibbonTabs_CollectionChanged;
                    contextTabGroup.IsGroupVisibleChanged += new PropertyChangedCallback(ContextTabGroup_IsGroupVisibleChanged);
                    break;

                case NotifyCollectionChangedAction.Move:
                    break;

                case NotifyCollectionChangedAction.Remove:

                    ContextTabGroup group = e.OldItems[0] as ContextTabGroup;

                    group.RibbonTabs.CollectionChanged -= ContextTabGroup_RibbonTabs_CollectionChanged;

                    //m_skipItemRemoveNotify = true;

                    RemoveContextTabs(group);

                    //m_skipItemRemoveNotify = false;

                    m_firstVisibleGroup = GetFisrtVisibleGroup(ContextTabGroups);

                    if (m_firstVisibleGroup == null)
                    {
                        HasVisibleContextTabGroup = false;
                    }

                    m_lastVisibleGroup = GetLastVisibleGroup(ContextTabGroups);

                    break;

                case NotifyCollectionChangedAction.Replace:

                    break;

                case NotifyCollectionChangedAction.Reset:
                    ContextTabGroupCollection ctgColl = new ContextTabGroupCollection();
                    foreach (RibbonTab tab in this.Items)
                    {
                        ContextTabGroup ctgTemp = tab.ContextTabGroup;
                        if (ctgTemp != null)
                        {
                            tab.ContextTabGroup = null;
                            if (ctgColl.Contains(ctgTemp) == false)
                            {
                                ctgColl.Add(ctgTemp);
                            }
                        }
                    }

                    foreach (ContextTabGroup ctg in ctgColl)
                    {
                        ctg.RibbonTabs.CollectionChanged -= ContextTabGroup_RibbonTabs_CollectionChanged;
                        //m_skipItemRemoveNotify = true;
                        RemoveContextTabs(ctg);
                        //m_skipItemRemoveNotify = false;
                        m_firstVisibleGroup = null;
                        m_lastVisibleGroup = null;
                        if (this.QuickAccessToolBar != null)
                        {
                            this.QuickAccessToolBar.ClearValue(QuickAccessToolBar.MaxWidthProperty);
                        }
                    }

                    break;
            }

        }

        /// <summary>
        /// Handles RibbonTabCollection changed event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The instance containing the event data.</param>
        private void ContextTabGroup_RibbonTabs_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            RibbonTabCollection ribbonTabCollection = sender as RibbonTabCollection;
            int count = ribbonTabCollection.Count;

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:

                    RibbonTab tab = e.NewItems[0] as RibbonTab;

                    ContextTabGroup ctg = null;
                    foreach (ContextTabGroup contextTabGroup in ContextTabGroups)
                    {
                        if (contextTabGroup.RibbonTabs.Contains(tab))
                        {
                            ctg = contextTabGroup;
                            break;
                        }
                    }

                    int newStartIndex = e.NewStartingIndex;
                    if (newStartIndex == count - 1)
                    {
                        int ctgsCount = ContextTabGroups.Count;
                        int ctgIndex = ContextTabGroups.IndexOf(ctg);

                        if (ctgsCount - 1 == ctgIndex)
                        {
                            if (tab.Parent is Ribbon && (tab.Parent as Ribbon).Items.Contains(tab))
                                (tab.Parent as Ribbon).Items.Remove(tab);

                            Items.Add(tab);
                            m_firstVisibleGroup = GetFisrtVisibleGroup(ContextTabGroups);
                            m_lastVisibleGroup = GetLastVisibleGroup(ContextTabGroups);
                        }
                        else
                        {
                            int ctgItemsCount = ctg.RibbonTabs.Count;

                            if (ctgItemsCount > 1)
                            {
                                RibbonTab temp = ctg.RibbonTabs[ctgItemsCount - 2];
                                int insertIndex = Items.IndexOf(temp) + 1;
                                Items.Insert(insertIndex, tab);
                            }
                            else
                            {
                                ContextTabGroup contextTabGroup = null;

                                for (int i = ctgIndex + 1; i < ctgsCount; i++)
                                {
                                    if (ContextTabGroups[i].RibbonTabs.Count != 0)
                                    {
                                        contextTabGroup = ContextTabGroups[i];
                                        break;
                                    }
                                }

                                if (contextTabGroup != null)
                                {
                                    int insertIndex = Items.IndexOf(contextTabGroup.RibbonTabs[0]);
                                    if (insertIndex > 0 && insertIndex < Items.Count)
                                    {
                                        Items.Insert(insertIndex, tab);
                                    }
                                }
                                else
                                {
                                    Items.Add(tab);
                                }

                                m_firstVisibleGroup = GetFisrtVisibleGroup(ContextTabGroups);
                                m_lastVisibleGroup = GetLastVisibleGroup(ContextTabGroups);
                            }
                        }
                    }
                    else
                    ////Insert
                    {
                        int insertIndex = Items.IndexOf(ctg.RibbonTabs[newStartIndex + 1]);
                        Items.Insert(insertIndex, tab);
                    }

                    break;
                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Remove:
                    if (ribbonTabCollection.Count == 0)
                    {
                        m_firstVisibleGroup = GetFisrtVisibleGroup(ContextTabGroups);
                        m_lastVisibleGroup = GetLastVisibleGroup(ContextTabGroups);
                    }

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
        /// Handles IsGroupVisibleChanged event.
        /// </summary>
        /// <param name="d">The source of the event</param>
        /// <param name="e">The instance containing the event data.</param>
        private void ContextTabGroup_IsGroupVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ContextTabGroup group = (ContextTabGroup)d;

            if (group.IsGroupVisible)
            {
                ClearContextTabs();

                if (m_firstVisibleGroup == null)
                {
                    m_firstVisibleGroup = group;
                }
                else
                {
                    int currentIndex = ContextTabGroups.IndexOf(m_firstVisibleGroup);
                    int index = ContextTabGroups.IndexOf(group);

                    if (index < currentIndex)
                    {
                        m_firstVisibleGroup = group;
                    }
                }

                if (m_lastVisibleGroup == null)
                {
                    m_lastVisibleGroup = group; ////GetLastVisibleGroup( ContextTabGroups );
                }
                else
                {
                    int currentIndex = ContextTabGroups.IndexOf(m_lastVisibleGroup);
                    int index = ContextTabGroups.IndexOf(group);

                    if (index > currentIndex)
                    {
                        m_lastVisibleGroup = group;
                    }
                }
                group.Visibility = Visibility.Visible;

                AddVisibleContextTabGroups();
            }
            else
            {
                m_firstVisibleGroup = GetFisrtVisibleGroup(ContextTabGroups);
                m_lastVisibleGroup = GetLastVisibleGroup(ContextTabGroups);
                group.Visibility = Visibility.Collapsed;
                RemoveContextTabs(group);
            }

            if (m_firstVisibleGroup == null)
            {
                HasVisibleContextTabGroup = false;
            }
            else
            {
                HasVisibleContextTabGroup = true;
            }

            if (group.IsGroupVisible)
            {
                if (group.RibbonTabs.Count > 0)
                {
                    RibbonTab rtab = group.RibbonTabs[0] as RibbonTab;
                    if (rtab != null)
                    {
                        this.SelectedItem = rtab;
                    }
                }
            }
            
            RibbonTab selectedTab = this.ItemContainerGenerator.ContainerFromItem(this.SelectedItem) as RibbonTab;

            if (!group.IsGroupVisible)
            {
                if (selectedTab == null)
                {
                    for (int i = 0; i < this.Items.Count; i++)
                    {
                        selectedTab = this.ItemContainerGenerator.ContainerFromIndex(0) as RibbonTab;
                        if (selectedTab != null)
                        {
                            SelectedIndex = Items.IndexOf(selectedTab);
                            break;
                        }
                    }
                }
            }

            if (selectedTab != null)
            {
                if (group.RibbonTabs.Contains(selectedTab))
                {
                    foreach (object tabObject in Items)
                    {
                        RibbonTab tab = this.ItemContainerGenerator.ContainerFromItem(tabObject) as RibbonTab;
                        if (tab != null)
                        {
                            if (tab.IsVisible)
                            {
                                tab.IsChecked = false;
                                selectedTab.IsChecked = true;
                                SelectedIndex = Items.IndexOf(selectedTab);
                                break;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Checks the context tab group visibility.
        /// </summary>
        /// <param name="tabGroup">The tab group.</param>
        private void CheckContextTabGroupVisibility(ContextTabGroup tabGroup)
        {
            ContextTabGroup group = tabGroup;

            if (group.IsGroupVisible)
            {
                if (m_firstVisibleGroup == null)
                {
                    m_firstVisibleGroup = group;
                }
                else
                {
                    int currentIndex = ContextTabGroups.IndexOf(m_firstVisibleGroup);
                    int index = ContextTabGroups.IndexOf(group);

                    if (index < currentIndex)
                    {
                        m_firstVisibleGroup = group;
                    }
                }

                if (m_lastVisibleGroup == null)
                {
                    m_lastVisibleGroup = group; ////GetLastVisibleGroup( ContextTabGroups );
                }
                else
                {
                    int currentIndex = ContextTabGroups.IndexOf(m_lastVisibleGroup);
                    int index = ContextTabGroups.IndexOf(group);

                    if (index > currentIndex)
                    {
                        m_lastVisibleGroup = group;
                    }
                }
                group.Visibility = Visibility.Visible;
            }
            else
            {
                m_firstVisibleGroup = GetFisrtVisibleGroup(ContextTabGroups);
                m_lastVisibleGroup = GetLastVisibleGroup(ContextTabGroups);
                group.Visibility = Visibility.Collapsed;
            }

            if (m_firstVisibleGroup == null)
            {
                HasVisibleContextTabGroup = false;
            }
            else
            {
                HasVisibleContextTabGroup = true;
            }

            RibbonTab selectedTab = this.ItemContainerGenerator.ContainerFromItem(this.SelectedItem) as RibbonTab;


            if (selectedTab != null)
            {
                if (group.RibbonTabs.Contains(selectedTab))
                {
                    foreach (object tabObject in Items)
                    {
                        RibbonTab tab = this.ItemContainerGenerator.ContainerFromItem(tabObject) as RibbonTab;
                        if (tab != null)
                        {
                            if (tab.IsVisible)
                            {
                                selectedTab.IsChecked = false;
                                tab.IsChecked = true;
                                SelectedIndex = Items.IndexOf(tab);
                                break;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Calls OnIsQATBelowChanged method of the instance, notifies of
        /// the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnIsQATBelowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Ribbon instance = (Ribbon)d;
            instance.OnIsQATBelowChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises IsQATBelowChanged
        /// event.
        /// </summary>
        /// <param name="e">Property change details, such as old value
        /// and new value.</param>
        protected virtual void OnIsQATBelowChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.IsQATBelow)
            {
                if (m_ribbonWindow != null)
                    m_ribbonWindow.TitleBar.QATColumnWidth = 0d;
            }

            if (IsQATBelowChanged != null)
            {
                IsQATBelowChanged(this, e);
            }
        }

        /// <summary>
        /// Handles Ribbon state changed event.
        /// </summary>
        /// <param name="d">The source of the event.</param>
        /// <param name="e">The instance containing the event data.</param>
        private void Ribbon_RibbonStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SynchronizeRibbonState();
        }

        /// <summary>
        /// Updates property value cache and raises RibbonStateChanged
        /// event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnRibbonStateChanged(DependencyPropertyChangedEventArgs e)
        {
            if ((RibbonState)e.NewValue == RibbonState.Adorner)
            {               
                ShowAdorned();
            }
            else
            {
                HideAdorned();               
            }

            HideBackStageInternal();

            if ((RibbonState)e.NewValue == RibbonState.Hide && (RibbonState)e.OldValue == RibbonState.Normal)
            {
                RibbonTab seltab = ItemContainerGenerator.ContainerFromItem(SelectedItem) as RibbonTab;
                if (seltab == null && SelectedIndex != -1)
                    seltab = (RibbonTab)this.Items[SelectedIndex];
                if (seltab != null)
                {
                    seltab.IsChecked = false;
                }
            }
            else if ((RibbonState)e.NewValue != RibbonState.Hide)
            {
                RibbonTab seltab =null;
                if(SelectedIndex != -1)
                    seltab = ItemContainerGenerator.ContainerFromIndex(SelectedIndex) as RibbonTab;
                if (seltab == null && SelectedIndex != -1)
                {
                    seltab = this.Items[SelectedIndex] as RibbonTab;
                }
                if (seltab != null)
                {
                    seltab.IsChecked = true;
                }
            }

            foreach (ContextTabGroup group in this.ContextTabGroups)
            {
                foreach (RibbonTab tab in group.RibbonTabs)
                {
                    tab.ContextAdorner = null;
                }
            }

            if (RibbonStateChanged != null)
            {
                RibbonStateChanged(this, e);
            }

            m_bItemsPanelAdornerLayerChanged = true;
        }

        /// <summary>
        /// Calls OnHasVisibleContextTabGroupChanged method of the
        /// instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value
        /// and new value.</param>
        private static void OnHasVisibleContextTabGroupChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Ribbon instance = (Ribbon)d;
            instance.OnHasVisibleContextTabGroupChanged(e);
        }

        /// <summary>
        /// Called when [selected index changed].
        /// </summary>
        /// <param name="d">The Dependency.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnSelectedIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Ribbon instance = (Ribbon)d;
            instance.OnSelectedIndexChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises
        /// HasVisibleContextTabGroupChanged event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnHasVisibleContextTabGroupChanged(DependencyPropertyChangedEventArgs e)
        {
            if (HasVisibleContextTabGroupChanged != null)
            {
                HasVisibleContextTabGroupChanged(this, e);
            }
        }

        /// <summary>
        /// Fires the QAT items collection changed.
        /// </summary>
        /// <param name="e">The instance containing the event data.</param>
        protected internal virtual void FireQATItemsCollectionChanged(QATItemsCollectionChangedEventArgs e)
        {
            if (QATItemsCollectionChanged != null)
            {
                QATItemsCollectionChanged(e);
            }
        }


        #region State Persistence

        /// <summary>
        /// Saves the initial state.
        /// </summary>
        private void SaveInitialState()
        {
            SaveDefaultState(reset_StoreFile);
        }

        private void SaveDefaultState(string saveFilename)
        {
            IsolatedStorageFile isoStorage = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);

            StringBuilder QATDynamicItemsString = new StringBuilder();

            Dictionary<FrameworkElement, string> paths = new Dictionary<FrameworkElement, string>();

            if (this.QuickAccessToolBar != null && this.QuickAccessToolBar.AutoPersist)
            {
                TraverseAndFindLogicalTree(this, "", paths);
                if (QATItems.Count <= QuickAccessToolBar.Items.Count)
                {
                    foreach (var element in QATItems)
                    {
                        if (paths.ContainsKey((FrameworkElement) element.Value))
                        {
                            QATDynamicItemsString.Append(paths[(FrameworkElement) element.Value]);
                            QATDynamicItemsString.Append(';');
                        }
                    }
                }
                else
                {
                    foreach (var element in QuickAccessToolBar.Items)
                    {
                        if (paths.ContainsKey((FrameworkElement) element))
                        {
                            QATDynamicItemsString.Append(paths[(FrameworkElement) element]);
                            QATDynamicItemsString.Append(';');
                        }
                    }
                }
            }

            ArrayList windowCoordinates = new ArrayList();

            if (this.m_ribbonWindow != null && this.m_ribbonWindow.AutoPersist)
            {
                windowCoordinates.Add(RootWindow.Top);
                windowCoordinates.Add(RootWindow.Left);
                windowCoordinates.Add(RootWindow.Width);
                windowCoordinates.Add(RootWindow.Height);

                if (RootWindow.WindowState == WindowState.Maximized)
                    windowCoordinates.Add("true");
                else
                    windowCoordinates.Add("false");
            }

            RibbonStateParams stateParams = new RibbonStateParams(this.QuickAccessToolBar,qatApplicationItemIndex, QATDynamicItemsString.ToString(), this.QATInitialItemsString.ToString(), this.IsQATBelow, this.RibbonState, windowCoordinates);
            IsolatedStorageFileStream stream = new IsolatedStorageFileStream(saveFilename, FileMode.Create, FileAccess.Write, isoStorage);
            XamlWriter.Save(stateParams, stream);
            Params = stateParams;
            stream.Close();
        }

        /// <summary>
        /// Saves the state of the ribbon.
        /// </summary>
        public void SaveRibbonState()
        {
            IsolatedStorageFile storage = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);
            QATItemCollection.Clear();
            foreach (UIElement element in QuickAccessToolBar.Items)
            {
                QATItemCollection.Add(element);
            }
            foreach (UIElement keyElement in QATItems.Keys)
            {
                if (QATItemCollection.Contains(keyElement) && !keyElement.Equals(QATItems[keyElement]))
                {
                    int index = QATItemCollection.IndexOf(keyElement);
                    QATItemCollection.RemoveAt(index);
                    QATItemCollection.Insert(index, QATItems[keyElement]);
                }
            }    
            SaveRibbonState(storage, default_StoreFile);
        }

        /// <summary>
        /// Saves the state of the ribbon.
        /// </summary>
        /// <param name="xmlWriter">The XmlWriter for saving.</param>
        public void SaveRibbonState(XmlWriter xmlWriter)
        {
            if (xmlWriter == null)
            {
                throw new ArgumentNullException("xmlWriter");
            }

            RibbonStateParams stateParams = CreateSaveData();
            Params = stateParams;
            using (xmlWriter)
            {
                XamlWriter.Save(stateParams, xmlWriter);
            }
        }


        /// <summary>
        /// Saves the state of the ribbon.
        /// </summary>
        /// <param name="isoStorage">The iso storage.</param>
        /// <param name="storeFileName">Name of the store file.</param>
        public void SaveRibbonState(IsolatedStorageFile isoStorage, string storeFileName)
        {
            RibbonStateParams stateParams = CreateSaveData();
            Params = stateParams;
            using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream(storeFileName, FileMode.Create, FileAccess.Write, isoStorage))
            {
                XamlWriter.Save(stateParams, stream);
            }
        }

        private RibbonStateParams CreateSaveData()
        {
            StringBuilder QATDynamicItemsString = new StringBuilder();

            Dictionary<FrameworkElement, string> paths = new Dictionary<FrameworkElement, string>();

            if (this.QuickAccessToolBar != null && this.PersistElements.Contains(RibbonElements.QuickAccessToolbar))
            {
                TraverseAndFindLogicalTree(this, "", paths);
                
                if(this.ApplicationMenu != null)
                TraverseAndFindLogicalTree(this.ApplicationMenu, "", paths);

                if (this.ApplicationMenu != null && this.ApplicationMenu.MenuItems != null)
                {
                    string path = string.Empty;
                    for (int i = 0; i < ApplicationMenu.MenuItems.Count; i++)
                    {
                        DependencyObject child = ApplicationMenu.MenuItems[i] as DependencyObject;
                        if (child == null) continue;
                        TraverseAndFindLogicalTree(child, path + i, paths);
                    }
                }

                if (this.ApplicationMenu != null && this.ApplicationMenu.ApplicationItems != null)
                {
                    string path = string.Empty;
                    for (int i = 0; i < ApplicationMenu.ApplicationItems.Count; i++)
                    {
                        DependencyObject child = ApplicationMenu.ApplicationItems[i] as DependencyObject;
                        if (child == null) continue;
                        TraverseAndFindLogicalTree(child, path + i, paths);
                    }
                }
                if (this.BackStage != null && this.BackStage.Items != null)
                {
                    string path = string.Empty;
                    for (int i = 0; i < BackStage.Items.Count; i++)
                    {
                        DependencyObject child = BackStage.Items[i] as DependencyObject;
                        if (child == null) continue;
                        TraverseAndFindLogicalTree(child, path + i, paths);
                    }
                }

                foreach (var element in QATItems)
                {
                    if (paths.ContainsKey((FrameworkElement)element.Value))
                    {
                        QATDynamicItemsString.Append(paths[(FrameworkElement)element.Value]);
                        QATDynamicItemsString.Append(';');
                    }
                }
            }

            ArrayList windowCoordinates = new ArrayList();

            if (this.m_ribbonWindow != null && this.PersistElements.Contains(RibbonElements.RibbonWindow))
            {
                windowCoordinates.Add(RootWindow.Top);
                windowCoordinates.Add(RootWindow.Left);
                windowCoordinates.Add(RootWindow.Width);
                windowCoordinates.Add(RootWindow.Height);

                if (RootWindow.WindowState == WindowState.Maximized)
                    windowCoordinates.Add("true");
                else
                    windowCoordinates.Add("false");
            }
           
            return new RibbonStateParams(this.QuickAccessToolBar,qatApplicationItemIndex, QATDynamicItemsString.ToString(), this.QATInitialItemsString.ToString(), this.IsQATBelow, this.RibbonState, windowCoordinates);
        }


        /// <summary>
        /// Traverses the and find logical tree.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="path">The path.</param>
        /// <param name="paths">The paths.</param>
        void TraverseAndFindLogicalTree(DependencyObject item, string path, IDictionary<FrameworkElement, string> paths)
        {
            FrameworkElement uielement = item as FrameworkElement;
            if (uielement != null && QATItems.ContainsValue(uielement))
            {
                if (!paths.ContainsKey(uielement))
                    paths.Add(uielement, path);
            }

            object[] children = LogicalTreeHelper.GetChildren(item).Cast<object>().ToArray();

            for (int i = 0; i < children.Length; i++)
            {
                DependencyObject child = children[i] as DependencyObject;
                if (child == null) continue;
                TraverseAndFindLogicalTree(child, path + i + ",", paths);
            }
        }

		/// <summary>
		/// Loads the default state.
		/// </summary>
		private void LoadDefaultState(string loadFilename)
		{
			IsolatedStorageFile isoStorage = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);

			if (FileExists(isoStorage, loadFilename))
			{
				Stream stream = new IsolatedStorageFileStream(loadFilename, FileMode.Open, isoStorage);

				RibbonStateParams stateParams = XamlReader.Load(stream) as RibbonStateParams;

				stream.Close();

				bool laodWindowState = this.m_ribbonWindow != null && this.m_ribbonWindow.AutoPersist;
                if(stateParams.QATItemsIndex.Count > 0)
                Params = stateParams;
                if (stateParams != null && stateParams.QatApplicationItemIndex != null)
                {
                    qatApplicationItemIndex = stateParams.QatApplicationItemIndex;
                }
				ApplyStateParams(stateParams, laodWindowState, this.AutoPersist);
			}
		}


        /// <summary>
        /// Loads the state of the ribbon.
        /// </summary>
        public void LoadRibbonState()
        {
            IsolatedStorageFile storage = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);
            LoadRibbonState(storage, default_StoreFile);
        }


        /// <summary>
        /// Loads the state of the ribbon.
        /// </summary>
        /// <param name="reader">The TextReader to load the data.</param>
        public void LoadRibbonState(TextReader reader)
        {
            using (XmlTextReader readerXML = new XmlTextReader(reader))
            {
                LoadRibbonState(readerXML);
            }
        }

        /// <summary>
        /// Loads the state of the ribbon.
        /// </summary>
        /// <param name="xmlReader">The XmlReader to load the data.</param>
        public void LoadRibbonState(XmlReader xmlReader)
        {
            isXMLStateLoading = true;
            RibbonStateParams stateParams;
            using (xmlReader)
            {
                stateParams = XamlReader.Load(xmlReader) as RibbonStateParams;
            }
            if (stateParams != null && stateParams.QATItemsIndex.Count > 0)
                CheckQATInitialItems();

            if (stateParams != null && stateParams.QatApplicationItemIndex != null)
            {
                qatApplicationItemIndex = stateParams.QatApplicationItemIndex;
            }
            bool loadWindowState = this.m_ribbonWindow != null && this.PersistElements.Contains(RibbonElements.RibbonWindow);
            bool loadRibbonState = this.PersistElements.Contains(RibbonElements.Ribbon);

            if (stateParams != null)
                ApplyStateParams(stateParams, loadWindowState, loadRibbonState);

            Params = stateParams;
            if (Params != null && Params.QATItemsIndex.Count > 0)
            {
                UpdateQATItems();
            }
            LoadCustomizeRibbon();
        }

        /// <summary>
        /// Loads the state of the ribbon.
        /// </summary>
        /// <param name="isoStorage">The iso storage.</param>
        /// <param name="loadFileName">Name of the load file.</param>
        public void LoadRibbonState(IsolatedStorageFile isoStorage, string loadFileName)
        {
            if (FileExists(isoStorage, loadFileName) && !isXMLStateLoading)
            {
                RibbonStateParams stateParams;
                using (Stream stream = new IsolatedStorageFileStream(loadFileName, FileMode.Open, isoStorage))
                {
                    stateParams = XamlReader.Load(stream) as RibbonStateParams;
                }
                if (stateParams != null && stateParams.QatApplicationItemIndex != null)
                {
                    qatApplicationItemIndex = stateParams.QatApplicationItemIndex;
                }
                bool loadWindowState = this.m_ribbonWindow != null && this.PersistElements.Contains(RibbonElements.RibbonWindow);
                bool loadRibbonState = this.PersistElements.Contains(RibbonElements.Ribbon);

                ApplyStateParams(stateParams, loadWindowState, loadRibbonState);
                Params = stateParams;
                if (Params != null && Params.QATItemsIndex.Count > 0)
                {
                    UpdateQATItems();
                }

                LoadCustomizeRibbon();
            }
            else
            {
                AttachIndex();
                AttachHeaders();
            }
        }

        public RibbonStateParams GetSerializedList(XmlReader xmlReader)
        {
            RibbonStateParams stateParams = null;
            using (xmlReader)
            {
                stateParams = XamlReader.Load(xmlReader) as RibbonStateParams;
            }

            return stateParams;
        }

        private void LoadCustomizeRibbon()
        {
            if (this.ShowCustomizeRibbon)
            {
                //Customize Ribbon
                CreateCustomRibbonTab();
                AttachHeaders();
                AttachIndex();
                ChangeTabOrder();
                ChangeTabVisibility();
                ChangeBarOrder();
                RenameTab();
                RenameBar();
            }
        }

        private void CreateCustomRibbonTab()
        {
            if (Params != null)
            {
                foreach (var customTab in Params.RibbonCustomTabCollection)
                {
                    RibbonCustomTab tab = customTab as RibbonCustomTab;
                    if (tab != null)
                    {
                        RibbonTab newTab = new RibbonTab() { Caption = tab.Caption, Tag = "True" };
                        foreach (var bar in tab.RibbonBarCollection)
                        {
                            int customCommnadCount = 0;
                            RibbonCustomBar customBar = bar as RibbonCustomBar;
                            if (customBar != null)
                            {
                                RibbonBar newBar = new RibbonBar() { Header = customBar.Header, Tag = "True" };

                                foreach (var arrayList in customBar.ItemIndex)
                                {
                                    IRibbonControl control = null;
                                    string[] itemIndex = arrayList.ToString().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                                    RibbonTab tabValue = itemIndex.Count() != 1 ? (itemIndex.Count() > int.Parse(itemIndex[0]) ? this.Items[int.Parse(itemIndex[0])] as RibbonTab : null) : null;
                                    if (tabValue != null)
                                    {
                                        RibbonBar barValue = itemIndex.Count() >= 2 ? (tabValue.Items.Count > int.Parse(itemIndex[1]) ? tabValue.Items[int.Parse(itemIndex[1])] as RibbonBar:null) : null;
                                        if (barValue != null)
                                        {
                                            bool isButtonPanel = itemIndex.Count() >= 3 ? ( barValue.Items.Count > int.Parse(itemIndex[2]) ? barValue.Items[int.Parse(itemIndex[2])] is ButtonPanel : false):false;
                                            if (isButtonPanel )
                                            {
                                                ButtonPanel panel = itemIndex.Count() >= 4 ? (barValue.Items.Count > int.Parse(itemIndex[3]) ? barValue.Items[int.Parse(itemIndex[3])] as ButtonPanel : null):null;
                                                if (panel != null)
                                                    control = itemIndex.Count() >= 3 ? (panel.Items.Count > int.Parse(itemIndex[2]) ? panel.Items[int.Parse(itemIndex[2])] as IRibbonControl : null):null;
                                                else
                                                    control = itemIndex.Count() >= 3 ? (barValue.Items.Count > int.Parse(itemIndex[2]) ? barValue.Items[int.Parse(itemIndex[2])] as IRibbonControl : null):null;

                                            }
                                            else
                                            {
                                                control = itemIndex.Count() >= 3 ? (barValue.Items.Count > int.Parse(itemIndex[2]) ? barValue.Items[int.Parse(itemIndex[2])] as IRibbonControl : null):null;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (itemIndex.Count()==1)
                                        {
                                            if (itemIndex[0].ToString().EndsWith("q"))
                                                control = QuickAccessToolBar.QATMenuItems[int.Parse(itemIndex[0].Replace('q',' '))] as IRibbonControl;
                                            else
                                                control = BackStage.Items[int.Parse(itemIndex[0])] as IRibbonControl;
                                        }
                                    }
                                    if (control != null)
                                    {
                                        UIElement cloned = CloneManager.CloneGeneral(control as UIElement, true) as UIElement;

                                        if (cloned is RibbonButton)
                                        {
                                            (cloned as RibbonButton).SizeForm = SizeForm.Small;
                                            (cloned as RibbonButton).Label = customBar.CustomCommandList[customCommnadCount].ToString();
                                            customCommnadCount++;
                                        }

                                        newBar.Items.Add(cloned);
                                    }

                                }
                                newTab.Items.Add(newBar);
                            }
                        }

                        this.Items.Add(newTab);
                    }
                }
            }
        }


        private void AttachHeaders()
        {
            foreach (RibbonTab item in this.Items)
            {
                QATCustomizationDialog.SetTabOriginalHeader(item, item.Caption);

                foreach (RibbonBar ribbonBar in item.Items)
                {
                    QATCustomizationDialog.SetBarOriginalHeader(ribbonBar, ribbonBar.Header);
                }
            }
        }

        private void AttachIndex()
        {
            foreach (RibbonTab item in this.Items)
            {
                QATCustomizationDialog.SetTabOriginalIndex(item, this.Items.IndexOf(item));

                foreach (RibbonBar ribbonBar in item.Items)
                {
                    QATCustomizationDialog.SetBarOriginalIndex(ribbonBar, item.Items.IndexOf(ribbonBar));
                }
            }
        }

        ArrayList list = new ArrayList();
        List<RibbonBarOrder> barList = new List<RibbonBarOrder>();  

        private void ChangeTabOrder()
        {
            list.Clear();
            if (Params != null)
            {                
                foreach (var tabOrder in Params.TabOrderList)
                {                    
                    foreach (var tab in this.Items)
                    {
                        if (tab is RibbonTab)
                        {
                            string caption = (tab as RibbonTab).Caption;
                            string tabOrderCaption = (tabOrder as TabItemState).OriginalCaption;
                            if (caption == tabOrderCaption)
                            {
                                if (!list.Contains(tab as RibbonTab))
                                {
                                    list.Add(tab as RibbonTab);
                                    break;
                                }
                            }
                        }
                    }
                }
            }           

            if (list.Count > 0)
            {
                int newPos = 0;
                foreach (var item in list)
                {
                    int oldPos = this.Items.IndexOf(item as RibbonTab);

                    RibbonTab oldTab = this.Items[oldPos] as RibbonTab;

                    this.Items.Remove(oldTab);
                    this.Items.Insert(newPos, oldTab);
                    newPos++;
                }
            }
        }

        private void ChangeBarOrder()
        {
            barList.Clear();
            if (Params != null)
            {
                foreach (var barOrder in Params.BarOrderList)
                {
                    int index = (barOrder as BarItemState).TabIndex;
                    string barHeader = (barOrder as BarItemState).OriginalHeader; 

                    RibbonTab tab = this.Items[index] as RibbonTab;

                    foreach (var ribbonBar in tab.Items)
                    {
                        string header = (ribbonBar as RibbonBar).Header;

                        if (header == barHeader)
                        {
                            RibbonBarOrder bar = new RibbonBarOrder();
                            bar.TabIndex = index;
                            bar.RibbonBarItem = ribbonBar as RibbonBar;
                            if (!barList.Contains(bar))
                                barList.Add(bar);
                        }
                    }
                }
            }

            if (barList.Count > 0)
            {
                int previousIndex = 0;
                int newPos = 0;
                foreach (var item in barList)
                {
                    if (previousIndex != item.TabIndex)
                        newPos = 0;

                    RibbonTab tab = this.Items[item.TabIndex] as RibbonTab;
                    int oldPos = tab.Items.IndexOf(item.RibbonBarItem as RibbonBar);
                    RibbonBar oldBar = tab.Items[oldPos] as RibbonBar;

                    tab.Items.Remove(oldBar);
                    tab.Items.Insert(newPos, oldBar);
                    newPos++;
                    previousIndex = item.TabIndex;
                }
            }
        }


        private void ChangeTabVisibility()
        {
            if (Params != null)
            {
                foreach (var tabOrder in Params.CollapsedTabList)
                {
                    RibbonTab tab = this.Items[(int)tabOrder] as RibbonTab;
                    if (tab != null)
                        tab.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void RenameTab()
        {
            int tabIndex=0;
            if (Params != null)
            {
                foreach (var tabOrder in Params.TabOrderList)
                {
                    RibbonTab tab = this.Items.Count > tabIndex ? this.Items[tabIndex] as RibbonTab : null;
                    if (tab != null)
                    {
                        tab.Caption = (tabOrder as TabItemState).Caption;
                        tabIndex++;
                    }
                }
            }
        }

        private void RenameBar()
        {
            int barIndex = 0;
            for (int i = 0; i < this.Items.Count; i++)
            {
                RibbonTab tab = this.Items[i] as RibbonTab;
                if (tab != null)
                {
                    foreach (RibbonBar item in tab.Items)
                    {
                        if (Params.BarOrderList.Count > barIndex)
                            item.Header = (Params.BarOrderList[barIndex] as BarItemState).Header;
                        barIndex++;
                    }
                }
            }
        }

        /// <summary>
        /// Applies the persisted indexes of the QAT items.
        /// </summary>
        /// <param name="qatItemIndexes"></param>
        private void ApplyQATItemsIndex(ArrayList qatItemIndexes)
        {

            this.QuickAccessToolBar.InternalCommandManager.SortByIndex();

            QuickAccessToolBar.LockQATItemsChange();
            QuickAccessToolBar.Items.Clear();
            this.UpdateItems();
            QuickAccessToolBar.UnlockQATItemsChange();
        }

        /// <summary>
        /// Parses and add to QAT.
        /// </summary>
        /// <param name="data">The data.</param>
        private void ParseAndAddToQAT(string data)
        {
            int[] indices = data.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => Int32.Parse(x)).ToArray();
            if (tempQATItems.Count > 0)
            {
                UIElement current = this;
                object[] children = LogicalTreeHelper.GetChildren(current).OfType<object>().ToArray();
                current = FindQATElement(ApplicationMenu);
                if (current == null && this.ApplicationMenu != null && this.ApplicationMenu.MenuItems != null)
                {
                    int menuindex = 0;

                    for (int i = 0; i < this.ApplicationMenu.MenuItems.Count; i++)
                    {
                        UIElement child = this.ApplicationMenu.MenuItems[i] as UIElement;

                        if (child != null && tempQATItems.Contains(child))
                        {
                            menuindex = tempQATItems.IndexOf(child);
                        }
                        if (tempQATItems[menuindex].Equals(child))
                        {
                            current = child;
                            break;
                        }
                    }

                    if (current != null)
                    {
                        tempQATItems.Remove(current as UIElement);
                        UIElement menuresult = current as UIElement;
                        AddItemToQAT(menuresult);
                        hadFound = false;
                        return;
                    }
                }
                if (current == null && this.ApplicationMenu != null && this.ApplicationMenu.ApplicationItems != null)
                {
                    int menuindex = 0;

                    for (int i = 0; i < this.ApplicationMenu.ApplicationItems.Count; i++)
                    {
                        UIElement child = this.ApplicationMenu.ApplicationItems[i] as UIElement;

                        if (child != null && tempQATItems.Contains(child) && this.ApplicationMenu.MenuItems.Contains(child))
                        {
                            menuindex = tempQATItems.IndexOf(child);
                        }
                        if (tempQATItems[menuindex].Equals(child))
                        {
                            current = child;
                            break;
                        }
                    }

                    if (current != null)
                    {
                        tempQATItems.Remove(current as UIElement);
                        UIElement menuresult = current as UIElement;
                        AddItemToQAT(menuresult);
                        hadFound = false;
                        return;
                    }
                }
                int index = 0;
                if (QATInitialItems.Count > 0)
                {
                    index = QATInitialItems.Count;
                }
                if (current != null && tempQATItems.Contains(current as UIElement))
                {
                    index = tempQATItems.IndexOf(current);
                }
                if (tempQATItems[index].Equals(current as UIElement) && ApplicationMenu != null)
                {
                    UIElement checkElement = ApplicationMenu;
                    for (int i = 0; i < indices.Length; i++)
                    {
                        if (((UIElement)current).Visibility == Visibility.Visible)
                        {
                            children = LogicalTreeHelper.GetChildren(checkElement).OfType<object>().ToArray();
                            bool indexIsInvalid = children.Length <= indices[i];
                            DependencyObject item = indexIsInvalid ? null : children[indices[i]] as DependencyObject;

                            if (item == null)
                            {
                                checkElement = this;
                            }
                            else
                            {
                                checkElement = item as UIElement;
                            }
                        }
                    }
                    if (!tempQATItems.Contains(checkElement))
                    {
                        current = this;
                        for (int i = 0; i < indices.Length; i++)
                        {
                            if (((UIElement)current).Visibility == Visibility.Visible)
                            {
                                children = LogicalTreeHelper.GetChildren(current).OfType<object>().ToArray();
                                bool indexIsInvalid = children.Length <= indices[i];
                                DependencyObject item = indexIsInvalid ? null : children[indices[i]] as DependencyObject;

                                if (item == null)
                                    return;

                                current = item as UIElement;
                            }

                            else
                                return;
                        }
                    }
                    else
                    {
                        current = checkElement;
                    }

                    }
                else if (this.BackStage != null && this.BackStage.Items.Count > 0 && indices.Length <= 2 && data.Split(',')[0].Length==2)
                {
                    DependencyObject currnt1 = this;
                    for (int i = 0; i < indices.Length; i++)
                    {
                        if (((UIElement)currnt1).Visibility == Visibility.Visible)
                        {
                            object[] children1 = LogicalTreeHelper.GetChildren(this.BackStage).OfType<object>().ToArray();
                            if (currnt1 is BackstageTabItem)
                                children1 = LogicalTreeHelper.GetChildren(currnt1).OfType<object>().ToArray();

                            bool indexIsInvalid = children1.Length <= indices[i];
                            DependencyObject item = indexIsInvalid ? null : children1[indices[i]] as DependencyObject;


                            if (item == null && indexIsInvalid && children1.Length > 0 && children1[0] is Panel)
                                item = children1[0] as DependencyObject;

                            if (item == null)
                                return;

                            if (item is Panel)
                            {
                                children1 = LogicalTreeHelper.GetChildren(item).OfType<object>().ToArray();
                                indexIsInvalid = children1.Length <= indices[i];
                                item = indexIsInvalid ? null : children1[indices[i]] as DependencyObject;

                                if (item == null)
                                    return;
                            }

                            currnt1 = item;
                        }

                        else
                            return;
                    }
                    UIElement rsultNew = currnt1 as UIElement;
                    if (!this.QuickAccessToolBar.Items.Contains(rsultNew))
                        AddItemToQAT(rsultNew);
                    return;
                }
                else
                {
                    current = this;
                    for (int i = 0; i < indices.Length; i++)
                    {
                        if (((UIElement)current).Visibility == Visibility.Visible)
                        {
                            children = LogicalTreeHelper.GetChildren(current).OfType<object>().ToArray();
                            bool indexIsInvalid = children.Length <= indices[i];
                            DependencyObject item = indexIsInvalid ? null : children[indices[i]] as DependencyObject;

                            if (item == null)
                                return;

                            current = item as UIElement;
                        }

                        else
                            return;
                    }
                }
                tempQATItems.Remove(current as UIElement);
                UIElement result = current as UIElement;
                AddItemToQAT(result);
                hadFound = false;
            }
			DependencyObject currnt = this;

            if (this.BackStage != null && this.BackStage.Items.Count > 0 && indices.Length <= 2 && data.Split(',')[0].Length==2)
            {
                for (int i = 0; i < indices.Length; i++)
                {
                    if (((UIElement)currnt).Visibility == Visibility.Visible)
                    {
                        object[] children = LogicalTreeHelper.GetChildren(this.BackStage).OfType<object>().ToArray();
                        if (currnt is BackstageTabItem)
                            children = LogicalTreeHelper.GetChildren(currnt).OfType<object>().ToArray();

                        bool indexIsInvalid = children.Length <= indices[i];
                        DependencyObject item = indexIsInvalid ? null : children[indices[i]] as DependencyObject;

                        if (item == null)
                            return;

                        if (item is Panel)
                        {
                            children = LogicalTreeHelper.GetChildren(item).OfType<object>().ToArray();
                            indexIsInvalid = children.Length <= indices[i];
                            item = indexIsInvalid ? null : children[indices[i]] as DependencyObject;

                            if (item == null)
                                return;
                        }

                        currnt = item;
                    }

                    else
                        return;
                }
                UIElement rsultNew = currnt as UIElement;
                if (!this.QuickAccessToolBar.Items.Contains(rsultNew))
                    AddItemToQAT(rsultNew);
                return;
            }

            for (int i = 0; i < indices.Length; i++)
               {
                   if (((UIElement)currnt).Visibility == Visibility.Visible)
                   {
                       object[] children = LogicalTreeHelper.GetChildren(currnt).OfType<object>().ToArray();
                       bool indexIsInvalid = children.Length <= indices[i];
                       DependencyObject item = indexIsInvalid ? null : children[indices[i]] as DependencyObject;

                       if (item == null)
                           return;

                       currnt = item;
                   }

                   else
                       return;
               }

            UIElement rsult = currnt as UIElement;
            AddItemToQAT(rsult);
        }


        /// <summary>
        /// Finds QAT element from ApplicationMenu.
        /// </summary>
        /// <param name="rootElement">The root element.</param>
        /// <returns>Return the QAT element</returns>
        private UIElement FindQATElement(UIElement rootElement)
        {
            UIElement result = null;
            if (!hadFound && rootElement != null)
            {
                object[] childrenCount = LogicalTreeHelper.GetChildren(rootElement).OfType<object>().ToArray();
                for (int i = 0; i < childrenCount.Length; i++)
                {
                    UIElement child = childrenCount[i] as UIElement;
                    int index = 0;
                    if (QATInitialItems.Count > 0)
                    {
                        index = QATInitialItems.Count;
                    }
                    if (child != null && tempQATItems.Contains(child))
                    {
                        index = tempQATItems.IndexOf(child);
                    }
                    if (index < tempQATItems.Count && tempQATItems[index].Equals(child))
                    {
                        result = child;
                        hadFound = true;
                        break;
                    }
                    else if (child != null)
                    {
                        if (!hadFound)
                            result = FindQATElement(child as UIElement);
                    }
                }
            }

            return result;
        }
        /// <summary>
        /// Files whether exists.
        /// </summary>
        /// <param name="storage">The storage.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <returns></returns>
        static bool FileExists(IsolatedStorageFile storage, string fileName)
        {
            string[] files = storage.GetFileNames(fileName);
            return files.Length != 0;
        }

        /// <summary>
        /// Resets the state of the ribbon.
        /// </summary>
        public void ResetRibbonState()
        {
            LoadDefaultState(reset_StoreFile);
        }


        /// <summary>
        /// Deletes the state of the ribbon.
        /// </summary>
        public void DeleteRibbonState()
        {
            IsolatedStorageFile storage = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);
            DeleteRibbonState(storage, default_StoreFile);
        }

        /// <summary>
        /// Deletes the state of the ribbon.
        /// </summary>
        /// <param name="isoStorage">The iso storage.</param>
        /// <param name="deleteFileName">Name of the delete file.</param>
        public void DeleteRibbonState(IsolatedStorageFile isoStorage, string deleteFileName)
        {
            if (!string.IsNullOrEmpty(deleteFileName) && 0 < isoStorage.GetFileNames(deleteFileName).Length)
            {
                isoStorage.DeleteFile(deleteFileName);
            }
        }

		/// <summary>
		/// Applies the state paramerters to this Ribbon instance.
		/// </summary>
		/// <param name="stateParams">the persisted state parameters.</param>
		/// <param name="loadWindowState">true to apply persisted window state.</param>
		/// <param name="loadRibbonState">true to apply persisted Ribbon State.</param>
		private void ApplyStateParams(RibbonStateParams stateParams, bool loadWindowState, bool loadRibbonState)
		{
			string QATString = stateParams.QATItemsString;

            var items = QATString.ToString().Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Distinct().ToList();

            var initialItems = stateParams.QATItemsInitialString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Distinct().ToList();
            tempQATItems.Clear();
            if (ApplicationMenu != null && qatApplicationItemIndex !=null && qatApplicationItemIndex.Count > 0 && QATItemCollection.Count==0)
            {
                for (int iCount = 0; iCount < qatApplicationItemIndex.Count; iCount++)
                {
                    object item = qatApplicationItemIndex[iCount];

                    if (item != null)
                    {
                        if (item.ToString().Contains(','))
                        {
                            string[] menuItem = item.ToString().Split(',');

                            if (menuItem.Length >= 2)
                            {
                                if (menuItem.Contains("MenuGroup"))
                                {
                                    int i = int.Parse(menuItem[0]);
                                    int j = int.Parse(menuItem[1]);

                                    QATItemCollection.Add(((ApplicationMenu.Items[i] as SplitMenuButton).Items[0] as ApplicationMenuGroup).Items[j] as UIElement);
                                }

                                if (menuItem.Contains("MenuItems"))
                                {
                                    int i = int.Parse(menuItem[0]);
                                    QATItemCollection.Add(ApplicationMenu.MenuItems[i] as UIElement);
                                }

                                if (menuItem.Contains("AppItems"))
                                {
                                    int i = int.Parse(menuItem[0]);
                                    QATItemCollection.Add(ApplicationMenu.ApplicationItems[i] as UIElement);
                                }
                            }
                        }
                        else
                        {
                            int i = int.Parse(item.ToString());
                            QATItemCollection.Add(ApplicationMenu.Items[i] as UIElement);
                        }
                    }
                }

                //foreach (string i in qatApplicationItemIndex)
                //{
                //    //QATItemCollection.Add(ApplicationMenu.Items[i] as UIElement);
                //}
            }
            foreach (UIElement element in QATItemCollection)
            {
                tempQATItems.Add(element);
            }
			if (this.QuickAccessToolBar != null && (this.QuickAccessToolBar.AutoPersist || this.PersistElements.Contains(RibbonElements.QuickAccessToolbar)))
			{
                if (this.QuickAccessToolBar.Items.Count > 0)
                {
					 List<UIElement> qatItems = new List<UIElement>();
                    foreach (var item in QuickAccessToolBar.Items) 
                    {
                        qatItems.Add(item as UIElement);
                    }
                    this.QuickAccessToolBar.InternalCommandManager.m_defaultQATItems = qatItems;
                }
                if (stateParams.QATItemsIndex.Count > 0 && QuickAccessToolBar.Items.Count>0)
			    {
                   List<UIElement> targetElementList = new List<UIElement>();
                   for (int index = 0; index < stateParams.QATItemsIndex.Count; index++)
                   {
                       for (int i = 0; i < QuickAccessToolBar.Items.Count; i++)
                       {
                           if ((QuickAccessToolBar.Items[i] is RibbonButton) && (QuickAccessToolBar.Items[i] as RibbonButton).Label ==
                               (stateParams.QATItemsIndex[index] as QATItemState).Label)
                           {
                               targetElementList.Add(QuickAccessToolBar.Items[i] as UIElement);
                           }
                       }
                   }

                   if (this.AutoPersist)
                   {
                       QuickAccessToolBar.Items.Clear();
                       QuickAccessToolBar.InternalCommandManager.Clear();

                       foreach (var uiElement in targetElementList)
                       {
                           if (uiElement != null)
                               AddItemToQAT(uiElement);
                       }
                   }
                      
			        
			    }
			    else
			    {
			        QATItems.Clear();
			        QuickAccessToolBar.CheckQATItemsCommand();
			        QuickAccessToolBar.Items.Clear();
			        QuickAccessToolBar.InternalCommandManager.Clear();

			        for (int index = 0; index < this.QATInitialItems.Count; index++)
			        {
			            if (!initialItems.ToList().Contains(index.ToString()) && QATString!=string.Empty)
			            {
			                UIElement target = this.QATInitialItems.ElementAt(index);
			                QuickAccessToolBar.NeedItemWrap = false;
			                QuickAccessToolBarItem item = new QuickAccessToolBarItem(target);

			                if (item.SourceElement is RibbonButton)
			                {
			                    RibbonButton ribbonButton = (RibbonButton) item.SourceElement;
			                    if (ribbonButton.IsMenuItem)
			                        continue;
			                }

			                QuickAccessToolBar.InternalCommandManager.Add(item);

			                try
			                {
			                    QuickAccessToolBar.Items.Add(item.SourceElement);
			                }
			                catch
			                {
			                }

			                QuickAccessToolBar.NeedItemWrap = true;
			                CloseAllPopups(target);
			            }
			        }
			    }
			    //Maintain the Removed QAT static Items
				this.QATInitialItemsString = new StringBuilder();
				foreach (var item in initialItems.ToList())
					this.QATInitialItemsString.Append(item + ",");

				ArrayList menuItems = stateParams.QATMenuItems;
				ObservableCollection<RibbonButton> qatMenuItems = new ObservableCollection<RibbonButton>();

				foreach (MenuItemState item in menuItems)
				{
					var menuItem = from RibbonButton menuButton in this.QuickAccessToolBar.QATMenuItems
								   where RibbonCommandManager.GetSynchronizedItem(menuButton) == item.SynchronizedItem && menuButton.Label == item.Label
								   select menuButton;
					RibbonButton qatMenuButton = menuItem.FirstOrDefault();
					if (qatMenuButton != null)
						qatMenuButton.IsSelected = item.IsSelected;
				}

				this.QuickAccessToolBar.UpdateQATItemsState();				

				ResetQATKeyTips();

                for (int i = 0; i < items.Count; i++)
                    ParseAndAddToQAT(items[i]);

                ApplyQATItemsIndex(stateParams.QATItemsIndex);

				this.IsQATBelow = stateParams.IsQATBelow;
				
				if (stateParams.IsQATBelow)
					this.QuickAccessToolBar.HasGeometry = false;
				else
					this.QuickAccessToolBar.HasGeometry = true;

				this.IsQATOnceLoaded = true;
			}

			if (loadRibbonState)
				this.RibbonState = stateParams.RibbonState;

			if (loadWindowState)
			{
                string IsMaximized = stateParams.WindowStates.Count >= 5 ? stateParams.WindowStates[4].ToString() : string.Empty;

				if (IsMaximized.Equals("true"))
					this.RootWindow.WindowState = WindowState.Maximized;
				else
				{
					if (this.RootWindow.WindowState == WindowState.Maximized)
						this.RootWindow.WindowState = WindowState.Normal;
                    this.RootWindow.Top = stateParams.WindowStates.Count > 0 ? (double)stateParams.WindowStates[0] : 0;
                    this.RootWindow.Left = stateParams.WindowStates.Count > 1 ? (double)stateParams.WindowStates[1] : 0;
                    this.RootWindow.Width = stateParams.WindowStates.Count > 2 ? (double)stateParams.WindowStates[2] : 0;
                    this.RootWindow.Height = stateParams.WindowStates.Count > 3 ? (double)stateParams.WindowStates[3] : 0;
				}
			}
		}

		/// <summary>
		/// Clears existing KeyTips for QAT items and generates new KeyTips.
		/// </summary>
		public void ResetQATKeyTips()
		{
			AutomaticKeys.m_keys.Clear();
			foreach (QuickAccessToolBarItem item in QuickAccessToolBar.InternalCommandManager.Items)
			{
				Ribbon.SetKeyTip(item.ClonedElement, AutomaticKeys.Next());
			}
		}

        #endregion

        /// <summary>
        /// Shows the back stage.
        /// </summary>
        public void ShowBackStage()
        {
            showBackStage = true;
            if (this.m_ribbonWindow != null && BackStageButton != null)
                BackStageButton.IsOpen = true;
         }
        internal ContextTabGroupCollection closedContextgroups = new ContextTabGroupCollection();
        /// <summary>
        /// Shows the back stage.
        /// </summary>
        internal void ShowBackStageInternal()
        {
            m_cancelAdornerState = true;
            if (this.m_ribbonWindow != null && this.m_ribbonWindow.BackStage != null && BackStageButton != null
                && m_ribbonWindow.BackStageContent!=null && m_ribbonWindow.BackStageContent.Visibility!=System.Windows.Visibility.Visible)
            {
                CancelEventArgs eventData = new CancelEventArgs();
                FireBackStageOpening(eventData);
                if (!eventData.Cancel)
                {
                    HideSelectedTab();
                    m_ribbonWindow.ShowBackStage();

                  
                    if (SkinStorage.GetVisualStyle(this) == "Office2013")
                    {
                        if (this.m_TabPanel != null)
                        {
                            this.m_TabPanel.Visibility = Visibility.Collapsed;
                            if (this.QuickAccessToolBar != null)
                                this.QuickAccessToolBar.Visibility = Visibility.Collapsed;
                            foreach (ContextTabGroup group in this.ContextTabGroups)
                            {
                                if (group.IsGroupVisible == true)
                                {
                                    closedContextgroups.Add(group);
                                    group.IsGroupVisible = false;
                                }

                            }
                        }
                    }


                    this.m_ribbonWindow.BackStage.GetCurrentSelectedTabItem();
                    ChangeEnableQATandToggleButton(false);
                    Window window = Window.GetWindow(this);
                    CollapseWindowsFormsHosts(window);
                    if (m_ribbonWindow != null && m_ribbonWindow.StatusBar != null)
                    {
                        m_ribbonWindow.StatusBar.Visibility = System.Windows.Visibility.Collapsed;
                        isStatusBarCollapsedByBackStage = true;
                    }
                    if (m_ribbonWindow.BackStage.IsKeytipOpen)
                    {
                        m_ribbonWindow.BackStage.ShowKeyTips();                        
                    }
                    FireBackStageOpened();
                }
                else
                {
                    BackStageButton.IsOpen = false;
                }
            }
        }

        internal void FireBackStageOpened()
        {
            if (BackStageOpened != null)
            {
                BackStageOpened(this, new EventArgs());
            }
        }
        protected void FireBackStageOpening(CancelEventArgs e)
        {
            if (BackStageOpening != null)
            {
                BackStageOpening(this, e);
            }
        }

        /// <summary>
        /// Collapses the windows forms hosts.
        /// </summary>
        /// <param name="parent">The parent.</param>
        private void CollapseWindowsFormsHosts(DependencyObject parent)
        {
            FrameworkElement frameworkElement = parent as FrameworkElement;
            if (frameworkElement != null)
            {
                if ((parent is WindowsFormsHost || parent is HwndHost))
                {
                    if ((((frameworkElement is WindowsFormsHost)) && (VisualUtils.FindDescendant(this.BackStage, typeof(WindowsFormsHost)) == null || !(VisualUtils.FindDescendant(this.BackStage, typeof(WindowsFormsHost)) as WindowsFormsHost).Equals(parent)) && frameworkElement.Visibility != Visibility.Collapsed) || (parent is HwndHost && (VisualUtils.FindDescendant(this.BackStage, typeof(WebBrowser)) == null || !(VisualUtils.FindDescendant(this.BackStage, typeof(WebBrowser)) as WebBrowser).Equals(parent)) && frameworkElement.Visibility != Visibility.Collapsed))
                    {
                        if (WFHvisible == null && frameworkElement is WindowsFormsHost)
                        {
                            if (frameworkElement.Visibility == Visibility.Visible)
                                WFHvisible = true;
                        }
                        if (browserVisible == null && frameworkElement is WebBrowser)
                        {
                            if (frameworkElement.Visibility == Visibility.Visible)
                                browserVisible = true;
                        }
                        collapsedElements.Add(frameworkElement, frameworkElement.Visibility);
                        frameworkElement.Visibility = Visibility.Collapsed;
                    }
                    if ((VisualUtils.FindDescendant(this.BackStage, typeof(WindowsFormsHost)) != null && (VisualUtils.FindDescendant(this.BackStage, typeof(WindowsFormsHost)) as WindowsFormsHost).Equals(parent)))
                    {
                       
                        if (WFHvisible != null && WFHvisible.Value)
                        {
                            if (frameworkElement.Visibility == Visibility.Visible)
                            frameworkElement.Visibility = Visibility.Collapsed;
                            if(!collapsedElements.ContainsKey(frameworkElement))
                            collapsedElements.Add(frameworkElement, frameworkElement.Visibility);
                            frameworkElement.Visibility = Visibility.Visible;
                        }
                    }
                    if ((VisualUtils.FindDescendant(this.BackStage, typeof(WebBrowser)) != null && (VisualUtils.FindDescendant(this.BackStage, typeof(WebBrowser)) as WebBrowser).Equals(parent)))
                    {
                        if (browserVisible != null && browserVisible.Value)
                        {
                            if (frameworkElement.Visibility == Visibility.Visible)
                                frameworkElement.Visibility = Visibility.Collapsed;
                            if (!collapsedElements.ContainsKey(frameworkElement))
                                collapsedElements.Add(frameworkElement, frameworkElement.Visibility);
                            frameworkElement.Visibility = Visibility.Visible;
                        }
                    }
                    return;
                }
            }

            //Check all elements.
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                CollapseWindowsFormsHosts(VisualTreeHelper.GetChild(parent, i));
            }
        }

        /// <summary>
        /// Changes the enable QA tand toggle button.
        /// </summary>
        /// <param name="state">if set to <c>true</c> [state].</param>
        private void ChangeEnableQATandToggleButton(bool state)
        {
            if (state)
            {
                if (this.QuickAccessToolBar != null)
                    this.QuickAccessToolBar.IsEnabled = true;
                if (this.ribbonToggleButton != null)
                    this.ribbonToggleButton.IsEnabled = true;
            }
            else
            {
                if (this.QuickAccessToolBar != null)
                    this.QuickAccessToolBar.IsEnabled = false;
                if (this.ribbonToggleButton != null)
                    this.ribbonToggleButton.IsEnabled = false;
            }
        }

        /// <summary>
        /// Hides the selected tab.
        /// </summary>
        private void HideSelectedTab()
        {
            if (this.SelectedItem != null)
            {
                RibbonTab selectedTab = this.ItemContainerGenerator.ContainerFromItem(this.SelectedItem) as RibbonTab;

                if (selectedTab != null)
                    selectedTab.m_tabButton.IsChecked = false;

                else
                {
                    selectedTab = this.Items[this.SelectedIndex] as RibbonTab;
                    if (selectedTab != null)
                    {
                        selectedTab.m_tabButton.IsChecked = false;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the current adoner.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        private static AdornerLayer GetCurrentAdoner(UIElement element)
        {
            UIElement current = element;
            while (true)
            {
                current = (UIElement)VisualTreeHelper.GetParent(current);
                if (current is AdornerDecorator) return AdornerLayer.GetAdornerLayer((UIElement)VisualTreeHelper.GetChild(current, 0));
            }
        }

        /// <summary>
        /// Hides the back stage.
        /// </summary>
        public void HideBackStage()
        {
            showBackStage = false;
            if (this.m_ribbonWindow != null && BackStageButton != null)
            {
                BackStageButton.IsOpen = false;
            }
        }

        /// <summary>
        /// Hides the back stage.
        /// </summary>
        internal void HideBackStageInternal()
        {
            if (this.m_ribbonWindow != null && this.m_ribbonWindow.BackStage != null && BackStageButton != null 
                && this.m_ribbonWindow.BackStageContent!=null && this.m_ribbonWindow.BackStageContent.Visibility==System.Windows.Visibility.Visible)
            {
                CancelEventArgs eventData = new CancelEventArgs();
                FireBackStageClosing(eventData);
                if (!eventData.Cancel)
                {
                   

                    m_ribbonWindow.HideBackStage();
                   
                    
                    if (SkinStorage.GetVisualStyle(this) == "Office2013")
                    {
                        if (this.m_TabPanel != null)
                        {
                            m_TabPanel.Visibility = Visibility.Visible;

                            if (this.QuickAccessToolBar != null)
                                this.QuickAccessToolBar.Visibility = Visibility.Visible;
                            RibbonTab selectedtab = this.SelectedTabItem;
                            foreach (ContextTabGroup group in this.closedContextgroups )
                            {
                                group.IsGroupVisible = true;
                            }
                            SelectedTabItem.SetValue(IsSelectedProperty, false);

                            SelectedTabItem = selectedtab;
                            SelectedTabItem.SetValue(IsSelectedProperty, true);
                            this.SelectedItem = selectedtab;
                        }
                    }
                    ShowSelectedTab();
                    ChangeEnableQATandToggleButton(true);

                    // Uncollapse elements
                    foreach (var element in collapsedElements)
                        element.Key.Visibility = element.Value;

                    collapsedElements.Clear();
                    if (this.m_ribbonWindow != null && this.m_ribbonWindow.StatusBar != null)
                    {
                        if (isStatusBarCollapsedByBackStage)
                        {
                            this.m_ribbonWindow.StatusBar.Visibility = System.Windows.Visibility.Visible;
                            this.isStatusBarCollapsedByBackStage = false;
                        }
                    }
                    FireBackStageClosed();
                    if (m_ribbonWindow.BackStage.IsKeytipOpen)
                    {
                        m_ribbonWindow.BackStage.IsKeytipOpen = false;
                    }
                }
                else
                {
                    BackStageButton.IsOpen = true;
                }
            }
        }

        internal virtual void FireBackStageClosed()
        {
            if (BackStageClosed != null)
            {
                BackStageClosed(this, new EventArgs());
            }
        }
        protected virtual void FireBackStageClosing(CancelEventArgs e)
        {
            if (BackStageClosing != null)
            {
                BackStageClosing(this, e);
            }
        }

        /// <summary>
        /// Shows the selected tab.
        /// </summary>
        private void ShowSelectedTab()
        {
            if (RibbonState == RibbonState.Normal && SelectedItem != null)
            {
                RibbonTab selectedTab = this.ItemContainerGenerator.ContainerFromItem(this.SelectedItem) as RibbonTab;

                if (selectedTab != null)
                {
                    selectedTab.m_tabButton.IsChecked = true;
                }
                else
                {
                    selectedTab = this.Items[this.SelectedIndex] as RibbonTab;
                    if (selectedTab != null)
                    {
                        selectedTab.m_tabButton.IsChecked = true;
                    }
                }
            }
        }

        ///// <summary>
        ///// Raises the <see cref="E:BackStageChanged"/> event.
        ///// </summary>
        ///// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        //private void GettingRootWindow()
        //{
        //    var item = VisualUtils.FindRootVisual(this) as RibbonWindow;
        //   RibbonWindow rwindow = item;
        //    if (rwindow != null)
        //    {
        //        rwindow.BackStage = this.BackStage;
        //    }
        //}

        /// <summary>
        /// Calls OnActiveColorSchemeChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnActiveColorSchemeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }

        /// <summary>
        /// Called when [back stage changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        //private static void OnBackStageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    Ribbon instance = (Ribbon)d;
        //    //instance.OnBackStageChanged(e);
        //}

        public void ShowCustomizationDialog() 
        {
            RibbonButton moreCommandsButton = this.QuickAccessToolBar.Template.FindName("PART_MoreCommands", this.QuickAccessToolBar) as RibbonButton;
            if (moreCommandsButton != null)
            {
                RibbonButtonAutomationPeer buttonAutomationPeer = new RibbonButtonAutomationPeer(moreCommandsButton);

                IInvokeProvider provider = buttonAutomationPeer.GetPattern(PatternInterface.Invoke) as IInvokeProvider;
                if (provider != null)
                    provider.Invoke();
            }
        }

        #endregion

        #region Command processing
        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Represents the method that will handle the CanExecute event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void CanProcessMinimizeCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Processes the minimize command.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The instance containing the event data.</param>
        private void ProcessMinimizeCommand(object sender, ExecutedRoutedEventArgs e)
        {
            MinimizeRibbon();
            isCheckedToggleButton = true;
            HideKeyTips();
            isCheckedToggleButton = false;
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Represents the method that will handle the CanExecute event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void CanProcessPlaceQATAboveCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            if (QuickAccessToolBar != null && !QuickAccessToolBar.HasGeometry)
            {
                e.CanExecute = true;
            }
            else
            {
                e.CanExecute = false;
            }
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Represents the method that will handle the Executed event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data</param>
        private void ProcessPlaceQATAboveCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if (IsQATBelow == false)
            {
                //IsQATBelow = true;
                IsQATBelow = false;

            }
            else
            {
                IsQATBelow = false;
                QuickAccessToolBar.HasGeometry = true;
            }
            
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Represents the method that will handle the CanExecute event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void CanProcessPlaceQATBelowCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            if (QuickAccessToolBar != null && QuickAccessToolBar.HasGeometry)
            {
                e.CanExecute = true;
            }
            else
            {
                e.CanExecute = false;
            }
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Represents the method that will handle the Executed event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void ProcessPlaceQATBelowCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if (IsQATBelow == true)
            {
                //IsQATBelow = false;
                IsQATBelow = true;

            }
            else
            {
                IsQATBelow = true;
            }
            if(IsQATBelow==true)
                QuickAccessToolBar.HasGeometry = false;
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Represents the method that will handle the CanExecute event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void CanProcessQATMoreCommandsCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            if (QuickAccessToolBar != null)
            {
                e.CanExecute = true;
            }
            else
            {
                e.CanExecute = false;
            }
        }

        /// <summary>
        /// Represents the method that will handle the Executed event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void ProcessQATMoreCommandsCommand(object sender, ExecutedRoutedEventArgs e)
        {
            ArrayList oldItems, newItems;
            CancelEventArgs eventData = new CancelEventArgs();
            FireQATCustomizeDialogOpening(eventData);

            if (!eventData.Cancel)
            {
                FrameworkElement parent = this.Parent as FrameworkElement;
                while (parent != null)
                {                   
                    if (parent.Parent == null || parent is Page)
                    {
                        break;
                    }

                    parent = parent.Parent as FrameworkElement;
                }

                SolidColorBrush skins = null;
                if(parent!=null)
                    skins = (SolidColorBrush)SkinManager.GetActiveColorScheme(parent);

                if (skins != null)
                {
                    m_blendColor = skins.Color;
                }                    

                QATCustomizationDialog dialog = new QATCustomizationDialog(IsQATBelow, SkinStorage.GetVisualStyle(this), m_blendColor, this);
                if (m_ribbonWindow != null)
                {
                    dialog.Owner = m_ribbonWindow;
                    dialog.FlowDirection = m_ribbonWindow.FlowDirection;
                }

                FrameworkElement lastCaptured = RibbonContextMenu.LastCapturedElement;
                if (lastCaptured != null)
                {
                    if (lastCaptured is RibbonItemsControl)
                    {
                        (lastCaptured as RibbonItemsControl).IsDropDownOpen = false;
                    }

                    CloseAllPopups(lastCaptured);
                }

                dialog.ShowDialog();

                if (dialog.DialogResult.Value)
                {
                    if (dialog.PART_chkShowQATBelow.IsChecked.Value)
                    {
                        RibbonCommands.PlaceQATBelow.Execute(null, this);
                    }
                    else
                    {
                        RibbonCommands.PlaceQATAbove.Execute(null, this);
                    }

                    oldItems = new ArrayList(QuickAccessToolBar.Items);
                    QuickAccessToolBar.LockQATItemsChange();
                    if(QuickAccessToolBar.ItemsSource == null)
                        QuickAccessToolBar.Items.Clear();
                    UpdateItems();
                    QuickAccessToolBar.UnlockQATItemsChange();
                    newItems = new ArrayList(QuickAccessToolBar.Items);

                    //if (!IsCollectionsEquals(oldItems, newItems))
                    //{
                    //    FireQATItemsCollectionChanged(new QATItemsCollectionChangedEventArgs(oldItems, newItems, CollectionType.CustomizationDialog));
                    //}
                }

                FireQATCustomizeDialogClosed();
            }
        }

        /// <summary>
        /// Represents the method that will handle the CanExecute event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void CanProcessAddToQATCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            if (QuickAccessToolBar != null)
            {
                e.CanExecute = true;
            }
            else
            {
                e.CanExecute = false;
            }
        }

        /// <summary>
        /// Represents the method that will handle the Executed event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void ProcessAddToQATCommand(object sender, ExecutedRoutedEventArgs e)
        {
            FrameworkElement element = null;
            if ((e.Source is RibbonBar && (e.Source as RibbonBar).PanelState == RibbonBarState.Collapsed) || e.Source is SplitButton)
                element = e.Source as FrameworkElement;
            else
                element = e.OriginalSource as FrameworkElement;

            if (element != null)
            {
                if (element.Parent is DropDownButton)
                {
                    if (!(element.Parent as DropDownButton).IsGroup)
                    {
                        AddItemToQAT(element as UIElement);
                        return;
                    }
                    else
                    {
                        FrameworkElement parent = element.Parent as FrameworkElement;
                        AddItemToQAT(parent as UIElement);
                        return;
                    }
                }

                if (ApplicationMenu != null && ApplicationMenu.ApplicationItems != null && ApplicationMenu.ApplicationItems.Contains(element))
                {
                    qatApplicationItemIndex.Add(ApplicationMenu.ApplicationItems.IndexOf(element) + "," + "AppItems");
                }

                if (ApplicationMenu != null && ApplicationMenu.Items.Contains(element))
                {
                    qatApplicationItemIndex.Add(ApplicationMenu.Items.IndexOf(element));
                }

                if (ApplicationMenu != null && ApplicationMenu.MenuItems != null && element != null && element.TemplatedParent != null && ApplicationMenu.MenuItems.Contains(element.TemplatedParent))
                {
                    qatApplicationItemIndex.Add(ApplicationMenu.MenuItems.IndexOf(element.TemplatedParent) + "," + "MenuItems");
                    AddItemToQAT(element.TemplatedParent as UIElement);
                    return;
                }

                if (element.Parent is ApplicationMenuGroup)
                {
                    object menuGroupItem = ApplicationMenu.Items.IndexOf((element.Parent as ApplicationMenuGroup).Parent) + ",";
                    menuGroupItem += (element.Parent as ApplicationMenuGroup).Items.IndexOf(element) + "," + "MenuGroup";
                    qatApplicationItemIndex.Add(menuGroupItem);
                }

                AddItemToQAT(element as UIElement);
            }
        }

        /// <summary>
        /// Represents the method that will handle the CanExecute event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void CanProcessRemoveFromQATCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        /// <summary>
        /// Processes removing from QAT command.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The instance containing the event data.</param>
        private void ProcessRemoveFromQATCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if (/*e.Parameter*/ e.OriginalSource != null)
            {
                if (QuickAccessToolBar.Items.Contains(e.OriginalSource/*e.Parameter*/))
                {
                    foreach (var obj in QuickAccessToolBar.QATMenuItems)
                    {
                        string itemName = RibbonCommandManager.GetSynchronizedItem(obj);
                        Dictionary<string, FrameworkElement> syncItemColl = RibbonCommandManager.SynchronizedItemCollection;
                        if (itemName != null && syncItemColl.ContainsKey(itemName))
                        {
                            FrameworkElement qatitem = syncItemColl[itemName];

                            if (qatitem is ICommandSource)
                                obj.Tag = (qatitem as ICommandSource).Command;
                        }
                    }
                    QuickAccessToolBar.NeedItemWrap = false;
                    QuickAccessToolBar.RemoveQATItemFireQATRemoved(e.OriginalSource);
                    QuickAccessToolBar.NeedItemWrap = true;

                    UIElement element = e.OriginalSource as UIElement;

                    if (QATItems.ContainsKey(element))
                        this.QATItems.Remove(element);

                    else if (this.QATInitialItems.Contains(element))
                        this.QATInitialItemsString.Append(this.QATInitialItems.IndexOf(element).ToString() + ",");
                }
            }

            CloseAllPopups(e.OriginalSource as UIElement);
        }
        #endregion

        #region Event Raisers

        /// <summary>
        /// Handles the Loaded event of the Ribbon control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void Ribbon_Loaded(object sender, RoutedEventArgs e)
        {
            ArrangeContextTabGroups();

            Items.MoveCurrentToFirst();
            m_ribbonWindow = VisualUtils.FindRootVisual(this) as RibbonWindow;
            if (m_ribbonWindow != null)
            {
                m_ribbonWindow.BackStageColor = this.BackStageColor;
                m_ribbonWindow.BackStageCornerImageVisibility = this.BackStageCornerImageVisibility;
                BindingUtils.SetBinding(m_ribbonWindow, this, RibbonWindow.BackStageProperty, Ribbon.BackStageProperty, BindingMode.TwoWay);
            }
            if (m_ribbonWindow != null && m_ribbonWindow.Width <= 300)
            {
                this.Visibility = Visibility.Collapsed;

                if (this.ApplicationMenu != null)
                {
                    this.ApplicationMenu.Visibility = Visibility.Collapsed;
                    m_ribbonWindow.TitleBar.AppMenuColumnWidth = 0d;
                }

                if (this.QuickAccessToolBar != null)
                {
                    this.QuickAccessToolBar.Visibility = Visibility.Collapsed;
                    m_ribbonWindow.TitleBar.QATColumnWidth = 0d;
                }
            }
            else if (m_ribbonWindow != null)
            {
                this.Visibility = Visibility.Visible;

                if (this.ApplicationMenu != null)
                {
                    this.ApplicationMenu.Visibility = Visibility.Visible;
                    BindingUtils.SetBinding(m_ribbonWindow.TitleBar, this.ApplicationMenu, TitleBar.AppMenuColumnWidthProperty, QuickAccessToolBar.ActualWidthProperty);
                }

                if (this.QuickAccessToolBar != null)
                {
                    this.QuickAccessToolBar.Visibility = Visibility.Visible;
                    if (!this.IsQATBelow)
                    {
                        BindingUtils.SetBinding(m_ribbonWindow.TitleBar, this.QuickAccessToolBar, TitleBar.QATColumnWidthProperty, QuickAccessToolBar.ActualWidthProperty);
                    }
                }
            }

            isItemLoaded = true;
            bool flag = false;
            for (int i = 0; i < this.Items.Count; i++)
            {
                RibbonTab ribbonTab = this.ItemContainerGenerator.ContainerFromIndex(i) as RibbonTab;
                if (ribbonTab != null)
                {
                    if (ribbonTab.IsChecked)
                    {
                        flag = true;
                        SelectedIndex = i;
                        break;
                    }
                    else
                    {
                        ribbonTab.IsChecked = false;
                    }
                }
            }
            if (!flag)
            {
                RibbonTab ribbonTab = this.ItemContainerGenerator.ContainerFromIndex(0) as RibbonTab;
                if (ribbonTab != null && this.RibbonState != RibbonState.Hide)
                {
                    ribbonTab.IsChecked = true;
                    SelectedIndex = 0;
                }
                else
                    SelectedIndex = -1;
            }
            isloaded = true;

            RibbonLayoutPanel panel = VisualUtils.FindDescendant(this, typeof(RibbonLayoutPanel)) as RibbonLayoutPanel;
            if (panel != null)
            {
                if (Ribbon.GetIsAutoSizeFormEnabled(this))
                {
                    panel.CheckForCollapse();
                }
            }
            if (BackStage != null)
            {
                //BackStage.FlowDirection = this.FlowDirection;
                if (BackStage.DataContext == null && this.DataContext != null)
                {
                    BindingUtils.SetBinding(BackStage, this, Backstage.DataContextProperty, Ribbon.DataContextProperty);                    
                }
                else if (m_ribbonWindow != null && this.BackStage!=null && m_ribbonWindow.DataContext == this.BackStage.DataContext)
                {
                    this.BackStage.DataContext = this.DataContext;
                }
            }
            if(showBackStage)
                ShowBackStage();

            RibbonHeight = this.Height;
        }


        /// <summary>
        /// Handles the SizeChanged event of the Ribbon control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.SizeChangedEventArgs"/>instance containing the event data.</param>
        private void Ribbon_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (m_isAdornersShown)
            {
                HideKeyTips();
            }
        }

        /// <summary>
        /// Fires the QAT customize dialog opening.
        /// </summary>
        /// <param name="e">The instance containing the event data.</param>
        protected virtual void FireQATCustomizeDialogOpening(CancelEventArgs e)
        {
            if (QATCustomizeDialogOpening != null)
            {
                QATCustomizeDialogOpening(this, e);
            }
        }

        /// <summary>
        /// Fires the QAT customize dialog closed.
        /// </summary>
        protected virtual void FireQATCustomizeDialogClosed()
        {
            if (QATCustomizeDialogClosed != null)
            {
                QATCustomizeDialogClosed(this, new EventArgs());               
            }
        }

        /// <summary>
        /// Fires the before qat drop down popup.
        /// </summary>
        /// <param name="e">The instance containing the event data.</param>
        protected virtual void FireBeforeQatDropDownPopup(CancelEventArgs e)
        {
            if (BeforeQatDropDownPopup != null)
            {
                BeforeQatDropDownPopup(this, e);
            }
        }

        /// <summary>
        /// Fires the after qat drop down popup.
        /// </summary>
        protected virtual void FireAfterQatDropDownPopup()
        {
            if (AfterQatDropDownPopup != null)
            {
                AfterQatDropDownPopup(this, new EventArgs());
            }
        }

        /// <summary>
        /// Handles the BeforeDropDownPopup event of the PopupButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.CancelEventArgs"/> instance containing the event data.</param>
        private void PopupButton_BeforeDropDownPopup(object sender, CancelEventArgs e)
        {
            FireBeforeQatDropDownPopup(e);
        }

        /// <summary>
        /// Handles the AfterDropDownPopup event of the PopupButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void PopupButton_AfterDropDownPopup(object sender, EventArgs e)
        {
            FireAfterQatDropDownPopup();
        }
        #endregion

        #region Dp getters & setters

        public Visibility BackStageCornerImageVisibility
        {
            get { return (Visibility)GetValue(BackStageCornerImageVisibilityProperty); }
            set { SetValue(BackStageCornerImageVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BackStageCornerImageVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BackStageCornerImageVisibilityProperty =
            DependencyProperty.Register("BackStageCornerImageVisibility", typeof(Visibility), typeof(Ribbon), new UIPropertyMetadata(Visibility.Visible,new PropertyChangedCallback(OnBackstageCornerImageVisibilityChanged)));

        private static void OnBackstageCornerImageVisibilityChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            Ribbon ribbon = sender as Ribbon;
            if (ribbon != null)
            {
                RibbonWindow window = VisualUtils.FindAncestor(ribbon, typeof(RibbonWindow)) as RibbonWindow;
                if (window != null)
                {
                    window.BackStageCornerImageVisibility = ribbon.BackStageCornerImageVisibility;
                }
            }
        }

        public Brush BackStageColor
        {
            get { return (Brush)GetValue(BackStageColorProperty); }
            set { SetValue(BackStageColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BackStageColor.  This enables animation, styling, binding, etc...      

        public static readonly DependencyProperty BackStageColorProperty =
            DependencyProperty.Register("BackStageColor", typeof(Brush), typeof(Ribbon), new UIPropertyMetadata(Brushes.Blue, new PropertyChangedCallback(OnBackStageColorChanged)));


        private static void OnBackStageColorChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            Ribbon ribbon = sender as Ribbon;
            if (ribbon != null)
            {
                RibbonWindow window = VisualUtils.FindAncestor(ribbon, typeof(RibbonWindow)) as RibbonWindow;
                if (window == null)
                    window = VisualUtils.FindLogicalAncestor(ribbon, typeof(RibbonWindow)) as RibbonWindow;
                if (window != null)
                {
                    window.BackStageColor = ribbon.BackStageColor;
                }
                if (SkinStorage.GetVisualStyle(ribbon) == "Office2013")
                {
                    if (window.TryFindResource("Office2013RibbonItemHoverBrush") == null && window.TryFindResource("Office2013RibbonItemPressedBrush") == null && window.TryFindResource("Office2013RibbonItemSelectedBrush") == null)
                    {


                        window.Resources.Add("Office2013RibbonItemHoverBrush", GetHoverColor(ribbon.BackStageColor));
                        window.Resources.Add("Office2013RibbonItemPressedBrush", GetPressedColor(ribbon.BackStageColor));
                        window.Resources.Add("Office2013RibbonItemSelectedBrush", GetSelectedColor(ribbon.BackStageColor));
                        window.Resources.Add("BackStageItemSelectedBrush", GetBackStageSelectedColor(ribbon.BackStageColor));

                    }
                    else
                    {
                        window.Resources["Office2013RibbonItemHoverBrush"] = GetHoverColor(ribbon.BackStageColor);
                        window.Resources["Office2013RibbonItemPressedBrush"] = GetPressedColor(ribbon.BackStageColor);
                        window.Resources["Office2013RibbonItemSelectedBrush"] = GetSelectedColor(ribbon.BackStageColor);
                        window.Resources["BackStageItemSelectedBrush"] = GetBackStageSelectedColor(ribbon.BackStageColor);
                    }
                }
               
            }
        }

        private static Brush GetHoverColor(Brush brush)
        {
          

            
            Color c = (brush as SolidColorBrush).Color;
            double Offset = 0.75;
            byte r = (byte)(c.R + ((255 - c.R) * Offset));
            byte g = (byte)(c.G + ((255 - c.G) * Offset));
            byte b = (byte)(c.B + ((255 - c.B) * Offset));
            return new SolidColorBrush(Color.FromRgb(r, g, b));

            
            
        }
        private static Brush GetBackStageSelectedColor(Brush brush)
        {
           

            Color c = (brush as SolidColorBrush).Color;
           
            byte r = (byte)(c.R +20);
            byte g = (byte)(c.G + 20);
            byte b = (byte)(c.B + 20);
            return new SolidColorBrush(Color.FromRgb(r, g, b));



        }
        private static Brush GetPressedColor(Brush brush)
        {
           

        

            Color c = (brush as SolidColorBrush).Color;
            double Offset = 0.60;
            byte r = (byte)(c.R + ((255 - c.R) * Offset));
            byte g = (byte)(c.G + ((255 - c.G) * Offset));
            byte b = (byte)(c.B + ((255 - c.B) * Offset));
            return new SolidColorBrush(Color.FromRgb(r, g, b));

        }
        private static Brush GetSelectedColor(Brush brush)
        {
           

            Color c = (brush as SolidColorBrush).Color;
            double Offset = 0.50;
            byte r = (byte)(c.R + ((255 - c.R) * Offset));
            byte g = (byte)(c.G + ((255 - c.G) * Offset));
            byte b = (byte)(c.B + ((255 - c.B) * Offset));
            return new SolidColorBrush(Color.FromRgb(r, g, b));



        }
        [TypeConverter(typeof(StringConverter))]
        public object BackStageHeader
        {
            get { return (object)GetValue(BackStageHeaderProperty); }
            set { SetValue(BackStageHeaderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BackStageHeader.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BackStageHeaderProperty =
            DependencyProperty.Register("BackStageHeader", typeof(object), typeof(Ribbon), new UIPropertyMetadata(wrapper.BackStageButtonHeader));

        [TypeConverter(typeof(StringConverter))]
        public object ApplicationMenuHeader
        {
            get { return (object)GetValue(ApplicationMenuHeaderProperty); }
            set { SetValue(ApplicationMenuHeaderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BackStageHeader.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ApplicationMenuHeaderProperty =
            DependencyProperty.Register("ApplicationMenuHeader", typeof(object), typeof(Ribbon), new UIPropertyMetadata(wrapper.ApplicationMenuButtonHeader));



        public bool ShowDefaultQATKeyTip
        {
            get { return (bool)GetValue(ShowDefaultQATKeyTipProperty); }
            set { SetValue(ShowDefaultQATKeyTipProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowDefaultQATKeyTip.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowDefaultQATKeyTipProperty =
            DependencyProperty.Register("ShowDefaultQATKeyTip", typeof(bool), typeof(Ribbon), new UIPropertyMetadata(true));

        

        /// <summary>
        /// Gets the key tip.
        /// </summary>
        /// <param name="obj">The obj value.</param>
        /// <returns>string value</returns>
        public static string GetKeyTip(DependencyObject obj)
        {
            if (obj != null)
                return (string)obj.GetValue(KeyTipProperty);

            else return String.Empty;
        }

        /// <summary>
        /// Sets the value of the KeyTip property for a given element.
        /// </summary>
        /// <param name="obj">The element on which to apply the property value.</param>
        /// <param name="value">Key tip value.</param>
        public static void SetKeyTip(DependencyObject obj, string value)
        {
            obj.SetValue(KeyTipProperty, value);
        }

        /// <summary>
        /// Gets the split menu key tip.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public static string GetSplitMenuKeyTip(DependencyObject obj)
        {
            return (string)obj.GetValue(SplitMenuKeyTipProperty);
        }

        /// <summary>
        /// Sets the split menu key tip.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="value">The value.</param>
        public static void SetSplitMenuKeyTip(DependencyObject obj, string value)
        {
            obj.SetValue(SplitMenuKeyTipProperty, value);
        }

        /// <summary>
        /// Gets the value of the IsQATItem property for a given element.
        /// </summary>
        /// <param name="obj">The element for which to retrieve the IsQATItem value.</param>
        /// <returns>return Is QATItem Property</returns>
        public static bool GetIsQATItem(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsQATItemProperty);
        }

        /// <summary>
        /// Sets the value of the IsQATItem property for a given element.
        /// </summary>
        /// <param name="obj">The element on which to apply the property value.</param>
        /// <param name="value">IsQATItem Boolean value.</param>
        public static void SetIsQATItem(DependencyObject obj, bool value)
        {
            obj.SetValue(IsQATItemProperty, value);
        }

        /// <summary>
        /// Gets the value of the ActiveColorScheme property for a given element.
        /// </summary>
        /// <param name="obj">The element for which to retrieve the ZIndex value.</param>
        /// <returns>returns Active Color Scheme Property</returns>
        [TypeConverter(typeof(BrushConverter))]
        public static object GetActiveColorScheme(DependencyObject obj)
        {
            return (object)obj.GetValue(ActiveColorSchemeProperty);
        }

        /// <summary>
        /// Sets the value of the ActiveColorScheme property for a given element.
        /// </summary>
        /// <param name="obj">The element on which to apply the property value.</param>
        /// <param name="value">Active scheme brush.</param>
        public static void SetActiveColorScheme(DependencyObject obj, object value)
        {
            obj.SetValue(ActiveColorSchemeProperty, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Ribbon controls are resized dynamically or not. This is a dependency property.
        /// </summary>
        //public bool IsAutoSizeFormEnabled
        //{
        //    get
        //    {
        //        return (bool)GetValue(IsAutoSizeFormEnabledProperty);
        //    }
        //    set
        //    {
        //        SetValue(IsAutoSizeFormEnabledProperty, value);
        //    }
        //}

       
        public double RibbonHeight
        {
            get
            {
                return ribbonHeight;
            }
            internal set
            {
                ribbonHeight=value;
            }

        }
        #endregion


        public void Dispose()
        {
            this.QATInitialItems = null;
            this._tempTabCollection = null;
            this.QATItems.Clear();
            if (RootWindow != null)
            {
                RootWindow.Closing -= new CancelEventHandler(RootWindow_Closing);
                RootWindow.Loaded -= new RoutedEventHandler(RootWindow_Loaded);
                RootWindow.SizeChanged -= new SizeChangedEventHandler(RootWindow_SizeChanged);
            }
            if (m_ribbonWindow != null)
            {
                m_ribbonWindow.IsMinimalSizeReachedChanged -= new PropertyChangedCallback(RibbonWindow_IsMinimalSizeRichedChanged);

               
                m_ribbonWindow.PreviewMouseDown -= new MouseButtonEventHandler(m_ribbonWindow_MouseDown);
                 #if !SyncfusionFramework3_5
                m_ribbonWindow.PreviewTouchDown -= new EventHandler<TouchEventArgs>(m_ribbonWindow_PreviewTouchDown);
#endif
                m_ribbonWindow.LostKeyboardFocus -= new KeyboardFocusChangedEventHandler(m_ribbonWindow_LostKeyboardFocus);

                if (m_ribbonWindow.TitleBar != null)
                    m_ribbonWindow.TitleBar.PreviewMouseLeftButtonDown -= new MouseButtonEventHandler(TitleBar_PreviewMouseLeftButtonDown);

                m_ribbonWindow.KeyDown -= new KeyEventHandler(m_ribbonWindow_KeyDown);
                m_ribbonWindow.SizeChanged -= new SizeChangedEventHandler(m_ribbonWindow_SizeChanged);
                m_ribbonWindow.KeyUp -= new KeyEventHandler(m_ribbonWindow_KeyUp);
                m_ribbonWindow.LocationChanged -= new EventHandler(m_ribbonWindow_LocationChanged);
            }
            if (this.m_ribbonWindow != null && m_ribbonWindow.TitleBar != null)
            {
                m_ribbonWindow.TitleBar.maxButton = null;
                m_ribbonWindow.TitleBar.closeButton = null;
                m_ribbonWindow.TitleBar.normalButton = null;
                m_ribbonWindow.TitleBar.minButton = null;
            }
            m_ribbonWindow = null;
            RootWindow = null;
           
            if (ribbon_contextmenu != null)
            {
                IList itemcollection = ribbon_contextmenu.ItemsSource as IList;
                itemcollection.Clear();
            }
            ribbon_contextmenu = null;
            if (this.ItemsSource == null)
                foreach (RibbonTab tab in Items)
                {
                    //foreach (var item in tab.Items)
                    //{
                    //    RibbonBar bar = item as RibbonBar;
                    //    if (bar != null)
                    //    {
                    //        bar.ItemsSource = null;
                    //        bar.Items.Clear();
                    //    }
                    //}

                    //tab.ItemsSource = null;
                    //tab.Items.Clear();

                    tab.m_tabButton.m_ribbonParent = null;
                    tab.m_tabButton = null;
                }

            this.ItemsSource = null;
            RibbonCommandManager.QATMenuItem = null;
            RibbonCommandManager.qATMenuItem = null;

            ContextTabGroups.CollectionChanged -= new NotifyCollectionChangedEventHandler(ContextTabGroups_CollectionChanged);
            Dictionary<string, FrameworkElement> syncItemColl = RibbonCommandManager.SynchronizedItemCollection;
            if (syncItemColl != null)
            {
                syncItemColl.Clear();
            }
            this.SelectedIndexChanged -= SelectedIndexChanged;        
            if (this.QuickAccessToolBar != null)
            {
                QuickAccessToolBar.ItemsSource = null;
                QuickAccessToolBar.Items.Clear();
                QuickAccessToolBar.m_popup_OverflowButton = null;
                QuickAccessToolBar.m_qatMenuItemsOverFlowControl = null;
                QuickAccessToolBar.PopupButton = null;
                QuickAccessToolBar.m_popupButtonOverflowed = null;
                QuickAccessToolBar.Ribbon = null;
                QuickAccessToolBar.m_popupButton = null;
                QuickAccessToolBar.m_defaultItems = null;
                QuickAccessToolBar.m_internalCommandManager = null;
                QuickAccessToolBar.m_mainItemsControl = null;
                QuickAccessToolBar.m_qatMenuItemsControl.ItemsSource = null;
                QuickAccessToolBar.m_qatMenuItemsControl.Items.Clear();
                QuickAccessToolBar.m_qatMenuItemsControl = null;
                QuickAccessToolBar.m_popup_OverflowButton = null;
                QuickAccessToolBar = null;
            }
            try
            {
                this.Items.Clear();
            }
            //SU I78477
            //catch (Exception ex)
            catch (Exception)
            //EU I78477
            {

            }
        }
    }

    /// <summary>
    /// Represent the Collection of Ribbon elements to persit its state.
    /// </summary>
    public enum RibbonElements
    {
        /// <summary>
        /// Persist Ribbon.
        /// </summary>
        Ribbon,
        /// <summary>
        /// Persist Quick Access Toolbar.
        /// </summary>
        QuickAccessToolbar,
        /// <summary>
        /// Persist Ribbon window.
        /// </summary>
        RibbonWindow
    }
}