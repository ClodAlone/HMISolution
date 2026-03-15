/**
 * DevExtreme (esm/ui/grid_core/ui.grid_core.focus.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import $ from "../../core/renderer";
import core from "./ui.grid_core.modules";
import {
    each
} from "../../core/utils/iterator";
import gridCoreUtils from "./ui.grid_core.utils";
import {
    equalByValue
} from "../../core/utils/common";
import {
    isDefined,
    isBoolean
} from "../../core/utils/type";
import {
    Deferred,
    when
} from "../../core/utils/deferred";
var ROW_FOCUSED_CLASS = "dx-row-focused";
var FOCUSED_ROW_SELECTOR = ".dx-row." + ROW_FOCUSED_CLASS;
var TABLE_POSTFIX_CLASS = "table";
var CELL_FOCUS_DISABLED_CLASS = "dx-cell-focus-disabled";
var FocusController = core.ViewController.inherit({
    init: function() {
        this._dataController = this.getController("data");
        this._keyboardController = this.getController("keyboardNavigation");
        this.component._optionsByReference.focusedRowKey = true
    },
    optionChanged: function(args) {
        if ("focusedRowIndex" === args.name) {
            var focusedRowKey = this.option("focusedRowKey");
            this._focusRowByIndex(args.value);
            this._triggerFocusedRowChangedIfNeed(focusedRowKey, args.value);
            args.handled = true
        } else if ("focusedRowKey" === args.name) {
            args.handled = true;
            if (Array.isArray(args.value) && JSON.stringify(args.value) === JSON.stringify(args.previousValue)) {
                return
            }
            var focusedRowIndex = this.option("focusedRowIndex");
            this._focusRowByKey(args.value);
            this._triggerFocusedRowChangedIfNeed(args.value, focusedRowIndex)
        } else if ("focusedColumnIndex" === args.name) {
            args.handled = true
        } else if ("focusedRowEnabled" === args.name) {
            args.handled = true
        } else if ("autoNavigateToFocusedRow" === args.name) {
            args.handled = true
        } else {
            this.callBase(args)
        }
    },
    _triggerFocusedRowChangedIfNeed: function(focusedRowKey, focusedRowIndex) {
        var focusedRowIndexByKey = this.getFocusedRowIndexByKey(focusedRowKey);
        if (focusedRowIndex === focusedRowIndexByKey) {
            var rowIndex = this._dataController.getRowIndexByKey(focusedRowKey);
            if (rowIndex >= 0) {
                var $rowElement = $(this.getView("rowsView").getRowElement(rowIndex));
                this.getController("keyboardNavigation")._fireFocusedRowChanged($rowElement, focusedRowIndex)
            }
        }
    },
    isAutoNavigateToFocusedRow: function() {
        return "infinite" !== this.option("scrolling.mode") && this.option("autoNavigateToFocusedRow")
    },
    _focusRowByIndex: function(index, operationTypes) {
        if (!this.option("focusedRowEnabled")) {
            return
        }
        index = void 0 !== index ? index : this.option("focusedRowIndex");
        if (index < 0) {
            if (this.isAutoNavigateToFocusedRow()) {
                this._resetFocusedRow()
            }
        } else {
            this._focusRowByIndexCore(index, operationTypes)
        }
    },
    _focusRowByIndexCore: function(index, operationTypes) {
        var dataController = this.getController("data");
        var pageSize = dataController.pageSize();
        var setKeyByIndex = () => {
            if (this._isValidFocusedRowIndex(index)) {
                var rowIndex = index - dataController.getRowIndexOffset(true);
                if (!operationTypes || operationTypes.paging && !operationTypes.filtering) {
                    var lastItemIndex = dataController._getLastItemIndex();
                    rowIndex = Math.min(rowIndex, lastItemIndex)
                }
                var focusedRowKey = dataController.getKeyByRowIndex(rowIndex, true);
                if (isDefined(focusedRowKey) && !this.isRowFocused(focusedRowKey)) {
                    this.option("focusedRowKey", focusedRowKey)
                }
            }
        };
        if (pageSize >= 0) {
            if (!this._isLocalRowIndex(index)) {
                var pageIndex = Math.floor(index / dataController.pageSize());
                when(dataController.pageIndex(pageIndex), dataController.waitReady()).done(() => {
                    setKeyByIndex()
                })
            } else {
                setKeyByIndex()
            }
        }
    },
    _isLocalRowIndex(index) {
        var dataController = this.getController("data");
        var isVirtualScrolling = this.getController("keyboardNavigation")._isVirtualScrolling();
        if (isVirtualScrolling) {
            var pageIndex = Math.floor(index / dataController.pageSize());
            var virtualItems = dataController.virtualItemsCount();
            var virtualItemsBegin = virtualItems ? virtualItems.begin : -1;
            var visibleRowsCount = dataController.getVisibleRows().length + dataController.getRowIndexOffset();
            var visiblePagesCount = Math.ceil(visibleRowsCount / dataController.pageSize());
            return virtualItemsBegin <= index && visiblePagesCount > pageIndex
        }
        return true
    },
    _setFocusedRowKeyByIndex: function(index) {
        var dataController = this.getController("data");
        if (this._isValidFocusedRowIndex(index)) {
            var rowIndex = Math.min(index - dataController.getRowIndexOffset(), dataController.items().length - 1);
            var focusedRowKey = dataController.getKeyByRowIndex(rowIndex);
            if (isDefined(focusedRowKey) && !this.isRowFocused(focusedRowKey)) {
                this.option("focusedRowKey", focusedRowKey)
            }
        }
    },
    _focusRowByKey: function(key) {
        if (!isDefined(key)) {
            this._resetFocusedRow()
        } else {
            this._navigateToRow(key, true)
        }
    },
    _resetFocusedRow: function() {
        var focusedRowKey = this.option("focusedRowKey");
        var isFocusedRowKeyDefined = isDefined(focusedRowKey);
        if (!isFocusedRowKeyDefined && this.option("focusedRowIndex") < 0) {
            return
        }
        var keyboardController = this.getController("keyboardNavigation");
        if (isFocusedRowKeyDefined) {
            this.option("focusedRowKey", null)
        }
        keyboardController.setFocusedRowIndex(-1);
        this.option("focusedRowIndex", -1);
        this.getController("data").updateItems({
            changeType: "updateFocusedRow",
            focusedRowKey: null
        });
        keyboardController._fireFocusedRowChanged(void 0, -1)
    },
    _isValidFocusedRowIndex: function(rowIndex) {
        var dataController = this.getController("data");
        var row = dataController.getVisibleRows()[rowIndex];
        return !row || "data" === row.rowType || "group" === row.rowType
    },
    publicMethods: function() {
        return ["navigateToRow", "isRowFocused"]
    },
    navigateToRow: function(key) {
        if (!this.isAutoNavigateToFocusedRow()) {
            this.option("focusedRowIndex", -1)
        }
        return this._navigateToRow(key)
    },
    _navigateToRow: function(key, needFocusRow) {
        var that = this;
        var dataController = that.getController("data");
        var isAutoNavigate = that.isAutoNavigateToFocusedRow();
        var d = new Deferred;
        if (void 0 === key || !dataController.dataSource()) {
            return d.reject().promise()
        }
        var rowIndexByKey = that.getFocusedRowIndexByKey(key);
        if (!isAutoNavigate && needFocusRow || rowIndexByKey >= 0) {
            that._navigateTo(key, d, needFocusRow)
        } else {
            dataController.getPageIndexByKey(key).done((function(pageIndex) {
                if (pageIndex < 0) {
                    d.resolve(-1);
                    return
                }
                if (pageIndex === dataController.pageIndex()) {
                    dataController.reload().done((function() {
                        if (that.isRowFocused(key) && dataController.getRowIndexByKey(key) >= 0) {
                            d.resolve(that.getFocusedRowIndexByKey(key))
                        } else {
                            that._navigateTo(key, d, needFocusRow)
                        }
                    })).fail(d.reject)
                } else {
                    dataController.pageIndex(pageIndex).done((function() {
                        that._navigateTo(key, d, needFocusRow)
                    })).fail(d.reject)
                }
            })).fail(d.reject)
        }
        return d.promise()
    },
    _navigateTo: function(key, deferred, needFocusRow) {
        var visibleRowIndex = this.getController("data").getRowIndexByKey(key);
        var isVirtualRowRenderingMode = gridCoreUtils.isVirtualRowRendering(this);
        var isAutoNavigate = this.isAutoNavigateToFocusedRow();
        if (isAutoNavigate && isVirtualRowRenderingMode && visibleRowIndex < 0) {
            this._navigateToVirtualRow(key, deferred, needFocusRow)
        } else {
            this._navigateToVisibleRow(key, deferred, needFocusRow)
        }
    },
    _navigateToVisibleRow: function(key, deferred, needFocusRow) {
        if (needFocusRow) {
            this._triggerUpdateFocusedRow(key, deferred)
        } else {
            var focusedRowIndex = this.getFocusedRowIndexByKey(key);
            this.getView("rowsView").scrollToRowElement(key, deferred).done((function() {
                deferred.resolve(focusedRowIndex)
            }))
        }
    },
    _navigateToVirtualRow: function(key, deferred, needFocusRow) {
        var that = this;
        var dataController = this.getController("data");
        var rowsScrollController = dataController._rowsScrollController;
        var rowIndex = gridCoreUtils.getIndexByKey(key, dataController.items(true));
        var scrollable = that.getView("rowsView").getScrollable();
        if (rowsScrollController && scrollable && rowIndex >= 0) {
            var focusedRowIndex = rowIndex + dataController.getRowIndexOffset(true);
            var offset = rowsScrollController.getItemOffset(focusedRowIndex);
            that.component.on("contentReady", (function triggerUpdateFocusedRow() {
                that.component.off("contentReady", triggerUpdateFocusedRow);
                if (needFocusRow) {
                    that._triggerUpdateFocusedRow(key, deferred)
                } else {
                    deferred.resolve(focusedRowIndex)
                }
            }));
            this.getView("rowsView").scrollTopPosition(offset)
        } else {
            deferred.resolve(-1)
        }
    },
    _triggerUpdateFocusedRow: function(key, deferred) {
        var dataController = this.getController("data");
        var focusedRowIndex = this.getFocusedRowIndexByKey(key);
        if (this._isValidFocusedRowIndex(focusedRowIndex)) {
            var d;
            if (this.option("focusedRowEnabled")) {
                dataController.updateItems({
                    changeType: "updateFocusedRow",
                    focusedRowKey: key
                })
            } else {
                d = this.getView("rowsView").scrollToRowElement(key)
            }
            when(d).done(() => {
                this.getController("keyboardNavigation").setFocusedRowIndex(focusedRowIndex);
                deferred && deferred.resolve(focusedRowIndex)
            })
        } else {
            deferred && deferred.resolve(-1)
        }
    },
    getFocusedRowIndexByKey: function(key) {
        var dataController = this.getController("data");
        var loadedRowIndex = dataController.getRowIndexByKey(key, true);
        return loadedRowIndex >= 0 ? loadedRowIndex + dataController.getRowIndexOffset(true) : -1
    },
    _focusRowByKeyOrIndex: function() {
        var focusedRowKey = this.option("focusedRowKey");
        var currentFocusedRowIndex = this.option("focusedRowIndex");
        var keyboardController = this.getController("keyboardNavigation");
        var dataController = this.getController("data");
        if (isDefined(focusedRowKey)) {
            var visibleRowIndex = dataController.getRowIndexByKey(focusedRowKey);
            if (visibleRowIndex >= 0) {
                if (keyboardController._isVirtualScrolling()) {
                    currentFocusedRowIndex = visibleRowIndex + dataController.getRowIndexOffset()
                }
                keyboardController.setFocusedRowIndex(currentFocusedRowIndex);
                this._triggerUpdateFocusedRow(focusedRowKey)
            } else {
                this._navigateToRow(focusedRowKey, true).done(focusedRowIndex => {
                    if (currentFocusedRowIndex >= 0 && focusedRowIndex < 0) {
                        this._focusRowByIndex()
                    } else if (currentFocusedRowIndex < 0 && focusedRowIndex >= 0) {
                        keyboardController.setFocusedRowIndex(focusedRowIndex)
                    }
                })
            }
        } else if (currentFocusedRowIndex >= 0) {
            this.getController("focus")._focusRowByIndex(currentFocusedRowIndex)
        }
    },
    isRowFocused: function(key) {
        var focusedRowKey = this.option("focusedRowKey");
        if (isDefined(focusedRowKey)) {
            return equalByValue(key, this.option("focusedRowKey"))
        }
    },
    updateFocusedRow: function(change) {
        var that = this;
        var focusedRowIndex = that._dataController.getRowIndexByKey(change.focusedRowKey);
        var rowsView = that.getView("rowsView");
        var $tableElement;
        each(rowsView.getTableElements(), (function(index, element) {
            var isMainTable = 0 === index;
            $tableElement = $(element);
            that._clearPreviousFocusedRow($tableElement, focusedRowIndex);
            that._prepareFocusedRow({
                changedItem: that._dataController.getVisibleRows()[focusedRowIndex],
                $tableElement: $tableElement,
                focusedRowIndex: focusedRowIndex,
                isMainTable: isMainTable
            })
        }))
    },
    _clearPreviousFocusedRow: function($tableElement, focusedRowIndex) {
        var $prevRowFocusedElement = $tableElement.find(FOCUSED_ROW_SELECTOR).filter((_, focusedRow) => {
            var $focusedRowTable = $(focusedRow).closest(".".concat(this.addWidgetPrefix(TABLE_POSTFIX_CLASS)));
            return $tableElement.is($focusedRowTable)
        });
        $prevRowFocusedElement.removeClass(ROW_FOCUSED_CLASS).removeClass(CELL_FOCUS_DISABLED_CLASS).removeAttr("tabindex");
        $prevRowFocusedElement.children("td").removeAttr("tabindex");
        if (0 !== focusedRowIndex) {
            var $firstRow = $(this.getView("rowsView").getRowElement(0));
            $firstRow.removeClass(CELL_FOCUS_DISABLED_CLASS).removeAttr("tabIndex")
        }
    },
    _prepareFocusedRow: function(options) {
        var $row;
        var changedItem = options.changedItem;
        if (changedItem && ("data" === changedItem.rowType || "group" === changedItem.rowType)) {
            var focusedRowIndex = options.focusedRowIndex;
            var $tableElement = options.$tableElement;
            var isMainTable = options.isMainTable;
            var tabIndex = this.option("tabindex") || 0;
            var rowsView = this.getView("rowsView");
            $row = $(rowsView._getRowElements($tableElement).eq(focusedRowIndex));
            $row.addClass(ROW_FOCUSED_CLASS).attr("tabindex", tabIndex);
            if (isMainTable) {
                rowsView.scrollToElementVertically($row)
            }
        }
        return $row
    }
});
export var focusModule = {
    defaultOptions: function() {
        return {
            focusedRowEnabled: false,
            autoNavigateToFocusedRow: true,
            focusedRowKey: null,
            focusedRowIndex: -1,
            focusedColumnIndex: -1
        }
    },
    controllers: {
        focus: FocusController
    },
    extenders: {
        controllers: {
            keyboardNavigation: {
                init: function() {
                    var rowIndex = this.option("focusedRowIndex");
                    var columnIndex = this.option("focusedColumnIndex");
                    this.createAction("onFocusedRowChanging", {
                        excludeValidators: ["disabled", "readOnly"]
                    });
                    this.createAction("onFocusedRowChanged", {
                        excludeValidators: ["disabled", "readOnly"]
                    });
                    this.createAction("onFocusedCellChanging", {
                        excludeValidators: ["disabled", "readOnly"]
                    });
                    this.createAction("onFocusedCellChanged", {
                        excludeValidators: ["disabled", "readOnly"]
                    });
                    this.callBase();
                    this.setRowFocusType();
                    this._focusedCellPosition = {};
                    if (isDefined(rowIndex)) {
                        this._focusedCellPosition.rowIndex = this.option("focusedRowIndex")
                    }
                    if (isDefined(columnIndex)) {
                        this._focusedCellPosition.columnIndex = this.option("focusedColumnIndex")
                    }
                },
                setFocusedRowIndex: function(rowIndex) {
                    var dataController = this.getController("data");
                    this.callBase(rowIndex);
                    var visibleRowIndex = rowIndex - dataController.getRowIndexOffset();
                    var visibleRow = dataController.getVisibleRows()[visibleRowIndex];
                    if (!visibleRow || !visibleRow.isNewRow) {
                        this.option("focusedRowIndex", rowIndex)
                    }
                },
                setFocusedColumnIndex: function(columnIndex) {
                    this.callBase(columnIndex);
                    this.option("focusedColumnIndex", columnIndex)
                },
                _escapeKeyHandler: function(eventArgs, isEditing) {
                    if (isEditing || !this.option("focusedRowEnabled")) {
                        this.callBase(eventArgs, isEditing);
                        return
                    }
                    if (this.isCellFocusType()) {
                        this.setRowFocusType();
                        this._focus(this._getCellElementFromTarget(eventArgs.originalEvent.target), true)
                    }
                },
                _updateFocusedCellPosition: function($cell, direction) {
                    var prevRowIndex = this.option("focusedRowIndex");
                    var prevColumnIndex = this.option("focusedColumnIndex");
                    var position = this.callBase($cell, direction);
                    if (position && position.columnIndex >= 0) {
                        this._fireFocusedCellChanged($cell, prevColumnIndex, prevRowIndex)
                    }
                }
            },
            editorFactory: {
                renderFocusOverlay: function($element, hideBorder) {
                    var keyboardController = this.getController("keyboardNavigation");
                    var focusedRowEnabled = this.option("focusedRowEnabled");
                    var editingController = this.getController("editing");
                    var $cell;
                    if (!focusedRowEnabled || !(null !== keyboardController && void 0 !== keyboardController && keyboardController.isRowFocusType()) || editingController.isEditing()) {
                        this.callBase($element, hideBorder)
                    } else if (focusedRowEnabled) {
                        var isRowElement = "row" === keyboardController._getElementType($element);
                        if (isRowElement && !$element.hasClass(ROW_FOCUSED_CLASS)) {
                            $cell = keyboardController.getFirstValidCellInRow($element);
                            keyboardController.focus($cell)
                        }
                    }
                }
            },
            columns: {
                getSortDataSourceParameters: function(_, sortByKey) {
                    var result = this.callBase.apply(this, arguments);
                    var dataController = this.getController("data");
                    var dataSource = dataController._dataSource;
                    var store = dataController.store();
                    var key = store && store.key();
                    var remoteOperations = dataSource && dataSource.remoteOperations() || {};
                    var isLocalOperations = Object.keys(remoteOperations).every(operationName => !remoteOperations[operationName]);
                    if (key && (this.option("focusedRowEnabled") && false !== this.getController("focus").isAutoNavigateToFocusedRow() || sortByKey)) {
                        key = Array.isArray(key) ? key : [key];
                        var notSortedKeys = key.filter(key => !this.columnOption(key, "sortOrder"));
                        if (notSortedKeys.length) {
                            result = result || [];
                            if (isLocalOperations) {
                                result.push({
                                    selector: dataSource.getDataIndexGetter(),
                                    desc: false
                                })
                            } else {
                                notSortedKeys.forEach(notSortedKey => result.push({
                                    selector: notSortedKey,
                                    desc: false
                                }))
                            }
                        }
                    }
                    return result
                }
            },
            data: {
                _applyChange: function(change) {
                    if (change && "updateFocusedRow" === change.changeType) {
                        return
                    }
                    return this.callBase.apply(this, arguments)
                },
                _fireChanged: function(e) {
                    this.callBase(e);
                    if (this.option("focusedRowEnabled") && this._dataSource) {
                        var isPartialUpdate = "update" === e.changeType && e.repaintChangesOnly;
                        var isPartialUpdateWithDeleting = isPartialUpdate && e.changeTypes && e.changeTypes.indexOf("remove") >= 0;
                        if ("refresh" === e.changeType && e.items.length || isPartialUpdateWithDeleting) {
                            this._updatePageIndexes();
                            this.processUpdateFocusedRow(e)
                        } else if ("append" === e.changeType || "prepend" === e.changeType) {
                            this._updatePageIndexes()
                        } else if ("update" === e.changeType && e.repaintChangesOnly) {
                            this.processUpdateFocusedRow(e)
                        }
                    }
                },
                _updatePageIndexes: function() {
                    var prevRenderingPageIndex = this._lastRenderingPageIndex || 0;
                    var renderingPageIndex = this._rowsScrollController ? this._rowsScrollController.pageIndex() : 0;
                    this._lastRenderingPageIndex = renderingPageIndex;
                    this._isPagingByRendering = renderingPageIndex !== prevRenderingPageIndex
                },
                isPagingByRendering: function() {
                    return this._isPagingByRendering
                },
                processUpdateFocusedRow: function(e) {
                    var operationTypes = e.operationTypes || {};
                    var focusController = this.getController("focus");
                    var {
                        reload: reload,
                        fullReload: fullReload
                    } = operationTypes;
                    var keyboardController = this.getController("keyboardNavigation");
                    var isVirtualScrolling = keyboardController._isVirtualScrolling();
                    var focusedRowKey = this.option("focusedRowKey");
                    var isAutoNavigate = focusController.isAutoNavigateToFocusedRow();
                    if (reload && !fullReload && isDefined(focusedRowKey)) {
                        focusController._navigateToRow(focusedRowKey, true).done((function(focusedRowIndex) {
                            if (focusedRowIndex < 0) {
                                focusController._focusRowByIndex(void 0, operationTypes)
                            }
                        }))
                    } else if (operationTypes.paging && !isVirtualScrolling) {
                        if (isAutoNavigate) {
                            var rowIndexByKey = this.getRowIndexByKey(focusedRowKey);
                            var isValidRowIndexByKey = rowIndexByKey >= 0;
                            var focusedRowIndex = this.option("focusedRowIndex");
                            var needFocusRowByIndex = focusedRowIndex >= 0 && (focusedRowIndex === rowIndexByKey || !isValidRowIndexByKey);
                            if (needFocusRowByIndex) {
                                focusController._focusRowByIndex(void 0, operationTypes)
                            }
                        } else if (this.getRowIndexByKey(focusedRowKey) < 0) {
                            this.option("focusedRowIndex", -1)
                        }
                    } else if (operationTypes.fullReload) {
                        focusController._focusRowByKeyOrIndex()
                    }
                },
                getPageIndexByKey: function(key) {
                    var that = this;
                    var d = new Deferred;
                    that.getGlobalRowIndexByKey(key).done((function(globalIndex) {
                        d.resolve(globalIndex >= 0 ? Math.floor(globalIndex / that.pageSize()) : -1)
                    })).fail(d.reject);
                    return d.promise()
                },
                getGlobalRowIndexByKey: function(key) {
                    if (this._dataSource.group()) {
                        return this._calculateGlobalRowIndexByGroupedData(key)
                    }
                    return this._calculateGlobalRowIndexByFlatData(key)
                },
                _calculateGlobalRowIndexByFlatData: function(key, groupFilter, useGroup) {
                    var that = this;
                    var deferred = new Deferred;
                    var dataSource = that._dataSource;
                    if (Array.isArray(key)) {
                        return deferred.resolve(-1).promise()
                    }
                    var filter = that._generateFilterByKey(key);
                    dataSource.load({
                        filter: that._concatWithCombinedFilter(filter),
                        skip: 0,
                        take: 1
                    }).done((function(data) {
                        if (data.length > 0) {
                            filter = that._generateOperationFilterByKey(key, data[0], useGroup);
                            dataSource.load({
                                filter: that._concatWithCombinedFilter(filter, groupFilter),
                                skip: 0,
                                take: 1,
                                requireTotalCount: true
                            }).done((function(_, extra) {
                                deferred.resolve(extra.totalCount)
                            }))
                        } else {
                            deferred.resolve(-1)
                        }
                    }));
                    return deferred.promise()
                },
                _concatWithCombinedFilter: function(filter, groupFilter) {
                    var combinedFilter = this.getCombinedFilter();
                    return gridCoreUtils.combineFilters([filter, combinedFilter, groupFilter])
                },
                _generateBooleanFilter: function(selector, value, sortInfo) {
                    var result;
                    if (false === value) {
                        result = [selector, "=", sortInfo.desc ? true : null]
                    } else if (true === value ? !sortInfo.desc : sortInfo.desc) {
                        result = [selector, "<>", value]
                    }
                    return result
                },
                _generateOperationFilterByKey: function(key, rowData, useGroup) {
                    var that = this;
                    var dataSource = that._dataSource;
                    var filter = that._generateFilterByKey(key, "<");
                    var sort = that._columnsController.getSortDataSourceParameters(!dataSource.remoteOperations().filtering, true);
                    if (useGroup) {
                        var group = that._columnsController.getGroupDataSourceParameters(!dataSource.remoteOperations().filtering);
                        if (group) {
                            sort = sort ? group.concat(sort) : group
                        }
                    }
                    if (sort) {
                        sort.slice().reverse().forEach((function(sortInfo) {
                            var selector = sortInfo.selector;
                            var getter;
                            if ("function" === typeof selector) {
                                getter = selector
                            } else {
                                getter = that._columnsController.columnOption(selector, "selector")
                            }
                            var value = getter ? getter(rowData) : rowData[selector];
                            filter = [
                                [selector, "=", value], "and", filter
                            ];
                            if (null === value || isBoolean(value)) {
                                var booleanFilter = that._generateBooleanFilter(selector, value, sortInfo);
                                if (booleanFilter) {
                                    filter = [booleanFilter, "or", filter]
                                }
                            } else {
                                var filterOperation = sortInfo.desc ? ">" : "<";
                                var sortFilter = [selector, filterOperation, value];
                                if (!sortInfo.desc) {
                                    sortFilter = [sortFilter, "or", [selector, "=", null]]
                                }
                                filter = [sortFilter, "or", filter]
                            }
                        }))
                    }
                    return filter
                },
                _generateFilterByKey: function(key, operation) {
                    var dataSourceKey = this._dataSource.key();
                    var filter = [];
                    if (!operation) {
                        operation = "="
                    }
                    if (Array.isArray(dataSourceKey)) {
                        for (var i = 0; i < dataSourceKey.length; ++i) {
                            var keyPart = key[dataSourceKey[i]];
                            if (keyPart) {
                                if (filter.length > 0) {
                                    filter.push("and")
                                }
                                filter.push([dataSourceKey[i], operation, keyPart])
                            }
                        }
                    } else {
                        filter = [dataSourceKey, operation, key]
                    }
                    return filter
                },
                _getLastItemIndex: function() {
                    return this.items(true).length - 1
                }
            },
            editing: {
                _deleteRowCore: function(rowIndex) {
                    var deferred = this.callBase.apply(this, arguments);
                    var dataController = this.getController("data");
                    var rowKey = dataController.getKeyByRowIndex(rowIndex);
                    deferred.done(() => {
                        var rowIndex = dataController.getRowIndexByKey(rowKey);
                        var visibleRows = dataController.getVisibleRows();
                        if (-1 === rowIndex && !visibleRows.length) {
                            this.getController("focus")._resetFocusedRow()
                        }
                    })
                }
            }
        },
        views: {
            rowsView: {
                _createRow: function(row) {
                    var $row = this.callBase.apply(this, arguments);
                    if (this.option("focusedRowEnabled") && row) {
                        if (this.getController("focus").isRowFocused(row.key)) {
                            $row.addClass(ROW_FOCUSED_CLASS)
                        }
                    }
                    return $row
                },
                _checkRowKeys: function(options) {
                    this.callBase.apply(this, arguments);
                    if (this.option("focusedRowEnabled") && this.option("dataSource")) {
                        var store = this._dataController.store();
                        if (store && !store.key()) {
                            this._dataController.fireError("E1042", "Row focusing")
                        }
                    }
                },
                _update: function(change) {
                    if ("updateFocusedRow" === change.changeType) {
                        if (this.option("focusedRowEnabled")) {
                            this.getController("focus").updateFocusedRow(change)
                        }
                    } else {
                        this.callBase(change)
                    }
                },
                updateFocusElementTabIndex: function($cellElements, preventScroll) {
                    var rowIndex = this.getController("keyboardNavigation").getVisibleRowIndex();
                    var row = this._dataController.getVisibleRows()[rowIndex];
                    if (this.option("focusedRowEnabled") && !(null !== row && void 0 !== row && row.isNewRow)) {
                        this._setFocusedRowElementTabIndex(preventScroll)
                    } else {
                        this.callBase($cellElements)
                    }
                },
                _setFocusedRowElementTabIndex: function(preventScroll) {
                    var focusedRowKey = this.option("focusedRowKey");
                    var tabIndex = this.option("tabIndex") || 0;
                    var dataController = this._dataController;
                    var columnsController = this._columnsController;
                    var rowIndex = dataController.getRowIndexByKey(focusedRowKey);
                    var columnIndex = this.option("focusedColumnIndex");
                    var $row = this._findRowElementForTabIndex();
                    if (!isDefined(this._scrollToFocusOnResize)) {
                        this._scrollToFocusOnResize = () => {
                            this.scrollToElementVertically(this._findRowElementForTabIndex());
                            this.resizeCompleted.remove(this._scrollToFocusOnResize)
                        }
                    }
                    $row.attr("tabIndex", tabIndex);
                    if (rowIndex >= 0 && !preventScroll) {
                        if (columnIndex < 0) {
                            columnIndex = 0
                        }
                        rowIndex += dataController.getRowIndexOffset();
                        columnIndex += columnsController.getColumnIndexOffset();
                        this.getController("keyboardNavigation").setFocusedCellPosition(rowIndex, columnIndex);
                        if (this.getController("focus").isAutoNavigateToFocusedRow()) {
                            var dataSource = dataController.dataSource();
                            var operationTypes = dataSource && dataSource.operationTypes();
                            if (operationTypes && !operationTypes.paging && !dataController.isPagingByRendering()) {
                                this.resizeCompleted.remove(this._scrollToFocusOnResize);
                                this.resizeCompleted.add(this._scrollToFocusOnResize)
                            }
                        }
                    }
                },
                _findRowElementForTabIndex: function() {
                    var focusedRowKey = this.option("focusedRowKey");
                    var rowIndex = this._dataController.getRowIndexByKey(focusedRowKey);
                    return $(this.getRowElement(rowIndex >= 0 ? rowIndex : 0))
                },
                scrollToRowElement: function(key) {
                    var rowIndex = this.getController("data").getRowIndexByKey(key);
                    var $row = $(this.getRow(rowIndex));
                    return this.scrollToElementVertically($row)
                },
                scrollToElementVertically: function($row) {
                    var scrollable = this.getScrollable();
                    if (scrollable && $row.length) {
                        var position = scrollable.getScrollElementPosition($row, "vertical");
                        return this.scrollTopPosition(position)
                    }
                    return (new Deferred).resolve()
                },
                scrollTopPosition: function(scrollTop) {
                    var d = new Deferred;
                    var scrollable = this.getScrollable();
                    if (scrollable) {
                        var currentScrollTop = scrollable.scrollTop();
                        var scrollHandler = () => {
                            scrollable.off("scroll", scrollHandler);
                            d.resolve()
                        };
                        if (scrollTop !== currentScrollTop) {
                            scrollable.on("scroll", scrollHandler);
                            this._dataController.resetFilterApplying();
                            scrollable.scrollTo({
                                top: scrollTop
                            });
                            return d.promise()
                        }
                    }
                    return d.resolve()
                }
            }
        }
    }
};
