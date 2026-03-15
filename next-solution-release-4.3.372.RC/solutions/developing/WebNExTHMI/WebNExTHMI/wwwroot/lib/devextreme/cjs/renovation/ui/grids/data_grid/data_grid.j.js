/**
 * DevExtreme (cjs/renovation/ui/grids/data_grid/data_grid.j.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.default = void 0;
var _component_registrator = _interopRequireDefault(require("../../../../core/component_registrator"));
var _data_grid = _interopRequireDefault(require("../../../component_wrapper/data_grid"));
var _data_grid2 = require("./data_grid");

function _interopRequireDefault(obj) {
    return obj && obj.__esModule ? obj : {
        default: obj
    }
}

function _toConsumableArray(arr) {
    return _arrayWithoutHoles(arr) || _iterableToArray(arr) || _unsupportedIterableToArray(arr) || _nonIterableSpread()
}

function _nonIterableSpread() {
    throw new TypeError("Invalid attempt to spread non-iterable instance.\nIn order to be iterable, non-array objects must have a [Symbol.iterator]() method.")
}

function _unsupportedIterableToArray(o, minLen) {
    if (!o) {
        return
    }
    if ("string" === typeof o) {
        return _arrayLikeToArray(o, minLen)
    }
    var n = Object.prototype.toString.call(o).slice(8, -1);
    if ("Object" === n && o.constructor) {
        n = o.constructor.name
    }
    if ("Map" === n || "Set" === n) {
        return Array.from(o)
    }
    if ("Arguments" === n || /^(?:Ui|I)nt(?:8|16|32)(?:Clamped)?Array$/.test(n)) {
        return _arrayLikeToArray(o, minLen)
    }
}

function _iterableToArray(iter) {
    if ("undefined" !== typeof Symbol && null != iter[Symbol.iterator] || null != iter["@@iterator"]) {
        return Array.from(iter)
    }
}

function _arrayWithoutHoles(arr) {
    if (Array.isArray(arr)) {
        return _arrayLikeToArray(arr)
    }
}

function _arrayLikeToArray(arr, len) {
    if (null == len || len > arr.length) {
        len = arr.length
    }
    for (var i = 0, arr2 = new Array(len); i < len; i++) {
        arr2[i] = arr[i]
    }
    return arr2
}

function _defineProperties(target, props) {
    for (var i = 0; i < props.length; i++) {
        var descriptor = props[i];
        descriptor.enumerable = descriptor.enumerable || false;
        descriptor.configurable = true;
        if ("value" in descriptor) {
            descriptor.writable = true
        }
        Object.defineProperty(target, descriptor.key, descriptor)
    }
}

function _createClass(Constructor, protoProps, staticProps) {
    if (protoProps) {
        _defineProperties(Constructor.prototype, protoProps)
    }
    if (staticProps) {
        _defineProperties(Constructor, staticProps)
    }
    Object.defineProperty(Constructor, "prototype", {
        writable: false
    });
    return Constructor
}

function _inheritsLoose(subClass, superClass) {
    subClass.prototype = Object.create(superClass.prototype);
    subClass.prototype.constructor = subClass;
    _setPrototypeOf(subClass, superClass)
}

function _setPrototypeOf(o, p) {
    _setPrototypeOf = Object.setPrototypeOf || function(o, p) {
        o.__proto__ = p;
        return o
    };
    return _setPrototypeOf(o, p)
}
var DataGrid = function(_DataGridBaseComponen) {
    _inheritsLoose(DataGrid, _DataGridBaseComponen);

    function DataGrid() {
        return _DataGridBaseComponen.apply(this, arguments) || this
    }
    var _proto = DataGrid.prototype;
    _proto.getProps = function() {
        var props = _DataGridBaseComponen.prototype.getProps.call(this);
        props.onKeyDown = this._wrapKeyDownHandler(props.onKeyDown);
        return props
    };
    _proto.getComponentInstance = function() {
        var _this$viewRef;
        return null === (_this$viewRef = this.viewRef) || void 0 === _this$viewRef ? void 0 : _this$viewRef.getComponentInstance.apply(_this$viewRef, arguments)
    };
    _proto.beginCustomLoading = function(messageText) {
        var _this$viewRef2;
        return null === (_this$viewRef2 = this.viewRef) || void 0 === _this$viewRef2 ? void 0 : _this$viewRef2.beginCustomLoading.apply(_this$viewRef2, arguments)
    };
    _proto.byKey = function(key) {
        var _this$viewRef3;
        return null === (_this$viewRef3 = this.viewRef) || void 0 === _this$viewRef3 ? void 0 : _this$viewRef3.byKey.apply(_this$viewRef3, arguments)
    };
    _proto.cancelEditData = function() {
        var _this$viewRef4;
        return null === (_this$viewRef4 = this.viewRef) || void 0 === _this$viewRef4 ? void 0 : _this$viewRef4.cancelEditData.apply(_this$viewRef4, arguments)
    };
    _proto.cellValue = function(rowIndex, dataField, value) {
        var _this$viewRef5;
        return null === (_this$viewRef5 = this.viewRef) || void 0 === _this$viewRef5 ? void 0 : _this$viewRef5.cellValue.apply(_this$viewRef5, arguments)
    };
    _proto.clearFilter = function(filterName) {
        var _this$viewRef6;
        return null === (_this$viewRef6 = this.viewRef) || void 0 === _this$viewRef6 ? void 0 : _this$viewRef6.clearFilter.apply(_this$viewRef6, arguments)
    };
    _proto.clearSelection = function() {
        var _this$viewRef7;
        return null === (_this$viewRef7 = this.viewRef) || void 0 === _this$viewRef7 ? void 0 : _this$viewRef7.clearSelection.apply(_this$viewRef7, arguments)
    };
    _proto.clearSorting = function() {
        var _this$viewRef8;
        return null === (_this$viewRef8 = this.viewRef) || void 0 === _this$viewRef8 ? void 0 : _this$viewRef8.clearSorting.apply(_this$viewRef8, arguments)
    };
    _proto.closeEditCell = function() {
        var _this$viewRef9;
        return null === (_this$viewRef9 = this.viewRef) || void 0 === _this$viewRef9 ? void 0 : _this$viewRef9.closeEditCell.apply(_this$viewRef9, arguments)
    };
    _proto.collapseAdaptiveDetailRow = function() {
        var _this$viewRef10;
        return null === (_this$viewRef10 = this.viewRef) || void 0 === _this$viewRef10 ? void 0 : _this$viewRef10.collapseAdaptiveDetailRow.apply(_this$viewRef10, arguments)
    };
    _proto.columnCount = function() {
        var _this$viewRef11;
        return null === (_this$viewRef11 = this.viewRef) || void 0 === _this$viewRef11 ? void 0 : _this$viewRef11.columnCount.apply(_this$viewRef11, arguments)
    };
    _proto.columnOption = function(id, optionName, optionValue) {
        var _this$viewRef12;
        return null === (_this$viewRef12 = this.viewRef) || void 0 === _this$viewRef12 ? void 0 : _this$viewRef12.columnOption.apply(_this$viewRef12, arguments)
    };
    _proto.deleteColumn = function(id) {
        var _this$viewRef13;
        return null === (_this$viewRef13 = this.viewRef) || void 0 === _this$viewRef13 ? void 0 : _this$viewRef13.deleteColumn.apply(_this$viewRef13, arguments)
    };
    _proto.deleteRow = function(rowIndex) {
        var _this$viewRef14;
        return null === (_this$viewRef14 = this.viewRef) || void 0 === _this$viewRef14 ? void 0 : _this$viewRef14.deleteRow.apply(_this$viewRef14, arguments)
    };
    _proto.deselectAll = function() {
        var _this$viewRef15;
        return null === (_this$viewRef15 = this.viewRef) || void 0 === _this$viewRef15 ? void 0 : _this$viewRef15.deselectAll.apply(_this$viewRef15, arguments)
    };
    _proto.deselectRows = function(keys) {
        var _this$viewRef16;
        return null === (_this$viewRef16 = this.viewRef) || void 0 === _this$viewRef16 ? void 0 : _this$viewRef16.deselectRows.apply(_this$viewRef16, arguments)
    };
    _proto.editCell = function(rowIndex, dataFieldColumnIndex) {
        var _this$viewRef17;
        return null === (_this$viewRef17 = this.viewRef) || void 0 === _this$viewRef17 ? void 0 : _this$viewRef17.editCell.apply(_this$viewRef17, arguments)
    };
    _proto.editRow = function(rowIndex) {
        var _this$viewRef18;
        return null === (_this$viewRef18 = this.viewRef) || void 0 === _this$viewRef18 ? void 0 : _this$viewRef18.editRow.apply(_this$viewRef18, arguments)
    };
    _proto.endCustomLoading = function() {
        var _this$viewRef19;
        return null === (_this$viewRef19 = this.viewRef) || void 0 === _this$viewRef19 ? void 0 : _this$viewRef19.endCustomLoading.apply(_this$viewRef19, arguments)
    };
    _proto.expandAdaptiveDetailRow = function(key) {
        var _this$viewRef20;
        return null === (_this$viewRef20 = this.viewRef) || void 0 === _this$viewRef20 ? void 0 : _this$viewRef20.expandAdaptiveDetailRow.apply(_this$viewRef20, arguments)
    };
    _proto.filter = function(filterExpr) {
        var _this$viewRef21;
        return null === (_this$viewRef21 = this.viewRef) || void 0 === _this$viewRef21 ? void 0 : _this$viewRef21.filter.apply(_this$viewRef21, arguments)
    };
    _proto.focus = function(element) {
        var _this$viewRef22;
        var params = [this._patchElementParam(element)];
        return null === (_this$viewRef22 = this.viewRef) || void 0 === _this$viewRef22 ? void 0 : _this$viewRef22.focus.apply(_this$viewRef22, _toConsumableArray(params.slice(0, arguments.length)))
    };
    _proto.getCellElement = function(rowIndex, dataField) {
        var _this$viewRef23;
        return null === (_this$viewRef23 = this.viewRef) || void 0 === _this$viewRef23 ? void 0 : _this$viewRef23.getCellElement.apply(_this$viewRef23, arguments)
    };
    _proto.getCombinedFilter = function(returnDataField) {
        var _this$viewRef24;
        return null === (_this$viewRef24 = this.viewRef) || void 0 === _this$viewRef24 ? void 0 : _this$viewRef24.getCombinedFilter.apply(_this$viewRef24, arguments)
    };
    _proto.getDataSource = function() {
        var _this$viewRef25;
        return null === (_this$viewRef25 = this.viewRef) || void 0 === _this$viewRef25 ? void 0 : _this$viewRef25.getDataSource.apply(_this$viewRef25, arguments)
    };
    _proto.getKeyByRowIndex = function(rowIndex) {
        var _this$viewRef26;
        return null === (_this$viewRef26 = this.viewRef) || void 0 === _this$viewRef26 ? void 0 : _this$viewRef26.getKeyByRowIndex.apply(_this$viewRef26, arguments)
    };
    _proto.getRowElement = function(rowIndex) {
        var _this$viewRef27;
        return null === (_this$viewRef27 = this.viewRef) || void 0 === _this$viewRef27 ? void 0 : _this$viewRef27.getRowElement.apply(_this$viewRef27, arguments)
    };
    _proto.getRowIndexByKey = function(key) {
        var _this$viewRef28;
        return null === (_this$viewRef28 = this.viewRef) || void 0 === _this$viewRef28 ? void 0 : _this$viewRef28.getRowIndexByKey.apply(_this$viewRef28, arguments)
    };
    _proto.getScrollable = function() {
        var _this$viewRef29;
        return null === (_this$viewRef29 = this.viewRef) || void 0 === _this$viewRef29 ? void 0 : _this$viewRef29.getScrollable.apply(_this$viewRef29, arguments)
    };
    _proto.getVisibleColumnIndex = function(id) {
        var _this$viewRef30;
        return null === (_this$viewRef30 = this.viewRef) || void 0 === _this$viewRef30 ? void 0 : _this$viewRef30.getVisibleColumnIndex.apply(_this$viewRef30, arguments)
    };
    _proto.hasEditData = function() {
        var _this$viewRef31;
        return null === (_this$viewRef31 = this.viewRef) || void 0 === _this$viewRef31 ? void 0 : _this$viewRef31.hasEditData.apply(_this$viewRef31, arguments)
    };
    _proto.hideColumnChooser = function() {
        var _this$viewRef32;
        return null === (_this$viewRef32 = this.viewRef) || void 0 === _this$viewRef32 ? void 0 : _this$viewRef32.hideColumnChooser.apply(_this$viewRef32, arguments)
    };
    _proto.isAdaptiveDetailRowExpanded = function(key) {
        var _this$viewRef33;
        return null === (_this$viewRef33 = this.viewRef) || void 0 === _this$viewRef33 ? void 0 : _this$viewRef33.isAdaptiveDetailRowExpanded.apply(_this$viewRef33, arguments)
    };
    _proto.isRowFocused = function(key) {
        var _this$viewRef34;
        return null === (_this$viewRef34 = this.viewRef) || void 0 === _this$viewRef34 ? void 0 : _this$viewRef34.isRowFocused.apply(_this$viewRef34, arguments)
    };
    _proto.isRowSelected = function(key) {
        var _this$viewRef35;
        return null === (_this$viewRef35 = this.viewRef) || void 0 === _this$viewRef35 ? void 0 : _this$viewRef35.isRowSelected.apply(_this$viewRef35, arguments)
    };
    _proto.keyOf = function(obj) {
        var _this$viewRef36;
        return null === (_this$viewRef36 = this.viewRef) || void 0 === _this$viewRef36 ? void 0 : _this$viewRef36.keyOf.apply(_this$viewRef36, arguments)
    };
    _proto.navigateToRow = function(key) {
        var _this$viewRef37;
        return null === (_this$viewRef37 = this.viewRef) || void 0 === _this$viewRef37 ? void 0 : _this$viewRef37.navigateToRow.apply(_this$viewRef37, arguments)
    };
    _proto.pageCount = function() {
        var _this$viewRef38;
        return null === (_this$viewRef38 = this.viewRef) || void 0 === _this$viewRef38 ? void 0 : _this$viewRef38.pageCount.apply(_this$viewRef38, arguments)
    };
    _proto.pageIndex = function(newIndex) {
        var _this$viewRef39;
        return null === (_this$viewRef39 = this.viewRef) || void 0 === _this$viewRef39 ? void 0 : _this$viewRef39.pageIndex.apply(_this$viewRef39, arguments)
    };
    _proto.pageSize = function(value) {
        var _this$viewRef40;
        return null === (_this$viewRef40 = this.viewRef) || void 0 === _this$viewRef40 ? void 0 : _this$viewRef40.pageSize.apply(_this$viewRef40, arguments)
    };
    _proto.refresh = function(changesOnly) {
        var _this$viewRef41;
        return null === (_this$viewRef41 = this.viewRef) || void 0 === _this$viewRef41 ? void 0 : _this$viewRef41.refresh.apply(_this$viewRef41, arguments)
    };
    _proto.repaintRows = function(rowIndexes) {
        var _this$viewRef42;
        return null === (_this$viewRef42 = this.viewRef) || void 0 === _this$viewRef42 ? void 0 : _this$viewRef42.repaintRows.apply(_this$viewRef42, arguments)
    };
    _proto.saveEditData = function() {
        var _this$viewRef43;
        return null === (_this$viewRef43 = this.viewRef) || void 0 === _this$viewRef43 ? void 0 : _this$viewRef43.saveEditData.apply(_this$viewRef43, arguments)
    };
    _proto.searchByText = function(text) {
        var _this$viewRef44;
        return null === (_this$viewRef44 = this.viewRef) || void 0 === _this$viewRef44 ? void 0 : _this$viewRef44.searchByText.apply(_this$viewRef44, arguments)
    };
    _proto.selectAll = function() {
        var _this$viewRef45;
        return null === (_this$viewRef45 = this.viewRef) || void 0 === _this$viewRef45 ? void 0 : _this$viewRef45.selectAll.apply(_this$viewRef45, arguments)
    };
    _proto.selectRows = function(keys, preserve) {
        var _this$viewRef46;
        return null === (_this$viewRef46 = this.viewRef) || void 0 === _this$viewRef46 ? void 0 : _this$viewRef46.selectRows.apply(_this$viewRef46, arguments)
    };
    _proto.selectRowsByIndexes = function(indexes) {
        var _this$viewRef47;
        return null === (_this$viewRef47 = this.viewRef) || void 0 === _this$viewRef47 ? void 0 : _this$viewRef47.selectRowsByIndexes.apply(_this$viewRef47, arguments)
    };
    _proto.showColumnChooser = function() {
        var _this$viewRef48;
        return null === (_this$viewRef48 = this.viewRef) || void 0 === _this$viewRef48 ? void 0 : _this$viewRef48.showColumnChooser.apply(_this$viewRef48, arguments)
    };
    _proto.undeleteRow = function(rowIndex) {
        var _this$viewRef49;
        return null === (_this$viewRef49 = this.viewRef) || void 0 === _this$viewRef49 ? void 0 : _this$viewRef49.undeleteRow.apply(_this$viewRef49, arguments)
    };
    _proto.updateDimensions = function() {
        var _this$viewRef50;
        return null === (_this$viewRef50 = this.viewRef) || void 0 === _this$viewRef50 ? void 0 : _this$viewRef50.updateDimensions.apply(_this$viewRef50, arguments)
    };
    _proto.resize = function() {
        var _this$viewRef51;
        return null === (_this$viewRef51 = this.viewRef) || void 0 === _this$viewRef51 ? void 0 : _this$viewRef51.resize.apply(_this$viewRef51, arguments)
    };
    _proto.addColumn = function(columnOptions) {
        var _this$viewRef52;
        return null === (_this$viewRef52 = this.viewRef) || void 0 === _this$viewRef52 ? void 0 : _this$viewRef52.addColumn.apply(_this$viewRef52, arguments)
    };
    _proto.addRow = function() {
        var _this$viewRef53;
        return null === (_this$viewRef53 = this.viewRef) || void 0 === _this$viewRef53 ? void 0 : _this$viewRef53.addRow.apply(_this$viewRef53, arguments)
    };
    _proto.clearGrouping = function() {
        var _this$viewRef54;
        return null === (_this$viewRef54 = this.viewRef) || void 0 === _this$viewRef54 ? void 0 : _this$viewRef54.clearGrouping.apply(_this$viewRef54, arguments)
    };
    _proto.collapseAll = function(groupIndex) {
        var _this$viewRef55;
        return null === (_this$viewRef55 = this.viewRef) || void 0 === _this$viewRef55 ? void 0 : _this$viewRef55.collapseAll.apply(_this$viewRef55, arguments)
    };
    _proto.collapseRow = function(key) {
        var _this$viewRef56;
        return null === (_this$viewRef56 = this.viewRef) || void 0 === _this$viewRef56 ? void 0 : _this$viewRef56.collapseRow.apply(_this$viewRef56, arguments)
    };
    _proto.expandAll = function(groupIndex) {
        var _this$viewRef57;
        return null === (_this$viewRef57 = this.viewRef) || void 0 === _this$viewRef57 ? void 0 : _this$viewRef57.expandAll.apply(_this$viewRef57, arguments)
    };
    _proto.expandRow = function(key) {
        var _this$viewRef58;
        return null === (_this$viewRef58 = this.viewRef) || void 0 === _this$viewRef58 ? void 0 : _this$viewRef58.expandRow.apply(_this$viewRef58, arguments)
    };
    _proto.exportToExcel = function(selectionOnly) {
        var _this$viewRef59;
        return null === (_this$viewRef59 = this.viewRef) || void 0 === _this$viewRef59 ? void 0 : _this$viewRef59.exportToExcel.apply(_this$viewRef59, arguments)
    };
    _proto.getSelectedRowKeys = function() {
        var _this$viewRef60;
        return null === (_this$viewRef60 = this.viewRef) || void 0 === _this$viewRef60 ? void 0 : _this$viewRef60.getSelectedRowKeys.apply(_this$viewRef60, arguments)
    };
    _proto.getSelectedRowsData = function() {
        var _this$viewRef61;
        return null === (_this$viewRef61 = this.viewRef) || void 0 === _this$viewRef61 ? void 0 : _this$viewRef61.getSelectedRowsData.apply(_this$viewRef61, arguments)
    };
    _proto.getTotalSummaryValue = function(summaryItemName) {
        var _this$viewRef62;
        return null === (_this$viewRef62 = this.viewRef) || void 0 === _this$viewRef62 ? void 0 : _this$viewRef62.getTotalSummaryValue.apply(_this$viewRef62, arguments)
    };
    _proto.getVisibleColumns = function(headerLevel) {
        var _this$viewRef63;
        return null === (_this$viewRef63 = this.viewRef) || void 0 === _this$viewRef63 ? void 0 : _this$viewRef63.getVisibleColumns.apply(_this$viewRef63, arguments)
    };
    _proto.getVisibleRows = function() {
        var _this$viewRef64;
        return null === (_this$viewRef64 = this.viewRef) || void 0 === _this$viewRef64 ? void 0 : _this$viewRef64.getVisibleRows.apply(_this$viewRef64, arguments)
    };
    _proto.isRowExpanded = function(key) {
        var _this$viewRef65;
        return null === (_this$viewRef65 = this.viewRef) || void 0 === _this$viewRef65 ? void 0 : _this$viewRef65.isRowExpanded.apply(_this$viewRef65, arguments)
    };
    _proto.totalCount = function() {
        var _this$viewRef66;
        return null === (_this$viewRef66 = this.viewRef) || void 0 === _this$viewRef66 ? void 0 : _this$viewRef66.totalCount.apply(_this$viewRef66, arguments)
    };
    _proto.isScrollbarVisible = function() {
        var _this$viewRef67;
        return null === (_this$viewRef67 = this.viewRef) || void 0 === _this$viewRef67 ? void 0 : _this$viewRef67.isScrollbarVisible.apply(_this$viewRef67, arguments)
    };
    _proto.getTopVisibleRowData = function() {
        var _this$viewRef68;
        return null === (_this$viewRef68 = this.viewRef) || void 0 === _this$viewRef68 ? void 0 : _this$viewRef68.getTopVisibleRowData.apply(_this$viewRef68, arguments)
    };
    _proto.getScrollbarWidth = function(isHorizontal) {
        var _this$viewRef69;
        return null === (_this$viewRef69 = this.viewRef) || void 0 === _this$viewRef69 ? void 0 : _this$viewRef69.getScrollbarWidth.apply(_this$viewRef69, arguments)
    };
    _proto.getDataProvider = function(selectedRowsOnly) {
        var _this$viewRef70;
        return null === (_this$viewRef70 = this.viewRef) || void 0 === _this$viewRef70 ? void 0 : _this$viewRef70.getDataProvider.apply(_this$viewRef70, arguments)
    };
    _proto._getActionConfigs = function() {
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
    };
    _createClass(DataGrid, [{
        key: "_propsInfo",
        get: function() {
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
    }, {
        key: "_viewComponent",
        get: function() {
            return _data_grid2.DataGrid
        }
    }]);
    return DataGrid
}(_data_grid.default);
exports.default = DataGrid;
(0, _component_registrator.default)("dxDataGrid", DataGrid);
DataGrid.defaultOptions = _data_grid2.defaultOptions;
module.exports = exports.default;
module.exports.default = exports.default;
