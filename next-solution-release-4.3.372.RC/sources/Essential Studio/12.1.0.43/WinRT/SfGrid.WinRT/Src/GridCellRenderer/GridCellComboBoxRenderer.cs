#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using Syncfusion.UI.Xaml.ScrollAxis;
#if WinRT
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
#else
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
#endif

namespace Syncfusion.UI.Xaml.Grid.Cells
{
#if WinRT
    using Key = Windows.System.VirtualKey;
    using KeyEventArgs = KeyRoutedEventArgs;
#endif
    public class GridCellComboBoxRenderer:GridVirtualizingCellRenderer<ContentControl,ComboBox>
    {
        #region Ctor
        public Key? lastKeyPressed;
        /// <summary>
        /// Initializes a new instance of the <see cref="GridCellComboBoxRenderer"/> class.
        /// </summary>
        public GridCellComboBoxRenderer()
        {
            this.IsDropDownable = true;
        }
        #endregion

        #region Override Methods

        #region Display/Edit Binding Overrides

        /// <summary>
        /// Called when [create edit unique identifier element].
        /// </summary>
        /// <returns></returns>
        protected override ComboBox OnCreateEditUIElement()
        {
            var comboBox = base.OnCreateEditUIElement();
#if WPF
            VisualContainer.SetWantsMouseInput(comboBox,true);
#endif
            return comboBox;
        }

        /// <summary>
        /// Called when [initialize display element].
        /// </summary>
        /// <param name="rowColumnIndex">Index of the row column.</param>
        /// <param name="uiElement">The unique identifier element.</param>
        /// <param name="column">The column.</param>
        /// <param name="dataContext">The data context.</param>
        public override void OnInitializeDisplayElement(RowColumnIndex rowColumnIndex, ContentControl uiElement, GridColumn column, object dataContext)
        {
            var gridcolumn = column as GridComboBoxColumn;

            uiElement.SetBinding(ContentControl.ContentProperty, column.DisplayBinding);
#if WPF
            var textAlignmentBind = new Binding { Path = new PropertyPath("TextAlignment"), Mode = BindingMode.TwoWay, Source = column };
            uiElement.SetBinding(TextBlock.TextAlignmentProperty, textAlignmentBind);
#endif
            var textAlignment = new Binding { Path = new PropertyPath("TextAlignment"), Mode = BindingMode.OneWay, Source = column, Converter = new TextAlignmentToHorizontalAlignmentConverter() };
            uiElement.SetBinding(Control.HorizontalContentAlignmentProperty, textAlignment);
            uiElement.VerticalAlignment = VerticalAlignment.Center;          
            if (gridcolumn.ItemTemplate != null)
                uiElement.ContentTemplate = gridcolumn.ItemTemplate;
            uiElement.Margin = ProcessUIElementPadding(column);
        }

        /// <summary>
        /// Called when [initialize edit element].
        /// </summary>
        /// <param name="rowColumnIndex">Index of the row column.</param>
        /// <param name="uiElement">The unique identifier element.</param>
        /// <param name="column">The column.</param>
        /// <param name="dataContext">The data context.</param>
        public override void OnInitializeEditElement(RowColumnIndex rowColumnIndex, ComboBox uiElement, GridColumn column, object dataContext)
        {
            InitializeEditBinding(uiElement, column);                       
            var textAlignment = new Binding { Path = new PropertyPath("TextAlignment"), Mode = BindingMode.OneWay, Source = column, Converter = new TextAlignmentToHorizontalAlignmentConverter() };
            uiElement.SetBinding(Control.HorizontalContentAlignmentProperty, textAlignment);
            uiElement.HorizontalAlignment = HorizontalAlignment.Stretch;
#if SILVERLIGHT
            BindingExpression = uiElement.GetBindingExpression(Selector.SelectedValueProperty);
#endif
        }
        #endregion

