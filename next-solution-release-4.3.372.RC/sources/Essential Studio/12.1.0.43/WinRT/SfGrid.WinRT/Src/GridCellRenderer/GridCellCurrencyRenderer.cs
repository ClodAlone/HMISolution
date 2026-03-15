#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Syncfusion.UI.Xaml.Grid.Utility;
using Syncfusion.UI.Xaml.ScrollAxis;

#if WPF
using Syncfusion.Windows.Shared;
#elif SILVERLIGHT
using Syncfusion.Windows.Tools.Controls;
#endif

namespace Syncfusion.UI.Xaml.Grid.Cells
{
    [ClassReference(IsReviewed = false)]
    public class GridCellCurrencyRenderer : GridVirtualizingCellRenderer<TextBlock, CurrencyTextBox>
    {

        #region Override Methods

        #region Display/Edit Binding Overrides
        /// <summary>
        /// Called when [initialize display element].
        /// </summary>
        /// <param name="rowColumnIndex">Index of the row column.</param>
        /// <param name="uiElement">The UI element.</param>
        /// <param name="column">The column.</param>
        /// <param name="dataContext">The data context.</param>
        public override void OnInitializeDisplayElement(RowColumnIndex rowColumnIndex, TextBlock uiElement, GridColumn column, object dataContext)
        {
            base.OnInitializeDisplayElement(rowColumnIndex, uiElement, column, dataContext);
            //Note: Padding cannot be binded for Display UI Element, since the padding will be irregular to match Edit/Display UI Elements
            //The Column will be refreshed via Dependency CallBack.
            uiElement.Padding = ProcessUIElementPadding(column);
        }

        /// <summary>
        /// Called when [initialize edit element].
        /// </summary>
        /// <param name="rowColumnIndex">Index of the row column.</param>
        /// <param name="uiElement">The UI element.</param>
        /// <param name="column">The column.</param>
        /// <param name="dataContext">The data context.</param>
        public override void OnInitializeEditElement(RowColumnIndex rowColumnIndex, CurrencyTextBox uiElement, GridColumn column, object dataContext)
        {
            InitializeEditUIElement(uiElement, column);
            BindingExpression = uiElement.GetBindingExpression(CurrencyTextBox.ValueProperty);
            base.OnInitializeEditElement(rowColumnIndex, uiElement, column, dataContext);
        }

        #endregion

        #region Display/Edit Value Overrides
        /// <summary>
        /// Gets the control value.
        /// </summary>
        /// <returns></returns>
        public override object GetControlValue()
        {
            if (!HasCurrentCellState)
                return base.GetControlValue();
            return CurrentCellRendererElement.GetValue(IsInEditing ? CurrencyTextBox.ValueProperty : TextBlock.TextProperty);
        }
        /// <summary>
        /// Sets the control value.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void SetControlValue(object value)
        {
            if (!HasCurrentCellState) return;
            if (IsInEditing)
                ((CurrencyTextBox) CurrentCellRendererElement).Value = (decimal?) value;
            else
                throw new InvalidOperationException("Value cannot be Set for Unloaded Editor");
        }

        #endregion

        #region Wire/UnWire UIElements Overrides

        /// <summary>
        /// Called when [edit element loaded].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        protected override void OnEditElementLoaded(object sender, RoutedEventArgs e)
        {
            var uiElement = (CurrencyTextBox) sender;
            uiElement.ValueChanged += OnValueChanged;
            uiElement.Focus();
            if ((this.DataGrid.EditorSelectionBehavior == EditorSelectionBehavior.SelectAll || this.DataGrid.IsAddNewIndex(this.CurrentCellIndex.RowIndex)) && PreviewInputText == null)
            {
                uiElement.SelectAll();
            }
            else
            {
                if (PreviewInputText == null)
                {
                    var index = uiElement.Text.Length;
                    uiElement.Select(index + 1, 0);
                    return;
                }
                decimal value;
                decimal.TryParse(PreviewInputText.ToString(), out value);
                uiElement.Value = value;
                var caretIndex = (uiElement.Text).IndexOf(PreviewInputText.ToString(CultureInfo.InvariantCulture), StringComparison.Ordinal);
                uiElement.Select(caretIndex + 1, 0);
            }
            PreviewInputText = null;
        }

#if SILVERLIGHT
        /// <summary>
        /// Called when [edit element lost focus].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        protected override void OnEditElementLostFocus(object sender, RoutedEventArgs e)
        {
            BindingExpression.UpdateSource();
            base.OnEditElementLostFocus(sender, e);
        }
#endif

