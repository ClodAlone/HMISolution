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
using Syncfusion.UI.Xaml.ScrollAxis;
using Syncfusion.UI.Xaml.Utility;
#if WinRT
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

#else
using System.Windows.Controls;
using System.Windows.Input;
#endif

namespace Syncfusion.UI.Xaml.Grid.Cells
{
#if WinRT
    using Key = Windows.System.VirtualKey;
    using KeyEventArgs = KeyRoutedEventArgs;
    using Windows.UI.Xaml;

#endif
    public class GridUnBoundCellRenderer : GridVirtualizingCellRenderer<ContentControl,ContentControl>
    {
        #region Fields
#if !WP
        private bool CanEnterIntoEditMode;
#endif

        #endregion

        #region Override Methods

        #region Display/Edit Binding Overrides
        /// <summary>
        /// Called when [initialize display element].
        /// </summary>
        /// <param name="rowColumnIndex">Index of the row column.</param>
        /// <param name="uiElement">The unique identifier element.</param>
        /// <param name="column">The column.</param>
        /// <param name="dataContext">The data context.</param>
        public override void OnInitializeDisplayElement(RowColumnIndex rowColumnIndex, ContentControl uiElement, GridColumn column, object dataContext)
        {
            var col = (GridUnBoundColumn) column;            
            //if (dataContext != null)
            uiElement.Content = DataGrid.GetUnBoundCellValue(column, dataContext);
            
            uiElement.Margin = ProcessUIElementPadding(column);
            uiElement.HorizontalAlignment = TextAlignmentToHorizontalAlignment(col.TextAlignment);
            uiElement.VerticalContentAlignment = VerticalAlignment.Stretch;

#if !SILVERLIGHT && !WP
            if (col.CellTemplate != null || col.CellTemplateSelector != null || col.EditTemplate != null || col.EditTemplateSelector != null)
#elif !WP
            if (col.CellTemplate != null || col.EditTemplate != null)
#else
            if (col.CellTemplate != null)
#endif
            {
                DataTemplate template = null;
#if !SILVERLIGHT && !WP
                if (col.CellTemplateSelector != null)
                    template = col.CellTemplateSelector.SelectTemplate(dataContext, uiElement);
#endif
                if (template == null && col.CellTemplate != null)
                    template = col.CellTemplate;
                if (template != null)
                    uiElement.ContentTemplate = template;
                uiElement.HorizontalContentAlignment = HorizontalAlignment.Stretch;
                uiElement.VerticalContentAlignment = VerticalAlignment.Stretch;
            }
        }

        /// <summary>
        /// Method which initialize the element with respect corresponding UnBound value
        /// </summary>
        /// <param name="uiElement"></param>
        /// <param name="column"></param>
        /// <param name="dataContext"></param>
        /// <remarks></remarks>
        /// 
        public override void OnInitializeEditElement(RowColumnIndex rowColumnIndex, ContentControl uiElement, GridColumn column, object dataContext)
        {
            var col = (GridUnBoundColumn) column;
            object contentValue = null;
            if (dataContext != null)
                contentValue = DataGrid.GetUnBoundCellValue(column, dataContext);

#if !SILVERLIGHT && !WP
            if (col.EditTemplate != null || col.EditTemplateSelector != null || col.CellTemplate != null || col.CellTemplateSelector != null)
#elif !WP
            if (col.EditTemplate != null || col.CellTemplate != null)
#else
            if(col.CellTemplate != null)
#endif
            {
                DataTemplate template = null;

#if !SILVERLIGHT && !WP
                if (col.EditTemplateSelector != null)
                    template = col.EditTemplateSelector.SelectTemplate(dataContext, uiElement);
#endif
#if !WP
                CanEnterIntoEditMode = true;
                if (template == null && col.EditTemplate != null)
                    template = col.EditTemplate;

                if (template == null && HasCurrentCellState)
                    CanEnterIntoEditMode = false;
#endif

#if !SILVERLIGHT && !WP
                if (template == null && col.CellTemplateSelector != null)
                    template = col.CellTemplateSelector.SelectTemplate(dataContext, uiElement);
#endif
                if (template == null && col.CellTemplate != null)
                    template = col.CellTemplate;
                if (template != null)
                {
                    uiElement.Content = contentValue;
                    uiElement.ContentTemplate = template;
                }

                uiElement.HorizontalContentAlignment = HorizontalAlignment.Stretch;
                uiElement.VerticalContentAlignment = VerticalAlignment.Stretch;
                return;
            }

#if !WP
            CanEnterIntoEditMode = true;
#endif
            var textbox = new TextBox
            {
                Padding = col.Padding,
                Text = contentValue == null ? null : contentValue.ToString(),
                TextAlignment = column.TextAlignment
            };
            uiElement.Content = textbox;
            uiElement.HorizontalContentAlignment = HorizontalAlignment.Stretch;
            uiElement.VerticalContentAlignment = VerticalAlignment.Stretch;
        }

#if !WP
        protected override void OnEditingComplete(UIElement currentRendererElement)
        {
            if (HasCurrentCellState && IsInEditing)
            {
                var txtbox = (CurrentCellRendererElement as ContentControl).Content as TextBox;
                if (txtbox != null)
                {
                    var record = txtbox.DataContext;
                    if (record != null)
                        DataGrid.RaiseQueryUnboundValue(UnBoundActions.CommitData, txtbox.Text, DataGrid.Columns[DataGrid.ResolveToGridVisibleColumnIndex(CurrentCellIndex.ColumnIndex)], record);
                }
            }
            base.OnEditingComplete(currentRendererElement);
        }
#endif
        #endregion

