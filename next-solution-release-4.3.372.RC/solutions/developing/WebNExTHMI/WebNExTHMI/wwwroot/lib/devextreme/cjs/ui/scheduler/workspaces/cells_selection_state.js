/**
 * DevExtreme (cjs/ui/scheduler/workspaces/cells_selection_state.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.default = void 0;
var _date = _interopRequireDefault(require("../../../core/utils/date"));

function _interopRequireDefault(obj) {
    return obj && obj.__esModule ? obj : {
        default: obj
    }
}

function _toConsumableArray(arr) {
    return _arrayWithoutHoles(arr) || _iterableToArray(arr) || _unsupportedIterableToArray(arr) || _nonIterableSpread()
}

function _nonIterableSpread() {
    throw new TypeError("Invalid attempt to spread non-iterable instance.\nIn order to be iterable, non-array objects must have a [Symbol.iterator]() method.")
}

function _unsupportedIterableToArray(o, minLen) {
    if (!o) {
        return
    }
    if ("string" === typeof o) {
        return _arrayLikeToArray(o, minLen)
    }
    var n = Object.prototype.toString.call(o).slice(8, -1);
    if ("Object" === n && o.constructor) {
        n = o.constructor.name
    }
    if ("Map" === n || "Set" === n) {
        return Array.from(o)
    }
    if ("Arguments" === n || /^(?:Ui|I)nt(?:8|16|32)(?:Clamped)?Array$/.test(n)) {
        return _arrayLikeToArray(o, minLen)
    }
}

function _iterableToArray(iter) {
    if ("undefined" !== typeof Symbol && null != iter[Symbol.iterator] || null != iter["@@iterator"]) {
        return Array.from(iter)
    }
}

function _arrayWithoutHoles(arr) {
    if (Array.isArray(arr)) {
        return _arrayLikeToArray(arr)
    }
}

function _arrayLikeToArray(arr, len) {
    if (null == len || len > arr.length) {
        len = arr.length
    }
    for (var i = 0, arr2 = new Array(len); i < len; i++) {
        arr2[i] = arr[i]
    }
    return arr2
}

function _defineProperties(target, props) {
    for (var i = 0; i < props.length; i++) {
        var descriptor = props[i];
        descriptor.enumerable = descriptor.enumerable || false;
        descriptor.configurable = true;
        if ("value" in descriptor) {
            descriptor.writable = true
        }
        Object.defineProperty(target, descriptor.key, descriptor)
    }
}

function _createClass(Constructor, protoProps, staticProps) {
    if (protoProps) {
        _defineProperties(Constructor.prototype, protoProps)
    }
    if (staticProps) {
        _defineProperties(Constructor, staticProps)
    }
    Object.defineProperty(Constructor, "prototype", {
        writable: false
    });
    return Constructor
}
var CellsSelectionState = function() {
    function CellsSelectionState(viewDataProvider) {
        this._viewDataProvider = viewDataProvider;
        this._focusedCell = null;
        this._selectedCells = null;
        this._firstSelectedCell = null;
        this._prevFocusedCell = null;
        this._prevSelectedCells = null
    }
    var _proto = CellsSelectionState.prototype;
    _proto.setFocusedCell = function(rowIndex, columnIndex, isAllDay) {
        if (rowIndex >= 0) {
            var cell = this._viewDataProvider.getCellData(rowIndex, columnIndex, isAllDay);
            this._focusedCell = cell
        }
    };
    _proto.setSelectedCells = function(lastCellCoordinates) {
        var _this = this;
        var firstCellCoordinates = arguments.length > 1 && void 0 !== arguments[1] ? arguments[1] : void 0;
        var viewDataProvider = this._viewDataProvider;
        var lastRowIndex = lastCellCoordinates.rowIndex,
            lastColumnIndex = lastCellCoordinates.columnIndex,
            isLastCellAllDay = lastCellCoordinates.allDay;
        if (lastRowIndex < 0) {
            return
        }
        var firstCell = firstCellCoordinates ? viewDataProvider.getCellData(firstCellCoordinates.rowIndex, firstCellCoordinates.columnIndex, firstCellCoordinates.allDay) : this._firstSelectedCell;
        var lastCell = viewDataProvider.getCellData(lastRowIndex, lastColumnIndex, isLastCellAllDay);
        this._firstSelectedCell = firstCell;
        if (firstCell.startDate.getTime() > lastCell.startDate.getTime()) {
            var _ref = [lastCell, firstCell];
            firstCell = _ref[0];
            lastCell = _ref[1]
        }
        var _firstCell = firstCell,
            firstStartDate = _firstCell.startDate,
            firstGroupIndex = _firstCell.groupIndex,
            firstCellIndex = _firstCell.index;
        var _lastCell = lastCell,
            lastStartDate = _lastCell.startDate,
            lastCellIndex = _lastCell.index;
        var cells = viewDataProvider.getCellsByGroupIndexAndAllDay(firstGroupIndex, isLastCellAllDay);
        var filteredCells = cells.reduce((function(selectedCells, cellsRow) {
            var filterData = {
                firstDate: firstStartDate,
                lastDate: lastStartDate,
                firstIndex: firstCellIndex,
                lastIndex: lastCellIndex
            };
            var filteredRow = _this._filterCellsByDateAndIndex(cellsRow, filterData);
            selectedCells.push.apply(selectedCells, _toConsumableArray(filteredRow));
            return selectedCells
        }), []);
        this._selectedCells = filteredCells.sort((function(firstCell, secondCell) {
            return firstCell.startDate.getTime() - secondCell.startDate.getTime()
        }))
    };
    _proto.setSelectedCellsByData = function(selectedCellsData) {
        this._selectedCells = selectedCellsData
    };
    _proto.getSelectedCells = function() {
        return this._selectedCells
    };
    _proto.releaseSelectedAndFocusedCells = function() {
        this.releaseSelectedCells();
        this.releaseFocusedCell()
    };
    _proto.releaseSelectedCells = function() {
        this._prevSelectedCells = this._selectedCells;
        this._prevFirstSelectedCell = this._firstSelectedCell;
        this._selectedCells = null;
        this._firstSelectedCell = null
    };
    _proto.releaseFocusedCell = function() {
        this._prevFocusedCell = this._focusedCell;
        this._focusedCell = null
    };
    _proto.restoreSelectedAndFocusedCells = function() {
        this._selectedCells = this._selectedCells || this._prevSelectedCells;
        this._focusedCell = this._focusedCell || this._prevFocusedCell;
        this._firstSelectedCell = this._firstSelectedCell || this._prevFirstSelectedCell;
        this._prevSelectedCells = null;
        this._prevFirstSelectedCell = null;
        this._prevFocusedCell = null
    };
    _proto.clearSelectedAndFocusedCells = function() {
        this._prevSelectedCells = null;
        this._selectedCells = null;
        this._prevFocusedCell = null;
        this._focusedCell = null
    };
    _proto._filterCellsByDateAndIndex = function(cellsRow, filterData) {
        var _this2 = this;
        var firstDate = filterData.firstDate,
            lastDate = filterData.lastDate,
            firstIndex = filterData.firstIndex,
            lastIndex = filterData.lastIndex;
        var firstDay = _date.default.trimTime(firstDate).getTime();
        var lastDay = _date.default.trimTime(lastDate).getTime();
        return cellsRow.filter((function(cell) {
            var startDate = cell.startDate,
                index = cell.index;
            var day = _date.default.trimTime(startDate).getTime();
            var daysAndIndexes = {
                day: day,
                index: index,
                firstDay: firstDay,
                firstIndex: firstIndex,
                lastDay: lastDay,
                lastIndex: lastIndex
            };
            return _this2._compareCellsByDateAndIndex(daysAndIndexes)
        }))
    };
    _proto._compareCellsByDateAndIndex = function(daysAndIndexes) {
        var day = daysAndIndexes.day,
            index = daysAndIndexes.index,
            firstDay = daysAndIndexes.firstDay,
            firstIndex = daysAndIndexes.firstIndex,
            lastDay = daysAndIndexes.lastDay,
            lastIndex = daysAndIndexes.lastIndex;
        if (firstDay === lastDay) {
            var validFirstIndex = firstIndex;
            var validLastIndex = lastIndex;
            if (validFirstIndex > validLastIndex) {
                var _ref2 = [validLastIndex, validFirstIndex];
                validFirstIndex = _ref2[0];
                validLastIndex = _ref2[1]
            }
            return firstDay === day && index >= validFirstIndex && index <= validLastIndex
        } else {
            return day === firstDay && index >= firstIndex || day === lastDay && index <= lastIndex || firstDay < day && day < lastDay
        }
    };
    _createClass(CellsSelectionState, [{
        key: "viewDataProvider",
        get: function() {
            return this._viewDataProvider
        }
    }, {
        key: "focusedCell",
        get: function() {
            var focusedCell = this._focusedCell;
            if (!focusedCell) {
                return
            }
            var groupIndex = focusedCell.groupIndex,
                startDate = focusedCell.startDate,
                allDay = focusedCell.allDay;
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
    }]);
    return CellsSelectionState
}();
exports.default = CellsSelectionState;
module.exports = exports.default;
module.exports.default = exports.default;
