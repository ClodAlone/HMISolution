#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using Syncfusion.Windows.Controls.Cells;
using Syncfusion.Windows.Controls.Scroll;
using Syncfusion.Windows.GridCommon;
using Syncfusion.Windows.Diagnostics;
using Syncfusion.Windows.Shared;
using System.Globalization;
using System.Windows.Shapes;

namespace Syncfusion.Windows.Controls.Grid
{
    /// <summary>
    /// Implements the model part of a text box cell.
    /// </summary>
    public class GridCellTextBoxModel : GridCellModel<GridCellTextBoxRenderer>
    {
        /// <summary>
        /// Calculates the preferred size of the cell based on its content, including cell margins. 
        /// </summary>
        /// <param name="rowIndex">Cell row index.</param>
        /// <param name="colIndex">Cell column index.</param>
        /// <param name="style">Cell style information.</param>
        /// <param name="queryBounds">Graphical bounds.</param>
        /// <returns>The optimal size of the cell.</returns>
        public override Size CalculatePreferredCellSize(int rowIndex, int colIndex, GridStyleInfo style, GridQueryBounds queryBounds)
        {
            Size clientSize = OnQueryPrefferedClientSize(rowIndex, colIndex, style, queryBounds);
            if (clientSize.IsEmpty)
                return Size.Empty;

            Thickness margins = style.TextMargins.ToThickness();
            if (style.HasImageIndex)
            {
                margins = style.AdjustImageWidthAndHeightToMargin(margins, clientSize);
            }
            else
            {
                margins = style.ErrorInfo.AdjustErrorInfoMargin(margins, clientSize);
            }
            // TextBoxView always seems to have this margin and I am not able to reset the margin.
            // Therefore I am also hard-codeing it here so that TextBox behavior is properly
            // emulated.
            margins.Left = Math.Max(margins.Left, 2);
            margins.Right = Math.Max(margins.Right, 2);

            if (style.HorizontalAlignment == HorizontalAlignment.Left && style.ErrorInfo.HasErrorMessage && style.ErrorInfo.ErrorContentAlignment == ImageContentAlignment.Left)
            {
                margins.Left = 20;
            }

            Size size = AddBorderMargins(clientSize, margins);
            size = AddBorderMargins(size, style.BorderMargins.ToThickness());
            size = AddBorderMargins(size, style.Padding.ToThickness());
            return size;
        }

        public override string GetFormattedText(GridStyleInfo style, object value, int textInfo)
        {
            if (!string.IsNullOrEmpty(style.Format) && style.FormatProvider == null)
            {
                CultureInfo ci = style.GetCulture(true);

                double d;
                if (double.TryParse(style.Text, NumberStyles.Number | NumberStyles.AllowExponent, ci.NumberFormat, out d))
                {
                    return Syncfusion.Windows.Styles.ValueConvert.FormatValue(d, typeof(double), style.Format, ci, ci.NumberFormat, style.FormatProvider);
                }
            }
            return base.GetFormattedText(style, value, textInfo);
        }
    }

    /// <summary>
    /// Implements the renderer part of a text box cell.
    /// </summary>
    /// <remarks>
    /// Use "TextBox" as identifier in <see cref="GridStyleInfo.CellType"/> of a cells <see cref="GridStyleInfo"/>
    /// to associate this cell type with a cell.
    /// </summary>
    public class GridCellTextBoxRenderer : GridVirtualizingCellRenderer<TextBox>
    {
        int textBoxSelectionStart = -1;
        int textBoxSelectionLength = 0;
        int ccSelectionStart, ccSelectionLength;

        /// <summary>
        /// Initializes a new <see cref="GridCellTextBoxRenderer"/>.
        /// </summary>
        public GridCellTextBoxRenderer()
        {
            SupportsRenderOptimization = true;
            AllowRecycle = true;
            IsControlTextShown = true;
            IsFocusable = true;
            AllowKeepAliveOnlyCurrentCell = true;
            // TextBoxView always seems to have this margin and I am not able to reset the margin.
            // Therefore I am also hard-coding it here so that TextBox behavior is properly
            // emulated.
            //SmallestMargins = new Thickness(2, 0, 2, 0);
        }


        protected override void OnRender(DrawingContext dc, RenderCellArgs rca, GridRenderStyleInfo style)
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

