#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using Syncfusion.UI.Xaml.ScrollAxis;
#if WinRT
using Syncfusion.UI.Xaml.Controls.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
#else
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Syncfusion.UI.Xaml.Grid.Utility;
#if WPF
using Syncfusion.Windows.Shared;
#elif SILVERLIGHT
using Syncfusion.Windows.Tools.Controls;
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
    public class GridCellDateTimeRenderer : GridVirtualizingCellRenderer<TextBlock, SfDatePicker>
#else
    public class GridCellDateTimeRenderer:GridVirtualizingCellRenderer<TextBlock, DateTimeEdit>
#endif
    {
        #region Ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="GridCellDateTimeRenderer"/> class.
        /// </summary>
        public GridCellDateTimeRenderer()
        {
#if WinRT
            IsFocusible = false;
#endif
        }
        #endregion 

        #region Override Methods

        #region Display/Edit Binding Overrides
        /// <summary>
        /// Called when [initialize display element].
        /// </summary>
        /// <param name="rowColumnIndex">Index of the row column.</param>
        /// <param name="uiElement">The UI element.</param>
        /// <param name="column">The column.</param>
        /// <param name="dataContext">The data context.</param>
        public override void OnInitializeDisplayElement(RowColumnIndex rowColumnIndex, TextBlock uiElement, GridColumn column, object dataContext)
        {
            base.OnInitializeDisplayElement(rowColumnIndex, uiElement, column, dataContext);
            //Note: Padding cannot be binded for Display UI Element, since the padding will be irregular to match Edit/Display UI Elements
            //The Column will be refreshed via Dependency CallBack.
            uiElement.Padding = ProcessUIElementPadding(column);
        }

        /// <summary>
        /// Called when [initialize edit element].
        /// </summary>
        /// <param name="rowColumnIndex">Index of the row column.</param>
        /// <param name="uiElement">The UI element.</param>
        /// <param name="column">The column.</param>
        /// <param name="dataContext">The data context.</param>
#if WinRT
        public override void OnInitializeEditElement(RowColumnIndex rowColumnIndex, SfDatePicker uiElement, GridColumn column, object dataContext)
#else
        public override void OnInitializeEditElement(RowColumnIndex rowColumnIndex, DateTimeEdit uiElement, GridColumn column, object dataContext)
#endif
        {
            InitializeEditUIElement(uiElement, column);
#if !WinRT
            BindingExpression = uiElement.GetBindingExpression(DateTimeEdit.DateTimeProperty);
#endif
#if !WinRT
            base.OnInitializeEditElement(rowColumnIndex, uiElement, column, dataContext);
#else
            var textPadding = new Binding { Path = new PropertyPath("Padding"), Mode = BindingMode.TwoWay, Source = column };
            uiElement.SetBinding(Control.PaddingProperty, textPadding);
            uiElement.VerticalAlignment = VerticalAlignment.Stretch;
            uiElement.HorizontalAlignment = HorizontalAlignment.Stretch;
#endif
            uiElement.HorizontalContentAlignment = TextAlignmentToHorizontalAlignment(column.TextAlignment);
        }
        #endregion

        #region Display/Edit Value Overrides
        /// <summary>
        /// Gets the control value.
        /// </summary>
        /// <returns></returns>
        public override object GetControlValue()
        {
            if (!HasCurrentCellState) 
                return base.GetControlValue();
#if WinRT
            return this.CurrentCellRendererElement.GetValue(IsInEditing ? SfDatePicker.ValueProperty : TextBlock.TextProperty);
#else
            return CurrentCellRendererElement.GetValue(IsInEditing ? DateTimeEdit.DateTimeProperty : TextBlock.TextProperty);
#endif
        }

#if !WinRT
        /// <summary>
        /// Sets the control value.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void SetControlValue(object value)
        {
            if (!HasCurrentCellState) return;
            if (IsInEditing)
                ((DateTimeEdit) CurrentCellRendererElement).DateTime = (DateTime?) value;
            else
                throw new Exception("Value cannot be Set for Unloaded Editor");
        }
#endif
        #endregion

        #region Wire/UnWire UIElement

        /// <summary>
        /// Called when [unwire edit unique identifier element].
        /// </summary>
        /// <param name="uiElement">The unique identifier element.</param>
#if WinRT
        protected override void OnEditElementLoaded(object sender, RoutedEventArgs e)
        {
            ((SfDatePicker)sender).ValueChanged += OnValueChanged;
        }
        protected override void OnUnwireEditUIElement(SfDatePicker uiElement)
        {
            uiElement.ValueChanged -= OnValueChanged;
        }
#else
        protected override void OnEditElementLoaded(object sender, RoutedEventArgs e)
        {
            ((DateTimeEdit)sender).TextChanged += OnTextChanged;
            ((DateTimeEdit) sender).Focus();
            if (PreviewInputText == null) return;
            ((DateTimeEdit) CurrentCellRendererElement).SelectedText = PreviewInputText.ToString();
            PreviewInputText = null;
        }
        protected override void OnUnwireEditUIElement(DateTimeEdit uiElement)
        {
            uiElement.TextChanged -= OnTextChanged;
        }
#endif

#if SILVERLIGHT
        protected override void OnEditElementLostFocus(object sender, RoutedEventArgs e)
        {
            if (BindingExpression != null)
                BindingExpression.UpdateSource();
            base.OnEditElementLostFocus(sender, e);
        }
#endif

        #endregion

        #region ShouldHandleKeyDown
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
#if WinRT
            if (!HasCurrentCellState)
                return true;
            if (!IsInEditing)
            {
                ProcessPreviewTextInput(e);
                if (!(CurrentCellRendererElement is SfNumericTextBox))
                    return true;
            }
#else
            if (!HasCurrentCellState || !IsInEditing)
                return true;
#endif
#if WinRT
            var CurrentCellUIElement = (SfDatePicker)CurrentCellRendererElement;
#else
            var CurrentCellUIElement = (DateTimeEdit)CurrentCellRendererElement;
#endif
            switch (e.Key)
            {
#if !WinRT
                case Key.F4:
                {
                    if (CurrentCellUIElement.IsDropDownOpen)
                        CurrentCellUIElement.ClosePopup();
                    else
                        CurrentCellUIElement.OpenPopup();
                    return true;
                }
#endif
                case Key.Escape:
                {
#if !WinRT
                    if (CurrentCellUIElement != null)
                        CurrentCellUIElement.ClearValue(DateTimeEdit.DateTimeProperty);
#endif
                    //TODO: Asked Tools Team Checking DropDown is Open or Not
                    //If DropDown is Open, we need to close the DropDown and should not End Edit (return false)
                    //else return true to EndEdit
                    //Currently we are returning true to end edit the DateTime Cell to stop the Exception that arises at UpdateSource of BindingExpression
                    return true;
                }
                case Key.Up:
                case Key.Down:
                    return !IsInEditing;
#if WPF
                case Key.Left:
                    return (CurrentCellUIElement.CaretIndex <= 0 && !CheckControlKeyPressed() && !CheckShiftKeyPressed());
                case Key.Right:
                    return (CurrentCellUIElement.CaretIndex >= CurrentCellUIElement.Text.Length && !CheckControlKeyPressed() && !CheckShiftKeyPressed());
                case Key.Home:
                    return (CurrentCellUIElement.CaretIndex == 0 && !CheckControlKeyPressed() && !CheckShiftKeyPressed());
                case Key.End:
                    return (CurrentCellUIElement.CaretIndex == CurrentCellUIElement.Text.Length && !CheckControlKeyPressed() && !CheckShiftKeyPressed());
#elif WinRT
                case Key.Left:
                case Key.Right:
                    return true;
#endif
            }
            return base.ShouldGridTryToHandleKeyDown(e);
        }

