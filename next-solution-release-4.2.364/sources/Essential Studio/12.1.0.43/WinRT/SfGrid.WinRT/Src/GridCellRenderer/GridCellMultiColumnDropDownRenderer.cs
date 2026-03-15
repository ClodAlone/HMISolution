#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
#if WinRT
using Syncfusion.UI.Xaml.ScrollAxis;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
#else
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Syncfusion.UI.Xaml.ScrollAxis;
using System.Windows.Data;
#endif

namespace Syncfusion.UI.Xaml.Grid.Cells
{
#if WinRT
    using Key = Windows.System.VirtualKey;
    using KeyEventArgs = KeyRoutedEventArgs;
#endif
    public class GridCellMultiColumnDropDownRenderer:GridVirtualizingCellRenderer<TextBlock, SfMultiColumnDropDownControl>
    {
        #region Ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="GridCellComboBoxRenderer"/> class.
        /// </summary>
        public GridCellMultiColumnDropDownRenderer()
        {
            this.IsDropDownable = true;
        }
        #endregion
        #region Override Methods
        /// <summary>
        /// Called when [create edit unique identifier element].
        /// </summary>
        /// <returns></returns>
        protected override SfMultiColumnDropDownControl OnCreateEditUIElement()
        {
            var sfMultiColumnDropDownControl = base.OnCreateEditUIElement();
#if WPF
            VisualContainer.SetWantsMouseInput(sfMultiColumnDropDownControl, true);
#endif
            return sfMultiColumnDropDownControl;
        }             

        #region Display/Edit Binding Overrides
        /// <summary>
        /// Called when [initialize display element].
        /// </summary>
        /// <param name="rowColumnIndex">Index of the row column.</param>
        /// <param name="uiElement">The unique identifier element.</param>
        /// <param name="column">The column.</param>
        /// <param name="dataContext">The data context.</param>
        public override void OnInitializeDisplayElement(RowColumnIndex rowColumnIndex, TextBlock uiElement, GridColumn column, object dataContext)
        {
            uiElement.SetBinding(TextBlock.TextProperty, column.DisplayBinding);
            uiElement.Padding = ProcessUIElementPadding(column);
            base.OnInitializeDisplayElement(rowColumnIndex, uiElement, column, dataContext);
        }
        /// <summary>
        /// Called when [initialize edit element].
        /// </summary>
        /// <param name="rowColumnIndex">Index of the row column.</param>
        /// <param name="uiElement">The unique identifier element.</param>
        /// <param name="column">The column.</param>
        /// <param name="dataContext">The data context.</param>
        public override void OnInitializeEditElement(RowColumnIndex rowColumnIndex, SfMultiColumnDropDownControl uiElement, GridColumn column, object dataContext)
        {           
            InitializeEditBinding(uiElement,column);

            uiElement.SetBinding(SfMultiColumnDropDownControl.SelectedValueProperty, column.ValueBinding);
            var Bind = new Binding { Path = new PropertyPath("DisplayMember"), Mode = BindingMode.TwoWay, Source = column };
            uiElement.SetBinding(SfMultiColumnDropDownControl.DisplayMemberProperty, Bind);            
            Bind = new Binding { Path = new PropertyPath("ValueMember"), Mode = BindingMode.TwoWay, Source = column };
            uiElement.SetBinding(SfMultiColumnDropDownControl.ValueMemberProperty, Bind);
            Bind = new Binding { Path = new PropertyPath("ItemsSource"), Mode = BindingMode.TwoWay, Source = column };
            uiElement.SetBinding(SfMultiColumnDropDownControl.ItemsSourceProperty, Bind);
            Bind = new Binding { Path = new PropertyPath("PopUpWidth"), Mode = BindingMode.TwoWay, Source = column };
            uiElement.SetBinding(SfMultiColumnDropDownControl.PopupWidthProperty, Bind);
            Bind = new Binding { Path = new PropertyPath("PopUpHeight"), Mode = BindingMode.TwoWay, Source = column };
            uiElement.SetBinding(SfMultiColumnDropDownControl.PopupHeightProperty, Bind);
            Bind = new Binding { Path = new PropertyPath("AllowAutoComplete"), Mode = BindingMode.TwoWay, Source = column };
            uiElement.SetBinding(SfMultiColumnDropDownControl.AllowAutoCompleteProperty, Bind);
            Bind = new Binding { Path = new PropertyPath("AllowSpinOnMouseWheel"), Mode = BindingMode.TwoWay, Source = column };
            uiElement.SetBinding(SfMultiColumnDropDownControl.AllowSpinOnMouseWheelProperty, Bind);
            Bind = new Binding { Path = new PropertyPath("AllowIncrementalFiltering"), Mode = BindingMode.TwoWay, Source = column };
            uiElement.SetBinding(SfMultiColumnDropDownControl.AllowIncrementalFilteringProperty, Bind);
            Bind = new Binding { Path = new PropertyPath("AllowCasingforFilter"), Mode = BindingMode.TwoWay, Source = column };
            uiElement.SetBinding(SfMultiColumnDropDownControl.AllowCaseSensitiveFilteringProperty, Bind);
            Bind = new Binding { Path = new PropertyPath("PopUpMinHeight"), Mode = BindingMode.TwoWay, Source = column };
            uiElement.SetBinding(SfMultiColumnDropDownControl.PopupMinHeightProperty, Bind);
            Bind = new Binding { Path = new PropertyPath("PopUpMaxHeight"), Mode = BindingMode.TwoWay, Source = column };
            uiElement.SetBinding(SfMultiColumnDropDownControl.PopupMaxHeightProperty, Bind);
            Bind = new Binding { Path = new PropertyPath("PopUpMaxWidth"), Mode = BindingMode.TwoWay, Source = column };
            uiElement.SetBinding(SfMultiColumnDropDownControl.PopupMaxWidthProperty, Bind);
            Bind = new Binding { Path = new PropertyPath("IsTextReadOnly"), Mode = BindingMode.TwoWay, Source = column };
            uiElement.SetBinding(SfMultiColumnDropDownControl.ReadOnlyProperty, Bind);
            Bind = new Binding { Path = new PropertyPath("AllowNullInput"), Mode = BindingMode.TwoWay, Source = column };
            uiElement.SetBinding(SfMultiColumnDropDownControl.AllowNullInputProperty, Bind);
            Bind = new Binding { Path = new PropertyPath("AutoGenerateColumns"), Mode = BindingMode.TwoWay, Source = column };
            uiElement.SetBinding(SfMultiColumnDropDownControl.AutoGenerateColumnsProperty, Bind);
            Bind = new Binding { Path = new PropertyPath("Columns"), Mode = BindingMode.TwoWay, Source = column };
            uiElement.SetBinding(SfMultiColumnDropDownControl.ColumnsProperty, Bind);
            Bind = new Binding { Path = new PropertyPath("GridColumnSizer"), Mode = BindingMode.TwoWay, Source = column };
            uiElement.SetBinding(SfMultiColumnDropDownControl.GridColumnSizerProperty, Bind);
            Bind = new Binding { Path = new PropertyPath("IsAutoPopupSize"), Mode = BindingMode.TwoWay, Source = column };
            uiElement.SetBinding(SfMultiColumnDropDownControl.IsAutoPopupSizeProperty, Bind);
            Bind = new Binding { Path = new PropertyPath("AutoGenerateColumnsMode"), Mode = BindingMode.TwoWay, Source = column };
            uiElement.SetBinding(SfMultiColumnDropDownControl.AutoGenerateColumnsModeProperty, Bind);           
#if SILVERLIGHT || WPF 
            BindingExpression = uiElement.GetBindingExpression(SfMultiColumnDropDownControl.SelectedValueProperty);
#endif
        }
        #endregion

