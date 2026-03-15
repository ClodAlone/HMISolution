#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Windows.Media;

namespace Syncfusion.Windows.Controls.Grid
{
    using System;
    using System.Windows;
    using System.Windows.Input;
    using Syncfusion.Windows.Controls.Cells;
    using Syncfusion.Windows.Controls.Scroll;
    using Syncfusion.Windows.Shared;
    using Syncfusion.Windows.Tools.Controls;

    /// <summary>
    /// Implements the model part of a mask edit cell.
    /// </summary>
    public class GridCellMaskEditCellModel : GridCellNumericEditCellModel<GridCellMaskEditCellRenderer>
    {
        /// <summary>
        /// Initializes a new <see cref="GridCellMaskEditCellModel"/>.
        /// </summary>
        public GridCellMaskEditCellModel()
        {
        }

        private string MaskedText
        {
            get;
            set;
        }

        /// <summary>
        /// Returns the display text as value object.
        /// </summary>
        /// <param name="style">Style information for the cell.</param>
        /// <param name="text">The display text.</param>
        /// <returns>The formatted value of the cell.</returns>       
        protected override object ApplyFormattedValue(GridStyleInfo style, string text)
        {
            return text;
        }

        /// <summary>
        /// This is called from GridStyleInfo.GetText (ignoring any <see cref="GridStyleInfo.Format"/> settings).
        /// </summary>
        /// <param name="style">The <see cref="GridStyleInfo"/> object that holds cell information.</param>
        /// <param name="value">The value to convert to a string.</param>
        /// <returns>The string that represents the given value.</returns>
        public override string GetText(GridStyleInfo style, object value)
        {
            this.MaskedText = (value != null && !(value is DBNull)) ? value.ToString() : string.Empty;
            return this.MaskedText;
        }

