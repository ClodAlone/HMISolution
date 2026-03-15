#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows;
using Syncfusion.Windows.Controls.Grid;
using System.Diagnostics;
using System.Globalization;
#if !WinRT
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Syncfusion.Windows.Controls.Scroll;
using Syncfusion.Windows.Tools.Controls;
using Syncfusion.Windows.Controls.Cells;
#else
using Syncfusion.WinRT.Controls.Grid;
using Windows.UI.Xaml;
using Syncfusion.WinRT.Controls.Scroll;
using Syncfusion.WinRT.Controls.Cells;

#endif
namespace Syncfusion.Windows.Controls.Grid
{
    public class GridCellCurrencyEditCellModel : GridCellModel<GridCellCurrencyEditCellRenderer>
    {
        public GridCellCurrencyEditCellModel()
        {
        }

        public override string GetText(GridStyleInfo style, object value)
        {
#if !WinRT
            return (value != null && !(value is DBNull)) ? value.ToString() : string.Empty;
#else
            return (value != null) ? value.ToString() : string.Empty;
#endif
        }

        public override string GetFormattedText(GridStyleInfo style, object value, int textInfo)
        {
            decimal dValue = 0;
            string retString = string.Empty;
            if (value != null && value.ToString().Length > 0)
            {
                double temp = Convert.ToDouble(value.ToString());  // The value is temp converted as Raw double without any format as decimal.TryParse() can't convert the value like " 5.02323-E05" which contains E in the text. 
                dValue = Convert.ToDecimal(temp);
            }
            string text = dValue.ToString();
            var numberFormat = style.NumberFormat;

            if (decimal.TryParse(text, NumberStyles.Currency, numberFormat, out dValue))
            {
                retString = dValue.ToString("C", numberFormat);
            }

            return retString;
        }
    }

    public class GridCellCurrencyEditCellRenderer : GridVirtualizingCellRenderer<CurrencyTextBox>
    {
        public GridCellCurrencyEditCellRenderer()
        {
            this.AllowRecycle = true;
            this.IsControlTextShown = true;
            this.SupportsRenderOptimization = true;
            this.IsFocusable = true;
            this.AllowKeepAliveOnlyCurrentCell = true;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            this.ControlValue = this.GetControlValue(this.CurrentStyle);
        }

        public override void OnInitializeContent(CurrencyTextBox uiElement, GridRenderStyleInfo style)
        {
            this.OnUnwireUIElement(uiElement);
            Thickness margins = style.TextMargins.ToThickness();
            margins.Left = Math.Max(0, margins.Left - 2);
            margins.Right = Math.Max(0, margins.Right - 2);
            if (style.HasImageIndex)
            {
                margins = style.AdjustImageWidthAndHeightToMargin(margins, style.GridControl);
            }
            else
            {
                margins = style.ErrorInfo.AdjustErrorInfoMarginOnEditing(margins, style.GridControl, style.CellRowColumnIndex);
            }
            uiElement.Padding = margins;
            uiElement.BorderThickness = new Thickness(0);
            uiElement.TextAlignment = this.HorizontalAlignmentToTextAlignment(style.HorizontalAlignment);//Currency edit Text should be in right
            uiElement.UseNullOption = style.CurrencyEdit.UseNullOption;
            var tb = uiElement;
            tb.IsReadOnly = style.ReadOnly;
            var numberFormatInfo = style.NumberFormat;
            if (numberFormatInfo != null)
            {
                tb.CurrencyDecimalDigits = numberFormatInfo.CurrencyDecimalDigits;
                tb.CurrencyPositivePattern = numberFormatInfo.CurrencyPositivePattern;
                tb.CurrencySymbol = numberFormatInfo.CurrencySymbol;
                tb.CurrencyNegativePattern = numberFormatInfo.CurrencyNegativePattern;
            }

            object val = this.GetControlValue(style);
            if (val != null && !string.IsNullOrEmpty(val.ToString()))
                tb.Value = Convert.ToDecimal(val);
            else if (style.CurrencyEdit.UseNullOption)
                tb.Value = null;
            tb.MinValue = style.CurrencyEdit.MinValue;
            tb.MaxValue = style.CurrencyEdit.MaxValue;
            decimal dValue = 0;

            if (style.CellValue != null && decimal.TryParse(style.CellValue.ToString(), NumberStyles.Currency, style.NumberFormat, out dValue))
            {
                if (dValue < 0)
                {
                    style.Foreground = style.HasNegativeForeground ? style.NegativeForeground : GridStyleInfo.Default.NegativeForeground;
                }
            }
            /// This will raise the exception and henve the cell value in style will not get updated, hence commented this
            //else
            //    style.CellValue = tb.MinValue;
            uiElement.Foreground = style.Foreground;
            uiElement.Background = style.Background;

            VisualContainer.SetWantsMouseInput(uiElement, false);
            this.OnWireUIElement(uiElement);
        }

