/**
 * DevExtreme (esm/ui/scheduler/workspaces/cells_selection_controller.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import _extends from "@babel/runtime/helpers/esm/extends";
import {
    isDateAndTimeView
} from "../../../renovation/ui/scheduler/view_model/to_test/views/utils/base";
export class CellsSelectionController {
    handleArrowClick(options) {
        var {
            key: key,
            focusedCellPosition: focusedCellPosition,
            edgeIndices: edgeIndices,
            getCellDataByPosition: getCellDataByPosition,
            isAllDayPanelCell: isAllDayPanelCell
        } = options;
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
    }
    getCellFromNextRowPosition(focusedCellPosition, direction, edgeIndices) {
        var {
            columnIndex: columnIndex,
            rowIndex: rowIndex
        } = focusedCellPosition;
        var deltaPosition = "next" === direction ? 1 : -1;
        var nextRowIndex = rowIndex + deltaPosition;
        var validRowIndex = nextRowIndex >= 0 && nextRowIndex <= edgeIndices.lastRowIndex ? nextRowIndex : rowIndex;
        return {
            columnIndex: columnIndex,
            rowIndex: validRowIndex
        }
    }
    getCellFromNextColumnPosition(options) {
        var {
            focusedCellPosition: focusedCellPosition,
            direction: direction,
            edgeIndices: edgeIndices,
            isRTL: isRTL,
            isGroupedByDate: isGroupedByDate,
            groupCount: groupCount,
            isMultiSelection: isMultiSelection,
            viewType: viewType
        } = options;
        var {
            columnIndex: columnIndex,
            rowIndex: rowIndex
        } = focusedCellPosition;
        var {
            firstColumnIndex: firstColumnIndex,
            lastColumnIndex: lastColumnIndex,
            firstRowIndex: firstRowIndex,
            lastRowIndex: lastRowIndex
        } = edgeIndices;
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
        return isDateAndTimeView(viewType) ? focusedCellPosition : this._processEdgeCell({
            nextColumnIndex: nextColumnIndex,
            rowIndex: rowIndex,
            columnIndex: columnIndex,
            firstColumnIndex: firstColumnIndex,
            lastColumnIndex: lastColumnIndex,
            firstRowIndex: firstRowIndex,
            lastRowIndex: lastRowIndex,
            step: step
        })
    }
    _processEdgeCell(options) {
        var {
            nextColumnIndex: nextColumnIndex,
            rowIndex: rowIndex,
            columnIndex: columnIndex,
            firstColumnIndex: firstColumnIndex,
            lastColumnIndex: lastColumnIndex,
            firstRowIndex: firstRowIndex,
            lastRowIndex: lastRowIndex,
            step: step
        } = options;
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
    }
    moveToCell(options) {
        var {
            isMultiSelection: isMultiSelection,
            isMultiSelectionAllowed: isMultiSelectionAllowed,
            focusedCellData: focusedCellData,
            currentCellData: currentCellData
        } = options;
        var isValidMultiSelection = isMultiSelection && isMultiSelectionAllowed;
        var nextFocusedCellData = isValidMultiSelection ? this._getNextCellData(currentCellData, focusedCellData) : currentCellData;
        return nextFocusedCellData
    }
    _getNextCellData(nextFocusedCellData, focusedCellData, isVirtualCell) {
        if (isVirtualCell) {
            return focusedCellData
        }
        var isValidNextFocusedCell = this._isValidNextFocusedCell(nextFocusedCellData, focusedCellData);
        return isValidNextFocusedCell ? nextFocusedCellData : focusedCellData
    }
    _isValidNextFocusedCell(nextFocusedCellData, focusedCellData) {
        if (!focusedCellData) {
            return true
        }
        var {
            groupIndex: groupIndex,
            allDay: allDay
        } = focusedCellData;
        var {
            groupIndex: nextGroupIndex,
            allDay: nextAllDay
        } = nextFocusedCellData;
        return groupIndex === nextGroupIndex && allDay === nextAllDay
    }
}
