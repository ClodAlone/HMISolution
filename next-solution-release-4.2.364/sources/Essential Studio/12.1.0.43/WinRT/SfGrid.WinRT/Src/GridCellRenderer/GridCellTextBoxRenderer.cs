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
#if WinRT
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Data;
#else
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
using System.Windows.Data;
#endif

namespace Syncfusion.UI.Xaml.Grid.Cells
{
#if WinRT
    using Key = Windows.System.VirtualKey;
    using KeyEventArgs = KeyRoutedEventArgs;
#endif
    [ClassReference(IsReviewed = false)]
    public class GridCellTextBoxRenderer : GridVirtualizingCellRenderer<TextBlock,TextBox>
    {
        #region Override Methods

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
            base.OnInitializeDisplayElement(rowColumnIndex, uiElement, column, dataContext);                    
            uiElement.Padding = ProcessUIElementPadding(column);
        }

        /// <summary>
        /// Called when [initialize edit element].
        /// </summary>
        /// <param name="rowColumnIndex">Index of the row column.</param>
        /// <param name="uiElement">The unique identifier element.</param>
        /// <param name="column">The column.</param>
        /// <param name="dataContext">The data context.</param>
        public override void OnInitializeEditElement(RowColumnIndex rowColumnIndex, TextBox uiElement, GridColumn column, object dataContext)
        {
            uiElement.SetBinding(TextBox.TextProperty, column.ValueBinding);
            base.OnInitializeEditElement(rowColumnIndex, uiElement, column, dataContext);
#if !WinRT
            BindingExpression = uiElement.GetBindingExpression(TextBox.TextProperty);           
#endif
            var textWrappingBinding = new Binding { Path = new PropertyPath("TextWrapping"), Mode = BindingMode.TwoWay, Source = column };
            uiElement.SetBinding(TextBox.TextWrappingProperty, textWrappingBinding);  
#if WPF
            var textDecorationsBinding = new Binding { Path = new PropertyPath("TextDecorations"), Mode = BindingMode.TwoWay, Source = column };
            uiElement.SetBinding(TextBox.TextDecorationsProperty, textDecorationsBinding);
#endif
#if WinRT
            var Bind = new Binding { Path = new PropertyPath("IsSpellCheckEnabled"), Mode = BindingMode.TwoWay, Source = column };
            uiElement.SetBinding(TextBox.IsSpellCheckEnabledProperty, Bind);
#endif
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
            return CurrentCellRendererElement.GetValue(IsInEditing ? TextBox.TextProperty : TextBlock.TextProperty);
        }

        /// <summary>
        /// Sets the control value.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void SetControlValue(object value)
        {
            if (!HasCurrentCellState)
                return;
            if (IsInEditing)
                ((TextBox) CurrentCellRendererElement).Text = (string) value;
            else
                throw new Exception("Value cannot be Set for Unloaded Editor");
        }
        #endregion

        #region Wire/UnWire UIElements Overrides
#if !WP
        protected override void OnEditElementLoaded(object sender, RoutedEventArgs e)
        {
            var uiElement = (TextBox)sender;
            uiElement.TextChanged += OnTextChanged;
#if WinRT
            uiElement.Focus(FocusState.Programmatic);
#else
            uiElement.Focus();
#endif
            if ((this.DataGrid.EditorSelectionBehavior == EditorSelectionBehavior.SelectAll || this.DataGrid.IsAddNewIndex(this.CurrentCellIndex.RowIndex)) && PreviewInputText==null)
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
                uiElement.Text = PreviewInputText;
                var caretIndex = (uiElement.Text).IndexOf(PreviewInputText.ToString());
                uiElement.Select(caretIndex + 1, 0);
            }
            PreviewInputText = null;
        }

        /// <summary>
        /// Called when [unwire edit unique identifier element].
        /// </summary>
        /// <param name="uiElement">The unique identifier element.</param>
        protected override void OnUnwireEditUIElement(TextBox uiElement)
        {
            uiElement.TextChanged -= OnTextChanged;
#if WinRT
            uiElement.KeyDown -= OnKeyDown;
#endif
        }
#endif

        #endregion

        #region PreviewTextInput Override

        /// <summary>
        /// Raises the <see cref="E:PreviewTextInput"></see>
        ///     event.
        /// </summary>
        /// <param name="e">The <see cref="KeyEventArgs"/> instance containing the event data.</param>
#if WinRT
        protected override void OnPreviewTextInput(KeyEventArgs e)
        {
            base.OnPreviewTextInput(e);
            if (e.Key >= Key.Number0 && e.Key <= Key.Number9)
                PreviewInputText = (e.Key - Key.Number0).ToString();
            else if (e.Key >= Key.NumberPad0 && e.Key <= Key.NumberPad9)
                PreviewInputText = (e.Key - Key.NumberPad0).ToString();
            else if ((e.Key >= Key.A && e.Key <= Key.Z))
                PreviewInputText = e.Key.ToString();
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
                if (!(CurrentCellRendererElement is TextBox))
                    return true;
            }
#else
            if (!HasCurrentCellState || !IsInEditing)
                return true;
#endif

            var CurrentCellUIElement = (TextBox) CurrentCellRendererElement;
            switch (e.Key)
            {
                case Key.Escape:
                {
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
#endif
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

        #region EventHandlers
#if !WP
        /// <summary>
        /// Called when [text changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="TextChangedEventArgs"/> instance containing the event data.</param>
        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            base.CurrentRendererValueChanged();
        }
#endif

#if WinRT
        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    {
                        var textbox = sender as TextBox;
                        if (textbox != null) textbox.ClearValue(TextBox.TextProperty);
                    }
                    break;
            }
        }
#endif
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
                           ? new Thickness(3 + padLeft, 3 + padTop, 5 + padRight, 5 + padBotton)
                           : new Thickness(3, 1, 6, 6);
#elif SILVERLIGHT
            return padding != DependencyProperty.UnsetValue
                           ? new Thickness(padLeft,padTop,padRight, padBotton)
                           : new Thickness(2, 2, 6, 2);
#else
            return padding != DependencyProperty.UnsetValue
                           ? new Thickness(3 + padLeft, 1 + padTop, 3 + padRight, 1 + padBotton)
                           : new Thickness(3, 1, 3, 1);
#endif
        }
        #endregion
    }
}