        /// <summary>
        /// Called when [unwire edit unique identifier element].
        /// </summary>
        /// <param name="uiElement">The unique identifier element.</param>
        protected override void OnUnwireEditUIElement(CurrencyTextBox uiElement)
        {
            uiElement.ValueChanged -= OnValueChanged;
        }

        #endregion

        #region ShouldGridTryToHandleKeyDown
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
            if (!HasCurrentCellState || !IsInEditing)
                return true;

            var CurrentCellUIElement = (CurrencyTextBox) CurrentCellRendererElement;
            switch (e.Key)
            {
                case Key.Escape:
                {
                    CurrentCellUIElement.ClearValue(CurrencyTextBox.ValueProperty);
                    return true;
                }
                case Key.Left:
                    return (CurrentCellUIElement.SelectionStart <= 0 && !CheckControlKeyPressed() && !CheckShiftKeyPressed());
                case Key.Right:
                    return (CurrentCellUIElement.SelectionStart >= CurrentCellUIElement.Text.Length && !CheckControlKeyPressed() && !CheckShiftKeyPressed());
                case Key.Home:
                    return (CurrentCellUIElement.SelectionStart == 0 && !CheckControlKeyPressed() && !CheckShiftKeyPressed());
                case Key.End:
                    return (CurrentCellUIElement.SelectionStart == CurrentCellUIElement.Text.Length && !CheckControlKeyPressed() && !CheckShiftKeyPressed());
            }
            return base.ShouldGridTryToHandleKeyDown(e);
        }

        #endregion

        #endregion

