//-------------------------------------------------------------------------------------------------
// <copyright file="GridTableModelEvents.cs" company="syncfusion">
// Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;

namespace Syncfusion.Windows.Forms.Grid.Grouping.Controls
{
    //// CancelEventArgs
    //// CollectionChangeEventArgs
    //// EventArgs
    //// GraphicsEventArgs
    //// GridBanneredRangesChangedEventArgs
    //// GridBanneredRangesChangingEventArgs
    //// GridCellsChangedEventArgs
    //// GridCellsChangingEventArgs
    //// GridCellTextEventArgs
    //// GridClearingCellsEventArgs
    //// GridCountChangedEventArgs
    //// GridCountChangingEventArgs
    //// GridCoveredRangesChangedEventArgs
    //// GridCoveredRangesChangingEventArgs
    //// GridCutPasteEventArgs
    //// GridDefaultSizeChangedEventArgs
    //// GridDefaultSizeChangingEventArgs
    //// GridEndUpdateRequestEventArgs
    //// GridFloatingCellsChangedEventArgs
    //// GridInvalidateRangeRequestEventArgs
    //// GridMergeCellsChangedEventArgs
    //// GridPasteCellTextEventArgs
    //// GridPrepareChangeSelectionEventArgs
    //// GridQueryBanneredRangeEventArgs
    //// GridQueryCanMergeCellsEventArgs
    //// GridQueryCellInfoEventArgs
    //// GridQueryCellModelEventArgs
    //// GridQueryCoveredRangeEventArgs
    //// GridQueryOleDataSourceDataEventArgs
    //// GridRangeInsertedEventArgs
    //// GridRangeInsertingEventArgs
    //// GridRangeMovedEventArgs
    //// GridRangeMovingEventArgs
    //// GridRangeRemovedEventArgs
    //// GridRangeRemovingEventArgs
    //// GridRowColCountEventArgs
    //// GridRowColHiddenEventArgs
    //// GridRowColHideEventArgs
    //// GridRowColHidingEventArgs
    //// GridRowColSizeChangedEventArgs
    //// GridRowColSizeChangingEventArgs
    //// GridRowColSizeEventArgs
    //// GridRowColSizeTotalEventArgs
    //// GridSaveCellInfoEventArgs
    //// GridSelectionChangedEventArgs
    //// GridSelectionChangingEventArgs

