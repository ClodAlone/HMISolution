#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Windows.Controls.Grid
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows.Controls.Primitives;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using Syncfusion.Windows.Controls.Scroll;
    using System.Windows.Input;
    using Syncfusion.Windows.Controls.Cells;
    using System.ComponentModel;
    using Syncfusion.Windows.Data;
    using System.Windows.Data;


    //public class GridDataCellBoundWrapper : DependencyObject
    //{
    //    public static readonly DependencyProperty CellBoundValueProperty = DependencyProperty.Register(
    //        "CellBoundValue",
    //        typeof(object),
    //        typeof(GridDataCellBoundWrapper),
    //        new FrameworkPropertyMetadata(OnValueChanged));

    //    /// <summary>
    //    /// Gets or sets the CellBoundValue. This is a DependencyProperty exposed for the
    //    /// underlying object data source. The object can be binded to an external WPF
    //    /// control's DependencyProperty in Two-way mode. The changes reflected in the
    //    /// object would be passed on to the underlying object accordingly.
    //    /// <para></para>
    //    /// <para></para>
    //    /// <code>    &lt;syncfusion:GridDataVisibleColumn MappingName=&quot;LastName&quot; HeaderText=&quot;LastName&quot;
    //    /// Width=&quot;90&quot; &gt;
    //    ///                         &lt;syncfusion:GridDataVisibleColumn.CellTemplate&gt;
    //    ///                             &lt;DataTemplate&gt;
    //    ///                                 &lt;TextBox Text=&quot;{Binding
    //    /// Path=CellBoundValue, Mode=TwoWay}&quot;
    //    ///                                        Foreground=&quot;Black&quot;
    //    /// syncfusion:VisualContainer.WantsMouseInput=&quot;True&quot;/&gt;
    //    ///                             &lt;/DataTemplate&gt;
    //    ///                         &lt;/syncfusion:GridDataVisibleColumn.CellTemplate&gt;
    //    ///                     &lt;/syncfusion:GridDataVisibleColumn&gt;</code>
    //    /// </summary>
    //    public object CellBoundValue
    //    {
    //        get
    //        {
    //            return this.GetValue(GridDataCellBoundWrapper.CellBoundValueProperty);
    //        }
    //        set
    //        {
    //            this.SetValue(GridDataCellBoundWrapper.CellBoundValueProperty, value);
    //        }
    //    }

    //    /// <summary>
    //    /// Specifies the cell style information.
    //    /// </summary>
    //    public GridStyleInfo Style
    //    {
    //        get;
    //        internal set;
    //    }

    //    private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
    //    {
    //        var boundObject = d as GridDataCellBoundWrapper;
    //        if (boundObject != null)
    //        {
    //            boundObject.RaiseOnValueChanged(args.NewValue);
    //        }
    //    }

    //    private void RaiseOnValueChanged(object value)
    //    {
    //        if (this.ValueChanged != null && value != null)
    //        {
    //            this.ValueChanged(this, new GridDataValueEventArgs<object>(value));
    //        }
    //    }

    //    /// <summary>
    //    /// Occurs when the cell bound value changes.
    //    /// </summary>
    //    public event EventHandler<GridDataValueEventArgs<object>> ValueChanged;

    //    public object Record
    //    {
    //        get;
    //        internal set;
    //    }
    //}

    //public class GridDataDataBoundTemplateCellBoundModel : GridCellModel<GridDataDataBoundTemplateCellRenderer>
    //{
    //    public override Size CalculatePreferredCellSize(int rowIndex, int colIndex, GridStyleInfo style, GridQueryBounds queryBounds)
    //    {
    //        DataTemplate dt = null;
    //        GridModel model = this.GridModel;
    //        GridControlBase view = null;
    //        var r = model.Views.GetEnumerator();
    //        if (r.MoveNext())
    //        {
    //            view = r.Current;
    //            if (view != null && style.CellItemTemplateKey != null && style.CellItemTemplateKey != string.Empty)
    //            {
    //                dt = (DataTemplate)view.TryFindResource(style.CellItemTemplateKey);
    //            }
    //            else if (style.CellItemTemplate != null)
    //            {
    //                dt = style.CellItemTemplate;
    //            }

    //            if (dt != null)
    //            {
    //                // we use ContentControl to determine the max size
    //                ContentControl contentControl = new ContentControl();
    //                contentControl.ContentTemplate = dt;
    //                contentControl.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
    //                // create the wrapper and set it to the Content, we try to recreate the UI binding here
    //                var cellBoundWrapper = new GridDataCellBoundWrapper() { CellBoundValue = style.CellValue, Style = style };
    //                if (style is GridDataStyleInfo)
    //                {
    //                    var styleInfo = style as GridDataStyleInfo;
    //                    var tableModel = this.GridModel as GridDataTableModel;
    //                    if (tableModel != null)
    //                    {
    //                        var recordIdx = styleInfo.CellIdentity.RecordIndex;
    //                        if (recordIdx > -1 && recordIdx < tableModel.SourceListCount)
    //                        {
    //                            var record = tableModel.View.Records[recordIdx];
    //                            cellBoundWrapper.Record = record;
    //                        }
    //                    }
    //                }

    //                contentControl.Content = cellBoundWrapper;
    //                // do a measure with positive infinity to compute the max value
    //                contentControl.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
    //                return contentControl.DesiredSize;
    //            }
    //        }

    //        return base.CalculatePreferredCellSize(rowIndex, colIndex, style, queryBounds);
    //    }
    //}

    //public class GridDataDataBoundTemplateCellRenderer : GridVirtualizingCellRenderer<ContentControl>
    //{
    //    SelectionChangedEventHandler selector_SelectionChangedHandler;

    //    public GridDataDataBoundTemplateCellRenderer()
    //    {
    //        this.SupportsRenderOptimization = false;
    //        this.AllowRecycle = true;
    //        this.IsControlTextShown = false;
    //        this.IsFocusable = true;
    //        this.ListenToSelectorChanged = false;
    //        this.AllowInvalidateMeasureChildren = false;
    //        this.selector_SelectionChangedHandler = new SelectionChangedEventHandler(OnSelectorSelectionChanged);
    //        this.AllowGridToFocus = false;
    //    }

    //    /// <summary>
    //    /// Gets or sets a value that indicates whether to listen to the selector-changed event.
    //    /// </summary>
    //    public bool ListenToSelectorChanged
    //    {
    //        get;
    //        set;
    //    }

    //    /// <summary>
    //    /// When it is true, the children are arranged with desired size.
    //    /// </summary>
    //    public bool AllowInvalidateMeasureChildren
    //    {
    //        get;
    //        set;
    //    }

    //    protected override void OnElementArranged(UIElement el, Rect rect)
    //    {
    //        // call this method to arrange the children with desired size, when template selector is used the children are dynamically loaded after the parent UIElement's ApplyTemplate is called.
    //        //if (AllowInvalidateMeasureChildren)
    //        //{
    //        //    InvalidateMeasureChildren(el, rect);
    //        //}
    //        //el.Dispatcher.BeginInvoke(new Action(() =>
    //        //{
    //        //    el.Arrange(rect);
    //        //}), null);
    //    }

    //    private void InvalidateMeasureChildren(UIElement el, Rect rect)
    //    {
    //        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(el); i++)
    //        {
    //            UIElement child = VisualTreeHelper.GetChild(el, i) as UIElement;
    //            if (child != null)
    //            {
    //                child.Measure(new Size(double.MaxValue, double.MaxValue));
    //                InvalidateMeasureChildren(child, rect);
    //            }
    //        }
    //    }


    //    protected override void OnEnteredEditMode()
    //    {
    //        base.OnEnteredEditMode();

    //        if (this.CurrentCellUIElement == null)
    //        {
    //            return;
    //        }

    //        int childCount = VisualTreeHelper.GetChildrenCount(this.CurrentCellUIElement);
    //        if (childCount == 1)
    //        {
    //            this.CurrentCellUIElement.Focus();
    //        }
    //    }

    //    protected override void OnSetFocus()
    //    {
    //        base.OnSetFocus();
    //        if (this.CurrentCellUIElement != null)
    //        {
    //            this.CurrentCellUIElement.Dispatcher.BeginInvoke(new Action(() =>
    //            {
    //                this.CurrentCellUIElement.Focus();
    //            }));
    //        }
    //    }

    //    protected override bool ShouldGridTryToHandlePreviewKeyDown(System.Windows.Input.KeyEventArgs e)
    //    {
    //        if (e.Key == System.Windows.Input.Key.Tab && this.CurrentCellUIElement != null)
    //        {
    //            int childCount = VisualTreeHelper.GetChildrenCount(this.CurrentCellUIElement);
    //            if (childCount > 1)
    //            {
    //                bool focus = false;
    //                for (int count = 0; count < childCount; count++)
    //                {
    //                    UIElement childUI = VisualTreeHelper.GetChild(this.CurrentCellUIElement, count) as UIElement;
    //                    if (childUI.IsFocused)
    //                    {
    //                        focus = true;
    //                        break;
    //                    }
    //                }

    //                if (focus)
    //                {
    //                    UIElement lastChild = VisualTreeHelper.GetChild(this.CurrentCellUIElement, childCount - 1) as UIElement;

    //                    if (!lastChild.IsFocused || (e.KeyboardDevice.Modifiers == System.Windows.Input.ModifierKeys.Shift))
    //                    {
    //                        return false;
    //                    }
    //                    else
    //                    {
    //                        this.GridControl.Focus();
    //                    }
    //                }
    //            }
    //            else
    //            {
    //                this.CurrentCell.EndEdit();
    //            }
    //        }

    //        if (this.CurrentCellUIElement != null)
    //        {
    //            var allowKeyHandling = GridControlBase.GetAllowDropDownKeyHandling(this.CurrentCellUIElement);
    //            if (allowKeyHandling)
    //            {
    //                if (e.Key == Key.Up || e.Key == Key.Down)
    //                {
    //                    return false;
    //                }

    //                if (e.Key == Key.Enter || e.Key == Key.Escape)
    //                {
    //                    // this.CurrentCell.CancelEdit(false);
    //                    return false;
    //                }
    //            }
    //        }

    //        return true;
    //    }

    //    //public override void RaiseGridPreviewMouseMove(Syncfusion.Windows.Controls.Cells.RowColumnIndex rci, System.Windows.Input.MouseEventArgs e)
    //    //{
    //    //    base.RaiseGridPreviewMouseMove(rci, e);

    //    //    if (this.CurrentCell.RowIndex == rci.RowIndex && this.CurrentCell.ColumnIndex != rci.ColumnIndex)
    //    //    {
    //    //        this.CurrentCell.MoveTo(rci.RowIndex, rci.ColumnIndex);
    //    //    }
    //    //}
    //    protected override void OnEditingComplete()
    //    {
    //        // don't call the base code here
    //        // base.OnEditingComplete();
    //    }

    //    public override void OnInitializeContent(ContentControl uiElement, GridRenderStyleInfo style)
    //    {
    //        base.OnInitializeContent(uiElement, style);
    //        bool found = false;
    //        DataTemplate dt = null;
    //        if (style.CellEditTemplateKey != null && style.CellEditTemplateKey != string.Empty)
    //        {
    //            dt = (DataTemplate)style.GridControl.TryFindResource(style.CellEditTemplateKey);
    //            found = dt != null;
    //        }
    //        //else if (style.CellEditTemplate != null)
    //        //{
    //        //    dt = style.CellEditTemplate;
    //        //    found = true;
    //        //}
    //        if (!found && style.CellEditTemplate != null)
    //        {
    //            dt = style.CellEditTemplate;
    //            found = true;
    //        }

    //        if (found)
    //        {
    //            this.OnUnwireUIElement(uiElement);
    //            uiElement.ContentTemplate = dt;
    //            //dt.Seal();
    //            //var frameWorkelement = dt.LoadContent() as FrameworkElement;

    //            //if (frameWorkelement == null)
    //            //{
    //            //    return;
    //            //}
    //            //frameWorkelement.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
    //            //uiElement = (ContentControl)frameWorkelement;
    //            GridDataCellBoundWrapper wrapperInstance = uiElement.DataContext as GridDataCellBoundWrapper;
    //            if (wrapperInstance == null)
    //            {
    //                wrapperInstance = this.CreateWrapperInstance();
    //            }
    //            wrapperInstance.CellBoundValue = style.CellValue;
    //            wrapperInstance.Style = style;
    //            this.ProvideWrapperInstance(wrapperInstance, style);
    //            uiElement.DataContext = wrapperInstance;
    //            //var wantsMouseInput = VisualContainer.GetWantsMouseInput(frameWorkelement, uiElement);
    //            //VisualContainer.SetWantsMouseInput(uiElement, wantsMouseInput);
    //            this.OnWireUIElement(uiElement);
    //            // focus on TAB, OnInitializeContent gets called resetting focus on the current element.
    //            if (uiElement != null)
    //            {
    //                var cellRowColIndex = VirtualizingCellsControl.GetCellRowColumnIndex(uiElement);
    //                if (this.CurrentCell.CellRowColumnIndex == cellRowColIndex)
    //                {
    //                    uiElement.Focus();
    //                }
    //            }
    //        }
    //    }

    //    public override void CreateRendererElement(ContentControl uiElement, GridRenderStyleInfo style)
    //    {
    //        base.CreateRendererElement(uiElement, style);
    //        bool found = false;
    //        DataTemplate dt = null;
    //        if (style.CellItemTemplateKey != null && style.CellItemTemplateKey != string.Empty)
    //        {
    //            dt = (DataTemplate)style.GridControl.TryFindResource(style.CellItemTemplateKey);
    //            found = dt != null;
    //        }
    //        //else if (style.CellEditTemplate != null)
    //        //{
    //        //    dt = style.CellEditTemplate;
    //        //    found = true;
    //        //}
    //        if (!found && style.CellItemTemplate != null)
    //        {
    //            dt = style.CellItemTemplate;
    //            found = true;
    //        }

    //        if (found)
    //        {

    //            this.OnUnwireUIElement(uiElement);
    //            uiElement.ContentTemplate = dt;
    //            //dt.Seal();
    //            //var frameWorkelement = dt.LoadContent() as FrameworkElement;

    //            //if (frameWorkelement == null)
    //            //{
    //            //    return;
    //            //}
    //            //frameWorkelement.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
    //            //uiElement = (ContentControl)frameWorkelement;
    //            //uiElement = frameWorkelement;
    //            //((CheckBox)uiElement.Child).Checked += new RoutedEventHandler(GridDataDataBoundTemplateCellRenderer_Checked);
    //            //((CheckBox)uiElement.Child).MouseMove += new MouseEventHandler(GridDataDataBoundTemplateCellRenderer_MouseMove);
    //            GridDataCellBoundWrapper wrapperInstance = uiElement.DataContext as GridDataCellBoundWrapper;
    //            if (wrapperInstance == null)
    //            {
    //                wrapperInstance = this.CreateWrapperInstance();
    //            }
    //            wrapperInstance.CellBoundValue = style.CellValue;
    //            wrapperInstance.Style = style;
    //            this.ProvideWrapperInstance(wrapperInstance, style);
    //            uiElement.DataContext = wrapperInstance;
    //            //var wantsMouseInput = VisualContainer.GetWantsMouseInput(frameWorkelement, uiElement);
    //            //VisualContainer.SetWantsMouseInput(uiElement, wantsMouseInput);
    //            GridControlBase.SetDelayLoad(uiElement, false);
    //            this.OnWireUIElement(uiElement);
    //            // focus on TAB, OnInitializeContent gets called resetting focus on the current element.
    //            if (uiElement != null)
    //            {
    //                var cellRowColIndex = VirtualizingCellsControl.GetCellRowColumnIndex(uiElement);
    //                if (this.CurrentCell.CellRowColumnIndex == cellRowColIndex)
    //                {
    //                    uiElement.Focus();
    //                }
    //            }
    //        }
    //    }

    //    protected virtual GridDataCellBoundWrapper CreateWrapperInstance()
    //    {
    //        return new GridDataCellBoundWrapper();
    //    }

    //    protected virtual void ProvideWrapperInstance(GridDataCellBoundWrapper wrapperInstance, GridRenderStyleInfo style)
    //    {
    //        if (style.ModelStyle is GridDataStyleInfo)
    //        {
    //            var styleInfo = style.ModelStyle as GridDataStyleInfo;
    //            if (this.TableModel != null)
    //            {
    //                wrapperInstance.Record = styleInfo.CellIdentity.RecordEntry;
    //                //var recordIdx = styleInfo.CellIdentity.Record;
    //                //if (recordIdx > -1 && recordIdx < this.TableModel.SourceListCount)
    //                //{
    //                //    var record = this.TableModel.View.Records[recordIdx];
    //                //    wrapperInstance.Record = record;
    //                //}
    //            }
    //        }
    //    }

    //    protected override void OnGridPreviewTextInput(TextCompositionEventArgs e)
    //    {
    //        if (CurrentCell.IsEditing)
    //            return;
    //        CurrentCell.ScrollInView();
    //        CurrentCell.BeginEdit(true);
    //        //e.Handled = true;
    //    }

    //    protected override void OnElementMeasured(System.Windows.UIElement el, System.Windows.Size size)
    //    {
    //        //el.Dispatcher.BeginInvoke(new Action(() =>
    //        //{
    //        //    el.Measure(size);
    //        //}), null);
    //    }

    //    protected GridDataTableModel TableModel
    //    {
    //        get
    //        {
    //            var tableModel = this.GridControl.Model as GridDataTableModel;
    //            return tableModel;
    //        }
    //    }

    //    protected override void OnWireUIElement(ContentControl uiElement)
    //    {
    //        base.OnWireUIElement(uiElement);
    //        GridDataCellBoundWrapper wrapperInstance = uiElement.DataContext as GridDataCellBoundWrapper;
    //        if (wrapperInstance != null)
    //        {
    //            wrapperInstance.ValueChanged += new EventHandler<GridDataValueEventArgs<object>>(wrapperInstance_ValueChanged);
    //        }
    //        if (this.ListenToSelectorChanged)
    //        {
    //            uiElement.AddHandler(Selector.SelectionChangedEvent, selector_SelectionChangedHandler);
    //        }

    //        //uiElement.PreviewKeyDown += new KeyEventHandler(uiElement_PreviewKeyDown);
    //        //var childEl = uiElement.Child as FrameworkElement;
    //        //var isSelector = childEl.FindElementOfType<Selector>() != null;
    //        // for dropdown selectors disable selection handlers
    //        //var wantSelection = false;
    //        //if (childEl != null)
    //        //    wantSelection = GridControlBase.GetAllowSelectionInDataTemplate(childEl as FrameworkElement);
    //        //if (wantSelection) 
    //        //{
    //        //    uiElement.PreviewMouseUp += new MouseButtonEventHandler(uiElement_PreviewMouseUp);
    //        //    uiElement.PreviewMouseMove += new MouseEventHandler(uiElement_PreviewMouseMove);
    //        //    uiElement.PreviewMouseDown += new MouseButtonEventHandler(uiElement_PreviewMouseDown);
    //        //}

    //        //if (childEl != null)
    //        //{
    //        //    var isAllowKeyHandling = GridControlBase.GetAllowDropDownKeyHandling(childEl);
    //        //    if (isAllowKeyHandling)
    //        //    {
    //        //        uiElement.GotFocus += new RoutedEventHandler(uiElement_GotFocus);
    //        //    }
    //        //}
    //    }

    //    void uiElement_GotFocus(object sender, RoutedEventArgs e)
    //    {
    //        // System.Diagnostics.Debug.WriteLine(string.Format("Type {0} IsFocused {1}", Keyboard.FocusedElement.GetType().Name, Keyboard.FocusedElement.IsKeyboardFocused));
    //        var border = sender as Border;
    //        if (!border.Child.IsFocused)
    //        {
    //            border.Child.Focus();
    //        }
    //    }

    //    //void uiElement_IsKeyboardFocusWithinChanged(object sender, DependencyPropertyChangedEventArgs e)
    //    //{
    //    //    if (this.HasCurrentCellState)
    //    //    {
    //    //        int childCount = VisualTreeHelper.GetChildrenCount(this.CurrentCellUIElement.Child);
    //    //        if (childCount == 1)//&& this.CurrentCellUIElement.IsKeyboardFocusWithin)
    //    //        {
    //    //            this.CurrentCellUIElement.Child.Focus();
    //    //        }
    //    //    }
    //    //}

    //    private void uiElement_PreviewKeyDown(object sender, KeyEventArgs e)
    //    {
    //        //if (!this.HasCurrentCellState)
    //        //{
    //        //    return;
    //        //}
    //        //if (this.CurrentCellUIElement != null)
    //        //{
    //        //    var allowKeyHandling = GridControlBase.GetAllowDropDownKeyHandling(this.CurrentCellUIElement.Child);
    //        //    if (allowKeyHandling)
    //        //    {
    //        //        if (e.Key == Key.Space)
    //        //        {
    //        //            CurrentCell.ScrollInView();
    //        //            CurrentCell.BeginEdit(true);

    //        //            //CurrentCellUIElement.Child.Focus();
    //        //            var cb = CurrentCellUIElement.Child as ComboBox;
    //        //            if (cb != null)
    //        //            {
    //        //                if (cb.IsDropDownOpen)
    //        //                {
    //        //                    cb.IsDropDownOpen = false;
    //        //                }
    //        //                else
    //        //                {
    //        //                    cb.IsDropDownOpen = true;
    //        //                }
    //        //            }
    //        //            e.Handled = true;
    //        //        }

    //        //        if (e.Key == Key.Enter)
    //        //        {
    //        //            CurrentCell.EndEdit();
    //        //        }
    //        //    }
    //        //}
    //    }

    //    protected override void OnCancelMouseCapture(UIElement element)
    //    {
    //        if (element is ComboBox)
    //        {
    //            return;
    //        }

    //        base.OnCancelMouseCapture(element);
    //    }

    //    private void uiElement_PreviewMouseUp(object sender, MouseButtonEventArgs e)
    //    {
    //        this.isInSelection = false;
    //        if (this.GridControl != null)
    //        {
    //            var mouseController = this.GridControl.MouseControllerDispatcher.Find("SelectCellsMouseController") as GridSelectCellsMouseController;
    //            if (mouseController != null && e.LeftButton == MouseButtonState.Pressed)
    //            {
    //                mouseController.EndSelectCellsController();
    //            }
    //        }
    //    }

    //    private void uiElement_PreviewMouseMove(object sender, MouseEventArgs e)
    //    {
    //        if (this.GridControl != null && this.isInSelection)
    //        {
    //            var rowColumnIndex = this.GridControl.PointToCellRowColumnIndex(e);
    //            var mouseController = this.GridControl.MouseControllerDispatcher.Find("SelectCellsMouseController") as GridSelectCellsMouseController;
    //            if (mouseController != null && e.LeftButton == MouseButtonState.Pressed)
    //            {
    //                mouseController.ChangeSelectCellsController(rowColumnIndex.RowIndex, rowColumnIndex.ColumnIndex);//, Keyboard.Modifiers);
    //                e.Handled = true;
    //            }
    //        }
    //    }

    //    private bool isInSelection = false;

    //    private void uiElement_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    //    {
    //        if (this.GridControl != null)
    //        {
    //            var rowColumnIndex = this.GridControl.PointToCellRowColumnIndex(e);
    //            var mouseController = this.GridControl.MouseControllerDispatcher.Find("SelectCellsMouseController") as GridSelectCellsMouseController;
    //            if (mouseController != null && e.LeftButton == MouseButtonState.Pressed)
    //            {
    //                this.isInSelection = true;
    //                mouseController.BeginSelectCellsController(rowColumnIndex.RowIndex, rowColumnIndex.ColumnIndex, Keyboard.Modifiers);
    //            }
    //        }
    //    }

    //    protected override void OnUnwireUIElement(ContentControl uiElement)
    //    {
    //        if (this.ListenToSelectorChanged)
    //        {
    //            uiElement.RemoveHandler(Selector.SelectionChangedEvent, selector_SelectionChangedHandler);
    //        }
    //        base.OnUnwireUIElement(uiElement);
    //        GridDataCellBoundWrapper wrapperInstance = uiElement.DataContext as GridDataCellBoundWrapper;
    //        if (wrapperInstance != null)
    //        {
    //            wrapperInstance.ValueChanged -= new EventHandler<GridDataValueEventArgs<object>>(wrapperInstance_ValueChanged);
    //        }

    //        //var childEl = uiElement.Child as FrameworkElement;
    //        //if (childEl != null)
    //        //{
    //        //    var isAllowKeyHandling = GridControlBase.GetAllowDropDownKeyHandling(childEl);
    //        //    if (isAllowKeyHandling)
    //        //    {
    //        //        uiElement.GotFocus -= new RoutedEventHandler(uiElement_GotFocus);
    //        //    }
    //        //}

    //        //uiElement.PreviewKeyDown -= new KeyEventHandler(uiElement_PreviewKeyDown);
    //        //uiElement.PreviewMouseUp -= new MouseButtonEventHandler(uiElement_PreviewMouseUp);
    //        //uiElement.PreviewMouseMove -= new MouseEventHandler(uiElement_PreviewMouseMove);
    //        //uiElement.PreviewMouseDown -= new MouseButtonEventHandler(uiElement_PreviewMouseDown);
    //        uiElement.DataContext = null;
    //    }

    //    void wrapperInstance_ValueChanged(object sender, GridDataValueEventArgs<object> e)
    //    {
    //        this.CurrentCell.BeginEdit();
    //        if (!this.IsInArrange && !this.CurrentCell.IsInEndEdit)
    //        {
    //            if (!this.SetControlValue(e.Value))
    //            {
    //                RefreshContent();
    //            }
    //        }
    //        this.CurrentCell.EndEdit();
    //    }

    //    protected override object GetControlValueFromEditorCore(ContentControl uiElement)
    //    {
    //        var wrapperInstance = uiElement.DataContext as GridDataCellBoundWrapper;
    //        if (wrapperInstance != null)
    //        {
    //            return wrapperInstance.CellBoundValue;
    //        }

    //        return base.GetControlValueFromEditorCore(uiElement);
    //    }

    //    protected override string GetControlTextFromEditorCore(ContentControl uiElement)
    //    {
    //        return CurrentStyle.CellValue.ToString();
    //    }

    //    protected virtual void OnSelectorSelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    //    {
    //        //if (IsInArrange)
    //        //{
    //        //    return;
    //        //}

    //        //Selector selector = e.OriginalSource as Selector;
    //        //if (GridControlBase.GetIgnoreChangedEvent(selector) == true)
    //        //{
    //        //    return;
    //        //}
    //        //else
    //        //{
    //        //    // call endedit to inform the current record manager to call the final end edit
    //        //    this.CurrentCell.EndEdit();
    //        //}
    //    }
    //}


    // - Inherited from GridControl GridCellDataBoundTemplateRenderer

    public class GridDataCellBoundWrapper : GridCellBoundWrapper
    {
        public object Record
        {
            get;
            set;
        }
    }


    public class GridDataDataBoundTemplateCellBoundModel : GridCellModel<GridDataDataTemplateCellRenderer>
    {
        public override Size CalculatePreferredCellSize(int rowIndex, int colIndex, GridStyleInfo style, GridQueryBounds queryBounds)
        {
            GridModel model = this.GridModel;
            GridControlBase view = null;
            var r = model.Views.GetEnumerator();
            if (r.MoveNext())
            {
                view = r.Current;
                bool found = false;
                DataTemplate dt = null;
                var uiElement = new GridCell();
                if (!string.IsNullOrEmpty(style.CellItemTemplateKey))
                {
                    //dt = (DataTemplate)style.GridControl.TryFindResource(style.CellItemTemplateKey);
                    dt = (DataTemplate)view.TryFindResource(style.CellItemTemplateKey);
                    found = dt != null;
                }
                else if (style.CellItemTemplate != null)
                {
                    dt = style.CellItemTemplate;
                    found = true;
                }
                if (found)
                {
                    uiElement.DataSource = null;
                    uiElement.ContentTemplate = dt;
                    uiElement.CellRowColumnIndex = style.CellRowColumnIndex;
                    var datagrid = view.FindParentElementOfType<GridDataControl>();
                    if (datagrid != null)
                    {
                        var wrapperInstance = new GridDataCellBoundWrapper
                            {
                                CellBoundValue = style.CellValue,
                                Style = style
                            };
                        this.ProvideDataWrapperInstance(wrapperInstance, style);
                        uiElement.Content = wrapperInstance;
                        uiElement.SetValue(ContentControl.DataContextProperty, wrapperInstance);
                    }

                }
                uiElement.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                return uiElement.DesiredSize;
            }
            return base.CalculatePreferredCellSize(rowIndex, colIndex, style, queryBounds);
        }

        public void ProvideDataWrapperInstance(GridDataCellBoundWrapper wrapperInstance, GridStyleInfo style)
        {
            if (style is GridDataStyleInfo)
            {
                var styleInfo = style as GridDataStyleInfo;
                var recordIdx = styleInfo.CellIdentity.RecordIndex;
                var TableModel = style.GridModel as GridDataTableModel;
                if (recordIdx > -1 && styleInfo.CellIdentity.TableCellType != GridDataTableCellType.AddNewRecordCell)
                {
                    if (!TableModel.Table.HasGroups)
                    {
                        if (recordIdx < TableModel.View.Records.Count)
                        {
                            wrapperInstance.Record = TableModel.View.Records[recordIdx];
                        }
                    }
                    else
                    {
                        if (TableModel.View.TopLevelGroup.DisplayElements.Count >= recordIdx)
                        {
                            var recordEntry = TableModel.View.TopLevelGroup.DisplayElements[recordIdx] as RecordEntry;
                            if (recordEntry != null)
                            {
                                wrapperInstance.Record = recordEntry;
                            }
                        }
                    }
                }
                else if (styleInfo.CellIdentity.TableCellType == GridDataTableCellType.AddNewRecordCell)
                {
                    //var datagrid = this.GridControl.FindParentElementOfType<GridDataControl>();
                    //if (datagrid != null && datagrid.Model.CurrencyManager.newItem != null)
                    //{
                    //    wrapperInstance.Record = TableModel.View.Records.CreateRecordEntry(datagrid.Model.CurrencyManager.newItem);
                    //}
                }
            }
        }

    }


    public class GridDataDataTemplateCellRenderer : GridCellDataBoundTemplateRenderer
    {
        public GridDataDataTemplateCellRenderer()
        {
            this.AllowCancelMouseCapture = false;
            this.IsFocusable = true;
            this.AllowRecycle = false;
            this.ListenToSelectorChanged = true;
            this.AllowInvalidateMeasureChildren = false;
            this.AllowRecycleWrapper = false;
        }

        protected override void OnUnwireUIElement(GridCell uiElement)
        {
            //var context = uiElement.DataContext;
            if (this.HasCurrentCellState)
            {
                var wrapperInstance = uiElement.DataSource as GridDataCellBoundWrapper;
                if (wrapperInstance != null)
                {
                    wrapperInstance.ValueChanged -= wrapperInstance_ValueChanged;
                    
                }
            }
            uiElement.DataSource = null;
        }

        protected override void OnEnteredEditMode()
        {
            if (CurrentCellUIElement != null)
            {
                var style = CurrentStyle;
                var text = GetControlText(style);
                if (this.ControlValue != null)
                {
                    if (this.ControlValue.ToString()!=text)
                    {
                        this.ControlValue = text;
                    }
                    //if (((object)this.ControlValue).Equals(text))
                    //    this.ControlValue = text;
                }   
            }
            else
            {
                GridControl.InvalidateCell(CellRowColumnIndex);
            }
        }

        protected override void OnWireUIElement(GridCell uiElement)
        {
            if (this.HasCurrentCellState)
            {
                var wrapperInstance = uiElement.DataSource as GridDataCellBoundWrapper;
                if (wrapperInstance != null)
                {
                    wrapperInstance.ValueChanged += wrapperInstance_ValueChanged;
                }
            }            
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
        }

        protected override void OnEditingComplete()
        {
            base.OnEditingComplete();
            //Already Invalidating the cell in deactivate method
            //this.GridControl.InvalidateCell(CellRowColumnIndex);
        }

        public override void wrapperInstance_ValueChanged(object sender, GridDataValueEventArgs<object> e)
        {
            if (!this.IsInArrange && !this.CurrentCell.IsInEndEdit)
            {
                if (this.HasCurrentCellState)
                {
                    if (!this.SetControlValue(e.Value))
                    {
                        RefreshContent();
                    }
                }
            }
        }
    }

}
