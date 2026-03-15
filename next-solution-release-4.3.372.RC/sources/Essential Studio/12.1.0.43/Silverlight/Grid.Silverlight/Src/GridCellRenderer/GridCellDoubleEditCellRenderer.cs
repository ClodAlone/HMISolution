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
    using System.Net;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;
    using Syncfusion.Windows.Controls.Grid;
    using Syncfusion.Windows.Controls.Scroll;
    using System.Diagnostics;
    using System.Globalization;
    using Syncfusion.Windows.Tools.Controls;

    public class GridCellDoubleEditCellModel : GridCellModel<GridCellDoubleEditCellRenderer>
    {
        public GridCellDoubleEditCellModel()
        {
        }

        public override string GetText(GridStyleInfo style, object value)
        {
            return (value != null && !(value is DBNull)) ? value.ToString() : string.Empty;
        }

        //public override bool ApplyFormattedText(GridStyleInfo style, string text, int textInfo)
        //{
        //    double dValue = 0.0;
        //    if (double.TryParse(text, NumberStyles.Number, style.NumberFormat, out dValue))
        //    {
        //        return true;
        //    }
        //    return base.ApplyFormattedText(style, text, textInfo);
        //}

        public override string GetFormattedText(GridStyleInfo style, object value, int textInfo)
        {
            Double dValue = 0;
            string retString = string.Empty;
            string text = this.GetText(style, value);
            var numberFormat = style.NumberFormat;

            if (Double.TryParse(text, NumberStyles.Any, numberFormat, out dValue))
            {
                retString = dValue.ToString("N", numberFormat);
            }
            return retString;
        }
    }

    public class GridCellDoubleEditCellRenderer : GridVirtualizingCellRenderer<DoubleTextBox>
    {
        public GridCellDoubleEditCellRenderer()
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
                this.ControlValue = value;
            }
        }

        private double GetDoubleValue(GridRenderStyleInfo style)
        {
            double dValue = double.MinValue;
            var value = this.GetControlValue(style);
            if (value != null && value.ToString() != string.Empty)
            {
                double.TryParse(value.ToString(), out dValue);
            }

            return dValue;
        }

        public override void OnInitializeContent(DoubleTextBox uiElement, GridRenderStyleInfo style)
        {
            this.OnUnwireUIElement(uiElement);
            var font = style.ReadOnlyFont;
            var tb = uiElement;
            Thickness margins = style.TextMargins.ToThickness();
            margins.Left = Math.Max(0, margins.Left - 2);
            margins.Right = Math.Max(0, margins.Right - 2);
            if (style.HasErrorInfo)
            {
                tb.Padding = style.ErrorInfo.AdjustErrorInfoMarginOnEditing(margins, style.GridControl, style.CellRowColumnIndex);
            }
            else
                tb.Padding = margins;
            tb.Margin = margins;
            tb.FontFamily = font.FontFamily;
            tb.FontSize = font.FontSize;
            tb.FontStretch = font.FontStretch;
            tb.FontWeight = font.FontWeight;
            tb.FontStyle = font.FontStyle;
            tb.Foreground = style.Foreground;
            tb.Background = style.Background;
            uiElement.TextAlignment = this.HorizontalAlignmentToTextAlignment(style.HorizontalAlignment);
            tb.HorizontalAlignment = style.HorizontalAlignment;
            tb.Padding = style.BorderMargins.ToThickness();
            tb.VerticalAlignment = style.VerticalAlignment;
            tb.UseNullOption = style.DoubleEdit.UseNullOption;
            //tb.MinValue = style.DoubleEdit.MinValue;
            //tb.MaxValue = style.DoubleEdit.MaxValue;
            if (!(style.DoubleEdit.HasMinValidation && style.DoubleEdit.MinValidation == MinValidation.OnLostFocus))
            {
                if (style.DoubleEdit.HasMinValue)
                    tb.MinValue = style.DoubleEdit.MinValue;
            }
            if (!(style.DoubleEdit.HasMaxValidation && style.DoubleEdit.MaxValidation == MaxValidation.OnLostFocus))
            {
                if (style.DoubleEdit.HasMaxValue)
                    tb.MaxValue = style.DoubleEdit.MaxValue;
            }
            tb.IsScrollingOnCircle = style.DoubleEdit.HasIsScrollingOnCircle ? style.DoubleEdit.IsScrollingOnCircle : tb.IsScrollingOnCircle;
            if (this.IsCurrentCell(style) && HasControlValue)
            {
                uiElement.Value = Convert.ToDouble(this.ControlValue);
            }
            else
            {
				//uiElement.Value = Convert.ToDouble(GetControlValue(style));
                var dValue = this.GetDoubleValue(style);
                if (dValue != double.MinValue)
                {
                    uiElement.Value = dValue;
                }
                else
                {
                    //uiElement.UseNullOption = true;
                    uiElement.Value = null;
                }
            }

            var numberFormatInfo = style.HasNumberFormat ? style.NumberFormat : this.CurrentStyle.GetCulture(false).NumberFormat;
            uiElement.NumberGroupSeparator = numberFormatInfo.NumberGroupSeparator;
            uiElement.NumberGroupSizes = numberFormatInfo.NumberGroupSizes;
            uiElement.NumberDecimalDigits = numberFormatInfo.NumberDecimalDigits;
            uiElement.NumberDecimalSeparator = numberFormatInfo.NumberDecimalSeparator;


            uiElement.BorderThickness = new Thickness(0);
            VisualContainer.SetWantsMouseInput(uiElement, false);
            this.OnWireUIElement(uiElement);
        }

        protected override void ArrangeUIElement(Cells.ArrangeCellArgs aca, DoubleTextBox uiElement, GridRenderStyleInfo style)
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

        protected override string GetControlTextFromEditorCore(DoubleTextBox uiElement)
        {
            return uiElement.Value.ToString();
        }


        protected override void OnWireUIElement(DoubleTextBox uiElement)
        {
            base.OnWireUIElement(uiElement);
            uiElement.ValueChanged += new PropertyChangedCallback(uiElement_ValueChanged);
           uiElement.KeyDown += new KeyEventHandler(uiElement_KeyDown);
            uiElement.TextChanged += new TextChangedEventHandler(uiElement_TextChanged);
        }


        /// <summary>
        /// This event fires while change the text in the UIElemnt
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        void uiElement_TextChanged(object sender, TextChangedEventArgs e)
        {
            var currencyEdit = (DoubleTextBox)sender;
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
                    currencyEdit.Select(currencyEdit.Value.ToString().Length, 0);
            }
        }


        /// <summary>
        /// This event fires while key down on the UIElement
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        void uiElement_KeyDown(object sender, KeyEventArgs e)
        {
             bool isShiftKey = (Keyboard.Modifiers & ModifierKeys.Shift) != ModifierKeys.None;
            DoubleTextBox textBox = (DoubleTextBox)sender;
            switch (e.Key)
            {
                case Key.Escape:
                    this.ActivateOptions.Element = null;
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

        protected override void OnGridPreviewTextInput(TextCompositionEventArgs e)
        {
            if (CurrentCell.IsEditing) return;

            CurrentCell.ScrollInView();
            CurrentCell.BeginEdit(true);
            if (CurrentCellUIElement == null) return;
            double value;
            if (double.TryParse(e.Text, out value))
            {
                CurrentCellUIElement.Value = value;               
            }
        }

        void uiElement_ValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DoubleTextBox textBox = (DoubleTextBox)d;
            if (!this.IsInArrange && this.IsCurrentCell(textBox) && !this.CurrentCell.IsInEndEdit)
            {
                if (!this.SetControlValue(textBox.Value))
                {
                    RefreshContent();
                }
            }
        }


        private void UpdateDoubleEdit()
        {
            if (CurrentCellUIElement != null)
            {
                //CurrentCellUIElement.Text = this.ControlText;
                //CurrentCellUIElement.Value = Convert.ToDouble(this.ControlValue);
                if (this.ControlValue != null && this.ControlValue.ToString() != string.Empty)
                {
                    CurrentCellUIElement.Value = Convert.ToDouble(this.ControlValue);
                }
                else
                {
                    //CurrentCellUIElement.UseNullOption = true;
                    CurrentCellUIElement.Value = null;
                }
                
            }
            else
            {
                GridControl.InvalidateCell(CellRowColumnIndex);
                //GridControl.InvalidateVisual(true);
            }
        }

        protected override void OnEnteredEditMode()
        {
            UpdateDoubleEdit();
        }

        protected override void OnEditingComplete()
        {
            UpdateDoubleEdit();
            GridControl.InvalidateCell(CellRowColumnIndex);
            if (this.isEnteryKeyPressed)
            {
                GridControl.InvalidateVisual(true);
            }
        }

        protected override void OnActivated()
        {
            UpdateDoubleEdit();
        }

        protected override void OnDeactivated()
        {
            UpdateDoubleEdit();
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

        protected override void OnSetFocus()
        {
            if (!ActivateOptions.IsActivateTriggeredByMouseDownIntoUIElement)
            {
                CurrentCellUIElement.SelectAll();
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
                    // e.Handled = true; Unreachable code
                   
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
                     if (this.CurrentCell.IsEditing)
                    {
                        DoubleTextBox tb = this.CurrentCellUIElement;
                        if (tb != null)
                        {
                            if (tb.IsScrollingOnCircle)
                            {
                                return false;
                            }
                            else
                            {
                                this.CurrentCell.MoveDown();
                                return false;
                            }
                        }
                    }
                    return true;
                case Key.Up:
                    if (this.CurrentCell.IsEditing)
                    {
                        DoubleTextBox tb = this.CurrentCellUIElement;
                        if (tb != null)
                        {
                            if (tb.IsScrollingOnCircle)
                            {
                                return false;
                            }
                            else
                            {
                                this.CurrentCell.MoveUp();
                                return false;
                            }
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
                        SetControlText("");
                        return false;
                    }
            }

            return base.ShouldGridTryToHandlePreviewKeyDown(e);
        }

        protected override void OnUnwireUIElement(DoubleTextBox uiElement)
        {
            base.OnUnwireUIElement(uiElement);
            uiElement.ValueChanged -= new PropertyChangedCallback(uiElement_ValueChanged);
            uiElement.KeyDown -= new KeyEventHandler(uiElement_KeyDown);
            uiElement.TextChanged -= new TextChangedEventHandler(uiElement_TextChanged);
        }
    }
}