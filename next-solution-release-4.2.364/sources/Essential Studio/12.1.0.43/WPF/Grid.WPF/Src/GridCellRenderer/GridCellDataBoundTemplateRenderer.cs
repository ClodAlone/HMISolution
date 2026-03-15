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
    using Syncfusion.Linq;
    using System.Linq;
    using Syncfusion.Windows.Collections;
    using Syncfusion.Windows.Controls.Cells;
    using Syncfusion.Windows.Data;

    /// <summary>
    /// This is a wrapper class that helps to bind a dependency property of an external WPF control with its cell bound value.
    /// This makes it possible to accommodate a data-bound template in grid cells.
    /// </summary>
    public class GridCellBoundWrapper : DependencyObject
    {
        public static readonly DependencyProperty CellBoundValueProperty = DependencyProperty.Register("CellBoundValue", typeof (object), typeof (GridCellBoundWrapper),new FrameworkPropertyMetadata(OnValueChanged));

        /// <summary>
        /// Gets or sets the CellBoundValue. This is a DependencyProperty exposed for the
        /// underlying object data source. The object can be binded to an external WPF
        /// control's DependencyProperty in Two-way mode. The changes reflected in the
        /// object would be passed on to the underlying object accordingly.
        /// <para></para>
        /// <para></para>
        /// <code>    &lt;syncfusion:GridDataVisibleColumn MappingName=&quot;LastName&quot; HeaderText=&quot;LastName&quot;
        /// Width=&quot;90&quot; &gt;
        ///                         &lt;syncfusion:GridDataVisibleColumn.CellTemplate&gt;
        ///                             &lt;DataTemplate&gt;
        ///                                 &lt;TextBox Text=&quot;{Binding
        /// Path=CellBoundValue, Mode=TwoWay}&quot;
        ///                                        Foreground=&quot;Black&quot;
        /// syncfusion:VisualContainer.WantsMouseInput=&quot;True&quot;/&gt;
        ///                             &lt;/DataTemplate&gt;
        ///                         &lt;/syncfusion:GridDataVisibleColumn.CellTemplate&gt;
        ///                     &lt;/syncfusion:GridDataVisibleColumn&gt;</code>
        /// </summary>
        public object CellBoundValue
        {
            get
            {
                return this.GetValue(GridCellBoundWrapper.CellBoundValueProperty);
            }
            set
            {
                this.SetValue(GridCellBoundWrapper.CellBoundValueProperty, value);
            }
        }

        /// <summary>
        /// Specifies the cell style information.
        /// </summary>
        public GridStyleInfo Style
        {
            get;
            set;
        }

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var boundObject = d as GridCellBoundWrapper;
            if (boundObject != null)
            {
                boundObject.RaiseOnValueChanged(args.NewValue);
            }
        }

        private void RaiseOnValueChanged(object value)
        {
            if (this.ValueChanged != null)
            {
                this.ValueChanged(this, new GridDataValueEventArgs<object>(value));
            }
        }

        /// <summary>
        /// Occurs when the cell bound value changes.
        /// </summary>
        public event EventHandler<GridDataValueEventArgs<object>> ValueChanged;
    }

    /// <summary>
    /// A cell model for the <see cref="GridCellDataTemplateRenderer"/>.
    /// </summary>
    public class GridCellDataBoundTemplateModel : GridCellModel<GridCellDataBoundTemplateRenderer>
    {
        /// <summary>
        /// Calculates the preferred size of the cell based on its content, including the cell template. 
        /// </summary>
        /// <param name="rowIndex">Cell row index.</param>
        /// <param name="colIndex">Cell column index.</param>
        /// <param name="style">Cell style information.</param>
        /// <param name="queryBounds">Graphical bounds.</param>
        /// <returns>The optimal size for the cell.</returns>
        public override Size CalculatePreferredCellSize(int rowIndex, int colIndex, GridStyleInfo style, GridQueryBounds queryBounds)
        {
            var model = this.GridModel;
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
                    else
                    {
                        var wrapperInstance = new GridCellBoundWrapper {CellBoundValue = style.CellValue, Style = style};
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

    /// <summary>
    /// A renderer that manages a DataTemplate specified with <see cref="GridRenderStyleInfo.CellTemplateKey"/>
    /// of the <see cref="GridRenderStyleInfo"/> inside cells. The <see cref="ContentControl.Content"/>
    /// will be the  <see cref="GridRenderStyleInfo.CellValue"/> which binds the DataTemplate to the value
    /// of the style.
    /// </summary>
    public class GridCellDataBoundTemplateRenderer : GridVirtualizingCellRenderer<GridCell>
    {
        /// <summary>
        /// Gets or sets a value that indicates whether to listen to the selector-changed event.
        /// </summary>
        public bool ListenToSelectorChanged
        {
            get;
            set;
        }

        /// <summary>
        /// When it is true, the children are arranged with desired size.
        /// </summary>
        public bool AllowInvalidateMeasureChildren
        {
            get;
            set;
        }

        public bool AllowRecycleWrapper { get; set; }

        /// <summary>
        /// Initializes a new <see cref="GridCellDataBoundTemplateRenderer"/>.
        /// </summary>
        public GridCellDataBoundTemplateRenderer()
        {
            this.IsFocusable = true;
            this.AllowRecycle = true;
            this.ListenToSelectorChanged = true;
            this.AllowInvalidateMeasureChildren = false;
            this.AllowRecycleWrapper = false;
        }

        protected override void ArrangeUIElement(ArrangeCellArgs aca, GridCell uiElement, GridRenderStyleInfo style)
        {
            if (style.FlowDirection == FlowDirection.RightToLeft)
            {
                uiElement.FlowDirection = style.FlowDirection;
                double m11 = -1;
                double m22 = 1;
                double offsetX = uiElement.Width;
                double offsetY = 0;
                uiElement.LayoutTransform = new MatrixTransform(m11, 0, 0, m22, offsetX, offsetY); //make sure this does not leak...
            }
            else
            {
                uiElement.LayoutTransform = MatrixTransform.Identity;
            }

            if (style.HasErrorInfo)
            {
                var margins = style.TextMargins.ToThickness();
                margins = style.ErrorInfo.AdjustErrorInfoMarginOnEditing(margins, style.GridControl, style.CellRowColumnIndex);
                uiElement.Padding = margins;
            }

            base.ArrangeUIElement(aca, uiElement, style);
        }

        bool pending = false;
        void ScrollOwner_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (pending || Mouse.LeftButton != MouseButtonState.Pressed)
                return;
            pending = true;

            this.GridControl.Dispatcher.BeginInvoke(new Action(() =>
            {
                GridControl.InvalidateVisual(true);
                pending = false;
            }), System.Windows.Threading.DispatcherPriority.ApplicationIdle);
        }

        protected override void OnEditingComplete()
        {
            base.OnEditingComplete();
        }
        protected override void OnEnteredEditMode()
        {
            base.OnEnteredEditMode();
            if (CurrentCellUIElement != null)
            {
                // GridRenderStyleInfo style = CurrentStyle; Unused local variable
                var value = GetControlValueFromEditor();
                this.ControlValue = value;
            }
            GridControl.InvalidateCell(CellRowColumnIndex);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        protected override void OnResetFocus()
        {
            base.OnResetFocus();
            if (this.CurrentCell.IsEditing)
            {
                this.CurrentCellUIElement.Focus();
            }
        }

        protected override void OnSetFocus()
        {
            base.OnSetFocus();
            if (this.CurrentCell != null && this.HasCurrentCellState && this.CurrentCellUIElement != null)
            {
                this.CurrentCellUIElement.Focus();
            }
        }

        /// <summary>
        /// Initializes the content of the data bound template cell
        /// using the information from the cell style (value, text,
        /// behavior etc.).
        /// </summary>
        /// <param name="uiElement">The cell UI element.</param>
        /// <param name="style">The cell style info.</param>
        public override void OnInitializeContent(GridCell uiElement, GridRenderStyleInfo style)
        {
            base.OnInitializeContent(uiElement, style);

            bool found = false;
            DataTemplate dt = null;
            if (!string.IsNullOrEmpty(style.CellEditTemplateKey))
            {
                dt = (DataTemplate)style.GridControl.TryFindResource(style.CellEditTemplateKey);
                found = dt != null;
            }
            else if (style.CellEditTemplate != null)
            {
                dt = style.CellEditTemplate;
                found = true;
            }
            else if (style.CellItemTemplate != null)
            {
                dt = style.CellItemTemplate;
                found = true;
            }
            else if (!string.IsNullOrEmpty(style.CellItemTemplateKey))
            {
                dt = (DataTemplate)style.GridControl.TryFindResource(style.CellItemTemplateKey);
                found = dt != null;
            }

            if (found)
            {
                OnUnwireUIElement(uiElement);
                //uiElement.DataSource = null;
                var datagrid = this.GridControl.FindParentElementOfType<GridDataControl>();
                if (datagrid != null)
                {
                    var wrapperInstance = uiElement.DataContext as GridDataCellBoundWrapper;
                    if (wrapperInstance == null || AllowRecycleWrapper)
                    {
                        wrapperInstance = this.CreateDataWrapperInstance();
                    }
                    //wrapperInstance.CellBoundValue = null;
                    //wrapperInstance.Style = null;

                    // We have set CellBoundValue from ContolValue.
                    //Because after changing the value if we click on the same data template cell then CellBoundValue changed to CellValue.
                    //So here CellBoundValue set from controlValue.
                    if (HasCurrentCellState && this.ControlValue != wrapperInstance.CellBoundValue)
                    {
                        wrapperInstance.CellBoundValue = this.ControlValue;
                    }
                    wrapperInstance.Style = style;
                    this.ProvideDataWrapperInstance(wrapperInstance, style);
                    uiElement.DataSource = wrapperInstance;
                }
                else
                {
                    var wrapperInstance = uiElement.DataContext as GridCellBoundWrapper;
                    if (wrapperInstance == null || AllowRecycleWrapper)
                    {
                        wrapperInstance = this.CreateWrapperInstance();
                    }
                    // We have set CellBoundValue from ContolValue.
                    //Because after changing the value if we click on the same data template cell then CellBoundValue changed to CellValue.
                    //So here CellBoundValue set from controlValue.
                    if (HasCurrentCellState && this.ControlValue != wrapperInstance.CellBoundValue)
                    {
                        wrapperInstance.CellBoundValue = this.ControlValue;
                    }
                    wrapperInstance.Style = style;
                    this.ProvideWrapperInstance(wrapperInstance, style);
                    uiElement.SetValue(ContentControl.DataContextProperty, wrapperInstance);
                    //uiElement.DataContext = wrapperInstance;
                }
                uiElement.ContentTemplate = dt;
                //GridControlBase.SetDelayLoad(uiElement, false);
                this.OnWireUIElement(uiElement);
            }
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            this.ControlValue = this.GetControlValueFromEditor();
        }

        public override void CreateRendererElement(GridCell uiElement, GridRenderStyleInfo style)
        {
            base.CreateRendererElement(uiElement, style);

            var found = false;
            DataTemplate dt = null;
            if (!string.IsNullOrEmpty(style.CellItemTemplateKey))
            {
                //dt = GridTemplateSelector.GetTemplate(style);
                dt = (DataTemplate)style.GridControl.TryFindResource(style.CellItemTemplateKey);
                found = dt != null;
            }
            else if (style.CellItemTemplate != null)
            {
                dt = style.CellItemTemplate;
                found = true;
            }
            if (found)
            {
                this.OnUnwireUIElement(uiElement);
                //uiElement.DataSource = null;
                //uiElement.DataContext = null;
                uiElement.ContentTemplate = dt;
                uiElement.CellRowColumnIndex = style.CellRowColumnIndex;
                var datagrid = this.GridControl.FindParentElementOfType<GridDataControl>();
                if (datagrid != null)
                {
                    var wrapperInstance = uiElement.DataSource as GridDataCellBoundWrapper;
                    if (wrapperInstance == null || AllowRecycleWrapper)
                    {
                        wrapperInstance = this.CreateDataWrapperInstance();
                    }
                    wrapperInstance.CellBoundValue = null;
                    wrapperInstance.Style = null;
                    wrapperInstance.CellBoundValue = style.CellValue;
                    wrapperInstance.Style = style;
                    this.ProvideDataWrapperInstance(wrapperInstance, style);
                    uiElement.DataSource = wrapperInstance;
                }
                else
                {
                    var wrapperInstance = uiElement.DataContext as GridCellBoundWrapper;
                    if (wrapperInstance == null || AllowRecycleWrapper)
                    {
                        wrapperInstance = this.CreateWrapperInstance();
                    }
                    wrapperInstance.CellBoundValue = style.CellValue;
                    wrapperInstance.Style = style;
                    this.ProvideWrapperInstance(wrapperInstance, style);
                    uiElement.SetValue(ContentControl.DataContextProperty, wrapperInstance);
                    //uiElement.DataSource = wrapperInstance;
                }
                //uiElement.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                //GridControlBase.SetDelayLoad(uiElement, false);
                this.OnWireUIElement(uiElement);
            }
        }

        public override void RaiseGridCellClick(int rowIndex, int colIndex, MouseControllerEventArgs e)
        {
            base.RaiseGridCellClick(rowIndex, colIndex, e);
        }

        public virtual GridCellBoundWrapper CreateWrapperInstance()
        {
            return new GridCellBoundWrapper();
        }

        public virtual GridDataCellBoundWrapper CreateDataWrapperInstance()
        {
            return new GridDataCellBoundWrapper();
        }

        protected GridDataTableModel TableModel
        {
            get
            {
                var tableModel = this.GridControl.Model as GridDataTableModel;
                return tableModel;
            }
        }

        public void ProvideDataWrapperInstance(GridDataCellBoundWrapper wrapperInstance, GridRenderStyleInfo style)
        {
            if (style.ModelStyle is GridDataStyleInfo)
            {
                var styleInfo = style.ModelStyle as GridDataStyleInfo;
                var recordIdx = -1;
                if (styleInfo.CellIdentity.TableCellType != GridDataTableCellType.DetailsViewCell)
                    recordIdx = styleInfo.CellIdentity.RecordIndex;

                if (recordIdx > -1 && styleInfo.CellIdentity.TableCellType != GridDataTableCellType.AddNewRecordCell && styleInfo.CellIdentity.TableCellType != GridDataTableCellType.DetailsViewCell)
                {
                    if (!this.TableModel.Table.HasGroups)
                    {
                        if (recordIdx < this.TableModel.View.Records.Count)
                        {
                            wrapperInstance.Record = this.TableModel.View.Records[recordIdx];
                        }
                    }
                    else
                    {
                        if (this.TableModel.View.TopLevelGroup.DisplayElements.Count >= recordIdx)
                        {
                            var recordEntry = this.TableModel.View.TopLevelGroup.DisplayElements[recordIdx] as RecordEntry;
                            if (recordEntry != null)
                            {
                                wrapperInstance.Record = recordEntry;
                            }
                        }
                    }
                }
                else if (styleInfo.CellIdentity.TableCellType == GridDataTableCellType.AddNewRecordCell)
                {
                    var datagrid = this.GridControl.FindParentElementOfType<GridDataControl>();
                    if (datagrid != null && datagrid.Model.CurrencyManager.newItem != null)
                    {
                        wrapperInstance.Record = this.TableModel.View.Records.CreateRecordEntry(datagrid.Model.CurrencyManager.newItem);
                    }
                }
                else if (styleInfo.CellIdentity.TableCellType == GridDataTableCellType.DetailsViewCell)
                {
                   
                    if (!this.TableModel.Table.HasGroups)
                    {
                        recordIdx = this.TableModel.ResolveIndexToRecordPosition(styleInfo.CellIdentity.RowIndex - 1);
                        if (recordIdx < this.TableModel.View.Records.Count)
                        {
                            wrapperInstance.Record = this.TableModel.View.Records[recordIdx];
                        }
                    }
                    else
                    {
                        recordIdx = this.TableModel.ResolveIndexToGroupPosition(styleInfo.CellIdentity.RowIndex - 1);
                        if (this.TableModel.View.TopLevelGroup.DisplayElements.Count >= recordIdx)
                        {
                            var recordEntry = this.TableModel.View.TopLevelGroup.DisplayElements[recordIdx] as RecordEntry;
                            if (recordEntry != null)
                            {
                                wrapperInstance.Record = recordEntry;
                            }
                        }
                    }
                }
            }
        }

        public virtual void ProvideWrapperInstance(GridCellBoundWrapper wrapperInstance, GridRenderStyleInfo style)
        {
        }

        protected override void OnWireUIElement(GridCell uiElement)
        {
            base.OnWireUIElement(uiElement);
            if (this.HasCurrentCellState)
            {
                if (this.CurrentCellUIElement != null && this.CurrentCellUIElement.DataContext != null)
                {
                    var wrapperInstance = CurrentCellUIElement.DataContext as GridCellBoundWrapper;
                    if (wrapperInstance != null)
                    {
                        wrapperInstance.ValueChanged += wrapperInstance_ValueChanged;
                    }
                }
            }
        }

        protected override void OnUnwireUIElement(GridCell uiElement)
        {
            base.OnUnwireUIElement(uiElement);
            if (this.HasCurrentCellState)
            {
                if (this.CurrentCellUIElement != null && this.CurrentCellUIElement.DataContext != null)
                {
                    var wrapperInstance = CurrentCellUIElement.DataContext as GridCellBoundWrapper;
                    if (wrapperInstance != null)
                    {
                        wrapperInstance.ValueChanged -= wrapperInstance_ValueChanged;
                    }
                }
            }
            uiElement.DataSource = null;
        }

        private bool ProcessTabKey()
        {
            if (this.CurrentCellUIElement == null)
                return true;
            var control = ((Control)this.CurrentCellUIElement.FocusedElement);
            if (control != null && this.CurrentCellUIElement != null)
            {
                var allChildren = CurrentCellUIElement.FindElementsOfType<Control>();
                var nextControl = allChildren.FirstOrDefault(c => c != null && c.TabIndex == control.TabIndex + 1);
                if (nextControl != default(Control))
                {
                    return false;
                }
            }
            return true;
        }

        protected override void OnActivated()
        {
            //To set the current cell background, while pressing the left and right arrow key.
            GridControl.InvalidateCell(CellRowColumnIndex);
        }

        protected override void OnDeactivated()
        {
            //To remove the current cell background, while pressing the left and right arrow key.
            GridControl.InvalidateCell(CellRowColumnIndex);
            if (GridControl is GridControl)
                GridControl.InvalidateVisual(); // Temporarily this code added here to avoid loding problem of CellItemTemplate. 
            else
                GridControl.InvalidateVisual(false);
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
                    return this.ProcessTabKey();

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
                case Key.Enter:
                    {
                        if (this.CurrentStyle != null)
                        {
                            var renderer = this.CurrentCell.Renderer;
                            if (renderer != null)
                            {
                                var sif = renderer.CurrentStyle.ModelStyle as GridDataStyleInfo;
                                if (sif != null &&
                                    sif.CellIdentity.TableCellType != GridDataTableCellType.AddNewRecordCell)
                                {
                                    e.Handled = true;
                                }
                                else if (sif != null &&
                                         sif.CellIdentity.TableCellType == GridDataTableCellType.AddNewRecordCell &&
                                         !CurrentCell.IsEditing)
                                {
                                    e.Handled = true;
                                }
                            }
                        }
                        //if (this.CurrentCell.IsEditing)
                        //    CurrentCell.EndEdit();
                        CurrentCell.MoveRight();

                        return true;
                    }

                case Key.Back:
                    {
                        CurrentCell.BeginEdit(true);
                        return false;
                    }
            }

            return base.ShouldGridTryToHandlePreviewKeyDown(e);
        }

        public virtual void wrapperInstance_ValueChanged(object sender, GridDataValueEventArgs<object> e)
        {
            if (!this.IsInArrange && !this.CurrentCell.IsInEndEdit && HasCurrentCellState)
            {
                if (!this.SetControlValue(e.Value))
                {
                    RefreshContent();
                }
            }
        }

        protected override object GetControlValueFromEditor()
        {
            if (this.HasCurrentCellState && this.CurrentCellUIElement != null)
            {
                var wrapperInstance = ((GridCell)this.CurrentCellUIElement).DataContext as GridCellBoundWrapper; //uiElement.DataSource changed to uiElement.DataContext because WrapperInstance created for DataContext.
                if (wrapperInstance != null)
                {
                    return wrapperInstance.Style.CellValue;

                }
            }
            return base.GetControlValueFromEditor();
        }

        protected override object GetControlValueFromEditorCore(GridCell uiElement)
        {
            var wrapperInstance = uiElement.DataContext as GridCellBoundWrapper; //uiElement.DataSource changed to uiElement.DataContext because WrapperInstance created for DataContext.
            if (wrapperInstance != null)
            {                
                return wrapperInstance.CellBoundValue;
            }
            return base.GetControlValueFromEditorCore(uiElement);
        }

        protected override string GetControlTextFromEditorCore(GridCell uiElement)
        {
            return CurrentStyle.CellValue.ToString();
        }

        protected override void OnGridPreviewTextInput(TextCompositionEventArgs e)
        {

            if (CurrentCell.IsEditing)
            {
                RefreshContent();
                return;
            }
            CurrentCell.ScrollInView();
            CurrentCell.BeginEdit(true);
        }
    }
}
