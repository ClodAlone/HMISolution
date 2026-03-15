/**
 * DevExtreme (esm/ui/scheduler/workspaces/view_model/grouped_data_map_provider.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import dateUtils from "../../../../core/utils/date";
import {
    isDateAndTimeView
} from "../../../../renovation/ui/scheduler/view_model/to_test/views/utils/base";
export class GroupedDataMapProvider {
    constructor(viewDataGenerator, viewDataMap, completeViewDataMap, viewOptions) {
        this.groupedDataMap = viewDataGenerator.generateGroupedDataMap(viewDataMap);
        this.completeViewDataMap = completeViewDataMap;
        this._viewOptions = viewOptions
    }
    getGroupStartDate(groupIndex) {
        var firstRow = this.getFirstGroupRow(groupIndex);
        if (firstRow) {
            var {
                startDate: startDate
            } = firstRow[0].cellData;
            return startDate
        }
    }
    getGroupEndDate(groupIndex) {
        var lastRow = this.getLastGroupRow(groupIndex);
        if (lastRow) {
            var lastColumnIndex = lastRow.length - 1;
            var {
                cellData: cellData
            } = lastRow[lastColumnIndex];
            var {
                endDate: endDate
            } = cellData;
            return endDate
        }
    }
    findGroupCellStartDate(groupIndex, startDate, endDate, isAllDay, isFindByDate) {
        if (isAllDay) {
            return this.findAllDayGroupCellStartDate(groupIndex, startDate)
        }
        var groupData = this.getGroupFromDateTableGroupMap(groupIndex);
        var checkCellStartDate = (rowIndex, columnIndex) => {
            var {
                cellData: cellData
            } = groupData[rowIndex][columnIndex];
            var {
                startDate: secondMin,
                endDate: secondMax
            } = cellData;
            if (isFindByDate) {
                secondMin = dateUtils.trimTime(secondMin);
                secondMax = dateUtils.setToDayEnd(secondMin)
            }
            if (dateUtils.intervalsOverlap({
                    firstMin: startDate,
                    firstMax: endDate,
                    secondMin: secondMin,
                    secondMax: secondMax
                })) {
                return secondMin
            }
        };
        var startDateVerticalSearch = (() => {
            var cellCount = groupData[0].length;
            for (var columnIndex = 0; columnIndex < cellCount; ++columnIndex) {
                for (var rowIndex = 0; rowIndex < groupData.length; ++rowIndex) {
                    var result = checkCellStartDate(rowIndex, columnIndex);
                    if (result) {
                        return result
                    }
                }
            }
        })();
        var startDateHorizontalSearch = (() => {
            for (var rowIndex = 0; rowIndex < groupData.length; ++rowIndex) {
                var row = groupData[rowIndex];
                for (var columnIndex = 0; columnIndex < row.length; ++columnIndex) {
                    var result = checkCellStartDate(rowIndex, columnIndex);
                    if (result) {
                        return result
                    }
                }
            }
        })();
        return startDateVerticalSearch > startDateHorizontalSearch ? startDateHorizontalSearch : startDateVerticalSearch
    }
    findAllDayGroupCellStartDate(groupIndex, startDate) {
        var groupStartDate = this.getGroupStartDate(groupIndex);
        return groupStartDate > startDate ? groupStartDate : startDate
    }
    findCellPositionInMap(cellInfo) {
        var {
            groupIndex: groupIndex,
            startDate: startDate,
            isAllDay: isAllDay,
            index: index
        } = cellInfo;
        var startTime = isAllDay ? dateUtils.trimTime(startDate).getTime() : startDate.getTime();
        var isStartDateInCell = cellData => {
            if (!isDateAndTimeView(this._viewOptions.viewType)) {
                return dateUtils.sameDate(startDate, cellData.startDate)
            }
            var cellStartTime = cellData.startDate.getTime();
            var cellEndTime = cellData.endDate.getTime();
            return isAllDay ? cellData.allDay && startTime >= cellStartTime && startTime <= cellEndTime : startTime >= cellStartTime && startTime < cellEndTime
        };
        var {
            allDayPanelGroupedMap: allDayPanelGroupedMap,
            dateTableGroupedMap: dateTableGroupedMap
        } = this.groupedDataMap;
        var rows = isAllDay && !this._viewOptions.isVerticalGrouping ? [allDayPanelGroupedMap[groupIndex]] || [] : dateTableGroupedMap[groupIndex] || [];
        for (var rowIndex = 0; rowIndex < rows.length; ++rowIndex) {
            var row = rows[rowIndex];
            for (var columnIndex = 0; columnIndex < row.length; ++columnIndex) {
                var cell = row[columnIndex];
                var {
                    cellData: cellData
                } = cell;
                if (this._isSameGroupIndexAndIndex(cellData, groupIndex, index)) {
                    if (isStartDateInCell(cellData)) {
                        return cell.position
                    }
                }
            }
        }
        return
    }
    _isSameGroupIndexAndIndex(cellData, groupIndex, index) {
        return cellData.groupIndex === groupIndex && (void 0 === index || cellData.index === index)
    }
    getCellsGroup(groupIndex) {
        var {
            dateTableGroupedMap: dateTableGroupedMap
        } = this.groupedDataMap;
        var groupData = dateTableGroupedMap[groupIndex];
        if (groupData) {
            var {
                cellData: cellData
            } = groupData[0][0];
            return cellData.groups
        }
    }
    getCompletedGroupsInfo() {
        var {
            dateTableGroupedMap: dateTableGroupedMap
        } = this.groupedDataMap;
        return dateTableGroupedMap.map(groupData => {
            var firstCell = groupData[0][0];
            var {
                allDay: allDay,
                groupIndex: groupIndex
            } = firstCell.cellData;
            return {
                allDay: allDay,
                groupIndex: groupIndex,
                startDate: this.getGroupStartDate(groupIndex),
                endDate: this.getGroupEndDate(groupIndex)
            }
        }).filter(_ref => {
            var {
                startDate: startDate
            } = _ref;
            return !!startDate
        })
    }
    getGroupIndices() {
        return this.getCompletedGroupsInfo().map(_ref2 => {
            var {
                groupIndex: groupIndex
            } = _ref2;
            return groupIndex
        })
    }
    getGroupFromDateTableGroupMap(groupIndex) {
        var {
            dateTableGroupedMap: dateTableGroupedMap
        } = this.groupedDataMap;
        return dateTableGroupedMap[groupIndex]
    }
    getFirstGroupRow(groupIndex) {
        var groupedData = this.getGroupFromDateTableGroupMap(groupIndex);
        if (groupedData) {
            var {
                cellData: cellData
            } = groupedData[0][0];
            return !cellData.allDay ? groupedData[0] : groupedData[1]
        }
    }
    getLastGroupRow(groupIndex) {
        var {
            dateTableGroupedMap: dateTableGroupedMap
        } = this.groupedDataMap;
        var groupedData = dateTableGroupedMap[groupIndex];
        if (groupedData) {
            var lastRowIndex = groupedData.length - 1;
            return groupedData[lastRowIndex]
        }
    }
    getLastGroupCellPosition(groupIndex) {
        var groupRow = this.getLastGroupRow(groupIndex);
        return null === groupRow || void 0 === groupRow ? void 0 : groupRow[(null === groupRow || void 0 === groupRow ? void 0 : groupRow.length) - 1].position
    }
    getRowCountInGroup(groupIndex) {
        var groupRow = this.getLastGroupRow(groupIndex);
        var cellAmount = groupRow.length;
        var lastCellData = groupRow[cellAmount - 1].cellData;
        var lastCellIndex = lastCellData.index;
        return (lastCellIndex + 1) / groupRow.length
    }
}
