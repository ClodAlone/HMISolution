/**
 * DevExtreme (esm/ui/scheduler/workspaces/ui.scheduler.work_space.grouped.strategy.vertical.js)
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
    Cache
} from "./cache";
import {
    FIRST_GROUP_CELL_CLASS,
    LAST_GROUP_CELL_CLASS
} from "../classes";
import {
    calculateDayDuration,
    getVerticalGroupCountClass
} from "../../../renovation/ui/scheduler/view_model/to_test/views/utils/base";
var DATE_HEADER_OFFSET = 10;
var WORK_SPACE_BORDER = 1;
class VerticalGroupedStrategy {
    constructor(workSpace) {
        this._workSpace = workSpace;
        this.cache = new Cache
    }
    prepareCellIndexes(cellCoordinates, groupIndex, inAllDayRow) {
        var rowIndex = cellCoordinates.rowIndex + groupIndex * this._workSpace._getRowCount();
        if (this._workSpace.supportAllDayRow() && this._workSpace.option("showAllDayPanel")) {
            rowIndex += groupIndex;
            if (!inAllDayRow) {
                rowIndex += 1
            }
        }
        return {
            rowIndex: rowIndex,
            columnIndex: cellCoordinates.columnIndex
        }
    }
    getGroupIndex(rowIndex) {
        return Math.floor(rowIndex / this._workSpace._getRowCount())
    }
    calculateHeaderCellRepeatCount() {
        return 1
    }
    insertAllDayRowsIntoDateTable() {
        return this._workSpace.option("showAllDayPanel")
    }
    getTotalCellCount() {
        return this._workSpace._getCellCount()
    }
    getTotalRowCount() {
        return this._workSpace._getRowCount() * this._workSpace._getGroupCount()
    }
    calculateTimeCellRepeatCount() {
        return this._workSpace._getGroupCount() || 1
    }
    getWorkSpaceMinWidth() {
        var minWidth = this._workSpace._getWorkSpaceWidth();
        var workspaceContainerWidth = getBoundingRect(this._workSpace.$element().get(0)).width - this._workSpace.getTimePanelWidth() - this._workSpace.getGroupTableWidth() - 2 * WORK_SPACE_BORDER;
        if (minWidth < workspaceContainerWidth) {
            minWidth = workspaceContainerWidth
        }
        return minWidth
    }
    getAllDayOffset() {
        return 0
    }
    getGroupCountClass(groups) {
        return getVerticalGroupCountClass(groups)
    }
    getLeftOffset() {
        return this._workSpace.getTimePanelWidth() + this._workSpace.getGroupTableWidth()
    }
    getGroupBoundsOffset(cellCount, $cells, cellWidth, coordinates) {
        var groupIndex = coordinates.groupIndex;
        return this.cache.get("groupBoundsOffset".concat(groupIndex), () => {
            var startOffset = $cells.eq(0).offset().left;
            var endOffset = $cells.eq(cellCount - 1).offset().left + cellWidth;
            var startDayHour = this._workSpace.option("startDayHour");
            var endDayHour = this._workSpace.option("endDayHour");
            var hoursInterval = this._workSpace.option("hoursInterval");
            var dayHeight = calculateDayDuration(startDayHour, endDayHour) / hoursInterval * this._workSpace.getCellHeight();
            var scrollTop = this.getScrollableScrollTop();
            var topOffset = groupIndex * dayHeight + getBoundingRect(this._workSpace._$thead.get(0)).height + this._workSpace.option("getHeaderHeight")() + DATE_HEADER_OFFSET - scrollTop;
            if (this._workSpace.option("showAllDayPanel") && this._workSpace.supportAllDayRow()) {
                topOffset += this._workSpace.getCellHeight() * (groupIndex + 1)
            }
            var bottomOffset = topOffset + dayHeight;
            return this._groupBoundsOffset = {
                left: startOffset,
                right: endOffset,
                top: topOffset,
                bottom: bottomOffset
            }
        })
    }
    shiftIndicator($indicator, height, rtlOffset, i) {
        var offset = this._workSpace.getIndicatorOffset(0);
        var tableOffset = this._workSpace.option("crossScrollingEnabled") ? 0 : this._workSpace.getGroupTableWidth();
        var horizontalOffset = rtlOffset ? rtlOffset - offset : offset;
        var verticalOffset = this._workSpace._getRowCount() * this._workSpace.getCellHeight() * i;
        if (this._workSpace.supportAllDayRow() && this._workSpace.option("showAllDayPanel")) {
            verticalOffset += this._workSpace.getAllDayHeight() * (i + 1)
        }
        $indicator.css("left", horizontalOffset + tableOffset);
        $indicator.css("top", height + verticalOffset)
    }
    getShaderOffset(i, width) {
        var offset = this._workSpace.option("crossScrollingEnabled") ? 0 : this._workSpace.getGroupTableWidth();
        return this._workSpace.option("rtlEnabled") ? getBoundingRect(this._$container.get(0)).width - offset - this._workSpace.getWorkSpaceLeftOffset() - width : offset
    }
    getShaderTopOffset(i) {
        return 0
    }
    getShaderHeight() {
        var height = this._workSpace.getIndicationHeight();
        if (this._workSpace.supportAllDayRow() && this._workSpace.option("showAllDayPanel")) {
            height += this._workSpace.getCellHeight()
        }
        return height
    }
    getShaderMaxHeight() {
        var height = this._workSpace._getRowCount() * this._workSpace.getCellHeight();
        if (this._workSpace.supportAllDayRow() && this._workSpace.option("showAllDayPanel")) {
            height += this._workSpace.getCellHeight()
        }
        return height
    }
    getShaderWidth() {
        return this._workSpace.getIndicationWidth(0)
    }
    getScrollableScrollTop() {
        return this._workSpace.getScrollable().scrollTop()
    }
    addAdditionalGroupCellClasses(cellClass, index, i, j) {
        cellClass = this._addLastGroupCellClass(cellClass, i + 1);
        return this._addFirstGroupCellClass(cellClass, i + 1)
    }
    _addLastGroupCellClass(cellClass, index) {
        if (index % this._workSpace._getRowCount() === 0) {
            return "".concat(cellClass, " ").concat(LAST_GROUP_CELL_CLASS)
        }
        return cellClass
    }
    _addFirstGroupCellClass(cellClass, index) {
        if ((index - 1) % this._workSpace._getRowCount() === 0) {
            return "".concat(cellClass, " ").concat(FIRST_GROUP_CELL_CLASS)
        }
        return cellClass
    }
}
export default VerticalGroupedStrategy;
