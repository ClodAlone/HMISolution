#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;

namespace Syncfusion.Windows.Tools.Controls
{
    public class ApplicationSplitButton : HeaderedItemsControl
    {
        #region Constructor

        public ApplicationSplitButton()
        {
            this.DefaultStyleKey = typeof(ApplicationSplitButton);
            this.KeyDown += new KeyEventHandler(ApplicationSplitButton_KeyDown);
            this.Loaded += new RoutedEventHandler(ApplicationSplitButton_Loaded);
        }

        #endregion

        #region Private Variables

        private Border PART_ToggleButton;
        private Border PART_ContentBorder;
        private Border PART_BottomScroll;
        private Border PART_TopScroll;
        private Border PART_RibbonDropDownBdr;
        private ScrollViewer PART_ScrollViewer;
        internal int menuGroupLoadCount;

        #endregion

        #region Properties

        public double ScrollerHeight
        {
            get { return (double)GetValue(ScrollerHeightProperty); }
            set { SetValue(ScrollerHeightProperty, value); }
        }

        public bool IsMouseOver
        {
            get { return (bool)GetValue(IsMouseOverProperty); }
            set { SetValue(IsMouseOverProperty, value); }
        }

        ApplicationMenu _parentApplicationMenu;
        internal ApplicationMenu parentApplicationMenu
        {
            get { return _parentApplicationMenu; }
            set
            {
                if (_parentApplicationMenu != value)
                {
                    _parentApplicationMenu = value;
                }
                this.SetApplcationMenuToRibbonGroup();
            }
        }

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }


        #endregion

        // Using a DependencyProperty as the backing store for Command.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(ApplicationSplitButton), new PropertyMetadata(null));

        public Object CommandParameter
        {
            get { return (Object)GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CommandParameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(Object), typeof(ApplicationSplitButton), new PropertyMetadata(null));

        private void SetApplcationMenuToRibbonGroup()
        {
            foreach (var item in this.Items.OfType<RibbonMenuGroup>())
            {
                if (item.parentApplicationMenu == null && this.parentApplicationMenu != null)
                    item.parentApplicationMenu = this.parentApplicationMenu;
            }
        }


        #region Implementation

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.PART_ToggleButton = this.GetTemplateChild("PART_ToggleButton") as Border;
            this.PART_ContentBorder = this.GetTemplateChild("PART_ContentBorder") as Border;
            this.PART_TopScroll = this.GetTemplateChild("PART_TopScroll") as Border;
            this.PART_BottomScroll = this.GetTemplateChild("PART_BottomScroll") as Border;
            this.PART_RibbonDropDownBdr = this.GetTemplateChild("PART_RibbonDropDownBdr") as Border;
            this.PART_ScrollViewer = this.GetTemplateChild("PART_ScrollViewer") as ScrollViewer;
            this.ScrollerHeight = 0d;
            this.SubscribeMouseEvents();
            this.Loaded += new RoutedEventHandler(ApplicationSplitButton_Loaded);
        }

        private void ApplicationSplitButton_Loaded(object sender, RoutedEventArgs e)
        {
            this.ScrollerHeight = 0d;           
        }

        private void SubscribeMouseEvents()
        {
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

            if (this.PART_ContentBorder != null)
            {
                this.PART_ContentBorder.MouseEnter += new MouseEventHandler(PART_ContentBorder_MouseEnter);
                this.PART_ContentBorder.MouseLeave += new MouseEventHandler(PART_ContentBorder_MouseLeave);
                this.PART_ContentBorder.MouseLeftButtonDown += new MouseButtonEventHandler(PART_ContentBorder_MouseLeftButtonDown);
            }

            if (this.PART_ToggleButton != null)
            {
                this.PART_ToggleButton.MouseEnter += new MouseEventHandler(PART_ToggleButton_MouseEnter);
                this.PART_ToggleButton.MouseLeave += new MouseEventHandler(PART_ToggleButton_MouseLeave);
            }
            if (this.parentApplicationMenu != null && this.parentApplicationMenu.PART_ApplicationSplitPopup != null && (this.parentApplicationMenu.PART_ApplicationSplitPopup.Child as UIElement) != null)
            {
                (this.parentApplicationMenu.PART_ApplicationSplitPopup.Child as UIElement).MouseEnter += new MouseEventHandler(ApplicationSplitButton_MouseEnter);
                (this.parentApplicationMenu.PART_ApplicationSplitPopup.Child as UIElement).MouseLeave += new MouseEventHandler(ApplicationSplitButton_MouseLeave);
            }
        }

