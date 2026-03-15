#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Windows.Controls.Grid
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Syncfusion.Windows.Shared;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Input;
    using Syncfusion.Windows.Controls.Scroll;
    using System.Windows.Media;

    /// <summary>
    /// Implements the model part of a currency edit cell.
    /// </summary>
    public class GridCellCurrencyEditCellModel : GridCellModel<GridCellCurrencyEditCellRenderer>
    {
        /// <summary>
        /// This is called from GridStyleInfo.GetText (ignoring any <see cref="GridStyleInfo.Format"/> settings).
        /// </summary>
        /// <param name="style">The <see cref="GridStyleInfo"/> object that holds cell information.</param>
        /// <param name="value">The value to convert to a string.</param>
        /// <returns>The string that represents the given value.</returns>
        public override string GetText(GridStyleInfo style, object value)
        {
            return (value != null && !(value is DBNull)) ? value.ToString() : string.Empty;
        }

        /// <summary>
        /// Returns formatted text for the currency cell.
        /// </summary>
        /// <param name="style">The <see cref="GridStyleInfo"/> object that holds cell information.</param>
        /// <param name="value">The value to format.</param>
        /// <param name="textInfo">TextInfo is a hint of who is calling, default is GridCellBaseTextInfo.DisplayText.</param>
        /// <returns>The formatted text for the given value.</returns>
        public override string GetFormattedText(GridStyleInfo style, object value, int textInfo)
        {
            decimal dValue = 0;
            string retString = string.Empty;
            var currencyeditstyle = style.CurrencyEdit;
            if (style.CellValue == null && currencyeditstyle.UseNullOption || style.CellValue == DBNull.Value && currencyeditstyle.UseNullOption)
                return currencyeditstyle.NullValue == null ? string.Empty : currencyeditstyle.NullValue.ToString();
            else if (style.CellValue != null)
            {
                double cellvalue;
                double.TryParse(style.CellValue.ToString(), out cellvalue);
                if (double.IsNaN(cellvalue))
                    return string.Empty;
            }
            
            if (value != null && value.ToString().Length > 0)
            {
                if (decimal.TryParse(value.ToString(), out dValue))
                {                   
                }
                else
                {
                    double temp = Convert.ToDouble(value.ToString());  // The value is temp converted as Raw double without any format as decimal.TryParse() can't convert the value like " 5.02323-E05" which contains E in the text. 
                    dValue = Convert.ToDecimal(temp);
                }
            }
            
            var numberFormat = style.HasNumberFormat ? style.NumberFormat : style.GetCulture(false).NumberFormat;
            string text = dValue.ToString(numberFormat);
            if (decimal.TryParse(text, NumberStyles.Currency, numberFormat, out dValue))
                retString = dValue.ToString("C", numberFormat);

            return retString;
        }


        /// <summary>
        /// Parses the display text and converts it into a cell value based on the given currency format.
        /// </summary>
        /// <param name="style">Style information for the cell.</param>
        /// <param name="text">The input text to be parsed.</param>
        /// <param name="textInfo">TextInfo is a hint of who is calling, default is GridCellBaseTextInfo.DisplayText.</param>
        /// <returns>True if value was parsed correctly and saved in style object as <see cref="GridStyleInfo.CellValue"/>; False otherwise.</returns>
        public override bool ApplyFormattedText(GridStyleInfo style, string text, int textInfo)
        {
            decimal dValue = 0;

            if (decimal.TryParse(text, NumberStyles.Currency, style.NumberFormat, out dValue))
            {
                style.CellValue = dValue;
                return true;
            }

            return base.ApplyFormattedText(style, text, textInfo);
        }
    }

    /// <summary>
    /// Implements the renderer part of a currency edit cell.
    /// </summary>
    public class GridCellCurrencyEditCellRenderer : GridVirtualizingCellRenderer<CurrencyTextBox>
    {
        int ccSelectionStart, ccSelectionLength, textSelectionStart = -1, textSelectionLength = 0;

        /// <summary>
        /// Initializes a new <see cref="GridCellCurrencyEditCellRenderer"/> object for the given cell.
        /// </summary>
        public GridCellCurrencyEditCellRenderer()
        {
            this.SupportsRenderOptimization = true;
            this.AllowRecycle = true;
            this.IsModifiable = true;
            this.IsFocusable = true;
            this.AllowKeepAliveOnlyCurrentCell = true;
        }

        protected override void OnRender(System.Windows.Media.DrawingContext dc, Syncfusion.Windows.Controls.Cells.RenderCellArgs rca, GridRenderStyleInfo style)
        {
            if (rca.CellUIElements != null)
            {
                return;
            }

            // Will only get hit if SupportsRenderOptimization is true, otherwise rca.CellUIElements is never null.
            Thickness margins = style.TextMargins.ToThickness();
            if (style.HasImageIndex)
                margins = style.AdjustImageWidthAndHeightToMargin(margins, rca.CellRect.Size);
            else
                margins = style.ErrorInfo.AdjustErrorInfoMargin(margins, rca.CellRect.Size);

            Rect textRectangle = rca.SubtractBorderMargins(rca.CellRect, margins);

            // TextBoxView always seems to have this margin and I am not able to reset the margin.
            // Therefore I am also hard-codeing it here so that TextBox behavior is properly
            // emulated.
            margins.Left = Math.Max(margins.Left, 2);
            margins.Right = Math.Max(margins.Right, 2);

            textRectangle = rca.SubtractBorderMargins(rca.CellRect, margins);
            if (textRectangle.IsEmpty)
            {
                return;
            }

            string text = string.Empty;
            if (this.IsCurrentCell(style) && this.HasControlText)
                text = this.ControlText;
            else
                text = this.GetControlText(style);

            decimal dValue = 0;

            if (style.CellValue != null && decimal.TryParse(style.CellValue.ToString(), NumberStyles.Currency, style.NumberFormat, out dValue))
            {
                if (dValue < 0)
                    style.Foreground = style.HasNegativeForeground ? style.NegativeForeground : style.Foreground;
            }
            if (style.HasMaxLength && text.Length - 1 > style.MaxLength)
            {
                text = text[0] + text.Substring(text.Length - style.MaxLength);
            }
            // Draw the formatted text string to the DrawingContext of the control.
            GridTextBoxPaint.DrawText(dc, textRectangle, text, style);
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            var dValue = this.GetDecimalValue(this.CurrentStyle);
            if (dValue != decimal.MinValue)
            {
                this.ControlValue = dValue;
            }
            else
            {
                var cellValue = this.CurrentStyle.CellValue;
                this.ControlValue = cellValue;
            }
        }

        private decimal GetDecimalValue(GridRenderStyleInfo style)
        {
            decimal dValue = decimal.MinValue;
            var value = this.GetControlValue(style);
            if (value != null && value.ToString() != string.Empty)
                decimal.TryParse(value.ToString(), out dValue);           
            return dValue;
        }

        /// <summary>
        /// Initializes the content of the currency cell
        /// using the information from the cell style (value, text,
        /// behavior etc.).
        /// </summary>
        /// <param name="uiElement">The currency text box.</param>
        /// <param name="style">The cell style info.</param>
        public override void OnInitializeContent(CurrencyTextBox uiElement, GridRenderStyleInfo style)
        {
            base.OnInitializeContent(uiElement, style);
            this.OnUnwireUIElement(uiElement);
            Thickness margins = style.TextMargins.ToThickness();
            margins.Left = Math.Max(0, margins.Left - 2);
            margins.Right = Math.Max(0, margins.Right - 2);

            if (style.HasImageIndex)
                margins = style.AdjustImageWidthAndHeightToMargin(margins, style.GridControl);
            else
                margins = style.ErrorInfo.AdjustErrorInfoMargin(margins, style.GridControl, style.CellRowColumnIndex);

            uiElement.Padding = margins;
            uiElement.BorderThickness = new Thickness(0);
            var numberFormatInfo = style.HasNumberFormat ? style.NumberFormat : style.GetCulture(false).NumberFormat;
            var currencyeditstyle = style.CurrencyEdit;
            uiElement.CurrencyDecimalDigits = numberFormatInfo.CurrencyDecimalDigits;
            uiElement.CurrencyDecimalSeparator = numberFormatInfo.CurrencyDecimalSeparator;
            uiElement.CurrencyGroupSeparator = numberFormatInfo.CurrencyGroupSeparator;
            uiElement.CurrencyGroupSizes = new System.Windows.Media.Int32Collection(numberFormatInfo.CurrencyGroupSizes.ToList()); ;
            uiElement.CurrencyPositivePattern = numberFormatInfo.CurrencyPositivePattern;
            uiElement.CurrencySymbol = numberFormatInfo.CurrencySymbol;
            uiElement.CurrencyNegativePattern = numberFormatInfo.CurrencyNegativePattern;
            if (this.GridControl.Model.Options.ShowErrorIconOnEditing)
                uiElement.Background = Brushes.Transparent;
            else
                uiElement.Background = Brushes.White;
            uiElement.Foreground = Brushes.Black;
            uiElement.PositiveForeground = Brushes.Black;
            uiElement.IsScrollingOnCircle = currencyeditstyle.HasIsScrollingOnCircle ? currencyeditstyle.IsScrollingOnCircle : uiElement.IsScrollingOnCircle;
            uiElement.UseNullOption = currencyeditstyle.UseNullOption;
            uiElement.NullValue = currencyeditstyle.NullValue;
            uiElement.MinValidation = currencyeditstyle.HasMinValidation ? currencyeditstyle.MinValidation : uiElement.MinValidation;
            uiElement.MaxValidation = currencyeditstyle.HasMaxValidation ? currencyeditstyle.MaxValidation : uiElement.MaxValidation;
            uiElement.MinValue = currencyeditstyle.HasMinValue ? currencyeditstyle.MinValue : uiElement.MinValue;
            uiElement.MaxValue = currencyeditstyle.HasMaxValue ? currencyeditstyle.MaxValue : uiElement.MaxValue;
            if (style.HasMaxLength && uiElement.Text.Length - 1 > style.MaxLength)
            {
                uiElement.Text = uiElement.Text[0] + uiElement.Text.Substring(uiElement.Text.Length - style.MaxLength);
            }
            uiElement.MaxLength = style.MaxLength;
            var dValue = this.GetDecimalValue(style);
            if (dValue != decimal.MinValue)
            {
                uiElement.Value = dValue;
            }
            else
            {
                //uiElement.UseNullOption = true;
                uiElement.Value = null;
                //uiElement.Text = string.Empty;
            }
            if (style.FlowDirection == FlowDirection.RightToLeft)
                {
                uiElement.FlowDirection = style.FlowDirection;
                uiElement.TextWrapping = TextWrapping.NoWrap;
             

                double m11 = -1;
                double m22 = 1;
                double offsetX = uiElement.Width;
                double offsetY = 0;
                uiElement.LayoutTransform = new MatrixTransform(m11, 0, 0, m22, offsetX, offsetY); //make sure this does not leak...
                }
            else
                {
                uiElement.LayoutTransform = MatrixTransform.Identity;
                }

            this.OnWireUIElement(uiElement);
        }

        protected override void ArrangeUIElement(Cells.ArrangeCellArgs aca, CurrencyTextBox uiElement, GridRenderStyleInfo style)
        {
            Thickness margins = style.TextMargins.ToThickness();
            margins = style.ErrorInfo.AdjustErrorInfoMarginOnEditing(margins, style.GridControl, style.CellRowColumnIndex);
            uiElement.Padding = margins;

            base.ArrangeUIElement(aca, uiElement, style);
        }

        protected override void OnEnteredEditMode()
        {
            this.ApplyTextBoxProperties();
            //if (CurrentCellUIElement != null)
            //{
            //    GridRenderStyleInfo style = CurrentStyle;         
            //    if (style.CellValue != null && this.ControlValue != null)
            //    {
            //        if (!(style.CellValue).Equals(this.ControlValue))
            //        {                        
            //            //this.ControlValue = style.CellValue;
            //            //this.CurrentCellUIElement.Text = style.CellValue.ToString();
            //        }
            //    }
            //}
            //else
            //{
            //    GridControl.InvalidateCell(CellRowColumnIndex);
            //}
        }

        protected override void OnActivated()
        {
            this.ApplyTextBoxProperties();
            if (this.CurrentCell.Grid.Model.Options.ExcelLikeCurrentCell)
            {
                this.RaiseBeginEdit();
            }
        }

        protected override void OnEditingComplete()
        {
            this.GridControl.InvalidateCell(this.CellRowColumnIndex);

            if (this.CurrentCellUIElement != null)
            {
                this.CurrentCellUIElement.CaretIndex = 0;
            }
        }

        protected override void OnDeactivated()
        {
            this.GridControl.InvalidateCell(this.CellRowColumnIndex);
        }

        protected override void OnGridPreviewTextInput(TextCompositionEventArgs e)
        {
            if (this.CurrentCell.IsEditing)
            {
                return;
            }

            this.CurrentCell.ScrollInView();
            this.CurrentCell.BeginEdit(true);
            if (this.CurrentCellUIElement != null)
                this.CurrentCellUIElement.Text = string.Empty;
        }



        private void ApplyTextBoxProperties()
        {
            var style = this.CurrentStyle;
            if (style != null)
            {
                var dValue = this.GetDecimalValue(style);
                if (dValue != decimal.MinValue && this.CurrentCellUIElement != null)
                {
                    this.CurrentCellUIElement.Value = dValue;
                }
            }
            if (CurrentCellUIElement != null && GridControl.Model.Options.AllowTextSelectionOnReadOnly)
                CurrentCellUIElement.IsReadOnly = style.ReadOnly;

            if (CurrentCellUIElement == null)
                this.GridControl.InvalidateCell(this.CellRowColumnIndex);
        }

        protected override string GetControlTextFromEditorCore(CurrencyTextBox uiElement)
        {
            return uiElement.Value.ToString();
        }

        /// <summary>
        /// Refreshes the current cell content.
        /// </summary>
        public override void RefreshContent()
        {
            base.RefreshContent();
            if (this.textSelectionStart != -1 && this.CurrentCellUIElement != null)
            {
                this.CurrentCellUIElement.SelectionStart = this.textSelectionStart;
                this.CurrentCellUIElement.SelectionLength = this.textSelectionLength;
            }
        }

        protected override object GetControlValueFromEditorCore(CurrencyTextBox uiElement)
        {
            return uiElement.Value;
        }

        protected override void OnSetFocus()
        {
            if ((this.GridControl.Model.Options.ActivateCurrentCellBehavior & GridCellActivateAction.SelectAll) != GridCellActivateAction.None)
            {
                if (CurrentCellUIElement != null)
                    this.CurrentCellUIElement.SelectAll();
            }
            else if(CurrentCellUIElement != null)
            {
                CurrentCellUIElement.Select(CurrentCellUIElement.Text.Length, 0);
                CurrentCellUIElement.Focus();
            }
        }

        protected override void OnWireUIElement(CurrencyTextBox uiElement)
        {
            base.OnWireUIElement(uiElement);
            uiElement.ValueChanged += new PropertyChangedCallback(this.OnCurrencyValueChanged);
            uiElement.Loaded+=new RoutedEventHandler(uiElement_Loaded);
            //uiElement.SelectionStartChanged += new PropertyChangedCallback(uiElement_SelectionStartChanged);
            //uiElement.SelectionLengthChanged += new PropertyChangedCallback(uiElement_SelectionLengthChanged);
            uiElement.AddHandler(CurrencyTextBox.PreviewKeyDownEvent, new KeyEventHandler(OnPreviewKeyDown), true);
            uiElement.AddHandler(CurrencyTextBox.KeyDownEvent, new KeyEventHandler(OnKeyDown), true);
            uiElement.AddHandler(CurrencyTextBox.MouseRightButtonUpEvent, new MouseButtonEventHandler(uiElement_MouseRightButtonUp), true);
        }

        void uiElement_Loaded(object sender, RoutedEventArgs e)
        {
            CurrencyTextBox tb = sender as CurrencyTextBox;
            if (tb.MaxLength != 0 && tb.Text.Length - 1 > tb.MaxLength)
                tb.Text = tb.Text[0] + tb.Text.Substring(tb.Text.Length - tb.MaxLength);
            if (CurrentCellUIElement != null && !this.CurrentCell.IsModified)
            {
                if ((this.GridControl.Model.Options.ActivateCurrentCellBehavior & GridCellActivateAction.SelectAll) != GridCellActivateAction.None)
                {
                    this.CurrentCellUIElement.SelectAll();
                }
                else
                {
                    CurrentCellUIElement.Select(CurrentCellUIElement.Text.Length, 0);
                    CurrentCellUIElement.Focus();
                }
            }
        }

        protected override void OnUnwireUIElement(CurrencyTextBox uiElement)
        {
            base.OnUnwireUIElement(uiElement);
            uiElement.ValueChanged -= new PropertyChangedCallback(this.OnCurrencyValueChanged);
            uiElement.Loaded -= new RoutedEventHandler(uiElement_Loaded);
            //uiElement.SelectionStartChanged -= new PropertyChangedCallback(uiElement_SelectionStartChanged);
            //uiElement.SelectionLengthChanged -= new PropertyChangedCallback(uiElement_SelectionLengthChanged);
            uiElement.RemoveHandler(CurrencyTextBox.KeyDownEvent, new KeyEventHandler(OnKeyDown));
            uiElement.RemoveHandler(CurrencyTextBox.PreviewKeyDownEvent, new KeyEventHandler(OnPreviewKeyDown));
            uiElement.RemoveHandler(CurrencyTextBox.MouseRightButtonUpEvent, new MouseButtonEventHandler(uiElement_MouseRightButtonUp));
        }

        void uiElement_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.GridControl.Model.DisableEditorsContextMenu)
            {
                e.Handled = true;
            }
        }        

        void uiElement_SelectionLengthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!this.IsInArrange)
            {
                this.textSelectionLength = ((CurrencyTextBox)d).SelectionLength;
            }
        }

        void uiElement_SelectionStartChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!this.IsInArrange)
            {
                this.textSelectionStart = ((CurrencyTextBox)d).SelectionStart;
            }
        }

        private void OnCurrencyValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var currency = (CurrencyTextBox)d;
            if (!this.IsInArrange && this.IsCurrentCell(currency) && !this.CurrentCell.IsInEndEdit)
            {
                if (currency.Value == null && !currency.UseNullOption)
                    currency.Value = 0;
                if (!this.SetControlValue(currency.Value))
                {
                    RefreshContent();
                }
            }
        }

        void OnPreviewKeyDown(object sender, KeyEventArgs args)
        {
            CurrencyTextBox textBox = (CurrencyTextBox)sender;
            switch (args.Key)
            {
                case Key.Right:
                case Key.Left:
                case Key.Down:
                case Key.Up:
                    this.ccSelectionLength = textBox.SelectionLength;
                    this.ccSelectionStart = textBox.SelectionStart;
                    break;
            }
        }

        void OnKeyDown(object sender, KeyEventArgs args)
        {
            CurrencyTextBox textBox = (CurrencyTextBox)sender;
            bool isShiftKey = (args.KeyboardDevice.Modifiers & ModifierKeys.Shift) != ModifierKeys.None;
            switch (args.Key)
            {
                case Key.Right:
                case Key.Left:
                case Key.Down:
                case Key.Up:
                    if (!isShiftKey)
                    {
                        if (ccSelectionStart == textBox.SelectionStart
                            && ccSelectionLength == textBox.SelectionLength)
                        {
                            this.GridControl.MoveCurrentCellWithArrowKey(args);
                        }
                    }
                    break;
            }
        }

        protected override bool ShouldGridTryToHandlePreviewKeyDown(KeyEventArgs e)
        {
            // return false to indicate the CurrentCellUIElement should handle the key
            // and the grid should ignore it.
            bool isControlKey = (e.KeyboardDevice.Modifiers & ModifierKeys.Control) != ModifierKeys.None;
            bool isShiftKey = (e.KeyboardDevice.Modifiers & ModifierKeys.Shift) != ModifierKeys.None;
            if (isControlKey && !this.CurrentCell.IsEditing)
            {
                return true;
            }

            switch (e.Key)
            {
                case Key.Tab:
                    return true;

                case Key.Left:
                    if (this.CurrentCell.IsEditing)
                    {
                        if (isControlKey)
                        {
                            CurrentCellUIElement.CaretIndex = 0;
                            e.Handled = true;
                            return false;
                        }
                        if (isShiftKey)
                        {
                            return false;
                        }
                        CurrencyTextBox tb = this.CurrentCellUIElement;
                        if (tb != null)
                        {
                            if (tb.CaretIndex == 0 && tb.SelectionLength == 0)
                            {
                                e.Handled = true;
                                return true;
                            }// When the entire text is selected pressing left arrow key moves the focus to the previous cell, whereas in Excel the cursor moves to the 0th index. The below code is added to move the cursor to the 0th Index
                            else if (tb.SelectionLength == tb.Text.Length)
                            {
                                tb.CaretIndex = 0;
                                e.Handled = true;
                                return false;
                            }
                            else
                                return false;
                        }       
                    }
                    return true;                    
                case Key.Right:
                    if (this.CurrentCell.IsEditing)
                    {
                        if (isControlKey)
                        {
                            CurrentCellUIElement.CaretIndex = CurrentCellUIElement.Text.Length;
                            e.Handled = true;
                            return false;
                        }
                        if (isShiftKey)
                        {
                            return false;
                        }
                        CurrencyTextBox tb = this.CurrentCellUIElement;
                        if (tb != null)
                        {
                            if (tb.CaretIndex == 0 && tb.SelectionLength == 0)
                            {
                                //e.Handled = true;
                                return false;
                            } // When the entire text is selected pressing right arrow key moves the focus to the next cell, whereas in Excel the cursor moves to the last position. The below code is added to move the cursor to the last position
                            else if (tb.SelectionLength == tb.Text.Length)
                            {
                                tb.CaretIndex = tb.Text.Length;
                                e.Handled = true;
                                return false;
                            }
                            else if (tb.CaretIndex == tb.Text.Length)
                                return true;
                            else
                                return false;
                        }                        
                    }
                    return true;                    
                case Key.End:
                case Key.Home:
                    if (this.CurrentCell.IsEditing)
                        return false;
                    else
                        return true;
                case Key.Down:
                    if (this.CurrentCell.IsEditing)
                    {
                        if (CurrentCellUIElement != null)
                        {
                            if (isControlKey)
                            {
                                CurrentCellUIElement.CaretIndex = CurrentCellUIElement.Text.Length;
                                e.Handled = true;
                                return false;
                            }
                            else if (isShiftKey)
                            {
                                CurrentCellUIElement.Select(CurrentCellUIElement.CaretIndex, CurrentCellUIElement.Text.Length);
                                e.Handled = true;
                                return false;
                            }
                            else if (CurrentCellUIElement.IsScrollingOnCircle)
                                return false;
                            else
                                return true;
                        }
                    }
                    return true;
                case Key.Up:
                         if (this.CurrentCell.IsEditing)
                    {
                        if (CurrentCellUIElement != null)
                        {
                            if (isControlKey)
                            {
                                e.Handled = true;
                                return false;
                            }
                            else if (isShiftKey)
                            {
                                e.Handled = true;
                                return false;
                            }
                            else if (CurrentCellUIElement.IsScrollingOnCircle)
                                return false;
                            else
                                return true;
                        }
                    }
                    return true;

                case Key.Delete:
                    {
                        this.CurrentCell.BeginEdit(true);
                        return false;
                    }
                case Key.Back:
                    {
                        this.CurrentCell.BeginEdit(true);
                        return false;
                    }
                case Key.Enter:
                    if (isShiftKey)
                    {
                        break;
                    }
                    else
                    {
                        if (this.CurrentStyle != null)
                        {
                            var renderer = this.CurrentCell.Renderer;
                            if (renderer != null)
                            {
                                GridDataStyleInfo sif = renderer.CurrentStyle.ModelStyle as GridDataStyleInfo;
                                if (sif != null && sif.CellIdentity.TableCellType != GridDataTableCellType.AddNewRecordCell)
                                {
                                    e.Handled = true;
                                }
                                else if (sif != null && sif.CellIdentity.TableCellType == GridDataTableCellType.AddNewRecordCell && !CurrentCell.IsEditing)
                                {
                                    e.Handled = true;
                                }
                            }
                        }
                        //if (this.CurrentCell.IsEditing)
                        //    CurrentCell.EndEdit();
                       CurrentCell.MoveRight();
                      
                        return true;
                        // break; Unreachable code
                    }
                case Key.F2:
                    {
                        //CurrentCell.BeginEdit(true);
                        CurrencyTextBox textBox = this.CurrentCellUIElement;
                        if (textBox != null && this.CurrentCell.IsEditing && textBox.SelectionLength == textBox.Text.Length)
                        {
                            textBox.CaretIndex = textBox.Text.Length;
                            e.Handled = true;
                            return false;
                        }
                    }

                    break;
            }

            return base.ShouldGridTryToHandlePreviewKeyDown(e);
        }

        /// <summary>
        /// Raises Grid Cell Click event.
        /// </summary>
        /// <param name="rowIndex">The cell row index.</param>
        /// <param name="colIndex">The cell column index.</param>
        /// <param name="e">A reference to <see cref="MouseControllerEventArgs"/>.</param>
        public override void RaiseGridCellClick(int rowIndex, int colIndex, MouseControllerEventArgs e)
        {
            if (CurrentCell.HasCurrentCellAt(rowIndex, colIndex))
            {   
                if(!CurrentCell.IsEditing && ((GridControl.Model.Options.ActivateCurrentCellBehavior & GridCellActivateAction.DblClickOnCell) == 0)
                    && (GridControl.Model.Options.ActivateCurrentCellBehavior != GridCellActivateAction.None))
                {
                    CurrentCell.BeginEdit(true);
                }
                if (this.HasCurrentCellState && this.CurrentCellUIElement != null && this.GridControl.Model.Options.ActivateCurrentCellBehavior == GridCellActivateAction.ClickOnCell)
                {
                    CurrentCellUIElement.Select(CurrentCellUIElement.Text.Length, 0);
                    CurrentCellUIElement.Focus();
                }
            }
            base.RaiseGridCellClick(rowIndex, colIndex, e);
        }

        internal override void UnRegisterUIElement(CurrencyTextBox uiElement)
        {
            uiElement.ClearValue(CurrencyTextBox.MinValueProperty);
            uiElement.ClearValue(CurrencyTextBox.MaxValueProperty);
            base.UnRegisterUIElement(uiElement);
        }
    }
}
