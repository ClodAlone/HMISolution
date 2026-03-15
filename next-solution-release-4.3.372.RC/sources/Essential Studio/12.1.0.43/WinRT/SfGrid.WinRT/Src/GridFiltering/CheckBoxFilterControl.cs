#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.Data.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
#if WinRT
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Core;
using Windows.UI.Input;
using Windows.Foundation;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Syncfusion.WinRT.Data;
using Key = Windows.System.VirtualKey;
using KeyEventArgs = Windows.UI.Xaml.Input.KeyRoutedEventArgs;
using Windows.UI.Xaml.Data;
using System.Collections;
using Syncfusion.UI.Xaml.Utility;
#else
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Collections;
using Syncfusion.UI.Xaml.Utility;
#endif

namespace Syncfusion.UI.Xaml.Grid
{
    public class CheckboxFilterControl : ContentControl, IDisposable
    {
        #region Private Members
        internal bool propertyChangedFromSelectAll = false;
        internal IEnumerable<FilterElement> FilterListBoxItem = new List<FilterElement>();
        internal IEnumerable<FilterElement> searchedItems = new List<FilterElement>();
        internal bool isSourceChangedasSearchedItems = false;
        internal GridFilterControl gridFilterCtrl = null;
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #region Dependency Properties
#if !WPF
        #region FilteredFrom
        /// <summary>
        /// DependencyProperty Registration for FilteredFrom of GridFilterControl
        /// </summary>
        /// <remarks></remarks>
        public static readonly DependencyProperty FilteredFromProperty = DependencyProperty.Register(
          "FilteredFrom", typeof(FilteredFrom), typeof(CheckboxFilterControl), new PropertyMetadata(FilteredFrom.None));

        /// <summary>
        /// Gets or sets FilteredFrom for GridFilterControl.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public FilteredFrom FilteredFrom
        {
            get { return (FilteredFrom)this.GetValue(CheckboxFilterControl.FilteredFromProperty); }
            set { this.SetValue(CheckboxFilterControl.FilteredFromProperty, value); }
        }

        #endregion
#endif
        #region SearchOptionVisibilityProperty

        /// <summary>
        /// DependencyProperty Registration for SearchOptionVisibility 
        /// </summary>
        /// <remarks></remarks>
        public static readonly DependencyProperty SearchOptionVisibilityProperty = DependencyProperty.Register(
           "SearchOptionVisibility", typeof(Visibility), typeof(CheckboxFilterControl), new PropertyMetadata(Visibility.Visible));

        /// <summary>
        /// Gets or sets a value indicating whether the Search option to visible or not.
        /// </summary>
        /// <value><see langword="true"/> if ; otherwise, <see langword="false"/>.</value>
        /// <remarks></remarks>
        public Visibility SearchOptionVisibility
        {
            get { return (Visibility)this.GetValue(CheckboxFilterControl.SearchOptionVisibilityProperty); }
            set { this.SetValue(CheckboxFilterControl.SearchOptionVisibilityProperty, value); }
        }

        #endregion

