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
    using Syncfusion.Windows.Tools.Controls;

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
            string strValue = value.ToString();
            if (TimeSpan.TryParse(strValue, out timeSpan))
            {
                string formatedValue;
                TimeSpanEdit te = new TimeSpanEdit();
               
                te.Format = format;
                te.Value = timeSpan;
                formatedValue = te.Text;

                te = null;

                return formatedValue;
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
            uiElement.Background = style.Background;
            uiElement.AllowNull = style.TimeSpanEdit.HasAllowNull ? style.TimeSpanEdit.AllowNull : GridTimeSpanEditStyleInfo.Default.AllowNull;
            uiElement.Format  = style.TimeSpanEdit.HasFormat ? style.TimeSpanEdit.Format : GridTimeSpanEditStyleInfo.Default.Format;
            uiElement.IncrementOnScrolling = style.TimeSpanEdit.HasIncrementOnScrolling ? 
                                              style.TimeSpanEdit.IncrementOnScrolling : GridTimeSpanEditStyleInfo.Default.IncrementOnScrolling;
            uiElement.MinValue  = style.TimeSpanEdit.HasMinValue ? style.TimeSpanEdit.MinValue : GridTimeSpanEditStyleInfo.Default.MinValue;
            uiElement.MaxValue = style.TimeSpanEdit.HasMaxValue ? style.TimeSpanEdit.MaxValue : GridTimeSpanEditStyleInfo.Default.MaxValue;
            uiElement.NullString  = style.TimeSpanEdit.HasNullString ? style.TimeSpanEdit.NullString : GridTimeSpanEditStyleInfo.Default.NullString;
            uiElement.ShowArrowButtons  = style.TimeSpanEdit.HasShowArrowButtons ? style.TimeSpanEdit.ShowArrowButtons : GridTimeSpanEditStyleInfo.Default.ShowArrowButtons;

            TimeSpan timeSpan  = new TimeSpan();
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
                uiElement.Value = timeSpan;
            }
            else
            {
                uiElement.Value  = null;
            }

            uiElement.Background = new SolidColorBrush(Colors.White);
            uiElement.Foreground = new SolidColorBrush(Colors.Black);
           
            OnWireUIElement(uiElement);
        }

        protected override void ArrangeUIElement(Cells.ArrangeCellArgs aca, TimeSpanEdit uiElement, GridRenderStyleInfo style)
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
        }

        protected override void OnUnwireUIElement(TimeSpanEdit uiElement)
        {
            base.OnUnwireUIElement(uiElement);
            uiElement.ValueChanged -= new PropertyChangedCallback(OnTimeSpanEditValueChanged);
            
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
            bool isControlKey = (Keyboard.Modifiers & ModifierKeys.Control) != ModifierKeys.None;
            bool isShiftKey = (Keyboard.Modifiers & ModifierKeys.Shift) != ModifierKeys.None;
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
                        TimeSpanEdit timeSpanEditControl = this.CurrentCellUIElement as TimeSpanEdit;
                        if (timeSpanEditControl != null)
                        {
                            if (timeSpanEditControl.SelectionStart == 0)
                                return true;
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
                        TimeSpanEdit timeSpanEditControl = this.CurrentCellUIElement as TimeSpanEdit;
                        if (timeSpanEditControl != null)
                        {
                            if ((timeSpanEditControl.SelectionStart + timeSpanEditControl.Text.Length) == timeSpanEditControl.Text.Length)
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
                    if (this.CurrentCell.IsEditing)
                        return false;
                    else
                        return true;
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
                        if (this.CurrentCell.IsEditing)
                        {
                            CurrentCell.EndEdit();
                            e.Handled = true;
                        }
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
                        CurrentCell.MoveRight();
                        CurrentCell.ScrollInView();
                        return true;
                        // break; Unreachable code
                    }
                case Key.F2:
                    {
                        CurrentCell.BeginEdit(true);
                        return false;
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
            if (this.CurrentCell.HasCurrentCellAt(rowIndex, colIndex)
                && !this.CurrentCell.IsEditing
                && this.GridControl.Model.Options.ActivateCurrentCellBehavior != GridCellActivateAction.DblClickOnCell)
            {
                this.CurrentCell.BeginEdit(true);
            }
            if (this.CurrentCellUIElement != null && this.GridControl.Model.Options.ActivateCurrentCellBehavior == GridCellActivateAction.ClickOnCell)
            {
                CurrentCellUIElement.Focus();
            }
            base.RaiseGridCellClick(rowIndex, colIndex, e);
        }
    }
}