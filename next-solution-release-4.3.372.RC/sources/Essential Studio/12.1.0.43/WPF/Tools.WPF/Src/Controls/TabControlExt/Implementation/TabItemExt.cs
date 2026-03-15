// <copyright file="TabItemExt.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using Syncfusion.Licensing;
using System.Windows.Input;
using System;
using Syncfusion.Windows.Shared;
using System.Windows.Controls.Primitives;
using System.ComponentModel;
using System.Windows.Interop;
using System.Windows.Documents;
using Syncfusion.Windows.Tools.Controls.Resources;
using System.Collections.Generic;
using System.Reflection;
using System.Diagnostics;

namespace Syncfusion.Windows.Tools.Controls
{

    
    /// <summary>
    /// The TabItemExt can be used to include various tabbed elements in to the
    /// TabControlExt. 
    /// </summary>
    /// <example>
    /// <code lang="XAML">
    /// <Window x:Class="TabControlExt.Window1"
    ///     xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    ///     xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    ///     Title="Window1" Height="300" Width="300"
    /// xmlns:syncfusion="http://schemas.syncfusion.com/wpf">
    ///     <Grid>
    ///         <!-- Adding TabcontrolExt -->
    ///         <syncfusion:TabControlExt Name="tabControlExt">
    ///           <!-- Adding TabItemExt -->
    ///            <syncfusion:TabItemExt Name="tabItemExt1" Header="TabItemExt"/>
    ///        </syncfusion:TabControlExt>
    ///     </Grid>
    /// </Window>
    /// </code>
    /// <code lang="C#">
    /// using System;
    /// using System.Collections.Generic;
    /// using System.Linq;
    /// using System.Text;
    /// using System.Windows;
    /// using System.Windows.Controls;
    /// using System.Windows.Data;
    /// using System.Windows.Documents;
    /// using System.Windows.Input;
    /// using System.Windows.Media;
    /// using System.Windows.Media.Imaging;
    /// using System.Windows.Navigation;
    /// using System.Windows.Shapes;
    /// using Syncfusion.Windows.Tools.Controls;
    /// namespace TabControlExt
    /// {
    ///     /// <summary>
    ///     /// Interaction logic for Window1.xaml
    ///     /// </summary>
    ///     public partial class Window1 : Window
    ///     {
    ///         public Window1()
    ///         {
    ///             InitializeComponent();
    ///             // Creating instance of the TabControlExt control
    ///             TabControlExt tabControlExt = new TabControlExt();
    ///             // Creating the instance of StackPanel
    ///             StackPanel stackPanel = new StackPanel();
    ///             // Creating instance of the TabItemExt 
    ///             TabItemExt tabItemExt1 = new TabItemExt();
    ///             // Setting header of the TabItemExt
    ///             tabItemExt1.Header = "TabItemExt";
    ///             // Adding tabitemext to tabcontrolext
    ///             tabControlExt.Items.Add(tabItemExt1);
    ///             // Adding control to the stackpanel
    ///             stackPanel.Children.Add(tabControlExt);
    ///         }
    ///     }
    /// }
    /// </code>
    /// </example>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif

    [SkinType(SkinVisualStyle = Skin.Office2007Blue,
   Type = typeof(TabItemExt), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/TabControlExt/Themes/Office2007BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Black,
    Type = typeof(TabItemExt), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/TabControlExt/Themes/Office2007BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Silver,
    Type = typeof(TabItemExt), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/TabControlExt/Themes/Office2007SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Blue,
  Type = typeof(TabItemExt), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/TabControlExt/Themes/Office2010BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Black,
    Type = typeof(TabItemExt), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/TabControlExt/Themes/Office2010BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Silver,
    Type = typeof(TabItemExt), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/TabControlExt/Themes/Office2010SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2003,
    Type = typeof(TabItemExt), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/TabControlExt/Themes/Office2003Style.xaml")]
    [SkinType(SkinVisualStyle = Skin.Blend,
    Type = typeof(TabItemExt), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/TabControlExt/Themes/BlendStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.SyncOrange,
    Type = typeof(TabItemExt), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/TabControlExt/Themes/SyncOrangeStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.ShinyRed,
    Type = typeof(TabItemExt), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/TabControlExt/Themes/ShinyRedStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.ShinyBlue,
    Type = typeof(TabItemExt), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/TabControlExt/Themes/ShinyBlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Default,
    Type = typeof(TabItemExt), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/TabControlExt/Themes/generic.xaml")]
    [SkinType(SkinVisualStyle = Skin.VS2010,
   Type = typeof(TabItemExt), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/TabControlExt/Themes/VS2010Style.xaml")]
    [SkinType(SkinVisualStyle = Skin.Metro,
   Type = typeof(TabItemExt), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/TabControlExt/Themes/MetroStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Transparent ,
   Type = typeof(TabItemExt), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/TabControlExt/Themes/TransparentStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2013,
   Type = typeof(TabItemExt), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/TabControlExt/Themes/Office2013Style.xaml")]
    public class TabItemExt : TabItem
    {
        /// <summary>
        /// Represents whether custom menu items enabled or not
        /// </summary>
        internal bool m_customTabItemEnabled = false;

        /// <summary>
        /// Represents header element
        /// </summary>
        internal ContentPresenter m_headerelement = null;

        /// <summary>
        /// Represents default header margin
        /// </summary>
        internal Thickness m_defaultHeaderMargin = new Thickness(0, 0, 0, 0);

        #region Initialization
        /// <summary>
        /// Initializes static members of the <see cref="TabItemExt"/> class.
        /// </summary>
        static TabItemExt()
        {
            FlowDirectionProperty.OverrideMetadata(typeof(TabItemExt), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnFlowDirectionChanged)));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TabItemExt), new FrameworkPropertyMetadata(typeof(TabItemExt)));
            ContentProperty.OverrideMetadata(typeof(TabItemExt), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnContentChanged)));
        }

        List<object> m_TabItemExtContent = new List<object>();
        bool isSelected = false;

        
        /// <summary>
        /// Initializes a new instance of the <see cref="TabItemExt"/> class.
        /// </summary>
        public TabItemExt()
        {
            if (EnvironmentTestTools.IsSecurityGranted)
            {
                EnvironmentTestTools.StartValidateLicense(typeof(TabItemExt));
            }
            this.Loaded += new RoutedEventHandler(TabItemExt_Loaded);
#if !SyncfusionFramework3_5
            //this.TouchMove+=TabItemExt_TouchMove;
            //IsManipulationEnabled = true;
            //AddHandler(UIElement.TouchMoveEvent, new EventHandler<TouchEventArgs>(TabItemExt_TouchMove), true);
#endif
        }

#if !SyncfusionFramework3_5
        void TabItemExt_TouchMove(object sender, TouchEventArgs e)
        {
            
        }
#endif

        private static void OnContentChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            TabItemExt tabItemExt = (sender as TabItemExt);
            if ((tabItemExt.Parent != null && (tabItemExt.Parent as TabControlExt != null)))
            {
                FrameworkElement content = tabItemExt.Content as FrameworkElement;
                if (content != null)
                {
                    content.Loaded += content_Loaded;
                    BindingUtils.SetBinding(content, tabItemExt.Parent as TabControlExt, FrameworkElement.FlowDirectionProperty, TabControlExt.FlowDirectionProperty);
                }
            }
            if ((tabItemExt.Parent != null && (tabItemExt.Parent as TabControlExt != null)) && (tabItemExt.Parent as TabControlExt).IsLazyLoaded)
            {
                tabItemExt.m_TabItemExtContent.Add(tabItemExt.Content);
                if (!tabItemExt.isSelected)
                    tabItemExt.Content = null;
            }
            if (args.NewValue is ContentPresenter)
            {
                UIElement element = (args.NewValue as ContentPresenter).Content as UIElement;
                DockingManager dockingmgr = DockingManager.ResolveManager(element);
                if (dockingmgr != null && dockingmgr.IsLazyLoaded )
                {
                    DockingManager.SetIsLogicalChild(element,true);                   
                }

                DocumentContainer doccontainer = DocumentContainer.GetDocumentContainer(element);
                if (doccontainer != null && !doccontainer.IsInDockingManager && doccontainer.IsLazyLoaded)
                {
                    FrameworkElement ele = element as FrameworkElement;
                    ele.Loaded += new RoutedEventHandler(TabItemExtContent_Loaded);                    
                }
            }
            if (tabItemExt.TabControlParent != null && tabItemExt.TabControlParent.DisplayMemberPath != string.Empty)
            {
                PropertyDescriptor propDescriptor = tabItemExt.TabControlParent.DisplayMemberPath != null ? TypeDescriptor.GetProperties(tabItemExt.Content)[tabItemExt.TabControlParent.DisplayMemberPath] : null;
                if (propDescriptor != null && tabItemExt != null)
                    tabItemExt.Header = propDescriptor.GetValue(tabItemExt.Content);
            }
        }

        static void content_Loaded(object sender, RoutedEventArgs e)
        {
            FrameworkElement element = sender as FrameworkElement;
            if (element != null)
            {
                TabControlExt tabcontrol = VisualUtils.FindAncestor(element as Visual, typeof(TabControlExt)) as TabControlExt;
                if (tabcontrol != null)
                {
                    foreach (var item in tabcontrol.Items)
                    {
                        TabItemExt tabItem = item as TabItemExt;
                        if (tabItem != null && tabItem.Content == sender)
                        {
                            element.InvalidateProperty(FlowDirectionProperty);
                        }
                    }
                }
            }
        }

        private static void OnTabIndexChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            TabItemExt item = obj as TabItemExt;
            item.OnTabIndexChanged(e);
        }

        private void OnTabIndexChanged(DependencyPropertyChangedEventArgs e)
        {
            int value = (int)e.NewValue;
            if (value > -1)
            {
                TabControlExt tabcontrol = (TabControlExt)Parent;
                
                if (tabcontrol != null && tabcontrol.m_loaded)
                {
                    int actualindex = tabcontrol.ItemContainerGenerator.IndexFromContainer(this);
                    if (actualindex != value)
                    {
                        tabcontrol.UpdateIndex(value, this);
                        tabcontrol.m_loaded = false;
                        tabcontrol.SetIndex();
                        tabcontrol.m_loaded = true;
                    }
                }
            }
        }

        protected override void OnSelected(RoutedEventArgs e)
        {
            if ((this.Parent != null && (this.Parent as TabControlExt != null)) && (this.Parent as TabControlExt).IsLazyLoaded)
            {
                this.isSelected = true;
                if(m_TabItemExtContent.Count>0)
                this.Content = m_TabItemExtContent[0];

            }
            base.OnSelected(e);
        }
        /// <summary>
        /// Handles the Loaded event of the TabItemExt control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void TabItemExt_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= new RoutedEventHandler(TabItemExt_Loaded);
             ResourceDictionary dictionary = new ResourceDictionary
                        {
                            Source = new Uri(
                                "pack://application:,,,/Syncfusion.Tools.WPF;component/Controls/TabControlExt/Themes/aero.normalcolor.xaml",
                                UriKind.RelativeOrAbsolute)
                        };

             if (CheckIsWindowsXP() && SkinStorage.GetVisualStyle(this) == "Default")
             {
                 RemoveDictionaryIfExist(this, dictionary);
                 this.Resources.MergedDictionaries.Add(dictionary);
             }
           
            #region CloseButtonVisibilityChecking

            if (this.Content != null)
            {
                ContentPresenter presenter = this.Content as ContentPresenter;
                if (presenter != null && presenter.Content != null)
                {
                    ToggleButton closebutton = GetTemplateChild("PART_CloseButton") as ToggleButton;
                    if (closebutton != null)
                    {
                        DockingManager owner = DockingManager.ResolveManager(presenter.Content as UIElement);
                        if (owner != null)
                        {
                            bool close = DockingManager.GetCanClose(presenter.Content as DependencyObject);
                            CheckCloseButtonVisibilityOnLoad(closebutton, close, owner.DisabledCloseButtonsBehavior);
                        }
                        else
                        {
                            DocumentContainer container = VisualUtils.FindAncestor((Visual)this, typeof(DocumentContainer)) as DocumentContainer;
                            if (container != null)
                            {
                                bool close = DocumentContainer.GetCanClose(presenter.Content as DependencyObject);
                                CheckCloseButtonVisibilityOnLoad(closebutton, close, container.DisabledButtonsBehavior);
                            }
                        }
                    }
                }
            }

            #endregion
            if (this.HeaderTemplate == null && BindingOperations.GetBinding(this, HeaderedContentControl.HeaderTemplateProperty)==null)
            {
                this.HeaderTemplate = dictionary["HeaderDataTemplate"] as DataTemplate;
            }
        }

