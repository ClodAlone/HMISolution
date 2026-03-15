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
    using Syncfusion.Windows.Tools.Controls;

    public class GridCellPercentEditCellModel : GridCellModel<GridCellPercentEditCellRenderer>
    {
        public GridCellPercentEditCellModel()
        {
        }

        public override string GetText(GridStyleInfo style, object value)
        {
            return (value != null && !(value is DBNull)) ? value.ToString() : string.Empty;
        }

        public override string GetFormattedText(GridStyleInfo style, object value, int textInfo)
        {
            Double dValue = 0;
            string retString = string.Empty;
            string text = this.GetText(style, value);
            var numberFormat = style.NumberFormat;

            if (Double.TryParse(text, NumberStyles.Any, numberFormat, out dValue))
            {
                dValue /= 100.0;
                retString = dValue.ToString("P", numberFormat);
            }

            return retString;
        }
    }

    public class GridCellPercentEditCellRenderer : GridVirtualizingCellRenderer<PercentTextBox>
    {
        public GridCellPercentEditCellRenderer()
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

        public override void OnInitializeContent(PercentTextBox uiElement, GridRenderStyleInfo style)
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
            var culture = style.GetCulture(true);
            var numberformat = culture.NumberFormat;

            uiElement.MinValue = style.PercentEdit.MinValue;
            uiElement.MaxValue = style.PercentEdit.MaxValue;
            uiElement.UseNullOption = style.PercentEdit.UseNullOption;

            if (style.CellValue == null)
                style.CellValue = style.PercentEdit.MinValue;

            var numberFormatInfo = style.HasNumberFormat ? this.CurrentStyle.NumberFormat : this.CurrentStyle.GetCulture(false).NumberFormat;

            uiElement.PercentageSymbol = numberFormatInfo.PercentSymbol;
            uiElement.PercentGroupSizes = numberFormatInfo.PercentGroupSizes;
            uiElement.PercentDecimalDigits = numberFormatInfo.PercentDecimalDigits;
            uiElement.PercentDecimalSeparator = numberFormatInfo.PercentDecimalSeparator;
            uiElement.PercentGroupSeparator = numberFormatInfo.PercentGroupSeparator;                
            
         

            var value = this.GetControlValue(style);
            if (value != null && value.ToString() != string.Empty)
            {
                uiElement.PercentValue = Convert.ToDouble(value);
            }
            else
            {
                uiElement.PercentValue = null;
                uiElement.Text = string.Empty;
            }

            uiElement.BorderThickness = new Thickness(0);
            VisualContainer.SetWantsMouseInput(uiElement, false);
            this.OnWireUIElement(uiElement);
        }

        protected override void ArrangeUIElement(Cells.ArrangeCellArgs aca, PercentTextBox uiElement, GridRenderStyleInfo style)
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

        protected override void OnWireUIElement(PercentTextBox uiElement)
        {
            base.OnWireUIElement(uiElement);
            uiElement.PercentValueChanged += new PropertyChangedCallback(uiElement_PercentValueChanged);
            uiElement.TextChanged += new TextChangedEventHandler(uiElement_TextChanged);
            uiElement.KeyDown += new KeyEventHandler(uiElement_KeyDown);
        }

        /// <summary>
        /// This Event fires while Key press on the UIElement
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        void uiElement_KeyDown(object sender, KeyEventArgs e)
        {
            bool isShiftKey = (Keyboard.Modifiers & ModifierKeys.Shift) != ModifierKeys.None;
            PercentTextBox textBox = (PercentTextBox)sender;
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

        /// <summary>
        /// This Event fires while Text Changed in the UIElement
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void uiElement_TextChanged(object sender, TextChangedEventArgs e)
        {
            var tb = (PercentTextBox)sender;
            if (!this.IsInArrange && this.IsCurrentCell(tb) && !this.CurrentCell.IsInEndEdit)
            {
                if (!this.SetControlValue(tb.PercentValue))
                {
                    RefreshContent();
                }
            }
            if (this.GridControl.Model.Options.ActivateCurrentCellBehavior != GridCellActivateAction.SelectAll)
            {
                if (tb.SelectionLength == tb.Text.Length)
                    tb.Select(tb.PercentValue.ToString().Length, 0);
            }
        }

        protected override string GetControlTextFromEditorCore(PercentTextBox uiElement)
        {
            return uiElement.PercentValue.ToString();
        }


        protected override void OnGridPreviewTextInput(TextCompositionEventArgs e)
        {
            if (CurrentCell.IsEditing) return;
            CurrentCell.ScrollInView();
            CurrentCell.BeginEdit(true);
            if (CurrentCellUIElement == null) return;

            PercentTextBox tb = this.CurrentCellUIElement;
            Int64 value;
            if (Int64.TryParse(e.Text, out value))
            {
                tb.PercentValue = Convert.ToDouble(value);             
            }
            e.Handled = true;          
        }

        void uiElement_PercentValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PercentTextBox percentTextBox = (PercentTextBox)d;
            if (!this.IsInArrange && IsCurrentCell(percentTextBox) && !CurrentCell.IsInEndEdit)
            {
                if (!SetControlValue(percentTextBox.PercentValue))
                {
                    RefreshContent();
                }
            }
        }

        private void UpdatePercentEdit()
        {
            if (CurrentCellUIElement != null)
            {
                CurrentCellUIElement.PercentValue = Convert.ToDouble(this.ControlValue);
            }
            else
            {
                GridControl.InvalidateCell(CellRowColumnIndex);
                GridControl.InvalidateVisual(true);
            }
        }

        protected override void OnEnteredEditMode()
        {
            UpdatePercentEdit();
        }

        protected override void OnActivated()
        {
            UpdatePercentEdit();
        }

        protected override void OnEditingComplete()
        {
            UpdatePercentEdit();
            GridControl.InvalidateCell(CellRowColumnIndex);
            if (this.isEnteryKeyPressed)
            {
                GridControl.InvalidateVisual(true);
            }
        }

        protected override void OnDeactivated()
        {
            UpdatePercentEdit();
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

        protected override void OnSetFocus()
        {
            if (!ActivateOptions.IsActivateTriggeredByMouseDownIntoUIElement)
            {
                CurrentCellUIElement.SelectAll();
            }
        }


        protected override void OnUnwireUIElement(PercentTextBox uiElement)
        {
            base.OnUnwireUIElement(uiElement);
            uiElement.PercentValueChanged -= new PropertyChangedCallback(uiElement_PercentValueChanged);
            uiElement.TextChanged -= new TextChangedEventHandler(uiElement_TextChanged);
            uiElement.KeyDown -= new KeyEventHandler(uiElement_KeyDown);
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
                case Key.Left:
                case Key.Down:
                case Key.Up:
                    {
                        CurrentCell.EndEdit();
                        return !CurrentCell.IsEditing;
                    }

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

    }
}







