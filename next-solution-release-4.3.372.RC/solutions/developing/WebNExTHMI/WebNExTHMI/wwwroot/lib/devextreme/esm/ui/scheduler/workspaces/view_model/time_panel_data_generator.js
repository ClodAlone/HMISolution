/**
 * DevExtreme (esm/ui/scheduler/workspaces/view_model/time_panel_data_generator.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import _extends from "@babel/runtime/helpers/esm/extends";
import _objectWithoutPropertiesLoose from "@babel/runtime/helpers/esm/objectWithoutPropertiesLoose";
var _excluded = ["allDay", "startDate", "endDate", "groups", "groupIndex", "isFirstGroupCell", "isLastGroupCell", "index"];
import {
    getDisplayedRowCount
} from "../../../../renovation/ui/scheduler/view_model/to_test/views/utils/base";
import {
    getTimePanelCellText
} from "../../../../renovation/ui/scheduler/view_model/to_test/views/utils/week";
export class TimePanelDataGenerator {
    constructor(viewDataGenerator) {
        this._viewDataGenerator = viewDataGenerator
    }
    getCompleteTimePanelMap(options, completeViewDataMap) {
        var {
            startViewDate: startViewDate,
            cellDuration: cellDuration,
            startDayHour: startDayHour,
            isVerticalGrouping: isVerticalGrouping,
            intervalCount: intervalCount,
            currentDate: currentDate,
            viewType: viewType,
            hoursInterval: hoursInterval,
            endDayHour: endDayHour
        } = options;
        var rowCountInGroup = this._viewDataGenerator.getRowCount({
            intervalCount: intervalCount,
            currentDate: currentDate,
            viewType: viewType,
            hoursInterval: hoursInterval,
            startDayHour: startDayHour,
            endDayHour: endDayHour
        });
        var cellCountInGroupRow = this._viewDataGenerator.getCellCount({
            intervalCount: intervalCount,
            currentDate: currentDate,
            viewType: viewType,
            hoursInterval: hoursInterval,
            startDayHour: startDayHour,
            endDayHour: endDayHour
        });
        var allDayRowsCount = 0;
        return completeViewDataMap.map((row, index) => {
            var _row$ = row[0],
                {
                    allDay: allDay,
                    startDate: startDate,
                    groups: groups,
                    groupIndex: groupIndex,
                    isFirstGroupCell: isFirstGroupCell,
                    isLastGroupCell: isLastGroupCell,
                    index: cellIndex
                } = _row$,
                restCellProps = _objectWithoutPropertiesLoose(_row$, _excluded);
            if (allDay) {
                allDayRowsCount += 1
            }
            var timeIndex = (index - allDayRowsCount) % rowCountInGroup;
            return _extends({}, restCellProps, {
                startDate: startDate,
                allDay: allDay,
                text: getTimePanelCellText(timeIndex, startDate, startViewDate, cellDuration, startDayHour),
                groups: isVerticalGrouping ? groups : void 0,
                groupIndex: isVerticalGrouping ? groupIndex : void 0,
                isFirstGroupCell: isVerticalGrouping && isFirstGroupCell,
                isLastGroupCell: isVerticalGrouping && isLastGroupCell,
                index: Math.floor(cellIndex / cellCountInGroupRow)
            })
        })
    }
    generateTimePanelData(completeTimePanelMap, options) {
        var {
            startRowIndex: startRowIndex,
            rowCount: rowCount,
            topVirtualRowHeight: topVirtualRowHeight,
            bottomVirtualRowHeight: bottomVirtualRowHeight,
            isGroupedAllDayPanel: isGroupedAllDayPanel,
            isVerticalGrouping: isVerticalGrouping,
            isAllDayPanelVisible: isAllDayPanelVisible
        } = options;
        var indexDifference = isVerticalGrouping || !isAllDayPanelVisible ? 0 : 1;
        var correctedStartRowIndex = startRowIndex + indexDifference;
        var displayedRowCount = getDisplayedRowCount(rowCount, completeTimePanelMap);
        var timePanelMap = completeTimePanelMap.slice(correctedStartRowIndex, correctedStartRowIndex + displayedRowCount);
        var timePanelData = {
            topVirtualRowHeight: topVirtualRowHeight,
            bottomVirtualRowHeight: bottomVirtualRowHeight,
            isGroupedAllDayPanel: isGroupedAllDayPanel
        };
        var {
            previousGroupedData: groupedData
        } = this._generateTimePanelDataFromMap(timePanelMap, isGroupedAllDayPanel);
        timePanelData.groupedData = groupedData;
        return timePanelData
    }
    _generateTimePanelDataFromMap(timePanelMap, isGroupedAllDayPanel) {
        return timePanelMap.reduce((_ref, cellData) => {
            var {
                previousGroupIndex: previousGroupIndex,
                previousGroupedData: previousGroupedData
            } = _ref;
            var currentGroupIndex = cellData.groupIndex;
            if (currentGroupIndex !== previousGroupIndex) {
                previousGroupedData.push({
                    dateTable: [],
                    isGroupedAllDayPanel: isGroupedAllDayPanel,
                    groupIndex: currentGroupIndex
                })
            }
            if (cellData.allDay) {
                previousGroupedData[previousGroupedData.length - 1].allDayPanel = cellData
            } else {
                previousGroupedData[previousGroupedData.length - 1].dateTable.push(cellData)
            }
            return {
                previousGroupIndex: currentGroupIndex,
                previousGroupedData: previousGroupedData
            }
        }, {
            previousGroupIndex: -1,
            previousGroupedData: []
        })
    }
}
