/**
 * DevExtreme (esm/renovation/ui/grids/data_grid/data_grid.j.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import registerComponent from "../../../../core/component_registrator";
import DataGridBaseComponent from "../../../component_wrapper/data_grid";
import {
    DataGrid as DataGridComponent,
    defaultOptions
} from "./data_grid";
export default class DataGrid extends DataGridBaseComponent {
    getProps() {
        var props = super.getProps();
        props.onKeyDown = this._wrapKeyDownHandler(props.onKeyDown);
        return props
    }
    getComponentInstance() {
        var _this$viewRef;
        return null === (_this$viewRef = this.viewRef) || void 0 === _this$viewRef ? void 0 : _this$viewRef.getComponentInstance(...arguments)
    }
    beginCustomLoading(messageText) {
        var _this$viewRef2;
        return null === (_this$viewRef2 = this.viewRef) || void 0 === _this$viewRef2 ? void 0 : _this$viewRef2.beginCustomLoading(...arguments)
    }
    byKey(key) {
        var _this$viewRef3;
        return null === (_this$viewRef3 = this.viewRef) || void 0 === _this$viewRef3 ? void 0 : _this$viewRef3.byKey(...arguments)
    }
    cancelEditData() {
        var _this$viewRef4;
        return null === (_this$viewRef4 = this.viewRef) || void 0 === _this$viewRef4 ? void 0 : _this$viewRef4.cancelEditData(...arguments)
    }
    cellValue(rowIndex, dataField, value) {
        var _this$viewRef5;
        return null === (_this$viewRef5 = this.viewRef) || void 0 === _this$viewRef5 ? void 0 : _this$viewRef5.cellValue(...arguments)
    }
    clearFilter(filterName) {
        var _this$viewRef6;
        return null === (_this$viewRef6 = this.viewRef) || void 0 === _this$viewRef6 ? void 0 : _this$viewRef6.clearFilter(...arguments)
    }
    clearSelection() {
        var _this$viewRef7;
        return null === (_this$viewRef7 = this.viewRef) || void 0 === _this$viewRef7 ? void 0 : _this$viewRef7.clearSelection(...arguments)
    }
    clearSorting() {
        var _this$viewRef8;
        return null === (_this$viewRef8 = this.viewRef) || void 0 === _this$viewRef8 ? void 0 : _this$viewRef8.clearSorting(...arguments)
    }
    closeEditCell() {
        var _this$viewRef9;
        return null === (_this$viewRef9 = this.viewRef) || void 0 === _this$viewRef9 ? void 0 : _this$viewRef9.closeEditCell(...arguments)
    }
    collapseAdaptiveDetailRow() {
        var _this$viewRef10;
        return null === (_this$viewRef10 = this.viewRef) || void 0 === _this$viewRef10 ? void 0 : _this$viewRef10.collapseAdaptiveDetailRow(...arguments)
    }
    columnCount() {
        var _this$viewRef11;
        return null === (_this$viewRef11 = this.viewRef) || void 0 === _this$viewRef11 ? void 0 : _this$viewRef11.columnCount(...arguments)
    }
    columnOption(id, optionName, optionValue) {
        var _this$viewRef12;
        return null === (_this$viewRef12 = this.viewRef) || void 0 === _this$viewRef12 ? void 0 : _this$viewRef12.columnOption(...arguments)
    }
    deleteColumn(id) {
        var _this$viewRef13;
        return null === (_this$viewRef13 = this.viewRef) || void 0 === _this$viewRef13 ? void 0 : _this$viewRef13.deleteColumn(...arguments)
    }
    deleteRow(rowIndex) {
        var _this$viewRef14;
        return null === (_this$viewRef14 = this.viewRef) || void 0 === _this$viewRef14 ? void 0 : _this$viewRef14.deleteRow(...arguments)
    }
    deselectAll() {
        var _this$viewRef15;
        return null === (_this$viewRef15 = this.viewRef) || void 0 === _this$viewRef15 ? void 0 : _this$viewRef15.deselectAll(...arguments)
    }
    deselectRows(keys) {
        var _this$viewRef16;
        return null === (_this$viewRef16 = this.viewRef) || void 0 === _this$viewRef16 ? void 0 : _this$viewRef16.deselectRows(...arguments)
    }
    editCell(rowIndex, dataFieldColumnIndex) {
        var _this$viewRef17;
        return null === (_this$viewRef17 = this.viewRef) || void 0 === _this$viewRef17 ? void 0 : _this$viewRef17.editCell(...arguments)
    }
    editRow(rowIndex) {
        var _this$viewRef18;
        return null === (_this$viewRef18 = this.viewRef) || void 0 === _this$viewRef18 ? void 0 : _this$viewRef18.editRow(...arguments)
    }
    endCustomLoading() {
        var _this$viewRef19;
        return null === (_this$viewRef19 = this.viewRef) || void 0 === _this$viewRef19 ? void 0 : _this$viewRef19.endCustomLoading(...arguments)
    }
    expandAdaptiveDetailRow(key) {
        var _this$viewRef20;
        return null === (_this$viewRef20 = this.viewRef) || void 0 === _this$viewRef20 ? void 0 : _this$viewRef20.expandAdaptiveDetailRow(...arguments)
    }
    filter(filterExpr) {
        var _this$viewRef21;
        return null === (_this$viewRef21 = this.viewRef) || void 0 === _this$viewRef21 ? void 0 : _this$viewRef21.filter(...arguments)
    }
    focus(element) {
        var _this$viewRef22;
        var params = [this._patchElementParam(element)];
        return null === (_this$viewRef22 = this.viewRef) || void 0 === _this$viewRef22 ? void 0 : _this$viewRef22.focus(...params.slice(0, arguments.length))
    }
    getCellElement(rowIndex, dataField) {
        var _this$viewRef23;
        return null === (_this$viewRef23 = this.viewRef) || void 0 === _this$viewRef23 ? void 0 : _this$viewRef23.getCellElement(...arguments)
    }
    getCombinedFilter(returnDataField) {
        var _this$viewRef24;
        return null === (_this$viewRef24 = this.viewRef) || void 0 === _this$viewRef24 ? void 0 : _this$viewRef24.getCombinedFilter(...arguments)
    }
    getDataSource() {
        var _this$viewRef25;
        return null === (_this$viewRef25 = this.viewRef) || void 0 === _this$viewRef25 ? void 0 : _this$viewRef25.getDataSource(...arguments)
    }
    getKeyByRowIndex(rowIndex) {
        var _this$viewRef26;
        return null === (_this$viewRef26 = this.viewRef) || void 0 === _this$viewRef26 ? void 0 : _this$viewRef26.getKeyByRowIndex(...arguments)
    }
    getRowElement(rowIndex) {
        var _this$viewRef27;
        return null === (_this$viewRef27 = this.viewRef) || void 0 === _this$viewRef27 ? void 0 : _this$viewRef27.getRowElement(...arguments)
    }
    getRowIndexByKey(key) {
        var _this$viewRef28;
        return null === (_this$viewRef28 = this.viewRef) || void 0 === _this$viewRef28 ? void 0 : _this$viewRef28.getRowIndexByKey(...arguments)
    }
    getScrollable() {
        var _this$viewRef29;
        return null === (_this$viewRef29 = this.viewRef) || void 0 === _this$viewRef29 ? void 0 : _this$viewRef29.getScrollable(...arguments)
    }
    getVisibleColumnIndex(id) {
        var _this$viewRef30;
        return null === (_this$viewRef30 = this.viewRef) || void 0 === _this$viewRef30 ? void 0 : _this$viewRef30.getVisibleColumnIndex(...arguments)
    }
    hasEditData() {
        var _this$viewRef31;
        return null === (_this$viewRef31 = this.viewRef) || void 0 === _this$viewRef31 ? void 0 : _this$viewRef31.hasEditData(...arguments)
    }
    hideColumnChooser() {
        var _this$viewRef32;
        return null === (_this$viewRef32 = this.viewRef) || void 0 === _this$viewRef32 ? void 0 : _this$viewRef32.hideColumnChooser(...arguments)
    }
    isAdaptiveDetailRowExpanded(key) {
        var _this$viewRef33;
        return null === (_this$viewRef33 = this.viewRef) || void 0 === _this$viewRef33 ? void 0 : _this$viewRef33.isAdaptiveDetailRowExpanded(...arguments)
    }
    isRowFocused(key) {
        var _this$viewRef34;
        return null === (_this$viewRef34 = this.viewRef) || void 0 === _this$viewRef34 ? void 0 : _this$viewRef34.isRowFocused(...arguments)
    }
    isRowSelected(key) {
        var _this$viewRef35;
        return null === (_this$viewRef35 = this.viewRef) || void 0 === _this$viewRef35 ? void 0 : _this$viewRef35.isRowSelected(...arguments)
    }
    keyOf(obj) {
        var _this$viewRef36;
        return null === (_this$viewRef36 = this.viewRef) || void 0 === _this$viewRef36 ? void 0 : _this$viewRef36.keyOf(...arguments)
    }
    navigateToRow(key) {
        var _this$viewRef37;
        return null === (_this$viewRef37 = this.viewRef) || void 0 === _this$viewRef37 ? void 0 : _this$viewRef37.navigateToRow(...arguments)
    }
    pageCount() {
        var _this$viewRef38;
        return null === (_this$viewRef38 = this.viewRef) || void 0 === _this$viewRef38 ? void 0 : _this$viewRef38.pageCount(...arguments)
    }
    pageIndex(newIndex) {
        var _this$viewRef39;
        return null === (_this$viewRef39 = this.viewRef) || void 0 === _this$viewRef39 ? void 0 : _this$viewRef39.pageIndex(...arguments)
    }
    pageSize(value) {
        var _this$viewRef40;
        return null === (_this$viewRef40 = this.viewRef) || void 0 === _this$viewRef40 ? void 0 : _this$viewRef40.pageSize(...arguments)
    }
    refresh(changesOnly) {
        var _this$viewRef41;
        return null === (_this$viewRef41 = this.viewRef) || void 0 === _this$viewRef41 ? void 0 : _this$viewRef41.refresh(...arguments)
    }
    repaintRows(rowIndexes) {
        var _this$viewRef42;
        return null === (_this$viewRef42 = this.viewRef) || void 0 === _this$viewRef42 ? void 0 : _this$viewRef42.repaintRows(...arguments)
    }
    saveEditData() {
        var _this$viewRef43;
        return null === (_this$viewRef43 = this.viewRef) || void 0 === _this$viewRef43 ? void 0 : _this$viewRef43.saveEditData(...arguments)
    }
    searchByText(text) {
        var _this$viewRef44;
        return null === (_this$viewRef44 = this.viewRef) || void 0 === _this$viewRef44 ? void 0 : _this$viewRef44.searchByText(...arguments)
    }
    selectAll() {
        var _this$viewRef45;
        return null === (_this$viewRef45 = this.viewRef) || void 0 === _this$viewRef45 ? void 0 : _this$viewRef45.selectAll(...arguments)
    }
    selectRows(keys, preserve) {
        var _this$viewRef46;
        return null === (_this$viewRef46 = this.viewRef) || void 0 === _this$viewRef46 ? void 0 : _this$viewRef46.selectRows(...arguments)
    }
    selectRowsByIndexes(indexes) {
        var _this$viewRef47;
        return null === (_this$viewRef47 = this.viewRef) || void 0 === _this$viewRef47 ? void 0 : _this$viewRef47.selectRowsByIndexes(...arguments)
    }
    showColumnChooser() {
        var _this$viewRef48;
        return null === (_this$viewRef48 = this.viewRef) || void 0 === _this$viewRef48 ? void 0 : _this$viewRef48.showColumnChooser(...arguments)
    }
    undeleteRow(rowIndex) {
        var _this$viewRef49;
        return null === (_this$viewRef49 = this.viewRef) || void 0 === _this$viewRef49 ? void 0 : _this$viewRef49.undeleteRow(...arguments)
    }
    updateDimensions() {
        var _this$viewRef50;
        return null === (_this$viewRef50 = this.viewRef) || void 0 === _this$viewRef50 ? void 0 : _this$viewRef50.updateDimensions(...arguments)
    }
    resize() {
        var _this$viewRef51;
        return null === (_this$viewRef51 = this.viewRef) || void 0 === _this$viewRef51 ? void 0 : _this$viewRef51.resize(...arguments)
    }
    addColumn(columnOptions) {
        var _this$viewRef52;
        return null === (_this$viewRef52 = this.viewRef) || void 0 === _this$viewRef52 ? void 0 : _this$viewRef52.addColumn(...arguments)
    }
    addRow() {
        var _this$viewRef53;
        return null === (_this$viewRef53 = this.viewRef) || void 0 === _this$viewRef53 ? void 0 : _this$viewRef53.addRow(...arguments)
    }
    clearGrouping() {
        var _this$viewRef54;
        return null === (_this$viewRef54 = this.viewRef) || void 0 === _this$viewRef54 ? void 0 : _this$viewRef54.clearGrouping(...arguments)
    }
    collapseAll(groupIndex) {
        var _this$viewRef55;
        return null === (_this$viewRef55 = this.viewRef) || void 0 === _this$viewRef55 ? void 0 : _this$viewRef55.collapseAll(...arguments)
    }
    collapseRow(key) {
        var _this$viewRef56;
        return null === (_this$viewRef56 = this.viewRef) || void 0 === _this$viewRef56 ? void 0 : _this$viewRef56.collapseRow(...arguments)
    }
    expandAll(groupIndex) {
        var _this$viewRef57;
        return null === (_this$viewRef57 = this.viewRef) || void 0 === _this$viewRef57 ? void 0 : _this$viewRef57.expandAll(...arguments)
    }
    expandRow(key) {
        var _this$viewRef58;
        return null === (_this$viewRef58 = this.viewRef) || void 0 === _this$viewRef58 ? void 0 : _this$viewRef58.expandRow(...arguments)
    }
    exportToExcel(selectionOnly) {
        var _this$viewRef59;
        return null === (_this$viewRef59 = this.viewRef) || void 0 === _this$viewRef59 ? void 0 : _this$viewRef59.exportToExcel(...arguments)
    }
    getSelectedRowKeys() {
        var _this$viewRef60;
        return null === (_this$viewRef60 = this.viewRef) || void 0 === _this$viewRef60 ? void 0 : _this$viewRef60.getSelectedRowKeys(...arguments)
    }
    getSelectedRowsData() {
        var _this$viewRef61;
        return null === (_this$viewRef61 = this.viewRef) || void 0 === _this$viewRef61 ? void 0 : _this$viewRef61.getSelectedRowsData(...arguments)
    }
    getTotalSummaryValue(summaryItemName) {
        var _this$viewRef62;
        return null === (_this$viewRef62 = this.viewRef) || void 0 === _this$viewRef62 ? void 0 : _this$viewRef62.getTotalSummaryValue(...arguments)
    }
    getVisibleColumns(headerLevel) {
        var _this$viewRef63;
        return null === (_this$viewRef63 = this.viewRef) || void 0 === _this$viewRef63 ? void 0 : _this$viewRef63.getVisibleColumns(...arguments)
    }
    getVisibleRows() {
        var _this$viewRef64;
        return null === (_this$viewRef64 = this.viewRef) || void 0 === _this$viewRef64 ? void 0 : _this$viewRef64.getVisibleRows(...arguments)
    }
    isRowExpanded(key) {
        var _this$viewRef65;
        return null === (_this$viewRef65 = this.viewRef) || void 0 === _this$viewRef65 ? void 0 : _this$viewRef65.isRowExpanded(...arguments)
    }
    totalCount() {
        var _this$viewRef66;
        return null === (_this$viewRef66 = this.viewRef) || void 0 === _this$viewRef66 ? void 0 : _this$viewRef66.totalCount(...arguments)
    }
    isScrollbarVisible() {
        var _this$viewRef67;
        return null === (_this$viewRef67 = this.viewRef) || void 0 === _this$viewRef67 ? void 0 : _this$viewRef67.isScrollbarVisible(...arguments)
    }
    getTopVisibleRowData() {
        var _this$viewRef68;
        return null === (_this$viewRef68 = this.viewRef) || void 0 === _this$viewRef68 ? void 0 : _this$viewRef68.getTopVisibleRowData(...arguments)
    }
    getScrollbarWidth(isHorizontal) {
        var _this$viewRef69;
        return null === (_this$viewRef69 = this.viewRef) || void 0 === _this$viewRef69 ? void 0 : _this$viewRef69.getScrollbarWidth(...arguments)
    }
    getDataProvider(selectedRowsOnly) {
        var _this$viewRef70;
        return null === (_this$viewRef70 = this.viewRef) || void 0 === _this$viewRef70 ? void 0 : _this$viewRef70.getDataProvider(...arguments)
    }
    _getActionConfigs() {
        return {
            onCellClick: {},
            onCellDblClick: {},
            onCellHoverChanged: {},
            onCellPrepared: {},
            onContextMenuPreparing: {},
            onEditingStart: {},
            onEditorPrepared: {},
            onEditorPreparing: {},
            onExported: {},
            onExporting: {},
            onFileSaving: {},
            onFocusedCellChanged: {},
            onFocusedCellChanging: {},
            onFocusedRowChanged: {},
            onFocusedRowChanging: {},
            onRowClick: {},
            onRowDblClick: {},
            onRowPrepared: {},
            onAdaptiveDetailRowPreparing: {},
            onDataErrorOccurred: {},
            onInitNewRow: {},
            onRowCollapsed: {},
            onRowCollapsing: {},
            onRowExpanded: {},
            onRowExpanding: {},
            onRowInserted: {},
            onRowInserting: {},
            onRowRemoved: {},
            onRowRemoving: {},
            onRowUpdated: {},
            onRowUpdating: {},
            onRowValidating: {},
            onSelectionChanged: {},
            onToolbarPreparing: {},
            onSaving: {},
            onSaved: {},
            onEditCanceling: {},
            onEditCanceled: {},
            onClick: {}
        }
    }
    get _propsInfo() {
        return {
            twoWay: [
                ["filterValue", "defaultFilterValue", "filterValueChange"],
                ["focusedColumnIndex", "defaultFocusedColumnIndex", "focusedColumnIndexChange"],
                ["focusedRowIndex", "defaultFocusedRowIndex", "focusedRowIndexChange"],
                ["focusedRowKey", "defaultFocusedRowKey", "focusedRowKeyChange"],
                ["selectedRowKeys", "defaultSelectedRowKeys", "selectedRowKeysChange"],
                ["selectionFilter", "defaultSelectionFilter", "selectionFilterChange"]
            ],
            allowNull: ["defaultFilterValue", "filterValue"],
            elements: [],
            templates: ["rowTemplate", "dataRowTemplate"],
            props: ["columns", "editing", "export", "groupPanel", "grouping", "masterDetail", "scrolling", "selection", "sortByGroupSummaryInfo", "summary", "columnChooser", "columnFixing", "filterPanel", "filterRow", "headerFilter", "useKeyboard", "keyboardNavigation", "loadPanel", "pager", "paging", "rowDragging", "searchPanel", "sorting", "stateStoring", "rowTemplate", "dataRowTemplate", "customizeColumns", "customizeExportData", "keyExpr", "remoteOperations", "allowColumnReordering", "allowColumnResizing", "autoNavigateToFocusedRow", "cacheEnabled", "cellHintEnabled", "columnAutoWidth", "columnHidingEnabled", "columnMinWidth", "columnResizingMode", "columnWidth", "dataSource", "dateSerializationFormat", "errorRowEnabled", "filterBuilder", "filterBuilderPopup", "filterSyncEnabled", "focusedRowEnabled", "highlightChanges", "noDataText", "renderAsync", "repaintChangesOnly", "rowAlternationEnabled", "showBorders", "showColumnHeaders", "showColumnLines", "showRowLines", "twoWayBindingEnabled", "wordWrapEnabled", "loadingTimeout", "commonColumnSettings", "toolbar", "onCellClick", "onCellDblClick", "onCellHoverChanged", "onCellPrepared", "onContextMenuPreparing", "onEditingStart", "onEditorPrepared", "onEditorPreparing", "onExported", "onExporting", "onFileSaving", "onFocusedCellChanged", "onFocusedCellChanging", "onFocusedRowChanged", "onFocusedRowChanging", "onRowClick", "onRowDblClick", "onRowPrepared", "onAdaptiveDetailRowPreparing", "onDataErrorOccurred", "onInitNewRow", "onKeyDown", "onRowCollapsed", "onRowCollapsing", "onRowExpanded", "onRowExpanding", "onRowInserted", "onRowInserting", "onRowRemoved", "onRowRemoving", "onRowUpdated", "onRowUpdating", "onRowValidating", "onSelectionChanged", "onToolbarPreparing", "onSaving", "onSaved", "onEditCanceling", "onEditCanceled", "adaptColumnWidthByRatio", "regenerateColumnsByVisibleItems", "useLegacyKeyboardNavigation", "useLegacyColumnButtonTemplate", "defaultFilterValue", "filterValueChange", "defaultFocusedColumnIndex", "focusedColumnIndexChange", "defaultFocusedRowIndex", "focusedRowIndexChange", "defaultFocusedRowKey", "focusedRowKeyChange", "defaultSelectedRowKeys", "selectedRowKeysChange", "defaultSelectionFilter", "selectionFilterChange", "className", "accessKey", "activeStateEnabled", "disabled", "focusStateEnabled", "height", "hint", "hoverStateEnabled", "onClick", "rtlEnabled", "tabIndex", "visible", "width", "filterValue", "focusedColumnIndex", "focusedRowIndex", "focusedRowKey", "selectedRowKeys", "selectionFilter"]
        }
    }
    get _viewComponent() {
        return DataGridComponent
    }
}
registerComponent("dxDataGrid", DataGrid);
DataGrid.defaultOptions = defaultOptions;