    //// eva GridTableModelCancelEventArgs Cancel GridTableModel tableModel CancelEventArgs inner
    //// eva GridTableModelCollectionChangeEventArgs CollectionChange GridTableModel tableModel CollectionChangeEventArgs inner
    //// eva GridTableModelEventArgs  GridTableModel tableModel EventArgs inner
    //// eva GridTableModelGraphicsEventArgs Graphics GridTableModel tableModel GraphicsEventArgs inner
    //// eva GridTableModelBanneredRangesChangedEventArgs GridBanneredRangesChanged GridTableModel tableModel GridBanneredRangesChangedEventArgs inner
    //// eva GridTableModelBanneredRangesChangingEventArgs GridBanneredRangesChanging GridTableModel tableModel GridBanneredRangesChangingEventArgs inner
    //// eva GridTableModelCellsChangedEventArgs GridCellsChanged GridTableModel tableModel GridCellsChangedEventArgs inner
    //// eva GridTableModelCellsChangingEventArgs GridCellsChanging GridTableModel tableModel GridCellsChangingEventArgs inner
    //// eva GridTableModelCellTextEventArgs GridCellText GridTableModel tableModel GridCellTextEventArgs inner
    //// eva GridTableModelClearingCellsEventArgs GridClearingCells GridTableModel tableModel GridClearingCellsEventArgs inner
    //// eva GridTableModelCountChangedEventArgs GridCountChanged GridTableModel tableModel GridCountChangedEventArgs inner
    //// eva GridTableModelCountChangingEventArgs GridCountChanging GridTableModel tableModel GridCountChangingEventArgs inner
    //// eva GridTableModelCoveredRangesChangedEventArgs GridCoveredRangesChanged GridTableModel tableModel GridCoveredRangesChangedEventArgs inner
    //// eva GridTableModelCoveredRangesChangingEventArgs GridCoveredRangesChanging GridTableModel tableModel GridCoveredRangesChangingEventArgs inner
    //// eva GridTableModelCutPasteEventArgs GridCutPaste GridTableModel tableModel GridCutPasteEventArgs inner
    //// eva GridTableModelDefaultSizeChangedEventArgs GridDefaultSizeChanged GridTableModel tableModel GridDefaultSizeChangedEventArgs inner
    //// eva GridTableModelDefaultSizeChangingEventArgs GridDefaultSizeChanging GridTableModel tableModel GridDefaultSizeChangingEventArgs inner
    //// eva GridTableModelEndUpdateRequestEventArgs GridEndUpdateRequest GridTableModel tableModel GridEndUpdateRequestEventArgs inner
    //// eva GridTableModelFloatingCellsChangedEventArgs GridFloatingCellsChanged GridTableModel tableModel GridFloatingCellsChangedEventArgs inner
    //// eva GridTableModelInvalidateRangeRequestEventArgs GridInvalidateRangeRequest GridTableModel tableModel GridInvalidateRangeRequestEventArgs inner
    //// eva GridTableModelMergeCellsChangedEventArgs GridMergeCellsChanged GridTableModel tableModel GridMergeCellsChangedEventArgs inner
    //// eva GridTableModelPasteCellTextEventArgs GridPasteCellText GridTableModel tableModel GridPasteCellTextEventArgs inner
    //// eva GridTableModelPrepareChangeSelectionEventArgs GridPrepareChangeSelection GridTableModel tableModel GridPrepareChangeSelectionEventArgs inner
    //// eva GridTableModelQueryBanneredRangeEventArgs GridQueryBanneredRange GridTableModel tableModel GridQueryBanneredRangeEventArgs inner
    //// eva GridTableModelQueryCanMergeCellsEventArgs GridQueryCanMergeCells GridTableModel tableModel GridQueryCanMergeCellsEventArgs inner
    //// eva GridTableModelQueryCellInfoEventArgs GridQueryCellInfo GridTableModel tableModel GridQueryCellInfoEventArgs inner
    //// eva GridTableModelQueryCellModelEventArgs GridQueryCellModel GridTableModel tableModel GridQueryCellModelEventArgs inner
    //// eva GridTableModelQueryCoveredRangeEventArgs GridQueryCoveredRange GridTableModel tableModel GridQueryCoveredRangeEventArgs inner
    //// eva GridTableModelQueryOleDataSourceDataEventArgs GridQueryOleDataSourceData GridTableModel tableModel GridQueryOleDataSourceDataEventArgs inner
    //// eva GridTableModelRangeInsertedEventArgs GridRangeInserted GridTableModel tableModel GridRangeInsertedEventArgs inner
    //// eva GridTableModelRangeInsertingEventArgs GridRangeInserting GridTableModel tableModel GridRangeInsertingEventArgs inner
    //// eva GridTableModelRangeMovedEventArgs GridRangeMoved GridTableModel tableModel GridRangeMovedEventArgs inner
    //// eva GridTableModelRangeMovingEventArgs GridRangeMoving GridTableModel tableModel GridRangeMovingEventArgs inner
    //// eva GridTableModelRangeRemovedEventArgs GridRangeRemoved GridTableModel tableModel GridRangeRemovedEventArgs inner
    //// eva GridTableModelRangeRemovingEventArgs GridRangeRemoving GridTableModel tableModel GridRangeRemovingEventArgs inner
    //// eva GridTableModelRowColCountEventArgs GridRowColCount GridTableModel tableModel GridRowColCountEventArgs inner
    //// eva GridTableModelRowColHiddenEventArgs GridRowColHidden GridTableModel tableModel GridRowColHiddenEventArgs inner
    //// eva GridTableModelRowColHideEventArgs GridRowColHide GridTableModel tableModel GridRowColHideEventArgs inner
    //// eva GridTableModelRowColHidingEventArgs GridRowColHiding GridTableModel tableModel GridRowColHidingEventArgs inner
    //// eva GridTableModelRowColSizeChangedEventArgs GridRowColSizeChanged GridTableModel tableModel GridRowColSizeChangedEventArgs inner
    //// eva GridTableModelRowColSizeChangingEventArgs GridRowColSizeChanging GridTableModel tableModel GridRowColSizeChangingEventArgs inner
    //// eva GridTableModelRowColSizeEventArgs GridRowColSize GridTableModel tableModel GridRowColSizeEventArgs inner
    //// eva GridTableModelRowColSizeTotalEventArgs GridRowColSizeTotal GridTableModel tableModel GridRowColSizeTotalEventArgs inner
    //// eva GridTableModelSaveCellInfoEventArgs GridSaveCellInfo GridTableModel tableModel GridSaveCellInfoEventArgs inner
    //// eva GridTableModelSelectionChangedEventArgs GridSelectionChanged GridTableModel tableModel GridSelectionChangedEventArgs inner
    //// eva GridTableModelSelectionChangingEventArgs GridSelectionChanging GridTableModel tableModel GridSelectionChangingEventArgs inner

#if later
    internal class GridModelEventsTarget : IGridModelEventsTarget
    {
        GridTableModel tableModel;
        GridGroupingControl owner;