        /// <summary>
        /// Return formatted text for the specified value.
        /// </summary>
        /// <param name="style">The <see cref="GridStyleInfo"/> object that holds cell information.</param>
        /// <param name="value">The value to format.</param>
        /// <param name="textInfo">TextInfo is a hint of who is calling, default is GridCellBaseTextInfo.DisplayText.</param>
        /// <returns>The formatted text for the given value.</returns>
        public override string GetFormattedText(GridStyleInfo style, object value, int textInfo)
        {
            string maskedText = GetText(style, value);
            GridMaskEditInfo mi = style.MaskEdit;
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

    /// <summary>
    /// Implements the renderer part of a mask edit cell.
    /// </summary>
    public class GridCellMaskEditCellRenderer : GridVirtualizingCellRenderer<MaskedTextBox>
    {
        int textSelectionStart = -1, textSelectionLength = 0; // ccSelectionStart, ccSelectionLength,
        string oldValue = string.Empty;


        /// <summary>
        /// Initializes a new <see cref="GridCellMaskEditCellRenderer"/>.
        /// </summary>
        public GridCellMaskEditCellRenderer()
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
            {
                return;
            }

            // Will only get hit if SupportsRenderOptimization is true, otherwise rca.CellUIElements is never null.
            Thickness margins = style.TextMargins.ToThickness();
            if (style.HasImageIndex)
            {
                margins = style.AdjustImageWidthAndHeightToMargin(margins, rca.CellRect.Size);
            }
            else
            {
                margins = style.ErrorInfo.AdjustErrorInfoMargin(margins, rca.CellRect.Size);
            }
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
            {
                text = this.ControlText;
            }
            else
            {
                text = this.GetControlText(style);
            }

            // Draw the formatted text string to the DrawingContext of the control.
            GridTextBoxPaint.DrawText(dc, textRectangle, text, style);
        }


        public override void OnInitializeContent(MaskedTextBox uiElement, GridRenderStyleInfo style)
        {
            base.OnInitializeContent(uiElement, style);
            OnUnwireUIElement(uiElement);
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

            GridMaskEditInfo mi = style.MaskEdit;
            //Apply Text before so that it gets processed before the Mask is applied
            uiElement.Mask = mi.HasMask ? mi.Mask : string.Empty;
            uiElement.DateSeparator = mi.HasDateSeparator ? mi.DateSeparator : string.Empty;
            uiElement.TimeSeparator = mi.HasTimeSeparator ? mi.TimeSeparator : string.Empty;
            uiElement.DecimalSeparator = mi.HasDecimalSeparator ? mi.DecimalSeparator : string.Empty;
            uiElement.NumberGroupSeparator = mi.HasNumberGroupSeparator ? mi.NumberGroupSeparator : string.Empty;
            uiElement.PromptChar = mi.HasPromptChar ? mi.PromptChar : '_';
            uiElement.CurrencySymbol = mi.HasCurrencySymbol ? mi.CurrencySymbol : string.Empty;
            uiElement.TextMaskFormat = MaskFormat.ExcludePromptAndLiterals;
            uiElement.StringValidation = mi.HasStringValidation ? mi.StringValidation : uiElement.StringValidation;
            if (uiElement.StringValidation == StringValidation.OnKeyPress)
            {
                uiElement.MaxLength = mi.HasMaxLength ? mi.MaxLength : uiElement.MaxLength;
                uiElement.MinLength = mi.HasMinLength ? mi.MinLength : uiElement.MinLength;
            }
            var value = this.GetControlText(style);
            if (value != null && value.ToString() != string.Empty)
            {
                uiElement.Value = Convert.ToString(value);
            }
            else
            {
                uiElement.Value = string.Empty;
                uiElement.Text = string.Empty;
            }
            oldValue = uiElement.Value;
            if (this.GridControl.Model.Options.ShowErrorIconOnEditing)
                uiElement.Background = Brushes.Transparent;
            else
                uiElement.Background = Brushes.White;
            uiElement.Foreground = Brushes.Black;
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
            OnWireUIElement(uiElement);
        }

        protected override void ArrangeUIElement(ArrangeCellArgs aca, MaskedTextBox uiElement, GridRenderStyleInfo style)
        {
            Thickness margins = style.TextMargins.ToThickness();
            margins = style.ErrorInfo.AdjustErrorInfoMarginOnEditing(margins, style.GridControl, style.CellRowColumnIndex);
            uiElement.Padding = margins;

            base.ArrangeUIElement(aca, uiElement, style);
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

        protected virtual void ApplyMaskedEditProperties()
        {
            var style = this.CurrentStyle;
            if (style != null)
            {
                var value = this.GetControlValue(style);
                if (value != null && value.ToString() != string.Empty)
                {
                    var stringValue = Convert.ToString(value);
                    if (this.CurrentCellUIElement != null)
                    {
                        this.CurrentCellUIElement.Value = stringValue;
                    }
                }
            }
            if (CurrentCellUIElement != null && GridControl.Model.Options.AllowTextSelectionOnReadOnly)
                CurrentCellUIElement.IsReadOnly = style.ReadOnly;
            if (CurrentCellUIElement == null)
                this.GridControl.InvalidateCell(this.CellRowColumnIndex);
        }

        protected override void OnEditingComplete()
        {
            this.GridControl.InvalidateCell(this.CellRowColumnIndex);

            if (this.CurrentCellUIElement != null)
            {
                this.CurrentCellUIElement.CaretIndex = 0;
            }
        }

        protected override void OnActivated()
        {
            this.ApplyMaskedEditProperties();
            if (this.CurrentCell.Grid.Model.Options.ExcelLikeCurrentCell)
            {
                this.RaiseBeginEdit();
            }
        }

        protected override void OnDeactivated()
        {
            this.GridControl.InvalidateCell(this.CellRowColumnIndex);
        }

        protected override string GetControlTextFromEditorCore(MaskedTextBox uiElement)
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

        protected override void OnWireUIElement(MaskedTextBox uiElement)
        {
            base.OnWireUIElement(uiElement);
            uiElement.ValueChanged += new PropertyChangedCallback(uiElement_ValueChanged);
            uiElement.Loaded+=new RoutedEventHandler(uiElement_Loaded);
            //uiElement.SelectionStartChanged += new PropertyChangedCallback(uiElement_SelectionStartChanged);
            //uiElement.SelectionLengthChanged += new PropertyChangedCallback(uiElement_SelectionLengthChanged);
            //uiElement.AddHandler(EditorBase.PreviewKeyDownEvent, new KeyEventHandler(OnPreviewKeyDown), true);
            //uiElement.AddHandler(EditorBase.KeyDownEvent, new KeyEventHandler(OnKeyDown), true);
            uiElement.AddHandler(MaskedTextBox.MouseRightButtonUpEvent, new MouseButtonEventHandler(uiElement_MouseRightButtonUp),true);
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

        void uiElement_ValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MaskedTextBox textBox = (MaskedTextBox)d;
            if (!this.IsInArrange && this.IsCurrentCell(textBox) && !this.CurrentCell.IsInEndEdit)
            {
                if (!this.SetControlText(textBox.Value))
                {
                    RefreshContent();
                }
            }
        }

        //void uiElement_SelectionLengthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    if (!this.IsInArrange)
        //    {
        //        this.textSelectionLength = ((MaskedTextBox)d).SelectionLength;
        //    }
        //}

        //void uiElement_SelectionStartChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    if (!this.IsInArrange)
        //    {
        //        this.textSelectionStart = ((MaskedTextBox)d).SelectionStart;
        //    }
        //}

        protected override void OnEnteredEditMode()
        {
            this.ApplyMaskedEditProperties();
        }


        protected override void OnUnwireUIElement(MaskedTextBox uiElement)
        {
            base.OnUnwireUIElement(uiElement);
            uiElement.ValueChanged -= new PropertyChangedCallback(uiElement_ValueChanged);
            uiElement.Loaded -= new RoutedEventHandler(uiElement_Loaded);
            //uiElement.SelectionStartChanged -= new PropertyChangedCallback(uiElement_SelectionStartChanged);
            //uiElement.SelectionLengthChanged -= new PropertyChangedCallback(uiElement_SelectionLengthChanged);
            //uiElement.RemoveHandler(MaskedTextBox.KeyDownEvent, new KeyEventHandler(OnKeyDown));
            //uiElement.RemoveHandler(MaskedTextBox.PreviewKeyDownEvent, new KeyEventHandler(OnPreviewKeyDown));
            uiElement.RemoveHandler(MaskedTextBox.MouseRightButtonUpEvent, new MouseButtonEventHandler(uiElement_MouseRightButtonUp));
        }

        void uiElement_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.GridControl.Model.DisableEditorsContextMenu)
            {
                e.Handled = true;
            }
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

        protected override void OnSetFocus()
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
                        if (oldValue != null && ControlValue != null && ControlValue.ToString() != string.Empty && oldValue.Length >= CurrentStyle.MaskEdit.MinLength)
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

                case Key.Right:
                    {
                        if (this.CurrentCell.IsEditing)
                        {
                            if (isShiftKey)
                            {
                                return false;
                            }
                            if (isControlKey)
                            {
                                CurrentCellUIElement.CaretIndex = CurrentCellUIElement.Text.Length;
                                e.Handled = true;
                                return false;
                            }
                            MaskedTextBox tb = this.CurrentCellUIElement;
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
                        // otherwise, move caret within textbox when cell is not in edit-mode.
                        return !this.CurrentCell.IsEditing;
                    }
                case Key.Left:
                    {
                        if (this.CurrentCell.IsEditing)
                        {
                            if (isShiftKey)
                            {
                                return false;
                            }
                            if (isControlKey)
                            {
                                CurrentCellUIElement.CaretIndex = 0;
                                e.Handled = true;
                                return false;
                            }
                            MaskedTextBox tb = this.CurrentCellUIElement;
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
                        // otherwise, move caret within textbox when cell is not in edit-mode.
                        return !this.CurrentCell.IsEditing;
                    }
                case Key.Down:
                    {
                    MaskedTextBox tb = this.CurrentCellUIElement;
                    if (this.CurrentCell.IsEditing)
                    {
                        if (CurrentCellUIElement != null)
                        {
                            if (isShiftKey)
                            {
                                CurrentCellUIElement.Select(CurrentCellUIElement.CaretIndex, CurrentCellUIElement.Text.Length);
                                e.Handled = true;
                                return false;
                            }
                            else if (isControlKey)
                            {
                                CurrentCellUIElement.CaretIndex = CurrentCellUIElement.Text.Length;
                                e.Handled = true;
                                return false;
                            }
                            else
                                return true;
                        }
                    }
                    else if (tb != null && tb.SelectionStart == 0 && tb.SelectionLength == tb.Text.Length)
                        {
                            return true;
                        }

                        // otherwise, move caret within textbox when cell is not in edit-mode.
                        return !this.CurrentCell.IsEditing;
                    }
                case Key.Up:
                    {
                        //// Move to next cell when whole text in textbox is selected.
                        MaskedTextBox tb = this.CurrentCellUIElement;
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
                                else
                                    return true;
                            }
                        }
                        else if (tb != null && tb.SelectionStart == 0 && tb.SelectionLength == tb.Text.Length)
                        {
                            return true;
                        }

                        // otherwise, move caret within textbox when cell is not in edit-mode.
                        return !this.CurrentCell.IsEditing;
                    }
                case Key.End:
                case Key.Home:
                    {
                        if (this.CurrentCell.IsEditing || isControlKey)
                        {
                            this.CurrentCell.BeginEdit(true);
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }

                //case Key.Delete:
                //    {
                //        this.CurrentCell.BeginEdit(true);
                //        //this.SetControlText(string.Empty);
                //        return false;
                //    }
                case Key.Back:
                    {
                        this.CurrentCell.BeginEdit(true);
                        //this.SetControlText(string.Empty);
                        return false;
                    }
                case Key.Enter:
                    if (isShiftKey)
                    {
                        if (this.CurrentCellUIElement != null && this.CurrentCellUIElement.AcceptsReturn)
                            return false;
                        else
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
                        if (this.CurrentCell.IsEditing)
                            CurrentCell.EndEdit();
                       CurrentCell.MoveRight();
                      
                        return true;
                        // break; Unreachable code
                    }
                case Key.F2:
                    {
                        //CurrentCell.BeginEdit(true);
                        MaskedTextBox textBox = this.CurrentCellUIElement;
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
        /// Raises the GridCellClick event for the cell.
        /// </summary>
        /// <param name="rowIndex">Cell row index.</param>
        /// <param name="colIndex">Cell column index.</param>
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

    }
}
