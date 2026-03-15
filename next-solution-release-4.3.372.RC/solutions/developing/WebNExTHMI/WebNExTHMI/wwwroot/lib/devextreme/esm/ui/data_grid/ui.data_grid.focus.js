/**
 * DevExtreme (esm/ui/data_grid/ui.data_grid.focus.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import gridCore from "./ui.data_grid.core";
import {
    focusModule
} from "../grid_core/ui.grid_core.focus";
import {
    Deferred
} from "../../core/utils/deferred";
import {
    isDefined
} from "../../core/utils/type";
import {
    equalByValue
} from "../../core/utils/common";
import {
    createGroupFilter
} from "./ui.data_grid.utils";
import {
    compileGetter
} from "../../core/utils/data";
import {
    extend
} from "../../core/utils/extend";
var MAX_SAFE_INTEGER = Number.MAX_SAFE_INTEGER || 9007199254740991;
gridCore.registerModule("focus", extend(true, {}, focusModule, {
    extenders: {
        controllers: {
            data: {
                changeRowExpand: function(path, isRowClick) {
                    if (this.option("focusedRowEnabled") && Array.isArray(path) && this.isRowExpanded(path)) {
                        var keyboardNavigation = this.getController("keyboardNavigation");
                        if ((!isRowClick || !keyboardNavigation.isKeyboardEnabled()) && this._isFocusedRowInsideGroup(path)) {
                            this.option("focusedRowKey", path)
                        }
                    }
                    return this.callBase.apply(this, arguments)
                },
                _isFocusedRowInsideGroup: function(path) {
                    var columnsController = this.getController("columns");
                    var focusedRowKey = this.option("focusedRowKey");
                    var rowIndex = this.getRowIndexByKey(focusedRowKey);
                    var focusedRow = rowIndex >= 0 && this.getVisibleRows()[rowIndex];
                    var groups = columnsController.getGroupDataSourceParameters(true);
                    if (focusedRow) {
                        for (var i = 0; i < path.length; ++i) {
                            var getter = compileGetter(groups[i] && groups[i].selector);
                            if (getter(focusedRow.data) !== path[i]) {
                                return false
                            }
                        }
                    }
                    return true
                },
                _getGroupPath: function(group) {
                    var groupPath = [group.key];
                    var items = group.items;
                    while (items && items[0]) {
                        var item = items[0];
                        if (void 0 !== item.key) {
                            groupPath.push(item.key)
                        }
                        items = item.items
                    }
                    return groupPath
                },
                _expandGroupByPath: function(that, groupPath, level) {
                    var d = new Deferred;
                    level++;
                    that.expandRow(groupPath.slice(0, level)).done((function() {
                        if (level === groupPath.length) {
                            d.resolve()
                        } else {
                            that._expandGroupByPath(that, groupPath, level).done(d.resolve).fail(d.reject)
                        }
                    })).fail(d.reject);
                    return d.promise()
                },
                _calculateGlobalRowIndexByGroupedData: function(key) {
                    var that = this;
                    var dataSource = that._dataSource;
                    var filter = that._generateFilterByKey(key);
                    var deferred = new Deferred;
                    var isGroupKey = Array.isArray(key);
                    var group = dataSource.group();
                    if (isGroupKey) {
                        return deferred.resolve(-1).promise()
                    }
                    if (!dataSource._grouping._updatePagingOptions) {
                        that._calculateGlobalRowIndexByFlatData(key, null, true).done(deferred.resolve).fail(deferred.reject);
                        return deferred
                    }
                    dataSource.load({
                        filter: that._concatWithCombinedFilter(filter),
                        group: group
                    }).done((function(data) {
                        if (!data || 0 === data.length || !isDefined(data[0].key) || -1 === data[0].key) {
                            return deferred.resolve(-1).promise()
                        }
                        var groupPath = that._getGroupPath(data[0]);
                        that._expandGroupByPath(that, groupPath, 0).done((function() {
                            that._calculateExpandedRowGlobalIndex(deferred, key, groupPath, group)
                        })).fail(deferred.reject)
                    })).fail(deferred.reject);
                    return deferred.promise()
                },
                _calculateExpandedRowGlobalIndex: function(deferred, key, groupPath, group) {
                    var groupFilter = createGroupFilter(groupPath, {
                        group: group
                    });
                    var dataSource = this._dataSource;
                    var scrollingMode = this.option("scrolling.mode");
                    var isVirtualScrolling = "virtual" === scrollingMode || "infinite" === scrollingMode;
                    var pageSize = dataSource.pageSize();
                    var groupOffset;
                    dataSource._grouping._updatePagingOptions({
                        skip: 0,
                        take: MAX_SAFE_INTEGER
                    }, (function(groupInfo, totalOffset) {
                        if (equalByValue(groupInfo.path, groupPath)) {
                            groupOffset = totalOffset
                        }
                    }));
                    this._calculateGlobalRowIndexByFlatData(key, groupFilter).done((function(dataOffset) {
                        var count;
                        var groupContinuationCount;
                        if (dataOffset < 0) {
                            deferred.resolve(-1);
                            return
                        }
                        var currentPageOffset = groupOffset % pageSize || pageSize;
                        count = currentPageOffset + dataOffset - groupPath.length;
                        if (isVirtualScrolling) {
                            groupContinuationCount = 0
                        } else {
                            groupContinuationCount = Math.floor(count / (pageSize - groupPath.length)) * groupPath.length
                        }
                        count = groupOffset + dataOffset + groupContinuationCount;
                        deferred.resolve(count)
                    })).fail(deferred.reject)
                }
            }
        }
    }
}));