        public event EventHandler Click;

        void PART_ContentBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (parentApplicationMenu != null)
            {
                if (parentApplicationMenu.PART_RibbonDropDown != null)
                {
                    parentApplicationMenu.PART_RibbonDropDown.IsOpen = false;
                    VisualStateManager.GoToState(this, "Normal", false);
                }
            }
            if (Click != null)
            {
                Click(this, e);
            }
        }

        private void PART_ContentBorder_MouseLeave(object sender, MouseEventArgs e)
        {
            VisualStateManager.GoToState(this, "Normal", false);
            this.HidePopup();
        }

        private void PART_ContentBorder_MouseEnter(object sender, MouseEventArgs e)
        {
            VisualStateManager.GoToState(this, "MouseOver", false);
            this.ShowPopup();
        }

        private void ScrollUp_Tick(object sender, EventArgs e)
        {
            this.PART_ScrollViewer.ScrollToVerticalOffset(this.PART_ScrollViewer.VerticalOffset + scrollableValue);
        }

        private DispatcherTimer ScrollerTick;

        private void PART_TopScroll_MouseLeave(object sender, MouseEventArgs e)
        {
            ScrollerTick.Stop();
            VisualStateManager.GoToState(this, "Normal", false);
        }

        private void PART_BottomScroll_MouseLeave(object sender, MouseEventArgs e)
        {
            ScrollerTick.Stop();
            VisualStateManager.GoToState(this, "Normal", false);
        }

        private void PART_BottomScroll_MouseEnter(object sender, MouseEventArgs e)
        {
            scrollableValue = -2;
            ScrollerTick.Start();
            VisualStateManager.GoToState(this, "BottomScrollMouseOver", false);
        }

        private double scrollableValue = 2;

        private void PART_TopScroll_MouseEnter(object sender, MouseEventArgs e)
        {
            scrollableValue = 2;
            ScrollerTick.Start();
            VisualStateManager.GoToState(this, "TopScrollMouseOver", false);
        }

        private void ApplicationSplitButton_MouseLeave(object sender, MouseEventArgs e)
        {
            this.IsMouseOver = false;
            if (this.HidePopup())
                VisualStateManager.GoToState(this, "Normal", false);
        }

        private void ApplicationSplitButton_MouseEnter(object sender, MouseEventArgs e)
        {
            HideMouseOverItems(this.Parent);
            this.IsMouseOver = true;
            this.Focus();
            if (this.ShowPopup())
                VisualStateManager.GoToState(this, "ToggleButtonMouseOver", false);
        }

        private bool ShowPopup()
        {
            if (parentApplicationMenu != null && parentApplicationMenu.PART_ApplicationSplitPopup != null && parentApplicationMenu.Part_MainPanelPresenter != null && parentApplicationMenu.menuPanelPresenter != null && this.parentApplicationMenu.MenuItemsPane != null)
            {
                //if (this.parentApplicationMenu.PART_ApplicationSplitPopup.IsOpen == true) return false;
                if (this.PART_RibbonDropDownBdr.Child != null)
                {
                    var child = this.PART_RibbonDropDownBdr.Child;
                    this.PART_RibbonDropDownBdr.Child = null;
                    this.parentApplicationMenu.PART_ApplicationSplitPopup.Child = child;
                    if ((this.parentApplicationMenu.PART_ApplicationSplitPopup.Child as UIElement) != null)
                    {
                        (this.parentApplicationMenu.PART_ApplicationSplitPopup.Child as UIElement).MouseEnter += new MouseEventHandler(ApplicationSplitButton_MouseEnter);
                        (this.parentApplicationMenu.PART_ApplicationSplitPopup.Child as UIElement).MouseLeave += new MouseEventHandler(ApplicationSplitButton_MouseLeave);
                        if (this.parentApplicationMenu.MenuItemsPane.Items.Count <= 0)
                        {
                            (this.parentApplicationMenu.PART_ApplicationSplitPopup.Child as FrameworkElement).Width = this.parentApplicationMenu.Part_MainPanelPresenter.ActualWidth;
                            (this.parentApplicationMenu.PART_ApplicationSplitPopup.Child as FrameworkElement).Height = this.parentApplicationMenu.Part_MainPanelPresenter.ActualHeight;
                        }
                        else
                        {
                            (this.parentApplicationMenu.PART_ApplicationSplitPopup.Child as FrameworkElement).Width = this.parentApplicationMenu.menuPanelPresenter.ActualWidth;
                            (this.parentApplicationMenu.PART_ApplicationSplitPopup.Child as FrameworkElement).Height = this.parentApplicationMenu.menuPanelPresenter.ActualHeight;
                        }
                    }
                }
                if (this.PART_ScrollViewer != null && this.PART_ScrollViewer.ScrollableHeight > 0 && this.PART_ScrollViewer.ScrollableHeight != this.PART_ScrollViewer.ExtentHeight)
                {
                    this.ScrollerHeight = 10d;
                }
                else
                {
                    this.ScrollerHeight = 0d;
                }
                this.parentApplicationMenu.PART_ApplicationSplitPopup.IsOpen = true;
                HideMouseOverMenuButtonItems(this);
                return true;
            }
            else return false;
        }

        public bool HidePopup()
        {
            if (parentApplicationMenu != null && parentApplicationMenu.PART_ApplicationSplitPopup != null && parentApplicationMenu.Part_MainPanelPresenter != null)
            {
                if (this.parentApplicationMenu.PART_ApplicationSplitPopup.IsOpen == false) return false;
                if (this.parentApplicationMenu.PART_ApplicationSplitPopup.Child != null)
                {
                    var child = this.parentApplicationMenu.PART_ApplicationSplitPopup.Child;
                    this.parentApplicationMenu.PART_ApplicationSplitPopup.Child = null;
                    this.PART_RibbonDropDownBdr.Child = child;
                }
                this.parentApplicationMenu.PART_ApplicationSplitPopup.IsOpen = false;
                return true;
            }
            else return false;
        }

        private void PART_ToggleButton_MouseLeave(object sender, MouseEventArgs e)
        {

        }

        protected override void OnItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    var itm = e.NewItems[0] as RibbonMenuGroup;
                    if (itm != null && this.parentApplicationMenu != null)
                    {
                        itm.parentApplicationMenu = this.parentApplicationMenu;
                    }
                    var appitm = e.NewItems[0] as ApplicationMenuButton;
                    if (appitm != null && this.parentApplicationMenu != null)
                    {
                        appitm.parentApplicationMenu = this.parentApplicationMenu;
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    break;
                default:
                    break;
            }
            base.OnItemsChanged(e);
        }

        private void PART_ToggleButton_MouseEnter(object sender, MouseEventArgs e)
        {
            VisualStateManager.GoToState(this, "ToggleButtonMouseOver", false);
            this.ShowPopup();
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            VisualStateManager.GoToState(this, "Normal", false);
            this.HidePopup();
            base.OnMouseLeave(e);
        }

        public Size IconSize
        {
            get { return (Size)GetValue(IconSizeProperty); }
            set { SetValue(IconSizeProperty, value); }
        }

        public static void OnMouseOverChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var senderItem = (ApplicationSplitButton)sender;
            senderItem.GotoVisualStates();
        }

        private void GotoVisualStates()
        {
            if (IsMouseOver)
                VisualStateManager.GoToState(this, "MouseOver", true);
            else
                VisualStateManager.GoToState(this, "Normal", true);
        }

        private void FindAndFocusItem(object parentItem, Key enteredKey)
        {
            ItemsControl parent = null;
            if (parentItem is ItemsControl)
                if (parentItem.GetType().Name == "RibbonItemsControl")
                    parent = parentItem as RibbonItemsControl;
                else if (parentItem.GetType().Name == "RibbonMenuGroup")
                    parent = parentItem as RibbonMenuGroup;

            int count = parent.Items.Count;
            int startingIndex = 0;

            HideMouseOverItems(parent);

            if (this != null)
                startingIndex = parent.ItemContainerGenerator.IndexFromContainer(this);

            int index = startingIndex;
            do
            {
                if (enteredKey == Key.Up)
                { index = (index + count - 1) % count; }
                else if (enteredKey == Key.Down)
                {
                    if (this.Parent is RibbonItemsControl && ((RibbonItemsControl)this.Parent).downCount == 0)
                    { index = 0; ((RibbonItemsControl)this.Parent).downCount++; }
                    else
                        index = (index + count + 1) % count;
                }
                else if (enteredKey == Key.Home)
                    index = 0;
                else
                    index = count - 1;

                var containerObj = parent.ItemContainerGenerator.ContainerFromIndex(index) as object;
                if (containerObj.GetType().Name == "ApplicationMenuButton")
                {
                    ApplicationMenuButton container = containerObj as ApplicationMenuButton;
                    if (null != container)
                    {
                        if (container.IsEnabled)
                        {
                            container.IsMouseOver = true;
                            container.Focus();
                            break;
                        }
                    }
                }
                else if (containerObj.GetType().Name == "ApplicationSplitButton")
                {
                    ApplicationSplitButton container = containerObj as ApplicationSplitButton;
                    if (null != container)
                    {
                        if (container.IsEnabled)
                        {
                            container.IsMouseOver = true;
                            container.Focus();
                            break;
                        }
                    }
                }
            }
            while (index != startingIndex);
        }

        private void HideMouseOverItems(object parentObj)
        {
            if (parentObj.GetType().Name == "RibbonItemsControl")
            {
                var parent = parentObj as RibbonItemsControl;
                foreach (var item in parent.Items)
                {
                    if (item.GetType().Name == "ApplicationSplitButton")
                    {
                        var menuItem = item as ApplicationSplitButton;
                        if (menuItem != null)
                        {
                            if (menuItem.IsMouseOver)
                                menuItem.IsMouseOver = false;
                        }
                    }

                    else if (item.GetType().Name == "ApplicationMenuButton")
                    {
                        var menuItem = item as ApplicationMenuButton;
                        if (menuItem != null)
                            if (menuItem.IsMouseOver)
                                menuItem.IsMouseOver = false;
                    }
                }
            }
        }

        private void HideMouseOverMenuButtonItems(ApplicationSplitButton parentObj)
        {
            if (parentObj.Items.Count > 0)
            {
                var menuGroup = parentObj.Items[0] as RibbonMenuGroup;

                foreach (var item in menuGroup.Items)
                {
                    if (item.GetType().Name == "ApplicationMenuButton")
                    {
                        var menuItem = item as ApplicationMenuButton;
                        if (menuItem != null)
                            if (menuItem.IsMouseOver)
                                menuItem.IsMouseOver = false;
                    }
                }
            }
        }

        void ApplicationSplitButton_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                this.ShowPopup();
            else if (e.Key == Key.Up || e.Key == Key.Down || e.Key == Key.Home || e.Key == Key.End)
            {
                var parentItem = ((ApplicationSplitButton)this).Parent;
                FindAndFocusItem(parentItem, e.Key);
            }
            else if (e.Key == Key.Right)
            {
                this.IsMouseOver = false;
                if (this.ShowPopup())
                    VisualStateManager.GoToState(this, "ToggleButtonMouseOver", false);
            }
            else if (e.Key == Key.Escape)
                this.parentApplicationMenu.PART_RibbonDropDown.IsOpen = false;
        }

        #endregion

        #region  Dependency Properties

        // Using a DependencyProperty as the backing store for IconSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconSizeProperty =
            DependencyProperty.Register("IconSize", typeof(Size), typeof(ApplicationSplitButton), new PropertyMetadata(new Size(20d, 20d)));

        public ImageSource Icon
        {
            get { return (ImageSource)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Icon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(ImageSource), typeof(ApplicationSplitButton), new PropertyMetadata(null));

        // Using a DependencyProperty as the backing store for IsMouseOver.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsMouseOverProperty =
            DependencyProperty.Register("IsMouseOver", typeof(bool), typeof(ApplicationSplitButton), new PropertyMetadata(false, OnMouseOverChanged));


        // Using a DependencyProperty as the backing store for ScrollerHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ScrollerHeightProperty =
            DependencyProperty.Register("ScrollerHeight", typeof(double), typeof(ApplicationSplitButton), new PropertyMetadata(0d));

        #endregion

    }
}