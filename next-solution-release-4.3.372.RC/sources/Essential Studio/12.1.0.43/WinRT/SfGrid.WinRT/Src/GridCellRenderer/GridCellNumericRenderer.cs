#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Globalization;
using Syncfusion.UI.Xaml.ScrollAxis;
#if !WinRT
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Syncfusion.UI.Xaml.Grid.Utility;
#endif

#if WPF
using Syncfusion.Windows.Shared;
#elif SILVERLIGHT
using Syncfusion.Windows.Tools.Controls;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Data;
using Windows.System;
using Syncfusion.UI.Xaml.Controls.Input;
#endif

namespace Syncfusion.UI.Xaml.Grid.Cells
{
#if WinRT
    using Key = VirtualKey;
    using KeyEventArgs = KeyRoutedEventArgs;
#endif
    [ClassReference(IsReviewed = false)]
#if WinRT
    public class GridCellNumericRenderer : GridVirtualizingCellRenderer<TextBlock, SfNumericTextBox>
#else
    public class GridCellNumericRenderer : GridVirtualizingCellRenderer<TextBlock, DoubleTextBox>
#endif
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
#if WinRT
        public override void OnInitializeEditElement(RowColumnIndex rowColumnIndex, SfNumericTextBox uiElement, GridColumn column, object dataContext)
#else
        public override void OnInitializeEditElement(RowColumnIndex rowColumnIndex, DoubleTextBox uiElement, GridColumn column, object dataContext)
#endif
        {
            InitializeEditUIElement(uiElement, column);
#if WinRT
            var wrapBind = new Binding { Path = new PropertyPath("TextWrapping"), Source = column };
            uiElement.SetBinding(TextBox.TextWrappingProperty, wrapBind);
            uiElement.ValueChangedMode = ValueChange.OnLostFocus;
#else
            uiElement.TextSelectionOnFocus = false;
            BindingExpression = uiElement.GetBindingExpression(DoubleTextBox.ValueProperty);
#endif
            var textPadding = new Binding { Path = new PropertyPath("Padding"), Mode = BindingMode.TwoWay, Source = column };
            uiElement.SetBinding(Control.PaddingProperty, textPadding);
            var textAlignmentBind = new Binding { Path = new PropertyPath("TextAlignment"), Mode = BindingMode.TwoWay, Source = column };
            uiElement.SetBinding(TextBox.TextAlignmentProperty, textAlignmentBind);
            uiElement.VerticalAlignment = VerticalAlignment.Stretch;
        }
        #endregion

        #region Display/Edit Value Overrides
        /// <summary>
        /// Sets the control value.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void SetControlValue(object value)
        {
            if (!HasCurrentCellState) return;
            if (IsInEditing)
#if WinRT
                ((SfNumericTextBox)CurrentCellRendererElement).Value = (double)value;
#else
                ((DoubleTextBox) CurrentCellRendererElement).Value = (double) value;
#endif
            else
                throw new Exception("Value cannot be Set for Unloaded Editor");
        }

        /// <summary>
        /// Gets the control value.
        /// </summary>
        /// <returns></returns>
        public override object GetControlValue()
        {
            if (!HasCurrentCellState) 
                return base.GetControlValue();
#if WinRT
            return CurrentCellRendererElement.GetValue(IsInEditing ? TextBox.TextProperty : TextBlock.TextProperty);
#else
            return CurrentCellRendererElement.GetValue(IsInEditing ? DoubleTextBox.ValueProperty : TextBlock.TextProperty);
#endif
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
#if WinRT
            var uiElement = ((SfNumericTextBox) sender);
            uiElement.ValueChanged += OnValueChanged;
            uiElement.Focus(FocusState.Programmatic);
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
                double value;
                double.TryParse(PreviewInputText.ToString(), out value);
                uiElement.Value = value;
                var caretIndex = uiElement.Text.IndexOf(PreviewInputText.ToString(CultureInfo.InvariantCulture), StringComparison.Ordinal);
                uiElement.Select(caretIndex + 1, 0);
            }
#else
            var uiElement = ((DoubleTextBox) sender);
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
                double value;
                double.TryParse(PreviewInputText.ToString(), out value);
                uiElement.Value = value;
                var caretIndex = (uiElement.Text).IndexOf(PreviewInputText.ToString(CultureInfo.InvariantCulture), StringComparison.Ordinal);
                uiElement.Select(caretIndex + 1, 0);
            }
