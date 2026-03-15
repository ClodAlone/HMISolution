#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.UI.Xaml.Grid.Utility;
using Syncfusion.UI.Xaml.ScrollAxis;
#if WinRT
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Core;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
#else
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows;
using System.Windows.Controls.Primitives;
#endif


namespace Syncfusion.UI.Xaml.Grid.Cells
{
#if WinRT
    using Key = Windows.System.VirtualKey;
    using KeyEventArgs = KeyRoutedEventArgs;
#endif
    [ClassReference(IsReviewed = false)]
    public class GridCellCheckBoxRenderer : GridVirtualizingCellRenderer<CheckBox,CheckBox>
    {
        #region Ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="GridCellCheckBoxRenderer"/> class.
        /// </summary>
        public GridCellCheckBoxRenderer()
        {
            SupportsRenderOptimization = false;
            IsEditable = false;
        }
        #endregion

        #region Override Methods

        #region Edit Binding Overrides
        /// <summary>
        /// Method which is initialize the Renderer element Bindings with corresponding column values.
        /// </summary>
        /// <param name="rowColumnIndex">RowColumnIndex for the Renderer Element</param>
        /// <param name="uiElement">Corresponding Renderer Element</param>
        /// <param name="column">Column which is providing the information ofr Binding</param>
        /// <param name="dataContext"></param>
        /// <remarks></remarks>
        public override void OnInitializeEditElement(RowColumnIndex rowColumnIndex, CheckBox uiElement, GridColumn column, object dataContext)
        {
#if WPF
            uiElement.FocusVisualStyle = null;
#endif
#if SILVERLIGHT
            var binding = column.ValueBinding.CreateEditBinding(UpdateSourceTrigger.Default);
#elif WPF || WP
            var binding = column.ValueBinding.CreateEditBinding(column.ValueBinding.UpdateSourceTrigger);
#else 
            var binding = column.ValueBinding.CreateEditBinding();
#endif
#if !WP
            if (!column.AllowEditing || !DataGrid.IsAddNewIndex(rowColumnIndex.RowIndex))
                binding.Mode = BindingMode.OneWay;
#endif
            uiElement.SetBinding(ToggleButton.IsCheckedProperty, binding);

            var paddingBind = new Binding { Path = new PropertyPath("Padding"), Mode = BindingMode.TwoWay, Source = column };
            uiElement.SetBinding(Control.PaddingProperty, paddingBind);
            var hAlignBind = new Binding { Path = new PropertyPath("HorizontalAlignment"), Mode = BindingMode.TwoWay, Source = column };
            uiElement.SetBinding(FrameworkElement.HorizontalAlignmentProperty, hAlignBind);
            var vAlignBind = new Binding { Path = new PropertyPath("VerticalAlignment"), Mode = BindingMode.TwoWay, Source = column };
            uiElement.SetBinding(FrameworkElement.VerticalAlignmentProperty, vAlignBind);
            var threeStateBind = new Binding { Path = new PropertyPath("IsThreeState"), Mode = BindingMode.TwoWay, Source = column };
            uiElement.SetBinding(ToggleButton.IsThreeStateProperty, threeStateBind);
#if !WP
#if !WinRT
            BindingExpression = uiElement.GetBindingExpression(ToggleButton.IsCheckedProperty);
#endif
            uiElement.IsEnabled = (column.AllowEditing || DataGrid.IsAddNewIndex(rowColumnIndex.RowIndex)) && DataGrid.SelectionMode != GridSelectionMode.None;
//#elif SILVERLIGHT || WinRT
            uiElement.Tag = rowColumnIndex;
#endif
        }

        /// <summary>
        /// Method which is used to update the Renderer element Bindings with corresponding column values.
        /// </summary>
        /// <param name="cellRowcolumnIndex">Cell Row Column Index</param>
        /// <param name="element">Corresponding Renderer Element</param>
        /// <param name="column">Corresponding column for update the binding</param>
        /// <remarks></remarks>
        public override void OnUpdateEditBinding(RowColumnIndex cellRowcolumnIndex, CheckBox element, GridColumn column, object dataContext)
        {
            OnInitializeEditElement(cellRowcolumnIndex, element, column, dataContext);
        }

#if SILVERLIGHT || WinRT || WPF
        /// <summary>
        /// Initializes the cell style.
        /// </summary>
        /// <param name="cellRowColumnIndex">Index of the cell row column.</param>
        /// <param name="record">The record.</param>
        /// <param name="cell">The cell.</param>
        /// <param name="column">The column.</param>
        protected override void InitializeCellStyle(RowColumnIndex cellRowColumnIndex, object record, UIElement cell, GridColumn column)
        {
            base.InitializeCellStyle(cellRowColumnIndex, record, cell, column);
            var gridcell = cell as GridCell;
            if (gridcell == null || !(gridcell.Content is CheckBox)) return;
            var elememt = gridcell.Content as CheckBox;
            elememt.Tag = cellRowColumnIndex;
        }
#endif

