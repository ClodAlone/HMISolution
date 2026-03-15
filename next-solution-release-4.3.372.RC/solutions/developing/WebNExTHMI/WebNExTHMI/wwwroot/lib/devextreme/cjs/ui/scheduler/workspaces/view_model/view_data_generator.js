/**
 * DevExtreme (cjs/ui/scheduler/workspaces/view_model/view_data_generator.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.ViewDataGenerator = void 0;
var _date = _interopRequireDefault(require("../../../../core/utils/date"));
var _constants = require("../../constants");
var _utils = require("../../resources/utils");
var _base = require("../../../../renovation/ui/scheduler/view_model/to_test/views/utils/base");

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
var HOUR_MS = _date.default.dateToMilliseconds("hour");
var DAY_MS = _date.default.dateToMilliseconds("day");
var ViewDataGenerator = function() {
    function ViewDataGenerator() {}
    var _proto = ViewDataGenerator.prototype;
    _proto.isSkippedDate = function() {
        return false
    };
    _proto.getStartViewDate = function(options) {
        return this._calculateStartViewDate(options)
    };
    _proto.getCompleteViewDataMap = function(options) {
        var _viewDataMap;
        var groups = options.groups,
            isGroupedByDate = options.isGroupedByDate,
            isHorizontalGrouping = options.isHorizontalGrouping,
            isVerticalGrouping = options.isVerticalGrouping,
            intervalCount = options.intervalCount,
            currentDate = options.currentDate,
            viewType = options.viewType,
            startDayHour = options.startDayHour,
            endDayHour = options.endDayHour,
            hoursInterval = options.hoursInterval;
        this._setVisibilityDates(options);
        this.setHiddenInterval(startDayHour, endDayHour, hoursInterval);
        var groupsList = (0, _utils.getAllGroups)(groups);
        var cellCountInGroupRow = this.getCellCount({
            intervalCount: intervalCount,
            currentDate: currentDate,
            viewType: viewType,
            startDayHour: startDayHour,
            endDayHour: endDayHour,
            hoursInterval: hoursInterval
        });
        var rowCountInGroup = this.getRowCount({
            intervalCount: intervalCount,
            currentDate: currentDate,
            viewType: viewType,
            hoursInterval: hoursInterval,
            startDayHour: startDayHour,
            endDayHour: endDayHour
        });
        var viewDataMap = [];
        var allDayPanelData = this._generateAllDayPanelData(options, rowCountInGroup, cellCountInGroupRow);
        var viewCellsData = this._generateViewCellsData(options, rowCountInGroup, cellCountInGroupRow);
        allDayPanelData && viewDataMap.push(allDayPanelData);
        (_viewDataMap = viewDataMap).push.apply(_viewDataMap, _toConsumableArray(viewCellsData));
        if (isHorizontalGrouping && !isGroupedByDate) {
            viewDataMap = this._transformViewDataMapForHorizontalGrouping(viewDataMap, groupsList)
        }
        if (isVerticalGrouping) {
            viewDataMap = this._transformViewDataMapForVerticalGrouping(viewDataMap, groupsList)
        }
        if (isGroupedByDate) {
            viewDataMap = this._transformViewDataMapForGroupingByDate(viewDataMap, groupsList)
        }
        var completeViewDataMap = this._addKeysToCells(viewDataMap);
        return completeViewDataMap
    };
    _proto._transformViewDataMapForHorizontalGrouping = function(viewDataMap, groupsList) {
        var result = viewDataMap.map((function(row) {
            return row.slice()
        }));
        groupsList.slice(1).forEach((function(groups, index) {
            var groupIndex = index + 1;
            viewDataMap.forEach((function(row, rowIndex) {
                var _result$rowIndex;
                var nextGroupRow = row.map((function(cellData) {
                    return _extends({}, cellData, {
                        groups: groups,
                        groupIndex: groupIndex
                    })
                }));
                (_result$rowIndex = result[rowIndex]).push.apply(_result$rowIndex, _toConsumableArray(nextGroupRow))
            }))
        }));
        return result
    };
    _proto._transformViewDataMapForVerticalGrouping = function(viewDataMap, groupsList) {
        var result = viewDataMap.map((function(row) {
            return row.slice()
        }));
        groupsList.slice(1).forEach((function(groups, index) {
            var groupIndex = index + 1;
            var nextGroupMap = viewDataMap.map((function(cellsRow) {
                var nextRow = cellsRow.map((function(cellData) {
                    return _extends({}, cellData, {
                        groupIndex: groupIndex,
                        groups: groups
                    })
                }));
                return nextRow
            }));
            result.push.apply(result, _toConsumableArray(nextGroupMap))
        }));
        return result
    };
    _proto._transformViewDataMapForGroupingByDate = function(viewDataMap, groupsList) {
        var correctedGroupList = groupsList.slice(1);
        var correctedGroupCount = correctedGroupList.length;
        var result = viewDataMap.map((function(cellsRow) {
            var groupedByDateCellsRow = cellsRow.reduce((function(currentRow, cell) {
                var rowWithCurrentCell = [].concat(_toConsumableArray(currentRow), [_extends({}, cell, {
                    isFirstGroupCell: true,
                    isLastGroupCell: 0 === correctedGroupCount
                })], _toConsumableArray(correctedGroupList.map((function(groups, index) {
                    return _extends({}, cell, {
                        groups: groups,
                        groupIndex: index + 1,
                        isFirstGroupCell: false,
                        isLastGroupCell: index === correctedGroupCount - 1
                    })
                }))));
                return rowWithCurrentCell
            }), []);
            return groupedByDateCellsRow
        }));
        return result
    };
    _proto._addKeysToCells = function(viewDataMap) {
        var totalColumnCount = viewDataMap[0].length;
        var _viewDataMap$reduce = viewDataMap.reduce((function(_ref, row, rowIndex) {
                var allDayPanelsCount = _ref.allDayPanelsCount,
                    currentViewDataMap = _ref.currentViewDataMap;
                var isAllDay = row[0].allDay;
                var keyBase = (rowIndex - allDayPanelsCount) * totalColumnCount;
                var currentAllDayPanelsCount = isAllDay ? allDayPanelsCount + 1 : allDayPanelsCount;
                currentViewDataMap[rowIndex].forEach((function(cell, columnIndex) {
                    cell.key = keyBase + columnIndex
                }));
                return {
                    allDayPanelsCount: currentAllDayPanelsCount,
                    currentViewDataMap: currentViewDataMap
                }
            }), {
                allDayPanelsCount: 0,
                currentViewDataMap: viewDataMap
            }),
            result = _viewDataMap$reduce.currentViewDataMap;
        return result
    };
    _proto.generateViewDataMap = function(completeViewDataMap, options) {
        var rowCount = options.rowCount,
            startCellIndex = options.startCellIndex,
            startRowIndex = options.startRowIndex,
            cellCount = options.cellCount,
            isVerticalGrouping = options.isVerticalGrouping,
            isAllDayPanelVisible = options.isAllDayPanelVisible;
        var sliceCells = function(row, rowIndex, startIndex, count) {
            var sliceToIndex = void 0 !== count ? startIndex + count : void 0;
            return row.slice(startIndex, sliceToIndex).map((function(cellData, columnIndex) {
                return {
                    cellData: cellData,
                    position: {
                        rowIndex: rowIndex,
                        columnIndex: columnIndex
                    }
                }
            }))
        };
        var correctedStartRowIndex = startRowIndex;
        var allDayPanelMap = [];
        if (this._isStandaloneAllDayPanel(isVerticalGrouping, isAllDayPanelVisible)) {
            correctedStartRowIndex++;
            allDayPanelMap = sliceCells(completeViewDataMap[0], 0, startCellIndex, cellCount)
        }
        var displayedRowCount = (0, _base.getDisplayedRowCount)(rowCount, completeViewDataMap);
        var dateTableMap = completeViewDataMap.slice(correctedStartRowIndex, correctedStartRowIndex + displayedRowCount).map((function(row, rowIndex) {
            return sliceCells(row, rowIndex, startCellIndex, cellCount)
        }));
        return {
            allDayPanelMap: allDayPanelMap,
            dateTableMap: dateTableMap
        }
    };
    _proto._isStandaloneAllDayPanel = function(isVerticalGrouping, isAllDayPanelVisible) {
        return !isVerticalGrouping && isAllDayPanelVisible
    };
    _proto.getViewDataFromMap = function(completeViewDataMap, viewDataMap, options) {
        var topVirtualRowHeight = options.topVirtualRowHeight,
            bottomVirtualRowHeight = options.bottomVirtualRowHeight,
            leftVirtualCellWidth = options.leftVirtualCellWidth,
            rightVirtualCellWidth = options.rightVirtualCellWidth,
            cellCount = options.cellCount,
            rowCount = options.rowCount,
            startRowIndex = options.startRowIndex,
            startCellIndex = options.startCellIndex,
            isProvideVirtualCellsWidth = options.isProvideVirtualCellsWidth,
            isGroupedAllDayPanel = options.isGroupedAllDayPanel,
            isVerticalGrouping = options.isVerticalGrouping,
            isAllDayPanelVisible = options.isAllDayPanelVisible;
        var allDayPanelMap = viewDataMap.allDayPanelMap,
            dateTableMap = viewDataMap.dateTableMap;
        var _dateTableMap$reduce = dateTableMap.reduce((function(_ref2, cellsRow) {
                var previousGroupIndex = _ref2.previousGroupIndex,
                    previousGroupedData = _ref2.previousGroupedData;
                var cellDataRow = cellsRow.map((function(_ref3) {
                    var cellData = _ref3.cellData;
                    return cellData
                }));
                var firstCell = cellDataRow[0];
                var isAllDayRow = firstCell.allDay;
                var currentGroupIndex = firstCell.groupIndex;
                if (currentGroupIndex !== previousGroupIndex) {
                    previousGroupedData.push({
                        dateTable: [],
                        isGroupedAllDayPanel: isGroupedAllDayPanel,
                        groupIndex: currentGroupIndex
                    })
                }
                if (isAllDayRow) {
                    previousGroupedData[previousGroupedData.length - 1].allDayPanel = cellDataRow
                } else {
                    previousGroupedData[previousGroupedData.length - 1].dateTable.push(cellDataRow)
                }
                return {
                    previousGroupedData: previousGroupedData,
                    previousGroupIndex: currentGroupIndex
                }
            }), {
                previousGroupIndex: -1,
                previousGroupedData: []
            }),
            groupedData = _dateTableMap$reduce.previousGroupedData;
        if (this._isStandaloneAllDayPanel(isVerticalGrouping, isAllDayPanelVisible)) {
            groupedData[0].allDayPanel = allDayPanelMap.map((function(_ref4) {
                var cellData = _ref4.cellData;
                return cellData
            }))
        }
        var totalCellCount = (0, _base.getTotalCellCountByCompleteData)(completeViewDataMap);
        var totalRowCount = (0, _base.getTotalRowCountByCompleteData)(completeViewDataMap);
        var displayedCellCount = (0, _base.getDisplayedCellCount)(cellCount, completeViewDataMap);
        var displayedRowCount = (0, _base.getDisplayedRowCount)(rowCount, completeViewDataMap);
        return {
            groupedData: groupedData,
            topVirtualRowHeight: topVirtualRowHeight,
            bottomVirtualRowHeight: bottomVirtualRowHeight,
            leftVirtualCellWidth: isProvideVirtualCellsWidth ? leftVirtualCellWidth : void 0,
            rightVirtualCellWidth: isProvideVirtualCellsWidth ? rightVirtualCellWidth : void 0,
            isGroupedAllDayPanel: isGroupedAllDayPanel,
            leftVirtualCellCount: startCellIndex,
            rightVirtualCellCount: void 0 === cellCount ? 0 : totalCellCount - startCellIndex - displayedCellCount,
            topVirtualRowCount: startRowIndex,
            bottomVirtualRowCount: totalRowCount - startRowIndex - displayedRowCount
        }
    };
    _proto._generateViewCellsData = function(options, rowCount, cellCountInGroupRow) {
        var viewCellsData = [];
        for (var rowIndex = 0; rowIndex < rowCount; rowIndex += 1) {
            viewCellsData.push(this._generateCellsRow(options, false, rowIndex, rowCount, cellCountInGroupRow))
        }
        return viewCellsData
    };
    _proto._generateAllDayPanelData = function(options, rowCount, columnCount) {
        if (!options.isAllDayPanelVisible) {
            return null
        }
        return this._generateCellsRow(options, true, 0, rowCount, columnCount)
    };
    _proto._generateCellsRow = function(options, allDay, rowIndex, rowCount, columnCount) {
        var cellsRow = [];
        for (var columnIndex = 0; columnIndex < columnCount; ++columnIndex) {
            var cellDataValue = this.getCellData(rowIndex, columnIndex, options, allDay);
            cellDataValue.index = rowIndex * columnCount + columnIndex;
            cellDataValue.isFirstGroupCell = this._isFirstGroupCell(rowIndex, columnIndex, options, rowCount, columnCount);
            cellDataValue.isLastGroupCell = this._isLastGroupCell(rowIndex, columnIndex, options, rowCount, columnCount);
            cellsRow.push(cellDataValue)
        }
        return cellsRow
    };
    _proto.getCellData = function(rowIndex, columnIndex, options, allDay) {
        return allDay ? this.prepareAllDayCellData(options, rowIndex, columnIndex) : this.prepareCellData(options, rowIndex, columnIndex)
    };
    _proto.prepareCellData = function(options, rowIndex, columnIndex) {
        var groups = options.groups,
            startDayHour = options.startDayHour,
            endDayHour = options.endDayHour,
            interval = options.interval,
            hoursInterval = options.hoursInterval;
        var groupsList = (0, _utils.getAllGroups)(groups);
        var startDate = this.getDateByCellIndices(options, rowIndex, columnIndex, this.getCellCountInDay(startDayHour, endDayHour, hoursInterval));
        var endDate = this.calculateEndDate(startDate, interval, endDayHour);
        var data = {
            startDate: startDate,
            endDate: endDate,
            allDay: this.tableAllDay,
            groupIndex: 0
        };
        if (groupsList.length > 0) {
            data.groups = groupsList[0]
        }
        return data
    };
    _proto.prepareAllDayCellData = function(options, rowIndex, columnIndex) {
        var data = this.prepareCellData(options, rowIndex, columnIndex);
        var startDate = _date.default.trimTime(data.startDate);
        return _extends({}, data, {
            startDate: startDate,
            endDate: startDate,
            allDay: true
        })
    };
    _proto.getDateByCellIndices = function(options, rowIndex, columnIndex, cellCountInDay) {
        var startViewDate = options.startViewDate;
        var startDayHour = options.startDayHour,
            interval = options.interval,
            firstDayOfWeek = options.firstDayOfWeek,
            intervalCount = options.intervalCount;
        var isStartViewDateDuringDST = startViewDate.getHours() !== Math.floor(startDayHour);
        if (isStartViewDateDuringDST) {
            var dateWithCorrectHours = (0, _base.getStartViewDateWithoutDST)(startViewDate, startDayHour);
            startViewDate = new Date(dateWithCorrectHours - _date.default.dateToMilliseconds("day"))
        }
        var columnCountBase = this.getCellCount(options);
        var rowCountBase = this.getRowCount(options);
        var cellIndex = this._calculateCellIndex(rowIndex, columnIndex, rowCountBase, columnCountBase);
        var millisecondsOffset = this.getMillisecondsOffset(cellIndex, interval, cellCountInDay);
        var offsetByCount = this.isWorkView ? this.getTimeOffsetByColumnIndex(columnIndex, this.getFirstDayOfWeek(firstDayOfWeek), columnCountBase, intervalCount) : 0;
        var startViewDateTime = startViewDate.getTime();
        var currentDate = new Date(startViewDateTime + millisecondsOffset + offsetByCount);
        var timeZoneDifference = isStartViewDateDuringDST ? 0 : _date.default.getTimezonesDifference(startViewDate, currentDate);
        currentDate.setTime(currentDate.getTime() + timeZoneDifference);
        return currentDate
    };
    _proto.getMillisecondsOffset = function(cellIndex, interval, cellCountInDay) {
        var dayIndex = Math.floor(cellIndex / cellCountInDay);
        var realHiddenInterval = dayIndex * this.hiddenInterval;
        return interval * cellIndex + realHiddenInterval
    };
    _proto.getTimeOffsetByColumnIndex = function(columnIndex, firstDayOfWeek, columnCount, intervalCount) {
        var firstDayOfWeekDiff = Math.max(0, firstDayOfWeek - 1);
        var columnsInWeek = columnCount / intervalCount;
        var weekendCount = Math.floor((columnIndex + firstDayOfWeekDiff) / columnsInWeek);
        return DAY_MS * weekendCount * 2
    };
    _proto.calculateEndDate = function(startDate, interval, endDayHour) {
        var result = new Date(startDate);
        result.setMilliseconds(result.getMilliseconds() + Math.round(interval));
        return result
    };
    _proto._calculateCellIndex = function(rowIndex, columnIndex, rowCount, columnCount) {
        return (0, _base.calculateCellIndex)(rowIndex, columnIndex, rowCount)
    };
    _proto.generateGroupedDataMap = function(viewDataMap) {
        var allDayPanelMap = viewDataMap.allDayPanelMap,
            dateTableMap = viewDataMap.dateTableMap;
        var _dateTableMap$reduce2 = dateTableMap.reduce((function(previousOptions, cellsRow) {
                var previousGroupedDataMap = previousOptions.previousGroupedDataMap,
                    previousRowIndex = previousOptions.previousRowIndex,
                    previousGroupIndex = previousOptions.previousGroupIndex;
                var currentGroupIndex = cellsRow[0].cellData.groupIndex;
                var currentRowIndex = currentGroupIndex === previousGroupIndex ? previousRowIndex + 1 : 0;
                cellsRow.forEach((function(cell) {
                    var groupIndex = cell.cellData.groupIndex;
                    if (!previousGroupedDataMap[groupIndex]) {
                        previousGroupedDataMap[groupIndex] = []
                    }
                    if (!previousGroupedDataMap[groupIndex][currentRowIndex]) {
                        previousGroupedDataMap[groupIndex][currentRowIndex] = []
                    }
                    previousGroupedDataMap[groupIndex][currentRowIndex].push(cell)
                }));
                return {
                    previousGroupedDataMap: previousGroupedDataMap,
                    previousRowIndex: currentRowIndex,
                    previousGroupIndex: currentGroupIndex
                }
            }), {
                previousGroupedDataMap: [],
                previousRowIndex: -1,
                previousGroupIndex: -1
            }),
            dateTableGroupedMap = _dateTableMap$reduce2.previousGroupedDataMap;
        var allDayPanelGroupedMap = [];
        null === allDayPanelMap || void 0 === allDayPanelMap ? void 0 : allDayPanelMap.forEach((function(cell) {
            var groupIndex = cell.cellData.groupIndex;
            if (!allDayPanelGroupedMap[groupIndex]) {
                allDayPanelGroupedMap[groupIndex] = []
            }
            allDayPanelGroupedMap[groupIndex].push(cell)
        }));
        return {
            allDayPanelGroupedMap: allDayPanelGroupedMap,
            dateTableGroupedMap: dateTableGroupedMap
        }
    };
    _proto._isFirstGroupCell = function(rowIndex, columnIndex, options, rowCount, columnCount) {
        var groupOrientation = options.groupOrientation,
            groups = options.groups,
            isGroupedByDate = options.isGroupedByDate;
        var groupCount = (0, _utils.getGroupCount)(groups);
        if (isGroupedByDate) {
            return columnIndex % groupCount === 0
        }
        if (groupOrientation === _constants.HORIZONTAL_GROUP_ORIENTATION) {
            return columnIndex % columnCount === 0
        }
        return rowIndex % rowCount === 0
    };
    _proto._isLastGroupCell = function(rowIndex, columnIndex, options, rowCount, columnCount) {
        var groupOrientation = options.groupOrientation,
            groups = options.groups,
            isGroupedByDate = options.isGroupedByDate;
        var groupCount = (0, _utils.getGroupCount)(groups);
        if (isGroupedByDate) {
            return (columnIndex + 1) % groupCount === 0
        }
        if (groupOrientation === _constants.HORIZONTAL_GROUP_ORIENTATION) {
            return (columnIndex + 1) % columnCount === 0
        }
        return (rowIndex + 1) % rowCount === 0
    };
    _proto.markSelectedAndFocusedCells = function(viewDataMap, renderOptions) {
        var _this = this;
        var selectedCells = renderOptions.selectedCells,
            focusedCell = renderOptions.focusedCell;
        if (!selectedCells && !focusedCell) {
            return viewDataMap
        }
        var allDayPanelMap = viewDataMap.allDayPanelMap,
            dateTableMap = viewDataMap.dateTableMap;
        var nextDateTableMap = dateTableMap.map((function(row) {
            return _this._markSelectedAndFocusedCellsInRow(row, selectedCells, focusedCell)
        }));
        var nextAllDayMap = this._markSelectedAndFocusedCellsInRow(allDayPanelMap, selectedCells, focusedCell);
        return {
            allDayPanelMap: nextAllDayMap,
            dateTableMap: nextDateTableMap
        }
    };
    _proto._markSelectedAndFocusedCellsInRow = function(dataRow, selectedCells, focusedCell) {
        return dataRow.map((function(cell) {
            var _cell$cellData = cell.cellData,
                index = _cell$cellData.index,
                groupIndex = _cell$cellData.groupIndex,
                allDay = _cell$cellData.allDay,
                startDate = _cell$cellData.startDate;
            var indexInSelectedCells = selectedCells.findIndex((function(_ref5) {
                var selectedCellIndex = _ref5.index,
                    selectedCellGroupIndex = _ref5.groupIndex,
                    selectedCellAllDay = _ref5.allDay,
                    selectedCellStartDate = _ref5.startDate;
                return groupIndex === selectedCellGroupIndex && (index === selectedCellIndex || void 0 === selectedCellIndex && startDate.getTime() === selectedCellStartDate.getTime()) && !!allDay === !!selectedCellAllDay
            }));
            var isFocused = !!focusedCell && index === focusedCell.cellData.index && groupIndex === focusedCell.cellData.groupIndex && allDay === focusedCell.cellData.allDay;
            if (!isFocused && -1 === indexInSelectedCells) {
                return cell
            }
            return _extends({}, cell, {
                cellData: _extends({}, cell.cellData, {
                    isSelected: indexInSelectedCells > -1,
                    isFocused: isFocused
                })
            })
        }))
    };
    _proto.getInterval = function(hoursInterval) {
        if (void 0 === this._interval) {
            this._interval = hoursInterval * HOUR_MS
        }
        return this._interval
    };
    _proto._getIntervalDuration = function(intervalCount) {
        return _date.default.dateToMilliseconds("day") * intervalCount
    };
    _proto._setVisibilityDates = function() {};
    _proto.getCellCountInDay = function(startDayHour, endDayHour, hoursInterval) {
        var result = (0, _base.calculateDayDuration)(startDayHour, endDayHour) / hoursInterval;
        return Math.ceil(result)
    };
    _proto.getCellCount = function(options) {
        var intervalCount = options.intervalCount,
            viewType = options.viewType,
            startDayHour = options.startDayHour,
            endDayHour = options.endDayHour,
            hoursInterval = options.hoursInterval;
        var cellCountInDay = this.getCellCountInDay(startDayHour, endDayHour, hoursInterval);
        var columnCountInDay = (0, _base.isHorizontalView)(viewType) ? cellCountInDay : 1;
        return this.daysInInterval * intervalCount * columnCountInDay
    };
    _proto.getRowCount = function(options) {
        var viewType = options.viewType,
            startDayHour = options.startDayHour,
            endDayHour = options.endDayHour,
            hoursInterval = options.hoursInterval;
        var cellCountInDay = this.getCellCountInDay(startDayHour, endDayHour, hoursInterval);
        var rowCountInDay = !(0, _base.isHorizontalView)(viewType) ? cellCountInDay : 1;
        return rowCountInDay
    };
    _proto.setHiddenInterval = function(startDayHour, endDayHour, hoursInterval) {
        this.hiddenInterval = DAY_MS - this.getVisibleDayDuration(startDayHour, endDayHour, hoursInterval)
    };
    _proto.getVisibleDayDuration = function(startDayHour, endDayHour, hoursInterval) {
        var cellCountInDay = this.getCellCountInDay(startDayHour, endDayHour, hoursInterval);
        return hoursInterval * cellCountInDay * HOUR_MS
    };
    _proto.getFirstDayOfWeek = function(firstDayOfWeekOption) {
        return firstDayOfWeekOption
    };
    _createClass(ViewDataGenerator, [{
        key: "daysInInterval",
        get: function() {
            return 1
        }
    }, {
        key: "isWorkView",
        get: function() {
            return false
        }
    }, {
        key: "tableAllDay",
        get: function() {
            return false
        }
    }]);
    return ViewDataGenerator
}();
exports.ViewDataGenerator = ViewDataGenerator;
