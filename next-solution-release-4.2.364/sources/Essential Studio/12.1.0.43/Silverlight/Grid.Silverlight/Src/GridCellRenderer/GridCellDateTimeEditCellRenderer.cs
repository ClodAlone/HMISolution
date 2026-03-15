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
    using System.Threading;
    using System.Globalization;
using System.Windows.Input;
    using Syncfusion.Windows.Controls.Scroll;
    using Syncfusion.Windows.Tools.Controls;

    public class GridCellDateTimeEditCellModel : GridCellModel<GridCellDateTimeEditCellRenderer>
    {
        /// <summary>
        /// This is called from GridStyleInfo.GetText (ignoring any <see cref="GridStyleInfo.Format"/> settings).
        /// </summary>
        /// <param name="style">The <see cref="GridStyleInfo"/> object that holds cell information.</param>
        /// <param name="value">The value to convert to a string.</param>
        /// <returns>The string that represents the given value.</returns>
        public override string GetText(GridStyleInfo style, object value)
        {
            return (value != null && !(value is DBNull)) ? Convert.ToString(value, style.CultureInfo) : string.Empty;
        }

        /// <summary>
        /// Returns formatted text for the date time edit cell.
        /// </summary>
        /// <param name="style">The <see cref="GridStyleInfo"/> object that holds cell information.</param>
        /// <param name="value">The value to format.</param>
        /// <param name="textInfo">TextInfo is a hint of who is calling, default is GridCellBaseTextInfo.DisplayText.</param>
        /// <returns>The formatted text for the given value.</returns>
        public override string GetFormattedText(GridStyleInfo style, object value, int textInfo)
        {
            string text = this.GetText(style, value);
            DateTime dateTime = DateTime.MinValue;
            string result = string.Empty;
            var culture = style.GetCulture(true);
            if (DateTime.TryParse(text, culture, System.Globalization.DateTimeStyles.None, out dateTime))
            {
                if (!style.DateTimeEdit.HasDateTimePattern)
                {
                    return dateTime.ToString(culture.DateTimeFormat.ShortDatePattern);
                }

                result = GetDateTimePatternString(dateTime, style.DateTimeEdit, style.DateTimeEdit.DateTimePattern, culture);

                return Convert.ToString(result, style.CultureInfo);
            }
            else
            {
                DateTime dt;
                double d;
                if (value != null && double.TryParse(value.ToString(), out d))
                {
                    dt = DateTime.FromOADate(d);
                    result = GetDateTimePatternString(dt, style.DateTimeEdit, style.DateTimeEdit.DateTimePattern, culture);
                    return Convert.ToString(result, style.CultureInfo);
                }
            }

            return style.DateTimeEdit.HasNoneDateText ? style.DateTimeEdit.NoneDateText : GridDateTimeEditStyleInfo.Default.NoneDateText;
        }

        internal static string GetDateTimePatternString(DateTime dateTime, GridDateTimeEditStyleInfo dateTimeEdit, DateTimePattern dateTimePattern, CultureInfo culture)
        {
            string result = string.Empty;
            switch (dateTimePattern)
            {
                case DateTimePattern.ShortDate:
                    result = dateTime.ToString(culture.DateTimeFormat.ShortDatePattern);
                    break;
                case DateTimePattern.ShortTime:
                    result = dateTime.ToString(culture.DateTimeFormat.ShortTimePattern);
                    break;
                case DateTimePattern.LongDate:
                    result = dateTime.ToString(culture.DateTimeFormat.LongDatePattern);
                    break;
                case DateTimePattern.LongTime:
                    result = dateTime.ToString(culture.DateTimeFormat.LongTimePattern);
                    break;
                case DateTimePattern.FullDateTime:
                    result = dateTime.ToString(culture.DateTimeFormat.FullDateTimePattern);
                    break;
                case DateTimePattern.MonthDay:
                    result = dateTime.ToString(culture.DateTimeFormat.MonthDayPattern);
                    break;
                case DateTimePattern.RFC1123:
                    result = dateTime.ToString(culture.DateTimeFormat.RFC1123Pattern);
                    break;
                case DateTimePattern.SortableDateTime:
                    result = dateTime.ToString(culture.DateTimeFormat.SortableDateTimePattern);
                    break;
                case DateTimePattern.UniversalSortableDateTime:
                    result = dateTime.ToString(culture.DateTimeFormat.UniversalSortableDateTimePattern);
                    break;
                case DateTimePattern.YearMonth:
                    result = dateTime.ToString(culture.DateTimeFormat.YearMonthPattern);
                    break;
                case DateTimePattern.CustomPattern:
                    if (dateTimeEdit.HasCustomPattern)
                    {
                        result = dateTime.ToString(dateTimeEdit.CustomPattern);
                    }
                    break;
            }

            return result;
        }

        /// <summary>
        /// Parses the display text and converts it into a cell value according to the specified culture.
        /// </summary>
        /// <param name="style">Style information for the cell.</param>
        /// <param name="text">The input text to be parsed.</param>
        /// <param name="textInfo">TextInfo is a hint of who is calling, default is GridCellBaseTextInfo.DisplayText.</param>
        /// <returns>True if value was parsed correctly and saved in style object as <see cref="GridStyleInfo.CellValue"/>; False otherwise.</returns>
        public override bool ApplyFormattedText(GridStyleInfo style, string text, int textInfo)
        {
            DateTime dateTime = DateTime.MinValue;
            if (DateTime.TryParse(text, style.CultureInfo, System.Globalization.DateTimeStyles.None, out dateTime))
            {
                style.CellValue = dateTime;
                return true;
            }
            return false;
        }
    }

    /// <summary>
    /// Implements the renderer part of a date time edit cell.
    /// </summary>
    public class GridCellDateTimeEditCellRenderer : GridVirtualizingCellRenderer<DateTimeEdit>
    {
        private int ccSelectionStart, ccSelectionLength; // textSelectionStart = -1, textSelectionLength = 0;
        /// <summary>
        /// Initializes a new <see cref="GridCellDateTimeEditCellRenderer"/>.
        /// </summary>
        public GridCellDateTimeEditCellRenderer()
        {
            this.SupportsRenderOptimization = true;
            this.AllowRecycle = true;
            //this.IsControlTextShown = true;
            this.IsFocusable = true;
            this.AllowKeepAliveOnlyCurrentCell = true;
        }

        /// <summary>
        /// Initializes the content of the date time edit cell
        /// using the information from the cell style (value, text,
        /// behavior etc.). You must override this method in your
        /// derived class.
        /// </summary>
        /// <param name="uiElement">The date time edit control.</param>
        /// <param name="style">The cell style info.</param>
        public override void OnInitializeContent(DateTimeEdit uiElement, GridRenderStyleInfo style)
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
            var dateTimeStyle = style.DateTimeEdit;
            var defaultStyle = GridDateTimeEditStyleInfo.Default;
            uiElement.Cursor = dateTimeStyle.HasCursor ? dateTimeStyle.Cursor : defaultStyle.Cursor;
            uiElement.CustomPattern = dateTimeStyle.HasCustomPattern ? dateTimeStyle.CustomPattern : defaultStyle.CustomPattern;
            uiElement.Pattern = dateTimeStyle.HasDateTimePattern ? dateTimeStyle.DateTimePattern : defaultStyle.DateTimePattern;
            uiElement.IsButtonPopUpEnabled = dateTimeStyle.HasIsButtonPopUpEnabled ? dateTimeStyle.IsButtonPopUpEnabled : defaultStyle.IsButtonPopUpEnabled;
            uiElement.IsCalendarEnabled = dateTimeStyle.HasIsCalendarEnabled ? dateTimeStyle.IsCalendarEnabled : defaultStyle.IsCalendarEnabled;
            uiElement.IsEmptyDateEnabled = dateTimeStyle.HasIsEmptyDateEnabled ? dateTimeStyle.IsEmptyDateEnabled : defaultStyle.IsEmptyDateEnabled;
            uiElement.IsEnabledRepeatButton = dateTimeStyle.HasIsEnabledRepeatButton ? dateTimeStyle.IsEnabledRepeatButton : defaultStyle.IsEnabledRepeatButton;
            uiElement.IsPopupEnabled = dateTimeStyle.HasIsPopupEnabled ? dateTimeStyle.IsPopupEnabled : defaultStyle.IsPopupEnabled;
            uiElement.IsScrollingOnCircle = dateTimeStyle.HasIsScrollingOnCircle ? dateTimeStyle.IsScrollingOnCircle : defaultStyle.IsScrollingOnCircle;
            uiElement.IsVisibleRepeatButton = dateTimeStyle.HasIsVisibleRepeatButton ? dateTimeStyle.IsVisibleRepeatButton : defaultStyle.IsVisibleRepeatButton;
            uiElement.MaxDateTime = dateTimeStyle.HasMaxDateTime ? dateTimeStyle.MaxDateTime : defaultStyle.MaxDateTime;
            uiElement.MinDateTime = dateTimeStyle.HasMinDateTime ? dateTimeStyle.MinDateTime : defaultStyle.MinDateTime;
            uiElement.NoneDateText = dateTimeStyle.HasNoneDateText ? dateTimeStyle.NoneDateText : defaultStyle.NoneDateText;
            uiElement.RepeatButtonBackground = dateTimeStyle.HasRepeatButtonBackground ? dateTimeStyle.RepeatButtonBackground : defaultStyle.RepeatButtonBackground;
            uiElement.RepeatButtonBorderBrush = dateTimeStyle.HasRepeatButtonBorderBrush ? dateTimeStyle.RepeatButtonBorderBrush : defaultStyle.RepeatButtonBorderBrush;
            uiElement.Foreground = style.Foreground;
            uiElement.Background = style.Background;
            uiElement.TextAlignment = TextAlignment.Center;
            uiElement.Padding = margins;
            uiElement.BorderThickness = new Thickness(0);
            DateTime date = DateTime.Now;
            double d;
            var isValid = false;
            if (style.CellValue != null)
            {
                var cellValue = style.CellValue.ToString();
                if (DateTime.TryParse(cellValue, out date))
                {
                    isValid = true;
                }
                else if (double.TryParse(cellValue, out d))
                {
                    isValid = true;
                    date = DateTime.FromOADate(d);
                }
            }
            if (isValid)
            {
                uiElement.DateTime = date;
            }
            else
            {
                uiElement.DateTime = null;
            }
            OnWireUIElement(uiElement);
        }

        protected override void ArrangeUIElement(Cells.ArrangeCellArgs aca, DateTimeEdit uiElement, GridRenderStyleInfo style)
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
            this.ControlValue = this.GetControlValue(this.CurrentStyle);
        }

        protected override void OnEnteredEditMode()
        {
            this.UpdateDateTimeEdit();
        }

        protected override void OnActivated()
        {
            this.UpdateDateTimeEdit();
        }

        protected override void OnEditingComplete()
        {
            this.UpdateDateTimeEdit();
            this.GridControl.InvalidateCell(this.CellRowColumnIndex);
        }

        protected override void OnDeactivated()
        {
            this.GridControl.InvalidateCell(this.CellRowColumnIndex);
        }

        private void UpdateDateTimeEdit()
        {
            if (this.CurrentCellUIElement != null)
            {
                var style = this.CurrentStyle;
                var nonDateText = style.DateTimeEdit.HasNoneDateText ? style.DateTimeEdit.NoneDateText : GridDateTimeEditStyleInfo.Default.NoneDateText;
                if (this.ControlValue != null && this.ControlValue.ToString() != string.Empty && this.ControlValue.ToString() != nonDateText)
                {
                    var value = this.ControlValue.ToString();
                    DateTime date = DateTime.Now;
                    if (DateTime.TryParse(value, out date))
                    {
                        this.CurrentCellUIElement.DateTime = date;
                    }
                }
                else
                {
                    this.CurrentCellUIElement.DateTime = null;
                }
            }
            else
            {
                this.GridControl.InvalidateCell(this.CellRowColumnIndex);
            }
        }

        //protected override string GetControlTextFromEditorCore(DateTimeEdit uiElement)
        //{
        //    return uiElement.DateTime.ToString();
        //}

        protected override object GetControlValueFromEditorCore(DateTimeEdit uiElement)
        {
            return uiElement.DateTime;
        }

        /// <summary>
        /// This event fires while we press the key
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void uiElement_KeyDown(object sender, KeyEventArgs e)
        {
            bool isShiftKey = (Keyboard.Modifiers & ModifierKeys.Shift) != ModifierKeys.None;
            DateTimeEdit textBox = (DateTimeEdit)sender;
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

        protected override void OnWireUIElement(DateTimeEdit uiElement)
        {
            base.OnWireUIElement(uiElement);
            uiElement.DateTimeChanged += OnDateTimeChanged;
            uiElement.SelectionChanged += uiElement_SelectionChanged;
            uiElement.KeyDown+=new KeyEventHandler(uiElement_KeyDown);
        }     
        

        void uiElement_SelectionChanged(object sender, RoutedEventArgs e)
        {
            var dateTimeEdit = sender as DateTimeEdit;
            this.ccSelectionLength = dateTimeEdit.SelectionLength;
            this.ccSelectionStart = dateTimeEdit.SelectionStart;
        }

        void OnPreviewKeyDown(object sender, KeyEventArgs args)
        {
            DateTimeEdit textBox = (DateTimeEdit)sender;

            if (args.Key == Key.Right || args.Key == Key.Left)
            {
                if (textBox.SelectionStart == 0 || textBox.SelectionStart + textBox.SelectionLength == textBox.Text.Length)
                {
                    this.GridControl.MoveCurrentCellWithArrowKey(args);
                }
            } 
        }

        protected override void OnUnwireUIElement(DateTimeEdit uiElement)
        {
            base.OnUnwireUIElement(uiElement);
            uiElement.DateTimeChanged -= OnDateTimeChanged;
            uiElement.SelectionChanged -= uiElement_SelectionChanged;
            uiElement.KeyDown -= new KeyEventHandler(uiElement_KeyDown);
            
          
        }

        private void OnDateTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var dateTimeEdit = d as DateTimeEdit;
            if (e.NewValue != null)
            {
                var dateTime = (DateTime)e.NewValue;
                if (!this.IsInArrange && this.IsCurrentCell(dateTimeEdit) && !this.CurrentCell.IsInEndEdit)
                {
                    if (!this.SetControlValue(dateTime))
                    {
                        RefreshContent();
                    }
                }
            }
            else
            {
                if (!this.IsInArrange && this.IsCurrentCell(dateTimeEdit) && !this.CurrentCell.IsInEndEdit)
                {
                    if (!this.SetControlValue(null))
                    {
                        this.RefreshContent();
                    }
                }
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
            {
                if (this.CurrentCellUIElement.SelectionLength == (this.CurrentCellUIElement.Text.Length - 1))
                {
                    this.CurrentCellUIElement.Text = e.Text;
                }
            }
        }

        protected override bool ShouldGridTryToHandlePreviewKeyDown(System.Windows.Input.KeyEventArgs e)
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
                    {
                        DateTimeEdit tb = this.CurrentCellUIElement;
                        if (tb != null && (tb.SelectionStart + tb.SelectionLength) == tb.Text.Length)
                        {
                            return true;
                        }

                        // otherwise, move caret within textbox when cell is not in edit-mode.
                        return !this.CurrentCell.IsEditing;
                    }
                case Key.Left:
                    {
                        DateTimeEdit tb = this.CurrentCellUIElement;
                        if (tb != null && tb.SelectionStart == 0)
                        {
                            return true;
                        }

                        // otherwise, move caret within textbox when cell is not in edit-mode.
                        return !this.CurrentCell.IsEditing;
                    }
                case Key.Down:
                case Key.Up:
                    {
                        // Move to next cell when whole text in textbox is selected.
                        DateTimeEdit tb = this.CurrentCellUIElement;
                        if (tb != null && tb.SelectionStart == 0 && tb.SelectionLength == tb.Text.Length)
                        {
                            return true;
                        }

                        // otherwise, move caret within textbox when cell is not in edit-mode.
                        return !this.CurrentCell.IsEditing;
                    }

                case Key.Delete:
                case Key.Back:
                case Key.End:
                case Key.Home:
                    if (this.CurrentCell.IsEditing)
                        return false;
                    else
                        return true;
                //case Key.F2:
                //    {
                //        CurrentCell.BeginEdit(true);
                //        DateTimeEdit textBox = this.CurrentCellUIElement;
                //        if (textBox != null && this.CurrentCell.IsEditing && textBox.SelectionLength == textBox.Text.Length)
                //        {
                //            textBox.CaretIndex = textBox.Text.Length;
                //            e.Handled = true;
                //            return false;
                //        }
                //    }
                //    break;

            }
            return base.ShouldGridTryToHandlePreviewKeyDown(e);
        }

        public override void RaiseGridCellClick(int rowIndex, int colIndex, MouseControllerEventArgs e)
        {
            if (this.CurrentCell.HasCurrentCellAt(rowIndex, colIndex)
                && !this.CurrentCell.IsEditing
                && this.GridControl.Model.Options.ActivateCurrentCellBehavior != GridCellActivateAction.DblClickOnCell)
            {
                this.CurrentCell.BeginEdit(true);
            }

            base.RaiseGridCellClick(rowIndex, colIndex, e);
        }
    }
}
