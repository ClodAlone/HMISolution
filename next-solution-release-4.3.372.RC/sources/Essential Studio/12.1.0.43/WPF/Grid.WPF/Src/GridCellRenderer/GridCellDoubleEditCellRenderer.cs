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
    using System.Windows.Media;
    using System.Windows;
    using Syncfusion.Windows.Controls.Scroll;
    using System.Windows.Input;

    /// <summary>
    /// Implements the model part of a double edit cell.
    /// </summary>
    public class GridCellDoubleEditCellModel : GridCellNumericEditCellModel<GridCellDoubleEditCellRenderer>
    {
        /// <summary>
        /// Returns formatted text for the double edit cell.
        /// </summary>
        /// <param name="style">The <see cref="GridStyleInfo"/> object that holds cell information.</param>
        /// <param name="value">The value to format.</param>
        /// <param name="textInfo">TextInfo is a hint of who is calling, default is GridCellBaseTextInfo.DisplayText.</param>
        /// <returns>The formatted text for the given value.</returns>
        protected override string GetFormattedText(string text, System.Globalization.NumberFormatInfo numberFormatInfo)
        {
            double dValue = 0.0;
            string retString = string.Empty;

            if (double.TryParse(text, NumberStyles.Any, numberFormatInfo, out dValue))
                retString = String.Format(numberFormatInfo, "{0:n}", dValue);
            else if (text.Equals(text.ToString(numberFormatInfo)))
                return text;
            return retString;
        }

        /// <summary>
        /// Parses the display text and converts it into a cell value to be stored in the style object.
        /// GridStyleInfo.NumberFormat is used for parsing the string.
        /// </summary>
        /// <param name="style">Style information for the cell.</param>
        /// <param name="text">The input text to be parsed.</param>
        /// <param name="textInfo">TextInfo is a hint of who is calling, default is GridCellBaseTextInfo.DisplayText.</param>
        /// <returns>True if value was parsed correctly and saved in style object as <see cref="GridStyleInfo.CellValue"/>; False otherwise.</returns>
        protected override object ApplyFormattedValue(GridStyleInfo style, string text)
        {
            double dValue = 0.0;
            if (double.TryParse(text, NumberStyles.Any, style.NumberFormat, out dValue))
                return dValue;
            return base.ApplyFormattedValue(style, text);
        }

        /// <summary>
        /// This is called from GridStyleInfo.GetText (ignoring any <see cref="GridStyleInfo.Format"/> settings).
        /// GridStyleInfo.NumberFormat is used for parsing the string.
        /// </summary>
        /// <param name="style">The <see cref="GridStyleInfo"/> object that holds cell information.</param>
        /// <param name="value">The value to convert to a string.</param>
        /// <returns>The string that represents the given value.</returns>
        public override string GetText(GridStyleInfo style, object value)
        {
            var numberFormat = style.HasNumberFormat ? style.NumberFormat : style.GetCulture(false).NumberFormat;
            return String.Format(numberFormat, "{0:n}", value); ;
        }
    }

    /// <summary>
    /// Implements the renderer part of a double edit cell.
    /// </summary>
    public class GridCellDoubleEditCellRenderer : GridVirtualizingCellRenderer<DoubleTextBox>
    {
        int ccSelectionStart, ccSelectionLength, textSelectionStart = -1, textSelectionLength = 0;

        /// <summary>
        /// Initializes a new <see cref="GridCellDoubleEditCellRenderer"/> object for the given cell.
        /// </summary>
        public GridCellDoubleEditCellRenderer()
        {
            this.SupportsRenderOptimization = true;
            this.AllowRecycle = true;
            this.IsControlTextShown = false;
            this.IsFocusable = true;
            this.AllowKeepAliveOnlyCurrentCell = true;
        }

        protected override void OnRender(System.Windows.Media.DrawingContext dc, Syncfusion.Windows.Controls.Cells.RenderCellArgs rca, GridRenderStyleInfo style)
        {
            if (rca.CellUIElements != null)
                return;

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
                return;

            string text = string.Empty;
            if (this.IsCurrentCell(style) && this.HasControlText)
                text = this.ControlText;
            else
                text = this.GetControlText(style);

            double dValue = 0;

            if (style.CellValue != null && double.TryParse(style.CellValue.ToString(), NumberStyles.Currency, style.NumberFormat, out dValue))
            {
                if (dValue < 0)
                    style.Foreground = style.HasNegativeForeground ? style.NegativeForeground : style.Foreground;
            }

            // Draw the formatted text string to the DrawingContext of the control.
            GridTextBoxPaint.DrawText(dc, textRectangle, text, style);
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            var dValue = this.GetDoubleValue(this.CurrentStyle);
            if (dValue != double.MinValue)
            {
                this.ControlValue = dValue;
            }
            else
            {
                var cellValue = this.CurrentStyle.CellValue;
                this.ControlValue = cellValue;
            }
        }

        private double GetDoubleValue(GridRenderStyleInfo style)
        {
            double dValue = double.MinValue;
            var value = this.GetControlValue(style);
            if (value != null && value.ToString() != string.Empty)
                double.TryParse(value.ToString(), out dValue);
            return dValue;
        }

        /// <summary>
        /// Initializes the content of the currency cell
        /// using the information from the cell style (value, text,
        /// behavior etc.).
        /// </summary>
        /// <param name="uiElement">The currency text box.</param>
        /// <param name="style">The cell style info.</param>
        public override void OnInitializeContent(DoubleTextBox uiElement, GridRenderStyleInfo style)
        {
            base.OnInitializeContent(uiElement, style);
            this.OnUnwireUIElement(uiElement);
            Thickness margins = style.TextMargins.ToThickness();
            margins.Left = Math.Max(0, margins.Left - 2);
            margins.Right = Math.Max(0, margins.Right - 2);
            if (style.HasImageIndex)
                margins = style.AdjustImageWidthAndHeightToMargin(margins, style.GridControl);
            else if(style.HasErrorInfo)
                margins = style.ErrorInfo.AdjustErrorInfoMarginOnEditing(margins, style.GridControl, style.CellRowColumnIndex);

            uiElement.Padding = margins;
            uiElement.BorderThickness = new Thickness(0);
            var dValue = this.GetDoubleValue(style);
            var doubleeditstyle = style.DoubleEdit;
            uiElement.UseNullOption = doubleeditstyle.UseNullOption;
            uiElement.MinValidation = doubleeditstyle.HasMinValidation ? doubleeditstyle.MinValidation : uiElement.MinValidation;
            uiElement.MaxValidation = doubleeditstyle.HasMaxValidation ? doubleeditstyle.MaxValidation : uiElement.MaxValidation;
            uiElement.MinValue = doubleeditstyle.HasMinValue ? doubleeditstyle.MinValue : uiElement.MinValue;
            uiElement.MaxValue = doubleeditstyle.HasMaxValue ? doubleeditstyle.MaxValue : uiElement.MaxValue;
            if (dValue != double.MinValue)
                uiElement.Value = dValue;
            else
            {
                uiElement.UseNullOption = true;
                uiElement.Value = null;
                uiElement.Text = string.Empty;
            }
            var numberFormatInfo = style.HasNumberFormat ? this.CurrentStyle.NumberFormat : this.CurrentStyle.GetCulture(false).NumberFormat;
            uiElement.NumberGroupSeparator = numberFormatInfo.NumberGroupSeparator;
            uiElement.NumberGroupSizes = new System.Windows.Media.Int32Collection(numberFormatInfo.NumberGroupSizes.ToList()); ;
            uiElement.NumberDecimalDigits = numberFormatInfo.NumberDecimalDigits;
            uiElement.NumberDecimalSeparator = numberFormatInfo.NumberDecimalSeparator;
            uiElement.TextSelectionOnFocus = false;
            if (this.GridControl.Model.Options.ShowErrorIconOnEditing)
                uiElement.Background = Brushes.Transparent;
            else
                uiElement.Background = Brushes.White;
            uiElement.Foreground = Brushes.Black;
            uiElement.PositiveForeground = Brushes.Black;
            uiElement.MaxLength = style.MaxLength;
            uiElement.NullValue = doubleeditstyle.NullValue;
            uiElement.IsScrollingOnCircle = doubleeditstyle.HasIsScrollingOnCircle ? doubleeditstyle.IsScrollingOnCircle : uiElement.IsScrollingOnCircle;                      
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

        protected override void ArrangeUIElement(Cells.ArrangeCellArgs aca, DoubleTextBox uiElement, GridRenderStyleInfo style)
        {
            base.ArrangeUIElement(aca, uiElement, style);

            // var doubleEdirStyle = style.DoubleEdit; Unused local variable

            var numberFormatInfo  = style.HasNumberFormat ? this.CurrentStyle.NumberFormat : this.CurrentStyle.GetCulture(false).NumberFormat;
            if (style.NumberFormat != null && style.NumberFormat != this.CurrentStyle.GetCulture(false).NumberFormat)
            {
                numberFormatInfo = style.NumberFormat;
            }
            else
            {
                numberFormatInfo = this.CurrentStyle.GetCulture(false).NumberFormat;
            }
            if (numberFormatInfo != null)
            {
                uiElement.NumberGroupSeparator = numberFormatInfo.NumberGroupSeparator;
                uiElement.NumberGroupSizes = new System.Windows.Media.Int32Collection(numberFormatInfo.NumberGroupSizes.ToList()); ;
                uiElement.NumberDecimalDigits = numberFormatInfo.NumberDecimalDigits;
                uiElement.NumberDecimalSeparator = numberFormatInfo.NumberDecimalSeparator;
            }

            Thickness margins = style.TextMargins.ToThickness();
            margins = style.ErrorInfo.AdjustErrorInfoMarginOnEditing(margins, style.GridControl, style.CellRowColumnIndex);
            uiElement.Padding = margins;

            //uiElement.MinValidation = doubleEdirStyle.HasMinValidation ? doubleEdirStyle.MinValidation : uiElement.MinValidation;
            //uiElement.MaxValidation = doubleEdirStyle.HasMaxValidation ? doubleEdirStyle.MaxValidation : uiElement.MaxValidation;
            //if (uiElement.MinValidation == MinValidation.OnKeyPress)
            //    uiElement.MinValue = doubleEdirStyle.HasMinValue ? doubleEdirStyle.MinValue : uiElement.MinValue;
            //if (uiElement.MaxValidation == MaxValidation.OnKeyPress)
            //    uiElement.MaxValue = doubleEdirStyle.HasMaxValue ? doubleEdirStyle.MaxValue : uiElement.MaxValue;

            
        }

        protected override void OnEnteredEditMode()
        {
            this.ApplyTextBoxProperties();
            //if (CurrentCellUIElement != null)
            //{
            //    GridRenderStyleInfo style = CurrentStyle;
            //    //var text = GetControlText(style);
            //    //if (!((object)text).Equals(this.ControlValue))
            //    if (style.CellValue != null && this.ControlValue != null)
            //    {
            //        if (!(style.CellValue).Equals(this.ControlValue))
            //        {
            //            //this.ControlValue = text;
            //            //this.ControlValue = style.CellValue;
            //            this.CurrentCellUIElement.Text = style.CellValue.ToString();
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
                this.RaiseBeginEdit();
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
                var dValue = this.GetDoubleValue(style);
                if (dValue != double.MinValue && this.CurrentCellUIElement != null)
                {
                    this.CurrentCellUIElement.Value = dValue;
                }
            }
            if (CurrentCellUIElement != null && GridControl.Model.Options.AllowTextSelectionOnReadOnly)
                CurrentCellUIElement.IsReadOnly = style.ReadOnly;

            if (CurrentCellUIElement == null)
                this.GridControl.InvalidateCell(this.CellRowColumnIndex);
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

        protected override object GetControlValueFromEditorCore(DoubleTextBox uiElement)
        {
            return uiElement.Value;
        }

        protected override string GetControlTextFromEditorCore(DoubleTextBox uiElement)
        {
            return uiElement.Value.ToString();
        }

        protected override void OnWireUIElement(DoubleTextBox uiElement)
        {
            base.OnWireUIElement(uiElement);
            uiElement.ValueChanged += new PropertyChangedCallback(this.OnDoubleValueChanged);
            uiElement.Loaded += new RoutedEventHandler(uiElement_Loaded);
            uiElement.AddHandler(DoubleTextBox.PreviewKeyDownEvent, new KeyEventHandler(OnPreviewKeyDown), true);
            uiElement.AddHandler(DoubleTextBox.KeyDownEvent, new KeyEventHandler(OnKeyDown), true);
            uiElement.AddHandler(DoubleTextBox.MouseRightButtonUpEvent, new MouseButtonEventHandler(textBox_MouseRightButtonUp), true);
        }

        void uiElement_Loaded(object sender, RoutedEventArgs e)
        {
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

        void textBox_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.GridControl.Model.DisableEditorsContextMenu)
                e.Handled = true;
        }

        protected override void OnUnwireUIElement(DoubleTextBox uiElement)
        {
            base.OnUnwireUIElement(uiElement);
            uiElement.ValueChanged -= new PropertyChangedCallback(this.OnDoubleValueChanged);
            uiElement.Loaded -= new RoutedEventHandler(uiElement_Loaded);
            uiElement.RemoveHandler(DoubleTextBox.KeyDownEvent, new KeyEventHandler(OnKeyDown));
            uiElement.RemoveHandler(DoubleTextBox.PreviewKeyDownEvent, new KeyEventHandler(OnPreviewKeyDown));
            uiElement.RemoveHandler(DoubleTextBox.MouseRightButtonUpEvent, new MouseButtonEventHandler(textBox_MouseRightButtonUp));            
        }

        private void OnDoubleValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var currency = (DoubleTextBox)d;
            if (!this.IsInArrange && this.IsCurrentCell(currency) && !this.CurrentCell.IsInEndEdit)
            {
                if (currency.Value == null && currency.UseNullOption)
                    currency.Value = currency.NullValue;
                else if (currency.Value == null)
                    currency.Value = 0;
                if (!this.SetControlValue(currency.Value))
                {
                    RefreshContent();
                }
            }
        }

        void OnPreviewKeyDown(object sender, KeyEventArgs args)
        {
            DoubleTextBox textBox = (DoubleTextBox)sender;
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
            DoubleTextBox textBox = (DoubleTextBox)sender;
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


        protected override void OnSetFocus()
        {
            if ((this.GridControl.Model.Options.ActivateCurrentCellBehavior & GridCellActivateAction.SelectAll)!= GridCellActivateAction.None)
            {
                this.CurrentCellUIElement.SelectAll();
            }
            else
            {
                CurrentCellUIElement.Select(CurrentCellUIElement.Text.Length, 0);
                CurrentCellUIElement.Focus();
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

                case Key.PageUp:
                    //To Copy-Cat Excel feature of PageUp in Edit and Non-Edit Mode
                    if (CurrentCellUIElement != null)
                    {
                        e.Handled = true;
                        return false;
                    }
                    return true;

                case Key.Left:
                    if (this.CurrentCell.IsEditing)
                    {
                        //Added to Move the Pointer to the Starting of the DoubleEdit Cell for Ctrl + Left Key
                        if (isControlKey)
                        {
                            CurrentCellUIElement.CaretIndex = 0;
                            e.Handled = true;
                            return false;
                        }
                        if (isShiftKey)
                            return false;
                        if (CurrentCellUIElement != null)
                        {
                            if (CurrentCellUIElement.CaretIndex == 0 && CurrentCellUIElement.SelectionLength == 0)
                            {
                                e.Handled = true;
                                return true;
                            }
                            // When the entire text is selected pressing left arrow key moves the focus to the previous cell, whereas in Excel the cursor moves to the 0th index. The below code is added to move the cursor to the 0th Index
                            else if (CurrentCellUIElement.SelectionLength == CurrentCellUIElement.Text.Length)
                            {
                                CurrentCellUIElement.CaretIndex = 0;
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
                        //Added to Move the Pointer to the End of the DoubleEdit Cell for Ctrl + Right
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
                        if (CurrentCellUIElement != null)
                        {
                            if (CurrentCellUIElement.CaretIndex == 0 && CurrentCellUIElement.SelectionLength == 0)
                            {
                                //e.Handled = true;
                                return false;
                            } // When the entire text is selected pressing right arrow key moves the focus to the next cell, whereas in Excel the cursor moves to the last position. The below code is added to move the cursor to the last position
                            else if (CurrentCellUIElement.SelectionLength == CurrentCellUIElement.Text.Length)
                            {
                                CurrentCellUIElement.CaretIndex = CurrentCellUIElement.Text.Length;
                                e.Handled = true;
                                return false;
                            }
                            else if (CurrentCellUIElement.CaretIndex == CurrentCellUIElement.Text.Length)
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
                    }
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
                if (!CurrentCell.IsEditing && ((GridControl.Model.Options.ActivateCurrentCellBehavior & GridCellActivateAction.DblClickOnCell) == 0)
                    && (GridControl.Model.Options.ActivateCurrentCellBehavior != GridCellActivateAction.None))
                {
                    CurrentCell.BeginEdit(true);
                }
                if (this.CurrentCellUIElement != null && this.GridControl.Model.Options.ActivateCurrentCellBehavior == GridCellActivateAction.ClickOnCell)
                {
                    CurrentCellUIElement.Select(CurrentCellUIElement.Text.Length, 0);
                    CurrentCellUIElement.Focus();
                }
            }
            base.RaiseGridCellClick(rowIndex, colIndex, e);
        }

        internal override void UnRegisterUIElement(DoubleTextBox uiElement)
        {
            uiElement.ClearValue(DoubleTextBox.MaxValueProperty);
            uiElement.ClearValue(DoubleTextBox.MinValueProperty);
            base.UnRegisterUIElement(uiElement);
        }
    }
}