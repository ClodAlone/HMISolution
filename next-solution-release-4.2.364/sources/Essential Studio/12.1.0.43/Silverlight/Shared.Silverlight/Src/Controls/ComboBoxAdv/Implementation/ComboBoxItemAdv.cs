#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Syncfusion.Windows.Tools.Controls
{
#if !WPF
    /// <summary>
    ///
    /// </summary>
    public class ComboBoxItemAdv : ContentControl
#else

    public class ComboBoxItemAdv : ContentControl
#endif
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ComboBoxItemAdv"/> class.
        /// </summary>
        public ComboBoxItemAdv()
        {
            DefaultStyleKey = typeof(ComboBoxItemAdv);
        }

        internal new ComboBoxAdv Parent;

        internal CheckBox CheckBox;

#if WPF

        //Selected event called when the ComboBoxItemAdv is Selected
        public static readonly RoutedEvent SelectedEvent = EventManager.RegisterRoutedEvent("SelectedEvent", RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(ComboBoxItemAdv));

        public event RoutedEventHandler Selected
        {
            add { this.AddHandler(SelectedEvent, value); }
            remove { this.RemoveHandler(SelectedEvent, value); }
        }

        //UnSelected event called when the ComboBoxItemAdv is UnSelected
        public static readonly RoutedEvent UnSelectedEvent = EventManager.RegisterRoutedEvent("UnSelectedEvent", RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(ComboBoxItemAdv));

        public event RoutedEventHandler UnSelected
        {
            add { this.AddHandler(UnSelectedEvent, value); }
            remove { this.RemoveHandler(SelectedEvent, value); }
        }

#endif

        /// <summary>
        ///
        /// </summary>
        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsSelected.  This enables animation, styling, binding, etc...
#if WPF

        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(ComboBoxItemAdv), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(ComboBoxItemAdv.OnIsSelectedChanged)));

#else
        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(ComboBoxItemAdv), new PropertyMetadata(false, new PropertyChangedCallback(OnIsSelectedChanged)));
