#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents the PopupMenuItemClick Handler. 
    /// </summary>
    public delegate void PopupMenuItemClickHandler(object sender, PopupMenuItem item);

    /// <summary>
    /// Represents a PopupMenu control.
    /// </summary>
    public class PopupMenu : Control
    {
        #region Fields
        private FrameworkElement popupFrom;

        private event MouseEventHandler ChildMouseEnterEvent;

        private event MouseEventHandler ChildMouseLeaveEvent;

        private event PopupMenuItemClickHandler ItemClickEvent;

        private System.Threading.Timer timer;
        private Grid popGrid;
        private StackPanel pnlRoot;
        private Popup popMenu;
        private StackGrid parentStackGrid;
        private LinearGradientBrush gb;
        private bool isNavPaneMode = false;
        private bool hasContentBorder = false;
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the parent stack grid.
        /// </summary>
        /// <value>The parent stack grid.</value>
        internal StackGrid ParentStackGrid
        {
            get
            {
                return this.parentStackGrid;
            }

            set
            {
                this.parentStackGrid = value;
            }
        }

        /// <summary>
        /// Sets element that will trigger the popup menu.
        /// </summary>
        internal FrameworkElement PopupFrom
        {
            set
            {
                if (this.popupFrom != null)
                {
                    this.popupFrom.MouseLeave -= new MouseEventHandler(this.PopupFrom_MouseLeave);
                }

                this.popupFrom = value;

                if (this.popupFrom != null)
                {
                    this.popupFrom.MouseLeftButtonDown += new MouseButtonEventHandler(this.PopupFrom_MouseLeftButtonDown);
                    this.popupFrom.MouseLeave += new MouseEventHandler(this.PopupFrom_MouseLeave);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is nav pane mode.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is nav pane mode; otherwise, <c>false</c>.
        /// </value>
        internal bool IsNavPaneMode
        {
            get
            {
                return isNavPaneMode;
            }

            set
            {
                isNavPaneMode = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has content border.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has content border; otherwise, <c>false</c>.
        /// </value>
        internal bool HasContentBorder
        {
            get
            {
                return this.hasContentBorder;
            }

            set
            {
                this.hasContentBorder = value;
            }
        }

        /// <summary>
        /// Gets the pop grid.
        /// </summary>
        /// <value>The pop grid.</value>
        internal Grid PopGrid
        {
            get
            {
                return this.popGrid;
            }
        }

        /// <summary>
        /// Gets the pop menu.
        /// </summary>
        /// <value>The pop menu.</value>
        internal Popup PopMenu
        {
            get
            {
                return this.popMenu;
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when [child mouse enter].
        /// </summary>
        internal event MouseEventHandler ChildMouseEnter
        {
            add
            {
                this.ChildMouseEnterEvent += value;
            }

            remove
            {
                this.ChildMouseEnterEvent -= value;
            }
        }

        /// <summary>
        /// Occurs when [child mouse leave].
        /// </summary>
        internal event MouseEventHandler ChildMouseLeave
        {
            add
            {
                this.ChildMouseLeaveEvent += value;
            }

            remove
            {
                this.ChildMouseLeaveEvent -= value;
            }
        }

        /// <summary>
        /// Occurs when [item click].
        /// </summary>
        internal event PopupMenuItemClickHandler ItemClick
        {
            add
            {
                this.ItemClickEvent += value;
            }

            remove
            {
                this.ItemClickEvent -= value;
            }
        }

        #endregion

        /// <summary>
        /// Initialize a new instance of <see cref="PopupMenu"/>
        /// </summary>
        public PopupMenu()
        {
            this.DefaultStyleKey = typeof(PopupMenu);

            FrameworkElement root = Application.Current.RootVisual as FrameworkElement;
            if (root != null)
            {
                root.MouseLeftButtonDown += new MouseButtonEventHandler(this.RootMouseLeftButtonDown);
            }
        }

        /// <summary>
        /// Roots the mouse left button down.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void RootMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(this.timer != null)
            this.timer.Change(100, System.Threading.Timeout.Infinite);
        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes (such as a rebuilding layout pass) call <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.popGrid = GetTemplateChild("popGrid") as Grid;
            this.pnlRoot = GetTemplateChild("pnlRoot") as StackPanel;
            this.popMenu = GetTemplateChild("popMenu") as Popup;

            this.popGrid.MouseEnter += new MouseEventHandler(this.Menu_MouseEnter);
            this.popGrid.MouseLeave += new MouseEventHandler(this.Menu_MouseLeave);
            this.timer = new System.Threading.Timer(new System.Threading.TimerCallback(this.PopupTimer_Elapsed), null, System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);

            CreateGradientBrush();
        }

        /// <summary>
        /// Creates the gradient brush.
        /// </summary>
        private void CreateGradientBrush()
        {
            gb = new LinearGradientBrush();
            gb.StartPoint = new Point(0.5, 1);
            gb.EndPoint = new Point(0.5, 0);
            GradientStopCollection gst = new GradientStopCollection();
            GradientStop gs1 = new GradientStop();
            gs1.Color = Color.FromArgb(255, 255, 255, 251);
            gs1.Offset = 1;
            gb.GradientStops.Add(gs1);
            GradientStop gs2 = new GradientStop();
            gs2.Color = Color.FromArgb(255, 255, 245, 202);
            gs2.Offset = 0.65;
            gb.GradientStops.Add(gs2);
            GradientStop gs3 = new GradientStop();
            gs3.Color = Color.FromArgb(255, 255, 235, 169);
            gs3.Offset = 0.6;
            gb.GradientStops.Add(gs3);
            GradientStop gs4 = new GradientStop();
            gs4.Color = Color.FromArgb(255, 255, 213, 98);
            gs4.Offset = 0.25;
            gb.GradientStops.Add(gs4);
            GradientStop gs5 = new GradientStop();
            gs5.Color = Color.FromArgb(255, 255, 218, 116);
            gs5.Offset = 0.05;
            gb.GradientStops.Add(gs5);
            GradientStop gs6 = new GradientStop();
            gs6.Color = Color.FromArgb(255, 255, 228, 145);
            gs6.Offset = 0;
            gb.GradientStops.Add(gs6);
        }

        /// <summary>
        /// Set the menu items from a list of items.
        /// </summary>
        /// <param name="items">Menu items.</param>
        internal void SetMenuItems(IEnumerable<PopupMenuItem> items)
        {
            this.SetMenuItems(items, null);
        }

        /// <summary>
        /// Method for menu creation.
        /// </summary>
        /// <param name="items">List of the items</param>
        /// <param name="parentId" >Current GroupBarItem Control parent ID</param>
        protected void SetMenuItems(IEnumerable<PopupMenuItem> items, int? parentId)
        {
            this.pnlRoot.Children.Clear();

            var myItems = items.Where(p => p.ParentId == parentId);            

            foreach (var item in myItems)
            {
                this.CreateMenuItem(item, items);
            }
        }

        /// <summary>
        /// Method create items for the PopupMenu control
        /// </summary>
        /// <param name="item" >Current GroupBarItem Control Instance</param>
        /// <param name="items">List of the tems</param>
        protected void CreateMenuItem(PopupMenuItem item, IEnumerable<PopupMenuItem> items)
        {
            Border border = new Border();
            border.Background = new SolidColorBrush(Colors.White);

            border.MouseEnter += new MouseEventHandler(this.Border_MouseEnter);
            border.MouseLeave += new MouseEventHandler(this.Border_MouseLeave);

            StackPanel panel = new StackPanel();
            panel.Orientation = Orientation.Horizontal;
            border.Child = panel;

            Border textBorder = new Border();
            textBorder.Background = new SolidColorBrush(Colors.Transparent);
            textBorder.BorderThickness = new Thickness(0, 0, 1, 0);
            textBorder.BorderBrush = new SolidColorBrush(Colors.Transparent);

            //TextBlock headingTextBlock = new TextBlock();
            ContentControl headingTextBlock = new ContentControl();
            //headingTextBlock.Text = item.HeaderText;           
            headingTextBlock.Content = item.HeaderText.Content;
            headingTextBlock.ContentTemplate = item.HeaderText.ContentTemplate;
            headingTextBlock.HorizontalAlignment = HorizontalAlignment.Left;
            headingTextBlock.VerticalAlignment = VerticalAlignment.Center;
            headingTextBlock.Margin = new Thickness(5, 5, 25, 0);

            Border imageBorder = new Border();
            imageBorder.BorderThickness = new Thickness(0, 0, 1, 0);
            imageBorder.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 197, 197, 197));
            StackPanel imagePanel = new StackPanel();
            imagePanel.Background = new SolidColorBrush(Color.FromArgb(255, 218, 218, 218));
            imageBorder.Child = imagePanel;
            imagePanel.Children.Add(item.CheckedImage);
            imagePanel.Children.Add(item.HeaderImage);
            panel.Children.Add(imageBorder);
            panel.Children.Add(headingTextBlock);

            border.Tag = item;
            border.MouseLeftButtonUp += new MouseButtonEventHandler(this.Border_MouseLeftButtonUp);

            this.pnlRoot.Children.Add(border);
        }

        /// <summary>
        /// Handles the MouseLeftButtonUp event of the Border control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void Border_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var item = ((Border)sender).Tag as PopupMenuItem;

            if (((sender as Border).Tag as PopupMenuItem).HeaderImage.Visibility == Visibility.Visible)
            {
                ((sender as Border).Tag as PopupMenuItem).HeaderImage.Visibility = Visibility.Collapsed;
                ((sender as Border).Tag as PopupMenuItem).CheckedImage.Visibility = Visibility.Visible;
            }
            else
            {
                ((sender as Border).Tag as PopupMenuItem).HeaderImage.Visibility = Visibility.Visible;
                ((sender as Border).Tag as PopupMenuItem).CheckedImage.Visibility = Visibility.Collapsed;
            }

            if (item != null)
            {
                this.ParentStackGrid.HideItem(item.Id);
                this.ParentStackGrid.LocalParent.SelectedItem = (GroupBarItem)this.ParentStackGrid.ItemContainerGenerator.ContainerFromIndex(item.Id);
                (this.ParentStackGrid.LocalParent.SelectedItem as GroupBarItem).ChangeItemExpandMode(ParentStackGrid.LocalParent);
                (this.ParentStackGrid.LocalParent.SelectedItem as GroupBarItem).UpdateVisualState();
            }
        }

        /// <summary>
        /// Handles the MouseLeave event of the Border control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        private void Border_MouseLeave(object sender, MouseEventArgs e)
        {
            ((Border)sender).Background = new SolidColorBrush(Colors.White);
            ((Border)sender).BorderBrush = new SolidColorBrush(Colors.White);
            (((((Border)sender).Child as StackPanel).Children[0] as Border).Child as StackPanel).Background = new SolidColorBrush(Color.FromArgb(255, 218, 218, 218));
            ((((Border)sender).Child as StackPanel).Children[0] as Border).BorderBrush = new SolidColorBrush(Color.FromArgb(255, 197, 197, 197));
        }

        /// <summary>
        /// Handles the MouseEnter event of the Border control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        private void Border_MouseEnter(object sender, MouseEventArgs e)
        {
            ((Border)sender).Background = gb;
            (((((Border)sender).Child as StackPanel).Children[0] as Border).Child as StackPanel).Background = new SolidColorBrush(Colors.Transparent);
            ((((Border)sender).Child as StackPanel).Children[0] as Border).BorderBrush = new SolidColorBrush(Colors.Transparent);
            ((Border)sender).BorderBrush = new SolidColorBrush(Color.FromArgb(255, 215, 208, 179));
        }

        /// <summary>
        /// Handles the MouseLeave event of the Menu control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        private void Menu_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!this.IsNavPaneMode)
            {
                this.timer.Change(100, System.Threading.Timeout.Infinite);
            }

            if (this.ChildMouseLeaveEvent != null)
            {
                this.ChildMouseLeaveEvent(sender, e);
            }
        }

        /// <summary>
        /// Handles the MouseEnter event of the Menu control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        private void Menu_MouseEnter(object sender, MouseEventArgs e)
        {
            this.popMenu.IsOpen = true;

            this.timer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);

            if (this.ChildMouseEnterEvent != null)
            {
                this.ChildMouseEnterEvent(sender, e);
            }
        }

        /// <summary>
        /// Popups the timer_ elapsed.
        /// </summary>
        /// <param name="state">The state.</param>
        private void PopupTimer_Elapsed(object state)
        {
            if (this.IsNavPaneMode)
            {
                if (this.HasContentBorder)
                {
                    this.popMenu.Dispatcher.BeginInvoke(() => this.ParentStackGrid.CollapseNavPane());
                }

                this.popMenu.Dispatcher.BeginInvoke(() => this.popGrid.Children.Clear());
            }

            this.popMenu.Dispatcher.BeginInvoke(() => this.popMenu.IsOpen = false);
        }

        /// <summary>
        /// Handles the MouseLeave event of the PopupFrom control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        private void PopupFrom_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!this.IsNavPaneMode)
            {
                this.timer.Change(100, System.Threading.Timeout.Infinite);
            }
            if (ItemClickEvent != null)
            {
            }
        }

        /// <summary>
        /// Handles the MouseLeftButtonDown event of the PopupFrom control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        private void PopupFrom_MouseLeftButtonDown(object sender, MouseEventArgs e)
        {
            this.popMenu.IsOpen = true;
            this.timer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
        }
    }
}
