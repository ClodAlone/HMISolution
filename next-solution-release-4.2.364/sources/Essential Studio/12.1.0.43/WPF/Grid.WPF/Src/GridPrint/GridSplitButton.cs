#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Windows.Controls.Grid
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows.Controls;
    using System.Windows;
    using System.Windows.Controls.Primitives;
    using System.Collections.ObjectModel;
    using System.Windows.Markup;
    using System.ComponentModel;
    using System.Collections;
    using System.Windows.Data;

    [TemplatePart(Name = GridSplitButton.SplitListBoxName, Type = typeof(ListBox))]
    [TemplatePart(Name = GridSplitButton.SplitToggleButtonName, Type = typeof(ToggleButton))]
    [ContentProperty("Items")]
    [DefaultProperty("Items")]
    internal class GridSplitButton : ContentControl
    {
        public const string SplitListBoxName = "PART_ListBox";
        public const string SplitToggleButtonName = "PART_ToggleButton";
        public static double PopupMaxDropDownHeight = SystemParameters.PrimaryScreenHeight / 3.0;

        static GridSplitButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GridSplitButton), new FrameworkPropertyMetadata(typeof(GridSplitButton)));
        }

        public GridSplitButton()
        {
        }

        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(
            "ItemsSource",
            typeof(IEnumerable),
            typeof(GridSplitButton));

        public IEnumerable ItemsSource
        {
            get
            {
                return (IEnumerable)this.GetValue(GridSplitButton.ItemsSourceProperty);
            }

            set
            {
                this.SetValue(GridSplitButton.ItemsSourceProperty, value);
            }
        }

        public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register(
            "SelectedItem",
            typeof(object),
            typeof(GridSplitButton));

        public object SelectedItem
        {
            get
            {
                return this.GetValue(GridSplitButton.SelectedItemProperty);
            }

            set
            {
                this.SetValue(GridSplitButton.SelectedItemProperty, value);
            }
        }

        public static readonly DependencyProperty SelectedIndexProperty = DependencyProperty.Register(
            "SelectedIndex",
            typeof(int),
            typeof(GridSplitButton));

        public int SelectedIndex
        {
            get
            {
                return (int)this.GetValue(GridSplitButton.SelectedIndexProperty);
            }

            set
            {
                this.SetValue(GridSplitButton.SelectedIndexProperty, value);
            }
        }

        private ObservableCollection<object> items;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Bindable(true)]
        public ObservableCollection<object> Items
        {
            get
            {
                if (this.items == null)
                {
                    this.CreateItemCollection();
                }

                return this.items;
            }
        }

        private void CreateItemCollection()
        {
            this.items = new ObservableCollection<object>();
            items.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(OnCollectionChanged);
        }

        private void OnCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (this.splitListBox == null)
            {
                return;
            }

            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                this.splitListBox.Items.Add(e.NewItems[0]);
                
            }
            else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                this.splitListBox.Items.Remove(e.OldItems[0]);
            }
            
            
        }

        public void ClosePopup()
        {
            this.SplitToggleButton.IsChecked = false;
        }

        private ListBox splitListBox;
        public ListBox SplitListBox
        {
            get
            {
                return this.splitListBox;
            }
        }

        private ToggleButton splitToggleButton;
        public ToggleButton SplitToggleButton
        {
            get
            {
                return this.splitToggleButton;
            }
        }

        public override void OnApplyTemplate()
        {
            if (this.splitListBox != null)
            {
                this.splitListBox.SelectionChanged -= new SelectionChangedEventHandler(splitListBox_SelectionChanged);
            }

            base.OnApplyTemplate();

            this.splitListBox = this.GetTemplateChild(GridSplitButton.SplitListBoxName) as ListBox;
            if (this.splitListBox != null)
            {
                if (this.ItemsSource == null)
                {
                    foreach (var item in this.Items)
                    {
                        this.splitListBox.Items.Add(item);
                    }
                }
                else
                {
                    var itemsSourceBinding = new Binding("ItemsSource") { Source = this };
                    this.splitListBox.SetBinding(ListBox.ItemsSourceProperty, itemsSourceBinding);
                }

                var selectedItemBinding = new Binding("SelectedItem") { Source = this };
                this.splitListBox.SetBinding(ListBox.SelectedItemProperty, selectedItemBinding);
                this.splitListBox.SelectionChanged += new SelectionChangedEventHandler(splitListBox_SelectionChanged);

                var selectedIndexBinding = new Binding("SelectedIndex") { Source = this };
                this.splitListBox.SetBinding(ListBox.SelectedIndexProperty, selectedIndexBinding);
            }

            this.splitToggleButton = this.GetTemplateChild(GridSplitButton.SplitToggleButtonName) as ToggleButton;
        }

        void splitListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.OnSelectionChanged(e);
        }

        protected virtual void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            e.RoutedEvent = GridSplitButton.SelectionChangedEvent;
            e.Source = this;
            base.RaiseEvent(e);
        }

        public static readonly RoutedEvent SelectionChangedEvent = EventManager.RegisterRoutedEvent(
            "SelectionChanged",
            RoutingStrategy.Direct,
            typeof(SelectionChangedEventHandler),
            typeof(GridSplitButton));

        public event SelectionChangedEventHandler SelectionChanged
        {
            add
            {
                this.AddHandler(GridSplitButton.SelectionChangedEvent, value);
            }

            remove
            {
                this.RemoveHandler(GridSplitButton.SelectionChangedEvent, value);
            }
        }

    }
}