        #region Display/Edit Value 
        /// <summary>
        /// Gets the control value.
        /// </summary>
        /// <returns></returns>
        public override object GetControlValue()
        {
            if (HasCurrentCellState)
                return CurrentCellRendererElement.GetValue(IsInEditing ? Selector.SelectedValueProperty : ContentControl.ContentProperty);
            return base.GetControlValue();
        }

        /// <summary>
        /// Sets the control value.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void SetControlValue(object value)
        {
            if (!HasCurrentCellState) return;
            if (IsInEditing)
                ((ComboBox) CurrentCellRendererElement).SelectedValue = value;
            else
                throw new Exception("Value cannot be Set for Unloaded Editor");
        }

        #endregion

        #region Wire/UnWire UIElements Overrides
        protected override void OnEditElementLoaded(object sender, RoutedEventArgs e)
        {
            ((ComboBox)sender).SelectionChanged += SelectionChangedinComboBox;
#if WPF                        
            ((ComboBox)sender).PreviewMouseDown += PreviewMouseDown;           
#endif
        }

        protected override void OnUnwireEditUIElement(ComboBox uiElement)
        {
            uiElement.SelectionChanged -= SelectionChangedinComboBox;
#if WPF
            uiElement.PreviewMouseDown -= PreviewMouseDown;
#endif
        }

#if SILVERLIGHT
        /// <summary>
        /// Called when [edit element lost focus].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        protected override void OnEditElementLostFocus(object sender, RoutedEventArgs e)
        {
            if (BindingExpression != null)
                BindingExpression.UpdateSource();
            base.OnEditElementLostFocus(sender, e);
        }
#endif

        #endregion

        #region ShouldGridHandleKeyDown

        /// <summary>
        /// Let Renderer decide whether the parent grid should be allowed to handle keys and prevent
        /// the key event from being handled by the visual UIElement for this renderer. If this method
        /// returns true the parent grid will handle arrow keys and set the Handled flag in the event
        /// data. Keys that the grid does not handle will be ignored and be routed to the UIElement
        /// for this renderer.
        /// </summary>
        /// <param name="e">A <see cref="KeyEventArgs" /> object.</param>
        /// <returns>
        /// True if the parent grid should be allowed to handle keys; false otherwise.
        /// </returns>
        protected override bool ShouldGridTryToHandleKeyDown(KeyEventArgs e)
        {
            lastKeyPressed = e.Key;

            if (lastKeyPressed.Value.GetHashCode() >= 44 && lastKeyPressed.Value.GetHashCode() <= 70 && HasCurrentCellState)
            {
                IsFocused = true;
                return false;
            }
            if (!HasCurrentCellState || !IsInEditing)
                return true;

#if WPF
            if ((CheckAltKeyPressed() && e.SystemKey == Key.Down) || ((CheckAltKeyPressed() && e.SystemKey == Key.Up) || e.Key == Key.F4))
#else
            if ((CheckAltKeyPressed() && e.Key == Key.Down) || ((CheckAltKeyPressed() && e.Key == Key.Up) || e.Key == Key.F4))
#endif
            {
                var comboBox = ((ComboBox)CurrentCellRendererElement);
                comboBox.IsDropDownOpen = !comboBox.IsDropDownOpen;
               
#if WinRT
                comboBox.Focus(FocusState.Programmatic);
#endif
                return false;
            }

            switch (e.Key)
            {
                case Key.End:
                case Key.Home:
                case Key.Enter:
                case Key.Down:
                case Key.Up:
                case Key.Escape:
                    return !((ComboBox)CurrentCellRendererElement).IsDropDownOpen;
            }
            return base.ShouldGridTryToHandleKeyDown(e);
        }

        #endregion

        #endregion

        #region Private Methods

