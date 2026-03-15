#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.Windows.Controls.Primitives;

namespace Syncfusion.Windows.Shared
{
    /// <summary>
    /// Represents the Tool Bar Splitter Item.
    /// </summary>
    [TemplateVisualState(Name = "PopupOpened", GroupName = "ViewStates")]
    [TemplateVisualState(Name = "PopupClosed", GroupName = "ViewStates")]
    public class ToolbarSplitItemAdv : MenuItemAdv
    {
        #region Internal Variables
        internal Border dropDownButton;
        internal Path downArrow, rightArrow;
        internal SolidColorBrush highlightBorderColor = new SolidColorBrush(Colors.Gray);
        #endregion

        #region Events
        /// <summary>
        /// RoutedEventHandler Click
        /// </summary>
        public new event RoutedEventHandler Click; 
        #endregion

        #region Constructors
        /// <summary>
        /// Public parameterless constructor.
        /// </summary>
        public ToolbarSplitItemAdv()
        {
            DefaultStyleKey = typeof(ToolbarSplitItemAdv);
            ContainersToItems = new Dictionary<DependencyObject, object>();
        } 
        #endregion

        #region Overrides
        /// <summary>
        /// Gets reference to the Template Elements.
        /// Takes care of the layout of items based on the Orientation DP of ToolbarAdv.
        /// </summary>
        public override void OnApplyTemplate()
        {
            dropDownButton = GetTemplateChild("PathDropDownBorder") as Border;
            downArrow = GetTemplateChild("PathDownArrow") as Path;
            rightArrow = GetTemplateChild("PathRightArrow") as Path;
            menuItem = GetTemplateChild("ToolbarItemHeaderPresenter") as ContentPresenter;
            subMenuItem = GetTemplateChild("SubToolbarMenuItem") as ContentPresenter;
            subMenuItemsPopup = GetTemplateChild("DropDownItemsPopup") as Popup;
            subMenuItemsPopupGrid = GetTemplateChild("SubToolbarMenuItemsPopupGrid") as Grid;
            subMenuItemsPopupRect = GetTemplateChild("DropDownItemsPopupRect") as Rectangle;
            menuItemAdvGrid = GetTemplateChild("ToolbarSplitItemAdvGrid") as Grid;
            highlightingRect = GetTemplateChild("HighlightToolbarSplitItem") as Border;
            iconGrid = GetTemplateChild("ToolbarSplitItemIconGrid") as Grid;
            subMenuItemGrid = GetTemplateChild("ToolbarSplitItemGrid") as Grid;
            menuItemIcon = GetTemplateChild("ToolbarSplitItemIcon") as Image;

            if (this.MenuReference != null)
            {
                highlightingColor = this.MenuReference.HighlightingColor;
                iconGridColor = this.MenuReference.LeftFrameBackground;
                subMenuItemsPopupBackgroundColor = this.MenuReference.SubMenuBackground;
            }

            if (subMenuItemsPopupRect != null && subMenuItem != null && dropDownButton != null)
            {
                subMenuItemsPopupRect.Fill = subMenuItemsPopupBackgroundColor;
                subMenuItemsPopupRect.Stroke = iconGridColor;
                subMenuItem.MouseEnter += new MouseEventHandler(subMenuItem_MouseEnter);
                dropDownButton.MouseEnter += new MouseEventHandler(dropDownButton_MouseEnter);
            }

            if (this.ParentMenuAdv != null)
            {
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

                if (this.ParentMenuAdv.Orientation == Orientation.Horizontal)
                {
                    downArrow.Visibility = Visibility.Visible;
                    rightArrow.Visibility = Visibility.Collapsed;
                }
                else
                {
                    downArrow.Visibility = Visibility.Collapsed;
                    rightArrow.Visibility = Visibility.Visible;
                }
            }

            if (this.Icon != null && subMenuItemGrid != null && iconGrid != null)
            {
                subMenuItemGrid.ColumnDefinitions[0].Width = new GridLength(23);
                iconGrid.Visibility = Visibility.Visible;
                menuItem.Margin = new Thickness(0, 5, 1, 5);
            }
        }