#endif
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
        /// Called when [unwire edit UI element].
        /// </summary>
        /// <param name="uiElement">The UI element.</param>
#if WinRT
        protected override void OnUnwireEditUIElement(SfNumericTextBox uiElement)
#else
        protected override void OnUnwireEditUIElement(DoubleTextBox uiElement)
#endif
        {
            uiElement.ValueChanged -= OnValueChanged;
        }

        #endregion

        #region PreviewTextInput Override
        /// <summary>
        /// Called when text is entered in the Data Control
        /// </summary>
        /// <param name="e">KeyRoutedEventArgs</param>
#if WinRT
        protected override void OnPreviewTextInput(KeyEventArgs e)
        {
            if (e.Key >= Key.Number0 && e.Key <= Key.Number9)
                PreviewInputText = (e.Key - Key.Number0);
            else if (e.Key >= Key.NumberPad0 && e.Key <= Key.NumberPad9)
                PreviewInputText = (e.Key - Key.NumberPad0);
            else
                PreviewInputText = 0;
        }
#endif
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
#if WinRT
            if (!HasCurrentCellState)
                return true;
            if (!IsInEditing)
            {
                ProcessPreviewTextInput(e);
                if (!(CurrentCellRendererElement is SfNumericTextBox))
                    return true;
            }
#else
            if (!HasCurrentCellState || !IsInEditing)
                return true;
#endif
#if !WinRT
            var CurrentCellUIElement = (DoubleTextBox) CurrentCellRendererElement;
#else
            var CurrentCellUIElement = (SfNumericTextBox)CurrentCellRendererElement;
