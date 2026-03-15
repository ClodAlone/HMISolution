/**
 * DevExtreme (esm/ui/scheduler/workspaces/view_model/view_data_provider.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import _extends from "@babel/runtime/helpers/esm/extends";
import _objectWithoutPropertiesLoose from "@babel/runtime/helpers/esm/objectWithoutPropertiesLoose";
var _excluded = ["groups", "groupOrientation", "groupByDate", "isAllDayPanelVisible"];
import dateUtils from "../../../../core/utils/date";
import {
    getGroupPanelData
} from "../../../../renovation/ui/scheduler/view_model/group_panel/utils";
import {
    isGroupingByDate,
    isHorizontalGroupingApplied,
    isVerticalGroupingApplied
} from "../../../../renovation/ui/scheduler/workspaces/utils";
import {
    calculateIsGroupedAllDayPanel
} from "../../../../renovation/ui/scheduler/view_model/to_test/views/utils/base";
import {
    DateHeaderDataGenerator
} from "./date_header_data_generator";
import {
    GroupedDataMapProvider
} from "./grouped_data_map_provider";
import {
    TimePanelDataGenerator
} from "./time_panel_data_generator";
import {
    getViewDataGeneratorByViewType
} from "./utils";
import timeZoneUtils from "../../utils.timeZone";
export default class ViewDataProvider {
    constructor(viewType) {
        this.viewDataGenerator = getViewDataGeneratorByViewType(viewType);
        this.viewData = {};
        this.completeViewDataMap = [];
        this.completeDateHeaderMap = [];
        this.viewDataMap = {};
        this._groupedDataMapProvider = null
    }
    get groupedDataMap() {
        return this._groupedDataMapProvider.groupedDataMap
    }
    get hiddenInterval() {
        return this.viewDataGenerator.hiddenInterval
    }
    isSkippedDate(date) {
        return this.viewDataGenerator.isSkippedDate(date)
    }
    update(options, isGenerateNewViewData) {
        this.viewDataGenerator = getViewDataGeneratorByViewType(options.viewType);
        var viewDataGenerator = this.viewDataGenerator;
        var dateHeaderDataGenerator = new DateHeaderDataGenerator(viewDataGenerator);
        var timePanelDataGenerator = new TimePanelDataGenerator(viewDataGenerator);
        var renderOptions = this._transformRenderOptions(options);
        renderOptions.interval = this.viewDataGenerator.getInterval(renderOptions.hoursInterval);
        this._options = renderOptions;
        if (isGenerateNewViewData) {
            this.completeViewDataMap = viewDataGenerator.getCompleteViewDataMap(renderOptions);
            this.completeDateHeaderMap = dateHeaderDataGenerator.getCompleteDateHeaderMap(renderOptions, this.completeViewDataMap);
            if (renderOptions.isGenerateTimePanelData) {
                this.completeTimePanelMap = timePanelDataGenerator.getCompleteTimePanelMap(renderOptions, this.completeViewDataMap)
            }
        }
        this.viewDataMap = viewDataGenerator.generateViewDataMap(this.completeViewDataMap, renderOptions);
        this.updateViewData(renderOptions);
        this._groupedDataMapProvider = new GroupedDataMapProvider(this.viewDataGenerator, this.viewDataMap, this.completeViewDataMap, {
            isVerticalGrouping: renderOptions.isVerticalGrouping,
            viewType: renderOptions.viewType
        });
        this.dateHeaderData = dateHeaderDataGenerator.generateDateHeaderData(this.completeDateHeaderMap, this.completeViewDataMap, renderOptions);
        if (renderOptions.isGenerateTimePanelData) {
            this.timePanelData = timePanelDataGenerator.generateTimePanelData(this.completeTimePanelMap, renderOptions)
        }
    }
    createGroupedDataMapProvider() {
        this._groupedDataMapProvider = new GroupedDataMapProvider(this.viewDataGenerator, this.viewDataMap, this.completeViewDataMap, {
            isVerticalGrouping: this._options.isVerticalGrouping,
            viewType: this._options.viewType
        })
    }
    updateViewData(options) {
        var renderOptions = this._transformRenderOptions(options);
        this.viewDataMapWithSelection = this.viewDataGenerator.markSelectedAndFocusedCells(this.viewDataMap, renderOptions);
        this.viewData = this.viewDataGenerator.getViewDataFromMap(this.completeViewDataMap, this.viewDataMapWithSelection, renderOptions)
    }
    _transformRenderOptions(renderOptions) {
        var {
            groups: groups,
            groupOrientation: groupOrientation,
            groupByDate: groupByDate,
            isAllDayPanelVisible: isAllDayPanelVisible
        } = renderOptions, restOptions = _objectWithoutPropertiesLoose(renderOptions, _excluded);
        return _extends({}, restOptions, {
            startViewDate: this.viewDataGenerator._calculateStartViewDate(renderOptions),
            isVerticalGrouping: isVerticalGroupingApplied(groups, groupOrientation),
            isHorizontalGrouping: isHorizontalGroupingApplied(groups, groupOrientation),
            isGroupedByDate: isGroupingByDate(groups, groupOrientation, groupByDate),
            isGroupedAllDayPanel: calculateIsGroupedAllDayPanel(groups, groupOrientation, isAllDayPanelVisible),
            groups: groups,
            groupOrientation: groupOrientation,
            isAllDayPanelVisible: isAllDayPanelVisible
        })
    }
    getGroupPanelData(options) {
        var renderOptions = this._transformRenderOptions(options);
        if (renderOptions.groups.length > 0) {
            var cellCount = this.getCellCount(renderOptions);
            return getGroupPanelData(renderOptions.groups, cellCount, renderOptions.isGroupedByDate, renderOptions.isGroupedByDate ? 1 : cellCount)
        }
        return
    }
    getGroupStartDate(groupIndex) {
        return this._groupedDataMapProvider.getGroupStartDate(groupIndex)
    }
    getGroupEndDate(groupIndex) {
        return this._groupedDataMapProvider.getGroupEndDate(groupIndex)
    }
    findGroupCellStartDate(groupIndex, startDate, endDate, isAllDay, isFindByDate) {
        return this._groupedDataMapProvider.findGroupCellStartDate(groupIndex, startDate, endDate, isAllDay, isFindByDate)
    }
    findAllDayGroupCellStartDate(groupIndex, startDate) {
        return this._groupedDataMapProvider.findAllDayGroupCellStartDate(groupIndex, startDate)
    }
    findCellPositionInMap(cellInfo) {
        return this._groupedDataMapProvider.findCellPositionInMap(cellInfo)
    }
    hasAllDayPanel() {
        var {
            viewData: viewData
        } = this.viewDataMap;
        var {
            allDayPanel: allDayPanel
        } = viewData.groupedData[0];
        return !viewData.isGroupedAllDayPanel && (null === allDayPanel || void 0 === allDayPanel ? void 0 : allDayPanel.length) > 0
    }
    getCellsGroup(groupIndex) {
        return this._groupedDataMapProvider.getCellsGroup(groupIndex)
    }
    getCompletedGroupsInfo() {
        return this._groupedDataMapProvider.getCompletedGroupsInfo()
    }
    getGroupIndices() {
        return this._groupedDataMapProvider.getGroupIndices()
    }
    getLastGroupCellPosition(groupIndex) {
        return this._groupedDataMapProvider.getLastGroupCellPosition(groupIndex)
    }
    getRowCountInGroup(groupIndex) {
        return this._groupedDataMapProvider.getRowCountInGroup(groupIndex)
    }
    getCellData(rowIndex, columnIndex, isAllDay, rtlEnabled) {
        var row = isAllDay && !this._options.isVerticalGrouping ? this.viewDataMap.allDayPanelMap : this.viewDataMap.dateTableMap[rowIndex];
        var actualColumnIndex = !rtlEnabled ? columnIndex : row.length - 1 - columnIndex;
        var {
            cellData: cellData
        } = row[actualColumnIndex];
        return cellData
    }
    getCellsByGroupIndexAndAllDay(groupIndex, allDay) {
        var rowsPerGroup = this._getRowCountWithAllDayRows();
        var isShowAllDayPanel = this._options.isAllDayPanelVisible;
        var firstRowInGroup = this._options.isVerticalGrouping ? groupIndex * rowsPerGroup : 0;
        var lastRowInGroup = this._options.isVerticalGrouping ? (groupIndex + 1) * rowsPerGroup - 1 : rowsPerGroup;
        var correctedFirstRow = isShowAllDayPanel && !allDay ? firstRowInGroup + 1 : firstRowInGroup;
        var correctedLastRow = allDay ? correctedFirstRow : lastRowInGroup;
        return this.completeViewDataMap.slice(correctedFirstRow, correctedLastRow + 1).map(row => row.filter(_ref => {
            var {
                groupIndex: currentGroupIndex
            } = _ref;
            return groupIndex === currentGroupIndex
        }))
    }
    getGroupData(groupIndex) {
        var {
            groupedData: groupedData
        } = this.viewData;
        if (this._options.isVerticalGrouping) {
            return groupedData.filter(item => item.groupIndex === groupIndex)[0]
        }
        var filterCells = row => null === row || void 0 === row ? void 0 : row.filter(cell => cell.groupIndex === groupIndex);
        var {
            allDayPanel: allDayPanel,
            dateTable: dateTable
        } = groupedData[0];
        var filteredDateTable = [];
        dateTable.forEach(row => {
            filteredDateTable.push(filterCells(row))
        });
        return {
            allDayPanel: filterCells(allDayPanel),
            dateTable: filteredDateTable
        }
    }
    getCellCountWithGroup(groupIndex) {
        var rowIndex = arguments.length > 1 && void 0 !== arguments[1] ? arguments[1] : 0;
        var {
            dateTableGroupedMap: dateTableGroupedMap
        } = this.groupedDataMap;
        return dateTableGroupedMap.filter((_, index) => index <= groupIndex).reduce((previous, row) => previous + row[rowIndex].length, 0)
    }
    getAllDayPanel(groupIndex) {
        var groupData = this.getGroupData(groupIndex);
        return null === groupData || void 0 === groupData ? void 0 : groupData.allDayPanel
    }
    isGroupIntersectDateInterval(groupIndex, startDate, endDate) {
        var groupStartDate = this.getGroupStartDate(groupIndex);
        var groupEndDate = this.getGroupEndDate(groupIndex);
        return startDate < groupEndDate && endDate > groupStartDate
    }
    findGlobalCellPosition(date) {
        var groupIndex = arguments.length > 1 && void 0 !== arguments[1] ? arguments[1] : 0;
        var allDay = arguments.length > 2 && void 0 !== arguments[2] ? arguments[2] : false;
        var {
            completeViewDataMap: completeViewDataMap
        } = this;
        var showAllDayPanel = this._options.isAllDayPanelVisible;
        for (var rowIndex = 0; rowIndex < completeViewDataMap.length; rowIndex += 1) {
            var currentRow = completeViewDataMap[rowIndex];
            for (var columnIndex = 0; columnIndex < currentRow.length; columnIndex += 1) {
                var cellData = currentRow[columnIndex];
                var {
                    startDate: currentStartDate,
                    endDate: currentEndDate,
                    groupIndex: currentGroupIndex,
                    allDay: currentAllDay
                } = cellData;
                if (groupIndex === currentGroupIndex && allDay === !!currentAllDay && this._compareDatesAndAllDay(date, currentStartDate, currentEndDate, allDay)) {
                    return {
                        position: {
                            columnIndex: columnIndex,
                            rowIndex: showAllDayPanel && !this._options.isVerticalGrouping ? rowIndex - 1 : rowIndex
                        },
                        cellData: cellData
                    }
                }
            }
        }
    }
    _compareDatesAndAllDay(date, cellStartDate, cellEndDate, allDay) {
        var time = date.getTime();
        var trimmedTime = dateUtils.trimTime(date).getTime();
        var cellStartTime = cellStartDate.getTime();
        var cellEndTime = cellEndDate.getTime();
        return !allDay && time >= cellStartTime && time < cellEndTime || allDay && trimmedTime === cellStartTime
    }
    getSkippedDaysCount(groupIndex, startDate, endDate, daysCount) {
        var {
            dateTableGroupedMap: dateTableGroupedMap
        } = this._groupedDataMapProvider.groupedDataMap;
        var groupedData = dateTableGroupedMap[groupIndex];
        var includedDays = 0;
        for (var rowIndex = 0; rowIndex < groupedData.length; rowIndex += 1) {
            for (var columnIndex = 0; columnIndex < groupedData[rowIndex].length; columnIndex += 1) {
                var cell = groupedData[rowIndex][columnIndex].cellData;
                if (startDate.getTime() < cell.endDate.getTime() && endDate.getTime() > cell.startDate.getTime()) {
                    includedDays += 1
                }
            }
        }
        var lastCell = groupedData[groupedData.length - 1][groupedData[0].length - 1].cellData;
        var lastCellStart = dateUtils.trimTime(lastCell.startDate);
        var daysAfterView = Math.floor((endDate.getTime() - lastCellStart.getTime()) / dateUtils.dateToMilliseconds("day"));
        var deltaDays = daysAfterView > 0 ? daysAfterView : 0;
        return daysCount - includedDays - deltaDays
    }
    getColumnsCount() {
        var {
            dateTableMap: dateTableMap
        } = this.viewDataMap;
        return dateTableMap ? dateTableMap[0].length : 0
    }
    getViewEdgeIndices(isAllDayPanel) {
        if (isAllDayPanel) {
            return {
                firstColumnIndex: 0,
                lastColumnIndex: this.viewDataMap.allDayPanelMap.length - 1,
                firstRowIndex: 0,
                lastRowIndex: 0
            }
        }
        return {
            firstColumnIndex: 0,
            lastColumnIndex: this.viewDataMap.dateTableMap[0].length - 1,
            firstRowIndex: 0,
            lastRowIndex: this.viewDataMap.dateTableMap.length - 1
        }
    }
    getGroupEdgeIndices(groupIndex, isAllDay) {
        var groupedDataMap = this.groupedDataMap.dateTableGroupedMap[groupIndex];
        var cellsCount = groupedDataMap[0].length;
        var rowsCount = groupedDataMap.length;
        var firstColumnIndex = groupedDataMap[0][0].position.columnIndex;
        var lastColumnIndex = groupedDataMap[0][cellsCount - 1].position.columnIndex;
        if (isAllDay) {
            return {
                firstColumnIndex: firstColumnIndex,
                lastColumnIndex: lastColumnIndex,
                firstRowIndex: 0,
                lastRowIndex: 0
            }
        }
        return {
            firstColumnIndex: firstColumnIndex,
            lastColumnIndex: lastColumnIndex,
            firstRowIndex: groupedDataMap[0][0].position.rowIndex,
            lastRowIndex: groupedDataMap[rowsCount - 1][0].position.rowIndex
        }
    }
    isSameCell(firstCellData, secondCellData) {
        var {
            startDate: firstStartDate,
            groupIndex: firstGroupIndex,
            allDay: firstAllDay,
            index: firstIndex
        } = firstCellData;
        var {
            startDate: secondStartDate,
            groupIndex: secondGroupIndex,
            allDay: secondAllDay,
            index: secondIndex
        } = secondCellData;
        return firstStartDate.getTime() === secondStartDate.getTime() && firstGroupIndex === secondGroupIndex && firstAllDay === secondAllDay && firstIndex === secondIndex
    }
    getLastViewDate() {
        var completeViewDataMap = this.completeViewDataMap;
        var rowsCount = completeViewDataMap.length - 1;
        return completeViewDataMap[rowsCount][completeViewDataMap[rowsCount].length - 1].endDate
    }
    getStartViewDate() {
        return this._options.startViewDate
    }
    getIntervalDuration(intervalCount) {
        return this.viewDataGenerator._getIntervalDuration(intervalCount)
    }
    getLastCellEndDate() {
        return new Date(this.getLastViewDate().getTime() - dateUtils.dateToMilliseconds("minute"))
    }
    getLastViewDateByEndDayHour(endDayHour) {
        var lastCellEndDate = this.getLastCellEndDate();
        var endTime = dateUtils.dateTimeFromDecimal(endDayHour);
        var endDateOfLastViewCell = new Date(lastCellEndDate.setHours(endTime.hours, endTime.minutes));
        return this._adjustEndDateByDaylightDiff(lastCellEndDate, endDateOfLastViewCell)
    }
    _adjustEndDateByDaylightDiff(startDate, endDate) {
        var daylightDiff = timeZoneUtils.getDaylightOffsetInMs(startDate, endDate);
        var endDateOfLastViewCell = new Date(endDate.getTime() - daylightDiff);
        return new Date(endDateOfLastViewCell.getTime() - dateUtils.dateToMilliseconds("minute"))
    }
    getCellCountInDay(startDayHour, endDayHour, hoursInterval) {
        return this.viewDataGenerator.getCellCountInDay(startDayHour, endDayHour, hoursInterval)
    }
    getCellCount(options) {
        return this.viewDataGenerator.getCellCount(options)
    }
    getRowCount(options) {
        return this.viewDataGenerator.getRowCount(options)
    }
    getVisibleDayDuration(startDayHour, endDayHour, hoursInterval) {
        return this.viewDataGenerator.getVisibleDayDuration(startDayHour, endDayHour, hoursInterval)
    }
    _getRowCountWithAllDayRows() {
        var allDayRowCount = this._options.isAllDayPanelVisible ? 1 : 0;
        return this.getRowCount(this._options) + allDayRowCount
    }
    getFirstDayOfWeek(firstDayOfWeekOption) {
        return this.viewDataGenerator.getFirstDayOfWeek(firstDayOfWeekOption)
    }
    setViewOptions(options) {
        this._options = this._transformRenderOptions(options)
    }
    getViewOptions() {
        return this._options
    }
    getViewPortGroupCount() {
        var {
            dateTableGroupedMap: dateTableGroupedMap
        } = this.groupedDataMap;
        return (null === dateTableGroupedMap || void 0 === dateTableGroupedMap ? void 0 : dateTableGroupedMap.length) || 0
    }
}
