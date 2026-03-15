#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Globalization;
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
#endif

namespace Syncfusion.UI.Xaml.Grid.Cells
{
    [ClassReference(IsReviewed = false)]
    public class GridCellPercentageRenderer : GridVirtualizingCellRenderer<TextBlock, PercentTextBox>
    {
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
        public override void OnInitializeEditElement(RowColumnIndex rowColumnIndex, PercentTextBox uiElement, GridColumn column, object dataContext)
        {
            base.OnInitializeEditElement(rowColumnIndex, uiElement, column, dataContext);
            InitializeEditUIElement(uiElement, column);
            BindingExpression = uiElement.GetBindingExpression(PercentTextBox.PercentValueProperty);
        }
        #endregion

        #region 

        /// <summary>
        /// Gets the control value.
        /// </summary>
        /// <returns></returns>
        public override object GetControlValue()
        {
            if (!HasCurrentCellState)
                return base.GetControlValue();
            return CurrentCellRendererElement.GetValue(IsInEditing ? PercentTextBox.PercentValueProperty : TextBlock.TextProperty);
        }

        /// <summary>
        /// Sets the control value.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void SetControlValue(object value)
        {
            if (!HasCurrentCellState) 
                return;

            if (IsInEditing)
                ((PercentTextBox) CurrentCellRendererElement).PercentValue = (double?) value;
            else
                throw new Exception("Value cannot be Set for Unloaded Editor");
        }
        #endregion

        #region Wire/UnWire UIElements Overrides
        /// <summary>
        /// Called when [edit element loaded].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        protected override void OnEditElementLoaded(object sender, RoutedEventArgs e)
        {
            var uiElement = ((PercentTextBox) sender);
            uiElement.PercentValueChanged += OnValueChanged;
            uiElement.Focus();
            if ((this.DataGrid.EditorSelectionBehavior == EditorSelectionBehavior.SelectAll || this.DataGrid.IsAddNewIndex(this.CurrentCellIndex.RowIndex)) && PreviewInputText == null)
            {
                uiElement.SelectAll();
            }
            else
            {
                if (PreviewInputText == null)
                {
                    var index = uiElement.Text.Length;
                    uiElement.Select(index + 1, 0);
                    return;
                }
                double value;
                double.TryParse(PreviewInputText.ToString(), out value);
                uiElement.PercentValue = value;
                var caretIndex = (uiElement.Text).IndexOf(PreviewInputText.ToString(CultureInfo.InvariantCulture), StringComparison.Ordinal);
                uiElement.Select(caretIndex + 1, 0);
            }
            PreviewInputText = null;
        }

#if SILVERLIGHT
        /// <summary>
        /// Called when [edit element lost focus].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        protected override void OnEditElementLostFocus(object sender, RoutedEventArgs e)
        {
            BindingExpression.UpdateSource();
            base.OnEditElementLostFocus(sender, e);
        }
#endif

        /// <summary>
        /// Called when [unwire edit UI element].
        /// </summary>
        /// <param name="uiElement">The UI element.</param>
        protected override void OnUnwireEditUIElement(PercentTextBox uiElement)
        {
            uiElement.PercentValueChanged -= OnValueChanged;
        }
        #endregion

        #region ShouldGridTryToHandleKeyDown
        /// <summary>
        /// Shoulds the grid try automatic handle key down.
        /// </summary>
        /// <param name="e">The <see cref="KeyEventArgs"/> instance containing the event data.</param>
        /// <returns></returns>
        protected override bool ShouldGridTryToHandleKeyDown(KeyEventArgs e)
        {
            if (!HasCurrentCellState || !IsInEditing)
                return true;

            var CurrentCellUIElement = (PercentTextBox) CurrentCellRendererElement;
            switch (e.Key)
            {
                case Key.Escape:
                {
                    if (CurrentCellUIElement != null)
                        CurrentCellUIElement.ClearValue(PercentTextBox.PercentValueProperty);
                    return true;
                }
                case Key.Left:
                    return (CurrentCellUIElement.SelectionStart <= 0 && !CheckControlKeyPressed() && !CheckShiftKeyPressed());
                case Key.Right:
                    return (CurrentCellUIElement.SelectionStart >= CurrentCellUIElement.Text.Length && !CheckControlKeyPressed() && !CheckShiftKeyPressed());
                case Key.Home:
                    return (CurrentCellUIElement.SelectionStart == 0 && !CheckControlKeyPressed() && !CheckShiftKeyPressed());
                case Key.End:
                    return (CurrentCellUIElement.SelectionStart == CurrentCellUIElement.Text.Length && !CheckControlKeyPressed() && !CheckShiftKeyPressed());
            }
            return base.ShouldGridTryToHandleKeyDown(e);
        }
        #endregion

        #endregion

        #region Event Handlers

