/**
 * DevExtreme (cjs/ui/scheduler/workspaces/ui.scheduler.work_space.grouped.strategy.horizontal.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.default = void 0;
var _position = require("../../../core/utils/position");
var _classes = require("../classes");
var HorizontalGroupedStrategy = function() {
    function HorizontalGroupedStrategy(workSpace) {
        this._workSpace = workSpace
    }
    var _proto = HorizontalGroupedStrategy.prototype;
    _proto.prepareCellIndexes = function(cellCoordinates, groupIndex, inAllDay) {
        var groupByDay = this._workSpace.isGroupedByDate();
        if (!groupByDay) {
            return {
                rowIndex: cellCoordinates.rowIndex,
                columnIndex: cellCoordinates.columnIndex + groupIndex * this._workSpace._getCellCount()
            }
        } else {
            return {
                rowIndex: cellCoordinates.rowIndex,
                columnIndex: cellCoordinates.columnIndex * this._workSpace._getGroupCount() + groupIndex
            }
        }
    };
    _proto.getGroupIndex = function(rowIndex, columnIndex) {
        var groupByDay = this._workSpace.isGroupedByDate();
        var groupCount = this._workSpace._getGroupCount();
        if (groupByDay) {
            return columnIndex % groupCount
        } else {
            return Math.floor(columnIndex / this._workSpace._getCellCount())
        }
    };
    _proto.calculateHeaderCellRepeatCount = function() {
        return this._workSpace._getGroupCount() || 1
    };
    _proto.insertAllDayRowsIntoDateTable = function() {
        return false
    };
    _proto.getTotalCellCount = function(groupCount) {
        groupCount = groupCount || 1;
        return this._workSpace._getCellCount() * groupCount
    };
    _proto.getTotalRowCount = function() {
        return this._workSpace._getRowCount()
    };
    _proto.calculateTimeCellRepeatCount = function() {
        return 1
    };
    _proto.getWorkSpaceMinWidth = function() {
        return (0, _position.getBoundingRect)(this._workSpace.$element().get(0)).width - this._workSpace.getTimePanelWidth()
    };
    _proto.getAllDayOffset = function() {
        return this._workSpace.getAllDayHeight()
    };
    _proto.getGroupCountClass = function(groups) {
        return
    };
    _proto.getLeftOffset = function() {
        return this._workSpace.getTimePanelWidth()
    };
    _proto._createGroupBoundOffset = function(startCell, endCell, cellWidth) {
        var extraOffset = cellWidth / 2;
        var startOffset = startCell ? startCell.offset().left - extraOffset : 0;
        var endOffset = endCell ? endCell.offset().left + cellWidth + extraOffset : 0;
        return {
            left: startOffset,
            right: endOffset,
            top: 0,
            bottom: 0
        }
    };
    _proto._getGroupedByDateBoundOffset = function($cells, cellWidth) {
        var lastCellIndex = $cells.length - 1;
        var startCell = $cells.eq(0);
        var endCell = $cells.eq(lastCellIndex);
        return this._createGroupBoundOffset(startCell, endCell, cellWidth)
    };
    _proto.getGroupBoundsOffset = function(cellCount, $cells, cellWidth, coordinates, groupedDataMap) {
        if (this._workSpace.isGroupedByDate()) {
            return this._getGroupedByDateBoundOffset($cells, cellWidth)
        }
        var startCell;
        var endCell;
        var cellIndex = this._workSpace.getCellIndexByCoordinates(coordinates);
        var groupIndex = coordinates.groupIndex || Math.floor(cellIndex / cellCount);
        var currentCellGroup = groupedDataMap.dateTableGroupedMap[groupIndex];
        if (currentCellGroup) {
            var groupRowLength = currentCellGroup[0].length;
            var groupStartPosition = currentCellGroup[0][0].position;
            var groupEndPosition = currentCellGroup[0][groupRowLength - 1].position;
            startCell = $cells.eq(groupStartPosition.columnIndex);
            endCell = $cells.eq(groupEndPosition.columnIndex)
        }
        return this._createGroupBoundOffset(startCell, endCell, cellWidth)
    };
    _proto.shiftIndicator = function($indicator, height, rtlOffset, groupIndex) {
        var offset = this._getIndicatorOffset(groupIndex);
        var horizontalOffset = rtlOffset ? rtlOffset - offset : offset;
        $indicator.css("left", horizontalOffset);
        $indicator.css("top", height)
    };
    _proto._getIndicatorOffset = function(groupIndex) {
        var groupByDay = this._workSpace.isGroupedByDate();
        return groupByDay ? this._calculateGroupByDateOffset(groupIndex) : this._calculateOffset(groupIndex)
    };
    _proto._calculateOffset = function(groupIndex) {
        return this._workSpace._getCellCount() * this._workSpace.getRoundedCellWidth(groupIndex - 1, 0) * groupIndex + this._workSpace.getIndicatorOffset(groupIndex) + groupIndex
    };
    _proto._calculateGroupByDateOffset = function(groupIndex) {
        return this._workSpace.getIndicatorOffset(0) * this._workSpace._getGroupCount() + this._workSpace.getRoundedCellWidth(groupIndex - 1, 0) * groupIndex
    };
    _proto.getShaderOffset = function(i, width) {
        var offset = this._workSpace._getCellCount() * this._workSpace.getRoundedCellWidth(i - 1) * i;
        return this._workSpace.option("rtlEnabled") ? (0, _position.getBoundingRect)(this._workSpace._dateTableScrollable.$content().get(0)).width - offset - this._workSpace.getTimePanelWidth() - width : offset
    };
    _proto.getShaderTopOffset = function(i) {
        return -this.getShaderMaxHeight() * (i > 0 ? 1 : 0)
    };
    _proto.getShaderHeight = function() {
        var height = this._workSpace.getIndicationHeight();
        return height
    };
    _proto.getShaderMaxHeight = function() {
        return (0, _position.getBoundingRect)(this._workSpace._dateTableScrollable.$content().get(0)).height
    };
    _proto.getShaderWidth = function(i) {
        return this._workSpace.getIndicationWidth(i)
    };
    _proto.getScrollableScrollTop = function(allDay) {
        return !allDay ? this._workSpace.getScrollable().scrollTop() : 0
    };
    _proto.addAdditionalGroupCellClasses = function(cellClass, index, i, j) {
        var applyUnconditionally = arguments.length > 4 && void 0 !== arguments[4] ? arguments[4] : false;
        cellClass = this._addLastGroupCellClass(cellClass, index, applyUnconditionally);
        return this._addFirstGroupCellClass(cellClass, index, applyUnconditionally)
    };
    _proto._addLastGroupCellClass = function(cellClass, index, applyUnconditionally) {
        if (applyUnconditionally) {
            return "".concat(cellClass, " ").concat(_classes.LAST_GROUP_CELL_CLASS)
        }
        var groupByDate = this._workSpace.isGroupedByDate();
        if (groupByDate) {
            if (index % this._workSpace._getGroupCount() === 0) {
                return "".concat(cellClass, " ").concat(_classes.LAST_GROUP_CELL_CLASS)
            }
        } else if (index % this._workSpace._getCellCount() === 0) {
            return "".concat(cellClass, " ").concat(_classes.LAST_GROUP_CELL_CLASS)
        }
        return cellClass
    };
    _proto._addFirstGroupCellClass = function(cellClass, index, applyUnconditionally) {
        if (applyUnconditionally) {
            return "".concat(cellClass, " ").concat(_classes.FIRST_GROUP_CELL_CLASS)
        }
        var groupByDate = this._workSpace.isGroupedByDate();
        if (groupByDate) {
            if ((index - 1) % this._workSpace._getGroupCount() === 0) {
                return "".concat(cellClass, " ").concat(_classes.FIRST_GROUP_CELL_CLASS)
            }
        } else if ((index - 1) % this._workSpace._getCellCount() === 0) {
            return "".concat(cellClass, " ").concat(_classes.FIRST_GROUP_CELL_CLASS)
        }
        return cellClass
    };
    return HorizontalGroupedStrategy
}();
var _default = HorizontalGroupedStrategy;
exports.default = _default;
module.exports = exports.default;
module.exports.default = exports.default;