        protected override void ArrangeUIElement(ArrangeCellArgs aca, CurrencyTextBox uiElement, GridRenderStyleInfo style)
        {
            Thickness margins = style.TextMargins.ToThickness();
            if (style.HasImageIndex)
            {
                margins = style.AdjustImageWidthAndHeightToMargin(margins, style.GridControl);
            }
            else
            {
                margins = style.ErrorInfo.AdjustErrorInfoMarginOnEditing(margins, style.GridControl, style.CellRowColumnIndex);
            }
            uiElement.Padding = margins;
            base.ArrangeUIElement(aca, uiElement, style);
        }

        protected override string GetControlTextFromEditorCore(CurrencyTextBox uiElement)
        {
            return uiElement.Value.ToString();
        }

        protected override void OnWireUIElement(CurrencyTextBox uiElement)
        {
            base.OnWireUIElement(uiElement);
            uiElement.KeyDown += new KeyEventHandler(uiElement_KeyDown);
            uiElement.TextChanged += new TextChangedEventHandler(uiElement_TextChanged);
        }

        void uiElement_TextChanged(object sender, TextChangedEventArgs e)
        {
            var currencyEdit = (CurrencyTextBox)sender;
            if (!this.IsInArrange && this.IsCurrentCell(currencyEdit) && !this.CurrentCell.IsInEndEdit)
            {
                if (!this.SetControlValue(currencyEdit.Value))
                {
                    RefreshContent();
                }
            }
            if (this.GridControl.Model.Options.ActivateCurrentCellBehavior != GridCellActivateAction.SelectAll)
            {
                if (currencyEdit.SelectionLength == currencyEdit.Text.Length)
                    currencyEdit.Select(currencyEdit.Value.ToString().Length + 1, 0);
            }
        }

        void uiElement_KeyDown(object sender, KeyEventArgs e)
        {
            //Console.WriteLine("Key:" + e.Key.ToString());
            if (e.Key == Key.Enter)
            {
                CurrentCell.MoveRight();
            }
            if (e.Key == Key.Escape)
            {
                this.ActivateOptions.Element = null;
            }

        }


        private void UpdateCurrencyEdit()
        {
            if (CurrentCellUIElement != null)
            {
                GridRenderStyleInfo style = CurrentStyle;
                decimal tmpvalue;
                decimal? result = decimal.TryParse(style.CellValue.ToString(), out tmpvalue) ?
                  tmpvalue : (decimal?)null;
                CurrentCellUIElement.Value = result;
            }
            else
            {
                GridControl.InvalidateCell(CellRowColumnIndex);
            }
        }

        protected override void OnActivated()
        {
            //base.OnActivated();
            this.GridControl.InvalidateCell(this.CellRowColumnIndex);
        }

        protected override void OnDeactivated()
        {
            UpdateCurrencyEdit();
            GridControl.InvalidateCell(CellRowColumnIndex);
            if (!this.isEnteryKeyPressed)
            {
                GridControl.InvalidateVisual(true);
            }
            else
            {
                this.isEnteryKeyPressed = false;
            }
        }

        protected override void OnEnteredEditMode()
        {
            UpdateCurrencyEdit();
        }

