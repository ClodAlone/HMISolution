#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Windows.Controls.Grid
{
    using System.Windows;
    using System.Linq;
    using System.Windows.Input;
    using Syncfusion.Windows.Controls.Cells;
    using Syncfusion.Windows.Controls.Scroll;
    using Syncfusion.Windows.Controls.Grid.Automation.Peers;
    using System.Windows.Media;
    using Syncfusion.Windows.GridCommon;
    using System.Collections.Generic;
    using System.Windows.Controls;
    using Syncfusion.Windows.Data;

    /// <summary>
    /// Cell Model for Grid Data Nested Cell.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class GridDataCellNestedGridModel : GridCellNestedGridModel
    {
        public GridDataCellNestedGridModel(GridNestedAxisLayout rowLayout, GridNestedAxisLayout columnLayout)
            : base(rowLayout, columnLayout)
        {
        }

        /// <summary>
        /// Creates the renderer.
        /// </summary>
        /// <returns></returns>
        public override IGridCellRenderer CreateRenderer()
        {
            IGridCellRenderer r = new GridDataCellNestedGridRenderer(this.RowLayout, this.ColumnLayout);
            r.RaiseCreated(this);
            return r;
        }
    }

    /// <summary>
    /// Provides implementation for NestedGrid renderer.
    /// </summary>
    public class GridDataCellNestedGridRenderer : GridVirtualizingCellRenderer<GridDataCellNestedGridEditor>
    {
        private GridNestedAxisLayout rowLayout;
        private GridNestedAxisLayout columnLayout;

        #region Ctor
        public GridDataCellNestedGridRenderer()
            : this(GridNestedAxisLayout.Nested, GridNestedAxisLayout.Normal)
        {
            this.IsEditable = false;
        }

        public GridDataCellNestedGridRenderer(GridNestedAxisLayout rowLayout, GridNestedAxisLayout columnLayout)
        {
            this.AllowRecycle = true;
            this.AllowRecycleIfIsKeyboardFocusWithin = true;
            this.SupportsRenderOptimization = false;
            this.rowLayout = rowLayout;
            this.columnLayout = columnLayout;

            this.IsControlTextShown = false;
            this.IsFocusable = true;
            this.IsModifiable = false;
            this.IsEditable = true;
        }
        #endregion

        #region Properties
        public bool ShareRowLayout
        {
            get
            {
                return this.rowLayout == GridNestedAxisLayout.Shared;
            }
        }

        public bool ShareColumnLayout
        {
            get
            {
                return this.columnLayout == GridNestedAxisLayout.Shared;
            }
        }

        public bool SingleColumnInParentLayout
        {
            get
            {
                return this.columnLayout == GridNestedAxisLayout.Nested;
            }
        }

        public bool SingleRowInParentLayout
        {
            get
            {
                return this.rowLayout == GridNestedAxisLayout.Nested;
            }
        }
        #endregion

        #region Overrides

        /// <summary>
        /// Called to initialize the content of the cell
        /// using the information from the cell style (value, text,
        /// behavior etc.). You must override this method in your
        /// derived class.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="style">The cell style info.</param>
        public override void OnInitializeContent(GridDataCellNestedGridEditor element, GridRenderStyleInfo style)
        {
            element.InitializeNested(style.GridControl, style.CellValue as GridModel, this.rowLayout, this.columnLayout);
        }

        public override void CreateRendererElement(GridDataCellNestedGridEditor element, GridRenderStyleInfo style)
        {
            element.InitializeNested(style.GridControl, style.CellValue as GridModel, this.rowLayout, this.columnLayout);
        }


        public override void EmptyRecycleBin()
        {
            foreach (var element in recycleBin)
            {
                var keu = element.Key;
                var grid = recycleBin.Dequeue(keu);
                while (grid != null)
                {
                    grid.Dispose();
                    grid = recycleBin.Dequeue(keu);

                }
            }
            base.EmptyRecycleBin();
        }

        /// <summary>
        /// Called when [render for printing].
        /// </summary>
        /// <param name="dc">The dc.</param>
        /// <param name="rca">The rca.</param>
        /// <param name="style">The style.</param>
        protected override void OnRenderForPrinting(System.Windows.Media.DrawingContext dc, RenderCellArgs rca, GridRenderStyleInfo style)
        {
            var NestedPrinting = new NestedGridPaint();
            NestedPrinting.DrawNestedGrid(dc, rca.CellRect, style, rowLayout, columnLayout);
            base.OnRenderForPrinting(dc, rca, style);
        }

        protected override string GetControlTextFromEditorCore(GridDataCellNestedGridEditor element)
        {
            return element.ToString();
        }

        protected override void OnUnwireUIElement(GridDataCellNestedGridEditor uiElement)
        {
            var tableModel = uiElement.TableModel;
            if (tableModel != null)
            {
                if (tableModel.Grid != null)
                {
                    tableModel.Grid.RenderStyles.Clear();
#if !SILVERLIGHT
                    tableModel.Grid.CellRenderers.EmptyRecycleBin();
#endif
                }

                tableModel.VolatileCellStyles.visibleRowIndexes.Remove(this);
                tableModel.VolatileCellStyles.visibleColumnIndexes.Remove(this);
                tableModel.VolatileCellStyles.Clear();
                tableModel.Data.Clear();
            }
            if (uiElement.TableModel != null)
                uiElement.TableModel.RaiseNestedGridUnLoaded(uiElement, null);
            // When grid view goes out of view, unwire the view from the underlying Model so
            // that it can be garbage collected if needed. Model will be set again when
            // OnInitializeContent is called when cell is scrolled into view.
            uiElement.Model = null;
            base.OnUnwireUIElement(uiElement);
        }

        protected override void OnWireUIElement(GridDataCellNestedGridEditor uiElement)
        {
            base.OnWireUIElement(uiElement);
        }

        protected override void OnPrepareUIElements(ArrangeCellArgs aca, System.Collections.Generic.List<UIElement> uiElements, ScrollControlChildFrame canvas, GridRenderStyleInfo cellInfo)
        {
            base.OnPrepareUIElements(aca, uiElements, canvas, cellInfo);
        }

        protected override void ArrangeUIElement(ArrangeCellArgs aca, GridDataCellNestedGridEditor uiElement, GridRenderStyleInfo style)
        {
            uiElement.ArrangeNested(null, aca, style);
            Rect clipRect = aca.SubtractBorderMargins(aca.CellClipRect, style.Padding.ToThickness());
            SetBounds(uiElement, clipRect, aca.ForceMeasure, true);
        }

        protected override void OnCancelMouseCapture(UIElement element)
        {
            var cellsControl = element as VirtualizingCellsControl;
            if (cellsControl != null)
            {
                cellsControl.MouseControllerDispatcher.CancelMode();
            }

            base.OnCancelMouseCapture(element);
        }

        protected override void OnRecaptureMouse(UIElement element)
        {
            var cellsControl = element as VirtualizingCellsControl;
            if (cellsControl != null)
            {
                cellsControl.MouseControllerDispatcher.RestoreMode();
            }

            base.OnRecaptureMouse(element);
        }

        private GridDataTableModel GetSelectedChildModel(GridDataControl dataGrid)
        {
            if (dataGrid.SelectedChildModel != null)
            {
                var model = dataGrid.SelectedChildModel;
                while (model.SelectedChildModel != null)
                    model = model.SelectedChildModel;
                return model;
            }
            return dataGrid.Model;
        }

        protected override bool ShouldGridTryToHandlePreviewKeyDown(KeyEventArgs e)
        {
            var dataGrid = this.CurrentCell.Grid.FindParentElementOfType<GridDataControl>();
            var selectedChildModel = this.GetSelectedChildModel(dataGrid);
            if (this.CurrentCellUIElement == null || (e.Key == Key.Enter && 
                !(selectedChildModel.IsEditing || selectedChildModel.Grid.CurrentCell.IsEditing)))
            {
                return true;
            }
            
            // This does not work multiple-level nested grids! Only for the first level.
            //if (!this.CurrentCellUIElement.IsKeyboardFocusWithin)
            var currentcell = CurrentCellUIElement.CurrentCell;
            if (!currentcell.HasCurrentCell && this.CurrentCellUIElement.IsKeyboardFocused)
            {
                //int lineSize = 0;
                //bool isrowHidden = CurrentCellUIElement.Model.RowHeights.GetHidden(r.Bottom, out lineSize);
                //bool iscolHidden = CurrentCellUIElement.Model.ColumnWidths.GetHidden(r.Right, out lineSize);
                GridRangeInfo r = CurrentCellUIElement.NavigateWithArrowKeysCellsRange;
                var top = r.Top;
                var bottom = r.Bottom;
                var left = r.Left;
                var right = r.Right;

                var cellrowcolIndex = RowColumnIndex.Empty;

                if (e.Key == Key.Down)
                {
                    //top -= 1;
                    if (currentcell.QueryNextEnabledCell(GridDirectionType.Down, ref top, ref left))
                        cellrowcolIndex = new RowColumnIndex(top, left);
                }
                else if (e.Key == Key.Up)
                {
                    //bottom += 1;
                    if (currentcell.QueryNextEnabledCell(GridDirectionType.Up, ref bottom, ref left))
                        cellrowcolIndex = new RowColumnIndex(bottom, left);
                }
                else if (e.Key == Key.Left)
                {
                    //right += 1;
                    if (currentcell.QueryNextEnabledCell(GridDirectionType.Left, ref bottom, ref right))
                        cellrowcolIndex = new RowColumnIndex(bottom, right);
                }
                else if (e.Key == Key.Right)
                {
                    //left -= 1;
                    if (currentcell.QueryNextEnabledCell(GridDirectionType.Right, ref top, ref left))
                        cellrowcolIndex = new RowColumnIndex(top, left);
                }
                else if (e.Key == Key.Tab)
                {
                    if (e.KeyboardDevice.Modifiers == ModifierKeys.Shift)
                    {
                        //if (iscolHidden)
                        //right += 1;
                        if (currentcell.QueryNextEnabledCell(GridDirectionType.Left, ref bottom, ref right))
                            cellrowcolIndex = new RowColumnIndex(bottom, right);
                    }
                    else
                    {
                        //if (iscolHidden)
                        //left -= 1;
                        if (currentcell.QueryNextEnabledCell(GridDirectionType.Right, ref top, ref left))
                            cellrowcolIndex = new RowColumnIndex(top, left);
                    }
                }

                if (!cellrowcolIndex.IsEmpty)
                {
                    var activateoption = new GridActivateCurrentCellOptions(GridSetCurrentCellOptions.ScrollInView)
                    {
                        IsExternalMove = true
                    };
                    currentcell.MoveTo(cellrowcolIndex, activateoption);
                    e.Handled = true;
                    return false;
                }
                //currentcell.CellRowColumnIndex = new RowColumnIndex(top, left);
                //CurrentCell.BeginEdit();
                //e.Handled = true;
                //return false;
            }

            return false;
        }

        // protected override void OnInitialize()
        // {
        // }
        protected override bool OnDeactivating()
        {
            // if (CurrentCellUIElement != null)
            // {
            //    CurrentCellUIElement.CurrentCell.Deactivate();
            //    CurrentCellUIElement.Model.Selections.Clear();
            // }
            return base.OnDeactivating();
        }

        protected override void OnDeactivated()
        {
        }

        protected override void ScrollInView()
        {
            int rowIndex = CellRowColumnIndex.RowIndex;
            // int columnIndex = CellRowColumnIndex.ColumnIndex; Unused local variable
            var cc = GridControl.GetCoveredCell(CellRowColumnIndex);
            // GridControlBase g = CurrentCellUIElement; Unused local variable

            if (this.rowLayout == GridNestedAxisLayout.Nested)
            {
                var m = CurrentStyle.CellValue as GridModel;
                PixelScrollAxis px = GridControl.ScrollRows as PixelScrollAxis;
                if (m == null || px == null || GridControl.IsRowVisible(rowIndex))
                {
                    return;
                }

                if (GridControl.TopRowIndex > rowIndex)
                {
                    double rowSize = m.RowHeights[m.RowCount - 1];
                    double scrollPos = px.Distances.GetCumulatedDistanceAt(cc.Bottom);
                    GridControl.VScrollBar.Value = scrollPos + GridControl.ScrollRows.GetLineSize(rowIndex) - rowSize;
                }
                else
                {
                    double rowSize = m.RowHeights[0];
                    double scrollPos = px.Distances.GetCumulatedDistanceAt(rowIndex);
                    GridControl.VScrollBar.Value = scrollPos + rowSize - GridControl.VScrollBar.LargeChange;
                }
            }
            else if (this.rowLayout == GridNestedAxisLayout.Normal)
            {
            }
        }

        public override void RefreshContent()
        {
        }

        //protected override void OnEditingComplete()
        //{
        //    if (this.CurrentCellUIElement != null)
        //    {
        //        this.CurrentCellUIElement.CurrentCell.Deactivate();
        //        this.CurrentCellUIElement.Model.Selections.Clear();
        //        this.GridControl.InvalidateVisual(true);
        //    }
        //}

        public override void RefreshCellUIElementsContent(VirtualizingCellsControl cellsControl, CellUIElements cellUIElements, RowColumnIndex rowColumnIndex)
        {
        }

        public override void Hide(UIElement e)
        {
            base.Hide(e);
        }

        #endregion
    }

    /// <summary>
    /// Derived Nested Grid editor for Grid Data Control.
    /// </summary>
    public class GridDataCellNestedGridEditor : GridCellNestedGridEditor
    {
        /// <summary>
        /// To maintain the selected record of the nested Grid, for internal use to pass this as argument in 
        /// record selection chagne event.
        /// </summary>
        internal object SelectedItem { get; set; }
        internal object OldSelectedItem { get; set; }

        public GridDataCellNestedGridEditor()
        {
            this.InitSelectsCellsMouseController();
            this.ClearVisualsCacheWhenUnloaded = true;
            this.Loaded += GridDataCellNestedGridEditor_Loaded;
            this.Unloaded += GridDataCellNestedGridEditor_Unloaded;
        }

        protected override void WireModel()
        {
            base.WireModel();
        }

        #region SelectedItem and SelectedItems Implementation

        private bool resetSelectedItems = false;
        private void ResetSelectedItems()
        {
            resetSelectedItems = true;
            var removedItems = new List<object>();

            foreach (var rec in this.TableModel.ChildGridSelectedItems)
            {
                removedItems.Add(rec);
            }

            this.TableModel.ChildGridSelectedItems.Clear();
            var dictionary = new Dictionary<int, object>();
            var tableModel = this.TableModel;
            // GridRangeInfoList rangeList = this.Model.SelectedRanges; Unused local variable
            foreach (GridRangeInfo range in this.Model.SelectedRanges)
            {
                GridRangeInfo internalrange = range;
                if (internalrange.RangeType == GridRangeInfoType.Table)
                    internalrange = range.ExpandRange(range.Top, range.Left, Model.RowCount, Model.ColumnCount);
                for (int index = internalrange.Top; index <= internalrange.Bottom; index++)
                {
                    int recordIndex = tableModel.ResolveIndexToRecordPosition(index);
                    object record = null;
                    if (this.TableModel.View != null && this.TableModel.View.Records != null)
                    {
                        if (recordIndex < this.TableModel.View.Records.Count)
                        {
                            if (recordIndex > -1)
                                record = this.TableModel.View.Records.GetItemAt(recordIndex);
                        }
                    }
                    if (record != null)
                    {
                        if (!dictionary.ContainsKey(recordIndex))
                        {
                            dictionary.Add(recordIndex, record);
                        }
                    }
                }
            }
            if (!this.TableModel.IsInFilter)
            {
                foreach (var record in dictionary.Values)
                {
                    TableModel.ChildGridSelectedItems.Add(record);
                }
                dictionary.Clear();
            }
            resetSelectedItems = false;

            #region  Raise the RaiseRecordsSelectionChanged event.

            List<object> addedItems = new List<object>();

            foreach (var rec in this.TableModel.ChildGridSelectedItems)
            {
                if (!removedItems.Contains(rec))
                    addedItems.Add(rec);
            }

            foreach (var rec in this.TableModel.ChildGridSelectedItems)
            {
                if (removedItems.Contains(rec))
                {
                    removedItems.Remove(rec);
                }
            }

            if (this.TableModel.ChildGridSelectedItems.Count == 0 && this.TableModel.Grid != null)
            {
                this.TableModel.ChildGridSelectedItem = null;
            }
            var r = this.TableModel.ChildGridSelectedItems.ToList();

            if (this.TableModel.IsInFilter && !this.TableModel.CurrencyManager.IsInFilterBarRow)
            {
                this.Model.SelectedRanges.Clear();
                this.TableModel.ChildGridSelectedItems.Clear();
                addedItems.Clear();
                removedItems.Clear();
                foreach (var record in r)
                {
                    if (this.TableModel.View.Contains(record))
                    {
                        this.TableModel.ChildGridSelectedItems.Add(record);
                        addedItems.Add(record);
                    }
                    else
                    {
                        removedItems.Add(record);
                    }
                }

                int itemCount = this.TableModel.ChildGridSelectedItems.Count;
                if (itemCount > 0)
                {
                    this.TableModel.ChildGridSelectedItem = this.TableModel.ChildGridSelectedItems[itemCount - 1];
                }
                else
                {
                    if (this.TableModel.ChildGridSelectedItem != null)
                        this.TableModel.ChildGridSelectedItem = null;
                }
            }
            if (this.TableModel.Options.ListBoxSelectionMode != GridSelectionMode.None && (removedItems.Count > 0 || addedItems.Count > 0))
                this.TableModel.Table.RaiseRecordsSelectionChanged(new GridDataRecordsSelectionChangedEventArgs(removedItems, addedItems));

            #endregion
        }
        void ChildGridSelectedItems_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (resetSelectedItems || this.TableModel == null)
            {
                return;
            }
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    int recordIndex = this.TableModel.View.Records.IndexOfRecord(e.NewItems[0]);
                    if (recordIndex == -1)
                        return;
                    int rowIndex = this.TableModel.ResolvePositionToIndex(recordIndex);
                    if (rowIndex >= 0)
                    {
                        var x = this.TableModel.TableProperties.ShowRowHeader == true ? 1 : 0;
                        var range = new GridRangeInfo(rowIndex, x, rowIndex, this.Model.ColumnCount - 1);
                        if (!this.Model.SelectedRanges.Contains(range))
                        {
                            this.Model.SelectedRanges.Add(range);
                            this.Model.InvalidateCell(range);
                        }
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    recordIndex = this.TableModel.View.Records.IndexOfRecord(e.OldItems[0]);
                    rowIndex = this.TableModel.ResolvePositionToIndex(recordIndex);
                    if (rowIndex > -1)
                    {
                        var range = GridRangeInfo.Row(rowIndex);
                        if (this.Model.SelectedRanges.Contains(range))
                        {
                            this.Model.SelectedRanges.Remove(range);
                            this.Model.InvalidateCell(range);
                        }
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    GridRangeInfoList rangeList = this.Model.SelectedRanges.Clone();
                    this.Model.SelectedRanges.Clear();
                    foreach (GridRangeInfo r in rangeList)
                    {
                        this.Model.InvalidateCell(r);
                    }
                    break;
            }
            this.Model.InvalidateVisual(true);
        }

        #endregion   

        public override void Dispose(bool disposing)
        {
            if (TableModel != null && this.TableModel.CurrencyManager != null)
            {
                this.TableModel.CurrencyManager.UnwireEvents();
                this.TableModel.CurrencyManager.CurrentRecordSelectionChanged -= CurrencyManager_CurrentRecordSelectionChanged;
                this.TableModel.ChildGridSelectedItems.CollectionChanged -= ChildGridSelectedItems_CollectionChanged;
            }
            this.Model.SelectionChanging -= model_SelectionChanging;
            this.Model.SelectionChanged -= model_SelectionChanged;
            this.Loaded -= GridDataCellNestedGridEditor_Loaded;
            this.Unloaded -= GridDataCellNestedGridEditor_Unloaded;
            this.SelectedItem = null;
            this.OldSelectedItem = null;

            base.Dispose(disposing);
        }

        protected override void UnwireModel()
        {
            if (this.TableModel != null && this.TableModel.CurrencyManager!=null)
            {
                this.TableModel.CurrencyManager.CurrentRecordSelectionChanged -= CurrencyManager_CurrentRecordSelectionChanged;
                this.TableModel.ChildGridSelectedItems.CollectionChanged -= ChildGridSelectedItems_CollectionChanged;
            }
            if (this.Model != null)
            {
                this.Model.SelectionChanging -= model_SelectionChanging;
                this.Model.SelectionChanged -= model_SelectionChanged;
            }
            base.UnwireModel();
        }

        public override void InitializeNested(GridControlBase simpleGridControl, GridModel model, GridNestedAxisLayout rowLayout, GridNestedAxisLayout columnLayout)
        {
            base.InitializeNested(simpleGridControl, model, rowLayout, columnLayout);
            ((GridDataChildTableModel)model).Grid = this;
            var grid = ((GridDataChildTableModel)model).Grid;
            var gdc = this.TableModel.Grid.FindParentElementOfType<GridDataControl>();
            grid.UseGuidelineSetToRenderBorder = gdc !=null && gdc.Model.Grid.UseGuidelineSetToRenderBorder;
            if (((GridDataChildTableModel)model).IsInSuspend)
                ((GridDataChildTableModel)model).ResumeEvents();

            this.TableModel.CurrencyManager.CurrentRecordSelectionChanged -= CurrencyManager_CurrentRecordSelectionChanged;
            this.TableModel.CurrencyManager.CurrentRecordSelectionChanged += CurrencyManager_CurrentRecordSelectionChanged;
            this.TableModel.ChildGridSelectedItems.CollectionChanged -= ChildGridSelectedItems_CollectionChanged;
            this.TableModel.ChildGridSelectedItems.CollectionChanged += ChildGridSelectedItems_CollectionChanged;
            this.Model.SelectionChanging -= model_SelectionChanging;
            this.Model.SelectionChanging += model_SelectionChanging;
            this.Model.SelectionChanged -= model_SelectionChanged;
            this.Model.SelectionChanged += model_SelectionChanged;

            //// we need to invalidate the Grid, since it is not refreshing when we Sort and expand the records often
            grid.InvalidateCells();

            //To show the error tool tip in Nested Grid
            GridTooltipService.SetShowErrorTooltips(this, this.TableModel.TableProperties.ShowErrorTooltips);
            GridTooltipService.SetShowTooltips(this, this.TableModel.TableProperties.ShowTooltips);

        }

        void GridDataCellNestedGridEditor_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.TableModel != null)
            {
                this.TableModel.RaiseNestedGridLoaded(sender, e);
            }
        }

        void GridDataCellNestedGridEditor_Unloaded(object sender, RoutedEventArgs e)
        {
            if (this.TableModel != null)
            {
                this.TableModel.RaiseNestedGridUnLoaded(sender, e);
            }
        }

        void CurrencyManager_CurrentRecordSelectionChanged(object sender, GridDataCurrentRecordSelectionChangedEventArgs args)
        {
            if (this.TableModel != null)
            {
                var view = this.TableModel.View;
                var record = args.NewIndex > -1 && args.NewIndex < this.TableModel.SourceListCount ? view.Records[args.NewIndex] : null;
                if (record != null)
                {
                    if (!this.TableModel.IsInSort)
                    {
                        this.TableModel.ChildGridSelectedItem = ((RecordEntry)record).Data;
                        this.ResetSelectedItems();
                    }
                }
                else
                {
                    this.TableModel.ChildGridSelectedItem = null;
                }
            }
        }

        //Set the NsetedGrid CurrentCell background and foreground.
        internal override void RaisePrepareRenderCell(GridPrepareRenderCellEventArgs e)
        {
            base.RaisePrepareRenderCell(e);
            var style = e.Style as GridRenderStyleInfo;
            var currentStyle = style.ModelStyle;
            if (currentStyle != null && this.TableModel != null)
            {
                if (this.TableModel.TableProperties.ShowHoveringBackground && !Model.SelectedRanges.AnyRangeContains(GridRangeInfo.Cell(e.Cell.RowIndex, e.Cell.ColumnIndex)))
                {
                    if (((currentStyle as GridDataStyleInfo).CellIdentity.TableCellType == GridDataTableCellType.RecordCell || (currentStyle as GridDataStyleInfo).CellIdentity.TableCellType == GridDataTableCellType.GroupCaptionCell))
                    {
                        if (rowSpanUnderMouse != null)
                        {
                            if (rowSpanUnderMouse.Contains(style.CellRowColumnIndex))
                            {
                                if ((currentStyle as GridDataStyleInfo).CellIdentity.TableCellType == GridDataTableCellType.RecordCell)
                                {
                                    e.Style.Background = this.TableModel.GridVisualStyle.HoveringRecordCellBackground;
                                    e.Style.Foreground = this.TableModel.GridVisualStyle.HoveringRecordCellForeground;
                                }
                                if ((currentStyle as GridDataStyleInfo).CellIdentity.TableCellType == GridDataTableCellType.GroupCaptionCell)
                                {
                                    e.Style.Background = this.TableModel.GridVisualStyle.HoveringGroupCaptionCellBackground;
                                }
                            }
                        }
                    }
                }
                if (Model.Options != null && (Model.Options.DrawSelectionOptions & (GridDrawSelectionOptions.ReplaceBackground | GridDrawSelectionOptions.ReplaceTextColor)) != 0)
                {
                    if (e.Cell.RowIndex >= Model.HeaderRows && e.Cell.ColumnIndex >= Model.HeaderColumns)
                    {
                        if (Model.SelectedRanges.AnyRangeContains(GridRangeInfo.Cell(e.Cell.RowIndex, e.Cell.ColumnIndex)))
                        {
                            var tableStyleIdentity = (currentStyle as GridDataStyleInfo).CellIdentity;
                            if ((Model.Options.DrawSelectionOptions & GridDrawSelectionOptions.ReplaceBackground) != 0)
                            {
                                if (CurrentCell.RowIndex == e.Style.RowIndex && CurrentCell.ColumnIndex == e.Style.ColumnIndex)
                                {
                                    if (tableStyleIdentity.TableCellType == GridDataTableCellType.FilterBarCell || tableStyleIdentity.TableCellType == GridDataTableCellType.DropDownFilterCell)
                                    { }
                                    else
                                    {
                                        if (this.CurrentCell.IsEditing && !this.TableModel.TableProperties.EnableVisualStyleForEditors)
                                            e.Style.Background = Brushes.White;
                                        else
                                            e.Style.Background = this.TableModel.GetCurrentCellSelectionBackground();
                                    }
                                }
                            }
                            if ((Model.Options.DrawSelectionOptions & GridDrawSelectionOptions.ReplaceTextColor) != 0)
                            {
                                if (CurrentCell.RowIndex == e.Style.RowIndex && CurrentCell.ColumnIndex == e.Style.ColumnIndex &&
                                    (currentStyle as GridDataStyleInfo).CellIdentity.TableCellType != GridDataTableCellType.FilterBarCell)
                                {
                                    if (this.CurrentCell.IsEditing && !this.TableModel.TableProperties.EnableVisualStyleForEditors)
                                        e.Style.Foreground = Brushes.Black;
                                    else
                                        e.Style.Foreground = this.TableModel.GetCurrentCellSelectionForeground();
                                }
                            }
                        }
                    }
                }
            }
        }

        public GridDataChildTableModel TableModel
        {
            get
            {
                return this.Model as GridDataChildTableModel;
            }
        }

        private void InitSelectsCellsMouseController()
        {
            var selectController = this.MouseControllerDispatcher.Find("SelectCellsMouseController") as GridSelectCellsMouseController;
            if (selectController != null)
            {
                selectController.AdjustedRangeFunc = (type, r, c, selectedRange) =>
                {
                    var style = this.Model[r, c] as GridDataStyleInfo;
                    var tableStyleIdentity = style.CellIdentity;
                    switch (type)
                    {
                        case GridSelectCellsMouseController.SelectionType.IsCells:
                            if (tableStyleIdentity.TableCellType != GridDataTableCellType.RecordCell && tableStyleIdentity.TableCellType != GridDataTableCellType.AddNewRecordCell && tableStyleIdentity.TableCellType != GridDataTableCellType.UnboundColumnCell && tableStyleIdentity.TableCellType != GridDataTableCellType.RowHeaderCell)
                            {
                                selectedRange = GridRangeInfo.Empty;
                            }
                            break;
                        case GridSelectCellsMouseController.SelectionType.IsRow:
                            if (tableStyleIdentity.TableCellType == GridDataTableCellType.RecordCell || tableStyleIdentity.TableCellType == GridDataTableCellType.AddNewRecordCell || tableStyleIdentity.TableCellType == GridDataTableCellType.UnboundColumnCell || tableStyleIdentity.TableCellType == GridDataTableCellType.RowHeaderCell)
                            {
                                var adjustValue = this.TableModel.Table.HasNestedTables ? 1 : 0;
                                var colOffset = this.TableModel.ResolveDefaultColumnOffset();
                                selectedRange = selectedRange.ExpandRange(0, colOffset, this.Model.RowCount, this.Model.ColumnCount - adjustValue);
                            }
                            else
                            {
                                selectedRange = GridRangeInfo.Empty;
                            }
                            break;
                    }
                    return selectedRange;
                };
            }
        }

        public override bool MoveCurrentCellWithArrowKey(System.Windows.Input.KeyEventArgs e)
        {
            if (this.TableModel == null)
            {
                return base.MoveCurrentCellWithArrowKey(e);
            }

            var isInAddNewRow = this.TableModel.CurrencyManager.IsInAddNewRow;

            if (isInAddNewRow)
            {
                bool isAddNewRowInBottom = (this.TableModel.TableProperties.AddNewRowPosition == Position.Bottom);
                bool isLastCol = CurrentCell.ColumnIndex == this.Model.ColumnCount - 1 - (this.TableModel.TableProperties.Relations.Count > 0 ? 1 : 0);
                if (isInAddNewRow && ((isLastCol && (e.Key == Key.Tab || e.Key == Key.Down)
                    && this.TableModel.View.Records.Count == 0) || (isLastCol && (e.Key == Key.Tab || e.Key == Key.Down)
                    && isAddNewRowInBottom) || (e.Key == Key.Enter || e.Key == Key.Return)))
                {
                    return e.Handled;
                }
            }

            return base.MoveCurrentCellWithArrowKey(e);
        }

        public override GridRangeInfo NavigateWithArrowKeysCellsRange
        {
            get
            {
                //adjust the first column CurrentCell movement while Key Navigation
                int leftColIdx = 0;
                if (this.Model is GridDataChildTableModel)
                {
                    leftColIdx += (this.Model as GridDataChildTableModel).Table.HasNestedTables && (this.Model as GridDataChildTableModel).TableProperties.ShowRecordPlusMinus ? 1 : 0;
                    leftColIdx += (this.Model as GridDataChildTableModel).Table.HasDetailsView ? 1 : 0;
                }
                return GridRangeInfo.Cells(
                    Model.HeaderRows + (this.TableModel != null ? this.TableModel.TableProperties.StackedHeaderRows.Count : 0),
                    Model.HeaderColumns + leftColIdx,
                    RowHeights.LineCount - (RowHeights.FooterLineCount + 1),
                    ColumnWidths.LineCount - (ColumnWidths.FooterLineCount + 1)
                );
            }
        }

        protected override bool ShouldRenderCurrentCellBorder()
        {
            var currentCell = this.CurrentCell;
            if ( currentCell != null &&  currentCell.Renderer != null)
            {
                var style = currentCell.Renderer.CurrentStyle.ModelStyle as GridDataStyleInfo;
                if ( style != null && (style.CellIdentity.TableCellType == GridDataTableCellType.RecordCell || style.CellIdentity.TableCellType == GridDataTableCellType.AddNewRecordCell))
                {
                    return base.ShouldRenderCurrentCellBorder();
                }

                return false;
            }

            return base.ShouldRenderCurrentCellBorder();
        }

        protected override void OnRaiseQueryAllowDragColumn(GridQueryDragColumnHeaderEventArgs args)
        {
            var colIdx = this.TableModel.ResolvePositionToVisibleColumnIndex(args.Column);
            GridDataVisibleColumn visibleCol = colIdx > -1 && colIdx < this.TableModel.TableProperties.VisibleColumns.Count ? this.TableModel.TableProperties.VisibleColumns[colIdx] : null;
            if (visibleCol != null)
            {
                args.AllowDrag = visibleCol.AllowDrag;
            }
            base.OnRaiseQueryAllowDragColumn(args);

            if (args.Handled)
            {
                return;
            }

            if (args.Reason == GridQueryDragColumnHeaderReason.HitTest || args.Reason == GridQueryDragColumnHeaderReason.MouseUp)
            {
                var colIndex = this.TableModel.ResolvePositionToVisibleColumnIndex(args.Column);
                if (colIndex < 0)
                {
                    args.AllowDrag = false;
                }
            }
            else if (args.Reason == GridQueryDragColumnHeaderReason.MouseMove)
            {
                var colIndex = this.TableModel.ResolvePositionToVisibleColumnIndex(args.InsertBeforeColumn);
                if (colIndex < 0)
                {
                    args.AllowDrag = false;
                }
            }
        }

        protected override void OnResizingColumns(GridResizingColumnsEventArgs args)
        {
            base.OnResizingColumns(args);
            if (args.Handled)
            {
                return;
            }
            // handling args.Reason == HitTest, actually enables resizing for all cells in that column, so do not handle it here
            if (args.Reason != GridResizeCellsReason.HitTest && !args.Columns.IsEmpty)
            {
                for (int i = args.Columns.Left;i <= args.Columns.Right;i++)
                {
                    var colIdx = this.TableModel.ResolvePositionToVisibleColumnIndex(i);
                    int hiddenCount = this.TableModel.HiddenColRanges.Count(v => v.Left < args.Columns.Left);
                    GridDataVisibleColumn visibleCol = colIdx > -1 && colIdx < this.TableModel.TableProperties.VisibleColumns.Count ? this.TableModel.TableProperties.VisibleColumns[colIdx] : null;
                    if (visibleCol != null)
                    {
                        args.AllowResize = visibleCol.AllowResize && !visibleCol.AutoFit;
                        if (args.AllowResize)
                        {
                            if (visibleCol.Width.UnitType == GridControlLengthUnitType.None || visibleCol.Width.UnitType == GridControlLengthUnitType.Star)
                            {
                                if (colIdx == hiddenCount && args.Width < 5)
                                {
                                    args.Width = 5;
                                }
                                if (args.Width < visibleCol.MinimumWidth)
                                    args.Width = visibleCol.MinimumWidth;
                                if ((args.Width > visibleCol.MaximumWidth) && (visibleCol.MaximumWidth != 0))
                                    args.Width = visibleCol.MaximumWidth;
                                if (args.Width == 0)
                                    visibleCol.IsHidden = true;
                            }
                            if (args.Reason == GridResizeCellsReason.MouseMove)
                            {
                                var deltaColumnIndex = this.ParentGrid.Model.ColumnCount - 2;
                                var extraWidth = 0.0;
                                if (this.ParentGrid.Model.ActiveGridView != null)
                                {
                                    for (int column = 0; column < this.ParentGrid.Model.ActiveGridView.NavigateWithArrowKeysCellsRange.Left; column++)
                                        extraWidth += this.ParentGrid.Model.ColumnWidths[column];
                                }
                                var TableModel = this.ParentGrid.Model as GridDataTableModel;
                                if (TableModel != null && TableModel.TableProperties.AllowNestedGridPadding)
                                    extraWidth += this.TableModel.RowHeights.PaddingDistance;
                                var deltaWidth = (this.TableModel.ColumnWidths.TotalExtent - (this.ParentGrid.Model.ColumnWidths.TotalExtent - extraWidth));
                                if (deltaWidth > 0)
                                {
                                    this.ParentGrid.Model.ColumnWidths[deltaColumnIndex] = this.ParentGrid.ScrollColumns.GetLineSize(deltaColumnIndex) + deltaWidth;
                                }
                            }

                            if (args.Reason == GridResizeCellsReason.MouseUp)
                            {
                                visibleCol.Width = new GridDataControlLength(args.Width);
#if SILVERLIGHT
                                args.Handled = true;
                                args.AllowResize = false;
#endif
                            }
                            else if (args.Reason == GridResizeCellsReason.DoubleClick)
                            {
                                this.TableModel.TableProperties.SuspendEvents();
                                var maxLength = this.Model.RowCount -1;
                                if (this.Model.Options.MaxLength > 0 && this.Model.Options.MaxLength <= this.Model.RowCount)
                                {
                                    maxLength = this.Model.Options.MaxLength;
                                }
                                var range = GridRangeInfo.Cells(0, i, maxLength, i);
                                this.TableModel.ResizeDataColumnsToFit(range, GridResizeToFitOptions.None);
                                this.TableModel.TableProperties.ResumeEvents();
                                args.AllowResize = false;
                            }
                        }
                    }
                }
            }
        }

        protected override System.Windows.Automation.Peers.AutomationPeer OnCreateAutomationPeer()
        {
            return new GridDataControlAutomationPeer(this);
        }

        #region Record Selection Change

        void model_SelectionChanging(object sender, GridSelectionChangingEventArgs e)
        {
            if (e.ClickRange == GridRangeInfo.Empty)
                return;
            if (this.TableModel != null && (e.Reason == GridSelectionReason.MouseDown || e.Reason == GridSelectionReason.ArrowKey || e.Reason == GridSelectionReason.Clear)) // || e.Reason == GridSelectionReason.SetCurrentCell)
            {
                var rowColIdx = new RowColumnIndex(e.ClickRange.Bottom, this.NavigateWithArrowKeysCellsRange.Left);
                var style = this.TableModel.Grid.GetRenderStyleInfo(rowColIdx).ModelStyle as GridDataStyleInfo;
                if (style.CellIdentity.TableCellType == GridDataTableCellType.RecordCell)
                {
                    GridDataRecordSelectionChangingEventArgs args = this.OnRaiseRecordSelectionChanging(sender, e);
                    if (args.Cancel)
                        e.Cancel = true;
                }
                (this.TableModel as GridDataChildTableModel).ParentRecord.Model.InvalidateVisual();
            }
        }

        internal GridDataRecordSelectionChangingEventArgs OnRaiseRecordSelectionChanging(object sender, GridSelectionChangingEventArgs e)
        {
            var newRowColIndex = new RowColumnIndex(e.ClickRange.Bottom, e.ClickRange.Right);
            var view = this.TableModel.View;
            int newRecordIndex = e.ClickRange.Bottom;

            var recIndex = this.TableModel.ResolveIndexToRecordPosition(newRecordIndex);
            var newSelectedItem = this.TableModel.View.Records.Count > 0 ? this.TableModel.Table.GetRecordFromRow(newRecordIndex) : null;

            var args = new GridDataRecordSelectionChangingEventArgs()
            {
                OldItem = this.SelectedItem,
                OldIndex = this.TableModel.CurrencyManager.CurrentCell.CellRowColumnIndex,
                NewItem = view.CreateRecordEntry(newSelectedItem),
                NewIndex = newRowColIndex,
                OldRecordIndex = this.SelectedItem != null ? view.Records.IndexOfRecord((this.SelectedItem as Syncfusion.Windows.Data.RecordEntry).Data): -1,
                NewRecordIndex = recIndex,
                Reason = e.Reason
            };

            this.TableModel.Table.RaiseRecordSelectionChanging(args);
            this.OldSelectedItem = args.OldItem;
            this.SelectedItem = args.NewItem;

            return args;
        }

        private bool isInModelSelectionChanged = false;
        void model_SelectionChanged(object sender, GridSelectionChangedEventArgs e)
        {
            isInModelSelectionChanged = true;
            if (!e.Range.IsEmpty && this.TableModel != null)
            {
                this.UpdateSelectionModels(e);
                if (((e.Reason == GridSelectionReason.MouseUp && e.Range.Height > 0) || e.Reason == GridSelectionReason.ArrowKey || e.Reason == GridSelectionReason.SelectRange || e.Reason == GridSelectionReason.Clear) && !this.TableModel.IsInSort)
                {
                    this.ResetSelectedItems();
                }

                if (e.Reason == GridSelectionReason.DeleteRow)
                {
                    this.Model.SelectedRanges.Clear();
                    this.Model.SelectedRanges.Add(e.Range);
                    this.ResetSelectedItems();
                }
            }
            isInModelSelectionChanged = false;
        }

        internal void UpdateSelectionModels(GridSelectionChangedEventArgs args)
        {
            var parentNestedGrid = this.ParentGrid as GridDataCellNestedGridEditor;
            if (parentNestedGrid != null && parentNestedGrid.TableModel != null)
            {
                if (parentNestedGrid.TableModel.SelectedChildModel == null)
                {
                    parentNestedGrid.TableModel.SelectedChildModel = this.TableModel;
                }
                else if (!(parentNestedGrid.TableModel.SelectedChildModel.Equals(this.TableModel)))
                {
                    if (args.Reason == GridSelectionReason.MouseDown && parentNestedGrid.TableModel.SelectedChildModel.Grid.CurrentCell.HasCurrentCell)
                    {
                        parentNestedGrid.TableModel.SelectedChildModel.Grid.CurrentCell.Deactivate();
                    }
                    parentNestedGrid.TableModel.SelectedChildModel = this.TableModel;
                }
                else if (this.isInModelSelectionChanged)
                {
                    if (this.TableModel.SelectedChildModel != null)
                    {
                        this.ClearChildGridSelections(this.TableModel.SelectedChildModel);
                        this.TableModel.SelectedChildModel = null;
                    }
                }
                if (parentNestedGrid is GridDataCellNestedGridEditor)
                {
                    parentNestedGrid.UpdateSelectionModels(args);
                }
            }
            else
            {
                var gdc = this.TableModel.Grid.FindParentElementOfType<GridDataControl>();
                if (gdc != null)
                {
                    if (gdc.SelectedChildModel == null)
                        gdc.SelectedChildModel = this.TableModel;
                    else if (!(gdc.SelectedChildModel.Equals(this.TableModel)))
                    {
                        if (args.Reason == GridSelectionReason.MouseDown && gdc.SelectedChildModel.Grid.CurrentCell.HasCurrentCell)
                            gdc.SelectedChildModel.Grid.CurrentCell.Deactivate();
                        gdc.SelectedChildModel = this.TableModel;
                    }
                    else if (this.isInModelSelectionChanged)
                    {
                        if (this.TableModel.SelectedChildModel != null)
                        {
                            this.ClearChildGridSelections(this.TableModel.SelectedChildModel);
                            this.TableModel.SelectedChildModel = null;
                        }
                    }
                }
            }
        }
      
        internal void ClearChildGridSelections(GridDataChildTableModel nestedGridModel)
        {
            var nestedGrid = nestedGridModel.Grid as GridDataCellNestedGridEditor;
            if (nestedGrid.TableModel != null && nestedGrid.TableModel.SelectedChildModel != null)
            {
                var cellRowColIndex = VirtualizingCellsControl.GetCellRowColumnIndex(nestedGrid);
                if (!cellRowColIndex.IsEmpty)
                {
                    nestedGridModel.InvalidateCell(cellRowColIndex);
                    nestedGridModel.InvalidateVisual();
                }
                nestedGrid.ClearChildGridSelections(nestedGrid.TableModel.SelectedChildModel);
            }
            if (!(nestedGridModel.Grid.CurrentCell.Renderer is GridCellCheckboxRenderer))
            {
                var SelectedRanges = nestedGridModel.SelectedRanges.Clone();
                nestedGridModel.Selections.Clear();
                nestedGridModel.SelectedRanges.Clear();
                if (nestedGridModel.Grid.CurrentCell.IsEditing)
                    nestedGridModel.Grid.CurrentCell.EndEdit();
                if (nestedGridModel.Grid.CurrentCell.HasCurrentCell)
                    nestedGridModel.Grid.CurrentCell.Deactivate();

                if (nestedGridModel.View != null)
                    nestedGridModel.View.MoveCurrentTo(null);

                nestedGridModel.ChildGridSelectedItem = null;
                if (nestedGridModel.ChildGridSelectedItems.Count > 0)
                    nestedGridModel.ChildGridSelectedItems.Clear();
                foreach (GridRangeInfo range in SelectedRanges)
                    nestedGridModel.Grid.InvalidateRenderCell(range);
            }
        }
        #endregion
    }

    public class NestedGridPaint
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public NestedGridPaint()
        {
        }

        /// <summary>
        /// Draws the nested grid.
        /// </summary>
        /// <param name="dc">The dc.</param>
        /// <param name="rc">The rc.</param>
        /// <param name="style">The style.</param>
        /// <param name="rowLayout">The row layout.</param>
        /// <param name="columnLayout">The column layout.</param>
        public void DrawNestedGrid(DrawingContext dc, Rect rc, GridRenderStyleInfo style, GridNestedAxisLayout rowLayout, GridNestedAxisLayout columnLayout)
        {
            VisualBrush vb = this.GetVisualBrush(rc.Size, style, rowLayout, columnLayout);
            Rect rect = rc;
            rect = new Rect(rc.X, rc.Y, rc.Width, rc.Height);
            dc.DrawRectangle(vb, null, rect);
        }

        /// <summary>
        /// Gets the visual brush.
        /// </summary>
        /// <param name="size">The size.</param>
        /// <param name="style">The style.</param>
        /// <param name="rowLayout">The row layout.</param>
        /// <param name="columnLayout">The column layout.</param>
        /// <returns></returns>
        private VisualBrush GetVisualBrush(Size size, GridRenderStyleInfo style, GridNestedAxisLayout rowLayout, GridNestedAxisLayout columnLayout)
        {

            VisualBrush visualBrush;
            GridDataCellNestedGridEditor b = new GridDataCellNestedGridEditor();
            b.InitializeNested(style.GridControl, style.CellValue as GridModel, rowLayout, columnLayout);
            visualBrush = new VisualBrush(b) { Stretch = Stretch.None, AlignmentX = AlignmentX.Left, AlignmentY = AlignmentY.Top };
            return visualBrush;
        }
    }

}