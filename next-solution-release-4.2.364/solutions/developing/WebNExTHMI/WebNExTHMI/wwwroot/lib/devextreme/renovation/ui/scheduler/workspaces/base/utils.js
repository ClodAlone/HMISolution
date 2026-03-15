/**
 * DevExtreme (renovation/ui/scheduler/workspaces/base/utils.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.getTotalRowCount = exports.getTotalCellCount = exports.getRowCountWithAllDayRow = exports.getHiddenInterval = exports.getDateTableWidth = exports.getDateForHeaderText = exports.createVirtualScrollingOptions = exports.createCellElementMetaData = exports.DATE_TABLE_MIN_CELL_WIDTH = void 0;
var _date = _interopRequireDefault(require("../../../../../core/utils/date"));
var _utils = require("../../../../../ui/scheduler/resources/utils");
var _utils2 = require("../utils");

function _interopRequireDefault(obj) {
    return obj && obj.__esModule ? obj : {
        default: obj
    }
}
var DAY_MS = _date.default.dateToMilliseconds("day");
var HOUR_MS = _date.default.dateToMilliseconds("hour");
var DATE_TABLE_MIN_CELL_WIDTH = 75;
exports.DATE_TABLE_MIN_CELL_WIDTH = DATE_TABLE_MIN_CELL_WIDTH;
var getTotalRowCount = function(rowCount, groupOrientation, groups, isAllDayPanelVisible) {
    var isVerticalGrouping = (0, _utils2.isVerticalGroupingApplied)(groups, groupOrientation);
    var groupCount = (0, _utils.getGroupCount)(groups);
    var totalRowCount = isVerticalGrouping ? rowCount * groupCount : rowCount;
    return isAllDayPanelVisible ? totalRowCount + groupCount : totalRowCount
};
exports.getTotalRowCount = getTotalRowCount;
var getTotalCellCount = function(cellCount, groupOrientation, groups) {
    var isHorizontalGrouping = (0, _utils2.isHorizontalGroupingApplied)(groups, groupOrientation);
    var groupCount = (0, _utils.getGroupCount)(groups);
    return isHorizontalGrouping ? cellCount * groupCount : cellCount
};
exports.getTotalCellCount = getTotalCellCount;
var getRowCountWithAllDayRow = function(rowCount, isAllDayPanelVisible) {
    return isAllDayPanelVisible ? rowCount + 1 : rowCount
};
exports.getRowCountWithAllDayRow = getRowCountWithAllDayRow;
var getHiddenInterval = function(hoursInterval, cellCountInDay) {
    var visibleInterval = hoursInterval * cellCountInDay * HOUR_MS;
    return DAY_MS - visibleInterval
};
exports.getHiddenInterval = getHiddenInterval;
var createCellElementMetaData = function(tableRect, cellRect) {
    var bottom = cellRect.bottom,
        height = cellRect.height,
        left = cellRect.left,
        right = cellRect.right,
        top = cellRect.top,
        width = cellRect.width,
        x = cellRect.x,
        y = cellRect.y;
    return {
        right: right,
        bottom: bottom,
        left: left - tableRect.left,
        top: top - tableRect.top,
        width: width,
        height: height,
        x: x,
        y: y
    }
};
exports.createCellElementMetaData = createCellElementMetaData;
var getDateForHeaderText = function(_, date) {
    return date
};
exports.getDateForHeaderText = getDateForHeaderText;
var getDateTableWidth = function(scrollableWidth, dateTable, viewDataProvider, workSpaceConfig) {
    var dateTableCell = dateTable.querySelector("td:not(.dx-scheduler-virtual-cell)");
    var cellWidth = dateTableCell.getBoundingClientRect().width;
    if (cellWidth < DATE_TABLE_MIN_CELL_WIDTH) {
        cellWidth = DATE_TABLE_MIN_CELL_WIDTH
    }
    var cellCount = viewDataProvider.getCellCount(workSpaceConfig);
    var totalCellCount = getTotalCellCount(cellCount, workSpaceConfig.groupOrientation, workSpaceConfig.groups);
    var minTablesWidth = totalCellCount * cellWidth;
    return scrollableWidth < minTablesWidth ? minTablesWidth : scrollableWidth
};
exports.getDateTableWidth = getDateTableWidth;
var createVirtualScrollingOptions = function(options) {
    return {
        getCellHeight: function() {
            return options.cellHeight
        },
        getCellWidth: function() {
            return options.cellWidth
        },
        getCellMinWidth: function() {
            return DATE_TABLE_MIN_CELL_WIDTH
        },
        isRTL: function() {
            return false
        },
        getSchedulerHeight: function() {
            return options.schedulerHeight
        },
        getSchedulerWidth: function() {
            return options.schedulerWidth
        },
        getViewHeight: function() {
            return options.viewHeight
        },
        getViewWidth: function() {
            return options.viewWidth
        },
        getScrolling: function() {
            return options.scrolling
        },
        getScrollableOuterWidth: function() {
            return options.scrollableWidth
        },
        getGroupCount: function() {
            return (0, _utils.getGroupCount)(options.groups)
        },
        isVerticalGrouping: function() {
            return options.isVerticalGrouping
        },
        getTotalRowCount: function() {
            return options.completeRowCount
        },
        getTotalCellCount: function() {
            return options.completeColumnCount
        }
    }
};
exports.createVirtualScrollingOptions = createVirtualScrollingOptions;
