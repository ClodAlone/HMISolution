#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
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
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Presents TabControlAdv panel.
    /// </summary>
    public class TabPanelAdv : ContentControl
    {
        #region Private members
        /// <summary>
        /// Close button.
        /// </summary>
        private CloseButton closeButton;

        /// <summary>
        /// Menu button.
        /// </summary>
        private MenuButton menuButton;

        /// <summary>
        /// Scrolling panel.
        /// </summary>
        private ScrollingPanel scrollingPanel;

        /// <summary>
        /// Tab items.
        /// </summary>
        private FrameworkElement content;

        /// <summary>
        /// Tabcontrol parent.
        /// </summary>
        private TabControlAdv tabControlParent;

        /// <summary>
        /// Tab list menu.
        /// </summary>
        private TabPopupMenu tabMenu = null;

        /// <summary>
        /// Menu panel.
        /// </summary>
        private StackPanel menuPanel;

        /// <summary>
        /// Buttons border.
        /// </summary>
        private Border buttonsBorder;

        /// <summary>
        /// Tab list menu offset.
        /// </summary>
        private Point popupMenuOffset = new Point(0, 15);

        /// <summary>
        /// The grid.
        /// </summary>
        private Grid mtabPanelAdvGrid=null;
        #endregion

       
        #region Initialization
        /// <summary>
        /// Initializes new instance of the TabPanelAdv class.
        /// </summary>
        public TabPanelAdv()
        {
            DefaultStyleKey = typeof(TabPanelAdv);
           
        }
        #endregion

        #region Properies
        /// <summary>
        /// Gets a value indicating whether all items are visible.
        /// </summary>
        public bool IsAllItemsVisible
        {
            get
            {
                return (bool)GetValue(IsAllItemsVisibleProperty);
            }

            internal set
            {
                SetValue(IsAllItemsVisibleProperty, value);
            }
        }

        /// <summary>
        /// Gets the scrolling panel.
        /// </summary>
        internal ScrollingPanel ScrollingPanel
        {
            get
            {
                return scrollingPanel;
            }
        }

        /// <summary>
        /// Gets the close button.
        /// </summary>
        internal CloseButton CloseButton
        {
            get
            {
                return closeButton;
            }
        }

        /// <summary>
        /// Gets the menu button.
        /// </summary>
        internal MenuButton MenuButton
        {
            get
            {
                return menuButton;
            }
        }

        /// <summary>
        /// Gets the menu panel.
        /// </summary>
        internal StackPanel MenuPanel
        {
            get
            {
                return menuPanel;
            }
        }

        /// <summary>
        /// Gets the buttons border.
        /// </summary>
        internal Border ButtonsBorder
        {
            get
            {
                return buttonsBorder;
            }
        }

        /// <summary>
        /// Gets the content element.
        /// </summary>
        internal FrameworkElement ContentElement
        {
            get
            {
                return content;
            }
        }

        /// <summary>
        /// Gets or sets the tabcontrol parent.
        /// </summary>
        internal TabControlAdv TabControlParent
        {
            get
            {
                return tabControlParent;
            }

            set
            {
                tabControlParent = value;
            }
        }

        /// <summary>
        /// Gets or sets the tab panel grid.
        /// </summary>
        internal Grid TabPanelGrid
        {
            get
            {
                return mtabPanelAdvGrid;
            }
        }
        #endregion

        #region DP properties
        /// <summary>
        /// Identifies the <see cref="IsAllItemsVisible"/> dependency property.
        /// </summary>
        protected static readonly DependencyProperty IsAllItemsVisibleProperty =
            DependencyProperty.Register("IsAllItemsVisible", typeof(bool), typeof(TabPanelAdv), new PropertyMetadata(true));
        #endregion

        #region Overrides
        /// <summary>
        /// Invoked whenever application code or internal processes (such as a rebuilding layout pass) 
        /// call System.Windows.Controls.Control.ApplyTemplate() method.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            scrollingPanel = this.GetTemplateChild("PART_ScrollingPanel") as ScrollingPanel;
            closeButton = this.GetTemplateChild("PART_CloseButton") as CloseButton;
            if (closeButton != null)
            {
                closeButton.Click += new RoutedEventHandler(CloseButtonClick);
            }

            menuButton = this.GetTemplateChild("PART_MenuButton") as MenuButton;
            if (menuButton != null)
            {
                menuButton.Click += new RoutedEventHandler(MenuButtonClicked);
            }

            menuPanel = this.GetTemplateChild("MenuPanel") as StackPanel;
            content = this.GetTemplateChild("PART_TabItems") as FrameworkElement;

            tabMenu = this.GetTemplateChild("TabPopupMenu") as TabPopupMenu;
            if (tabMenu != null)
            {
                tabMenu.LostFocus += new RoutedEventHandler(TabMenuLostFocus);
                tabMenu.Opened += new RoutedEventHandler(TabMenuOpened);
                tabMenu.Closed += new RoutedEventHandler(TabMenuClosed);

                //this.Dispatcher.BeginInvoke(delegate
                //    {
                        tabMenu.TabControlParent = this.TabControlParent;
                        if (this.TabControlParent != null)
                        {
                            this.TabControlParent.UpdateCloseButtonsVisibility();
                            if (TabControlParent.TabPanelBackground != null)
                                this.Background = TabControlParent.TabPanelBackground;
                            //this.TabControlParent.SizeChanged += new SizeChangedEventHandler(TabControlParentSizeChanged);
                            //if (mtabPanelAdvGrid != null)
                            //{
                            //    mtabPanelAdvGrid.Width = this.TabControlParent.ActualWidth;
                            //}
                        }
                    //});
            }

            //mtabPanelAdvGrid = this.GetTemplateChild("TabPanelAdvGrid") as Grid;
            this.buttonsBorder = this.GetTemplateChild("ButtonsBorder") as Border;
            if (tabMenu != null)
            {
                tabMenu.MenuItems.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(MenuItems_CollectionChanged);
            }
        }

        /// <summary>
        /// Handles the CollectionChanged event of the MenuItems control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        void MenuItems_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)

                foreach (TabPopupMenuItem elem in e.NewItems)
                {
                    if (elem.MenuParent == null)
                        elem.MenuParent = this.tabMenu;
                }
        }

        #endregion

        #region Implementation
        /// <summary>
        /// Occurs when either the System.Windows.FrameworkElement.ActualHeight or the
        /// System.Windows.FrameworkElement.ActualWidth properties change value on a
        /// System.Windows.FrameworkElement.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void TabControlParentSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.TabControlParent != null)
            {
                if (this.TabControlParent.TabStripPlacement == TabStripPlacement.Top
                    || this.TabControlParent.TabStripPlacement == TabStripPlacement.Bottom)
                {
                    mtabPanelAdvGrid.Width = this.TabControlParent.ActualWidth;
                }
                else
                {
                    mtabPanelAdvGrid.Width = this.TabControlParent.ActualHeight;
                }
            }
        }

        /// <summary>
        /// Occurs when tab menu is opened.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void TabMenuOpened(object sender, RoutedEventArgs e)
        {
            if (this.TabControlParent != null)
            {
                //this.TabControlParent.FireDropDownContextMenuOpen(this.tabMenu.MenuItems);
            }
        }

        /// <summary>
        /// Occurs when tab menu is closed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void TabMenuClosed(object sender, RoutedEventArgs e)
        {
            if (this.TabControlParent != null)
            {
                this.TabControlParent.FireDropDownContextMenuClose(this.tabMenu.MenuItems);
            }
        }

        /// <summary>
        /// Occurs when tab menu losts focus.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void TabMenuLostFocus(object sender, RoutedEventArgs e)
        {
            if (!menuButton.IsMouseOver)
            {
                tabMenu.Close();
            }
        }

        /// <summary>
        /// Occurs when the menu item is selected.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void MenuItemIsSelectedChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            TabPopupMenuItem menuItem = (TabPopupMenuItem)sender;

            if (menuItem.Tag is TabItemAdv)
            {
                this.TabControlParent.SelectedItem = menuItem.Tag as TabItemAdv;
            }

            tabMenu.Close();
        }

        /// <summary>
        /// Updates tab menu.
        /// </summary>
        private void PrepareTabMenu()
        {
            if (this.TabControlParent != null && tabMenu != null)
            {
                RotateTransform rotate = new RotateTransform();
                switch (this.TabControlParent.TabStripPlacement)
                {
                    case TabStripPlacement.Top:
                        rotate.Angle = 0d;
                        tabMenu.HorizontalOffset = 0;
                        tabMenu.VerticalOffset = this.MenuButton.ActualHeight;
                        rotate.CenterX = tabMenu.ActualWidth / 2;
                        rotate.CenterY = tabMenu.ActualHeight / 2;
                        break;
                    case TabStripPlacement.Left:
                        rotate.Angle = 90d;
                        rotate.CenterX = 0;
                        rotate.CenterY = tabMenu.ActualHeight / 2;
                        tabMenu.HorizontalOffset = this.MenuButton.ActualHeight;
                        tabMenu.VerticalOffset = 3;
                        break;
                    case TabStripPlacement.Right:
                        rotate.Angle = -90d;
                        rotate.CenterX = 0;
                        rotate.CenterY = tabMenu.ActualHeight / 2;
                        tabMenu.HorizontalOffset = -(this.MenuButton.ActualHeight + this.tabMenu.GetMenuSize().Width + 2);
                        tabMenu.VerticalOffset = 0;
                        break;
                    case TabStripPlacement.Bottom:
                        rotate.Angle = 180d;
                        rotate.CenterX = tabMenu.ActualWidth / 2;
                        rotate.CenterY = tabMenu.ActualHeight / 2;
                        tabMenu.HorizontalOffset = 0;
                        tabMenu.VerticalOffset = -(this.MenuButton.ActualHeight + this.tabMenu.GetMenuSize().Height + 2);
                        break;
                }

                tabMenu.RenderTransform = rotate;

                //if (tabControlParent.VisualStyle == TabControlVisualStyle.Blend ||
                //    tabControlParent.VisualStyle == TabControlVisualStyle.Office2007Black)
                //{
                //    this.Foreground = new SolidColorBrush(Colors.White);
                //}
            }
        }

        /// <summary>
        /// Occurs when menu button is clicked.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void MenuButtonClicked(object sender, RoutedEventArgs e)
        {
            if (this.TabControlParent != null && tabMenu != null)
            {
                if (!tabMenu.IsOpen)
                {
                    
                   // tabMenu.MenuItems.Clear();
                               
                    menuButton = e.OriginalSource as MenuButton;
                    tabMenu.PlacementTarget = menuButton;
                    TabPopupMenuItemCollection newcollection = new TabPopupMenuItemCollection();
                    for (int i = 0; i < tabControlParent.TabHeaders.Count; i++)
                    {
                        TabItemAdv item = tabControlParent.TabHeaders[i] as TabItemAdv;
                        if (item != null && item.Visibility == Visibility.Collapsed)
                        {
                            continue;
                        }

                        TabPopupMenuItem newItem = new TabPopupMenuItem();
                        if (item.TabItemParent != null)
                        {
                            newItem.HeaderImage = item.TabItemParent.Image;
                            newItem.Tag = item.TabItemParent;
                            if (item.TabItemParent.HeaderTemplate != null)
                            {
                                newItem.HeaderTextTemplate = item.TabItemParent.HeaderTemplate;
                            }

                            newItem.HeaderText = item.TabItemParent.Header;
                        }

                        newItem.IsSelectedChanged += new PropertyChangedCallback(MenuItemIsSelectedChanged);
                        newItem.MenuParent = tabMenu;
                        newcollection.Add(newItem);
                    }

                    int oldcount = tabMenu.MenuItems.Count;
                    int tempindex = 0;
                    TabPopupMenuItemCollection additems= new TabPopupMenuItemCollection();
                  
                    foreach (TabPopupMenuItem nitem in newcollection)
                    {
                      
                        foreach (TabPopupMenuItem oitem in tabMenu.MenuItems)
                        {
                            if (nitem.Tag.Equals(oitem.Tag))
                            {
                                tempindex = 0;
                                break;
                            }
                            tempindex++;
                        }
                        if (tempindex == oldcount)
                        {
                            additems.Add(nitem);
                            tempindex = 0;
                        }
                    }

                    int newcount = newcollection.Count;
                    tempindex = 0;
                    
                    TabPopupMenuItemCollection deleteitems = new TabPopupMenuItemCollection();

                    foreach (TabPopupMenuItem oitem in tabMenu.MenuItems)
                    {
                        foreach (TabPopupMenuItem nitem in newcollection)
                        {
                            if (oitem.Tag != null && oitem.Tag.Equals(nitem.Tag) || oitem.IsCustomPopupMenuItem)
                            {
                                tempindex = 0;
                                break;
                            }
                             
                            tempindex++;
                        }
                        if (tempindex == newcount)
                        {
                            deleteitems.Add(oitem);
                            tempindex = 0;
                        }                          
                    }
                    
                    foreach (TabPopupMenuItem item in additems)
                        tabMenu.MenuItems.Add(item);
                    foreach (TabPopupMenuItem item in deleteitems)
                        tabMenu.MenuItems.Remove(item);

                    tabMenu.Focus();
                    this.PrepareTabMenu();
                    tabMenu.Show();
                    //tabMenu.MenuItems.UpdatePanelChildren();
                    FrameworkElement root = Application.Current.RootVisual as FrameworkElement;
                    GeneralTransform gt = tabMenu.TransformToVisual(root as UIElement);
                    Point offset = gt.Transform(new Point(0, 0));
                    double tabMenuTop = offset.Y + tabMenu.VerticalOffset;
                    double tabMenuLeft = offset.X + tabMenu.HorizontalOffset;
                    if (tabMenuLeft + tabMenu.GetMenuSize().Width > root.ActualWidth)
                    {
                        tabMenu.HorizontalOffset -= tabMenuLeft + tabMenu.GetMenuSize().Width - root.ActualWidth + 3;
                    }

                    if (tabMenuTop + tabMenu.GetMenuSize().Height > root.ActualHeight)
                    {
                        tabMenu.VerticalOffset -= tabMenuTop + tabMenu.GetMenuSize().Height - root.ActualHeight + 3;
                    }
                   
                }
                else
                {
                    tabMenu.Close();
                }
            }
        }

        /// <summary>
        /// Occurs when close button is clicked.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void CloseButtonClick(object sender, RoutedEventArgs e)
        {
            if (this.TabControlParent != null)
            {
                this.TabControlParent.CloseSelectedTabItem();
            }
        }
        #endregion
    }
}
