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
    /// Implements the model part of a percent edit cell.
    /// </summary>
    public class GridCellPercentEditCellModel : GridCellNumericEditCellModel<GridCellPercentEditCellRenderer>
    {
        public override string GetFormattedText(GridStyleInfo style, object value, int textInfo)
        {
            double dValue = 0.0;
            string text = string.Empty;
            var numberFormat = style.HasNumberFormat ? style.NumberFormat : style.GetCulture(false).NumberFormat;
            value= (value != null && !(value is DBNull)) ? value.ToString() : string.Empty;
            if (double.TryParse(value.ToString(), out dValue))
                text = dValue.ToString(numberFormat);
            else
                text = this.GetText(style, value);

            if (style.NumberFormat != null && style.NumberFormat != style.GetCulture(false).NumberFormat)
            {
                numberFormat = style.NumberFormat;
            }
            else
            {
                numberFormat = style.GetCulture(false).NumberFormat;
            }
            //var formattedText = this.GetFormattedText(text, numberFormat);
            string retString = "";
            //text = this.GetNumber(text, numberFormat);
            if (double.TryParse(text, NumberStyles.Any, numberFormat, out dValue))
            {
                if (style.PercentEditMode == PercentEditMode.DoubleMode)
                    dValue /= 100.0;
                retString = dValue.ToString("P", numberFormat);
            }

            return retString;
            //return formattedText;
        }

        protected override string GetFormattedText(string text, System.Globalization.NumberFormatInfo numberFormatInfo)
        {
            double dValue = 0.0;
            string retString = "";
            text = this.GetNumber(text, numberFormatInfo);
            if (double.TryParse(text, NumberStyles.Any, numberFormatInfo, out dValue))
            {
                dValue /= 100.0;
                retString = dValue.ToString("P", numberFormatInfo);
            }

            return retString;
        }

        protected string GetNumber(string text, NumberFormatInfo numberFormat)
        {
            string retString = String.Empty;
            int len = text.Length;

            for (int i = 0; i < len; i++)
            {                
                char c = text[i];
                if (Char.IsDigit(c) || numberFormat.NegativeSign.Contains(c.ToString()) || c.ToString() == numberFormat.PercentDecimalSeparator)
                {
                    retString += c;
                }
            }

            return retString;
        }

       //// protected override object ApplyFormattedValue(GridStyleInfo style, string text)
       // {
       //     double dValue = 0.0;
       //     if (style.PercentEditMode != PercentEditMode.DoubleMode)
       //     {
       //         var startIdx = text.IndexOf(".") + 1;
       //         text = text.Substring(startIdx, text.Length - startIdx);
       //     }

       //     text = this.GetNumber(text, style.NumberFormat);
       //     if (double.TryParse(text, NumberStyles.Any, style.NumberFormat, out dValue))
       //     {
       //         if (style.PercentEditMode != PercentEditMode.DoubleMode)
       //         {
       //             return dValue / Math.Pow(10, style.NumberFormat.PercentDecimalDigits);
       //         }
       //         else
       //         {
       //             return dValue;
       //         }
       //     }
       //     return base.ApplyFormattedValue(style, text);
       // }
    }

    /// <summary>
    /// Implements the renderer part of a percent edit cell.
    /// </summary>
    public class GridCellPercentEditCellRenderer : GridVirtualizingCellRenderer<PercentTextBox>
    {
        int ccSelectionStart = 0, ccSelectionLength = 0; // , textSelectionStart = -1, textSelectionLength = 0;
        public GridCellPercentEditCellRenderer()
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

            decimal dValue = 0;

            if (style.CellValue != null && decimal.TryParse(style.CellValue.ToString(), NumberStyles.Currency, style.NumberFormat, out dValue))
            {
                if (dValue < 0)
                {
                    style.Foreground = style.HasNegativeForeground ? style.NegativeForeground : style.Foreground;
                }
            }

            // Draw the formatted text string to the DrawingContext of the control.
            GridTextBoxPaint.DrawText(dc, textRectangle, text, style);
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            var dValue = this.GetPercentValue(this.CurrentStyle);
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

        private double GetPercentValue(GridRenderStyleInfo style)
        {
            double dValue = double.MinValue;
            var value = this.GetControlValue(style);
            if (value != null && value.ToString() != string.Empty)
            {
                double.TryParse(value.ToString(), out dValue);
            }

            return dValue;
        }

        /// <summary>
        /// Initializes the content of the percent edit cell
        /// using the information from the cell style (value, text,
        /// behavior etc.).
        /// </summary>
        /// <param name="uiElement">The percent text box control.</param>
        /// <param name="style">The cell style info.</param>
        public override void OnInitializeContent(PercentTextBox uiElement, GridRenderStyleInfo style)
        {
            base.OnInitializeContent(uiElement, style);
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
            var dValue = this.GetPercentValue(style);
            if (dValue != double.MinValue)
            {
                uiElement.PercentValue = dValue;
            }
            else
            {
                uiElement.UseNullOption = true;
                uiElement.PercentValue = null;
                uiElement.Text = string.Empty;
            }

            var numberFormatInfo = style.NumberFormat != null ? style.NumberFormat : style.GetCulture(false).NumberFormat;
            uiElement.PercentageSymbol = numberFormatInfo.PercentSymbol;
            uiElement.PercentGroupSeparator = numberFormatInfo.PercentGroupSeparator;
            uiElement.PercentGroupSizes = new System.Windows.Media.Int32Collection(numberFormatInfo.PercentGroupSizes.ToList()); ;
            uiElement.PercentDecimalDigits = numberFormatInfo.PercentDecimalDigits;
            uiElement.PercentDecimalSeparator = numberFormatInfo.PercentDecimalSeparator;
            uiElement.PercentEditMode = style.PercentEditMode;
            if (this.GridControl.Model.Options.ShowErrorIconOnEditing)
                uiElement.Background = Brushes.Transparent;
            else
                uiElement.Background = Brushes.White;
            uiElement.Foreground = Brushes.Black;
            uiElement.MaxLength = style.MaxLength;
            var Percenteditstyle = style.PercentEdit;
            uiElement.MinValidation = Percenteditstyle.HasMinValidation ? Percenteditstyle.MinValidation : uiElement.MinValidation;
            uiElement.MaxValidation = Percenteditstyle.HasMaxValidation ? Percenteditstyle.MaxValidation : uiElement.MaxValidation;
            uiElement.MinValue = Percenteditstyle.HasMinValue ? (double)Percenteditstyle.MinValue : uiElement.MinValue;
            uiElement.MaxValue = Percenteditstyle.HasMaxValue ? (double)Percenteditstyle.MaxValue : uiElement.MaxValue;
            uiElement.IsScrollingOnCircle = Percenteditstyle.HasIsScrollingOnCircle ? Percenteditstyle.IsScrollingOnCircle : uiElement.IsScrollingOnCircle;
            uiElement.UseNullOption = Percenteditstyle.UseNullOption;
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
            //uiElement.IsReadOnly = style.HasReadOnly ? style.ReadOnly : false;
            this.OnWireUIElement(uiElement);
        }

        protected override void ArrangeUIElement(Cells.ArrangeCellArgs aca, PercentTextBox uiElement, GridRenderStyleInfo style)
        {
            Thickness margins = style.TextMargins.ToThickness();
            margins = style.ErrorInfo.AdjustErrorInfoMarginOnEditing(margins, style.GridControl, style.CellRowColumnIndex);
            uiElement.Padding = margins;

            base.ArrangeUIElement(aca, uiElement, style);
        }

        protected override void OnEnteredEditMode()
        {
            //ApplyTextBoxProperties();
            if (CurrentCellUIElement != null)
            {
                GridRenderStyleInfo style = CurrentStyle;
                //var text = GetControlText(style);
                //if (!((object)text).Equals(this.ControlValue))
                if (style.CellValue != null && this.ControlValue != null)
                {
                    if (!(style.CellValue).Equals(this.ControlValue) && this.CurrentCell.IsModified)
                    {
                        //this.ControlValue = text;
                        this.ControlValue = style.CellValue;
                    }
                }
            }
            else
            {
                GridControl.InvalidateCell(CellRowColumnIndex);
            }
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
            //if (this.CurrentCellUIElement != null)
            //{
            //    this.CurrentCellUIElement.PercentValue = null;
            //    this.CurrentCellUIElement.Text = string.Empty;
            //}

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
                var dValue = this.GetPercentValue(style);
                if (dValue != double.MinValue && this.CurrentCellUIElement != null)
                {
                    this.CurrentCellUIElement.PercentValue = dValue;
                }
            }
            if (CurrentCellUIElement != null && GridControl.Model.Options.AllowTextSelectionOnReadOnly)
                CurrentCellUIElement.IsReadOnly = style.ReadOnly;

            if (CurrentCellUIElement == null)
                this.GridControl.InvalidateCell(this.CellRowColumnIndex);
        }

        protected override string GetControlTextFromEditorCore(PercentTextBox uiElement)
        {
            return uiElement.PercentValue != null ? uiElement.PercentValue.ToString() : string.Empty;
        }

        protected override void OnWireUIElement(PercentTextBox uiElement)
        {
            base.OnWireUIElement(uiElement);
            uiElement.PercentValueChanged += new PropertyChangedCallback(uiElement_PercentValueChanged);
            uiElement.Loaded += new RoutedEventHandler(uiElement_Loaded);
            uiElement.AddHandler(PercentTextBox.KeyDownEvent, new KeyEventHandler(OnKeyDown), true);
            uiElement.AddHandler(PercentTextBox.MouseRightButtonUpEvent, new MouseButtonEventHandler(uiElement_MouseRightButtonUp), true);
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

        void uiElement_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.GridControl.Model.DisableEditorsContextMenu)
            {
                e.Handled = true;
            }
        }

        protected override void OnUnwireUIElement(PercentTextBox uiElement)
        {
            base.OnUnwireUIElement(uiElement);
            uiElement.PercentValueChanged -= new PropertyChangedCallback(uiElement_PercentValueChanged);
            uiElement.Loaded -= new RoutedEventHandler(uiElement_Loaded);
            uiElement.RemoveHandler(PercentTextBox.MouseRightButtonUpEvent, new MouseButtonEventHandler(uiElement_MouseRightButtonUp));
            uiElement.RemoveHandler(PercentTextBox.KeyDownEvent, new KeyEventHandler(OnKeyDown));
        }

        void uiElement_PercentValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var percent = (PercentTextBox)d;
            if (!this.IsInArrange && this.IsCurrentCell(percent) && !this.CurrentCell.IsInEndEdit)
            {
                if (percent.PercentValue == null && !percent.UseNullOption)
                    percent.PercentValue = 0;
                if (!this.SetControlValue(percent.PercentValue))
                {
                    RefreshContent();
                }
            }
        }

        void OnKeyDown(object sender, KeyEventArgs args)
        {
            PercentTextBox textBox = (PercentTextBox)sender;
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

        protected override bool ShouldGridTryToHandlePreviewKeyDown(KeyEventArgs e)        {
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
                        if (isShiftKey)
                        {
                            return false;
                        }
                        PercentTextBox tb = this.CurrentCellUIElement;
                        if (isControlKey)
                        {
                            tb.CaretIndex = 0;
                            e.Handled = true;
                            return false;
                        }
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
                    // e.Handled = true; Unreachable code
                case Key.Right:
                    if (this.CurrentCell.IsEditing)
                    {
                        if (isShiftKey)
                        {
                            return false;
                        }
                        PercentTextBox tb = this.CurrentCellUIElement;
                        if (isControlKey)
                        {
                            tb.CaretIndex = tb.Text.Length;
                            e.Handled = true;
                            return false;
                        }
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
                case Key.Up:
                    if (this.CurrentCell.IsEditing)
                    {
                        PercentTextBox tb = this.CurrentCellUIElement;
                        if (tb != null)
                        {
                            if (tb.IsScrollingOnCircle)
                            {
                                return false;
                            }
                            else
                            {
                                return true;
                            }
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
                        PercentTextBox textBox = this.CurrentCellUIElement;
                        if (textBox != null && this.CurrentCell.IsEditing && textBox.SelectionLength == textBox.Text.Length)
                        {
                            textBox.CaretIndex = textBox.Text.Length;
                            e.Handled = true;
                            return false;
                        }
                    }
                    break;
            }

            //e.Handled = true;
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

        internal override void UnRegisterUIElement(PercentTextBox uiElement)
        {
            uiElement.ClearValue(PercentTextBox.MaxValueProperty);
            uiElement.ClearValue(PercentTextBox.MinValueProperty);
            base.UnRegisterUIElement(uiElement);
        }
    }
}
