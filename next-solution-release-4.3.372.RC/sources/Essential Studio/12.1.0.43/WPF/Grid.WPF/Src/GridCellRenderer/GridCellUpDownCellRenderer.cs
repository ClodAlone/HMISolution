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
    using System.Windows;
    using Syncfusion.Windows.Shared;
    using System.Globalization;
    using System.Windows.Input;
    using Syncfusion.Windows.Controls.Scroll;
    using System.Windows.Controls;
    using Syncfusion.Windows.GridCommon;
    using System.Windows.Media;
    using System.Reflection;

    /// <summary>
    /// Implements the model part of an up down cell.
    /// </summary>
    public class GridCellUpDownCellModel : GridCellModel<GridCellUpDownCellRenderer>
    {
        /// <summary>
        /// Returns the display text as value object according to the specified number format.
        /// </summary>
        /// <param name="style">Style information for the cell.</param>
        /// <param name="text">The display text.</param>
        /// <returns>The formatted value of the cell.</returns> 
        public override bool ApplyFormattedText(GridStyleInfo style, string text, int textInfo)
        {
            double dValue = 0.0;

            if (double.TryParse(text, NumberStyles.Number, style.NumberFormat, out dValue))
            {
                style.CellValue = dValue;
                return true;
            }
            return false;
        }

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
        /// Return formatted text for the specified value.
        /// </summary>
        /// <param name="style">The <see cref="GridStyleInfo"/> object that holds cell information.</param>
        /// <param name="value">The value to format.</param>
        /// <param name="textInfo">TextInfo is a hint of who is calling, default is GridCellBaseTextInfo.DisplayText.</param>
        /// <returns>The formatted text for the given value.</returns>
        public override string GetFormattedText(GridStyleInfo style, object value, int textInfo)
        {
            string text = this.GetText(style, value);
            return this.GetFormattedText(text, style.NumberFormat);
        }

        protected string GetNumber(string text, NumberFormatInfo numberFormat)
        {
            string retString = String.Empty;
            int len = text.Length;

            if (text.Contains("."))
            {
                len = text.IndexOf(".");
            }

            for (int i = 0; i < len; i++)
            {
                char c = text[i];

                if (Char.IsDigit(c) || numberFormat.NegativeSign.Contains(c.ToString()))
                {
                    retString += c;
                }
            }

            return retString;
        }

        protected virtual string GetFormattedText(string text, NumberFormatInfo numberFormatInfo)
        {
            double dValue = 0.0;
            string retString = string.Empty;

            if (double.TryParse(text, NumberStyles.Number, numberFormatInfo, out dValue))
            {
                retString = String.Format(numberFormatInfo, "{0:n}", dValue);
            }

            return retString;
        }
    }

    /// <summary>
    /// Implements the renderer part of an up down cell.
    /// </summary>
    public class GridCellUpDownCellRenderer : GridVirtualizingCellRenderer<UpDown>
    {

        private UpDownPaint UpDownPaint;
        /// <summary>
        /// Initializes a new <see cref="GridCellUpDownCellRenderer"/>.
        /// </summary>
        public GridCellUpDownCellRenderer()
        {
            this.SupportsRenderOptimization = true;
            this.AllowRecycle = true;
            this.IsControlTextShown = true;
            this.IsFocusable = true;
            this.AllowKeepAliveOnlyCurrentCell = true;
            this.UpDownPaint = new UpDownPaint();
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
                margins = style.ErrorInfo.AdjustErrorInfoMargin(margins, style.GridControl, style.CellRowColumnIndex);
            }
            Rect textRectangle = rca.SubtractBorderMargins(rca.CellRect, margins);
            textRectangle = rca.SubtractBorderMargins(rca.CellRect, margins);
            //this was removed since this changes the given horizontal alignment for Updown edit.
            //style.HorizontalAlignment = HorizontalAlignment.Center;
            if (textRectangle.IsEmpty)
            {
                return;
            }

            object controlValue = null;
            if (this.IsCurrentCell(style) && this.HasControlValue)
            {
                controlValue = this.ControlValue != null ? this.ControlValue : null;
            }
            else
            {
                controlValue = this.GetControlText(style);               
            }
            double dValue = 0.0;
            var numberFormat = style.HasNumberFormat ? style.NumberFormat : style.GetCulture(false).NumberFormat;
            var text = style.CellValue != null && style.CellValue.ToString() != string.Empty ? style.CellValue.ToString() : string.Empty;
            string retString = string.Empty;
            if (double.TryParse(text, NumberStyles.Number, numberFormat, out dValue))
            {
                if (dValue < 0)
                {
                    style.Foreground = style.UpDownEdit.HasNegativeForeground ? style.UpDownEdit.NegativeForeground : style.HasNegativeForeground ? style.NegativeForeground : style.Foreground;
                } 
                retString = String.Format(numberFormat, "{0:n}", dValue);
            }
            if (retString.Equals(string.Empty) || retString == "")
            {
                retString = "0.00";
            }
            this.UpDownPaint.DrawUpdown(dc, textRectangle, retString, style);
            base.OnRender(dc, rca, style);
        }

        /// <summary>
        /// Initializes the content of the updown cell
        /// using the information from the cell style (value, text,
        /// behavior etc.).
        /// </summary>
        /// <param name="button">The UpDown control.</param>
        /// <param name="style">The cell style info.</param>
        public override void OnInitializeContent(UpDown uiElement, GridRenderStyleInfo style)
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
            uiElement.NumberFormatInfo = style.HasNumberFormat ? style.NumberFormat : style.GetCulture(false).NumberFormat;
            uiElement.NegativeForeground = style.UpDownEdit.HasNegativeForeground ? style.UpDownEdit.NegativeForeground : GridUpDownEditStyleInfo.Default.NegativeForeground;
            uiElement.FocusedBackground = style.UpDownEdit.HasFocusedBackground ? style.UpDownEdit.FocusedBackground : GridUpDownEditStyleInfo.Default.FocusedBackground;
            uiElement.FocusedForeground = style.UpDownEdit.HasFocusedForeground ? style.UpDownEdit.FocusedForeground : GridUpDownEditStyleInfo.Default.FocusedForeground;
            uiElement.FocusedBorderBrush = style.UpDownEdit.HasFocusedBorderBrush ? style.UpDownEdit.FocusedBorderBrush : GridUpDownEditStyleInfo.Default.FocusedBorderBrush;

            if (style.UpDownEdit.HasStep)
            {
                uiElement.Step = style.UpDownEdit.Step;
            }
            if (style.UpDownEdit.HasMaxValue)
            {
                uiElement.MaxValue = style.UpDownEdit.MaxValue;
            }
            if (style.UpDownEdit.HasMinValue)
            {
                uiElement.MinValue = style.UpDownEdit.MinValue;
            }            
            //uiElement.MinValue = style.UpDownEdit.HasMinValue ? style.UpDownEdit.MinValue : 0;
            uiElement.AnimationSpeed = style.UpDownEdit.HasAnimationSpeed ? style.UpDownEdit.AnimationSpeed : 0.1d;
            uiElement.UseNullOption = false;
           // double dValue = 0.0;
            //var text = style.CellValue != null ? style.CellValue.ToString() : string.Empty;
            double dValue = GetUpDownValue(style);
            //if (double.TryParse(text, NumberStyles.Number, uiElement.NumberFormatInfo, out dValue))
            if(dValue!=double.MinValue)
            {
                uiElement.Value = dValue;
            }
            else
            {
                uiElement.UseNullOption = true;
                uiElement.Value = null;
                uiElement.NullValueText = string.Empty;
            }
            uiElement.Background = Brushes.White;
            uiElement.Foreground = Brushes.Black;
            if (style.FlowDirection == FlowDirection.RightToLeft)
            {
                uiElement.FlowDirection = style.FlowDirection;
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
            //this has been included for to change the style's horizontal aliagnment as Text alignment.
            uiElement.TextAlignment = HorizontalAlignmentToTextAlignment(style.HorizontalAlignment);
            OnWireUIElement(uiElement);
        }

        protected override void ArrangeUIElement(Cells.ArrangeCellArgs aca, UpDown uiElement, GridRenderStyleInfo style)
        {
            Thickness margins = style.TextMargins.ToThickness();
            margins = style.ErrorInfo.AdjustErrorInfoMarginOnEditing(margins, style.GridControl, style.CellRowColumnIndex);
            uiElement.Padding = margins;

            base.ArrangeUIElement(aca, uiElement, style);
        }

        private double GetUpDownValue(GridRenderStyleInfo style)
        {
            double dValue = double.MinValue;
            var value = this.GetControlValue(style);
            if (value != null && value.ToString() != string.Empty)
            {
                double.TryParse(value.ToString(), out dValue);
            }

            return dValue;
        }


        protected override void OnInitialize()
        {
            base.OnInitialize();
            this.ControlValue = this.GetControlValueFromEditor();
        }

        protected override void OnGridPreviewTextInput(TextCompositionEventArgs e)
        {
            if (this.CurrentCell.IsEditing)
            {
                return;
            }

            this.CurrentCell.ScrollInView();
            this.CurrentCell.BeginEdit(true);
        }

        protected override void OnEnteredEditMode()
        {
            this.UpdateUpDownControl();
        }

        protected override void OnActivated()
        {
            this.UpdateUpDownControl();
            if (this.HasCurrentCellState && !CurrentCell.IsEditing)
            {
                RaiseBeginEdit();
            }
        }

        protected override void OnEditingComplete()
        {
            this.GridControl.InvalidateCell(this.CellRowColumnIndex);
        }

        protected override void OnDeactivated()
        {
            this.GridControl.InvalidateCell(this.CellRowColumnIndex);
        }

        private void UpdateUpDownControl()
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
            }
        }

        protected override object GetControlValueFromEditorCore(UpDown uiElement)
        {
            return uiElement.Value;
        }

        protected override void OnWireUIElement(UpDown uiElement)
        {
            base.OnWireUIElement(uiElement);
            uiElement.ValueChanged += new PropertyChangedCallback(OnUpDownValueChanged);
            uiElement.RemoveHandler(UpDown.MouseRightButtonUpEvent, new MouseButtonEventHandler(uiElement_MouseRightButtonUp));
        }

        protected override void OnUnwireUIElement(UpDown uiElement)
        {
            base.OnUnwireUIElement(uiElement);
            uiElement.ValueChanged -= new PropertyChangedCallback(OnUpDownValueChanged);
            uiElement.AddHandler(UpDown.MouseRightButtonUpEvent, new MouseButtonEventHandler(uiElement_MouseRightButtonUp), true);
        }

        void uiElement_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.GridControl.Model.DisableEditorsContextMenu)
            {
                e.Handled = true;
            }
        }

        void OnUpDownValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var upDown = (UpDown)d;
            if (!this.IsInArrange && this.IsCurrentCell(upDown) && !this.CurrentCell.IsInEndEdit)
            {
                if (!this.SetControlValue(upDown.Value))
                {
                    RefreshContent();
                }
            }
        }

        protected override string GetControlTextFromEditorCore(UpDown uiElement)
        {
            return uiElement.Value != null ? uiElement.Value.ToString() : string.Empty;
        }


        //public override bool ValidateControlValue(object value)
        //{
        //    var style = this.CurrentStyle;
        //    var maxValue = style.UpDownEdit.HasMaxValue ? style.UpDownEdit.MaxValue : 100;
        //    var minValue = style.UpDownEdit.HasMinValue ? style.UpDownEdit.MinValue : 0;
        //    double result;

        //    if (value != null)
        //    {
        //        if (double.TryParse(value.ToString(), out result))
        //        {
        //            if (result <= maxValue && result >= minValue)
        //            {
        //                return true;
        //            }
        //        }
        //    }
        //    else
        //    {
        //        return true;
        //    }

        //    return false;
        //}

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
                        if (isShiftKey)
                        {
                            return false;
                        }
                        UpDown updowncontrol = this.CurrentCellUIElement as UpDown;
                        var doubletextbox = updowncontrol.FindElementsOfType<DoubleTextBox>();
                        DoubleTextBox Doubletextbox = doubletextbox.Where(o => o != null && o.Name == "DoubleTextBox").FirstOrDefault();
                        if (Doubletextbox != null)
                        {
                            if (Doubletextbox.CaretIndex == 0 && Doubletextbox.SelectionLength == 0)
                            {
                                e.Handled = true;
                                return true;
                            }// When the entire text is selected pressing left arrow key moves the focus to the previous cell, whereas in Excel the cursor moves to the 0th index. The below code is added to move the cursor to the 0th Index
                            else if (Doubletextbox.SelectionLength == Doubletextbox.Text.Length)
                            {
                                Doubletextbox.CaretIndex = 0;
                                e.Handled = true;
                                return false;
                            }
                            else
                                return false;
                        }
                        else
                            return false;
                    }
                    return true;
                case Key.Right:
                    if (this.CurrentCell.IsEditing)
                    {
                        if (isShiftKey)
                        {
                            return false;
                        }
                        UpDown updowncontrol = this.CurrentCellUIElement as UpDown;
                        var doubletextbox = updowncontrol.FindElementsOfType<DoubleTextBox>();
                        DoubleTextBox Doubletextbox = doubletextbox.Where(o => o != null && o.Name == "DoubleTextBox").FirstOrDefault();
                        if (Doubletextbox != null)
                        {
                            if (Doubletextbox.CaretIndex == 0 && Doubletextbox.SelectionLength == 0)
                            {
                                //e.Handled = true;
                                return false;
                            } // When the entire text is selected pressing right arrow key moves the focus to the next cell, whereas in Excel the cursor moves to the last position. The below code is added to move the cursor to the last position
                            else if (Doubletextbox.SelectionLength == Doubletextbox.Text.Length)
                            {
                                Doubletextbox.CaretIndex = Doubletextbox.Text.Length;
                                e.Handled = true;
                                return false;
                            }
                            else if (Doubletextbox.CaretIndex == Doubletextbox.Text.Length)
                                return true;
                            else
                                return false;
                        }
                        else
                            return false;
                    }
                    return true;

                case Key.Down:
                case Key.Up:
                    {
                        if (this.CurrentCell.IsEditing)
                            return false;
                        else
                            return true;
                    }

                case Key.End:
                case Key.Home:
                    {
                        if (this.CurrentCell.IsEditing)
                        {
                            return false;
                        }
                        else
                        {
                            return true;
                        }
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
                        //    {
                        //      CurrentCell.EndEdit();
                        //      e.Handled = true;
                        //    }
                        CurrentCell.MoveRight();
                       return true;
                    }
                case Key.F2:
                    {
                        if (CurrentCell.IsEditing)
                        {
                            CurrentCell.EndEdit(); e.Handled = true;
                            return false;
                        }
                        else
                        {
                            CurrentCell.BeginEdit(true); e.Handled = true;
                            return false;
                        }
                    }
            }

            return base.ShouldGridTryToHandlePreviewKeyDown(e);
        }

        /// <summary>
        /// Raises GridCellClick event.
        /// </summary>
        /// <param name="rowIndex">The cell row index.</param>
        /// <param name="colIndex">The cell column index.</param>
        /// <param name="e">A <see cref="MouseControllerEventArgs"/> object.</param>
        public override void RaiseGridCellClick(int rowIndex, int colIndex, MouseControllerEventArgs e)
        {
            if (CurrentCell.HasCurrentCellAt(rowIndex, colIndex))
            {
                if (!CurrentCell.IsEditing && ((GridControl.Model.Options.ActivateCurrentCellBehavior & GridCellActivateAction.DblClickOnCell) == 0)
                    && (GridControl.Model.Options.ActivateCurrentCellBehavior != GridCellActivateAction.None))
                {
                    CurrentCell.BeginEdit(true);
                }
                if (HasCurrentCellState && this.GridControl.Model.Options.ActivateCurrentCellBehavior == GridCellActivateAction.ClickOnCell)
                {
                    if (this.CurrentCellUIElement != null)
                        CurrentCellUIElement.Focus();
                }
            }
            base.RaiseGridCellClick(rowIndex, colIndex, e);
        }
    }

    public class UpDownPaint
    {
        private Dictionary<Size, VisualBrush> brushes = new Dictionary<Size, VisualBrush>();
        private string VisualStyle = string.Empty;
        //Thickness at right side is increased to avoid the overlap of text with the updown thump.
        private Thickness margin = new Thickness(0, 0, 20, 0);

        /// <summary>
        /// Default constructor.
        /// </summary>
        public UpDownPaint()
        {
        }

        /// <summary>
        /// Gets or sets the margins.
        /// </summary>
        public Thickness Margin
        {
            get { return this.margin; }
            set { this.margin = value; }
        }

        /// <summary>
        /// Draws the UpDown Control in the cell rectangle.
        /// </summary>
        /// <param name="dc">The drawing context.</param>
        /// <param name="rc">Cell rectangle.</param>
        /// <param name="text">Text to be drawn over the UpDown Control.</param>
        /// <param name="style">Cell style information.</param>
        /// <returns>Cell margins.</returns>
        public Thickness DrawUpdown(DrawingContext dc, Rect rc, string text, GridStyleInfo style)
        {
            VisualBrush vb = this.GetVisualBrush(rc.Size, style);
            dc.DrawRectangle(vb, null, rc);
            rc = this.SubtractBorderMargins(rc, this.margin);
            GridTextBoxPaint.DrawText(dc, rc, text, style);
            return this.margin;
        }

        /// <summary>
        /// Removes border margins to get cells client area.
        /// </summary>
        /// <param name="cellRect">Cell rectangle.</param>
        /// <param name="mi">The border margins.</param>
        /// <returns>Returns the cells client area.</returns>
        public Rect SubtractBorderMargins(Rect cellRect, Thickness mi)
        {
            if (cellRect.IsEmpty || cellRect.Width <= mi.Left + mi.Right || cellRect.Height <= mi.Top + mi.Bottom)
            {
                return new Rect(0, 0, 0, 0);
            }
            cellRect.Height -= mi.Bottom + mi.Top;
            cellRect.Y += mi.Top;
            cellRect.Width -= mi.Right + mi.Left;
            cellRect.X += mi.Left;
            return cellRect;
        }

    
        private VisualBrush GetVisualBrush(Size size, GridStyleInfo style)
        {
            if (style.GridModel is GridDataTableModel)
            {
                var gridDataTableModel = style.GridModel as GridDataTableModel;
                var canRecycle = gridDataTableModel.TableProperties.VisualStyle.ToString().Equals(VisualStyle);
                if (!canRecycle)
                    brushes.Clear();
                VisualStyle = gridDataTableModel.TableProperties.VisualStyle.ToString(); 
            }
            if (this.brushes.ContainsKey(size))
            {
                return this.brushes[size];
            }

            bool wasAnimated = GridUtil.IsAnimated;
            try
            {
                GridUtil.IsAnimated = false;

                VisualBrush visualBrush;
                UpDown b = new UpDown();
                b.Background = Brushes.Transparent;
                b.BorderThickness = new Thickness(0);
                b.BeginInit();
                b.Width = size.Width;
                b.Height = size.Height;
                b.EndInit();
                b.Measure(size);
                b.Arrange(new Rect(size));
                visualBrush = new VisualBrush();
                visualBrush.Visual = b;
                this.brushes[size] = visualBrush;
                return visualBrush;
            }
            finally
            {
                GridUtil.IsAnimated = wasAnimated;
            }
        }
    }
}