            /// If we resize the column means Text will displace from its origional.
            /// Because While increasing the column size margin.left also increase.
            /// So I have set the Constant value for margin.Left
            if (style.HorizontalAlignment == HorizontalAlignment.Left && style.ErrorInfo.HasErrorMessage && style.ErrorInfo.ErrorContentAlignment == ImageContentAlignment.Left)
            {
                margins.Left = 20;
            }

            textRectangle = rca.SubtractBorderMargins(rca.CellRect, margins);

            if (textRectangle.IsEmpty)
                return;

            string text;
            if (IsCurrentCell(style) && HasControlText && this.CurrentCell.IsEditing)
                text = ControlText;
            else
                text = GetControlText(style);

            double dValue = 0;

            if (style.CellValue != null && double.TryParse(style.CellValue.ToString(), out dValue))
            {
                if (dValue < 0)
                {
                    style.Foreground = style.HasNegativeForeground ? style.NegativeForeground : style.Foreground;
                }
            }

            if (text.Length != 0 && text.Length > style.MaxLength && style.HasMaxLength)
                text = text.Remove(style.MaxLength);
            // Draw the formatted text string to the DrawingContext of the control.
            GridTextBoxPaint.DrawText(dc, textRectangle, text, style);
        }


        /// <summary>
        /// Initializes the content of the textbox cell
        /// using the information from the cell style (value, text,
        /// behavior etc.).
        /// </summary>
        /// <param name="textBox">The text box.</param>
        /// <param name="style">The cell style info.</param
        public override void OnInitializeContent(TextBox textBox, GridRenderStyleInfo style)
        {
            base.OnInitializeContent(textBox, style);
            this.GridControl.Model.QueryCoveredRange -= new GridQueryCoveredRangeEventHandler(Model_QueryCoveredRange);
            Thickness margins = style.TextMargins.ToThickness();
            // TextBoxView always seems to have this margin and I am not able to reset the margin.
            // Therefore I am also hard-codeing it here so that TextBox behavior is properly
            // emulated.
            margins.Left = Math.Max(0, margins.Left - 2);
            margins.Right = Math.Max(0, margins.Right - 2);
            if (style.HasImageIndex)
                margins = style.AdjustImageWidthAndHeightToMargin(margins, style.GridControl);
            else
            {
              //  margins = style.ErrorInfo.AdjustErrorInfoMargin(margins, style.GridControl, style.CellRowColumnIndex);
            }
            textBox.Padding = margins;

            if (IsCurrentCell(style) && HasControlText && this.CurrentCell.IsEditing)//&& style.CellType != "FormulaCell")
            {
                if (!(textBox.Text.Equals((object)this.ControlText)))
                {
                    textBox.Text = ControlText;
                }
            }
            //else
            //    textBox.Text = GetControlText(style);

            if (textBox.Text.Length != 0 && textBox.Text.Length > style.MaxLength && style.HasMaxLength)
                textBox.Text = textBox.Text.Remove(style.MaxLength);

            textBox.MaxLength = style.MaxLength;
            if (this.GridControl.Model.Options.ShowErrorIconOnEditing)
                textBox.Background = Brushes.Transparent;
            else
                textBox.Background = Brushes.White;
            textBox.Foreground = Brushes.Black;

            textBox.BorderThickness = new Thickness(0);

            if (style.FlowDirection == FlowDirection.RightToLeft)
            {
                textBox.FlowDirection = style.FlowDirection;
                double m11 = -1;
                double m22 = 1;
                double offsetX = textBox.Width;
                double offsetY = 0;
                textBox.LayoutTransform = new MatrixTransform(m11, 0, 0, m22, offsetX, offsetY); //make sure this does not leak...
            }
            else
                textBox.LayoutTransform = MatrixTransform.Identity;

            this.GridControl.Model.QueryCoveredRange += new GridQueryCoveredRangeEventHandler(Model_QueryCoveredRange);
        }

