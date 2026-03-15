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
    using System.Windows;
    using System.Windows.Input;
    using Syncfusion.Windows.Controls.Cells;
    using Syncfusion.Windows.Controls.Scroll;
    using Syncfusion.Windows.Tools.Controls;
    using System.Globalization;
    using System.Windows.Controls;

    public class GridCellMaskEditCellModel : GridCellModel<GridCellMaskEditCellRenderer>
    {
        public GridCellMaskEditCellModel()
        {
        }

        private string MaskedText
        {
            get;
            set;
        }

        public override string GetText(GridStyleInfo style, object value)
        {
            this.MaskedText = (value != null && !(value is DBNull)) ? value.ToString() : string.Empty;
            return this.MaskedText;
        }

        public override string GetFormattedText(GridStyleInfo style, object value, int textInfo)
        {
            string maskedText = GetText(style, value);
            GridMaskEditStyleInfo mi = style.MaskEdit;
            return MaskedEditorModel.GetMaskedText(
                mi.Mask,
                maskedText,
                mi.DateSeparator,
                mi.TimeSeparator,
                mi.DecimalSeparator,
                mi.NumberGroupSeparator,
                mi.PromptChar,
                mi.CurrencySymbol);
        }
    }

    public class GridCellMaskEditCellRenderer : GridVirtualizingCellRenderer<MaskedTextBox>
    {
        string oldValue = string.Empty;

        public GridCellMaskEditCellRenderer()
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
            var value = this.GetControlValue(this.CurrentStyle);
            if (value != null && value.ToString() != string.Empty)
            {
                this.ControlValue = Convert.ToString(value);
            }
        }

        public override void OnInitializeContent(MaskedTextBox textBox, GridRenderStyleInfo style)
        {
            OnUnwireUIElement(textBox);
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
            textBox.Padding = margins;
            textBox.BorderThickness = new Thickness(0);
            GridMaskEditStyleInfo maskeditstyle = style.MaskEdit;
            textBox.Foreground = style.Foreground;
            textBox.Background = style.Background;
            textBox.HorizontalAlignment = style.HorizontalAlignment;
            textBox.VerticalAlignment = style.VerticalAlignment;
            textBox.Mask = maskeditstyle.HasMask ? maskeditstyle.Mask : string.Empty;
            textBox.DateSeparator = maskeditstyle.HasDecimalSeparator ? maskeditstyle.DateSeparator : string.Empty;
            textBox.TimeSeparator = maskeditstyle.HasTimeSeparator ? maskeditstyle.TimeSeparator : string.Empty;
            textBox.DecimalSeparator = maskeditstyle.HasDecimalSeparator ? maskeditstyle.DecimalSeparator : string.Empty;
            textBox.NumberGroupSeparator = maskeditstyle.HasNumberGroupSeparator ? maskeditstyle.NumberGroupSeparator : string.Empty;
            textBox.PromptChar = maskeditstyle.HasPromptChar ? maskeditstyle.PromptChar : '_';
            textBox.CurrencySymbol = maskeditstyle.HasCurrencySymbol ? maskeditstyle.CurrencySymbol : string.Empty;
            textBox.TextMaskFormat = MaskFormat.ExcludePromptAndLiterals;
            textBox.StringValidation = maskeditstyle.HasStringValidation ? maskeditstyle.StringValidation : StringValidation.OnLostFocus;
            textBox.TextAlignment = this.HorizontalAlignmentToTextAlignment(style.HorizontalAlignment);
            
            if (maskeditstyle.HasMaxLength && textBox.StringValidation == StringValidation.OnKeyPress)
                textBox.MaxLength = maskeditstyle.MaxLength;
            if (maskeditstyle.HasMinLength && textBox.StringValidation == StringValidation.OnKeyPress)
                textBox.MinLength = maskeditstyle.MinLength;
            if (this.IsCurrentCell(style) && HasControlValue)
            {
                textBox.Value = Convert.ToString(this.ControlValue);
            }
            else
            {
                
                
                    var value = this.GetControlText(style);
                    if (value != null && value.ToString() != string.Empty)
                    {
                        textBox.Value = Convert.ToString(value);
                    }
                    else
                    {
                        textBox.Value = string.Empty;
                        textBox.Text = string.Empty;
                    }
            }
            oldValue = textBox.Value;
            VisualContainer.SetWantsMouseInput(textBox, false);
            OnWireUIElement(textBox);
        }

        protected override void ArrangeUIElement(ArrangeCellArgs aca, MaskedTextBox uiElement, GridRenderStyleInfo style)
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

        protected override void OnWireUIElement(MaskedTextBox uiElement)
        {
            base.OnWireUIElement(uiElement);
            uiElement.ValueChanged += new PropertyChangedCallback(uiElement_ValueChanged);
            uiElement.KeyDown += new KeyEventHandler(uiElement_KeyDown);
            uiElement.TextChanged += new System.Windows.Controls.TextChangedEventHandler(uiElement_TextChanged);
        }

        /// <summary>
        /// This event fires while text changed in the UIElement
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void uiElement_TextChanged(object sender, TextChangedEventArgs e)
        {
            var currencyEdit = (MaskedTextBox)sender;
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
            bool isShiftKey = (Keyboard.Modifiers & ModifierKeys.Shift) != ModifierKeys.None;
            MaskedTextBox textBox = (MaskedTextBox)sender;
            switch (e.Key)
            {
                case Key.Escape:
                    this.ActivateOptions.Element = null;
                    break;
                case Key.Right:
                case Key.Left:
                case Key.Down:
                case Key.Up:
                    if (textBox.SelectionStart == 0 || textBox.SelectionStart == textBox.Text.Length)
                    {
                        this.GridControl.MoveCurrentCellWithArrowKey(e);
                    }
                    break;
                case Key.Enter:
                    if (isShiftKey)
                    {
                        break;
                    }
                    else
                    {
                        if (this.CurrentStyle != null)
                        {
                            GridDataStyleInfo sif = this.CurrentStyle.GridModel[this.CurrentCell.RowIndex, this.CurrentCell.ColumnIndex] as GridDataStyleInfo;
                            if (sif != null && sif.CellIdentity.TableCellType != GridDataTableCellType.AddNewRecordCell)
                            {
                                e.Handled = true;
                            }
                            else if (sif != null && sif.CellIdentity.TableCellType == GridDataTableCellType.AddNewRecordCell && !CurrentCell.IsEditing)
                            {
                                e.Handled = true;
                            }
                        }
                        CurrentCell.MoveRight();
                        break;
                    }  
            }
        }

		void uiElement_ValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MaskedTextBox textBox = (MaskedTextBox)d;
            if (!this.IsInArrange && this.IsCurrentCell(textBox) && !this.CurrentCell.IsInEndEdit)
            {
                if (!this.SetControlValue(textBox.Value))
                {
                    RefreshContent();
                }
            }
        }
		
		protected override string GetControlTextFromEditorCore(MaskedTextBox uiElement)
        {
            return uiElement.Value.ToString();
        }
		
		private void UpdateMaskEdit()
        {
            if (CurrentCellUIElement != null)
            {
                CurrentCellUIElement.Value = Convert.ToString(this.ControlValue);
            }
            else
            {
                GridControl.InvalidateCell(CellRowColumnIndex);
				GridControl.InvalidateVisual(true);
            }
        }
		
		protected override void OnEnteredEditMode()
        {
            UpdateMaskEdit();
        }

        protected override void OnGridPreviewTextInput(TextCompositionEventArgs e)
        {
            if (CurrentCell.IsEditing) return;

            CurrentCell.ScrollInView();
            CurrentCell.BeginEdit(true);
            if (CurrentCellUIElement == null) return;
            CurrentCellUIElement.Value = e.Text;
        }
		
		protected override void OnDeactivated()
        {
            UpdateMaskEdit();
            GridControl.InvalidateCell(CellRowColumnIndex);
            if (this.isEnteryKeyPressed)
            {
                GridControl.InvalidateVisual(true);
            }
            else
            {
                this.isEnteryKeyPressed = false;
            }
        }
		
		protected override void OnActivated()
        {
            UpdateMaskEdit();
        }

        //Override to revert the cell value, if length was less than the MinLength
        protected override bool OnSaveChanges()
        {
            try
            {
                if (this.CurrentStyle.MaskEdit.HasMinLength)
                {
                    if (ControlValue.ToString().Length >= this.CurrentStyle.MaskEdit.MinLength)
                    {
                        CurrentStyle.ModelStyle.CellValue = ControlValue;
                    }
                    else
                    {
                        if (oldValue != null && ControlValue !=null && ControlValue.ToString() != string.Empty && oldValue.Length >= CurrentStyle.MaskEdit.MinLength)
                        {
                            CurrentStyle.ModelStyle.CellValue = oldValue;
                        }
                        else
                        {
                            CurrentStyle.ModelStyle.CellValue = string.Empty;
                        }
                    }
                }
                else
                {
                    CurrentStyle.ModelStyle.CellValue = ControlValue;
                }
            }
            catch (Exception ex)
            {
                CurrentStyle.Exception = ex;
                return false;
            }
            return true;
        }
		
		protected override void OnEditingComplete()
        {
            UpdateMaskEdit();
            GridControl.InvalidateCell(CellRowColumnIndex);
            if (this.isEnteryKeyPressed)
            {
                GridControl.InvalidateVisual(true);
            }
        }

        
        protected override void OnSetFocus()
        {
            if (!ActivateOptions.IsActivateTriggeredByMouseDownIntoUIElement)
            {
                this.CurrentCellUIElement.SelectAll();
            }
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
                        MaskedTextBox tb = this.CurrentCellUIElement;
                        if (tb != null)
                        {
                            if (tb.SelectionLength == tb.Text.Length)
                                return true;
                            else
                                return false;
                        }
                        return false;
                    }
                    e.Handled = true;
                    return true;
                case Key.Left:
                    if (this.CurrentCell.IsEditing)
                    {
                        MaskedTextBox tb = this.CurrentCellUIElement;
                        if (tb != null)
                        {
                            if (tb.SelectionStart == 0)
                                return true;
                            else
                                return false;
                        }
                        return false;
                    }
                    e.Handled = true;
                    return true;
                case Key.Down:
                case Key.Up:
                    if (this.CurrentCell.IsEditing)
                    {
                        MaskedTextBox tb = this.CurrentCellUIElement;
                        if (tb != null)
                        {
                            return true;
                        }
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
		
		protected override void OnUnwireUIElement(MaskedTextBox uiElement)
        {
            base.OnUnwireUIElement(uiElement);
            uiElement.ValueChanged -= new PropertyChangedCallback(uiElement_ValueChanged);
            uiElement.KeyDown -= new KeyEventHandler(uiElement_KeyDown);
            uiElement.TextChanged -= new System.Windows.Controls.TextChangedEventHandler(uiElement_TextChanged);
        }
    }
}
