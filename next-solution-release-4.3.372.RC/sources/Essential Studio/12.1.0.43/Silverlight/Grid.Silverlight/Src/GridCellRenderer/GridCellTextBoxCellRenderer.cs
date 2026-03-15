#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows;
using System.Windows.Input;
using System.Globalization;

#if !WinRT
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Syncfusion.Windows.Controls.Cells;
using Syncfusion.Windows.Controls.Scroll;
using System.Windows.Controls.Primitives;
using Syncfusion.Windows.Controls.Grid;

namespace Syncfusion.Windows.Controls.Grid
#else
using Syncfusion.WinRT.Controls.Grid;
using Syncfusion.WinRT.Controls.Cells;
using Syncfusion.WinRT.Controls.Scroll;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Windows.UI.Core;
using Windows.System;
using Windows.UI.Xaml.Media;
using Windows.UI;

namespace Syncfusion.WinRT.Controls.Grid
#endif
{
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public class GridCellTextBoxModel : GridCellModel<GridCellTextBoxCellRenderer>
    {
        public GridCellTextBoxModel()
        {
        }

        public override string GetFormattedText(GridStyleInfo style, object value, int textInfo)
        {
            string text = base.GetFormattedText(style, value, textInfo);
            if (text.Contains("\r"))
                text = text.Replace("\r", Environment.NewLine);
            return text;
        }
    }
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public class GridCellTextBoxCellRenderer : GridVirtualizingCellRenderer<TextBox>
    {
        int textBoxSelectionStart = -1;
        int textBoxSelectionLength = 0;
        // int ccSelectionStart, ccSelectionLength; Variable assigned but it is not used

        public GridCellTextBoxCellRenderer()
        {
            this.AllowRecycle = true;
            this.AllowKeepAliveOnlyCurrentCell = true;
            this.IsControlTextShown = true;
            this.SupportsRenderOptimization = true;
            this.IsFocusable = true;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            this.textBoxSelectionStart = -1;
            this.ControlText = this.GetControlText(this.CurrentStyle);
        }

        protected override void ArrangeUIElement(ArrangeCellArgs aca, TextBox uiElement, GridRenderStyleInfo style)
        {
            Thickness margins = style.TextMargins.ToThickness();
            if (style.HasImageIndex)
            {
                margins = style.AdjustImageWidthAndHeightToMargin(margins, style.GridControl);
            }
            else
            {
#if!WinRT
                margins = style.ErrorInfo.AdjustErrorInfoMarginOnEditing(margins, style.GridControl, style.CellRowColumnIndex);
#endif
            }
            uiElement.Padding = margins;

            if (GridControl.Model.Options.ExcelLikeSelectionFrame)
            {
                var tbMargin = uiElement.Margin;
                tbMargin.Left += 2;
                uiElement.Padding = tbMargin;
            }

            base.ArrangeUIElement(aca, uiElement, style);
        }

        public override void OnInitializeContent(TextBox textBox, GridRenderStyleInfo style)
        {
            this.OnUnwireUIElement(textBox);
            Thickness margins = style.TextMargins.ToThickness();
            margins.Left = Math.Max(0, margins.Left - 2);
            margins.Right = Math.Max(0, margins.Right - 2);
            var font = style.ReadOnlyFont;
            var tb = textBox;
            tb.AcceptsReturn = true;
#if !WinRT
            if (style.HasImageIndex)
            {
                margins = style.AdjustImageWidthAndHeightToMargin(margins, style.GridControl);
            }
            else
            {
                margins = style.ErrorInfo.AdjustErrorInfoMarginOnEditing(margins, style.GridControl, style.CellRowColumnIndex);
            }
#endif
            tb.Padding = margins;
            tb.FontFamily = font.FontFamily;
            tb.FontSize = font.FontSize;
            tb.FontStretch = font.FontStretch;
            tb.FontWeight = font.FontWeight;
            tb.FontStyle = font.FontStyle;
            tb.Foreground = style.Foreground;
            tb.Background = style.Background;
            tb.IsEnabled = !style.ReadOnly;
            //tb.IsReadOnly = style.ReadOnly;
            tb.HorizontalAlignment = style.HorizontalAlignment;
            tb.TextWrapping = style.TextWrapping;
            tb.VerticalAlignment = style.VerticalAlignment;
            tb.VerticalContentAlignment = style.VerticalAlignment;
            switch (style.HorizontalAlignment)
            {
                case HorizontalAlignment.Center:
                    tb.TextAlignment = TextAlignment.Center;
                    break;
                case HorizontalAlignment.Left:
                    tb.TextAlignment = TextAlignment.Left;
                    break;
                case HorizontalAlignment.Right:
                    tb.TextAlignment = TextAlignment.Right;
                    break;
                case HorizontalAlignment.Stretch:
                    tb.TextAlignment = TextAlignment.Justify;
                    break;
                default:
                    tb.TextAlignment = TextAlignment.Left;
                    break;
            }
            if (this.IsCurrentCell(style) && HasControlText)
            {
                textBox.Text = this.ControlText;
            }
            else
            {
                textBox.Text = GetControlText(style);
            }
            textBox.BorderThickness = new Thickness(0);
            VisualContainer.SetWantsMouseInput(textBox, false);
            this.OnWireUIElement(textBox);
            //In ExcelLikeSelectionFrame, the current cell border was lager then the normal border.
            //So the textbox cursor was disappeared in the current cell border.
            //To fix this issue we need to increase the textbox lext margin.
            if (GridControl.Model.Options.ExcelLikeSelectionFrame)
            {
                var tbMargin = textBox.Margin;
                tbMargin.Left += 2;
                textBox.Padding = tbMargin;
            }
        }

        protected override string GetControlTextFromEditorCore(TextBox uiElement)
        {
            return uiElement.Text;
        }

        protected override void OnWireUIElement(TextBox textBox)
        {
            base.OnWireUIElement(textBox);
            textBox.TextChanged += new TextChangedEventHandler(textBox_TextChanged);
            textBox.SelectionChanged += new RoutedEventHandler(textBox_SelectionChanged);
#if !WinRT
            textBox.KeyDown += new KeyEventHandler(textBox_KeyDown);
#else
            textBox.KeyDown += textBox_KeyDown;
#endif
        }
#if WinRT
        void textBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            switch (e.Key)
            {
                case VirtualKey.Escape:
                    this.ActivateOptions.Element = null;
                    break;
            }
        }
#endif
        protected override void OnUnwireUIElement(TextBox textBox)
        {
            base.OnUnwireUIElement(textBox);
            textBox.TextChanged -= new TextChangedEventHandler(textBox_TextChanged);
            textBox.SelectionChanged -= new RoutedEventHandler(textBox_SelectionChanged);
#if !WinRT
            textBox.KeyDown -= new KeyEventHandler(textBox_KeyDown);
#else
            textBox.KeyDown -= textBox_KeyDown;
#endif
        }

#if !WinRT

        void textBox_KeyDown(object sender, KeyEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            switch (e.Key)
            {
                case Key.Escape:
                    this.ActivateOptions.Element = null;
                    break;
            }
        }
#endif

        void textBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (!IsInArrange)
            {
                textBoxSelectionStart = textBox.SelectionStart;
                textBoxSelectionLength = textBox.SelectionLength;
            }

            if (HasCurrentCellState)
                GridControl.InvalidateCell(this.CellRowColumnIndex);
        }

        void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (!(textBox.Text.Equals((object)this.CurrentStyle.CellValue)) || IsModified)
            {
                if (!this.IsInArrange && IsCurrentCell(textBox) && !CurrentCell.IsInEndEdit)
                {
                    if (!SetControlText(textBox.Text))
                    {
                        RefreshContent();
                    }
                    else
                    {
                        textBox.InvalidateMeasure();
                    }
                }
            }
        }

        protected override void OnEnteredEditMode()
        {
            if(this.CurrentStyle.IsChanged)
               UpdateTextBoxText();
        }

        protected override void OnActivated()
        {
            UpdateTextBoxText();
        }

        protected override void OnEditingComplete()
        {
			GridControl.InvalidateCell(CellRowColumnIndex);
            if (this.isEnteryKeyPressed)
            {
                GridControl.InvalidateVisual(true);
            }
        }

        protected override void OnDeactivated()
        {
            GridControl.InvalidateCell(CellRowColumnIndex);
            if (!this.isEnteryKeyPressed)
            {
                if (this.CurrentStyle.IsChanged)
                    GridControl.InvalidateVisual(true);
            }
            else
            {
                this.isEnteryKeyPressed = false;
            }
        }

        private void UpdateTextBoxText()
        {
            if (CurrentCellUIElement != null)
            {
                GridRenderStyleInfo style = CurrentStyle;
                string text = GetControlText(style);
                CurrentCellUIElement.Text = text;
            }
            else
            {
                GridControl.InvalidateCell(CellRowColumnIndex);
            }
        }

        public override void RefreshContent()
        {
            base.RefreshContent();
            if (textBoxSelectionStart != -1 && CurrentCellUIElement != null)
                CurrentCellUIElement.Select(textBoxSelectionStart, textBoxSelectionLength);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

#if SyncfusionFramework4_0
        protected override void OnGridPreviewTextInput(TextCompositionEventArgs e)
        {
            if (CurrentCell.IsEditing)
                return;

            CurrentCell.ScrollInView();
            CurrentCell.BeginEdit(true);
            TextBox tb = CurrentCellUIElement;
            if (tb != null)
            {
                /// tb.TextChanged += new TextChangedEventHandler(tb_TextChanged);
                System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"[\b]");
                string str = regex.Replace(e.Text, "");
                if (!str.Equals("\r") || GridControl.Model.EnableMultiline)
                {
                    tb.Text = str;
                    tb.Select(tb.Text.Length, 1);
                }

                tb.Focus();

            }


            e.Handled = true;
        }

        void tb_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