        #region Event Handlers
        /// <summary>
        /// Called when [text changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="TextChangedEventArgs"/> instance containing the event data.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        private void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            base.CurrentRendererValueChanged();
        }

        #endregion

        #region Private Methods
        /// <summary>
        /// Computes the Padding for the Display UIElement.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <remarks>Padding Cannot be applied at RunTime, as Currently We dont haven't constructed Padding via Binding Expression</remarks>
        /// <returns></returns>
        private static Thickness ProcessUIElementPadding(GridColumn column)
        {
            var padLeft = column.Padding.Left;
            var padRight = column.Padding.Right;
            var padTop = column.Padding.Top;
            var padBotton = column.Padding.Bottom;
            var padding = column.ReadLocalValue(GridColumn.PaddingProperty);
#if WPF
            return padding != DependencyProperty.UnsetValue
                           ? new Thickness(3 + padLeft, 2 + padTop, 3 + padRight, 5 + padBotton)
                           : new Thickness(3, 5, 3, 6);
#else
            return padding != DependencyProperty.UnsetValue
                           ? new Thickness(2 + padLeft, 2 +padTop, 3 + padRight, 2+ padBotton)
                           : new Thickness(2, 2, 3, 2);
#endif
        }

        /// <summary>
        /// Processes the edit binding.
        /// </summary>
        /// <param name="uiElement">The UI element.</param>
        /// <param name="gridColumn">Currency Column</param>
        private void InitializeEditUIElement(EditorBase uiElement, GridColumn gridColumn)
        {
            uiElement.TextSelectionOnFocus = false;
            var currencyColumn = (GridCurrencyColumn) gridColumn;
            var bind = currencyColumn.ValueBinding.CreateEditBinding(currencyColumn.UpdateTrigger);
            uiElement.SetBinding(CurrencyTextBox.ValueProperty, bind);
            bind = new Binding { Path = new PropertyPath("AllowScrollingOnCircle"), Mode = BindingMode.TwoWay, Source = currencyColumn };
            uiElement.SetBinding(EditorBase.IsScrollingOnCircleProperty, bind);
            bind = new Binding { Path = new PropertyPath("MinValue"), Mode = BindingMode.TwoWay, Source = currencyColumn };
            uiElement.SetBinding(CurrencyTextBox.MinValueProperty, bind);
            bind = new Binding { Path = new PropertyPath("MaxValue"), Mode = BindingMode.TwoWay, Source = currencyColumn };
            uiElement.SetBinding(CurrencyTextBox.MaxValueProperty, bind);
            bind = new Binding { Path = new PropertyPath("CurrencyDecimalDigits"), Mode = BindingMode.TwoWay, Source = currencyColumn };
            uiElement.SetBinding(CurrencyTextBox.CurrencyDecimalDigitsProperty, bind);
            bind = new Binding { Path = new PropertyPath("CurrencyGroupSeparator"), Mode = BindingMode.TwoWay, Source = currencyColumn };
            uiElement.SetBinding(CurrencyTextBox.CurrencyGroupSeparatorProperty, bind);
            bind = new Binding { Path = new PropertyPath("CurrencySymbol"), Mode = BindingMode.TwoWay, Source = currencyColumn };
            uiElement.SetBinding(CurrencyTextBox.CurrencySymbolProperty, bind);
            bind = new Binding { Path = new PropertyPath("CurrencyDecimalSeparator"), Mode = BindingMode.TwoWay, Source = currencyColumn };
            uiElement.SetBinding(CurrencyTextBox.CurrencyDecimalSeparatorProperty, bind);
            bind = new Binding { Path = new PropertyPath("CurrencyGroupSizes"), Mode = BindingMode.TwoWay, Source = currencyColumn };
            uiElement.SetBinding(CurrencyTextBox.CurrencyGroupSizesProperty, bind);
            bind = new Binding { Path = new PropertyPath("CurrencyPositivePattern"), Mode = BindingMode.TwoWay, Source = currencyColumn };
            uiElement.SetBinding(CurrencyTextBox.CurrencyPositivePatternProperty, bind);
            bind = new Binding { Path = new PropertyPath("CurrencyNegativePattern"), Mode = BindingMode.TwoWay, Source = currencyColumn };
            uiElement.SetBinding(CurrencyTextBox.CurrencyNegativePatternProperty, bind);
            bind = new Binding { Path = new PropertyPath("MaxValidation"), Mode = BindingMode.TwoWay, Source = currencyColumn };
            uiElement.SetBinding(EditorBase.MaxValidationProperty, bind);
            bind = new Binding { Path = new PropertyPath("MinValidation"), Mode = BindingMode.TwoWay, Source = currencyColumn };
            uiElement.SetBinding(EditorBase.MinValidationProperty, bind);
            bind = new Binding { Path = new PropertyPath("AllowNullValue"), Mode = BindingMode.TwoWay, Source = currencyColumn };
            uiElement.SetBinding(EditorBase.UseNullOptionProperty, bind);
            bind = new Binding { Path = new PropertyPath("NullValue"), Mode = BindingMode.TwoWay, Source = currencyColumn };
            uiElement.SetBinding(CurrencyTextBox.NullValueProperty, bind);
            uiElement.WatermarkTextIsVisible = true;
            bind = new Binding { Path = new PropertyPath("NullText"), Mode = BindingMode.TwoWay, Source = currencyColumn };
            uiElement.SetBinding(CurrencyTextBox.WatermarkTextProperty, bind);
#if WPF
            bind = new Binding { Path = new PropertyPath("TextDecorations"), Mode = BindingMode.TwoWay, Source = currencyColumn };
            uiElement.SetBinding(CurrencyTextBox.TextDecorationsProperty, bind);            
#endif
        }
        #endregion
    }
}
