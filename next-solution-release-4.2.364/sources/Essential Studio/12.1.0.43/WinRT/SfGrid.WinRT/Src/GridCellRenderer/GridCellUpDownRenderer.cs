#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#if WinRT
using System;
using Syncfusion.UI.Xaml.Controls.Input;
using Syncfusion.UI.Xaml.ScrollAxis;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
#else
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Syncfusion.UI.Xaml.Grid.Utility;
using Syncfusion.UI.Xaml.ScrollAxis;
#if WPF
using Syncfusion.Windows.Shared;
#elif SILVERLIGHT
using Syncfusion.Windows.Tools.Controls;
using UpDown = Syncfusion.Windows.Controls.NumericUpDown;
#endif
#endif

namespace Syncfusion.UI.Xaml.Grid.Cells
{
#if WinRT
    using Key = Windows.System.VirtualKey;
    using KeyEventArgs = KeyRoutedEventArgs;
    using EventArgs = PointerRoutedEventArgs;
#endif
    [ClassReference(IsReviewed = false)]
#if WinRT
    public class GridCellUpDownRenderer : GridVirtualizingCellRenderer<TextBlock, SfNumericUpDown>
#else
    public class GridCellUpDownRenderer:GridVirtualizingCellRenderer<TextBlock, UpDown>
#endif
    {

        #region Ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="GridCellUpDownRenderer"/> class.
        /// </summary>
        public GridCellUpDownRenderer()
        {
            SupportsRenderOptimization = true;
            IsFocusible = true;
            IsEditable = true;
        }
        #endregion

        #region Override Methods
        /// <summary>
        /// Called when [initialize display element].
        /// </summary>
        /// <param name="rowColumnIndex">Index of the row column.</param>
        /// <param name="uiElement">The UI element.</param>
        /// <param name="column">The column.</param>
        /// <param name="dataContext">The data context.</param>
        public override void OnInitializeDisplayElement(RowColumnIndex rowColumnIndex, TextBlock uiElement, GridColumn column, object dataContext)
        {
            uiElement.SetBinding(TextBlock.TextProperty, column.DisplayBinding);
#if WinRT
            uiElement.Padding = new Thickness(10, 4, 0, 0);
#else
            uiElement.Padding = new Thickness(3, 3, 0, 0);
#endif
            var padding = column.ReadLocalValue(GridColumn.PaddingProperty);
            if (padding != DependencyProperty.UnsetValue)
                uiElement.Padding = column.Padding;
            uiElement.TextAlignment = column.TextAlignment;
        }

        /// <summary>
        /// Called when [initialize edit element].
        /// </summary>
        /// <param name="rowColumnIndex">Index of the row column.</param>
        /// <param name="uiElement">The UI element.</param>
        /// <param name="column">The column.</param>
        /// <param name="dataContext">The data context.</param>
#if WinRT
        public override void OnInitializeEditElement(RowColumnIndex rowColumnIndex, SfNumericUpDown uiElement, GridColumn column, object dataContext)
#else
        public override void OnInitializeEditElement(RowColumnIndex rowColumnIndex, UpDown uiElement, GridColumn column, object dataContext)
#endif
        {
            var upDownColumn = column as GridUpDownColumn;
            ProcessEditBinding(uiElement, upDownColumn);
            var padding = column.ReadLocalValue(GridColumn.PaddingProperty);
            if (padding != DependencyProperty.UnsetValue)
                uiElement.Padding = column.Padding;
            uiElement.HorizontalContentAlignment = TextAlignmentToHorizontalAlignment(column.TextAlignment);
#if WPF
            uiElement.TextAlignment = TextAlignment.Left;
#endif
        }

#if WinRT
        protected override void SetFocus(UIElement uiElement, bool needToFocus)
        {
            base.SetFocus(uiElement, needToFocus);
        }
#endif
        public override void SetControlValue(object value)
        {
            if (HasCurrentCellState)
            {
                Double _value;
                Double.TryParse(value.ToString(), out _value);
                if (IsInEditing)
#if WinRT
                    ((SfNumericUpDown)CurrentCellRendererElement).Value = (value == null ? double.MinValue : _value);
#else
                    ((UpDown)CurrentCellRendererElement).Value = (value == null ? double.MinValue : _value);
#endif
                else
                    ((TextBlock)CurrentCellRendererElement).Text = (value == null ? string.Empty : value.ToString());
            }
        }

        /// <summary>
        /// Called when [update display binding].
        /// </summary>
        /// <param name="cellRowcolumnIndex">Index of the cell rowcolumn.</param>
        /// <param name="uiElement">The UI element.</param>
        /// <param name="column">The column.</param>
        /// <param name="dataContext">The data context.</param>
        public override void OnUpdateDisplayBinding(RowColumnIndex cellRowcolumnIndex, TextBlock uiElement, GridColumn column, object dataContext)
        {
            uiElement.SetBinding(TextBlock.TextProperty, column.DisplayBinding);
            uiElement.Padding = new Thickness(6, 3, 0, 0);
            var padding = column.ReadLocalValue(GridColumn.PaddingProperty);
            if (padding != DependencyProperty.UnsetValue)
                uiElement.Padding = column.Padding;
            uiElement.TextAlignment = column.TextAlignment;
        }

        /// <summary>
        /// Called when [update edit binding].
        /// </summary>
        /// <param name="cellRowcolumnIndex">Index of the cell rowcolumn.</param>
        /// <param name="element">The element.</param>
        /// <param name="column">The column.</param>
        /// <param name="dataContext">The data context.</param>
#if WinRT
        public override void OnUpdateEditBinding(RowColumnIndex cellRowcolumnIndex, SfNumericUpDown element, GridColumn column, object dataContext)
#else
        public override void OnUpdateEditBinding(RowColumnIndex cellRowcolumnIndex, UpDown element, GridColumn column, object dataContext)
#endif
        {
            ProcessClearBinding(element);
#if WinRT      
            element.SetBinding(SfNumericUpDown.ValueProperty, column.ValueBinding);
#else
            var bind = column.ValueBinding.CreateEditBinding(column.UpdateTrigger);
            element.SetBinding(CurrencyTextBox.ValueProperty, bind);
            ProcessEditBinding(element, column as GridUpDownColumn);
#endif
            element.Padding = column.Padding;

        }

        /// <summary>
        /// Let Renderer decide whether the parent grid should be allowed to handle keys and prevent
        /// the key event from being handled by the visual UIElement for this renderer. If this method
        /// returns true the parent grid will handle arrow keys and set the Handled flag in the event
        /// data. Keys that the grid does not handle will be ignored and be routed to the UIElement
        /// for this renderer.
        /// </summary>
        /// <param name="e">A <see cref="KeyEventArgs" /> object.</param>
        /// <returns>
        /// True if the parent grid should be allowed to handle keys; false otherwise.
        /// </returns>
        protected override bool ShouldGridTryToHandleKeyDown(KeyEventArgs e)
        {
            if (!this.HasCurrentCellState)
                return true;
#if !WinRT
            var CurrentCellUIElement = CurrentCellRendererElement as UpDown;
#endif
            switch (e.Key)
            {
                case Key.Enter:
                case Key.F2:
                case Key.PageDown:
                case Key.PageUp:
                case Key.Left:
                case Key.Right:
                case Key.Tab:
                case Key.Delete:
                    return true;
                case Key.C:
                case Key.V:
                case Key.X:
                case Key.A:
                    {
                        if (CheckControlKeyPressed() && !IsInEditing)
                            return true;
                        return false;
                    }
                case Key.Escape:
                    {
                        if (this.IsInEditing)
                        {
#if !WinRT
                            var editUIElement = CurrentCellRendererElement as UpDown;
                            if (editUIElement != null) editUIElement.ClearValue(UpDown.ValueProperty);
#else
                            var editUIElement = CurrentCellRendererElement as SfNumericUpDown;
                            if (editUIElement != null) editUIElement.ClearValue(SfNumericUpDown.ValueProperty);
#endif
                            return true;
                        }
                        return false;
                    }
                case Key.Up:
                case Key.Down:
                    return !IsInEditing;
            }
            return base.ShouldGridTryToHandleKeyDown(e);
        }

        #endregion
        #region PreviewTextInput Override
        /// <summary>
        /// Called when text is entered in the Data Control
        /// </summary>
        /// <param name="e">KeyRoutedEventArgs</param>
#if WinRT
        protected override void OnPreviewTextInput(KeyEventArgs e)
        {
            if (e.Key >= Key.Number0 && e.Key <= Key.Number9)
                PreviewInputText = (e.Key - Key.Number0);
            else if (e.Key >= Key.NumberPad0 && e.Key <= Key.NumberPad9)
                PreviewInputText = (e.Key - Key.NumberPad0);
            else
                PreviewInputText = 0;
        }
#endif
        #endregion

#if WinRT
        protected override void OnUnwireEditUIElement(SfNumericUpDown uiElement)
        {
            uiElement.ValueChanged -= OnValueChanged;
        }
#endif
#if WinRT
        private void ProcessPreviewTextInput(KeyEventArgs e)
        {
            if ((!char.IsLetterOrDigit(e.Key.ToString(), 0) || !DataGrid.AllowEditing || DataGrid.NavigationMode != NavigationMode.Cell) || CheckControlKeyPressed() || (!(e.Key >= Key.A && e.Key <= Key.Z) && !(e.Key >= Key.Number0 && e.Key <= Key.Number9) && !(e.Key >= Key.NumberPad0 && e.Key <= Key.NumberPad9)))
                return;
            if (DataGrid.SelectionController.CurrentCellManager.BeginEdit())
                PreviewTextInput(e);
        }
#endif
        protected override void OnEditElementLoaded(object sender, RoutedEventArgs e)
        {
#if WinRT            
            var uiElement = ((SfNumericUpDown)sender);
            uiElement.ValueChanged +=OnValueChanged;            
            double value = double.MinValue ;
            if(HasCurrentCellState)
            uiElement.Focus(FocusState.Programmatic);
            if (PreviewInputText != null)
            {
                double.TryParse(PreviewInputText.ToString(), out value);
                uiElement.Value = value;                
            }
            PreviewInputText = null;
            base.OnEditElementLoaded(sender, e);
#endif
        }

        private void OnValueChanged(object sender, ValueChangedEventArgs e)
        {
            base.CurrentRendererValueChanged();
        }

        #region Private Methods
        /// <summary>
        /// Processes the edit binding.
        /// </summary>
        /// <param name="uiElement">The UI element.</param>
        /// <param name="upDownColumn">Up down column.</param>
#if WinRT
        private void ProcessEditBinding(SfNumericUpDown uiElement, GridUpDownColumn upDownColumn)
#else
        private void ProcessEditBinding(UpDown uiElement, GridUpDownColumn upDownColumn)
#endif
        {
#if WinRT
            uiElement.SetBinding(SfNumericUpDown.ValueProperty, upDownColumn.ValueBinding);
            var bind = new Binding { Path = new PropertyPath("FormatString"), Source = upDownColumn };
            uiElement.SetBinding(SfNumericUpDown.FormatStringProperty, bind);
            bind = new Binding { Path = new PropertyPath("Culture"), Source = upDownColumn };
            uiElement.SetBinding(SfNumericUpDown.CultureProperty, bind);
            bind = new Binding { Path = new PropertyPath("MinValue"), Source = upDownColumn };
            uiElement.SetBinding(SfNumericUpDown.MinimumProperty, bind);
            bind = new Binding { Path = new PropertyPath("MaxValue"), Source = upDownColumn };
            uiElement.SetBinding(SfNumericUpDown.MaximumProperty, bind);
            bind = new Binding { Path = new PropertyPath("AutoReverse"), Source = upDownColumn };
            uiElement.SetBinding(SfNumericUpDown.AutoReverseProperty, bind);
            bind = new Binding { Path = new PropertyPath("ParsingMode"), Source = upDownColumn };
            uiElement.SetBinding(SfNumericUpDown.ParsingModeProperty, bind);
            bind = new Binding { Path = new PropertyPath("Step"), Mode = BindingMode.TwoWay, Source = upDownColumn };
            uiElement.SetBinding(SfNumericUpDown.SmallChangeProperty, bind);
            bind = new Binding { Path = new PropertyPath("BlockCharactersOnTextInput"), Mode = BindingMode.TwoWay, Source = upDownColumn };
            uiElement.SetBinding(SfNumericUpDown.BlockCharactersOnTextInputProperty, bind);
            bind = new Binding { Path = new PropertyPath("MaximumNumberDecimalDigits"), Mode = BindingMode.TwoWay, Source = upDownColumn };
            uiElement.SetBinding(SfNumericUpDown.MaximumNumberDecimalDigitsProperty, bind);
            bind = new Binding { Path = new PropertyPath("LargeChange"), Mode = BindingMode.TwoWay, Source = upDownColumn };
            uiElement.SetBinding(SfNumericUpDown.LargeChangeProperty, bind);
            bind = new Binding { Path = new PropertyPath("SmallChange"), Mode = BindingMode.TwoWay, Source = upDownColumn };
            uiElement.SetBinding(SfNumericUpDown.SmallChangeProperty, bind);
            bind = new Binding { Path = new PropertyPath("SpinButtonsAlignment"), Mode = BindingMode.TwoWay, Source = upDownColumn };
            uiElement.SetBinding(SfNumericUpDown.SpinButtonsAlignmentProperty, bind);
#else
            var bind = upDownColumn.ValueBinding.CreateEditBinding(upDownColumn.UpdateTrigger);
            uiElement.SetBinding(UpDown.ValueProperty, bind);
#if WPF
            bind = new Binding { Path = new PropertyPath("Culture"), Mode = BindingMode.TwoWay, Source = upDownColumn };
            uiElement.SetBinding(UpDown.CultureProperty, bind);
            bind = new Binding { Path = new PropertyPath("MinValue"), Mode = BindingMode.TwoWay, Source = upDownColumn };
            uiElement.SetBinding(UpDown.MinValueProperty, bind);
            bind = new Binding { Path = new PropertyPath("MaxValue"), Mode = BindingMode.TwoWay, Source = upDownColumn };
            uiElement.SetBinding(UpDown.MaxValueProperty, bind);
            bind = new Binding { Path = new PropertyPath("NumberFormat"), Mode = BindingMode.TwoWay, Source = upDownColumn };
            uiElement.SetBinding(UpDown.NumberFormatInfoProperty, bind);
            bind = new Binding { Path = new PropertyPath("Step"), Mode = BindingMode.TwoWay, Source = upDownColumn };
            uiElement.SetBinding(UpDown.StepProperty, bind);
            bind = new Binding { Path = new PropertyPath("NumberDecimalDigits"), Mode = BindingMode.TwoWay, Source = upDownColumn };
            uiElement.SetBinding(UpDown.NumberDecimalDigitsProperty, bind);
            bind = new Binding { Path = new PropertyPath("AllowNullValue"), Mode = BindingMode.TwoWay, Source = upDownColumn };
            uiElement.SetBinding(UpDown.UseNullOptionProperty, bind);
            bind = new Binding { Path = new PropertyPath("AllowScrollingOnCircle"), Mode = BindingMode.TwoWay, Source = upDownColumn };
            uiElement.SetBinding(EditorBase.IsScrollingOnCircleProperty, bind);
#endif
#endif
        }

        /// <summary>
        /// Processes the clear binding.
        /// </summary>
        /// <param name="element">The element.</param>
#if WinRT
        private void ProcessClearBinding(SfNumericUpDown element)
#else
        private void ProcessClearBinding(UpDown element)
#endif
        {
#if WinRT
            element.ClearValue(SfNumericUpDown.ValueProperty);
            element.ClearValue(SfNumericUpDown.CultureProperty);
            element.ClearValue(SfNumericUpDown.FormatStringProperty);
            element.ClearValue(SfNumericUpDown.MinimumProperty);
            element.ClearValue(SfNumericUpDown.CultureProperty);
            element.ClearValue(SfNumericUpDown.MaximumProperty);
            element.ClearValue(SfNumericUpDown.AutoReverseProperty);
            element.ClearValue(SfNumericUpDown.ParsingModeProperty);
#else
            element.ClearValue(UpDown.ValueProperty);
#if WPF
            element.ClearValue(UpDown.IsScrollingOnCircleProperty);
            element.ClearValue(UpDown.CultureProperty);
            element.ClearValue(UpDown.MinValueProperty);
            element.ClearValue(UpDown.MaxValueProperty);
            element.ClearValue(UpDown.NumberFormatInfoProperty);
            element.ClearValue(UpDown.StepProperty);
            element.ClearValue(UpDown.NumberDecimalDigitsProperty);
#endif
#endif
        }
        #endregion
    }
}