        void Model_QueryCoveredRange(object sender, GridQueryCoveredRangeEventArgs e)
        {
            //No need to conver when TextBox needs to be loaded inside one cell
            if (requiredRange.Width == 1 && requiredRange.Height == 1)
                return;

            if (requiredRange.Contains(GridRangeInfo.Cell(e.CellRowColumnIndex.RowIndex, e.CellRowColumnIndex.ColumnIndex)))
            {
                //Console.WriteLine("Top : {0} \t Bottom : {1} \t Right : {2} \t Left : {3}", requiredRange.Top, requiredRange.Bottom, requiredRange.Right, requiredRange.Left);
                e.Range = new Syncfusion.Windows.Controls.Cells.CoveredCellInfo(requiredRange.Top, requiredRange.Left, requiredRange.Bottom, requiredRange.Right);                
                e.Handled = true;   
            }
        }

       internal GridRangeInfo requiredRange = GridRangeInfo.Empty;

        double threshHold = 7;

        private GridRangeInfo GetRequiredRange(RowColumnIndex cell)
        {
            GridRangeInfo range = GridRangeInfo.Cell(cell.RowIndex, cell.ColumnIndex);
            TextBox tb = this.CurrentCell.Renderer.CurrentCellUIElement as TextBox;

            GridStyleInfo style = this.GridControl.Model[cell.RowIndex, cell.ColumnIndex];
            string text = tb != null ? tb.Text : style.FormattedText;
            Rect cellRect = this.GridControl.RangeToRect(ScrollAxisRegion.Body, ScrollAxisRegion.Body, range, false, false);
            Rect textRect = SubtractBorderMargins(cellRect, style.TextMargins.ToThickness());
            CultureInfo cultureInfo = style.GetCulture(true);
            FlowDirection flowDirection = style.FlowDirection;
            Typeface typeface = style.Typeface;
            double emSize = style.ReadOnlyFont.FontSize;
            double lineHeight = style.ReadOnlyFont.GetLineHeightValue();
            Brush foreground = style.Foreground;
            FormattedText formattedText = new FormattedText(text, cultureInfo, flowDirection, typeface, emSize, foreground);            
            int colIndex = cell.ColumnIndex + 1;            
            while (formattedText.Width + threshHold > textRect.Width && colIndex < this.GridControl.Model.ColumnCount)
            {                
                //Console.WriteLine("Column Index : ", colIndex);
                range = GridRangeInfo.Cells(cell.RowIndex, cell.ColumnIndex, cell.RowIndex, colIndex);
                colIndex++;
                cellRect = this.GridControl.RangeToRect(ScrollAxisRegion.Body, ScrollAxisRegion.Body, range, false, false);
                textRect = SubtractBorderMargins(cellRect, style.TextMargins.ToThickness());
            }
            int rowndex = cell.RowIndex + 1;
            while (formattedText.Height > textRect.Height && rowndex < this.GridControl.Model.RowCount)
            {
                range = GridRangeInfo.Cells(cell.RowIndex, cell.ColumnIndex, rowndex, colIndex - 1);
                rowndex++;
                cellRect = this.GridControl.RangeToRect(ScrollAxisRegion.Body, ScrollAxisRegion.Body, range, false, false);
                textRect = SubtractBorderMargins(cellRect, style.TextMargins.ToThickness());
            }     
            return range;
        }

        public Rect SubtractBorderMargins(Rect cellRect, Thickness mi)
        {
            if (cellRect.IsEmpty || cellRect.Width <= mi.Left + mi.Right || cellRect.Height <= mi.Top + mi.Bottom)
                return new Rect(0, 0, 0, 0);

            cellRect.Height -= mi.Bottom + mi.Top;
            cellRect.Y += mi.Top;
            cellRect.Width -= mi.Right + mi.Left;
            cellRect.X += mi.Left;

            return cellRect;
        }