        #region ItemsSource
        /// <summary>
        /// DependencyProperty Registration for ItemSource of CheckboxFilterControl
        /// </summary>
        /// <remarks></remarks>
        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(
          "ItemsSource", typeof(object), typeof(CheckboxFilterControl), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets ItemsSource for CheckboxFilterControl.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public Object ItemsSource
        {
            get { return (object)this.GetValue(CheckboxFilterControl.ItemsSourceProperty); }
            set { this.SetValue(CheckboxFilterControl.ItemsSourceProperty, value); }
        }
        #endregion

        #region HasItemsSource
        /// <summary>
        /// DependencyProperty Registration for HasItemsSource of CheckboxFilterControl
        /// </summary>
        /// <remarks></remarks>
        public static readonly DependencyProperty HasItemsSourceProperty = DependencyProperty.Register(
          "HasItemsSource", typeof(bool), typeof(CheckboxFilterControl), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets HasItemsSource for CheckboxFilterControl.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public bool HasItemsSource
        {
            get { return (bool)this.GetValue(CheckboxFilterControl.HasItemsSourceProperty); }
            set { this.SetValue(CheckboxFilterControl.HasItemsSourceProperty, value); }
        }
        #endregion

        #region IsItemSourceLoaded
        /// <summary>
        /// DependencyProperty Registration for IsItemSourceLoaded of CheckboxFilterControl
        /// </summary>
        /// <remarks></remarks>
        public static readonly DependencyProperty IsItemSourceLoadedProperty = DependencyProperty.Register(
          "IsItemSourceLoaded", typeof(bool), typeof(CheckboxFilterControl), new PropertyMetadata(false));

        /// <summary>
        /// Gets or sets ItemsSource for CheckboxFilterControl.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public bool IsItemSourceLoaded
        {
            get { return (bool)this.GetValue(CheckboxFilterControl.IsItemSourceLoadedProperty); }
            set { this.SetValue(CheckboxFilterControl.IsItemSourceLoadedProperty, value); }
        }
        #endregion

        #endregion

        #region Ctor
        public CheckboxFilterControl()
        {
            this.DefaultStyleKey = typeof(CheckboxFilterControl);
            this.Loaded += OnCheckboxFilterControlLoaded;
        }

        #endregion

        #region Events

        #region SelectAllCheckBoxChecked

        public event SelectAllCheckBoxCheckedEventHandler SelectAllCheckBoxChecked;

        /// <summary>
        /// Raises the select all check box checked.
        /// </summary>
        /// <param name="FilterElements">The filter elements.</param>
        internal void RaiseSelectAllCheckBoxChecked(List<FilterElement> FilterElements)
        {
            OnSelectAllCheckBoxChecked(new SelectAllCheckBoxCheckedEventArgs() { FilterElements = FilterElements });
        }

        /// <summary>
        /// Raises the <see cref="E:SelectAllCheckBoxChecked"/> event.
        /// </summary>
        /// <param name="e">The <see cref="Syncfusion.Windows.Controls.Grid.SelectAllCheckBoxCheckedEventArgs"/> instance containing the event data.</param>
        private void OnSelectAllCheckBoxChecked(SelectAllCheckBoxCheckedEventArgs e)
        {
            if (this.SelectAllCheckBoxChecked != null)
                this.SelectAllCheckBoxChecked(this, e);
        }

        #endregion

        #region SelectAllUnCheckBoxChecked

        public event SelectAllCheckBoxUnCheckedEventHandler SelectAllUnCheckBoxChecked;

        /// <summary>
        /// Raises the select all un check box checked.
        /// </summary>
        /// <param name="FilterElements">The filter elements.</param>
        internal void RaiseSelectAllUnCheckBoxChecked(List<FilterElement> FilterElements)
        {
            OnSelectAllUnCheckBoxChecked(new SelectAllCheckBoxUnCheckedEventArgs() { FilterElements = FilterElements });
        }

        /// <summary>
        /// Raises the <see cref="E:SelectAllUnCheckBoxChecked"/> event.
        /// </summary>
        /// <param name="e">The <see cref="Syncfusion.Windows.Controls.Grid.SelectAllCheckBoxUnCheckedEventArgs"/> instance containing the event data.</param>
        private void OnSelectAllUnCheckBoxChecked(SelectAllCheckBoxUnCheckedEventArgs e)
        {
            if (this.SelectAllUnCheckBoxChecked != null)
                this.SelectAllUnCheckBoxChecked(this, e);
        }

        #endregion

        #region CheckboxFCLoaded

        void OnCheckboxFilterControlLoaded(object sender, RoutedEventArgs e)
        {
            WireEvents();
        }

        /// <summary>
        /// Delete button click
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="T:Windows.UI.Xaml.RoutedEventArgs">RoutedEventArgs</see> that contains the event data.</param>
        /// <remarks></remarks>
        void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.SearchTextBox != null)
            {
                this.SearchTextBox.ClearValue(TextBox.TextProperty);
#if WinRT
                this.SearchTextBox.Focus(FocusState.Keyboard);
#endif
            }
        }


        #region Search TextBox