        public GridModelEventsTarget(GridTableModel tableModel, GridGroupingControl owner)
        {
            this.tableModel = tableModel;
            this.owner = owner;
        }
        
        #region IGridModelEventsTarget Members

        public void OnBaseStylesMapChanged(EventArgs e)
        {
            // Not Used.
        }

        public void OnQueryColCount(GridRowColCountEventArgs e)
        {
            // Not Used.
        }

        public void OnPrepareClearSelection(EventArgs e)
        {
            owner.OnPrepareClearSelection(new GridTableModelEventArgs(tableModel, e));
        }

        public void OnSaveColWidth(GridRowColSizeEventArgs e)
        {
            // Not Used.
        }

        public void OnMergeCellsChanged(GridMergeCellsChangedEventArgs e)
        {
            // Not Used.
        }

        public void OnEndUpdateRequest(GridEndUpdateRequestEventArgs e)
        {
            // Not Used.
        }

        public void OnDefaultColWidthChanged(GridDefaultSizeChangedEventArgs e)
        {
            // Not Used.
        }

        public void OnCoveredRangesChanged(GridCoveredRangesChangedEventArgs e)
        {
            // Not Used.
        }

        public void OnSelectionChanged(GridSelectionChangedEventArgs e)
        {
            owner.OnSelectionChanged(new GridTableModelGridSelectionChangedEventArgs(tableModel, e));
        }

        public void OnRowsMoving(GridRangeMovingEventArgs e)
        {
            // Not Used.
        }

        public void OnRowHeightsChanging(GridRowColSizeChangingEventArgs e)
        {
            // Not Used.
        }

        public void OnHeaderRowCountChanged(GridCountChangedEventArgs e)
        {
            // Not Used.
        }

        public void OnQueryHideRow(GridRowColHideEventArgs e)
        {
            // Not Used.
        }

        public void OnFrozenRowCountChanging(GridCountChangingEventArgs e)
        {
            // Not Used.
        }

        public void OnColsHiding(GridRowColHidingEventArgs e)
        {
            // Not Used.
        }

        public void OnQueryCellFormattedText(GridCellTextEventArgs e)
        {
            owner.OnQueryCellFormattedText(new GridTableModelGridCellTextEventArgs(tableModel, e));
        }

        public void OnDefaultRowHeightChanged(GridDefaultSizeChangedEventArgs e)
        {
            // Not Used.
        }

        public void OnClipboardCopy(GridCutPasteEventArgs e)
        {
            owner.OnClipboardCopy(new GridTableModelGridCutPasteEventArgs(tableModel, e));
        }

        public void OnCellModelsChanged(CollectionChangeEventArgs e)
        {
            // Not Used.
        }

        public void OnBanneredRangesChanging(GridBanneredRangesChangingEventArgs e)
        {
            // Not Used.
        }

        public void OnPasteCellText(GridPasteCellTextEventArgs e)
        {
            owner.OnPasteCellText(new GridTableModelGridPasteCellTextEventArgs(tableModel, e));
        }

        public void OnHeaderColCountChanged(GridCountChangedEventArgs e)
        {
            // Not Used.
        }

        public void OnFrozenColCountChanged(GridCountChangedEventArgs e)
        {
            // Not Used.
        }

        public void OnColWidthsChanging(GridRowColSizeChangingEventArgs e)
        {
            // Not Used.
        }

        public void OnColsInserting(GridRangeInsertingEventArgs e)
        {
            // Not Used.
        }