        #endregion

        #region Display/Edit Value Overrides
        /// <summary>
        /// Gets the control value.
        /// </summary>
        /// <returns></returns>
        public override object GetControlValue()
        {
            return HasCurrentCellState 
                   ? CurrentCellRendererElement.GetValue(ToggleButton.IsCheckedProperty) 
                   : base.GetControlValue();
        }

        /// <summary>
        /// Sets the control value.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void SetControlValue(object value)
        {
            if (HasCurrentCellState)
                ((CheckBox) CurrentCellRendererElement).IsChecked = (bool) value;
        }

        #endregion

        #region ShouldGridTryToHandleKeyDown
        /// <summary>
        /// Shoulds the grid try automatic handle key down.
        /// </summary>
        /// <param name="e">The <see cref="Windows.UI.Core.KeyEventArgs"/> instance containing the event data.</param>
        /// <returns></returns>
        protected override bool ShouldGridTryToHandleKeyDown(KeyEventArgs e)
        {
            if (!HasCurrentCellState)
                return true;

            var CurrentCellUIElement = (CheckBox)CurrentCellRendererElement;
            switch (e.Key)
            {
#if !WP
                case Key.Up:
                case Key.Down:
                    return true;
                case Key.Space:
                {
                    if (!CurrentCellUIElement.IsEnabled) return true;
#if !WinRT
                    if(BindingExpression!=null)
                        BindingExpression.UpdateSource();
                    CurrentCellUIElement.Focus();
#else
                    CurrentCellUIElement.Focus(FocusState.Programmatic);
#endif
                    DataGrid.RaiseCurrentCellValueChangedEvent(new CurrentCellValueChangedEventArgs { RowColumnIndex = CurrentCellIndex });
                    return true;
                }
#endif
            }
            return base.ShouldGridTryToHandleKeyDown(e);
        }
        #endregion


        #region Wire/UnWire UIElements Overrides
#if !WP
        protected override void OnEditElementLoaded(object sender, RoutedEventArgs e)
        {
            ((CheckBox)sender).Click += OnCheckBoxClick;
#if WPF
            ((CheckBox)sender).PreviewMouseUp += OnMouseUp;
#endif
            base.OnEditElementLoaded(sender, e);
        }

        protected override void OnUnwireEditUIElement(CheckBox uiElement)
        {
            uiElement.Click -= OnCheckBoxClick;
#if WPF
            uiElement.PreviewMouseUp -= OnMouseUp;
#endif
            base.OnUnwireEditUIElement(uiElement);
        }
#endif
        #endregion

        #endregion

        #region EventHandlers
#if !WP

#if WPF
        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (HasCurrentCellState && !((CheckBox)sender).IsPressed && IsFocused)
            {
                ((CheckBox)sender).IsChecked = !((CheckBox)sender).IsChecked;
                var rowColumnIndex = (sender as CheckBox).Tag is RowColumnIndex ? (RowColumnIndex)(sender as CheckBox).Tag : RowColumnIndex.Empty;
                if (!rowColumnIndex.IsEmpty)
                    DataGrid.RaiseCurrentCellValueChangedEvent(new CurrentCellValueChangedEventArgs { RowColumnIndex = rowColumnIndex });
            }
        }
#endif

        /// <summary>
        /// Called when [CheckBox click].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        void OnCheckBoxClick(object sender, RoutedEventArgs e)
        {
            var rowColumnIndex = (sender as CheckBox).Tag is RowColumnIndex ? (RowColumnIndex) (sender as CheckBox).Tag : RowColumnIndex.Empty;
#if !WPF
            if (!rowColumnIndex.IsEmpty)
            {
                this.DataGrid.SelectionController.HandlePointerOperations(new GridPointerEventArgs(PointerOperation.Pressed,null), rowColumnIndex);
                this.DataGrid.SelectionController.HandlePointerOperations(new GridPointerEventArgs(PointerOperation.Released, null), rowColumnIndex);
            }
#endif
            if(HasCurrentCellState)
#if WinRT
            DataGrid.RaiseCurrentCellValueChangedEvent(new CurrentCellValueChangedEventArgs { RowColumnIndex = rowColumnIndex });
#else
            DataGrid.RaiseCurrentCellValueChangedEvent(new CurrentCellValueChangedEventArgs { RowColumnIndex = rowColumnIndex });
#endif
#if !WP && !WinRT
            if (BindingExpression != null)
                BindingExpression.UpdateSource();
#endif
        }
#endif
        #endregion
    }
}
