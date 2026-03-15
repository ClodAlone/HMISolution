/**
 * DevExtreme (esm/ui/scheduler/workspaces/view_model/view_data_generator.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import _extends from "@babel/runtime/helpers/esm/extends";
import dateUtils from "../../../../core/utils/date";
import {
    HORIZONTAL_GROUP_ORIENTATION
} from "../../constants";
import {
    getAllGroups,
    getGroupCount
} from "../../resources/utils";
import {
    calculateCellIndex,
    calculateDayDuration,
    isHorizontalView,
    getStartViewDateWithoutDST,
    getDisplayedRowCount,
    getTotalCellCountByCompleteData,
    getTotalRowCountByCompleteData,
    getDisplayedCellCount
} from "../../../../renovation/ui/scheduler/view_model/to_test/views/utils/base";
var HOUR_MS = dateUtils.dateToMilliseconds("hour");
var DAY_MS = dateUtils.dateToMilliseconds("day");
export class ViewDataGenerator {
    get daysInInterval() {
        return 1
    }
    get isWorkView() {
        return false
    }
    get tableAllDay() {
        return false
    }
    isSkippedDate() {
        return false
    }
    getStartViewDate(options) {
        return this._calculateStartViewDate(options)
    }
    getCompleteViewDataMap(options) {
        var {
            groups: groups,
            isGroupedByDate: isGroupedByDate,
            isHorizontalGrouping: isHorizontalGrouping,
            isVerticalGrouping: isVerticalGrouping,
            intervalCount: intervalCount,
            currentDate: currentDate,
            viewType: viewType,
            startDayHour: startDayHour,
            endDayHour: endDayHour,
            hoursInterval: hoursInterval
        } = options;
        this._setVisibilityDates(options);
        this.setHiddenInterval(startDayHour, endDayHour, hoursInterval);
        var groupsList = getAllGroups(groups);
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
        viewDataMap.push(...viewCellsData);
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
    }
    _transformViewDataMapForHorizontalGrouping(viewDataMap, groupsList) {
        var result = viewDataMap.map(row => row.slice());
        groupsList.slice(1).forEach((groups, index) => {
            var groupIndex = index + 1;
            viewDataMap.forEach((row, rowIndex) => {
                var nextGroupRow = row.map(cellData => _extends({}, cellData, {
                    groups: groups,
                    groupIndex: groupIndex
                }));
                result[rowIndex].push(...nextGroupRow)
            })
        });
        return result
    }
    _transformViewDataMapForVerticalGrouping(viewDataMap, groupsList) {
        var result = viewDataMap.map(row => row.slice());
        groupsList.slice(1).forEach((groups, index) => {
            var groupIndex = index + 1;
            var nextGroupMap = viewDataMap.map(cellsRow => {
                var nextRow = cellsRow.map(cellData => _extends({}, cellData, {
                    groupIndex: groupIndex,
                    groups: groups
                }));
                return nextRow
            });
            result.push(...nextGroupMap)
        });
        return result
    }
    _transformViewDataMapForGroupingByDate(viewDataMap, groupsList) {
        var correctedGroupList = groupsList.slice(1);
        var correctedGroupCount = correctedGroupList.length;
        var result = viewDataMap.map(cellsRow => {
            var groupedByDateCellsRow = cellsRow.reduce((currentRow, cell) => {
                var rowWithCurrentCell = [...currentRow, _extends({}, cell, {
                    isFirstGroupCell: true,
                    isLastGroupCell: 0 === correctedGroupCount
                }), ...correctedGroupList.map((groups, index) => _extends({}, cell, {
                    groups: groups,
                    groupIndex: index + 1,
                    isFirstGroupCell: false,
                    isLastGroupCell: index === correctedGroupCount - 1
                }))];
                return rowWithCurrentCell
            }, []);
            return groupedByDateCellsRow
        });
        return result
    }
    _addKeysToCells(viewDataMap) {
        var totalColumnCount = viewDataMap[0].length;
        var {
            currentViewDataMap: result
        } = viewDataMap.reduce((_ref, row, rowIndex) => {
            var {
                allDayPanelsCount: allDayPanelsCount,
                currentViewDataMap: currentViewDataMap
            } = _ref;
            var isAllDay = row[0].allDay;
            var keyBase = (rowIndex - allDayPanelsCount) * totalColumnCount;
            var currentAllDayPanelsCount = isAllDay ? allDayPanelsCount + 1 : allDayPanelsCount;
            currentViewDataMap[rowIndex].forEach((cell, columnIndex) => {
                cell.key = keyBase + columnIndex
            });
            return {
                allDayPanelsCount: currentAllDayPanelsCount,
                currentViewDataMap: currentViewDataMap
            }
        }, {
            allDayPanelsCount: 0,
            currentViewDataMap: viewDataMap
        });
        return result
    }
    generateViewDataMap(completeViewDataMap, options) {
        var {
            rowCount: rowCount,
            startCellIndex: startCellIndex,
            startRowIndex: startRowIndex,
            cellCount: cellCount,
            isVerticalGrouping: isVerticalGrouping,
            isAllDayPanelVisible: isAllDayPanelVisible
        } = options;
        var sliceCells = (row, rowIndex, startIndex, count) => {
            var sliceToIndex = void 0 !== count ? startIndex + count : void 0;
            return row.slice(startIndex, sliceToIndex).map((cellData, columnIndex) => ({
                cellData: cellData,
                position: {
                    rowIndex: rowIndex,
                    columnIndex: columnIndex
                }
            }))
        };
        var correctedStartRowIndex = startRowIndex;
        var allDayPanelMap = [];
        if (this._isStandaloneAllDayPanel(isVerticalGrouping, isAllDayPanelVisible)) {
            correctedStartRowIndex++;
            allDayPanelMap = sliceCells(completeViewDataMap[0], 0, startCellIndex, cellCount)
        }
        var displayedRowCount = getDisplayedRowCount(rowCount, completeViewDataMap);
        var dateTableMap = completeViewDataMap.slice(correctedStartRowIndex, correctedStartRowIndex + displayedRowCount).map((row, rowIndex) => sliceCells(row, rowIndex, startCellIndex, cellCount));
        return {
            allDayPanelMap: allDayPanelMap,
            dateTableMap: dateTableMap
        }
    }
    _isStandaloneAllDayPanel(isVerticalGrouping, isAllDayPanelVisible) {
        return !isVerticalGrouping && isAllDayPanelVisible
    }
    getViewDataFromMap(completeViewDataMap, viewDataMap, options) {
        var {
            topVirtualRowHeight: topVirtualRowHeight,
            bottomVirtualRowHeight: bottomVirtualRowHeight,
            leftVirtualCellWidth: leftVirtualCellWidth,
            rightVirtualCellWidth: rightVirtualCellWidth,
            cellCount: cellCount,
            rowCount: rowCount,
            startRowIndex: startRowIndex,
            startCellIndex: startCellIndex,
            isProvideVirtualCellsWidth: isProvideVirtualCellsWidth,
            isGroupedAllDayPanel: isGroupedAllDayPanel,
            isVerticalGrouping: isVerticalGrouping,
            isAllDayPanelVisible: isAllDayPanelVisible
        } = options;
        var {
            allDayPanelMap: allDayPanelMap,
            dateTableMap: dateTableMap
        } = viewDataMap;
        var {
            previousGroupedData: groupedData
        } = dateTableMap.reduce((_ref2, cellsRow) => {
            var {
                previousGroupIndex: previousGroupIndex,
                previousGroupedData: previousGroupedData
            } = _ref2;
            var cellDataRow = cellsRow.map(_ref3 => {
                var {
                    cellData: cellData
                } = _ref3;
                return cellData
            });
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
        }, {
            previousGroupIndex: -1,
            previousGroupedData: []
        });
        if (this._isStandaloneAllDayPanel(isVerticalGrouping, isAllDayPanelVisible)) {
            groupedData[0].allDayPanel = allDayPanelMap.map(_ref4 => {
                var {
                    cellData: cellData
                } = _ref4;
                return cellData
            })
        }
        var totalCellCount = getTotalCellCountByCompleteData(completeViewDataMap);
        var totalRowCount = getTotalRowCountByCompleteData(completeViewDataMap);
        var displayedCellCount = getDisplayedCellCount(cellCount, completeViewDataMap);
        var displayedRowCount = getDisplayedRowCount(rowCount, completeViewDataMap);
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
    }
    _generateViewCellsData(options, rowCount, cellCountInGroupRow) {
        var viewCellsData = [];
        for (var rowIndex = 0; rowIndex < rowCount; rowIndex += 1) {
            viewCellsData.push(this._generateCellsRow(options, false, rowIndex, rowCount, cellCountInGroupRow))
        }
        return viewCellsData
    }
    _generateAllDayPanelData(options, rowCount, columnCount) {
        if (!options.isAllDayPanelVisible) {
            return null
        }
        return this._generateCellsRow(options, true, 0, rowCount, columnCount)
    }
    _generateCellsRow(options, allDay, rowIndex, rowCount, columnCount) {
        var cellsRow = [];
        for (var columnIndex = 0; columnIndex < columnCount; ++columnIndex) {
            var cellDataValue = this.getCellData(rowIndex, columnIndex, options, allDay);
            cellDataValue.index = rowIndex * columnCount + columnIndex;
            cellDataValue.isFirstGroupCell = this._isFirstGroupCell(rowIndex, columnIndex, options, rowCount, columnCount);
            cellDataValue.isLastGroupCell = this._isLastGroupCell(rowIndex, columnIndex, options, rowCount, columnCount);
            cellsRow.push(cellDataValue)
        }
        return cellsRow
    }
    getCellData(rowIndex, columnIndex, options, allDay) {
        return allDay ? this.prepareAllDayCellData(options, rowIndex, columnIndex) : this.prepareCellData(options, rowIndex, columnIndex)
    }
    prepareCellData(options, rowIndex, columnIndex) {
        var {
            groups: groups,
            startDayHour: startDayHour,
            endDayHour: endDayHour,
            interval: interval,
            hoursInterval: hoursInterval
        } = options;
        var groupsList = getAllGroups(groups);
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
    }
    prepareAllDayCellData(options, rowIndex, columnIndex) {
        var data = this.prepareCellData(options, rowIndex, columnIndex);
        var startDate = dateUtils.trimTime(data.startDate);
        return _extends({}, data, {
            startDate: startDate,
            endDate: startDate,
            allDay: true
        })
    }
    getDateByCellIndices(options, rowIndex, columnIndex, cellCountInDay) {
        var startViewDate = options.startViewDate;
        var {
            startDayHour: startDayHour,
            interval: interval,
            firstDayOfWeek: firstDayOfWeek,
            intervalCount: intervalCount
        } = options;
        var isStartViewDateDuringDST = startViewDate.getHours() !== Math.floor(startDayHour);
        if (isStartViewDateDuringDST) {
            var dateWithCorrectHours = getStartViewDateWithoutDST(startViewDate, startDayHour);
            startViewDate = new Date(dateWithCorrectHours - dateUtils.dateToMilliseconds("day"))
        }
        var columnCountBase = this.getCellCount(options);
        var rowCountBase = this.getRowCount(options);
        var cellIndex = this._calculateCellIndex(rowIndex, columnIndex, rowCountBase, columnCountBase);
        var millisecondsOffset = this.getMillisecondsOffset(cellIndex, interval, cellCountInDay);
        var offsetByCount = this.isWorkView ? this.getTimeOffsetByColumnIndex(columnIndex, this.getFirstDayOfWeek(firstDayOfWeek), columnCountBase, intervalCount) : 0;
        var startViewDateTime = startViewDate.getTime();
        var currentDate = new Date(startViewDateTime + millisecondsOffset + offsetByCount);
        var timeZoneDifference = isStartViewDateDuringDST ? 0 : dateUtils.getTimezonesDifference(startViewDate, currentDate);
        currentDate.setTime(currentDate.getTime() + timeZoneDifference);
        return currentDate
    }
    getMillisecondsOffset(cellIndex, interval, cellCountInDay) {
        var dayIndex = Math.floor(cellIndex / cellCountInDay);
        var realHiddenInterval = dayIndex * this.hiddenInterval;
        return interval * cellIndex + realHiddenInterval
    }
    getTimeOffsetByColumnIndex(columnIndex, firstDayOfWeek, columnCount, intervalCount) {
        var firstDayOfWeekDiff = Math.max(0, firstDayOfWeek - 1);
        var columnsInWeek = columnCount / intervalCount;
        var weekendCount = Math.floor((columnIndex + firstDayOfWeekDiff) / columnsInWeek);
        return DAY_MS * weekendCount * 2
    }
    calculateEndDate(startDate, interval, endDayHour) {
        var result = new Date(startDate);
        result.setMilliseconds(result.getMilliseconds() + Math.round(interval));
        return result
    }
    _calculateCellIndex(rowIndex, columnIndex, rowCount, columnCount) {
        return calculateCellIndex(rowIndex, columnIndex, rowCount)
    }
    generateGroupedDataMap(viewDataMap) {
        var {
            allDayPanelMap: allDayPanelMap,
            dateTableMap: dateTableMap
        } = viewDataMap;
        var {
            previousGroupedDataMap: dateTableGroupedMap
        } = dateTableMap.reduce((previousOptions, cellsRow) => {
            var {
                previousGroupedDataMap: previousGroupedDataMap,
                previousRowIndex: previousRowIndex,
                previousGroupIndex: previousGroupIndex
            } = previousOptions;
            var {
                groupIndex: currentGroupIndex
            } = cellsRow[0].cellData;
            var currentRowIndex = currentGroupIndex === previousGroupIndex ? previousRowIndex + 1 : 0;
            cellsRow.forEach(cell => {
                var {
                    groupIndex: groupIndex
                } = cell.cellData;
                if (!previousGroupedDataMap[groupIndex]) {
                    previousGroupedDataMap[groupIndex] = []
                }
                if (!previousGroupedDataMap[groupIndex][currentRowIndex]) {
                    previousGroupedDataMap[groupIndex][currentRowIndex] = []
                }
                previousGroupedDataMap[groupIndex][currentRowIndex].push(cell)
            });
            return {
                previousGroupedDataMap: previousGroupedDataMap,
                previousRowIndex: currentRowIndex,
                previousGroupIndex: currentGroupIndex
            }
        }, {
            previousGroupedDataMap: [],
            previousRowIndex: -1,
            previousGroupIndex: -1
        });
        var allDayPanelGroupedMap = [];
        null === allDayPanelMap || void 0 === allDayPanelMap ? void 0 : allDayPanelMap.forEach(cell => {
            var {
                groupIndex: groupIndex
            } = cell.cellData;
            if (!allDayPanelGroupedMap[groupIndex]) {
                allDayPanelGroupedMap[groupIndex] = []
            }
            allDayPanelGroupedMap[groupIndex].push(cell)
        });
        return {
            allDayPanelGroupedMap: allDayPanelGroupedMap,
            dateTableGroupedMap: dateTableGroupedMap
        }
    }
    _isFirstGroupCell(rowIndex, columnIndex, options, rowCount, columnCount) {
        var {
            groupOrientation: groupOrientation,
            groups: groups,
            isGroupedByDate: isGroupedByDate
        } = options;
        var groupCount = getGroupCount(groups);
        if (isGroupedByDate) {
            return columnIndex % groupCount === 0
        }
        if (groupOrientation === HORIZONTAL_GROUP_ORIENTATION) {
            return columnIndex % columnCount === 0
        }
        return rowIndex % rowCount === 0
    }
    _isLastGroupCell(rowIndex, columnIndex, options, rowCount, columnCount) {
        var {
            groupOrientation: groupOrientation,
            groups: groups,
            isGroupedByDate: isGroupedByDate
        } = options;
        var groupCount = getGroupCount(groups);
        if (isGroupedByDate) {
            return (columnIndex + 1) % groupCount === 0
        }
        if (groupOrientation === HORIZONTAL_GROUP_ORIENTATION) {
            return (columnIndex + 1) % columnCount === 0
        }
        return (rowIndex + 1) % rowCount === 0
    }
    markSelectedAndFocusedCells(viewDataMap, renderOptions) {
        var {
            selectedCells: selectedCells,
            focusedCell: focusedCell
        } = renderOptions;
        if (!selectedCells && !focusedCell) {
            return viewDataMap
        }
        var {
            allDayPanelMap: allDayPanelMap,
            dateTableMap: dateTableMap
        } = viewDataMap;
        var nextDateTableMap = dateTableMap.map(row => this._markSelectedAndFocusedCellsInRow(row, selectedCells, focusedCell));
        var nextAllDayMap = this._markSelectedAndFocusedCellsInRow(allDayPanelMap, selectedCells, focusedCell);
        return {
            allDayPanelMap: nextAllDayMap,
            dateTableMap: nextDateTableMap
        }
    }
    _markSelectedAndFocusedCellsInRow(dataRow, selectedCells, focusedCell) {
        return dataRow.map(cell => {
            var {
                index: index,
                groupIndex: groupIndex,
                allDay: allDay,
                startDate: startDate
            } = cell.cellData;
            var indexInSelectedCells = selectedCells.findIndex(_ref5 => {
                var {
                    index: selectedCellIndex,
                    groupIndex: selectedCellGroupIndex,
                    allDay: selectedCellAllDay,
                    startDate: selectedCellStartDate
                } = _ref5;
                return groupIndex === selectedCellGroupIndex && (index === selectedCellIndex || void 0 === selectedCellIndex && startDate.getTime() === selectedCellStartDate.getTime()) && !!allDay === !!selectedCellAllDay
            });
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
        })
    }
    getInterval(hoursInterval) {
        if (void 0 === this._interval) {
            this._interval = hoursInterval * HOUR_MS
        }
        return this._interval
    }
    _getIntervalDuration(intervalCount) {
        return dateUtils.dateToMilliseconds("day") * intervalCount
    }
    _setVisibilityDates() {}
    getCellCountInDay(startDayHour, endDayHour, hoursInterval) {
        var result = calculateDayDuration(startDayHour, endDayHour) / hoursInterval;
        return Math.ceil(result)
    }
    getCellCount(options) {
        var {
            intervalCount: intervalCount,
            viewType: viewType,
            startDayHour: startDayHour,
            endDayHour: endDayHour,
            hoursInterval: hoursInterval
        } = options;
        var cellCountInDay = this.getCellCountInDay(startDayHour, endDayHour, hoursInterval);
        var columnCountInDay = isHorizontalView(viewType) ? cellCountInDay : 1;
        return this.daysInInterval * intervalCount * columnCountInDay
    }
    getRowCount(options) {
        var {
            viewType: viewType,
            startDayHour: startDayHour,
            endDayHour: endDayHour,
            hoursInterval: hoursInterval
        } = options;
        var cellCountInDay = this.getCellCountInDay(startDayHour, endDayHour, hoursInterval);
        var rowCountInDay = !isHorizontalView(viewType) ? cellCountInDay : 1;
        return rowCountInDay
    }
    setHiddenInterval(startDayHour, endDayHour, hoursInterval) {
        this.hiddenInterval = DAY_MS - this.getVisibleDayDuration(startDayHour, endDayHour, hoursInterval)
    }
    getVisibleDayDuration(startDayHour, endDayHour, hoursInterval) {
        var cellCountInDay = this.getCellCountInDay(startDayHour, endDayHour, hoursInterval);
        return hoursInterval * cellCountInDay * HOUR_MS
    }
    getFirstDayOfWeek(firstDayOfWeekOption) {
        return firstDayOfWeekOption
    }
}
