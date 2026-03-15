#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Windows.Media;

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

            if (DateTime.TryParse(text, culture, System.Globalization.DateTimeStyles.None, out dateTime) && style.DateTimeEdit.DateTimePattern != DateTimePattern.CustomPattern)
            {
                if (!style.DateTimeEdit.HasDateTimePattern)
                {
                    return dateTime.ToString(culture.DateTimeFormat.ShortDatePattern);
                }

                result = GetDateTimePatternString(dateTime, style.DateTimeEdit, style.DateTimeEdit.DateTimePattern, culture);

                return Convert.ToString(result, style.CultureInfo);
            }
            else if (style.DateTimeEdit.DateTimePattern == DateTimePattern.CustomPattern)
            {
                if (value != null && !(value is DBNull))
                {
                    var txt = Convert.ToString(value, style.CultureInfo);
                    if (txt.Length >= 1)
                    {
                        DateTime dt;
                        CultureInfo ci = style.GetCulture(true);

                        if (value is DateTime)
                        {
                            result = GetDateTimePatternString(Convert.ToDateTime(value), style.DateTimeEdit, style.DateTimeEdit.DateTimePattern, culture);
                        }
                        else
                        {
                            double d;
                            if (double.TryParse(value.ToString(), out d))
                            {
                                dt = DateTime.FromOADate(d);
                                result = GetDateTimePatternString(dt, style.DateTimeEdit, style.DateTimeEdit.DateTimePattern, culture);
                            }
                            else if(DateTime.TryParse(value.ToString(), out dt))
                            {
                                result = GetDateTimePatternString(dt, style.DateTimeEdit, style.DateTimeEdit.DateTimePattern, culture);
                            }
                        }
                        return Convert.ToString(result, style.CultureInfo);
                    }                    
                }                
            }

            if (style.DateTimeEdit.NoneDateText != GridDateTimeEditStyleInfo.Default.NoneDateText)
            {
                return style.DateTimeEdit.NoneDateText;
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
        int ccSelectionStart, ccSelectionLength; // , textSelectionStart = -1, textSelectionLength = 0; Variable assiged but it is never used 
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

            // Draw the formatted text string to the DrawingContext of the control.
            GridTextBoxPaint.DrawText(dc, textRectangle, text, style);
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
            uiElement.CanEdit = style.DateTimeEdit.CanEdit;
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
                uiElement.Text = null;
                uiElement.DateTime = null;
            }
            if (this.GridControl.Model.Options.ShowErrorIconOnEditing)
                uiElement.Background = Brushes.Transparent;
            else
                uiElement.Background = Brushes.White;
            uiElement.Foreground = Brushes.Black;
            
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
            OnWireUIElement(uiElement);
        }

        protected override void ArrangeUIElement(Syncfusion.Windows.Controls.Cells.ArrangeCellArgs aca, DateTimeEdit uiElement, GridRenderStyleInfo style)
        {
            base.ArrangeUIElement(aca, uiElement, style);
            var dateTimeStyle = style.DateTimeEdit;
            var defaultStyle = GridDateTimeEditStyleInfo.Default;
            uiElement.AutoCorrectedHiglightDuration = dateTimeStyle.HasAutoCorrectedHighlighDuration ? dateTimeStyle.AutoCorrectedHighlightDuration : defaultStyle.AutoCorrectedHighlightDuration;
            uiElement.CorrectForeground = dateTimeStyle.HasCorrectForeground ? dateTimeStyle.CorrectForeground : defaultStyle.CorrectForeground;
            uiElement.Cursor = dateTimeStyle.HasCursor ? dateTimeStyle.Cursor : defaultStyle.Cursor;
            uiElement.CustomPattern = dateTimeStyle.HasCustomPattern ? dateTimeStyle.CustomPattern : defaultStyle.CustomPattern;
            uiElement.Pattern = dateTimeStyle.HasDateTimePattern ? dateTimeStyle.DateTimePattern : defaultStyle.DateTimePattern;
            uiElement.IncorrectForeground = dateTimeStyle.HasIncorrectForeground ? dateTimeStyle.IncorrectForeground : defaultStyle.IncorrectForeground;
            uiElement.IsAnimation = dateTimeStyle.HasIsAnimation ? dateTimeStyle.IsAnimation : defaultStyle.IsAnimation;
            uiElement.IsAutoCorrect = dateTimeStyle.HasIsAutoCorrect ? dateTimeStyle.IsAutoCorrect : defaultStyle.IsAutoCorrect;
            uiElement.IsButtonPopUpEnabled = dateTimeStyle.HasIsButtonPopUpEnabled ? dateTimeStyle.IsButtonPopUpEnabled : defaultStyle.IsButtonPopUpEnabled;
            uiElement.IsCalendarEnabled = dateTimeStyle.HasIsCalendarEnabled ? dateTimeStyle.IsCalendarEnabled : defaultStyle.IsCalendarEnabled;
            //uiElement.IsEditable = dateTimeStyle.HasIsEditable ? dateTimeStyle.IsEditable : defaultStyle.IsEditable;
            uiElement.IsEmptyDateEnabled = dateTimeStyle.HasIsEmptyDateEnabled ? dateTimeStyle.IsEmptyDateEnabled : defaultStyle.IsEmptyDateEnabled;
            uiElement.IsEnabledRepeatButton = dateTimeStyle.HasIsEnabledRepeatButton ? dateTimeStyle.IsEnabledRepeatButton : defaultStyle.IsEnabledRepeatButton;
            uiElement.IsHoldMaxWidth = dateTimeStyle.HasIsHoldMaxWidth ? dateTimeStyle.IsHoldMaxWidth : defaultStyle.IsHoldMaxWidth;
            uiElement.IsPopupEnabled = dateTimeStyle.HasIsPopupEnabled ? dateTimeStyle.IsPopupEnabled : defaultStyle.IsPopupEnabled;
            uiElement.IsScrollingOnCircle = dateTimeStyle.HasIsScrollingOnCircle ? dateTimeStyle.IsScrollingOnCircle : defaultStyle.IsScrollingOnCircle;
            uiElement.IsVisibleRepeatButton = dateTimeStyle.HasIsVisibleRepeatButton ? dateTimeStyle.IsVisibleRepeatButton : defaultStyle.IsVisibleRepeatButton;
            uiElement.IsWatchEnabled = dateTimeStyle.HasIsWatchEnabled ? dateTimeStyle.IsWatchEnabled : defaultStyle.IsWatchEnabled;
            uiElement.MaxDateTime = dateTimeStyle.HasMaxDateTime ? dateTimeStyle.MaxDateTime : defaultStyle.MaxDateTime;
            uiElement.MinDateTime = dateTimeStyle.HasMinDateTime ? dateTimeStyle.MinDateTime : defaultStyle.MinDateTime;
            uiElement.NoneDateText = dateTimeStyle.HasNoneDateText ? dateTimeStyle.NoneDateText : defaultStyle.NoneDateText;
            uiElement.RepeatButtonBackground = dateTimeStyle.HasRepeatButtonBackground ? dateTimeStyle.RepeatButtonBackground : defaultStyle.RepeatButtonBackground;
            uiElement.RepeatButtonBorderBrush = dateTimeStyle.HasRepeatButtonBorderBrush ? dateTimeStyle.RepeatButtonBorderBrush : defaultStyle.RepeatButtonBorderBrush;
            uiElement.UncertainForeground = dateTimeStyle.HasUncertainForeground ? dateTimeStyle.UncertainForeground : defaultStyle.UncertainForeground;
            //uiElement.IsVisibleRepeatButton = false;
            if (dateTimeStyle.HasPopupDelay)
            {
                uiElement.PopupDelay = dateTimeStyle.PopupDelay;
            }

            if (style.HasCultureInfo)
            {
                uiElement.CultureInfo = style.CultureInfo;
            }

            Thickness margins = style.TextMargins.ToThickness();
            margins = style.ErrorInfo.AdjustErrorInfoMarginOnEditing(margins, style.GridControl, style.CellRowColumnIndex);
            uiElement.Padding = margins;
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
                    this.CurrentCellUIElement.Text = null;
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

        protected override void OnWireUIElement(DateTimeEdit uiElement)
        {
            base.OnWireUIElement(uiElement);
            uiElement.IsDropDownOpenChanged += uiElement_IsDropDownOpenChanged;
            uiElement.DateTimeChanged += OnDateTimeChanged;
            uiElement.SelectionChanged += uiElement_SelectionChanged;
            uiElement.AddHandler(DateTimeEdit.MouseRightButtonUpEvent, new MouseButtonEventHandler(uiElement_MouseRightButtonUp),true);
            //uiElement.AddHandler(DateTimeEdit.PreviewKeyDownEvent, new KeyEventHandler(OnPreviewKeyDown), true);
        }

        void uiElement_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.GridControl.Model.DisableEditorsContextMenu)
            {
                e.Handled = true;
            }
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
            uiElement.IsDropDownOpenChanged -= uiElement_IsDropDownOpenChanged;
            uiElement.DateTimeChanged -= OnDateTimeChanged;
            uiElement.SelectionChanged -= uiElement_SelectionChanged;
            uiElement.RemoveHandler(DateTimeEdit.MouseRightButtonUpEvent, new MouseButtonEventHandler(uiElement_MouseRightButtonUp));
            //uiElement.RemoveHandler(DateTimeEdit.PreviewKeyDownEvent, new KeyEventHandler(OnPreviewKeyDown));
        }

        void uiElement_IsDropDownOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            this.GridControl.CurrentCell.IsDroppedDown = (bool)e.NewValue;
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
                //There is no need to set the caret index for DateTimeEdit. so this was removed.
                //this.CurrentCellUIElement.CaretIndex = e.Text.Length;
            }
            //e.Handled = true;
        }

        protected override bool ShouldGridTryToHandlePreviewKeyDown(System.Windows.Input.KeyEventArgs e)
        {
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
                case Key.Right:
                    {
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
                            DateTimeEdit tb = this.CurrentCellUIElement;
                            if (tb != null && (tb.SelectionStart + tb.SelectionLength) == tb.Text.Length)
                            {
                                return true;
                            }
                        }
                        // otherwise, move caret within textbox when cell is not in edit-mode.
                        return !this.CurrentCell.IsEditing;
                    }
                case Key.Left:
                    {
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

                            DateTimeEdit tb = this.CurrentCellUIElement;
                            if (tb != null && tb.SelectionStart == 0)
                            {
                                return true;
                            }
                        }

                        // otherwise, move caret within textbox when cell is not in edit-mode.
                        return !this.CurrentCell.IsEditing;
                    }
                case Key.Down:
                    {
                    DateTimeEdit tb = this.CurrentCellUIElement;
                    if (this.CurrentCell.IsEditing)
                    {
                        if (CurrentCellUIElement != null)
                        {
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
                            else if (CurrentCellUIElement.IsScrollingOnCircle)
                                if(CurrentCellUIElement.Text.Length==0)
                                    return true;
                                else
                                    return false;
                            else
                                return true;
                        }
                    }
                    else if (tb != null && tb.SelectionStart == 0 && tb.SelectionLength == tb.Text.Length)
                        {
                            return true;
                        }
                        //otherwise, move caret within textbox when cell is not in edit-mode.
                        return !this.CurrentCell.IsEditing;
                }
                case Key.Up:
                    {
                        //Move to next cell when whole text in textbox is selected.
                        DateTimeEdit tb = this.CurrentCellUIElement;
                        if (this.CurrentCell.IsEditing)
                        {
                            if (CurrentCellUIElement != null)
                            {
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
                                else if (CurrentCellUIElement.IsScrollingOnCircle)
                                    return false;
                                else
                                    return true;
                            }
                        }
                        else if (tb != null && tb.SelectionStart == 0 && tb.SelectionLength == tb.Text.Length)
                        {
                            return true;
                        }
                        //otherwise, move caret within textbox when cell is not in edit-mode.
                        return !this.CurrentCell.IsEditing;
                    }
                    // return true; Unreachable code
                case Key.Delete:
                case Key.Back:
                case Key.End:
                case Key.Home:
                    {
                        if (this.CurrentCell.IsEditing || isControlKey)
                        {
                            this.CurrentCell.BeginEdit(true);
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
                        //    CurrentCell.EndEdit();
                       CurrentCell.MoveRight();
                      return true;
                        // break; Unreachable code
                    }
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
            //e.Handled = true;
        }

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