        /// <summary>
        /// OnSearchTextBoxTextChanged Event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="T:Windows.UI.Xaml.Controls.TextChangedEventArgs">TextChangedEventArgs</see> that contains the event data.</param>
        /// <remarks></remarks>
#if WinRT
        async void OnSearchTextBoxTextChanged(object sender, TextChangedEventArgs e)
#else
        void OnSearchTextBoxTextChanged(object sender, TextChangedEventArgs e)
#endif
        {
            var textBox = sender as TextBox;
            var filterText = textBox.Text.ToLower();
            if (this.FilterListBoxItem == null)
                return;
#if WinRT
            await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
#endif
                searchedItems =
                    this.FilterListBoxItem.Where(input => input.FormatedString(input.ActualValue).ToLower().Contains(filterText)).ToList();
#if WinRT
            });
#endif
            ItemsSource = searchedItems;
            isSourceChangedasSearchedItems = true;
            this.SelectAllCheckBox.IsEnabled = searchedItems.Any();
            this.MaintainSelectAllCheckBox();
        }

        /// <summary>
        /// SearchTextBox_KeyDown Event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="T:Windows.UI.Xaml.Input.KeyRoutedEventArgs">KeyRoutedEventArgs</see> that contains the event data.</param>
        /// <remarks></remarks>
        void SearchTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                var textBox = sender as TextBox;
                textBox.ClearValue(TextBox.TextProperty);
                e.Handled = true;
            }
            else if (e.Key == Key.Enter)
            {
                gridFilterCtrl.InvokeFilter();
                gridFilterCtrl.IsOpen = false;
            }

        }

#if WPF
        void SearchTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            UIElement focusedelement = Keyboard.FocusedElement as UIElement;
            if (e.Key == Key.Down)
            {
                focusedelement.MoveFocus(new TraversalRequest(FocusNavigationDirection.Down));
                e.Handled = true;
            }
            else if (e.Key == Key.Up)
            {
                focusedelement.MoveFocus(new TraversalRequest(FocusNavigationDirection.Up));
                e.Handled = true;
            }
        }
