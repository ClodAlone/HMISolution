/**
 * DevExtreme (cjs/ui/grid_core/ui.grid_core.utils.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.default = void 0;
var _size = require("../../core/utils/size");
var _renderer = _interopRequireDefault(require("../../core/renderer"));
var _type = require("../../core/utils/type");
var _deferred = require("../../core/utils/deferred");
var _filtering = _interopRequireDefault(require("../shared/filtering"));
var _string = require("../../core/utils/string");
var _iterator = require("../../core/utils/iterator");
var _extend = require("../../core/utils/extend");
var _position = require("../../core/utils/position");
var _data = require("../../core/utils/data");
var _common = require("../../core/utils/common");
var _load_panel = _interopRequireDefault(require("../load_panel"));
var _utils = require("../../data/utils");
var _format_helper = _interopRequireDefault(require("../../format_helper"));
var _window = require("../../core/utils/window");
var _events_engine = _interopRequireDefault(require("../../events/core/events_engine"));

function _interopRequireDefault(obj) {
    return obj && obj.__esModule ? obj : {
        default: obj
    }
}
var DATAGRID_SELECTION_DISABLED_CLASS = "dx-selection-disabled";
var DATAGRID_GROUP_OPENED_CLASS = "dx-datagrid-group-opened";
var DATAGRID_GROUP_CLOSED_CLASS = "dx-datagrid-group-closed";
var DATAGRID_EXPAND_CLASS = "dx-datagrid-expand";
var NO_DATA_CLASS = "nodata";
var SCROLLING_MODE_INFINITE = "infinite";
var SCROLLING_MODE_VIRTUAL = "virtual";
var LEGACY_SCROLLING_MODE = "scrolling.legacyMode";
var SCROLLING_MODE_OPTION = "scrolling.mode";
var ROW_RENDERING_MODE_OPTION = "scrolling.rowRenderingMode";
var DATE_INTERVAL_SELECTORS = {
    year: function(value) {
        return value && value.getFullYear()
    },
    month: function(value) {
        return value && value.getMonth() + 1
    },
    day: function(value) {
        return value && value.getDate()
    },
    quarter: function(value) {
        return value && Math.floor(value.getMonth() / 3) + 1
    },
    hour: function(value) {
        return value && value.getHours()
    },
    minute: function(value) {
        return value && value.getMinutes()
    },
    second: function(value) {
        return value && value.getSeconds()
    }
};
var getIntervalSelector = function() {
    var data = arguments[1];
    var value = this.calculateCellValue(data);
    if (!(0, _type.isDefined)(value)) {
        return null
    } else if (isDateType(this.dataType)) {
        var nameIntervalSelector = arguments[0];
        return DATE_INTERVAL_SELECTORS[nameIntervalSelector](value)
    } else if ("number" === this.dataType) {
        var groupInterval = arguments[0];
        return Math.floor(Number(value) / groupInterval) * groupInterval
    }
};
var equalSelectors = function(selector1, selector2) {
    if ((0, _type.isFunction)(selector1) && (0, _type.isFunction)(selector2)) {
        if (selector1.originalCallback && selector2.originalCallback) {
            return selector1.originalCallback === selector2.originalCallback && selector1.columnIndex === selector2.columnIndex
        }
    }
    return selector1 === selector2
};

function isDateType(dataType) {
    return "date" === dataType || "datetime" === dataType
}
var setEmptyText = function($container) {
    $container.get(0).textContent = "\xa0"
};
var normalizeSortingInfo = function(sort) {
    sort = sort || [];
    var result = (0, _utils.normalizeSortingInfo)(sort);
    for (var i = 0; i < sort.length; i++) {
        if (sort && sort[i] && void 0 !== sort[i].isExpanded) {
            result[i].isExpanded = sort[i].isExpanded
        }
        if (sort && sort[i] && void 0 !== sort[i].groupInterval) {
            result[i].groupInterval = sort[i].groupInterval
        }
    }
    return result
};
var formatValue = function(value, options) {
    var valueText = _format_helper.default.format(value, options.format) || value && value.toString() || "";
    var formatObject = {
        value: value,
        valueText: options.getDisplayFormat ? options.getDisplayFormat(valueText) : valueText,
        target: options.target || "row",
        groupInterval: options.groupInterval
    };
    return options.customizeText ? options.customizeText.call(options, formatObject) : formatObject.valueText
};
var getSummaryText = function(summaryItem, summaryTexts) {
    var displayFormat = summaryItem.displayFormat || summaryItem.columnCaption && summaryTexts[summaryItem.summaryType + "OtherColumn"] || summaryTexts[summaryItem.summaryType];
    return formatValue(summaryItem.value, {
        format: summaryItem.valueFormat,
        getDisplayFormat: function(valueText) {
            return displayFormat ? (0, _string.format)(displayFormat, valueText, summaryItem.columnCaption) : valueText
        },
        customizeText: summaryItem.customizeText
    })
};
var getWidgetInstance = function($element) {
    var editorData = $element.data && $element.data();
    var dxComponents = editorData && editorData.dxComponents;
    var widgetName = dxComponents && dxComponents[0];
    return widgetName && editorData[widgetName]
};
var equalFilterParameters = function equalFilterParameters(filter1, filter2) {
    if (Array.isArray(filter1) && Array.isArray(filter2)) {
        if (filter1.length !== filter2.length) {
            return false
        } else {
            for (var i = 0; i < filter1.length; i++) {
                if (!equalFilterParameters(filter1[i], filter2[i])) {
                    return false
                }
            }
        }
        return true
    } else if ((0, _type.isFunction)(filter1) && filter1.columnIndex >= 0 && (0, _type.isFunction)(filter2) && filter2.columnIndex >= 0) {
        return filter1.columnIndex === filter2.columnIndex && (0, _data.toComparable)(filter1.filterValue) === (0, _data.toComparable)(filter2.filterValue)
    } else {
        return (0, _data.toComparable)(filter1) == (0, _data.toComparable)(filter2)
    }
};
var _default = {
    renderNoDataText: function($element) {
        $element = $element || this.element();
        if (!$element) {
            return
        }
        var noDataClass = this.addWidgetPrefix(NO_DATA_CLASS);
        var noDataElement = $element.find("." + noDataClass).last();
        var isVisible = this._dataController.isEmpty();
        var isLoading = this._dataController.isLoading();
        if (!noDataElement.length) {
            noDataElement = (0, _renderer.default)("<span>").addClass(noDataClass).appendTo($element)
        }
        if (isVisible && !isLoading) {
            noDataElement.removeClass("dx-hidden").text(this._getNoDataText())
        } else {
            noDataElement.addClass("dx-hidden")
        }
    },
    renderLoadPanel: function($element, $container, isLocalStore) {
        var loadPanelOptions;
        this._loadPanel && this._loadPanel.$element().remove();
        loadPanelOptions = this.option("loadPanel");
        if (loadPanelOptions && ("auto" === loadPanelOptions.enabled ? !isLocalStore : loadPanelOptions.enabled)) {
            loadPanelOptions = (0, _extend.extend)({
                shading: false,
                message: loadPanelOptions.text,
                container: $container
            }, loadPanelOptions);
            this._loadPanel = this._createComponent((0, _renderer.default)("<div>").appendTo($container), _load_panel.default, loadPanelOptions)
        } else {
            this._loadPanel = null
        }
    },
    calculateLoadPanelPosition: function($element) {
        var $window = (0, _renderer.default)((0, _window.getWindow)());
        if ((0, _size.getHeight)($element) > (0, _size.getHeight)($window)) {
            return {
                of: $window,
                boundary: $element,
                collision: "fit"
            }
        }
        return {
            of: $element
        }
    },
    getIndexByKey: function(key, items, keyName) {
        var index = -1;
        if (void 0 !== key && Array.isArray(items)) {
            keyName = arguments.length <= 2 ? "key" : keyName;
            for (var i = 0; i < items.length; i++) {
                var item = (0, _type.isDefined)(keyName) ? items[i][keyName] : items[i];
                if ((0, _common.equalByValue)(key, item)) {
                    index = i;
                    break
                }
            }
        }
        return index
    },
    combineFilters: function(filters, operation) {
        var resultFilter = [];
        operation = operation || "and";
        for (var i = 0; i < filters.length; i++) {
            var _filters$i;
            if (!filters[i]) {
                continue
            }
            if (1 === (null === (_filters$i = filters[i]) || void 0 === _filters$i ? void 0 : _filters$i.length) && "!" === filters[i][0]) {
                if ("and" === operation) {
                    return ["!"]
                } else if ("or" === operation) {
                    continue
                }
            }
            if (resultFilter.length) {
                resultFilter.push(operation)
            }
            resultFilter.push(filters[i])
        }
        if (1 === resultFilter.length) {
            resultFilter = resultFilter[0]
        }
        if (resultFilter.length) {
            return resultFilter
        }
    },
    checkChanges: function(changes, changeNames) {
        var changesWithChangeNamesCount = 0;
        for (var i = 0; i < changeNames.length; i++) {
            if (changes[changeNames[i]]) {
                changesWithChangeNamesCount++
            }
        }
        return changes.length && changes.length === changesWithChangeNamesCount
    },
    equalFilterParameters: equalFilterParameters,
    proxyMethod: function(instance, methodName, defaultResult) {
        if (!instance[methodName]) {
            instance[methodName] = function() {
                var dataSource = this._dataSource;
                return dataSource ? dataSource[methodName].apply(dataSource, arguments) : defaultResult
            }
        }
    },
    formatValue: formatValue,
    getFormatOptionsByColumn: function(column, target) {
        return {
            format: column.format,
            getDisplayFormat: column.getDisplayFormat,
            customizeText: column.customizeText,
            target: target,
            trueText: column.trueText,
            falseText: column.falseText
        }
    },
    getDisplayValue: function(column, value, data, rowType) {
        if (column.displayValueMap && void 0 !== column.displayValueMap[value]) {
            return column.displayValueMap[value]
        } else if (column.calculateDisplayValue && data && "group" !== rowType) {
            return column.calculateDisplayValue(data)
        } else if (column.lookup && !("group" === rowType && (column.calculateGroupValue || column.calculateDisplayValue))) {
            return column.lookup.calculateCellValue(value)
        }
        return value
    },
    getGroupRowSummaryText: function(summaryItems, summaryTexts) {
        var result = "(";
        for (var i = 0; i < summaryItems.length; i++) {
            var summaryItem = summaryItems[i];
            result += (i > 0 ? ", " : "") + getSummaryText(summaryItem, summaryTexts)
        }
        return result + ")"
    },
    getSummaryText: getSummaryText,
    normalizeSortingInfo: normalizeSortingInfo,
    getFormatByDataType: function(dataType) {
        switch (dataType) {
            case "date":
                return "shortDate";
            case "datetime":
                return "shortDateShortTime"
        }
    },
    getHeaderFilterGroupParameters: function(column, remoteGrouping) {
        var result = [];
        var dataField = column.dataField || column.name;
        var groupInterval = _filtering.default.getGroupInterval(column);
        if (groupInterval) {
            (0, _iterator.each)(groupInterval, (function(index, interval) {
                result.push(remoteGrouping ? {
                    selector: dataField,
                    groupInterval: interval,
                    isExpanded: index < groupInterval.length - 1
                } : getIntervalSelector.bind(column, interval))
            }));
            return result
        }
        if (remoteGrouping) {
            result = [{
                selector: dataField,
                isExpanded: false
            }]
        } else {
            result = function(data) {
                var result = column.calculateCellValue(data);
                if (void 0 === result || "" === result) {
                    result = null
                }
                return result
            };
            if (column.sortingMethod) {
                result = [{
                    selector: result,
                    compare: column.sortingMethod.bind(column)
                }]
            }
        }
        return result
    },
    equalSortParameters: function(sortParameters1, sortParameters2, ignoreIsExpanded) {
        sortParameters1 = normalizeSortingInfo(sortParameters1);
        sortParameters2 = normalizeSortingInfo(sortParameters2);
        if (Array.isArray(sortParameters1) && Array.isArray(sortParameters2)) {
            if (sortParameters1.length !== sortParameters2.length) {
                return false
            } else {
                for (var i = 0; i < sortParameters1.length; i++) {
                    if (!equalSelectors(sortParameters1[i].selector, sortParameters2[i].selector) || sortParameters1[i].desc !== sortParameters2[i].desc || sortParameters1[i].groupInterval !== sortParameters2[i].groupInterval || !ignoreIsExpanded && Boolean(sortParameters1[i].isExpanded) !== Boolean(sortParameters2[i].isExpanded)) {
                        return false
                    }
                }
            }
            return true
        } else {
            return (!sortParameters1 || !sortParameters1.length) === (!sortParameters2 || !sortParameters2.length)
        }
    },
    getPointsByColumns: function(items, pointCreated, isVertical, startColumnIndex) {
        var cellsLength = items.length;
        var notCreatePoint = false;
        var item;
        var offset;
        var columnIndex = startColumnIndex || 0;
        var result = [];
        var rtlEnabled;
        for (var i = 0; i <= cellsLength; i++) {
            if (i < cellsLength) {
                item = items.eq(i);
                offset = item.offset();
                rtlEnabled = "rtl" === item.css("direction")
            }
            var point = {
                index: columnIndex,
                x: offset ? offset.left + (!isVertical && rtlEnabled ^ i === cellsLength ? (0, _position.getBoundingRect)(item[0]).width : 0) : 0,
                y: offset ? offset.top + (isVertical && i === cellsLength ? (0, _position.getBoundingRect)(item[0]).height : 0) : 0,
                columnIndex: columnIndex
            };
            if (!isVertical && i > 0) {
                var prevItemOffset = items.eq(i - 1).offset();
                if (prevItemOffset.top < point.y) {
                    point.y = prevItemOffset.top
                }
            }
            if (pointCreated) {
                notCreatePoint = pointCreated(point)
            }
            if (!notCreatePoint) {
                result.push(point)
            }
            columnIndex++
        }
        return result
    },
    getExpandCellTemplate: function() {
        return {
            allowRenderToDetachedContainer: true,
            render: function(container, options) {
                var $container = (0, _renderer.default)(container);
                if ((0, _type.isDefined)(options.value) && !(options.data && options.data.isContinuation) && !options.row.isNewRow) {
                    var rowsView = options.component.getView("rowsView");
                    $container.addClass(DATAGRID_EXPAND_CLASS).addClass(DATAGRID_SELECTION_DISABLED_CLASS);
                    (0, _renderer.default)("<div>").addClass(options.value ? DATAGRID_GROUP_OPENED_CLASS : DATAGRID_GROUP_CLOSED_CLASS).appendTo($container);
                    rowsView.setAria("label", options.value ? rowsView.localize("dxDataGrid-ariaCollapse") : rowsView.localize("dxDataGrid-ariaExpand"), $container)
                } else {
                    setEmptyText($container)
                }
            }
        }
    },
    setEmptyText: setEmptyText,
    isDateType: isDateType,
    getSelectionRange: function(focusedElement) {
        try {
            if (focusedElement) {
                return {
                    selectionStart: focusedElement.selectionStart,
                    selectionEnd: focusedElement.selectionEnd
                }
            }
        } catch (e) {}
        return {}
    },
    setSelectionRange: function(focusedElement, selectionRange) {
        try {
            if (focusedElement && focusedElement.setSelectionRange) {
                focusedElement.setSelectionRange(selectionRange.selectionStart, selectionRange.selectionEnd)
            }
        } catch (e) {}
    },
    focusAndSelectElement: function(component, $element) {
        var isFocused = $element.is(":focus");
        _events_engine.default.trigger($element, "focus");
        var isSelectTextOnEditingStart = component.option("editing.selectTextOnEditStart");
        var element = $element.get(0);
        if (!isFocused && isSelectTextOnEditingStart && $element.is(".dx-texteditor-input") && !$element.is("[readonly]")) {
            var editor = getWidgetInstance($element.closest(".dx-texteditor"));
            (0, _deferred.when)(editor && editor._loadItemDeferred).done((function() {
                element.select()
            }))
        }
    },
    getWidgetInstance: getWidgetInstance,
    getLastResizableColumnIndex: function(columns, resultWidths) {
        var hasResizableColumns = columns.some((function(column) {
            return column && !column.command && !column.fixed && false !== column.allowResizing
        }));
        var lastColumnIndex;
        for (lastColumnIndex = columns.length - 1; columns[lastColumnIndex]; lastColumnIndex--) {
            var column = columns[lastColumnIndex];
            var width = resultWidths && resultWidths[lastColumnIndex];
            var allowResizing = !hasResizableColumns || false !== column.allowResizing;
            if (!column.command && !column.fixed && "adaptiveHidden" !== width && allowResizing) {
                break
            }
        }
        return lastColumnIndex
    },
    isElementInCurrentGrid: function(controller, $element) {
        if ($element && $element.length) {
            var $grid = $element.closest("." + controller.getWidgetContainerClass()).parent();
            return $grid.is(controller.component.$element())
        }
        return false
    },
    isVirtualRowRendering: function(that) {
        var rowRenderingMode = that.option(ROW_RENDERING_MODE_OPTION);
        var isVirtualMode = that.option(SCROLLING_MODE_OPTION) === SCROLLING_MODE_VIRTUAL;
        var isAppendMode = that.option(SCROLLING_MODE_OPTION) === SCROLLING_MODE_INFINITE;
        if (false === that.option(LEGACY_SCROLLING_MODE) && (isVirtualMode || isAppendMode)) {
            return true
        }
        return rowRenderingMode === SCROLLING_MODE_VIRTUAL
    },
    getPixelRatio: function(window) {
        return window.devicePixelRatio || 1
    },
    getContentHeightLimit: function(browser) {
        if (browser.mozilla) {
            return 8e6
        }
        return 15e6 / this.getPixelRatio((0, _window.getWindow)())
    }
};
exports.default = _default;
module.exports = exports.default;
module.exports.default = exports.default;
