#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Collections.Generic;
using Syncfusion.UI.Xaml.Utility;
#if WinRT
using Syncfusion.UI.Xaml.ScrollAxis;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
#else
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Syncfusion.UI.Xaml.ScrollAxis;
using System.Windows.Data;
using System.Windows.Media;
#endif

namespace Syncfusion.UI.Xaml.Grid.Cells
{
#if WinRT
    using Key = Windows.System.VirtualKey;
    using KeyEventArgs = KeyRoutedEventArgs;
#endif
    [ClassReference(IsReviewed = false)]
    public class GridCellTemplateRenderer : GridVirtualizingCellRenderer<ContentControl, ContentControl>
    {
        #region Private Fields
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
            InitializeDisplayTemplate(uiElement, (GridTemplateColumn)column, dataContext);
            var Bind = new Binding { Path = new PropertyPath("Padding"), Mode = BindingMode.TwoWay, Source = column };
            uiElement.SetBinding(Control.PaddingProperty, Bind);

            Bind = new Binding { Path = new PropertyPath("HorizontalAlignment"), Mode = BindingMode.TwoWay, Source = column };
            uiElement.SetBinding(FrameworkElement.HorizontalAlignmentProperty, Bind);
            uiElement.HorizontalContentAlignment = HorizontalAlignment.Stretch;
#if !WP
            Bind = new Binding { Path = new PropertyPath("VerticalAlignment"), Mode = BindingMode.TwoWay, Source = column };
            uiElement.SetBinding(FrameworkElement.VerticalAlignmentProperty, Bind);
            uiElement.VerticalContentAlignment = VerticalAlignment.Stretch;
#endif
        }

        /// <summary>
        /// Called when [initialize edit element].
        /// </summary>
        /// <param name="rowColumnIndex">Index of the row column.</param>
        /// <param name="uiElement">The unique identifier element.</param>
        /// <param name="column">The column.</param>
        /// <param name="dataContext">The data context.</param>
        public override void OnInitializeEditElement(RowColumnIndex rowColumnIndex, ContentControl uiElement, GridColumn column, object dataContext)
        {
            InitializeEditTemplate(uiElement, (GridTemplateColumn)column, dataContext);
            var Bind = new Binding { Path = new PropertyPath("Padding"), Mode = BindingMode.TwoWay, Source = column };
            uiElement.SetBinding(Control.PaddingProperty, Bind);
            Bind = new Binding { Path = new PropertyPath("HorizontalAlignment"), Mode = BindingMode.TwoWay, Source = column };
            uiElement.SetBinding(FrameworkElement.HorizontalAlignmentProperty, Bind);
            uiElement.HorizontalContentAlignment = HorizontalAlignment.Stretch;
#if !WP
            Bind = new Binding { Path = new PropertyPath("VerticalAlignment"), Mode = BindingMode.TwoWay, Source = column };
            uiElement.SetBinding(FrameworkElement.VerticalAlignmentProperty, Bind);
            uiElement.VerticalContentAlignment = VerticalAlignment.Stretch;
#endif
        }
        #endregion

        #region Display/Edit Value Overrides
#if !WP
        /// <summary>
        /// Called when [entered edit mode].
        /// </summary>
        /// <param name="currentRendererElement">The current renderer element.</param>
        protected override void OnEnteredEditMode(UIElement currentRendererElement)
        {
            UpdateCurrentCellState(currentRendererElement, CanEnterIntoEditMode);
            isfocused = false;
        }

        /// <summary>
        /// Called when [edit element lost focus].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        /// <remarks></remarks>
        protected override void OnEditElementLostFocus(object sender, RoutedEventArgs e)
        {
            //base.OnEditElementLostFocus(sender, e);
        }
#endif 
        /// <summary>
        /// Initializes the cell style.
        /// </summary>
        /// <param name="cellRowColumnIndex">Index of the cell row column.</param>
        /// <param name="record">The record.</param>
        /// <param name="cell">The cell.</param>
        /// <param name="column">The column.</param>
        protected override void InitializeCellStyle(RowColumnIndex cellRowColumnIndex, object record, UIElement cell, GridColumn column)
        {
            base.InitializeCellStyle(cellRowColumnIndex, record, cell, column);
            var uiElement = (cell as GridCell).Content as ContentControl;
            var gridColumn = column as GridTemplateColumn;
            if (uiElement != null && gridColumn != null)
            {
                if (HasCurrentCellState && CurrentCellIndex == cellRowColumnIndex && IsInEditing)
                    InitializeEditTemplate(uiElement, gridColumn, record);
                else
                    InitializeDisplayTemplate(uiElement, gridColumn, record);
            }
        }