        private static Thickness ProcessUIElementPadding(GridColumn column)
        {
            var padLeft = column.Padding.Left;
            var padRight = column.Padding.Right;
            var padTop = column.Padding.Top;
            var padBotton = column.Padding.Bottom;
            var padding = column.ReadLocalValue(GridColumn.PaddingProperty);
#if WinRT
            return padding != DependencyProperty.UnsetValue
                           ? new Thickness(3 + padLeft, 3 + padTop, 5 + padRight, 5 + padBotton)
                           : new Thickness(0, 0,0,0);
#elif SILVERLIGHT
            return padding != DependencyProperty.UnsetValue
                           ? new Thickness(4 + padLeft, 3 + padTop, 5 + padRight, 5 + padBotton)
                           : new Thickness(2, 2, 6, 6);
#else
            return padding != DependencyProperty.UnsetValue
                           ? new Thickness(3 + padLeft, 1 + padTop, 3 + padRight, 1 + padBotton)
                           : new Thickness(3,0,3,0);
#endif
        }
        #region Display/ Edit Value Overrides
        /// <summary>
        /// Gets the control value.
        /// </summary>
        /// <returns></returns>
        public override object GetControlValue()
        {
            if (!HasCurrentCellState) return base.GetControlValue();            
            return CurrentCellRendererElement.GetValue(IsInEditing ? SfMultiColumnDropDownControl.SelectedValueProperty : TextBlock.TextProperty);
        }

        /// <summary>
        /// Sets the control value.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void SetControlValue(object value)
        {
            if (!HasCurrentCellState) return;
            if (IsInEditing)
                ((SfMultiColumnDropDownControl)CurrentCellRendererElement).SelectedItem = value;
            else
                throw new Exception("Value cannot be Set for Unloaded Editor");
        }
        #endregion

        #region Wire/UnWire UIElements Overrides
        protected override void OnEditElementLoaded(object sender, RoutedEventArgs e)
        {
            var uiElement = ((SfMultiColumnDropDownControl)sender);
            uiElement.SelectionChanged += uiElement_SelectionChanged;
#if WinRT
            uiElement.Editor.Focus(FocusState.Programmatic);
#else
            uiElement.Editor.Focus();  
#endif
            if ((DataGrid.EditorSelectionBehavior == EditorSelectionBehavior.SelectAll || DataGrid.IsAddNewIndex(CurrentCellIndex.RowIndex)) && PreviewInputText == null)
            {
                uiElement.Editor.SelectAll();
            }
             else
            {
                var index = uiElement.Editor.Text.Length;
                uiElement.Editor.Select(index + 1, 0);
                return;
           }
            PreviewInputText = null;            
#if WPF
            ((SfMultiColumnDropDownControl)sender).PreviewMouseDown += PreviewMouseDown;     
#endif            
        }

#if SILVERLIGHT
        protected override void OnEditElementLostFocus(object sender, RoutedEventArgs e)
        {
            if (BindingExpression != null)
                BindingExpression.UpdateSource();
            base.OnEditElementLostFocus(sender, e);
        }
#endif
        
