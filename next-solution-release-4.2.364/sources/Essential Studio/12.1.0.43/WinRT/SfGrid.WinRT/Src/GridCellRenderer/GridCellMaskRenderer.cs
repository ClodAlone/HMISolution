#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Syncfusion.UI.Xaml.Grid.Utility;
using Syncfusion.UI.Xaml.ScrollAxis;

#if WPF
using Syncfusion.Windows.Shared;
#else
using Syncfusion.Windows.Tools.Controls;
#endif

namespace Syncfusion.UI.Xaml.Grid.Cells
{
    [ClassReference(IsReviewed = false)]
    public class GridCellMaskRenderer:GridVirtualizingCellRenderer<TextBlock, MaskedTextBox>
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
        public override void OnInitializeEditElement(RowColumnIndex rowColumnIndex, MaskedTextBox uiElement, GridColumn column, object dataContext)
        {
            InitializeEditUIElement(uiElement, column);
            BindingExpression = uiElement.GetBindingExpression(MaskedTextBox.ValueProperty);
            var textPadding = new Binding { Path = new PropertyPath("Padding"), Mode = BindingMode.TwoWay, Source = column };
            uiElement.SetBinding(Control.PaddingProperty, textPadding);
            var textAlignBind = new Binding { Path = new PropertyPath("TextAlignment"), Mode = BindingMode.TwoWay, Source = column };
            uiElement.SetBinding(TextBox.TextAlignmentProperty, textAlignBind);
            var textWrapBinding = new Binding { Path = new PropertyPath("TextWrapping"), Mode = BindingMode.TwoWay, Source = column };
            uiElement.SetBinding(TextBox.TextWrappingProperty, textWrapBinding);
            uiElement.VerticalAlignment = VerticalAlignment.Stretch;
        }
        #endregion

