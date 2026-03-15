#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Syncfusion.Windows.Shared;
using System.ComponentModel;
using Syncfusion.Windows.Tools.Controls.Resources;
using System.IO.IsolatedStorage;
using System.Windows.Markup;
using System.IO;
using System.Xml.Serialization;
using System.Text;
using System.Xml;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Windows.Controls.Primitives;
using System.Collections;

namespace Syncfusion.Windows.Tools.Controls
{
    #region Ribbon

    /// <summary>
    /// Represents the Ribbon Class.
    /// </summary>
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Blend,
        Type = typeof(Ribbon), XamlResource = "/Syncfusion.Theming.Blend;component/Ribbon.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Blue,
        Type = typeof(Ribbon), XamlResource = "/Syncfusion.Theming.Office2007Blue;component/Ribbon.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Black,
        Type = typeof(Ribbon), XamlResource = "/Syncfusion.Theming.Office2007Black;component/Ribbon.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Silver,
        Type = typeof(Ribbon), XamlResource = "/Syncfusion.Theming.Office2007Silver;component/Ribbon.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Default,
        Type = typeof(Ribbon), XamlResource = "/Syncfusion.Ribbon.Silverlight;component/themes/generic.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Windows7,
      Type = typeof(Ribbon), XamlResource = "/Syncfusion.Ribbon.Silverlight;component/themes/generic.xaml")]

    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2003,
        Type = typeof(Ribbon), XamlResource = "/Syncfusion.Theming.Office2003;component/Ribbon.xaml")]

    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Blue,
       Type = typeof(Ribbon), XamlResource = "/Syncfusion.Theming.Office2010Blue;component/Ribbon.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Black,
       Type = typeof(Ribbon), XamlResource = "/Syncfusion.Theming.Office2010Black;component/Ribbon.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Silver,
       Type = typeof(Ribbon), XamlResource = "/Syncfusion.Theming.Office2010Silver;component/Ribbon.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.VS2010,
       Type = typeof(Ribbon), XamlResource = "/Syncfusion.Theming.VS2010;component/Ribbon.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Metro,
      Type = typeof(Ribbon), XamlResource = "/Syncfusion.Theming.Metro;component/Ribbon.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Transparent,
     Type = typeof(Ribbon), XamlResource = "/Syncfusion.Theming.Transparent;component/Ribbon.xaml")]

    [StyleTypedProperty(Property = "QuickAccessToolbarStyle", StyleTargetType = typeof(QuickAccessToolBar))]
    [StyleTypedProperty(Property = "ApplicationMenuStyle", StyleTargetType = typeof(ApplicationMenu))]
    public class Ribbon :
        ItemsControl,
        IRibbonTabItemSelector,IDisposable
    {
        #region Enums

        /// <summary>
        /// Specifies available states of Ribbon toolbar
        /// </summary>
        public enum RibbonToolbarState
        {
            /// <summary>
            /// Toolbar is hidden
            /// </summary>
            Hidden,

            /// <summary>
            /// Toolbar is placed above the ribbon tabs
            /// </summary>
            AboveRibbon,

            /// <summary>
            /// Toolbar is placed below ribbon tabs
            /// </summary>
            BelowRibbon
        }

        #endregion

        #region Contsructors
        /// <summary>
        /// Initialize a new instance of <see cref="Ribbon"/>
        /// </summary>
        public Ribbon()
        {
            this.DefaultStyleKey = typeof(Ribbon);
            this.tabs = new Dictionary<object, RibbonTab>();
            this.SizeChanged += new SizeChangedEventHandler(this.SizeChangedHandler);
            SynchronizedCommands = new SynchronizedItemsCollection();
            ResourceWrapperKeys = new ResourceWrapper();
            Application.Current.Exit += new EventHandler(Current_Exit);
            this.Unloaded += new RoutedEventHandler(Ribbon_Unloaded);
        }

        void Ribbon_Unloaded(object sender, RoutedEventArgs e)
        {

            
            
        }

        /// <summary>
        /// Handles the Exit event of the Current control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void Current_Exit(object sender, EventArgs e)
        {
            if (this.AutoPersist || this.QuickAccessToolBar != null && this.QuickAccessToolBar.AutoPersist || this.IsInRibbonWindow && this.Window != null && this.Window.AutoPersist)
                SaveDefaultState(default_StoreFile);
        }


        /// <summary>
        /// Initializes the <see cref="Ribbon"/> class.
        /// </summary>
        static Ribbon()
        {
            if (DesignerProperties.IsInDesignTool)
            {
                LoadDependentAssemblies load = new LoadDependentAssemblies();
                load = null;
            }
        }

        #endregion

        #region Properties

        private Popup BackStagePopUp;

        /// <summary>
        /// Gets or sets the index of the selected.
        /// </summary>
        /// <value>The index of the selected.</value>
        public int SelectedIndex
        {
            get { return this.selectedIndex; }
            set { this.selectedIndex = value; }
        }

        /// <summary>
        /// Used to store the Default Store File Name.
        /// </summary>
        private string default_StoreFile = "RibbonState.xml";

        /// <summary>
        /// Represent the BackStage button in Office 2010 UI.
        /// </summary>
        public  BackStageButton BackStageButton;

        /// <summary>
        /// Represent Ribbon Toggle button in Office 2010 UI.
        /// </summary>
        internal ToggleButton ribbonToggleButton;
        
        /// <summary>
        /// Used to store the Reset Store File Name.
        /// </summary>
        private string reset_StoreFile = "ResetState.xml";

        /// <summary>
        /// Instance to store the Index of Removing Static Items in QAT.
        /// </summary>
        private StringBuilder RemovedStaticQATItems = new StringBuilder();

        /// <summary>
        /// Temp Collection to store Ribbon Items.
        /// </summary>
        private List<object> tempTabCollection = new List<object>();

        /// <summary>
        /// Used to save the currently selected Tab while Modal Tab displayed.
        /// </summary>
        private object tempSelectedItem;

        /// <summary>
        /// Private Variable for Resource Wrappers.
        /// </summary>
        private ResourceWrapper ResourceWrapperKeys;

        /// <summary>
        /// Gets or sets the synchronized commands.
        /// </summary>
        /// <value>The synchronized commands.</value>
        public SynchronizedItemsCollection SynchronizedCommands
        {
            get { return (SynchronizedItemsCollection)GetValue(SynchronizedCommandsProperty); }
            set { SetValue(SynchronizedCommandsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SynchronizedCommands.  This enables animation, styling, binding, etc...
        /// <summary>
        /// The Collection of commands which used to synchronize Quick Access Toolbar Items. It is a Dependency Proprty.
        /// </summary>
        public static readonly DependencyProperty SynchronizedCommandsProperty =
            DependencyProperty.Register("SynchronizedCommands", typeof(SynchronizedItemsCollection), typeof(Ribbon), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the tab panel item.
        /// </summary>
        /// <value>The tab panel item.</value>
        [Description("Used to add Help or Extra button at the extreme right of Tab Header")]
        [Category("Ribbon Properties")]
        public Control TabPanelItem
        {
            get { return (Control)GetValue(TabPanelItemProperty); }
            set { SetValue(TabPanelItemProperty, value); }
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
        /// Using a DependencyProperty as the backing store for ModalTabCollection.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ModalTabCollectionProperty =
            DependencyProperty.Register("ModalTabCollection", typeof(ModalTabCollection), typeof(Ribbon), new PropertyMetadata(new ModalTabCollection()));



        
        /// <summary>
        /// Using a DependencyProperty as the backing store for TabPanelItem.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TabPanelItemProperty =
            DependencyProperty.Register("TabPanelItem", typeof(Control), typeof(Ribbon), new PropertyMetadata(null, new PropertyChangedCallback(OnTabPanelItemChanged)));



        /// <summary>
        /// Gets or sets the Application Icon.
        /// </summary>
        /// <value>The sys icon.</value>
        public ImageSource  Icon
        {
            get { return (ImageSource )GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for Icon.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(ImageSource ), typeof(Ribbon), new PropertyMetadata(null));



        /// <summary>
        /// Gets or sets the back stage.
        /// </summary>
        /// <value>The back stage.</value>
        public Backstage  BackStage
        {
            get { return (Backstage)GetValue(BackStageProperty); }
            set { SetValue(BackStageProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for BackStage.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty BackStageProperty =
            DependencyProperty.Register("BackStage", typeof(Backstage), typeof(Ribbon), new PropertyMetadata(null));



        /// <summary>
        /// Called when [tab panel item changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnTabPanelItemChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var senderObj = sender as Ribbon;
            if (senderObj != null && senderObj.TabPanelItemChanged != null)
            {
                senderObj.TabPanelItemChanged(sender, e);
            }
        }

        /// <summary>
        /// Event that is raised when TabPanelItem property is changed.
        /// </summary>
        public event PropertyChangedCallback TabPanelItemChanged;

        /// <summary>
        /// Gets or sets the application menu.
        /// </summary>
        /// <value>The application menu.</value>
        [Description("Shows at the the top left corner which contains standard commands")]
        [Category("Ribbon Properties")]
        public ApplicationMenu ApplicationMenu
        {
            get { return (ApplicationMenu)GetValue(ApplicationMenuProperty); }
            set { SetValue(ApplicationMenuProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ApplicationMenu.  This enables animation, styling, binding, etc...
        /// <summary>
        /// Gets or Set the Application Menu for Ribbon.
        /// </summary>
        public static readonly DependencyProperty ApplicationMenuProperty =
            DependencyProperty.Register("ApplicationMenu", typeof(ApplicationMenu), typeof(Ribbon), new PropertyMetadata(null, OnApplicationMenuChanged));

        /// <summary>
        /// Called when [application menu changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnApplicationMenuChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var senderObj = sender as Ribbon;
            if (senderObj.ApplicationMenu != null)
            {
                senderObj.ApplicationMenu.Style = senderObj.ApplicationMenuStyle;
            }
        }

        /// <summary>
        /// Gets or sets the application menu style.
        /// </summary>
        /// <value>The application menu style.</value>
        [Description("Used to change ApplicationMenu Style")]
        [Category("Ribbon Properties")]
        public Style ApplicationMenuStyle
        {
            get { return (Style)GetValue(ApplicationMenuStyleProperty); }
            set { SetValue(ApplicationMenuStyleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ApplicationMenuStyle.  This enables animation, styling, binding, etc...
        /// <summary>
        /// Used to Gets or Sets the custom style for Application Menu.
        /// </summary>
        public static readonly DependencyProperty ApplicationMenuStyleProperty =
            DependencyProperty.Register("ApplicationMenuStyle", typeof(Style), typeof(Ribbon), new PropertyMetadata(null, OnApplicationMenuStyleChanged));

        /// <summary>
        /// Called when [application menu style changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnApplicationMenuStyleChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var senderObj = sender as Ribbon;
            if (senderObj.ApplicationMenu != null)
            {
                senderObj.ApplicationMenu.Style = senderObj.ApplicationMenuStyle;
            }
        }

        /// <summary>
        /// Gets the screen tip.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public static ScreenTip GetScreenTip(DependencyObject obj)
        {
            return (ScreenTip)obj.GetValue(ScreenTipProperty);
        }

        /// <summary>
        /// Sets the screen tip.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="value">The value.</param>
        public static void SetScreenTip(DependencyObject obj, ScreenTip value)
        {
            obj.SetValue(ScreenTipProperty, value);
        }

        // Using a DependencyProperty as the backing store for ScreenTip.  This enables animation, styling, binding, etc...
        /// <summary>
        /// Used for displaying Tool Tip of any element.
        /// </summary>
        public static readonly DependencyProperty ScreenTipProperty =
            DependencyProperty.RegisterAttached("ScreenTip", typeof(ScreenTip), typeof(Ribbon), new PropertyMetadata(null, new PropertyChangedCallback(OnScreenTipChanged)));

        /// <summary>
        /// Called when [screen tip changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnScreenTipChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement element = sender as FrameworkElement;
            var _bar = VisualUtils.FindAncestor(sender, typeof(Ribbon));

            if (_bar != null && _bar as UIElement != null)
            {
                ToolTipService.SetToolTip(element, e.NewValue);
                ToolTipService.SetPlacement(element, System.Windows.Controls.Primitives.PlacementMode.Bottom);
                ToolTipService.SetPlacementTarget(element, _bar as UIElement);
            }
            else
            {
                ToolTipService.SetToolTip(element, e.NewValue);
                ToolTipService.SetPlacement(element, System.Windows.Controls.Primitives.PlacementMode.Bottom);
                ToolTipService.SetPlacementTarget(element, element);
            }
        }


        /// <summary>
        /// Initializes the screen tip.
        /// </summary>
        private void InitializeScreenTip()
        {
            foreach (var item in this.GetTabs())
            {
                if (item != null)
                {
                    foreach (var baritem in item.Items.OfType<RibbonBar>())
                    {
                        if (baritem != null)
                        {
                            TextBlock targetRelative= baritem.PartTextBlock;
                            foreach (var baritems in baritem.Items)
                            {
                                if (baritems is ButtonPanel)
                                {
                                    foreach (var grpitm in (baritems as ButtonPanel).Items)
                                    {
                                        SetScreenToolTip(targetRelative, grpitm);
                                    }
                                }
                                else if (baritems as DependencyObject != null)
                                {
                                    SetScreenToolTip(targetRelative, baritems);
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Sets the screen tool tip.
        /// </summary>
        /// <param name="baritem">The baritem.</param>
        /// <param name="itemToSetToolTip">The item to set tool tip.</param>
        private void SetScreenToolTip(TextBlock  baritem, object itemToSetToolTip)
        {
            if (itemToSetToolTip as DependencyObject != null)
            {
                var tooltip = ToolTipService.GetToolTip(itemToSetToolTip as DependencyObject);
                if (tooltip != null)
                {
                    if (itemToSetToolTip as FrameworkElement != null && (tooltip as ScreenTip) != null)
                    {
                        (tooltip as ScreenTip).parentUIElement = itemToSetToolTip as FrameworkElement;
                    }
                    ToolTipService.SetPlacementTarget(itemToSetToolTip as DependencyObject, itemToSetToolTip as UIElement);

                }
            }
        }

        internal void FireOnBackStageColorChanged()
        {
            if (this.BackStage != null)
            {
                (this.BackStage as IBackStageColor).OnBackStageColorChanged(BackStageColor);
                if (BackStageButton != null)
                    (BackStageButton as IBackStageColor).OnBackStageColorChanged(BackStageColor);
                if (this.BackStage.Items != null)
                {
                    foreach (object item in BackStage.Items)
                    {
                        if (item is IBackStageColor)
                        {
                            (item as IBackStageColor).OnBackStageColorChanged(BackStageColor);
                        }
                    }
                }

            }
        }

        private QatCustomizationDialog _qatdialog;

        /// <summary>
        /// Gets the QAT customization dialog.
        /// </summary>
        /// <value>The QAT customization dialog.</value>
        [Description("Used to get QAT customization dialog window")]
        [Category("Ribbon Properties")]
        public QatCustomizationDialog QATCustomizationDialog
        {
            get
            {
                return _qatdialog;
            }
        }

        /// <summary>
        /// Gets or sets the quick access toolbar style.
        /// </summary>
        /// <value>The quick access toolbar style.</value>
        [Description("Used to change Quick Access Toolbar Style")]
        [Category("Ribbon Properties")]
        public Style QuickAccessToolbarStyle
        {
            get { return (Style)GetValue(QuickAccessToolbarStyleProperty); }
            set { SetValue(QuickAccessToolbarStyleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for QuickAccessToolbarStyle.  This enables animation, styling, binding, etc...
        /// <summary>
        /// To describe the Style of QuickAccessToolBar .
        /// </summary>
        public static readonly DependencyProperty QuickAccessToolbarStyleProperty =
            DependencyProperty.Register("QuickAccessToolbarStyle", typeof(Style), typeof(Ribbon), new PropertyMetadata(null, OnQuickAccessToolbarStyleChanged));

        /// <summary>
        /// Called when [quick access toolbar style changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnQuickAccessToolbarStyleChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var senderObj = sender as Ribbon;
            if (senderObj.QuickAccessToolBar != null)
            {
                senderObj.QuickAccessToolBar.Style = senderObj.QuickAccessToolbarStyle;
                if (senderObj.titlePanel != null && senderObj.QuickAccessToolBar != null)
                {
                    senderObj.titlePanel.TempQATToolBar = senderObj.QuickAccessToolBar;
                }
            }
        }

        /// <summary>
        /// Gets or sets the quick access tool bar.
        /// </summary>
        /// <value>The quick access tool bar.</value>
        [Description("Contains frequently used commands")]
        [Category("Ribbon Properties")]
        public QuickAccessToolBar QuickAccessToolBar
        {
            get { return (QuickAccessToolBar)GetValue(QuickAccessToolBarProperty); }
            set { SetValue(QuickAccessToolBarProperty, value); }
        }

        // Using a DependencyProperty as the backing store for QuickAccessToolBar.  This enables animation, styling, binding, etc...
        /// <summary>
        /// Used to describe QuickAccessToolBar in Ribbon Control.
        /// </summary>
        public static readonly DependencyProperty QuickAccessToolBarProperty =
            DependencyProperty.Register("QuickAccessToolBar", typeof(QuickAccessToolBar), typeof(Ribbon), new PropertyMetadata(null, OnQuickAccessToolBarChanged));

        /// <summary>
        /// Called when [quick access tool bar changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnQuickAccessToolBarChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var senderObj = sender as Ribbon;
            if (senderObj.QuickAccessToolBar != null)
            {
                senderObj.QuickAccessToolBar.Style = senderObj.QuickAccessToolbarStyle;
            }
        }

        /// <summary>
        /// Gets or sets the state of the ribbon.
        /// </summary>
        /// <value>The state of the ribbon.</value>
        public RibbonState RibbonState
        {
            get { return (RibbonState)GetValue(RibbonStateProperty); }
            set { SetValue(RibbonStateProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for RibbonState.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty RibbonStateProperty =
            DependencyProperty.Register("RibbonState", typeof(RibbonState), typeof(Ribbon), new PropertyMetadata(RibbonState.Normal, new PropertyChangedCallback(OnRibbonStateChangedCallback)));

        /// <summary>
        /// Called when [ribbon state changed callback].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnRibbonStateChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Ribbon)d).OnRibbonStateChanged();
        }



        /// <summary>
        /// Gets or sets a value indicating whether [save original state].
        /// </summary>
        /// <value><c>true</c> if [save original state]; otherwise, <c>false</c>.</value>
        public bool AutoPersist
        {
            get { return (bool)GetValue(AutoPersistProperty); }
            set { SetValue(AutoPersistProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SaveOriginalState.  This enables animation, styling, binding, etc...
        /// <summary>
        /// Indicates whether to save state persisted on loading.
        /// </summary>
        public static readonly DependencyProperty AutoPersistProperty =
            DependencyProperty.Register("AutoPersist", typeof(bool), typeof(Ribbon), new PropertyMetadata(false));


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



        #region Caption

        /// <summary>
        /// Gets or sets the content of <see cref="Ribbon"/> caption
        /// </summary>
        [Description("Represents the Title which displays ate the top of the Ribbon Control")]
        [Category("Ribbon Properties")]
        public String Title
        {
            get { return (String)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Caption.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(String), typeof(Ribbon), new PropertyMetadata(string.Empty));

        #endregion

        #region ToolBarMenu

        /// <summary>
        /// Gets or sets <see cref="RibbonDropDown"/> of the quick access toolbar's menu button
        /// </summary>
        public RibbonDropDown ToolBarMenu
        {
            get { return (RibbonDropDown)GetValue(ToolBarMenuProperty); }
            set { SetValue(ToolBarMenuProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ToolBarMenu.  This enables animation, styling, binding, etc...
        /// <summary>
        /// Identifies the ToolBarMenu Dependency Property.
        /// </summary>
        public static readonly DependencyProperty ToolBarMenuProperty = DependencyProperty.Register("ToolBarMenu", typeof(RibbonDropDown), typeof(Ribbon), null);

        #endregion

        /// <summary>
        /// Gets or sets a value indicating whether this instance is QAT below.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is QAT below; otherwise, <c>false</c>.
        /// </value>
        public bool IsQATBelow
        {
            get { return (bool)GetValue(IsQATBelowProperty); }
            set { SetValue(IsQATBelowProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for IsQATBelow.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsQATBelowProperty =
            DependencyProperty.Register("IsQATBelow", typeof(bool), typeof(Ribbon), new PropertyMetadata(false, new PropertyChangedCallback(IsQATBelowChangedCallBack)));

        /// <summary>
        /// Determines whether [is QAT below changed call back] [the specified sender].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        public static void IsQATBelowChangedCallBack(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var senderobj = sender as Ribbon;
           
                if (senderobj.IsQATBelow == true)
                {
                    senderobj.QATState = QATState.BelowRibbon;
                    if (senderobj.QuickAccessToolBar != null)
                        senderobj.QuickAccessToolBar.SeperatorVisibility = Visibility.Collapsed;
                }
                else
                {
                    senderobj.QATState = QATState.AboveRibbon;
                    senderobj.QuickAccessToolBar.SeperatorVisibility = Visibility.Visible;
                }
            
        }

        /// <summary>
        /// Gets or sets the state of the QAT.
        /// </summary>
        /// <value>The state of the QAT.</value>
        [Description("Used to show QAT Above or Below the Ribbon or Hide from the Ribbon")]
        [Category("Ribbon Properties")]
        public QATState QATState
        {
            get { return (QATState)GetValue(QATStateProperty); }
            set { SetValue(QATStateProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for QATState. This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty QATStateProperty =
            DependencyProperty.Register("QATState", typeof(QATState), typeof(Ribbon), new PropertyMetadata(QATState.AboveRibbon, new PropertyChangedCallback(OnQATStateChangedCallBack)));

        /// <summary>
        /// 
        /// </summary>
        public Brush BackStageColor
        {
            get { return (Brush)GetValue(BackStageColorProperty); }
            set { SetValue(BackStageColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BackStageColor.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty BackStageColorProperty =
            DependencyProperty.Register("BackStageColor", typeof(Brush), typeof(Ribbon), new PropertyMetadata(new SolidColorBrush(Colors.Blue),new PropertyChangedCallback(OnBackStageColorChanged)));

        
        /// <summary>
        /// 
        /// </summary>
        public object BackStageHeader
        {
            get { return (object)GetValue(BackStageHeaderProperty); }
            set { SetValue(BackStageHeaderProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for BackStageHeader.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty BackStageHeaderProperty =
            DependencyProperty.Register("BackStageHeader", typeof(object), typeof(Ribbon), new PropertyMetadata("File"));
        
        private static void OnBackStageColorChanged(DependencyObject obj,DependencyPropertyChangedEventArgs e)
        {
            Ribbon ribbon = obj as Ribbon;

            if (ribbon != null)
            {
                ribbon.FireOnBackStageColorChanged();
            }
        }

        /// <summary>
        /// Called when [QAT state changed call back].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        public static void OnQATStateChangedCallBack(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var senderobj = sender as Ribbon;
            if (senderobj.IsQATBelowChanged != null)
            {
                senderobj.IsQATBelowChanged(sender, e);
            }

            ((Ribbon)sender).OnQATStateChangedLocal();
        }

        /// <summary>
        /// 
        /// </summary>
        public event PropertyChangedCallback IsQATBelowChanged;

        private ContextMenuAdv _ribbonContextMenu;

        internal  RibbonWindow Window;

        private int selectedIndex = 0;

        /// <summary>
        /// Gets the ribbon context menu.
        /// </summary>
        /// <value>The ribbon context menu.</value>
        public ContextMenuAdv RibbonContextMenu
        {
            get
            {
                return _ribbonContextMenu;
            }
        }

        #region SelectedTab

        /// <summary>
        /// Gets selected <see cref="RibbonTab"/>
        /// </summary>
        public RibbonTab SelectedTabItem
        {
            get
            {
                return this.SelectedTabInternal;
            }
        }

        #endregion

        #region SelectedTabInternal

        /// <summary>
        /// Gets or sets the selected tab internal.
        /// </summary>
        /// <value>The selected tab internal.</value>
        private RibbonTab SelectedTabInternal
        {
            get
            {
                return this.selectedTab;
            }

            set
            {
                if (value != null && this.selectedTab != value)
                {
                    if (this.selectedTab != null)
                    {
                        this.selectedTab.IsCollapsed = true;
                        this.selectedTab.IsChecked = false;
                    }

                    if (value.Visibility == Visibility.Visible)
                    {
                        this.selectedTab = value;
                    }

                    if (this.selectedTab != null)
                    {
                        this.selectedTab.IsChecked = true;
                    }

                    this.SelectedIndex = this.GetTabs().IndexOf(this.selectedTab);
                    
                    this.OnSelectedTabChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is in ribbon window.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is in ribbon window; otherwise, <c>false</c>.
        /// </value>
        public bool IsInRibbonWindow
        {
            get { return (bool)GetValue(IsInRibbonWindowProperty); }
            set { SetValue(IsInRibbonWindowProperty, value); }
        }

       
        /// <summary>
        /// Using a DependencyProperty as the backing store for IsInRibbonWindow.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsInRibbonWindowProperty =
            DependencyProperty.Register("IsInRibbonWindow", typeof(bool), typeof(Ribbon), new PropertyMetadata(false, new PropertyChangedCallback(OnIsInRibbonWindowChanged)));

        /// <summary>
        /// Called when [is in ribbon window changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnIsInRibbonWindowChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            Ribbon _sender = sender as Ribbon;
            _sender.UpdateMargin();
        }

        /// <summary>
        /// Updates the margin.
        /// </summary>
        private void UpdateMargin()
        {
            if (IsInRibbonWindow)
            {
                this.Margin = new Thickness(0);
                if (_title != null)
                {
                    _title.Visibility = System.Windows.Visibility.Collapsed;
                }
            }
            else
            {
                this.Margin = new Thickness(0, 24, 0, 0);
                if (_title != null)
                {
                    _title.Visibility = System.Windows.Visibility.Visible;
                }
            }
        }

        #endregion

        #endregion

        /// <summary>
        /// Raises the <see cref="E:System.Windows.UIElement.MouseRightButtonUp"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Input.MouseButtonEventArgs"/> that contains the event data.</param>
        protected override void OnMouseRightButtonUp(MouseButtonEventArgs e)
        {
            var obj = e.OriginalSource;
            var items = VisualTreeHelper.FindElementsInHostCoordinates(e.GetPosition(this), this);
            base.OnMouseRightButtonUp(e);
        }

        #region Overrides

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes (such as a rebuilding layout pass) call <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
             base.OnApplyTemplate();

            if (this.QuickAccessToolBar != null)
            {
                this.QuickAccessToolBar.OnMinimizeRibbonClick -= new EventHandler(QuickAccessToolBar_OnMinimizeRibbonClick);
                this.QuickAccessToolBar.OnMoreCommandsClick -= new EventHandler(QuickAccessToolBar_OnMoreCommandsClick);
                this.QuickAccessToolBar.OnShowBelowtheRibbonClick -= new EventHandler(QuickAccessToolBar_OnShowBelowtheRibbonClick);
            }

            this.InitTabStrip();

            this.InitPopupHost();

            this.tabPresenter = this.GetTemplateChild("Part_TabPresenter") as ItemsPresenter;

            _title = GetTemplateChild("Title") as Border;

            this.normalTabHost = this.GetTemplateChild("Part_NormalTabHost") as ContentPresenter;
            titlePanel = GetTemplateChild("Part_TitlePanel") as RibbonTitlePanel;

            if (titlePanel != null && this.QuickAccessToolBar != null)
            {
                titlePanel.TempQATToolBar = this.QuickAccessToolBar;
            }

            this.InitilizeQuickAccessToolBar();
            if (this.QuickAccessToolBar != null)
            {
                _qatdialog = new QatCustomizationDialog(this, this.QuickAccessToolBar);
                _qatdialog.Closed += new EventHandler(_qatdialog_Closed);
            }
            _morecommands = GetTemplateChild("PART_MoreCommands") as ContextMenuItemAdv;
            if (_morecommands != null)
            {
                _morecommands.Header = ResourceWrapperKeys.MoreCommands;
                _morecommands.Click += new RoutedEventHandler(QuickAccessToolBar_OnMoreCommandsClick);
            }

            _showbelow = GetTemplateChild("PART_ShowBelow") as ContextMenuItemAdv;
            if (_showbelow != null)
            {
                _showbelow.Header = ResourceWrapperKeys.QATShowBelow;
                _showbelow.Click += new RoutedEventHandler(QuickAccessToolBar_OnShowBelowtheRibbonClick);
            }

            _minimizeribbon = GetTemplateChild("PART_Minimize") as ContextMenuItemAdv;
            if (_minimizeribbon != null)
            {
                _minimizeribbon.Header = ResourceWrapperKeys.MinimizeRibbon;
                _minimizeribbon.Click += new RoutedEventHandler(QuickAccessToolBar_OnMinimizeRibbonClick);
            }

            if (this.BackStagePopUp != null)
                this.BackStagePopUp.Child = null;

            this.BackStagePopUp = this.GetTemplateChild("Part_PopUp") as Popup;

            if (BackStagePopUp != null)
            {
                this.BackStagePopUp.Child = this.BackStage;
            }

            this.BackStageButton = this.GetTemplateChild("Part_BackStageButton") as BackStageButton;

            if (this.BackStageButton != null)
            {
                this.BackStageButton.parentRibbon = this;
                this.BackStageButton.Click += new RoutedEventHandler(BackStageButton_Click);
            }

            this.ribbonToggleButton = this.GetTemplateChild("Part_ToggleButton") as ToggleButton;

            if (this.ribbonToggleButton != null)
                this.ribbonToggleButton.Click += new RoutedEventHandler(ribbonToggleButton_Click);

            if (this.ApplicationMenu != null)
            {
                this.ApplicationMenu.Style = this.ApplicationMenuStyle;
            }

            if (QuickAccessToolBar == null && _showbelow != null && _morecommands != null)
            {
                _showbelow.IsEnabled = _morecommands.IsEnabled = false;
            }

            this.Loaded -= new RoutedEventHandler(Ribbon_Loaded);
            this.Loaded += new RoutedEventHandler(Ribbon_Loaded);

            OnQATStateChangedLocal();

            Grid _ribbon = this.GetTemplateChild("Ribbon") as Grid;
            if (_ribbon != null)
            {
                _ribbonContextMenu = ContextMenuAdvService.GetContextMenuAdv(_ribbon);
            }
            if (_ribbonContextMenu != null)
            {
                _ribbonContextMenu.Opened += new RoutedEventHandler(_ribbonContextMenu_Opened);
                _ribbonContextMenu.Closed += new RoutedEventHandler(_ribbonContextMenu_Closed);
            }
            UpdateMargin();
            this.Window = (RibbonWindow)this.GetRibbonWindow(this, typeof(RibbonWindow));
            if (this.IsInRibbonWindow)
            {
                Window.Closing += new ClosedEventHandler(Window_Closing);
                this.Window.SizeChanged += new SizeChangedEventHandler(Window_SizeChanged);
            }
            if(Window != null)
                Window.ribbon = this;
            FireOnBackStageColorChanged();

            GetCurrentThemeTabItemStyle();
        }

        void BackStageButton_Click(object sender, RoutedEventArgs e)
        {
            //if (!this.BackStageButton.IsOpen && this.BackStage != null)
            //    this.BackStage.Focus();
        }

        /// <summary>
        /// Handles the SizeChanged event of the Window control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.SizeChangedEventArgs"/> instance containing the event data.</param>
        void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            string currentVisualStyle = (Syncfusion.Windows.Controls.Theming.SkinManager.GetVisualStyle(this)).ToString();
            if (currentVisualStyle.Contains("Office2010") && this.IsInRibbonWindow)
            {
                if (this.BackStage != null)
                {
                    if (Window.Height > 55 && Window.Width > 5)
                    {
                        this.BackStage.Width = this.Window.Width - 9;
                        this.BackStage.Height = this.Window.Height - 55;
                    }
                }
            }
        }

        /// <summary>
        /// Handles the Click event of the ribbonToggleButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void ribbonToggleButton_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)ribbonToggleButton.IsChecked)
                this.RibbonState = RibbonState.Hide;
            else if (!(bool)ribbonToggleButton.IsChecked)
                this.RibbonState = RibbonState.Normal;
        }

        /// <summary>
        /// Handles the Closing event of the Window control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Syncfusion.Windows.Tools.Controls.ClosedEventArgs"/> instance containing the event data.</param>
        void Window_Closing(object sender, ClosedEventArgs e)
        {
            if (this.AutoPersist || this.QuickAccessToolBar != null && this.QuickAccessToolBar.AutoPersist || this.IsInRibbonWindow && this.Window != null && this.Window.AutoPersist)
                SaveDefaultState(default_StoreFile);
        }

        /// <summary>
        /// Handles the Closed event of the _ribbonContextMenu control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void _ribbonContextMenu_Closed(object sender, RoutedEventArgs e)
        {
            RemoveAddToQATMenuItemFromContextmenu();
        }

        /// <summary>
        /// Handles the Opened event of the _ribbonContextMenu control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void _ribbonContextMenu_Opened(object sender, RoutedEventArgs e)
        {
            if (OnRibbonContextMenuOpened != null)
                OnRibbonContextMenuOpened(sender, e);
        }

        /// <summary>
        /// Handles the Loaded event of the Ribbon control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void Ribbon_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeScreenTip();
            OnRibbonStateChanged();

            RibbonWindow window = (RibbonWindow)VisualUtils.FindAncestor(this, typeof(RibbonWindow));
            if (window != null)
            {
                window.BackStageColor = BackStageColor;
                //window.ribbon=this;
            }

            if (!DesignerProperties.IsInDesignTool)
            {
                if (this.AutoPersist || this.QuickAccessToolBar != null && this.QuickAccessToolBar.AutoPersist || this.IsInRibbonWindow && this.Window != null && this.Window.AutoPersist)
                {
                    CheckInitialStates();
                    SaveInitialState();
                    LoadDefaultState(default_StoreFile);
                }
            }
            this.SelectedIndex = this.GetTabs().IndexOf(this.SelectedTabItem);       
        }

        /// <summary>
        /// Gets the last tab button style.
        /// </summary>
        private void GetLastTabButtonStyle()
        {
            foreach (var item in this.Items)
            {
                if (item is RibbonTab)
                {
                    RibbonTab tab = item as RibbonTab;
                    //tempTabButtonStyle = st;
                    break;
                }
            }
        }

        /// <summary>
        /// List to store Initial QAT static items.
        /// </summary>
        private List<UIElement> InitialStaticQATItems = new List<UIElement>();

        /// <summary>
        /// Checks the initial states.
        /// </summary>
        private void CheckInitialStates()
        {
            if (this.QuickAccessToolBar != null)
            {
                foreach (var item in this.QuickAccessToolBar.Items)
                    this.InitialStaticQATItems.Add((UIElement)item);
            }
        }

        /// <summary>
        /// Called before the <see cref="E:System.Windows.UIElement.MouseWheel"/> event occurs to provide handling for the event in a derived class without attaching a delegate.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Input.MouseWheelEventArgs"/> that contains the event data.</param>
        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);
            this.TabScrolling(e.Delta);
        }


        /// <summary>
        /// Occurs when [on ribbon context menu opened].
        /// </summary>
        public event RoutedEventHandler OnRibbonContextMenuOpened;

        ContextMenuItemAdv _morecommands;
        ContextMenuItemAdv _showbelow;
        ContextMenuItemAdv _minimizeribbon;

        /// <summary>
        /// Handles the Closed event of the _qatdialog control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void _qatdialog_Closed(object sender, EventArgs e)
        {
            if (_qatdialog.DialogResult)
            {
                if (_qatdialog.QATState.Value && QATState != QATState.BelowRibbon)
                    QATState = Controls.QATState.BelowRibbon;

                else if (!_qatdialog.QATState.Value && QATState != QATState.AboveRibbon)
                    QATState = Controls.QATState.AboveRibbon;
            }
            else
            {
                if (this.QATState == QATState.AboveRibbon)
                    _qatdialog.QATState = false;
                else
                    _qatdialog.QATState = true;              
            }
        }

        /// <summary>
        /// Initilizes the quick access tool bar.
        /// </summary>
        private void InitilizeQuickAccessToolBar()
        {
            if (this.QuickAccessToolBar != null)
            {
                this.QuickAccessToolBar.parentRibbon = this;
                this.QuickAccessToolBar.Style = this.QuickAccessToolbarStyle;
                this.QuickAccessToolBar.OnMinimizeRibbonClick += new EventHandler(QuickAccessToolBar_OnMinimizeRibbonClick);
                this.QuickAccessToolBar.OnMoreCommandsClick += new EventHandler(QuickAccessToolBar_OnMoreCommandsClick);
                this.QuickAccessToolBar.OnShowBelowtheRibbonClick += new EventHandler(QuickAccessToolBar_OnShowBelowtheRibbonClick);
            }
            this.PART_QATAboveContainer = this.GetTemplateChild("PART_QATAboveContainer") as Grid;
            this.PART_QATBelowContainer = this.GetTemplateChild("PART_QATBelowContainer") as Grid;
            this.Part_QATBorderContainer = this.GetTemplateChild("Part_QATBorderContainer") as Border;
        }

        private Grid PART_QATAboveContainer;
        private Grid PART_QATBelowContainer;
        private Border Part_QATBorderContainer;

        /// <summary>
        /// Called when [QAT state changed local].
        /// </summary>
        private void OnQATStateChangedLocal()
        {
            if (QuickAccessToolBar == null) return;
            if (this.QATState == QATState.AboveRibbon)
            {
                if (this.IsQATBelow)
                    this.IsQATBelow = false;
                this.QuickAccessToolBar.QATButtonCaption = ResourceWrapperKeys.QATShowBelow;
                QuickAccessToolBar.CurrentQATState = QATState.AboveRibbon;
                this.QuickAccessToolBar.HasGeometry = true;

                this._qatdialog.QATState = false;

                this.QuickAccessToolBar.PathGeometryVisibility = Visibility.Visible;

                if (QuickAccessToolBar.PathGeometryVisibility == Visibility.Collapsed)
                {
                    QuickAccessToolBar.Margin = new Thickness(20, 0, 0, 0);
                }

                if (this.PART_QATAboveContainer != null && this.PART_QATBelowContainer.Children.Count > 0 && this.PART_QATBelowContainer != null)
                {
                    var qatitem = this.PART_QATBelowContainer.Children[0];
                    this.PART_QATBelowContainer.Children.Clear();
                    this.PART_QATAboveContainer.Children.Clear();
                    this.PART_QATAboveContainer.Children.Add(qatitem);
                }
                if (Part_QATBorderContainer != null)
                    Part_QATBorderContainer.Visibility = System.Windows.Visibility.Collapsed;

                if (this.PART_QATAboveContainer != null && PART_QATAboveContainer.Visibility == System.Windows.Visibility.Collapsed)
                    PART_QATAboveContainer.Visibility = System.Windows.Visibility.Visible;
            }
            else if (this.QATState == QATState.BelowRibbon)
            {
                if (!this.IsQATBelow) 
                    this.IsQATBelow = true;
                QuickAccessToolBar.CurrentQATState = QATState.BelowRibbon;
                this.QuickAccessToolBar.QATButtonCaption = ResourceWrapperKeys.QATShowAbove;

                this.QuickAccessToolBar.HasGeometry = false;
                this.QuickAccessToolBar.PathGeometryVisibility = Visibility.Collapsed;
                this.QuickAccessToolBar.Margin = new Thickness(0, 0, 0, 0);

                this._qatdialog.QATState = true;

                if (this.PART_QATAboveContainer != null && this.PART_QATAboveContainer.Children.Count > 0 && this.PART_QATBelowContainer != null)
                {
                    var qatitem = this.PART_QATAboveContainer.Children[0];
                    this.PART_QATAboveContainer.Children.Clear();
                    this.PART_QATBelowContainer.Children.Clear();
                    this.PART_QATBelowContainer.Children.Add(qatitem);
                }
                if (Part_QATBorderContainer != null)
                    Part_QATBorderContainer.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                QuickAccessToolBar.CurrentQATState = QATState.Hidden;

                if (Part_QATBorderContainer != null && Part_QATBorderContainer.Visibility == System.Windows.Visibility.Visible)
                    Part_QATBorderContainer.Visibility = System.Windows.Visibility.Collapsed;

                if (this.PART_QATAboveContainer != null && PART_QATAboveContainer.Visibility == System.Windows.Visibility.Visible)
                    PART_QATAboveContainer.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Handles the OnShowBelowtheRibbonClick event of the QuickAccessToolBar control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void QuickAccessToolBar_OnShowBelowtheRibbonClick(object sender, EventArgs e)
        {
            ShowBelowExecute();
        }

        /// <summary>
        /// Handles the OnMoreCommandsClick event of the QuickAccessToolBar control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void QuickAccessToolBar_OnMoreCommandsClick(object sender, EventArgs e)
        {
            MoreCommandsExecute();
        }

        /// <summary>
        /// Handles the OnMinimizeRibbonClick event of the QuickAccessToolBar control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void QuickAccessToolBar_OnMinimizeRibbonClick(object sender, EventArgs e)
        {
            MinimizeExecute();
        }

        /// <summary>
        /// Shows the below execute.
        /// </summary>
        private void ShowBelowExecute()
        {
            if (this.QATState == QATState.AboveRibbon)
            {
                this.QATState = QATState.BelowRibbon;
                _showbelow.Header = ResourceWrapperKeys.QATShowAbove;
            }
            else
            {
                this.QATState = QATState.AboveRibbon;
                _showbelow.Header = ResourceWrapperKeys.QATShowBelow;
            }
        }

        /// <summary>
        /// Shows the back stage.
        /// </summary>
        internal void ShowBackStage()
        {
            this.SelectedTabInternal.IsChecked = false;
            ChangeEnableQATandToggleButton(false);
        }


        /// <summary>
        /// Hides the back stage.
        /// </summary>
        internal void HideBackStage()
        {
            this.SelectedTabInternal.Visibility = Visibility.Visible;
            this.SelectedTabInternal.IsChecked = true;
            ChangeEnableQATandToggleButton(true);
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
        /// Mores the commands execute.
        /// </summary>
        private void MoreCommandsExecute()
        {
            _qatdialog.Show();
            if (QuickAccessToolBar != null && QuickAccessToolBar.PART_QATDropDownControl != null && QuickAccessToolBar.PART_QATDropDownControl._dropdown != null)
                QuickAccessToolBar.PART_QATDropDownControl._dropdown.IsOpen = false;
        }

        /// <summary>
        /// Minimizes the execute.
        /// </summary>
        private void MinimizeExecute()
        {
            if (QuickAccessToolBar != null && QuickAccessToolBar.PART_QATDropDownControl != null && QuickAccessToolBar.PART_QATDropDownControl._dropdown != null)
                QuickAccessToolBar.PART_QATDropDownControl._dropdown.IsOpen = false;
            if (this.RibbonState == Controls.RibbonState.Hide)
            {
                _minimizeribbon.IsChecked = false;
                this.RibbonState = Controls.RibbonState.Normal;
            }
            else if (this.RibbonState == Controls.RibbonState.Normal)
            {
                _minimizeribbon.IsChecked = true;
                this.RibbonState = Controls.RibbonState.Hide;
            }
        }

        ///// <summary>
        ///// Provides the behavior for the "measure" pass of Silverlight layout. Classes can override this method to define their own measure pass behavior.
        ///// </summary>
        ///// <param name="availableSize">The available size that this object can give to child objects. Infinity can be specified as a value to indicate that the object will size to whatever content is available.</param>
        ///// <returns>
        ///// The size that this object determines it needs during layout, based on its calculations of child object allotted sizes.
        ///// </returns>
        //protected override Size MeasureOverride(Size availableSize)
        //{
        //    this.UpdateTabsHeight();

        //    return base.MeasureOverride(availableSize);
        //}

        /// <summary>
        /// Determines if the specified item is (or is eligible to be) its own container.
        /// </summary>
        /// <param name="item">The item to check.</param>
        /// <returns>
        /// true if the item is (or is eligible to be) its own container; otherwise, false.
        /// </returns>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return (item is RibbonTab) || (item is RibbonTabsGroup);
        }

        /// <summary>
        /// Creates or identifies the element that is used to display the given item.
        /// </summary>
        /// <returns>
        /// The element that is used to display the given item.
        /// </returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new RibbonTab();
        }

        /// <summary>
        /// Prepares the specified element to display the specified item.
        /// </summary>
        /// <param name="element">The element used to display the specified item.</param>
        /// <param name="item">The item to display.</param>
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            if (this.PrepareRibbonTab(element, item) || this.PrepareRibbonTabsGroup(element, item))
            {
                this.OnTabsCollectionChanged();
            }
        }

        /// <summary>
        /// Undoes the effects of the <see cref="M:System.Windows.Controls.ItemsControl.PrepareContainerForItemOverride(System.Windows.DependencyObject,System.Object)"/> method.
        /// </summary>
        /// <param name="element">The container element.</param>
        /// <param name="item">The target item.</param>
        protected override void ClearContainerForItemOverride(DependencyObject element, object item)
        {
            if (this.ClearRibbonTab(element, item) ||
                this.ClearRibbonTabsGroup(element, item))
            {
                this.OnTabsCollectionChanged();
            }
        }

        /// <summary>
        /// Called before the <see cref="E:System.Windows.UIElement.MouseLeftButtonDown"/> event occurs.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            RibbonTabsGroupItem tabsGroupItem = e.OriginalSource as RibbonTabsGroupItem;

            if (tabsGroupItem != null)
            {
                RibbonTabsGroup tabsGroup = tabsGroupItem.TabsGroup;

                if (tabsGroup != null)
                {
                    RibbonTabCollection tabs = tabsGroup.Tabs;

                    if (tabs.Count > 0)
                    {
                        foreach (RibbonTab tab in tabs)
                        {
                            if (tab.IsCollapsed)
                            {
                                tab.IsChecked = true;
                                break;
                            }
                        }
                    }
                }

                e.Handled = true;
            }

            base.OnMouseLeftButtonDown(e);
        }

        /// <summary>
        /// Invoked when the <see cref="P:System.Windows.Controls.ItemsControl.Items"/> property changes.
        /// </summary>
        /// <param name="e">Information about the change.</param>
        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    this.OnItemsAdded(e);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    this.OnItemsRemoved(e);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    this.OnItemsRemoved(e);
                    this.OnItemsAdded(e);
                    break;
            }

            base.OnItemsChanged(e);
        }

        /// <summary>
        /// Called when [selected tab changed].
        /// </summary>
        internal virtual void OnSelectedTabChanged()
        {
            if (this.SelectedTabChanged != null)
            {
                this.SelectedTabChanged(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Checks for ribbon tab is checked.
        /// </summary>
        private void CheckForRibbonTabIsChecked()
        {
            int cnt = 0;
            var CurrentTabs = this.GetTabs();
            foreach (RibbonTab item in CurrentTabs)
            {
                if (item != null && item.IsChecked)
                {
                    cnt++;
                    break;
                }
            }
            if (cnt <= 0 && CurrentTabs.Count > 0)
                (this.GetTabs()[0] as RibbonTab).IsChecked = true;
        }

        #endregion

        #region Event handlers

        /// <summary>
        /// Called when [item clicked].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnItemClicked(object sender, System.EventArgs e)
        {
            if (this.RibbonState == Controls.RibbonState.Hide)
            {
                if (this.popupTabHost != null)
                {
                    this.popupTabHost.IsOpen = false;
                }
            }
        }

        /// <summary>
        /// Called when [minimize ribbon item click].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void OnMinimizeRibbonItemClick(object sender, RoutedEventArgs e)
        {
            ToggleRibbonState();
        }

        /// <summary>
        /// Mouses down handler.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void MouseDownHandler(object sender, MouseButtonEventArgs e)
        {
            TabButton item = this.tabStrip.GetItemAt(e);

            if (item != null)
            {
                bool bIsSelected = item.IsSelected;

                item.ProcessMouseDown(e);

                if (!bIsSelected)
                {
                    e.Handled = true;
                }
            }
            SetTabItemStyles();
        }

        /// <summary>
        /// Sizes the changed handler.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.SizeChangedEventArgs"/> instance containing the event data.</param>
        private void SizeChangedHandler(object sender, SizeChangedEventArgs e)
        {
            if (this.RibbonState == Controls.RibbonState.Hide)
            {
                if (this.popupTabHost != null)
                {
                    this.popupTabHost.IsOpen = false;
                }
            }
        }

        /// <summary>
        /// Determines whether [is open changed handler] [the specified sender].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void IsOpenChangedHandler(object sender, EventArgs e)
        {
            if (this.tabStrip != null)
            {
                this.tabStrip.ShowSelectedTab = (this.RibbonState == Controls.RibbonState.Normal) || this.popupTabHost.IsOpen;
                //if (this.popupTabHost.IsOpen)
                //    GetLastTabButtonStyle();
            }
        }

        /// <summary>
        /// Tabs the strip arranged handler.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void TabStripArrangedHandler(object sender, EventArgs e)
        {
            if (this.titlePanel != null)
            {
                this.titlePanel.OnLayoutChanged();
            }
        }

        /// <summary>
        /// Called when [tab is selected changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnTabIsSelectedChanged(object sender, EventArgs e)
        {
            RibbonTab tab = sender as RibbonTab;

            if (tab.IsChecked)
            {
                this.SelectedTabInternal = tab;
                if (this.BackStageButton != null && this.BackStageButton.IsOpen)
                    this.BackStageButton.IsOpen = false;
            }

            if (System.ComponentModel.DesignerProperties.IsInDesignTool)
                CheckForRibbonTabIsChecked();
        }

        /// <summary>
        /// Called when [tabs group tabs changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        private void OnTabsGroupTabsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (RibbonTab tab in e.NewItems)
                    {
                        if (tab.IsWrapper)
                        {
                            this.OnRibbonTabAdded(tab);
                        }
                    }

                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (RibbonTab tab in e.OldItems)
                    {
                        if (tab.IsWrapper)
                        {
                            this.OnRibbonTabRemoved(tab);
                        }
                    }

                    break;
            }

            this.OnTabsCollectionChanged();
        }

        #endregion

        #region Implementation

        /// <summary>
        /// Inits the tab strip.
        /// </summary>
        private void InitTabStrip()
        {
            if (this.tabStrip != null)
            {
                this.tabStrip.Arranged -= new System.EventHandler(this.TabStripArrangedHandler);

                this.tabStrip.Selector = null;

                this.tabStrip.Items.Clear();

                this.tabStrip = null;
            }

            this.tabStrip = this.GetTemplateChild("Part_TabStrip") as RibbonTabStrip;

            if (this.tabStrip != null)
            {
                this.tabStrip.Arranged += new System.EventHandler(this.TabStripArrangedHandler);

                this.tabStrip.Selector = this;
            }
        }


        /// <summary>
        /// Tabs the scrolling.
        /// </summary>
        /// <param name="delta">The delta.</param>
        private void TabScrolling(int delta)
        {
            if (RibbonState == RibbonState.Normal)
            {
                List<RibbonTab> TabItems = this.GetTabs();
                if (TabItems.Count > 0)
                {
                    if (delta < 0)
                    {
                        for (int i = SelectedIndex + 1; i < TabItems.Count; i++)
                        {
                            RibbonTab selectedTab = this.SelectedTabItem;

                            if (selectedTab != null)
                            {
                                selectedTab.IsCollapsed = true;
                                selectedTab.IsChecked = false;
                            }

                            RibbonTab item = TabItems[i] as RibbonTab;
                            if (item != null)
                            {
                                item.IsChecked = true;
                            }
                            this.SelectedIndex = i;
                            
                        }
                    }
                    else if (delta > 0)
                    {
                        for (int i = SelectedIndex - 1; i >= 0; i--)
                        {
                            if (i < TabItems.Count)
                            {
                                RibbonTab selectedTab = this.SelectedTabItem;

                                if (selectedTab != null)
                                {
                                    selectedTab.IsCollapsed = true;
                                    selectedTab.IsChecked = false;
                                }

                                RibbonTab item = TabItems[i] as RibbonTab;
                                if (item != null)
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
        }


        /// <summary>
        /// Inits the popup host.
        /// </summary>
        private void InitPopupHost()
        {
            if (this.popupTabHost != null)
            {
                this.popupTabHost.IsOpenChanged -= new System.EventHandler(this.IsOpenChangedHandler);
                this.popupTabHost.OutsideRectMouseDown -= new MouseButtonEventHandler(this.MouseDownHandler);
            }

            this.popupTabHost = this.GetTemplateChild("Part_PopupTabHost") as RibbonDropDown;

            if (this.popupTabHost != null)
            {
                this.popupTabHost.IsOpenChanged += new System.EventHandler(this.IsOpenChangedHandler);
                this.popupTabHost.OutsideRectMouseDown += new MouseButtonEventHandler(this.MouseDownHandler);
            }
        }

        /// <summary>
        /// Called when [tabs collection changed].
        /// </summary>
        private void OnTabsCollectionChanged()
        {
            this.UpdateTabsHeight();

            this.UpdateTabStrip();
        }

        /// <summary>
        /// Updates the height of the tabs.
        /// </summary>
        internal void UpdateTabsHeight()
        {
            if (this.tabPresenter != null)
            {
                double height = 0;

                foreach (RibbonTab tab in this.GetTabs())
                {
                    if (tab != null)
                    {
                        Visibility visibility = tab.Visibility;

                        tab.Visibility = Visibility.Visible;

                        tab.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

                        if (height < tab.DesiredSize.Height)
                        {
                            height = tab.DesiredSize.Height;
                        }

                        tab.Visibility = visibility;

                        if (System.ComponentModel.DesignerProperties.IsInDesignTool)
                        {
                            if (this.SelectedTabInternal != null && tab != this.SelectedTabInternal && tab.Part_Background != null)
                            {
                                tab.Part_Background.Visibility = System.Windows.Visibility.Collapsed;
                            }
                            else if (tab.Part_Background != null)
                            {
                                tab.Part_Background.Visibility = System.Windows.Visibility.Visible;
                            }
                        }
                    }
                }

                this.tabPresenter.MinHeight = height;
            }
        }

        /// <summary>
        /// Binds the tab properties.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="path">The path.</param>
        /// <param name="source">The source.</param>
        /// <param name="target">The target.</param>
        private void BindTabProperties(DependencyProperty property, string path, DependencyObject source, DependencyObject target)
        {
            Binding binding = new Binding(path);
            binding.Source = source;
            BindingOperations.SetBinding(target, property, binding);
        }

        /// <summary>
        /// Updates the tab strip.
        /// </summary>
        internal void UpdateTabStrip()
        {
            if (this.tabStrip != null)
            {
                this.tabStrip.Items.Clear();

                foreach (RibbonTab tab in this.GetTabs())
                {
                    TabButton tabItem = tab.TabItem;
                    this.BindTabProperties(TabButton.FontSizeProperty, "FontSize", tab, tab.TabItem);
                    this.BindTabProperties(TabButton.FontFamilyProperty, "FontFamily", tab, tab.TabItem);
                    this.BindTabProperties(TabButton.FontStyleProperty, "FontStyle", tab, tab.TabItem);
                    this.BindTabProperties(TabButton.FontWeightProperty, "FontWeight", tab, tab.TabItem);
                    this.BindTabProperties(TabButton.ForegroundProperty, "Foreground", tab, tab.TabItem);
                    this.BindTabProperties(TabButton.VisibilityProperty, "TabButtonVisiblity", tab, tab.TabItem);
                    if (tabItem != null && tabItem.Visibility == Visibility.Visible)
                    {
                        this.tabStrip.Items.Add(tabItem);
                    }
                }
            }
        }

        /// <summary>
        /// To store Ribbon State as temporarly.
        /// </summary>
        private RibbonState tempRibbonState = RibbonState.Normal;

        private Style tempTabButtonStyle = null;

        /// <summary>
        /// Called when [is minimized changed].
        /// </summary>
        private void OnRibbonStateChanged()
        {
            if (tempRibbonState == RibbonState)
            {
                foreach (var item in this.Items)
                {
                    if (item is RibbonTab)
                    {
                        RibbonTab tab = item as RibbonTab;
                        tempTabButtonStyle = tab.TabButtonStyle;
                        break;
                    }
                }
                SetTabItemStyles();
                return;
            }
            if (this.normalTabHost != null && this.popupTabHost != null)
            {
                if (this.RibbonState == Controls.RibbonState.Hide)
                {
                    object content = this.normalTabHost.Content;

                    foreach (var item in this.Items)
                    {
                        if (item is RibbonTab)
                        {
                            RibbonTab tab = item as RibbonTab;
                            tempTabButtonStyle = tab.TabButtonStyle;
                            break;
                        }
                    }

                    this.normalTabHost.Content = null;
                    this.popupTabHost.Content = content;

                    SetTabItemStyles();


                    if (this.tabStrip != null)
                    {
                        this.tabStrip.ShowSelectedTab = this.popupTabHost.IsOpen;
                    }

                    if (this._minimizeribbon != null)
                    {
                        this._minimizeribbon.IsChecked = true;
                    }
                    if (QuickAccessToolBar != null && QuickAccessToolBar.PART_QATDropDownControl != null && QuickAccessToolBar.PART_QATDropDownControl.PART_MinimizeRibbon != null)
                    {
                        QuickAccessToolBar.PART_QATDropDownControl.PART_MinimizeRibbon.IsChecked = true;
                    }
                }
                else if (this.RibbonState == Controls.RibbonState.Normal)
                {
                    object content = this.popupTabHost.Content;

                    foreach (var item in this.Items)
                    {
                        if (item is RibbonTab)
                        {
                            RibbonTab tab = item as RibbonTab;
                            tempTabButtonStyle = tab.TabButtonStyle;
                            break;
                        }
                    }

                    this.popupTabHost.Content = null;
                    this.normalTabHost.Content = content;

                    SetTabItemStyles();

                    if (this.tabStrip != null)
                    {
                        this.tabStrip.ShowSelectedTab = true;
                    }
                    if (this._minimizeribbon != null)
                    {
                        this._minimizeribbon.IsChecked = false;
                    }
                    if (QuickAccessToolBar != null && QuickAccessToolBar.PART_QATDropDownControl != null && QuickAccessToolBar.PART_QATDropDownControl.PART_MinimizeRibbon != null)
                    {
                        QuickAccessToolBar.PART_QATDropDownControl.PART_MinimizeRibbon.IsChecked = false;
                    }
                }
            }
            tempRibbonState = RibbonState;
        }

        /// <summary>
        /// Sets the tab item styles.
        /// </summary>
        private void SetTabItemStyles()
        {
            foreach (RibbonTab ribbonTab in this.GetTabs())                
                    ribbonTab.TabItem.Style = this.tempTabButtonStyle;               
        }

        /// <summary>
        /// Gets the current theme tab item style.
        /// </summary>
        private void GetCurrentThemeTabItemStyle()
        {
            foreach (var item in this.Items)
            {
                if (item is RibbonTab)
                {
                    RibbonTab tab = item as RibbonTab;
                    tempTabButtonStyle = tab.TabButtonStyle;
                    break;
                }
            }
        }

        /// <summary>
        /// Prepares the ribbon tab.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        private bool PrepareRibbonTab(DependencyObject element, object item)
        {
            bool bResult = false;

            RibbonTab tab = element as RibbonTab;

            if (tab != null)
            {
                if (!object.ReferenceEquals(element, item))
                {
                    ContentPresenter presenter = new ContentPresenter();

                    presenter.Content = item;

                    tab.Items.Add(presenter);

                    this.tabs[item] = tab;

                    this.OnRibbonTabAdded(tab);
                }

                bResult = true;
            }

            return bResult;
        }

        /// <summary>
        /// Prepares the ribbon tabs group.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        private bool PrepareRibbonTabsGroup(DependencyObject element, object item)
        {
            bool bResult = false;

            RibbonTabsGroup tabsGroup = element as RibbonTabsGroup;

            if (tabsGroup != null)
            {
                tabsGroup.TabsChanged += new NotifyCollectionChangedEventHandler(this.OnTabsGroupTabsChanged);

                if (this.titlePanel != null)
                {
                    RibbonTabsGroupItem tabsGroupItem = new RibbonTabsGroupItem();

                    tabsGroupItem.TabsGroup = tabsGroup;
                    tabsGroup.TabsGroupItem = tabsGroupItem;

                    this.titlePanel.GroupItems.Add(tabsGroupItem);
                }

                this.OnTabsCollectionChanged();

                bResult = true;
            }

            return bResult;
        }

        /// <summary>
        /// Clears the ribbon tab.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        private bool ClearRibbonTab(DependencyObject element, object item)
        {
            bool bResut = false;

            RibbonTab tab = element as RibbonTab;

            if (tab != null)
            {
                if (!object.ReferenceEquals(element, item))
                {
                    foreach (object obj in tab.Items)
                    {
                        ContentPresenter presenter = obj as ContentPresenter;

                        if (presenter != null)
                        {
                            presenter.Content = null;
                        }
                    }

                    tab.Items.Clear();

                    if (this.tabs.ContainsKey(item))
                    {
                        this.tabs.Remove(item);
                    }

                    this.OnRibbonTabRemoved(tab);
                }

                bResut = true;
            }

            return bResut;
        }

        /// <summary>
        /// Clears the ribbon tabs group.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        private bool ClearRibbonTabsGroup(DependencyObject element, object item)
        {
            bool bResut = false;

            RibbonTabsGroup tabsGroup = element as RibbonTabsGroup;

            if (tabsGroup != null)
            {
                tabsGroup.TabsChanged -= new NotifyCollectionChangedEventHandler(this.OnTabsGroupTabsChanged);

                if (this.titlePanel != null)
                {
                    RibbonTabsGroupItem tabsGroupItem = tabsGroup.TabsGroupItem;

                    if (tabsGroupItem != null)
                    {
                        this.titlePanel.GroupItems.Remove(tabsGroupItem);

                        tabsGroupItem.TabsGroup = null;
                        tabsGroup.TabsGroupItem = null;
                    }
                }

                this.OnTabsCollectionChanged();

                bResut = true;
            }

            return bResut;
        }

        /// <summary>
        /// Raises the <see cref="E:ItemsAdded"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        private void OnItemsAdded(NotifyCollectionChangedEventArgs e)
        {
            foreach (object obj in e.NewItems)
            {
                if (!this.OnRibbonTabsGroupAdded(obj as RibbonTabsGroup))
                {
                    this.OnRibbonTabAdded(obj as RibbonTab);
                }
            }
        }

        /// <summary>
        /// Raises the <see cref="E:ItemsRemoved"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        private void OnItemsRemoved(NotifyCollectionChangedEventArgs e)
        {
            foreach (object obj in e.OldItems)
            {
                if (!this.OnRibbonTabsGroupRemoved(obj as RibbonTabsGroup))
                {
                    this.OnRibbonTabRemoved(obj as RibbonTab);
                }
            }
        }

        /// <summary>
        /// Called when [ribbon tabs group added].
        /// </summary>
        /// <param name="tabsGroup">The tabs group.</param>
        /// <returns></returns>
        private bool OnRibbonTabsGroupAdded(RibbonTabsGroup tabsGroup)
        {
            bool bResult = false;

            if (tabsGroup != null)
            {
                foreach (object obj in tabsGroup.Items)
                {
                    this.OnRibbonTabAdded(obj as RibbonTab);
                }

                bResult = true;
            }

            return bResult;
        }

        /// <summary>
        /// Called when [ribbon tab added].
        /// </summary>
        /// <param name="tab">The tab.</param>
        private void OnRibbonTabAdded(RibbonTab tab)
        {
            if (tab != null)
            {
                tab.IsSelectedChanged += new EventHandler(this.OnTabIsSelectedChanged);
                tab.ItemClicked += new EventHandler(this.OnItemClicked);

                if (tab.Visibility == Visibility.Visible && (tab.IsChecked || this.SelectedTabInternal == null))
                {
                    this.SelectedTabInternal = tab;
                }
            }
        }

        /// <summary>
        /// Called when [ribbon tabs group removed].
        /// </summary>
        /// <param name="tabsGroup">The tabs group.</param>
        /// <returns></returns>
        private bool OnRibbonTabsGroupRemoved(RibbonTabsGroup tabsGroup)
        {
            bool bResult = false;

            if (tabsGroup != null)
            {
                foreach (object obj in tabsGroup.Items)
                {
                    this.OnRibbonTabRemoved(obj as RibbonTab);
                }

                bResult = true;
            }

            return bResult;
        }

        /// <summary>
        /// Called when [ribbon tab removed].
        /// </summary>
        /// <param name="tab">The tab.</param>
        private void OnRibbonTabRemoved(RibbonTab tab)
        {
            if (tab != null)
            {
                tab.IsSelectedChanged -= new EventHandler(this.OnTabIsSelectedChanged);
                tab.ItemClicked -= new EventHandler(this.OnItemClicked);

                if (tab.IsChecked)
                {
                    List<RibbonTab> tabsList = this.GetTabs();

                    this.SelectedTabInternal = tabsList.Count > 0 ? tabsList[0] : null;
                }
            }
        }

        /// <summary>
        /// Selects the tab.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public bool SelectTab(int index)
        {
            var lstTab = this.GetTabs();
            if (lstTab.Count <= index) return false;
            var seltab = lstTab[index];
            if (seltab != null)
            {
                seltab.IsChecked = true;
                this.SelectedTabInternal = seltab;
                return true;
            }
            else return false;

        }

        /// <summary>
        /// Gets the tabs.
        /// </summary>
        /// <returns></returns>
        public List<RibbonTab> GetTabs()
        {
            List<RibbonTab> list = new List<RibbonTab>();

            foreach (object item in this.Items)
            {
                RibbonTabsGroup tabsGroup = item as RibbonTabsGroup;

                if (tabsGroup != null)
                {
                    foreach (RibbonTab tab in tabsGroup.Tabs)
                    {
                        if (tab.Visibility == Visibility.Visible)
                        {
                            list.Add(tab);
                            if (!tab.IsChecked)
                            {
                                tab.IsCollapsed = true;
                                tab.Visibility = Visibility.Collapsed;
                            }
                        }
                        else if (tab.IsCollapsed)
                        {
                            list.Add(tab);
                        }
                    }
                }
                else
                {
                    RibbonTab tab = item as RibbonTab;
                    if (tab == null)
                    {
                        if (this.tabs.ContainsKey(item))
                        {
                            list.Add(this.tabs[item]);
                        }
                    }
                    else
                    {
                        if (tab.Visibility == Visibility.Visible)
                        {
                            list.Add(tab);
                            if (!tab.IsChecked)
                            {
                                tab.IsCollapsed = true;
                                tab.Visibility = Visibility.Collapsed;
                            }
                        }
                        else if (tab.IsCollapsed)
                        {
                            list.Add(tab);
                        }
                    }
                }
            }

            return list;
        }

        #endregion

        #region IRibbonTabItemSelector implementation

        /// <summary>
        /// Called when [double click].
        /// </summary>
        /// <param name="item">The item.</param>
        void IRibbonTabItemSelector.OnDoubleClick(TabButton item)
        {
            ToggleRibbonState();
            this.popupTabHost.IsOpen = false;
        }

        private void ToggleRibbonState()
        {
            switch (this.RibbonState)
            {
                case RibbonState.Normal:
                    this.RibbonState = Controls.RibbonState.Hide;
                    break;
                case RibbonState.Hide:
                    this.RibbonState = Controls.RibbonState.Normal;
                    break;
                case RibbonState.Adorner:
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Called when [mouse down].
        /// </summary>
        /// <param name="item">The item.</param>
        void IRibbonTabItemSelector.OnMouseDown(TabButton item)
        {
            if (this.RibbonState == Controls.RibbonState.Hide && this.popupTabHost != null)
            {
                this.popupTabHost.SetSize(this.ActualWidth, double.NaN);

                this.popupTabHost.IsOpen = true;
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when <see cref="SelectedTabItem"/>  was changed
        /// </summary>
        public event EventHandler SelectedTabChanged;

        #endregion

        #region Fields
        private const string AddtoQATString = "Add to Quick Access Toolbar";
        private const string RemovefromQATString = "Remove from Quick Access Toolbar";
        private RibbonTabStrip tabStrip = null;
        private ItemsPresenter tabPresenter = null;
        private RibbonTitlePanel titlePanel = null;
        private ContentPresenter normalTabHost = null;
        private RibbonDropDown popupTabHost = null;
        //private RibbonItem showBelowRibbonItem = null;
        //private RibbonItem morecommands = null;
        //private RibbonItem minimizeRibbonItem = null;
        private RibbonTab selectedTab = null;
        private Dictionary<object, RibbonTab> tabs = null;
        private Border _title;

        /// <summary>
        /// Used store the QAT Bar elements at the Run time for State Persistence.
        /// </summary>
        internal readonly Dictionary<UIElement, UIElement> QATItems = new Dictionary<UIElement, UIElement>();

        #endregion

        #region State Persistence

        /// <summary>
        /// Saves the initial state.
        /// </summary>
        private void SaveInitialState()
        {
            SaveDefaultState(reset_StoreFile);
        }

        /// <summary>
        /// Saves the default state.
        /// </summary>
        /// <param name="storeFileName">Name of the store file.</param>
        private void SaveDefaultState(string storeFileName)
        {
            IsolatedStorageFile isoStorage = IsolatedStorageFile.GetUserStoreForApplication();

            StringBuilder builder = new StringBuilder();

            Dictionary<FrameworkElement, string> paths = new Dictionary<FrameworkElement, string>();

            if (this.QuickAccessToolBar != null && this.QuickAccessToolBar.AutoPersist)
            {
                TraverseAndFindLogicalTree(this, "", paths);

                foreach (var element in QATItems)
                {
                    if (paths.ContainsKey((FrameworkElement)element.Value))
                    {
                        builder.Append(paths[(FrameworkElement)element.Value]);
                        builder.Append(';');
                    }
                }
            }

            List<object> WindowStates = new List<object>();
            if (this.IsInRibbonWindow && this.Window.AutoPersist)
                GetWindowCoordinates(ref  WindowStates);

            RibbonStateParams param = new RibbonStateParams(builder.ToString(), IsQATBelow, RibbonState, this, RemovedStaticQATItems.ToString(), WindowStates);

            XmlSerializer xs = new XmlSerializer(typeof(RibbonStateParams));
            StringBuilder sb = new StringBuilder();
            XmlWriter writer = XmlWriter.Create(sb);
            xs.Serialize(writer, param);
            writer.Close();


            IsolatedStorageFileStream isoStream = new IsolatedStorageFileStream(@storeFileName, FileMode.Create, isoStorage);
            StreamWriter streamWriter = new StreamWriter(isoStream);
            streamWriter.Write(sb.ToString());
            streamWriter.Close();
            isoStream.Close();
            paths.Clear();
        }

        /// <summary>
        /// Saves the state of the ribbon.
        /// </summary>
        public void SaveRibbonState()
        {
            IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication();
            SaveRibbonState(storage, default_StoreFile);
        }

        /// <summary>
        /// Saves the state of the ribbon.
        /// </summary>
        /// <param name="isoStorage">The iso storage.</param>
        /// <param name="storeFileName">Name of the store file.</param>s
        public void SaveRibbonState(IsolatedStorageFile isoStorage, string storeFileName)
        {

            StringBuilder builder = new StringBuilder();

            Dictionary<FrameworkElement, string> paths = new Dictionary<FrameworkElement, string>();

            if (this.QuickAccessToolBar != null && this.PersistElements.Contains(RibbonElements.QuickAccessToolbar ))
            {
                TraverseAndFindLogicalTree(this, "", paths);

                foreach (var element in QATItems)
                {
                    if (paths.ContainsKey((FrameworkElement)element.Value))
                    {
                        builder.Append(paths[(FrameworkElement)element.Value]);
                        builder.Append(';');
                    }
                }
            }

            List <object> WindowStates=new List<object> ();
            if (this.IsInRibbonWindow && this.PersistElements.Contains(RibbonElements.RibbonWindow ))
                GetWindowCoordinates(ref  WindowStates);            

            RibbonStateParams param = new RibbonStateParams(builder.ToString(), IsQATBelow, RibbonState, this, RemovedStaticQATItems.ToString(), WindowStates);

            XmlSerializer xs = new XmlSerializer(typeof(RibbonStateParams));
            StringBuilder sb = new StringBuilder();
            XmlWriter writer = XmlWriter.Create(sb);
            xs.Serialize(writer, param);
            writer.Close();


            IsolatedStorageFileStream isoStream = new IsolatedStorageFileStream(@storeFileName, FileMode.Create, isoStorage);
            StreamWriter streamWriter = new StreamWriter(isoStream);
            streamWriter.Write(sb.ToString());
            streamWriter.Close();
            isoStream.Close();
        }

        /// <summary>
        /// Gets the window coordinates.
        /// </summary>
        /// <param name="WindowStates">The window states.</param>
        private void GetWindowCoordinates(ref List<object> WindowStates)
        {
            if (this.Window != null)
            {
                WindowStates.Add(this.Window.Top);
                WindowStates.Add(this.Window.Left);
                WindowStates.Add(this.Window.Height);
                WindowStates.Add(this.Window.Width);

                if (Window.WindowState == WindowState.Maximized)
                    WindowStates.Add("true");
                else
                    WindowStates.Add("false"); 
            }
        }

        /// <summary>
        /// Gets the ribbon window.
        /// </summary>
        /// <param name="startingfrom">The startingfrom.</param>
        /// <param name="ancestortype">The ancestortype.</param>
        /// <returns></returns>
        private DependencyObject GetRibbonWindow(DependencyObject startingfrom, Type ancestortype)
        {
            var item = VisualTreeHelper.GetParent(startingfrom);

            while (item != null && !(item.GetType() == ancestortype))
            {
                if (item is DependencyObject)
                {
                    item = VisualTreeHelper.GetParent(item);
                    if (item is RibbonWindow)
                        break;
                }
                else
                {
                    break;
                }
            }
            return item as DependencyObject;
        }

        /// <summary>
        /// Traverses the and find logical tree.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="path">The path.</param>
        /// <param name="paths">The paths.</param>
        private void TraverseAndFindLogicalTree(DependencyObject item, string path, IDictionary<FrameworkElement, string> paths)
        {
            FrameworkElement uielement = item as FrameworkElement;
            if (uielement != null && QATItems.ContainsValue(uielement))
            {
                if (!paths.ContainsKey(uielement))
                    paths.Add(uielement, path);
            }

            object[] child1 = FindChildren(uielement);

            for (int i = 0; i < child1.Length; i++)
            {
                DependencyObject child = child1[i] as DependencyObject;
                if (child == null) continue;
                TraverseAndFindLogicalTree(child, path + i + ",", paths);
            }
        }

        /// <summary>
        /// Finds the children.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        private object[] FindChildren(FrameworkElement item)
        {
            if (item is ItemsControl)
                return (item as ItemsControl).Items.ToArray();
            else
                return new object[0];
        }

        /// <summary>
        /// Loads the default state.
        /// </summary>
        /// <param name="loadFileName">Name of the load file.</param>
        private void LoadDefaultState(string loadFileName)
        {
            IsolatedStorageFile isoStorage = IsolatedStorageFile.GetUserStoreForApplication();
            
            if (FileExists(isoStorage, loadFileName))
            {
                IsolatedStorageFileStream isoStream = new IsolatedStorageFileStream(@loadFileName, FileMode.OpenOrCreate, isoStorage);

                StreamReader sr = new StreamReader(isoStream);

                string content = sr.ReadToEnd();

                isoStream.Close();

                if (content.Length > 0)
                {
                    XmlSerializer xs = new XmlSerializer(typeof(RibbonStateParams));
                    StringReader temp = new StringReader(content);
                    RibbonStateParams param = (RibbonStateParams)xs.Deserialize(temp);

                    string QATString = param.QATItemsString;

                    if (this.QuickAccessToolBar != null && this.QuickAccessToolBar.AutoPersist)
                    {
                        string[] items = QATString.ToString().Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                        QATItems.Clear();

                        this.QuickAccessToolBar.Items.Clear();
                        this.QuickAccessToolBar.QATItemCollections.Clear();

                        if (this.QuickAccessToolBar.PART_OverflowItemsControl != null)
                            this.QuickAccessToolBar.PART_OverflowItemsControl.Children.Clear();

                        int[] indices = param.RemovedQATStaticItemsIndices.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => Int32.Parse(x)).ToArray();

                        for (int index = 0; index < this.InitialStaticQATItems.Count; index++)
                        {
                            if (!indices.ToList().Contains(index))
                            {
                                UIElement target = this.InitialStaticQATItems.ElementAt(index);
                                this.QuickAccessToolBar.Items.Add(target);
                            }
                        }

                        //To Maintain the Removed Static QAT items
                        this.RemovedStaticQATItems = new StringBuilder();

                        foreach (var item in indices.ToList())
                            this.RemovedStaticQATItems.Append(item + ",");

                        PersistQATMenuItems(param);

                        for (int i = 0; i < items.Length; i++)
                            ParseAndAddToQAT(items[i]);

                        this.IsQATBelow = param.IsQATBelow;
                        if (IsQATBelow)
                            this.QuickAccessToolBar.HasGeometry = false;
                    }

                    if (this.AutoPersist)
                        this.RibbonState = param.RibbonState;

                    if (this.IsInRibbonWindow && this.Window.AutoPersist)
                        LoadWindowCoordinates(param);
                }
            }
        }


        /// <summary>
        /// Loads the state of the ribbon.
        /// </summary>
        public void LoadRibbonState()
        {
            IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication();
            LoadRibbonState(storage, default_StoreFile);
        }

        /// <summary>
        /// Loads the state of the ribbon.
        /// </summary>
        /// <param name="isoStorage">The iso storage.</param>
        /// <param name="loadFileName">Name of the load file.</param>
        public void LoadRibbonState(IsolatedStorageFile isoStorage, string loadFileName)
        {
            if (FileExists(isoStorage, loadFileName))
            {
                IsolatedStorageFileStream isoStream = new IsolatedStorageFileStream(@loadFileName, FileMode.OpenOrCreate, isoStorage);

                StreamReader sr = new StreamReader(isoStream);

                string content = sr.ReadToEnd();

                isoStream.Close();

                if (content.Length > 0)
                {
                    XmlSerializer xs = new XmlSerializer(typeof(RibbonStateParams));
                    StringReader temp = new StringReader(content);
                    RibbonStateParams param = (RibbonStateParams)xs.Deserialize(temp);

                    string QATString = param.QATItemsString;

                    if (this.QuickAccessToolBar != null && this.PersistElements.Contains(RibbonElements.QuickAccessToolbar))
                    {
                        string[] items = QATString.ToString().Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                        QATItems.Clear();

                        this.QuickAccessToolBar.Items.Clear();
                        this.QuickAccessToolBar.QATItemCollections.Clear();

                        if (this.QuickAccessToolBar.PART_OverflowItemsControl != null)
                            this.QuickAccessToolBar.PART_OverflowItemsControl.Children.Clear();

                        int[] indices = param.RemovedQATStaticItemsIndices.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => Int32.Parse(x)).ToArray();

                        for (int index = 0; index < this.InitialStaticQATItems.Count; index++)
                        {
                            if (!indices.ToList().Contains(index))
                            {
                                UIElement target = this.InitialStaticQATItems.ElementAt(index);
                                this.QuickAccessToolBar.Items.Add(target);
                            }
                        }

                        //To Maintain the Removed Static QAT items
                        this.RemovedStaticQATItems = new StringBuilder();

                        foreach (var item in indices.ToList())
                            this.RemovedStaticQATItems.Append(item + ",");

                        PersistQATMenuItems(param);

                        for (int i = 0; i < items.Length; i++)
                            ParseAndAddToQAT(items[i]);

                        this.IsQATBelow = param.IsQATBelow;
                        if (IsQATBelow)
                            this.QuickAccessToolBar.HasGeometry = false;
                    }

                    if (this.PersistElements.Contains(RibbonElements.Ribbon))
                        this.RibbonState = param.RibbonState;

                    if (this.IsInRibbonWindow && this.PersistElements.Contains(RibbonElements.RibbonWindow))
                        LoadWindowCoordinates(param);
                }
            }
        }

        /// <summary>
        /// Loads the window coordinates.
        /// </summary>
        /// <param name="param">The param.</param>
        private void LoadWindowCoordinates(RibbonStateParams param)
        {
            if (param.WindowCoordinates.Count > 0 && this.Window != null)
            {
                string IsMaximized = param.WindowCoordinates[4].ToString();

                 if (IsMaximized.Equals("true"))
                     this.Window.WindowState = WindowState.Maximized;
                 else
                 {
                     if (this.Window.WindowState == WindowState.Maximized)
                         this.Window.WindowState = WindowState.Normal;
                     this.Window.Top = (double)param.WindowCoordinates[0];
                     this.Window.Left = (double)param.WindowCoordinates[1];
                     this.Window.Height = (double)param.WindowCoordinates[2];
                     this.Window.Width = (double)param.WindowCoordinates[3];
                 }
            }
        }

        /// <summary>
        /// Persists the QAT menu items.
        /// </summary>
        /// <param name="param">The param.</param>
        private void PersistQATMenuItems(RibbonStateParams param)
        {
            int[] CheckedIndices = param.CheckedItemsIndex.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => Int32.Parse(x)).ToArray();

            for (int i = 0; i < this.SynchronizedCommands.Count; i++)
            {
                if (CheckedIndices.Contains(i))
                    ((RibbonCommandProvider)this.SynchronizedCommands[i]).IsSynchronizedwithQAT = true;
                else
                    ((RibbonCommandProvider)this.SynchronizedCommands[i]).IsSynchronizedwithQAT = false;
            }

            this.QuickAccessToolBar.SynchronizeQATItems();
        }

        /// <summary>
        /// Parses the and add to QAT.
        /// </summary>
        /// <param name="data">The data.</param>
        void ParseAndAddToQAT(string data)
        {
            int[] indices = data.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => Int32.Parse(x)).ToArray();

            DependencyObject current = this;
            for (int i = 0; i < indices.Length; i++)
            {
                object[] children = FindChildren((FrameworkElement)current);

                bool indexIsInvalid = children.Length <= indices[i];
                DependencyObject item = indexIsInvalid ? null : children[indices[i]] as DependencyObject;

                if (item == null)
                {
                    return;
                }
                current = item;
            }

            CurrentQATButton = (FrameworkElement)current;

            OnAddtoQATCommand();
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
            IsolatedStorageFile isoStorage = IsolatedStorageFile.GetUserStoreForApplication();
            DeleteRibbonState(isoStorage, default_StoreFile);           
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

        #endregion


        #region Modal Tabs

        /// <summary>
        /// Shows the modal tab.
        /// </summary>
        /// <param name="ribbonTabName">Name of the ribbon tab.</param>
        /// <returns></returns>
        public bool ShowModalTab(string ribbonTabName)
        {
            this.tempTabCollection.Clear();
            foreach (var item in this.Items)
            {
                this.tempTabCollection.Add(item);
            }
            this.tempSelectedItem = this.SelectedTabInternal;

            foreach (RibbonTab modalTab in this.ModalTabCollection)
                if (modalTab.Name.Equals(ribbonTabName))
                {
                    ShowModalTab(modalTab);
                    return true;
                }
            return false;
        }

        /// <summary>
        /// Shows the modal tab.
        /// </summary>
        /// <param name="modalTab">The modal tab.</param>
        private void ShowModalTab(RibbonTab modalTab)
        {
            this.Items.Clear();
            this.Items.Add(modalTab);
            modalTab.Visibility = Visibility.Visible;
            this.SelectedTabInternal = modalTab;
        }

        /// <summary>
        /// Closes the modal tabs.
        /// </summary>
        /// <returns></returns>
        public bool CloseModalTabs()
        {
            if (this.Items.Count == 1)
            {
                this.Items.Clear();
                foreach (var tab in this.tempTabCollection)
                    this.Items.Add(tab);
                (tempSelectedItem as RibbonTab).Visibility = Visibility.Visible;
                this.SelectedTabInternal = this.tempSelectedItem as RibbonTab;
                return true;
            }
            else
                return false;
        }

        #endregion

        /// <summary>
        /// Gets or sets the current QAT button.
        /// </summary>
        /// <value>The current QAT button.</value>
        internal FrameworkElement CurrentQATButton { get; set; }


        /// <summary>
        /// Instance of MenuItems in QAT Bar
        /// </summary>
        internal List<UIElement> MenuItemsInQAT = new List<UIElement>();

        /// <summary>
        /// Adds the QA tin context menu.
        /// </summary>
        /// <param name="realSource">The real source.</param>
        internal void AddQATinContextMenu(FrameworkElement realSource)
        {
            RemoveAddToQATMenuItemFromContextmenu();
            if (this.QuickAccessToolBar == null) return;

            if (realSource is IRibbonControl)
            {
                Syncfusion.Windows.Shared.ContextMenuItemAdv addQAt = new ContextMenuItemAdv ();
                //addQAt.Header = AddtoQATString;
                addQAt.Header = ResourceWrapperKeys.AddToQuickAccessToolbar;
                addQAt.Command = AddtoQATCommand;
                if (QuickAccessToolBar.CanAddQATItem(realSource) == false)
                {
                    addQAt.IsEnabled = false;
                    CurrentQATButton = null;
                }
                else
                {
                    //QuickAccessToolBar.QATClonedItems.Add(realSource);
                    CurrentQATButton = realSource;
                }
                if (CurrentQATButton is ButtonPanel || CurrentQATButton is RibbonGallery || CurrentQATButton is RibbonGalleryItem)
                {
                    CurrentQATButton = null;
                    return;
                }
                this.RibbonContextMenu.Items.Insert(0, addQAt);
            }
        }

        /// <summary>
        /// Removes the QA tin context menu.
        /// </summary>
        /// <param name="realSource">The real source.</param>
        internal void RemoveQATinContextMenu(FrameworkElement realSource)
        {
            RemoveAddToQATMenuItemFromContextmenu();
            if (this.QuickAccessToolBar == null) return;

            if (realSource is IRibbonControl)
            {
                Syncfusion.Windows.Shared.ContextMenuItemAdv addQAt = new ContextMenuItemAdv();
                //addQAt.Header = RemovefromQATString;
                addQAt.Header = ResourceWrapperKeys.RemoveFromQuickAccessToolbar;
                addQAt.Command = RemoveFromQATCommand;
                CurrentQATButton = realSource;
                if (CurrentQATButton is ButtonPanel || CurrentQATButton is RibbonGallery || CurrentQATButton is RibbonGalleryItem)
                {
                    CurrentQATButton = null;
                    return;
                }
                this.RibbonContextMenu.Items.Insert(0, addQAt);
            }
        }

        /// <summary>
        /// Removes the add to QAT menu item from contextmenu.
        /// </summary>
        private void RemoveAddToQATMenuItemFromContextmenu()
        {
            if (this.RibbonContextMenu == null) return;
            if (this.RibbonContextMenu.Items.Count > 0)
            {
                var itmcoll = this.RibbonContextMenu.Items.OfType<ContextMenuItemAdv>();
                if (itmcoll.Count() > 0 && (itmcoll.FirstOrDefault().Header.ToString() == ResourceWrapperKeys.AddToQuickAccessToolbar || itmcoll.FirstOrDefault().Header.ToString() == ResourceWrapperKeys.RemoveFromQuickAccessToolbar))
                    this.RibbonContextMenu.Items.RemoveAt(0);
            }
        }


        private RibbonCommand addtoQATCommand;

        /// <summary>
        /// Gets the addto QAT command.
        /// </summary>
        /// <value>The addto QAT command.</value>
        public RibbonCommand AddtoQATCommand
        {
            get
            {
                if (addtoQATCommand == null)
                {
                    addtoQATCommand = new RibbonCommand(outputParam => OnAddtoQATCommand());
                }
                return addtoQATCommand;
            }
        }


        private RibbonCommand removeFromQATCommand;
        /// <summary>
        /// Gets the remove from QAT command.
        /// </summary>
        /// <value>The remove from QAT command.</value>
        public RibbonCommand RemoveFromQATCommand
        {
            get
            {
                if (removeFromQATCommand == null)
                {
                    removeFromQATCommand = new RibbonCommand(outputParam => OnRemoveFromQATCommand());
                }
                return removeFromQATCommand;
            }
        }

        /// <summary>
        /// Called when [remove from QAT command].
        /// </summary>
        private void OnRemoveFromQATCommand()
        {
            if (CurrentQATButton != null)
            {
                if (this.QuickAccessToolBar != null)
                {
                    var remEle = from res in this.QuickAccessToolBar.QATItemCollections
                                 where res.Value == CurrentQATButton
                                 select res;
                    if (remEle.Count() > 0)
                    {
                        this.QuickAccessToolBar.QATItemCollections.Remove(remEle.FirstOrDefault().Key);
                    }
                }

                if (this.QuickAccessToolBar.Items.IndexOf(CurrentQATButton) != -1)
                {

                    if (this.QATItems.ContainsKey((UIElement)CurrentQATButton))
                        this.QATItems.Remove((UIElement)CurrentQATButton);
                    else if (this.MenuItemsInQAT.Contains(CurrentQATButton))
                        this.MenuItemsInQAT.Remove(CurrentQATButton);
                    else
                        RemovedStaticQATItems.Append(this.InitialStaticQATItems.IndexOf(CurrentQATButton).ToString() + ',');

                    this.QuickAccessToolBar.Items.Remove(CurrentQATButton);
                }

                CurrentQATButton = null;
            }
        }

        /// <summary>
        /// Called when [addto QAT command].
        /// </summary>
        private void OnAddtoQATCommand()
        {
            if (CurrentQATButton != null)
            {
                UIElement newQAtItem = CloneManager.CloneGeneral(CurrentQATButton, false);
                if (QuickAccessToolBar != null && QuickAccessToolBar.CanAddQATItem(CurrentQATButton) && QuickAccessToolBar.CanAddQATItem(newQAtItem))
                {
                    CurrentQATButton.Tag = CurrentQATButton.GetHashCode();
                    if (!QuickAccessToolBar.QATItemCollections.ContainsKey(CurrentQATButton))
                        QuickAccessToolBar.QATItemCollections.Add(CurrentQATButton, newQAtItem);
                }

                if (newQAtItem != null && QuickAccessToolBar != null && QuickAccessToolBar.Items.Contains(newQAtItem) == false)
                {
                    QuickAccessToolBar.Items.Add(newQAtItem);

                    if (!QATItems.ContainsKey(newQAtItem))
                        this.QATItems.Add(newQAtItem, (UIElement)CurrentQATButton);
                }
                CurrentQATButton = null;
            }
        }


        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (this.Window != null)
                this.Window.ribbon = null;

            Application.Current.Exit -= new EventHandler(Current_Exit);
            if (_morecommands != null)
                _morecommands.Click -= new RoutedEventHandler(QuickAccessToolBar_OnMoreCommandsClick);

            if (_showbelow != null)
                _showbelow.Click -= new RoutedEventHandler(QuickAccessToolBar_OnShowBelowtheRibbonClick);

            if (_minimizeribbon != null)
                _minimizeribbon.Click -= new RoutedEventHandler(QuickAccessToolBar_OnMinimizeRibbonClick);

            if (_ribbonContextMenu != null)
            {
                _ribbonContextMenu.Opened -= new RoutedEventHandler(_ribbonContextMenu_Opened);
                _ribbonContextMenu.Closed -= new RoutedEventHandler(_ribbonContextMenu_Closed);
            }
            if (this.popupTabHost != null)
            {
                this.popupTabHost.IsOpenChanged -= new System.EventHandler(this.IsOpenChangedHandler);
                this.popupTabHost.OutsideRectMouseDown -= new MouseButtonEventHandler(this.MouseDownHandler);
            }

            if (this.ribbonToggleButton != null)
                this.ribbonToggleButton.Click -= new RoutedEventHandler(ribbonToggleButton_Click);
           if(this.ModalTabCollection != null)
            this.ModalTabCollection.Clear();
            this.ClearValue(Ribbon.ModalTabCollectionProperty);
            this.ModalTabCollection = null;
            this._showbelow = null;

            this._minimizeribbon = null;
            this._morecommands = null;
            this.normalTabHost = null;
            this.tempSelectedItem = null;
            if (this.titlePanel != null && this.titlePanel.TempQATToolBar != null)
            {
                this.titlePanel.TempQATToolBar.QATItemCollections.Clear();
                this.titlePanel.TempQATToolBar.Items.Clear();
                this.titlePanel.TempQATToolBar = null;

            }
            if (this._title != null)
            {
                this._title.Child = null;
                this._title = null;
            }
            this.titlePanel = null;
            this.Window = null;
            if (this.RibbonContextMenu != null)
            {
                this.RibbonContextMenu.Items.Clear();
                this.RibbonContextMenu.Owner = null;
            }
            if (this._ribbonContextMenu != null)
            {
                this._ribbonContextMenu.Items.Clear();
                this._ribbonContextMenu.Owner = null;
            }
            if (this.QuickAccessToolBar != null)
                this.QuickAccessToolBar.Dispose();
           
            this._ribbonContextMenu = null;
            this.MenuItemsInQAT.Clear();
            this.QATItems.Clear();
            this.ToolBarMenu = null;
            this.popupTabHost = null;
            this.tabStrip.Items.Clear();
            this.tabStrip = null;
            this.tabPresenter = null;
            this.ResourceWrapperKeys = null;
            if (selectedTab != null)
            {
                foreach (RibbonBar rbar in selectedTab.Items)
                {
                    rbar.Dispose();
                    rbar.Items.Clear();
                }
                this.selectedTab.Items.Clear();
                if (this.selectedTab.panel != null)
                {
                    this.selectedTab.panel.Children.Clear();
                    this.selectedTab.panel = null;
                }
                this.selectedTab = null;
            }
            
            this.tabs.Clear();

            if (this.QATCustomizationDialog != null)
            {
                this.QATCustomizationDialog.AddedItems.Clear();
                this.QATCustomizationDialog.RemovedItems.Clear();
            }
            if (this._qatdialog != null)
            {
                this._qatdialog._source.Clear();
                this._qatdialog._destination.Clear();
                this._qatdialog.QATItemCollections.Clear();
                this._qatdialog.PART_btnCancel = null;
                this._qatdialog.PART_btnDown = null;
                this._qatdialog.PART_btnRemove = null;
                this._qatdialog.PART_btnReset = null;
                this._qatdialog.PART_btnUp = null;
                this._qatdialog.AddedItems.Clear();
                this._qatdialog.RemovedItems.Clear();
                this._qatdialog = null;
            }
            this._qatdialog = null;
            if (PersistElements != null)
            {
                this.PersistElements.Clear();
                this.PersistElements = null;
            }
            if (this.persistElements != null)
            {
                this.persistElements.Clear();
                this.persistElements = null;
            }
            //this.Loaded -= new RoutedEventHandler(Ribbon_Loaded);
            //this.Unloaded -= new RoutedEventHandler(Ribbon_Unloaded);
            this.SizeChanged -= new SizeChangedEventHandler(this.SizeChangedHandler);

            if (this.PART_QATAboveContainer != null)
            {
                this.PART_QATAboveContainer.Children.Clear();
                this.PART_QATAboveContainer = null;
            }
            if (this.PART_QATBelowContainer != null)
            {
                this.PART_QATBelowContainer.Children.Clear();
                this.PART_QATBelowContainer = null;
            }
            if (this.Part_QATBorderContainer != null)
            {
                this.Part_QATBorderContainer.Child = null;
                this.Part_QATBorderContainer = null;
            }
            if (this.BackStageButton != null)
            {
                BackStageButton.Wrapper = null;
                this.BackStageButton.parentRibbon = null;
                this.BackStageButton = null;
            }
            if (this.BackStage != null)
            {
                this.BackStage.Items.Clear();
                this.BackStage = null;
            }
            if (this.QuickAccessToolBar != null)
            {
                QuickAccessToolBar.Dispose();
                QuickAccessToolBar.QATItemCollections.Clear();
                QuickAccessToolBar.PART_NormalContainer.Children.Clear();
                QuickAccessToolBar.PART_OverFlowContainer.Children.Clear();
                QuickAccessToolBar.PART_OverflowItemsControl.Children.Clear();
                QuickAccessToolBar.PART_OverFlowTopContainer.Children.Clear();
                QuickAccessToolBar.PART_OverflowRibbonDropDown = null;
                QuickAccessToolBar.PART_OverFlowToggleButton = null;
                QuickAccessToolBar.PART_QATDropDownControl = null;
                QuickAccessToolBar.PART_RibbonDropDown = null;
                QuickAccessToolBar._parentRibbon = null;
                this.QuickAccessToolBar.Items.Clear();
                ContentControl ctrl = this.QuickAccessToolBar.Parent as ContentControl;
                if (ctrl != null)
                    ctrl.Content = null;
                this.QuickAccessToolBar = null;
            }
            this.InitialStaticQATItems.Clear();
            foreach (RibbonTab rtab in Items)
            {
                foreach (RibbonBar rbar in rtab.Items)
                {
                    rbar.Dispose();
                    rbar.Items.Clear();
                }
                rtab.Items.Clear();
                if (rtab.panel != null)
                {
                    rtab.panel.Children.Clear();
                    rtab.panel = null;
                }
            }
            this.Items.Clear();
        }
    }
    #endregion

    /// <summary>
    /// 
    /// </summary>
    public enum RibbonElements
    {
        /// <summary>
        /// 
        /// </summary>
        Ribbon,
        /// <summary>
        /// 
        /// </summary>
        QuickAccessToolbar,
        /// <summary>
        /// 
        /// </summary>
        RibbonWindow
    }
}