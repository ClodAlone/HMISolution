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
    /// Implements the model part of an integer edit cell.
    /// </summary>
    public class GridCellIntegerEditCellModel : GridCellNumericEditCellModel<GridCellIntegerEditCellRenderer>
    {
        protected string GetNumber(string text, NumberFormatInfo numberFormat)
        {
            string retString = String.Empty;
            int len = text.Length;

            if (text.Contains("."))
                len = text.IndexOf(".");

            for (int i = 0; i < len; i++)
            {
                char c = text[i];

                if (Char.IsDigit(c) || numberFormat.NegativeSign.Contains(c.ToString()))
                    retString += c;
            }

            return retString;
        }

        /// <summary>
        /// Return formatted text for the specified value.
        /// </summary>
        /// <param name="style">The <see cref="GridStyleInfo"/> object that holds cell information.</param>
        /// <param name="value">The value to format.</param>
        /// <param name="textInfo">TextInfo is a hint of who is calling, default is GridCellBaseTextInfo.DisplayText.</param>
        /// <returns>The formatted text for the given value.</returns>
        protected override string GetFormattedText(string text, System.Globalization.NumberFormatInfo numberFormatInfo)
        {
            //This wont be invoked. Because we have handled GetFormattedText(stye, value, textInfo)
            // Integer text should not have decimal digits
            var numberFormatInfoCopy = numberFormatInfo.Clone() as System.Globalization.NumberFormatInfo;
            numberFormatInfoCopy.NumberDecimalDigits = 0;
            text = this.GetNumber(text, numberFormatInfoCopy);
            Int64 preValue;
            string retString = string.Empty;
            if (Int64.TryParse(text, out preValue))
                return retString = preValue.ToString("N", numberFormatInfoCopy);

            return "0";
        }

        public override string GetFormattedText(GridStyleInfo style, object value, int textInfo)
        {
            string text = this.GetText(style, value);
            //var formattedText = base.GetFormattedText(style, value, textInfo);
            var numberFormat = style.HasNumberFormat ? style.NumberFormat : style.GetCulture(false).NumberFormat;
            if (style.NumberFormat != null && style.NumberFormat != style.GetCulture(false).NumberFormat)
                numberFormat = style.NumberFormat;
            else
                numberFormat = style.GetCulture(false).NumberFormat;
            //var formattedText = this.GetFormattedText(text, numberFormat);
            //return formattedText;

            var numberFormatInfoCopy = numberFormat.Clone() as System.Globalization.NumberFormatInfo;
            numberFormatInfoCopy.NumberDecimalDigits = 0;

            if (style.IntegerEdit != null)
                numberFormatInfoCopy.NumberGroupSeparator = style.IntegerEdit.HasGroupSeperatorEnabled? style.IntegerEdit.GroupSeperatorEnabled ? numberFormat.NumberGroupSeparator : string.Empty : numberFormat.NumberGroupSeparator;

            text = this.GetNumber(text, numberFormatInfoCopy);
            Int64 preValue;
            string retString = string.Empty;
            if (Int64.TryParse(text, out preValue))
                return retString = preValue.ToString("N", numberFormatInfoCopy);
            if (style.IntegerEdit.UseNullOption)
                return retString;

            return "0";
        }

        /// <summary>
        /// Parses the display text and converts it into a cell value to be stored in the style object.
        /// </summary>
        /// <param name="style">Style information for the cell.</param>
        /// <param name="text">The input text to be parsed.</param>
        /// <returns>True if value was parsed correctly and saved in style object as <see cref="GridStyleInfo.CellValue"/>; False otherwise.</returns>
        protected override object ApplyFormattedValue(GridStyleInfo style, string text)
        {
            // Integer text should not have decimal digits
            if (style.NumberFormat!=null)
                style.NumberFormat.NumberDecimalDigits = 0;                
            text = this.GetNumber(text, style.NumberFormat);
            Int64 preValue = Int64.MinValue;
            if (Int64.TryParse(text, out preValue))
                return preValue;
            return base.ApplyFormattedValue(style, text);
        }
    }

    /// <summary>
    /// Implements the renderer part an integer edit cell.
    /// </summary>
    public class GridCellIntegerEditCellRenderer : GridVirtualizingCellRenderer<IntegerTextBox>
    {
        int ccSelectionStart, ccSelectionLength, textSelectionStart = -1, textSelectionLength = 0;

        public GridCellIntegerEditCellRenderer()
        {
            this.SupportsRenderOptimization = true;
            this.AllowRecycle = true;
            this.IsControlTextShown = true;
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

            if (this.IsNegativeValue(style.CellValue))
                style.Foreground = style.HasNegativeForeground ? style.NegativeForeground : style.Foreground;

            // Draw the formatted text string to the DrawingContext of the control.
            GridTextBoxPaint.DrawText(dc, textRectangle, text, style);
        }


        protected virtual bool IsNegativeValue(object cellValue)
        {
            Int64 preValue = Int64.MinValue;
            if (cellValue != null && Int64.TryParse(cellValue.ToString(), out preValue))
            {
                if (preValue < 0)
                    return true;
            }

            return false;
        }


        public override void OnInitializeContent(IntegerTextBox uiElement, GridRenderStyleInfo style)
        {
            base.OnInitializeContent(uiElement, style);
            OnUnwireUIElement(uiElement);
            Thickness margins = style.TextMargins.ToThickness();
            margins.Left = Math.Max(0, margins.Left - 2);
            margins.Right = Math.Max(0, margins.Right - 2);
            if (style.HasImageIndex)
                margins = style.AdjustImageWidthAndHeightToMargin(margins, style.GridControl);
            else
                margins = style.ErrorInfo.AdjustErrorInfoMargin(margins, style.GridControl, style.CellRowColumnIndex);

            uiElement.Padding = margins;
            uiElement.BorderThickness = new Thickness(0);
            var integerEditStyle = style.IntegerEdit;
            uiElement.IsScrollingOnCircle = integerEditStyle.HasIsScrollingOnCircle ? integerEditStyle.IsScrollingOnCircle : uiElement.IsScrollingOnCircle;
            var numberFormatInfo = style.HasNumberFormat ? style.NumberFormat : style.GetCulture(false).NumberFormat;
            uiElement.GroupSeperatorEnabled = integerEditStyle.HasGroupSeperatorEnabled? integerEditStyle.GroupSeperatorEnabled : uiElement.GroupSeperatorEnabled;
            uiElement.NumberGroupSeparator = integerEditStyle.HasGroupSeperatorEnabled ? integerEditStyle.GroupSeperatorEnabled ? numberFormatInfo.NumberGroupSeparator : string.Empty : numberFormatInfo.NumberGroupSeparator;
            uiElement.NumberGroupSizes = new System.Windows.Media.Int32Collection(numberFormatInfo.NumberGroupSizes.ToList());
            uiElement.Background = Brushes.White;
            uiElement.Foreground = Brushes.Black;
            uiElement.PositiveForeground = Brushes.Black;
            uiElement.MaxLength = style.MaxLength;
            var integereditstyle = style.IntegerEdit;
            uiElement.IsScrollingOnCircle = integereditstyle.HasIsScrollingOnCircle ? integereditstyle.IsScrollingOnCircle : uiElement.IsScrollingOnCircle;
            uiElement.EnableFocusColors = false;
            uiElement.MinValidation = integereditstyle.HasMinValidation ? integereditstyle.MinValidation : uiElement.MinValidation;
            uiElement.MaxValidation = integereditstyle.HasMaxValidation ? integereditstyle.MaxValidation : uiElement.MaxValidation;
            uiElement.UseNullOption = integereditstyle.UseNullOption;
            uiElement.NullValue = integereditstyle.NullValue;
            uiElement.MinValue = integereditstyle.HasMinValue ? integereditstyle.MinValue : uiElement.MinValue;
            uiElement.MaxValue = integereditstyle.HasMaxValue ? integereditstyle.MaxValue : uiElement.MaxValue;
            var dValue = this.GetIntegerValue(style);
            if (dValue != long.MinValue)
                uiElement.Value = dValue;
            else
            {
                uiElement.UseNullOption = true;
                uiElement.Value = null;
                uiElement.Text = string.Empty;
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
                uiElement.LayoutTransform = MatrixTransform.Identity;
            OnWireUIElement(uiElement);
        }

        protected override void ArrangeUIElement(Cells.ArrangeCellArgs aca, IntegerTextBox uiElement, GridRenderStyleInfo style)
        {
            Thickness margins = style.TextMargins.ToThickness();
            margins = style.ErrorInfo.AdjustErrorInfoMarginOnEditing(margins, style.GridControl, style.CellRowColumnIndex);
            uiElement.Padding = margins;

            base.ArrangeUIElement(aca, uiElement, style);

            var integerEdirStyle = style.IntegerEdit;

            var numberFormatInfo = style.HasNumberFormat ? style.NumberFormat : style.GetCulture(false).NumberFormat;
            uiElement.NumberGroupSeparator = integerEdirStyle.HasGroupSeperatorEnabled? integerEdirStyle.GroupSeperatorEnabled ? numberFormatInfo.NumberGroupSeparator : string.Empty : numberFormatInfo.NumberGroupSeparator;
            uiElement.NumberGroupSizes = new System.Windows.Media.Int32Collection(numberFormatInfo.NumberGroupSizes.ToList());
            uiElement.EnableFocusColors = false;
            uiElement.IsScrollingOnCircle = integerEdirStyle.HasIsScrollingOnCircle ? integerEdirStyle.IsScrollingOnCircle : uiElement.IsScrollingOnCircle;
            //uiElement.MinValidation = integerEdirStyle.HasMinValidation ? integerEdirStyle.MinValidation : uiElement.MinValidation;
            //uiElement.MaxValidation = integerEdirStyle.HasMaxValidation ? integerEdirStyle.MaxValidation : uiElement.MaxValidation;
            //if (uiElement.MinValidation == MinValidation.OnKeyPress)
            //    uiElement.MinValue = integerEdirStyle.HasMinValue ? integerEdirStyle.MinValue : uiElement.MinValue;
            //if (uiElement.MaxValidation == MaxValidation.OnKeyPress)
            //    uiElement.MaxValue = integerEdirStyle.HasMaxValue ? integerEdirStyle.MaxValue : uiElement.MaxValue;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            var dValue = this.GetIntegerValue(this.CurrentStyle);
            if (dValue != long.MinValue)
                this.ControlValue = dValue;
            else
            {
                var cellValue = this.CurrentStyle.CellValue;
                this.ControlValue = cellValue;
            }
        }

        protected override void OnEditingComplete()
        {
            this.GridControl.InvalidateCell(this.CellRowColumnIndex);

            if (this.CurrentCellUIElement != null)
                this.CurrentCellUIElement.CaretIndex = 0;
        }


        protected override void OnEnteredEditMode()
        {
            this.ApplyIntegerTextBoxProperties();
        }

        protected override void OnActivated()
        {
            this.ApplyIntegerTextBoxProperties();
        }

        protected virtual void ApplyIntegerTextBoxProperties()
        {
            var style = this.CurrentStyle;
            if (style != null)
            {
                var dValue = this.GetIntegerValue(style);
                if (dValue != long.MinValue && this.CurrentCellUIElement != null)
                    this.CurrentCellUIElement.Value = dValue;
            }
            if (CurrentCellUIElement != null && GridControl.Model.Options.AllowTextSelectionOnReadOnly)
                CurrentCellUIElement.IsReadOnly = style.ReadOnly;
            if (CurrentCellUIElement == null)
                this.GridControl.InvalidateCell(this.CellRowColumnIndex);
        }

        protected override void OnDeactivated()
        {
            this.GridControl.InvalidateCell(this.CellRowColumnIndex);
        }

        private long GetIntegerValue(GridStyleInfo style)
        {
            long dValue = long.MinValue;
            var value = this.GetControlValue(this.CurrentStyle);
            if (value != null && value.ToString() != string.Empty)
                long.TryParse(value.ToString(), out dValue);
            return dValue;
        }


        protected override string GetControlTextFromEditorCore(IntegerTextBox uiElement)
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

        protected override void OnWireUIElement(IntegerTextBox uiElement)
        {
            base.OnWireUIElement(uiElement);
            uiElement.ValueChanged += new PropertyChangedCallback(uiElement_ValueChanged);
            uiElement.Loaded += new RoutedEventHandler(uiElement_Loaded);
            //uiElement.SelectionStartChanged += new PropertyChangedCallback(uiElement_SelectionStartChanged);
            //uiElement.SelectionLengthChanged += new PropertyChangedCallback(uiElement_SelectionLengthChanged);
            uiElement.AddHandler(IntegerTextBox.PreviewKeyDownEvent, new KeyEventHandler(OnPreviewKeyDown), true);
            uiElement.AddHandler(IntegerTextBox.KeyDownEvent, new KeyEventHandler(OnKeyDown), true);
            uiElement.AddHandler(IntegerTextBox.MouseRightButtonUpEvent, new MouseButtonEventHandler(uiElement_MouseRightButtonUp), true);
        }

        void uiElement_Loaded(object sender, RoutedEventArgs e)
        {
            if (CurrentCellUIElement != null && !this.CurrentCell.IsModified)
                if ((this.GridControl.Model.Options.ActivateCurrentCellBehavior & GridCellActivateAction.SelectAll) != GridCellActivateAction.None)
                    this.CurrentCellUIElement.SelectAll();
                else
                {
                    CurrentCellUIElement.Select(CurrentCellUIElement.Text.Length, 0);
                    CurrentCellUIElement.Focus();
                }
        }

        void uiElement_ValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var integerEdit = (IntegerTextBox)d;
            if (!this.IsInArrange && this.IsCurrentCell(integerEdit) && !this.CurrentCell.IsInEndEdit)
            {
                if (integerEdit.Value == null && integerEdit.UseNullOption)
                    integerEdit.Value = integerEdit.NullValue;
                else if (integerEdit.Value == null)
                    integerEdit.Value = 0;
                if (!this.SetControlValue(integerEdit.Value))
                    RefreshContent();
            }
        }

        //void uiElement_SelectionLengthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    if (!this.IsInArrange)
        //    {
        //        this.textSelectionLength = ((IntegerTextBox)d).SelectionLength;
        //    }
        //}

        //void uiElement_SelectionStartChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    if (!this.IsInArrange)
        //    {
        //        this.textSelectionStart = ((IntegerTextBox)d).SelectionStart;
        //    }
        //}


        protected override void OnUnwireUIElement(IntegerTextBox uiElement)
        {
            base.OnUnwireUIElement(uiElement);
            uiElement.ValueChanged -= new PropertyChangedCallback(uiElement_ValueChanged);
            uiElement.Loaded -= new RoutedEventHandler(uiElement_Loaded);
            //uiElement.SelectionStartChanged -= new PropertyChangedCallback(uiElement_SelectionStartChanged);
            //uiElement.SelectionLengthChanged -= new PropertyChangedCallback(uiElement_SelectionLengthChanged);
            uiElement.RemoveHandler(IntegerTextBox.KeyDownEvent, new KeyEventHandler(OnKeyDown));
            uiElement.RemoveHandler(IntegerTextBox.PreviewKeyDownEvent, new KeyEventHandler(OnPreviewKeyDown));
            uiElement.RemoveHandler(IntegerTextBox.MouseRightButtonUpEvent, new MouseButtonEventHandler(uiElement_MouseRightButtonUp));
        }

        void uiElement_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.GridControl.Model.DisableEditorsContextMenu)
                e.Handled = true;
        }        

        void OnPreviewKeyDown(object sender, KeyEventArgs args)
        {
            IntegerTextBox textBox = (IntegerTextBox)sender;
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
            IntegerTextBox textBox = (IntegerTextBox)sender;
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

        protected override void OnGridPreviewTextInput(TextCompositionEventArgs e)
        {
            if (this.CurrentCell.IsEditing)
                return;

            this.CurrentCell.ScrollInView();
            this.CurrentCell.BeginEdit(true);
            if (this.CurrentCellUIElement != null)
                this.CurrentCellUIElement.Text = string.Empty;
        }

        protected override void OnSetFocus()
        {
            if ((this.GridControl.Model.Options.ActivateCurrentCellBehavior & GridCellActivateAction.SelectAll) != GridCellActivateAction.None)
                this.CurrentCellUIElement.SelectAll();
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
                return true;

            switch (e.Key)
            {
                case Key.Tab:
                    return true;

                case Key.Left:
                    if (this.CurrentCell.IsEditing)
                    {
                        if (isShiftKey)
                            return false;
                        if (CurrentCellUIElement != null)
                        {
                            if (isControlKey)
                            {
                                CurrentCellUIElement.CaretIndex = 0;
                                e.Handled = true;
                                return false;
                            }
                            if (CurrentCellUIElement.CaretIndex == 0 && CurrentCellUIElement.SelectionLength == 0)
                            {
                                e.Handled = true;
                                return true;
                            }// When the entire text is selected pressing left arrow key moves the focus to the previous cell, whereas in Excel the cursor moves to the 0th index. The below code is added to move the cursor to the 0th Index
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
                        if (isShiftKey)
                            return false;
                        if (CurrentCellUIElement != null)
                        {
                            if (isControlKey)
                            {
                                CurrentCellUIElement.CaretIndex = CurrentCellUIElement.Text.Length;
                                e.Handled = true;
                                return false;
                            }
                            if (CurrentCellUIElement.CaretIndex == 0 && CurrentCellUIElement.SelectionLength == 0)
                                //e.Handled = true;
                                return false;
                             // When the entire text is selected pressing right arrow key moves the focus to the next cell, whereas in Excel the cursor moves to the last position. The below code is added to move the cursor to the last position
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
                            if (isShiftKey)
                            {
                                CurrentCellUIElement.Select(CurrentCellUIElement.CaretIndex, (CurrentCellUIElement.Text.Length - CurrentCellUIElement.CaretIndex));
                                e.Handled = true;
                                return false;
                            }
                            else if (isControlKey)
                            {
                                CurrentCellUIElement.CaretIndex = CurrentCellUIElement.Text.Length;
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
                            if (isShiftKey)
                            {
                                e.Handled = true;
                                return false;
                            }
                            else if (isControlKey)
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
                        break;
                    else
                    {
                        if (this.CurrentStyle != null)
                        {
                            var renderer = this.CurrentCell.Renderer;
                            if (renderer != null)
                            {
                                GridDataStyleInfo sif = renderer.CurrentStyle.ModelStyle as GridDataStyleInfo;
                                if (sif != null && sif.CellIdentity.TableCellType != GridDataTableCellType.AddNewRecordCell)
                                    e.Handled = true;
                                else if (sif != null && sif.CellIdentity.TableCellType == GridDataTableCellType.AddNewRecordCell && !CurrentCell.IsEditing)
                                    e.Handled = true;
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
        /// Raises the GridCellClick event for the cell.
        /// </summary>
        /// <param name="rowIndex">Cell row index.</param>
        /// <param name="colIndex">Cell column index.</param>
        /// <param name="e">A reference to <see cref="MouseControllerEventArgs"/>.</param>
        public override void RaiseGridCellClick(int rowIndex, int colIndex, MouseControllerEventArgs e)
        {
            if (CurrentCell.HasCurrentCellAt(rowIndex, colIndex)
                && !CurrentCell.IsEditing
                && ((GridControl.Model.Options.ActivateCurrentCellBehavior & GridCellActivateAction.DblClickOnCell) == 0)
                && (GridControl.Model.Options.ActivateCurrentCellBehavior != GridCellActivateAction.None))
                CurrentCell.BeginEdit(true);

            base.RaiseGridCellClick(rowIndex, colIndex, e);
        }

        internal override void UnRegisterUIElement(IntegerTextBox uiElement)
        {
            uiElement.ClearValue(IntegerTextBox.MinValueProperty);
            uiElement.ClearValue(IntegerTextBox.MaxValueProperty);
            base.UnRegisterUIElement(uiElement);
        }
    }
}