        #region Display/ Edit Value Overrides
        /// <summary>
        /// Sets the control value.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void SetControlValue(object value)
        {
            if (!HasCurrentCellState) return;
            if (IsInEditing)
                ((MaskedTextBox) CurrentCellRendererElement).Value = (string) value;
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
            return CurrentCellRendererElement.GetValue(IsInEditing ? MaskedTextBox.ValueProperty : TextBlock.TextProperty);
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
            ((MaskedTextBox)sender).TextChanged += OnTextChanged;
            ((MaskedTextBox) sender).Focus();
            ProcessCaretIndex(sender as UIElement);
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
        protected override void OnUnwireEditUIElement(MaskedTextBox uiElement)
        {
            uiElement.TextChanged -= OnTextChanged;
        }
        #endregion

        #region PreviewTextInput Override
        /// <summary>
        /// Called when text is entered in the Data Control
        /// </summary>
        /// <param name="e">KeyRoutedEventArgs</param>
        protected override void OnPreviewTextInput(TextCompositionEventArgs e)
        {
            if (!HasCurrentCellState)
                return;
            PreviewInputText = e.Text;
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
            if (!HasCurrentCellState ||  !IsInEditing)
                return true;
            var currentCellUiElement = (MaskedTextBox) CurrentCellRendererElement;
            switch (e.Key)
            {
                case Key.Escape:
                {
                    currentCellUiElement.ClearValue(MaskedTextBox.ValueProperty);
                    return true;
                }
                case Key.F2:
                {
                    if (!IsInEditing)
                        DataGrid.Focus();
                    return true;
                }
#if WPF
                case Key.A:
                    return CheckControlKeyPressed() && !IsInEditing;
                case Key.Left:
                    return (currentCellUiElement.CaretIndex <= 0 && !CheckControlKeyPressed() && !CheckShiftKeyPressed());
                case Key.Right:
                    return (currentCellUiElement.CaretIndex >= currentCellUiElement.Text.Length && !CheckControlKeyPressed() && !CheckShiftKeyPressed());
                case Key.Home:
                    return (currentCellUiElement.CaretIndex == 0 && !CheckControlKeyPressed() && !CheckShiftKeyPressed());
                case Key.End:
                    return (currentCellUiElement.CaretIndex == currentCellUiElement.Text.Length && !CheckControlKeyPressed() && !CheckShiftKeyPressed());
#endif
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
        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            base.CurrentRendererValueChanged();
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// Processes the edit binding.
        /// </summary>
        /// <param name="uiElement">The UI element.</param>
        /// <param name="column">Grid Column</param>
        private void InitializeEditUIElement(MaskedTextBox uiElement, GridColumn column)
        {
            var maskedColumn = ((GridMaskColumn)column);
            var bind = maskedColumn.ValueBinding.CreateEditBinding(maskedColumn.UpdateTrigger);
            uiElement.SetBinding(MaskedTextBox.ValueProperty, bind);
            bind = new Binding { Path = new PropertyPath("SelectTextOnFocus"), Mode = BindingMode.TwoWay, Source = maskedColumn };
            uiElement.SetBinding(MaskedTextBox.TextSelectionOnFocusProperty, bind);
			bind = new Binding { Path = new PropertyPath("IsNumeric"), Mode = BindingMode.TwoWay, Source = maskedColumn };
            uiElement.SetBinding(MaskedTextBox.IsNumericProperty, bind);
            bind = new Binding { Path = new PropertyPath("DateSeparator"), Mode = BindingMode.TwoWay, Source = maskedColumn };
            uiElement.SetBinding(MaskedTextBox.DateSeparatorProperty, bind);
            bind = new Binding { Path = new PropertyPath("DecimalSeparator"), Mode = BindingMode.TwoWay, Source = maskedColumn };
            uiElement.SetBinding(MaskedTextBox.DecimalSeparatorProperty, bind);           
            bind = new Binding { Path = new PropertyPath("TimeSeparator"), Mode = BindingMode.TwoWay, Source = maskedColumn };
            uiElement.SetBinding(MaskedTextBox.TimeSeparatorProperty, bind);                           
            bind = new Binding { Path = new PropertyPath("PromptChar"), Mode = BindingMode.TwoWay, Source = maskedColumn };
            uiElement.SetBinding(MaskedTextBox.PromptCharProperty, bind);
            bind = new Binding { Path = new PropertyPath("Mask"), Mode = BindingMode.TwoWay, Source = maskedColumn };
            uiElement.SetBinding(MaskedTextBox.MaskProperty, bind);
            bind = new Binding { Path = new PropertyPath("MaskFormat"), Mode = BindingMode.TwoWay, Source = maskedColumn };
            uiElement.SetBinding(MaskedTextBox.TextMaskFormatProperty, bind);
            uiElement.WatermarkTextIsVisible = false;
#if WPF
            bind = new Binding { Path = new PropertyPath("TextDecorations"), Mode = BindingMode.TwoWay, Source = maskedColumn };
            uiElement.SetBinding(MaskedTextBox.TextDecorationsProperty, bind);            
#endif     
        }

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
#if WPF
            return padding != DependencyProperty.UnsetValue
                           ? new Thickness(13 + padLeft, 7 + padTop, 13 + padRight, 5 + padBotton)
                           : new Thickness(3, 7, 3, 8);
#else
            return padding != DependencyProperty.UnsetValue
                           ? new Thickness(10 + padLeft, 3 + padTop, 10 + padRight, 5 + padBotton)
                           : new Thickness(2, 3, 2, 2);
#endif
        }

        /// <summary>
        /// Processes the index of the caret.
        /// </summary>
        /// <param name="uiElement">The unique identifier element.</param>
        private void ProcessCaretIndex(UIElement uiElement)
        {
            var editor = uiElement as MaskedTextBox;
            if (editor != null)
            {
                if (this.DataGrid.EditorSelectionBehavior == EditorSelectionBehavior.SelectAll || this.DataGrid.IsAddNewIndex(this.CurrentCellIndex.RowIndex))
                {
                    editor.SelectAll();
                }
                else
                {
                    if (string.Equals(PreviewInputText, string.Empty) || PreviewInputText == null)
                    {
                        var caretIndex = editor.Text.Length;
                        editor.Select(caretIndex, 0);
                    }
                }
            }
            PreviewInputText = string.Empty;
        }
        #endregion
    }
}