        #region Display/Edit Value Overrides
        /// <summary>
        /// Gets the control value.
        /// </summary>
        /// <returns></returns>
        public override object GetControlValue()
        {
            if (HasCurrentCellState)
            {
                var contentControl = CurrentCellRendererElement as ContentControl;
                if (IsInEditing)
                {
                    if (contentControl != null && contentControl.Content is TextBox)
                        return (contentControl.Content as TextBox).GetValue(TextBox.TextProperty);
                }
                else
                {
                    if (contentControl != null && !(contentControl.Content is UIElement))
                        return CurrentCellRendererElement.GetValue(ContentControl.ContentProperty);
                }
            }
            return base.GetControlValue();
        }

        /// <summary>
        /// Sets the control value.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void SetControlValue(object value)
        {
            if (HasCurrentCellState)
            {
                var contentControl = CurrentCellRendererElement as ContentControl;
                if (IsInEditing)
                {
                    if (contentControl != null && contentControl.Content is TextBox)
                        (contentControl.Content as TextBox).Text = value == null ? string.Empty : value.ToString();
                }
                else
                {
                    if (contentControl != null && !(contentControl.Content is UIElement))
                        contentControl.Content = value;
                }
            }
        }
        #endregion

        #region PreviewTextInput Override
        /// <summary>
        /// Raises the <see cref="E:PreviewTextInput" /> event.
        /// </summary>
        /// <param name="e">The <see cref="KeyEventArgs"/> instance containing the event data.</param>
#if !WinRT
        protected override void OnPreviewTextInput(TextCompositionEventArgs e)
#else
        protected override void OnPreviewTextInput(KeyEventArgs e)
#endif
        {
            if (!HasCurrentCellState)
                return;
#if !WinRT && !WP
            PreviewInputText = e.Text;
#elif !WP
            if (e.Key >= Key.Number0 && e.Key <= Key.Number9)
                PreviewInputText = (e.Key - Key.Number0).ToString();
            else if (e.Key >= Key.NumberPad0 && e.Key <= Key.NumberPad9)
                PreviewInputText = (e.Key - Key.NumberPad0).ToString();
            else if (e.Key >= Key.A && e.Key <= Key.Z)
                PreviewInputText = e.Key.ToString();
#endif
        }
        #endregion

        #region Update/Ensure Editing Overrides

        protected override void SetFocus(UIElement element, bool needToFocus)
        {
            if (!HasCurrentCellState)
                return;
#if !WPF
            var CurrentCellUIElement = (CurrentCellRendererElement as ContentControl);
            if (CurrentCellUIElement != null && CurrentCellUIElement.ContentTemplate != null)
            {
                if (needToFocus)
                {
                    var focusedElement = FocusManagerHelper.GetFocusedUIElement(CurrentCellRendererElement);
                    var uiElement = focusedElement ?? element;
                    base.SetFocus(uiElement, true);
                }
            }
            else if (IsInEditing)
#endif
            {
                if (CurrentCellRendererElement is ContentControl && (CurrentCellRendererElement as ContentControl).Content is TextBox)
                    element = (CurrentCellRendererElement as ContentControl).Content as TextBox;
            }
            base.SetFocus(element, needToFocus);
        }

#if !WP
        protected override void OnEnteredEditMode(UIElement currentRendererElement)
        {
            UpdateCurrentCellState(currentRendererElement, CanEnterIntoEditMode);
        }