        public void OnRowsMoved(GridRangeMovedEventArgs e)
        {
            // Not Used.
        }

        public void OnSaveCellFormattedText(GridCellTextEventArgs e)
        {
            owner.OnSaveCellFormattedText(new GridTableModelGridCellTextEventArgs(tableModel, e));
        }

        public void OnQueryDragDropMoveClearCells(CancelEventArgs e)
        {
            owner.OnQueryDragDropMoveClearCells(new GridTableModelCancelEventArgs(tableModel, e));
        }

        public void OnSaveCellText(GridCellTextEventArgs e)
        {
            owner.OnSaveCellText(new GridTableModelGridCellTextEventArgs(tableModel, e));
        }

        public void OnDataChanged(EventArgs e)
        {
            // Not Used.
        }

        public void OnColsMoved(GridRangeMovedEventArgs e)
        {
            // Not Used.
        }

        public void OnColsHidden(GridRowColHiddenEventArgs e)
        {
            // Not Used.
        }

        public void OnBeginUpdateRequest(EventArgs e)
        {
            // Not Used.
        }

        public void OnRowsInserted(GridRangeInsertedEventArgs e)
        {
            // Not Used.
        }

        public void OnFrozenRowCountChanged(GridCountChangedEventArgs e)
        {
            // Not Used.
        }

        public void OnConfirmingPendingChanges(CancelEventArgs e)
        {
            // Not Used.
        }

        public void OnBanneredRangesChanged(GridBanneredRangesChangedEventArgs e)
        {
            // Not Used.
        }

        public void OnSaveRowHeight(GridRowColSizeEventArgs e)
        {
            // Not Used.
        }

        public void OnRowsHidden(GridRowColHiddenEventArgs e)
        {
            // Not Used.
        }

        public void OnQueryBanneredRange(GridQueryBanneredRangeEventArgs e)
        {
            owner.OnQueryBanneredRange(new GridTableModelGridQueryBanneredRangeEventArgs(tableModel, e));
        }

        public void OnFrozenColCountChanging(GridCountChangingEventArgs e)
        {
            // Not Used.
        }

        public void OnColsInserted(GridRangeInsertedEventArgs e)
        {
            // Not Used.
        }

        public void OnQueryHideCol(GridRowColHideEventArgs e)
        {
            // Not Used.
        }

        public void OnQueryRowHeight(GridRowColSizeEventArgs e)
        {
            // Not Used.
        }

        public void OnQueryCellText(GridCellTextEventArgs e)
        {
            owner.OnQueryCellText(new GridTableModelGridCellTextEventArgs(tableModel, e));
        }

        public void OnDataProviderChanged(EventArgs e)
        {
            // Not Used.
        }

        public void OnQueryRowHeightTotal(GridRowColSizeTotalEventArgs e)
        {
            // Not Used.
        }

        public void OnDefaultRowHeightChanging(GridDefaultSizeChangingEventArgs e)
        {
            // Not Used.
        }

        public void OnColsRemoving(GridRangeRemovingEventArgs e)
        {
            // Not Used.
        }

        public void OnDefaultColWidthChanging(GridDefaultSizeChangingEventArgs e)
        {
            // Not Used.
        }

        public void OnClipboardPaste(GridCutPasteEventArgs e)
        {
            owner.OnClipboardPaste(new GridTableModelGridCutPasteEventArgs(tableModel, e));
        }

        public void OnColWidthsChanged(GridRowColSizeChangedEventArgs e)
        {
            // Not Used.
        }

        public void OnClipboardPasted(GridCutPasteEventArgs e)
        {
            owner.OnClipboardPasted(new GridTableModelGridCutPasteEventArgs(tableModel, e));
        }

        public void OnColsMoving(GridRangeMovingEventArgs e)
        {
            // Not Used.
        }

        public void OnDataProviderSaveCellInfo(GridSaveCellInfoEventArgs e)
        {
            // Not Used.
        }

        public void OnHeaderColCountChanging(GridCountChangingEventArgs e)
        {
            // Not Used.
        }

        public void OnInvalidateRangeRequest(GridInvalidateRangeRequestEventArgs e)
        {
            // Not Used.
        }

        public void OnCoveredRangesChanging(GridCoveredRangesChangingEventArgs e)
        {
            // Not Used.
        }

