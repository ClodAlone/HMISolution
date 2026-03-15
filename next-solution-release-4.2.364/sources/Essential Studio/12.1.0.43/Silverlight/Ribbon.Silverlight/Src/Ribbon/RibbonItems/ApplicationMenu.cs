#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace Syncfusion.Windows.Tools.Controls
{
    using System;
    using System.Collections;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Controls.Primitives;
    using System.ComponentModel;
    using System.Windows.Shapes;

    /// <summary>
    /// Represent's <see cref="ApplicationMenu"/>'s menu.
    /// </summary>
    public class ApplicationMenu :
        RibbonMenu,IDisposable
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationMenu"/> class.
        /// </summary>
        public ApplicationMenu()
        {
            this.DefaultStyleKey = typeof(ApplicationMenu);
            Application.Current.Host.Content.Resized += new EventHandler(this.OnHostContentResized);
            this.Unloaded += new RoutedEventHandler(ApplicationMenu_Unloaded);
        }

        void ApplicationMenu_Unloaded(object sender, RoutedEventArgs e)
        {
           
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        [Category("Common Properties")]
        [Description("Represents the image to be displayed on the Application Menu Button")]
        public ImageSource ApplicationButtonImage
        {
            get { return (ImageSource)GetValue(ApplicationButtonImageProperty); }
            set { SetValue(ApplicationButtonImageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Icon.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty ApplicationButtonImageProperty =
            DependencyProperty.Register("ApplicationButtonImage", typeof(ImageSource), typeof(ApplicationMenu), new PropertyMetadata(null));

        #region SystemItemsPane

        /// <summary>
        /// Gets or sets the system items.
        /// </summary>
        /// <value>The system items.</value>
        [Category("Common Properties")]
        [Description("Used to Add System Item in Application Menu")]
        public RibbonItemsControl SystemItemsPane
        {
            get { return (RibbonItemsControl)GetValue(SystemItemsPaneProperty); }
            set { SetValue(SystemItemsPaneProperty, value); }
        }

        /// <summary>
        /// Identifier for <see cref="SystemItemsPane"/> property.
        /// </summary>
        public static readonly DependencyProperty SystemItemsPaneProperty = DependencyProperty.Register(
            "SystemItemsPane",
            typeof(RibbonItemsControl),
            typeof(ApplicationMenu),
            new PropertyMetadata(new PropertyChangedCallback(SystemItemsPaneChangedCallback)));

        /// <summary>
        /// Systems the items pane changed callback.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void SystemItemsPaneChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ApplicationMenu)d).OnSystemItemsPaneChanged(e.OldValue as RibbonItemsControl, e.NewValue as RibbonItemsControl);
        }

        #endregion

        #region MenuItemsPane

        /// <summary>
        /// Gets or sets the menu items.
        /// </summary>
        /// <value>The menu items.</value>
        [Category("Common Properties")]
        [Description("Used to Add Menu Item in Application Menu")]
        public RibbonItemsControl MenuItemsPane
        {
            get { return (RibbonItemsControl)GetValue(MenuItemsPaneProperty); }
            set { SetValue(MenuItemsPaneProperty, value); }
        }

        /// <summary>
        /// Identifier for <see cref="MenuItemsPane"/> property.
        /// </summary>
        public static readonly DependencyProperty MenuItemsPaneProperty = DependencyProperty.Register("MenuItemsPane", typeof(RibbonItemsControl), typeof(ApplicationMenu), new PropertyMetadata(new PropertyChangedCallback(MenuItemsPaneChangedCallback)));

        /// <summary>
        /// Menus the items pane changed callback.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void MenuItemsPaneChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ApplicationMenu)d).OnMenuItemsPaneChanged(e.OldValue as RibbonItemsControl, e.NewValue as RibbonItemsControl);
        }

        #endregion

        #endregion

        #region Overrides

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes (such as a rebuilding layout pass) call <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (this.menuPanelPresenter != null)
            {
                this.menuPanelPresenter.Content = null;
            }

            if (this.systemPanelPresenter != null)
            {
                this.systemPanelPresenter.Content = null;
            }

            this.Part_RootButton = this.GetTemplateChild("Part_RootButton") as Grid;
            this.PART_ApplicationSplitPopup = this.GetTemplateChild("PART_ApplicationSplitPopup") as Popup;
            this.PART_RibbonDropDown = this.GetTemplateChild("PART_RibbonDropDown") as Popup;
            this.menuPanelPresenter = this.GetTemplateChild("Part_MenuPanelPresenter") as ContentPresenter;
            this.Part_MainPanelPresenter = this.GetTemplateChild("Part_MainPanelPresenter") as ContentPresenter;
            this.systemPanelPresenter = this.GetTemplateChild("Part_SystemPanelPresenter") as ContentPresenter;
            this.outrect = GetTemplateChild("Part_OutsideRect") as Rectangle;

            if (outrect != null)
                outrect.MouseLeftButtonDown += new MouseButtonEventHandler(outrect_MouseLeftButtonDown);

            if (PART_RibbonDropDown != null)
            {
                PART_RibbonDropDown.Opened += new EventHandler(PART_RibbonDropDown_Opened);
                PART_RibbonDropDown.Closed += new EventHandler(PART_RibbonDropDown_Closed);
            }

            if (this.Part_RootButton != null)
            {
                this.Part_RootButton.MouseLeftButtonDown += new MouseButtonEventHandler(Part_RootButton_MouseLeftButtonDown);
                this.Part_RootButton.MouseEnter += new MouseEventHandler(Part_RootButton_MouseEnter);
                this.Part_RootButton.MouseLeave += new MouseEventHandler(Part_RootButton_MouseLeave);
            }

            this.OnMenuItemsPaneChanged(null, this.MenuItemsPane);
            this.OnSystemItemsPaneChanged(null, this.SystemItemsPane);
        }

        void PART_RibbonDropDown_Closed(object sender, EventArgs e)
        {
            VisualStateManager.GoToState(this, "Normal", true);
            if (OnApplicationMenuClosed != null)
                OnApplicationMenuClosed(this, e);
        }

        void PART_RibbonDropDown_Opened(object sender, EventArgs e)
        {
            if (OnApplicationMenuOpened != null)
                OnApplicationMenuOpened(this, e);
        }

        void outrect_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (PART_RibbonDropDown.IsOpen)
            {
                PART_RibbonDropDown.IsOpen = false;
            }
        }

        private void OnHostContentResized(object sender, EventArgs e)
        {
            if (PART_RibbonDropDown != null)
            {
                if (this.PART_RibbonDropDown.IsOpen)
                {
                    this.UpdateOutsideRect();
                }
            }
        }

        private void Part_RootButton_MouseLeave(object sender, MouseEventArgs e)
        {
            if (PART_RibbonDropDown != null && !this.PART_RibbonDropDown.IsOpen)
                VisualStateManager.GoToState(this, "Normal", true);
        }

        private void Part_RootButton_MouseEnter(object sender, MouseEventArgs e)
        {
            if (PART_RibbonDropDown != null && !this.PART_RibbonDropDown.IsOpen)
                VisualStateManager.GoToState(this, "MouseOver", true);
        }

        private void Part_RootButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (PART_RibbonDropDown != null)
            {
                this.PART_RibbonDropDown.IsOpen = (this.PART_RibbonDropDown.IsOpen == true) ? false : true;
                InitializeItemsPane();
                VisualStateManager.GoToState(this, "Pressed", true);
                UpdateOutsideRect();
            }
        }

        private void InitializeItemsPane()
        {
            if (this.ItemsPane == null || this.PART_RibbonDropDown == null) return;

            foreach (var item in this.ItemsPane.Items)
            {
                if (item is SplitMenuButton)
                    ((SplitMenuButton)item).menuGroupLoadCount = 0;
            }

            if (this.PART_RibbonDropDown.IsOpen)
            {
                if (this.ItemsPane.Items.Count > 0)
                {
                    if (this.ItemsPane.Items[0] is ButtonBase)
                        ((ButtonBase)this.ItemsPane.Items[0]).Focus();
                    else if (this.ItemsPane.Items[0] is HeaderedItemsControl)
                        ((HeaderedItemsControl)this.ItemsPane.Items[0]).Focus();
                }
            }
        }

        private void UpdateOutsideRect()
        {
            try
            {
                GeneralTransform gt = this.TransformToVisual(null);

                if (gt != null)
                {
                    Point pt = gt.Transform(new Point(0, 0));

                    MatrixTransform transform = new MatrixTransform();
                    transform.Matrix = new Matrix(1, 0, 0, 1, -pt.X, -pt.Y);

                    if (this.outrect != null)
                    {
                        this.outrect.RenderTransform = transform;

                        System.Windows.Interop.Content content = Application.Current.Host.Content;

                        this.outrect.Width = content.ActualWidth;
                        this.outrect.Height = content.ActualHeight;
                    }
                }
            }
            catch
            {
            }
        }

        private Grid Part_RootButton;
        private Rectangle outrect;
        /// <summary>
        /// 
        /// </summary>
        public event EventHandler OnApplicationMenuOpened;
        /// <summary>
        /// 
        /// </summary>
        public event EventHandler OnApplicationMenuClosed;

        void PART_RibbonDropDown_IsOpenChanged(object sender, EventArgs e)
        {
            if (PART_RibbonDropDown.IsOpen)
            {
                if (OnApplicationMenuOpened != null)
                    OnApplicationMenuOpened(this, e);
            }
            else
            {
                VisualStateManager.GoToState(this, "Normal", true);
                if (OnApplicationMenuClosed != null)
                    OnApplicationMenuClosed(this, e);
            }
        }

        internal override void OnItemsPaneChanged(RibbonItemsControl oldValue, RibbonItemsControl newValue)
        {
            if (newValue != null)
            {
                foreach (var item in newValue.Items)
                {
                    if (item is SimpleMenuButton)
                    {
                        ((SimpleMenuButton)item).parentApplicationMenu = this;
                    }
                    else if (item is SplitMenuButton)
                    {
                        ((SplitMenuButton)item).parentApplicationMenu = this;
                    }
                }
                newValue.ItemsChanged -= newValue_ItemsChanged;
                newValue.ItemsChanged += newValue_ItemsChanged;
            }
            base.OnItemsPaneChanged(oldValue, newValue);
        }

        void newValue_ItemsChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (var item in e.NewItems)
                {
                    if (item is SimpleMenuButton)
                    {
                        ((SimpleMenuButton)item).parentApplicationMenu = this;
                    }
                    else if (item is SplitMenuButton)
                    {
                        ((SplitMenuButton)item).parentApplicationMenu = this;
                    }
                }
            }
        }

        /// <summary>
        /// Called when [query drop down bounds].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="Syncfusion.Windows.Tools.Controls.BoundsEventArgs"/> instance containing the event data.</param>
        internal override void OnQueryDropDownBounds(object sender, BoundsEventArgs args)
        {
            UIElement ui = sender as UIElement;

            if (ui != null)
            {
                UIElement parent = VisualTreeHelper.GetParent(this.menuPanelPresenter) as UIElement;

                if (parent != null)
                {
                    Size menuPanelSize = parent.RenderSize;

                    GeneralTransform gt = parent.TransformToVisual(ui);
                    Point pt = gt.Transform(new Point(0, 0));

                    args.Rect = new Rect(ui.RenderSize.Width, pt.Y, menuPanelSize.Width, menuPanelSize.Height);
                }
            }
        }

        /// <summary>
        /// Processes the key down.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.KeyEventArgs"/> instance containing the event data.</param>
        public override void ProcessKeyDown(System.Windows.Input.KeyEventArgs e)
        {
            if (this.IsOpen)
            {
                if (!e.Handled)
                {
                    switch (e.Key)
                    {
                        case Key.Down:
                            e.Handled = this.ProcessKeyDown();
                            break;
                        case Key.Up:
                            e.Handled = this.ProcessKeyUp();
                            break;
                        case Key.Left:
                            e.Handled = this.ProcessKeyLeft();
                            break;
                        case Key.Right:
                            e.Handled = this.ProcessKeyRight();
                            break;
                    }
                }

                base.ProcessKeyDown(e);
            }
        }

        #endregion

        #region Implementation

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            ContentControl cctrl = this.Parent as ContentControl;
            if (cctrl != null)
            {
                cctrl.Content = null;
            }
        }

        /// <summary>
        /// Processes the key down.
        /// </summary>
        /// <returns></returns>
        private bool ProcessKeyDown()
        {
            RibbonItemBase selectedItem = this.SelectedItem;

            if (selectedItem != null)
            {
                RibbonItemBase nextItem = null;

                int mainIndex = this.GetItemIndex(this.ItemsPane, selectedItem);

                if (mainIndex >= 0)
                {
                    nextItem = this.GetNextItem(this.ItemsPane, mainIndex);

                    if (nextItem == null)
                    {
                        nextItem = this.GetFirstItem(this.MenuItemsPane);

                        if (nextItem == null)
                        {
                            nextItem = this.GetFirstItem(this.SystemItemsPane);
                        }
                    }
                }
                else
                {
                    int menuIndex = this.GetItemIndex(this.MenuItemsPane, selectedItem);

                    if (menuIndex >= 0)
                    {
                        nextItem = this.GetNextItem(this.MenuItemsPane, menuIndex);

                        if (nextItem == null)
                        {
                            nextItem = this.GetFirstItem(this.SystemItemsPane);

                            if (nextItem == null)
                            {
                                nextItem = this.GetFirstItem(this.ItemsPane);
                            }
                        }
                    }
                    else
                    {
                        int systemIndex = this.GetItemIndex(this.SystemItemsPane, selectedItem);

                        if (systemIndex >= 0)
                        {
                            nextItem = this.GetNextItem(this.SystemItemsPane, systemIndex);

                            if (nextItem == null)
                            {
                                nextItem = this.GetFirstItem(this.ItemsPane);

                                if (nextItem == null)
                                {
                                    nextItem = this.GetFirstItem(this.MenuItemsPane);
                                }
                            }
                        }
                    }
                }

                if (nextItem != null)
                {
                    nextItem.IsSelected = true;

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Processes the key up.
        /// </summary>
        /// <returns></returns>
        private bool ProcessKeyUp()
        {
            RibbonItemBase selectedItem = this.SelectedItem;

            if (selectedItem != null)
            {
                RibbonItemBase nextItem = null;

                int mainIndex = this.GetItemIndex(this.ItemsPane, selectedItem);

                if (mainIndex >= 0)
                {
                    nextItem = this.GetPrevItem(this.ItemsPane, mainIndex);

                    if (nextItem == null)
                    {
                        nextItem = this.GetLastItem(this.SystemItemsPane);

                        if (nextItem == null)
                        {
                            nextItem = this.GetLastItem(this.MenuItemsPane);
                        }
                    }
                }
                else
                {
                    int menuIndex = this.GetItemIndex(this.MenuItemsPane, selectedItem);

                    if (menuIndex >= 0)
                    {
                        nextItem = this.GetPrevItem(this.MenuItemsPane, menuIndex);

                        if (nextItem == null)
                        {
                            nextItem = this.GetLastItem(this.ItemsPane);

                            if (nextItem == null)
                            {
                                nextItem = this.GetLastItem(this.SystemItemsPane);
                            }
                        }
                    }
                    else
                    {
                        int systemIndex = this.GetItemIndex(this.SystemItemsPane, selectedItem);

                        if (systemIndex >= 0)
                        {
                            nextItem = this.GetPrevItem(this.SystemItemsPane, systemIndex);

                            if (nextItem == null)
                            {
                                nextItem = this.GetLastItem(this.MenuItemsPane);

                                if (nextItem == null)
                                {
                                    nextItem = this.GetLastItem(this.ItemsPane);
                                }
                            }
                        }
                    }
                }

                if (nextItem != null)
                {
                    nextItem.IsSelected = true;

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Processes the key left.
        /// </summary>
        /// <returns></returns>
        private bool ProcessKeyLeft()
        {
            RibbonItemBase selectedItem = this.SelectedItem;

            if (selectedItem != null)
            {
                RibbonItemBase nextItem = null;

                int mainIndex = this.GetItemIndex(this.ItemsPane, selectedItem);

                if (mainIndex >= 0)
                {
                    nextItem = this.GetNeighbourItem(this.MenuItemsPane, selectedItem);
                }
                else
                {
                    int menuIndex = this.GetItemIndex(this.MenuItemsPane, selectedItem);

                    if (menuIndex >= 0)
                    {
                        nextItem = this.GetNeighbourItem(this.ItemsPane, selectedItem);
                    }
                    else
                    {
                        int systemIndex = this.GetItemIndex(this.SystemItemsPane, selectedItem);

                        if (systemIndex >= 0)
                        {
                            nextItem = this.GetPrevItem(this.SystemItemsPane, systemIndex);

                            if (nextItem == null)
                            {
                                nextItem = this.GetLastItem(this.SystemItemsPane);
                            }
                        }
                    }
                }

                if (nextItem != null)
                {
                    nextItem.IsSelected = true;

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Processes the key right.
        /// </summary>
        /// <returns></returns>
        private bool ProcessKeyRight()
        {
            RibbonItemBase selectedItem = this.SelectedItem;

            if (selectedItem != null)
            {
                RibbonDropDownItem dropDownItem = selectedItem as RibbonDropDownItem;

                if (dropDownItem != null && dropDownItem.DropDown != null)
                {
                    dropDownItem.ShowPopup = true;
                }
                else
                {
                    RibbonItemBase nextItem = null;

                    int mainIndex = this.GetItemIndex(this.ItemsPane, selectedItem);

                    if (mainIndex >= 0)
                    {
                        nextItem = this.GetNeighbourItem(this.MenuItemsPane, selectedItem);
                    }
                    else
                    {
                        int menuIndex = this.GetItemIndex(this.MenuItemsPane, selectedItem);

                        if (menuIndex >= 0)
                        {
                            nextItem = this.GetNeighbourItem(this.ItemsPane, selectedItem);
                        }
                        else
                        {
                            int systemIndex = this.GetItemIndex(this.SystemItemsPane, selectedItem);

                            if (systemIndex >= 0)
                            {
                                nextItem = this.GetNextItem(this.SystemItemsPane, systemIndex);

                                if (nextItem == null)
                                {
                                    nextItem = this.GetFirstItem(this.SystemItemsPane);
                                }
                            }
                        }
                    }

                    if (nextItem != null)
                    {
                        nextItem.IsSelected = true;

                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Gets the prev item.
        /// </summary>
        /// <param name="itemsList">The items list.</param>
        /// <param name="from">From.</param>
        /// <returns></returns>
        private RibbonItemBase GetPrevItem(IList itemsList, int from)
        {
            if (from > itemsList.Count)
            {
                from = itemsList.Count;
            }

            if (from >= 0)
            {
                for (int i = from - 1; i >= 0; i--)
                {
                    RibbonItemBase item = itemsList[i] as RibbonItemBase;

                    if (item != null)
                    {
                        if (item.Visibility == Visibility.Visible)
                        {
                            return item;
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the next item.
        /// </summary>
        /// <param name="itemsList">The items list.</param>
        /// <param name="from">From.</param>
        /// <returns></returns>
        private RibbonItemBase GetNextItem(IList itemsList, int from)
        {
            if (from >= 0)
            {
                for (int i = from + 1; i < itemsList.Count; i++)
                {
                    RibbonItemBase item = itemsList[i] as RibbonItemBase;

                    if (item != null)
                    {
                        if (item.Visibility == Visibility.Visible)
                        {
                            return item;
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the last item.
        /// </summary>
        /// <param name="itemsList">The items list.</param>
        /// <returns></returns>
        private RibbonItemBase GetLastItem(IList itemsList)
        {
            for (int i = itemsList.Count - 1; i >= 0; i--)
            {
                RibbonItemBase item = itemsList[i] as RibbonItemBase;

                if (item != null)
                {
                    if (item.Visibility == Visibility.Visible)
                    {
                        return item;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the first item.
        /// </summary>
        /// <param name="itemsList">The items list.</param>
        /// <returns></returns>
        private RibbonItemBase GetFirstItem(IList itemsList)
        {
            for (int i = 0; i < itemsList.Count; i++)
            {
                RibbonItemBase item = itemsList[i] as RibbonItemBase;

                if (item != null)
                {
                    if (item.Visibility == Visibility.Visible)
                    {
                        return item;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the neighbour item.
        /// </summary>
        /// <param name="itemsList">The items list.</param>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        private RibbonItemBase GetNeighbourItem(IList itemsList, RibbonItemBase item)
        {
            Point zero = new Point(0, 0);

            double offset = double.MaxValue;

            RibbonItemBase neighbourItem = this.GetFirstItem(itemsList);

            for (RibbonItemBase nextItem = neighbourItem; nextItem != null; nextItem = this.GetNextItem(itemsList, itemsList.IndexOf(nextItem)))
            {
                GeneralTransform gt = nextItem.TransformToVisual(item);

                double itemOffset = Math.Abs(gt.Transform(zero).Y);

                if (offset > itemOffset)
                {
                    neighbourItem = nextItem;

                    offset = itemOffset;
                }
                else
                {
                    break;
                }
            }

            return neighbourItem;
        }

        /// <summary>
        /// Gets the prev item.
        /// </summary>
        /// <param name="itemsControl">The items control.</param>
        /// <param name="from">From.</param>
        /// <returns></returns>
        private RibbonItemBase GetPrevItem(ItemsControl itemsControl, int from)
        {
            if (itemsControl != null)
            {
                return this.GetPrevItem(itemsControl.Items, from);
            }

            return null;
        }

        /// <summary>
        /// Gets the next item.
        /// </summary>
        /// <param name="itemsControl">The items control.</param>
        /// <param name="from">From.</param>
        /// <returns></returns>
        private RibbonItemBase GetNextItem(ItemsControl itemsControl, int from)
        {
            if (itemsControl != null)
            {
                return this.GetNextItem(itemsControl.Items, from);
            }

            return null;
        }

        /// <summary>
        /// Gets the last item.
        /// </summary>
        /// <param name="itemsControl">The items control.</param>
        /// <returns></returns>
        private RibbonItemBase GetLastItem(ItemsControl itemsControl)
        {
            if (itemsControl != null)
            {
                return this.GetLastItem(itemsControl.Items);
            }

            return null;
        }

        /// <summary>
        /// Gets the first item.
        /// </summary>
        /// <param name="itemsControl">The items control.</param>
        /// <returns></returns>
        private RibbonItemBase GetFirstItem(ItemsControl itemsControl)
        {
            if (itemsControl != null)
            {
                return this.GetFirstItem(itemsControl.Items);
            }

            return null;
        }

        /// <summary>
        /// Gets the neighbour item.
        /// </summary>
        /// <param name="itemsControl">The items control.</param>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        private RibbonItemBase GetNeighbourItem(ItemsControl itemsControl, RibbonItemBase item)
        {
            if (itemsControl != null)
            {
                return this.GetNeighbourItem(itemsControl.Items, item);
            }

            return null;
        }

        /// <summary>
        /// Called when [system items pane changed].
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void OnSystemItemsPaneChanged(RibbonItemsControl oldValue, RibbonItemsControl newValue)
        {
            if (oldValue != null)
            {
                oldValue.ItemClicked -= new System.EventHandler(this.OnPanelItemClicked);
                oldValue.ItemSelected -= new System.EventHandler(this.OnPanelItemSelected);
            }

            if (newValue != null)
            {
                newValue.ItemClicked += new System.EventHandler(this.OnPanelItemClicked);
                newValue.ItemSelected += new System.EventHandler(this.OnPanelItemSelected);

                FrameworkElement root = this.GetTemplateChild("Part_Root") as FrameworkElement;

                if (root != null && root.Resources != null)
                {
                    newValue.Style = root.Resources["SystemItemsPaneStyle"] as Style;
                }
            }

            if (this.systemPanelPresenter != null)
            {
                this.systemPanelPresenter.Content = newValue;
            }
        }

        /// <summary>
        /// Called when [menu items pane changed].
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void OnMenuItemsPaneChanged(RibbonItemsControl oldValue, RibbonItemsControl newValue)
        {
            if (oldValue != null)
            {
                oldValue.ItemClicked -= new System.EventHandler(this.OnPanelItemClicked);
                oldValue.ItemSelected -= new System.EventHandler(this.OnPanelItemSelected);
            }

            if (newValue != null)
            {
                newValue.ItemClicked += new System.EventHandler(this.OnPanelItemClicked);
                newValue.ItemSelected += new System.EventHandler(this.OnPanelItemSelected);

                FrameworkElement root = this.GetTemplateChild("Part_Root") as FrameworkElement;

                if (root != null && root.Resources != null)
                {
                    newValue.Style = root.Resources["MenuItemsPaneStyle"] as Style;
                }
            }

            if (this.menuPanelPresenter != null)
            {
                this.menuPanelPresenter.Content = newValue;
            }
        }

        #endregion

        #region EventHandlers

        /// <summary>
        /// Called when [panel item clicked].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnPanelItemClicked(object sender, System.EventArgs e)
        {
            this.IsOpen = false;
        }

        /// <summary>
        /// Called when [panel item selected].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnPanelItemSelected(object sender, System.EventArgs e)
        {
            this.SelectedItem = sender as RibbonItemBase;
        }

        #endregion

        #region Fields

        internal Popup PART_ApplicationSplitPopup = null;
        internal ContentPresenter menuPanelPresenter;
        private ContentPresenter systemPanelPresenter;
        internal Popup PART_RibbonDropDown = null;
        internal ContentPresenter Part_MainPanelPresenter = null;
        #endregion
    }


}
