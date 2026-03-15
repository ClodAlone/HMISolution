/**
 * DevExtreme (esm/ui/scheduler/workspaces/view_model/view_data_generator_month.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    getToday,
    setOptionHour
} from "../../../../renovation/ui/scheduler/view_model/to_test/views/utils/base";
import {
    ViewDataGenerator
} from "./view_data_generator";
import dateUtils from "../../../../core/utils/date";
import {
    calculateCellIndex,
    calculateStartViewDate,
    getCellText,
    isFirstCellInMonthWithIntervalCount,
    getViewStartByOptions
} from "../../../../renovation/ui/scheduler/view_model/to_test/views/utils/month";
var DAY_IN_MILLISECONDS = dateUtils.dateToMilliseconds("day");
var DAYS_IN_WEEK = 7;
var WEEKS_IN_MONTH = 4;
export class ViewDataGeneratorMonth extends ViewDataGenerator {
    get tableAllDay() {
        return
    }
    getCellData(rowIndex, columnIndex, options, allDay) {
        var data = super.getCellData(rowIndex, columnIndex, options, false);
        var startDate = data.startDate;
        var {
            indicatorTime: indicatorTime,
            timeZoneCalculator: timeZoneCalculator,
            intervalCount: intervalCount
        } = options;
        data.today = this.isCurrentDate(startDate, indicatorTime, timeZoneCalculator);
        data.otherMonth = this.isOtherMonth(startDate, this._minVisibleDate, this._maxVisibleDate);
        data.firstDayOfMonth = isFirstCellInMonthWithIntervalCount(startDate, intervalCount);
        data.text = getCellText(startDate, intervalCount);
        return data
    }
    isCurrentDate(date, indicatorTime, timeZoneCalculator) {
        return dateUtils.sameDate(date, getToday(indicatorTime, timeZoneCalculator))
    }
    isOtherMonth(cellDate, minDate, maxDate) {
        return !dateUtils.dateInRange(cellDate, minDate, maxDate, "date")
    }
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
        return calculateStartViewDate(options.currentDate, options.startDayHour, options.startDate, options.intervalCount, this.getFirstDayOfWeek(options.firstDayOfWeek))
    }
    _setVisibilityDates(options) {
        var {
            intervalCount: intervalCount,
            startDate: startDate,
            currentDate: currentDate
        } = options;
        var firstMonthDate = dateUtils.getFirstMonthDate(startDate);
        var viewStart = getViewStartByOptions(startDate, currentDate, intervalCount, firstMonthDate);
        this._minVisibleDate = new Date(viewStart.setDate(1));
        var nextMonthDate = new Date(viewStart.setMonth(viewStart.getMonth() + intervalCount));
        this._maxVisibleDate = new Date(nextMonthDate.setDate(0))
    }
    getCellCount() {
        return DAYS_IN_WEEK
    }
    getRowCount(options) {
        return WEEKS_IN_MONTH * options.intervalCount + 2
    }
    getCellCountInDay() {
        return 1
    }
    setHiddenInterval() {
        this.hiddenInterval = 0
    }
}
