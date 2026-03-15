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
    public class GridCellTimeSpanEditCellModel : GridCellModel<GridCellTimeSpanEditCellRenderer>
    {
        /// <summary>
        /// Returns the display text as value object according to the specified number format.
        /// </summary>
        /// <param name="style">Style information for the cell.</param>
        /// <param name="text">The display text.</param>
        /// <returns>The formatted value of the cell.</returns> 
        public override bool ApplyFormattedText(GridStyleInfo style, string text, int textInfo)
        {
            TimeSpan timeSpan = TimeSpan.MinValue ;
            if (TimeSpan.TryParse(text, out timeSpan))
            {
                style.CellValue = timeSpan;
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
            string format = style.HasTimeSpanEdit && style.TimeSpanEdit.HasFormat ? style.TimeSpanEdit.Format : GridTimeSpanEditStyleInfo.Default.Format;

            TimeSpan timeSpan = TimeSpan.MinValue;
            if (TimeSpan.TryParse(value.ToString(), out timeSpan))
            {
                return timeSpan.ToString();
            }

            return style.HasTimeSpanEdit && style.TimeSpanEdit.HasNullString ? style.TimeSpanEdit.NullString : GridTimeSpanEditStyleInfo.Default.NullString;
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
            return this.GetText(style, value);            
        }
    }

    /// <summary>
    /// Implements the renderer part of an up down cell.
    /// </summary>
    public class GridCellTimeSpanEditCellRenderer : GridVirtualizingCellRenderer<TimeSpanEdit>
    {

        private TimeSpanEditPaint TimeSpanEditPaint;
        /// <summary>
        /// Initializes a new <see cref="GridCellTimeSpanEditCellRenderer"/>.
        /// </summary>
        public GridCellTimeSpanEditCellRenderer()
        {
            this.SupportsRenderOptimization = true;
            this.AllowRecycle = true;
            this.IsControlTextShown = true;
            this.IsFocusable = true;
            this.AllowKeepAliveOnlyCurrentCell = true;
            this.TimeSpanEditPaint = new TimeSpanEditPaint();
        }

        protected override void OnRender(System.Windows.Media.DrawingContext dc, Syncfusion.Windows.Controls.Cells.RenderCellArgs rca, GridRenderStyleInfo style)
        {
            if (rca.CellUIElements != null)
            {
                return;
            }

            // Will only get hit if SupportsRenderOptimization is true, otherwise rca.CellUIElements is never null.
            Thickness margins = new Thickness(0);
            if (style.HasImageIndex)
            {
                margins = style.AdjustImageWidthAndHeightToMargin(margins, rca.CellRect.Size);
            }
            else
            {
                margins = style.ErrorInfo.AdjustErrorInfoMargin(margins, style.GridControl, style.CellRowColumnIndex);
            }

            // TextBoxView always seems to have this margin and I am not able to reset the margin.
            // Therefore I am also hard-codeing it here so that TextBox behavior is properly
            // emulated.
            margins.Left = Math.Max(margins.Left, 2);
            margins.Right = Math.Max(margins.Right, 2);

            Rect textRectangle = rca.SubtractBorderMargins(rca.CellRect, margins);
            textRectangle = rca.SubtractBorderMargins(rca.CellRect, margins);
           // style.HorizontalAlignment = HorizontalAlignment.Center;
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
                controlValue = this.GetControlValue(style);
            }

            this.TimeSpanEditPaint.DrawTimeSpanEdit(dc, textRectangle, controlValue.ToString(), style);

            base.OnRender(dc, rca, style);
        }

        private string GetFormat(GridRenderStyleInfo style)
        {
            if (style != null && style.HasTimeSpanEdit)
            {
                return style.TimeSpanEdit.HasFormat ? style.TimeSpanEdit.Format : GridTimeSpanEditStyleInfo.Default.Format;
            }

            return GridTimeSpanEditStyleInfo.Default.Format;
        }

        /// <summary>
        /// Initializes the content of the updown cell
        /// using the information from the cell style (value, text,
        /// behavior etc.).
        /// </summary>
        /// <param name="button">The UpDown control.</param>
        /// <param name="style">The cell style info.</param>
        public override void OnInitializeContent(TimeSpanEdit uiElement, GridRenderStyleInfo style)
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
            uiElement.Foreground = style.Foreground;

            uiElement.AllowNull = style.TimeSpanEdit.HasAllowNull ? style.TimeSpanEdit.AllowNull : GridTimeSpanEditStyleInfo.Default.AllowNull;
            if (uiElement.Format == string.Empty)
            {
                uiElement.Format = style.TimeSpanEdit.HasFormat ? style.TimeSpanEdit.Format : "d.h:m:s";
            }
            uiElement.IncrementOnScrolling = style.TimeSpanEdit.HasIncrementOnScrolling ? 
                                              style.TimeSpanEdit.IncrementOnScrolling : GridTimeSpanEditStyleInfo.Default.IncrementOnScrolling;
            uiElement.MinValue  = style.TimeSpanEdit.HasMinValue ? style.TimeSpanEdit.MinValue : GridTimeSpanEditStyleInfo.Default.MinValue;
            uiElement.MaxValue = style.TimeSpanEdit.HasMaxValue ? style.TimeSpanEdit.MaxValue : GridTimeSpanEditStyleInfo.Default.MaxValue;
            uiElement.NullString  = style.TimeSpanEdit.HasNullString ? style.TimeSpanEdit.NullString : GridTimeSpanEditStyleInfo.Default.NullString;
            uiElement.ShowArrowButtons  = style.TimeSpanEdit.HasShowArrowButtons ? style.TimeSpanEdit.ShowArrowButtons : GridTimeSpanEditStyleInfo.Default.ShowArrowButtons;

            // TimeSpan timeSpan  = new TimeSpan(); The variable is assigned but its never used
            // var isValid = false; The variable is assigned but its never used
            //if (style.CellValue != null)
            //{
            //    var cellValue = style.CellValue.ToString();
            //    if (TimeSpan.TryParse(cellValue, out timeSpan))
            //    {
            //        isValid = true;
            //    }
            //}
            //if (isValid)
            //{
            //    uiElement.Value =(TimeSpan?)timeSpan;
            //}
            //else
            //{
            //    uiElement.Value  = null;
            //}
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

            OnWireUIElement(uiElement);
        }

        protected override void ArrangeUIElement(Cells.ArrangeCellArgs aca, TimeSpanEdit uiElement, GridRenderStyleInfo style)
        {
            Thickness margins = style.TextMargins.ToThickness();
            margins = style.ErrorInfo.AdjustErrorInfoMarginOnEditing(margins, style.GridControl, style.CellRowColumnIndex);
            uiElement.Padding = margins;

            base.ArrangeUIElement(aca, uiElement, style);
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
            this.UpdateTimeSpanEditControl();
        }

        protected override void OnActivated()
        {
            this.UpdateTimeSpanEditControl();
        }

        protected override void OnEditingComplete()
        {
            this.GridControl.InvalidateCell(this.CellRowColumnIndex);
        }

        protected override void OnDeactivated()
        {
            this.GridControl.InvalidateCell(this.CellRowColumnIndex);
        }

        private void UpdateTimeSpanEditControl()
        {
            if (this.CurrentCellUIElement != null)
            {
                this.CurrentCellUIElement.Format = this.GetFormat(this.CurrentStyle);

                var style = this.CurrentStyle;
                var nullString = style.TimeSpanEdit.HasNullString ? style.TimeSpanEdit.NullString : GridTimeSpanEditStyleInfo.Default.NullString;
                if (this.ControlValue != null && this.ControlValue.ToString() != string.Empty && this.ControlValue.ToString() != nullString)
                {
                    var value = this.ControlValue.ToString();
                    TimeSpan  timeSpan = new TimeSpan();
                    if (TimeSpan.TryParse(value, out timeSpan))
                    {
                        this.CurrentCellUIElement.Value = (TimeSpan?)timeSpan;
                    }
                }
                else
                {
                    this.CurrentCellUIElement.Value = null;
                }
            }
            else
            {
                this.GridControl.InvalidateCell(this.CellRowColumnIndex);
            }
        }

        protected override object GetControlValueFromEditorCore(TimeSpanEdit uiElement)
        {
            return uiElement.Value;
        }

        protected override void OnWireUIElement(TimeSpanEdit uiElement)
        {
            base.OnWireUIElement(uiElement);
            uiElement.ValueChanged += new PropertyChangedCallback(OnTimeSpanEditValueChanged);
            uiElement.RemoveHandler(TimeSpanEdit.MouseRightButtonUpEvent, new MouseButtonEventHandler(uiElement_MouseRightButtonUp));
        }

        protected override void OnUnwireUIElement(TimeSpanEdit uiElement)
        {
            base.OnUnwireUIElement(uiElement);
            uiElement.ValueChanged -= new PropertyChangedCallback(OnTimeSpanEditValueChanged);
            uiElement.AddHandler(TimeSpanEdit.MouseRightButtonUpEvent, new MouseButtonEventHandler(uiElement_MouseRightButtonUp), true);
        }

        void uiElement_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.GridControl.Model.DisableEditorsContextMenu)
            {
                e.Handled = true;
            }
        }

        void OnTimeSpanEditValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var timeSpanEdit = (TimeSpanEdit)d;
            if (!this.IsInArrange && this.IsCurrentCell(timeSpanEdit) && !this.CurrentCell.IsInEndEdit)
            {
                if (!this.SetControlValue(timeSpanEdit.Value))
                {
                    RefreshContent();
                }
            }
        }

        protected override string GetControlTextFromEditorCore(TimeSpanEdit uiElement)
        {
            return uiElement.Value != null ? uiElement.Value.ToString() : string.Empty;
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
                case Key.A:
                    if (isControlKey)
                    {
                        CurrentCellUIElement.SelectAll();
                        e.Handled = true;
                        return false;
                    }
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
                            CurrentCellUIElement.CaretIndex = 0;
                            e.Handled = true;
                            return false;
                        }
                        TimeSpanEdit timeSpanEditControl = this.CurrentCellUIElement as TimeSpanEdit;
                        if (timeSpanEditControl != null)
                        {
                            if (timeSpanEditControl.CaretIndex == 0 && timeSpanEditControl.SelectionLength == 0)
                            {
                                e.Handled = true;
                                return true;
                            }// When the entire text is selected pressing left arrow key moves the focus to the previous cell, whereas in Excel the cursor moves to the 0th index. The below code is added to move the cursor to the 0th Index
                            else if (timeSpanEditControl.SelectionLength == timeSpanEditControl.Text.Length)
                            {
                                timeSpanEditControl.CaretIndex = 0;
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
                        if (isControlKey)
                        {
                            CurrentCellUIElement.CaretIndex = CurrentCellUIElement.Text.Length;
                            e.Handled = true;
                            return false;
                        }
                        TimeSpanEdit timeSpanEditControl = this.CurrentCellUIElement as TimeSpanEdit;
                        if (timeSpanEditControl != null)
                        {
                            if (timeSpanEditControl.CaretIndex == 0 && timeSpanEditControl.SelectionLength == 0)
                            {
                                //e.Handled = true;
                                return false;
                            } // When the entire text is selected pressing right arrow key moves the focus to the next cell, whereas in Excel the cursor moves to the last position. The below code is added to move the cursor to the last position
                            else if (timeSpanEditControl.SelectionLength == timeSpanEditControl.Text.Length)
                            {
                                timeSpanEditControl.CaretIndex = timeSpanEditControl.Text.Length;
                                e.Handled = true;
                                return false;
                            }
                            else if (timeSpanEditControl.CaretIndex == timeSpanEditControl.Text.Length)
                                return true;
                            else
                                return false;
                        }
                        else
                            return false;
                    }
                    return true;

                case Key.Down:
                    {
                        if (this.CurrentCell.IsEditing)
                        {
                            if (isControlKey)
                            {
                                CurrentCellUIElement.CaretIndex = CurrentCellUIElement.Text.Length;
                                e.Handled = true;
                                return false;
                            }
                            return false;
                        }
                        else
                            return true;
                    }
                case Key.Up:
                    {
                        if (this.CurrentCell.IsEditing)
                        {
                            if (isControlKey)
                            {
                                e.Handled = true;
                                return false;
                            }
                            return false;
                        }
                        else
                            return true;
                    }
                case Key.End:
                    {
                        if (this.CurrentCell.IsEditing)
                        {
                            CurrentCellUIElement.CaretIndex = CurrentCellUIElement.Text.Length;
                            e.Handled = true;
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                case Key.Home:
                    {
                        if (this.CurrentCell.IsEditing)
                        {
                            CurrentCellUIElement.CaretIndex = 0;
                            e.Handled = true;
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
                        //{
                        //    CurrentCell.EndEdit();
                        //    e.Handled = true;
                        //}
                        CurrentCell.MoveRight();
                        return true;
                    }
                //case Key.F2:
                //    {
                //        CurrentCell.BeginEdit(true);
                //        return false;
                //    }
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
                if (this.CurrentCell.IsEditing && this.CurrentCellUIElement != null && this.GridControl.Model.Options.ActivateCurrentCellBehavior == GridCellActivateAction.ClickOnCell)
                {
                    CurrentCellUIElement.Focus();
                }
            }
            base.RaiseGridCellClick(rowIndex, colIndex, e);
        }
    }

    public class TimeSpanEditPaint
    {
        private Dictionary<Size, VisualBrush> brushes = new Dictionary<Size, VisualBrush>();

        /// <summary>
        /// Default constructor.
        /// </summary>
        public TimeSpanEditPaint()
        {
        }

        /// <summary>
        /// Draws the TimeSpanEdit Control in the cell rectangle.
        /// </summary>
        /// <param name="dc">The drawing context.</param>
        /// <param name="rc">Cell rectangle.</param>
        /// <param name="text">Text to be drawn over the TimeSpanEdit Control.</param>
        /// <param name="style">Cell style information.</param>
        /// <returns>Cell margins.</returns>
        public void DrawTimeSpanEdit(DrawingContext dc, Rect rc, string text, GridStyleInfo style)
        {
            VisualBrush vb = this.GetVisualBrush(rc.Size,style);
            dc.DrawRectangle(vb, null, rc);
            
            if (style.HasTimeSpanEdit)
            {
                text = style.TimeSpanEdit.HasFormat ? this.GetFormattedText(text,style) : text;
            }

            GridTextBoxPaint.DrawText(dc, rc, text, style);
        }

        private string GetFormattedText(string text, GridStyleInfo style)
        {
            string formattedText;
            TimeSpanEdit timeSpanEdit = new TimeSpanEdit();
            if (style.HasTimeSpanEdit)
            {
                timeSpanEdit.NullString = style.TimeSpanEdit.HasNullString ? style.TimeSpanEdit.NullString : GridTimeSpanEditStyleInfo.Default.NullString;
                timeSpanEdit.Format = style.TimeSpanEdit.HasFormat ? style.TimeSpanEdit.Format : GridTimeSpanEditStyleInfo.Default.Format;
               

                TimeSpan timeSpan = new TimeSpan();
                var isValid = false;
                if (style.CellValue != null)
                {
                    var cellValue = style.CellValue.ToString();
                    if (TimeSpan.TryParse(cellValue, out timeSpan))
                    {
                        isValid = true;
                    }
                }
                if (isValid)
                {
                    timeSpanEdit.Value = (TimeSpan?)timeSpan;
                }
                else
                {
                    timeSpanEdit.Value = null;
                }
            }
            formattedText = timeSpanEdit.Text;
            timeSpanEdit = null;

            return formattedText;
        }

        private VisualBrush GetVisualBrush(Size size, GridStyleInfo style)
        {
            if (this.brushes.ContainsKey(size))
            {
                return this.brushes[size];
            }

            bool wasAnimated = GridUtil.IsAnimated;
            try
            {
                GridUtil.IsAnimated = false;

                VisualBrush visualBrush;
                TimeSpanEdit b = new TimeSpanEdit();
                b.Background = Brushes.Transparent;
                b.BorderThickness = new Thickness(0);
                b.BeginInit();
                b.Width = size.Width;
                b.Height = size.Height;
                b.ShowArrowButtons = false;
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