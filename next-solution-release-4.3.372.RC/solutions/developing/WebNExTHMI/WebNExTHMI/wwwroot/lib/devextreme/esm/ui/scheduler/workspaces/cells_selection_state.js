/**
 * DevExtreme (esm/ui/scheduler/workspaces/cells_selection_state.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import dateUtils from "../../../core/utils/date";
export default class CellsSelectionState {
    constructor(viewDataProvider) {
        this._viewDataProvider = viewDataProvider;
        this._focusedCell = null;
        this._selectedCells = null;
        this._firstSelectedCell = null;
        this._prevFocusedCell = null;
        this._prevSelectedCells = null
    }
    get viewDataProvider() {
        return this._viewDataProvider
    }
    get focusedCell() {
        var focusedCell = this._focusedCell;
        if (!focusedCell) {
            return
        }
        var {
            groupIndex: groupIndex,
            startDate: startDate,
            allDay: allDay
        } = focusedCell;
        var cellInfo = {
            groupIndex: groupIndex,
            startDate: startDate,
            isAllDay: allDay,
            index: focusedCell.index
        };
        var cellPosition = this.viewDataProvider.findCellPositionInMap(cellInfo);
        return {
            coordinates: cellPosition,
            cellData: focusedCell
        }
    }
    setFocusedCell(rowIndex, columnIndex, isAllDay) {
        if (rowIndex >= 0) {
            var cell = this._viewDataProvider.getCellData(rowIndex, columnIndex, isAllDay);
            this._focusedCell = cell
        }
    }
    setSelectedCells(lastCellCoordinates) {
        var firstCellCoordinates = arguments.length > 1 && void 0 !== arguments[1] ? arguments[1] : void 0;
        var viewDataProvider = this._viewDataProvider;
        var {
            rowIndex: lastRowIndex,
            columnIndex: lastColumnIndex,
            allDay: isLastCellAllDay
        } = lastCellCoordinates;
        if (lastRowIndex < 0) {
            return
        }
        var firstCell = firstCellCoordinates ? viewDataProvider.getCellData(firstCellCoordinates.rowIndex, firstCellCoordinates.columnIndex, firstCellCoordinates.allDay) : this._firstSelectedCell;
        var lastCell = viewDataProvider.getCellData(lastRowIndex, lastColumnIndex, isLastCellAllDay);
        this._firstSelectedCell = firstCell;
        if (firstCell.startDate.getTime() > lastCell.startDate.getTime()) {
            [firstCell, lastCell] = [lastCell, firstCell]
        }
        var {
            startDate: firstStartDate,
            groupIndex: firstGroupIndex,
            index: firstCellIndex
        } = firstCell;
        var {
            startDate: lastStartDate,
            index: lastCellIndex
        } = lastCell;
        var cells = viewDataProvider.getCellsByGroupIndexAndAllDay(firstGroupIndex, isLastCellAllDay);
        var filteredCells = cells.reduce((selectedCells, cellsRow) => {
            var filterData = {
                firstDate: firstStartDate,
                lastDate: lastStartDate,
                firstIndex: firstCellIndex,
                lastIndex: lastCellIndex
            };
            var filteredRow = this._filterCellsByDateAndIndex(cellsRow, filterData);
            selectedCells.push(...filteredRow);
            return selectedCells
        }, []);
        this._selectedCells = filteredCells.sort((firstCell, secondCell) => firstCell.startDate.getTime() - secondCell.startDate.getTime())
    }
    setSelectedCellsByData(selectedCellsData) {
        this._selectedCells = selectedCellsData
    }
    getSelectedCells() {
        return this._selectedCells
    }
    releaseSelectedAndFocusedCells() {
        this.releaseSelectedCells();
        this.releaseFocusedCell()
    }
    releaseSelectedCells() {
        this._prevSelectedCells = this._selectedCells;
        this._prevFirstSelectedCell = this._firstSelectedCell;
        this._selectedCells = null;
        this._firstSelectedCell = null
    }
    releaseFocusedCell() {
        this._prevFocusedCell = this._focusedCell;
        this._focusedCell = null
    }
    restoreSelectedAndFocusedCells() {
        this._selectedCells = this._selectedCells || this._prevSelectedCells;
        this._focusedCell = this._focusedCell || this._prevFocusedCell;
        this._firstSelectedCell = this._firstSelectedCell || this._prevFirstSelectedCell;
        this._prevSelectedCells = null;
        this._prevFirstSelectedCell = null;
        this._prevFocusedCell = null
    }
    clearSelectedAndFocusedCells() {
        this._prevSelectedCells = null;
        this._selectedCells = null;
        this._prevFocusedCell = null;
        this._focusedCell = null
    }
    _filterCellsByDateAndIndex(cellsRow, filterData) {
        var {
            firstDate: firstDate,
            lastDate: lastDate,
            firstIndex: firstIndex,
            lastIndex: lastIndex
        } = filterData;
        var firstDay = dateUtils.trimTime(firstDate).getTime();
        var lastDay = dateUtils.trimTime(lastDate).getTime();
        return cellsRow.filter(cell => {
            var {
                startDate: startDate,
                index: index
            } = cell;
            var day = dateUtils.trimTime(startDate).getTime();
            var daysAndIndexes = {
                day: day,
                index: index,
                firstDay: firstDay,
                firstIndex: firstIndex,
                lastDay: lastDay,
                lastIndex: lastIndex
            };
            return this._compareCellsByDateAndIndex(daysAndIndexes)
        })
    }
    _compareCellsByDateAndIndex(daysAndIndexes) {
        var {
            day: day,
            index: index,
            firstDay: firstDay,
            firstIndex: firstIndex,
            lastDay: lastDay,
            lastIndex: lastIndex
        } = daysAndIndexes;
        if (firstDay === lastDay) {
            var validFirstIndex = firstIndex;
            var validLastIndex = lastIndex;
            if (validFirstIndex > validLastIndex) {
                [validFirstIndex, validLastIndex] = [validLastIndex, validFirstIndex]
            }
            return firstDay === day && index >= validFirstIndex && index <= validLastIndex
        } else {
            return day === firstDay && index >= firstIndex || day === lastDay && index <= lastIndex || firstDay < day && day < lastDay
        }
    }
}