        #endregion

        #region Wire/UnWire UIElements Overrides
#if !WP
        /// <summary>
        /// Called when [unwire edit unique identifier element].
        /// </summary>
        /// <param name="uiElement">The unique identifier element.</param>
        protected override void OnUnwireEditUIElement(ContentControl uiElement)
        {
            uiElement.ContentTemplate = null;
            base.OnUnwireEditUIElement(uiElement);
        }
#endif

        /// <summary>
        /// Called when [unwire display unique identifier element].
        /// </summary>
        /// <param name="uiElement">The unique identifier element.</param>
        protected override void OnUnwireDisplayUIElement(ContentControl uiElement)
        {
            uiElement.ContentTemplate = null;
            base.OnUnwireDisplayUIElement(uiElement);
        }
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
            
            if (!HasCurrentCellState) return false;
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
                case Key.Delete:
                {
                    if (handleTemplatedUIElementKeyDown)
                        return false;
                    return !IsInEditing;
                }
            }
            return false;
        }
        #endregion

        #endregion

        #region Private Methods

        protected override void SetFocus(UIElement element, bool needToFocus)
        {
            UIElement uiElement;
            if (needToFocus)
            {
                var focusedElement = FocusManagerHelper.GetFocusedUIElement(CurrentCellRendererElement);
                uiElement = focusedElement ?? element;
            }
            else
                uiElement = element;
            base.SetFocus(uiElement, needToFocus);
        }

#if !WP

        protected override void OnEditElementLoaded(object sender, RoutedEventArgs e)
        {
            var focusedElement = FocusManagerHelper.GetFocusedUIElement((UIElement)sender);
            if (focusedElement != null)
#if WinRT
                focusedElement.Focus(FocusState.Programmatic);
#else
                focusedElement.Focus();
#endif
            base.OnEditElementLoaded(sender, e);
        }
#endif

        /// <summary>
        /// Applies the display data template.
        /// </summary>
        /// <param name="uiElement">The unique identifier element.</param>
        /// <param name="templateColumn">The template column.</param>
        /// <param name="dataContext">The data context.</param>
        private void InitializeDisplayTemplate(ContentControl uiElement, GridTemplateColumn templateColumn, object dataContext)
        {
            DataTemplate template = null;
#if !SILVERLIGHT && !WP
            if (templateColumn.CellTemplateSelector != null)
                template = templateColumn.CellTemplateSelector.SelectTemplate(dataContext, uiElement);
#endif
            if (template == null && templateColumn.CellTemplate != null)
                template = templateColumn.CellTemplate;

            if (template != null)
                uiElement.ContentTemplate = template;
            uiElement.Content = dataContext;
        }

        /// <summary>
        /// Applies the edit data template.
        /// </summary>
        /// <param name="uiElement">The unique identifier element.</param>
        /// <param name="templateColumn">The template column.</param>
        /// <param name="dataContext">The data context.</param>
        private void InitializeEditTemplate(ContentControl uiElement, GridTemplateColumn templateColumn, object dataContext)
        {
            DataTemplate template = null;

#if !SILVERLIGHT && !WP
            if (templateColumn.EditTemplateSelector != null)
                template = templateColumn.EditTemplateSelector.SelectTemplate(dataContext, uiElement);
#endif
#if !WP
            CanEnterIntoEditMode = true;
            if (template == null && templateColumn.EditTemplate != null)
                template = templateColumn.EditTemplate;

            if (template == null && HasCurrentCellState)
                CanEnterIntoEditMode = false;
#endif

#if !SILVERLIGHT && !WP
            if (template == null && templateColumn.CellTemplateSelector != null)
                template = templateColumn.CellTemplateSelector.SelectTemplate(dataContext, uiElement);
#endif
            if (template == null && templateColumn.CellTemplate != null)
                template = templateColumn.CellTemplate;

            if (template != null)
                uiElement.ContentTemplate = template;
            uiElement.Content = dataContext;
        }
        #endregion
    }
}