        protected override void ArrangeUIElement(ArrangeCellArgs aca, TextBox textBox, GridRenderStyleInfo style)
        {
            if (style.TextWrapping != TextWrapping.NoWrap)
            {
                Rect bounds = aca.CellRect;
                if (bounds.Height < style.Font.GetLineHeightValue() * 2)
                    textBox.TextWrapping = TextWrapping.NoWrap;
            }
            if (style.Font.Orientation != 0)
            {
                textBox.RenderTransform = null;
            }

            if (style.Font.Orientation != 0)
            {
                textBox.RenderTransform = null;
            }
            Thickness margins = style.TextMargins.ToThickness();
            // TextBoxView always seems to have this margin and I am not able to reset the margin.
            // Therefore I am also hard-codeing it here so that TextBox behavior is properly
            // emulated.
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


            //Console.WriteLine(string.Format("Width : {0} \t Height :{0}", aca.CellRect.Width, aca.CellRect.Height));
            //Console.WriteLine(string.Format("required Width : {0} \t required Height :{0}", requiredRange.Width, requiredRange.Height));
            if (requiredRange.Width >= 2)
            {
                var width = aca.CellRect.Width * requiredRange.Width;
                aca.CellRect = new Rect(aca.CellRect.X, aca.CellRect.Y, width, aca.CellRect.Height);
            }

            if (requiredRange.Height >= 2)
            {
                var height = aca.CellRect.Height * requiredRange.Height;
                aca.CellRect = new Rect(aca.CellRect.X, aca.CellRect.Y, aca.CellRect.Width, height);
            }
            base.ArrangeUIElement(aca, textBox, style);
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            textBoxSelectionStart = -1;
            ControlText = GetControlText(CurrentStyle);
            
        }

        protected override void OnEnteredEditMode()
        {

            CoveredCellInfo cc = this.GridControl.CoveredCells.GetCellSpan(CurrentCell.RowIndex, CurrentCell.ColumnIndex);
            if (cc == null)
            {
                if (this.GridControl.Model.Options.EnableFloatingCell || this.CurrentStyle.EnableFloatingCell)
                    requiredRange = GetRequiredRange(this.CurrentCell.CellRowColumnIndex);
            }
            UpdateTextBoxText();

            //if (this.GridControl.Model.Options.EnableFloatingCell || this.CurrentStyle.EnableFloatingCell)
            //    requiredRange = GetRequiredRange(this.CurrentCell.CellRowColumnIndex);
            //if (CurrentCellUIElement != null)
            //{
            //    GridRenderStyleInfo style = CurrentStyle;
            //    string text = GetControlText(style);
            //    if (this.ControlValue!=null)
            //    {
            //        this.ControlValue = text;
            //    }
            //}           
        }

        protected override void OnActivated()
        {
            UpdateTextBoxText();
        }

        protected override void OnEditingComplete()
        {
            // don't update the textbox since we would be saving the changes back to some underlying value
            //UpdateTextBoxText();            
            if (this.GridControl.Model.Options.EnableFloatingCell || this.CurrentStyle.EnableFloatingCell)
            {
                this.requiredRange = GridRangeInfo.Empty;
                //this.GridControl.Model.CoveredCells.Clear();
                this.GridControl.Model.SelectedRanges.Clear();
            }
            GridControl.InvalidateCell(CellRowColumnIndex);
        }

        protected override void OnDeactivated()
        {
            UpdateTextBoxText();
            GridControl.InvalidateCell(CellRowColumnIndex);
        }

        private void UpdateTextBoxText()
        {
            if (CurrentCellUIElement != null)
            {
                GridRenderStyleInfo style = CurrentStyle;
                string text = GetControlText(style);
                CurrentCellUIElement.Text = text;
                this.ControlText = text;
                if (GridControl.Model.Options.AllowTextSelectionOnReadOnly)
                    CurrentCellUIElement.IsReadOnly = style.ReadOnly;
            }
            else
            {
                GridControl.InvalidateCell(CellRowColumnIndex);
            }
        }

        protected override string GetControlTextFromEditorCore(TextBox textBox)
        {
            return textBox.Text;
        }

        /// <summary>
        /// Refreshes the textbox cell.
        /// </summary>
        public override void RefreshContent()
        {
            base.RefreshContent();
            if (textBoxSelectionStart != -1 && CurrentCellUIElement != null)
                CurrentCellUIElement.Select(textBoxSelectionStart, textBoxSelectionLength);
        }

