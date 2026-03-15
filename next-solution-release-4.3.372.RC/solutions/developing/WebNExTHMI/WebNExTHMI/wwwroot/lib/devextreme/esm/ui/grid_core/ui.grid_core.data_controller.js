/**
 * DevExtreme (esm/ui/grid_core/ui.grid_core.data_controller.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import $ from "../../core/renderer";
import modules from "./ui.grid_core.modules";
import gridCoreUtils from "./ui.grid_core.utils";
import ArrayStore from "../../data/array_store";
import CustomStore from "../../data/custom_store";
import errors from "../widget/ui.errors";
import {
    noop,
    deferRender,
    equalByValue
} from "../../core/utils/common";
import {
    each
} from "../../core/utils/iterator";
import {
    isDefined
} from "../../core/utils/type";
import {
    extend
} from "../../core/utils/extend";
import DataHelperMixin from "../../data_helper";
import {
    when,
    Deferred
} from "../../core/utils/deferred";
import {
    findChanges
} from "../../core/utils/array_compare";
export var dataControllerModule = {
    defaultOptions: function() {
        return {
            loadingTimeout: 0,
            dataSource: null,
            cacheEnabled: true,
            repaintChangesOnly: false,
            highlightChanges: false,
            onDataErrorOccurred: null,
            remoteOperations: "auto",
            paging: {
                enabled: true,
                pageSize: void 0,
                pageIndex: void 0
            }
        }
    },
    controllers: {
        data: modules.Controller.inherit({}).include(DataHelperMixin).inherit(function() {
            var changePaging = function(that, optionName, value) {
                var dataSource = that._dataSource;
                if (dataSource) {
                    if (void 0 !== value) {
                        var oldValue = that._getPagingOptionValue(optionName);
                        if (oldValue !== value) {
                            if ("pageSize" === optionName) {
                                dataSource.pageIndex(0)
                            }
                            dataSource[optionName](value);
                            that._skipProcessingPagingChange = true;
                            that.option("paging." + optionName, value);
                            that._skipProcessingPagingChange = false;
                            var pageIndex = dataSource.pageIndex();
                            return dataSource["pageIndex" === optionName ? "load" : "reload"]().done(() => that.pageChanged.fire(pageIndex))
                        }
                        return Deferred().resolve().promise()
                    }
                    return dataSource[optionName]()
                }
                return 0
            };
            var members = {
                init: function() {
                    var that = this;
                    that._items = [];
                    that._cachedProcessedItems = null;
                    that._columnsController = that.getController("columns");
                    that._currentOperationTypes = null;
                    that._dataChangedHandler = e => {
                        that._currentOperationTypes = this._dataSource.operationTypes();
                        that._handleDataChanged(e);
                        that._currentOperationTypes = null
                    };
                    that._columnsChangedHandler = that._handleColumnsChanged.bind(that);
                    that._loadingChangedHandler = that._handleLoadingChanged.bind(that);
                    that._loadErrorHandler = that._handleLoadError.bind(that);
                    that._customizeStoreLoadOptionsHandler = that._handleCustomizeStoreLoadOptions.bind(that);
                    that._changingHandler = that._handleChanging.bind(that);
                    that._dataPushedHandler = that._handleDataPushed.bind(that);
                    that._columnsController.columnsChanged.add(that._columnsChangedHandler);
                    that._isLoading = false;
                    that._isCustomLoading = false;
                    that._repaintChangesOnly = void 0;
                    that._changes = [];
                    that.createAction("onDataErrorOccurred");
                    that.dataErrorOccurred.add((function(error) {
                        return that.executeAction("onDataErrorOccurred", {
                            error: error
                        })
                    }));
                    that._refreshDataSource()
                },
                _getPagingOptionValue: function(optionName) {
                    return this._dataSource[optionName]()
                },
                callbackNames: function() {
                    return ["changed", "loadingChanged", "dataErrorOccurred", "pageChanged", "dataSourceChanged", "pushed"]
                },
                callbackFlags: function(name) {
                    if ("dataErrorOccurred" === name) {
                        return {
                            stopOnFalse: true
                        }
                    }
                },
                publicMethods: function() {
                    return ["beginCustomLoading", "endCustomLoading", "refresh", "filter", "clearFilter", "getCombinedFilter", "keyOf", "byKey", "getDataByKeys", "pageIndex", "pageSize", "pageCount", "totalCount", "_disposeDataSource", "getKeyByRowIndex", "getRowIndexByKey", "getDataSource", "getVisibleRows", "repaintRows"]
                },
                reset: function() {
                    this._columnsController.reset();
                    this._items = [];
                    this._refreshDataSource()
                },
                optionChanged: function(args) {
                    var that = this;
                    var dataSource;

                    function handled() {
                        args.handled = true
                    }
                    if ("dataSource" === args.name && args.name === args.fullName && (args.value === args.previousValue || that.option("columns") && Array.isArray(args.value) && Array.isArray(args.previousValue))) {
                        var _this$_dataSource;
                        var isValueChanged = args.value !== args.previousValue;
                        if (isValueChanged) {
                            var store = that.store();
                            if (store) {
                                store._array = args.value
                            }
                        }
                        handled();
                        var isParasiteChange = Array.isArray(args.value) && !isValueChanged && (null === (_this$_dataSource = this._dataSource) || void 0 === _this$_dataSource ? void 0 : _this$_dataSource.isLoading());
                        if (!isParasiteChange) {
                            that.refresh(that.option("repaintChangesOnly"))
                        }
                        return
                    }
                    switch (args.name) {
                        case "cacheEnabled":
                        case "repaintChangesOnly":
                        case "highlightChanges":
                        case "loadingTimeout":
                            handled();
                            break;
                        case "remoteOperations":
                        case "keyExpr":
                        case "dataSource":
                        case "scrolling":
                            handled();
                            that.reset();
                            break;
                        case "paging":
                            dataSource = that.dataSource();
                            if (dataSource && that._setPagingOptions(dataSource)) {
                                var pageIndex = dataSource.pageIndex();
                                dataSource.load().done(() => that.pageChanged.fire(pageIndex))
                            }
                            handled();
                            break;
                        case "rtlEnabled":
                            that.reset();
                            break;
                        case "columns":
                            dataSource = that.dataSource();
                            if (dataSource && dataSource.isLoading() && args.name === args.fullName) {
                                this._useSortingGroupingFromColumns = true;
                                dataSource.load()
                            }
                            break;
                        default:
                            that.callBase(args)
                    }
                },
                isReady: function() {
                    return !this._isLoading
                },
                getDataSource: function() {
                    return this._dataSource && this._dataSource._dataSource
                },
                getCombinedFilter: function(returnDataField) {
                    return this.combinedFilter(void 0, returnDataField)
                },
                combinedFilter: function(filter, returnDataField) {
                    var dataSource = this._dataSource;
                    var columnsController = this._columnsController;
                    if (dataSource) {
                        if (void 0 === filter) {
                            filter = dataSource.filter()
                        }
                        var additionalFilter = this._calculateAdditionalFilter();
                        if (additionalFilter) {
                            if (columnsController.isDataSourceApplied() || columnsController.isAllDataTypesDefined()) {
                                filter = gridCoreUtils.combineFilters([additionalFilter, filter])
                            }
                        }
                        filter = columnsController.updateFilter(filter, returnDataField || dataSource.remoteOperations().filtering)
                    }
                    return filter
                },
                waitReady: function() {
                    if (this._updateLockCount) {
                        this._readyDeferred = new Deferred;
                        return this._readyDeferred
                    }
                    return when()
                },
                _endUpdateCore: function() {
                    var changes = this._changes;
                    if (changes.length) {
                        this._changes = [];
                        var repaintChangesOnly = changes.every(change => change.repaintChangesOnly);
                        this.updateItems(1 === changes.length ? changes[0] : {
                            repaintChangesOnly: repaintChangesOnly
                        })
                    }
                    if (this._readyDeferred) {
                        this._readyDeferred.resolve();
                        this._readyDeferred = null
                    }
                },
                _handleCustomizeStoreLoadOptions: function(e) {
                    var _storeLoadOptions$fil;
                    var columnsController = this._columnsController;
                    var dataSource = this._dataSource;
                    var storeLoadOptions = e.storeLoadOptions;
                    if (e.isCustomLoading && !storeLoadOptions.isLoadingAll) {
                        return
                    }
                    storeLoadOptions.filter = this.combinedFilter(storeLoadOptions.filter);
                    if (1 === (null === (_storeLoadOptions$fil = storeLoadOptions.filter) || void 0 === _storeLoadOptions$fil ? void 0 : _storeLoadOptions$fil.length) && "!" === storeLoadOptions.filter[0]) {
                        e.data = [];
                        e.extra = e.extra || {};
                        e.extra.totalCount = 0
                    }
                    if (!columnsController.isDataSourceApplied()) {
                        columnsController.updateColumnDataTypes(dataSource)
                    }
                    this._columnsUpdating = true;
                    columnsController.updateSortingGrouping(dataSource, !this._useSortingGroupingFromColumns);
                    this._columnsUpdating = false;
                    storeLoadOptions.sort = columnsController.getSortDataSourceParameters();
                    storeLoadOptions.group = columnsController.getGroupDataSourceParameters();
                    dataSource.sort(storeLoadOptions.sort);
                    dataSource.group(storeLoadOptions.group);
                    storeLoadOptions.sort = columnsController.getSortDataSourceParameters(!dataSource.remoteOperations().sorting);
                    e.group = columnsController.getGroupDataSourceParameters(!dataSource.remoteOperations().grouping)
                },
                _handleColumnsChanged: function(e) {
                    var that = this;
                    var changeTypes = e.changeTypes;
                    var optionNames = e.optionNames;
                    var filterValue;
                    var filterValues;
                    var filterApplied;
                    if (changeTypes.sorting || changeTypes.grouping) {
                        if (that._dataSource && !that._columnsUpdating) {
                            that._dataSource.group(that._columnsController.getGroupDataSourceParameters());
                            that._dataSource.sort(that._columnsController.getSortDataSourceParameters());
                            that.reload()
                        }
                    } else if (changeTypes.columns) {
                        filterValues = that._columnsController.columnOption(e.columnIndex, "filterValues");
                        if (optionNames.filterValues || optionNames.filterType && Array.isArray(filterValues) || optionNames.filterValue || optionNames.selectedFilterOperation || optionNames.allowFiltering) {
                            filterValue = that._columnsController.columnOption(e.columnIndex, "filterValue");
                            if (Array.isArray(filterValues) || void 0 === e.columnIndex || isDefined(filterValue) || !optionNames.selectedFilterOperation || optionNames.filterValue) {
                                that._applyFilter();
                                filterApplied = true
                            }
                        }
                        if (!that._needApplyFilter && !gridCoreUtils.checkChanges(optionNames, ["width", "visibleWidth", "filterValue", "bufferedFilterValue", "selectedFilterOperation", "filterValues", "filterType"])) {
                            that._columnsController.columnsChanged.add((function updateItemsHandler() {
                                that._columnsController.columnsChanged.remove(updateItemsHandler);
                                that.updateItems({
                                    virtualColumnsScrolling: e.changeTypes.virtualColumnsScrolling
                                })
                            }))
                        }
                        if (isDefined(optionNames.visible)) {
                            var column = that._columnsController.columnOption(e.columnIndex);
                            if (column && (isDefined(column.filterValue) || isDefined(column.filterValues))) {
                                that._applyFilter();
                                filterApplied = true
                            }
                        }
                    }
                    if (!filterApplied && changeTypes.filtering) {
                        that.reload()
                    }
                },
                _handleDataChanged: function(e) {
                    var that = this;
                    var dataSource = that._dataSource;
                    var columnsController = that._columnsController;
                    var isAsyncDataSourceApplying = false;
                    this._useSortingGroupingFromColumns = false;
                    if (dataSource && !that._isDataSourceApplying) {
                        that._isDataSourceApplying = true;
                        when(that._columnsController.applyDataSource(dataSource)).done((function() {
                            if (that._isLoading) {
                                that._handleLoadingChanged(false)
                            }
                            if (isAsyncDataSourceApplying && e && e.isDelayed) {
                                e.isDelayed = false
                            }
                            that._isDataSourceApplying = false;
                            var needApplyFilter = that._needApplyFilter;
                            that._needApplyFilter = false;
                            if (needApplyFilter && !that._isAllDataTypesDefined && (additionalFilter = that._calculateAdditionalFilter(), additionalFilter && additionalFilter.length)) {
                                errors.log("W1005", that.component.NAME);
                                that._applyFilter()
                            } else {
                                that.updateItems(e, true)
                            }
                            var additionalFilter
                        })).fail((function() {
                            that._isDataSourceApplying = false
                        }));
                        if (that._isDataSourceApplying) {
                            isAsyncDataSourceApplying = true;
                            that._handleLoadingChanged(true)
                        }
                        that._needApplyFilter = !that._columnsController.isDataSourceApplied();
                        that._isAllDataTypesDefined = columnsController.isAllDataTypesDefined()
                    }
                },
                _handleLoadingChanged: function(isLoading) {
                    this._isLoading = isLoading;
                    this._fireLoadingChanged()
                },
                _handleLoadError: function(e) {
                    this.dataErrorOccurred.fire(e)
                },
                _handleDataPushed: function(changes) {
                    this.pushed.fire(changes)
                },
                fireError: function() {
                    this.dataErrorOccurred.fire(errors.Error.apply(errors, arguments))
                },
                _setPagingOptions: function(dataSource) {
                    var pageIndex = this.option("paging.pageIndex");
                    var pageSize = this.option("paging.pageSize");
                    var pagingEnabled = this.option("paging.enabled");
                    var scrollingMode = this.option("scrolling.mode");
                    var appendMode = "infinite" === scrollingMode;
                    var virtualMode = "virtual" === scrollingMode;
                    var paginate = pagingEnabled || virtualMode || appendMode;
                    var isChanged = false;
                    dataSource.requireTotalCount(!appendMode);
                    if (void 0 !== pagingEnabled && dataSource.paginate() !== paginate) {
                        dataSource.paginate(paginate);
                        isChanged = true
                    }
                    if (void 0 !== pageSize && dataSource.pageSize() !== pageSize) {
                        dataSource.pageSize(pageSize);
                        isChanged = true
                    }
                    if (void 0 !== pageIndex && dataSource.pageIndex() !== pageIndex) {
                        dataSource.pageIndex(pageIndex);
                        isChanged = true
                    }
                    return isChanged
                },
                _getSpecificDataSourceOption: function() {
                    var dataSource = this.option("dataSource");
                    if (Array.isArray(dataSource)) {
                        return {
                            store: {
                                type: "array",
                                data: dataSource,
                                key: this.option("keyExpr")
                            }
                        }
                    }
                    return dataSource
                },
                _initDataSource: function() {
                    var dataSource = this.option("dataSource");
                    var oldDataSource = this._dataSource;
                    this.callBase();
                    dataSource = this._dataSource;
                    this._useSortingGroupingFromColumns = true;
                    this._cachedProcessedItems = null;
                    if (dataSource) {
                        this._setPagingOptions(dataSource);
                        this.setDataSource(dataSource)
                    } else if (oldDataSource) {
                        this.updateItems()
                    }
                },
                _loadDataSource: function() {
                    var dataSource = this._dataSource;
                    var result = new Deferred;
                    when(this._columnsController.refresh(true)).always((function() {
                        if (dataSource) {
                            dataSource.load().done(result.resolve).fail(result.reject)
                        } else {
                            result.resolve()
                        }
                    }));
                    return result.promise()
                },
                _beforeProcessItems: function(items) {
                    return items.slice(0)
                },
                getRowIndexDelta: function() {
                    return 0
                },
                _processItems: function(items, change) {
                    var that = this;
                    var rowIndexDelta = that.getRowIndexDelta();
                    var changeType = change.changeType;
                    var visibleColumns = that._columnsController.getVisibleColumns(null, "loadingAll" === changeType);
                    var visibleItems = that._items;
                    var lastVisibleItem = "append" === changeType && visibleItems.length > 0 ? visibleItems[visibleItems.length - 1] : null;
                    var dataIndex = isDefined(null === lastVisibleItem || void 0 === lastVisibleItem ? void 0 : lastVisibleItem.dataIndex) ? lastVisibleItem.dataIndex + 1 : 0;
                    var options = {
                        visibleColumns: visibleColumns,
                        dataIndex: dataIndex
                    };
                    var result = [];
                    each(items, (function(index, item) {
                        if (isDefined(item)) {
                            options.rowIndex = index - rowIndexDelta;
                            item = that._processItem(item, options);
                            result.push(item)
                        }
                    }));
                    return result
                },
                _processItem: function(item, options) {
                    item = this._generateDataItem(item, options);
                    item = this._processDataItem(item, options);
                    item.dataIndex = options.dataIndex++;
                    return item
                },
                _generateDataItem: function(data) {
                    return {
                        rowType: "data",
                        data: data,
                        key: this.keyOf(data)
                    }
                },
                _processDataItem: function(dataItem, options) {
                    dataItem.values = this.generateDataValues(dataItem.data, options.visibleColumns);
                    return dataItem
                },
                generateDataValues: function(data, columns, isModified) {
                    var values = [];
                    var value;
                    for (var i = 0; i < columns.length; i++) {
                        var column = columns[i];
                        value = isModified ? void 0 : null;
                        if (!column.command) {
                            if (column.calculateCellValue) {
                                value = column.calculateCellValue(data)
                            } else if (column.dataField) {
                                value = data[column.dataField]
                            }
                        }
                        values.push(value)
                    }
                    return values
                },
                _applyChange: function(change) {
                    if ("update" === change.changeType) {
                        this._applyChangeUpdate(change)
                    } else if (this.items().length && change.repaintChangesOnly && "refresh" === change.changeType) {
                        this._applyChangesOnly(change)
                    } else if ("refresh" === change.changeType) {
                        this._applyChangeFull(change)
                    }
                },
                _applyChangeFull: function(change) {
                    this._items = change.items.slice(0)
                },
                _getRowIndices: function(change) {
                    var rowIndices = change.rowIndices.slice(0);
                    var rowIndexDelta = this.getRowIndexDelta();
                    rowIndices.sort((function(a, b) {
                        return a - b
                    }));
                    for (var i = 0; i < rowIndices.length; i++) {
                        var correctedRowIndex = rowIndices[i];
                        if (change.allowInvisibleRowIndices) {
                            correctedRowIndex += rowIndexDelta
                        }
                        if (correctedRowIndex < 0) {
                            rowIndices.splice(i, 1);
                            i--
                        }
                    }
                    return rowIndices
                },
                _applyChangeUpdate: function(change) {
                    var that = this;
                    var items = change.items;
                    var rowIndices = that._getRowIndices(change);
                    var rowIndexDelta = that.getRowIndexDelta();
                    var repaintChangesOnly = that.option("repaintChangesOnly");
                    var prevIndex = -1;
                    var rowIndexCorrection = 0;
                    var changeType;
                    change.items = [];
                    change.rowIndices = [];
                    change.columnIndices = [];
                    change.changeTypes = [];
                    var equalItems = function(item1, item2, strict) {
                        var result = item1 && item2 && equalByValue(item1.key, item2.key);
                        if (result && strict) {
                            result = item1.rowType === item2.rowType && ("detail" !== item2.rowType || item1.isEditing === item2.isEditing)
                        }
                        return result
                    };
                    each(rowIndices, (function(index, rowIndex) {
                        var columnIndices;
                        rowIndex += rowIndexCorrection + rowIndexDelta;
                        if (prevIndex === rowIndex) {
                            return
                        }
                        prevIndex = rowIndex;
                        var oldItem = that._items[rowIndex];
                        var oldNextItem = that._items[rowIndex + 1];
                        var newItem = items[rowIndex];
                        var newNextItem = items[rowIndex + 1];
                        var strict = equalItems(oldItem, oldNextItem) || equalItems(newItem, newNextItem);
                        if (newItem) {
                            newItem.rowIndex = rowIndex;
                            change.items.push(newItem)
                        }
                        if (oldItem && newItem && equalItems(oldItem, newItem, strict)) {
                            changeType = "update";
                            that._items[rowIndex] = newItem;
                            if (oldItem.visible !== newItem.visible) {
                                change.items.splice(-1, 1, {
                                    visible: newItem.visible
                                })
                            } else if (repaintChangesOnly && !change.isFullUpdate) {
                                columnIndices = that._partialUpdateRow(oldItem, newItem, rowIndex - rowIndexDelta)
                            }
                        } else if (newItem && !oldItem || newNextItem && equalItems(oldItem, newNextItem, strict)) {
                            changeType = "insert";
                            that._items.splice(rowIndex, 0, newItem);
                            rowIndexCorrection++
                        } else if (oldItem && !newItem || oldNextItem && equalItems(newItem, oldNextItem, strict)) {
                            changeType = "remove";
                            that._items.splice(rowIndex, 1);
                            rowIndexCorrection--;
                            prevIndex = -1
                        } else if (newItem) {
                            changeType = "update";
                            that._items[rowIndex] = newItem
                        } else {
                            return
                        }
                        change.rowIndices.push(rowIndex - rowIndexDelta);
                        change.changeTypes.push(changeType);
                        change.columnIndices.push(columnIndices)
                    }))
                },
                _isCellChanged: function(oldRow, newRow, visibleRowIndex, columnIndex, isLiveUpdate) {
                    if (JSON.stringify(oldRow.values[columnIndex]) !== JSON.stringify(newRow.values[columnIndex])) {
                        return true
                    }

                    function isCellModified(row, columnIndex) {
                        return row.modifiedValues ? void 0 !== row.modifiedValues[columnIndex] : false
                    }
                    if (isCellModified(oldRow, columnIndex) !== isCellModified(newRow, columnIndex)) {
                        return true
                    }
                    return false
                },
                _getChangedColumnIndices: function(oldItem, newItem, visibleRowIndex, isLiveUpdate) {
                    var columnIndices;
                    if (oldItem.rowType === newItem.rowType) {
                        if ("group" !== newItem.rowType && "groupFooter" !== newItem.rowType) {
                            columnIndices = [];
                            if ("detail" !== newItem.rowType) {
                                for (var columnIndex = 0; columnIndex < oldItem.values.length; columnIndex++) {
                                    if (this._isCellChanged(oldItem, newItem, visibleRowIndex, columnIndex, isLiveUpdate)) {
                                        columnIndices.push(columnIndex)
                                    }
                                }
                            }
                        }
                        if ("group" === newItem.rowType && newItem.isExpanded === oldItem.isExpanded && oldItem.cells) {
                            columnIndices = oldItem.cells.map((cell, index) => {
                                var _cell$column;
                                return "groupExpand" !== (null === (_cell$column = cell.column) || void 0 === _cell$column ? void 0 : _cell$column.type) ? index : -1
                            }).filter(index => index >= 0)
                        }
                    }
                    return columnIndices
                },
                _partialUpdateRow: function(oldItem, newItem, visibleRowIndex, isLiveUpdate) {
                    var _changedColumnIndices;
                    var changedColumnIndices = this._getChangedColumnIndices(oldItem, newItem, visibleRowIndex, isLiveUpdate);
                    if (null !== (_changedColumnIndices = changedColumnIndices) && void 0 !== _changedColumnIndices && _changedColumnIndices.length && this.option("dataRowTemplate")) {
                        changedColumnIndices = void 0
                    }
                    if (changedColumnIndices) {
                        oldItem.cells && oldItem.cells.forEach((function(cell, columnIndex) {
                            var isCellChanged = changedColumnIndices.indexOf(columnIndex) >= 0;
                            if (!isCellChanged && cell && cell.update) {
                                cell.update(newItem)
                            }
                        }));
                        newItem.update = oldItem.update;
                        newItem.watch = oldItem.watch;
                        newItem.cells = oldItem.cells;
                        if (isLiveUpdate) {
                            newItem.oldValues = oldItem.values
                        }
                        oldItem.update && oldItem.update(newItem)
                    }
                    return changedColumnIndices
                },
                _isItemEquals: function(item1, item2) {
                    if (JSON.stringify(item1.values) !== JSON.stringify(item2.values)) {
                        return false
                    }
                    if (["modified", "isNewRow", "removed", "isEditing"].some(field => item1[field] !== item2[field])) {
                        return false
                    }
                    if ("group" === item1.rowType || "groupFooter" === item1.rowType) {
                        var _item1$data, _item2$data, _item1$data2, _item2$data2;
                        var expandedMatch = item1.isExpanded === item2.isExpanded;
                        var summaryCellsMatch = JSON.stringify(item1.summaryCells) === JSON.stringify(item2.summaryCells);
                        var continuationMatch = (null === (_item1$data = item1.data) || void 0 === _item1$data ? void 0 : _item1$data.isContinuation) === (null === (_item2$data = item2.data) || void 0 === _item2$data ? void 0 : _item2$data.isContinuation) && (null === (_item1$data2 = item1.data) || void 0 === _item1$data2 ? void 0 : _item1$data2.isContinuationOnNextPage) === (null === (_item2$data2 = item2.data) || void 0 === _item2$data2 ? void 0 : _item2$data2.isContinuationOnNextPage);
                        if (!expandedMatch || !summaryCellsMatch || !continuationMatch) {
                            return false
                        }
                    }
                    return true
                },
                _applyChangesOnly: function(change) {
                    var rowIndices = [];
                    var columnIndices = [];
                    var changeTypes = [];
                    var items = [];
                    var newIndexByKey = {};

                    function getRowKey(row) {
                        if (row) {
                            return row.rowType + "," + JSON.stringify(row.key)
                        }
                    }
                    var currentItems = this._items;
                    var oldItems = currentItems.slice();
                    change.items.forEach((function(item, index) {
                        var key = getRowKey(item);
                        newIndexByKey[key] = index;
                        item.rowIndex = index
                    }));
                    var result = findChanges(oldItems, change.items, getRowKey, (item1, item2) => {
                        if (!this._isItemEquals(item1, item2)) {
                            return false
                        }
                        if (item1.cells) {
                            item1.update && item1.update(item2);
                            item1.cells.forEach((function(cell) {
                                if (cell && cell.update) {
                                    cell.update(item2, true)
                                }
                            }))
                        }
                        return true
                    });
                    if (!result) {
                        this._applyChangeFull(change);
                        return
                    }
                    result.forEach(change => {
                        switch (change.type) {
                            case "update":
                                var index = change.index;
                                var newItem = change.data;
                                var oldItem = change.oldItem;
                                var changedColumnIndices = this._partialUpdateRow(oldItem, newItem, index, true);
                                rowIndices.push(index);
                                changeTypes.push("update");
                                items.push(newItem);
                                currentItems[index] = newItem;
                                columnIndices.push(changedColumnIndices);
                                break;
                            case "insert":
                                rowIndices.push(change.index);
                                changeTypes.push("insert");
                                items.push(change.data);
                                columnIndices.push(void 0);
                                currentItems.splice(change.index, 0, change.data);
                                break;
                            case "remove":
                                rowIndices.push(change.index);
                                changeTypes.push("remove");
                                currentItems.splice(change.index, 1);
                                items.push(change.oldItem);
                                columnIndices.push(void 0)
                        }
                    });
                    change.repaintChangesOnly = true;
                    change.changeType = "update";
                    change.rowIndices = rowIndices;
                    change.columnIndices = columnIndices;
                    change.changeTypes = changeTypes;
                    change.items = items;
                    if (oldItems.length) {
                        change.isLiveUpdate = true
                    }
                    this._correctRowIndices(rowIndex => {
                        var oldRowIndexOffset = this._rowIndexOffset || 0;
                        var rowIndexOffset = this.getRowIndexOffset();
                        var oldItem = oldItems[rowIndex - oldRowIndexOffset];
                        var key = getRowKey(oldItem);
                        var newVisibleRowIndex = newIndexByKey[key];
                        return newVisibleRowIndex >= 0 ? newVisibleRowIndex + rowIndexOffset - rowIndex : 0
                    })
                },
                _correctRowIndices: noop,
                _afterProcessItems: function(items) {
                    return items
                },
                _updateItemsCore: function(change) {
                    var items;
                    var dataSource = this._dataSource;
                    var changeType = change.changeType || "refresh";
                    change.changeType = changeType;
                    if (dataSource) {
                        var cachedProcessedItems = this._cachedProcessedItems;
                        if (change.useProcessedItemsCache && cachedProcessedItems) {
                            items = cachedProcessedItems
                        } else {
                            items = change.items || dataSource.items();
                            items = this._beforeProcessItems(items);
                            items = this._processItems(items, change);
                            this._cachedProcessedItems = items
                        }
                        items = this._afterProcessItems(items, change);
                        change.items = items;
                        var oldItems = this._items.length === items.length && this._items;
                        this._applyChange(change);
                        var rowIndexDelta = this.getRowIndexDelta();
                        each(this._items, (index, item) => {
                            item.rowIndex = index - rowIndexDelta;
                            if (oldItems) {
                                item.cells = oldItems[index].cells || []
                            }
                            var newItem = items[index];
                            if (newItem) {
                                item.loadIndex = newItem.loadIndex
                            }
                        });
                        this._rowIndexOffset = this.getRowIndexOffset()
                    } else {
                        this._items = []
                    }
                },
                _handleChanging: function(e) {
                    var rows = this.getVisibleRows();
                    var dataSource = this.dataSource();
                    if (dataSource) {
                        e.changes.forEach((function(change) {
                            if ("insert" === change.type && change.index >= 0) {
                                var dataIndex = 0;
                                for (var i = 0; i < change.index; i++) {
                                    var row = rows[i];
                                    if (row && ("data" === row.rowType || "group" === row.rowType)) {
                                        dataIndex++
                                    }
                                }
                                change.index = dataIndex
                            }
                        }))
                    }
                },
                updateItems: function(change, isDataChanged) {
                    change = change || {};
                    if (void 0 !== this._repaintChangesOnly) {
                        change.repaintChangesOnly = change.repaintChangesOnly || this._repaintChangesOnly;
                        change.needUpdateDimensions = change.needUpdateDimensions || this._needUpdateDimensions
                    } else if (change.changes) {
                        change.repaintChangesOnly = this.option("repaintChangesOnly")
                    } else if (isDataChanged) {
                        var operationTypes = this.dataSource().operationTypes();
                        change.repaintChangesOnly = operationTypes && !operationTypes.grouping && !operationTypes.filtering && this.option("repaintChangesOnly");
                        change.isDataChanged = true;
                        if (operationTypes && (operationTypes.reload || operationTypes.paging || operationTypes.groupExpanding)) {
                            change.needUpdateDimensions = true
                        }
                    }
                    if (this._updateLockCount) {
                        this._changes.push(change);
                        return
                    }
                    this._updateItemsCore(change);
                    if (change.cancel) {
                        return
                    }
                    this._fireChanged(change)
                },
                loadingOperationTypes: function() {
                    var dataSource = this.dataSource();
                    return dataSource && dataSource.loadingOperationTypes() || {}
                },
                _fireChanged: function(change) {
                    if (this._currentOperationTypes) {
                        change.operationTypes = this._currentOperationTypes;
                        this._currentOperationTypes = null
                    }
                    deferRender(() => {
                        this.changed.fire(change)
                    })
                },
                isLoading: function() {
                    return this._isLoading || this._isCustomLoading
                },
                _fireLoadingChanged: function() {
                    this.loadingChanged.fire(this.isLoading(), this._loadingText)
                },
                _calculateAdditionalFilter: function() {
                    return null
                },
                _applyFilter: function() {
                    var dataSource = this._dataSource;
                    if (dataSource) {
                        dataSource.pageIndex(0);
                        this._isFilterApplying = true;
                        return this.reload().done(() => {
                            if (this._isFilterApplying) {
                                this.pageChanged.fire()
                            }
                        })
                    }
                },
                resetFilterApplying: function() {
                    this._isFilterApplying = false
                },
                filter: function(filterExpr) {
                    var dataSource = this._dataSource;
                    var filter = dataSource && dataSource.filter();
                    if (0 === arguments.length) {
                        return filter
                    }
                    filterExpr = arguments.length > 1 ? Array.prototype.slice.call(arguments, 0) : filterExpr;
                    if (gridCoreUtils.equalFilterParameters(filter, filterExpr)) {
                        return
                    }
                    if (dataSource) {
                        dataSource.filter(filterExpr)
                    }
                    this._applyFilter()
                },
                clearFilter: function(filterName) {
                    var that = this;
                    var columnsController = that._columnsController;
                    var clearColumnOption = function(optionName) {
                        var columnCount = columnsController.columnCount();
                        for (var index = 0; index < columnCount; index++) {
                            columnsController.columnOption(index, optionName, void 0)
                        }
                    };
                    that.component.beginUpdate();
                    if (arguments.length > 0) {
                        switch (filterName) {
                            case "dataSource":
                                that.filter(null);
                                break;
                            case "search":
                                that.searchByText("");
                                break;
                            case "header":
                                clearColumnOption("filterValues");
                                break;
                            case "row":
                                clearColumnOption("filterValue")
                        }
                    } else {
                        that.filter(null);
                        that.searchByText("");
                        clearColumnOption("filterValue");
                        clearColumnOption("bufferedFilterValue");
                        clearColumnOption("filterValues")
                    }
                    that.component.endUpdate()
                },
                _fireDataSourceChanged: function() {
                    var that = this;
                    that.changed.add((function changedHandler() {
                        that.changed.remove(changedHandler);
                        that.dataSourceChanged.fire()
                    }))
                },
                _getDataSourceAdapter: noop,
                _createDataSourceAdapterCore: function(dataSource, remoteOperations) {
                    var dataSourceAdapterProvider = this._getDataSourceAdapter();
                    var dataSourceAdapter = dataSourceAdapterProvider.create(this.component);
                    dataSourceAdapter.init(dataSource, remoteOperations);
                    return dataSourceAdapter
                },
                isLocalStore: function(store) {
                    store = store || this.store();
                    return store instanceof ArrayStore
                },
                isCustomStore: function(store) {
                    store = store || this.store();
                    return store instanceof CustomStore
                },
                _createDataSourceAdapter: function(dataSource) {
                    var remoteOperations = this.option("remoteOperations");
                    var store = dataSource.store();
                    var enabledRemoteOperations = {
                        filtering: true,
                        sorting: true,
                        paging: true,
                        grouping: true,
                        summary: true
                    };
                    if (remoteOperations && remoteOperations.groupPaging) {
                        remoteOperations = extend({}, enabledRemoteOperations, remoteOperations)
                    }
                    if ("auto" === remoteOperations) {
                        remoteOperations = this.isLocalStore(store) || this.isCustomStore(store) ? {} : {
                            filtering: true,
                            sorting: true,
                            paging: true
                        }
                    }
                    if (true === remoteOperations) {
                        remoteOperations = enabledRemoteOperations
                    }
                    return this._createDataSourceAdapterCore(dataSource, remoteOperations)
                },
                setDataSource: function(dataSource) {
                    var oldDataSource = this._dataSource;
                    if (!dataSource && oldDataSource) {
                        oldDataSource.cancelAll();
                        oldDataSource.changed.remove(this._dataChangedHandler);
                        oldDataSource.loadingChanged.remove(this._loadingChangedHandler);
                        oldDataSource.loadError.remove(this._loadErrorHandler);
                        oldDataSource.customizeStoreLoadOptions.remove(this._customizeStoreLoadOptionsHandler);
                        oldDataSource.changing.remove(this._changingHandler);
                        oldDataSource.pushed.remove(this._dataPushedHandler);
                        oldDataSource.dispose(this._isSharedDataSource)
                    }
                    if (dataSource) {
                        dataSource = this._createDataSourceAdapter(dataSource)
                    }
                    this._dataSource = dataSource;
                    if (dataSource) {
                        this._fireDataSourceChanged();
                        this._isLoading = !dataSource.isLoaded();
                        this._needApplyFilter = true;
                        this._isAllDataTypesDefined = this._columnsController.isAllDataTypesDefined();
                        dataSource.changed.add(this._dataChangedHandler);
                        dataSource.loadingChanged.add(this._loadingChangedHandler);
                        dataSource.loadError.add(this._loadErrorHandler);
                        dataSource.customizeStoreLoadOptions.add(this._customizeStoreLoadOptionsHandler);
                        dataSource.changing.add(this._changingHandler);
                        dataSource.pushed.add(this._dataPushedHandler)
                    }
                },
                items: function() {
                    return this._items
                },
                isEmpty: function() {
                    return !this.items().length
                },
                pageCount: function() {
                    return this._dataSource ? this._dataSource.pageCount() : 1
                },
                dataSource: function() {
                    return this._dataSource
                },
                store: function() {
                    var dataSource = this._dataSource;
                    return dataSource && dataSource.store()
                },
                loadAll: function(data) {
                    var that = this;
                    var d = new Deferred;
                    var dataSource = that._dataSource;
                    if (dataSource) {
                        if (data) {
                            var options = {
                                data: data,
                                isCustomLoading: true,
                                storeLoadOptions: {
                                    isLoadingAll: true
                                },
                                loadOptions: {
                                    filter: that.getCombinedFilter(),
                                    group: dataSource.group(),
                                    sort: dataSource.sort()
                                }
                            };
                            dataSource._handleDataLoaded(options);
                            when(options.data).done((function(data) {
                                data = that._beforeProcessItems(data);
                                d.resolve(that._processItems(data, {
                                    changeType: "loadingAll"
                                }), options.extra && options.extra.summary)
                            })).fail(d.reject)
                        } else if (!dataSource.isLoading()) {
                            var loadOptions = extend({}, dataSource.loadOptions(), {
                                isLoadingAll: true,
                                requireTotalCount: false
                            });
                            dataSource.load(loadOptions).done((function(items, extra) {
                                items = that._beforeProcessItems(items);
                                items = that._processItems(items, {
                                    changeType: "loadingAll"
                                });
                                d.resolve(items, extra && extra.summary)
                            })).fail(d.reject)
                        } else {
                            d.reject()
                        }
                    } else {
                        d.resolve([])
                    }
                    return d
                },
                getKeyByRowIndex: function(rowIndex, byLoaded) {
                    var item = this.items(byLoaded)[rowIndex];
                    if (item) {
                        return item.key
                    }
                },
                getRowIndexByKey: function(key, byLoaded) {
                    return gridCoreUtils.getIndexByKey(key, this.items(byLoaded))
                },
                keyOf: function(data) {
                    var store = this.store();
                    if (store) {
                        return store.keyOf(data)
                    }
                },
                byKey: function(key) {
                    var store = this.store();
                    var rowIndex = this.getRowIndexByKey(key);
                    var result;
                    if (!store) {
                        return
                    }
                    if (rowIndex >= 0) {
                        result = (new Deferred).resolve(this.items()[rowIndex].data)
                    }
                    return result || store.byKey(key)
                },
                key: function() {
                    var store = this.store();
                    if (store) {
                        return store.key()
                    }
                },
                getRowIndexOffset: function() {
                    return 0
                },
                getDataByKeys: function(rowKeys) {
                    var that = this;
                    var result = new Deferred;
                    var deferreds = [];
                    var data = [];
                    each(rowKeys, (function(index, key) {
                        deferreds.push(that.byKey(key).done((function(keyData) {
                            data[index] = keyData
                        })))
                    }));
                    when.apply($, deferreds).always((function() {
                        result.resolve(data)
                    }));
                    return result
                },
                pageIndex: function(value) {
                    return changePaging(this, "pageIndex", value)
                },
                pageSize: function(value) {
                    return changePaging(this, "pageSize", value)
                },
                beginCustomLoading: function(messageText) {
                    this._isCustomLoading = true;
                    this._loadingText = messageText || "";
                    this._fireLoadingChanged()
                },
                endCustomLoading: function() {
                    this._isCustomLoading = false;
                    this._loadingText = void 0;
                    this._fireLoadingChanged()
                },
                refresh: function(options) {
                    if (true === options) {
                        options = {
                            reload: true,
                            changesOnly: true
                        }
                    } else if (!options) {
                        options = {
                            lookup: true,
                            selection: true,
                            reload: true
                        }
                    }
                    var that = this;
                    var dataSource = that.getDataSource();
                    var changesOnly = options.changesOnly;
                    var d = new Deferred;
                    var customizeLoadResult = function() {
                        that._repaintChangesOnly = !!changesOnly
                    };
                    when(!options.lookup || that._columnsController.refresh()).always((function() {
                        if (options.load || options.reload) {
                            dataSource && dataSource.on("customizeLoadResult", customizeLoadResult);
                            when(that.reload(options.reload, changesOnly)).always((function() {
                                dataSource && dataSource.off("customizeLoadResult", customizeLoadResult);
                                that._repaintChangesOnly = void 0
                            })).done(d.resolve).fail(d.reject)
                        } else {
                            that.updateItems({
                                repaintChangesOnly: options.changesOnly
                            });
                            d.resolve()
                        }
                    }));
                    return d.promise()
                },
                getVisibleRows: function() {
                    return this.items()
                },
                _disposeDataSource: function() {
                    this.setDataSource(null)
                },
                dispose: function() {
                    this._disposeDataSource();
                    this.callBase.apply(this, arguments)
                },
                repaintRows: function(rowIndexes, changesOnly) {
                    rowIndexes = Array.isArray(rowIndexes) ? rowIndexes : [rowIndexes];
                    if (rowIndexes.length > 1 || isDefined(rowIndexes[0])) {
                        this.updateItems({
                            changeType: "update",
                            rowIndices: rowIndexes,
                            isFullUpdate: !changesOnly
                        })
                    }
                },
                skipProcessingPagingChange: function(fullName) {
                    return this._skipProcessingPagingChange && ("paging.pageIndex" === fullName || "paging.pageSize" === fullName)
                },
                getUserState: function() {
                    return {
                        searchText: this.option("searchPanel.text"),
                        pageIndex: this.pageIndex(),
                        pageSize: this.pageSize()
                    }
                },
                getCachedStoreData: function() {
                    return this._dataSource && this._dataSource.getCachedStoreData()
                },
                isLastPageLoaded: function() {
                    var pageIndex = this.pageIndex();
                    var pageCount = this.pageCount();
                    return pageIndex === pageCount - 1
                }
            };
            gridCoreUtils.proxyMethod(members, "load");
            gridCoreUtils.proxyMethod(members, "reload");
            gridCoreUtils.proxyMethod(members, "push");
            gridCoreUtils.proxyMethod(members, "itemsCount", 0);
            gridCoreUtils.proxyMethod(members, "totalItemsCount", 0);
            gridCoreUtils.proxyMethod(members, "hasKnownLastPage", true);
            gridCoreUtils.proxyMethod(members, "isLoaded", true);
            gridCoreUtils.proxyMethod(members, "totalCount", 0);
            return members
        }())
    }
};