        private bool isEnteryKeyPressed = false;
        protected override bool ShouldGridTryToHandlePreviewKeyDown(KeyEventArgs e)
        {
            // return false to indicate the CurrentCellUIElement should handle the key
            // and the grid should ignore it.
            bool isControlKey = (Keyboard.Modifiers & ModifierKeys.Control) != ModifierKeys.None;
            bool isShiftKey = (Keyboard.Modifiers & ModifierKeys.Shift) != ModifierKeys.None;
            if (isControlKey)
                return true;

            switch (e.Key)
            {
                case Key.Enter:
                    {
                        isEnteryKeyPressed = true;
                        if (this.GridControl is GridControl)
                        {
                            if (this.GridControl.Model.Options.EnterKeyBehaviour == EnterKeyBehaviour.MouseDown)
                                this.CurrentCell.MoveDown();
                            else
                                this.CurrentCell.MoveRight();
                            CurrentCell.ScrollInView();
                            e.Handled = true;
                            return true;
                        }
                        e.Handled = true;
                        this.CurrentCell.MoveRight();
                        CurrentCell.ScrollInView();
                        return true;
                    }
                    // break; Unreachable code
                case Key.Tab:
                    return true;
                case Key.Right:
                    if (this.CurrentCell.IsEditing)
                    {
                        if (isShiftKey)
                        {
                            return false;
                        }
                        TextBox tb = this.CurrentCellUIElement;
                        if (tb != null)
                        {
                            if (tb.SelectionLength == tb.Text.Length)
                                return true;
                            else
                                this.CurrentCell.MoveRight();
                            return false;
                        }
                        return false;
                    }
                    return true;
                    //e.Handled = true; Unreachable code

                case Key.Left:
                    if (this.CurrentCell.IsEditing)
                    {
                        if (isShiftKey)
                        {
                            return false;
                        }
                        TextBox tb = this.CurrentCellUIElement;
                        if (tb != null)
                        {
                            if (tb.SelectionStart == 0)
                            {
                                this.CurrentCell.MoveLeft();
                                e.Handled = true;
                                return false;
                            }
                            else

                                return false;
                        }
                        return false;
                    }
                    return true;
                    // e.Handled = true; Unreachable code

                case Key.Down:
                    if (CurrentCell.IsEditing)
                    {
                        // Move to next cell when whole text in textbox is selected.
                        TextBox tb = CurrentCellUIElement;
                        if (tb != null && tb.SelectionStart == 0 && tb.SelectionLength == tb.Text.Length)
                            return true;
                        else
                        {
                            this.CurrentCell.MoveDown();
                            return false;
                        }

#if SILVERLIGHT
                        // e.Handled = this.CurrentCell.IsEditing; Unreachable code
#endif
                        // otherwise, move caret within textbox when cell is not in edit-mode.
                        // return !CurrentCell.IsEditing; Unreachable code
                    }
                    return true;
                case Key.Up:
                    if(CurrentCell.IsEditing)
                    {
                        
                            // Move to next cell when whole text in textbox is selected.
                            TextBox tb = CurrentCellUIElement;
                            if (tb != null && tb.SelectionStart == 0 && tb.SelectionLength == tb.Text.Length)
                                return true;
                            else
                            {
                                this.CurrentCell.MoveUp();
                                return false;
                            }

#if SILVERLIGHT
                            // e.Handled = this.CurrentCell.IsEditing; Unreachable code
#endif
                        // otherwise, move caret within textbox when cell is not in edit-mode.
                        // return !CurrentCell.IsEditing; Unreachable code

                    }
                    return true;

                case Key.End:
                    return true;
                case Key.Home:
                    if (this.CurrentCell.IsEditing)
                        return false;
                    else
                        return true;
                case Key.Delete:
                    {
                        CurrentCell.BeginEdit(true);
                        return false;
                    }
            }

            return base.ShouldGridTryToHandlePreviewKeyDown(e);
        }

        protected override void OnGridPreviewTextInput(TextCompositionEventArgs e)
        {
            if (CurrentCell.IsEditing)
                return;

            CurrentCell.ScrollInView();
            CurrentCell.BeginEdit(true);
            CurrencyTextBox tb = CurrentCellUIElement;
            if (tb != null)
            {
                decimal dvalue;
                //SetControlText(e.Text);
                if (decimal.TryParse(e.Text, out dvalue))
                    tb.Value = dvalue;
                //tb.CaretIndex = tb.Text.Length;
            }
            e.Handled = true;
        }

        protected override void OnEditingComplete()
        {
            UpdateCurrencyEdit();
            GridControl.InvalidateCell(CellRowColumnIndex);
            if (this.isEnteryKeyPressed)
            {
                GridControl.InvalidateVisual(true);
            }
        }

        protected override void OnUnwireUIElement(CurrencyTextBox uiElement)
        {
            base.OnUnwireUIElement(uiElement);
            uiElement.KeyDown -= new KeyEventHandler(uiElement_KeyDown);
            uiElement.TextChanged -= new TextChangedEventHandler(uiElement_TextChanged);

        }
    }
}