        /// <summary>
        /// Wire events from textBox
        /// </summary>
        /// <param name="textBox"></param>
        protected override void OnWireUIElement(TextBox textBox)
        {
            base.OnWireUIElement(textBox);
            textBox.AddHandler(TextBox.SelectionChangedEvent, new RoutedEventHandler(textBox_SelectionChanged));
            textBox.AddHandler(TextBox.TextChangedEvent, new TextChangedEventHandler(textBox_TextChanged));
            textBox.AddHandler(TextBox.PreviewKeyDownEvent, new KeyEventHandler(textBox_PreviewKeyDown), true);
            textBox.AddHandler(TextBox.KeyDownEvent, new KeyEventHandler(textBox_KeyDown), true);
            textBox.AddHandler(TextBox.MouseRightButtonUpEvent, new MouseButtonEventHandler(textBox_MouseRightButtonUp), true);
        }
        void textBox_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.GridControl.Model.DisableEditorsContextMenu)
                e.Handled = true;
        }

        /// <summary>
        /// Unwire previously wired events from textBox. 
        /// </summary>
        /// <param name="textBox"></param>
        protected override void OnUnwireUIElement(TextBox textBox)
        {
            base.OnUnwireUIElement(textBox);
            textBox.RemoveHandler(TextBox.SelectionChangedEvent, new RoutedEventHandler(textBox_SelectionChanged));
            textBox.RemoveHandler(TextBox.TextChangedEvent, new TextChangedEventHandler(textBox_TextChanged));
            textBox.RemoveHandler(TextBox.PreviewKeyDownEvent, new KeyEventHandler(textBox_PreviewKeyDown));
            textBox.RemoveHandler(TextBox.KeyDownEvent, new KeyEventHandler(textBox_KeyDown));
            textBox.RemoveHandler(TextBox.MouseRightButtonUpEvent, new MouseButtonEventHandler(textBox_MouseRightButtonUp));
        }

        void textBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                TextBox textBox = (TextBox)sender;
                switch (e.Key)
                {
                    case Key.Right:
                    case Key.Left:
                    case Key.Down:
                    case Key.Up:
                        ccSelectionStart = textBox.SelectionStart;
                        ccSelectionLength = textBox.SelectionLength;
                        break;
                }
            }
            catch
            {

            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
        void textBox_KeyDown(object sender, KeyEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            bool isShiftKey = (e.KeyboardDevice.Modifiers & ModifierKeys.Shift) != ModifierKeys.None;
            switch (e.Key)
            {
                case Key.Right:
                case Key.Left:
                case Key.Down:
                case Key.Up:
                    if (!isShiftKey)
                    {
                        if (ccSelectionStart == textBox.SelectionStart
                            && ccSelectionLength == textBox.SelectionLength)
                            this.GridControl.MoveCurrentCellWithArrowKey(e);
                    }
                    break;
            }
        }

        void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {

            TextBox tb = (TextBox)sender;
            //Added IsModified also in if condition, to triger the CurrentCellChanged event when CellValue and TextBox value are equal.
            if (!(tb.Text.Equals((object)this.CurrentStyle.CellValue)) || IsModified)
            {
                if (!this.IsInArrange && IsCurrentCell(tb) && !CurrentCell.IsInEndEdit)
                {
                    if (!SetControlText(tb.Text))
                        RollbackTextChange(tb); // reverses change.
                    //else
                    //{
                    //    var Cell = this.GridControl.CurrentCell.CellRowColumnIndex;
                    //    this.GridControl.InvalidateCell(GridRangeInfo.Cell(Cell.RowIndex, Cell.ColumnIndex));
                    //}
                }
                if (this.GridControl.Model.Options.EnableFloatingCell || (this.CurrentCell.Renderer!=null && this.CurrentStyle.EnableFloatingCell))
                {
                    GridRangeInfo r = GetRequiredRange(this.CurrentCell.CellRowColumnIndex);
                    if (r.Width > requiredRange.Width || r.Height > requiredRange.Height)
                    {
                        // RowColumnIndex cell = this.CurrentCell.CellRowColumnIndex; Unused local variable
                        this.CurrentCell.EndEdit();
                        this.CurrentCell.BeginEdit();
                    }
                }
            }
        }

        void textBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (!IsInArrange)
            {
                textBoxSelectionStart = textBox.SelectionStart;
                textBoxSelectionLength = textBox.SelectionLength;
            }
        }

        protected override void OnGridPreviewTextInput(TextCompositionEventArgs e)
        {
            if (CurrentCell.IsEditing)
            {
                //CurrentStyle.CellValue = this.CurrentCellUIElement.Text;
                return;
            }

            CurrentCell.ScrollInView();
            CurrentCell.BeginEdit(true);
            TextBox tb = CurrentCellUIElement;
            if (tb != null)
            {
                //SetControlText(e.Text);
                System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"[\b]");
                string str = regex.Replace(e.Text, "");
                switch (tb.CharacterCasing)
                {
                    case CharacterCasing.Lower:
                        str = str.ToLower();
                        break;
                    case CharacterCasing.Upper:
                        str = str.ToUpper();
                        break;
                    default:
                        break;
                }

                if (str == "\r")//|| tb.Text.Contains("\r"))
                    tb.Text += str;
                else
                    tb.Text = str;

                //tb.Text = str;
                tb.CaretIndex = tb.Text.Length;
            }
            e.Handled = true;
        }

        protected override void OnSetFocus()
        {
            //if (!ActivateOptions.IsActivateTriggeredByMouseDownIntoUIElement)
            if ((this.GridControl.Model.Options.ActivateCurrentCellBehavior & GridCellActivateAction.SelectAll)!= GridCellActivateAction.None)
                CurrentCellUIElement.SelectAll();
            else
            {
                CurrentCellUIElement.Select(CurrentCellUIElement.Text.Length, 0);
                CurrentCellUIElement.Focus();
            }
        }

        protected override void OnResetFocus()
        {
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
                        {
                            return false;
                        }
                        if (isControlKey)
                        {
                            if (CurrentCellUIElement.CaretIndex <= 0)
                            {
                                e.Handled = true;
                                return false;
                            }
                        }
                        TextBox tb = this.CurrentCellUIElement as TextBox;
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
                        if (isControlKey)
                        {
                            if (CurrentCellUIElement.CaretIndex >= CurrentCellUIElement.Text.Length)
                            {
                                e.Handled = true;
                                return false;
                            }
                        }
                        TextBox tb = this.CurrentCellUIElement as TextBox;
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
                    if (this.CurrentCell.IsEditing)
                    {
                        if (CurrentCellUIElement != null)
                        {
                            if (CurrentCellUIElement.TextWrapping == TextWrapping.Wrap)
                            {
                                return false;
                            }
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
                            if (CurrentCellUIElement.TextWrapping == TextWrapping.Wrap)
                            {
                                return false;
                            }
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
                            else
                                return true;
                        }
                    }
                    return true;
                case Key.ImeProcessed:
                    {
                        if (!this.CurrentCell.IsEditing)
                        {
                            this.CurrentCell.BeginEdit();
                        }
                        return true;

                    }

                case Key.Back:
                case Key.Delete:
                    {
                        CurrentCell.BeginEdit(true);
                        return false;
                    }
                case Key.Enter:
                    if (this.CurrentStyle.AcceptsReturn || isShiftKey)
                    {
                        if (this.CurrentCellUIElement != null && !this.CurrentCellUIElement.AcceptsReturn)
                            this.CurrentCellUIElement.AcceptsReturn = true;
                        return false;
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
                        //    CurrentCell.Deactivate();
                        if (this.GridControl is GridControl)
                        {
                            if (this.GridControl.Model.Options.EnterKeyBehaviour == EnterKeyBehaviour.MouseDown)
                                this.CurrentCell.MoveDown();
                            else
                                this.CurrentCell.MoveRight();
                            return true;
                        }
                        this.CurrentCell.MoveRight();                        
                        return true;
                        // break; Unreachable code
                    }

                case Key.F2:
                    {
                        TextBox textBox = this.CurrentCellUIElement;
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
        /// Raises GridCellClick event.
        /// </summary>
        /// <param name="rowIndex">The cell row index.</param>
        /// <param name="colIndex">The cell column index.</param>
        /// <param name="e">A <see cref="MouseControllerEventArgs"/> object.</param>
        public override void RaiseGridCellClick(int rowIndex, int colIndex, MouseControllerEventArgs e)
        {
            if (CurrentCell.HasCurrentCellAt(rowIndex, colIndex)
                && !CurrentCell.IsEditing
                && ((GridControl.Model.Options.ActivateCurrentCellBehavior & GridCellActivateAction.DblClickOnCell) == 0)
                && (GridControl.Model.Options.ActivateCurrentCellBehavior != GridCellActivateAction.None))
            {
                CurrentCell.BeginEdit(true);
            }
            base.RaiseGridCellClick(rowIndex, colIndex, e);
        }
    }

}