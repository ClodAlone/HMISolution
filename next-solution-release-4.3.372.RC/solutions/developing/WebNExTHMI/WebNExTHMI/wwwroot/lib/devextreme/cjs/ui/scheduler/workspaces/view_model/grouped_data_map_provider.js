/**
 * DevExtreme (cjs/ui/scheduler/workspaces/view_model/grouped_data_map_provider.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.GroupedDataMapProvider = void 0;
var _date = _interopRequireDefault(require("../../../../core/utils/date"));
var _base = require("../../../../renovation/ui/scheduler/view_model/to_test/views/utils/base");

function _interopRequireDefault(obj) {
    return obj && obj.__esModule ? obj : {
        default: obj
    }
}
var GroupedDataMapProvider = function() {
    function GroupedDataMapProvider(viewDataGenerator, viewDataMap, completeViewDataMap, viewOptions) {
        this.groupedDataMap = viewDataGenerator.generateGroupedDataMap(viewDataMap);
        this.completeViewDataMap = completeViewDataMap;
        this._viewOptions = viewOptions
    }
    var _proto = GroupedDataMapProvider.prototype;
    _proto.getGroupStartDate = function(groupIndex) {
        var firstRow = this.getFirstGroupRow(groupIndex);
        if (firstRow) {
            var startDate = firstRow[0].cellData.startDate;
            return startDate
        }
    };
    _proto.getGroupEndDate = function(groupIndex) {
        var lastRow = this.getLastGroupRow(groupIndex);
        if (lastRow) {
            var lastColumnIndex = lastRow.length - 1;
            var cellData = lastRow[lastColumnIndex].cellData;
            var endDate = cellData.endDate;
            return endDate
        }
    };
    _proto.findGroupCellStartDate = function(groupIndex, startDate, endDate, isAllDay, isFindByDate) {
        if (isAllDay) {
            return this.findAllDayGroupCellStartDate(groupIndex, startDate)
        }
        var groupData = this.getGroupFromDateTableGroupMap(groupIndex);
        var checkCellStartDate = function(rowIndex, columnIndex) {
            var cellData = groupData[rowIndex][columnIndex].cellData;
            var secondMin = cellData.startDate,
                secondMax = cellData.endDate;
            if (isFindByDate) {
                secondMin = _date.default.trimTime(secondMin);
                secondMax = _date.default.setToDayEnd(secondMin)
            }
            if (_date.default.intervalsOverlap({
                    firstMin: startDate,
                    firstMax: endDate,
                    secondMin: secondMin,
                    secondMax: secondMax
                })) {
                return secondMin
            }
        };
        var startDateVerticalSearch = function() {
            var cellCount = groupData[0].length;
            for (var columnIndex = 0; columnIndex < cellCount; ++columnIndex) {
                for (var rowIndex = 0; rowIndex < groupData.length; ++rowIndex) {
                    var result = checkCellStartDate(rowIndex, columnIndex);
                    if (result) {
                        return result
                    }
                }
            }
        }();
        var startDateHorizontalSearch = function() {
            for (var rowIndex = 0; rowIndex < groupData.length; ++rowIndex) {
                var row = groupData[rowIndex];
                for (var columnIndex = 0; columnIndex < row.length; ++columnIndex) {
                    var result = checkCellStartDate(rowIndex, columnIndex);
                    if (result) {
                        return result
                    }
                }
            }
        }();
        return startDateVerticalSearch > startDateHorizontalSearch ? startDateHorizontalSearch : startDateVerticalSearch
    };
    _proto.findAllDayGroupCellStartDate = function(groupIndex, startDate) {
        var groupStartDate = this.getGroupStartDate(groupIndex);
        return groupStartDate > startDate ? groupStartDate : startDate
    };
    _proto.findCellPositionInMap = function(cellInfo) {
        var _this = this;
        var groupIndex = cellInfo.groupIndex,
            startDate = cellInfo.startDate,
            isAllDay = cellInfo.isAllDay,
            index = cellInfo.index;
        var startTime = isAllDay ? _date.default.trimTime(startDate).getTime() : startDate.getTime();
        var isStartDateInCell = function(cellData) {
            if (!(0, _base.isDateAndTimeView)(_this._viewOptions.viewType)) {
                return _date.default.sameDate(startDate, cellData.startDate)
            }
            var cellStartTime = cellData.startDate.getTime();
            var cellEndTime = cellData.endDate.getTime();
            return isAllDay ? cellData.allDay && startTime >= cellStartTime && startTime <= cellEndTime : startTime >= cellStartTime && startTime < cellEndTime
        };
        var _this$groupedDataMap = this.groupedDataMap,
            allDayPanelGroupedMap = _this$groupedDataMap.allDayPanelGroupedMap,
            dateTableGroupedMap = _this$groupedDataMap.dateTableGroupedMap;
        var rows = isAllDay && !this._viewOptions.isVerticalGrouping ? [allDayPanelGroupedMap[groupIndex]] || [] : dateTableGroupedMap[groupIndex] || [];
        for (var rowIndex = 0; rowIndex < rows.length; ++rowIndex) {
            var row = rows[rowIndex];
            for (var columnIndex = 0; columnIndex < row.length; ++columnIndex) {
                var cell = row[columnIndex];
                var cellData = cell.cellData;
                if (this._isSameGroupIndexAndIndex(cellData, groupIndex, index)) {
                    if (isStartDateInCell(cellData)) {
                        return cell.position
                    }
                }
            }
        }
        return
    };
    _proto._isSameGroupIndexAndIndex = function(cellData, groupIndex, index) {
        return cellData.groupIndex === groupIndex && (void 0 === index || cellData.index === index)
    };
    _proto.getCellsGroup = function(groupIndex) {
        var dateTableGroupedMap = this.groupedDataMap.dateTableGroupedMap;
        var groupData = dateTableGroupedMap[groupIndex];
        if (groupData) {
            var cellData = groupData[0][0].cellData;
            return cellData.groups
        }
    };
    _proto.getCompletedGroupsInfo = function() {
        var _this2 = this;
        var dateTableGroupedMap = this.groupedDataMap.dateTableGroupedMap;
        return dateTableGroupedMap.map((function(groupData) {
            var firstCell = groupData[0][0];
            var _firstCell$cellData = firstCell.cellData,
                allDay = _firstCell$cellData.allDay,
                groupIndex = _firstCell$cellData.groupIndex;
            return {
                allDay: allDay,
                groupIndex: groupIndex,
                startDate: _this2.getGroupStartDate(groupIndex),
                endDate: _this2.getGroupEndDate(groupIndex)
            }
        })).filter((function(_ref) {
            var startDate = _ref.startDate;
            return !!startDate
        }))
    };
    _proto.getGroupIndices = function() {
        return this.getCompletedGroupsInfo().map((function(_ref2) {
            var groupIndex = _ref2.groupIndex;
            return groupIndex
        }))
    };
    _proto.getGroupFromDateTableGroupMap = function(groupIndex) {
        var dateTableGroupedMap = this.groupedDataMap.dateTableGroupedMap;
        return dateTableGroupedMap[groupIndex]
    };
    _proto.getFirstGroupRow = function(groupIndex) {
        var groupedData = this.getGroupFromDateTableGroupMap(groupIndex);
        if (groupedData) {
            var cellData = groupedData[0][0].cellData;
            return !cellData.allDay ? groupedData[0] : groupedData[1]
        }
    };
    _proto.getLastGroupRow = function(groupIndex) {
        var dateTableGroupedMap = this.groupedDataMap.dateTableGroupedMap;
        var groupedData = dateTableGroupedMap[groupIndex];
        if (groupedData) {
            var lastRowIndex = groupedData.length - 1;
            return groupedData[lastRowIndex]
        }
    };
    _proto.getLastGroupCellPosition = function(groupIndex) {
        var groupRow = this.getLastGroupRow(groupIndex);
        return null === groupRow || void 0 === groupRow ? void 0 : groupRow[(null === groupRow || void 0 === groupRow ? void 0 : groupRow.length) - 1].position
    };
    _proto.getRowCountInGroup = function(groupIndex) {
        var groupRow = this.getLastGroupRow(groupIndex);
        var cellAmount = groupRow.length;
        var lastCellData = groupRow[cellAmount - 1].cellData;
        var lastCellIndex = lastCellData.index;
        return (lastCellIndex + 1) / groupRow.length
    };
    return GroupedDataMapProvider
}();
exports.GroupedDataMapProvider = GroupedDataMapProvider;