#endif

        /// <summary>
        ///
        /// </summary>
        public bool IsHighlighted
        {
            get { return (bool)GetValue(IsHighlightedProperty); }
            internal set { SetValue(IsHighlightedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsHighlighted.  This enables animation, styling, binding, etc...
        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty IsHighlightedProperty =
            DependencyProperty.Register("IsHighlighted", typeof(bool), typeof(ComboBoxItemAdv), new PropertyMetadata(false));

        /// <summary>
        ///
        /// </summary>
        public bool IsPressed
        {
            get { return (bool)GetValue(IsPressedProperty); }
            protected set { SetValue(IsPressedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsHighlighted.  This enables animation, styling, binding, etc...
        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty IsPressedProperty =
            DependencyProperty.Register("IsPressed", typeof(bool), typeof(ComboBoxItemAdv), new PropertyMetadata(false));

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Element.MouseLeftButtonDown"/> routed event is raised on this element. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs"/> that contains the event data. The event data reports that the left mouse button was pressed.</param>
        protected override void OnMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            this.IsPressed = true;
            if (Parent != null)
            {
                Parent.ExternalChange = false;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseLeftButtonUp(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            UpdateSelection();
            if (!Parent.AllowMultiSelect)
            {
                if (Parent.IsDropDownOpen)
                {
                    Parent.IsDropDownOpen = false;
                }
            }
            this.IsPressed = false;
        }

        internal void UpdateSelection()
        {
            if (!IsSelected)
            {
                if (this.Content != null)
                    IsSelected = true;
#if SILVERLIGHT
                VisualStateManager.GoToState(this, "Selected", true);
#endif
                if (!Parent.AllowMultiSelect)
                {
                    for (int i = 0; i < Parent.Items.Count; i++)
                    {
                        ComboBoxItemAdv boxItem = Parent.ItemContainerGenerator.ContainerFromIndex(i) as ComboBoxItemAdv;
                        if (boxItem == null)
                            boxItem = Parent.Items[i] as ComboBoxItemAdv;
                        if (boxItem != null && boxItem != this)
                        {
                            boxItem.IsSelected = false;
#if SILVERLIGHT
                            VisualStateManager.GoToState(boxItem, "Normal", true);
#endif
                        }
                    }
                    IsSelected = false;
                }
            }
            else if (Parent.AllowMultiSelect)
            {
                ObservableCollection<object> selItems = new ObservableCollection<object>(Parent.SelectedItems.Cast<object>());
                if (selItems != null)
                {
                    object item = Parent.ItemContainerGenerator.ItemFromContainer(this) as object;
                    if (item != null)
                    {
                        if (selItems.Contains(item))
                        {
                            selItems.Remove(item);
                        }
                    }
                }
                IsSelected = false;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="e"></param>
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
            VisualStateManager.GoToState(this, "Focussed", true);
        }

        /// <summary>
        ///
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            CheckBox = GetTemplateChild("PART_CheckBox") as CheckBox;
            if (CheckBox != null)
            {
                CheckBox.Checked -= new RoutedEventHandler(CheckBox_Checked);
                CheckBox.Unchecked -= new RoutedEventHandler(CheckBox_Unchecked);
                CheckBox.Checked += new RoutedEventHandler(CheckBox_Checked);
                CheckBox.Unchecked += new RoutedEventHandler(CheckBox_Unchecked);
            }

            if (Parent != null)
            {
                Parent.UpdateSelectMode();
                if (Parent.AllowMultiSelect)
                {
                    Parent.removeFlag = false;
                    if (Parent.newItem != null && Parent.Items.Contains(Parent.newItem))
                    {
                        foreach (var selItem in Parent.SelItemsInternal)
                        {
                            foreach (var item in Parent.Items)
                            {
                                if (item != null && item.Equals(selItem) && Parent.newItem.Equals(item))
                                {
                                    if (Parent.itemcount >= 1)
                                    {
                                        ComboBoxItemAdv boxItem = Parent.ItemContainerGenerator.ContainerFromItem(Parent.newItem) as ComboBoxItemAdv;
                                        if ((boxItem != null && boxItem.IsSelected) || boxItem == null)
                                            Parent.removeFlag = true;
                                    }
                                    Parent.itemcount++;
                                }
                            }
                        }
                        if (Parent.removeFlag)
                            if (Parent.SelItemsInternal.Count > 0)
                                Parent.SelItemsInternal.Remove(Parent.newItem);
                        Parent.SelectItems();
                        Parent.oldItem = null;
                        Parent.itemcount = 0;
                    }
                    if (Parent.oldItem != null && this.DataContext != null)
                    {
                        if (this.DataContext.Equals(Parent.oldItem))
                        {
                            if (this.CheckBox != null)
                            {
                                Parent.internalSelect = true;
                                this.CheckBox.IsChecked = false;
                                Parent.internalSelect = false;
                            }
                        }
                        else
                            Parent.SelectItems();
                    }
                    else
                        Parent.SelectItems();
                }
            }
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!Parent.internalSelect && IsPressed)
                this.IsSelected = false;
            if (Parent != null && Parent.AllowMultiSelect)
            {
                ObservableCollection<object> selItems = new ObservableCollection<object>(Parent.SelectedItems.Cast<object>());
                if (selItems.Count <= 0)
                {
                    Parent.SelectedIndex = -1;
                }
                if (selItems != null)
                {
                    object item = Parent.ItemContainerGenerator.ItemFromContainer(this) as object;
                    if (item != null)
                    {
                        if (selItems.Contains(item) && !Parent.AllowSelect)
                        {
                            selItems.Remove(item);
                        }
                    }
                }
                Parent.UpdateSelectionBox();
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (!Parent.internalSelect && this.Content != null)
                this.IsSelected = true;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseMove(System.Windows.Input.MouseEventArgs e)
        {
            base.OnMouseMove(e);
            Parent.IsGotKeyBoardFocus = false;
#if !WPF
            if (Parent != null)
            {
                Parent.Items.ToList().ForEach(i =>
                    {
                        ComboBoxItemAdv item=Parent.ItemContainerGenerator.ContainerFromItem(i) as ComboBoxItemAdv;
                        VisualStateManager.GoToState(item, "Normal", true);
                    });
            }
#endif
            VisualStateManager.GoToState(this, "MouseOver", true);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseEnter(System.Windows.Input.MouseEventArgs e)
        {
            if (Parent != null && !Parent.IsGotKeyBoardFocus)
            {
                Parent.NotifyComboBoxItemAdvEnter(this, true);
            }
            base.OnMouseEnter(e);
        }

#if WPF

        protected override void OnGotKeyboardFocus(System.Windows.Input.KeyboardFocusChangedEventArgs e)
        {
            if (Parent != null)
            {
                Parent.IsGotKeyBoardFocus = true;
                Parent.NotifyComboBoxItemAdvEnter(this, true);
            }
            base.OnGotKeyboardFocus(e);
        }

        protected override void OnLostKeyboardFocus(System.Windows.Input.KeyboardFocusChangedEventArgs e)
        {
            if (Parent != null)
            {
                Parent.NotifyComboBoxItemAdvEnter(this, false);
            }
            base.OnLostKeyboardFocus(e);
        }

#endif

        /// <summary>
        ///
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseLeave(System.Windows.Input.MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            VisualStateManager.GoToState(this, "Normal", true);
            if (Parent != null && !Parent.IsGotKeyBoardFocus)
            {
                Parent.NotifyComboBoxItemAdvEnter(this, false);
            }
        }

        private static void OnIsSelectedChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            ComboBoxItemAdv instance = sender as ComboBoxItemAdv;
            if (instance != null)
            {
#if WPF
                if (instance.IsSelected)
                {
                    RoutedEventArgs e = new RoutedEventArgs();
                    e.RoutedEvent = ComboBoxItemAdv.SelectedEvent;
                    e.Source = instance;
                    instance.RaiseEvent(e);
                }
                else
                {
                    RoutedEventArgs e = new RoutedEventArgs();
                    e.RoutedEvent = ComboBoxItemAdv.UnSelectedEvent;
                    e.Source = instance;
                    instance.RaiseEvent(e);
                }
#endif
                if (instance.Parent != null)
                {
                    if (!instance.Parent.AllowMultiSelect)
                    {
                        instance.Parent.SelItemsInternal.Clear();
                    }
                    if (!instance.Parent.internalSelect)
                    {
                        if (instance.Parent.ItemsSource != null && instance.DataContext != null)
                        {
                            if (instance.IsSelected)
                                instance.Parent.SelItemsInternal.Add(instance.DataContext);
                            else if (instance.Parent.SelItemsInternal.Contains(instance.DataContext))
                                instance.Parent.SelItemsInternal.Remove(instance.DataContext);
                        }
                        else if (instance.Content != null)
                        {
                            if (instance.IsSelected)
                                instance.Parent.SelItemsInternal.Add(instance);
                            else if (instance.Parent.SelItemsInternal.Contains(instance))
                                instance.Parent.SelItemsInternal.Remove(instance);
                        }
                    }

                    if (instance.Parent.SelectedItems == null && instance.Parent.SelItemsInternal.Count > 0)
                    {
                        ObservableCollection<object> collecion = new ObservableCollection<object>();
                        foreach (var item in instance.Parent.SelItemsInternal)
                        {
                            collecion.Add(item);
                        }
                        instance.Parent.SelectedItems = collecion;
                    }

                    if (instance.Parent.SelectedItems != null)
                    {
                        if (!instance.Parent.AllowSelect)
                        {
                            ObservableCollection<object> selItems =
                                new ObservableCollection<object>(instance.Parent.SelectedItems.Cast<object>());
                            ObservableCollection<ComboBoxItemAdv> selComboItems =
                                new ObservableCollection<ComboBoxItemAdv>();
                            foreach (object item in instance.Parent.SelectedItems)
                            {
                                ComboBoxItemAdv cItem =
                                    instance.Parent.ItemContainerGenerator.ContainerFromItem(item) as ComboBoxItemAdv;
                                if (cItem != null)
                                    selComboItems.Add(cItem);
                                else if (item is ComboBoxItemAdv)
                                    selComboItems.Add(item as ComboBoxItemAdv);
                            }

                            for (int i = 0; i < instance.Parent.Items.Count; i++)
                            {
                                ComboBoxItemAdv boxItem =
                                    instance.Parent.ItemContainerGenerator.ContainerFromIndex(i) as ComboBoxItemAdv;
                                if (boxItem != null && boxItem.Content != null)
                                {
                                    if (boxItem.IsSelected && !instance.Parent.AllowSelect)
                                    {
                                        VisualStateManager.GoToState(boxItem, "Selected", true);

                                        if (!instance.Parent.AllowMultiSelect && selComboItems.Contains(boxItem) &&
                                            selComboItems.Count > 0)
                                        {
                                            instance.Parent.SelectedIndex = i;
                                            if (instance.Parent.SelectedItem is ComboBoxItemAdv)
                                            {
                                                instance.Parent.SelectionBoxItem =
                                                    (instance.Parent.SelectedItem as ComboBoxItemAdv).Content;
                                            }
                                            else
                                                instance.Parent.SelectionBoxItem = instance.Parent.SelectedItem;
                                        }

                                        if (!selItems.Contains(instance.Parent.Items[i]))
                                        {
                                            selItems.Add(instance.Parent.Items[i]);
                                        }
                                    }
                                    else
                                    {
                                        VisualStateManager.GoToState(boxItem, "Unselected", true);
                                    }
                                }
                            }
                        }
                        instance.Parent.UpdateSelectionBox();
                    }
                }

                if (instance.Parent != null && instance.Parent.ExternalChange)
                    instance.Parent.ExternalChange = false;
            }
        }
    }
}