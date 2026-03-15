// <copyright file="NavigationToolbar.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

#region file using
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls.Primitives;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Collections;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Media;
using System.Windows.Media;
using System.Collections.Specialized;
using System.Diagnostics;
using Syncfusion.Licensing;
#endregion

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents the NavigationToolbar UI element.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class NavigationToolbar : Selector
    {
        #region Constants
        /// <summary>
        /// Default width of contained NavigationToolbarItem.
        /// </summary>
        private const double DEF_ITEM_DEFAULT_WIDTH = 26d;
        
        /// <summary>
        /// Default width of contained menu.
        /// </summary>
        private const double DEF_MENU_DEFAULT_WIDTH = 20d;
        
        /// <summary>
        /// Default height for menu item headers that present GroupBarItems.
        /// </summary>
        private const double DEF_MENUITEM_HEADER_HEIGHT = 17d;
        #endregion

        #region Private members
        /// <summary>
        /// Menu item with check box that allow to hide/show
        /// <see cref="Syncfusion.Windows.Tools.Controls.GroupBarItem"/> objects.
        /// </summary>
        private MenuItem m_buttonsMenu;
        
        /// <summary>
        /// Main menu item.
        /// </summary>
        private MenuItem m_mainMenu;

        /// <summary>
        /// Options Menu Item
        /// </summary>
        private MenuItem m_optionsMenu;
        
        /// <summary>
        /// Number of <see cref="Syncfusion.Windows.Tools.Controls.NavigationToolbarItem"/> objects in the menu.
        /// </summary>
        private int m_menuItemsCount = 0;
        
        /// <summary>
        /// Enables or disables automatic coercion of property.
        /// </summary>
        private bool m_autoWidthCoerce = true;
        
        /// <summary>
        /// Defines whether the control was focused.
        /// </summary>
        private bool m_bFocused = false;

        private GroupBar gBar = null;
        #endregion

        #region Dependency properties
        /// <summary>
        /// Identifies <see cref="ItemDefaultWidth"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemDefaultWidthProperty = DependencyProperty.Register("ItemDefaultWidth", typeof(double), typeof(NavigationToolbar), new UIPropertyMetadata(DEF_ITEM_DEFAULT_WIDTH));
        
        /// <summary>
        /// Identifies <see cref="MenuDefaultWidth"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MenuDefaultWidthProperty = DependencyProperty.Register("MenuDefaultWidth", typeof(double), typeof(NavigationToolbar), new UIPropertyMetadata(DEF_MENU_DEFAULT_WIDTH));
        
        /// <summary>
        /// Identifies <see cref="AvailableWidth"/> dependency property.
        /// </summary>
        private static readonly DependencyProperty AvailableWidthProperty = DependencyProperty.Register("AvailableWidth", typeof(double), typeof(NavigationToolbar), new UIPropertyMetadata(0d, null, OnAvailableWidthCoerce));
        
        /// <summary>
        /// Identifies <see cref="ItemMargin"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemMarginProperty = DependencyProperty.Register("ItemMargin", typeof(Thickness), typeof(NavigationToolbar), new UIPropertyMetadata(new Thickness(0)));

        #endregion

        #region Properties
        /// <summary>
        /// Gets the logical parent as GroupBar.
        /// </summary>
        /// <value>
        /// Type: <see cref="GroupBar"/>
        /// </value>
        /// <seealso cref="GroupBar"/>
        public GroupBar LogicalParent
        {
            get
            {
                return TemplatedParent as GroupBar;
            }
        }
        
        /// <summary>
        /// Gets inherited SelectedItem property as <see cref="Syncfusion.Windows.Tools.Controls.NavigationToolbarItem"/>.
        /// </summary>
        /// <value>
        /// Type: <see cref="NavigationToolbarItem"/>
        /// </value>
        /// <seealso cref="NavigationToolbarItem"/>
        new public NavigationToolbarItem SelectedItem
        {
            get
            {
                return base.SelectedItem as NavigationToolbarItem;
            }
        }
        
        /// <summary>
        /// Gets or sets default width of the contained items.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// </value>
        /// <seealso cref="double"/>
        public double ItemDefaultWidth
        {
            get
            {
                return (double)GetValue(ItemDefaultWidthProperty);
            }

            set
            {
                SetValue(ItemDefaultWidthProperty, value);
            }
        }
        
        /// <summary>
        /// Gets or sets default width of the contained menu.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// </value>
        /// <seealso cref="double"/>
        public double MenuDefaultWidth
        {
            get
            {
                return (double)GetValue(MenuDefaultWidthProperty);
            }

            set
            {
                SetValue(MenuDefaultWidthProperty, value);
            }
        }
        
        /// <summary>
        /// Gets or sets width that available for hosting new items.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// </value>
        /// <seealso cref="double"/>
        private double AvailableWidth
        {
            get
            {
                return (double)GetValue(AvailableWidthProperty);
            }

            set
            {
                SetValue(AvailableWidthProperty, value);
            }
        }
        
        /// <summary>
        /// Gets main menu item in control's template.
        /// </summary>
        /// <value>
        /// Type: <see cref="MenuItem"/>
        /// </value>
        /// <seealso cref="MenuItem"/>
        internal MenuItem MainMenu
        {
            get
            {
                return m_mainMenu;
            }
        }
        
        /// <summary>
        /// Gets menu item containing check boxes which allow to
        /// hide/show <see cref="Syncfusion.Windows.Tools.Controls.GroupBarItem"/> objects.
        /// </summary>
        /// <value>
        /// Type: <see cref="MenuItem"/>
        /// </value>
        /// <seealso cref="MenuItem"/>
        internal MenuItem ButtonsMenu
        {
            get
            {
                return m_buttonsMenu;
            }
        }
        
        /// <summary>
        /// Gets actual menu width taking into the account control's scaling ratio.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// </value>
        /// <seealso cref="double"/>
        private double ActualMenuWidth
        {
            get
            {
                return DEF_MENU_DEFAULT_WIDTH;
            }
        }
        
        /// <summary>
        /// Gets actual item width, taking into the account control's scaling ratio.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// </value>
        /// <seealso cref="double"/>
        private double ActualItemWidth
        {
            get
            {
                double width = DEF_ITEM_DEFAULT_WIDTH;
                width += ItemMargin.Left * 2;
                width += ItemMargin.Right * 2;

                return width;
            }
        }
        
        /// <summary>
        /// Gets first visible item to be pushed to menu when there is
        /// not enough space to host all items.
        /// </summary>
        /// <value>
        /// Type: <see cref="NavigationToolbarItem"/>
        /// </value>
        /// <seealso cref="NavigationToolbarItem"/>
        private NavigationToolbarItem FirstVisibleItem
        {
            get
            {
                NavigationToolbarItem item = null;
                
                if (MenuItemsCount < Items.Count)
                {
                    item = Items[MenuItemsCount] as NavigationToolbarItem;
                }

                return item;
            }
        }

        /// <summary>
        /// Gets the menu items count.
        /// </summary>
        /// <value>The menu items count.</value>
        private int MenuItemsCount
        {
            get
            {
                return m_menuItemsCount;
            }
        }
        
        /// <summary>
        /// Gets the first item pushed to the menu.
        /// </summary>
        /// <value>
        /// Type: <see cref="NavigationToolbarItem"/>
        /// </value>
        /// <seealso cref="NavigationToolbarItem"/>
        private NavigationToolbarItem FirstItemInMenu
        {
            get
            {
                NavigationToolbarItem item = null;

                if (MenuItemsCount > 0 && MenuItemsCount <= this.Items.Count)
                {
                    item = Items[MenuItemsCount - 1] as NavigationToolbarItem;
                }

                return item;
            }
        }

        /// <summary>
        /// Gets or sets the item margin.
        /// </summary>
        /// <value>The item margin.</value>
        public Thickness ItemMargin
        {
            get
            {
                return (Thickness)GetValue(ItemMarginProperty);
            }

            set
            {
                SetValue(ItemMarginProperty, value);
            }
        }
        #endregion

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the NavigationToolbar class.
        /// </summary>
        public NavigationToolbar()
            : base()
        {
            if (EnvironmentTestTools.IsSecurityGranted)
            {
                EnvironmentTestTools.StartValidateLicense(typeof(NavigationToolbar));
            }
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                if (Application.Current != null && !Syncfusion.Windows.Shared.BindingUtils.GetEnableBindingErrors(Application.Current.MainWindow))
                    System.Diagnostics.PresentationTraceSources.DataBindingSource.Switch.Level = System.Diagnostics.SourceLevels.Critical;
            }

            this.Loaded += new RoutedEventHandler(NavigationToolbar_Loaded);
        }

        /// <summary>
        /// Handles the Loaded event of the NavigationToolbar control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void NavigationToolbar_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        /// <summary>
        /// Initializes static members of the <see cref="NavigationToolbar"/> class.
        /// </summary>
        static NavigationToolbar()
        {
            //EnvironmentTest.ValidateLicense(typeof(NavigationToolbar));
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(NavigationToolbar),
                new FrameworkPropertyMetadata(typeof(NavigationToolbar)));

            WidthProperty.OverrideMetadata(
                typeof(NavigationToolbar),
                new FrameworkPropertyMetadata(OnWidthChanged));
        }
        
        /// <summary>
        /// Initializes the control in the given template.
        /// </summary>
        /// <param name="inTemplate">Template to initialize control in.</param>
        private void Initialize(ControlTemplate inTemplate)
        {
            m_buttonsMenu = inTemplate.FindName("ButtonsMenu", this) as MenuItem;
            if (m_buttonsMenu == null)
            {
                throw new ApplicationException("ButtonsMenu not found");
            }

            m_mainMenu = inTemplate.FindName("TopMenuItem", this) as MenuItem;
            
            if (m_mainMenu == null)
            {
                throw new ApplicationException("TopMenuItem not found");
            }

            m_optionsMenu = Template.FindName("OptionsMenu", this) as MenuItem;
            m_optionsMenu.Click += new RoutedEventHandler(OptionsCommandExecuting);
            
            Menu menu = m_mainMenu.Parent as Menu;

            menu.GotMouseCapture += new MouseEventHandler(ContextMenuOpening);
            menu.LostMouseCapture += new MouseEventHandler(ContextMenuClosing);
            //GroupBar gBar=null;
            if (menu != null)
            {
                menu.IsMouseCaptureWithinChanged += new DependencyPropertyChangedEventHandler(IsMouseCaptureWithInChanged);
                gBar = TemplatedParent as GroupBar;

                if (gBar != null)
                {
                    menu.IsEnabled = gBar.IsToolBarEnabled;
                }
            }

            AvailableWidth -= ActualMenuWidth + BorderThickness.Left + BorderThickness.Right;
            if (gBar != null)
            {
                if (gBar.ItemsSource == null)
                {
                    var toolbarItems = from GroupBarItem item in gBar.Items
                                       where !item.ShowInGroupBar && !gBar.HiddenIndices.Contains(gBar.Items.IndexOf(item))
                                       select item;

                    foreach (var item in toolbarItems)
                    {
                        NavigationToolbarItem navItem = new NavigationToolbarItem(item);
                        if (item.ShowInGroupBar)
                            Items.Add(navItem);
                    
                    }
                }
                else
                {
                    for (int i = 0; i < Items.Count; i++)
                    {
                        GroupBarItem item = gBar.ItemContainerGenerator.ContainerFromIndex(i) as GroupBarItem;
                        if (item != null)
                        {
                            if (!item.ShowInGroupBar && !gBar.HiddenIndices.Contains(gBar.Items.IndexOf(item)))
                            {
                                NavigationToolbarItem navItem = new NavigationToolbarItem(item);
                                Items.Add(navItem);
                            }
                        }
                    }
                }
            }

            RefreshButtonsMenu();
        }

        #endregion

        #region Implementation

        /// <summary>
        /// Contexts the menu opening.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        //SU I78477
        //private void ContextMenuOpening(object sender, MouseEventArgs e)
        private new void ContextMenuOpening(object sender, MouseEventArgs e)
            //EU I78477
        {
            GroupBar gBar = TemplatedParent as GroupBar;
            if (!m_mainMenu.IsSubmenuOpen)
            {
                gBar.OnNavigationMenuOpeningExecuted(m_mainMenu, new EventArgs());
            }
        }

        /// <summary>
        /// Contexts the menu closing.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        //SU I78477
        //private void ContextMenuClosing(object sender, MouseEventArgs e)
        private new void ContextMenuClosing(object sender, MouseEventArgs e)
            //EU I78477
        {
            GroupBar gBar = TemplatedParent as GroupBar;
            if (!m_mainMenu.IsSubmenuOpen)
            {
                gBar.OnNavigationMenuClosingExecuted(m_mainMenu, new System.ComponentModel.CancelEventArgs(true));
            }
        }

        /// <summary>
        /// Optionses the command executing.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void OptionsCommandExecuting(object sender, RoutedEventArgs e)
        {
            GroupBar gBar = TemplatedParent as GroupBar;
            gBar.OnNavigationOptionsMenuItemClickExecuted(sender, e);
        }

        /// <summary>
        /// Sets the menu enabled.
        /// </summary>
        /// <param name="newValue">if set to <c>true</c> [new value].</param>
        internal void SetMenuEnabled(bool newValue)
        {
            if (m_mainMenu != null)
            {
                m_mainMenu.IsEnabled = newValue;
            }
        }
        
        /// <summary>
        /// Coerces the value of <see cref="AvailableWidth"/> property. If trying to
        /// set negative value, visible items, if any, are pushed to
        /// menu, otherwise - if there are items in menu and enough space
        /// to host them, items are popped from menu.
        /// </summary>
        /// <param name="d">Object to which this property belongs.</param>
        /// <param name="value">Value that should be checked.</param>
        /// <returns>
        /// Checked value.
        /// </returns>
        private static object OnAvailableWidthCoerce(DependencyObject d, object value)
        {
            NavigationToolbar owner = d as NavigationToolbar;

            if (owner != null && owner.m_autoWidthCoerce)
            {
                double newAvailableWidth = (double)value;

                if (newAvailableWidth < 0)
                {
                    int count = (int)Math.Ceiling(Math.Abs(newAvailableWidth) / owner.ActualItemWidth);
                    double ret = newAvailableWidth;

                    while (owner.MenuItemsCount != owner.Items.Count && count-- > 0)
                    {
                        owner.PushToTopOfMenu(owner.FirstVisibleItem);
                        ret += owner.ActualItemWidth;
                    }

                    return ret;
                }
                else
                {
                    int count = (int)Math.Floor(newAvailableWidth / owner.ActualItemWidth);
                    double ret = newAvailableWidth;

                    while (owner.MenuItemsCount > 0 && count-- > 0)
                    {
                        owner.PopTopMenuItem();
                        ret -= owner.ActualItemWidth;
                    }

                    return ret;
                }
            }

            return value;
        }
        
        /// <summary>
        /// Called when the value of  property is changed.
        /// </summary>
        /// <param name="d">NavigationToolbar object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that
        /// contains the event data.</param>
        private static void OnWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NavigationToolbar owner = d as NavigationToolbar;

            if (owner != null)
            {
                double oldWidth = (double)e.OldValue;
                double newWidth = (double)e.NewValue;
                double widthChange = newWidth - oldWidth;

                if (!double.IsNaN(widthChange))
                {
                    owner.AvailableWidth += widthChange;
                }
            }
        }
        
        /// <summary>
        /// Refreshes menu buttons after drag and drop has been completed.
        /// </summary>
        /// <param name="source">The <see cref="Syncfusion.Windows.Tools.Controls.GroupBarItem"/> that is dragged.</param>
        /// <param name="target">The <see cref="Syncfusion.Windows.Tools.Controls.GroupBarItem"/> on which dragged item was dropped.</param>
        internal void RefreshButtonsMenu(GroupBarItem source, GroupBarItem target)
        {
            MenuItem sourceMenuItem = GetExistingMenuItem(m_buttonsMenu, source);
            MenuItem targetMenuItem = GetExistingMenuItem(m_buttonsMenu, target);

            int sourceIndex = ButtonsMenu.Items.IndexOf(sourceMenuItem);
            int targetIndex = ButtonsMenu.Items.IndexOf(targetMenuItem);

            ButtonsMenu.Items.Remove(sourceMenuItem);
            ButtonsMenu.Items.Insert(targetIndex, sourceMenuItem);
        }
        
        /// <summary>
        /// Refreshes menu buttons by replacing all menu items.
        /// </summary>
        internal void RefreshButtonsMenu()
        {
            if (LogicalParent != null)
            {
                m_buttonsMenu.Items.Clear();

                for(int i=0;i<LogicalParent.Items.Count;i++)
                {
                    GroupBarItem item;

                    if (LogicalParent.Items[i] is GroupBarItem)
                    {
                        item = LogicalParent.Items[i] as GroupBarItem;
                    }
                    else
                    {
                        item = LogicalParent.ItemContainerGenerator.ContainerFromIndex(i) as GroupBarItem;

                    }
                    if (item != null)
                    {
                        MenuItem mi = GetNewMenuItem(item, true, item.ShowInGroupBar);
                        m_buttonsMenu.Items.Add(mi);
                    }
                }
            }
        }
        
        /// <summary>
        /// Refreshes selected state for all items.
        /// </summary>
        internal void RefreshSelectedItems()
        {
            foreach (NavigationToolbarItem item in Items)
            {
                item.IsSelected = item.GroupBarItem.IsSelected;
            }
        }
        
        /// <summary>
        /// Creates the panel of the given size that hosts image and text.
        /// </summary>
        /// <param name="size">The size of the panel.</param>
        /// <param name="text">Text to host.</param>       
        /// <returns>
        /// Panel of the given size that hosts text and image.
        /// </returns>
        internal DockPanel CreateItemPanel(Size size, string text)
        {
            DockPanel panel = new DockPanel();
            panel.Height = size.Height;
            panel.Width = size.Width;

            TextBlock textBlock = new TextBlock();
            textBlock.Text = text;
            textBlock.VerticalAlignment = VerticalAlignment.Center;
            textBlock.Margin = new Thickness(5d, 0d, 0d, 0d);
            panel.Children.Add(textBlock);

            return panel;
        }
        
        /// <summary>
        /// Creates the panel of the given size that hosts image and text.
        /// </summary>
        /// <param name="size">The size of the panel.</param>
        /// <param name="text">Text to host.</param>
        /// <param name="imageSource">The image source to host.</param>
        /// <returns>
        /// Panel of the given size that hosts text and image.
        /// </returns>
        internal DockPanel CreateItemPanel(Size size, string text, ImageSource imageSource)
        {
            DockPanel panel = new DockPanel();
            panel.Height = size.Height;
            panel.Width = size.Width;

            Image image = new Image();
            image.Source = imageSource;
            image.VerticalAlignment = VerticalAlignment.Center;
            panel.Children.Add(image);

            TextBlock textBlock = new TextBlock();
            textBlock.Text = text;
            textBlock.VerticalAlignment = VerticalAlignment.Center;
            textBlock.Margin = new Thickness(5d, 0d, 0d, 0d);
            panel.Children.Add(textBlock);

            return panel;
        }
        
        /// <summary>
        /// Finds <see cref="Syncfusion.Windows.Tools.Controls.NavigationToolbarItem"/> for given GroupBarItem.
        /// </summary>
        /// <param name="forItem">Item to search <see cref="Syncfusion.Windows.Tools.Controls.NavigationToolbarItem"/> for.</param>
        /// <returns>The <see cref="Syncfusion.Windows.Tools.Controls.NavigationToolbarItem"/> that presents
        /// the given <see cref="Syncfusion.Windows.Tools.Controls.GroupBarItem"/>.
        /// </returns>
        internal NavigationToolbarItem GetToolbarItem(GroupBarItem forItem)
        {
            NavigationToolbarItem navItem = null;

            if (forItem != null)
            {
                foreach (NavigationToolbarItem item in Items)
                {
                    if (item.GroupBarItem == forItem)
                    {
                        navItem = item;
                    }
                }
            }

            return navItem;
        }
        
        /// <summary>
        /// Gets existing menu item that presents given <see cref="Syncfusion.Windows.Tools.Controls.GroupBarItem"/>.
        /// </summary>
        /// <param name="inMenuItem">The <see cref="System.Windows.Controls.MenuItem"/> to search in.</param>
        /// <param name="forItem">The <see cref="Syncfusion.Windows.Tools.Controls.GroupBarItem"/> to search menu item for.</param>
        /// <returns>
        /// The <see cref="System.Windows.Controls.MenuItem"/> that presents 
        /// the given <see cref="Syncfusion.Windows.Tools.Controls.GroupBarItem"/>.
        /// </returns>
        private MenuItem GetExistingMenuItem(MenuItem inMenuItem, GroupBarItem forItem)
        {
            MenuItem item = null;

            if (inMenuItem != null && forItem != null)
            {
                for (int i = 0, length = inMenuItem.Items.Count; i < length; i++)
                {
                    MenuItem mi = inMenuItem.Items[i] as MenuItem;

                    if (mi != null && mi.Tag as GroupBarItem == forItem)
                    {
                        item = mi;
                    }
                }
            }

            return item;
        }
        
        /// <summary>
        /// Gets new menu item for the given <see cref="Syncfusion.Windows.Tools.Controls.GroupBarItem"/>.
        /// </summary>
        /// <param name="forItem">The <see cref="Syncfusion.Windows.Tools.Controls.GroupBarItem"/> to create menu item for.</param>
        /// <param name="isCheckable">Defines whether menu item must be checkable.</param>
        /// <param name="isChecked">Defines whether menu item must be checked.</param>
        /// <returns>
        /// The <see cref="System.Windows.Controls.MenuItem"/> item with the given properties.
        /// </returns>
        internal MenuItem GetNewMenuItem(GroupBarItem forItem, bool isCheckable, bool isChecked)
        {
            var visibleItems = from NavigationToolbarItem item in Items
                               where item.GroupBarItem == forItem
                               select item;

            if (visibleItems.Count() != 0 && !isChecked)
            {
                isChecked = true;
            }

            MenuItem mi = new MenuItem();
            mi.IsCheckable = true;
            mi.Tag = forItem;
            mi.StaysOpenOnClick = true;
            mi.IsChecked = isChecked;

            CheckBox icon = CreateMenuItemIcon(mi, forItem.HeaderImageSource);
            ToggleButton header = new ToggleButton();

            if (forItem.LogicalParent.ItemsSource != null)
            {
                if (null != forItem.DataContext)
                {
                    header.Content = forItem.Content;

                    if (forItem.HeaderTemplateSelector != null || forItem.HeaderTemplate != null)
                    {
                        if (forItem.HeaderTemplateSelector != null)
                        {
                            DataTemplate temp = ((DataTemplateSelector)forItem.HeaderTemplateSelector).SelectTemplate(forItem.Content, forItem);
                            header.ContentTemplate = temp;
                        }
                        else
                        {
                            header.ContentTemplate = forItem.HeaderTemplate;
                        }
                    }
                    else
                    {
                        if (forItem.LogicalParent.ItemTemplateSelector != null)
                        {
                            DataTemplate temp = ((DataTemplateSelector)forItem.LogicalParent.ItemTemplateSelector).SelectTemplate(forItem.Content, forItem);
                            header.ContentTemplate = temp;
                        }
                        else
                        {
                            header.ContentTemplate = forItem.LogicalParent.ItemTemplate;
                        }
                    }
                }
            }
            else
            {
                header = CreateMenuItemHeader(mi, forItem.HeaderText);
            }

            if (icon != null && header != null)
            {
                Binding bind = new Binding();
                bind.Source = mi;
                bind.Path = new PropertyPath("IsChecked");
                header.SetBinding(CheckBox.IsCheckedProperty, bind);

                bind = new Binding();
                bind.Source = header;
                bind.Path = new PropertyPath("IsChecked");
                icon.SetBinding(CheckBox.IsCheckedProperty, bind);

                mi.Header = header;
                mi.Icon = icon;
            }

            mi.Checked += delegate(object sender, RoutedEventArgs args)
                {
                    LogicalParent.ShowItem(forItem);
                };

            mi.Unchecked += delegate(object sender, RoutedEventArgs args)
                {
                    LogicalParent.HideItem(forItem);
                };

            return mi;
        }
        
        /// <summary>
        /// Gets new menu item for the given <see cref="Syncfusion.Windows.Tools.Controls.GroupBarItem"/>.
        /// </summary>
        /// <param name="forItem">The <see cref="Syncfusion.Windows.Tools.Controls.GroupBarItem"/> to create menu item for.</param>
        /// <returns>
        /// The <see cref="System.Windows.Controls.MenuItem"/> that presents given <see cref="Syncfusion.Windows.Tools.Controls.GroupBarItem"/>.
        /// </returns>
        private MenuItem GetNewMenuItem(GroupBarItem forItem)
        {
            return GetNewMenuItem(forItem, false, false);
        }
        
        /// <summary>
        /// Creates menu item header for the given <see cref="Syncfusion.Windows.Tools.Controls.GroupBarItem"/>.
        /// </summary>
        /// <param name="forItem">The <see cref="System.Windows.Controls.MenuItem"/> to create menu item header for.</param>
        /// <param name="text">Text to be hosted in the header.</param>
        /// <returns>
        /// New menu item header that presents the given <see cref="Syncfusion.Windows.Tools.Controls.GroupBarItem"/>.
        /// </returns>
        private ToggleButton CreateMenuItemHeader(MenuItem forItem, string text)
        {
            ToggleButton button = new ToggleButton();
            button.Content = CreateItemPanel(new Size(Double.NaN, DEF_MENUITEM_HEADER_HEIGHT), text);
            return button;
        }
        
        /// <summary>
        /// Creates menu item icon for the given <see cref="Syncfusion.Windows.Tools.Controls.GroupBarItem"/>.
        /// </summary>
        /// <param name="forItem">The <see cref="System.Windows.Controls.MenuItem"/> to create menu item header for.</param>
        /// <param name="imageSource">Source of the image to be hosted in the header.</param>
        /// <returns>
        /// New menu item icon that presents given <see cref="Syncfusion.Windows.Tools.Controls.GroupBarItem"/>.
        /// </returns>
        private CheckBox CreateMenuItemIcon(MenuItem forItem, ImageSource imageSource)
        {
            CheckBox checkBox = new CheckBox();
            Image image = new Image();
            image.Source = imageSource;
            image.VerticalAlignment = VerticalAlignment.Center;
            image.HorizontalAlignment = HorizontalAlignment.Center;
            checkBox.Content = image;
            checkBox.MaxHeight = DEF_MENUITEM_HEADER_HEIGHT + 1d;
            checkBox.Margin = new Thickness(1d);
            return checkBox;
        }
        
        /// <summary>
        /// Gets the index in the menu for the given <see cref="Syncfusion.Windows.Tools.Controls.NavigationToolbarItem"/>.
        /// </summary>
        /// <param name="item">The <see cref="Syncfusion.Windows.Tools.Controls.NavigationToolbarItem"/> to search index for.</param>
        /// <returns>
        /// Index of the given <see cref="Syncfusion.Windows.Tools.Controls.NavigationToolbarItem"/> in the menu.
        /// </returns>
        private int GetItemIndexInMenu(NavigationToolbarItem item)
        {
            int index = -1;

            if (item != null)
            {
                for (int i = MainMenu.Items.Count - 1; i >= 0; --i)
                {
                    MenuItem mi = MainMenu.Items[i] as MenuItem;
                    if (mi != null)
                    {
                        GroupBarItem gbItem = mi.Tag as GroupBarItem;

                        if (gbItem != null && gbItem == item.GroupBarItem)
                        {
                            index = i;
                        }
                    }
                }
            }

            return index;
        }
        
        /// <summary>
        /// Calculates the index in the menu for the given <see cref="Syncfusion.Windows.Tools.Controls.NavigationToolbarItem"/> for
        /// insertion into the menu.
        /// </summary>
        /// <param name="item"> The <see cref="Syncfusion.Windows.Tools.Controls.NavigationToolbarItem"/> to calculate index for.</param>
        /// <returns>
        /// Index for the given <see cref="Syncfusion.Windows.Tools.Controls.NavigationToolbarItem"/> in the menu.
        /// </returns>
        private int CalculateItemIndexInMenu(NavigationToolbarItem item)
        {
            return MainMenu.Items.Count - Items.IndexOf(item);
        }
        
        /// <summary>
        /// Defines whether the given <see cref="Syncfusion.Windows.Tools.Controls.NavigationToolbarItem"/> is in the menu.
        /// </summary>
        /// <param name="item">The <see cref="Syncfusion.Windows.Tools.Controls.NavigationToolbarItem"/> to check.</param>
        /// <returns>
        /// Value indicating whether the given <see cref="Syncfusion.Windows.Tools.Controls.NavigationToolbarItem"/> is in the menu.
        /// </returns>
        private bool IsItemInMenu(NavigationToolbarItem item)
        {
            bool bItemInMenu = false;

            if (item != null)
            {
                int index = Items.IndexOf(item);

                if (index < Items.Count - 1)
                {
                    NavigationToolbarItem navigationItem = Items[index + 1] as NavigationToolbarItem;

                    if (navigationItem != null && navigationItem.ShowInToolbar)
                    {
                        bItemInMenu = true;
                    }
                }

                if (AvailableWidth < ActualItemWidth && FirstVisibleItem == null)
                {
                    bItemInMenu = true;
                }
            }

            return bItemInMenu;
        }
        
        /// <summary>
        /// Pushes the given <see cref="Syncfusion.Windows.Tools.Controls.NavigationToolbarItem"/> to the top of the menu.
        /// </summary>
        /// <param name="item">The <see cref="Syncfusion.Windows.Tools.Controls.NavigationToolbarItem"/> to push to top of menu.</param>
        private void PushToTopOfMenu(NavigationToolbarItem item)
        {
            InsertToMenu(item, MainMenu.Items.Count - MenuItemsCount);
        }
        
        /// <summary>
        /// Pops top menu item into the toolbar.
        /// </summary>
        private void PopTopMenuItem()
        {
            if (FirstItemInMenu != null)
            {
                FirstItemInMenu.ShowInToolbar = true;
                RemoveFromMenu(MainMenu.Items.Count - MenuItemsCount);
            }
        }
        
        /// <summary>
        /// Inserts the given <see cref="Syncfusion.Windows.Tools.Controls.NavigationToolbarItem"/> into the menu at specified index.
        /// </summary>
        /// <param name="item">The <see cref="Syncfusion.Windows.Tools.Controls.NavigationToolbarItem"/> to insert to the menu.</param>
        /// <param name="indexInMenu">Insertion index.</param>
        private void InsertToMenu(NavigationToolbarItem item, int indexInMenu)
        {
            MenuItem mi = GetNewMenuItem(item.GroupBarItem);
            
            mi.Click += delegate(object sender, RoutedEventArgs args)
                {
                    if (LogicalParent != null)
                    {
                        LogicalParent.SelectedObject = item;
                    }
                };

            if (m_menuItemsCount == 0)
            {
                MainMenu.Items.Add(new Separator());
                ++indexInMenu;
            }

            MainMenu.Items.Insert(indexInMenu, mi);
            item.ShowInToolbar = false;
            ++m_menuItemsCount;
        }
        
        /// <summary>
        /// Removes menu item at specified index.
        /// </summary>
        /// <param name="index">Index to remove item at.</param>
        private void RemoveFromMenu(int index)
        {
            if (index != -1)
            {
                MainMenu.Items.RemoveAt(index);
                --m_menuItemsCount;

                if (m_menuItemsCount == 0)
                {
                    MainMenu.Items.RemoveAt(MainMenu.Items.Count - 1);
                }
            }
        }
        
        /// <summary>
        /// Builds the current template's visual tree if necessary.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            Initialize(Template);
        }
        
        /// <summary>
        /// Called when the selection changes.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> that
        /// contains the event data.</param>
        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);

            if (SelectedItem != null && LogicalParent != null)
            {
                LogicalParent.SelectedObject = SelectedItem.GroupBarItem;
            }
            else if (SelectedItem == null && LogicalParent != null)
            {
                LogicalParent.SelectedObject = null;
            }
        }
        
        /// <summary>
        /// Invoked when an unhandled attached event reaches 
        /// an element in its route that is derived from this class. 
        /// Implement this method to add class handling for this event. 
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> that 
        /// contains the event data. 
        /// This event data reports details about the mouse button that was pressed 
        /// and the handled state.</param>
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            LogicalParent.Focus();
        }
        
        /// <summary>
        /// Updates the current selection when an item in the Selector has changed.
        /// </summary>
        /// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> that
        /// contains the event data. </param>
        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);
            
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                m_autoWidthCoerce = false;

                for (int i = 0, len = e.NewItems.Count; i < len; ++i)
                {
                    double newAvaliableWidth = AvailableWidth - ActualItemWidth;
                    NavigationToolbarItem addedItem = e.NewItems[i] as NavigationToolbarItem;

                    if (!IsItemInMenu(addedItem))
                    {
                        if (newAvaliableWidth < 0)
                        {
                            if (IsItemInMenu(addedItem))
                            {
                                NavigationToolbarItem itemInMenu = addedItem;
                                int indexInMenu = CalculateItemIndexInMenu(addedItem);
                                InsertToMenu(itemInMenu, indexInMenu);
                            }
                            else
                            {
                                PushToTopOfMenu(e.NewItems[0] as NavigationToolbarItem);
                            }
                        }
                        else
                        {
                            AvailableWidth = newAvaliableWidth;
                        }
                    }
                }

                m_autoWidthCoerce = true;
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                m_autoWidthCoerce = false;

                for (int i = 0, len = e.OldItems.Count; i < len; ++i)
                {
                    NavigationToolbarItem item = e.OldItems[i] as NavigationToolbarItem;

                    if (item.ShowInToolbar)
                    {
                        double newAvailableWidth = AvailableWidth + ActualItemWidth;
                        if (MenuItemsCount != 0)
                        {
                            PopTopMenuItem();
                        }
                        else
                        {
                            AvailableWidth += ActualItemWidth;
                        }
                    }
                    else
                    {
                        int indexInMenu = GetItemIndexInMenu(item);
                        RemoveFromMenu(indexInMenu);
                    }
                }

                m_autoWidthCoerce = true;
            }
            
            RefreshSelectedItems();
            
        }
        
        /// <summary>
        /// Raises the <see cref="OnInitialized"/> event. 
        /// This method is invoked whenever Initialized property is set to true internally. 
        /// </summary>
        /// <param name="e">The <see cref="RoutedEventArgs"/> that contains the event data.</param>
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
        }
        
        /// <summary>
        /// Called when IsMouseCaptureWithin property is changed.
        /// </summary>
        /// <param name="sender">Source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that 
        /// contains the event data.</param>
        private void IsMouseCaptureWithInChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.IsMouseCaptureWithin)
            {
                if (!m_bFocused)
                {
                    Focus();
                    m_bFocused = true;
                }
            }
        }
        
        /// <summary>
        /// Raises the <see cref="System.Windows.UIElement.LostFocus"/> routed event by using the event data that is provided.
        /// </summary>
        /// <param name="e">A <see cref="System.Windows.RoutedEventArgs"/> that contains the event data. 
        /// This event data must contain the identifier for the <see cref="System.Windows.UIElement.LostFocus"/> event.</param>
        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);
            m_bFocused = false;
        }
        #endregion
    }
}
