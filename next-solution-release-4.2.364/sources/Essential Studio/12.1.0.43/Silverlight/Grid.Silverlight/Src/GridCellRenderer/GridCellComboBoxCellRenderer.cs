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
using System.Linq;
using System.Reflection;
#if !WinRT
using Syncfusion.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Syncfusion.Windows.Controls.Scroll;
using Syncfusion.Linq;
using Syncfusion.Windows.Data;
namespace Syncfusion.Windows.Controls.Grid
#else
using Syncfusion.WinRT.Controls.Grid;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using Syncfusion.WinRT.Controls.Scroll;
using Windows.UI.Core;
using Windows.System;
namespace Syncfusion.WinRT.Controls.Grid
#endif
{
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public class GridCellComboBoxCellModel : GridCellModel<GridComboBoxCellRenderer>
    {
        public GridCellComboBoxCellModel()
        {
        }

        public override string GetText(GridStyleInfo style, object value)
        {
            if (!string.IsNullOrEmpty(style.DisplayMember) && string.IsNullOrEmpty(style.ValueMember))
            {
                Type type = value.GetType();
#if !WinRT
                var propertyInfoCollection = type.GetProperties();
#else
                var propertyInfoCollection = type.GetTypeInfo().DeclaredProperties;
#endif
                var propertyInfo = propertyInfoCollection.FirstOrDefault(p => p.Name == style.DisplayMember);
                if (propertyInfo != null)
                {
                    var displayvalue = propertyInfo.GetValue(value);
                    return displayvalue != null ? displayvalue.ToString() : string.Empty;
                }
            }
            if (style.ItemsSource != null && (!string.IsNullOrEmpty(style.ValueMember)))
            {
                PropertyInfo propertyInfo = null;
                foreach (var record in style.ItemsSource)
                {
                    Type type = record.GetType();
#if !WinRT
                    var propertyInfoCollection = type.GetProperties();
#else
                    var propertyInfoCollection = type.GetTypeInfo().DeclaredProperties;
#endif
                    propertyInfo = propertyInfoCollection.FirstOrDefault(p => p.Name == style.ValueMember);
                    break;
                }

                if (propertyInfo != null)
                {
                    foreach (var record in style.ItemsSource)
                    {
                        if (value.ToString().Equals(propertyInfo.GetValue(record).ToString()))
                        {
                            Type type = record.GetType();
#if !WinRT
                            var propertyInfoCollection = type.GetProperties();
#else
                            var propertyInfoCollection = type.GetTypeInfo().DeclaredProperties;
#endif
                            propertyInfo = propertyInfoCollection.FirstOrDefault(p => p.Name == style.DisplayMember);
                            if (propertyInfo != null)
                            {
                                var displayvalue = propertyInfo.GetValue(record);
                                return displayvalue != null ? displayvalue.ToString() : string.Empty;
                            }
                            else
                                break;
                        }
                    }
                }
            }
            return base.GetText(style, value);
        }

        public override string GetFormattedText(GridStyleInfo style, object value, int textInfo)
        {
            return this.GetText(style, value);
        }
    }

#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public class GridComboBoxCellRenderer : GridVirtualizingCellRenderer<ComboBox>
    {
        public GridComboBoxCellRenderer()
        {
            this.AllowRecycle = true;
            this.SupportsRenderOptimization = true;
            this.IsFocusable = true;
            this.IsControlTextShown = false;
            this.AllowKeepAliveOnlyCurrentCell = true;
            this.IsDropDownable = true;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            this.ControlValue = this.GetControlValueFromEditor();
        }

        public override void OnInitializeContent(ComboBox comboBox, GridRenderStyleInfo style)
        {
            if (comboBox.ItemsSource == null)
                comboBox.ItemsSource = style.ItemsSource ?? style.ItemsSource;
            DataTemplate dt = null;
            if (style.CellItemTemplateKey != null)
            {
                dt = (DataTemplate)style.GridControl.TryFindResource(style.CellItemTemplateKey);
            }
            else if (style.CellItemTemplate != null)
            {
                dt = style.CellItemTemplate;

            }
            comboBox.ItemTemplate = dt ?? dt;
            if (dt == null && !string.IsNullOrEmpty(style.DisplayMember))
            {
                comboBox.DisplayMemberPath = style.DisplayMember ?? style.DisplayMember;
                comboBox.SelectedValuePath = style.ValueMember ?? style.ValueMember;
            }
            /*if (this.IsCurrentCell(comboBox) && this.ControlValue != null)
            {
                comboBox.SelectedItem = this.ControlValue;
            }
            else
            {
                comboBox.SelectedItem = style.CellValue ?? style.CellValue;
            }*/

            if (comboBox.SelectedItem == null && this.HasCurrentCellState && this.ControlValue != null)
                comboBox.SelectedValue = this.ControlValue;
            else
                style.CellValue = comboBox.SelectedValue;

            Thickness margins = style.TextMargins.ToThickness();
            margins.Left = Math.Max(0, margins.Left - 2);
            margins.Right = Math.Max(0, margins.Right - 2);
            comboBox.Margin = margins;
            comboBox.Padding = style.BorderMargins.ToThickness();
            var font = style.ReadOnlyFont;
            comboBox.FontFamily = font.FontFamily;
            comboBox.FontSize = font.FontSize;
            comboBox.FontStretch = font.FontStretch;
            comboBox.FontWeight = font.FontWeight;
            comboBox.FontStyle = font.FontStyle;
            comboBox.Foreground = style.Foreground;
            comboBox.Background = style.Background;
#if !WinRT
            if (this.GridControl is GridDataControlBaseImpl)
            {
                if ((this.GridControl as GridDataControlBaseImpl).TableModel.TableProperties.VisualStyle == VisualStyle.Office14Blue)
                    comboBox.Foreground = new GridDataOffice14BlueVisualStyle().ValueForegroundBrush;
            }
#endif

            comboBox.HorizontalAlignment = style.HorizontalAlignment;
            comboBox.VerticalAlignment = style.VerticalAlignment;
            VisualContainer.SetWantsMouseInput(comboBox, false);
            comboBox.BorderThickness = new Thickness(1);
        }

        protected override object GetControlValueFromEditorCore(ComboBox comboBox)
        {
            return comboBox.SelectedValue;
        }

        protected override void OnEnteredEditMode()
        {
            UpdateComboBox();
        }

        bool isEnteryKeyPressed = false;
        protected override void OnDeactivated()
        {
            UpdateComboBox();
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

        protected override void OnActivated()
        {
            UpdateComboBox();
        }

        /// <summary>
        /// This Method Used to refresh the UIElement
        /// </summary>
        private void UpdateComboBox()
        {
            if (CurrentCellUIElement != null)
            {
                if (this.ControlValue != null && this.ControlValue.ToString() != string.Empty)
                {
                    CurrentCellUIElement.SelectedValue = (this.ControlValue);
                }

            }
            else
            {
                GridControl.InvalidateCell(CellRowColumnIndex);
                GridControl.InvalidateVisual(true);
            }
        }


        protected override void OnWireUIElement(ComboBox comboBox)
        {
            base.OnWireUIElement(comboBox);
            comboBox.SelectionChanged += new SelectionChangedEventHandler(comboBox_SelectionChanged);
        }

        /// <summary>
        /// In this event we have Unload the CurrentCell UIElement Because if Dynamically set itemsource means Dropdown list doesnt load so that we have Unload and reload the UIElement.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        protected override void OnUnwireUIElement(ComboBox comboBox)
        {
            base.OnUnwireUIElement(comboBox);
            comboBox.SelectionChanged -= new SelectionChangedEventHandler(comboBox_SelectionChanged);
        }

        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;

            if (this.IsCurrentCell(comboBox))
            {
                var value = this.GetControlValueFromEditorCore(comboBox);
                if (!this.SetControlValue(value))
                {
                    this.RefreshContent();
                }
                var contPresentBorder = comboBox.FindElementsOfType<Border>().Where(o => o != null && o.Name == "ContentPresenterBorder").FirstOrDefault();
                if (contPresentBorder != null)
                {
                    var contPresenter = contPresentBorder.FindElementsOfType<ContentPresenter>();
                    foreach (var item in contPresenter)
                    {
                        if (item != null && item.Name == "ContentPresenter")
                        {
                            var tb = item.FindElementOfType<TextBlock>();
                            if (tb == null)
                                tb = item.Content as TextBlock;
                            if (tb != null)
                            {
                                if (string.IsNullOrEmpty(this.CurrentStyle.DisplayMember))
                                {
                                    if (value != null)
                                    {
                                        if (string.IsNullOrWhiteSpace(tb.Text))
                                        {
                                            CurrentCell.UnloadCurrentCellUIElement();
#if WinRT
                                        GridControl.Focus(FocusState.Keyboard);
#else
                                            GridControl.Focus();
#endif
                                        }
                                        tb.Text = value.ToString();
                                    }
                                }
                                else
                                {
                                    var valueType = comboBox.SelectedItem.GetType();
#if !WinRT
                                    var properties = valueType.GetProperties();
#else
                                    var properties = valueType.GetTypeInfo().DeclaredProperties;
#endif
                                    var displayProp = properties.FirstOrDefault(p => p.Name == this.CurrentStyle.DisplayMember);
                                    if (displayProp != null)
                                    {
                                        var displayText = displayProp.GetValue(comboBox.SelectedItem, null);
                                        tb.Text = displayText != null ? displayText.ToString() : string.Empty;
                                    }
                                }
                            }
                            else
                            {
                                CurrentCell.UnloadCurrentCellUIElement();
#if ! WinRT
                                GridControl.Focus();
#else
                                GridControl.Focus(FocusState.Keyboard);
#endif
                            }
                        }
                    }
                }
            }
        }

        public override void RaiseGridCellClick(int rowIndex, int colIndex, MouseControllerEventArgs e)
        {
            if (CurrentCell.HasCurrentCellAt(rowIndex, colIndex)
                && !CurrentCell.IsEditing
                && GridControl.Model.Options.ActivateCurrentCellBehavior != GridCellActivateAction.DblClickOnCell)
            {
                CurrentCell.BeginEdit(true);
            }
            base.RaiseGridCellClick(rowIndex, colIndex, e);
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
                case Key.Home:
                    if (this.CurrentCell.IsEditing)
                        return false;
                    else
                        return true;
                case Key.Space:
                    if (this.CurrentCell.IsEditing)
                    {
                        if (this.CurrentCellUIElement != null)
                        {
                            var combo = this.CurrentCellUIElement as ComboBox;
                            if (combo != null)
                            {
                                combo.IsDropDownOpen = !combo.IsDropDownOpen;
                            }
                        }
                        return false;
                    }
                    else
                    {
                        return true;
                    }

            }
            return base.ShouldGridTryToHandlePreviewKeyDown(e);
        }

#else
        public override bool ShouldGridTryToHandlePreviewKeyDown(Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            CoreVirtualKeyStates ctrl = Window.Current.CoreWindow.GetAsyncKeyState(VirtualKey.Control);
            CoreVirtualKeyStates shift = Window.Current.CoreWindow.GetAsyncKeyState(VirtualKey.Control);
            bool isControlKey = false;
            bool isShiftKey = false;

            if (ctrl.HasFlag(CoreVirtualKeyStates.Down))
                isControlKey = true;

            if (shift.HasFlag(CoreVirtualKeyStates.Down))
                isShiftKey = true;

            if (isControlKey)
                return true;

            switch (e.Key)
            {
                case VirtualKey.Enter:
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
                    break;
                case VirtualKey.Tab:
                    return true;
                case VirtualKey.Home:
                    if (this.CurrentCell.IsEditing)
                        return false;
                    else
                        return true;
                case VirtualKey.Space:
                    if (this.CurrentCell.IsEditing)
                    {
                        if (this.CurrentCellUIElement != null)
                        {
                            var combo = this.CurrentCellUIElement as ComboBox;
                            if (combo != null)
                            {
                                combo.IsDropDownOpen = !combo.IsDropDownOpen;
                            }
                        }
                        return false;
                    }
                    else
                    {
                        return true;
                    }

            }
            return base.ShouldGridTryToHandlePreviewKeyDown(e);
        }
#endif
    }
}