#endif

        protected override void OnSetFocus()
        {
            if (this.GridControl.Model.Options.ActivateCurrentCellBehavior == GridCellActivateAction.SelectAll)
                CurrentCellUIElement.SelectAll();
            else
            {
                CurrentCellUIElement.Select(CurrentCellUIElement.Text.Length, 0);
#if !WinRT
                CurrentCellUIElement.Focus();
#else
                CurrentCellUIElement.Focus(FocusState.Programmatic);
#endif
            }
        }

        private bool isEnteryKeyPressed = false;
        
#if WinRT
        public override bool ShouldGridTryToHandlePreviewKeyDown(KeyRoutedEventArgs e)
        {
            bool isControlKey = false;
            bool isShiftKey = false;
            CoreVirtualKeyStates ctrl = Window.Current.CoreWindow.GetAsyncKeyState(VirtualKey.Control);
            if (ctrl == CoreVirtualKeyStates.Down)
                isControlKey = true;

            CoreVirtualKeyStates shift = Window.Current.CoreWindow.GetAsyncKeyState(VirtualKey.Shift);
            if (shift == CoreVirtualKeyStates.Down)
                isShiftKey = true;

            if (isControlKey)
                return true;

            switch (e.Key)
            {
                case VirtualKey.Enter:
                    {
                        if (isShiftKey && this.GridControl.Model.EnableMultiline)
                        {
                            return false;
                        }
                        else
                        {
                            if (this.GridControl is GridControl)
                            {
                                if (this.GridControl.Model.Options.EnterKeyBehaviour == EnterKeyBehaviour.MouseDown)
                                    this.CurrentCell.MoveDown();
                                else
                                    this.CurrentCell.MoveRight();
                                CurrentCell.ScrollInView();
                                e.Handled = true;
                                return false;
                            }
                            e.Handled = true;
                            this.CurrentCell.MoveRight();
                            CurrentCell.ScrollInView();
                            return false;
                        }
                    }
                case VirtualKey.Tab:
                    return true;

                case VirtualKey.Right:
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
                case VirtualKey.Left:
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
                                e.Handled = true;
                                return false;
                            }
                            else

                                return false;
                        }
                        return false;
                    }
                    return true;
                case VirtualKey.Down:
                    if (CurrentCell.IsEditing)
                    {
                        // Move to next cell when whole text in textbox is selected.
                        TextBox tb = CurrentCellUIElement;
                        if (tb != null && tb.SelectionStart == 0 && tb.SelectionLength == tb.Text.Length)
                            return true;
                        else
                        {
                            this.CurrentCell.MoveDown();
                            e.Handled = true;
                            return false;
                        }
                    }
                    return true;
                case VirtualKey.Up:
                    if (CurrentCell.IsEditing)
                    {
                        // Move to next cell when whole text in textbox is selected.
                        TextBox tb = CurrentCellUIElement;
                        if (tb != null && tb.SelectionStart == 0 && tb.SelectionLength == tb.Text.Length)
                            return true;
                        else
                        {
                            this.CurrentCell.MoveUp();
                            e.Handled = true;
                            return false;
                        }
                    }
                    return true;

                case VirtualKey.End:
                    return true;
                case VirtualKey.Home:
                    if (this.CurrentCell.IsEditing)
                    {
                        e.Handled = true;
                        return false;
                    }
                    else
                        return true;
                case VirtualKey.Delete:
                    {
                        CurrentCell.BeginEdit(true);
                        SetControlText("");
                        return false;
                    }
                case VirtualKey.F2:
                    {
                        TextBox textBox = this.CurrentCellUIElement;
                        if (textBox != null && this.CurrentCell.IsEditing && textBox.SelectionLength == textBox.Text.Length)
                        {
                            textBox.SelectionStart = textBox.Text.Length;
                            e.Handled = true;
                            return false;
                        }
                    }
                    break;

            }

            return base.ShouldGridTryToHandlePreviewKeyDown(e);
        }