        /// <summary>
        /// Called when [text changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="TextChangedEventArgs"/> instance containing the event data.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        private void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            base.CurrentRendererValueChanged();
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Computes the Padding for the Display UIElement.
        /// </summary>
        /// <param name="column">The column.</param>
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
                           ? new Thickness(3 + padLeft, 6 + padTop, 3 + padRight, 6 + padBotton)
                           : new Thickness(3, 6, 3, 6);
#else
            return padding != DependencyProperty.UnsetValue
                           ? new Thickness(2 + padLeft, 2 + padTop, 3 + padRight, 6 + padBotton)
                           : new Thickness(2, 2, 3, 2);
#endif
        }


        /// <summary>
        /// Processes the edit binding.
        /// </summary>
        /// <param name="uiElement">The unique identifier element.</param>
        /// <param name="column">Grid Column for the Editor</param>
        private void InitializeEditUIElement(PercentTextBox uiElement, GridColumn column)
        {
            uiElement.TextSelectionOnFocus = false;
            var percentColumn = (GridPercentColumn) column;
            var bind = percentColumn.ValueBinding.CreateEditBinding(percentColumn.UpdateTrigger);
            uiElement.SetBinding(PercentTextBox.PercentValueProperty, bind);
            bind = new Binding { Path = new PropertyPath("AllowScrollingOnCircle"), Mode = BindingMode.TwoWay, Source = percentColumn };
            uiElement.SetBinding(EditorBase.IsScrollingOnCircleProperty, bind);
            bind = new Binding { Path = new PropertyPath("MinValue"), Mode = BindingMode.TwoWay, Source = percentColumn };
            uiElement.SetBinding(PercentTextBox.MinValueProperty, bind);
            bind = new Binding { Path = new PropertyPath("MaxValue"), Mode = BindingMode.TwoWay, Source = percentColumn };
            uiElement.SetBinding(PercentTextBox.MaxValueProperty, bind);
            bind = new Binding { Path = new PropertyPath("PercentEditMode"), Mode = BindingMode.TwoWay, Source = percentColumn };
            uiElement.SetBinding(PercentTextBox.PercentEditModeProperty, bind);
            bind = new Binding { Path = new PropertyPath("PercentDecimalDigits"), Mode = BindingMode.TwoWay, Source = percentColumn };
            uiElement.SetBinding(PercentTextBox.PercentDecimalDigitsProperty, bind);
            bind = new Binding { Path = new PropertyPath("PercentDecimalSeparator"), Mode = BindingMode.TwoWay, Source = percentColumn };
            uiElement.SetBinding(PercentTextBox.PercentDecimalSeparatorProperty, bind);
            bind = new Binding { Path = new PropertyPath("PercentGroupSeparator"), Mode = BindingMode.TwoWay, Source = percentColumn };
            uiElement.SetBinding(PercentTextBox.PercentGroupSeparatorProperty, bind);
            bind = new Binding { Path = new PropertyPath("PercentGroupSizes"), Mode = BindingMode.TwoWay, Source = percentColumn };
            uiElement.SetBinding(PercentTextBox.PercentGroupSizesProperty, bind);
            bind = new Binding { Path = new PropertyPath("PercentNegativePattern"), Mode = BindingMode.TwoWay, Source = percentColumn };
            uiElement.SetBinding(PercentTextBox.PercentNegativePatternProperty, bind);
            bind = new Binding { Path = new PropertyPath("PercentPositivePattern"), Mode = BindingMode.TwoWay, Source = percentColumn };
            uiElement.SetBinding(PercentTextBox.PercentPositivePatternProperty, bind);
            bind = new Binding { Path = new PropertyPath("PercentSymbol"), Mode = BindingMode.TwoWay, Source = percentColumn };
            uiElement.SetBinding(PercentTextBox.PercentageSymbolProperty, bind);
            bind = new Binding { Path = new PropertyPath("AllowNullValue"), Mode = BindingMode.TwoWay, Source = percentColumn };
            uiElement.SetBinding(EditorBase.UseNullOptionProperty, bind);
            bind=new Binding{Path=new PropertyPath("MaxValidation"), Mode=BindingMode.TwoWay, Source=percentColumn};
            uiElement.SetBinding(EditorBase.MaxValidationProperty, bind);
            bind = new Binding { Path = new PropertyPath("MinValidation"), Mode = BindingMode.TwoWay, Source = percentColumn };
            uiElement.SetBinding(EditorBase.MinValidationProperty, bind);
            bind = new Binding { Path = new PropertyPath("NullValue"), Mode = BindingMode.TwoWay, Source = percentColumn };
            uiElement.SetBinding(PercentTextBox.NullValueProperty, bind);
            uiElement.WatermarkTextIsVisible = true;
            bind = new Binding { Path = new PropertyPath("NullText"), Mode = BindingMode.TwoWay, Source = percentColumn };
            uiElement.SetBinding(PercentTextBox.WatermarkTextProperty, bind);
#if WPF
            bind = new Binding { Path = new PropertyPath("TextDecorations"), Mode = BindingMode.TwoWay, Source = percentColumn };
            uiElement.SetBinding(PercentTextBox.TextDecorationsProperty, bind);
#endif
        }
        #endregion
    }
}