        /// <summary>
        /// ToolbarSplitItemAdv OnMouseEnter (Override)
        /// Highlights mouse-entered item.
        /// Shows subMenuItemsPopup if ExpandMode is ExpandOnMouseOver.
        /// </summary>
        /// <param name="e">Mouse Event Args </param>
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            this.MenuReference.isMouseOver = true;
            if (this.ParentMenuAdv != null)
            {
                foreach (FrameworkElement item in this.ParentMenuAdv.ContainersToItems.Keys)
                {
                    if ((item is ToolbarSplitItemAdv) && (item != this))
                    {
                        ((ToolbarSplitItemAdv)item).subMenuItemsPopup.IsOpen = false;
                        VisualStateManager.GoToState(((ToolbarSplitItemAdv)item), "PopupClosed", false);
                        ((ToolbarSplitItemAdv)item).highlightingRect.Background = transparent;
                        ((ToolbarSplitItemAdv)item).highlightingRect.BorderBrush = transparent;
                        ((ToolbarSplitItemAdv)item).dropDownButton.BorderBrush = transparent;
                    }
                }

                if (this.ParentMenuAdv.ExpandMode == ExpandModes.ExpandOnMouseOver)
                {
                    if (ContainsMouse(e, this.dropDownButton))
                    {
                        //this.HandlePopupOffset();
                        IsSubMenuOpen = true;
                        VisualStateManager.GoToState(this, "PopupOpened", false);
                    }
                }
            }

            this.Focus();
            highlightingRect.Background = highlightingColor;
            highlightingRect.BorderBrush = highlightBorderColor;
            dropDownButton.BorderBrush = highlightBorderColor;
        }

        /// <summary>
        /// ToolbarSplitItemAdv OnMouseLeave (Override)
        /// </summary>
        /// <param name="e">Mouse Event Args </param>
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            this.MenuReference.isMouseOver = false;
            if ((this.ParentMenuAdv != null) && (!this.ParentMenuAdv.firstClick) && (!IsSubMenuOpen))
            {
                highlightingRect.Background = transparent;
                highlightingRect.BorderBrush = transparent;
            }
            dropDownButton.BorderBrush = transparent;
        }

        /// <summary>
        /// ToolbarSplitItemAdv OnMouseLeftButtonDown (Override)
        /// Opens subMenuItemsPopup if pressed on dropDownButton. 
        /// Triggers Click event if pressed on other parts of the control.
        /// </summary>
        /// <param name="e">Mouse Button Event Args </param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (ContainsMouse(e, this.dropDownButton))
            {
                //this.HandlePopupOffset();
                IsSubMenuOpen = !IsSubMenuOpen;
                if (IsSubMenuOpen)
                {
                    VisualStateManager.GoToState(this, "PopupOpened", false);
                }
                else
                {
                    VisualStateManager.GoToState(this, "PopupClosed", false);
                }
            }
            else
            {
                IsSubMenuOpen = false;
                VisualStateManager.GoToState(this, "PopupClosed", false);
                if (Click != null)
                {
                    Click(this, new RoutedEventArgs());
                }
            }
        } 
        #endregion

        #region Methods
        /// <summary>
        /// subMenuItem_MouseEnter
        /// Sets the highlighting effect when the mouse moves from the menu header into the popup.
        /// </summary>
        /// <param name="sender">the ContentPresenter into which the mouse has entered</param>
        /// <param name="e">Mouse Event Args </param>
        void subMenuItem_MouseEnter(object sender, MouseEventArgs e)
        {
            highlightingRect.Background = highlightingColor;
        }

        

        /// <summary>
        /// Opens the subMenuItemsPopup if ExpandMode is ExpandOnMouseOver.
        /// </summary>
        /// <param name="sender">object into which the mouse has entered.</param>
        /// <param name="e">Mouse Event Args </param>
        void dropDownButton_MouseEnter(object sender, MouseEventArgs e)
        {
            if (this.ParentMenuAdv.ExpandMode == ExpandModes.ExpandOnMouseOver)
            {
                this.IsSubMenuOpen = true;
                VisualStateManager.GoToState(this, "PopupOpened", false);
            }
        }
        #endregion
    }
}
