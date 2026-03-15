#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Collections;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents tab popup menu.
    /// </summary>
    public class TabPopupMenu : Control
    {
        #region Fields
        /// <summary>
        /// Popup placement target.
        /// </summary>
        private FrameworkElement placementTarget;

        /// <summary>
        /// Popup used for drawing the popup.
        /// </summary>
        private Border popupBorder;

        /// <summary>
        /// Panel used for layouting popup items.
        /// </summary>
        private StackPanel popupRoot;

        /// <summary>
        /// The popup.
        /// </summary>
        private Popup popup;

        /// <summary>
        /// Indicates whether mouse is over the popup.
        /// </summary>
        private bool bIsMouseOver = false;

        /// <summary>
        /// Selected menuItem.
        /// </summary>
        private TabPopupMenuItem highLightedMenuItem;

        /// <summary>
        /// Indicates the tabControl parent.
        /// </summary>
        private TabControlAdv mTabControlParent;
        #endregion

        #region Initialization
        /// <summary>
        /// Initializes new instance of the TabPopupMenu class.
        /// </summary>
        public TabPopupMenu()
        {
            this.DefaultStyleKey = typeof(TabPopupMenu);            

            this.MenuItems = new TabPopupMenuItemCollection();
            this.MenuItems.CollectionChanged += new NotifyCollectionChangedEventHandler(MenuItemsCollectionChanged);

            FrameworkElement root = Application.Current.RootVisual as FrameworkElement;
            if (root != null)
            {
                root.MouseLeftButtonDown += new MouseButtonEventHandler(this.RootMouseLeftButtonDown);
            }

            this.MouseEnter += new MouseEventHandler(this.PopupMouseEnter);
            this.MouseLeave += new MouseEventHandler(this.PopupMouseLeave);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the highLighted menuItem.
        /// </summary>
        public TabPopupMenuItem HighLightedMenuItem
        {
            get
            {
                return this.highLightedMenuItem;
            }

            set
            {
                this.highLightedMenuItem = value;
            }
        }

        /// <summary>
        /// Gets or sets the placement target of the popup.
        /// </summary>
        public FrameworkElement PlacementTarget
        {
            get
            {
                return this.placementTarget;
            }

            set
            {
                this.placementTarget = value;
            }
        }

        /// <summary>
        /// Gets or sets menu items collection.
        /// </summary>
        public TabPopupMenuItemCollection MenuItems
        {
            get
            {
                return (TabPopupMenuItemCollection) GetValue(MenuItemsProperty);
            }

            set
            {
                SetValue(MenuItemsProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the vertical offset of the popup.
        /// </summary>
        public double VerticalOffset
        {
            get
            {
                return (double) GetValue(VerticalOffsetProperty);
            }

            set
            {
                SetValue(VerticalOffsetProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the horizontal offset of the popup.
        /// </summary>
        public double HorizontalOffset
        {
            get
            {
                return (double) GetValue(HorizontalOffsetProperty);
            }

            set
            {
                SetValue(HorizontalOffsetProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the popup is open.
        /// </summary>
        internal bool IsOpen
        {
            get
            {
                bool b = false;
                if (this.popup != null)
                {
                    b = this.popup.IsOpen;
                }

                return b;
            }

            set
            {
                if (this.popup != null)
                {
                    this.popup.IsOpen = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the tabControl parent.
        /// </summary>
        internal TabControlAdv TabControlParent
        {
            get
            {
                return mTabControlParent;
            }

            set
            {
                mTabControlParent = value;
            }
		}

		#region MenuItemStyle

        /// <summary>
        /// Gets or sets the menu item style.
        /// </summary>
        /// <value>The menu item style.</value>
		internal Style MenuItemStyle
		{
			get { return (Style)GetValue(MenuItemStyleProperty); }
			set { SetValue(MenuItemStyleProperty, value); }
		}

        /// <summary>
        /// Identifies the MenuItemStyle Dependency Property.
        /// </summary>
		internal static readonly DependencyProperty MenuItemStyleProperty = DependencyProperty.Register(
			"MenuItemStyle", 
			typeof(Style), 
			typeof(TabPopupMenu), 
			new PropertyMetadata(MenuItemStyleChangedCallback));

        /// <summary>
        /// Menus the item style changed callback.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
		private static void MenuItemStyleChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((TabPopupMenu)d).UpdateItems();
		}

		#endregion
		
		#endregion

		#region Events
		/// <summary>
        /// Occurs when a particular instance of a TabPopupMenu opens.
        /// </summary>
        public event RoutedEventHandler Opened;

        /// <summary>
        /// Occurs when a particular instance of a TabPopupMenu closes.
        /// </summary>
        public event RoutedEventHandler Closed;
        #endregion

        #region Dependency properties
        /// <summary>
        /// Identifies the <see cref="MenuItems"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MenuItemsProperty =
            DependencyProperty.Register("MenuItems", typeof(TabPopupMenuItemCollection), typeof(TabPopupMenu), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="VerticalOffset"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty VerticalOffsetProperty =
            DependencyProperty.Register("VerticalOffset", typeof(double), typeof(TabPopupMenu), new PropertyMetadata(0d));

        /// <summary>
        /// Identifies the <see cref="HorizontalOffset"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HorizontalOffsetProperty =
            DependencyProperty.Register("HorizontalOffset", typeof(double), typeof(TabPopupMenu), new PropertyMetadata(0d));
        #endregion

        #region Overrides
        /// <summary>
        /// Invoked whenever application code or internal processes (such as a rebuilding layout pass) 
        /// call System.Windows.Controls.Control.ApplyTemplate() method.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.popupBorder = this.GetTemplateChild("PopupBorder") as Border;

			this.popupRoot = this.GetTemplateChild("PopupRoot") as StackPanel;
            if (this.popupRoot != null)
            {
                this.MenuItems.Parent = this.popupRoot;
            }

            this.popup = this.GetTemplateChild("Popup") as Popup;
            if (this.popup != null)
            {
                this.popup.Opened += new EventHandler(this.PopupOpened);
                this.popup.Closed += new EventHandler(this.PopupClosed);
            }

			this.UpdateItems();
        }

        /// <summary>
        /// Provides the behavior for the "measure" pass of Silverlight layout. Classes
        /// can override this method to define their own measure pass behavior.
        /// </summary>
        /// <param name="availableSize">The available size that this object can give to child objects. Infinity can
        /// be specified as a value to indicate that the object will size to whatever
        /// content is available.</param>
        /// <returns>The size that this object determines it needs during layout, based on its
        /// calculations of child object allotted sizes.</returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            base.MeasureOverride(availableSize);
            Size desiredSize = new Size(0, 0);

            if (this.popup.IsOpen)
            {
                foreach (TabPopupMenuItem item in this.MenuItems)
                {
                    item.Measure(availableSize);
                    desiredSize.Width = Math.Max(desiredSize.Width, item.DesiredSize.Width);
                    desiredSize.Height += item.DesiredSize.Height;
                }
            }

            return desiredSize;
        }

        /// <summary>
        /// Override method for OnKeyDown event
        /// </summary>
        /// <param name="e">e represents KeyEventArgs object</param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (!e.Handled)
            {
                TabPopupMenuItem item = null;
                int direction = 0;
                int startIndex = -1;
                bool isHandled = false;
                switch (e.Key)
                {
                    case Key.End:
                        direction = -1;
                        startIndex = this.MenuItems.Count;
                        isHandled = true;
                        break;
                    case Key.Home:
                        direction = 1;
                        startIndex = -1;
                        isHandled = true;
                        break;
                    case Key.Up:
                        direction = -1;
                        startIndex = this.MenuItems.IndexOf(this.HighLightedMenuItem);
                        isHandled = true;
                        break;
                    case Key.Down:
                        direction = 1;
                        startIndex = this.MenuItems.IndexOf(this.HighLightedMenuItem);
                        isHandled = true;
                        break;
                    case Key.Enter:
                        this.HighLightedMenuItem.IsSelected = true;
                        isHandled = false;
                        break;
                }

                if (isHandled)
                {
                    item = this.FindNextTabItem(startIndex, direction);
                    if (item != null)
                    {
                        e.Handled = true;
                        foreach (TabPopupMenuItem menuItem in this.MenuItems)
                        {
                            if (item != menuItem)
                            {
                                menuItem.IsHighlighted = false;
                            }
                        }

                        item.IsHighlighted = true;
                    }
                }
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// internal method to get menu size
        /// </summary>
        /// <returns>returns size of the menu item</returns>
        internal Size GetMenuSize()
        {
            Size desiredSize = new Size(0, 0);
            foreach (TabPopupMenuItem item in this.MenuItems)
            {
                item.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                desiredSize.Width = Math.Max(desiredSize.Width, item.DesiredSize.Width);
                desiredSize.Height += item.DesiredSize.Height;
            }

            return desiredSize;
        }

        /// <summary>
        /// Gets next item from the specified index according to the direction.
        /// </summary>
        /// <param name="startIndex">Index from which searching is started.</param>
        /// <param name="direction">The direction to search the tab item.</param>
        /// <returns>Next tab item.</returns>
        internal TabPopupMenuItem FindNextTabItem(int startIndex, int direction)
        {
            int index = startIndex;
            for (int i = 0; i < this.MenuItems.Count; i++)
            {
                index += direction;
                if (index >= this.MenuItems.Count)
                {
                    index = 0;
                }
                else if (index < 0)
                {
                    index = this.MenuItems.Count - 1;
                }

                TabPopupMenuItem itemAtIndex = this.MenuItems[index];
                if (((itemAtIndex != null) && itemAtIndex.IsEnabled) && (itemAtIndex.Visibility == Visibility.Visible))
                {
                    return itemAtIndex;
                }
            }

            return null;
        }

        /// <summary>
        /// Occurs when menu items collection si changed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void MenuItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
			switch (e.Action)
			{
				case NotifyCollectionChangedAction.Add:
				case NotifyCollectionChangedAction.Replace:
					this.UpdateItems(e.NewItems);
					break;
			}
        }

        /// <summary>
        /// Occurs when popup is opened.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void PopupOpened(object sender, EventArgs e)
        {
            if (this.Opened != null)
            {
                this.Opened(sender, new RoutedEventArgs());
            }
        }

        /// <summary>
        /// Occurs when popup is closed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void PopupClosed(object sender, EventArgs e)
        {
            if (this.Closed != null)
            {
                this.Closed(sender, new RoutedEventArgs());
            }
        }

        /// <summary>
        /// Occurs when the left mouse button is pressed while the mouse pointer
        /// is over this control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void RootMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!this.bIsMouseOver && this.popup != null)
            {
                this.Close();
            }
        }

        /// <summary>
        /// Occurs when the mouse enters the element.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void PopupMouseEnter(object sender, MouseEventArgs e)
        {
            this.bIsMouseOver = true;
        }

        /// <summary>
        /// Occurs when the mouse leaves the element.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void PopupMouseLeave(object sender, MouseEventArgs e)
        {
            this.bIsMouseOver = false;
        }

        /// <summary>
        /// Updates the items.
        /// </summary>
		private void UpdateItems()
		{
			this.UpdateItems(this.MenuItems);
		}

        /// <summary>
        /// Updates the items.
        /// </summary>
        /// <param name="items">The items.</param>
		private void UpdateItems(IList items)
		{
			foreach (TabPopupMenuItem item in items)
			{
                if (TabControlParent != null && TabControlParent.MenuItemStyle != null)
                {
                    item.Style = TabControlParent.MenuItemStyle;
                }
                else
                {
                    item.Style = this.MenuItemStyle;
                }
			}
		}

        /// <summary>
        /// Shows the popup.
        /// </summary>
        public void Show()
        {
            if(this.TabControlParent != null)
            this.TabControlParent.FireDropDownContextMenuOpen(this.MenuItems);
            this.popup.IsOpen = true;
        }

        /// <summary>
        /// Closes the popup.
        /// </summary>
        public void Close()
        {
            this.popup.IsOpen = false;
        }
        #endregion
    }
}
