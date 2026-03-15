#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.ComponentModel;
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Tools.Controls 
{
    /// <summary>
    /// Represents the RibbonMenu Class.
    /// </summary>
    [ContentProperty("ItemsPane")]
    public class RibbonMenu : RibbonDropDown
    {
        #region Constructor

        /// <summary>
        /// Initialize a new instance of <see cref="RibbonMenu"/>
        /// </summary>
        public RibbonMenu()
        {
            this.DefaultStyleKey = typeof(RibbonMenu);
        }

        /// <summary>
        /// Initializes the <see cref="RibbonMenu"/> class.
        /// </summary>
        static RibbonMenu()
        {
            if (DesignerProperties.IsInDesignTool)
            {
                LoadDependentAssemblies load = new LoadDependentAssemblies();
                load = null;
            }
        }
        #endregion

        #region Properties

        #region ItemsPane

        /// <summary>
        /// Gets or sets <see cref="ItemsControl"/> used to generate the content of the menu.
        /// </summary>
        public RibbonItemsControl ItemsPane
        {
            get { return (RibbonItemsControl)GetValue(ItemsPaneProperty); }
            set { SetValue(ItemsPaneProperty, value); }
        }

        /// <summary>
        /// Identifier for <see cref="ItemsPane"/> property.
        /// </summary>
        public static readonly DependencyProperty ItemsPaneProperty = DependencyProperty.Register(
            "ItemsPane",
            typeof(RibbonItemsControl),
            typeof(RibbonMenu),
            new PropertyMetadata(new PropertyChangedCallback(ItemsPaneChangedCallback)));

        /// <summary>
        /// Itemses the pane changed callback.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void ItemsPaneChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((RibbonMenu)d).OnItemsPaneChanged(e.OldValue as RibbonItemsControl, e.NewValue as RibbonItemsControl);
        }

        #endregion

        #region SelectedIndex

        /// <summary>
        /// Gets or sets the index specifying the currently selected item in <see cref="ItemsPane"/> collection.
        /// </summary>
        public int SelectedIndex
        {
            get { return (int)GetValue(SelectedIndexProperty); }
            set { SetValue(SelectedIndexProperty, value); }
        }

        /// <summary>
        /// The identifier of <see cref="SelectedIndex"/> property
        /// </summary>
        public static readonly DependencyProperty SelectedIndexProperty = DependencyProperty.Register("SelectedIndex", typeof(int), typeof(RibbonMenu), new PropertyMetadata(-1, new PropertyChangedCallback(SelectedIndexChangedCallback)));

        /// <summary>
        /// Selecteds the index changed callback.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void SelectedIndexChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((RibbonMenu)d).OnSeletedIndexChanged((int)e.OldValue, (int)e.NewValue);
        }

        #endregion

        #region SelectedItem

        /// <summary>
        /// Gets or sets the selected item.
        /// </summary>
        /// <value>The selected item.</value>
        internal RibbonItemBase SelectedItem
        {
            get
            {
                return this.selectedItem;
            }

            set
            {
                if (this.selectedItem != value)
                {
                    if (this.selectedItem != null)
                    {
                        this.selectedItem.IsSelected = false;
                    }

                    this.selectedItem = value;

                    this.SelectedIndex = this.GetItemIndex(this.ItemsPane, value);
                }
            }
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

            if (this.mainPanelPresenter != null)
            {
                this.mainPanelPresenter.Content = null;
            }

            this.mainPanelPresenter = this.GetTemplateChild("Part_MainPanelPresenter") as ContentPresenter;

            this.OnItemsPaneChanged(null, this.ItemsPane);
        }

        /// <summary>
        /// Called when [is open changed].
        /// </summary>
        /// <param name="isOpen">if set to <c>true</c> [is open].</param>
        public override void OnIsOpenChanged(bool isOpen)
        {
            base.OnIsOpenChanged(isOpen);

            if (isOpen)
            {
               // this.SelectItem(this.GetFirstItem());
            }
            else
            {
                this.SelectedItem = null;
            }
        }

        /// <summary>
        /// Processes the key down.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.KeyEventArgs"/> instance containing the event data.</param>
        public override void ProcessKeyDown(KeyEventArgs e)
        {
            if (this.IsOpen)
            {
                if (!e.Handled)
                {
                    switch (e.Key)
                    {
                        case Key.Up:
                            this.SelectItem(this.GetPrevItem());
                            e.Handled = true;
                            break;
                        case Key.Down:
                            this.SelectItem(this.GetNextItem());
                            e.Handled = true;
                            break;
                        case Key.Home:
                            this.SelectItem(this.GetFirstItem());
                            e.Handled = true;
                            break;
                        case Key.End:
                            this.SelectItem(this.GetLastItem());
                            e.Handled = true;
                            break;
                        case Key.Left:
                            this.IsOpen = false;
                            e.Handled = true;
                            break;
                        case Key.Enter:
                            if (this.SelectedItem != null)
                            {
                                this.SelectedItem.RaiseKeyDown(e);
                            }

                            e.Handled = true;
                            break;
                    }
                }
            }

            base.ProcessKeyDown(e);
        }

        /// <summary>
        /// Called when [seleted index changed].
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        internal virtual void OnSeletedIndexChanged(int oldValue, int newValue)
        {
        }

        /// <summary>
        /// Called when [items pane changed].
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        internal virtual void OnItemsPaneChanged(RibbonItemsControl oldValue, RibbonItemsControl newValue)
        {
            if (oldValue != null)
            {
                oldValue.ItemClicked -= new System.EventHandler(this.OnItemClicked);
                oldValue.ItemSelected -= new System.EventHandler(this.OnItemSelected);
                oldValue.QueryDropDownBounds -= new EventHandler<BoundsEventArgs>(this.QueryDropDownBoundsHandler);
            }

            if (newValue != null)
            {
                newValue.ItemClicked += new System.EventHandler(this.OnItemClicked);
                newValue.ItemSelected += new System.EventHandler(this.OnItemSelected);
                newValue.QueryDropDownBounds += new EventHandler<BoundsEventArgs>(this.QueryDropDownBoundsHandler);

                FrameworkElement root = this.GetTemplateChild("Part_Root") as FrameworkElement;

                if (root != null && root.Resources != null)
                {
                    newValue.Style = root.Resources["ItemsPaneStyle"] as Style;
                }
            }

            if (this.mainPanelPresenter != null)
            {
                this.mainPanelPresenter.Content = newValue;
            }
        }

        /// <summary>
        /// Called when [query drop down bounds].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="Syncfusion.Windows.Tools.Controls.BoundsEventArgs"/> instance containing the event data.</param>
        internal virtual void OnQueryDropDownBounds(object sender, BoundsEventArgs args)
        {
        }

        #endregion

        #region Event handlers

        /// <summary>
        /// Called when [item clicked].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnItemClicked(object sender, System.EventArgs e)
        {
            this.IsOpen = false;
        }

        /// <summary>
        /// Called when [item selected].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnItemSelected(object sender, System.EventArgs e)
        {
            this.SelectedItem = sender as RibbonItemBase;
        }

        /// <summary>
        /// Queries the drop down bounds handler.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="Syncfusion.Windows.Tools.Controls.BoundsEventArgs"/> instance containing the event data.</param>
        private void QueryDropDownBoundsHandler(object sender, BoundsEventArgs args)
        {
            this.OnQueryDropDownBounds(sender, args);
        }

        #endregion

        #region Implementation

        /// <summary>
        /// Gets the index of the item.
        /// </summary>
        /// <param name="itemsControl">The items control.</param>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        internal int GetItemIndex(ItemsControl itemsControl, RibbonItemBase item)
        {
            if (itemsControl != null && item != null)
            {
                return itemsControl.Items.IndexOf(item);
            }

            return -1;
        }

        /// <summary>
        /// Gets the prev item.
        /// </summary>
        /// <returns></returns>
        private RibbonItemBase GetPrevItem()
        {
            ItemsControl itemsControl = this.ItemsPane;

            if (itemsControl != null)
            {
                IList items = itemsControl.Items;

                int itemsCount = items.Count;

                if (this.SelectedIndex >= 0 && this.SelectedIndex < itemsCount)
                {
                    int sItem = this.SelectedIndex;

                    for (int i = 0; i < itemsCount; i++)
                    {
                        if (sItem > 0 && sItem < itemsCount)
                        {
                            sItem -= 1;
                        }
                        else if (itemsCount - 1 >= 0)
                        {
                            sItem = itemsCount - 1;
                        }

                        RibbonItemBase item = items[sItem] as RibbonItemBase;

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
        /// <returns></returns>
        private RibbonItemBase GetNextItem()
        {
            ItemsControl itemsControl = this.ItemsPane;

            if (itemsControl != null)
            {
                IList items = itemsControl.Items;

                int itemsCount = items.Count;

                int sItem = this.SelectedIndex;

                for (int i = 0; i < itemsCount; i++)
                {
                    if (sItem >= 0 && sItem + 1 < itemsCount)
                    {
                        sItem += 1;
                    }
                    else if (itemsCount > 0)
                    {
                        sItem = 0;
                    }

                    RibbonItemBase item = items[sItem] as RibbonItemBase;

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
        /// <returns></returns>
        private RibbonItemBase GetFirstItem()
        {
            ItemsControl itemsControl = this.ItemsPane;

            if (itemsControl != null)
            {
                IList items = itemsControl.Items;

                int itemsCount = items.Count;

                for (int i = 0; i < itemsCount; i++)
                {
                    RibbonItemBase item = items[i] as RibbonItemBase;
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
        /// <returns></returns>
        private RibbonItemBase GetLastItem()
        {
            ItemsControl itemsControl = this.ItemsPane;

            if (itemsControl != null)
            {
                IList items = itemsControl.Items;

                int itemsCount = items.Count;

                for (int i = itemsCount - 1; i >= 0; i--)
                {
                    RibbonItemBase item = items[i] as RibbonItemBase;

                    if (item.Visibility == Visibility.Visible)
                    {
                        return item;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Selects the item.
        /// </summary>
        /// <param name="item">The item.</param>
        private void SelectItem(RibbonItemBase item)
        {
            if (item != null)
            {
                item.IsSelected = true;
            }
        }

        #endregion

        #region Fields

        private RibbonItemBase selectedItem = null;

        private ContentPresenter mainPanelPresenter;

        #endregion
    }
}
