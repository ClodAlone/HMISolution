/**
 * DevExtreme (esm/ui/scheduler/workspaces/view_model/date_header_data_generator.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import _objectWithoutPropertiesLoose from "@babel/runtime/helpers/esm/objectWithoutPropertiesLoose";
import _extends from "@babel/runtime/helpers/esm/extends";
var _excluded = ["startDate", "endDate", "isFirstGroupCell", "isLastGroupCell"];
import dateUtils from "../../../../core/utils/date";
import {
    getGroupCount
} from "../../resources/utils";
import {
    getHeaderCellText,
    formatWeekdayAndDay,
    getHorizontalGroupCount,
    getTotalCellCountByCompleteData,
    getDisplayedCellCount
} from "../../../../renovation/ui/scheduler/view_model/to_test/views/utils/base";
export class DateHeaderDataGenerator {
    constructor(viewDataGenerator) {
        this._viewDataGenerator = viewDataGenerator
    }
    getCompleteDateHeaderMap(options, completeViewDataMap) {
        var {
            isGenerateWeekDaysHeaderData: isGenerateWeekDaysHeaderData
        } = options;
        var result = [];
        if (isGenerateWeekDaysHeaderData) {
            var weekDaysRow = this._generateWeekDaysHeaderRowMap(options, completeViewDataMap);
            result.push(weekDaysRow)
        }
        var dateRow = this._generateHeaderDateRow(options, completeViewDataMap);
        result.push(dateRow);
        return result
    }
    _generateWeekDaysHeaderRowMap(options, completeViewDataMap) {
        var {
            isGroupedByDate: isGroupedByDate,
            groups: groups,
            groupOrientation: groupOrientation,
            startDayHour: startDayHour,
            endDayHour: endDayHour,
            hoursInterval: hoursInterval,
            isHorizontalGrouping: isHorizontalGrouping,
            intervalCount: intervalCount
        } = options;
        var cellCountInDay = this._viewDataGenerator.getCellCountInDay(startDayHour, endDayHour, hoursInterval);
        var horizontalGroupCount = getHorizontalGroupCount(groups, groupOrientation);
        var index = completeViewDataMap[0][0].allDay ? 1 : 0;
        var colSpan = isGroupedByDate ? horizontalGroupCount * cellCountInDay : cellCountInDay;
        var groupCount = getGroupCount(groups);
        var datesRepeatCount = isHorizontalGrouping && !isGroupedByDate ? groupCount : 1;
        var daysInGroup = this._viewDataGenerator.daysInInterval * intervalCount;
        var daysInView = daysInGroup * datesRepeatCount;
        var weekDaysRow = [];
        for (var dayIndex = 0; dayIndex < daysInView; dayIndex += 1) {
            var cell = completeViewDataMap[index][dayIndex * colSpan];
            weekDaysRow.push(_extends({}, cell, {
                colSpan: colSpan,
                text: formatWeekdayAndDay(cell.startDate),
                isFirstGroupCell: false,
                isLastGroupCell: false
            }))
        }
        return weekDaysRow
    }
    _generateHeaderDateRow(options, completeViewDataMap) {
        var {
            today: today,
            isGroupedByDate: isGroupedByDate,
            groupOrientation: groupOrientation,
            groups: groups,
            headerCellTextFormat: headerCellTextFormat,
            getDateForHeaderText: getDateForHeaderText,
            interval: interval,
            startViewDate: startViewDate,
            startDayHour: startDayHour,
            endDayHour: endDayHour,
            hoursInterval: hoursInterval,
            intervalCount: intervalCount,
            currentDate: currentDate,
            viewType: viewType
        } = options;
        var horizontalGroupCount = getHorizontalGroupCount(groups, groupOrientation);
        var index = completeViewDataMap[0][0].allDay ? 1 : 0;
        var colSpan = isGroupedByDate ? horizontalGroupCount : 1;
        var isVerticalGrouping = "vertical" === groupOrientation;
        var cellCountInGroupRow = this._viewDataGenerator.getCellCount({
            intervalCount: intervalCount,
            currentDate: currentDate,
            viewType: viewType,
            hoursInterval: hoursInterval,
            startDayHour: startDayHour,
            endDayHour: endDayHour
        });
        var cellCountInDay = this._viewDataGenerator.getCellCountInDay(startDayHour, endDayHour, hoursInterval);
        var slicedByColumnsData = isGroupedByDate ? completeViewDataMap[index].filter((_, columnIndex) => columnIndex % horizontalGroupCount === 0) : completeViewDataMap[index];
        return slicedByColumnsData.map((_ref, index) => {
            var {
                startDate: startDate,
                isFirstGroupCell: isFirstGroupCell,
                isLastGroupCell: isLastGroupCell
            } = _ref, restProps = _objectWithoutPropertiesLoose(_ref, _excluded);
            var text = getHeaderCellText(index % cellCountInGroupRow, startDate, headerCellTextFormat, getDateForHeaderText, {
                interval: interval,
                startViewDate: startViewDate,
                startDayHour: startDayHour,
                cellCountInDay: cellCountInDay
            });
            return _extends({}, restProps, {
                startDate: startDate,
                text: text,
                today: dateUtils.sameDate(startDate, today),
                colSpan: colSpan,
                isFirstGroupCell: isGroupedByDate || isFirstGroupCell && !isVerticalGrouping,
                isLastGroupCell: isGroupedByDate || isLastGroupCell && !isVerticalGrouping
            })
        })
    }
    generateDateHeaderData(completeDateHeaderMap, completeViewDataMap, options) {
        var {
            isGenerateWeekDaysHeaderData: isGenerateWeekDaysHeaderData,
            cellWidth: cellWidth,
            isProvideVirtualCellsWidth: isProvideVirtualCellsWidth,
            startDayHour: startDayHour,
            endDayHour: endDayHour,
            hoursInterval: hoursInterval
        } = options;
        var dataMap = [];
        var weekDayRowConfig = {};
        var validCellWidth = cellWidth || 0;
        if (isGenerateWeekDaysHeaderData) {
            weekDayRowConfig = this._generateDateHeaderDataRow(options, completeDateHeaderMap, completeViewDataMap, this._viewDataGenerator.getCellCountInDay(startDayHour, endDayHour, hoursInterval), 0, validCellWidth);
            dataMap.push(weekDayRowConfig.dateRow)
        }
        var datesRowConfig = this._generateDateHeaderDataRow(options, completeDateHeaderMap, completeViewDataMap, 1, isGenerateWeekDaysHeaderData ? 1 : 0, validCellWidth);
        dataMap.push(datesRowConfig.dateRow);
        return {
            dataMap: dataMap,
            leftVirtualCellWidth: isProvideVirtualCellsWidth ? datesRowConfig.leftVirtualCellWidth : void 0,
            rightVirtualCellWidth: isProvideVirtualCellsWidth ? datesRowConfig.rightVirtualCellWidth : void 0,
            leftVirtualCellCount: datesRowConfig.leftVirtualCellCount,
            rightVirtualCellCount: datesRowConfig.rightVirtualCellCount,
            weekDayLeftVirtualCellWidth: weekDayRowConfig.leftVirtualCellWidth,
            weekDayRightVirtualCellWidth: weekDayRowConfig.rightVirtualCellWidth,
            weekDayLeftVirtualCellCount: weekDayRowConfig.leftVirtualCellCount,
            weekDayRightVirtualCellCount: weekDayRowConfig.rightVirtualCellCount
        }
    }
    _generateDateHeaderDataRow(options, completeDateHeaderMap, completeViewDataMap, baseColSpan, rowIndex, cellWidth) {
        var {
            startCellIndex: startCellIndex,
            cellCount: cellCount,
            isProvideVirtualCellsWidth: isProvideVirtualCellsWidth,
            groups: groups,
            groupOrientation: groupOrientation,
            isGroupedByDate: isGroupedByDate
        } = options;
        var horizontalGroupCount = getHorizontalGroupCount(groups, groupOrientation);
        var colSpan = isGroupedByDate ? horizontalGroupCount * baseColSpan : baseColSpan;
        var leftVirtualCellCount = Math.floor(startCellIndex / colSpan);
        var displayedCellCount = getDisplayedCellCount(cellCount, completeViewDataMap);
        var actualCellCount = Math.ceil((startCellIndex + displayedCellCount) / colSpan);
        var totalCellCount = getTotalCellCountByCompleteData(completeViewDataMap);
        var dateRow = completeDateHeaderMap[rowIndex].slice(leftVirtualCellCount, actualCellCount);
        var finalLeftVirtualCellCount = leftVirtualCellCount * colSpan;
        var finalLeftVirtualCellWidth = finalLeftVirtualCellCount * cellWidth;
        var finalRightVirtualCellCount = totalCellCount - actualCellCount * colSpan;
        var finalRightVirtualCellWidth = finalRightVirtualCellCount * cellWidth;
        return {
            dateRow: dateRow,
            leftVirtualCellCount: finalLeftVirtualCellCount,
            leftVirtualCellWidth: isProvideVirtualCellsWidth ? finalLeftVirtualCellWidth : void 0,
            rightVirtualCellCount: finalRightVirtualCellCount,
            rightVirtualCellWidth: isProvideVirtualCellsWidth ? finalRightVirtualCellWidth : void 0
        }
    }
}
