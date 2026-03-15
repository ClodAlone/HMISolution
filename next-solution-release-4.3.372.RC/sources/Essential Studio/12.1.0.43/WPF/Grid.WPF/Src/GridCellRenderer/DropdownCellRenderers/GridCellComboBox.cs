#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace Syncfusion.Windows.Controls.Grid
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;
    using System.ComponentModel;

    /// <summary>
    /// For internal use.
    /// </summary>
#if SyncfusionFramework4_0
    [DesignTimeVisible(false)]
#endif
    public class HoverListBox : ListBox
    {
        /// <summary>
        /// For internal use.
        /// </summary>
        internal HoverListBox(GridCellComboBoxDropDown dropDown)
        {
            this.DefaultStyleKey = typeof(HoverListBox);
            this.DropDownPopupHost = dropDown;
            this.SelectionMode = System.Windows.Controls.SelectionMode.Single;
        }

        /// <summary>
        /// For internal use.
        /// </summary>
        public GridCellComboBoxDropDown DropDownPopupHost
        {
            get;
            private set;
        }

        /// <summary>
        /// For internal use.
        /// </summary>
        internal void NotifyListBoxItemEnter(HoverListBoxItem item)
        {
            //item.Focus();
            this.HighlightedItem = item;
            this.Items.MoveCurrentTo(item.Content);
            //var container = this.ItemContainerGenerator.ItemFromContainer(item) as HoverListBoxItem;
            //if (container != null)
            //{
            this.SelectedIndex = this.ItemContainerGenerator.IndexFromContainer(item);
            //}
            //else
            //{
            //    this.SelectedIndex = this.Items.IndexOf(item);
            //}
        }

        /// <summary>
        /// For internal use.
        /// </summary>
        internal void NotifyListBoxItemMouseUp(HoverListBoxItem item)
        {
            //if(!((HoverListBoxItem)this.SelectedItem).Equals(item))
            //    this.SelectedItem = item;
            this.DropDownPopupHost.Close();
        }

        /// <summary>
        /// For internal use.
        /// </summary>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return (item is HoverListBoxItem);
        }

        /// <summary>
        /// For internal use.
        /// </summary>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new HoverListBoxItem();
        }

        private WeakReference highlightedItem = null;

        /// <summary>
        /// For internal use.
        /// </summary>
        internal HoverListBoxItem HighlightedItem
        {
            get
            {
                if (this.highlightedItem == null)
                {
                    return null;
                }

                return this.highlightedItem.Target as HoverListBoxItem;
            }

            set
            {
                HoverListBoxItem item = (this.highlightedItem != null) ? (this.highlightedItem.Target as HoverListBoxItem) : null;
                if (item != null)
                {
                    item.SetIsHighlighted(false);
                }
                if (value != null)
                {
                    this.highlightedItem = new WeakReference(value);
                    for (int i = 0;i < this.Items.Count;i++)
                    {
                        var prevItem = (HoverListBoxItem)this.ItemContainerGenerator.ContainerFromIndex(i);
                        if (prevItem != null && prevItem.IsHighlighted)
                        {
                            prevItem.IsHighlighted = false;
                        }
                    }
                    value.SetIsHighlighted(true);
                }
                else
                {
                    this.highlightedItem = null;
                }
            }
        }

        internal ItemsPresenter ItemPresenter;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            ItemPresenter = this.GetTemplateChild("PART_ItemsPresenter") as ItemsPresenter;
        }
    }

    /// <summary>
    /// For internal use.
    /// </summary>
    
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif

    public class HoverListBoxItem : ListBoxItem
    {
        static HoverListBoxItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(HoverListBoxItem), new FrameworkPropertyMetadata(typeof(HoverListBoxItem)));
        }

        /// <summary>
        /// For internal use.
        /// </summary>
        public HoverListBoxItem()
        {
            
        }

        /// <summary>
        /// For internal use.
        /// </summary>
        internal HoverListBox ListBox
        {
            get
            {
                return ((ItemsControl.ItemsControlFromItemContainer(this) as Selector)) as HoverListBox;
            }
        }

        public static readonly DependencyProperty IsHighlightedProperty = DependencyProperty.Register(
            "IsHighlighted",
            typeof(bool),
            typeof(HoverListBoxItem),
            new PropertyMetadata(false, OnIsHighlightedChanged));
        
        private static void OnIsHighlightedChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            //var listBoxItem = dpo as HoverListBoxItem; Unused local variable
            //System.Diagnostics.Debug.WriteLine(string.Format("IsHighlighted {0}", args.NewValue));
        }

        /// <summary>
        /// For internal use.
        /// </summary>
        public bool IsHighlighted
        {
            get
            {
                return (bool)this.GetValue(HoverListBoxItem.IsHighlightedProperty);
            }

            set
            {
                this.SetValue(HoverListBoxItem.IsHighlightedProperty, value);
            }
        }

        /// <summary>
        /// For internal use.
        /// </summary>
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            var parentListBox = this.ListBox;
            if (parentListBox != null && this.ListBox.DropDownPopupHost.IsMouseTrackingEnabled)//if the listbox is not null and Mouse Tracking is enabled then the mouse move has been noticed.
            {
                parentListBox.NotifyListBoxItemEnter(this);
            }
        }

        /// <summary>
        /// For internal use.
        /// </summary>
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            var parentListBox = this.ListBox;
            if (parentListBox != null)
            {
                parentListBox.NotifyListBoxItemMouseUp(this);
            }

            e.Handled = true;
            base.OnMouseLeftButtonUp(e);
        }
        /// <summary>
        /// For internal use.
        /// </summary>

        internal void SetIsHighlighted(bool value)
        {
            this.IsHighlighted = value;
        }
    }

    /// <summary>
    /// Defines the drop-down combobox control that can be used for Combobox cells.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class GridCellComboBoxDropDown : GridCellDropDownControlBase
    {
        /// <summary>
        /// Gets the <see cref="ListBox"/> that is displayed in the drop-down part.
        /// </summary>
        internal HoverListBox ListBoxPart
        {
            get
            {
                if (this.PopupContent != null)
                {
                    return this.PopupContent.Content as HoverListBox;
                }
                return null;
            }
        }

        protected override void OnContentLoaded(ContentControl popupContent)
        {
            popupContent.Content = new HoverListBox(this);
            // this will wire the events in the base implementation
            base.OnContentLoaded(popupContent);
        }
    }
}
