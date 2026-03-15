/**
 * DevExtreme (esm/ui/scheduler/workspaces/view_model/view_data_generator_timeline_month.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    ViewDataGenerator
} from "./view_data_generator";
import {
    calculateCellIndex
} from "../../../../renovation/ui/scheduler/view_model/to_test/views/utils/month";
import {
    calculateStartViewDate
} from "../../../../renovation/ui/scheduler/view_model/to_test/views/utils/timeline_month";
import {
    setOptionHour
} from "../../../../renovation/ui/scheduler/view_model/to_test/views/utils/base";
import dateUtils from "../../../../core/utils/date";
var DAY_IN_MILLISECONDS = dateUtils.dateToMilliseconds("day");
export class ViewDataGeneratorTimelineMonth extends ViewDataGenerator {
    _calculateCellIndex(rowIndex, columnIndex, rowCount, columnCount) {
        return calculateCellIndex(rowIndex, columnIndex, rowCount, columnCount)
    }
    calculateEndDate(startDate, interval, endDayHour) {
        return setOptionHour(startDate, endDayHour)
    }
    getInterval() {
        return DAY_IN_MILLISECONDS
    }
    _calculateStartViewDate(options) {
        return calculateStartViewDate(options.currentDate, options.startDayHour, options.startDate, options.intervalCount)
    }
    getCellCount(options) {
        var {
            intervalCount: intervalCount,
            currentDate: currentDate
        } = options;
        var cellCount = 0;
        for (var i = 1; i <= intervalCount; i++) {
            cellCount += new Date(currentDate.getFullYear(), currentDate.getMonth() + i, 0).getDate()
        }
        return cellCount
    }
    setHiddenInterval() {
        this.hiddenInterval = 0
    }
}