        /// <summary>
        /// Checks the is windows XP.
        /// </summary>
        /// <returns></returns>
        internal bool CheckIsWindowsXP()
        {
            System.OperatingSystem osInfo = System.Environment.OSVersion;

            switch (osInfo.Platform)
            {
                case System.PlatformID.Win32NT:
                    switch (osInfo.Version.Major)
                    {
                        case 5:
                            if (osInfo.Version.Minor == 0)
                                return false;
                            else
                                return true;
                    }
                    break;
            }
            return false;
        }

        /// <summary>
        /// RemoveDictionary If Exist
        /// </summary>
        /// <param name="element"></param>
        /// <param name="dictionary"></param>
        private static void RemoveDictionaryIfExist(FrameworkElement element, ResourceDictionary dictionary)
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
        private static void TabItemExtContent_Loaded(object sender, RoutedEventArgs e)
        {
            FrameworkElement element = sender as FrameworkElement;
            if (element != null)
            {
                element.Loaded -= new RoutedEventHandler(TabItemExtContent_Loaded);
                DocumentContainer.SetIsLogicalChild(element, true);
            }
        }
        /// <summary>
        /// Raises the <see cref="E:System.Windows.FrameworkElement.Initialized"/> event. This method is invoked whenever <see cref="P:System.Windows.FrameworkElement.IsInitialized"/> is set to true internally.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.RoutedEventArgs"/> that contains the event data.</param>
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                Binding imageBinding = new Binding();
                Binding imageHeightBinding = new Binding();
                Binding imageWidthBinding = new Binding();
                Binding allignmentBinding = new Binding();
                Binding hoverBackgroundBinding = new Binding();
                Binding customTemplateBinding = new Binding();
                Binding useCustomTemplateBinding = new Binding();
                imageBinding.Mode = BindingMode.TwoWay;
                imageHeightBinding.Mode = BindingMode.TwoWay;
                imageWidthBinding.Mode = BindingMode.TwoWay;
                allignmentBinding.Mode = BindingMode.TwoWay;
                hoverBackgroundBinding.Mode = BindingMode.TwoWay;
                customTemplateBinding.Mode = BindingMode.TwoWay;
                useCustomTemplateBinding.Mode = BindingMode.TwoWay;
                imageBinding.Source = this;
                imageHeightBinding.Source = this;
                imageWidthBinding.Source = this;
                allignmentBinding.Source = this;
                hoverBackgroundBinding.Source = this;
                customTemplateBinding.Source = this;
                useCustomTemplateBinding.Source = this;
                imageBinding.Path = new PropertyPath(ImageProperty);
                imageHeightBinding.Path = new PropertyPath(ImageHeightProperty);
                imageWidthBinding.Path = new PropertyPath(ImageWidthProperty);
                allignmentBinding.Path = new PropertyPath(ImageAlignmentProperty);
                hoverBackgroundBinding.Path = new PropertyPath(HoverBackgroundProperty);
                customTemplateBinding.Path = new PropertyPath(CustomEditableTemplateProperty);
                useCustomTemplateBinding.Path = new PropertyPath(UseCustomEditableTemplateProperty);
                SetBinding(TabControlExt.ImageProperty, imageBinding);
                SetBinding(TabControlExt.ImageHeightProperty, imageHeightBinding);
                SetBinding(TabControlExt.ImageWidthProperty, imageWidthBinding);
                SetBinding(TabControlExt.ImageAlignmentProperty, allignmentBinding);
                SetBinding(TabControlExt.HoverBackgroundProperty, hoverBackgroundBinding);
                SetBinding(TabControlExt.CustomEditableTemplateProperty, customTemplateBinding);
                SetBinding(TabControlExt.UseCustomEditableTemplateProperty, useCustomTemplateBinding);
            }
        }

        /// <summary>
        /// Sets the Visibility of Border
        /// </summary>
        /// <param name="visibility"></param>
        internal void SetTemplate(Visibility visibility)
        {
            if (this.Template != null)
            {
                FrameworkElement bd = this.Template.FindName("Bd", this) as FrameworkElement;

                if (bd != null)
                {
                    bd.Visibility = visibility;
                }
            }
        }

        
        #endregion

        #region Public properties

        /// <summary>
        /// Gets the tab control parent.
        /// </summary>
        /// <value>The tab control parent.</value>
        internal TabControlExt TabControlParent
        {
            get
            {
                return TabControlExt.ItemsControlFromItemContainer(this) as TabControlExt;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether UseCustomEditableTemplate is true.
        /// </summary>
        /// <value>
        ///     <c>true</c> if [use custom editable template]; otherwise, <c>false</c>.
        /// </value>
        public bool UseCustomEditableTemplate
        {
            get
            {
                return (bool)GetValue(UseCustomEditableTemplateProperty);
            }

            set
            {
#if !SyncfusionFramework3_5
                SetCurrentValue(UseCustomEditableTemplateProperty, value);
#else
                SetValue(UseCustomEditableTemplateProperty, value);
#endif
            }
        }

        /// <summary>
        /// Gets or sets the value of the CustomEditableTemplate dependency property.
        /// </summary>
        /// <value>The custom editable template.</value>
        public DataTemplate CustomEditableTemplate
        {
            get
            {
                return (DataTemplate)GetValue(CustomEditableTemplateProperty);
            }

            set
            {
#if !SyncfusionFramework3_5
                SetCurrentValue(CustomEditableTemplateProperty, value);
#else
                SetValue(CustomEditableTemplateProperty, value);
#endif
            }
        }

        /// <summary>
        /// Gets or sets the value of the TabItemContextMenuItemTemplate dependency property.
        /// </summary>
        public DataTemplate TabItemContextMenuItemTemplate
        {
            get
            {
                return (DataTemplate)GetValue(TabItemContextMenuItemTemplateProperty);
            }

            set
            {
#if !SyncfusionFramework3_5
                SetCurrentValue(TabItemContextMenuItemTemplateProperty, value);
#else
                SetValue(TabItemContextMenuItemTemplateProperty, value);
#endif
            }
        }

        /// <summary>
        /// Gets or sets the tab list context menu style.
        /// </summary>
        /// <value>The tab list context menu style.</value>
        public Style TabItemContextMenuStyle
        {
            get
            {
                return (Style)GetValue(TabItemContextMenuStyleProperty);
            }
            set
            {
#if !SyncfusionFramework3_5
                SetCurrentValue(TabItemContextMenuStyleProperty, value);
#else
                SetValue(TabItemContextMenuStyleProperty, value);
#endif
            }
        }

        /// <summary>
        /// Gets or sets the tab list context menu template.
        /// </summary>
        /// <value>The tab list context menu template.</value>
        public ControlTemplate TabItemContextMenuTemplate
        {
            get
            {
                return (ControlTemplate)GetValue(TabItemContextMenuTemplateProperty);
            }
            set
            {
#if !SyncfusionFramework3_5
                SetCurrentValue(TabItemContextMenuTemplateProperty, value);
#else
                SetValue(TabItemContextMenuTemplateProperty, value);
#endif
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [enable close menu item].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [enable close menu item]; otherwise, <c>false</c>.
        /// </value>
        internal bool EnableCloseMenuItem
        {
            get
            {
                return (bool)GetValue(EnableCloseMenuItemProperty);
            }
            set
            {
#if !SyncfusionFramework3_5
                SetCurrentValue(EnableCloseMenuItemProperty, value);
#else
                SetValue(EnableCloseMenuItemProperty, value);
#endif

            }
        }

        /// <summary>
        /// Gets or sets the close menu item visibility.
        /// </summary>
        /// <value>The close menu item visibility.</value>
        internal Visibility CloseMenuItemVisibility
        {
            get
            {
                return (Visibility)GetValue(CloseMenuItemVisibilityProperty);
            }
            set
            {
#if !SyncfusionFramework3_5
                SetCurrentValue(CloseMenuItemVisibilityProperty, value);
#else
                SetValue(CloseMenuItemVisibilityProperty, value);
#endif
            }
        }

        /// <summary>
        /// Gets or sets the close all menu item visibility.
        /// </summary>
        /// <value>The close all menu item visibility.</value>
        internal Visibility CloseAllMenuItemVisibility
        {
            get
            {
                return (Visibility)GetValue(CloseAllMenuItemVisibilityProperty);
            }
            set
            {
#if !SyncfusionFramework3_5
                SetCurrentValue(CloseAllMenuItemVisibilityProperty, value);
#else
                SetValue(CloseAllMenuItemVisibilityProperty, value);
#endif
            }
        }

        /// <summary>
        /// Gets or sets the close all but this menu item visibility.
        /// </summary>
        /// <value>The close all but this menu item visibility.</value>
        internal Visibility CloseAllButThisMenuItemVisibility
        {
            get
            {
                return (Visibility)GetValue(CloseAllButThisMenuItemVisibilityProperty);
            }
            set
            {
#if !SyncfusionFramework3_5
                SetCurrentValue(CloseAllButThisMenuItemVisibilityProperty, value);
#else
                SetValue(CloseAllButThisMenuItemVisibilityProperty, value);
#endif
            }
        }

        /// <summary>
        /// Gets or sets the tab list context menu item style.
        /// </summary>
        /// <value>The tab list context menu item style.</value>
        public Style TabItemContextMenuItemStyle
        {
            get
            {
                return (Style)GetValue(TabItemContextMenuItemStyleProperty);
            }
            set
            {
#if !SyncfusionFramework3_5
                SetCurrentValue(TabItemContextMenuItemStyleProperty, value);
#else
                SetValue(TabItemContextMenuItemStyleProperty, value);
#endif
            }
        }

        /// <summary>
        /// Gets the value of the ContextMenuItems dependency property.
        /// </summary>
        /// <value>The context menu items.</value>
        public ObservableCollection<object> ContextMenuItems
        {
            get
            {
                return (ObservableCollection<object>)GetValue(ContextMenuItemsProperty);
            }
        }

        /// <summary>
        /// Gets or sets the value of the HoverBackground dependency property.
        /// </summary>
        /// <value>The hover background.</value>
        public Brush HoverBackground
        {
            get
            {
                return (Brush)GetValue(HoverBackgroundProperty);
            }

            set
            {
#if !SyncfusionFramework3_5
                SetCurrentValue(HoverBackgroundProperty, value);
#else
                SetValue(HoverBackgroundProperty, value);
#endif
            }
        }

        /// <summary>
        /// Gets or sets the value of the ImageAlignment dependency property.
        /// </summary>
        /// <value>The image alignment.</value>
        public ImageAlignment ImageAlignment
        {
            get
            {
                return (ImageAlignment)GetValue(ImageAlignmentProperty);
            }

            set
            {
#if !SyncfusionFramework3_5
                SetCurrentValue(ImageAlignmentProperty, value);
#else
                SetValue(ImageAlignmentProperty, value);
#endif
            }
        }

        /// <summary>
        /// Gets or sets the value of the Image dependency property.
        /// </summary>
        public ImageSource Image
        {
            get
            {
                return (ImageSource)GetValue(ImageProperty);
            }

            set
            {
#if !SyncfusionFramework3_5
                SetCurrentValue(ImageProperty, value);
#else
                SetValue(ImageProperty, value);
#endif
            }
        }

        /// <summary>
        /// Gets or sets height of the image.
        /// </summary>
        public double ImageHeight
        {
            get
            {
                return (double)GetValue(ImageHeightProperty);
            }

            set
            {
#if !SyncfusionFramework3_5
                SetCurrentValue(ImageHeightProperty, value);
#else
                SetValue(ImageHeightProperty, value);
#endif
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is tab group focus.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is tab group focus; otherwise, <c>false</c>.
        /// </value>
        internal bool IsTabGroupFocus
        {
            get
            {
                return (bool)GetValue(IsTabGroupFocusProperty);
            }
            set
            {
#if !SyncfusionFramework3_5
                SetCurrentValue(IsTabGroupFocusProperty, value);
#else
                SetValue(IsTabGroupFocusProperty, value);
#endif

            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is focus.
        /// </summary>
        /// <value><c>true</c> if this instance is focus; otherwise, <c>false</c>.</value>
        internal bool IsFocus
        {
            get
            {
                return (bool)GetValue(IsFocusProperty);
            }
            set
            {
#if !SyncfusionFramework3_5
                SetCurrentValue(IsFocusProperty, value);
#else
                SetValue(IsFocusProperty, value);
#endif
            }
        }

        /// <summary>
        /// Gets or sets width of the image.
        /// </summary>
        public double ImageWidth
        {
            get
            {
                return (double)GetValue(ImageWidthProperty);
            }

            set
            {
#if !SyncfusionFramework3_5
                SetCurrentValue(ImageWidthProperty, value);
#else
                SetValue(ImageWidthProperty, value);
#endif
            }
        }

        /// <summary>
        /// Gets or sets the value of the ItemToolTip dependency property.
        /// </summary>
        public object ItemToolTip
        {
            get
            {
                return GetValue(ItemToolTipProperty);
            }

            set
            {
#if !SyncfusionFramework3_5
                SetCurrentValue(ItemToolTipProperty, value);
#else
                SetValue(ItemToolTipProperty, value);
#endif
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is tab editing.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is tab editing; otherwise, <c>false</c>.
        /// </value>
        public bool IsTabEditing
        {
            get
            {
                return (bool)GetValue(IsTabEditingProperty);
            }
            internal set
            {
                SetValue(IsTabEditingProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value that determines the order in which elements receive focus when the user navigates through controls by using the TAB key.
        /// </summary>
        /// <value></value>
        /// <returns>A value that determines the order of logical navigation for a device. The default value is <see cref="F:System.Int32.MaxValue"/>.</returns>
        public new int TabIndex
        {
            get
            {
                return (int)GetValue(TabIndexProperty);
            }
            set
            {
#if !SyncfusionFramework3_5
                SetCurrentValue(TabIndexProperty, value);
#else
                SetValue(TabIndexProperty, value);
#endif
            }
        }

        #endregion

        #region DP properties

        /// <summary>
        /// Represents the TabIndex Dependency property
        /// </summary>
        public static new readonly DependencyProperty TabIndexProperty =
            DependencyProperty.Register("TabIndex", typeof(int), typeof(TabItemExt), new FrameworkPropertyMetadata(-1,new PropertyChangedCallback(OnTabIndexChanged)));


        /// <summary>
        /// Represents the IsTabEditing dependency property
        /// </summary>
        public static readonly DependencyProperty IsTabEditingProperty =
           DependencyProperty.Register("IsTabEditing", typeof(bool), typeof(TabItemExt), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender)); //new PropertyChangedCallback(OnUseCustomEditableTemplateChanged)));

        /// <summary>
        /// Represents the UseCustomEditableTemplateProperty Dependency property
        /// </summary>
        public static readonly DependencyProperty UseCustomEditableTemplateProperty =
            DependencyProperty.Register("UseCustomEditableTemplate", typeof(bool), typeof(TabItemExt), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnUseCustomEditableTemplateChanged)));

        /// <summary>
        /// Represents the CustomEditableTemplateProperty Dependency property
        /// </summary>
        public static readonly DependencyProperty CustomEditableTemplateProperty =
            DependencyProperty.Register("CustomEditableTemplate", typeof(DataTemplate), typeof(TabItemExt), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnCustomEditableTemplateChanged)));

        /// <summary>
        /// Represents the ContextMenuItemsPropertyKey Dependency property
        /// </summary>
        protected internal static readonly DependencyPropertyKey ContextMenuItemsPropertyKey =
            DependencyProperty.RegisterReadOnly("ContextMenuItems", typeof(ObservableCollection<object>), typeof(TabItemExt), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnContextMenuItemsChanged)));

        /// <summary>
        /// Represents the TabItemContextMenuItemTemplateProperty Dependency property
        /// </summary>
        public static readonly DependencyProperty TabItemContextMenuItemTemplateProperty =
            DependencyProperty.Register("TabItemContextMenuItemTemplate", typeof(DataTemplate), typeof(TabItemExt), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnTabItemContextMenuItemTemplateChanged)));

        /// <summary>
        /// Represents the TabItemContextMenuStyle Dependency property
        /// </summary>
        public static readonly DependencyProperty TabItemContextMenuStyleProperty =
          DependencyProperty.Register("TabItemContextMenuStyle", typeof(Style), typeof(TabItemExt), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Represents the TabItemContextMenuTemplate Dependency property
        /// </summary>
        public static readonly DependencyProperty TabItemContextMenuTemplateProperty =
         DependencyProperty.Register("TabItemContextMenuTemplate", typeof(ControlTemplate), typeof(TabItemExt), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Represents the CloseAllButThisMenuItemVisibility Dependency property
        /// </summary>
        internal static readonly DependencyProperty CloseAllButThisMenuItemVisibilityProperty =
            DependencyProperty.Register("CloseAllButThisMenuItemVisibility", typeof(Visibility), typeof(TabItemExt), new FrameworkPropertyMetadata(Visibility.Visible));

        /// <summary>
        /// Represents the CloseAllMenuItemVisibility Dependency property
        /// </summary>
        internal static readonly DependencyProperty CloseAllMenuItemVisibilityProperty =
            DependencyProperty.Register("CloseAllMenuItemVisibility", typeof(Visibility), typeof(TabItemExt), new FrameworkPropertyMetadata(Visibility.Visible));

        /// <summary>
        /// Represents the CloseMenuItemVisibility Dependency property
        /// </summary>
        internal static readonly DependencyProperty CloseMenuItemVisibilityProperty =
            DependencyProperty.Register("CloseMenuItemVisibility", typeof(Visibility), typeof(TabItemExt), new FrameworkPropertyMetadata(Visibility.Visible));

        /// <summary>
        /// Represents the EnableCloseMenuItem Dependency property
        /// </summary>
        internal static readonly DependencyProperty EnableCloseMenuItemProperty =
            DependencyProperty.Register("EnableCloseMenuItem", typeof(bool), typeof(TabItemExt), new FrameworkPropertyMetadata(true));

        /// <summary>
        /// Represents the TabItemContextMenuItemStyle Dependency property
        /// </summary>
        public static readonly DependencyProperty TabItemContextMenuItemStyleProperty =
         DependencyProperty.Register("TabItemContextMenuItemStyle", typeof(Style), typeof(TabItemExt), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// Represents the ContextMenuItemsProperty Dependency property
        /// </summary>
        public static readonly DependencyProperty ContextMenuItemsProperty = ContextMenuItemsPropertyKey.DependencyProperty;

        /// <summary>
        /// Represents the HoverBackgroundProperty Dependency property
        /// </summary>
        public static readonly DependencyProperty HoverBackgroundProperty =
            DependencyProperty.Register("HoverBackground", typeof(Brush), typeof(TabItemExt), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnHoverBackgroundChanged)));

        /// <summary>
        /// Represents the ImageAlignmentProperty Dependency property
        /// </summary>
        public static readonly DependencyProperty ImageAlignmentProperty =
            DependencyProperty.Register("ImageAlignment", typeof(ImageAlignment), typeof(TabItemExt), new FrameworkPropertyMetadata(ImageAlignment.LeftOfText, new PropertyChangedCallback(OnImageAlignmentChanged)));

        /// <summary>
        /// Represents the ImageProperty Dependency property
        /// </summary>
        public static readonly DependencyProperty ImageProperty =
            DependencyProperty.Register("Image", typeof(ImageSource), typeof(TabItemExt), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnImageChanged)));

        /// <summary>
        /// Represents the ImageHeightProperty Dependency property
        /// </summary>
        public static readonly DependencyProperty ImageHeightProperty =
            DependencyProperty.Register("ImageHeight", typeof(double), typeof(TabItemExt), new FrameworkPropertyMetadata(double.NaN));

        /// <summary>
        /// Represents the ImageWidthProperty Dependency property
        /// </summary>
        public static readonly DependencyProperty ImageWidthProperty =
            DependencyProperty.Register("ImageWidth", typeof(double), typeof(TabItemExt), new FrameworkPropertyMetadata(double.NaN));

        /// <summary>
        /// Represents the IsFocus dependency property
        /// </summary>
        internal static readonly DependencyProperty IsFocusProperty =
            DependencyProperty.Register("IsFocus", typeof(bool), typeof(TabItemExt), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsRender));


        /// <summary>
        /// Represents the IsTabGroupFocus dependency property
        /// </summary>
        internal static readonly DependencyProperty IsTabGroupFocusProperty =
            DependencyProperty.Register("IsTabGroupFocus", typeof(bool), typeof(TabItemExt), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Presents tool tip for item's header.
        /// </summary>
        public static readonly DependencyProperty ItemToolTipProperty =
            DependencyProperty.Register("ItemToolTip", typeof(object), typeof(TabItemExt), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnItemToolTipChanged)));


        /// <summary>
        ///  Represents the HeaderMargin
        /// </summary>
        public static readonly DependencyProperty HeaderMarginProperty =
            DependencyProperty.RegisterAttached("HeaderMargin", typeof(Thickness), typeof(TabItemExt), new FrameworkPropertyMetadata(null));
        
        /// <summary>
        ///  Represents the HeaderContainerMargin
        /// </summary>
        public static readonly DependencyProperty HeaderContainerMarginProperty =
            DependencyProperty.RegisterAttached("HeaderContainerMargin", typeof(Thickness), typeof(TabItemExt), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Gets or sets the header margin.
        /// </summary>
        /// <value>The header margin.</value>
        public Thickness HeaderMargin
        {
            get { return (Thickness)GetValue(HeaderMarginProperty); }
            set
            {
                #if !SyncfusionFramework3_5
                SetCurrentValue(HeaderMarginProperty, value); 
#else
                SetValue(HeaderMarginProperty, value); 
#endif
            }
        }

        /// <summary>
        /// Gets or sets the header container margin.
        /// </summary>
        /// <value>The header container margin.</value>
        internal Thickness HeaderContainerMargin
        {
            get { return (Thickness)GetValue(HeaderContainerMarginProperty); }
            set
            {
#if !SyncfusionFramework3_5
                SetCurrentValue(HeaderContainerMarginProperty, value); 
#else
                SetValue(HeaderContainerMarginProperty, value);
#endif
            }
        }

        /// <summary>
        /// Gets or sets the icon margin.
        /// </summary>
        /// <value>The icon margin.</value>
        public Thickness IconMargin
        {
            get { return (Thickness)GetValue(IconMarginProperty); }
            set { SetValue(IconMarginProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IconMargin.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconMarginProperty =
            DependencyProperty.Register("IconMargin", typeof(Thickness), typeof(TabItemExt), new FrameworkPropertyMetadata(null));

       
        
        #endregion

        ResourceWrapper res = new ResourceWrapper();
        #region Events
        /// <summary>
        /// Event that is raised when FlowDirection property is changed.
        /// </summary>
        public event PropertyChangedCallback FlowDirectionChanged;

        /// <summary>
        /// Event that is raised when UseCustomEditableTemplate property is changed.
        /// </summary>
        public event PropertyChangedCallback UseCustomEditableTemplateChanged;

        /// <summary>
        /// Event that is raised when CustomEditableTemplate property is changed.
        /// </summary>
        public event PropertyChangedCallback CustomEditableTemplateChanged;

        /// <summary>
        /// Event that is raised when ContextMenuItems property is changed.
        /// </summary>
        public event PropertyChangedCallback ContextMenuItemsChanged;

        /// <summary>
        /// Event that is raised when TabItemContextMenuItemTemplate property is changed.
        /// </summary>
        public event PropertyChangedCallback TabItemContextMenuItemTemplateChanged;

        /// <summary>
        /// Event that is raised when HoverBackground property is changed.
        /// </summary>
        public event PropertyChangedCallback HoverBackgroundChanged;

        /// <summary>
        /// Event that is raised when ImageAlignment property is changed.
        /// </summary>
        public event PropertyChangedCallback ImageAlignmentChanged;

        /// <summary>
        /// Event that is raised when Image property is changed.
        /// </summary>
        public event PropertyChangedCallback ImageChanged;

        /// <summary>
        /// Event that is raised when ItemToolTip property is changed.
        /// </summary>
        public event PropertyChangedCallback ItemToolTipChanged;
        #endregion

        #region Event handlers
        /// <summary>
        /// Calls OnUseCustomEditableTemplateChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnUseCustomEditableTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabItemExt instance = (TabItemExt)d;
            instance.OnUseCustomEditableTemplateChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises UseCustomEditableTemplateChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnUseCustomEditableTemplateChanged(DependencyPropertyChangedEventArgs e)
        {
            if (UseCustomEditableTemplateChanged != null)
            {
                UseCustomEditableTemplateChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnCustomEditableTemplateChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnCustomEditableTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabItemExt instance = (TabItemExt)d;
            instance.OnCustomEditableTemplateChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises CustomEditableTemplateChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnCustomEditableTemplateChanged(DependencyPropertyChangedEventArgs e)
        {
            if (CustomEditableTemplateChanged != null)
            {
                CustomEditableTemplateChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnContextMenuItemsChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnContextMenuItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabItemExt instance = (TabItemExt)d;
            instance.OnContextMenuItemsChanged(e);
        }

        /// <summary>
        /// Calls OnTabItemContextMenuItemTemplateChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnTabItemContextMenuItemTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabItemExt instance = (TabItemExt)d;
            instance.OnTabItemContextMenuItemTemplateChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises ContextMenuItemsChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnContextMenuItemsChanged(DependencyPropertyChangedEventArgs e)
        {
            if (ContextMenuItemsChanged != null)
            {
                ContextMenuItemsChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises TabItemContextMenuItemTemplateChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnTabItemContextMenuItemTemplateChanged(DependencyPropertyChangedEventArgs e)
        {
            if (TabItemContextMenuItemTemplateChanged != null)
            {
                TabItemContextMenuItemTemplateChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnHoverBackgroundChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnHoverBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabItemExt instance = (TabItemExt)d;
            instance.OnHoverBackgroundChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises HoverBackgroundChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnHoverBackgroundChanged(DependencyPropertyChangedEventArgs e)
        {
            if (HoverBackgroundChanged != null)
            {
                HoverBackgroundChanged(this, e);
            }
        }

       

        /// <summary>
        /// Calls OnImageAlignmentChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnImageAlignmentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabItemExt instance = (TabItemExt)d;
            instance.OnImageAlignmentChanged(e);
        }

        /// <summary>
        /// Announces that the keyboard is focused on this element.
        /// </summary>
        /// <param name="e">Keyboard input event arguments.</param>
        protected override void OnPreviewGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            if (TabControlParent != null)
            {
                if (TabControlParent.m_focusflag)
                {
                    if (TabControlParent.SelectedItem != null && TabControlParent.GetTabItem(TabControlParent.SelectedItem) != TabControlParent.GetTabItem(this))
                    {
                        TabControlParent.previewselectedargs =
                            new PreviewSelectedItemChangedEventArgs(TabControlParent.GetTabItem(TabControlParent.SelectedItem), TabControlParent.GetTabItem(this) as TabItemExt);
                        TabControlParent.FirePreviewSelectedItemChangedEvent(TabControlParent.previewselectedargs);

                        e.Handled = TabControlParent.previewselectedargs.Cancel;
                        
                    }
                    base.OnPreviewGotKeyboardFocus(e);
                }
                TabControlParent.m_focusflag = false;
            }
        }

        /// <summary>
        /// internal variable which has new tab flag
        /// </summary>
        bool m_IsNewTab = false;

        /// <summary>
        /// Gets or Sets IsNewTab property
        /// </summary>
        [Browsable(false)]
        public bool IsNewTab
        {
            get
            {
                return m_IsNewTab;
            }
            set
            {
                m_IsNewTab = value;
            }
        }

        /// <summary>
        /// internal variable which has tab control ext new parent
        /// </summary>
        internal TabControlExt newtabParent = null;

        

        /// <summary>
        /// Called to remeasure a control.
        /// </summary>
        /// <param name="constraint">The maximum size that the method can return.</param>
        /// <returns>
        /// The size of the control, up to the maximum specified by <paramref name="constraint"/>.
        /// </returns>
        protected override Size MeasureOverride(Size constraint)
        {
            if (TabControlParent != null)
            {
                if (TabControlParent.HideHeaderOnSingleChild)
                {
                    if (CheckItemsVisibility())
                    {
                        SetTemplate(Visibility.Collapsed);
                    }
                    else
                    {
                        SetTemplate(Visibility.Visible);
                    }
                }
                else
                {
                    SetTemplate(Visibility.Visible);
                }
            }
            return base.MeasureOverride(constraint);
        }

        /// <summary>
        /// Gets the text block.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        private String GetHeaderText()
        {
            TextBlock headerblock = null;
            headerblock = m_headerelement !=null ? VisualUtils.FindDescendant((Visual)m_headerelement, typeof(TextBlock)) as TextBlock : null ;
            return headerblock !=null ? headerblock.Text : this.Header.ToString();
        }

        /// <summary>
        /// Calculates the text element size difference.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        internal double CalculateTextElementSizeDifference()
        {
            if (this.Header != null)
            {
                TextBlock textblock = new TextBlock();
                textblock.Text = GetHeaderText();
                BindingUtils.SetBinding(textblock, this, TextElement.FontFamilyProperty, TextElement.FontFamilyProperty);
                BindingUtils.SetBinding(textblock, this, TextElement.FontSizeProperty, TextElement.FontSizeProperty);
                BindingUtils.SetBinding(textblock, this, TextElement.FontStretchProperty, TextElement.FontStretchProperty);
                BindingUtils.SetBinding(textblock, this, TextElement.FontStyleProperty, TextElement.FontStyleProperty);
                textblock.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                Size normalsize = textblock.DesiredSize;
                TextElement.SetFontWeight(textblock, FontWeights.ExtraBlack);
                textblock.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                Size maxsize = textblock.DesiredSize;
                if (maxsize.Width > 0 && normalsize.Width > 0)
                    return (maxsize.Width - normalsize.Width) / 2;
            }
            return 0;
        }

        /// <summary>
        /// Checks the items visibility.
        /// </summary>
        /// <returns></returns>
        private bool CheckItemsVisibility()
        {
            int count = 0;
            if (TabControlParent.Items.Count > 0)
            {
                if (TabControlParent.Items[0] is TabItemExt)
                {
                    foreach (TabItemExt item in TabControlParent.Items)
                    {
                        if (item.Visibility == Visibility.Visible)
                        {
                            count++;
                        }
                        if (count > 1)
                        {
                            return false;
                        }
                    }
                    return true;
                }
                else
                {
                    foreach (object obj in TabControlParent.Items)
                    {
                        TabItemExt item = TabControlParent.GetTabItem(obj);
                        if (item != null)
                        {
                            if (item.Visibility == Visibility.Visible)
                            {
                                count++;
                            }
                            if (count > 1)
                            {
                                return false;
                            }
                        }
                    }
                    return true;
                }
            }
            return false;
        }
        bool m_TabEditingflg = false;
        /// <summary>
        /// Raises the <see cref="E:System.Windows.Controls.Control.MouseDoubleClick"/> routed event.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            if (e.StylusDevice == null || e.StylusDevice != null)
            {
                base.OnMouseDoubleClick(e);
                IsTabEditing = true;
                m_TabEditingflg = true;
            }
        }
       

        /// <summary>
        /// Checks the close button visibility on load.
        /// </summary>
        /// <param name="closebutton">The closebutton.</param>
        /// <param name="close">if set to <c>true</c> [close].</param>
        /// <param name="behavior">The behavior.</param>
        internal void CheckCloseButtonVisibilityOnLoad(ToggleButton closebutton, bool close, DisabledButtonsBehavior behavior)
        {
            if (TabControlParent != null && TabControlParent.CloseButtonType == CloseButtonType.IndividualOnMouseOver)
            {
                closebutton.Visibility = Visibility.Hidden;
            }
            else if (TabControlParent != null && (TabControlParent.CloseButtonType == CloseButtonType.Both || TabControlParent.CloseButtonType == CloseButtonType.Individual))
            {
                closebutton.Visibility = Visibility.Visible;
            }
            else if (TabControlParent != null && TabControlParent.CloseButtonType == CloseButtonType.Extended)
            {
                if (IsSelected)
                    closebutton.Visibility = Visibility.Visible;
                else
                    closebutton.Visibility = Visibility.Hidden;
            }
            else
            {
                closebutton.Visibility = Visibility.Collapsed;
            }
            closebutton.IsEnabled = true;

            if (!close)
            {
                switch (behavior)
                {
                    case DisabledButtonsBehavior.Collapse:
                        closebutton.Visibility = Visibility.Collapsed;
                        break;
                    case DisabledButtonsBehavior.Hide:
                        closebutton.Visibility = Visibility.Hidden;
                        break;
                    case DisabledButtonsBehavior.Disable:
                        closebutton.IsEnabled = false;
                        break;
                }
            }
        }

        #region TouchEvents
#if !SyncfusionFramework3_5

        //protected override void OnTouchLeave(TouchEventArgs e)
        //{
        //    TabControlExt tabControlParent = Parent as TabControlExt;
        //    if (tabControlParent != null && tabControlParent.IsTouchEnabled && tabControlParent.m_tabControlExtTouchDeviceId == e.TouchDevice.Id)
        //    {
        //        #region CloseButtonVisibilityChecking

        //        if (this.Content != null)
        //        {
        //            ContentPresenter presenter = this.Content as ContentPresenter;
        //            if (presenter != null && presenter.Content != null)
        //            {
        //                ToggleButton closebutton = GetTemplateChild("PART_CloseButton") as ToggleButton;
        //                if (closebutton != null)
        //                {
        //                    DockingManager owner = DockingManager.ResolveManager(presenter.Content as UIElement);
        //                    if (owner != null)
        //                    {
        //                        bool close = DockingManager.GetCanClose(presenter.Content as DependencyObject);
        //                        CheckCloseButtonVisibilityOnLoad(closebutton, close, owner.DisabledCloseButtonsBehavior);
        //                    }
        //                    else
        //                    {
        //                        DocumentContainer container = VisualUtils.FindAncestor((Visual)this, typeof(DocumentContainer)) as DocumentContainer;
        //                        if (container != null)
        //                        {
        //                            bool close = DocumentContainer.GetCanClose(presenter.Content as DependencyObject);
        //                            CheckCloseButtonVisibilityOnLoad(closebutton, close, container.DisabledButtonsBehavior);
        //                        }
        //                    }
        //                }
        //            }
        //        }

        //        #endregion
        //    }
        //    base.OnTouchLeave(e);
        //}

        //protected override void OnTouchMove(TouchEventArgs e)
        //{
        //    TabControlExt tabControlParent = Parent as TabControlExt;
        //    if (tabControlParent != null && tabControlParent.IsTouchEnabled && tabControlParent.m_tabControlExtTouchDeviceId == e.TouchDevice.Id)
        //    {
        //        if (tabControlParent.m_tabControlExtSystemGesture == SystemGesture.RightTap)
        //        {
                    
        //        }
        //        if (this.Content != null)
        //        {
        //            ContentPresenter presenter = this.Content as ContentPresenter;
        //            if (presenter != null && presenter.Content != null)
        //            {
        //                ToggleButton closebutton = GetTemplateChild("PART_CloseButton") as ToggleButton;
        //                if (closebutton != null)
        //                {
        //                    DockingManager owner = DockingManager.ResolveManager(presenter.Content as UIElement);
        //                    if (owner != null)
        //                    {
        //                        bool close = DockingManager.GetCanClose(presenter.Content as DependencyObject);
        //                        CheckCloseButtonVisibilityOnMouseMove(closebutton, close, owner.DisabledCloseButtonsBehavior);
        //                    }
        //                    else
        //                    {
        //                        DocumentContainer container = VisualUtils.FindAncestor((Visual)this, typeof(DocumentContainer)) as DocumentContainer;
        //                        if (container != null)
        //                        {
        //                            bool close = DocumentContainer.GetCanClose(presenter.Content as DependencyObject);
        //                            CheckCloseButtonVisibilityOnMouseMove(closebutton, close, container.DisabledButtonsBehavior);
        //                        }
        //                    }
        //                }
        //            }
        //        }

        //        if ((Parent as DocumentTabControl) != null)
        //        {
        //            int count = 0;
        //            foreach (TabItemExt tab in (Parent as DocumentTabControl).Items)
        //            {
        //                if (this.Equals(tab))
        //                {
        //                    (Parent as DocumentTabControl).m_mousemoveonitemindex = count;
        //                    break;
        //                }
        //                count++;
        //            }
        //        }
        //    }
        //    base.OnTouchMove(e);
        //}

        //internal bool m_IsNoSelect = false;
        //internal bool m_IsTouchDownElement = false;
        //protected override void OnTouchUp(TouchEventArgs e)
        //{
        //    TabControlExt tabControlParent = Parent as TabControlExt;
        //    if (tabControlParent != null && tabControlParent.IsTouchEnabled && tabControlParent.m_tabControlExtTouchDeviceId == e.TouchDevice.Id)
        //    {
        //        if (!m_IsNoSelect)
        //        {
        //            if (m_ActiveWindowFlag)
        //            {
        //                if (TabControlParent != null && TabControlParent.GetTabItem(TabControlParent.SelectedItem) != this)
        //                {
        //                    TabControlParent.previewselectedargs =
        //                        new PreviewSelectedItemChangedEventArgs(TabControlParent.GetTabItem(TabControlParent.SelectedItem), TabControlParent.GetTabItem(this));
        //                    TabControlParent.FirePreviewSelectedItemChangedEvent(TabControlParent.previewselectedargs);
        //                    if (!TabControlParent.previewselectedargs.Cancel)
        //                    {
        //                        IsSelected = true;
        //                        m_ActiveWindowFlag = true;
        //                    }
        //                    TabControlParent.m_focusflag = false;
        //                }
        //            }
        //        }
        //        m_IsNoSelect = false;
        //        m_IsTouchDownElement = false;
        //        #region FingerDoubleTouch
        //        if (tabControlParent.m_tabControlExtSystemGesture == SystemGesture.TwoFingerTap)
        //        {
        //            IsTabEditing = true;
        //            m_TabEditingflg = true;
        //        }
        //        #endregion

        //        #region TouchRightFingerUp
        //        if (tabControlParent.m_tabControlExtSystemGesture == SystemGesture.RightTap)
        //        {
        //            if (TabControlParent != null)
        //            {
        //                if (TabControlParent.ShowTabItemContextMenu)
        //                {
        //                    var inBuildContextMenu = new ContextMenu();
        //                    AddItemstoMenu(inBuildContextMenu, this);

        //                    if (TabControlParent.DefaultContextMenuItemVisibility == Visibility.Visible)
        //                    {
        //                        if (TabControlParent.IsCustomTabItemContextMenuEnabled)
        //                        {
        //                            if (ContextMenu == null)
        //                            {
        //                                ContextMenu = inBuildContextMenu;
        //                            }
        //                            if (ContextMenu.Items.Count > 0)
        //                            {
        //                                ContextMenu.Visibility = Visibility.Visible;
        //                            }

        //                        }
        //                        else
        //                        {
        //                            ContextMenu = inBuildContextMenu;
        //                        }
        //                        CloseMenuItemVisibility = Visibility.Visible;
        //                        CloseAllMenuItemVisibility = Visibility.Visible;
        //                        CloseAllButThisMenuItemVisibility = Visibility.Visible;

        //                    }
        //                    else
        //                    {
        //                        if (TabControlParent.IsCustomTabItemContextMenuEnabled)
        //                        {
        //                            if (ContextMenu == null)
        //                            {
        //                                ContextMenu = inBuildContextMenu;
        //                            }
        //                            if (ContextMenu.Items.Count == 0)
        //                            {
        //                                ContextMenu.Visibility = Visibility.Collapsed;
        //                            }
        //                        }
        //                        else
        //                        {
        //                            ContextMenu = inBuildContextMenu;
        //                            ContextMenu.Visibility = Visibility.Collapsed;
        //                        }
        //                        CloseMenuItemVisibility = Visibility.Collapsed;
        //                        CloseAllMenuItemVisibility = Visibility.Collapsed;
        //                        CloseAllButThisMenuItemVisibility = Visibility.Collapsed;
        //                    }
        //                }
        //                else
        //                {
        //                    ContextMenu = null;
        //                }
        //            }

        //            #region ContextMenu Style and Template

        //            if (ContextMenu != null)
        //            {
        //                ContextMenu.Style = TabItemContextMenuStyle ?? ContextMenu.Style;

        //                ContextMenu.Template = TabItemContextMenuTemplate ?? ContextMenu.Template;

        //                if (ContextMenu.Items.Count > 0)
        //                {
        //                    foreach (var menuitem in ContextMenu.Items.OfType<MenuItem>())
        //                    {
        //                        menuitem.HeaderTemplate = TabItemContextMenuItemTemplate ?? menuitem.HeaderTemplate;
        //                        menuitem.Style = TabItemContextMenuItemStyle ?? menuitem.Style;
        //                    }
        //                }
        //                if (ContextMenu.Items.Count > 0 && TabControlParent.IsCustomTabItemContextMenuEnabled && ContextMenu.Visibility == Visibility.Collapsed)
        //                    ContextMenu.Visibility = Visibility.Visible;
        //            }
        //            #endregion

        //            #region ContextMenu Visibility
        //            if (ContextMenu != null)
        //            {
        //                if (IsNewTab)
        //                {
        //                    ContextMenu.Visibility = Visibility.Collapsed;
        //                }
        //                ContextMenu.Opened += ContextMenu_Opened;
        //            }
        //            #endregion

        //            if (TabControlParent != null)
        //            {
        //                TabControlParent.SelectedItem = this;
        //                TabControlParent.ActivatedItem = this;
        //            }
        //        }
        //        #endregion
        //    }
        //    base.OnTouchUp(e);
        //}

        //protected override void OnTouchDown(TouchEventArgs e)
        //{
        //    TabControlExt tabControlParent = Parent as TabControlExt;
        //    if (tabControlParent != null && tabControlParent.IsTouchEnabled && tabControlParent.m_tabControlExtTouchDeviceId == e.TouchDevice.Id)
        //    {
        //        m_IsTouchDownElement = true;
        //        #region TouchLeftFingerDown
        //        try
        //        {
        //            if (this.ContextMenu != null)
        //                this.ContextMenu.Visibility = Visibility.Collapsed;
        //            if (!m_TabEditingflg)
        //            {
        //                if (IsTabEditing)
        //                {
        //                    IsTabEditing = false;
        //                    if (TabControlParent != null)
        //                    {
        //                        TabControlParent.CompleteHeaderEdit(this, true);
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                m_TabEditingflg = false;
        //            }

        //            if (TabControlParent != null)
        //            {
        //                this.TabControlParent.IsFocus = true;
        //                TabControlParent.m_focusflag = true;
        //            }
                    
        //            if (IsNewTab && newtabParent != null)
        //            {
        //                int index1 = newtabParent.Items.Count;
        //                object newtabitem = newtabParent.InvokeNewTabItem(this, null);
        //                int index2 = newtabParent.Items.Count;

        //                if (index1 >= index2 && newtabParent.SelectionStack.Count > 0)
        //                {
        //                    newtabParent.SelectedItem = newtabParent.SelectionStack[newtabParent.SelectionStack.Count - 1];
        //                }
        //                else
        //                {

        //                    newtabParent.SelectedItem = newtabitem;

        //                    if (newtabParent.Items.IndexOf(newtabitem) == index2 - 1)
        //                    {
        //                        newtabParent.TabLayoutPanel.StartScrolling(-80 * index2, -100 * index2);
        //                    }
        //                    else
        //                    {
        //                        if (newtabitem is TabItemExt)
        //                        {
        //                            newtabParent.TabLayoutPanel.SelectItemInternal(newtabitem as TabItemExt);
        //                        }
        //                        else
        //                        {
        //                            newtabParent.TabLayoutPanel.SelectItemInternal(newtabParent.GetTabItem(newtabitem));
        //                        }
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                DockingManager owner = VisualUtils.FindAncestor(this, typeof(DockingManager)) as DockingManager;
        //                if (owner != null && !owner.EnableDocumentTabHeaderEdit)
        //                {
        //                    ContentPresenter cont = Content as ContentPresenter;
        //                    if (cont != null)
        //                    {
        //                        UIElement innercontent = cont.Content as UIElement;
        //                        if (innercontent != null && !BrowserInteropHelper.IsBrowserHosted)
        //                        {
        //                            if (Application.Current != null)
        //                            {
        //                                innercontent.Focusable = true;
        //                                if (!(innercontent is DockingManager) && (innercontent is IInputElement))
        //                                    FocusManager.SetFocusedElement(Application.Current.MainWindow, innercontent as IInputElement);
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        catch { }
        //        #endregion
        //    }
        //    base.OnTouchDown(e);
        //}
#endif
        #endregion
        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.MouseLeave"/> attached event is raised on this element. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseEventArgs"/> that contains the event data.</param>
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            if (e.StylusDevice == null || e.StylusDevice != null)
            {
                base.OnMouseLeave(e);

                #region CloseButtonVisibilityChecking

                if (this.Content != null)
                {
                    ContentPresenter presenter = this.Content as ContentPresenter;
                    if (presenter != null && presenter.Content != null)
                    {
                        ToggleButton closebutton = GetTemplateChild("PART_CloseButton") as ToggleButton;
                        if (closebutton != null)
                        {
                            DockingManager owner = DockingManager.ResolveManager(presenter.Content as UIElement);
                            if (owner != null)
                            {
                                bool close = DockingManager.GetCanClose(presenter.Content as DependencyObject);
                                CheckCloseButtonVisibilityOnLoad(closebutton, close, owner.DisabledCloseButtonsBehavior);
                            }
                            else
                            {
                                DocumentContainer container = VisualUtils.FindAncestor((Visual)this, typeof(DocumentContainer)) as DocumentContainer;
                                if (container != null)
                                {
                                    bool close = DocumentContainer.GetCanClose(presenter.Content as DependencyObject);
                                    CheckCloseButtonVisibilityOnLoad(closebutton, close, container.DisabledButtonsBehavior);
                                }
                            }
                        }
                    }
                }

                #endregion
            }
        }

        /// <summary>
        /// Checks the close button visibility on mouse move.
        /// </summary>
        /// <param name="closebutton">The closebutton.</param>
        /// <param name="close">if set to <c>true</c> [close].</param>
        /// <param name="behavior">The behavior.</param>
        private void CheckCloseButtonVisibilityOnMouseMove(ToggleButton closebutton, bool close, DisabledButtonsBehavior behavior)
        {
            if (TabControlParent != null && (TabControlParent.CloseButtonType == CloseButtonType.IndividualOnMouseOver || TabControlParent.CloseButtonType == CloseButtonType.Both || TabControlParent.CloseButtonType == CloseButtonType.Individual || TabControlParent.CloseButtonType == CloseButtonType.Extended))
            {
                closebutton.Visibility = Visibility.Visible;
            }
            else
            {
                closebutton.Visibility = Visibility.Collapsed;
            }
            closebutton.IsEnabled = true;

            if (!close)
            {
                switch (behavior)
                {
                    case DisabledButtonsBehavior.Collapse:
                        closebutton.Visibility = Visibility.Collapsed;
                        break;
                    case DisabledButtonsBehavior.Hide:
                        closebutton.Visibility = Visibility.Hidden;
                        break;
                    case DisabledButtonsBehavior.Disable:
                        closebutton.IsEnabled = false;
                        break;
                }
            }
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.MouseMove"/> attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseEventArgs"/> that contains the event data.</param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (e.StylusDevice == null || e.StylusDevice != null)
            {
                base.OnMouseMove(e);

                if (this.Content != null)
                {
                    ContentPresenter presenter = this.Content as ContentPresenter;
                    if (presenter != null && presenter.Content != null)
                    {
                        ToggleButton closebutton = GetTemplateChild("PART_CloseButton") as ToggleButton;
                        if (closebutton != null)
                        {
                            DockingManager owner = DockingManager.ResolveManager(presenter.Content as UIElement);
                            if (owner != null)
                            {
                                bool close = DockingManager.GetCanClose(presenter.Content as DependencyObject);
                                CheckCloseButtonVisibilityOnMouseMove(closebutton, close, owner.DisabledCloseButtonsBehavior);
                            }
                            else
                            {
                                DocumentContainer container = VisualUtils.FindAncestor((Visual)this, typeof(DocumentContainer)) as DocumentContainer;
                                if (container != null)
                                {
                                    bool close = DocumentContainer.GetCanClose(presenter.Content as DependencyObject);
                                    CheckCloseButtonVisibilityOnMouseMove(closebutton, close, container.DisabledButtonsBehavior);
                                }
                            }
                        }
                    }
                }

                if ((Parent as DocumentTabControl) != null)
                {
                    int count = 0;
                    foreach (TabItemExt tab in (Parent as DocumentTabControl).Items)
                    {
                        if (this.Equals(tab))
                        {
                            (Parent as DocumentTabControl).m_mousemoveonitemindex = count;
                            break;
                        }
                        count++;
                    }
                }
            }
        }

       

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
          

            if (e.Property == SkinStorage.VisualStyleProperty)
            {                
                    ResourceDictionary rd = new ResourceDictionary();
                    if (TabControlParent != null)
                    {
                        if (TabControlParent.TabVisualStyle != TabVisualStyle.None)
                        {
                            if (SkinStorage.GetVisualStyle(this) == "Office2010Black")
                            {                       
                                rd.Source = new Uri(@"/Syncfusion.Tools.WPF;component/Controls/TabControlExt/Themes/ExcelBlackStyle.xaml", UriKind.RelativeOrAbsolute);
                                this.Style = rd["TabItemExtStyle"] as Style;
                            }
                            else if(SkinStorage.GetVisualStyle(this) == "Office2010Blue")
                            {
                                rd.Source = new Uri(@"/Syncfusion.Tools.WPF;component/Controls/TabControlExt/Themes/ExcelBlueStyle.xaml", UriKind.RelativeOrAbsolute);
                                this.Style = rd["TabItemExtStyle"] as Style;
                            }
                            else if(SkinStorage.GetVisualStyle(this) == "Office2010Silver")
                            {
                                rd.Source = new Uri(@"/Syncfusion.Tools.WPF;component/Controls/TabControlExt/Themes/ExcelSilverStyle.xaml", UriKind.RelativeOrAbsolute);
                                this.Style = rd["TabItemExtStyle"] as Style;
                            }
                        }
                    }
                }            
        }

        internal bool m_ActiveWindowFlag = true;
        /// <summary>
        /// Responds to the <see cref="E:System.Windows.ContentElement.MouseLeftButtonDown"/> event.
        /// </summary>
        /// <param name="e">Provides data for <see cref="T:System.Windows.Input.MouseButtonEventArgs"/>.</param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (e.StylusDevice == null || e.StylusDevice != null)
            {
                try
                {
                    if (this.ContextMenu != null)
                        this.ContextMenu.Visibility = Visibility.Collapsed;
                    if (!m_TabEditingflg)
                    {
                        if (IsTabEditing)
                        {
                            IsTabEditing = false;
                            if (TabControlParent != null)
                            {
                                TabControlParent.CompleteHeaderEdit(this, true);
                            }
                        }
                    }
                    else
                    {
                        m_TabEditingflg = false;
                    }

                    if (TabControlParent != null)
                    {
                        this.TabControlParent.IsFocus = true;
                        TabControlParent.m_focusflag = true;
                    }
                    if (m_ActiveWindowFlag)
                    {
                        if (TabControlParent != null && TabControlParent.GetTabItem(TabControlParent.SelectedItem) != this)
                        {
                            TabControlParent.previewselectedargs =
                                new PreviewSelectedItemChangedEventArgs(TabControlParent.GetTabItem(TabControlParent.SelectedItem), TabControlParent.GetTabItem(this));
                            TabControlParent.FirePreviewSelectedItemChangedEvent(TabControlParent.previewselectedargs);
                            if (!TabControlParent.previewselectedargs.Cancel)
                            {
                                IsSelected = true;
                                m_ActiveWindowFlag = true;
                            }
                            TabControlParent.m_focusflag = false;
                        }
                    }
                    if (IsNewTab && newtabParent != null && newtabParent.TabLayoutPanel != null && newtabParent.TabLayoutPanel.VisibleItemsCount!=0)
                    {
                        int index1 = newtabParent.Items.Count;
                        object newtabitem = newtabParent.InvokeNewTabItem(this, null);
                        int index2 = newtabParent.Items.Count;

                        if (index1 >= index2 && newtabParent.SelectionStack.Count > 0)
                        {
                            newtabParent.SelectedItem = newtabParent.SelectionStack[newtabParent.SelectionStack.Count - 1];
                        }
                        else
                        {

                            newtabParent.SelectedItem = newtabitem;

                            if (newtabParent.Items.IndexOf(newtabitem) == index2 - 1)
                            {
                                newtabParent.TabLayoutPanel.StartScrolling(-80 * index2, -100 * index2);
                            }
                            else
                            {
                                if (newtabitem is TabItemExt)
                                {
                                    newtabParent.TabLayoutPanel.SelectItemInternal(newtabitem as TabItemExt);
                                }
                                else
                                {
                                    newtabParent.TabLayoutPanel.SelectItemInternal(newtabParent.GetTabItem(newtabitem));
                                }
                            }
                        }
                    }
                    else
                    {
                        DockingManager owner = VisualUtils.FindAncestor(this, typeof(DockingManager)) as DockingManager;
                        if (owner != null && !owner.EnableDocumentTabHeaderEdit)
                        {
                            ContentPresenter cont = Content as ContentPresenter;
                            if (cont != null)
                            {
                                UIElement innercontent = cont.Content as UIElement;
                                if (innercontent != null && !BrowserInteropHelper.IsBrowserHosted)
                                {
                                    if (Application.Current != null)
                                    {
                                        innercontent.Focusable = true;
                                        if (!(innercontent is DockingManager) && (innercontent is IInputElement))
                                            FocusManager.SetFocusedElement(Application.Current.MainWindow, innercontent as IInputElement);
                                    }
                                }
                            }
                        }
                    }
                }
                catch { }
            }
        }

      
        internal void AddItemstoMenu(ContextMenu menu, TabItemExt item)
        {
            var collection = TabControlExt.GetContextMenuItems(item);
            if (collection != null)
            {
                foreach (object menuitem in collection)
                {
                    var frameworkElement = menuitem as FrameworkElement;
                    if (frameworkElement != null)
                    {
                        var mnuitem = frameworkElement.Parent as ContextMenu;

                        if (mnuitem != null)
                        {
                            mnuitem.Items.Remove(menuitem);
                        }
                    }
                    menu.Items.Add(menuitem);
                }
            }
        }

        private void ContextMenu_Opened(object sender, RoutedEventArgs e)
        {
           if(TabControlParent!=null && TabControlParent.TabLayoutPanel!=null && !(TabControlParent.TabLayoutPanel.m_dragInfo.Equals(null)))
              TabControlParent.TabLayoutPanel.m_dragInfo.SkipDrag = true;
            ((ContextMenu)sender).Opened -= ContextMenu_Opened;
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.UIElement.MouseRightButtonUp"/> routed event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs"/> that contains the event data. The event data reports that the right mouse button was released.</param>
        protected override void OnMouseRightButtonUp(MouseButtonEventArgs e)
        {
            if (e.StylusDevice == null || e.StylusDevice != null)
            {
                base.OnMouseRightButtonUp(e);

                if (TabControlParent != null)
                {
                    FrameworkElement borderElement = this.Template.FindName("Bd", this) as FrameworkElement;
                    if (borderElement != null)
                    {
                        HitTestResult testresult = VisualTreeHelper.HitTest(borderElement as Visual, e.GetPosition(this));
                        if (testresult != null)
                        {
                            if (TabControlParent.ShowTabItemContextMenu)
                            {
                                var inBuildContextMenu = new ContextMenu();
                                AddItemstoMenu(inBuildContextMenu, this);

                                if (TabControlParent.DefaultContextMenuItemVisibility == Visibility.Visible)
                                {
                                    if (TabControlParent.IsCustomTabItemContextMenuEnabled)
                                    {
                                        if (ContextMenu == null)
                                        {
                                            ContextMenu = inBuildContextMenu;
                                        }
                                        if (ContextMenu.Items.Count > 0)
                                        {
                                            ContextMenu.Visibility = Visibility.Visible;
                                        }

                                    }
                                    else
                                    {
                                        ContextMenu = inBuildContextMenu;
                                    }
                                    CloseMenuItemVisibility = Visibility.Visible;
                                    CloseAllMenuItemVisibility = Visibility.Visible;
                                    CloseAllButThisMenuItemVisibility = Visibility.Visible;

                                }
                                else
                                {
                                    if (TabControlParent.IsCustomTabItemContextMenuEnabled)
                                    {
                                        if (ContextMenu == null)
                                        {
                                            ContextMenu = inBuildContextMenu;
                                        }
                                        if (ContextMenu.Items.Count == 0)
                                        {
                                            ContextMenu.Visibility = Visibility.Collapsed;
                                        }
                                    }
                                    else
                                    {
                                        ContextMenu = inBuildContextMenu;
                                        ContextMenu.Visibility = Visibility.Collapsed;
                                    }
                                    CloseMenuItemVisibility = Visibility.Collapsed;
                                    CloseAllMenuItemVisibility = Visibility.Collapsed;
                                    CloseAllButThisMenuItemVisibility = Visibility.Collapsed;
                                }
                            }
                            else
                            {
                                ContextMenu = null;
                            }
                        }
                        else
                            this.ContextMenu = null;
                    }
                }
                DockingManager manager = VisualUtils.FindAncestor(this, typeof(DockingManager)) as DockingManager;
                ContentPresenter presenter = this.Content as ContentPresenter;
                if (presenter != null && manager != null)
                {
                    FrameworkElement element = presenter.Content as FrameworkElement;
                    if (element != null && manager.ShowTabItemContextMenu)
                    {
                        DocumentTabItemMenuItemCollection itemCollection = DockingManager.GetDocumentTabItemContextMenuItems(element);
                        if (itemCollection != null && itemCollection.Count > 0)
                        {
                            int customItemCount = itemCollection.Count;
                            for (int i = 0; i < customItemCount; ++i)
                            {
                                ContextMenu childmenuitem = (itemCollection[i] as FrameworkElement).Parent as ContextMenu;
                                if (childmenuitem != null)
                                {
                                    childmenuitem.Items.Remove(itemCollection[i]);
                                }
                                MenuItem menuitem = itemCollection[i] as MenuItem;
                                if (menuitem != null)
                                {
                                    menuitem.Focusable = false;
                                    if (ContextMenu != null)
                                        ContextMenu.Items.Add(menuitem);
                                }
                            }
                        }
                    }
                }

                #region ContextMenu Style and Template

                if (ContextMenu != null)
                {
                    ContextMenu.Style = TabItemContextMenuStyle ?? ContextMenu.Style;

                    ContextMenu.Template = TabItemContextMenuTemplate ?? ContextMenu.Template;

                    if (ContextMenu.Items.Count > 0)
                    {
                        foreach (var menuitem in ContextMenu.Items.OfType<MenuItem>())
                        {
                            menuitem.HeaderTemplate = TabItemContextMenuItemTemplate ?? menuitem.HeaderTemplate;
                            menuitem.Style = TabItemContextMenuItemStyle ?? menuitem.Style;
                        }
                    }
                    if (ContextMenu.Items.Count > 0 && TabControlParent.IsCustomTabItemContextMenuEnabled && ContextMenu.Visibility == Visibility.Collapsed)
                        ContextMenu.Visibility = Visibility.Visible;
                }
                #endregion

                #region ContextMenu Visibility
                if (ContextMenu != null)
                {
                    if (IsNewTab)
                    {
                        ContextMenu.Visibility = Visibility.Collapsed;
                    }
                    ContextMenu.Opened += ContextMenu_Opened;
                }
                #endregion

                if (TabControlParent != null)
                {
                    TabControlParent.SelectedItem = this;
                    TabControlParent.ActivatedItem = this;
                }
            }
        }


        /// <summary>
        /// Gets the contents.
        /// </summary>
        /// <returns></returns>
        internal FrameworkElement GetContents()
        {
            ContentPresenter presenter = this.Content as ContentPresenter;
            if (presenter != null && presenter.Content != null)
            {
                FrameworkElement element = presenter.Content as FrameworkElement;
                return element;
            }
            return null;
        }

        /// <summary>
        /// Updates property value cache and raises ImageAlignmentChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnImageAlignmentChanged(DependencyPropertyChangedEventArgs e)
        {
            if (ImageAlignmentChanged != null)
            {
                ImageAlignmentChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnImageChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnImageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabItemExt instance = (TabItemExt)d;
            instance.OnImageChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises ImageChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnImageChanged(DependencyPropertyChangedEventArgs e)
        {
            if (ImageChanged != null)
            {
                ImageChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnFlowDirectionChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnFlowDirectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabItemExt instance = (TabItemExt)d;
            instance.OnFlowDirectionChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises IsCloseTabProcessEnabledChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnFlowDirectionChanged(DependencyPropertyChangedEventArgs e)
        {
            if (FlowDirectionChanged != null)
            {
                FlowDirectionChanged(this, e);
            }
        }

        /// <summary>
        /// Raises ItemToolTipChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnItemToolTipChanged(DependencyPropertyChangedEventArgs e)
        {
            if (null != ItemToolTipChanged)
            {
                ItemToolTipChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnItemToolTipChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnItemToolTipChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabItemExt instance = (TabItemExt)d;
            instance.OnItemToolTipChanged(e);
        }
        #endregion

        #region Overriden methods
        /// <summary>
        /// Calls on Apply Template
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            ToggleButton button = GetTemplateChild("PART_CloseButton") as ToggleButton;
            m_headerelement = GetTemplateChild("Content") as ContentPresenter;
            if (button != null && IsNewTab)
            {
                button.Visibility = Visibility.Hidden;
            }            
        }
       
        #endregion
    }
}