#endif
        #endregion

        #region Select All State Listner

        /// <summary>
        /// Called when [select all unchecked].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void OnSelectAllUnchecked(object sender, RoutedEventArgs e)
        {
            propertyChangedFromSelectAll = true;
            this.FilterListBoxItem.ForEach(c => c.IsSelected = false);
            this.MaintainSelectAllCheckBox();
            propertyChangedFromSelectAll = false;
        }

        /// <summary>
        /// Called when [select all checked].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void OnSelectAllChecked(object sender, RoutedEventArgs e)
        {
            propertyChangedFromSelectAll = true;
            this.FilterListBoxItem.ForEach(c => c.IsSelected = true);
            this.MaintainSelectAllCheckBox();
            propertyChangedFromSelectAll = false;
        }

        #endregion
        #endregion

        #endregion

        #region Methods

        internal void MaintainSelectAllCheckBox()
        {
            var itemsSource = this.ItemsSource as IEnumerable<FilterElement>;
            if (itemsSource == null || this.SelectAllCheckBox == null)
                return;

            this.SelectAllCheckBox.IsEnabled = itemsSource.Any();

            var uncheked = itemsSource.Where(y => !y.IsSelected).ToList();
            if (uncheked.Count == 0)
            {
                this.SelectAllCheckBox.IsThreeState = false;
                this.SelectAllCheckBox.IsChecked = true;
                this.SelectAllCheckBox.IsEnabled = true;
                if (gridFilterCtrl.OkButton != null)
                    gridFilterCtrl.OkButton.IsEnabled = true;
            }
            else if (uncheked.Count == itemsSource.Count())
            {
                this.SelectAllCheckBox.IsThreeState = false;
                this.SelectAllCheckBox.IsChecked = false;
                if (gridFilterCtrl.OkButton != null && !gridFilterCtrl.IsAdvancedFilterVisible)
                    gridFilterCtrl.OkButton.IsEnabled = false;
            }
            else
            {
                this.SelectAllCheckBox.IsThreeState = true;
                if (gridFilterCtrl.OkButton != null)
                    gridFilterCtrl.OkButton.IsEnabled = true;
                this.SelectAllCheckBox.IsChecked = null;
            }

            if (gridFilterCtrl.OkButton != null && (!itemsSource.Any() || gridFilterCtrl.ImmediateUpdateColumnFilter || !this.HasItemsSource)
                                                 && !gridFilterCtrl.IsAdvancedFilterVisible)
                gridFilterCtrl.OkButton.IsEnabled = false;
        }

        internal void MaintainAPIChanges()
        {
            if (this.gridFilterCtrl.CheckboxFilterStyle != null)
                this.Style = this.gridFilterCtrl.CheckboxFilterStyle;
        }
        #endregion

        #region UIElements

        /// <summary>
        /// Gets or sets the select all check box.
        /// </summary>
        /// <value>The select all check box.</value>
        internal CheckBox SelectAllCheckBox;

        /// <summary>
        /// Gets or sets the search text box.
        /// </summary>
        /// <value>The search text box.</value>
        internal TextBox SearchTextBox;

        /// <summary>
        /// Gets or sets the ScrollViwer of ListPart Area.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        internal ScrollViewer PartScrollViewer;

        /// <summary>
        /// Gets or sets the ItemsControl.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        internal ItemsControl PART_ItemsControl;

        /// <summary>
        /// Gets or sets the delete button.
        /// </summary>
        /// <value>The cancel button.</value>
        internal Button DeleteButton;

        #endregion

        #region Wire/UnWire Events

        private void WireEvents()
        {
            if (this.SelectAllCheckBox != null)
            {
                this.SelectAllCheckBox.Checked += OnSelectAllChecked;
                this.SelectAllCheckBox.Unchecked += OnSelectAllUnchecked;
            }

            if (this.SearchTextBox != null)
            {
#if WPF
                this.SearchTextBox.PreviewKeyDown += SearchTextBox_PreviewKeyDown;
#endif
                this.SearchTextBox.KeyDown += SearchTextBox_KeyDown;
                this.SearchTextBox.TextChanged += OnSearchTextBoxTextChanged;
            }

            if (this.PART_ItemsControl != null)
                this.PART_ItemsControl.Loaded += PART_ItemsControl_Loaded;

            if (this.DeleteButton != null)
                this.DeleteButton.Click += DeleteButton_Click;
        }

        private void UnWireEvents()
        {

            if (this.SearchTextBox != null)
            {
#if WPF
                this.SearchTextBox.PreviewKeyDown -= SearchTextBox_PreviewKeyDown;
#endif
                this.SearchTextBox.KeyDown -= SearchTextBox_KeyDown;
                this.SearchTextBox.TextChanged -= OnSearchTextBoxTextChanged;
            }

            if (this.SelectAllCheckBox != null)
            {
                this.SelectAllCheckBox.Checked -= OnSelectAllChecked;
                this.SelectAllCheckBox.Unchecked -= OnSelectAllUnchecked;
            }

            if (this.PART_ItemsControl != null)
                this.PART_ItemsControl.Loaded -= PART_ItemsControl_Loaded;

            if (this.DeleteButton != null)
                this.DeleteButton.Click -= DeleteButton_Click;

        }

        /// <summary>
        /// PART_ItemsControl_Loaded event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="T:Windows.UI.Xaml.RoutedEventArgs">RoutedEventArgs</see> that contains the event data.</param>
        /// <remarks></remarks>
        void PART_ItemsControl_Loaded(object sender, RoutedEventArgs e)
        {
            var itemsControl = sender as ItemsControl;
            PartScrollViewer = GridUtil.GetChildObject<ScrollViewer>(itemsControl, "");
        }

        #endregion

        #region Overrides
#if WinRT
        protected override void OnApplyTemplate()
#else
        public override void OnApplyTemplate()
#endif
        {
            base.OnApplyTemplate();
            SelectAllCheckBox = this.GetTemplateChild("PART_CheckBox") as CheckBox;
            this.DeleteButton = this.GetTemplateChild("PART_DeleteButton") as Button;
            this.PART_ItemsControl = this.GetTemplateChild("PART_ItemsControl") as ItemsControl;
            this.SearchTextBox = this.GetTemplateChild("PART_SearchTextBox") as TextBox;

            this.MaintainSelectAllCheckBox();
        }

        public void Dispose()
        {
            UnWireEvents();
            this.Loaded -= OnCheckboxFilterControlLoaded;
            if (gridFilterCtrl != null)
                gridFilterCtrl = null;
        }

        #endregion

    }
}
