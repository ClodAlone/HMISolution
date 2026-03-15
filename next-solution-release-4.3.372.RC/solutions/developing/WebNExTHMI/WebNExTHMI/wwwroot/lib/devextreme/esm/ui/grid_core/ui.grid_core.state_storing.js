/**
 * DevExtreme (esm/ui/grid_core/ui.grid_core.state_storing.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    getKeyHash,
    equalByValue
} from "../../core/utils/common";
import {
    isDefined
} from "../../core/utils/type";
import {
    extend
} from "../../core/utils/extend";
import {
    StateStoringController
} from "./ui.grid_core.state_storing_core";
import {
    Deferred
} from "../../core/utils/deferred";
var getDataState = that => {
    var pagerView = that.getView("pagerView");
    var dataController = that.getController("data");
    var state = {
        allowedPageSizes: pagerView ? pagerView.getPageSizes() : void 0,
        filterPanel: {
            filterEnabled: that.option("filterPanel.filterEnabled")
        },
        filterValue: that.option("filterValue"),
        focusedRowKey: that.option("focusedRowEnabled") ? that.option("focusedRowKey") : void 0
    };
    return extend(state, dataController.getUserState())
};
var processLoadState = that => {
    var columnsController = that.getController("columns");
    var selectionController = that.getController("selection");
    var exportController = that.getController("export");
    var dataController = that.getController("data");
    if (columnsController) {
        columnsController.columnsChanged.add((function() {
            that.updateState({
                columns: columnsController.getUserState()
            })
        }))
    }
    if (selectionController) {
        selectionController.selectionChanged.add((function(e) {
            that.updateState({
                selectedRowKeys: e.selectedRowKeys,
                selectionFilter: e.selectionFilter
            })
        }))
    }
    if (dataController) {
        that._initialPageSize = that.option("paging.pageSize");
        that._initialFilterValue = that.option("filterValue");
        dataController.changed.add((function() {
            var state = getDataState(that);
            that.updateState(state)
        }))
    }
    if (exportController) {
        exportController.selectionOnlyChanged.add((function() {
            that.updateState({
                exportSelectionOnly: exportController.selectionOnly()
            })
        }))
    }
};
var DEFAULT_FILTER_VALUE = null;
var getFilterValue = (that, state) => {
    var filterSyncController = that.getController("filterSync");
    var columnsController = that.getController("columns");
    var hasFilterState = state.columns || void 0 !== state.filterValue;
    if (filterSyncController) {
        if (hasFilterState) {
            return state.filterValue || filterSyncController.getFilterValueFromColumns(state.columns)
        } else {
            return that._initialFilterValue || filterSyncController.getFilterValueFromColumns(columnsController.getColumns())
        }
    }
    return DEFAULT_FILTER_VALUE
};
export var stateStoringModule = {
    defaultOptions: function() {
        return {
            stateStoring: {
                enabled: false,
                storageKey: null,
                type: "localStorage",
                customLoad: null,
                customSave: null,
                savingTimeout: 2e3
            }
        }
    },
    controllers: {
        stateStoring: StateStoringController
    },
    extenders: {
        views: {
            rowsView: {
                init: function() {
                    var that = this;
                    var dataController = that.getController("data");
                    that.callBase();
                    dataController.stateLoaded.add((function() {
                        if (dataController.isLoaded() && !dataController.getDataSource()) {
                            that.setLoading(false);
                            that.renderNoDataText();
                            var columnHeadersView = that.component.getView("columnHeadersView");
                            columnHeadersView && columnHeadersView.render();
                            that.component._fireContentReadyAction()
                        }
                    }))
                }
            }
        },
        controllers: {
            stateStoring: {
                init: function() {
                    this.callBase.apply(this, arguments);
                    processLoadState(this)
                },
                isLoading: function() {
                    return this.callBase() || this.getController("data").isStateLoading()
                },
                state: function(_state) {
                    var result = this.callBase.apply(this, arguments);
                    if (void 0 !== _state) {
                        this.applyState(extend({}, _state))
                    }
                    return result
                },
                updateState: function(state) {
                    if (this.isEnabled()) {
                        var oldState = this.state();
                        var newState = extend({}, oldState, state);
                        var oldStateHash = getKeyHash(oldState);
                        var newStateHash = getKeyHash(newState);
                        if (!equalByValue(oldStateHash, newStateHash)) {
                            extend(this._state, state);
                            this.save()
                        }
                    } else {
                        extend(this._state, state)
                    }
                },
                applyState: function(state) {
                    var allowedPageSizes = state.allowedPageSizes;
                    var searchText = state.searchText;
                    var selectedRowKeys = state.selectedRowKeys;
                    var selectionFilter = state.selectionFilter;
                    var exportController = this.getController("export");
                    var columnsController = this.getController("columns");
                    var dataController = this.getController("data");
                    var scrollingMode = this.option("scrolling.mode");
                    var isVirtualScrollingMode = "virtual" === scrollingMode || "infinite" === scrollingMode;
                    var showPageSizeSelector = true === this.option("pager.visible") && this.option("pager.showPageSizeSelector");
                    this.component.beginUpdate();
                    if (columnsController) {
                        columnsController.setUserState(state.columns)
                    }
                    if (exportController) {
                        exportController.selectionOnly(state.exportSelectionOnly)
                    }
                    if (!this.option("selection.deferred")) {
                        this.option("selectedRowKeys", selectedRowKeys || [])
                    }
                    this.option("selectionFilter", selectionFilter);
                    if (allowedPageSizes && "auto" === this.option("pager.allowedPageSizes")) {
                        this.option("pager").allowedPageSizes = allowedPageSizes
                    }
                    if (this.option("focusedRowEnabled")) {
                        this.option("focusedRowIndex", -1);
                        this.option("focusedRowKey", state.focusedRowKey || null)
                    }
                    this.component.endUpdate();
                    this.option("searchPanel.text", searchText || "");
                    this.option("filterValue", getFilterValue(this, state));
                    this.option("filterPanel.filterEnabled", state.filterPanel ? state.filterPanel.filterEnabled : true);
                    this.option("paging.pageIndex", state.pageIndex || 0);
                    this.option("paging.pageSize", (!isVirtualScrollingMode || showPageSizeSelector) && isDefined(state.pageSize) ? state.pageSize : this._initialPageSize);
                    dataController && dataController.reset()
                }
            },
            columns: {
                getVisibleColumns: function() {
                    var visibleColumns = this.callBase.apply(this, arguments);
                    var stateStoringController = this.getController("stateStoring");
                    return stateStoringController.isEnabled() && !stateStoringController.isLoaded() ? [] : visibleColumns
                }
            },
            data: {
                callbackNames: function() {
                    return this.callBase().concat(["stateLoaded"])
                },
                _refreshDataSource: function() {
                    var callBase = this.callBase;
                    var stateStoringController = this.getController("stateStoring");
                    if (stateStoringController.isEnabled() && !stateStoringController.isLoaded()) {
                        clearTimeout(this._restoreStateTimeoutID);
                        var deferred = new Deferred;
                        this._restoreStateTimeoutID = setTimeout(() => {
                            stateStoringController.load().always(() => {
                                this._restoreStateTimeoutID = null
                            }).done(() => {
                                callBase.call(this);
                                this.stateLoaded.fire();
                                deferred.resolve()
                            }).fail(error => {
                                this.stateLoaded.fire();
                                this._handleLoadError(error || "Unknown error");
                                deferred.reject()
                            })
                        });
                        return deferred.promise()
                    } else if (!this.isStateLoading()) {
                        callBase.call(this)
                    }
                },
                isLoading: function() {
                    var stateStoringController = this.getController("stateStoring");
                    return this.callBase() || stateStoringController.isLoading()
                },
                isStateLoading: function() {
                    return isDefined(this._restoreStateTimeoutID)
                },
                isLoaded: function() {
                    return this.callBase() && !this.isStateLoading()
                },
                dispose: function() {
                    clearTimeout(this._restoreStateTimeoutID);
                    this.callBase()
                }
            },
            selection: {
                _fireSelectionChanged: function(options) {
                    var stateStoringController = this.getController("stateStoring");
                    var isDeferredSelection = this.option("selection.deferred");
                    if (stateStoringController.isLoading() && isDeferredSelection) {
                        return
                    }
                    this.callBase.apply(this, arguments)
                }
            }
        }
    }
};