#else
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
                        if (isShiftKey && this.GridControl.Model.EnableMultiline)
                        {
                            return false;
                        }
                        else
                        {
                            if (this.GridControl is GridControl)
                            {
                                if (this.GridControl.Model.Options.EnterKeyBehaviour == EnterKeyBehaviour.MouseDown)
                                    this.CurrentCell.MoveDown();
                                else
                                    this.CurrentCell.MoveRight();
                                CurrentCell.ScrollInView();
                                e.Handled = true;
                                return false;
                            }
                            e.Handled = true;
                            this.CurrentCell.MoveRight();
                            CurrentCell.ScrollInView();
                            return false;
                        }
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
                                e.Handled = true;
                                return false;
                            }
                            else

                                return false;
                        }
                        return false;
                    }
                    return true;
                //e.Handled = true; Unreachable code
                case Key.Down:
                    if (CurrentCell.IsEditing)
                    {
                        // Move to next cell when whole text in textbox is selected.
                        TextBox tb = CurrentCellUIElement;
                        if (tb != null && tb.SelectionStart == 0 && tb.SelectionLength == tb.Text.Length)
                            return true;
                        else
                        {
                            this.CurrentCell.MoveDown();
                            e.Handled = true;
                            return false;
                        }

#if SILVERLIGHT
                        // e.Handled = this.CurrentCell.IsEditing; Unreachable code
#endif
                        // otherwise, move caret within textbox when cell is not in edit-mode.
                        // return !CurrentCell.IsEditing; Unreachable code
                    }
                    return true;
                case Key.Up:
                    if (CurrentCell.IsEditing)
                    {
                        // Move to next cell when whole text in textbox is selected.
                        TextBox tb = CurrentCellUIElement;
                        if (tb != null && tb.SelectionStart == 0 && tb.SelectionLength == tb.Text.Length)
                            return true;
                        else
                        {
                            this.CurrentCell.MoveUp();
                            e.Handled = true;
                            return false;
                        }

#if SILVERLIGHT
                        // e.Handled = this.CurrentCell.IsEditing; Unreachable code
#endif
                        // otherwise, move caret within textbox when cell is not in edit-mode.
                        // return !CurrentCell.IsEditing; Unreachable code
                    }
                    return true;

                case Key.End:
                    return true;
                case Key.Home:
                    if (this.CurrentCell.IsEditing)
                    {
                        e.Handled = true;
                        return false;
                    }
                    else
                        return true;
                case Key.Delete:
                    {
                        CurrentCell.BeginEdit(true);
                        SetControlText("");
                        return false;
                    }
                case Key.F2:
                    {
                        TextBox textBox = this.CurrentCellUIElement;
                        if (textBox != null && this.CurrentCell.IsEditing && textBox.SelectionLength == textBox.Text.Length)
                        {
                            textBox.SelectionStart = textBox.Text.Length;
                            e.Handled = true;
                            return false;
                        }
                    }
                    break;

            }
            return base.ShouldGridTryToHandlePreviewKeyDown(e);
        }
#endif
        public override void RaiseGridCellClick(int rowIndex, int colIndex, MouseControllerEventArgs e)
        {
            if ((CurrentCell.HasCurrentCellAt(rowIndex, colIndex) && !CurrentCell.IsEditing) &&
                 (GridControl.Model.Options.ActivateCurrentCellBehavior != GridCellActivateAction.DblClickOnCell))
            {
                CurrentCell.BeginEdit(true);
            }
            base.RaiseGridCellClick(rowIndex, colIndex, e);
        }
    }
}
