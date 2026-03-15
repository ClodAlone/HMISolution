/**
 * DevExtreme (esm/ui/scheduler/workspaces/ui.scheduler.work_space.grouped.strategy.horizontal.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    getBoundingRect
} from "../../../core/utils/position";
import {
    FIRST_GROUP_CELL_CLASS,
    LAST_GROUP_CELL_CLASS
} from "../classes";
class HorizontalGroupedStrategy {
    constructor(workSpace) {
        this._workSpace = workSpace
    }
    prepareCellIndexes(cellCoordinates, groupIndex, inAllDay) {
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
    }
    getGroupIndex(rowIndex, columnIndex) {
        var groupByDay = this._workSpace.isGroupedByDate();
        var groupCount = this._workSpace._getGroupCount();
        if (groupByDay) {
            return columnIndex % groupCount
        } else {
            return Math.floor(columnIndex / this._workSpace._getCellCount())
        }
    }
    calculateHeaderCellRepeatCount() {
        return this._workSpace._getGroupCount() || 1
    }
    insertAllDayRowsIntoDateTable() {
        return false
    }
    getTotalCellCount(groupCount) {
        groupCount = groupCount || 1;
        return this._workSpace._getCellCount() * groupCount
    }
    getTotalRowCount() {
        return this._workSpace._getRowCount()
    }
    calculateTimeCellRepeatCount() {
        return 1
    }
    getWorkSpaceMinWidth() {
        return getBoundingRect(this._workSpace.$element().get(0)).width - this._workSpace.getTimePanelWidth()
    }
    getAllDayOffset() {
        return this._workSpace.getAllDayHeight()
    }
    getGroupCountClass(groups) {
        return
    }
    getLeftOffset() {
        return this._workSpace.getTimePanelWidth()
    }
    _createGroupBoundOffset(startCell, endCell, cellWidth) {
        var extraOffset = cellWidth / 2;
        var startOffset = startCell ? startCell.offset().left - extraOffset : 0;
        var endOffset = endCell ? endCell.offset().left + cellWidth + extraOffset : 0;
        return {
            left: startOffset,
            right: endOffset,
            top: 0,
            bottom: 0
        }
    }
    _getGroupedByDateBoundOffset($cells, cellWidth) {
        var lastCellIndex = $cells.length - 1;
        var startCell = $cells.eq(0);
        var endCell = $cells.eq(lastCellIndex);
        return this._createGroupBoundOffset(startCell, endCell, cellWidth)
    }
    getGroupBoundsOffset(cellCount, $cells, cellWidth, coordinates, groupedDataMap) {
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
    }
    shiftIndicator($indicator, height, rtlOffset, groupIndex) {
        var offset = this._getIndicatorOffset(groupIndex);
        var horizontalOffset = rtlOffset ? rtlOffset - offset : offset;
        $indicator.css("left", horizontalOffset);
        $indicator.css("top", height)
    }
    _getIndicatorOffset(groupIndex) {
        var groupByDay = this._workSpace.isGroupedByDate();
        return groupByDay ? this._calculateGroupByDateOffset(groupIndex) : this._calculateOffset(groupIndex)
    }
    _calculateOffset(groupIndex) {
        return this._workSpace._getCellCount() * this._workSpace.getRoundedCellWidth(groupIndex - 1, 0) * groupIndex + this._workSpace.getIndicatorOffset(groupIndex) + groupIndex
    }
    _calculateGroupByDateOffset(groupIndex) {
        return this._workSpace.getIndicatorOffset(0) * this._workSpace._getGroupCount() + this._workSpace.getRoundedCellWidth(groupIndex - 1, 0) * groupIndex
    }
    getShaderOffset(i, width) {
        var offset = this._workSpace._getCellCount() * this._workSpace.getRoundedCellWidth(i - 1) * i;
        return this._workSpace.option("rtlEnabled") ? getBoundingRect(this._workSpace._dateTableScrollable.$content().get(0)).width - offset - this._workSpace.getTimePanelWidth() - width : offset
    }
    getShaderTopOffset(i) {
        return -this.getShaderMaxHeight() * (i > 0 ? 1 : 0)
    }
    getShaderHeight() {
        var height = this._workSpace.getIndicationHeight();
        return height
    }
    getShaderMaxHeight() {
        return getBoundingRect(this._workSpace._dateTableScrollable.$content().get(0)).height
    }
    getShaderWidth(i) {
        return this._workSpace.getIndicationWidth(i)
    }
    getScrollableScrollTop(allDay) {
        return !allDay ? this._workSpace.getScrollable().scrollTop() : 0
    }
    addAdditionalGroupCellClasses(cellClass, index, i, j) {
        var applyUnconditionally = arguments.length > 4 && void 0 !== arguments[4] ? arguments[4] : false;
        cellClass = this._addLastGroupCellClass(cellClass, index, applyUnconditionally);
        return this._addFirstGroupCellClass(cellClass, index, applyUnconditionally)
    }
    _addLastGroupCellClass(cellClass, index, applyUnconditionally) {
        if (applyUnconditionally) {
            return "".concat(cellClass, " ").concat(LAST_GROUP_CELL_CLASS)
        }
        var groupByDate = this._workSpace.isGroupedByDate();
        if (groupByDate) {
            if (index % this._workSpace._getGroupCount() === 0) {
                return "".concat(cellClass, " ").concat(LAST_GROUP_CELL_CLASS)
            }
        } else if (index % this._workSpace._getCellCount() === 0) {
            return "".concat(cellClass, " ").concat(LAST_GROUP_CELL_CLASS)
        }
        return cellClass
    }
    _addFirstGroupCellClass(cellClass, index, applyUnconditionally) {
        if (applyUnconditionally) {
            return "".concat(cellClass, " ").concat(FIRST_GROUP_CELL_CLASS)
        }
        var groupByDate = this._workSpace.isGroupedByDate();
        if (groupByDate) {
            if ((index - 1) % this._workSpace._getGroupCount() === 0) {
                return "".concat(cellClass, " ").concat(FIRST_GROUP_CELL_CLASS)
            }
        } else if ((index - 1) % this._workSpace._getCellCount() === 0) {
            return "".concat(cellClass, " ").concat(FIRST_GROUP_CELL_CLASS)
        }
        return cellClass
    }
}
export default HorizontalGroupedStrategy;
