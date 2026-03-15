// <copyright file="DocumentTabControl.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Syncfusion.Windows.Shared;
using Syncfusion.Licensing;
using System.Windows.Media.Media3D;
using System.Threading;
using System.Collections.Specialized;
using System.Windows.Documents;
using Syncfusion.Windows.Tools.Controls.Resources;


namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents class for document tab control
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    [SkinType(SkinVisualStyle = Skin.Office2007Blue,
    Type = typeof(DocumentContainer), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/DocumentContainer/Themes/Office2007BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Black,
    Type = typeof(DocumentContainer), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/DocumentContainer/Themes/Office2007BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Silver,
    Type = typeof(DocumentContainer), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/DocumentContainer/Themes/Office2007SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Blue,
    Type = typeof(DocumentContainer), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/DocumentContainer/Themes/Office2010BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Black,
    Type = typeof(DocumentContainer), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/DocumentContainer/Themes/Office2010BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Silver,
    Type = typeof(DocumentContainer), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/DocumentContainer/Themes/Office2010SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2003,
    Type = typeof(DocumentContainer), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/DocumentContainer/Themes/Office2003Style.xaml")]
    [SkinType(SkinVisualStyle = Skin.Blend,
    Type = typeof(DocumentContainer), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/DocumentContainer/Themes/BlendStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.SyncOrange,
    Type = typeof(DocumentContainer), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/DocumentContainer/Themes/SyncOrangeStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.ShinyRed,
    Type = typeof(DocumentContainer), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/DocumentContainer/Themes/ShinyRedStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.ShinyBlue,
    Type = typeof(DocumentContainer), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/DocumentContainer/Themes/ShinyBlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Default,
    Type = typeof(DocumentContainer), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/DocumentContainer/Themes/vista.aero.xaml")]
    public class DocumentTabControl : TabControlExt, IDisposable
    {

        #region Constants
        /////// <summary>
        /////// Represents the New Horizontal Tab Group
        /////// </summary>
        ////private const string HORIZONTAL_TAB = "New Horizontal Tab Group";

        /////// <summary>
        /////// Represents the New Vertical Tab Group
        /////// </summary>
        ////private const string VERTICAL_TAB = "New Vertical Tab Group";

        /////// <summary>
        /////// Represents the Move To Next Tab Group
        /////// </summary>
        ////private const string NEXT_TAB_GROUP = "Move To Next Tab Group";

        /////// <summary>
        /////// Represents the Move To Previous Tab Group
        /////// </summary>
        ////private const string PREVIOUS_TAB_GROUP = "Move To Previous Tab Group";

        /// <summary>
        /// Represents the HORIZONTAL_IMAGE
        /// </summary>
        private const string HORIZONTAL_IMAGE = "pack://application:,,,/Syncfusion.Tools.WPF;component/Framework/DocumentContainer/Resources/Horizontal_TabGroup_Image.png";

        /// <summary>
        /// Represents the VERTICAL_IMAGE
        /// </summary>
        private const string VERTICAL_IMAGE = "pack://application:,,,/Syncfusion.Tools.WPF;component/Framework/DocumentContainer/Resources/Vertical_TabGroup_Image.png";

        /// <summary>
        /// Contains path to container generic templates.
        /// </summary>
        private const string GENERIC_TEMPATES = "pack://application:,,,/Syncfusion.Tools.WPF;component/Framework/DocumentContainer/Themes/generic.xaml";

        /// <summary>
        /// Contains name of the separator style.
        /// </summary>
        private const string SEP_STYLE = "SeparatorStyle1";

        /// <summary>
        /// Contains name of the separator template.
        /// </summary>
        private const string SEP_TEMPLATE = "TDISeparatorTemplate";

        /////// <summary>
        /////// Contains floating menu item name.
        /////// </summary>
        ////private const string FLOATING = "Floating";

        /////// <summary>
        /////// Contains dockable menu item name.
        /////// </summary>
        ////private const string DOCKABLE = "Dockable";

        /////// <summary>
        /////// Contains document menu item name.
        /////// </summary>
        ////private const string DOCUMENT = "Document";

        /// <summary>
        /// Contains menu item count.
        /// </summary>
        private const int GROUP_MENUITEMS_COUNT = 4;
        #endregion

        #region Private members

        /// <summary>
        /// decides whether tabcontrol index can be updated
        /// </summary>
        internal bool m_canupdateindex = true;

        /// <summary>
        /// Represent object reference of Resource wrapper class.
        /// </summary>
        static ResourceWrapper wrapper = new ResourceWrapper();

        /// <summary>
        /// Stores the index of mouse moved item
        /// </summary>
        internal int m_mousemoveonitemindex = -1;

        /// <summary>
        /// represents the width of the tabbed item under mouse moved on header panel 
        /// </summary>
        internal double m_mouseonheaderpaneltabareawidth = 0;

        /// <summary>
        /// Stores the index of mouse moved item
        /// </summary>
        internal FrameworkElement m_mousemoveonitem = null;

        /// <summary>
        /// Contains generic templates.
        /// </summary>
        private readonly ResourceDictionary m_genericTempatesDictionary = new ResourceDictionary();

        /// <summary>
        /// Represents DocumentContainer
        /// </summary>
        private DocumentContainer m_documentContainer = null;

        /// <summary>
        /// Represents DocumentContainer InternalCloseItem
        /// </summary>
        private bool m_isInternalCloseItem = false;


        /// <summary>
        /// This varaible is a reference to language dictionary.
        /// </summary>
       // private static ResourceDictionary langDictionary;

        /// <summary>
        /// represent the flag flag
        /// </summary>
        //SU I78477
        //public bool m_Flag = true;
        public new bool m_Flag = true;
        //EU I78477

        /// <summary>
        /// represents textblock template
        /// </summary>
        private DataTemplate HeaderDataTemplate = null;

        /// <summary>
        /// represents textbox template
        /// </summary>
        private DataTemplate EditableDataTemplate = null;

        internal List<UIElement> TabPositionCache;

        #endregion

        #region Events
        /// <summary>
        /// Occurs when [last item closed]. 
        /// </summary>
        public static readonly RoutedEvent LastItemClosedEvent;

        /// <summary>
        /// Occurs when [last item closed].
        /// </summary>
        public event RoutedEventHandler LastItemClosed
        {
            add
            {
                AddHandler(LastItemClosedEvent, value);
            }

            remove
            {
                RemoveHandler(LastItemClosedEvent, value);
            }
        }
        #endregion

        #region Initialization
        /// <summary>
        /// Initializes static members of the <see cref="DocumentTabControl"/> class.
        /// </summary>
        static DocumentTabControl()
        {
            EnvironmentTest.ValidateLicense(typeof(DocumentTabControl));
            LastItemClosedEvent = EventManager.RegisterRoutedEvent("LastItemClosed", RoutingStrategy.Direct, typeof(RoutedEvent), typeof(DocumentTabControl));
            DefaultStyleKeyProperty.OverrideMetadata(
            typeof(DocumentTabControl),
            new FrameworkPropertyMetadata(typeof(DocumentTabControl)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentTabControl"/> class.
        /// </summary>
        public DocumentTabControl()
        {
            DragDropHelper.SetIsDragSource(this, true);
            DragDropHelper.SetIsDropTarget(this, true);
            AllowDragDrop = false;
            SetCommnadBindings();
            m_genericTempatesDictionary.Source = new Uri(GENERIC_TEMPATES, UriKind.RelativeOrAbsolute);
            SelectionChanged += new SelectionChangedEventHandler(DocumentTabControl_SelectionChanged);
            BindingUtils.SetRelativeBinding(this, DragDropHelper.ShowDragAdornerProperty, typeof(DocumentContainer), DocumentContainer.ShowDragAdornerProperty);
            BindingUtils.SetRelativeBinding(this, DragDropHelper.DragDropTemplateProperty, typeof(DocumentContainer), DocumentContainer.DragDropTemplateProperty);

            //langDictionary = new ResourceDictionary();
            //langDictionary.Source = new Uri(@"/Syncfusion.Tools.WPF;component/Themes/LangDictionary.xaml", UriKind.RelativeOrAbsolute);
            ResourceDictionary dict = new ResourceDictionary();

            dict.Source = new Uri(@"/Syncfusion.Tools.WPF;component/Framework/DocumentContainer/Themes/vista.aero.xaml", UriKind.RelativeOrAbsolute);
            HeaderDataTemplate = dict["HeaderDataTemplate"] as DataTemplate;
            EditableDataTemplate = dict["EditableDataTemplate"] as DataTemplate;

            this.Loaded += new RoutedEventHandler(DocumentTabControl_Loaded);
            BeforeLabelEdit += new BeforeLabelEditHandler(DocumentTabControl_BeforeLabelEdit);
            AfterLabelEdit += new AfterLabelEditHandler(DocumentTabControl_AfterLabelEdit);

            TabPositionCache = new List<UIElement>();
            this.Unloaded += new RoutedEventHandler(DocumentTabControl_Unloaded);
        }

        void DocumentTabControl_Unloaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= new RoutedEventHandler(DocumentTabControl_Loaded);
            BeforeLabelEdit -= new BeforeLabelEditHandler(DocumentTabControl_BeforeLabelEdit);
            AfterLabelEdit -= new AfterLabelEditHandler(DocumentTabControl_AfterLabelEdit);
            this.Unloaded -= new RoutedEventHandler(DocumentTabControl_Unloaded);

            if (Container != null && Container.DockingManager != null)
            {
                DockingManager owner = Container.DockingManager;
                if (owner.m_documentTabContrlsList.Contains(this))
                {
                    owner.m_documentTabContrlsList.Remove(this);
                }
            }
        }

        /// <summary>
        /// Handles the SelectionChanged event of the DocumentTabControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> instance containing the event data.</param>
        void DocumentTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                TabItemExt item = e.AddedItems[0] as TabItemExt;
                if (item != null)
                {
                    if(item.Parent!=null)                        
                    BindingUtils.SetBinding(item.Parent, Container, DocumentTabControl.IsLazyLoadedProperty, DocumentContainer.IsLazyLoadedProperty);
                    BindingUtils.SetBinding(item, this, TabItemExt.FocusVisualStyleProperty, TabControlExt.FocusVisualStyleProperty, BindingMode.OneWay);
                    if (this.SelectedContent != null)
                    {
                        BindingUtils.SetBinding(this.SelectedContent as DependencyObject, this, FrameworkElement.FocusVisualStyleProperty, TabControlExt.FocusVisualStyleProperty, BindingMode.OneWay);
                    }
                }
                if (Container != null)
                {
                    if(this.Items.Contains(item))
                    {
                        DockingManager docmanager = Container.DockingManager;
                        if (docmanager != null && docmanager.m_mousemoveonheaderpanel)
                        {
                            int index = (docmanager.m_mouseonheaderpanelindex < 0) ? this.m_mousemoveonitemindex : docmanager.m_mouseonheaderpanelindex;
                            if(index >=0 && index <this.Items.Count)
                            Container.ActiveDocument = this.Items[index] as UIElement;
                        }
                        else
                            Container.ActiveDocument = GetContent(item);
                        if (Container.ActiveDocument != null)
                            DockingManager.SetTabControl(Container.ActiveDocument, this as TabControlExt);
                    }
                }
            }
        }

        /// <summary>
        /// Handles the AfterLabelEdit event of the DocumentTabControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Syncfusion.Windows.Tools.Controls.AfterLabelEditEventArgs"/> instance containing the event data.</param>
        void DocumentTabControl_AfterLabelEdit(object sender, AfterLabelEditEventArgs e)
        {
            e.TabItem.HeaderTemplate = HeaderDataTemplate;
            ContentPresenter presenter = e.TabItem.Content as ContentPresenter;
            FrameworkElement element = presenter.Content as FrameworkElement;
            DockingManager.SetHeader(element, e.TabItem.Header);
        }

        /// <summary>
        /// Handles the BeforeLabelEdit event of the DocumentTabControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Syncfusion.Windows.Tools.Controls.BeforeLabelEditEventArgs"/> instance containing the event data.</param>
        void DocumentTabControl_BeforeLabelEdit(object sender, BeforeLabelEditEventArgs e)
        {
            e.TabItem.HeaderTemplate = EditableDataTemplate;
        }

        //private UIElement GetContentFromTabItem(TabItemExt item)
        //{
        //    if (item != null)
        //    {
        //        ContentPresenter presenter = item.Content as ContentPresenter;

        //    }
        //}

        /// <summary>
        /// Called when an internal process or application calls ApplyTemplate, which is used to build the current template's visual tree.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

       
        }

        internal TDILayoutPanel tdipanel = null;

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property == SkinStorage.VisualStyleProperty)
            {
                //if (!(m_documentContainer != null && m_documentContainer.IsInDockingManager))
                //{
                if (SkinStorage.GetVisualStyle(this) == "Blend")
                {
                    ResourceDictionary rd = new ResourceDictionary();
                    rd.Source = new Uri(@"/Syncfusion.Tools.WPF;component/Controls/TabControlExt/Themes/BlendStyle.xaml", UriKind.RelativeOrAbsolute);

                    this.Style = rd["BlendTabControlExtStyle"] as Style;
                }
                if (SkinStorage.GetVisualStyle(this) == "Office2003")
                    {
                        ResourceDictionary rd = new ResourceDictionary();
                        rd.Source = new Uri(@"/Syncfusion.Tools.WPF;component/Controls/TabControlExt/Themes/Office2003Style.xaml", UriKind.RelativeOrAbsolute);

                        this.Style = rd["Office2003TabControlExtStyle"] as Style;
                    }
                    if (SkinStorage.GetVisualStyle(this) == "Office2007Blue")
                    {
                        ResourceDictionary rd = new ResourceDictionary();
                        rd.Source = new Uri(@"/Syncfusion.Tools.WPF;component/Controls/TabControlExt/Themes/Office2007BlueStyle.xaml", UriKind.RelativeOrAbsolute);

                        this.Style = rd["Office2007BlueTabControlExtStyle"] as Style;
                    }
                    if (SkinStorage.GetVisualStyle(this) == "Office2007Black")
                    {
                        ResourceDictionary rd = new ResourceDictionary();
                        rd.Source = new Uri(@"/Syncfusion.Tools.WPF;component/Controls/TabControlExt/Themes/Office2007BlackStyle.xaml", UriKind.RelativeOrAbsolute);

                        this.Style = rd["Office2007BlackTabControlExtStyle"] as Style;
                    }
                    if (SkinStorage.GetVisualStyle(this) == "Office2007Silver")
                    {
                        ResourceDictionary rd = new ResourceDictionary();
                        rd.Source = new Uri(@"/Syncfusion.Tools.WPF;component/Controls/TabControlExt/Themes/Office2007SilverStyle.xaml", UriKind.RelativeOrAbsolute);

                        this.Style = rd["Office2007SilverTabControlExtStyle"] as Style;
                    }
                    if (SkinStorage.GetVisualStyle(this) == "Office2010Blue")
                    {
                        ResourceDictionary rd = new ResourceDictionary();
                        rd.Source = new Uri(@"/Syncfusion.Tools.WPF;component/Controls/TabControlExt/Themes/Office2010BlueStyle.xaml", UriKind.RelativeOrAbsolute);

                        this.Style = rd["Office2010BlueTabControlExtStyle"] as Style;
                    }
                    if (SkinStorage.GetVisualStyle(this) == "Office2010Black")
                    {
                        ResourceDictionary rd = new ResourceDictionary();
                        rd.Source = new Uri(@"/Syncfusion.Tools.WPF;component/Controls/TabControlExt/Themes/Office2010BlackStyle.xaml", UriKind.RelativeOrAbsolute);

                        this.Style = rd["Office2010BlackTabControlExtStyle"] as Style;
                    }
                    if (SkinStorage.GetVisualStyle(this) == "Office2010Silver")
                    {
                        ResourceDictionary rd = new ResourceDictionary();
                        rd.Source = new Uri(@"/Syncfusion.Tools.WPF;component/Controls/TabControlExt/Themes/Office2010SilverStyle.xaml", UriKind.RelativeOrAbsolute);

                        this.Style = rd["Office2010SilverTabControlExtStyle"] as Style;
                    }
                    if (SkinStorage.GetVisualStyle(this) == "ShinyRed")
                    {
                        ResourceDictionary rd = new ResourceDictionary();
                        rd.Source = new Uri(@"/Syncfusion.Tools.WPF;component/Controls/TabControlExt/Themes/ShinyRedStyle.xaml", UriKind.RelativeOrAbsolute);

                        this.Style = rd["ShinyRedTabControlExtStyle"] as Style;
                    }
                    if (SkinStorage.GetVisualStyle(this) == "ShinyBlue")
                    {
                        ResourceDictionary rd = new ResourceDictionary();
                        rd.Source = new Uri(@"/Syncfusion.Tools.WPF;component/Controls/TabControlExt/Themes/ShinyBlueStyle.xaml", UriKind.RelativeOrAbsolute);

                        this.Style = rd["ShinyBlueTabControlExtStyle"] as Style;
                    }
                    if (SkinStorage.GetVisualStyle(this) == "SyncOrange")
                    {
                        ResourceDictionary rd = new ResourceDictionary();
                        rd.Source = new Uri(@"/Syncfusion.Tools.WPF;component/Controls/TabControlExt/Themes/SyncOrangeStyle.xaml", UriKind.RelativeOrAbsolute);

                        this.Style = rd["SyncOrangeTabControlExtStyle"] as Style;
                    }
                    if (SkinStorage.GetVisualStyle(this) == "VS2010")
                    {
                        ResourceDictionary rd = new ResourceDictionary();
                        rd.Source = new Uri(@"/Syncfusion.Tools.WPF;component/Controls/TabControlExt/Themes/VS2010Style.xaml", UriKind.RelativeOrAbsolute);

                        this.Style = rd["VS2010TabControlExtStyle"] as Style;
                    }
                    if (SkinStorage.GetVisualStyle(this) == "Metro")
                    {
                        ResourceDictionary rd = new ResourceDictionary();
                        rd.Source = new Uri(@"/Syncfusion.Tools.WPF;component/Controls/TabControlExt/Themes/MetroStyle.xaml", UriKind.RelativeOrAbsolute);
                        this.Style = rd["MetroTabControlExtStyle"] as Style;
                    }
                    if (SkinStorage.GetVisualStyle(this) == "Transparent")
                    {
                        ResourceDictionary rd = new ResourceDictionary();
                        rd.Source = new Uri(@"/Syncfusion.Tools.WPF;component/Controls/TabControlExt/Themes/TransparentStyle.xaml", UriKind.RelativeOrAbsolute);
                        this.Style = rd["TransparentTabControlExtStyle"] as Style;
                    }
                //}
            }
            else if (e.Property == SkinManager.ActiveColorSchemeProperty)
            {
                Brush brush;

                if (e.NewValue != null)
                {
                    brush = Brushes.Red;
                    //brush = e.NewValue as Brush;
                    ResourceDictionary rd = new ResourceDictionary();
                    ResourceDictionary newwindow = new ResourceDictionary();
                    if (brush is SolidColorBrush)
                    {
                        Color color = (brush as SolidColorBrush).Color;
                        var mergd = new ResourceDictionary();
                        mergd.Add("TabControlExtStyle", rd["TabControlExtStyle"]);
                        newwindow.MergedDictionaries.Add(mergd);
                        //newwindow.Source = rd.Source;
                        newwindow.Source = new Uri(@"/Syncfusion.Tools.WPF;component//Framework/DocumentContainer/Themes/Office2007BlackStyle.xaml", UriKind.RelativeOrAbsolute);
                        newwindow = SkinColorScheme.ApplyCustomColorScheme(newwindow, color);
                        this.Style = newwindow["TabControlExtStyle"] as Style;
                    }
                }

            }
        }

        /// <summary>
        /// Handles the Loaded event of the DocumentTabControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void DocumentTabControl_Loaded(object sender, RoutedEventArgs e)
        {

            TabItemExt item = GetSelectedTabItem();
            
            if (tdipanel != null)
            {
                if (Items.Count < 1)
                {
                    if (tdipanel.ActiveTabControl != null)
                    {
                        if (tdipanel.ActiveTabControl.Items.Count < 1)
                        {
                            this.tdipanel.Visibility = Visibility.Collapsed;
                        }
                    }
                    else
                    {
                        this.tdipanel.Visibility = Visibility.Collapsed;
                    }
                }
                else
                {
                    this.tdipanel.Visibility = Visibility.Visible;
                }
            }

            if (item != null && item.DataContext != null)
            {

                if (SelectedContentPresenter != null)
                {
                    SelectedContentPresenter.DataContext = item.DataContext;
                    item.UpdateLayout();
                }
            }
            if (item != null)
                item.Focus();

            if (Container.MdiActiveElement != null)
            {
                foreach (var tabitem in Items)
                {
                    var tabItemExt = tabitem as TabItemExt;
                    if (tabItemExt != null)
                    {
                        var contentPresenter = tabItemExt.Content as ContentPresenter;
                        if (contentPresenter != null && contentPresenter.Content == Container.MdiActiveElement)
                            SelectedItem = tabitem;
                    }
                }
            }
            if (Container != null && Container.DockingManager != null)
            {
                DockingManager owner = Container.DockingManager;
                if (!owner.m_documentTabContrlsList.Contains(this))
                {
                    owner.m_documentTabContrlsList.Add(this);
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentTabControl"/> class.
        /// </summary>
        /// <param name="container">The container.</param>
        public DocumentTabControl(DocumentContainer container)
            : this()
        {
            Container = container;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the container.
        /// </summary>
        /// <value>The container.</value>
        public DocumentContainer Container
        {
            get
            {
                return m_documentContainer;
            }

            set
            {
                m_documentContainer = value;
                SetCommonOptions();
            }
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Switches the immediate.
        /// </summary>
        /// <param name="step">The direction.</param>
        public void SwitchImmediate(int step)
        {
            TabItemExt item = FindNextTabItem(SelectedIndex, step);

            if (item != null)
            {
                SelectedItem = item;
            }
        }

        /// <summary>
        /// Updates the after persist load.
        /// </summary>
        /// <param name="activeDocument">The active document.</param>
        public void UpdateAfterPersistLoad(UIElement activeDocument)
        {
            ReorderItems();
            SetActiveDocument(activeDocument);
        }

        /// <summary>
        /// Gets the ordered items.
        /// </summary>
        /// <returns>IList Control </returns>
        public IList<Control> GetOrderedItems()
        {
            int cnt = Items.Count;
            IList<Control> result = new List<Control>(cnt);
            int index = SelectedIndex;
            AddItemToList(result, index, cnt);
            AddItemToList(result, 0, index);

            return result;
        }


        /// <summary>
        /// Sets the active item.
        /// </summary>
        /// <param name="activeItem">The active item.</param>
        /// <returns>bool item Focus </returns>
        public bool SetActiveItem(FrameworkElement activeItem)
        {
            TabItemExt item = null;

            if (activeItem is TabItemExt)
            {
                item = activeItem as TabItemExt;
            }
            else
            {
                item = GetTabItem(activeItem);
            }

            if (item != null && SelectedItem != item)
            {

                SelectedItem = item;
                FrameworkElement element = null;
                if (DockingManager.IsChildOfDocking(item.Content as FrameworkElement))
                {
                    element = item.Content as FrameworkElement;
                }
                else if (DockingManager.IsChildOfDocking((item.Content as ContentPresenter).Content as FrameworkElement))
                {
                    element = (item.Content as ContentPresenter).Content as FrameworkElement;
                }

                DockingManager docking = DockingManager.ResolveManager(element);
                if (docking != null)
                {
                    ActiveWindowChangingEventArgs args = new ActiveWindowChangingEventArgs();                    
                    args.OldValue = docking.ActiveWindow;
                    args.NewValue = element;
                    if (args.OldValue != args.NewValue)
                    {
                        docking.FireActiveWindowChanging(args.NewValue, args);
                        if (!args.Cancel)
                        {
                            DockingManager.SetNewFocusedElement(args.NewValue);
                        }
                    }
                }
                item.Focus();
                return true;

            }

            return false;
        }
        /// <summary>
        /// Compares item content with element
        /// </summary>
        /// <param name="item">tabitem</param>
        /// <param name="element">uielement</param>
        /// <returns></returns>
        public static bool GetContentBool(ContentControl item, FrameworkElement element)
        {
            if (item != null)
            {
                FrameworkElement presenter = null;

                if (DockingManager.IsChildOfDocking(item.Content as FrameworkElement))
                {
                    presenter = item.Content as FrameworkElement;
                }
                else if (DockingManager.IsChildOfDocking((item.Content as ContentPresenter).Content as FrameworkElement))
                {
                    presenter = (item.Content as ContentPresenter).Content as FrameworkElement;
                }

                if (presenter != null)
                {
                    if (presenter == element)
                    {
                        return true;
                    }
                }
                else
                {
                    presenter = (item.Content as ContentPresenter).Content as FrameworkElement;

                    if (presenter != null && presenter == element)
                    {
                        return true;
                    }
                    else
                    {
                        presenter = item.Content as FrameworkElement;
                        if (presenter != null && presenter == element)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Gets the tab item.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>TabItemExt item</returns>
        public TabItemExt GetTabItem(UIElement element)
        {
            FrameworkElement ele = element as FrameworkElement;
            if (ele != null)
            {
                foreach (TabItemExt item in Items)
                {
                    if (GetContentBool(item, element as FrameworkElement) || (Container != null && Container.IsInDockingManager && this.ItemContainerGenerator.IndexFromContainer(item)==this.GetIndexOfElement(ele)))
                    {
                        return item;
                    }
                }
            }

            return null;
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            if (e.StylusDevice == null || e.StylusDevice != null)
            {
                base.OnMouseDown(e);
                this.IsFocus = true;
                if (this.SelectedItem != null && (this.SelectedItem is TabItemExt))
                {
                    (this.SelectedItem as TabItemExt).IsFocus = true;
                }
            }
        }

        /// <summary>
        /// Invoke on Items Collection Change
        /// </summary>
        /// <param name="e">NotifyCollectionChangedEventArgs</param>
        protected override void OnItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {

            base.OnItemsChanged(e);

            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (object element in e.NewItems)
                {
                    ContentControl cont = element as ContentControl;
                    if (cont != null)
                    {
                        UIElement contelement = GetContent(cont);
                        contelement.AllowDrop = true;
                    }
                }
            }
            //MT 2420 - Tab Order issue
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                case NotifyCollectionChangedAction.Remove:
                    UpdateIndexOrder();
                    break;
            }

        }

        internal void UpdateIndexOrder()
        {
            if (Container.IsInDockingManager && !Container.DockingManager.m_loadingState && Container.IsLoaded && m_canupdateindex)
            {
                for (int i = 0; i < Items.Count; i++)
                {
                    FrameworkElement item = GetContent((Items[i] as TabItemExt)) as FrameworkElement;
                    if (item != null && TDILayoutPanel.GetTDIIndex(item as DependencyObject) != i)
                    {
                        TDILayoutPanel.SetTDIIndex(item as DependencyObject, i);
                    }
                }
            }
        }
        /// <summary>
        /// Gets the content.
        /// </summary>
        /// <param name="item">The item ContentControl.</param>
        /// <returns>UIElement element</returns>
        public static UIElement GetContent(ContentControl item)
        {
            if (item != null)
            {

                FrameworkElement presenter = null;


                if (DockingManager.IsChildOfDocking(item.Content as FrameworkElement))
                {
                    presenter = item.Content as FrameworkElement;


                }
                else if (item.Content != null && DockingManager.IsChildOfDocking((item.Content as ContentPresenter).Content as FrameworkElement))
                {
                    presenter = (item.Content as ContentPresenter).Content as FrameworkElement;
                }

                if (presenter != null)
                {
                    //DocumentContainer.SetHeader(presenter, (item as TabItemExt).Header);
                    //DocumentContainer.SetHeaderTemplate(presenter, (item as TabItemExt).HeaderTemplate);
                    return presenter;
                }
                else if (item.Content != null)
                {
                    presenter = (item.Content as ContentPresenter).Content as FrameworkElement;

                    if (presenter == null)
                    {
                        presenter = item.Content as FrameworkElement;
                    }
                    //DocumentContainer.SetHeader(presenter, (item as TabItemExt).Header);
                    //DocumentContainer.SetHeaderTemplate(presenter, (item as TabItemExt).HeaderTemplate);
                    return presenter;
                }
            }

            return null;
        }
        #endregion

        #region IDisposable Members
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public new void Dispose()
        {
            ClearVisual();
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Removes the item.
        /// </summary>
        /// <param name="item">The item ContentControl.</param>
        internal override void RemoveItem(object item)
        {
            try
            {
                if (item is UIElement)
                {
                    UIElement element = (UIElement)item;
                    DetachHandlers(element);
                    TabItemExt tabItem = GetTabItem(element);
                    DockingManager owner = DockingManager.ResolveManager(element);
                    if (null != tabItem)
                    {
                        TabItemExt previousItem = null;
                        if (owner != null)
                        {
                            foreach (FrameworkElement oldactive in owner.lastacitive)
                            {
                                if (DockingManager.GetState(oldactive) != DockState.Hidden)
                                {
                                    previousItem = GetTabItem(oldactive);
                                    if (previousItem != null)
                                    {
                                        if (DockingManager.GetState(previousItem) == DockState.Document)
                                        {
                                            owner.ActivateWindow(previousItem.Name);
                                        }
                                        break;
                                    }
                                }
                            }
                        }
                        if (previousItem == null)
                        {
                            for (int i = SelectionStack.Count - 1; i >= 0; i--)
                            {
                                if (SelectionStack[i] != null)
                                {
                                    if ((SelectionStack[i] as TabItemExt) != tabItem)
                                    {
                                        if ((this.Items[0] as TabItemExt) != null && (this.Items[0] as TabItemExt) != tabItem)
                                        {
                                            previousItem = this.Items[0] as TabItemExt;
                                        }
                                        else if (this.Items.Count > 1 && (this.Items[1] as TabItemExt) != null)
                                        {
                                            previousItem = this.Items[1] as TabItemExt;
                                        }
                                        if (previousItem == null)
                                        {
                                            previousItem = SelectionStack[i] as TabItemExt;
                                        }
                                    }
                                }
                                if (previousItem != null && previousItem != tabItem && ((!m_documentContainer.IsInDockingManager) ? previousItem.IsVisible : true))
                                {
                                    break;
                                }
                            }
                        }

                        if (previousItem != null && ((!m_documentContainer.IsInDockingManager) ? previousItem.IsVisible : true))
                        {
                            if (!m_Flag && m_documentContainer.IsInDockingManager)
                            {
                                base.m_Flag = false;
                            }
                            this.SelectedItem = previousItem;
                            //this.IsTabGroupFocus = false;
                            if (!m_Flag && m_documentContainer.IsInDockingManager)
                            {
                                base.m_Flag = true;
                            }
                            FrameworkElement element2 = null;
                            if (previousItem.Content != null && DockingManager.IsChildOfDocking(previousItem.Content as FrameworkElement))
                            {
                                element2 = previousItem.Content as FrameworkElement;
                            }
                            else if (previousItem.Content != null)
                            {
                                element2 = (previousItem.Content as ContentPresenter).Content as FrameworkElement;
                            }
                            DockingManager docking = DockingManager.ResolveManager(element2);
                            if (docking != null)
                            {
                                ActiveWindowChangingEventArgs args = new ActiveWindowChangingEventArgs();
                                if (args.OldValue != args.NewValue && docking != null)
                                {
                                    docking.FireActiveWindowChanging(args.NewValue, args);
                                    if (!args.Cancel)
                                    {
                                        DockingManager.SetNewFocusedElement(args.NewValue);
                                    }
                                }
                            }
                        }
                        if (!m_Flag && m_documentContainer.IsInDockingManager)
                        {
                            base.m_Flag = false;
                        }
                        Items.Remove(tabItem);
                        if (this.TabPositionCache != null && this.TabPositionCache.Contains(GetContent(tabItem)))
                        {
                            this.TabPositionCache.Remove(GetContent(tabItem));
                        }

                        if (m_documentContainer != null && (m_documentContainer.ILayoutPanel != null) && ((m_documentContainer.ILayoutPanel as TDILayoutPanel).ActiveTabControl != null))
                        {
                            element = null;

                            if (((m_documentContainer.ILayoutPanel as TDILayoutPanel).ActiveTabControl).Items.Contains(tabItem))
                                ((m_documentContainer.ILayoutPanel as TDILayoutPanel).ActiveTabControl).Items.Remove(tabItem);
                        }

                        DragDropHelper helper = DragDropHelper.GetInstance();
                        if (helper != null)
                        {
                            helper.SetNull();
                        }

                        if (!m_Flag && m_documentContainer.IsInDockingManager)
                        {
                            base.m_Flag = true;
                        }
                        this.SelectedItem = previousItem;
                        SelectionStack.Remove(tabItem);
                        if (previousItem == null && SelectionStack.Count == 2)
                        {

                            TabItemExt remainItem = SelectionStack[1] as TabItemExt;
                            if ((this.Items[0] as TabItemExt) != null && (this.Items[0] as TabItemExt) != tabItem)
                            {
                                remainItem = this.Items[0] as TabItemExt;
                            }
                            else if (this.Items.Count > 1 && (this.Items[1] as TabItemExt) != null)
                            {
                                remainItem = this.Items[1] as TabItemExt;
                            }
                            this.SelectedItem = this.Items[0];
                            this.IsTabGroupFocus = false;
                            remainItem.IsTabGroupFocus = false;
                        }

                        ContentPresenter presenter = (ContentPresenter)tabItem.Content;
                        presenter.ClearValue(ContentPresenter.ContentProperty);
                        tabItem.ClearValue(TabItemExt.ContentProperty);
                        tabItem.ContextMenuItems.Clear();

                        if (!m_documentContainer.IsInDockingManager)
                        {
                            //element.ClearValue(TDILayoutPanel.TDIGroupOrientationProperty);
                            //element.ClearValue(TDILayoutPanel.WayOfTDIGroupProperty);
                        }
                        tabItem.PreviewMouseLeftButtonUp -= DockingManager.OnTabItemExtPreviewMouseLeftButtonUp;
                    }
                    else
                    {
                        base.RemoveItem(item);
                    }
                    item = null;
                }
            }
            catch
            { }
        }

        /// <summary>
        /// Removes the item at specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        internal override void RemoveItemAt(int index)
        {
            if (index >= 0 && index < Items.Count)
            {
                TabItemExt tabItem = (TabItemExt)Items[index];
                RemoveItem(GetContent(tabItem));
            }
            else
            {
                base.RemoveItemAt(index);
            }
        }

        /// <summary>
        /// Adds the item.
        /// </summary>
        /// <param name="item">The item ContentControl.</param>
        internal override void AddItem(object item)
        {
            if (item is UIElement)
            {
                UIElement element = GetContent((ContentControl)item);
                InsertItemExt(element, 0);
            }
            else
            {
                base.AddItem(item);
            }
        }

        /// <summary>
        /// Adds the content of the item.
        /// </summary>
        /// <param name="item">The item ContentControl.</param>
        internal void AddItemContent(object item)
        {
            if (item is UIElement)
            {
                UIElement element = (UIElement)item;
                element.SetValue(TDILayoutPanel.WayOfTDIGroupProperty, GetValue(TDILayoutPanel.WayOfTDIGroupProperty));
                InsertItemExt(element, 0);
            }
            else
            {
                base.AddItem(item);
            }
        }

        /// <summary>
        /// Get value indicates whether or not this control contains the element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns> bool presenter Document</returns>
        internal bool ContainsItem(UIElement element)
        {
            foreach (TabItem item in Items)
            {
                FrameworkElement presenter = (FrameworkElement)item.Content;

                if (element == presenter)
                {
                    return true;
                }
                else
                {

                    presenter = (item.Content as ContentPresenter).Content as FrameworkElement;
                    if (presenter == element)
                    {
                        return true;
                    }

                }
            }
            return false;
        }

        /// <summary>
        /// Valids the content of the item.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        internal FrameworkElement ValidItemContent(UIElement element)
        {
            foreach (TabItem item in Items)
            {
                FrameworkElement presenter = (FrameworkElement)item.Content;

                if (element == presenter)
                {
                    return presenter;
                }
                else
                {

                    presenter = (item.Content as ContentPresenter).Content as FrameworkElement;
                    if (presenter == element)
                    {
                        return presenter;
                    }

                }
            }
            return null;
        }

        /// <summary>
        /// Updates the index.
        /// </summary>
        /// <param name="index">The index.</param>
        internal void UpdateIndex(int index,FrameworkElement element)
        {
            if (index != -1 && (index <= this.Items.Count || m_canupdateindex))
            {
                m_canupdateindex = false;
                FrameworkElement tab = null;
                int actualindex = this.GetIndexOfElement(element);
                if (actualindex > -1 && actualindex < this.Items.Count)
                {
                    tab = this.Items[actualindex] as FrameworkElement;
                    DockingManager docmanager = DockingManager.ResolveManager(element as UIElement);
                    if (docmanager != null)
                    {
                        if (docmanager.m_mousemoveonheaderpanel)
                        {
                            docmanager.m_mousemoveonTabpaneladv = false;
                            this.m_mousemoveonitem = null;
                        }
                    }
                    if (actualindex != -1 && actualindex != index)
                    {
                        if (TabPositionCache.Count > actualindex)
                        {
                            this.Items.RemoveAt(actualindex);
                            this.TabPositionCache.RemoveAt(actualindex);
                        }
                        if (this.Items.Count - 1 >= index)
                        {
                            if (this.Items.Contains(tab))
                                this.Items.Remove(tab);
                            this.InsertItem(index, tab);
                            if (index <= this.TabPositionCache.Count)
                                this.TabPositionCache.Insert(index, element as UIElement);
                        }
                        else
                        {
                            this.Items.Add(tab);
                            this.TabPositionCache.Add(element as UIElement);
                        }
						UpdateTabPositionCache(TabPositionCache);
                    }
                    this.UpdateLayout();
                }
                m_canupdateindex = true;
            }
        }

        /// <summary>
        /// Gets the index of element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>int returnValue</returns>
        internal int GetIndexOfElement(FrameworkElement element)
        {
            int returnValue = -1;

            for (int i = 0, cnt = Items.Count; i < cnt; ++i)
            {
                TabItemExt item = (TabItemExt)Items[i];
                ContentPresenter presenter = (ContentPresenter)item.Content;

                if (presenter.Content == element)
                {
                    returnValue = i;
                    break;
                }
            }

            return returnValue;
        }

        /// <summary>
        /// Inserts the item ext.
        /// </summary>
        /// <param name="item">The item UIElement.</param>
        /// <param name="insertIndex">Index of the insert.</param>
        internal void InsertItemExt(UIElement item, int insertIndex)
        {
            if (DockingManager.GetState(item) != DockState.Hidden)
            {
                InserItemEx(item, insertIndex);
                Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Resets the items.
        /// </summary>
        /// <param name="items">The items.</param>
        internal void ResetItems(IEnumerable items)
        {
            ClearVisual();

            foreach (object item in items)
            {
                UIElement element = null;

                if (item is UIElement)
                {
                    element = item as UIElement;
                }
                else
                {
                    element = new ContentControl();
                    (element as ContentControl).DataContext = item;
                }

                if (Container != null && Container.AddTabDocumentAtLast)
                {
                    TabLayoutPanel.AddAtLast = true;
                    int insertindex = Items.Count;
                    InsertItemExt(element, insertindex);
                    DockingManager.SetNewFocusedElement(element as FrameworkElement);
                }
                else
                {
                    InsertItemExt(element, 0);
                    SelectedIndex = 0;
                    DockingManager.SetNewFocusedElement(element as FrameworkElement);
                }
            }
        }

        /// <summary>
        /// Updates the menu items
        /// </summary>
        /// <param name="tabItem">The tab item.</param>
        internal void UpdateMenuItems(TabItemExt tabItem)
        {
            tabItem.ContextMenuItems.Clear();
            UIElement element = GetContent(tabItem);
            SetMenuItems(tabItem, element);
        }

        /// <summary>
        /// Invoked when a KeyDown attached routed event occurs.
        /// </summary>
        /// <param name="e">Event data</param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if ((e.Key == Key.Tab || e.Key == Key.Q) &&
                (e.KeyboardDevice.Modifiers == ModifierKeys.Control))
            {
               e.Handled = true;
            }
            if ((e.Key == Key.Tab || e.Key == Key.Q) &&
                (e.KeyboardDevice.Modifiers == (ModifierKeys.Control | ModifierKeys.Shift)))
            {
               e.Handled = true;
            }
            if (!e.Handled && m_documentContainer != null && m_documentContainer.DockingManager != null && m_documentContainer.DockingManager.SwitchMode != SwitchMode.None)
                base.OnKeyDown(e);
        }

        /// <summary>
        /// Calls when TabItem closes
        /// </summary>
        /// <param name="e">Event arguments</param>
        protected override void FireOnCloseButtonClick(CloseTabEventArgs e)
        {
            base.FireOnCloseButtonClick(e);

            if (e.TargetTabItem is TabItemExt)
            {
                UIElement element = GetContent(e.TargetTabItem);
                if (element != null)
                {
                    CloseButtonEventArgs args = new CloseButtonEventArgs(element);
                    Container.FireCloseButtonClick(args);
                    if (!args.Cancel)
                    {
                        SetHiddenDockState(e.TargetTabItem);

                        DockingManager owner = DockingManager.ResolveManager(element);
                        if (owner != null)
                        {
                            owner.SetFocus((FrameworkElement)element);
                        }
                        CloseTabEventArgs closedargs = new CloseTabEventArgs(e.TargetTabItem);
                        Container.FireDocumentClosed(closedargs);
                    }
                    else
                    {
                        e.Cancel = true;
                    }
                }
            }

            FireOnLastItemClosed();
        }

        /// <summary>
        /// Calls when other TabItems close
        /// </summary>
        /// <param name="e">Event arguments</param>
        protected override void FireOnCloseOtherTabs(CloseTabEventArgs e)
        {
            base.FireOnCloseOtherTabs(e);

            if (!e.Cancel)
            {
                TabItem targetItem = e.TargetTabItem;

                for (int i = Items.Count - 1; i > -1; --i)
                {
                    TabItemExt item = null;
                    try
                    {
                        item = Items[i] as TabItemExt;
                    }
                    catch
                    { }

                    if (null != item && targetItem != item && CanCloseTabItem(item) && !CanceledClosed(item))
                    {
                        SetHiddenDockState(item);
                    }
                }

                FireOnLastItemClosed();
            }
        }

        /// <summary>
        /// Calls when all TabItems close
        /// </summary>
        /// <param name="e">Event arguments</param>
        protected override void FireOnCloseAllTabs(CloseTabEventArgs e)
        {
            base.FireOnCloseAllTabs(e);

            if (!e.Cancel)
            {
                for (int i = Items.Count - 1; i > -1; --i)
                {
                    try
                    {
                        TabItemExt item = Items[i] as TabItemExt;

                        if (null != item && CanCloseTabItem(item) && !CanceledClosed(item))
                        {
                            SetHiddenDockState(item);
                        }
                    }
                    catch
                    { }
                }

                FireOnLastItemClosed();
            }
        }

        /// <summary>
        /// Represents the method that will handle the CanExecute event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        /// <property name="flag" value="Finished"/>
        protected override void CanProcessCloseTabItemCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            TabItemExt item = ActivatedItem as TabItemExt;
            SetCanExecuteClose(e, item);
        }

        /// <summary>
        /// Represents the method that will handle the CanExecute event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        /// <property name="flag" value="Finished"/>
        protected override void CanProcessCloseCurrentTabItemCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            TabItemExt item = SelectedItem as TabItemExt;
            SetCanExecuteClose(e, item);
        }

        /// <summary>
        /// Represents the method that will handle the CanExecute event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        /// <property name="flag" value="Finished"/>
        protected override void CanProcessCloseTabsCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            base.CanProcessCloseTabsCommand(sender, e);

            if (e.CanExecute)
            {
                bool isEnabledCloseAllItem = false;
                bool isEnabledCloseOtherItem = false;
                TabItemExt selItem = ActivatedItem;
                bool bCloseAll = (string)e.Parameter == "All";

                foreach (object element in Items)
                {
                    TabItemExt item = GetTabItem(element);
                    ContentPresenter presenter = item.Content as ContentPresenter;

                    if (presenter != null && presenter.Content!=null && DockingManager.GetCanClose(presenter.Content as DependencyObject))
                    {
                        isEnabledCloseAllItem = true;

                        if (selItem != item)
                        {
                            isEnabledCloseOtherItem = true;
                        }
                    }
                }

                if (bCloseAll && isEnabledCloseAllItem || !bCloseAll && isEnabledCloseOtherItem)
                {
                    e.CanExecute = true;
                }
                else
                {
                    e.CanExecute = false;
                }
            }
        }

        /// <summary>
        /// Adds the item to list.
        /// </summary>
        /// <param name="result">The result UIElement.</param>
        /// <param name="start">The start IList Control .</param>
        /// <param name="end">The end IList Control .</param>
        private void AddItemToList(IList<Control> result, int start, int end)
        {
            if (start >= 0)
            {
                for (int i = start; i < end; ++i)
                {
                    result.Add((Control)Items[i]);
                }
            }
        }

        /// <summary>
        /// Inserts the item ext.
        /// </summary>
        /// <param name="collection">The collection UIElement.</param>
        /// <param name="item">The item UIElement.</param>
        /// <param name="insertIndex">Index of the insert.</param>
        private void InsertItemExt(ObservableCollection<TabItemExt> collection, UIElement item, int insertIndex)
        {
            if (DockingManager.GetState(item) != DockState.Hidden)
            {
                TabItemExt tabItem = CreateTabItem(item);
                AttachHandlers(item);
                collection.Insert(insertIndex, tabItem);
                Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Fires the on last item closed.
        /// </summary>
        private void FireOnLastItemClosed()
        {
            if (LastItemClosedEvent != null && Items.Count < 1)
            {
                RaiseEvent(new RoutedEventArgs(LastItemClosedEvent, this));
            }
        }

        /// <summary>
        /// States the change handler.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        protected void StateChangeHandler(object sender, RoutedEventArgs e)
        {
            if (m_isInternalCloseItem)
            {
                return;
            }

            UIElement source = (UIElement)e.OriginalSource;
            DockState dockState = DockingManager.GetState(source);

            if (dockState != DockState.Document)
            {
                RemoveItem(source);
            }
            else if (!IsItemExisted(source))
            {
                InsertItemExt(source, 0);
            }
        }

        /// <summary>
        /// Attaches the handlers.
        /// </summary>
        /// <param name="child">The child.</param>
        private void AttachHandlers(IInputElement child)
        {
            if (child != null)
            {
                child.RemoveHandler(DockingManager.DockStateChangedEvent, new RoutedEventHandler(StateChangeHandler));
                child.AddHandler(DockingManager.DockStateChangedEvent, new RoutedEventHandler(StateChangeHandler));
            }
        }

        /// <summary>
        /// Detaches the handlers.
        /// </summary>
        /// <param name="child">The child.</param>
        private void DetachHandlers(IInputElement child)
        {
            if (child != null)
            {
                child.RemoveHandler(DockingManager.DockStateChangedEvent, new RoutedEventHandler(StateChangeHandler));
            }
        }

        /// <summary>
        /// Determines whether [is item existed] [the specified item].
        /// </summary>
        /// <param name="item">The item UIElement.</param>
        /// <returns>
        /// <c>true</c> if [is item existed] [the specified item]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsItemExisted(UIElement item)
        {
            bool exist = false;

            foreach (TabItem tabItem in Items)
            {
                UIElement element = GetContent(tabItem);

                if (element != null && item == element)
                {
                    exist = true;
                    break;
                }
            }

            return exist;
        }

        /// <summary>
        /// Sets the common options for each item.
        /// </summary>
        private void SetCommonOptions()
        {
            SelectedIndex = GetCorrectIndex();
        }

        /// <summary>
        /// Gets the index of the correct.
        /// </summary>
        /// <returns>The correct visible index</returns>
        private int GetCorrectIndex()
        {
            int index = 0;

            if (Container != null)
            {
                index = Container.Items.IndexOf(Container.ActiveDocument);

                for (int i = 0, cnt = index; i < cnt; ++i)
                {
                    UIElement item = (UIElement)Container.Items[i];

                    if (DockingManager.GetState(item) == DockState.Hidden)
                    {
                        index--;
                    }
                }
            }

            return index;
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Controls.Primitives.Selector.SelectionChanged"/> routed event.
        /// </summary>
        /// <param name="e">Provides data for <see cref="T:System.Windows.Controls.SelectionChangedEventArgs"/>.</param>
        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);
            TabControlExt previoustab = Container.m_previoustabcontrol;
            TabItemExt item = GetSelectedTabItem();
            TabItemExt olditem = null, newitem = null;
            if (e.RemovedItems != null && e.RemovedItems.Count > 0)
            {
                olditem = e.RemovedItems[0] as TabItemExt;

                if (IsLoaded && olditem != null
                    && (olditem.Content as ContentPresenter) != null
                    && (olditem.Content as ContentPresenter).Content != null)
                {
                    DependencyObject element = (olditem.Content as ContentPresenter).Content as DependencyObject;
                    if (TDILayoutPanel.GetIsSelected(element) != false) 
                        TDILayoutPanel.SetIsSelected(element, false);
                }
            }
            if (e.AddedItems != null && e.AddedItems.Count > 0)
            {
                newitem = e.AddedItems[0] as TabItemExt;

                if (IsLoaded && newitem != null
                    && (newitem.Content as ContentPresenter) != null
                    && (newitem.Content as ContentPresenter).Content != null)
                {
                    DependencyObject newelement = (newitem.Content as ContentPresenter).Content as DependencyObject;
                    if (TDILayoutPanel.GetIsSelected(newelement) != true) 
                        TDILayoutPanel.SetIsSelected(newelement, true);
                }
            }

            if (newitem != null)
            {
                if (newitem.TabControlParent != null)
                {
                    bool check = true;
                    DocumentTabControl tabcontrolparent = VisualUtils.FindAncestor((Visual)newitem.TabControlParent, typeof(DocumentTabControl)) as DocumentTabControl;
                    DocumentTabControl m_previoustabcontrolparent = VisualUtils.FindAncestor((Visual)previoustab, typeof(DocumentTabControl)) as DocumentTabControl;
                    if (tabcontrolparent != null && m_previoustabcontrolparent != null)
                    {
                        if (!m_previoustabcontrolparent.Equals(tabcontrolparent))
                        {
                            check = false;
                        }
                    }
                    if (previoustab != null && previoustab != newitem.TabControlParent && check)
                    {
                        previoustab.IsTabGroupFocus = false;
                        if (previoustab.Items.Count > 0)
                        {
                            if (previoustab.SelectedItem != null && (previoustab.SelectedItem as TabItemExt) != null)
                            {
                                (previoustab.SelectedItem as TabItemExt).IsTabGroupFocus = false;
                            }
                        }
                    }
                    newitem.IsTabGroupFocus = true;
                    newitem.TabControlParent.IsTabGroupFocus = true;
                    previoustab = newitem.TabControlParent;
                }
            }
            if (olditem != null)
            {
                if (olditem.TabControlParent != null && newitem != null && newitem.TabControlParent != null)
                {
                    if (olditem.TabControlParent != newitem.TabControlParent)
                    {
                        olditem.IsTabGroupFocus = false;
                        olditem.TabControlParent.IsTabGroupFocus = false;
                    }
                }
                if (olditem.TabControlParent != null && newitem == null)
                {
                    olditem.IsTabGroupFocus = false;
                    olditem.TabControlParent.IsTabGroupFocus = false;
                }
                ContentPresenter headpresenter = olditem.Template.FindName("Content", olditem) as ContentPresenter;
                if (headpresenter != null && newitem != null)
                {
                    TextElement.SetFontWeight(headpresenter, FontWeights.Normal);
                }
            }

            if (item != null && item.DataContext != null)
            {

                if (SelectedContentPresenter != null)
                {
                    SelectedContentPresenter.DataContext = item.DataContext;
                    if ((SelectedContentPresenter.Content as ContentPresenter) != null)
                    {
                        (SelectedContentPresenter.Content as ContentPresenter).Loaded += new RoutedEventHandler(DocumentTabControl_Loaded);
                    }
                    item.UpdateLayout();
                    SelectedContentPresenter.UpdateLayout();
                }
            }

            UpdateSelectedContent(item, SelectedContentPresenter);
        }

        /// <summary>
        /// Updates the content of the selected.
        /// </summary>
        /// <param name="tabItem">The tab item.</param>
        /// <param name="scp">The SCP.</param>
        private void UpdateSelectedContent(TabItemExt tabItem, ContentPresenter scp)
        {

            if (tabItem != null)
            {
                FrameworkElement visualParent = VisualTreeHelper.GetParent(tabItem) as FrameworkElement;
                if (scp != null)
                {
                    scp.HorizontalAlignment = tabItem.HorizontalContentAlignment;
                    scp.VerticalAlignment = tabItem.VerticalContentAlignment;
                }
            }
        }

        /// <summary>
        /// Creates the tab item.
        /// </summary>
        /// <param name="item">The item UIElement.</param>
        /// <returns>The created tab item.</returns>
        private TabItemExt CreateTabItem(UIElement item)
        {
            ContentPresenter presenter = new ContentPresenter
            {
                Content = item,
            };
            if ((item as FrameworkElement).DataContext != null)
            {
                presenter.DataContext = (item as FrameworkElement).DataContext;
            }

            TabItemExt tabItem = new TabItemExt
            {
                Content = presenter,
            };



            if ((item as FrameworkElement).DataContext != null)
            {
                tabItem.DataContext = (item as FrameworkElement).DataContext;
                // (tabItem.Content as FrameworkElement).DataContext = (item as FrameworkElement).DataContext;
                //DocumentContainer.SetHeader(item, tabItem.Header);
                //DocumentContainer.SetHeaderTemplate(item, tabItem.HeaderTemplate);
            }

            Style style = null;

            style = (Style)DocumentContainer.GetDocumentTabItemStyle(item) ??
                                (Style)DocumentContainer.GetDocumentTabItemStyle(Container);

            if (m_documentContainer.IsInDockingManager)
            {
                //SetTabItemExtBinding(item, tabItem, DockingManager.HeaderProperty, HeaderedContentControl.HeaderProperty);
                //SetTabItemExtBinding(item, tabItem, DockingManager.HeaderTemplateProperty, HeaderedContentControl.HeaderTemplateProperty);
                Style tabstyle = (Style)DockingManager.GetDocumentTabItemStyle(item);
                if (tabstyle == null)
                {
                    tabstyle = (Style)DockingManager.GetDocumentTabItemStyle(m_documentContainer.FlipParent as DockingManager);
                    if (tabstyle == null)
                    {
                        tabstyle = (Style)DockingManager.GetDocumentTabItemStyle(m_documentContainer.FlipParent as DockingManager);
                    }
                }

                style = tabstyle;

                //style = (Style)DockingManager.GetDocumentTabItemStyle(item) ??
                //          (Style)DockingManager.GetDocumentTabItemStyle(m_documentContainer.FlipParent as DockingManager);
            }

            ImageBrush brush = null;

            if (m_documentContainer.IsInDockingManager)
            {
                brush = (ImageBrush)DockingManager.GetIcon(item);
            }

            if (brush == null)
            {
                brush = (ImageBrush)DocumentContainer.GetIcon(item) ??
                            (ImageBrush)DocumentContainer.GetIcon(Container);
            }

            if (style != null)
            {
                tabItem.Style = style;
                SetterBaseCollection collection = tabItem.Style.Setters;

                bool _headerflag = false;
                bool _headertempFlag = false;
                bool _imageFlag = false;
                bool _imageAlignmentFlag = false;
                ImageSource imgsource = null;
                ImageAlignment imgalign = ImageAlignment.LeftOfText;
                foreach (Setter setter in collection)
                {
                    if (setter.Property.Equals(TabItemExt.HeaderProperty))
                    {
                        _headerflag = true;
                    }

                    if (setter.Property.Equals(TabItemExt.HeaderTemplateProperty))
                    {
                        _headertempFlag = true;
                    }

                    if (setter.Property.Equals(ContentPresenter.ContentProperty))
                    {
                        if (setter.Value is Binding)
                        {
                            (tabItem.Content as FrameworkElement).SetBinding(ContentPresenter.ContentProperty, setter.Value as Binding);
                        }
                        else
                        {
                            (tabItem.Content as ContentPresenter).Content = (bool)setter.Value;
                        }
                    }

                    if (setter.Property.Equals(ContentPresenter.ContentTemplateProperty))
                    {
                        if (setter.Value is Binding)
                        {
                            (tabItem.Content as ContentPresenter).SetBinding(ContentPresenter.ContentTemplateProperty, setter.Value as Binding);
                        }
                        else
                        {
                            if (tabItem.Content is ContentPresenter)
                            {
                                (tabItem.Content as ContentPresenter).ContentTemplate = (DataTemplate)setter.Value;
                            }
                            else if (tabItem.Content is ContentControl)
                            {
                                (tabItem.Content as ContentControl).ContentTemplate = (DataTemplate)setter.Value;
                            }
                        }
                    }

                    if (setter.Property.Equals(ContentPresenter.ContentTemplateSelectorProperty))
                    {
                        if (setter.Value is Binding)
                        {
                            (tabItem.Content as ContentPresenter).SetBinding(ContentPresenter.ContentTemplateSelectorProperty, setter.Value as Binding);
                        }
                        else
                        {
                            if (tabItem.Content is ContentPresenter)
                            {
                                (tabItem.Content as ContentPresenter).ContentTemplateSelector = (DataTemplateSelector)setter.Value;
                            }
                            else if (tabItem.Content is ContentControl)
                            {
                                (tabItem.Content as ContentControl).ContentTemplateSelector = (DataTemplateSelector)setter.Value;
                            }
                        }
                    }

                    if (setter.Property.Equals(TabItemExt.ImageProperty))
                    {
                        _imageFlag = true;
                        imgsource = setter.Value as ImageSource;
                    }

                    if (setter.Property.Equals(TabItemExt.ImageAlignmentProperty))
                    {
                        _imageAlignmentFlag = true;
                        imgalign = (ImageAlignment)setter.Value;
                    }
                }

                if (!_headerflag)
                {
                    SetTabItemExtBinding(item, tabItem, DocumentContainer.HeaderProperty, HeaderedContentControl.HeaderProperty);
                }

                if (!_headertempFlag)
                {
                    SetTabItemExtBinding(item, tabItem, DocumentContainer.HeaderTemplateProperty, HeaderedContentControl.HeaderTemplateProperty);
                }

                if (!_imageFlag)
                {
                    if (brush != null)
                    {
                        GeometryDrawing geometry = new GeometryDrawing(brush, null, new RectangleGeometry(new Rect(0, 0, 16, 16)));
                        DrawingImage image = new DrawingImage(geometry);
                        tabItem.Image = image;
                    }

                    if (!_imageAlignmentFlag)
                    {
                        tabItem.ImageAlignment = ImageAlignment.LeftOfText;
                    }
                }
            }
            else
            {
                SetTabItemExtBinding(item, tabItem, DocumentContainer.HeaderProperty, HeaderedContentControl.HeaderProperty);
                SetTabItemExtBinding(item, tabItem, DocumentContainer.HeaderTemplateProperty, HeaderedContentControl.HeaderTemplateProperty);
                if (brush != null)
                {
                    GeometryDrawing geometry = new GeometryDrawing(brush, null, new RectangleGeometry(new Rect(0, 0, 16, 16)));
                    DrawingImage image = new DrawingImage(geometry);
                    tabItem.Image = image;
                    tabItem.ImageAlignment = ImageAlignment.LeftOfText;
                }
            }


            BindingUtils.SetBinding(tabItem, item, TabItemExt.ItemToolTipProperty, DocumentContainer.TabCaptionToolTipProperty);
            return tabItem;
        }

        /// <summary>
        /// Sets the tab item ext binding.
        /// </summary>
        /// <param name="sourceElement">The source element.</param>
        /// <param name="destElement">The dest element.</param>
        /// <param name="source">The source UIElement.</param>
        /// <param name="dest">The dest UIElement.</param>
        private void SetTabItemExtBinding(UIElement sourceElement, FrameworkElement destElement, DependencyProperty source, DependencyProperty dest)
        {
            Binding binding = new Binding();
            binding.Source = sourceElement;
            binding.Mode = BindingMode.OneWay;
            binding.Path = new PropertyPath(source);
            destElement.SetBinding(dest, binding);
        }

        internal void UpdateTabPositionCache(List<UIElement> TabPositionCache)
        {
            foreach (FrameworkElement element in TabPositionCache)
            {
                DockingManager.SetDocumentTabOrderIndex(element, TabPositionCache.IndexOf(element));
            }
        }

        /// <summary>
        /// Inserts the item ex.
        /// </summary>
        /// <param name="item">The item UIElement.</param>
        /// <param name="insertIndex">Index of the insert.</param>
        private void InserItemEx(UIElement item, int insertIndex)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            DocumentContainer.CheckNameOfElement((FrameworkElement)item);
            TabItemExt tabItem = CreateTabItem(item);

            tabItem.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(DockingManager.OnTabItemExtPreviewMouseLeftButtonDown);
            tabItem.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(DockingManager.OnTabItemExtPreviewMouseLeftButtonUp);
#if !SyncfusionFramework3_5
            //tabItem.PreviewTouchDown += DockingManager.tabItemext_PreviewTouchDown;
            //tabItem.PreviewTouchUp += DockingManager.tabItemext_PreviewTouchUp;
#endif

            if (TabPositionCache.Contains(item))
            {
                insertIndex = FindNearestIndex(Items, TabPositionCache, item);
            }
            DockingManager.SetTabControl(item as DependencyObject, this as TabControlExt);
            DockingManager docmanager = Container.DockingManager;
            if (docmanager != null && docmanager.m_mousemoveonheaderpanel)
            {
                int index = (docmanager.m_mouseonheaderpanelindex < 0) ? this.m_mousemoveonitemindex : docmanager.m_mouseonheaderpanelindex;
                if (index >=0 && this.Items.Count >= index)
                {
                    insertIndex = index;
                }
            }
            Items.Insert(insertIndex, tabItem);

            if (!TabPositionCache.Contains(item))
            {
                TabPositionCache.Insert(insertIndex, item);
                if ((Container.IsInDockingManager &&  (Container.DockingManager!=null) && !Container.DockingManager.m_loadingState))
                {
                    UpdateTabPositionCache(TabPositionCache);
                }
            }

            SelectedItem = tabItem;
            SetActiveItem(tabItem);
            SetMenuItems(tabItem, item);
            AttachHandlers(item);

            if (IsLoaded)
            {
                Container.ActiveDocument = item;
            }
        }

        private int FindNearestIndex(ItemCollection items, List<UIElement> tabPositionCache, UIElement item)
        {
            int index = tabPositionCache.IndexOf(item);
            if(items.Count == 0 || index == 0)
                return 0;
            if(index == tabPositionCache.Count - 1)
                return items.Count;

            object previousElement = tabPositionCache[index - 1];
            int previousIndex = index - 1;
            object nextElement = tabPositionCache[index + 1];
            int nextIndex = index + 1;

            bool stopIterating = false;

            while (!stopIterating)
            {
                if (ContainsItem(nextElement as UIElement))
                {
                    stopIterating = true;
                    return items.IndexOf(GetTabItem(nextElement as FrameworkElement));
                }

                if (nextIndex == tabPositionCache.Count - 1)
                {
                    stopIterating = true;
                    continue;
                }

                nextElement = tabPositionCache[nextIndex + 1];
                nextIndex = nextIndex + 1;
            }

            stopIterating = false;

            while (!stopIterating)
            {
                if (ContainsItem(previousElement as UIElement))
                {
                    stopIterating = true;
                    return items.IndexOf(GetTabItem(previousElement as UIElement)) + 1;
                }

                if (previousIndex == 0)
                {
                    stopIterating = true;
                    return 0;
                }

                previousElement = tabPositionCache[previousIndex - 1];
                previousIndex = previousIndex - 1;
            }

            return 0;
        }

        /// <summary>
        /// Sets the menu items.
        /// </summary>
        /// <param name="tabItem">The tab item to add menu items.</param>
        /// <param name="element">The element hosted in the tab item.</param>
        private void SetMenuItems(TabItemExt tabItem, UIElement element)
        {
            bool isInDockingManager = Container.IsInDockingManager;

            if (isInDockingManager)
            {
                AddDockingMenuItems(tabItem, element);
            }

            if (Container.TabGroupEnabled)
            {
                RoutedCommand[] commands = new RoutedCommand[]
                {
                    NewHorizontalTabGroupCommand,
                    NewVerticalTabGroupCommand, 
                    MoveToNextTabGroupCommand, 
                    MoveToPreviousTabGroupCommand
                };

                string[] headers = new string[]
                {
                   wrapper.NewHorizontalTabGroup ,  //langDictionary["NewHorizontalTabGroup"] as string,
                   wrapper.NewVerticalTabGroup ,  //langDictionary["NewVerticalTabGroup"] as string,
                   wrapper.MoveToNextTabGroup ,  //langDictionary["MoveToNextTabGroup"] as string,
                   wrapper.MoveToPreviousTabGroup   // langDictionary["MoveToPreviousTabGroup"] as string
                };

                ////string[] headers = new string[]
                ////{
                ////    HORIZONTAL_TAB,
                ////    VERTICAL_TAB,
                ////    NEXT_TAB_GROUP,
                ////    PREVIOUS_TAB_GROUP
                ////};

                bool[] isInVisbleDisabled = new bool[]
                {
                    true, true, true, true
                };

                Separator separator = AddSeparator(tabItem);
                List<MenuItem> menuItems = ApplyMenuItems(tabItem, element, commands, headers, isInVisbleDisabled);
#if DEBUG
                if (GROUP_MENUITEMS_COUNT != menuItems.Count)
                {
                    throw new NotImplementedException();
                }
#endif
                MultiBinding multiBinding = new MultiBinding
                {
                    Converter = new QuantityToVisibilityConvertor()
                };

                foreach (MenuItem item in menuItems)
                {
                    Binding binding = new Binding
                    {
                        Source = item,
                        Path = new PropertyPath(UIElement.IsEnabledProperty)
                    };
                    multiBinding.Bindings.Add(binding);
                }

                BindingOperations.ClearBinding(separator, VisibilityProperty);
                separator.SetBinding(VisibilityProperty, multiBinding);
            }
        }
        /// <summary>
        /// Adds menu items for Docking context menu.
        /// </summary>
        /// <param name="tabItem">The tab item to add menu items.</param>
        /// <param name="element">The element hosted in the tab item.</param>
        private void AddDockingMenuItems(TabItemExt tabItem, UIElement element)
        {
            AddSeparator(tabItem);

            RoutedCommand[] commands = new RoutedCommand[]
            {
                DockingManager.FloatingCommand, 
                DockingManager.DockableCommand
            };
            string[] headers = new string[]
            {
                wrapper.Floating ,   //langDictionary["Floating"] as string,
                wrapper .Dockable    //langDictionary["Dockable"] as string,                       
            };
            //string[] headers = new string[]
            //{
            //    FLOATING,
            //    DOCKABLE,
            //};
            bool[] isInVisbleDisabled = new bool[] { false, false };

            ApplyMenuItems(tabItem, element, commands, headers, isInVisbleDisabled);
            AddDocumentMenuItem(tabItem);
        }

        /// <summary>
        /// Sets commands and headers for menu items.
        /// </summary>
        /// <param name="tabItem">The tab item to add menu items.</param>
        /// <param name="element">The element hosted in the tab item.</param>
        /// <param name="commands">Commands list to menu items.</param>
        /// <param name="headers">Headers list to menu items.</param>
        /// <param name="isInVisbleDisabled">The is in visible disabled.</param>
        /// <returns>List MenuItem </returns>
        private List<MenuItem> ApplyMenuItems(TabItemExt tabItem, UIElement element, RoutedCommand[] commands, string[] headers, bool[] isInVisbleDisabled)
        {
            int cnt = commands.Length;
            List<MenuItem> menuItems = new List<MenuItem>(cnt);

            for (int i = 0; i < cnt; ++i)
            {
                MenuItem item = SetMenuItem(tabItem, element, commands[i], headers[i], isInVisbleDisabled[i]);
                menuItems.Add(item);
            }

            return menuItems;
        }

        /// <summary>
        /// Adds separator to the context menu.
        /// </summary>
        /// <param name="tabItem">Tab item to add separator to.</param>
        /// <returns>Separator separator</returns>
        private Separator AddSeparator(TabItemExt tabItem)
        {
            if (Container.m_genericTempatesDictionary == null)
            {
                Container.GetVisualStyleSource();
            }
           
            Separator separator = new Separator
            {

                Style = (Style)Container.m_genericTempatesDictionary[SEP_STYLE]
                //Template = (ControlTemplate)m_genericTempatesDictionary[SEP_TEMPLATE]
            };              
            tabItem.ContextMenuItems.Add(separator);
            return separator;
        }

        /// <summary>
        /// Sets the menu items.
        /// </summary>
        /// <param name="tabItem">The tab item.</param>
        /// <param name="element">The element.</param>
        /// <param name="routedCommand">The routed command.</param>
        /// <param name="header">The header.</param>
        /// <param name="isInVisbleDisabled">if set to <c>true</c> [is in visible disabled].</param>
        /// <returns>MenuItem item</returns>
        private MenuItem SetMenuItem(TabItemExt tabItem, UIElement element, ICommand routedCommand, string header, bool isInVisbleDisabled)
        {
            MenuItem item = new MenuItem
            {
                Header = header,
                CommandTarget = this,
                Command = routedCommand,
                CommandParameter = element
            };

            if (isInVisbleDisabled)
            {
                BindingUtils.SetBinding(item, item, VisibilityProperty, IsEnabledProperty, BindingMode.OneWay, new BooleanToVisibilityConverter());
            }

            SetItemIcon(item);
            item.IsEnabledChanged += new DependencyPropertyChangedEventHandler(OnMenuItemIsEnabledChanged);

            tabItem.ContextMenuItems.Add(item);
            return item;
        }

        /// <summary>
        /// Sets the command bindings.
        /// </summary>
        private void SetCommnadBindings()
        {
            CommandBinding floatingCommand
                = new CommandBinding(DockingManager.FloatingCommand, new ExecutedRoutedEventHandler(ExecuteFloatingCommand), new CanExecuteRoutedEventHandler(CanExecuteFloatingCommand));
            CommandBinding dockableCommand
                = new CommandBinding(DockingManager.DockableCommand, new ExecutedRoutedEventHandler(ExecuteDockableCommand), new CanExecuteRoutedEventHandler(CanExecuteDockableCommand));

            CommandBindings.Add(floatingCommand);
            CommandBindings.Add(dockableCommand);
        }

        /// <summary>
        /// Executes the dockable command.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private void ExecuteDockableCommand(object sender, ExecutedRoutedEventArgs e)
        {
            ExecuteCommand(e, DockState.Dock);
        }

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        /// <param name="state">The state.</param>
        private void ExecuteCommand(ExecutedRoutedEventArgs e, DockState state)
        {
            FrameworkElement element = e.Parameter as FrameworkElement;
#if DEBUG
            if (null == element)
            {
                throw new ArgumentException();
            }
#endif
            DockingManager.SetNoDock(element, DockState.Float == state);
            DockingManager manager = DockingManager.ResolveManager(element);
            if (manager != null)
            {
                if (DockingManager.CanChangeState(element, state))
                {
                    DockStateChangingEventArgs args = new DockStateChangingEventArgs();
                    args.SourceElement = element;
                    args.TargetState = state;
                    args.PresentState = DockState.Document;
                    manager.FireDockStateChanging(element, args);
                    if (!args.Cancel)
                    {
                        manager.m_IsStateChangingChecked = true;
                        DockingManager.SetState(element, state);
                    }
                }
            }
            DetachHandlers(element);
        }

        /// <summary>
        /// Executes the floating command.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private void ExecuteFloatingCommand(object sender, ExecutedRoutedEventArgs e)
        {
            ExecuteCommand(e, DockState.Float);
        }

        /// <summary>
        /// Clears the visual children.
        /// </summary>
        private void ClearVisual()
        {
            if (null != Items)
            {
                foreach (TabItemExt item in Items)
                {
                    ContentPresenter presenter = (ContentPresenter)item.Content;
                    presenter.ClearValue(ContentPresenter.ContentProperty);
                    item.ContextMenuItems.Clear();
                }
            }

            foreach (IInputElement element in Container.m_LogicalChildren)
            {
                DetachHandlers(element);
            }

            Clear();
        }

        /// <summary>
        /// Clears items collection.
        /// <remarks>This method doesn't remove handlers of item.</remarks>
        /// </summary>
        private void Clear()
        {
            Items.Clear();
        }

        /// <summary>
        /// Finds the next tab item.
        /// </summary>
        /// <param name="startIndex">The start index.</param>
        /// <param name="adjustValue">The adjust value.</param>
        /// <returns>TabItemExt item</returns>
        private TabItemExt FindNextTabItem(int startIndex, int adjustValue)
        {
            int index = startIndex;

            for (int i = 0, cnt = Items.Count; i < cnt; ++i)
            {
                index += adjustValue;

                if (index >= Items.Count)
                {
                    index = 0;
                }
                else if (index < 0)
                {
                    index = Items.Count - 1;
                }

                TabItemExt item = ItemContainerGenerator.ContainerFromIndex(index) as TabItemExt;

                if (item != null && item.IsEnabled && item.Visibility == Visibility.Visible)
                {
                    return item;
                }
            }

            return null;
        }

        /// <summary>
        /// Sets the DockState to hidden.
        /// </summary>
        /// <param name="item">The item UIElement.</param>
        private void SetHiddenDockState(ContentControl item)
        {
            FrameworkElement element = GetContent(item) as FrameworkElement;
            if (element == null)
                return;
            m_isInternalCloseItem = true;
            DockingManager.SetState(element, DockState.Hidden);
            m_isInternalCloseItem = false;
            RemoveItem(element);
        }

        /// <summary>
        /// Sets the active document.
        /// </summary>
        /// <param name="activeDocument">The active document.</param>
        private void SetActiveDocument(UIElement activeDocument)
        {
            if (null != Container && ContainsItem(activeDocument))
            {
                SelectedItem = GetTabItem(activeDocument);
                FrameworkElement selectedElement = SelectedItem as FrameworkElement;
                if (selectedElement!=null && selectedElement.DataContext != null && SelectedContentPresenter!=null)
                {
                    SelectedContentPresenter.DataContext = (SelectedItem as FrameworkElement).DataContext;
                }
            }
        }

        /// <summary>
        /// Reorders the items.
        /// </summary>
        private void ReorderItems()
        {
            int cnt = Items.Count;

            if (1 < cnt)
            {
                Dictionary<int, TabItemExt> list = new Dictionary<int, TabItemExt>(cnt);
                TabItemExt selectedItem = null;

                foreach (TabItemExt item in Items)
                {
                    UIElement element = GetContent(item);
                    int index = (int)element.GetValue(TDILayoutPanel.TDIIndexProperty);
                    if (index > 0)
                    {
                        index = VerifyListKeys(list, index);
                        list.Add(index, item);
                    }
                    if ((bool)TDILayoutPanel.GetIsSelected(element))
                    {
                        selectedItem = item;
                    }
                }

                int key = 0;

                while (0 < list.Count)
                {
                    if (list.ContainsKey(key))
                    {
                        TabItemExt item = list[key];
                        Items.Remove(item);
                        Items.Add(item);
                        list.Remove(key);
                        UIElement obj = null;
                        if (item.Content is ContentPresenter)
                        {
                            obj = (item.Content as ContentPresenter).Content as UIElement;
                            if (TabPositionCache.Contains(obj) && Container.IsInDockingManager && (Container.DockingManager!=null) && !Container.DockingManager.m_loadingState)
                            {
                                TabPositionCache.Remove(obj);
                                TabPositionCache.Add(obj);
								UpdateTabPositionCache(TabPositionCache);
                            }
                        }
                    }

                    ++key;
                }

                SelectedItem = selectedItem;
            }
        }

        /// <summary>
        /// Verifies the list keys.
        /// </summary>
        /// <param name="list">The list UIElement.</param>
        /// <param name="index">The index UIElement.</param>
        /// <returns>int index value</returns>
        private static int VerifyListKeys(Dictionary<int, TabItemExt> list, int index)
        {
            if (list.ContainsKey(index))
            {
                int max = index;

                foreach (KeyValuePair<int, TabItemExt> item in list)
                {
                    if (item.Key > max)
                    {
                        max = item.Key;
                    }
                }

                index = max + 1;
            }

            return index;
        }

        /// <summary>
        /// Determines whether this instance [can execute floating command] the specified sender.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.CanExecuteRoutedEventArgs"/> instance containing the event data.</param>
        private static void CanExecuteFloatingCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            DependencyObject element = e.Parameter as DependencyObject;

            if (null != element)
            {
                e.CanExecute = DockingManager.GetCanFloat(element);
            }
        }

        /// <summary>
        /// Determines whether this instance [can execute dockable command] the specified sender.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.CanExecuteRoutedEventArgs"/> instance containing the event data.</param>
        private static void CanExecuteDockableCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            DependencyObject element = e.Parameter as DependencyObject;

            if (null != element)
            {
                e.CanExecute = DockingManager.GetCanDock(element);
            }
        }

        /// <summary>
        /// Sets the item icon.
        /// </summary>
        /// <param name="item">The item UIElement.</param>
        private static void SetItemIcon(MenuItem item)
        {
            string header = item.Header.ToString();

            if (header == wrapper.NewHorizontalTabGroup || header == wrapper.NewVerticalTabGroup)              //if (header == langDictionary["NewHorizontalTabGroup"] as string || header == langDictionary["NewVerticalTabGroup"] as string)
            {
                item.Icon = GetCommandIcon(header, item.IsEnabled);
            }
        }

        /// <summary>
        /// Gets the command icon.
        /// </summary>
        /// <param name="header">The header.</param>
        /// <param name="enabled">if set to <c>true</c> [enabled].</param>
        /// <returns>Image image value</returns>
        private static Image GetCommandIcon(string header, bool enabled)
        {
            Uri uri = header == wrapper.NewHorizontalTabGroup ?          //langDictionary["NewHorizontalTabGroup"] as string ?
                new Uri(HORIZONTAL_IMAGE, UriKind.RelativeOrAbsolute)
                : new Uri(VERTICAL_IMAGE, UriKind.RelativeOrAbsolute);

            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.UriSource = uri;
            image.EndInit();

            Image icon = new Image
            {
                Source = image
            };

            if (!enabled && EnvironmentTest.IsSecurityGranted)
            {
                icon = GetGrayScaleImage(icon);
            }

            return icon;
        }

        /// <summary>
        /// Gets the gray scale image.
        /// </summary>
        /// <param name="icon">The icon UIElement.</param>
        /// <returns>Image image value</returns>
        private static Image GetGrayScaleImage(Image icon)
        {
            BitmapSource bitmapSource = (BitmapSource)icon.Source;

            ColorContext sourceColorContext = new ColorContext(bitmapSource.Format);
            ColorContext destColorContext = new ColorContext(PixelFormats.Bgra32);
            ColorConvertedBitmap ccb = new ColorConvertedBitmap(bitmapSource, sourceColorContext, destColorContext, PixelFormats.Bgra32);

            int width = ccb.PixelWidth;
            int height = ccb.PixelHeight;
            int stride = width * ((ccb.Format.BitsPerPixel + 7) / 8);
            byte[] bits = new byte[height * stride];

            ccb.CopyPixels(bits, stride, 0);

            int length = bits.GetLength(0);
            int step = ccb.Format.BitsPerPixel / 8;

            for (int i = 0; i < length; i += step)
            {
                byte blue = bits[i];
                byte green = bits[i + 1];
                byte red = bits[i + 2];
                byte alpha = bits[i + 3];

                byte gray = (byte)(red * 0.30f + green * 0.59f + blue * 0.11f);

                bits[i] = gray;
                bits[i + 1] = gray;
                bits[i + 2] = gray;
                bits[i + 3] = (byte)(alpha / 2);
            }

            BitmapSource convertedSource = BitmapSource.Create(ccb.PixelWidth, ccb.PixelHeight, ccb.DpiX, ccb.DpiY, ccb.Format, null, bits, stride);

            Image image = new Image
            {
                Source = convertedSource
            };

            return image;
        }

        /// <summary>
        /// Adds "Document" menu item to the context menu of the Docking window.
        /// </summary>
        /// <param name="tabItem">The tab item to add menu item.</param>
        private static void AddDocumentMenuItem(TabItemExt tabItem)
        {
            MenuItem item = new MenuItem
            {
                Header = wrapper.Document,                                   //langDictionary["Document"] as string,
                IsCheckable = false,
                IsChecked = true
            };
            tabItem.ContextMenuItems.Add(item);
        }

        /// <summary>
        /// Handles the IsEnabledChanged event of the MenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnMenuItemIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            MenuItem item = sender as MenuItem;
            SetItemIcon(item);
        }

        /// <summary>
        /// Sets CanExecute value for some menu item.
        /// </summary>
        /// <param name="e">The event data.</param>
        /// <param name="item">Tab item to change.</param>
        private void SetCanExecuteClose(CanExecuteRoutedEventArgs e, TabItemExt item)
        {
            e.CanExecute = CanCloseTabItem(item);
        }

        /// <summary>
        /// Determines whether this instance [can close tab item] the specified item.
        /// </summary>
        /// <param name="item">The tab item</param>
        /// <returns>
        /// <c>true</c> if this instance [can close tab item] the specified item; otherwise, <c>false</c>.
        /// </returns>
        protected override bool CanCloseTabItem(TabItemExt item)
        {
            bool bCanClose = true;

            if (item != null)
            {
                ContentPresenter presenter = item.Content as ContentPresenter;

                if (presenter != null && presenter.Content != null)
                {
                    if (presenter.Content is DependencyObject)
                    {
                        if (!DockingManager.GetCanClose(presenter.Content as DependencyObject))
                        {
                            bCanClose = false;
                        }
                    }
                    else if (!DockingManager.GetCanClose(presenter as DependencyObject))
                    {
                        bCanClose = false;
                    }
                }
            }

            return bCanClose;
        }
        #endregion

        #region Commands
        /// <summary>
        /// Command for new horizontal tab group.
        /// </summary>
        public static readonly RoutedCommand NewHorizontalTabGroupCommand = new RoutedCommand(wrapper.NewHorizontalTabGroup, typeof(DocumentTabControl));

        /// <summary>
        /// Command for new Vertical tab.
        /// </summary>
        public static readonly RoutedCommand NewVerticalTabGroupCommand = new RoutedCommand(wrapper.NewVerticalTabGroup, typeof(DocumentTabControl));

        /// <summary>
        /// Represents move to next tab group command.
        /// </summary>
        public static readonly RoutedCommand MoveToNextTabGroupCommand = new RoutedCommand(wrapper.MoveToNextTabGroup, typeof(DocumentTabControl));

        /// <summary>
        /// Represents move to previous tab group command.
        /// </summary>
        public static readonly RoutedCommand MoveToPreviousTabGroupCommand = new RoutedCommand(wrapper.MoveToPreviousTabGroup, typeof(DocumentTabControl));
        #endregion
    }
}