#if WinRT
        private void ProcessPreviewTextInput(KeyEventArgs e)
        {
            if ((!char.IsLetterOrDigit(e.Key.ToString(), 0) || !DataGrid.AllowEditing || DataGrid.NavigationMode != NavigationMode.Cell) || CheckControlKeyPressed() || (!(e.Key >= Key.A && e.Key <= Key.Z) && !(e.Key >= Key.Number0 && e.Key <= Key.Number9) && !(e.Key >= Key.NumberPad0 && e.Key <= Key.NumberPad9)))
                return;
            if (DataGrid.SelectionController.CurrentCellManager.BeginEdit())
                PreviewTextInput(e);
        }
#endif
        #endregion

        #endregion

        #region Event Handlers
        /// <summary>
        /// Called when [text changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="TextChangedEventArgs"/> instance containing the event data.</param>
#if WinRT
        private void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
#else
        private void OnTextChanged(object sender, TextChangedEventArgs e)
#endif
        {
            base.CurrentRendererValueChanged();
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// Computes the Padding for the Display UIElement.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <remarks>Padding Cannot be applied at RunTime, as Currently We dont haven't constructed Padding via Binding Expression</remarks>
        /// <returns></returns>
        private static Thickness ProcessUIElementPadding(GridColumn column)
        {
            var padLeft = column.Padding.Left;
            var padRight = column.Padding.Right;
            var padTop = column.Padding.Top;
            var padBotton = column.Padding.Bottom;
            var padding = column.ReadLocalValue(GridColumn.PaddingProperty);
#if WPF
            return padding != DependencyProperty.UnsetValue
                           ? new Thickness(3 + padLeft, 6 + padTop, 3 + padRight, 5 + padBotton)
                           : new Thickness(3, 7, 3, 6);
#elif SILVERLIGHT
            return padding != DependencyProperty.UnsetValue
                           ? new Thickness(2 + padLeft, 2 + padTop, 3 + padRight, 3 + padBotton)
                           : new Thickness(3, 1, 3, 3);
#else
            return padding != DependencyProperty.UnsetValue
                           ? new Thickness(3 + padLeft, 1 + padTop, 3 + padRight, 6 + padBotton)
                           : new Thickness(2, 5, 2, 6);
#endif
        }

        /// <summary>
        /// Processes the edit binding.
        /// </summary>
        /// <param name="uiElement">The UI element.</param>
        /// <param name="column"></param>
#if WinRT
        private void InitializeEditUIElement(SfDatePicker uiElement, GridColumn column)
#else
        private void InitializeEditUIElement(DateTimeEdit uiElement, GridColumn column)
#endif
        {
            var dateTimeColumn = (GridDateTimeColumn)column;
#if WinRT
            uiElement.SetBinding(SfDatePicker.ValueProperty, dateTimeColumn.ValueBinding);
            var bind = new Binding { Path = new PropertyPath("FormatString"), Source = dateTimeColumn };
            uiElement.SetBinding(SfDatePicker.FormatStringProperty, bind);
            bind = new Binding { Path = new PropertyPath("AllowInlineEditing"), Source = dateTimeColumn };
            uiElement.SetBinding(SfDatePicker.AllowInlineEditingProperty, bind);
            bind = new Binding { Path = new PropertyPath("ShowDropDownButton"), Source = dateTimeColumn };
            uiElement.SetBinding(SfDatePicker.ShowDropDownButtonProperty, bind);            
#else
            var bind = dateTimeColumn.ValueBinding.CreateEditBinding(dateTimeColumn.UpdateTrigger);
            uiElement.SetBinding(DateTimeEdit.DateTimeProperty, bind);
            bind = new Binding { Path = new PropertyPath("MinDateTime"), Mode = BindingMode.TwoWay, Source = dateTimeColumn };
            uiElement.SetBinding(DateTimeEdit.MinDateTimeProperty, bind);
            bind = new Binding { Path = new PropertyPath("MaxDateTime"), Mode = BindingMode.TwoWay, Source = dateTimeColumn };
            uiElement.SetBinding(DateTimeEdit.MaxDateTimeProperty, bind);
            bind = new Binding { Path = new PropertyPath("CustomPattern"), Mode = BindingMode.TwoWay, Source = dateTimeColumn };
            uiElement.SetBinding(DateTimeBase.CustomPatternProperty, bind);
            bind = new Binding { Path = new PropertyPath("CanEdit"), Mode = BindingMode.TwoWay, Source = dateTimeColumn };
            uiElement.SetBinding(DateTimeBase.CanEditProperty, bind);
            bind = new Binding { Path = new PropertyPath("EnableBackspaceKey"), Mode = BindingMode.TwoWay, Source = dateTimeColumn };
            uiElement.SetBinding(DateTimeEdit.EnableBackspaceKeyProperty, bind);
            bind = new Binding { Path = new PropertyPath("EnableDeleteKey"), Mode = BindingMode.TwoWay, Source = dateTimeColumn };
            uiElement.SetBinding(DateTimeEdit.EnableDeleteKeyProperty, bind);
            bind = new Binding { Path = new PropertyPath("AllowScrollingOnCircle"), Mode = BindingMode.TwoWay, Source = dateTimeColumn };
            uiElement.SetBinding(DateTimeBase.IsScrollingOnCircleProperty, bind);
            bind = new Binding { Path = new PropertyPath("DateTimeFormat"), Mode = BindingMode.TwoWay, Source = dateTimeColumn };
            uiElement.SetBinding(DateTimeBase.DateTimeFormatProperty, bind);
            bind = new Binding { Path = new PropertyPath("Pattern"), Mode = BindingMode.TwoWay, Source = dateTimeColumn };
            uiElement.SetBinding(DateTimeBase.PatternProperty, bind);
            bind = new Binding { Path = new PropertyPath("EnableClassicStyle"), Mode = BindingMode.TwoWay, Source = dateTimeColumn };
            uiElement.SetBinding(DateTimeEdit.EnableClassicStyleProperty, bind);
            bind = new Binding { Path = new PropertyPath("AllowNullValue"), Mode = BindingMode.TwoWay, Source = dateTimeColumn };
            uiElement.SetBinding(DateTimeEdit.IsEmptyDateEnabledProperty, bind);
            bind = new Binding { Path = new PropertyPath("ShowRepeatButton"), Mode = BindingMode.TwoWay, Source = dateTimeColumn };
            uiElement.SetBinding(DateTimeBase.IsVisibleRepeatButtonProperty, bind);
            bind = new Binding { Path = new PropertyPath("NullValue"), Mode = BindingMode.TwoWay, Source = dateTimeColumn };
            uiElement.SetBinding(DateTimeEdit.NullValueProperty, bind);
            bind = new Binding { Path = new PropertyPath("NullText"), Mode = BindingMode.TwoWay, Source = dateTimeColumn };
            uiElement.SetBinding(DateTimeEdit.NoneDateTextProperty, bind);  
#endif
#if WPF
            bind = new Binding { Path = new PropertyPath("DisableDateSelection"), Mode = BindingMode.TwoWay, Source = dateTimeColumn };
            uiElement.SetBinding(DateTimeEdit.DisableDateSelectionProperty, bind);
            bind = new Binding { Path = new PropertyPath("TextDecorations"), Mode = BindingMode.TwoWay, Source = dateTimeColumn };
            uiElement.SetBinding(TimeSpanEdit.TextDecorationsProperty, bind);
#endif
#if WPF     
            if ((column as GridDateTimeColumn).MaxDateTime != System.DateTime.MaxValue)
            {
                bind = new Binding { Path = new PropertyPath("MaxDateTime"), Mode = BindingMode.TwoWay, Source = dateTimeColumn };
                uiElement.SetBinding(DateTimeEdit.NullValueProperty, bind);
            }
            else
            {
                bind = new Binding { Path = new PropertyPath("NullValue"), Mode = BindingMode.TwoWay, Source = dateTimeColumn };
                uiElement.SetBinding(DateTimeEdit.NullValueProperty, bind);
            }
                bind = new Binding { Path = new PropertyPath("NullText"), Mode = BindingMode.TwoWay, Source = dateTimeColumn };
                uiElement.SetBinding(DateTimeEdit.NoneDateTextProperty, bind);            
#endif
        }
        #endregion
    }
}