        protected override void OnEditElementLoaded(object sender, RoutedEventArgs e)
        {
            if (!((sender as ContentControl).Content is TextBox)) return;
            var uiElement = (TextBox) (sender as ContentControl).Content;
#if WinRT
            uiElement.Focus(FocusState.Programmatic);
#else
            uiElement.Focus();
#endif
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
                uiElement.Text = PreviewInputText;
                var caretIndex = (uiElement.Text).IndexOf(PreviewInputText.ToString(CultureInfo.InvariantCulture), StringComparison.Ordinal);
                uiElement.Select(caretIndex + 1, 0);
            }
            PreviewInputText = null;
        }
#endif
        #endregion

        #region ShouldGridTryToHandleKeyDown
        /// <summary>
        /// Shoulds the grid try automatic handle key down.
        /// </summary>
        /// <param name="e">The <see cref="KeyEventArgs"/> instance containing the event data.</param>
        /// <returns></returns>
        protected override bool ShouldGridTryToHandleKeyDown(KeyEventArgs e)
        {
            if (!HasCurrentCellState)
                return true;
#if !WP
            var CurrentCellUIElement = (CurrentCellRendererElement as ContentControl);
            if (CurrentCellUIElement != null && CurrentCellUIElement.ContentTemplate != null)
            {
                var columnIndex = DataGrid.ResolveToGridVisibleColumnIndex(CurrentCellIndex.ColumnIndex);
                var column = ((GridTemplateColumn)DataGrid.Columns[columnIndex]);
                var handleTemplatedUIElementKeyDown = FocusManagerHelper.GetWantsKeyInput(column);
                switch (e.Key)
                {
                    case Key.F2:
                    case Key.Escape:
                    {
                        e.Handled = true;
                        return true;
                    }
                    case Key.Tab:
                    case Key.Enter:
                    case Key.PageUp:
                    case Key.PageDown:
                        if (!IsInEditing)
                            base.SetFocus(CurrentCellRendererElement, true);
                        return true;
                    case Key.Home:
                    case Key.End:
                    case Key.Down:
                    case Key.Up:
                    case Key.Left:
                    case Key.Right:
                    {
                        if (!handleTemplatedUIElementKeyDown) return !IsInEditing;
                        var focusedElement = GridUtil.FindDescendant(CurrentCellRendererElement, typeof(GridCell));
                        return focusedElement != null;
                    }
                }
                return base.ShouldGridTryToHandleKeyDown(e);
            }
#endif
#if !WP
            var uiElement = CurrentCellUIElement.Content as TextBox;
            if (uiElement == null || !IsInEditing)
                return true;
            switch (e.Key)
            {
                case Key.Left:
                    return (uiElement.SelectionStart <= 0 && !CheckControlKeyPressed() && !CheckShiftKeyPressed());
                case Key.Right:
                    return (uiElement.SelectionStart >= uiElement.Text.Length && !CheckControlKeyPressed() && !CheckShiftKeyPressed());
                case Key.Home:
                    return (uiElement.SelectionStart == 0 && !CheckControlKeyPressed() && !CheckShiftKeyPressed());
                case Key.End:
                    return (uiElement.SelectionStart == uiElement.Text.Length && !CheckControlKeyPressed() && !CheckShiftKeyPressed());
            }
#endif
            return base.ShouldGridTryToHandleKeyDown(e);
        }
        #endregion

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
#if WinRT
            return padding != DependencyProperty.UnsetValue
                           ? new Thickness(3 + padLeft, 3 + padTop, 5 + padRight, 5 + padBotton)
                           : new Thickness(3, 1, 6, 6);
#elif SILVERLIGHT
            return padding != DependencyProperty.UnsetValue
                           ? new Thickness(4 + padLeft, 3 + padTop, 5 + padRight, 5 + padBotton)
                           : new Thickness(2, 2, 6, 6);
#else
            return padding != DependencyProperty.UnsetValue
                           ? new Thickness(3 + padLeft, 1 + padTop, 3 + padRight, 1 + padBotton)
                           : new Thickness(3, 1, 3, 1);
#endif
        }
        #endregion
    }
}
