#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System.Collections;
using System.Collections.Generic;
using System.Reflection;

#if WINDOWS_PHONE||WINDOWS_PHONE_7
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows;
using Syncfusion.WP.Primitives;
using System;
using System.Windows.Media;
using System.Windows.Data;
using System.ComponentModel;

namespace Syncfusion.WP.Controls.Navigation
#elif SILVERLIGHT
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows;
using Syncfusion.Tools.Primitives;
using System;
using System.Windows.Media;
using System.Windows.Data;
using System.ComponentModel;
namespace Syncfusion.Tools.Controls.Navigation
#elif WPF
using System.Windows.Controls;
using System.Windows;
using Syncfusion.Windows.Primitives;
using System;
using System.Windows.Media;
using System.Windows.Data;
using System.Collections.ObjectModel;
using Syncfusion.Licensing;
using System.ComponentModel;
namespace Syncfusion.Windows.Controls.Navigation
#else
using System.Collections.ObjectModel;
using Syncfusion.UI.Xaml.Controls;
using Syncfusion.UI.Xaml.Primitives;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.Foundation;

// The Templated Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234235

namespace Syncfusion.UI.Xaml.Controls.Navigation
#endif
{
    /// <summary>
    /// Represents a control that enables the user to navigate through items 
    /// <see cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfTreeNavigatorItem"/>
    /// in a tree structure
    /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfTreeNavigator"/>
    /// </summary>
#if WPF
    public class SfTreeNavigator : System.Windows.Controls.HeaderedItemsControl
#else
    public class SfTreeNavigator : HeaderedItemsControl
#endif
    {
#if WINDOWS_PHONE||WINDOWS_PHONE_7
        TreeNavigatorItemsHost _host;
#endif

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfTreeNavigator"/> class.
        /// </summary>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfTreeNavigatorItem"/>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfTreeNavigator"/>
        /// <seealso cref="N:Syncfusion.UI.Xaml.Controls">Syncfusion.UI.Xaml.Controls
        /// Namespace</seealso>
        public SfTreeNavigator()
        {
#if WPF
            if (EnvironmentTestSfTreeNavigation.IsSecurityGranted)
            {
                EnvironmentTestSfTreeNavigation.StartValidateLicense(typeof(SfTreeNavigator));
            }
#endif

            this.DefaultStyleKey = typeof(SfTreeNavigator);
            DrillDownItem = this;
            DrillDownItems = new ObservableCollection<object>();

#if WINDOWS_PHONE||WINDOWS_PHONE_7
            _host = new TreeNavigatorItemsHost();
            Loaded+=SfTreeNavigator_Loaded;
#else 
            Loaded += SfTreeView_Loaded;
#endif
        }
#if WINDOWS_PHONE||WINDOWS_PHONE_7
        void SfTreeNavigator_Loaded(object sender, RoutedEventArgs e)
        {
            this.LayoutUpdated += SfTreeNavigator_LayoutUpdated;
        }
#else
        void SfTreeView_Loaded(object sender, RoutedEventArgs e)
        {          
            if (PART_Host != null)
            {
                if (PART_Navigator.ActiveItem != PART_Host)
                {
                    PART_BackButton.Visibility = Visibility.Collapsed;
                }
                UpdateNavigationMode();
                if(SelectedItem != null)
                    Select(SelectedItem);
            }
            this.LayoutUpdated += SfTreeNavigator_LayoutUpdated;
        }
#endif
        private void SfTreeNavigator_LayoutUpdated(object sender, object e)
        {
            if (SelectedItem != null &&((SelectedItem as SfTreeNavigatorItem) !=null && !(SelectedItem as SfTreeNavigatorItem).IsSelected))
            {
                Select(null);
                Select(SelectedItem);
                UpdateLayout();
           }
            if(PART_Navigator!=null)
                (PART_Navigator as SfNavigator).Clip = new RectangleGeometry() { Rect = new Rect() { Height = ActualHeight, Width = ActualWidth, X = 0, Y = 0 } };        
#if WINRT
            if(Windows.ApplicationModel.DesignMode.DesignModeEnabled)
#else
            if(DesignerProperties.GetIsInDesignMode(this))
#endif
                LayoutUpdated -= SfTreeNavigator_LayoutUpdated;
        }


        internal TreeNavigatorItemsHost ActiveTreeHost
        {
            get
            {
                if (PART_Navigator != null)
                {
                    return PART_Navigator.ActiveItem == null ? PART_Host as TreeNavigatorItemsHost : PART_Navigator.ActiveItem as TreeNavigatorItemsHost;
                }
                return null;
            }
        }
        /// <summary>
        /// Occurs when the <see 
        /// cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfTreeNavigatorItem"/> has changed.
        /// </summary>
#if WPF
        public static readonly RoutedEvent SelectedEvent =
                                    EventManager.RegisterRoutedEvent(
                                    "SelectionChanged", RoutingStrategy.Bubble,
                                    typeof(SelectionChangedEventHandler),
                                    typeof(SfTreeNavigator));

        public event SelectionChangedEventHandler SelectionChanged
        {
            add { AddHandler(SelectedEvent, value); }
            remove { RemoveHandler(SelectedEvent, value); }
        }
#else
        public event SelectionChangedEventHandler SelectionChanged;
#endif
        private TreeNavigatorItemsHost prevHost;

        internal TreeNavigatorItemsHost PART_Host;

        internal SfTreeNavigatorItem prevItem;

        internal SfNavigator PART_Navigator;

        internal List<object> hierarchyselectionlist = new List<object>();

        internal bool internalselection = false;

#if WINDOWS_PHONE
#else
        private Button PART_BackButton;
#endif

        private Button PART_HomeButton;
        private TreeNavigatorItemsHost PART_DrillDownItemsHost;
        private Border PART_DefaultModeHeader;
        private Grid PART_ExtendedModeHeader;

        /// <summary>
        /// Initializes all the child elements of the <see 
        /// cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfTreeNavigator"/> control.
        /// </summary>
#if !WINRT
        public override void OnApplyTemplate()
#else
        protected override void OnApplyTemplate()
#endif
        {
            PART_Navigator = GetTemplateChild("PART_Navigator") as SfNavigator;
            PART_Host = GetTemplateChild("PART_Host") as TreeNavigatorItemsHost;
            PART_DrillDownItemsHost = GetTemplateChild("PART_DrillDownItemsHost") as TreeNavigatorItemsHost;
            PART_DefaultModeHeader = GetTemplateChild("PART_DefaultModeHeader") as Border;
            PART_ExtendedModeHeader = GetTemplateChild("PART_ExtendedModeHeader") as Grid;
            if (PART_DrillDownItemsHost != null)
            {
                PART_DrillDownItemsHost.parentTree = this;
                PART_DrillDownItemsHost.hostitemsControl = this;
            }
#if WINDOWS_PHONE
#else
            PART_BackButton = GetTemplateChild("PART_BackButton") as Button;
#endif

            PART_HomeButton = GetTemplateChild("PART_HomeButton") as Button;
            if (PART_Host != null)
            {
                PART_Host.parentTree = this;
                PART_Host.hostitemsControl = this;
            }
           
#if WINDOWS_PHONE
#else
            if (PART_BackButton != null)
            {
                PART_BackButton.Click += PartBackButtonClick;
            }
#endif


            if (PART_HomeButton != null)
            {
                PART_HomeButton.Click += PART_HomeButton_Click;
            }
#if WINRT||WPFSILVERLIGHT
            UpdateNavigationMode();
#endif
            base.OnApplyTemplate();
        }

        void PART_HomeButton_Click(object sender, RoutedEventArgs e)
        {
            if (ActiveTreeHost != null && PART_Navigator != null && PART_Navigator.ActiveItem != PART_Host)
            {
                PART_Navigator.ActiveItem = PART_Host;
                DrillDownItem = this;
            }
        }

        void PartBackButtonClick(object sender, RoutedEventArgs e)
        {
            GoBack();
        }

        /// <summary>
        /// Gets or sets the item <see 
        /// cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfTreeNavigatorItem"/>
        /// that has been selected by the user.
        /// <value> The default value is null </value>
        /// </summary>
        public object SelectedItem
        {
            get { return (object)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for SelectedItem.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(object), typeof(SfTreeNavigator), new PropertyMetadata(null, OnSelectionChanged));

        /// <summary>
        /// Gets or sets the child item <see 
        /// cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfTreeNavigatorItem"/>
        /// </summary>
#if WPF
        /// <summary>
        /// Using a DependencyProperty as the backing store for DrillDownItem.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DrillDownItemProperty =
           DependencyProperty.Register("DrillDownItem", typeof(System.Windows.Controls.HeaderedItemsControl), typeof(SfTreeNavigator), new PropertyMetadata(null));

        public System.Windows.Controls.HeaderedItemsControl DrillDownItem
        {
            get { return (System.Windows.Controls.HeaderedItemsControl)GetValue(DrillDownItemProperty); }
            internal set { SetValue(DrillDownItemProperty, value); }
        }
#else
        
        /// <summary>
        /// Using a DependencyProperty as the backing store for DrillDownItem.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DrillDownItemProperty =
           DependencyProperty.Register("DrillDownItem", typeof(HeaderedItemsControl), typeof(SfTreeNavigator), new PropertyMetadata(null));

        public HeaderedItemsControl DrillDownItem
        {
            get { return (HeaderedItemsControl)GetValue(DrillDownItemProperty); }
            internal set { SetValue(DrillDownItemProperty, value); }
        }
#endif

        /// <summary>
        /// Using a DependencyProperty as the backing store for DrillDownItems.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DrillDownItemsProperty =
            DependencyProperty.Register("DrillDownItems", typeof (ObservableCollection<object>), typeof (SfTreeNavigator), new PropertyMetadata(default(ObservableCollection<object>)));

        /// <summary>
        /// Gets or sets a collection of the child items <see 
        /// cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfTreeNavigatorItem"/>
        /// </summary>
        public ObservableCollection<object> DrillDownItems
        {
            get { return (ObservableCollection<object>) GetValue(DrillDownItemsProperty); }
            set { SetValue(DrillDownItemsProperty, value); }
        }
        /// <summary>
        /// Using a DependencyProperty as the backing store for NavigationMode.  This enables animation, styling, binding, etc...
        /// </summary>
#if !(WINDOWS_PHONE||WINDOWS_PHONE_7)
        public static readonly DependencyProperty NavigationModeProperty =
           DependencyProperty.Register("NavigationMode", typeof(NavigationMode), typeof(SfTreeNavigator), new PropertyMetadata(NavigationMode.Default, OnNavigationModeChanged));

        /// <summary>
        /// Gets or sets the mode of navigation to other items
        /// <see cref="T:Syncfusion.UI.Xaml.Controls.Navigation.NavigationMode"/>
        /// </summary>
        public NavigationMode NavigationMode
        {
            get { return (NavigationMode) GetValue(NavigationModeProperty); }
            set { SetValue(NavigationModeProperty, value); }
        }

        private static void OnNavigationModeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var control = sender as SfTreeNavigator;
            if (control != null)
            {
                control.UpdateNavigationMode();
            }
        }

        private void UpdateNavigationMode()
        {
            if (NavigationMode == NavigationMode.Default)
            {
                if(PART_DefaultModeHeader != null)
                    PART_DefaultModeHeader.Visibility = Visibility.Visible;
                if(PART_ExtendedModeHeader != null)
                    PART_ExtendedModeHeader.Visibility = Visibility.Collapsed;
            }
            if (NavigationMode == NavigationMode.Extended)
            {
                if (PART_DefaultModeHeader != null)
                    PART_DefaultModeHeader.Visibility = Visibility.Collapsed;
                if (PART_ExtendedModeHeader != null)
                    PART_ExtendedModeHeader.Visibility = Visibility.Visible;
            }
        }

#endif
        private static void OnSelectionChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var control = sender as SfTreeNavigator;
            if (control != null)
            {
                control.OnSelectionChanged(args);
#if !WPF
                if (control.SelectionChanged != null)
                {
#endif
                    List<object> oldItems = new List<object>();
                    List<object> newItems = new List<object>();
                    oldItems.Add(args.OldValue);
                    newItems.Add(args.NewValue);
#if WPF  
                    SelectionChangedEventArgs selectionargs = new SelectionChangedEventArgs(SelectedEvent, oldItems,
                                                                                          newItems);
                    control.RaiseEvent(selectionargs);
#else
                    SelectionChangedEventArgs selectionargs = new SelectionChangedEventArgs(oldItems, newItems);
                    control.SelectionChanged(control, selectionargs);
                }
#endif
            }
        }

        /// <summary>
        /// Occurs when the selected item has changed.
        /// </summary>
        /// <param name="args"></param>
        protected virtual void OnSelectionChanged(DependencyPropertyChangedEventArgs args)
        {
            if (Items.Count > 0)
            {
                if (PART_Navigator != null)
                {
                    Select(args.NewValue);
                }
            }
#if WINDOWS_PHONE||WINDOWS_PHONE_7
#else
            if (PART_BackButton != null)
            {
                if (PART_Navigator != null && PART_Navigator.ActiveItem != PART_Host)
                {
                    PART_BackButton.Visibility = Visibility.Visible;
                }
                else
                {
                    PART_BackButton.Visibility = Visibility.Collapsed;
                }
            }
#endif

        }

        /// <summary>
        /// When invoked, the control passes back to the parent item.
        /// </summary>
        public void GoBack()
        {
            if (ActiveTreeHost != null && PART_Navigator != null && PART_Navigator.ActiveItem != PART_Host)
            {
                IsHitTestVisible = false;
                prevHost = ActiveTreeHost;
                PART_Navigator.Navigated += PartNavigatorNavigated;

                if (ActiveTreeHost.hostitemsControl is SfTreeNavigatorItem)
                {
                    var treeitem = ActiveTreeHost.hostitemsControl as SfTreeNavigatorItem;
                    PART_Navigator.ActiveItem = treeitem.parentHost;
#if WPF
                    DrillDownItem = (ActiveTreeHost.hostitemsControl as System.Windows.Controls.HeaderedItemsControl);
#else
                    DrillDownItem = (ActiveTreeHost.hostitemsControl as HeaderedItemsControl);
#endif
                }
                else if (ActiveTreeHost.hostitemsControl is SfTreeNavigator)
                {
                    PART_Navigator.ActiveItem = PART_Host;
#if WPF
                    DrillDownItem = (ActiveTreeHost.hostitemsControl as System.Windows.Controls.HeaderedItemsControl);
#else
                    DrillDownItem = (ActiveTreeHost.hostitemsControl as HeaderedItemsControl);
#endif
                }
                SelectedItem = null;
                if(GetDrillDownItems().Count > 0)
                DrillDownItems.RemoveAt(DrillDownItems.Count-1);
            }
                if (ActiveTreeHost.parentTree == ActiveTreeHost.hostitemsControl)
                    PART_Navigator.ActiveItem = PART_Host;            
            IsHitTestVisible = true;
            PART_Navigator.Items.Remove(prevHost);
            prevHost.Dispose();
#if WPFSILVERLIGHT || WINRT
            if (PART_Navigator.ActiveItem == PART_Host && PART_BackButton.Visibility != Visibility.Collapsed)
            {
                PART_BackButton.Visibility = Visibility.Collapsed;
            }
#endif
        }
#if WINDOWS_PHONE
        void PartNavigatorNavigated(object sender, EventArgs e)
#else
        void PartNavigatorNavigated(object sender, RoutedEventArgs e)
        
#endif
    {
            PART_Navigator.Navigated -= PartNavigatorNavigated;
    }
#if WINDOWS_PHONE
        void PartNavigatorNavigatedForward(object sender, EventArgs e)
#else
        void PartNavigatorNavigatedForward(object sender, RoutedEventArgs e)
#endif
        
        {
            (sender as SfNavigator).Clip = new RectangleGeometry() { Rect = new Rect() { Height = ActualHeight, Width = ActualWidth, X = 0, Y = 0 } };
#if WPFSILVERLIGHT || WINRT
            if (NavigationMode==NavigationMode.Default && PART_Navigator.ActiveItem != PART_Host && PART_BackButton.Visibility != Visibility.Visible)
            {
                PART_BackButton.Visibility = Visibility.Visible;
            }
#endif
            if (ActiveTreeHost != null)
            {
                var treeItem = ActiveTreeHost.ItemContainerGenerator.ContainerFromIndex(0) as SfTreeNavigatorItem;
                if (treeItem != null)
                {
                #if !WINRT
                    treeItem.Focus();
                #else
                    treeItem.Focus(FocusState.Keyboard);
                #endif
                }
            }
            PART_Navigator.Navigated -= PartNavigatorNavigatedForward;
        }

        /// <summary>
        /// Occurs when the items <see 
        /// cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfTreeNavigatorItem"/> has changed.
        /// </summary>
        /// <param name="e"></param>
#if WINDOWS_PHONE||WINDOWS_PHONE_7||WPFSILVERLIGHT
        protected override void OnItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
#else
        protected override void OnItemsChanged(object e)
#endif
        {
            base.OnItemsChanged(e);
            if (PART_Navigator != null)
            {
                PART_Navigator.ActiveItem = PART_Host;
                DrillDownItem = this;
                DrillDownItems = new ObservableCollection<object>();
            }
#if WINDOWS_PHONE||WINDOWS_PHONE_7||WPFSILVERLIGHT
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset && (this.DataContext == null || this.Items.Count == 0 || this.ItemsSource == null))
#else
            if ((this.DataContext==null || this.Items.Count==0 ||this.ItemsSource==null))
#endif
            {
                if (PART_Navigator != null)
                    PART_Navigator.Visibility = Visibility.Collapsed;
#if !(WINDOWS_PHONE||WINDOWS_PHONE_7)
                if (PART_ExtendedModeHeader != null && NavigationMode==Navigation.NavigationMode.Extended)
#else
                if (PART_ExtendedModeHeader != null)
#endif
                    PART_ExtendedModeHeader.Visibility = Visibility.Collapsed;
                else if (PART_DefaultModeHeader != null)
                    PART_DefaultModeHeader.Visibility = Visibility.Collapsed;
            }
            else
            {
                if (PART_Navigator != null)
                    PART_Navigator.Visibility = Visibility.Visible;
#if !(WINDOWS_PHONE||WINDOWS_PHONE_7)
                if (PART_ExtendedModeHeader != null && NavigationMode==Navigation.NavigationMode.Extended)
#else
                if(PART_ExtendedModeHeader != null)
#endif
                    PART_ExtendedModeHeader.Visibility = Visibility.Visible;
                else if (PART_DefaultModeHeader != null)
                    PART_DefaultModeHeader.Visibility = Visibility.Visible;
            }

#if WPFSILVERLIGHT ||WINRT
            if (PART_Navigator!=null && PART_Navigator.ActiveItem == PART_Host && PART_BackButton!=null && PART_BackButton.Visibility != Visibility.Collapsed)
            {
                PART_BackButton.Visibility = Visibility.Collapsed;
            }
#endif
        }

        /// <summary>
        /// Sets the item as te selected item.
        /// </summary>
        /// <param name="item"></param>
        public void Select(object item)
        {
            if (prevItem != null)
            {
                prevItem.IsSelected = false;
            }

            if (item == null)
            {
                return;
            }

            if (ActiveTreeHost != null)
            {
                ActiveTreeHost.IsTabStop = false;
                SfTreeNavigatorItem treeitem;
                if (item is SfTreeNavigator)
                {
                    if (prevItem != null)
                    {
                        prevItem.IsSelected = false;
                    }
                }
                if (item is SfTreeNavigatorItem)
                    treeitem = (item as SfTreeNavigatorItem);
                else
                    treeitem = ActiveTreeHost.ItemContainerGenerator.ContainerFromItem(item) as SfTreeNavigatorItem;
                if (treeitem != null)
                {
                    if (prevItem != null)
                    {
                        prevItem.IsSelected = false;
                        VisualStateManager.GoToState(prevItem, "Normal", true);
                    }
                    foreach (var treeItem in ActiveTreeHost.Items)
                    {
                        SfTreeNavigatorItem tempItem = null;
                        if (item is SfTreeNavigatorItem)
                            tempItem = (item as SfTreeNavigatorItem);
                        else
                            tempItem = ActiveTreeHost.ItemContainerGenerator.ContainerFromItem(treeItem) as SfTreeNavigatorItem;

                        if (tempItem != null)
                            tempItem.IsSelected = false;
                    }
                    treeitem.IsSelected = true;
                    prevItem = treeitem;
                    NavigateItem(treeitem);
                }
                else
                {
                    if (ItemsSource != null)
                    {
                        bool selectionlistupdated = false;
                        hierarchyselectionlist = GetHierarchySelectionList(new List<object>(), Items, out selectionlistupdated);
                        if (hierarchyselectionlist.Count > 1 && !internalselection)
                        {
                            treeitem = FindInitialHierarchyItem(hierarchyselectionlist);
                            if (treeitem != null)
                            {
                                NavigateItem(treeitem);
                            }
                        }
                    }
                }
            }
        }

        internal SfTreeNavigatorItem FindInitialHierarchyItem(List<object> items)
        {
            SfTreeNavigatorItem treeitem = null;
            while (treeitem == null && ActiveTreeHost != null)
            {
                foreach (var item in items)
                {
                    treeitem = ActiveTreeHost.ItemContainerGenerator.ContainerFromItem(item) as
                               SfTreeNavigatorItem;
                    if(treeitem != null)
                        break;
                }
                if (ActiveTreeHost.Equals(PART_Host))
                {
                    break;
                }
                if (treeitem == null)
                {
                    if (ActiveTreeHost != null && PART_Navigator != null && PART_Navigator.ActiveItem != PART_Host)
                    {
                        IsHitTestVisible = false;
                        prevHost = ActiveTreeHost;
                        PART_Navigator.Navigated += PartNavigatorNavigated;

                        if (ActiveTreeHost.hostitemsControl is SfTreeNavigatorItem)
                        {
                            var treeitem1 = ActiveTreeHost.hostitemsControl as SfTreeNavigatorItem;
                            PART_Navigator.ActiveItem = treeitem1.parentHost;
#if WPF
                            DrillDownItem = (ActiveTreeHost.hostitemsControl as System.Windows.Controls.HeaderedItemsControl);
#else
                            DrillDownItem = (ActiveTreeHost.hostitemsControl as HeaderedItemsControl);
#endif
                        }
                        else if (ActiveTreeHost.hostitemsControl is SfTreeNavigator)
                        {
                            PART_Navigator.ActiveItem = PART_Host;
#if WPF
                            DrillDownItem = (ActiveTreeHost.hostitemsControl as System.Windows.Controls.HeaderedItemsControl);
#else
                            DrillDownItem = (ActiveTreeHost.hostitemsControl as HeaderedItemsControl);
#endif
                        }
                        if (GetDrillDownItems().Count > 0)
                            DrillDownItems.RemoveAt(DrillDownItems.Count - 1);
                    }
                }
            }
            return treeitem;
        }

        internal List<object> GetHierarchySelectionList(List<object> selectionlist, IEnumerable items, out bool selectionlistupdated)
        {
            selectionlistupdated = false;
            foreach (var obj in items)
            {
                if (obj.Equals(SelectedItem))
                {
                    selectionlist.Add(obj);
                    return selectionlist;
                }
#if WINDOWS_PHONE_7 || SILVERLIGHT || WPF
                IEnumerable<PropertyInfo> properties = obj.GetType().GetProperties();
#else
                IEnumerable<PropertyInfo> properties = obj.GetType().GetTypeInfo().DeclaredProperties;
#endif
                foreach (PropertyInfo propertyInfo in properties)
                {
#if WINDOWS_PHONE_7 || SILVERLIGHT || WPF
                    IEnumerable collection = propertyInfo.GetValue(obj, null) as IEnumerable;
#else
                    IEnumerable collection = propertyInfo.GetValue(obj) as IEnumerable;
#endif
                    if (collection != null)
                    {
                        List<object> selectionitems = GetHierarchySelectionList(selectionlist, collection, out selectionlistupdated);
                        SfTreeNavigatorItem visibletreeitem = ActiveTreeHost.ItemContainerGenerator.ContainerFromItem(obj) as SfTreeNavigatorItem;
                        
                        if (selectionitems.Count > 0)
                        {
                            if(!selectionlistupdated)
                                selectionitems.Insert(0, obj);
                            if (visibletreeitem != null)
                                selectionlistupdated = true;
                            return selectionitems;
                        }
                    }
                }
            }
            return selectionlist;
        }

        internal ObservableCollection<object> GetDrillDownItems()
        {
            if (DrillDownItems != null)
                return DrillDownItems;
            else
                return DrillDownItems = new ObservableCollection<object>();
        }
        internal void NavigateItem(SfTreeNavigatorItem treeitem)
        {
            if (treeitem.HasItems)
            {
                var host = treeitem.childHost != null ? treeitem.childHost : new TreeNavigatorItemsHost();
                host.ItemsSource = treeitem.Items;
                host.ItemTemplate = treeitem.ItemTemplate;
                host.Header = treeitem.Header;
                host.parentTree = this;
                host.hostitemsControl = treeitem;
                treeitem.childHost = host;
                bool isAlreadyExists = false;
                if (PART_Navigator != null)
                {
                    foreach (TreeNavigatorItemsHost thost in PART_Navigator.Items)
                    {
                        if (thost.hostitemsControl == host.hostitemsControl)
                        {
                            PART_Navigator.ActiveItem = thost;
                            isAlreadyExists = true;
                            break;
                        }
                    }
                    if (!isAlreadyExists)
                    {
                        PART_Navigator.Items.Add(host);
                        PART_Navigator.ActiveItem = host;
                    }
                    DrillDownItem = treeitem;
                    if (this.ItemsSource != null)
                    {
                        if (!GetDrillDownItems().Contains(treeitem.DataContext))
                            DrillDownItems.Add(treeitem.DataContext);
                    }
                    else
                        GetDrillDownItems().Add(new TreeNavigatorHeaderItem()
                        {
                            Header = treeitem.Header,
                            ActualTreeViewItem = treeitem,
                            HeaderTemplate = treeitem.HeaderTemplate,
#if !(WINDOWS_PHONE||SILVERLIGHT)
                            HeaderTemplateSelector = treeitem.HeaderTemplateSelector,
#endif
                            Foreground = treeitem.Foreground,
                        });
                }
                PART_Navigator.Navigated += PartNavigatorNavigatedForward;
            }
        }
    }
}