        private void InitializeEditBinding(ComboBox uiElement, GridColumn column)
        {
            var comboBoxColumn = (GridComboBoxColumn) column;
            var itemsSourceBinding = new Binding { Path = new PropertyPath("ItemsSource"), Mode = BindingMode.TwoWay, Source = comboBoxColumn };
            uiElement.SetBinding(ItemsControl.ItemsSourceProperty, itemsSourceBinding);
            uiElement.SetBinding(Selector.SelectedValueProperty, comboBoxColumn.ValueBinding);
#if !SILVERLIGHT
            var displayMemberBinding = new Binding { Path = new PropertyPath("DisplayMemberPath"), Mode = BindingMode.TwoWay, Source = comboBoxColumn };
            uiElement.SetBinding(ItemsControl.DisplayMemberPathProperty, displayMemberBinding);
#endif
            var selectedValuePathBinding = new Binding { Path = new PropertyPath("SelectedValuePath"), Mode = BindingMode.TwoWay, Source = comboBoxColumn };
            uiElement.SetBinding(Selector.SelectedValuePathProperty, selectedValuePathBinding);               

#if WPF
            var staysOpenOnEditBinding = new Binding { Path = new PropertyPath("StaysOpenOnEdit"), Mode = BindingMode.TwoWay, Source = comboBoxColumn };
            uiElement.SetBinding(ComboBox.StaysOpenOnEditProperty, staysOpenOnEditBinding);
            var isEditableBinding = new Binding { Path = new PropertyPath("IsEditable"), Mode = BindingMode.TwoWay, Source = comboBoxColumn };
            uiElement.SetBinding(ComboBox.IsEditableProperty, isEditableBinding);                       
            var itemTemplateBinding = new Binding { Path = new PropertyPath("ItemTemplate"), Mode = BindingMode.TwoWay, Source = comboBoxColumn };
            uiElement.SetBinding(ComboBox.ItemTemplateProperty, itemTemplateBinding); 
#endif
#if SILVERLIGHT || WinRT
            var itemTemplateBinding = new Binding { Path = new PropertyPath("ItemTemplate"), Mode = BindingMode.TwoWay, Source = comboBoxColumn };
            uiElement.SetBinding(ComboBox.ItemTemplateProperty, itemTemplateBinding);
#endif
        }
        private static Thickness ProcessUIElementPadding(GridColumn column)
        {
            var padLeft = column.Padding.Left;
            var padRight = column.Padding.Right;
            var padTop = column.Padding.Top;
            var padBotton = column.Padding.Bottom;
            var padding = column.ReadLocalValue(GridColumn.PaddingProperty);
#if WinRT
            return padding != DependencyProperty.UnsetValue
                           ? new Thickness(7.5 + padLeft, 0 + padTop, 7.5 + padRight, 0 + padBotton)
                           : new Thickness(7.5, 0,7.5,0);
#elif SILVERLIGHT
            return padding != DependencyProperty.UnsetValue
                           ? new Thickness(2 + padLeft, 0 + padTop, 2+ padRight, 0 + padBotton)
                           : new Thickness(2, 0, 2, 0);
#else
            return padding != DependencyProperty.UnsetValue
                           ? new Thickness(1 + padLeft, 0 + padTop, 1 + padRight, 0 + padBotton)
                           : new Thickness(1,0,1,0);
#endif
        }

#if WPF
        void PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var combobox = sender as ComboBox;
            if (combobox.IsMouseCaptured && combobox.IsKeyboardFocused)
            {
                if (!this.DataGrid.Validations.CheckForValidation(false))
                {
                    combobox.ReleaseMouseCapture();
                    e.Handled = true;
                }
            }
        }
#endif
#if !SILVERLIGHT
        protected override void OnEnteredEditMode(UIElement currentRendererElement)
        {
            base.OnEnteredEditMode(currentRendererElement);                       
        }
#endif       

#if WinRT
        private void SelectionChangedinComboBox(object sender, Windows.UI.Xaml.Controls.SelectionChangedEventArgs e)
#else
        private void SelectionChangedinComboBox(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
#endif
        {
            var comboBox = (ComboBox) sender;
            DataGrid.RaiseCurrentCellDropDownSelectionChangedEvent(new CurrentCellDropDownSelectionChangedEventArgs
            {
                SelectedIndex = comboBox.SelectedIndex, 
                SelectedItem = comboBox.SelectedItem, 
                RowColumnIndex = CurrentCellIndex
            });
        }
        #endregion
    }
}
