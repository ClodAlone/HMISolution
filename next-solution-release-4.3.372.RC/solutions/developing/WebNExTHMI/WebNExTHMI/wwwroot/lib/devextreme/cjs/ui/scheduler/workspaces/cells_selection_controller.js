/**
 * DevExtreme (cjs/ui/scheduler/workspaces/cells_selection_controller.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.CellsSelectionController = void 0;
var _base = require("../../../renovation/ui/scheduler/view_model/to_test/views/utils/base");

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
var CellsSelectionController = function() {
    function CellsSelectionController() {}
    var _proto = CellsSelectionController.prototype;
    _proto.handleArrowClick = function(options) {
        var key = options.key,
            focusedCellPosition = options.focusedCellPosition,
            edgeIndices = options.edgeIndices,
            getCellDataByPosition = options.getCellDataByPosition,
            isAllDayPanelCell = options.isAllDayPanelCell;
        var nextCellIndices;
        switch (key) {
            case "down":
                nextCellIndices = this.getCellFromNextRowPosition(focusedCellPosition, "next", edgeIndices);
                break;
            case "up":
                nextCellIndices = this.getCellFromNextRowPosition(focusedCellPosition, "prev", edgeIndices);
                break;
            case "left":
                nextCellIndices = this.getCellFromNextColumnPosition(_extends({}, options, {
                    direction: "prev"
                }));
                break;
            case "right":
                nextCellIndices = this.getCellFromNextColumnPosition(_extends({}, options, {
                    direction: "next"
                }))
        }
        var currentCellData = getCellDataByPosition(nextCellIndices.rowIndex, nextCellIndices.columnIndex, isAllDayPanelCell);
        return this.moveToCell(_extends({}, options, {
            currentCellData: currentCellData
        }))
    };
    _proto.getCellFromNextRowPosition = function(focusedCellPosition, direction, edgeIndices) {
        var columnIndex = focusedCellPosition.columnIndex,
            rowIndex = focusedCellPosition.rowIndex;
        var deltaPosition = "next" === direction ? 1 : -1;
        var nextRowIndex = rowIndex + deltaPosition;
        var validRowIndex = nextRowIndex >= 0 && nextRowIndex <= edgeIndices.lastRowIndex ? nextRowIndex : rowIndex;
        return {
            columnIndex: columnIndex,
            rowIndex: validRowIndex
        }
    };
    _proto.getCellFromNextColumnPosition = function(options) {
        var focusedCellPosition = options.focusedCellPosition,
            direction = options.direction,
            edgeIndices = options.edgeIndices,
            isRTL = options.isRTL,
            isGroupedByDate = options.isGroupedByDate,
            groupCount = options.groupCount,
            isMultiSelection = options.isMultiSelection,
            viewType = options.viewType;
        var columnIndex = focusedCellPosition.columnIndex,
            rowIndex = focusedCellPosition.rowIndex;
        var firstColumnIndex = edgeIndices.firstColumnIndex,
            lastColumnIndex = edgeIndices.lastColumnIndex,
            firstRowIndex = edgeIndices.firstRowIndex,
            lastRowIndex = edgeIndices.lastRowIndex;
        var step = isGroupedByDate && isMultiSelection ? groupCount : 1;
        var sign = isRTL ? -1 : 1;
        var deltaColumnIndex = "next" === direction ? sign * step : -1 * sign * step;
        var nextColumnIndex = columnIndex + deltaColumnIndex;
        var isValidColumnIndex = nextColumnIndex >= firstColumnIndex && nextColumnIndex <= lastColumnIndex;
        if (isValidColumnIndex) {
            return {
                columnIndex: nextColumnIndex,
                rowIndex: rowIndex
            }
        }
        return (0, _base.isDateAndTimeView)(viewType) ? focusedCellPosition : this._processEdgeCell({
            nextColumnIndex: nextColumnIndex,
            rowIndex: rowIndex,
            columnIndex: columnIndex,
            firstColumnIndex: firstColumnIndex,
            lastColumnIndex: lastColumnIndex,
            firstRowIndex: firstRowIndex,
            lastRowIndex: lastRowIndex,
            step: step
        })
    };
    _proto._processEdgeCell = function(options) {
        var nextColumnIndex = options.nextColumnIndex,
            rowIndex = options.rowIndex,
            columnIndex = options.columnIndex,
            firstColumnIndex = options.firstColumnIndex,
            lastColumnIndex = options.lastColumnIndex,
            firstRowIndex = options.firstRowIndex,
            lastRowIndex = options.lastRowIndex,
            step = options.step;
        var validColumnIndex = nextColumnIndex;
        var validRowIndex = rowIndex;
        var isLeftEdgeCell = nextColumnIndex < firstColumnIndex;
        var isRightEdgeCell = nextColumnIndex > lastColumnIndex;
        if (isLeftEdgeCell) {
            var columnIndexInNextRow = lastColumnIndex - (step - columnIndex % step - 1);
            var nextRowIndex = rowIndex - 1;
            var isValidRowIndex = nextRowIndex >= firstRowIndex;
            validRowIndex = isValidRowIndex ? nextRowIndex : rowIndex;
            validColumnIndex = isValidRowIndex ? columnIndexInNextRow : columnIndex
        }
        if (isRightEdgeCell) {
            var _columnIndexInNextRow = firstColumnIndex + columnIndex % step;
            var _nextRowIndex = rowIndex + 1;
            var _isValidRowIndex = _nextRowIndex <= lastRowIndex;
            validRowIndex = _isValidRowIndex ? _nextRowIndex : rowIndex;
            validColumnIndex = _isValidRowIndex ? _columnIndexInNextRow : columnIndex
        }
        return {
            columnIndex: validColumnIndex,
            rowIndex: validRowIndex
        }
    };
    _proto.moveToCell = function(options) {
        var isMultiSelection = options.isMultiSelection,
            isMultiSelectionAllowed = options.isMultiSelectionAllowed,
            focusedCellData = options.focusedCellData,
            currentCellData = options.currentCellData;
        var isValidMultiSelection = isMultiSelection && isMultiSelectionAllowed;
        var nextFocusedCellData = isValidMultiSelection ? this._getNextCellData(currentCellData, focusedCellData) : currentCellData;
        return nextFocusedCellData
    };
    _proto._getNextCellData = function(nextFocusedCellData, focusedCellData, isVirtualCell) {
        if (isVirtualCell) {
            return focusedCellData
        }
        var isValidNextFocusedCell = this._isValidNextFocusedCell(nextFocusedCellData, focusedCellData);
        return isValidNextFocusedCell ? nextFocusedCellData : focusedCellData
    };
    _proto._isValidNextFocusedCell = function(nextFocusedCellData, focusedCellData) {
        if (!focusedCellData) {
            return true
        }
        var groupIndex = focusedCellData.groupIndex,
            allDay = focusedCellData.allDay;
        var nextGroupIndex = nextFocusedCellData.groupIndex,
            nextAllDay = nextFocusedCellData.allDay;
        return groupIndex === nextGroupIndex && allDay === nextAllDay
    };
    return CellsSelectionController
}();
exports.CellsSelectionController = CellsSelectionController;
