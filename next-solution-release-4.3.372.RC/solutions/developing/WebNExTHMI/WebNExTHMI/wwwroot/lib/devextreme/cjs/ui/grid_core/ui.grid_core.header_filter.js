/**
 * DevExtreme (cjs/ui/grid_core/ui.grid_core.header_filter.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.headerFilterModule = void 0;
exports.invertFilterExpression = invertFilterExpression;
var _events_engine = _interopRequireDefault(require("../../events/core/events_engine"));
var _uiGrid_core = _interopRequireDefault(require("./ui.grid_core.modules"));
var _filtering = _interopRequireDefault(require("../shared/filtering"));
var _uiGrid_core2 = _interopRequireDefault(require("./ui.grid_core.utils"));
var _uiGrid_core3 = require("./ui.grid_core.header_filter_core");
var _message = _interopRequireDefault(require("../../localization/message"));
var _click = require("../../events/click");
var _data = require("../../core/utils/data");
var _iterator = require("../../core/utils/iterator");
var _type = require("../../core/utils/type");
var _position = require("../../core/utils/position");
var _extend = require("../../core/utils/extend");
var _utils = require("../../data/data_source/utils");
var _date = _interopRequireDefault(require("../../localization/date"));
var _variable_wrapper = _interopRequireDefault(require("../../core/utils/variable_wrapper"));
var _deferred = require("../../core/utils/deferred");
var _accessibility = require("../shared/accessibility");
var _query = _interopRequireDefault(require("../../data/query"));
var _store_helper = _interopRequireDefault(require("../../data/store_helper"));

function _interopRequireDefault(obj) {
    return obj && obj.__esModule ? obj : {
        default: obj
    }
}

function _extends() {
    _extends = Object.assign || function(target) {
        for (var i = 1; i < arguments.length; i++) {
            var source = arguments[i];
            for (var key in source) {
                if (Object.prototype.hasOwnProperty.call(source, key)) {
                    target[key] = source[key]
                }
            }
        }
        return target
    };
    return _extends.apply(this, arguments)
}
var DATE_INTERVAL_FORMATS = {
    month: function(value) {
        return _date.default.getMonthNames()[value - 1]
    },
    quarter: function(value) {
        return _date.default.format(new Date(2e3, 3 * value - 1), "quarter")
    }
};

function ungroupUTCDates(items, dateParts, dates) {
    dateParts = dateParts || [];
    dates = dates || [];
    items.forEach((function(item) {
        if ((0, _type.isDefined)(item.key)) {
            var isMonthPart = 1 === dateParts.length;
            dateParts.push(isMonthPart ? item.key - 1 : item.key);
            if (item.items) {
                ungroupUTCDates(item.items, dateParts, dates)
            } else {
                var date = new Date(Date.UTC.apply(Date, dateParts));
                dates.push(date)
            }
            dateParts.pop()
        } else {
            dates.push(null)
        }
    }));
    return dates
}

function convertDataFromUTCToLocal(data, column) {
    var dates = ungroupUTCDates(data);
    var query = (0, _query.default)(dates);
    var group = _uiGrid_core2.default.getHeaderFilterGroupParameters(_extends({}, column, {
        calculateCellValue: function(date) {
            return date
        }
    }));
    return _store_helper.default.queryByOptions(query, {
        group: group
    }).toArray()
}

function isUTCFormat(format) {
    return "Z" === (null === format || void 0 === format ? void 0 : format.slice(-1)) || "'Z'" === (null === format || void 0 === format ? void 0 : format.slice(-3))
}
var HeaderFilterController = _uiGrid_core.default.ViewController.inherit({
    init: function() {
        this._columnsController = this.getController("columns");
        this._dataController = this.getController("data");
        this._headerFilterView = this.getView("headerFilterView")
    },
    _updateSelectedState: function(items, column) {
        var i = items.length;
        var isExclude = "exclude" === column.filterType;
        while (i--) {
            var item = items[i];
            if ("items" in items[i]) {
                this._updateSelectedState(items[i].items, column)
            }(0, _uiGrid_core3.updateHeaderFilterItemSelectionState)(item, _uiGrid_core2.default.getIndexByKey(items[i].value, column.filterValues, null) > -1, isExclude)
        }
    },
    _normalizeGroupItem: function(item, currentLevel, options) {
        var value;
        var displayValue;
        var path = options.path;
        var valueSelector = options.valueSelector;
        var displaySelector = options.displaySelector;
        var column = options.column;
        if (valueSelector && displaySelector) {
            value = valueSelector(item);
            displayValue = displaySelector(item)
        } else {
            value = item.key;
            displayValue = value
        }
        if (!(0, _type.isObject)(item)) {
            item = {}
        } else {
            item = (0, _extend.extend)({}, item)
        }
        path.push(value);
        if (1 === path.length) {
            item.value = path[0]
        } else {
            item.value = path.join("/")
        }
        item.text = this.getHeaderItemText(displayValue, column, currentLevel, options.headerFilterOptions);
        return item
    },
    getHeaderItemText: function(displayValue, column, currentLevel, headerFilterOptions) {
        var text = _uiGrid_core2.default.formatValue(displayValue, function(value, column, currentLevel) {
            var groupInterval = _filtering.default.getGroupInterval(column);
            var result = _uiGrid_core2.default.getFormatOptionsByColumn(column, "headerFilter");
            if (groupInterval) {
                result.groupInterval = groupInterval[currentLevel];
                if (_uiGrid_core2.default.isDateType(column.dataType)) {
                    result.format = DATE_INTERVAL_FORMATS[groupInterval[currentLevel]]
                } else if ("number" === column.dataType) {
                    result.getDisplayFormat = function() {
                        var formatOptions = {
                            format: column.format,
                            target: "headerFilter"
                        };
                        var firstValueText = _uiGrid_core2.default.formatValue(value, formatOptions);
                        var secondValue = value + groupInterval[currentLevel];
                        var secondValueText = _uiGrid_core2.default.formatValue(secondValue, formatOptions);
                        return firstValueText && secondValueText ? firstValueText + " - " + secondValueText : ""
                    }
                }
            }
            return result
        }(displayValue, column, currentLevel));
        if (!text) {
            text = headerFilterOptions.texts.emptyValue
        }
        return text
    },
    _processGroupItems: function(groupItems, currentLevel, path, options) {
        var displaySelector;
        var valueSelector;
        var column = options.column;
        var lookup = column.lookup;
        var level = options.level;
        path = path || [];
        currentLevel = currentLevel || 0;
        if (lookup) {
            displaySelector = (0, _data.compileGetter)(lookup.displayExpr);
            valueSelector = (0, _data.compileGetter)(lookup.valueExpr)
        }
        for (var i = 0; i < groupItems.length; i++) {
            groupItems[i] = this._normalizeGroupItem(groupItems[i], currentLevel, {
                column: options.column,
                headerFilterOptions: options.headerFilterOptions,
                displaySelector: displaySelector,
                valueSelector: valueSelector,
                path: path
            });
            if ("items" in groupItems[i]) {
                if (currentLevel === level || !(0, _type.isDefined)(groupItems[i].value)) {
                    delete groupItems[i].items
                } else {
                    this._processGroupItems(groupItems[i].items, currentLevel + 1, path, options)
                }
            }
            path.pop()
        }
    },
    getDataSource: function(column) {
        var _column$headerFilter;
        var that = this;
        var dataSource = that._dataController.dataSource();
        var remoteGrouping = null === dataSource || void 0 === dataSource ? void 0 : dataSource.remoteOperations().grouping;
        var group = _uiGrid_core2.default.getHeaderFilterGroupParameters(column, remoteGrouping);
        var headerFilterDataSource = null === (_column$headerFilter = column.headerFilter) || void 0 === _column$headerFilter ? void 0 : _column$headerFilter.dataSource;
        var headerFilterOptions = that.option("headerFilter");
        var isLookup = false;
        var options = {
            component: that.component
        };
        if (!dataSource) {
            return
        }
        if ((0, _type.isDefined)(headerFilterDataSource) && !(0, _type.isFunction)(headerFilterDataSource)) {
            options.dataSource = (0, _utils.normalizeDataSourceOptions)(headerFilterDataSource)
        } else if (column.lookup) {
            isLookup = true;
            var lookupDataSourceOptions;
            if (column.lookup.items) {
                lookupDataSourceOptions = column.lookup.items
            } else {
                lookupDataSourceOptions = column.lookup.dataSource;
                if ((0, _type.isFunction)(lookupDataSourceOptions) && !_variable_wrapper.default.isWrapped(lookupDataSourceOptions)) {
                    lookupDataSourceOptions = lookupDataSourceOptions({})
                }
            }
            options.dataSource = (0, _utils.normalizeDataSourceOptions)(lookupDataSourceOptions)
        } else {
            var cutoffLevel = Array.isArray(group) ? group.length - 1 : 0;
            that._currentColumn = column;
            var filter = that._dataController.getCombinedFilter();
            that._currentColumn = null;
            options.dataSource = {
                filter: filter,
                group: group,
                useDefaultSearch: true,
                load: function(options) {
                    var d = new _deferred.Deferred;
                    options.dataField = column.dataField || column.name;
                    dataSource.load(options).done((function(data) {
                        var convertUTCDates = remoteGrouping && isUTCFormat(column.serializationFormat) && cutoffLevel > 3;
                        if (convertUTCDates) {
                            data = convertDataFromUTCToLocal(data, column)
                        }
                        that._processGroupItems(data, null, null, {
                            level: cutoffLevel,
                            column: column,
                            headerFilterOptions: headerFilterOptions
                        });
                        d.resolve(data)
                    })).fail(d.reject);
                    return d
                }
            }
        }
        if ((0, _type.isFunction)(headerFilterDataSource)) {
            headerFilterDataSource.call(column, options)
        }
        var origPostProcess = options.dataSource.postProcess;
        options.dataSource.postProcess = function(data) {
            var items = data;
            if (isLookup) {
                if (0 === this.pageIndex() && !this.searchValue()) {
                    items = items.slice(0);
                    items.unshift(null)
                }
                that._processGroupItems(items, null, null, {
                    level: 0,
                    column: column,
                    headerFilterOptions: headerFilterOptions
                })
            }
            items = origPostProcess && origPostProcess.call(this, items) || items;
            that._updateSelectedState(items, column);
            return items
        };
        return options.dataSource
    },
    getCurrentColumn: function() {
        return this._currentColumn
    },
    showHeaderFilterMenu: function(columnIndex, isGroupPanel) {
        var columnsController = this._columnsController;
        var column = (0, _extend.extend)(true, {}, this._columnsController.getColumns()[columnIndex]);
        if (column) {
            var visibleIndex = columnsController.getVisibleIndex(columnIndex);
            var view = isGroupPanel ? this.getView("headerPanel") : this.getView("columnHeadersView");
            var $columnElement = $columnElement || view.getColumnElements().eq(isGroupPanel ? column.groupIndex : visibleIndex);
            this.showHeaderFilterMenuBase({
                columnElement: $columnElement,
                column: column,
                applyFilter: true,
                apply: function() {
                    columnsController.columnOption(columnIndex, {
                        filterValues: this.filterValues,
                        filterType: this.filterType
                    })
                }
            })
        }
    },
    showHeaderFilterMenuBase: function(options) {
        var _this = this;
        var that = this;
        var column = options.column;
        if (column) {
            var groupInterval = _filtering.default.getGroupInterval(column);
            var dataSource = that._dataController.dataSource();
            var remoteFiltering = dataSource && dataSource.remoteOperations().filtering;
            (0, _extend.extend)(options, column, {
                type: groupInterval && groupInterval.length > 1 ? "tree" : "list",
                remoteFiltering: remoteFiltering,
                onShowing: function(e) {
                    var dxResizableInstance = e.component.$overlayContent().dxResizable("instance");
                    dxResizableInstance && dxResizableInstance.option("onResizeEnd", (function(e) {
                        var columnsController = that.getController("columns");
                        var headerFilterByColumn = columnsController.columnOption(options.dataField, "headerFilter");
                        headerFilterByColumn = headerFilterByColumn || {};
                        headerFilterByColumn.width = e.width;
                        headerFilterByColumn.height = e.height;
                        columnsController.columnOption(options.dataField, "headerFilter", headerFilterByColumn, true)
                    }))
                },
                onHidden: function() {
                    return (0, _accessibility.restoreFocus)(_this)
                }
            });
            options.dataSource = that.getDataSource(options);
            if (options.isFilterBuilder) {
                options.dataSource.filter = null;
                options.alignment = "right"
            }
            that._headerFilterView.showHeaderFilterMenu(options.columnElement, options)
        }
    },
    hideHeaderFilterMenu: function() {
        this._headerFilterView.hideHeaderFilterMenu()
    }
});
var ColumnHeadersViewHeaderFilterExtender = (0, _extend.extend)({}, _uiGrid_core3.headerFilterMixin, {
    _renderCellContent: function($cell, options) {
        var $headerFilterIndicator;
        var column = options.column;
        if (!column.command && (0, _uiGrid_core3.allowHeaderFiltering)(column) && this.option("headerFilter.visible") && "header" === options.rowType) {
            $headerFilterIndicator = this._applyColumnState({
                name: "headerFilter",
                rootElement: $cell,
                column: column,
                showColumnLines: this.option("showColumnLines")
            });
            $headerFilterIndicator && this._subscribeToIndicatorEvent($headerFilterIndicator, column, "headerFilter")
        }
        this.callBase($cell, options)
    },
    _subscribeToIndicatorEvent: function($indicator, column, indicatorName) {
        var that = this;
        if ("headerFilter" === indicatorName) {
            _events_engine.default.on($indicator, _click.name, that.createAction((function(e) {
                e.event.stopPropagation();
                that.getController("headerFilter").showHeaderFilterMenu(column.index, false)
            })))
        }
    },
    _updateIndicator: function($cell, column, indicatorName) {
        var $indicator = this.callBase($cell, column, indicatorName);
        $indicator && this._subscribeToIndicatorEvent($indicator, column, indicatorName)
    },
    _updateHeaderFilterIndicators: function() {
        if (this.option("headerFilter.visible")) {
            this._updateIndicators("headerFilter")
        }
    },
    _needUpdateFilterIndicators: function() {
        return true
    },
    _columnOptionChanged: function(e) {
        var optionNames = e.optionNames;
        if (_uiGrid_core2.default.checkChanges(optionNames, ["filterValues", "filterType"])) {
            if (this._needUpdateFilterIndicators()) {
                this._updateHeaderFilterIndicators()
            }
            return
        }
        this.callBase(e)
    }
});
var HeaderPanelHeaderFilterExtender = (0, _extend.extend)({}, _uiGrid_core3.headerFilterMixin, {
    _createGroupPanelItem: function($rootElement, groupColumn) {
        var that = this;
        var $item = that.callBase.apply(that, arguments);
        var $headerFilterIndicator;
        if (!groupColumn.command && (0, _uiGrid_core3.allowHeaderFiltering)(groupColumn) && that.option("headerFilter.visible")) {
            $headerFilterIndicator = that._applyColumnState({
                name: "headerFilter",
                rootElement: $item,
                column: {
                    alignment: (0, _position.getDefaultAlignment)(that.option("rtlEnabled")),
                    filterValues: groupColumn.filterValues,
                    allowHeaderFiltering: true
                },
                showColumnLines: true
            });
            $headerFilterIndicator && _events_engine.default.on($headerFilterIndicator, _click.name, that.createAction((function(e) {
                var event = e.event;
                event.stopPropagation();
                that.getController("headerFilter").showHeaderFilterMenu(groupColumn.index, true)
            })))
        }
        return $item
    }
});

function invertFilterExpression(filter) {
    return ["!", filter]
}
var DataControllerFilterRowExtender = {
    skipCalculateColumnFilters: function() {
        return false
    },
    _calculateAdditionalFilter: function() {
        if (this.skipCalculateColumnFilters()) {
            return this.callBase()
        }
        var filters = [this.callBase()];
        var columns = this._columnsController.getVisibleColumns(null, true);
        var headerFilterController = this.getController("headerFilter");
        var currentColumn = headerFilterController.getCurrentColumn();
        (0, _iterator.each)(columns, (function(_, column) {
            var filter;
            if (currentColumn && currentColumn.index === column.index) {
                return
            }
            if ((0, _uiGrid_core3.allowHeaderFiltering)(column) && column.calculateFilterExpression && Array.isArray(column.filterValues) && column.filterValues.length) {
                var filterValues = [];
                (0, _iterator.each)(column.filterValues, (function(_, filterValue) {
                    if (Array.isArray(filterValue)) {
                        filter = filterValue
                    } else {
                        if (column.deserializeValue && !_uiGrid_core2.default.isDateType(column.dataType) && "number" !== column.dataType) {
                            filterValue = column.deserializeValue(filterValue)
                        }
                        filter = column.createFilterExpression(filterValue, "=", "headerFilter")
                    }
                    if (filter) {
                        filter.columnIndex = column.index
                    }
                    filterValues.push(filter)
                }));
                filterValues = _uiGrid_core2.default.combineFilters(filterValues, "or");
                filters.push("exclude" === column.filterType ? ["!", filterValues] : filterValues)
            }
        }));
        return _uiGrid_core2.default.combineFilters(filters)
    }
};
var headerFilterModule = {
    defaultOptions: function() {
        return {
            headerFilter: {
                visible: false,
                width: 252,
                height: 325,
                allowSearch: false,
                searchTimeout: 500,
                texts: {
                    emptyValue: _message.default.format("dxDataGrid-headerFilterEmptyValue"),
                    ok: _message.default.format("dxDataGrid-headerFilterOK"),
                    cancel: _message.default.format("dxDataGrid-headerFilterCancel")
                }
            }
        }
    },
    controllers: {
        headerFilter: HeaderFilterController
    },
    views: {
        headerFilterView: _uiGrid_core3.HeaderFilterView
    },
    extenders: {
        controllers: {
            data: DataControllerFilterRowExtender
        },
        views: {
            columnHeadersView: ColumnHeadersViewHeaderFilterExtender,
            headerPanel: HeaderPanelHeaderFilterExtender
        }
    }
};
exports.headerFilterModule = headerFilterModule;
