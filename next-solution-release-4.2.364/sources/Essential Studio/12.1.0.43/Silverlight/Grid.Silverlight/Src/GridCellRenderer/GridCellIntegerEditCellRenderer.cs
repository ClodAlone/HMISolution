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
    //using Syncfusion.Silverlight.Tools.Controls;
    using System.Globalization;
    using Syncfusion.Windows.Tools.Controls;
    using Syncfusion.Windows.Controls.Cells;

    public class GridCellIntegerEditCellModel : GridCellModel<GridCellIntegerEditCellRenderer>
    {
        public GridCellIntegerEditCellModel()
        {
        }

        public override string GetText(GridStyleInfo style, object value)
        {
            return (value != null && !(value is DBNull)) ? value.ToString() : string.Empty;
        }

        //public override bool ApplyFormattedText(GridStyleInfo style, string text, int textInfo)
        //{
        //    Int64 dValue = 0;
        //    if (Int64.TryParse(text, NumberStyles.Integer, style.NumberFormat, out dValue))
        //    {
        //        return true;
        //    }
        //    return base.ApplyFormattedText(style, text, textInfo);
        //}

        public override string GetFormattedText(GridStyleInfo style, object value, int textInfo)
        {
            Int64 dValue = 0;
            string retString = string.Empty;
            string text = this.GetText(style, value);
            //var numberFormat = style.NumberFormat;
            var numberFormat = style.HasNumberFormat ? style.NumberFormat : style.GetCulture(false).NumberFormat;
            // Integer text should not have decimal digits
            var numberFormatInfoCopy = numberFormat.Clone() as System.Globalization.NumberFormatInfo;
            numberFormatInfoCopy.NumberDecimalDigits = 0;
            string format = style.Format;

            if (Int64.TryParse(text, out dValue))
            {
                retString = dValue.ToString("N", numberFormatInfoCopy);
                if (!string.IsNullOrEmpty(format))
                    retString = dValue.ToString(format);
            }
            return retString;
        }

    }

    public class GridCellIntegerEditCellRenderer : GridVirtualizingCellRenderer<IntegerTextBox>
    {
        public GridCellIntegerEditCellRenderer()
        {
            this.AllowRecycle = true;
            this.IsControlTextShown = true;
            this.SupportsRenderOptimization = true;
            this.IsFocusable = true;
            this.AllowKeepAliveOnlyCurrentCell = true;
        }

        protected override void OnInitialize()
        {

            CellUIElements cellUI = this.GridControl.GetCellUIElements(CellRowColumnIndex);           
            if (cellUI != null && cellUI.UIElements.Count > 0 && !cellUI.IsDirty)
            {
                this.ActivateOptions.Element = cellUI.UIElements[0];
                if (!(this.ActivateOptions.Element is ComboBox))
                    this.RefreshContent();
            }
            base.OnInitialize();
            var value = this.GetControlValue(this.CurrentStyle);
            if (value != null && value.ToString() != string.Empty)
            {
                this.ControlValue = value;
            }
        }

        private Int64 GetIntegerValue(GridRenderStyleInfo style)
        {
            Int64 dValue = Int64.MinValue;
            var value = this.GetControlValue(style);
            if (value != null && value.ToString() != string.Empty)
            {
                Int64.TryParse(value.ToString(), out dValue);
            }

            return dValue;
        }


        public override void OnInitializeContent(IntegerTextBox uiElement, GridRenderStyleInfo style)
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
            var font = style.ReadOnlyFont;
            uiElement.Padding = margins;
            uiElement.FontFamily = font.FontFamily;
            uiElement.FontSize = font.FontSize;
            uiElement.FontStretch = font.FontStretch;
            uiElement.FontWeight = font.FontWeight;
            uiElement.FontStyle = font.FontStyle;
            uiElement.Foreground = style.Foreground;
            uiElement.Background = style.Background;
            uiElement.HorizontalAlignment = style.HorizontalAlignment;
            uiElement.TextAlignment = this.HorizontalAlignmentToTextAlignment(style.HorizontalAlignment);
            uiElement.VerticalAlignment = style.VerticalAlignment;
            uiElement.UseNullOption = style.IntegerEdit.UseNullOption;

            //uiElement.MinValue = style.IntegerEdit.MinValue;
            //uiElement.MaxValue = style.IntegerEdit.MaxValue;
            if (!(style.IntegerEdit.HasMinValidation && style.IntegerEdit.MinValidation == MinValidation.OnLostFocus))
            {
                if (style.IntegerEdit.HasMinValue)
                    uiElement.MinValue = style.IntegerEdit.MinValue;
            }
            if (!(style.IntegerEdit.HasMaxValidation && style.IntegerEdit.MaxValidation == MaxValidation.OnLostFocus))
            {
                if (style.IntegerEdit.HasMaxValue)
                    uiElement.MaxValue = style.IntegerEdit.MaxValue;
            }
            uiElement.IsScrollingOnCircle = style.IntegerEdit.HasIsScrollingOnCircle ? style.IntegerEdit.IsScrollingOnCircle : uiElement.IsScrollingOnCircle;
            if (this.IsCurrentCell(style) && HasControlValue)
            {
                uiElement.Value = Convert.ToInt64(this.ControlValue);
            }
            else
            {
				//uiElement.Value = Convert.ToInt64(GetControlValue(style));
                var value = GetIntegerValue(style);
                if (value != Int64.MinValue)
                    uiElement.Value = value; //Convert.ToInt64(GetControlValue(style));
                else
                {
                    //uiElement.UseNullOption = true;
                    uiElement.Value = null;
                }
            }

            var numberFormatInfo = style.HasNumberFormat ? style.NumberFormat : style.GetCulture(false).NumberFormat;
            uiElement.NumberGroupSeparator = numberFormatInfo.NumberGroupSeparator;
            uiElement.NumberGroupSizes = numberFormatInfo.NumberGroupSizes;

            uiElement.BorderThickness = new Thickness(0);
            VisualContainer.SetWantsMouseInput(uiElement, false);
            this.OnWireUIElement(uiElement);
        }

        protected override void ArrangeUIElement(ArrangeCellArgs aca, IntegerTextBox uiElement, GridRenderStyleInfo style)
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

        protected override void OnWireUIElement(IntegerTextBox uiElement)
        {
            base.OnWireUIElement(uiElement);
            uiElement.ValueChanged += new PropertyChangedCallback(uiElement_ValueChanged);
            uiElement.TextChanged += new TextChangedEventHandler(uiElement_TextChanged);
            uiElement.KeyDown += new KeyEventHandler(uiElement_KeyDown);
        }

        /// <summary>
        /// This event fires while text changed in the UIElement
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void uiElement_TextChanged(object sender, TextChangedEventArgs e)
        {
            var currencyEdit = (IntegerTextBox)sender;
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


       
        /// <summary>
        /// This event fires while Key Down on the UIElement
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void uiElement_KeyDown(object sender, KeyEventArgs e)
        {
             bool isShiftKey = (Keyboard.Modifiers & ModifierKeys.Shift) != ModifierKeys.None;
            
            IntegerTextBox textBox = (IntegerTextBox)sender;
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
        
            IntegerTextBox tb = CurrentCellUIElement;
            Int64 value;
            if (Int64.TryParse(e.Text, out value))
            {
                CurrentCellUIElement.Value = value;
           
            }
          
            e.Handled = true;         
        }       

        void uiElement_ValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            IntegerTextBox textBox = (IntegerTextBox)d;
            if (!this.IsInArrange && this.IsCurrentCell(textBox) && !this.CurrentCell.IsInEndEdit)
            {
                if (!this.SetControlValue(textBox.Value))
                {
                    RefreshContent();
                }
            }
        }

        protected override string GetControlTextFromEditorCore(IntegerTextBox uiElement)
        {
            return uiElement.Value.ToString();
        }


        private void UpdateIntegerEdit()
        {
            if (CurrentCellUIElement != null)
            {				
                if (this.ControlValue != null && this.ControlValue.ToString() != string.Empty)
                {
                    CurrentCellUIElement.Value = Convert.ToInt64(this.ControlValue);
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
                GridControl.InvalidateVisual(true);
            }
        }

        protected override void OnEnteredEditMode()
        {
            UpdateIntegerEdit();
        }

        protected override void OnDeactivated()
        {
            UpdateIntegerEdit();
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

        protected override void OnActivated()
        {
            UpdateIntegerEdit();
        }

        protected override void OnEditingComplete()
        {
            UpdateIntegerEdit();
            GridControl.InvalidateCell(CellRowColumnIndex);
            if (this.isEnteryKeyPressed)
            {
                GridControl.InvalidateVisual(true);
            }
        }

        protected override void OnSetFocus()
        {
            if (this.GridControl.Model.Options.ActivateCurrentCellBehavior == GridCellActivateAction.SelectAll)
                CurrentCellUIElement.SelectAll();
          
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
                        this.CurrentCell.MoveRight();
                        CurrentCell.ScrollInView();
                        e.Handled = true;
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
                            {
                                this.CurrentCell.MoveRight();
                                e.Handled = true;
                            }
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
                case Key.Up:
                    if (this.CurrentCell.IsEditing)
                    {
                        IntegerTextBox tb = this.CurrentCellUIElement;
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

        protected override void OnUnwireUIElement(IntegerTextBox uiElement)
        {
            base.OnUnwireUIElement(uiElement);
            uiElement.ValueChanged -= new PropertyChangedCallback(uiElement_ValueChanged);
            uiElement.KeyDown -= new KeyEventHandler(uiElement_KeyDown);
            uiElement.TextChanged -= new TextChangedEventHandler(uiElement_TextChanged);
        }
    }
}
