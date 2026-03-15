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
    using System.Globalization;
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


    public class GridCellUpDownEditCellModel : GridCellModel<GridCellUpDownEditCellRenderer>
    {
        public GridCellUpDownEditCellModel()
        {
        }

        public override string GetText(GridStyleInfo style, object value)
        {
            return (value != null && !(value is DBNull)) ? value.ToString() : string.Empty;
        }

        public override string GetFormattedText(GridStyleInfo style, object value, int textInfo)
        {
            decimal dValue = 0;
            string retString = string.Empty;
            string text = this.GetText(style, value);
            var culture = style.GetCulture(true);
            var numberFormat = culture.NumberFormat;

            if (decimal.TryParse(text, NumberStyles.Number, numberFormat, out dValue))
            {
                retString = dValue.ToString("N", numberFormat);
            }

            return retString;
        }
    }

    public class GridCellUpDownEditCellRenderer : GridVirtualizingCellRenderer<NumericUpDown>
    {
        public GridCellUpDownEditCellRenderer()
        {
            this.AllowRecycle = true;
            //this.IsControlTextShown = true;
            this.SupportsRenderOptimization = true;
            this.IsFocusable = true;
            this.AllowKeepAliveOnlyCurrentCell = true;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            this.ControlValue = this.GetControlValueFromEditor();
        }

        public override void OnInitializeContent(NumericUpDown uiElement, GridRenderStyleInfo style)
        {
            base.OnInitializeContent(uiElement, style);
            this.OnUnwireUIElement(uiElement);
            Thickness margins = style.TextMargins.ToThickness();
            margins.Left = Math.Max(0, margins.Left - 2);
            margins.Right = Math.Max(0, margins.Right - 2);
            if (style.HasErrorInfo)
            {
                margins = style.AdjustImageWidthAndHeightToMargin(margins, style.GridControl);
            }
            else
            {
                margins = style.ErrorInfo.AdjustErrorInfoMarginOnEditing(margins, style.GridControl, style.CellRowColumnIndex);
            }
            uiElement.Padding = margins;
            uiElement.BorderThickness = new Thickness(0);
            var nformat = style.HasNumberFormat ? style.NumberFormat : style.GetCulture(false).NumberFormat;
            uiElement.DecimalPlaces = nformat.NumberDecimalDigits;
            if(style.UpDownEdit.HasMaxValue)
                uiElement.Maximum = style.UpDownEdit.MaxValue;
            if(style.UpDownEdit.HasMinValue)
                uiElement.Minimum = style.UpDownEdit.MinValue;
            if(style.UpDownEdit.HasStep)
                uiElement.Increment = style.UpDownEdit.Step;

            double dValue = 0.0;
            var text = style.CellValue != null ? style.CellValue.ToString() : string.Empty;
            if (double.TryParse(text, NumberStyles.Number, nformat, out dValue))
            {
                uiElement.Value = dValue;
            }

            if (style.CellValue == null)
                style.CellValue = style.UpDownEdit.MinValue;

            VisualContainer.SetWantsMouseInput(uiElement, false);
            this.OnWireUIElement(uiElement);
        }

        protected override void ArrangeUIElement(Cells.ArrangeCellArgs aca, NumericUpDown uiElement, GridRenderStyleInfo style)
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

        protected override object GetControlValueFromEditorCore(NumericUpDown uiElement)
        {
            return uiElement.Value;
        }

        protected override void OnWireUIElement(NumericUpDown uiElement)
        {
            base.OnWireUIElement(uiElement);
            uiElement.ValueChanged += new RoutedPropertyChangedEventHandler<double>(UpDown_ValueChanged);            
        }

        void UpDown_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var upDown = (NumericUpDown)sender;
            if (!this.IsInArrange && this.IsCurrentCell(upDown) && !this.CurrentCell.IsInEndEdit)
            {
                if (!this.SetControlValue(upDown.Value))
                {
                    RefreshContent();
                }
            }
        }

        private void UpdateNumericUpDown()
        {
            if (this.CurrentCellUIElement != null)
            {
                double dValue = 0.0;
                var style = this.CurrentStyle;
                var numberFormat = style.HasNumberFormat ? style.NumberFormat : style.GetCulture(false).NumberFormat;
                var text = this.ControlValue != null && this.ControlValue.ToString() != string.Empty ? this.ControlValue.ToString() : string.Empty;
                if (double.TryParse(text, NumberStyles.Number, numberFormat, out dValue))
                {
                    this.CurrentCellUIElement.Value = dValue;
                }
                else
                {
                    this.CurrentCellUIElement.Value = 0;
                }
            }
            else
            {
                this.GridControl.InvalidateCell(this.CellRowColumnIndex);
                this.GridControl.InvalidateVisual(true);
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
            
            if (this.CurrentCellUIElement == null) return;
           
            double dValue = 0.0;
           NumericUpDown tb=this.CurrentCellUIElement as NumericUpDown;
            if (double.TryParse(e.Text,out dValue))
            {
                tb.Value = dValue;
            }
            e.Handled=true;
        }

        protected override void OnEnteredEditMode()
        {
            this.UpdateNumericUpDown();
        }

        protected override void OnActivated()
        {
            this.UpdateNumericUpDown();
        }

        protected override void OnEditingComplete()
        {
            this.GridControl.InvalidateCell(this.CellRowColumnIndex);
            this.GridControl.InvalidateVisual(true);
        }

        protected override void OnDeactivated()
        {
            this.GridControl.InvalidateCell(this.CellRowColumnIndex);
            this.GridControl.InvalidateVisual(true);
        }

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
                        if (this.GridControl is GridControl)
                        {
                            if (this.GridControl.Model.Options.EnterKeyBehaviour == EnterKeyBehaviour.MouseDown)
                                this.CurrentCell.MoveDown();
                            else
                                this.CurrentCell.MoveRight();
                            CurrentCell.ScrollInView();
                            return true;
                        }
                        this.CurrentCell.MoveRight();
                        CurrentCell.ScrollInView();
                        return true;
                    }
                    // break; Unreachable code
                case Key.Tab:
                    return true;

                case Key.Right:
                case Key.Left:
                case Key.Down:
                case Key.Up:
                    {
                        // otherwise, move caret within textbox when cell is not in edit-mode.
                        //CurrentCell.EndEdit();
                        return !this.CurrentCell.IsEditing;
                    }

                case Key.End:
                    return true;
                case Key.Home:
                    if (this.CurrentCell.IsEditing)
                        return false;
                    else
                        return true;
            }

            return base.ShouldGridTryToHandlePreviewKeyDown(e);
        }

        protected override void OnUnwireUIElement(NumericUpDown uiElement)
        {
            base.OnUnwireUIElement(uiElement);
            uiElement.ValueChanged -= new RoutedPropertyChangedEventHandler<double>(UpDown_ValueChanged);
        }
    }
}
