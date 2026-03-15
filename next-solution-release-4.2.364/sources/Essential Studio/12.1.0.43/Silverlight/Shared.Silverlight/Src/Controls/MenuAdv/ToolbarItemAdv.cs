#region Copyright Syncfusion Inc. 2001 - 2014
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
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;

namespace Syncfusion.Windows.Shared
{
    /// <summary>
    /// Represents the Tool Bar Item Class.
    /// </summary>
    public class ToolbarItemAdv : ContentControl 
    {
        #region Dependency Properties
        /// <summary>
        /// This property holds the icon image source for a toolbar item.
        /// </summary>
        public static readonly DependencyProperty IconProperty = DependencyProperty.Register("Icon", typeof(ImageSource), typeof(ToolbarItemAdv), null);

        /// <summary>
        /// Gets or sets the icon for the ToolbarItemAdv object.
        /// </summary>
        public ImageSource Icon
        {
            get { return GetValue(IconProperty) as ImageSource; }
            set { SetValue(IconProperty, value); }
        } 
        #endregion

        #region Internal Variables
        internal Border highlightingRect;
        internal Grid toolbarItemGrid, toolbarItemIconGrid;
        internal ContentPresenter toolbarItemContent;
        internal StackPanel menuAdvStackPanel;
        internal SolidColorBrush transparent = new SolidColorBrush(Colors.Transparent);
        internal Brush highlightingColor;
        internal Brush highlightingBorderColor = new SolidColorBrush(Colors.Gray);
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the parent MenuAdv object.
        /// </summary>
        internal MenuAdv ParentMenuAdv
        {
            get;
            set;
        } 
        #endregion

        #region Events
        /// <summary>
        /// RoutedEventHandler Click
        /// </summary>
        public event RoutedEventHandler Click; 
        #endregion

        #region Constructors
        /// <summary>
        /// Public parameterless constructor
        /// </summary>
        public ToolbarItemAdv()
        {
            this.DefaultStyleKey = typeof(ToolbarItemAdv);
        }
        
        #endregion

        #region Overrides
        /// <summary>
        /// Gets references to the Template elements
        /// Modifies the Template elements depending on the presence of an Icon.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            toolbarItemGrid = GetTemplateChild("ToolbarItemGrid") as Grid;
            toolbarItemIconGrid = GetTemplateChild("ToolbarItemIconGrid") as Grid;
            toolbarItemContent = GetTemplateChild("ToolbarItemContent") as ContentPresenter;
            highlightingRect = GetTemplateChild("HighlightToolbarItem") as Border;

            if (this.ParentMenuAdv != null)
            {
                highlightingColor = this.ParentMenuAdv.HighlightingColor;
                if (this.ParentMenuAdv.isInitialOrientation)
                {
                    menuAdvStackPanel = GetStackPanel();
                    if (menuAdvStackPanel != null)
                    {
                        Binding bind = new Binding();
                        bind.Source = this.ParentMenuAdv.Orientation;
                        bind.Mode = BindingMode.OneWay;
                        menuAdvStackPanel.SetBinding(StackPanel.OrientationProperty, bind);

                        Binding colorBind = new Binding();
                        colorBind.Source = this.ParentMenuAdv.Background;
                        colorBind.Mode = BindingMode.OneWay;
                        menuAdvStackPanel.SetBinding(StackPanel.BackgroundProperty, colorBind);
                    }

                    this.ParentMenuAdv.isInitialOrientation = false;
                }
            }
            if (Icon != null && toolbarItemIconGrid != null && toolbarItemGrid != null && toolbarItemContent!= null)
            {
                toolbarItemIconGrid.Visibility = Visibility.Visible;
                toolbarItemGrid.ColumnDefinitions[0].Width = new GridLength(26);
                toolbarItemContent.Margin = new Thickness(0, 5, 5, 5);
            }
        }

        /// <summary>
        /// Changes the visual for the item into which the mouse has entered.
        /// </summary>
        /// <param name="e">Mouse Event Args</param>
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            if (this.ParentMenuAdv != null)
            {
                foreach (FrameworkElement item in this.ParentMenuAdv.ContainersToItems.Keys)
                {
                    if (item is ToolbarSplitItemAdv)
                    {
                        ((ToolbarSplitItemAdv)item).subMenuItemsPopup.IsOpen = false;
                        VisualStateManager.GoToState(((ToolbarSplitItemAdv)item), "PopupClosed", false);
                        ((ToolbarSplitItemAdv)item).highlightingRect.Background = transparent;
                        ((ToolbarSplitItemAdv)item).highlightingRect.BorderBrush = transparent;
                        ((ToolbarSplitItemAdv)item).dropDownButton.BorderBrush = transparent;
                    }
                }
            }

            if (!(this.Content is MenuItemSeparator))
            {
                highlightingRect.Background = highlightingColor;
                highlightingRect.BorderBrush = highlightingBorderColor;
            }
        }

        /// <summary>
        /// Changes the visual for the item from which the mouse has left.
        /// </summary>
        /// <param name="e">Mouse Event Args</param>
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            if (!(this.Content is MenuItemSeparator))
            {
                highlightingRect.Background = transparent;
                highlightingRect.BorderBrush = transparent;
            }
        }

        /// <summary>
        /// Triggers the Click event if it's not null.
        /// </summary>
        /// <param name="e">Mouse Button Event Args</param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            if (Click != null)
            {
                Click(this, new RoutedEventArgs());
            }
        } 
        #endregion

        #region Methods
        /// <summary>
        /// Gets the ItemsPanel of ControlTemplate.
        /// </summary>
        /// <returns>StackPanel(ItemsPresenter) of MenuAdv</returns>
        internal StackPanel GetStackPanel()
        {
            DependencyObject element = this;
            while (!(element is StackPanel) && element != null)
            {
                element = VisualTreeHelper.GetParent(element);
            }

            if (element != null)
            {
                return element as StackPanel;
            }

            return null;
        }  
        #endregion
    }
}
