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

#if !WinRT
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Syncfusion.Windows.Controls.Grid;
using Syncfusion.Windows.Controls.Cells;
using Syncfusion.Windows.Styles;
namespace Syncfusion.Windows.Controls.Grid
#else
using Windows.Foundation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI;
using Syncfusion.WinRT.Styles;
using Windows.UI.Xaml;
using Syncfusion.WinRT.Controls.Cells;
using Windows.UI.Core;
using Windows.System;
namespace Syncfusion.WinRT.Controls.Grid
#endif
{
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public class GridCellCheckBoxCellModel : GridCellModel<GridCellCheckBoxCellRenderer>
    {
        protected override Size OnQueryPrefferedClientSize(int rowIndex, int colIndex, GridStyleInfo style, GridQueryBounds queryBounds)
        {
            return new Size(13, 13);
        }
    }

#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public class GridCellCheckBoxCellRenderer : GridVirtualizingCellRenderer<Border>
    {
        public GridCellCheckBoxCellRenderer()
        {
            this.AllowRecycle = true;
            this.IsControlTextShown = false;
            this.SupportsRenderOptimization = false;
            this.IsFocusable = true;            
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            this.ControlValue = this.GetControlValueFromEditor();
        }

        public override void OnInitializeContent(Border border, GridRenderStyleInfo style)
        {
            base.OnInitializeContent(border, style);
            this.OnUnwireUIElement(border);
            var checkBox = new CheckBox();
            checkBox.IsThreeState = style.IsThreeState;
            border.Background = new SolidColorBrush(Colors.Transparent);
            border.Child = checkBox;
            checkBox.Measure(new Size(double.MaxValue, double.MaxValue));
            
            object value = this.GetControlText(style);
            checkBox.IsChecked = (bool?)ValueConvert.ChangeType(value, typeof(bool?), style.GetCulture(true));
            
            checkBox.HorizontalAlignment = style.HorizontalAlignment;
            checkBox.VerticalAlignment = style.VerticalAlignment;
            checkBox.BorderThickness = new Thickness(0);
#if !WinRT
            if (this.GridControl is GridDataControlBaseImpl)
            {
                if (!style.Enabled || style.ReadOnly || !(this.GridControl as GridDataControlBaseImpl).TableModel.TableProperties.AllowEdit)
                    checkBox.IsEnabled = false;
                else
                    checkBox.IsEnabled = true;
            }
            else
#endif
            {
                if (!style.Enabled || style.ReadOnly)
                    checkBox.IsEnabled = false;
                else
                    checkBox.IsEnabled = true;
            }
            Thickness margins = style.TextMargins.ToThickness();
            checkBox.Margin = margins;          
            this.OnWireUIElement(border);
        }

        protected override void OnActivated()
        {
            //base.OnActivated();
            this.GridControl.InvalidateCell(this.CellRowColumnIndex);
        }

        protected override void OnSetFocus()
        {
            base.OnSetFocus();
            var checkBox = this.GetCheckBox(this.CurrentCellUIElement);
            if (checkBox != null)
            {
#if !WinRT
                checkBox.Focus();
#else
                checkBox.Focus(FocusState.Keyboard);
#endif

            }
        }

        protected override object GetControlValueFromEditorCore(Border uiElement)
        {
            if (uiElement.Child != null)
            {
                var checkBox = uiElement.Child as CheckBox;
                return checkBox.IsChecked;
            }

            return null;
        }

        protected override void OnWireUIElement(Border border)
        {
            base.OnWireUIElement(border);
            if (border.Child != null)
            {
                var checkBox = border.Child as CheckBox;
                checkBox.Checked += new RoutedEventHandler(OnCheckedChange);
                checkBox.Unchecked += new RoutedEventHandler(OnCheckedChange);
                checkBox.Indeterminate += new RoutedEventHandler(OnCheckedChange);
            }
        }

        protected override void OnUnwireUIElement(Border border)
        {
            base.OnUnwireUIElement(border);
            if (border.Child != null)
            {
                var checkBox = border.Child as CheckBox;
                checkBox.Checked -= new RoutedEventHandler(OnCheckedChange);
                checkBox.Unchecked -= new RoutedEventHandler(OnCheckedChange);
                checkBox.Indeterminate -= new RoutedEventHandler(OnCheckedChange);
            }
        }

        private void OnCheckedChange(object sender, RoutedEventArgs e)
        {
            this.OnClickedCheckBox((CheckBox)sender);
        }

        private void OnClickedCheckBox(CheckBox checkBox)
        {
            if (!this.IsCurrentCell(checkBox))
            {
                var cellRowColumnIndex = VirtualizingCellsControl.GetCellRowColumnIndex(checkBox);
                this.CurrentCell.MoveTo(cellRowColumnIndex);
            }

            CurrentCell.BeginEdit(true);
            object value = GetControlValueFromEditor();
            if (!SetControlValue(value))
            {
                RefreshContent();
            }
        }

        public override void RaiseGridCellClick(int rowIndex, int colIndex, Scroll.MouseControllerEventArgs e)
        {
            if (CurrentCell.HasCurrentCellAt(rowIndex, colIndex)
            && !CurrentCell.IsEditing)
            //&& GridControl.Model.Options.ActivateCurrentCellBehavior != GridCellActivateAction.DblClickOnCell)
            {
                CurrentCell.BeginEdit(true);

            }
            base.RaiseGridCellClick(rowIndex, colIndex, e);
        }

        private CheckBox GetCheckBox(Border border)
        {
            if (border != null && border.Child != null)
            {
                return border.Child as CheckBox;
            }

            return null;
        }

        protected override void OnDeactivated()
        {
            this.GridControl.InvalidateCell(this.CellRowColumnIndex);
            if (this.GridControl.CurrentCell.IsModified)
            {                
                this.GridControl.InvalidateVisual(true);
            }
        }
#if !WinRT
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
                case Key.Space:
                    {
                        if (!this.CurrentCell.IsEditing)
                            this.CurrentCell.BeginEdit();
                       
                        var checkBox = this.GetCheckBox(this.CurrentCellUIElement);
                        if (checkBox != null && this.IsCurrentCell(checkBox) && !checkBox.IsFocused)
                        {
                            checkBox.Focus();
                        }
                        break;
                    }
                case Key.Tab:
                    return true;
                case Key.Home:
                    if (this.CurrentCell.IsEditing)
                        return false;
                    else
                        return true;
                case Key.Enter:
                    {
                        //isEnteryKeyPressed = true;
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
            }
            return base.ShouldGridTryToHandlePreviewKeyDown(e);
        }
#else
        public override bool ShouldGridTryToHandlePreviewKeyDown(Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            CoreVirtualKeyStates ctrl = Window.Current.CoreWindow.GetAsyncKeyState(VirtualKey.Control);
            bool isControlKey = false;

            if (ctrl.HasFlag(CoreVirtualKeyStates.Down))
                isControlKey = true;

            if (isControlKey)
                return true;

            switch (e.Key)
            {
                case VirtualKey.Space:
                    {
                        if (!this.CurrentCell.IsEditing)
                            this.CurrentCell.BeginEdit();

                        var checkBox = this.GetCheckBox(this.CurrentCellUIElement);
                        if (checkBox != null && this.IsCurrentCell(checkBox) &&
#if !WinRT
                        !checkBox.IsFocused)
                        {
                            checkBox.Focus();
                        }
#else
                        checkBox.FocusState != FocusState.Keyboard)
                        {
                            checkBox.Focus(FocusState.Keyboard);
                        }
#endif
                        break;
                    }
                case VirtualKey.Tab:
                    return true;
                case VirtualKey.Home:
                    if (this.CurrentCell.IsEditing)
                        return false;
                    else
                        return true;
                case VirtualKey.Enter:
                    {
                        //isEnteryKeyPressed = true;
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
            }
            return base.ShouldGridTryToHandlePreviewKeyDown(e);
        }
#endif
    }
}
