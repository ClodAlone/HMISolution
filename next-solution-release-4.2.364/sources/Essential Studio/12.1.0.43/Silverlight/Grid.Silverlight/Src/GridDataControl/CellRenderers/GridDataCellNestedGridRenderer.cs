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
            // When grid view goes out of view, unwire the view from the underlying Model so
            // that it can be garbage collected if needed. Model will be set again when
            // OnInitializeContent is called when cell is scrolled into view.
            uiElement.Model = null;
            uiElement.CurrentCellDeactivated -= new GridCurrentCellDeactivatedEventHandler(uiElement_CurrentCellDeactivated);
            uiElement.SelectionChanged -= new GridSelectionChangedEventHandler(uiElement_SelectionChanged);
            base.OnUnwireUIElement(uiElement);
        }

        protected override void OnWireUIElement(GridDataCellNestedGridEditor uiElement)
        {
            base.OnWireUIElement(uiElement);
            uiElement.CurrentCellDeactivated += new GridCurrentCellDeactivatedEventHandler(uiElement_CurrentCellDeactivated);
            uiElement.SelectionChanged += new GridSelectionChangedEventHandler(uiElement_SelectionChanged);
        }

        void uiElement_CurrentCellDeactivated(object sender, GridCurrentCellDeactivatedEventArgs e)
        {
            //Here the current cell value was not displayed, so this was invalidated.
            this.GridControl.InvalidateCell(e.CellRowColumnIndex);
        }

        void uiElement_SelectionChanged(object sender, GridSelectionChangedEventArgs e)
        {
            //Here the drawn selection was not drawn, so it is invalidated.
            this.GridControl.InvalidateRenderCell(e.Range);
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
            VirtualizingCellsControl cellsControl = element as VirtualizingCellsControl;
            if (cellsControl != null)
            {
                cellsControl.MouseControllerDispatcher.CancelMode();
            }

            base.OnCancelMouseCapture(element);
        }

        protected override void OnRecaptureMouse(UIElement element)
        {
            VirtualizingCellsControl cellsControl = element as VirtualizingCellsControl;
            if (cellsControl != null)
            {
                cellsControl.MouseControllerDispatcher.RestoreMode();
            }

            base.OnRecaptureMouse(element);
        }

        protected override bool ShouldGridTryToHandlePreviewKeyDown(KeyEventArgs e)
        {
            if (this.CurrentCellUIElement == null || e.Key == Key.Enter)
            {
                return true;
            }

            // This does not work multiple-level nested grids! Only for the first level.
            if (!this.CurrentCellUIElement.CurrentCell.HasCurrentCell)//!this.CurrentCellUIElement.IsKeyboardFocusWithin)
            {
                GridRangeInfo r = CurrentCellUIElement.NavigateWithArrowKeysCellsRange;
                int lineSize = 0;
                bool isHidden = CurrentCellUIElement.Model.RowHeights.GetHidden(r.Bottom, out lineSize);
                if (e.Key == Key.Down)
                {
                    CurrentCellUIElement.CurrentCell.CellRowColumnIndex = new RowColumnIndex(r.Top, r.Left);
                    CurrentCell.BeginEdit();
                    e.Handled = true;
                    return false;
                }

                if (e.Key == Key.Up)
                {
                    if (isHidden)
                        CurrentCellUIElement.CurrentCell.CellRowColumnIndex = new RowColumnIndex(CurrentCellUIElement.ScrollRows.GetPreviousScrollLineIndex(r.Bottom), r.Left);
                    else
                        CurrentCellUIElement.CurrentCell.CellRowColumnIndex = new RowColumnIndex(r.Bottom, r.Left);
                    CurrentCell.BeginEdit();
                    e.Handled = true;
                    return false;
                }

                if (e.Key == Key.Left)
                {
                    if (isHidden)
                        CurrentCellUIElement.CurrentCell.CellRowColumnIndex = new RowColumnIndex(CurrentCellUIElement.ScrollRows.GetPreviousScrollLineIndex(r.Bottom), r.Right);
                    else
                        CurrentCellUIElement.CurrentCell.CellRowColumnIndex = new RowColumnIndex(r.Bottom, r.Right);
                    CurrentCell.BeginEdit();
                    e.Handled = true;
                    return false;
                }

                if (e.Key == Key.Right)
                {
                    CurrentCellUIElement.CurrentCell.CellRowColumnIndex = new RowColumnIndex(r.Top, r.Left);
                    CurrentCell.BeginEdit();
                    e.Handled = true;
                    return false;
                }
                if (e.Key == Key.Tab)
                {
                    if (Keyboard.Modifiers == ModifierKeys.Shift)
                    {
                        if (isHidden)
                            CurrentCellUIElement.CurrentCell.CellRowColumnIndex = new RowColumnIndex(CurrentCellUIElement.ScrollRows.GetPreviousScrollLineIndex(r.Bottom), r.Right);
                        else
                            CurrentCellUIElement.CurrentCell.CellRowColumnIndex = new RowColumnIndex(r.Bottom, r.Right);
                    }
                    else
                        CurrentCellUIElement.CurrentCell.CellRowColumnIndex = new RowColumnIndex(r.Top, r.Left);
                    CurrentCell.BeginEdit();
                    e.Handled = true;
                    return false;
                }
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
            int columnIndex = CellRowColumnIndex.ColumnIndex;
            CoveredCellInfo cc = GridControl.GetCoveredCell(CellRowColumnIndex);
            GridControlBase g = CurrentCellUIElement;

            if (this.rowLayout == GridNestedAxisLayout.Nested)
            {
                GridModel m = CurrentStyle.CellValue as GridModel;
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

        protected override void OnEditingComplete()
        {
            if (this.CurrentCellUIElement != null)
            {
                this.CurrentCellUIElement.CurrentCell.Deactivate();
                this.CurrentCellUIElement.Model.Selections.Clear();
                this.GridControl.InvalidateVisual(true);
            }
        }

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
        internal object SelectedItem { get; set; }
        internal object OldSelectedItem { get; set; }

        public GridDataCellNestedGridEditor()
        {
            this.InitSelectsCellsMouseController();
        }

        #region SelectedItem and SelectedItems Implementation

        private bool resetSelectedItems = false;
        private void ResetSelectedItems()
        {
            resetSelectedItems = true;

            List<object> removedItems = new List<object>();

            foreach (var rec in this.TableModel.ChildGridSelectedItems)
            {
                removedItems.Add(rec);
            }

            this.TableModel.ChildGridSelectedItems.Clear();
            Dictionary<int, object> dictionary = new Dictionary<int, object>();
            GridDataChildTableModel tableModel = this.TableModel;
            GridRangeInfoList rangeList = this.Model.SelectedRanges;
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
                    if (this.TableModel.Options.ListBoxSelectionMode != GridSelectionMode.MultiExtended && this.TableModel.Options.ListBoxSelectionMode != GridSelectionMode.MultiSimple)
                    {
                        var selectRanges = this.Model.SelectedRanges.Clone();
                        this.Model.SelectedRanges.Clear();
                        foreach (GridRangeInfo _range in selectRanges)
                            this.Model.InvalidateCell(_range);
                    }
                    this.UpdateSelectionModels(new GridSelectionChangedEventArgs(GridRangeInfo.Empty, GridRangeInfoList.Empty, GridSelectionReason.Clear));
                    int recordIndex = this.TableModel.View.Records.IndexOfRecord(e.NewItems[0]);
                    int rowIndex = this.TableModel.ResolvePositionToIndex(recordIndex);
                    int x = this.TableModel.TableProperties.ShowRowHeader == true ? 1 : 0;
                    GridRangeInfo range = new GridRangeInfo(rowIndex, x, rowIndex, this.Model.ColumnCount - 1);
                    if (!this.Model.SelectedRanges.Contains(range))
                    {
                        this.Model.SelectedRanges.Add(range);
                        this.Model.InvalidateCell(range);
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    recordIndex = this.TableModel.View.Records.IndexOfRecord(e.OldItems[0]);
                    rowIndex = this.TableModel.ResolvePositionToIndex(recordIndex);
                    x = this.TableModel.TableProperties.ShowRowHeader == true ? 1 : 0;
                    range = new GridRangeInfo(rowIndex, x, rowIndex, this.Model.ColumnCount - 1);

                    if (this.Model.SelectedRanges.Contains(range))
                    {
                        this.Model.SelectedRanges.Remove(range);
                        this.Model.InvalidateCell(range);
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

        protected override void dispose()
        {
            this.TableModel.CurrencyManager.CurrentRecordSelectionChanged -= new GridDataCurrentRecordSelectionChangedEventHandler(CurrencyManager_CurrentRecordSelectionChanged);
            this.TableModel.ChildGridSelectedItems.CollectionChanged -= new System.Collections.Specialized.NotifyCollectionChangedEventHandler(ChildGridSelectedItems_CollectionChanged);
            this.Model.SelectionChanging -= new GridSelectionChangingEventHandler(model_SelectionChanging);
            this.Model.SelectionChanged -= new GridSelectionChangedEventHandler(model_SelectionChanged);
            this.SelectedItem = null;
            this.OldSelectedItem = null;
            base.dispose();
        }

        public override void InitializeNested(GridControlBase simpleGridControl, GridModel model, GridNestedAxisLayout rowLayout, GridNestedAxisLayout columnLayout)
        {
            base.InitializeNested(simpleGridControl, model, rowLayout, columnLayout);
            ((GridDataChildTableModel)model).Grid = this;
            var grid = ((GridDataChildTableModel)model).Grid;
            if (((GridDataChildTableModel)model).IsInSuspend)
                ((GridDataChildTableModel)model).ResumeEvents();
            this.TableModel.CurrencyManager.CurrentRecordSelectionChanged -= new GridDataCurrentRecordSelectionChangedEventHandler(CurrencyManager_CurrentRecordSelectionChanged);
            this.TableModel.CurrencyManager.CurrentRecordSelectionChanged += new GridDataCurrentRecordSelectionChangedEventHandler(CurrencyManager_CurrentRecordSelectionChanged);
            this.TableModel.ChildGridSelectedItems.CollectionChanged -= new System.Collections.Specialized.NotifyCollectionChangedEventHandler(ChildGridSelectedItems_CollectionChanged);
            this.TableModel.ChildGridSelectedItems.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(ChildGridSelectedItems_CollectionChanged);
            this.Model.SelectionChanging -= new GridSelectionChangingEventHandler(model_SelectionChanging);
            this.Model.SelectionChanging += new GridSelectionChangingEventHandler(model_SelectionChanging);
            this.Model.SelectionChanged -= new GridSelectionChangedEventHandler(model_SelectionChanged);
            this.Model.SelectionChanged += new GridSelectionChangedEventHandler(model_SelectionChanged);

            //// we need to invalidate the Grid, since it is not refreshing when we Sort and expand the records often
            if (grid != null)
            {
                grid.InvalidateCells();
            }

            //To show the error tool tip in Nested Grid
            GridTooltipService.SetShowErrorTooltips(this, this.TableModel.TableProperties.ShowErrorTooltips);
            GridTooltipService.SetShowTooltips(this, this.TableModel.TableProperties.ShowTooltips);
        }

        void CurrencyManager_CurrentRecordSelectionChanged(object sender, GridDataCurrentRecordSelectionChangedEventArgs args)
        {
            if (this.TableModel != null)
            {
                var view = this.TableModel.View;
                var record = args.NewIndex > -1 && args.NewIndex < this.TableModel.SourceListCount ? view.Records[args.NewIndex] : null;

                if (record != null)
                {
                    this.TableModel.ChildGridSelectedItem = ((RecordEntry)record).Data;
                    this.ResetSelectedItems();
                }
                else
                {
                    this.TableModel.ChildGridSelectedItem = null;
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
        //We have already set the currentcellbackgroung from RaisePrepareRenderCell() method, so no need this code.
        //internal override Brush GetCurrentCellBackground()
        //{
        //    var currentStyle = this.Model[this.CurrentCell.RowIndex, this.CurrentCell.ColumnIndex];
        //    if (!this.TableModel.TableProperties.IsLegacyStyleEnabled)
        //    {
        //        if ((Model.Options.DrawSelectionOptions & (GridDrawSelectionOptions.ReplaceBackground | GridDrawSelectionOptions.ReplaceTextColor | GridDrawSelectionOptions.AlphaBlend)) != 0)
        //        {
        //            if (this.CurrentCell.RowIndex >= Model.HeaderRows && this.CurrentCell.ColumnIndex >= Model.HeaderColumns)
        //            {
        //                if (Model.SelectedRanges.AnyRangeContains(GridRangeInfo.Cell(this.CurrentCell.RowIndex, this.CurrentCell.ColumnIndex)))
        //                {
        //                    if ((Model.Options.DrawSelectionOptions & GridDrawSelectionOptions.ReplaceBackground) != 0)
        //                    {
        //                        if (((currentStyle as GridDataStyleInfo).CellIdentity.TableCellType == GridDataTableCellType.GroupCaptionCell ||
        //                            (currentStyle as GridDataStyleInfo).CellIdentity.TableCellType == GridDataTableCellType.GroupCaptionPlusMinusCell))
        //                            return this.TableModel.GetGroupCaptionSelectionBackground();
        //                        else if (CurrentCell.RowIndex == this.CurrentCell.RowIndex && CurrentCell.ColumnIndex == this.CurrentCell.ColumnIndex)
        //                            return this.TableModel.GetCurrentCellSelectionBackground();
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    return base.GetCurrentCellBackground();
        //}
        
        //This is added to change the current cell background as white and foreground as black when the cell is in edit mode.
        internal override void RaisePrepareRenderCell(GridPrepareRenderCellEventArgs e)
        {
            base.RaisePrepareRenderCell(e);

            var currentStyle = this.CurrentCell.Grid.Model[e.Cell.RowIndex, e.Cell.ColumnIndex];

                if (!e.Style.IsEmpty && CurrentCell.RowIndex == e.Style.RowIndex && CurrentCell.ColumnIndex == e.Style.ColumnIndex &&
                                        (currentStyle as GridDataStyleInfo).CellIdentity.TableCellType != GridDataTableCellType.FilterBarCell
                                        && Model.Options.ShowCurrentCell)
                {
                    if (this.CurrentCell.IsEditing)
                    {
                        e.Style.Background = Brushes.White;
                    }
                    else if(this.CurrentCell.HasCurrentCell)
                        e.Style.Background = this.TableModel.GetCurrentCellSelectionBackground();
                }

                if (!e.Style.IsEmpty && CurrentCell.RowIndex == e.Style.RowIndex && CurrentCell.ColumnIndex == e.Style.ColumnIndex &&
                                        (currentStyle as GridDataStyleInfo).CellIdentity.TableCellType != GridDataTableCellType.FilterBarCell
                                        && Model.Options.ShowCurrentCell)
                {
                    if (this.CurrentCell.IsEditing)
                    {
                        e.Style.Foreground = Brushes.Black;
                    }
                    else if(this.CurrentCell.HasCurrentCell)
                        e.Style.Foreground = this.TableModel.GetCurrentCellSelectionForeground();
                }
        }

        private void InitSelectsCellsMouseController()
        {
            var selectController = this.MouseControllerDispatcher.Find("SelectCellsMouseController") as GridSelectCellsMouseController;
            if (selectController != null)
            {
                selectController.AdjustedRangeFunc = (type, r, c, selectedRange) =>
                {
                    //if (this.ParentGrid != null && ((GridDataChildTableModel)this.ParentGrid.Model) != null && ((GridDataChildTableModel)this.ParentGrid.Model).ChildTableModelCollection != null && ((GridDataTableProperties)((GridDataChildTableModel)this.ParentGrid.Model).TableProperties).ClearMultiSelectionInNestedGrid)
                    //{
                    //    if (((GridDataChildTableModel)this.ParentGrid.Model).SelectedRanges.Count >= 1)
                    //    {
                    //        var rowcol = this.ParentGrid.CurrentCell.CellRowColumnIndex;                            
                    //        ((GridDataChildTableModel)this.ParentGrid.Model).SelectedRanges.Clear();
                    //        //this.ParentGrid.InvalidateCell(GridRangeInfo.Cell(rowcol.RowIndex, rowcol.ColumnIndex));
                    //        this.ParentGrid.InvalidateVisual();
                    //    }                        
                    //    foreach (var models in ((GridDataChildTableModel)this.ParentGrid.Model).ChildTableModelCollection)
                    //    {
                    //        if (models.SelectedRanges.Count >= 1)
                    //        {
                                
                    //            models.SelectedCells = GridRangeInfo.Empty;
                    //            models.SelectedRanges.Clear();
                    //            models.Selections.Clear();
                    //            models.RefreshDisplay(true);
                    //        }
                    //    }
                    //}
                    var style = this.Model[r, c] as GridDataStyleInfo;
                    if (style == null) return GridRangeInfo.Empty;
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
                            if (tableStyleIdentity.TableCellType == GridDataTableCellType.RecordCell || tableStyleIdentity.TableCellType == GridDataTableCellType.AddNewRecordCell || tableStyleIdentity.TableCellType == GridDataTableCellType.UnboundColumnCell || tableStyleIdentity.TableCellType == GridDataTableCellType.RowHeaderCell||tableStyleIdentity.TableCellType==GridDataTableCellType.RecordPlusMinusCell)
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
            //// we are handling the Key.Enter && Key.Escape in the CurrentRecordManager
            if (!isInAddNewRow && e.Key != Key.Enter)
            {
                return base.MoveCurrentCellWithArrowKey(e);
            }
            else if (isInAddNewRow && e.Key != Key.Enter && e.Key != Key.Escape)
            {
                return base.MoveCurrentCellWithArrowKey(e);
            }

            return e.Handled;
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
                    this.TableModel.TableProperties.StackedHeaderRows.Count + Model.HeaderRows,
                    Model.HeaderColumns + leftColIdx,
                    RowHeights.LineCount - (RowHeights.FooterLineCount + 1),
                    ColumnWidths.LineCount - (ColumnWidths.FooterLineCount + 1)
                );
            }
        }

        protected override bool ShouldRenderCurrentCellBorder()
        {
            var currentCell = this.CurrentCell;
            if (currentCell.Renderer != null)
            {
                var style = currentCell.Renderer.CurrentStyle.ModelStyle as GridDataStyleInfo;
                if (style.CellIdentity.TableCellType == GridDataTableCellType.RecordCell || style.CellIdentity.TableCellType == GridDataTableCellType.AddNewRecordCell)
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

        protected override void OnCellClick(GridCellClickEventArgs e)
        {           

            base.OnCellClick(e);
        }

        protected override void OnResizingColumns(GridResizingColumnsEventArgs args)
        {
            base.OnResizingColumns(args);
            if (args.Handled)
            {
                return;
            }

            // handling args.Reason == HitTest, actually enables resizing for all cells in that column, so do not handle it here
            if (((args.Reason == GridResizeCellsReason.MouseMove) || (args.Reason == GridResizeCellsReason.MouseDown) || (args.Reason == GridResizeCellsReason.DoubleClick)) && !args.Columns.IsEmpty)
            {
                for (int i = args.Columns.Left; i <= args.Columns.Right; i++)
                {
                    var colIdx = this.TableModel.ResolvePositionToVisibleColumnIndex(i);
                    GridDataVisibleColumn visibleCol = colIdx > -1 && colIdx < this.TableModel.TableProperties.VisibleColumns.Count ? this.TableModel.TableProperties.VisibleColumns[colIdx] : null;
                    if (visibleCol != null)
                    {
                        args.AllowResize = visibleCol.AllowResize;
                        if (args.Reason == GridResizeCellsReason.MouseMove)
                        {
                            var deltaColumnIndex = this.ParentGrid.Model.ColumnCount - 1;
                            if ((args.Width - visibleCol.ActualWidth) >= 0)
                            {
                                this.ParentGrid.ScrollColumns.SetLineResize(deltaColumnIndex, args.Width - visibleCol.ActualWidth);
                            }
                        }
                        if (args.Reason == GridResizeCellsReason.MouseUp)
                        {
                            this.TableModel.TableProperties.SuspendEvents();
                            visibleCol.ActualWidth = args.Width;
                            this.TableModel.TableProperties.ResumeEvents();
                        }
                    }
                }
            }
        }

        #region Record Selection Change

        void model_SelectionChanging(object sender, GridSelectionChangingEventArgs e)
        {
            if (this.TableModel != null && (e.Reason == GridSelectionReason.MouseDown || e.Reason == GridSelectionReason.ArrowKey || e.Reason == GridSelectionReason.Clear)) // || e.Reason == GridSelectionReason.SetCurrentCell)
            {
                RowColumnIndex rowColIdx = new RowColumnIndex(e.ClickRange.Bottom, e.ClickRange.Left);
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
            RowColumnIndex newRowColIndex = new RowColumnIndex(e.ClickRange.Bottom, e.ClickRange.Right);
            var view = this.TableModel.View;
            int newRecordIndex = e.ClickRange.Bottom;

            var recIndex = this.TableModel.ResolveIndexToRecordPosition(newRecordIndex);
            var newSelectedItem = this.TableModel.View.Records.Count > 0 ? this.TableModel.Table.GetRecordFromRow(newRecordIndex) : null;

            GridDataRecordSelectionChangingEventArgs args = new GridDataRecordSelectionChangingEventArgs()
            {
                OldItem = this.SelectedItem,
                OldIndex = this.TableModel.CurrencyManager.CurrentCell.CellRowColumnIndex,
                NewItem = view.CreateRecordEntry(newSelectedItem),
                NewIndex = newRowColIndex,
                OldRecordIndex = this.SelectedItem != null ? view.Records.IndexOfRecord((this.SelectedItem as Syncfusion.Windows.Data.RecordEntry).Data) : -1,
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
            if (sender is GridDataChildTableModel)
            {
                if(this.TableModel != null && !(this.TableModel.Equals(sender as GridDataChildTableModel)))
                {
                    return;
                }
            }
            isInModelSelectionChanged = true;
            if (!e.Range.IsEmpty && this.TableModel != null)
            {
                this.UpdateSelectionModels(e);
                if ((e.Reason == GridSelectionReason.MouseUp && e.Range.Height > 0) || e.Reason == GridSelectionReason.ArrowKey || e.Reason == GridSelectionReason.SelectRange || e.Reason == GridSelectionReason.Clear)
                {                    
                    this.ResetSelectedItems();
                }
            }
            isInModelSelectionChanged = false;
        }
        #endregion
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
                nestedGrid.ClearChildGridSelections(nestedGrid.TableModel.SelectedChildModel);
            }
            var SelectedRanges = nestedGridModel.SelectedRanges.Clone();
            nestedGridModel.Selections.Clear();
            nestedGridModel.SelectedRanges.Clear();
            if (nestedGridModel.Grid.CurrentCell.IsEditing)
                nestedGridModel.Grid.CurrentCell.EndEdit();
            if (nestedGridModel.Grid.CurrentCell.HasCurrentCell)
                nestedGridModel.Grid.CurrentCell.Deactivate();
            nestedGridModel.ChildGridSelectedItem = null;
            if (nestedGridModel.ChildGridSelectedItems.Count > 0)
                nestedGridModel.ChildGridSelectedItems.Clear();
            foreach (GridRangeInfo range in SelectedRanges)
                nestedGridModel.Grid.InvalidateRenderCell(range);
        }
    }
}