        /// <summary>
        /// Called when [unwire edit unique identifier element].
        /// </summary>
        /// <param name="uiElement">The unique identifier element.</param>
        protected override void OnUnwireEditUIElement(SfMultiColumnDropDownControl uiElement)
        {
            uiElement.SelectionChanged -= uiElement_SelectionChanged;
#if WPF
            uiElement.PreviewMouseDown -= PreviewMouseDown;          
#endif
        }
        #endregion

        #endregion
        #region Private Methods

        private void InitializeEditBinding(SfMultiColumnDropDownControl uiElement, GridColumn column)
        {
            var gridMultiColumnDropDownList = (GridMultiColumnDropDownList)column;
#if !SILVERLIGHT
            var itemsSourceBinding = new Binding { Path = new PropertyPath("ItemsSource"), Mode = BindingMode.TwoWay, Source = gridMultiColumnDropDownList };
            uiElement.SetBinding(ItemsControl.ItemsSourceProperty, itemsSourceBinding);
#endif
           uiElement.SetBinding(SfMultiColumnDropDownControl.SelectedValueProperty, gridMultiColumnDropDownList.ValueBinding);
            var displayMemberBinding = new Binding { Path = new PropertyPath("DisplayMember"), Mode = BindingMode.TwoWay, Source = gridMultiColumnDropDownList };
            uiElement.SetBinding(SfMultiColumnDropDownControl.DisplayMemberProperty, displayMemberBinding);            
        }
        #endregion
#if WPF
        void PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var sfMultiColumnDropDownControl = sender as SfMultiColumnDropDownControl;
            if (sfMultiColumnDropDownControl.IsMouseCaptured && sfMultiColumnDropDownControl.IsKeyboardFocused)
            {
                if (!this.DataGrid.Validations.CheckForValidation(false))
                {
                    sfMultiColumnDropDownControl.ReleaseMouseCapture();
                    e.Handled = true;
                }
            }
        }
#endif
        protected override bool ShouldGridTryToHandleKeyDown(KeyEventArgs e)
        {

#if !WinRT
            if (!HasCurrentCellState || !IsInEditing)
                return true;
#endif        
#if WinRT
            UIElement CurrentCellUIElement;

            if (CurrentCellRendererElement is SfMultiColumnDropDownControl)
                CurrentCellUIElement = ((SfMultiColumnDropDownControl)CurrentCellRendererElement).Editor;
            else
                CurrentCellUIElement = (TextBlock)CurrentCellRendererElement;
#else
            var CurrentCellUIElement = ((SfMultiColumnDropDownControl)CurrentCellRendererElement).Editor;
#endif

            switch (e.Key)
            {
                case Key.Escape:
                    {
                        if(CurrentCellRendererElement is SfMultiColumnDropDownControl)
                        CurrentCellUIElement.ClearValue(TextBox.TextProperty);      
        
                        return true;
                    }
#if WPF
                case Key.Left:
                    return (CurrentCellUIElement.CaretIndex <= 0 && !CheckControlKeyPressed() && !CheckShiftKeyPressed());
                case Key.Right:
                    return (CurrentCellUIElement.CaretIndex >= CurrentCellUIElement.Text.Length && !CheckControlKeyPressed() && !CheckShiftKeyPressed());
                case Key.Home:
                    return (CurrentCellUIElement.CaretIndex == 0 && !CheckControlKeyPressed() && !CheckShiftKeyPressed());
                case Key.End:
                    return (CurrentCellUIElement.CaretIndex == CurrentCellUIElement.Text.Length && !CheckControlKeyPressed() && !CheckShiftKeyPressed());
                case Key.Up:
                case Key.Down:
                    return false;
#endif
            }
            return base.ShouldGridTryToHandleKeyDown(e);
        }
#if !SILVERLIGHT
        protected override void OnEnteredEditMode(UIElement currentRendererElement)
        {
#if WPF
            VisualContainer.SetWantsMouseInput(currentRendererElement, false);
#endif
            base.OnEnteredEditMode(currentRendererElement);
        }
#endif   

        #region Private Methods
        /// <summary>
        /// Handles the SelectionChanged event of the uiElement control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void uiElement_SelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            DataGrid.RaiseCurrentCellDropDownSelectionChangedEvent(
                new CurrentCellDropDownSelectionChangedEventArgs
                {
                    RowColumnIndex = CurrentCellIndex, 
                    SelectedIndex = args.SelectedIndex, 
                    SelectedItem = args.SelectedItem
                });
        }
        #endregion
    }
}