        public void OnHeaderRowCountChanging(GridCountChangingEventArgs e)
        {
            // Not Used.
        }

        public void OnClipboardCanCopy(GridCutPasteEventArgs e)
        {
            owner.OnClipboardCanCopy(new GridTableModelGridCutPasteEventArgs(tableModel, e));
        }

        public void OnRowsRemoving(GridRangeRemovingEventArgs e)
        {
            // Not Used.
        }

        public void OnSaveHideCol(GridRowColHideEventArgs e)
        {
            // Not Used.
        }

        public void OnQueryColWidth(GridRowColSizeEventArgs e)
        {
            // Not Used.
        }

        public void OnSaveHideRow(GridRowColHideEventArgs e)
        {
            // Not Used.
        }

        public void OnClearingCells(GridClearingCellsEventArgs e)
        {
            owner.OnClearingCells(new GridTableModelGridClearingCellsEventArgs(tableModel, e));
        }

        public void OnClipboardCanCut(GridCutPasteEventArgs e)
        {
            owner.OnClipboardCanCut(new GridTableModelGridCutPasteEventArgs(tableModel, e));
        }

        public void OnQueryCoveredRange(GridQueryCoveredRangeEventArgs e)
        {
            // Not Used.
        }

        public void OnRowsRemoved(GridRangeRemovedEventArgs e)
        {
            // Not Used.
        }

        public void OnRowsInserting(GridRangeInsertingEventArgs e)
        {
            // Not Used.
        }

        public void OnFloatingCellsChanged(GridFloatingCellsChangedEventArgs e)
        {
            // Not Used.
        }

        public void OnQueryRowCount(GridRowColCountEventArgs e)
        {
            // Not Used.
        }

        public void OnColsRemoved(GridRangeRemovedEventArgs e)
        {
            // Not Used.
        }

        public void OnQueryCellInfo(GridQueryCellInfoEventArgs e)
        {
            // Not Used.
        }

        public void OnRowsHiding(GridRowColHidingEventArgs e)
        {
            // Not Used.
        }

        public void OnPrepareGraphics(GraphicsEventArgs e)
        {
            // Not Used.
        }

        public void OnQueryOleDataSourceData(GridQueryOleDataSourceDataEventArgs e)
        {
            // Not Used.
        }

        public void OnClipboardCut(GridCutPasteEventArgs e)
        {
            owner.OnClipboardCut(new GridTableModelGridCutPasteEventArgs(tableModel, e));
        }

        public void OnSelectionChanging(GridSelectionChangingEventArgs e)
        {
            owner.OnSelectionChanging(new GridTableModelGridSelectionChangingEventArgs(tableModel, e));
        }

        public void OnSaveRowCount(GridRowColCountEventArgs e)
        {
            // Not Used.
        }

        public void OnSaveColCount(GridRowColCountEventArgs e)
        {
            // Not Used.
        }

        public void OnQueryCanMergeCells(GridQueryCanMergeCellsEventArgs e)
        {
            // Not Used.
        }

        public void OnSaveCellInfo(GridSaveCellInfoEventArgs e)
        {
            // Not Used.
        }

        public void OnClipboardCanPaste(GridCutPasteEventArgs e)
        {
            owner.OnClipboardCanPaste(new GridTableModelGridCutPasteEventArgs(tableModel, e));
        }

        public void OnPrepareChangeSelection(GridPrepareChangeSelectionEventArgs e)
        {
            owner.OnPrepareChangeSelection(new GridTableModelGridPrepareChangeSelectionEventArgs(tableModel, e));
        }

        public void OnQueryCellModel(GridQueryCellModelEventArgs e)
        {
            owner.OnQueryCellModel(new GridTableModelGridQueryCellModelEventArgs(tableModel, e));
        }

        public void OnCellsChanging(GridCellsChangingEventArgs e)
        {
            // Not Used.
        }

        public void OnRowHeightsChanged(GridRowColSizeChangedEventArgs e)
        {
            // Not Used.
        }

        public void OnCellsChanged(GridCellsChangedEventArgs e)
        {
            // Not Used.
        }

        public void OnDataProviderQueryCellInfo(GridQueryCellInfoEventArgs e)
        {
            // Not Used.
        }

        #endregion
    }

#endif
}