#endif
            switch (e.Key)
            {
                case Key.Escape:
                {
#if WinRT
                    CurrentCellUIElement.ClearValue(SfNumericTextBox.ValueProperty);
#else
                    CurrentCellUIElement.ClearValue(DoubleTextBox.ValueProperty);
#endif
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

#if WinRT
        private void ProcessPreviewTextInput(KeyEventArgs e)
        {
            if ((!char.IsLetterOrDigit(e.Key.ToString(), 0) || !DataGrid.AllowEditing || DataGrid.NavigationMode != NavigationMode.Cell) || CheckControlKeyPressed() || (!(e.Key >= Key.A && e.Key <= Key.Z) && !(e.Key >= Key.Number0 && e.Key <= Key.Number9) && !(e.Key >= Key.NumberPad0 && e.Key <= Key.NumberPad9)))
                return;
            if (DataGrid.SelectionController.CurrentCellManager.BeginEdit())
                PreviewTextInput(e);
        }
#endif
        #endregion

        #endregion

        #region Event Handlers

        /// <summary>
        /// Called when [text changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="TextChangedEventArgs"/> instance containing the event data.</param>
        /// <exception cref="System.NotImplementedException"></exception>
#if WinRT
        private void OnValueChanged(object sender, ValueChangedEventArgs e)
#else
        void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
#endif
        {
            base.CurrentRendererValueChanged();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Computes the Padding for the Display UIElement.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <returns></returns>
        private static Thickness ProcessUIElementPadding(GridColumn column)
        {
            var padLeft = column.Padding.Left;
            var padRight = column.Padding.Right;
            var padTop = column.Padding.Top;
            var padBotton = column.Padding.Bottom;
            var padding = column.ReadLocalValue(GridColumn.PaddingProperty);
#if WinRT
            return padding != DependencyProperty.UnsetValue
                           ? new Thickness(3 + padLeft, 1 + padTop, 3 + padRight, 6 + padBotton)
                           : new Thickness(2, 3, 2, 6);
#elif SILVERLIGHT
            return padding != DependencyProperty.UnsetValue
                           ? new Thickness(2 + padLeft, 2 + padTop, 3 + padRight, 6 + padBotton)
                           : new Thickness(2, 2, 3, 2);
#else
            return padding != DependencyProperty.UnsetValue
                           ? new Thickness(3 + padLeft, 6 + padTop, 3 + padRight, 6 + padBotton)
                           : new Thickness(3, 6, 3, 6);
#endif
        }

        /// <summary>
        /// Processes the edit binding.
        /// </summary>
        /// <param name="uiElement">The UI element.</param>
        /// <param name="column">GridColumn of the Editing Column</param>
#if WinRT
        private void InitializeEditUIElement(SfNumericTextBox uiElement, GridColumn column)
#else
        private void InitializeEditUIElement(DoubleTextBox uiElement, GridColumn column)
#endif
        {
            var numericColumn = ((GridNumericColumn)column);
#if WinRT
            var bind = new Binding { Path = new PropertyPath("FormatString"), Source = numericColumn };
            uiElement.SetBinding(SfNumericTextBox.FormatStringProperty, bind);
            uiElement.SetBinding(SfNumericTextBox.ValueProperty, numericColumn.ValueBinding);
            bind = new Binding { Path = new PropertyPath("ParsingMode"), Source = numericColumn };
            uiElement.SetBinding(SfNumericTextBox.ParsingModeProperty, bind);
            bind = new Binding { Path = new PropertyPath("BlockCharactersOnTextInput"), Source = numericColumn };
            uiElement.SetBinding(SfNumericTextBox.BlockCharactersOnTextInputProperty, bind);
            bind = new Binding { Path = new PropertyPath("AllowNullInput"), Source = numericColumn };
            uiElement.SetBinding(SfNumericTextBox.AllowNullProperty, bind);
#else
            var bind = numericColumn.ValueBinding.CreateEditBinding(numericColumn.UpdateTrigger);
            uiElement.SetBinding(DoubleTextBox.ValueProperty, bind);
            bind = new Binding { Path = new PropertyPath("AllowScrollingOnCircle"), Mode = BindingMode.TwoWay, Source = numericColumn };
            uiElement.SetBinding(EditorBase.IsScrollingOnCircleProperty, bind);
            bind = new Binding { Path = new PropertyPath("MinValue"), Mode = BindingMode.TwoWay, Source = numericColumn };
            uiElement.SetBinding(DoubleTextBox.MinValueProperty, bind);
            bind = new Binding { Path = new PropertyPath("MaxValue"), Mode = BindingMode.TwoWay, Source = numericColumn };
            uiElement.SetBinding(DoubleTextBox.MaxValueProperty, bind);
            bind = new Binding { Path = new PropertyPath("NumberDecimalDigits"), Mode = BindingMode.TwoWay, Source = numericColumn };
            uiElement.SetBinding(DoubleTextBox.NumberDecimalDigitsProperty, bind);
            bind = new Binding { Path = new PropertyPath("NumberDecimalSeparator"), Mode = BindingMode.TwoWay, Source = numericColumn };
            uiElement.SetBinding(DoubleTextBox.NumberDecimalSeparatorProperty, bind);
            bind = new Binding { Path = new PropertyPath("NumberGroupSeparator"), Mode = BindingMode.TwoWay, Source = numericColumn };
            uiElement.SetBinding(DoubleTextBox.NumberGroupSeparatorProperty, bind);
            bind = new Binding { Path = new PropertyPath("NumberGroupSizes"), Mode = BindingMode.TwoWay, Source = numericColumn };
            uiElement.SetBinding(DoubleTextBox.NumberGroupSizesProperty, bind);
            bind = new Binding { Path = new PropertyPath("AllowNullValue"), Mode = BindingMode.TwoWay, Source = numericColumn };
            uiElement.SetBinding(EditorBase.UseNullOptionProperty, bind);
            bind = new Binding { Path = new PropertyPath("MaxValidation"), Mode = BindingMode.TwoWay, Source = numericColumn };
            uiElement.SetBinding(EditorBase.MaxValidationProperty, bind);
            bind = new Binding { Path = new PropertyPath("MinValidation"), Mode = BindingMode.TwoWay, Source = numericColumn };
            uiElement.SetBinding(EditorBase.MinValidationProperty, bind);             
            bind = new Binding { Path = new PropertyPath("NullValue"), Mode = BindingMode.TwoWay, Source = numericColumn };
            uiElement.SetBinding(DoubleTextBox.NullValueProperty, bind);
            uiElement.WatermarkTextIsVisible = true;
            bind = new Binding { Path = new PropertyPath("NullText"), Mode = BindingMode.TwoWay, Source = numericColumn };
            uiElement.SetBinding(DoubleTextBox.WatermarkTextProperty, bind);
#endif
#if WPF
            bind = new Binding { Path = new PropertyPath("TextDecorations"), Mode = BindingMode.TwoWay, Source = numericColumn };
            uiElement.SetBinding(DoubleTextBox.TextDecorationsProperty, bind);
#endif
        }
        #endregion
    }
}

