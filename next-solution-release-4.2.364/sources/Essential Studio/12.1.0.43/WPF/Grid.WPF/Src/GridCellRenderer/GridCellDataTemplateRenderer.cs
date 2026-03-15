#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Windows.Controls.Grid
{
    using System.ComponentModel;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;
    using System.Windows.Media;
    using Syncfusion.Windows.Controls.Scroll;
    using Syncfusion.Windows.GridCommon;
    using System;

    /// <summary>
    /// A cell model for the <see cref="GridCellDataTemplateRenderer"/>.
    /// </summary>
    public class GridCellDataTemplateModel : GridCellModel<GridCellDataTemplateRenderer>
    {
        /// <summary>
        /// Calculates the preferred size of the cell based on its content, including cell template. 
        /// </summary>
        /// <param name="rowIndex">Cell row index.</param>
        /// <param name="colIndex">Cell column index.</param>
        /// <param name="style">Cell style information.</param>
        /// <param name="queryBounds">Graphical bounds.</param>
        /// <returns>the optimal size of the cell.</returns>
        public override Size CalculatePreferredCellSize(int rowIndex, int colIndex, GridStyleInfo style, GridQueryBounds queryBounds)
        {
            DataTemplate dt = null;
            GridModel model = this.GridModel;
            GridControlBase view = null;
            var r = model.Views.GetEnumerator();
            if (r.MoveNext())
            {
                view = r.Current;
                if (view != null && string.IsNullOrEmpty(style.CellItemTemplateKey))
                {
                    dt = (DataTemplate)view.TryFindResource(style.CellItemTemplateKey);
                }

                if (dt != null)
                {
                    ContentControl contentControl = new ContentControl();
                    contentControl.ContentTemplate = dt;
                    contentControl.Measure(new Size(double.MaxValue, double.MaxValue));
                    contentControl.Content = style;
                    contentControl.Measure(new Size(double.MaxValue, double.MaxValue));
                    return contentControl.DesiredSize;
                }
            }

            return base.CalculatePreferredCellSize(rowIndex, colIndex, style, queryBounds);
        }
    }

    /// <summary>
    /// A renderer that manages a DataTemplate specified with <see cref="GridRenderStyleInfo.CellTemplateKey"/>
    /// of the <see cref="GridRenderStyleInfo"/> inside cells. The <see cref="ContentControl.Content"/>
    /// will be the  <see cref="GridRenderStyleInfo.CellValue"/> which binds the DataTemplate to the value
    /// of the style.
    /// </summary>
    public class GridCellDataTemplateRenderer : GridVirtualizingCellRenderer<ContentControl>
    {
        // Important: Breaking change !!!
        // Content is now the Style object -> Binding Path needs to include CellValue\
        // Example:  <TextBox Text="{Binding Path=CellValue.Title}"/>

        // See if CellValue implements NotifyPropertyChanged. Subscribe to NotifyPropertyChanged
        // when it becomes current cell. Unwire when end deactivated.


        RoutedEventHandler textBox_TextChangedHandler;
        SelectionChangedEventHandler selector_SelectionChangedHandler;
        object savedCellValue = null;
        INotifyPropertyChanged notifyPropertyChanged = null;

        /// <summary>
        /// Initializes a new <see cref="GridCellDataTemplateRenderer"/>.
        /// </summary>
        public GridCellDataTemplateRenderer()
        {
            this.IsFocusable = true;
            AllowRecycle = true;

            textBox_TextChangedHandler = new RoutedEventHandler(textBox_textChanged);
            selector_SelectionChangedHandler = new SelectionChangedEventHandler(selector_SelectionChanged);
        }

        /// <summary>
        /// Initializes the content of the data template cell
        /// using the information from the cell style (value, text,
        /// behavior etc.).
        /// </summary>
        /// <param name="uiElement">The cell UI element.</param>
        /// <param name="style">The cell style info.</param>
        public override void OnInitializeContent(ContentControl uiElement, GridRenderStyleInfo style)
        {
            base.OnInitializeContent(uiElement, style);

            bool found = false;

            if (style.CellEditTemplateKey != null)
            {
                DataTemplate dt = (DataTemplate)style.GridControl.TryFindResource(style.CellEditTemplateKey);
                found = dt != null;

                if (found)
                {
                    uiElement.ContentTemplate = dt;
                }
            }

            if (!found)
            {
                uiElement.ContentTemplate = style.CellEditTemplate;
            }

            GridControlBase.SetDelayLoad(uiElement, false);
            // We will set Content in OnElementMeasured after visual tree for template was created.
            //uiElement.Content = style;

            // Content will be reset when UIElement is recycled and UnwireUIElement is called.
        }

        protected override void ArrangeUIElement(Cells.ArrangeCellArgs aca, ContentControl uiElement, GridRenderStyleInfo style)
        {
            Thickness margins = style.TextMargins.ToThickness();
            margins = style.ErrorInfo.AdjustErrorInfoMarginOnEditing(margins, style.GridControl, style.CellRowColumnIndex);
            uiElement.Padding = margins;

            base.ArrangeUIElement(aca, uiElement, style);
        }


        public override void CreateRendererElement(ContentControl uiElement, GridRenderStyleInfo style)
        {
            base.CreateRendererElement(uiElement, style);

            bool found = false;

            if (style.CellItemTemplateKey != null)
            {
                DataTemplate dt = (DataTemplate)style.GridControl.TryFindResource(style.CellItemTemplateKey);
                found = dt != null;

                if (found)
                {
                    uiElement.ContentTemplate = dt;
                }
            }

            if (!found)
            {
                uiElement.ContentTemplate = style.CellItemTemplate;
            }

            GridControlBase.SetDelayLoad(uiElement, false);
        }

        protected override void OnElementArranged(System.Windows.UIElement el, System.Windows.Rect rect)
        {
            //el.Dispatcher.BeginInvoke(new Action(() =>
            //{
            //    el.Arrange(rect);
            //}), System.Windows.Threading.DispatcherPriority.Loaded, null);
            //base.OnElementArranged(el, rect);
        }

        protected override void OnElementMeasured(UIElement el, Size size)
        {
            //GridRenderStyleInfo style = GridControlBase.GetRenderStyleInfo(el);
            //ContentControl contentControl = (ContentControl)el;
            //contentControl.Content = style;
            //el.Dispatcher.BeginInvoke(new Action(() =>
            //{
            //    el.Measure(size);
            //}), System.Windows.Threading.DispatcherPriority.Loaded, null);
        }


        protected override void OnActivated()
        {
            base.OnActivated();
            if (CurrentCellUIElement != null)
            {
                GridStyleInfo style = CurrentCellUIElement.Content as GridStyleInfo;
                if (style != null)
                    notifyPropertyChanged = style.CellValue as INotifyPropertyChanged;
                if (notifyPropertyChanged != null)
                    notifyPropertyChanged.PropertyChanged += new PropertyChangedEventHandler(notifyPropertyChanged_PropertyChanged);
            }
        }

        protected override void OnDeactivated()
        {
            base.OnDeactivated();

            if (notifyPropertyChanged != null)
                notifyPropertyChanged.PropertyChanged -= new PropertyChangedEventHandler(notifyPropertyChanged_PropertyChanged);
            notifyPropertyChanged = null;
        }

        protected override bool ShouldGridTryToHandlePreviewKeyDown(KeyEventArgs e)
        {
            // return false to indicate the CurrentCellUIElement should handle the key
            // and the grid should ignore it.
            bool isControlKey = (e.KeyboardDevice.Modifiers & ModifierKeys.Control) != ModifierKeys.None;
            if (isControlKey)
                return true;

            switch (e.Key)
            {
                case Key.Tab:
                    return true;

                case Key.Right:
                case Key.Left:
                case Key.Down:
                case Key.Up:
                    {
                        // otherwise, move caret within textbox when cell is not in edit-mode.
                        return !CurrentCell.IsEditing;
                    }

                case Key.End:
                case Key.Home:
                    {
                        CurrentCell.BeginEdit(true);
                        return false;
                    }

                case Key.Delete:
                    {
                        CurrentCell.BeginEdit(true);
                        return false;
                    }


                    // break; Unreachable code
            }

            return base.ShouldGridTryToHandlePreviewKeyDown(e);
        }

        void notifyPropertyChanged_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // this should get called for checkboxes, slider controls that
            // directly change the property value.

            if (!IsInArrange && !CurrentCell.IsInCancelEdit)
            {
                object value = CurrentStyle.CellValue;

                if (!ValidateControlValue(value))
                {
                    if (savedCellValue != null)

                        // TODO: Rollback?
                        return;
                }

                if (!NotifyCurrentCellChanging())
                {
                    // TODO: Rollback?
                    return;
                }

                NotifyCurrentCellChanged();
            }
        }

        protected override void OnWireUIElement(ContentControl uiElement)
        {
            base.OnWireUIElement(uiElement);
            uiElement.AddHandler(TextBoxBase.TextChangedEvent, textBox_TextChangedHandler);
            uiElement.AddHandler(Selector.SelectionChangedEvent, selector_SelectionChangedHandler);

            // TODO: Is there a way to wire up an event by name?
            // Then a attached property could indicate the name of the changed event for controls and we could wire it up here.
        }

        protected override void OnUnwireUIElement(ContentControl uiElement)
        {
            uiElement.Content = null;
            uiElement.RemoveHandler(TextBoxBase.TextChangedEvent, textBox_TextChangedHandler);
            uiElement.RemoveHandler(Selector.SelectionChangedEvent, selector_SelectionChangedHandler);
            base.OnUnwireUIElement(uiElement);
        }

        void selector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsInArrange)
                return;

            Selector selector = e.OriginalSource as Selector;

            if (GridControlBase.GetIgnoreChangedEvent(selector) == true)
                return;

            if (IsCurrentCell(selector) && !CurrentCell.IsInEndEdit)
            {
                object value = CurrentStyle.CellValue;

                if (!ValidateControlValue(value))
                {
                    //RollbackTextChange(selector);
                    return;
                }

                if (!NotifyCurrentCellChanging())
                {
                    //RollbackTextChange(textBox);
                    return;
                }

                NotifyCurrentCellChanged();
            }
        }

        void textBox_textChanged(object sender, RoutedEventArgs e)
        {
            if (IsInArrange)
                return;

            TextBoxBase textBox = e.OriginalSource as TextBoxBase;

            if (GridControlBase.GetIgnoreChangedEvent(textBox) == true)
                return;

            if (IsCurrentCell(textBox) && !CurrentCell.IsInEndEdit)
            {
                object value = CurrentStyle.CellValue;

                if (!ValidateControlValue(value))
                {
                    RollbackTextChange(textBox);
                    return;
                }

                if (!NotifyCurrentCellChanging())
                {
                    RollbackTextChange(textBox);
                    return;
                }

                NotifyCurrentCellChanged();
            }
        }

        protected override string GetControlTextFromEditorCore(ContentControl uiElement)
        {
            return CurrentStyle.CellValue.ToString();
        }

        protected override void OnGridPreviewTextInput(TextCompositionEventArgs e)
        {
            CurrentCell.ScrollInView();
            CurrentCell.BeginEdit(true);
        }
    }
}
